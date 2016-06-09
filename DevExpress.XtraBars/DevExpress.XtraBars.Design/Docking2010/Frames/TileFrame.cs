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
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class TileFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public TileFrame() {
			InitializeComponent();
			CreateImages();
		}
		void InitEditingView() {
			if(EditingView != null)
				((IDesignTimeSupport)EditingView).Load();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(EditingView != null)
					((IDesignTimeSupport)EditingView).Unload();
				DocumentManagerInfo.SelectionChanged -= OnInfo_SelectionChanged;
			}
			base.Dispose(disposing);
		}
		BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		EditingDocumentManagerInfo documentManagerInfo;
		protected EditingDocumentManagerInfo DocumentManagerInfo {
			get { return documentManagerInfo; }
		}
		public override void InitComponent() {
			base.InitComponent();
			this.documentManagerInfo = InfoObject as EditingDocumentManagerInfo;
			DocumentManagerInfo.SelectionChanged += OnInfo_SelectionChanged;
			btnPopulate.Enabled = DocumentTable.Count != 0;
			InitEditingView();
		}
		void OnInfo_SelectionChanged(object sender, EventArgs e) {
			RaiseRefreshWizard("", "ChangedView");
			Populate();
			RefreshPropertyGrid();
		}
		void CreateImages() {
			Tree.ImageList = imageList1;
		}
		public DXTreeView Tree { get { return treeView1; } }
		public override void DoInitFrame() {
			Populate();
		}
		Hashtable expandedItems = new Hashtable();
		protected string GetNodeKey(TreeNode node) {
			int level = 0;
			TreeNode pnode = node;
			while(pnode.Parent != null) {
				level++;
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
		WindowsUIView View { get { return (WindowsUIView)EditingView; } }
		public virtual void Populate() {
			Tree.BeginUpdate();
			try {
				Tree.Nodes.Clear();
				foreach(BaseTile tile in View.Tiles) {
					AddTile(Tree.Nodes, tile);
				}
			}
			finally {
				Tree.EndUpdate();
			}
		}
		public virtual void Repopulate() {
			SaveExpandedItems();
			Populate();
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
			BaseTile tile = node.Tag as BaseTile;
			return GetItemCaption(tile, isEditing);
		}
		string GetItemCaption(BaseTile tile, bool isEditing) {
			return !string.IsNullOrEmpty(tile.Site.Name) ? tile.Site.Name : "Tile"; ;
		}
		protected object GetNodeObject(TreeNode node) {
			if(node == null || node.Tag == null) return null;
			return node.Tag as BaseTile;
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
			}
			else
				pgMain.SelectedObject = null;
			SetDeleteEnabled(e.Node);
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			RefreshTree();
		}
		protected virtual void tree_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e) {
			e.CancelEdit = !IsAllowEditObject(GetNodeObject(e.Node));
		}
		protected void UpdateNode(BaseTile tile) {
			Tree.SelectedNode = AddTile(Tree.Nodes, tile);
			Repopulate();
		}
		protected void CreateTile() {
			Tile tile = new Tile();
			View.Tiles.Add(tile);
			Repopulate();
		}
		protected bool CreateTile(BaseDocument document) {
			return View.Controller.AddTile((Document)document);
		}
		protected virtual TreeNode AddTile(TreeNodeCollection nodes, BaseTile tile) {
			TreeNode node = new TreeNode();
			node.Tag = tile;
			node.Text = GetNodeCaption(node, false);
			node.SelectedImageIndex = node.ImageIndex = 0;
			nodes.Add(node);
			return node;
		}
		void DeleteTile() {
			if(Tree.SelectedNode == null) return;
			BaseTile tile = Tree.SelectedNode.Tag as BaseTile;
			if(tile != null) {
				if(MessageBox.Show(string.Format("Are you sure you want to delete '{0}' tile?", tile.Site.Name),
					"XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
					View.Tiles.Remove(tile);
					Tree.Nodes.Remove(Tree.SelectedNode);
					tile.Dispose();
					SetSelectedNode();
				}
			}
		}
		void SetSelectedNode() {
			SetDeleteEnabled(Tree.SelectedNode);
			pgMain.SelectedObject = null;
			if(Tree.SelectedNode != null && Tree.Nodes.Count != 0) {
				int i = Tree.SelectedNode.Index;
				Tree.SelectedNode = Tree.Nodes[i];
			}
			Repopulate();
		}
		void SetDeleteEnabled(TreeNode node) {
			btnDelete.Enabled = node != null && node.Tag is BaseTile;
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			CreateTile();
		}
		void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteTile();
		}
		void btnClear_Click(object sender, System.EventArgs e) {
			ClearTiles();
		}
		protected virtual void tree_GetNodeEditText(object sender, TreeViewGetNodeEditTextEventArgs e) {
			e.Text = GetNodeCaption(e.Node, true);
		}
		protected virtual void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				DeleteTile();
			}
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
		private void btnPopulate_Click(object sender, EventArgs e) {
			CreateTiles();
		}
		protected void CreateTiles() {
			foreach(Document document in DocumentTable) {
				BaseTile tile;
				if(!View.Tiles.TryGetValue(document, out tile))
					CreateTile(document);
			}
			Repopulate();
		}
		public void ClearTiles() {
			if(MessageBox.Show("Are you sure you want to remove all tiles?", "XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) return;
			BaseTile[] tiles = View.Tiles.ToArray();
			View.Tiles.RemoveRange(tiles);
			for(int i = 0; i < tiles.Length; i++)
				tiles[i].Dispose();
			SetSelectedNode();
			Repopulate();
		}
		IList<Document> documentTable;
		public IList<Document> DocumentTable {
			get {
				if(documentTable == null) {
					documentTable = new List<Document>();
					PopulateTiles();
				}
				return documentTable;
			}
		}
		protected void PopulateTiles() {
			foreach(Document document in EditingView.Documents)
				DocumentTable.Add(document);
		}
	}
}
