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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region  WebRanges
	public class WebRanges {
		#region static
		static int GetIndex(List<int> list, int value) {
			int result = Algorithms.BinarySearch(list, new ValueComparable(value));
			if (result < 0)
				result = ~result - 1;
			return result;
		}
		#endregion
		#region inner classes
		class ValueComparable : IComparable<int> {
			readonly int value;
			public ValueComparable(int value) {
				this.value = value;
			}
			public int CompareTo(int other) {
				return other - value;
			}
		}
		class RangeInterval {
			public int FirstPositionIndex { get; private set; }
			public int LastPositionIndex { get; private set; }
			public RangeInterval(int firstPositionIndex, int lastPositionIndex) {
				FirstPositionIndex = firstPositionIndex;
				LastPositionIndex = lastPositionIndex;
			}
		}
		class VisibleRangeInformation {
			public List<int> BoundModelIndices = new List<int>();
			public List<RangeInterval> TileRanges = new List<RangeInterval>();
		}
		#endregion
		#region fields
		List<CellRange> unchangedRanges;
		List<CellRange> changedRanges;
		Dictionary<CellRange, List<CellRange>> chartRanges;
		List<CellPosition> changedCellPositions;
		int top;
		int left;
		int bottom;
		int right;
		Worksheet sheet;
		int tileWidth;
		int tileHeight;
		#endregion
		public WebRanges(Worksheet sheet) {
			this.sheet = sheet;
			changedRanges = new List<CellRange>();
			changedCellPositions = new List<CellPosition>();
			unchangedRanges = new List<CellRange>();
		}
		#region Properties
		public CellPosition FirstVisiblePosition { get { return GetFirstVisiblePosition(); ; } }
		#endregion
		public void SetTilesSize(int tileWidth, int tileHeight) {
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
		}
		public Dictionary<TilePosition, CellRange> GetTilesByRange(CellRange range) {
			VisibleRangeInformation columnTileRanges = ResolveColumnTileRanges(range.BottomRight.Column / tileWidth + 1);
			VisibleRangeInformation rowTileRanges = ResolveRowTileRanges(range.BottomRight.Row / tileHeight + 1);
			int firstColumnTile = GetIndex(columnTileRanges.BoundModelIndices, range.TopLeft.Column);
			int lastColumnTile = GetIndex(columnTileRanges.BoundModelIndices, range.BottomRight.Column);
			int firstRowTile = GetIndex(rowTileRanges.BoundModelIndices, range.TopLeft.Row);
			int lastRowTile = GetIndex(rowTileRanges.BoundModelIndices, range.BottomRight.Row);
			Dictionary<TilePosition, CellRange> tiles = new Dictionary<TilePosition, CellRange>();
			for (int rowIndex = firstRowTile; rowIndex < lastRowTile + 1; rowIndex++)
				for (int columnIndex = firstColumnTile; columnIndex < lastColumnTile + 1; columnIndex++) {
					TilePosition tilePosition = new TilePosition(columnIndex, rowIndex);
					CellRange tileRange = new CellRange(sheet, new CellPosition(columnTileRanges.TileRanges[tilePosition.TileColumn].FirstPositionIndex, rowTileRanges.TileRanges[tilePosition.TileRow].FirstPositionIndex),
															   new CellPosition(columnTileRanges.TileRanges[tilePosition.TileColumn].LastPositionIndex, rowTileRanges.TileRanges[tilePosition.TileRow].LastPositionIndex));
					tiles.Add(tilePosition, tileRange);
				}
			return tiles;
		}
		public List<CellRange> GetChangedVisibleRanges(CellRange visibleRange) {
			List<CellRange> resultRanges = new List<CellRange>();
			foreach (CellRange range in changedRanges)
				if (visibleRange.Intersects(range))
					resultRanges.Add(range);
			return resultRanges;
		}
		VisibleRangeInformation ResolveColumnTileRanges(int columnTileCount) {
			VisibleRangeInformation columnTileRanges = new VisibleRangeInformation();
			int currentColumn = 0;
			columnTileRanges.BoundModelIndices.Add(currentColumn);
			for (int columnTile = 0; columnTile < columnTileCount; columnTile++) {
				RangeInterval rangePosition = GetTileModelRange(currentColumn, true);
				currentColumn = rangePosition.LastPositionIndex + 1;
				columnTileRanges.BoundModelIndices.Add(currentColumn);
				columnTileRanges.TileRanges.Add(rangePosition);
			}
			return columnTileRanges;
		}
		VisibleRangeInformation ResolveRowTileRanges(int rowTileCount) {
			VisibleRangeInformation rowTileRanges = new VisibleRangeInformation();
			int currentRow = 0;
			rowTileRanges.BoundModelIndices.Add(currentRow);
			for (int rowTitle = 0; rowTitle < rowTileCount; rowTitle++) {
				RangeInterval rangePosition = GetTileModelRange(currentRow, false);
				currentRow = rangePosition.LastPositionIndex + 1;
				rowTileRanges.BoundModelIndices.Add(currentRow);
				rowTileRanges.TileRanges.Add(rangePosition);
			}
			return rowTileRanges;
		}
		RangeInterval GetTileModelRange(int startSearchIndex, bool isCol) {
			int firesRangeIndex = startSearchIndex,
				visibleCellCount = 0,
				currentColumn = startSearchIndex - 1;
			while (visibleCellCount < tileWidth) {
				currentColumn++;
				if ((isCol && !sheet.IsColumnVisible(currentColumn)) || (!isCol && !sheet.IsRowVisible(currentColumn))) {
					if (firesRangeIndex == currentColumn) 
						firesRangeIndex++;
				}
				else {
					visibleCellCount++;
				}
			}
			return new RangeInterval(firesRangeIndex, currentColumn);
		}
		internal void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (unchangedRanges == null || unchangedRanges.Count == 0)
				return;
			InsertCellMode mode = notificationContext.Mode;
			if (mode == InsertCellMode.ShiftCellsRight)
				OnRangeRemovingCore(notificationContext.Range, RemoveCellMode.ShiftCellsLeft);
			else if (mode == InsertCellMode.ShiftCellsDown)
				OnRangeRemovingCore(notificationContext.Range, RemoveCellMode.ShiftCellsUp);
		}
		internal void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			OnRangeRemovingCore(notificationContext.Range, notificationContext.Mode);
		}
		internal void OnRangeRemovingCore(CellRange range, RemoveCellMode mode) {
			if (unchangedRanges == null || unchangedRanges.Count == 0)
				return;
			CellRange prohibitedRange = range;
			Worksheet sheet = range.Worksheet as Worksheet;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				prohibitedRange = new CellRange(sheet, range.TopLeft, new CellPosition(sheet.MaxColumnCount, range.BottomRight.Row));
			else if (mode == RemoveCellMode.ShiftCellsUp)
				prohibitedRange = new CellRange(sheet, range.TopLeft, new CellPosition(range.BottomRight.Column, sheet.MaxRowCount));
			ChangeRangeCore(prohibitedRange);
		}
		internal void ChangeRange(CellRange changingRange) {
			if (unchangedRanges == null || unchangedRanges.Count == 0)
				return;
			ChangeRangeCore(changingRange);
		}
		internal CellRange RoundRangeByTiles(CellRange visibleRange) {
			CellPosition topLeft = CellPosition.InvalidValue;
			CellPosition bottomRight = CellPosition.InvalidValue;
			List<CellRange> processedRanges = unchangedRanges;
			foreach (CellRange range in processedRanges) {
				if (!topLeft.IsValid && range.ContainsCell(visibleRange.LeftColumnIndex, visibleRange.TopRowIndex))
					topLeft = range.TopLeft;
				if (!bottomRight.IsValid && range.ContainsCell(visibleRange.RightColumnIndex, visibleRange.BottomRowIndex))
					bottomRight = range.BottomRight;
				if (topLeft.IsValid && bottomRight.IsValid)
					break;
			}
			return new CellRange(sheet, topLeft.IsValid ? topLeft : visibleRange.TopLeft, bottomRight.IsValid ? bottomRight : visibleRange.BottomRight);
		}
		void ChangeRangeCore(CellRange changingRange) {
			List<CellRange> changedCharts = new List<CellRange>();
			foreach (CellRange chartRange in chartRanges.Keys)
				if (changingRange.Intersects(chartRange)) {
					changedRanges.AddRange(chartRanges[chartRange]);
					changedCharts.Add(chartRange);
				}
			foreach (CellRange chartRange in changedCharts)
				chartRanges.Remove(chartRange);
			foreach (CellRange range in unchangedRanges)
				if (changingRange.Intersects(range) && !changedRanges.Contains(range))
					changedRanges.Add(range);
			foreach (CellRange range in changedRanges)
				if (unchangedRanges.Contains(range))
					unchangedRanges.Remove(range);
		}
		CellPosition GetFirstVisiblePosition() {
			int row = GetFirstVisibleRow();
			int col = GetFirstVisibleColumn();
			if (row != -1 && col != -1)
				return new CellPosition(col, row);
			return CellPosition.InvalidValue;
		}
		int GetFirstVisibleRow() {
			if (sheet.Rows.InnerList.Count == 0 || sheet.Rows.InnerList[0].Index > 0)
				return 0;
			int previousRowIndex = -1;
			foreach (Row row in sheet.Rows.InnerList) {
				if (previousRowIndex != -1 && previousRowIndex + 1 != row.Index)
					return previousRowIndex + 1;
				if (sheet.IsRowVisible(row.Index))
					return row.Index;
				previousRowIndex = row.Index;
			}
			if (previousRowIndex + 1 >= IndicesChecker.MaxRowCount)
				return -1;
			return previousRowIndex + 1;
		}
		int GetFirstVisibleColumn() {
			if (sheet.Columns.InnerList.Count == 0 || sheet.Columns.InnerList[0].Index > 0)
				return 0;
			int previousColumnIndex = -1;
			foreach (Column column in sheet.Columns.InnerList) {
				if (previousColumnIndex != -1 && previousColumnIndex + 1 != column.Index)
					return previousColumnIndex + 1;
				if (sheet.IsColumnVisible(column.Index))
					return column.Index;
				previousColumnIndex = column.Index;
			}
			if (previousColumnIndex + 1 >= IndicesChecker.MaxColumnCount)
				return -1;
			return previousColumnIndex + 1;
		}
		internal void AddChangedCellPosition(ICell cell) {
			if (unchangedRanges == null || unchangedRanges.Count == 0)
				return;
			CellPosition position = cell.Position;
			if (changedCellPositions.Contains(position))
				return;
			top = Math.Min(top, position.Row);
			bottom = Math.Max(bottom, position.Row);
			left = Math.Min(left, position.Column);
			right = Math.Max(right, position.Column);
			changedCellPositions.Add(position);
		}
		public void BeginUpdate(List<CellRange> unchangedRanges) {
			changedRanges.Clear();
			changedCellPositions.Clear();
			InitializeTLBR();
			this.unchangedRanges = unchangedRanges;
			InitializeCharts();
		}
		public List<CellRange> EndUpdate() {
			if (unchangedRanges.Count > 0) {
				CellRange changedRange = new CellRange(unchangedRanges[0].Worksheet, new CellPosition(left, top), new CellPosition(right, bottom));
				foreach (CellRange chartRange in chartRanges.Keys)
					if (!ProcessChangedChartCells(chartRange, changedRange, chartRanges[chartRange]))
						ProcessIndirectChartChanges(chartRange, chartRanges[chartRange]);
				foreach (CellRange range in unchangedRanges)
					if (!changedRanges.Contains(range))
						if (!ProcessChangedCells(range, changedRange))
							ProcessIndirectChanges(range);
			}
			return changedRanges;
		}
		bool ProcessChangedCells(CellRange range, CellRange cellChangedRange) {
			if (changedCellPositions.Count == 0 || !range.Intersects(cellChangedRange))
				return false;
			bool result = false;
			foreach (CellPosition cellPosition in changedCellPositions)
				if (range.ContainsCell(cellPosition.Column, cellPosition.Row)) {
					changedRanges.Add(range);
					changedCellPositions.Remove(cellPosition);
					result = true;
					break;
				}
			return result;
		}
		bool ProcessChangedChartCells(CellRange range, CellRange cellChangedRange, List<CellRange> chartRanges) {
			if (changedCellPositions.Count == 0 || !range.Intersects(cellChangedRange))
				return false;
			bool result = false;
			foreach (CellPosition cellPosition in changedCellPositions)
				if (range.ContainsCell(cellPosition.Column, cellPosition.Row)) {
					changedRanges.AddRange(chartRanges);
					changedCellPositions.Remove(cellPosition);
					result = true;
					break;
				}
			return result;
		}
		void ProcessIndirectChanges(CellRange range) {
			if (changedRanges.Contains(range))
				return;
			foreach (ICellBase cellInfo in range.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (cell == null)
					continue;
				if (cell.IsValueUpdated()) {
					changedRanges.Add(range);
					break;
				}
			}
		}
		void ProcessIndirectChartChanges(CellRange chartRange, List<CellRange> ranges) {
			foreach (CellBase cellInfo in chartRange.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (cell == null)
					continue;
				if (cell.IsValueUpdated()) {
					changedRanges.AddRange(ranges);
					break;
				}
			}
		}
		void InitializeTLBR() {
			top = IndicesChecker.MaxRowCount;
			bottom = 0;
			left = IndicesChecker.MaxColumnCount;
			right = 0;
		}
		void InitializeCharts() {
			chartRanges = new Dictionary<CellRange, List<CellRange>>();
			if (unchangedRanges == null || unchangedRanges.Count == 0)
				return;
			ICellTable cellTable = unchangedRanges[0].Worksheet;
			Worksheet sheet = cellTable.Workbook.GetSheetById(cellTable.SheetId) as Worksheet;
			foreach (CellRange range in unchangedRanges)
				foreach (IDrawingObject chart in sheet.DrawingObjectsByZOrderCollections.GetDrawingObjects(range, DrawingObjectType.Chart))
					foreach (FormulaReferencedRange referencedRange in (chart as Chart).GetAllReferencedRanges()) {
						CellRange referencedRangeValue = referencedRange.CellRange as CellRange;
						if (chartRanges.ContainsKey(referencedRangeValue))
							chartRanges[referencedRangeValue].Add(range);
						else chartRanges.Add(referencedRangeValue, new List<CellRange> { range });
					}
		}
#if DEBUGTEST
		internal void SetTestChartRanges(Dictionary<CellRange, List<CellRange>> testChartRanges) {
			chartRanges = testChartRanges;
		}
#endif
	}
	public struct TilePosition {
		#region fields
		readonly int tileColumn;
		readonly int tileRow;
		#endregion
		public TilePosition(int tileColumn, int tileRow) {
			this.tileColumn = tileColumn;
			this.tileRow = tileRow;
		}
		#region Properties
		public int TileColumn { get { return tileColumn; } }
		public int TileRow { get { return tileRow; } }
		#endregion
	}
	#endregion
}
