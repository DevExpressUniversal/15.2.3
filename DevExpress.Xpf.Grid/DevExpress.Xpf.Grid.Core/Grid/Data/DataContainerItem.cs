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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using System.Text;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class RowsContainer : DependencyObject, IItemsContainer {
		protected static readonly DependencyPropertyKey ItemsPropertyKey;
		public static readonly DependencyProperty ItemsProperty;
		protected static readonly DependencyPropertyKey RowsLocationPropertyKey;
		public static readonly DependencyProperty AnimationProgressProperty;
		static RowsContainer() {
			Type ownerType = typeof(RowsContainer);
			ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items", typeof(ObservableCollection<RowDataBase>), ownerType, new PropertyMetadata(null));
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			AnimationProgressProperty = DependencyPropertyManager.Register("AnimationProgress", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((RowsContainer)d).OnAnimationProgressChanged()));
		}
		private RowDataBase ownerRowData;
		internal void SetOwnerRowData(RowDataBase value) {
			if(ownerRowData == value)
				return;
			if(ownerRowData != null && ownerRowData.RowsContainer == this) {
				ownerRowData.RowsContainer = null;
			}
			ownerRowData = value;
		}
		internal abstract MasterRowsContainer MasterRootRowsContainer { get; }
		internal abstract SynchronizationQueues SynchronizationQueues { get; }
		public double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
		protected RowsContainer() {
		}
		class RowsContainerItemsCollection : VersionedObservableCollection<RowDataBase> {
			readonly RowsContainer dataContainerItem;
			public RowsContainerItemsCollection(RowsContainer dataContainerItem, Guid cacheVersion)
				: base(cacheVersion) {
				this.dataContainerItem = dataContainerItem;
			}
			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
				base.OnCollectionChanged(e);
				HierarchyChangedEventArgs changedEventArgs;
				if(e.Action == NotifyCollectionChangedAction.Remove) { 
					changedEventArgs = new HierarchyChangedEventArgs(HierarchyChangeType.ItemRemoved, (IItem)e.OldItems[0]);
				} else {
					changedEventArgs = HierarchyChangedEventArgs.Default;
				}
				dataContainerItem.MasterRootRowsContainer.RaiseHierarchyChanged(changedEventArgs);
			}
		}
		internal virtual Guid GetCacheVersion() {
			return Guid.Empty;
		}
		public ObservableCollection<RowDataBase> Items {
			get { return (ObservableCollection<RowDataBase>)GetValue(ItemsProperty); }
			internal set { this.SetValue(ItemsPropertyKey, value); }
		}
		public void RaiseItemsRemoved(IEnumerable items) {
			if (items != null) {
				foreach (IItem item in items) {
					MasterRootRowsContainer.RaiseHierarchyChanged(new HierarchyChangedEventArgs(HierarchyChangeType.ItemRemoved, item));
					RaiseItemsRemoved(item.ItemsContainer.Items);
				}
			}
		}
		internal virtual void Synchronize(NodeContainer nodeContainer) {
			MasterRootRowsContainer.TreeBuilder.Synchronize(this, nodeContainer);
		}
		internal virtual bool BaseSyncronize(NodeContainer nodeContainer) {
			return false;
		}
		internal virtual void StoreFreeData() {
			if(Items == null)
				return;
			foreach(RowDataBase rowData in Items) {
				rowData.StoreAsFreeData(this);
			}
		}
		void OnAnimationProgressChanged() {
#if DEBUGTEST
			EventLog.Default.AddEvent(new DependencyPropertyValueSnapshot<RowsContainer, double>(AnimationProgressProperty, this, AnimationProgress));
#endif
			MasterRootRowsContainer.RaiseHierarchyChanged(HierarchyChangedEventArgs.Default);
		}
		internal void InitItemsCollection() {
			Items = new RowsContainerItemsCollection(this, GetCacheVersion());
		}
		public void StoreFreeRowData(RowNode node, RowDataBase rowData) {
			VerifyVisualRootTreeBuilder();
			FreeRowDataInfo rowDataInfo = FindRowDataInfo(node.GetFreeRowDataQueue(SynchronizationQueues), rowData);
			if(rowDataInfo != null) rowDataInfo.DataContainer = this;
			else node.GetFreeRowDataQueue(SynchronizationQueues).AddLast(new FreeRowDataInfo(this, rowData));
		}
		public void UnstoreFreeRowData(RowNode node, RowDataBase rowData) {
			VerifyVisualRootTreeBuilder();
			FreeRowDataInfo rowDataInfo = FindRowDataInfo(node.GetFreeRowDataQueue(SynchronizationQueues), rowData);
			if(rowDataInfo != null) node.GetFreeRowDataQueue(SynchronizationQueues).Remove(rowDataInfo);
		}
		static FreeRowDataInfo FindRowDataInfo(LinkedList<FreeRowDataInfo> list, RowDataBase rowData) {
			foreach(FreeRowDataInfo item in list)
				if(item.RowData == rowData)
					return item;
			return null;
		}
		void VerifyVisualRootTreeBuilder() {
			if(!(MasterRootRowsContainer.TreeBuilder is VisualDataTreeBuilder))
				throw new InvalidOperationException("This method is valid only when used inside VisualTreeBuilder");
		}
		internal virtual RowsContainerSyncronizerBase CreateRowsContainerSyncronizer() {
			return new RowsContainerSyncronizer(this);
		}
		internal void Clear() {
			foreach(var item in Items) {
				HierarchyPanel.DetachItem(item);
				if(item.RowsContainer != null)
					item.RowsContainer.Clear();
			}
			Items.Clear();
		}
		internal IEnumerable GetEnumerable() {
			foreach(RowDataBase rowData in Items)
				yield return rowData;
			foreach(LinkedList<FreeRowDataInfo> list in SynchronizationQueues.AllFreeQueues)
				foreach(FreeRowDataInfo rowDataInfo in list)
					yield return rowDataInfo.RowData;
		}
		#region IItemsContainer Members
		IList<IItem> IItemsContainer.Items {
			get { return new SimpleBridgeList<IItem, RowDataBase>(Items, rowData => rowData); }
		}
		Size IItemsContainer.DesiredSize { get; set; }
		Size IItemsContainer.RenderSize { get; set; }
		#endregion
		#region debug print
