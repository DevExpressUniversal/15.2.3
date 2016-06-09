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
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Xpf.Spreadsheet {
	public enum SpreadsheetControlStringId {
		Caption_UnprotectSheetFormTitle,
		Caption_UnprotectSheetFormPassword,
		Caption_UnprotectWorkbookFormTitle,
		Caption_UnprotectWorkbookFormPassword,
		Caption_UnprotectRangeFormTitle,
		Caption_UnprotectRangeFormPassword,
		Caption_UnprotectRangeFormDescription,
		Caption_ProtectSheetFormTitle,
		Caption_ProtectSheetFormProtectSheet,
		Caption_ProtectSheetFormPassword,
		Caption_ProtectSheetFormPermissions,
		Caption_ProtectWorkbookFormTitle,
		Caption_ProtectWorkbookFormProtectedObjects,
		Caption_ProtectWorkbookFormStructure,
		Caption_ProtectWorkbookFormWindows,
		Caption_ProtectWorkbookFormPassword,
		Caption_ConfirmPasswordFormTitle,
		Caption_ConfirmPasswordFormPassword,
		Caption_ConfirmPasswordFormDescription,
		Caption_UnhideSheetFormTitle,
		Caption_UnhideSheetFormUnhideSheet,
		Caption_RenameSheetFormTitle,
		Caption_RenameSheetFormSheetName,
		Caption_PasteSpecialFormTitle,
		Caption_PasteSpecialFormPasteAs,
		Caption_PasteSpecialFormSkipBlanks,
		Caption_HyperlinkFormTitle,
		Caption_HyperlinkFormCellReference,
		Caption_HyperlinkFormSelectPlaceInDocument,
		Caption_HyperlinkFormEMail,
		Caption_HyperlinkFormSubject,
		Caption_HyperlinkFormAddress,
		Caption_HyperlinkFormLinkTo,
		Caption_HyperlinkFormExistingFileOrWebPage,
		Caption_HyperlinkFormPlaceInThisDocument,
		Caption_HyperlinkFormEMailAddress,
		Caption_HyperlinkFormTextToDisplay,
		Caption_HyperlinkFormScreenTip,
		Caption_FormatCellsFormTitle,
		Caption_FormatCellsFormTabNumber,
		Caption_FormatCellsFormTabAlignment,
		Caption_FormatCellsFormTabFont,
		Caption_FormatCellsFormTabBorder,
		Caption_FormatCellsFormTabFill,
		Caption_FormatCellsFormTabProtection,
		Caption_FormatCellsFormAlignmentTextAlignment,
		Caption_FormatCellsFormAlignmentHorizontal,
		Caption_FormatCellsFormAlignmentVertical,
		Caption_FormatCellsFormAlignmentIndent,
		Caption_FormatCellsFormAlignmentJustifyDistributed,
		Caption_FormatCellsFormAlignmentTextControl,
		Caption_FormatCellsFormAlignmentWrapText,
		Caption_FormatCellsFormAlignmentShrinkToFit,
		Caption_FormatCellsFormAlignmentMergeCells,
		Caption_FormatCellsFormAlignmentRightToLeft,
		Caption_FormatCellsFormAlignmentTextDirection,
		Caption_FormatCellsFormBorderLine,
		Caption_FormatCellsFormBorderStyle,
		Caption_FormatCellsFormBorderColor,
		Caption_FormatCellsFormBorderPresets,
		Caption_FormatCellsFormBorderBorder,
		Caption_FormatCellsFormBorderDescription,
		Caption_FormatCellsFormFillBackgroundColor,
		Caption_FormatCellsFormFillPatternColor,
		Caption_FormatCellsFormFillPatternStyle,
		Caption_FormatCellsFormFillNoColor,
		Caption_FormatCellsFormFillSample,
		Caption_FormatCellsFormFontFamily,
		Caption_FormatCellsFormFontStyle,
		Caption_FormatCellsFormFontSize,
		Caption_FormatCellsFormFontUnderline,
		Caption_FormatCellsFormFontColor,
		Caption_FormatCellsFormFontEffects,
		Caption_FormatCellsFormFontStrikethrough,
		Caption_FormatCellsFormFontSuperscript,
		Caption_FormatCellsFormFontSubscript,
		Caption_FormatCellsFormFontPreview,
		Caption_FormatCellsFormFontSample,
		Caption_FormatCellsFormFontDescription,
		Caption_FormatCellsFormNumberCategory,
		Caption_FormatCellsFormNumberSample,
		Caption_FormatCellsFormNumberDecimalPlaces,
		Caption_FormatCellsFormNumberSymbol,
		Caption_FormatCellsFormNumberUseThousandSeparator,
		Caption_FormatCellsFormNumberNegativeNumbers,
		Caption_FormatCellsFormNumberType,
		Caption_FormatCellsFormNumberLocation,
		Caption_FormatCellsFormProtectionLocked,
		Caption_FormatCellsFormProtectionHidden,
		Caption_FormatCellsFormProtectionDescription,
		Caption_ChartTitleFormTitle,
		Caption_ChartHorizontalAxisTitleFormTitle,
		Caption_ChartVerticalAxisTitleFormTitle,
		Caption_ConditionalFormattingBetweenRuleFormAnd,
		Caption_ConditionalFormattingBetweenRuleFormWith,
		Caption_InsertTableFormDataLocation,
		Caption_InsertTableFormTableHasHeaders,
		Caption_ChartSelectDataFormTitle,
		Caption_ChartSelectDataFormDataRange,
		Caption_FindReplaceFormTitle,
		Caption_FindReplaceFormFindWhat,
		Caption_FindReplaceFormReplaceWith,
		Caption_FindReplaceFormSearchBy,
		Caption_FindReplaceFormSearchIn,
		Caption_FindReplaceFormMatchCase,
		Caption_FindReplaceFormMatchEntireCellContents,
		Caption_FindReplaceFormFindPage,
		Caption_FindReplaceFormReplacePage,
		Caption_FindReplaceFormFindButton,
		Caption_FindReplaceFormReplaceButton,
		Caption_FindReplaceFormReplaceAllButton,
		Caption_DefinedNameFormName,
		Caption_DefinedNameFormScope,
		Caption_DefinedNameFormComment,
		Caption_DefinedNameFormReference,
		Caption_NameManagerFormTitle,
		Caption_NameManagerFormNew,
		Caption_NameManagerFormEdit,
		Caption_NameManagerFormDelete,
		Caption_NameManagerFormReference,
		Caption_NameManagerFormCloseButton,
		Caption_NameManagerFormColumnName,
		Caption_NameManagerFormColumnReference,
		Caption_NameManagerFormColumnScope,
		Caption_NameManagerFormColumnComment,
		Caption_CreateDefinedNamesFromSelectionFormTitle,
		Caption_CreateDefinedNamesFromSelectionFormCreateNameValues,
		Caption_CreateDefinedNamesFromSelectionFormTopRow,
		Caption_CreateDefinedNamesFromSelectionFormLeftColumn,
		Caption_CreateDefinedNamesFromSelectionFormBottomRow,
		Caption_CreateDefinedNamesFromSelectionFormRightColumn,
		Caption_InsertSymbolFromSelectionFormTitle,
		Caption_ProtectedRangeFormName,
		Caption_ProtectedRangeFormReference,
		Caption_ProtectedRangeFormPassword,
		Caption_ProtectedRangeFormSetPassword,
		Caption_ProtectedRangeFormPermissions,
		Caption_ProtectedRangeManagerFormTitle,
		Caption_ProtectedRangeManagerFormDescription,
		Caption_ProtectedRangeManagerFormNew,
		Caption_ProtectedRangeManagerFormModify,
		Caption_ProtectedRangeManagerFormDelete,
		Caption_ProtectedRangeManagerFormColumnTitle,
		Caption_ProtectedRangeManagerFormColumnReference,
		Caption_ProtectedRangeManagerFormPermissionsDescription,
		Caption_ProtectedRangeManagerFormPermissions,
		Caption_RowHeightFormTitle,
		Caption_RowHeightFormValue,
		Caption_ColumnWidthFormTitle,
		Caption_ColumnWidthFormValue,
		Caption_DefaultColumnWidthFormTitle,
		Caption_DefaultColumnWidthFormValue,
		Caption_DocumentPropertiesFormTitle,
		Caption_DocumentPropertiesFormGeneralPage,
		Caption_DocumentPropertiesFormSummaryPage,
		Caption_DocumentPropertiesFormStatisticsPage,
		Caption_DocumentPropertiesFormCustomPage,
		Caption_DocumentPropertiesFormFileName,
		Caption_DocumentPropertiesFormFileLocation,
		Caption_DocumentPropertiesFormFileSize,
		Caption_DocumentPropertiesFormFileShortName,
		Caption_DocumentPropertiesFormFileCreated,
		Caption_DocumentPropertiesFormFileModified,
		Caption_DocumentPropertiesFormFileAccessed,
		Caption_DocumentPropertiesFormFileAttributes,
		Caption_DocumentPropertiesFormFileAttributeReadOnly,
		Caption_DocumentPropertiesFormFileAttributeHidden,
		Caption_DocumentPropertiesFormFileAttributeArchive,
		Caption_DocumentPropertiesFormFileAttributeSystem,
		Caption_DocumentPropertiesFormDocumentTitle,
		Caption_DocumentPropertiesFormSubject,
		Caption_DocumentPropertiesFormAuthor,
		Caption_DocumentPropertiesFormManager,
		Caption_DocumentPropertiesFormCompany,
		Caption_DocumentPropertiesFormCategory,
		Caption_DocumentPropertiesFormKeywords,
		Caption_DocumentPropertiesFormComments,
		Caption_DocumentPropertiesFormCreated,
		Caption_DocumentPropertiesFormModified,
		Caption_DocumentPropertiesFormAccessed,
		Caption_DocumentPropertiesFormPrinted,
		Caption_DocumentPropertiesFormLastSavedBy,
		Caption_DocumentPropertiesFormPropertyName,
		Caption_DocumentPropertiesFormPropertyType,
		Caption_DocumentPropertiesFormPropertyValue,
		Caption_DocumentPropertiesFormProperties,
		Caption_DocumentPropertiesFormYes,
		Caption_DocumentPropertiesFormNo,
		Caption_DocumentPropertiesFormColumnName,
		Caption_DocumentPropertiesFormColumnValue,
		Caption_DocumentPropertiesFormColumnType,
		Caption_OutlineSettingFormTitle,
		Caption_OutlineSettingFormDirection,
		Caption_OutlineSettingFormSummaryRows,
		Caption_OutlineSettingFormSummaryColumns,
		Caption_OutlineSettingFormAutomaticStyles,
		Caption_OutlineSettingFormCreateButton,
		Caption_OutlineSettingFormApplyStylesButton,
		Caption_OutlineGroupFormRows,
		Caption_OutlineGroupFormColumns,
		Caption_OutlineSubtotalFormSubtotal,
		Caption_OutlineSubtotalFormAtEachChangeIn,
		Caption_OutlineSubtotalFormUseFunction,
		Caption_OutlineSubtotalFormAddSubtotalTo,
		Caption_OutlineSubtotalFormReplaceCurrentSubtotal,
		Caption_OutlineSubtotalFormPageBreakBetweenGroups,
		Caption_OutlineSubtotalFormSummaryBelowData,
		Caption_OutlineSubtotalFormRemoveAll,
		Caption_GenericFilterFormTitle,
		Caption_GenericFilterFormShowRows,
		Caption_GenericFilterFormOperationAnd,
		Caption_GenericFilterFormOperationOr,
		Caption_GenericFilterFormDescriptionSingleCharacter,
		Caption_GenericFilterFormDescriptionSeriesCharacters,
		Caption_Top10FilterFormTitle,
		Caption_Top10FilterFormShow,
		Caption_SimpleFilterFormTitle,
		Msg_ErrorEmptySimpleFilter,
		Caption_SimpleFilterCheckAll,
		Caption_SimpleFilterUncheckAll,
		Caption_PatternStyleNone,
		Caption_PatternStyleSolid,
		Caption_PatternStyleMediumGray,
		Caption_PatternStyleDarkGray,
		Caption_PatternStyleLightGray,
		Caption_PatternStyleDarkHorizontal,
		Caption_PatternStyleDarkVertical,
		Caption_PatternStyleDarkDown,
		Caption_PatternStyleDarkUp,
		Caption_PatternStyleDarkTrellis,
		Caption_PatternStyleLightHorizontal,
		Caption_PatternStyleLightVertical,
		Caption_PatternStyleLightDown,
		Caption_PatternStyleLightUp,
		Caption_PatternStyleLightGrid,
		Caption_PatternStyleLightTrellis,
		Caption_PatternStyleGray125,
		Caption_PatternStyleGray0625,
		Caption_PageSetupFormTitle,
		Caption_PageSetupFormTabPage,
		Caption_PageSetupFormTabMargins,
		Caption_PageSetupFormTabHeaderFooter,
		Caption_PageSetupFormTabSheet,
		Caption_PageSetupFormPageOrienation,
		Caption_PageSetupFormPagePortraitOrienation,
		Caption_PageSetupFormPageLandscapeOrienation,
		Caption_PageSetupFormPageScaling,
		Caption_PageSetupFormPageAdjustTo,
		Caption_PageSetupFormPageFitTo,
		Caption_PageSetupFormPageNormalSizeScaling,
		Caption_PageSetupFormPagePagesWideBy,
		Caption_PageSetupFormPageTall,
		Caption_PageSetupFormPagePaperSize,
		Caption_PageSetupFormPagePrintQuality,
		Caption_PageSetupFormPageFirstPageNumber,
		Caption_PageSetupFormPagePrint,
		Caption_PageSetupFormPagePrintPreview,
		Caption_PageSetupFormSheetPrintArea,
		Caption_PageSetupFormSheetPrint,
		Caption_PageSetupFormSheetPageOrder,
		Caption_PageSetupFormSheetComments,
		Caption_PageSetupFormSheetCellErrorAs,
		Caption_PageSetupFormSheetGridlines,
		Caption_PageSetupFormSheetBlackAndWhite,
		Caption_PageSetupFormSheetDraftQuality,
		Caption_PageSetupFormSheetRowAndColumnHeadings,
		Caption_PageSetupFormSheetDownThanOver,
		Caption_PageSetupFormSheetOverThanDown,
		Caption_PageSetupFormMarginTop,
		Caption_PageSetupFormMarginBottom,
		Caption_PageSetupFormMarginLeft,
		Caption_PageSetupFormMarginRight,
		Caption_PageSetupFormMarginHeader,
		Caption_PageSetupFormMarginFooter,
		Caption_PageSetupFormMarginHorizontally,
		Caption_PageSetupFormMarginVertically,
		Caption_PageSetupFormMarginCenterOnPage,
		Caption_PageSetupFormHeaderFooterHeader,
		Caption_PageSetupFormHeaderFooterFooter,
		Caption_PageSetupFormHeaderFooterCustomHeaderFooter,
		Caption_PageSetupFormHeaderFooterDifferentOddAndEvenPages,
		Caption_PageSetupFormHeaderFooterDifferentFirstPage,
		Caption_PageSetupFormHeaderFooterScaleWithDocument,
		Caption_PageSetupFormHeaderFooterAlignWithPageMargins,
		Caption_HeaderFooterFormTitle,
		Caption_HeaderFooterTabHeaderFooter,
		Caption_HeaderFooterTabFirstHeaderFooter,
		Caption_HeaderFooterTabOddHeaderFooter,
		Caption_HeaderFooterTabEvenHeaderFooter,
		Caption_HeaderFooterLeftHeader,
		Caption_HeaderFooterCenterHeader,
		Caption_HeaderFooterRightHeader,
		Caption_HeaderFooterLeftFooter,
		Caption_HeaderFooterCenterFooter,
		Caption_HeaderFooterRightFooter,
		Caption_HeaderFooterDescription,
		Caption_HeaderFooterInsertPageNumber,
		Caption_HeaderFooterInsertNumberOfPages,
		Caption_HeaderFooterInsertDate,
		Caption_HeaderFooterInsertTime,
		Caption_HeaderFooterInsertFilePath,
		Caption_HeaderFooterInsertFileName,
		Caption_HeaderFooterInsertSheetName,
		Caption_DataValidationFormTitle,
		Caption_DataValidationFormClearAllBtn,
		Caption_DataValidationSettingsPage,
		Caption_DataValidationInputMessagePage,
		Caption_DataValidationErrorAlertPage,
		Caption_DataValidationCriteria,
		Caption_DataValidationAllow,
		Caption_DataValidationData,
		Caption_DataValidationIgnoreBlank,
		Caption_DataValidationInCellDropdown,
		Caption_DataValidationApplyToAllCells,
		Caption_DataValidationInputMessageCheckbox,
		Caption_DataValidationInputMessageText,
		Caption_DataValidationInputMessageTitle,
		Caption_DataValidationInputMessage,
		Caption_DataValidationErrorMessageCheckbox,
		Caption_DataValidationErrorMessageText,
		Caption_DataValidationErrorMessageStyle,
		Caption_DataValidationErrorMessage,
		Caption_InsertPivotTableFormTitle,
		Caption_InsertChangeDataSourcePivotTableDataToAnalyze,
		Caption_InsertChangeDataSourcePivotTableTableRange,
		Caption_InsertMovePivotTablePlaceForPivotTableReport,
		Caption_InsertMovePivotTableNewWorksheet,
		Caption_InsertMovePivotTableExistingWorksheet,
		Caption_InsertMovePivotTableLocation,
		Caption_MoveOrCopySheetFormTitle,
		Caption_MoveOrCopySheetMoveSelectedSheetBefore,
		Caption_MoveOrCopySheetCreateCopy,
		Caption_OptionsPivotTableFormTitle,
		Caption_OptionsPivotTableName,
		Caption_OptionsPivotTableTabLayoutAndFormat,
		Caption_OptionsPivotTableTabTotalsAndFilters,
		Caption_OptionsPivotTableTabDisplay,
		Caption_OptionsPivotTableTabPrinting,
		Caption_OptionsPivotTableTabData,
		Caption_OptionsPivotTableTabAltText,
		Caption_OptionsPivotTableLayoutAndFormatLayout,
		Caption_OptionsPivotTableLayoutAndFormatMergeAndCenterCells,
		Caption_OptionsPivotTableLayoutAndFormatIndentRowLabels,
		Caption_OptionsPivotTableLayoutAndFormatCharacters,
		Caption_OptionsPivotTableLayoutAndFormatReportFilterArea,
		Caption_OptionsPivotTableLayoutAndFormatReportFilterFields,
		Caption_OptionsPivotTableLayoutAndFormatColumn,
		Caption_OptionsPivotTableLayoutAndFormatRow,
		Caption_OptionsPivotTableLayoutAndFormatFormat,
		Caption_OptionsPivotTableLayoutAndFormatErrorValuesShow,
		Caption_OptionsPivotTableLayoutAndFormatEmptyCellsShow,
		Caption_OptionsPivotTableLayoutAndFormatAutofitColumnWidths,
		Caption_OptionsPivotTableLayoutAndFormatPreserveCellFormatting,
		Caption_OptionsPivotTableTotalsAndFiltersGrandTotals,
		Caption_OptionsPivotTableTotalsAndFiltersRowGrandTotals,
		Caption_OptionsPivotTableTotalsAndFiltersColumnsGrandTotals,
		Caption_OptionsPivotTableTotalsAndFiltersFilters,
		Caption_OptionsPivotTableTotalsAndFiltersSubtotalPageItems,
		Caption_OptionsPivotTableTotalsAndFiltersMultipleFilters,
		Caption_OptionsPivotTableTotalsAndFiltersSorting,
		Caption_OptionsPivotTableTotalsAndFiltersUseCustomList,
		Caption_OptionsPivotTableDisplayDisplay,
		Caption_OptionsPivotTableDisplayExpandCollapsButtons,
		Caption_OptionsPivotTableDisplayContextualTooltips,
		Caption_OptionsPivotTableDisplayPropertiesInTooltips,
		Caption_OptionsPivotTableDisplayFieldCaptionsFilterDropDowns,
		Caption_OptionsPivotTableDisplayClassicPivotTableLayout,
		Caption_OptionsPivotTableDisplayShowValuesRow,
		Caption_OptionsPivotTableDisplayItemsNoDataRows,
		Caption_OptionsPivotTableDisplayItemsNoDataColumns,
		Caption_OptionsPivotTableDisplayItemLabelsWhenNoFieldsInValuesArea,
		Caption_OptionsPivotTableDisplayFieldList,
		Caption_OptionsPivotTableDisplaySortAToZ,
		Caption_OptionsPivotTableDisplaySortInDataSourceOrder,
		Caption_OptionsPivotTablePrintingPrint,
		Caption_OptionsPivotTablePrintingExpandCollapseButtonDisplayedPivotTable,
		Caption_OptionsPivotTablePrintingRepeatRowLabels,
		Caption_OptionsPivotTablePrintingSetPrintTitles,
		Caption_OptionsPivotTableDataPivotTableData,
		Caption_OptionsPivotTableDataSaveSourceData,
		Caption_OptionsPivotTableDataEnableShowDetails,
		Caption_OptionsPivotTableDataRefreshData,
		Caption_OptionsPivotTableDataRetainItemsDeleted,
		Caption_OptionsPivotTableDataNumberOfItemsToRetainPerField,
		Caption_OptionsPivotTableDataWhatIfAnalysis,
		Caption_OptionsPivotTableDataEnableCellEditing,
		Caption_OptionsPivotTableAltTextTitle,
		Caption_OptionsPivotTableAltTextDescription,
		Caption_OptionsPivotTableAltTextDescriptionText,
		Caption_MovePivotTableFormTitle,
		Caption_ChangeDataSourcePivotTableFormTitle,
		Caption_FieldAndDataFieldSettingsPivotTableSourceName,
		Caption_FieldAndDataFieldSettingsPivotTableCustomName,
		Caption_FieldSettingsPivotTableFormTitle,
		Caption_FieldSettingsPivotTableTabSubtotalsAndFilters,
		Caption_FieldSettingsPivotTableTabLayoutAndPrint,
		Caption_FieldSettingsPivotTableSubtotals,
		Caption_FieldSettingsPivotTableSubtotalAutomatic,
		Caption_FieldSettingsPivotTableSubtotalNone,
		Caption_FieldSettingsPivotTableSubtotalCustom,
		Caption_FieldSettingsPivotTableSelectSubtotalFunctions,
		Caption_FieldSettingsPivotTableFilter,
		Caption_FieldSettingsPivotTableNewItemsManualFilter,
		Caption_FieldSettingsPivotTableLayout,
		Caption_FieldSettingsPivotTableItemLabelsOutlineForm,
		Caption_FieldSettingsPivotTableCompactForm,
		Caption_FieldSettingsPivotTableSubtotalTop,
		Caption_FieldSettingsPivotTableItemLabelsTabularForm,
		Caption_FieldSettingsPivotTableRepeatItemLabels,
		Caption_FieldSettingsPivotTableInsertBlankLine,
		Caption_FieldSettingsPivotTableShowItemsWithNoData,
		Caption_FieldSettingsPivotTablePrint,
		Caption_FieldSettingsPivotTableInsertPageBreak,
		Caption_DataFieldSettingsPivotTableFormTitle,
		Caption_DataFieldSettingsPivotTableTabSummarizeValuesBy,
		Caption_DataFieldSettingsPivotTableTabShowValuesAs,
		Caption_DataFieldSettingsPivotTableSummarizeValueFieldBy,
		Caption_DataFieldSettingsPivotTableChooseTheTypeOfCalculation,
		Caption_DataFieldSettingsPivotTableShowValuesAs,
		Caption_DataFieldSettingsPivotTableBaseField,
		Caption_DataFieldSettingsPivotTableBaseItem,
		Caption_PivotTableFieldsFormTitle,
		Caption_PivotTableFieldsFormChooseFields,
		Caption_PivotTableFieldsFormDragFields,
		Caption_PivotTableFieldsFilters,
		Caption_PivotTableFieldsColumns,
		Caption_PivotTableFieldsRows,
		Caption_PivotTableFieldsValues,
		Caption_PivotTableFieldsDeferLayoutUpdate,
		Caption_PivotTableFieldsButtonUpdate,
		Caption_PivotTableFieldFilterItemsFormTitle,
		Caption_PivotTablePageFieldFilterItemsSelectMultipleItems,
		Caption_PivotTableValueFilterFormTitle,
		Caption_PivotTableValueFilterFormShowItemsForWhich,
		Caption_PivotTableLabelDateValueFilterFormAnd,
		Caption_PivotTableLabelFilterFormTitle,
		Caption_PivotTableLabelFilterFormShowItemsForWhichTheLabel,
		Caption_PivotTableTop10FilterFormTitle,
		Caption_PivotTableTop10FilterFormShow,
		Caption_PivotTableTop10FilterFormBy,
		Caption_PivotTableDateFilterFormTitle,
		Caption_PivotTableLabelFilterFormShowItemsForWhichTheDate,
		Caption_PivotTableShowValuesAsFormTitle,
		Caption_PivotTableShowValuesAsFormBaseField,
		Caption_PivotTableShowValuesAsFormBaseItem
	}
	public class SpreadsheetStringIdConverter : StringIdConverter<XtraSpreadsheetStringId> {
		static SpreadsheetStringIdConverter() {
			XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFile);
		}
		protected override XtraLocalizer<XtraSpreadsheetStringId> Localizer { get { return XtraSpreadsheetLocalizer.Active; } }
	}
	public class SpreadsheetControlStringIdConverter : StringIdConverter<SpreadsheetControlStringId> {
		static SpreadsheetControlStringIdConverter() {
			XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_UnprotectSheetFormPassword);
		}
		protected override XtraLocalizer<SpreadsheetControlStringId> Localizer { get { return XpfSpreadsheetLocalizer.Active; } }
	}
}
namespace DevExpress.Xpf.Spreadsheet.Localization {
	public class XpfSpreadsheetLocalizer : DXLocalizer<SpreadsheetControlStringId> {
		static XpfSpreadsheetLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SpreadsheetControlStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(SpreadsheetControlStringId.Caption_UnprotectSheetFormTitle, "Unprotect Sheet");
			AddString(SpreadsheetControlStringId.Caption_UnprotectSheetFormPassword, "Password:");
			AddString(SpreadsheetControlStringId.Caption_UnprotectWorkbookFormTitle, "Unprotect Workbook");
			AddString(SpreadsheetControlStringId.Caption_UnprotectWorkbookFormPassword, "Password:");
			AddString(SpreadsheetControlStringId.Caption_UnprotectRangeFormTitle, "Unlock Range");
			AddString(SpreadsheetControlStringId.Caption_UnprotectRangeFormPassword, "Password:");
			AddString(SpreadsheetControlStringId.Caption_UnprotectRangeFormDescription, "A cell you are trying to change is password protected.");
			AddString(SpreadsheetControlStringId.Caption_ProtectSheetFormTitle, "Protect Sheet");
			AddString(SpreadsheetControlStringId.Caption_ProtectSheetFormProtectSheet, "Protect worksheet and contents of locked cells");
			AddString(SpreadsheetControlStringId.Caption_ProtectSheetFormPassword, "Password to unprotect sheet:");
			AddString(SpreadsheetControlStringId.Caption_ProtectSheetFormPermissions, "Allow all users of this worksheet to:");
			AddString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormTitle, "Protect Structure and Windows");
			AddString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormProtectedObjects, "Protect workbook for");
			AddString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormStructure, "Structure");
			AddString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormWindows, "Windows");
			AddString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormPassword, "Password (optional):");
			AddString(SpreadsheetControlStringId.Caption_ConfirmPasswordFormTitle, "Confirm Password");
			AddString(SpreadsheetControlStringId.Caption_ConfirmPasswordFormPassword, "Reenter password to proceed:");
			AddString(SpreadsheetControlStringId.Caption_ConfirmPasswordFormDescription, "Caution: If you lose or forget the password, it cannot be recovered. It is advisable to keep a list of passwords and their corresponding workbook and sheet names in a safe place. (Remember that passwords are case-sensitive.)");
			AddString(SpreadsheetControlStringId.Caption_UnhideSheetFormTitle, "Unhide");
			AddString(SpreadsheetControlStringId.Caption_UnhideSheetFormUnhideSheet, "Unhide sheet:");
			AddString(SpreadsheetControlStringId.Caption_RenameSheetFormTitle, "Rename Sheet");
			AddString(SpreadsheetControlStringId.Caption_RenameSheetFormSheetName, "Sheet Name:");
			AddString(SpreadsheetControlStringId.Caption_PasteSpecialFormTitle, "Paste Special");
			AddString(SpreadsheetControlStringId.Caption_PasteSpecialFormPasteAs, "Paste As:");
			AddString(SpreadsheetControlStringId.Caption_PasteSpecialFormSkipBlanks, "Skip blanks");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormTitle, "Insert Hyperlink");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormCellReference, "Type the cell reference:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormSelectPlaceInDocument, "Or select a place in this document:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormEMail, "E-mail address:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormSubject, "Subject:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormAddress, "Address:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormLinkTo, "Link to:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormExistingFileOrWebPage, "Exisisting File or Web Page");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormPlaceInThisDocument, "Place in This Document");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormEMailAddress, "E-mail Address");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormTextToDisplay, "Text to display:");
			AddString(SpreadsheetControlStringId.Caption_HyperlinkFormScreenTip, "ScreenTip:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTitle, "Format Cells");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabNumber, "Number");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabAlignment, "Alignment");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabFont, "Font");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabBorder, "Border");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabFill, "Fill");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormTabProtection, "Protection");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentTextAlignment, "Text Alignment");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentHorizontal, "Horizontal:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentVertical, "Vertical:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentIndent, "Indent:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentJustifyDistributed, "Justify distributed");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentTextControl, "Text control");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentWrapText, "Wrap Text");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentShrinkToFit, "Shrink to fit");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentMergeCells, "Merge Cells");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentRightToLeft, "Right-To-Left");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormAlignmentTextDirection, "Text direction:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderLine, "Line");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderStyle, "Style:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderColor, "Color:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderPresets, "Presets");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderBorder, "Border");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormBorderDescription, "The selected border style can be applied by clicking the presets, preview diagram or the buttons above.");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFillBackgroundColor, "Background Color:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFillPatternColor, "Pattern Color:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFillPatternStyle, "Pattern Style:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFillNoColor, "No Color");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFillSample, "Sample");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontFamily, "Font:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontStyle, "Font style:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontSize, "Font size:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontUnderline, "Underline:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontColor, "Color:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontEffects, "Effects");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontStrikethrough, "Strikethrough");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontSuperscript, "Superscript");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontSubscript, "Subscript");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontPreview, "Preview");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontSample, "AaBbCcYyZz");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormFontDescription, "This is a TrueType font. The same font will be used on both your printer and your screen.");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberCategory, "Category:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberSample, "Sample");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberDecimalPlaces, "Decimal places:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberSymbol, "Symbol:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberUseThousandSeparator, "Use 1000 Separator");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberNegativeNumbers, "Negative numbers:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberType, "Type:");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormNumberLocation, "Locate (location):");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormProtectionLocked, "Locked");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormProtectionHidden, "Hidden");
			AddString(SpreadsheetControlStringId.Caption_FormatCellsFormProtectionDescription, "Locking cell or hiding formulas has no effect until you protect the worksheet (Review tab, Changes group, Protect Sheet button).");
			AddString(SpreadsheetControlStringId.Caption_ChartTitleFormTitle, "Change Chart Title");
			AddString(SpreadsheetControlStringId.Caption_ChartHorizontalAxisTitleFormTitle, "Change Horizontal Axis Title");
			AddString(SpreadsheetControlStringId.Caption_ChartVerticalAxisTitleFormTitle, "Change Vertical Axis Title");
			AddString(SpreadsheetControlStringId.Caption_ConditionalFormattingBetweenRuleFormAnd, "and");
			AddString(SpreadsheetControlStringId.Caption_ConditionalFormattingBetweenRuleFormWith, "with");
			AddString(SpreadsheetControlStringId.Caption_InsertTableFormDataLocation, "Where is the data for your table?");
			AddString(SpreadsheetControlStringId.Caption_InsertTableFormTableHasHeaders, "My table has headers");
			AddString(SpreadsheetControlStringId.Caption_ChartSelectDataFormTitle, "Select Data Source");
			AddString(SpreadsheetControlStringId.Caption_ChartSelectDataFormDataRange, "Chart data range:");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormTitle, "Find and Replace");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormFindWhat, "Find what:");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormReplaceWith, "Replace with:");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormSearchBy, "Search:");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormSearchIn, "Look in:");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormMatchCase, "Match case");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormMatchEntireCellContents, "Match entire cell contents");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormFindPage, "Fin_d");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormReplacePage, "Re_place");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormFindButton, "_Find Next");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormReplaceButton, "_Replace");
			AddString(SpreadsheetControlStringId.Caption_FindReplaceFormReplaceAllButton, "Replace _All");
			AddString(SpreadsheetControlStringId.Caption_DefinedNameFormName, "Name:");
			AddString(SpreadsheetControlStringId.Caption_DefinedNameFormScope, "Scope:");
			AddString(SpreadsheetControlStringId.Caption_DefinedNameFormComment, "Comment:");
			AddString(SpreadsheetControlStringId.Caption_DefinedNameFormReference, "Refers to:");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormTitle, "Name Manager");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormNew, "_New...");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormEdit, "_Edit...");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormDelete, "_Delete");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormReference, "Refers to:");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormCloseButton, "Close");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormColumnName, "Name");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormColumnReference, "Refers to");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormColumnScope, "Scope");
			AddString(SpreadsheetControlStringId.Caption_NameManagerFormColumnComment, "Comment");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormTitle, "Create Names from Selection");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormCreateNameValues, "Create names from values in the:");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormTopRow, "Top row");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormLeftColumn, "Left column");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormBottomRow, "Bottom row");
			AddString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormRightColumn, "Right column");
			AddString(SpreadsheetControlStringId.Caption_InsertSymbolFromSelectionFormTitle, "Symbol");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeFormName, "Title:");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeFormReference, "Refers to cells:");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeFormPassword, "Range password:");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeFormSetPassword, "Set _Password");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeFormPermissions, "P_ermissions...");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormTitle, "Allow Users to Edit Ranges");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormDescription, "Ranges unlocked by a password when sheet is protected:");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormNew, "_New...");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormModify, "_Modify...");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormDelete, "_Delete");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormColumnTitle, "Title");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormColumnReference, "Refers to cells");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormPermissionsDescription, "Specify who may edit the range without a password:");
			AddString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormPermissions, "_Permissions...");
			AddString(SpreadsheetControlStringId.Caption_RowHeightFormTitle, "Row Height");
			AddString(SpreadsheetControlStringId.Caption_RowHeightFormValue, "Row height:");
			AddString(SpreadsheetControlStringId.Caption_ColumnWidthFormTitle, "Column Width");
			AddString(SpreadsheetControlStringId.Caption_ColumnWidthFormValue, "Column width:");
			AddString(SpreadsheetControlStringId.Caption_DefaultColumnWidthFormTitle, "Standard Width");
			AddString(SpreadsheetControlStringId.Caption_DefaultColumnWidthFormValue, "Standard column width:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormTitle, "Properties");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormGeneralPage, "General");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormSummaryPage, "Summary");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormStatisticsPage, "Statistics");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormCustomPage, "Custom");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileName, "File name:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileLocation, "Location:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileSize, "Size:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileShortName, "MS-DOS name:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileCreated, "Created:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileModified, "Modified:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAccessed, "Accessed:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAttributes, "Attributes:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAttributeReadOnly, "Read only");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAttributeHidden, "Hidden");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAttributeArchive, "Archive");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormFileAttributeSystem, "System");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormDocumentTitle, "Title:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormSubject, "Subject:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormAuthor, "Author:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormManager, "Manager:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormCompany, "Company:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormCategory, "Category:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormKeywords, "Keywords:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormComments, "Comments:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormCreated, "Created:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormModified, "Modified:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormAccessed, "Accessed:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormPrinted, "Printed:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormLastSavedBy, "Last saved by:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormPropertyName, "Name:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormPropertyType, "Type:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormPropertyValue, "Value:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormProperties, "Properties:");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormYes, "Yes");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormNo, "No");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormColumnName, "Name");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormColumnValue, "Value");
			AddString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormColumnType, "Type");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormTitle, "Settings");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormDirection, "Direction");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormSummaryRows, "Summary rows below detail");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormSummaryColumns, "Summary columns to right of detail");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormAutomaticStyles, "Automatic styles");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormCreateButton, "Create");
			AddString(SpreadsheetControlStringId.Caption_OutlineSettingFormApplyStylesButton, "Apply Styles");
			AddString(SpreadsheetControlStringId.Caption_OutlineGroupFormRows, "Rows");
			AddString(SpreadsheetControlStringId.Caption_OutlineGroupFormColumns, "Columns");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormSubtotal, "Subtotal");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormAtEachChangeIn, "At Each Change In:");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormUseFunction, "Use Function:");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormAddSubtotalTo, "Add Subtotal To:");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormReplaceCurrentSubtotal, "Replace Current Subtotal");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormPageBreakBetweenGroups, "Page Break Between Groups");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormSummaryBelowData, "Summary Below Data");
			AddString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormRemoveAll, "Remove All");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormTitle, "Custom AutoFilter");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormShowRows, "Show Rows Where:");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormOperationAnd, "And");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormOperationOr, "Or");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormDescriptionSingleCharacter, "Use ? to represent any single character");
			AddString(SpreadsheetControlStringId.Caption_GenericFilterFormDescriptionSeriesCharacters, "Use * to represent any series of characters");
			AddString(SpreadsheetControlStringId.Caption_Top10FilterFormTitle, "Top 10 AutoFilter");
			AddString(SpreadsheetControlStringId.Caption_Top10FilterFormShow, "Show");
			AddString(SpreadsheetControlStringId.Caption_SimpleFilterFormTitle, "AutoFilter");
			AddString(SpreadsheetControlStringId.Msg_ErrorEmptySimpleFilter, "Cannot apply an empty filter. Check any item in the list.");
			AddString(SpreadsheetControlStringId.Caption_SimpleFilterCheckAll, "Check All");
			AddString(SpreadsheetControlStringId.Caption_SimpleFilterUncheckAll, "Uncheck All");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleNone, "None");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleSolid, "Solid");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleMediumGray, "Medium Gray");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkGray, "Dark Gray");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightGray, "Light Gray");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkHorizontal, "Dark Horizontal");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkVertical, "Dark Vertical");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkDown, "Dark Down");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkUp, "Dark Up");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleDarkTrellis, "Dark Trellis");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightHorizontal, "Light Horizontal");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightVertical, "Light Vertical");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightDown, "Light Down");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightUp, "Light Up");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightGrid, "Light Grid");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleLightTrellis, "Light Trellis");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleGray125, "Gray125");
			AddString(SpreadsheetControlStringId.Caption_PatternStyleGray0625, "Gray0625");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormTitle, "Page Setup");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormTabPage, "Page");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormTabMargins, "Margins");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormTabHeaderFooter, "Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormTabSheet, "Sheet");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageOrienation, "Orientation");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePortraitOrienation, "Portrait");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageLandscapeOrienation, "Landscape");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageScaling, "Scaling");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageAdjustTo, "Adjust to:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageFitTo, "Fit to:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageNormalSizeScaling, "% normal size");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePagesWideBy, "page(s) wide by");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageTall, "tall");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePaperSize, "Paper size:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePrintQuality, "Print quality:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPageFirstPageNumber, "First page number:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePrint, "Print...");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormPagePrintPreview, "Print preview");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetPrintArea, "Print area:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetPrint, "Print");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetPageOrder, "Page order");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetComments, "Comments:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetCellErrorAs, "Cell errors as:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetGridlines, "Gridlines");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetBlackAndWhite, "Black and white");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetDraftQuality, "Draft quality");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetRowAndColumnHeadings, "Row and column headings");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetDownThanOver, "Down, then over");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormSheetOverThanDown, "Over, then down");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginTop, "Top:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginBottom, "Bottom:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginLeft, "Left:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginRight, "Right:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginHeader, "Header:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginFooter, "Footer:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginHorizontally, "Horizontally");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginVertically, "Vertically");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormMarginCenterOnPage, "Center on page");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterHeader, "Header:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterFooter, "Footer:");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterCustomHeaderFooter, "Custom Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterDifferentOddAndEvenPages, "Different odd and even pages");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterDifferentFirstPage, "Different first page");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterScaleWithDocument, "Scale with document");
			AddString(SpreadsheetControlStringId.Caption_PageSetupFormHeaderFooterAlignWithPageMargins, "Align with page margins");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterFormTitle, "Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterTabHeaderFooter, "Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterTabFirstHeaderFooter, "First Page Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterTabOddHeaderFooter, "Odd Page Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterTabEvenHeaderFooter, "Even Page Header/Footer");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterLeftHeader, "Left header:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterCenterHeader, "Center header:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterRightHeader, "Right header:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterLeftFooter, "Left footer:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterCenterFooter, "Center footer:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterRightFooter, "Right footer:");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterDescription, "To insert a page number, date, time, file path, filename, or tab name: position the insertion point in the edit box, then choose the appropriate button.");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertPageNumber, "Insert Page Number");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertNumberOfPages, "Insert Number of Pages");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertDate, "Insert Date");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertTime, "Insert Time");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertFilePath, "Insert File Path");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertFileName, "Insert File Name");
			AddString(SpreadsheetControlStringId.Caption_HeaderFooterInsertSheetName, "Insert Sheet Name");
			AddString(SpreadsheetControlStringId.Caption_DataValidationFormTitle, "Data Validation");
			AddString(SpreadsheetControlStringId.Caption_DataValidationFormClearAllBtn, "Clear All");
			AddString(SpreadsheetControlStringId.Caption_DataValidationSettingsPage, "Settings");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInputMessagePage, "Input Message");
			AddString(SpreadsheetControlStringId.Caption_DataValidationErrorAlertPage, "Error Alert");
			AddString(SpreadsheetControlStringId.Caption_DataValidationCriteria, "Validation criteria");
			AddString(SpreadsheetControlStringId.Caption_DataValidationAllow, "Allow:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationData, "Data:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationIgnoreBlank, "Ignore blank");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInCellDropdown, "In-cell dropdown");
			AddString(SpreadsheetControlStringId.Caption_DataValidationApplyToAllCells, "Apply these changes to all other cells with the same settings");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInputMessageCheckbox, "Show input message when cell is selected");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInputMessageText, "When cell is selected, show this input message:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInputMessageTitle, "Title:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationInputMessage, "Input message:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationErrorMessageCheckbox, "Show error alert after invalid data is entered");
			AddString(SpreadsheetControlStringId.Caption_DataValidationErrorMessageText, "When user enters invalid data, show this error alert:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationErrorMessageStyle, "Style:");
			AddString(SpreadsheetControlStringId.Caption_DataValidationErrorMessage, "Error message:");
			AddString(SpreadsheetControlStringId.Caption_InsertPivotTableFormTitle, "Create PivotTable");
			AddString(SpreadsheetControlStringId.Caption_InsertChangeDataSourcePivotTableDataToAnalyze, "Choose the data that you want to analyze");
			AddString(SpreadsheetControlStringId.Caption_InsertChangeDataSourcePivotTableTableRange, "Table/Range:");
			AddString(SpreadsheetControlStringId.Caption_InsertMovePivotTablePlaceForPivotTableReport, "Choose where you want the PivotTable report to be placed");
			AddString(SpreadsheetControlStringId.Caption_InsertMovePivotTableNewWorksheet, "New Worksheet");
			AddString(SpreadsheetControlStringId.Caption_InsertMovePivotTableExistingWorksheet, "Existing Worksheet");
			AddString(SpreadsheetControlStringId.Caption_InsertMovePivotTableLocation, "Location:");
			AddString(SpreadsheetControlStringId.Caption_MoveOrCopySheetFormTitle, "Move or Copy");
			AddString(SpreadsheetControlStringId.Caption_MoveOrCopySheetMoveSelectedSheetBefore, "Move selected sheets before sheet:");
			AddString(SpreadsheetControlStringId.Caption_MoveOrCopySheetCreateCopy, "Create a copy");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableFormTitle, "PivotTable Options");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableName, "PivotTable Name:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabLayoutAndFormat, "Layout & Format");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabTotalsAndFilters, "Totals & Filters");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabDisplay, "Display");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabPrinting, "Printing");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabData, "Data");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTabAltText, "Alt Text");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatLayout, "Layout");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatMergeAndCenterCells, "Merge and center cells with labels");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatIndentRowLabels, "When in compact form indent row labels:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatCharacters, "character(s)");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatReportFilterArea, "Display fields in report filter area:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatReportFilterFields, "Report filter fields per");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatColumn, "column:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatRow, "row:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatFormat, "Format");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatErrorValuesShow, "For error values show:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatEmptyCellsShow, "For empty cells show:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatAutofitColumnWidths, "Autofit column widths on update");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableLayoutAndFormatPreserveCellFormatting, "Preserve cell formatting on update");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersGrandTotals, "Grand Totals");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersRowGrandTotals, "Show grand totals for rows");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersColumnsGrandTotals, "Show grand totals for columns");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersFilters, "Filters");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersSubtotalPageItems, "Subtotal filtered page items");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersMultipleFilters, "Allow multiple filters per field");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersSorting, "Sorting");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableTotalsAndFiltersUseCustomList, "Use Custom Lists when sorting");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayDisplay, "Display");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayExpandCollapsButtons, "Show expand/collapse buttons");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayContextualTooltips, "Show contextual tooltips");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayPropertiesInTooltips, "Show properties in tooltips");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayFieldCaptionsFilterDropDowns, "Display field captions and filter drop downs");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayClassicPivotTableLayout, "Classic PivotTable layout (enables dragging of fields in the grid)");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayShowValuesRow, "Show the Values row");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayItemsNoDataRows, "Show items with no data on rows");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayItemsNoDataColumns, "Show items with no data on columns");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayItemLabelsWhenNoFieldsInValuesArea, "Display item labels when no fields are in the values area");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplayFieldList, "Field List");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplaySortAToZ, "Sort A to Z");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDisplaySortInDataSourceOrder, "Sort in data source order");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTablePrintingPrint, "Print");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTablePrintingExpandCollapseButtonDisplayedPivotTable, "Print expand/collapse buttons when displayed on PivotTable");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTablePrintingRepeatRowLabels, "Repeat row labels on each printed page");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTablePrintingSetPrintTitles, "Set print titles");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataPivotTableData, "PivotTable Data");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataSaveSourceData, "Save source data with file");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataEnableShowDetails, "Enable show details");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataRefreshData, "Refresh data when opening the file");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataRetainItemsDeleted, "Retain items deleted from the data source");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataNumberOfItemsToRetainPerField, "Number of items to retain per field:");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataWhatIfAnalysis, "What-If Analysis");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableDataEnableCellEditing, "Enable cell editing in the values area");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableAltTextTitle, "Title");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableAltTextDescription, "Description");
			AddString(SpreadsheetControlStringId.Caption_OptionsPivotTableAltTextDescriptionText, "Titles and descriptions provide alternative, text-based representations of the information contained in tables. This information is useful for people with vision or cognitive impairments who may not be able to see or understand the table.A title can be read to a person with a disability and is used to determine whether they wish to hear the description of the content.");
			AddString(SpreadsheetControlStringId.Caption_MovePivotTableFormTitle, "Move PivotTable");
			AddString(SpreadsheetControlStringId.Caption_ChangeDataSourcePivotTableFormTitle, "Change PivotTable Data Source");
			AddString(SpreadsheetControlStringId.Caption_FieldAndDataFieldSettingsPivotTableSourceName, "Source Name:");
			AddString(SpreadsheetControlStringId.Caption_FieldAndDataFieldSettingsPivotTableCustomName, "Custom Name:");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableFormTitle, "Field Settings");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableTabSubtotalsAndFilters, "Subtotals & Filters");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableTabLayoutAndPrint, "Layout & Print");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSubtotals, "Subtotals");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSubtotalAutomatic, "Automatic");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSubtotalNone, "None");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSubtotalCustom, "Custom");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSelectSubtotalFunctions, "Select one or more functions:");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableFilter, "Filter");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableNewItemsManualFilter, "Include new items in manual filter");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableLayout, "Layout");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableItemLabelsOutlineForm, "Show item labels in outline form");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableCompactForm, "Display labels from the next field in the same column (compact form)");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableSubtotalTop, "Display subtotals at the top of each group");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableItemLabelsTabularForm, "Show item labels in tabular form");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableRepeatItemLabels, "Repeat item labels");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableInsertBlankLine, "Insert blank line after each item label");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableShowItemsWithNoData, "Show items with no data");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTablePrint, "Print");
			AddString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableInsertPageBreak, "Insert page break after each item");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableFormTitle, "Value Field Settings");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableTabSummarizeValuesBy, "Summarize Values By");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableTabShowValuesAs, "Show Values As");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableSummarizeValueFieldBy, "Summarize value field by");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableChooseTheTypeOfCalculation, "Choose the type of calculation that you want to use to summarize data from the selected field");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableShowValuesAs, "Show values as");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableBaseField, "Base field:");
			AddString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableBaseItem, "Base item:");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsFormTitle, "PivotTable Fields");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsFormChooseFields, "Choose fields to add to report:");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsFormDragFields, "Drag fields between areas below:");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsFilters, "FILTERS");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsColumns, "COLUMNS");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsRows, "ROWS");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsValues, "VALUES");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsDeferLayoutUpdate, "Defer Layout Update");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldsButtonUpdate, "Update");
			AddString(SpreadsheetControlStringId.Caption_PivotTableFieldFilterItemsFormTitle, "Item Filter");
			AddString(SpreadsheetControlStringId.Caption_PivotTablePageFieldFilterItemsSelectMultipleItems, "Select Multiple Items");
			AddString(SpreadsheetControlStringId.Caption_PivotTableValueFilterFormTitle, "Value Filter");
			AddString(SpreadsheetControlStringId.Caption_PivotTableValueFilterFormShowItemsForWhich, "Show items for which");
			AddString(SpreadsheetControlStringId.Caption_PivotTableLabelDateValueFilterFormAnd, "and");
			AddString(SpreadsheetControlStringId.Caption_PivotTableLabelFilterFormTitle, "Label Filter");
			AddString(SpreadsheetControlStringId.Caption_PivotTableLabelFilterFormShowItemsForWhichTheLabel, "Show items for which the label");
			AddString(SpreadsheetControlStringId.Caption_PivotTableTop10FilterFormTitle, "Top 10 Filter");
			AddString(SpreadsheetControlStringId.Caption_PivotTableTop10FilterFormShow, "Show");
			AddString(SpreadsheetControlStringId.Caption_PivotTableTop10FilterFormBy, "by");
			AddString(SpreadsheetControlStringId.Caption_PivotTableDateFilterFormTitle, "Date Filter");
			AddString(SpreadsheetControlStringId.Caption_PivotTableLabelFilterFormShowItemsForWhichTheDate, "Show items for which the date");
			AddString(SpreadsheetControlStringId.Caption_PivotTableShowValuesAsFormTitle, "Show Values As");
			AddString(SpreadsheetControlStringId.Caption_PivotTableShowValuesAsFormBaseField, "Base Field:");
			AddString(SpreadsheetControlStringId.Caption_PivotTableShowValuesAsFormBaseItem, "Base Item:");
		}
		#endregion
		public static XtraLocalizer<SpreadsheetControlStringId> CreateDefaultLocalizer() {
			return new XpfSpreadsheetResLocalizer();
		}
		public static string GetString(SpreadsheetControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SpreadsheetControlStringId> CreateResXLocalizer() {
			return new XpfSpreadsheetResLocalizer();
		}
	}
	public class XpfSpreadsheetResLocalizer : DXResXLocalizer<SpreadsheetControlStringId> {
		public XpfSpreadsheetResLocalizer()
			: base(new XpfSpreadsheetLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Spreadsheet.LocalizationRes", typeof(XpfSpreadsheetResLocalizer).Assembly);
		}
	}
}
