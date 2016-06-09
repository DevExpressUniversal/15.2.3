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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectTableCommand
	public class SelectTableCommand : RichEditSelectionCommand {
		public SelectTableCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SelectTableCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectTable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SelectTableCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SelectTableCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SelectTableCommandImageName")]
#endif
		public override string ImageName { get { return "SelectTable"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override void ChangeSelection(Selection selection) {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			Table table = ActivePieceTable.FindParagraph(selection.Start).GetCell().Table;
			Paragraph startParagraph = ActivePieceTable.Paragraphs[table.FirstRow.FirstCell.StartParagraphIndex];
			Paragraph endParagraph = ActivePieceTable.Paragraphs[table.LastRow.LastCell.EndParagraphIndex];
			selection.ClearMultiSelection();
			selection.Start = startParagraph.LogPosition;
			selection.End = endParagraph.EndLogPosition + 1;
			selection.SelectedCells = GetSelectedCells(table);
			ValidateSelection(selection, true);
		}
		protected internal virtual SelectedCellsCollection GetSelectedCells(Table table) {
			SelectedCellsCollection result = new SelectedCellsCollection();
			TableRowCollection rows = table.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = rows[i];
				result.AddSelectedCells(currentRow, 0, currentRow.Cells.Count - 1);
			}
			return result;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region SelectTablePlaceholderCommand
	public class SelectTablePlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public SelectTablePlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectTablePlaceholder; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableElements; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableElementsDescription; } }
		public override string ImageName { get { return "Select"; } }
		#endregion
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
		}
	}
	#endregion
}
