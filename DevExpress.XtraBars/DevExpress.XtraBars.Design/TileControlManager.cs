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
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Design {
	public class TileControlManagerPGFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public TileControlManagerPGFrame() {
		}
		private IDesignerHost designerHost = null;
		private IContainer components;
		private TileControlToolbar tileControlToolbar2;
		private PanelControl panelControl1;
		private TileControlItemsTree treeView1;
		private ImageList imageList1;
		private IComponentChangeService componentChangeService = null;
		#region Initialize Component
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileControlManagerPGFrame));
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.tileControlToolbar2 = new DevExpress.XtraBars.Design.TileControlToolbar();
			this.treeView1 = new DevExpress.XtraBars.Design.TileControlItemsTree();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(263, 129);
			this.splMain.Size = new System.Drawing.Size(5, 322);
			this.pgMain.Location = new System.Drawing.Point(268, 129);
			this.pgMain.Size = new System.Drawing.Size(322, 322);
			this.pnlControl.Controls.Add(this.tileControlToolbar2);
			this.pnlControl.Size = new System.Drawing.Size(590, 101);
			this.lbCaption.Size = new System.Drawing.Size(590, 42);
			this.pnlMain.Controls.Add(this.panelControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 129);
			this.pnlMain.Size = new System.Drawing.Size(263, 322);
			this.horzSplitter.Size = new System.Drawing.Size(590, 4);
			this.panelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelControl1.Controls.Add(this.treeView1);
			this.panelControl1.Location = new System.Drawing.Point(4, 3);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(256, 316);
			this.panelControl1.TabIndex = 3;
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "");
			this.imageList1.Images.SetKeyName(6, "");
			this.imageList1.Images.SetKeyName(7, "");
			this.tileControlToolbar2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileControlToolbar2.Location = new System.Drawing.Point(4, 4);
			this.tileControlToolbar2.Name = "tileControlToolbar2";
			this.tileControlToolbar2.Size = new System.Drawing.Size(582, 93);
			this.tileControlToolbar2.TabIndex = 0;
			this.treeView1.AllowDrag = true;
			this.treeView1.AllowDrop = true;
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.ComponentChangeService = null;
			this.treeView1.DefaultExpandCollapseButtonOffset = 5;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Host = null;
			this.treeView1.ImageIndex = 0;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.InplaceTileControlIndex = 1;
			this.treeView1.Location = new System.Drawing.Point(2, 2);
			this.treeView1.Name = "treeView1";
			this.treeView1.Ribbon = null;
			this.treeView1.SelectedImageIndex = 0;
			this.treeView1.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.treeView1.Size = new System.Drawing.Size(252, 312);
			this.treeView1.TabIndex = 3;
			this.treeView1.TileControlDropDownIndex = 1;
			this.treeView1.AllowSkinning = true;
			this.Name = "TileControlManager";
			this.Size = new System.Drawing.Size(590, 451);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Disposing
		protected override void Dispose(bool disposing) {
			UnSubscribeEvents();
			base.Dispose(disposing);
		}
		#endregion
		#region Suscribe/UnSubscribe
		protected virtual void SubscribeEvents() {
			Toolbar.AddGroup.ItemClick += OnAddGroup;
			Toolbar.AddItem.ItemClick += OnAddMediumItem;
			Toolbar.AddMediumItemPopupCmd.ItemClick += OnAddMediumItem;
			Toolbar.AddWideItemPopupCmd.ItemClick += OnAddWideItem;
			Toolbar.AddLargeItemPopupCmd.ItemClick += OnAddLargeItem;
			Toolbar.AddSmallItemPopupCmd.ItemClick += OnAddSmallItem;
			Toolbar.MoveUp.ItemClick += OnMoveUp;
			Toolbar.MoveDown.ItemClick += OnMoveDown;
			Toolbar.Remove.ItemClick += OnRemove;
			pgMain.PropertyValueChanged += OnObjectPropertiesChanged;
		}
		protected virtual void UnSubscribeEvents() {
			Toolbar.AddGroup.ItemClick -= OnAddGroup;
			Toolbar.AddItem.ItemClick -= OnAddMediumItem;
			Toolbar.AddMediumItemPopupCmd.ItemClick -= OnAddMediumItem;
			Toolbar.AddWideItemPopupCmd.ItemClick -= OnAddWideItem;
			Toolbar.AddLargeItemPopupCmd.ItemClick -= OnAddLargeItem;
			Toolbar.AddSmallItemPopupCmd.ItemClick -= OnAddSmallItem;
			Toolbar.MoveUp.ItemClick -= OnMoveUp;
			Toolbar.MoveDown.ItemClick -= OnMoveDown;
			Toolbar.Remove.ItemClick -= OnRemove;
			pgMain.PropertyValueChanged -= OnObjectPropertiesChanged;
		}
		#endregion
		public override void InitComponent() {
			base.InitComponent();
			InitializeComponent();
			InitializeToolbar();
			InitializeTree();
			if(Tile != null)
				TileControlTree.Initialize(Tile.Name, Tile);
			UpdateButtons();
			SubscribeEvents();
		}
		protected virtual TileControlToolbar Toolbar { get { return tileControlToolbar2; } }
		protected virtual void InitializeToolbar() {
			pnlControl.DockPadding.All = 0;
			pnlControl.Height = Toolbar.ToolbarRibbon.GetMinHeight();
		}
		protected virtual void AddItemCore(TileItemSize itemType) {
			TileControlTree.AddItem(itemType);
			if(TileControlTree.SelNode != null) TileControlTree.SelNode.Expand();
			UpdateButtons();
		}
		bool IsTileControlDesigner { get { return Tile != null; } }
		[Browsable(false)]
		protected virtual TileControlItemsTree TileControlTree { get { return treeView1 as TileControlItemsTree; } }
		protected virtual void InitializeTree() {
			TileControlTree.Ribbon = Ribbon;
			TileControlTree.ComponentChangeService = ComponentChangeService;
			TileControlTree.Host = DesignerHost;
			TileControlTree.Nodes.Clear();
			TileControlTree.SelectionChanged += OnTileControlTreeSelectionChanged;
			TileControlTree.KeyDown += OnTileControlTreeKeyDown;
			TileControlTree.AfterSelect += OnTileControlTreeAfterSelect;
			TileControlTree.Bounds = new Rectangle(TileControlTree.Bounds.Location, new Size(TileControlTree.Bounds.Width, pnlMain.Bottom - TileControlTree.Bounds.Top));	
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		[Browsable(false)]
		public TileControl Tile { get { return EditingComponent as TileControl; } }
		[Browsable(false)]
		protected virtual ISite ComponentSite {
			get {
				if(Tile != null)
					return Tile.Site;
				return Ribbon.Site;
			}
		}
		[Browsable(false)]
		protected virtual IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = ComponentSite.GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			}
		}
		[Browsable(false)]
		protected internal virtual IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) componentChangeService = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		void OnSelectedIndexChanged(object sender, EventArgs e) {
			if(Tile != null)
				TileControlTree.Initialize(Tile.Name, Tile);
			UpdateButtons();
		}
		protected virtual void OnAddGroup(object sender, ItemClickEventArgs e) {
			TileControlTree.AddGroup();
			TileControlTree.Nodes[0].Expand();
			UpdateButtons();
		}
		protected virtual void OnAddMediumItem(object sender, ItemClickEventArgs e) {
			AddItemCore(TileItemSize.Medium);
		}
		protected virtual void OnAddWideItem(object sender, ItemClickEventArgs e) {
			AddItemCore(TileItemSize.Wide);
		}
		protected virtual void OnAddLargeItem(object sender, ItemClickEventArgs e) {
			AddItemCore(TileItemSize.Large);
		}
		protected virtual void OnAddSmallItem(object sender, ItemClickEventArgs e) {
			AddItemCore(TileItemSize.Small);
		}
		protected virtual void OnMoveUp(object sender, ItemClickEventArgs e) {
			TileControlTree.MoveUp();
			UpdateButtons();
		}
		protected virtual void OnMoveDown(object sender, ItemClickEventArgs e) {
			TileControlTree.MoveDown();
			UpdateButtons();
		}
		protected virtual void RemoveItems() {
			TileControlTree.RemoveItems();
			UpdateButtons();
		}
		protected virtual void OnRemove(object sender, ItemClickEventArgs e) {
			RemoveItems();
		}
		protected virtual void OnTileControlTreeKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode != Keys.Delete) return;
			TileControlTree.RemoveItems();
			UpdateButtons();
		}
		void OnTileControlTreeSelectionChanged(object sender, EventArgs e) {
			pgMain.SelectedObjects = TileControlTree.SelectedTreeItems;
		}
		void OnTileControlTreeAfterSelect(object sender, TreeViewEventArgs e) {
			UpdateButtons();
		}
		protected virtual void UpdateButtons() {
			Toolbar.AddGroup.Enabled = TileControlTree.Nodes.Count == 0 ? false : true;
			Toolbar.AddItem.Enabled = TileControlTree.SelectedNode == null || TileControlTree.SelectedNode == TileControlTree.Nodes[0] ? false : true;
			Toolbar.MoveUp.Enabled = TileControlTree.SelectedNode == null || TileControlTree.SelectedNode.PrevNode == null ? false : true;
			Toolbar.MoveDown.Enabled = TileControlTree.SelectedNode == null || TileControlTree.SelectedNode.NextNode == null ? false : true;
			Toolbar.Remove.Enabled = TileControlTree.SelectedNode == null ? false : true;
		}
		protected void OnObjectPropertiesChanged(object sender, PropertyValueChangedEventArgs e) {
			for(int i = 0; i < pgMain.SelectedObjects.Length; i++) {
				TileControlTree.UpdateTreeNodesText(pgMain.SelectedObjects[i]);
			}
			TileControlTree.Update();
			if(Ribbon != null)
				Ribbon.Refresh();
		}
		protected override bool AllowGlobalStore { get { return false; } }
	}
}
