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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class NavigationTreeFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public NavigationTreeFrame() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DocumentManagerInfo.SelectionChanged -= OnInfo_SelectionChanged;
				if(View != null)
					View.HierarchyChanged -= new EventHandler(OnHierarchyChanged);
			}
			base.Dispose(disposing);
		}
		BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		TreeNode dropTargetNode;
		[Browsable(false)]
		public TreeNode DropTargetNode { get { return dropTargetNode; } set { dropTargetNode = value; } }
		EditingDocumentManagerInfo documentManagerInfo;
		protected EditingDocumentManagerInfo DocumentManagerInfo {
			get { return documentManagerInfo; }
		}
		public override void InitComponent() {
			base.InitComponent();
			this.documentManagerInfo = InfoObject as EditingDocumentManagerInfo;
			DocumentManagerInfo.SelectionChanged += OnInfo_SelectionChanged;
			if(View != null)
				View.HierarchyChanged += new EventHandler(OnHierarchyChanged);
		}
		void OnHierarchyChanged(object sender, EventArgs e) {
			Repopulate();
		}
		void OnInfo_SelectionChanged(object sender, EventArgs e) {
			RaiseRefreshWizard("", "ChangedView");
			PopulateTree();
			RefreshPropertyGrid();
		}
		public DXTreeView Tree { get { return navigateTree; } }
		public override void DoInitFrame() {
			dropTargetNode = null;
			PopulateTree();
			PopulateDocuments();
			PopulateTiles();
		}
		protected internal void PopulateTiles() {
			tilesTree.BeginUpdate();
			try {
				tilesTree.Nodes.Clear();
				foreach(BaseTile tile in View.Tiles) {
					AddTile(tilesTree.Nodes, tile);
				}
			}
			finally {
				tilesTree.EndUpdate();
			}
		}
		protected virtual TreeNode AddTile(TreeNodeCollection nodes, BaseTile tile) {
			TileNode node = new TileNode(tile);
			nodes.Add(node);
			return node;
		}
		protected internal void PopulateDocuments() {
			documentsTree.BeginUpdate();
			try {
				documentsTree.Nodes.Clear();
				foreach(BaseDocument document in EditingView.Documents) {
					AddDocument(documentsTree.Nodes, document);
				}
			}
			finally {
				documentsTree.EndUpdate();
			}
		}
		protected virtual TreeNode AddDocument(TreeNodeCollection nodes, BaseDocument document) {
			DocumentNode node = new DocumentNode(document);
			nodes.Add(node);
			return node;
		}
		IList<string> expandedItems = new List<string>();
		protected string GetNodeKey(TreeNode node) {
			int level = 0;
			TreeNode pnode = node;
			string fullPath = node.Text;
			while(pnode.Parent != null) {
				level++;
				pnode = pnode.Parent;
				fullPath += pnode.Text;
			}
			return string.Format("{0}:{1}{2}{3}", level, node.Tag == null ? null : node.Tag.GetType(), node.Tag == null ? 0 : node.Tag.GetHashCode(), fullPath);
		}
		protected virtual void ClearExpandedItems() {
			expandedItems.Clear();
		}
		protected virtual void SaveExpandedItems() {
			ClearExpandedItems();
			SaveExpandedNodes(Tree.Nodes);
		}
		protected virtual void RestoreExpandedItems() {
			if(expandedItems.Count > 0)
				RestoreExpandedNodes(Tree.Nodes);
		}
		IList<string> savedSelectedNodeKeys;
		protected virtual void RestoreSelectedItem() {
			if(savedSelectedNodeKeys.Count != 0) {
				RestoreSelectedNode(Tree.Nodes, savedSelectedNodeKeys);
			}
		}
		protected virtual void SaveSelectedItem() {
			savedSelectedNodeKeys = new List<string>();
			if(Tree.SelectedNode != null)
				foreach(TreeNode node in navigateTree.SelNodes) {
					savedSelectedNodeKeys.Add(GetNodeKey(node));
				}
		}
		void RestoreSelectedNode(TreeNodeCollection nodes, IList<string> selectedNodeKeys) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				if(selectedNodeKeys.Contains(GetNodeKey(node))) {
					navigateTree.AddNodeToSelection(node);
				}
				RestoreSelectedNode(node.Nodes, selectedNodeKeys);
			}
		}
		void SaveExpandedNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				if(!node.IsExpanded) continue;
				this.expandedItems.Add(GetNodeKey(node));
				SaveExpandedNodes(node.Nodes);
			}
		}
		void RestoreExpandedNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				string key = GetNodeKey(node);
				if(expandedItems.Contains(key))
					node.Expand();
				RestoreExpandedNodes(node.Nodes);
			}
		}
		WindowsUIView View { get { return (WindowsUIView)EditingView; } }
		public virtual void PopulateTree() {
			Tree.BeginUpdate();
			try {
				Tree.Nodes.Clear();
				foreach(IContentContainer container in View.ContentContainers) {
					if(container.Parent == null)
						AddContentContainer(Tree.Nodes, container);
				}
			}
			finally { Tree.EndUpdate(); }
		}
		public virtual void Repopulate() {
			SaveExpandedItems();
			SaveSelectedItem();
			PopulateTree();
			RestoreExpandedItems();
			Tree.UpdateSelection();
			RestoreSelectedItem();
		}
		public virtual void RefreshTree() {
			Tree.BeginUpdate();
			try {
				RefreshNodes(Tree.Nodes);
			}
			finally { Tree.EndUpdate(); }
		}
		protected void RefreshNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				RefreshNodes(node.Nodes);
			}
		}
		protected object GetNodeObject(TreeNode node) {
			return node.Tag;
		}
		protected virtual bool IsAllowSelectObject(object val) {
			return true;
		}
		protected virtual void navigateTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			tree_AfterSelect(sender, e);
			btnClear.Enabled = false;
			btnDown.Enabled = btnUp.Enabled = false;
			if(e.Node is IOrderedItem) {
				bool canMove = ((IOrderedItem)e.Node).CanMove();
				btnDown.Enabled = btnUp.Enabled = canMove;
			}
			if(e.Node is IChildItem) {
				btnClear.Enabled = ((IChildItem)e.Node).CanResetParent();
			}
			if(e.Node is IActivationTarget) {
				btnClear.Enabled = ((IActivationTarget)e.Node).CanClearActivationTarget();
			}
		}
		protected virtual void tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(e.Node == null) return;
			if(IsAllowSelectObject(GetNodeObject(e.Node))) {
				pgMain.SelectedObject = GetNodeObject(e.Node);
			}
			else pgMain.SelectedObject = null;
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			RefreshTree();
		}
		protected virtual void tree_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			e.CancelEdit = e.Node is InfrastructureNode;
		}
		protected virtual TreeNode AddContentContainer(TreeNodeCollection nodes, IContentContainer container) {
			TreeNode node = Classifier.CreateNode(container);
			nodes.Add(node);
			return node;
		}
		void SetSelectedNode() {
			pgMain.SelectedObject = null;
			if(Tree.SelectedNode != null && Tree.Nodes.Count != 0) {
				int i = Tree.SelectedNode.Index;
				Tree.SelectedNode = Tree.Nodes[i];
			}
			Repopulate();
		}
		#region documentsTreeDragDrop
		object dragDocumentItem;
		void documentsTree_DragNodeStart(object sender, EventArgs e) {
			dragDocumentItem = documentsTree.SelNodes;
		}
		void documentsTree_DragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			DragNodeGetObjectCore(e, dragDocumentItem as TreeNode[]);
		}
		void documentsTree_DragEnter(object sender, DragEventArgs e) {
			SetDragDropEffect(e, typeof(DocumentNode));
		}
		void SetDragDropEffect(DragEventArgs e, Type type) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems == null) {
				e.Effect = DragDropEffects.None;
				return;
			}
			if(CanRemoveItems(dragItems, type))
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.None;
		}
		bool CanRemoveItems(IEnumerable<IDragItem> dragItems, Type type) {
			bool canRemove = dragItems.GetEnumerator().MoveNext();
			foreach(IDragItem item in dragItems) {
				canRemove &= item.GetType() == type;
			}
			return canRemove;
		}
		void documentsTree_DragDrop(object sender, DragEventArgs e) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems != null) {
				RemoveDocuments(dragItems);
				Repopulate();
			}
		}
		void RemoveDocuments(IEnumerable<IDragItem> dragItems) {
			foreach(IDragItem item in dragItems) {
				DocumentGroup documentGroup = ((DocumentNode)item).GetDocumentGroup();
				if(documentGroup != null) {
					documentGroup.Items.Remove(item.Content as Document);
				}
			}
		}
		#endregion
		#region tilesTreeDragDrop
		object dragTileItem;
		void tilesTree_DragNodeStart(object sender, EventArgs e) {
			dragTileItem = tilesTree.SelNodes;
		}
		void tilesTree_DragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			DragNodeGetObjectCore(e, dragTileItem as TreeNode[]);
		}
		protected void DragNodeGetObjectCore(TreeViewGetDragObjectEventArgs e, TreeNode[] nodes) {
			if(nodes == null) return;
			DragInfo info = new DragInfo(ConverToIDragItem(nodes));
			e.DragObject = info;
			e.AllowEffects = DragDropEffects.Copy | DragDropEffects.Move;
		}
		IEnumerable<IDragItem> ConverToIDragItem(TreeNode[] nodes) {
			foreach(TreeNode node in nodes) {
				if(node is IDragItem) {
					yield return node as IDragItem;
				}
			}
		}
		void tilesTree_DragEnter(object sender, DragEventArgs e) {
			SetDragDropEffect(e, typeof(TileNode));
		}
		void tilesTree_DragDrop(object sender, DragEventArgs e) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems != null) {
				RemoveTiles(dragItems);
				Repopulate();
			}
		}
		void RemoveTiles(IEnumerable<IDragItem> dragItems) {
			foreach(IDragItem item in dragItems) {
				TileContainer tileContainer = ((TileNode)item).GetTileContainer();
				if(tileContainer != null)
					tileContainer.Items.Remove(item.Content as BaseTile);
			}
		}
		#endregion
		#region navigationTreeDragDrop
		object dragNavigationItem;
		void navigateTree_DragNodeStart(object sender, EventArgs e) {
			dragNavigationItem = Tree.SelNodes;
		}
		void navigateTree_DragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			DragNodeGetObjectCore(e, dragNavigationItem as TreeNode[]);
		}
		protected virtual void navigationTreeDragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems == null) return;
			if(DropTargetNode != null && DropTargetNode is IDropTargetItem) {
				IDropTargetItem parentNode = DropTargetNode as IDropTargetItem;
				foreach(IDragItem item in dragItems) {
					if(parentNode.CanDrop(item))
						parentNode.Drop(item);
				}
			}
			Repopulate();
		}
		protected virtual void navigationTreeDragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems == null) return;
			TreeNode node = Tree.GetNodeAt(Tree.PointToClient(new Point(e.X, e.Y)));
			if(node != null) {
				DropTargetNode = node;
			}
			if(DropTargetNode is IDropTargetItem)
				e.Effect = CanDropToTargetNode((IDropTargetItem)DropTargetNode, dragItems) ? DragDropEffects.Copy : DragDropEffects.None;
			else
				e.Effect = DragDropEffects.None;
		}
		bool CanDropToTargetNode(IDropTargetItem targetNode, IEnumerable<IDragItem> dragItems) {
			if(dragItems == null) return false;
			bool canDrop = false;
			foreach(IDragItem item in dragItems) {
				canDrop |= targetNode.CanDrop(item);
			}
			return canDrop;
		}
		protected virtual void navigationTreeDragLeave(object sender, System.EventArgs e) {
			Repopulate();
		}
		protected virtual void navigationTreeDragEnter(object sender, DragEventArgs e) {
			SetNavigationTreeDragDropEffect(e);
		}
		void ChangeNodesState(TreeNodeCollection nodes, IEnumerable<IDragItem> dragItems) {
			foreach(TreeNode treeNode in nodes) {
				if(treeNode is IDropTargetItem)
					treeNode.ImageIndex = treeNode.SelectedImageIndex = CanDropToTargetNode((IDropTargetItem)treeNode, dragItems) ? 3 : treeNode.ImageIndex;
				ChangeNodesState(treeNode.Nodes, dragItems);
			}
		}
		protected void SetNavigationTreeDragDropEffect(DragEventArgs e) {
			IEnumerable<IDragItem> dragItems = GetDragObject(e.Data);
			if(dragItems == null) { e.Effect = DragDropEffects.None; return; }
			TreeNode node = Tree.GetNodeAt(Tree.PointToClient(new Point(e.X, e.Y)));
			if(node is IDropTargetItem)
				e.Effect = CanDropToTargetNode((IDropTargetItem)node, dragItems) ? DragDropEffects.Copy : DragDropEffects.None;
			ChangeNodesState(Tree.Nodes, dragItems);
		}
		IEnumerable<IDragItem> GetDragObject(IDataObject data) {
			DragInfo info = data.GetData(typeof(DragInfo)) as DragInfo;
			if(info == null) return null;
			return info != null ? info.Items : null;
		}
		#endregion
		void btnUp_Click(object sender, EventArgs e) {
			if(navigateTree.SelectedNode is IOrderedItem) {
				((IOrderedItem)navigateTree.SelectedNode).MoveUp();
				Repopulate();
			}
		}
		void btnDown_Click(object sender, EventArgs e) {
			if(navigateTree.SelectedNode is IOrderedItem) {
				((IOrderedItem)navigateTree.SelectedNode).MoveDown();
				Repopulate();
			}
		}
		void btnClear_Click(object sender, EventArgs e) {
			if(navigateTree.SelectedNode is IChildItem) {
				((IChildItem)navigateTree.SelectedNode).ResetParent();
				Repopulate();
				btnClear.Enabled = false;
			}
			if(navigateTree.SelectedNode is IActivationTarget) {
				((IActivationTarget)navigateTree.SelectedNode).ClearActivationTagret();
				Repopulate();
				btnClear.Enabled = false;
			}
		}
		void navigationTreeKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				IEnumerable<IDragItem> items = ConverToIDragItem(navigateTree.SelNodes);
				if(items == null) return;
				if(CanRemoveItems(items, typeof(DocumentNode))) {
					RemoveDocuments(items);
					e.Handled = true;
					Repopulate();
				}
				if(CanRemoveItems(items, typeof(TileNode))) {
					RemoveTiles(items);
					e.Handled = true;
					Repopulate();
				}
			}
		}
	}
	static class Classifier {
		public static TreeNode CreateNode(IContentContainer container) {
			if(container is Page)
				return new PageNode((Page)container);
			if(container is DocumentGroup)
				return new DocumentGroupNode((DocumentGroup)container);
			if(container is TileContainer)
				return new TileContainerNode((TileContainer)container);
			if(container is Flyout)
				return new FlyoutNode((Flyout)container);
			return null;
		}
	}
	class DragInfo {
		internal IEnumerable<IDragItem> Items { get; private set; }
		public DragInfo(IEnumerable<IDragItem> items) {
			Items = items;
		}
	}
	#region TreeNodes
	class InfrastructureNode : TreeNode, IDropTargetItem {
		public bool CanDrop(IDragItem item) {
			return CanDropCore(item);
		}
		public void Drop(IDragItem item) {
			DropCore(item);
		}
		protected virtual bool CanDropCore(IDragItem item) {
			if(Parent is IDropTargetItem)
				return ((IDropTargetItem)Parent).CanDrop(item);
			return false;
		}
		protected virtual void DropCore(IDragItem item) {
			if(Parent is IDropTargetItem)
				((IDropTargetItem)Parent).Drop(item);
		}
	}
	class DocumentNode : TreeNode, IDragItem, IOrderedItem {
		BaseDocument document;
		DocumentGroup group;
		public DocumentNode(BaseDocument document) {
			Tag = document;
			ImageIndex = SelectedImageIndex = 4;
			if(document != null) {
				Text = document.Site.Name;
				this.document = document;
			}
			else Text = "(Empty)";
		}
		public override string ToString() {
			if(document != null)
				return document.ControlName;
			else return "(Empty)";
		}
		public object Content {
			get { return Tag; }
		}
		public void MoveUp() {
			if(group == null) return;
			group.Items.Move(GetNewIndexForItem(group, (Document)document, true), (Document)document);
		}
		int GetNewIndexForItem(DocumentGroup group, Document document, bool isMoveUp) {
			int resutl = group.Items.IndexOf(document);
			resutl += isMoveUp ? -1 : 1;
			return Math.Max(0, Math.Min(group.Items.Count - 1, resutl));
		}
		public void MoveDown() {
			if(group == null) return;
			group.Items.Move(GetNewIndexForItem(group, (Document)document, false), (Document)document);
		}
		public bool CanMove() {
			DocumentGroup documentGroup = GetDocumentGroup();
			if(documentGroup != null && documentGroup.Items.Contains(document as Document) && documentGroup.Items.Count != 1) {
				group = documentGroup;
				return true;
			}
			group = null;
			return false;
		}
		internal DocumentGroup GetDocumentGroup() {
			for(TreeNode treeNodeParent = Parent; treeNodeParent != null; treeNodeParent = treeNodeParent.Parent) {
				if(treeNodeParent.Tag is DocumentGroup)
					return treeNodeParent.Tag as DocumentGroup;
			}
			return null;
		}
	}
	class TileNode : TreeNode, IDragItem, IDropTargetItem, IOrderedItem, IActivationTargetOwner {
		BaseTile baseTile;
		TileContainer container;
		internal TileNode(BaseTile baseTile) {
			this.baseTile = baseTile;
			Tag = baseTile;
			Text = baseTile.Site.Name;
			Tile tile = baseTile as Tile;
			ImageIndex = SelectedImageIndex = 5;
			if(tile != null) {
				Nodes.Add(new DocumentNode(tile.Document));
				Nodes.Add(new ActivationTargetNode(tile.ActivationTarget));
			}
		}
		public override string ToString() {
			if(baseTile != null)
				return baseTile.Site.Name;
			else return "(Empty)";
		}
		public object Content {
			get { return Tag; }
		}
		public bool CanDrop(IDragItem item) {
			return item.Content is IContentContainer && ActivationTargetHelper.CanAddActivationTarget((Tile)baseTile, item.Content as IContentContainer);
		}
		public void Drop(IDragItem item) {
			Tile tile = baseTile as Tile;
			if(tile != null) {
				IContentContainer activationTarget = item.Content as IContentContainer;
				if(ActivationTargetHelper.CanAddActivationTarget(tile, activationTarget)) {
					activationTarget.Parent = GetTileContainer();
					tile.ActivationTarget = activationTarget;
				}
			}
		}
		public void MoveUp() {
			if(container == null) return;
			container.Items.Move(GetNewIndexForItem(container, baseTile, true), baseTile);
		}
		public void MoveDown() {
			if(container == null) return;
			container.Items.Move(GetNewIndexForItem(container, baseTile, false), baseTile);
		}
		int GetNewIndexForItem(TileContainer tileContainer, BaseTile tile, bool isMoveUp) {
			int resutl = tileContainer.Items.IndexOf(tile);
			resutl += isMoveUp ? -1 : 1;
			return Math.Max(0, Math.Min(tileContainer.Items.Count - 1, resutl));
		}
		public bool CanMove() {
			TileContainer tileContainer = GetTileContainer();
			if(tileContainer != null && tileContainer.Items.Contains(baseTile) && tileContainer.Items.Count != 1) {
				container = tileContainer;
				return true;
			}
			container = null;
			return false;
		}
		internal TileContainer GetTileContainer() {
			for(TreeNode treeNodeParent = Parent; treeNodeParent != null; treeNodeParent = treeNodeParent.Parent) {
				if(treeNodeParent.Tag is TileContainer)
					return treeNodeParent.Tag as TileContainer;
			}
			return null;
		}
		public void ClearActivationTagret() {
			Tile tile = baseTile as Tile;
			if(tile != null && tile.ActivationTarget != null) {
				if(ActivationTargetAlreadyAssignedWithTiles(tile, tile.ActivationTarget.Parent as TileContainer))
					tile.ActivationTarget.Parent = null;
				tile.ActivationTarget = null;
			}
		}
		bool ActivationTargetAlreadyAssignedWithTiles(Tile tile, TileContainer tileContainer) {
			if(tileContainer != null) {
				foreach(Tile existingTile in tileContainer.Items)
					if(existingTile != tile && existingTile.ActivationTarget == tile.ActivationTarget) return false;
			}
			return true;
		}
		public bool CanAddActivationTagret(IDragItem item) {
			return CanDrop(item);
		}
	}
	class ChildrenNode : InfrastructureNode, IChildItem {
		IContentContainer parent;
		IContentContainer except;
		IList<IContentContainer> children;
		public ChildrenNode(IContentContainer parent, IContentContainer except) {
			this.parent = parent;
			this.except = except;
			children = new List<IContentContainer>();
			PopulateChildren();
			Tag = parent;
			Text = "Children";
			ImageIndex = SelectedImageIndex = 1;
			foreach(IContentContainer child in children) {
				Nodes.Add(Classifier.CreateNode(child));
			}
		}
		void PopulateChildren() {
			using(var e = parent.GetEnumerator(1)) {
				while(e.MoveNext()) {
					if(e.Current == except || e.Current == parent) continue;
					children.Add(e.Current);
				}
			}
		}
		protected override bool CanDropCore(IDragItem item) {
			return item.Content is IContentContainer && item.Content != this.parent;
		}
		protected override void DropCore(IDragItem item) {
			IContentContainer contentContainer = item.Content as IContentContainer;
			if(contentContainer.Parent != parent && contentContainer != parent.Parent)
				((IContainerRemove)contentContainer).ContainerRemoved();
			contentContainer.Parent = parent;
		}
		public bool CanResetParent() {
			if(children == null) return false;
			if(children.Count == 1)
				return ActivationTargetHelper.CanRemoveChildItem(children[0], children[0].Parent);
			return children.Count != 0;
		}
		public void ResetParent() {
			foreach(IContentContainer child in children) {
				if(ActivationTargetHelper.CanRemoveChildItem(child, child.Parent))
					child.Parent = null;
			}
		}
	}
	class ActivationTargetNode : InfrastructureNode, IActivationTarget {
		IContentContainer container;
		internal ActivationTargetNode(IContentContainer target) {
			Tag = target;
			Text = "ActivationTarget";
			ImageIndex = SelectedImageIndex = 0;
			this.container = target;
			if(target != null)
				Nodes.Add(Classifier.CreateNode(target));
			Expand();
		}
		protected override bool CanDropCore(IDragItem item) {
			if(Parent is IActivationTargetOwner)
				return ((IActivationTargetOwner)Parent).CanAddActivationTagret(item);
			return item.Content is IContentContainer;
		}
		public bool CanClearActivationTarget() {
			return container != null && Parent is IActivationTargetOwner;
		}
		public void ClearActivationTagret() {
			if(Parent is IActivationTargetOwner)
				((IActivationTargetOwner)Parent).ClearActivationTagret();
		}
	}
	class TileContainerNode : BaseContentContainerNode, IDropTargetItem, IActivationTargetOwner {
		TileContainer tileContainer { get { return Container as TileContainer; } }
		public TileContainerNode(TileContainer container) : base(container) {		   
			ImageIndex = SelectedImageIndex = 11;
			Nodes.Add(new ItemsNode(container.Items));
			Nodes.Add(new ActivationTargetNode(container.ActivationTarget));
			Nodes.Add(new ChildrenNode(container, container.ActivationTarget));
			Expand();
		}
		class ItemsNode : InfrastructureNode {
			TileCollection items;
			public ItemsNode(TileCollection items) {
				this.items = items;
				Tag = items.Owner;
				Text = "Items";
				ImageIndex = SelectedImageIndex = 2;
				foreach(BaseTile tile in items)
					Nodes.Add(new TileNode(tile));
				Expand();
			}
		}
		protected bool CanAddTile(IDragItem item) {
			Tile tile = item.Content as Tile;
			if(tile != null) {
				return tile.ActivationTarget != tileContainer;
			}
			return true;
		}
		public bool CanDrop(IDragItem item) {
			return item is TileNode && CanAddTile(item);
		}
		public void Drop(IDragItem item) {
			if(item.Content is IContentContainer)
				tileContainer.ActivationTarget = item.Content as IContentContainer;
			if(item.Content is BaseTile) {
				if(CanAddTile(item))
					tileContainer.Items.Add(item.Content as BaseTile);
			}
		}
		public void ClearActivationTagret() {
			if(tileContainer.ActivationTarget != null)
				tileContainer.ActivationTarget.Parent = null;
			tileContainer.ActivationTarget = null;
		}
		public bool CanAddActivationTagret(IDragItem item) {
			return tileContainer.ActivationTarget != item.Content && item.Content != tileContainer && item.Content is IContentContainer;
		}
	}
	class BaseContentContainerNode : TreeNode, IDragItem, IChildItem, IOrderedItem {
		BaseContentContainer container;
		protected BaseContentContainer Container { get { return container; } }
		public BaseContentContainerNode(BaseContentContainer container) {
			this.container = container;
			Tag = container;
			Text = container.Site.Name;
		}
		public bool CanResetParent() {
			return container != null && container.Parent != null && !(Parent is ActivationTargetNode) && ActivationTargetHelper.CanRemoveChildItem(container, container.Parent);
		}
		public void ResetParent() {
			container.Parent = null;
		}
		WindowsUIView View {
			get {
				if(container == null || container.Manager == null) return null;
				return container.Manager.View as WindowsUIView;
			}
		}
		#region IOrderedItem Members
		public void MoveUp() {
			View.ContentContainers.Move(GetNewIndexForItem(container, true), container);
		}
		int GetNewIndexForItem(BaseContentContainer group, bool isMoveUp) {
			int resutl = View.ContentContainers.IndexOf(group);
			resutl += isMoveUp ? -1 : 1;
			return Math.Max(0, Math.Min(View.ContentContainers.Count - 1, resutl));
		}
		public void MoveDown() {
			View.ContentContainers.Move(GetNewIndexForItem(container, false), container);
		}
		public bool CanMove() {
			if(View != null && View.ContentContainers.Count > 1) return true;
			return false;
		}
		#endregion
		public object Content {
			get { return Tag; }
		}
	}
	class DocumentGroupNode : BaseContentContainerNode, IDropTargetItem {
		DocumentGroup documentGroup { get { return Container as DocumentGroup; } }
		public DocumentGroupNode(DocumentGroup container) : base(container) {
			if(container is PageGroup)
				ImageIndex = SelectedImageIndex = 7;
			if(container is SlideGroup)
				ImageIndex = SelectedImageIndex = 8;
			if(container is SplitGroup)
				ImageIndex = SelectedImageIndex = 9;
			if(container is TabbedGroup)
				ImageIndex = SelectedImageIndex = 10;
			Nodes.Add(new ItemsNode(container.Items));
			Nodes.Add(new ChildrenNode(container, null));
			Expand();
		}
		class ItemsNode : InfrastructureNode {
			DocumentCollection items;
			public ItemsNode(DocumentCollection items) {
				this.items = items;
				Tag = items.Owner;
				Text = "Items";
				ImageIndex = SelectedImageIndex = 2;
				foreach(Document document in items)
					Nodes.Add(new DocumentNode(document));
				Expand();
			}
		}
		public bool CanDrop(IDragItem item) {
			return item is DocumentNode && !documentGroup.Items.Contains(item.Content as Document);
		}
		public void Drop(IDragItem item) {
			documentGroup.Items.Add(item.Content as Document);
		}
	}
	class PageNode : BaseContentContainerNode, IDropTargetItem {
		Page page { get { return Container as Page; } }
		public PageNode(Page page) :base(page){		  
			ImageIndex = SelectedImageIndex = 6;
			Nodes.Add(new DocumentNode(page.Document));
			Nodes.Add(new ChildrenNode(page, null));
			Expand();
		}
		public bool CanDrop(IDragItem item) {
			return item.Content is Document;
		}
		public void Drop(IDragItem item) {
			page.Document = item.Content as Document;
		}
	}
	class FlyoutNode : BaseContentContainerNode, IDropTargetItem {
		Flyout flyout { get { return Container as Flyout; } }
		public FlyoutNode(Flyout flyout) : base(flyout) {		   
			ImageIndex = SelectedImageIndex = 12;
			Nodes.Add(new DocumentNode(flyout.Document));
			Nodes.Add(new ChildrenNode(flyout, null));
			Expand();
		}
		public bool CanDrop(IDragItem item) {
			return item.Content is Document;
		}
		public void Drop(IDragItem item) {
			flyout.Document = item.Content as Document;
		}
	}
	#endregion
	#region Interfaces
	interface IActivationTarget {
		bool CanClearActivationTarget();
		void ClearActivationTagret();
	}
	interface IActivationTargetOwner {
		void ClearActivationTagret();
		bool CanAddActivationTagret(IDragItem item);
	}
	interface IChildItem {
		bool CanResetParent();
		void ResetParent();
	}
	interface IOrderedItem {
		void MoveUp();
		void MoveDown();
		bool CanMove();
	}
	interface IDragItem {
		object Content { get; }
	}
	interface IDropTargetItem {
		bool CanDrop(IDragItem item);
		void Drop(IDragItem item);
	}
	#endregion
}
