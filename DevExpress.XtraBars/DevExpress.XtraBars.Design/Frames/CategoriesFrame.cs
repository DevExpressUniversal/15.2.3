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
using DevExpress.XtraEditors;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	[CLSCompliant(false)]
	public class CategoriesFrame : DevExpress.XtraBars.Design.Frames.BaseTreeFrame {
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		protected override string GroupCaption { get { return "Categories:"; } }
		protected override string DescriptionText { get { return "You can add / delete categories and customize categories and items. Use drag&drop to move items between categories."; } }
		public CategoriesFrame() {
			InitializeComponent();
			AllowDropTree = true;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing )	{
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.tree.AllowDrag = true;
			this.tree.AllowDrop = true;
			this.tree.LineColor = System.Drawing.Color.Black;
			this.tree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.tree.Size = new System.Drawing.Size(152, 169);
			this.tree.DragNodeGetObject += new DevExpress.Utils.Design.TreeViewGetDragObjectEventHandler(this.tree_DragNodeGetObject);
			this.tree.DragNodeAllow += new System.ComponentModel.CancelEventHandler(this.tree_DragNodeAllow);
			this.tree.AllowMultiSelectNode += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_AllowMultiSelectNode);
			this.tree.AllowSkinning = true;
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Controls.Add(this.btnAdd);
			this.pnlControl.Location = new System.Drawing.Point(0, 38);
			this.pnlControl.Size = new System.Drawing.Size(415, 54);
			this.pgMain.Location = new System.Drawing.Point(165, 92);
			this.pgMain.Size = new System.Drawing.Size(250, 196);
			this.splMain.Location = new System.Drawing.Point(160, 92);
			this.splMain.Size = new System.Drawing.Size(5, 196);
			this.lbCaption.Size = new System.Drawing.Size(415, 42);
			this.pnlMain.Location = new System.Drawing.Point(0, 92);
			this.pnlMain.Size = new System.Drawing.Size(160, 196);
			this.horzSplitter.Location = new System.Drawing.Point(165, 92);
			this.horzSplitter.Size = new System.Drawing.Size(250, 4);
			this.btnDelete.Enabled = false;
			this.btnDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDelete.Location = new System.Drawing.Point(36, 4);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(30, 30);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.ToolTip = "Delete Category";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnAdd.Location = new System.Drawing.Point(0, 4);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(30, 30);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.ToolTip = "Add New Category";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.Name = "CategoriesFrame";
			this.Size = new System.Drawing.Size(415, 288);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override void InitImages() {
			base.InitImages();
			btnAdd.Image = DesignerImages16.Images[DesignerImages16AddIndex];
			btnDelete.Image = DesignerImages16.Images[DesignerImages16RemoveIndex];
		}
		public override void Populate() {
			if(Manager == null) return;
			Tree.BeginUpdate();
			try {
				Tree.Nodes.Clear();
				AddCategory(Tree.Nodes, null);
				AddCategory(Tree.Nodes, BarManagerCategory.DefaultCategory);
				foreach(BarManagerCategory category in Manager.Categories) {
					AddCategory(Tree.Nodes, category);
				}
			}
			finally {
				Tree.EndUpdate();
			}
		}
		protected override string GetNodeCaption(TreeNode node, bool isEditing) {
			object val = GetNodeObject(node);
			BarManagerCategory category = val as BarManagerCategory;
			if(category != null) {
				if(category.Equals(BarManagerCategory.DefaultCategory)) return "[Unassigned items]";
				return category.Name;
			}
			if(node.Tag == null) return "[All items]";
			return base.GetNodeCaption(node, isEditing);
		}
		protected override bool IsAllowEditObject(object val) {
			if((val != null && val.Equals(BarManagerCategory.DefaultCategory)) || val == null) return false;
			if(val is BarManagerCategory) return true;
			return false;
		}
		private int GetCategoryIndex(BarManagerCategory category) {
			return IsAllowEditObject(category) ? 4 : 5;
		}
		protected virtual TreeNode AddCategory(TreeNodeCollection nodes, BarManagerCategory category) {
			TreeNode node = new TreeNode();
			node.Tag = category;
			node.Text = GetNodeCaption(node, false);
			node.SelectedImageIndex = node.ImageIndex = GetCategoryIndex(category);
			nodes.Add(node);
			AddCategoryItems(node.Nodes, category);
			OnNodeAdded(node);
			return node;
		}
		protected virtual void AddCategoryItems(TreeNodeCollection nodes, BarManagerCategory category) {
			foreach(BarItem item in Manager.Items) {
				if(category != null && item.CategoryGuid != category.Guid) continue;
				AddItem(nodes, item, false);
			}
		}
		private void AddCategory() {
			string caption = CreateBarEditForm(true, "", "Category");
			if(caption != null) {
				BarManagerCategory category = null;
				try {
					category = Manager.Categories.Add(caption);
				}
				catch(Exception ex) {
					XtraMessageBox.Show(LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				category.Visible = true;
				Tree.SelectedNode = AddCategory(Tree.Nodes, category);
				Repopulate();
			}
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			AddCategory();
		}
		private void SetDeleteEnabled(TreeNode node) {
			btnDelete.Enabled = node != null && node.Tag is BarManagerCategory && !node.Tag.Equals(BarManagerCategory.DefaultCategory);
		}
		protected override void tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			base.tree_AfterSelect(sender, e);
			SetDeleteEnabled(e.Node);
		}
		private void DeleteCategory() {
			if(Tree.SelectedNode == null) return;
			BarManagerCategory category = Tree.SelectedNode.Tag as BarManagerCategory;
			if(category != null && !category.Equals(BarManagerCategory.DefaultCategory)) {
				if(MessageBox.Show(string.Format("Are you sure you want to delete '{0}' category?", category.Name), "XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
					Manager.Categories.Remove(category);
					Tree.Nodes.Remove(Tree.SelectedNode);
					SetDeleteEnabled(Tree.SelectedNode);
					int i = Tree.SelectedNode.Index;
					Repopulate();
					Tree.SelectedNode = Tree.Nodes[i];
				}
			}
		}
		private void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteCategory();
		}
		protected override void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) DeleteCategory();
			if(e.KeyCode == Keys.Insert) AddCategory();
		}
		TreeNode selectedNode = null;
		protected override void tree_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			BarItem[] items = GetDragItems(e.Data);
			if(items == null) return;
			Point dragPoint = Tree.PointToClient(new Point(e.X, e.Y));
			TreeNode node = Tree.GetNodeAt(dragPoint);
			if(selectedNode != node) 
				if(selectedNode != null) {
					int index = GetCategoryIndex(selectedNode.Tag as BarManagerCategory);
					if(selectedNode.ImageIndex != index) selectedNode.ImageIndex = index;
				}
			BarItem item = items[0];
			if(node != null && node.Tag is BarManagerCategory) {
				BarManagerCategory category = node.Tag as BarManagerCategory;
				if(item.Category.Guid != category.Guid) {
					e.Effect = DragDropEffects.Move;
					if(node.ImageIndex != 6) node.ImageIndex = 6;
					selectedNode = node;
					return;
				}
			}
			e.Effect = DragDropEffects.None;
		}
		protected override void tree_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			BarItem[] items = GetDragItems(e.Data);
			if(selectedNode == null || items == null || e.Effect == DragDropEffects.None) return;
			BarManagerCategory category = selectedNode.Tag as BarManagerCategory;
			if(category == null) return;
			int index = GetCategoryIndex(category);
			if(selectedNode.ImageIndex != index) selectedNode.ImageIndex = index;
			foreach(BarItem item in items) {
				item.Category = category;
			}
			Hashtable saveExpanded = new Hashtable();
			saveExpanded[true] = category;
			saveExpanded[selectedNode.Tag] = 0;
			foreach(TreeNode node in Tree.Nodes) {
				if(node.IsExpanded && node.Tag != null) saveExpanded[node.Tag] = 0;
			}
			Tree.BeginUpdate();
			try {
				Repopulate();
				foreach(TreeNode node in Tree.Nodes) {
					if(node.Tag == null) continue;
					if(saveExpanded.ContainsKey(node.Tag)) node.Expand();
					if(Object.Equals(node.Tag, saveExpanded[true])) Tree.SelectedNode = node;
				}
			}
			finally {
				Tree.EndUpdate();
			}
		}
		private void tree_AllowMultiSelectNode(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			e.Cancel = e.Node == null || !(e.Node.Tag is BarItem);
		}
		private void tree_DragNodeAllow(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = Tree.SelCount == 0 || !(Tree.SelNode.Tag is BarItem);
		}
		private BarItem[] GetDragItems(IDataObject data) {
			TreeNode[] nodes = data.GetData(typeof(TreeNode[])) as TreeNode[];
			if(nodes != null && nodes.Length > 0) {
				BarItem[] items = new BarItem[nodes.Length];
				for(int n = 0; n < nodes.Length; n++) {
					items[n] = nodes[n].Tag as BarItem;
				}
				return items;
			}
			return null;
		}
		private void tree_DragNodeGetObject(object sender, DevExpress.Utils.Design.TreeViewGetDragObjectEventArgs e) {
			e.AllowEffects = DragDropEffects.Move;
		}
	}
}
