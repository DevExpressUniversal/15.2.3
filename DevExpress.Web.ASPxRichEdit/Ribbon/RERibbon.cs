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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.Office.Model;
using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class RichEditDefaultRibbon {
		ASPxRichEdit richedit;
		RibbonTab[] defaultRibbonTabs;
		RibbonContextTabCategory tableContextTabCategory;
		RibbonContextTabCategory headerFooterContextTabCategory;
		RibbonTab fileTab;
		RibbonTab homeTab;
		RibbonTab insertTab;
		RibbonTab pageLayoutTab;
		RibbonTab mailMergeTab;
		RibbonTab viewTab;
		RibbonTab tableDesignContextualTab;
		RibbonTab tableLayoutContextualTab;
		RibbonTab headerFooterContextualTab;
		RibbonGroup commonGroup;
		RibbonGroup undoGroup;
		RibbonGroup clipboardGroup;
		RibbonGroup fontGroup;
		RibbonGroup paragraphGroup;
		RibbonGroup stylesGroup;
		RibbonGroup editingGroup;
		RibbonGroup pagesGroup;
		RibbonGroup tablesGroup;
		RibbonGroup illustrationsGroup;
		RibbonGroup linksGroup;
		RibbonGroup headerAndFooterGroup;
		RibbonGroup symbolsGroup;
		RibbonGroup pageSetupGroup;
		RibbonGroup backgroundGroup;
		RibbonGroup insertFieldsGroup;
		RibbonGroup mailMergeViewGroup;
		RibbonGroup currentRecordGroup;
		RibbonGroup finishMailMergeGroup;
		RibbonGroup showGroup;
		RibbonGroup viewGroup;
		RibbonGroup tableStyleOptionsGroup;
		RibbonGroup tableStylesGroup;
		RibbonGroup bordersAndShadingsGroup;
		RibbonGroup tableGroup;
		RibbonGroup rowAndColumnsGroup;
		RibbonGroup mergeGroup;
		RibbonGroup alignmentGroup;
		RibbonGroup headerFooterNavigationGroup;
		RibbonGroup headerFooterOptionsGroup;
		RibbonGroup headerFooterCloseGroup;
		protected ASPxRichEdit RichEdit {
			get { return richedit; }
			private set { richedit = value; }
		}
		public RichEditDefaultRibbon(ASPxRichEdit richedit) {
			this.richedit = richedit;
		}
		public RibbonTab[] DefaultRibbonTabs {
			get {
				if(defaultRibbonTabs == null) {
					List<RibbonTab> tabs = new List<RibbonTab>();
					tabs.Add(FileTab);
					tabs.Add(HomeTab);
					tabs.Add(InsertTab);
					tabs.Add(PageLayoutTab);
					tabs.Add(MailMergeTab);
					tabs.Add(ViewTab);
					defaultRibbonTabs = tabs.ToArray();
				}
				return defaultRibbonTabs;
			}
		}
		RibbonContextTabCategory[] defaultContextCategories;
		public RibbonContextTabCategory[] DefaultRibbonContextTabCategories {
			get {
				if(defaultContextCategories == null) {
					List<RibbonContextTabCategory> categories = new List<RibbonContextTabCategory>();
					categories.Add(TableContextTabCategory);
					categories.Add(HeaderFooterContextTabCategory);
					defaultContextCategories = categories.ToArray();
				}
				return defaultContextCategories;
			}
		}
		#region RibbonFileTab
		public RibbonTab CreateFileTab() {
			return FileTab;
		}
		protected RibbonTab FileTab {
			get {
				if(fileTab == null) {
					fileTab = new RERFileTab();
					fileTab.Groups.Add(CommonGroup);
				}
				return fileTab;
			}
		}
		protected RibbonGroup CommonGroup {
			get {
				if(commonGroup == null)
					commonGroup = CreateFileCommonGroup();
				return commonGroup;
			}
		}
		public RibbonGroup CreateFileCommonGroup() {
			var ribbonCommonGroup = new RERFileCommonGroup();
			var reFileNew = new RERNewCommand(RibbonItemSize.Large);
			ribbonCommonGroup.Items.Add(reFileNew);
			var reFilOpen = new REROpenCommand(RibbonItemSize.Large);
			ribbonCommonGroup.Items.Add(reFilOpen);
			var reFileSave = new RERSaveCommand(RibbonItemSize.Large);
			ribbonCommonGroup.Items.Add(reFileSave);
			var reFileSaveAs = new RERSaveAsCommand(RibbonItemSize.Large);
			ribbonCommonGroup.Items.Add(reFileSaveAs);
			var rePrint = new RERPrintCommand(RibbonItemSize.Large);
			ribbonCommonGroup.Items.Add(rePrint);
			return ribbonCommonGroup;
		}
		#endregion
		#region RibbonHomeGroup
		public RibbonTab CreateHomeTab() {
			return HomeTab;
		}
		protected RibbonTab HomeTab {
			get {
				if(homeTab == null) {
					homeTab = new RERHomeTab();
					homeTab.Groups.Add(UndoGroup);
					homeTab.Groups.Add(ClipboardGroup);
					homeTab.Groups.Add(FontGroup);
					homeTab.Groups.Add(ParagraphGroup);
					homeTab.Groups.Add(StylesGroup);
					homeTab.Groups.Add(EditingGroup);
				}
				return homeTab;
			}
		}
		protected RibbonGroup UndoGroup {
			get {
				if(undoGroup == null)
					undoGroup = CreateUndoGroup();
				return undoGroup;
			}
		}
		protected RibbonGroup ClipboardGroup {
			get {
				if(clipboardGroup == null)
					clipboardGroup = CreateClipboardGroup();
				return clipboardGroup;
			}
		}
		protected RibbonGroup FontGroup {
			get {
				if(fontGroup == null)
					fontGroup = CreateFontGroup();
				return fontGroup;
			}
		}
		protected RibbonGroup ParagraphGroup {
			get {
				if(paragraphGroup == null)
					paragraphGroup = CreateParagraphGroup();
				return paragraphGroup;
			}
		}
		protected RibbonGroup StylesGroup {
			get {
				if(stylesGroup == null)
					stylesGroup = CreateStylesGroup();
				return stylesGroup;
			}
		}
		protected RibbonGroup EditingGroup {
			get {
				if(editingGroup == null)
					editingGroup = CreateEditingGroup();
				return editingGroup;
			}
		}
		public RibbonGroup CreateUndoGroup() {
			var undoGroup = new RERUndoGroup();
			var reFileUndo = new RERUndoCommand();
			undoGroup.Items.Add(reFileUndo);
			var reFileRedo = new RERRedoCommand();
			undoGroup.Items.Add(reFileRedo);
			return undoGroup;
		}
		public RibbonGroup CreateClipboardGroup() {
			var clipboardGroup = new RERClipboardGroup();
			var paste = new RERPasteCommand(RibbonItemSize.Large);
			clipboardGroup.Items.Add(paste);
			var cut = new RERCutCommand();
			clipboardGroup.Items.Add(cut);
			var copy = new RERCopyCommand();
			clipboardGroup.Items.Add(copy);
			return clipboardGroup;
		}
		public RibbonGroup CreateFontGroup() {
			var fontGroup = new RERFontGroup();
			var reFontNameCommand = new RERFontNameCommand();
			if(RichEdit != null && !RichEdit.DesignMode) {
				reFontNameCommand.FillItems();
			}
			fontGroup.Items.Add(reFontNameCommand);
			var fontSize = new RERFontSizeCommand();
			if(RichEdit != null && !RichEdit.DesignMode) {
				fontSize.FillItems();
			}
			fontGroup.Items.Add(fontSize);
			var increaseFontSize = new RERIncreaseFontSizeCommand();
			fontGroup.Items.Add(increaseFontSize);
			var decreaseFontSize = new RERDecreaseFontSizeCommand();
			fontGroup.Items.Add(decreaseFontSize);
			var changeCase = new RERChangeCaseCommand();
			fontGroup.Items.Add(changeCase);
			var bold = new RERFontBoldCommand();
			fontGroup.Items.Add(bold);
			var italic = new RERFontItalicCommand();
			fontGroup.Items.Add(italic);
			var underline = new RERFontUnderlineCommand();
			fontGroup.Items.Add(underline);
			var strikeout = new RERFontStrikeoutCommand();
			fontGroup.Items.Add(strikeout);
			var superscript = new RERFontSuperscriptCommand();
			fontGroup.Items.Add(superscript);
			var subscript = new RERFontSubscriptCommand();
			fontGroup.Items.Add(subscript);
			var fontColor = new RERFontColorCommand();
			fontGroup.Items.Add(fontColor);
			var fontBackColor = new RERFontBackColorCommand();
			fontGroup.Items.Add(fontBackColor);
			var clearFormatting = new RERClearFormattingCommand();
			fontGroup.Items.Add(clearFormatting);
			return fontGroup;
		}
		public RibbonGroup CreateParagraphGroup() {
			var paragraphGroup = new RERParagraphGroup();
			var bulletedList = new RERBulletedListCommand();
			paragraphGroup.Items.Add(bulletedList);
			var numberingList = new RERNumberingListCommand();
			paragraphGroup.Items.Add(numberingList);
			var multilevelList = new RERMultilevelListCommand();
			paragraphGroup.Items.Add(multilevelList);
			var decreaseIndent = new RERDecreaseIndentCommand();
			paragraphGroup.Items.Add(decreaseIndent);
			var increaseIndent = new RERIncreaseIndentCommand();
			paragraphGroup.Items.Add(increaseIndent);
			var showWhitespace = new RERShowWhitespaceCommand();
			paragraphGroup.Items.Add(showWhitespace);
			var alignLeft = new RERAlignLeftCommand();
			paragraphGroup.Items.Add(alignLeft);
			var alignCenter = new RERAlignCenterCommand();
			paragraphGroup.Items.Add(alignCenter);
			var alignRight = new RERAlignRightCommand();
			paragraphGroup.Items.Add(alignRight);
			var alignJustify = new RERAlignJustifyCommand();
			paragraphGroup.Items.Add(alignJustify);
			var lineSpacing = new RERParagraphLineSpacingCommand();
			paragraphGroup.Items.Add(lineSpacing);
			var paragraphBackColor = new RERParagraphBackColorCommand();
			paragraphGroup.Items.Add(paragraphBackColor);
			return paragraphGroup;
		}
		public RibbonGroup CreateStylesGroup() {
			var group = new RERStylesGroup();
			var changeStyle = new RERChangeStyleCommand();
			group.Items.Add(changeStyle);
			return group;
		}
		public RibbonGroup CreateEditingGroup() {
			var editingGroup = new REREditingGroup();
			var selectAll = new RERSelectAllCommand();
			editingGroup.Items.Add(selectAll);
			return editingGroup;
		}
		#endregion
		#region RibbonInsertTab
		public RibbonTab CreateInsertTab() {
			return InsertTab;
		}
		protected RibbonTab InsertTab {
			get {
				if(insertTab == null) {
					insertTab = new RERInsertTab();
					insertTab.Groups.Add(PagesGroup);
					insertTab.Groups.Add(TablesGroup);
					insertTab.Groups.Add(IllustrationsGroup);
					insertTab.Groups.Add(LinksGroup);
					insertTab.Groups.Add(HeaderAndFooterGroup);
					insertTab.Groups.Add(SymbolsGroup);
				}
				return insertTab;
			}
		}
		protected RibbonGroup PagesGroup {
			get {
				if(pagesGroup == null)
					pagesGroup = CreatePagesGroup();
				return pagesGroup;
			}
		}
		protected RibbonGroup TablesGroup {
			get {
				if(tablesGroup == null)
					tablesGroup = CreateTablesGroup();
				return tablesGroup;
			}
		}
		protected RibbonGroup IllustrationsGroup {
			get {
				if(illustrationsGroup == null)
					illustrationsGroup = CreateIllustrationsGroup();
				return illustrationsGroup;
			}
		}
		protected RibbonGroup LinksGroup {
			get {
				if(linksGroup == null)
					linksGroup = CreateLinksGroup();
				return linksGroup;
			}
		}
		protected RibbonGroup HeaderAndFooterGroup {
			get {
				if (headerAndFooterGroup == null)
					headerAndFooterGroup = CreateHeaderAndFooterGroup();
				return headerAndFooterGroup;
			}
		}
		protected RibbonGroup SymbolsGroup {
			get {
				if(symbolsGroup == null)
					symbolsGroup = CreateSymbolsGroup();
				return symbolsGroup;
			}
		}
		public RibbonGroup CreatePagesGroup() {
			var group = new RERPagesGroup();
			var find = new RERInsertPageBreakCommand(RibbonItemSize.Large);
			group.Items.Add(find);
			return group;
		}
		public RibbonGroup CreateTablesGroup() {
			var group = new RERTablesGroup();
			var insertTable = new RERInsertTableCommand(RibbonItemSize.Large);
			group.Items.Add(insertTable);
			return group;
		}
		public RibbonGroup CreateIllustrationsGroup() {
			var group = new RERIllustrationsGroup();
			var insertPicture = new RERInsertPictureCommand(RibbonItemSize.Large);
			group.Items.Add(insertPicture);
			return group;
		}
		public RibbonGroup CreateLinksGroup() {
			var group = new RERLinksGroup();
			var bookmark = new RERShowBookmarksFormCommand(RibbonItemSize.Large);
			group.Items.Add(bookmark);
			var hyperlink = new RERShowHyperlinkFormCommand(RibbonItemSize.Large);
			group.Items.Add(hyperlink);
			return group;
		}
		public RibbonGroup CreateHeaderAndFooterGroup() {
			var group = new RERHeaderAndFooterGroup();
			var header = new REREditPageHeaderCommand(RibbonItemSize.Large);
			group.Items.Add(header);
			var footer = new REREditPageFooterCommand(RibbonItemSize.Large);
			group.Items.Add(footer);
			var pageNumber = new RERInsertPageNumberFieldCommand(RibbonItemSize.Large);
			group.Items.Add(pageNumber);
			var pageCount = new RERInsertPageCountFieldCommand(RibbonItemSize.Large);
			group.Items.Add(pageCount);
			return group;
		}
		public RibbonGroup CreateSymbolsGroup() {
			var group = new RERSymbolsGroup();
			var symbol = new RERShowSymbolFormCommand(RibbonItemSize.Large);
			group.Items.Add(symbol);
			return group;
		}
		#endregion
		#region RibbonPageLayoutTab
		public RibbonTab CreatePageLayoutTab() {
			return PageLayoutTab;
		}
		protected RibbonTab PageLayoutTab {
			get {
				if(pageLayoutTab == null) {
					pageLayoutTab = new RERPageLayoutTab();
					pageLayoutTab.Groups.Add(PageSetupGroup);
					pageLayoutTab.Groups.Add(BackgroundGroup);
				}
				return pageLayoutTab;
			}
		}
		protected RibbonGroup PageSetupGroup {
			get {
				if(pageSetupGroup == null)
					pageSetupGroup = CreatePageSetupGroup();
				return pageSetupGroup;
			}
		}
		protected RibbonGroup BackgroundGroup {
			get {
				if(backgroundGroup == null)
					backgroundGroup = CreateBackgroundGroup();
				return backgroundGroup;
			}
		}
		public RibbonGroup CreatePageSetupGroup() {
			var group = new RERPageSetupGroup();
			var pageMargins = new RERPageMarginsCommand(RibbonItemSize.Large);
			group.Items.Add(pageMargins);
			var pageOrientation = new RERChangeSectionPageOrientationCommand(RibbonItemSize.Large);
			group.Items.Add(pageOrientation);
			var paperKind = new RERChangeSectionPaperKindCommand(RibbonItemSize.Large);
			group.Items.Add(paperKind);
			var columns = new RERSetSectionColumnsCommand(RibbonItemSize.Large);
			group.Items.Add(columns);
			var insertBreak = new RERInsertBreakCommand(RibbonItemSize.Large);
			group.Items.Add(insertBreak);
			return group;
		}
		public RibbonGroup CreateBackgroundGroup() {
			var group = new RERBackgroundGroup();
			var pageColor = new RERChangePageColorCommand(RibbonItemSize.Large);
			group.Items.Add(pageColor);
			return group;
		}
		#endregion
		#region RibbonMailMergeTab
		public RibbonTab CreateMailMergeTab() {
			return MailMergeTab;
		}
		protected RibbonTab MailMergeTab {
			get {
				if(mailMergeTab == null) {
					mailMergeTab = new RERMailMergeTab();
					mailMergeTab.Groups.Add(InsertFieldsGroup);
					mailMergeTab.Groups.Add(MailMergeViewGroup);
					mailMergeTab.Groups.Add(CurrentRecordGroup);
					mailMergeTab.Groups.Add(FinishMailMergeGroup);
				}
				return mailMergeTab;
			}
		}
		protected RibbonGroup InsertFieldsGroup {
			get {
				if(insertFieldsGroup == null)
					insertFieldsGroup = CreateInsertFieldsGroup();
				return insertFieldsGroup;
			}
		}
		protected RibbonGroup MailMergeViewGroup {
			get {
				if(mailMergeViewGroup == null)
					mailMergeViewGroup = CreateMailMergeViewGroup();
				return mailMergeViewGroup;
			}
		}
		protected RibbonGroup CurrentRecordGroup {
			get {
				if(currentRecordGroup == null)
					currentRecordGroup = CreateCurrentRecordGroup();
				return currentRecordGroup;
			}
		}
		protected RibbonGroup FinishMailMergeGroup {
			get {
				if(finishMailMergeGroup == null)
					finishMailMergeGroup = CreateFinishMailMergeGroup();
				return finishMailMergeGroup;
			}
		}
		public RibbonGroup CreateInsertFieldsGroup() {
			var group = new RERInsertFieldsGroup();
			var insertField = new RERCreateFieldCommand(RibbonItemSize.Large);
			group.Items.Add(insertField);
			var insertMergeField = new RERInsertMergeFieldCommand(RibbonItemSize.Large);
			group.Items.Add(insertMergeField);
			return group;
		}
		public RibbonGroup CreateMailMergeViewGroup() {
			var group = new RERMailMergeViewGroup();
			var toggleViewMergedData = new RERToggleViewMergedDataCommand(RibbonItemSize.Large);
			group.Items.Add(toggleViewMergedData);
			var showAllFieldCodes = new RERToggleShowAllFieldCodesCommand(RibbonItemSize.Large);
			group.Items.Add(showAllFieldCodes);
			var showAllFieldResults = new RERToggleShowAllFieldResultsCommand(RibbonItemSize.Large);
			group.Items.Add(showAllFieldResults);
			var updateAllFields = new RERUpdateAllFieldsCommand(RibbonItemSize.Large);
			group.Items.Add(updateAllFields);
			return group;
		}
		public RibbonGroup CreateCurrentRecordGroup() {
			var group = new RERCurrentRecordGroup();
			var firstDataRecord = new RERFirstDataRecordCommand(RibbonItemSize.Large);
			group.Items.Add(firstDataRecord);
			var previousDataRecord = new RERPreviousDataRecordCommand(RibbonItemSize.Large);
			group.Items.Add(previousDataRecord);
			var nextDataRecord = new RERNextDataRecordCommand(RibbonItemSize.Large);
			group.Items.Add(nextDataRecord);
			var lastDataRecord = new RERLastDataRecordCommand(RibbonItemSize.Large);
			group.Items.Add(lastDataRecord);
			return group;
		}
		public RibbonGroup CreateFinishMailMergeGroup() {
			var group = new RERFinishMailMergeGroup();
			var finishAndMerge = new RERFinishAndMergeCommand(RibbonItemSize.Large);
			group.Items.Add(finishAndMerge);
			return group;
		}
		#endregion
		#region RibbonViewTab
		public RibbonTab CreateViewTab() {
			return ViewTab;
		}
		protected RibbonTab ViewTab {
			get {
				if(viewTab == null) {
					viewTab = new RERViewTab();
					viewTab.Groups.Add(ShowGroup);
					viewTab.Groups.Add(ViewGroup);
				}
				return viewTab;
			}
		}
		protected RibbonGroup ShowGroup {
			get {
				if(showGroup == null)
					showGroup = CreateShowGroup();
				return showGroup;
			}
		}
		protected RibbonGroup ViewGroup {
			get {
				if(viewGroup == null)
					viewGroup = CreateViewGroup();
				return viewGroup;
			}
		}
		public RibbonGroup CreateShowGroup() {
			var group = new RERShowGroup();
			var horizontalRuler = new RERToggleShowHorizontalRulerCommand(RibbonItemSize.Large);
			horizontalRuler.Checked = RichEdit == null || RichEdit.Settings.HorizontalRuler.Visibility == RichEditRulerVisibility.Hidden;
			group.Items.Add(horizontalRuler);
			return group;
		}
		public RibbonGroup CreateViewGroup() {
			var group = new RERViewGroup();
			var fullScreen = new RERToggleFullScreenCommand(RibbonItemSize.Large);
			group.Items.Add(fullScreen);
			return group;
		}
		#endregion
		#region RibbonReviewTab
		#endregion
		#region RibbonReferencesTab
		#endregion
		#region RibbonPictureTab
		#endregion
		#region RibbonTableContextualTab
		public RibbonContextTabCategory CreateTableContextTabCategory() {
			return TableContextTabCategory;
		}
		protected RibbonContextTabCategory TableContextTabCategory {
			get {
				if(tableContextTabCategory == null) {
					tableContextTabCategory = new RERTableToolsContextTabCategory();
					tableContextTabCategory.Tabs.Add(TableDesignTab);
					tableContextTabCategory.Tabs.Add(TableLayoutTab);
				}
				return tableContextTabCategory;
			}
		}
		public RibbonTab CreateTableDesignTab() {
			return TableDesignTab;
		}
		protected RibbonTab TableDesignTab {
			get {
				if(tableDesignContextualTab == null) {
					tableDesignContextualTab = new RERTableDesignTab();
					tableDesignContextualTab.Groups.Add(TableStyleOptionsGroup);
					tableDesignContextualTab.Groups.Add(TableStylesGroup);
					tableDesignContextualTab.Groups.Add(BordersAndShadingsGroup);
				}
				return tableDesignContextualTab;
			}
		}
		protected RibbonGroup TableStyleOptionsGroup {
			get {
				if(tableStyleOptionsGroup == null)
					tableStyleOptionsGroup = CreateTableStyleOptionsGroup();
				return tableStyleOptionsGroup;
			}
		}
		protected RibbonGroup TableStylesGroup {
			get {
				if(tableStylesGroup == null)
					tableStylesGroup = CreateTableStylesGroup();
				return tableStylesGroup;
			}
		}
		protected RibbonGroup BordersAndShadingsGroup {
			get {
				if(bordersAndShadingsGroup == null)
					bordersAndShadingsGroup = CreateBordersAndShadingsGroup();
				return bordersAndShadingsGroup;
			}
		}
		public RibbonGroup CreateTableStyleOptionsGroup() {
			var group = new RERTableStyleOptionsGroup();
			var toggleFirstRow = new RERToggleFirstRowCommand();
			group.Items.Add(toggleFirstRow);
			var toggleLastRow = new RERToggleLastRowCommand();
			group.Items.Add(toggleLastRow);
			var toggleBandedRows = new RERToggleBandedRowsCommand();
			group.Items.Add(toggleBandedRows);
			var toggleFirstColumn = new RERToggleFirstColumnCommand();
			group.Items.Add(toggleFirstColumn);
			var toggleLastColumn = new RERToggleLastColumnCommand();
			group.Items.Add(toggleLastColumn);
			var toggleBandedColumn = new RERToggleBandedColumnCommand();
			group.Items.Add(toggleBandedColumn);
			return group;
		}
		public RibbonGroup CreateTableStylesGroup() {
			var group = new RERTableStylesGroup();
			var changeTableStyle = new RERChangeTableStyleCommand();
			group.Items.Add(changeTableStyle);
			return group;
		}
		public RibbonGroup CreateBordersAndShadingsGroup() {
			var group = new RERBordersAndShadingsGroup();
			var сhangeCurrentBorderRepositoryItemLineStyle = new RERChangeCurrentBorderRepositoryItemLineStyleCommand();
			if (RichEdit != null && !RichEdit.DesignMode)
				сhangeCurrentBorderRepositoryItemLineStyle.FillItems();
			group.Items.Add(сhangeCurrentBorderRepositoryItemLineStyle);
			var changeCurrentBorderRepositoryItemLineThickness = new RERChangeCurrentBorderRepositoryItemLineThicknessCommand();
			if (RichEdit != null && !RichEdit.DesignMode)
				changeCurrentBorderRepositoryItemLineThickness.FillItems();
			group.Items.Add(changeCurrentBorderRepositoryItemLineThickness);
			var changeCurrentBorderRepositoryItemColorCommand = new RERChangeCurrentBorderRepositoryItemColorCommand(RibbonItemSize.Large);
			group.Items.Add(changeCurrentBorderRepositoryItemColorCommand);
			var changeTableBorders = new RERChangeTableBordersCommand(RibbonItemSize.Large);
			group.Items.Add(changeTableBorders);
			var changeTableCellShading = new RERChangeTableCellShadingCommand(RibbonItemSize.Large);
			group.Items.Add(changeTableCellShading);
			return group;
		}
		public RibbonTab CreateTableLayoutTab() {
			return TableLayoutTab;
		}
		protected RibbonTab TableLayoutTab {
			get {
				if(tableLayoutContextualTab == null) {
					tableLayoutContextualTab = new RERTableLayoutTab();
					tableLayoutContextualTab.Groups.Add(TableGroup);
					tableLayoutContextualTab.Groups.Add(RowAndColumnsGroup);
					tableLayoutContextualTab.Groups.Add(MergeGroup);
					tableLayoutContextualTab.Groups.Add(AlignmentGroup);
				}
				return tableLayoutContextualTab;
			}
		}
		protected RibbonGroup TableGroup {
			get {
				if(tableGroup == null)
					tableGroup = CreateTableGroup();
				return tableGroup;
			}
		}
		protected RibbonGroup RowAndColumnsGroup {
			get {
				if(rowAndColumnsGroup == null)
					rowAndColumnsGroup = CreateRowAndColumnsGroup();
				return rowAndColumnsGroup;
			}
		}
		protected RibbonGroup MergeGroup {
			get {
				if(mergeGroup == null)
					mergeGroup = CreateMergeGroup();
				return mergeGroup;
			}
		}
		protected RibbonGroup AlignmentGroup {
			get {
				if(alignmentGroup == null)
					alignmentGroup = CreateAlignmentGroup();
				return alignmentGroup;
			}
		}
		public RibbonGroup CreateTableGroup() {
			var group = new RERTableGroup();
			var selectTableElements = new RERSelectTableElementsCommand(RibbonItemSize.Large);
			group.Items.Add(selectTableElements);
			var toggleShowTableGridLines = new RERToggleShowTableGridLinesCommand(RibbonItemSize.Large);
			group.Items.Add(toggleShowTableGridLines);
			var showTablePropertiesForm = new RERShowTablePropertiesFormCommand(RibbonItemSize.Large);
			group.Items.Add(showTablePropertiesForm);
			return group;
		}
		public RibbonGroup CreateRowAndColumnsGroup() {
			var group = new RERRowAndColumnsGroup();
			var deleteTableElements = new RERDeleteTableElementsCommand(RibbonItemSize.Large);
			group.Items.Add(deleteTableElements);
			var insertTableRowAbove = new RERInsertTableRowAboveCommand(RibbonItemSize.Large);
			group.Items.Add(insertTableRowAbove);
			var insertTableRowBelow = new RERInsertTableRowBelowCommand(RibbonItemSize.Large);
			group.Items.Add(insertTableRowBelow);
			var insertTableColumnToTheLeft = new RERInsertTableColumnToTheLeftCommand(RibbonItemSize.Large);
			group.Items.Add(insertTableColumnToTheLeft);
			var insertTableColumnToTheRight = new RERInsertTableColumnToTheRightCommand(RibbonItemSize.Large);
			group.Items.Add(insertTableColumnToTheRight);
			return group;
		}
		public RibbonGroup CreateMergeGroup() {
			var group = new RERMergeGroup();
			var mergeTableCells = new RERMergeTableCellsCommand(RibbonItemSize.Large);
			group.Items.Add(mergeTableCells);
			var splitTableCells = new RERSplitTableCellsCommand(RibbonItemSize.Large);
			group.Items.Add(splitTableCells);
			return group;
		}
		public RibbonGroup CreateAlignmentGroup() {
			var group = new RERAlignmentGroup();
			var toggleTableCellsTopLeftAlignment = new RERToggleTableCellsTopLeftAlignmentCommand();
			group.Items.Add(toggleTableCellsTopLeftAlignment);
			var toggleTableCellsTopCenterAlignment = new RERToggleTableCellsTopCenterAlignmentCommand();
			group.Items.Add(toggleTableCellsTopCenterAlignment);
			var toggleTableCellsTopRightAlignment = new RERToggleTableCellsTopRightAlignmentCommand();
			group.Items.Add(toggleTableCellsTopRightAlignment);
			var toggleTableCellsMiddleLeftAlignment = new RERToggleTableCellsMiddleLeftAlignmentCommand();
			group.Items.Add(toggleTableCellsMiddleLeftAlignment);
			var toggleTableCellsMiddleCenterAlignment = new RERToggleTableCellsMiddleCenterAlignmentCommand();
			group.Items.Add(toggleTableCellsMiddleCenterAlignment);
			var toggleTableCellsMiddleRightAlignment = new RERToggleTableCellsMiddleRightAlignmentCommand();
			group.Items.Add(toggleTableCellsMiddleRightAlignment);
			var toggleTableCellsBottomLeftAlignment = new RERToggleTableCellsBottomLeftAlignmentCommand();
			group.Items.Add(toggleTableCellsBottomLeftAlignment);
			var toggleTableCellsBottomCenterAlignment = new RERToggleTableCellsBottomCenterAlignmentCommand();
			group.Items.Add(toggleTableCellsBottomCenterAlignment);
			var toggleTableCellsBottomRightAlignment = new RERToggleTableCellsBottomRightAlignmentCommand();
			group.Items.Add(toggleTableCellsBottomRightAlignment);
			var showCellOptionsForm = new RERShowTableOptionsFormCommand(RibbonItemSize.Large);
			group.Items.Add(showCellOptionsForm);
			return group;
		}
		#endregion
		#region RibbonHeaderFooterContextualTab
		public RibbonContextTabCategory CreateHeaderFooterContextTabCategory() {
			return HeaderFooterContextTabCategory;
		}
		protected RibbonContextTabCategory HeaderFooterContextTabCategory {
			get {
				if(headerFooterContextTabCategory == null) {
					headerFooterContextTabCategory = new RERHeaderAndFooterToolsContextTabCategory();
					headerFooterContextTabCategory.Tabs.Add(HeaderFooterContextualTab);
				}
				return headerFooterContextTabCategory;
			}
		}
		public RibbonTab CreateHeaderFooterContextualTab() {
			return HeaderFooterContextualTab;
		}
		protected RibbonTab HeaderFooterContextualTab {
			get {
				if (headerFooterContextualTab == null) {
					headerFooterContextualTab = new RERHeaderAndFooterTab();
					headerFooterContextualTab.Groups.Add(HeaderFooterNavigationGroup);
					headerFooterContextualTab.Groups.Add(HeaderFooterOptionsGroup);
					headerFooterContextualTab.Groups.Add(HeaderFooterCloseGroup);
				}
				return headerFooterContextualTab;
			}
		}
		protected RibbonGroup HeaderFooterNavigationGroup {
			get {
				if (headerFooterNavigationGroup == null)
					headerFooterNavigationGroup = CreateHeaderFooterNavigationGroup();
				return headerFooterNavigationGroup;
			}
		}
		public RibbonGroup CreateHeaderFooterNavigationGroup() {
			var group = new RERHeaderFooterNavigationGroup();
			group.Items.Add(new RERGoToPageHeaderCommand(RibbonItemSize.Large));
			group.Items.Add(new RERGoToPageFooterCommand(RibbonItemSize.Large));
			group.Items.Add(new RERGoToPreviousPageHeaderFooterCommand(RibbonItemSize.Large));
			group.Items.Add(new RERGoToNextPageHeaderFooterCommand(RibbonItemSize.Large));
			group.Items.Add(new RERToggleHeaderFooterLinkToPreviousCommand(RibbonItemSize.Large));
			return group;
		}
		protected RibbonGroup HeaderFooterOptionsGroup {
			get {
				if (headerFooterOptionsGroup == null)
					headerFooterOptionsGroup = CreateHeaderFooterOptionsGroup();
				return headerFooterOptionsGroup;
			}
		}
		public RibbonGroup CreateHeaderFooterOptionsGroup() {
			var group = new RERHeaderFooterOptionsGroup();
			group.Items.Add(new RERToggleDifferentFirstPageCommand());
			group.Items.Add(new RERToggleDifferentOddAndEvenPagesCommand());
			return group;
		}
		protected RibbonGroup HeaderFooterCloseGroup {
			get {
				if (headerFooterCloseGroup == null)
					headerFooterCloseGroup = CreateHeaderFooterCloseGroup();
				return headerFooterCloseGroup;
			}
		}
		public RibbonGroup CreateHeaderFooterCloseGroup() {
			var group = new RERHeaderFooterCloseGroup();
			group.Items.Add(new RERClosePageHeaderFooterCommand(RibbonItemSize.Large));
			return group;
		}
		#endregion
	}
}
