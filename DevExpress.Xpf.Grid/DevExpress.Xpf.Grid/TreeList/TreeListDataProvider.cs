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
using System.ComponentModel;
using System.Windows;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections.Specialized;
using System.Linq;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#else
using System.Windows.Forms;
using DevExpress.Xpf.GridData;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListRowState : DependencyObject { }
	public class TreeListDataProvider : GridDataProviderBase, IEvaluatorDataAccess {
		protected internal class SummaryItem : Dictionary<SummaryItemBase, TreeListSummaryValue> { }
		readonly TreeListView view;
		protected DataColumnInfoCollection columnsCollection;
		int currentControllerRow = GridControl.InvalidRowHandle;
		bool nodesCountIsActual;
		int nodesCount;
		Locker dataUpdateLocker = new Locker();
		CriteriaOperator filterCriteria;
		internal ExpressionEvaluator filterExpressionEvaluator;
		ISelectionController selectionController;
		internal const string CheckBoxExceptionMessage = "CheckBoxFieldName should correspond to a NullableBoolean or Boolean field in a data source.";
		public TreeListNodeCollection Nodes { get { return RootNode.Nodes; } }
		public TreeListView View { get { return view; } }
		public override int CurrentControllerRow {
			get { return currentControllerRow; }
			set {
				if(!IsValidRowHandle(value) && value != GridControl.InvalidRowHandle)
					return;
				if(CurrentControllerRow == value) return;
				currentControllerRow = value;
				OnCurrentControllerRowChanged();
			}
		}
		public TreeListNode CurrentNode { get { return GetNodeByRowHandle(CurrentControllerRow); } }
		public override int CurrentIndex { get { return GetRowVisibleIndexByHandle(CurrentControllerRow); } }
		readonly ValueComparer valueComparer;
		public override ValueComparer ValueComparer { get { return valueComparer; } }
		internal TreeListNode RootNode { get; set; }
		protected override Type ItemTypeCore { get { return DataHelper.ItemType; } }
		protected internal TreeListDataHelperBase DataHelper { get; private set; }
		protected TreeListFilterHelper FilterHelper { get; private set; }
		protected internal TreeListNodesInfo NodesInfo { get; private set; }
#if SL
		protected TreeListNodeValuesCache ValuesCache { get; private set; }
#endif
		protected TreeListNodeComparer NodesComparer { get; private set; }
		protected Dictionary<TreeListNode, SummaryItem> SummaryData { get; private set; }
		public TreeListDataProvider(TreeListView view) {
			this.view = view;
			this.selectionController = new TreeListSelectionController(this);
			RootNode = new RootTreeListNode(this);
			SummaryData = new Dictionary<TreeListNode, SummaryItem>();
			FilterHelper = CreateFilterHelper();
			valueComparer = CreateValueComparer();
			NodesComparer = CreateNodesComparer();
			columnsCollection = new DataColumnInfoCollection();
			NodesInfo = new TreeListNodesInfo(this);
			NodesState = new TreeListNodesState(this);
#if SL
			ValuesCache = new TreeListNodeValuesCache(this);
#endif
			UpdateDataHelper();
		}
		public virtual int TotalNodesCount {
			get {
				if(!nodesCountIsActual) {
					nodesCount = GetTotalNodesCount();
					nodesCountIsActual = true;
				}
				return nodesCount;
			}
		}
		private int GetTotalNodesCount() {
			int count = 0;
			foreach(TreeListNode node in new TreeListNodeIterator(RootNode.Nodes))
				count++;
			return count;
		}
		public override bool IsUpdateLocked {
			get { return dataUpdateLocker.IsLocked; }
		}
		public override ISummaryItemOwner TotalSummaryCore {
			get {
				if(View.DataControl == null)
					return null;
				return View.DataControl.TotalSummaryCore;
			}
		}
		public override int VisibleCount { get { return NodesInfo.TotalVisibleNodesCount; } }
		public override int DataRowCount { get { return NodesInfo.TotalNodesCount; } }
		public int MaxVisibleLevel { get; protected set; }
		int maxLevelCore = -1;
		public int MaxLevel {
			get {
				if(maxLevelCore == -1)
					maxLevelCore = CalcMaxNodeLevel(false);
				return maxLevelCore; 
			}
		}
		public bool CanUseFastPropertyDescriptors { get { return View.TreeDerivationMode == TreeDerivationMode.Selfreference && !View.IsDesignTime; } }
		protected bool IsSorted { get; private set; }
		protected bool IsFiltered { get; private set; }
		IValidationAttributeOwner ValidationOwner { get { return View.DataControl as IValidationAttributeOwner; } }
		public override int GetRowLevelByControllerRow(int rowHandle) {
			if(!IsValidRowHandle(rowHandle))
				return -1;
			return GetRowLevelByControllerRowCore(rowHandle);
		}
		public override int GetActualRowLevel(int rowHandle, int level) {
			return level;
		}
		protected internal int GetRowLevelByControllerRowCore(int rowHandle) {
			return GetRowLevelByControllerRowCore(rowHandle, true);
		}
		protected internal int GetRowLevelByControllerRowCore(int rowHandle, bool actualLevel) {
			TreeListNode node = NodesInfo.GetNodeByRowHandle(rowHandle);
			return node == null ? -1 : (actualLevel ? node.ActualLevel : node.Level);
		}
		public override int GetRowVisibleIndexByHandle(int rowHandle) {
			if(!IsValidRowHandle(rowHandle))
				return -1;
			TreeListNode node = NodesInfo.GetNodeByRowHandle(rowHandle);
			return NodesInfo.GetVisibleIndexByNode(node); 
		}
		public override bool IsValidRowHandle(int rowHandle) {
			return 0 <= rowHandle && rowHandle < NodesInfo.TotalNodesCount;
		}
		internal bool IsValidVisibleIndex(int visibleIndex) {
			return 0 <= visibleIndex && visibleIndex < NodesInfo.TotalVisibleNodesCount;
		}
		public override object GetRowValue(int rowHandle) {
			TreeListNode node = GetNodeByRowHandle(rowHandle);
			if(node != null)
				return node.Content;
			return null;
		}
		public TreeListNode GetNodeByRowHandle(int rowHandle) {
			return IsValidRowHandle(rowHandle) ? NodesInfo.GetNodeByRowHandle(rowHandle) : null;
		}
		public TreeListNode GetNodeByVisibleIndex(int visibleIndex) {
			if(!IsValidVisibleIndex(visibleIndex))
				return null;
			return NodesInfo.GetNodeByVisibleIndex(visibleIndex);
		}
		public int GetVisibleIndexByNode(TreeListNode node) { 
			if(node == null) return -1;
			return NodesInfo.GetVisibleIndexByNode(node);
		}
		public override object GetNodeIdentifier(int rowHandle) {
			return GetNodeByRowHandle(rowHandle);
		}
		public int GetRowHandleByNode(TreeListNode node) {
			if(node == null) return GridControl.InvalidRowHandle;
			return NodesInfo.GetRowHandleByNode(node);
		}
		public override object GetRowValue(int rowHandle, string fieldName) {
			TreeListNode node = GetNodeByRowHandle(rowHandle);
			if(node != null)
				return DataHelper.GetValue(node, fieldName);
			return null;
		}
		public virtual object GetNodeValue(TreeListNode node, string fieldName) {
			return DataHelper.GetValue(node, fieldName);
		}
		public override object GetRowValue(int rowHandle, DevExpress.Data.DataColumnInfo info) {
			return DataHelper.GetValue(GetNodeByRowHandle(rowHandle), info);
		}
		public override int GetControllerRow(int visibleIndex) {
			if(!IsValidVisibleIndex(visibleIndex))
				return GridControl.InvalidRowHandle;
			return NodesInfo.GetRowHandleByNode(NodesInfo.GetNodeByVisibleIndex(visibleIndex));
		}
		public override bool IsRowVisible(int rowHandle) {
			if(!IsValidRowHandle(rowHandle))
				return false;
			TreeListNode node = NodesInfo.GetNodeByRowHandle(rowHandle);
			return NodesInfo.GetVisibleIndexByNode(node) > -1;  
		}
		public override int GetChildRowCount(int rowHandle) {
			if(!IsValidRowHandle(rowHandle))
				return -1;
			TreeListNode node = NodesInfo.GetNodeByRowHandle(rowHandle);
			return node.Nodes.Count;
		}
		public override int GetChildRowHandle(int rowHandle, int childIndex) {
			if(!IsValidRowHandle(rowHandle))
				return GridControl.InvalidRowHandle;
			TreeListNode node = NodesInfo.GetNodeByRowHandle(rowHandle);
			if(childIndex < 0 || childIndex >= node.Nodes.Count)
				return GridControl.InvalidRowHandle;
			return NodesInfo.GetRowHandleByNode(node.Nodes[childIndex]);
		}
		public override void Syncronize(IList<GridSortInfo> sortList, int groupCount, CriteriaOperator filterCriteria) {
			if(View.DataControl == null)
				return;
			BeginUpdateCore();
			try {
				try {
					FilterCriteria = filterCriteria;
				}
				catch {
				}
			}
			finally {
				EndUpdateCore();
			}
		}
		public virtual void ResetCurrentPosition() {
			if(View.FocusedRowHandleChangedLocker.IsLocked || (View.DataControl != null && View.DataControl.CurrentItemChangedLocker.IsLocked)
				|| IsUpdateLocked) return;
			if(CurrentControllerRow == GridControl.InvalidRowHandle)
				CurrentControllerRow = Nodes.Count > 0 ? 0 : GridControl.InvalidRowHandle;
			if(view.IsMultiSelection && CurrentControllerRow != GridControl.InvalidRowHandle && View.DataControl.AllowUpdateSelectedItems) {
				if(TreeListSelection.Count == 0 && (View.DataControl.isSync || View.DataControl.SelectedItems == null)) {
					TreeListSelection.BeginSelection();
					try {
						TreeListSelection.SetSelected(CurrentControllerRow, true);
					}
					finally {
						if(TreeListSelection.GetSelected(CurrentControllerRow))
							TreeListSelection.EndSelection();
						else TreeListSelection.CancelSelection();
					}
				}
			}
		}
		#region sorting
		protected virtual void DoSortNodes(IList<GridSortInfo> sortList, TreeListNode parentNode) {
			if(!DataHelper.IsReady || IsUpdateLocked) return;
			view.OnStartedSort();
			try {
				if(!View.AutoScrollOnSorting)
					View.ScrollIntoViewLocker.Lock();
				TreeListNode currentNode = CurrentNode;
				if(!IsSorted && sortList.Count == 0) return;
				NodesComparer.SetSortInfo(sortList);
				View.DataControl.SynchronizeSortInfo(GetSortInfo(sortList), 0);
				SortNodes(parentNode);
				IsSorted = sortList.Count > 0;
				if(currentNode != null && NodesState.FocusedRow == null && !View.FocusedRowHandleChangedLocker.IsLocked)
					CurrentControllerRow = GetRowHandleByNode(currentNode);
			} finally {
				if(!View.AutoScrollOnSorting)
					View.ScrollIntoViewLocker.Unlock();
				view.OnEndedSort();
			}
		}
		List<IColumnInfo> GetSortInfo(IList<GridSortInfo> sortList) {
			List<IColumnInfo> res = new List<IColumnInfo>();
			foreach(GridSortInfo info in sortList) {
				res.Add(new DummyColumnInfo(info.FieldName, info.GetSortOrder()));
			}
			return res;
		}
		protected virtual void SortNodes(TreeListNode parentNode){
			SortNodes(parentNode.Nodes);
		}
		protected internal virtual void SortNodes(TreeListNodeCollection nodes, bool recursive = true) {
			NodesInfo.SetDirty();
			nodes.DoSort(NodesComparer);
			if(recursive)
				foreach(TreeListNode node in nodes)
					SortNodes(node.Nodes);
		}
		#endregion
		#region summaries
		public override void SynchronizeSummary() {
			if(IsUpdateLocked) return;
			UpdateTotalSummary();
		}
		public override void UpdateTotalSummary() {
			if(TotalSummaryCore == null)
				return;
			CalcSummary(TotalSummaryCore.Concat(View.ViewBehavior.GetServiceSummaries()));
		}
		protected internal virtual bool CanCalculateSummary(IEnumerable<SummaryItemBase> summary) {
			if(!summary.Any()) return false;
			bool canCalculate = false;
			foreach(SummaryItemBase item in summary) {
				if(item.SummaryType == SummaryItemType.Custom)
					canCalculate = View.HasCustomSummary || item is ServiceSummaryItem;
				else 
					canCalculate = (item.SummaryType != SummaryItemType.None);
				if(canCalculate) 
					break;
			}
			return canCalculate;
		}
		protected virtual void CalcSummary(IEnumerable<SummaryItemBase> summary) {
			CalcSummaryCore(summary, SummaryData, false);
		}
		protected internal virtual void CalcSummaryCore(IEnumerable<SummaryItemBase> summary, Dictionary<TreeListNode, SummaryItem> summaryData, bool calcOnlySelectedItems) {
			summaryData.Clear();
			if(!RootNode.HasChildren || !CanCalculateSummary(summary)) return;
			foreach(TreeListNode node in new TreeListNodeIterator(RootNode)) {
				if(!node.IsVisible || (calcOnlySelectedItems && !View.GetSelectedRowHandlesCore().Contains(node.RowHandle)))
					continue;
				foreach(SummaryItemBase item in summary) {
					if(!CanCalculateSummaryItem(item))
						continue;
					UpdateSummaryValue(node, summaryData, item, node);
				}
			}
			foreach(TreeListNode node in summaryData.Keys) {
				SummaryItem summaryItem = summaryData[node];
				foreach(TreeListSummaryValue summaryValue in summaryItem.Values)
					summaryValue.Finish(RootTreeListNode.IsRootNode(node) ? null : node);
			}
		}
		protected virtual bool CanCalculateSummaryItem(SummaryItemBase item) {
			if(item.SummaryType == SummaryItemType.None) return false;
			ServiceSummaryItem serviceItem = item as ServiceSummaryItem;
			if(serviceItem != null && serviceItem.CustomServiceSummaryItemType == CustomServiceSummaryItemType.SortedList) {
				DataColumnInfo columnInfo = Columns[serviceItem.FieldName];
				return columnInfo != null && (typeof(IComparable).IsAssignableFrom(columnInfo.GetDataType()) || IsUnboundWithExpression(columnInfo));
			}
			return true;
		}
		protected bool IsUnboundWithExpression(DataColumnInfo ci) {
			TreeListUnboundPropertyDescriptor unboundPropertyDescriptor = ci.PropertyDescriptor as TreeListUnboundPropertyDescriptor;
			if(unboundPropertyDescriptor == null)
				return false;
			return !string.IsNullOrEmpty(unboundPropertyDescriptor.UnboundInfo.Expression);
		}
		protected void UpdateSummaryValue(TreeListNode summaryOwner, Dictionary<TreeListNode, SummaryItem> summaryData, SummaryItemBase item, TreeListNode node) {
			bool isRootNode = RootTreeListNode.IsRootNode(summaryOwner);
			TreeListNode parent = isRootNode ? summaryOwner : summaryOwner.parentNodeCore;
			if(parent == null)
				return;
			if(!summaryData.ContainsKey(parent))
				summaryData[parent] = new SummaryItem();
			TreeListSummaryValue value;
			if(!summaryData[parent].ContainsKey(item)) {
				value = CreateSummaryValue(item);
				value.Start(isRootNode ? null : parent);
				summaryData[parent][item] = value;
			}
			else {
				value = summaryData[parent][item];
			}
			if(!isRootNode)
				value.Calculate(node, item.IgnoreNullValues.GetValueOrDefault(View.SummariesIgnoreNullValues));
			if(TreeListSummarySettings.GetIsRecursive(item) && !isRootNode)
				UpdateSummaryValue(parent, summaryData, item, node);
		}
		public override object GetTotalSummaryValue(SummaryItemBase item) {
			return GetSummaryValue(RootNode, item);
		}
		internal virtual object GetSummaryValueCore(TreeListNode node, SummaryItemBase item, Dictionary<TreeListNode, SummaryItem> summaryData) {
			if(!summaryData.ContainsKey(node))
				return null;
			if(!summaryData[node].ContainsKey(item))
				return null;
			return summaryData[node][item].Value;
		}
		public object GetSummaryValue(TreeListNode node, SummaryItemBase item) {
			return GetSummaryValueCore(node, item, SummaryData);
		}
		internal SummaryItem GetRootSummaryItem() {
			SummaryItem rootItem = null;
			SummaryData.TryGetValue(RootNode, out rootItem);
			return rootItem;
		}
		protected TreeListSummaryValue CreateSummaryValue(SummaryItemBase item) {
			switch(item.SummaryType) {
				case SummaryItemType.Count:
					return new TreeListSummaryCountValue(item);
				case SummaryItemType.Min:
					return new TreeListSummaryMinValue(item);
				case SummaryItemType.Max:
					return new TreeListSummaryMaxValue(item);
				case SummaryItemType.Sum:
					return new TreeListSummarySumValue(item);
				case SummaryItemType.Average:
					return new TreeListSummaryAvgValue(item);
				case SummaryItemType.Custom:
					ServiceSummaryItem serviceItem = item as ServiceSummaryItem;
					if(serviceItem != null) {
						if(serviceItem.CustomServiceSummaryItemType == CustomServiceSummaryItemType.SortedList)
							return new TreeListSummarySortedList(serviceItem);
						if(serviceItem.CustomServiceSummaryItemType == CustomServiceSummaryItemType.DateTimeAverage)
							return new TreeListSummaryDateTimeAvarage(serviceItem);
					}
					return new TreeListSummaryCustomValue(item, View);
				default:
					throw new ArgumentException();
			}
		}
		#endregion
		#region editing
		bool currentRowEditing = false;
		public override bool EndCurrentRowEdit() {
			if(!IsReady || !IsCurrentRowEditing) return true;
			TreeListNode currentNode = CurrentNode;
			if(currentNode == null)
				return true;
			try {
				if(!View.RaiseValidateNode(CurrentControllerRow, currentNode.Content)) return false;
				EndDataRowEdit(currentNode);
			}
			catch(Exception e) {
				ControllerRowExceptionEventArgs args = new ControllerRowExceptionEventArgs(CurrentControllerRow, currentNode.Content, e);
				View.RaiseInvalidNodeException(currentNode, args);
				if(args.Action == ExceptionAction.CancelAction) {
					CancelDataRowEdit(CurrentNode);
					return true;
				}
				return false;
			}
			StopCurrentRowEdit();
			OnEndCurrentRowEdit();
			return true;
		}
		public override void BeginCurrentRowEdit() {
			if(!IsReady || CurrentNode == null) return;
			if(!IsCurrentRowEditing) {
				BeginDataRowEdit(CurrentNode);
				currentRowEditing = true;
			}
		}
		public override void CancelCurrentRowEdit() {
			if(!IsReady || CurrentNode == null) return;
			if(IsCurrentRowEditing) {
				CancelDataRowEdit(CurrentNode);
				StopCurrentRowEdit();
				OnEndCurrentRowEdit();
			}
		}
		protected virtual void OnEndCurrentRowEdit() {
			if(!IsCurrentRowEditing && !IsUpdateLocked) {
				TreeListNode node = GetNodeByRowHandle(CurrentControllerRow);
				if(node != null) {
					UpdateNodeInternal(node, false);
					DoRefreshRow(node);
				}
			}
		}
		public override void SetRowValue(RowHandle rowHandle, DevExpress.Data.DataColumnInfo info, object value) {
			if(rowHandle.Value == CurrentControllerRow)
				BeginCurrentRowEdit();
			TreeListNode node = GetNodeByRowHandle(rowHandle.Value);
			DataHelper.SetValue(node, info.Name, value);
			if(!DataHelper.SupportNotifications)
				OnNodeCollectionChanged(node, NodeChangeType.Content);
		}
		public virtual void SetNodeValue(TreeListNode node, string fieldName, object value) {
			if(node == CurrentNode)
				BeginCurrentRowEdit();
			DataHelper.SetValue(node, fieldName, value);
		}
		protected void BeginDataRowEdit(TreeListNode node) {
			IEditableObject editable = GetEditableObject(node);
			if(editable != null) {
				editable.BeginEdit();
			}
#if SL
			else {
				ValuesCache.SaveValues();
			}
#endif
		}
		protected void CancelDataRowEdit(TreeListNode node) {
			IEditableObject editable = GetEditableObject(node);
			if(editable != null) {
				editable.CancelEdit();
			}
#if SL
			else {
				ValuesCache.RestoreValues();
			}
#endif
		}
		protected void EndDataRowEdit(TreeListNode node) {
			IEditableObject editable = GetEditableObject(node);
			if(editable != null)
				editable.EndEdit();
		}
		protected virtual void StopCurrentRowEdit() {
			currentRowEditing = false;
		}
		public override bool IsCurrentRowEditing { get { return currentRowEditing; } }
		protected IEditableObject GetEditableObject(TreeListNode node) {
			if(node != null)
				return node.Content as IEditableObject;
			return null;
		}
		public override bool AllowEdit { get { return DataHelper.AllowEdit; } }
		public bool AllowRemove { get { return DataHelper.AllowRemove; } }
		#endregion
		protected virtual void OnCurrentControllerRowChanged() {
			View.OnCurrentIndexChanged();
		}
		public override void RefreshRow(int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return;
			View.UpdateRowDataByRowHandle(rowHandle, (rowData) => rowData.UpdateData());
		}
		public void ReloadChildNodes(TreeListNode node) {
			if(node == null || !ReferenceEquals(node.DataProvider, this))
				return;
			DataHelper.ReloadChildNodes(node);
		}
		public override bool AutoExpandAllGroups { get; set; }
		Dictionary<int, TreeListRowState> rowStates = new Dictionary<int, TreeListRowState>();
		public override DependencyObject GetRowState(int controllerRow, bool createNewIfNotExist) {
			if(rowStates.ContainsKey(controllerRow))
				return rowStates[controllerRow];
			if(!createNewIfNotExist) return null;
			rowStates[controllerRow] = new TreeListRowState();
			return rowStates[controllerRow];
		}
		public override RowDetailContainer GetRowDetailContainer(int controllerRow, Func<RowDetailContainer> createContainerDelegate, bool createNewIfNotExist) {
			throw new NotSupportedException(); 
		}
		public override ErrorInfo GetErrorInfo(RowHandle rowHandle, string fieldName) {
			ErrorInfo info = new ErrorInfo();
			DataColumnInfo ci = Columns[fieldName];
			if(ci == null || ci.PropertyDescriptor is TreeListUnboundPropertyDescriptor) return info;
			TreeListNode node = GetNodeByRowHandle(rowHandle.Value);
			if(node == null) return info;
			IDataErrorInfo errorInfo = GetRowErrorInfo(node.Content);
			IDXDataErrorInfo dxErrorInfo = GetRowDXErrorInfo(node.Content);
			if(errorInfo != null && dxErrorInfo == null) {
				string error = errorInfo[ci.Name];
				if(!string.IsNullOrEmpty(error))
					info.ErrorText = error;
			}
			if(dxErrorInfo != null)
				dxErrorInfo.GetPropertyError(ci.Name, info);
			if(ValidationOwner != null && !ValidationOwner.CalculateValidationAttribute(ci.Name, rowHandle.Value))
				return info;
			string errorText = GetValidationAttributesErrorText(rowHandle.Value, ci);
			if(!string.IsNullOrEmpty(errorText))
				info.ErrorText = errorText;
			return info;
		}
		protected override object GetRowValueForValidationAttribute(int controllerRow, string columnName) {
			return GetRowValue(controllerRow, columnName);
		}
		public override ErrorInfo GetErrorInfo(RowHandle rowHandle) {
			ErrorInfo info = new ErrorInfo();
			TreeListNode node = GetNodeByRowHandle(rowHandle.Value);
			if(node == null)
				return info;
			IDataErrorInfo errorInfo = GetRowErrorInfo(node.Content);
			IDXDataErrorInfo dxErrorInfo = GetRowDXErrorInfo(node.Content);
			if(errorInfo != null && dxErrorInfo == null) {
				string error = errorInfo.Error;
				if(!string.IsNullOrEmpty(error))
					info.ErrorText = error;
			}
			if(dxErrorInfo != null)
				dxErrorInfo.GetError(info);
			return info;
		}
		protected IDataErrorInfo GetRowErrorInfo(object row) {
			return row as IDataErrorInfo;
		}
		protected IDXDataErrorInfo GetRowDXErrorInfo(object row) {
			return row as IDXDataErrorInfo;
		}
		protected internal bool IsRecursiveNodesUpdateLocked { get; private set; }
		protected internal void LockRecursiveNodesUpdate() {
			IsRecursiveNodesUpdateLocked = true;
		}
		protected internal void UnlockRecursiveNodesUpdate() {
			IsRecursiveNodesUpdateLocked = false;
		}
		public override ISelectionController Selection { get { return selectionController; } }
		public TreeListSelectionController TreeListSelection { get { return (TreeListSelectionController)Selection; } }
		public override bool IsReady {
			get { return DataHelper.IsReady; }
		}
		public override DevExpress.Data.Filtering.CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				if(Equals(FilterCriteria, value))
					return;
				CriteriaOperator oldCriteria = FilterCriteria;
				try {
					filterCriteria = CriteriaOperator.Clone(value);
					OnFilterExpressionChanged();
				}
				catch {
					filterCriteria = oldCriteria;
					OnFilterExpressionChanged();
				}
			}
		}
		protected ExpressionEvaluator FilterExpressionEvaluator {
			get {
				if(filterExpressionEvaluator == null) {
					if(!ReferenceEquals(FilterCriteria, null)) {
						Exception e;
						this.filterExpressionEvaluator = CreateExpressionEvaluator(FilterCriteria, out e);
					}
				}
				return filterExpressionEvaluator;
			}
		}
		public override DataColumnInfoCollection Columns { get { return columnsCollection; } }
		public override DataColumnInfoCollection DetailColumns { get { return new DataColumnInfoCollection(); } }
		public bool IsUnboundMode { get { return DataHelper.IsUnboundMode; } }
		protected internal override DevExpress.Data.BaseGridController DataController { get { return null; } }
		public override bool CanColumnSortCore(string fieldName) {
			return Columns[fieldName] != null;
		}
		public override bool IsGroupRowHandle(int rowHandle) {
			return false;
		}
		public override void MakeRowVisible(int rowHandle) {
			TreeListNode node = GetNodeByRowHandle(rowHandle);
			if(node == null) return;
			TreeListNode parentNode = node.ParentNode;
			while(parentNode != null) {
				parentNode.IsExpanded = true;
				parentNode = parentNode.ParentNode;
			}
		}
		public override void BeginUpdate() {
			Selection.BeginSelection();
			SaveNodesState();
			BeginUpdateCore();
		}
		internal void SaveNodesState(bool supressLocker = false) {
			NodesState.SaveNodesState(supressLocker);
			NodesState.SaveCurrentFocus(supressLocker);
		}
		public override void EndUpdate() {
			if(DataHelper.RequiresReloadDataOnEndUpdate)
				DataHelper.LoadData();
			UpdateColumnsUnboundTypeIfNeeded();
			RestoreNodesState();
			EndUpdateCore();
			Selection.EndSelection();
		}
		internal void RestoreNodesState(bool supressLocker = false) {
			NodesState.RestoreNodesState(supressLocker);
			NodesState.RestoreCurrentFocus();
		}
		public void CancelUpdate() {
			dataUpdateLocker.Unlock();
		}
		public override void RePopulateColumns() {
			Columns.Clear();
			ResetValidationAttributes();
			DataHelper.PopulateColumns();
		}
		public virtual ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out Exception e) {
			e = null;
			if(!IsReady || Columns.Count == 0) return null;
			try {
				ExpressionEvaluator evaluator = new ExpressionEvaluator(GetFilterDescriptorCollection(), criteriaOperator, false); 
				evaluator.DataAccess = this;
				return evaluator;
			}
			catch(Exception ex) {
				e = ex;
				return null;
			}
		}
		public override void DoRefresh() {
			DoRefresh(true);
		}
		protected internal void DoRefresh(bool keepNodesState) {
			if(!IsReady) return;
			if(keepNodesState)
				SaveNodesState();
			BeginUpdateCore();
			try {
				DataHelper.LoadData();
			}
			finally {
				if(keepNodesState)
					RestoreNodesState(); 
				EndUpdateCore();
			}
		}
		protected TreeListNodesState NodesState { get; private set; }
		public override int GetParentRowHandle(int rowHandle) {
			TreeListNode node = GetNodeByRowHandle(rowHandle);
			if(node != null) {
				TreeListNode parentNode = node.VisibleParent;
				if(parentNode != null)
					return NodesInfo.GetRowHandleByNode(parentNode);
			}
			return DevExpress.Data.DataController.InvalidRow;
		}
		public override object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime, bool implyNullLikeEmptyStringWhenFiltering) {
			DataColumnInfo columnInfo = Columns[column.FieldName];
			if(columnInfo != null)
				return FilterHelper.GetUniqueColumnValuesCore(columnInfo, includeFilteredOut, roundDataTime, column.ColumnFilterMode == ColumnFilterMode.DisplayText);
			return null;
		}
		public override DevExpress.Data.Filtering.CriteriaOperator CalcColumnFilterCriteriaByValue(ColumnBase column, object columnValue) {
			DataColumnInfo columnInfo = Columns[column.FieldName];
			if(columnInfo != null) {
				bool roundDateTime = (column.ActualEditSettings is DevExpress.Xpf.Editors.Settings.DateEditSettings) || column.RoundDateTimeForColumnFilter;
				return FilterHelper.CalcColumnFilterCriteriaByValue(columnInfo, columnValue, roundDateTime, column.ColumnFilterMode == ColumnFilterMode.DisplayText);
			}
			return null;
		}
		public virtual TreeListNode FindNodeByValue(object value) {
			if(!IsReady) return null;
			foreach(TreeListNode node in new TreeListNodeIterator(Nodes)) {
				if(Object.Equals(value, node.Content))
					return node;
			}
			return null;
		}
		public virtual IList<TreeListNode> FindNodesByValue(object value) {
			List<TreeListNode> nodes = new List<TreeListNode>();
			if(IsReady) {
				foreach(TreeListNode node in new TreeListNodeIterator(Nodes)) {
					if(Object.Equals(value, node.Content))
						nodes.Add(node);
				}
			}
			return nodes;
		}
		public TreeListNode FindNodeById(int id) { 
			if(!IsReady) return null;
			foreach(TreeListNode node in new TreeListNodeIterator(Nodes)) {
				if(node.Id == id)
					return node;
			}
			return null;
		}
		public virtual TreeListNode FindNodeByValue(string fieldName, object value) {
			if(!IsReady) return null;
			DataColumnInfo column = Columns[fieldName];
			if(column == null)
				return null;
			try {
				value = column.ConvertValue(value);
			}
			catch {
				return null;
			}
			foreach(TreeListNode node in new TreeListNodeIterator(Nodes)) {
				if(ValueComparer.Equals(DataHelper.GetValue(node, column), value))
					return node;
			}
			return null;
		}
		public TreeListNode FindVisibleNode(int rowHandle) {
			if(!IsReady)
				return null;
			return NodesInfo.FindVisibleNode(rowHandle);
		}
		public void CloseActiveEditor() {
			View.CloseEditor();
		}
		public void DoSortNodes() {
			DoSortNodes(RootNode);
		}
		public void DoSortNodes(TreeListNode parentNode) {
			if(View.DataControl == null) return;
			DoSortNodes(View.DataControl.SortInfoCore, parentNode);
		}
		public virtual TreeListFilterHelper CreateFilterHelper() {
			return new TreeListFilterHelper(this);
		}
		public override void DeleteRow(RowHandle rowHandle) {
			DeleteNode(GetNodeByRowHandle(rowHandle.Value), true);
		}
		public void DeleteNode(TreeListNode node, bool deleteChildren) {
			if(node == null || !AllowRemove) return;
			DataHelper.DeleteNode(node, deleteChildren, true);
		}
		protected internal virtual bool OnNodeExpandingOrCollapsing(TreeListNode treeListNode) {
			bool result = true;
			if(treeListNode.IsExpanded)
				result = view.RaiseNodeCollapsing(treeListNode);
			else
				result = view.RaiseNodeExpanding(treeListNode);
			if(result)
				DataHelper.NodeExpandingCollapsing(treeListNode);
			return result;
		}
		protected internal virtual void OnNodeExpandedOrCollapsed(TreeListNode treeListNode) {
			if(treeListNode.IsExpanded) {
				view.CheckFocusedNodeOnExpand(treeListNode);
				view.RaiseNodeExpanded(treeListNode);
			}
			else {
				view.CheckFocusedNodeOnCollapse(treeListNode);
				view.RaiseNodeCollapsed(treeListNode);
			}
		}
		protected internal virtual void OnNodeCheckStateChanged(TreeListNode treeListNode) {
			view.RaiseNodeCheckStateChanged(treeListNode);
		}
		protected internal virtual void OnNodeChanged(TreeListNode treeListNode, NodeChangeType changeType) {
			view.RaiseNodeChanged(treeListNode, changeType);
		}
		protected internal virtual TreeListNode CreateNode(object content) {
			return new TreeListNode(content);
		}
		protected virtual TreeListNodeComparer CreateNodesComparer() {
			return new TreeListNodeComparer(this);
		}
		protected virtual ValueComparer CreateValueComparer() {
			return new ValueComparer();
		}
		protected virtual TreeListDataHelperBase CreateDataHelper() {
			if(DataSource == null)
				return new TreeListUnboundDataHelper(this);
			switch(View.TreeDerivationMode) {
				case TreeDerivationMode.ChildNodesSelector:
					return new TreeListHierarchicalDataHelper(this, DataSource);
				case TreeDerivationMode.HierarchicalDataTemplate:
					return new TreeListHierarchicalDataTemplateHelper(this, DataSource);
				default:
					return new TreeListSelfReferenceDataHelper(this, DataSource);
			}
		}
		protected virtual void OnFilterExpressionChanged() {
			ResetFilterExpressionEvaluator();
			DoRefreshCore(false);
		}
		void ResetFilterExpressionEvaluator() {
			this.filterExpressionEvaluator = null;
		}
		protected internal void UpdateDataHelper() {
			if(DataHelper != null)
				DataHelper.Dispose();
#if SL
			ValuesCache.Clear();
#endif
			DataHelper = CreateDataHelper();
			this.shouldUpdateColumnsUnboundType = true;
		}
		bool shouldUpdateColumnsUnboundType;
		void UpdateColumnsUnboundTypeIfNeeded() {
			if(shouldUpdateColumnsUnboundType && DataHelper.IsLoaded) {
				if(View.DisplayMemberBindingClient != null) {
					shouldUpdateColumnsUnboundType = false;
					bool requiresReloadData = DataHelper.RequiresReloadDataOnEndUpdate;
					DataHelper.RequiresReloadDataOnEndUpdate = false;
					try {
						View.DisplayMemberBindingClient.UpdateColumns();
					}
					finally {
						DataHelper.RequiresReloadDataOnEndUpdate = requiresReloadData;
					}
				}
			}
		}
		protected internal override void OnDataSourceChanged() {
			BeginUpdateCore();
			try {
				NodesState.LockSaveNodesState();
				ResetFilterExpressionEvaluator();
				UpdateDataHelper();
				RePopulateColumns();
				this.currentControllerRow = GridControl.InvalidRowHandle;
				DoRefresh(false);
				View.OnDataSourceChanged();
			} finally {
				UpdateColumnsUnboundTypeIfNeeded();
				NodesState.UnlockSaveNodesState();
				EndUpdateCore();
				ResetCurrentPosition();
			}
			ScheduleRepopulateColumnsIfNeeded();
		}
		void ScheduleRepopulateColumnsIfNeeded() {
			if(!DataHelper.IsLoaded) 
				isRepopulateColumnsNeeded = true;
		}
		protected virtual void DoRefreshCore(bool forceResort = true) {
			if(IsUpdateLocked) return;
			if(forceResort) 
				DoSortNodes();
			DoFilterNodes();
			NodesInfo.SetDirty();
			View.UpdateFocusedNode();
			SynchronizeSummary();
			UpdateRows();
		}
		protected internal void DoFilterNodes() {
			DoFilterNodes(RootNode);
			TreeListNode node = CurrentNode;
			if(node != null && !node.IsVisible)
				CurrentControllerRow = 0;
			bool selectionChanged = false;
			TreeListSelection.BeginSelection();
			try {
				foreach(TreeListNode n in TreeListSelection.GetSelectedNodes()) {
					if(!n.IsVisible) {
						TreeListSelection.SetSelected(n.RowHandle, false);
						selectionChanged = true;
					}
				}
			}
			finally {
				if(selectionChanged) TreeListSelection.EndSelection();
				else TreeListSelection.CancelSelection();
			}
		}
		protected internal void DoFilterNodes(TreeListNode parentNode) {
			if(IsUpdateLocked) return;
			if(!IsFiltered && !HasFilter) return;
			DoFilterNodesCore(parentNode);
			IsFiltered = HasFilter;
		}
