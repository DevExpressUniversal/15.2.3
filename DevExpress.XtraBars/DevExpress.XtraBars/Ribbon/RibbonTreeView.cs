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
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Frames;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.Customization;
namespace DevExpress.XtraBars.Ribbon.Customization {
	#region Enums
	public enum ItemsAdditionMode { Add, AddNotAll, AddToParent, AddToParentNotAll, CantAdd, AddAfterTarget, AddBeforeTarget, None, Move, Copy }
	public enum RibbonTreeSelectionMode { None, Page, PageCategory, PageGroup, Group, Item, ButtonGroupItem, Unused }
	public enum RibbonTreeImages { Category, Page, Group, Item, ButtonItem, Add, AddNotAll, AddUp, AddUpNotAll, CantAdd, ItemBeginGroup, AddDown, AddDownNotAll, PageCategory };
	#endregion
	#region Info
	public class DXTreeViewRibbonItemInfo {
		object item;
		TreeNode node;
		public DXTreeViewRibbonItemInfo(TreeNode node, object item) {
			this.node = node;
			this.item = item;
		}
		public object Item { get { return item; } set { item = value; } }
		public TreeNode Node { get { return node; } }
		public bool IsGallery { get { return (Item is RibbonGalleryBarItem) || (Item is RibbonGalleryBarItemLink); } }
		public bool IsItem {
			get {
				return (Item is BarItem) || (Item is BarItemLink);
			}
		}
		public bool IsBarButtonGroup {
			get {
				return (Item is BarButtonGroup) || (Item is BarButtonGroupLink);
			}
		}
		public bool IsEditor { get { return (Item is BarEditItem) || (Item is BarEditItemLink); } }
		public bool CanAddToButtonGroup { get { return !(IsGallery | IsBarButtonGroup | IsEditor); } }
	}
	public class RibbonItemsDragInfo {
		DXTreeViewRibbonItemInfo[] items;
		bool dragFromList;
		bool canDropGallery;
		public RibbonItemsDragInfo(RibbonItemsListBox list) {
			this.canDropGallery = true;
			this.dragFromList = true;
			this.items = new DXTreeViewRibbonItemInfo[list.SelectedIndices.Count];
			for(int i = 0; i < list.SelectedIndices.Count; i++) {
				this.items[i] = new DXTreeViewRibbonItemInfo(null, list.GetBarItem(list.SelectedIndices[i]));
			}
		}
		public RibbonItemsDragInfo(TreeNode[] nodes) {
			this.dragFromList = false;
			this.items = new DXTreeViewRibbonItemInfo[nodes.Length];
			for(int i = 0; i < items.Length; i++) {
				this.items[i] = new DXTreeViewRibbonItemInfo(nodes[i], nodes[i].Tag);
			}
		}
		public RibbonItemsDragInfo(ListView.SelectedListViewItemCollection items) {
			this.dragFromList = true;
			this.items = new DXTreeViewRibbonItemInfo[items.Count];
			for(int i = 0; i < items.Count; i++)
				this.items[i] = new DXTreeViewRibbonItemInfo(null, items[i].Tag);
		}
		public bool DragFromList { get { return dragFromList; } }
		public DXTreeViewRibbonItemInfo[] Items { get { return items; } }
		ItemsAdditionMode GetAdditionMode(int add, int notAdd, int addUp) {
			if(addUp != 0) {
				if(notAdd != 0) return ItemsAdditionMode.AddToParentNotAll;
				return ItemsAdditionMode.AddToParent;
			}
			if(add != 0) {
				if(notAdd != 0) return ItemsAdditionMode.AddNotAll;
				return ItemsAdditionMode.Add;
			}
			return ItemsAdditionMode.CantAdd;
		}
		RibbonPageCategory Category(TreeNode node) { return node.Tag as RibbonPageCategory; }
		public ItemsAdditionMode CanAddItemsToPageCategory(TreeNode node) {
			int add = 0, notAdd = 0, addUp = 0;
			for(int i = 0; i < Items.Length; i++) {
				RibbonPageCategory pageCategory = Items[i].Item as RibbonPageCategory;
				if(pageCategory != null && pageCategory != pageCategory.Collection.DefaultCategory && Category(node) != pageCategory.Collection.DefaultCategory) addUp++;
				else if(Items[i].Item is RibbonPage) add++;
				else notAdd++;
			}
			return GetAdditionMode(add, notAdd, addUp);
		}
		public ItemsAdditionMode CanAddItemsToPage() {
			int add = 0, notAdd = 0, addUp = 0;
			for(int i = 0; i < Items.Length; i++) {
				if(Items[i].Item is RibbonPageGroup) add++;
				else if(Items[i].Item is RibbonPage) addUp++;
				else notAdd++;
			}
			return GetAdditionMode(add, notAdd, addUp);
		}
		public ItemsAdditionMode CanAddItemsToGroup() {
			int add = 0, notAdd = 0, addUp = 0;
			for(int i = 0; i < Items.Length; i++) {
				if(Items[i].IsItem) add++;
				else if(Items[i].Item is RibbonPageGroup) addUp++;
				else notAdd++;
			}
			return GetAdditionMode(add, notAdd, addUp);
		}
		public virtual bool CanDropGallery { get { return canDropGallery; } set { canDropGallery = value; } }
		public ItemsAdditionMode CanAddItemsToItems(object obj) {
			int add = 0, notAdd = 0, addUp = 0;
			for(int i = 0; i < Items.Length; i++) {
				if(IsBarButtonGroup(obj)) {
					if(Items[i].IsItem && Items[i].CanAddToButtonGroup) add++;
					else notAdd++;
				}
				else if(Items[i].IsItem) addUp++;
				else notAdd++;
			}
			return GetAdditionMode(add, notAdd, addUp);
		}
		public ItemsAdditionMode CanAddItemsToButtonGroupItems() {
			int notAdd = 0, addUp = 0;
			for(int i = 0; i < Items.Length; i++) {
				if(Items[i].IsItem && Items[i].CanAddToButtonGroup) addUp++;
				else notAdd++;
			}
			return GetAdditionMode(0, notAdd, addUp);
		}
		public bool IsBarButtonGroup(object obj) {
			if((obj as BarButtonGroup) != null) return true;
			if((obj as BarButtonGroupLink) != null) return true;
			return false;
		}
		public virtual ItemsAdditionMode CanAddItems(TreeNode node, RibbonTreeSelectionMode mode) {
			switch(mode) {
				case RibbonTreeSelectionMode.None:
					return ItemsAdditionMode.Add;
				case RibbonTreeSelectionMode.PageCategory:
					return CanAddItemsToPageCategory(node);
				case RibbonTreeSelectionMode.Page:
					return CanAddItemsToPage();
				case RibbonTreeSelectionMode.Group:
					return CanAddItemsToGroup();
				case RibbonTreeSelectionMode.Item:
					return CanAddItemsToItems(node == null ? null : node.Tag);
				case RibbonTreeSelectionMode.ButtonGroupItem:
					return CanAddItemsToButtonGroupItems();
			}
			return ItemsAdditionMode.CantAdd;
		}
	}
	#endregion
	#region Controls
	[ToolboxItem(false)]
	public class RibbonItemsListBox : BarItemsListBox {
		protected override void DoDragDrop(MouseEventArgs e) {
			DoDragDrop(new RibbonItemsDragInfo(this), DragDropEffects.Copy);
		}
		protected override void OnQueryContinueDragCore(QueryContinueDragEventArgs e) {
		}
		protected override void OnGiveFeedbackCore(GiveFeedbackEventArgs e) {
		}
		public object[] SelectedObjects {
			get {
				if(SelectedIndices.Count == 0) return null;
				object[] obj = new object[SelectedIndices.Count];
				for(int i = 0; i < SelectedIndices.Count; i++) {
					obj[i] = GetBarItem(SelectedIndices[i]);
				}
				return obj;
			}
		}
	}
	[ToolboxItem(false)]
	public class ItemsTreeView : DXTreeView {
		public ItemsTreeView() {
			DragNodeGetObject += new TreeViewGetDragObjectEventHandler(OnDragNodeGetObject);
			DragNodeStart += new EventHandler(OnDragNodeStart);
			DragOver += new DragEventHandler(OnDragOver);
			DragEnter += new DragEventHandler(OnDragEnter);
			DragLeave += new EventHandler(OnDragLeave);
			DragDrop += new DragEventHandler(OnDragDrop);
			SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			AllowDrag = true;
			AllowDrop = true;
		}
		protected override void Dispose(bool disposing) {
			DragNodeGetObject -= new TreeViewGetDragObjectEventHandler(OnDragNodeGetObject);
			DragNodeStart -= new EventHandler(OnDragNodeStart);
			DragOver -= new DragEventHandler(OnDragOver);
			DragEnter -= new DragEventHandler(OnDragEnter);
			DragLeave -= new EventHandler(OnDragLeave);
			DragDrop -= new DragEventHandler(OnDragDrop);
			base.Dispose(disposing);
		}
		public virtual void OnDragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) { }
		public virtual void OnDragNodeStart(object sender, EventArgs e) { }
		protected virtual void OnDragOver(object sender, DragEventArgs e) { }
		public virtual void OnDragEnter(object sender, DragEventArgs e) { }
		public virtual void OnDragLeave(object sender, EventArgs e) { }
		protected virtual void OnDragDrop(object sender, DragEventArgs e) { }
		protected void SetNodeImageIndex(TreeNode node, int index) {
			if(node == null) return;
			node.SelectedImageIndex = node.ImageIndex = index;
		}
		protected virtual void UpdateTreeNodeText(TreeNode node, object item) { }
		public virtual void UpdateTreeNodesText(object item) { UpdateTreeNodesText(Nodes, item); }
		protected virtual void UpdateTreeNodesText(TreeNodeCollection nodes, object item) {
			for(int i = 0; i < nodes.Count; i++) {
				UpdateTreeNodesText(nodes[i].Nodes, item);
				UpdateTreeNodeText(nodes[i], item);
			}
		}
	}
	[ToolboxItem(false)]
	public class ItemLinksTreeView : ItemsTreeView {
		BarItemLinkCollection itemLinks;
		TreeNode dropTargetNode = null;
		TreeNode dropSelectedNode = null;
		RibbonControl ribbon;
		RibbonStatusBar statusBar;
		IDesignerHost designerHost = null;
		IComponentChangeService componentChangeService = null;
		public ItemLinksTreeView(BarItemLinkCollection itemLinks)
			: base() {
			this.itemLinks = itemLinks;
		}
		[Browsable(false)]
		public IDesignerHost DesignerHost { get { return designerHost; } set { designerHost = value; } }
		[Browsable(false)]
		public IComponentChangeService ComponentChangeService {
			get { return componentChangeService; }
			set { componentChangeService = value; }
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get { return ribbon; } set { ribbon = value; } }
		[Browsable(false)]
		public RibbonStatusBar StatusBar { get { return statusBar; } set { statusBar = value; } }
		public ItemLinksTreeView() : this(null) { }
		[Browsable(false)]
		public BarItemLinkCollection ItemLinks { get { return itemLinks; } set { itemLinks = value; } }
		[Browsable(false)]
		public TreeNode DropTargetNode { get { return dropTargetNode; } set { dropTargetNode = value; } }
		[Browsable(false)]
		public TreeNode DropSelectedNode { get { return dropSelectedNode; } set { dropSelectedNode = value; } }
		public virtual void RefreshObjects() {
			if(Ribbon != null) Ribbon.Refresh();
			if(StatusBar != null) StatusBar.Refresh();
		}
		public void ClearTree() {
			Nodes.Clear();
		}
		protected bool ButtonGroupHasClone(TreeNodeCollection nodes, TreeNode node) {
			if(ButtonGroup(node) == null) return false;
			for(int i = 0; i < nodes.Count; i++) {
				if(ButtonGroupHasClone(nodes[i].Nodes, node) == true) return true;
				if(nodes[i] != node && ButtonGroup(nodes[i]) != null && ButtonGroup(nodes[i]) == ButtonGroup(node)) return true;
			}
			return false;
		}
		protected void SynchronizeButtonGroups(TreeNode clonedNode, TreeNode originParent) {
			int i;
			BarButtonGroup group = ButtonGroup(originParent);
			for(i = 0; i < group.ItemLinks.Count; i++) {
				if(i < clonedNode.Nodes.Count && clonedNode.Nodes[i].Tag == group.ItemLinks[i]) continue;
				if(i < clonedNode.Nodes.Count - 1 && clonedNode.Nodes[i + 1].Tag == group.ItemLinks[i]) {
					clonedNode.Nodes.RemoveAt(i);
					continue;
				}
				TreeNode node = new TreeNode();
				node.Tag = group.ItemLinks[i];
				node.Text = ItemCaption(group.ItemLinks[i]);
				node.ImageIndex = (int)RibbonTreeImages.ButtonItem;
				node.SelectedImageIndex = (int)RibbonTreeImages.ButtonItem;
				clonedNode.Nodes.Insert(i, node);
			}
			while(clonedNode.Nodes.Count > group.ItemLinks.Count) {
				clonedNode.Nodes.RemoveAt(group.ItemLinks.Count);
			}
		}
		protected void SynchronizeButtonGroups(TreeNode originParent) {
			if(ButtonGroupHasClone(Nodes, originParent) == false) return;
			SynchronizeButtonGroups(Nodes, originParent);
		}
		protected void SynchronizeButtonGroups(TreeNodeCollection nodes, TreeNode originParent) {
			for(int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++) {
				SynchronizeButtonGroups(nodes[nodeIndex].Nodes, originParent);
				if(ButtonGroup(nodes[nodeIndex]) != null && ButtonGroup(nodes[nodeIndex]) == ButtonGroup(originParent)) SynchronizeButtonGroups(nodes[nodeIndex], originParent);
			}
		}
		public TreeNode CreateNode(BarItemLinkCollection links, TreeNode parentNode, int itemIndex, RibbonTreeImages imageIndex) {
			TreeNode node = new TreeNode(ItemCaption(links[itemIndex]), (int)imageIndex, (int)imageIndex);
			node.Tag = links[itemIndex];
			if(parentNode == null) Nodes.Add(node);
			else parentNode.Nodes.Add(node);
			return node;
		}
		public virtual void CreateTree() {
			ClearTree();
			if(itemLinks == null)
				return;
			BeginUpdate();
			try {
				for(int itemIndex = 0; itemIndex < itemLinks.Count; itemIndex++) {
					TreeNode node = CreateNode(ItemLinks, null, itemIndex, ItemImageIndex(false, ItemLinks[itemIndex]));
					if(ItemLinks[itemIndex] is BarButtonGroupLink) CreateTree(node, (ItemLinks[itemIndex].Item as BarButtonGroup).ItemLinks);
				}
			}
			finally { EndUpdate(); }
		}
		public void CreateTree(TreeNode node, BarItemLinkCollection links) {
			for(int itemIndex = 0; itemIndex < links.Count; itemIndex++) {
				CreateNode(links, node, itemIndex, ItemImageIndex(true, links[itemIndex]));
			}
		}
		public BarItemLink Item(TreeNode node) {
			if(node == null || (node.Parent != null && Item(node.Parent) != null)) return null;
			return node.Tag as BarItemLink;
		}
		public BarItemLink ButtonItem(TreeNode node) {
			if(node == null || node.Parent == null || Item(node.Parent) == null) return null;
			return node.Tag as BarItemLink;
		}
		protected BarItemLink SwapItems(BarItemLinkCollection links, BarItemLink link1, BarItemLink link2) {
			int index1 = links.IndexOf(link1);
			links.Insert(index1, link2.Item);
			links.Remove(link2);
			return links[index1];
		}
		protected virtual object SwapObjects(TreeNode node1, TreeNode node2) {
			if(node1.Parent != null) return SwapItems(GetLinks(node1), ButtonItem(node1), ButtonItem(node2));
			return SwapItems(GetLinks(node1), Item(node1), Item(node2));
		}
		protected virtual void SwapNodes(TreeNodeCollection nodes, TreeNode node1, TreeNode node2) {
			int index1 = nodes.IndexOf(node1);
			nodes.Remove(node2);
			nodes.Insert(index1, node2);
		}
		protected virtual TreeNode SwapNearNodes(TreeNode node1, TreeNode node2) {
			node2.Tag = SwapObjects(node1, node2);
			SwapNodes(GetParentNodes(node1), node1, node2);
			return node1;
		}
		protected TreeNodeCollection GetParentNodes(TreeNode node) {
			if(node.Parent == null) return Nodes;
			return node.Parent.Nodes;
		}
		BarItemLinkCollection GetLinks(TreeNode node) {
			if(node == null || node.Parent == null) return ItemLinks;
			return GetButtonGroup(node.Parent.Tag).ItemLinks;
		}
		protected virtual void ChangeComponents() {
			if(StatusBar != null) ComponentChangeService.OnComponentChanged(StatusBar, null, null, null);
			if(Ribbon != null) ComponentChangeService.OnComponentChanged(Ribbon.Toolbar, null, null, null);
		}
		public virtual void MoveUp() {
			if(SelectedNode == null || SelectedNode.PrevNode == null) return;
			SelectedNode = SwapNearNodes(SelectedNode.PrevNode, SelectedNode).PrevNode;
			RefreshObjects();
			ChangeComponents();
			SynchronizeButtonGroups(SelectedNode.Parent);
		}
		public virtual void MoveDown() {
			if(SelectedNode == null || SelectedNode.NextNode == null) return;
			SelectedNode = SwapNearNodes(SelectedNode, SelectedNode.NextNode);
			RefreshObjects();
			ChangeComponents();
			SynchronizeButtonGroups(SelectedNode.Parent);
		}
		protected virtual bool RemoveObject(TreeNode node) {
			(node.Tag as BarItemLink).Dispose();
			return true;
		}
		public virtual void Remove() {
			if(Nodes.Count == 0 || SelNodes == null || SelNodes.Length == 0) return;
			int count = SelCount;
			BeginUpdate();
			try {
				TreeNode[] nodes = (TreeNode[])SelNodes.Clone();
				for(int itemIndex = 0; itemIndex < count; itemIndex++) {
					if(RemoveObject(nodes[itemIndex])) {
						TreeNode pnode = nodes[itemIndex].Parent;
						GetParentNodes(nodes[itemIndex]).Remove(nodes[itemIndex]);
						SynchronizeButtonGroups(pnode);
					}
				}
			}
			finally {
				EndUpdate();
				ChangeComponents();
				RefreshObjects();
			}
		}
		protected virtual bool CanDropGallery { get { return false; } }
		public override void OnDragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			TreeNode[] nodes = DragDropSelNodes;
			if(nodes.Length < 1) return;
			RibbonItemsDragInfo info = CreateRibbonItemsDragInfo(nodes);
			info.CanDropGallery = CanDropGallery;
			e.DragObject = info;
			e.AllowEffects = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected virtual TreeNode[] DragDropSelNodes {
			get { return SelNodes; }
		}
		protected virtual RibbonItemsDragInfo CreateRibbonItemsDragInfo(TreeNode[] SelNodes) {
			return new RibbonItemsDragInfo(SelNodes);
		}
		protected void SetDragDropEffect(DragEventArgs e) {
			if((e.AllowedEffect & DragDropEffects.Move) == 0) {
				e.Effect = DragDropEffects.Copy;
				return;
			}
			e.Effect = DragDropEffects.Move;
			if((e.KeyState & 8) != 0) {
				RibbonItemsDragInfo dragInfo = GetDragObject(e.Data);
				if(dragInfo.Items.Length > 0 &&
					(dragInfo.Items[0].Item is RibbonPageGroup || dragInfo.Items[0].Item is RibbonPage)) return;
				e.Effect |= DragDropEffects.Copy;
			}
		}
		public override void OnDragNodeStart(object sender, EventArgs e) {
			DropSelectedNode = SelectedNode;
			DropTargetNode = SelectedNode;
		}
		public override void OnDragEnter(object sender, DragEventArgs e) {
			RibbonItemsDragInfo info = GetDragObject(e.Data);
			info.CanDropGallery = false;
			SetDragDropEffect(e);
		}
		public override void OnDragLeave(object sender, EventArgs e) {
			ResetNodeImage(DropTargetNode);
			DropTargetNode = null;
		}
		protected virtual RibbonTreeImages ItemImageIndex(bool buttonItem, BarItemLink link) {
			if(!buttonItem) return link.BeginGroup ? RibbonTreeImages.ItemBeginGroup : RibbonTreeImages.Item;
			return link.BeginGroup ? RibbonTreeImages.ButtonItem : RibbonTreeImages.ButtonItem;
		}
		protected virtual RibbonTreeImages ItemImageIndex(TreeNode node) {
			return ItemImageIndex(node.Parent != null, node.Tag as BarItemLink);
		}
		public virtual void ResetNodeImage(TreeNode node) {
			if(node == null) return;
			node.SelectedImageIndex = node.ImageIndex = (int)ItemImageIndex(node);
		}
		protected virtual RibbonTreeSelectionMode GetSelectionMode(TreeNode node) {
			if(node == null || node.Parent == null) return RibbonTreeSelectionMode.Item;
			return RibbonTreeSelectionMode.ButtonGroupItem;
		}
		protected virtual RibbonItemsDragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(RibbonItemsDragInfo)) as RibbonItemsDragInfo;
		}
		protected int GetImageUpDownIndex(TreeNode targetNode, TreeNode sourceNode, bool notAll) {
			if(sourceNode != null && targetNode.Parent == sourceNode.Parent && targetNode.Index > sourceNode.Index) {
				return notAll ? (int)RibbonTreeImages.AddDownNotAll : (int)RibbonTreeImages.AddDown;
			}
			return notAll ? (int)RibbonTreeImages.AddUpNotAll : (int)RibbonTreeImages.AddUp;
		}
		protected override void OnDragOver(object sender, DragEventArgs e) {
			RibbonItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			if(node == DropTargetNode && node != null) return;
			ResetNodeImage(DropTargetNode);
			DropTargetNode = node;
			SetDragDropEffect(e);
			if(node == DropSelectedNode && !dragInfo.DragFromList) {
				e.Effect = DragDropEffects.None;
				SetNodeImageIndex(node, (int)RibbonTreeImages.CantAdd);
				return;
			}
			switch(dragInfo.CanAddItems(node, GetSelectionMode(node))) {
				case ItemsAdditionMode.Add:
					SetNodeImageIndex(node, (int)RibbonTreeImages.Add);
					break;
				case ItemsAdditionMode.AddNotAll:
					SetNodeImageIndex(node, (int)RibbonTreeImages.AddNotAll);
					break;
				case ItemsAdditionMode.AddToParent:
					SetNodeImageIndex(node, GetImageUpDownIndex(DropTargetNode, dragInfo.Items[0].Node, false));
					break;
				case ItemsAdditionMode.AddToParentNotAll:
					SetNodeImageIndex(node, GetImageUpDownIndex(DropTargetNode, dragInfo.Items[0].Node, true));
					break;
				case ItemsAdditionMode.CantAdd:
					e.Effect = DragDropEffects.None;
					SetNodeImageIndex(node, (int)RibbonTreeImages.CantAdd);
					return;
			}
		}
		TreeNodeCollection GetChildNodes(TreeNode parentNode) {
			if(parentNode == null) return Nodes;
			return parentNode.Nodes;
		}
		protected override void OnDragDrop(object sender, DragEventArgs e) {
			ResetNodeImage(DropTargetNode);
			RibbonItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			bool inButtonGroup = DropTargetNode == null ? false : Item(DropTargetNode.Parent) != null;
			TreeNodeCollection nodes = inButtonGroup ? DropTargetNode.Parent.Nodes : Nodes;
			BarItemLinkCollection links = inButtonGroup ? ButtonGroup(DropTargetNode.Parent).ItemLinks : ItemLinks;
			CopyItems(nodes, links, DropTargetNode, dragInfo.Items as DXTreeViewRibbonItemInfo[], e.Effect == DragDropEffects.Move, inButtonGroup);
		}
		bool IsButtonGroup(TreeNode node) {
			if(node == null || node.Tag == null || !(node.Tag is BarButtonGroupLink)) return false;
			return true;
		}
		BarItem GetItem(object item) {
			if(item as BarItem != null) return item as BarItem;
			return (item as BarItemLink).Item;
		}
		protected virtual string FullCaption(string text, string name) {
			return text + "   <" + name + ">";
		}
		protected virtual string ItemCaption(BarItemLink link) {
			return ItemCaption(link.Item);
		}
		protected string ItemCaption(BarItem item) {
			return FullCaption(item.Caption, item.Name);
		}
		protected BarButtonGroup GetButtonGroup(object item) { return GetItem(item) as BarButtonGroup; }
		TreeNode CreateCopyTreeNode(BarItemLink link, RibbonTreeImages imageIndex) {
			TreeNode node;
			node = new TreeNode(ItemCaption(link), (int)imageIndex, (int)imageIndex);
			node.Tag = link;
			return node;
		}
		void InsertCopyTreeNode(TreeNodeCollection nodes, TreeNode node, int index) {
			if(nodes != null) nodes.Insert(index, node);
			else Nodes.Insert(index, node);
		}
		DXTreeViewRibbonItemInfo[] GetChildItems(BarButtonGroup group) {
			DXTreeViewRibbonItemInfo[] items = new DXTreeViewRibbonItemInfo[group.ItemLinks.Count];
			for(int itemIndex = 0; itemIndex < group.ItemLinks.Count; itemIndex++)
				items[itemIndex] = new DXTreeViewRibbonItemInfo(null, group.ItemLinks[itemIndex]);
			return items;
		}
		protected void RemoveAfterCopy(TreeNode node) {
			if(node.Tag is RibbonPageGroup) return;
			if(node.Tag is RibbonPage) return;
			if(node.Tag is RibbonPageCategory) return;
			RemoveObject(node);
			GetParentNodes(node).Remove(node);
		}
		protected int GetInsertPosition(TreeNode targetNode, TreeNodeCollection nodes, TreeNode node) {
			if(targetNode == null) return nodes.Count;
			if(node == null) return targetNode.Index;
			if(targetNode.Parent == node.Parent && targetNode.Index > node.Index) return targetNode.Index + 1;
			return targetNode.Index;
		}
		protected int GetInsertPosition(TreeNode targetNode, ItemsAdditionMode mode) {
			if(mode == ItemsAdditionMode.AddAfterTarget) return targetNode.Index + 1;
			return targetNode.Index;
		}
		bool IsInButtonGroup(TreeNode targetNode) { return targetNode != null && targetNode.Parent != null; }
		protected BarButtonGroup ButtonGroup(TreeNode node) {
			if(Item(node) == null) return null;
			return Item(node).Item as BarButtonGroup;
		}
		bool ShouldCopyItems(DXTreeViewRibbonItemInfo[] dragItems) {
			if(Nodes.Count == 1 && Nodes[0].Nodes.Count == 0 && dragItems[0].Node == Nodes[0]) return false;
			return true;
		}
		bool allowGallery = true;
		public bool AllowGallery { get { return allowGallery; } set { allowGallery = value; } }
		void CopyItems(TreeNodeCollection nodes, BarItemLinkCollection links, TreeNode targetNode, DXTreeViewRibbonItemInfo[] dragItems, bool bMove, bool inButtonGroup) {
			if(!ShouldCopyItems(dragItems)) return;
			BeginUpdate();
			try {
				int insertAt = GetInsertPosition(targetNode, nodes, dragItems[0].Node);
				for(int itemIndex = 0; itemIndex < dragItems.Length; itemIndex++) {
					if(dragItems[itemIndex].IsGallery && !AllowGallery) continue;
					if(IsButtonGroup(targetNode) && dragItems[itemIndex].CanAddToButtonGroup) {
						CreateButtonGroupItemNode(targetNode, ButtonGroup(targetNode).ItemLinks, GetItem(dragItems[itemIndex].Item));
						if(bMove) RemoveAfterCopy(dragItems[itemIndex].Node);
						SynchronizeButtonGroups(Nodes, targetNode);
					}
					else {
						if(inButtonGroup && !dragItems[itemIndex].CanAddToButtonGroup) continue;
						TreeNode node = CreateItemNode(inButtonGroup, nodes, links, insertAt, GetItem(dragItems[itemIndex].Item));
						if(bMove) RemoveAfterCopy(dragItems[itemIndex].Node);
						SynchronizeButtonGroups(Nodes, node.Parent);
						insertAt = node.Index + 1;
					}
				}
			}
			finally { EndUpdate(); }
			ChangeComponents();
			RefreshObjects();
		}
		TreeNode CreateItemNode(bool inButtonGroup, TreeNodeCollection nodes, BarItemLinkCollection links, int insertAt, BarItem item) {
			BarItemLink link = links.Insert(insertAt, item);
			TreeNode node = CreateCopyTreeNode(link, ItemImageIndex(inButtonGroup, link));
			InsertCopyTreeNode(nodes, node, insertAt);
			if(IsButtonGroup(node)) CreateTree(node, ButtonGroup(node).ItemLinks);
			return node;
		}
		void CreateButtonGroupItemNode(TreeNode targetNode, BarItemLinkCollection groupLinks, BarItem item) {
			BarItemLink link = groupLinks.Add(item);
			TreeNode node = CreateCopyTreeNode(link, ItemImageIndex(true, link));
			InsertCopyTreeNode(targetNode.Nodes, node, targetNode.Nodes.Count);
		}
		[Browsable(false)]
		public object[] SelectedTreeItems {
			get {
				object[] selItems = new Object[SelNodes.Length];
				for(int itemIndex = 0; itemIndex < SelNodes.Length; itemIndex++) {
					if(Item(SelNodes[itemIndex]) != null) selItems[itemIndex] = Item(SelNodes[itemIndex]).Item;
					else if(ButtonItem(SelNodes[itemIndex]) != null) selItems[itemIndex] = ButtonItem(SelNodes[itemIndex]).Item;
					else selItems[itemIndex] = SelNodes[itemIndex].Tag;
				}
				return selItems;
			}
		}
		[Browsable(false)]
		public BarItemLink[] SelectedLinks {
			get {
				ArrayList list = new ArrayList();
				for(int itemIndex = 0; itemIndex < SelNodes.Length; itemIndex++) {
					BarItemLink link = Item(SelNodes[itemIndex]);
					if(link != null) list.Add(link);
				}
				return list.ToArray(typeof(BarItemLink)) as BarItemLink[];
			}
		}
		protected override void UpdateTreeNodeText(TreeNode node, object item) {
			if(node.Tag as BarItemLink != null && (node.Tag as BarItemLink).Item == item) node.Text = ItemCaption(item as BarItem);
		}
	}
	[ToolboxItem(false)]
	public class RibbonTreeView : ItemLinksTreeView {
		RibbonControl ribbon;
		DevExpress.XtraEditors.Designer.Utils.XtraPGFrame ownerFrame = null;
		public RibbonTreeView() : this(null) { }
		public RibbonTreeView(RibbonControl ribbon) {
			this.ribbon = ribbon;
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}
		protected string PageCaption(RibbonPage page) { return FullCaption(page.Text, page.Name); }
		protected string GroupCaption(RibbonPageGroup group) { return FullCaption(group.Text, group.Name); }
		protected string CategoryCaption(RibbonPageCategory category) { return FullCaption(category.Text, category.Name); }
		public DevExpress.XtraEditors.Designer.Utils.XtraPGFrame OwnerFrame { get { return ownerFrame; } set { ownerFrame = value; } }
		protected virtual TreeNode AppendNode(TreeNode parentNode, object tag, string caption, RibbonTreeImages imageIndex) {
			TreeNode node = new TreeNode(caption, (int)imageIndex, (int)imageIndex);
			node.Tag = tag;
			if(parentNode != null) parentNode.Nodes.Add(node);
			else Nodes.Add(node);
			return node;
		}
		protected virtual bool ShouldAddEntry(object entry) {
			return true;
		}
		public void AppendRibbonPageCategory(RibbonPageCategory pageCategory) {
			if(!ShouldAddEntry(pageCategory))
				return;
			BeginUpdate();
			try {
				TreeNode pageCategoryNode = AppendNode(null, GetRibbonPageCategoryTag(pageCategory), pageCategory.Text, CategoryTreeImage);
				ProcessTreeNodeCore(pageCategoryNode, pageCategory);
				for(int pageIndex = 0; pageIndex < pageCategory.Pages.Count; pageIndex++)
					AppendRibbonPage(pageCategoryNode, pageCategory.Pages[pageIndex]);
			}
			finally { EndUpdate(); }
		}
		protected virtual RibbonTreeImages CategoryTreeImage {
			get { return RibbonTreeImages.Category; }
		}
		protected virtual object GetRibbonPageCategoryTag(RibbonPageCategory pageCategory) {
			return pageCategory;
		}
		public void AppendRibbonPage(TreeNode pageCategoryNode, RibbonPage page) {
			if(!ShouldAddEntry(page))
				return;
			BeginUpdate();
			try {
				TreeNode pageNode = AppendNode(pageCategoryNode, GetRibbonPageTag(page), PageCaption(page), PageTreeImage);
				ProcessTreeNodeCore(pageNode, page);
				for(int groupIndex = 0; groupIndex < page.Groups.Count; groupIndex++)
					AppendRibbonGroup(pageNode, page.Groups[groupIndex]);
			}
			finally { EndUpdate(); }
		}
		protected virtual RibbonTreeImages PageTreeImage {
			get {
				return RibbonTreeImages.Page;
			}
		}
		protected virtual object GetRibbonPageTag(RibbonPage page) {
			return page;
		}
		public void AppendRibbonGroup(TreeNode pageNode, RibbonPageGroup group) {
			if(!ShouldAddEntry(group))
				return;
			BeginUpdate();
			try {
				TreeNode groupNode = AppendNode(pageNode, GetRibbonPageGroupTag(group), GroupCaption(group), GroupTreeImage);
				ProcessTreeNodeCore(groupNode, group);
				for(int itemIndex = 0; itemIndex < group.ItemLinks.Count; itemIndex++)
					AppendRibbonItem(groupNode, group.ItemLinks[itemIndex]);
			}
			finally { EndUpdate(); }
		}
		protected virtual RibbonTreeImages GroupTreeImage {
			get {
				return RibbonTreeImages.Group;
			}
		}
		protected virtual void ProcessTreeNodeCore(TreeNode node, object entry) {
		}
		protected virtual object GetRibbonPageGroupTag(RibbonPageGroup group) {
			return group;
		}
		public void AppendRibbonItem(TreeNode groupNode, BarItemLink link) {
			AppendRibbonItem(groupNode, link, false);
		}
		public void AppendRibbonItem(TreeNode groupNode, BarItemLink link, bool inButtonGroup) {
			if(!ShouldAddEntry(link))
				return;
			BeginUpdate();
			try {
				TreeNode itemNode = AppendNode(groupNode, GetRibbonItemTag(link), ItemCaption(link), ItemImageIndex(inButtonGroup, link));
				ProcessTreeNodeCore(itemNode, link);
				BarButtonGroupLink buttonLink = link as BarButtonGroupLink;
				if(buttonLink != null) {
					for(int itemIndex = 0; itemIndex < buttonLink.Item.ItemLinks.Count; itemIndex++) {
						BarItemLink subItemLink = buttonLink.Item.ItemLinks[itemIndex];
						TreeNode node = AppendNode(itemNode, GetRibbonItemTag(subItemLink), ItemCaption(subItemLink), ItemImageIndex(true, GetBarSubItemLink(link, subItemLink)));
						ProcessTreeNodeCore(node, subItemLink);
					}
				}
				AppendRibbonPopupItems(itemNode, link, inButtonGroup);
			}
			finally { EndUpdate(); }
		}
		public void AppendRibbonPopupItems(TreeNode itemNode, BarItemLink link, bool inButtonGroup) {
			if(!ShouldProcessRibbonPopupItems) return;
			BarItemLinkCollection popupLinks = CustomizationHelperBase.GetPopupItemLinks(Ribbon.OptionsCustomizationForm, link);
			if(popupLinks == null)
				return;
			foreach(BarItemLink popupLink in popupLinks) {
				TreeNode popupLinkNode = AppendNode(itemNode, GetRibbonItemTag(popupLink), ItemCaption(popupLink), ItemImageIndex(inButtonGroup, popupLink));
				ProcessTreeNodeCore(popupLinkNode, popupLink);
			}
		}
		protected virtual bool ShouldProcessRibbonPopupItems { get { return false; } }
		protected virtual object GetRibbonItemTag(BarItemLink link) {
			return link;
		}
		protected virtual BarItemLink GetBarSubItemLink(BarItemLink link, BarItemLink subItemLink) {
			return link;
		}
		public override void CreateTree() {
			ClearTree();
			OnCreateTreeStarted();
			BeginUpdate();
			try {
				AppendRibbonPageCategory(Ribbon.PageCategories.DefaultCategory);
				for(int pageCategoryIndex = 0; pageCategoryIndex < Ribbon.PageCategories.Count; pageCategoryIndex++) {
					AppendRibbonPageCategory(Ribbon.PageCategories[pageCategoryIndex]);
				}
			}
			finally {
				EndUpdate();
			}
			OnCreateTreeCompleted();
		}
		protected virtual void OnCreateTreeStarted() { }
		protected virtual void OnCreateTreeCompleted() { }
		public RibbonPageCategory Category(TreeNode node) {
			if(node == null) return null;
			return node.Tag as RibbonPageCategory;
		}
		public RibbonPage Page(TreeNode node) {
			if(node == null) return null;
			return node.Tag as RibbonPage;
		}
		public RibbonPageGroup Group(TreeNode node) {
			if(node == null) return null;
			return node.Tag as RibbonPageGroup;
		}
		RibbonPage SwapPages(RibbonPageCategory category, RibbonPage page1, RibbonPage page2) {
			int index1 = category.Pages.IndexOf(page1);
			category.Pages.Remove(page2);
			category.Pages.Insert(index1, page2);
			return page2;
		}
		protected override TreeNode SwapNearNodes(TreeNode node1, TreeNode node2) {
			if(Category(node1) == Ribbon.PageCategories.DefaultCategory || Category(node2) == Ribbon.PageCategories.DefaultCategory) return node2;
			return base.SwapNearNodes(node1, node2);
		}
		RibbonPageCategory SwapCategories(RibbonPageCategory category1, RibbonPageCategory category2) {
			if(category1 == Ribbon.PageCategories.DefaultCategory || category2 == Ribbon.PageCategories.DefaultCategory) return category2;
			int index1 = Ribbon.PageCategories.IndexOf(category1);
			Ribbon.PageCategories.Remove(category2);
			Ribbon.PageCategories.Insert(index1, category2);
			return category2;
		}
		RibbonPageGroup SwapGroups(RibbonPage page, RibbonPageGroup group1, RibbonPageGroup group2) {
			int index1 = page.Groups.IndexOf(group1);
			page.Groups.Remove(group2);
			page.Groups.Insert(index1, group2);
			return group2;
		}
		BarItemLink SwapItems(RibbonPageGroup group, BarItemLink link1, BarItemLink link2) {
			return SwapItems(group.ItemLinks, link1, link2);
		}
		BarItemLink SwapButtonItems(BarItemLink link, BarItemLink link1, BarItemLink link2) {
			BarButtonGroup group = link.Item as BarButtonGroup;
			return SwapItems(group.ItemLinks, link1, link2);
		}
		protected override object SwapObjects(TreeNode node1, TreeNode node2) {
			if(Category(node1) != null) return SwapCategories(Category(node1), Category(node2));
			else if(Page(node1) != null) return SwapPages(Category(node1.Parent), Page(node1), Page(node2));
			else if(Group(node1) != null) return SwapGroups(Page(node1.Parent), Group(node1), Group(node2));
			else if(Item(node1) != null) return SwapItems(Group(node1.Parent), Item(node1), Item(node2));
			else return SwapButtonItems(Item(node1.Parent), ButtonItem(node1), ButtonItem(node2));
		}
		bool RemovePageCategory(TreeNode node) {
			if(Category(node) == Ribbon.PageCategories.DefaultCategory) return false;
			Ribbon.PageCategories.Remove(Category(node));
			Ribbon.Container.Remove(Category(node));
			for(int pageIndex = 0; pageIndex < node.Nodes.Count; pageIndex++) {
				RemovePage(node.Nodes[pageIndex]);
			}
			return true;
		}
		void RemovePage(TreeNode node) {
			RibbonPage page = Page(node);
			if(page == null) return;
			page.Collection.Remove(page);
			Ribbon.Container.Remove(Page(node));
			for(int groupIndex = 0; groupIndex < node.Nodes.Count; groupIndex++) {
				RemoveGroup(node.Nodes[groupIndex]);
			}
		}
		void RemoveGroup(TreeNode node) {
			Page(node.Parent).Groups.Remove(Group(node));
			Ribbon.Container.Remove(Group(node));
		}
		void RemoveItem(TreeNode node) {
			Group(node.Parent).ItemLinks.Remove(Item(node));
			ComponentChangeService.OnComponentChanged(node.Parent, null, null, null);
		}
		void RemoveButtonItem(TreeNode node) { (Item(node.Parent).Item as BarButtonGroup).ItemLinks.Remove(ButtonItem(node)); }
		protected override bool RemoveObject(TreeNode node) {
			if(Category(node) != null) return RemovePageCategory(node);
			else if(Page(node) != null) RemovePage(node);
			else if(Group(node) != null) RemoveGroup(node);
			else if(Item(node) != null) RemoveItem(node);
			else RemoveButtonItem(node);
			return true;
		}
		protected override RibbonTreeSelectionMode GetSelectionMode(TreeNode node) {
			if(Category(node) != null) return RibbonTreeSelectionMode.PageCategory;
			if(Page(node) != null) return RibbonTreeSelectionMode.Page;
			if(Group(node) != null) return RibbonTreeSelectionMode.Group;
			if(Item(node) != null) return RibbonTreeSelectionMode.Item;
			if(ButtonItem(node) != null) return RibbonTreeSelectionMode.ButtonGroupItem;
			return RibbonTreeSelectionMode.None;
		}
		TreeNode CreateNode(object tag, string text, int imageIndex) {
			TreeNode node = new TreeNode(text, imageIndex, imageIndex);
			node.Tag = tag;
			return node;
		}
		public TreeNode AddRibbonPageCategory() {
			BeginUpdate();
			try {
				RibbonPageCategory pageCategory = DesignerHost.CreateComponent(Ribbon.DefaultPageCategoryType) as RibbonPageCategory;
				if(pageCategory == null)
					pageCategory = new RibbonPageCategory();
				Ribbon.Container.Add(pageCategory);
				pageCategory.Text = pageCategory.Name;
				Ribbon.PageCategories.Add(pageCategory);
				Ribbon.Container.Add(pageCategory);
				ComponentChangeService.OnComponentChanged(Ribbon, null, null, null);
				Nodes.Add(CreateNode(pageCategory, pageCategory.Text, (int)RibbonTreeImages.Category));
			}
			finally { EndUpdate(); }
			return Nodes[Nodes.Count - 1];
		}
		public TreeNode AddRibbonPage(TreeNode categoryNode) {
			RibbonPage page = DesignerHost.CreateComponent(Ribbon.DefaultPageType) as RibbonPage;
			if(page == null)
				page = new RibbonPage();
			Ribbon.Container.Add(page);
			page.Text = page.Name;
			if(Category(categoryNode) == null) {
				Ribbon.Pages.Add(page);
				categoryNode = Nodes[0];
			}
			else Category(categoryNode).Pages.Add(page);
			TreeNode node = CreateNode(page, PageCaption(page), (int)RibbonTreeImages.Page);
			categoryNode.Nodes.Add(node);
			ComponentChangeService.OnComponentChanged(Ribbon, null, null, null);
			categoryNode.Expand();
			RefreshObjects();
			return categoryNode.Nodes[categoryNode.Nodes.Count - 1];
		}
		public TreeNode AddRibbonPageGroup(TreeNode page) {
			RibbonPageGroup group = DesignerHost.CreateComponent(Ribbon.DefaultPageGroupType) as RibbonPageGroup;
			if(group == null)	
				group = new RibbonPageGroup();
			Ribbon.Container.Add(group);
			group.Text = group.Name;
			Page(page).Groups.Add(group);
			ComponentChangeService.OnComponentChanged(Page(page), null, null, null);
			page.Nodes.Add(CreateNode(group, GroupCaption(group), (int)RibbonTreeImages.Group));
			page.Expand();
			RefreshObjects();
			return page.Nodes[page.Nodes.Count - 1];
		}
		public TreeNode AddRibbonPage() {
			if(SelectedNode == null) return null;
			if(Category(SelectedNode) != null) return AddRibbonPage(SelectedNode);
			if(Category(SelectedNode.Parent) != null) return AddRibbonPage(SelectedNode.Parent);
			return null;
		}
		public TreeNode AddRibbonPageGroup() {
			if(SelectedNode == null) return null;
			if(Page(SelectedNode) != null) return AddRibbonPageGroup(SelectedNode);
			else if(Page(SelectedNode.Parent) != null) return AddRibbonPageGroup(SelectedNode.Parent);
			return null;
		}
		BarItem GetItem(object item) {
			if(item as BarItem != null) return item as BarItem;
			if(item as BarItemLink != null) return (item as BarItemLink).Item;
			return null;
		}
		ItemsAdditionMode GetAdditionMode(TreeNode targetNode, TreeNode sourceNode) {
			if(sourceNode == null) return ItemsAdditionMode.AddBeforeTarget;
			if(sourceNode.Parent == targetNode.Parent && sourceNode.Index < targetNode.Index) return ItemsAdditionMode.AddAfterTarget;
			return ItemsAdditionMode.AddBeforeTarget;
		}
		ItemsAdditionMode GetAdditionMode(TreeNode targetNode, int insertAt) {
			if(insertAt == targetNode.Index) return ItemsAdditionMode.AddBeforeTarget;
			return ItemsAdditionMode.AddAfterTarget;
		}
		ItemsAdditionMode CopyRibbonItem(BarItemLinkCollection links, TreeNodeCollection nodes, int insertAt, DXTreeViewRibbonItemInfo info, bool inButtonGroup) {
			BarItemLink link = links.Insert(insertAt, GetItem(info.Item));
			nodes.Insert(insertAt, CreateNode(link, ItemCaption(GetItem(info.Item)), (int)ItemImageIndex(inButtonGroup, link)));
			ComponentChangeService.OnComponentChanged(nodes[0].Parent.Tag, null, null, null);
			if(info.IsBarButtonGroup) {
				for(int i = 0; i < GetButtonGroup(info.Item).ItemLinks.Count; i++) {
					this.AppendRibbonItem(nodes[insertAt], GetButtonGroup(info.Item).ItemLinks[i], true);
				}
			}
			nodes[0].Parent.Expand();
			return ItemsAdditionMode.Add;
		}
		ItemsAdditionMode CopyRibbonItem(TreeNode targetNode, TreeNode currNode, DXTreeViewRibbonItemInfo info, ItemsAdditionMode dirMode) {
			if(targetNode == null) return ItemsAdditionMode.CantAdd;
			if(Group(targetNode) != null) return CopyRibbonItem(Group(targetNode).ItemLinks, targetNode.Nodes, targetNode.Nodes.Count, info, false);
			else if(Item(targetNode) != null) {
				if(Item(targetNode) as BarButtonGroupLink != null) {
					if(info.IsItem && info.CanAddToButtonGroup) return CopyRibbonItem(ButtonGroup(targetNode).ItemLinks, targetNode.Nodes, targetNode.Nodes.Count, info, true);
				}
				else {
					int insertAt = GetInsertPosition(currNode, dirMode);
					CopyRibbonItem(Group(targetNode.Parent).ItemLinks, targetNode.Parent.Nodes, insertAt, info, false);
					return dirMode;
				}
			}
			else if(ButtonItem(targetNode) != null && info.CanAddToButtonGroup) {
				int insertAt = GetInsertPosition(currNode, dirMode);
				CopyRibbonItem(ButtonGroup(targetNode.Parent).ItemLinks, targetNode.Parent.Nodes, insertAt, info, true);
				return dirMode;
			}
			return ItemsAdditionMode.CantAdd;
		}
		ItemsAdditionMode MoveRibbonPageGroup(TreeNode targetNode, TreeNode currNode, DXTreeViewRibbonItemInfo info, ItemsAdditionMode dirMode) {
			TreeNode pageNode;
			ItemsAdditionMode addMode;
			int insertAt;
			if(targetNode == null) return ItemsAdditionMode.CantAdd;
			else if(Page(targetNode) != null) {
				pageNode = targetNode;
				insertAt = targetNode.Nodes.Count;
				addMode = ItemsAdditionMode.Add;
			}
			else if(Page(targetNode.Parent) != null) {
				pageNode = targetNode.Parent;
				addMode = dirMode;
				insertAt = GetInsertPosition(currNode, dirMode);
			}
			else return ItemsAdditionMode.CantAdd;
			RibbonPageGroup group = info.Item as RibbonPageGroup;
			if(group.Page == Page(pageNode)) {
				insertAt = Math.Max(0, insertAt - (group.Page.Groups.IndexOf(group) <= insertAt ? 1 : 0));
			}
			group.Page.Groups.Remove(group);
			Page(pageNode).Groups.Insert(insertAt, group);
			info.Node.Remove();
			pageNode.Nodes.Insert(insertAt, info.Node);
			return addMode;
		}
		ItemsAdditionMode MoveRibbonPage(TreeNode targetNode, TreeNode currNode, DXTreeViewRibbonItemInfo info, ItemsAdditionMode dirMode) {
			TreeNode categoryNode;
			ItemsAdditionMode addMode;
			int insertAt;
			if(targetNode == null) return ItemsAdditionMode.CantAdd;
			else if(Category(targetNode) != null) {
				categoryNode = targetNode;
				insertAt = targetNode.Nodes.Count;
				addMode = ItemsAdditionMode.Add;
			}
			else if(Category(targetNode.Parent) != null) {
				categoryNode = targetNode.Parent;
				addMode = dirMode;
				insertAt = GetInsertPosition(currNode, dirMode);
			}
			else return ItemsAdditionMode.CantAdd;
			RibbonPage page = info.Item as RibbonPage;
			if(page.Category == Category(categoryNode)) {
				insertAt = Math.Max(0, insertAt - (page.Category.Pages.IndexOf(page) <= insertAt ? 1 : 0));
			}
			page.Category.Pages.Remove(page);
			Category(categoryNode).Pages.Insert(insertAt, page);
			info.Node.Remove();
			categoryNode.Nodes.Insert(insertAt, info.Node);
			return dirMode;
		}
		ItemsAdditionMode MoveRibbonPageCategory(TreeNode targetNode, TreeNode currNode, DXTreeViewRibbonItemInfo info, ItemsAdditionMode dirMode) {
			if(Category(targetNode) == null) return ItemsAdditionMode.CantAdd;
			int insertAt = GetInsertPosition(currNode, dirMode); 
			RibbonPageCategory category = info.Item as RibbonPageCategory;
			insertAt = Math.Max(0, insertAt - Ribbon.PageCategories.IndexOf(category) - 1 <= insertAt ? 1 : 0);
			Ribbon.PageCategories.Remove(category);
			Ribbon.PageCategories.Insert(insertAt - 1, category);
			info.Node.Remove();
			Nodes.Insert(insertAt, info.Node);
			return dirMode;
		}
		ItemsAdditionMode CopyItem(TreeNode targetNode, TreeNode currNode, DXTreeViewRibbonItemInfo info, ItemsAdditionMode dirMode) {
			if(info.Item as RibbonPageCategory != null) return MoveRibbonPageCategory(targetNode, currNode, info, dirMode);
			else if(info.Item as RibbonPage != null) return MoveRibbonPage(targetNode, currNode, info, dirMode);
			else if(info.Item as RibbonPageGroup != null) return MoveRibbonPageGroup(targetNode, currNode, info, dirMode);
			else if(info.Item as BarItem != null || info.Item as BarItemLink != null) return CopyRibbonItem(targetNode, currNode, info, dirMode);
			return ItemsAdditionMode.CantAdd;
		}
		void CopyItems(TreeNode targetNode, DXTreeViewRibbonItemInfo[] dragItems, bool bMove) {
			TreeNode currNode = targetNode;
			ItemsAdditionMode dirMode = GetAdditionMode(targetNode, dragItems[0].Node);
			for(int itemIndex = 0; itemIndex < dragItems.Length; itemIndex++) {
				ItemsAdditionMode addMode = CopyItem(targetNode, currNode, dragItems[itemIndex], dirMode);
				if(addMode == ItemsAdditionMode.CantAdd) continue;
				if(addMode == ItemsAdditionMode.AddAfterTarget) currNode = currNode.NextNode;
				if(bMove) RemoveAfterCopy(dragItems[itemIndex].Node);
				else {
					if(ButtonGroup(targetNode) != null) SynchronizeButtonGroups(targetNode);
					else if(ButtonGroup(targetNode.Parent) != null) SynchronizeButtonGroups(targetNode.Parent);
				}
			}
		}
		protected override RibbonTreeImages ItemImageIndex(TreeNode node) {
			if(Category(node) != null) return RibbonTreeImages.Category;
			if(Page(node) != null) return RibbonTreeImages.Page;
			if(Group(node) != null) return RibbonTreeImages.Group;
			if(Item(node) != null) return Item(node).BeginGroup ? RibbonTreeImages.ItemBeginGroup : RibbonTreeImages.Item;
			return RibbonTreeImages.ButtonItem;
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
		}
		protected override void UpdateTreeNodeText(TreeNode node, object item) {
			base.UpdateTreeNodeText(node, item);
			if(node.Tag != item) return;
			if(node.Tag as RibbonPageGroup != null) node.Text = GroupCaption(item as RibbonPageGroup);
			else if(node.Tag as RibbonPage != null) node.Text = PageCaption(item as RibbonPage);
		}
		#region Drag and Drop
		public override void OnDragEnter(object sender, DragEventArgs e) {
			SetDragDropEffect(e);
		}
		protected override bool CanDropGallery { get { return true; } }
		protected override void OnDragDrop(object sender, DragEventArgs e) {
			ResetNodeImage(DropTargetNode);
			RibbonItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			CopyItems(DropTargetNode, dragInfo.Items as DXTreeViewRibbonItemInfo[], e.Effect == DragDropEffects.Move);
			RefreshObjects();
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class RunTimeRibbonTreeView : RibbonTreeView {
		public RunTimeRibbonTreeView() {
			this.DrawMode = GetDrawModeOption();
		}
		protected override bool UseThemes {
			get { return true; }
		}
		public void RefreshNodeState() {
			RefreshCheckBoxesCore();
		}
		#region Overrides
		protected override string FullCaption(string text, string name) {
			return text;
		}
		protected override string ItemCaption(BarItemLink link) {
			return FullCaption(link.Caption, link.Item.Name);
		}
		protected override void ChangeComponents() { }
		protected virtual void RefreshState() {
			RefreshObjects();
			ChangeComponents();
			SynchronizeButtonGroups(SelectedNode.Parent);
		}
		protected override RibbonTreeImages ItemImageIndex(TreeNode node) {
			const int noimage = -1;
			return (RibbonTreeImages)noimage;
		}		
		protected override BarItemLink GetBarSubItemLink(BarItemLink link, BarItemLink subItemLink) {
			return subItemLink;
		}
		protected override bool ShouldAddEntry(object entry) {
			RibbonPageGroup group = entry as RibbonPageGroup;
			if(group == null)
				return base.ShouldAddEntry(entry);
			return group.Visible;
		}
		protected override void OnCreateTreeCompleted() {
			RefreshCheckBoxesCore();
		}
		protected override bool ShouldProcessRibbonPopupItems {
			get { return true; }
		}
		protected override void OnAfterCheck(TreeViewEventArgs e) {
			base.OnAfterCheck(e);
			RefreshSubTreeCheckState(e);
		}
		protected override void ProcessTreeNodeCore(TreeNode node, object entry) {
			SetNodeCheckState(node);
			ProcessTreeNodeTextCore(node);
		}
		protected virtual void ProcessTreeNodeTextCore(TreeNode node) {
			if(!CustomizationHelperBase.ShouldProcessSuffix(node))
				return;
			node.Text = CustomizationHelperBase.AddInternalSuffixToText(node.Text);
		}
		protected virtual void SetNodeCheckState(TreeNode node) {
			if(!ShouldProcessCheckChange(node))
				return;
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(node);
			node.Checked = info.IsVisible;
		}
		protected virtual void RefreshCheckBoxesCore() {
			TreeViewUtils.ProcessTreeNodes(this, TreeNodeCheckStateHandler);
		}
		protected virtual void TreeNodeCheckStateHandler(TreeNode node) {
			if(!ShouldHideNodeCheck(node))
				return;
			TreeViewUtils.HideCheckBox(this, node);
		}
		protected virtual bool ShouldHideNodeCheck(TreeNode node) {
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(node);
			return info.ItemType != NodeType.Category && info.ItemType != NodeType.Page;
		}
		bool checkUpdating = false;
		protected virtual void RefreshSubTreeCheckState(TreeViewEventArgs e) {
			if(e.Action == TreeViewAction.Unknown)
				return;
			if(!ShouldProcessCheckChange(e.Node))
				return;
			if(checkUpdating)
				return;
			try {
				checkUpdating = true;
				if(CustomizationHelperBase.FromNode(e.Node).ItemType == NodeType.Category)
					RefreshCategorySubTreeCheckStateCore(e.Node);
				else RefreshPageParentCheckStateCore(e.Node);
			}
			finally {
				checkUpdating = false;
			}
		}
		protected virtual void RefreshCategorySubTreeCheckStateCore(TreeNode node) {
			foreach(TreeNode childNode in node.Nodes)
				childNode.Checked = node.Checked;
		}
		protected virtual void RefreshPageParentCheckStateCore(TreeNode node) {
			TreeNode categoryNode = node.Parent;
			bool? childState = GetDirectChildsState(categoryNode);
			categoryNode.Checked = childState ?? true;
		}
		protected virtual bool ShouldProcessCheckChange(TreeNode node) {
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(node);
			return info.ItemType == NodeType.Category || info.ItemType == NodeType.Page;
		}
		protected bool? GetDirectChildsState(TreeNode node) {
			int res = 0;
			foreach(TreeNode childNode in node.Nodes)
				res += childNode.Checked ? 1 : 0;
			if(res == 0) return false;
			if(res == node.Nodes.Count) return true;
			return null;
		}
		#endregion
		#region Moving
		protected override TreeNode SwapNearNodes(TreeNode node1, TreeNode node2) {
			SwapNodes(GetParentNodes(node1), node1, node2);
			return node1;
		}
		public virtual void MoveUpCrossParent() {
			if(SelectedNode == null)
				return;
			if(SelectedNode.PrevNode != null) {
				MoveUp();
				return;
			}
			MoveUpCrossParentCore();
		}
		public virtual void MoveDownCrossParent() {
			if(SelectedNode == null)
				return;
			if(SelectedNode.NextNode != null) {
				MoveDown();
				return;
			}
			MoveDownCrossParentCore();
		}
		public virtual void MoveUpCrossParentCore() {
			if(SelectedNode.Parent == null || SelectedNode.Parent.PrevNode == null)
				return;
			TreeNode node = SelectedNode;
			TreeNode parentNode = node.Parent;
			parentNode.Nodes.Remove(node);
			parentNode.PrevNode.Nodes.Add(node);
			parentNode.PrevNode.Expand();
			SelectedNode = node;
			RefreshState();
		}
		public virtual void MoveDownCrossParentCore() {
			if(SelectedNode.Parent == null || SelectedNode.Parent.NextNode == null)
				return;
			TreeNode node = SelectedNode;
			TreeNode parentNode = node.Parent;
			parentNode.Nodes.Remove(node);
			parentNode.NextNode.Nodes.Insert(0, node);
			parentNode.NextNode.Expand();
			SelectedNode = node;
			RefreshState();
		}
		#endregion
		#region Tags
		protected override object GetRibbonPageCategoryTag(RibbonPageCategory pageCategory) {
			IdInfo idSourceInfo = GetSourceIdInfo(pageCategory);
			return new RibbonPageCategoryInfo(pageCategory, IdGenerator.Instance.Generate(Ribbon), idSourceInfo, Ribbon) { IsVisible = pageCategory.Visible };
		}
		protected override object GetRibbonPageTag(RibbonPage page) {
			IdInfo idSourceInfo = GetSourceIdInfo(page);
			return new RibbonPageInfo(page, IdGenerator.Instance.Generate(Ribbon), idSourceInfo, Ribbon) { IsVisible = page.Visible };
		}
		protected override object GetRibbonPageGroupTag(RibbonPageGroup group) {
			IdInfo idSourceInfo = GetSourceIdInfo(group);
			return new RibbonGroupInfo(group, IdGenerator.Instance.Generate(Ribbon), idSourceInfo, Ribbon);
		}
		protected override object GetRibbonItemTag(BarItemLink link) {
			IdInfo idSourceInfo = GetSourceIdInfo(link);
			RibbonItemLinkInfo res = new RibbonItemLinkInfo(link, IdGenerator.Instance.Generate(Ribbon), idSourceInfo, Ribbon);
			if(idSourceInfo.IsEmpty) {
				res.ItemSourceIdInfo = GetItemSourceInfo(link);
				System.Diagnostics.Debug.Assert(res.ItemSourceIdInfo != null, "Can't find ItemSourceIdInfo");
			}
			return res;
		}
		protected virtual IdInfo GetItemSourceInfo(BarItemLink link) {
			BarItem item = link.Item;
			foreach(BarItemLink parentLink in item.Links) {
				IdInfo res = GetSourceIdInfo(parentLink);
				if(!res.IsEmpty) return res;
			}
			return null;
		}
		protected virtual IdInfo GetSourceIdInfo(object entry) {
			IdLinkTable linkTable = RibbonSourceStateInfo.Instance.GetLinkTable(Ribbon);
			IdInfo res = linkTable.GetId(entry);
			if(res == null)
				res = IdInfo.Empty;
			return res;
		}
		#endregion
		#region Drag and Drop
		protected override RibbonItemsDragInfo CreateRibbonItemsDragInfo(TreeNode[] SelNodes) {
			TreeNode[] nodes = new TreeNode[] { SelectedNode };
			return new RunTimeRibbonItemsDragInfo(this, nodes);
		}
		protected override RibbonItemsDragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(RunTimeRibbonItemsDragInfo)) as RunTimeRibbonItemsDragInfo;
		}
		public override void ResetNodeImage(TreeNode node) { }
		protected override TreeNode[] DragDropSelNodes {
			get { return new TreeNode[] { SelectedNode }; }
		}
		protected override bool CanDropGallery { get { return true; } }
		public override void OnDragEnter(object sender, DragEventArgs e) {
			SetDragDropEffect(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			RunTimeRibbonItemsDragInfo dragInfo = GetDragObject(e.Data) as RunTimeRibbonItemsDragInfo;
			if(dragInfo == null)
				return;
			dragInfo.ProcessDragDrop(DropTargetNode);
		}
		protected override void OnDragOver(DragEventArgs e) {
			RibbonItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null)
				return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			if(node == DropTargetNode && node != null)
				return;
			DropTargetNode = node;
			SetDragDropEffect(e);
			if(node == DropSelectedNode) {
				e.Effect = DragDropEffects.None;
				return;
			}
			ItemsAdditionMode mode = dragInfo.CanAddItems(node, RibbonTreeSelectionMode.Unused);
			if(mode == ItemsAdditionMode.None) {
				e.Effect = DragDropEffects.None;
				return;
			}
			e.Effect = mode == ItemsAdditionMode.Move ? DragDropEffects.Move : DragDropEffects.Copy;
		}
		#endregion
	}
	public class RunTimeRibbonTreeViewOriginalView : RunTimeRibbonTreeView {
		public RunTimeRibbonTreeViewOriginalView() : this(false) {
		}
		public RunTimeRibbonTreeViewOriginalView(bool isCustomizationMode) {
			this.ViewType = TreeViewType.Default;
			this.IsCustomizationMode = isCustomizationMode;
		}
		public TreeViewType ViewType { get; set; }
		public bool IsCustomizationMode { get; set; }
		#region Overloads
		protected override bool ShouldAddEntry(object entry) {
			IdLinkTable linkTable = RibbonSourceStateInfo.Instance.GetLinkTable(Ribbon);
			return linkTable.GetId(entry) != null;
		}
		protected override void ProcessTreeNodeCore(TreeNode node, object entry) {
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(node);
			info.Alias = GetEntrySourceName(info);
			node.Text = info.Alias;
			ApplyOriginalSettings(node, entry);
		}
		protected virtual void ApplyOriginalSettings(TreeNode node, object entry) {
			if(!ShouldApplyOriginalSettings)
				return;
			IdLinkTable table = RibbonSourceStateInfo.Instance.GetLinkTable(Ribbon);
			IdInfo id = table.GetId(entry);
			if(id == null)
				return;
			RibbonElementSettings settings = table.GetOriginalSettings(id);
			node.Checked = settings.IsVisible;
		}
		protected virtual bool ShouldApplyOriginalSettings {
			get { return IsCustomizationMode; }
		}
		protected override TreeNode AppendNode(TreeNode parentNode, object tag, string caption, RibbonTreeImages imageIndex) {
			if(ViewType == TreeViewType.Default) 
				return base.AppendNode(parentNode, tag, caption, imageIndex);
			return AppendCommandNodeCore(parentNode, tag, caption, imageIndex);
		}
		protected override void OnCreateTreeStarted() {
			if(ViewType != TreeViewType.Commands)
				return;
			CommandNodes.Clear();
		}
		protected override void OnCreateTreeCompleted() {
			if(ViewType == TreeViewType.Default)
				CreateTreeCompletedDefault();
			else CreateTreeCompletedCommands();
		}
		protected override void RefreshSubTreeCheckState(TreeViewEventArgs e) { }
		#endregion
		#region Default View Initialization
		protected virtual void CreateTreeCompletedDefault() {
			RestoreParentChildLinks(Nodes);
			SortTreeNodesDefault();
		}
		protected virtual void RestoreParentChildLinks(TreeNodeCollection nodes) {
			List<NodeReorderInfo> list = new List<NodeReorderInfo>();
			RestoreParentChildLinksCore(nodes, list);
			foreach(NodeReorderInfo info in list) {
				info.Source.Nodes.Remove(info.Node);
				info.Target.Nodes.Add(info.Node);
			}
		}
		protected virtual void RestoreParentChildLinksCore(TreeNodeCollection nodes, List<NodeReorderInfo> list) {
			foreach(TreeNode node in nodes) {
				RestoreParentChildLinksCore(node.Nodes, list);
				RestoreParentChildLinkCore(node, list);
			}
		}
		protected virtual void RestoreParentChildLinkCore(TreeNode node, List<NodeReorderInfo> list) {
			if(CustomizationHelperBase.FromNode(node).ItemType != NodeType.Group)
				return;
			TreeNode parent = node.Parent;
			TreeNode sourceParent = GetSourceParentNode(node);
			if(object.ReferenceEquals(parent, sourceParent))
				return;
			NodeReorderInfo info = new NodeReorderInfo(node, parent, sourceParent);
			list.Add(info);
		}
		protected virtual TreeNode GetSourceParentNode(TreeNode node) {
			CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(node);
			IdLinkTable table = RibbonSourceStateInfo.Instance.GetLinkTable(Ribbon);
			IdInfo parentIdInfo = table.GetParentIdInfo(info.SourceIdInfo);
			return CustomizationHelperBase.FindTreeNodeCore(Nodes, parentIdInfo);
		}
		protected virtual void SortTreeNodesDefault() {
			TreeViewUtils.ProcessTreeNodes(this, SortTreeNodeDefaultCore);
		}
		protected virtual void SortTreeNodeDefaultCore(TreeNode node) {
			if(node.Nodes.Count == 0)
				return;
			List<TreeNode> list = new List<TreeNode>();
			foreach(TreeNode currNode in node.Nodes) list.Add(currNode);
			list.Sort(DefaultNodesComparer.Instance);
			node.Nodes.Clear();
			foreach(TreeNode currNode in list) node.Nodes.Add(currNode);
		}
		#endregion
		#region Commands View Initialization
		List<TreeNode> CommandNodes = new List<TreeNode>();
		protected virtual void CreateTreeCompletedCommands() {
			PrepareBarButtonGroupChildsInfo();
			CommandNodes.Sort(CommandNodesComparer.Instance);
			BeginUpdate();
			try {
				foreach(TreeNode node in CommandNodes) {
					if(!IsBarButtonGroupChild(node)) Nodes.Add(node);
				}
			}
			finally {
				EndUpdate();
			}
		}
		List<BarItemLink> BarButtonGroupChildNodes = new List<BarItemLink>();
		protected virtual void PrepareBarButtonGroupChildsInfo() {
			BarButtonGroupChildNodes.Clear();
			foreach(TreeNode node in CommandNodes) {
				BarButtonGroupLink groupLink = ((RibbonItemLinkInfo)node.Tag).ItemLink as BarButtonGroupLink;
				if(groupLink != null) PrepareBarButtonGroupChildsInfoCore(groupLink);
			}
		}
		protected virtual void PrepareBarButtonGroupChildsInfoCore(BarButtonGroupLink groupLink) {
			foreach(BarItemLink link in groupLink.Item.ItemLinks) {
				BarButtonGroupChildNodes.Add(link);
			}
		}
		protected virtual bool IsBarButtonGroupChild(TreeNode node) {
			RibbonItemLinkInfo linkInfo = node.Tag as RibbonItemLinkInfo;
			return BarButtonGroupChildNodes.Contains(linkInfo.ItemLink);
		}
		#endregion
		#region Helpers
		protected virtual string GetEntrySourceName(IIdInfoContainer info) {
			IdLinkTable linkTable = RibbonSourceStateInfo.Instance.GetLinkTable(Ribbon);
			return linkTable.GetEntryName(info.SourceIdInfo);
		}
		protected virtual TreeNode AppendCommandNodeCore(TreeNode parentNode, object tag, string caption, RibbonTreeImages imageIndex) {
			TreeNode node = new TreeNode(caption, (int)imageIndex, (int)imageIndex) { Tag = tag };
			if(ShouldAddNodeToCommandList(node)) CommandNodes.Add(node);
			return node;
		}
		protected virtual bool ShouldAddNodeToCommandList(TreeNode node) {
			RibbonItemLinkInfo linkInfo = node.Tag as RibbonItemLinkInfo;
			if(linkInfo == null) return false;
			return !IsDuplicateItemLink(linkInfo);
		}
		protected virtual bool IsDuplicateItemLink(RibbonItemLinkInfo linkInfo) {
			BarItem item = linkInfo.ItemLink.Item;
			return CommandNodes.Exists(node => {
				RibbonItemLinkInfo info = node.Tag as RibbonItemLinkInfo;
				return object.ReferenceEquals(info.ItemLink.Item, item);
			});
		}
		#endregion
		#region Data
		public enum TreeViewType { Default,  Commands }
		public class NodeReorderInfo {
			public NodeReorderInfo(TreeNode node, TreeNode source, TreeNode target) {
				this.Node = node;
				this.Source = source;
				this.Target = target;
			}
			public TreeNode Node { get; private set; }
			public TreeNode Source { get; private set; }
			public TreeNode Target { get; private set; }
		}
		#endregion
		#region Comparers
		public class DefaultNodesComparer : IComparer<TreeNode> {
			static DefaultNodesComparer() {
				Instance = new DefaultNodesComparer();
			}
			public int Compare(TreeNode left, TreeNode right) {
				var leftInfo = CustomizationHelperBase.FromNode(left);
				var rightInfo = CustomizationHelperBase.FromNode(right);
				return leftInfo.SourceIdInfo.Id.CompareTo(rightInfo.SourceIdInfo.Id);
			}
			public static DefaultNodesComparer Instance { get; private set; }
		}
		public class CommandNodesComparer : IComparer<TreeNode> {
			static CommandNodesComparer() {
				Instance = new CommandNodesComparer();
			}
			public int Compare(TreeNode left, TreeNode right) {
				return left.Text.CompareTo(right.Text);
			}
			public static CommandNodesComparer Instance { get; set; }
		}
		#endregion
		#region Drag and Drop
		protected override void OnDragOver(DragEventArgs e) {
			e.Effect = DragDropEffects.None;
		}
		protected override void OnDragDrop(DragEventArgs e) {
		}
		#endregion
	}
	#region Drag and Drop Info
	class RunTimeRibbonItemsDragInfo : RibbonItemsDragInfo {
		public RunTimeRibbonItemsDragInfo(RunTimeRibbonTreeView tree, TreeNode[] nodes) : base(nodes) {
			this.RibbonTreeView = tree;
			this.CanDropGallery = true;
			this.DragDropHelper = null;
		}
		public override ItemsAdditionMode CanAddItems(TreeNode targetNode, RibbonTreeSelectionMode mode) {
			DragDropHelper = CreateDragDropHelper(IsLocalDrapDrop ? DragDropSupportBase.DragDropAction.Move : DragDropSupportBase.DragDropAction.Copy);
			if(!DragDropHelper.ShouldProcessDragDrop(SourceNode, targetNode))
				return ItemsAdditionMode.None;
			return IsLocalDrapDrop ? ItemsAdditionMode.Move : ItemsAdditionMode.Copy;
		}
		public virtual void ProcessDragDrop(TreeNode targetNode) {
			if(DragDropHelper == null)
				return;
			DragDropHelper.ProcessDragDrop(SourceNode, targetNode);
			DragDropHelper = null;
		}
		protected virtual DragDropSupportBase CreateDragDropHelper(DragDropSupportBase.DragDropAction cmd) {
			return DragDropSupportBase.Create(RibbonTreeView, cmd);
		}
		protected virtual TreeNode SourceNode {
			get {
				System.Diagnostics.Debug.Assert(Items.Length == 1, "Invalid lenght of DragDropItems array");
				if(Items.Length != 1)
					return null;
				return Items[0].Node;
			} 
		}
		protected bool IsLocalDrapDrop {
			get {
				return RibbonTreeView.GetType() != typeof(RunTimeRibbonTreeViewOriginalView);
			}
		}
		protected DragDropSupportBase DragDropHelper { get; private set; }
		protected RunTimeRibbonTreeView RibbonTreeView { get; private set; }
	}
	#endregion
	#region TreeView Utils
	public class TreeViewUtils {
		public delegate void TreeNodeProcessor(TreeNode node);
		public static void ProcessTreeNodes(TreeView tree, TreeNodeProcessor callback) {
			ProcessTreeNodesCore(tree.Nodes, callback);
		}
		static void ProcessTreeNodesCore(TreeNodeCollection nodes, TreeNodeProcessor callback) {
			foreach(TreeNode node in nodes) {
				ProcessTreeNodesCore(node.Nodes, callback);
				callback(node);
			}
		}
		public static void HideCheckBox(TreeView tree, TreeNode node) {
			DevExpress.XtraBars.BarNativeMethods.TVITEM tvi = new DevExpress.XtraBars.BarNativeMethods.TVITEM();
			tvi.hItem = node.Handle;
			tvi.mask = BarNativeMethods.TVIF_STATE;
			tvi.stateMask = BarNativeMethods.TVIS_STATEIMAGEMASK;
			tvi.state = 0;
			BarNativeMethods.SendMessage(tree.Handle, BarNativeMethods.TVM_SETITEM, IntPtr.Zero, ref tvi);
		}
	}
	#endregion
	#endregion
}
