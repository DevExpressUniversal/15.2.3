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
using System.Windows.Forms;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon.Design {
	[CLSCompliant(false)]
	public class ItemLinksBaseManager : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		protected SplitContainerControl splitContainerControl1;
		PanelControl itemsPanel;
		PanelControl linksPanel;
		TablePanelControl itemsBottomPanel;
		PanelControl linksBottomPanel;
		PanelControl linksContentPanel;
		PanelControl itemsContentPanel;
		PanelControl propertyGridTopSpacePanel;
		RibbonItemsListBox itemsList;
		LabelControl labelCategories;
		CategoriesComboBox categories;
		ImageList treeImages;
		IContainer components;
		RibbonStatusBar statusBar = null;
		ItemsListHelper itemsListHelper = null;
		IDesignerHost designerHost = null;
		ItemLinksTreeView itemsTree;
		ItemLinksBaseToolbar linksToolbar;
		ItemLinksBaseToolbar itemsToolbar;
		CheckEdit beginGroup;
		IComponentChangeService componentChangeService = null;
		SearchControl linksSearchControl;
		SearchControl itemsSearchControl;
		public ItemLinksBaseManager() { }
		[Browsable(false)]
		protected virtual IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = ComponentSite.GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			}
		}
		[Browsable(false)]
		protected virtual IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) componentChangeService = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		[Browsable(false)]
		protected virtual CheckEdit BeginGroup { get { return beginGroup; } }
		[Browsable(false)]
		protected virtual CategoriesComboBox Categories { get { return categories; } }
		[Browsable(false)]
		protected virtual ItemsListHelper ItemsListHelper { get { return itemsListHelper; } set { itemsListHelper = value; } }
		[Browsable(false)]
		protected virtual ISite ComponentSite { get { return Ribbon.Site; } }
		[Browsable(false)]
		public virtual RibbonItemsListBox ItemsList { get { return itemsList; } }
		[Browsable(false)]
		public virtual ItemLinksBaseToolbar LinksToolbar { get { return linksToolbar; } }
		[Browsable(false)]
		public virtual ItemLinksBaseToolbar ItemsToolbar { get { return itemsToolbar; } }
		[Browsable(false)]
		public virtual ItemLinksTreeView ItemsTree { get { return itemsTree; } }
		[Browsable(false)]
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		[Browsable(false)]
		protected RibbonStatusBar StatusBar {
			get {
				if(statusBar == null) statusBar = StatusBarHelper.GetStatusBar(Ribbon);
				return statusBar;
			}
		}
		public override void InitComponent() {
			base.InitComponent();
			InitializeComponent();
			InitializeToolbar();
			InitializeItemsList();
			InitializeItemsTree();
			InitializeItemsListHelper();
			InitializeToolbarEvents();
			InitializePropertyGridEvents();
			InitializeSearchEvents();
			InitializeSearch();
		}
		protected virtual void InitializeSearch() {
			this.itemsSearchControl.Visible = LinksToolbar.SearchButton.Checked;
			this.linksSearchControl.Visible = ItemsToolbar.SearchButton.Checked;
			this.itemsTree.BringToFront();
			this.itemsList.BringToFront();
			this.ItemsToolbar.SearchButton.Visible = false;
		}
		System.Collections.Generic.List<Control> enabledButtons = new System.Collections.Generic.List<Control>();
		protected System.Collections.Generic.List<Control> EnabledButtons {
			get { return this.enabledButtons; }
		}
		protected virtual void InitializeSearchEvents() {
			this.itemsSearchControl.EditValueChanged += itemsSearchControl_EditValueChanged;
			FillEnabledButton();
		}
		protected virtual void FillEnabledButton() {
			if(LinksToolbar.MoveUpButton.Visible == true)
				enabledButtons.Add(LinksToolbar.MoveUpButton);
			if(LinksToolbar.MoveDownButton.Visible == true)
				enabledButtons.Add(LinksToolbar.MoveDownButton);
			if(LinksToolbar.RemoveButton.Visible == true)
				enabledButtons.Add(LinksToolbar.RemoveButton);
			if(LinksToolbar.AddItemButton.Visible == true)
				enabledButtons.Add(LinksToolbar.AddItemButton);
			if(LinksToolbar.RemoveItemButton.Visible == true)
				enabledButtons.Add(LinksToolbar.RemoveItemButton);
			if(LinksToolbar.AddButtonGroup.Visible == true)
				enabledButtons.Add(LinksToolbar.AddButtonGroup);
		}
		bool allowUpdateToolbarButtonsCore = true;
		[Browsable(false)]
		protected virtual bool AllowUpdateToolbarButtons {
			get { return allowUpdateToolbarButtonsCore; }
			set {
				if(allowUpdateToolbarButtonsCore == value) return;
				allowUpdateToolbarButtonsCore = value;
			}
		}
		void itemsSearchControl_EditValueChanged(object sender, EventArgs e) {
			AllowUpdateToolbarButtons = string.IsNullOrEmpty(this.itemsSearchControl.Text);
			SetToolbarButtonsEnable(AllowUpdateToolbarButtons);
		}
		void SetToolbarButtonsEnable(bool enable) {
			foreach(var button in enabledButtons)
				if(button.Enabled != enable)
					button.Enabled = enable;
			UpdateBarButtons();
		}
		protected virtual void InitializeToolbar() {
			ItemsToolbar.InitializeItemTypeMenu(ItemsToolbar.ItemTypeImages);
			pnlControl.DockPadding.All = 0;
			pnlControl.Height = 0;
			if(!AllowDesignerCaption) {
				Controls.Add(propertyGridTopSpacePanel);
				propertyGridTopSpacePanel.Height = ItemsToolbar.Height;
				Controls.SetChildIndex(propertyGridTopSpacePanel, 0);
				Controls.SetChildIndex(pgMain, 0);
			}
		}
		protected bool AllowDesignerCaption { get { return false; } }
		protected virtual ItemLinksBaseToolbar CreateLinksToolbar() { return new LinksToolbar(); }
		protected virtual ItemLinksBaseToolbar CreateItemsToolbar() { return new ItemsToolbar(); }
		protected virtual ItemLinksTreeView CreateItemsTree() { return new ItemLinksTreeView() { AllowSkinning = true }; }
		protected virtual void InitializeItemsListHelper() {
			ItemsListHelper = new ItemsListHelper(Ribbon);
			ItemsListHelper.DesignerHost = DesignerHost;
			ItemsListHelper.ComponentChangeService = ComponentChangeService;
		}
		protected virtual void InitializeItemsTreeLinks() { }
		protected virtual void UpdateBeginGroup() {
			if(ItemsTree.Nodes.Count == 0 || ItemsTree.SelNodes.Length == 0 || ItemsTree.SelNodes[0].Parent != null || ItemsTree.SelectedNode == null) {
				BeginGroup.Enabled = false;
				return;
			}
			BeginGroup.Enabled = true;
			BeginGroup.Checked = (ItemsTree.SelectedNode.Tag as BarItemLink).BeginGroup;
		}
		protected virtual void InitializeItemsTree() {
			ItemsTree.Ribbon = Ribbon;
			ItemsTree.StatusBar = StatusBar;
			ItemsTree.DesignerHost = DesignerHost;
			ItemsTree.ComponentChangeService = ComponentChangeService;
			InitializeItemsTreeLinks();
			ItemsTree.CreateTree();
		}
		void InitializeItemsList() {
			categories.Manager = Ribbon.Manager;
			categories.InitCategories(0);
			ItemsList.Manager = Ribbon.Manager;
			categories.SelectedIndex = categories.Properties.Items.Count - 1;
		}
		void ClearPropertyGridEvents() {
			pgMain.PropertyValueChanged -= new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		void InitializePropertyGridEvents() {
			pgMain.PropertyValueChanged += new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
				ClearToolbarEvents();
				ClearPropertyGridEvents();
				ClearSearchingEvents();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemLinksBaseManager));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.beginGroup = new DevExpress.XtraEditors.CheckEdit();
			this.linksPanel = new DevExpress.XtraEditors.PanelControl();
			this.itemsTree = CreateItemsTree();
			this.treeImages = new System.Windows.Forms.ImageList(this.components);
			this.categories = new DevExpress.XtraBars.Ribbon.Design.CategoriesComboBox();
			this.labelCategories = new DevExpress.XtraEditors.LabelControl();
			this.itemsPanel = new DevExpress.XtraEditors.PanelControl();
			this.itemsBottomPanel = new TablePanelControl();
			this.linksBottomPanel = new PanelControl();
			this.linksContentPanel = new PanelControl();
			this.itemsContentPanel = new PanelControl();
			this.linksSearchControl = new SearchControl();
			this.itemsSearchControl = new SearchControl();
			this.propertyGridTopSpacePanel = new PanelControl();
			this.itemsList = new DevExpress.XtraBars.Ribbon.Customization.RibbonItemsListBox();
			this.linksToolbar = CreateLinksToolbar();
			this.itemsToolbar = CreateItemsToolbar();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.beginGroup.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.linksPanel)).BeginInit();
			this.linksPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.categories.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsPanel)).BeginInit();
			this.itemsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.itemsBottomPanel)).BeginInit();
			this.itemsBottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.linksBottomPanel)).BeginInit();
			this.linksBottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.linksSearchControl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsSearchControl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsContentPanel)).BeginInit();
			this.itemsContentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridTopSpacePanel)).BeginInit();
			this.propertyGridTopSpacePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.linksContentPanel)).BeginInit();
			this.linksContentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.itemsList)).BeginInit();
			this.SuspendLayout();
			this.linksSearchControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.linksSearchControl.Name = "linksSearchControl";
			this.linksSearchControl.Client = this.itemsList;
			this.itemsSearchControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.itemsSearchControl.Name = "itemsSearchControl";
			this.itemsSearchControl.Client = this.itemsTree;
			this.splMain.Location = new System.Drawing.Point(568, 94);
			this.splMain.Size = new System.Drawing.Size(6, 410);
			this.pgMain.Location = new System.Drawing.Point(574, 94);
			this.pgMain.Size = new System.Drawing.Size(250, 410);
			this.pnlControl.Size = new System.Drawing.Size(824, 66);
			this.lbCaption.Size = new System.Drawing.Size(824, 42);
			this.pnlMain.Controls.Add(this.splitContainerControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 94);
			this.pnlMain.Size = new System.Drawing.Size(568, 410);
			this.horzSplitter.Size = new System.Drawing.Size(824, 4);
			this.beginGroup.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.beginGroup.Location = new System.Drawing.Point(-1, 4);
			this.beginGroup.Name = "beginGroup";
			this.beginGroup.Properties.Caption = "Begin group";
			this.beginGroup.Size = new System.Drawing.Size(282, 19);
			this.beginGroup.TabIndex = 1;
			this.beginGroup.CheckedChanged += new System.EventHandler(this.beginGroup_CheckedChanged);
			this.linksPanel.Dock = DockStyle.Fill;
			this.linksPanel.Location = new System.Drawing.Point(0, 32);
			this.linksPanel.Name = "panelControl2";
			this.linksPanel.Size = new System.Drawing.Size(280, 352);
			this.linksPanel.TabIndex = 0;
			this.linksPanel.Controls.Add(this.itemsTree);
			this.linksPanel.Controls.Add(this.itemsSearchControl);
			this.itemsTree.AllowDrag = true;
			this.itemsTree.AllowDrop = true;
			this.itemsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.itemsTree.ComponentChangeService = null;
			this.itemsTree.DesignerHost = null;
			this.itemsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemsTree.DropSelectedNode = null;
			this.itemsTree.DropTargetNode = null;
			this.itemsTree.ImageIndex = 0;
			this.itemsTree.ImageList = this.treeImages;
			this.itemsTree.ItemLinks = null;
			this.itemsTree.Location = new System.Drawing.Point(2, 2);
			this.itemsTree.Name = "itemsTree";
			this.itemsTree.Ribbon = null;
			this.itemsTree.SelectedImageIndex = 0;
			this.itemsTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.itemsTree.Size = new System.Drawing.Size(276, 348);
			this.itemsTree.StatusBar = null;
			this.itemsTree.TabIndex = 2;
			this.itemsTree.SelectionChanged += new System.EventHandler(this.itemsTree_SelectionChanged);
			this.itemsTree.Enter += new System.EventHandler(this.itemsTree_Enter);
			this.itemsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.itemsTree_AfterSelect);
			this.itemsTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemsTree_KeyDown);
			this.treeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
			this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
			this.treeImages.Images.SetKeyName(0, "category.png");
			this.treeImages.Images.SetKeyName(1, "");
			this.treeImages.Images.SetKeyName(2, "");
			this.treeImages.Images.SetKeyName(3, "");
			this.treeImages.Images.SetKeyName(4, "");
			this.treeImages.Images.SetKeyName(5, "");
			this.treeImages.Images.SetKeyName(6, "");
			this.treeImages.Images.SetKeyName(7, "");
			this.treeImages.Images.SetKeyName(8, "");
			this.treeImages.Images.SetKeyName(9, "");
			this.treeImages.Images.SetKeyName(10, "");
			this.treeImages.Images.SetKeyName(11, "");
			this.treeImages.Images.SetKeyName(12, "");
			this.categories.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.categories.Location = new System.Drawing.Point(5, 5);
			this.categories.Manager = null;
			this.categories.Margin = new Padding(0, 5, 0, 0);
			this.categories.Name = "categories";
			this.categories.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.categories.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.categories.Size = new System.Drawing.Size(120, 20);
			this.categories.TabIndex = 2;
			this.categories.SelectedIndexChanged += new System.EventHandler(this.categories_SelectedIndexChanged);
			this.labelCategories.Location = new System.Drawing.Point(0, 8);
			this.labelCategories.Margin = new Padding(8, 7, 0, 0);
			this.labelCategories.Name = "labelControl1";
			this.labelCategories.Size = new System.Drawing.Size(56, 13);
			this.labelCategories.TabIndex = 1;
			this.labelCategories.Text = "Categories:";
			this.itemsPanel.Dock = DockStyle.Fill;
			this.itemsPanel.Controls.Add(this.itemsList);
			this.itemsPanel.Location = new System.Drawing.Point(0, 32);
			this.itemsPanel.Name = "panelControl1";
			this.itemsPanel.Size = new System.Drawing.Size(272, 368);
			this.itemsPanel.TabIndex = 0;
			this.itemsBottomPanel.Dock = DockStyle.Bottom;
			this.itemsBottomPanel.Location = new System.Drawing.Point(0, 32);
			this.itemsBottomPanel.Name = "itemsBottomPanel";
			this.itemsBottomPanel.Size = new System.Drawing.Size(60, 27);
			this.itemsBottomPanel.TabIndex = 0;
			this.itemsBottomPanel.AddControl(this.labelCategories, 0, 0);
			this.itemsBottomPanel.AddControl(this.categories, 1, 0);
			this.itemsBottomPanel.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.linksBottomPanel.Dock = DockStyle.Bottom;
			this.linksBottomPanel.Location = new System.Drawing.Point(0, 32);
			this.linksBottomPanel.Name = "linksBottomPanel";
			this.linksBottomPanel.Size = new System.Drawing.Size(60, 27);
			this.linksBottomPanel.TabIndex = 0;
			this.linksBottomPanel.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.linksBottomPanel.Controls.Add(this.beginGroup);
			this.itemsContentPanel.Dock = DockStyle.Fill;
			this.itemsContentPanel.Location = new System.Drawing.Point(0, 32);
			this.itemsContentPanel.Name = "itemsContentPanel";
			this.itemsContentPanel.Size = new System.Drawing.Size(60, 60);
			this.itemsContentPanel.TabIndex = 0;
			this.itemsContentPanel.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.itemsContentPanel.Controls.Add(this.itemsPanel);
			this.itemsContentPanel.Controls.Add(this.itemsToolbar);
			this.itemsContentPanel.Controls.Add(this.itemsBottomPanel);
			this.propertyGridTopSpacePanel.Dock = DockStyle.Top;
			this.propertyGridTopSpacePanel.Location = new System.Drawing.Point(0, 0);
			this.propertyGridTopSpacePanel.Name = "propertyGridTopSpacePanel";
			this.propertyGridTopSpacePanel.Size = new System.Drawing.Size(60, 60);
			this.propertyGridTopSpacePanel.TabIndex = 0;
			this.propertyGridTopSpacePanel.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.linksContentPanel.Dock = DockStyle.Fill;
			this.linksContentPanel.Location = new System.Drawing.Point(0, 32);
			this.linksContentPanel.Name = "linksContentPanel";
			this.linksContentPanel.Size = new System.Drawing.Size(60, 60);
			this.linksContentPanel.TabIndex = 0;
			this.linksContentPanel.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.linksContentPanel.Controls.Add(linksPanel);
			this.linksContentPanel.Controls.Add(linksToolbar);
			this.linksContentPanel.Controls.Add(linksBottomPanel);
			this.itemsList.Appearance.BackColor = System.Drawing.Color.White;
			this.itemsList.Appearance.Options.UseBackColor = true;
			this.itemsList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.itemsList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemsList.ItemHeight = 16;
			this.itemsList.Location = new System.Drawing.Point(2, 2);
			this.itemsList.Name = "itemsList";
			this.itemsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.itemsList.Size = new System.Drawing.Size(268, 364);
			this.itemsList.TabIndex = 0;
			this.itemsList.Enter += new System.EventHandler(this.itemsList_Enter);
			this.itemsList.SelectedIndexChanged += new System.EventHandler(this.itemsList_SelectedIndexChanged);
			this.itemsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemsList_KeyDown);
			this.linksToolbar.Dock = System.Windows.Forms.DockStyle.Top;
			this.linksToolbar.Location = new System.Drawing.Point(4, 4);
			this.linksToolbar.Name = "linksToolbar";
			this.linksToolbar.SelBarItem = null;
			this.linksToolbar.SelEditorInfo = null;
			this.linksToolbar.TabIndex = 0;
			this.itemsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
			this.itemsToolbar.Location = new System.Drawing.Point(4, 4);
			this.itemsToolbar.Name = "itemsToolbar";
			this.itemsToolbar.SelBarItem = null;
			this.itemsToolbar.SelEditorInfo = null;
			this.itemsToolbar.TabIndex = 0;
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.linksContentPanel);
			this.splitContainerControl1.Panel1.Text = "splitContainerControl1_Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.itemsContentPanel);
			this.splitContainerControl1.Panel2.Text = "splitContainerControl1_Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(568, 410);
			this.splitContainerControl1.SplitterPosition = 286;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.Name = "ItemLinksBaseManager";
			this.Size = new System.Drawing.Size(824, 504);
			((System.ComponentModel.ISupportInitialize)(this.linksSearchControl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsSearchControl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.beginGroup.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.linksPanel)).EndInit();
			this.linksPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.categories.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsPanel)).EndInit();
			this.itemsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.itemsList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemsBottomPanel)).EndInit();
			this.itemsBottomPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.linksBottomPanel)).EndInit();
			this.linksBottomPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.itemsContentPanel)).EndInit();
			this.itemsContentPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propertyGridTopSpacePanel)).EndInit();
			this.propertyGridTopSpacePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.linksContentPanel)).EndInit();
			this.linksContentPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void InitializeToolbarEvents() {
			LinksToolbar.MoveUpButton.Click += MoveUp_Click;
			LinksToolbar.MoveDownButton.Click += MoveDown_Click;
			LinksToolbar.RemoveButton.Click += Remove_Click;
			LinksToolbar.SearchButton.CheckedChanged += LinksToolbarSearchButton_CheckedChanged;
			ItemsToolbar.RemoveItemButton.Click += RemoveItem_Click;
			ItemsToolbar.AddButtonGroup.Click += AddButtonGroup_Click;
			ItemsToolbar.CreateItem += Toolbar_CreateItem;
			ItemsToolbar.SearchButton.CheckedChanged += ItemsToolbarSearchButton_CheckedChanged;
		}
		void ItemsToolbarSearchButton_CheckedChanged(object sender, EventArgs e) {
			SearchControlVisibleChange(linksSearchControl, ItemsToolbar.SearchButton.Checked);
		}
		void LinksToolbarSearchButton_CheckedChanged(object sender, EventArgs e) {
			SearchControlVisibleChange(itemsSearchControl, LinksToolbar.SearchButton.Checked);
		}
		void SearchControlVisibleChange(SearchControl control, bool visible) {
			if(!visible)
				control.ClearFilter();
			control.Visible = visible;
		}
		protected virtual void ClearToolbarEvents() {
			LinksToolbar.MoveUpButton.Click -= MoveUp_Click;
			LinksToolbar.MoveDownButton.Click -= MoveDown_Click;
			LinksToolbar.RemoveButton.Click -= Remove_Click;
			LinksToolbar.SearchButton.CheckedChanged -= LinksToolbarSearchButton_CheckedChanged;
			ItemsToolbar.RemoveItemButton.Click -= RemoveItem_Click;
			ItemsToolbar.AddButtonGroup.Click -= AddButtonGroup_Click;
			ItemsToolbar.CreateItem -= new ItemCreateEventHandler(Toolbar_CreateItem);
			ItemsToolbar.SearchButton.CheckedChanged -= ItemsToolbarSearchButton_CheckedChanged;
		}
		protected virtual void ClearSearchingEvents() {
			this.itemsSearchControl.EditValueChanged -= itemsSearchControl_EditValueChanged;
			this.enabledButtons.Clear();
		}
		protected virtual void MoveUp_Click(object sender, EventArgs e) {
			ItemsTree.MoveUp();
			UpdateBarButtons();
		}
		protected virtual void MoveDown_Click(object sender, EventArgs e) {
			ItemsTree.MoveDown();
			UpdateBarButtons();
		}
		protected virtual void Remove_Click(object sender, EventArgs e) {
			if(ItemsTree.SelectedNode != null && ItemsTree.SelectedNode.Nodes.Count > 0) {
				if(XtraMessageBox.Show(this.FindForm(), GetConfirmMsgText(ItemsTree.SelectedNode), "Ribbon Control Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
					return;
			}
			ItemsTree.Remove();
			UpdateBarButtons();
		}
		string GetConfirmMsgText(TreeNode node) {
			if(node == null) return string.Empty;
			string nodeText = string.Empty;
			IComponent component = node.Tag as IComponent;
			if(component != null && component.Site != null) {
				nodeText = component.Site.Name;
			}
			else if(!string.IsNullOrEmpty(node.Text)) {
				nodeText = node.Text;
			}
			return string.Format("Are you sure you want to delete {0} ?", nodeText);
		}
		void Toolbar_CreateItem(object sender, ItemCreateEventArgs e) {
			BarManagerCategory cat = SelCategory.Equals(BarManagerCategory.TotalCategory) ? BarManagerCategory.DefaultCategory : SelCategory;
			ItemsList.CreateNewItem(e.ItemInfo, e.Arguments, Ribbon, cat);
			UpdateBarButtons();
		}
		protected virtual BarItemInfo GetButtonGroupInfo(BarItemInfo[] info) {
			for(int i = 0; i < info.Length; i++) {
				if(info[i].ItemType == typeof(BarButtonGroup)) return info[i];
			}
			return null;
		}
		protected BarManagerCategory SelCategory {
			get { return categories.SelectedItem as BarManagerCategory; }
		}
		protected virtual void AddButtonGroup_Click(object sender, EventArgs e) {
			BarItemInfo[] info = RibbonDesignTimeHelpers.GetItemInfoList(Ribbon);
			ItemsList.CreateNewItem(GetButtonGroupInfo(info), null, Ribbon, SelCategory);
			UpdateBarButtons();
		}
		protected virtual void RemoveItem_Click(object sender, EventArgs e) {
			ItemsListHelper.RemoveItems(ItemsTree, ItemsList);
			itemsList_SelectedIndexChanged(sender, EventArgs.Empty);
		}
		protected virtual void UpdateRemoveItemButton() {
			bool enabled = true;
			if(itemsList.SelectedIndices.Count == 0 || itemsList.Items.Count == 0) enabled = false;
			ItemsToolbar.RemoveItemButton.Enabled = enabled;
		}
		protected virtual void UpdateRemoveButton() {
			bool enabled = true;
			if(ItemsTree.Nodes.Count == 0 || ItemsTree.SelectedNode == null) enabled = false;
			LinksToolbar.RemoveButton.Enabled = enabled;
		}
		protected virtual void UpdateMoveUpDownButtons() {
			if(ItemsTree.SelectedNode == null || ItemsTree.SelNodes.Length > 1) {
				LinksToolbar.MoveUpButton.Enabled = LinksToolbar.MoveDownButton.Enabled = false;
				return;
			}
			if(ItemsTree.SelectedNode.PrevNode == null) {
				LinksToolbar.MoveUpButton.Enabled = false;
			}
			else {
				LinksToolbar.MoveUpButton.Enabled = true;
			}
			if(ItemsTree.SelectedNode.NextNode == null) {
				LinksToolbar.MoveDownButton.Enabled = false;
			}
			else {
				LinksToolbar.MoveDownButton.Enabled = true;
			}
		}
		protected virtual void UpdateBarButtons() {
			if(!AllowUpdateToolbarButtons) return;
			UpdateMoveUpDownButtons();
			UpdateRemoveButton();
			UpdateRemoveItemButton();
			UpdateBeginGroup();
		}
		protected virtual void categories_SelectedIndexChanged(object sender, System.EventArgs e) {
			ItemsList.FillListBox(Categories.SelCategory, Categories.SelectedIndex);
			UpdateBarButtons();
		}
		void itemsList_SelectedIndexChanged(object sender, System.EventArgs e) {
			BarLinkInfoProvider.Reset(Ribbon.Items);
			if(ItemsList.SelectedObjects != null && ItemsList.SelectedObjects[0] != null)
				pgMain.SelectedObjects = ItemsList.SelectedObjects;
			UpdateBarButtons();
		}
		RibbonPage GetOwnerPage(TreeNode node) {
			if(node == null) return null;
			if(node.Tag as RibbonPage != null) return node.Tag as RibbonPage;
			return GetOwnerPage(node.Parent);
		}
		private void SelectRibbonPage() {
			if(ItemsTree.SelNodes.Length > 0) {
				Ribbon.SelectedPage = GetOwnerPage(ItemsTree.SelNodes[0]);
			}
		}
		private void itemsTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			UpdateBarButtons();
		}
		private void itemsTree_SelectionChanged(object sender, System.EventArgs e) {
			BarLinkInfoProvider.Reset(Ribbon.Items);
			SelectRibbonPage();
			UpdateBeginGroup();
			foreach(BarItemLink link in ItemsTree.SelectedLinks) {
				BarLinkInfoProvider.SetLinkInfo(link.Item, link);
			}
			if(ItemsTree.SelectedTreeItems.Length > 0 && ItemsTree.SelectedTreeItems[0] != null)
				pgMain.SelectedObjects = ItemsTree.SelectedTreeItems;
		}
		private void itemsTree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) ItemsTree.Remove();
			UpdateBarButtons();
		}
		private void itemsList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				ItemsListHelper.RemoveItems(ItemsTree, ItemsList);
				itemsList_SelectedIndexChanged(sender, EventArgs.Empty);
			}
			UpdateBarButtons();
		}
		protected void OnObjectPropertiesChanged(object sender, PropertyValueChangedEventArgs e) {
			for(int i = 0; i < pgMain.SelectedObjects.Length; i++) {
				ItemsTree.UpdateTreeNodesText(pgMain.SelectedObjects[i]);
			}
			UpdateItemsTreeNodes();
			UpdateBeginGroup();
			ItemsList.Update();
			ItemsList.Invalidate();
			Ribbon.Refresh();
		}
		protected virtual void UpdateItemsTreeNodes() {
			for(int i = 0; i < ItemsTree.SelNodes.Length; i++) {
				ItemsTree.ResetNodeImage(ItemsTree.SelNodes[i]);
			}
		}
		protected virtual void OnBeginGroupChanged(bool bChecked) {
			for(int i = 0; i < ItemsTree.SelNodes.Length; i++) {
				(ItemsTree.SelNodes[i].Tag as BarItemLink).BeginGroup = bChecked;
			}
			UpdateItemsTreeNodes();
		}
		private void beginGroup_CheckedChanged(object sender, System.EventArgs e) {
			OnBeginGroupChanged(BeginGroup.Checked);
			Ribbon.Refresh();
			if(StatusBar != null) StatusBar.Refresh();
		}
		protected void itemsTree_Enter(object sender, System.EventArgs e) {
			itemsTree_SelectionChanged(sender, e);
		}
		protected void itemsList_Enter(object sender, System.EventArgs e) {
			itemsList_SelectedIndexChanged(sender, e);
		}
	}
	internal class TablePanelControl : PanelControl {
		TableLayoutPanel tableLayoutPanel;
		public TablePanelControl() {
			InitializeComponent();
		}
		void InitializeComponent() {
			this.tableLayoutPanel = new TableLayoutPanel();
			TableLayoutPanel.ColumnCount = 2;
			TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
			TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			TableLayoutPanel.Name = "tableLayoutPanel";
			TableLayoutPanel.RowCount = 1;
			TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			TableLayoutPanel.Size = new System.Drawing.Size(100, 100);
			TableLayoutPanel.TabIndex = 0;
			Controls.Add(TableLayoutPanel);
		}
		public void AddControl(Control control, int col, int row) {
			TableLayoutPanel.Controls.Add(control, col, row);
		}
		public TableLayoutPanel TableLayoutPanel { get { return tableLayoutPanel; } }
	}
}
