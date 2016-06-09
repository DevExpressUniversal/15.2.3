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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region AutoFilterBase
	public abstract class AutoFilterBase {
		AutoFilterColumnCollection filterColumns;
		CellRange filterRange;
		SortState sortState;
		Worksheet sheet;
		protected AutoFilterBase(Worksheet sheet, CellRange filterRange) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.sortState = new SortState(sheet);
			this.filterColumns = new AutoFilterColumnCollection(sheet);
			SetRangeCore(filterRange);
		}
		public abstract IFilteringBehaviour FilteringBehaviour { get; }
		public bool Enabled { get { return Range != null; } }
		public AutoFilterColumnCollection FilterColumns { get { return filterColumns; } }
		public SortState SortState { get { return sortState; } }
		public Worksheet Sheet { get { return sheet; } }
		public DocumentModel Workbook { get { return Sheet.Workbook; } }
		public bool IsNonDefault {
			get {
				if (!Enabled)
					return false;
				foreach (AutoFilterColumn column in filterColumns)
					if (column.IsNonDefault)
						return true;
				return false;
			}
		}
		bool ConsistsOnlyOfHeaderRow { get { return Enabled ? Range.Height <= 1 : false; } }
		#region Range
		public virtual CellRange Range {
			get { return filterRange; }
			set {
				if (CellRange.Equals(filterRange, value))
					return;
				Workbook.BeginUpdate(); 
				try {
					DocumentHistory history = Workbook.History;
					ChangeAutoFilterRangeHistoryItem item = new ChangeAutoFilterRangeHistoryItem(this, filterRange, value);
					history.Add(item);
					item.Execute();
					SetDefinedName();
				}
				finally {
					Workbook.EndUpdate();
				}
			}
		}
		public void CreateFilterColumnsForRange(CellRange range) {
			FilterColumns.ClearCore();
			for (int i = 0; i < range.Width; ++i)
				FilterColumns.AddCore(new AutoFilterColumn(Sheet) { ColumnId = i });
		}
		protected internal void SetRangeCore(CellRange range) {
			if (range == null)
				FilterColumns.ClearCore();
			else
				if (Range == null)
					CreateFilterColumnsForRange(range);
			SetRange(range);
		}
		protected internal void SetRangeCore(CellRange range, AutoFilterColumnCollection savedCollection) {
			if (range == null)
				FilterColumns.ClearCore();
			else
				if (Range == null)
					for (int i = 0; i < range.Width; i++)
						FilterColumns.AddCore(savedCollection[i]);
			SetRange(range);
		}
		public void SetRange(CellRange range) {
			this.filterRange = range;
			RaiseOnRangeChanged(FilterColumns);
		}
		void SetDefinedName() {
			if (Range == null)
				RemoveDefinedName();
			else
				CreateDefinedName(Range);
		}
		protected virtual void RemoveDefinedName() {
		}
		protected virtual void CreateDefinedName(CellRange range) {
		}
		#endregion
		#region OnRangeChanged
		AutoFilterRangeChangedEventHandler onRangeChanged;
		internal event AutoFilterRangeChangedEventHandler OnRangeChanged { add { onRangeChanged += value; } remove { onRangeChanged -= value; } }
		void RaiseOnRangeChanged(AutoFilterColumnCollection filtersColumns) {
			if (onRangeChanged != null)
				onRangeChanged(this, new AutoFilterRangeChangedEventArgs(filtersColumns));
		}
		#endregion
		#region OnRangeRemoving
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			OnRangeRemovingCore(notificationContext.Range, notificationContext.Mode);
		}
		protected internal void OnRangeRemovingCore(CellRange removableRange, RemoveCellMode mode) {
			if (!Enabled)
				return;
			if (mode == RemoveCellMode.ShiftCellsLeft) {
				OnRangeRemovingShiftCellsLeft(removableRange);
				SortState.OnRangeRemovingShiftCellsLeft(removableRange);
			}
			else if (mode == RemoveCellMode.ShiftCellsUp) {
				OnRangeRemovingShiftCellsUp(removableRange);
				SortState.OnRangeRemovingShiftCellsUp(removableRange);
			}
			else
				OnRangeRemovingDefault(removableRange);
		}
		protected virtual void OnRangeRemovingShiftCellsLeft(CellRange removableRange) {
		}
		protected virtual void OnRangeRemovingShiftCellsUp(CellRange removableRange) {
		}
		protected virtual void OnRangeRemovingDefault(CellRange removableRange) {
		}
		#endregion
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (!Enabled)
				return;
			CellRange insertableRange = notificationContext.Range;
			if (notificationContext.Mode == InsertCellMode.ShiftCellsDown) {
				OnRangeInsertingShiftCellsDown(insertableRange);
				SortState.OnRangeInsertingShiftDown(insertableRange);
			}
			else {
				OnRangeInsertingShiftCellsRight(insertableRange);
				SortState.OnRangeInsertingShiftRight(insertableRange);
			}
		}
		protected virtual void OnRangeInsertingShiftCellsDown(CellRange insertableRange) {
		}
		protected virtual void OnRangeInsertingShiftCellsRight(CellRange insertableRange) {
		}
		#endregion
		internal bool IsRowVisible(CellRange range, int rowIndex) {
			if (range == null)
				return true;
			if (rowIndex < range.TopRowIndex || rowIndex > range.BottomRowIndex)
				return true;
			int count = this.FilterColumns.Count;
			for (int i = 0; i < count; i++)
				if (!FilterColumns[i].IsRowVisible(range, rowIndex, FilteringBehaviour))
					return false;
			return true;
		}
		#region Disable
		public void Disable() {
			if (!Enabled)
				return;
			Workbook.BeginUpdate();
			try {
				ClearFilterCore();
				Range = null;
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		#endregion
		#region ClearFilter
		public void ClearFilter() {
			if (!Enabled)
				return;
			Workbook.BeginUpdate();
			try {
				ClearFilterCore();
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		void ClearFilterCore() {
			ClearColumns();
			SortState.Clear();
			UnHideRows();
		}
		void ClearColumns() {
			int count = FilterColumns.Count;
			for (int i = 0; i < count; i++)
				FilterColumns[i].Clear();
		}
		void UnHideRows() {
			if (ConsistsOnlyOfHeaderRow)
				return;
			int to = Range.BottomRight.Row;
			for (int i = Range.TopLeft.Row; i <= to; i++)
				UnhideRow(Sheet.Rows[i]);
		}
		void UnhideRow(Row row) {
			if (!FilterRowVisibilityHelper.IsRowHiddenByOtherFilter(this, row))
				Sheet.UnhideRows(row.Index, row.Index);
		}
		#endregion
		#region ReApply
		public virtual void ReApplyFilter() {
			if (!Enabled || ConsistsOnlyOfHeaderRow)
				return;
			Workbook.BeginUpdate();
			try {
				UpdateDynamicFilters();
				ReApplyFilterCore(GetDataRange());
				Workbook.CalculationChain.MarkupDependentsForRecalculation(Range);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		void ReApplyFilterCore(CellRange range) {
			int to = range.BottomRight.Row;
			for (int i = range.TopLeft.Row; i <= to; i++) {
				bool rowShouldBeHidden = !IsRowVisible(range, i);
				Row currentRow = Sheet.Rows[i];
				if (rowShouldBeHidden != currentRow.IsHidden) {
					if (rowShouldBeHidden)
						Sheet.HideRows(i, i);
					else
						UnhideRow(currentRow);
				}
			}
		}
		void UpdateDynamicFilters() {
			int count = FilterColumns.Count;
			for (int i = 0; i < count; i++) {
				AutoFilterColumn column = FilterColumns[i];
				CellRange columnRange = column.GetColumnRange(this);
				CellAutoFilterValueProvider valueProvider = new CellAutoFilterValueProvider(columnRange);
				column.Update(valueProvider, FilteringBehaviour);
			}
		}
		#endregion
		#region GetDataRange
		public CellRange GetDataRange() {
			CellRange range = Range;
			if (!Enabled)
				return null;
			if (ConsistsOnlyOfHeaderRow) {
				range = TryExtendDataRange(range);
				if (range.Height <= 1)
					return range;
			}
			CellPosition topLeft = range.TopLeft;
			topLeft = new CellPosition(topLeft.Column, topLeft.Row + 1);
			return new CellRange(range.Worksheet, topLeft, range.BottomRight);
		}
		public CellRange GetFilterColumnDataRange(int modelColumnIndex) {
			CellRange range = Range;
			if (range == null)
				return null;
			if (ConsistsOnlyOfHeaderRow)
				range = TryExtendDataRange(range);
			int topRowIndex = range.TopLeft.Row;
			CellPosition topLeft = new CellPosition(modelColumnIndex, range.Height > 1 ? topRowIndex + 1 : topRowIndex);
			CellPosition bottomRight = new CellPosition(modelColumnIndex, range.BottomRight.Row);
			return new CellRange(range.Worksheet, topLeft, bottomRight);
		}
		#endregion
		#region ExtendDataRange
		protected virtual CellRange TryExtendDataRange(CellRange range) {
			for (; ; ) {
				CellRange extendedRange = ExtendDown(range);
				if (extendedRange.CellCount == 1)
					break;
				if (range.CellCount == extendedRange.CellCount)
					break;
				range = extendedRange;
			}
			return range.Clone();
		}
		CellRange ExtendDown(CellRange range) {
			int row = range.BottomRight.Row;
			int to = range.BottomRight.Column;
			for (int i = range.TopLeft.Column; i <= to; i++)
				range = ExtendDown(range, new CellPosition(i, row));
			return range;
		}
		CellRange ExtendDown(CellRange range, CellPosition from) {
			for (int i = from.Row + 1; i < sheet.MaxRowCount; i++) {
				ICell cell = sheet.TryGetCell(from.Column, i);
				if (cell == null || cell.Value.IsEmpty)
					return range;
				range = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column, i));
			}
			return range;
		}
		#endregion
		public void CopyFrom(AutoFilterBase sourceAutoFilter) {
			CellRange sourceAutoFilterRange = sourceAutoFilter.Range;
			CopyFilterRange(sourceAutoFilterRange);
			this.SortState.CopyFrom(sourceAutoFilter.SortState);
			for (int i = 0; i < sourceAutoFilter.FilterColumns.Count; ++i)
				FilterColumns[i].CopyFrom(sourceAutoFilter.FilterColumns[i]);
		}
		protected virtual void CopyFilterRange(CellRange sourceAutoFilterRange) {
			if (sourceAutoFilterRange != null)
				Range = sourceAutoFilterRange.Clone(this.Sheet) as CellRange;
		}
	}
	#endregion
	#region SheetAutoFilter
	public class SheetAutoFilter : AutoFilterBase {
		public static readonly string filterDatabaseName = "_xlnm._FilterDatabase";
		public SheetAutoFilter(Worksheet sheet)
			: this(sheet, null) {
		}
		public SheetAutoFilter(Worksheet sheet, CellRange filterRange)
			: base(sheet, filterRange) {
		}
		public override IFilteringBehaviour FilteringBehaviour { get { return DefaultFilteringBehaviour.Instance; } }
		#region Notifications
		public bool CanRangeRemove(CellRangeBase removableRange, RemoveCellMode mode) {
			if (!Enabled)
				return true;
			if (IsNonDefault && removableRange.RangeType != CellRangeType.IntervalRange &&
			   (mode == RemoveCellMode.ShiftCellsUp || mode == RemoveCellMode.ShiftCellsLeft))
				return false;
			return RemoveOrInsertFilterRangeHelper.CanRangeRemove(Range, removableRange, mode);
		}
		public bool CanRangeInsert(CellRangeBase insertableRange, InsertCellMode mode) {
			if (!Enabled)
				return true;
			if (IsNonDefault && insertableRange.RangeType != CellRangeType.IntervalRange)
				return false;
			return RemoveOrInsertFilterRangeHelper.CanRangeInsert(Range, insertableRange, mode);
		}
		#region OnRangeRemoving
		protected override void OnRangeRemovingShiftCellsLeft(CellRange removableRange) {
			if (TryInvalidateRange(removableRange) ||
				removableRange.TopLeft.Column > Range.BottomRight.Column ||
				removableRange.TopLeft.Row > Range.TopLeft.Row ||
				removableRange.BottomRight.Row < Range.BottomRight.Row)
				return;
			CellRangeBase intersectRange = removableRange.Intersects(Range) ? removableRange.IntersectionWith(Range).CellRangeValue : null;
			int offset = -removableRange.Width;
			if (intersectRange == null) {
				Range = Range.GetResized(offset, 0, offset, 0);
				return;
			}
			int deletedWidth = intersectRange.Width;
			int firstColumnDeleted = intersectRange.TopLeft.Column - Range.TopLeft.Column;
			if (firstColumnDeleted <= 0) {
				Range = Range.GetResized(deletedWidth + offset, 0, offset, 0);
				UpdateColumns(deletedWidth, 0);
			}
			else {
				if (removableRange.RangeType == CellRangeType.IntervalRange) {
					Range = Range.GetResized(0, 0, -deletedWidth, 0);
					UpdateColumns(deletedWidth, firstColumnDeleted);
				}
			}
		}
		void UpdateColumns(int deletedWidth, int firstColumnDeleted) {
			for (int i = 0; i < deletedWidth; ++i)
				FilterColumns.RemoveAt(firstColumnDeleted);
			ReApplyFilter();
		}
		protected override void OnRangeRemovingShiftCellsUp(CellRange removableRange) {
			if (TryInvalidateRange(removableRange) ||
				removableRange.TopLeft.Row > Range.BottomRight.Row ||
				removableRange.TopLeft.Column > Range.TopLeft.Column ||
				removableRange.BottomRight.Column < Range.BottomRight.Column)
				return;
			int intersectHeight = removableRange.Intersects(Range) ? removableRange.IntersectionWith(Range).CellRangeValue.Height : 0;
			if (intersectHeight == 0) {
				int offset = -removableRange.Height;
				Range = Range.GetResized(0, offset, 0, offset);
			}
			else
				Range = Range.GetResized(0, 0, 0, -intersectHeight);
		}
		protected override void OnRangeRemovingDefault(CellRange removableRange) {
			TryInvalidateRange(removableRange);
		}
		bool TryInvalidateRange(CellRange cellRange) {
			CellRange fistRowRange = Range.GetSubRowRange(0, 0);
			if (cellRange.Includes(fistRowRange)) {
				Disable();
				return true;
			}
			return false;
		}
		#endregion
		#region OnRangeInserting
		protected override void OnRangeInsertingShiftCellsDown(CellRange insertableRange) {
			int insertableTopRow = insertableRange.TopLeft.Row;
			if (insertableTopRow > Range.BottomRight.Row ||
				insertableRange.TopLeft.Column > Range.TopLeft.Column ||
				insertableRange.BottomRight.Column < Range.BottomRight.Column)
				return;
			int offset = insertableRange.Height;
			if (insertableTopRow <= Range.TopLeft.Row)
				Range = Range.GetResized(0, offset, 0, offset);
			else
				Range = Range.GetResized(0, 0, 0, offset);
		}
		protected override void OnRangeInsertingShiftCellsRight(CellRange insertableRange) {
			int insertableLeftColumn = insertableRange.TopLeft.Column;
			if (insertableLeftColumn > Range.BottomRight.Column ||
				insertableRange.TopLeft.Row > Range.TopLeft.Row ||
				insertableRange.BottomRight.Row < Range.BottomRight.Row)
				return;
			int offset = insertableRange.Width;
			int firstColumnInserted = insertableLeftColumn - Range.TopLeft.Column;
			if (firstColumnInserted <= 0)
				Range = Range.GetResized(offset, 0, offset, 0);
			else {
				if (insertableRange.RangeType == CellRangeType.IntervalRange) {
					Range = Range.GetResized(0, 0, offset, 0);
					for (int i = 0; i < offset; ++i)
						FilterColumns.Insert(firstColumnInserted, new AutoFilterColumn(Sheet) { ColumnId = firstColumnInserted });
				}
			}
		}
		#endregion
		#endregion
		#region DefinedNames
		protected override void RemoveDefinedName() {
			Sheet.RemoveDefinedName(filterDatabaseName);
		}
		protected override void CreateDefinedName(CellRange range) {
			DefinedNameBase filterDatabase;
			if (Sheet.DefinedNames.TryGetItemByName(filterDatabaseName, out filterDatabase))
				filterDatabase.Expression = CreateFilterDatabaseExpression(range);
			else {
				DefinedName name = Sheet.CreateDefinedName(filterDatabaseName, CreateFilterDatabaseExpression(range));
				name.IsHidden = true;
			}
		}
		ParsedExpression CreateFilterDatabaseExpression(CellRange range) {
			ICellTable sheet = range.Worksheet;
			SheetDefinition sheetDefinition = new SheetDefinition(sheet.Name);
			int sheetDefinitionIndex = Workbook.DataContext.RegisterSheetDefinition(sheetDefinition);
			CellOffset topLeft = range.TopLeft.AsAbsolute().ToCellOffset();
			CellOffset bottomRight = range.BottomRight.AsAbsolute().ToCellOffset();
			ParsedThingArea3dRel ptg = new ParsedThingArea3dRel(topLeft, bottomRight, sheetDefinitionIndex);
			ParsedExpression expression = new ParsedExpression();
			expression.Add(ptg);
			return expression;
		}
		#endregion
		protected internal CellRange GetShrinkedRange() {
			CellRange range = Range;
			if (!Enabled)
				return null;
			for (int i = range.BottomRowIndex; i > range.TopRowIndex; i--) {
				for (int j = range.LeftColumnIndex; j <= range.RightColumnIndex; j++) {
					ICell cell = Sheet.TryGetCell(j, i);
					if (cell != null && !cell.Value.IsEmpty)
						return new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column, i));
				}
			}
			return range.Clone();
		}
	}
	#endregion
	#region TableAutoFilter
	public class TableAutoFilter : AutoFilterBase {
		public TableAutoFilter(Table table)
			: base(table.Worksheet, table.Range) {
		}
		public override IFilteringBehaviour FilteringBehaviour { get { return DefaultFilteringBehaviour.Instance; } }
		public void OnTableRangeSetting(CellRange newTableRange, bool tableHasTotalRows) {
			if (Enabled)
				Range = tableHasTotalRows ? newTableRange.GetResized(0, 0, 0, -1) : newTableRange.Clone();
		}
		protected override CellRange TryExtendDataRange(CellRange range) {
			return range.Clone();
		}
	}
	#endregion
	public interface IFilteringBehaviour {
		bool AllowsNumericAndWildcardComparison { get; }
		bool AllowsStringComparison { get; }
		bool DefaultTop10Behaviour { get; }
	}
	public class DefaultFilteringBehaviour : IFilteringBehaviour {
		static DefaultFilteringBehaviour instance = new DefaultFilteringBehaviour();
		public static DefaultFilteringBehaviour Instance {
			get {
				return instance;
			}
		}
		DefaultFilteringBehaviour() {
		}
		#region FilteringBehaviour Members
		public bool AllowsNumericAndWildcardComparison { get { return false; } }
		public bool AllowsStringComparison { get { return false; } }
		public bool DefaultTop10Behaviour { get { return true; } }
		#endregion
	}
	#region RemoveOrInsertFilterRangeHelper
	public struct RemoveOrInsertFilterRangeHelper {
		#region Static Members
		public static bool CanRangeRemove(CellRange filterRange, CellRangeBase removableRange, RemoveCellMode mode) {
			RemoveOrInsertFilterRangeHelper helper = new RemoveOrInsertFilterRangeHelper();
			helper.Initialize(filterRange);
			return helper.CanRangeRemove(removableRange, mode);
		}
		public static bool CanRangeInsert(CellRange filterRange, CellRangeBase insertableRange, InsertCellMode mode) {
			RemoveOrInsertFilterRangeHelper helper = new RemoveOrInsertFilterRangeHelper();
			helper.Initialize(filterRange);
			return helper.CanRangeInsert(insertableRange, mode);
		}
		#endregion
		CellRange filterRange;
		#region Properties
		ICellTable Sheet { get { return filterRange.Worksheet; } }
		int FilterFirstColumn { get { return filterRange.TopLeft.Column; } }
		int FilterFirstRow { get { return filterRange.TopLeft.Row; } }
		int FilterLastColumn { get { return filterRange.BottomRight.Column; } }
		int FilterLastRow { get { return filterRange.BottomRight.Row; } }
		#endregion
		void Initialize(CellRange filterRange) {
			this.filterRange = filterRange;
		}
		delegate bool RemoveOrInsertFilterRangeValidator(CellRange actionRange);
		#region CanRangeRemove
		bool CanRangeRemove(CellRangeBase actionRange, RemoveCellMode mode) {
			if (mode == RemoveCellMode.ShiftCellsUp)
				return CanRangeRemoveOrInsertCore(actionRange, CanRangeRemoveShiftCellsUp);
			return true; 
		}
		bool CanRangeRemoveShiftCellsUp(CellRange actionRange) {
			return !IntersectsWithRangeAboveFilter(actionRange) || CanShiftCellsAboveFilter(actionRange);
		}
		#endregion
		#region CanRangeInsert
		bool CanRangeInsert(CellRangeBase actionRange, InsertCellMode mode) {
			if (mode == InsertCellMode.ShiftCellsRight)
				return CanRangeRemoveOrInsertCore(actionRange, CanRangeInsertShiftCellsRight);
			else
				return CanRangeRemoveOrInsertCore(actionRange, CanRangeInsertShiftCellsDown);
		}
		#region CanRangeInsertShiftCellsRight
		bool CanRangeInsertShiftCellsRight(CellRange actionRange) {
			if (IntersectsWithRangeLeftOfFilter(actionRange))
				return CanShiftCellsLeftOfFilter(actionRange);
			else {
				CellRange filterFirstColumnRange = filterRange.GetSubColumnRange(0, 0);
				if (actionRange.Intersects(filterFirstColumnRange))
					return actionRange.BottomRight.Column > FilterFirstColumn || actionRange.ContainsRange(filterFirstColumnRange);
				else
					return actionRange.TopLeft.Row != FilterFirstRow || actionRange.TopLeft.Column > FilterLastColumn;
			}
		}
		bool IntersectsWithRangeLeftOfFilter(CellRange actionRange) {
			if (FilterFirstColumn == 0)
				return false;
			CellRange rangeLeftOfFilter = new CellRange(Sheet, new CellPosition(0, FilterFirstRow), new CellPosition(FilterFirstColumn - 1, FilterLastRow));
			return actionRange.Intersects(rangeLeftOfFilter);
		}
		bool CanShiftCellsLeftOfFilter(CellRange actionRange) {
			return (actionRange.TopLeft.Row <= FilterFirstRow && actionRange.BottomRight.Row >= FilterLastRow) ||
					actionRange.BottomRight.Column > FilterFirstColumn;
		}
		#endregion
		#region CanRangeInsertShiftCellsDown
		bool CanRangeInsertShiftCellsDown(CellRange actionRange) {
			if (IntersectsWithRangeAboveFilter(actionRange))
				return CanShiftCellsAboveFilter(actionRange);
			else {
				CellRange filterFirstRowRange = filterRange.GetSubRowRange(0, 0);
				return !actionRange.Intersects(filterFirstRowRange) || actionRange.TopLeft.Column < FilterFirstColumn || actionRange.ContainsRange(filterFirstRowRange);
			}
		}
		bool IntersectsWithRangeAboveFilter(CellRange actionRange) {
			if (FilterFirstRow == 0)
				return false;
			CellRange rangeAboveFilter = new CellRange(Sheet, new CellPosition(FilterFirstColumn, 0), new CellPosition(FilterLastColumn, FilterFirstRow - 1));
			return actionRange.Intersects(rangeAboveFilter);
		}
		bool CanShiftCellsAboveFilter(CellRange actionRange) {
			return (actionRange.TopLeft.Column <= FilterFirstColumn && actionRange.BottomRight.Column >= FilterLastColumn) ||
					actionRange.BottomRight.Row >= FilterFirstRow;
		}
		#endregion
		#endregion
		bool CanRangeRemoveOrInsertCore(CellRangeBase actionRange, RemoveOrInsertFilterRangeValidator validator) {
			CellRangeType rangeType = actionRange.RangeType;
			if (rangeType == CellRangeType.UnionRange)
				return ProcessUnionRange((CellUnion)actionRange, validator);
			else if (rangeType == CellRangeType.SingleRange)
				return validator(actionRange.GetFirstInnerCellRange());
			return true;
		}
		bool ProcessUnionRange(CellUnion unionRange, RemoveOrInsertFilterRangeValidator validator) {
			IList<CellRangeBase> innerRanges = unionRange.InnerCellRanges;
			int count = innerRanges.Count;
			for (int i = 0; i < count; i++)
				if (!CanRangeRemoveOrInsertCore(innerRanges[i], validator))
					return false;
			return true;
		}
	}
	#endregion
	#region FilterRowVisibilityHelper
	public struct FilterRowVisibilityHelper {
		#region Static Members
		public static bool IsRowHiddenByOtherFilter(AutoFilterBase autoFilter, Row row) {
			FilterRowVisibilityHelper helper = new FilterRowVisibilityHelper();
			helper.Initialize(autoFilter);
			return helper.IsRowHiddenByOtherFilter(row.GetCellIntervalRange(), row.Index);
		}
		public static bool IsRowHiddenByOtherFilterOrColumns(AutoFilterBase autoFilter, Row row, int columnIndex, IFilteringBehaviour filteringBehaviour) {
			FilterRowVisibilityHelper helper = new FilterRowVisibilityHelper();
			helper.Initialize(autoFilter);
			int rowIndex = row.Index;
			return helper.IsRowHiddenByOtherFilter(row.GetCellIntervalRange(), rowIndex) ||
				   helper.IsRowHiddenByOtherColumns(rowIndex, columnIndex, filteringBehaviour);
		}
		#endregion
		AutoFilterBase autoFilter;
		Worksheet Sheet { get { return autoFilter.Sheet; } }
		void Initialize(AutoFilterBase autoFilter) {
			this.autoFilter = autoFilter;
		}
		bool IsRowHiddenByOtherFilter(CellRange rowRange, int rowIndex) {
			if (IsRowHiddenByOtherFilterCore(Sheet.AutoFilter, rowIndex))
				return true;
			List<Table> tables = Sheet.Tables.GetItems(rowRange, true);
			int count = tables.Count;
			for (int i = 0; i < count; i++)
				if (IsRowHiddenByOtherFilterCore(tables[i].AutoFilter, rowIndex))
					return true;
			return false;
		}
		bool IsRowHiddenByOtherFilterCore(AutoFilterBase otherFilter, int rowIndex) {
			if (!otherFilter.Enabled || otherFilter.Range.Equals(autoFilter.Range))
				return false;
			return !otherFilter.IsRowVisible(otherFilter.GetDataRange(), rowIndex);
		}
		bool IsRowHiddenByOtherColumns(int rowIndex, int columnIndex, IFilteringBehaviour filteringBehaviour) {
			CellRange filterRange = autoFilter.GetDataRange();
			AutoFilterColumnCollection columns = autoFilter.FilterColumns;
			int columnsCount = columns.Count;
			for (int i = 0; i < columnsCount; i++) {
				AutoFilterColumn current = columns[i];
				if (current.ColumnId == columnIndex)
					continue;
				if (!current.IsRowVisible(filterRange, rowIndex, filteringBehaviour))
					return true;
			}
			return false;
		}
	}
	#endregion
}
