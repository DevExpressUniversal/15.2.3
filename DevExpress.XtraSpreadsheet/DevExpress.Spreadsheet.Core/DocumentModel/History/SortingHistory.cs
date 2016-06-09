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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region ChangeSortConditionSortReferenceHistoryItem
	public class ChangeSortConditionSortReferenceHistoryItem : SpreadsheetCellRangeHistoryItem {
		#region Fields
		readonly SortCondition sortCondition;
		#endregion
		public ChangeSortConditionSortReferenceHistoryItem(SortCondition sortCondition, CellRange oldRange, CellRange newRange)
			: base(sortCondition.DocumentModel.ActiveSheet, oldRange, newRange) {
			this.sortCondition = sortCondition;
		}
		protected override void UndoCore() {
			sortCondition.SetSortReferenceInternal(OldRange);
		}
		protected override void RedoCore() {
			sortCondition.SetSortReferenceInternal(NewRange);
		}
	}
	#endregion
	#region SortConditionDeleteHistoryItem
	public class SortConditionDeleteHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly SortConditionCollection columns;
		readonly SortCondition column;
		readonly int index;
		#endregion
		public SortConditionDeleteHistoryItem(SortConditionCollection columns, SortCondition column)
			: base(column.DocumentModel.ActiveSheet) {
			Guard.ArgumentNotNull(columns, "columns");
			this.columns = columns;
			this.column = column;
			this.index = columns.IndexOf(column);
		}
		protected override void RedoCore() {
			columns.RemoveCore(column);
		}
		protected override void UndoCore() {
			columns.InsertCore(column, index);
		}
	}
	#endregion
	#region SortConditionInsertHistoryItem
	public class SortConditionInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly SortConditionCollection columns;
		readonly SortCondition column;
		readonly int index;
		#endregion
		public SortConditionInsertHistoryItem(SortConditionCollection columns, SortCondition column, int index)
			: base(column.DocumentModel.ActiveSheet) {
			Guard.ArgumentNotNull(columns, "columns");
			this.columns = columns;
			this.column = column;
			this.index = index;
		}
		protected override void RedoCore() {
			columns.InsertCore(column, index);
		}
		protected override void UndoCore() {
			columns.RemoveCore(column);
		}
	}
	#endregion
	#region ChangeSortStateSortRangeHistoryItem
	public class ChangeSortStateSortRangeHistoryItem : SpreadsheetCellRangeHistoryItem {
		readonly SortState owner;
		public ChangeSortStateSortRangeHistoryItem(Worksheet worksheet, SortState owner, CellRange oldRange, CellRange newRange)
			: base(worksheet, oldRange, newRange) {
			this.owner = owner;
		}
		protected override void UndoCore() {
			owner.SetSortRangeCore(OldRange);
		}
		protected override void RedoCore() {
			owner.SetSortRangeCore(NewRange);
		}
	}
	#endregion
	#region ChangeAutoFilterRangeHistoryItem
	public class ChangeAutoFilterRangeHistoryItem : SpreadsheetCellRangeHistoryItem {
		readonly AutoFilterBase autoFilter;
		AutoFilterColumnCollection savedCollection;
		public ChangeAutoFilterRangeHistoryItem(AutoFilterBase autoFilter, CellRange oldRange, CellRange newRange)
			: base(autoFilter.Sheet, oldRange, newRange) {
			this.autoFilter = autoFilter;
			this.savedCollection = new AutoFilterColumnCollection(autoFilter.Sheet);
		}
		protected override void UndoCore() {
			SetRange(autoFilter.FilterColumns, OldRange);
		}
		protected override void RedoCore() {
			SetRange(autoFilter.FilterColumns, NewRange);
		}
		void SetRange(AutoFilterColumnCollection columns, CellRange range) {
			SaveCollection(columns);
			if (savedCollection.Count == 0)
				autoFilter.SetRangeCore(range);
			else
				autoFilter.SetRangeCore(range, savedCollection);
		}
		void SaveCollection(AutoFilterColumnCollection sourceCollection) {
			int count = sourceCollection.Count;
			for (int i = 0; i < count; i++)
				savedCollection.AddCore(sourceCollection[i]);
		}
	}
	#endregion
	#region AutoFilterColumnInsertHistoryItem
	public class AutoFilterColumnInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly AutoFilterColumnCollection columns;
		readonly AutoFilterColumn column;
		readonly int index;
		#endregion
		public AutoFilterColumnInsertHistoryItem(AutoFilterColumnCollection columns, AutoFilterColumn column, int index)
			: base(column.Sheet) {
			Guard.ArgumentNotNull(columns, "columns");
			this.columns = columns;
			this.column = column;
			this.index = index;
		}
		protected override void RedoCore() {
			columns.InsertCore(column, index);
		}
		protected override void UndoCore() {
			columns.RemoveCore(column);
		}
	}
	#endregion
	#region AutoFilterColumnDeleteHistoryItem
	public class AutoFilterColumnDeleteHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly AutoFilterColumnCollection columns;
		readonly AutoFilterColumn column;
		readonly int index;
		#endregion
		public AutoFilterColumnDeleteHistoryItem(AutoFilterColumnCollection columns, AutoFilterColumn column)
			: base(column.Sheet) {
			Guard.ArgumentNotNull(columns, "columns");
			this.columns = columns;
			this.column = column;
			this.index = columns.IndexOf(column);
		}
		protected override void RedoCore() {
			columns.RemoveCore(column);
		}
		protected override void UndoCore() {
			columns.InsertCore(column, index);
		}
	}
	#endregion
	#region AutoFilterColumnIdHistoryItem
	public class AutoFilterColumnIdHistoryItem : SpreadsheetIntHistoryItem {
		readonly AutoFilterColumn column;
		public AutoFilterColumnIdHistoryItem(AutoFilterColumn column, int newValue, int oldValue)
			: base(column.Sheet.Workbook, oldValue, newValue) {
			this.column = column;
		}
		protected override void RedoCore() {
			column.SetColumnIdCore(NewValue);
		}
		protected override void UndoCore() {
			column.SetColumnIdCore(OldValue);
		}
	}
	#endregion
	#region CustomFilterCriterionAndHistoryItem
	public class CustomFilterCriterionAndHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly CustomFilterCollection filters;
		public CustomFilterCriterionAndHistoryItem(CustomFilterCollection filters, bool newValue, bool oldValue)
			: base(filters.Sheet.Workbook, oldValue, newValue) {
			this.filters = filters;
		}
		protected override void RedoCore() {
			filters.SetCriterionAndCore(NewValue);
		}
		protected override void UndoCore() {
			filters.SetCriterionAndCore(OldValue);
		}
	}
	#endregion
	#region CustomFilterInsertHistoryItem
	public class CustomFilterInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly CustomFilterCollection filters;
		readonly CustomFilter filter;
		readonly int index;
		#endregion
		public CustomFilterInsertHistoryItem(CustomFilterCollection filters, CustomFilter filter, int index)
			: base(filters.Sheet) {
			Guard.ArgumentNotNull(filters, "columns");
			this.filters = filters;
			this.filter = filter;
			this.index = index;
		}
		protected override void RedoCore() {
			filters.InsertCore(filter, index);
		}
		protected override void UndoCore() {
			filters.RemoveCore(filter);
		}
	}
	#endregion
	#region CustomFilterRemoveHistoryItem
	public class CustomFilterRemoveHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly CustomFilterCollection filters;
		readonly CustomFilter filter;
		readonly int index;
		#endregion
		public CustomFilterRemoveHistoryItem(CustomFilterCollection filters, CustomFilter filter)
			: base(filters.Sheet) {
			Guard.ArgumentNotNull(filters, "columns");
			this.filters = filters;
			this.filter = filter;
			this.index = filters.IndexOf(filter);
		}
		protected override void RedoCore() {
			filters.RemoveCore(filter);
		}
		protected override void UndoCore() {
			filters.InsertCore(filter, index);
		}
	}
	#endregion
	#region FilterCriteriaFilterByBlankHistoryItem
	public class FilterCriteriaFilterByBlankHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly FilterCriteria criteria;
		public FilterCriteriaFilterByBlankHistoryItem(FilterCriteria criteria, bool newValue, bool oldValue)
			: base(criteria.Sheet.Workbook, oldValue, newValue) {
			this.criteria = criteria;
		}
		protected override void RedoCore() {
			criteria.SetFilterByBlankCore(NewValue);
		}
		protected override void UndoCore() {
			criteria.SetFilterByBlankCore(OldValue);
		}
	}
	#endregion
}
