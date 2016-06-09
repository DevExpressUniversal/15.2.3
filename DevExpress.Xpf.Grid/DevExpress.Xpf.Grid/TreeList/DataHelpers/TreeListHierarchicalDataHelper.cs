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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Utils;
using System.Windows;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using DevExpress.Xpf.GridData;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListHierarchicalDataHelper : TreeListBoundDataHelper {
		Dictionary<object, TreeListNodeCollection> notificationDictionary;
		Dictionary<TreeListNodeCollection, object> mirrorNotificationDictionary;
		public readonly int MaxNodeId = int.MaxValue / 2;
		protected internal int NodeCounter { get; set; }
		public TreeListHierarchicalDataHelper(TreeListDataProvider provider, object dataSource)
			: base(provider, dataSource) {
		}
		public override void DeleteNode(TreeListNode node, bool deleteChildren, bool modifySource) {
			if(node == null) return;
			IList sourceList = null;
			if(modifySource) {
				TreeListNode parent = node.ParentNode;
				if(parent == null)
					sourceList = ListSource;
				else 
					sourceList = parent.ItemsSource;
				if(sourceList != null)
					sourceList.Remove(node.Content);
			}
			RemoveTreeListNode(node);
		}
		protected internal Dictionary<object, TreeListNodeCollection> NotificationDictionary {
			get {
				if(notificationDictionary == null)
					notificationDictionary = new Dictionary<object, TreeListNodeCollection>();
				return notificationDictionary;
			}
		}
		protected internal Dictionary<TreeListNodeCollection, object> MirrorNotificationDictionary {
			get {
				if(mirrorNotificationDictionary == null)
					mirrorNotificationDictionary = new Dictionary<TreeListNodeCollection, object>();
				return mirrorNotificationDictionary;
			}
		}
		void ClearNotifications(List<TreeListNode> nodes) {
			foreach(TreeListNode node in nodes) {
				ClearNotifications(node.Nodes);
				ClearNodeNotification(node);
			}
		}
		void ClearNotifications(TreeListNodeCollection nodes) {
			ClearNotifications(nodes, true);
		}
		void ClearNotifications(TreeListNode node, bool recursive) {
			ClearNodeNotification(node);
			if(recursive)
				ClearNotifications(node.Nodes, recursive);
		}
		void ClearNotifications(TreeListNodeCollection nodes, bool recursive) {
			if(recursive)
				foreach(TreeListNode node in new TreeListNodeIterator(nodes))
					ClearNodeNotification(node);
			else
				foreach(TreeListNode node in nodes)
					ClearNodeNotification(node);
		}
		void UpdateNodeChildren(TreeListNodeCollection nodes, object s, ListChangedEventArgs e) {
			IBindingList list = s as IBindingList;
			if(list == null)
				return;
#if !SL
			System.Windows.Threading.Dispatcher dispatcher = View.Dispatcher;
			if(dispatcher == null || dispatcher.CheckAccess())
#endif
				UpdateNodeChildrenCore(nodes, e, list);
#if !SL
			else dispatcher.BeginInvoke(new System.Action(() => UpdateNodeChildrenCore(nodes, e, list)));
#endif
			nodes.Owner.IsExpandButtonVisible = DefaultBoolean.Default;
		}
		void UpdateNodeChildrenCore(TreeListNodeCollection nodes, ListChangedEventArgs e, IBindingList list) {
			if(e.ListChangedType == ListChangedType.Reset)
				OnListReset(nodes, list);
			else
				OnListChanged(nodes, list, e);
		}
		void OnListChanged(TreeListNodeCollection nodes, IBindingList list, ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
					OnItemAdded(nodes, list, e.NewIndex);
					break;
				case ListChangedType.ItemChanged:
					OnItemChanged(nodes, list, e);
					break;
				case ListChangedType.ItemDeleted:
					OnItemDeleted(nodes, list, e);
					break;
				case ListChangedType.ItemMoved:
					OnItemMoved(nodes, e);
					break;
			}
		}
		protected virtual void OnItemChanged(TreeListNodeCollection nodes, IBindingList list, ListChangedEventArgs e) {
			if(e.PropertyDescriptor == null && e.OldIndex != e.NewIndex)
				OnListReset(nodes, list);
			IList<TreeListNode> changedNodes = DataProvider.FindNodesByValue(list[e.NewIndex]);
			if(changedNodes == null || changedNodes.Count == 0) {
				OnListReset(nodes, list);
				changedNodes = DataProvider.FindNodesByValue(list[e.NewIndex]);
			}
			if(changedNodes != null) {
				foreach(TreeListNode node in changedNodes) {
					node.IsChecked = DataProvider.GetObjectIsChecked(node);
					if(!IsLoading)
						node.ForceUpdateExpandState();
					UpdateChildren(node, e);
					DataProvider.OnNodeCollectionChanged(node, NodeChangeType.Content);
				}
			}
		}
		void UpdateChildren(TreeListNode node, ListChangedEventArgs e) {
			if(!View.AllowChildNodeSourceUpdates || !node.ChildrenWereEverFetched)
				return;
			if(View.TreeDerivationMode == TreeDerivationMode.ChildNodesSelector &&
				View.ChildNodesSelector == null &&
				e.PropertyDescriptor != null &&
				e.PropertyDescriptor.Name == View.ChildNodesPath) {
					ReloadChildNodes(node);
			}
			else {
				var children = GetChildren(node);
				if(!ReferenceEquals(children, node.ItemsSource)) {
					ReloadChildNodes(node, children);
				}
			}
		}
		public override void ReloadChildNodes(TreeListNode node, IEnumerable children = null) {
			if(node == null)
				return;
			ClearNotifications(node, true);
			DataProvider.SaveFocusState();
			DataProvider.BeginUpdateCore();
			try {
				node.Nodes.Clear();
				CreateChildren(node, children);
				if(node.IsExpanded && (View.FetchSublevelChildrenOnExpand || CreateAllNodes))
					FillChildrenSubNodes(node, CreateAllNodes);
			}
			finally {
				DataProvider.CancelUpdate();
				DataProvider.RestoreFocusState();
				UpdateNodes(node);
			}
			DataProvider.OnNodeCollectionChanged(node, NodeChangeType.Expand);
		}
		protected virtual void OnItemAdded(TreeListNodeCollection nodes, IBindingList list, int index) {
			TreeListNode node = DataProvider.CreateNode(list[index]);
			node.DataProvider = DataProvider;
			node.InitIsChecked();
			DataProvider.SaveFocusState();
			DataProvider.BeginUpdateCore();
			try {
				nodes.Insert(index, node);
				AddNode(nodes, node);
			}
			finally {
				DataProvider.CancelUpdate();
				DataProvider.RestoreFocusState();
			}
			DataProvider.OnNodeCollectionChanged(node, NodeChangeType.Add, false);
		}
		protected virtual void OnItemMoved(TreeListNodeCollection nodes, ListChangedEventArgs e) {
			DataProvider.BeginUpdateCore();
			DataProvider.SaveNodesState();
			TreeListNode movedNode = nodes[e.OldIndex];
			try {
				nodes.Remove(movedNode);
				nodes.Insert(e.NewIndex, movedNode);
			}
			finally {
				DataProvider.RestoreNodesState();
				DataProvider.CancelUpdate();
			}
			DataProvider.OnNodeCollectionChanged(movedNode, NodeChangeType.Add, false);
		}
		protected internal override void UpdateNodeId(TreeListNode node) {
			node.Id = NodeCounter++;
		}
		protected override void CalcNodeIds() {
			NodeCounter = 0;
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
				node.Id = NodeCounter++;
		}
		protected internal override void RecalcNodeIdsIfNeeded() {
			if(NodeCounter >= MaxNodeId)
				CalcNodeIds();
		}
		protected virtual void OnItemDeleted(TreeListNodeCollection nodes, IBindingList list, ListChangedEventArgs e) {
			List<TreeListNode> oldNodes;
			DataProvider.SaveFocusState();
			DataProvider.BeginUpdateCore();
			try {
				oldNodes = GetOldNodes(nodes, list, e);
				ClearNotifications(oldNodes);
				RemoveOldNodes(nodes, oldNodes);
			}
			finally {
				DataProvider.CancelUpdate();
				DataProvider.RestoreFocusState();
			}
			DataProvider.OnNodeCollectionChanged(oldNodes.Count > 0 ? oldNodes[0] : null, NodeChangeType.Remove, false);
		}
		protected virtual void OnListReset(TreeListNodeCollection nodes, IBindingList list) {
			DataProvider.BeginUpdate();
			try {
				ClearNotifications(nodes, true);
				nodes.Clear();
				AddNewNodes(nodes, list);
			} finally {
				DataProvider.EndUpdate();
			}
		}
		List<TreeListNode> GetOldNodes(TreeListNodeCollection treeListNodeCollection, IBindingList list, ListChangedEventArgs e) {
			List<TreeListNode> nodes = new List<TreeListNode>();
			ArrayList sourceList = new ArrayList(list);
			foreach(TreeListNode node in treeListNodeCollection) {
				if(sourceList.Count > 0 && sourceList.Contains(node.Content)) {
					sourceList.Remove(node.Content);
					continue;
				}
				nodes.Add(node);
			}
			return nodes;
		}
		void AddNewNodes(TreeListNodeCollection nodes, IEnumerable list) {
			foreach(var item in list)
				AddNewNode(nodes, item);
		}
		void AddNewNode(TreeListNodeCollection nodes, object item) {
			TreeListNode node = DataProvider.CreateNode(item);
			nodes.Add(node);
			AddNode(nodes, node);
		}
		protected virtual void AddNode(TreeListNodeCollection nodes, TreeListNode node) {
			if(View.FetchSublevelChildrenOnExpand || CreateAllNodes) {
				AddNotification(node.Nodes, GetChildren(node));
				FillSubNodes(node);
				if(CreateAllNodes)
					FillChildrenSubNodes(node, CreateAllNodes);
			}
			else
				node.IsExpandButtonVisible = DefaultBoolean.True;
		}
		public override void Dispose() {
			ResetNotifications();
			base.Dispose();
			DataProvider.Nodes.Clear();
		}
		void RemoveOldNodes(TreeListNodeCollection nodes, IEnumerable oldNodes) {
			foreach(TreeListNode nodeChild in oldNodes)
				nodes.Remove(nodeChild);
		}
		protected virtual IEnumerable GetChildren(TreeListNode node) {
			if(node == null)
				return null;
			object content = node.Content;
			if(content == null)
				return null;
			if(View.ChildNodesSelector != null) {
				IEnumerable children = View.ChildNodesSelector.SelectChildren(content);
				if(children != null)
					return children;
			}
			if(!string.IsNullOrEmpty(View.ChildNodesPath)) {
				PropertyDescriptor prop = TypeDescriptor.GetProperties(content).Find(View.ChildNodesPath, false);
				if(prop == null)
					return null;
				return prop.GetValue(content) as IEnumerable;
			}
			return null;
		}
		public override void LoadData() {
			ResetNotifications();
			DataProvider.Nodes.Clear();
			if(ListSource == null) return;
			AddNodesFromItems();
			if(View.FetchSublevelChildrenOnExpand || CreateAllNodes)
				FillChildrenSubNodes(DataProvider.RootNode, CreateAllNodes);
		}
		protected bool CreateAllNodes { get { return !View.EnableDynamicLoading; } }
		public void ResetNotifications() {
			foreach(var item in NotificationDictionary) {
				IBindingList bList = item.Key as IBindingList;
				if(bList != null)
					bList.ListChanged -= OnBindingListChanged;
			}
			NotificationDictionary.Clear();
			MirrorNotificationDictionary.Clear();
		}
		void AddNodesFromItems() {
			AddNodesFromItems(DataProvider.RootNode, ListSource);
			AddNotification(DataProvider.Nodes, ListSource);
		}
		void AddNodesFromItems(TreeListNode treeListNode, IEnumerable children) {
			BeginLoad();
			DataProvider.BeginUpdateCore();
			try {
				TreeListNodeCollection treeListNodeCollection = treeListNode.Nodes;
				foreach(object item in children) {
					TreeListNode nodeItem = DataProvider.CreateNode(item);
					nodeItem.DataProvider = DataProvider;
					nodeItem.InitIsChecked();
					nodeItem.IsExpandButtonVisible = DefaultBoolean.True;
					nodeItem.Template = GetItemTemplate(treeListNode);
					treeListNodeCollection.Add(nodeItem);
#if !SL
					if(nodeItem.Template == null)
						AsignImplicitDataTemplate(nodeItem);
#endif
				}
			}
			finally {
				DataProvider.CancelUpdate();
				UpdateNodes(treeListNode);
				EndLoad();
				if(!IsLoading && (View.ExpandStateBinding != null || !string.IsNullOrEmpty(View.ExpandStateFieldName)))
					DataProvider.UpdateNodesExpandState(treeListNode.Nodes, false);
			}
		}
		protected virtual DataTemplate GetItemTemplate(TreeListNode treeListNode) { return null; }
		protected virtual void AsignImplicitDataTemplate(TreeListNode treeListNode) { }
		protected override PropertyDescriptor GetActualComplexPropertyDescriptor(TreeListDataProvider provider, object obj, string name) {
			if(provider.DataSource is ITypedList)
				return new TreeListComplexPropertyDescriptor(provider, obj, name);
			return new UnitypeComplexPropertyDescriptor(provider, obj, name);
		}
		protected override PropertyDescriptor GetActualPropertyDescriptor(PropertyDescriptor descriptor) {
			if(DataProvider.DataSource is ITypedList)
				return descriptor;
			return new UnitypeDataPropertyDescriptor(descriptor);
		}
		bool CreateChildren(TreeListNode node, IEnumerable children = null) {
			if(children == null) {
				children = TryGetChildrenFromNoteDictionary(node);
				if(children == null)
					children = GetChildren(node);
				if(children == null)
					return false;
			}
			AddNotification(node.Nodes, children);
			AddNodesFromItems(node, children);
			node.ItemsSource = children as IList;
			foreach(var item in children)
				return true;
			return false;
		}
		IEnumerable TryGetChildrenFromNoteDictionary(TreeListNode node) {
			KeyValuePair<object, TreeListNodeCollection> pair = new KeyValuePair<object, TreeListNodeCollection>(null, null);
			if(MirrorNotificationDictionary.ContainsKey(node.Nodes))
				return MirrorNotificationDictionary[node.Nodes] as IEnumerable;
			return null;
		}
		void AddNotification(TreeListNodeCollection nodes, IEnumerable list) {
			if(list == null || NotificationDictionary.ContainsKey(list) || MirrorNotificationDictionary.ContainsKey(nodes)) return;
			IBindingList bindingList = ExtractListSource(list) as IBindingList;
			if(bindingList != null) {
				NotificationDictionary.Add(bindingList, nodes);
				MirrorNotificationDictionary.Add(nodes, bindingList);
				bindingList.ListChanged += OnBindingListChanged;
			}
		}
		void OnBindingListChanged(object sender, ListChangedEventArgs e) {
			IBindingList collection = sender as IBindingList;
			if(NotificationDictionary.ContainsKey(collection))
				UpdateNodeChildren(NotificationDictionary[collection], sender, e);
		}
		void ClearNodeNotification(TreeListNode node) {
			object key = null;
			if(MirrorNotificationDictionary.TryGetValue(node.Nodes, out key)) {
				(key as IBindingList).ListChanged -= OnBindingListChanged;
				MirrorNotificationDictionary.Remove(NotificationDictionary[key]);
				NotificationDictionary.Remove(key);
			}
		}
		protected override void PatchColumnCollection(PropertyDescriptorCollection properties) {
			List<IColumnInfo> newInfos = new List<IColumnInfo>();
			IList<IColumnInfo> infos = View.GetColumns();
			foreach(IColumnInfo colInfo in infos) {
				bool present = false;
				if(properties != null)
					foreach(PropertyDescriptor descript in properties)
						if(descript.Name == colInfo.FieldName) {
							present = true;
							break;
						}
				if(!present)
					newInfos.Add(colInfo);
			}
			foreach(IColumnInfo column in newInfos) {
				if(column.UnboundType == UnboundColumnType.Bound
					&& !string.IsNullOrEmpty(column.FieldName)
					&& !column.FieldName.Contains("."))
					PopulateColumn(new UnitypeDataPropertyDescriptor(column.FieldName, column.ReadOnly));
			}
			if(!string.IsNullOrEmpty(View.CheckBoxFieldName)) {
				bool present = false;
				foreach(PropertyDescriptor descript in properties)
					if(descript.Name == View.CheckBoxFieldName) {
						present = true;
						break;
					}
				if(!present)
					PopulateColumn(new UnitypeDataPropertyDescriptor(View.CheckBoxFieldName, false));
			}
		}
		public override void NodeExpandingCollapsing(TreeListNode treeListNode) {
			if(CreateAllNodes) return;
			DataProvider.SaveFocusState();
			if(View.DataControl != null)
				View.DataControl.CurrentItemChangedLocker.Lock();
			try {
				if(treeListNode.Nodes.Count == 0) 
					FillSubNodes(treeListNode);
				if(View.FetchSublevelChildrenOnExpand)
					FillChildrenSubNodes(treeListNode);
			}
			finally {
				RecalcNodeIdsIfNeeded();
				if(View.DataControl != null)
					View.DataControl.CurrentItemChangedLocker.Unlock();
				DataProvider.RestoreFocusState();
			}
		}
		void UpdateNodes(TreeListNode parentNode) {
			DataProvider.DoSortNodes(parentNode);
			DataProvider.DoFilterNodes(parentNode);
			DataProvider.SynchronizeSummary();
		}
		void FillChildrenSubNodes(TreeListNode treeListNode, bool createAllNodes = false) {
			foreach(TreeListNode node in treeListNode.Nodes) {
				if(node.Nodes.Count == 0) {
					FillSubNodes(node);
					if(createAllNodes)
						FillChildrenSubNodes(node, createAllNodes);
				}
			}
		}
		void FillSubNodes(TreeListNode treeListNode) {
			if(!treeListNode.ChildrenWereEverFetched) {
				CreateChildren(treeListNode);
				treeListNode.isExpandButtonVisible = DefaultBoolean.Default;
				treeListNode.ChildrenWereEverFetched = true;
			}
		}
		protected internal object GetRowData(TreeListNode node) {
			if(DataProvider.IsValidVisibleIndex(node.VisibleIndex)) {
				RowData rowData = View.GetRowData(node.RowHandle);
				if(rowData != null) {
					rowData.UpdateData();
					return rowData;
				}
			}
			return CreateFakeRowData(node);
		}
		protected RowData CreateFakeRowData(TreeListNode node) {
			RowData rowData = new TreeListRowData(View.VisualDataTreeBuilder);
			rowData.AssignFrom(node.RowHandle);
			return rowData;
		}
	}
}
