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
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid.Hierarchy;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public abstract class DataTreeBuilder {
		internal delegate DataRowNode CreateRowNodeDelegate(DataControllerValuesContainer controllerValues);
		internal static T CreateRowElement<T>(bool isGroupRow, Func<T> groupRowCreator, Func<T> dataRowCreator) {
			return isGroupRow ? groupRowCreator() : dataRowCreator();
		}
		readonly HeadersData headersData;
		readonly DataViewBase view;
		readonly DetailNodeContainer rootNodeContainer;
		readonly MasterNodeContainer masterRootNodeContainer;
		readonly DetailRowsContainer rootRowsContainer;
		readonly MasterRowsContainer masterRootRowsContainer;
		public abstract bool SupportsHorizontalVirtualization { get; }
		public abstract bool SupportsMasterDetail { get; }
		public DetailNodeContainer RootNodeContainer { get { return rootNodeContainer; } }
		public MasterNodeContainer MasterRootNodeContainer { get { return masterRootNodeContainer; } }
		public DetailRowsContainer RootRowsContainer { get { return rootRowsContainer; } }
		public MasterRowsContainer MasterRootRowsContainer { get { return masterRootRowsContainer; } }
		public DataViewBase View { get { return view; } }
		public virtual int VisibleCount { get { return view.DataControl.VisibleRowCount; } }
		public HeadersData HeadersData { get { return headersData; } }
		public DataTreeBuilder(DataViewBase view, MasterNodeContainer masterRootNode, MasterRowsContainer masterRootDataItem) {
			this.view = view;
			headersData = new HeadersData(this);
			rootRowsContainer = masterRootDataItem != null ?
				new DetailRowsContainer(this, 0) :
				new MasterRowsContainer(this, 0);
			this.masterRootRowsContainer = masterRootDataItem ?? (MasterRowsContainer)rootRowsContainer;
			rootNodeContainer = masterRootNode != null ?
				new DetailNodeContainer(this, 0) :
				new MasterNodeContainer(this, 0);
			this.masterRootNodeContainer = masterRootNode ?? (MasterNodeContainer)rootNodeContainer;
		}
		public void SynchronizeMasterNode() {
			MasterRootRowsContainer.Synchronize(MasterRootNodeContainer);
			AfterSynchronization();
			MasterRootRowsContainer.UpdatePostponedData(true, MasterRootRowsContainer.RowsToUpdate.Count != 0);
		}
		public virtual void ClearRowsCache() { }
		public virtual void AfterSynchronization() { }
		public virtual void Synchronize(RowsContainer dataContainer, NodeContainer nodeContainer) { }
		public virtual void ForceLayout() { }
		public virtual void SetRowStateDirty() { }
		internal virtual DataRowNode GetGroupSummaryRowNode(int rowHandle) { return null; }
		internal virtual void AddGroupSummaryRowNode(int rowHandle, DataRowNode node) { }
		internal virtual DataRowNode GetRowNode(CreateRowNodeDelegate createDelegate, DataControllerValuesContainer controllerValues) {
			return createDelegate(controllerValues);
		}
		internal virtual void UpdateColumnData(ColumnsRowDataBase rowData, GridColumnData cellData, ColumnBase column) {
		}
		internal virtual void UpdateCellData(RowData rowData, GridCellData cellData, ColumnBase column) {
			cellData.Data = rowData.DataContext;
		}
		internal abstract void UpdateRowData(RowData rowData);
		internal virtual void UpdateRowDataError(RowData rowData) { }
		internal virtual void UpdateGroupRowData(RowData rowData) { }
		internal abstract IList<ColumnBase> GetVisibleColumns();
		internal abstract IList<ColumnBase> GetFixedNoneColumns();
		internal abstract IList<ColumnBase> GetFixedLeftColumns();
		internal abstract IList<ColumnBase> GetFixedRightColumns();
		internal abstract ColumnBase GetGroupColumnByNode(DataRowNode node);
		internal abstract object GetGroupValueByNode(DataRowNode node);
		internal abstract string GetGroupRowDisplayTextByNode(DataRowNode node);
		internal abstract object GetCellValue(RowData rowData, string fieldName);
#if !SL
		internal abstract object GetServiceTotalSummaryValue(ServiceSummaryItem item);
#endif
		internal abstract object GetWpfRow(RowData rowData, int listSourceRowIndex);
		internal abstract object GetRowValue(RowData rowData);
		internal abstract IList<SummaryItemBase> GetGroupSummaries();
		internal abstract bool TryGetGroupSummaryValue(RowData rowData, SummaryItemBase item, out object value);
		protected internal virtual int GetRowHandleByVisibleIndexCore(int visibleIndex) {
			return view.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
		}
		protected internal virtual int GetRowLevelByControllerRow(int rowHandle) {
			return view.DataProviderBase.GetRowLevelByControllerRow(rowHandle);
		}
		protected internal virtual int GetRowVisibleIndexByHandleCore(int rowHandle) {
			return view.DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
		}
		protected internal virtual int GetRowLevelByVisibleIndex(int visibleIndex) {
			return view.DataProviderBase.GetRowLevelByVisibleIndex(visibleIndex);
		}
	}
	public class VisualDataTreeBuilder : DataTreeBuilder {
		readonly SynchronizationQueues synchronizationQueues;
		readonly Dictionary<int, RowData> rows = new Dictionary<int, RowData>();
		readonly Dictionary<int, DataRowNode> nodes = new Dictionary<int, DataRowNode>();
		readonly Dictionary<int, RowData> groupSummaryRows = new Dictionary<int, RowData>();
		readonly Dictionary<int, DataRowNode> groupSummaryNodes = new Dictionary<int, DataRowNode>();
		internal SynchronizationQueues SynchronizationQueues { get { return synchronizationQueues; } }
		public Dictionary<int, RowData> Rows { get { return rows; } }
		public Dictionary<int, RowData> GroupSummaryRows { get { return groupSummaryRows; } }
		public Dictionary<int, DataRowNode> Nodes { get { return nodes; } }
		public Dictionary<int, DataRowNode> GroupSummaryNodes { get { return groupSummaryNodes; } }
		public override bool SupportsMasterDetail { get { return true; } }
		public override bool SupportsHorizontalVirtualization { get { return true; } }
		public VisualDataTreeBuilder(DataViewBase view, MasterNodeContainer masterRootNode,
			MasterRowsContainer masterRootDataItem, SynchronizationQueues synchronizationQueues)
			: base(view, masterRootNode, masterRootDataItem) {
				this.synchronizationQueues = synchronizationQueues;
		}
		public void CacheRowData(RowData rowData) {
			Rows[rowData.RowHandle.Value] = rowData;
		}
		public void CacheGroupSummaryRowData(RowData rowData) {
			GroupSummaryRows[rowData.RowHandle.Value] = rowData;
		}
		public override void ClearRowsCache() {
			Rows.Clear();
			GroupSummaryRows.Clear();
		}
		public override void AfterSynchronization() {
			SynchronizationQueues.SynchronizeUnsynchronizedNodes();
			View.DataControl.MasterDetailProvider.SynchronizeDetailTree();
		}
		public override void ForceLayout() {
			View.ForceLayout();
		}
		public override void SetRowStateDirty() {
			View.RowsStateDirty = true;
		}
		internal override DataRowNode GetGroupSummaryRowNode(int rowHandle) {
			DataRowNode rowNode;
			if(GroupSummaryNodes.TryGetValue(rowHandle, out rowNode))
				return rowNode;
			return null;
		}
		internal override void AddGroupSummaryRowNode(int rowHandle, DataRowNode node) {
			GroupSummaryNodes[rowHandle] = node;
		}
		internal override DataRowNode GetRowNode(CreateRowNodeDelegate createDelegate, DataControllerValuesContainer controllerValues) {
			DataRowNode rowNode;
			if(Nodes.TryGetValue(controllerValues.RowHandle.Value, out rowNode)) {
				rowNode.Update(controllerValues);
				return rowNode;
			}
			rowNode = createDelegate(controllerValues);
			Nodes.Add(controllerValues.RowHandle.Value, rowNode);
			return rowNode;
		}
		internal override void UpdateCellData(RowData rowData, GridCellData cellData, ColumnBase column) {
			base.UpdateCellData(rowData, cellData, column);
			cellData.UpdateFullState(rowData.RowHandle.Value);
			if(View.NeedCellsWidthUpdateOnScrolling)
				cellData.SyncCellContentPresenterProperties();
			rowData.UpdateCellDataError(column, cellData);
		}
		internal override void UpdateRowData(RowData rowData) {
			rowData.UpdateFullState();
		}
		internal override void UpdateRowDataError(RowData rowData) {
			RowValidationError rowStateError = View.DataControl.RowStateError;
			if(rowStateError != null && rowData.RowHandleCore.Value == View.FocusedRowHandle) {
				BaseEditHelper.SetValidationError(rowData, rowStateError);
				View.ValidationError = rowStateError;
				return;
			}
			if(!View.HasCellEditorError || (View.ValidationError != BaseEditHelper.GetValidationError(rowData))) {
				if((View.ItemsSourceErrorInfoShowMode & ItemsSourceErrorInfoShowMode.Row) != ItemsSourceErrorInfoShowMode.None) {
					DevExpress.XtraEditors.DXErrorProvider.ErrorInfo errorInfo = View.DataProviderBase.GetErrorInfo(rowData.RowHandle);
						BaseEditHelper.SetValidationError(rowData, String.IsNullOrEmpty(errorInfo.ErrorText) ? null
							: View.CreateRowValidationError(errorInfo.ErrorText, errorInfo.ErrorType, rowData.RowHandle.Value));
						if(!View.HasCellEditorError) View.ValidationError = null;
				}
				else
					BaseEditHelper.SetValidationError(rowData, null);
			}
			rowData.UpdateIndicatorState();
		}
		public override void Synchronize(RowsContainer dataContainer, NodeContainer nodeContainer) {
			dataContainer.CreateRowsContainerSyncronizer().Syncronize(nodeContainer);
		}
		internal override IList<ColumnBase> GetVisibleColumns() {
			return View.VisibleColumnsCore;
		}
		internal override IList<ColumnBase> GetFixedNoneColumns() {
			return ((ITableView)View).TableViewBehavior.FixedNoneVisibleColumns;
		}
		internal override IList<ColumnBase> GetFixedLeftColumns() {
			return ((ITableView)View).TableViewBehavior.FixedLeftVisibleColumns;
		}
		internal override IList<ColumnBase> GetFixedRightColumns() {
			return ((ITableView)View).TableViewBehavior.FixedRightVisibleColumns;
		}
		internal override ColumnBase GetGroupColumnByNode(DataRowNode node) {
			return View.GetColumnBySortLevel(node.Level);
		}
		internal override object GetGroupValueByNode(DataRowNode node) {
			return View.GetGroupDisplayValue(node.RowHandle.Value);
		}
		internal override string GetGroupRowDisplayTextByNode(DataRowNode node) {
			return View.GetGroupRowDisplayText(node.RowHandle.Value);
		}
		internal override object GetCellValue(RowData rowData, string fieldName) {
			return View.DataControl.GetCellValue(rowData.RowHandleCore.Value, fieldName);
		}
#if !SL
		internal override object GetServiceTotalSummaryValue(ServiceSummaryItem item) {
			return View.DataControl.DataProviderBase.GetTotalSummaryValue(item);
		}
#endif
		internal override object GetWpfRow(RowData rowData, int listSourceRowIndex) {
			return View.GetWpfRow(rowData.RowHandle, listSourceRowIndex);
		}
		internal override object GetRowValue(RowData rowData) {
			return View.GetRowValue(rowData.RowHandle);
		}
		internal override IList<SummaryItemBase> GetGroupSummaries() {
			return View.DataControl.GetGroupSummaries();
		}
		internal override bool TryGetGroupSummaryValue(RowData rowData, SummaryItemBase item, out object value) {
			return rowData.View.DataControl.DataProviderBase.TryGetGroupSummaryValue(rowData.RowHandle.Value, item, out value);
		}
	}
	public class UnsynchronizedNodeInfo {
		public NodeContainer NodeContainer { get; private set; }
		public int NodeIndex { get; private set; }
		public RowsContainer RowsContainer { get; private set; }
		public UnsynchronizedNodeInfo(RowsContainer dataContainer, NodeContainer nodeContainer, int nodeIndex) {
			this.NodeContainer = nodeContainer;
			this.NodeIndex = nodeIndex;
			this.RowsContainer = dataContainer;
		}
	}
	public class FreeRowDataInfo {
		public RowDataBase RowData { get; set; }
		public RowsContainer DataContainer { get; set; }
		public FreeRowDataInfo(RowsContainer dataContainer, RowDataBase rowData) {
			this.RowData = rowData;
			this.DataContainer = dataContainer;
		}
	}
	public class SynchronizationQueues {
		readonly Queue<UnsynchronizedNodeInfo> unsynchronizedNodes = new Queue<UnsynchronizedNodeInfo>();
		readonly LinkedList<FreeRowDataInfo> freeRowDataQueue = new LinkedList<FreeRowDataInfo>();
		readonly LinkedList<FreeRowDataInfo> freeGroupRowDataQueue = new LinkedList<FreeRowDataInfo>();
		readonly LinkedList<FreeRowDataInfo> freeGroupSummaryRowDataQueue = new LinkedList<FreeRowDataInfo>();
		public Queue<UnsynchronizedNodeInfo> UnsynchronizedNodes { get { return unsynchronizedNodes; } }
		public LinkedList<FreeRowDataInfo> FreeRowDataQueue { get { return freeRowDataQueue; } }
		public LinkedList<FreeRowDataInfo> FreeGroupRowDataQueue { get { return freeGroupRowDataQueue; } }
		public LinkedList<FreeRowDataInfo> FreeGroupSummaryRowDataQueue { get { return freeGroupSummaryRowDataQueue; } }
		List<LinkedList<FreeRowDataInfo>> allFreeQueues;
		internal List<LinkedList<FreeRowDataInfo>> AllFreeQueues {
			get {
				if(allFreeQueues == null)
					allFreeQueues = new List<LinkedList<FreeRowDataInfo>>() { FreeRowDataQueue, FreeGroupRowDataQueue, FreeGroupSummaryRowDataQueue };
				return allFreeQueues;
			}
		}
		public void SynchronizeUnsynchronizedNodes() {
			while(UnsynchronizedNodes.Count > 0) {
				UnsynchronizedNodeInfo nodeInfo = UnsynchronizedNodes.Dequeue();
				RowNode node = nodeInfo.NodeContainer.Items[nodeInfo.NodeIndex];
				RowDataBase rowData = null;
				bool reusedRowData = true;
				LinkedList<FreeRowDataInfo> list = node.GetFreeRowDataQueue(this);
				if(list.Count > 0) {
					FreeRowDataInfo freeRowDataInfo = list.First.Value;
					rowData = freeRowDataInfo.RowData;
					if(freeRowDataInfo.DataContainer != nodeInfo.RowsContainer) {
						freeRowDataInfo.DataContainer.Items.Remove(rowData);
					}
					list.RemoveFirst();
				}
				if(rowData == null) {
					rowData = node.CreateRowData();
					reusedRowData = false;
				}
				rowData.SetVisibleIndex(nodeInfo.NodeIndex);
				rowData.AssignVirtualizedRowData(nodeInfo.RowsContainer, nodeInfo.NodeContainer, node, reusedRowData);
				if(!nodeInfo.RowsContainer.Items.Contains(rowData))
					nodeInfo.RowsContainer.Items.Add(rowData);
			}
		}
		public virtual void Clear() {
			foreach(LinkedList<FreeRowDataInfo> queue in AllFreeQueues)
				ClearCore(queue);
		}
		protected void ClearCore(LinkedList<FreeRowDataInfo> list) {
			while(list.Count > 0) {
				FreeRowDataInfo info = list.First.Value;
				if(info.RowData != null)
					HierarchyPanel.DetachItem(info.RowData);
				list.RemoveFirst();
			}
		}
#if DEBUGTEST
		public void Print() {
			PrintCore();
			System.Diagnostics.Debug.WriteLine("");
		}
		protected virtual void PrintCore() {
			System.Diagnostics.Debug.WriteLine("UnsychronizedNodes:");
			foreach(UnsynchronizedNodeInfo nodeInfo in UnsynchronizedNodes) {
				System.Diagnostics.Debug.WriteLine(string.Format("NodeContainer={0}, RowsContainer={1}, Index={2}, ", nodeInfo.NodeContainer.GetHashCode(), nodeInfo.RowsContainer.GetHashCode(), nodeInfo.NodeIndex));
				nodeInfo.NodeContainer.Items[nodeInfo.NodeIndex].PrintOnlySelf();
			}
			PrintFreeRowDataQueue(FreeRowDataQueue, "FreeRowDataQueue");
			PrintFreeRowDataQueue(FreeGroupRowDataQueue, "FreeGroupRowDataQueue");
		}
		protected void PrintFreeRowDataQueue(LinkedList<FreeRowDataInfo> list, string name) {
			System.Diagnostics.Debug.WriteLine(name + ":");
			foreach(FreeRowDataInfo freeRowDataInfo in list) {
				System.Diagnostics.Debug.WriteLine(string.Format("DataContainer={0}, ", freeRowDataInfo.DataContainer.GetHashCode()));
				freeRowDataInfo.RowData.PrintOnlySelf();
			}
		}
		protected virtual string GetContainerSpecificString() {
			return string.Empty;
		}
#endif
	}
}
