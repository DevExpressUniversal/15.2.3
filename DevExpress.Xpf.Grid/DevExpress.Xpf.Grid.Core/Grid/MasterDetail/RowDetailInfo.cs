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
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Native {
	public sealed class RowDetailContainer {
		public DetailInfoWithContent RootDetailInfo { get; set; }
		public DataControlBase MasterDataControl { get; private set; }
		public int MasterListIndex { get; internal set; }
		public object Row { get; internal set; }
		public RowDetailContainer(DataControlBase masterDataControl, object row) {
			this.MasterDataControl = masterDataControl;
			this.Row = row;
		}
		public void Clear() {
			EnumerateInfoWithContent(x => x.Clear());
		}
		public void Detach() {
			EnumerateInfoWithContent(x => x.Detach());
		}
		public void RemoveFromDetailClones() {
			EnumerateInfoWithContent(x => x.RemoveFromDetailClones());
		}
		void EnumerateInfoWithContent(Action<DetailInfoWithContent> action) {
			foreach(RowDetailInfoBase info in RootDetailInfo.GetRowDetailInfoEnumerator()) {
				DetailInfoWithContent infoWithContent = info as DetailInfoWithContent;
				if(infoWithContent != null)
					action(infoWithContent);
			}
		}
	}
	public abstract class RowDetailInfoBase {
		bool isExpanded;
#if DEBUGTEST
		internal static int InstanceCount { get; set; }
#endif
		protected RowDetailInfoBase() {
#if DEBUGTEST
			InstanceCount++;
#endif
		}
		protected abstract void OnExpanded();
		protected abstract void OnCollapsed();
		public virtual bool IsExpanded {
			get { return isExpanded; }
			set {
				if(isExpanded == value) return;
				isExpanded = value;
				if(isExpanded)
					OnExpanded();
				else
					OnCollapsed();
			}
		}
		public virtual bool IsDetailRowExpanded(DetailDescriptorBase descriptor) {
			return IsExpanded;
		}
		public virtual void SetDetailRowExpanded(bool expand, DetailDescriptorBase descriptor) {
			IsExpanded = expand;
		}
		public abstract NodeContainer GetNodeContainer();
		public abstract RowsContainer GetRowsContainerAndUpdateMasterRowData(RowData masterRowData);
		public abstract int CalcTotalLevel();
		public abstract int CalcRowsCount();
		public abstract int CalcVisibleDataRowCount();
		public abstract bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex);
		public abstract DataViewBase FindFirstDetailView();
		public abstract DataViewBase FindLastInnerDetailView();
		public abstract DataControlBase FindVisibleDetailDataControl();
		public abstract DetailDescriptorBase FindVisibleDetailDescriptor();
		public DataControlBase FindDetailDataControl(DataControlDetailDescriptor descriptor) {
			foreach(RowDetailInfoBase detailInfo in GetRowDetailInfoEnumerator()) {
				DataControlDetailInfo dataControlDetailInfo = detailInfo as DataControlDetailInfo;
				if(dataControlDetailInfo != null && (descriptor == null || dataControlDetailInfo.DataControlDetailDescriptor == descriptor))
					return dataControlDetailInfo.DataControl;
			}
			return null;
		}
		public void OnUpdateRow(object row) {
			foreach(RowDetailInfoBase detailInfo in GetRowDetailInfoEnumerator())
				detailInfo.UpdateRow(row);
		}
		internal abstract IEnumerable<RowDetailInfoBase> GetRowDetailInfoEnumerator();
		protected abstract void UpdateRow(object row);
		internal virtual void RemoveFromDetailClones() { }
	}
	public enum DetailNodeKind { DetailHeader, DetailContent, ColumnHeaders, NewItemRow, DataRowsContainer, TotalSummary, FixedTotalSummary, TabHeaders }
	public class DetailSynchronizationQueues : SynchronizationQueues {
		readonly Dictionary<DetailNodeKind, LinkedList<FreeRowDataInfo>> detailRowQueues = new Dictionary<DetailNodeKind, LinkedList<FreeRowDataInfo>>();
		public LinkedList<FreeRowDataInfo> GetSynchronizationQueue(DetailNodeKind detailNodeKind) {
			LinkedList<FreeRowDataInfo> result;
			if(!detailRowQueues.TryGetValue(detailNodeKind, out result)) {
				detailRowQueues[detailNodeKind] = (result = new LinkedList<FreeRowDataInfo>());
			}
			return result;
		}
		public override void Clear() {
			base.Clear();
			foreach(LinkedList<FreeRowDataInfo> list in detailRowQueues.Values) {
				ClearCore(list);
			}
		}
#if DEBUGTEST
		protected override void PrintCore() {
			base.PrintCore();
			foreach(KeyValuePair<DetailNodeKind, LinkedList<FreeRowDataInfo>> pair in detailRowQueues) {
				PrintFreeRowDataQueue(pair.Value, pair.Key.ToString());
			}
		}
#endif
	}
	public abstract class DetailInfoWithContent : RowDetailInfoBase {
		#region inner classes
		internal abstract class DetailRowNode : RowNode {
			bool isDataExpanded;
			readonly Func<DetailNodeKind, RowDataBase> createRowDataDelegate;
			internal readonly DetailNodeKind detailNodeKind;
			protected readonly DetailInfoWithContent owner;
			internal RowsContainer childrenRowsContainer { get; private set; }
			internal RowDataBase CurrentRowData { get; set; }
			public DetailRowNode(DetailInfoWithContent owner, DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate) {
				this.detailNodeKind = detailNodeKind;
				this.createRowDataDelegate = createRowDataDelegate;
				this.owner = owner;
			}
			public void UpdateExpandInfoEx(NodeContainer childrenNodeContainer, RowsContainer childrenRowsContainer, int startVisibleIndex, bool isRowVisible) {
				NodesContainer = childrenNodeContainer;
				this.childrenRowsContainer = childrenRowsContainer;
				this.isDataExpanded = childrenNodeContainer != null;
				UpdateExpandInfo(startVisibleIndex, isRowVisible);
			}
#if DEBUGTEST
			public
#else
			protected
#endif
			override bool IsDataExpanded { get { return isDataExpanded; } }
			public override object MatchKey { get { return detailNodeKind; } }
			internal override LinkedList<FreeRowDataInfo> GetFreeRowDataQueue(SynchronizationQueues synchronizationQueues) {
				return ((DetailSynchronizationQueues)synchronizationQueues).GetSynchronizationQueue(detailNodeKind);
			}
			internal override RowDataBase GetRowData() {
				return CurrentRowData;
			}
			internal override FrameworkElement GetRowElement() {
				return CurrentRowData == null ? null : CurrentRowData.WholeRowElement;
			}
			internal override RowDataBase CreateRowData() {
				return createRowDataDelegate(detailNodeKind);
			}
			internal abstract void AssignToRowData(RowDataBase rowData, RowDataBase masterRowData);
			internal override bool IsFixedNode { get { return owner.IsFixedNode(this); } }
		}
		internal class DetailHeaderRowNode : DetailRowNode {
			internal bool ShowBottomLine { get; set; }
			public DetailHeaderRowNode(DetailInfoWithContent owner, DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate)
				: base(owner, detailNodeKind, createRowDataDelegate) { }
			internal override void AssignToRowData(RowDataBase rowData, RowDataBase masterRowData) {
				if(rowData is DetailRowData)
					(rowData as DetailRowData).SetMasterView(masterRowData.View);
				DetailHeaderControlBase headerControl = (DetailHeaderControlBase)rowData.WholeRowElement;
				headerControl.ShowBottomLine = ShowBottomLine;
			}
		}
		internal class DetailNodeContainer : NodeContainer {
			readonly DetailInfoWithContent detailInfo;
			public DetailNodeContainer(DetailInfoWithContent detailInfo) {
				this.detailInfo = detailInfo;
			}
			protected override IEnumerator<RowNode> GetRowDataEnumerator() {
				return detailInfo.GetRowNodesEnumerator(StartScrollIndex);
			}
			internal override RowNode GetNodeToScroll() {
				RowNode node = detailInfo.GetNodeToScroll();
				return node != null ? node.GetNodeToScroll() : null;
			}
		}
		class DetailRowsContainerSyncronizer : RowsContainerSyncronizerBase {
			readonly Dictionary<DetailNodeKind, RowDataBase> unmatchedItems = new Dictionary<DetailNodeKind, RowDataBase>();
			public DetailRowsContainerSyncronizer(DetailRowsContainer dataContainer)
				: base(dataContainer) {
			}
			protected override IEnumerable<RowDataBase> GetUnmatchedItems() {
				return unmatchedItems.Values;
			}
			protected override void EnqueueUnmatchedItem(RowDataBase rowData) {
				unmatchedItems.Add((DetailNodeKind)rowData.MatchKey, rowData);
			}
			protected override RowDataBase DequeueUnmatchedItem(object matchKey) {
				DetailNodeKind nodeKind = (DetailNodeKind)matchKey;
				RowDataBase data;
				if(unmatchedItems.TryGetValue(nodeKind, out data)) {
					unmatchedItems.Remove(nodeKind);
					return data;
				}
				return null;
			}
		}
		internal class DetailRowsContainer : RowsContainer, IDetailRootItemsContainer {
			private RowData masterRowData;
			readonly MasterRowsContainer masterRowsContainer;
			readonly DetailDescriptorBase detailDescriptor;
			public DetailRowsContainer(MasterRowsContainer masterRowsContainer, DetailDescriptorBase detailDescriptor) {
				this.masterRowsContainer = masterRowsContainer;
				this.detailDescriptor = detailDescriptor;
				InitItemsCollection();
			}
			internal RowData MasterRowData {
				get { return masterRowData; }
				set {
					if(masterRowData == value)
						return;
					masterRowData = value;
				}
			}
			internal override MasterRowsContainer MasterRootRowsContainer { get { return masterRowsContainer; } }
			internal override SynchronizationQueues SynchronizationQueues { get { return detailDescriptor.SynchronizationQueues; } }
			internal override RowsContainerSyncronizerBase CreateRowsContainerSyncronizer() {
				return new DetailRowsContainerSyncronizer(this);
			}
			double IItemsContainer.AnimationProgress { get { return 1; } }
		}
		public class DetailRowData : RowDataBase {
			readonly DetailNodeKind nodeKind;
			public DetailRowData(DetailNodeKind nodeKind, Func<FrameworkElement> createRowElementDelegate)
				: base(createRowElementDelegate) {
				this.nodeKind = nodeKind;
			}
			internal override object MatchKey { get { return nodeKind; } }
			protected override NotImplementedRowDataReusingStrategy CreateReusingStrategy(Func<FrameworkElement> createRowElementDelegate) {
				return new DetailRowDataReusingStrategy(this, createRowElementDelegate);
			}
			internal virtual void SetMasterView(DataViewBase view) {
				this.View = view;
			}
			internal override void UpdateLineLevel() {
				LineLevel = 0;
				DetailLevel = int.MaxValue;
			}
		}
		internal class DetailColumnsData : ColumnsRowDataBase {
			readonly DetailNodeKind nodeKind;
			public DetailColumnsData(DetailNodeKind nodeKind, DataTreeBuilder treeBuilder, Func<FrameworkElement> createRowElementDelegate)
				: base(treeBuilder, createRowElementDelegate) {
				this.nodeKind = nodeKind;
			}
			internal override object MatchKey { get { return nodeKind; } }
			protected override NotImplementedRowDataReusingStrategy CreateReusingStrategy(Func<FrameworkElement> createRowElementDelegate) {
				return new DetailRowDataReusingStrategy(this, createRowElementDelegate);
			}
			internal override void UpdateLineLevel() {
				bool isLastRow = true;
				if(IsNotLastRow()) {
					LineLevel = 0;
					DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
					return;
				}
				DataViewBase targetView = null;
				int targetVisibleIndex = -1;
				if(View.DataControl.DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
					RowData rowData = targetView.GetRowData(targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex));
					if(rowData != null) {
						LineLevel = GetLineLevel(this, 0, 0);
						DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
						if(isLastRow)
							LineLevel = DetailLevel;
						return;
					}
				}
				LineLevel = 0;
				DetailLevel = 0;
			}
			bool IsNotLastRow() {
				if(View.ShowFixedTotalSummary)
					return true;
				if(nodeKind == DetailNodeKind.ColumnHeaders)
					return View.IsNewItemRowVisible || View.DataControl.VisibleRowCount > 0;
				if(nodeKind == DetailNodeKind.NewItemRow)
					return View.DataControl.VisibleRowCount > 0;
				return false;
			}
		}
		#endregion
		Dictionary<DetailNodeKind, DetailRowNode> nodesCache = new Dictionary<DetailNodeKind, DetailRowNode>();
		DetailNodeContainer nodeContainer;
		DetailRowsContainer rowsContainer;
		protected TableViewBehavior MasterTableViewBehavior { get { return ((ITableView)MasterRootRowsContainer.View).TableViewBehavior; } }
		internal DetailNodeContainer NodeContainer { get { return nodeContainer; } }
		internal DetailRowsContainer RowsContainer { get { return rowsContainer; } }
		protected MasterRowsContainer MasterRootRowsContainer { get { return MasterDataControl.DataView.MasterRootRowsContainer; } }
		internal DataControlBase MasterDataControl { get { return container.MasterDataControl; } }
		protected object Row { get { return container.Row; } }
