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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public abstract class GridBaseViewModel {
		static ReadOnlyCollection<GridViewColumnState> defaultGroupedColumns = new List<GridViewColumnState>().AsReadOnly();
		ReadOnlyCollection<GridBaseColumnState> sortedColumns;
		string totalFilterExpression;
		public GridBaseViewModel() {
			CustomOperationHelper = CreateCustomOperationHelper();
			Columns = CreateColumns();
			Pager = CreatePager();
			SearchPanel = CreateSearchPanel();
			IsFilterApplied = true;
			TotalSummary = CreateSummary();
		}
		protected internal GridBaseViewModel(ASPxGridBase grid)
			: this() {
			Grid = grid;
		}
		protected internal virtual ASPxGridBase Grid { get; private set; }
		protected internal GridBaseCustomOperationHelper CustomOperationHelper { get; set; }
		protected internal bool IsFilterApplied { get; set; }
		protected internal string KeyFieldName { get; set; }
		protected internal string FilterExpression { get; set; }
		protected internal string AppliedFilterExpression { get { return IsFilterApplied ? FilterExpression : string.Empty; } }
		protected virtual internal ReadOnlyCollection<GridViewColumnState> GroupedColumnsInternal { get { return defaultGroupedColumns; } }
		protected internal ReadOnlyCollection<GridBaseColumnState> SortedColumns {
			get {
				EnsureSortedColumns();
				return sortedColumns;
			}
		}
		protected internal IGridColumnStateCollection Columns { get; protected set; }
		protected internal GridBasePagerState Pager { get; private set; }
		protected internal GridBaseSearchPanelState SearchPanel { get; set; }
		protected internal IGridSummaryItemStateCollection TotalSummary { get; protected set; }
		protected internal string TotalFilterExpression {
			get {
				if(totalFilterExpression == null)
					totalFilterExpression = CalcTotalFilterExpression();
				return totalFilterExpression;
			}
		}
		protected abstract IGridColumnStateCollection CreateColumns();
		protected abstract GridBasePagerState CreatePager();
		protected abstract GridBaseSearchPanelState CreateSearchPanel();
		protected abstract IGridSummaryItemStateCollection CreateSummary();
		protected abstract GridBaseCustomOperationHelper CreateCustomOperationHelper();
		protected internal string ExpandedStateValue { get; set; }
		protected internal ExpandCollapseInfo ExpandCollapseInfo { get; set; }
		protected internal void ApplyPagingState(GridBasePagerState pagerState) {
			Pager.Assign(pagerState);
		}
		protected internal void ApplySortingState(GridBaseColumnState columnState) {
			ApplySortingState(columnState, true);
		}
		protected internal void ApplySortingState(GridBaseColumnState columnState, bool reset) {
			SortBy(columnState, reset);
		}
		protected internal void ApplyFilteringState(GridBaseColumnState columnState) {
			ApplyColumnStateCore(columnState);
		}
		protected internal void ApplyFilteringState(GridBaseFilteringState filteringState) {
			FilterExpression = filteringState.FilterExpression;
			IsFilterApplied = filteringState.IsFilterApplied;
			SearchPanel.Filter = filteringState.SearchPanelFilter;
			foreach(var modifiedColumn in filteringState.ModifiedColumns) {
				ApplyColumnStateCore(modifiedColumn);
			}
		}
		protected void ApplyColumnStateCore(GridBaseColumnState columnState) {
			var destinationColoumn = Columns[columnState.FieldName];
			if(destinationColoumn != null)
				destinationColoumn.Assign(columnState);
		}
		protected internal virtual void Assign(GridBaseViewModel source) {
			if(source == null)
				return;
			Columns.Assign(source.Columns);
			KeyFieldName = source.KeyFieldName;
			FilterExpression = source.FilterExpression;
			IsFilterApplied = source.IsFilterApplied;
			TotalSummary.Assign(source.TotalSummary);
			Pager.Assign(source.Pager);
			SearchPanel.Assign(source.SearchPanel);
			CustomOperationHelper = source.CustomOperationHelper;
			CustomOperationHelper.GridViewModel = this;
			source.CustomOperationHelper = null;
		}
		internal void EnsureSortedColumns() {
			if(this.sortedColumns != null)
				return;
			var list = new List<GridBaseColumnState>();
			PopulateSortedColumnsList(list);
			BuildSortIndices(list);
			this.sortedColumns = list.AsReadOnly();
		}
		void PopulateSortedColumnsList(List<GridBaseColumnState> list) {
			foreach(var column in Columns) {
				if(column.GetSortIndex() > -1)
					list.Add(column);
			}
			list.Sort((c1, c2) => { return c1.GetSortIndex().CompareTo(c2.GetSortIndex()); });
		}
		void BuildSortIndices(List<GridBaseColumnState> list) {
			for(int i = 0; i < list.Count; i++)
				list[i].SetSortIndex(i);
		}
		internal void SortedColumnsChanged(GridBaseColumnState column) {
			var list = new List<GridBaseColumnState>();
			PopulateSortedColumnsList(list);
			list.Remove(column);
			var index = Math.Min(column.SortIndex, list.Count);
			if(index > -1 && column.GroupIndex < 0)
				list.Insert(index, column);
			BuildSortIndices(list);
			this.sortedColumns = null;
		}
		protected void ResetSorting() {
			foreach(var column in Columns) {
				if(column.GroupIndex > -1)
					continue;
				column.SortIndex = -1;
				column.SortOrder = ColumnSortOrder.None;
			}
		}
		protected internal void SortBy(GridBaseColumnState column, bool reset) {
			var targetColumn = Columns[column.FieldName];
			if(targetColumn == null)
				return;
			var sortIndex = column.SortIndex;
			var order = column.SortOrder;
			if(reset)
				ResetSorting();
			targetColumn.SortIndex = sortIndex;
			targetColumn.SortOrder = order;
		}
		protected virtual void SaveCore(TypedBinaryWriter writer) {
			SaveColumns(writer);
			SavePager(writer);
			SaveSummary(Grid.DataProxy.TotalSummary, writer);
			writer.WriteObject(Grid.KeyFieldName);
			writer.WriteObject(Grid.FilterExpression);
			writer.WriteObject(Grid.FilterEnabled);
			writer.WriteObject(Grid.SearchPanelFilter);
			writer.WriteObject(Grid.SettingsSearchPanel.ColumnNames);
			writer.WriteObject(Grid.SettingsSearchPanel.GroupOperator);
		}
		protected virtual void LoadCore(TypedBinaryReader reader) {
			LoadColumns(reader);
			LoadPager(reader);
			LoadSummary(TotalSummary, reader);
			KeyFieldName = reader.ReadObject<string>();
			FilterExpression = reader.ReadObject<string>();
			IsFilterApplied = reader.ReadObject<bool>();
			SearchPanel.Filter = reader.ReadObject<string>();
			SearchPanel.ColumnNames = reader.ReadObject<string>();
			SearchPanel.GroupOperator = reader.ReadObject<GridViewSearchPanelGroupOperator>();
		}
		void SaveColumns(TypedBinaryWriter writer) {
			var dataColumns = Grid.ColumnHelper.AllDataColumns;
			writer.WriteObject(dataColumns.Count);
			foreach(var column in dataColumns) {
				SaveColumn(writer, column);
			}
		}
		protected virtual void SaveColumn(TypedBinaryWriter writer, IWebGridDataColumn column) {
			writer.WriteObject(column.FieldName);
			writer.WriteObject((int)column.SortOrder);
			writer.WriteObject(column.SortIndex);
			writer.WriteObject(Grid.ColumnHelper.GetColumnGlobalIndex(column));
			writer.WriteType(GetColumnDataType(column));
			writer.WriteObject((int)column.Adapter.Settings.FilterMode);
			writer.WriteObject((int)Grid.FilterHelper.GetEditKind(column));
			writer.WriteObject((int)column.Adapter.Settings.AllowFilterBySearchPanel);
		}
		Type GetColumnDataType(IWebGridDataColumn dataColumn) {
			var columnState = Columns[dataColumn.FieldName];
			if(columnState != null && columnState.DataType != null)
				return columnState.DataType;
			if(columnState != null && columnState.ViewModel.CustomOperationHelper.DataRows.Count == 0)
				return null;
			return dataColumn.Adapter.DataType;
		}
		void LoadColumns(TypedBinaryReader reader) {
			var count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				var column = Columns.Add();
				LoadColumn(reader, column);
			}
		}
		protected virtual void LoadColumn(TypedBinaryReader reader, GridBaseColumnState column) {
			column.FieldName = reader.ReadObject<string>();
			column.SortOrder = (ColumnSortOrder)reader.ReadObject<int>();
			column.SetSortIndex(reader.ReadObject<int>());
			column.Index = reader.ReadObject<int>();
			column.DataType = reader.ReadType();
			column.FilterMode = (ColumnFilterMode)reader.ReadObject<int>();
			column.EditKind = (GridColumnEditKind)reader.ReadObject<int>();
			column.AllowFilterBySearchPanel = (DefaultBoolean)reader.ReadObject<int>();
		}
		protected virtual void SavePager(TypedBinaryWriter writer) {
			var settings = Grid.SettingsPager;
			writer.WriteObject(Grid.PageIndex);
			writer.WriteObject(settings.Visible && settings.Mode == GridViewPagerMode.ShowPager);
			writer.WriteObject(Grid.PageCount);
			var showPageSizeItem = settings.PageSizeItemSettings.Visible;
			writer.WriteObject(showPageSizeItem);
			if(!showPageSizeItem)
				return;
			writer.WriteObject(settings.PageSizeItemSettings.ShowAllItem);
			writer.WriteObject(Grid.DataProxy.PageSizeShowAllItem);
			var items = settings.PageSizeItemSettings.Items.Select(i => int.Parse(i)).ToList();
			writer.WriteObject(items.Count);
			foreach(var item in items)
				writer.WriteObject(item);
		}
		protected virtual void LoadPager(TypedBinaryReader reader) {
			Pager.PageIndex = reader.ReadObject<int>();
			Pager.Visible = reader.ReadObject<bool>();
			Pager.PageCount = reader.ReadObject<int>();
			Pager.ShowPageSizeItem = reader.ReadObject<bool>();
			if(!Pager.ShowPageSizeItem)
				return;
			Pager.ShowAllItem = reader.ReadObject<bool>();
			Pager.ShowAllItemSelected = reader.ReadObject<bool>();
			var count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++)
				Pager.PageSizeItems.Add(reader.ReadObject<int>());
		}
		protected void SaveSummary(IList summaryItems, TypedBinaryWriter writer) {
			writer.WriteObject(summaryItems.Count);
			foreach(ASPxSummaryItemBase summary in summaryItems) {
				writer.WriteObject(summary.FieldName);
				writer.WriteObject((int)summary.SummaryType);
				writer.WriteObject(summary.Tag);
			}
		}
		protected void LoadSummary(IGridSummaryItemStateCollection summaryItems, TypedBinaryReader reader) {
			var count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				var item = summaryItems.Add();
				item.FieldName = reader.ReadObject<string>();
				item.SummaryType = (SummaryItemType)reader.ReadObject<int>();
				item.Tag = reader.ReadObject<string>();
			}
		}
		protected internal List<GridBaseColumnState> GetSearchableColumns() {
			var names = GridColumnHelper.CalcSearchPanelColumnNames((IEnumerable<IWebColumnInfo >)Columns, SearchPanel.ColumnNames);
			List<GridBaseColumnState> searchableColumns = new List<GridBaseColumnState>();
			foreach(var column in Columns) {
				var allow = column.AllowFilterBySearchPanel;
				var nameAssigned = names.Contains(column.FieldName);
				if(GridColumnHelper.IsAllowSearchByColumn(allow, nameAssigned))
					searchableColumns.Add(column);
			}
			return searchableColumns;
		}
		string CalcTotalFilterExpression() {
			List<CriteriaOperator> operators = new List<CriteriaOperator>();
			if(!string.IsNullOrEmpty(SearchPanel.Filter)) {
				var searchPanelFilterCriteria = MVCxGridViewCustomBindingFilterHelper.ParseSearchPanelFilter(this);
				if(!ReferenceEquals(searchPanelFilterCriteria, null))
					operators.Add(searchPanelFilterCriteria);
			}
			if(!string.IsNullOrEmpty(AppliedFilterExpression))
				operators.Add(CriteriaOperator.Parse(AppliedFilterExpression));
			if(operators.Count > 0)
				return CriteriaOperator.And(operators).ToString();
			return string.Empty;
		}
		protected static T Load<T>(string name) where T : GridBaseViewModel {
			string stringState = MVCxGridView.GetCustomOperationState(name);
			if(string.IsNullOrEmpty(stringState))
				return null;
			T result = Activator.CreateInstance<T>();
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(stringState))) {
				using(TypedBinaryReader reader = new TypedBinaryReader(stream))
					result.LoadCore(reader);
			}
			return result;
		}
		protected internal string Save() {
			if(Grid == null)
				return string.Empty;
			using(MemoryStream stream = new MemoryStream()) {
				using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
					SaveCore(writer);
					return Convert.ToBase64String(stream.ToArray());
				}
			}
		}
	}
	public class GridBasePagerState {
		public GridBasePagerState()
			: this(null) {
		}
		internal GridBasePagerState(GridBaseViewModel owner) {
			PageSizeItems = new List<int>();
		}
		protected internal int PageIndex { get; set; }
		protected internal virtual int PageSize { get; set; }
		protected internal bool Visible { get; set; }
		protected internal int PageCount { get; set; }
		protected internal bool ShowPageSizeItem { get; set; }
		protected internal bool ShowAllItem { get; set; }
		protected internal List<int> PageSizeItems { get; private set; }
		protected internal bool ShowAllItemSelected { get; set; }
		protected virtual string CurrentStateKey { get { return MvcUtils.CallbackName; } }
		public virtual void Assign(GridBasePagerState source) {
			if(source == null)
				return;
			PageIndex = source.PageIndex;
			Visible = source.Visible;
			PageCount = source.PageCount;
			ShowPageSizeItem = source.ShowPageSizeItem;
			ShowAllItem = source.ShowAllItem;
			ShowAllItemSelected = source.ShowAllItemSelected;
			PageSizeItems.Clear();
			PageSizeItems.AddRange(source.PageSizeItems);
		}
	}
	public class GridBaseFilteringState {
		List<GridBaseColumnState> modifiedColumnsInternal;
		string filterExpressionInternal;
		bool isFilterApplied;
		string searchPanelFilter;
		public GridBaseFilteringState() {
			this.modifiedColumnsInternal = new List<GridBaseColumnState>();
			this.isFilterApplied = true;
		}
		protected internal ReadOnlyCollection<GridBaseColumnState> ModifiedColumns { get { return this.modifiedColumnsInternal.AsReadOnly(); } }
		protected internal string FilterExpression {
			get { return GridViewModel != null ? GridViewModel.FilterExpression : filterExpressionInternal; }
			set {
				if(GridViewModel != null)
					GridViewModel.FilterExpression = value;
				else
					filterExpressionInternal = value;
			}
		}
		protected internal bool IsFilterApplied {
			get { return GridViewModel != null ? GridViewModel.IsFilterApplied : isFilterApplied; }
			set {
				if(GridViewModel != null)
					GridViewModel.IsFilterApplied = value;
				else
					isFilterApplied = value;
			}
		}
		protected internal string SearchPanelFilter {
			get { return GridViewModel != null ? GridViewModel.SearchPanel.Filter : searchPanelFilter; }
			set {
				if(GridViewModel != null)
					GridViewModel.SearchPanel.Filter = value;
				else
					searchPanelFilter = value;
			}
		}
		internal GridBaseViewModel GridViewModel { get; set; }
		internal void AddModifiedColumn(GridBaseColumnState columnState) {
			this.modifiedColumnsInternal.Add(columnState);
		}
	}
	public class GridBaseSummaryItemState {
		public GridBaseSummaryItemState() {
			Tag = string.Empty;
		}
		protected internal string FieldName { get; set; }
		protected internal SummaryItemType SummaryType { get; set; }
		protected internal string Tag { get; set; }
		public virtual void Assign(GridBaseSummaryItemState source) {
			if(source == null)
				return;
			FieldName = source.FieldName;
			SummaryType = source.SummaryType;
			Tag = source.Tag;
		}
	}
	public class GridBaseSearchPanelState {
		public GridBaseSearchPanelState() {
			ColumnNames = "*";
		}
		protected internal string ColumnNames { get; set; }
		protected internal string Filter { get; set; }
		protected internal GridViewSearchPanelGroupOperator GroupOperator { get; set; }
		public virtual void Assign(GridBaseSearchPanelState source) {
			if(source == null)
				return;
			ColumnNames = source.ColumnNames;
			Filter = source.Filter;
			GroupOperator = source.GroupOperator;
		}
	}
	public class GridBaseColumnState : IDataColumnInfo, IWebColumnInfo {
		int sortIndex = -1;
		string filterExpression;
		public GridBaseColumnState()
			: this(string.Empty) {
		}
		public GridBaseColumnState(string fieldName) {
			FieldName = fieldName;
			AllowFilterBySearchPanel = DefaultBoolean.Default;
		}
		protected internal GridBaseViewModel ViewModel { get; set; }
		protected internal string FieldName { get; set; }
		protected internal ColumnSortOrder SortOrder { get; set; }
		protected internal string FilterExpression {
			get {
				if(ViewModel != null)
					return GridViewCustomOperationCallbackHelper.GetColumnFilterExpression(ViewModel, this);
				return filterExpression;
			}
			set {
				if(ViewModel != null)
					GridViewCustomOperationCallbackHelper.ApplyFilterByColumn(ViewModel, this, CriteriaOperator.Parse(value));
				else
					filterExpression = value;
			}
		}
		protected internal int SortIndex {
			get { return sortIndex; }
			set {
				if(sortIndex == value)
					return;
				SetSortIndex(value);
				ChangeSortOrder(value);
				if(ViewModel != null)
					ViewModel.SortedColumnsChanged(this);
			}
		}
		protected internal int GroupIndex {
			get { return GetGroupIndex(); }
			set { SetGroupIndex(value); }
		}
		protected internal DefaultBoolean AllowFilterBySearchPanel { get; set; }
		protected internal int Index { get; set; }
		protected internal Type DataType { get; set; }
		protected internal ColumnFilterMode FilterMode { get; set; }
		protected internal GridColumnEditKind EditKind { get; set; }
		protected internal AutoFilterCondition AutoFilterCondition { get; set; }
		protected internal void SetSortIndex(int index) {
			this.sortIndex = index;
		}
		protected internal int GetSortIndex() {
			return this.sortIndex;
		}
		protected internal virtual void SetGroupIndex(int index) {
		}
		protected internal virtual int GetGroupIndex() {
			return -1;
		}
		protected virtual string CurrentStateKey { get { return MvcUtils.CallbackName; } }
		protected void ChangeSortOrder(int index) {
			if(SortOrder == ColumnSortOrder.None && index > -1)
				SortOrder = ColumnSortOrder.Ascending;
		}
		public virtual void Assign(GridBaseColumnState source) {
			if(source == null)
				return;
			FieldName = source.FieldName;
			SortOrder = source.SortOrder;
			SortIndex = source.SortIndex;
			GroupIndex = source.GroupIndex;
			FilterExpression = source.FilterExpression;
			Index = source.Index;
			DataType = source.DataType;
			FilterMode = source.FilterMode;
			EditKind = source.EditKind;
			AutoFilterCondition = source.AutoFilterCondition;
			AllowFilterBySearchPanel = source.AllowFilterBySearchPanel;
		}
		protected internal void ForceType(Type columnType) {
			DataType = columnType;
		}
		#region IDataColumnInfo Members
		string IDataColumnInfo.FieldName { get { return FieldName; } }
		string IDataColumnInfo.Caption { get { throw new NotImplementedException(); } }
		List<IDataColumnInfo> IDataColumnInfo.Columns { get { throw new NotImplementedException(); } }
		DataControllerBase IDataColumnInfo.Controller { get { return null; } }
		Type IDataColumnInfo.FieldType { get { return DataType; } }
		string IDataColumnInfo.Name { get { throw new NotImplementedException(); } }
		string IDataColumnInfo.UnboundExpression { get { throw new NotImplementedException(); } }
		#endregion
		#region IWebColumnInfo Members
		XtraGrid.ColumnGroupInterval IWebColumnInfo.GroupInterval { get { throw new NotImplementedException(); } }
		bool IWebColumnInfo.ReadOnly { get { throw new NotImplementedException(); } }
		ColumnSortOrder IWebColumnInfo.SortOrder { get { throw new NotImplementedException(); } }
		string IWebColumnInfo.UnboundExpression { get { throw new NotImplementedException(); } }
		UnboundColumnType IWebColumnInfo.UnboundType { get { throw new NotImplementedException(); } }
		EditPropertiesBase IWebColumnInfo.CreateEditProperties() { return null; }
		string IWebColumnInfo.FieldName { get { return FieldName; } }
		#endregion
	}
	public abstract class GridBaseColumnStateCollection<T> : List<T>, IGridColumnStateCollection where T : GridBaseColumnState {
		internal GridBaseColumnStateCollection(GridBaseViewModel viewModel)
			: base() {
			ViewModel = viewModel;
		}
		protected internal GridBaseViewModel ViewModel { get; private set; }
		protected internal T this[string fieldName] {
			get { return this.SingleOrDefault(c => c.FieldName == fieldName); }
		}
		protected internal T Add() {
			return Add(string.Empty);
		}
		protected internal T Add(string fieldName) {
			return Add(CreateItem(fieldName));
		}
		protected abstract T CreateItem(string fieldName);
		protected internal new T Add(T column) {
			base.Add(column);
			column.Index = Count - 1;
			column.ViewModel = ViewModel;
			return column;
		}
		public virtual void Assign(IGridColumnStateCollection source) {
			if(source == null)
				return;
			Clear();
			foreach(var item in source) {
				var column = Add();
				column.Assign(item);
			}
		}
		#region IGridColumnStateCollection members
		GridBaseColumnState IGridColumnStateCollection.Add() { return Add(); }
		IEnumerable<GridBaseColumnState> IGridColumnStateCollection.Where(Func<GridBaseColumnState, bool> predicate) { return this.Where(predicate); }
		IEnumerator<GridBaseColumnState> IGridColumnStateCollection.GetEnumerator() { return base.GetEnumerator(); }
		GridBaseColumnState IGridColumnStateCollection.this[string fieldName] { get { return this[fieldName]; } }
		#endregion
	}
	public abstract class GridBaseSummaryItemStateCollection<T> : List<T>, IGridSummaryItemStateCollection where T : GridBaseSummaryItemState {
		protected internal T Add() {
			var newItem = CreateNewItem();
			base.Add(newItem);
			return newItem;
		}
		public virtual void Assign(IGridSummaryItemStateCollection source) {
			if(source == null)
				return;
			Clear();
			foreach(var item in source) {
				var newItem = CreateNewItem();
				Add(newItem);
				newItem.Assign(item);
			}
		}
		protected internal abstract T CreateNewItem();
		#region IGridSummaryItemStateCollection members
		GridBaseSummaryItemState IGridSummaryItemStateCollection.Add() { return Add(); }
		IEnumerable<GridBaseSummaryItemState> IGridSummaryItemStateCollection.Where(Func<GridBaseSummaryItemState, bool> predicate) { return this.Where(predicate); }
		IEnumerable<T1> IGridSummaryItemStateCollection.OfType<T1>() { { return this.OfType<T1>(); } }
		int IGridSummaryItemStateCollection.IndexOf(GridBaseSummaryItemState item) { return this.IndexOf((T)item); }
		IEnumerator<GridBaseSummaryItemState> IGridSummaryItemStateCollection.GetEnumerator() { return base.GetEnumerator(); }
		#endregion
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public interface IGridColumnStateCollection {
		GridBaseColumnState Add();
		IEnumerable<GridBaseColumnState> Where(Func<GridBaseColumnState, bool> predicate);
		IEnumerator<GridBaseColumnState> GetEnumerator();
		GridBaseColumnState this[string fieldName] { get; }
		void Assign(IGridColumnStateCollection source);
	}
	public interface IGridSummaryItemStateCollection {
		GridBaseSummaryItemState Add();
		IEnumerable<GridBaseSummaryItemState> Where(Func<GridBaseSummaryItemState, bool> predicate);
		IEnumerable<T> OfType<T>() where T : GridBaseSummaryItemState;
		int IndexOf(GridBaseSummaryItemState item);
		int Count { get; }
		IEnumerator<GridBaseSummaryItemState> GetEnumerator();
		void Assign(IGridSummaryItemStateCollection source);
	}
}
