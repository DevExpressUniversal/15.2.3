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
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc.Internal {
	public class GridViewCustomOperationsProvider: WebDataProviderBase, IDataControllerData2 {
		class GridViewPropertyDescriptor: PropertyDescriptor {
			Type propertyType;
			internal GridViewPropertyDescriptor(string propertyName, Type propertyType)
				: base(propertyName, new Attribute[] { }) {
				this.propertyType = propertyType;
			}
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return propertyType; } }
			public override Type ComponentType { get { return null; } }
			public override bool CanResetValue(object component) {
				return true;
			}
			public override bool ShouldSerializeValue(object component) {
				return true;
			}
			public override object GetValue(object component) {
				throw new NotImplementedException();
			}
			public override void SetValue(object component, object value) {
				throw new NotImplementedException();
			}
			public override void ResetValue(object component) {
				throw new NotImplementedException();
			}
		}
		ListSourceDataController dataController;
		public GridViewCustomOperationsProvider(MVCxGridViewDataProxy proxy)
			: base(proxy) {
		}
		public new MVCxGridViewDataProxy Proxy { get { return (MVCxGridViewDataProxy)base.Proxy; } }
		protected GridBaseCustomOperationHelper CustomOperationHelper { get { return Proxy.CustomOperationHelper; } }
		protected ListSourceDataController DataController {
			get {
				if (dataController == null)
					PrepareDataController();
				return dataController;
			}
		}
		public override bool IsReady { get { return true; } }
		public override int VisibleCount { get { return CustomOperationHelper.VisibleRowCount; } }
		public override int ListSourceRowCount { get { return CustomOperationHelper.TotalRowCount; } }
		protected internal override DataColumnInfoCollection Columns { get { return DataController.Columns; } }
		public override string FilterExpression { get { return CustomOperationHelper.GridViewModel.FilterExpression; } }
		protected virtual void PrepareDataController() {
			this.dataController = new ListSourceDataController();
			this.dataController.ForcedDataRowType = Proxy.DataRowType;
			this.dataController.DataClient = this;
			this.dataController.SortClient = Proxy.Owner.SortClient;
			this.dataController.SetDataSource(CustomOperationHelper.DataRows);
		}
		public override int FindRowByKey(string keyFieldName, object keyValue, bool expandGroups) {
			var index = DataController.FindRowByValue(keyFieldName, keyValue);
			return index > -1 ? CustomOperationHelper.DataRowVisibleIndices[index] : -1;
		}
		public override int FindRowByKeys(Dictionary<string, object> columnValues, bool expandGroups) {
			if(columnValues == null) return -1;
			var index = DataController.FindRowByValues(columnValues);
			return index > -1 ? CustomOperationHelper.DataRowVisibleIndices[index] : -1;
		}
		public override Type GetFieldTypeCore(string fieldName) {
			return Columns[fieldName].Type;
		}
		public override List<int> GetParentGroupRows(int visibleIndex) {
			return CustomOperationHelper.GetParentGroupRows(visibleIndex);
		}
		public override List<int> GetFooterParentGroupRows(int visibleIndex) {
			return CustomOperationHelper.GetFooterParentGroupRows(visibleIndex);
		}
		public override object GetRow(int visibleIndex) {
			var dataRowIndex = CustomOperationHelper.GetDataRowIndex(visibleIndex);
			if(dataRowIndex == -1)
				return null;
			return DataController.GetRow(dataRowIndex);
		}
		public override int GetRowLevel(int visibleIndex) {
			return CustomOperationHelper.GetRowLevel(visibleIndex);
		}
		public override WebRowType GetRowType(int visibleIndex) {
			return CustomOperationHelper.GetRowType(visibleIndex);
		}
		public override object GetRowValue(int visibleIndex, string fieldName, bool isDesignTime) {
			if(GetRowType(visibleIndex) == WebRowType.Group)
				return CustomOperationHelper.GetGroupRowValue(visibleIndex);
			DataColumnInfo colInfo = DataController.Columns[fieldName];
			var dataRowIndex = CustomOperationHelper.GetDataRowIndex(visibleIndex);
			return colInfo != null ? DataController.GetRowValue(dataRowIndex, colInfo) : null;
		}
		public override bool HasFieldName(string fieldName) {
			return DataController.Columns[fieldName] != null;
		}
		public override bool IsGroupRowFitOnPage(int visibleIndex, int visibleStartIndex, int pageSize) {
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return true;
			return CustomOperationHelper.IsGroupRowFitOnPage(visibleIndex);
		}
		public override bool IsRowExpanded(int visibleIndex) {
			return CustomOperationHelper.IsRowExpanded(visibleIndex);
		}
		public override int RowIsLastInLevel(int visibleIndex) {
			return CustomOperationHelper.IsRowLastInLevel(visibleIndex) ? 0 : -1;
		}
		protected internal override void UpdateColumnBindings() {
			DataController.RePopulateColumns();
		}
		public override object GetListSourceRowValue(int listSourceRowIndex, string fieldName) {
			return DataController.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		protected internal override void ForceDataRowType() {
			if(Proxy == null) return;
			DataController.ForcedDataRowType = Proxy.DataRowType;
		}
		public override object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut) {
			var columnType = CustomOperationHelper.GridViewModel.Columns[fieldName].DataType;
			if (DataController.ListSource.Count > 0 && DataController.ListSource[0].GetType() == columnType)
				return DataController.ListSource.Cast<object>().ToArray();
			var column = DataController.Columns[fieldName];
			return column != null ? DataController.FilterHelper.GetUniqueColumnValues(column.Index, maxCount, true, true, null) : null;
		}
		public override object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			return CustomOperationHelper.GetTotalSummaryValue(item);
		}
		public override object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			return CustomOperationHelper.GetGroupSummaryValue(visibleIndex, item);
		}
		protected override List<object> GetTotalSummary() {
			return CustomOperationHelper.GetTotalSummary();
		}
		protected override List<object> GetGroupSummary(int visibleIndex) {
			return CustomOperationHelper.GetGroupSummary(visibleIndex);
		}
		public override bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			return CustomOperationHelper.IsGroupSummaryExists(visibleIndex, item);
		}
		public override void ValidateSelectedKeys() {
		}
		public override bool IsValidKey(object serializedKey) {
			return true;
		}
		public override Dictionary<object, bool> GetFilteredSelectedKeys() {
			return new Dictionary<object, bool>();
		}
		public override Dictionary<string, Type> GetColumnPropertyTypes() {
			var columns = new List<DataColumnInfo>();
			columns.AddRange(DataController.Columns);
			columns.AddRange(DataController.DetailColumns);
			return columns.ToDictionary(col => col.PropertyDescriptor.Name, col => col.PropertyDescriptor.PropertyType);
		}
		public override bool IsUnboundField(string fieldName) {
			throw new NotImplementedException();
		}
		protected override List<DataColumnInfo> GetSavedColums(List<string> usedFields) {
			throw new NotImplementedException();
		}
		public override int GetChildDataRowCount(int visibleIndex) {
			throw new NotImplementedException();
		}
		protected override void SaveParentGroupRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleStartIndex) {
			throw new NotImplementedException();
		}
		public override bool IsExpressionFitToRule(string expression, int visibleIndex) {
			throw new NotImplementedException();
		}
		protected override List<object> GetFormatConditionsSummary() {
			throw new NotImplementedException();
		}
		public override object GetFormatConditionSummaryValue(FormatConditionSummary item) {
			throw new NotImplementedException();
		}
		public override void Dispose() {
			base.Dispose();
			DataController.Dispose();
			CustomOperationHelper.Dispose();
		}
		protected override PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(DataController.ListSource.Count == 0 && Proxy.DataRowType == null) {
				var isNeedClearDescCollection = true;
				var viewModel = CustomOperationHelper.GridViewModel;
				foreach(var columnState in viewModel.Columns) {
					if(string.IsNullOrEmpty(columnState.FieldName)) continue;
					if(isNeedClearDescCollection) {
						collection.Clear();
						isNeedClearDescCollection = false;
					}
					var propertyDescriptor = new GridViewPropertyDescriptor(columnState.FieldName, columnState.DataType ?? typeof(string));
					collection.Add(propertyDescriptor);
				}
				if(viewModel.Columns[viewModel.KeyFieldName] == null)
					collection.Add(new GridViewPropertyDescriptor(viewModel.KeyFieldName, typeof(string)));
			}
			return base.PatchPropertyDescriptorCollection(collection);
		}
		public override bool AllowSearchPanelFilter(DataColumnInfo column) {
			return true;
		}
		#region IDataControllerData Members
		IWebDataEvents Events { get { return Proxy.Events; } }
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return base.GetUnboundColumns();
		}
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if (Events == null) 
				return value;
			return Events.GetUnboundData(listSourceRow, column.Name, value);
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if (Events != null)
				Events.SetUnboundData(listSourceRow, column.Name, value);
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties {
			get { return true; }
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			return base.GetComplexColumns();
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { 
			get { return false; } 
		}
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			return PatchPropertyDescriptorCollection(collection);
		}
		#endregion
	}
	public abstract class GridBaseCustomOperationHelper : IDisposable {
		GridViewGroupNode[] groupNodesArray;
		public GridBaseCustomOperationHelper(GridBaseViewModel viewModel) {
			GridViewModel = viewModel;
			RootGroupNode = CreateGroupNode(null, null);
			ClientExpandedState = new GridViewExpandedState(this);
			ExpandedState = new GridViewExpandedState(this);
			GroupNodes = new List<GridViewGroupNode>();
			DataRows = new ArrayList();
			DataRowVisibleIndices = new List<int>();
			CurrentVisibleIndex = -1;
			FilteredTotalRowCount = int.MaxValue;
		}
		public GridBaseViewModel GridViewModel { get; set; }
		public GridViewExpandedState ExpandedState { get; set; }
		protected internal GridViewExpandedState ClientExpandedState { get; set; }
		public GridViewGroupNode RootGroupNode { get; protected set; }
		public ReadOnlyCollection<GridViewColumnState> GroupedColumns { get { return GridViewModel.GroupedColumnsInternal; } }
		public abstract IGridSummaryItemStateCollection GroupSummary { get; }
		protected int StartVisibleIndex {
			get {
				var visibleRowCount = GroupedColumns.Count == 0 ? Math.Min(TotalRowCount, FilteredTotalRowCount) : VisibleRowCount;
				var pageCount = (visibleRowCount - 1) / GridViewModel.Pager.PageSize;
				var pageIndex = Math.Min(GridViewModel.Pager.PageIndex, pageCount);
				return Math.Max(0, pageIndex * GridViewModel.Pager.PageSize);
			}
		}
		protected int EndVisibleIndex {
			get {
				if(GridViewModel.Pager.PageIndex < 0)
					return int.MaxValue;
				return StartVisibleIndex + GridViewModel.Pager.PageSize - 1;
			}
		}
		public int CurrentVisibleIndex { get; set; }
		public int VisibleRowCount { get { return CurrentVisibleIndex + 1; } }
		public int TotalRowCount { get; protected set; }
		public int FilteredTotalRowCount { get; protected set; }
		protected List<GridViewGroupNode> GroupNodes { get; set; }
		public ArrayList DataRows { get; set; }
		public List<int> DataRowVisibleIndices { get; set; }
		GridViewGroupNode[] GroupNodesArray {
			get {
				if(groupNodesArray == null)
					groupNodesArray = GroupNodes.ToArray();
				return groupNodesArray;
			}
		}
		public GridViewGroupNode FindGroupNode(int visibleIndex) {
			if(RootGroupNode.ChildNodes.Count == 0)
				return RootGroupNode;
			var index = DataUtils.BinarySearch<GridViewGroupNode, int>(GroupNodesArray, visibleIndex, GroupNodeBinarySearchComparer);
			return index != -1 ? GroupNodesArray[index] : null;
		}
		public WebRowType GetRowType(int visibleIndex) {
			var node = FindGroupNode(visibleIndex);
			if(node != null && node.VisibleIndex == visibleIndex)
				return WebRowType.Group;
			return WebRowType.Data;
		}
		public int GetDataRowIndex(int visibleIndex) {
			return DataRowVisibleIndices.IndexOf(visibleIndex);
		}
		public int GetRowLevel(int visibleIndex) {
			if(visibleIndex < 0) return -1;
			var node = FindGroupNode(visibleIndex);
			if(GetRowType(visibleIndex) == WebRowType.Group)
				return node.Level - 1;
			return node.Level;
		}
		public object GetGroupRowValue(int visibleIndex) {
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return null;
			return FindGroupNode(visibleIndex).Key;
		}
		public bool IsGroupRowFitOnPage(int visibleIndex) {
			var node = FindGroupNode(visibleIndex);
			if(node == null || !node.IsExpanded)
				return true;
			int count = node.ChildNodes.Count > 0 ? node.ChildNodes.Count : node.TotalDataRowCount; 
			return node.VisibleIndex + count <= EndVisibleIndex;
		}
		public bool IsRowExpanded(int visibleIndex) {
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return true;
			var node = FindGroupNode(visibleIndex);
			return node.IsExpanded;
		}
		public bool IsRowLastInLevel(int visibleIndex) {
			var node = FindGroupNode(visibleIndex);
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return visibleIndex == node.VisibleIndex + node.TotalDataRowCount; 
			if(node.Level == 1)
				return false;
			var parentNode = node.ParentNode;
			return parentNode.ChildNodes.IndexOf(node) == parentNode.ChildNodes.Count - 1;
		}
		public List<int> GetParentGroupRows(int visibleIndex) {
			var node = FindGroupNode(visibleIndex);
			if(node == null || node.IsRoot)
				return new List<int>();
			if(GetRowType(visibleIndex) == WebRowType.Group)
				node = node.ParentNode;
			return GetTreeLine(node).Select(i => i.VisibleIndex).ToList();
		}
		public List<int> GetFooterParentGroupRows(int visibleIndex) {
			var result = new List<int>();
			var node = FindGroupNode(visibleIndex);
			if(node == null || node.IsRoot)
				return result;
			if(GetRowType(visibleIndex) == WebRowType.Data) {
				if(!IsRowLastInLevel(visibleIndex))
					return result;
				result.Add(node.VisibleIndex);
			}
			var parent = node.ParentNode;
			while(!node.IsRoot || !parent.IsRoot) {
				if(!IsRowLastInLevel(node.VisibleIndex))
					break;
				result.Add(parent.VisibleIndex);
				node = parent;
				parent = node.ParentNode;
			}
			return result;
		}
		public bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return false;
			var itemState = FindSummaryItem(GroupSummary, item);
			return itemState != null;
		}
		public object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			var itemState = FindSummaryItem(GridViewModel.TotalSummary, item);
			if(itemState == null || RootGroupNode.SummaryValues == null)
				return null;
			var itemIndex = GridViewModel.TotalSummary.IndexOf(itemState);
			return RootGroupNode.SummaryValues[itemIndex];
		}
		public object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			if(!IsGroupSummaryExists(visibleIndex, item))
				return null;
			var itemState = FindSummaryItem(GroupSummary, item);
			var itemIndex = GroupSummary.IndexOf(itemState);
			var node = FindGroupNode(visibleIndex);
			return node.SummaryValues != null ? node.SummaryValues[itemIndex] : null;
		}
		public List<object> GetTotalSummary() {
			return RootGroupNode.SummaryValues.OfType<object>().ToList();
		}
		public List<object> GetGroupSummary(int visibleIndex) {
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return null;
			var node = FindGroupNode(visibleIndex);
			return node.SummaryValues.OfType<object>().ToList();
		}
		public void ProcessCustomBindingCore() {
			if(GridViewCustomOperationCallbackHelper.IsHeaderFilterPopupAction) {
				LoadHeaderFilterValues();
				return;
			}
			LoadClientExpandedState();
			LoadSummary(RootGroupNode);
			LoadTotalRowCount();
			LoadChildNodes(RootGroupNode);
			if(GroupedColumns.Count == 0) {
				var count = Math.Min(TotalRowCount, FilteredTotalRowCount);
				RootGroupNode.TotalDataRowCount = count;
				CurrentVisibleIndex = count - 1;
			}
		}
		void LoadHeaderFilterValues() {
			string fieldName;
			var filterExpression = GridViewCustomOperationCallbackHelper.GetHeaderFilterExpression(GridViewModel, out fieldName);
			var data = GetHeaderFilterValues(fieldName, filterExpression);
			if(data != null) {
				bool requireWrapType = !data.OfType<object>().Any(d => d.GetType().IsClass);
				if(requireWrapType)
					data = data.OfType<object>().Select(d => CreateDynamicObject(fieldName, d));
				DataRows.AddRange(DataUtils.ConvertEnumerableToList(data));
			}
		}
		protected abstract IEnumerable GetHeaderFilterValues(string fieldName, string filterExpression);
		static object CreateDynamicObject(string fieldName, object value) {
			var obj = new ExpandoObject() as IDictionary<string, object>;
			obj[fieldName] = value;
			return obj;
		}
		void LoadClientExpandedState() {
			ClientExpandedState.Load(GridViewModel.ExpandedStateValue);
		}
		void LoadTotalRowCount() {
			TotalRowCount = GetDataRowCount(true);
			if(GroupedColumns.Count > 0 || string.IsNullOrEmpty(GridViewModel.TotalFilterExpression))
				return;
			FilteredTotalRowCount = GetDataRowCount(false);
		}
		protected abstract int GetDataRowCount(bool ignoreFilter);
		void LoadChildNodes(GridViewGroupNode node) {
			if(HasDataRows(node))
				LoadDataRows(node);
			else
				LoadGroups(node);
		}
		bool HasDataRows(GridViewGroupNode node) {
			return node.Level == GroupedColumns.Count;
		}
		void LoadGroups(GridViewGroupNode node) {
			var data = GroupingInfo(node);
			if(data == null)
				return;
			foreach(var info in data)
				AddGroupNode(node, info);
		}
		protected abstract IEnumerable<GridViewGroupInfo> GroupingInfo(GridViewGroupNode node);
		void AddGroupNode(GridViewGroupNode parentNode, GridViewGroupInfo info) {
			var node = CreateGroupNode(info.KeyValue, parentNode);
			parentNode.ChildNodes.Add(node);
			GroupNodes.Add(node);
			if(IsGroupNodeExpanded(node)) {
				ExpandedState.Add(GetGroupInfoList(node));
				if(HasDataRows(node))
					node.TotalDataRowCount = info.DataRowCount;
				LoadChildNodes(node);
			}
			LoadSummary(node);
		}
		protected virtual GridViewGroupNode CreateGroupNode(object key, GridViewGroupNode parentNode) {
			if(parentNode == null)
				return new GridViewGroupNode() { Level = 0, VisibleIndex = -1 };
			return new GridViewGroupNode() {
				Key = key,
				ParentNode = parentNode,
				Level = parentNode.Level + 1,
				VisibleIndex = ++CurrentVisibleIndex
			};
		}
		void LoadDataRows(GridViewGroupNode node) {
			CurrentVisibleIndex += node.TotalDataRowCount;
			var data = GetData(node);
			if(data != null) {
				node.DataRows = DataUtils.ConvertEnumerableToList(data);
				CacheDataRows(node);
			}
		}
		protected abstract IEnumerable GetData(GridViewGroupNode node);
		void CacheDataRows(GridViewGroupNode node) {
			DataRows.AddRange(node.DataRows);
			var start = node.VisibleIndex + node.StartDataRowIndex + 1;
			var count = node.DataRows != null ? node.DataRows.Count : 0;
			var indices = Enumerable.Range(start, count).ToList();
			DataRowVisibleIndices.AddRange(indices);
		}
		void LoadSummary(GridViewGroupNode node) {
			var items = node.IsRoot ? GridViewModel.TotalSummary : GroupSummary;
			if(items.Count == 0 || !IsLoadSummary(node))
				return;
			var data = GetSummaryValue(node, items);
			if(data != null)
				node.SummaryValues = DataUtils.ConvertEnumerableToList(data);
		}
		protected abstract IEnumerable GetSummaryValue(GridViewGroupNode node, IGridSummaryItemStateCollection items);
		protected bool IsLoadSummary(GridViewGroupNode node) {
			if(node.VisibleIndex < StartVisibleIndex && CurrentVisibleIndex >= StartVisibleIndex)
				return true;
			return StartVisibleIndex <= node.VisibleIndex && EndVisibleIndex >= node.VisibleIndex || node.IsRoot;
		}
		protected bool IsGroupNodeExpanded(GridViewGroupNode node) {
			var info = GridViewModel.ExpandCollapseInfo;
			if(info != null) {
				if(info.RowIndex == -1 || node.VisibleIndex == info.RowIndex)
					return info.Expanded;
				if(info.Recursive) {
					var treeLine = GetTreeLine(node);
					treeLine.Add(RootGroupNode);
					return treeLine.Where(n => n.VisibleIndex == info.RowIndex).FirstOrDefault() != null;
				}
			}
			return ClientExpandedState.GetIsExpanded(GetGroupInfoList(node));
		}
		protected List<GridViewGroupInfo> GetGroupInfoList(GridViewGroupNode node) {
			return GetTreeLine(node).Select(i => CreateGroupInfo(i)).ToList();
		}
		List<GridViewGroupNode> GetTreeLine(GridViewGroupNode node) {
			List<GridViewGroupNode> list = new List<GridViewGroupNode>();
			while(!node.IsRoot) {
				list.Add(node);
				node = node.ParentNode;
			}
			list.Reverse();
			return list;
		}
		GridViewGroupInfo CreateGroupInfo(GridViewGroupNode node) {
			return new GridViewGroupInfo() {
				KeyValue = node.Key,
				FieldName = GroupedColumns[node.Level - 1].FieldName
			};
		}
		GridBaseSummaryItemState FindSummaryItem(IGridSummaryItemStateCollection collection, ASPxSummaryItemBase item) {
			return collection.Where(i => i.FieldName == item.FieldName && i.SummaryType == item.SummaryType && i.Tag == item.Tag).SingleOrDefault();
		}
		int GroupNodeBinarySearchComparer(GridViewGroupNode node, int visibleIndex) {
			int start = node.VisibleIndex;
			int end = node.ChildNodes.Count > 0 ? start : start + node.TotalDataRowCount;
			if(visibleIndex < start)
				return 1;
			if(visibleIndex > end)
				return -1;
			return 0;
		}
		#region IDisposable Members
		public void Dispose() {
			RootGroupNode.ChildNodes.Clear();
			RootGroupNode = null;
			GroupNodes.Clear();
			GroupNodes = null;
			this.groupNodesArray = null;
			DataRows.Clear();
			DataRows = null;
		}
		#endregion
	}
	public class GridViewExpandedState {
		class ExpandedStateNode {
			public ExpandedStateNode() {
				ChildNodes = new List<ExpandedStateNode>();
			}
			public object Key { get; set; }
			public List<ExpandedStateNode> ChildNodes { get; protected set; }
		}
		GridBaseCustomOperationHelper helper;
		ExpandedStateNode rootNode;
		int groupedColumnCount = -1;
		internal GridViewExpandedState() {
			this.rootNode = new ExpandedStateNode();
		}
		public GridViewExpandedState(GridBaseCustomOperationHelper helper)
			: this() {
			this.helper = helper;
			GroupedColumnIndeces = new int[0];
		}
		protected GridBaseCustomOperationHelper Helper { get { return helper; } }
		protected GridBaseViewModel GridViewModel { get { return Helper.GridViewModel; } }
		protected int[] GroupedColumnIndeces { get; set; }
		ExpandedStateNode RootNode { get { return rootNode; } }
		public void Add(List<GridViewGroupInfo> infoList) {
			var node = RootNode;
			foreach(var info in infoList) {
				var findedNode = node.ChildNodes.Where(c => object.Equals(c.Key, info.KeyValue)).SingleOrDefault();
				if(findedNode == null) {
					findedNode = new ExpandedStateNode() { Key = info.KeyValue };
					node.ChildNodes.Add(findedNode);
				}
				node = findedNode;
			}
		}
		public bool GetIsExpanded(List<GridViewGroupInfo> groupInfoList) {
			if(groupInfoList.Count > GroupedColumnCount)
				return false;
			var node = RootNode;
			for(var i = 0; i < groupInfoList.Count; i++) {
				var groupInfo = groupInfoList[i];
				node = node.ChildNodes.Where(n => object.Equals(n.Key, groupInfo.KeyValue)).FirstOrDefault();
				if(node == null)
					return false;
			}
			return true;
		}
		protected int GroupedColumnCount {
			get {
				if(groupedColumnCount == -1)
					groupedColumnCount = GetGroupedColumnCount();
				return groupedColumnCount;
			}
		}
		protected virtual int[] ActualGroupedColumnIndeces {
			get { return Helper.GroupedColumns.Select(c => c.Index).ToArray(); }
		}
		int GetGroupedColumnCount() {
			var count = Math.Min(ActualGroupedColumnIndeces.Length, GroupedColumnIndeces.Length);
			for(var i = 0; i < count; i++) {
				if(ActualGroupedColumnIndeces[i] != GroupedColumnIndeces[i])
					return i;
			}
			return count;
		}
		public string Save() {
			using(MemoryStream stream = new MemoryStream()) {
				using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
					SaveCore(writer);
					return Convert.ToBase64String(stream.ToArray());
				}
			}
		}
		void SaveCore(TypedBinaryWriter writer) {
			writer.WriteObject(ActualGroupedColumnIndeces.Length);
			if(ActualGroupedColumnIndeces.Length == 0) return;
			foreach(var index in ActualGroupedColumnIndeces)
				writer.WriteObject(index);
			SaveChildNodes(writer, RootNode.ChildNodes);
		}
		void SaveChildNodes(TypedBinaryWriter writer, List<ExpandedStateNode> nodes) {
			writer.WriteObject(nodes.Count);
			foreach(var node in nodes) {
				writer.WriteTypedObject(node.Key);
				SaveChildNodes(writer, node.ChildNodes);
			}
		}
		public void Load(string value) {
			if(string.IsNullOrEmpty(value))
				return;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				using(TypedBinaryReader reader = new TypedBinaryReader(stream))
					LoadCore(reader);
			}
		}
		void LoadCore(TypedBinaryReader reader) {
			var groupedColumnsCount = reader.ReadObject<int>();
			if(groupedColumnsCount == 0) return;
			GroupedColumnIndeces = new int[groupedColumnsCount];
			for(int i = 0; i < groupedColumnsCount; i++)
				GroupedColumnIndeces[i] = reader.ReadObject<int>();
			LoadChildNodes(reader, RootNode);
		}
		void LoadChildNodes(TypedBinaryReader reader, ExpandedStateNode node) {
			var count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				var child = new ExpandedStateNode();
				child.Key = reader.ReadTypedObject();
				node.ChildNodes.Add(child);
				LoadChildNodes(reader, child);
			}
		}
	}
	public class GridViewGroupNode {
		public GridViewGroupNode() {
			ChildNodes = new List<GridViewGroupNode>();
		}
		public object Key { get; set; }
		public int Level { get; set; }
		public GridViewGroupNode ParentNode { get; set; }
		public List<GridViewGroupNode> ChildNodes { get; protected set; }
		public bool IsRoot { get { return Level == 0; } }
		public int VisibleIndex { get; set; }
		public bool IsExpanded { get { return ChildNodes.Count > 0 || TotalDataRowCount > 0; } }
		public int TotalDataRowCount { get; set; }
		public IList DataRows { get; set; }
		public int StartDataRowIndex { get; set; }
		public IList SummaryValues { get; set; }
	}
}
