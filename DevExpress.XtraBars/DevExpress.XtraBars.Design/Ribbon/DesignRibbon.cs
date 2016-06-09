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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon.Design {
	[ToolboxItem(false)]
	public class CategoriesComboBox : ComboBoxEdit {
		BarManager manager = null;
		public CategoriesComboBox() { 
		}
		public BarManager Manager { get { return manager; } set { manager = value; } }
		public void InitCategories(int selIndex) { 
			if(Manager == null)return ;
			Properties.Items.Clear();
			Properties.Items.BeginUpdate();
			try { 
				foreach(BarManagerCategory category in Manager.Categories) {
					Properties.Items.Add(category);
				}
				Properties.Items.Add(BarManagerCategory.TotalCategory);
				Properties.Items.Insert(0, BarManagerCategory.DefaultCategory);
				if(selIndex == -1) selIndex = Properties.Items.Count - 1;
				if(selIndex > -1 && selIndex < Properties.Items.Count) SelectedIndex = selIndex;
			}
			finally { Properties.Items.EndUpdate(); }
		}
		public BarManagerCategory SelCategory { 
			get {
				if(SelectedIndex == 0)return BarManagerCategory.DefaultCategory;
				if(SelectedIndex == Manager.Categories.Count + 1)return BarManagerCategory.TotalCategory;
				return Manager.Categories[SelectedIndex - 1] as BarManagerCategory;
			}
		}
	}
	public class StatusBarHelper { 
		public static RibbonStatusBar GetStatusBar(RibbonControl ribbon) {
			for(int objIndex = 0; objIndex < ribbon.Parent.Container.Components.Count; objIndex++) {
				RibbonStatusBar statusBar = ribbon.Parent.Container.Components[objIndex] as RibbonStatusBar;
				if(statusBar == null || statusBar.Ribbon != ribbon) continue;
				return statusBar;
			}
			return null;			   
		}
	}
	public class GalleryItemsDragInfo {
		TreeNode[] nodes;
		public GalleryItemsDragInfo(TreeNode[] nodes) {
			this.nodes = nodes;
		}
		public TreeNode[] Nodes { get { return nodes; } }
		public bool IsItems { get { return Nodes[0].Tag is GalleryItem; } }
		public bool IsGroups { get { return Nodes[0].Tag is GalleryItemGroup; } }
		public bool IsGallery { get { return Nodes[0].Tag is RibbonGalleryBarItem || Nodes[0].Tag is GalleryDropDown; } }
		public GalleryItemGroup[] Groups {
			get { 
				if(!IsGroups)return null;
				GalleryItemGroup[] conts = new GalleryItemGroup[Nodes.Length];
				for(int itemIndex = 0; itemIndex < Nodes.Length; itemIndex++) {
					conts[itemIndex] = Nodes[itemIndex].Tag as GalleryItemGroup;
				}
				return conts;
			}
		}
		public GalleryItem[] Items {
			get {
				if(!IsItems)return null;
				GalleryItem[] items = new GalleryItem[Nodes.Length];
				for(int itemIndex = 0; itemIndex < Nodes.Length; itemIndex++) {
					items[itemIndex] = Nodes[itemIndex].Tag as GalleryItem;
				}
				return items;
			}
		}
	}
	public enum GalleryTreeImages { InRibbonGallery, PopupGallery, Group, Item, MoveIn, MoveUp, MoveDown, CantMove }
	[DXToolboxItem(true)]
	public class GalleryItemsTree : ItemsTreeView { 
		BaseGallery gallery;
		string galleryName;
		TreeNode dropTargetNode;
		RibbonControl ribbon;
		IComponentChangeService componentChangeService;
		IDesignerHost host;
		int galleryDropDownIndex = 1;
		int inplaceGalleryIndex = 1;
		public GalleryItemsTree() {
			this.dropTargetNode = null;
			this.ribbon = null;
			this.componentChangeService = null;
			this.host = null;
		}
		public virtual RibbonControl Ribbon { get { return ribbon; } set { ribbon = value; } }
		public virtual IComponentChangeService ComponentChangeService { get { return componentChangeService; } set { componentChangeService = value; } }
		public virtual IDesignerHost Host { get { return host; } set { host = value;} }
		public virtual BaseGallery Gallery { get { return gallery; } }
		public virtual InRibbonGallery InRibbonGallery { get { return gallery as InRibbonGallery; } }
		public virtual InDropDownGallery PopupGallery { get { return gallery as InDropDownGallery; } }
		public string GalleryName { get { return galleryName; } }
		public bool IsGallery(TreeNode node) { return node.Tag is RibbonGalleryBarItem || node.Tag is GalleryDropDown; }
		public virtual void Initialize(string name, BaseGallery gallery) { 
			this.gallery = gallery;
			this.galleryName = name;
			FillTree();	
		}
		protected virtual GalleryItemGroup SelGroup {
			get {
				if(SelectedNode == null) return null;
				if(SelectedNode.Parent != null && SelectedNode.Parent.Tag as GalleryItemGroup != null) return SelectedNode.Parent.Tag as GalleryItemGroup;
				return SelectedNode.Tag as GalleryItemGroup;
			}
		}
		protected virtual GalleryItem SelItem {
			get {
				if(SelectedNode == null || SelectedNode.Tag as GalleryItem == null) return null;
				return SelectedNode.Tag as GalleryItem;
			}
		}
		protected override bool UseThemes {
			get { return true; }
		}
		protected virtual void FillTree() { 
			Nodes.Clear();
			AddGalleryNode();
			for(int i = 0; i < Gallery.Groups.Count; i++) {
				AddGroupNode(Gallery.Groups[i]);
				SelectedNode = Nodes[0].Nodes[i];
				for(int j = 0; j < Gallery.Groups[i].Items.Count; j++) {
					AddItemNode(Gallery.Groups[i].Items[j]);
				}
			}
		}
		protected virtual object GalleryOwner { 
			get {
				if(Gallery is InRibbonGallery) return ((InRibbonGallery)Gallery).OwnerItem;
				if(Gallery is InDropDownGallery) return ((InDropDownGallery)Gallery).GalleryDropDown;
				if(Gallery is PopupGalleryEditGallery) return ((PopupGalleryEditGallery)Gallery).GalleryItem;
				if(Gallery is GalleryControlGallery) return ((GalleryControlGallery)Gallery).GalleryControl;
				return null;
			}
		}
		protected virtual void AddGalleryNode() {
			int imageIndex = Gallery is InRibbonGallery ? (int)GalleryTreeImages.InRibbonGallery : (int)GalleryTreeImages.PopupGallery;
			TreeNode node = new TreeNode(GalleryName, imageIndex, imageIndex);
			node.Tag = GalleryOwner;
			Nodes.Add(node);
		}
		public virtual void AddGalleryDropDown() {
			if(Ribbon == null) return;
			GalleryDropDown dropDown = RibbonDesignTimeManager.AddGalleryDropDown();
			Initialize(dropDown.Name, dropDown.Gallery);
			SelectedNode = Nodes[0];
			AddGroup();
		}
		protected RibbonDesignTimeManager RibbonDesignTimeManager {
			get { return Ribbon.GetDesignTimeManager() as RibbonDesignTimeManager; }
		}
		public virtual void AddInRibbonGallery() {
			if(Ribbon == null) return;
			RibbonGalleryBarItem gallery = new RibbonGalleryBarItem(null);
			gallery.Caption = "InplaceGallery" + InplaceGalleryIndex;
			InplaceGalleryIndex++;
			Ribbon.Container.Add(gallery);
			Ribbon.Items.Add(gallery);
			Initialize(gallery.Caption, gallery.Gallery);
			SelectedNode = Nodes[0];
			AddGroup();
		}
		public virtual void AddGroup() {
			if(gallery == null)return ;
			GalleryItemGroup grp = new GalleryItemGroup();
			grp.Caption = GalleryControlDesignTimeManager.GetGalleryGroupName();
			Gallery.Groups.Add(grp);
			ComponentChangeService.OnComponentChanged(Gallery, null, null, null);
			SelectedNode = AddGroupNode(grp);
		}
		public virtual void AddItem() {
			if(SelGroup == null) return;
			GalleryItem item = new GalleryItem();
			item.Caption = GalleryControlDesignTimeManager.GetGalleryItemName();
			SelGroup.Items.Add(item);
			SelectedNode = AddItemNode(item);
		}
		protected virtual TreeNode AddItemNode(GalleryItem item) {
			TreeNode node = new TreeNode(item.Caption, (int)GalleryTreeImages.Item, (int)GalleryTreeImages.Item);
			node.Tag = item;
			if(IsSelGroup) SelectedNode.Nodes.Add(node);
			else SelectedNode.Parent.Nodes.Add(node);
			return node;
		}
		protected virtual TreeNode AddGroupNode(GalleryItemGroup grp) {
			TreeNode node = new TreeNode(grp.Caption, (int)GalleryTreeImages.Group, (int)GalleryTreeImages.Group);
			node.Tag = grp;
			Nodes[0].Nodes.Add(node);
			return node;
		}
		public object[] SelectedTreeItems {
			get {
				object[] selItems = new Object[SelNodes.Length];
				for(int itemIndex = 0; itemIndex < SelNodes.Length; itemIndex++) {
					selItems[itemIndex] = new GalleryObjectDescriptor(SelNodes[itemIndex].Tag, Gallery, null);
				}
				return selItems;
			}
		}
		protected virtual bool CanMoveUp() {
			if(SelNodes == null || SelNodes.Length != 1 || SelectedNode.PrevNode == null) return false;
			return true;
		}
		protected virtual bool CanMoveDown() {
			if(SelNodes == null || SelNodes.Length != 1 || SelectedNode.NextNode == null) return false;
			return true;
		}
		protected virtual void SwapGroups(TreeNode node1, TreeNode node2) {
			GalleryItemGroup cont1 = node1.Tag as GalleryItemGroup, cont2 = node2.Tag as GalleryItemGroup;
			int index = Gallery.Groups.IndexOf(cont1);
			Gallery.Groups.Remove(cont2);
			Gallery.Groups.Insert(index, cont2);
			ComponentChangeService.OnComponentChanged(Gallery, null, null, null);
			SwapNodes(node1, node2);
		}
		protected virtual void SwapItems(TreeNode node1, TreeNode node2) {
			GalleryItem item1 = node1.Tag as GalleryItem, item2 = node2.Tag as GalleryItem;
			int index = item1.GalleryGroup.Items.IndexOf(item1);
			item1.GalleryGroup.Items.Remove(item2);
			item1.GalleryGroup.Items.Insert(index, item2);
			ComponentChangeService.OnComponentChanged(item1.GalleryGroup, null, null, null);
			SwapNodes(node1, node2);
		}
		protected virtual void SwapNodes(TreeNode node1, TreeNode node2) {
			int index = node1.Parent.Nodes.IndexOf(node1);
			node1.Parent.Nodes.Remove(node2);
			node1.Parent.Nodes.Insert(index, node2);
		}
		protected virtual bool IsSelGroup { 
			get {
				if(SelectedNode == null) return false;
				return SelectedNode.Tag is GalleryItemGroup;	
			} 
		}
		protected virtual bool IsSelItem {
			get {
				if(SelectedNode == null) return false;
				return SelectedNode.Tag is GalleryItem;
			} 
		}
		public virtual void MoveUp() {
			if(!CanMoveUp()) return;
			TreeNode selNode = SelectedNode;
			if(IsSelGroup) SwapGroups(SelectedNode.PrevNode, SelectedNode);
			else if(IsSelItem) SwapItems(SelectedNode.PrevNode, SelectedNode);
			SelectedNode = selNode;
		}
		public virtual void MoveDown() {
			if(!CanMoveDown()) return;
			TreeNode selNode = SelectedNode;
			if(IsSelGroup) SwapGroups(SelectedNode, SelectedNode.NextNode);
			else if(IsSelItem) SwapItems(SelectedNode, SelectedNode.NextNode);
			SelectedNode = selNode;
		}
		protected virtual GalleryItemsDragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(GalleryItemsDragInfo)) as GalleryItemsDragInfo;
		}
		protected virtual bool IsNodeInDragNodes(GalleryItemsDragInfo info, TreeNode node) {
			for(int itemIndex = 0; itemIndex < info.Nodes.Length; itemIndex++) {
				if(info.Nodes[itemIndex] == node) return true;
			}
			return false;
		}
		protected virtual DragDropEffects GetDragDropEffect(GalleryItemsDragInfo info, TreeNode node) {
			if(info.IsGallery || node == null || IsNodeInDragNodes(info, node)) return DragDropEffects.None;
			if(info.IsGroups && node.Tag is GalleryItem) return DragDropEffects.None;
			if(info.IsItems && IsGallery(node)) return DragDropEffects.None;
			return DragDropEffects.Move;
		}
		public override void OnDragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			if(SelNodes == null || SelNodes.Length == 0) return;
			GalleryItemsDragInfo info = new GalleryItemsDragInfo(SelNodes);
			e.DragObject = new GalleryItemsDragInfo(SelNodes);
			e.AllowEffects = DragDropEffects.Move;
		}
		public override void OnDragNodeStart(object sender, EventArgs e) { DropTargetNode = null; }
		protected TreeNode DropTargetNode { get { return dropTargetNode; } set { dropTargetNode = value; } }
		protected virtual GalleryTreeImages GetDirection(TreeNode node, GalleryItemsDragInfo dragInfo) {
			if(dragInfo.Nodes[0].Parent != node.Parent)return GalleryTreeImages.MoveUp;
			if(node.Parent.Nodes.IndexOf(node) > node.Parent.Nodes.IndexOf(dragInfo.Nodes[dragInfo.Nodes.Length-1])) return GalleryTreeImages.MoveDown;
			return GalleryTreeImages.MoveUp;
		}
		protected virtual GalleryTreeImages GetNodeImageIndex(TreeNode node, GalleryItemsDragInfo dragInfo) {
			if(IsNodeInDragNodes(dragInfo, node)) return GalleryTreeImages.CantMove;
			if(dragInfo.IsGroups) {
				if(IsGallery(node)) return GalleryTreeImages.MoveIn;
				else if(node.Tag is GalleryItem) return GalleryTreeImages.CantMove;
				return GetDirection(node, dragInfo);
			}
			if(dragInfo.IsItems) {
				if(IsGallery(node)) return GalleryTreeImages.CantMove;
				else if(node.Tag is GalleryItemGroup) return GalleryTreeImages.MoveIn;
				return GetDirection(node, dragInfo);
			}
			return GalleryTreeImages.CantMove;
		}
		protected virtual GalleryTreeImages DefaultImageIndex(TreeNode node) {
			if(node.Tag is RibbonGalleryBarItem)
				return GalleryTreeImages.InRibbonGallery;
			else if(node.Tag is GalleryDropDown)
				return GalleryTreeImages.PopupGallery;
			else if(node.Tag is GalleryItemGroup)
				return GalleryTreeImages.Group;
			return GalleryTreeImages.Item;
		}
		protected virtual void ResetNodeImage(TreeNode node) {
			if(node == null) return;
			node.ImageIndex = node.SelectedImageIndex = (int)DefaultImageIndex(node);
		}
		protected virtual BaseGallery GetGallery(TreeNode node) {
			RibbonGalleryBarItem barItem = node.Tag as RibbonGalleryBarItem;
			GalleryDropDown dropDown = node.Tag as GalleryDropDown;
			GalleryControl gallery = node.Tag as GalleryControl;
			if(gallery != null) return gallery.Gallery;
			if(barItem != null) return barItem.Gallery;
			if(dropDown != null) return dropDown.Gallery;
			return null;
		}
		protected virtual void RemoveObjectFromCollection(TreeNode node) { 
			BaseGallery galleryControl = GetGallery(node.Parent);
			GalleryItemGroup galleryGroup = node.Parent.Tag as GalleryItemGroup;
			if(galleryControl != null) galleryControl.Groups.Remove(node.Tag as GalleryItemGroup);
			else if(galleryGroup != null) galleryGroup.Items.Remove(node.Tag as GalleryItem);
		}
		protected virtual void RemoveObjectsFromCollection(GalleryItemsDragInfo info, TreeNode node) {
			for(int itemIndex = 0; itemIndex < info.Nodes.Length; itemIndex++) {
				RemoveObjectFromCollection(info.Nodes[itemIndex]);
				info.Nodes[itemIndex].Parent.Nodes.Remove(info.Nodes[itemIndex]);
			}
		}
		protected virtual int GetInsertIndex(GalleryItemsDragInfo info, TreeNode node, GalleryTreeImages direction) {
			BaseGallery galleryControl = GetGallery(node);
			GalleryItemGroup galleryGroup = node.Tag as GalleryItemGroup;
			GalleryItem galleryItem = node.Tag as GalleryItem;
			if(galleryControl != null) return galleryControl.Groups.Count;
			if(galleryGroup != null) {
				if(info.IsItems) return galleryGroup.Items.Count;
				else return direction == GalleryTreeImages.MoveDown ? Gallery.Groups.IndexOf(galleryGroup) + 1 : Gallery.Groups.IndexOf(galleryGroup);
			}
			if(galleryItem != null) return direction == GalleryTreeImages.MoveDown ? galleryItem.GalleryGroup.Items.IndexOf(galleryItem) + 1 : galleryItem.GalleryGroup.Items.IndexOf(galleryItem);
			return 0;
		}
		protected virtual void InsertObjectsIntoCollection(GalleryItemsDragInfo info, TreeNode node, GalleryTreeImages direction) {
			int insertAt = GetInsertIndex(info, node, direction);
			if(IsGallery(node)) InsertObjectsIntoGallery(info, node, insertAt);
			else if(node.Tag is GalleryItemGroup) InsertObjectsIntoGroup(info, node, insertAt);
			else if(node.Tag is GalleryItem) InsertObjectsIntoGroup(info, node.Parent, insertAt);
		}
		protected virtual void InsertObjectsIntoGallery(GalleryItemsDragInfo info, TreeNode node, int insertAt) {
			for(int itemIndex = info.Nodes.Length - 1; itemIndex >= 0; itemIndex--) {
				Gallery.Groups.Insert(insertAt, info.Nodes[itemIndex].Tag as GalleryItemGroup);
				Nodes[0].Nodes.Insert(insertAt, info.Nodes[itemIndex]);
			}
			ComponentChangeService.OnComponentChanged(Gallery, null, null, null);
			return;
		}
		protected virtual void InsertItemsIntoGroup(GalleryItemsDragInfo info, TreeNode node, int insertAt) {
			GalleryItemGroup grp = node.Tag as GalleryItemGroup;
			for(int index = info.Nodes.Length - 1; index >=0 ; index--) {
				grp.Items.Insert(insertAt, info.Nodes[index].Tag as GalleryItem);
				node.Nodes.Insert(insertAt, info.Nodes[index]);
			}
			ComponentChangeService.OnComponentChanged(grp, null, null, null);
		}
		protected virtual void InsertObjectsIntoGroup(GalleryItemsDragInfo info, TreeNode node, int insertAt) {
			if(info.IsItems) InsertItemsIntoGroup(info, node, insertAt);
			else InsertObjectsIntoGallery(info, node, insertAt);
		}
		protected virtual void MoveNodesTo(GalleryItemsDragInfo info, TreeNode node) {
			GalleryTreeImages direction = GetDirection(node, info);
			object grp = info.Nodes[0].Parent.Tag;
			RemoveObjectsFromCollection(info, node);
			ComponentChangeService.OnComponentChanged(grp, null, null, null);
			InsertObjectsIntoCollection(info, node, direction);
		}
		protected override void OnDragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
			GalleryItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			e.Effect = GetDragDropEffect(dragInfo, node);
			if(DropTargetNode == node) return;
			SetNodeImageIndex(node, (int)GetNodeImageIndex(node, dragInfo));
			ResetNodeImage(DropTargetNode);
			DropTargetNode = node;  
		}
		public override void OnDragEnter(object sender, DragEventArgs e) { }
		public override void OnDragLeave(object sender, EventArgs e) { ResetNodeImage(DropTargetNode); }
		protected override void OnDragDrop(object sender, DragEventArgs e) {
			GalleryItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			MoveNodesTo(dragInfo, node);
			ResetNodeImage(DropTargetNode); 
		}
		public virtual void RemoveItems() {
			BeginUpdate();
			try {
				if(Nodes.Count == 0 || SelNodes == null || SelNodes.Length == 0) return;
				TreeNode[] nodes = new TreeNode[SelNodes.Length];
				for(int i = 0; i < SelNodes.Length; i++) nodes[i] = SelNodes[i];
				for(int i = 0; i < nodes.Length; i++) RemoveNode(nodes[i]);
			}
			finally { EndUpdate(); }
		}
		protected virtual void RemoveNode(TreeNode node) {
			GalleryItem item = node.Tag as GalleryItem;
			GalleryItemGroup grp = node.Tag as GalleryItemGroup;
			BaseGallery galleryControl = GetGallery(node);
			if(item != null) {
				GalleryItemGroup itemGroup = item.GalleryGroup;
				item.GalleryGroup.Items.Remove(item);
				ComponentChangeService.OnComponentChanged(itemGroup, null, null, null);
				node.Parent.Nodes.Remove(node);
			}
			else if(grp != null) {
				while(node.Nodes.Count != 0) {
					RemoveNode(node.Nodes[0]);
				}
				ComponentChangeService.OnComponentChanged(Gallery, null, null, null);
				grp.Dispose();
				node.Parent.Nodes.Remove(node);
			}
			else if(galleryControl != null) {
				while(Nodes[0].Nodes.Count != 0)
					RemoveNode(Nodes[0].Nodes[0]);
				if(InRibbonGallery != null) {
					using(DesignerTransaction trans = Host.CreateTransaction("Try to remove item")) {
						Ribbon.Items.Remove(InRibbonGallery.OwnerItem);
						ComponentChangeService.OnComponentChanged(Ribbon, null, null, null);
						InRibbonGallery.OwnerItem.Manager = null;
						trans.Commit();
						Ribbon.Container.Remove(InRibbonGallery.OwnerItem);
						InRibbonGallery.OwnerItem.Dispose();
					}
				}
				else if(PopupGallery != null) {
					Ribbon.Container.Remove(PopupGallery.GalleryDropDown);
				}
				Nodes.Clear();
			}
		}
		protected override void UpdateTreeNodeText(TreeNode node, object item) {
			if(node.Tag != item) return;
			if(item is RibbonGalleryBarItem) node.Text = (item as RibbonGalleryBarItem).Caption;
			else if(item is GalleryDropDown) node.Text = (item as GalleryDropDown).Name;
			else if(item is GalleryItemGroup) node.Text = (item as GalleryItemGroup).Caption;
			else if(item is GalleryItem) node.Text = (item as GalleryItem).Caption;
		}
		public int InplaceGalleryIndex { get { return inplaceGalleryIndex; } set { inplaceGalleryIndex = value; } }
		public int GalleryDropDownIndex { get { return galleryDropDownIndex; } set { galleryDropDownIndex = value; } }
		RibbonBarManager Manager { get { return Ribbon.Manager as RibbonBarManager; } }
		GalleryControl GalleryControl { get { return ((GalleryControlGallery)Gallery).GalleryControl; } }
		bool IsGalleryControlDesigner { get { return Gallery is GalleryControlGallery; } }
	}
}
