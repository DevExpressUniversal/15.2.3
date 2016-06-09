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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraBars.Navigation.Frames {
	[ToolboxItem(false)]
	public class ElementsDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.SimpleButton btAddGroup;
		private DXTreeView groupTreeList;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private DevExpress.Utils.ImageCollection imgs;
		private DevExpress.XtraEditors.SimpleButton btMoveUp;
		private DevExpress.XtraEditors.SimpleButton btMoveDown;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private ColumnHeader columnHeader1;
		private XtraEditors.SimpleButton btAddItem;
		private DevExpress.XtraEditors.SimpleButton btRemove;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElementsDesigner));
			this.btAddGroup = new DevExpress.XtraEditors.SimpleButton();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.imgs = new DevExpress.Utils.ImageCollection(this.components);
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupTreeList = new DevExpress.Utils.Design.DXTreeView();
			this.btMoveUp = new DevExpress.XtraEditors.SimpleButton();
			this.btMoveDown = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.btAddItem = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(360, 100);
			this.splMain.Size = new System.Drawing.Size(5, 360);
			this.splMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
			this.pgMain.Location = new System.Drawing.Point(365, 100);
			this.pgMain.Size = new System.Drawing.Size(397, 360);
			this.pnlControl.Controls.Add(this.btAddItem);
			this.pnlControl.Controls.Add(this.btMoveUp);
			this.pnlControl.Controls.Add(this.btMoveDown);
			this.pnlControl.Controls.Add(this.btAddGroup);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Size = new System.Drawing.Size(762, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(762, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 100);
			this.pnlMain.Size = new System.Drawing.Size(360, 360);
			this.horzSplitter.Size = new System.Drawing.Size(762, 4);
			this.btAddGroup.AllowFocus = false;
			this.btAddGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btAddGroup.Location = new System.Drawing.Point(0, 4);
			this.btAddGroup.Name = "btAddGroup";
			this.btAddGroup.Size = new System.Drawing.Size(30, 30);
			this.btAddGroup.TabIndex = 0;
			this.btAddGroup.TabStop = false;
			this.btAddGroup.ToolTip = "Add Group";
			this.btAddGroup.Click += new System.EventHandler(this.btAddGroup_Click);
			this.btRemove.AllowFocus = false;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btRemove.Location = new System.Drawing.Point(72, 4);
			this.btRemove.Name = "btRemove";
			this.btRemove.Size = new System.Drawing.Size(30, 30);
			this.btRemove.TabIndex = 0;
			this.btRemove.TabStop = false;
			this.btRemove.ToolTip = "Remove Element";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "");
			this.imgs.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgs.ImageStream")));
			this.imgs.TransparentColor = System.Drawing.Color.Magenta;
			this.columnHeader1.Width = 172;
			this.groupTreeList.AllowDrag = true;
			this.groupTreeList.AllowDrop = true;
			this.groupTreeList.AllowSkinning = true;
			this.groupTreeList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.groupTreeList.DefaultExpandCollapseButtonOffset = 5;
			this.groupTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupTreeList.HideSelection = false;
			this.groupTreeList.ImageIndex = 0;
			this.groupTreeList.ImageList = this.imageList1;
			this.groupTreeList.Location = new System.Drawing.Point(4, 22);
			this.groupTreeList.Name = "groupTreeList";
			this.groupTreeList.SelectedImageIndex = 0;
			this.groupTreeList.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.groupTreeList.Size = new System.Drawing.Size(352, 334);
			this.groupTreeList.TabIndex = 0;
			this.groupTreeList.SelectionChanged += OnGroupTreeListSelectionChanged;
			this.groupTreeList.DragNodeGetObject += OnTreeViewNavigationsDragNodeGetObject;
			this.groupTreeList.DragNodeAllow += OnTreeViewNavigationsDragNodeAllow;
			this.groupTreeList.AllowMultiSelectNode += OnTreeViewNavigationsAllowMultiSelectNode;
			this.groupTreeList.AfterCollapse += OnGroupTreeListAfterCollapse;
			this.groupTreeList.AfterExpand += OnGroupTreeListAfterExpand;
			this.groupTreeList.DragDrop += OnGroupTreeListDragDrop;
			this.groupTreeList.DragOver += OnTreeViewNavigationsDragOver;
			this.groupTreeList.DragLeave += OnGroupTreeListDragLeave;
			this.groupTreeList.GotFocus += OnGroupTreeListGotFocus;
			this.groupTreeList.KeyDown += OnGroupTreeListKeyDown;
			this.groupTreeList.LostFocus += OnGroupTreeListLostFocus;
			this.btMoveUp.AllowFocus = false;
			this.btMoveUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btMoveUp.Location = new System.Drawing.Point(108, 4);
			this.btMoveUp.Name = "btMoveUp";
			this.btMoveUp.Size = new System.Drawing.Size(30, 30);
			this.btMoveUp.TabIndex = 2;
			this.btMoveUp.TabStop = false;
			this.btMoveUp.ToolTip = "Move Toward Beginning";
			this.btMoveUp.Click += new System.EventHandler(this.btMoveUp_Click);
			this.btMoveDown.AllowFocus = false;
			this.btMoveDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btMoveDown.Location = new System.Drawing.Point(144, 4);
			this.btMoveDown.Name = "btMoveDown";
			this.btMoveDown.Size = new System.Drawing.Size(30, 30);
			this.btMoveDown.TabIndex = 1;
			this.btMoveDown.TabStop = false;
			this.btMoveDown.ToolTip = "Move Toward End";
			this.btMoveDown.Click += new System.EventHandler(this.btMoveDown_Click);
			this.groupControl1.Controls.Add(this.groupTreeList);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl1.Size = new System.Drawing.Size(360, 360);
			this.groupControl1.TabIndex = 4;
			this.groupControl1.Text = "AccordionControl Elements:";
			this.btAddItem.AllowFocus = false;
			this.btAddItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btAddItem.Location = new System.Drawing.Point(36, 4);
			this.btAddItem.Name = "btAddItem";
			this.btAddItem.Size = new System.Drawing.Size(30, 30);
			this.btAddItem.TabIndex = 3;
			this.btAddItem.TabStop = false;
			this.btAddItem.ToolTip = "Add Item";
			this.btAddItem.Click += new System.EventHandler(this.btAddItem_Click);
			this.Name = "ElementsDesigner";
			this.Size = new System.Drawing.Size(762, 460);
			this.Load += new System.EventHandler(this.GroupItemDesigner_Load);
			this.Resize += new System.EventHandler(this.GroupItemDesigner_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private AccordionControl AccordionControl { get { return EditingObject as AccordionControl; } }
		protected override void InitImages() {
			base.InitImages();
			btAddGroup.Image = imgs.Images[0];
			btAddItem.Image = imgs.Images[1];
			btRemove.Image = imgs.Images[2];
			btMoveUp.Image = imgs.Images[3];
			btMoveDown.Image = imgs.Images[4];
		}
		public ElementsDesigner()
			: base(0) {
			InitializeComponent();
			pgMain.BringToFront();
		}
		public override void InitComponent() {
			lbCaption.Text = "Elements Designer";
			IComponentChangeService cs = AccordionControlGetService((typeof(IComponentChangeService))) as IComponentChangeService;
			if(cs != null) cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			FillData();
		}
		protected void DeInit() {
			if(AccordionControl == null) return;
			IComponentChangeService cs = AccordionControlGetService((typeof(IComponentChangeService))) as IComponentChangeService;
			if(cs != null) cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			if(pgMain.SelectedObjects != null && e.ChangedItem != null && e.ChangedItem.Label == "Caption") {
				foreach(object obj in pgMain.SelectedObjects) {
					OnComponentRename(this, new ComponentRenameEventArgs(obj, "", ""));
				}
			}
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			TreeNode node = FindGroup(e.Component as AccordionControlElement);
			TreeNode nodeItem = FindNodeItem(e.Component as AccordionControlElement);
			if(node != null) node.Text = GetNodeText(e.Component as AccordionControlElement);
			if(nodeItem != null) nodeItem.Text = GetNodeText(e.Component as AccordionControlElement);
		}
		protected TreeNode FindGroup(AccordionControlElement group) {
			if(group == null) return null;
			foreach(TreeNode node in groupTreeList.Nodes) {
				if(node.Tag == group) return node;
			}
			return null;
		}
		protected TreeNode FindNodeItem(AccordionControlElement item) {
			if(item == null) return null;
			foreach(TreeNode nodeGroup in groupTreeList.Nodes) {
				foreach(TreeNode node in nodeGroup.Nodes) {
					AccordionControlElement element = node.Tag as AccordionControlElement;
					if(element == item) return node;
				}
			}
			return null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DeInit();
			}
			base.Dispose(disposing);
		}
		protected virtual void FillData() {
			FillGroupTreeList(null);
		}
		Hashtable savedNodes = new Hashtable();
		bool loading = false;
		protected virtual void FillGroupTreeList(AccordionControlElement selectedGroup) {
			this.loading = true;
			try {
				savedNodes.Clear();
				foreach(TreeNode node in groupTreeList.Nodes) {
					if(node.IsExpanded) savedNodes[node.Tag] = 0;
				}
				groupTreeList.BeginUpdate();
				groupTreeList.Nodes.Clear();
				try {
					AddGroupsInTreeList(selectedGroup, AccordionControl.Elements, groupTreeList.Nodes, 0);
				}
				finally {
					groupTreeList.EndUpdate();
				}
			}
			finally {
				this.loading = false;
			}
		}
		protected void AddGroupsInTreeList(AccordionControlElement selectedGroup, AccordionControlElementCollection col, TreeNodeCollection nodes, int level) {
			foreach(AccordionControlElement group in col) {
				TreeNode node = new TreeNode(GetNodeText(group), 0, 0);
				node.Tag = group;
				nodes.Add(node);
				ResetNodeImage(node);
				if(group == selectedGroup) {
					groupTreeList.SelectedNode = node;
				}
				if(group.Elements != null && group.Elements.Count > 0)
					AddGroupsInTreeList(selectedGroup, group.Elements, node.Nodes, level + 1);
			}
		}
		protected string GetNodeText(AccordionControlElement element) {
			string name = element.Site == null ? string.Empty : element.Site.Name;
			return string.Format("{0} [{1}]", name, element.Text);
		}
		private void OnGroupTreeListKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				RemoveNodes(groupTreeList.SelNodes);
			if(e.KeyCode == Keys.Insert)
				AddGroup();
		}
		private void SetNodeExpand(TreeNode node, bool expand) {
			AccordionControlElement group = node.Tag as AccordionControlElement;
			if(group != null) group.Expanded = expand;
		}
		private void OnGroupTreeListAfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(!this.loading) {
				if(e.Node.Tag is AccordionControlElement)
					SetNodeExpand(e.Node, true);
			}
		}
		private void OnGroupTreeListAfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(e.Node.Tag is AccordionControlElement)
				SetNodeExpand(e.Node, false);
		}
		private void OnTreeViewNavigationsDragNodeGetObject(object sender, DevExpress.Utils.Design.TreeViewGetDragObjectEventArgs e) {
			TreeNode[] selNodes = e.DragObject as TreeNode[];
			if(selNodes == null || selNodes.Length == 0) return;
			AccordionControlElement[] links = new AccordionControlElement[selNodes.Length];
			for(int n = 0; n < selNodes.Length; n++) {
				links[n] = selNodes[n].Tag as AccordionControlElement;
			}
			e.DragObject = new AccordionDragInfo(links);
		}
		private void OnTreeViewNavigationsDragNodeAllow(object sender, System.ComponentModel.CancelEventArgs e) {
			TreeNode node = groupTreeList.SelNode;
			e.Cancel = node == null || !(node.Tag is AccordionControlElement);
		}
		private void OnTreeViewNavigationsAllowMultiSelectNode(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			e.Cancel = !(e.Node.Tag is AccordionControlElement);
		}
		public const int ImageGroup = 0, ImageItem = 1, ImageAdd = 2, ImageInsert = 3, ImageInsertBottom = 4, ImageCancel = 5;
		int GetNodeImageIndex(TreeNode node) {
			if(node != null) {
				AccordionControlElement elem = node.Tag as AccordionControlElement;
				if(elem != null) return elem.Style == ElementStyle.Group ? ImageGroup : ImageItem;
			}
			return -1;
		}
		AccordionDragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(AccordionDragInfo)) as AccordionDragInfo;
		}
		TreeNode dropTargetNode = null, dropSelectedNode = null;
		protected void ResetNodeImage(TreeNode node) { SetNodeImage(node, GetNodeImageIndex(node)); }
		protected void SetNodeImage(TreeNode node, int index) {
			if(node == null) return;
			node.SelectedImageIndex = node.ImageIndex = index;
		}
		private void OnTreeViewNavigationsDragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			AccordionDragInfo info = GetDragObject(e.Data);
			if(info == null) return;
			TreeNode node = groupTreeList.GetNodeAt(groupTreeList.PointToClient(new Point(e.X, e.Y)));
			int setIndex = -1;
			if(node == null) {
				this.dropTargetNode = null;
				e.Effect = info.IsElementDragging ? DragDropEffects.Move : DragDropEffects.Copy;
				return;
			}
			if(node == this.dropSelectedNode && this.dropTargetNode == node) {
				return;
			}
			AccordionControlElement destGroup = GetNodeGroup(node);
			if(info.IsElementDragging) {
				setIndex = node.Parent == null ? ImageAdd : ImageInsert;
				if(destGroup == info.Element.OwnerElement) {
					if(setIndex == ImageAdd) setIndex = -1;
					else {
						int prevIndex = destGroup.Elements.IndexOf(info.Element);
						int newIndex = destGroup.Elements.IndexOf(node.Tag as AccordionControlElement);
						if(prevIndex == newIndex || newIndex - 1 == prevIndex) setIndex = -1;
					}
				}
			}
			else {
				if(info.DragElements.Length != 1 || !CanDragDrop(info.DragElements[0], destGroup)) setIndex = ImageCancel;
				else setIndex = node.Parent == null ? ImageAdd : ImageInsert;
			}
			this.dropSelectedNode = node;
			ResetNodeImage(this.dropTargetNode);
			this.dropTargetNode = node;
			if(setIndex != -1) SetNodeImage(this.dropTargetNode, setIndex);
			if(setIndex == -1 || setIndex == ImageCancel)
				e.Effect = DragDropEffects.None;
			else e.Effect = info.IsElementDragging ? DragDropEffects.Move : DragDropEffects.Copy;
		}
		AccordionControlElement GetNodeGroup(TreeNode node) {
			if(node == null) return null;
			AccordionControlElement item = node.Tag as AccordionControlElement;
			if(node.Parent != null && item.Style == ElementStyle.Item) node = node.Parent;
			return node.Tag as AccordionControlElement;
		}
		private void OnGroupTreeListDragLeave(object sender, System.EventArgs e) {
			SetDefaultCursor();
		}
		void SetDefaultCursor() {
			ResetNodeImage(this.dropTargetNode);
			this.dropTargetNode = null;
			Cursor = Cursors.Default;
		}
		protected bool CanDragDrop(AccordionControlElement element, AccordionControlElement elementNode) {
			if(element == null || elementNode == null || element == elementNode) return false;
			AccordionControlElement ownerNode = elementNode.OwnerElement;
			while(ownerNode != null) {
				if(ownerNode == element) return false;
				ownerNode = ownerNode.OwnerElement;
			}
			return true;
		}
		void OnGroupTreeListDragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			AccordionControlElement destGroup = GetNodeGroup(this.dropTargetNode);
			AccordionDragInfo info = GetDragObject(e.Data);
			if(info == null) return;
			if(destGroup == null) {
				if(info.DragElements.Length != 1) return;
				if(info.DragElements[0].OwnerElement != null)
					info.DragElements[0].OwnerElement.Elements.Remove(info.DragElements[0]);
				if(AccordionControl.Elements.Contains(info.DragElements[0]))
					AccordionControl.Elements.Remove(info.DragElements[0]);
				AccordionControl.Elements.Add(info.DragElements[0]);
			}
			else {
				int destIndex = 0;
				AccordionControlElement elem = this.dropTargetNode.Tag as AccordionControlElement;
				if(elem != null && elem.Style == ElementStyle.Item) destIndex = destGroup.Elements.IndexOf(elem);
				foreach(AccordionControlElement link in info.DragElements) {
					if(!CanDragDrop(link, destGroup)) continue;
					if(link.OwnerElement != null) link.OwnerElement.Elements.Remove(link);
					else if(AccordionControl.Elements.Contains(link)) AccordionControl.Elements.Remove(link);
					destGroup.Elements.Insert(destIndex, link);
				}
			}
			FillGroupTreeList(destGroup);
			if(destGroup != null) destGroup.Expanded = true;
			SetDefaultCursor();
		}
		private void OnTreeViewGotFocus(object sender, EventArgs e) {
			CheckButtons();
		}
		private void OnGroupTreeListLostFocus(object sender, EventArgs e) {
			CheckButtons();
		}
		private void OnGroupTreeListGotFocus(object sender, EventArgs e) {
			if(groupTreeList.SelCount > 0) {
				OnGroupTreeListSelectionChanged(this, EventArgs.Empty);
			}
			CheckButtons();
		}
		object AccordionControlGetService(Type type) {
			if(AccordionControl.Site == null) return null;
			return AccordionControl.Site.GetService(type);
		}
		ISelectionService SelectionService { get { return AccordionControlGetService(typeof(ISelectionService)) as ISelectionService; } }
		protected override bool AllowGlobalStore { get { return false; } }
		IDesignerHost DesignerHost {
			get {
				if(AccordionControl == null || AccordionControl.Site == null)
					return null;
				return AccordionControl.Site.Container as IDesignerHost;
			}
		}
		protected void AddGroup() {
			if(DesignerHost == null) return;
			AccordionControlElement element = new AccordionControlElement();
			if(groupTreeList.SelNode != null) {
				AccordionControlElement selectedElement = groupTreeList.SelNode.Tag as AccordionControlElement;
				if(selectedElement.OwnerElement != null)
					selectedElement.OwnerElement.Elements.Add(element);
				else AccordionControl.Elements.Add(element);
			}
			else AccordionControl.Elements.Add(element);
			DesignerHost.Container.Add(element);
			element.Text = element.Site.Name.Replace("accordionControl","");
			FillGroupTreeList(element);
		}
		protected void AddItem() {
			if(DesignerHost == null || groupTreeList.SelNode == null) return;
			AccordionControlElement element = new AccordionControlElement();
			element.Style = ElementStyle.Item;
			AccordionControlElement parentElement = groupTreeList.SelNode.Tag as AccordionControlElement;
			if(parentElement.Style == ElementStyle.Group) {
				parentElement.Elements.Add(element);
			}
			else {
				if(parentElement.OwnerElement == null) return;
				parentElement.OwnerElement.Elements.Add(element);
			}
			DesignerHost.Container.Add(element);
			element.Text = element.Site.Name.Replace("accordionControl", "");
			FillGroupTreeList(element);
		}
		private void btAddGroup_Click(object sender, System.EventArgs e) {
			BeginTreeViewUpdate();
			AddGroup();
			EndTreeViewUpdate();
		}
		protected void BeginTreeViewUpdate() {
			groupTreeList.BeginSelection();
			groupTreeList.SelectedNode = null;
		}
		protected void EndTreeViewUpdate() {
			groupTreeList.EndSelection();
		}
		protected void CheckButtons() {
			btRemove.Enabled = groupTreeList.SelNode != null;
			if(groupTreeList.SelNode != null) {
				btRemove.Enabled = true;
				TreeNodeCollection ownerNodes = GetSelectedNodeOwnerCollection(groupTreeList);
				int index = ownerNodes.IndexOf(groupTreeList.SelNode);
				btMoveUp.Enabled = index > 0;
				btMoveDown.Enabled = index > -1 && index < ownerNodes.Count - 1;
			}
			btMoveUp.Visible = btMoveDown.Visible = groupTreeList.SelNode != null;
		}
		protected TreeNodeCollection GetSelectedNodeOwnerCollection(DXTreeView treeList) {
			if(treeList.SelNode.Parent != null)
				return treeList.SelNode.Parent.Nodes;
			return treeList.Nodes;
		}
		protected void RemoveNodes(TreeNode[] nodes) {
			if(nodes == null || nodes.Length == 0) return;
			AccordionControl.BeginUpdate();
			try {
				groupTreeList.BeginSelection();
				try {
					for(int n = nodes.Length - 1; n >= 0; n--) {
						TreeNode node = nodes[n];
						AccordionControlElement group = node.Tag as AccordionControlElement;
						if(group != null) group.Dispose();
						node.Remove();
					}
					groupTreeList.UpdateSelection();
				}
				finally {
					groupTreeList.EndSelection();
				}
			}
			finally {
				AccordionControl.EndUpdate();
			}
			if(groupTreeList.Nodes.Count == 0) {
				ClearSelection();
			}
		}
		protected void ClearSelection() {
			pgMain.SelectedObject = null;
			if(SelectionService != null) SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
			CheckButtons();
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			BeginTreeViewUpdate();
			RemoveNodes(groupTreeList.SelNodes);
			EndTreeViewUpdate();
			CheckButtons();
		}
		private void GroupItemDesigner_Load(object sender, System.EventArgs e) {
			groupTreeList.Focus();
			CheckButtons();
		}
		void MoveNode(AccordionControlElement element, int delta) {
			if(element == null) return;
			AccordionControlElementCollection col = element.OwnerElement == null ? AccordionControl.Elements : element.OwnerElement.Elements;
			int index = col.IndexOf(element);
			col.Remove(element);
			col.Insert(index + delta, element);
		}
		protected void MoveNode(int delta) {
			AccordionControlElement group = groupTreeList.SelNode.Tag as AccordionControlElement;
			MoveNode(group, delta);
			FillGroupTreeList(group);
			CheckButtons();
		}
		private void btMoveUp_Click(object sender, System.EventArgs e) {
			MoveNode(-1);
		}
		private void btMoveDown_Click(object sender, System.EventArgs e) {
			MoveNode(1);
		}
		private void OnGroupTreeListSelectionChanged(object sender, System.EventArgs e) {
			if(groupTreeList.SelCount == 0) return;
			CheckButtons();
			TreeNode[] selNodes = groupTreeList.SelNodes;
			object[] objs = new object[selNodes.Length];
			for(int n = 0; n < selNodes.Length; n++) {
				objs[n] = selNodes[n].Tag as AccordionControlElement;
				groupTreeList.SelectedNode = selNodes[n];
			}
			pgMain.SelectedObjects = objs;
			if(SelectionService != null) SelectionService.SetSelectedComponents(objs, SelectionTypes.Replace);
		}
		void OnResize() {
			if(pgMain.Width < 50 || pnlMain.Width < 50) pnlMain.Width = pgMain.Width = this.Width / 2;
			if(groupControl1.Width < 50) groupControl1.Width  = pnlMain.Width / 2;
		}
		private void splitter_SplitterMoved(object sender, SplitterEventArgs e) {
			OnResize();
		}
		private void GroupItemDesigner_Resize(object sender, EventArgs e) {
			OnResize();
		}
		private void OnTreeViewSizeChanged(object sender, EventArgs e) {
			columnHeader1.Width = -2;
		}
		private void btAddItem_Click(object sender, EventArgs e) {
			BeginTreeViewUpdate();
			AddItem();
			EndTreeViewUpdate();
		}
	}
}
