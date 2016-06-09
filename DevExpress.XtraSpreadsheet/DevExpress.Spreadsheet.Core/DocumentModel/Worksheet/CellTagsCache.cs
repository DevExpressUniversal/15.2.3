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
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellTagsCache
	public class CellTagsCache {
		#region Fields
		readonly Worksheet sheet;
		readonly Dictionary<CellPosition, object> cache = new Dictionary<CellPosition, object>();
		int left;
		int right;
		int top;
		int bottom;
		#endregion
		public CellTagsCache(Worksheet sheet) {
			this.sheet = sheet;
			SetDefaultSizes();
		}
		bool IsValid { get { return cache.Count > 0; } }
		public bool IsEmpty { get { return !IsValid; } }
		CellRange GetSizedRange(ICellTable worksheet) {
			return new CellRange(worksheet, new CellPosition(left, top), new CellPosition(right, bottom));
		}
		void SetDefaultSizes() {
			left = IndicesChecker.MaxColumnCount;
			right = -1;
			top = IndicesChecker.MaxRowCount;
			bottom = -1;
		}
		public object GetValue(CellPosition key) {
			object result = null;
			cache.TryGetValue(key, out result);
			return result;
		}
		public object GetValue(int column, int row) {
			CellPosition key = new CellPosition(column, row);
			return GetValue(key);
		}
		#region SetValue / NoHistory
		public void SetValue(int column, int row, object value) {
			if (!IndicesChecker.CheckIsColumnIndexValid(column))
				Exceptions.ThrowArgumentException("column", column);
			if (!IndicesChecker.CheckIsRowIndexValid(row))
				Exceptions.ThrowArgumentException("row", row);
			SetValue(new CellPosition(column, row), value);
		}
		public void SetValue(CellPosition key, object value) {
			if (value == null)
				RemoveValueCore(key);
			else
				SetValueCore(key, value);
		}
		void SetValueCore(CellPosition cell, object value) {
			cache[cell] = value;
			UpdateBoundingRangeSizes(cell.Column, cell.Row);
		}
		void RemoveValueCore(CellPosition cell) {
			cache.Remove(cell);
			SetDefaultSizes();
			foreach (CellPosition key in cache.Keys)
				UpdateBoundingRangeSizes(key.Column, key.Row);
		}
		void UpdateBoundingRangeSizes(int column, int row) {
			left = Math.Min(left, column);
			right = Math.Max(right, column);
			top = Math.Min(top, row);
			bottom = Math.Max(bottom, row);
		}
		internal void ClearCore() {
			cache.Clear();
			SetDefaultSizes();
		}
		#endregion
		#region SetValue / WithHistory
		public void ExchangeValues(CellPosition firstCell, CellPosition secondCell) {
			object firstTag = GetValue(firstCell.Column, firstCell.Row);
			object secondTag = GetValue(secondCell.Column, secondCell.Row);
			if (firstTag == null && secondTag == null)
				return;
			sheet.Workbook.BeginUpdate();
			try {
				SetValue(firstCell, firstTag, secondTag);
				SetValue(secondCell, secondTag, firstTag);
			}
			finally {
				sheet.Workbook.EndUpdate();
			}
		}
		void SetValue(CellPosition cell, object oldTag, object newTag) {
			DocumentHistory history = sheet.Workbook.History;
			SpreadsheetCellTagsChangedHistoryItem historyItem = new SpreadsheetCellTagsChangedHistoryItem(sheet, cell, oldTag, newTag);
			history.Add(historyItem);
			historyItem.Execute();
		}
		void RemoveValue(CellPosition cell, object oldTag) {
			SetValue(cell, oldTag, null);
		}
		void Clear() {
			DocumentHistory history = sheet.Workbook.History;
			SpreadsheetCellTagsClearedHistoryItem historyItem = new SpreadsheetCellTagsClearedHistoryItem(sheet, cache);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public IEnumerable<KeyValuePair<CellPosition, object>> GetExistingCellTagsFrom(CellRange range) {
			if (IsValid) {
				CellRange rangeCoveredAllTags = GetSizedRange(this.sheet);
				CellRange intersection = range.Intersection(rangeCoveredAllTags);
				CellPosition key = CellPosition.InvalidValue;
				foreach (var keyValue in cache) {
					CellPosition position = keyValue.Key;
					if (intersection.ContainsCell(position.Column, position.Row))
						yield return keyValue;
				}
			}
		}
		#region OnRangeRemoving
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			if (!IsValid)
				return;
			CellRange removingRange = notificationContext.Range;
			OnRangeRemovingDefault(removingRange);
			if (!IsValid)
				return;
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				OnRangeRemovingShiftLeft(removingRange);
			else if (mode == RemoveCellMode.ShiftCellsUp)
				OnRangeRemovingShiftUp(removingRange);
		}
		void OnRangeRemovingShiftLeft(CellRange cellRange) {
			if (cellRange.IsColumnRangeInterval()) {
				if (right < cellRange.LeftColumnIndex)
					return;
				if (left > cellRange.RightColumnIndex)
					CommonOffset(-cellRange.Width, 0);
				else if (left >= cellRange.LeftColumnIndex && right <= cellRange.RightColumnIndex)
					Clear();
				else
					Offset(cellRange.RightColumnIndex + 1, right, top, bottom, -cellRange.Width, 0);
			}
			else
				Offset(cellRange.RightColumnIndex + 1, right, cellRange.TopRowIndex, cellRange.BottomRowIndex, -cellRange.Width, 0);
		}
		void OnRangeRemovingShiftUp(CellRange cellRange) {
			if (cellRange.IsRowRangeInterval()) {
				if (bottom < cellRange.TopRowIndex)
					return;
				if (top > cellRange.BottomRowIndex)
					CommonOffset(0, -cellRange.Height);
				else if (top >= cellRange.TopRowIndex && right <= cellRange.BottomRowIndex)
					Clear();
				else
					Offset(left, right, cellRange.BottomRowIndex + 1, bottom, 0, -cellRange.Height);
			}
			else
				Offset(cellRange.LeftColumnIndex, cellRange.RightColumnIndex, cellRange.BottomRowIndex + 1, bottom, 0, -cellRange.Height);
		}
		void OnRangeRemovingDefault(CellRange cellRange) {
			if (cellRange.Includes(GetSizedRange(cellRange.Worksheet))) {
				Clear();
				return;
			}
			for (int col = cellRange.LeftColumnIndex; col <= cellRange.RightColumnIndex; col++)
				for (int row = cellRange.TopRowIndex; row <= cellRange.BottomRowIndex; row++) {
					CellPosition key = new CellPosition(col, row);
					if (cache.ContainsKey(key))
						RemoveValue(key, cache[key]);
				}
		}
		#endregion
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (!IsValid)
				return;
			InsertCellMode mode = notificationContext.Mode;
			if (mode == InsertCellMode.ShiftCellsDown)
				OnRangeInsertingShiftDown(notificationContext.Range);
			else if (mode == InsertCellMode.ShiftCellsRight)
				OnRangeInsertingShiftRight(notificationContext.Range);
			else
				OnRangeInsertingDefault(notificationContext.Range);
		}
		void OnRangeInsertingShiftRight(CellRange cellRange) {
			if (cellRange.IsColumnRangeInterval()) {
				if (right < cellRange.LeftColumnIndex)
					return;
				if (left >= cellRange.LeftColumnIndex)
					CommonOffset(cellRange.Width, 0);
				else
					Offset(cellRange.LeftColumnIndex, right, top, bottom, cellRange.Width, 0);
			}
			else
				Offset(cellRange.LeftColumnIndex, right, cellRange.TopRowIndex, cellRange.BottomRowIndex, cellRange.Width, 0);
		}
		void OnRangeInsertingShiftDown(CellRange cellRange) {
			if (cellRange.IsRowRangeInterval()) {
				if (bottom < cellRange.TopRowIndex)
					return;
				if (top >= cellRange.TopRowIndex)
					CommonOffset(0, cellRange.Height);
				else
					Offset(left, right, cellRange.TopRowIndex, bottom, 0, cellRange.Height);
			}
			else
				Offset(cellRange.LeftColumnIndex, cellRange.RightColumnIndex, cellRange.TopRowIndex, bottom, 0, cellRange.Height);
		}
		void OnRangeInsertingDefault(CellRange cellRange) {
			OnRangeRemovingDefault(cellRange);
		}
		#endregion
		void CommonOffset(int colOffset, int rowOffset) {
			Offset(left, right, top, bottom, colOffset, rowOffset);
		}
		public void ShiftTagsInRange(CellRange range, CellPositionOffset offset) {
			if (!IsValid)
				return;
			if (range is CellIntervalRange) {
				CommonOffset(offset.ColumnOffset, offset.RowOffset);
				return;
			}
			Offset(range.LeftColumnIndex,
				range.RightColumnIndex,
				range.TopRowIndex,
				range.BottomRowIndex,
				offset.ColumnOffset,
				offset.RowOffset);
		}
		void Offset(int startColumn, int endColumn, int startRow, int endRow, int colOffset, int rowOffset) {
			if (startColumn > right || startRow > bottom)
				return;
			endColumn = Math.Min(right, endColumn);
			endRow = Math.Min(bottom, endRow);
			if (rowOffset >= 0)
				for (int row = endRow; row >= startRow; row--)
					OffsetInLoneRow(row, startColumn, endColumn, colOffset, rowOffset);
			else if (rowOffset < 0)
				for (int row = startRow; row <= endRow; row++)
					OffsetInLoneRow(row, startColumn, endColumn, colOffset, rowOffset);
		}
		void OffsetInLoneRow(int row, int startColumn, int endColumn, int colOffset, int rowOffset) {
			if (colOffset >= 0)
				for (int col = endColumn; col >= startColumn; col--)
					LoneOffset(col, row, colOffset, rowOffset);
			else if (colOffset < 0)
				for (int col = startColumn; col <= endColumn; col++)
					LoneOffset(col, row, colOffset, rowOffset);
		}
		void LoneOffset(int column, int row, int colOffset, int rowOffset) {
			CellPosition currentPosition = new CellPosition(column, row);
			CellPosition newPosition = new CellPosition(column + colOffset, row + rowOffset);
			ExchangeValues(currentPosition, newPosition);
		}
	}
	#endregion
}
