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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Web.Internal;
using DevExpress.Web.Design.Forms;
namespace DevExpress.Web.Design {
	public abstract class ItemsEditorFormBase : TwoSidesFormBase {
		public const string AddItemImageResource = "DevExpress.Web.Design.Images.AddItem.png";
		public const string InsertItemImageResource = "DevExpress.Web.Design.Images.InsertItem.png";
		public const string RemoveItemImageResource = "DevExpress.Web.Design.Images.RemoveItem.png";
		public const string MoveDownItemImageResource = "DevExpress.Web.Design.Images.MoveDown.png";
		public const string MoveUpItemImageResource = "DevExpress.Web.Design.Images.MoveUp.png";
		public const string RemoveAllItemImageResource = "DevExpress.Web.Design.Images.RemoveAllItem.png";
		private static Keys[] ShortCutKeys = new Keys[] { ((Keys)(Keys.Control | Keys.A)), (Keys)(Keys.Delete) };
		private PropertyGrid fPropertyGrid;
		private ContextMenuStrip fPopupMenu = null;
		private ToolStrip fToolStrip;
		private Collection fUndoBuffer = null;
		protected Panel fItemViewerPanel = null;
		protected Collection Collection {
			get { return PropertyValue as Collection; }
		}
		protected Panel ItemViewerPanel {
			get { return fItemViewerPanel; }
		}
		protected ContextMenuStrip PopupMenu {
			get {
				if(fPopupMenu == null)
					fPopupMenu = CreatePopupMenu();
				return fPopupMenu;
			}
		}
		protected PropertyGrid PropertyGrid {
			get { return fPropertyGrid; }
		}
		protected ToolStrip ToolStrip {
			get { return fToolStrip; }
		}
		protected Collection UndoBuffer {
			get {
				if(fUndoBuffer == null)
					fUndoBuffer = new Collection();
				return fUndoBuffer;
			}
		}
		public ItemsEditorFormBase(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected virtual void FocusItemViewer() { }
		protected virtual void PropertyValueChanged(PropertyValueChangedEventArgs e) { }
		protected override void InitializeForm() {
			base.InitializeForm();
		}
		protected override void AddControlsToRightPanel(Panel rightPanel) {
			AddPropertyGrid(rightPanel);
			AddObjectNameLabel(rightPanel);
		}
		protected override void AssignControls() {
			base.AssignControls();
			UpdateTools();
			FocusItemViewer();
		}
		protected override void SaveUndoData() {
			if(Collection != null)
				UndoBuffer.Assign(Collection);
		}
		protected override void Undo() {
			if(Collection != null)
				Collection.Assign(UndoBuffer);
			base.Undo();
		}
		protected override void SetTabIndexes() {
			base.SetTabIndexes();
			fPropertyGrid.TabStop = true;
			fPropertyGrid.TabIndex = TabOrder[2];
			fToolStrip.TabStop = true;
			fToolStrip.TabIndex = TabOrder[1];
		}
		protected virtual void AddNewItem() {
			object item = CreateNewItem();
			OnAddNewItem(item);
			ComponentChanged(false, true);
		}
		protected virtual void InsertNewItem() {
			object item = CreateNewItem();
			OnInsertItem(item);
			ComponentChanged(false, true);
		}
		protected abstract object CreateNewItem();
		protected virtual void OnAddNewItem(object item) { }
		protected virtual void OnInsertItem(object item) { }
		protected virtual bool IsInsertButtonEnable() {
			return Collection.Count != 0;
		}
		protected virtual bool IsRemoveButtonEnable() {
			return Collection.Count != 0;
		}
		protected virtual bool IsRemoveAllButtonEnable() {
			return Collection.Count != 0;
		}
		protected virtual System.Windows.Forms.Control CreateItemViewer() {
			return null;
		}
		protected virtual void RemoveItem() {
			ComponentChanged(false, true);
		}
		protected virtual void RemoveAllItems() {
			if(Collection != null)
				Collection.Clear();
			ComponentChanged(false, true);
		}
		protected virtual ToolStripItem CreateAddInsertItem(bool isInsert, string toolTipText, string image, EventHandler onClick) {
			List<CollectionItemType> items = GetCollectionItemTypes();
			if(items.Count < 2) return CreatePushButton(toolTipText, image, onClick);
			ToolStripSplitButton res = CreatePushButtonCore(new ToolStripSplitButton(), toolTipText, image, null) as ToolStripSplitButton;
			res.ButtonClick += onClick;
			for(int n = 0; n < items.Count; n++)
				CreateToolStripItem(res.DropDownItems, items[n], isInsert);
			return res;
		}
		void CreateToolStripItem(ToolStripItemCollection collection, CollectionItemType item, bool isInsert) {
			if(item.BeginGroup && collection.Count > 0)
				collection.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripItem = new ToolStripMenuItem(item.Text, null, 
				item.Items.Count == 0 ? new EventHandler(OnSplitCreateItemClick) : null);
			toolStripItem.Tag = item;
			toolStripItem.Name = isInsert ? "insert" : "add";
			foreach (CollectionItemType subItem in item.Items)
				CreateToolStripItem(toolStripItem.DropDownItems, subItem, false);
			collection.Add(toolStripItem);
		}
		protected virtual void OnSplitCreateItemClick(object sender, EventArgs e) {
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			CollectionItemType itemType = (CollectionItemType)menuItem.Tag;
			object item = Activator.CreateInstance(itemType.Type);
			OnItemCreated(item);
			if(menuItem.Name == "insert")
				OnInsertItem(item);
			else
				OnAddNewItem(item);
			ComponentChanged(false, true);
			UpdateTools();
		}
		protected virtual void OnItemCreated(object item) {
		}
		public class CollectionItemType {
			Type type;
			string text;
			List<CollectionItemType> items;
			bool beginGroup;
			public CollectionItemType(Type type) : this(type, type.ToString()) { }
			public CollectionItemType(Type type, string text) {
				this.text = text;
				this.type = type;
				this.items = new List<CollectionItemType>();
			}
			public CollectionItemType(Type type, string text, params CollectionItemType[] items)
				: this(type, text, false, items) {
			}
			public CollectionItemType(Type type, string text, bool beginGroup, params CollectionItemType[] items)
				: this(type, text) {
				this.beginGroup = beginGroup;
				Items.AddRange(items);
			}
			public Type Type { get { return type; } }
			public string Text { get { return text; } }
			public List<CollectionItemType> Items { get { return items; } }
			public bool BeginGroup { get { return beginGroup; } }
			void Sort(List<CollectionItemType> list) {
				list.Sort(delegate(CollectionItemType x, CollectionItemType y) {
					return Comparer<string>.Default.Compare(x.Text, y.Text);
				});
			}
			public CollectionItemType Sort() {
				List<CollectionItemType> sorted = new List<CollectionItemType>();
				List<CollectionItemType> group = new List<CollectionItemType>();
				for(int i = 0; i <= Items.Count; i++) {
					if(i == Items.Count || Items[i].BeginGroup) {
						Sort(group);
						foreach(CollectionItemType groupedItem in group)
							groupedItem.beginGroup = false;
						group[0].beginGroup = true;
						sorted.AddRange(group);
						group.Clear();
					}
					if(i < Items.Count)
						group.Add(Items[i]);
				}
				this.items = sorted;
				return this;
			}
		}
		protected virtual List<CollectionItemType> GetCollectionItemTypes() {
			return new List<CollectionItemType>();
		}
		protected virtual void FillItemsViewer() { }
		protected virtual void AddToolStripButtons(List<ToolStripItem> buttons) {
			buttons.AddRange(new ToolStripItem[] { 
				CreateAddInsertItem(false, StringResources.ItemsEditor_AddItemButtonText, AddItemImageResource, OnAddItemButtonClick), 
				CreateAddInsertItem(true, StringResources.ItemsEditor_InsertItemButtonText, InsertItemImageResource, OnInsertButtonClick),
				CreatePushButton(StringResources.ItemsEditor_RemoveItemButtonText, RemoveItemImageResource, OnRemoveItemButtonClick),
				CreateToolStripSeparator(),
				CreatePushButton(StringResources.ItemsEditor_MoveUpItemButtonText, MoveUpItemImageResource, OnMoveUpItemButtonClick),
				CreatePushButton(StringResources.ItemsEditor_MoveDownItemButtonText, MoveDownItemImageResource, OnMoveDownItemButtonClick), 
				CreateToolStripSeparator(),
				CreatePushButton(StringResources.ItemsEditor_RemoveAllItemsButtonText, RemoveAllItemImageResource, OnRemoveAllItemsButtonClick) });
		}
		protected virtual void AddMenuItems(List<ToolStripItem> buttons) {
			buttons.AddRange(new ToolStripItem[] { CreateMenuItem(StringResources.ItemsEditorPopupMenu_AddItemButtonText, AddItemImageResource, OnAddItemButtonClick, Keys.None), 
				CreateMenuItem(StringResources.ItemsEditorPopupMenu_InsertItemButtonText, InsertItemImageResource, OnInsertButtonClick, Keys.None),
				CreateMenuItem(StringResources.ItemsEditorPopupMenu_RemoveItemButtonText, RemoveItemImageResource, OnRemoveItemButtonClick, ShortCutKeys[1])});
			CreateCustomMenuItems(buttons);
			buttons.AddRange(new ToolStripItem[] { 
				CreateToolStripSeparator(),
				CreateMenuItem(StringResources.ItemsEditorPopupMenu_MoveUpItemButtonText, MoveUpItemImageResource, OnMoveUpItemButtonClick, Keys.None),
				CreateMenuItem(StringResources.ItemsEditorPopupMenu_MoveDownItemButtonText, MoveDownItemImageResource, OnMoveDownItemButtonClick, Keys.None)});
			ToolStripItem selectAllItem = CreateSelectAllMenuItem();
			if(selectAllItem != null) {
				buttons.Add(CreateToolStripSeparator());
				buttons.Add(selectAllItem);
			}
		}
		protected virtual ToolStripItem CreateSelectAllMenuItem() {
			return CreateMenuItem(StringResources.ItemsEditorPopupMenu_SelectAllItemButtonText, MoveDownItemImageResource, OnSelectAllButtonClick, ShortCutKeys[0]);
		}
		protected virtual void CreateCustomMenuItems(List<ToolStripItem> buttons) { }
		protected virtual void SetVisiblePropertyInViewer() { }
		protected virtual void MoveViewerItem(int oldIndex, int newIndex) { }
		protected virtual IList GetParentViewerItemCollection() { return null; }
		protected int GetViewerItemIndex(object item) { return GetParentViewerItemCollection().IndexOf(item); }
		protected virtual void MoveUpViewerItem() { }
		protected virtual void MoveDownViewerItem() { }
		protected virtual void MoveUpItem() {
			MoveUpViewerItem();
			ComponentChanged(false, true);
		}
		protected virtual void MoveDownItem() {
			MoveDownViewerItem();
			ComponentChanged(false, true);
		}
		protected virtual void SelectAll() { }
		protected virtual void UpdateToolStrip() {
			ToolStripItem item = FindToolItemByText(StringResources.ItemsEditor_RemoveItemButtonText);
			item.Enabled = IsRemoveButtonEnable();
			item = FindToolItemByText(StringResources.ItemsEditor_MoveDownItemButtonText);
			item.Enabled = IsMoveDownButtonEnabled();
			item = FindToolItemByText(StringResources.ItemsEditor_MoveUpItemButtonText);
			item.Enabled = IsMoveUpButtonEnabled();
			item = FindToolItemByText(StringResources.ItemsEditor_RemoveAllItemsButtonText);
			item.Enabled = IsRemoveAllButtonEnable();
			item = FindToolItemByText(StringResources.ItemsEditor_InsertItemButtonText);
			item.Enabled = IsInsertButtonEnable();
		}
		protected virtual void UpdateMenuStrip() {
			ToolStripItem item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_RemoveItemButtonText);
			item.Enabled = IsRemoveButtonEnable();
			item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_MoveDownItemButtonText);
			item.Enabled = IsMoveDownButtonEnabled();
			item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_MoveUpItemButtonText);
			item.Enabled = IsMoveUpButtonEnabled();
			item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_InsertItemButtonText);
			item.Enabled = IsInsertButtonEnable();
		}
		protected virtual string GetRemoveAllConfirmString() {
			return StringResources.ItemsEditor_RemoveAllConfirmDialogText;
		}
		protected abstract bool IsMoveDownButtonEnabled();
		protected abstract bool IsMoveUpButtonEnabled();
		protected override void AddControlsToLeftPanel(Panel panel) {
			fItemViewerPanel = new Panel();
			SetDockForItemViewerPanel(DockStyle.Fill);
			AddItemsViewer(fItemViewerPanel);
			AddToolStrip(fItemViewerPanel);
			AddItemsNameLabel(fItemViewerPanel);
			panel.Controls.Add(fItemViewerPanel);
			AddCustomControlsToLeftPanel(panel);
		}
		protected virtual void AddCustomControlsToLeftPanel(Panel panel) {
		}
		protected ToolStripItem CreatePushButtonCore(ToolStripItem button, string toolTipText, Image image, EventHandler onClick) {
			button.ToolTipText = toolTipText;
			button.Image = image;
			button.Click += onClick;
			button.Text = toolTipText;
			button.DisplayStyle = ToolStripItemDisplayStyle.Image;
			button.AutoToolTip = true;
			button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			return button;
		}
		protected ToolStripItem CreatePushButtonCore(ToolStripItem button, string toolTipText, string imageResource, EventHandler onClick) {
			Bitmap image = null;
			if(imageResource != "") {
				image = CreateBitmapFromResources(imageResource);
				image.MakeTransparent(Color.Magenta);
			}
			return CreatePushButtonCore(button, toolTipText, image, onClick);
		}
		protected ToolStripButton CreatePushButton(string toolTipText, Image image, EventHandler onClick) {
			return (ToolStripButton)CreatePushButtonCore(new ToolStripButton(), toolTipText, image, onClick);
		}
		protected ToolStripButton CreatePushButton(string toolTipText, string imageResource, EventHandler onClick) {
			return (ToolStripButton)CreatePushButtonCore(new ToolStripButton(), toolTipText, imageResource, onClick);
		}
		protected ToolStripItem CreateMenuItem(string text, string imageResource, EventHandler onClick, Keys shortCutKeys) {
			Bitmap image = null;
			if(imageResource != "") {
				image = CreateBitmapFromResources(imageResource);
				image.MakeTransparent(Color.Magenta);
			}
			return CreateMenuItem(text, image, onClick, shortCutKeys);
		}
		protected ToolStripMenuItem CreateMenuItem(string text, Image image, EventHandler onClick, Keys shortCutKeys) {
			ToolStripMenuItem button = null;
			button = new ToolStripMenuItem(text, image, onClick);
			button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			button.ShortcutKeys = shortCutKeys;
			return button;
		}
		protected ToolStripSeparator CreateToolStripSeparator() {
			return new ToolStripSeparator();
		}
		protected ToolStripItem FindToolItemByText(string text) {
			foreach(ToolStripItem item in fToolStrip.Items) {
				if(item.ToolTipText == text)
					return item;
			}
			return null;
		}
		protected ToolStripItem FindPopupMenuItemByText(string text) {
			foreach(ToolStripItem item in PopupMenu.Items) {
				if(item.Text == text)
					return item;
			}
			return null;
		}
		protected void SetDockForItemViewerPanel(DockStyle dockStyle) {
			fItemViewerPanel.Dock = dockStyle;
		}
		protected void UpdateTools() {
			UpdateToolStrip();
			UpdateMenuStrip();
		}
		private void AddItemsNameLabel(Panel panel) {
			Label label = ControlCreator.CreateLabel(EditingPropertyName + ":");
			label.Dock = DockStyle.Top;
			panel.Controls.Add(label);
		}
		private void AddItemsViewer(Panel panel) {
			System.Windows.Forms.Control itemViewer = CreateItemViewer();
			panel.Controls.Add(itemViewer);
		}
		private void AddPropertyGrid(Panel panel) {
			fPropertyGrid = new PropertyGrid();
			fPropertyGrid.TabStop = true;
			fPropertyGrid.Dock = DockStyle.Fill;
			fPropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(OnPropertyValueChanged);
			fPropertyGrid.Site = new FormPropertyGridSite(ServiceProvider, fPropertyGrid);
			panel.Controls.Add(fPropertyGrid);
		}
		private void AddToolStrip(Panel panel) {
			fToolStrip = new ToolStrip();
			fToolStrip.RenderMode = ToolStripRenderMode.System;
			fToolStrip.CanOverflow = false;
			fToolStrip.GripStyle = ToolStripGripStyle.Hidden;
			fToolStrip.AutoSize = false;
			fToolStrip.Dock = DockStyle.Top;
			fToolStrip.Margin = new Padding(0);
			fToolStrip.Items.AddRange(GetToolStripButtons());
			panel.Controls.Add(fToolStrip);
		}
		private ContextMenuStrip CreatePopupMenu() {
			ContextMenuStrip menu = new ContextMenuStrip();
			menu.Items.AddRange(GetPopupMenuItems());
			return menu;
		}
		private ToolStripItem[] GetToolStripButtons() {
			List<ToolStripItem> items = new List<ToolStripItem>();
			AddToolStripButtons(items);
			return items.ToArray();
		}
		private ToolStripItem[] GetPopupMenuItems() {
			List<ToolStripItem> items = new List<ToolStripItem>();
			AddMenuItems(items);
			return items.ToArray();
		}
		private void OnAddItemButtonClick(object sender, EventArgs e) {
			AddNewItem();
			UpdateTools();
		}
		private void OnInsertButtonClick(object sender, EventArgs e) {
			InsertNewItem();
			UpdateTools();
		}
		private void OnMoveUpItemButtonClick(object sender, EventArgs e) {
			MoveUpItem();
		}
		private void OnMoveDownItemButtonClick(object sender, EventArgs e) {
			MoveDownItem();
		}
		private void OnRemoveAllItemsButtonClick(object sender, EventArgs e) {
			if(MessageBoxEx.Show(this, GetRemoveAllConfirmString(), StringResources.ItemsEditor_ConfirmDialogCaption, MessageBoxButtonsEx.OKCancel) == DialogResultEx.OK) {
				fPropertyGrid.SelectedObject = null;
				RemoveAllItems();
				UpdateTools();
				ComponentChanged(false, true);
			}
		}
		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			if(e.ChangedItem.GridItemType == GridItemType.Property)
				SetVisiblePropertyInViewer();
			PropertyValueChanged(e);
			ComponentChanged(false, true);
		}
		private void OnRemoveItemButtonClick(object sender, EventArgs e) {
			RemoveItem();
			UpdateTools();
		}
		private void OnSelectAllButtonClick(object sender, EventArgs e) {
			SelectAll();
		}
		private void AddObjectNameLabel(Panel panel) {
			Label label = ControlCreator.CreateLabel("Properties:");
			panel.Controls.Add(label);
		}
	}
}
