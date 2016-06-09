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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Layout;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteTableCellsDispatcherCommand
	public class DeleteTableCellsDispatcherCommand : InsertDeleteTableCellsDispatcherCommandBase {
		public DeleteTableCellsDispatcherCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCellsDescription; } }
		public override string ImageName { get { return "DeleteTableCells"; } }
		#endregion
		protected internal override void TableCellsOperationWithShiftToTheHorizontally() {
			DeleteTableCellsWithShiftToTheHorizontallyCommand command = new DeleteTableCellsWithShiftToTheHorizontallyCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithShiftToTheVertically() {
			DeleteTableCellsWithShiftToTheVerticallyCommand command = new DeleteTableCellsWithShiftToTheVerticallyCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithRow() {
			DeleteTableRowsCommand command = new DeleteTableRowsCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithColumn() {
			DeleteTableColumnsCommand command = new DeleteTableColumnsCommand(Control);
			command.ExecuteCore();
		}
	}
	#endregion
	#region DeleteTableCellsWithShiftToTheHorizontallyCommand
	public class DeleteTableCellsWithShiftToTheHorizontallyCommand : RichEditSelectionCommand {
		public DeleteTableCellsWithShiftToTheHorizontallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCellsDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			if (IsSelectedEntireTable(cellsCollection)) {
				DeleteEntireTable();
				return;
			}
			Table table = cellsCollection.FirstSelectedCell.Table;
			int topRowIndex = cellsCollection.GetTopRowIndex();
			TableColumnWidthCalculator calculator = new TableColumnWidthCalculator(table, Control.InnerDocumentServer.Owner);
			TableWidthsContainer container = calculator.CalculateWidths();
			for (int i = cellsCollection.GetBottomRowIndex(); i >= topRowIndex; i--) {
				SelectedCellsIntervalInRow currentCellsInterval = cellsCollection[i];
				TableRow currentRow = currentCellsInterval.Row;
				if (IsSelectedEntireRow(currentCellsInterval)) {
					DeleteEntireRow(currentRow);
					continue;
				}
				TableCellCollection cellsInCurrentRow = currentRow.Cells;
				int startIndex = currentCellsInterval.NormalizedStartCellIndex;
				int endIndex = currentCellsInterval.NormalizedEndCellIndex;
				for (int j = endIndex; j >= startIndex; j--) {
					TableCell currentCell = cellsInCurrentRow[j];
					if (currentCell.VerticalMerging == MergingState.Continue)
						continue;
					ActivePieceTable.DeleteTableCellWithContentKnownWidths(currentCell, false, Control.InnerDocumentServer.Owner, container, true);
				}
			}
			table.Normalize(true);
			table.NormalizeRows();
		}
		protected internal virtual bool IsSelectedEntireTable(SelectedCellsCollection selectedCells) {
			return selectedCells.IsSelectedEntireTable();
		}
		protected internal virtual bool IsSelectedEntireRow(SelectedCellsIntervalInRow cellsInterval) {
			TableRow row = cellsInterval.Row;
			return cellsInterval.NormalizedStartCell == row.FirstCell && cellsInterval.NormalizedEndCell == row.LastCell;
		}
		protected internal virtual void DeleteEntireTable() {
			DeleteTableCommand command = new DeleteTableCommand(Control);
			command.PerformModifyModel();
		}
		protected internal virtual void DeleteEntireRow(TableRow deletedRow) {
			ActivePieceTable.DeleteTableRowWithContent(deletedRow);
			deletedRow.Table.NormalizeCellColumnSpans();
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
	#region DeleteTableCellsWithShiftToTheVerticallyCommand
	public class DeleteTableCellsWithShiftToTheVerticallyCommand : RichEditSelectionCommand {
		public DeleteTableCellsWithShiftToTheVerticallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCellsDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			ActivePieceTable.DeleteTableCellsWithShiftToTheUp((SelectedCellsCollection)DocumentModel.Selection.SelectedCells);
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