#region filter 
		protected virtual void DoFilterNodesCore(TreeListNode parent) {
			foreach(TreeListNode node in parent.Nodes) {
				bool res = DoFilterNode(node);
				if(res && View.FilterMode == TreeListFilterMode.Standard)
					continue;
				else
					DoFilterNodesCore(node);
			}
		}
		protected virtual bool DoFilterNode(TreeListNode node) {
			try {
				node.IsVisible = CalcNodeVisibility(node);
				node.IsExpandedSetInternally = false;
				if(View.FilterMode == TreeListFilterMode.Standard) {
					if(!node.IsVisible)
						node.ProcessNodeAndDescendantsAction((n) => {
							n.IsVisible = false;
							return true;
						});
				}
				if(View.FilterMode == TreeListFilterMode.Extended) {
					if(node.IsVisible)
						UpdateParentVisibilityStateOnFilter(node);
				}
				if(node.IsVisible && HasFilter && View.ExpandNodesOnFiltering)
					UpdateParentExpandStateOnFilter(node);
				return !node.IsVisible;
			}
			catch {
				return true;
			}
		}
		protected virtual void UpdateParentVisibilityStateOnFilter(TreeListNode node) {
			TreeListNode parent = node.ParentNode;
			while(parent != null && !parent.IsVisible) {
				parent.IsVisible = true;
				parent = parent.ParentNode;
			}
		}
		protected virtual void UpdateParentExpandStateOnFilter(TreeListNode node) {
			BeginUpdateCore();
			try {
				TreeListNode parent = node.ParentNode;
				while(parent != null && !parent.IsExpandedSetInternally) {
					parent.IsExpanded = true;
					parent.IsExpandedSetInternally = parent.IsExpanded;
					parent = parent.ParentNode;
				}
			}
			finally {
				CancelUpdate();
			}
		}
		protected virtual bool CalcNodeVisibility(TreeListNode node) {
			bool? res = IsNodeUserFit(node);
			if(res.HasValue)
				return res.Value;
			return FilterExpressionEvaluator != null ? FilterExpressionEvaluator.Fit(node) : true;
		}
		protected virtual bool? IsNodeUserFit(TreeListNode node) {
			return View.RaiseCustomNodeFilter(node);
		}
