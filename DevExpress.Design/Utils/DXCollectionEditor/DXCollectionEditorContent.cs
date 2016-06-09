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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design.Internal;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Design {
	[ToolboxItem(false)]
	public partial class DXCollectionEditorContent : XtraUserControl, ICollectionEventsProvider
#if DEBUGTEST
, ICollectionEditorContentTest
#endif
 {
		public DXCollectionEditorContent() {
			InitializeComponent();
			disposableElements = new List<IDisposable>();
			InitializeMultiColumnListView();
			InitializePopUpMenus();
			InitIcons();
			Initialize(null, null);
		}
		List<IDisposable> disposableElements;
		Type[] newItemTypes;
		DXCollectionEditorBase.ColumnHeader[] columnHeaders;
		ExternalCollectionHelper externalCollectionHelper;
		InternalCollectionHelper internalCollectionHelper;
		bool isCollectionDirty = false;
		int defaultColumnWidth = 65;
		int listBoxPanelDefaultWidth = 130;
		int gridPanelDefaultWidth = 230;
		int contentContainerDefaultHeight = 200;
		int previewContainerPadding = 30;
		#region Initialize and Clear
		void InitIcons() {
			upItemButton.Text = String.Empty;
			downItemButton.Text = String.Empty;
			searchButton.Text = String.Empty;
			upItemButton.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16UpIndex];
			downItemButton.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16DownIndex];
			searchButton.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.FindImage;
		}
		public void Initialize(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] newItemTypeInfos, DevExpress.Utils.Design.DXCollectionEditorBase.UISettings settings) {
			settings = GetSettings(settings);
			InitMultiColumListViewImageList(newItemTypeInfos);
			InitializeColumnsFieldName(settings.ColumnHeaders);
			InitializeSearchPanel(settings.AllowSearch);
			SetReorderingButtonsVisible(settings.AllowReordering);
			InitializeColumns(settings.ColumnHeaders);
			InitializePreviewControl(settings.PreviewControl, settings.ShowPreviewControlBorder);
			this.newItemTypes = GetItemTypesFromInfos(newItemTypeInfos);
			bool isCanAdd = (externalCollectionHelper != null) && externalCollectionHelper.CanAddNewItem(this.newItemTypes);
			InizializeDropDownButton(newItemTypeInfos);
			InitializeFormIsCollectionReadOnly(IsReadOnly, isCanAdd);
			InitializeInternalHelper(externalCollectionHelper, newItemTypeInfos);
		}
		bool IsReadOnly {
			get { return (externalCollectionHelper == null) || externalCollectionHelper.IsReadOnly; }
		}
		void InitializeInternalHelper(ExternalCollectionHelper externalHelper, DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] newItemTypes) {
			if(externalHelper != null) {
				internalCollectionHelper = new InternalCollectionHelper(externalHelper.Collection, this, newItemTypes);
				UpdateListView(internalCollectionHelper.Collection.FirstOrDefault());
			}
		}
		protected void RefreshContent() {
			if(externalCollectionHelper != null) {
				internalCollectionHelper.Refresh(externalCollectionHelper.Collection);
				UpdateListView(internalCollectionHelper.Collection.FirstOrDefault());
			}
		}
		void InitializeSearchPanel(bool isVisible) {
			this.searchButton.Visible = isVisible;
			this.itemsSearchControl.Client = multiColumnListView.MultiColumnListBox as ISearchControlClient;
		}
		void InizializeDropDownButton(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
			if(itemTypeInfos != null) {
				addItemButtonPopupMenu.Items.Clear();
				FillAddItemButton(itemTypeInfos);
				UpdateAddItemButton(itemTypeInfos.FirstOrDefault());
				Change_AddItemButton_Style(itemTypeInfos.Length);
			}
		}
		DXPopupMenu addItemButtonPopupMenu, addRemovePopupMenu;
		DXMenuItem removeItem;
		DXSubMenuItem addItem;
		void InitializePopUpMenus() {
			addItemButtonPopupMenu = new DXPopupMenu();
			addRemovePopupMenu = new DXPopupMenu();
			addItem = new DXSubMenuItem("Add");
			removeItem = new DXMenuItem("Remove");
			removeItem.Click += new EventHandler(RemoveButton_Click);
			this.addRemovePopupMenu.Items.Add(addItem);
			this.addRemovePopupMenu.Items.Add(removeItem);
			disposableElements.Add(addItemButtonPopupMenu);
			disposableElements.Add(addRemovePopupMenu);
			disposableElements.Add(addItem);
			disposableElements.Add(removeItem);
		}
		void InitializeFormIsCollectionReadOnly(bool isReadOnly, bool isCanAdd) {
			this.addItemDropDownButton.Enabled = (!isReadOnly & isCanAdd);
			this.removeItemButton.Enabled &= !isReadOnly;
			this.upItemButton.Enabled &= !isReadOnly;
			this.downItemButton.Enabled &= !isReadOnly;
		}
		IMultiColumnListBox multiColumnListView;
		void InitializeMultiColumnListView() {
			this.multiColumnListView = MultiColumnListBoxCreator.CreateMultiColumnListBox();
			this.disposableElements.Add(this.multiColumnListView.MultiColumnListBox);
			this.multiColumnListView.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.multiColumnListView.MultiColumnListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.multiColumnListView.MultiColumnListBox.Name = "listView1";
			this.itemsGroupControl.Controls.Add(multiColumnListView.MultiColumnListBox);
			((Control)multiColumnListView).BringToFront();
			this.multiColumnListView.SelectedItemChanged += ListView_SelectedItemChanged;
			this.multiColumnListView.MultiColumnListBox.MouseDown += this.MultiColumnListView_MouseDown;
		}
		void InitMultiColumListViewImageList(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
			if(itemTypeInfos != null) {
				ImageList imageList = new ImageList();
				foreach(var itemTypeInfo in itemTypeInfos) {
					if(itemTypeInfo.Image != null)
						imageList.Images.Add(itemTypeInfo.Image);
				}
				this.multiColumnListView.ImageList = imageList;
				this.disposableElements.Add(imageList);
			}
		}
		string[] columnsFieldName = { "Name", "Type" };
		void InitializeColumnsFieldName(DXCollectionEditorBase.ColumnHeader[] columnHeaders) {
			int i = 0;
			foreach(DXCollectionEditorBase.ColumnHeader header in columnHeaders) {
				header.FieldName = columnsFieldName[i % 2];
				i++;
			}
		}
		void InitializeColumns(DXCollectionEditorBase.ColumnHeader[] columnHeaders) {
			this.columnHeaders = columnHeaders;
			multiColumnListView.Properties.Columns.Clear();
			multiColumnListView.Properties.ShowHeader = columnHeaders.Length > 1;
			foreach(DXCollectionEditorBase.ColumnHeader header in columnHeaders) {
				multiColumnListView.Properties.Columns.Add(new XtraEditors.Controls.LookUpColumnInfo(header.FieldName, defaultColumnWidth, header.Caption));
			}
		}
		IDXCollectionEditorPreviewControl previewControl;
		void InitializePreviewControl(IDXCollectionEditorPreviewControl previewControl, bool isShowGroupControl) {
			if(previewControl != null && previewControl.Control != null) {
				this.previewControl = (IDXCollectionEditorPreviewControl)previewControl;
				PreviewContainerVisibileChange(true);
				previewControl.Control.Dock = DockStyle.Fill;
				Control preview = null;
				if(isShowGroupControl) {
					DevExpress.XtraEditors.GroupControl previewGroupControl = new GroupControl();
					InitializePreviewGroupControl(previewGroupControl, previewControl.Control);
					preview = previewGroupControl;
				}
				else {
					preview = previewControl.Control;
				}
				this.previewSplitContainerControl.Panel1.Controls.Clear();
				this.previewSplitContainerControl.Panel1.Controls.Add(preview);
			}
			else
				PreviewContainerVisibileChange(false);
		}
		void InitializePreviewGroupControl(DevExpress.XtraEditors.GroupControl previewGroupControl, Control previewControl) {
			((System.ComponentModel.ISupportInitialize)(previewGroupControl)).BeginInit();
			previewGroupControl.CaptionImageUri.Uri = "";
			previewGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			previewGroupControl.Location = new System.Drawing.Point(0, 0);
			previewGroupControl.Name = "previewGroupControl";
			previewGroupControl.Size = new System.Drawing.Size(362, 293);
			previewGroupControl.TabIndex = 0;
			previewGroupControl.Text = "Preview";
			((System.ComponentModel.ISupportInitialize)(previewGroupControl)).EndInit();
			this.disposableElements.Add(previewGroupControl);
			previewGroupControl.Controls.Add(previewControl);
		}
		void PreviewContainerVisibileChange(bool isVisible) {
			var previewContainerSize = new System.Drawing.Size(0, 0);
			int resizablePreviewControlWidth = 0;
			int resizablePreviewControlHeight = 0;
			var resizablePreviewControl = this.previewControl as IXtraResizableControl;
			if(resizablePreviewControl != null) {
				resizablePreviewControlWidth = resizablePreviewControl.MinSize.Width + previewContainerPadding;
				resizablePreviewControlHeight = resizablePreviewControl.MinSize.Height + previewContainerPadding;
			}
			previewContainerSize = new Size(resizablePreviewControlWidth, resizablePreviewControlHeight);
			SetContentSizes(previewContainerSize);
			this.previewSplitContainerControl.PanelVisibility = isVisible ? DevExpress.XtraEditors.SplitPanelVisibility.Both : DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
		}
		void SetContentSizes(Size previewContainerSize) {
			if(this.Parent != null) {
				int parentFormMinWidth = listBoxPanelDefaultWidth + previewContainerSize.Width + gridPanelDefaultWidth;
				int contentContainerHeight = previewContainerSize.Height < contentContainerDefaultHeight ? contentContainerDefaultHeight : previewContainerSize.Height;
				int heightDifference = this.Parent.Size.Height - this.itemsListSplitContainerControl.Height;
				int parentFormMinHeight = contentContainerHeight + heightDifference;
				this.Parent.MinimumSize = new Size(parentFormMinWidth, parentFormMinHeight);
				this.itemsListSplitContainerControl.Panel2.MinSize = previewContainerSize.Width + gridPanelDefaultWidth;
				this.itemsListSplitContainerControl.Panel1.MinSize = listBoxPanelDefaultWidth;
				this.previewSplitContainerControl.Panel2.MinSize = gridPanelDefaultWidth;
				this.previewSplitContainerControl.Panel1.MinSize = previewContainerSize.Width;
				this.previewSplitContainerControl.SplitterPosition = previewContainerSize.Width;
				float widthCoef = previewContainerSize.Width == 0 ? 3.0f : 2.0f;
				float heightCoef = 2.5f;
				int defaultParentFormWidth = previewContainerSize.Width + (int)((float)gridPanelDefaultWidth * widthCoef);
				this.itemsListSplitContainerControl.SplitterPosition = (int)((float)gridPanelDefaultWidth * widthCoef / 2.0f);
				int defaultParentFormHeight = (int)((float)contentContainerDefaultHeight * heightCoef);
				int parentFormWidth = parentFormMinWidth < defaultParentFormWidth ? defaultParentFormWidth : parentFormMinWidth;
				int parenFormHeight = parentFormMinHeight < defaultParentFormHeight ? defaultParentFormHeight : parentFormMinHeight;
				this.Parent.Size = new Size(parentFormWidth, parenFormHeight);
			}
		}
		DevExpress.Utils.Design.DXCollectionEditorBase.UISettings GetSettings(DevExpress.Utils.Design.DXCollectionEditorBase.UISettings settings) {
			if(settings == null)
				return new DevExpress.Utils.Design.DXCollectionEditorBase.UISettings
				{
					AllowReordering = true,
					AllowSearch = true,
					ColumnHeaders = GetDefaultColumnHeaders(),
					ShowPreviewControlBorder = true,
					PreviewControl = null
				};
			else
				if(settings.ColumnHeaders == null)
					settings.ColumnHeaders = GetDefaultColumnHeaders();
			return settings;
		}
		DXCollectionEditorBase.ColumnHeader[] GetDefaultColumnHeaders() {
			return new DXCollectionEditorBase.ColumnHeader[] { new DXCollectionEditorBase.ColumnHeader() { FieldName = columnsFieldName[0], Caption = columnsFieldName[0] } };
		}
		public void Clear(DialogResult result) {
			if(IsReadOnly || this.internalCollectionHelper == null || !isCollectionDirty)
				return;
			this.isContentChanged = internalCollectionHelper.ChangeCollection(result == DialogResult.OK);
			externalCollectionHelper.ChangeCollection(internalCollectionHelper);
		}
		public void Reset() {
			this.externalCollectionHelper = null;
			if(this.internalCollectionHelper != null)
				this.internalCollectionHelper.Reset();
		}
		public void InitializeEditContex(ITypeDescriptorContext context, IServiceProvider provider) {
			this.propertyGrid.Site = new PropGridSite(provider, context);
		}
		#endregion
		#region Propereties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable Items {
			get { return externalCollectionHelper.Collection; }
			set { externalCollectionHelper = new ExternalCollectionHelper(value); }
		}
		bool isContentChanged = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsContentChanged {
			get { return this.isContentChanged; }
		}
		protected DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx PropertyGrid {
			get { return this.propertyGrid; }
		}
		#endregion
		#region Private Methods
		Type[] GetItemTypesFromInfos(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
			if(itemTypeInfos == null) return null;
			return (from typeInfo in itemTypeInfos select typeInfo.Type).ToArray();
		}
		object GetElement(object selectedObject) {
			IDXObjectWrapper wrapper = selectedObject as IDXObjectWrapper;
			return ((wrapper != null) ? wrapper.SourceObject : selectedObject);
		}
		void UpdateDataRepositoryItem(object item) {
			var repositoryItem = this.internalCollectionHelper.Find(item) as DevExpress.Utils.Design.InternalCollectionHelper.DataRepositoryItem;
			if(repositoryItem != null) {
				this.multiColumnListView.Properties.BeginUpdate();
				repositoryItem.Name = ((ICollectionEventsProvider)this).GetCustomDisplayText(item, columnsFieldName[0]);
				repositoryItem.Type = GetItemType(item);
				this.multiColumnListView.MultiColumnListBox.Invalidate();
				this.multiColumnListView.Properties.EndUpdate();
			}
		}
		void UpdateListView(object selectedItem) {
			this.multiColumnListView.MultiColumnListBox.SuspendLayout();
			this.multiColumnListView.Properties.DataSource = internalCollectionHelper.Collection;
			this.multiColumnListView.Update();
			this.multiColumnListView.MultiColumnListBox.ResumeLayout();
			this.multiColumnListView.SelectedItem = selectedItem;
			this.itemsGroupControl.Text = String.Format("Items({0})", this.internalCollectionHelper.Count);
		}
		void UpdateAddItemButton(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo typeInfo) {
			addItemDropDownButton.Text = "Add(" + typeInfo.Type.Name + ")";
			addItemDropDownButton.Tag = typeInfo;
		}
		void OnSelectedItemChanged() {
			object selectedItem = null;
			var repositoryItem = GetSelectedItem();
			if(repositoryItem != null)
				selectedItem = repositoryItem.Tag;
			SetRemoveButtonState(selectedItem);
			if(externalCollectionHelper != null)
				SetReorderingButtonsEnabled(externalCollectionHelper.IsReadOnly);
			RefreshPropertyGrid(selectedItem);
			RaiseSelectedItemChanged(new SelectedItemChangedEventArgs(selectedItem));
		}
		InternalCollectionHelper.IDataRepositoryItem GetSelectedItem() {
			return this.multiColumnListView.SelectedItem as DevExpress.Utils.Design.InternalCollectionHelper.IDataRepositoryItem;
		}
		void SetRemoveButtonState(object selectedItem) {
			this.removeItemButton.Enabled = (selectedItem != null && this.internalCollectionHelper.Count > 0);
		}
		void SetReorderingButtonsEnabled(bool isReadOnly) {
			this.upItemButton.Enabled = this.downItemButton.Enabled = !(this.internalCollectionHelper.Count <= 1 || isReadOnly);
		}
		void SetReorderingButtonsVisible(bool isVisible) {
			this.upItemButton.Visible = this.downItemButton.Visible = isVisible;
		}
		void Change_AddItemButton_Style(int elementsCount) {
			if(elementsCount <= 1) {
				addItemDropDownButton.DropDownArrowStyle = DropDownArrowStyle.Hide;
				addItemDropDownButton.Text = "Add";
				addItemDropDownButton.Width = removeItemButton.Width;
			}
			else {
				addItemDropDownButton.DropDownArrowStyle = DropDownArrowStyle.Default;
				addItemDropDownButton.Width = 135;
			}
		}
		void FillAddItemButton(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
#if DEBUGTEST
			newItemTypesForTests = this.newItemTypes;
#endif
			foreach(var type in itemTypeInfos) {
				DXMenuItem dxItem = new DXMenuItem(type.Type.Name, new EventHandler(OnClickMenuItem), type.Image);
				disposableElements.Add(dxItem);
				dxItem.Tag = type;
				addItemButtonPopupMenu.Items.Add(dxItem);
				addItem.Items.Add(dxItem);
			}
		}
		string GetItemType(object item) {
			return item.GetType().ToString();
		}
		void PropertyValueChanged(object item, string propertyName) {
			if(item != null) {
				PropertyItemChangedEventArgs args = new PropertyItemChangedEventArgs(item, propertyName);
				PropertyItemChangedEventHandler handler = Events[itemChanged] as PropertyItemChangedEventHandler;
				if(handler != null)
					handler(this, args);
				if(args.UpdateVisibleInfo)
					this.UpdateDataRepositoryItem(item);
				if(this.previewControl != null)
					this.previewControl.OnItemChanged(args);
			}
		}
		object AddItemFromQuery(Type newItemType) {
			QueryNewItemEventArgs args = new QueryNewItemEventArgs(newItemType);
			QueryNewItemEventHandler handler = Events[addNewItem] as QueryNewItemEventHandler;
			if(handler != null)
				handler(this, args);
			OnAddItem(args.Item);
			return args.Item;
		}
		void ICollectionEventsProvider.RaiseCollectionChanging(CollectionChangingEventArgs args) {
			CollectionChangingEventHandler handler = Events[collectionChanging] as CollectionChangingEventHandler;
			CollectionChangingEventArgs previewArgs = new CollectionChangingEventArgs(args.Action, args.Item, args.TargetItem);
			if(handler != null)
				handler(this, args);
			if(this.previewControl != null) {
				this.previewControl.OnCollectionChanging(previewArgs);
			}
		}
		void ICollectionEventsProvider.RaiseCollectionChanged(CollectionChangedEventArgs args) {
			CollectionChangedEventHandler handler = Events[collectionChanged] as CollectionChangedEventHandler;
			if(handler != null)
				handler(this, args);
			if(this.previewControl != null)
				this.previewControl.OnCollectionChanged(args);
		}
		void RaiseSelectedItemChanged(SelectedItemChangedEventArgs e) {
			if(this.previewControl != null)
				previewControl.OnSelectedItemChanged(e);
		}
		string ICollectionEventsProvider.GetCustomDisplayText(object element, string fieldName) {
			CustomDisplayTextEventArgs args = new CustomDisplayTextEventArgs(element, fieldName);
			QueryCustomDisplayTextEventHandler handler = Events[queryСustomDisplayText] as QueryCustomDisplayTextEventHandler;
			if(handler != null)
				handler(this, args);
			return args.DisplayText;
		}
		#endregion
		#region Protected Methods
		protected void OnAddItem(object item) {
			isCollectionDirty = true;
			internalCollectionHelper.OnAddNewItem(item);
		}
		protected object OnRemoveItem(object item) {
			isCollectionDirty = true;
			return internalCollectionHelper.OnRemoveItem(item);
		}
		protected void MoveSelectedItem(int offset) {
			isCollectionDirty = true;
			var selectedItem = GetSelectedItem();
			internalCollectionHelper.MoveItem(selectedItem, offset);
			UpdateListView(selectedItem);
		}
		protected void RefreshPropertyGrid(object item) {
			this.propertyGrid.SelectedObject = item;
		}
		protected void AddOtherButton(SimpleButton button, bool? isFirst = true, int index = 0) {
			this.otherButtonsStackPanel.Controls.Add(button);
			switch(isFirst) {
				case true:
					this.otherButtonsStackPanel.Controls.SetChildIndex(button, 0);
					break;
				case false:
					break;
				case null:
					break;
				default:
					break;
			}
		}
		string parentStoreName = "$ContentContainer";
		string itemsSplitterPositionProperty = "ListBoxSplitterPosition";
		string previewSplitterPositionProperty = "PreviewControlSplitterPosition";
		string searchButtonCheck = "SearchButtonCheck";
		protected internal void Store(DevExpress.Utils.Design.PropertyStore parentStore) {
			if(parentStore != null) {
				DevExpress.Utils.Design.PropertyStore store = new DevExpress.Utils.Design.PropertyStore(parentStore, parentStoreName);
				store.AddProperty(itemsSplitterPositionProperty, this.itemsListSplitContainerControl.SplitterPosition);
				store.AddProperty(previewSplitterPositionProperty, this.previewSplitContainerControl.SplitterPosition);
				store.AddProperty(searchButtonCheck, this.searchButton.Checked);
				store.Store();
			}
		}
		protected internal void Restore(DevExpress.Utils.Design.PropertyStore parentStore) {
			if(parentStore != null) {
				DevExpress.Utils.Design.PropertyStore store = parentStore.RestoreProperty(parentStoreName, null) as DevExpress.Utils.Design.PropertyStore;
				if(store != null) {
					store.Restore();
					this.itemsListSplitContainerControl.SplitterPosition = store.RestoreIntProperty(itemsSplitterPositionProperty, this.itemsListSplitContainerControl.SplitterPosition);
					this.previewSplitContainerControl.SplitterPosition = store.RestoreIntProperty(previewSplitterPositionProperty, this.previewSplitContainerControl.SplitterPosition);
					this.searchButton.Checked = store.RestoreBoolProperty(searchButtonCheck, searchButton.Checked);
				}
			}
		}
		#endregion
		#region Delegate and Events
		readonly static object itemChanged = new object();
		readonly static object queryСustomDisplayText = new object();
		readonly static object addNewItem = new object();
		readonly static object collectionChanged = new object();
		readonly static object collectionChanging = new object();
		public event PropertyItemChangedEventHandler ItemChanged {
			add { this.Events.AddHandler(itemChanged, value); }
			remove { this.Events.RemoveHandler(itemChanged, value); }
		}
		public event QueryCustomDisplayTextEventHandler QueryCustomDisplayText {
			add { this.Events.AddHandler(queryСustomDisplayText, value); }
			remove { this.Events.RemoveHandler(queryСustomDisplayText, value); }
		}
		public event QueryNewItemEventHandler QueryNewItem {
			add { this.Events.AddHandler(addNewItem, value); }
			remove { this.Events.RemoveHandler(addNewItem, value); }
		}
		public event CollectionChangedEventHandler CollectionChanged {
			add { this.Events.AddHandler(collectionChanged, value); }
			remove { this.Events.RemoveHandler(collectionChanged, value); }
		}
		public event CollectionChangingEventHandler CollectionChanging {
			add { this.Events.AddHandler(collectionChanging, value); }
			remove { this.Events.RemoveHandler(collectionChanging, value); }
		}
		#endregion
		#region Events handler
		void PgMain_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			this.PropertyValueChanged(this.GetElement(((PropertyGrid)sender).SelectedObject), e.ChangedItem.PropertyDescriptor.Name);
		}
		void AddItemDropDownButton_Click(object sender, EventArgs e) {
			object item = AddItemFromQuery(((DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo)addItemDropDownButton.Tag).Type);
			if(item == null) return;
			var repositoryItem = internalCollectionHelper.Find(item);
			UpdateListView(repositoryItem);
		}
		void RemoveButton_Click(object sender, System.EventArgs e) {
			object currentSelectItem = internalCollectionHelper.Find(OnRemoveItem(this.multiColumnListView.SelectedItem));
			UpdateListView(currentSelectItem);
		}
		void ListView_SelectedItemChanged(object sender, System.EventArgs e) {
			OnSelectedItemChanged();
		}
		void UpItemButton_Click(object sender, EventArgs e) {
			MoveSelectedItem(-1);
		}
		void DownItemButton_Click(object sender, EventArgs e) {
			MoveSelectedItem(1);
		}
		void SearchButton_CheckedChanged(object sender, EventArgs e) {
			CheckButton searchButton = sender as CheckButton;
			if(searchButton != null) {
				this.itemsSearchControl.Visible = searchButton.Checked;
				if(!searchButton.Checked) {
					this.itemsSearchControl.SetFilter(null);
				}
			}
		}
		void AddItemDropDownButton_ArrowButtonClick(object sender, EventArgs e) {
			MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(addItemButtonPopupMenu, addItemDropDownButton, new Point(0, addItemDropDownButton.Size.Height));
		}
		void OnClickMenuItem(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			UpdateAddItemButton(((DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo)item.Tag));
			AddItemDropDownButton_Click(addItemDropDownButton, EventArgs.Empty);
		}
		void MultiColumnListView_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				this.BeginInvoke(new MethodInvoker(delegate() {
					MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(addRemovePopupMenu, multiColumnListView.MultiColumnListBox, new Point(e.X, e.Y));
				}));
			}
		}
		void ItemsSearchControl_Properties_EditValueChanging(object sender, XtraEditors.Controls.ChangingEventArgs e) { }
		#endregion
		class PropGridSite : ISite, IServiceProvider {
			IServiceProvider serviceProvider;
			ITypeDescriptorContext context;
			IComponent component;
			bool isGetService = false;
			public PropGridSite(IServiceProvider serviceProvider, ITypeDescriptorContext context) {
				this.serviceProvider = serviceProvider;
				this.context = context;
				this.component = GetComponent();
			}
			public IComponent Component {
				get { return this.component; }
			}
			public IContainer Container {
				get {
					if(context == null) return null;
					return context.Container;
				}
			}
			public bool DesignMode {
				get { return false; }
			}
			public string Name {
				get { return component.Site.Name; }
				set { }
			}
			public object GetService(System.Type t) {
				if(!this.isGetService) {
					if(this.serviceProvider != null) {
						try {
							this.isGetService = true;
							return this.serviceProvider.GetService(t);
						}
						finally {
							this.isGetService = false;
						}
					}
				}
				return (object)null;
			}
			IComponent GetComponent() {
				IComponent component = null;
				if(this.context != null)
					component = this.context.Instance as IComponent;
				if(component != null) return component;
				if(this.serviceProvider != null) {
					IDesignerHost host = this.serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
					component = (host == null ? null : host.RootComponent);
				}
				return component;
			}
		}
