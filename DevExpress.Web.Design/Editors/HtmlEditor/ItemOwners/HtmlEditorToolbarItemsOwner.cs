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
	public class HtmlEditorToolbarEditor : TypeEditorBase {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			return new WrapperEditorForm(new HtmlEditorToolbarItemsOwner(component as ASPxHtmlEditor, provider));
		}
	}
	public class StandardToolbar1 : HtmlEditorToolbar { }
	public class StandardToolbar2 : HtmlEditorToolbar { }
	public class TableToolbar : HtmlEditorToolbar { }
	public class HtmlEditorToolbarItemsOwner : ItemsEditorOwner {
		public HtmlEditorToolbarItemsOwner(ASPxHtmlEditor htmlEditor, IServiceProvider provider)
			: base(htmlEditor, "Toolbars", provider, htmlEditor.Toolbars) {
		}
		public HtmlEditorToolbarItemsOwner(HtmlEditorToolbarCollection items)
			: base(null, null, null, items) {
		}
		public ASPxHtmlEditor HtmlEditor { get { return (ASPxHtmlEditor)Component; } }
		protected override void FillItemTypes() {
			AddItemType(typeof(StandardToolbar1), "Standard Toolbar 1", TabControlItemImageResource);
			AddItemType(typeof(StandardToolbar2), "Standard Toolbar 2", TabControlItemImageResource);
			AddItemType(typeof(TableToolbar), "Table Toolbar", TabControlItemImageResource);
			AddItemType(typeof(HtmlEditorToolbar), "Custom Toolbar", TabControlItemImageResource);
			AddItemType(typeof(AlignmentGroup), "Alignment");
			AddGroupItemType(typeof(AlignmentGroup), typeof(ToolbarJustifyLeftButton), "Left", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(ToolbarJustifyCenterButton), "Center", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(ToolbarJustifyRightButton), "Right", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(ToolbarJustifyFullButton), "Justify", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(ClipboardGroup), "Clipboard");
			AddGroupItemType(typeof(ClipboardGroup), typeof(ToolbarCutButton), "Cut", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(ToolbarCopyButton), "Copy", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(ToolbarPasteButton), "Paste", RibbonButtonItemImageResource);
			AddItemType(typeof(ColorGroup), "Colors");
			AddGroupItemType(typeof(ColorGroup), typeof(ToolbarBackColorButton), "Back Color", ColorEditImageResource);
			AddGroupItemType(typeof(ColorGroup), typeof(ToolbarFontColorButton), "Font Color", ColorEditImageResource);
			AddItemType(typeof(DialogGroup), "Dialogs");
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertLinkDialogButton), "Insert Link", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertImageDialogButton), "Insert Image", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertAudioDialogButton), "Insert Audio", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertVideoDialogButton), "Insert Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertFlashDialogButton), "Insert Flash", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertYouTubeVideoDialogButton), "Insert YouTube Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarInsertPlaceholderDialogButton), "Insert Placeholder", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarCheckSpellingButton), "Check Spelling", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarPasteFromWordButton), "Paste From Word", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(ToolbarCustomDialogButton), "Custom Dialog", RibbonButtonItemImageResource);
			AddItemType(typeof(FormattingGroup), "Formatting");
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarBoldButton), "Bold", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarItalicButton), "Italic", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarUnderlineButton), "Underline", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarStrikethroughButton), "Strikeout", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarFontNameEdit), "Font Name", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarFontSizeEdit), "Font Size", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarParagraphFormattingEdit), "Paragraph", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarInsertOrderedListButton), "Insert Ordered List", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(ToolbarInsertUnorderedListButton), "Insert Unordered Lis", RibbonButtonItemImageResource);
			AddItemType(typeof(IndentGroup), "Indent/Outdent");
			AddGroupItemType(typeof(IndentGroup), typeof(ToolbarIndentButton), "Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(IndentGroup), typeof(ToolbarOutdentButton), "Outdent", RibbonButtonItemImageResource);
			AddItemType(typeof(SubscriptGroup), "Subscript/Superscript");
			AddGroupItemType(typeof(SubscriptGroup), typeof(ToolbarSubscriptButton), "Subscript", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(SubscriptGroup), typeof(ToolbarSuperscriptButton), "Superscript", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(TableGroup), "Tables");
			AddGroupItemType(typeof(TableGroup), typeof(InsertTableGroup), "Insert");
			AddGroupItemType(typeof(TableGroup), typeof(DeleteTableGroup), "Delete");
			AddGroupItemType(typeof(TableGroup), typeof(MergeTableGroup), "Merge");
			AddGroupItemType(typeof(TableGroup), typeof(SplitTableGroup), "Split");
			AddGroupItemType(typeof(TableGroup), typeof(TablePropertiesGroup), "Properties");
			AddGroupItemType(typeof(TableGroup), typeof(ToolbarTableOperationsDropDownButton), "Table Operations", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(ToolbarInsertTableDialogButton), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(ToolbarInsertTableColumnToLeftButton), "Column To Left", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(ToolbarInsertTableColumnToRightButton), "Column To RightButton", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(ToolbarInsertTableRowAboveButton), "Row Above", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(ToolbarInsertTableRowBelowButton), "Row Below", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(ToolbarDeleteTableButton), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(ToolbarDeleteTableColumnButton), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(ToolbarDeleteTableRowButton), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(ToolbarMergeTableCellRightButton), "Cell Right", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(ToolbarMergeTableCellDownButton), "Cell Down", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(ToolbarTableCellPropertiesDialogButton), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(ToolbarTableColumnPropertiesDialogButton), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(ToolbarTablePropertiesDialogButton), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(ToolbarTableRowPropertiesDialogButton), "Cell", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(ToolbarSplitTableCellVerticallyButton), "Cell Vertically", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(ToolbarSplitTableCellHorizontallyButton), "Cell Horizontally", RibbonButtonItemImageResource);
			AddItemType(typeof(EditingGroup), "Undo/Redo");
			AddGroupItemType(typeof(EditingGroup), typeof(ToolbarUndoButton), "Undo", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(EditingGroup), typeof(ToolbarRedoButton), "Redo", RibbonButtonItemImageResource);
			AddItemType(typeof(OtherGroup), "Other");
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarCustomCssEdit), "Custom Css", ComboboxImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarUnlinkButton), "Unlink", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarExportDropDownButton), "Export", DropDownButtonImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarPrintButton), "Print", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarFullscreenButton), "Full Screen", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(ToolbarRemoveFormatButton), "Remove Format", RibbonButtonItemImageResource);
			AddItemType(typeof(ToolbarCustomCssListEditItem), "ListEdit Item", ListEditImageResource);
			AddItemType(typeof(ToolbarExportDropDownItem), "Export Item", RibbonButtonItemImageResource);
			AddItemType(typeof(CustomItemsGroup), "Custom Items");
			AddGroupItemType(typeof(CustomItemsGroup), typeof(CustomToolbarButton), "Button", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(ToolbarComboBox), "Combo Box", "Combo Box", ComboboxImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(ToolbarDropDownItemPicker), "DropDown Item Picker", DropDownButtonImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(ToolbarDropDownMenu), "DropDown Menu", DropDownButtonImageResource);
			AddItemType(typeof(ToolbarMenuItem), "Menu Item", RibbonButtonItemImageResource);
			AddItemType(typeof(ToolbarListEditItem), "ListEdit Item", ListEditImageResource);
			AddItemType(typeof(ToolbarCustomListEditItem), "Custom ListEdit Item", ListEditImageResource);
			AddItemType(typeof(ToolbarItemPickerItem), "Item Picker", ListEditImageResource);			
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
				if(designTimeItem.ColumnType == typeof(StandardToolbar1) || designTimeItem.ColumnType == typeof(StandardToolbar2) || designTimeItem.ColumnType == typeof(TableToolbar))
					newItem = CreateDefaultToolbar(designTimeItem.ColumnType);
				else {
					newItem = CreateNewItem(designTimeItem);
					FillDefaultItems(newItem);
				}
				if(newItem != null) {
					MoveItemTo(newItem, target, direction);
					SetFocusedItem(newItem, true);
					return;
				}
			}
			base.AddItemCore(designTimeItem, target, direction);
		}
		HtmlEditorToolbar CreateDefaultToolbar(Type type) {
			var tab = CreateDefaultToolbarCore(type);
			Items.Add(tab);
			return tab;
		}
		HtmlEditorToolbar CreateDefaultToolbarCore(Type type) {
			if(type == typeof(StandardToolbar1))
				return HtmlEditorToolbar.CreateStandardToolbar1();
			if(type == typeof(StandardToolbar2))
				return HtmlEditorToolbar.CreateStandardToolbar2(HtmlEditor.IsRightToLeft());
			if(type == typeof(TableToolbar))
				return HtmlEditorToolbar.CreateTableToolbar();
			return null;
		}
		void FillDefaultItems(IDesignTimeCollectionItem item) {
			if(item == null)
				return;
			if(item.GetType() == typeof(ToolbarTableOperationsDropDownButton))
				(item as ToolbarTableOperationsDropDownButton).CreateDefaultItems();
			else if(item.GetType() == typeof(ToolbarFontNameEdit))
				(item as ToolbarFontNameEdit).CreateDefaultItems();
			else if(item.GetType() == typeof(ToolbarFontSizeEdit))
				(item as ToolbarFontSizeEdit).CreateDefaultItems();
			else if(item.GetType() == typeof(ToolbarParagraphFormattingEdit))
				(item as ToolbarParagraphFormattingEdit).CreateDefaultItems();
		}
		public override Type GetDefaultItemType() {
			return this.defaultItemType;
		}
		Type GetDefaultType(IDesignTimeCollectionItem selectedBand) {
			if(selectedBand == null)
				return typeof(HtmlEditorToolbar);
			var type = selectedBand.GetType();
			if(typeof(HtmlEditorToolbar).IsAssignableFrom(type))
				return typeof(CustomToolbarButton);
			if(type == typeof(ToolbarCustomCssEdit))
				return typeof(ToolbarCustomCssListEditItem);
			if(type == typeof(ToolbarComboBox))
				return typeof(ToolbarCustomListEditItem);
			if(typeof(ToolbarComboBoxBase).IsAssignableFrom(type))
				return typeof(ToolbarListEditItem);
			if(type == typeof(ToolbarExportDropDownButton))
				return typeof(ToolbarExportDropDownItem);
			if(type == typeof(ToolbarDropDownItemPicker))
				return typeof(ToolbarItemPickerItem);
			if(type == typeof(ToolbarTableOperationsDropDownButton))
				return typeof(ToolbarInsertTableDialogButton);
			if(type == typeof(ToolbarDropDownMenu))
				return typeof(ToolbarMenuItem);
			return null;
		}
		protected override bool IsSelectionChangingEnabled() {
			if(SelectedItems.Count == 0)
				return false;
			return SelectedItems.All(i => i.Parent != null && typeof(HtmlEditorToolbar).IsAssignableFrom(i.Parent.GetType()));
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
				parentDesignTimeItem = focusedItem != null && focusedItem.Parent != null ? FindDesignTimeItem(focusedItem.Parent.GetType()) : null;
				return CanAssociate(designTimeItem, parentDesignTimeItem, parentMenuItem);
				case DesignEditorMenuRootItemActionType.InsertChild:
				parentDesignTimeItem = focusedItem != null ? FindDesignTimeItem(focusedItem.GetType()) : null;
				return CanAssociate(designTimeItem, parentDesignTimeItem, parentMenuItem);
				case DesignEditorMenuRootItemActionType.ChangeTo:
				return CanAssociate(designTimeItem, FindDesignTimeItem(typeof(HtmlEditorToolbar)), parentMenuItem);
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
				return typeof(HtmlEditorToolbar).IsAssignableFrom(designTimeItem.ColumnType);
			}
			return true;
		}
		public override bool CanMoveItem(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {			
			if(!base.CanMoveItem(source, target, direction))
				return false;
			if(direction == InsertDirection.Inside)
				return CanAssociate(source, target);
			var targetParent = target.Parent != null ? target.Parent : null;
			return CanAssociate(source, targetParent);
		}
		protected bool CanAssociate(IDesignTimeCollectionItem child, IDesignTimeCollectionItem parent) {
			var childType = child.GetType();
			if(parent == null)
				return typeof(HtmlEditorToolbar).IsAssignableFrom(childType);			
			var parentType = parent.GetType();
			if(typeof(HtmlEditorToolbar).IsAssignableFrom(parentType) || parentType == typeof(ToolbarTableOperationsDropDownButton))
				return typeof(HtmlEditorToolbarItem).IsAssignableFrom(childType);
			return childType == GetDefaultType(parent);
		}
		protected bool CanAssociate(IDesignTimeColumnAndEditorItem child, IDesignTimeColumnAndEditorItem parent, DesignEditorDescriptorItem menuItem) {
			var parentItemType = parent == null ? null : parent.ColumnType;
			return CanAssociate(child.ColumnType, parentItemType, menuItem);
		}
		protected bool CanAssociate(Type childType, Type focusedItemType, DesignEditorDescriptorItem menuItem) {
			if(focusedItemType == null)
				return typeof(HtmlEditorToolbar).IsAssignableFrom(childType);
			if(menuItem == null) {
				if(typeof(HtmlEditorToolbarItem).IsAssignableFrom(childType))
					return true;
			}
			else {
				IDesignTimeColumnAndEditorItem menuItemType = menuItem.ItemType;
				if(typeof(HtmlEditorToolbar).IsAssignableFrom(focusedItemType) || focusedItemType == typeof(ToolbarTableOperationsDropDownButton)) {
					if(menuItemType == null) {
						foreach(Type groupType in GroupItemTypes.Keys) {
							if(GroupItemTypes[groupType].Contains(childType))
								return false;
						}
						return (typeof(HtmlEditorToolbarItem).IsAssignableFrom(childType) && childType != typeof(ListEditItem)) || typeof(GroupBase).IsAssignableFrom(childType);
					}
					else {
						List<Type> listDesignItems = GroupItemTypes[menuItemType.ColumnType];
						if(listDesignItems != null && listDesignItems.Contains(childType))
							return childType != typeof(ListEditItem);
					}
				}
			}
			if(focusedItemType == typeof(ToolbarCustomCssEdit))
				return childType == typeof(ToolbarCustomCssListEditItem);
			if(focusedItemType == typeof(ToolbarComboBox))
				return childType == typeof(ToolbarCustomListEditItem);
			if(typeof(ToolbarComboBoxBase).IsAssignableFrom(focusedItemType))
				return childType == typeof(ToolbarListEditItem);
			if(focusedItemType == typeof(ToolbarExportDropDownButton))
				return childType == typeof(ToolbarExportDropDownItem);
			if(focusedItemType == typeof(ToolbarDropDownItemPicker))
				return childType == typeof(ToolbarItemPickerItem);
			if(focusedItemType == typeof(ToolbarDropDownMenu))
				return childType == typeof(ToolbarMenuItem);
			return false;
		}
		protected virtual bool IsBeginGroup(Type type) {
			return (type == typeof(CustomItemsGroup)) || (type == typeof(ToolbarFontNameEdit)) || (type == typeof(ToolbarInsertOrderedListButton))
				|| (type == typeof(HtmlEditorToolbar)) || (type == typeof(ToolbarTableOperationsDropDownButton));
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
			item.Caption = "Create Default Toolbars";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Toolbars";
		}
	}
	public partial class HtmlEditorToolbarsEditorForm : ItemsEditorFrame {
		HtmlEditorToolbarItemsOwner HtmlEditorToolbarOwner { get { return ItemsOwner as HtmlEditorToolbarItemsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(HtmlEditorToolbarOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = DesignUtils.ShowMessage(HtmlEditorToolbarOwner.HtmlEditor.Site, string.Format(string.Format("Do you want to delete the existing toolbars?")),
					string.Format("Create default toolbars for '{0}'", HtmlEditorToolbarOwner.HtmlEditor.GetType().Name), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					HtmlEditorToolbarOwner.SetSelection(HtmlEditorToolbarOwner.TreeListItemsHash.Keys.ToList());
					HtmlEditorToolbarOwner.RemoveSelectedItems();
				}
			}
			foreach(var designItemType in GetDefaultToolbarTypes(menuItem)) {
				ItemsOwner.AddItem(designItemType);
				TreeListItems.FindNodeByKeyID(ItemsOwner.FocusedNodeID).ExpandAll();
			}
		}
		protected virtual List<IDesignTimeColumnAndEditorItem> GetDefaultToolbarTypes(DesignEditorDescriptorItem pressedMenuItem) {
			if(pressedMenuItem.ParentItem != null)
				return new List<IDesignTimeColumnAndEditorItem>() { pressedMenuItem.ItemType };
			return HtmlEditorToolbarOwner.ColumnsAndEditors.Where(i => i.ColumnType == typeof(StandardToolbar1) || i.ColumnType == typeof(StandardToolbar2)).ToList();
		}
	}
}
