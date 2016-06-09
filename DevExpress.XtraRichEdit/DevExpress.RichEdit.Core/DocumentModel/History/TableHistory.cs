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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Commands;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region CreateEmptyTableHistoryItem
	public class CreateEmptyTableHistoryItem : RichEditHistoryItem {
		#region Fields
		int insertedTableIndex = -1;
		Table table;
		#endregion
		public CreateEmptyTableHistoryItem(PieceTable pieceTable, TableCell sourceCell)
			: base(pieceTable) {
			this.table = new Table(PieceTable, sourceCell, 0, 0);
		}
		#region Properties
		public int InsertedTableIndex { get { return insertedTableIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Tables.RemoveLast();
			insertedTableIndex = -1;
		}
		protected override void RedoCore() {
			PieceTable.Tables.Add(table);
			insertedTableIndex = PieceTable.Tables.Count - 1;
		}
	}
	#endregion
	#region CreateTableHistoryItem
	public class CreateTableHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly ParagraphIndex firstParagraphIndex;
		readonly int rowCount;
		readonly int cellCount;
		Table table;
		#endregion
		public CreateTableHistoryItem(PieceTable pieceTable, ParagraphIndex firstParagraphIndex, int rowCount, int cellCount)
			: base(pieceTable) {
			this.firstParagraphIndex = firstParagraphIndex;
			this.rowCount = rowCount;
			this.cellCount = cellCount;
		}
		#region Properties
		public ParagraphIndex FirstParagraphIndex { get { return firstParagraphIndex; } }
		public int RowCount { get { return rowCount; } }
		public int CellCount { get { return cellCount; } }
		#endregion
		public override void Execute() {
			this.table = PieceTable.TableCellsManager.CreateTableCore(FirstParagraphIndex, RowCount, CellCount);
		}
		protected override void UndoCore() {
			PieceTable.TableCellsManager.RemoveTable(table);
			Selection selection = DocumentModel.Selection;
			selection.SetStartCell(selection.NormalizedStart);
		}
		protected override void RedoCore() {
			PieceTable.TableCellsManager.InsertTable(table);
		}
	}
	#endregion
	#region TableConditionalFormattingController
	public class TableConditionalFormattingController {
		readonly Table table;
		readonly PieceTable pieceTable;
		public TableConditionalFormattingController(Table table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.pieceTable = table.PieceTable;
		}
		public static void ResetTablesCachedProperties(DocumentModel documentModel) {
			documentModel.GetPieceTables(true).ForEach(ResetTablesCachedProperties);
		}
		static void ResetTablesCachedProperties(PieceTable pieceTable) {
			pieceTable.Tables.ForEach(ResetTableCachedProperties);
		}
		static void ResetTableCachedProperties(Table table) {
			TableConditionalFormattingController controller = new TableConditionalFormattingController(table);
			controller.ResetCachedProperties(0);
		}
		public void ResetCachedProperties(int startRowIndex) {
			startRowIndex = Math.Max(0, startRowIndex);
			TableRowCollection rows = table.Rows;
			int count = rows.Count;
			if (startRowIndex == count && startRowIndex > 0)
				startRowIndex--;
			for (int i = startRowIndex; i < count; i++) {
				ResetRowCachedProperties(rows[i]);
			}
		}
		void ResetRowCachedProperties(TableRow row) {
			TableCellCollection cells = row.Cells;
			row.ResetConditionalType();
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				TableCell cell = cells[i];
				cell.ResetConditionalType();
				if (ShouldResetCellCachedIndicies(cell))
					ResetParagraphsInCell(cell);
			}
		}
		bool ShouldResetCellCachedIndicies(TableCell cell) {
			return true;
		}
		void ResetParagraphsInCell(TableCell cell) {
			ParagraphIndex endIndex = cell.EndParagraphIndex;
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			if (endIndex < ParagraphIndex.Zero)
				return;
			endIndex = Algorithms.Min(new ParagraphIndex(pieceTable.Paragraphs.Count - 1), endIndex);
			for (ParagraphIndex i = cell.StartParagraphIndex; i <= endIndex; i++)
				paragraphs[i].ResetCachedIndices(ResetFormattingCacheType.All);
		}
	}
	#endregion
	#region InsertEmptyTableRowHistoryItem
	public class InsertEmptyTableRowHistoryItem : RichEditHistoryItem {
		#region Fields
		int rowIndex = -1;
		TableRow row;
		Table table;
		#endregion
		public InsertEmptyTableRowHistoryItem(PieceTable pieceTable, Table table, int rowIndex)
			: base(pieceTable) {
			Guard.ArgumentNotNull(table, "table");
			Guard.ArgumentNonNegative(rowIndex, "row index");
			this.rowIndex = rowIndex;
			this.table = table;
		}
		#region Properties
		public int InsertedRowIndex { get { return rowIndex; } }
		public Table Table { get { return table; } }
		#endregion
		public override void Execute() {
			row = new TableRow(Table, 0);
			RedoCore();
		}
		protected override void UndoCore() {
			Debug.Assert(Table.Rows[rowIndex] == row);
			Table.Rows.DeleteRowCore(rowIndex);
			new TableConditionalFormattingController(Table).ResetCachedProperties(rowIndex);
		}		
		protected override void RedoCore() {
			new TableConditionalFormattingController(Table).ResetCachedProperties(rowIndex);
			Table.Rows.AddRowCore(rowIndex, row);			
		}
	}
	#endregion
	#region InsertEmptyTableCellHistoryItem
	public class InsertEmptyTableCellHistoryItem : RichEditHistoryItem {
		#region Fields
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		TableCell insertedCell;
		TableRow row;
		int insertedIndex;
		#endregion
		public InsertEmptyTableCellHistoryItem(PieceTable pieceTable, TableRow row, int insertedIndex, ParagraphIndex start, ParagraphIndex end)
			: base(pieceTable) {
			Guard.ArgumentNotNull(row, "tableRow");
			if (start < ParagraphIndex.Zero)
				Exceptions.ThrowArgumentException("start", start);
			if (end < ParagraphIndex.Zero)
				Exceptions.ThrowArgumentException("end", end);
			this.row = row;
			this.startParagraphIndex = start;
			this.endParagraphIndex = end;
			this.insertedIndex = insertedIndex;
		}
		public Table Table { get { return Row.Table; } }
		public TableRow Row { get { return row; } }
		public TableCell InsertedCell { get { return insertedCell; } }
		public override void Execute() {
			insertedCell = new TableCell(Row);
			RedoCore();
		}
		protected override void RedoCore() {
			Row.Cells.AddCellCore(insertedIndex, InsertedCell);
			PieceTable.TableCellsManager.InitializeTableCell(insertedCell, startParagraphIndex, endParagraphIndex);
			new TableConditionalFormattingController(Table).ResetCachedProperties(row.IndexInTable);
		}
		protected override void UndoCore() {
			PieceTable.TableCellsManager.RemoveTableCell(InsertedCell);
			Row.Cells.DeleteInternal(InsertedCell);
			new TableConditionalFormattingController(Table).ResetCachedProperties(row.IndexInTable);
		}
	}
	#endregion
	#region ChangeTableStyleIndexHistoryItem
	public class ChangeTableStyleIndexHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int oldIndex;
		readonly int newIndex;
		readonly int tableIndex;
		#endregion
		public ChangeTableStyleIndexHistoryItem(PieceTable pieceTable, int tableIndex, int oldStyleIndex, int newStyleIndex)
			: base(pieceTable) {
			this.oldIndex = oldStyleIndex;
			this.newIndex = newStyleIndex;
			this.tableIndex = tableIndex;
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public int TableIndex { get { return tableIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Tables[TableIndex].SetTableStyleIndexCore(OldIndex);
		}
		protected override void RedoCore() {
			PieceTable.Tables[TableIndex].SetTableStyleIndexCore(NewIndex);
		}
	}
	#endregion
	#region ChangeTableCellStyleIndexHistoryItem
	public class ChangeTableCellStyleIndexHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int oldIndex;
		readonly int newIndex;
		readonly int tableIndex;
		readonly int rowIndex;
		readonly int columnIndex;
		#endregion
		public ChangeTableCellStyleIndexHistoryItem(PieceTable pieceTable, int tableIndex, int rowIndex, int columnIndex, int oldStyleIndex, int newStyleIndex)
			: base(pieceTable) {
			this.oldIndex = oldStyleIndex;
			this.newIndex = newStyleIndex;
			this.tableIndex = tableIndex;
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public int TableIndex { get { return tableIndex; } }
		public int RowIndex { get { return rowIndex; } }
		public int ColumnIndex { get { return columnIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Tables[TableIndex].Rows[rowIndex].Cells[columnIndex].SetTableCellStyleIndexCore(OldIndex);
		}
		protected override void RedoCore() {
			PieceTable.Tables[TableIndex].Rows[rowIndex].Cells[columnIndex].SetTableCellStyleIndexCore(NewIndex);
		}
	}
	#endregion
	#region Comments
	#endregion
	#region TableRowCollectionHistoryItemBase (abstract class)
	public abstract class TableRowCollectionHistoryItemBase : RichEditHistoryItem {
		#region Fields
		readonly int tableIndex;
		readonly int rowIndex;
		#endregion
		protected TableRowCollectionHistoryItemBase(PieceTable pieceTable, int tableIndex, int rowIndex)
			: base(pieceTable) {
			Guard.ArgumentNonNegative(tableIndex, "tableIndex");
			Guard.ArgumentNonNegative(rowIndex, "rowIndex");
			this.tableIndex = tableIndex;
			this.rowIndex = rowIndex;
		}
		#region Properties
		public int TableIndex { get { return tableIndex; } }
		public int RowIndex { get { return rowIndex; } }
		public Table Table { get { return PieceTable.Tables[TableIndex]; } }
		public TableRow Row { get { return Table.Rows[RowIndex]; } }
		#endregion
	}
	#endregion
	#region DeleteTableHistoryItem
	public class DeleteTableHistoryItem : RichEditHistoryItem {
		Table deletedTable;
		public DeleteTableHistoryItem(PieceTable pieceTable, Table deletedTable)
			: base(pieceTable) {
			Guard.ArgumentNotNull(deletedTable, "DeletedTable");
			Guard.ArgumentNotNull(deletedTable.FirstRow, "FirstRow");
			Guard.ArgumentNotNull(deletedTable.FirstRow.FirstCell, "FirstCell");
			this.deletedTable = deletedTable;
		}
		Table DeletedTable { get { return deletedTable; } }
		protected override void RedoCore() {
			ParagraphIndex startParagraph = DeletedTable.FirstRow.FirstCell.StartParagraphIndex;
			DocumentLogPosition position = PieceTable.Paragraphs[Algorithms.Min(startParagraph, new ParagraphIndex(PieceTable.Paragraphs.Count - 1))].LogPosition;
			PieceTable.TableCellsManager.RemoveTable(DeletedTable);
			PieceTable.DocumentModel.Selection.SetStartCell(position);
		}
		protected override void UndoCore() {
			PieceTable.TableCellsManager.InsertTable(DeletedTable);
		}
	}
	#endregion
	#region DeleteTableRowHistoryItem
	public class DeleteTableRowHistoryItem : TableRowCollectionHistoryItemBase {
		#region Fields
		TableRow deletedRow;
		RunIndex runIndex;
		#endregion
		public DeleteTableRowHistoryItem(PieceTable pieceTable, int tableIndex, int rowIndex)
			: base(pieceTable, tableIndex, rowIndex) {
			TableRowCollection rows = Table.Rows;
			this.deletedRow = rows[RowIndex];
		}
		#region Properties
		TableRow DeletedRow { get { return deletedRow; } }
		protected internal RunIndex RunIndex { get { return runIndex; } set { runIndex = value; } }
		#endregion
		protected override void RedoCore() {
			PieceTable.ApplyChanges(DocumentModelChangeType.DeleteContent, RunIndex, RunIndex.MaxValue);
			TableRowCollection rows = Table.Rows;
			rows.DeleteRowCore(RowIndex);
			new TableConditionalFormattingController(Table).ResetCachedProperties(RowIndex);
		}
		protected override void UndoCore() {
			TableRowCollection rows = Table.Rows;
			rows.AddRowCore(RowIndex, DeletedRow);
			new TableConditionalFormattingController(Table).ResetCachedProperties(RowIndex);
		}
	}
	#endregion
	#region DeleteEmptyTableCellHistoryItem
	public class DeleteEmptyTableCellHistoryItem : TableRowCollectionHistoryItemBase {
		TableCell cell;
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		int cellIndex;
		public DeleteEmptyTableCellHistoryItem(PieceTable pieceTable, int tableIndex, int rowIndex, int cellIndex)
			: base(pieceTable, tableIndex, rowIndex) {
			this.cellIndex = cellIndex;
			this.cell = Table.Rows[RowIndex].Cells[cellIndex];
			this.startParagraphIndex = cell.StartParagraphIndex;
			this.endParagraphIndex = cell.EndParagraphIndex;
		}
		protected override void RedoCore() {
			RunIndex runIndex = PieceTable.Paragraphs[Table.Rows.First.Cells.First.StartParagraphIndex].FirstRunIndex;
			PieceTable.ApplyChanges(DocumentModelChangeType.DeleteContent, runIndex, RunIndex.MaxValue);
			PieceTable.TableCellsManager.RemoveTableCell(cell);
			Table.Rows[RowIndex].Cells.DeleteInternal(cell);
			NormalizeSelectedCellsInSelection();
			new TableConditionalFormattingController(Table).ResetCachedProperties(RowIndex);
		}
		protected internal void NormalizeSelectedCellsInSelection() {
			if (DocumentModel.Selection.SelectedCells is StartSelectedCellInTable)
				return;
			SelectedCellsCollection selectedCells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			int selectedRowsCount = selectedCells.RowsCount;
			if (selectedRowsCount == 1 && selectedCells.IsNotEmpty && selectedCells.First.IsContainsOnlyOneCell) {
				DocumentModel.Selection.SetStartCell(PieceTable.Paragraphs[startParagraphIndex].LogPosition);
				return;
			}
			for (int i = selectedRowsCount - 1; i >= 0; i--) {
				SelectedCellsIntervalInRow cellsInRow = selectedCells[i];
				if (cellsInRow.Row == cell.Row) {
					NormalizeSelectedCellsInSelectionCore(cellsInRow, selectedCells);
					break;
				}
			}
		}
		protected internal void NormalizeSelectedCellsInSelectionCore(SelectedCellsIntervalInRow cellsInRow, SelectedCellsCollection selectedCells) {
			if (cellsInRow.StartCellIndex == cellIndex) {
				if (cellsInRow.IsContainsOnlyOneCell) {
					selectedCells.Remove(cellsInRow);
					return;
				}
				if (cellsInRow.StartCellIndex < cellsInRow.EndCellIndex)
					cellsInRow.StartCellIndex++;
				else
					cellsInRow.StartCellIndex--;
				return;
			}
			if (cellsInRow.EndCellIndex == cellIndex)
				if (cellsInRow.EndCellIndex > cellsInRow.StartCellIndex)
					cellsInRow.EndCellIndex--;
				else
					cellsInRow.EndCellIndex++;
		}
		protected override void UndoCore() {
			PieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, endParagraphIndex);
			Table.Rows[RowIndex].Cells.AddCellCore(cellIndex, cell);
			new TableConditionalFormattingController(Table).ResetCachedProperties(RowIndex);
		}
	}
	#endregion
	#region ConvertParagraphsIntoTableRowHistoryItem
	public class ConvertParagraphsIntoTableRowHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly ParagraphIndex paragraphIndex;
		readonly int paragraphCount;
		TableRow row;
		TableCell[] newCells;
		#endregion
		public ConvertParagraphsIntoTableRowHistoryItem(PieceTable pieceTable, TableRow row, ParagraphIndex paragraphIndex, int paragraphCount)
			: base(pieceTable) {
			Guard.ArgumentNotNull(row, "row");
			Guard.ArgumentNonNegative(paragraphCount, "paragraphCount");
			this.row = row;
			this.paragraphIndex = paragraphIndex;
			this.paragraphCount = paragraphCount;
		}
		#region Properties
		public Table Table { get { return TableRow.Table; } }
		public TableRow TableRow { get { return row; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public int ParagraphCount { get { return paragraphCount; } }
		#endregion
		public override void Execute() {
			this.newCells = new TableCell[paragraphCount];
			ParagraphIndex count = paragraphIndex + paragraphCount;
			for (ParagraphIndex i = paragraphIndex; i < count; i++) {
				TableCell cell = new TableCell(row);
				newCells[i - paragraphIndex] = cell;
			}
			RedoCore();
		}
		protected override void UndoCore() {
			PieceTable.TableCellsManager.RevertParagraphsFromTableCells(TableRow);
		}
		protected override void RedoCore() {
			ParagraphIndex count = ParagraphIndex + ParagraphCount;
			for (ParagraphIndex i = paragraphIndex; i < count; i++) {
				TableCell cell = newCells[i - paragraphIndex];
				row.Cells.AddInternal(cell);
				PieceTable.TableCellsManager.InitializeTableCell(cell, i, i);
			}
		}
	}
	#endregion
	#region ChangeCellParagraphIndexCoreHistoryItem (abstract class)
	public abstract class ChangeCellParagraphIndexCoreHistoryItem : RichEditHistoryItem {
		#region Fields
		int tableCellIndex;
		int tableRowIndex;
		int tableIndex;
		readonly ParagraphIndex paragraphIndex;
		ParagraphIndex previousParagraphIndex = new ParagraphIndex(-1);
		#endregion
		protected ChangeCellParagraphIndexCoreHistoryItem(PieceTable pieceTable, TableCell tableCell, ParagraphIndex paragraphIndex)
			: base(pieceTable) {
			Guard.ArgumentNotNull(tableCell, "tableCell");
			this.SetTableCell(tableCell);
			this.paragraphIndex = paragraphIndex;
		}
		#region Properties
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		protected ParagraphIndex PreviousParagraphIndex { get { return previousParagraphIndex; } }
		int TableCellIndex { get { return tableCellIndex; } }
		int TableIndex { get { return tableIndex; } }
		Table Table { get { return (TableIndex >= 0) ? PieceTable.Tables[TableIndex] : null; } }
		protected TableCell TableCell { get { return (tableRowIndex >= 0 && tableCellIndex >= 0) ? Table.Rows[tableRowIndex].Cells[TableCellIndex] : null; } }
		#endregion
		protected void SetPreviousParagraphIndex(ParagraphIndex index) {
			this.previousParagraphIndex = index;
		}
		public void SetTableCell(TableCell cell) {
			if (cell == null) {
				this.tableCellIndex = -1;
				this.tableRowIndex = -1;
				this.tableCellIndex = -1;
				return;
			}
			Table table = cell.Table;
			this.tableIndex = table.PieceTable.Tables.IndexOf(table);
			TableRow row = cell.Row;
			this.tableRowIndex = table.Rows.IndexOf(row);
			this.tableCellIndex = row.Cells.IndexOf(cell);
		}
	}
	#endregion
	#region ChangeCellEndParagraphIndexHistoryItem
	public class ChangeCellEndParagraphIndexHistoryItem : ChangeCellParagraphIndexCoreHistoryItem {
		public ChangeCellEndParagraphIndexHistoryItem(PieceTable pieceTable, TableCell tableCell, ParagraphIndex paragraphIndex)
			: base(pieceTable, tableCell, paragraphIndex) {
			SetPreviousParagraphIndex(tableCell.EndParagraphIndex);
		}
		protected override void UndoCore() {
			TableCell.EndParagraphIndex = PreviousParagraphIndex;
		}
		protected override void RedoCore() {
			TableCell.EndParagraphIndex = ParagraphIndex;
		}
	}
	#endregion
	#region ChangeCellStartParagraphIndexHistoryItem
	public class ChangeCellStartParagraphIndexHistoryItem : ChangeCellParagraphIndexCoreHistoryItem {
		public ChangeCellStartParagraphIndexHistoryItem(PieceTable pieceTable, TableCell tableCell, ParagraphIndex paragraphIndex)
			: base(pieceTable, tableCell, paragraphIndex) {
			SetPreviousParagraphIndex(tableCell.StartParagraphIndex);
		}
		protected override void UndoCore() {
			TableCell.StartParagraphIndex = PreviousParagraphIndex;
		}
		protected override void RedoCore() {
			TableCell.StartParagraphIndex = ParagraphIndex;
		}
	}
	#endregion
	#region SelectTableColumnsHistoryItem
	public class SelectTableColumnsHistoryItem : RichEditHistoryItem {
		int tableIndex;
		int startColumnIndex;
		int endColumnIndex;
		SelectionPersistentInfo selectionInfo;
		IRichEditControl control;
		public SelectTableColumnsHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int TableIndex { get { return tableIndex; } set { tableIndex = value; } }
		public int StartColumnIndex { get { return startColumnIndex; } set { startColumnIndex = value; } }
		public int EndColumnIndex { get { return endColumnIndex; } set { endColumnIndex = value; } }
		public IRichEditControl Control { get { return control; } set { control = value; } }
		protected override void UndoCore() {
			DocumentModel.Selection.RestoreSelection(selectionInfo);
		}
		protected override void RedoCore() {
			this.selectionInfo = DocumentModel.Selection.GetSelectionPersistentInfo();
			SelectTableColumnsCommand command = new SelectTableColumnsCommand(Control);
			command.Rows = PieceTable.Tables[tableIndex].Rows;
			command.StartColumnIndex = StartColumnIndex;
			command.EndColumnIndex = EndColumnIndex;
			command.ChangeSelection(DocumentModel.Selection);
		}
	}
	#endregion
	#region SelectTableRowHistoryItemm
	public class SelectTableRowHistoryItem : RichEditHistoryItem {
		int tableIndex;
		int startRowIndex;
		int endRowIndex;
		SelectionPersistentInfo selectionInfo;
		IRichEditControl control;
		public SelectTableRowHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int TableIndex { get { return tableIndex; } set { tableIndex = value; } }
		public int StartRowIndex { get { return startRowIndex; } set { startRowIndex = value; } }
		public int EndRowIndex { get { return endRowIndex; } set { endRowIndex = value; } }
		public IRichEditControl Control { get { return control; } set { control = value; } }
		protected override void UndoCore() {
			DocumentModel.Selection.RestoreSelection(selectionInfo);
		}
		protected override void RedoCore() {
			this.selectionInfo = DocumentModel.Selection.GetSelectionPersistentInfo();
			SelectTableRowCommand command = new SelectTableRowCommand(Control);
			command.Rows = PieceTable.Tables[tableIndex].Rows;
			command.StartRowIndex = StartRowIndex;
			command.EndRowIndex = EndRowIndex;
			command.ChangeSelection(DocumentModel.Selection);
		}
	}
	#endregion
	#region MoveTableRowToOtherTableHistoryItem
	public class MoveTableRowToOtherTableHistoryItem : RichEditHistoryItem {
		Table targetTable;
		TableRow row;
		Table previousTable;
		public MoveTableRowToOtherTableHistoryItem(PieceTable pieceTable, Table targetTable, TableRow row)
			: base(pieceTable) {
			this.targetTable = targetTable;
			this.row = row;
			this.previousTable = row.Table;
		}
		protected override void RedoCore() {
			targetTable.Rows.AddInternal(row);
			row.SetTable(targetTable);
			new TableConditionalFormattingController(targetTable).ResetCachedProperties(targetTable.Rows.Count - 1);
		}
		protected override void UndoCore() {
			int index = targetTable.Rows.IndexOf(row);
			targetTable.Rows.DeleteRowCore(index);
			new TableConditionalFormattingController(targetTable).ResetCachedProperties(index);
			row.SetTable(previousTable);
		}
	}
	#endregion
	#region DeleteTableFromTableCollectionHistoryItem
	public class DeleteTableFromTableCollectionHistoryItem : RichEditHistoryItem {
		Table deletedTable;
		public DeleteTableFromTableCollectionHistoryItem(PieceTable pieceTable, Table deletedTable)
			: base(pieceTable) {
			Guard.ArgumentNotNull(deletedTable, "DeletedTable");
			this.deletedTable = deletedTable;
		}
		public Table DeletedTable { get { return deletedTable; } }
		protected override void RedoCore() {
			int index = PieceTable.Tables.IndexOf(deletedTable);
			PieceTable.Tables.Remove(index);
			PieceTable.DocumentModel.InvalidateDocumentLayout();
		}
		protected override void UndoCore() {
			PieceTable.Tables.Add(deletedTable);
			PieceTable.DocumentModel.InvalidateDocumentLayout();
		}
	}
	#endregion
}
