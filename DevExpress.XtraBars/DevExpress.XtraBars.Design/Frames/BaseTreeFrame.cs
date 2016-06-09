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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	[CLSCompliant(false)]
	public class BaseTreeFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public DevExpress.Utils.Design.DXTreeView tree;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private System.ComponentModel.IContainer components = null;
		public BaseTreeFrame() {
			InitializeComponent();
			CreateImages();			
			groupControl1.Text = GroupCaption;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.tree = new DevExpress.Utils.Design.DXTreeView();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.pgMain.Location = new System.Drawing.Point(167, 60);
			this.pgMain.Size = new System.Drawing.Size(289, 252);
			this.pnlControl.TabIndex = 1;
			this.pnlMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.groupControl1});
			this.horzSplitter.Visible = false;
			this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tree.HideSelection = false;
			this.tree.ImageIndex = -1;
			this.tree.LabelEdit = true;
			this.tree.Location = new System.Drawing.Point(4, 23);
			this.tree.Name = "tree";
			this.tree.SelectedImageIndex = -1;
			this.tree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.Standard;
			this.tree.Size = new System.Drawing.Size(152, 225);
			this.tree.TabIndex = 0;
			this.tree.GetNodeEditText += new TreeViewGetNodeEditTextEventHandler(tree_GetNodeEditText);
			this.tree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
			this.tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tree_KeyDown);
			this.tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDown);
			this.tree.DragOver += new System.Windows.Forms.DragEventHandler(this.tree_DragOver);
			this.tree.DragLeave += new System.EventHandler(this.tree_DragLeave);
			this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.tree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
			this.tree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tree_MouseMove);
			this.tree.DragDrop += new System.Windows.Forms.DragEventHandler(this.tree_DragDrop);
			this.tree.AllowSkinning = true;
			this.groupControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.tree});
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.DockPadding.All = 2;
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(160, 252);
			this.groupControl1.TabIndex = 1;
			this.groupControl1.Text = "Tree";
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.horzSplitter,
																		  this.pgMain,
																		  this.splMain,
																		  this.pnlMain,
																		  this.pnlControl,
																		  this.lbCaption});
			this.Name = "BaseTreeFrame";
			this.Size = new System.Drawing.Size(412, 288);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual string GroupCaption { get { return ""; } }
		public DXTreeView Tree { get { return tree; } }
		[Browsable(false)]
		public BarManager Manager {
			get {
				if(EditingObject == null) return null;
				RibbonControl ribbon = EditingObject as RibbonControl;
				if(ribbon != null) return ribbon.Manager;
				return EditingObject as BarManager;
			}
		}
		public bool AllowDropTree {
			get { return tree.AllowDrop; }
			set { tree.AllowDrop = value; }
		}
		private void CreateImages() {
			tree.ImageList = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources("DevExpress.XtraBars.Design.Frames.BarsIcons.bmp", typeof(BaseTreeFrame).Assembly, new Size(16, 16), Color.Magenta, ColorDepth.Depth32Bit);
		}
		protected virtual TreeNode AddBar(TreeNodeCollection nodes, Bar bar) {
			TreeNode node = new TreeNode();
			node.Tag = bar;
			node.Text = GetNodeCaption(node, false);
			node.ImageIndex = node.SelectedImageIndex = bar == bar.Manager.MainMenu ? 1 : 0;
			nodes.Add(node);
			AddItems(node.Nodes, bar as BarLinksHolder);
			OnNodeAdded(node);
			return node;
		}
		protected virtual void AddItems(TreeNodeCollection nodes, BarLinksHolder holder) {
			if(holder == null) return;
			foreach(BarItemLink link in holder.ItemLinks) {
				AddItemLink(nodes, link);
			}
		}
		private int LinkContainerItemIcon(BarItem item) {
			return item is BarLinkContainerItem ? 2 : 3;
		}
		protected virtual void AddItemLink(TreeNodeCollection nodes, BarItemLink link) {
			if(link == null) return;
			TreeNode node = new TreeNode(GetItemCaption(link.Item, false));
			node.Tag = link;
			node.ImageIndex = node.SelectedImageIndex = LinkContainerItemIcon(link.Item);
			nodes.Add(node);
			BarLinksHolder holder = link.Item as BarLinksHolder;
			if(holder != null) AddItems(node.Nodes, holder);
			OnNodeAdded(node);
		}
		protected virtual void AddItem(TreeNodeCollection nodes, BarItem item, bool withSubItems) {
			if(item == null) return;
			TreeNode node = new TreeNode(GetItemCaption(item, false));
			node.Tag = item;
			node.ImageIndex = node.SelectedImageIndex = LinkContainerItemIcon(item);
			nodes.Add(node);
			if(withSubItems) {
				BarLinksHolder holder = item as BarLinksHolder;
				if(holder != null) AddItems(node.Nodes, holder);
			}
			OnNodeAdded(node);
		}
		protected virtual void OnNodeAdded(TreeNode node) {
			if(this.expandedItems.ContainsKey(GetNodeKey(node))) 
				node.Expand();
		}
		public override void DoInitFrame() {
			Populate();
		}
		Hashtable expandedItems = new Hashtable();
		protected string GetNodeKey(TreeNode node) {
			int level = 0;
			TreeNode pnode = node;
			while(pnode.Parent != null) {
				level ++;
				pnode = pnode.Parent;
			}
			return string.Format("{0}:{1}{2}", level, node.Tag == null ? null : node.Tag.GetType(), node.Tag == null ? 0 : node.Tag.GetHashCode());
		}
		protected virtual void ClearExpandedItems() {
			expandedItems.Clear();
		}
		protected virtual void SaveExpandedItems() {
			ClearExpandedItems();
			SaveExpandedNodes(Tree.Nodes);
		}
		void SaveExpandedNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				if(!node.IsExpanded) continue;
				this.expandedItems[GetNodeKey(node)] = true;
				SaveExpandedNodes(node.Nodes);
			}
		}
		public virtual void Repopulate() {
			SaveExpandedItems();
			Populate();
		}
		public virtual void Populate() {
		}
		public virtual void RefreshTree() {
			Tree.BeginUpdate();
			try {
				RefreshNodes(Tree.Nodes);
			}
			finally {
				Tree.EndUpdate();
			}
		}
		protected void RefreshNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				node.Text = GetNodeCaption(node, false);
				RefreshNodes(node.Nodes);
			}
		}
		protected virtual string GetNodeCaption(TreeNode node, bool isEditing) {
			BarItemLink link = node.Tag as BarItemLink;
			BarItem item = node.Tag as BarItem;
			Bar bar = node.Tag as Bar;
			if(link != null) return GetItemCaption(link.Item, isEditing);
			if(item != null) return GetItemCaption(item, isEditing);
			if(bar != null) return isEditing ? bar.Text : string.Format("{0}  <{1}>", bar.Text, bar.BarName);
			return "";
		}
		string GetItemCaption(BarItem item, bool isEditing) {
			if(isEditing) return item.Caption;
			if(item.Caption == string.Empty) return string.Format("<{0}>", item.Name);
			return string.Format("{0}  <{1}>", item.Caption, item.Name);
		}
		protected virtual void SetLinkCaption(TreeNode node, string label) {
			BarItemLink link = node.Tag as BarItemLink;
			BarItem item = node.Tag as BarItem;
			Bar bar = node.Tag as Bar;
			BarManagerCategory category = node.Tag as BarManagerCategory;
			if(link != null) link.Item.Caption = label; 
			if(item != null) item.Caption = label;
			if(bar != null) bar.Text = label;
			if(category != null) category.Name = label;
		}
		protected object GetNodeObject(TreeNode node) {
			if(node == null || node.Tag == null) return null;
			BarItemLink link = node.Tag as BarItemLink;
			if(link != null) return link.Item; 
			return node.Tag;
		}
		protected virtual bool IsAllowEditObject(object val) {
			return true;
		}
		protected virtual bool IsAllowSelectObject(object val) {
			return true;
		}
		protected virtual void tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			if(e.Node == null) return;
			if(IsAllowSelectObject(GetNodeObject(e.Node))) {
				pgMain.SelectedObject = GetNodeObject(e.Node);
			} else
				pgMain.SelectedObject = null;
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			if(e.ChangedItem == null) return;
			if(e.ChangedItem.Label == "(Name)" || e.ChangedItem.Label == "Name" || e.ChangedItem.Label == "Text" || e.ChangedItem.Label == "Caption" || e.ChangedItem.Label == "UserCaption") {
				RefreshTree();
			}
		}
		protected virtual void tree_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			if(e.Label != "" && e.Label != null) {
				try {
					SetLinkCaption(e.Node, e.Label);
				}
				catch(Exception ex) {
					e.CancelEdit = true;
					XtraMessageBox.Show(LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				pgMain.Refresh();
			}
			e.CancelEdit = true;
			e.Node.Text = GetNodeCaption(e.Node, false);
		}
		protected virtual void tree_GetNodeEditText(object sender, TreeViewGetNodeEditTextEventArgs e) {
			e.Text = GetNodeCaption(e.Node, true);
		}
		protected virtual void tree_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			e.CancelEdit = !IsAllowEditObject(GetNodeObject(e.Node));
		}
		protected virtual string CreateBarEditForm(bool newToolbar, string caption, string name) {
			DevExpress.XtraBars.Customization.EditForm editForm = new DevExpress.XtraBars.Customization.EditForm();
			editForm.SuspendLayout();
			editForm.Text = "New " + name;
			editForm.lbCaption.Text = name + " name:";
			editForm.ShowInTaskbar = false;
			editForm.ControlBox = false;
			editForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			editForm.MaximizeBox = editForm.MinimizeBox = false;
			editForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			editForm.tbName.Text = caption;
			editForm.ResumeLayout(false);
			DialogResult res = editForm.ShowDialog(this);
			string ret = res == DialogResult.OK ? editForm.tbName.Text : null;
			return ret;
		}
		protected virtual void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
		}
		protected virtual void tree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
		}
		protected virtual void tree_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
		}
		protected virtual void tree_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
		}
		protected virtual void tree_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
		}
		protected virtual void tree_DragLeave(object sender, System.EventArgs e) {
		}
	}
	public class CategoriesHelper {
		public virtual string CreateBarEditForm(DevExpress.XtraEditors.Designer.Utils.XtraPGFrame frame, bool newToolbar, string caption, string name) {
			DevExpress.XtraBars.Customization.EditForm editForm = new DevExpress.XtraBars.Customization.EditForm();
			editForm.SuspendLayout();
			editForm.Text = "New " + name;
			editForm.lbCaption.Text = name + " name:";
			editForm.ShowInTaskbar = false;
			editForm.ControlBox = false;
			editForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			editForm.MaximizeBox = editForm.MinimizeBox = false;
			editForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			editForm.tbName.Text = caption;
			editForm.ResumeLayout(false);
			DialogResult res = editForm.ShowDialog(frame);
			string ret = res == DialogResult.OK ? editForm.tbName.Text : null;
			return ret;
		}
	}
}
