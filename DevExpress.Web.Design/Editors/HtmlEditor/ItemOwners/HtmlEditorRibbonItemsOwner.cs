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
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class AlignmentGroup : GroupBase { }
	public class ClipboardGroup : GroupBase { }
	public class ColorGroup : GroupBase { }
	public class DialogGroup : GroupBase { }
	public class FormattingGroup : GroupBase { }
	public class IndentGroup : GroupBase { }
	public class SubscriptGroup : GroupBase { }
	public class TableGroup : GroupBase { }
	public class InsertTableGroup : GroupBase { }
	public class DeleteTableGroup : GroupBase { }
	public class MergeTableGroup : GroupBase { }
	public class ChangeTableGroup : GroupBase { }
	public class SplitTableGroup : GroupBase { }
	public class TablePropertiesGroup : GroupBase { }
	public class EditingGroup : GroupBase { }
	public class ExportGroup : GroupBase { }
	public class OtherGroup : GroupBase { }
	public class CustomItemsGroup : GroupBase { }
	public class RibbonGroupItemsOwner : RibbonItemsOwner {
		public RibbonGroupItemsOwner(object component, IServiceProvider provider, IList items)
			: base(component, provider, items) {
		}
		public RibbonGroupItemsOwner(Collection items)
			: base(null, null, items) {
		}
		protected override bool CanAssociate(Type childType, Type focusedItemType, DesignEditorDescriptorItem menuItem) {
			if(menuItem != null && typeof(RibbonGroup).IsAssignableFrom(focusedItemType))			
				return CanAssociateGroupMenuItems(childType, focusedItemType, menuItem);
			return base.CanAssociate(childType, focusedItemType, menuItem);
		}
		protected virtual bool CanAssociateGroupMenuItems(Type childType, Type focusedItemType, DesignEditorDescriptorItem menuItem) {
			if(menuItem == null || !typeof(RibbonGroup).IsAssignableFrom(focusedItemType) || childType == typeof(ListEditItem))
				return false;
			IDesignTimeColumnAndEditorItem menuItemType = menuItem.ItemType;
			if(menuItemType == null)
				return IsGroupItemType(childType) ? false : typeof(RibbonItemBase).IsAssignableFrom(childType) || typeof(GroupBase).IsAssignableFrom(childType);
			List<Type> listDesignItems = GroupItemTypes[menuItemType.ColumnType];
			return listDesignItems != null && listDesignItems.Contains(childType);
		}
		protected virtual bool IsBeginGroup(Type type) {
			return false;
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
		protected override DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = base.CreateInsertChildMenuItem(isToolbar);
			var focusedItem = FocusedItemForAction;
			if(focusedItem != null && FindDesignTimeItem(focusedItem.GetType()) == null)
				item.Enabled = false;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return CreateDefaultItemsItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected virtual DesignEditorDescriptorItem CreateDefaultItemsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Create Default Tabs";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
	}
	public class HtmlEditorRibbonContextTabsOwner : HtmlEditorRibbonItemsOwner {
		public HtmlEditorRibbonContextTabsOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(HETableToolsRibbonTabCategory), "Table Tools", TabControlItemImageResource);
			AddItemType(typeof(RibbonContextTabCategory), "Custom Tab Category", TabControlItemImageResource);
			base.FillItemTypes();
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(RibbonContextTabCategory)) || base.IsBeginGroup(type);
		}
		protected override IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			IDesignTimeCollectionItem newItem = null;
			if(designTimeItem.ColumnType == typeof(HETableToolsRibbonTabCategory))
				newItem = CreateDefaultRibbonTabCategory(designTimeItem.ColumnType);
			else if(typeof(HERibbonTabBase).IsAssignableFrom(designTimeItem.ColumnType))
				newItem = CreateDefaultRibbonTab(designTimeItem.ColumnType, GetFocusedRibbonTabCategoryItem() as RibbonContextTabCategory);
			return newItem;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabCategoryItem() {
			return typeof(RibbonContextTabCategory).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		RibbonTab CreateDefaultRibbonTab(Type type, RibbonContextTabCategory tabCategory) {
			var tab = CreateDefaultRibbonTabCore(type);
			tabCategory.Tabs.Add(tab);
			return tab;
		}
		RibbonContextTabCategory CreateDefaultRibbonTabCategory(Type type) {
			RibbonContextTabCategory tabCategory = null;
			if(type == typeof(HETableToolsRibbonTabCategory))
				tabCategory = RibbonHelper.CreateTableToolsTabCategory();
			Items.Add(tabCategory);
			return tabCategory;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(RibbonContextTabCategory).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override bool IsContextTabCategoriesCollection() {
			return Items != null && Items.GetType() == typeof(HtmlEditorRibbonContextTabCategoryCollection);
		}
		public override void CreateDefaultItems() {
			BeginUpdate();
			(Items as HtmlEditorRibbonContextTabCategoryCollection).CreateDefaultRibbonContextTabCategories();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
		protected override DesignEditorDescriptorItem CreateDefaultItemsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Create Default Tab Categories";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
	}
	public class HtmlEditorRibbonItemsOwner : RibbonGroupItemsOwner {
		public HtmlEditorRibbonItemsOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		public HtmlEditorRibbonItemsOwner(Collection items)
			: base(null, null, items) {
		}
		public ASPxHtmlEditor HtmlEditor { get { return Component as ASPxHtmlEditor; } }
		public HtmlEditorDefaultRibbon RibbonHelper { get { return new HtmlEditorDefaultRibbon(HtmlEditor); } }
		protected override void FillItemTypes() {
			AddItemType(typeof(HEHomeRibbonTab), "Home", TabControlItemImageResource);
			AddItemType(typeof(HEInsertRibbonTab), "Insert", TabControlItemImageResource);
			AddItemType(typeof(HEViewRibbonTab), "View", TabControlItemImageResource);
			AddItemType(typeof(HEReviewRibbonTab), "Review", TabControlItemImageResource);
			AddItemType(typeof(HETableRibbonTab), "Table", TabControlItemImageResource);
			AddItemType(typeof(HETableLayoutRibbonTab), "Layout", TabControlItemImageResource);
			AddItemType(typeof(RibbonTab), "Custom Tab", TabControlItemImageResource);
			AddItemType(typeof(HEUndoRibbonGroup), "Undo Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEClipboardRibbonGroup), "Clipboard Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEFontRibbonGroup), "Font Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEParagraphRibbonGroup), "Paragraph Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEImagesRibbonGroup), "Images Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HELinksRibbonGroup), "Links Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEMediaRibbonGroup), "Media Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEViewsRibbonGroup), "Views Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HETablesRibbonGroup), "Tables Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEInsertTableRibbonGroup), "Insert Table Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEDeleteTableRibbonGroup), "Delete Table Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEMergeTableRibbonGroup), "Merge Table Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HETablePropertiesRibbonGroup), "Table Properties Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HESpellingRibbonGroup), "Spelling Group", RibbonGroupItemImageResource);
			AddItemType(typeof(HEEditingRibbonGroup), "Editing Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RibbonGroup), "Custom Group", RibbonGroupItemImageResource);
			AddItemType(typeof(AlignmentGroup), "Alignment");
			AddGroupItemType(typeof(AlignmentGroup), typeof(HEAlignmentLeftRibbonCommand), "Left", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(HEAlignmentCenterRibbonCommand), "Center", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(HEAlignmentRightRibbonCommand), "Right", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(HEAlignmentJustifyRibbonCommand), "Justify", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(ClipboardGroup), "Clipboard");
			AddGroupItemType(typeof(ClipboardGroup), typeof(HECutSelectionRibbonCommand), "Cut", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(HECopySelectionRibbonCommand), "Copy", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardGroup), typeof(HEPasteSelectionRibbonCommand), "Paste", RibbonButtonItemImageResource);
			AddItemType(typeof(ColorGroup), "Colors");
			AddGroupItemType(typeof(ColorGroup), typeof(HEBackColorRibbonCommand), "Back Color", ColorEditImageResource);
			AddGroupItemType(typeof(ColorGroup), typeof(HEFontColorRibbonCommand), "Font Color", ColorEditImageResource);
			AddItemType(typeof(DialogGroup), "Dialogs");
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertLinkDialogRibbonCommand), "Insert Link", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertImageRibbonCommand), "Insert Image", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertAudioDialogRibbonCommand), "Insert Audio", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertVideoDialogRibbonCommand), "Insert Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertFlashDialogRibbonCommand), "Insert Flash", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertYouTubeVideoDialogRibbonCommand), "Insert YouTube Video", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEInsertPlaceholderDialogRibbonCommand), "Insert Placeholder", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HECheckSpellingRibbonCommand), "Check Spelling", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEPasteFromWordRibbonCommand), "Paste From Word", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HEFindAndReplaceDialogRibbonCommand), "Find and Replace", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DialogGroup), typeof(HECustomDialogRibbonCommand), "Custom Dialog", RibbonButtonItemImageResource);
			AddItemType(typeof(FormattingGroup), "Formatting");
			AddGroupItemType(typeof(FormattingGroup), typeof(HEBoldRibbonCommand), "Bold", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEItalicRibbonCommand), "Italic", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEUnderlineRibbonCommand), "Underline", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEStrikeoutRibbonCommand), "Strikeout", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEFontNameRibbonCommand), "Font Name", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEFontSizeRibbonCommand), "Font Size", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEParagraphFormattingRibbonCommand), "Paragraph", ComboboxImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEInsertOrderedListRibbonCommand), "Insert Ordered List", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FormattingGroup), typeof(HEInsertUnorderedListRibbonCommand), "Insert Unordered Lis", RibbonButtonItemImageResource);
			AddItemType(typeof(IndentGroup), "Indent/Outdent");
			AddGroupItemType(typeof(IndentGroup), typeof(HEIndentRibbonCommand), "Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(IndentGroup), typeof(HEOutdentRibbonCommand), "Outdent", RibbonButtonItemImageResource);
			AddItemType(typeof(SubscriptGroup), "Subscript/Superscript");
			AddGroupItemType(typeof(SubscriptGroup), typeof(HESubscriptRibbonCommand), "Subscript", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(SubscriptGroup), typeof(HESuperscriptRibbonCommand), "Superscript", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(TableGroup), "Tables");
			AddGroupItemType(typeof(TableGroup), typeof(InsertTableGroup), "Insert");
			AddGroupItemType(typeof(TableGroup), typeof(DeleteTableGroup), "Delete");
			AddGroupItemType(typeof(TableGroup), typeof(MergeTableGroup), "Merge");
			AddGroupItemType(typeof(TableGroup), typeof(SplitTableGroup), "Split");
			AddGroupItemType(typeof(TableGroup), typeof(TablePropertiesGroup), "Properties");
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableRibbonCommand), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableDropDownRibbonCommand), "Table", DropDownButtonImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableByGridHighlightingRibbonCommand), "Table By Grid Highlighting", DropDownButtonImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableColumnToLeftRibbonCommand), "Column To Left", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableColumnToRightRibbonCommand), "Column To RightButton", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableRowAboveRibbonCommand), "Row Above", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertTableGroup), typeof(HEInsertTableRowBelowRibbonCommand), "Row Below", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableRibbonCommand), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableColumnRibbonCommand), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DeleteTableGroup), typeof(HEDeleteTableRowRibbonCommand), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(HEMergeTableCellRightRibbonCommand), "Cell Right", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeTableGroup), typeof(HEMergeTableCellDownRibbonCommand), "Cell Down", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETablePropertiesRibbonCommand), "Table", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableColumnPropertiesRibbonCommand), "Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableRowPropertiesRibbonCommand), "Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(HETableCellPropertiesRibbonCommand), "Cell", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(HESplitTableCellVerticallyRibbonCommand), "Cell Vertically", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(SplitTableGroup), typeof(HESplitTableCellHorizontallyRibbonCommand), "Cell Horizontally", RibbonButtonItemImageResource);
			AddItemType(typeof(EditingGroup), "Undo/Redo");
			AddGroupItemType(typeof(EditingGroup), typeof(HEUndoRibbonCommand), "Undo", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(EditingGroup), typeof(HERedoRibbonCommand), "Redo", RibbonButtonItemImageResource);
			AddItemType(typeof(ExportGroup), "Export");
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToRtfDropDownRibbonCommand), "Rtf", DropDownButtonImageResource);
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToDocxDropDownRibbonCommand), "Docx", DropDownButtonImageResource);
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToMhtDropDownRibbonCommand), "Mht", DropDownButtonImageResource);
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToOdtDropDownRibbonCommand), "Odts", DropDownButtonImageResource);
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToPdfDropDownRibbonCommand), "Pdf", DropDownButtonImageResource);
			AddGroupItemType(typeof(ExportGroup), typeof(HEExportToTxtDropDownRibbonCommand), "Txt", DropDownButtonImageResource);
			AddItemType(typeof(OtherGroup), "Other");
			AddGroupItemType(typeof(OtherGroup), typeof(HECustomCssRibbonCommand), "Custom Css", ComboboxImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HEUnlinkRibbonCommand), "Unlink", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HEPrintRibbonCommand), "Print", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HEFullscreenRibbonCommand), "Full Screen", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(OtherGroup), typeof(HERemoveFormatRibbonCommand), "Remove Format", RibbonButtonItemImageResource);
			AddItemType(typeof(CustomItemsGroup), "Custom Items");
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonButtonItem), "Button", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonCheckBoxItem), "Check Box", "Check Box", CheckColumnImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonColorButtonItem), "Color Button", ColorEditImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonDropDownButtonItem), "DropDown Button", DropDownButtonImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonOptionButtonItem), "Option Button", RibbonOptionButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonToggleButtonItem), "Toggle Button", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonComboBoxItem), "Combo Box", "Combo Box", ComboboxImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonDateEditItem), "Date Edit", "Date Edit", DateEditImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonSpinEditItem), "Spin Edit", "Spin Edit", SpinEditImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonTextBoxItem), "Text Box", "Text Box", TextImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(RibbonTemplateItem), "Template", RibbonTemplateItemImageResource);
			AddGroupItemType(typeof(CustomItemsGroup), typeof(ListEditItem), "ListEdit Item", ListEditImageResource);
			AddItemType(typeof(ToolbarCustomCssListEditItem), "ToolbarCustomCssListEditItem", ListEditImageResource);
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(CustomItemsGroup)) || (type == typeof(HEFontNameRibbonCommand)) || (type == typeof(HEInsertOrderedListRibbonCommand))
				|| (type == typeof(RibbonTab)) || (type == typeof(RibbonGroup));
		}
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(designTimeItem != null) {
				IDesignTimeCollectionItem newItem = CreateDesignTimeCollectionItem(designTimeItem);
				if(newItem == null) {
					if(typeof(HERibbonGroupBase).IsAssignableFrom(designTimeItem.ColumnType))
						newItem = CreateDefaultRibbonGroup(designTimeItem.ColumnType, (GetFocusedRibbonTabItem() as RibbonTab));
					else {
						newItem = CreateNewItem(designTimeItem);
						FillDefaultItems(newItem);
					}
				}
				if(newItem != null) {
					MoveItemTo(newItem, target, direction);
					SetFocusedItem(newItem, true);
					return;
				}
			}
			base.AddItemCore(designTimeItem, target, direction);
		}
		protected virtual IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			return typeof(HERibbonTabBase).IsAssignableFrom(designTimeItem.ColumnType) ? CreateDefaultRibbonTab(designTimeItem.ColumnType) : null;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabItem() {
			return typeof(RibbonTab).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		RibbonTab CreateDefaultRibbonTab(Type type) {
			var tab = CreateDefaultRibbonTabCore(type);
			Items.Add(tab);
			return tab;
		}
		protected RibbonTab CreateDefaultRibbonTabCore(Type type) {
			if(type == typeof(HEHomeRibbonTab))
				return RibbonHelper.CreateHomeTab();
			if(type == typeof(HEInsertRibbonTab))
				return RibbonHelper.CreateInsertTab();
			if(type == typeof(HEViewRibbonTab))
				return RibbonHelper.CreateViewTab();
			if(type == typeof(HETableLayoutRibbonTab))
				return RibbonHelper.CreateTableLayoutTab();
			if(type == typeof(HETableRibbonTab))
				return RibbonHelper.CreateTableTab();
			if(type == typeof(HEReviewRibbonTab))
				return RibbonHelper.CreateReviewTab();
			return null;
		}
		RibbonGroup CreateDefaultRibbonGroup(Type type, RibbonTab ribbonTab) {
			var group = CreateDefaultRibbonGroupCore(type);
			ribbonTab.Groups.Add(group);
			return group;
		}
		RibbonGroup CreateDefaultRibbonGroupCore(Type type) {
			if(type == typeof(HEClipboardRibbonGroup))
				return RibbonHelper.CreateClipboardGroup();
			if(type == typeof(HEFontRibbonGroup))
				return RibbonHelper.CreateFontGroup();
			if(type == typeof(HEParagraphRibbonGroup))
				return RibbonHelper.CreateParagraphGroup();
			if(type == typeof(HEUndoRibbonGroup))
				return RibbonHelper.CreateUndoGroup();
			if(type == typeof(HEImagesRibbonGroup))
				return RibbonHelper.CreateImagesGroup();
			if(type == typeof(HELinksRibbonGroup))
				return RibbonHelper.CreateLinksGroup();
			if(type == typeof(HEViewsRibbonGroup))
				return RibbonHelper.CreateViewsGroup();
			if(type == typeof(HETablesRibbonGroup))
				return RibbonHelper.CreateTablesGroup();
			if(type == typeof(HEInsertTableRibbonGroup))
				return RibbonHelper.CreateInsertTableGroup();
			if(type == typeof(HEDeleteTableRibbonGroup))
				return RibbonHelper.CreateDeleteTableGroup();
			if(type == typeof(HEMergeTableRibbonGroup))
				return RibbonHelper.CreateMergeTableGroup();
			if(type == typeof(TablePropertiesGroup))
				return RibbonHelper.CreateTablePropertiesGroup();
			if(type == typeof(HESpellingRibbonGroup))
				return RibbonHelper.CreateSpellingGroup();
			if(type == typeof(HEMediaRibbonGroup))
				return RibbonHelper.CreateMediaGroup();
			if(type == typeof(HEEditingRibbonGroup))
				return RibbonHelper.CreateEditingGroup();
			return null;
		}
		void FillDefaultItems(IDesignTimeCollectionItem item) {
			if(item == null)
				return;
			if(item.GetType() == typeof(HEFontNameRibbonCommand))
				(item as HEFontNameRibbonCommand).CreateDefaultItems(false);
			else if(item.GetType() == typeof(HEFontSizeRibbonCommand))
				(item as HEFontSizeRibbonCommand).CreateDefaultItems(false);
			else if(item.GetType() == typeof(HEParagraphFormattingRibbonCommand))
				(item as HEParagraphFormattingRibbonCommand).CreateDefaultItems(false);
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(HERibbonTabBase).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override bool CanAssociate(Type childType, Type focusedItemType, DesignEditorDescriptorItem menuItem) {
			if(focusedItemType != null && typeof(RibbonDropDownButtonItem).IsAssignableFrom(focusedItemType))
				return typeof(RibbonDropDownButtonItem).IsAssignableFrom(childType);
			if(focusedItemType == typeof(HECustomCssRibbonCommand))
				return childType == typeof(ToolbarCustomCssListEditItem);
			if(focusedItemType != null && typeof(RibbonComboBoxItem).IsAssignableFrom(focusedItemType))
				return childType == typeof(ListEditItem);
			return base.CanAssociate(childType, focusedItemType, menuItem);
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Ribbon items";
		}
		public virtual void CreateDefaultItems() {
			BeginUpdate();
			(Items as HtmlEditorRibbonTabCollection).CreateDefaultRibbonTabs();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
	}
	public class HtmlEditorRibbonContextTabsEditorFrame : HtmlEditorRibbonItemsEditorFrame {
		HtmlEditorRibbonItemsOwner HtmlEditorRibbonOwner { get { return ItemsOwner as HtmlEditorRibbonItemsOwner; } }
		protected override string FrameName { get { return "HtmlEditorRibbonContextTabsEditorFrame"; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(HtmlEditorRibbonOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = DesignUtils.ShowMessage(HtmlEditorRibbonOwner.HtmlEditor.Site, string.Format(string.Format("Do you want to delete the existing context tabs?")),
					string.Format("Create default context tabs for '{0}'", HtmlEditorRibbonOwner.HtmlEditor.GetType().Name), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					HtmlEditorRibbonOwner.SetSelection(HtmlEditorRibbonOwner.TreeListItemsHash.Keys.ToList());
					HtmlEditorRibbonOwner.RemoveSelectedItems();
				}
			}
			HtmlEditorRibbonOwner.CreateDefaultItems();
		}
	}
	public class HtmlEditorRibbonItemsEditorFrame : ItemsEditorFrame {
		HtmlEditorRibbonItemsOwner HtmlEditorRibbonOwner { get { return ItemsOwner as HtmlEditorRibbonItemsOwner; } }
		protected override string FrameName { get { return "HtmlEditorRibbonItemsEditorFrame"; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(HtmlEditorRibbonOwner.TreeListItems.Count > 0) {	 
				DialogResult dialogResult = DesignUtils.ShowMessage(HtmlEditorRibbonOwner.HtmlEditor.Site, string.Format(string.Format("Do you want to delete the existing ribbon tabs?")),
					string.Format("Create default tabs for '{0}'", HtmlEditorRibbonOwner.HtmlEditor.GetType().Name), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					HtmlEditorRibbonOwner.SetSelection(HtmlEditorRibbonOwner.TreeListItemsHash.Keys.ToList());
					HtmlEditorRibbonOwner.RemoveSelectedItems();
				}
			}
			HtmlEditorRibbonOwner.CreateDefaultItems();
		}
	}
}
