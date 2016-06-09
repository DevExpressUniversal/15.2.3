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

using System;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteCoreCommand
	public class DeleteCoreCommand : DeleteCommandBase {
		public DeleteCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool ExtendSelection { get { return false; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Delete; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteDescription; } }
		#endregion
		protected internal override void ModifyModel() {
			Selection selection = DocumentModel.Selection;
			SelectionRangeCollection sorted = selection.GetSortedSelectionCollection();
			if (!ValidateSelectionRanges(sorted))
				return;
			for (int i = sorted.Count - 1; i >= 0; i--) {
				SelectionRange selectionRange = sorted[i];
				int selectionLength = Math.Max(1, Math.Abs(selectionRange.Length));
				DocumentLogPosition selectionStart = selectionRange.From;
				bool resetLastParagraphProperties = IsResetLastParagraph(selectionRange);
				bool documentLastParagraphSelected = false;
				if (selectionStart + selectionLength > ActivePieceTable.DocumentEndLogPosition) {
					selectionLength = ActivePieceTable.DocumentEndLogPosition - selectionStart;
					documentLastParagraphSelected = true;
				}
				if (selectionLength > 0)
					DeleteContentCore(selectionStart, selectionLength, documentLastParagraphSelected);
				if (resetLastParagraphProperties)
					ResetLastParagraphProperties();
			}
			selection.ClearMultiSelection();
			RunIndex runIndex = selection.Interval.NormalizedStart.RunIndex;
			ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ValidateSelectionInterval, runIndex, runIndex);
		}
		protected internal void ResetLastParagraphProperties() {
			TextRunBase run = ActivePieceTable.Runs.Last;
			run.CharacterStyleIndex = 0;
			run.CharacterProperties.Reset();
			Paragraph paragraph = ActivePieceTable.Paragraphs.Last;
			paragraph.ParagraphStyleIndex = 0;
			paragraph.ParagraphProperties.Reset();
			TabFormattingInfo tabs = paragraph.Tabs.GetTabs();
			tabs.Clear();
			paragraph.Tabs.SetTabs(tabs);
			if (paragraph.IsInList())
				ActivePieceTable.RemoveNumberingFromParagraph(paragraph);
		}
		protected internal virtual bool IsResetLastParagraph(SelectionRange selectionRange) {
			if(IsWholeDocumentSelected(selectionRange) )
				return true;
			if (selectionRange.Length == 0 && DocumentModel.ActivePieceTable.IsEmpty)
				return true;
			return false;
		}
		protected virtual bool IsWholeDocumentSelected(SelectionRange selectionRange) {
			DocumentLogPosition startLogPosition = selectionRange.From;
			DocumentLogPosition endLogPosition = selectionRange.From + selectionRange.Length;
			return startLogPosition == ActivePieceTable.DocumentStartLogPosition && endLogPosition == ActivePieceTable.DocumentEndLogPosition + 1;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			Selection selection = DocumentModel.Selection;
			if (selection.Items.Count == 1 && selection.Items[0].Length == 0)
				return IsContentEditable && ActivePieceTable.CanEditRange(selection.Items[0].Start, 1);
			return base.CanChangePosition(pos);
		}
	}
	#endregion
	#region CommentDeleteCoreCommand
	public class CommentDeleteCoreCommand : DeleteCoreCommand {
		public CommentDeleteCoreCommand(IRichEditControl control)
			: base(control) { 
		}
		protected internal override bool CanEditSelection() {
			if (!base.CanEditSelection())
				return false;
			return true;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			if (!base.CanChangePosition(pos))
				return false;
			return CanChangePositionCore();
		}
		bool CanChangePositionCore() {
			Selection selection = DocumentModel.Selection;
			BookmarkCollection bookmarks = ActiveView.DocumentModel.ActivePieceTable.Bookmarks;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				if ((selection.End) == bookmarks[i].End)
					return false;
			}
			return true;
		}
	}
	#endregion
	#region DeleteCommand
	public class DeleteCommand : MultiCommand {
		public DeleteCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.Delete; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Delete; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteDescription; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SelectFieldNextToCaretCommand(Control));
			Commands.Add(DocumentModel.CommandsCreationStrategy.CreateDeleteCoreCommand(Control));
		}
	}
	#endregion
}
