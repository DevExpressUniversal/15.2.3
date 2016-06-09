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

using System.Collections.Generic;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class SpreadsheetDefaultRibbon {
		ASPxSpreadsheet spreadsheet;
		RibbonTab[] defaultRibbonTabs;
		RibbonContextTabCategory[] defaultRibbonTabCategories;
		SRFileTab fileTab;
		SRHomeTab homeTab;
		SRInsertTab insertTab;
		SRPageLayoutTab pageLayoutTab;
		SRFormulasTab formulasTab;
		SRDataTab dataTab;
		SRViewTab viewTab;
		SRTableDesignContextTab tableDesignContextTab;
		SRPictureFormatContextTab pictureFormatContextTab;
		SRChartDesignContextTab chartDesignContextTab;
		SRChartLayoutContextTab chartLayoutContextTab;
		SRChartFormatContextTab chartFormatContextTab;
		SRTableToolsContextTabCategory tableToolsContextTabCategory;
		SRPictureToolsContextTabCategory pictureToolsContextTabCategory;
		SRChartToolsContextTabCategory chartToolsContextTabCategory;
		SRFileCommonGroup commonGroup;
		SRUndoGroup undoGroup;
		SRClipboardGroup clipboardGroup;
		SRFontGroup fontGroup;
		SRAlignmentGroup alignmentGroup;
		SRNumberGroup numberGroup;
		SRCellsGroup cellsGroup;
		SREditingGroup editingGroup;
		SRStylesGroup stylesGroup;
		SRIllustrationsGroup illustrationsGroup;
		SRChartsGroup chartsGroup;
		SRLinksGroup linksGroup;
		SRTablesGroup tablesGroup;
		SRPageSetupGroup pageSetupGroup;
		SRShowGroup showGroup;
		SRPrintGroup printGroup;
		SRFunctionLibraryGroup functionLibraryGroup;
		SRCalculationGroup calculationGroup;
		SRDataSortAndFilterGroup sortFilterGroup;
		SRDataToolsGroup dataToolsGroup;
		SRViewGroup viewGroup;
		SRWindowGroup windowGroup;
		SRTablePropertiesGroup tablePropertiesGroup;
		SRTableToolsGroup tableToolsGroup;
		SRTableStyleOptionsGroup tableStyleOptionsGroup;
		SRTableStyleGroup tableStyleGroup;
		SRPictureArrangeGroup pictureArrangeGroup;
		SRChartTypeGroup chartTypeGroup;
		SRChartDataGroup chartDataGroup;
		SRChartLabelsGroup chartLabelsGroup;
		SRChartAxesGroup chartAxesGroup;
		SRChartArrangeGroup chartArrangeGroup;
		protected ASPxSpreadsheet Spreadsheet {
			get {
				return spreadsheet;
			}
			private set {
				spreadsheet = value;
			}
		}
		public SpreadsheetDefaultRibbon(ASPxSpreadsheet spreadsheet) {
			this.spreadsheet = spreadsheet;
		}
		public RibbonTab[] DefaultRibbonTabs {
			get {
				if(defaultRibbonTabs == null) {
					List<RibbonTab> tabs = new List<RibbonTab>();
					tabs.Add(FileTab);
					tabs.Add(HomeTab);
					tabs.Add(InsertTab);
					tabs.Add(PageLayoutTab);
					tabs.Add(FormulasTab);
					tabs.Add(DataTab);
					tabs.Add(ViewTab);
					defaultRibbonTabs = tabs.ToArray();
				}
				return defaultRibbonTabs;
			}
		}
		public RibbonContextTabCategory[] DefaultRibbonContextTabCategories {
			get {
				if(defaultRibbonTabCategories == null) {
					List<RibbonContextTabCategory> tabCategories = new List<RibbonContextTabCategory>();
					tabCategories.Add(TableToolsContextTabCategory);
					tabCategories.Add(PictureToolsContextTabCategory);
					tabCategories.Add(ChartToolsContextTabCategory);
					defaultRibbonTabCategories = tabCategories.ToArray();
				}
				return defaultRibbonTabCategories;
			}
		}
		#region RibbonFileTab
		public SRFileTab CreateFileTab() {
			return FileTab;
		}
		protected SRFileTab FileTab {
			get {
				if(fileTab == null) {
					fileTab = new SRFileTab();
					fileTab.Groups.Add(CommonGroup);
				}
				return fileTab;
			}
		}
		protected SRFileCommonGroup CommonGroup {
			get {
				if(commonGroup == null)
					commonGroup = CreateFileCommonGroup();
				return commonGroup;
			}
		}
		public SRFileCommonGroup CreateFileCommonGroup() {
			var ribbonCommonGroup = new SRFileCommonGroup();
			var spFileNew = new SRFileNewCommand();
			ribbonCommonGroup.Items.Add(spFileNew);
			var spFilOpen = new SRFileOpenCommand();
			ribbonCommonGroup.Items.Add(spFilOpen);
			var spFileSave = new SRFileSaveCommand();
			ribbonCommonGroup.Items.Add(spFileSave);
			var spFileSaveAs = new SRFileSaveAsCommand();
			ribbonCommonGroup.Items.Add(spFileSaveAs);
			var spFilePrint = new SRFilePrintCommand();
			ribbonCommonGroup.Items.Add(spFilePrint);
			return ribbonCommonGroup;
		}
		#endregion
		#region RibbonHomeTab
		public SRHomeTab CreateHomeTab() {
			return HomeTab;
		}
		protected SRHomeTab HomeTab {
			get {
				if(homeTab == null) {
					homeTab = new SRHomeTab();
					homeTab.Groups.Add(UndoGroup);
					homeTab.Groups.Add(ClipboardGroup);
					homeTab.Groups.Add(FontGroup);
					homeTab.Groups.Add(AlignmentGroup);
					homeTab.Groups.Add(NumberGroup);
					homeTab.Groups.Add(CellsGroup);
					homeTab.Groups.Add(EditingGroup);
					homeTab.Groups.Add(StylesGroup);
				}
				return homeTab;
			}
		}
		protected SRUndoGroup UndoGroup {
			get {
				if(undoGroup == null)
					undoGroup = CreateUndoGroup();
				return undoGroup;
			}
		}
		public SRUndoGroup CreateUndoGroup() {
			var undoGroup = new SRUndoGroup();
			var spFileUndo = new SRFileUndoCommand();
			undoGroup.Items.Add(spFileUndo);
			var spFileRedo = new SRFileRedoCommand();
			undoGroup.Items.Add(spFileRedo);
			return undoGroup;
		}
		protected SRClipboardGroup ClipboardGroup {
			get {
				if(clipboardGroup == null)
					clipboardGroup = CreateClipboardGroup();
				return clipboardGroup;
			}
		}
		public SRClipboardGroup CreateClipboardGroup() {
			var clipboardGroup = new SRClipboardGroup();
			var paste = new SRPasteSelectionCommand();
			clipboardGroup.Items.Add(paste);
			var cut = new SRCutSelectionCommand();
			clipboardGroup.Items.Add(cut);
			var copy = new SRCopySelectionCommand();
			clipboardGroup.Items.Add(copy);
			return clipboardGroup;
		}
		protected SRFontGroup FontGroup {
			get {
				if(fontGroup == null)
					fontGroup = CreateFontGroup();
				return fontGroup;
			}
		}
		public SRFontGroup CreateFontGroup() {
			var fontGroup = new SRFontGroup();
			var spFontNameCommand = new SRFormatFontNameCommand();
			if(Spreadsheet == null || !Spreadsheet.DesignMode) {
				spFontNameCommand.FillItems();
			}
			fontGroup.Items.Add(spFontNameCommand);
			var fontSize = new SRFormatFontSizeCommand();
			if(Spreadsheet == null || !Spreadsheet.DesignMode) {
				fontSize.FillItems();
			}
			fontGroup.Items.Add(fontSize);
			var increaseFontSize = new SRFormatIncreaseFontSizeCommand();
			fontGroup.Items.Add(increaseFontSize);
			var decreaseFontSize = new SRFormatDecreaseFontSizeCommand();
			fontGroup.Items.Add(decreaseFontSize);
			var bold = new SRFormatFontBoldCommand();
			fontGroup.Items.Add(bold);
			var italic = new SRFormatFontItalicCommand();
			fontGroup.Items.Add(italic);
			var underline = new SRFormatFontUnderlineCommand();
			fontGroup.Items.Add(underline);
			var strike = new SRFormatFontStrikeoutCommand();
			fontGroup.Items.Add(strike);
			var borders = new SRFormatBordersCommand();
			fontGroup.Items.Add(borders);
			var fillColor = new SRFormatFillColorCommand();
			fontGroup.Items.Add(fillColor);
			var fontColor = new SRFormatFontColorCommand();
			fontGroup.Items.Add(fontColor);
			var borderLineColor = new SRFormatBorderLineColorCommand();
			fontGroup.Items.Add(borderLineColor);
			return fontGroup;
		}
		protected SRAlignmentGroup AlignmentGroup {
			get {
				if(alignmentGroup == null)
					alignmentGroup = CreateAlignmentGroup();
				return alignmentGroup;
			}
		}
		public SRAlignmentGroup CreateAlignmentGroup() {
			var alignmentGroup = new SRAlignmentGroup();
			var topAlign = new SRFormatAlignmentTopCommand();
			alignmentGroup.Items.Add(topAlign);
			var middleAlign = new SRFormatAlignmentMiddleCommand();
			alignmentGroup.Items.Add(middleAlign);
			var bottomAlign = new SRFormatAlignmentBottomCommand();
			alignmentGroup.Items.Add(bottomAlign);
			var leftAlign = new SRFormatAlignmentLeftCommand();
			alignmentGroup.Items.Add(leftAlign);
			var centerAlign = new SRFormatAlignmentCenterCommand();
			alignmentGroup.Items.Add(centerAlign);
			var rightAlign = new SRFormatAlignmentRightCommand();
			alignmentGroup.Items.Add(rightAlign);
			var decreaseFontSize = new SRFormatDecreaseIndentCommand();
			alignmentGroup.Items.Add(decreaseFontSize);
			var increaseIndent = new SRFormatIncreaseIndentCommand();
			alignmentGroup.Items.Add(increaseIndent);
			var wrapText = new SRFormatWrapTextCommand();
			alignmentGroup.Items.Add(wrapText);
			var mergeCell = new SREditingMergeCellsGroupCommand();
			alignmentGroup.Items.Add(mergeCell);
			return alignmentGroup;
		}
		protected SRNumberGroup NumberGroup {
			get {
				if(numberGroup == null)
					numberGroup = CreateNumberGroup();
				return numberGroup;
			}
		}
		public SRNumberGroup CreateNumberGroup() {
			var alignmentGroup = new SRNumberGroup();
			var numberAccounting = new SRFormatNumberAccountingCommand();
			alignmentGroup.Items.Add(numberAccounting);
			var percentStyle = new SRFormatNumberPercentCommand();
			alignmentGroup.Items.Add(percentStyle);
			var commaStyle = new SRFormatNumberCommaStyleCommand();
			alignmentGroup.Items.Add(commaStyle);
			var increaseDecimal = new SRFormatNumberIncreaseDecimalCommand();
			alignmentGroup.Items.Add(increaseDecimal);
			var decreaseDecimal = new SRFormatNumberDecreaseDecimalCommand();
			alignmentGroup.Items.Add(decreaseDecimal);
			return alignmentGroup;
		}
		protected SRCellsGroup CellsGroup {
			get {
				if(cellsGroup == null)
					cellsGroup = CreateCellsGroup();
				return cellsGroup;
			}
		}
		public SRCellsGroup CreateCellsGroup() {
			var ribbonCellsGroup = new SRCellsGroup();
			RibbonDropDownButtonItem insert = new SRFormatInsertCommand();
			ribbonCellsGroup.Items.Add(insert);
			RibbonDropDownButtonItem delete = new SRFormatRemoveCommand();
			ribbonCellsGroup.Items.Add(delete);
			RibbonDropDownButtonItem format = new SRFormatFormatCommand();
			ribbonCellsGroup.Items.Add(format);
			return ribbonCellsGroup;
		}
		protected SREditingGroup EditingGroup {
			get {
				if(editingGroup == null)
					editingGroup = CreateEditingGroup();
				return editingGroup;
			}
		}
		public SREditingGroup CreateEditingGroup() {
			var ribbonEditingGroup = new SREditingGroup();
			RibbonDropDownButtonItem mathSimpleFunctions = new SRFormatAutoSumCommand();
			ribbonEditingGroup.Items.Add(mathSimpleFunctions);
			RibbonDropDownButtonItem fill = new SRFormatFillCommand();
			ribbonEditingGroup.Items.Add(fill);
			RibbonDropDownButtonItem clears = new SRFormatClearCommand();
			ribbonEditingGroup.Items.Add(clears);
			SREditingSortAndFilterCommand sort = new SREditingSortAndFilterCommand();
			ribbonEditingGroup.Items.Add(sort);
			SREditingFindAndSelectCommand findAndSelect = new SREditingFindAndSelectCommand();
			ribbonEditingGroup.Items.Add(findAndSelect);
			return ribbonEditingGroup;
		}
		protected SRStylesGroup StylesGroup {
			get {
				if(stylesGroup == null)
					stylesGroup = CreateStylesGroup();
				return stylesGroup;
			}
		}
		public SRStylesGroup CreateStylesGroup() {
			var stylesGroup = new SRStylesGroup();
			var formatAsTable = new SRFormatAsTableCommand();
			stylesGroup.Items.Add(formatAsTable);
			return stylesGroup;
		}
		#endregion
		#region RibbonInsertTab
		public SRInsertTab CreateInsertTab() {
			return InsertTab;
		}
		protected SRInsertTab InsertTab {
			get {
				if(insertTab == null) {
					insertTab = new SRInsertTab();
					insertTab.Groups.Add(TablesGroup);
					insertTab.Groups.Add(IllustrationsGroup);
					insertTab.Groups.Add(ChartsGroup);
					insertTab.Groups.Add(LinksGroup);
				}
				return insertTab;
			}
		}
		protected SRChartsGroup ChartsGroup {
			get {
				if(chartsGroup == null)
					chartsGroup = CreateChartsGroup();
				return chartsGroup;
			}
		}
		public SRChartsGroup CreateChartsGroup() {
			var ribbonChartsGroup = new SRChartsGroup();
			var column = new SRInsertChartColumnCommand();
			ribbonChartsGroup.Items.Add(column);
			var line = new SRInsertChartLinesCommand();
			ribbonChartsGroup.Items.Add(line);
			var pie = new SRInsertChartPiesCommand();
			ribbonChartsGroup.Items.Add(pie);
			var bar = new SRInsertChartBarsCommand();
			ribbonChartsGroup.Items.Add(bar);
			var area = new SRInsertChartAreasCommand();
			ribbonChartsGroup.Items.Add(area);
			var scatter = new SRInsertChartScattersCommand();
			ribbonChartsGroup.Items.Add(scatter);
			var other = new SRInsertChartOthersCommand();
			ribbonChartsGroup.Items.Add(other);
			return ribbonChartsGroup;
		}
		protected SRIllustrationsGroup IllustrationsGroup {
			get {
				if(illustrationsGroup == null)
					illustrationsGroup = CreateIllustrationsGroup();
				return illustrationsGroup;
			}
		}
		public SRIllustrationsGroup CreateIllustrationsGroup() {
			var ribbonIllustrationsGroup = new SRIllustrationsGroup();
			var picture = new SRFormatInsertPictureCommand();
			ribbonIllustrationsGroup.Items.Add(picture);
			return ribbonIllustrationsGroup;
		}
		protected SRLinksGroup LinksGroup {
			get {
				if(linksGroup == null)
					linksGroup = CreateLinksGroup();
				return linksGroup;
			}
		}
		public SRLinksGroup CreateLinksGroup() {
			var ribbonLinksGroup = new SRLinksGroup();
			var hyperLink = new SRFormatInsertHyperlinkCommand();
			ribbonLinksGroup.Items.Add(hyperLink);
			return ribbonLinksGroup;
		}
		protected SRTablesGroup TablesGroup {
			get {
				if(tablesGroup == null)
					tablesGroup = CreateTablesGroup();
				return tablesGroup;
			}
		}
		public SRTablesGroup CreateTablesGroup() {
			var ribbonTablesGroup = new SRTablesGroup();
			var insertTable = new SRInsertTableCommand();
			ribbonTablesGroup.Items.Add(insertTable);
			return ribbonTablesGroup;
		}
		#endregion
		#region PageLayout
		public SRPageLayoutTab CreatePageLayoutTab() {
			return PageLayoutTab;
		}
		protected SRPageLayoutTab PageLayoutTab {
			get {
				if(pageLayoutTab == null) {
					pageLayoutTab = new SRPageLayoutTab();
					pageLayoutTab.Groups.Add(PageSetupGroup);
					pageLayoutTab.Groups.Add(PrintGroup);
				}
				return pageLayoutTab;
			}
		}
		protected SRPageSetupGroup PageSetupGroup {
			get {
				if(pageSetupGroup == null)
					pageSetupGroup = CreatePageSetupGroup();
				return pageSetupGroup;
			}
		}
		public SRPageSetupGroup CreatePageSetupGroup() {
			SRPageSetupGroup ribbonPageSetupGroup = new SRPageSetupGroup();
			SRPageSetupMarginsCommand pageMargins = new SRPageSetupMarginsCommand();
			ribbonPageSetupGroup.Items.Add(pageMargins);
			SRPageSetupOrientationCommand pageOrientation = new SRPageSetupOrientationCommand();
			ribbonPageSetupGroup.Items.Add(pageOrientation);
			SRPageSetupPaperKindCommand paperSize = new SRPageSetupPaperKindCommand();
			ribbonPageSetupGroup.Items.Add(paperSize);
			return ribbonPageSetupGroup;
		}
		protected SRPrintGroup PrintGroup {
			get {
				if(printGroup == null)
					printGroup = CreatePrintGroup();
				return printGroup;
			}
		}
		public SRPrintGroup CreatePrintGroup() {
			SRPrintGroup ribbonPrintGroup = new SRPrintGroup();
			SRPrintGridlinesCommand printGridlines = new SRPrintGridlinesCommand();
			ribbonPrintGroup.Items.Add(printGridlines);
			SRPrintHeadingsCommand printHeadings = new SRPrintHeadingsCommand();
			ribbonPrintGroup.Items.Add(printHeadings);
			return ribbonPrintGroup;
		}
		#endregion
		#region RibbonFormulasTab
		public SRFormulasTab CreateFormulasTab() {
			return FormulasTab;
		}
		protected SRFormulasTab FormulasTab {
			get {
				if(formulasTab == null) {
					formulasTab = new SRFormulasTab();
					formulasTab.Groups.Add(FunctionLibraryGroup);
					formulasTab.Groups.Add(CalculationGroup);
				}
				return formulasTab;
			}
		}
		protected SRFunctionLibraryGroup FunctionLibraryGroup {
			get {
				if(functionLibraryGroup == null)
					functionLibraryGroup = CreateFunctionLibraryGroup();
				return functionLibraryGroup;
			}
		}
		public SRFunctionLibraryGroup CreateFunctionLibraryGroup() {
			SRFunctionLibraryGroup ribbonFunctionLibrary = new SRFunctionLibraryGroup();
			var autoSumCommand = new SRFunctionsAutoSumCommand();
			ribbonFunctionLibrary.Items.Add(autoSumCommand);
			var financialCommand = new SRFunctionsFinancialCommand();
			ribbonFunctionLibrary.Items.Add(financialCommand);
			var logicalCommand = new SRFunctionsLogicalCommand();
			ribbonFunctionLibrary.Items.Add(logicalCommand);
			var textCommand = new SRFunctionsTextCommand();
			ribbonFunctionLibrary.Items.Add(textCommand);
			var dateAndTimeCommand = new SRFunctionsDateAndTimeCommand();
			ribbonFunctionLibrary.Items.Add(dateAndTimeCommand);
			var lookupCommand = new SRFunctionsLookupAndReferenceCommand();
			ribbonFunctionLibrary.Items.Add(lookupCommand);
			var mathCommand = new SRFunctionsMathAndTrigonometryCommand();
			ribbonFunctionLibrary.Items.Add(mathCommand);
			var moreCommand = new SRFunctionsMoreCommand();
			ribbonFunctionLibrary.Items.Add(moreCommand);
			return ribbonFunctionLibrary;
		}
		protected SRCalculationGroup CalculationGroup {
			get {
				if(calculationGroup == null)
					calculationGroup = CreateCalculationGroup();
				return calculationGroup;
			}
		}
		public SRCalculationGroup CreateCalculationGroup() {
			SRCalculationGroup ribbonCalculationGroup = new SRCalculationGroup();
			SRFunctionsCalculationOptionCommand calculationOptions = new SRFunctionsCalculationOptionCommand();
			ribbonCalculationGroup.Items.Add(calculationOptions);
			SRFunctionsCalculateNowCommand calculateNow = new SRFunctionsCalculateNowCommand();
			ribbonCalculationGroup.Items.Add(calculateNow);
			SRFunctionsCalculateSheetCommand calculateSheet = new SRFunctionsCalculateSheetCommand();
			ribbonCalculationGroup.Items.Add(calculateSheet);
			return ribbonCalculationGroup;
		}
		#endregion
		#region RibbonDataTab
		public SRDataTab CreateDataTab() {
			return DataTab;
		}
		protected SRDataTab DataTab {
			get {
				if(dataTab == null) {
					dataTab = new SRDataTab();
					dataTab.Groups.Add(DataSortAndFilterGroup);
					dataTab.Groups.Add(DataToolsGroup);
				}
				return dataTab;
			}
		}
		protected SRDataSortAndFilterGroup DataSortAndFilterGroup {
			get {
				if(sortFilterGroup == null)
					sortFilterGroup = CreateDataSortAndFilterGroup();
				return sortFilterGroup;
			}
		}
		protected SRDataToolsGroup DataToolsGroup {
			get {
				if(dataToolsGroup == null)
					dataToolsGroup = CreateDataToolsGroup();
				return dataToolsGroup;
			}
		}
		public SRDataToolsGroup CreateDataToolsGroup() {
			var ribbonDataToolsGroup = new SRDataToolsGroup();
			var dataValidation = new SRDataToolsDataValidationGroupCommand();
			ribbonDataToolsGroup.Items.Add(dataValidation);
			return ribbonDataToolsGroup;
		}
		public SRDataSortAndFilterGroup CreateDataSortAndFilterGroup() {
			SRDataSortAndFilterGroup ribbonSortFilterGroup = new SRDataSortAndFilterGroup();
			var sortASC = new SRDataSortAscendingCommand();
			ribbonSortFilterGroup.Items.Add(sortASC);
			var sortDESC = new SRDataSortDescendingCommand();
			ribbonSortFilterGroup.Items.Add(sortDESC);
			var dataFilterToggle = new SRDataFilterToggleCommand();
			dataFilterToggle.ShowText = true;
			ribbonSortFilterGroup.Items.Add(dataFilterToggle);
			var dataFilterClear = new SRDataFilterClearCommand();
			ribbonSortFilterGroup.Items.Add(dataFilterClear);
			var dataFilterReApply = new SRDataFilterReApplyCommand();
			ribbonSortFilterGroup.Items.Add(dataFilterReApply);
			return ribbonSortFilterGroup;
		}
		#endregion
		#region RibbonViewTab
		public SRViewTab CreateViewTab() {
			return ViewTab;
		}
		protected SRViewTab ViewTab {
			get {
				if(viewTab == null) {
					viewTab = new SRViewTab();
					viewTab.Groups.Add(ShowGroup);
					viewTab.Groups.Add(ViewGroup);
					viewTab.Groups.Add(WindowGroup);
				}
				return viewTab;
			}
		}
		protected SRViewGroup ViewGroup {
			get {
				if(viewGroup == null)
					viewGroup = CreateViewGroup();
				return viewGroup;
			}
		}
		public SRViewGroup CreateViewGroup() {
			var ribbonViewGroup = new SRViewGroup();
			var spFullScreeen = new SRFullScreenCommand();
			ribbonViewGroup.Items.Add(spFullScreeen);
			return ribbonViewGroup;
		}
		protected SRShowGroup ShowGroup {
			get {
				if(showGroup == null)
					showGroup = CreateShowGroup();
				return showGroup;
			}
		}
		public SRShowGroup CreateShowGroup() {
			SRShowGroup ribbonShowGroup = new SRShowGroup();
			SRViewShowGridlinesCommand showGridlines = new SRViewShowGridlinesCommand();
			ribbonShowGroup.Items.Add(showGridlines);
			return ribbonShowGroup;
		}
		protected SRWindowGroup WindowGroup {
			get {
				if(windowGroup == null)
					windowGroup = CreateWindowGroup();
				return windowGroup;
			}
		}
		public SRWindowGroup CreateWindowGroup() {
			var ribbonWindowGroup = new SRWindowGroup();
			var freezePanes = new SRViewFreezePanesGroupCommand();
			ribbonWindowGroup.Items.Add(freezePanes);
			return ribbonWindowGroup;
		}
		#endregion
		#region Table Tools context tab
		public SRTableDesignContextTab CreateTableDesignContextTab() {
			return TableDesignContextTab;
		}
		protected SRTableDesignContextTab TableDesignContextTab{
			get {
				if(tableDesignContextTab == null) {
					tableDesignContextTab = new SRTableDesignContextTab();
					tableDesignContextTab.Groups.Add(TablePropertiesGroup);
					tableDesignContextTab.Groups.Add(TableToolsGroup);
					tableDesignContextTab.Groups.Add(TableStyleOptionsGroup);
					tableDesignContextTab.Groups.Add(TableStyleGroup);
				}
				return tableDesignContextTab;
			}
		}
		protected SRTablePropertiesGroup TablePropertiesGroup {
			get {
				if(tablePropertiesGroup == null)
					tablePropertiesGroup = CreateTablePropertiesGroup();
				return tablePropertiesGroup;
			}
		}
		public SRTablePropertiesGroup CreateTablePropertiesGroup() {
			SRTablePropertiesGroup tablePropertiesGroup = new SRTablePropertiesGroup();
			var renameTable = new SRRenameTableCommand();
			tablePropertiesGroup.Items.Add(renameTable);
			return tablePropertiesGroup;
		}
		protected SRTableToolsGroup TableToolsGroup {
			get {
				if(tableToolsGroup == null)
					tableToolsGroup = CreateTableToolsGroup();
				return tableToolsGroup;
			}
		}
		public SRTableToolsGroup CreateTableToolsGroup() {
			SRTableToolsGroup tableToolsGroup = new SRTableToolsGroup();
			var convertToRange = new SRConvertToRangeCommand();
			tableToolsGroup.Items.Add(convertToRange);
			return tableToolsGroup;
		}
		protected SRTableStyleOptionsGroup TableStyleOptionsGroup {
			get {
				if(tableStyleOptionsGroup == null)
					tableStyleOptionsGroup = CreateTableStyleOptionsGroup();
				return tableStyleOptionsGroup;
			}
		}
		public SRTableStyleOptionsGroup CreateTableStyleOptionsGroup() {
			SRTableStyleOptionsGroup tableStyleOptionsGroup = new SRTableStyleOptionsGroup();
			var toggleHeaderRow = new SRToggleHeaderRowCommand();
			tableStyleOptionsGroup.Items.Add(toggleHeaderRow);
			var toggleTotalRow = new SRToggleTotalRowCommand();
			tableStyleOptionsGroup.Items.Add(toggleTotalRow);
			var toggleBandedColumns = new SRToggleBandedColumnsCommand();
			tableStyleOptionsGroup.Items.Add(toggleBandedColumns);
			var toggleFirstColumn = new SRToggleFirstColumnCommand();
			tableStyleOptionsGroup.Items.Add(toggleFirstColumn);
			var toggleLastColumn = new SRToggleLastColumnCommand();
			tableStyleOptionsGroup.Items.Add(toggleLastColumn);
			var toggleBandedRows = new SRToggleBandedRowsCommand();
			tableStyleOptionsGroup.Items.Add(toggleBandedRows);
			return tableStyleOptionsGroup;
		}
		protected SRTableStyleGroup TableStyleGroup {
			get {
				if(tableStyleGroup == null)
					tableStyleGroup = CreateTableStyleGroup();
				return tableStyleGroup;
			}
		}
		public SRTableStyleGroup CreateTableStyleGroup() {
			SRTableStyleGroup tableStyleGroup = new SRTableStyleGroup();
			var modifyStyle = new SRModifyTableStyleCommand();
			tableStyleGroup.Items.Add(modifyStyle);
			return tableStyleGroup;
		}
		#endregion
		#region Picture format context tab
		public SRPictureFormatContextTab CreatePictureFormatContextTab() {
			return PictureFormatContextTab;
		}
		protected SRPictureFormatContextTab PictureFormatContextTab {
			get {
				if(pictureFormatContextTab == null) {
					pictureFormatContextTab = new SRPictureFormatContextTab();
					pictureFormatContextTab.Groups.Add(PictureArrangeGroup);
				}
				return pictureFormatContextTab;
			}
		}
		protected SRPictureArrangeGroup PictureArrangeGroup {
			get {
				if(pictureArrangeGroup == null)
					pictureArrangeGroup = CreatePictureArrangeGroup();
				return pictureArrangeGroup;
			}
		}
		public SRPictureArrangeGroup CreatePictureArrangeGroup() {
			SRPictureArrangeGroup pictureArrangeGroup = new SRPictureArrangeGroup();
			pictureArrangeGroup.Items.Add(new SRArrangeBringForwardGroupCommand());
			pictureArrangeGroup.Items.Add(new SRArrangeSendBackwardGroupCommand());
			return pictureArrangeGroup;
		}
		#endregion
		#region Chart Design context tab
		public SRChartDesignContextTab CreateChartDesignContextTab() {
			return ChartDesignContextTab;
		}
		protected SRChartDesignContextTab ChartDesignContextTab {
			get {
				if(chartDesignContextTab == null) {
					chartDesignContextTab = new SRChartDesignContextTab();
					chartDesignContextTab.Groups.Add(ChartTypeGroup);
					chartDesignContextTab.Groups.Add(ChartDataGroup);
				}
				return chartDesignContextTab;
			}
		}
		protected SRChartTypeGroup ChartTypeGroup {
			get {
				if(chartTypeGroup == null)
					chartTypeGroup = CreateChartTypeGroup();
				return chartTypeGroup;
			}
		}
		public SRChartTypeGroup CreateChartTypeGroup() {
			SRChartTypeGroup group = new SRChartTypeGroup();
			group.Items.Add(new SRChangeChartTypeCommand());
			return group;
		}
		protected SRChartDataGroup ChartDataGroup {
			get {
				if(chartDataGroup == null)
					chartDataGroup = CreateChartDataGroup();
				return chartDataGroup;
			}
		}
		public SRChartDataGroup CreateChartDataGroup() {
			SRChartDataGroup group = new SRChartDataGroup();
			group.Items.Add(new SRChartSwitchRowColumnCommand());
			group.Items.Add(new SRChartSelectDataCommand());
			return group;
		}
		#endregion
		#region Chart Layout context tab
		public SRChartLayoutContextTab CreateChartLayoutContextTab() {
			return ChartLayoutContextTab;
		}
		protected SRChartLayoutContextTab ChartLayoutContextTab {
			get {
				if(chartLayoutContextTab == null) {
					chartLayoutContextTab = new SRChartLayoutContextTab();
					chartLayoutContextTab.Groups.Add(ChartLabelsGroup);
					chartLayoutContextTab.Groups.Add(ChartAxesGroup);
				}
				return chartLayoutContextTab;
			}
		}
		protected SRChartLabelsGroup ChartLabelsGroup {
			get {
				if(chartLabelsGroup == null)
					chartLabelsGroup = CreateChartLabelsGroup();
				return chartLabelsGroup;
			}
		}
		public SRChartLabelsGroup CreateChartLabelsGroup() {
			SRChartLabelsGroup group = new SRChartLabelsGroup();
			group.Items.Add(new SRChartTitleCommand());
			group.Items.Add(new SRChartAxisTitlesCommand());
			group.Items.Add(new SRChartLegendCommand());
			group.Items.Add(new SRChartDataLabelsCommand());
			return group;
		}
		protected SRChartAxesGroup ChartAxesGroup {
			get {
				if(chartAxesGroup == null)
					chartAxesGroup = CreateChartAxesGroup();
				return chartAxesGroup;
			}
		}
		public SRChartAxesGroup CreateChartAxesGroup() {
			SRChartAxesGroup group = new SRChartAxesGroup();
			group.Items.Add(new SRChartAxesCommand());
			group.Items.Add(new SRChartGridlinesCommand());
			return group;
		}
		#endregion
		#region Picture tools context tab
		public SRChartFormatContextTab CreateChartFormatContextTab() {
			return ChartFormatContextTab;
		}
		protected SRChartFormatContextTab ChartFormatContextTab {
			get {
				if(chartFormatContextTab == null) {
					chartFormatContextTab = new SRChartFormatContextTab();
					chartFormatContextTab.Groups.Add(ChartArrangeGroup);
				}
				return chartFormatContextTab;
			}
		}
		protected SRChartArrangeGroup ChartArrangeGroup {
			get {
				if(chartArrangeGroup == null)
					chartArrangeGroup = CreateChartArrangeGroup();
				return chartArrangeGroup;
			}
		}
		public SRChartArrangeGroup CreateChartArrangeGroup() {
			SRChartArrangeGroup group = new SRChartArrangeGroup();
			group.Items.Add(new SRArrangeBringForwardGroupCommand());
			group.Items.Add(new SRArrangeSendBackwardGroupCommand());
			return group;
		}
		#endregion
		#region Table Tools context tab category
		public SRTableToolsContextTabCategory CreateTableToolsContextTabCategory() {
			return TableToolsContextTabCategory;
		}
		protected SRTableToolsContextTabCategory TableToolsContextTabCategory {
			get {
				if(tableToolsContextTabCategory == null) {
					tableToolsContextTabCategory = new SRTableToolsContextTabCategory();
					tableToolsContextTabCategory.Tabs.Add(TableDesignContextTab);
				}
				return tableToolsContextTabCategory;
			}
		}
		#endregion
		#region Picture Tools context tab category
		public SRPictureToolsContextTabCategory CreatePictureToolsContextTabCategory() {
			return PictureToolsContextTabCategory;
		}
		protected SRPictureToolsContextTabCategory PictureToolsContextTabCategory {
			get {
				if(pictureToolsContextTabCategory == null) {
					pictureToolsContextTabCategory = new SRPictureToolsContextTabCategory();
					pictureToolsContextTabCategory.Tabs.Add(PictureFormatContextTab);
				}
				return pictureToolsContextTabCategory;
			}
		}
		#endregion
		#region Chart Tools context tab category
		public SRChartToolsContextTabCategory CreateChartToolsContextTabCategory() {
			return ChartToolsContextTabCategory;
		}
		protected SRChartToolsContextTabCategory ChartToolsContextTabCategory {
			get {
				if(chartToolsContextTabCategory == null) {
					chartToolsContextTabCategory = new SRChartToolsContextTabCategory();
					chartToolsContextTabCategory.Tabs.Add(ChartDesignContextTab);
					chartToolsContextTabCategory.Tabs.Add(ChartLayoutContextTab);
					chartToolsContextTabCategory.Tabs.Add(ChartFormatContextTab);
				}
				return chartToolsContextTabCategory;
			}
		}
		#endregion
	}
}
