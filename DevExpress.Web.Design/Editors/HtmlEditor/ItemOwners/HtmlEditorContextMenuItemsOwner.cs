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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class DefaultItems { }
	public class HtmlEditorContextMenuItemsOwner : ItemsEditorOwner {
		public HtmlEditorContextMenuItemsOwner(ASPxHtmlEditor htmlEditor, IServiceProvider provider, Collection items)
			: base(htmlEditor, "ContextMenuItems", provider, items) {
		}
		public HtmlEditorContextMenuItemsOwner(HtmlEditorToolbarCollection items)
			: base(null, null, null, items) {
		}
		public ASPxHtmlEditor HtmlEditor { get { return (ASPxHtmlEditor)Component; } }
		protected override void FillItemTypes() {
			AddItemType(typeof(DefaultItems), "Default Items", RibbonButtonItemImageResource);
			AddItemType(typeof(ClipboardGroup), "Clipboard");
			AddGroupItemType(typeof(ClipboardGroup), typeof(HECutContextMenuItem), "Cut", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(HECopyContextMenuItem), "Copy", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(HEPasteContextMenuItem), "Paste", RibbonButtonItemImageResource);
			AddItemType(typeof(DialogGroup), "Dialogs");
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeLinkDialogContextMenuItem), "Change Link", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeImageDialogContextMenuItem), "Change Image", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeAudioDialogContextMenuItem), "Change Audio", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeVideoDialogContextMenuItem), "Change Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeFlashDialogContextMenuItem), "Change Flash", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangeYouTubeVideoDialogContextMenuItem), "Change YouTube Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEChangePlaceholderDialogContextMenuItem), "Change Placeholder", RibbonButtonItemImageResource);
			AddItemType(typeof(IndentGroup), "Indent/Outdent");
			AddGroupItemType(typeof(IndentGroup), typeof(HEIndentContextMenuItem), "Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(IndentGroup), typeof(HEOutdentContextMenuItem), "Outdent", RibbonButtonItemImageResource);
			AddItemType(typeof(TableGroup), "Tables");
			AddGroupItemType(typeof(TableGroup), typeof(InsertTableGroup), "Insert");
			AddGroupItemType(typeof(TableGroup), typeof(DeleteTableGroup), "Delete");
			AddGroupItemType(typeof(TableGroup), typeof(MergeTableGroup), "Merge");
			AddGroupItemType(typeof(TableGroup), typeof(SplitTableGroup), "Split");
			AddGroupItemType(typeof(TableGroup), typeof(TablePropertiesGroup), "Properties");
			AddGroupItemType(typeof(TableGroup), typeof(ToolbarTableOperationsDropDownButton), "Table Operations", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableContextMenuItem), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableColumnToLeftContextMenuItem), "Column To Left", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableColumnToRightContextMenuItem), "Column To Right", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableRowAboveContextMenuItem), "Row Above", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableRowBelowContextMenuItem), "Row Below", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableContextMenuItem), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableColumnContextMenuItem), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableRowContextMenuItem), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(HEMergeTableCellRightContextMenuItem), "Cell Right", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(HEMergeTableCellDownContextMenuItem), "Cell Down", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETablePropertiesDialogContextMenuItem), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableColumnPropertiesDialogContextMenuItem), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableRowPropertiesDialogContextMenuItem), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableCellPropertiesDialogContextMenuItem), "Cell", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(HESplitTableCellVerticalContextMenuItem), "Cell Vertically", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(HESplitTableCellHorizontalContextMenuItem), "Cell Horizontally", RibbonButtonItemImageResource);
			AddItemType(typeof(OtherGroup), "Other");
			AddGroupItemType(typeof(OtherGroup), typeof(HEUnlinkContextMenuItem), "Unlink", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HESelectAllContextMenuItem), "Select All", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HtmlEditorContextMenuItem), "Custom Item", RibbonButtonItemImageResource);
		}
		Type defaultItemType;
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			IDesignTimeCollectionItem parentItem;
			if(direction == InsertDirection.Inside)
				parentItem = target;
			else
				parentItem = target != null ? target.Parent : null;
			this.defaultItemType = GetDefaultType(parentItem);
			if(designTimeItem != null) {
				IDesignTimeCollectionItem newItem = null;
				if(designTimeItem.ColumnType == typeof(DefaultItems))
					CreateDefaultToolbar(designTimeItem.ColumnType);
				else {
					newItem = CreateNewItem(designTimeItem);
					MoveItemTo(newItem, target, direction);
					SetFocusedItem(newItem, true);
				}
				return;
			}
			base.AddItemCore(designTimeItem, target, direction);
		}
		void CreateDefaultToolbar(Type type) {
			HtmlEditorContextMenuItemCollection items = new HtmlEditorContextMenuItemCollection(HtmlEditor);
			items.CreateDefaultItems();
			foreach(var item in items)
				Items.Add(item);
		}
		public override Type GetDefaultItemType() {
			return this.defaultItemType;
		}
		Type GetDefaultType(IDesignTimeCollectionItem selectedBand) {
			return typeof(HtmlEditorContextMenuItem);
		}
		protected override bool IsSelectionChangingEnabled() {
			if(SelectedItems.Count == 0)
				return false;
			return true;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			var canCreateBase = base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
			if(!canCreateBase)
				return false;
			var focusedItem = FocusedItemForAction;
			IDesignTimeColumnAndEditorItem parentDesignTimeItem;
			switch(parentMenuItem.ActionType) {
				case DesignEditorMenuRootItemActionType.AddItem:
				case DesignEditorMenuRootItemActionType.InsertBefore:
				case DesignEditorMenuRootItemActionType.ChangeTo:
				parentDesignTimeItem = focusedItem != null && focusedItem.Parent != null ? FindDesignTimeItem(focusedItem.Parent.GetType()) : null;
				return CanAssociate(designTimeItem, parentDesignTimeItem, parentMenuItem);
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
				return designTimeItem.ColumnType == typeof(DefaultItems);
			}
			return true;
		}
		protected bool CanAssociate(IDesignTimeColumnAndEditorItem child, IDesignTimeColumnAndEditorItem parent, DesignEditorDescriptorItem menuItem) {
			var parentItemType = parent == null ? null : parent.ColumnType;
			return CanAssociate(child.ColumnType, parentItemType, menuItem);
		}
		protected bool CanAssociate(Type childType, Type focusedItemType, DesignEditorDescriptorItem menuItem) {
			if(menuItem == null) {
				if(typeof(HtmlEditorContextMenuItem).IsAssignableFrom(childType))
					return true;
			}
			else {
				IDesignTimeColumnAndEditorItem menuItemType = menuItem.ItemType;
				if(menuItemType == null) {
					foreach(Type groupType in GroupItemTypes.Keys) {
						if(GroupItemTypes[groupType].Contains(childType))
							return false;
					}
					return (typeof(HtmlEditorContextMenuItem).IsAssignableFrom(childType)) || typeof(GroupBase).IsAssignableFrom(childType);
				}
				else {
					List<Type> listDesignItems = GroupItemTypes[menuItemType.ColumnType];
					if(listDesignItems != null && listDesignItems.Contains(childType))
						return true;
				}
			}
			return false;
		}
		protected virtual bool IsBeginGroup(Type type) {
			return (type == typeof(CustomItemsGroup));
		}
		protected override DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			DesignEditorDescriptorItem menuItem = new DesignEditorDescriptorItem() {
				Caption = designTimeItem.Text,
				ImageIndex = designTimeItem.ImageIndex,
				ItemType = designTimeItem,
				ParentItem = parent,
				Enabled = true
			};
			menuItem.BeginGroup = IsBeginGroup(designTimeItem.ColumnType);
			if(typeof(GroupBase).IsAssignableFrom(designTimeItem.ColumnType)) {
				menuItem.EditorType = DesignEditorDescriptorItemType.DropDown;
				menuItem.ActionType = parent.ActionType;
				PopulateChildItems(menuItem, isToolbar);
			}
			return menuItem;
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return CreateDefaultItemsItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected DesignEditorDescriptorItem CreateDefaultItemsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Create Default Items";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "ContextMenu";
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.RetriveFields,
				DesignEditorMenuRootItemActionType.CreateDefaultItems
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown, 
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
	}
	public partial class HtmlEditorContextMenuEditorFrame : ItemsEditorFrame {
		HtmlEditorContextMenuItemsOwner HtmlEditorContextMenuOwner { get { return ItemsOwner as HtmlEditorContextMenuItemsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(HtmlEditorContextMenuOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = DesignUtils.ShowMessage(HtmlEditorContextMenuOwner.HtmlEditor.Site, string.Format(string.Format("Do you want to delete the existing items?")),
					string.Format("Create default items for '{0}'", HtmlEditorContextMenuOwner.HtmlEditor.GetType().Name), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					HtmlEditorContextMenuOwner.SetSelection(HtmlEditorContextMenuOwner.TreeListItemsHash.Keys.ToList());
					HtmlEditorContextMenuOwner.RemoveSelectedItems();
				}
			}
			foreach(var designItemType in GetDefaultContextMenuItemTypes(menuItem))
				ItemsOwner.AddItem(designItemType);
		}
		protected virtual List<IDesignTimeColumnAndEditorItem> GetDefaultContextMenuItemTypes(DesignEditorDescriptorItem pressedMenuItem) {
			if(pressedMenuItem.ParentItem != null)
				return new List<IDesignTimeColumnAndEditorItem>() { pressedMenuItem.ItemType };
			return HtmlEditorContextMenuOwner.ColumnsAndEditors.Where(i => i.ColumnType == typeof(DefaultItems)).ToList();
		}
	}
}