#endregion
		bool addingFirstNode;
		internal void OnNodeCollectionChanging(TreeListNode node, NodeChangeType changeType) {
			addingFirstNode = false;
			if(changeType == NodeChangeType.Remove) {
				UnselectRemovedNode(node);
			}
			if(changeType == NodeChangeType.Add && Nodes.Count == 0) {  
				addingFirstNode = true;
			}
		}
		protected internal virtual void OnNodeCollectionChanged(TreeListNode node, NodeChangeType changeType, bool raiseNodeChangedEvent = true) {
			if(IsRecursiveNodesUpdateLocked) return;
			if(raiseNodeChangedEvent)
				OnNodeChanged(node, changeType);
			if(changeType == NodeChangeType.Add && !DataHelper.IsLoading) 
				node.ForceUpdateExpandState();
			if(changeType == NodeChangeType.ExpandButtonVisibility || changeType == NodeChangeType.Image || changeType == NodeChangeType.IsCheckBoxEnabled) {
				UpdateRow(node);
				return;
			}
			if(changeType == NodeChangeType.CheckBox) {
				if(IsRecursiveCheckingAllowed(node))
					node.CheckBoxUpdateLocker.DoLockedActionIfNotLocked(delegate { UpdateRows(); });
				else {
					node.CheckBoxUpdateLocker.DoIfNotLocked(delegate { UpdateRow(node); });
					node.isCheckStateInitialized = true;
				}
				return;
			}
			if(changeType == NodeChangeType.Remove || changeType == NodeChangeType.Add) {
				nodesCountIsActual = false;
				rowStates.Clear();
				NodesInfo.SetDirty();
			}
			if(changeType == NodeChangeType.Add)
				OnNodeAdded(node);
			if(changeType == NodeChangeType.Remove)
				OnNodeRemoved(node);
			if(changeType == NodeChangeType.Content)
				OnNodeContentChanged(node);
			if(changeType == NodeChangeType.Expand) {
				NodesInfo.SetDirty(true);
				UpdateRows();
			}
		}
		void UpdateNodeInternal(TreeListNode node, bool filterChildren) {
			if(IsUpdateLocked || IsCurrentRowEditing) return;
			UpdateNodeVisiblility(node, filterChildren);
			DoSortInternal(node);
			SynchronizeSummary();
			View.UpdateColumnsTotalSummary();
		}
		bool HasFilter { get { return !ReferenceEquals(FilterCriteria, null) || View.IsCustomNodeFilterAssigned; } }
		void OnNodeContentChanged(TreeListNode node) {
			UpdateNodeInternal(node, false);
			DoRefreshRow(node);
			if(node == CurrentNode && View.DataControl != null)
				View.DataControl.UpdateCurrentCellValue();
		}
		void DoRefreshRow(TreeListNode node) {
			if(IsSorted || IsFiltered || View.ViewBehavior.GetServiceSummaries().Any())
				UpdateRows();
			else
				UpdateRow(node);
		}
		void UpdateNodeVisiblility(TreeListNode node, bool filterChildren) {
			if(View.FilterMode == TreeListFilterMode.Extended) {
				TreeListNode rootNode = node.RootNode;
				DoFilterNode(rootNode);
				DoFilterNodes(rootNode);
			}
			else {
				DoFilterNode(node);
				if(filterChildren)
					DoFilterNodes(node);
			}
			if(HasFilter)
				NodesInfo.SetDirty(true);
		}
		void OnNodeRemoved(TreeListNode node) {
			if(IsUpdateLocked) return;
			SynchronizeSummary();
			bool hasFocusedNode = View.FocusedNode != null;
			if(View.FocusedNode == node && NodesState.FocusedRow == null) { 
				View.FocusedNode = FindVisibleNode(View.FocusedRowHandle);
			}
			UpdateRows();
			if(hasFocusedNode)
				ResetCurrentPosition();
		}
		void OnNodeAdded(TreeListNode node) {
			if(IsUpdateLocked) return;
			UpdateNodeInternal(node, true);
			View.UpdateFocusedNode();
			UpdateColumnsUnboundTypeIfNeeded();
			DataHelper.RecalcNodeIdsIfNeeded();
			UpdateRows();
			if(addingFirstNode)
				ResetCurrentPosition();
		}
		void DoSortInternal(TreeListNode node) {
			if(IsCurrentRowEditing) return;
			DoSortNodes(node.parentNodeCore);
		}
		void UnselectRemovedNode(TreeListNode node) {
			if(!View.IsMultiSelection || View.DataControl == null || !View.DataControl.AllowUpdateSelectedItems)
				return;
			Selection.BeginSelection();
			SelectNodesRecursive(node, false);
			Selection.EndSelection();
		}
		protected void SelectNodesRecursive(TreeListNode node, bool selected) {
			if(node == null) return;
			Selection.SetSelected(node.RowHandle, selected);
			foreach(TreeListNode child in node.Nodes)
				SelectNodesRecursive(child, selected);
		}
		protected internal virtual void UpdateRows() {
			if(IsUpdateLocked) return;
			maxLevelCore = -1;
			MaxVisibleLevel = CalcMaxNodeLevel(true);
			view.UpdateRows();
		}
		protected internal virtual void UpdateRow(TreeListNode node) {
			if(IsUpdateLocked || node == null) return;
			RefreshRow(node.RowHandle);
		}
		protected internal virtual bool UseFirstRowTypeWhenPopulatingColumns(Type itemType) {
			return itemType.FullName == ListDataControllerHelper.UseFirstRowTypeWhenPopulatingColumnsTypeName;
		}
		protected internal override void ForceClearData() {
			if(DataHelper != null)
				DataHelper.Dispose();
#if SL
			ValuesCache.Clear();
#endif
			NodesInfo.SetDirty();
		}
		protected internal virtual UnboundColumnInfoCollection GetUnboundColumns() {
			return GetUnboundColumnsCore(View.GetColumns().Concat(View.ViewBehavior.GetServiceUnboundColumns()));
		}
		protected internal virtual ComplexColumnInfoCollection GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach(IColumnInfo column in View.GetColumns()) {
				if(column.UnboundType != UnboundColumnType.Bound) continue;
				if(column.FieldName.Contains(".") && Columns[column.FieldName] == null)
					res.Add(column.FieldName);
			}
			return res;
		}
		protected internal virtual PropertyDescriptorCollection GetFilterDescriptorCollection() {
			PropertyDescriptorCollection collection = new PropertyDescriptorCollection(null);
			foreach(DataColumnInfo column in Columns) {
				if(ShouldFilterByDisplayText(column))
					collection.Add(new TreeListDisplayTextPropertyDescriptor(this, column.Name));
				else
					collection.Add(column.PropertyDescriptor);
				collection.Add(new TreeListSearchDisplayTextPropertyDescriptor(this, column.Name));
			}
			return collection;
		}
		protected virtual bool ShouldFilterByDisplayText(DataColumnInfo dataColumn) {
			ColumnBase column = View.ColumnsCore[dataColumn.Name];
			if(column != null)
				return column.ColumnFilterMode == ColumnFilterMode.DisplayText;
			return false;
		}
		protected internal void ToggleExpandedAllNodes(bool expand) {
			ToggleExpandedAllChildNodes(RootNode, expand);
		}
		protected internal void ExpandToLevel(int maxLevel) {
			BeginUpdateCore();
			SaveFocusState();
			try {
				RootNode.ProcessNodeAndDescendantsAction((node) =>
				{
					if(maxLevel >= 0  && node.Level > maxLevel) return false;
					node.IsExpanded = true;
					return true;
				});
			}
			finally {
				RestoreFocusState();
				EndUpdateCore();
			}
		}
		protected internal void ToggleExpandedAllChildNodes(TreeListNode parent, bool expand) {
			if(parent == null) return;
			BeginUpdateCore();
			if(expand) SaveFocusState();
			try {
				parent.ToggleExpandedAllChildrenCore(expand);
			}
			finally {
				if(expand) RestoreFocusState();
				EndUpdateCore();
			}
		}
		protected internal virtual DevExpress.Data.Filtering.Helpers.ExpressionEvaluator CreateExpressionEvaluator(string criteria, out Exception e) {
			e = null;
			if(string.IsNullOrEmpty(criteria)) return null;
			CriteriaOperator criteriaOperator;
			try {
				criteriaOperator = CriteriaOperator.Parse(criteria, null);
			}
			catch(DevExpress.Data.Filtering.Exceptions.CriteriaParserException) {
				criteriaOperator = CriteriaOperator.Parse(string.Empty, null);
			}
			return CreateExpressionEvaluator(criteriaOperator, out e);
		}
		protected internal virtual object GetUnboundData(object p, string propName, object value) {
			ColumnBase column = View.ColumnsCore[propName];
			if(column != null && column.Binding != null) {
				int rowHandle = GetRowHandleByNode(p as TreeListNode);
				if(rowHandle != GridControl.InvalidRowHandle)
					return column.DisplayMemberBindingCalculator.GetValue(rowHandle, DataControlBase.InvalidRowIndex);
			}
			return View.RaiseCustomUnboundColumnData(p, propName, value, true);
		}
		object GetRow(TreeListNode node) {
			return node != null ? node.Content : null;
		}
		protected internal virtual void SetUnboundData(object p, string propName, object value) {
			ColumnBase column = View.ColumnsCore[propName];
			if(column != null && column.Binding != null) {
				int rowHandle = GetRowHandleByNode(p as TreeListNode);
				if(rowHandle != GridControl.InvalidRowHandle) {
					column.DisplayMemberBindingCalculator.SetValue(rowHandle, value);
					return;
				}
			}
			View.RaiseCustomUnboundColumnData(p, propName, value, false);
		}
		internal override DataColumnInfo GetActualColumnInfo(string fieldName) {
			return Columns[fieldName] ?? DetailColumns[fieldName];
		}
		protected internal IList<TreeListNode> GetAllNodes() {
			return GetAllNodesCore(node => true, node => true);
		}
		protected internal IList<TreeListNode> GetAllVisibleNodes() {
			return GetAllNodesCore(node => node.IsExpanded, node => node.IsVisible);
		}
		IList<TreeListNode> GetAllNodesCore(Func<TreeListNode, bool> shouldCollectChildren, Func<TreeListNode, bool> shouldProcessNode) {
			List<TreeListNode> list = new List<TreeListNode>();
			CollectNodes(Nodes, list, shouldCollectChildren, shouldProcessNode);
			return list;
		}
		internal override void EnsureRowLoaded(int rowHandle) { }
		void CollectNodes(TreeListNodeCollection nodes, IList<TreeListNode> list, Func<TreeListNode, bool> shouldCollectChildren, Func<TreeListNode, bool> shouldProcessNode) {
			foreach(TreeListNode node in nodes) {
				if(shouldProcessNode(node))
					list.Add(node);
				if(shouldCollectChildren(node))
					CollectNodes(node.Nodes, list, shouldCollectChildren, shouldProcessNode);
			}
		}
		protected internal void SaveExpansionState() {
			NodesState.SaveNodesState();
		}
		protected internal void SaveFocusState() {
			NodesState.SaveCurrentFocus();
		}
		protected internal void BeginUpdateCore() {
			dataUpdateLocker.Lock();
		}
		protected internal void EndUpdateCore() {
			dataUpdateLocker.Unlock();
			DoRefreshCore();
		}
		protected internal void RestoreFocusState() {
			NodesState.RestoreCurrentFocus();
		}
		int CalcMaxNodeLevel(bool visibleOnly) {
			return Math.Max(0, GetMaxNodeLevel(RootNode, -1, visibleOnly));
		}
		int GetMaxNodeLevel(TreeListNode rootNode, int level, bool visibleOnly) {
			int maxLevel = level++;
			if(!visibleOnly || rootNode.IsExpanded)
				foreach(TreeListNode node in rootNode.Nodes) {
					if(node.IsVisible)
						maxLevel = Math.Max(GetMaxNodeLevel(node, level, visibleOnly), maxLevel);
					else if(View.FilterMode == TreeListFilterMode.Smart && node.HasVisibleChildren)
						maxLevel = Math.Max(GetMaxNodeLevel(node, level - 1, visibleOnly), maxLevel);
				}
			return maxLevel;
		}
		protected internal virtual void OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			View.OnSelectionChanged(e);
		}
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			return DataHelper.GetValue(theObject as TreeListNode, descriptor);
		}
		#endregion
		public override int FindRowByRowValue(object value) {
#if DEBUGTEST
			FindRowByRowValueCallCount++;
#endif
			TreeListNode node = FindNodeByValue(value);
			if(node == null)
				return GridControl.InvalidRowHandle;
			return GetRowHandleByNode(node);
		}
		public override int FindRowByValue(string fieldName, object value) {
			TreeListNode node = FindNodeByValue(fieldName, value);
			if(node == null)
				return GridControl.InvalidRowHandle;
			return GetRowHandleByNode(node);
		}
		public override object GetRowByListIndex(int listIndex) {
			return DataHelper.GetDataRowByListIndex(listIndex);
		}
		public override object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) {
			return DataHelper.GetCellValueByListIndex(listSourceRowIndex, fieldName);
		}
		public override int GetRowHandleByListIndex(int listIndex) {
			object row = GetRowByListIndex(listIndex);
			if(row == null)
				return GridControl.InvalidRowHandle;
			return GetRowHandleByNode(FindNodeByValue(row));
		}
		public override int GetListIndexByRowHandle(int rowHandle) {
			TreeListNode node = GetNodeByRowHandle(rowHandle);
			if(node != null)
				return DataHelper.GetListIndexByDataRow(node.Content);
			return -1;
		}
		internal override bool IsServerMode {
			get { return false; }
		}
		internal override bool IsICollectionView {
			get { return false; }
		}
		internal override bool IsAsyncServerMode {
			get { return false; }
		}
		internal override bool IsAsyncOperationInProgress { get { return false; } set { } }
		internal override void ScheduleAutoPopulateColumns() {
			autoPopulateColumns = true;
		}
		internal override void CancelAllGetRows() {
		}
		internal override void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount) {
		}
		#region Grouping
		public override GridSummaryItemCollection GroupSummary {
			get { return View.DataControl != null ? (GridSummaryItemCollection)View.DataControl.GroupSummaryCore : null; }
		}
		public override void ExpandAll() {
			throw new System.NotImplementedException();
		}
		public override void CollapseAll() {
			throw new System.NotImplementedException();
		}
		public override void ChangeGroupExpanded(int controllerRow, bool recursive) {
			throw new System.NotImplementedException();
		}
		internal override int GetGroupIndex(string fieldName) {
			throw new System.NotImplementedException();
		}
		public override int GroupedColumnCount {
			get { return 0; }
		}
		public override ISummaryItemOwner GroupSummaryCore { get { throw new NotImplementedException(); } }
		public override bool IsGroupRow(int visibleIndex) {
			throw new System.NotImplementedException();
		}
		public override bool IsGroupRowExpanded(int controllerRow) {
			throw new System.NotImplementedException();
		}
		public override void UpdateGroupSummary() {
			throw new System.NotImplementedException();
		}
		public override int GetControllerRowByGroupRow(int groupRowHandle) {
			throw new System.NotImplementedException();
		}
		public override object GetGroupRowValue(int rowHandle, ColumnBase column) {
			throw new System.NotImplementedException();
		}
		public override object GetGroupRowValue(int rowHandle) {
			throw new System.NotImplementedException();
		}
		public override bool TryGetGroupSummaryValue(int rowHandle, SummaryItemBase item, out object value) {
			throw new System.NotImplementedException();
		}
		#endregion
		internal void CustomNodeSort(TreeListCustomColumnSortEventArgs e) {
			View.RaiseCustomColumnSort(e);
		}
		internal void UpdateNodeId(TreeListNode node) {
			DataHelper.UpdateNodeId(node);
		}
		internal void TryRepopulateColumns() {
			if(!isRepopulateColumnsNeeded) return;
			if(!DataHelper.IsLoaded) return;
			bool repopulateColumns = autoPopulateColumns && ShouldTryRepopulateColumns();
			DataHelper.PopulateColumns();
			if(repopulateColumns && View.DataControl != null)
				View.DataControl.PopulateColumns();
			isRepopulateColumnsNeeded = false;
		}
		bool ShouldTryRepopulateColumns() {
			IList<IColumnInfo> columns = View.GetColumns();
			return columns.Count == 0 || (Columns.Count == 1 && Columns[0].PropertyDescriptor is DevExpress.Data.Access.SimpleListPropertyDescriptor);
		}
		bool autoPopulateColumns;
		bool isRepopulateColumnsNeeded;
		protected internal void OnFilterModeChanged() {
			DoRefreshCore(false);
		}
		protected internal bool? GetObjectIsChecked(TreeListNode node) {
			if(!string.IsNullOrEmpty(View.CheckBoxFieldName)) {
				try {
					object value = View.GetNodeValue(node, View.CheckBoxFieldName);
					if(View.CheckBoxValueConverter != null && Columns[View.CheckBoxFieldName] != null) {
						return (bool?)View.CheckBoxValueConverter.Convert(value, typeof(bool?), null, null);
					}
					return (bool?)value;
				}
				catch (Exception e){
					if(e is InvalidCastException)
						throw new InvalidCastException(CheckBoxExceptionMessage);
					else throw e;
				}
			}
			return node == null ? null : node.IsChecked;
		}
		protected internal void SetObjectIsChecked(TreeListNode node, bool? checkStatus) {
			if(!string.IsNullOrEmpty(View.CheckBoxFieldName)) {
				CloseActiveEditor();
				object value = checkStatus;
				if(View.CheckBoxValueConverter != null && Columns[View.CheckBoxFieldName] != null) {
					Type targetType = GetTargetType(node, Columns[View.CheckBoxFieldName]);
					value = View.CheckBoxValueConverter.ConvertBack(value, targetType, null, null);
				}
				SetNodeValue(node, View.CheckBoxFieldName, value);
			}
		}
		Type GetTargetType(TreeListNode node, DataColumnInfo columnInfo) {
			PropertyDescriptor descriptor = columnInfo.PropertyDescriptor;
			UnitypeDataPropertyDescriptor unitypeDescriptor = descriptor as UnitypeDataPropertyDescriptor;
			if(unitypeDescriptor != null) {
				PropertyDescriptor tempDescriptor = unitypeDescriptor.GetProperyDescriptor(node.Content);
				if(tempDescriptor != null)
					descriptor = tempDescriptor;
			}
			Type targetType = descriptor.PropertyType;
			return targetType;
		}
		protected internal bool IsRecursiveCheckingAllowed(TreeListNode node) {
			if(View.AllowRecursiveNodeChecking)
				return node.isCheckStateInitialized;
			else return false;
		}
		internal void InitNodesIsChecked() {
			foreach(TreeListNode node in new TreeListNodeIterator(Nodes))
				node.InitIsChecked();
		}
		public static bool IsUnitypeColumn(DataColumnInfo info) {
			return info.PropertyDescriptor is UnitypeDataPropertyDescriptor || info.PropertyDescriptor is UnitypeComplexPropertyDescriptor;
		}
		protected internal virtual void OnColumnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(View.TreeDerivationMode == TreeDerivationMode.Selfreference) return;
			if(e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset) {
				foreach(IDataColumnInfo column in View.ColumnsCore)
					if(Columns[column.FieldName] == null) {
						RePopulateColumns();
						break;
					}
			}
		}
		protected internal void UpdateNodesExpandState(TreeListNodeCollection nodes, bool updateEvaluator = true) {
			foreach(TreeListNode node in new TreeListNodeIterator(nodes)) {
				if(updateEvaluator) node.UpdateExpandStateBindingEvaluator();
				node.ForceUpdateExpandState();
			}
		}
		protected internal override object GetFormatInfoCellValueByListIndex(int listIndex, string fieldName) {
			return GetNodeValue(GetNodeByRowHandle(listIndex), fieldName);
		}
	}
	public class TreeListNodesInfo {
#if DEBUGTEST
		internal 
#endif
		Dictionary<int, TreeListNode> rowHandleToNodeCache;
		#if DEBUGTEST
		internal 
#endif
		Dictionary<int, TreeListNode> visibleIndexToNodeCache;
		Dictionary<TreeListNode, int> nodeToVisibleIndexCache;
		bool shouldRefreshRowHandles, shouldRefreshVisibleIndicies;
		int oldVisibleNodesCount;
		public TreeListNodesInfo(TreeListDataProvider dataProvider) {
			DataProvider = dataProvider;
			rowHandleToNodeCache = new Dictionary<int, TreeListNode>();
			visibleIndexToNodeCache = new Dictionary<int, TreeListNode>();
			nodeToVisibleIndexCache = new Dictionary<TreeListNode, int>();
			SetDirty();
		}
		protected TreeListDataProvider DataProvider { get; private set; }
		public int TotalNodesCount {
			get {
				EnsureRowHandles();
				return rowHandleToNodeCache.Count;
			}
		}
		public int TotalVisibleNodesCount {
			get {
				EnsureVisibleIndicies();
				return visibleIndexToNodeCache.Count;
			}
		}
		public TreeListNode GetNodeByRowHandle(int rowHandle) {
			EnsureRowHandles();
			TreeListNode node = null;
			if(rowHandleToNodeCache.TryGetValue(rowHandle, out node))
				return node;
			return null;
		}
		public int GetRowHandleByNode(TreeListNode node) {
			EnsureRowHandles();
			return node.rowHandle;
		}
		public int GetVisibleIndexByNode(TreeListNode node) {
			EnsureVisibleIndicies();
			if(!nodeToVisibleIndexCache.ContainsKey(node))
				return -1;
			return node.visibleIndex;
		}
		public TreeListNode GetNodeByVisibleIndex(int visibleIndex) {
			TreeListNode node = null;
			EnsureVisibleIndicies();
			if(visibleIndexToNodeCache.TryGetValue(visibleIndex, out node))
				return node;
			return null;
		}
		internal TreeListNode FindVisibleNode(int rowHandle) {
			if(rowHandle < 0) return null;
			for(int i = rowHandle; i < TotalNodesCount; i++) {
				TreeListNode node = GetNodeByRowHandle(i);
				if(node.IsVisible && GetVisibleIndexByNode(node) >= 0) return node;
			}
			for(int i = rowHandle - 1; i >= 0; i--) {
				TreeListNode node = GetNodeByRowHandle(i);
				if(node.IsVisible && GetVisibleIndexByNode(node) >= 0) return node;
			}
			return null;
		}
		public void SetDirty() {
			SetDirty(false);
		}
		public void SetDirty(bool visibleIndiciesOnly) {
			if(!this.shouldRefreshVisibleIndicies) {
				this.oldVisibleNodesCount = this.visibleIndexToNodeCache.Count;
				this.shouldRefreshVisibleIndicies = true;
				this.ClearVisibleIndicies();
			}
			if(visibleIndiciesOnly)
				return;
			this.shouldRefreshRowHandles = true;
			this.ClearRowHandles();
		}
		protected void EnsureRowHandles() {
			if(!shouldRefreshRowHandles)
				return;
			int counter = 0;
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes)) {
				node.rowHandle = counter;
				rowHandleToNodeCache[counter] = node;
				counter++;
			}
			shouldRefreshRowHandles = false;
		}
		protected void EnsureVisibleIndicies() {
			if(!shouldRefreshVisibleIndicies)
				return;
			int oldVisibleIndiciesCount = this.oldVisibleNodesCount;
			int counter = 0;
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes, true)) {
				if(!node.IsVisible) continue;
				node.visibleIndex = counter;
				visibleIndexToNodeCache[counter] = node;
				nodeToVisibleIndexCache[node] = counter;
				counter++;
			}
			shouldRefreshVisibleIndicies = false;
			if(visibleIndexToNodeCache.Count != oldVisibleIndiciesCount) {
				if(DataProvider.View.DataControl != null) {
					DataProvider.View.DataControl.RaiseVisibleRowCountChanged();
				}
			}
		}
		protected void ClearRowHandles() {
			rowHandleToNodeCache.Clear();
		}
		protected void ClearVisibleIndicies() {
			visibleIndexToNodeCache.Clear();
			nodeToVisibleIndexCache.Clear();
		}
	}
	public class TreeListNodesState {
		Locker currentFocusLocker = new Locker(), nodesStateLocker = new Locker();
		public TreeListNodesState(TreeListDataProvider provider) {
			DataProvider = provider;
			NodesState = new Dictionary<object, TreeListNodeState>();
		}
		protected TreeListDataProvider DataProvider { get; private set; }
		protected TreeListView View { get { return DataProvider.View; } }
		protected Dictionary<object, TreeListNodeState> NodesState { get; private set; }
		public object FocusedRow { get; private set; }
		public int FocusedRowHandle { get; private set; }
		public void ClearState() {
			ClearNodesState();
			ClearCurrentFocus();
		}
		protected void ClearNodesState() {
			NodesState.Clear();
		}
		protected void ClearCurrentFocus() {
			FocusedRow = null;
			FocusedRowHandle = TreeListControl.InvalidRowHandle;
		}
		internal void LockSaveNodesState() {
			currentFocusLocker.Lock();
			nodesStateLocker.Lock();
		}
		internal void UnlockSaveNodesState() {
			currentFocusLocker.Unlock();
			nodesStateLocker.Unlock();
		}
		public bool SaveCurrentFocus(bool supressLocker = false) {
			if(supressLocker || !currentFocusLocker.IsLocked) {
				SaveCurrentFocusCore();
				if(!supressLocker) currentFocusLocker.Lock();
				return true;
			}
			currentFocusLocker.Lock();
			return false;
		}
		protected virtual void SaveCurrentFocusCore() {
			if(View.DataControl != null && !View.DataControl.isSync) return;
			FocusedRow = View.DataControl != null ? View.DataControl.CurrentItem : null;
			FocusedRowHandle = View.FocusedRowHandle;
		}
		public void RestoreCurrentFocus() {
			currentFocusLocker.Unlock();
			currentFocusLocker.DoIfNotLocked(() => RestoreCurrentFocusCore());
		}
		protected virtual void RestoreCurrentFocusCore() {
			if(View.DataControl != null && !View.DataControl.isSync) return;
			if(View.DataControl != null)
				View.DataControl.CurrentItemChangedLocker.Lock();
			View.ScrollIntoViewLocker.Lock();
			bool focusedNodeChanged = false;
			try {
				if(FocusedRow != null && DataProvider.IsReady) {
					TreeListNode node = DataProvider.FindNodeByValue(FocusedRow);
					if(node == null)
						node = DataProvider.FindVisibleNode(FocusedRowHandle);
					if(View.FocusedNode != node)
						focusedNodeChanged = true;
					View.FocusedNode = node;
					View.SetFocusedRowHandle(DataProvider.GetRowHandleByNode(View.FocusedNode));
					if(View.DataControl != null && View.DataControl.AllowUpdateCurrentItem)
						View.DataControl.SetCurrentItemCore((View.FocusedNode != null) ? View.FocusedNode.Content : null);
				}
			}
			finally {
				if(View.DataControl != null)
					View.DataControl.CurrentItemChangedLocker.Unlock();
				View.ScrollIntoViewLocker.Unlock();
				ClearCurrentFocus();
				if(focusedNodeChanged)
					DataProvider.ResetCurrentPosition();
			}
		}
		public bool SaveNodesState(bool supressLocker = false) {
			if(supressLocker || !nodesStateLocker.IsLocked) {
				SaveNodesStateCore();
				if(!supressLocker) nodesStateLocker.Lock();
				return true;
			}
			nodesStateLocker.Lock();
			return false;
		}
		protected virtual void SaveNodesStateCore() {
			ClearNodesState();
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
				if(node.Content != null && !NodesState.ContainsKey(node.Content))
					NodesState.Add(node.Content, new TreeListNodeState() {
						IsExpanded = node.IsExpanded,
						IsChecked = node.IsChecked,
						IsSelected = DataProvider.Selection.GetSelected(node.RowHandle),
						SelectedObject = DataProvider.Selection.GetSelectedObject(node.RowHandle)
					});
		}
		public void RestoreNodesState(bool supressLocker = false) {
			if(supressLocker) {
				RestoreNodesStateCore();
				return;
			}
			nodesStateLocker.Unlock();
			nodesStateLocker.DoIfNotLocked(() => RestoreNodesStateCore());
		}
		protected virtual void RestoreNodesStateCore() {
			if(NodesState.Count == 0) return;
			DataProvider.Selection.BeginSelection();
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes)) {
				TreeListNodeState state;
				if(NodesState.TryGetValue(node.Content, out state)) {
					node.IsExpanded = state.IsExpanded;
					if(string.IsNullOrEmpty(View.CheckBoxFieldName))
						node.IsChecked = state.IsChecked;
					DataProvider.Selection.SetSelected(node.RowHandle, state.IsSelected, state.SelectedObject);
				}
			}
			DataProvider.Selection.EndSelection();
			ClearNodesState();
		}
		#region inner classes
		protected class TreeListNodeState {
			public bool IsExpanded { get; set; }
			public bool? IsChecked { get; set; }
			public bool IsSelected { get; set; }
			public object SelectedObject { get; set; }
		}
		#endregion
	}
}
