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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class ContentContainersFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		DXPopupMenu popupMenu;
		public ContentContainersFrame() {
			InitializeComponent();
			CreateImages();
			RegisterContentContainers();
			popupMenu = new DXPopupMenu();
			InitializePopupMenu();
		}
		void InitEditingView() {
			if(EditingView != null)
				((IDesignTimeSupport)EditingView).Load();
		}
		protected virtual void InitializePopupMenu() {
			foreach(KeyValuePair<string, Function<IContentContainer>> containerInitializer in contentContainers) {
				this.popupMenu.Items.Add(CreateMenuItem(containerInitializer));
			}
		}
		protected virtual DXMenuItem CreateMenuItem(KeyValuePair<string, Function<IContentContainer>> containerInitializer) {
			DXMenuItem dxItem = new DXMenuItem(containerInitializer.Key, new EventHandler(OnClickMenuItem));
			dxItem.Image = ImageCollection.GetImageListImage(imageList1, containerInitializer.Key + "_16x16.png");
			dxItem.Tag = containerInitializer.Value;
			return dxItem;
		}
		void OnClickMenuItem(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			IContentContainer container = ((Function<IContentContainer>)item.Tag)();
			((ISupportInitialize)container).BeginInit();
			View.ContentContainers.Add(container);
			((ISupportInitialize)container).EndInit();
			Repopulate();
		}
		void RegisterContentContainers() {
			contentContainers = new Dictionary<string, Function<IContentContainer>>();
			contentContainers.Add("TileContainer", delegate() { return Create("CreateTileContainer"); });
			contentContainers.Add("PageGroup", delegate() { return Create("CreatePageGroup"); });
			contentContainers.Add("TabbedGroup", delegate() { return Create("CreateTabbedGroup"); });
			contentContainers.Add("SlideGroup", delegate() { return Create("CreateSlideGroup"); });
			contentContainers.Add("SplitGroup", delegate() { return Create("CreateSplitGroup"); });
			contentContainers.Add("Page", delegate() { return Create("CreatePage"); });
			contentContainers.Add("Flyout", delegate() { return Create("CreateFlyout"); });
		}
		IContentContainer Create(string createFunction) {
			var mInfo = typeof(WindowsUIView).GetMethod(createFunction, 
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return  mInfo.Invoke(View, new object[] { }) as IContentContainer;
		}
		Dictionary<string, Function<IContentContainer>> contentContainers;
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
				foreach(IContentContainer container in View.ContentContainers)
					AddContentContainer(Tree.Nodes, container);
			}
			finally { Tree.EndUpdate(); }
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
			finally { Tree.EndUpdate(); }
		}
		protected void RefreshNodes(TreeNodeCollection nodes) {
			if(nodes == null) return;
			foreach(TreeNode node in nodes) {
				node.Text = GetNodeCaption(node, false);
				RefreshNodes(node.Nodes);
			}
		}
		protected virtual string GetNodeCaption(TreeNode node, bool isEditing) {
			IContentContainer container = node.Tag as IContentContainer;
			return GetItemCaption(container, isEditing);
		}
		string GetItemCaption(IContentContainer container, bool isEditing) {
			return !string.IsNullOrEmpty(container.Site.Name) ? container.Site.Name : "ContentContainer"; ;
		}
		protected object GetNodeObject(TreeNode node) {
			if(node == null || node.Tag == null) return null;
			return node.Tag as IContentContainer;
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
		protected void UpdateNode(IContentContainer container) {
			Tree.SelectedNode = AddContentContainer(Tree.Nodes, container);
			Repopulate();
		}
		protected virtual TreeNode AddContentContainer(TreeNodeCollection nodes, IContentContainer container) {
			TreeNode node = new TreeNode();
			node.Tag = container;
			node.Text = GetNodeCaption(node, false);
			node.SelectedImageIndex = node.ImageIndex = GetDocumentImageIndex(container);
			nodes.Add(node);
			return node;
		}
		int GetDocumentImageIndex(IContentContainer container) {
			if(container is Page)
				return 0;
			if(container is PageGroup)
				return 1;
			if(container is SlideGroup)
				return 2;
			if(container is SplitGroup)
				return 3;
			if(container is TabbedGroup)
				return 4;
			if(container is TileContainer)
				return 5;
			if(container is Flyout)
				return 6;
			return -1;
		}
		private void DeleteContentContainer() {
			if(Tree.SelectedNode == null) return;
			IContentContainer container = Tree.SelectedNode.Tag as IContentContainer;
			if(container != null) {
				if(MessageBox.Show(string.Format("Are you sure you want to delete '{0}' container?", ((IComponent)container).Site.Name),
					"XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
					View.ContentContainers.Remove(container);
					Tree.Nodes.Remove(Tree.SelectedNode);					
					RemoveItems(container);
					container.Dispose();
					SetSelectedNode();
				}
			}
		}
		void RemoveItems(IContentContainer container){
			if(container is DocumentGroup) ((DocumentGroup)container).Items.Clear();
			if(container is Page) ((Page)container).Document = null;
			if(container is TileContainer) ((TileContainer)container).Items.Clear();
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
			btnDelete.Enabled = node != null && node.Tag is IContentContainer;
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(this.popupMenu, btnAdd, new Point(0, btnAdd.Size.Height));
		}
		void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteContentContainer();
			Repopulate();
		}
		void btnClear_Click(object sender, System.EventArgs e) {
			ClearContentContainers();
			Repopulate();
		}
		protected virtual void tree_GetNodeEditText(object sender, TreeViewGetNodeEditTextEventArgs e) {
			e.Text = GetNodeCaption(e.Node, true);
		}
		protected virtual void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				DeleteContentContainer();
				Repopulate();
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
		public void ClearContentContainers() {
			if(MessageBox.Show("Are you sure you want to remove all containers?", "XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) return;
			IContentContainer[] containers = View.ContentContainers.ToArray();
			View.ContentContainers.RemoveRange(containers);
			for(int i = 0; i < containers.Length; i++) {
				containers[i].Dispose();
			}
			SetSelectedNode();
			Repopulate();
		}
	}
}
