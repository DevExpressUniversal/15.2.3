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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.IO;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public class GridViewCustomBindingArgsBase: EventArgs {
		public GridViewCustomBindingArgsBase(GridViewModel state) {
			State = state;
			FilterExpression = state.TotalFilterExpression;
		}
		public GridViewModel State { get; protected set; }
		public string FilterExpression { get; protected set; }
	}
	public class GridViewCustomBindingGetDataRowCountArgs: GridViewCustomBindingArgsBase {
		public GridViewCustomBindingGetDataRowCountArgs(GridViewModel state) : base(state) { }
		public GridViewCustomBindingGetDataRowCountArgs(GridViewModel state, string filterExpression)
			: base(state) {
			FilterExpression = filterExpression;
		}
		public int DataRowCount { get; set; }
	}
	public class GridViewCustomBindingGetUniqueHeaderFilterValuesArgs: GridViewCustomBindingArgsBase {
		public GridViewCustomBindingGetUniqueHeaderFilterValuesArgs(GridViewModel state, string fieldName, string filterExpression)
			: base(state) {
			FieldName = fieldName;
		}
		public string FieldName { get; protected set; }
		public IEnumerable Data { get; set; }
	}
	public class GridViewCustomBindingDataArgsBase: GridViewCustomBindingArgsBase {
		public GridViewCustomBindingDataArgsBase(GridViewModel state, IList<GridViewGroupInfo> groupInfoList)
			: base(state) {
			GroupInfoList = groupInfoList;
		}
		public IList<GridViewGroupInfo> GroupInfoList { get; protected set; }
		public IEnumerable Data { get; set; }
	}
	public class GridViewCustomBindingGetDataArgs: GridViewCustomBindingDataArgsBase {
		public GridViewCustomBindingGetDataArgs(GridViewModel state, IList<GridViewGroupInfo> groupInfoList, int startDataRowIndex, int count)
			: base(state, groupInfoList) {
			StartDataRowIndex = startDataRowIndex;
			DataRowCount = count;
		}
		public int StartDataRowIndex { get; protected set; }
		public int DataRowCount { get; protected set; }
	}
	public class GridViewCustomBindingGetGroupingInfoArgs: GridViewCustomBindingDataArgsBase {
		public GridViewCustomBindingGetGroupingInfoArgs(GridViewModel state, IList<GridViewGroupInfo> groupInfoList, string fieldName, ColumnSortOrder sortOrder)
			: base(state, groupInfoList) {
			FieldName = fieldName;
			SortOrder = sortOrder;
		}
		public string FieldName { get; protected set; }
		public ColumnSortOrder SortOrder { get; protected set; }
		public new IEnumerable<GridViewGroupInfo> Data { get { return (IEnumerable<GridViewGroupInfo>)base.Data; } set { base.Data = value; } }
	}
	public class GridViewCustomBindingGetSummaryValuesArgs: GridViewCustomBindingDataArgsBase {
		public GridViewCustomBindingGetSummaryValuesArgs(GridViewModel state, IList<GridViewGroupInfo> groupInfoList, List<GridViewSummaryItemState> summaryItems)
			: base(state, groupInfoList) {
			SummaryItems = summaryItems;
		}
		public List<GridViewSummaryItemState> SummaryItems { get; protected set; }
	}
	public delegate void GridViewCustomBindingGetDataRowCountHandler(GridViewCustomBindingGetDataRowCountArgs e);
	public delegate void GridViewCustomBindingGetUniqueHeaderFilterValuesHandler(GridViewCustomBindingGetUniqueHeaderFilterValuesArgs e);
	public delegate void GridViewCustomBindingGetDataHandler(GridViewCustomBindingGetDataArgs e);
	public delegate void GridViewCustomBindingGetGroupingInfoHandler(GridViewCustomBindingGetGroupingInfoArgs e);
	public delegate void GridViewCustomBindingGetSummaryValuesHandler(GridViewCustomBindingGetSummaryValuesArgs e);
	public class GridViewGroupInfo {
		public string FieldName { get; set; }
		public object KeyValue { get; set; }
		public int DataRowCount { get; set; }
	}
	public class GridViewSearchPanelState : GridBaseSearchPanelState {
		public new string ColumnNames { get { return base.ColumnNames; } set { base.ColumnNames = value; } }
		public new string Filter { get { return base.Filter; } set { base.Filter = value; } }
		public new GridViewSearchPanelGroupOperator GroupOperator { get { return base.GroupOperator; } set { base.GroupOperator = value; } }
	}
	public class GridViewModel : GridBaseViewModel {
		ReadOnlyCollection<GridViewColumnState> groupedColumns;
		public GridViewModel()
			: base() {
				Initialize();
		}
		protected internal GridViewModel(ASPxGridBase grid)
			: base(grid) {
				Initialize();
		}
		protected void Initialize() {
			GroupSummary = (GridViewSummaryItemStateCollection)CreateSummary();
		}
		protected override GridBaseCustomOperationHelper CreateCustomOperationHelper() {
			return new GridViewCustomOperationHelper(this);
		}
		protected override IGridColumnStateCollection CreateColumns() {
			return new GridViewColumnStateCollection(this);
		}
		protected override GridBasePagerState CreatePager() {
			return new GridViewPagerState(this);
		}
		protected override GridBaseSearchPanelState CreateSearchPanel() {
			return new GridViewSearchPanelState();
		}
		protected override IGridSummaryItemStateCollection CreateSummary() {
			return new GridViewSummaryItemStateCollection();
		}
		protected internal new GridViewCustomOperationHelper CustomOperationHelper {
			get { return (GridViewCustomOperationHelper)base.CustomOperationHelper; }
			set { base.CustomOperationHelper = value; }
		}
		public new GridViewColumnStateCollection Columns { get { return (GridViewColumnStateCollection)base.Columns; } }
		public new GridViewPagerState Pager { get { return (GridViewPagerState)base.Pager; } }
		public new string KeyFieldName { get { return base.KeyFieldName; } set { base.KeyFieldName = value; } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new bool IsFilterApplied { get { return base.IsFilterApplied; } set { base.IsFilterApplied = value; } }
		public new string AppliedFilterExpression { get { return base.AppliedFilterExpression; } }
		public new GridViewSearchPanelState SearchPanel { get { return (GridViewSearchPanelState)base.SearchPanel; } }
		protected internal FindSearchParserResults SearchPanelFilterParseResult { get; set; }
		public new GridViewSummaryItemStateCollection TotalSummary { get { return (GridViewSummaryItemStateCollection)base.TotalSummary; } }
		public GridViewSummaryItemStateCollection GroupSummary { get; private set; }
		public new ReadOnlyCollection<GridViewColumnState> SortedColumns {
			get { return base.SortedColumns.OfType<GridViewColumnState>().ToList().AsReadOnly(); }
		}
		public ReadOnlyCollection<GridViewColumnState> GroupedColumns { get { return GroupedColumnsInternal; } }
		protected internal override ReadOnlyCollection<GridViewColumnState> GroupedColumnsInternal {
			get {
				EnsureGroupedColumns();
				return groupedColumns;
			}
		}
		public void SortBy(GridViewColumnState column, bool reset) {
			base.SortBy(column, reset);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, null, null, null);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, null, null);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, null, getGroupingInfoMethod, null);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, getGroupingInfoMethod, null);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			if(MvcUtils.IsCallback())
				ExpandCollapseInfo = GridViewCustomOperationCallbackHelper.GetExpandCollapseInfo();
			CustomOperationHelper.ProcessCustomBinding(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, getGroupingInfoMethod, getUniqueHeaderFilterValuesMethod);
		}
		public void ApplyPagingState(GridViewPagerState pagerState) {
			base.ApplyPagingState(pagerState);
		}
		public void ApplySortingState(GridViewColumnState columnState) {
			base.ApplySortingState(columnState);
		}
		public void ApplySortingState(GridViewColumnState columnState, bool reset) {
			base.ApplySortingState(columnState, reset);
		}
		public void ApplyGroupingState(GridViewColumnState columnState) {
			ApplyColumnStateCore(columnState);
		}
		public void ApplyFilteringState(GridViewColumnState columnState) {
			base.ApplyFilteringState(columnState);
		}
		public void ApplyFilteringState(GridViewFilteringState filteringState) {
			base.ApplyFilteringState(filteringState);
		}
		protected internal override void Assign(GridBaseViewModel source) {
			GridViewModel viewModel = source as GridViewModel;
			if(viewModel != null) {
				GroupSummary.Assign(viewModel.GroupSummary);
				SearchPanelFilterParseResult = viewModel.SearchPanelFilterParseResult;
			}
			base.Assign(source);
		}
		internal void EnsureGroupedColumns() {
			if(this.groupedColumns != null)
				return;
			var list = new List<GridViewColumnState>();
			PopulateGroupedColumnsList(list);
			BuildGroupIndices(list);
			this.groupedColumns = list.AsReadOnly();
		}
		void PopulateGroupedColumnsList(List<GridViewColumnState> list) {
			foreach(var column in Columns) {
				if(column.GetGroupIndex() > -1)
					list.Add(column);
			}
			list.Sort((c1, c2) => { return c1.GetGroupIndex().CompareTo(c2.GetGroupIndex()); });
		}
		void BuildGroupIndices(List<GridViewColumnState> list) {
			for(int i = 0; i < list.Count; i++)
				list[i].SetGroupIndex(i);
		}
		internal void GroupedColumnsChanged(GridViewColumnState column) {
			var list = new List<GridViewColumnState>();
			PopulateGroupedColumnsList(list);
			list.Remove(column);
			var index = Math.Min(column.GroupIndex, list.Count);
			if(index > -1)
				list.Insert(index, column);
			BuildGroupIndices(list);
			this.groupedColumns = null;
		}
		protected internal static GridViewModel Load(string name) {
			return Load<GridViewModel>(name);
		}
		protected override void SaveCore(TypedBinaryWriter writer) {
			base.SaveCore(writer);
			writer.WriteObject(CustomOperationHelper.ExpandedState.Save());
			SaveSummary((ASPxSummaryItemCollection)Grid.DataProxy.GroupSummary, writer);
		}
		protected override void LoadCore(TypedBinaryReader reader) {
			base.LoadCore(reader);
			ExpandedStateValue = reader.ReadObject<string>();
			LoadSummary(GroupSummary, reader);
		}
		protected override void SaveColumn(TypedBinaryWriter writer, IWebGridDataColumn column) {
			base.SaveColumn(writer, column);
			var gridViewColumn = (GridViewDataColumn)column;
			writer.WriteObject(gridViewColumn.GroupIndex);
			writer.WriteObject((int)gridViewColumn.Settings.AutoFilterCondition);
		}
		protected override void LoadColumn(TypedBinaryReader reader, GridBaseColumnState column) {
			base.LoadColumn(reader, column);
			column.SetGroupIndex(reader.ReadObject<int>());
			column.AutoFilterCondition = (AutoFilterCondition)reader.ReadObject<int>();
		}
		protected override void SavePager(TypedBinaryWriter writer) {
			base.SavePager(writer);
			writer.WriteObject(Grid.PageSize);
		}
		protected override void LoadPager(TypedBinaryReader reader) {
			base.LoadPager(reader);
			Pager.PageSize = reader.ReadObject<int>();
		}
	}
	public class GridViewPagerState: GridBasePagerState {
		const int DefaultPageSize = 10;
		public GridViewPagerState()
			: base() {
			Initialize();
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, GridViewModel.Load(CurrentStateKey));
		}
		internal GridViewPagerState(GridViewModel owner)
			: base(owner) {
			Initialize();
		}
		void Initialize() {
			PageSize = DefaultPageSize;
		}
		public new int PageIndex { get { return base.PageIndex; } set { base.PageIndex = value; } }
		public new int PageSize { get { return base.PageSize; } set { base.PageSize = value; } }
		public override void Assign(GridBasePagerState source) {
			PageSize = source.PageSize;
			base.Assign(source);
		}
	}
	public class GridViewColumnStateCollection : GridBaseColumnStateCollection<GridViewColumnState> {
		internal GridViewColumnStateCollection(GridViewModel gridViewModel) 
			: base(gridViewModel) {
		}
		public new GridViewColumnState this[string fieldName] { get { return base[fieldName]; } }
		public new GridViewColumnState Add() {
			return base.Add();
		}
		public new GridViewColumnState Add(string fieldName) {
			return base.Add(fieldName);
		}
		public new GridViewColumnState Add(GridViewColumnState column) {
			return base.Add(column);
		}
		protected override GridViewColumnState CreateItem(string fieldName) {
			return new GridViewColumnState(fieldName);
		}
	}
	public class GridViewColumnState : GridBaseColumnState {
		int groupIndex = -1;
		public GridViewColumnState()
			: base() {
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, GridViewModel.Load(CurrentStateKey));
		}
		public GridViewColumnState(string fieldName)
			: base(fieldName) {
		}
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		public new ColumnSortOrder SortOrder { get { return base.SortOrder; } set { base.SortOrder = value; } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new int SortIndex { get { return base.SortIndex; } set { base.SortIndex = value; } }
		public new int GroupIndex { get { return base.GroupIndex; } set { base.GroupIndex = value; } }
		protected internal new GridViewModel ViewModel { get { return (GridViewModel)base.ViewModel; } }
		protected internal override void SetGroupIndex(int index) {
			if(GroupIndex == index)
				return;
			this.groupIndex = index;
			ChangeSortOrder(index);
			if(ViewModel != null)
				ViewModel.GroupedColumnsChanged(this);
		}
		protected internal override int GetGroupIndex() {
			return this.groupIndex;
		}
		public new DefaultBoolean AllowFilterBySearchPanel { get { return base.AllowFilterBySearchPanel; } set { base.AllowFilterBySearchPanel = value; } }
		public new void ForceType(Type columnType) { base.ForceType(columnType); }
	}
	public class GridViewSummaryItemState : GridBaseSummaryItemState {
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		public new SummaryItemType SummaryType { get { return base.SummaryType; } set { base.SummaryType = value; } }
		public new string Tag { get { return base.Tag; } set { base.Tag = value; } }
	}
	public class GridViewFilteringState : GridBaseFilteringState {
		public GridViewFilteringState()
			: base() {
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, DevExpress.Web.Mvc.GridViewModel.Load);
		}
		public new ReadOnlyCollection<GridViewColumnState> ModifiedColumns { get { return base.ModifiedColumns.OfType<GridViewColumnState>().ToList().AsReadOnly(); } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new bool IsFilterApplied { get { return base.IsFilterApplied; } set { base.IsFilterApplied = value; } }
		public new string SearchPanelFilter { get { return base.SearchPanelFilter; } set { base.SearchPanelFilter = value; } }
	}
	public class GridViewSummaryItemStateCollection : GridBaseSummaryItemStateCollection<GridViewSummaryItemState> {
		public new GridViewSummaryItemState Add(GridViewSummaryItemState state) {
			base.Add(state);
			return state;
		}
		protected internal override GridViewSummaryItemState CreateNewItem() {
			return new GridViewSummaryItemState();
		}
	}
}