#if DEBUGTEST
		public void Print() {
			Print(0);
			System.Diagnostics.Debug.WriteLine("");
		}
		public void Print(int level) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < level; i++) {
				sb.Append("\t");
			}
			sb.Append(string.Format("Type={0}, Items.Count={1}, ", GetType().Name, Items.Count));
			sb.Append(GetContainerSpecificString());
			sb.Append(string.Format("HashCode={0}, ", GetHashCode()));
			System.Diagnostics.Debug.WriteLine(sb.ToString());
			for(int i = 0; i < Items.Count; i++) {
				Items[i].Print(level + 1);
			}
		}
		protected virtual string GetContainerSpecificString() {
			return string.Empty;
		}
#endif
		#endregion
	}
	public class DataRowsContainer : RowsContainer {
		internal const string IsGroupRowsContainerPropertyName = "IsGroupRowsContainer";
		protected static readonly DependencyPropertyKey IsGroupRowsContainerPropertyKey;
		public static readonly DependencyProperty IsGroupRowsContainerProperty;
		public static readonly DependencyProperty GroupLevelProperty;
		static readonly DependencyPropertyKey GroupLevelPropertyKey;
		static DataRowsContainer() {
			Type ownerType = typeof(DataRowsContainer);
			IsGroupRowsContainerPropertyKey = DependencyPropertyManager.RegisterReadOnly(IsGroupRowsContainerPropertyName, typeof(bool), ownerType, new PropertyMetadata(false));
			IsGroupRowsContainerProperty = IsGroupRowsContainerPropertyKey.DependencyProperty;
			GroupLevelPropertyKey = DependencyPropertyManager.RegisterReadOnly("GroupLevel", typeof(int), ownerType, new PropertyMetadata(0));
			GroupLevelProperty = GroupLevelPropertyKey.DependencyProperty;
		}
		public bool IsGroupRowsContainer {
			get { return (bool)GetValue(IsGroupRowsContainerProperty); }
			private set { this.SetValue(IsGroupRowsContainerPropertyKey, value); }
		}
		public int GroupLevel {
			get { return (int)GetValue(GroupLevelProperty); }
			private set { this.SetValue(GroupLevelPropertyKey, value); }
		}
		protected readonly DataTreeBuilder treeBuilder;
		protected internal DataViewBase View { get { return TreeBuilder.View; } }
		internal DataTreeBuilder TreeBuilder { get { return treeBuilder; } }
		internal override MasterRowsContainer MasterRootRowsContainer { get { return TreeBuilder.MasterRootRowsContainer; } }
		internal override SynchronizationQueues SynchronizationQueues { get { return ((VisualDataTreeBuilder)TreeBuilder).SynchronizationQueues; } }
		public DataRowsContainer(DataTreeBuilder treeBuilder, int level)
			: base() {
			this.treeBuilder = treeBuilder;
			this.GroupLevel = level;
			InitItemsCollection();
		}
		internal override bool BaseSyncronize(NodeContainer nodeContainer) {
			if(nodeContainer == null) return false;
			DataNodeContainer dataNodeContainer = (DataNodeContainer)nodeContainer;
			GroupLevel = dataNodeContainer.GroupLevel;
			bool oldIsGroupRowsContainer = IsGroupRowsContainer;
			IsGroupRowsContainer = dataNodeContainer.IsGroupRowsContainer;
			return (View.CacheVersion != ((ISupportCacheVersion)Items).CacheVersion) || (oldIsGroupRowsContainer != IsGroupRowsContainer);
		}
		internal override Guid GetCacheVersion() {
			return View.CacheVersion;
		}
#if DEBUGTEST
		protected override string GetContainerSpecificString() {
			return string.Format("View: {0} ", View.GetHashCode());
		}
#endif
	}
	public class DetailRowsContainer : DataRowsContainer, IDetailRootItemsContainer {
		public DetailRowsContainer(DataTreeBuilder treeBuilder, int level)
			: base(treeBuilder, level) {
		}
		double IItemsContainer.AnimationProgress { get { return 1; } }
		internal override void Synchronize(NodeContainer nodeContainer) {
			TreeBuilder.ClearRowsCache();
			base.Synchronize(nodeContainer);
		}
		internal override void StoreFreeData() {
			TreeBuilder.ClearRowsCache();
			base.StoreFreeData();
		}
	}
	public class MasterRowsContainer : DetailRowsContainer, IRootItemsContainer {
		DataViewBase focusedView;
		public MasterRowsContainer(DataTreeBuilder treeBuilder, int level)
			: base(treeBuilder, level) {
			this.focusedView = treeBuilder.View;
		}
		protected DataControlBase MasterDataControl { get { return TreeBuilder.View.DataControl; } }
		public DataViewBase FocusedView {
			get { return focusedView; }
			internal set {
				if(focusedView == value) return;
				DataViewBase oldFocusedView = focusedView;
				focusedView = value;
				oldFocusedView.ProcessFocusedViewChange();
				focusedView.ProcessFocusedViewChange();
				focusedView.RootView.RaiseFocusedViewChanged(oldFocusedView, focusedView);
			}
		}
		#region IRootItemsContainer Members
		public event HierarchyChangedEventHandler HierarchyChanged;
		public void RaiseHierarchyChanged(HierarchyChangedEventArgs eventArgs) {
			if(HierarchyChanged != null)
				HierarchyChanged(this, eventArgs);
		}
		double IRootItemsContainer.ScrollItemOffset {
			get { return View.DataPresenter.ScrollItemOffset; }
		}
		IItem IRootItemsContainer.ScrollItem {
			get { return View.DataPresenter.GetRowDataToScroll(); }
		}
		#endregion
		#region cascade updates
		List<RowData> rowsToUpdate = new List<RowData>();
		internal List<RowData> RowsToUpdate { get { return rowsToUpdate; } }
#if SL
		DispatcherTimer timer;
#endif
		int oldStartVisibleIndex;
		bool isDirect = false;
		internal void UpdatePostponedData(bool updateStartIndex, bool updateImmediately) {
			if(!(View.ViewBehavior.AllowCascadeUpdate || View.GetAllowGroupSummaryCascadeUpdate) || View.DataPresenter == null || View.DataPresenter.AdjustmentInProgress || View.PostponedNavigationInProgress)
				return;
			ProcessInvisibleItems();
			rowsToUpdate.Clear();
			VirtualItemsEnumerator en = View.CreateAllRowsEnumerator();
			while(en.MoveNext()) {
				if(en.CurrentData is RowData) {
					rowsToUpdate.Add((RowData)en.CurrentData);
				}
			}
			ProcessInvisibleItems();
			int newStartVisibleIndex = View.DataPresenter.ScrollOffset;
			if(updateStartIndex && newStartVisibleIndex != oldStartVisibleIndex) {
				isDirect = oldStartVisibleIndex < newStartVisibleIndex;
				oldStartVisibleIndex = newStartVisibleIndex;
			}
#if !SL
			if(!updateImmediately)
				InvokeUpdatePostponedDataItem();
			else
				if(!View.DataPresenter.CanScrollWithAnimation) UpdatePostponedDataItem();
#else       
			if(timer != null) InvokeUpdatePostponedDataItem();
			if(timer == null) {
				timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromMilliseconds(0);
				timer.Tick += (d, e) => {
					timer.Stop();
					timer = null;
					InvokeUpdatePostponedDataItem();
				};
			}
			if(!timer.IsEnabled) timer.Start();
#endif
		}
		void InvokeUpdatePostponedDataItem() {
			if(rowsToUpdate.Count > 0)
#if !SL
				View.Dispatcher.BeginInvoke(new Action(UpdatePostponedDataItem), DispatcherPriority.Background);
#else
				View.Dispatcher.BeginInvoke(new Action(UpdatePostponedDataItem));
#endif
		}
		void ProcessInvisibleItems() {
			foreach(RowData data in rowsToUpdate) {
				data.UpdateIsDirty();
			}
		}
#if DEBUGTEST
		internal
#endif
		void UpdatePostponedDataItem() {
			rowsToUpdate.Sort(CompareRowData);
			RowData data;
			do {
				if(rowsToUpdate.Count == 0)
					return;
				data = rowsToUpdate[0];
				rowsToUpdate.RemoveAt(0);
			} while(!data.IsRowInView() || data.IsReady);
			data.RefreshData();
#if SL
			if(timer == null)
#endif
			InvokeUpdatePostponedDataItem();
		}
#if DEBUGTEST
		internal IList<RowData> GetSortedRowsToUpdate() {
			List<RowData> rows = new List<RowData>(rowsToUpdate);
			rows.Sort(CompareRowData);
			return rows;
		}
#endif
		List<int> rowData1Indices = new List<int>();
		List<int> rowData2Indices = new List<int>();
		int CompareRowData(RowData rowData1, RowData rowData2) {
			int result;
			if(rowData1.View == rowData2.View) { 
				result = Comparer<int>.Default.Compare(rowData1.ControllerVisibleIndex, rowData2.ControllerVisibleIndex);
			} else {
				CollectParentIndices(rowData1, rowData1Indices);
				CollectParentIndices(rowData2, rowData2Indices);
				result = 0;
				int count = Math.Min(rowData1Indices.Count, rowData2Indices.Count);
				for(int i = 1; i <= count; i++) {
					result = Comparer<int>.Default.Compare(rowData1Indices[rowData1Indices.Count - i], rowData2Indices[rowData2Indices.Count - i]);
					if(result != 0)
						break;
				}
				if(result == 0)
					result = Comparer<int>.Default.Compare(rowData1Indices.Count, rowData2Indices.Count);
			}
			if(!isDirect)
				return -result;
			return result;
		}
		static void CollectParentIndices(RowData rowData, List<int> indices) {
			indices.Clear();
			rowData.View.DataControl.EnumerateThisAndParentDataControls((dataControl, visibleIndex) => indices.Add(visibleIndex), rowData.ControllerVisibleIndex);
		}
		#endregion
	}
	public abstract class RowsContainerSyncronizerBase {
		protected readonly RowsContainer dataContainer;
		Dictionary<object, RowDataBase> matchedItems = new Dictionary<object, RowDataBase>();
		bool changed = false;
		protected int SyncronizeStartIndex { get { return 0; } }
		protected ObservableCollection<RowDataBase> Items { get { return dataContainer.Items; } set { dataContainer.Items = value; } }
		protected RowsContainerSyncronizerBase(RowsContainer dataContainer) {
			this.dataContainer = dataContainer;
		}
		public void Syncronize(NodeContainer nodeContainer) {
			bool shouldRecreateItems = dataContainer.BaseSyncronize(nodeContainer);
			if(Items == null || shouldRecreateItems) {
				dataContainer.StoreFreeData();
				dataContainer.RaiseItemsRemoved(Items);
				dataContainer.InitItemsCollection();
			}
			BeforeSyncronize(nodeContainer);
			for(int i = SyncronizeStartIndex; i < nodeContainer.Items.Count; i++) {
				RowDataBase data = GetRowData(nodeContainer, i);
				AssignRowDataFromNodeContainer(nodeContainer, data, i);
			}
			AfterSyncronize();
		}
		void SetRowDataVisibleIndex(RowDataBase rowData, int nodeIndex) {
			int currentVisibleIndex = (rowData as ISupportVisibleIndex).VisibleIndex;
			if(currentVisibleIndex != nodeIndex) {
				changed = true;
				rowData.SetVisibleIndex(nodeIndex);
			}
		}
		RowDataBase GetRowData(NodeContainer nodeContainer, int nodeIndex) {
			return GetItem(nodeContainer.Items[nodeIndex]);
		}
		void AssignRowDataFromNodeContainer(NodeContainer nodeContainer, RowDataBase rowData, int nodeIndex) {
			if(rowData != null) {
				SetRowDataVisibleIndex(rowData, nodeIndex);
				rowData.AssignVirtualizedRowData(dataContainer, nodeContainer, nodeContainer.Items[nodeIndex], false);
			}
			else {
				dataContainer.SynchronizationQueues.UnsynchronizedNodes.Enqueue(new UnsynchronizedNodeInfo(dataContainer, nodeContainer, nodeIndex));
			}
		}
		void AfterSyncronize() {
			SetInvisibleIndex(GetUnmatchedItems(), true);
			if(changed)
				((VersionedObservableCollection<RowDataBase>)Items).RaiseCollectionChanged();
		}
		void SetInvisibleIndex(IEnumerable items, bool storeAsFreeData) {
			foreach(RowDataBase rowData in items) {
				if(!OrderPanelBase.IsInvisibleIndex((rowData as ISupportVisibleIndex).VisibleIndex)) {
					changed = true;
					rowData.SetVisibleIndex(OrderPanelBase.InvisibleIndex);
					SetInvisibleIndex(((IItem)rowData).ItemsContainer.Items, false);
				}
				if(storeAsFreeData) {
					rowData.StoreAsFreeData(dataContainer);
					if(rowData.RequireUpdateRow) rowData.ClearRow();
				}
			}
		}
		void BeforeSyncronize(NodeContainer nodeContainer) {
			foreach(RowDataBase data in Items) {
				bool matchFound = false;
				foreach(RowNode node in nodeContainer.Items) {
					if(node.IsMatchedRowData(data)) {
						matchedItems.Add(node.MatchKey, data);
						matchFound = true;
						break;
					}
				}
				if(!matchFound)
					EnqueueUnmatchedItem(data);
			}
		}
		RowDataBase GetItem(RowNode rowNode) {
			RowDataBase matchedData = null;
			if(matchedItems.TryGetValue(rowNode.MatchKey, out matchedData))
				return matchedData;
			return DequeueUnmatchedItem(rowNode.MatchKey);
		}
		protected abstract void EnqueueUnmatchedItem(RowDataBase rowData);
		protected abstract RowDataBase DequeueUnmatchedItem(object matchKey);
		protected abstract IEnumerable<RowDataBase> GetUnmatchedItems();
	}
	public class RowsContainerSyncronizer : RowsContainerSyncronizerBase {
		readonly Queue<RowDataBase> unmatchedRowItems = new Queue<RowDataBase>();
		readonly Queue<RowDataBase> unmatchedGroupRowItems = new Queue<RowDataBase>();
		readonly Queue<RowDataBase> unmatchedGroupSummaryRowItems = new Queue<RowDataBase>();
		public RowsContainerSyncronizer(RowsContainer dataContainer)
			: base(dataContainer) {
		}
		protected override IEnumerable<RowDataBase> GetUnmatchedItems() {
			List<RowDataBase> list = new List<RowDataBase>();
			list.AddRange(unmatchedRowItems);
			list.AddRange(unmatchedGroupRowItems);
			list.AddRange(unmatchedGroupSummaryRowItems);
			return list;
		}
		Queue<RowDataBase> GetQueue(object matchKey) {
			if(matchKey is GroupSummaryRowKey)
				return unmatchedGroupSummaryRowItems;
			RowHandle rowHandle = (RowHandle)matchKey;
			return rowHandle.Value < 0 && rowHandle.Value != DataControlBase.NewItemRowHandle ? unmatchedGroupRowItems : unmatchedRowItems;
		}
		protected override void EnqueueUnmatchedItem(RowDataBase rowData) {
			GetQueue(rowData.MatchKey).Enqueue(rowData);
		}
		protected override RowDataBase DequeueUnmatchedItem(object matchKey) {
			Queue<RowDataBase> queue = GetQueue(matchKey);
			if(queue.Count > 0) {
				RowDataBase result = queue.Dequeue();
				return result;
			}
			return null;
		}
	}
	public class GroupSummaryRowKey {
		public GroupSummaryRowKey(RowHandle rowHandle, int level) {
			this.RowHandle = rowHandle;
			this.Level = level;
		}
		public RowHandle RowHandle { get; private set; }
		public int Level { get; private set; }
		public override int GetHashCode() {
			return RowHandle.Value;
		}
		public override bool Equals(object obj) {
			GroupSummaryRowKey key = obj as GroupSummaryRowKey;
			if(key == null)
				return false;
			return RowHandle.Equals(key.RowHandle) && Level == key.Level;
		}
	}
}
