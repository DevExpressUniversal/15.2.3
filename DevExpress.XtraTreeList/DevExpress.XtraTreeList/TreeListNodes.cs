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

using System.ComponentModel;
using System.Collections;
namespace DevExpress.XtraTreeList.Nodes {
	using DevExpress.XtraTreeList;
	using DevExpress.XtraTreeList.Nodes.Operations;
	using System;
using System.Windows.Forms;
using System.Collections.Generic;
	using DevExpress.Utils;
	using DevExpress.XtraTreeList.Internal;
	public class TreeListNode : System.ICloneable {
		TreeListNodes nodes;
		bool hasChildren;
		bool expanded;
		object tag;
		int imageIndex, selectImageIndex, stateImageIndex;
		int id;
		bool visible;
		protected internal TreeListNodes owner;
		internal bool _visible;
		internal bool isExpandedSetInternally;
		CheckState checkState;
		protected internal TreeListNode(int id, TreeListNodes owner) {
			this.id = id;
			this.owner = owner;
			this.nodes = CreateNodes(owner.TreeList);
			this.hasChildren = false;
			this.expanded = false;
			this.tag = null;
			this.imageIndex = this.selectImageIndex = 0;
			this.stateImageIndex = -1;
			this.visible = this._visible = true;
			this.checkState = CheckState.Unchecked;
		}
		protected virtual TreeListNodes CreateNodes(TreeList treeList){
			return new TreeListNodes(null, this);
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeId")]
#endif
		public int Id { get { return id; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeHasChildren")]
#endif
		public virtual bool HasChildren {
			get { return hasChildren; }
			set { 
				if(HasChildren != value) {
					hasChildren = value;
					Changed(NodeChangeTypeEnum.HasChildren);
				}
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeLevel")]
#endif
		public int Level {
			get {
				int level = 0;
				TreeListNodes nodes = owner;
				while(nodes.ParentNode != null) {
					level++;
					nodes = nodes.ParentNode.owner;
				}
				return level;
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeExpanded")]
#endif
		public virtual bool Expanded {
			get { return expanded; }
			set { 
				if(Expanded == value) return;
				if(value && !HasChildren) return; 
				OnChangeExpanded(value);
			}
		}
		bool changingExpanded;
		void OnChangeExpanded(bool value) {
			if(changingExpanded) return;
			try {
				changingExpanded = true;
				OnChangeExpandedCore(value);
			}
			finally {
				changingExpanded = false;
			}
		}
		protected virtual void OnChangeExpandedCore(bool value) {
			bool canChange = TreeList.OnBeforeChangeExpanded(this, value);
			if(canChange) {
				int index = TreeList.GetNodeVisibleIndexOnExpand(this);
				Action0 expandAction = delegate {
					expanded = value;
					if(TreeList.IsVirtualMode && expanded)
						TreeList.OnVirtualTreeListNodeExpand(this);
					TreeList.OnAfterChangeExpanded(this, index);
				};
				if(value && !TreeList.IsLockExpandCollapse)
					TreeList.ExecuteExpandAction(expandAction, this);
				else
					expandAction();
				Changed(NodeChangeTypeEnum.Expanded);
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeTag")]
#endif
		public virtual object Tag {
			get { return tag; }
			set {
				if(Tag == null) {
					if(value != null) {
						tag = value;
						Changed(NodeChangeTypeEnum.Tag);
					}
				}
				else if(!Tag.Equals(value)) {
					tag = value; 
					Changed(NodeChangeTypeEnum.Tag);
				}
			}
		}
		internal void SetTagInternal(object tag) {
			this.tag = tag;
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeVisible")]
#endif
		public virtual bool Visible {
			get {
				if(visible && !_visible) return false;
				return visible;
			}
			set {
				if(value == visible) {
					UpdateVisibilityByFilterMode();
					return;
				};
				visible = value;
				UpdateVisibilityByFilterMode();
				if(!Visible && TreeList.Selection.Contains(this)) {
					TreeList.Selection.InternalRemove(this);
					TreeList.RaiseSelectionChanged();
				}
				TreeList.RecalcRowCount();
				TreeList.NullTopVisibleNode();
				if(TreeList.FocusedNode == this || TreeList.AutoFocusedNode == this) {
					TreeList.SetFocusedRowIndexCore(-1);
					TreeList.SetAutoFocusedNodeInternal(TreeListFilterHelper.GetNewFocusedNode(this));
				}
				TreeList.LayoutChanged();
			}
		}
		void UpdateVisibilityByFilterMode() {
			FilterMode filterMode = TreeList.GetActualTreeListFilterMode();
			if(filterMode == FilterMode.Standard) {
				_visible = (ParentNode != null && !ParentNode.Visible) ? false : visible;
				TreeListFilterHelper.UpdateChildNodesVisibility(this);
			}
			if(filterMode == FilterMode.Extended) {
				if(Visible) {
					TreeListNode parent = ParentNode;
					while(parent != null && !parent.Visible) {
						parent.Visible = true;
						parent = parent.ParentNode;
					}
				}
			}
		}
		internal bool IsVisible { get { return visible; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), System.Obsolete("You should use the 'Tag' property instead of 'Data'")]
		public object Data { get { return Tag; } set { Tag = value; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeTreeList")]
#endif
		public TreeList TreeList { 
			get {
				return (owner == null ? null : owner.TreeList);
			} 
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeNodes")]
#endif
		public virtual TreeListNodes Nodes { get { return nodes; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeParentNode")]
#endif
		public TreeListNode ParentNode { get { return (owner == null ? null : owner.ParentNode); } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeImageIndex"),
#endif
 DefaultValue(0)]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex != value) {
					imageIndex = value;
					Changed(NodeChangeTypeEnum.ImageIndex);
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeSelectImageIndex"),
#endif
 DefaultValue(0)]
		public virtual int SelectImageIndex {
			get { return selectImageIndex; }
			set {
				if(SelectImageIndex != value) {
					selectImageIndex = value;
					Changed(NodeChangeTypeEnum.SelectImageIndex);
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeStateImageIndex"),
#endif
 DefaultValue(-1)]
		public virtual int StateImageIndex {
			get { return stateImageIndex; }
			set {
				if(StateImageIndex != value) {
					stateImageIndex = value;
					Changed(NodeChangeTypeEnum.StateImageIndex);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Checked { 
			get { return CheckState == CheckState.Checked; }
			set { CheckState = value ? CheckState.Checked : CheckState.Unchecked; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodeCheckState"),
#endif
 DefaultValue(CheckState.Unchecked)]
		public virtual CheckState CheckState {
			get { return checkState; }
			set {
				if(CheckState != value) {
					checkState = value;
					Changed(NodeChangeTypeEnum.CheckedState);
				}
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeRootNode")]
#endif
		public TreeListNode RootNode {
			get {
				TreeListNode parent = this;
				while(parent.ParentNode != null)
					parent = parent.ParentNode;
				return parent;
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeFirstNode")]
#endif
		public virtual TreeListNode FirstNode { get { return Nodes.FirstNode; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeLastNode")]
#endif
		public virtual TreeListNode LastNode { get { return Nodes.LastNode; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodePrevNode")]
#endif
		public virtual TreeListNode PrevNode { get { return owner.GetPrevNode(this); } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeNextNode")]
#endif
		public virtual TreeListNode NextNode { get { return owner.GetNextNode(this); } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodePrevVisibleNode")]
#endif
		public virtual TreeListNode PrevVisibleNode { get { return TreeListNodesIterator.GetPrevVisible(this); } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeNextVisibleNode")]
#endif
		public virtual TreeListNode NextVisibleNode { get { return TreeListNodesIterator.GetNextVisible(this); } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeFocused")]
#endif
		public bool Focused {
			get {
				if(TreeList == null) return false;
				return (this == TreeList.FocusedNode);
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeSelected")]
#endif
		public virtual bool Selected {
			get {
				if(TreeList == null) return false;
				return TreeList.Selection.Contains(this);
			}
			set {
				if(TreeList == null || Selected == value) return;
				if(TreeList.OptionsSelection.MultiSelect) {
					if(value)
						TreeList.Selection.Add(this);
					else
						TreeList.Selection.Remove(this);
				}
				else {
					TreeList.FocusedNode = this;
				}
			}
		}
		public bool HasAsParent(TreeListNode node) {
			if(node == null) return true;
			TreeListNode parent = this;
			while(parent.ParentNode != null) {
				parent = parent.ParentNode;
				if(parent == node) return true;
			}
			return false;
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodeItem")]
#endif
		public virtual object this[object columnID] {
			get { return GetValue(columnID); }
			set { SetValue(columnID, value); }
		}
		public virtual object GetValue(object columnID) {
			return TreeList.GetNodeValue(this, columnID);
		}
		public virtual void SetValue(object columnID, object val) {
			TreeList.OnSetValue(this, columnID, val);
		}
		public virtual string GetDisplayText(object columnID) {
			return TreeList.Data.GetDisplayText(Id, columnID, this);
		}
		public virtual object Clone() {
			TreeListNode node = TreeList.CreateNode(Id, owner, this.Tag);
			node.tag = this.Tag;
			node.expanded = this.Expanded;
			node.hasChildren = this.HasChildren;
			node.imageIndex = this.ImageIndex;
			node.selectImageIndex = this.SelectImageIndex;
			node.stateImageIndex = this.StateImageIndex;
			node.checkState = this.CheckState;
			TreeList.CloneInfoList.RegisterClone(Id);
			return node;
		}
		protected internal bool HasClones { get { return (TreeList.CloneInfoList.GetInfoByRowId(Id) != null); } }
		public virtual void ExpandAll() {
			if(TreeList == null) return;
			TreeList.InternalFullExpandNode(this);
		}
		public void CheckAll() {
			CheckAllCore(true);
		}
		public void UncheckAll() {
			CheckAllCore(false);
		}
		protected void CheckAllCore(bool check) {
			Checked = check;
			if(TreeList != null)
				TreeList.NodesIterator.DoLocalOperation(new TreeListOperationDelegate(delegate(TreeListNode node) { node.Checked = check; }), Nodes);
		}
		protected virtual void Changed(NodeChangeTypeEnum changeType) {
			if(TreeList != null)
				TreeList.InternalNodeChanged(this, changeType);
		}
		protected internal void SetId(int nodeID) { id = nodeID; }
		internal bool IsFirstVisible {
			get {
				if(owner == null)
					return true;
				return owner.FirstNodeEx == this;
			}
		}
		internal bool IsLastVisible {
			get {
				if(owner == null)
					return true;
				return owner.LastNodeEx == this;
			}
		}
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraTreeList.Design.Serializers.TreeListNodesCodeDomSerializer, " + AssemblyInfo.SRAssemblyTreeListDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class TreeListNodes : IEnumerable<TreeListNode>, IEnumerable, ICollection {
		ArrayList nodes;
		TreeList treeList;
		TreeListNode parentNode;
		IDictionary<TreeListNode, int> NodeIndices { get; set; }
		protected void ClearNodeIndices() {
			NodeIndices.Clear();
		}
		protected void ResetNodeIndices() {
			ClearNodeIndices();
			int i = 0;
			foreach(TreeListNode node in this) 
				NodeIndices[node] = i++;
		}
		public IEnumerator GetEnumerator() {
			return nodes.GetEnumerator();
		}
		IEnumerator<TreeListNode> IEnumerable<TreeListNode>.GetEnumerator() {
			foreach(TreeListNode node in nodes)
				yield return node;
		}
		static ArrayList EmptyList = new ArrayList(0);
		public TreeListNodes(TreeList treeList, TreeListNode parentNode) {
			nodes = EmptyList;
			NodeIndices = new Dictionary<TreeListNode, int>(); 
			this.treeList = treeList;
			this.parentNode = parentNode;
		}
		public TreeListNodes(TreeList treeList) : this(treeList, null) { }				
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesTreeList")]
#endif
		public TreeList TreeList {
			get {
				if(ParentNode != null)
					return parentNode.TreeList;
				return treeList; 
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListNodesParentNode"),
#endif
 DefaultValue(null)]
		public TreeListNode ParentNode { 
			get {
				return parentNode; 
			} 
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesFirstNode")]
#endif
		public TreeListNode FirstNode {
			get {
				if(nodes.Count == 0) return null;
				return this[0]; 
			}
		}
		internal TreeListNode FirstNodeEx {
			get {
				if(nodes.Count == 0) return null;
				return GetFirstVisible();
			}
		}
		internal TreeListNode LastNodeEx {
			get {
				if(nodes.Count == 0) return null;
				return GetLastVisible();
			}
		}
		TreeListNode GetFirstVisible() {
			for(int i = 0; i < this.Count; i++) {
				if(this[i].Visible) return this[i];
			}
			return null;
		}
		TreeListNode GetLastVisible() {
			for(int i = this.Count -1; i >= 0; i--) {
				if(this[i].Visible) return this[i];
			}
			return null;
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesLastNode")]
#endif
		public TreeListNode LastNode {
			get {
				if(nodes.Count == 0) return null;
				return this[nodes.Count - 1]; 
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Add(TreeListNode node) {
			if(node != null) {
				_add(node);
			}
		}
		public TreeListNode Add(object nodeData) {
			if(TreeList == null) return null;
			return TreeList.AppendNode(nodeData, ParentNode);
		}
		public TreeListNode Add(params object[] nodeData) {
			if(TreeList == null) return null;
			return TreeList.AppendNode(nodeData, ParentNode);
		}
		internal TreeListNode add(int nodeID, object tag) {
			TreeListNode node = TreeList.CreateNode(nodeID, this, tag);
			node.SetTagInternal(tag);
			_add(node);
			return node;
		}
		void CheckEmptyList() {
			if(nodes == EmptyList)
				nodes = new ArrayList(4); 
		}
		internal void _add(TreeListNode node) {
			CheckEmptyList();
			NodeIndices[node] = nodes.Add(node);
			Changed(node, NodeChangeTypeEnum.Add);
		}
		internal void remove(TreeListNode node) {
			int cachedIndex = -1;
			if(NodeIndices.TryGetValue(node, out cachedIndex) && cachedIndex > -1) 
				nodes.RemoveAt(cachedIndex);
			else 
				nodes.Remove(node);
			ClearNodeIndices();
			Changed(node, NodeChangeTypeEnum.Remove);
		}
		internal void removeAt(int index) {
			TreeListNode node = this[index];
			nodes.RemoveAt(index);
			ClearNodeIndices();
			Changed(node, NodeChangeTypeEnum.Remove);
		}
		internal void _insert(int index, TreeListNode node) {
			Insert(index, node);
			ClearNodeIndices();
			Changed(node, NodeChangeTypeEnum.Add);
		}
		internal void _clear() {
			ClearNodeIndices();
			nodes = EmptyList;
		}
		protected internal void Insert(int index, TreeListNode node) {
			CheckEmptyList();
			nodes.Insert(index, node);
			ClearNodeIndices();
		}
		public void Remove(TreeListNode node) {
			ClearNodeIndices();
			TreeList.DeleteNode(node);
		}
		public void RemoveAt(int index) {
			if((index < 0) || (index >= Count)) return;
			Remove(this[index]);
		}
		public int IndexOf(TreeListNode node) {
			if(node == null) return -1;
			int index;
			if(NodeIndices.TryGetValue(node, out index))
				return index;
			index = nodes.IndexOf(node);
			NodeIndices[node] = index;
			return index;
		}
		public void Clear() {
			if(nodes.Count == 0) return;
			ClearNodeIndices();
			TreeList.OnClearNodes(this);
			nodes = EmptyList;
			TreeList.OnClearNodesComplete(this);
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesCount")]
#endif
		public int Count {
			get { return nodes.Count; }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesIsSynchronized")]
#endif
		public bool IsSynchronized {
			get { return nodes.IsSynchronized; }
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesSyncRoot")]
#endif
		public object SyncRoot {
			get { return nodes.SyncRoot; }
		}
		public void CopyTo(System.Array array, int index) {
			nodes.CopyTo(array, index);
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesItem")]
#endif
		public TreeListNode this[int index] { get { return (TreeListNode)nodes[index]; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListNodesAutoFilterNode")]
#endif
		public TreeListAutoFilterNode AutoFilterNode { get { return TreeList.AutoFilterNode; } }
		internal TreeListNode GetPrevNode(TreeListNode child) {
			int index = IndexOf(child);
			return (index > 0 ? this[index - 1] : null);
		}
		internal TreeListNode GetNextNode(TreeListNode child) {
			int index = IndexOf(child);
			if(index == -1) return null;
			return (index < Count - 1 ? this[index + 1] : null);
		}
		protected internal virtual void SortNodes(IComparer comparer) {
			try {
				nodes.Sort(comparer);
			}
			finally {
				ResetNodeIndices();
			}
		}
		protected void Changed(TreeListNode node, NodeChangeTypeEnum changeType) {
			if(TreeList != null)
				TreeList.NodeCollectionChanged(this, node, changeType);
		}
		internal void DeleteSelectedNodes() {
			if(treeList.Selection.Count == 0) return;
			List<TreeListNode> selectedNodes = new List<TreeListNode>(treeList.Selection);
			List<TreeListNode> nodesToIgnore = new List<TreeListNode>(); 
			TreeListNode tempNode;
			for(int i = 0; i < treeList.Selection.Count; i++) {
				TreeListNode currentNode = treeList.Selection[i];
				for(int j = 0; j < treeList.Selection.Count; j++) {
					if(i == j) continue;
					tempNode = treeList.Selection[j];
					if(tempNode.HasAsParent(currentNode)) {
						selectedNodes.Remove(tempNode);
					}
				}
			}
			treeList.BeginUpdate();
			try {
				ClearNodeIndices();
				for(int i = 0; i < selectedNodes.Count; i++) {
					TreeListNode node = selectedNodes[i];
					treeList.DeleteNode(node);
				}
			}
			finally {
				treeList.EndUpdate();
			}
		}
	}
	public sealed class TreeListAutoFilterNode : TreeListNode {
		internal static bool IsAutoFilterNode(TreeListNode node) { return node is TreeListAutoFilterNode; }
		public TreeListAutoFilterNode(TreeList treeList) : base(TreeList.AutoFilterNodeId, new TreeListNodes(treeList)) { }
		protected override TreeListNodes CreateNodes(TreeList treeList) { return null; }
		#region hidden properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool HasChildren { get { return false; } set {  } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Checked { get { return false; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override CheckState CheckState { get { return CheckState.Unchecked; } set { base.CheckState = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Expanded { get { return false; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible { get { return true; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int ImageIndex { get { return -1; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Selected { get { return false; } set {  } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int SelectImageIndex { get { return -1; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int StateImageIndex { get { return -1; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode FirstNode { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode LastNode { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode PrevNode { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode NextNode { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode PrevVisibleNode { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override TreeListNode NextVisibleNode { get { return null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void ExpandAll() {  }
		#endregion
	}
}
