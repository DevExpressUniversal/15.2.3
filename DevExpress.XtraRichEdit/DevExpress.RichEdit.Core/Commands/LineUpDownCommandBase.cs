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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region LineUpDownCommandBase (abstract class)
	public abstract class LineUpDownCommandBase : RichEditSelectionCommand {
		protected LineUpDownCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Row; } }
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			NextCaretPositionVerticalDirectionCalculator calculator = CreateNextCaretPositionCalculator();
			DocumentModelPosition newPosition = calculator.CalculateNextPosition(CaretPosition);
			if (Object.ReferenceEquals(newPosition, null))
				return pos.LogPosition;
			return GetLeftVisibleLogPosition(newPosition);
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal abstract NextCaretPositionVerticalDirectionCalculator CreateNextCaretPositionCalculator();
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region NextCaretPositionVerticalDirectionCalculator (abstract class)
	public abstract class NextCaretPositionVerticalDirectionCalculator {
		#region Fields
		readonly IRichEditControl control;
		bool fixedDirection = true;
		#endregion
		protected NextCaretPositionVerticalDirectionCalculator(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public PieceTable ActivePieceTable { get { return control.InnerDocumentServer.DocumentModel.ActivePieceTable; } }
		public bool FixedDirection { get { return fixedDirection; } set { fixedDirection = value; } }
		protected internal virtual RichEditView View { get { return Control.InnerControl.ActiveView; } }
		#endregion
		public virtual DocumentModelPosition CalculateNextPosition(CaretPosition caretPosition) {
			View.CheckExecutedAtUIThread();
			caretPosition.Update(DocumentLayoutDetailsLevel.Character);
			int x = caretPosition.LayoutPosition.Box.ActualSizeBounds.X;
			Debug.Assert(caretPosition.PageViewInfo != null);
			Row targetRow = GetTargetLayoutRowObtainTargetPageViewInfo(caretPosition, ref x);
			if (targetRow == null)
				return GetDefaultPosition();
			if (!Object.ReferenceEquals(ActivePieceTable, targetRow.Paragraph.PieceTable)) 
				return null;
			CharacterBox character = GetTargetCharacter(targetRow, x);
			return character.GetFirstPosition(ActivePieceTable);
		}
		protected internal virtual DocumentModelPosition GetDefaultPosition() {
			return null;
		}
		protected internal virtual Row GetTargetLayoutRowObtainTargetPageViewInfo(CaretPosition pos, ref int caretX) {
			DocumentLayoutPosition layoutPosition = pos.LayoutPosition;
			Row layoutPositionRow = layoutPosition.Row;
			int rowIndexInColumn = layoutPosition.Column.Rows.IndexOf(layoutPositionRow);
			Debug.Assert(rowIndexInColumn >= 0);
			TableCellViewInfo sourceCellViewInfo = layoutPosition.TableCell;
			CorrectRowAndCellViewInfo(pos, ref rowIndexInColumn, ref sourceCellViewInfo);
			CorrectCaretXOnExtendSelection(sourceCellViewInfo, ref caretX);
			return GetTargetLayoutRowObtainTargetPageViewInfoCore(pos.PageViewInfo, ref caretX, layoutPosition.Column, rowIndexInColumn, sourceCellViewInfo);
		}
		protected internal virtual void CorrectCaretXOnExtendSelection(TableCellViewInfo sourceCellViewInfo, ref int caretX) {
			if (sourceCellViewInfo == null)
				return;
			if (caretX < sourceCellViewInfo.TextLeft)
				caretX = sourceCellViewInfo.TextLeft;
			else if (caretX > (sourceCellViewInfo.TextLeft + sourceCellViewInfo.Width))
				caretX  = sourceCellViewInfo.TextLeft + sourceCellViewInfo.Width;
		}
		protected internal virtual void CorrectRowAndCellViewInfo(CaretPosition pos, ref int rowIndexInColumn, ref TableCellViewInfo sourceCellViewInfo) {
			DocumentModel documentModel = pos.DocumentModel;
			Selection selection = documentModel.Selection;
			if (IsCaretAtTheBegginingOfAnotherCell(pos, documentModel)) {
				if (sourceCellViewInfo == null) {
					return;
				}
				rowIndexInColumn = Math.Max(0, rowIndexInColumn - 1);
				TableCellViewInfo cellFromPreviousLayoutRow = pos.LayoutPosition.Column.Rows[rowIndexInColumn].GetCellViewInfo();
				TableRowViewInfoBase tableRow = sourceCellViewInfo.GetTableRow();
				bool cellsInDifferntTables = cellFromPreviousLayoutRow != null && !Object.ReferenceEquals(sourceCellViewInfo.TableViewInfo.Table, cellFromPreviousLayoutRow.TableViewInfo.Table);
				if (cellsInDifferntTables && selection.FirstAndSecondCellHaveCommonTableButSecondCellNotParentForFirstCell(sourceCellViewInfo.Cell, cellFromPreviousLayoutRow.Cell)) {
					tableRow = cellFromPreviousLayoutRow.GetTableRow();
					sourceCellViewInfo = GetLastNonCoveredByVerticalMergingCell(tableRow);
				}
				else if (IsCellFromPreviousRowIsInNestedTableInSourceCellViewInfo(sourceCellViewInfo, cellFromPreviousLayoutRow)) {
					sourceCellViewInfo = cellFromPreviousLayoutRow;
				}
				else if (sourceCellViewInfo.Cell.IsFirstCellInRow && tableRow.Previous != null) {
					tableRow = tableRow.Previous;
					sourceCellViewInfo = GetLastNonCoveredByVerticalMergingCell(tableRow);
				}
				else if (cellFromPreviousLayoutRow == null)
					sourceCellViewInfo = null;
				else
					sourceCellViewInfo = GetPreviousNonVerticalMergedCellInRow(sourceCellViewInfo);
				if (sourceCellViewInfo != null) {
					int newRowIndex = pos.LayoutPosition.Column.Rows.IndexOf(sourceCellViewInfo.GetRows(pos.LayoutPosition.Column).Last);
					rowIndexInColumn = newRowIndex;
				}
			}
		}
		bool IsCaretAtTheBegginingOfAnotherCell(CaretPosition pos, DocumentModel documentModel) {
			Selection selection = documentModel.Selection;
			return selection.Length > 0 && selection.End > selection.Start && pos.LayoutPosition.Row.GetFirstPosition(documentModel.ActivePieceTable).LogPosition == pos.LogPosition;
		}
		bool IsCellFromPreviousRowIsInNestedTableInSourceCellViewInfo(TableCellViewInfo sourceCellViewInfo, TableCellViewInfo cellFromPreviousLayoutRow) {
			if (cellFromPreviousLayoutRow == null)
				return false;
			bool isParent =  sourceCellViewInfo.Cell.DocumentModel.Selection.FirstCellIsParentCellForSecondCellsTable(sourceCellViewInfo.Cell, cellFromPreviousLayoutRow.Cell);
			return isParent;
		}
		protected internal virtual Row GetTargetLayoutRowObtainTargetPageViewInfoCore(PageViewInfo pageViewInfo, ref int caretX, Column column, int rowIndexInColumn, TableCellViewInfo sourceCellViewInfo) {
			if (ShouldObtainTargetPageViewInfo(rowIndexInColumn, column.Rows.Count)
				 || IsBoundaryTableCellRowOnCurrentPage(sourceCellViewInfo, column, rowIndexInColumn, caretX))
				return ObtainTargetRowFromCurrentPageViewInfoAndCaretX(pageViewInfo, sourceCellViewInfo, ref caretX);
			return ObtainTargetLayoutRow(rowIndexInColumn, caretX, column, sourceCellViewInfo);
		}
		protected internal virtual Row ObtainTargetLayoutRow(int rowIndexInColumn, int caretX, Column column, TableCellViewInfo sourceCell) {
			TableCellViewInfo nextCell = sourceCell;
			if (sourceCell == null) { 
				Row row = GetNextLayoutRowByDirection(column.Rows, rowIndexInColumn);
				nextCell = row.GetCellViewInfo();
				if (nextCell == null)
					return row;
				TableCellViewInfo targetCell = FindTableCellViewInfoInTableRowByX(nextCell.GetTableRow(), caretX);
				RowCollection targetCellLayoutRows = targetCell.GetRows(column);
				if (targetCellLayoutRows.Count == 0) {
					TableCellViewInfo nextCellWithNonZeroRows = GetNextCellViewInfoInTargetRowWithNonZeroRows(column, targetCell.GetTableRow(), targetCell);
					if (nextCellWithNonZeroRows != null)
						targetCellLayoutRows = nextCellWithNonZeroRows.GetRows(column);
				}
				return GetTargetLayoutRowCore(targetCellLayoutRows);
			}
			return ObtainTargetLayoutRowFromNextCellInCurrentTable(rowIndexInColumn, caretX, column, nextCell);
		}
		#region By Cell
		protected internal virtual bool ShouldJumpToNextCellInCurrentTable(bool shouldJumpToCellInNestedTable, TableCellViewInfo nextCell, bool isLastOrFirstLayoutRowInCell) {
			bool shouldJumpToNextCellInCurrentTable = (nextCell != null) && isLastOrFirstLayoutRowInCell;
			return !shouldJumpToCellInNestedTable && shouldJumpToNextCellInCurrentTable;
		}
		protected internal virtual bool ShouldObtainNextLayoutRowInsideCell(bool isLastOrFirstLayoutRowInCell, TableCellViewInfo nextCell) {
			return !isLastOrFirstLayoutRowInCell;
		}
		protected internal virtual Row MoveBetweenRowsInsideCell(RowCollection cellRows, int rowIndexInCell) {
			return GetNextLayoutRowByDirection(cellRows, rowIndexInCell);
		}
		protected internal virtual Row ObtainTargetLayoutRowFromNextCellInCurrentTable(int rowIndexInColumn, int caretX, Column column, TableCellViewInfo sourceCellViewInfo) {
			TableCellViewInfo nextCell = GetTargetCellViewInfoFromNextTableRow(sourceCellViewInfo, caretX);
			RowCollection cellRows = sourceCellViewInfo.GetRows(column);
			int rowIndexInCell = cellRows.IndexOf(column.Rows[rowIndexInColumn]);
			bool shouldJumpToCellInNestedTable = ShouldJumpToCellInNestedTable(column.Rows, sourceCellViewInfo.Cell.Table.NestedLevel, rowIndexInColumn, caretX);
			bool isLastOrFirstLayoutRowInCell = IsLastOrFirstLayoutRowInCell(rowIndexInCell, cellRows.Count);
			if (ShouldJumpToNextCellInCurrentTable(shouldJumpToCellInNestedTable, nextCell,isLastOrFirstLayoutRowInCell))
				return ObtainTargetLayoutRowFromNextCellInCurrentTableCore(caretX, column, sourceCellViewInfo, nextCell);
			if (shouldJumpToCellInNestedTable)
				return ObtainLayoutRowFromValidCellInFirstRowNestedTable(rowIndexInColumn, caretX, column, sourceCellViewInfo.TableViewInfo.Table.NestedLevel);
			if (ShouldObtainNextLayoutRowInsideCell(isLastOrFirstLayoutRowInCell, nextCell)) {
				Row targetLayoutRow = MoveBetweenRowsInsideCell(cellRows, rowIndexInCell);
#if (DEBUGTEST||DEBUG)
				TableCellViewInfo rowCell = targetLayoutRow.GetCellViewInfo(); 
				if (!Object.ReferenceEquals(rowCell, sourceCellViewInfo))
					Exceptions.ThrowInternalException();
#endif
				return targetLayoutRow;
			}
			TableRowViewInfoBase lastTableRow = GetLastTableRowInDirection(sourceCellViewInfo, caretX);
			TableCellViewInfo firstOrLastCellInRow = GetFirstOrLastCellViewInfoInRow(lastTableRow);
			return MoveFromInnerIntoOuterTable(sourceCellViewInfo, column, firstOrLastCellInRow, caretX);
		}
		protected internal virtual Row ObtainTargetLayoutRowFromNextCellInCurrentTableCore(int caretX, Column column, TableCellViewInfo sourceCellViewInfo, TableCellViewInfo nextCell) {
			RowCollection cellRows = nextCell.GetRows(column);
			if (cellRows.Count == 0)
				return column.Rows.First;
			Row firstOrLastRowInCell = GetTargetLayoutRowCore(cellRows );
			int nextCellViewInfoNestedLevel = firstOrLastRowInCell.GetCellViewInfo().TableViewInfo.Table.NestedLevel;
			bool isLayoutRowInNestedTable = nextCellViewInfoNestedLevel != sourceCellViewInfo.Cell.Table.NestedLevel;
			if (isLayoutRowInNestedTable)
				nextCell = FindMostNestedTableCell(nextCell, column, caretX);
			return GetTargetLayoutRowCore(nextCell.GetRows(column));
		}
		protected internal bool IsLastOrFirstLayoutRowInCell(int rowIndexInCell, int cellsRowsCount) {
			return ShouldObtainTargetPageViewInfo(rowIndexInCell, cellsRowsCount);
		}
		protected internal TableCellViewInfo GetTargetCellViewInfoFromNextTableRow(TableCellViewInfo cell, int x) {
			TableRowViewInfoBase row = cell.GetTableRow();
			for (; ; ) {
				TableRowViewInfoBase nextRow = GetTargetTableRowCore(row);
				if (nextRow == null)
					return null;
				TableCellViewInfo nextCell = FindTableCellViewInfoInTableRowByX(nextRow, x);
				if (!Object.ReferenceEquals(nextCell, cell))
					return nextCell;
				row = nextRow;
			}
		}
		protected internal virtual TableCellViewInfo FindTableCellViewInfoInTableRowByX(TableRowViewInfoBase tableRow, int x) {
			TableCellViewInfoCollection cells = tableRow.Cells;
			TableCellAnchorComparable predicate = new TableCellAnchorComparable(x);
			int index = Algorithms.BinarySearch(cells, predicate);
			if (index >= 0)
				return cells[index];
			if (index == ~cells.Count) {
				if (predicate.CompareTo(cells.First) > 0)
					return cells.First;
				else
					return cells.Last;
			}
			else
				return cells[~index];
		}
		#endregion
		#region NestedTable
		protected internal virtual Row ObtainLayoutRowFromValidCellInFirstRowNestedTable(int rowIndex, int caretX, Column column, int sourceNestedLevel) {
			Row nextLayoutRow = GetNextLayoutRowByDirection(column.Rows, rowIndex);
			TableCellViewInfo firstCellInInnerTable = GetTargetCellViewInfoInNestedTable(nextLayoutRow, caretX, sourceNestedLevel);
			if (firstCellInInnerTable == null) return null;
			Row firstLayoutRowInInnerTableCell = GetFirstOrLastLayoutRowInCell(firstCellInInnerTable.GetRows(column));
			TableCellViewInfo candidateCell = firstLayoutRowInInnerTableCell.GetCellViewInfo();
			while (firstCellInInnerTable.TableViewInfo.Table.NestedLevel != candidateCell.TableViewInfo.Table.NestedLevel) {
				firstCellInInnerTable = FindTableCellViewInfoInTableRowByX(candidateCell.GetTableRow(), caretX);
				firstLayoutRowInInnerTableCell = GetFirstOrLastLayoutRowInCell(firstCellInInnerTable.GetRows(column));
				candidateCell = firstLayoutRowInInnerTableCell.GetCellViewInfo();
			}
			return firstLayoutRowInInnerTableCell;
		}
		internal bool ShouldJumpToCellInNestedTable(RowCollection rows, int currentCellNestedLevel, int sourceRowIndexInColumn, int caretX) {
			Row sourceCellRow = rows[sourceRowIndexInColumn];
			TableCellViewInfo sourceCell = sourceCellRow.GetCellViewInfo();
			if (sourceCell == null || sourceCell.InnerTables.Count == 0)
				return false;
			Row nextLayoutRow = GetNextLayoutRowByDirection(rows, sourceRowIndexInColumn);
			TableCellViewInfo targetCell = GetTargetCellViewInfoInNestedTable(nextLayoutRow, caretX, currentCellNestedLevel);
			if (targetCell == null) 
				return false; 
			int nextRowTableNestedLevel = targetCell.Cell.Table.NestedLevel;
			bool nextCellIsNested = nextRowTableNestedLevel > currentCellNestedLevel;
			if (!nextCellIsNested)
				return false;
			TableCellViewInfo parentCellViewInfoForTargetCell = targetCell.TableViewInfo.ParentTableCellViewInfo;
			while (parentCellViewInfoForTargetCell != null) {
				if (Object.ReferenceEquals(sourceCell, parentCellViewInfoForTargetCell))
					return true;
				parentCellViewInfoForTargetCell = parentCellViewInfoForTargetCell.TableViewInfo.ParentTableCellViewInfo;
			}
			return false;
		}
		protected internal virtual Row ObtainRowFromOuterTextRowOrOuterTableWhenCellsInInnerTable(Column column, TableCellViewInfo innerSourceCellViewInfo, int caretX, TableCellViewInfo firstOrLastCellInRow) {
			RowCollection firstLastTextRowsCollection = firstOrLastCellInRow.GetRows(column);
			Row firstOrLastTextRowInCell = GetTargetLayoutRowInCell(firstLastTextRowsCollection);
			int rowIndexInColumn = column.Rows.IndexOf(firstOrLastTextRowInCell);
			Row row = GetNextLayoutRowByDirection(column.Rows, rowIndexInColumn); 
			return row;
		}
		#endregion
		protected internal virtual TableRowViewInfoBase GetLastTableRowInDirection(TableCellViewInfo cell, int x) {
			TableRowViewInfoBase tableRow = cell.GetTableRow();
			int rowSpan = cell.RowSpan;
			for (int i = 0; i < rowSpan && tableRow != null; i++) {
				TableRowViewInfoBase nextTableRow= GetTargetTableRowCore(tableRow);
				if (nextTableRow == null)
					return tableRow;
				tableRow = nextTableRow;
			}
#if (DEBUGTEST||DEBUG)
			Exceptions.ThrowInternalException();
#endif
			return null;
		}
		protected internal virtual CharacterBox GetTargetCharacter(Row row, int caretX) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActivePieceTable);
			request.LogicalPoint = new Point(caretX, row.Bounds.Y + row.BaseLineOffset);
			RichEditHitTestResult result = new RichEditHitTestResult(View.DocumentLayout, ActivePieceTable);
			BoxHitTestCalculator hitTestCalculator = View.CreateHitTestCalculator(request, result);
			int boxIndex = hitTestCalculator.FastHitTestIndexCore(row.Boxes, new BoxAndPointXComparable<Box>(request.LogicalPoint), false);
			Debug.Assert(boxIndex >= 0);
			DetailRow detailRow = View.DocumentLayout.CreateDetailRowForBox(row, row.Boxes[boxIndex], false);
			hitTestCalculator.FastHitTestCharacter(detailRow.Characters, false);
			Debug.Assert(result.IsValid(DocumentLayoutDetailsLevel.Character));
			return result.Character;
		}
		#region Page
		TableCellViewInfo GetNextCellViewInfoInTargetRowWithNonZeroRows(Column column, TableRowViewInfoBase targetRow, TableCellViewInfo cellWithZeroRows) {
			TableCellViewInfo nextCell;
			nextCell = cellWithZeroRows;
			int cellsCount = targetRow.Cells.Count;
			int cellIndex = targetRow.Cells.IndexOf(cellWithZeroRows);
			while (nextCell.GetRows(column).Count == 0 && cellIndex < cellsCount) {
				nextCell = targetRow.Cells[cellIndex];
				cellIndex++;
			}
			return nextCell;
		}
		protected internal virtual Row ObtainTargetRowFromCurrentPageViewInfoAndCaretX(PageViewInfo pageViewInfo, TableCellViewInfo cellViewInfoFromPrevoiusTableViewInfo, ref int caretX) {
			Point physicalCaretPoint = View.CreatePhysicalPoint(pageViewInfo, new Point(caretX, 0));
			PageViewInfo targetPageViewInfo = ObtainTargetPageViewInfo(pageViewInfo, physicalCaretPoint.X);
			if (targetPageViewInfo == null)
				return null;
			Point logicalCaretPoint = View.CreateLogicalPoint(targetPageViewInfo.ClientBounds, new Point(physicalCaretPoint.X, 0));
			caretX = logicalCaretPoint.X;
			Column column = GetTargetColumn(targetPageViewInfo.Page, logicalCaretPoint.X);
			Row firstColumnRow = GetTargetLayoutRowCore(column.Rows);
			TableCellViewInfo firstColumnRowCell = firstColumnRow.GetCellViewInfo();
			if (firstColumnRowCell == null)
				return GetTargetLayoutRowCore(column.Rows);
			Table targetTable = cellViewInfoFromPrevoiusTableViewInfo != null ? cellViewInfoFromPrevoiusTableViewInfo.TableViewInfo.Table : firstColumnRowCell.TableViewInfo.Table;
			int targetTableFromPreviousPageNestedLevel = targetTable.NestedLevel;
			int firstColumnRowCellTableNestedLevel = firstColumnRowCell.TableViewInfo.Table.NestedLevel;
			bool isExpectedTheNestedTableEndsOnPreviousPageAndOuterTableContinuesOnTheNextPage = targetTableFromPreviousPageNestedLevel - 1 == firstColumnRowCellTableNestedLevel;
			if (isExpectedTheNestedTableEndsOnPreviousPageAndOuterTableContinuesOnTheNextPage) {
				Table parentTargetTable = targetTable.ParentCell.Table; 
				targetTable = parentTargetTable;
			}
			TableRowViewInfoBase targetRow = FindMostParentTableRowViewInfo(targetTable, firstColumnRowCell);
			TableCellViewInfo cellFromNextPage = FindTableCellViewInfoInTableRowByX(targetRow, caretX);
			if (cellFromNextPage.IsStartOnPreviousTableViewInfo() && cellFromNextPage.GetRows(column).Count == 0) {
				TableCellViewInfo nextCell = GetTargetCellViewInfoFromNextTableRow(cellFromNextPage, caretX);
				if (nextCell == null) { 
					nextCell = GetNextCellViewInfoInTargetRowWithNonZeroRows(column, targetRow, cellFromNextPage);
					return ObtainTargetLayoutRowFromNextCellInCurrentTableCore(caretX, column, cellFromNextPage, nextCell);
				}
				return ObtainTargetLayoutRowFromNextCellInCurrentTableCore(caretX, column, cellFromNextPage, nextCell);
			}
			TableCellViewInfo mostNestedTableCell = FindMostNestedTableCell(cellFromNextPage, column, caretX);
			return GetTargetLayoutRowCore(mostNestedTableCell.GetRows(column));
		}
		protected internal virtual TableRowViewInfoBase FindMostParentTableRowViewInfo(Table targetTable, TableCellViewInfo firstColumnRowCell) {
			if (firstColumnRowCell == null)
				return null;
			TableRowViewInfoBase result = firstColumnRowCell.GetTableRow();
			TableCellViewInfo parentCellViewInfo = firstColumnRowCell.TableViewInfo.ParentTableCellViewInfo;
			while (parentCellViewInfo != null) {
				if (Object.ReferenceEquals(targetTable, parentCellViewInfo.TableViewInfo.Table)
					|| parentCellViewInfo.TableViewInfo.Table.NestedLevel == 0) {
					result = parentCellViewInfo.GetTableRow();
					break;
				}
				parentCellViewInfo = parentCellViewInfo.TableViewInfo.ParentTableCellViewInfo;
			}
			return result;
		}
		TableCellViewInfo FindMostNestedTableCell(TableCellViewInfo theCell, Column column, int caretX) {
			TableCellViewInfo nextCell = theCell;
			while (nextCell.InnerTables.Count > 0 && nextCell.InnerTables[0].FirstContentInParentCell) {
				RowCollection rowsFromNextCell = nextCell.GetRows(column);
				int rowIndexInNextCell = column.Rows.IndexOf(rowsFromNextCell.First);
				Row nextLayoutRow = column.Rows[rowIndexInNextCell];
				nextCell = GetTargetCellViewInfoInNestedTable(nextLayoutRow, caretX, nextCell.TableViewInfo.Table.NestedLevel);
			}
			return nextCell;
		}
		protected internal virtual PageViewInfo ObtainTargetPageViewInfo(PageViewInfo currentPageViewInfo, int physicalCaretX) {
			Debug.Assert(currentPageViewInfo != null);
			PageViewInfoRowCollection pageRows = View.PageViewInfoGenerator.ActiveGenerator.PageRows;
			int rowIndex = GetPageViewInfoRowIndex(pageRows, currentPageViewInfo);
			Debug.Assert(rowIndex >= 0);
			PageViewInfoRow pageRow;
			if (ShouldObtainTargetPageViewInfo(rowIndex, pageRows.Count))
				pageRow = GenerateTargetPageViewInfoRow();
			else
				pageRow = ObtainExistingTargetPageViewInfoRow(pageRows, rowIndex);
			if (pageRow != null) {
				PageViewInfo result = pageRow.GetPageAtPoint(new Point(physicalCaretX, 0), false);
				Debug.Assert(result != null);
				return result;
			}
			else
				return null;
		}
		protected internal virtual int GetPageViewInfoRowIndex(PageViewInfoRowCollection pageRows, PageViewInfo pageViewInfo) {
			int count = pageRows.Count;
			for (int i = 0; i < count; i++) {
				if (pageRows[i].IndexOf(pageViewInfo) >= 0)
					return i;
			}
			return -1;
		}
		protected internal virtual PageViewInfoRow GenerateTargetPageViewInfoRow() {
			InvisiblePageRowsGenerator generator = CreateInvisiblePageRowsGenerator();
			PageViewInfoRow row = generator.GenerateNextRow();
			if (row != null)
				CorrectGeneratedRowPagesHorizontalLocation(generator.LayoutManager, row);
			return row;
		}
		protected internal virtual InvisiblePageRowsGenerator CreateInvisiblePageRowsGenerator() {
			InvisiblePageRowsGenerator generator = new InvisiblePageRowsGenerator(View.FormattingController.PageController.Pages, View.PageViewInfoGenerator);
			generator.FirstPageIndex = CalculateFirstInvisiblePageIndex();
			if (generator.FirstPageIndex < 0 && !FixedDirection) {
				generator.FirstPageIndex = 0;
				generator.FirstInvalidPageIndex = View.FormattingController.PageController.Pages.Count;
			}
			else
				generator.FirstInvalidPageIndex = CalculateFirstInvalidPageIndex();
			Rectangle bounds = generator.LayoutManager.ViewPortBounds;
			bounds.Y = 0;
			generator.LayoutManager.ViewPortBounds = bounds;
			return generator;
		}
		protected internal virtual Column GetTargetColumn(Page page, int caretX) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActivePieceTable);
			request.LogicalPoint = new Point(caretX, 0);
			request.DetailsLevel = DocumentLayoutDetailsLevel.Column;
			request.Accuracy = HitTestAccuracy.NearestColumn;
			PageArea pageArea = GetTargetPageArea(page);
			RichEditHitTestResult result = new RichEditHitTestResult(View.DocumentLayout, ActivePieceTable);
			BoxHitTestCalculator calculator = View.CreateHitTestCalculator(request, result);
			calculator.FastHitTestAssumingArrangedHorizontally(pageArea.Columns, false);
			Debug.Assert(result.Column != null);
			return result.Column;
		}
		#endregion
		protected internal Row GetNextLayoutRowByDirection(RowCollection rows, int sourceRowIndex) {
			int newIndex = GetNextLayoutRowIndexInDirection(sourceRowIndex);
			if (newIndex >= rows.Count)
				Exceptions.ThrowInternalException();
			return rows[newIndex];
		}
		protected internal virtual TableCellViewInfo GetLastNonCoveredByVerticalMergingCell(TableRowViewInfoBase row) {
			int rowCellsCount = row.Cells.Count;
			TableCellViewInfo result = row.Cells.Last;
			for (int cellIndex = rowCellsCount - 1; cellIndex >= 0 && result.Cell.VerticalMerging == MergingState.Restart; cellIndex--)
				result = row.Cells[cellIndex];
			return result;
		}
		protected internal virtual TableCellViewInfo GetPreviousNonVerticalMergedCellInRow(TableCellViewInfo sourceCellViewInfo) {
			TableRowViewInfoBase row = sourceCellViewInfo.GetTableRow();
			int sourceCellViewInfoIndexInRow = row.Cells.IndexOf(sourceCellViewInfo);
			TableCellViewInfo result = row.Cells[Math.Max(0, sourceCellViewInfoIndexInRow - 1)];
			for (int cellIndex = sourceCellViewInfoIndexInRow - 1; cellIndex >= 0 && result.Cell.VerticalMerging == MergingState.Restart; cellIndex--)
				result = row.Cells[cellIndex];
			return result;
		}
		#region abstract methods
		protected internal abstract PageArea GetTargetPageArea(Page page);
		protected internal abstract Row GetTargetLayoutRowCore(RowCollection rows);
		protected internal abstract int GetNextLayoutRowIndexInDirection(int sourceRowIndex);
		protected internal abstract Row GetTargetLayoutRowInCell(RowCollection rows);
		protected internal abstract bool ShouldObtainTargetPageViewInfo(int sourceRowIndex, int sourceRowCount);
		protected internal abstract PageViewInfoRow ObtainExistingTargetPageViewInfoRow(PageViewInfoRowCollection pageRows, int currentRowIndex);
		protected internal abstract int CalculateFirstInvisiblePageIndex();
		protected internal abstract int CalculateFirstInvalidPageIndex();
		protected internal abstract void CorrectGeneratedRowPagesHorizontalLocation(PageGeneratorLayoutManager layoutManager, PageViewInfoRow row);
		protected internal abstract TableRowViewInfoBase GetTargetTableRowCore(TableRowViewInfoBase row);
		protected internal abstract TableCellViewInfo GetFirstOrLastCellViewInfoInRow(TableRowViewInfoBase row);
		protected internal abstract TableCellViewInfo GetTargetCellViewInfoInNestedTable(Row nextLayoutRow, int caretX, int currentNestedLevel);
		protected internal abstract Row GetFirstOrLastLayoutRowInCell(RowCollection cellRows);
		protected abstract Row GetBoundaryRowInColumn(Column column);
		protected abstract bool IsBoundaryRowInCell(Column column, TableCellViewInfo cell, Row sourceRow);
		protected abstract bool IsCellInBoundaryRow(Column column, TableCellViewInfo cell, int caretX);
		protected abstract int GetCellBoundaryRowIndex(Column column, TableCellViewInfo parentCell);
		protected internal abstract bool IsCellViewInfoContinuesOnAnotherPage(TableCellViewInfo cell);
		protected internal abstract Row MoveFromInnerIntoOuterTable(TableCellViewInfo innerSourceCellViewInfo, Column column, TableCellViewInfo firstOrLastCellInRow, int caretX);
		#endregion
		protected internal bool IsBoundaryTableCellRowOnCurrentPage(TableCellViewInfo cell, Column column, int sourceRowIndexInColumn, int caretX) {
			if (!IsCellInBoundaryRow(column, cell, caretX))
				return false;
			Row sourceRow = column.Rows[sourceRowIndexInColumn];			
			if (!IsBoundaryRowInCell(column, cell, sourceRow))
				return false;
			TableCellRow boundaryRowInColumn = GetBoundaryRowInColumn(column) as TableCellRow;
			if (boundaryRowInColumn == null)
				return false;
			TableCellViewInfo currentCell = cell;
			while (true) {
				TableCellViewInfo parentCell = currentCell.TableViewInfo.ParentTableCellViewInfo;
				if (parentCell == null)
					break;
				Row lastRowInCurrentCell = currentCell.GetRows(column).Last;
				bool sourceRowIsLastInCurrentCell = Object.ReferenceEquals(lastRowInCurrentCell, sourceRow);
				if (!sourceRowIsLastInCurrentCell && GetCellBoundaryRowIndex(column, parentCell) != sourceRowIndexInColumn)
					return false;
				if(!IsCellInBoundaryRow(column, parentCell, caretX))
					return false;
				else if (sourceRowIsLastInCurrentCell) {
					TableCellRow lastRowInParentCell = parentCell.GetRows(column).Last as TableCellRow;
					Debug.Assert(lastRowInParentCell != null);
					if (lastRowInParentCell != null) {
						TableCellViewInfo lastRowCellViewInfo = lastRowInParentCell.CellViewInfo;
						while (lastRowCellViewInfo.TableViewInfo != parentCell.TableViewInfo && lastRowCellViewInfo.TableViewInfo != currentCell.TableViewInfo) {
							lastRowCellViewInfo = lastRowCellViewInfo.TableViewInfo.ParentTableCellViewInfo;
						}
						if (lastRowCellViewInfo.TableViewInfo == parentCell.TableViewInfo)
							return false;
					}
				}
				currentCell = parentCell;
			}
			TableViewInfo outerTableViewInfo = currentCell.TableViewInfo;
			TableViewInfo boundaryOuterTableViewInfo = boundaryRowInColumn.CellViewInfo.TableViewInfo;
			while (boundaryOuterTableViewInfo.ParentTableCellViewInfo != null)
				boundaryOuterTableViewInfo = boundaryOuterTableViewInfo.ParentTableCellViewInfo.TableViewInfo;
			return Object.ReferenceEquals(outerTableViewInfo, boundaryOuterTableViewInfo);
		}
	}
	#endregion
	#region NextCaretPositionUpDirectionCalculator (abstract class)
	public abstract class NextCaretPositionUpDirectionCalculator : NextCaretPositionVerticalDirectionCalculator {
		protected NextCaretPositionUpDirectionCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override int CalculateFirstInvisiblePageIndex() {
			return View.CalculateFirstInvisiblePageIndexBackward();
		}
		protected internal override int CalculateFirstInvalidPageIndex() {
			return -1;
		}
		protected internal override PageArea GetTargetPageArea(Page page) {
			return page.Areas.Last;
		}
		protected internal override Row GetTargetLayoutRowCore(RowCollection rows) {
			return rows.Last;
		}
		protected internal override TableCellViewInfo GetFirstOrLastCellViewInfoInRow(TableRowViewInfoBase row) {
			return row.Cells.First;
		}
		protected internal override void CorrectGeneratedRowPagesHorizontalLocation(PageGeneratorLayoutManager layoutManager, PageViewInfoRow row) {
			int count = row.Count;
			Debug.Assert(count > 0);
			int x = row[0].Bounds.X;
			for (int i = count - 1; i >= 0; i--) {
				PageViewInfo pageViewInfo = row[i];
				Rectangle bounds = pageViewInfo.Bounds;
				bounds.X = x;
				pageViewInfo.Bounds = bounds;
				layoutManager.UpdatePageClientBounds(pageViewInfo);
				x += bounds.Width;
			}
			Algorithms.InvertElementsOrder(row);
		}
		protected internal override TableRowViewInfoBase GetTargetTableRowCore(TableRowViewInfoBase sourceRow) {
			return sourceRow.Previous;
		}
		protected internal override TableCellViewInfo GetTargetCellViewInfoInNestedTable(Row nextLayoutRow,int caretX, int currentNestedLevel) {
			TableCellViewInfo topLeftCell = GetBottomRightTableCellViewInfoByNestedLevel(nextLayoutRow, currentNestedLevel + 1);
			if (topLeftCell == null)
				return null;
			TableCellViewInfo targetCell = FindTableCellViewInfoInTableRowByX(topLeftCell.GetTableRow(), caretX);
			return targetCell;
		}
		protected internal virtual TableCellViewInfo GetBottomRightTableCellViewInfoByNestedLevel(Row nextLayoutRow, int desiredNestedLevel) {
			TableCellViewInfo result = nextLayoutRow.GetCellViewInfo();
			if (result == null)
				return null;
			int nextLayoutRowTableNestedLevel = result.TableViewInfo.Table.NestedLevel;
			while (nextLayoutRowTableNestedLevel > desiredNestedLevel) {
				result = result.TableViewInfo.ParentTableCellViewInfo;
				nextLayoutRowTableNestedLevel = result.TableViewInfo.Table.NestedLevel;
			}
			return result;
		}
		protected internal override Row GetFirstOrLastLayoutRowInCell(RowCollection cellRows) {
			return cellRows.Last;
		}
		protected override Row GetBoundaryRowInColumn(Column column) {
			return column.Rows.First;
		}
		protected override bool IsBoundaryRowInCell(Column column, TableCellViewInfo cell, Row sourceRow) {
			return Object.ReferenceEquals(cell.GetRows(column).First, sourceRow);
		}
		protected override int GetCellBoundaryRowIndex(Column column, TableCellViewInfo parentCell) {
			return parentCell.GetFirstRowIndex(column);
		}
		protected override bool IsCellInBoundaryRow(Column column, TableCellViewInfo cell, int caretX) {
			return cell != null &&
				cell.TopAnchorIndex == 0;
		}
		protected internal override bool IsCellViewInfoContinuesOnAnotherPage(TableCellViewInfo cell) {
			return cell.IsStartOnPreviousTableViewInfo();
		}
		protected internal override Row MoveFromInnerIntoOuterTable(TableCellViewInfo innerSourceCellViewInfo, Column column, TableCellViewInfo innerFirstCellInTableRow, int caretX) {
			if (ShouldJumpUpFromMultipleInnerTableToOuter(innerFirstCellInTableRow))
				return ObtainRowInNextCellWhenFirstCellInTableRowIsInMultipleInnerTable(column, caretX, innerFirstCellInTableRow.TableViewInfo);
			return ObtainRowFromOuterTextRowOrOuterTableWhenCellsInInnerTable(column, innerSourceCellViewInfo, caretX, innerFirstCellInTableRow);
		}
		bool ShouldJumpUpFromMultipleInnerTableToOuter(TableCellViewInfo firstCellInTableRow) {
			return firstCellInTableRow.Cell.IsFirstRow && firstCellInTableRow.TableViewInfo.FirstContentInParentCell;
		}
		Row ObtainRowInNextCellWhenFirstCellInTableRowIsInMultipleInnerTable(Column column, int caretX, TableViewInfo tableViewInfo) {
			TableCellViewInfo parentCellViewInfo = tableViewInfo.ParentTableCellViewInfo;
			if (parentCellViewInfo == null)
				return null; 
			Row theRow = GetTargetLayoutRowInCell(parentCellViewInfo.GetRows(column));
			int rowIndexInColumn1 = column.Rows.IndexOf(theRow);
			return ObtainTargetLayoutRowFromNextCellInCurrentTable(rowIndexInColumn1, caretX, column, parentCellViewInfo);
		}
	}
	#endregion
	#region NextCaretPositionDownDirectionCalculator (abstract class)
	public abstract class NextCaretPositionDownDirectionCalculator : NextCaretPositionVerticalDirectionCalculator {
		protected NextCaretPositionDownDirectionCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override PageArea GetTargetPageArea(Page page) {
			return page.Areas.First;
		}
		protected internal override Row GetTargetLayoutRowCore(RowCollection rows) {
			return rows.First;
		}
		protected internal override Row GetTargetLayoutRowInCell(RowCollection rows) {
			return rows.Last;
		}
		protected internal override TableCellViewInfo GetFirstOrLastCellViewInfoInRow(TableRowViewInfoBase row) {
			if (row == null)
				return null;
			return GetLastNonCoveredByVerticalMergingCell(row);
		}
		protected internal override int CalculateFirstInvisiblePageIndex() {
			return View.CalculateFirstInvisiblePageIndexForward();
		}
		protected internal override int CalculateFirstInvalidPageIndex() {
			return View.FormattingController.PageController.Pages.Count;
		}
		protected internal override void CorrectGeneratedRowPagesHorizontalLocation(PageGeneratorLayoutManager layoutManager, PageViewInfoRow row) {
		}
		protected internal override TableRowViewInfoBase GetTargetTableRowCore(TableRowViewInfoBase sourceRow) {
			return sourceRow.Next;
		}
		protected internal override TableCellViewInfo GetTargetCellViewInfoInNestedTable(Row nextLayoutRow, int caretX, int currentNestedLevel) {
			TableCellViewInfo topLeftCell = GetTopLeftTableCellViewInfoByNestedLevel(nextLayoutRow, currentNestedLevel + 1);
			if (topLeftCell == null)
				return null;
			TableCellViewInfo targetCell = FindTableCellViewInfoInTableRowByX(topLeftCell.GetTableRow(), caretX);
			return targetCell;
		}
		internal TableCellViewInfo GetTopLeftTableCellViewInfoByNestedLevel(Row nextLayoutRow, int desiredNestedLevel) {
			TableCellViewInfo result = nextLayoutRow.GetCellViewInfo();
			if (result == null)
				return null;
			int nextLayoutRowTableNestedLevel = result.TableViewInfo.Table.NestedLevel;
			while (nextLayoutRowTableNestedLevel > desiredNestedLevel) {
				result = result.TableViewInfo.ParentTableCellViewInfo;
				nextLayoutRowTableNestedLevel = result.TableViewInfo.Table.NestedLevel;
			}
			return result;
		}
		protected internal override Row GetFirstOrLastLayoutRowInCell(RowCollection cellRows) {
			return cellRows.First;
		}
		protected override Row GetBoundaryRowInColumn(Column column) {
			return column.Rows.Last;
				}
		protected override bool IsBoundaryRowInCell(Column column, TableCellViewInfo cell, Row sourceRow) {
			return Object.ReferenceEquals(cell.GetRows(column).Last, sourceRow);
		}
		protected override bool IsCellInBoundaryRow(Column column, TableCellViewInfo cell, int caretX) {
			return cell != null &&
				cell.BottomAnchorIndex + 1 == cell.TableViewInfo.Anchors.Count;
			}
		protected override int GetCellBoundaryRowIndex(Column column, TableCellViewInfo parentCell) {
			return parentCell.GetLastRowIndex(column);
		}
		protected internal override bool IsCellViewInfoContinuesOnAnotherPage(TableCellViewInfo cell) {
			return cell.IsEndOnNextTableViewInfo();
		}
		protected internal override Row MoveFromInnerIntoOuterTable(TableCellViewInfo innerSourceCellViewInfo, Column column, TableCellViewInfo lastCellInTableRow, int caretX) {
			TableCellViewInfo targetLastCellViewInfoWithNonZeroRows = lastCellInTableRow;
			TableRowViewInfoBase row = targetLastCellViewInfoWithNonZeroRows.GetTableRow();
			int lastCellIndex = row.Cells.IndexOf(targetLastCellViewInfoWithNonZeroRows);
			RowCollection firstLastTextRowsCollection = targetLastCellViewInfoWithNonZeroRows.GetRows(column);
			while (lastCellIndex > 0 && firstLastTextRowsCollection.Count == 0 && targetLastCellViewInfoWithNonZeroRows.IsStartOnPreviousTableViewInfo()) {
				lastCellIndex--;
				targetLastCellViewInfoWithNonZeroRows = row.Cells[lastCellIndex];
				firstLastTextRowsCollection = targetLastCellViewInfoWithNonZeroRows.GetRows(column);
			}
			return ObtainRowFromOuterTextRowOrOuterTableWhenCellsInInnerTable(column, innerSourceCellViewInfo, caretX, targetLastCellViewInfoWithNonZeroRows);
		}
	}
	#endregion
}
