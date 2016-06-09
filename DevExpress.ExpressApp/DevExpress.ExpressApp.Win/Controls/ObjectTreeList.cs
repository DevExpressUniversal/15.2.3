#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
namespace DevExpress.ExpressApp.Win.Controls {
	public class ObjectTreeListNode : TreeListNode {
		private object objectBehind;
		private bool isPopulated;
		public ObjectTreeListNode(int nodeID, TreeListNodes owner) : base(nodeID, owner) { }
		public object Object {
			get { return objectBehind; }
			set { objectBehind = value; }
		}
		public bool IsChildrenPopulated {
			get { return isPopulated; }
			set { isPopulated = value; }
		}
	}
	[ToolboxItem(false)]
	public class ObjectTreeList : TreeList {
		protected enum SearchTreeListNodeMode { ThroughCreatedNodesOnly, ThroughAllPossibleNodes }
		protected class FindNodeByObjectOperation : TreeListOperation {
			protected object tagValue;
			protected TreeListNode node;
			public FindNodeByObjectOperation(object tagValue) {
				this.tagValue = tagValue;
			}
			public override bool CanContinueIteration(TreeListNode node) {
				return (this.Node == null);
			}
			public override void Execute(TreeListNode node) {
				if(((ObjectTreeListNode)node).Object == tagValue) {
					this.node = node;
				}
			}
			public TreeListNode Node {
				get { return node; }
			}
		}
		class UpdateNodesValuesOperation : TreeListOperation {
			private Action<ObjectTreeListNode, TreeListColumn> setNodeValueDelegate;
			private TreeListColumn column;
			public UpdateNodesValuesOperation(Action<ObjectTreeListNode, TreeListColumn> setNodeValueDelegate, TreeListColumn column)
				: base() {
				this.setNodeValueDelegate = setNodeValueDelegate;
				this.column = column;
			}
			public override void Execute(TreeListNode node) {
				ObjectTreeListNode objectTreeListNode = node as ObjectTreeListNode;
				if(objectTreeListNode != null && objectTreeListNode.Visible) {
					setNodeValueDelegate(objectTreeListNode, column);
				}
			}
		}
		private static readonly object nodesReloading;
		private Locker loadFromDataSourceLock = new Locker();
		private ListChangedEventHandler listChangedEventHandler;
		private Dictionary<IBindingList, object> listeningBindingLists = new Dictionary<IBindingList, object>();
		protected NodeObjectAdapter nodeObjectAdapter;
		private bool buildChildNodesRecursive = true;
		private bool dataSourceChanging = false;
		private IBindingList processingList = null;
		private List<string> imageNames = null;
		private FocusedNodeChangedEventArgs focusedNodeChangedPendingCall;
		private int focusedNodeChangedLocker = 0;
		private int saveViewStateCount = 0;
		private void AddListChangedHandler(object parent, IBindingList list) {
			list.ListChanged += listChangedEventHandler;
			if(!listeningBindingLists.ContainsKey(list)) {
				listeningBindingLists.Add(list, parent);
			}
		}
		private void RemoveListChangedHandler(IBindingList list) {
			list.ListChanged -= listChangedEventHandler;
			listeningBindingLists.Remove(list);
		}
		private void DestroyNodes() {
			UnsubscribeFromBindingLists();
			Nodes.Clear();
		}
		private void UnsubscribeFromBindingLists() {
			foreach(IBindingList list in listeningBindingLists.Keys) {
				list.ListChanged -= new ListChangedEventHandler(bindingDataSource_ListChanged);
			}
			listeningBindingLists.Clear();
		}
		private void LoadFromDataSource() {
			focusedNodeChangedLocker++;
			if(!loadFromDataSourceLock.Locked) {
				RaiseNodesReloading();
				BeginUpdateNodes();
				try {
					DestroyNodes();
					if(DataSource != null) {
						if(DataSource is IEnumerable) {
							foreach(object item in (IEnumerable)DataSource) {
								if(nodeObjectAdapter.IsRoot(item)) {
									BuildControlNode(item, null);
								}
							}
						}
						else {
							BuildControlNode(DataSource, null);
						}
					}
					if(DataSource is IBindingList) {
						AddListChangedHandler(null, (IBindingList)DataSource);
					}
					if(Nodes.Count == 1) {
						Nodes[0].Expanded = true;
					}
				}
				finally {
					EndUpdateNodes();
				}
				RaiseNodesReloaded();
			}
			focusedNodeChangedLocker--;
		}
		private object FindListOwner(IBindingList list) {
			return (list != null) ? listeningBindingLists[list] : null;
		}
		public ObjectTreeListNode FindBuiltAncestorNode(object obj) {
			ObjectTreeListNode result = null;
			object parentNode = obj;
			while(parentNode != null) {
				result = FindControlNode(parentNode, SearchTreeListNodeMode.ThroughCreatedNodesOnly);
				if(result != null) {
					break;
				}
				parentNode = nodeObjectAdapter.GetParent(parentNode);
			}
			if(!BuildChildNodesRecursive && result != null && obj != result.Object && result.Nodes.Count == 0) {
				BuildChildNodes(result);
				if(result.Nodes.Count > 0) {
					result = FindBuiltAncestorNode(obj);
				}
			}
			return result;
		}
		private bool IsParentChanged(ObjectTreeListNode node) {
			object currentParentObj = nodeObjectAdapter.GetParent(node.Object);
			ObjectTreeListNode currentParentNode = (ObjectTreeListNode)node.ParentNode;
			if(currentParentNode == null) {
				return !nodeObjectAdapter.IsRoot(node.Object);
			}
			else {
				object previousParentObj = currentParentNode.Object;
				return currentParentObj != previousParentObj;
			}
		}
		private void bindingDataSource_ListChanged(object sender, ListChangedEventArgs e) {
			OnBindingListChanged((IBindingList)sender, e);
		}
		private ObjectTreeListNode FindAbsentObjectNode(TreeListNodes nodes, IList list) {
			foreach(ObjectTreeListNode node in nodes) {
				if(!list.Contains(node.Object)) {
					return node;
				}
			}
			return null;
		}
		private ObjectTreeListNode FindAbsentObjectNode(TreeListNodes nodes, IList list, bool recursive) {
			ObjectTreeListNode result = FindAbsentObjectNode(nodes, list);
			if(result == null && recursive) {
				foreach(ObjectTreeListNode node in nodes) {
					if(node.HasChildren && node.IsChildrenPopulated) {
						result = FindAbsentObjectNode(node.Nodes, list, true);
						if(result != null) {
							break;
						}
					}
				}
			}
			return result;
		}
		private ObjectTreeListNode FindControlNodeThroughAllPossibleNodes(object nodeObject) {
			ObjectTreeListNode parent = null;
			if(!HasCircularReference(nodeObject)) {
				parent = FindControlNode(nodeObjectAdapter.GetParent(nodeObject));
			}
			TreeListNodes children = Nodes;
			if(parent != null) {
				BuildChildNodes(parent);
				children = parent.Nodes;
			}
			foreach(ObjectTreeListNode childNode in children) {
				object childNodeObject = nodeObjectAdapter.ObjectSpace != null ? nodeObjectAdapter.ObjectSpace.GetObject(childNode.Object) : childNode.Object;
				if(childNodeObject == nodeObject) {
					return childNode;
				}
			}
			return null;
		}
#if DebugTest
		public bool HasCircularReference(object nodeObject) {
#else
		private bool HasCircularReference(object nodeObject) {
#endif
			HashSet<object> hash = new HashSet<object>();
			hash.Add(nodeObject);
			bool result = false;
			object currentObj = nodeObjectAdapter.GetParent(nodeObject);
			while(currentObj != null) {
				if(hash.Contains(currentObj)) {
					result = true;
					break;
				}
				hash.Add(currentObj);
				currentObj = nodeObjectAdapter.GetParent(currentObj);
			}
			return result;
		}
		private void RemoveControlNode(TreeListNodes nodes, ObjectTreeListNode node) {
			if(node != null) {
				if(node.HasChildren) {
					ObjectTreeListNode[] deletedNodes = new ObjectTreeListNode[node.Nodes.Count];
					node.Nodes.CopyTo(deletedNodes, 0);
					foreach(ObjectTreeListNode deletedNode in deletedNodes) {
						RemoveControlNode(node.Nodes, deletedNode);
					}
				}
				foreach(KeyValuePair<IBindingList, object> pair in listeningBindingLists) {
					if(pair.Value == node.Object) {
						RemoveListChangedHandler(pair.Key);
						break;
					}
				}
				nodes.Remove(node);
			}
		}
		private void UpdateControlNodeImage(ObjectTreeListNode controlNode) {
			string imageKey = "";
			Image nodeImage = nodeObjectAdapter.GetImage(controlNode.Object, out imageKey);
			if(nodeImage != null) {
				if(this.StateImageList == null) {
					this.StateImageList = new ImageCollection();
					this.imageNames = new List<string>();
				}
				if(string.IsNullOrEmpty(imageKey)) {
					imageKey = controlNode.Object.GetHashCode().ToString();
				}
				if(this.imageNames.IndexOf(imageKey) == -1) {
					this.imageNames.Add(imageKey);
					this.StateImageList.AddImage(nodeImage);
				}
				controlNode.ImageIndex = this.imageNames.IndexOf(imageKey);
				controlNode.StateImageIndex = controlNode.ImageIndex;
			}
		}
		protected override TreeListNode CreateNode(int nodeID, TreeListNodes owner, object tag) {
			ObjectTreeListNode result = new ObjectTreeListNode(nodeID, owner);
			result.Object = tag;
			return result;
		}
		protected virtual ObjectTreeListNode BuildControlNode(object nodeObject, ObjectTreeListNode parentNode) {
			ObjectTreeListNode controlNode = (ObjectTreeListNode)AppendNode(null, parentNode, nodeObject);
			controlNode.HasChildren = nodeObjectAdapter.HasChildren(nodeObject);
			IBindingList childrenBindingList = nodeObjectAdapter.GetChildren(nodeObject) as IBindingList;
			if(childrenBindingList != null) {
				AddListChangedHandler(controlNode.Object, childrenBindingList);
			}
			OnControlNodeBuilt(controlNode);
			return controlNode;
		}
		public ObjectTreeListNode FindControlNode(object nodeObject) {
			return FindControlNode(nodeObject, SearchTreeListNodeMode.ThroughAllPossibleNodes);
		}
		protected virtual ObjectTreeListNode FindControlNode(object nodeObject, SearchTreeListNodeMode mode) {
			if(nodeObjectAdapter != null && nodeObject != null) {
				if(mode == SearchTreeListNodeMode.ThroughAllPossibleNodes) {
					return FindControlNodeThroughAllPossibleNodes(nodeObject);
				}
				else {
					FindNodeByObjectOperation findOperation = new FindNodeByObjectOperation(nodeObject);
					NodesIterator.DoOperation(findOperation);
					return (ObjectTreeListNode)findOperation.Node;
				}
			}
			return null;
		}
		protected ObjectTreeListNode FindControlNodeInChildren(TreeListNode node, object nodeObject, bool recursive) {
			ObjectTreeListNode result = null;
			foreach(ObjectTreeListNode childNode in node.Nodes) {
				if(childNode.Object == nodeObject) {
					result = childNode;
					break;
				}
				if(recursive) {
					result = FindControlNodeInChildren(childNode, nodeObject, true);
					if(result != null)
						break;
				}
			}
			return result;
		}
		protected virtual void OnControlNodeBuilt(ObjectTreeListNode controlNode) {
			BeginUpdate();
			foreach(TreeListColumn column in Columns) {
				SetNodeValue(controlNode, column);
			}
			UpdateControlNodeImage(controlNode);
			EndUpdate();
		}
#if DebugTest
		protected virtual void SetNodeValue(ObjectTreeListNode controlNode, TreeListColumn column) {
#else
		private void SetNodeValue(ObjectTreeListNode controlNode, TreeListColumn column) {
#endif
			if(column.FieldName == nodeObjectAdapter.DisplayPropertyName) {
				controlNode.SetValue(column, nodeObjectAdapter.GetDisplayPropertyValue(controlNode.Object));
			}
			else {
				ITypeInfo controlNodeType = XafTypesInfo.Instance.FindTypeInfo(controlNode.Object.GetType());
				IMemberInfo member = controlNodeType.FindMember(column.FieldName);
				object dataObject = controlNode.Object;
				if(member != null && member.GetOwnerInstance(dataObject) != null) {
					object value = member.GetValue(dataObject);
					if(value == null && member.MemberTypeInfo.IsValueType) {
						value = member.MemberTypeInfo.CreateInstance();
					}
					controlNode.SetValue(column, value);
				}
			}
		}
		protected virtual void OnChildNodesBuilt(ObjectTreeListNode controlNode) { }
		protected virtual void OnBindingListItemChanged(IBindingList bindingList, ListChangedEventArgs e) {
			object obj = bindingList[e.NewIndex];
			ObjectTreeListNode node = FindControlNode(obj, SearchTreeListNodeMode.ThroughCreatedNodesOnly);
			if(node != null) { 
				if(IsParentChanged(node)) {
					if(node.ParentNode == null) {
						RemoveControlNode(Nodes, node);
					}
					else {
						RemoveControlNode(((ObjectTreeListNode)node.ParentNode).Nodes, node);
						if(nodeObjectAdapter.GetParent(obj) == nodeObjectAdapter.RootValue) {
							BuildControlNode(obj, null);
						}
					}
				}
				else {
					RefreshObject(obj);
				}
			}
		}
		protected virtual void OnBindingListReset(IBindingList bindingList, ListChangedEventArgs e) {
			if(bindingList == DataSource) {
				focusedNodeChangedPendingCall = null;
				LoadFromDataSource();
				ExecuteFocusedNodeChangedPendingCall();
			}
			else if(listeningBindingLists.ContainsKey(bindingList)) {
				if(processingList == bindingList) {
					throw new InvalidOperationException("Recursive ListChanged.Reset processing");
				}
				bindingList.ListChanged -= new ListChangedEventHandler(bindingDataSource_ListChanged);
				try {
					processingList = bindingList;
					object parent = listeningBindingLists[bindingList];
					ObjectTreeListNode treeNode = FindBuiltAncestorNode(parent);
					if(treeNode != null) {
						if(treeNode.IsChildrenPopulated && (treeNode.HasChildren || nodeObjectAdapter.HasChildren(parent))) {
							ObjectTreeListNode[] deletingNodes = new ObjectTreeListNode[treeNode.Nodes.Count];
							treeNode.Nodes.CopyTo(deletingNodes, 0);
							foreach(ObjectTreeListNode childNode in deletingNodes) {
								RemoveControlNode(treeNode.Nodes, childNode);
							}
							treeNode.IsChildrenPopulated = false;
							BuildChildNodes(treeNode);
						}
						treeNode.HasChildren = nodeObjectAdapter.HasChildren(parent);
					}
				}
				finally {
					processingList = null;
					bindingList.ListChanged += new ListChangedEventHandler(bindingDataSource_ListChanged);
				}
			}
		}
		protected virtual void OnBindingListItemAdded(IBindingList bindingList, ListChangedEventArgs e) {
			try {
				loadFromDataSourceLock.Lock();
				object newObject = bindingList[e.NewIndex];
				object parent = nodeObjectAdapter.GetParent(newObject);
				RefreshObject(parent);
				if(parent == nodeObjectAdapter.RootValue) {
					BuildControlNode(newObject, null);
				}
				else {
					ObjectTreeListNode ancestorNode = FindBuiltAncestorNode(parent);
					if(ancestorNode == null)
						BuildControlNode(newObject, null);
					else {
						if(ancestorNode.HasChildren) {
							bool isAdded = FindControlNode(newObject, SearchTreeListNodeMode.ThroughCreatedNodesOnly) != null;
							if(!isAdded && ancestorNode.Object == parent && ancestorNode.Nodes.Count > 0) {
								BuildControlNode(newObject, ancestorNode);
							}
						}
						else {
							ancestorNode.IsChildrenPopulated = false;
							ancestorNode.HasChildren = true;
						}
					}
				}
			}
			finally {
				loadFromDataSourceLock.Unlock();
			}
		}
		protected virtual void OnBindingListItemDeleted(IBindingList bindingList, ListChangedEventArgs e) {
			ObjectTreeListNode parentNode = null;
			ObjectTreeListNode deletedObjectNode = null;
			if(bindingList == DataSource) {
				deletedObjectNode = FindAbsentObjectNode(Nodes, DataSource as IList, true);
			}
			else {
				object listOwner = FindListOwner(bindingList);
				parentNode = FindControlNode(listOwner, SearchTreeListNodeMode.ThroughCreatedNodesOnly);
				if(parentNode != null) {
					deletedObjectNode = FindAbsentObjectNode(parentNode.Nodes, bindingList);
				}
			}
			if(parentNode != null) {
				if(!nodeObjectAdapter.HasChildren(parentNode.Object)) {
					parentNode.Expanded = false;
					parentNode.HasChildren = false;
					parentNode.IsChildrenPopulated = false;
				}
				RemoveControlNode(parentNode.Nodes, deletedObjectNode);
				OnControlNodeBuilt(parentNode);
			}
			else {
				RemoveControlNode(Nodes, deletedObjectNode);
			}
		}
		protected virtual void OnBindingListChanged(IBindingList bindingList, ListChangedEventArgs e) {
			BeginUpdateNodes();
			try {
				switch(e.ListChangedType) {
					case ListChangedType.ItemChanged:
						OnBindingListItemChanged(bindingList, e);
						break;
					case ListChangedType.Reset:
						OnBindingListReset(bindingList, e);
						break;
					case ListChangedType.ItemAdded:
						OnBindingListItemAdded(bindingList, e);
						break;
					case ListChangedType.ItemDeleted:
						OnBindingListItemDeleted(bindingList, e);
						break;
				}
			}
			finally {
				EndUpdateNodes();
			}
		}
		protected virtual void BeginUpdateNodes() {
			if(saveViewStateCount == 0) {
				HandledEventArgs args = new HandledEventArgs();
				if(SaveViewState != null) {
					SaveViewState(this, args);
				}
				if(!args.Handled) {
					SaveViewStateCore();
				}
				BeginUnboundLoad();
			}
			saveViewStateCount++;
		}
		protected virtual void EndUpdateNodes() {
			saveViewStateCount--;
			if(saveViewStateCount == 0) {
				HandledEventArgs args = new HandledEventArgs();
				if(LoadViewState != null) {
					LoadViewState(this, args);
				}
				if(!args.Handled) {
					LoadViewStateCore();
				}
				EndUnboundLoad();
			}
		}
		protected virtual void SaveViewStateCore() {
		}
		protected virtual void LoadViewStateCore() {
		}
		protected virtual void RaiseNodesReloading() {
			EventHandler handler = (EventHandler)base.Events[nodesReloading];
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		protected override void RaiseBeforeExpand(TreeListNode node, ref bool canExpand) {
			base.RaiseBeforeExpand(node, ref canExpand);
			BeginUpdate();
			int topVisibleNodeIndex = TopVisibleNodeIndex;
			try {
				BuildChildNodes((ObjectTreeListNode)node);
			}
			finally {
				TopVisibleNodeIndex = topVisibleNodeIndex;
				EndUpdate();
			}
		}
		protected override void UpdateDataSource(bool updateContent) {
			if(dataSourceChanging) {
				object focusedObject = FocusedObject;
				if(Columns.Count == 0) {
					TreeListColumn column = Columns.Add();
					column.FieldName = nodeObjectAdapter.DisplayPropertyName;
					column.Caption = column.FieldName;
					column.MinWidth = 5;
					column.VisibleIndex = 0;
					column.SortOrder = SortOrder.Ascending;
					OptionsView.ShowColumns = false;
					ColumnCreated(column);
				}
				focusedNodeChangedPendingCall = null;
				LoadFromDataSource();
				if(updateContent && focusedObject != null && IsInitialized) {
					FocusedObject = focusedObject;
				}
				else {
					ExecuteFocusedNodeChangedPendingCall();
				}
			}
		}
		private void ExecuteFocusedNodeChangedPendingCall() {
			if(focusedNodeChangedLocker == 0 && focusedNodeChangedPendingCall != null) {
				RaiseFocusedNodeChanged(focusedNodeChangedPendingCall.OldNode,
					focusedNodeChangedPendingCall.Node);
				focusedNodeChangedPendingCall = null;
			}
		}
		protected override void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			if(focusedNodeChangedLocker == 0) {
				base.RaiseFocusedNodeChanged(oldNode, newNode);
			}
			else {
				this.focusedNodeChangedPendingCall = new FocusedNodeChangedEventArgs(oldNode, newNode);
			}
		}
		public void RaiseFocusedNodeChanged(TreeListNode newNode) {
			base.RaiseFocusedNodeChanged(null, newNode);
		}
		protected virtual void ColumnCreated(TreeListColumn column) { }
		protected override TreeListCustomizationForm CreateCustomizationForm() {
			return new XafTreeListCustomizationForm(this, this.Handler);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeFromBindingLists();
				nodeObjectAdapter = null;
			}
			base.Dispose(disposing);
		}
		public override void RefreshDataSource() { }
		public virtual void RefreshObject(object obj) {
			ObjectTreeListNode node = FindControlNode(obj, SearchTreeListNodeMode.ThroughCreatedNodesOnly);
			if(node != null) {
				OnControlNodeBuilt(node);
			}
		}
		static ObjectTreeList() {
			nodesReloading = new object();
		}
		public ObjectTreeList(NodeObjectAdapter nodeObjectAdapter) {
			MinimumSize = new Size(100, 75);
			this.listChangedEventHandler = new ListChangedEventHandler(bindingDataSource_ListChanged);
			if(nodeObjectAdapter == null) {
				throw new ArgumentNullException("nodeObjectAdapter");
			}
			this.nodeObjectAdapter = nodeObjectAdapter;
			this.OptionsNavigation.AutoFocusNewNode = false;
		}
		protected override void Columns_Changed(object sender, CollectionChangeEventArgs e) {
			base.Columns_Changed(sender, e);
			if(e.Action == CollectionChangeAction.Add) {
				BeginUpdate();
				UpdateNodesValues((TreeListColumn)e.Element);
				EndUpdate();
			}
		}
#if DebugTest
		protected virtual void UpdateNodesValues(TreeListColumn column) {
#else
		private void UpdateNodesValues(TreeListColumn column) {
#endif
			UpdateNodesValuesOperation operation = new UpdateNodesValuesOperation(SetNodeValue, column);
			NodesIterator.DoLocalOperation(operation, Nodes);
		}
		public void ReBuildChildNodes(ObjectTreeListNode node) {
			node.IsChildrenPopulated = false;
			node.Nodes.Clear();
			BuildChildNodes(node);
		}
		public virtual void BuildChildNodes(ObjectTreeListNode node) { 
			if(nodeObjectAdapter.HasChildren(node.Object) && !node.IsChildrenPopulated) {
				BeginUnboundLoad();
				try {
					IEnumerable children = nodeObjectAdapter.GetChildren(node.Object);
					foreach(object childNodeObject in children) {
						if(buildChildNodesRecursive) {
							BuildChildNodes(BuildControlNode(childNodeObject, node));
						}
						else {
							BuildControlNode(childNodeObject, node);
						}
					}
					OnChildNodesBuilt(node);
				}
				finally {
					EndUnboundLoad();
				}
				node.IsChildrenPopulated = true;
			}
		}
		public override int SetFocusedNode(TreeListNode node) {
			int newFocusedRowIndex = base.SetFocusedNode(node);
			if(OptionsSelection.MultiSelect && newFocusedRowIndex == FocusedRowIndex) {
				if(node != null && !Selection.Contains(node)) {
					Selection.Set(node);
				}
			}
			return newFocusedRowIndex;
		}
		public IList GetSelectedObjects() {
			ArrayList list = new ArrayList();
			if(OptionsSelection.MultiSelect) {
				foreach(ObjectTreeListNode node in Selection) {
					list.Add(node.Object);
				}
			}
			else {
				if(FocusedObject != null) {
					list.Add(FocusedObject);
				}
			}
			return list.ToArray();
		}
		public new ObjectTreeListNode FocusedNode {
			get { return base.FocusedNode as ObjectTreeListNode; }
			set { base.FocusedNode = value; }
		}
		public object FocusedObject {
			get {
				if(FocusedNode != null) {
					return FocusedNode.Object;
				}
				else {
					return null;
				}
			}
			set {
				if(value != DBNull.Value) {
					FocusedNode = FindControlNode(value);
				}
			}
		}
		public bool BuildChildNodesRecursive {
			get { return buildChildNodesRecursive; }
			set { buildChildNodesRecursive = value; }
		}
		public new object DataSource {
			get { return base.DataSource; }
			set {
				dataSourceChanging = true;
				base.DataSource = value;
				dataSourceChanging = false;
			}
		}
		public new ImageCollection StateImageList {
			get { return (ImageCollection)base.StateImageList; }
			set { base.StateImageList = value; }
		}
		public event EventHandler NodesReloading {
			add {
				Events.AddHandler(nodesReloading, value);
			}
			remove {
				Events.RemoveHandler(nodesReloading, value);
			}
		}
		public event EventHandler<HandledEventArgs> SaveViewState;
		public event EventHandler<HandledEventArgs> LoadViewState;
#if DebugTest
		public void DebugTest_UpdateNodesValues(TreeListColumn column) {
			UpdateNodesValues(column);
		}
#endif
	}
	public class XafTreeListCustomizationListBox : TreeListCustomizationListBox {
		public XafTreeListCustomizationListBox(XafTreeListCustomizationForm form) : base(form) { }
		protected override void DrawItemObject(DevExpress.Utils.Drawing.GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			base.DrawItemObject(cache, index, bounds, itemState);
			if(SelectedIndex > -1 && index == SelectedIndex) {
				Color foreColor = ForeColor;
				SolidBrush solidBrush = new SolidBrush(Color.FromArgb(25, foreColor.R, foreColor.G, foreColor.B));
				Rectangle rect = bounds;
				cache.FillRectangle(solidBrush, rect);
			}
		}
	}
	public class XafTreeListCustomizationForm : TreeListCustomizationForm {
		public XafTreeListCustomizationForm(TreeList treeList, TreeListHandler handler) : base(treeList, handler) { }
		protected override DevExpress.XtraEditors.Customization.CustomizationListBoxBase CreateCustomizationListBox() {
			return new XafTreeListCustomizationListBox(this);
		}
	}
}
