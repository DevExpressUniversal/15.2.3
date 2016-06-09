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

using DevExpress.Xpf.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Data.Helpers;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Utils;
using System.Windows;
using System.Windows.Data;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListSelfReferenceDataHelper : TreeListBoundDataHelper {
		bool requiresReloadDataOnEndUpdateCore = true;
		IBindingList bindingList;
		public TreeListSelfReferenceDataHelper(TreeListDataProvider provider, object dataSource)
			: base(provider, dataSource) {
		}
		protected override IBindingList BindingList {
			get { return bindingList; }
			set {
				if(BindingList != null)
					BindingList.ListChanged -= OnBindingListChanged;
				bindingList = value;
				if(BindingList != null)
					BindingList.ListChanged += OnBindingListChanged;
			}
		}
		public override bool RequiresReloadDataOnEndUpdate {
			get { return requiresReloadDataOnEndUpdateCore; }
			internal set {
				if(requiresReloadDataOnEndUpdateCore == value)
					return;
				requiresReloadDataOnEndUpdateCore = value;
			}
		}
		public override void Dispose() {
			base.Dispose();
			BindingList = null;
		}
		protected override bool CanPopulate(PropertyDescriptor descriptor) { return true; }
		protected virtual bool IsDetailDescriptor(PropertyDescriptor descriptor) {
			return typeof(IList).IsAssignableFrom(descriptor.PropertyType) && !typeof(Array).IsAssignableFrom(descriptor.PropertyType);
		}
		protected object GetListItem(int index) {
			if(ListSource.Count > index)
				return ListSource[index];
			return null;
		}
		protected virtual void OnBindingListChanged(object sender, ListChangedEventArgs e) {
#if !SL
			System.Windows.Threading.Dispatcher dispatcher = View.Dispatcher;
			if(dispatcher == null || dispatcher.CheckAccess())
#endif
				OnListChanged(e);
#if !SL
			else dispatcher.BeginInvoke(new Action(() => { OnListChanged(e); }));
#endif
		}
		void OnListChanged(ListChangedEventArgs e) {
			DataProvider.TryRepopulateColumns();
			OnBindingListChanged(e);
		}
		void OnBindingListChanged(ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.PropertyDescriptorAdded || e.ListChangedType == ListChangedType.PropertyDescriptorDeleted ||
				e.ListChangedType == ListChangedType.PropertyDescriptorChanged) {
				DataProvider.OnDataSourceChanged();
				return;
			}
			if(DataProvider.IsUpdateLocked && RequiresReloadDataOnEndUpdate) return;
			switch(e.ListChangedType) {
				case ListChangedType.ItemChanged:
					OnItemChanged(e.NewIndex, e.PropertyDescriptor != null ? e.PropertyDescriptor.Name : String.Empty);
					break;
				case ListChangedType.ItemAdded:
					OnItemAdded(e.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					OnItemDeleted(e.NewIndex);
					break;
				default:
					OnReset();
					break;
			}
		}
		protected virtual void OnItemChanged(int index, string memberName) {
			object item = GetListItem(index);
			if(item == null) return;
			TreeListNode node = TryFindNodeByValue(item);
			if(node == null)
				return;
			node.IsChecked = DataProvider.GetObjectIsChecked(node);
			if(!IsLoading)
				node.ForceUpdateExpandState();
			if(CheckParentIDChanging(node, memberName))
				return;
			DataProvider.OnNodeCollectionChanged(node, NodeChangeType.Content);
		}
		TreeListNode TryFindNodeByValue(object item) {
			TreeListNode node = DataProvider.FindNodeByValue(item);
			if(node == null) {
				OnReset();
				node = DataProvider.FindNodeByValue(item);
			}
			return node;
		}
		protected virtual bool CheckParentIDChanging(TreeListNode node, string memberName) {
			if(!(IsValidColumnName(View.KeyFieldName) && IsValidColumnName(View.ParentFieldName))) return false;
			bool shouldReload = false;
			if(!String.IsNullOrEmpty(memberName))
				shouldReload = (memberName == View.ParentFieldName || memberName == View.KeyFieldName);
			else {
				object obj1 = GetValue(node, View.ParentFieldName);
				object obj2 = (node.ParentNode == null ? View.RootValue : GetValue(node.ParentNode, View.KeyFieldName));
				shouldReload = !object.Equals(obj1, obj2);
			}
			if(shouldReload)
				OnReset();
			return shouldReload;
		}
		protected virtual void OnItemAdded(int index) {
			DataProvider.SaveFocusState();
			object item = GetListItem(index);
			if(item == null) return;
			TreeListNode node = DataProvider.CreateNode(item);
			node.DataProvider = DataProvider;
			node.InitIsChecked();
			object id = GetValue(node, View.KeyFieldName);
			if(DataProvider.FindNodeByValue(View.KeyFieldName, id) != null) {
				OnReset();
				return;
			}
			object parentId = GetValue(node, View.ParentFieldName);
			if(parentId == null || Object.Equals(parentId, View.RootValue)) {
				DataProvider.Nodes.Add(node);
			}
			else {
				TreeListNode parentNode = DataProvider.FindNodeByValue(View.KeyFieldName, parentId);
				if(parentNode != null)
					parentNode.Nodes.Add(node);
				else
					DataProvider.Nodes.Add(node);
				IEnumerable<TreeListNode> childNodes = GetChildNodesById(id);
				if(childNodes != null)
					MoveChildNodesToParent(node, childNodes);
			}
			UpdateNodesIdsOnIndexInsert(node, index);
			DataProvider.RestoreFocusState();
			DataProvider.SortNodes(node.ParentNode == null ? View.Nodes : node.ParentNode.Nodes, false);
			View.UpdateRows();
		}
		IEnumerable<TreeListNode> GetChildNodesById(object id) {
			IEnumerable<TreeListNode> childNodes = DataProvider.Nodes.Where((node) => {
				object nodeParentId = DataProvider.GetNodeValue(node, View.ParentFieldName);
				return nodeParentId != null && nodeParentId.Equals(id) && !nodeParentId.Equals(DataProvider.GetNodeValue(node, View.KeyFieldName));
			});
			return childNodes.ToList();
		}
		void MoveChildNodesToParent(TreeListNode parentNode, IEnumerable<TreeListNode> children) {
			foreach(TreeListNode child in children) {
				DataProvider.Nodes.Remove(child);
				parentNode.Nodes.Add(child);
			}
		}
		protected virtual void OnItemDeleted(int index) {
			DataProvider.SaveFocusState();
			try {
				TreeListNode node = DataProvider.FindNodeById(index);
				if(node != null) {
					DeleteNode(node, false, false);
					UpdateNodesIdsOnIndexRemove(index);
				}
			}
			finally {
				DataProvider.RestoreFocusState();
			}
		}
		protected virtual void OnReset() {
			requiresReloadDataOnEndUpdateCore = false;
			DataProvider.BeginUpdate();
			try {
				LoadData();
			}
			finally {
				DataProvider.EndUpdate();
				requiresReloadDataOnEndUpdateCore = true;
			}
		}
		public void DeleteNode(TreeListNode node) {
			DeleteNode(node, false, true);
		}
		public void DeleteNode(TreeListNode node, bool deleteChildren) {
			DeleteNode(node, deleteChildren, true);
		}
		public override void DeleteNode(TreeListNode node, bool deleteChildren, bool modifySource) {
			if(!IsReady) return;
			if(deleteChildren) {
				if(modifySource) {
					List<object> objectsToRemove = new List<object>();
					objectsToRemove.AddRange(new TreeListNodeIterator(node.Nodes).Select<TreeListNode, object>(n => n.Content));
					objectsToRemove.Add(node.Content);
					RemoveObjectsFromSource(objectsToRemove);
				}
			} else {
				DataProvider.LockRecursiveNodesUpdate();
				try {
					if(modifySource)
						ListSource.Remove(node.Content);
					List<TreeListNode> nodesList = node.Nodes.ToList<TreeListNode>();
					int rootParentIndex = GetRootParentIndex(node);
					foreach(TreeListNode child in nodesList) {
						rootParentIndex++;
						DataProvider.Nodes.Insert(rootParentIndex, child);
					}
					node.Nodes.Clear();
				} finally {
					DataProvider.UnlockRecursiveNodesUpdate();
				}
			}
			RemoveTreeListNode(node);
		}
		void RemoveObjectsFromSource(List<object> objectsToRemove) {
			DataProvider.LockRecursiveNodesUpdate();
			try {
				foreach(object obj in objectsToRemove)
					ListSource.Remove(obj);
			} finally {
				DataProvider.UnlockRecursiveNodesUpdate();
			}
		}
		internal void SyncNodesIds(List<int> deletedIds) {
			deletedIds.Sort();
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
				node.Id -= GetMaxIndex(deletedIds, node.Id);
		}
		int GetMaxIndex(List<int> deletedIds, int nodeId) {
			int i = 0;
			foreach(int id in deletedIds) {
				if(nodeId <= id)
					break;
				i++;
			}
			return i;
		}
		void UpdateNodesIdsOnIndexInsert(TreeListNode treeNode, int index) {
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes)) {
				if(node.Id >= index)
					node.Id++;
			}
			treeNode.Id = index;
		}
		void UpdateNodesIdsOnIndexRemove(int index) {
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
				if(node.Id > index)
					node.Id--;
		}
		protected override bool IsColumnsVisibleCore(DataColumnInfo column) {
			return base.IsColumnsVisibleCore(column) && (column.Name != View.KeyFieldName && column.Name != View.ParentFieldName);
		}
		bool IsValidColumnName(string fieldName) {
			return Columns[fieldName] != null;
		}
		public override object GetDataRowByListIndex(int listIndex) {
			if(!IsReady) return null;
			if(listIndex < 0 || listIndex >= ListSource.Count) return null;
			return ListSource[listIndex];
		}
		public override object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) {
			if(!IsReady)
				return null;
			if(listSourceRowIndex < 0 || listSourceRowIndex >= ListSource.Count)
				return null;
			TreeListNode node = DataProvider.FindNodeByValue(ListSource[listSourceRowIndex]);
			if(node == null)
				return null;
			return DataProvider.GetNodeValue(node, fieldName);
		}
		public override int GetListIndexByDataRow(object row) {
			if(!IsReady || row == null) return -1;
			if(ListSource.Contains(row))
				return ListSource.IndexOf(row);
			return -1;
		}
		public override void LoadData() {
			CheckServiceColumns();
			DataProvider.Nodes.Clear();
			DataProvider.LockRecursiveNodesUpdate();
			BeginLoad();
			try {
				DataProvider.LockRecursiveNodesUpdate();
				LoadDataCore();
			} finally {
				DataProvider.UnlockRecursiveNodesUpdate();
				EndLoad();
			}
		}
		protected override void EndLoad() {
			base.EndLoad();
			if(!IsLoading && (View.ExpandStateBinding != null || !string.IsNullOrEmpty(View.ExpandStateFieldName)))
				DataProvider.UpdateNodesExpandState(DataProvider.Nodes, false);
		}
		protected void CheckServiceColumns() {
			IColumnCollection columns = View.ColumnsCore;
			if((columns[View.KeyFieldName] != null && columns[View.KeyFieldName].UnboundType != DevExpress.Data.UnboundColumnType.Bound) ||
				(columns[View.ParentFieldName] != null && columns[View.ParentFieldName].UnboundType != DevExpress.Data.UnboundColumnType.Bound))
				throw new ArgumentException("A service column cannot be represented by an unbound column.");
		}
		protected virtual void LoadDataCore() {
			Dictionary<object, TreeListNode> tempMap = GetNodeDictionaryFromListSource();
			if(Columns[View.ParentFieldName] == null
				|| Columns[View.KeyFieldName] == null
				|| Columns[View.ParentFieldName] == Columns[View.KeyFieldName]) {
				LoadLinearData(tempMap);
				return;
			}
			string parentFieldName = View.ParentFieldName;
			foreach(object key in tempMap.Keys) {
				TreeListNode node = tempMap[key];
				object parentKey = null;
				if(!String.IsNullOrEmpty(parentFieldName))
					parentKey = GetValue(node, parentFieldName);
				if(parentKey != null && tempMap.ContainsKey(parentKey)) {
					if(View.RootValue != null && Object.Equals(parentKey, View.RootValue))
						DataProvider.Nodes.Add(node);
					else {
						if(tempMap[parentKey] == node)
							DataProvider.Nodes.Add(node);
						else
							tempMap[parentKey].Nodes.Add(node);
					}
				} else {
					if(View.RootValue == null || Object.Equals(parentKey, View.RootValue))
						DataProvider.Nodes.Add(node);
				}
			}
		}
		Dictionary<object, TreeListNode> GetNodeDictionaryFromListSource() {
			Dictionary<object, TreeListNode> tempMap = new Dictionary<object, TreeListNode>();
			int autoId = 0;
			bool usePk = false;
			string keyFieldName = View.KeyFieldName;
			if(!View.IsDesignTime && ListSource != null && ListSource.Count > 0)
				usePk = Columns[keyFieldName] != null;
			for(int i = 0; i < ListSource.Count; i++) {
				object sourceItem = ListSource[i];
				object key = null;
				if(usePk) {
					key = GetValue(sourceItem, keyFieldName);
					if(key == null)
						throw new Exception("Missing primary key.");
					if(tempMap.ContainsKey(key))
						throw new Exception("Duplicated primary key.");
				} else
					key = autoId++;
				TreeListNode node = DataProvider.CreateNode(null);
				node.Id = i;
				node.DataProvider = this.DataProvider;
				node.Content = sourceItem;
				node.InitIsChecked();
				tempMap[key] = node;
			}
			return tempMap;
		}
		void LoadLinearData(Dictionary<object, TreeListNode> tempMap) {
			foreach(TreeListNode node in tempMap.Values) {
				DataProvider.Nodes.Add(node);
			}
		}
	}
}