#if DEBUGTEST
		Type[] newItemTypesForTests;
		ExternalCollectionHelper ICollectionEditorContentTest.ExternalHelper { get { return externalCollectionHelper; } }
		InternalCollectionHelper ICollectionEditorContentTest.InternalHelper { get { return internalCollectionHelper; } }
		void ICollectionEditorContentTest.Clear(DialogResult result) {
			this.Clear(result);
		}
		Type[] ICollectionEditorContentTest.NewItemTypes {
			get { return newItemTypesForTests; }
		}
		object ICollectionEditorContentTest.AddItemFromQuery(Type newItemType) {
			return this.AddItemFromQuery(newItemType);
		}
#endif
	}
#if DEBUGTEST
	interface ICollectionEditorContentTest {
		ExternalCollectionHelper ExternalHelper { get; }
		InternalCollectionHelper InternalHelper { get; }
		void Clear(DialogResult result);
		Type[] NewItemTypes { get; }
		object AddItemFromQuery(Type newItemType);
	}
#endif
	interface ICollectionEventsProvider {
		void RaiseCollectionChanging(CollectionChangingEventArgs args);
		void RaiseCollectionChanged(CollectionChangedEventArgs args);
		string GetCustomDisplayText(object element, string fieldName);
	}
	public interface IDXCollectionEditorPreviewControl {
		Control Control { get; }
		void OnCollectionChanged(CollectionChangedEventArgs args);
		void OnItemChanged(PropertyItemChangedEventArgs args);
		void OnSelectedItemChanged(SelectedItemChangedEventArgs args);
		void OnCollectionChanging(CollectionChangingEventArgs args);
	}
}
