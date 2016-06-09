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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PrintRangeCalculator { 
		readonly Worksheet sheet;
		int previousNonVisibleFormatIndexCached = Int32.MinValue;
		int previousVisibleFormatIndexCached = Int32.MinValue;
		public PrintRangeCalculator(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		protected Worksheet Sheet { get { return sheet; } }
		public CellRange CalculateWithoutDefindeName() {
			return CalculateWithoutDefindeName(true);
		}
		public CellRange CalculateWithoutDefindeName(bool returnFirstCellFromRangeForEmptyDocument) {
			RangeIntermediateInfo result = new RangeIntermediateInfo();
			CalculateMergedCellsPrintRange(result);
			CalculateRangeIncludesEveryDrawingObject(result);
			CalculateVisibleRangeUntilLastTableBottomRight(result);
			CalculateCellsRightmostCell(result);
			CalculateFirstVisibleTopLeft(result, returnFirstCellFromRangeForEmptyDocument);
			return result.ToRange(Sheet, returnFirstCellFromRangeForEmptyDocument);
		}
		protected virtual void CalculateVisibleRangeUntilLastTableBottomRight(RangeIntermediateInfo info) {
			foreach (Table table in Sheet.Tables) {
				ProcessTable(info, table.Range);
			}
		}
		protected void ProcessTable(RangeIntermediateInfo info, CellRange tableRange) {
			info.ExpandToRightColumn(GetLastVisibleColumnUntilTable(info, tableRange.RightColumnIndex));
			info.ExpandToBottomRow(GetLastVisibleRowUntilTable(info, tableRange.BottomRowIndex));
		}
		int GetLastVisibleColumnUntilTable(RangeIntermediateInfo info, int tableRightColumnIndex) {
			if(info.RightChanged && info.RightColumn > tableRightColumnIndex)
				return RangeIntermediateInfo.BottomRightInitial;
			int leftColumn = info.RightChanged ? info.RightColumn : 0;
			int rightColumn = tableRightColumnIndex;
			return CalculateLastVisibleColumnRightFromTo(leftColumn, rightColumn);
		}
		int GetLastVisibleRowUntilTable(RangeIntermediateInfo info, int tableBottomRowIndex) {
			if(info.BottomChanged && info.BottomRow > tableBottomRowIndex)
				return RangeIntermediateInfo.BottomRightInitial;
			int topRow = info.BottomChanged ? info.BottomRow : 0;
			int bottomRow = tableBottomRowIndex;
			return CalculateLastVisibleRow(topRow, bottomRow);
		}
		bool CheckCellVisible(ICell _cell) {
			Cell cell = _cell as Cell;
			int formatIndex = cell.FormatIndex;
			if (cell.HasFormula || !cell.Value.IsEmpty)
				return true;
			if (formatIndex == previousNonVisibleFormatIndexCached)
				return false;
			if (formatIndex == previousVisibleFormatIndexCached)
				return true;
			CellFormat formatReadOnly = cell.FormatInfo;
			bool hasVisibleFillOrBorder = formatReadOnly.IsVisible;
			if (hasVisibleFillOrBorder)
				previousVisibleFormatIndexCached = formatIndex;
			else
				previousNonVisibleFormatIndexCached = formatIndex;
			return hasVisibleFillOrBorder;
		}
		public virtual int GetStopTopRowIndex() {
			return -1;
		}
		protected virtual int GetStopLeftColumnIndex() {
			return -1;
		}
		protected virtual int GetStopRightColumnIndex() { 
			return Sheet.MaxColumnCount; 
		}
		protected virtual int GetStopBottomRowIndex() {
			return Sheet.MaxRowCount;
		}
		protected void CalculateCellsRightmostCell(RangeIntermediateInfo result) {
			int stopLeft = GetStopLeftColumnIndex();
			int stopRight = GetStopRightColumnIndex(); 
			int stopTop = GetStopTopRowIndex();
			int stopBottom = GetStopBottomRowIndex();
			IEnumerable<Row> rowsReversed = Sheet.Rows.GetExistingVisibleRows(stopTop + 1, stopBottom - 1, true);
			foreach (Row row in rowsReversed) {
				if (row.CellsCount == 0)
					continue; 
				ICellCollection cells = row.Cells;
				ICell rightCell = GetRightmostCell(cells, stopLeft, stopRight);
				if (rightCell == null) 
					continue;
				result.ExpandToRightColumn(rightCell.ColumnIndex);
				ProcessFoundCell(result, rightCell.Position);
				result.ExpandToTopRow(row.Index);
				result.ExpandToBottomRow(row.Index);
			}
		}
		protected virtual void ProcessFoundCell(RangeIntermediateInfo result, CellPosition pos) {
		}
		protected void CalculateFirstVisibleTopLeft(RangeIntermediateInfo result, bool returnFirstCellFromRangeForEmptyDocument) {
			if (!returnFirstCellFromRangeForEmptyDocument) {
				bool someDataExistsInRange = result.RightChanged || result.BottomChanged;
				if ( !someDataExistsInRange)
					return;
			}
			int topRow = CalculateFirstVisibleTop(result, returnFirstCellFromRangeForEmptyDocument);
			int leftColumn = CalcuateFirstVisibleLeft(result, returnFirstCellFromRangeForEmptyDocument);
			result.ExpandToTopRow(topRow);
			result.ExpandToLeftColumn(leftColumn);
		}
		int CalculateFirstVisibleTop(RangeIntermediateInfo info,  bool returnFirstCellFromRangeForEmptyDocument) {
			int result = RangeIntermediateInfo.TopLeftInitial;
			int startRowIndexToSearch = GetStopTopRowIndex() + 1; ;  
			int endRowIndexToSearch = GetStopBottomRowIndex() -1; 
			if (info.TopChanged || info.BottomChanged)
				endRowIndexToSearch = Math.Min(info.TopRow, info.BottomRow);
			if (endRowIndexToSearch == 0 || (sheet.Rows.Count == 0 && sheet.DrawingObjects.Count == 0))
				result = returnFirstCellFromRangeForEmptyDocument ? 0 : RangeIntermediateInfo.TopLeftInitial;
			else {
				Row row = null;
				for (int rowIndex = startRowIndexToSearch; rowIndex < endRowIndexToSearch; rowIndex++) {
					row = sheet.Rows.TryGetRow(rowIndex);
					if (row == null || row.IsVisible) {
						result = rowIndex;
						break;
					}
				}
			}
			return result;
		}
		int CalcuateFirstVisibleLeft(RangeIntermediateInfo info,  bool returnFirstCellFromRangeForEmptyDocument) {
			int result = RangeIntermediateInfo.TopLeftInitial;
			int startColumnIndexToSearch = GetStopLeftColumnIndex() + 1; 
			int endColumnToSeach = GetStopRightColumnIndex() - 1; 
			if (info.LeftChanged || info.RightChanged)
				endColumnToSeach = Math.Min(info.LeftColumn, info.RightColumn);
			if (endColumnToSeach == 0 || sheet.Columns.Count == 0)
				result = returnFirstCellFromRangeForEmptyDocument ? 0 : startColumnIndexToSearch;
			else {
				Column col = null;
				for (int columnIndex = startColumnIndexToSearch; columnIndex < endColumnToSeach; columnIndex++) {
					col = sheet.Columns.TryGetColumn(columnIndex);
					if (col == null || col.IsVisible) {
						result = columnIndex;
						break;
					}
				}
			}
			return result;
		}
		protected virtual IColumnRange GetLastLayoutVisibleColumn(int maxCellColumnIndex) {
			return CalculateLastExistingLayoutVisibleColumnRange(maxCellColumnIndex);
		}
		IColumnRange CalculateLastExistingLayoutVisibleColumnRange(int maxCellColumnIndex) {
			int lastColumnIndex = Sheet.MaxColumnCount - 1;
			Enumerable<IColumnRange> existingReversedColumnsEnumberable = new Enumerable<IColumnRange>(Sheet.Columns.GetExistingColumnRangesEnumerator(0, lastColumnIndex, true));
			foreach (IColumnRange column in existingReversedColumnsEnumberable) {
				if (column.EndIndex < lastColumnIndex) {
					if (column.HasVisibleBorder)
						return column;
				}
			}
			return null;
		}
		protected IColumnRange CalculateLastExistingDesignLayoutVisibleColumnRange(int maxCellColumnIndex) {
			int lastColumnIndex = Sheet.MaxColumnCount - 1;
			foreach (IColumnRange column in new Enumerable<IColumnRange>(Sheet.Columns.GetExistingColumnRangesEnumerator(0, lastColumnIndex, true))) {
				if (column.EndIndex < lastColumnIndex) {
					if (column.HasVisibleBorder)
						return column;
				}
			}
			return null;
		}
		protected virtual void CalculateMergedCellsPrintRange(RangeIntermediateInfo result) {
			foreach (CellRange mergedRange in Sheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable()) {
				if (mergedRange.IsColumnRangeInterval() || mergedRange.IsRowRangeInterval())
					continue;
				ProcessMergedRange(result, mergedRange);
			}
		}
		protected void ProcessMergedRange(RangeIntermediateInfo result, CellRange mergedRange) {
			CellPosition topLeft = mergedRange.TopLeft;
			CellPosition bottomRight = mergedRange.BottomRight;
			result.ExpandToRightColumn(CalculateLastVisibleColumnRightFromTo(topLeft.Column, bottomRight.Column));
			result.ExpandToBottomRow(CalculateLastVisibleRow(topLeft.Row, bottomRight.Row));
		}
		protected virtual void CalculateRangeIncludesEveryDrawingObject(RangeIntermediateInfo result) {
			foreach (IDrawingObject drawing in Sheet.DrawingObjects) {
				ProcessDrawingObject(result, drawing);
			}
		}
		protected void ProcessDrawingObject(RangeIntermediateInfo result, IDrawingObject drawingObject) {
			result.ExpandToRightColumn(drawingObject.To.Col);
			result.ExpandToBottomRow(drawingObject.To.Row);
			result.ExpandToLeftColumn(drawingObject.From.Col);
			result.ExpandToTopRow(drawingObject.From.Row);
		}
		protected int CalculateLastVisibleColumnRightFromTo(int from, int to) {
			ColumnCollection columns = Sheet.Columns;
			if (columns.Count <= 0) 
				return to;
			for (int i = to; i >= from; i--) {
				IColumnRange columnRange = columns.TryGetColumnRange(i);
				if (columnRange == null)
					return i;
				if (columnRange.IsVisible)
					return i;
				i = columnRange.StartIndex; 
			}
			return RangeIntermediateInfo.BottomRightInitial;
		}
		protected int CalculateLastVisibleRow(int from, int to) {
			IRowCollection rows = Sheet.Rows;
			if (rows.Count <= 0) 
				return to;
			for (int i = to; i >= from; i--) {
				Row row = rows.TryGetRow(i);
				if (row == null || row.IsVisible)
					return i;
			}
			return RangeIntermediateInfo.BottomRightInitial;
		}
		protected ICell GetRightmostCell(ICellCollection cells, int firstColumnIndexWhereCellsNotCanBe, int lastColumnIndexWhereCellsNotCanBe) {
			ICell result = null;
			if (Sheet.Columns.Count <= 0) {
				result = cells.TryGetLastCellWith(CheckCellVisible, firstColumnIndexWhereCellsNotCanBe, lastColumnIndexWhereCellsNotCanBe);
				return result;
			}
			for (int i = cells.Count - 1; i >= 0; i--) {
				result = cells.InnerList[i];
				int columnIndex = result.ColumnIndex;
				if (columnIndex <= firstColumnIndexWhereCellsNotCanBe)
					return null;
				if (columnIndex >= lastColumnIndexWhereCellsNotCanBe)
					continue;
				IColumnRange columnRange = Sheet.Columns.TryGetColumnRange(columnIndex);
				if (columnRange == null || columnRange.IsVisible) {
					if (CheckCellVisible(result))
						return result;
				}
			}
			return null;
		}
	}
}
