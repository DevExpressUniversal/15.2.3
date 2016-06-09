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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.TableLayout;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertDeleteTableCellsDispatcherCommandBase (abstract class)
	public abstract class InsertDeleteTableCellsDispatcherCommandBase : RichEditMenuItemSimpleCommand {
		#region Fields
		TableCellsParameters cellsParameters;
		#endregion
		protected InsertDeleteTableCellsDispatcherCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public TableCellsParameters CellsParameters { get { return cellsParameters; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			DefaultValueBasedCommandUIState<TableCellsParameters> valueState = state as DefaultValueBasedCommandUIState<TableCellsParameters>;
			if (valueState == null || valueState.Value == null)
				return;
			this.cellsParameters = valueState.Value;
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			switch (CellsParameters.CellOperation) {
				default:
				case TableCellOperation.ShiftToTheHorizontally:
					TableCellsOperationWithShiftToTheHorizontally();
					break;
				case TableCellOperation.ShiftToTheVertically:
					TableCellsOperationWithShiftToTheVertically();
					break;
				case TableCellOperation.RowOperation:
					TableCellsOperationWithRow();
					break;
				case TableCellOperation.ColumnOperation:
					TableCellsOperationWithColumn();
					break;
			}
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal abstract void TableCellsOperationWithShiftToTheHorizontally();
		protected internal abstract void TableCellsOperationWithShiftToTheVertically();
		protected internal abstract void TableCellsOperationWithRow();
		protected internal abstract void TableCellsOperationWithColumn();
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
	#region InsertTableCellsDispatcherCommand
	public class InsertTableCellsDispatcherCommand : InsertDeleteTableCellsDispatcherCommandBase {
		public InsertTableCellsDispatcherCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCellsDescription; } }
		public override string ImageName { get { return "InsertTableCells"; } }
		#endregion
		protected internal override void TableCellsOperationWithShiftToTheHorizontally() {
			InsertTableCellsWithShiftToTheHorizontallyCommand command = new InsertTableCellsWithShiftToTheHorizontallyCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithShiftToTheVertically() {
			InsertTableCellsWithShiftToTheVerticallyCommand command = new InsertTableCellsWithShiftToTheVerticallyCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithRow() {
			InsertTableRowAboveCommand command = new InsertTableRowAboveCommand(Control);
			command.ExecuteCore();
		}
		protected internal override void TableCellsOperationWithColumn() {
			InsertTableColumnToTheLeftCommand command = new InsertTableColumnToTheLeftCommand(Control);
			command.ExecuteCore();
		}
	}
	#endregion
	#region InsertTableCellsWithShiftToTheHorizontallyCommand
	public class InsertTableCellsWithShiftToTheHorizontallyCommand : RichEditSelectionCommand {
		public InsertTableCellsWithShiftToTheHorizontallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCellsDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			int topRowIndex = cellsCollection.GetTopRowIndex();
			bool forceVisible = GetForceVisible();
			for (int i = cellsCollection.GetBottomRowIndex(); i >= topRowIndex; i--) {
				SelectedCellsIntervalInRow currentCellsInterval = cellsCollection[i];
				int startIndex = currentCellsInterval.NormalizedStartCellIndex;
				int endIndex = currentCellsInterval.NormalizedEndCellIndex;
				for (int j = endIndex; j >= startIndex; j--) {
					ActivePieceTable.InsertTableCellToTheLeft(currentCellsInterval.NormalizedStartCell, forceVisible, Control.InnerDocumentServer.Owner);
				}
			}
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
	#region InsertTableCellsWithShiftToTheVerticallyCommand
	public class InsertTableCellsWithShiftToTheVerticallyCommand : RichEditSelectionCommand {
		public InsertTableCellsWithShiftToTheVerticallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCellsDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			int topBottomRowIndex = cellsCollection.GetBottomRowIndex();
			for (int i = cellsCollection.GetTopRowIndex(); i <= topBottomRowIndex; i++) {
				SelectedCellsIntervalInRow currentCellsInterval = cellsCollection[i];
				TableCellsOperationWithShiftToTheVerticallyCore(currentCellsInterval);
			}
		}
		protected internal void TableCellsOperationWithShiftToTheVerticallyCore(SelectedCellsIntervalInRow cellsInterval) {
			int insertedRowsCount = CalculateInsertedRowsCount(cellsInterval);
			TableRow currentRow = cellsInterval.Row;
			bool forceVisible = GetForceVisible();
			for (int i = 0; i < insertedRowsCount; i++) {
				ActivePieceTable.InsertTableRowBelow(currentRow.Table.Rows.Last, forceVisible);
				TableCellCollection cellsInCurrentRow = currentRow.Cells;
				int startIndex = cellsInterval.NormalizedStartCellIndex;
				int endIndex = cellsInterval.NormalizedEndCellIndex;
				for (int j = endIndex; j >= startIndex; j--) {
					TableCell currentCell = cellsInCurrentRow[j];
					ActivePieceTable.InsertTableCellWithShiftToTheDown(currentCell, forceVisible, Control.InnerDocumentServer.Owner);
				}
				currentRow = currentRow.Next;
				if (currentRow == null)
					return;
			}
			DeleteTextInCells(cellsInterval.Row, insertedRowsCount);
		}
		protected internal int CalculateInsertedRowsCount(SelectedCellsIntervalInRow cellsInterval) {
			int result = Int32.MaxValue;
			TableCellCollection cells = cellsInterval.Row.Cells;
			int endCellIndex = cellsInterval.EndCellIndex;
			for (int i = cellsInterval.StartCellIndex; i <= endCellIndex; i++) {
				TableCell currentCell = cells[i];
				result = Math.Min(result, TableCellVerticalBorderCalculator.GetVerticalSpanCells(currentCell, false).Count);
			}
			return result;
		}
		protected internal void DeleteTextInCells(TableRow row, int rowsCount) {
			int startIndex = row.IndexInTable + 1;
			int endIndex = startIndex + rowsCount;
			TableRowCollection rows = row.Table.Rows;
			for (int i = startIndex; i < endIndex; i++) {
				TableRow currentRow = rows[i];
				TableCellCollection cells = currentRow.Cells;
				int cellsCount = cells.Count;
				for (int j = 0; j < cellsCount; j++) {
					TableCell currentCell = cells[j];
					if (currentCell.VerticalMerging == MergingState.Continue)
						DeleteContentInCells(currentCell);
				}
			}
		}
		protected internal void DeleteContentInCells(TableCell cell) {
			RunInfo runInfo = ActivePieceTable.GetRunInfoByTableCell(cell);
			DocumentLogPosition startLogPosition = runInfo.Start.LogPosition;
			ActivePieceTable.DeleteContent(startLogPosition, runInfo.End.LogPosition - startLogPosition + 1, false);
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