#if DEBUGTEST
		internal
#endif
		protected int MasterVisibleIndex {
			get {
				int rowHandle = MasterDataControl.DataProviderBase.GetRowHandleByListIndex(container.MasterListIndex);
				return MasterDataControl.DataProviderBase.GetRowVisibleIndexByHandle(rowHandle);
			}
		}
		protected internal readonly RowDetailContainer container;
		protected readonly DetailDescriptorBase detailDescriptor;
		protected DetailInfoWithContent(DetailDescriptorBase detailDescriptor, RowDetailContainer container) {
			this.detailDescriptor = detailDescriptor;
			this.container = container;
		}
		public override NodeContainer GetNodeContainer() {
			return nodeContainer;
		}
		public override RowsContainer GetRowsContainerAndUpdateMasterRowData(RowData masterRowData) {
			UpdateMasterRowData(masterRowData);
			return rowsContainer;
		}
		public void UpdateMasterRowData(RowData masterRowData) {
			if(rowsContainer != null)
				rowsContainer.MasterRowData = masterRowData;
		}
		protected override void OnCollapsed() { }
		protected override void OnExpanded() {
			ForceCreateContainers();
		}
		public void ForceCreateContainers() {
			if(nodeContainer == null)
				nodeContainer = new DetailNodeContainer(this);
			if(rowsContainer == null)
				rowsContainer = new DetailRowsContainer(MasterRootRowsContainer, detailDescriptor);
		}
		internal DetailRowNode GetRowNode(DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate) {
			DetailRowNode node = null;
			if(!nodesCache.TryGetValue(detailNodeKind, out node)) {
				node = CreateDetailRowNode(detailNodeKind, createRowDataDelegate);
				nodesCache[detailNodeKind] = node;
			}
			return node;
		}
		protected RowNode FindNodeByDetailNodeKind(DetailNodeKind detailNodeKind) {
			return nodeContainer.Items.FirstOrDefault(node => (DetailNodeKind)node.MatchKey == detailNodeKind);
		}
		internal virtual DetailRowNode CreateDetailRowNode(DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate) {
			return new DetailHeaderRowNode(this, detailNodeKind, createRowDataDelegate);
		}
		internal RowNode GetNodeToScroll() {
			foreach(DetailNodeKind nodeKind in GetNodeScrollOrder()) {
				RowNode node = FindNodeByDetailNodeKind(nodeKind);
				if(node != null)
					return node;
			}
			return null;
		}
		protected abstract DetailNodeKind[] GetNodeScrollOrder();
		internal virtual bool IsFixedNode(DetailRowNode detailRowNode) { return false; }
		protected DetailHeaderControlBase CreateDetailHeaderControl(Func<DetailHeaderControlBase> createControl) {
			DetailHeaderControlBase control = createControl();
			control.DetailDescriptor = detailDescriptor;
			return control;
		}
		internal IEnumerator<RowNode> GetRowNodesEnumerator(int startVisibleIndex) {
			int nextLevelVisibleRowsCount = GetNextLevelRowCount() - startVisibleIndex;
			int serviceVisibleRowsCount = GetServiceRowsCount() + Math.Min(nextLevelVisibleRowsCount, 0);
			return GetRowNodesEnumeratorCore(startVisibleIndex, nextLevelVisibleRowsCount, serviceVisibleRowsCount);
		}
		internal abstract IEnumerator<RowNode> GetRowNodesEnumeratorCore(int nextLevelStartVisibleIndex, int nextLevelVisibleRowsCount, int serviceVisibleRowsCount);
		public override int CalcTotalLevel() {
			return GetTopServiceRowsCount();
		}
		public sealed override int CalcRowsCount() {
			return IsExpanded ? GetServiceRowsCount() + GetNextLevelRowCount() : 0;
		}
		public sealed override int CalcVisibleDataRowCount() {
			return IsExpanded ? GetVisibleDataRowCount() : 0;
		}
		int GetServiceRowsCount() {
			return GetTopServiceRowsCount() + GetBottomServiceRowsCount();
		}
		protected abstract int GetTopServiceRowsCount();
		protected abstract int GetBottomServiceRowsCount();
		protected abstract int GetNextLevelRowCount();
		protected abstract int GetVisibleDataRowCount();
		protected int CalcHeaderRowCount() {
			return detailDescriptor.ShowHeader ? 1 : 0;
		}
		protected int CalcContentRowCount() {
			return detailDescriptor.ContentTemplate != null ? 1 : 0;
		}
		protected RowNode GetDetailHeaderNode() {
			return GetRowNode(DetailNodeKind.DetailHeader, nodeKind => new DetailRowData(nodeKind, () => CreateDetailHeaderControl(MasterTableViewBehavior.CreateDetailHeaderElement)));
		}
		protected RowNode GetDetailContentNode() {
			return GetRowNode(DetailNodeKind.DetailContent, nodeKind => new DetailRowData(nodeKind, () => CreateDetailHeaderControl(MasterTableViewBehavior.CreateDetailContentElement)));
		}
		internal override IEnumerable<RowDetailInfoBase> GetRowDetailInfoEnumerator() {
			yield return this;
		}
		protected override void UpdateRow(object row) {
			container.Row = row;
		}
		public virtual void Clear() {
			RowsContainer.Clear();
		}
		public virtual void Detach() {
		}
	}
	public class ContentDetailInfo : DetailInfoWithContent {
		ContentDetailDescriptor ContentDetailDescriptor { get { return (ContentDetailDescriptor)detailDescriptor; } }
		public ContentDetailInfo(ContentDetailDescriptor contentDetailDescriptor, RowDetailContainer container)
			: base(contentDetailDescriptor, container) {
		}
		internal override IEnumerator<RowNode> GetRowNodesEnumeratorCore(int nextLevelStartVisibleIndex, int nextLevelVisibleRowsCount, int serviceVisibleRowsCount) {
			bool hasContentRow = detailDescriptor.ContentTemplate != null;
			if(detailDescriptor.ShowHeader && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				DetailHeaderRowNode rowNode = (DetailHeaderRowNode)GetDetailHeaderNode();
				rowNode.ShowBottomLine = !hasContentRow || serviceVisibleRowsCount == 0;
				yield return rowNode;
			}
			if(serviceVisibleRowsCount > 0) {
				DetailHeaderRowNode rowNode = (DetailHeaderRowNode)GetDetailContentNode();
				rowNode.ShowBottomLine = true;
				yield return rowNode;
			}
		}
		protected override DetailNodeKind[] GetNodeScrollOrder() {
			return new DetailNodeKind[] { DetailNodeKind.DetailContent, DetailNodeKind.DetailHeader };
		}
		protected override int GetTopServiceRowsCount() {
			return CalcHeaderRowCount() + 1;
		}
		protected override int GetBottomServiceRowsCount() {
			return 0;
		}
		protected override int GetNextLevelRowCount() {
			return 0; 
		}
		protected override int GetVisibleDataRowCount() {
			return 0;
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		public override DataViewBase FindFirstDetailView() {
			return null;
		}
		public override DataViewBase FindLastInnerDetailView() {
			return null;
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor() {
			return ContentDetailDescriptor; 
		}
		public override DataControlBase FindVisibleDetailDataControl() {
			return null;
		}
	}
	public class TabsDetailInfo : DetailInfoWithContent {
		#region inner classes
		internal class TabHeadersRowNode : DetailRowNode {
			public TabHeadersRowNode(TabsDetailInfo owner, Func<DetailNodeKind, RowDataBase> createRowDataDelegate)
				: base(owner, DetailNodeKind.TabHeaders, createRowDataDelegate) {
			}
			internal override void AssignToRowData(RowDataBase rowData, RowDataBase masterRowData) {
				((TabHeadersRowData)rowData).AssignFrom((TabsDetailInfo)owner);
				if(rowData is TabHeadersRowData)
					(rowData as TabHeadersRowData).SetMasterView(masterRowData.View);
			}
		}
		public class TabHeadersRowData : DetailRowData {
			public int SelectedTabIndex {
				get { return (int)GetValue(SelectedTabIndexProperty); }
				set { SetValue(SelectedTabIndexProperty, value); }
			}
			public static readonly DependencyProperty SelectedTabIndexProperty = DependencyProperty.Register("SelectedTabIndex", typeof(int), typeof(TabHeadersRowData), new PropertyMetadata(0, (d, e) => ((TabHeadersRowData)d).OnSelectedTabIndexChanged()));
			internal TabsDetailInfo CurrentTabsDetailInfo { get; private set; }
			public TabHeadersRowData(Func<FrameworkElement> createRowElementDelegate)
				: base(DetailNodeKind.TabHeaders, createRowElementDelegate) {
			}
			void OnSelectedTabIndexChanged() {
				CurrentTabsDetailInfo.SelectedTabIndex = SelectedTabIndex;
			}
			internal Locker SelectedIndexLocker = new Locker();
			internal void AssignFrom(TabsDetailInfo currentTabsDetailInfo) {
				this.CurrentTabsDetailInfo = currentTabsDetailInfo;
				SelectedIndexLocker.DoLockedAction(() => {
					SelectedTabIndex = currentTabsDetailInfo.SelectedTabIndex;
				});
			}
		}
		#endregion
		int selectedTabIndex;
		public int SelectedTabIndex {
			get { return selectedTabIndex; }
			set {
				if(selectedTabIndex == value)
					return;
				selectedTabIndex = value;
				ValidateChildDetailInfo();
				detailDescriptor.InvalidateTree();
				FocusViewInsideTabOrMaster();
			}
		}
		TabViewDetailDescriptor TabDetailDescriptor { get { return (TabViewDetailDescriptor)detailDescriptor; } }
		Dictionary<DetailDescriptorBase, DetailInfoWithContent> detailInfoCache = new Dictionary<DetailDescriptorBase, DetailInfoWithContent>();
#if DEBUGTEST
		internal Dictionary<DetailDescriptorBase, DetailInfoWithContent> DetailInfoCacheDebugTest { get { return detailInfoCache; } }
		internal
#endif
		DetailInfoWithContent selectedDetailInfo;
		public TabsDetailInfo(TabViewDetailDescriptor tabDetailDescriptor, RowDetailContainer container)
			: base(tabDetailDescriptor, container) {
		}
		protected override void OnExpanded() {
			base.OnExpanded();
			ValidateChildDetailInfo();
		}
		protected override void OnCollapsed() {
			if(selectedDetailInfo != null) {
				selectedDetailInfo.IsExpanded = false;
			}
		}
		void ValidateChildDetailInfo() {
			if(!IsExpanded)
				throw new InvalidOperationException();
			if(TabDetailDescriptor.DetailDescriptors.Count == 0)
				return;
			if(selectedDetailInfo != null)
				selectedDetailInfo.IsExpanded = false;
			DetailDescriptorBase selectedDescriptor = TabDetailDescriptor.DetailDescriptors[SelectedTabIndex];
			if(!detailInfoCache.TryGetValue(selectedDescriptor, out selectedDetailInfo)) {
				detailInfoCache[selectedDescriptor] = selectedDetailInfo = selectedDescriptor.CreateRowDetailInfo(container);
			}
			UpdateSelectedDetailInfoMasterRowData(RowsContainer.MasterRowData);
			selectedDetailInfo.IsExpanded = true;
		}
		void FocusViewInsideTabOrMaster() {
			DataViewBase firstDetailView = null;
			if(selectedDetailInfo != null) {
				firstDetailView = selectedDetailInfo.FindFirstDetailView();
			}
			if(firstDetailView != null) {
				int rowHandle = firstDetailView.DataControl.GetRowHandleByVisibleIndexCore(0);
				firstDetailView.ScrollIntoView(rowHandle);
			} else {
				int rowHandle = MasterDataControl.GetRowHandleByVisibleIndexCore(MasterVisibleIndex);
				MasterDataControl.DataView.ScrollIntoView(rowHandle);
			}
		}
		public override RowsContainer GetRowsContainerAndUpdateMasterRowData(RowData masterRowData) {
			UpdateSelectedDetailInfoMasterRowData(masterRowData);
			return base.GetRowsContainerAndUpdateMasterRowData(masterRowData);
		}
		void UpdateSelectedDetailInfoMasterRowData(RowData masterRowData) {
			if(selectedDetailInfo != null)
				selectedDetailInfo.UpdateMasterRowData(masterRowData);
		}
		internal override IEnumerator<RowNode> GetRowNodesEnumeratorCore(int nextLevelStartVisibleIndex, int nextLevelVisibleRowsCount, int serviceVisibleRowsCount) {
			if(detailDescriptor.ShowHeader && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				yield return GetDetailHeaderNode();
			}
			if(TabDetailDescriptor.ContentTemplate != null && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				yield return GetDetailContentNode();
			}
			if(serviceVisibleRowsCount > 0) {
				DetailRowNode detailTabHeadersNode = GetRowNode(DetailNodeKind.TabHeaders, nodeKind => new TabHeadersRowData(() => CreateDetailHeaderControl(MasterTableViewBehavior.CreateDetailTabHeadersElement)));
				if(selectedDetailInfo != null && selectedDetailInfo.NodeContainer != null) {
					if(nextLevelVisibleRowsCount > 0)
						detailTabHeadersNode.UpdateExpandInfoEx(selectedDetailInfo.NodeContainer, selectedDetailInfo.RowsContainer, nextLevelStartVisibleIndex, true);
					else
						detailTabHeadersNode.UpdateExpandInfoEx(null, null, nextLevelStartVisibleIndex, true);
				}
				yield return detailTabHeadersNode;
			}
		}
		protected override DetailNodeKind[] GetNodeScrollOrder() {
			return new DetailNodeKind[] { DetailNodeKind.TabHeaders, DetailNodeKind.DetailContent, DetailNodeKind.DetailHeader };
		}
		public override int CalcTotalLevel() {
			return base.CalcTotalLevel() + (selectedDetailInfo != null ? selectedDetailInfo.CalcTotalLevel() : 0);
		}
		protected override int GetNextLevelRowCount() {
			return selectedDetailInfo != null ? selectedDetailInfo.CalcRowsCount() : 0;
		}
		protected override int GetVisibleDataRowCount() {
			return selectedDetailInfo != null ? selectedDetailInfo.CalcVisibleDataRowCount() : 0;
		}
		protected override int GetTopServiceRowsCount() {
			return CalcHeaderRowCount() + CalcContentRowCount() + 1;
		}
		protected override int GetBottomServiceRowsCount() {
			return 0;
		}
		internal override DetailInfoWithContent.DetailRowNode CreateDetailRowNode(DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate) {
			if(detailNodeKind == DetailNodeKind.TabHeaders)
				return new TabHeadersRowNode(this, createRowDataDelegate);
			return base.CreateDetailRowNode(detailNodeKind, createRowDataDelegate);
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			if(selectedDetailInfo == null) {
				targetView = null;
				targetVisibleIndex = -1;
				return false;
			}
			return selectedDetailInfo.FindViewAndVisibleIndexByScrollIndex(scrollIndex, forwardIfServiceRow, out targetView, out targetVisibleIndex);
		}
		public override DataViewBase FindFirstDetailView() {
			if(selectedDetailInfo == null) return null;
			return selectedDetailInfo.FindFirstDetailView();
		}
		public override DataViewBase FindLastInnerDetailView() {
			if(selectedDetailInfo == null) return null;
			return selectedDetailInfo.FindLastInnerDetailView();
		}
		public override DataControlBase FindVisibleDetailDataControl() {
			if(selectedDetailInfo == null) return null;
			return selectedDetailInfo.FindVisibleDetailDataControl();
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor() {
			if(selectedDetailInfo == null) return TabDetailDescriptor;
			return selectedDetailInfo.FindVisibleDetailDescriptor();
		}
		public override bool IsDetailRowExpanded(DetailDescriptorBase descriptor) {
			if(IsExpanded) {
				if(descriptor == null)
					return true;
				int index = TabDetailDescriptor.DetailDescriptors.IndexOf(descriptor);
				if(index > -1 && index == SelectedTabIndex)
					return true;
			}
			return false;
		}
		public override void SetDetailRowExpanded(bool expand, DetailDescriptorBase descriptor) {
			base.SetDetailRowExpanded(expand, descriptor);
			if(IsExpanded && descriptor != null) {
				int index = TabDetailDescriptor.DetailDescriptors.IndexOf(descriptor);
				if(index > -1) {
					SelectedTabIndex = index;
					return;
				}
			}
		}
		internal override IEnumerable<RowDetailInfoBase> GetRowDetailInfoEnumerator() {
			yield return this;
			foreach(RowDetailInfoBase detailInfo in detailInfoCache.Values) {
				foreach(RowDetailInfoBase subDetailInfo in detailInfo.GetRowDetailInfoEnumerator()) {
					yield return subDetailInfo;
				}
			}
		}
	}
	public class DataControlDetailInfo : DetailInfoWithContent, IDataControlParent {
		#region inner classes
		internal class DetailViewRowNode : DetailRowNode {
			readonly DataViewBase view;
			public DetailViewRowNode(DetailInfoWithContent owner, DataViewBase view, DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate)
				: base(owner, detailNodeKind, createRowDataDelegate) {
				this.view = view;
			}
			internal override void AssignToRowData(RowDataBase rowData, RowDataBase masterRowData) {
				IViewRowData viewRowData = rowData as IViewRowData;
				if(viewRowData != null)
					viewRowData.SetViewAndUpdate(view);
			}
		}
		public class DetailViewRowData : DetailRowData, IViewRowData {
			public DetailViewRowData(DetailNodeKind nodeKind, Func<FrameworkElement> createRowElementDelegate)
				: base(nodeKind, createRowElementDelegate) {
			}
			internal override void SetMasterView(DataViewBase view) { }
			internal override void UpdateLineLevel() {
				DataViewBase targetView = null;
				int targetVisibleIndex = -1;
				if(View.DataControl.DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
					RowData rowData = targetView.GetRowData(targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex));
					if(rowData != null) {
						LineLevel = GetLineLevel(this, 0, 0);
						bool isLastRow = true;
						DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
						if(isLastRow)
							LineLevel = DetailLevel;
						return;
					}
				}
				LineLevel = 0;
				DetailLevel = 0;
			}
			protected override int GetLineLevel(RowDataBase rowData, int lineLevel, int detailCount) {
				if((rowData is RowData) && (((rowData as RowData).RowPosition != Grid.RowPosition.Bottom) ||
					((rowData as RowData).RowPosition != Grid.RowPosition.Single))) {
					return lineLevel;
				}
				return base.GetLineLevel(rowData, lineLevel, detailCount);
			}
			#region IViewRowData Members
			void IViewRowData.SetViewAndUpdate(DataViewBase view) {
				View = view;
			}
			#endregion
		}
		#endregion
		DataControlBase dataControl;
		internal DataControlBase DataControl { get { return dataControl; } set { dataControl = value; } }
		internal DataControlDetailDescriptor DataControlDetailDescriptor { get { return (DataControlDetailDescriptor)detailDescriptor; } }
		Grid.DetailNodeContainer DetailRootNodeContainer { get { return SafeGetViewProperty<Grid.DetailNodeContainer>(view => view.RootNodeContainer); } }
		public DataControlDetailInfo(DataControlDetailDescriptor detailDescriptor, RowDetailContainer container)
			: base(detailDescriptor, container) {
		}
		internal void UpdateDataControl() {
			DataControlDetailDescriptor.DataControl.CloneDetail(MasterDataControl.DataView.MasterRootNodeContainer, MasterRootRowsContainer, Row, DataControlDetailDescriptor.GetItemsSourceBinding(), this, !DataControlDetailDescriptor.DataControl.CanAutoPopulateColumns);
		}
		public void CloneDetail() {
			if(dataControl == null)
				UpdateDataControl();
			DataControlDetailDescriptor.PopulateColumnsIfNeeded(dataControl.DataProviderBase);
			DataControlDetailDescriptor.DataControl.CopyToDetail(dataControl);
			dataControl.DataView.UpdateContentLayout();
			if(dataControl.DataView.RootDataPresenter != null)
				dataControl.DataView.RootDataPresenter.ScrollInfoCore.SecondarySizeScrollInfo.Invalidate();
		}
		protected override void OnExpanded() {
			CloneDetail();
		}
		protected override void OnCollapsed() {
			base.OnCollapsed();
		}
		T SafeGetViewProperty<T>(Func<DataViewBase, T> getPropertyDelegate) where T : class {
			return (dataControl != null) && (dataControl.DataView != null) ? getPropertyDelegate(dataControl.DataView) : null;
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			int visibleRowCountWithDetails = DataControl.VisibleRowCount + DataControl.MasterDetailProvider.CalcVisibleDetailRowsCount();
			if(scrollIndex >= visibleRowCountWithDetails) {
				if(scrollIndex >= visibleRowCountWithDetails + GetBottomServiceRowsCount()) {
					if(forwardIfServiceRow) {
						targetView = FindFirstDetailView();
						if(targetView != null) {
							targetVisibleIndex = 0;
							return true;
						}
					} else {
						IDataControlParent dataControlParent = this;
						if(dataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
							return true;
						}
					}
				} else {
					if(forwardIfServiceRow) {
						IDataControlParent dataControlParent = this;
						if(dataControlParent.FindNextOuterMasterRow(out targetView, out targetVisibleIndex)) {
							return true;
						}
					} else {
						targetView = FindLastInnerDetailView();
						if(targetView != null) {
							targetVisibleIndex = targetView.DataControl.VisibleRowCount - 1;
							return true;
						}
					}
				}
				targetView = null;
				targetVisibleIndex = -1;
				return false;
			}
			return DataControl.FindViewAndVisibleIndexByScrollIndexCore(scrollIndex, forwardIfServiceRow, out targetView, out targetVisibleIndex);
		}
		public override DataViewBase FindFirstDetailView() {
			if(DataControl.VisibleRowCount > 0 || DataControl.DataView.IsNewItemRowVisible) {
				return DataControl.DataView;
			}
			return null;
		}
		public override DataViewBase FindLastInnerDetailView() {
			return DataControl.FindLastInnerDetailView();
		}
		public override DataControlBase FindVisibleDetailDataControl() {
			return DataControl;
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor() {
			return DataControlDetailDescriptor;
		}
		protected override int GetNextLevelRowCount() {
			return DataControl != null ? DataControl.VisibleRowCount + DataControl.viewCore.CalcGroupSummaryVisibleRowCount() + DataControl.MasterDetailProvider.CalcVisibleDetailRowsCount() : 0; 
		}
		protected override int GetVisibleDataRowCount() {
			if(DataControl == null)
				return 0;
			int visibleDataRowCount = DataControl.VisibleRowCount + DataControl.MasterDetailProvider.CalcVisibleDetailDataRowCount();
			if(DataControl.DataView.IsNewItemRowVisible)
				visibleDataRowCount++;
			return visibleDataRowCount;
		}
		protected override int GetTopServiceRowsCount() {
			int count = CalcHeaderRowCount() + CalcContentRowCount();
			if(DataControl != null) {
				if(DataControl.DataView.ShowColumnHeaders)
					count++;
				if(DataControl.DataView.IsNewItemRowVisible)
					count++;
			}
			return count;
		}
		protected override int GetBottomServiceRowsCount() {
			int count = 0;
			if(DataControl != null) {
				if(DataControl.DataView.ShowTotalSummary)
					count++;
				if(DataControl.DataView.ShowFixedTotalSummary)
					count++;
			}
			return count;
		}
		protected override DetailNodeKind[] GetNodeScrollOrder() {
			return new DetailNodeKind[] { DetailNodeKind.DataRowsContainer, DetailNodeKind.NewItemRow, DetailNodeKind.TotalSummary, DetailNodeKind.FixedTotalSummary, DetailNodeKind.ColumnHeaders, DetailNodeKind.DetailContent, DetailNodeKind.DetailHeader };
		}
		internal override IEnumerator<RowNode> GetRowNodesEnumeratorCore(int nextLevelStartVisibleIndex, int nextLevelVisibleRowsCount, int serviceVisibleRowsCount) {
			bool hasContentRow = detailDescriptor.ContentTemplate != null;
			if(DataControlDetailDescriptor.ShowHeader && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				DetailHeaderRowNode rowNode = (DetailHeaderRowNode)GetDetailHeaderNode();
				rowNode.ShowBottomLine = !hasContentRow;
				yield return rowNode;
			}
			if(hasContentRow && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				DetailHeaderRowNode rowNode = (DetailHeaderRowNode)GetDetailContentNode();
				rowNode.ShowBottomLine = true;
				yield return rowNode;
			}
			if(DataControl.DataView.ShowColumnHeaders && serviceVisibleRowsCount > 0) {
				serviceVisibleRowsCount--;
				yield return GetRowNode(DetailNodeKind.ColumnHeaders, nodeKind => new DetailColumnsData(nodeKind, SafeGetViewProperty<DataTreeBuilder>(view => view.VisualDataTreeBuilder), MasterTableViewBehavior.CreateDetailColumnHeadersElement));
			}
			bool isFixedTotalSummaryRowVisible = DataControl.DataView.ShowFixedTotalSummary && serviceVisibleRowsCount > 0;
			if(isFixedTotalSummaryRowVisible)
				serviceVisibleRowsCount--;
			bool isTotalSummaryVisible = DataControl.DataView.ShowTotalSummary && serviceVisibleRowsCount > 0;
			if(isTotalSummaryVisible)
				serviceVisibleRowsCount--;
			bool isNewItemRowVisible = DataControl.DataView.IsNewItemRowVisible && serviceVisibleRowsCount > 0;
			if(isNewItemRowVisible)
				serviceVisibleRowsCount--;
			if(isNewItemRowVisible) {
				yield return GetRowNode(DetailNodeKind.NewItemRow, nodeKind => new DetailColumnsData(nodeKind, SafeGetViewProperty<DataTreeBuilder>(view => view.VisualDataTreeBuilder), MasterTableViewBehavior.CreateDetailNewItemRowElement));
			}
			if(nextLevelVisibleRowsCount > 0) {
				DetailRowNode detailContainerNode = GetRowNode(DetailNodeKind.DataRowsContainer, nodeKind => new DetailRowData(nodeKind, () => new EmptyDetailRowControl()));
				detailContainerNode.UpdateExpandInfoEx(DetailRootNodeContainer, SafeGetViewProperty<RowsContainer>(view => view.RootRowsContainer), nextLevelStartVisibleIndex, false);
				yield return detailContainerNode;
			}
			if(isTotalSummaryVisible) {
				yield return GetRowNode(DetailNodeKind.TotalSummary, nodeKind => new DetailColumnsData(nodeKind, SafeGetViewProperty<DataTreeBuilder>(view => view.VisualDataTreeBuilder), MasterTableViewBehavior.CreateDetailTotalSummaryElement));
			}
			if(isFixedTotalSummaryRowVisible) {
				yield return GetRowNode(DetailNodeKind.FixedTotalSummary, nodeKind => new DetailViewRowData(nodeKind, MasterTableViewBehavior.CreateDetailFixedTotalSummaryElement));
			}
		}
		internal override DetailRowNode CreateDetailRowNode(DetailNodeKind detailNodeKind, Func<DetailNodeKind, RowDataBase> createRowDataDelegate) {
			return detailNodeKind == DetailNodeKind.DetailHeader || detailNodeKind == DetailNodeKind.DetailContent ? 
				base.CreateDetailRowNode(detailNodeKind, createRowDataDelegate) :
				new DetailViewRowNode(this, DataControl.DataView, detailNodeKind, createRowDataDelegate);
		}
		internal override bool IsFixedNode(DetailInfoWithContent.DetailRowNode detailRowNode) {
			return false;
		}
		#region IDataControlParent Members
		IEnumerable<ColumnsRowDataBase> IDataControlParent.GetColumnsRowDataEnumerator() {
			return RowsContainer.Items.Where(rowData => rowData is ColumnsRowDataBase).Cast<ColumnsRowDataBase>();
		}
		ColumnsRowDataBase IDataControlParent.GetNewItemRowData() {
			return (ColumnsRowDataBase)GetRowDataByKind(DetailNodeKind.NewItemRow);
		}
		ColumnsRowDataBase IDataControlParent.GetHeadersRowData() {
			return (ColumnsRowDataBase)GetRowDataByKind(DetailNodeKind.ColumnHeaders);
		}
		RowDataBase GetRowDataByKind(DetailNodeKind kind) {
			return RowsContainer.Items.FirstOrDefault(rowData => object.Equals(rowData.MatchKey, kind));
		}
		DataViewBase IDataControlParent.FindMasterView() {
			return MasterDataControl.DataView;
		}
		bool IDataControlParent.FindMasterRow(out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = MasterDataControl.DataView;
			targetVisibleIndex = MasterVisibleIndex;
			return true;
		}
		bool IDataControlParent.FindNextOuterMasterRow(out DataViewBase targetView, out int targetVisibleIndex) {
			return MasterDataControl.FindNextOuterMasterRow(MasterVisibleIndex, out targetView, out targetVisibleIndex);
		}
		void IDataControlParent.InvalidateTree() {
			DataControlDetailDescriptor.InvalidateTree();
		}
		void IDataControlParent.EnumerateParentDataControls(Action<DataControlBase, int> action) {
			action(MasterDataControl, MasterVisibleIndex);
			MasterDataControl.DataControlParent.EnumerateParentDataControls(action);
		}
		void IDataControlParent.ValidateMasterDetailConsistency(DataControlBase dataControl) {
			dataControl.DataProviderBase.ThrowNotSupportedExceptionIfInServerMode();
		}
		void IDataControlParent.CollectViewVisibleIndexChain(List<KeyValuePair<DataViewBase, int>> chain) {
			chain.Insert(0, new KeyValuePair<DataViewBase, int>(MasterDataControl.DataView, MasterVisibleIndex));
			MasterDataControl.CollectViewVisibleIndexChain(chain);
		}
		void IDataControlParent.CollectParentFixedRowsScrollIndexes(List<int> scrollIndexes) {
			int masterScrollIndex = MasterDataControl.DataView.ConvertVisibleIndexToScrollIndex(MasterVisibleIndex);
			for(int i = GetTopServiceRowsCount(); i >= 1; i--) {
				scrollIndexes.Add(masterScrollIndex - i);
			}
			scrollIndexes.Add(masterScrollIndex);
			MasterDataControl.CollectParentFixedRowsScrollIndexes(MasterVisibleIndex, scrollIndexes);
		}
		int IDataControlParent.CalcTotalLevel() {
			return MasterDataControl.CalcTotalLevel(MasterVisibleIndex) + MasterDataControl.MasterDetailProvider.CalcTotalLevel(MasterVisibleIndex) + 1;
		}
		#endregion
		protected override void UpdateRow(object row) {
			base.UpdateRow(row);
			if(DataControl != null) DataControl.DataContext = row;
		}
		internal override void RemoveFromDetailClones() {
			if(DataControl != null)
				DataControlDetailDescriptor.DataControl.DetailClones.Remove(DataControl);
		}
		public override void Clear() {
			base.Clear();
			ClearGrid();
		}
		public override void Detach() {
			base.Detach();
			ClearGrid();
		}
		private void ClearGrid() {
			DataControl.DataView.DataControlMenu.Destroy();
			DataControl.UnselectAll();
			DataControlDetailDescriptor.DataControl.DetailClones.Remove(dataControl);
		}
	}
	public class EmptyRowDetailInfo : RowDetailInfoBase {
		public static EmptyRowDetailInfo Instance = new EmptyRowDetailInfo();
		EmptyRowDetailInfo() { }
		public override bool IsExpanded {
			get { return false; }
			set { throw new NotImplementedException(); }
		}
		protected override void OnExpanded() {
			throw new NotImplementedException();
		}
		protected override void OnCollapsed () {
			throw new NotImplementedException();
		}
		public override NodeContainer GetNodeContainer() {
			return null;
		}
		public override RowsContainer GetRowsContainerAndUpdateMasterRowData(RowData masterRowData) {
			return null;
		}
		public override int CalcTotalLevel() {
			return 0;
		}
		public override int CalcRowsCount() {
			return 0;
		}
		public override int CalcVisibleDataRowCount() {
			return 0;
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		public override DataViewBase FindFirstDetailView() {
			return null;
		}
		public override DataViewBase FindLastInnerDetailView() {
			return null;
		}
		public override DataControlBase FindVisibleDetailDataControl() {
			return null;
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor() {
			return null;
		}
		internal override IEnumerable<RowDetailInfoBase> GetRowDetailInfoEnumerator() {
			return new RowDetailInfoBase[0];
		}
		protected override void UpdateRow(object row) {
		}
	}
}
