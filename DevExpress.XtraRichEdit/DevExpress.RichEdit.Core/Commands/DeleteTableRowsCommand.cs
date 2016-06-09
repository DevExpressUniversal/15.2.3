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
using System.Collections.Generic;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteTableRowsCommand
	public class DeleteTableRowsCommand : RichEditSelectionCommand {
		public DeleteTableRowsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteTableRows; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableRows; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableRowsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsCommandImageName")]
#endif
		public override string ImageName { get { return "DeleteTableRows"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			List<TableRow> selectedRows = DocumentModel.Selection.GetSelectedTableRows();
			PerformModifyModelCore(selectedRows);
		}
		protected internal virtual void PerformModifyModelCore(List<TableRow> selectedRows) {
			int selectedRowsCount = selectedRows.Count;
			if (selectedRowsCount <= 0)
				return;
			Table table = selectedRows[0].Table;
			for (int i = selectedRowsCount - 1; i >= 0; i--) {
				TableRow currentRow = selectedRows[i];
				ActivePieceTable.DeleteTableRowWithContent(currentRow);
			}
			table.NormalizeCellColumnSpans();
			DocumentModel.CheckIntegrity();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			Selection selection = DocumentModel.Selection;
			if (!selection.IsWholeSelectionInOneTable()) {
				state.Enabled = false;
				return;
			}
			UpdateUIStateEnabled(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal virtual void UpdateUIStateEnabled(ICommandUIState state) {
			SelectedCellsCollection cells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			int rowsCount = cells.RowsCount;
			state.Enabled = rowsCount > 0 && rowsCount < cells.First.Table.Rows.Count;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
	#region DeleteTableRowsMenuCommand
	public class DeleteTableRowsMenuCommand : RichEditMenuItemSimpleCommand {
		public DeleteTableRowsMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsMenuCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteTableRowsMenuItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsMenuCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableRows; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableRowsMenuCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableRowsDescription; } }
		#endregion
		protected internal virtual DeleteTableRowsCommand CreateInnerCommand() {
			return DocumentModel.CommandsCreationStrategy.CreateDeleteTableRowsCommand(Control);
		}
		protected internal override void ExecuteCore() {
			DeleteTableRowsCommand command = CreateInnerCommand();
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			DeleteTableRowsCommand command = CreateInnerCommand();
			command.UpdateUIState(state);
			if (state.Enabled == false) {
				state.Visible = false;
				return;
			}
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			state.Enabled = state.Enabled && cellsCollection.IsSelectedEntireTableRows();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
