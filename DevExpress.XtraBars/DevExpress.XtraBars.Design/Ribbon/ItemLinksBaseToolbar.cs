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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Customization.Helpers;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
namespace DevExpress.XtraBars.Ribbon.Design {
	[ToolboxItem(false)]
	public class ItemLinksBaseToolbar : DevExpress.XtraEditors.XtraUserControl {
		PopupMenu btItemTypeMenu;
		IContainer components = null;
		BarItemInfo selBarItem = null;
		EditorClassInfo selEditorInfo = null;
		ImageCollection itemTypeImages;
		protected BarAndDockingController barAndDockingController1;
		BarManager barManager;
		private FlowLayoutPanel flowLayoutPanel;
		private DropDownButton ddAddItem;
		private SimpleButton btnRemoveItem;
		private SimpleButton btnAddButtonGroup;
		private SimpleButton btnRemove;
		private SimpleButton btnMoveDown;
		private SimpleButton btnMoveUp;
		private ImageCollection imgs;
		private LabelControl moveDeleteSeparator;
		private BarDockControl barDockControlTop;
		private BarDockControl barDockControlBottom;
		private BarDockControl barDockControlLeft;
		private BarDockControl barDockControlRight;
		private XtraEditors.CheckButton btnSearch;
		private LabelControl addDeleteSeparator;
		public ItemLinksBaseToolbar() {
			InitializeComponent();
			FilterButtons();
			Stream str = typeof(BarManager).Assembly.GetManifestResourceStream("DevExpress.XtraBars.BarItems.bmp");
			this.itemTypeImages = new ImageCollection();
			this.itemTypeImages.Images.AddImageStrip(Bitmap.FromStream(str));
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitTooltips();
			InitSearchButton();
		}
		protected virtual void InitSearchButton() {
			this.btnSearch.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.FindImage;
		}
		protected virtual void InitTooltips() {
			foreach(Control control in ButtonContainer.Controls) {
				SimpleButton button = control as SimpleButton;
				if(button != null && button.Tag != null)
					button.SuperTip = CreateSuperTip((string)button.Tag);
			}
		}
		protected SuperToolTip CreateSuperTip(string text) {
			SuperToolTip superTip = new SuperToolTip();
			superTip.Items.Add(new ToolTipItem() { Text = text });
			return superTip;
		}
		public Control ButtonContainer { get { return flowLayoutPanel; } }
		protected ImageCollection SmallImages { get { return imgs; } }
		protected virtual void FilterButtons() {
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemLinksBaseToolbar));
			this.btItemTypeMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.itemTypeImages = new DevExpress.Utils.ImageCollection(this.components);
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.btnMoveUp = new DevExpress.XtraEditors.SimpleButton();
			this.imgs = new DevExpress.Utils.ImageCollection(this.components);
			this.btnMoveDown = new DevExpress.XtraEditors.SimpleButton();
			this.moveDeleteSeparator = new DevExpress.XtraEditors.LabelControl();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.ddAddItem = new DevExpress.XtraEditors.DropDownButton();
			this.btnAddButtonGroup = new DevExpress.XtraEditors.SimpleButton();
			this.addDeleteSeparator = new DevExpress.XtraEditors.LabelControl();
			this.btnRemoveItem = new DevExpress.XtraEditors.SimpleButton();
			this.btnSearch = new DevExpress.XtraEditors.CheckButton();
			((System.ComponentModel.ISupportInitialize)(this.btItemTypeMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemTypeImages)).BeginInit();
			this.flowLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgs)).BeginInit();
			this.SuspendLayout();
			this.btItemTypeMenu.Manager = this.barManager;
			this.btItemTypeMenu.Name = "btItemTypeMenu";
			this.barManager.AllowShowToolbarsPopup = false;
			this.barManager.Controller = this.barAndDockingController1;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 30;
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(425, 0);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 35);
			this.barDockControlBottom.Size = new System.Drawing.Size(425, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 35);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(425, 0);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 35);
			this.itemTypeImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("itemTypeImages.ImageStream")));
			this.flowLayoutPanel.Controls.Add(this.btnMoveUp);
			this.flowLayoutPanel.Controls.Add(this.btnMoveDown);
			this.flowLayoutPanel.Controls.Add(this.moveDeleteSeparator);
			this.flowLayoutPanel.Controls.Add(this.btnRemove);
			this.flowLayoutPanel.Controls.Add(this.ddAddItem);
			this.flowLayoutPanel.Controls.Add(this.btnAddButtonGroup);
			this.flowLayoutPanel.Controls.Add(this.addDeleteSeparator);
			this.flowLayoutPanel.Controls.Add(this.btnRemoveItem);
			this.flowLayoutPanel.Controls.Add(this.btnSearch);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(425, 35);
			this.flowLayoutPanel.TabIndex = 17;
			this.flowLayoutPanel.WrapContents = false;
			this.btnMoveUp.AllowFocus = false;
			this.btnMoveUp.ImageIndex = 6;
			this.btnMoveUp.ImageList = this.imgs;
			this.btnMoveUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnMoveUp.Location = new System.Drawing.Point(0, 0);
			this.btnMoveUp.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(28, 30);
			this.btnMoveUp.TabIndex = 14;
			this.btnMoveUp.Tag = "Move Up";
			this.imgs.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgs.ImageStream")));
			this.imgs.Images.SetKeyName(0, "AddCategory.png");
			this.imgs.Images.SetKeyName(1, "AddPage.png");
			this.imgs.Images.SetKeyName(2, "AddGroup.png");
			this.imgs.Images.SetKeyName(3, "AddButtomGroup.png");
			this.imgs.Images.SetKeyName(4, "AddItem.png");
			this.btnMoveDown.AllowFocus = false;
			this.btnMoveDown.ImageIndex = 7;
			this.btnMoveDown.ImageList = this.imgs;
			this.btnMoveDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnMoveDown.Location = new System.Drawing.Point(34, 0);
			this.btnMoveDown.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(28, 30);
			this.btnMoveDown.TabIndex = 13;
			this.btnMoveDown.Tag = "Move Down";
			this.moveDeleteSeparator.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.moveDeleteSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.moveDeleteSeparator.LineLocation = DevExpress.XtraEditors.LineLocation.Left;
			this.moveDeleteSeparator.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.moveDeleteSeparator.LineVisible = true;
			this.moveDeleteSeparator.Location = new System.Drawing.Point(72, 1);
			this.moveDeleteSeparator.Margin = new System.Windows.Forms.Padding(7, 1, 5, 0);
			this.moveDeleteSeparator.Name = "moveDeleteSeparator";
			this.moveDeleteSeparator.Size = new System.Drawing.Size(3, 28);
			this.moveDeleteSeparator.TabIndex = 19;
			this.btnRemove.AllowFocus = false;
			this.btnRemove.ImageIndex = 5;
			this.btnRemove.ImageList = this.imgs;
			this.btnRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemove.Location = new System.Drawing.Point(83, 0);
			this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(28, 30);
			this.btnRemove.TabIndex = 12;
			this.btnRemove.Tag = "Remove";
			this.ddAddItem.AllowFocus = false;
			this.ddAddItem.DropDownControl = this.btItemTypeMenu;
			this.ddAddItem.ImageIndex = 4;
			this.ddAddItem.ImageList = this.imgs;
			this.ddAddItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
			this.ddAddItem.Location = new System.Drawing.Point(114, 0);
			this.ddAddItem.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.ddAddItem.MenuManager = this.barManager;
			this.ddAddItem.Name = "ddAddItem";
			this.ddAddItem.Size = new System.Drawing.Size(80, 30);
			this.ddAddItem.TabIndex = 17;
			this.ddAddItem.Text = "Button";
			this.ddAddItem.Click += new System.EventHandler(this.OnCreateItemDropDownButtonClick);
			this.btnAddButtonGroup.AllowFocus = false;
			this.btnAddButtonGroup.ImageIndex = 3;
			this.btnAddButtonGroup.ImageList = this.imgs;
			this.btnAddButtonGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAddButtonGroup.Location = new System.Drawing.Point(200, 0);
			this.btnAddButtonGroup.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.btnAddButtonGroup.Name = "btnAddButtonGroup";
			this.btnAddButtonGroup.Size = new System.Drawing.Size(28, 30);
			this.btnAddButtonGroup.TabIndex = 15;
			this.btnAddButtonGroup.Tag = "Add Button Group";
			this.addDeleteSeparator.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.addDeleteSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.addDeleteSeparator.LineLocation = DevExpress.XtraEditors.LineLocation.Left;
			this.addDeleteSeparator.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.addDeleteSeparator.LineVisible = true;
			this.addDeleteSeparator.Location = new System.Drawing.Point(238, 1);
			this.addDeleteSeparator.Margin = new System.Windows.Forms.Padding(7, 1, 5, 0);
			this.addDeleteSeparator.Name = "addDeleteSeparator";
			this.addDeleteSeparator.Size = new System.Drawing.Size(3, 28);
			this.addDeleteSeparator.TabIndex = 20;
			this.btnRemoveItem.AllowFocus = false;
			this.btnRemoveItem.ImageIndex = 5;
			this.btnRemoveItem.ImageList = this.imgs;
			this.btnRemoveItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemoveItem.Location = new System.Drawing.Point(249, 0);
			this.btnRemoveItem.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.btnRemoveItem.Name = "btnRemoveItem";
			this.btnRemoveItem.Size = new System.Drawing.Size(30, 30);
			this.btnRemoveItem.TabIndex = 16;
			this.btnRemoveItem.Tag = "Remove";
			this.btnSearch.AllowFocus = false;
			this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnSearch.Location = new System.Drawing.Point(285, 0);
			this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(28, 30);
			this.btnSearch.TabIndex = 21;
			this.btnSearch.Tag = "Search";
			this.btnSearch.Text = "S";
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ItemLinksBaseToolbar";
			this.Size = new System.Drawing.Size(425, 35);
			((System.ComponentModel.ISupportInitialize)(this.btItemTypeMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemTypeImages)).EndInit();
			this.flowLayoutPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imgs)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		void UpdateItemTypeText() {
			UpdateItemTypeText(false);
		}
		void UpdateItemTypeText(bool useEditorInfo) { 
			if(SelBarItem == null)
				return;
			int prevWidth = AddItemButton.CalcBestSize().Width;
			try {
				string caption = SelBarItem.GetShortCaption();
				AddItemButton.Text = caption;
				AddItemButton.SuperTip = CreateSuperTip(string.Format("Add {0}", caption));
				if(useEditorInfo) AddItemButton.Text += " (" + SelEditorInfo.Name + ")";
			}
			finally {
				int nextWidth = AddItemButton.CalcBestSize().Width;
				if(nextWidth != prevWidth)
					AddItemButton.Width += (nextWidth - prevWidth);
			}
		}
		public BarButtonItem CreateMenuButtonItem(BarItemInfo info, ImageCollection images) {
			BarButtonItem button = new BarButtonItem(Manager, info.GetShortCaption(), 0, null);
			if(ImageCollection.IsImageExists(null, images, info.ImageIndex))
				button.Glyph = images.Images[info.ImageIndex];
			button.Tag = info;
			button.ItemClick += new ItemClickEventHandler(OnItemTypeClick);
			return button;
		}
		public BarSubItem CreateMenuSubItem(BarItemInfo info, ImageCollection images) {
			BarSubItem subItem = new BarSubItem(Manager, info.GetShortCaption(), 0, null);
			DevExpress.XtraBars.Customization.Helpers.DesignTimeManager.DesignTimeCreateItemMenu.CreateEditors(subItem, info, Manager, new ItemClickEventHandler(OnCreateEditorClick));
			return subItem;
		}
		public BarManager Manager { get { return barManager; } }
		public void InitializeItemTypeMenu(ImageCollection images) {
			if(Manager == null) return;
			BarItemInfo[] itemInfo = BarUtilites.GetItemInfoList(Manager);
			int[] ImageIndex = { };
			for(int i = 0; i < itemInfo.Length; i++) {
				BarItemInfo info = itemInfo[i];
				if(info.ItemType.Equals(typeof(BarEditItem)) || info.ItemType.IsSubclassOf(typeof(BarEditItem))) {
					btItemTypeMenu.ItemLinks.Add(CreateMenuSubItem(info, images));
				}
				else {
					btItemTypeMenu.ItemLinks.Add(CreateMenuButtonItem(info, images));
				}
			}
			SelBarItem = itemInfo[0];
			UpdateItemTypeText();
		}
		protected void OnCreateEditorClick(object sender, ItemClickEventArgs e) {
			DesignTimeManager.DesignTimeCreateItemMenu.EditorInfo info = e.Item.Tag as DesignTimeManager.DesignTimeCreateItemMenu.EditorInfo;
			if(info == null || info.ItemInfo == null) return;
			SelBarItem = info.ItemInfo as BarItemInfo;
			SelEditorInfo = info.Editor;
			UpdateItemTypeText(false);
			OnCreateItem(info.ItemInfo, info.Editor == null ? null : info.Editor.Name);
		}
		void btAddItem_ItemClick(object sender, ItemClickEventArgs e) {
			OnCreateItem(SelBarItem, SelEditorInfo == null ? null: SelEditorInfo.Name);
		}
		void OnCreateItemDropDownButtonClick(object sender, EventArgs e) {
			if(SelBarItem != null)
				OnCreateItem(SelBarItem, SelEditorInfo == null ? null : SelEditorInfo.Name);
		}
		protected void OnItemClick(object sender, ItemClickEventArgs e) {
			BarItemInfo info = e.Item.Tag as BarItemInfo;
			if(info == null) return;
			OnCreateItem(info, null);
		}
		void OnItemTypeClick(object sender, ItemClickEventArgs e) { 
			SelBarItem = e.Item.Tag as BarItemInfo;
			UpdateItemTypeText();
			OnCreateItem(SelBarItem, null);
		}
		protected virtual void OnCreateItem(BarItemInfo info, object arguments) {
			if(CreateItem != null) CreateItem(this, new ItemCreateEventArgs(info, arguments));
		}
		public event ItemCreateEventHandler CreateItem;
		public ImageCollection ItemTypeImages { get { return itemTypeImages; } }
		public BarItemInfo SelBarItem { get { return selBarItem; } set { selBarItem = value; } }
		public EditorClassInfo SelEditorInfo { get { return selEditorInfo; } set { selEditorInfo = value; } }
		public SimpleButton MoveUpButton { get { return btnMoveUp; } }
		public SimpleButton MoveDownButton { get { return btnMoveDown; } }
		public SimpleButton RemoveButton { get { return btnRemove; } }
		public DropDownButton AddItemButton { get { return ddAddItem; } }
		public SimpleButton RemoveItemButton { get { return btnRemoveItem; } }
		public SimpleButton AddButtonGroup { get { return btnAddButtonGroup; } }
		public Control MoveDeleteSeparator { get { return moveDeleteSeparator; } }
		public Control AddDeleteSeparator { get { return addDeleteSeparator; } }
		public DevExpress.XtraEditors.CheckButton SearchButton { get { return btnSearch; } }
	}
	[ToolboxItem(false)]
	public class LinksToolbar : ItemLinksBaseToolbar {
		protected override void FilterButtons() {
			base.FilterButtons();
			AddItemButton.Visible = RemoveItemButton.Visible = AddButtonGroup.Visible = AddDeleteSeparator.Visible = false;
		}
	}
	[ToolboxItem(false)]
	public class ItemsToolbar : ItemLinksBaseToolbar {
		protected override void FilterButtons() {
			base.FilterButtons();
			MoveUpButton.Visible = MoveDownButton.Visible = RemoveButton.Visible = MoveDeleteSeparator.Visible = false;
		}
	}
	public delegate void ItemCreateEventHandler(object sender, ItemCreateEventArgs e);
	public class ItemCreateEventArgs : EventArgs {
		BarItemInfo itemInfo;
		object arguments;
		public ItemCreateEventArgs(BarItemInfo itemInfo, object arguments) {
			this.itemInfo = itemInfo;
			this.arguments = arguments;
		}
		public BarItemInfo ItemInfo { get { return itemInfo; } }
		public object Arguments { get { return arguments; } }
	}
}
