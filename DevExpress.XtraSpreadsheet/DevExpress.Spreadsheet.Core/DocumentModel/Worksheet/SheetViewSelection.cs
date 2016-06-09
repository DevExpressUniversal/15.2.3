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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Layout.Engine;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum ShrinkRange {
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}
	public class SheetViewSelection {
		#region Fields
		public static int InvalidSelectedCommentIndex = -1;
		readonly Worksheet sheet;
		readonly List<CellRange> selectedRanges;
		readonly List<int> selectedDrawingIndexes;
		int selectedCommentIndex = InvalidSelectedCommentIndex;
		int activeRangeIndex;
		CellPosition activeCell;
		#endregion
		public SheetViewSelection(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.selectedRanges = new List<CellRange>();
			this.selectedDrawingIndexes = new List<int>();
			SetSelection(new CellPosition(0, 0));
		}
		#region Properties
		public CellPosition ActiveCell { get { return activeCell; } }
		public int ActiveRangeIndex { get { return activeRangeIndex; } }
		public IList<CellRange> SelectedRanges { get { return selectedRanges; } }
		public List<int> SelectedDrawingIndexes { get { return selectedDrawingIndexes; } }
		public bool IsSingleCell { get { return SelectedRanges.Count == 1 && SelectedRanges[0].CellCount == 1; } }
		public CellRange ActiveRange { get { return SelectedRanges[ActiveRangeIndex]; } }
		public Worksheet Sheet { get { return sheet; } }
		public bool IsDrawingSelected { get { return selectedDrawingIndexes.Count > 0; } }
		public bool IsDrawingMultiSelection { get { return selectedDrawingIndexes.Count > 1; } }
		public bool IsChartSelected { get { return SelectedChart != null; } }
		public bool IsCommentSelected { get { return selectedCommentIndex != InvalidSelectedCommentIndex; } }
		public Chart SelectedChart {
			get {
				int count = selectedDrawingIndexes.Count;
				for (int i = 0; i < count; i++) {
					IDrawingObject drawing = sheet.DrawingObjects[selectedDrawingIndexes[i]];
					if (drawing.DrawingType == DrawingObjectType.Chart)
						return (Chart)drawing;
				}
				return null;
			}
		}
		public bool IsMultiSelection { get { return selectedRanges.Count > 1; } }
		public bool IsEmpty { get; set; }
		public int SelectedCommentIndex { get { return selectedCommentIndex; } }
		#endregion
		#region Events
		#region SelectionChanged
		EventHandler onSelectionChanged;
		internal event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region BeginReferenceEditMode
		EventHandler onBeginReferenceEditMode;
		internal event EventHandler BeginReferenceEditMode { add { onBeginReferenceEditMode += value; } remove { onBeginReferenceEditMode -= value; } }
		protected internal virtual void RaiseBeginReferenceEditMode() {
			if (onBeginReferenceEditMode != null)
				onBeginReferenceEditMode(this, EventArgs.Empty);
		}
		#endregion
		#region EndReferenceEditMode
		EventHandler onEndReferenceEditMode;
		internal event EventHandler EndReferenceEditMode { add { onEndReferenceEditMode += value; } remove { onEndReferenceEditMode -= value; } }
		protected internal virtual void RaiseEndReferenceEditMode() {
			if (onEndReferenceEditMode != null)
				onEndReferenceEditMode(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void SelectComment(int index) {
			ClearDrawingSelection();
			this.selectedCommentIndex = index;
			RaiseSelectionChanged();
		}
		public CellRangeBase AsRange() {
			int count = SelectedRanges.Count;
			if (count == 1)
				return SelectedRanges[0];
			else {
				List<CellRangeBase> ranges = new List<CellRangeBase>(count);
				for (int i = 0; i < count; i++)
					ranges.Add(SelectedRanges[i]);
				return new CellUnion(ranges[0].Worksheet, ranges);
			}
		}
		protected internal void SetActiveRangeIndex(int value) {
			activeRangeIndex = value;
		}
		public void SetSelection(CellRangeBase range) {
			SetSelection(range, true);
		}
		public void SetSelection(CellRangeBase range, bool expandToMergedCellSize) {
			Guard.ArgumentNotNull(range, "selectionRange");
			CellUnion unionRange = range as CellUnion;
			CellPosition activeCell = unionRange != null ? unionRange.GetFirstInnerCellRange().TopLeft : range.TopLeft;
			SetSelection(range, activeCell, expandToMergedCellSize);
		}
		public void SetSelection(CellRangeBase range, CellPosition activeCell, bool expandToMergedCellSize) {
			SetSelectionCore(activeCell, range, expandToMergedCellSize);
			OnSelectionChanged();
		}
		public bool SetSelection(CellPosition activeCellPosition) {
			bool isPageSelected = !IsDrawingSelected && !IsCommentSelected;
			ClearPivotSelection();
			if (isPageSelected && activeCell.Row == activeCellPosition.Row && activeCell.Column == activeCellPosition.Column) {
				if (selectedRanges.Count == 1 && selectedRanges[0].CellCount == 1)
					return false; 
			}
			CellRange range = new CellRange(this.sheet, activeCellPosition, activeCellPosition);
			SetSelection(range, activeCellPosition, true);
			return true;
		}
		public bool SetSelectedRanges(IList<CellRangeBase> ranges, bool expandToMergedCellSize) {
			int count = ranges.Count;
			if (count <= 0)
				return false;
			ICellTable worksheet = ranges[0].Worksheet;
			if (!Object.ReferenceEquals(worksheet, sheet))
				return false;
			for (int i = 1; i < count; i++)
				if (!Object.ReferenceEquals(ranges[i].Worksheet, worksheet))
					return false;
			CellPosition activeCell = ranges[0].TopLeft;
			SetSelectionCore(activeCell, ranges, expandToMergedCellSize);
			OnSelectionChanged();
			return true;
		}
		internal void SetSelectionCore(CellPosition activeCell, IList<CellRangeBase> ranges, bool expandToMergedCellSize) {
			Guard.ArgumentNotNull(ranges, "ranges");
			this.activeCell = GetActiveCellPosition(activeCell, ranges[0]);
			this.activeRangeIndex = 0;
			this.selectedRanges.Clear();
			AppendRanges(ranges, expandToMergedCellSize);
			IsEmpty = false;
		}
		internal void SetSelectionCore(CellPosition activeCell, CellRangeBase range, bool expandToMergedCellSize) {
			Guard.ArgumentNotNull(range, "range");
			this.activeCell = GetActiveCellPosition(activeCell, range);
			this.activeRangeIndex = 0;
			this.selectedRanges.Clear();
			AppendRange(range, expandToMergedCellSize);
			IsEmpty = false;
		}
		CellPosition GetActiveCellPosition(CellPosition activeCell, CellRangeBase range) {
			int col = activeCell.Column;
			int row = activeCell.Row;
			if (range.RangeType == CellRangeType.IntervalRange && !CheckAllSelected(range))
				return GetActiveCellPositionCore(col, row, range.IsColumnRangeInterval());
			return new CellPosition(col, row);
		}
		CellPosition GetActiveCellPositionCore(int col, int row, bool isColumn) {
			int initialRow = row;
			int initialCol = col;
			int maxRowIndex = sheet.MaxRowCount - 1;
			int maxColumnIndex = sheet.MaxColumnCount - 1;
			CellRange mergedCellRange = sheet.MergedCells.FindMergedCell(col, row);
			while (mergedCellRange != null) {
				int height = mergedCellRange.Height;
				int width = mergedCellRange.Width;
				bool lookNext = isColumn ? row != 0 || width > 1 : col != 0 || height > 1;
				if (!lookNext)
					break;
				row = isColumn ? row + height : row;
				col = isColumn ? col : col + width;
				if (row >= maxRowIndex || col >= maxColumnIndex)
					break;
				mergedCellRange = sheet.MergedCells.FindMergedCell(col, row);
			}
			if (col > maxColumnIndex)
				col = initialCol;
			if (row > maxRowIndex)
				row = initialRow;
			return new CellPosition(col, row);
		}
		bool ReplaceActiveRange(CellRange range) {
			if (ActiveRange.EqualsPosition(range))
				return false;
			int columnIndex = Math.Min(range.BottomRight.Column, Math.Max(range.TopLeft.Column, activeCell.Column));
			int rowIndex = Math.Min(range.BottomRight.Row, Math.Max(range.TopLeft.Row, activeCell.Row));
			activeCell = new CellPosition(columnIndex, rowIndex);
			SelectedRanges[ActiveRangeIndex] = range.TryConvertToCellInterval();
			IsEmpty = false;
			OnSelectionChanged();
			return true;
		}
		void AppendRange(CellRangeBase range, bool expandToMergedCellSize) {
			if (IsEmpty) {
				this.activeRangeIndex = 0;
				selectedRanges.Clear();
			}
			IsEmpty = false;
			CellUnion unionRange = range as CellUnion;
			if (unionRange != null)
				AppendRanges(unionRange.InnerCellRanges, expandToMergedCellSize);
			else {
				CellRange cellRange = (CellRange)range;
				if (expandToMergedCellSize)
					selectedRanges.Add(ExpandRangeToMergedCellSizeSingle(cellRange));
				else
					selectedRanges.Add(cellRange);
			}
		}
		void AppendRanges(IList<CellRangeBase> ranges, bool expandToMergedCellSize) {
			int count = ranges.Count;
			for (int i = 0; i < count; i++)
				AppendRange(ranges[i], expandToMergedCellSize);
		}
		public void AppendSelection(CellRangeBase range) {
			AppendSelection(range, true);
		}
		public void AppendSelection(CellRangeBase range, bool expandToMergedCellSize) {
			AppendRange(range, expandToMergedCellSize);
		}
		public void AppendActiveSelection(CellPosition position) {
			AppendActiveSelection(new CellRange(sheet, position, position));
		}
		public void AppendActiveSelection(CellRangeBase range) {
			AppendActiveSelection(range, true);
		}
		public void AppendActiveSelection(CellRangeBase range, bool expandToMergedCellSize) {
			Guard.ArgumentNotNull(range, "range");
			Debug.Assert(range.RangeType != CellRangeType.UnionRange);
			this.activeCell = GetActiveCellPosition(range.TopLeft, range);
			this.activeRangeIndex = selectedRanges.Count;
			ClearPivotSelection();
			AppendRange(range, expandToMergedCellSize);
			OnSelectionChanged();
		}
		public void ExtendActiveRangeToPosition(CellPosition position) {
			int topRow = Math.Min(position.Row, ActiveCell.Row);
			int leftColumn = Math.Min(position.Column, ActiveCell.Column);
			CellPosition topLeft = new CellPosition(leftColumn, topRow);
			int bottomRow = Math.Max(position.Row, ActiveCell.Row);
			int rightColumn = Math.Max(position.Column, ActiveCell.Column);
			CellPosition bottomRight = new CellPosition(rightColumn, bottomRow);
			ReplaceActiveRange(ExpandRangeToMergedCellSizeSingle(new CellRange(sheet, topLeft, bottomRight)));
		}
		public void FinishSelectionEdit() {
			RaiseEndReferenceEditMode();
		}
		public void StartSelectionEdit() {
			RaiseBeginReferenceEditMode();
		}
		public void ExtendActiveRangeToColumn(int column) {
			int startColumn = Math.Min(ActiveCell.Column, column);
			int endColumn = Math.Max(ActiveCell.Column, column);
			CellIntervalRange range = CellIntervalRange.CreateColumnInterval(sheet, startColumn, PositionType.Absolute, endColumn, PositionType.Absolute);
			ReplaceActiveRange(range);
		}
		public void ExtendActiveRangeToRow(int row) {
			int startRow = Math.Min(ActiveCell.Row, row);
			int endRow = Math.Max(ActiveCell.Row, row);
			CellIntervalRange range = CellIntervalRange.CreateRowInterval(sheet, startRow, PositionType.Absolute, endRow, PositionType.Absolute);
			ReplaceActiveRange(range);
		}
		protected internal void AddSelectionCore(CellRangeBase range, bool expandToMergedCellSize) {
			AppendRange(range, expandToMergedCellSize);
		}
		public void SetSelectedDrawingIndex(int index) {
			ClearDrawingSelection();
			AddSelectedDrawingIndex(index);
		}
		public void AddSelectedDrawingIndex(int index) {
			ClearCommentSelection();
			this.selectedDrawingIndexes.Add(index);
			RaiseSelectionChanged();
		}
		public void RemoveSelectedDrawingIndex(int index) {
			selectedDrawingIndexes.Remove(index);
			RaiseSelectionChanged();
		}
		public void ToggleDrawingSelection(int index) {
			if (selectedDrawingIndexes.Contains(index))
				RemoveSelectedDrawingIndex(index);
			else
				AddSelectedDrawingIndex(index);
		}
		public void ClearDrawingSelection() {
			this.selectedDrawingIndexes.Clear();
		}
		public void ClearCommentSelection() {
			this.selectedCommentIndex = InvalidSelectedCommentIndex;
		}
		public void Clear() {
			selectedRanges.Clear();
			activeRangeIndex = -1;
			activeCell = CellPosition.InvalidValue;
			ClearDrawingSelection();
			ClearCommentSelection();
		}
		void ClearPivotSelection() {
			if (Sheet.PivotSelection != null)
				Sheet.PivotSelection.Clear();
		}
		protected internal void OnDrawingRemoved(int index) {
			for (int i = selectedDrawingIndexes.Count - 1; i >= 0; i--) {
				int selectedIndex = selectedDrawingIndexes[i];
				if (selectedIndex == index)
					RemoveSelectedDrawingIndex(index);
				else if (selectedIndex > index)
					selectedDrawingIndexes[i]--;
			}
		}
		protected internal void OnSelectionChanged() {
			ClearDrawingSelection();
			ClearCommentSelection();
			RaiseSelectionChanged();
		}
		delegate CellPosition CalculateNextPositionMethod(CellPosition position);
		#region MoveLeft (Keybinding: Keys.Left)
		public bool MoveLeft() {
			return SetSelection(CalculateVisiblePositionAtLeft(activeCell));
		}
		CellPosition CalculateVisiblePositionAtLeft(CellPosition position) {
			int columnIndex = GetActualCellRange(position).TopLeft.Column;
			for (int i = columnIndex - 1; i >= 0; i--) {
				if (sheet.IsColumnVisible(i)) {
					columnIndex = i;
					break;
				}
			}
			return new CellPosition(columnIndex, position.Row);
		}
		#endregion
		#region MoveRight (Keybinding: Keys.Right)
		public bool MoveRight() {
			return SetSelection(CalculateVisiblePositionAtRight(activeCell));
		}
		CellPosition CalculateVisiblePositionAtRight(CellPosition position) {
			int columnIndex = GetActualCellRange(position).BottomRight.Column;
			for (int i = columnIndex + 1; i < this.sheet.MaxColumnCount; i++) {
				if (sheet.IsColumnVisible(i)) {
					columnIndex = i;
					break;
				}
			}
			return new CellPosition(columnIndex, position.Row);
		}
		#endregion
		#region MoveUp (Keybinding: Keys.Up)
		public bool MoveUp() {
			return SetSelection(CalculateVisiblePositionAbove(activeCell));
		}
		CellPosition CalculateVisiblePositionAbove(CellPosition position) {
			int rowIndex = GetActualCellRange(position).TopLeft.Row;
			for (int i = rowIndex - 1; i >= 0; i--) {
				if (sheet.IsRowVisible(i)) {
					rowIndex = i;
					break;
				}
			}
			return new CellPosition(position.Column, rowIndex);
		}
		#endregion
		#region MoveDown (Keybinding: Keys.Down)
		public bool MoveDown() {
			return SetSelection(CalculateVisiblePositionBelow(activeCell));
		}
		CellPosition CalculateVisiblePositionBelow(CellPosition position) {
			int rowIndex = GetActualCellRange(position).BottomRight.Row;
			for (int i = rowIndex + 1; i < sheet.MaxRowCount; i++) {
				if (sheet.IsRowVisible(i)) {
					rowIndex = i;
					break;
				}
			}
			return new CellPosition(position.Column, rowIndex);
		}
		#endregion
		#region MoveToLeftColumn (Keybinding: Keys.Home)
		public bool MoveToLeftColumn() {
			return SetSelection(GetActualCellRange(CalculateLeftMostVisiblePosition(activeCell)).TopLeft);
		}
		CellPosition CalculateLeftMostVisiblePosition(CellPosition position) {
			if (sheet.Columns.Count <= 0)
				return new CellPosition(0, position.Row);
			int columnIndex = 0;
			IEnumerator<IColumnRange> enumerator = sheet.Columns.GetExistingColumnRangesEnumerator(0, Math.Max(0, position.Column - 1), false);
			for (; ; ) {
				if (!enumerator.MoveNext())
					break;
				IColumnRange columnRange = enumerator.Current;
				if (columnRange.StartIndex > columnIndex) 
					break;
				if (!columnRange.IsVisible) {
					columnIndex = columnRange.EndIndex + 1;
				}
			}
			return new CellPosition(Math.Min(this.sheet.MaxColumnCount - 1, columnIndex), position.Row);
		}
		#endregion
		#region MoveToTopLeftCell (Keybinding: Keys.Ctrl | Keys.Home)
		public bool MoveToTopLeftCell() {
			return SetSelection(CalculateTopLeftCellPosition(activeCell));
		}
		CellPosition CalculateTopLeftCellPosition(CellPosition position) {
			position = CalculateLeftMostVisiblePosition(position);
			return CalculateUpperMostVisiblePosition(position);
		}
		#endregion
		#region MoveToLastUsedCell (Keybinding: Keys.Ctrl | Keys.End)
		public bool MoveToLastUsedCell() {
			return SetSelection(CalculateLastUsedCellPosition());
		}
		CellPosition CalculateLastUsedCellPosition() {
			return sheet.GetLastUsedCellPosition();
		}
		#endregion
		#region MoveLeftToDataEdge (Keybinding: Keys.Ctrl | Keys.Left)
		public bool MoveLeftToDataEdge() {
			return SetSelection(CalculateDataEdgePositionAtLeft(activeCell));
		}
		CellPosition CalculateDataEdgePositionAtLeft(CellPosition position) {
			IRow row = sheet.Rows.TryGetRow(position.Row);
			if (row == null)
				return CalculateLeftMostVisiblePosition(position);
			CellPosition nextPosition = CalculateDataEdgePositionCore(position, CalculateVisiblePositionAtLeft);
			if (!nextPosition.EqualsPosition(position))
				return nextPosition;
			IEnumerator enumerator = row.Cells.GetExistingNonEmptyVisibleCells(0, Math.Max(0, position.Column - 1), true).GetEnumerator();
			if (!enumerator.MoveNext())
				return CalculateLeftMostVisiblePosition(position);
			CellBase cell = (CellBase)enumerator.Current;
			return new CellPosition(cell.ColumnIndex, position.Row);
		}
		CellPosition CalculateDataEdgePositionCore(CellPosition position, CalculateNextPositionMethod calculateNextPosition) {
			ICellBase activeCell = sheet.TryGetCell(position.Column, position.Row);
			if (activeCell == null || activeCell.Value.IsEmpty)
				return position;
			CellPosition result = position;
			for (; ; ) {
				CellPosition nextPosition = calculateNextPosition(result);
				ICellBase cell = sheet.TryGetCell(nextPosition.Column, nextPosition.Row);
				if (cell != null && !cell.Value.IsEmpty && !nextPosition.EqualsPosition(result))
					result = nextPosition;
				else
					break;
			}
			return result;
		}
		#endregion
		#region MoveRightToDataEdge (Keybinding: Keys.Ctrl | Keys.Right)
		public bool MoveRightToDataEdge() {
			return SetSelection(CalculateDataEdgePositionAtRight(activeCell));
		}
		CellPosition CalculateDataEdgePositionAtRight(CellPosition position) {
			IRowBase row = sheet.Rows.TryGetRow(position.Row);
			if (row == null)
				return CalculateRightMostVisiblePosition(position);
			CellPosition nextPosition = CalculateDataEdgePositionCore(position, CalculateVisiblePositionAtRight);
			if (!nextPosition.EqualsPosition(position))
				return nextPosition;
			IEnumerator enumerator = row.Cells.GetExistingNonEmptyVisibleCells(Math.Min(sheet.MaxColumnCount - 1, position.Column + 1), sheet.MaxColumnCount - 1, false).GetEnumerator();
			if (!enumerator.MoveNext())
				return CalculateRightMostVisiblePosition(position);
			CellBase cell = (CellBase)enumerator.Current;
			return new CellPosition(cell.ColumnIndex, position.Row);
		}
		CellPosition CalculateRightMostVisiblePosition(CellPosition position) {
			if (sheet.Columns.Count <= 0)
				return new CellPosition(sheet.MaxColumnCount - 1, position.Row);
			int columnIndex = sheet.MaxColumnCount - 1;
			IEnumerator<IColumnRange> enumerator = sheet.Columns.GetExistingColumnRangesEnumerator(Math.Min(sheet.MaxColumnCount - 1, position.Column + 1), sheet.MaxColumnCount - 1, true);
			for (; ; ) {
				if (!enumerator.MoveNext())
					break;
				IColumnRange columnRange = enumerator.Current;
				if (columnRange.EndIndex < columnIndex) 
					break;
				if (!columnRange.IsVisible) {
					columnIndex = columnRange.StartIndex - 1;
				}
			}
			return new CellPosition(Math.Max(0, columnIndex), position.Row);
		}
		#endregion
		#region MoveUpToDataEdge (Keybinding: Keys.Ctrl | Keys.Up)
		public bool MoveUpToDataEdge() {
			return SetSelection(CalculateDataEdgePositionAbove(activeCell));
		}
		CellPosition CalculateDataEdgePositionAbove(CellPosition position) {
			IOrderedEnumerator<ICell> enumerator = ColumnCollection.GetExistingVisibleNonEmptyCellsEnumerator(sheet, position.Column, 0, Math.Max(0, position.Row - 1), true);
			if (!enumerator.MoveNext())
				return CalculateUpperMostVisiblePosition(position);
			CellPosition nextPosition = CalculateDataEdgePositionCore(position, CalculateVisiblePositionAbove);
			if (!nextPosition.EqualsPosition(position))
				return nextPosition;
			ICell cell = enumerator.Current;
			return new CellPosition(position.Column, cell.RowIndex);
		}
		CellPosition CalculateUpperMostVisiblePosition(CellPosition position) {
			if (sheet.Rows.Count <= 0)
				return new CellPosition(position.Column, 0);
			int rowIndex = 0;
			IOrderedEnumerator<Row> enumerator = sheet.Rows.GetExistingRowsEnumerator(0, Math.Max(0, position.Row - 1), false);
			for (; ; ) {
				if (!enumerator.MoveNext())
					break;
				Row row = enumerator.Current;
				if (row.Index > rowIndex) 
					break;
				if (!row.IsVisible) {
					rowIndex = row.Index + 1;
				}
			}
			return new CellPosition(position.Column, Math.Min(sheet.MaxRowCount - 1, rowIndex));
		}
		#endregion
		#region MoveDownToDataEdge (Keybinding: Keys.Ctrl | Keys.Down)
		public bool MoveDownToDataEdge() {
			return SetSelection(CalculateDataEdgePositionBelow(activeCell));
		}
		CellPosition CalculateDataEdgePositionBelow(CellPosition position) {
			IOrderedEnumerator<ICell> enumerator = ColumnCollection.GetExistingVisibleNonEmptyCellsEnumerator(sheet, position.Column, Math.Min(sheet.MaxRowCount - 1, position.Row + 1), sheet.MaxRowCount - 1, false);
			if (!enumerator.MoveNext())
				return CalculateBottomMostVisiblePosition(position);
			CellPosition nextPosition = CalculateDataEdgePositionCore(position, CalculateVisiblePositionBelow);
			if (!nextPosition.EqualsPosition(position))
				return nextPosition;
			ICell cell = enumerator.Current;
			return new CellPosition(position.Column, cell.RowIndex);
		}
		CellPosition CalculateBottomMostVisiblePosition(CellPosition position) {
			if (sheet.Rows.Count <= 0)
				return new CellPosition(position.Column, sheet.MaxRowCount - 1);
			int rowIndex = sheet.MaxRowCount - 1;
			IOrderedEnumerator<Row> enumerator = sheet.Rows.GetExistingRowsEnumerator(Math.Min(sheet.MaxRowCount - 1, position.Row + 1), sheet.MaxRowCount - 1, true);
			for (; ; ) {
				if (!enumerator.MoveNext())
					break;
				Row row = enumerator.Current;
				if (row.Index < rowIndex) 
					break;
				if (!row.IsVisible) {
					rowIndex = row.Index - 1;
				}
			}
			return new CellPosition(position.Column, Math.Max(0, rowIndex));
		}
		#endregion
		public bool IsSingleMergedCell() {
			if (SelectedRanges.Count != 1)
				return false;
			CellRange actualRange = GetActualCellRange(activeCell);
			return actualRange.ContainsRange(SelectedRanges[0]);
		}
		public bool IsMultiMergedCell() {
			if (SelectedRanges.Count <= 1)
				return false;
			CellRange actualRange = GetActualCellRange(activeCell);
			foreach (CellRange currentRange in SelectedRanges)
				if (actualRange.ContainsRange(currentRange))
					return true;
			return false;
		}
		public void SetActiveCellSmart(CellPosition position) {
			if (ActiveCell.Equals(position))
				return;
			int count = SelectedRanges.Count;
			for (int i = 0; i < count; i++) {
				if (SelectedRanges[i].ContainsCell(position.Column, position.Row)) {
					this.activeCell = position;
					this.activeRangeIndex = i;
					OnSelectionChanged();
					return;
				}
			}
			SetSelection(position);
		}
		internal void SetActiveCellCore(CellPosition position) {
			if (ActiveCell.Equals(position))
				return;
			this.activeCell = position;
			OnSelectionChanged();
		}
		#region MoveActiveCellRight (Keybinding: Keys.Tab)
		public bool MoveActiveCellRight() {
			if (IsSingleCell || IsSingleMergedCell())
				return MoveRight();
			CellRange activeRange = ActiveRange;
			if (activeCell.Column < activeRange.BottomRight.Column) {
				activeCell = CalculateVisiblePositionAtRight(activeCell);
				OnSelectionChanged();
				return true;
			}
			if (activeCell.Row < activeRange.BottomRight.Row) {
				activeCell = new CellPosition(activeRange.TopLeft.Column, activeCell.Row);
				activeCell = CalculateVisiblePositionBelow(activeCell);
				OnSelectionChanged();
				return true;
			}
			ChangeActiveRangeCore(1);
			this.activeCell = ActiveRange.TopLeft;
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region MoveActiveCellLeft (Keybinding: Keys.Shift | Keys.Tab)
		public bool MoveActiveCellLeft() {
			if (IsSingleCell || IsSingleMergedCell())
				return MoveLeft();
			CellRange activeRange = ActiveRange;
			if (activeCell.Column > activeRange.TopLeft.Column) {
				activeCell = CalculateVisiblePositionAtLeft(activeCell);
				OnSelectionChanged();
				return true;
			}
			if (activeCell.Row > activeRange.TopLeft.Row) {
				activeCell = new CellPosition(activeRange.BottomRight.Column, activeCell.Row);
				activeCell = CalculateVisiblePositionAbove(activeCell);
				OnSelectionChanged();
				return true;
			}
			ChangeActiveRangeCore(-1);
			this.activeCell = ActiveRange.BottomRight;
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region MoveActiveCellDown (Keybinding: Keys.Enter)
		public bool MoveActiveCellDown() {
			if (IsSingleCell || IsSingleMergedCell())
				return MoveDown();
			CellRange activeRange = ActiveRange;
			if (activeCell.Row < activeRange.BottomRight.Row) {
				activeCell = CalculateVisiblePositionBelow(activeCell);
				OnSelectionChanged();
				return true;
			}
			if (activeCell.Column < activeRange.BottomRight.Column) {
				activeCell = new CellPosition(activeCell.Column, activeRange.TopLeft.Row);
				activeCell = CalculateVisiblePositionAtRight(activeCell);
				OnSelectionChanged();
				return true;
			}
			ChangeActiveRangeCore(1);
			this.activeCell = ActiveRange.TopLeft;
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region MoveActiveCellUp (Keybinding: Keys.Shift | Keys.Enter)
		public bool MoveActiveCellUp() {
			if (IsSingleCell || IsSingleMergedCell())
				return MoveUp();
			CellRange activeRange = ActiveRange;
			if (activeCell.Row > activeRange.TopLeft.Row) {
				activeCell = CalculateVisiblePositionAbove(activeCell);
				OnSelectionChanged();
				return true;
			}
			if (activeCell.Column > activeRange.TopLeft.Column) {
				activeCell = new CellPosition(activeCell.Column, activeRange.BottomRight.Row);
				activeCell = CalculateVisiblePositionAtLeft(activeCell);
				OnSelectionChanged();
				return true;
			}
			ChangeActiveRangeCore(-1);
			this.activeCell = ActiveRange.BottomRight;
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region MoveActiveCellToNextCorner (Keybinding: Keys.Ctrl | Keys.Period)
		public bool MoveActiveCellToNextCorner() {
			CellRange range = ActiveRange;
			if (range.CellCount <= 1)
				return false;
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			if (range.Width == 1 || range.Height == 1)
				this.activeCell = CalculateNextCell(activeCell, new CellPosition[] { topLeft, bottomRight });
			else
				this.activeCell = CalculateNextCell(activeCell, new CellPosition[] { topLeft, new CellPosition(bottomRight.Column, topLeft.Row), bottomRight, new CellPosition(topLeft.Column, bottomRight.Row) });
			OnSelectionChanged();
			return true;
		}
		CellPosition CalculateNextCell(CellPosition position, CellPosition[] positions) {
			int count = positions.Length;
			if (count <= 0)
				return position;
			int index = -1;
			for (int i = 0; i < count; i++) {
				if (positions[i].EqualsPosition(position)) {
					index = i;
					break;
				}
			}
			if (index < 0)
				return positions[0];
			return positions[(index + 1) % positions.Length];
		}
		#endregion
		#region SelectNextActiveRange (Keybinding: Keys.Ctrl | Keys.Alt | Keys.Right)
		public bool SelectNextActiveRange() {
			return ChangeActiveRange(1);
		}
		#endregion
		#region SelectPreviousActiveRange (Keybinding: Keys.Ctrl | Keys.Alt | Keys.Left)
		public bool SelectPreviousActiveRange() {
			return ChangeActiveRange(-1);
		}
		bool ChangeActiveRange(int offset) {
			if (ChangeActiveRangeCore(offset)) {
				OnSelectionChanged();
				return true;
			}
			else
				return false;
		}
		bool ChangeActiveRangeCore(int offset) {
			int count = SelectedRanges.Count;
			if (count == 1 && ActiveCell.EqualsPosition(ActiveRange.TopLeft))
				return false;
			this.activeRangeIndex += offset;
			if (activeRangeIndex < 0)
				activeRangeIndex += count;
			activeRangeIndex %= count;
			this.activeCell = ActiveRange.TopLeft;
			return true;
		}
		#endregion
		#region SelectActiveCell (Keybinding: Keys.Shift | Keys.Backspace)
		public bool SelectActiveCell() {
			return SetSelection(activeCell);
		}
		#endregion
		#region SelectEntireColumns (Keybinding: Keys.Ctrl | Keys.Space)
		public bool SelectEntireColumns() {
			CellRange activeRange = ActiveRange;
			CellRange newRange = CellIntervalRange.CreateColumnInterval(sheet, activeRange.TopLeft.Column, PositionType.Relative, activeRange.BottomRight.Column, PositionType.Relative);
			SelectedRanges[ActiveRangeIndex] = ExpandRangeToMergedCellSizeSingle(newRange);
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region SelectEntireRows (Keybinding: Keys.Shift | Keys.Space)
		public bool SelectEntireRows() {
			CellRange activeRange = ActiveRange;
			CellRange newRange = CellIntervalRange.CreateRowInterval(sheet, activeRange.TopLeft.Row, PositionType.Relative, activeRange.BottomRight.Row, PositionType.Relative);
			SelectedRanges[ActiveRangeIndex] = ExpandRangeToMergedCellSizeSingle(newRange);
			OnSelectionChanged();
			return true;
		}
		#endregion
		#region SelectAll (Keybinding: Keys.Ctrl | Keys.A or Keys.Ctrl | Keys.Shift | Keys.Space)
		public bool SelectAll() {
			return SelectAll(activeCell, false);
		}
		public bool SelectAll(CellPosition newActiveCell, bool haveToSelectAll) {
			SetSelectionCore(newActiveCell, GetSelectAllRange(haveToSelectAll), false);
			OnSelectionChanged();
			return true;
		}
		internal CellRangeBase GetSelectAllRange(bool haveToSelectAll) {
			CellIntervalRange allColumnsInterval = CellIntervalRange.CreateColumnInterval(sheet,
													   0,
													   PositionType.Relative,
													   sheet.MaxColumnCount - 1,
													   PositionType.Relative);
			if (haveToSelectAll)
				return allColumnsInterval;
			if (this.AsRange().RangeType == CellRangeType.UnionRange) {
				selectedRanges[activeRangeIndex] = allColumnsInterval;
				return this.AsRange();
			}
			CellRange selection = selectedRanges[0];
			if (selection.Equals(allColumnsInterval))
				return allColumnsInterval;
			Table table = sheet.GetTableByCellPosition(activeCell.Column, activeCell.Row);
			CellRange range = table == null ? GetResultRange(selection, allColumnsInterval) : GetResultRangeForTable(table, selection, allColumnsInterval);
			if (range.BottomRight.Column >= selection.BottomRight.Column && range.BottomRight.Row >= selection.BottomRight.Row)
				return range;
			return allColumnsInterval;
		}
		internal CellRange GetResultRange(CellRange selection, CellRange allCells) {
			CellRange activeCellRange = new CellRange(sheet, activeCell, activeCell);
			CellRange range = sheet.GetCurrentRegion(activeCellRange);
			bool tablesExists = sheet.Tables.ContainsItemsInRange(range, true);
			if (tablesExists)
				return allCells;
			ICellBase activeCellValue = sheet.TryGetCell(activeCell.Column, activeCell.Row);
			if (range.Equals(selection) || range.Equals(activeCellRange) && (activeCellValue == null || activeCellValue.Value.IsEmpty))
				return allCells;
			if (activeCell.Row == range.BottomRight.Row)
				range = DeleteEmptyBotRow(range);
			if (activeCell.Column == range.BottomRight.Column)
				range = DeleteEmptyRightColumn(range);
			return range;
		}
		internal CellRange GetResultRangeForTable(Table table, CellRange selection, CellRange allCells) {
			if (selection.Equals(table.Range))
				return allCells;
			CellRange dataRange = table.GetDataRange();
			if (dataRange.ContainsRange(selection) && !dataRange.Equals(selection))
				return dataRange;
			return table.Range;
		}
		internal CellRange DeleteEmptyBotRow(CellRange selection) {
			int botRow = selection.BottomRight.Row;
			bool isEmpty = true;
			for (int i = selection.TopLeft.Column; i <= selection.BottomRight.Column; ++i) {
				ICellBase cell = sheet.TryGetCell(i, botRow);
				if (cell != null && !cell.Value.IsEmpty) {
					isEmpty = false;
					break;
				}
			}
			if (isEmpty) {
				CellRange lastRow = new CellRange(sheet, new CellPosition(selection.TopLeft.Column, selection.BottomRight.Row), selection.BottomRight);
				List<CellRange> mergedRangeIntersectsLastRow = sheet.MergedCells.GetMergedCellRangesIntersectsRange(lastRow);
				foreach (CellRange mergedRange in mergedRangeIntersectsLastRow) {
					ICellBase cell = mergedRange.TryGetCellRelative(0, 0);
					if (cell != null && !cell.Value.IsEmpty && !mergedRange.IntersectionWith(lastRow).IsError) {
						isEmpty = false;
						break;
					}
				}
			}
			if (isEmpty)
				return selection.GetResized(0, 0, 0, -1);
			return selection;
		}
		internal CellRange DeleteEmptyRightColumn(CellRange selection) {
			int rightColumn = selection.BottomRight.Column;
			bool isEmpty = true;
			for (int i = selection.TopLeft.Row; i <= selection.BottomRight.Row; ++i) {
				ICellBase cell = sheet.TryGetCell(rightColumn, i);
				if (cell != null && !cell.Value.IsEmpty) {
					isEmpty = false;
					break;
				}
			}
			if (isEmpty) {
				CellRange lastColumn = new CellRange(sheet, new CellPosition(selection.BottomRight.Column, selection.TopLeft.Row), selection.BottomRight);
				List<CellRange> mergedRangeIntersectsLastColumn = sheet.MergedCells.GetMergedCellRangesIntersectsRange(lastColumn);
				foreach (CellRange mergedRange in mergedRangeIntersectsLastColumn){
					ICellBase cell = mergedRange.TryGetCellRelative(0, 0);
					if (cell != null && !cell.Value.IsEmpty && !mergedRange.IntersectionWith(lastColumn).IsError) {
						isEmpty = false;
						break;
					}
				}
			}
			if (isEmpty)
				return selection.GetResized(0, 0, -1, 0);
			return selection;
		}
		public bool IsAllSelected {
			get {
				CellRangeBase range = this.AsRange();
				if (range.RangeType != CellRangeType.UnionRange)
					return CheckAllSelected(range);
				List<CellRangeBase> innerRanges = ((CellUnion)range).InnerCellRanges;
				foreach (CellRange innerRange in innerRanges) {
					if (CheckAllSelected(innerRange))
						return true;
				}
				return false;
			}
		}
		#endregion
		bool CheckAllSelected(CellRangeBase range) {
			if (range.TopLeft.Column != 0 || range.TopLeft.Row != 0)
				return false;
			if (range.BottomRight.Column != sheet.MaxColumnCount - 1 || range.BottomRight.Row != sheet.MaxRowCount - 1)
				return false;
			return true;
		}
		public CellRange GetActiveCellRange() {
			return GetActualCellRange(ActiveCell);
		}
		public CellRange GetActualCellRange(CellPosition position) {
			CellRange range = sheet.MergedCells.FindMergedCell(position.Column, position.Row);
			if (range != null)
				return range;
			else
				return new CellRange(sheet, position, position);
		}
		public Table TryGetActiveTable() {
			return sheet.Tables.TryGetItem(ActiveCell);
		}
		public PivotTable TryGetActivePivotTable() {
			return sheet.PivotTables.TryGetItem(ActiveCell);
		}
		public PivotZone TryGetActivePivotZone() {
			PivotTable activePivotTable = TryGetActivePivotTable();
			if (activePivotTable != null)
				return activePivotTable.CalculationInfo.GetPivotZoneByCellPosition(ActiveCell);
			return null;
		}
		public IList<Table> GetActiveTables() {
			return sheet.Tables.GetItems(ActiveRange, true);
		}
		public bool ContainsPivotTableInActiveRange() {
			return sheet.PivotTables.ContainsItemsInRange(ActiveRange, true);
		}
		public void ExpandToMergedCells() {
			int count = selectedRanges.Count;
			for (int i = 0; i < count; i++)
				selectedRanges[i] = ExpandRangeToMergedCellSizeSingle(selectedRanges[i]);
		}
		public static CellRangeBase ExpandRangeToMergedCellSize(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				List<CellRangeBase> expandedRanges = new List<CellRangeBase>();
				foreach (CellRangeBase innerCellRange in union.InnerCellRanges)
					expandedRanges.Add(ExpandRangeToMergedCellSize(innerCellRange));
				CellUnion result = new CellUnion(expandedRanges);
				if (range.Worksheet != null)
					result.Worksheet = range.Worksheet;
				return result;
			}
			else
				return ExpandRangeToMergedCellSizeSingle((CellRange)range);
		}
		public static CellRange ExpandRangeToMergedCellSizeSingle(CellRange range) {
			for (; ; ) {
				CellRange expandedRange = ExpandRangeToMergedCellSizeCore(range);
				if (Object.ReferenceEquals(expandedRange, range))
					return range;
				if (expandedRange.EqualsPosition(range))
					return range;
				range = expandedRange;
			}
		}
		static CellRange ExpandRangeToMergedCellSizeCore(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet == null)
				return range;
			List<CellRange> mergedCells = sheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
			if (mergedCells.Count <= 0)
				return range;
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			int count = mergedCells.Count;
			for (int i = 0; i < count; i++) {
				CellRange mergedCell = mergedCells[i];
				CellPosition mergedCellTopLeft = mergedCell.TopLeft;
				CellPosition mergedCellBottomRight = mergedCell.BottomRight;
				topLeft = new CellPosition(Math.Min(mergedCellTopLeft.Column, topLeft.Column), Math.Min(mergedCellTopLeft.Row, topLeft.Row));
				bottomRight = new CellPosition(Math.Max(mergedCellBottomRight.Column, bottomRight.Column), Math.Max(mergedCellBottomRight.Row, bottomRight.Row));
			}
			return range.Expand(new CellRange(sheet, topLeft, bottomRight));
		}
		public static CellRange ContractRangeToMergedCellSizeSingle(CellRange range, ShrinkRange shrinkRange) {
			for (; ; ) {
				CellRange narrowedRange = ContractRangeToMergedCellSizeCore(range, shrinkRange);
				if (Object.ReferenceEquals(narrowedRange, range))
					return range;
				if (narrowedRange.EqualsPosition(range))
					return range;
				range = narrowedRange;
			}
		}
		static CellRange ContractRangeToMergedCellSizeCore(CellRange range, ShrinkRange shrinkRange) {
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet == null)
				return range;
			List<CellRange> mergedCells = sheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
			for (int i = 0; i < mergedCells.Count; i++)
				if (range.ContainsRange(mergedCells[i]))
					mergedCells.Remove(mergedCells[i--]);
			if (mergedCells.Count <= 0)
				return range;
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			int count = mergedCells.Count;
			for (int i = 0; i < count; i++) {
				CellRange mergedCell = mergedCells[i];
				CellPosition mergedCellTopLeft = mergedCell.TopLeft;
				CellPosition mergedCellBottomRight = mergedCell.BottomRight;
				if (shrinkRange == ShrinkRange.Left) {
					topLeft = new CellPosition(Math.Min(mergedCellTopLeft.Column, topLeft.Column), Math.Min(mergedCellTopLeft.Row, topLeft.Row));
					bottomRight = new CellPosition(Math.Min(mergedCellBottomRight.Column, bottomRight.Column), Math.Max(mergedCellBottomRight.Row, bottomRight.Row));
				}
				else if (shrinkRange == ShrinkRange.Right) {
					topLeft = new CellPosition(Math.Max(mergedCellTopLeft.Column, topLeft.Column), Math.Min(mergedCellTopLeft.Row, topLeft.Row));
					bottomRight = new CellPosition(Math.Max(mergedCellBottomRight.Column, bottomRight.Column), Math.Max(mergedCellBottomRight.Row, bottomRight.Row));
				}
				else if (shrinkRange == ShrinkRange.Up) {
					topLeft = new CellPosition(Math.Min(mergedCellTopLeft.Column, topLeft.Column), Math.Min(mergedCellTopLeft.Row, topLeft.Row));
					bottomRight = new CellPosition(Math.Max(mergedCellBottomRight.Column, bottomRight.Column), Math.Min(mergedCellBottomRight.Row, bottomRight.Row));
				}
				else if (shrinkRange == ShrinkRange.Down) {
					topLeft = new CellPosition(Math.Min(mergedCellTopLeft.Column, topLeft.Column), Math.Max(mergedCellTopLeft.Row, topLeft.Row));
					bottomRight = new CellPosition(Math.Max(mergedCellBottomRight.Column, bottomRight.Column), Math.Max(mergedCellBottomRight.Row, bottomRight.Row));
				}
			}
			return range.ExcludeRange(new CellRange(sheet, topLeft, bottomRight) as CellRangeBase) as CellRange;
		}
		#region ExtendLeft
		public bool ExtendLeft() {
			CellRange range = new CellRange(sheet, CalculateVisiblePositionAtLeft(ActiveRange.TopLeft), ActiveRange.BottomRight);
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ShrinkLeft
		public bool ShrinkLeft() {
			CellPosition nextPosition = CalculateVisiblePositionAtRight(ActiveRange.TopLeft);
			if (nextPosition.Column > ActiveRange.BottomRight.Column)
				return false;
			CellRange range = new CellRange(sheet, nextPosition, ActiveRange.BottomRight);
			range = ContractRangeToMergedCellSizeSingle(range, ShrinkRange.Left);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ExpandLeft (Keybinding: Keys.Shift | Keys.Left)
		public bool ExpandLeft() {
			if (GetActualCellRange(activeCell).BottomRight.Column != ActiveRange.TopLeft.Column || ActiveRange.TopLeft.Column == ActiveRange.BottomRight.Column)
				return ExtendLeft();
			else
				return ShrinkRight();
		}
		#endregion
		#region ExtendRight
		public bool ExtendRight() {
			CellRange range = new CellRange(sheet, ActiveRange.TopLeft, CalculateVisiblePositionAtRight(ActiveRange.BottomRight));
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ShrinkRight
		public bool ShrinkRight() {
			CellPosition nextPosition = CalculateVisiblePositionAtLeft(ActiveRange.BottomRight);
			if (nextPosition.Column < ActiveRange.TopLeft.Column)
				return false;
			CellRange range = new CellRange(sheet, ActiveRange.TopLeft, nextPosition);
			range = ContractRangeToMergedCellSizeSingle(range, ShrinkRange.Right);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ExpandRight (Keybinding: Keys.Shift | Keys.Right)
		public bool ExpandRight() {
			if (GetActualCellRange(activeCell).TopLeft.Column != ActiveRange.BottomRight.Column || ActiveRange.TopLeft.Column == ActiveRange.BottomRight.Column)
				return ExtendRight();
			else
				return ShrinkLeft();
		}
		#endregion
		#region ExtendUp
		public bool ExtendUp() {
			CellRange range = new CellRange(sheet, CalculateVisiblePositionAbove(ActiveRange.TopLeft), ActiveRange.BottomRight);
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ShrinkUp
		public bool ShrinkUp() {
			CellPosition position = CalculateVisiblePositionBelow(ActiveRange.TopLeft);
			if (position.Row > ActiveRange.BottomRight.Row)
				return false;
			CellRange range = new CellRange(sheet, position, ActiveRange.BottomRight);
			range = ContractRangeToMergedCellSizeSingle(range, ShrinkRange.Up);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ExpandUp (Keybinding: Keys.Shift | Keys.Up)
		public bool ExpandUp() {
			if (GetActualCellRange(activeCell).BottomRight.Row != ActiveRange.TopLeft.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				return ExtendUp();
			else
				return ShrinkDown();
		}
		#endregion
		public bool ExpandActiveRangeUp(CellPosition position) {
			CellRange range;
			if (GetActualCellRange(activeCell).BottomRight.Row != ActiveRange.TopLeft.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				range = new CellRange(sheet, new CellPosition(Math.Min(position.Column, ActiveRange.TopLeft.Column), position.Row), ActiveRange.BottomRight);
			else
				range = new CellRange(sheet, ActiveRange.TopLeft, new CellPosition(Math.Max(position.Column, ActiveRange.BottomRight.Column), position.Row));
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#region ExtendDown
		public bool ExtendDown() {
			CellRange range = new CellRange(sheet, ActiveRange.TopLeft, CalculateVisiblePositionBelow(ActiveRange.BottomRight));
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ShrinkDown
		public bool ShrinkDown() {
			CellPosition position = CalculateVisiblePositionAbove(ActiveRange.BottomRight);
			if (position.Row < ActiveRange.TopLeft.Row)
				return false;
			CellRange range = new CellRange(sheet, ActiveRange.TopLeft, position);
			range = ContractRangeToMergedCellSizeSingle(range, ShrinkRange.Down);
			return ReplaceActiveRange(range);
		}
		#endregion
		#region ExpandDown (Keybinding: Keys.Shift | Keys.Down)
		public bool ExpandDown() {
			if (GetActualCellRange(activeCell).TopLeft.Row != ActiveRange.BottomRight.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				return ExtendDown();
			else
				return ShrinkUp();
		}
		#endregion
		public bool ExpandActiveRangeDown(CellPosition position) {
			CellRange range;
			if (GetActualCellRange(activeCell).TopLeft.Row != ActiveRange.BottomRight.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				range = new CellRange(sheet, ActiveRange.TopLeft, new CellPosition(Math.Max(position.Column, ActiveRange.BottomRight.Column), position.Row));
			else
				range = new CellRange(sheet, new CellPosition(Math.Min(position.Column, ActiveRange.TopLeft.Column), position.Row), ActiveRange.BottomRight);
			range = ExpandRangeToMergedCellSizeSingle(range);
			return ReplaceActiveRange(range);
		}
		#region ExpandToLeftColumn (Keybinding: Keys.Shift | Keys.Left)
		public bool ExpandToLeftColumn() {
			CellPosition newPosition = CalculateLeftMostVisiblePosition(activeCell);
			newPosition = new CellPosition(newPosition.Column, Math.Min(ActiveRange.TopLeft.Row, newPosition.Row));
			if (activeCell.Column == ActiveRange.TopLeft.Column) {
				CellPosition newBottomRight = new CellPosition(ActiveRange.TopLeft.Column, ActiveRange.BottomRight.Row);
				return ReplaceActiveRange(new CellRange(sheet, newPosition, newBottomRight));
			}
			else
				return ReplaceActiveRange(new CellRange(sheet, newPosition, ActiveRange.BottomRight));
		}
		#endregion
		#region ExpandToTopLeftCell (Keybinding: Keys.Shift | Keys.Control | Keys.Home)
		public bool ExpandToTopLeftCell() {
			return ReplaceActiveRange(new CellRange(sheet, CalculateTopLeftCellPosition(activeCell), ActiveRange.BottomRight));
		}
		#endregion
		#region ExpandToLastUsedCell (Keybinding: Keys.Shift | Keys.Control | Keys.End)
		public bool ExpandToLastUsedCell() {
			CellPosition lastUsedCellPosition = CalculateLastUsedCellPosition();
			int topLeftColumn = Math.Min(lastUsedCellPosition.Column, Math.Min(activeCell.Column, ActiveRange.TopLeft.Column));
			int topLeftRow = Math.Min(lastUsedCellPosition.Row, Math.Min(activeCell.Row, ActiveRange.TopLeft.Row));
			int bottomRightColumn = Math.Max(lastUsedCellPosition.Column, activeCell.Column);
			int bottomRightRow = Math.Max(lastUsedCellPosition.Row, activeCell.Row);
			return ReplaceActiveRange(new CellRange(sheet, new CellPosition(topLeftColumn, topLeftRow), new CellPosition(bottomRightColumn, bottomRightRow)));
		}
		#endregion
		#region ExtendLeftToDataEdge
		public bool ExtendLeftToDataEdge() {
			CellPosition position = new CellPosition(Math.Min(activeCell.Column, ActiveRange.TopLeft.Column), activeCell.Row);
			CellPosition newPosition = CalculateDataEdgePositionAtLeft(position);
			newPosition = new CellPosition(newPosition.Column, Math.Min(ActiveRange.TopLeft.Row, newPosition.Row));
			return ReplaceActiveRange(new CellRange(sheet, newPosition, ActiveRange.BottomRight));
		}
		#endregion
		#region ShrinkLeftToDataEdge
		public bool ShrinkLeftToDataEdge() {
			return ShrinkLeftToDataEdgeCore(false);
		}
		bool ShrinkLeftToDataEdgeCore(bool allowInvertSelection) {
			CellPosition position = new CellPosition(Math.Min(activeCell.Column, ActiveRange.TopLeft.Column), activeCell.Row);
			CellPosition nextPosition = CalculateDataEdgePositionAtRight(position);
			nextPosition = new CellPosition(nextPosition.Column, Math.Min(ActiveRange.TopLeft.Row, nextPosition.Row));
			if (!allowInvertSelection && nextPosition.Column > ActiveRange.BottomRight.Column)
				nextPosition = new CellPosition(ActiveRange.BottomRight.Column, nextPosition.Row);
			return ReplaceActiveRange(new CellRange(sheet, nextPosition, ActiveRange.BottomRight));
		}
		#endregion
		#region ExpandLeftToDataEdge (Keybinding: Keys.Shift | Keys.Control | Keys.Left)
		public bool ExpandLeftToDataEdge() {
			if (activeCell.Column != ActiveRange.TopLeft.Column || ActiveRange.TopLeft.Column == ActiveRange.BottomRight.Column)
				return ExtendLeftToDataEdge();
			else
				return ShrinkRightToDataEdgeCore(true);
		}
		#endregion
		#region ExtendRightToDataEdge
		public bool ExtendRightToDataEdge() {
			CellPosition position = new CellPosition(Math.Max(activeCell.Column, ActiveRange.BottomRight.Column), activeCell.Row);
			CellPosition newPosition = CalculateDataEdgePositionAtRight(position);
			newPosition = new CellPosition(newPosition.Column, Math.Max(ActiveRange.BottomRight.Row, newPosition.Row));
			return ReplaceActiveRange(new CellRange(sheet, ActiveRange.TopLeft, newPosition));
		}
		#endregion
		#region ShrinkRightToDataEdge
		public bool ShrinkRightToDataEdge() {
			return ShrinkRightToDataEdgeCore(false);
		}
		bool ShrinkRightToDataEdgeCore(bool allowInvertSelection) {
			CellPosition position = new CellPosition(Math.Max(activeCell.Column, ActiveRange.BottomRight.Column), activeCell.Row);
			CellPosition nextPosition = CalculateDataEdgePositionAtLeft(position);
			nextPosition = new CellPosition(nextPosition.Column, Math.Max(ActiveRange.BottomRight.Row, nextPosition.Row));
			if (!allowInvertSelection && nextPosition.Column < ActiveRange.TopLeft.Column)
				nextPosition = new CellPosition(ActiveRange.TopLeft.Column, nextPosition.Row);
			return ReplaceActiveRange(new CellRange(sheet, ActiveRange.TopLeft, nextPosition));
		}
		#endregion
		#region ExpandRightToDataEdge (Keybinding: Keys.Shift | Keys.Control | Keys.Right)
		public bool ExpandRightToDataEdge() {
			if (activeCell.Column != ActiveRange.BottomRight.Column || ActiveRange.TopLeft.Column == ActiveRange.BottomRight.Column)
				return ExtendRightToDataEdge();
			else
				return ShrinkLeftToDataEdgeCore(true);
		}
		#endregion
		#region ExtendUpToDataEdge
		public bool ExtendUpToDataEdge() {
			CellPosition position = new CellPosition(activeCell.Column, Math.Min(activeCell.Row, ActiveRange.TopLeft.Row));
			CellPosition newPosition = CalculateDataEdgePositionAbove(position);
			newPosition = new CellPosition(Math.Min(ActiveRange.TopLeft.Column, newPosition.Column), newPosition.Row);
			return ReplaceActiveRange(new CellRange(sheet, newPosition, ActiveRange.BottomRight));
		}
		#endregion
		#region ShrinkUpToDataEdge
		public bool ShrinkUpToDataEdge() {
			return ShrinkUpToDataEdgeCore(false);
		}
		bool ShrinkUpToDataEdgeCore(bool allowInvertSelection) {
			CellPosition position = new CellPosition(activeCell.Column, Math.Min(activeCell.Row, ActiveRange.TopLeft.Row));
			CellPosition nextPosition = CalculateDataEdgePositionBelow(position);
			nextPosition = new CellPosition(Math.Min(ActiveRange.TopLeft.Column, nextPosition.Column), nextPosition.Row);
			if (!allowInvertSelection && nextPosition.Row > ActiveRange.BottomRight.Row)
				nextPosition = new CellPosition(nextPosition.Column, ActiveRange.BottomRight.Row);
			return ReplaceActiveRange(new CellRange(sheet, nextPosition, ActiveRange.BottomRight));
		}
		#endregion
		#region ExpandUpToDataEdge
		public bool ExpandUpToDataEdge() {
			if (activeCell.Row != ActiveRange.TopLeft.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				return ExtendUpToDataEdge();
			else
				return ShrinkDownToDataEdgeCore(true);
		}
		#endregion
		#region ExtendDownToDataEdge
		public bool ExtendDownToDataEdge() {
			CellPosition position = new CellPosition(activeCell.Column, Math.Max(activeCell.Row, ActiveRange.BottomRight.Row));
			CellPosition newPosition = CalculateDataEdgePositionBelow(position);
			newPosition = new CellPosition(Math.Max(ActiveRange.BottomRight.Column, newPosition.Column), newPosition.Row);
			return ReplaceActiveRange(new CellRange(sheet, ActiveRange.TopLeft, newPosition));
		}
		#endregion
		#region ShrinkDownToDataEdge
		public bool ShrinkDownToDataEdge() {
			return ShrinkDownToDataEdgeCore(false);
		}
		bool ShrinkDownToDataEdgeCore(bool allowInvertSelection) {
			CellPosition position = new CellPosition(activeCell.Column, Math.Max(activeCell.Row, ActiveRange.BottomRight.Row));
			CellPosition nextPosition = CalculateDataEdgePositionAbove(position);
			nextPosition = new CellPosition(Math.Max(ActiveRange.BottomRight.Column, nextPosition.Column), nextPosition.Row);
			if (!allowInvertSelection && nextPosition.Row < ActiveRange.TopLeft.Row)
				nextPosition = new CellPosition(nextPosition.Column, ActiveRange.TopLeft.Row);
			return ReplaceActiveRange(new CellRange(sheet, ActiveRange.TopLeft, nextPosition));
		}
		#endregion
		#region ExpandDownToDataEdge
		public bool ExpandDownToDataEdge() {
			if (activeCell.Row != ActiveRange.BottomRight.Row || ActiveRange.TopLeft.Row == ActiveRange.BottomRight.Row)
				return ExtendDownToDataEdge();
			else
				return ShrinkUpToDataEdgeCore(true);
		}
		#endregion
	}
}
