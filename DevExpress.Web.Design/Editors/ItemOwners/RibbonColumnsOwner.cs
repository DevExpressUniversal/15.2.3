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
using System.Linq;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Design;
using DevExpress.Web.ASPxRichEdit.Design;
using DevExpress.Web.ASPxSpreadsheet.Design;
using DevExpress.Web.DocumentViewer.Design;
namespace DevExpress.Web.Design {
	public class RibbonItemsOwnerCollection {
		List<RibbonItemsOwner> listEditors;
		public RibbonItemsOwnerCollection() {
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new HtmlEditorRibbonItemsOwner(null); }));
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new SpreadsheetRibbonItemsOwner(null); }));
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new SpreadsheetRibbonContextTabCategoriesOwner(null); }));
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new RichEditRibbonItemsOwner(null); }));
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new RichEditRibbonContextTabItemsOwner(null); }));
			AddItemsOwner(SafeCreateRibbonItemsOwner(() => { return new DocumentViewerRibbonItemsOwner(null); }));
		}
		public List<RibbonItemsOwner> ListEditors {
			get {
				if(listEditors == null)
					listEditors = new List<RibbonItemsOwner>();
				return listEditors;
			}
		}
		protected void AddItemsOwner(RibbonItemsOwner owner) {
			if(owner != null)
				ListEditors.Add(owner);
		}
		protected RibbonItemsOwner SafeCreateRibbonItemsOwner(Function<RibbonItemsOwner> createFunc) {
			try{
				return createFunc();
			}
			catch{
				return null;
			}
		}
		public void UpdateResourceImageMap(Dictionary<string, int> resourceImageMap) {
			foreach(var editor in ListEditors) {
				Dictionary<int, int> imageIndexDictionary = new Dictionary<int, int>();
				foreach(var key in editor.ResourceImageMap.Keys) {
					if(!resourceImageMap.ContainsKey(key))
						resourceImageMap.Add(key, resourceImageMap.Count);
					imageIndexDictionary.Add(editor.ResourceImageMap[key], resourceImageMap[key]);
				}
				editor.ResourceImageMap.Clear();
				foreach(var key in resourceImageMap.Keys)
					editor.ResourceImageMap.Add(key, resourceImageMap[key]);
				List<IDesignTimeColumnAndEditorItem> newColumnsAndEditors = new List<IDesignTimeColumnAndEditorItem>();
				foreach(var item in editor.ColumnsAndEditors)
					newColumnsAndEditors.Add(new DesignTimeColumnType(item.ColumnType, item.Text, item.EditorName, item.ImageIndex > -1 ? imageIndexDictionary[item.ImageIndex] : -1));
				editor.ColumnsAndEditors.Clear();
				editor.ColumnsAndEditors.AddRange(newColumnsAndEditors);
			}
		}
		public void RemoveItems(List<IDesignTimeColumnAndEditorItem> columnsAndEditors) {
			foreach(var item in columnsAndEditors) {
				foreach(var editor in ListEditors)
					editor.ColumnsAndEditors.RemoveAll(i => i.ColumnType == item.ColumnType);
			}
		}
		public IDesignTimeColumnAndEditorItem FindDesignTimeItem(Type type) {
			foreach(var editor in ListEditors) {
				IDesignTimeColumnAndEditorItem item = editor.ColumnsAndEditors.Find(i => i.ColumnType == type);
				if(item != null)
					return item;
			}
			return null;
		}
		public bool ContainsItemType(Type type) {
			return FindDesignTimeItem(type) != null;
		}
	}
	public class RibbonItemsOwner : ItemsEditorOwner {
		public RibbonItemsOwner(object component, string collectionPropertyName, IServiceProvider provider, IList items)
			: base(component, collectionPropertyName, provider, items) {
		}
		public RibbonItemsOwner(object component, IServiceProvider provider, IList items)
			: base(component, "Ribbon Items", provider, items) {
		}
		public RibbonItemsOwner(object component, IServiceProvider provider)
			: this(component, provider, ((ASPxRibbon)component).Tabs) {
		}
		public RibbonItemsOwner(Collection items)
			: base(null, null, null, items) {
		}
		RibbonItemsOwnerCollection editors;
		protected RibbonItemsOwnerCollection Editors {
			get {
				if(editors == null) {
					editors = new RibbonItemsOwnerCollection();
					editors.RemoveItems(ColumnsAndEditors);
					editors.UpdateResourceImageMap(ResourceImageMap);
				}
				return editors;
			}
		}
		protected override void FillItemTypes() { 
			AddItemType(typeof(RibbonTab), "Tab", TabControlItemImageResource);
			AddItemType(typeof(RibbonContextTabCategory), "TabCategory", RibbonContextTabImageResource);
			AddItemType(typeof(RibbonGroup), "Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RibbonButtonItem), "Button", RibbonButtonItemImageResource);
			AddItemType(typeof(RibbonCheckBoxItem), "Check Box", "Check Box", CheckColumnImageResource);
			AddItemType(typeof(RibbonColorButtonItem), "Color Button", ColorEditImageResource);
			AddItemType(typeof(RibbonComboBoxItem), "Combo Box", "Combo Box", ComboboxImageResource);
			AddItemType(typeof(RibbonDateEditItem), "Date Edit", "Date Edit", DateEditImageResource);
			AddItemType(typeof(RibbonDropDownButtonItem), "DropDown Button", DropDownButtonImageResource);
			AddItemType(typeof(RibbonDropDownToggleButtonItem), "DropDown Toggle Button", DropDownToggleButtonImageResource);
			AddItemType(typeof(RibbonGalleryBarItem), "Gallery Bar", RibbonGalleryBarResource);
			AddItemType(typeof(RibbonGalleryDropDownItem), "Gallery DropDown", RibbonGalleryDropDownResource);
			AddItemType(typeof(RibbonGalleryGroup), "Gallery Group", RibbonGalleryGroupResource);
			AddItemType(typeof(RibbonGalleryItem), "Gallery Item", RibbonGalleryItemResource);
			AddItemType(typeof(ListEditItem), "ListEdit Item", ListEditImageResource);
			AddItemType(typeof(RibbonOptionButtonItem), "Option Button", RibbonOptionButtonItemImageResource);
			AddItemType(typeof(RibbonSpinEditItem), "Spin Edit", "Spin Edit", SpinEditImageResource);
			AddItemType(typeof(RibbonTemplateItem), "Template", RibbonTemplateItemImageResource);
			AddItemType(typeof(RibbonTextBoxItem), "Text Box", "Text Box", TextImageResource);
			AddItemType(typeof(RibbonToggleButtonItem), "Toggle Button", RibbonToggleButtonItemImageResource);
		}
		protected override void FillMenuItemResouceImages() {
			base.FillMenuItemResouceImages();
			AddResourceImage(AddChildItemImageResource);
		}
		public override bool CanMoveItem(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(!base.CanMoveItem(source, target, direction))
				return false;
			var sourceType = source.GetType();
			var targetType = target != null ? target.GetType() : null;
			var targetParentType = target != null && target.Parent != null ? target.Parent.GetType() : null;
			if(direction == InsertDirection.Inside)
				return CanAssociate(sourceType, targetType, null, Editors);
			return CanAssociate(sourceType, targetParentType, null, Editors);
		}
		protected override bool IsSelectionChangingEnabled() {
			if(SelectedItems.Count == 0)
				return false;
			return SelectedItems.All(i => i.Parent != null && typeof(RibbonGroup).IsAssignableFrom(i.Parent.GetType()));
		}
		Type defaultItemType;
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			IDesignTimeCollectionItem parentItem;
			if(direction == InsertDirection.Inside)
				parentItem = target;
			else
				parentItem = target != null ? target.Parent : null;
			this.defaultItemType = GetDefaultType(parentItem);
			base.AddItemCore(designTimeItem, target, direction);
		}
		public override Type GetDefaultItemType() {
			return this.defaultItemType;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			var canCreateBase = base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
			if(!canCreateBase)
				return false;
			var focusedItem = FocusedItemForAction;
			if(!isToolbar && parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.InsertChild) {
				if(focusedItem == null || focusedItem.GetType() == typeof(RibbonContextTabCategory) || focusedItem.GetType() == typeof(RibbonTab) || focusedItem.GetType() == typeof(RibbonComboBoxItem)
					|| focusedItem.GetType() == typeof(RibbonGalleryBarItem) || focusedItem.GetType() == typeof(RibbonGalleryDropDownItem)
					|| focusedItem.GetType() == typeof(RibbonGalleryGroup))
					return false;
			}
			IDesignTimeColumnAndEditorItem parentDesignTimeItem;
			switch(parentMenuItem.ActionType) {
				case DesignEditorMenuRootItemActionType.AddItem:
				case DesignEditorMenuRootItemActionType.InsertBefore:
					parentDesignTimeItem = focusedItem != null && focusedItem.Parent != null ? FindDesignTimeItem(focusedItem.Parent.GetType()) : null;
					return CanAssociate(designTimeItem, parentDesignTimeItem, parentMenuItem);
				case DesignEditorMenuRootItemActionType.InsertChild:
					parentDesignTimeItem = focusedItem != null ? FindDesignTimeItem(focusedItem.GetType()) : null;
					return CanAssociate(designTimeItem, parentDesignTimeItem, parentMenuItem);
				case DesignEditorMenuRootItemActionType.ChangeTo:
					return CanAssociate(designTimeItem, FindDesignTimeItem(typeof(RibbonGroup)), parentMenuItem);
			}
			return true;
		}
		protected override DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = base.CreateInsertChildMenuItem(isToolbar);
			if(isToolbar)
				return item;
			var focusedItem = FocusedItemForAction;
			if(focusedItem == null && IsContextTabCategoriesCollection()) {
				item.Caption = "Add Context Tab Category";
				item.ImageIndex = GetResourceImageIndex(TabControlItemImageResource);
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(RibbonContextTabCategory));
			} else if(focusedItem == null || focusedItem.GetType() == typeof(RibbonContextTabCategory)) {
				item.Caption = "Add Tab";
				item.ImageIndex = GetResourceImageIndex(RibbonContextTabImageResource);
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(RibbonTab));
			} else if(focusedItem.GetType() == typeof(RibbonTab)) { 
				item.Caption = "Add Group";
				item.ImageIndex = GetResourceImageIndex(RibbonGroupItemImageResource);
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(RibbonGroup));
			} else if(focusedItem.GetType() == typeof(RibbonComboBoxItem)) {
				item.Caption = "Add ListEdit Item";
				item.ImageIndex = GetResourceImageIndex(MemoImageResource); 
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(ListEditItem));
			} else if(focusedItem.GetType() == typeof(RibbonGalleryBarItem) ||
				focusedItem.GetType() == typeof(RibbonGalleryDropDownItem)) {
				item.Caption = "Add Gallery Group";
				item.ImageIndex = GetResourceImageIndex(RibbonGalleryGroupResource);
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(RibbonGalleryGroup));
			} else if(focusedItem.GetType() == typeof(RibbonGalleryGroup)) {
				item.Caption = "Add Gallery Item";
				item.ImageIndex = GetResourceImageIndex(RibbonGalleryItemResource);
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(RibbonGalleryItem));
			} else { 
				item.Caption = "Add Item";
			}
			return item;
		}
		protected virtual bool CanAssociate(IDesignTimeColumnAndEditorItem child, IDesignTimeColumnAndEditorItem parent, DesignEditorDescriptorItem menuItem) {
			var parentItemType = parent == null ? null : parent.ColumnType;
			return CanAssociate(child.ColumnType, parentItemType, menuItem);
		}
		protected virtual bool CanAssociate(Type childType, Type parentType, DesignEditorDescriptorItem menuItem, RibbonItemsOwnerCollection editors) {
			if(Editors.ContainsItemType(childType) || Editors.ContainsItemType(parentType)) {
				foreach(var editor in Editors.ListEditors) {
					if(editor.CanAssociate(childType, parentType, menuItem))
						return true;
				}
			}
			return CanAssociate(childType, parentType, menuItem);
		}
		protected virtual bool CanAssociate(Type childType, Type parentType, DesignEditorDescriptorItem menuItem) {
			if(parentType == null && IsContextTabCategoriesCollection())
				return typeof(RibbonContextTabCategory).IsAssignableFrom(childType);
			if(parentType == null)
				return typeof(RibbonTab).IsAssignableFrom(childType);
			if(typeof(RibbonContextTabCategory).IsAssignableFrom(parentType))
				return typeof(RibbonTab).IsAssignableFrom(childType);
			if(typeof(RibbonTab).IsAssignableFrom(parentType))
				return typeof(RibbonGroup).IsAssignableFrom(childType);
			if(typeof(RibbonGroup).IsAssignableFrom(parentType))
				return typeof(RibbonItemBase).IsAssignableFrom(childType) && !Editors.ContainsItemType(childType);
			if(typeof(RibbonDropDownButtonItem).IsAssignableFrom(parentType))
				return typeof(RibbonDropDownButtonItem).IsAssignableFrom(childType);
			if(typeof(RibbonComboBoxItem).IsAssignableFrom(parentType))
				return typeof(ListEditItem).IsAssignableFrom(childType);
			if(typeof(RibbonGalleryBarItem).IsAssignableFrom(parentType) || typeof(RibbonGalleryDropDownItem).IsAssignableFrom(parentType))
				return typeof(RibbonGalleryGroup).IsAssignableFrom(childType);
			if(typeof(RibbonGalleryGroup).IsAssignableFrom(parentType))
				return typeof(RibbonGalleryItem).IsAssignableFrom(childType);
			return false;
		}
		protected virtual bool IsContextTabCategoriesCollection() {
			return Items != null && Items.GetType() == typeof(RibbonContextTabCategoryCollection);
		}
		Type GetDefaultType(IDesignTimeCollectionItem selectedBand) {
			if(selectedBand == null && IsContextTabCategoriesCollection())
				return typeof(RibbonContextTabCategory);
			if(selectedBand == null)
				return typeof(RibbonTab);
			var type = selectedBand.GetType();
			if(typeof(RibbonContextTabCategory).IsAssignableFrom(type))
				return typeof(RibbonTab);
			if(typeof(RibbonTab).IsAssignableFrom(type))
				return typeof(RibbonGroup);
			if(typeof(RibbonGroup).IsAssignableFrom(type))
				return typeof(RibbonButtonItem);
			if(typeof(RibbonDropDownButtonItem).IsAssignableFrom(type))
				return type;
			if(typeof(RibbonComboBoxItem).IsAssignableFrom(type))
				return typeof(ListEditItem);
			if(typeof(RibbonGalleryBarItem).IsAssignableFrom(type) || typeof(RibbonGalleryDropDownItem).IsAssignableFrom(type))
				return typeof(RibbonGalleryGroup);
			if(typeof(RibbonGalleryGroup).IsAssignableFrom(type))
				return typeof(RibbonGalleryItem);
			return null;
		}
		public override List<string> GetViewDependedProperties() {
			var list = base.GetViewDependedProperties();
			list.AddRange(new string[] { 
				"Name", 
				"Text"
			});
			return list;
		}
		protected override IDesignTimeColumnAndEditorItem GetDesignTimeItem(Type itemType) {
			return Editors.FindDesignTimeItem(itemType);
		}
	}
}
