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
using DevExpress.XtraTreeList;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.Utils;
using DevExpress.XtraTreeList.Columns;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Commands;
using System.Drawing;
using System.Collections;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList.Data;
using System.Data;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region ResourcesTree
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.ResourcesTreeDesigner, " + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ResourcesTree.bmp")
	]
	[Docking(DockingBehavior.Ask)]
	public class ResourcesTree : TreeList, IDisposable, ISupportInitialize, IResourceTreeControllerOwner {
		ResourcesTreeControllerWin controller;
		bool isLoaded = false;
		public ResourcesTree() {
			this.controller = new ResourcesTreeControllerWin(this);
			this.controller.Changed += new EventHandler(OnControllerChanged);
			this.OptionsView.ShowIndicator = false;
			this.OptionsBehavior.Editable = false;
		}
		#region Properties
		#region DataSource
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new object DataSource { get { return base.DataSource; } set { base.DataSource = value; } }
		#endregion
		#region DataMember
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new string DataMember { get { return base.DataMember; } set { base.DataMember = value; } }
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerExtensionsLocalizedDescription("ResourcesTreeSchedulerControl"),
#endif
		Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return Controller.SchedulerControl; }
			set { Controller.SchedulerControl = value; }
		}
		protected internal ISchedulerStorageBase SchedulerStorage { get { return SchedulerControl != null ? SchedulerControl.DataStorage : null; } }
		[DefaultValue(ScrollVisibility.Always), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ScrollVisibility HorzScrollVisibility { get { return base.HorzScrollVisibility; } set { base.HorzScrollVisibility = value; } }
#if !SL
	[DevExpressXtraSchedulerExtensionsLocalizedDescription("ResourcesTreeVertScrollVisibility")]
#endif
		public override ScrollVisibility VertScrollVisibility { get { return CalculateActualVertScrollVisibility(); } set { base.VertScrollVisibility = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, true)]
		public new ResourceTreeColumnCollection Columns { get { return (ResourceTreeColumnCollection)base.Columns; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ColumnPanelRowHeight { get { return base.ColumnPanelRowHeight; } set { base.ColumnPanelRowHeight = value; } }
		protected internal ResourcesTreeControllerWin Controller { get { return controller; } }
		internal bool IsLoaded { get { return isLoaded; } }
		protected override bool ActualAutoNodeHeight {
			get { return Controller.LayoutStrategy.ActualAutoNodeHeight; }
		}
		protected override bool EnableEnhancedSorting {
			get {
				return true;
			}
		}
		protected override bool CanExpandNodesOnIncrementalSearch {
			get { return OptionsBehavior.ExpandNodesOnIncrementalSearch; }
		} 
		#endregion
#if !SL
	[DevExpressXtraSchedulerExtensionsLocalizedDescription("ResourcesTreeIsUnboundMode")]
#endif
		public override bool IsUnboundMode {
			get {
				if (DesignMode)
					return false;
				return base.IsUnboundMode;
			}
		}
		public override object InternalGetService(Type service) {
			if (!DesignMode)
				return base.InternalGetService(service);
			if (service != null && service.Equals(typeof(DataColumnInfoCollection)))
				return new DesignTimeDataColumnInfoCollection(this);
			return base.InternalGetService(service);
		}
		internal ScrollVisibility CalculateActualVertScrollVisibility() {
			if (SchedulerControl == null)
				return base.VertScrollVisibility;
			Rectangle schedulerBounds = SchedulerControl.Bounds;
			return schedulerBounds.Height == Bounds.Height ? base.VertScrollVisibility : ScrollVisibility.Never;
		}
		void OnControllerChanged(object sender, EventArgs e) {
			if (SchedulerControl != null)
				MenuManager = SchedulerControl.MenuManager;
			LayoutChanged();
		}
		public override void LayoutChanged() {
			base.LayoutChanged();
			if (!this.disposing)
				Update();
		}
		public override void Refresh() {
			LayoutChanged();
		}
		protected override void UpdateLayout() {
			base.UpdateLayout();
		}
		#region IDisposable implementation
		bool disposing = false;
		protected override void Dispose(bool disposing) {
			try {
				this.disposing = true;
				if (disposing) {
					Controller.Changed -= new EventHandler(OnControllerChanged);
					Controller.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			base.BeginInit();
			controller.BeginUpdate();
		}
		void ISupportInitialize.EndInit() {
			base.EndInit();
			controller.EndUpdate();
		}
		#endregion
		#region IResourceTreeControllerOwner Members
		Resource IResourceTreeControllerOwner.FirstVisibleResource {
			get {
				if (SchedulerControl == null)
					return ResourceBase.Empty;
				ResourceBaseCollection visibleResources = SchedulerControl.ActiveView.VisibleResources;
				return visibleResources.Count == 0 ? ResourceBase.Empty : visibleResources[0];
			}
		}
		void IResourceTreeControllerOwner.SetTopVisibleNode(TreeListNode node) {
			ForceSetTopVisibleNodeIndex(GetVisibleIndexByNode(node));
		}
		#endregion
		protected override void OnLoadedCore() {
			base.OnLoadedCore();
			this.isLoaded = true;
			Controller.UpdateDataSource(); 
			Controller.UpdateNodesCache();
			Controller.UpdateLayout();
		}
		protected override TreeListOptionsView CreateOptionsView() {
			return new ResourcesTreeOptionsView();
		}
		protected override TreeListOptionsBehavior CreateOptionsBehavior() {
			return new ResourcesTreeOptionsBehavior(this);
		}
		protected override TreeListOptionsFilter CreateOptionsFilter() {
			return new ResourcesTreeOptionsFilter();
		}
		protected override TreeListColumnCollection CreateColumns() {
			return new ResourceTreeColumnCollection(this);
		}
		#region Filtering
		[DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FilterNodeEventHandler FilterNode {
			add { }
			remove { }
		}
		protected override void OnActiveFilterCriteriaChanged() {
			base.OnActiveFilterCriteriaChanged();
			Controller.OnFilterChanged();
		}
		protected override void OnActiveFilterEnabledChanged() {
			base.OnActiveFilterEnabledChanged();
			Controller.OnFilterChanged();
		}
		protected override void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionsBehaviorChanged(sender, e);
			if (e.Name == "EnableFiltering")
				Controller.OnFilterChanged();
		}
		protected override void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionsViewChanged(sender, e);
			if (e.Name == "ShowAutoFilterRow")
				Controller.OnShowAutoFilterRowChanged();
		}
		#endregion
		protected override bool DoSort(TreeListNodes nodes, bool makeFocusedNodeVisible) {
			if (!IsLoaded)
				return true;
			if (SchedulerStorage == null)
				return true;
			if (DataSource == null)
				return true;
			ResourcesTreeColumnInfos schedulerColumns = ((IInternalResourceStorage)SchedulerStorage.Resources).ResourcesTreeSortedColumns;
			schedulerColumns.BeginUpdate();
			schedulerColumns.Clear();
			int count = SortedColumns.Count;
			for (int i = 0; i < count; i++) {
				TreeListColumn column = SortedColumns[i];
				schedulerColumns.Add(new ResourcesTreeColumnInfo(column.FieldName, column.SortOrder));
			}
			bool result = base.DoSort(nodes, makeFocusedNodeVisible);
			if (Controller.CanUpdateScheduler())
				schedulerColumns.EndUpdate();
			else
				schedulerColumns.CancelUpdate();
			return result;
		}
		protected internal int ProtectedCalculateVisibleRowsAreaHeight() {
			return CalculateVisibleRowsAreaHeight();
		}
		protected override IComparer CreateNodesComparer() {
			return new SchedulerTreeListResourceNodesComparer(this, SortedColumns);
		}
	}
	#endregion
	#region Options
	#region ResourcesTreeOptionsColumn
	public class ResourcesTreeOptionsColumn : TreeListOptionsColumn {
		public ResourcesTreeOptionsColumn() {
			base.AllowSort = false;
			base.AllowEdit = false;
			base.ReadOnly = true;
		}
		[DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowSort { get { return base.AllowSort; } set { } }
		[DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowEdit { get { return base.AllowEdit; } set { } }
		[DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ReadOnly { get { return base.ReadOnly; } set { } }
	}
	#endregion
	#region ResourcesTreeOptionsView
	public class ResourcesTreeOptionsView : TreeListOptionsView {
		public ResourcesTreeOptionsView() {
			this.ShowVertLines = false;
			this.ShowIndentAsRowStyle = true;
		}
		[DefaultValue(false), XtraSerializableProperty()]
		public override bool ShowIndicator { get { return base.ShowIndicator; } set { base.ShowIndicator = value; } }
		[DefaultValue(false), XtraSerializableProperty()]
		public override bool ShowVertLines { get { return base.ShowVertLines; } set { base.ShowVertLines = value; } }
		[DefaultValue(true), XtraSerializableProperty()]
		public override bool ShowIndentAsRowStyle { get { return base.ShowIndentAsRowStyle; } set { base.ShowIndentAsRowStyle = value; } }
	}
	#endregion
	#region ResourcesTreeOptionsBehavior
	public class ResourcesTreeOptionsBehavior : TreeListOptionsBehavior {
		public ResourcesTreeOptionsBehavior(ResourcesTree treeList) : base(treeList) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public override bool AutoNodeHeight { get { return base.AutoNodeHeight; } set { base.AutoNodeHeight = value; } }
	}
	#endregion
	#region ResourcesTreeFilterOptions
	public class ResourcesTreeOptionsFilter : TreeListOptionsFilter {
		[DefaultValue(FilterMode.Smart), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override FilterMode FilterMode { get { return FilterMode.Smart; } set { } }
	}
	#endregion
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region DesignTimeDataColumnInfoCollection
	public class DesignTimeDataColumnInfoCollection : DataColumnInfoCollection {
		public DesignTimeDataColumnInfoCollection(ResourcesTree tree)
			: base(null) {
			SchedulerControl scheduler = tree.SchedulerControl;
			if (scheduler == null)
				return;
			ISchedulerStorageBase storage = scheduler.DataStorage;
			if (storage == null)
				return;
			IResourceStorageBase resources = storage.Resources;
			if (resources == null)
				return;
			Dictionary<string, string> mappings = resources.Mappings.MemberDictionary;
			foreach (var item in mappings) {
				if (String.IsNullOrEmpty(item.Value))
					continue;
				List.Add(new DataColumnInfo(new DataColumn(item.Value)));
			}
			foreach (var item in resources.CustomFieldMappings) {
				if (String.IsNullOrEmpty(item.Member))
					continue;
				List.Add(new DataColumnInfo(new DataColumn(item.Member)));
			}
		}
		public int Add(DataColumnInfo item) {
			return List.Add(item);
		}
		public bool Contains(DataColumnInfo item) {
			return List.Contains(item);
		}
		public void Insert(int index, DataColumnInfo value) {
			List.Insert(index, value);
		}
		public void Remove(DataColumnInfo value) {
			List.Remove(value);
		}
		public int IndexOf(DataColumnInfo value) {
			return List.IndexOf(value);
		}
		public void CopyTo(DataColumnInfo[] array, int index) {
			List.CopyTo(array, index);
		}
		protected override void AddColumnDefinitions(IList dataSource) {
		}
	}
	#endregion
	#region SchedulerTreeListResourceNodesComparer
	public class SchedulerTreeListResourceNodesComparer : IComparer {
		ICollection sortColumns;
		ResourcesTree resourceTree;
		public SchedulerTreeListResourceNodesComparer(ResourcesTree treeList, ICollection sortColumns) {
			this.sortColumns = sortColumns;
			this.resourceTree = treeList;
		}
		internal static int InternalCompare(object x, object y) {
			if (x == y)
				return 0;
			if (x == null || x == DBNull.Value)
				return -1;
			if (y == null || y == DBNull.Value)
				return 1;
			IComparable xComp = x as IComparable;
			if (xComp == null)
				xComp = (IComparable)x.ToString();
			IComparable yComp = y as IComparable;
			if (yComp == null)
				yComp = (IComparable)y.ToString();
			int res = 0;
			try {
				res = xComp.CompareTo(yComp);
			} catch { }
			return res;
		}
		int IComparer.Compare(object x, object y) {
			int res = 0;
			TreeListNode node1 = x as TreeListNode;
			TreeListNode node2 = y as TreeListNode;
			foreach (TreeListColumn columnID in sortColumns) {
				ColumnSortMode sortMode = columnID.SortMode;
				object v1 = GetCompareValue(node1, columnID, sortMode);
				object v2 = GetCompareValue(node2, columnID, sortMode);
				res = InternalCompare(v1, v2);
				if (res == 0)
					continue;
				if (columnID.SortOrder == SortOrder.Ascending)
					return res;
				res = (res > 0 ? -1 : 1);
				return res;
			}
			if (res == 0) {
				int index1 = GetNodeIndex(node1);
				int index2 = GetNodeIndex(node2);
				res = index1 - index2;
			}
			return res;
		}
		object GetCompareValue(TreeListNode node, TreeListColumn column, ColumnSortMode sortMode) {
			if (sortMode == ColumnSortMode.DisplayText)
				return node.GetDisplayText(column);
			return node.GetValue(column);
		}
		int GetNodeIndex(TreeListNode node) {
			if (resourceTree.Controller.NodesCache.Resources.ContainsKey(node)) {
				Resource resource = resourceTree.Controller.NodesCache.Resources[node];
				return resourceTree.Controller.SchedulerControl.DataStorage.Resources.Items.IndexOf(resource);
			}
			return this.resourceTree.Nodes.IndexOf(node);
		}
	}
	#endregion
	#region ResourceTreeColumnCollection
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039:ListsAreStronglyTyped")]
	public class ResourceTreeColumnCollection : TreeListColumnCollection {
		public ResourceTreeColumnCollection(ResourcesTree owner)
			: base(owner) {
		}
		protected override TreeListColumn CreateColumn() {
			return new ResourceTreeColumn();
		}
	}
	#endregion
	#region ResourceTreeColumn
	public class ResourceTreeColumn : TreeListColumn {
	}
	#endregion
	#region IResourceTreeControllerOwner
	public interface IResourceTreeControllerOwner {
		object DataSource { get; set; }
		TreeListNodesIterator NodesIterator { get; }
		object GetDataRecordByNode(TreeListNode node);
		Resource FirstVisibleResource { get; }
		void SetTopVisibleNode(TreeListNode node);
		void ExpandAll();
		void BeginSort();
		void EndSort();
	}
	#endregion
	#region ResourceNodesCache
	public class ResourceNodesCache {
		Dictionary<Resource, TreeListNode> nodes;
		Dictionary<TreeListNode, Resource> resources;
		public Dictionary<Resource, TreeListNode> Nodes { get { return nodes; } }
		public Dictionary<TreeListNode, Resource> Resources { get { return resources; } }
		public ResourceNodesCache() {
			nodes = new Dictionary<Resource, TreeListNode>();
			resources = new Dictionary<TreeListNode, Resource>();
		}
		public void Clear() {
			this.nodes.Clear();
			this.resources.Clear();
		}
		public void Add(TreeListNode node, Resource resource) {
			resources.Add(node, resource);
			nodes.Add(resource, node);
		}
		public bool TryGetValue(Resource resource, out TreeListNode node) {
			return Nodes.TryGetValue(resource, out node);
		}
		public bool TryGetValue(TreeListNode node, out Resource resource) {
			return Resources.TryGetValue(node, out resource);
		}
	}
	#endregion
	#region ResourcesTreeController
	public abstract class ResourcesTreeController : IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		InnerSchedulerControl control;
		BatchUpdateHelper batchUpdateHelper;
		IResourceTreeControllerOwner owner;
		ISchedulerStorageBase storage;
		ResourceNodesCache nodesCache;
		bool deferredRefresh;
		protected ResourcesTreeController(IResourceTreeControllerOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.owner = owner;
			this.nodesCache = new ResourceNodesCache();
			SubscribeTreeListEvents();
		}
		public IResourceTreeControllerOwner Owner { get { return owner; } }
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
		public ResourceNodesCache NodesCache { get { return nodesCache; } }
		public InnerSchedulerControl InnerControl {
			get { return control; }
			set {
				if (control == value)
					return;
				SetInnerControlCore(value);
				OnInnerControlChanged();
			}
		}
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void Refresh() {
			Refresh(true);
		}
		protected internal virtual void Refresh(bool notifyOwnerChanged) {
			if (IsLockFilterCriteriaUpdate)
				return;
			if (Storage != null) {
				UpdateDataSource();
				UpdateLayout(notifyOwnerChanged);
			}
		}
		#region Filtering
		protected internal bool IsLockFilterCriteriaUpdate { get { return lockFilterCriteriaUpdate > 0; } }
		int lockFilterCriteriaUpdate = 0;
		internal void OnFilterChanged() {
			lockFilterCriteriaUpdate++;
			try {
				UpdateFilterCriteria();
			} catch {
			} finally {
				lockFilterCriteriaUpdate--;
			}
		}
		protected internal abstract void UpdateFilterCriteria();
		protected internal virtual void OnShowAutoFilterRowChanged() {
			UpdateLayout();
		}
		#endregion
		protected internal virtual void UpdateLayout() {
			UpdateLayout(true);
		}
		protected internal virtual void UpdateLayout(bool raiseChangedNotification) {
			ExpandNodes();
			UpdateTopVisibleNode();
			UpdateColumnPanel();
			if (raiseChangedNotification)
				RaiseChanged();
		}
		protected internal virtual void UpdateColumnPanel() {
		}
		protected internal virtual void UpdateTopVisibleNode() {
			UnsubscribeTreeListEvents();
			try {
				TreeListNode node;
				if (NodesCache.TryGetValue(Owner.FirstVisibleResource, out node))
					Owner.SetTopVisibleNode(node);
			} finally {
				SubscribeTreeListEvents();
			}
		}
		protected internal virtual void UpdateDataSource() {
			if (control == null)
				return;
			ResourceBaseCollection resources = control.GetResourcesTree(false);
			SuspendVisualUpdate();
			try {
				Owner.DataSource = null;
				Owner.DataSource = CreateTreeListData(resources);
			} finally {
				UpdateNodesCache();
				ResumeVisualUpdate();
			}
		}
		protected virtual void SuspendVisualUpdate() {
			Owner.BeginSort();
		}
		protected virtual void ResumeVisualUpdate() {
			Owner.EndSort();
		}
		protected internal virtual void UpdateNodesCache() {
			NodesCache.Clear();
			BeginUpdate();
			Owner.ExpandAll();
			DoNodesOperation(new TreeListOperationDelegate(UpdateNodeCacheMethod));
			EndUpdate();
		}
		protected internal virtual void ExpandNodes() {
			DoNodesOperation(new TreeListOperationDelegate(ExpandNodeMethod));
		}
		protected internal virtual void DoNodesOperation(TreeListOperationDelegate operation) {
			owner.NodesIterator.DoOperation(operation);
		}
		protected internal virtual void ExpandNodeMethod(TreeListNode node) {
			ResourceTreeData data = (ResourceTreeData)Owner.GetDataRecordByNode(node);
			if (data != null)
				node.Expanded = ((IInternalResource)data.Resource).IsExpanded;
		}
		protected internal virtual void UpdateNodeCacheMethod(TreeListNode node) {
			ResourceTreeData data = (ResourceTreeData)Owner.GetDataRecordByNode(node);
			if (data != null)
				NodesCache.Add(node, data.Resource);
		}
		protected internal ResourceTreeData CreateTreeListData(ResourceBaseCollection resources) {
			List<ResourceTreeData> rootNodes = new List<ResourceTreeData>();
			foreach (ResourceBase resource in resources) {
				ResourceTreeData data = CreateTreeListDataRecursive(resource);
				rootNodes.Add(data);
			}
			ResourceTreeData treeRoot = new ResourceTreeData(null, Storage);
			treeRoot.Children.AddRange(rootNodes);
			return treeRoot;
		}
		protected internal ResourceTreeData CreateTreeListDataRecursive(Resource resource) {
			ResourceTreeData result = new ResourceTreeData(resource, Storage);
			foreach (ResourceBase child in ((IInternalResource)resource).ChildResources) {
				ResourceTreeData data = CreateTreeListDataRecursive(child);
				result.Children.Add(data);
			}
			return result;
		}
		protected internal virtual void SetInnerControlCore(InnerSchedulerControl value) {
			UnsubscribeSchedulerEvents();
			control = value;
			storage = control != null ? control.Storage : null;
			SubscribeSchedulerEvents();
		}
		#region Events Subscription
		#region SubscribeSchedulerEvents
		protected internal virtual void SubscribeSchedulerEvents() {
			if (control == null)
				return;
			SubscribeSchedulerControlEvents();
			if (storage != null) {
				SubscribeStorageEvents();
			}
		}
		#endregion
		#region UnsubscribeSchedulerEvents
		protected internal virtual void UnsubscribeSchedulerEvents() {
			if (control == null)
				return;
			UnsubscribeSchedulerControlEvents();
			if (storage != null) {
				UnsubscribeStorageEvents();
			}
		}
		#endregion
		#region SubscribeControlEvents
		protected internal virtual void SubscribeSchedulerControlEvents() {
			control.ActiveViewChanged += new EventHandler(OnActiveViewChanged);
			control.StorageChanged += new EventHandler(OnStorageChanged);
		}
		#endregion
		#region UnsubscribeControlEvents
		protected internal virtual void UnsubscribeSchedulerControlEvents() {
			control.ActiveViewChanged -= new EventHandler(OnActiveViewChanged);
			control.StorageChanged -= new EventHandler(OnStorageChanged);
		}
		#endregion
		#region SubscribeStorageEvents
		protected internal virtual void SubscribeStorageEvents() {
			IInternalSchedulerStorage internalStorage = (IInternalSchedulerStorage)storage;
			internalStorage.InternalResourcesChanged += OnResourcesChanged;
			internalStorage.InternalResourcesDeleted += OnResourcesChanged;
			internalStorage.InternalResourcesInserted += OnResourcesChanged;
			internalStorage.InternalResourceCollectionCleared += OnResourcesCollectionChanged;
			internalStorage.InternalResourceCollectionLoaded += OnResourcesCollectionChanged;
			internalStorage.InternalResourceVisibilityChanged += OnResourceVisibilityChanged;
			internalStorage.InternalDeferredNotifications += OnDeferredNotifications;
			internalStorage.BeforeDispose += OnStorageBeforeDispose;
		}
		#endregion
		#region UnsubscribeStorageEvents
		protected internal virtual void UnsubscribeStorageEvents() {
			IInternalSchedulerStorage internalStorage = (IInternalSchedulerStorage)storage;
			internalStorage.InternalResourcesChanged -= OnResourcesChanged;
			internalStorage.InternalResourcesDeleted -= OnResourcesChanged;
			internalStorage.InternalResourcesInserted -= OnResourcesChanged;
			internalStorage.InternalResourceCollectionCleared -= OnResourcesCollectionChanged;
			internalStorage.InternalResourceCollectionLoaded -= OnResourcesCollectionChanged;
			internalStorage.InternalResourceVisibilityChanged -= OnResourceVisibilityChanged;
			internalStorage.InternalDeferredNotifications -= OnDeferredNotifications;
			internalStorage.BeforeDispose -= OnStorageBeforeDispose;
		}
		#endregion
		#region TreeListEvents
		protected internal virtual void SubscribeTreeListEvents() {
		}
		protected internal virtual void UnsubscribeTreeListEvents() {
		}
		#endregion
		#endregion
		protected internal virtual void OnActiveViewChanged(object sender, EventArgs e) {
			Refresh();
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			if (storage != null) {
				UnsubscribeStorageEvents();
			}
			this.storage = InnerControl.Storage;
			if (storage != null) {
				SubscribeStorageEvents();
				Refresh();
			}
		}
		void OnStorageBeforeDispose(object sender, EventArgs e) {
			if (storage == null) 
				return;
			UnsubscribeStorageEvents();
			this.storage = null;
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, EventArgs e) {
			Refresh();
		}
		protected internal virtual void OnDeferredNotifications(object sender, EventArgs e) {
			if (Storage == null)
				return;
			this.deferredRefresh = true;
		}
		protected void EnsureRefresh() {
			if (this.deferredRefresh) {
				this.deferredRefresh = false;
				Refresh();
			}
		}
		protected internal virtual void OnResourcesCollectionChanged(object sender, EventArgs e) {
			Refresh();
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			Refresh();
		}
		protected internal virtual void OnInnerControlChanged() {
			Refresh();
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			Refresh();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			Refresh();
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (disposing) {
					UnsubscribeTreeListEvents();
					UnsubscribeSchedulerEvents();
					this.storage = null;
					this.control = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ResourcesTreeController() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region ResourcesTreeControllerWin
	public class ResourcesTreeControllerWin : ResourcesTreeController {
		#region Fields
		SchedulerControl schedulerControl;
		ILayoutStrategy strategy;
		SchedulerBlocker blockerSchedulerControlNotifications;
		#endregion
		public ResourcesTreeControllerWin(ResourcesTree owner)
			: base(owner) {
			this.blockerSchedulerControlNotifications = new SchedulerBlocker();
		}
		#region Properties
		public new ResourcesTree Owner { get { return (ResourcesTree)base.Owner; } }
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				this.schedulerControl = value;
				if (value != null)
					InnerControl = value.InnerControl;
				else {
					InnerControl = null;
					Owner.DataSource = null;
				}
				UpdateLayoutStrategy();
			}
		}
		#region NodeHeightCalculationStrategy
		protected internal ILayoutStrategy LayoutStrategy {
			get {
				if (strategy == null)
					UpdateLayoutStrategy();
				return strategy;
			}
		}
		#endregion
		protected SchedulerBlocker BlockerSchedulerControlNotifications { get { return blockerSchedulerControlNotifications; } }
		#endregion
		#region Events Subscription
		protected internal override void SubscribeSchedulerControlEvents() {
			if (schedulerControl == null)
				return;
			base.SubscribeSchedulerControlEvents();
			schedulerControl.BeforeDispose += new EventHandler(OnBeforeSchedulerControlDispose);
			schedulerControl.InnerControl.AfterApplyChanges += new AfterApplyChangesEventHandler(OnAfterApplyChanges);
		}
		void OnAfterApplyChanges(object sender, AfterApplyChangesEventArgs e) {
			BlockerSchedulerControlNotifications.Lock();
			try {
				if ((e.Actions & ChangeActions.RecalcViewLayout) != ChangeActions.None) {
					EnsureRefresh();
					UpdateLayout(true);
				}
			} finally {
				BlockerSchedulerControlNotifications.Unlock();
			}
		}
		protected internal override void UnsubscribeSchedulerControlEvents() {
			if (schedulerControl == null)
				return;
			base.UnsubscribeSchedulerControlEvents();
			schedulerControl.BeforeDispose -= new EventHandler(OnBeforeSchedulerControlDispose);
			schedulerControl.InnerControl.AfterApplyChanges -= new AfterApplyChangesEventHandler(OnAfterApplyChanges);
		}
		protected internal virtual void OnTopVisibleNodeIndexChanged(object sender, EventArgs e) {
			schedulerControl.AnimationManager.SetRestrictions();
			try {
				schedulerControl.ActiveView.FirstVisibleResourceIndex = Owner.TopVisibleNodeIndex;
			} finally {
				schedulerControl.AnimationManager.RemoveRestrictions(true);
			}
		}
		protected internal override void OnActiveViewChanged(object sender, EventArgs e) {
			UpdateLayoutStrategy();
			base.OnActiveViewChanged(sender, e);
		}
		void BeforeCollapse(object sender, BeforeCollapseEventArgs e) {
			CollapseExpandResourceCore(e.Node, new CollapseResourceCommand(SchedulerControl));
			UpdateTopVisibleNode();
		}
		void BeforeExpand(object sender, BeforeExpandEventArgs e) {
			CollapseExpandResourceCore(e.Node, new ExpandResourceCommand(SchedulerControl));
			UpdateTopVisibleNode();
		}
		protected internal virtual void CollapseExpandResourceCore(TreeListNode node, ResourceCommandBase command) {
			UnsubscribeSchedulerEvents();
			try {
				Resource res;
				if (!NodesCache.TryGetValue(node, out res))
					return;
				command.Resource = res;
				command.Execute();
			} finally {
				SubscribeSchedulerEvents();
			}
		}
		protected internal override void UnsubscribeTreeListEvents() {
			base.UnsubscribeTreeListEvents();
			Owner.CalcNodeHeight -= new CalcNodeHeightEventHandler(OnCalcNodeHeight);
			Owner.BeforeCollapse -= new BeforeCollapseEventHandler(BeforeCollapse);
			Owner.BeforeExpand -= new BeforeExpandEventHandler(BeforeExpand);
			Owner.TopVisibleNodeIndexChanged -= new EventHandler(OnTopVisibleNodeIndexChanged);
		}
		protected internal override void SubscribeTreeListEvents() {
			base.SubscribeTreeListEvents();
			Owner.CalcNodeHeight += new CalcNodeHeightEventHandler(OnCalcNodeHeight);
			Owner.BeforeCollapse += new BeforeCollapseEventHandler(BeforeCollapse);
			Owner.BeforeExpand += new BeforeExpandEventHandler(BeforeExpand);
			Owner.TopVisibleNodeIndexChanged += new EventHandler(OnTopVisibleNodeIndexChanged);
		}
		#endregion
		public bool CanUpdateScheduler() {
			return !BlockerSchedulerControlNotifications.IsLocked;
		}
		protected internal override void UpdateColumnPanel() {
			if (SchedulerControl == null)
				return;
			LayoutStrategy.UpdateColumnPanelRowHeight();
		}
		protected internal override void UpdateLayout() {
			if (Owner.IsLoaded)
				base.UpdateLayout();
		}
		protected internal override void UpdateNodesCache() {
			if (Owner.IsLoaded)
				base.UpdateNodesCache();
		}
		void OnCalcNodeHeight(object sender, CalcNodeHeightEventArgs e) {
			if (e.Node.GetType() == typeof(TreeListAutoFilterNode))
				CalculateAutoFilterNodeHeight(e);
			else
				CalculateNodeHeight(e);
			return;
		}
		protected internal void CalculateAutoFilterNodeHeight(CalcNodeHeightEventArgs e) {
			e.NodeHeight = LayoutStrategy.AutoFilterNodeHeight;
		}
		protected internal void CalculateNodeHeight(CalcNodeHeightEventArgs e) {
			Resource res;
			if (!NodesCache.TryGetValue(e.Node, out res))
				return;
			e.NodeHeight = LayoutStrategy.CalculateNodeHeight(res);
		}
		void OnBeforeSchedulerControlDispose(object sender, EventArgs e) {
			UnsubscribeSchedulerControlEvents();
			this.schedulerControl = null;
			UpdateLayoutStrategy();
		}
		protected internal virtual ILayoutStrategy CreateLayoutStrategy() {
			if (SchedulerControl == null)
				return new NonTimeLineLayoutStrategy();
			SchedulerViewType viewType = SchedulerControl.ActiveView.Type;
			if (viewType == SchedulerViewType.Gantt || viewType == SchedulerViewType.Timeline)
				return new TimeLineLayoutStrategy(Owner);
			return new NonTimeLineLayoutStrategy();
		}
		void UpdateLayoutStrategy() {
			strategy = CreateLayoutStrategy();
		}
		protected internal override void UpdateFilterCriteria() {
			if (schedulerControl == null)
				return;
			((IInternalResourceStorage)SchedulerControl.DataStorage.Resources).ResourcesTreeFilter = GetFilterString();
		}
		protected internal string GetFilterString() {
			bool applyFilter = Owner.OptionsBehavior.EnableFiltering && Owner.ActiveFilterEnabled;
			if (!applyFilter)
				return String.Empty;
			String result = Owner.ActiveFilterString;
			MappingCollection mappings = ((IInternalResourceStorage)SchedulerControl.DataStorage.Resources).ActualMappings;
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				MappingBase mapping = mappings[i];
				result = result.Replace(mapping.Member, mapping.Name);
			}
			return result;
		}
	}
	#endregion
	public interface ILayoutStrategy {
		int AutoFilterNodeHeight { get; }
		int CalculateNodeHeight(Resource res);
		bool ActualAutoNodeHeight { get; }
		void UpdateColumnPanelRowHeight();
	}
	public class TimeLineLayoutStrategy : ILayoutStrategy {
		ResourcesTree resourcesTree;
		const int MinColumnPanelHeight = 18;
		const int TreeListRowSeparatorHeight = 1;
		public TimeLineLayoutStrategy(ResourcesTree resourceTree) {
			Guard.ArgumentNotNull(resourceTree, "resourceTree");
			Guard.ArgumentNotNull(resourceTree.SchedulerControl, "resourceTree.SchedulerControl");
			this.resourcesTree = resourceTree;
		}
		internal ResourcesTree ResourcesTree { get { return resourcesTree; } }
		internal SchedulerViewBase ActiveView { get { return ResourcesTree.SchedulerControl.ActiveView; } }
		internal int VerticalOverlap { get { return ActiveView.ViewInfo.Painter.HorizontalHeaderPainter.VerticalOverlap; } }
		public int AutoFilterNodeHeight {
			get {
				return ResourcesTree.OptionsView.ShowAutoFilterRow ? ResourcesTree.ViewInfo.RowHeight : 0;
			}
		}
		public bool ActualAutoNodeHeight { get { return false; } }
		protected internal virtual int CalculateActualAutoFilterRowHeight() {
			if (AutoFilterNodeHeight == 0)
				return 0;
			return AutoFilterNodeHeight + ResourcesTree.ViewInfo.ViewRects.TopRowSeparator.Height;
		}
		public int CalculateNodeHeight(Resource res) {
			int resourceHeight = CalculateResourceHeight(res);
			int result = resourceHeight > 0 ? resourceHeight : -1;
			return result;
		}
		int CalculateResourceHeight(Resource res) {
			SchedulerViewCellContainerCollection containers = ActiveView.ViewInfo.CellContainers;
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = containers[i];
				if (ResourceBase.MatchIds(res, container.Resource))
					return GetResourceHeight(containers, i);
			}
			return 0;
		}
		protected internal virtual int GetResourceHeight(SchedulerViewCellContainerCollection containers, int i) {
			if (i == containers.Count - 1)
				return GetLastResourceHeight(containers);
			if (i == 0)
				return GetFirstResourceHeight(containers);
			return GetResourceHeightCore(containers, i);
		}
		protected internal virtual int GetLastResourceHeight(SchedulerViewCellContainerCollection containers) {
			int result = containers[containers.Count - 1].Bounds.Height + (CalculateResourceContainerSeparatorHeight(containers) - TreeListRowSeparatorHeight) - TreeListRowSeparatorHeight;
			int totalHeight = ResourcesTree.ProtectedCalculateVisibleRowsAreaHeight();
			int magicDelta = totalHeight - (containers[containers.Count - 1].Bounds.Bottom - containers[0].Bounds.Top);
			return result + magicDelta;
		}
		protected internal virtual int GetFirstResourceHeight(SchedulerViewCellContainerCollection containers) {
			int separatorHeight = CalculateResourceContainerSeparatorHeight(containers);
			int actualTreeListRowSeparatorHeight = CalculateActualTreeListRowSeparatorHeight();
			int resHeight = GetResourceHeightCore(containers, 0) - (separatorHeight - actualTreeListRowSeparatorHeight);
			int panelHeight = CalculateActualColumnPanelHeight() + CalculateActualAutoFilterRowHeight();
			int headersHeight = CalculateSchedulerHeadersHeight();
			return Math.Max(0, headersHeight + resHeight - panelHeight);
		}
		int CalculateActualTreeListRowSeparatorHeight() {
			return (this.ResourcesTree.OptionsView.ShowHorzLines) ? TreeListRowSeparatorHeight : 0;
		}
		int CalculateResourceContainerSeparatorHeight(SchedulerViewCellContainerCollection containers) {
			if (containers.Count < 2)
				return 0;
			return containers[1].Bounds.Top - containers[0].Bounds.Bottom;
		}
		protected internal virtual int GetResourceHeightCore(SchedulerViewCellContainerCollection containers, int i) {
			int actualTreeListRowSeparatorHeight = CalculateActualTreeListRowSeparatorHeight();
			return containers[i + 1].Bounds.Top - containers[i].Bounds.Top - actualTreeListRowSeparatorHeight;
		}
		public void UpdateColumnPanelRowHeight() {
			ResourcesTree.ColumnPanelRowHeight = CalculateActualColumnPanelHeight();
		}
		protected internal int CalculateActualColumnPanelHeight() {
			if (!ResourcesTree.OptionsView.ShowColumns)
				return 0;
			int headersHeight = CalculateSchedulerHeadersHeight();
			return Math.Max(MinColumnPanelHeight, headersHeight - CalculateActualAutoFilterRowHeight());
		}
		protected internal int CalculateSchedulerHeadersHeight() {
			if (ActiveView.ViewInfo == null)
				return 0;
			SchedulerViewCellContainerCollection cells = ActiveView.ViewInfo.CellContainers;
			if (cells.Count == 0)
				return 0;
			return cells[0].Bounds.Top - ActiveView.Control.ClientBounds.Top - VerticalOverlap;
		}
	}
	public class NonTimeLineLayoutStrategy : ILayoutStrategy {
		public bool ActualAutoNodeHeight { get { return true; } }
		public int CalculateNodeHeight(Resource res) {
			return 0;
		}
		public int AutoFilterNodeHeight { get { return 0; } }
		public void UpdateColumnPanelRowHeight() {
		}
	}
	#region ResourceTreeData
	public class ResourceTreeData : DevExpress.XtraTreeList.TreeList.IVirtualTreeListData {
		Resource resource;
		ISchedulerStorageBase storage;
		List<ResourceTreeData> children;
		public ResourceTreeData(Resource resource, ISchedulerStorageBase storage) {
			this.resource = resource;
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
			this.children = new List<ResourceTreeData>();
		}
		public Resource Resource { get { return resource; } set { resource = value; } }
		public List<ResourceTreeData> Children { get { return children; } }
		#region IVirtualTreeListData Members
		void TreeList.IVirtualTreeListData.VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info) {
			if (resource != null)
				info.CellData = ResourceHierarchyHelper.GetResourceFieldValue(resource, info.Column.FieldName, storage);
		}
		void TreeList.IVirtualTreeListData.VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info) {
			info.Children = children;
		}
		void TreeList.IVirtualTreeListData.VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info) {
			info.Cancel = true;
		}
		#endregion
	}
	#endregion
}
