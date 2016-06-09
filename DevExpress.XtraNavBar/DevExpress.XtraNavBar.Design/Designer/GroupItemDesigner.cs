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
using DevExpress.XtraNavBar;
namespace DevExpress.XtraNavBar.Frames {
	[ToolboxItem(false)]
	public class GroupItemDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.SimpleButton btAdd;
		private MultiSelectTreeView treeViewItems;
		private DXTreeView treeViewNavigations;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private DevExpress.XtraEditors.SplitterControl splitter1;
		private DevExpress.XtraEditors.SimpleButton btMoveUp;
		private DevExpress.XtraEditors.SimpleButton btMoveDown;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.GroupControl groupControl2;
		private ColumnHeader columnHeader1;
		private DevExpress.XtraEditors.SimpleButton btRemove;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupItemDesigner));
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.treeViewItems = new MultiSelectTreeView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.treeViewNavigations = new DXTreeView();
			this.splitter1 = new DevExpress.XtraEditors.SplitterControl();
			this.btMoveUp = new DevExpress.XtraEditors.SimpleButton();
			this.btMoveDown = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(360, 100);
			this.splMain.Size = new System.Drawing.Size(5, 360);
			this.splMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
			this.pgMain.Location = new System.Drawing.Point(365, 100);
			this.pgMain.Size = new System.Drawing.Size(397, 360);
			this.pnlControl.Controls.Add(this.btMoveUp);
			this.pnlControl.Controls.Add(this.btMoveDown);
			this.pnlControl.Controls.Add(this.btAdd);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Size = new System.Drawing.Size(762, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(762, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Controls.Add(this.splitter1);
			this.pnlMain.Controls.Add(this.groupControl2);
			this.pnlMain.Location = new System.Drawing.Point(0, 100);
			this.pnlMain.Size = new System.Drawing.Size(360, 360);
			this.horzSplitter.Size = new System.Drawing.Size(762, 4);
			this.btAdd.AllowFocus = false;
			this.btAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btAdd.Location = new System.Drawing.Point(0, 4);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(30, 30);
			this.btAdd.TabIndex = 0;
			this.btAdd.TabStop = false;
			this.btAdd.ToolTip = "Add group";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.btRemove.AllowFocus = false;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btRemove.Location = new System.Drawing.Point(36, 4);
			this.btRemove.Name = "btRemove";
			this.btRemove.Size = new System.Drawing.Size(30, 30);
			this.btRemove.TabIndex = 0;
			this.btRemove.TabStop = false;
			this.btRemove.ToolTip = "Remove group";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.treeViewItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeViewItems.AllowDrag = true;
			this.treeViewItems.AllowDrop = true;
			this.treeViewItems.HideSelection = false;
			this.treeViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewItems.Location = new System.Drawing.Point(4, 23);
			this.treeViewItems.Name = "treeVViewItems";
			this.treeViewItems.Size = new System.Drawing.Size(172, 333);
			this.treeViewItems.ImageList = this.imageList1;
			this.treeViewItems.TabIndex = 1;
			this.treeViewItems.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.treeViewItems.SelectionChanged += OnTreeViewItemsSelectionChanged;
			this.treeViewItems.SizeChanged += OnTreeViewSizeChanged;
			this.treeViewItems.GotFocus += OnTreeViewGotFocus;
			this.treeViewItems.KeyDown += OnTreeViewItemsKeyDown;
			this.treeViewItems.MouseDown += OnTreeViewItemsMouseDown;
			this.treeViewItems.MouseMove += OnTreeViewItemsMouseMove;
			this.treeViewItems.MouseUp += OnTreeViewItemsMouseUp;
			this.columnHeader1.Width = 172;
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.treeViewNavigations.AllowDrag = true;
			this.treeViewNavigations.AllowDrop = true;
			this.treeViewNavigations.AllowSkinning = true;
			this.treeViewNavigations.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeViewNavigations.DefaultExpandCollapseButtonOffset = 5;
			this.treeViewNavigations.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewNavigations.HideSelection = false;
			this.treeViewNavigations.ImageIndex = 0;
			this.treeViewNavigations.ImageList = this.imageList1;
			this.treeViewNavigations.Location = new System.Drawing.Point(4, 23);
			this.treeViewNavigations.Name = "treeViewNavigations";
			this.treeViewNavigations.SelectedImageIndex = 0;
			this.treeViewNavigations.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.treeViewNavigations.Size = new System.Drawing.Size(167, 333);
			this.treeViewNavigations.TabIndex = 0;
			this.treeViewNavigations.SelectionChanged += OnTreeViewNavigationsSelectionChanged;
			this.treeViewNavigations.DragNodeGetObject += OnTreeViewNavigationsDragNodeGetObject;
			this.treeViewNavigations.DragNodeAllow += OnTreeViewNavigationsDragNodeAllow;
			this.treeViewNavigations.AllowMultiSelectNode += OnTreeViewNavigationsAllowMultiSelectNode;
			this.treeViewNavigations.AfterCollapse += OnTreeViewNavigationsAfterCollapse;
			this.treeViewNavigations.AfterExpand += OnTreeViewNavigationsAfterExpand;
			this.treeViewNavigations.DragDrop +=  OnTreeViewNavigationsDragDrop;
			this.treeViewNavigations.DragOver +=  OnTreeViewNavigationsDragOver;
			this.treeViewNavigations.DragLeave +=  OnTreeViewNavigationsDragLeave;
			this.treeViewNavigations.GotFocus += OnTreeViewNavigationsGotFocus;
			this.treeViewNavigations.KeyDown += OnTreeViewNavigationsKeyDown;
			this.treeViewNavigations.LostFocus +=  OnTreeViewNavigationsLostFocus;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(175, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(5, 360);
			this.splitter1.TabIndex = 11;
			this.splitter1.TabStop = false;
			this.btMoveUp.AllowFocus = false;
			this.btMoveUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btMoveUp.Location = new System.Drawing.Point(72, 4);
			this.btMoveUp.Name = "btMoveUp";
			this.btMoveUp.Size = new System.Drawing.Size(30, 30);
			this.btMoveUp.TabIndex = 2;
			this.btMoveUp.TabStop = false;
			this.btMoveUp.ToolTip = "Move Toward Beginning";
			this.btMoveUp.Click += new System.EventHandler(this.btMoveUp_Click);
			this.btMoveDown.AllowFocus = false;
			this.btMoveDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btMoveDown.Location = new System.Drawing.Point(108, 4);
			this.btMoveDown.Name = "btMoveDown";
			this.btMoveDown.Size = new System.Drawing.Size(30, 30);
			this.btMoveDown.TabIndex = 1;
			this.btMoveDown.TabStop = false;
			this.btMoveDown.ToolTip = "Move Toward End";
			this.btMoveDown.Click += new System.EventHandler(this.btMoveDown_Click);
			this.groupControl1.Controls.Add(this.treeViewNavigations);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl1.Size = new System.Drawing.Size(175, 360);
			this.groupControl1.TabIndex = 4;
			this.groupControl1.Text = "NavBar Groups:";
			this.groupControl2.Controls.Add(this.treeViewItems);
			this.groupControl2.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupControl2.Location = new System.Drawing.Point(180, 0);
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl2.Size = new System.Drawing.Size(180, 360);
			this.groupControl2.TabIndex = 5;
			this.groupControl2.Text = "NavBar Items:";
			this.Name = "GroupItemDesigner";
			this.Size = new System.Drawing.Size(762, 460);
			this.Load += new System.EventHandler(this.GroupItemDesigner_Load);
			this.Resize += new System.EventHandler(this.GroupItemDesigner_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		void OnTreeViewItemsSelectionChanged(object sender, EventArgs e) {
			TreeNode[] indexes = treeViewItems.SelNodes;
			if(indexes == null || indexes.Length == 0) return;
			object[] list = new Object[treeViewItems.SelNodes.Length];
			for(int i = 0; i < indexes.Length; i++)
				list[i] = indexes[i].Tag;
			pgMain.SelectedObjects = list;
			if(SelectionService != null) SelectionService.SetSelectedComponents(list, SelectionTypes.Replace);
			CheckButtons();
		}
		private NavBarControl NavBar { get { return EditingObject as NavBarControl; } }
		protected override void InitImages() {
			base.InitImages();
			btAdd.Image = DesignerImages16.Images[DesignerImages16AddIndex];
			btRemove.Image = DesignerImages16.Images[DesignerImages16RemoveIndex];
			btMoveUp.Image = DesignerImages16.Images[DesignerImages16UpIndex];
			btMoveDown.Image = DesignerImages16.Images[DesignerImages16DownIndex];
		}
		public GroupItemDesigner() : base(0) {
			InitializeComponent();
			pgMain.BringToFront();
		}
		public override void InitComponent() {
			lbCaption.Text = "Groups / Items / Links Designer";
			IComponentChangeService cs = NavBarGetService((typeof(IComponentChangeService))) as IComponentChangeService;
			if(cs != null) cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			NavBar.Groups.CollectionItemChanged += new CollectionItemEventHandler(OnGroupCollectionChanged);
			NavBar.Items.CollectionItemChanged += new CollectionItemEventHandler(OnItemCollectionChanged);
			FillData();
		}
		protected void DeInit() {
			if(NavBar == null) return;
			IComponentChangeService cs = NavBarGetService((typeof(IComponentChangeService))) as IComponentChangeService;
			if(cs != null) cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			NavBar.Groups.CollectionItemChanged -= new CollectionItemEventHandler(OnGroupCollectionChanged);
			NavBar.Items.CollectionItemChanged -= new CollectionItemEventHandler(OnItemCollectionChanged);
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
			TreeNode node = FindGroup(e.Component as NavBarGroup);
			TreeNode nodeItem = FindNodeItem(e.Component as NavBarItem);
			TreeNode lvItem = FindItem(e.Component as NavBarItem);
			if(node != null) node.Text = GetNavElementText(e.Component as NavElement);
			if(nodeItem != null) nodeItem.Text = GetNavElementText(e.Component as NavElement);
			if(lvItem != null) lvItem.Text = GetNavElementText(e.Component as NavElement);
		}
		protected TreeNode FindGroup(NavBarGroup group) {
			if(group == null) return null;
			foreach(TreeNode node in treeViewNavigations.Nodes) {
				if(node.Tag == group) return node;
			}
			return null;
		}
		protected TreeNode FindItem(NavBarItem item) {
			if(item == null) return null;
			foreach(TreeNode lvItem in treeViewItems.Nodes) {
				if(lvItem.Tag == item) return lvItem;
			}
			return null;
		}
		protected TreeNode FindNodeItem(NavBarItem item) {
			if(item == null) return null;
			foreach(TreeNode nodeGroup in treeViewNavigations.Nodes) {
				foreach(TreeNode node in nodeGroup.Nodes) {
					NavBarItemLink link = node.Tag as NavBarItemLink;
					if(link != null && link.Item == item) return node;
				}
			}
			return null;
		}
		protected void OnGroupCollectionChanged(object sender, CollectionItemEventArgs e) {
		}
		protected void OnItemCollectionChanged(object sender, CollectionItemEventArgs e) {
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DeInit();
			}
			base.Dispose(disposing);
		}
		protected virtual void FillData() {
			FillTreeView(null);
			FillItems(null);
		}
		Hashtable savedNodes = new Hashtable();
		bool loading = false;
		protected virtual void FillTreeView(NavBarGroup selectedGroup) {
			this.loading = true;
			try {
				savedNodes.Clear();
				foreach(TreeNode node in treeViewNavigations.Nodes) {
					if(node.IsExpanded) savedNodes[node.Tag] = 0;
				}
				treeViewNavigations.BeginUpdate();
				treeViewNavigations.Nodes.Clear();
				try {
					foreach(NavBarGroup group in NavBar.Groups) {
						TreeNode node = new TreeNode(GetNavElementText(group), 0, 0);
						node.Tag = group;
						if(savedNodes.Count == 0) {
							if(group.Expanded) node.Expand();
						} else {
							if(savedNodes.Contains(group)) node.Expand();
						}
						AddLinks(node, group);
						treeViewNavigations.Nodes.Add(node);
						if(group == selectedGroup) {
							treeViewNavigations.SelectedNode = node;
							node.Expand();
						}
					}
				}
				finally {
					treeViewNavigations.EndUpdate();
				}
			}
			finally {
				this.loading = false;
			}
		}
		private void FillItems(NavBarItem selectedItem) {
			treeViewItems.BeginUpdate();
			try {
				treeViewItems.Nodes.Clear();
				foreach(NavBarItem item in NavBar.Items) {
					TreeNode lvItem = treeViewItems.Nodes.Add(GetNavElementText(item));
					lvItem.ImageIndex = 1;
					lvItem.Tag = item;
					if(selectedItem == item) treeViewItems.SelectedNode = lvItem;
				}
			}
			finally {
				treeViewItems.EndUpdate();
			}
		}
		protected string GetNavElementText(NavElement element) {
			return string.Format("{0} [{1}]", element.Name, element.Caption);
		}
		protected void AddLinks(TreeNode node, NavBarGroup group) {
			foreach(NavBarItemLink link in group.ItemLinks) {
				TreeNode childNode = new TreeNode(GetNavElementText(link.Item), 1, 1);
				childNode.Tag = link;
				node.Nodes.Add(childNode);
			}
		}
		Point mDown = new Point(-10000, -10000);
		private void OnTreeViewItemsMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			mDown = new Point(e.X, e.Y);
		}
		private void OnTreeViewItemsMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			Point mDown = new Point(-10000, -10000);
		}
		private void OnTreeViewItemsMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if((Math.Abs(e.X - mDown.X) > 5 || Math.Abs(e.Y - mDown.Y) > 5)) {
				int selectedItemsCount = treeViewItems.SelNodes.Length;
				if(selectedItemsCount < 0 || e.Button != MouseButtons.Left) return;
				treeViewItems.DoDragDrop(new DragInfo(treeViewItems.SelNodes, NavBar.Items), DragDropEffects.Copy);
			}
		}
		private void OnTreeViewItemsKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) RemoveItems();
			if(e.KeyCode == Keys.Insert) AddItem();
		}
		private void OnTreeViewNavigationsKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) 
				RemoveNodes(treeViewNavigations.SelNodes);
			if(e.KeyCode == Keys.Insert) 
				AddGroup();
		}
		private void SetNodeExpand(TreeNode node, bool expand) {
			NavBarGroup group = node.Tag as NavBarGroup;
			if(group != null) group.Expanded = expand;
		}
		private void OnTreeViewNavigationsAfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(!this.loading) {
				if(e.Node.Tag is NavBarGroup)
					SetNodeExpand(e.Node, true);
			}
		}
		private void OnTreeViewNavigationsAfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(e.Node.Tag is NavBarGroup)
				SetNodeExpand(e.Node, false);
		}
		private void OnTreeViewNavigationsDragNodeGetObject(object sender, DevExpress.Utils.Design.TreeViewGetDragObjectEventArgs e) {
			TreeNode[] selNodes = e.DragObject as TreeNode[];
			if(selNodes == null || selNodes.Length == 0) return;
			NavBarItemLink[] links = new NavBarItemLink[selNodes.Length];
			for(int n = 0; n < selNodes.Length; n++) {
				links[n] = selNodes[n].Tag as NavBarItemLink;
			}
			e.DragObject = new DragInfo(links);
		}
		private void OnTreeViewNavigationsDragNodeAllow(object sender, System.ComponentModel.CancelEventArgs e) {
			TreeNode node = treeViewNavigations.SelNode;
			e.Cancel = node == null || !(node.Tag is NavBarItemLink);
		}
		private void OnTreeViewNavigationsAllowMultiSelectNode(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			e.Cancel = !(e.Node.Tag is NavBarItemLink);
		}
		public const int ImageGroup = 0, ImageItem = 1, ImageAdd = 2, ImageInsert = 3;
		int GetNodeImageIndex(TreeNode node) {
			if(node != null) return (node.Tag is NavBarGroup) ? ImageGroup : ImageItem;
			return -1;
		}
		DragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(DragInfo)) as DragInfo;
		}
		TreeNode dropTargetNode = null, dropSelectedNode = null;
		protected void ResetNodeImage(TreeNode node) { SetNodeImage(node, GetNodeImageIndex(node)); }
		protected void SetNodeImage(TreeNode node, int index) {
			if(node == null) return;
			node.SelectedImageIndex = node.ImageIndex = index;
		}
		private void OnTreeViewNavigationsDragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			DragInfo info = GetDragObject(e.Data);
			if(info == null) return;
			TreeNode node =	treeViewNavigations.GetNodeAt(treeViewNavigations.PointToClient(new Point(e.X, e.Y)));
			int setIndex = -1;
			if(node == null) {
				return;
			}
			if(node == this.dropSelectedNode && this.dropTargetNode == node) {
				return;
			} 
			if(info.IsLinkDragging) {
				NavBarGroup destGroup = GetNodeGroup(node);
				setIndex = node.Parent == null ? ImageAdd : ImageInsert;
				if(destGroup == info.Link.Group) {
					if(setIndex == ImageAdd) setIndex = -1;
					else {
						int prevIndex = destGroup.ItemLinks.IndexOf(info.Link);
						int newIndex = destGroup.ItemLinks.IndexOf(node.Tag as NavBarItemLink);
						if(prevIndex == newIndex || newIndex - 1 == prevIndex) setIndex = -1;
					}
				}
			} else {
				setIndex = node.Parent == null ? ImageAdd : ImageInsert;
			}
			this.dropSelectedNode = node;
			ResetNodeImage(this.dropTargetNode);
			this.dropTargetNode = node;
			if(setIndex != -1) SetNodeImage(this.dropTargetNode, setIndex);
			if(setIndex == -1)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = info.IsLinkDragging ? DragDropEffects.Move : DragDropEffects.Copy;
			SetActiveGroup(this.dropTargetNode);
		}
		NavBarGroup GetNodeGroup(TreeNode node) {
			if(node == null) return null;
			if(node.Parent != null) node = node.Parent;
			return node.Tag as NavBarGroup;
		}
		private void OnTreeViewNavigationsDragLeave(object sender, System.EventArgs e) {
			SetDefaultCursor();
		}
		void SetDefaultCursor() {
			ResetNodeImage(this.dropTargetNode);
			this.dropTargetNode = null;
			Cursor = Cursors.Default;
		}
		void SetActiveGroup(TreeNode node) {
			if(node == null) return;
			NavBar.ActiveGroup = node.Tag as NavBarGroup;
		}
		void OnTreeViewNavigationsDragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			NavBarGroup destGroup = GetNodeGroup(this.dropTargetNode);
			DragInfo info = GetDragObject(e.Data);
			if(destGroup == null || info == null) return;
			int destIndex = 0;
			if(!(this.dropTargetNode.Tag is NavBarGroup)) destIndex = destGroup.ItemLinks.IndexOf(dropTargetNode.Tag as NavBarItemLink);
			if(info.IsLinkDragging) {
				foreach(NavBarItemLink link in info.DragLinks) {
					NavBarItem item = link.Item;
					link.Dispose();
					destGroup.ItemLinks.Insert(destIndex, item);
				}
			} else {
				foreach(NavBarItem item in info.Items) {
					destGroup.ItemLinks.Insert(destIndex, item);
				}
			}
			FillTreeView(destGroup);
			if(destGroup != null) destGroup.Expanded = true;
			SetDefaultCursor();
		}
		private void OnTreeViewGotFocus(object sender, EventArgs e) {
			CheckButtons();
		}
		private void OnTreeViewNavigationsLostFocus(object sender, EventArgs e) {
			CheckButtons();
		}
		private void OnTreeViewNavigationsGotFocus(object sender, EventArgs e) {
			if(treeViewNavigations.SelCount > 0) {
				OnTreeViewNavigationsSelectionChanged(this, EventArgs.Empty);
			}
			CheckButtons();
		}
		private void lvItems_SelectedIndexChanged(object sender, System.EventArgs e) {
		}
		object NavBarGetService(Type type) {
			if(NavBar.Site == null) return null;
			return NavBar.Site.GetService(type);
		}
		ISelectionService SelectionService { get { return NavBarGetService(typeof(ISelectionService)) as ISelectionService; } }
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("GroupsPanel", groupControl2.Width);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			groupControl2.Width = localStore.RestoreIntProperty("GroupsPanel", groupControl2.Width);
			base.RestoreLocalProperties(localStore);
		}
		protected override bool AllowGlobalStore { get { return false; } }
		protected bool ButtonOpearateGroups {
			get { return treeViewNavigations.ContainsFocus; }
		}
		protected void AddGroup() {
			NavBarGroup group = NavBar.Groups.Add();
			FillTreeView(group);
		}
		protected void AddItem() {
			NavBarItem item = NavBar.Items.Add();
			FillItems(item);
		}
		private void btAdd_Click(object sender, System.EventArgs e) {
			BeginTreeViewUpdate();
			if(ButtonOpearateGroups)
				AddGroup();
			else
				AddItem();
			EndTreeViewUpdate();
		}
		protected void BeginTreeViewUpdate() {
			if(ButtonOpearateGroups) {
				treeViewNavigations.BeginSelection();
				treeViewNavigations.SelectedNode = null;
			}
			else {
				treeViewItems.BeginSelection();
				treeViewItems.SelectedNode = null;
			}
		}
		protected void EndTreeViewUpdate() {
			if(ButtonOpearateGroups)
				treeViewNavigations.EndSelection();
			else
				treeViewItems.EndSelection();
		}
		protected void CheckButtons() {
			btAdd.ToolTip = ButtonOpearateGroups ? "Add Group" : "Add Item";
			if(ButtonOpearateGroups) {
				bool isLink = treeViewNavigations.SelNode != null && (treeViewNavigations.SelNode.Tag is NavBarItemLink);
				btRemove.ToolTip = isLink ? "Remove Link" : "Remove Group";
				btRemove.Enabled = treeViewNavigations.SelNode != null;
				if(treeViewNavigations.SelNode != null) {
					btRemove.Enabled = AllowRemoveGroup(treeViewNavigations.SelNode.Tag);
					int index = treeViewNavigations.Nodes.IndexOf(treeViewNavigations.SelNode);
					btMoveUp.Enabled = index > 0;
					btMoveDown.Enabled = index > -1 && index < treeViewNavigations.Nodes.Count - 1;
				}
				btMoveUp.Visible = btMoveDown.Visible = !isLink && treeViewNavigations.SelNode != null;
			} else {
				btRemove.ToolTip = "Remove Item";
				btRemove.Enabled = treeViewItems.SelNodes.Length > 0 && AllowRemoveItem(treeViewItems.SelNodes[0].Tag);
				btMoveUp.Visible = btMoveDown.Visible = false;
			}
		}
		protected bool AllowRemoveGroup(object group) {
			if(group == null) return false;
			return InheritanceHelper.AllowCollectionItemRemove(NavBar, TypeDescriptor.GetProperties(NavBar)["Groups"], NavBar.Groups, group);
		}
		protected bool AllowRemoveItem(object item) {
			if(item == null) return false;
			return InheritanceHelper.AllowCollectionItemRemove(NavBar, TypeDescriptor.GetProperties(NavBar)["Items"], NavBar.Items, item);
		}
		protected void RemoveNodes(TreeNode[] nodes) {
			if(nodes == null || nodes.Length == 0) return;
			NavBar.BeginUpdate();
			try {
				treeViewNavigations.BeginSelection();
				try {
					for(int n = nodes.Length - 1; n >= 0; n--) {
						TreeNode node = nodes[n];
						NavBarGroup group = node.Tag as NavBarGroup;
						NavBarItemLink link = node.Tag as NavBarItemLink;
						if(group != null && !AllowRemoveGroup(group)) continue;
						if(link != null) link.Dispose();
						if(group != null) group.Dispose();
						node.Remove();
					}
					treeViewNavigations.UpdateSelection();
				}
				finally {
					treeViewNavigations.EndSelection();
				}
			}
			finally {
				NavBar.EndUpdate();
			}
			if(treeViewNavigations.Nodes.Count == 0) {
				ClearSelection();
			}
		}
		protected void ClearSelection() {
			pgMain.SelectedObject = null;
			if(SelectionService != null) SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
			CheckButtons();
		}
		protected void RemoveItems() {
			TreeNode[] indexes = treeViewItems.SelNodes;
			if(indexes == null || indexes.Length == 0) return;
			int oldIndex = treeViewItems.Nodes.IndexOf(indexes[0]);
			for(int i = 0; i < indexes.Length; i++) {
				NavBarItem item = indexes[i].Tag as NavBarItem;
				if(AllowRemoveItem(item)) item.Dispose();
			}
			FillItems(null);
			if(treeViewItems.Nodes.Count > 0) {
				treeViewItems.SelectedNode = treeViewItems.Nodes[(oldIndex < treeViewItems.Nodes.Count ? oldIndex : treeViewItems.Nodes.Count - 1)];
			} else {
				ClearSelection();
			}
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			BeginTreeViewUpdate();
			if(ButtonOpearateGroups) {
				RemoveNodes(treeViewNavigations.SelNodes);
			} else {
				RemoveItems();
			}
			EndTreeViewUpdate();
		}
		protected override string DescriptionText { get { return "In order to add an item link to the required group, drag the selected item from the 'NavBar Items' list and drop it onto the 'NavBar Groups' panel. To delete an item link from a group, press the Del key when the required node is selected within the 'NavBar Groups' panel."; } }
		private void GroupItemDesigner_Load(object sender, System.EventArgs e) {
			treeViewNavigations.Focus();
			CheckButtons();
		}
		void MoveGroup(NavBarGroup group, int delta) {
			if(group == null) return;
			int index = NavBar.Groups.IndexOf(group);
			NavBar.Groups.Move(index, index + delta);
			FillTreeView(group);
			CheckButtons();
		}
		private void btMoveUp_Click(object sender, System.EventArgs e) {
			MoveGroup(treeViewNavigations.SelNode.Tag as NavBarGroup, -1);
		}
		private void btMoveDown_Click(object sender, System.EventArgs e) {
			MoveGroup(treeViewNavigations.SelNode.Tag as NavBarGroup, 1);
		}
		private void OnTreeViewNavigationsSelectionChanged(object sender, System.EventArgs e) {
			if(treeViewNavigations.SelCount == 0) return;
			CheckButtons();
			TreeNode[] selNodes = treeViewNavigations.SelNodes;
			object[] objs = new object[selNodes.Length];
			for(int n = 0; n < selNodes.Length; n++) {
				NavBarItemLink link = selNodes[n].Tag as NavBarItemLink;
				object obj = selNodes[n].Tag;
				if(link != null) obj = link.Item;
				objs[n] = obj;
			}
			pgMain.SelectedObjects = objs;
			if(SelectionService != null) SelectionService.SetSelectedComponents(objs, SelectionTypes.Replace);
		}
		void OnResize() {
			if(pgMain.Width < 50 || pnlMain.Width < 50) pnlMain.Width = pgMain.Width = this.Width / 2;
			if(groupControl1.Width < 50 || groupControl2.Width < 50) groupControl1.Width = groupControl2.Width = pnlMain.Width / 2;
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
	}
}
