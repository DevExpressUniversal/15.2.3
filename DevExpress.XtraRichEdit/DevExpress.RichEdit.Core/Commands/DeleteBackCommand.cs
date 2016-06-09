#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System.Collections.Generic;
using System;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteBackCoreCommand
	public class DeleteBackCoreCommand : DeleteCommandBase {
		public DeleteBackCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool ExtendSelection { get { return false; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBackCore; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBackCoreDescription; } }
		#endregion
		protected internal override void ModifyModel() {
			Selection selection = DocumentModel.Selection;
			List<TableRow> selectedRows = selection.GetSelectedTableRows();
			Table selectedTable = selectedRows.Count > 0 ? selectedRows[0].Table : null;
			SelectionRangeCollection sorted = selection.GetSortedSelectionCollection();
			if (!ValidateSelectionRanges(sorted))
				return;
			for (int i = sorted.Count - 1; i >= 0; i--) {
				SelectionRange selectionRange = sorted[i];
				DocumentLogPosition selectionStartPos = selectionRange.From;
				int selectionLength = selectionRange.Length;
				bool documentLastParagraphSelected = false;
				if (selectionStartPos + selectionLength > ActivePieceTable.DocumentEndLogPosition) {
					selectionLength = ActivePieceTable.DocumentEndLogPosition - selectionStartPos;
					documentLastParagraphSelected = true;
				}
				if (selectionLength > 0)
					DeleteContentCore(selectionStartPos, selectionLength, documentLastParagraphSelected, true);
				else {
					DocumentModelPosition selectionStart = GetSelectionStartBySelectionRange(selectionRange);
					if (selectionStart != null)
						DeleteBackProcess(selectionStart, selectionStartPos);
				}
			}
			if (selectedTable != null && selectedTable.Rows.Count == 0)
				ActivePieceTable.DeleteTableFromTableCollection(selectedTable);
			selection.ClearMultiSelection();
			RunIndex runIndex = selection.Interval.NormalizedStart.RunIndex;
			ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ValidateSelectionInterval, runIndex, runIndex);
		}
		DocumentModelPosition GetSelectionStartBySelectionRange(SelectionRange range) {
			System.Collections.Generic.List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				if (item.NormalizedStart == range.From && item.Length == range.Length)
					return item.Interval.NormalizedStart;
			}
			return null;
		}
		protected internal virtual void DeleteBackProcess(DocumentModelPosition selectionStart, DocumentLogPosition selectionStartPos) {
			Paragraph paragraph = ActivePieceTable.Paragraphs[selectionStart.ParagraphIndex];
			selectionStartPos--;
			if (selectionStartPos >= DocumentLogPosition.Zero) {
				if (IsDeletedNumeration(selectionStart, paragraph))
					ActivePieceTable.RemoveNumberingFromParagraph(paragraph);
				else
					DeleteContentCore(selectionStartPos, 1, false);
			} else {
				if (paragraph.IsInList())
					ActivePieceTable.RemoveNumberingFromParagraph(paragraph);
			}
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			Selection selection = DocumentModel.Selection;
			if (selection.Items.Count == 1 && selection.Items[0].Length == 0) {
				DocumentLogPosition start = DevExpress.Utils.Algorithms.Max(ActivePieceTable.DocumentStartLogPosition, selection.Items[0].Start - 1);
				return IsContentEditable && ActivePieceTable.CanEditRange(start, 1);
			}
			return base.CanChangePosition(pos);
		}
		protected internal virtual bool IsDeletedNumeration(DocumentModelPosition selectionStart, Paragraph paragraph) {
			return paragraph.IsInList() && paragraph.LogPosition == selectionStart.LogPosition;
		}
	}
	#endregion
	#region DeleteBackCommand
	public class DeleteBackCommand : MultiCommand {
		public DeleteBackCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBack; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBackDescription; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SelectFieldPrevToCaretCommand(Control));
			Commands.Add(DocumentModel.CommandsCreationStrategy.CreateDeleteBackCoreCommand(Control));
		}
	}
	#endregion
}
