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
	public class UsedRangeCalculator  {
		readonly Worksheet sheet;
		readonly int defaultFormatIndex;
		public UsedRangeCalculator(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			defaultFormatIndex = sheet.Workbook.StyleSheet.DefaultCellFormatIndex;
		}
		protected Worksheet Sheet { get { return sheet; } }
		public bool IncludeEmptyCells { get; set; }
		public CellRange Calculate() {
			RangeIntermediateInfo result = new RangeIntermediateInfo();
			CalculateRangeWithNonEmptyCells(result);
			CalculateRangeIncludesFormattedColumns(result);
			CalculateRootMergedCellRange(result);
			CalculateLastTableBottomRight(Sheet.Tables, result);
			bool returnFirstCellFromRangeForEmptyDocument = true;
			return result.ToRange(Sheet, returnFirstCellFromRangeForEmptyDocument);
		}
		void CalculateRangeIncludesFormattedColumns(RangeIntermediateInfo result) {
			List<Column> innerColumns = Sheet.Columns.InnerList; 
			int count = innerColumns.Count;
			if (count == 0)
				return;
			Predicate<Column> columnHasFormatting = column => { return 
				column.FormatIndex > defaultFormatIndex
				&& (column.EndIndex - column.StartIndex < IndicesChecker.MaxColumnCount / 2); };
			for (int i = innerColumns.Count - 1; i >= 0; i--) {
				Column column = innerColumns[i];
				if (columnHasFormatting(column)) {
					result.ExpandToLeftColumn(column.StartIndex);
					result.ExpandToRightColumn(column.EndIndex);
					break;
				}
			}
			if (count == 1)
				return;
			Column first = innerColumns.Find(columnHasFormatting);
			if (first != null) {
				result.ExpandToLeftColumn(first.StartIndex);
				result.ExpandToRightColumn(first.EndIndex);
			}
		}
		void CalculateRootMergedCellRange(RangeIntermediateInfo result) {
			foreach (CellRange mergedCell in Sheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable()) {
				if (mergedCell.IsWholeWorksheetRange()) {
					result.ExpandToLeftColumn(mergedCell.TopLeft.Column);
					result.ExpandToTopRow(mergedCell.TopLeft.Row);
				}
				else if (mergedCell.IsColumnRangeInterval()) {
					result.ExpandToLeftColumn(mergedCell.TopLeft.Column);
					result.ExpandToRightColumn(mergedCell.BottomRight.Column);
				}
				else if (mergedCell.IsRowRangeInterval()) {
					result.ExpandToTopRow(mergedCell.TopLeft.Row);
					result.ExpandToBottomRow(mergedCell.BottomRight.Row);
				}
				else {
					result.Expand(mergedCell);
				}
			}
		}
		void CalculateRangeWithNonEmptyCells(RangeIntermediateInfo result) {
			int lastrowIndex = Sheet.MaxRowCount - 1;
			foreach (Row row in Sheet.Rows.GetExistingRows(0, lastrowIndex, false)) {
				if (row.FormatIndex > defaultFormatIndex) {
					result.ExpandToTopRow(row.Index);
					result.ExpandToBottomRow(row.Index);
				}
				if (row.CellsCount == 0)
					continue;
				CalculateCellRangeWithContentAndFormattingInRow(row, result);
			}
		}
		void CalculateCellRangeWithContentAndFormattingInRow(Row row, RangeIntermediateInfo allCellsWithContent) {
			ICellCollection cells = row.Cells;
			Func<ICell, bool> condition = cell => { return ProcessCellCondition(cell); };
			int firstColumnIndexWhereCellsNotCanBe = 0 - 1;
			int lastColumnIndexWhereCellsNotCanBe = sheet.MaxColumnCount;
			ICell rightCell = cells.TryGetLastCellWith(condition, firstColumnIndexWhereCellsNotCanBe, lastColumnIndexWhereCellsNotCanBe);
			if (rightCell == null)
				return;
			allCellsWithContent.ExpandToRightColumn(rightCell.ColumnIndex);
			allCellsWithContent.ExpandToTopRow(row.Index);
			allCellsWithContent.ExpandToBottomRow(row.Index);
			ICell firstContentCell = cells.TryGetFirstCellWith(condition);
			allCellsWithContent.ExpandToLeftColumn(firstContentCell.ColumnIndex);
		}
		bool ProcessCellCondition(ICell cell) {
			bool cellHasFormatOrContent = cell.HasContent || cell.FormatIndex > defaultFormatIndex;
			return IncludeEmptyCells ? cellHasFormatOrContent || cell.Value.IsEmpty : cellHasFormatOrContent;
		}
		protected void CalculateLastTableBottomRight(TableCollection tables, RangeIntermediateInfo result) {
			CellRange tableRange = null;
			foreach (Table table in tables) {
				tableRange = table.Range;
				result.Expand(tableRange);
			}
		}
	}
	public class RangeIntermediateInfo {
		static RangeIntermediateInfo Initial = new RangeIntermediateInfo();
		public const int TopLeftInitial = Int32.MaxValue;
		public const int BottomRightInitial = Int32.MinValue;
		int topRow;
		int bottomRow;
		int rightColumn;
		int leftColumn;
		public RangeIntermediateInfo() {
			topRow = TopLeftInitial;
			bottomRow = BottomRightInitial;
			rightColumn = BottomRightInitial;
			leftColumn = TopLeftInitial;
		}
		public int TopRow { get { return topRow; } }
		public int BottomRow { get { return bottomRow; } }
		public int RightColumn { get { return rightColumn; } }
		public int LeftColumn { get { return leftColumn; } }
		public void ExpandToRightColumn(int index) {
			rightColumn = Math.Max(rightColumn, index);
		}
		public void ExpandToLeftColumn(int index) {
			leftColumn = Math.Min(leftColumn, index);
		}
		public void ExpandToTopRow(int index) {
			topRow = Math.Min(topRow, index);
		}
		public void ExpandToBottomRow(int index) {
			bottomRow = Math.Max(bottomRow, index);
		}
		public CellRange ToRange(Worksheet sheet, bool returnFirstCellFromRangeForEmptyDocument) {
			CellPosition start = new CellPosition(leftColumn, topRow);
			CellPosition end = new CellPosition(rightColumn, bottomRow);
			if (!start.IsValid && !end.IsValid) {
				if (!returnFirstCellFromRangeForEmptyDocument)
					return null;
				int correctedTopRow = (topRow == Initial.topRow) ? 0 : topRow;
				int correctedLeftColumn = (leftColumn == Initial.leftColumn) ? 0 : leftColumn;
				int correctedBottomRow = bottomRow == Initial.bottomRow ? 0 : bottomRow;
				int correctedRightColumn = rightColumn == Initial.rightColumn ? 0 : rightColumn;
				start = new CellPosition(correctedLeftColumn, correctedTopRow);
				end = new CellPosition(correctedRightColumn, correctedBottomRow);
			}
			else if (start.IsValid && !end.IsValid) {
				end = start;
			}
			else if (!start.IsValid || !end.IsValid) {
				if (!returnFirstCellFromRangeForEmptyDocument)
					return null;
				start = new CellPosition(0, 0);
				end = new CellPosition(0, 0);
			}
			return TryConvertToIntervalRange(sheet, start, end);
		}
		CellRange TryConvertToIntervalRange(Worksheet sheet, CellPosition start, CellPosition end) {
			CellRange result = null;
			if (CellRange.CheckIsColumnRangeInterval(start, end, sheet))
				result = CellIntervalRange.CreateColumnInterval(sheet,
					start.Column, start.ColumnType,
					end.Column, end.ColumnType);
			else if (CellRange.CheckIsRowRangeInterval(start, end, sheet))
				result = CellIntervalRange.CreateRowInterval(sheet,
					start.Row, start.RowType,
					end.Row, end.RowType);
			return result == null ? new CellRange(sheet, start, end) : result;
		}
		public void Expand(RangeIntermediateInfo other) {
			ExpandToRightColumn(other.RightColumn);
			ExpandToLeftColumn(other.leftColumn);
			ExpandToTopRow(other.TopRow);
			ExpandToBottomRow(other.BottomRow);
		}
		public void Expand(CellRange range) {
			ExpandToRightColumn(range.BottomRight.Column);
			ExpandToLeftColumn(range.TopLeft.Column);
			ExpandToTopRow(range.TopLeft.Row);
			ExpandToBottomRow(range.BottomRight.Row);
		}
		public bool TopChanged { get { return topRow != TopLeftInitial; } }
		public bool LeftChanged { get { return leftColumn != TopLeftInitial; } }
		public bool RightChanged { get { return rightColumn != BottomRightInitial; } }
		public bool BottomChanged { get { return bottomRow != BottomRightInitial; } }
	}
}
