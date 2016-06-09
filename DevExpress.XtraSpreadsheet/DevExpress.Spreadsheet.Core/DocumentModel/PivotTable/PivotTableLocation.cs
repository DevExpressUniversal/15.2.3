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

using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils.Trees;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IPivotTableLocation
	public interface IPivotTableLocation {
		CellRangeBase WholeRange { get; }
		int FirstDataColumn { get; }
		int FirstDataRow { get; }
		PivotTableLocationCache Cache { get; }
	}
	#endregion
	#region PivotTableLocation
	public class PivotTableLocation : IPivotTableLocation, ISpreadsheetRangeObject {
		#region Static members
		public static CellRange GetDefaultCellRange(CellRange location) {
			CellPosition newTopLeft = location.TopLeft;
			CellPosition newBottomRight = new CellPosition(newTopLeft.Column + 2, newTopLeft.Row + 17); 
			return new CellRange(location.Worksheet, newTopLeft, newBottomRight);
		}
		#endregion
		#region Fields
		internal const int PageFieldRangeWidth = 2;
		readonly PivotTableLocationCache cache;
		CellRange range;
		CellRangeBase wholeRange;
		CellRangeBase pageFieldsRange;
		int firstHeaderRow;
		int firstDataRow;
		int firstDataColumn;
		int rowPageCount;
		int columnPageCount;
		#endregion
		public PivotTableLocation(CellRange range) {
			Guard.ArgumentNotNull(range, "Range");
			Guard.ArgumentNotNull(range.Worksheet, "worksheet");
			this.range = range;
			this.wholeRange = range.Clone();
			cache = new PivotTableLocationCache();
		}
		#region Properties
		DocumentModel DocumentModel { get { return (DocumentModel)range.Worksheet.Workbook; } }
		#region Range
		public CellRange Range { get { return range; } }
		#endregion
		#region WholeRange
		public CellRangeBase WholeRange { get { return wholeRange; } }
		#endregion
		#region PageFieldsRange
		public CellRangeBase PageFieldsRange { get { return pageFieldsRange; } }
		#endregion
		#region ColumnPageCount
		public int ColumnPageCount {
			get { return columnPageCount; }
			set {
				if (columnPageCount == value)
					return;
				ApplyHistory(new ActionHistoryItem<int>(DocumentModel, columnPageCount, value, SetColumnPageCountCore));
			}
		}
		void SetColumnPageCountCore(int value) {
			columnPageCount = value;
		}
		#endregion
		#region RowPageCount
		public int RowPageCount {
			get { return rowPageCount; }
			set {
				if (rowPageCount == value)
					return;
				ApplyHistory(new ActionHistoryItem<int>(DocumentModel, rowPageCount, value, SetRowPageCountCore));
			}
		}
		void SetRowPageCountCore(int value) {
			rowPageCount = value;
		}
		#endregion
		#region FirstDataColumn
		public int FirstDataColumn {
			get { return firstDataColumn; }
			set {
				if (firstDataColumn == value)
					return;
				ApplyHistory(new ActionHistoryItem<int>(DocumentModel, firstDataColumn, value, SetFirstDataColumnCore));
			}
		}
		void SetFirstDataColumnCore(int value) {
			firstDataColumn = value;
		}
		#endregion
		#region FirstDataRow
		public int FirstDataRow {
			get { return firstDataRow; }
			set {
				if (firstDataRow == value)
					return;
				ApplyHistory(new ActionHistoryItem<int>(DocumentModel, firstDataRow, value, SetFirstDataRowCore));
			}
		}
		void SetFirstDataRowCore(int value) {
			firstDataRow = value;
		}
		#endregion
		#region FirstHeaderRow
		public int FirstHeaderRow {
			get { return firstHeaderRow; }
			set {
				if (firstHeaderRow == value)
					return;
				ApplyHistory(new ActionHistoryItem<int>(DocumentModel, firstHeaderRow, value, SetFirstHeaderRowCore));
			}
		}
		void SetFirstHeaderRowCore(int value) {
			firstHeaderRow = value;
		}
		#endregion
		bool HasPageFields { get { return columnPageCount != 0 && rowPageCount != 0; } }
		PivotTableLocationEmptyCellsCache EmptyCellsCache { 
			get { 
				PivotTableLocationEmptyCellsCache emptyCellsCache = cache.EmptyCellsCache;
				if (!emptyCellsCache.IsValid)
					emptyCellsCache.Prepare(wholeRange as CellUnion);
				return emptyCellsCache;
			}
		}
		#endregion
		void ApplyHistory(HistoryItem item) {
			DocumentHistory history = ((DocumentModel)range.Worksheet.Workbook).History;
			history.Add(item);
			item.Execute();
		}
		#region Notification
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = range as CellUnion;
				IList<CellRangeBase> innerCellRanges = union.InnerCellRanges;
				foreach (CellRangeBase rangeBase in innerCellRanges) {
					IModelErrorInfo error = CanRangeInsert(rangeBase, mode);
					if (error != null)
						return error;
				}
				return null;
			}
			return CanInsertSingleRange(range as CellRange, mode);
		}
		IModelErrorInfo CanInsertSingleRange(CellRange range, InsertCellMode mode) {
			if (mode == InsertCellMode.ShiftCellsDown) {
				if (range.TopLeft.Column > wholeRange.BottomRight.Column ||
					range.BottomRight.Column < wholeRange.TopLeft.Column ||
					range.TopLeft.Row > wholeRange.BottomRight.Row ||
					range.TopLeft.Row <= wholeRange.TopLeft.Row &&
					range.TopLeft.Column <= wholeRange.TopLeft.Column && range.BottomRight.Column >= wholeRange.BottomRight.Column)
					return null;
			} else {
				if (range.TopLeft.Row > wholeRange.BottomRight.Row ||
					range.BottomRight.Row < wholeRange.TopLeft.Row ||
					range.TopLeft.Column > wholeRange.BottomRight.Column ||
					range.TopLeft.Column <= wholeRange.TopLeft.Column &&
					range.TopLeft.Row <= wholeRange.TopLeft.Row && range.BottomRight.Row >= wholeRange.BottomRight.Row)
					return null;
			}
			return CanModifyEmptyCellsRange(range, true, ModelErrorType.PivotTableCanNotBeChanged, mode, PivotTableLocationEmptyCellsNotificationHelper.CheckPermittedInsertCellMode);
		}
		IModelErrorInfo CanModifyEmptyCellsRange<T>(CellRange range, bool shift, ModelErrorType errorType, T mode, Func<PivotTableLocationEmptyCellsType, T, bool> checkPermittedMode) {
			if (!range.Intersects(wholeRange.GetCoveredRange())) 
				return shift ? new ModelErrorInfo(errorType) : null; 
			List<PivotTableLocationEmptyCellsInfo> emptyCells = EmptyCellsCache.GetIntersectedItems(range);
			if (emptyCells.Count == 0)
				return new ModelErrorInfo(errorType);
			foreach (PivotTableLocationEmptyCellsInfo info in emptyCells) {
				if (!checkPermittedMode(info.EmptyCellsType, mode))
					return new ModelErrorInfo(errorType);
			}
			return null;
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = range as CellUnion;
				foreach (CellRange item in union.InnerCellRanges) {
					IModelErrorInfo error = CanRemoveSingleRange(item, mode);
					if (error != null)
						return error;
				}
				return null;
			}
			return CanRemoveSingleRange((CellRange)range, mode);
		}
		IModelErrorInfo CanRemoveSingleRange(CellRange range, RemoveCellMode mode) {
			if (range.Includes(wholeRange))
				return null;
			switch (mode) {
				case RemoveCellMode.Default:
					return CanModifyEmptyCellsRange(range, false, ModelErrorType.PivotTablePartCanNotBeChanged, mode, PivotTableLocationEmptyCellsNotificationHelper.CheckPermittedRemoveCellMode);
				case RemoveCellMode.NoShiftOrRangeToPasteCutRange:
					return CanModifyEmptyCellsRange(range, false, ModelErrorType.PivotTableCanNotBeChanged, mode, PivotTableLocationEmptyCellsNotificationHelper.CheckPermittedRemoveCellMode);
				case RemoveCellMode.ShiftCellsLeft:
					if (range.TopLeft.Row > wholeRange.BottomRight.Row ||
						range.BottomRight.Row < wholeRange.TopLeft.Row ||
						range.TopLeft.Column > wholeRange.BottomRight.Column ||
						range.BottomRight.Column < wholeRange.TopLeft.Column &&
						range.TopLeft.Row <= wholeRange.TopLeft.Row && range.BottomRight.Row >= wholeRange.BottomRight.Row)
						return null;
					return CanModifyEmptyCellsRange(range, true, ModelErrorType.PivotTableCanNotBeChanged, mode, PivotTableLocationEmptyCellsNotificationHelper.CheckPermittedRemoveCellMode);
				case RemoveCellMode.ShiftCellsUp:
					if (range.TopLeft.Column > wholeRange.BottomRight.Column ||
						range.BottomRight.Column < wholeRange.TopLeft.Column ||
						range.TopLeft.Row > wholeRange.BottomRight.Row ||
						range.BottomRight.Row < wholeRange.TopLeft.Row &&
						range.TopLeft.Column <= wholeRange.TopLeft.Column && range.BottomRight.Column >= wholeRange.BottomRight.Column)
						return null;
					return CanModifyEmptyCellsRange(range, true, ModelErrorType.PivotTableCanNotBeChanged, mode, PivotTableLocationEmptyCellsNotificationHelper.CheckPermittedRemoveCellMode);
				default:
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
					return null;
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			ShiftRange(context);
		}
		public bool OnRangeRemoving(RemoveRangeNotificationContext context) {
			if (context.Range.Includes(WholeRange))
				return false;
			return ShiftRange(context);
		}
		bool ShiftRange(InsertRemoveRangeNotificationContextBase context) {
			CellRange newRange = context.Visitor.ProcessCellRange(range.Clone()) as CellRange;
			if (newRange == null)
				return false;
			if (range.Equals(newRange))
				return true;
			ShiftRange(newRange);
			return true;
		}
		protected internal void ShiftRange(CellRange value) {
			ApplyHistory(new ActionHistoryItem<CellRange>(DocumentModel, range, value, ShiftRangeCore));
		}
		protected internal void ShiftRangeCore(CellRange value) {
			CellRange oldRange = range;
			range = value;
			CellPositionOffset offset = new CellPositionOffset(oldRange.TopLeft, range.TopLeft);
			CellRangeBase oldWholeRange = wholeRange;
			wholeRange = oldWholeRange.GetShiftedAny(offset, oldWholeRange.Worksheet);
			if (pageFieldsRange != null) {
				CellRangeBase oldPageFieldsRange = pageFieldsRange;
				pageFieldsRange = oldPageFieldsRange.GetShiftedAny(offset, oldPageFieldsRange.Worksheet);
			}
			RaiseRangeChanged(oldRange, range);
			RaiseWholeRangeChanged(oldWholeRange, wholeRange);
		}
		#endregion
		#region SetRange
		public void SetRange(CellRange newRange, PivotTable pivotTable) {
			CellUnion newPageFieldsRange = TryGetPageFieldsRange(newRange, columnPageCount, rowPageCount, pivotTable.PageFields.Count, pivotTable.PageOverThenDown);
			if (CellRangeBase.Equals(pageFieldsRange, newPageFieldsRange) && range.Equals(newRange))
				return;
			CellRangeBase newWholeRange = GetWholeRange(newRange, newPageFieldsRange, pivotTable);
			SetRange(newRange, newPageFieldsRange, newWholeRange);
		}
		public void SetRange(CellRange newRange, CellRangeBase newPageFieldsRange, CellRangeBase newWholeRange) {
			DocumentModel.BeginUpdate();
			try {
				ApplyHistory(new ActionHistoryItem<CellRange>(DocumentModel, range, newRange, SetRangeCore));
				ApplyHistory(new ActionHistoryItem<CellRangeBase>(DocumentModel, pageFieldsRange, newPageFieldsRange, SetPageFieldsRangeCore));
				ApplyHistory(new ActionHistoryItem<CellRangeBase>(DocumentModel, wholeRange, newWholeRange, SetWholeRangeCore));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void SetPageFieldsRangeCore(CellRangeBase value) {
			CellRangeBase oldPageFieldsRange = this.pageFieldsRange;
			this.pageFieldsRange = value;
		}
		void SetWholeRangeCore(CellRangeBase value) {
			CellRangeBase oldWholeRange = this.wholeRange;
			this.wholeRange = value;
			RaiseWholeRangeChanged(oldWholeRange, wholeRange);
		}
		internal void SetRangeCore(CellRange value) {
			CellRange oldRange = this.range;
			this.range = value;
			RaiseRangeChanged(oldRange, range);
		}
		public static CellRangeBase GetWholeRange(CellRange newRange, CellUnion newPageFieldsRange, PivotTable pivotTable) {
			if (newPageFieldsRange == null)
				return newRange.Clone();
			bool hasGeneralRange = pivotTable.ColumnFields.Count > 0 || pivotTable.RowFields.Count > 0 || pivotTable.DataFields.Count > 0;
			if (!hasGeneralRange)
				return newPageFieldsRange.Clone();
			CellUnion result = newPageFieldsRange.Clone();
			result.InnerCellRanges.Add(newRange);
			return result;
		}
		public static CellUnion TryGetPageFieldsRange(CellRange generalRange, int columnPageCount, int rowPageCount, int pageFieldsCount, bool pageOverThenDown) {
			if (pageFieldsCount == 0)
				return null;
			Func<int, int> getRowCount;
			if (pageOverThenDown)
				getRowCount = (i) => {
					if ((pageFieldsCount - (rowPageCount - 1) * columnPageCount) <= i)
						return rowPageCount - 1;
					return rowPageCount;
				};
			else
				getRowCount = (i) => {
					if (i < columnPageCount - 1)
						return rowPageCount;
					int count = pageFieldsCount % rowPageCount;
					return count == 0 ? rowPageCount : count;
				};
			int leftColumn = generalRange.LeftColumnIndex;
			int topRow = generalRange.TopRowIndex - rowPageCount - 1;
			List<CellRangeBase> innerCellRanges = new List<CellRangeBase>();
			for (int i = 0; i < columnPageCount; i++) {
				CellPosition topLeft = new CellPosition(leftColumn + i * (PageFieldRangeWidth + 1), topRow);
				CellPosition bottomRight = new CellPosition(topLeft.Column + PageFieldRangeWidth - 1, topRow + getRowCount(i) - 1);
				innerCellRanges.Add(new CellRange(generalRange.Worksheet, topLeft, bottomRight));
			}
			return new CellUnion(generalRange.Worksheet, innerCellRanges);
		}
		public void UpdateRangesWithoutHistory(PivotTable pivotTable) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			CellUnion pageFieldsRange = TryGetPageFieldsRange(range, columnPageCount, rowPageCount, pivotTable.PageFields.Count, pivotTable.PageOverThenDown);
			if (pageFieldsRange != null)
				SetPageFieldsRangeCore(pageFieldsRange);
			CellRangeBase wholeRange = GetWholeRange(range, pageFieldsRange, pivotTable);
			SetWholeRangeCore(wholeRange);
		}
		#endregion
		public int GetDataColumnItemIndex(int absoluteDataIndex) {
			return absoluteDataIndex - range.LeftColumnIndex - firstDataColumn;
		}
		public int GetDataRowItemIndex(int absoluteDataIndex) {
			return absoluteDataIndex - range.TopRowIndex - firstDataRow;
		}
		#region IPivotTableLocation members
		PivotTableLocationCache IPivotTableLocation.Cache { get { return cache; } }
		#endregion
		#region GetRange methods (for API)
		protected internal CellRange TryGetRowRange(int rowFieldsCount, int dataFieldsCount) {
			if (firstDataColumn == 0 || (rowFieldsCount == 0 && dataFieldsCount == 1))
				return null;
			int left = range.LeftColumnIndex;
			int right = range.LeftColumnIndex + firstDataColumn - 1;
			int top = range.TopRowIndex + firstDataRow - 1;
			int bottom = range.BottomRowIndex;
			return TryGetRangeCore(left, right, top, bottom);
		}
		protected internal CellRange TryGetColumnRange() {
			if (firstDataRow <= 1)
				return null;
			int left = range.LeftColumnIndex + firstDataColumn;
			int right = range.RightColumnIndex;
			int top = range.TopRowIndex;
			int bottom = range.TopRowIndex + firstDataRow - 1;
			return TryGetRangeCore(left, right, top, bottom);
		}
		protected internal CellRange TryGetDataRange() {
			if (HasPageFields && range.CellCount == 1)
				return null;
			int left = range.LeftColumnIndex + firstDataColumn;
			int right = range.RightColumnIndex;
			if (left > right)
				left = right;
			int top = range.TopRowIndex + firstDataRow;
			int bottom = range.BottomRowIndex;
			if (top > bottom)
				top = bottom;
			return TryGetRangeCore(left, right, top, bottom);
		}
		CellRange TryGetRangeCore(int left, int right, int top, int bottom) {
			CellPosition topLeft = new CellPosition(left, top);
			CellPosition bottomRight = new CellPosition(right, bottom);
			if (!topLeft.IsValid || !bottomRight.IsValid)
				return null;
			return new CellRange(range.Worksheet, topLeft, bottomRight);
		}
		#endregion
		#region RangeChanged
		CellRangeChangedEventHandler onRangeChanged;
		public event CellRangeChangedEventHandler RangeChanged { add { onRangeChanged += value; } remove { onRangeChanged -= value; } }
		protected internal virtual void RaiseRangeChanged(CellRange oldRange, CellRange newRange) {
			if (onRangeChanged != null) {
				CellRangeChangedEventArgs args = new CellRangeChangedEventArgs(oldRange, newRange);
				onRangeChanged(this, args);
			}
		}
		#endregion
		#region WholeRangeChanged
		CellRangeBaseChangedEventHandler onWholeRangeChanged;
		public event CellRangeBaseChangedEventHandler WholeRangeChanged { add { onWholeRangeChanged += value; } remove { onWholeRangeChanged -= value; } }
		protected internal virtual void RaiseWholeRangeChanged(CellRangeBase oldWholeRange, CellRangeBase newWholeRange) {
			if (onWholeRangeChanged != null) {
				CellRangeBaseChangedEventArgs args = new CellRangeBaseChangedEventArgs(oldWholeRange, newWholeRange);
				onWholeRangeChanged(this, args);
			}
		}
		#endregion
		public void CopyFromNoHistory(PivotTableLocation source, CellPositionOffset offset) {
			cache.SetInvalid();
			wholeRange = source.wholeRange.GetShiftedAny(offset, range.Worksheet);
			if (source.pageFieldsRange == null)
				pageFieldsRange = null;
			else
				pageFieldsRange = source.pageFieldsRange.GetShiftedAny(offset, range.Worksheet);
			firstHeaderRow = source.firstHeaderRow;
			firstDataRow = source.firstDataRow;
			firstDataColumn = source.firstDataColumn;
			rowPageCount = source.rowPageCount;
			columnPageCount = source.columnPageCount;
		}
	}
	#endregion
	#region PivotTableLocationCache
	public class PivotTableLocationCache {
		#region Fields
		readonly PivotTableLocationPageFieldsZonesCache pageFieldsZonesCache = new PivotTableLocationPageFieldsZonesCache();
		readonly PivotTableLocationEmptyCellsCache emptyCellsCache = new PivotTableLocationEmptyCellsCache();
		PivotTableLocationZone general = PivotTableLocationZone.Invalid;
		PivotTableLocationZone columnHeader = PivotTableLocationZone.Invalid;
		PivotTableLocationZone rowHeader = PivotTableLocationZone.Invalid;
		PivotTableLocationZone firstHeaderCell = PivotTableLocationZone.Invalid;
		PivotTableLocationZone columnStripe = PivotTableLocationZone.Invalid;
		PivotTableLocationZone rowStripe = PivotTableLocationZone.Invalid;
		bool isValid = false;
		#endregion
		#region Properties
		public PivotTableLocationZone General { get { return general; } }
		public PivotTableLocationZone ColumnHeader { get { return columnHeader; } }
		public PivotTableLocationZone RowHeader { get { return rowHeader; } }
		public PivotTableLocationZone FirstHeaderCell { get { return firstHeaderCell; } set { firstHeaderCell = value; } } 
		public PivotTableLocationZone ColumnStripe { get { return columnStripe; } }
		public PivotTableLocationZone RowStripe { get { return rowStripe; } }
		public PivotTableLocationPageFieldsZonesCache PageFieldsZonesCache { get { return pageFieldsZonesCache; } }
		public PivotTableLocationEmptyCellsCache EmptyCellsCache { get { return emptyCellsCache; } }
		public bool IsValid { get { return isValid; } }
		#endregion
		public void Prepare(IPivotTableLocation location, bool hasRowFields) {
			if (isValid)
				return;
			CellRangeBase wholeRange = location.WholeRange;
			int firstDataColumn = location.FirstDataColumn;
			int firstDataRow = location.FirstDataRow;
			if (wholeRange.RangeType != CellRangeType.UnionRange)
				PrepareCore(wholeRange as CellRange, firstDataColumn, firstDataRow, hasRowFields);
			else
				PrepareWithPageRangeAndEmptyCells(wholeRange as CellUnion, firstDataColumn, firstDataRow, hasRowFields);
			isValid = true;
		}
		void PrepareWithPageRangeAndEmptyCells(CellUnion wholeRange, int firstDataColumn, int firstDataRow, bool hasRowFields) {
			List<CellRangeBase> ranges = wholeRange.InnerCellRanges;
			int count = ranges.Count;
			CellRangeBase lastRange = ranges[count - 1];
			bool hasGeneralRange = ranges[0].TopLeft.Row != lastRange.TopLeft.Row;
			if (hasGeneralRange) 
				PrepareCore(lastRange as CellRange, firstDataColumn, firstDataRow, hasRowFields);
			pageFieldsZonesCache.Prepare(wholeRange);
			emptyCellsCache.Prepare(wholeRange);
		}
		void PrepareCore(CellRange range, int firstDataColumn, int firstDataRow, bool hasRowFields) {
			CellPosition generalTopLeft = range.TopLeft;
			CellPosition generalBottomRight = range.BottomRight;
			general = new PivotTableLocationZone(generalTopLeft, generalBottomRight);
			bool hasHeaderColumn = firstDataColumn > 0;
			bool hasHeaderRow = firstDataRow > 0;
			int absoluteFirstDataColumn = generalTopLeft.Column + firstDataColumn;
			int absoluteFirstDataRow = generalTopLeft.Row + firstDataRow;
			if (hasHeaderColumn)
				columnHeader = new PivotTableLocationZone(generalTopLeft, new CellPosition(absoluteFirstDataColumn - 1, generalBottomRight.Row));
			if (hasHeaderRow)
				rowHeader = new PivotTableLocationZone(generalTopLeft, new CellPosition(generalBottomRight.Column, absoluteFirstDataRow - 1));
			if (hasHeaderColumn && hasHeaderRow && absoluteFirstDataRow > 1) 
				firstHeaderCell = new PivotTableLocationZone(generalTopLeft, new CellPosition(absoluteFirstDataColumn - 1, absoluteFirstDataRow - (hasRowFields ? 2 : 1)));
			if (absoluteFirstDataColumn <= generalBottomRight.Column && absoluteFirstDataRow <= generalBottomRight.Row) {
				columnStripe = new PivotTableLocationZone(new CellPosition(absoluteFirstDataColumn, absoluteFirstDataRow), generalBottomRight);
				rowStripe = new PivotTableLocationZone(new CellPosition(generalTopLeft.Column, absoluteFirstDataRow), generalBottomRight);
			}
		}
		public void SetInvalid() {
			if (!isValid)
				return;
			isValid = false;
			general = PivotTableLocationZone.Invalid;
			columnHeader = PivotTableLocationZone.Invalid;
			rowHeader = PivotTableLocationZone.Invalid;
			firstHeaderCell = PivotTableLocationZone.Invalid;
			columnStripe = PivotTableLocationZone.Invalid;
			rowStripe = PivotTableLocationZone.Invalid;
			pageFieldsZonesCache.Clear();
			emptyCellsCache.Clear();
		}
	}
	#endregion
	#region PivotTableLocationPageFieldsZonesCache
	public class PivotTableLocationPageFieldsZonesCache {
		readonly List<PivotTableLocationZone> innerCollection = new List<PivotTableLocationZone>();
		#region Properties
		public PivotTableLocationZone this[int index] { get { return innerCollection[index]; } }
		public int Count { get { return innerCollection.Count; } }
		public bool IsEmpty { get { return Count == 0; } }
		#endregion
		public void Prepare(CellUnion wholeRange) {
			List<CellRangeBase> ranges = (wholeRange as CellUnion).InnerCellRanges;
			int count = ranges.Count;
			bool hasGeneralRange = ranges[0].TopLeft.Row != ranges[count - 1].TopLeft.Row;
			if (hasGeneralRange)
				count--;
			for (int i = 0; i < count; i++) {
				CellRange range = ranges[i] as CellRange;
				PivotTableLocationZone zone = new PivotTableLocationZone();
				zone.TopLeft = range.TopLeft;
				zone.BottomRight = range.BottomRight;
				innerCollection.Add(zone);
			}
		}
		public int TryGetIndexByCellPosition(int column, int row) {
			if (IsEmpty)
				return -1;
			CellPosition topLeft = innerCollection[0].TopLeft;
			int topLeftColumn = topLeft.Column;
			if (column < topLeftColumn || row < topLeft.Row)
				return -1;
			int pageFieldRangeWidth = PivotTableLocation.PageFieldRangeWidth + 1;
			int relativeColumn = column - topLeftColumn;
			int aspect = relativeColumn % pageFieldRangeWidth;
			if (aspect == 2)
				return -1;
			int index = relativeColumn / pageFieldRangeWidth;
			if (row <= innerCollection[index].BottomRight.Row)
				return index;
			return -1;
		}
		public void Clear() {
			innerCollection.Clear();
		}
	}
	#endregion
	#region PivotTableLocationZone (struct)
	public struct PivotTableLocationZone {
		#region Static Members
		public static PivotTableLocationZone Invalid = CreateInvalid();
		static PivotTableLocationZone CreateInvalid() {
			PivotTableLocationZone result = new PivotTableLocationZone();
			result.topLeft = CellPosition.InvalidValue;
			result.bottomRight = CellPosition.InvalidValue;
			return result;
		}
		#endregion
		#region Fields
		CellPosition topLeft;
		CellPosition bottomRight;
		#endregion
		public PivotTableLocationZone(CellPosition topLeft, CellPosition bottomRight) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
		#region Properties
		public CellPosition TopLeft { get { return topLeft; } set { topLeft = value; } }
		public CellPosition BottomRight { get { return bottomRight; } set { bottomRight = value; } }
		public int Height { get { return bottomRight.Row - topLeft.Row + 1; } }
		public int Width { get { return bottomRight.Column - topLeft.Column + 1; } }
		public bool IsValid { get { return topLeft.IsValid && bottomRight.IsValid; } }
		#endregion
		public bool ContainsCell(int column, int row) {
			return
				column >= topLeft.Column && column <= bottomRight.Column &&
				row >= topLeft.Row && row <= bottomRight.Row;
		}
		public bool ContainsRange(CellRange range) {
			return
				range.LeftColumnIndex >= topLeft.Column && range.RightColumnIndex <= bottomRight.Column &&
				range.TopRowIndex >= topLeft.Row && range.BottomRowIndex <= bottomRight.Row;
		}
		public bool IntersectRange(CellRange range) {
			bool hasCommonColumns = Math.Max(topLeft.Column, range.LeftColumnIndex) <= Math.Min(bottomRight.Column, range.RightColumnIndex);
			bool hasCommonRows = Math.Max(topLeft.Row, range.TopRowIndex) <= Math.Min(bottomRight.Row, range.BottomRowIndex);
			return hasCommonColumns && hasCommonRows;
		}
		public TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(int column, int row) {
			return new TableStyleFormatBordersOutlineInfo(column == topLeft.Column, column == bottomRight.Column, row == topLeft.Row, row == bottomRight.Row);
		}
	}
	#endregion
	#region PivotTableLocationEmptyCellsType (enum)
	public enum PivotTableLocationEmptyCellsType {
		PageFields,
		TopRightPage,
		FirstBottomRightPage,
		LastBottomRightPage,
		FirstBeforeLocation,
		LastBeforeLocation,
		RightLocation
	}
	#endregion
	#region PivotTableLocationEmptyCellsInfo
	public struct PivotTableLocationEmptyCellsInfo {
		#region Fields
		PivotTableLocationZone locationZone;
		PivotTableLocationEmptyCellsType emptyCellsType;
		#endregion
		public PivotTableLocationEmptyCellsInfo(PivotTableLocationZone locationZone, PivotTableLocationEmptyCellsType emptyCellsType) {
			this.locationZone = locationZone;
			this.emptyCellsType = emptyCellsType;
		}
		#region Properties
		public PivotTableLocationZone LocationZone { get { return locationZone; } }
		public PivotTableLocationEmptyCellsType EmptyCellsType { get { return emptyCellsType; } }
		#endregion
	}
	#endregion
	#region PivotTableLocationEmptyCellsCache
	public class PivotTableLocationEmptyCellsCache {
		#region Fields
		readonly RTree2D<PivotTableLocationEmptyCellsInfo> tree = new RTree2D<PivotTableLocationEmptyCellsInfo>();
		bool isValid = false;
		#endregion
		#region Properties
		public bool IsValid { get { return isValid; } }
		public int Count { get { return tree.Count; } }
		#endregion
		public void Invalidate() {
			if (isValid) {
				isValid = false;
				Clear();
			}
		}
		public void Prepare(CellUnion wholeRange) {
			if (isValid || wholeRange == null)
				return;
			isValid = true;
			List<CellRangeBase> wholeRanges = wholeRange.InnerCellRanges;
			int wholeRangesCount = wholeRanges.Count;
			if (wholeRangesCount <= 1)
				return;
			CellRangeBase lastRange = wholeRanges[wholeRangesCount - 1];
			if (wholeRanges[0].TopLeft.Row != lastRange.TopLeft.Row)
				Prepare(wholeRanges);
			else
				PrepareOnlyPageEmptyCells(wholeRanges);
		}
		void PrepareOnlyPageEmptyCells(List<CellRangeBase> pageRanges) {
			CellRangeBase firstRange = pageRanges[0];
			CellPosition firstTopLeft = firstRange.TopLeft;
			CellPosition firstBottomRight = firstRange.BottomRight;
			int pageRangesCount = pageRanges.Count;
			CellPosition embeddedPagePosition;
			RegisterPageFieldsInfoes(pageRanges, pageRangesCount, firstTopLeft.Row, firstBottomRight, out embeddedPagePosition);
			if (embeddedPagePosition.IsValid) {
				int pageRight = GetPageRight(firstTopLeft.Column, pageRangesCount);
				RegisterInfo(embeddedPagePosition.Column + 1, embeddedPagePosition.Row + 1, pageRight, firstBottomRight.Row, PivotTableLocationEmptyCellsType.LastBottomRightPage);
			}
		}
		void Prepare(List<CellRangeBase> wholeRanges) {
			int pageRangesCount = wholeRanges.Count - 1;
			CellRangeBase firstRange = wholeRanges[0];
			CellPosition firstTopLeft = firstRange.TopLeft;
			CellPosition firstBottomRight = firstRange.BottomRight;
			int pageTop = firstTopLeft.Row;
			int pageLeft = firstTopLeft.Column;
			int pageBottom = firstBottomRight.Row;
			int pageRight = GetPageRight(pageLeft, pageRangesCount);
			CellPosition embeddedPagePosition;
			RegisterPageFieldsInfoes(wholeRanges, pageRangesCount, pageTop, firstBottomRight, out embeddedPagePosition);
			CellPosition locationBottomRight = wholeRanges[pageRangesCount].BottomRight;
			int locationRight = locationBottomRight.Column;
			int locationBottom = locationBottomRight.Row;
			if (embeddedPagePosition.IsValid) {
				int embeddedPageColumn = embeddedPagePosition.Column;
				int embeddedPageRow = embeddedPagePosition.Row;
				if (locationRight <= embeddedPageColumn) {
					RegisterInfo(embeddedPageColumn + 1, embeddedPageRow + 1, pageRight, pageBottom, PivotTableLocationEmptyCellsType.LastBottomRightPage);
					RegisterHorizontalInfo(pageLeft, locationRight, pageBottom + 1, PivotTableLocationEmptyCellsType.FirstBeforeLocation);
					RegisterHorizontalInfo(locationRight + 1, pageRight, pageBottom + 1, PivotTableLocationEmptyCellsType.LastBeforeLocation);
					RegisterInfo(locationRight + 1, pageBottom + 2, pageRight, locationBottom, PivotTableLocationEmptyCellsType.RightLocation);
					return;
				}
				if (locationRight <= pageRight) {
					RegisterInfo(embeddedPageColumn + 1, embeddedPageRow + 1, locationRight, pageBottom, PivotTableLocationEmptyCellsType.FirstBottomRightPage);
					RegisterInfo(locationRight + 1, embeddedPageRow + 1, pageRight, pageBottom, PivotTableLocationEmptyCellsType.LastBottomRightPage);
					RegisterHorizontalInfo(pageLeft, locationRight, pageBottom + 1, PivotTableLocationEmptyCellsType.FirstBeforeLocation);
					RegisterHorizontalInfo(locationRight + 1, pageRight, pageBottom + 1, PivotTableLocationEmptyCellsType.LastBeforeLocation);
					RegisterInfo(locationRight + 1, pageBottom + 2, pageRight, locationBottom, PivotTableLocationEmptyCellsType.RightLocation);
					return;
				}
				RegisterInfo(pageRight + 1, pageTop, locationRight, embeddedPageRow, PivotTableLocationEmptyCellsType.TopRightPage);
				RegisterInfo(embeddedPageColumn + 1, embeddedPageRow + 1, locationRight, pageBottom, PivotTableLocationEmptyCellsType.LastBottomRightPage);
				RegisterHorizontalInfo(pageLeft, locationRight, pageBottom + 1, PivotTableLocationEmptyCellsType.FirstBeforeLocation);
				return;
			}
			if (locationRight < pageRight) {
				RegisterHorizontalInfo(pageLeft, locationRight, pageBottom + 1, PivotTableLocationEmptyCellsType.FirstBeforeLocation);
				RegisterHorizontalInfo(locationRight + 1, pageRight, pageBottom + 1, PivotTableLocationEmptyCellsType.LastBeforeLocation);
				RegisterInfo(locationRight + 1, pageBottom + 2, pageRight, locationBottom, PivotTableLocationEmptyCellsType.RightLocation);
				return;
			}
			if (locationRight > pageRight)
				RegisterInfo(pageRight + 1, pageTop, locationRight, pageBottom, PivotTableLocationEmptyCellsType.TopRightPage);
			RegisterHorizontalInfo(pageLeft, locationRight, pageBottom + 1, PivotTableLocationEmptyCellsType.FirstBeforeLocation);
			isValid = true;
		}
		int GetPageRight(int pageLeft, int pageRangesCount) {
			return pageLeft + PivotTableLocation.PageFieldRangeWidth * pageRangesCount + pageRangesCount - 2;
		}
		void RegisterPageFieldsInfoes(List<CellRangeBase> pageRanges, int pageRangesCount, int topRow, CellPosition firstBottomRight, out CellPosition embeddedPagePosition) {
			int bottomRow = firstBottomRight.Row;
			CellPosition currentPageBottomRight = firstBottomRight;
			int firstPageBottomRightRow = currentPageBottomRight.Row;
			int previousBottomRightColumn = currentPageBottomRight.Column;
			int previousBottomRightRow = currentPageBottomRight.Row;
			embeddedPagePosition = CellPosition.InvalidValue;
			for (int i = 1; i < pageRangesCount; i++) {
				RegisterVerticalInfo(previousBottomRightColumn + 1, topRow, previousBottomRightRow, PivotTableLocationEmptyCellsType.PageFields);
				currentPageBottomRight = pageRanges[i].BottomRight;
				int currentBottomRightColumn = currentPageBottomRight.Column;
				int currentBottomRightRow = currentPageBottomRight.Row;
				if (currentBottomRightRow < bottomRow && !embeddedPagePosition.IsValid)
					embeddedPagePosition = new CellPosition(previousBottomRightColumn + 1, currentBottomRightRow);
				previousBottomRightColumn = currentBottomRightColumn;
				previousBottomRightRow = currentBottomRightRow;
			}
		}
		void RegisterVerticalInfo(int column, int topRow, int bottomRow, PivotTableLocationEmptyCellsType emptyCellsType) {
			RegisterInfo(column, topRow, column, bottomRow, emptyCellsType);
		}
		void RegisterHorizontalInfo(int leftColumn, int rightColumn, int row, PivotTableLocationEmptyCellsType emptyCellsType) {
			RegisterInfo(leftColumn, row, rightColumn, row, emptyCellsType);
		}
		void RegisterInfo(int leftColumn, int topRow, int rightColumn, int bottomRow, PivotTableLocationEmptyCellsType emptyCellsType) {
			CellPosition topLeft = new CellPosition(leftColumn, topRow);
			CellPosition bottomRight = new CellPosition(rightColumn, bottomRow);
			PivotTableLocationZone zone = new PivotTableLocationZone(topLeft, bottomRight);
			PivotTableLocationEmptyCellsInfo info = new PivotTableLocationEmptyCellsInfo(zone, emptyCellsType);
			InsertCore(info);
		}
		protected virtual void InsertCore(PivotTableLocationEmptyCellsInfo info) {
			PivotTableLocationZone locationZone = info.LocationZone;
			CellPosition topLeft = locationZone.TopLeft;
			tree.Insert(topLeft.Column, topLeft.Row, locationZone.Width, locationZone.Height, info);
		}
		public void Clear() {
			tree.Clear();
		}
		public List<PivotTableLocationEmptyCellsInfo> GetIntersectedItems(CellRange range) {
			CellPosition topLeft = range.TopLeft;
			return tree.Search(new NodeBase(topLeft.Column, topLeft.Row, range.Width, range.Height), true);
		}
	}
	#endregion
	#region PivotTableLocationEmptyCellsNotificationHelper
	public static class PivotTableLocationEmptyCellsNotificationHelper {
		static Dictionary<PivotTableLocationEmptyCellsType, InsertCellMode> insertCellModeTable = GetInsertCellModeTable();
		static Dictionary<PivotTableLocationEmptyCellsType, RemoveCellMode> removeCellModeTable = GetRemoveCellModeTable();
		static Dictionary<PivotTableLocationEmptyCellsType, InsertCellMode> GetInsertCellModeTable() {
			Dictionary<PivotTableLocationEmptyCellsType, InsertCellMode> result = new Dictionary<PivotTableLocationEmptyCellsType, InsertCellMode>();
			InsertCellMode mode = InsertCellMode.ShiftCellsRight;
			result.Add(PivotTableLocationEmptyCellsType.FirstBeforeLocation, mode);
			result.Add(PivotTableLocationEmptyCellsType.FirstBottomRightPage, mode);
			result.Add(PivotTableLocationEmptyCellsType.TopRightPage, mode);
			mode |= InsertCellMode.ShiftCellsDown;
			result.Add(PivotTableLocationEmptyCellsType.LastBottomRightPage, mode);
			result.Add(PivotTableLocationEmptyCellsType.LastBeforeLocation, mode);
			result.Add(PivotTableLocationEmptyCellsType.RightLocation, mode);
			return result;
		}
		static Dictionary<PivotTableLocationEmptyCellsType, RemoveCellMode> GetRemoveCellModeTable() {
			Dictionary<PivotTableLocationEmptyCellsType, RemoveCellMode> result = new Dictionary<PivotTableLocationEmptyCellsType, RemoveCellMode>();
			RemoveCellMode mode = RemoveCellMode.Default | RemoveCellMode.NoShiftOrRangeToPasteCutRange;
			result.Add(PivotTableLocationEmptyCellsType.PageFields, mode);
			mode |= RemoveCellMode.ShiftCellsLeft;
			result.Add(PivotTableLocationEmptyCellsType.FirstBeforeLocation, mode);
			result.Add(PivotTableLocationEmptyCellsType.FirstBottomRightPage, mode);
			result.Add(PivotTableLocationEmptyCellsType.TopRightPage, mode);
			mode |= RemoveCellMode.ShiftCellsUp;
			result.Add(PivotTableLocationEmptyCellsType.LastBottomRightPage, mode);
			result.Add(PivotTableLocationEmptyCellsType.LastBeforeLocation, mode);
			result.Add(PivotTableLocationEmptyCellsType.RightLocation, mode);
			return result;
		}
		public static bool CheckPermittedInsertCellMode(PivotTableLocationEmptyCellsType type, InsertCellMode mode) {
			if (insertCellModeTable.ContainsKey(type))
				return (insertCellModeTable[type] & mode) != 0;
			return false;
		}
		public static bool CheckPermittedRemoveCellMode(PivotTableLocationEmptyCellsType type, RemoveCellMode mode) {
			return (removeCellModeTable[type] & mode) != 0;
		}
	}
	#endregion
}
