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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Web.ASPxHtmlEditor.Design;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxRichEdit.Design {
	public class FileCommonRibbonGroup : GroupBase { };
	public class UndoRibbonGroup : GroupBase { };
	public class ClipboardRibbonGroup : GroupBase { };
	public class FontRibbonGroup : GroupBase { };
	public class ParagraphRibbonGroup : GroupBase { };
	public class EditingRibbonGroup : GroupBase { };
	public class StylesRibbonGroup : GroupBase { };
	public class PagesRibbonGroup : GroupBase { };
	public class TablesRibbonGroup : GroupBase { };
	public class IllustrationsRibbonGroup : GroupBase { };
	public class LinksRibbonGroup : GroupBase { };
	public class HeaderAndFooterRibbonGroup : GroupBase { };
	public class SymbolsRibbonGroup : GroupBase { };
	public class PageSetupRibbonGroup : GroupBase { };
	public class BackgroundRibbonGroup : GroupBase { };
	public class InsertFieldsRibbonGroup : GroupBase { };
	public class MailMergeViewRibbonGroup : GroupBase { };
	public class MailMergeCurrentRecordRibbonGroup : GroupBase { };
	public class FinishMailMergeRibbonGroup : GroupBase { };
	public class ShowRibbonGroup : GroupBase { };
	public class ViewRibbonGroup : GroupBase { };
	public class CustomItemsRibbonGroup : GroupBase { };
	public class TableStyleOptionsGroup : GroupBase { };
	public class TableStylesGroup : GroupBase { };
	public class BordersAndShadingsGroup : GroupBase { };
	public class TableGroup : GroupBase { };
	public class RowAndColumnsGroup : GroupBase { };
	public class MergeGroup : GroupBase { };
	public class AlignmentGroup : GroupBase { };
	public class HeaderFooterNavigationGroup : GroupBase { };
	public class HeaderFooterOptionsGroup : GroupBase { };
	public class HeaderFooterCloseGroup : GroupBase { };
	public class RichEditRibbonItemsOwner : RibbonGroupItemsOwner {
		public RichEditRibbonItemsOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		public RichEditRibbonItemsOwner(Collection items)
			: base(null, null, items) {
		}
		public ASPxRichEdit RichEdit { get { return Component as ASPxRichEdit; } }
		public RichEditDefaultRibbon RibbonHelper { get { return new RichEditDefaultRibbon(RichEdit); } }
		protected override void FillItemTypes() {
			#region RibbonTabs
			AddItemType(typeof(RERFileTab), "File", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RERHomeTab), "Home", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RERInsertTab), "Insert", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RERPageLayoutTab), "Page Layout", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RERMailMergeTab), "Mail Merge", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RERViewTab), "View", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RibbonTab), "Custom Tab", string.Empty, TabControlItemImageResource);
			#endregion
			#region RibbonGroups
			AddItemType(typeof(RERFileCommonGroup), "File Common Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERUndoGroup), "Undo Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERClipboardGroup), "Clipboard Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERFontGroup), "Font Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERParagraphGroup), "Paragraph Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERStylesGroup), "Styles Group", RibbonGroupItemImageResource);
			AddItemType(typeof(REREditingGroup), "Editing Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERPagesGroup), "Pages Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERTablesGroup), "Tables Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERIllustrationsGroup), "Illustrations Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERLinksGroup), "Links Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERHeaderAndFooterGroup), "Header and Footer Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERSymbolsGroup), "Symbols Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERPageSetupGroup), "Page Setups Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERBackgroundGroup), "Background Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERInsertFieldsGroup), "Insert Fields Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERMailMergeViewGroup), "Mail Merge View Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERCurrentRecordGroup), "Current Record Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERFinishMailMergeGroup), "Finish Mail Merge Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERShowGroup), "Shows Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RERViewGroup), "View Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RibbonGroup), "Custom Group", RibbonGroupItemImageResource);
			#endregion
			#region RibbonItems
			AddItemType(typeof(FileCommonRibbonGroup), "File Common");
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(RERNewCommand), "New", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(REROpenCommand), "Open", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(RERSaveCommand), "Save", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(RERSaveAsCommand), "Save As", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(RERPrintCommand), "Print", RibbonButtonItemImageResource);
			AddItemType(typeof(UndoRibbonGroup), "Undo");
			AddGroupItemType(typeof(UndoRibbonGroup), typeof(RERUndoCommand), "Undo", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(UndoRibbonGroup), typeof(RERRedoCommand), "Redo", RibbonButtonItemImageResource);
			AddItemType(typeof(ClipboardRibbonGroup), "Clipboard");
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(RERPasteCommand), "Paste", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(RERCutCommand), "Cut", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(RERCopyCommand), "Copy", RibbonButtonItemImageResource);
			AddItemType(typeof(FontRibbonGroup), "Font");
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontNameCommand), "Font Name", ComboboxImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontSizeCommand), "Font Size", ComboboxImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERIncreaseFontSizeCommand), "Increase Font Size", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERDecreaseFontSizeCommand), "Decrease Font Size", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERChangeCaseCommand), "Change Case", DropDownButtonImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontBoldCommand), "Bold", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontItalicCommand), "Italic", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontUnderlineCommand), "Underline", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontStrikeoutCommand), "Strikeout", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontSuperscriptCommand), "Superscript", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontSubscriptCommand), "Subscript", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontColorCommand), "Font Color", ColorEditImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERFontBackColorCommand), "Font Back Color", ColorEditImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(RERClearFormattingCommand), "Clear Formatting", RibbonButtonItemImageResource);
			AddItemType(typeof(ParagraphRibbonGroup), "Paragraph");
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERBulletedListCommand), "Bulleted List", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERNumberingListCommand), "Numbering List", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERMultilevelListCommand), "Multilevel List", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERDecreaseIndentCommand), "Decrease Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERIncreaseIndentCommand), "Increase Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERShowWhitespaceCommand), "Show Whitespace", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERAlignLeftCommand), "Align Left", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERAlignCenterCommand), "Align Center", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERAlignRightCommand), "Align Right", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERAlignJustifyCommand), "Align Justify", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERParagraphLineSpacingCommand), "Paragraph Line Spacing", DropDownButtonImageResource);
			AddGroupItemType(typeof(ParagraphRibbonGroup), typeof(RERParagraphBackColorCommand), "Paragraph Back Color", ColorEditImageResource);
			AddItemType(typeof(StylesRibbonGroup), "Styles");
			AddGroupItemType(typeof(StylesRibbonGroup), typeof(RERChangeStyleCommand), "Change Style", RibbonGalleryBarResource);
			AddItemType(typeof(EditingRibbonGroup), "Editing");
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(RERSelectAllCommand), "Select All", RibbonButtonItemImageResource);
			AddItemType(typeof(PagesRibbonGroup), "Pages");
			AddGroupItemType(typeof(PagesRibbonGroup), typeof(RERInsertPageBreakCommand), "Page Break", RibbonButtonItemImageResource);
			AddItemType(typeof(TablesRibbonGroup), "Tables");
			AddGroupItemType(typeof(TablesRibbonGroup), typeof(RERInsertTableCommand), "Insert Table", DropDownButtonImageResource);
			AddItemType(typeof(IllustrationsRibbonGroup), "Illustrations");
			AddGroupItemType(typeof(IllustrationsRibbonGroup), typeof(RERInsertPictureCommand), "Insert Picture", RibbonButtonItemImageResource);
			AddItemType(typeof(LinksRibbonGroup), "Links");
			AddGroupItemType(typeof(LinksRibbonGroup), typeof(RERShowBookmarksFormCommand), "Show Bookmark Form", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(LinksRibbonGroup), typeof(RERShowHyperlinkFormCommand), "Show Hyperlink Form", RibbonButtonItemImageResource);
			AddItemType(typeof(HeaderAndFooterRibbonGroup), "Header and Footer");
			AddGroupItemType(typeof(HeaderAndFooterRibbonGroup), typeof(REREditPageHeaderCommand), "Edit Page Header", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(HeaderAndFooterRibbonGroup), typeof(REREditPageFooterCommand), "Edit Page Footer", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(HeaderAndFooterRibbonGroup), typeof(RERInsertPageNumberFieldCommand), "Insert Page Number Field", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(HeaderAndFooterRibbonGroup), typeof(RERInsertPageCountFieldCommand), "Insert Page Count Field", RibbonButtonItemImageResource);
			AddItemType(typeof(SymbolsRibbonGroup), "Symbols");
			AddGroupItemType(typeof(SymbolsRibbonGroup), typeof(RERShowSymbolFormCommand), "Show Symbol Form", RibbonButtonItemImageResource);
			AddItemType(typeof(PageSetupRibbonGroup), "Page Setup");
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(RERPageMarginsCommand), "Page Margins", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(RERChangeSectionPageOrientationCommand), "Page Orientation", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(RERChangeSectionPaperKindCommand), "Paper Kind", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(RERSetSectionColumnsCommand), "Columns", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(RERInsertBreakCommand), "Break", DropDownButtonImageResource);
			AddItemType(typeof(BackgroundRibbonGroup), "Background");
			AddGroupItemType(typeof(BackgroundRibbonGroup), typeof(RERChangePageColorCommand), "Change Page Color", ColorEditImageResource);
			AddItemType(typeof(InsertFieldsRibbonGroup), "Insert Fields");
			AddGroupItemType(typeof(InsertFieldsRibbonGroup), typeof(RERCreateFieldCommand), "Create Field", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(InsertFieldsRibbonGroup), typeof(RERInsertMergeFieldCommand), "Insert Merge Field", RibbonButtonItemImageResource);
			AddItemType(typeof(MailMergeViewRibbonGroup), "Mail Merge View");
			AddGroupItemType(typeof(MailMergeViewRibbonGroup), typeof(RERToggleViewMergedDataCommand), "View Merged Data", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeViewRibbonGroup), typeof(RERToggleShowAllFieldCodesCommand), "Show All Field Codes", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeViewRibbonGroup), typeof(RERToggleShowAllFieldResultsCommand), "Show All Field Results", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeViewRibbonGroup), typeof(RERUpdateAllFieldsCommand), "Update All Fields", RibbonButtonItemImageResource);
			AddItemType(typeof(MailMergeCurrentRecordRibbonGroup), "Mail Merge Current Record");
			AddGroupItemType(typeof(MailMergeCurrentRecordRibbonGroup), typeof(RERFirstDataRecordCommand), "First Data Record", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeCurrentRecordRibbonGroup), typeof(RERPreviousDataRecordCommand), "Previous Data Record", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeCurrentRecordRibbonGroup), typeof(RERNextDataRecordCommand), "Next Data Record", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MailMergeCurrentRecordRibbonGroup), typeof(RERLastDataRecordCommand), "Last Data Record", RibbonButtonItemImageResource);
			AddItemType(typeof(FinishMailMergeRibbonGroup), "Finish Mail Merge");
			AddGroupItemType(typeof(FinishMailMergeRibbonGroup), typeof(RERFinishAndMergeCommand), "Finish And Merge", RibbonButtonItemImageResource);
			AddItemType(typeof(ShowRibbonGroup), "Show");
			AddGroupItemType(typeof(ShowRibbonGroup), typeof(RERToggleShowHorizontalRulerCommand), "Show Horizontal Ruler", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(ViewRibbonGroup), "View");
			AddGroupItemType(typeof(ViewRibbonGroup), typeof(RERToggleFullScreenCommand), "FullScreen", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(CustomItemsRibbonGroup), "Custom Items");
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonButtonItem), "Button", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonCheckBoxItem), "Check Box", "Check Box", CheckColumnImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonColorButtonItem), "Color Button", ColorEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonComboBoxItem), "Combo Box", "Combo Box", ComboboxImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonDateEditItem), "Date Edit", "Date Edit", DateEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonDropDownButtonItem), "DropDown Button", DropDownButtonImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonDropDownToggleButtonItem), "DropDown Toggle Button", DropDownToggleButtonImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonGalleryBarItem), "Gallery Bar", RibbonGalleryBarResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonGalleryDropDownItem), "Gallery DropDown", RibbonGalleryDropDownResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonGalleryGroup), "Gallery Group", RibbonGalleryGroupResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonGalleryItem), "Gallery Item", RibbonGalleryItemResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(ListEditItem), "ListEdit Item", ListEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonOptionButtonItem), "Option Button", RibbonOptionButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonSpinEditItem), "Spin Edit", "Spin Edit", SpinEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonTemplateItem), "Template", RibbonTemplateItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonTextBoxItem), "Text Box", "Text Box", TextImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonToggleButtonItem), "Toggle Button", RibbonToggleButtonItemImageResource);
			#endregion
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(CustomItemsRibbonGroup)) || (type == typeof(RibbonTab)) || (type == typeof(RibbonGroup));
		}
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(designTimeItem != null) {
				IDesignTimeCollectionItem newItem = CreateDesignTimeCollectionItem(designTimeItem);
				if(newItem != null) {
					MoveItemTo(newItem, target, direction);
					SetFocusedItem(newItem, true);
					return;
				}
			}
			base.AddItemCore(designTimeItem, target, direction);
		}
		protected virtual IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			IDesignTimeCollectionItem newItem = null;
			if(typeof(RERTab).IsAssignableFrom(designTimeItem.ColumnType))
				newItem = CreateDefaultRibbonTab(designTimeItem.ColumnType);
			else if(typeof(RERGroup).IsAssignableFrom(designTimeItem.ColumnType))
				newItem = CreateDefaultRibbonGroup(designTimeItem.ColumnType, (GetFocusedRibbonTabItem() as RibbonTab));
			else {
				newItem = CreateNewItem(designTimeItem);
				FillDefaultItems(newItem);
			}
			return newItem;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabItem() {
			return typeof(RibbonTab).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		protected virtual RibbonTab CreateDefaultRibbonTab(Type type) {
			var tab = CreateDefaultRibbonTabCore(type);
			Items.Add(tab);
			return tab;
		}
		RibbonTab CreateDefaultRibbonTabCore(Type type) {
			if(type == typeof(RERFileTab))
				return RibbonHelper.CreateFileTab();
			if(type == typeof(RERHomeTab))
				return RibbonHelper.CreateHomeTab();
			if(type == typeof(RERInsertTab))
				return RibbonHelper.CreateInsertTab();
			if(type == typeof(RERPageLayoutTab))
				return RibbonHelper.CreatePageLayoutTab();
			if(type == typeof(RERMailMergeTab))
				return RibbonHelper.CreateMailMergeTab();
			if(type == typeof(RERViewTab))
				return RibbonHelper.CreateViewTab();
			return null;
		}
		RibbonGroup CreateDefaultRibbonGroup(Type type, RibbonTab ribbonTab) {
			var group = CreateDefaultRibbonGroupCore(type);
			ribbonTab.Groups.Add(group);
			return group;
		}
		RibbonGroup CreateDefaultRibbonGroupCore(Type type) {
			if(type == typeof(RERFileCommonGroup))
				return RibbonHelper.CreateFileCommonGroup();
			if(type == typeof(RERUndoGroup))
				return RibbonHelper.CreateUndoGroup();
			if(type == typeof(RERClipboardGroup))
				return RibbonHelper.CreateClipboardGroup();
			if(type == typeof(RERFontGroup))
				return RibbonHelper.CreateFontGroup();
			if(type == typeof(RERParagraphGroup))
				return RibbonHelper.CreateParagraphGroup();
			if(type == typeof(RERStylesGroup))
				return RibbonHelper.CreateStylesGroup();
			if(type == typeof(REREditingGroup))
				return RibbonHelper.CreateEditingGroup();
			if(type == typeof(RERPagesGroup))
				return RibbonHelper.CreatePagesGroup();
			if(type == typeof(RERTablesGroup))
				return RibbonHelper.CreateTablesGroup();
			if(type == typeof(RERIllustrationsGroup))
				return RibbonHelper.CreateIllustrationsGroup();
			if(type == typeof(RERLinksGroup))
				return RibbonHelper.CreateLinksGroup();
			if(type == typeof(RERHeaderAndFooterGroup))
				return RibbonHelper.CreateHeaderAndFooterGroup();
			if(type == typeof(RERSymbolsGroup))
				return RibbonHelper.CreateSymbolsGroup();
			if(type == typeof(RERPageSetupGroup))
				return RibbonHelper.CreatePageSetupGroup();
			if(type == typeof(RERBackgroundGroup))
				return RibbonHelper.CreateBackgroundGroup();
			if(type == typeof(RERInsertFieldsGroup))
				return RibbonHelper.CreateInsertFieldsGroup();
			if(type == typeof(RERMailMergeViewGroup))
				return RibbonHelper.CreateMailMergeViewGroup();
			if(type == typeof(RERCurrentRecordGroup))
				return RibbonHelper.CreateCurrentRecordGroup();
			if(type == typeof(RERFinishMailMergeGroup))
				return RibbonHelper.CreateFinishMailMergeGroup();
			if(type == typeof(RERShowGroup))
				return RibbonHelper.CreateShowGroup();
			if(type == typeof(RERViewGroup))
				return RibbonHelper.CreateViewGroup();
			return null;
		}
		void FillDefaultItems(IDesignTimeCollectionItem item) {
			if(item == null)
				return;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(RERTab).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Ribbon items";
		}
	}
	public class RichEditRibbonContextTabItemsOwner : RichEditRibbonItemsOwner {
		public RichEditRibbonContextTabItemsOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		public RichEditRibbonContextTabItemsOwner(Collection items)
			: base(null, null, items) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(RERTableToolsContextTabCategory), "Table Tools", TabControlItemImageResource);
			AddItemType(typeof(RERHeaderAndFooterToolsContextTabCategory), "Header And Footer Tools", TabControlItemImageResource);
			AddItemType(typeof(RERTableDesignTab), "Design", TabControlItemImageResource);
			AddItemType(typeof(RERTableLayoutTab), "Layout", TabControlItemImageResource);
			AddItemType(typeof(RERHeaderAndFooterTab), "Header And Footer", TabControlItemImageResource);
			AddItemType(typeof(RERTableStyleOptionsGroup), "Table Style Options", RibbonGroupItemImageResource);
			AddItemType(typeof(RERTableStylesGroup), "Table Styles", RibbonGroupItemImageResource);
			AddItemType(typeof(RERBordersAndShadingsGroup), "Borders And Shadings", RibbonGroupItemImageResource);
			AddItemType(typeof(RERTableGroup), "Table", RibbonGroupItemImageResource);
			AddItemType(typeof(RERRowAndColumnsGroup), "Rows And Columns", RibbonGroupItemImageResource);
			AddItemType(typeof(RERMergeGroup), "Merge", RibbonGroupItemImageResource);
			AddItemType(typeof(RERAlignmentGroup), "Alignment", RibbonGroupItemImageResource);
			AddItemType(typeof(RERHeaderFooterNavigationGroup), "Navigation", RibbonGroupItemImageResource);
			AddItemType(typeof(RERHeaderFooterOptionsGroup), "Options", RibbonGroupItemImageResource);
			AddItemType(typeof(RERHeaderFooterCloseGroup), "Close", RibbonGroupItemImageResource);
			AddItemType(typeof(TableStyleOptionsGroup), "Table Style Options");
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleFirstRowCommand), "Header Row", CheckColumnImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleLastRowCommand), "Total Row", CheckColumnImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleBandedRowsCommand), "Banded Rows", CheckColumnImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleFirstColumnCommand), "First Column", CheckColumnImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleLastColumnCommand), "Last Column", CheckColumnImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(RERToggleBandedColumnCommand), "Banded Column", CheckColumnImageResource);
			AddItemType(typeof(TableStylesGroup), "Table Styles");
			AddGroupItemType(typeof(TableStylesGroup), typeof(RERChangeTableStyleCommand), "Table Styles", RibbonGalleryBarResource);
			AddItemType(typeof(BordersAndShadingsGroup), "Borders And Shadings");
			AddGroupItemType(typeof(BordersAndShadingsGroup), typeof(RERChangeCurrentBorderRepositoryItemLineStyleCommand), "Current Border Style", ComboboxImageResource);
			AddGroupItemType(typeof(BordersAndShadingsGroup), typeof(RERChangeCurrentBorderRepositoryItemLineThicknessCommand), "Current Border Thickness", ComboboxImageResource);
			AddGroupItemType(typeof(BordersAndShadingsGroup), typeof(RERChangeCurrentBorderRepositoryItemColorCommand), "Current Border Color", ColorEditImageResource);
			AddGroupItemType(typeof(BordersAndShadingsGroup), typeof(RERChangeTableBordersCommand), "Change Table Borders", DropDownButtonImageResource);
			AddItemType(typeof(TableGroup), "Table");
			AddGroupItemType(typeof(TableGroup), typeof(RERSelectTableElementsCommand), "Select Table Elements", DropDownButtonImageResource);
			AddGroupItemType(typeof(TableGroup), typeof(RERShowTablePropertiesFormCommand), "Table Properties", RibbonButtonItemImageResource);
			AddItemType(typeof(RowAndColumnsGroup), "Rows And Columns");
			AddGroupItemType(typeof(RowAndColumnsGroup), typeof(RERDeleteTableElementsCommand), "Delete Table Elements", DropDownButtonImageResource);
			AddGroupItemType(typeof(RowAndColumnsGroup), typeof(RERInsertTableRowAboveCommand), "Insert Table Row Above", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(RowAndColumnsGroup), typeof(RERInsertTableRowBelowCommand), "Insert Table Row Below", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(RowAndColumnsGroup), typeof(RERInsertTableColumnToTheLeftCommand), "Insert Table Column To The Left", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(RowAndColumnsGroup), typeof(RERInsertTableColumnToTheRightCommand), "Insert Table Column To The Right", RibbonButtonItemImageResource);
			AddItemType(typeof(MergeGroup), "Merge");
			AddGroupItemType(typeof(MergeGroup), typeof(RERMergeTableCellsCommand), "Merge Table Cells", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(MergeGroup), typeof(RERSplitTableCellsCommand), "Split Table Cells", RibbonButtonItemImageResource);
			AddItemType(typeof(AlignmentGroup), "Rows And Columns");
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsTopLeftAlignmentCommand), "Top Left Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsTopCenterAlignmentCommand), "Top Center Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsTopRightAlignmentCommand), "Top Right Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsMiddleLeftAlignmentCommand), "Middle Left Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsMiddleCenterAlignmentCommand), "Middle Center Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsMiddleRightAlignmentCommand), "Middle Right Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsBottomLeftAlignmentCommand), "Bottom Left Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsBottomCenterAlignmentCommand), "Bottom Center Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERToggleTableCellsBottomRightAlignmentCommand), "Bottom Right Alignment", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentGroup), typeof(RERShowTableOptionsFormCommand), "Cell Margins", RibbonButtonItemImageResource);
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(RibbonContextTabCategory)) || base.IsBeginGroup(type);
		}
		protected override IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			IDesignTimeCollectionItem newItem = null;
			if(typeof(RERContextTabCategory) == designTimeItem.ColumnType)
				newItem = CreateDefaultRibbonTabCategory(designTimeItem.ColumnType);
			else
				newItem = base.CreateDesignTimeCollectionItem(designTimeItem);
			return newItem;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabCategoryItem() {
			return typeof(RibbonContextTabCategory).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		RibbonContextTabCategory CreateDefaultRibbonTabCategory(Type type) {
			var tabCategory = CreateDefaultRibbonTabCategoryCore(type);
			Items.Add(tabCategory);
			return tabCategory;
		}
		RibbonContextTabCategory CreateDefaultRibbonTabCategoryCore(Type type) {
			if(type == typeof(RERTableToolsContextTabCategory))
				return RibbonHelper.CreateTableContextTabCategory();
			if(type == typeof(RERHeaderAndFooterToolsContextTabCategory))
				return RibbonHelper.CreateHeaderFooterContextTabCategory();
			return null;
		}
		protected override RibbonTab CreateDefaultRibbonTab(Type type) {
			var tabCategory = GetFocusedRibbonTabCategoryItem() as RibbonContextTabCategory;
			var tab = CreateDefaultRibbonTabCore(type);
			tabCategory.Tabs.Add(tab);
			return tab;
		}
		RibbonTab CreateDefaultRibbonTabCore(Type type) {
			if(type == typeof(RERTableDesignTab))
				return RibbonHelper.CreateTableDesignTab();
			if(type == typeof(RERTableLayoutTab))
				return RibbonHelper.CreateTableLayoutTab();
			if(type == typeof(RERHeaderAndFooterTab))
				return RibbonHelper.CreateHeaderFooterContextualTab();
			return null;
		}
		RibbonGroup CreateDefaultRibbonGroupCore(Type type) {
			if(type == typeof(RERTableStyleOptionsGroup))
				return RibbonHelper.CreateTableStyleOptionsGroup();
			if(type == typeof(RERTableStylesGroup))
				return RibbonHelper.CreateTableStylesGroup();
			if(type == typeof(RERBordersAndShadingsGroup))
				return RibbonHelper.CreateBordersAndShadingsGroup();
			if(type == typeof(RERTableGroup))
				return RibbonHelper.CreateTableGroup();
			if(type == typeof(RERRowAndColumnsGroup))
				return RibbonHelper.CreateRowAndColumnsGroup();
			if(type == typeof(RERMergeGroup))
				return RibbonHelper.CreateMergeGroup();
			if(type == typeof(RERAlignmentGroup))
				return RibbonHelper.CreateAlignmentGroup();
			if(type == typeof(RERHeaderFooterNavigationGroup))
				return RibbonHelper.CreateHeaderFooterNavigationGroup();
			if(type == typeof(RERHeaderFooterOptionsGroup))
				return RibbonHelper.CreateHeaderFooterOptionsGroup();
			if(type == typeof(RERHeaderFooterCloseGroup))
				return RibbonHelper.CreateHeaderFooterCloseGroup();
			return null;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Ribbon Context Tabs";
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
		public void CreateDefaultItems() {
			BeginUpdate();
			(Items as RichEditRibbonContextTabCategoryCollection).CreateDefaultRibbonContextTabCategories();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(RibbonContextTabCategory).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override bool IsContextTabCategoriesCollection() {
			return Items != null && Items.GetType() == typeof(RichEditRibbonContextTabCategoryCollection);
		}
	}
	public partial class RichEditRibbonItemsEditorForm : ItemsEditorFrame {
		RichEditRibbonItemsOwner RichEditRibbonOwner { get { return ItemsOwner as RichEditRibbonItemsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(RichEditRibbonOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = DesignUtils.ShowMessage(RichEditRibbonOwner.RichEdit.Site, string.Format(string.Format("Do you want to delete the existing ribbon tabs?")),
					string.Format("Create default tabs for '{0}'", RichEditRibbonOwner.RichEdit.GetType().Name), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					RichEditRibbonOwner.SetSelection(RichEditRibbonOwner.TreeListItemsHash.Keys.ToList());
					RichEditRibbonOwner.RemoveSelectedItems();
				}
			}
			CreateDefaultItems(menuItem);
		}
		protected virtual DialogResult ShowDeleteConfirmation() {
			return DesignUtils.ShowMessage(RichEditRibbonOwner.RichEdit.Site, string.Format(string.Format("Do you want to delete the existing ribbon tabs?")),
					string.Format("Create default tabs for '{0}'", RichEditRibbonOwner.RichEdit.GetType().Name), MessageBoxButtons.YesNoCancel);
		}
		protected virtual void CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			foreach(var designItemType in GetDefaultRibbonItemTypes(menuItem)) {
				ItemsOwner.AddItem(designItemType);
				TreeListItems.FindNodeByKeyID(ItemsOwner.FocusedNodeID).ExpandAll();
			}
		}
		protected virtual List<IDesignTimeColumnAndEditorItem> GetDefaultRibbonItemTypes(DesignEditorDescriptorItem pressedMenuItem) {
			if(pressedMenuItem.ParentItem != null)
				return new List<IDesignTimeColumnAndEditorItem>() { pressedMenuItem.ItemType };
			return RichEditRibbonOwner.ColumnsAndEditors.Where(i => typeof(RERTab).IsAssignableFrom(i.ColumnType)).ToList();
		}
	}
	public partial class RichEditContextTabsItemsEditorForm : RichEditRibbonItemsEditorForm {
		protected RichEditRibbonContextTabItemsOwner RichEditRibbonOwner { get { return ItemsOwner as RichEditRibbonContextTabItemsOwner; } }
		protected override DialogResult ShowDeleteConfirmation() {
			return DesignUtils.ShowMessage(RichEditRibbonOwner.RichEdit.Site, string.Format(string.Format("Do you want to delete the existing context tabs?")),
					string.Format("Create default context tabs for '{0}'", RichEditRibbonOwner.RichEdit.GetType().Name), MessageBoxButtons.YesNoCancel);
		}
		protected override void CreateDefaultItems(DesignEditorDescriptorItem pressedMenuItem) {
			RichEditRibbonOwner.CreateDefaultItems();
		}
	}
}
