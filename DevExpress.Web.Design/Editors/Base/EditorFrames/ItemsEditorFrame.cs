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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.Web.Design {
	public class ItemsEditorFrame : TwoSidesEditorFrame {
		const string PropretyGridName = "PropertyGrid";
		const string LabelInfoName = "LabelInfoName";
		const string TextEmptyProperties = "No item selected";
		const int ToolbarPanelHeight = 46;
		protected string ActionRemoveConfirmMessage = "Are you sure you want to remove the selected items with their child items?";
		protected string ActionConfirmRemoveUncheckedItems = "Are you sure you want to remove the unchecked items?";
		protected string ActionChangeToConfirmMessage = "A selected item contains child items. Are you sure you want to continue operation?\nAll the child items will be removed in this case.";
		protected override string FrameName { get { return "ItemsEditorFrame"; } }
		ToolbarItemsGenerator toolBarGenerator;
		Dictionary<string, LabelControl> infoLabels;
		ImageCollection iconCollection;
		BarManager barManager;
		List<string> displayFields;
		Dictionary<string, ImageCollection> spriteCache;
		ItemsEditorOwner itemsOwner;
		public ItemsEditorFrame() 
			: this(null) {
		}
		public ItemsEditorFrame(ItemsEditorOwner itemsOwner)
			: base() {
			ItemsOwner = itemsOwner;
			PostponeCreateControls += (s, e) => { PostponeInitializeFrame(); };
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			Initialize();
		}
		protected void Initialize() {
			InitializeItemsOwner();
			InitializeFrame();
		}
		protected void InitializeItemsOwner() {
			if(ItemsOwner == null)
				ItemsOwner = (ItemsEditorOwner)DesignerItem.Tag;
			ItemsOwner.OnUpdateImageMap += UpdateIconCollection;
			ItemsOwner.OnEndDataUpdate = EndDataOwnerUpdate;
		}
		protected ImageCollection IconCollection {
			get {
				if(iconCollection == null)
					iconCollection = new ImageCollection();
				return iconCollection;
			}
		}
		protected PopupControlContainer RetrieveFieldsPopup { get; private set; }
		protected BarManager BarManager {
			get {
				if(barManager == null)
					barManager = MenuManagerHelper.GetMenuManager(LookAndFeel, this) as BarManager;
				return barManager;
			}
		}
		protected ItemsDesignerTreeList TreeListItems { get; private set; }
		protected CheckBox CheckAll { get; private set; }
		protected ItemsEditorOwner ItemsOwner {
			get { return itemsOwner; }
			set {
				itemsOwner = value;
				if(itemsOwner != null)
					CommonDesignerServiceRegisterHelper.RegisterItemsOwner(itemsOwner.ServiceProvider, itemsOwner, DesignerItem);
			}
		}
		protected DataItemsEditorOwner DataOwner { get { return ItemsOwner as DataItemsEditorOwner; } }
		protected SimpleButton ButtonRetrieveFieldsCancel { get; set; }
		protected SimpleButton ButtonRetrieveFieldsOk { get; set; }
		protected CheckedListBoxControl RetrieveFieldsCheckedListBox { get; set; }
		public DXPropertyGridEx ItemPropertyGrid { get; private set; }
		protected DXPropertyGridEx EditorPropertyGrid { get; private set; }
		protected internal XtraTabControl PropertiesTabControl { get; private set; }
		protected XtraTabPage ItemPropertiesTab { get; private set; }
		protected XtraTabPage EditorPropertiesTab { get; private set; }
		protected PanelControl TopLevelPanel { get; set; }
		protected PanelControl ToolbarPanel { get; set; }
		protected ToolbarItemsGenerator ToolBarGenerator {
			get {
				if(toolBarGenerator == null)
					toolBarGenerator = new ToolbarItemsGenerator(ItemsOwner, ToolbarPanel, IconCollection, ToolBarButtonClick);
				return toolBarGenerator;
			}
		}
		protected List<string> DisplayFields {
			get {
				if(displayFields == null)
					displayFields = new List<string>();
				return displayFields;
			}
		}
		bool NeedConfirmation { get; set; }
		Size RetrieveFieldsSize { get; set; }
		bool InnerControlsCreated { get; set; }
		internal Dictionary<string, LabelControl> InfoLabels {
			get {
				if(infoLabels == null)
					infoLabels = new Dictionary<string, LabelControl>();
				return infoLabels;
			}
		}
		Dictionary<string, ImageCollection> SpriteCache {
			get {
				if(spriteCache == null)
					spriteCache = new Dictionary<string, ImageCollection>();
				return spriteCache;
			}
		}
		void InitializeFrame() {
			NeedConfirmation = true;
			Text = ItemsOwner.GetFormCaption();
			Font = GetDialogFont(ItemsOwner.ServiceProvider);
			ClientSize = new Size(580, 610);
		}
		protected virtual void EndDataOwnerUpdate(TreeListNodesState validatedState) {
			TreeListItems.EndDataOwnerUpdate(validatedState);
		}
		protected void PostponeInitializeFrame() {
			PostponedCreateInnerControls();
			ItemsOwner.OnComponentChanged += OnEditingComponentChanged;
			UpdateTreeListItems();
			SelectionChanged();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible) {
				if(DataOwner != null) {
					DataOwner.ResetDataSourceFieldInfo();
					FillToolbarItems();
					FillRetireveFieldsCheckedList();
				}
				UpdateTreeListItems();
			}
		}
		protected virtual void UpdateTreeListItems() {
			if(TreeListItems != null) {
				TreeListItems.ClearNodes();
				ItemsOwner.RecreateTreeListItems(false);
				if(TreeListItems.Nodes.Count > 0) {
					TreeListItems.FocusedNode = TreeListItems.Nodes.FirstNode;
					TreeListItems.Selection.Add(TreeListItems.FocusedNode);
				}
			}
		}
		protected override void OnInitEmbeddedFrame(EmbeddedFrameInit frameInit) {
			base.OnInitEmbeddedFrame(frameInit);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ItemPropertyGrid != null && ItemPropertyGrid.Visible)
				UpdateLabelForEmptyPropertiesLocation(ItemPropertyGrid);
			if(EditorPropertyGrid != null && EditorPropertyGrid.Visible)
				UpdateLabelForEmptyPropertiesLocation(EditorPropertyGrid);
		}
		bool PostponedCreateInnerControls() {
			if(InnerControlsCreated)
				return false;
			SuspendLayout();
			CreateToolbarPanel();
			CreateRetrieveFieldsPopup();
			CreateIconCollection();
			CreateTopLevelEditor();
			CreateToolBarLinks();
			CreateItemsTreeList();
			CreateTabControl();
			RecalcMainPanelSize();
			InnerControlsCreated = true;
			ResumeLayout(true);
			return true;
		}
		protected void CreateTopLevelEditor() {
			if(!ItemsOwner.HasTopLevelEditor)
				return;
			TopPanel.Height = 76;
			TopLevelPanel = DesignTimeFormHelper.CreatePanel(TopPanel, "TopLevelPanel", DockStyle.Top);
			TopLevelPanel.Height = ToolbarPanel.Top;
			BaseEdit topLevelControl = null;
			switch(ItemsOwner.GetTopLevelEditorType()) {
				case PropertyEditorType.TextEdit:
					topLevelControl = new TextEdit() { Name = "TextEditTopLevelPropertyEditor" };
					break;
				case PropertyEditorType.ComboBox:
					topLevelControl = new ComboBoxEdit() { Name = "ComboBoxTopLevelPropertyEditor" };
					break;
			}
			var label = new LabelControl() { Name = "LabelTopLevelPropertyEditor" };
			label.Text = ItemsOwner.GetTopLevelEditorName();
			label.Parent = TopLevelPanel;
			label.Width = DesignTimeFormHelper.GetTextWidth(this, label.Text, label.Font) + 8;
			label.Top = TopLevelPanel.Height / 2 - label.Height / 2;
			label.Left = 8;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			topLevelControl.Parent = TopLevelPanel;
			topLevelControl.EditValue = ItemsOwner.GetDependendPropertyValue();
			topLevelControl.EditValueChanged += (s, e) => { ItemsOwner.SetDependendPropertyValue(topLevelControl.EditValue); };
			topLevelControl.Top = label.Top - 2;
			topLevelControl.Left = label.Left + label.Width + 8;
			topLevelControl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
		}
		protected void CreateToolBarLinks() {
			if(RetrieveFieldsPopup != null)
				ToolBarGenerator.AddActionPopup(DesignEditorMenuRootItemActionType.RetriveFields, RetrieveFieldsPopup);
			ToolBarGenerator.AddActionHandler(DesignEditorMenuRootItemActionType.SetItemsAmount, SpinEditItemLeave);
			ToolBarGenerator.GenerateToolBarLinks();			
		}
		protected void CreateToolbarPanel() {
			ToolbarPanel = DesignTimeFormHelper.CreatePanel(TopPanel, "TopPanel", DockStyle.Fill);
			if(ItemsOwner.HasTopLevelEditor || HasLabelCaption) {
				ToolbarPanel.Dock = DockStyle.Bottom;
				ToolbarPanel.Height = ToolbarPanelHeight;
			}
		}
		protected virtual void CreateRetrieveFieldsPopup() {
			if(!ItemsOwner.SupportDataSource)
				return;
			RetrieveFieldsPopup = new PopupControlContainer(Components);
			MainPanel.Controls.Add(RetrieveFieldsPopup);
			RetrieveFieldsPopup.BorderStyle = BorderStyles.Default;
			RetrieveFieldsPopup.FormMinimumSize = RetrieveFieldsPopup.Size = new Size(200, 210);
			if(!RetrieveFieldsSize.IsEmpty)
				RetrieveFieldsPopup.Size = RetrieveFieldsSize;
			RetrieveFieldsPopup.ShowSizeGrip = true;
			RetrieveFieldsPopup.TabIndex = 1;
			RetrieveFieldsPopup.Visible = false;
			RetrieveFieldsPopup.Popup += PrepareFieldsPopup;
			RetrieveFieldsPopup.Manager = BarManager;
			CreatePopupRetrieveFieldsCheckAll();
			CreateRetrieveFieldsCheckListBox();
			CreateRetrieveFieldsOkCancelButtons();
			SetupRetrieveColumnsControl();
		}
		void CreateRetrieveFieldsOkCancelButtons() {
			var buttonSize = new Size(80, 23);
			int top = (RetrieveFieldsCheckedListBox.Top + RetrieveFieldsCheckedListBox.Height + RetrieveFieldsPopup.Height - buttonSize.Height) / 2;
			var location = new Point(RetrieveFieldsPopup.Width - (buttonSize.Width + 15) * 2, top);
			ButtonRetrieveFieldsOk = DesignTimeFormHelper.CreateButton(RetrieveFieldsPopup, buttonSize, location, 0, "OK", DialogResult.None, btnRetrieveFieldsContainerOK_Click);
			location.X += buttonSize.Width + 15;
			ButtonRetrieveFieldsCancel = DesignTimeFormHelper.CreateButton(RetrieveFieldsPopup, buttonSize, location, 1, "Cancel", DialogResult.None, btnRetrieveFieldsContainerCancel_Click);
		}
		void CreateRetrieveFieldsCheckListBox() { 
			RetrieveFieldsCheckedListBox = new CheckedListBoxControl();
			RetrieveFieldsPopup.Controls.Add(RetrieveFieldsCheckedListBox);
			RetrieveFieldsCheckedListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			RetrieveFieldsCheckedListBox.CheckOnClick = true;
			RetrieveFieldsCheckedListBox.HotTrackItems = true;
			RetrieveFieldsCheckedListBox.IncrementalSearch = true;
			RetrieveFieldsCheckedListBox.Location = new Point(0, 30);
			RetrieveFieldsCheckedListBox.Size = new Size(RetrieveFieldsPopup.Width,RetrieveFieldsPopup.Height - RetrieveFieldsCheckedListBox.Top - 35);
			RetrieveFieldsCheckedListBox.TabIndex = 1;
			RetrieveFieldsCheckedListBox.DrawItem += clbFields_DrawItem;
		}
		void CreatePopupRetrieveFieldsCheckAll() { 
			CheckAll = new CheckBox();
			RetrieveFieldsPopup.Controls.Add(CheckAll);
			CheckAll.AutoSize = true;
			CheckAll.Location = new Point(14, 7);
			CheckAll.Size = new Size(70, 17);
			CheckAll.TabIndex = 0;
			CheckAll.Text = "Check all";
			CheckAll.UseVisualStyleBackColor = true;
			CheckAll.CheckedChanged += RetrieveFieldsCheckAllChanged;
		}
		void CreateTabControl() {
			PropertiesTabControl = new XtraTabControl();
			RightPanel.Controls.Add(PropertiesTabControl);
			PropertiesTabControl.Dock = DockStyle.Fill;
			PropertiesTabControl.TabIndex = 3;
			CreatePropertyTabs();			
		}
		void CreatePropertyTabs() {
			ItemPropertiesTab = CreatePropertyTabPage("ItemPropertiesTab");
			ItemPropertiesTab.Text = ItemsOwner.GetItemPropertiesTabCaption();
			ItemPropertyGrid = CreatePropertyGrid(ItemPropertiesTab);
			EditorPropertiesTab = CreatePropertyTabPage("EditorPropertiesTab");
			EditorPropertyGrid = CreatePropertyGrid(EditorPropertiesTab); 
			PropertiesTabControl.SelectedTabPage = ItemPropertiesTab;
			PropertiesTabControl.TabPages.AddRange(new XtraTabPage[] { ItemPropertiesTab, EditorPropertiesTab });
			PropertiesTabControl.SelectedPageChanged += PropertiesTabControl_SelectedPageChanged;
		}
		XtraTabPage CreatePropertyTabPage(string name) {
			var tabPage = new XtraTabPage();
			tabPage.Size = PropertiesTabControl.Size;
			tabPage.Name = name;
			tabPage.Appearance.Header.Font = Font;
			CreateLabelInfo(tabPage, TextEmptyProperties);
			return tabPage;
		}
		DXPropertyGridEx CreatePropertyGrid(Control parent) {
			var propertyGrid = new DXPropertyGridEx();
			parent.Controls.Add(propertyGrid);
			propertyGrid.SelectedObjectsChanged += propertyGrid_SelectedObjectsChanged;
			propertyGrid.Name = PropretyGridName;
			var serviceProvider = ItemsOwner.ServiceProvider;
			if(serviceProvider != null)
				propertyGrid.Site = new FormPropertyGridSite(serviceProvider, propertyGrid);
			propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			propertyGrid.Size = parent.Size;
			propertyGrid.Tag = FindInnerPropertyGrid(propertyGrid);
			propertyGrid.SizeChanged += PropertyGrid_SizeChanged;
			propertyGrid.PropertyValueChanged += (s, e) => {
				CommonDesignerUndoHelper.SavePropertyGridChangedValue(ItemsOwner.ServiceProvider, (DXPropertyGridEx)s, e);
			};
			propertyGrid.Font = Font;
			return propertyGrid;
		}
		void CreateLabelInfo(Control container, string text) {
			var label = new LabelControl();
			label.Name = LabelInfoName;
			container.Controls.Add(label);
			InfoLabels[container.Name] = label;
			label.Font = new Font(container.Font.FontFamily, 16, container.Font.Style, container.Font.Unit, container.Font.GdiCharSet, false);
			label.ForeColor = Color.LightGray;
			label.Text = text;
			label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			label.BringToFront();
			label.Visible = false;
		}
		void PropertiesTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			UpdateLabelForEmptyPropertiesLocation(GetPropertyGrid(e.Page));
		}
		void PropertiesContainer_SizeChanged(object sender, EventArgs e) {
			UpdateLabelForEmptyPropertiesLocation((Control)sender);
		}
		void PropertyGrid_SizeChanged(object sender, EventArgs e) {
			UpdateLabelForEmptyPropertiesLocation((Control)sender);
		}
		void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e) {
			var propertyGrid = (DXPropertyGridEx)sender;
			var isValidProperties = ValidatePropertyGridSelectedObject(propertyGrid);
			UpdateLabelForEmptyPropertiesLocation(propertyGrid);
			InfoLabels[propertyGrid.Parent.Name].Visible = !isValidProperties;
			propertyGrid.Enabled = isValidProperties;
		}
		bool ValidatePropertyGridSelectedObject(DXPropertyGridEx propertyGrid) {
			return propertyGrid.SelectedObjects != null && propertyGrid.SelectedObjects.Length > 0;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (Components != null))
				Components.Dispose();
			base.Dispose(disposing);
		}
		void CreateIconCollection() {
			IconCollection.Clear();
			foreach(var imageResourceUrl in ItemsOwner.ResourceImageMap.OrderBy(i => i.Value).Select(i => i.Key))
				UpdateIconCollection(imageResourceUrl);
		}
		void UpdateIconCollection(string resourceUrl) { 
				var imageInfo = resourceUrl.Split(',');
				if(imageInfo.Length > 1)
					LoadSpriteIcon(imageInfo);
				else
					LoadIcon(imageInfo[0]);
		}
		void LoadIcon(string imageUrl) {
			var image = CreateBitmapFromResources(imageUrl);
			if(image != null) {
				image.MakeTransparent(Color.FromArgb(255, 0, 255));
				IconCollection.AddImage(image);
			}
		}
		void LoadSpriteIcon(string[] imageInfo) {
			var imageCollection = GetSpriteImageCollection(imageInfo[0]);
			if(imageCollection == null)
				return;
			var imageList = imageCollection.Images;
			var index = -1;
			int.TryParse(imageInfo[1], out index);
			if(index >= 0 && imageList.Count > index)
				IconCollection.AddImage(imageList[index]);
		}
		ImageCollection GetSpriteImageCollection(string resourcePath) {
			if(SpriteCache.ContainsKey(resourcePath))
				return SpriteCache[resourcePath];
			var image = CreateBitmapFromResources(resourcePath);
			if(image == null)
				return null;
			var iconSize = new Size(16, 16);
			var imageWidth = image.Width;
			SpriteCache[resourcePath] = new ImageCollection();
			for(var hIndex = 0; hIndex < image.Height; hIndex += 16) {
				var imageRow = image.Clone(new Rectangle(0, hIndex, imageWidth, 16), image.PixelFormat);
				SpriteCache[resourcePath].Images.AddRange(ImageHelper.CreateImageCollectionCore(imageRow, iconSize, Color.Transparent).Images);
			}
			return SpriteCache[resourcePath];
		}
		public Bitmap CreateBitmapFromResources(string resourceName) {
			var stream = GetType().Assembly.GetManifestResourceStream(resourceName);
			return stream != null ? (Bitmap)Bitmap.FromStream(stream) : null;
		}
		void SpinEditItemLeave(object sender, EventArgs e) {
			var spinEdit = (SpinEdit)sender;
			var descriptorItem = (DesignEditorDescriptorItem)spinEdit.Tag;
			descriptorItem.Value = spinEdit.Value;
			HandleMenuItemAction(descriptorItem);
		}
		DXMenuItem CreateMenuItem(DesignEditorDescriptorItem descriptorItem) {
			return descriptorItem.HasChildItems ? CreateMenuSubItem(descriptorItem) : CreateMenuSimpleItem(descriptorItem);
		}
		DXSubMenuItem CreateMenuSubItem(DesignEditorDescriptorItem descriptorItem) {
			var menuSubItem = new DXSubMenuItem();
			CustomizeMenuItem(menuSubItem, descriptorItem);
			descriptorItem.ChildItems.ForEach(c => menuSubItem.Items.Add(CreateMenuItem(c)));
			return menuSubItem;
		}
		DXMenuItem CreateMenuSimpleItem(DesignEditorDescriptorItem descriptorItem) {
			var dxMenuItem = new DXMenuItem();
			CustomizeMenuItem(dxMenuItem, descriptorItem);
			dxMenuItem.Click += BarManagerItemClick;
			return dxMenuItem;
		}
		void CustomizeMenuItem(DXMenuItem dxMenuItem, DesignEditorDescriptorItem descriptorItem) {
			dxMenuItem.Caption = descriptorItem.Caption;
			dxMenuItem.Tag = descriptorItem;
			dxMenuItem.Image = GetImageByIndex(descriptorItem.ImageIndex);
			dxMenuItem.Enabled = descriptorItem.Enabled;
			dxMenuItem.BeginGroup = descriptorItem.BeginGroup;
		}
		Image GetImageByIndex(int index) {
			if(index < 0 || index >= IconCollection.Images.Count)
				return null;
			return IconCollection.Images[index];
		}
		void CreateItemsTreeList() {
			TreeListItems = new ItemsDesignerTreeList(ItemsOwner);
			TreeListItems.Parent = LeftPanel;
			TreeListItems.SetRowFont(Font);
			TreeListItems.FocusedNode = TreeListItems.FindNodeByKeyID(ItemsOwner.FocusedNodeID);
			TreeListItems.OptionsView.ShowColumns = false;
			TreeListItems.OptionsView.ShowIndicator = false;
			TreeListItems.OptionsView.ShowHorzLines = false;
			TreeListItems.OptionsView.ShowVertLines = false;
			TreeListItems.MenuManager = BarManager;
			TreeListItems.TabIndex = 0;
			TreeListItems.Dock = DockStyle.Fill;
			TreeListItems.StateImageList = IconCollection;
			TreeListItems.ExpandAll();
			TreeListItems.SelectionChanged += treeList_SelectionChanged;
			TreeListItems.KeyDown += treeList_KeyDown;
			TreeListItems.ItemsContextMenu.BeforePopup += TreeListContextMenu_BeforePopup;
		}
		Size FormSize { get; set; }
		int LeftPanelMinimizeWidth = 240;
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(PropertiesTabControl != null) {
				if(PropertiesTabControl.Size.Width <= LeftPanelMinimizeWidth && FormSize.Width > Size.Width)
					TreeListItems.Size = new Size(PropertiesTabControl.Size.Width - FormSize.Width + Size.Width, PropertiesTabControl.Size.Height);
			}
			FormSize = Size;
		}
		protected void TreeListContextMenu_BeforePopup(object sender, EventArgs e) {
			var menu = sender as DXPopupMenu;
			if(menu == null)
				return;
			menu.Items.Clear();
			ItemsOwner.GetEditorDescriptorItems(false).ForEach(i => menu.Items.Add(CreateMenuItem(i)));
		}
		void treeList_KeyDown(object sender, KeyEventArgs e) {
			var processed = false;
			switch(e.KeyCode) {
				case Keys.Delete:
					if(ShowTopPanel) {
						MenuItemClick_Remove();
						processed = true;
					}
					break;
				case Keys.Insert:
					if(ShowTopPanel) {
						ItemsOwner.InsertItem(null);
						processed = true;
					}
					break;
				case Keys.Up:
					if(e.Control) {
						MenuItemClick_MoveItem(false);
						processed = true;
					}
					break;
				case Keys.Down:
					if(e.Control) {
						MenuItemClick_MoveItem(true);
						processed = true;
					}
					break;
				case Keys.Left:
				case Keys.Right:
					if(e.Shift) {
						MenuItemClick_ChangeSelectionIndent(e.KeyCode == Keys.Right);
						processed = true;
					}
					break;
				case Keys.A:
					if(e.Control) {
						MenuItemClick_SelectAll();
						processed = true;
					}
					break;
				case Keys.Apps:
					TreeListItems.ShowContextMenu(true);
					processed = true;
					break;
			}
			e.Handled = processed;
		}
		public override void StoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.StoreLocalProperties(localStore);
			if(!IsHandleCreated || RetrieveFieldsPopup == null) return;
			localStore.AddProperty(GetPropertyPath("RetrieveFieldsPopup_Width"), RetrieveFieldsPopup.Width);
			localStore.AddProperty(GetPropertyPath("RetrieveFieldsPopup_Height"), RetrieveFieldsPopup.Height);
		}
		public override void RestoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			RetrieveFieldsSize = new Size(localStore.RestoreIntProperty(GetPropertyPath("RetrieveFieldsPopup_Width"), 0),
										  localStore.RestoreIntProperty(GetPropertyPath("RetrieveFieldsPopup_Height"), 0));
		}
		protected virtual void FillToolbarItems() {
			if(ToolbarPanel == null)
				return;
			var descriptorItems = ItemsOwner.GetEditorDescriptorItems(true);
			foreach(var control in ToolbarPanel.Controls) {
				if(!(control is DevExpress.XtraEditors.DropDownButton))
					continue;
				var dropDownButton = (DevExpress.XtraEditors.DropDownButton)control;
				var menuToolbarItem = dropDownButton.Tag as DesignEditorDescriptorItem;
				var descriptorItem = descriptorItems.First(i => i.ActionType == menuToolbarItem.ActionType);
				ToolBarGenerator.UpdateDropDownButtonState(dropDownButton, descriptorItem);
				if(ItemsOwner.CanCreatePopupMenuForDesignEditorDescriptorItem(descriptorItem))
					CreateBarItemPopupMenu(dropDownButton, descriptorItem.ChildItems);
			}
		}
		void CreateBarItemPopupMenu(DevExpress.XtraEditors.DropDownButton dropDown, List<DesignEditorDescriptorItem> items) {
			if(items.Count == 0 || dropDown == null)
				return;
			var popupMenu = dropDown.DropDownControl as DXPopupMenu;
			if(popupMenu == null) {
				popupMenu = new DXPopupMenu();
				dropDown.DropDownControl = popupMenu;
			}
			popupMenu.Items.Clear();
			items.ForEach(i => popupMenu.Items.Add(CreateMenuItem(i)));
		}
		void CreateBarItemPopupMenu(DXSubMenuItem dxMenuItem, List<DesignEditorDescriptorItem> items) {
			if(items.Count == 0 || dxMenuItem == null)
				return;
			dxMenuItem.Items.Clear();
			foreach(var item in items)
				dxMenuItem.Items.Add(CreateMenuItem(item));
		}
		protected virtual void treeList_SelectionChanged(object sender, EventArgs e) {
			SelectionChanged();
		}
		protected internal void SelectionChanged() {
			UpdatePropertiesObjects();
			FillToolbarItems();
		}
		void ToolBarButtonClick(object sender, EventArgs e) {
			var dropDownButton = sender as DevExpress.XtraEditors.DropDownButton;
			if(dropDownButton == null) 
				return;
			var descriptorItem = dropDownButton.Tag as DesignEditorDescriptorItem;
			if(descriptorItem == null)
				return;
			HandleMenuItemAction(descriptorItem);
		}
		void BarManagerItemClick(object sender, EventArgs e) {
			var dxMenuItem = sender as DXMenuItem;
			if(dxMenuItem == null)
				return;
			HandleMenuItemAction(dxMenuItem.Tag as DesignEditorDescriptorItem);
		}
		void HandleMenuItemAction(DesignEditorDescriptorItem descriptorItem) {
			if(descriptorItem == null)
				return;
			var rootItem = descriptorItem.ParentItem ?? descriptorItem;
			if(descriptorItem.EditorType == DesignEditorDescriptorItemType.DropDown)
				return;
			ProcessMenuItemAction(rootItem, descriptorItem);
		}
		protected virtual void ProcessMenuItemAction(DesignEditorDescriptorItem rootItem, DesignEditorDescriptorItem descriptorItem) {
			switch(rootItem.ActionType) {
				case DesignEditorMenuRootItemActionType.AddItem:
					MenuItemClick_AddItem(descriptorItem);
					break;
				case DesignEditorMenuRootItemActionType.InsertBefore:
					MenuItemClick_InsertBefore(descriptorItem);
					break;
				case DesignEditorMenuRootItemActionType.InsertChild:
					MenuItemClick_InsertChild(descriptorItem);
					break;
				case DesignEditorMenuRootItemActionType.Remove:
					MenuItemClick_Remove();
					break;
				case DesignEditorMenuRootItemActionType.RemoveInnerItems:
					MenuItemClick_RemoveInnerItems();
					break;
				case DesignEditorMenuRootItemActionType.MoveUp:
					MenuItemClick_MoveItem(false);
					break;
				case DesignEditorMenuRootItemActionType.MoveDown:
					MenuItemClick_MoveItem(true);
					break;
				case DesignEditorMenuRootItemActionType.MoveRight:
					MenuItemClick_ChangeSelectionIndent(true);
					break;
				case DesignEditorMenuRootItemActionType.MoveLeft:
					MenuItemClick_ChangeSelectionIndent(false);
					break;
				case DesignEditorMenuRootItemActionType.ChangeTo:
					if(descriptorItem != rootItem)
						MenuItemClick_ChangeTo(descriptorItem);
					break;
				case DesignEditorMenuRootItemActionType.RetriveFields:
					MenuItemClick_RetriveFields();
					break;
				case DesignEditorMenuRootItemActionType.SelectAll:
					MenuItemClick_SelectAll();
					break;
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
					MenuItemClick_CreateDefaultItems(descriptorItem);
					break;
				case DesignEditorMenuRootItemActionType.SetItemsAmount:
					ItemsOwner.SetItemsAmount(Convert.ToInt32(descriptorItem.Value));
					break;
			}
		}
		protected virtual void MenuItemClick_AddItem(DesignEditorDescriptorItem descriptorItem) {
			ItemsOwner.AddItem(descriptorItem.ItemType);
		}
		protected virtual void MenuItemClick_InsertBefore(DesignEditorDescriptorItem descriptorItem) {
			ItemsOwner.InsertItem(descriptorItem.ItemType);
		}
		protected virtual void MenuItemClick_InsertChild(DesignEditorDescriptorItem descriptorItem) {
			ItemsOwner.AddChildItem(descriptorItem.ItemType);
		}
		protected virtual void MenuItemClick_Remove() {
			if(ShowConfirmationToChangeHierarchy(ActionRemoveConfirmMessage) == DialogResult.OK)
				ItemsOwner.RemoveSelectedItems();
		}
		protected virtual void MenuItemClick_RemoveInnerItems() {
			ItemsOwner.RemoveInnerSelectedItems();
		}
		protected virtual void MenuItemClick_MoveItem(bool down) {
			ItemsOwner.MoveSelectedItems(down);
		}
		protected virtual void MenuItemClick_ChangeSelectionIndent(bool increase) { 
			ItemsOwner.ChangeSelectionIndent(increase);
		}
		protected virtual void MenuItemClick_ChangeTo(DesignEditorDescriptorItem descriptorItem) {
			if(ShowConfirmationToChangeHierarchy(ActionChangeToConfirmMessage) == DialogResult.OK)
				ItemsOwner.ChangeSelectedItemsTo(descriptorItem.ItemType);
		}
		DialogResult ShowConfirmationToChangeHierarchy(string message) {
			if(ItemsOwner.NeedConfirmRemove)
				return ShowConfirmation(message);
			return DialogResult.OK;
		}
		DialogResult ShowConfirmation(string message) {
			if(!NeedConfirmation)
				return DialogResult.OK;
			var result = ConfirmMessageBox.Show(message, ItemsOwner.ServiceProvider);
			NeedConfirmation = result.NeedConfirmation;
			return result.Dialogresult;
		}
		protected virtual void MenuItemClick_RetriveFields() { }
		protected virtual void MenuItemClick_SelectAll() {
			TreeListItems.SelectAll();
		}
		protected virtual void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem descriptorItem) {
			var result = ItemsOwner.Items.Count == 0 ? DialogResult.No
				: DesignTimeFormHelper.ShowMessage(ItemsOwner.ServiceProvider, ItemsOwner.CreateDefaultItemsConfirmMessage, ItemsOwner.CreateDefaultItemsConfirmMessage, MessageBoxButtons.YesNoCancel);
			if(result != DialogResult.Cancel)
				ItemsOwner.CreateDefaultItems(result == DialogResult.Yes);
		}
		void OnEditingComponentChanged(ComponentChangedEventArgs e) {
			var propertyDescriptor = e.Member as PropertyDescriptor;
			if(propertyDescriptor == null)
				return;
			var focusedItem = ItemsOwner.FocusedItem;
			if(focusedItem == null)
				return;
			if(!ValidatePropertyGridDescriptor(propertyDescriptor, focusedItem.GetType()))
				return;
			var list = ItemsOwner.GetViewDependedProperties();
			var propertyName = e.Member.Name;
			if(!list.Contains(propertyName))
				return;
			SetPropertyGridsEnabled(false);
			ItemsOwner.SelectedItemsPropertyChanged(propertyName);
			ItemsOwner.RecreateTreeListItems();
			SetPropertyGridsEnabled(true);
		}
		bool ValidatePropertyGridDescriptor(PropertyDescriptor propertyDescriptor, Type validateType) {
			if(propertyDescriptor.ComponentType.IsAssignableFrom(validateType))
				return true;
			var filteredDescriptor = propertyDescriptor as FilterObjectPropertyDescriptor;
			if(filteredDescriptor == null)
				return false;
			return GridViewFieldConverterHelper.GetObjectByObjectWrapperHierarchy(filteredDescriptor.SourceObject as IDXObjectWrapper, validateType) != null;
		}
		void SetPropertyGridsEnabled(bool enabled) {
			if(ItemPropertyGrid != null)
				ItemPropertyGrid.Enabled = enabled;
			if(EditorPropertyGrid != null)
				EditorPropertyGrid.Enabled = enabled;
		}
		void UpdateLabelForEmptyPropertiesLocation(Control propertyGrid) {
			var labelInfo = InfoLabels[propertyGrid.Parent.Name];
			var innerPropertyGrid = propertyGrid.Tag as Control;
			var x = innerPropertyGrid.Left + (innerPropertyGrid.Width - labelInfo.Width) / 2;
			var y = innerPropertyGrid.Top + (innerPropertyGrid.Height - labelInfo.Height) / 2;
			labelInfo.Left = x >= 0 ? x : 0;
			labelInfo.Top = y >= 0 ? y : 0;
		}
		Control GetPropertyGrid(Control container) { 
			return container.Controls[PropretyGridName];
		}
		object[] GetSelectedItemProperties() {
			var result = new ArrayList();
			var selectedItems = GetSelectedDesignTimeItems();
			var visibleProperties = ItemsOwner.GetVisibleProperties();
			foreach(var item in selectedItems) {
				var hiddenProperties = item.GetHiddenPropertyNames();
				if(hiddenProperties.Length > 0)
					result.Add(new FilterObject(item, hiddenProperties, FilterObjectFilterPropertiesType.Exclude));
				else
					result.Add(item);
				if(visibleProperties.Length != 0)
					result.Add(new FilterObject(item, visibleProperties, FilterObjectFilterPropertiesType.Include));
			}
			return result.ToArray();
		}
		HashSet<IDesignTimeCollectionItem> GetSelectedDesignTimeItems() {
			var result = new HashSet<IDesignTimeCollectionItem>();
			var treeListSelection = TreeListItems.Selection.OfType<TreeListNode>();
			foreach(var node in treeListSelection) {
				var dataItem = TreeListItems.GetDataItem(node);
				if(dataItem != null)
					result.Add(ItemsOwner.TreeListItemsHash[dataItem.ID]);
			}
			return result;
		}
		object[] GetSelectedEditorProperties() {
			return TreeListItems.Selection.OfType<TreeListNode>().
				Select(n => ItemsOwner.TreeListItemsHash[TreeListItems.GetDataItem(n).ID]).OfType<IDesignTimeCollectionItem>().
				Select(c => c.EditorProperties).
				Where(p => p != null).ToArray();
		}
		void UpdatePropertiesObjects() {
			if(EditorPropertiesTab == null)
				return;
			try {
				PropertiesTabControl.SuspendLayout();
				ItemPropertyGrid.SelectedObjects = GetSelectedItemProperties();
				EditorPropertyGrid.SelectedObjects = GetSelectedEditorProperties();
			} catch {
			} finally {
				var editorName = ItemsOwner.GetEditorTabName();
				EditorPropertiesTab.Text = !string.IsNullOrEmpty(editorName) ? string.Format("{0} Properties", editorName) : string.Empty;
				EditorPropertiesTab.PageVisible = !string.IsNullOrEmpty(EditorPropertiesTab.Text);
				ExpandItemPropertyGridProperties();
				PropertiesTabControl.ResumeLayout();
			}
		}
		void ExpandItemPropertyGridProperties() {
			if(EmbeddedFrameInitObject == null)
				return;
			if(EmbeddedFrameInitObject.ExpandAllProperties) {
				ItemPropertyGrid.ExpandAllGridItems();
			} else if(EmbeddedFrameInitObject.ExpandedPropertiesOnStart != null) {
				foreach(var property in EmbeddedFrameInitObject.ExpandedPropertiesOnStart)
					ItemPropertyGrid.ExpandProperty(property);
			}
		}
		void SetupRetrieveColumnsControl() {
			if(DataOwner != null) {
				RetrieveFieldsCheckedListBox.ItemCheck += CheckItemListBoxRetrieveFields;
				FillRetireveFieldsCheckedList();
			}
		}
		void FillRetireveFieldsCheckedList() {
			if(RetrieveFieldsCheckedListBox != null) {
				RetrieveFieldsCheckedListBox.Items.Clear();
				foreach(DesignTimeFieldInfo fieldInfo in DataOwner.FieldInfoList)
					RetrieveFieldsCheckedListBox.Items.Add(fieldInfo.Name, fieldInfo.IsBindableType);
			}
		}
		void CheckItemListBoxRetrieveFields(object sender, XtraEditors.Controls.ItemCheckEventArgs e) {
			CheckAll.CheckedChanged -= RetrieveFieldsCheckAllChanged;
			CheckAll.Checked = RetrieveFieldsCheckedListBox.CheckedItemsCount == RetrieveFieldsCheckedListBox.ItemCount;
			CheckAll.CheckedChanged += RetrieveFieldsCheckAllChanged;
		}
		void RetrieveFieldsCheckAllChanged(object sender, EventArgs e) {
			if(CheckAll.Checked)
				RetrieveFieldsCheckedListBox.CheckAll();
			else
				RetrieveFieldsCheckedListBox.UnCheckAll();
		}
		void clbFields_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			if(DisplayFields.IndexOf(e.Item.ToString()) >= 0)
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
		}
		protected void PrepareFieldsPopup(object sender, EventArgs e) {
			ManagerWaitingForm.ShowWaitForm();
			BeforeShowRetrieveFieldsPopup();
			ManagerWaitingForm.CloseWaitForm();
		}
		protected virtual void BeforeShowRetrieveFieldsPopup() {
			DisplayFields.Clear();
			DisplayFields.AddRange(DataOwner.GetUsedFieldNames());
			RetrieveFieldsCheckedListBox.UnCheckAll();
			foreach(string fieldName in DisplayFields) {
				int index = RetrieveFieldsCheckedListBox.Items.IndexOf(fieldName);
				if(index > -1)
					RetrieveFieldsCheckedListBox.Items[index].CheckState = System.Windows.Forms.CheckState.Checked;
			}
			CheckItemListBoxRetrieveFields(null, null);
		}
		void btnRetrieveFieldsContainerOK_Click(object sender, EventArgs e) {
			RetrieveFieldsPopup.HidePopup();
			UpdateItemsFromFieldsSelector();
		}
		void btnRetrieveFieldsContainerCancel_Click(object sender, EventArgs e) {
			RetrieveFieldsPopup.HidePopup();
		}
		protected virtual void UpdateItemsFromFieldsSelector() {
			var itemValues = RetrieveFieldsCheckedListBox.Items.OfType<CheckedListBoxItem>().Select(c => c.ToString());
			var checkedItemValues = RetrieveFieldsCheckedListBox.Items.GetCheckedValues();
			var uncheckedItems = itemValues.Except(checkedItemValues);
			bool isRemove = false;
			if(DisplayFields.Intersect(uncheckedItems).Count() != 0)
				isRemove = ShowConfirmation(ActionConfirmRemoveUncheckedItems) == DialogResult.OK;
			var itemsToAdd = new List<string>();
			foreach(CheckedListBoxItem item in RetrieveFieldsCheckedListBox.Items) {
				var fieldName = item.ToString();
				if(item.CheckState == System.Windows.Forms.CheckState.Checked) {
					if(DisplayFields.IndexOf(fieldName) < 0)
						itemsToAdd.Add(fieldName);
				} else if(isRemove) {
					DataOwner.RemoveDataItem(fieldName);
				}
			}
			if(itemsToAdd.Count != 0)
				DataOwner.CreateDataItems(itemsToAdd);
		}
		Control FindInnerPropertyGrid(Control propertyGrid) { 
			foreach(Control innerControl in propertyGrid.Controls)
				if(innerControl.GetType().Name == "PropertyGridView")
					return innerControl;
			return null;
		}
		protected override void OnRefreshPropertyGrid() { 
			UpdatePropertiesObjects();
		}
		protected override void OnShowPropertyGridToolBar(bool show) { 
			if(ItemPropertyGrid != null && ItemPropertyGrid.Visible)
				ItemPropertyGrid.ToolbarVisible = show;
			if(EditorPropertyGrid != null && EditorPropertyGrid.Visible)
				EditorPropertyGrid.ToolbarVisible = show;
		}
	}
	public class ToolbarItemsGenerator {
		const int ButtonMargin = 3;
		const int SeparatorMargin = 7;
		protected Size DefaultButtonSize = new Size(28, 30);
		protected Size DefaultDropDownSize = new Size(44, 30);
		protected Size DefaultSpinEditSize = new Size(50, 26);
		Dictionary<DesignEditorMenuRootItemActionType, PopupControlContainer> actionPopups;
		Dictionary<DesignEditorMenuRootItemActionType, EventHandler> actionHandlers;
		int nextToolBarItemleft;
		IDXMenuManager menuManager;
		public ToolbarItemsGenerator(ItemsEditorOwner itemsOwner, PanelControl toolbarPanel, ImageCollection iconCollection, EventHandler globalActionHandler) {
			ItemsOwner = itemsOwner;
			ToolbarPanel = toolbarPanel;
			IconCollection = iconCollection;
			GlobalActionHandler = globalActionHandler;
		}
		EventHandler GlobalActionHandler { get; set; }
		ImageCollection IconCollection { get; set; }
		ItemsEditorOwner ItemsOwner { get; set; }
		PanelControl ToolbarPanel { get; set; }
		Dictionary<DesignEditorMenuRootItemActionType, PopupControlContainer> ActionPopups {
			get {
				if(actionPopups == null)
					actionPopups = new Dictionary<DesignEditorMenuRootItemActionType, PopupControlContainer>();
				return actionPopups;
			}
		}
		Dictionary<DesignEditorMenuRootItemActionType, EventHandler> ActionHandlers {
			get {
				if(actionHandlers == null)
					actionHandlers = new Dictionary<DesignEditorMenuRootItemActionType, EventHandler>();
				return actionHandlers;
			}
		}
		IDXMenuManager MenuManager {
			get {
				if(menuManager == null)
					menuManager = MenuManagerHelper.GetMenuManager(ToolbarPanel.LookAndFeel, ToolbarPanel);
				return menuManager;
			}
		}
		public void AddActionPopup(DesignEditorMenuRootItemActionType actionType, PopupControlContainer popupControl) {
			ActionPopups[actionType] = popupControl;
		}
		public void AddActionHandler(DesignEditorMenuRootItemActionType actionType, EventHandler handler) {
			ActionHandlers[actionType] = handler;
		}
		public void GenerateToolBarLinks() {
			nextToolBarItemleft = 0;
			foreach(var item in ItemsOwner.GetEditorDescriptorItems(true)) {
				switch(item.EditorType) {
					case DesignEditorDescriptorItemType.Button:
					case DesignEditorDescriptorItemType.DropDown:
					case DesignEditorDescriptorItemType.DropDownButton:
						CreateDropDown(ToolbarPanel, item);
						break;
					case DesignEditorDescriptorItemType.SpinEdit:
						CreateSpinEditItem(ToolbarPanel, item);
						break;
				}
			}
		}
		Control CreateSpinEditItem(Control container, DesignEditorDescriptorItem descriptorItem) {
			var spinEditItem = FindToolbarItemByActionType<SpinEdit>(container, descriptorItem.ActionType);
			if(spinEditItem == null) {
				CreateSeparator(container, descriptorItem);
				spinEditItem = new SpinEdit();
				spinEditItem.Name = descriptorItem.Caption;
				CreateLabel(container, descriptorItem);
				spinEditItem.Parent = container;
			}
			spinEditItem.Size = DefaultSpinEditSize;
			spinEditItem.ToolTip = descriptorItem.Caption;
			spinEditItem.Properties.MinValue = Convert.ToInt32(descriptorItem.Parameters[0]);
			spinEditItem.Properties.MaxValue = Convert.ToInt32(descriptorItem.Parameters[1]);
			spinEditItem.Value = Convert.ToDecimal(descriptorItem.Value);
			if(ActionHandlers.ContainsKey(descriptorItem.ActionType))
				spinEditItem.Leave += ActionHandlers[descriptorItem.ActionType];
			spinEditItem.Tag = descriptorItem;
			SetToolBarItemDimentions(spinEditItem);
			return spinEditItem;
		}
		Control CreateLabel(Control container, DesignEditorDescriptorItem descriptorItem) {
			var label = new LabelControl() {
				Name = string.Format("label_{0}", descriptorItem.ActionType),
				Text = descriptorItem.Caption
			};
			label.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			label.Parent = container;
			SetToolBarItemDimentions(label);
			return label;
		}
		DevExpress.XtraEditors.DropDownButton CreateDropDown(Control container, DesignEditorDescriptorItem descriptorItem) {
			var dropDown = FindToolbarItemByActionType<DevExpress.XtraEditors.DropDownButton>(container, descriptorItem.ActionType);
			if(dropDown == null) {
				CreateSeparator(container, descriptorItem);
				dropDown = new DevExpress.XtraEditors.DropDownButton();
				dropDown.Parent = container;
				dropDown.AllowFocus = false;
				dropDown.ImageList = IconCollection;
				dropDown.ImageLocation = ImageLocation.MiddleLeft;
			}
			CustomizeDropDown(dropDown, descriptorItem);
			return dropDown;
		}
		void CustomizeDropDown(DevExpress.XtraEditors.DropDownButton dropDown, DesignEditorDescriptorItem descriptorItem) {
			if(ActionPopups.ContainsKey(descriptorItem.ActionType))
				dropDown.DropDownControl = ActionPopups[descriptorItem.ActionType];
			dropDown.Name = string.Format("{0}_{1}", dropDown.GetType().Name, descriptorItem.ActionType);
			dropDown.ImageIndex = descriptorItem.ImageIndex;
			dropDown.Text = string.Empty;
			dropDown.Margin = new Padding(ButtonMargin, 0, ButtonMargin, 0);
			dropDown.Tag = descriptorItem;
			UpdateDropDownButtonState(dropDown, descriptorItem);
			SetToolBarItemDimentions(dropDown);
		}
		public void UpdateDropDownButtonState(DevExpress.XtraEditors.DropDownButton dropDownButton, DesignEditorDescriptorItem descriptorItem) {
			switch(descriptorItem.EditorType) {
				case DesignEditorDescriptorItemType.Button:
					dropDownButton.DropDownArrowStyle = DropDownArrowStyle.Hide;
					dropDownButton.Size = DefaultButtonSize;
					AddActionEventHandler(dropDownButton, descriptorItem.ActionType);
					break;
				case DesignEditorDescriptorItemType.DropDown:
					dropDownButton.DropDownArrowStyle = DropDownArrowStyle.Show;
					dropDownButton.Size = DefaultDropDownSize;
					RemoveActionEventHandler(dropDownButton, descriptorItem.ActionType);
					break;
				case DesignEditorDescriptorItemType.DropDownButton:
					dropDownButton.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
					dropDownButton.Size = DefaultDropDownSize;
					AddActionEventHandler(dropDownButton, descriptorItem.ActionType);
					break;
			}
			dropDownButton.ActAsDropDown = dropDownButton.DropDownArrowStyle != DropDownArrowStyle.Hide;
			dropDownButton.Enabled = descriptorItem.Enabled;
			dropDownButton.ToolTip = descriptorItem.Caption;
			if(dropDownButton.ImageIndex == -1) {
				dropDownButton.Text = descriptorItem.Caption;
				dropDownButton.Width = DesignTimeFormHelper.GetTextWidth(dropDownButton, dropDownButton.Text, dropDownButton.Font) + 8;
			}
			if(dropDownButton.MenuManager != MenuManager)
				dropDownButton.MenuManager = MenuManager;
		}
		void AddActionEventHandler(DevExpress.XtraEditors.DropDownButton dropDownButton, DesignEditorMenuRootItemActionType actionType) {
			if(!ActionHandlers.ContainsKey(actionType)) {
				dropDownButton.Click -= GlobalActionHandler;
				dropDownButton.Click += GlobalActionHandler;
			}
		}
		void RemoveActionEventHandler(DevExpress.XtraEditors.DropDownButton dropDownButton, DesignEditorMenuRootItemActionType actionType) {
			var handler = ActionHandlers.ContainsKey(actionType) ? ActionHandlers[actionType] : GlobalActionHandler;
			dropDownButton.Click -= handler;
		}
		void CreateSeparator(Control container, DesignEditorDescriptorItem descriptorItem) {
			if(!descriptorItem.BeginGroup)
				return;
			var separator = new LabelControl();
			separator.AutoSizeMode = LabelAutoSizeMode.None;
			separator.BorderStyle = BorderStyles.NoBorder;
			separator.LineLocation = LineLocation.Left;
			separator.LineOrientation = LabelLineOrientation.Vertical;
			separator.LineVisible = true;
			separator.Margin = new Padding(SeparatorMargin, 0, SeparatorMargin, 0);
			separator.Location = new Point(nextToolBarItemleft + separator.Margin.Left, 0);
			separator.Name = string.Format("vertSeparator_{0}", descriptorItem.ActionType);
			separator.Size = new Size(3, 28);
			separator.Parent = container;
			SetToolBarItemDimentions(separator);
		}
		void SetToolBarItemDimentions(Control toolBarItem) {
			toolBarItem.Left = nextToolBarItemleft != 0 ? nextToolBarItemleft + toolBarItem.Margin.Left : 0;
			nextToolBarItemleft = toolBarItem.Left;
			toolBarItem.Top = ToolbarPanel.Height / 2 - toolBarItem.Height / 2;
			nextToolBarItemleft += toolBarItem.Width + toolBarItem.Margin.Right;
		}
		T FindToolbarItemByActionType<T>(Control container, DesignEditorMenuRootItemActionType actionType) where T : Control {
			foreach(Control control in container.Controls) {
				var result = control as T;
				if(result != null) {
					var descriptorItem = control.Tag as DesignEditorDescriptorItem;
					if(descriptorItem != null && descriptorItem.ActionType == actionType)
						return result;
				}
			}
			return null;
		}
	}
	interface IUpdatableViewControl { 
		void UpdateView();
	}
}
