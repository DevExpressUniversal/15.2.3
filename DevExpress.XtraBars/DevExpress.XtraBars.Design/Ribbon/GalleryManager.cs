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
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon.Design;
using DevExpress.XtraBars.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Gallery;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Ribbon.Design {
	public class GalleryManager : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public GalleryManager() { }
		private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private IDesignerHost designerHost = null;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private DevExpress.XtraBars.Ribbon.Design.GalleryToolbar galleryToolbar2;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.Utils.Design.DXTreeView treeView1;
		private IComponentChangeService componentChangeService = null;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GalleryManager));
			this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.galleryToolbar2 = new DevExpress.XtraBars.Ribbon.Design.GalleryToolbar();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.treeView1 = new GalleryItemsTree(); 
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(263, 129);
			this.splMain.Size = new System.Drawing.Size(6, 322);
			this.pgMain.Location = new System.Drawing.Point(269, 129);
			this.pgMain.Size = new System.Drawing.Size(321, 322);
			this.pnlControl.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.galleryToolbar2});
			this.pnlControl.Size = new System.Drawing.Size(590, 101);
			this.lbCaption.Size = new System.Drawing.Size(590, 42);
			this.pnlMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.panelControl1,
																				  this.labelControl1,
																				  this.comboBoxEdit1});
			this.pnlMain.Location = new System.Drawing.Point(0, 129);
			this.pnlMain.Size = new System.Drawing.Size(263, 322);
			this.horzSplitter.Size = new System.Drawing.Size(590, 4);
			this.comboBoxEdit1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.comboBoxEdit1.Location = new System.Drawing.Point(57, 5);
			this.comboBoxEdit1.Name = "comboBoxEdit1";
			this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																												  new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxEdit1.Size = new System.Drawing.Size(200, 20);
			this.comboBoxEdit1.TabIndex = 0;
			this.labelControl1.Location = new System.Drawing.Point(8, 9);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(37, 14);
			this.labelControl1.TabIndex = 1;
			this.labelControl1.Text = "Gallery";
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.galleryToolbar2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryToolbar2.Location = new System.Drawing.Point(4, 4);
			this.galleryToolbar2.Name = "galleryToolbar2";
			this.galleryToolbar2.Size = new System.Drawing.Size(582, 93);
			this.galleryToolbar2.TabIndex = 0;
			this.panelControl1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panelControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.treeView1});
			this.panelControl1.Location = new System.Drawing.Point(4, 32);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(256, 287);
			this.panelControl1.TabIndex = 3;
			this.panelControl1.Text = "panelControl1";
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Location = new System.Drawing.Point(2, 2);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(252, 283);
			this.treeView1.TabIndex = 3;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pgMain,
																		  this.splMain,
																		  this.pnlMain,
																		  this.pnlControl,
																		  this.horzSplitter,
																		  this.lbCaption});
			this.Name = "GalleryManager";
			this.Size = new System.Drawing.Size(590, 451);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			ClearPropertyGridEvents();
		}
		public override void InitComponent() { 
			base.InitComponent();
			InitializeComponent();
			InitializeToolbar();
			InitializeTree();
			InitializeComboBox();
			UpdateButtons();
			InitializePropertyGridEvents();
		}
		void ClearPropertyGridEvents() {
			pgMain.PropertyValueChanged -= new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		void InitializePropertyGridEvents() {
			pgMain.PropertyValueChanged += new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		protected virtual GalleryToolbar Toolbar { get { return galleryToolbar2; } }
		protected virtual void InitializeToolbar() { 
			pnlControl.DockPadding.All = 0;
			pnlControl.Height = Toolbar.ToolbarRibbon.GetMinHeight();
			Toolbar.AddInplaceGallery.ItemClick += new ItemClickEventHandler(OnAddInplaceGallery);
			Toolbar.AddPopupGallery.ItemClick += new ItemClickEventHandler(OnAddPopupGallery);
			Toolbar.AddGroup.ItemClick += new ItemClickEventHandler(OnAddGroup);
			Toolbar.AddItem.ItemClick += new ItemClickEventHandler(OnAddItem);
			Toolbar.MoveUp.ItemClick += new ItemClickEventHandler(OnMoveUp);
			Toolbar.MoveDown.ItemClick += new ItemClickEventHandler(OnMoveDown);
			Toolbar.Remove.ItemClick += new ItemClickEventHandler(OnRemove);
			Toolbar.GenerateValues.ItemClick += new ItemClickEventHandler(OnGenerateValues);
			if(IsGalleryControlDesigner) {
				Toolbar.ShowGalleryButtons = false;
			}
		}
		protected BaseGallery GetGallery() {
			if(ComboBox.SelectedIndex == -1)
				return null;
			RibbonGalleryBarItem inRibbonGalleryItem = ComboBox.Properties.Items[ComboBox.SelectedIndex] as RibbonGalleryBarItem;
			GalleryDropDown galleryPopup = ComboBox.Properties.Items[ComboBox.SelectedIndex] as GalleryDropDown;
			GalleryControlGallery gallery = ComboBox.Properties.Items[ComboBox.SelectedIndex] as GalleryControlGallery;
			if(inRibbonGalleryItem != null)
				return inRibbonGalleryItem.Gallery;
			if(galleryPopup != null)
				return galleryPopup.Gallery;
			return gallery;
		}
		void OnGenerateValues(object sender, ItemClickEventArgs e) {
			List<GalleryItem> items = GetGallery().GetAllItems();
			GetGallery().BeginUpdate();
			try {
				for(int i = 0; i < items.Count; i++) {
					items[i].Value = i;
				}
			}
			finally {
				GetGallery().EndUpdate();
			}
			this.pgMain.Refresh();
		}
		bool IsGalleryControlDesigner { get { return Gallery != null; } }
		[Browsable(false)]
		protected virtual ComboBoxEdit ComboBox { get { return comboBoxEdit1; } }
		protected virtual void InitializeComboBox() {
			ComboBox.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
			if(Gallery != null) {
				ComboBox.Properties.Items.Add(Gallery);
			}
			if(Ribbon != null) {
				foreach(object comp in Ribbon.Container.Components) {
					if(comp as RibbonGalleryBarItem != null) ComboBox.Properties.Items.Add(comp);
					else if(comp as GalleryDropDown != null) ComboBox.Properties.Items.Add(comp);
				}
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ComboBox.Properties.Items.Count == 0)
				ComboBox.Text = "Empty";
			else {
				RibbonEditorForm rf = Parent.Parent.Parent as RibbonEditorForm;
				if(rf.IsGallery()) {
					ComboBox.SelectedIndex = ComboBox.Properties.Items.IndexOf(rf.ComponentObj);
					rf.ComponentObj = null;
				}
				else ComboBox.SelectedIndex = 0;
			}
		}
		[Browsable(false)]
		protected virtual GalleryItemsTree GalleryTree { get { return treeView1 as GalleryItemsTree; } }
		protected virtual void InitializeTree() { 
			GalleryTree.Ribbon = Ribbon;
			GalleryTree.ComponentChangeService = ComponentChangeService;
			GalleryTree.Host = DesignerHost;
			GalleryTree.Nodes.Clear();
			GalleryTree.SelectionChanged += new EventHandler(OnGalleryTreeSelectionChanged);
			GalleryTree.KeyDown += new KeyEventHandler(OnGalleryTreeKeyDown);
			GalleryTree.AfterSelect += new TreeViewEventHandler(OnGalleryTreeAfterSelect);
			GalleryTree.Bounds = new Rectangle(GalleryTree.Bounds.Location, new Size(GalleryTree.Bounds.Width, pnlMain.Bottom - GalleryTree.Bounds.Top));
			if(ComboBox.SelectedIndex == -1) return ;
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		[Browsable(false)]
		public GalleryControlGallery Gallery { get { return EditingComponent as GalleryControlGallery; } }
		[Browsable(false)]
		protected virtual ISite ComponentSite { 
			get {
				if(Gallery != null) {
					if(Gallery.PopupGalleryEdit != null)
						return Gallery.PopupGalleryEdit.Site;
					return Gallery.GalleryControl.Site;
				}
				return Ribbon.Site; 
			} 
		}
		[Browsable(false)]
		protected virtual IDesignerHost DesignerHost { 
			get { 
				if(designerHost == null)designerHost = ComponentSite.GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			} 
		}
		[Browsable(false)]
		protected internal virtual IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)componentChangeService = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		protected virtual void OnAddInplaceGallery(object sender, ItemClickEventArgs e) {
			GalleryTree.AddInRibbonGallery();
			ComboBox.Properties.Items.Add(GalleryTree.InRibbonGallery.OwnerItem);
			ComboBox.SelectedIndex = ComboBox.Properties.Items.Count - 1;
			UpdateButtons();
		}
		protected virtual void OnAddPopupGallery(object sender, ItemClickEventArgs e) {
			GalleryTree.AddGalleryDropDown();
			ComboBox.Properties.Items.Add(GalleryTree.PopupGallery.GalleryDropDown);
			ComboBox.SelectedIndex = ComboBox.Properties.Items.Count - 1;
			UpdateButtons();
		}
		protected virtual void OnAddGroup(object sender, ItemClickEventArgs e) { 
			GalleryTree.AddGroup();
			GalleryTree.Nodes[0].Expand();
			UpdateButtons();
		}
		protected virtual void OnAddItem(object sender, ItemClickEventArgs e) {
			GalleryTree.AddItem();
			if(GalleryTree.SelNode != null) GalleryTree.SelNode.Expand();
			UpdateButtons();
		}
		protected virtual void OnMoveUp(object sender, ItemClickEventArgs e) {
			GalleryTree.MoveUp();
			UpdateButtons();
		}
		protected virtual void OnMoveDown(object sender, ItemClickEventArgs e) {
			GalleryTree.MoveDown();
			UpdateButtons();
		}
		protected virtual void RemoveItems() {
			GalleryTree.RemoveItems();
			if(GalleryTree.Nodes.Count == 0) UpdateComboBox();
			UpdateButtons();
		}
		protected virtual void OnRemove(object sender, ItemClickEventArgs e) {
			RemoveItems();
		}
		protected virtual void OnGalleryTreeKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode != Keys.Delete) return;
			GalleryTree.RemoveItems();
			if(GalleryTree.Nodes.Count == 0) UpdateComboBox();
			UpdateButtons();
		}
		protected virtual void UpdateComboBox() {
			int prevIndex = ComboBox.SelectedIndex;
			ComboBox.Properties.Items.Clear();
			foreach(Component comp in Ribbon.Container.Components) {
				if(comp as RibbonGalleryBarItem != null || comp as GalleryDropDown != null) ComboBox.Properties.Items.Add(comp);
			}
			if(prevIndex >= ComboBox.Properties.Items.Count) prevIndex--;
			if(ComboBox.Properties.Items.Count == 0)
				ComboBox.Text = "Empty";
			else ComboBox.SelectedIndex = prevIndex;
		}
		void OnSelectedIndexChanged(object sender, EventArgs e) { 
			if(ComboBox.SelectedIndex == -1)return ;
			RibbonGalleryBarItem inRibbonGalleryItem = ComboBox.Properties.Items[ComboBox.SelectedIndex] as RibbonGalleryBarItem;
			GalleryDropDown galleryPopup = ComboBox.Properties.Items[ComboBox.SelectedIndex] as GalleryDropDown;
			GalleryControlGallery gallery = ComboBox.Properties.Items[ComboBox.SelectedIndex] as GalleryControlGallery;
			if(gallery != null) {
				GalleryTree.Initialize(gallery.PopupGalleryEdit != null? gallery.PopupGalleryEdit.Name: gallery.GalleryControl.Name, gallery);
			}
			else if(inRibbonGalleryItem != null)
				GalleryTree.Initialize(inRibbonGalleryItem.Caption, inRibbonGalleryItem.Gallery);
			else if(galleryPopup != null)
				GalleryTree.Initialize(galleryPopup.Name, galleryPopup.Gallery);
			UpdateButtons();
		}
		void OnGalleryTreeSelectionChanged(object sender, EventArgs e) {
			pgMain.SelectedObjects = GalleryTree.SelectedTreeItems;
		}
		void OnGalleryTreeAfterSelect(object sender, TreeViewEventArgs e) {
			UpdateButtons();
		}
		protected virtual void UpdateButtons() {
			Toolbar.AddGroup.Enabled = GalleryTree.Nodes.Count == 0? false: true;
			Toolbar.AddItem.Enabled = GalleryTree.SelectedNode == null || GalleryTree.SelectedNode == GalleryTree.Nodes[0] ? false : true;
			Toolbar.MoveUp.Enabled = GalleryTree.SelectedNode == null || GalleryTree.SelectedNode.PrevNode == null ? false : true;
			Toolbar.MoveDown.Enabled = GalleryTree.SelectedNode == null || GalleryTree.SelectedNode.NextNode == null ? false : true;
			Toolbar.Remove.Enabled = GalleryTree.SelectedNode == null ? false : true;
			Toolbar.GenerateValues.Enabled = GalleryTree.SelectedNode == null ? false : true;
		}
		protected void OnObjectPropertiesChanged(object sender, PropertyValueChangedEventArgs e) {
			for(int i = 0; i < pgMain.SelectedObjects.Length; i++) {
				object obj = pgMain.SelectedObjects[i] is GalleryObjectDescriptor ? ((GalleryObjectDescriptor)pgMain.SelectedObjects[i]).Item : pgMain.SelectedObjects[i];
				GalleryTree.UpdateTreeNodesText(obj);
			}
			GalleryTree.Update();
			if(Ribbon != null)
				Ribbon.Refresh();
		}
		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
		}
		protected override bool AllowGlobalStore { get { return false; } }
	}
}
