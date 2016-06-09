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
using System.Globalization;
using System.Reflection;
using System.Resources;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraSpreadsheet.Localization {
	#region XtraSpreadsheetStringId
	public enum XtraSpreadsheetStringId {
		Msg_ErrorInternalError,
		Msg_ErrorUseInvalidWorkbook,
		Msg_ErrorDuplicateSheetName,
		Msg_ErrorBlankSheetName,
		Msg_ErrorDefinedNameAlreadyExists,
		Msg_ErrorUseDeletedStyle,
		Msg_ErrorStyleNotFound,
		Msg_ErrorDuplicateStyleName,
		Msg_ErrorDeletingBuiltInTableStyle,
		Msg_ErrorDefinedNameReferenceWithoutSheet,
		Msg_ErrorIncorrectRowIndex,
		Msg_ErrorIncorrectColumnIndex,
		Msg_ErrorIncorrectReferenceExpression,
		Msg_ErrorIncorrectReferenceExpressionInDefinedName,
		Msg_ErrorUnionRangesWithDifferentWorksheets,
		Msg_ErrorTableColumnAlreadyExists,
		Msg_ErrorIncorrectRowHeight,
		Msg_ErrorIncorrectColumnWidth,
		Msg_ErrorIncorrectIndentValue,
		Msg_ErrorFormula,
		Msg_ErrorIncorectCount,
		Msg_ErrorIncorectLowBound,
		Msg_ErrorIncorectHighBound,
		Msg_ErrorIncorectMidPoint,
		Msg_CondFmtIncorectValueType,
		Msg_ErrorDeleteSingularWorksheet,
		Msg_ErrorTableRowIndexOutside,
		Msg_ErrorTableColumnIndexOutside,
		Msg_ErrorCanNotCreateTable,
		Msg_ErrorTableAlreadyExists,
		Msg_ErrorTableNameIsNotValid,
		Msg_ErrorInsertAboveHeaderRow,
		Msg_ErrorInsertBelowTotalRow,
		Msg_ErrorDeleteHeaderRow,
		Msg_ErrorStayOnlyHeaders,
		Msg_ErrorNegativeIndexNotAllowed,
		Msg_ErrorUseDeletedHyperlink,
		Msg_ErrorAttemptToRemoveArrayFormula,
		Msg_ErrorAttemptToRemoveTableHeader,
		Msg_ErrorWorksheetToBeDeletedNotFound,
		Msg_ErrorUseDeletedWorksheet,
		Msg_ErrorUseRangeFromAnotherWorksheet,
		Msg_ErrorUseRangeFromAnotherWorkbook,
		Msg_ErrorUseShapeFromAnotherWorksheet,
		Msg_ErrorUseTableStyleFromAnotherWorkbook,
		Msg_ErrorUseDeletedDefinedName,
		Msg_ErrorUseDeletedConditionalFormatting,
		Msg_ErrorCommentAlreadyExists,
		Msg_ErrorInvalidDefinedName,
		Msg_ErrorInvalidDefinedName_FormulaBar,
		Msg_ErrorSheetNameContainsNotAllowedCharacters,
		Msg_ErrorSheetNameStartOrEndWithSingleQuote,
		Msg_ErrorSheetNameExceedAllowedLength,
		Msg_ErrorWorksheetIndexOutside,
		Msg_ErrorUseInvalidStyle,
		Msg_ErrorUseStyleFromAnotherWorkbook,
		Msg_ErrorUseInvalidRangeFormatObject,
		Msg_ErrorSharedFormulaIndexOutside,
		Msg_ErrorSetInsideBordersToCell,
		Msg_ErrorChangingPartOfAnArray,
		Msg_ErrorCannotChangingPartOfAnArray,
		Msg_ErrorMuiltiCellArrayFormulaInTable,
		Msg_ErrorAttemptToCreateSharedFormulaInMergedCells,
		Msg_ErrorAttemptToCreateArrayFormulaInMergedCells,
		Msg_ErrorChangingPartOfAMergedCell,
		Msg_ErrorCannotChangingMergedCell,
		Msg_ErrorRelativeReference,
		Msg_ErrorZeroCountRanges,
		Msg_ErrorZeroCountSheets,
		Msg_ErrorCopyAreaCannotBeFitIntoThePasteArea,
		Msg_ErrorCopyAreaCannotOverlapUnlessSameSizeAndShape,
		Msg_ErrorAttemptToRemoveRangeWithLockedCells,
		Msg_ErrorDefinedNameNotFounded,
		Msg_ErrorWorksheetWithNameNotFound,
		Msg_ErrorShapeWithNameNotFound,
		Msg_ErrorShapeWithIdNotFound,
		Msg_ErrorIncorrectColumnHeading,
		Msg_ErrorIncorrectRowHeading,
		Msg_ErrorCellIndexInRangeOutOfRange,
		Msg_ErrorIncorrectIndexToInsert,
		Msg_ErrorNegativeFrozenColumnOffset,
		Msg_ErrorNegativeFrozenRowOffset,
		Msg_ErrorRowOffsetRefersBeyondWorksheet,
		Msg_ErrorColumnOffsetRefersBeyondWorksheet,
		Msg_ErrorAttemptToHideAllColumns,
		Msg_ErrorAttemptToHideAllRows,
		Msg_ErrorInvalidCustomPropertyValue,
		Msg_ErrorFirstDVCriteriaMustNotBeEmpty,
		Msg_ErrorSecondDVCriteriaMustNotBeEmpty,
		Msg_ErrorFirstDVCriteriaMustBeEmpty,
		Msg_ErrorSecondDVCriteriaMustBeEmpty,
		Msg_IncompliantArrayParsedFormula,
		Msg_IncompliantCellParsedFormula,
		Msg_IncompliantNamedParsedFormula,
		Msg_DefinedNameHasInvalidScope,
		Msg_DefinedNameHasInvalidExpression,
		Msg_DefinedNameHasBeenChanged,
		Msg_DefinedNameInvalidName,
		Msg_ProtectedRangeEmptyName,
		Msg_ProtectedRangeInvalidName,
		Msg_ProtectedRangeDuplicateName,
		Msg_NumberFormatRecordsSkipped,
		Msg_RowFirstColumnOutOfXLSRange,
		Msg_RowLastColumnOutOfXLSRange,
		Msg_RefToError,
		Msg_RefTruncated,
		Msg_TableRefToRef,
		Msg_TableRefToError,
		Msg_CellOnSheet,
		Msg_ArrayFormulaRange,
		Msg_SharedFormulaRange,
		Msg_ElfToRef,
		Msg_ElfLelToError,
		Msg_StyleNameHasBeenChanged,
		Msg_XFTableHasBeenTruncated,
		Msg_DefaultStyleXFUsed,
		Msg_DefaultCellXFUsed,
		Msg_FuncVarExceedMaxArgCount,
		Msg_FuncExceedMaxNestingLevel,
		Msg_FormulaExceedMaxSize,
		Msg_TableColumnNameChanged,
		Msg_InvalidHyperlinkRemoved,
		Msg_InvalidFormulaRemoved,
		Msg_InvalidBorderStyleRemoved,
		Msg_InvalidFormula,
		Msg_ErrorInvalidReference,
		Msg_InvalidFilePath,
		Msg_CircularReference,
		Msg_CustomFunctionAlreadyDefined,
		Msg_CustomFunctionInvalidName,
		Msg_CustomFunctionInvalidParameters,
		Msg_ExternalLinkInvalidPath,
		Msg_ExternalLinkAlreadyExists,
		Msg_ExternalWorkbookWithoutSheets,
		Msg_ColumnIndexOutOfRange,
		Msg_RowIndexOutOfRange,
		Msg_InvalidSheetSpecified,
		Msg_InvalidStyleName,
		Msg_CanReplaceTheContentsOfTheDestinationCells,
		Msg_CanSaveChangesNameReference,
		Msg_UnableToSortSelection,
		Msg_UnableToSortMultipleSelection,
		Msg_UnableToSortMergedCells,
		Msg_InvalidSortColumnIndex,
		Msg_ErrorOverlapRange,
		Msg_ErrorUnionRange,
		Msg_ErrorConditionalFormattingUnionRange,
		Msg_ErrorConditionalFormattingRange,
		Msg_ErrorConditionalFormattingFormula,
		Msg_ErrorUseDeletedObject,
		Msg_CellOrChartIsReadonly,
		Msg_CellOrChartIsReadonlyShort,
		Msg_PasswordNotConfirmed,
		Msg_IncorrectPassword,
		Msg_SearchCantFindData,
		Msg_ReplaceAllSucceeded,
		Msg_CantReplaceOnProtectedSheet,
		Msg_ReplaceCantFindMatch,
		Msg_ErrorLastPrimaryAxisCannotBeChanged,
		Msg_ErrorAxisGroupCannotBeChanged,
		Msg_ErrorSomeChartTypesCannotBeCombinedWithOtherChartTypes,
		Msg_ErrorSurfaceChartMustContainAtLeastTwoSeries,
		Msg_ErrorIncorrectCreateStockHighLowCloseChart,
		Msg_ErrorIncorrectCreateStockOpenHighLowCloseChart,
		Msg_ErrorIncorrectCreateStockVolumeHighLowCloseChart,
		Msg_ErrorIncorrectCreateStockVolumeOpenHighLowCloseChart,
		Msg_ErrorInvalidOperationForDataSheet,
		Msg_ErrorInvalidRange,
		Msg_ErrorInvalidNumberFormat,
		Msg_ErrorRangeContainsTable,
		Msg_ErrorRangeConsistsOfEmptyCells,
		Msg_ConfirmDeleteDefinedName,
		Msg_CanNotShiftNonBlankCellsOffOfTheSheet,
		Msg_DeleteSheetConfirmation,
		Msg_ConfirmModifyExistingOutline,
		Msg_CannotCreateOutline,
		Msg_ErrorSubtotalUncomplete,
		Msg_SubtotalNeedColumnNames,
		Msg_GroupingNeedUnprotectSheet,
		Msg_ErrorInvalidFilterArgument,
		Msg_ErrorInvalidCreateFromSelectionRange,
		Msg_ErrorIndexOutOfRange,
		Msg_CommandRequiresAtLeastTwoRows,
		Msg_PivotTableWillNotFitOnTheSheet,
		Msg_PivotTableWillNotFitOnTheSheetSelectNewLocation,
		Msg_PivotTableWillOverrideSheetCells,
		Msg_PivotTableIndentOverflow,
		Msg_PivotTableFieldNameIsInvalid,
		Msg_PivotTableCanNotBeChanged,
		Msg_PivotTablePartCanNotBeChanged,
		Msg_PivotTableCanNotBeBuiltFromEmptyCache,
		Msg_PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow,
		Msg_PivotTableReportCanNotOverlapTable,
		Msg_PivotTableDataSourceAndDestinationReferencesAreBothInvalid,
		Msg_PivotTableDataSourceReferenceNotValid,
		Msg_PivotTableDestinationReferenceNotValid,
		Msg_PivotTableNameAlreadyExists,
		Msg_PivotTableNameIsInvalid,
		Msg_PivotTableCanNotOverlapPivotTable,
		Msg_PivotTableTooMuchDataFields,
		Msg_PivotTableTooMuchPageFields,
		Msg_PivotTableNotEnoughDataFields,
		Msg_PivotCacheStringVeryLong,
		Msg_PivotCacheSourceTypeIsInvalid,
		Msg_PivotFieldNameAlreadyExists,
		Msg_PivotFieldNameIsInvalid,
		Msg_PivotFieldCannotBePlacedOnThatAxis,
		Msg_PivotFieldHasTooMuchItems,
		Msg_PivotFieldHasTooMuchItems_ColumnField,
		Msg_PivotCalculationRequiresField,
		Msg_PivotCalculationRequiresItem,
		Msg_PivotCannotHideLastVisibleItem,
		Msg_CantExportPivotCacheSourceExternal,
		Msg_PivotFieldContainsTooMuschUniqueItems,
		Msg_PivotFilterRequiresValue,
		Msg_PivotFilterRequiresSecondValue,
		Msg_PivotFilterRequiresMeasureField,
		Msg_PivotFilterCannotAddFilterToPageField,
		Msg_PivotFilterTop10CountMustBeInteger,
		Msg_PivotFilterCannotChangeTop10TypeProperty,
		Msg_PivotCacheFieldContainsNonDateItems,
		Msg_PivotTableSavedWithoutUnderlyingData,
		Msg_PivotCanNotDetermineField,
		Msg_ErrorRangeMustConsistOfASingleRowOrColumn,
		Msg_ErrorSparklinePositionsMustBeUnique,
		Msg_ErrorSparklinePositionOrDataRangeIsInvalid,
		Msg_CantDoThatToAMergedCell,
		Msg_SelectedCellsAffectsPivotTableAndCanNotBeChanged,
		Msg_ChartDataRangeIntersectPivotTable,
		Caption_PivotDefaultColumnHeader,
		Caption_PivotDefaultDataCaption,
		Caption_PivotDefaultRowHeader,
		Caption_PivotGrandTotal,
		Caption_PivotEmptyValue,
		Caption_PivotTrueValue,
		Caption_PivotFalseValue,
		FileFilterDescription_OpenXmlFiles,
		FileFilterDescription_OpenDocumentFiles,
		FileFilterDescription_DocFiles,
		FileFilterDescription_CSVFiles,
		FileFilterDescription_TextFiles,
		FileFilterDescription_HtmlFiles,
		FileFilterDescription_XlsmFiles,
		FileFilterDescription_XltFiles,
		FileFilterDescription_XltxFiles,
		FileFilterDescription_XltmFiles,
		FileFilterDescription_AllFiles,
		Caption_InvalidExpression,
		Caption_PermissionEditRangeWithoutPassword,
		Caption_PageFile,
		Caption_GroupCommon,
		Caption_GroupInfo,
		Caption_PageHome,
		Caption_GroupFont,
		Caption_GroupClipboard,
		Caption_GroupAlignment,
		Caption_GroupNumber,
		Caption_GroupStyles,
		Caption_GroupCells,
		Caption_GroupEditing,
		Caption_PageMailMerge,
		Caption_GroupMailMergeData,
		Caption_GroupMailMergeMode,
		Caption_GroupMailMergeExtended,
		Caption_GroupMailMergeGrouping,
		Caption_GroupMailMergeFiltering,
		Caption_GroupMailMergeBinding,
		Caption_MailMergeFilterExpressionEditor,
		Caption_PageInsert,
		Caption_GroupIllustrations,
		Caption_GroupCharts,
		Caption_GroupLinks,
		Caption_GroupSymbols,
		Caption_GroupTables,
		Caption_PagePageLayout,
		Caption_GroupPageSetup,
		Caption_GroupArrange,
		Caption_GroupPrint,
		Caption_PageFormulas,
		Caption_GroupFunctionLibrary,
		Caption_GroupFormulaDefinedNames,
		Caption_GroupFormulaAuditing,
		Caption_GroupFormulaCalculation,
		Caption_PageData,
		Caption_GroupSortAndFilter,
		Caption_GroupDataTools,
		Caption_GroupOutline,
		Caption_PageView,
		Caption_GroupShow,
		Caption_GroupZoom,
		Caption_GroupWindow,
		Caption_PageReview,
		Caption_GroupComments,
		Caption_GroupChanges,
		Caption_PageCategoryChartTools,
		Caption_PageChartsDesign,
		Caption_GroupChartsDesignType,
		Caption_GroupChartsDesignData,
		Caption_GroupChartsDesignLayouts,
		Caption_GroupChartsDesignStyles,
		Caption_PageChartsLayout,
		Caption_GroupChartsLayoutLabels,
		Caption_GroupChartsLayoutAxes,
		Caption_GroupChartsLayoutAnalysis,
		Caption_PageFormat,
		Caption_PageCategoryPictureTools,
		Caption_PageCategoryDrawingTools,
		Caption_ColorAutomatic,
		Caption_NoFill,
		Caption_NoColor,
		Caption_StyleGalleryGroupCustom,
		Caption_StyleGalleryGroupGoogBadAndNeutral,
		Caption_StyleGalleryGroupDataAndModel,
		Caption_StyleGalleryGroupTitlesAndHeadings,
		Caption_StyleGalleryGroupThemedCellStyles,
		Caption_StyleGalleryGroupNumberFormat,
		Caption_PageCategoryTableTools,
		Caption_TableToolsDesignPage,
		Caption_GroupTableStyleOptions,
		Caption_GroupTableProperties,
		Caption_LightTableStyleNamePart,
		Caption_MediumTableStyleNamePart,
		Caption_DarkTableStyleNamePart,
		Caption_TableStyleNameIsNone,
		Caption_PrefixTableStyleNamePart,
		Caption_CustomTableStyleCategory,
		Caption_LightTableStyleCategory,
		Caption_MediumTableStyleCategory,
		Caption_DarkTableStyleCategory,
		Caption_PageCategoryPivotTableTools,
		Caption_PivotTableAnalyze,
		Caption_PivotTableAnalyzePivotTable,
		Caption_PivotTableAnalyzeActiveField,
		Caption_PivotTableAnalyzeData,
		Caption_PivotTableAnalyzeActions,
		Caption_PivotTableAnalyzeShow,
		Caption_PivotTableDesign,
		Caption_PivotTableDesignLayout,
		Caption_PivotTableDesignPivotTableStyleOptions,
		Caption_PivotTableDesignPivotTableStyles,
		Caption_PrefixPivotStyleNamePart,
		Caption_ConditionalFormattingTop10Rule_FormText,
		Caption_ConditionalFormattingBottom10Rule_FormText,
		Caption_ConditionalFormattingTop10PercentRule_FormText,
		Caption_ConditionalFormattingBottom10PercentRule_FormText,
		Caption_ConditionalFormattingAboveAverageRule_FormText,
		Caption_ConditionalFormattingBelowAverageRule_FormText,
		Caption_ConditionalFormattingGreaterThanRule_FormText,
		Caption_ConditionalFormattingLessThanRule_FormText,
		Caption_ConditionalFormattingEqualToRule_FormText,
		Caption_ConditionalFormattingTextContainsRule_FormText,
		Caption_ConditionalFormattingDuplicateValuesRule_FormText,
		Caption_ConditionalFormattingDateOccurringRule_FormText,
		Caption_ConditionalFormattingBetweenRule_FormText,
		Caption_ConditionalFormattingTopRule_LabelHeaderText,
		Caption_ConditionalFormattingBottomRule_LabelHeaderText,
		Caption_ConditionalFormattingAboveAverageRule_LabelHeaderText,
		Caption_ConditionalFormattingAboveAverageRule_LabelWithText,
		Caption_ConditionalFormattingBelowAverageRule_LabelHeaderText,
		Caption_ConditionalFormattingBelowAverageRule_LabelWithText,
		Caption_ConditionalFormattingGreaterThanRule_LabelHeaderText,
		Caption_ConditionalFormattingLessThanRule_LabelHeaderText,
		Caption_ConditionalFormattingEqualToRule_LabelHeaderText,
		Caption_ConditionalFormattingTextContainsRule_LabelHeaderText,
		Caption_ConditionalFormattingDuplicateValuesRule_LabelHeaderText,
		Caption_ConditionalFormattingDateOccurringRule_LabelHeaderText,
		Caption_ConditionalFormattingBetweenRule_LabelHeaderText,
		Caption_ConditionalFormattingTopBottomRule_LabelWithText,
		Caption_ConditionalFormattingTopBottomPercentRule_LabelWithText,
		Caption_ConditionalFormattingDuplicateValuesRule_LabelWithText,
		Caption_ConditionalFormattingDateOccurringRule_LabelWithText,
		Caption_NewDefinedNameFormTitle,
		Caption_EditDefinedNameFormTitle,
		Caption_NewProtectedRangeFormTitle,
		Caption_EditProtectedRangeFormTitle,
		Caption_PermissionSelectLockedCells,
		Caption_PermissionSelectUnlockedCells,
		Caption_PermissionFormatCells,
		Caption_PermissionFormatColumns,
		Caption_PermissionFormatRows,
		Caption_PermissionInsertColumns,
		Caption_PermissionInsertRows,
		Caption_PermissionInsertHyperlinks,
		Caption_PermissionDeleteColumns,
		Caption_PermissionDeleteRows,
		Caption_PermissionSort,
		Caption_PermissionAutoFilter,
		Caption_PermissionPivotTable,
		Caption_PermissionEditObjects,
		Caption_PermissionEditScenarios,
		Caption_SearchByRows,
		Caption_SearchByColumns,
		Caption_SearchInFormulas,
		Caption_SearchInValues,
		Caption_GenericFilterOperatorNone,
		Caption_GenericFilterOperatorEquals,
		Caption_GenericFilterOperatorDoesNotEqual,
		Caption_GenericFilterOperatorGreater,
		Caption_GenericFilterOperatorGreaterOrEqual,
		Caption_GenericFilterOperatorLess,
		Caption_GenericFilterOperatorLessOrEqual,
		Caption_GenericFilterOperatorAfter,
		Caption_GenericFilterOperatorAfterOrEqual,
		Caption_GenericFilterOperatorBefore,
		Caption_GenericFilterOperatorBeforeOrEqual,
		Caption_GenericFilterOperatorBeginsWith,
		Caption_GenericFilterOperatorDoesNotBeginWith,
		Caption_GenericFilterOperatorEndsWith,
		Caption_GenericFilterOperatorDoesNotEndWith,
		Caption_GenericFilterOperatorContains,
		Caption_GenericFilterOperatorDoesNotContain,
		Caption_Top10FilterTypeItems,
		Caption_Top10FilterTypePercent,
		Caption_Top10FilterOrderTop,
		Caption_Top10FilterOrderBottom,
		Caption_Bytes,
		MenuCmd_None,
		MenuCmd_ShowDocumentProperties,
		MenuCmd_ShowDocumentPropertiesDescription,
		MenuCmd_FormatFillColor,
		MenuCmd_FormatFillColorDescription,
		MenuCmd_FormatAlignmentTop,
		MenuCmd_FormatAlignmentTopDescription,
		MenuCmd_FormatAlignmentMiddle,
		MenuCmd_FormatAlignmentMiddleDescription,
		MenuCmd_FormatAlignmentBottom,
		MenuCmd_FormatAlignmentBottomDescription,
		MenuCmd_FormatWrapText,
		MenuCmd_FormatWrapTextDescription,
		MenuCmd_FormatIncreaseIndent,
		MenuCmd_FormatIncreaseIndentDescription,
		MenuCmd_FormatDecreaseIndent,
		MenuCmd_FormatDecreaseIndentDescription,
		MenuCmd_FormatBorderColor,
		MenuCmd_FormatBorderColorDescription,
		MenuCmd_FormatBorderLineStyle,
		MenuCmd_FormatBorderLineStyleDescription,
		MenuCmd_FormatThickBorder,
		MenuCmd_FormatThickBorderDescription,
		MenuCmd_FormatOutsideBorders,
		MenuCmd_FormatOutsideBordersDescription,
		MenuCmd_FormatLeftBorder,
		MenuCmd_FormatLeftBorderDescription,
		MenuCmd_FormatRightBorder,
		MenuCmd_FormatRightBorderDescription,
		MenuCmd_FormatTopBorder,
		MenuCmd_FormatTopBorderDescription,
		MenuCmd_FormatBottomBorder,
		MenuCmd_FormatBottomBorderDescription,
		MenuCmd_FormatBottomDoubleBorder,
		MenuCmd_FormatBottomDoubleBorderDescription,
		MenuCmd_FormatBottomThickBorder,
		MenuCmd_FormatBottomThickBorderDescription,
		MenuCmd_FormatTopAndBottomBorder,
		MenuCmd_FormatTopAndBottomBorderDescription,
		MenuCmd_FormatTopAndThickBottomBorder,
		MenuCmd_FormatTopAndThickBottomBorderDescription,
		MenuCmd_FormatTopAndDoubleBottomBorder,
		MenuCmd_FormatTopAndDoubleBottomBorderDescription,
		MenuCmd_FormatAllBorders,
		MenuCmd_FormatAllBordersDescription,
		MenuCmd_FormatNoBorders,
		MenuCmd_FormatNoBordersDescription,
		MenuCmd_FormatBordersCommandGroup,
		MenuCmd_FormatBordersCommandGroupDescription,
		MenuCmd_ToggleCellLocked,
		MenuCmd_ToggleCellLockedDescription,
		MenuCmd_FormatNumber,
		MenuCmd_FormatNumberDescription,
		MenuCmd_FormatNumberGeneral,
		MenuCmd_FormatNumberGeneralDescription,
		MenuCmd_FormatNumberDecimal,
		MenuCmd_FormatNumberDecimalDescription,
		MenuCmd_FormatNumberPercent,
		MenuCmd_FormatNumberPercentDescription,
		MenuCmd_FormatNumberPercentage,
		MenuCmd_FormatNumberPercentageDescription,
		MenuCmd_FormatNumberScientific,
		MenuCmd_FormatNumberScientificDescription,
		MenuCmd_FormatNumberFraction,
		MenuCmd_FormatNumberFractionDescription,
		MenuCmd_FormatNumberAccounting,
		MenuCmd_FormatNumberAccountingDescription,
		MenuCmd_FormatNumberAccountingCurrency,
		MenuCmd_FormatNumberAccountingCurrencyDescription,
		MenuCmd_FormatNumberShortDate,
		MenuCmd_FormatNumberShortDateDescription,
		MenuCmd_FormatNumberLongDate,
		MenuCmd_FormatNumberLongDateDescription,
		MenuCmd_FormatNumberText,
		MenuCmd_FormatNumberTextDescription,
		MenuCmd_FormatNumberTime,
		MenuCmd_FormatNumberTimeDescription,
		MenuCmd_FormatNumberAccountingUS,
		MenuCmd_FormatNumberAccountingUSDescription,
		MenuCmd_FormatNumberAccountingUK,
		MenuCmd_FormatNumberAccountingUKDescription,
		MenuCmd_FormatNumberAccountingPRC,
		MenuCmd_FormatNumberAccountingPRCDescription,
		MenuCmd_FormatNumberAccountingEuro,
		MenuCmd_FormatNumberAccountingEuroDescription,
		MenuCmd_FormatNumberAccountingSwiss,
		MenuCmd_FormatNumberAccountingSwissDescription,
		MenuCmd_FormatNumberAccountingCommandGroup,
		MenuCmd_FormatNumberAccountingCommandGroupDescription,
		MenuCmd_FormatNumberIncreaseDecimal,
		MenuCmd_FormatNumberIncreaseDecimalDescription,
		MenuCmd_FormatNumberDecreaseDecimal,
		MenuCmd_FormatNumberDecreaseDecimalDescription,
		MenuCmd_FormatNumberCommaStyle,
		MenuCmd_FormatNumberCommaStyleDescription,
		MenuCmd_FormatCellStyle,
		MenuCmd_FormatCellStyleDescription,
		MenuCmd_EditingMergeCellsCommandGroup,
		MenuCmd_EditingMergeCellsCommandGroupDescription,
		MenuCmd_EditingMergeAndCenterCells,
		MenuCmd_EditingMergeAndCenterCellsDescription,
		MenuCmd_EditingMergeCellsAcross,
		MenuCmd_EditingMergeCellsAcrossDescription,
		MenuCmd_EditingMergeCells,
		MenuCmd_EditingMergeCellsDescription,
		MenuCmd_EditingUnmergeCells,
		MenuCmd_EditingUnmergeCellsDescription,
		MenuCmd_EditingFillCommandGroup,
		MenuCmd_EditingFillCommandGroupDescription,
		MenuCmd_EditingFillDown,
		MenuCmd_EditingFillDownDescription,
		MenuCmd_EditingFillUp,
		MenuCmd_EditingFillUpDescription,
		MenuCmd_EditingFillLeft,
		MenuCmd_EditingFillLeftDescription,
		MenuCmd_EditingFillRight,
		MenuCmd_EditingFillRightDescription,
		MenuCmd_FormatClearCommandGroup,
		MenuCmd_FormatClearCommandGroupDescription,
		MenuCmd_FormatClearAll,
		MenuCmd_FormatClearAllDescription,
		MenuCmd_FormatClearFormats,
		MenuCmd_FormatClearFormatsDescription,
		MenuCmd_FormatClearContents,
		MenuCmd_FormatClearContentsDescription,
		MenuCmd_FormatClearComments,
		MenuCmd_FormatClearCommentsDescription,
		MenuCmd_FormatClearHyperlinks,
		MenuCmd_FormatClearHyperlinksDescription,
		MenuCmd_FormatRemoveHyperlinks,
		MenuCmd_FormatRemoveHyperlinksDescription,
		MenuCmd_ViewShowGridlines,
		MenuCmd_ViewShowGridlinesDescription,
		MenuCmd_ViewShowHeadings,
		MenuCmd_ViewShowHeadingsDescription,
		MenuCmd_ViewShowFormulas,
		MenuCmd_ViewShowFormulasDescription,
		MenuCmd_ViewFreezePanesCommandGroup,
		MenuCmd_ViewFreezePanesCommandGroupDescription,
		MenuCmd_ViewFreezePanes,
		MenuCmd_ViewFreezePanesDescription,
		MenuCmd_ViewUnfreezePanes,
		MenuCmd_ViewUnfreezePanesDescription,
		MenuCmd_ViewFreezeTopRow,
		MenuCmd_ViewFreezeTopRowDescription,
		MenuCmd_ViewFreezeFirstColumn,
		MenuCmd_ViewFreezeFirstColumnDescription,
		MenuCmd_InsertSheet,
		MenuCmd_InsertSheetDescription,
		MenuCmd_InsertSheetContextMenuItem,
		MenuCmd_InsertSheetContextMenuItemDescription,
		MenuCmd_InsertSheetRows,
		MenuCmd_InsertSheetRowsDescription,
		MenuCmd_InsertSheetRowsContextMenuItem,
		MenuCmd_InsertSheetRowsContextMenuItemDescription,
		MenuCmd_InsertSheetColumns,
		MenuCmd_InsertSheetColumnsDescription,
		MenuCmd_InsertSheetColumnsContextMenuItem,
		MenuCmd_InsertSheetColumnsContextMenuItemDescription,
		MenuCmd_ShowInsertSheetCellsForm,
		MenuCmd_ShowInsertSheetCellsFormDescription,
		MenuCmd_InsertCellsCommandGroup,
		MenuCmd_InsertCellsCommandGroupDescription,
		MenuCmd_PageSetupMarginsCommandGroup,
		MenuCmd_PageSetupMarginsCommandGroupDescription,
		MenuCmd_PageSetupSetPaperKind,
		MenuCmd_PageSetupSetPaperKindDescription,
		MenuCmd_PageSetupPaperKindCommandGroup,
		MenuCmd_PageSetupPaperKindCommandGroupDescription,
		MenuCmd_PageSetupPrintAreaCommandGroup,
		MenuCmd_PageSetupPrintAreaCommandGroupDescription,
		MenuCmd_PageSetupSetPrintArea,
		MenuCmd_PageSetupSetPrintAreaDescription,
		MenuCmd_PageSetupClearPrintArea,
		MenuCmd_PageSetupClearPrintAreaDescription,
		MenuCmd_PageSetupAddPrintArea,
		MenuCmd_PageSetupAddPrintAreaDescription,
		MenuCmd_PageSetupPrintGridlines,
		MenuCmd_PageSetupPrintGridlinesDescription,
		MenuCmd_PageSetupPrintHeadings,
		MenuCmd_PageSetupPrintHeadingsDescription,
		MenuCmd_RemoveCellsCommandGroup,
		MenuCmd_RemoveCellsCommandGroupDescription,
		MenuCmd_RemoveSheetRows,
		MenuCmd_RemoveSheetRowsDescription,
		MenuCmd_RemoveSheetRowsContextMenuItem,
		MenuCmd_RemoveSheetRowsContextMenuItemDescription,
		MenuCmd_RemoveSheetColumns,
		MenuCmd_RemoveSheetColumnsDescription,
		MenuCmd_RemoveSheetColumnsContextMenuItem,
		MenuCmd_RemoveSheetColumnsContextMenuItemDescription,
		MenuCmd_RemoveSheet,
		MenuCmd_RemoveSheetDescription,
		MenuCmd_RemoveSheetContextMenuItem,
		MenuCmd_RemoveSheetContextMenuItemDescription,
		MenuCmd_FormatCommandGroup,
		MenuCmd_FormatCommandGroupDescription,
		MenuCmd_FormatAutoFitRowHeight,
		MenuCmd_FormatAutoFitRowHeightDescription,
		MenuCmd_FormatAutoFitColumnWidth,
		MenuCmd_FormatAutoFitColumnWidthDescription,
		MenuCmd_FormatTabColor,
		MenuCmd_FormatTabColorDescription,
		MenuCmd_HideRows,
		MenuCmd_HideRowsDescription,
		MenuCmd_HideRowsContextMenuItem,
		MenuCmd_HideRowsContextMenuItemDescription,
		MenuCmd_HideColumns,
		MenuCmd_HideColumnsDescription,
		MenuCmd_HideColumnsContextMenuItem,
		MenuCmd_HideColumnsContextMenuItemDescription,
		MenuCmd_HideSheet,
		MenuCmd_HideSheetDescription,
		MenuCmd_HideSheetContextMenuItem,
		MenuCmd_HideSheetContextMenuItemDescription,
		MenuCmd_UnhideRows,
		MenuCmd_UnhideRowsDescription,
		MenuCmd_UnhideRowsContextMenuItem,
		MenuCmd_UnhideRowsContextMenuItemDescription,
		MenuCmd_UnhideColumns,
		MenuCmd_UnhideColumnsDescription,
		MenuCmd_UnhideColumnsContextMenuItem,
		MenuCmd_UnhideColumnsContextMenuItemDescription,
		MenuCmd_UnhideSheet,
		MenuCmd_UnhideSheetDescription,
		MenuCmd_UnhideSheetContextMenuItem,
		MenuCmd_UnhideSheetContextMenuItemDescription,
		MenuCmd_HideAndUnhideCommandGroup,
		MenuCmd_HideAndUnhideCommandGroupDescription,
		MenuCmd_RenameSheet,
		MenuCmd_RenameSheetDescription,
		MenuCmd_RenameSheetContextMenuItem,
		MenuCmd_RenameSheetContextMenuItemDescription,
		MenuCmd_FormatRowHeight,
		MenuCmd_FormatRowHeightDescription,
		MenuCmd_FormatColumnWidth,
		MenuCmd_FormatColumnWidthDescription,
		MenuCmd_FormatDefaultColumnWidth,
		MenuCmd_FormatDefaultColumnWidthDescription,
		MenuCmd_MoveOrCopySheet,
		MenuCmd_MoveOrCopySheetDescription,
		MenuCmd_MoveOrCopySheetContextMenuItem,
		MenuCmd_MoveOrCopySheetContextMenuItemDescription,
		MenuCmd_FormatCellsNumber,
		MenuCmd_FormatCellsNumberDescription,
		MenuCmd_FormatCellsAlignment,
		MenuCmd_FormatCellsAlignmentDescription,
		MenuCmd_FormatCellsFont,
		MenuCmd_FormatCellsFontDescription,
		MenuCmd_FormatCellsBorder,
		MenuCmd_FormatCellsBorderDescription,
		MenuCmd_FormatCellsFill,
		MenuCmd_FormatCellsFillDescription,
		MenuCmd_FormatCellsProtection,
		MenuCmd_FormatCellsProtectionDescription,
		MenuCmd_FormatCellsContextMenuItem,
		MenuCmd_FormatCellsContextMenuItemDescription,
		MenuCmd_FunctionsFinancialCommandGroup,
		MenuCmd_FunctionsFinancialCommandGroupDescription,
		MenuCmd_FunctionsLogicalCommandGroup,
		MenuCmd_FunctionsLogicalCommandGroupDescription,
		MenuCmd_FunctionsTextCommandGroup,
		MenuCmd_FunctionsTextCommandGroupDescription,
		MenuCmd_FunctionsDateAndTimeCommandGroup,
		MenuCmd_FunctionsDateAndTimeCommandGroupDescription,
		MenuCmd_FunctionsLookupAndReferenceCommandGroup,
		MenuCmd_FunctionsLookupAndReferenceCommandGroupDescription,
		MenuCmd_FunctionsMathAndTrigonometryCommandGroup,
		MenuCmd_FunctionsMathAndTrigonometryCommandGroupDescription,
		MenuCmd_FunctionsMoreCommandGroup,
		MenuCmd_FunctionsMoreCommandGroupDescription,
		MenuCmd_FunctionsStatisticalCommandGroup,
		MenuCmd_FunctionsStatisticalCommandGroupDescription,
		MenuCmd_FunctionsEngineeringCommandGroup,
		MenuCmd_FunctionsEngineeringCommandGroupDescription,
		MenuCmd_FunctionsCubeCommandGroup,
		MenuCmd_FunctionsCubeCommandGroupDescription,
		MenuCmd_FunctionsInformationCommandGroup,
		MenuCmd_FunctionsInformationCommandGroupDescription,
		MenuCmd_FunctionsCompatibilityCommandGroup,
		MenuCmd_FunctionsCompatibilityCommandGroupDescription,
		MenuCmd_FunctionsWebCommandGroup,
		MenuCmd_FunctionsWebCommandGroupDescription,
		MenuCmd_FunctionsAutoSumCommandGroup,
		MenuCmd_FunctionsAutoSumCommandGroupDescription,
		MenuCmd_FunctionsInsertSpecificFunction,
		MenuCmd_FunctionsInsertSpecificFunctionDescription,
		MenuCmd_FunctionsInsertSum,
		MenuCmd_FunctionsInsertSumDescription,
		MenuCmd_FunctionsInsertAverage,
		MenuCmd_FunctionsInsertAverageDescription,
		MenuCmd_FunctionsInsertCountNumbers,
		MenuCmd_FunctionsInsertCountNumbersDescription,
		MenuCmd_FunctionsInsertMax,
		MenuCmd_FunctionsInsertMaxDescription,
		MenuCmd_FunctionsInsertMin,
		MenuCmd_FunctionsInsertMinDescription,
		MenuCmd_FunctionsInsertCount,
		MenuCmd_FunctionsInsertCountDescription,
		MenuCmd_FunctionsInsertProduct,
		MenuCmd_FunctionsInsertProductDescription,
		MenuCmd_FunctionsInsertStdDev,
		MenuCmd_FunctionsInsertStdDevDescription,
		MenuCmd_FunctionsInsertStdDevp,
		MenuCmd_FunctionsInsertStdDevpDescription,
		MenuCmd_FunctionsInsertVar,
		MenuCmd_FunctionsInsertVarDescription,
		MenuCmd_FunctionsInsertVarp,
		MenuCmd_FunctionsInsertVarpDescription,
		MenuCmd_CollapseOrExpandFormulaBar,
		MenuCmd_CollapseOrExpandFormulaBarDescription,
		TargetFrameDescription_NewWindow,
		TargetFrameDescription_ParentFrame,
		TargetFrameDescription_SameFrame,
		TargetFrameDescription_WholePage,
		HyperlinkForm_SelectedBookmarkNone,
		HyperlinkForm_NodeCellReferences,
		HyperlinkForm_NodeDefinedName,
		HyperlinkForm_DisabledDisplayText,
		Tooltip_Hyperlink,
		Tooltip_FormulaBar,
		Tooltip_NameBox,
		Tooltip_ExpandFormulaBar,
		Tooltip_CollapseFormulaBar,
		Tooltip_FormulaBarCancelButton,
		Tooltip_FormulaBarOkButton,
		Tooltip_FormulaBarFunctionButton,
		Scope_Workbook,
		Scope_SheetIndex,
		Scope_ExternalWorkbook,
		Scope_DefinedName,
		MenuCmd_PasteCsvText,
		MenuCmd_PasteXmlSpreadsheet,
		MenuCmd_PasteXmlSpreadsheetDescription,
		MenuCmd_PasteRtfText,
		MenuCmd_PasteRtfTextDescription,
		MenuCmd_PasteTabDelimitedText,
		MenuCmd_PasteTabDelimitedDescription,
		MenuCmd_PasteCsvTextDescription,
		MenuCmd_PasteDataInterchangeFormat,
		MenuCmd_PasteDataInterchangeFormatDescription,
		MenuCmd_PasteBiff8Content,
		MenuCmd_PasteBiff8ContentDescription,
		MenuCmd_AddNewWorksheetDescription,
		MenuCmd_ConditionalFormattingRemoveFromSheet,
		MenuCmd_ConditionalFormattingRemoveFromSheetDescription,
		MenuCmd_ConditionalFormattingRemove,
		MenuCmd_ConditionalFormattingRemoveDescription,
		MenuCmd_ConditionalFormattingCommandGroup,
		MenuCmd_ConditionalFormattingCommandGroupDescription,
		MenuCmd_ConditionalFormattingColorScalesCommandGroup,
		MenuCmd_ConditionalFormattingColorScalesCommandGroupDescription,
		MenuCmd_ConditionalFormattingRemoveCommandGroup,
		MenuCmd_ConditionalFormattingRemoveCommandGroupDescription,
		MenuCmd_ConditionalFormattingColorScaleGreenYellowRed,
		MenuCmd_ConditionalFormattingColorScaleGreenYellowRedDescription,
		MenuCmd_ConditionalFormattingColorScaleRedYellowGreen,
		MenuCmd_ConditionalFormattingColorScaleRedYellowGreenDescription,
		MenuCmd_ConditionalFormattingColorScaleGreenWhiteRed,
		MenuCmd_ConditionalFormattingColorScaleGreenWhiteRedDescription,
		MenuCmd_ConditionalFormattingColorScaleRedWhiteGreen,
		MenuCmd_ConditionalFormattingColorScaleRedWhiteGreenDescription,
		MenuCmd_ConditionalFormattingColorScaleBlueWhiteRed,
		MenuCmd_ConditionalFormattingColorScaleBlueWhiteRedDescription,
		MenuCmd_ConditionalFormattingColorScaleRedWhiteBlue,
		MenuCmd_ConditionalFormattingColorScaleRedWhiteBlueDescription,
		MenuCmd_ConditionalFormattingColorScaleWhiteRed,
		MenuCmd_ConditionalFormattingColorScaleWhiteRedDescription,
		MenuCmd_ConditionalFormattingColorScaleRedWhite,
		MenuCmd_ConditionalFormattingColorScaleRedWhiteDescription,
		MenuCmd_ConditionalFormattingColorScaleGreenWhite,
		MenuCmd_ConditionalFormattingColorScaleGreenWhiteDescription,
		MenuCmd_ConditionalFormattingColorScaleWhiteGreen,
		MenuCmd_ConditionalFormattingColorScaleWhiteGreenDescription,
		MenuCmd_ConditionalFormattingColorScaleGreenYellow,
		MenuCmd_ConditionalFormattingColorScaleGreenYellowDescription,
		MenuCmd_ConditionalFormattingColorScaleYellowGreen,
		MenuCmd_ConditionalFormattingColorScaleYellowGreenDescription,
		MenuCmd_ConditionalFormattingDataBarsCommandGroup,
		MenuCmd_ConditionalFormattingDataBarsCommandGroupDescription,
		MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroup,
		MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroupDescription,
		MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroup,
		MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetsCommandGroup,
		MenuCmd_ConditionalFormattingIconSetsCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroup,
		MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetsShapesCommandGroup,
		MenuCmd_ConditionalFormattingIconSetsShapesCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroup,
		MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroup,
		MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroupDescription,
		MenuCmd_ConditionalFormattingIconSetArrows3Colored,
		MenuCmd_ConditionalFormattingIconSetArrows3ColoredDescription,
		MenuCmd_ConditionalFormattingIconSetArrows3Grayed,
		MenuCmd_ConditionalFormattingIconSetArrows3GrayedDescription,
		MenuCmd_ConditionalFormattingIconSetArrows4Colored,
		MenuCmd_ConditionalFormattingIconSetArrows4ColoredDescription,
		MenuCmd_ConditionalFormattingIconSetArrows4Grayed,
		MenuCmd_ConditionalFormattingIconSetArrows4GrayedDescription,
		MenuCmd_ConditionalFormattingIconSetArrows5Colored,
		MenuCmd_ConditionalFormattingIconSetArrows5ColoredDescription,
		MenuCmd_ConditionalFormattingIconSetArrows5Grayed,
		MenuCmd_ConditionalFormattingIconSetArrows5GrayedDescription,
		MenuCmd_ConditionalFormattingIconSetTriangles3,
		MenuCmd_ConditionalFormattingIconSetTriangles3Description,
		MenuCmd_ConditionalFormattingIconSetTrafficLights3,
		MenuCmd_ConditionalFormattingIconSetTrafficLights3Description,
		MenuCmd_ConditionalFormattingIconSetTrafficLights3Rimmed,
		MenuCmd_ConditionalFormattingIconSetTrafficLights3RimmedDescription,
		MenuCmd_ConditionalFormattingIconSetTrafficLights4,
		MenuCmd_ConditionalFormattingIconSetTrafficLights4Description,
		MenuCmd_ConditionalFormattingIconSetSigns3,
		MenuCmd_ConditionalFormattingIconSetSigns3Description,
		MenuCmd_ConditionalFormattingIconSetRedToBlack,
		MenuCmd_ConditionalFormattingIconSetRedToBlackDescription,
		MenuCmd_ConditionalFormattingIconSetSymbols3Circled,
		MenuCmd_ConditionalFormattingIconSetSymbols3CircledDescription,
		MenuCmd_ConditionalFormattingIconSetSymbols3,
		MenuCmd_ConditionalFormattingIconSetSymbols3Description,
		MenuCmd_ConditionalFormattingIconSetFlags3,
		MenuCmd_ConditionalFormattingIconSetFlags3Description,
		MenuCmd_ConditionalFormattingIconSetStars3,
		MenuCmd_ConditionalFormattingIconSetStars3Description,
		MenuCmd_ConditionalFormattingIconSetRatings4,
		MenuCmd_ConditionalFormattingIconSetRatings4Description,
		MenuCmd_ConditionalFormattingIconSetRatings5,
		MenuCmd_ConditionalFormattingIconSetRatings5Description,
		MenuCmd_ConditionalFormattingIconSetQuarters5,
		MenuCmd_ConditionalFormattingIconSetQuarters5Description,
		MenuCmd_ConditionalFormattingIconSetBoxes5,
		MenuCmd_ConditionalFormattingIconSetBoxes5Description,
		MenuCmd_ConditionalFormattingDataBarGradientBlue,
		MenuCmd_ConditionalFormattingDataBarGradientBlueDescription,
		MenuCmd_ConditionalFormattingDataBarSolidBlue,
		MenuCmd_ConditionalFormattingDataBarSolidBlueDescription,
		MenuCmd_ConditionalFormattingDataBarGradientGreen,
		MenuCmd_ConditionalFormattingDataBarGradientGreenDescription,
		MenuCmd_ConditionalFormattingDataBarSolidGreen,
		MenuCmd_ConditionalFormattingDataBarSolidGreenDescription,
		MenuCmd_ConditionalFormattingDataBarGradientRed,
		MenuCmd_ConditionalFormattingDataBarGradientRedDescription,
		MenuCmd_ConditionalFormattingDataBarSolidRed,
		MenuCmd_ConditionalFormattingDataBarSolidRedDescription,
		MenuCmd_ConditionalFormattingDataBarGradientOrange,
		MenuCmd_ConditionalFormattingDataBarGradientOrangeDescription,
		MenuCmd_ConditionalFormattingDataBarSolidOrange,
		MenuCmd_ConditionalFormattingDataBarSolidOrangeDescription,
		MenuCmd_ConditionalFormattingDataBarGradientLightBlue,
		MenuCmd_ConditionalFormattingDataBarGradientLightBlueDescription,
		MenuCmd_ConditionalFormattingDataBarSolidLightBlue,
		MenuCmd_ConditionalFormattingDataBarSolidLightBlueDescription,
		MenuCmd_ConditionalFormattingDataBarGradientPurple,
		MenuCmd_ConditionalFormattingDataBarGradientPurpleDescription,
		MenuCmd_ConditionalFormattingDataBarSolidPurple,
		MenuCmd_ConditionalFormattingDataBarSolidPurpleDescription,
		MenuCmd_ConditionalFormattingTopBottomRuleCommandGroup,
		MenuCmd_ConditionalFormattingTopBottomRuleCommandGroupDescription,
		MenuCmd_ConditionalFormattingTop10RuleCommand,
		MenuCmd_ConditionalFormattingTop10RuleCommandDescription,
		MenuCmd_ConditionalFormattingBottom10RuleCommand,
		MenuCmd_ConditionalFormattingBottom10RuleCommandDescription,
		MenuCmd_ConditionalFormattingTop10PercentRuleCommand,
		MenuCmd_ConditionalFormattingTop10PercentRuleCommandDescription,
		MenuCmd_ConditionalFormattingBottom10PercentRuleCommand,
		MenuCmd_ConditionalFormattingBottom10PercentRuleCommandDescription,
		MenuCmd_ConditionalFormattingAboveAverageRuleCommand,
		MenuCmd_ConditionalFormattingAboveAverageRuleCommandDescription,
		MenuCmd_ConditionalFormattingBelowAverageRuleCommand,
		MenuCmd_ConditionalFormattingBelowAverageRuleCommandDescription,
		MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroup,
		MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroupDescription,
		MenuCmd_ConditionalFormattingGreaterThanRuleCommand,
		MenuCmd_ConditionalFormattingGreaterThanRuleCommandDescription,
		MenuCmd_ConditionalFormattingLessThanRuleCommand,
		MenuCmd_ConditionalFormattingLessThanRuleCommandDescription,
		MenuCmd_ConditionalFormattingEqualToRuleCommand,
		MenuCmd_ConditionalFormattingEqualToRuleCommandDescription,
		MenuCmd_ConditionalFormattingTextContainsRuleCommand,
		MenuCmd_ConditionalFormattingTextContainsRuleCommandDescription,
		MenuCmd_ConditionalFormattingDuplicateValuesRuleCommand,
		MenuCmd_ConditionalFormattingDuplicateValuesRuleCommandDescription,
		MenuCmd_ConditionalFormattingDateOccurringRuleCommand,
		MenuCmd_ConditionalFormattingDateOccurringRuleCommandDescription,
		MenuCmd_ConditionalFormattingBetweenRuleCommand,
		MenuCmd_ConditionalFormattingBetweenRuleCommandDescription,
		MenuCmd_SortAscending,
		MenuCmd_SortAscendingDescription,
		MenuCmd_SortDescending,
		MenuCmd_SortDescendingDescription,
		MenuCmd_DataFilterEquals,
		MenuCmd_DataFilterEqualsDescription,
		MenuCmd_DataFilterDoesNotEqual,
		MenuCmd_DataFilterDoesNotEqualDescription,
		MenuCmd_DataFilterGreaterThan,
		MenuCmd_DataFilterGreaterThanDescription,
		MenuCmd_DataFilterGreaterThanOrEqualTo,
		MenuCmd_DataFilterGreaterThanOrEqualToDescription,
		MenuCmd_DataFilterLessThan,
		MenuCmd_DataFilterLessThanDescription,
		MenuCmd_DataFilterLessThanOrEqualTo,
		MenuCmd_DataFilterLessThanOrEqualToDescription,
		MenuCmd_DataFilterBetween,
		MenuCmd_DataFilterBetweenDescription,
		MenuCmd_DataFilterTop10,
		MenuCmd_DataFilterTop10Description,
		MenuCmd_DataFilterAboveAverage,
		MenuCmd_DataFilterAboveAverageDescription,
		MenuCmd_DataFilterBelowAverage,
		MenuCmd_DataFilterBelowAverageDescription,
		MenuCmd_DataFilterBeginsWith,
		MenuCmd_DataFilterBeginsWithDescription,
		MenuCmd_DataFilterEndsWith,
		MenuCmd_DataFilterEndsWithDescription,
		MenuCmd_DataFilterContains,
		MenuCmd_DataFilterContainsDescription,
		MenuCmd_DataFilterDoesNotContain,
		MenuCmd_DataFilterDoesNotContainDescription,
		MenuCmd_DataFilterToday,
		MenuCmd_DataFilterTodayDescription,
		MenuCmd_DataFilterYesterday,
		MenuCmd_DataFilterYesterdayDescription,
		MenuCmd_DataFilterTomorrow,
		MenuCmd_DataFilterTomorrowDescription,
		MenuCmd_DataFilterThisWeek,
		MenuCmd_DataFilterThisWeekDescription,
		MenuCmd_DataFilterNextWeek,
		MenuCmd_DataFilterNextWeekDescription,
		MenuCmd_DataFilterLastWeek,
		MenuCmd_DataFilterLastWeekDescription,
		MenuCmd_DataFilterThisMonth,
		MenuCmd_DataFilterThisMonthDescription,
		MenuCmd_DataFilterNextMonth,
		MenuCmd_DataFilterNextMonthDescription,
		MenuCmd_DataFilterLastMonth,
		MenuCmd_DataFilterLastMonthDescription,
		MenuCmd_DataFilterThisQuarter,
		MenuCmd_DataFilterThisQuarterDescription,
		MenuCmd_DataFilterNextQuarter,
		MenuCmd_DataFilterNextQuarterDescription,
		MenuCmd_DataFilterLastQuarter,
		MenuCmd_DataFilterLastQuarterDescription,
		MenuCmd_DataFilterThisYear,
		MenuCmd_DataFilterThisYearDescription,
		MenuCmd_DataFilterNextYear,
		MenuCmd_DataFilterNextYearDescription,
		MenuCmd_DataFilterLastYear,
		MenuCmd_DataFilterLastYearDescription,
		MenuCmd_DataFilterYearToDate,
		MenuCmd_DataFilterYearToDateDescription,
		MenuCmd_DataFilterQuarter,
		MenuCmd_DataFilterQuarterDescription,
		MenuCmd_DataFilterMonth,
		MenuCmd_DataFilterMonthDescription,
		MenuCmd_DataFilterCustom,
		MenuCmd_DataFilterCustomDescription,
		MenuCmd_DataFilterClear,
		MenuCmd_DataFilterClearDescription,
		MenuCmd_DataFilterColumnClear,
		MenuCmd_DataFilterColumnClearDescription,
		MenuCmd_DataFilterReApply,
		MenuCmd_DataFilterReApplyDescription,
		MenuCmd_DataFilterToggle,
		MenuCmd_DataFilterToggleDescription,
		MenuCmd_DataFilterDateFilters,
		MenuCmd_DataFilterDateFiltersDescription,
		MenuCmd_DataFilterNumberFilters,
		MenuCmd_DataFilterNumberFiltersDescription,
		MenuCmd_DataFilterTextFilters,
		MenuCmd_DataFilterTextFiltersDescription,
		MenuCmd_DataFilterAllDatesInPeriodFilters,
		MenuCmd_DataFilterAllDatesInPeriodFiltersDescription,
		MenuCmd_DataFilterDateEquals,
		MenuCmd_DataFilterDateEqualsDescription,
		MenuCmd_DataFilterDateAfter,
		MenuCmd_DataFilterDateAfterDescription,
		MenuCmd_DataFilterDateBefore,
		MenuCmd_DataFilterDateBeforeDescription,
		MenuCmd_DataFilterDateBetween,
		MenuCmd_DataFilterDateBetweenDescription,
		MenuCmd_DataFilterDateCustom,
		MenuCmd_DataFilterDateCustomDescription,
		MenuCmd_DataFilterSimple,
		MenuCmd_DataFilterSimpleDescription,
		MenuCmd_DataValidation,
		MenuCmd_DataValidationDescription,
		MenuCmd_DataValidationCircleInvalidData,
		MenuCmd_DataValidationCircleInvalidDataDescription,
		MenuCmd_DataValidationClearValidationCircle,
		MenuCmd_DataValidationClearValidationCircleDescription,
		MenuCmd_InsertChartColumnCommandGroup,
		MenuCmd_InsertChartColumnCommandGroupDescription,
		MenuCmd_InsertChartBarCommandGroup,
		MenuCmd_InsertChartBarCommandGroupDescription,
		MenuCmd_InsertChartColumn2DCommandGroup,
		MenuCmd_InsertChartColumn2DCommandGroupDescription,
		MenuCmd_InsertChartColumn3DCommandGroup,
		MenuCmd_InsertChartColumn3DCommandGroupDescription,
		MenuCmd_InsertChartCylinderCommandGroup,
		MenuCmd_InsertChartCylinderCommandGroupDescription,
		MenuCmd_InsertChartConeCommandGroup,
		MenuCmd_InsertChartConeCommandGroupDescription,
		MenuCmd_InsertChartPyramidCommandGroup,
		MenuCmd_InsertChartPyramidCommandGroupDescription,
		MenuCmd_InsertChartBar2DCommandGroup,
		MenuCmd_InsertChartBar2DCommandGroupDescription,
		MenuCmd_InsertChartBar3DCommandGroup,
		MenuCmd_InsertChartBar3DCommandGroupDescription,
		MenuCmd_InsertChartHorizontalCylinderCommandGroup,
		MenuCmd_InsertChartHorizontalCylinderCommandGroupDescription,
		MenuCmd_InsertChartHorizontalConeCommandGroup,
		MenuCmd_InsertChartHorizontalConeCommandGroupDescription,
		MenuCmd_InsertChartHorizontalPyramidCommandGroup,
		MenuCmd_InsertChartHorizontalPyramidCommandGroupDescription,
		MenuCmd_InsertChartPieCommandGroup,
		MenuCmd_InsertChartPieCommandGroupDescription,
		MenuCmd_InsertChartPie2DCommandGroup,
		MenuCmd_InsertChartPie2DCommandGroupDescription,
		MenuCmd_InsertChartPie3DCommandGroup,
		MenuCmd_InsertChartPie3DCommandGroupDescription,
		MenuCmd_InsertChartDoughnut2DCommandGroup,
		MenuCmd_InsertChartDoughnut2DCommandGroupDescription,
		MenuCmd_InsertChartLineCommandGroup,
		MenuCmd_InsertChartLineCommandGroupDescription,
		MenuCmd_InsertChartLine2DCommandGroup,
		MenuCmd_InsertChartLine2DCommandGroupDescription,
		MenuCmd_InsertChartLine3DCommandGroup,
		MenuCmd_InsertChartLine3DCommandGroupDescription,
		MenuCmd_InsertChartBubbleCommandGroup,
		MenuCmd_InsertChartBubbleCommandGroupDescription,
		MenuCmd_InsertChartOtherCommandGroup,
		MenuCmd_InsertChartOtherCommandGroupDescription,
		MenuCmd_InsertChartStockCommandGroup,
		MenuCmd_InsertChartStockCommandGroupDescription,
		MenuCmd_InsertChartRadarCommandGroup,
		MenuCmd_InsertChartRadarCommandGroupDescription,
		MenuCmd_InsertChartColumnClustered2D,
		MenuCmd_InsertChartColumnClustered2DDescription,
		MenuCmd_InsertChartColumnStacked2D,
		MenuCmd_InsertChartColumnStacked2DDescription,
		MenuCmd_InsertChartColumnPercentStacked2D,
		MenuCmd_InsertChartColumnPercentStacked2DDescription,
		MenuCmd_InsertChartColumnClustered3D,
		MenuCmd_InsertChartColumnClustered3DDescription,
		MenuCmd_InsertChartColumnStacked3D,
		MenuCmd_InsertChartColumnStacked3DDescription,
		MenuCmd_InsertChartColumnPercentStacked3D,
		MenuCmd_InsertChartColumnPercentStacked3DDescription,
		MenuCmd_InsertChartCylinderClustered,
		MenuCmd_InsertChartCylinderClusteredDescription,
		MenuCmd_InsertChartCylinderStacked,
		MenuCmd_InsertChartCylinderStackedDescription,
		MenuCmd_InsertChartCylinderPercentStacked,
		MenuCmd_InsertChartCylinderPercentStackedDescription,
		MenuCmd_InsertChartConeClustered,
		MenuCmd_InsertChartConeClusteredDescription,
		MenuCmd_InsertChartConeStacked,
		MenuCmd_InsertChartConeStackedDescription,
		MenuCmd_InsertChartConePercentStacked,
		MenuCmd_InsertChartConePercentStackedDescription,
		MenuCmd_InsertChartPyramidClustered,
		MenuCmd_InsertChartPyramidClusteredDescription,
		MenuCmd_InsertChartPyramidStacked,
		MenuCmd_InsertChartPyramidStackedDescription,
		MenuCmd_InsertChartPyramidPercentStacked,
		MenuCmd_InsertChartPyramidPercentStackedDescription,
		MenuCmd_InsertChartBarClustered2D,
		MenuCmd_InsertChartBarClustered2DDescription,
		MenuCmd_InsertChartBarStacked2D,
		MenuCmd_InsertChartBarStacked2DDescription,
		MenuCmd_InsertChartBarPercentStacked2D,
		MenuCmd_InsertChartBarPercentStacked2DDescription,
		MenuCmd_InsertChartBarClustered3D,
		MenuCmd_InsertChartBarClustered3DDescription,
		MenuCmd_InsertChartBarStacked3D,
		MenuCmd_InsertChartBarStacked3DDescription,
		MenuCmd_InsertChartBarPercentStacked3D,
		MenuCmd_InsertChartBarPercentStacked3DDescription,
		MenuCmd_InsertChartHorizontalCylinderClustered,
		MenuCmd_InsertChartHorizontalCylinderClusteredDescription,
		MenuCmd_InsertChartHorizontalCylinderStacked,
		MenuCmd_InsertChartHorizontalCylinderStackedDescription,
		MenuCmd_InsertChartHorizontalCylinderPercentStacked,
		MenuCmd_InsertChartHorizontalCylinderPercentStackedDescription,
		MenuCmd_InsertChartHorizontalConeClustered,
		MenuCmd_InsertChartHorizontalConeClusteredDescription,
		MenuCmd_InsertChartHorizontalConeStacked,
		MenuCmd_InsertChartHorizontalConeStackedDescription,
		MenuCmd_InsertChartHorizontalConePercentStacked,
		MenuCmd_InsertChartHorizontalConePercentStackedDescription,
		MenuCmd_InsertChartHorizontalPyramidClustered,
		MenuCmd_InsertChartHorizontalPyramidClusteredDescription,
		MenuCmd_InsertChartHorizontalPyramidStacked,
		MenuCmd_InsertChartHorizontalPyramidStackedDescription,
		MenuCmd_InsertChartHorizontalPyramidPercentStacked,
		MenuCmd_InsertChartHorizontalPyramidPercentStackedDescription,
		MenuCmd_InsertChartColumn3D,
		MenuCmd_InsertChartColumn3DDescription,
		MenuCmd_InsertChartCylinder,
		MenuCmd_InsertChartCylinderDescription,
		MenuCmd_InsertChartCone,
		MenuCmd_InsertChartConeDescription,
		MenuCmd_InsertChartPyramid,
		MenuCmd_InsertChartPyramidDescription,
		MenuCmd_InsertChartPie2D,
		MenuCmd_InsertChartPie2DDescription,
		MenuCmd_InsertChartPieExploded2D,
		MenuCmd_InsertChartPieExploded2DDescription,
		MenuCmd_InsertChartPie3D,
		MenuCmd_InsertChartPie3DDescription,
		MenuCmd_InsertChartPieExploded3D,
		MenuCmd_InsertChartPieExploded3DDescription,
		MenuCmd_InsertChartDoughnut2D,
		MenuCmd_InsertChartDoughnut2DDescription,
		MenuCmd_InsertChartDoughnutExploded2D,
		MenuCmd_InsertChartDoughnutExploded2DDescription,
		MenuCmd_InsertChartLine,
		MenuCmd_InsertChartLineDescription,
		MenuCmd_InsertChartStackedLine,
		MenuCmd_InsertChartStackedLineDescription,
		MenuCmd_InsertChartPercentStackedLine,
		MenuCmd_InsertChartPercentStackedLineDescription,
		MenuCmd_InsertChartLineWithMarkers,
		MenuCmd_InsertChartLineWithMarkersDescription,
		MenuCmd_InsertChartStackedLineWithMarkers,
		MenuCmd_InsertChartStackedLineWithMarkersDescription,
		MenuCmd_InsertChartPercentStackedLineWithMarkers,
		MenuCmd_InsertChartPercentStackedLineWithMarkersDescription,
		MenuCmd_InsertChartLine3D,
		MenuCmd_InsertChartLine3DDescription,
		MenuCmd_InsertChartAreaCommandGroup,
		MenuCmd_InsertChartAreaCommandGroupDescription,
		MenuCmd_InsertChartArea2DCommandGroup,
		MenuCmd_InsertChartArea2DCommandGroupDescription,
		MenuCmd_InsertChartArea3DCommandGroup,
		MenuCmd_InsertChartArea3DCommandGroupDescription,
		MenuCmd_InsertChartArea,
		MenuCmd_InsertChartAreaDescription,
		MenuCmd_InsertChartStackedArea,
		MenuCmd_InsertChartStackedAreaDescription,
		MenuCmd_InsertChartPercentStackedArea,
		MenuCmd_InsertChartPercentStackedAreaDescription,
		MenuCmd_InsertChartArea3D,
		MenuCmd_InsertChartArea3DDescription,
		MenuCmd_InsertChartStackedArea3D,
		MenuCmd_InsertChartStackedArea3DDescription,
		MenuCmd_InsertChartPercentStackedArea3D,
		MenuCmd_InsertChartPercentStackedArea3DDescription,
		MenuCmd_InsertChartScatterCommandGroup,
		MenuCmd_InsertChartScatterCommandGroupDescription,
		MenuCmd_InsertChartScatterMarkers,
		MenuCmd_InsertChartScatterMarkersDescription,
		MenuCmd_InsertChartScatterLines,
		MenuCmd_InsertChartScatterLinesDescription,
		MenuCmd_InsertChartScatterSmoothLines,
		MenuCmd_InsertChartScatterSmoothLinesDescription,
		MenuCmd_InsertChartScatterLinesAndMarkers,
		MenuCmd_InsertChartScatterLinesAndMarkersDescription,
		MenuCmd_InsertChartScatterSmoothLinesAndMarkers,
		MenuCmd_InsertChartScatterSmoothLinesAndMarkersDescription,
		MenuCmd_InsertChartBubble,
		MenuCmd_InsertChartBubbleDescription,
		MenuCmd_InsertChartBubble3D,
		MenuCmd_InsertChartBubble3DDescription,
		MenuCmd_InsertChartStockHighLowClose,
		MenuCmd_InsertChartStockHighLowCloseDescription,
		MenuCmd_InsertChartStockOpenHighLowClose,
		MenuCmd_InsertChartStockOpenHighLowCloseDescription,
		MenuCmd_InsertChartRadar,
		MenuCmd_InsertChartRadarDescription,
		MenuCmd_InsertChartRadarWithMarkers,
		MenuCmd_InsertChartRadarWithMarkersDescription,
		MenuCmd_InsertChartRadarFilled,
		MenuCmd_InsertChartRadarFilledDescription,
		MenuCmd_InsertChartStockVolumeHighLowClose,
		MenuCmd_InsertChartStockVolumeHighLowCloseDescription,
		MenuCmd_InsertChartStockVolumeOpenHighLowClose,
		MenuCmd_InsertChartStockVolumeOpenHighLowCloseDescription,
		MenuCmd_ChartAxesCommandGroup,
		MenuCmd_ChartAxesCommandGroupDescription,
		MenuCmd_ChartPrimaryHorizontalAxisCommandGroup,
		MenuCmd_ChartPrimaryHorizontalAxisCommandGroupDescription,
		MenuCmd_ChartPrimaryVerticalAxisCommandGroup,
		MenuCmd_ChartPrimaryVerticalAxisCommandGroupDescription,
		MenuCmd_ChartHidePrimaryHorizontalAxisCommand,
		MenuCmd_ChartHidePrimaryHorizontalAxisCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommand,
		MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommand,
		MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommand,
		MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisDefaultCommand,
		MenuCmd_ChartPrimaryHorizontalAxisDefaultCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommand,
		MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommand,
		MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommand,
		MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommand,
		MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommandDescription,
		MenuCmd_ChartHidePrimaryVerticalAxisCommand,
		MenuCmd_ChartHidePrimaryVerticalAxisCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommand,
		MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommand,
		MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommand,
		MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisDefaultCommand,
		MenuCmd_ChartPrimaryVerticalAxisDefaultCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommand,
		MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommand,
		MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommand,
		MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommand,
		MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommandDescription,
		MenuCmd_ChartGridlinesCommandGroup,
		MenuCmd_ChartGridlinesCommandGroupDescription,
		MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroup,
		MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroupDescription,
		MenuCmd_ChartPrimaryVerticalGridlinesCommandGroup,
		MenuCmd_ChartPrimaryVerticalGridlinesCommandGroupDescription,
		MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommand,
		MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommandDescription,
		MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommand,
		MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommandDescription,
		MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommand,
		MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommandDescription,
		MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommand,
		MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommandDescription,
		MenuCmd_ChartPrimaryVerticalGridlinesNoneCommand,
		MenuCmd_ChartPrimaryVerticalGridlinesNoneCommandDescription,
		MenuCmd_ChartPrimaryVerticalGridlinesMajorCommand,
		MenuCmd_ChartPrimaryVerticalGridlinesMajorCommandDescription,
		MenuCmd_ChartPrimaryVerticalGridlinesMinorCommand,
		MenuCmd_ChartPrimaryVerticalGridlinesMinorCommandDescription,
		MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommand,
		MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommandDescription,
		MenuCmd_ChartAxisTitlesCommandGroup,
		MenuCmd_ChartAxisTitlesCommandGroupDescription,
		MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroup,
		MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroupDescription,
		MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroup,
		MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroupDescription,
		MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommand,
		MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommandDescription,
		MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommand,
		MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommand,
		MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommand,
		MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommand,
		MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommandDescription,
		MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommand,
		MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommandDescription,
		MenuCmd_ChartTitleCommandGroup,
		MenuCmd_ChartTitleCommandGroupDescription,
		MenuCmd_ChartTitleNoneCommand,
		MenuCmd_ChartTitleNoneCommandDescription,
		MenuCmd_ChartTitleCenteredOverlayCommand,
		MenuCmd_ChartTitleCenteredOverlayCommandDescription,
		MenuCmd_ChartTitleAboveCommand,
		MenuCmd_ChartTitleAboveCommandDescription,
		MenuCmd_ChartLegendCommandGroup,
		MenuCmd_ChartLegendCommandGroupDescription,
		MenuCmd_ChartLegendNoneCommand,
		MenuCmd_ChartLegendNoneCommandDescription,
		MenuCmd_ChartLegendAtRightCommand,
		MenuCmd_ChartLegendAtRightCommandDescription,
		MenuCmd_ChartLegendAtTopCommand,
		MenuCmd_ChartLegendAtTopCommandDescription,
		MenuCmd_ChartLegendAtLeftCommand,
		MenuCmd_ChartLegendAtLeftCommandDescription,
		MenuCmd_ChartLegendAtBottomCommand,
		MenuCmd_ChartLegendAtBottomCommandDescription,
		MenuCmd_ChartLegendOverlayAtRightCommand,
		MenuCmd_ChartLegendOverlayAtRightCommandDescription,
		MenuCmd_ChartLegendOverlayAtLeftCommand,
		MenuCmd_ChartLegendOverlayAtLeftCommandDescription,
		MenuCmd_ChartDataLabelsCommandGroup,
		MenuCmd_ChartDataLabelsCommandGroupDescription,
		MenuCmd_ChartDataLabelsNoneCommand,
		MenuCmd_ChartDataLabelsNoneCommandDescription,
		MenuCmd_ChartDataLabelsDefaultCommand,
		MenuCmd_ChartDataLabelsDefaultCommandDescription,
		MenuCmd_ChartDataLabelsCenterCommand,
		MenuCmd_ChartDataLabelsCenterCommandDescription,
		MenuCmd_ChartDataLabelsInsideEndCommand,
		MenuCmd_ChartDataLabelsInsideEndCommandDescription,
		MenuCmd_ChartDataLabelsInsideBaseCommand,
		MenuCmd_ChartDataLabelsInsideBaseCommandDescription,
		MenuCmd_ChartDataLabelsOutsideEndCommand,
		MenuCmd_ChartDataLabelsOutsideEndCommandDescription,
		MenuCmd_ChartDataLabelsBestFitCommand,
		MenuCmd_ChartDataLabelsBestFitCommandDescription,
		MenuCmd_ChartDataLabelsLeftCommand,
		MenuCmd_ChartDataLabelsLeftCommandDescription,
		MenuCmd_ChartDataLabelsRightCommand,
		MenuCmd_ChartDataLabelsRightCommandDescription,
		MenuCmd_ChartDataLabelsAboveCommand,
		MenuCmd_ChartDataLabelsAboveCommandDescription,
		MenuCmd_ChartDataLabelsBelowCommand,
		MenuCmd_ChartDataLabelsBelowCommandDescription,
		MenuCmd_ChartLinesCommandGroup,
		MenuCmd_ChartLinesCommandGroupDescription,
		MenuCmd_ChartLinesNoneCommand,
		MenuCmd_ChartLinesNoneCommandDescription,
		MenuCmd_ChartShowDropLinesCommand,
		MenuCmd_ChartShowDropLinesCommandDescription,
		MenuCmd_ChartShowHighLowLinesCommand,
		MenuCmd_ChartShowHighLowLinesCommandDescription,
		MenuCmd_ChartShowDropLinesAndHighLowLinesCommand,
		MenuCmd_ChartShowDropLinesAndHighLowLinesCommandDescription,
		MenuCmd_ChartShowSeriesLinesCommand,
		MenuCmd_ChartShowSeriesLinesCommandDescription,
		MenuCmd_ChartUpDownBarsCommandGroup,
		MenuCmd_ChartUpDownBarsCommandGroupDescription,
		MenuCmd_ChartHideUpDownBarsCommand,
		MenuCmd_ChartHideUpDownBarsCommandDescription,
		MenuCmd_ChartShowUpDownBarsCommand,
		MenuCmd_ChartShowUpDownBarsCommandDescription,
		MenuCmd_ChartErrorBarsCommandGroup,
		MenuCmd_ChartErrorBarsCommandGroupDescription,
		MenuCmd_ChartErrorBarsNoneCommand,
		MenuCmd_ChartErrorBarsNoneCommandDescription,
		MenuCmd_ChartErrorBarsPercentageCommand,
		MenuCmd_ChartErrorBarsPercentageCommandDescription,
		MenuCmd_ChartErrorBarsStandardErrorCommand,
		MenuCmd_ChartErrorBarsStandardErrorCommandDescription,
		MenuCmd_ChartErrorBarsStandardDeviationCommand,
		MenuCmd_ChartErrorBarsStandardDeviationCommandDescription,
		MenuCmd_ChartSwitchRowColumnCommand,
		MenuCmd_ChartSwitchRowColumnCommandDescription,
		MenuCmd_ChangeChartType,
		MenuCmd_ChangeChartTypeDescription,
		MenuCmd_ChangeChartTypeContextMenuItem,
		MenuCmd_ChartSelectData,
		MenuCmd_ChartSelectDataDescription,
		MenuCmd_ChartSelectDataContextMenuItem,
		MenuCmd_ChartChangeTitleContextMenuItem,
		MenuCmd_ChartChangeTitleContextMenuItemDescription,
		MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItem,
		MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItemDescription,
		MenuCmd_ChartChangeVerticalAxisTitleContextMenuItem,
		MenuCmd_ChartChangeVerticalAxisTitleContextMenuItemDescription,
		MenuCmd_MoveToNextSheetDescription,
		MenuCmd_MoveToPreviousSheetDescription,
		Msg_ErrorIncorrectErrorBarsCollectionCount,
		Msg_ErrorErrorBarsWithBarDirectionAlreadyExists,
		StyleName_Normal,
		Caption_PasteSpecial_All,
		Caption_PasteSpecial_Formulas,
		Caption_PasteSpecial_Values,
		Caption_PasteSpecial_Formats,
		Caption_PasteSpecial_Comments,
		Caption_PasteSpecial_AllExceptBorders,
		Caption_PasteSpecial_ColumnWidths,
		Caption_PasteSpecial_FormulasAndNumberFormats,
		Caption_PasteSpecial_ValuesAndNumberFormats,
		Caption_Subtotal_ColumnHeader,
		Caption_Subtotal_Grand,
		Caption_Subtotal_SumName,
		MoveOrCopySheetForm_Move,
		MoveOrCopySheetForm_Copy,
		MoveOrCopySheetForm_MoveToEnd,
		FontStyle_Regular,
		FontStyle_Italic,
		FontStyle_Bold,
		FontStyle_BoldItalic,
		Caption_FormatNumberCustom,
		Caption_FormatNumberDate,
		NameBox_SelectionModeFormat,
		Msg_ErrorIncorectRotationAngleValue,
		Msg_OdsCaseSensitiveIgnored,
		Msg_OdsCalendarIgnored,
		Msg_OdsUnknownDateFormat,
		Msg_OdsTooLowDateValue,
		Msg_OdsTimeFormatIgnored,
		Msg_ErrorCommandCannotPerformedWithMultipleSelections,
		Msg_BuiltInFunctionNotFound,
		Msg_BuiltInReplaceInvalidVolatile,
		Msg_BuiltInReplaceInvalidReturnType,
		Msg_BuiltInReplaceInvalidParametersCount,
		Msg_BuiltInReplaceInvalidParameter,
		MenuCmd_TableToolsToggleHeaderRowCommand,
		MenuCmd_TableToolsToggleHeaderRowCommandDescription,
		MenuCmd_TableToolsToggleTotalRowCommand,
		MenuCmd_TableToolsToggleTotalRowCommandDescription,
		MenuCmd_TableToolsToggleBandedColumnsCommand,
		MenuCmd_TableToolsToggleBandedColumnsCommandDescription,
		MenuCmd_TableToolsToggleBandedRowsCommand,
		MenuCmd_TableToolsToggleBandedRowsCommandDescription,
		MenuCmd_TableToolsToggleFirstColumnCommand,
		MenuCmd_TableToolsToggleFirstColumnCommandDescription,
		MenuCmd_TableToolsToggleLastColumnCommand,
		MenuCmd_TableToolsToggleLastColumnCommandDescription,
		MenuCmd_TableToolsRenameTableCommand,
		MenuCmd_TableToolsRenameTableCommandDescription,
		MenuCmd_TableToolsConvertToRange,
		MenuCmd_TableToolsConvertToRangeDescription,
		Msg_ErrorDataTableExporterOverflow,
		Msg_ErrorDataTableExporterConversionError,
		Msg_ErrorNoColumnInDataTable,
		Msg_ErrorNoColumnsInDataTable,
		Msg_ErrorRangeColumnCountIsNotTheSameAsColumnCountInDataTable,
		Msg_ErrorConditionalFormattingRank,
		MenuCmd_MailMergeSelectDataMemberCommandDescription,
		MenuCmd_MailMergeSelectDataMemberCommand,
		MenuCmd_MailMergeAddDataSourceCommand,
		MenuCmd_MailMergeAddDataSourceCommandDescription,
		MenuCmd_MailMergeSelectDataSourceCommand,
		MenuCmd_MailMergeSelectDataSourceCommandDescription,
		MenuCmd_MailMergeDocumentsModeCommand,
		MenuCmd_MailMergeDocumentsModeCommandDescription,
		MenuCmd_MailMergeOneDocumentModeCommand,
		MenuCmd_MailMergeOneDocumentModeCommandDescription,
		MenuCmd_MailMergeOneSheetModeCommand,
		MenuCmd_MailMergeOneSheetModeCommandDescription,
		MenuCmd_MailMergeHorizontalModeCommand,
		MenuCmd_MailMergeHorizontalModeCommandDescription,
		MenuCmd_MailMergeVerticalModeCommand,
		MenuCmd_MailMergeVerticalModeCommandDescription,
		MenuCmd_MailMergeOrientationCommandGroup,
		MenuCmd_MailMergeOrientationCommandGroupDescription,
		MenuCmd_MailMergeResetRangeCommand,
		MenuCmd_MailMergeResetRangeCommandDescription,
		MenuCmd_MailMergeSetDetailRangeCommand,
		MenuCmd_MailMergeSetDetailRangeCommandDescription,
		MenuCmd_MailMergeSetGroupCommand,
		MenuCmd_MailMergeSetGroupCommandDescription,
		MenuCmd_MailMergeSetGroupHeaderCommand,
		MenuCmd_MailMergeSetGroupHeaderCommandDescription,
		MenuCmd_MailMergeSetGroupFooterCommand,
		MenuCmd_MailMergeSetGroupFooterCommandDescription,
		MenuCmd_MailMergeSetFilterCommand,
		MenuCmd_MailMergeSetFilterCommandDescription,
		MenuCmd_MailMergeResetFilterCommand,
		MenuCmd_MailMergeResetFilterCommandDescription,
		MenuCmd_MailMergeSetFooterRangeCommand,
		MenuCmd_MailMergeSetFooterRangeCommandDescription,
		MenuCmd_MailMergeSetHeaderRangeCommand,
		MenuCmd_MailMergeSetHeaderRangeCommandDescription,
		MenuCmd_EditingMailMergeMasterDetailCommandGroupDescription,
		MenuCmd_EditingMailMergeMasterDetailCommandGroup,
		MenuCmd_MailMergeManageQueriesCommand,
		MenuCmd_MailMergeManageRelationsCommand,
		MenuCmd_MailMergeManageRelationsCommandGroup,
		MenuCmd_MailMergeManageRelationsCommandGroupDescription,
		MenuCmd_MailMergeManageDataSourceCommandGroup,
		MenuCmd_MailMergeManageDataSourceCommandGroupDescription,
		MenuCmd_MailMergeManageDataSourcesCommand,
		MenuCmd_MailMergeSetDetailLevelCommandDescription,
		MenuCmd_MailMergeSetDetailLevelCommand,
		MenuCmd_MailMergeSetDetailDataMemberCommandDescription,
		MenuCmd_MailMergeSetDetailDataMemberCommand,
		MenuCmd_MailMergeShowRangesCommandDescription,
		MenuCmd_MailMergeShowRangesCommand,
		MenuCmd_MailMergePreviewCommand,
		MenuCmd_MailMergePreviewCommandDescription,
		MenuCmd_ReviewProtectSheet,
		MenuCmd_ReviewProtectSheetDescription,
		MenuCmd_ReviewProtectSheetContextMenuItem,
		MenuCmd_ReviewUnprotectSheet,
		MenuCmd_ReviewUnprotectSheetDescription,
		MenuCmd_ReviewUnprotectSheetContextMenuItem,
		MenuCmd_ReviewProtectWorkbook,
		MenuCmd_ReviewProtectWorkbookDescription,
		MenuCmd_ReviewUnprotectWorkbook,
		MenuCmd_ReviewUnprotectWorkbookDescription,
		MenuCmd_ReviewShowProtectedRangeManager,
		MenuCmd_ReviewShowProtectedRangeManagerDescription,
		MenuCmd_ReviewInsertCommentContextMenuItem,
		MenuCmd_ReviewInsertCommentContextMenuItemDescription,
		MenuCmd_ReviewDeleteCommentContextMenuItem,
		MenuCmd_ReviewDeleteCommentContextMenuItemDescription,
		MenuCmd_ReviewShowHideComment,
		MenuCmd_ReviewShowHideCommentDescription,
		MenuCmd_InsertTable,
		MenuCmd_InsertTableDescription,
		MenuCmd_FormatAsTable,
		MenuCmd_FormatAsTableDescription,
		MenuCmd_EditingSortAndFilterCommandGroup,
		MenuCmd_EditingSortAndFilterCommandGroupDescription,
		MenuCmd_EditingFindAndSelectCommandGroup,
		MenuCmd_EditingFindAndSelectCommandGroupDescription,
		MenuCmd_EditingFind,
		MenuCmd_EditingFindDescription,
		MenuCmd_EditingReplace,
		MenuCmd_EditingReplaceDescription,
		MenuCmd_EditingSelectFormulas,
		MenuCmd_EditingSelectFormulasDescription,
		MenuCmd_EditingSelectComments,
		MenuCmd_EditingSelectCommentsDescription,
		MenuCmd_EditingSelectConditionalFormatting,
		MenuCmd_EditingSelectConditionalFormattingDescription,
		MenuCmd_EditingSelectConstants,
		MenuCmd_EditingSelectConstantsDescription,
		MenuCmd_EditingSelectDataValidation,
		MenuCmd_EditingSelectDataValidationDescription,
		MenuCmd_DefineNameCommandGroup,
		MenuCmd_DefineNameCommandGroupDescription,
		MenuCmd_DefineNameCommand,
		MenuCmd_DefineNameCommandDescription,
		MenuCmd_ShowNameManager,
		MenuCmd_ShowNameManagerDescription,
		MenuCmd_CreateDefinedNamesFromSelection,
		MenuCmd_CreateDefinedNamesFromSelectionDescription,
		MenuCmd_InsertDefinedName,
		MenuCmd_InsertDefinedNameDescription,
		MenuCmd_InsertDefinedNameCommandGroup,
		MenuCmd_InsertDefinedNameCommandGroupDescription,
		MenuCmd_CalculateNow,
		MenuCmd_CalculateNowDescription,
		MenuCmd_CalculateSheet,
		MenuCmd_CalculateSheetDescription,
		MenuCmd_CalculationOptionsCommandGroup,
		MenuCmd_CalculationOptionsCommandGroupDescription,
		MenuCmd_CalculationModeAutomatic,
		MenuCmd_CalculationModeAutomaticDescription,
		MenuCmd_CalculationModeManual,
		MenuCmd_CalculationModeManualDescription,
		ConditionalFormattingStyle_LightRedFillWithDarkRedText,
		ConditionalFormattingStyle_YellowFillWithDarkYellowText,
		ConditionalFormattingStyle_GreenFillWithDarkGreenText,
		ConditionalFormattingStyle_LightRedFill,
		ConditionalFormattingStyle_RedText,
		ConditionalFormattingStyle_RedBorder,
		Caption_TimePeriod_Yesterday,
		Caption_TimePeriod_Today,
		Caption_TimePeriod_Tomorrow,
		Caption_TimePeriod_Last7Days,
		Caption_TimePeriod_LastWeek,
		Caption_TimePeriod_ThisWeek,
		Caption_TimePeriod_NextWeek,
		Caption_TimePeriod_LastMonth,
		Caption_TimePeriod_ThisMonth,
		Caption_TimePeriod_NextMonth,
		Caption_CustomPropertyTypeText,
		Caption_CustomPropertyTypeNumber,
		Caption_CustomPropertyTypeDateTime,
		Caption_CustomPropertyTypeBoolean,
		Caption_CustomPropertyYes,
		Caption_CustomPropertyNo,
		Caption_CustomPropertyCheckedBy,
		Caption_CustomPropertyClient,
		Caption_CustomPropertyDateCompleted,
		Caption_CustomPropertyDepartment,
		Caption_CustomPropertyDestination,
		Caption_CustomPropertyDisposition,
		Caption_CustomPropertyDivision,
		Caption_CustomPropertyDocumentNumber,
		Caption_CustomPropertyEditor,
		Caption_CustomPropertyForwardTo,
		Caption_CustomPropertyGroup,
		Caption_CustomPropertyLanguage,
		Caption_CustomPropertyMailstop,
		Caption_CustomPropertyMatter,
		Caption_CustomPropertyOffice,
		Caption_CustomPropertyOwner,
		Caption_CustomPropertyProject,
		Caption_CustomPropertyPublisher,
		Caption_CustomPropertyPurpose,
		Caption_CustomPropertyReceivedFrom,
		Caption_CustomPropertyRecordedBy,
		Caption_CustomPropertyRecordedDate,
		Caption_CustomPropertyReference,
		Caption_CustomPropertySource,
		Caption_CustomPropertyStatus,
		Caption_CustomPropertyTelephoneNumber,
		Caption_CustomPropertyTypist,
		Caption_ChangeChartTitleFormLabel,
		Caption_ChangeChartHorizontalAxisTitleFormLabel,
		Caption_ChangeChartVerticalAxisTitleFormLabel,
		Msg_ErrorAddCondFmtToIncorrectSheet,
		Caption_CreateTable,
		Caption_GroupTableTools,
		Msg_ErrorResizeRangeToMergedCell,
		Msg_ShiftCellsInATableIsNotAllowed,
		Msg_ShiftCellInAutoFilterIsNotAllowed,
		Msg_ChangingRangeOfAutoFilterNotAllowed,
		Msg_IntersectedRangesCanNotBeChanged,
		Msg_DiffRangeTypesCanNotBeChanged,
		Msg_UnmergeMergedCellsClarification,
		Msg_TableOverlap,
		Msg_ErrorTableCannotBeCreatedInTheLastRow,
		Msg_ErrorInvalidObjectUsage,
		Msg_CondFmtExpressionCantBeAnArray,
		Msg_CondFmtExpressionCantContainErrorValues,
		Msg_CondFmtExpressionCantContainRelativeRefs,
		Msg_NoCellsWereFound,
		FormatCellsForm_NumberDescription_General,
		FormatCellsForm_NumberDescription_Text,
		FormatCellsForm_CategoryDescription_Number,
		FormatCellsForm_CategoryDescription_Currency,
		FormatCellsForm_CategoryDescription_Accounting,
		FormatCellsForm_CategoryDescription_Date,
		FormatCellsForm_CategoryDescription_Time,
		FormatCellsForm_CategoryDescription_Percentage,
		FormatCellsForm_CategoryDescription_Custom,
		FormatCellsForm_HorizontalAlignmentCaption_General,
		FormatCellsForm_HorizontalAlignmentCaption_LeftIndent,
		FormatCellsForm_HorizontalAlignmentCaption_Center,
		FormatCellsForm_HorizontalAlignmentCaption_RightIndent,
		FormatCellsForm_HorizontalAlignmentCaption_Fill,
		FormatCellsForm_HorizontalAlignmentCaption_Justify,
		FormatCellsForm_HorizontalAlignmentCaption_CenterAcrossSelection,
		FormatCellsForm_HorizontalAlignmentCaption_DistributedIndent,
		FormatCellsForm_VerticalAlignmentCaption_Top,
		FormatCellsForm_VerticalAlignmentCaption_Center,
		FormatCellsForm_VerticalAlignmentCaption_Bottom,
		FormatCellsForm_VerticalAlignmentCaption_Justify,
		FormatCellsForm_VerticalAlignmentCaption_Distributed,
		FormatCellsForm_TextDirectionCaption_Context,
		FormatCellsForm_TextDirectionCaption_LeftToRight,
		FormatCellsForm_TextDirectionCaption_RightToLeft,
		FormatCellsForm_UnderlineCaption_None,
		FormatCellsForm_UnderlineCaption_Single,
		FormatCellsForm_UnderlineCaption_Double,
		FormatCellsForm_UnderlineCaption_SingleAccounting,
		FormatCellsForm_UnderlineCaption_DoubleAccounting,
		FieldListDockPanel_Text,
		MailMergeParametersDockPanel_Text,
		MailMergeParametersDockPanel_NameColumn,
		MailMergeParametersDockPanel_ValueColumn,
		MailMergeParametersDockPanel_EditParameters,
		MenuCmd_AutoOutlineCommandDescription,
		MenuCmd_AutoOutlineCommand,
		MenuCmd_ClearOutlineCommandCommandDescription,
		MenuCmd_ClearOutlineCommandCommand,
		MenuCmd_GroupCommandDescription,
		MenuCmd_GroupCommand,
		MenuCmd_HideDetailCommandDescription,
		MenuCmd_HideDetailModeCommand,
		MenuCmd_ShowDetailCommandDescription,
		MenuCmd_ShowDetailCommand,
		MenuCmd_SubtotalCommandDescription,
		MenuCmd_SubtotalCommand,
		MenuCmd_UngroupCommandDescription,
		MenuCmd_UngroupCommand,
		MenuCmd_GroupFormCaption,
		MenuCmd_UngroupFormCaption,
		MenuCmd_OutlineGroupCommandGroupDescription,
		MenuCmd_OutlineUngroupCommandGroupDescription,
		MenuCmd_OutlineDialogCommand,
		MenuCmd_OutlineDialogCommandDescription,
		CaptionAllFunctionsGroup,
		CaptionDatabaseFunctionsGroup,
		CaptionUserDefinedFunctionsGroup,
		DefaultWorksheetName,
		DefaultRecoveredWorksheetName,
		NewSpreadsheetMailMergeParameter_Text,
		EditSpreadsheetMailMergeParameter_Text,
		Msg_ParametersInvalidCharacters,
		Msg_ParametersNoName,
		Msg_ParametersIdenticalNames,
		MenuCmd_PageSetup,
		MenuCmd_PageSetupDescription,
		MenuCmd_PageSetupMargins,
		MenuCmd_PageSetupMarginsDescription,
		MenuCmd_PageSetupHeaderFooter,
		MenuCmd_PageSetupHeaderFooterDescription,
		MenuCmd_PageSetupSheet,
		MenuCmd_PageSetupSheetDescription,
		MenuCmd_PageSetupMorePaperSizes,
		MenuCmd_PageSetupCustomMargins,
		PageSetupForm_CommentsPrintModeNone,
		PageSetupForm_CommentsPrintModeAtEndOfSheet,
		PageSetupForm_CommentsPrintModeAsDisplayedOnSheet,
		PageSetupForm_ErrorPrintModeDisplayed,
		PageSetupForm_ErrorPrintModeBlank,
		PageSetupForm_ErrorPrintModeDash,
		PageSetupForm_ErrorPrintModeNA,
		Msg_IncorrectNumberRange,
		Msg_InvalidNumber,
		Msg_PageSetupMarginsNotFitPageSize,
		Msg_PageSetupProblemFormula,
		Msg_HeaderFooterTooLongTextString,
		HeaderFooterPredefinedString_None,
		HeaderFooterPredefinedString_Page,
		HeaderFooterPredefinedString_Of,
		HeaderFooterPredefinedString_Confidential,
		HeaderFooterPredefinedString_PreparedBy,
		HeaderFooterFormatTag_PageNumberAnalog,
		HeaderFooterFormatTag_PageTotalAnalog,
		HeaderFooterFormatTag_DateAnalog,
		HeaderFooterFormatTag_TimeAnalog,
		HeaderFooterFormatTag_WorkbookFilePathAnalog,
		HeaderFooterFormatTag_WorkbookFileNameAnalog,
		HeaderFooterFormatTag_WorksheetNameAnalog,
		Msg_SelectionContainsMoreThanOneDataValidation,
		Msg_SelectionContainsCellsWithoutDataValidation,
		Msg_MoreThan255InvalidDataCircles,
		Msg_DataValidationFormulaIsEmpty,
		Msg_DataValidationBothFormulasAreEmpty,
		Msg_DataValidationMinGreaterThanMax,
		Msg_DataValidationInvalidReference,
		Msg_DataValidationDefinedNameNotFound,
		Msg_DataValidationInvalidNonnumericValue,
		Msg_DataValidationInvalidDecimalValue,
		Msg_DataValidationInvalidNegativeValue,
		Msg_DataValidationUnionRangeNotAllowed,
		Msg_DataValidationMoreThanOneCellInRange,
		Msg_DataValidationMustBeRowOrColumnRange,
		Msg_DataValidationInvalidDate,
		Msg_DataValidationInvalidTime,
		Msg_DataValidationFailed,
		Caption_DataValidationMaximum,
		Caption_DataValidationMinimum,
		Caption_DataValidationValue,
		Caption_DataValidationDate,
		Caption_DataValidationStartDate,
		Caption_DataValidationEndDate,
		Caption_DataValidationTime,
		Caption_DataValidationStartTime,
		Caption_DataValidationEndTime,
		Caption_DataValidationLength,
		Caption_DataValidationFormula,
		Caption_DataValidationSource,
		Caption_DataValidationAnyValue,
		Caption_DataValidationWholeNumber,
		Caption_DataValidationDecimal,
		Caption_DataValidationList,
		Caption_DataValidationTextLength,
		Caption_DataValidationCustom,
		Caption_DataValidationStop,
		Caption_DataValidationWarning,
		Caption_DataValidationInformation,
		Caption_DataValidationBetween,
		Caption_DataValidationNotBetween,
		Caption_DataValidationEqual,
		Caption_DataValidationNotEqual,
		Caption_DataValidationGreaterThan,
		Caption_DataValidationLessThan,
		Caption_DataValidationGreaterThanOrEqual,
		Caption_DataValidationLessThanOrEqual,
		MenuCmd_InsertPivotTable,
		MenuCmd_InsertPivotTableDescription,
		MenuCmd_OptionsPivotTable,
		MenuCmd_OptionsPivotTableDescription,
		MenuCmd_OptionsPivotTableContextMenuItem,
		MenuCmd_MovePivotTable,
		MenuCmd_MovePivotTableDescription,
		MenuCmd_ChangeDataSourcePivotTable,
		MenuCmd_ChangeDataSourcePivotTableDescription,
		MenuCmd_SelectFieldTypePivotTable,
		MenuCmd_SelectFieldTypePivotTableDescription,
		MenuCmd_FieldSettingsPivotTableContextMenuItem,
		MenuCmd_DataFieldSettingsPivotTableContextMenuItem,
		MenuCmd_PivotTableExpandField,
		MenuCmd_PivotTableExpandFieldDescription,
		MenuCmd_PivotTableExpandFieldContextMenuItem,
		MenuCmd_PivotTableCollapseField,
		MenuCmd_PivotTableCollapseFieldDescription,
		MenuCmd_PivotTableCollapseFieldContextMenuItem,
		MenuCmd_FieldListPanelPivotTable,
		MenuCmd_FieldListPanelPivotTableDescription,
		MenuCmd_ShowFieldListPanelPivotTableContextMenuItem,
		MenuCmd_HideFieldListPanelPivotTableContextMenuItem,
		MenuCmd_PivotTableRefreshCommandAndCommandGroup,
		MenuCmd_PivotTableRefreshAllCommand,
		MenuCmd_PivotTableRefreshDescription,
		MenuCmd_ShowPivotTableExpandCollapseButton,
		MenuCmd_ShowPivotTableExpandCollapseButtonDescription,
		MenuCmd_ShowPivotTableFieldHeaders,
		MenuCmd_ShowPivotTableFieldHeadersDescription,
		MenuCmd_PivotTableClearCommandGroup,
		MenuCmd_PivotTableClearCommandGroupDescription,
		MenuCmd_ClearAllPivotTable,
		MenuCmd_ClearAllPivotTableDescription,
		MenuCmd_ClearFiltersPivotTable,
		MenuCmd_PivotTableSelectCommandGroup,
		MenuCmd_PivotTableSelectCommandGroupDescription,
		MenuCmd_SelectLabelsAndValuesPivotTable,
		MenuCmd_SelectLabelsAndValuesPivotTableDescription,
		MenuCmd_SelectValuesPivotTable,
		MenuCmd_SelectValuesPivotTableDescription,
		MenuCmd_SelectLabelsPivotTable,
		MenuCmd_SelectLabelsPivotTableDescription,
		MenuCmd_SelectEntirePivotTable,
		MenuCmd_SelectEntirePivotTableDescription,
		MenuCmd_PivotTableSubtotalsCommandGroup,
		MenuCmd_PivotTableSubtotalsCommandGroupDescription,
		MenuCmd_PivotTableGrandTotalsCommandGroup,
		MenuCmd_PivotTableGrandTotalsCommandGroupDescription,
		MenuCmd_PivotTableReportLayoutCommandGroup,
		MenuCmd_PivotTableReportLayoutCommandGroupDescription,
		MenuCmd_PivotTableBlankRowsCommandGroup,
		MenuCmd_PivotTableBlankRowsCommandGroupDescription,
		MenuCmd_PivotTableDoNotShowSubtotals,
		MenuCmd_PivotTableShowAllSubtotalsAtBottom,
		MenuCmd_PivotTableShowAllSubtotalsAtTop,
		MenuCmd_PivotTableGrandTotalsOffRowsColumns,
		MenuCmd_PivotTableGrandTotalsOnRowsColumns,
		MenuCmd_PivotTableGrandTotalsOnRowsOnly,
		MenuCmd_PivotTableGrandTotalsOnColumnsOnly,
		MenuCmd_PivotTableShowCompactForm,
		MenuCmd_PivotTableShowOutlineForm,
		MenuCmd_PivotTableShowTabularForm,
		MenuCmd_PivotTableRepeatAllItemLabels,
		MenuCmd_PivotTableDoNotRepeatItemLabels,
		MenuCmd_PivotTableInsertBlankLine,
		MenuCmd_PivotTableRemoveBlankLine,
		MenuCmd_PivotTableToggleRowHeaders,
		MenuCmd_PivotTableToggleRowHeadersDescription,
		MenuCmd_PivotTableToggleColumnHeaders,
		MenuCmd_PivotTableToggleColumnHeadersDescription,
		MenuCmd_PivotTableToggleBandedRows,
		MenuCmd_PivotTableToggleBandedRowsDescription,
		MenuCmd_PivotTableToggleBandedColumns,
		MenuCmd_PivotTableToggleBandedColumnsDescription,
		MenuCmd_PivotTableFieldsFilters,
		MenuCmd_PivotTableFieldsFiltersDescription,
		MenuCmd_PivotTableLabelFilters,
		MenuCmd_PivotTableLabelFiltersDescription,
		MenuCmd_PivotTableDateFilters,
		MenuCmd_PivotTableDateFiltersDescription,
		MenuCmd_PivotTableValueFilters,
		MenuCmd_PivotTableValueFiltersDescription,
		MenuCmd_PivotTableItemFilter,
		MenuCmd_PivotTableItemFilterDescription,
		MenuCmd_PivotTableSortCommandGroup,
		MenuCmd_PivotTableSortCommandGroupDescription,
		MenuCmd_PivotTableExpandCollapseCommandGroup,
		MenuCmd_PivotTableExpandCollapseCommandGroupDescription,
		OptionsPivotTableForm_PageDownThenOver,
		OptionsPivotTableForm_PageOverThenDown,
		OptionsPivotTableForm_MissingItemsLimitAutomatic,
		OptionsPivotTableForm_MissingItemsLimitNone,
		OptionsPivotTableForm_MissingItemsLimitMax,
		FieldAndDataFieldSettingsPivotTableForm_FunctionDefault,
		FieldAndDataFieldSettingsPivotTableForm_FunctionSum,
		FieldAndDataFieldSettingsPivotTableForm_FunctionCount,
		FieldAndDataFieldSettingsPivotTableForm_FunctionAverage,
		FieldAndDataFieldSettingsPivotTableForm_FunctionMax,
		FieldAndDataFieldSettingsPivotTableForm_FunctionMin,
		FieldAndDataFieldSettingsPivotTableForm_FunctionProduct,
		FieldAndDataFieldSettingsPivotTableForm_FunctionCountNumbers,
		FieldAndDataFieldSettingsPivotTableForm_FunctionStdDev,
		FieldAndDataFieldSettingsPivotTableForm_FunctionStdDevp,
		FieldAndDataFieldSettingsPivotTableForm_FunctionVar,
		FieldAndDataFieldSettingsPivotTableForm_FunctionVarp,
		DataFieldSettingsPivotTableForm_ShowValueAsNoCalculation,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfTotal,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfColumn,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfRow,
		DataFieldSettingsPivotTableForm_ShowValuePercent,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfParentRow,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfParentColumn,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfParent,
		DataFieldSettingsPivotTableForm_ShowValueDifference,
		DataFieldSettingsPivotTableForm_ShowValuePercentDifference,
		DataFieldSettingsPivotTableForm_ShowValueRunningTotal,
		DataFieldSettingsPivotTableForm_ShowValuePercentOfRunningTotal,
		DataFieldSettingsPivotTableForm_ShowValueRankAscending,
		DataFieldSettingsPivotTableForm_ShowValueRankDescending,
		DataFieldSettingsPivotTableForm_ShowValueIndex,
		DataFieldSettingsPivotTableForm_BaseItemPrevious,
		DataFieldSettingsPivotTableForm_BaseItemNext,
		DataFieldSettingsPivotTableForm_PartOfCustomName,
		PivotPageFieldAllItemsCaption,
		PivotPageFieldMultipleItemsCaption,
		DefaultInitialTableColumnNamePrefix,
		Msg_ErrorInvalidTableStyleType,
		Msg_ErrorStyleTypeMustBeTableOrPivot,
		Msg_CannotChangeDefaultOrPredefinedStyleType,
		Msg_CannotChangeAppliedStyleType,
		Msg_PasteAreaNotSameSizeAsSelectionClarification,
		MenuCmd_PivotLabelFilterDoesNotBeginWith,
		MenuCmd_PivotLabelFilterDoesNotBeginWithDescription,
		MenuCmd_PivotLabelFilterDoesNotEndWith,
		MenuCmd_PivotLabelFilterDoesNotEndWithDescription,
		MenuCmd_PivotLabelFilterNotBetween,
		MenuCmd_PivotLabelFilterNotBetweenDescription,
		MenuCmd_PivotClearFieldFilter,
		MenuCmd_PivotClearFieldFilterDescription,
		MenuCmd_SortAscendingOnlyNumbers,
		MenuCmd_SortAscendingOnlyNumbersDescription,
		MenuCmd_SortDescendingOnlyNumbers,
		MenuCmd_SortDescendingOnlyNumbersDescription,
		MenuCmd_SortAscendingOnlyDates,
		MenuCmd_SortAscendingOnlyDatesDescription,
		MenuCmd_SortDescendingOnlyDates,
		MenuCmd_SortDescendingOnlyDatesDescription,
		MenuCmd_RemovePivotField,
		MenuCmd_RemovePivotFieldDescription,
		MenuCmd_PivotTableSummarizeValuesBy,
		MenuCmd_PivotTableMoreOptions,
		MenuCmd_RemoveGrandTotalPivotTable,
		MenuCmd_RemoveGrandTotalPivotTableDescription,
		MenuCmd_MovePivotFieldReference,
		MenuCmd_MovePivotFieldReferenceUp,
		MenuCmd_MovePivotFieldReferenceDown,
		MenuCmd_MovePivotFieldReferenceToBeginning,
		MenuCmd_MovePivotFieldReferenceToEnd,
		MenuCmd_MovePivotFieldReferenceToLeft,
		MenuCmd_MovePivotFieldReferenceToRight,
		MenuCmd_MovePivotFieldReferenceToRows,
		MenuCmd_MovePivotFieldReferenceToColumns,
		MenuCmd_PivotTableShowValuesAs,
		MenuCmd_SubtotalPivotField,
		Caption_GenericFilterOperatorBetween,
		Caption_GenericFilterOperatorNotBetween,
		Caption_Top10FilterTypeSum,
		Caption_ShowValuesAsCalculation,
		Msg_PivotValueFilterNeedAtLeastOneDataField,
		Msg_PivotFilterValueMustBeBetween,
		Msg_PivotFilterNumber,
		Msg_PivotFilterNumbers,
		Msg_PivotFilterInteger,
		Msg_PivotFilterEndNumberMustBeGreaterThanStartNumber,
		Msg_PivotTableCanNotApplyDataValidation,
		Msg_PivotFilterInvalidDate,
		Msg_PivotTableSubtotalListPlace,
		Msg_PivotTableFieldMoveRestricted,
	}
	#endregion
	#region XtraSpreadsheetLocalizer
	public class XtraSpreadsheetLocalizer : XtraLocalizer<XtraSpreadsheetStringId> {
		static XtraSpreadsheetLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			#region Checked texts
			AddString(XtraSpreadsheetStringId.Msg_ErrorInternalError, "An internal error occurred");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedWorksheet, "Worksheet is already deleted.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseInvalidWorkbook, "This workbook is no longer valid.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDuplicateSheetName, "Cannot rename a sheet to the same name as existing sheet.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorBlankSheetName, "The worksheet name must not be an empty string.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSheetNameContainsNotAllowedCharacters, "The worksheet name must not contain the following symbols: \\ / ? : * [ or ]");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSheetNameStartOrEndWithSingleQuote, "The worksheet name must not start and end with a single quote (').");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSheetNameExceedAllowedLength, "The worksheet name must not exceed 31 characters.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedDefinedName, "Defined name is already deleted.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedConditionalFormatting, "Conditional formatting  is already deleted.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCommentAlreadyExists, "Comment with the same reference already exists.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidDefinedName, "The text is not a valid defined name.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidDefinedName_FormulaBar, "You must enter a valid reference you want to go to, or type a valid name for the selection");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists, "Duplicate defined names are not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedStyle, "Style is already deleted.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorStyleNotFound, "This style name does not exist.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDuplicateStyleName, "Style with the same name already exists.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDeletingBuiltInTableStyle, "Cannot delete built-in table style.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameReferenceWithoutSheet, "Defined name reference should contain worksheet definition for all cell references.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, "Row index should be non-negative and less than the number of rows in a worksheet (1048576).");
			AddString(XtraSpreadsheetStringId.Msg_ErrorNegativeIndexNotAllowed, "Index cannot be negative.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeight, "Row height should be between 0 and 409.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnWidth, "Column width should be between 0 and 255 characters.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectIndentValue, "Indent should be between 0 and 250.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveTableHeader, "Cannot delete a row containing a table header.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound, "Worksheet is not found in a workbook.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet, "Cannot use a range from another worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorkbook, "Cannot use a range from another workbook.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseShapeFromAnotherWorksheet, "Cannot use a shape from another worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseTableStyleFromAnotherWorkbook, "Cannot use a table style from another workbook.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, "Column index should be non-negative and less than the number of columns in a worksheet (16384).");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpression, "The text is not a valid reference.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableColumnAlreadyExists, "Table column with the same name already exists.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorWorksheetIndexOutside, "Worksheet index should be positive and less than the number of worksheets."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorFormula, "Formula is incorrect. Formula: \"{0}\", Culture: {1}.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCanNotCreateTable, "Cannot create a table in the specified position.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableAlreadyExists, "The name you entered already exists. Enter a unique name."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableNameIsNotValid, "The name you entered is not valid."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableRowIndexOutside, "Table row index should be positive and should not exceed the number of rows in a table.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableColumnIndexOutside, "Table column index should be positive and should not exceed the number of columns in a table.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSharedFormulaIndexOutside, "Shared formula with this index does not exist.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInsertAboveHeaderRow, "Cannot insert a table row above the header row.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInsertBelowTotalRow, "Cannot insert a table row below the total row.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDeleteHeaderRow, "Cannot delete the header row.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorStayOnlyHeaders, "Table cannot contain only the header row.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedHyperlink, "Hyperlink is already deleted.");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_OpenXmlFiles, "Excel Workbook");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_OpenDocumentFiles, "OpenDocument Spreadsheet");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_DocFiles, "Excel 97-2003 Workbook");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_CSVFiles, "CSV (Comma delimited)");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_TextFiles, "Text (Tab delimited)");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_HtmlFiles, "HyperText Markup Language Format");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_XlsmFiles, "Excel Macro-Enabled Workbook");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_XltFiles, "Excel 97-2003 Template");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_XltxFiles, "Excel Template");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_XltmFiles, "Excel Macro-Enabled Template");
			AddString(XtraSpreadsheetStringId.FileFilterDescription_AllFiles, "All Files");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseInvalidStyle, "Invalid style object.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseStyleFromAnotherWorkbook, "Cannot use a style object from another workbook.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseInvalidRangeFormatObject, "Invalid range format object.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSetInsideBordersToCell, "Cannot set inside borders to a single cell.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray, "Changing part of an array is not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCannotChangingPartOfAnArray, "You cannot change part of an array.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorMuiltiCellArrayFormulaInTable, "Multi-cell array formulas are not allowed in tables.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorZeroCountRanges, "The number of ranges should be greater than zero.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorZeroCountSheets, "The number of worksheets should be greater than zero.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpressionInDefinedName, "To use defined name in the CreateRange method, the DefinedName.RefersTo property should be set to a cell range reference.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAMergedCell, "Changing part of a merged cell is not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCannotChangingMergedCell, "This operation cannot be performed over merged cells.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorRelativeReference, "Absolute range reference is required.");
			AddString(XtraSpreadsheetStringId.Msg_IncompliantArrayParsedFormula, "Cell {0} on sheet \"{1}\" has a non-compliant array parsed formula. Replaced by error #N/A.");
			AddString(XtraSpreadsheetStringId.Msg_IncompliantCellParsedFormula, "Cell {0} on sheet \"{1}\" has a non-compliant cell parsed formula. Replaced by error #N/A.");
			AddString(XtraSpreadsheetStringId.Msg_IncompliantNamedParsedFormula, "Defined name \"{0}\"({1}): parsed formula is non-compliant. Replaced by error #N/A.");
			AddString(XtraSpreadsheetStringId.Msg_DefinedNameHasInvalidScope, "Defined name \"{0}\" has invalid scope. Ignored.");
			AddString(XtraSpreadsheetStringId.Msg_DefinedNameHasInvalidExpression, "Defined name \"{0}\"({1}): parsed expression is not valid. Replaced by error #REF.");
			AddString(XtraSpreadsheetStringId.Msg_DefinedNameHasBeenChanged, "Defined name \"{0}\"({1}): name has been changed to \"{2}\".");
			AddString(XtraSpreadsheetStringId.Msg_DefinedNameInvalidName, "The name you entered is not valid.\r\n\r\nReasons for this can include:\r\n\r\n    - The name does not begin with a letter or underscore\r\n    - The name contains a space or other invalid characters\r\n    - The name conflicts with a built-in name or the name of another object in the workbook.");
			AddString(XtraSpreadsheetStringId.Msg_ProtectedRangeEmptyName, "You must enter a title for the range.");
			AddString(XtraSpreadsheetStringId.Msg_ProtectedRangeInvalidName, "The title '{0}' contains illegal characters. Range titles may only contain letters, numbers, spaces, and must begin with a letter.");
			AddString(XtraSpreadsheetStringId.Msg_ProtectedRangeDuplicateName, "A range with the title '{0}' already exists. Please enter a new title for the range.");
			AddString(XtraSpreadsheetStringId.Msg_NumberFormatRecordsSkipped, "Number formats: the number of records exceeded the maximum value of ({0}). Some records are not written.");
			AddString(XtraSpreadsheetStringId.Msg_RowFirstColumnOutOfXLSRange, "All non-empty cells of row {0} are beyond the limits of XLS cell range. Row has been skipped.");
			AddString(XtraSpreadsheetStringId.Msg_RowLastColumnOutOfXLSRange, "Some non-empty cells of row {0} are beyond the limits of XLS cell range. Row has been truncated.");
			AddString(XtraSpreadsheetStringId.Msg_RefToError, "formula has reference(s) exceeding the limits of XLS range. Replaced by error #REF!.");
			AddString(XtraSpreadsheetStringId.Msg_RefTruncated, "formula has reference(s) exceeding the limits of XLS range. Truncated.");
			AddString(XtraSpreadsheetStringId.Msg_TableRefToRef, "formula has table reference(s). Replaced by cell reference.");
			AddString(XtraSpreadsheetStringId.Msg_TableRefToError, "formula has table reference(s). Replaced by error #REF!.");
			AddString(XtraSpreadsheetStringId.Msg_CellOnSheet, "Cell {0} on sheet \"{1}\"");
			AddString(XtraSpreadsheetStringId.Msg_ArrayFormulaRange, "array formula range");
			AddString(XtraSpreadsheetStringId.Msg_SharedFormulaRange, "shared formula range");
			AddString(XtraSpreadsheetStringId.Msg_ElfToRef, "natural language formula has been replaced by cell reference.");
			AddString(XtraSpreadsheetStringId.Msg_ElfLelToError, "deleted natural language formula reference has been replaced by error #NAME!.");
			AddString(XtraSpreadsheetStringId.Msg_StyleNameHasBeenChanged, "Custom style \"{0}\": name has been changed to \"{1}\".");
			AddString(XtraSpreadsheetStringId.Msg_XFTableHasBeenTruncated, "Possible loss of formatting: the number of formats exceeds the maximum value. Extended format table has been truncated.");
			AddString(XtraSpreadsheetStringId.Msg_DefaultStyleXFUsed, "Possible loss of formatting: default style format is used instead of original formatting.");
			AddString(XtraSpreadsheetStringId.Msg_DefaultCellXFUsed, "Possible loss of formatting: default cell format is used instead of original formatting.");
			AddString(XtraSpreadsheetStringId.Msg_FuncVarExceedMaxArgCount, "formula with more than 30 arguments per function has been converted to #VALUE! error.");
			AddString(XtraSpreadsheetStringId.Msg_FuncExceedMaxNestingLevel, "formula with more than 7 nested function levels has been converted to #VALUE! error.");
			AddString(XtraSpreadsheetStringId.Msg_FormulaExceedMaxSize, "formula exceeds the maximum character length or byte size. Converted to #VALUE! error.");
			AddString(XtraSpreadsheetStringId.Msg_TableColumnNameChanged, "Column name \"{0}\" in a table \"{1}\" is not unique. Changed to \"{2}\".");
			AddString(XtraSpreadsheetStringId.Msg_InvalidHyperlinkRemoved, "Hyperlink with invalid target Uri \"{0}\" has been removed.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidFormulaRemoved, "Cell {0} on the sheet \"{1}\" has an invalid formula. Formula has been removed.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidFormula, "The formula you typed contains an error.\r\n\r\nIf you are not trying to enter a formula, avoid using an equal sign (=) or minus sign (-), or precede it with a single quotation mark (').");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidReference, "Reference is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidFilePath, "Cannot open the specified file.");
			AddString(XtraSpreadsheetStringId.Msg_CircularReference, "Circular Reference Warning\r\n\r\nOne or more formulas contain a circular reference and may not calculate correctly. Circular references are any references within a formula that depend upon the results of that same formula.\r\nFor example, a cell that refers to its own value or a cell that refers to another cell which depends on the original cell's value both contain circular references.");
			AddString(XtraSpreadsheetStringId.Msg_CustomFunctionAlreadyDefined, "Function with same name is already defined.");
			AddString(XtraSpreadsheetStringId.Msg_CustomFunctionInvalidName, "The function name is not valid. It should start with a letter or an underscore, and should not contain the following symbols: \\ / ? : * [ or ].");
			AddString(XtraSpreadsheetStringId.Msg_CustomFunctionInvalidParameters, "The specified function parameters are invalid. Maximum number of parameters is 255. Required parameters should precede optional parameters. Unlimited parameter should be the final parameter. Array should not contain empty values.");
			AddString(XtraSpreadsheetStringId.Msg_ExternalLinkInvalidPath, "External link path is invalid.");
			AddString(XtraSpreadsheetStringId.Msg_ExternalLinkAlreadyExists, "External link with the same name already exists.");
			AddString(XtraSpreadsheetStringId.Msg_ExternalWorkbookWithoutSheets, "External link should contain at least one worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidStyleName, "Style name is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_ColumnIndexOutOfRange, "Column index is out of range.");
			AddString(XtraSpreadsheetStringId.Msg_RowIndexOutOfRange, "Row index is out of range.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidSheetSpecified, "Specified sheet is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_CanNotShiftNonBlankCellsOffOfTheSheet, "To prevent possible loss of data, Spreasheet cannot shift nonblank cells off of the worksheet. Select another location in which to insert new cells, or delete data from the end of your worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_DeleteSheetConfirmation, "You can't undo deleting sheets, and you might be removing some data. If you don't need it, click Delete.");
			AddString(XtraSpreadsheetStringId.Caption_InvalidExpression, "Invalid");
			AddString(XtraSpreadsheetStringId.Caption_PermissionEditRangeWithoutPassword, "Edit range without a password");
			AddString(XtraSpreadsheetStringId.Caption_PageFile, "File");
			AddString(XtraSpreadsheetStringId.Caption_GroupCommon, "Common");
			AddString(XtraSpreadsheetStringId.Caption_GroupInfo, "Info");
			AddString(XtraSpreadsheetStringId.Caption_PageHome, "Home");
			AddString(XtraSpreadsheetStringId.Caption_GroupFont, "Font");
			AddString(XtraSpreadsheetStringId.Caption_GroupClipboard, "Clipboard");
			AddString(XtraSpreadsheetStringId.Caption_GroupAlignment, "Alignment");
			AddString(XtraSpreadsheetStringId.Caption_GroupNumber, "Number");
			AddString(XtraSpreadsheetStringId.Caption_GroupStyles, "Styles");
			AddString(XtraSpreadsheetStringId.Caption_GroupCells, "Cells");
			AddString(XtraSpreadsheetStringId.Caption_GroupEditing, "Editing");
			AddString(XtraSpreadsheetStringId.Caption_PageMailMerge, "Mail Merge");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeData, "External Data Sources");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeMode, "Mode");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeExtended, "Template Ranges");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeGrouping, "Sort && Group");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeFiltering, "Filter");
			AddString(XtraSpreadsheetStringId.Caption_GroupMailMergeBinding, "Design");
			AddString(XtraSpreadsheetStringId.Caption_MailMergeFilterExpressionEditor, "Filter Expression");
			AddString(XtraSpreadsheetStringId.Caption_PageView, "View");
			AddString(XtraSpreadsheetStringId.Caption_GroupShow, "Show");
			AddString(XtraSpreadsheetStringId.Caption_GroupZoom, "Zoom");
			AddString(XtraSpreadsheetStringId.Caption_GroupWindow, "Window");
			AddString(XtraSpreadsheetStringId.Caption_PageReview, "Review");
			AddString(XtraSpreadsheetStringId.Caption_GroupComments, "Comments");
			AddString(XtraSpreadsheetStringId.Caption_GroupChanges, "Changes");
			AddString(XtraSpreadsheetStringId.Caption_PageCategoryChartTools, "Chart Tools");
			AddString(XtraSpreadsheetStringId.Caption_PageChartsDesign, "Design");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsDesignType, "Type");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsDesignData, "Data");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsDesignLayouts, "Chart Layouts");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsDesignStyles, "Chart Styles");
			AddString(XtraSpreadsheetStringId.Caption_PageChartsLayout, "Layout");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutLabels, "Labels");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAxes, "Axes");
			AddString(XtraSpreadsheetStringId.Caption_GroupChartsLayoutAnalysis, "Analysis");
			AddString(XtraSpreadsheetStringId.Caption_PageFormat, "Format");
			AddString(XtraSpreadsheetStringId.Caption_PageCategoryPictureTools, "Picture Tools");
			AddString(XtraSpreadsheetStringId.Caption_PageCategoryDrawingTools, "Drawing Tools");
			AddString(XtraSpreadsheetStringId.Caption_PageInsert, "Insert");
			AddString(XtraSpreadsheetStringId.Caption_GroupIllustrations, "Illustrations");
			AddString(XtraSpreadsheetStringId.Caption_GroupCharts, "Charts");
			AddString(XtraSpreadsheetStringId.Caption_GroupLinks, "Links");
			AddString(XtraSpreadsheetStringId.Caption_GroupSymbols, "Symbols");
			AddString(XtraSpreadsheetStringId.Caption_GroupTables, "Tables");
			AddString(XtraSpreadsheetStringId.Caption_PagePageLayout, "Page Layout");
			AddString(XtraSpreadsheetStringId.Caption_GroupPageSetup, "Page Setup");
			AddString(XtraSpreadsheetStringId.Caption_GroupArrange, "Arrange");
			AddString(XtraSpreadsheetStringId.Caption_GroupPrint, "Print");
			AddString(XtraSpreadsheetStringId.Caption_PageFormulas, "Formulas");
			AddString(XtraSpreadsheetStringId.Caption_GroupFunctionLibrary, "Function Library");
			AddString(XtraSpreadsheetStringId.Caption_GroupFormulaDefinedNames, "Defined Names");
			AddString(XtraSpreadsheetStringId.Caption_GroupFormulaAuditing, "Formula Auditing");
			AddString(XtraSpreadsheetStringId.Caption_GroupFormulaCalculation, "Calculation");
			AddString(XtraSpreadsheetStringId.Caption_PageData, "Data");
			AddString(XtraSpreadsheetStringId.Caption_GroupSortAndFilter, "Sort & Filter");
			AddString(XtraSpreadsheetStringId.Caption_ColorAutomatic, "Automatic");
			AddString(XtraSpreadsheetStringId.Caption_NoFill, "No Fill");
			AddString(XtraSpreadsheetStringId.Caption_NoColor, "No Color");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupCustom, "Custom");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupGoogBadAndNeutral, "Good, Bad and Neutral");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupDataAndModel, "Data and Model");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupTitlesAndHeadings, "Titles and Headings");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupThemedCellStyles, "Themed Cell Styles");
			AddString(XtraSpreadsheetStringId.Caption_StyleGalleryGroupNumberFormat, "Number Format");
			AddString(XtraSpreadsheetStringId.Caption_PageCategoryTableTools, "Table Tools");
			AddString(XtraSpreadsheetStringId.Caption_TableToolsDesignPage, "Design");
			AddString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions, "Table Style Options");
			AddString(XtraSpreadsheetStringId.Caption_GroupTableProperties, "Properties");
			AddString(XtraSpreadsheetStringId.Caption_LightTableStyleNamePart, "Light");
			AddString(XtraSpreadsheetStringId.Caption_MediumTableStyleNamePart, "Medium");
			AddString(XtraSpreadsheetStringId.Caption_DarkTableStyleNamePart, "Dark");
			AddString(XtraSpreadsheetStringId.Caption_TableStyleNameIsNone, "None");
			AddString(XtraSpreadsheetStringId.Caption_PrefixTableStyleNamePart, "Table Style");
			AddString(XtraSpreadsheetStringId.Caption_CustomTableStyleCategory, "Custom");
			AddString(XtraSpreadsheetStringId.Caption_LightTableStyleCategory, "Light");
			AddString(XtraSpreadsheetStringId.Caption_MediumTableStyleCategory, "Medium");
			AddString(XtraSpreadsheetStringId.Caption_DarkTableStyleCategory, "Dark");
			AddString(XtraSpreadsheetStringId.Caption_PageCategoryPivotTableTools, "PivotTable Tools");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyze, "Analyze");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzePivotTable, "PivotTable");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActiveField, "Active Field");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeData, "Data");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActions, "Actions");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeShow, "Show");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableDesign, "Design");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableDesignLayout, "Layout");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyleOptions, "PivotTable Style Options");
			AddString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyles, "PivotTable Styles");
			AddString(XtraSpreadsheetStringId.Caption_PrefixPivotStyleNamePart, "Pivot Style");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTop10Rule_FormText, "Top 10 Items");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottom10Rule_FormText, "Bottom 10 Items");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTop10PercentRule_FormText, "Top 10%");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottom10PercentRule_FormText, "Bottom 10%");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_FormText, "Above Average");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_FormText, "Below Average");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingGreaterThanRule_FormText, "Greater Than");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingLessThanRule_FormText, "Less Than");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingEqualToRule_FormText, "Equal To");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTextContainsRule_FormText, "Text That Contains");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_FormText, "Duplicate Values");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_FormText, "A Date Occurring");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBetweenRule_FormText, "Between");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopRule_LabelHeaderText, "Format cells that rank in the TOP:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBottomRule_LabelHeaderText, "Format cells that rank in the BOTTOM:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_LabelHeaderText, "Format cells that are ABOVE AVERAGE:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingAboveAverageRule_LabelWithText, "for the selected range with");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_LabelHeaderText, "Format cells that are BELOW AVERAGE:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBelowAverageRule_LabelWithText, "for the selected range with");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingGreaterThanRule_LabelHeaderText, "Format cells that are GREATER THAN:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingLessThanRule_LabelHeaderText, "Format cells that are LESS THAN:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingEqualToRule_LabelHeaderText, "Format cells that are EQUAL TO:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTextContainsRule_LabelHeaderText, "Format cells that contain the text:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_LabelHeaderText, "Format cells that contain:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_LabelHeaderText, "Format cells that contain a date occurring:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingBetweenRule_LabelHeaderText, "Format cells that are BETWEEN:");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomRule_LabelWithText, "with");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingTopBottomPercentRule_LabelWithText, "% with");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDuplicateValuesRule_LabelWithText, "values with");
			AddString(XtraSpreadsheetStringId.Caption_ConditionalFormattingDateOccurringRule_LabelWithText, "with");
			AddString(XtraSpreadsheetStringId.Caption_NewDefinedNameFormTitle, "New Name");
			AddString(XtraSpreadsheetStringId.Caption_EditDefinedNameFormTitle, "Edit Name");
			AddString(XtraSpreadsheetStringId.Caption_NewProtectedRangeFormTitle, "New Range");
			AddString(XtraSpreadsheetStringId.Caption_EditProtectedRangeFormTitle, "Modify Range");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToCreateArrayFormulaInMergedCells, "Creating array formulas in merged cells is not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToCreateSharedFormulaInMergedCells, "This operation is not allowed in merged cells.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveArrayFormula, "Range does not contain array formulas.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorectCount, "Count should be greater than 0.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorectHighBound, "High bound is incorrect.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorectLowBound, "Low bound is incorrect.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorectMidPoint, "Midpoint is incorrect.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDeleteSingularWorksheet, "Workbook should contain at least one visible worksheet. To hide, delete, or move the sheet, insert a new sheet or unhide a hidden sheet.");
			AddString(XtraSpreadsheetStringId.Caption_PermissionSelectLockedCells, "Select locked cells");
			AddString(XtraSpreadsheetStringId.Caption_PermissionSelectUnlockedCells, "Select unlocked cells");
			AddString(XtraSpreadsheetStringId.Caption_PermissionFormatCells, "Format cells");
			AddString(XtraSpreadsheetStringId.Caption_PermissionFormatColumns, "Format columns");
			AddString(XtraSpreadsheetStringId.Caption_PermissionFormatRows, "Format rows");
			AddString(XtraSpreadsheetStringId.Caption_PermissionInsertColumns, "Insert columns");
			AddString(XtraSpreadsheetStringId.Caption_PermissionInsertRows, "Insert rows");
			AddString(XtraSpreadsheetStringId.Caption_PermissionInsertHyperlinks, "Insert hyperlinks");
			AddString(XtraSpreadsheetStringId.Caption_PermissionDeleteColumns, "Delete columns");
			AddString(XtraSpreadsheetStringId.Caption_PermissionDeleteRows, "Delete rows");
			AddString(XtraSpreadsheetStringId.Caption_PermissionSort, "Sort");
			AddString(XtraSpreadsheetStringId.Caption_PermissionAutoFilter, "Use AutoFilter");
			AddString(XtraSpreadsheetStringId.Caption_PermissionPivotTable, "Use PivotTable reports");
			AddString(XtraSpreadsheetStringId.Caption_PermissionEditObjects, "Edit objects");
			AddString(XtraSpreadsheetStringId.Caption_PermissionEditScenarios, "Edit scenarios");
			AddString(XtraSpreadsheetStringId.Caption_SearchByRows, "By Rows");
			AddString(XtraSpreadsheetStringId.Caption_SearchByColumns, "By Columns");
			AddString(XtraSpreadsheetStringId.Caption_SearchInFormulas, "Formulas");
			AddString(XtraSpreadsheetStringId.Caption_SearchInValues, "Values");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNone, " ");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals, "equals");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual, "does not equal");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreater, "is greater than");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreaterOrEqual, "is greater than or equal to");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLess, "is less than");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLessOrEqual, "is less than or equal to");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfter, "is after");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfterOrEqual, "is after or equal to");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBefore, "is before");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeforeOrEqual, "is before or equal to");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeginsWith, "begins with");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotBeginWith, "does not begin with");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEndsWith, "ends with");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEndWith, "does not end with");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorContains, "contains");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotContain, "does not contain");
			AddString(XtraSpreadsheetStringId.Caption_Top10FilterTypeItems, "Items");
			AddString(XtraSpreadsheetStringId.Caption_Top10FilterTypePercent, "Percent");
			AddString(XtraSpreadsheetStringId.Caption_Top10FilterOrderTop, "Top");
			AddString(XtraSpreadsheetStringId.Caption_Top10FilterOrderBottom, "Bottom");
			AddString(XtraSpreadsheetStringId.Caption_Bytes, "bytes");
			AddString(XtraSpreadsheetStringId.MenuCmd_None, "");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowDocumentProperties, "Document Properties");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowDocumentPropertiesDescription, "Show the Properties dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatFillColor, "Fill Color");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatFillColorDescription, "Color the background of selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentTop, "Top Align");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentTopDescription, "Align text to the top of the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentMiddle, "Middle Align");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentMiddleDescription, "Align text so that it is centered between the top and bottom of the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentBottom, "Bottom Align");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAlignmentBottomDescription, "Align text to the bottom of the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatWrapText, "Wrap Text");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatWrapTextDescription, "Make all content visible within a cell by displaying it on multiple lines.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatIncreaseIndent, "Increase Indent");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatIncreaseIndentDescription, "Increase the margin between the border and the text in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatDecreaseIndent, "Decrease Indent");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatDecreaseIndentDescription, "Decrease the margin between the border and the text in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBorderColor, "Line Color");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBorderColorDescription, "Change the border line color.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBorderLineStyle, "Line Style");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBorderLineStyleDescription, "Change the style of the line used to draw borders.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatThickBorder, "Thick Box Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatThickBorderDescription, "Thick Box Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatOutsideBorders, "Outside Borders");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatOutsideBordersDescription, "Outside Borders.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatLeftBorder, "Left Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatLeftBorderDescription, "Left Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRightBorder, "Right Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRightBorderDescription, "Right Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopBorder, "Top Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopBorderDescription, "Top Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomBorder, "Bottom Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomBorderDescription, "Bottom Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomDoubleBorder, "Bottom Double Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomDoubleBorderDescription, "Bottom Double Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomThickBorder, "Thick Bottom Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBottomThickBorderDescription, "Thick Bottom Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndBottomBorder, "Top and Bottom Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndBottomBorderDescription, "Top and Bottom Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndThickBottomBorder, "Top and Thick Bottom Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndThickBottomBorderDescription, "Top and Thick Bottom Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndDoubleBottomBorder, "Top and Double Bottom Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTopAndDoubleBottomBorderDescription, "Top and Double Bottom Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAllBorders, "All Borders");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAllBordersDescription, "All Borders.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNoBorders, "No Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNoBordersDescription, "No Border.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBordersCommandGroup, "Borders");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatBordersCommandGroupDescription, "Borders.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ToggleCellLocked, "Lock Cell");
			AddString(XtraSpreadsheetStringId.MenuCmd_ToggleCellLockedDescription, "Lock the selected cells to keep people from making changes to them.\r\n\r\nYou must turn on Protect Sheet in order for this feature to work.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumber, "Number Format");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDescription, "Choose how the values in a cell are displayed: as a percentage, as currency, as a date or time, etc.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberGeneral, "General");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberGeneralDescription, "No specific format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecimal, "Number");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecimalDescription, "Display the value of the cell as a decimal number.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercent, "Percent Style");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercentDescription, "Display the value of the cell as a percentage.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercentage, "Percentage");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercentageDescription, "Display the value of the cell as a percentage.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberScientific, "Scientific");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberScientificDescription, "Display the value of the cell in scientific format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberFraction, "Fraction");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberFractionDescription, "Display the value of the cell as a fraction.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccounting, "Accounting");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingDescription, "Display the value of the cell as an accounting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCurrency, "Currency");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCurrencyDescription, "Display the value of the cell in currency format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberShortDate, "Short Date");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberShortDateDescription, "Display the value of the cell in short date format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberLongDate, "Long Date");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberLongDateDescription, "Display the value of the cell in long date format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberText, "Text");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberTextDescription, "Display the value of the cell as text.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberTime, "Time");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberTimeDescription, "Display the value of the cell in time format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingUS, "$ English (U.S.)");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingUSDescription, "This will change the format of the cell to Accounting with a '$' as currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingUK, "£ English (U.K.)");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingUKDescription, "This will change the format of the cell to Accounting with a '£' as currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingPRC, "¥ Chinese (PRC)");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingPRCDescription, "This will change the format of the cell to Accounting with a '¥' as currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingEuro, "€ Euro");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingEuroDescription, "This will change the format of the cell to Accounting with a '€' as currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingSwiss, "fr. French (Switzerland)");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingSwissDescription, "This will change the format of the cell to Accounting with a 'fr.' as currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCommandGroup, "Accounting Number Format");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCommandGroupDescription, "Choose an alternate currency format for the selected cell.\r\n\r\nFor instance, choose Euros instead of Dollars.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberIncreaseDecimal, "Increase Decimal");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberIncreaseDecimalDescription, "Show more precise values by showing more decimal places.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecreaseDecimal, "Decrease Decimal");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecreaseDecimalDescription, "Show less precise values by showing fewer decimal places.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberCommaStyle, "Comma Style");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatNumberCommaStyleDescription, "Display the value of the cell with a thousands separator.\r\n\r\nThis will change the format of the cell to Accounting without a currency symbol.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellStyle, "Cell Styles");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellStyleDescription, "Quickly format a cell by choosing from pre-defined styles.\r\n\r\nYou can also define your own cell styles.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAsTable, "Format As Table");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAsTableDescription, "Quickly convert a range of cells to a table with it's own style.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSortAndFilterCommandGroup, "Sort && Filter");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSortAndFilterCommandGroupDescription, "Arrange data so that it is easier to analyze.\r\n\r\nYou can sort the selected data in ascending or descending order, or you can temporarily filter out specific values.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFindAndSelectCommandGroup, "Find && Select");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFindAndSelectCommandGroupDescription, "Find and select specific text, formatting, or type of information within the document.\r\n\r\nYou can also replace the information with new text or formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFind, "Find");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFindDescription, "Find and select specific text, formatting, or type of information within the document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingReplace, "Replace");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingReplaceDescription, "Find and select specific text, formatting, or type of information within the document.\r\n\r\nYou can also replace the information with new text or formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectFormulas, "Formulas");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectFormulasDescription, "Go To Formulas.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectComments, "Comments");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectCommentsDescription, "Go To Comments.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectConditionalFormatting, "Conditional Formatting");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectConditionalFormattingDescription, "Go To Conditional Formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectConstants, "Constants");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectConstantsDescription, "Go To Constants.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectDataValidation, "Data Validation");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingSelectDataValidationDescription, "Go To Data Validation.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DefineNameCommandGroup, "Define Name");
			AddString(XtraSpreadsheetStringId.MenuCmd_DefineNameCommandGroupDescription, "Name cells so that you can refer to them in formulas by that name.\r\n\r\nFor example, you might name the cells A20 to A40 \"Expenses\".\r\n\r\nNames can be used in formulas to make them easier to understand.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DefineNameCommand, "Define Name");
			AddString(XtraSpreadsheetStringId.MenuCmd_DefineNameCommandDescription, "Name cells so that you can refer to them in formulas by that name.\r\n\r\nFor example, you might name the cells A20 to A40 \"Expenses.\"\r\n\r\nNames can be used in formulas to make them easier to understand.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowNameManager, "Name Manager");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowNameManagerDescription, "Create, edit, delete, and find all the names used in the workbook.\r\n\r\nNames can be used in formulas as substitutes for cell references.\r\n\r\nFor example: =SUM(Sales) instead of =SUM(C20:C30).");
			AddString(XtraSpreadsheetStringId.MenuCmd_CreateDefinedNamesFromSelection, "Create from Selection");
			AddString(XtraSpreadsheetStringId.MenuCmd_CreateDefinedNamesFromSelectionDescription, "Automatically generate names from the selected cells.\r\n\r\nMany people choose to use the text in the top row or leftmost column of a selection.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertDefinedName, "Use in Formula");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertDefinedNameDescription, "Choose a name used in this workbook and insert it into the current formula.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertDefinedNameCommandGroup, "Use in Formula");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertDefinedNameCommandGroupDescription, "Choose a name used in this workbook and insert it into the current formula.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculateNow, "Calculate Now");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculateNowDescription, "Calculate entire workbook now.\r\n\r\nThis is only necessary if automatic calculation has been turned off.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculateSheet, "Calculate Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculateSheetDescription, "Calculate the current sheet now.\r\n\r\nThis is only necessary if automatic calculation has been turned off.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationOptionsCommandGroup, "Calculation Options");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationOptionsCommandGroupDescription, "Specify when formulas are calculated.\r\n\r\nBy default, any time you change a value that affects other values, the new values are calculated immediately.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationModeAutomatic, "Automatic");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationModeAutomaticDescription, "Automatic.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationModeManual, "Manual");
			AddString(XtraSpreadsheetStringId.MenuCmd_CalculationModeManualDescription, "Manual.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCellsCommandGroup, "Merge Cells");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCellsCommandGroupDescription, "Click here to merge across several cells, or to split cells that have been merged back into individual cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeAndCenterCells, "Merge && Center");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeAndCenterCellsDescription, "Joins the selected cells into one larger cell and centers the contents of the new cell.\r\n\r\nThis is often used to create labels that span multiple columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCellsAcross, "Merge Across");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCellsAcrossDescription, "Merge each row of the selected cells into a larger cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCells, "Merge Cells");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMergeCellsDescription, "Merge the selected cells into one cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingUnmergeCells, "Unmerge Cells");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingUnmergeCellsDescription, "Split the selected cells into multiple new cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillCommandGroup, "Fill");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillCommandGroupDescription, "Continue a pattern into one or more adjacent cells.\r\n\r\nYou can fill cells in any direction and into any rage of adjacent cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillDown, "Fill Down");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillDownDescription, "Continue a pattern into one or more adjacent cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillUp, "Fill Up");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillUpDescription, "Continue a pattern into one or more adjacent cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillLeft, "Fill Left");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillLeftDescription, "Continue a pattern into one or more adjacent cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillRight, "Fill Right");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingFillRightDescription, "Continue a pattern into one or more adjacent cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearCommandGroup, "Clear");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearCommandGroupDescription, "Delete everything from the cell, or selectively remove the formatting, the contents, or the comments.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearAll, "Clear All");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearAllDescription, "Clear everything from the selected cells.\r\n\r\nAll contents, formatting, and comments are cleared from the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearFormats, "Clear Formats");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearFormatsDescription, "Clear only the formatting that is applied to the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearContents, "Clear Contents");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearContentsDescription, "Clear only the contents in the selected cells.\r\n\r\nThe formatting and comments are not cleared.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearComments, "Clear Comments");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearCommentsDescription, "Clear any comments that are attached to the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearHyperlinks, "Clear Hyperlinks");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatClearHyperlinksDescription, "Clear the hyperlinks from the selected cells.\r\n\r\nThe formatting is not cleared.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRemoveHyperlinks, "Remove Hyperlinks");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRemoveHyperlinksDescription, "Remove the hyperlinks and formatting from the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowGridlines, "Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowGridlinesDescription, "Show the lines between rows and columns in the sheet to make editing and reading easier.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowHeadings, "Headings");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowHeadingsDescription, "Show row and column headings.\r\n\r\nRow headings are the row numbers to the side of the sheet.\r\n\r\nColumn headings are the letters or numbers that appear above the columns on a sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowFormulas, "Show Formulas");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewShowFormulasDescription, "Display the formula in each cell instead of the resulting value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezePanesCommandGroup, "Freeze Panes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezePanesCommandGroupDescription, "Keep a portion of the sheet visible while the rest of the sheet scrolls.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezePanes, "Freeze Panes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezePanesDescription, "Keep rows and columns visible while the rest of the worksheet scrolls (based on current selection).");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewUnfreezePanes, "Unfreeze Panes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewUnfreezePanesDescription, "Unlock all rows and columns to scroll through the entire worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezeTopRow, "Freeze Top Row");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezeTopRowDescription, "Keep the top row visible while scrolling through the rest of the worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezeFirstColumn, "Freeze First Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_ViewFreezeFirstColumnDescription, "Keep the first column visible while scrolling through the rest of the worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheet, "Insert Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetDescription, "Insert Worksheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetContextMenuItem, "Insert");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetContextMenuItemDescription, "Insert Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetRows, "Insert Sheet Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsDescription, "Add new rows to worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsContextMenuItem, "Insert");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsContextMenuItemDescription, "Add new rows to worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetColumns, "Insert Sheet Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsDescription, "Add new columns to worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsContextMenuItem, "Insert");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsContextMenuItemDescription, "Add new columns to worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowInsertSheetCellsForm, "Insert Cells...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowInsertSheetCellsFormDescription, "Add new rows, columns, cells, or sheets to workbook.\r\n\r\nNew rows will be added above the selection and new columns will be added to the left of the selection.\r\n\r\nAdd three columns at once by selecting three existing columns first; this also works for multiple rows.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertCellsCommandGroup, "Insert");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertCellsCommandGroupDescription, "Click here to insert cells, rows, or columns into the sheet or table, or to add a sheet to the workbook.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupMarginsCommandGroup, "Margins");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupMarginsCommandGroupDescription, "Select the margin sizes for the document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSetPaperKind, "Size");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSetPaperKindDescription, "Choose a paper size for the current sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPaperKindCommandGroup, "Size");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPaperKindCommandGroupDescription, "Choose a paper size for the current sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintAreaCommandGroup, "Print Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintAreaCommandGroupDescription, "Mark a specific area of the sheet for printing.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSetPrintArea, "Set Print Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSetPrintAreaDescription, "Mark a specific area of the sheet for printing.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupClearPrintArea, "Clear Print Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupClearPrintAreaDescription, "Clear print area.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupAddPrintArea, "Add to Print Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupAddPrintAreaDescription, "Add a specific area of the sheet to Print Area.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintGridlines, "Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintGridlinesDescription, "Print the lines between rows and columns in the sheet to make reading easier.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintHeadings, "Headings");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupPrintHeadingsDescription, "Print row and column headings.\r\n\r\nRow headings are the row numbers to the size of the sheet.\r\n\r\nColumn headings are the letters or numbers that appear above the columns on a sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveCellsCommandGroup, "Delete");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveCellsCommandGroupDescription, "Delete cells, rows, or columns from the sheet or table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetRows, "Delete Sheet Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetRowsDescription, "Delete rows from worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetRowsContextMenuItem, "Delete");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetRowsContextMenuItemDescription, "Delete rows from worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetColumns, "Delete Sheet Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetColumnsDescription, "Delete columns from worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetColumnsContextMenuItem, "Delete");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetColumnsContextMenuItemDescription, "Delete columns from worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheet, "Delete Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetDescription, "Delete sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetContextMenuItem, "Delete");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveSheetContextMenuItemDescription, "Delete sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCommandGroup, "Format");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCommandGroupDescription, "Change the row height or column width, organize sheets, or protect or hide cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAutoFitRowHeight, "AutoFit Row Height");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAutoFitRowHeightDescription, "AutoFit row height.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAutoFitColumnWidth, "AutoFit Column Width");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatAutoFitColumnWidthDescription, "AutoFit column width.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTabColor, "Tab Color");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatTabColorDescription, "Change the sheet tab color.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideRows, "Hide Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideRowsDescription, "Hide Rows.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideRowsContextMenuItem, "Hide");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideRowsContextMenuItemDescription, "Hide Rows.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideColumns, "Hide Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideColumnsDescription, "Hide Columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideColumnsContextMenuItem, "Hide");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideColumnsContextMenuItemDescription, "Hide Columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideSheet, "Hide Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideSheetDescription, "Hide Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideSheetContextMenuItem, "Hide");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideSheetContextMenuItemDescription, "Hide Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideRows, "Unhide Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideRowsDescription, "Unhide Rows.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideRowsContextMenuItem, "Unhide");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideRowsContextMenuItemDescription, "Unhide Rows.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideColumns, "Unhide Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideColumnsDescription, "Unhide Columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideColumnsContextMenuItem, "Unhide");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideColumnsContextMenuItemDescription, "Unhide Columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideSheet, "Unhide Sheet...");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideSheetDescription, "Unhide Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideSheetContextMenuItem, "Unhide...");
			AddString(XtraSpreadsheetStringId.MenuCmd_UnhideSheetContextMenuItemDescription, "Unhide Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideAndUnhideCommandGroup, "Hide && Unhide");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideAndUnhideCommandGroupDescription, "Hide && Unhide.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RenameSheet, "Rename Sheet...");
			AddString(XtraSpreadsheetStringId.MenuCmd_RenameSheetDescription, "Rename Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RenameSheetContextMenuItem, "Rename...");
			AddString(XtraSpreadsheetStringId.MenuCmd_RenameSheetContextMenuItemDescription, "Rename Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRowHeight, "Row Height...");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatRowHeightDescription, "Row height.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatColumnWidth, "Column Width...");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatColumnWidthDescription, "Column width.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatDefaultColumnWidth, "Default Width...");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatDefaultColumnWidthDescription, "Standard column width.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheet, "Move or Copy Sheet...");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetDescription, "Move or Copy Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetContextMenuItem, "Move or Copy...");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetContextMenuItemDescription, "Move or Copy.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsNumber, "Format Cells: Number");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsNumberDescription, "Show the Number tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsAlignment, "Format Cells: Alignment");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsAlignmentDescription, "Show the Alignment tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsFont, "Format Cells: Font");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsFontDescription, "Show the Font tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsBorder, "Format Cells: Border");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsBorderDescription, "Show the Border tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsFill, "Format Cells: Fill");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsFillDescription, "Show the Fill tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsProtection, "Format Cells: Protection");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsProtectionDescription, "Show the Protection tab of the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsContextMenuItem, "Format Cells...");
			AddString(XtraSpreadsheetStringId.MenuCmd_FormatCellsContextMenuItemDescription, "Show the Format Cells dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsFinancialCommandGroup, "Financial");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsFinancialCommandGroupDescription, "Browse and select from a list of financial functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsLogicalCommandGroup, "Logical");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsLogicalCommandGroupDescription, "Browse and select from a list of logical functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsTextCommandGroup, "Text");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsTextCommandGroupDescription, "Browse and select from a list of text functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsDateAndTimeCommandGroup, "Date && Time");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsDateAndTimeCommandGroupDescription, "Browse and select from a list of date and time functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsLookupAndReferenceCommandGroup, "Lookup && Reference");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsLookupAndReferenceCommandGroupDescription, "Browse and select from a list of lookup and reference functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsMathAndTrigonometryCommandGroup, "Math && Trig");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsMathAndTrigonometryCommandGroupDescription, "Browse and select from a list of math and trigonometry functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsMoreCommandGroup, "More");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsMoreCommandGroupDescription, "Browse and select from a list of statistical, engineering, cube, informational, and compatibility functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsStatisticalCommandGroup, "Statistical");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsStatisticalCommandGroupDescription, "Browse and select from a list of statistical functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsEngineeringCommandGroup, "Engineering");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsEngineeringCommandGroupDescription, "Browse and select from a list of engineering functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsCubeCommandGroup, "Cube");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsCubeCommandGroupDescription, "Browse and select from a list of cube functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInformationCommandGroup, "Information");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInformationCommandGroupDescription, "Browse and select from a list of information functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsCompatibilityCommandGroup, "Compatibility");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsCompatibilityCommandGroupDescription, "Browse and select from a list of compatibility functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsWebCommandGroup, "Web");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsWebCommandGroupDescription, "Browse and select from a list of web functions.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsAutoSumCommandGroup, "AutoSum");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsAutoSumCommandGroupDescription, "Click here to display the result of a simple calculation, such as Average or Maximum Value, after the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSpecificFunction, "Insert specific function");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSpecificFunctionDescription, "Insert specific function.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSum, "Sum");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSumDescription, "Sum.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertAverage, "Average");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertAverageDescription, "Average.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCountNumbers, "Count Numbers");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCountNumbersDescription, "Count Numbers.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMax, "Max");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMaxDescription, "Max.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMin, "Min");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMinDescription, "Min.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCount, "Count");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCountDescription, "Count.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertProduct, "Product");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertProductDescription, "Product.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDev, "StdDev");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDevDescription, "StdDev.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDevp, "StdDevp");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDevpDescription, "StdDevp.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVar, "Var");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVarDescription, "Var.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVarp, "Varp");
			AddString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVarpDescription, "Varp.");
			AddString(XtraSpreadsheetStringId.MenuCmd_CollapseOrExpandFormulaBar, "Collapse or expand Formula Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_CollapseOrExpandFormulaBarDescription, "Collapse or expand Formula Bar (Ctrl+Shift+U)");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteTabDelimitedText, "Unformatted text (Tab delimited text)");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteTabDelimitedDescription, "Insert the contents of the Clipboard as text without any formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteCsvText, "Comma delimited (Csv)");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteCsvTextDescription, "Insert the contents of the Clipboard in Csv format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteBiff8Content, "Microsoft Excel 8.0 Format");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteBiff8ContentDescription, "Insert the contents of the Clipboard with the most of Microsoft Excel formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteXmlSpreadsheet, "Xml Spreadsheet 2003 (xml)");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteXmlSpreadsheetDescription, "Insert the contents of the Clipboard in Xml Spreadsheet 2003 format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteRtfText, "Formatted text (RTF)");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteRtfTextDescription, "Insert the contents of the Clipboard as text with font and table formatting.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteDataInterchangeFormat, "");
			AddString(XtraSpreadsheetStringId.MenuCmd_PasteDataInterchangeFormatDescription, "");
			AddString(XtraSpreadsheetStringId.TargetFrameDescription_NewWindow, "New window");
			AddString(XtraSpreadsheetStringId.TargetFrameDescription_ParentFrame, "Parent frame");
			AddString(XtraSpreadsheetStringId.TargetFrameDescription_SameFrame, "Same frame");
			AddString(XtraSpreadsheetStringId.TargetFrameDescription_WholePage, "Whole page");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCopyAreaCannotBeFitIntoThePasteArea, "The copy area cannot be fit into the paste area.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCopyAreaCannotOverlapUnlessSameSizeAndShape, "Copy and paste areas cannot overlap unless they are same size and shape.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveRangeWithLockedCells, "You are trying to delete a range that contains a locked cell. Locked cells cannot be deleted while the worksheet is protected.\r\n\r\nTo delete a locked cell, first remove protection using the Unprotect Sheet command (Review tab, Changes group). You may be prompted for a password.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameNotFounded, "DefinedName is not found.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorWorksheetWithNameNotFound, "Worksheet with this name is not found.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorShapeWithNameNotFound, "Shape with this name is not found.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorShapeWithIdNotFound, "Shape with this Id is not found.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnHeading, "Column heading is incorrect.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeading, "Row heading is incorrect.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCellIndexInRangeOutOfRange, "Cell index in a range should be non-negative and less than the number of cells in a range.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectIndexToInsert, "Worksheet index cannot be negative or greater than the number of worksheets in a workbook."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorNegativeFrozenColumnOffset, "Column offset should be non-negative."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorNegativeFrozenRowOffset, "Row offset should be non-negative."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorRowOffsetRefersBeyondWorksheet, "The specified offset refers to the row beyond the worksheet."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorColumnOffsetRefersBeyondWorksheet, "The specified offset refers to the column beyond the worksheet."); 
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidCustomPropertyValue, "The value entered does not match the specified type. The value will be stored as text.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidFilterArgument, "The filter argument entered is invalid.");
			AddString(XtraSpreadsheetStringId.HyperlinkForm_SelectedBookmarkNone, "<None>");
			AddString(XtraSpreadsheetStringId.HyperlinkForm_NodeCellReferences, "Cell References");
			AddString(XtraSpreadsheetStringId.HyperlinkForm_NodeDefinedName, "Defined Names");
			AddString(XtraSpreadsheetStringId.HyperlinkForm_DisabledDisplayText, "<<Selection in Document>>");
			AddString(XtraSpreadsheetStringId.Tooltip_Hyperlink, "Click once to follow. Click and hold to select this cell.");
			AddString(XtraSpreadsheetStringId.Tooltip_FormulaBar, "Formula Bar");
			AddString(XtraSpreadsheetStringId.Tooltip_NameBox, "Name Box");
			AddString(XtraSpreadsheetStringId.Tooltip_ExpandFormulaBar, "Expand Formula Bar");
			AddString(XtraSpreadsheetStringId.Tooltip_CollapseFormulaBar, "Collapse Formula Bar");
			AddString(XtraSpreadsheetStringId.Tooltip_FormulaBarCancelButton, "Cancel");
			AddString(XtraSpreadsheetStringId.Tooltip_FormulaBarOkButton, "Enter");
			AddString(XtraSpreadsheetStringId.Tooltip_FormulaBarFunctionButton, "Insert Function");
			AddString(XtraSpreadsheetStringId.Scope_Workbook, "Workbook");
			AddString(XtraSpreadsheetStringId.Scope_SheetIndex, "SheetIndex={0}");
			AddString(XtraSpreadsheetStringId.Scope_ExternalWorkbook, "external workbook");
			AddString(XtraSpreadsheetStringId.Scope_DefinedName, "DefinedName");
			AddString(XtraSpreadsheetStringId.MenuCmd_AddNewWorksheetDescription, "Insert Worksheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemoveFromSheet, "Clear Rules from Entire Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemoveFromSheetDescription, "Clear Rules from Entire Sheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemove, "Clear Rules from Selected Cells");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemoveDescription, "Clear Rules from Selected Cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingCommandGroup, "Conditional Formatting");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingCommandGroupDescription, "Highlight interesting cells, emphasize unusual values, and visualize data using Data Bars, Color Scales, and Icon Sets based on criteria.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScalesCommandGroup, "Color Scales");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScalesCommandGroupDescription, "Highlight interesting cells, emphasize unusual values, and visualize data using Color Scales based on criteria.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemoveCommandGroup, "Clear Rules");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingRemoveCommandGroupDescription, "Clear Rules.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowRed, "Green - Yellow - Red Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowRedDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedYellowGreen, "Red - Yellow - Green Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedYellowGreenDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteRed, "Green - White - Red Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteRedDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteGreen, "Red - White - Green Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteGreenDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleBlueWhiteRed, "Blue - White - Red Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleBlueWhiteRedDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteBlue, "Red - White - Blue Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteBlueDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteRed, "White - Red Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteRedDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhite, "Red - White Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhite, "Green - White Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteGreen, "White - Green Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteGreenDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellow, "Green - Yellow Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleYellowGreen, "Yellow - Green Color Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleYellowGreenDescription, "Displays a two or three color gradient in a range of cells. The shade of the color represents the value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsCommandGroup, "Data Bars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsCommandGroupDescription, "Highlight interesting cells, emphasize unusual values, and visualize data using Data Bars based on criteria.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroup, "Gradient Fill");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroupDescription, "Gradient Fill.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroup, "Solid Fill");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroupDescription, "Solid Fill.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsCommandGroup, "Icon Sets");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsCommandGroupDescription, "Highlight interesting cells, emphasize unusual values, and visualize data using Icon Sets based on criteria.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroup, "Directional");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroupDescription, "Directional.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsShapesCommandGroup, "Shapes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsShapesCommandGroupDescription, "Shapes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroup, "Indicators");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroupDescription, "Indicators.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroup, "Ratings");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroupDescription, "Ratings.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3Colored, "3 Arrows (Colored)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3ColoredDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3Grayed, "3 Arrows (Gray)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3GrayedDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4Colored, "4 Arrows (Colored)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4ColoredDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4Grayed, "4 Arrows (Gray)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4GrayedDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5Colored, "5 Arrows (Colored)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5ColoredDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5Grayed, "5 Arrows (Gray)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5GrayedDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTriangles3, "3 Triangles");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTriangles3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3, "3 Traffic Lights ()");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3Rimmed, "3 Traffic Lights (Rimmed)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3RimmedDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights4, "4 Traffic Lights");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights4Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSigns3, "3 Signs");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSigns3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRedToBlack, "Red To Black");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRedToBlackDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3Circled, "3 Symbols (Circled)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3CircledDescription, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3, "3 Symbols (Uncircled)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetFlags3, "3 Flags");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetFlags3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetStars3, "3 Stars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetStars3Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings4, "4 Ratings");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings4Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings5, "5 Ratings");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings5Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetQuarters5, "5 Quarters");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetQuarters5Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetBoxes5, "5 Boxes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetBoxes5Description, "Display an icon from the above icon set in each cell. Each icon represents a value in the cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientBlue, "Blue Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientBlueDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidBlue, "Blue Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidBlueDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientGreen, "Green Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientGreenDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidGreen, "Green Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidGreenDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientRed, "Red Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientRedDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidRed, "Red Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidRedDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientOrange, "Orange Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientOrangeDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidOrange, "Orange Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidOrangeDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientLightBlue, "Light Blue Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientLightBlueDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidLightBlue, "Light Blue Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidLightBlueDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientPurple, "Purple Data Bar (Gradient)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientPurpleDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidPurple, "Purple Data Bar (Solid)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidPurpleDescription, "View a colored data bar in the cell. The length of the data bar represents the value in the cell. A longer bar represents a higher value.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTopBottomRuleCommandGroup, "Top/Bottom Rules");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTopBottomRuleCommandGroupDescription, "Top/Bottom Rules");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10RuleCommand, "Top 10 Items...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10RuleCommandDescription, "Top 10 Items...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10RuleCommand, "Bottom 10 Items...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10RuleCommandDescription, "Bottom 10 Items...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10PercentRuleCommand, "Top 10%...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTop10PercentRuleCommandDescription, "Top 10%...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10PercentRuleCommand, "Bottom 10%...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBottom10PercentRuleCommandDescription, "Bottom 10%...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingAboveAverageRuleCommand, "Above Average...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingAboveAverageRuleCommandDescription, "Above Average...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBelowAverageRuleCommand, "Below Average...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBelowAverageRuleCommandDescription, "Below Average...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroup, "Highlight Cells Rules");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingHighlightCellsRuleCommandGroupDescription, "Highlight Cells Rules");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingGreaterThanRuleCommand, "Greater Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingGreaterThanRuleCommandDescription, "Greater Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingLessThanRuleCommand, "Less Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingLessThanRuleCommandDescription, "Less Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingEqualToRuleCommand, "Equal To...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingEqualToRuleCommandDescription, "Equal To...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTextContainsRuleCommand, "Text that Contains...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingTextContainsRuleCommandDescription, "Text that Contains...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDuplicateValuesRuleCommand, "Duplicate Values...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDuplicateValuesRuleCommandDescription, "Duplicate Values...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDateOccurringRuleCommand, "A Date Occurring...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDateOccurringRuleCommandDescription, "A Date Occurring...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBetweenRuleCommand, "Between...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingBetweenRuleCommandDescription, "Between...");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscending, "Sort A to Z");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscendingDescription, "Sort the selection so that the lowest values are at the top of the column.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescending, "Sort Z to A");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescendingDescription, "Sort the selection so that the highest values are at the top of the column.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterEquals, "Equals...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterEqualsDescription, "Equals.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqual, "Does Not Equal...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqualDescription, "Does not equal.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThan, "Greater Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanDescription, "Greater than.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualTo, "Greater Than Or Equal To...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualToDescription, "Greater than or equal to.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLessThan, "Less Than...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanDescription, "Less than.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualTo, "Less Than Or Equal To...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualToDescription, "Less than or equal to.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBetween, "Between...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBetweenDescription, "Between.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTop10, "Top 10 ...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTop10Description, "Top 10.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterAboveAverage, "Above Average");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterAboveAverageDescription, "Above average.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBelowAverage, "Below Average");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBelowAverageDescription, "Below average.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBeginsWith, "Begins With...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterBeginsWithDescription, "Begins with.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterEndsWith, "Ends With...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterEndsWithDescription, "Ends with.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterContains, "Contains...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterContainsDescription, "Contains.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotContain, "Does Not Contain...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotContainDescription, "Does not contain.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterToday, "Today");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTodayDescription, "Today.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterYesterday, "Yesterday");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterYesterdayDescription, "Yesterday.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTomorrow, "Tomorrow");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTomorrowDescription, "Tomorrow.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisWeek, "This Week");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisWeekDescription, "This week.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextWeek, "Next Week");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextWeekDescription, "Next week");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastWeek, "Last Week");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastWeekDescription, "Last week");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisMonth, "This Month");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisMonthDescription, "This month.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextMonth, "Next Month");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextMonthDescription, "Next month.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastMonth, "Last Month");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastMonthDescription, "Last month.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisQuarter, "This Quarter");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisQuarterDescription, "This quarter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextQuarter, "Next Quarter");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextQuarterDescription, "Next quarter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastQuarter, "Last Quarter");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastQuarterDescription, "Last quarter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisYear, "This Year");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterThisYearDescription, "This year.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextYear, "Next Year");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNextYearDescription, "Next year.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastYear, "Last Year");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterLastYearDescription, "Last year.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterYearToDate, "Year to Date");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterYearToDateDescription, "Year to date.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterQuarter, "Quarter {0}");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterQuarterDescription, "Quarter {0}.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterMonth, "{0}");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterMonthDescription, "{0}.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterCustom, "Custom Filter...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterCustomDescription, "Custom filter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterClear, "Clear");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterClearDescription, "Clear the filter and sort state for the current range of data.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClear, "Clear Filter From '{0}'");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClearDescription, "Clear filter from '{0}'.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterReApply, "Reapply");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterReApplyDescription, "Reapply the filter and sort in the current range.\r\n\r\nNew or modified data in the column won't be filtered or sorted until you click Reapply.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterToggle, "Filter");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterToggleDescription, "Enable filtering of the selected cells.\r\n\r\nOnce filtering is turned on, click the arrow in the column header to choose a filter for the column.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateFilters, "Date filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateFiltersDescription, "Date filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNumberFilters, "Number Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterNumberFiltersDescription, "Number filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTextFilters, "Text Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterTextFiltersDescription, "Text Filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterAllDatesInPeriodFilters, "All Dates in the Period");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterAllDatesInPeriodFiltersDescription, "All dates in the period.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateEquals, "Equals...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateEqualsDescription, "Equals.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateAfter, "After...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateAfterDescription, "After.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateBefore, "Before...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateBeforeDescription, "Before.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateBetween, "Between...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateBetweenDescription, "Between.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateCustom, "Custom Filter...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterDateCustomDescription, "Custom filter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterSimple, "Filter by Values ...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFilterSimpleDescription, "Filter by values.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnCommandGroup, "Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnCommandGroupDescription, "Insert a column chart.\r\n\r\nColumn charts are used to compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarCommandGroup, "Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarCommandGroupDescription, "Insert a bar chart.\r\n\r\nBar charts are the best chart type for comparing multiple values.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn2DCommandGroup, "2-D Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn3DCommandGroup, "3-D Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn3DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderCommandGroup, "Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeCommandGroup, "Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidCommandGroup, "Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBar2DCommandGroup, "2-D Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBar2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBar3DCommandGroup, "3-D Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBar3DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderCommandGroup, "Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeCommandGroup, "Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidCommandGroup, "Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieCommandGroup, "Pie");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieCommandGroupDescription, "Insert a pie chart.\r\n\r\nPie charts display the contribution of each value to a total.\r\n\r\nUse it when values can be added together or when you have only one data series and all values are positive.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie2DCommandGroup, "2-D Pie");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie3DCommandGroup, "3-D Pie");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie3DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnut2DCommandGroup, "Doughnut");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnut2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLineCommandGroup, "Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLineCommandGroupDescription, "Insert a line chart.\r\n\r\nLine charts are used to display trends over time.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine2DCommandGroup, "2-D Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine3DCommandGroup, "3-D Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine3DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubbleCommandGroup, "Bubble");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubbleCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartOtherCommandGroup, "Other Charts");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartOtherCommandGroupDescription, "Insert a stock or radar chart.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockCommandGroup, "Stock");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarCommandGroup, "Radar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnClustered2D, "Clustered Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnClustered2DDescription, "Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the order of categories is not important or for displaying item counts such as a histogram.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnStacked2D, "Stacked Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnStacked2DDescription, "Compare the contribution of each value to a total across categories by using vertical rectangles.\r\n\r\nUse it to emphasize the total across series for one category.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnPercentStacked2D, "100% Stacked Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnPercentStacked2DDescription, "Compare the percentage that each value contributes to a total across categories by using vertical rectangles.\r\n\r\nUse is to emphasize the proportion of each data series.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnClustered3D, "3-D Clustered Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnClustered3DDescription, "Compare values across categories and display clustered columns in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnStacked3D, "Stacked Column in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnStacked3DDescription, "Compare the contribution of each value to a total across categories and display stacked columns in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnPercentStacked3D, "100% Stacked Column in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumnPercentStacked3DDescription, "Compare the percentage that each value contributes to a total across categories and display 100% stacked columns in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderClustered, "Clustered Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderStacked, "Stacked Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderPercentStacked, "100% Stacked Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderPercentStackedDescription, "Compare the percentage that each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeClustered, "Clustered Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeStacked, "Stacked Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConePercentStacked, "100% Stacked Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConePercentStackedDescription, "Compare the percentage that each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidClustered, "Clustered Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidStacked, "Stacked Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidPercentStacked, "100% Stacked Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidPercentStackedDescription, "Compare the percentage that each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarClustered2D, "Clustered Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarClustered2DDescription, "Compare values across categories using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarStacked2D, "Stacked Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarStacked2DDescription, "Compare the contribution of each value to a total across categories by using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarPercentStacked2D, "100% Stacked Bar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarPercentStacked2DDescription, "Compare the percentage each value contributes to a total across categories using horizontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or when the category text is very long.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarClustered3D, "Clustered Bar in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarClustered3DDescription, "Compare values across categories and display clustered bars in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarStacked3D, "Stacked Bar in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarStacked3DDescription, "Compare the contribution of each value to a total across categories and display stacked bars in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarPercentStacked3D, "100% Stacked Bar in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBarPercentStacked3DDescription, "Compare the percentange each value contributes to a total across categories and display 100% stacked bars in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderClustered, "Clustered Horizontal Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderStacked, "Stacked Horizontal Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderPercentStacked, "100% Stacked Horizontal Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalCylinderPercentStackedDescription, "Compare the percentange each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeClustered, "Clustered Horizontal Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeStacked, "Stacked Horizontal Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConeStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConePercentStacked, "100% Stacked Horizontal Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalConePercentStackedDescription, "Compare the percentange each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidClustered, "Clustered Horizontal Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidClusteredDescription, "Compare values across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidStacked, "Stacked Horizontal Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidStackedDescription, "Compare the contribution of each value to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidPercentStacked, "100% Stacked Horizontal Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartHorizontalPyramidPercentStackedDescription, "Compare the percentange each value contributes to a total across categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn3D, "3-D Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartColumn3DDescription, "Compare values across categories and across series on three axes.\r\n\r\nUse it when the categories and series are equally important.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinder, "3-D Cylinder");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCylinderDescription, "Compare values across categories and across series and display a cylinder chart on three axes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartCone, "3-D Cone");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartConeDescription, "Compare values across categories and across series and display a cone chart on three axes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramid, "3-D Pyramid");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPyramidDescription, "Compare values across categories and across series and display a pyramid chart on three axes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie2D, "Pie");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie2DDescription, "Display the contribution of each value to a total.\r\n\r\nUse it when the values can be added together or when you have only one data series and all values are positive.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieExploded2D, "Exploded Pie");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieExploded2DDescription, "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a pie chart, and explode individual values instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie3D, "Pie in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPie3DDescription, "Display the contribution of each value to a total.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieExploded3D, "Exploded pie in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPieExploded3DDescription, "Display the contribution of each value to a total while emphasizing individual values.\r\n\r\nConsider using a 3-D pie chart, and explode individual values instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnut2D, "Doughnut");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnut2DDescription, "Display the contribution of each value to a total like a pie chart, but it can contain multiple series.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnutExploded2D, "Exploded Doughnut");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartDoughnutExploded2DDescription, "Display the contribution of each value to a total while emphasizing individual values like an exploded pie chart, but it can contain multiple series.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine, "Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLineDescription, "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are many data points and the order is important.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedLine, "Stacked Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedLineDescription, "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedLine, "100% Stacked line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedLineDescription, "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLineWithMarkers, "Line with Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLineWithMarkersDescription, "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when there are only a few data points.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedLineWithMarkers, "Stacked Line with Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedLineWithMarkersDescription, "Display the trend of the contribution of each value over time or ordered categories.\r\n\r\nConsider using a stacked area chart instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedLineWithMarkers, "100% Stacked Line with Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedLineWithMarkersDescription, "Display the trend of the percentage each value contributes over time or ordered categories.\r\n\r\nConsider using 100% stacked area chart instead.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine3D, "3-D Line");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartLine3DDescription, "Display each row or column of data as a 3-D ribbon on three axes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartAreaCommandGroup, "Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartAreaCommandGroupDescription, "Insert an area chart.\r\n\r\nArea charts emphasize differences between several sets of data over a period of time.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea2DCommandGroup, "2-D Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea2DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea3DCommandGroup, "3-D Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea3DCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea, "Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartAreaDescription, "Display the trend of values over time or categories.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedArea, "Stacked Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedAreaDescription, "Display the trend of the contribution of each value over time or categories.\r\n\r\nUse it to emphasize the trend in the total across series for one category.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedArea, "100% Stacked Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedAreaDescription, "Display the trend of the percentage each value contibutes over time or categories.\r\n\r\nUse it to emphasize the trend in the proportion of each series.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea3D, "3-D Area");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartArea3DDescription, "Display the trend of values over time or categories using areas on three axes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedArea3D, "Stacked Area in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStackedArea3DDescription, "Display the trend of the contribution of each value over time or categories by using stacked areas in a 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedArea3D, "100% Stacked Area in 3-D");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartPercentStackedArea3DDescription, "Display the trend of the percentage each value contributes over time or categories by using 100% stacked areas in 3-D format.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterCommandGroup, "Scatter");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterCommandGroupDescription, "Insert a Scatter chart, also known as an X Y chart.\r\n\r\nThis type of chart compares pairs of values.\r\n\r\nUse it when the values being charted are not in X-axis order or when they represent separate measurements.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterMarkers, "Scatter with only Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterMarkersDescription, "Compare pairs of values.\r\n\r\nUse is when the values are not in X-axis order or when they represent separate measurements.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterLines, "Scatter with Straight Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterLinesDescription, "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents separate samples.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterSmoothLines, "Scatter with Smooth Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterSmoothLinesDescription, "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis order and the data represents a function.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterLinesAndMarkers, "Scatter with Straight Lines and Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterLinesAndMarkersDescription, "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents separate samples.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterSmoothLinesAndMarkers, "Scatter with Smooth Lines and Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartScatterSmoothLinesAndMarkersDescription, "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis order and the data represents a function.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubble, "Bubble");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubbleDescription, "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubble3D, "Bubble with a 3-D effect");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartBubble3DDescription, "Resembles a scatter chart, but compares sets of three values instead of two. The third value determines the size of the bubble marker, which is displayed with a 3-D effect.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockHighLowClose, "High-Low-Close");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockHighLowCloseDescription, "Requires three series of values in order High, Low and Close.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockOpenHighLowClose, "Open-High-Low-Close");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockOpenHighLowCloseDescription, "Requires four series of values in order Open, High, Low and Close.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockVolumeHighLowClose, "Volume-High-Low-Close");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockVolumeHighLowCloseDescription, "Requires four series of values in order Volume, High, Low and Close.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockVolumeOpenHighLowClose, "Volume-Open-High-Low-Close");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartStockVolumeOpenHighLowCloseDescription, "Requires five series of values in order Volume, Open, High, Low and Close.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadar, "Radar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarDescription, "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarWithMarkers, "Radar with Markers");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarWithMarkersDescription, "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarFilled, "Filled Radar");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertChartRadarFilledDescription, "Display values relative to a center point.\r\n\r\nUse it when the categories are not directly comparable and there is only one series.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartAxesCommandGroup, "Axes");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartAxesCommandGroupDescription, "Change the formatting and layout of each axis.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisCommandGroup, "Primary Horizontal Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisCommandGroup, "Primary Vertical Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryHorizontalAxisCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryHorizontalAxisCommandDescription, "Do not display axis.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommand, "Show Left to Right Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisLeftToRightCommandDescription, "Display Axis Left to Right with Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommand, "Show Right to Left Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisRightToLeftCommandDescription, "Display Axis Right to Left with Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommand, "Show Axis without Labeling");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisHideLabelsCommandDescription, "Display Axis without labels or tick marks");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisDefaultCommand, "Show Default Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisDefaultCommandDescription, "Display Axis with default order and labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommand, "Show Axis in Log Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleLogarithmCommandDescription, "Display Axis using a log 10 base scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommand, "Show Axis in Thousands");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleThousandsCommandDescription, "Display Axis with numbers represented in Thousands");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommand, "Show Axis in Millions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleMillionsCommandDescription, "Display Axis with numbers represented in Millions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommand, "Show Axis in Billions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisScaleBillionsCommandDescription, "Display Axis with numbers represented in Billions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryVerticalAxisCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHidePrimaryVerticalAxisCommandDescription, "Do not display axis.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommand, "Show Left to Right Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisLeftToRightCommandDescription, "Display Axis Left to Right with Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommand, "Show Right to Left Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisRightToLeftCommandDescription, "Display Axis Right to Left with Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommand, "Show Axis without Labeling");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisHideLabelsCommandDescription, "Display Axis without labels or tick marks");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisDefaultCommand, "Show Default Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisDefaultCommandDescription, "Display Axis with default order and labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommand, "Show Axis in Log Scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleLogarithmCommandDescription, "Display Axis using a log 10 base scale");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommand, "Show Axis in Thousands");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleThousandsCommandDescription, "Display Axis with numbers represented in Thousands");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommand, "Show Axis in Millions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleMillionsCommandDescription, "Display Axis with numbers represented in Millions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommand, "Show Axis in Billions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisScaleBillionsCommandDescription, "Display Axis with numbers represented in Billions");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartGridlinesCommandGroup, "Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartGridlinesCommandGroupDescription, "Turn gridlines on or off.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroup, "Primary Horizontal Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesCommandGroup, "Primary Vertical Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesNoneCommandDescription, "Do not display Horizontal Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommand, "Major Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorCommandDescription, "Display Horizontal Gridlines for Major units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommand, "Minor Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMinorCommandDescription, "Display Horizontal Gridlines for Minor units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommand, "Major & Minor Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalGridlinesMajorAndMinorCommandDescription, "Display Horizontal Gridlines for Major and Minor units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesNoneCommandDescription, "Do not display Vertical Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorCommand, "Major Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorCommandDescription, "Display Vertical Gridlines for Major units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMinorCommand, "Minor Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMinorCommandDescription, "Display Vertical Gridlines for Minor units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommand, "Major & Minor Gridlines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalGridlinesMajorAndMinorCommandDescription, "Display Vertical Gridlines for Major and Minor units");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartAxisTitlesCommandGroup, "Axis Titles");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartAxisTitlesCommandGroupDescription, "Add, remove, or position the text used to label each axis.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroup, "Primary Horizontal Axis Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroup, "Primary Vertical Axis Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroupDescription, " ");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommandDescription, "Do not display an Axis Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommand, "Title Below Axis");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommandDescription, "Display Title below Horizontal Axis and resize chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommandDescription, "Do not display an Axis Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommand, "Rotated Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommandDescription, "Display Rotated Axis Title and resize chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommand, "Vertical Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommandDescription, "Display Axis Title with vertical text and resize chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommand, "Horizontal Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommandDescription, "Display Axis Title horizontally and resize chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleCommandGroup, "Chart Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleCommandGroupDescription, "Add, remove, or position the chart title.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleNoneCommandDescription, "Do not display a chart Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleCenteredOverlayCommand, "Centered Overlay Title");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleCenteredOverlayCommandDescription, "Overlay centered Title on chart without resizing chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleAboveCommand, "Above Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartTitleAboveCommandDescription, "Display Title at top of chart area and resize chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendCommandGroup, "Legend");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendCommandGroupDescription, "Add, remove, or position the chart legend.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendNoneCommandDescription, "Turn off Legend");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtRightCommand, "Show Legend at Right");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtRightCommandDescription, "Show Legend and align right");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtTopCommand, "Show Legend at Top");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtTopCommandDescription, "Show Legend and align top");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtLeftCommand, "Show Legend at Left");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtLeftCommandDescription, "Show Legend and align left");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtBottomCommand, "Show Legend at Bottom");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendAtBottomCommandDescription, "Show Legend and align bottom");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtRightCommand, "Overlay Legend at Right");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtRightCommandDescription, "Show Legend at right of the chart without resizing");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtLeftCommand, "Overlay Legend at Left");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLegendOverlayAtLeftCommandDescription, "Show Legend at left of the chart without resizing");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCommandGroup, "Data Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCommandGroupDescription, "Add, remove, or position data labels.\r\n\r\nUse data labels to label the elements of the chart with their actual data values.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsNoneCommandDescription, "Turn off Data Labels for selection");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsDefaultCommand, "Show");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsDefaultCommandDescription, "Turn on Data Labels for selection");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCenterCommand, "Center");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCenterCommandDescription, "Display Data Labels and position centered on the data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideEndCommand, "Inside End");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideEndCommandDescription, "Display Data Labels and position inside the end of data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideBaseCommand, "Inside Base");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideBaseCommandDescription, "Display Data Labels and position inside the base of data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsOutsideEndCommand, "Outside End");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsOutsideEndCommandDescription, "Display Data Labels and position outside the end of data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBestFitCommand, "Best Fit");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBestFitCommandDescription, "Display Data Labels and position with Best Fit");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsLeftCommand, "Left");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsLeftCommandDescription, "Display Data Labels and position left of the data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsRightCommand, "Right");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsRightCommandDescription, "Display Data Labels and position right of the data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsAboveCommand, "Above");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsAboveCommandDescription, "Display Data Labels and position above data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBelowCommand, "Below");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBelowCommandDescription, "Display Data Labels and position below data point(s)");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLinesCommandGroup, "Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLinesCommandGroupDescription, "Add other lines to the chart, such as Drop Lines or High-Low Lines.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLinesNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartLinesNoneCommandDescription, "Do not show Drop Lines, High-Low Lines or Series Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesCommand, "Drop Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesCommandDescription, "Show Drop Lines on an Area or Line Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowHighLowLinesCommand, "High-Low Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowHighLowLinesCommandDescription, "Show High-Low Lines on a 2D Line Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesAndHighLowLinesCommand, "Drop and High-Low Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesAndHighLowLinesCommandDescription, "Show Drop Lines and High-Low Lines on a 2D Line Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowSeriesLinesCommand, "Series Lines");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowSeriesLinesCommandDescription, "Show Series Lines on a 2D stacked Bar/Column Pie or Pie or Bar of Pie Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartUpDownBarsCommandGroup, "Up/Down Bars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartUpDownBarsCommandGroupDescription, "Add Up/Down Bars to the chart.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHideUpDownBarsCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartHideUpDownBarsCommandDescription, "Do not show Up/Down Bars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowUpDownBarsCommand, "Up/Down Bars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartShowUpDownBarsCommandDescription, "Show Up/Down Bars on a Line Chart");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsCommandGroup, "Error Bars");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsCommandGroupDescription, "Add error bars to the chart.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsNoneCommand, "None");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsNoneCommandDescription, "Removes the Error Bars for the selected series or all Error Bars if none are selected");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsPercentageCommand, "Error Bars with Percentage");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsPercentageCommandDescription, "Displays Error Bars for the selected chart series with 5% value");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardErrorCommand, "Error Bars with Standard Error");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardErrorCommandDescription, "Displays Error Bars for the selected chart series using Standard Error");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardDeviationCommand, "Error Bars with Standard Deviation");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardDeviationCommandDescription, "Displays Error Bars for the selected chart series with 1 standard deviation");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartSwitchRowColumnCommand, "Switch Row/Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartSwitchRowColumnCommandDescription, "Swap the data over the axis.\r\n\r\nData being charted on the X axis will move to the Y axis and vice versa.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChangeChartType, "Change Chart Type");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChangeChartTypeDescription, "Change to a different type of chart.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChangeChartTypeContextMenuItem, "Change Chart Type...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeTitleContextMenuItem, "Change Chart Title...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeTitleContextMenuItemDescription, "Change the chart title.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItem, "Change Horizontal Axis Title...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeHorizontalAxisTitleContextMenuItemDescription, "Change horizontal axis title.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeVerticalAxisTitleContextMenuItem, "Change Vertical Axis Title...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartChangeVerticalAxisTitleContextMenuItemDescription, "Change vertical axis title.");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertTable, "Table");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertTableDescription, "Insert Table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveToNextSheetDescription, "Switches between worksheet tabs, from left-to-right.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MoveToPreviousSheetDescription, "Switches between worksheet tabs, from right-to-left.");
			AddString(XtraSpreadsheetStringId.StyleName_Normal, "Normal");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_All, "&All");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_Formulas, "&Formulas");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_Values, "&Values");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_Formats, "Forma&ts");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_Comments, "&Comments");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_AllExceptBorders, "All e&xcept borders");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_ColumnWidths, "Column &widths");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_FormulasAndNumberFormats, "Fo&rmulas and number formats");
			AddString(XtraSpreadsheetStringId.Caption_PasteSpecial_ValuesAndNumberFormats, "Val&ues and number formats");
			AddString(XtraSpreadsheetStringId.Msg_OdsCaseSensitiveIgnored, "Case sensitivity is not supported. Function calculation results may differ.");
			AddString(XtraSpreadsheetStringId.Msg_OdsCalendarIgnored, "Non-gregorian calendar is not supported. Date values may differ.");
			AddString(XtraSpreadsheetStringId.Msg_OdsUnknownDateFormat, "Unknown date format. Date values may differ.");
			AddString(XtraSpreadsheetStringId.Msg_OdsTooLowDateValue, "Date must be greater than 01/01/1900. Date format is ignored.");
			AddString(XtraSpreadsheetStringId.Msg_OdsTimeFormatIgnored, "Unknown time format. Time value is ignored.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorectRotationAngleValue, "Rotation angle must be between -90 and 90 degrees.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorCommandCannotPerformedWithMultipleSelections, "The command you chose cannot be performed with multiple selections.\r\n\r\nSelect a single range and click the command again.");
			AddString(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound, "Built-in function is not found.");
			AddString(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidVolatile, "Custom function must have the same Volatile property value as the built-in function being replaced.");
			AddString(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidReturnType, "Custom function must have the same ReturnDataType as the built-in function being replaced.");
			AddString(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidParametersCount, "Custom function must have the same number of parameters as the built-in function being replaced.");
			AddString(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidParameter, "Custom function and built-in function must have Parameter {0} of the same type.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorOverlapRange, "A table cannot overlap another table.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUnionRange, "That command cannot be used on multiple selections.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingUnionRange, "You may not use reference operators (such as union, intersections, and ranges) or array constants for Conditional Formatting criteria.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingRange, "This type of reference cannot be used in a Conditional Formatting formula.\n Change the reference to a single cell, or use the reference with a worksheet function, such as =SUM(A1:E5).");
			AddString(XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingFormula, "Enter a valid formula.");
			AddString(XtraSpreadsheetStringId.Msg_CellOrChartIsReadonlyShort, "The cell or chart you're trying to change is on a protected sheet.");
			AddString(XtraSpreadsheetStringId.Msg_CellOrChartIsReadonly, "The cell or chart you're trying to change is on a protected sheet.\r\n\r\nTo make changes, click Unprotect Sheet in the Review tab (you might need a password).");
			AddString(XtraSpreadsheetStringId.Msg_PasswordNotConfirmed, "Confirmation password is not identical.");
			AddString(XtraSpreadsheetStringId.Msg_IncorrectPassword, "The password you supplied is not correct. Verify that the CAPS LOCK key is off and be sure to use the correct capitalization.");
			AddString(XtraSpreadsheetStringId.Msg_SearchCantFindData, "Cannot find the data you're searching for.");
			AddString(XtraSpreadsheetStringId.Msg_ReplaceAllSucceeded, "Search is completed and {0} replacements are made.");
			AddString(XtraSpreadsheetStringId.Msg_CantReplaceOnProtectedSheet, "You cannot use this command on a protected sheet. To use this command, you must first unprotect the sheet (Review tab, Changes group, Unprotect Sheet button). You may be prompted for a password.");
			AddString(XtraSpreadsheetStringId.Msg_ReplaceCantFindMatch, "Cannot find a match.");
			AddString(XtraSpreadsheetStringId.MoveOrCopySheetForm_Move, "Move");
			AddString(XtraSpreadsheetStringId.MoveOrCopySheetForm_Copy, "Copy");
			AddString(XtraSpreadsheetStringId.MoveOrCopySheetForm_MoveToEnd, "(move to end)");
			AddString(XtraSpreadsheetStringId.Msg_CanReplaceTheContentsOfTheDestinationCells, "Do you want to replace the contents of the destination cells?");
			AddString(XtraSpreadsheetStringId.Msg_CanSaveChangesNameReference, "Do you want to save the changes you made to the name reference?");
			AddString(XtraSpreadsheetStringId.Msg_UnableToSortSelection, "The command could not be completed by using range specified. Select a single cell within the range and try the command again.");
			AddString(XtraSpreadsheetStringId.Msg_UnableToSortMultipleSelection, "The command you chose cannot be performed with multiple selections.\r\n\r\nSelect a single range and click the command again.");
			AddString(XtraSpreadsheetStringId.Msg_UnableToSortMergedCells, "This operation requires the merged cells to be identically sized.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidSortColumnIndex, "This command cannot be performed with specified column index.");
			AddString(XtraSpreadsheetStringId.FontStyle_Regular, "Regular");
			AddString(XtraSpreadsheetStringId.FontStyle_Italic, "Italic");
			AddString(XtraSpreadsheetStringId.FontStyle_Bold, "Bold");
			AddString(XtraSpreadsheetStringId.FontStyle_BoldItalic, "Bold Italic");
			AddString(XtraSpreadsheetStringId.Caption_FormatNumberCustom, "Custom");
			AddString(XtraSpreadsheetStringId.Caption_FormatNumberDate, "Date");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleHeaderRowCommand, "Header Row");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleHeaderRowCommandDescription, "Turn on or off the header row of the table.\r\n\r\nA header row formats the top row of the table specially.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleTotalRowCommand, "Total Row");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleTotalRowCommandDescription, "Turn on or off the total row of the table.\r\n\r\nThe total row is the row at the end of the table which displays totals for each column.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleBandedColumnsCommand, "Banded Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleBandedColumnsCommandDescription, "Display banded columns, in which even columns are formatted differently from odd columns.\r\n\r\nThis banding can make tables easier to read.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleBandedRowsCommand, "Banded Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleBandedRowsCommandDescription, "Display banded rows, in which even rows are formatted differently from odd rows.\r\n\r\nThis banding can make tables easier to read.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleFirstColumnCommand, "First Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleFirstColumnCommandDescription, "Display special formatting for the first column of the table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleLastColumnCommand, "Last Column");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsToggleLastColumnCommandDescription, "Display special formatting for the last column of the table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsRenameTableCommand, "Table Name");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsRenameTableCommandDescription, "Edit the name of this table used to refer to it in formulas.");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsConvertToRange, "Convert to Range");
			AddString(XtraSpreadsheetStringId.MenuCmd_TableToolsConvertToRangeDescription, "Convert this table into a normal range of cells.\r\n\r\nAll of the data is preserved.");
			AddString(XtraSpreadsheetStringId.NameBox_SelectionModeFormat, "{0}R x {1}C");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDataTableExporterOverflow, "Narrowing Conversion (Subscribe to the CellValueConversionError event).");
			AddString(XtraSpreadsheetStringId.Msg_ErrorDataTableExporterConversionError, "Conversion error (Subscribe to the CellValueConversionError event).");
			AddString(XtraSpreadsheetStringId.Msg_ErrorNoColumnInDataTable, "There is no {0} column in the DataTable.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorNoColumnsInDataTable, "The number of Options.CustomConverters exceeds the number of dataTable columns.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorRangeColumnCountIsNotTheSameAsColumnCountInDataTable, "The number of columns in the DataTable must be the same as the number of columns in the cell range.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorConditionalFormattingRank, "Enter a whole number between 1 and 1000.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataMemberCommandDescription, "Select Data Member");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataMemberCommand, "Select Data Member");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeAddDataSourceCommand, "Add Data Source");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeAddDataSourceCommandDescription, "Create a connection to a data source.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataSourceCommand, "Select Data Source");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataSourceCommandDescription, "Select Data Source");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeDocumentsModeCommand, "Multiple Documents");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeDocumentsModeCommandDescription, "Create a separate workbook for each record of the data source.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOneDocumentModeCommand, "Multiple Sheets");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOneDocumentModeCommandDescription, "Create a separate worksheet in a single workbook for each record of the data source.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOneSheetModeCommand, "Single Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOneSheetModeCommandDescription, "Insert all merged ranges into a single worksheet according to the selected document orientation.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeHorizontalModeCommand, "Horizontal");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeHorizontalModeCommandDescription, "Insert merged ranges one after the other from left to right. Header and footer are on the left and on the right of the sheet respectively.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeVerticalModeCommand, "Vertical");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeVerticalModeCommandDescription, "Insert merged ranges one under the other. Header and footer are at the top and at the bottom of the sheet respectively.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOrientationCommandGroup, "Document Orientation");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeOrientationCommandGroupDescription, "Select either vertical or horizontal orientation for a resulting document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeResetRangeCommand, "Reset");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeResetRangeCommandDescription, "Reset selected ranges of the template.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailRangeCommand, "Detail");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailRangeCommandDescription, "Specify a detail range to repeat for each record of the data source.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupCommand, "Sort Fields");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupCommandDescription, "Specify sorting criteria for data in the selected detail range.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupHeaderCommand, "Group Header");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupHeaderCommandDescription, "Specify a group header. It is based on a sort field and displays information at the beginning of a group of records in a resulting document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupFooterCommand, "Group Footer");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupFooterCommandDescription, "Specify a group footer. It is based on a sort field and displays information at the end of a group of records in a resulting document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetFilterCommand, "Edit Filter");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetFilterCommandDescription, "Specify filter criteria for data in the selected detail range.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeResetFilterCommand, "Reset Filter");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeResetFilterCommandDescription, "Reset data filter in the selected detail range.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetFooterRangeCommand, "Footer");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetFooterRangeCommandDescription, "Specify a footer range in the template.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetHeaderRangeCommand, "Header");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetHeaderRangeCommandDescription, "Specify a header range in the template.");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMailMergeMasterDetailCommandGroup, "Master-Detail");
			AddString(XtraSpreadsheetStringId.MenuCmd_EditingMailMergeMasterDetailCommandGroupDescription, "Create a multi-level detail.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailLevelCommand, "Detail Level");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailLevelCommandDescription, "Specify the detail range of the next level.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailDataMemberCommand, "Data Member");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailDataMemberCommandDescription, "Set a data member for a detail level.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeShowRangesCommand, "Mail Merge Design View");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeShowRangesCommandDescription, "Highlight template ranges.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergePreviewCommand, "Mail Merge Preview");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergePreviewCommandDescription, "Preview a resulting document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewProtectSheet, "Protect Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewProtectSheetContextMenuItem, "Protect Sheet...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewProtectSheetDescription, "Prevent unwanted changes to the data in a sheet by specifying what information can be edited.\r\n\r\nFor example, you can prevent people from editing locked cells or changing the formatting of the document.\r\n\r\nYou can specify a password that can be entered to unprotect the sheet and allow these changes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewUnprotectSheet, "Unprotect Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewUnprotectSheetContextMenuItem, "Unprotect Sheet...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewUnprotectSheetDescription, "Prevent unwanted changes to the data in a sheet by specifying what information can be edited.\r\n\r\nFor example, you can prevent people from editing locked cells or changing the formatting of the document.\r\n\r\nYou can specify a password that can be entered to unprotect the sheet and allow these changes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewProtectWorkbook, "Protect Workbook");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewProtectWorkbookDescription, "Prevent unwanted changes to the structure of the workbook, such as moving, deleting, or adding sheets.\r\n\r\nYou can specify a password that can be entered to unprotect the workbook and allow these changes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewUnprotectWorkbook, "Unprotect Workbook");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewUnprotectWorkbookDescription, "Prevent unwanted changes to the structure of the workbook, such as moving, deleting, or adding sheets.\r\n\r\nYou can specify a password that can be entered to unprotect the workbook and allow these changes.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewShowProtectedRangeManager, "Allow Users to Edit Ranges");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewShowProtectedRangeManagerDescription, "Allows specific people to edit ranges of cells in a protected workbook or sheet.\r\n\r\nBefore using this feature, first set security on the sheet using the Protect Sheet command.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewInsertCommentContextMenuItem, "Insert Comment");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewInsertCommentContextMenuItemDescription, "Add a note about this part of the document.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewDeleteCommentContextMenuItem, "Delete Comment");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewDeleteCommentContextMenuItemDescription, "Delete the selected comment.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewShowHideComment, "Show/Hide Comment");
			AddString(XtraSpreadsheetStringId.MenuCmd_ReviewShowHideCommentDescription, "Show or hide the comment on the active cell.");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_LightRedFillWithDarkRedText, "Light Red Fill with Dark Red Text");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_YellowFillWithDarkYellowText, "Yellow Fill with Dark Yellow Text");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_GreenFillWithDarkGreenText, "Green Fill with Dark Green Text");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_LightRedFill, "Light Red Fill");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_RedText, "Red Text");
			AddString(XtraSpreadsheetStringId.ConditionalFormattingStyle_RedBorder, "Red Border");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_Yesterday, "Yesterday");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_Today, "Today");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_Tomorrow, "Tomorrow");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_Last7Days, "In the last 7 days");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_LastWeek, "Last week");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_ThisWeek, "This week");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_NextWeek, "Next week");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_LastMonth, "Last month");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_ThisMonth, "This month");
			AddString(XtraSpreadsheetStringId.Caption_TimePeriod_NextMonth, "Next month");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeText, "Text");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeNumber, "Number");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeDateTime, "Date");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTypeBoolean, "Yes or no");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyYes, "Yes");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyNo, "No");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyCheckedBy, "Checked by");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyClient, "Client");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDateCompleted, "Date completed");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDepartment, "Department");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDestination, "Destination");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDisposition, "Disposition");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDivision, "Division");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyDocumentNumber, "Document number");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyEditor, "Editor");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyForwardTo, "Forward to");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyGroup, "Group");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyLanguage, "Language");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyMailstop, "Mailstop");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyMatter, "Matter");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyOffice, "Office");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyOwner, "Owner");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyProject, "Project");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyPublisher, "Publisher");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyPurpose, "Purpose");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyReceivedFrom, "Received from");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyRecordedBy, "Recorded by");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyRecordedDate, "Recorded date");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyReference, "Reference");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertySource, "Source");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyStatus, "Status");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTelephoneNumber, "Telephone number");
			AddString(XtraSpreadsheetStringId.Caption_CustomPropertyTypist, "Typist");
			AddString(XtraSpreadsheetStringId.Caption_ChangeChartTitleFormLabel, "Chart Title:");
			AddString(XtraSpreadsheetStringId.Caption_ChangeChartHorizontalAxisTitleFormLabel, "Horizontal Axis Title:");
			AddString(XtraSpreadsheetStringId.Caption_ChangeChartVerticalAxisTitleFormLabel, "Vertical Axis Title:");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAddCondFmtToIncorrectSheet, "Conditional formatting can be added only to a worksheet that contains the cell range with this formatting.");
			AddString(XtraSpreadsheetStringId.Caption_CreateTable, "Create Table");
			AddString(XtraSpreadsheetStringId.Msg_ErrorResizeRangeToMergedCell, "This operation requires the merged cells to be identically sized.");
			AddString(XtraSpreadsheetStringId.Caption_GroupTableTools, "Tools");
			AddString(XtraSpreadsheetStringId.Msg_ShiftCellsInATableIsNotAllowed, "Shifting cells in a table is not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ShiftCellInAutoFilterIsNotAllowed, "Shifting cells in a table is not allowed.");
			AddString(XtraSpreadsheetStringId.Msg_ChangingRangeOfAutoFilterNotAllowed, "This operation is attempting to change a filtered range on a worksheet and can not be completed. To complete this operation, Autofilters in the sheet need to be removed.");
			AddString(XtraSpreadsheetStringId.Msg_IntersectedRangesCanNotBeChanged, "Can not use this command on overlapping selections.");
			AddString(XtraSpreadsheetStringId.Msg_DiffRangeTypesCanNotBeChanged, "The command can not be used with selections that contain entire rows or columns, and also other cells. Try selecting only entire rows, entire columns, or just groups of cells.");
			AddString(XtraSpreadsheetStringId.Msg_UnmergeMergedCellsClarification, "This operation will cause some merged cells to unmerge. Do you wish to continue?");
			AddString(XtraSpreadsheetStringId.Msg_TableOverlap, "Can not complete operation: A table cannot overlap with a PivotTable report, query results, a table, merged cells, or an XML mapping.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorTableCannotBeCreatedInTheLastRow, "Can not create a table in the last row of the worksheet. A table must have at least two rows, one for the table header, and one for data.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidObjectUsage, "The object is invalid.");
			AddString(XtraSpreadsheetStringId.Msg_CondFmtExpressionCantContainErrorValues, "Expression can not contain error value.");
			AddString(XtraSpreadsheetStringId.Msg_CondFmtExpressionCantContainRelativeRefs, "Expression can not contain relative references.");
			AddString(XtraSpreadsheetStringId.Msg_NoCellsWereFound, "No cells were found.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_NumberDescription_General, "General format cells have no specific number format.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_NumberDescription_Text, "Text format cells are treated as text even when a number is in the cell. The cell is displayed exactly as entered.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Number, "Number is used for general display of numbers. Currency and Accounting offer specialized formatting for monetary value.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Currency, "Currency formats are used for general monetary values. Use Accounting formats to align decimal points in a column.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Accounting, "Accounting formats line up the currency symbols and decimal points in a column.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Date, "Date formats display date and time serial numbers as date values. Date formats that begin with an asterisk (*) respond to changes in regional date and time settings that are specified for the operating system. Formats without an asterisk are not affected by operating system settings.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Time, "Time formats display date and time serial numbers as date values. Time formats that begin with an asterisk (*) respond to changes in regional date and time settings that are specified for the operating system. Formats without an asterisk are not affected by operating system settings.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Percentage, "Percentage formats multiply the cell value by 100 and displays the result with a percent symbol.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Custom, "Type the number format code, using one of the existing codes as a starting point.");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_General, "General");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_LeftIndent, "Left (Indent)");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Center, "Center");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_RightIndent, "Right (Indent)");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Fill, "Fill");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Justify, "Justify");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_CenterAcrossSelection, "Center Across Selection");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_DistributedIndent, "Distributed (Indent)");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Top, "Top");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Center, "Center");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Bottom, "Bottom");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Justify, "Justify");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Distributed, "Distributed");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_Context, "Context");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_LeftToRight, "Left-to-Right");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_RightToLeft, "Right-to-Left");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_None, "None");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_Single, "Single");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_Double, "Double");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_SingleAccounting, "Single Accounting");
			AddString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_DoubleAccounting, "Double Accounting");
			#endregion
			#region New texts
			AddString(XtraSpreadsheetStringId.Msg_CondFmtExpressionCantBeAnArray, "Array can not be used in conditional formatting formula.");
			AddString(XtraSpreadsheetStringId.Msg_CondFmtIncorectValueType, "Incorrect value type");
			AddString(XtraSpreadsheetStringId.FieldListDockPanel_Text, "Field List");
			AddString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_Text, "Parameters");
			AddString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_NameColumn, "Name");
			AddString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_ValueColumn, "Value");
			AddString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_EditParameters, "Edit Parameters...");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartSelectData, "Select Data");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartSelectDataDescription, "Change the data range included in the chart.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChartSelectDataContextMenuItem, "Select Data...");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedObject, "Can't use delete object.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorLastPrimaryAxisCannotBeChanged, "The last primary axis on the chart cannot be converted to a secondary axis.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAxisGroupCannotBeChanged, "Cannot change the axis type for this data series in the current chart view collection.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSomeChartTypesCannotBeCombinedWithOtherChartTypes, "Some chart types cannot be combined with other chart types. Select a different chart type.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSurfaceChartMustContainAtLeastTwoSeries, "A surface chart must contain at least two series.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectCreateStockHighLowCloseChart, "To create this stock chart, arrange the data on your sheet in this order: high price, low price, closing price. Use dates or stock names as labels.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectCreateStockOpenHighLowCloseChart, "To create this stock chart, arrange the data on your sheet in this order: opening price, high price, low price, closing price. Use dates or stock names as labels.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectCreateStockVolumeHighLowCloseChart, "To create this stock chart, arrange the data on your sheet in this order: volume traded, high price, low price, closing price. Use dates or stock names as labels.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectCreateStockVolumeOpenHighLowCloseChart, "To create this stock chart, arrange the data on your sheet in this order: volume traded, opening price, high price, low price, closing price. Use dates or stock names as labels.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidRange, "Invalid range.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidNumberFormat, "Can't use the specified number format.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorRangeContainsTable, "Specified range must not intersect a table.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorRangeConsistsOfEmptyCells, "Specified range must contain at least one cell with data.");
			AddString(XtraSpreadsheetStringId.Msg_ConfirmDeleteDefinedName, "Are you sure you want to delete the name {0} ?");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIncorrectErrorBarsCollectionCount, "Count of error bars collection should be between 0 and 2.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorErrorBarsWithBarDirectionAlreadyExists, "Error bars with the same direction already exists.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidOperationForDataSheet, "This operation is invalid for data sheet.");
			AddString(XtraSpreadsheetStringId.Caption_GroupOutline, "Outline");
			AddString(XtraSpreadsheetStringId.Caption_Subtotal_ColumnHeader, "(Column {0})");
			AddString(XtraSpreadsheetStringId.Caption_Subtotal_Grand, "Grand {0}");
			AddString(XtraSpreadsheetStringId.Caption_Subtotal_SumName, "Total");
			AddString(XtraSpreadsheetStringId.Msg_ConfirmModifyExistingOutline, "Modify existing outline?");
			AddString(XtraSpreadsheetStringId.Msg_CannotCreateOutline, "Cannot create an outline.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSubtotalUncomplete, "The command could not be completed by using the range specified. Select a single cell within the range and try the command again.");
			AddString(XtraSpreadsheetStringId.Msg_SubtotalNeedColumnNames, "Spreadsheet cannot determine which row in your list or selection contains column labels, which are required for this command.\r\n\r\n●If you want the first row of the selection or list used as labels and not as data, click OK.\r\n●If you selected a subset of cells in error, select a single cell, and try the command again.\r\n●To create column labels, click Cancel, and enter a text label at the top of each column of data.");
			AddString(XtraSpreadsheetStringId.Msg_GroupingNeedUnprotectSheet, "You cannot use this command on a protected sheet. To use this command, you must first unprotect the sheet (Review tab, Changes group, Unprotect Sheet button). You may be prompted for a password.");
			AddString(XtraSpreadsheetStringId.MenuCmd_AutoOutlineCommandDescription, "Analyze formulas to determine the grouping levels and create an outline that contains column groups, row groups or both.");
			AddString(XtraSpreadsheetStringId.MenuCmd_AutoOutlineCommand, "Auto Outline");
			AddString(XtraSpreadsheetStringId.MenuCmd_ClearOutlineCommandCommandDescription, "Ungroup all the groups in the outline.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ClearOutlineCommandCommand, "Clear Outline");
			AddString(XtraSpreadsheetStringId.MenuCmd_GroupCommandDescription, "Organize rows and columns into groups allowing you to show and hide them.");
			AddString(XtraSpreadsheetStringId.MenuCmd_GroupCommand, "Group");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideDetailCommandDescription, "Collapse a group of cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideDetailModeCommand, "Hide Detail");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowDetailCommandDescription, "Expand a collapsed group of cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowDetailCommand, "Show Detail");
			AddString(XtraSpreadsheetStringId.MenuCmd_SubtotalCommandDescription, "Calculate rows of related data by automatically inserting subtotals and totals for the selected cells.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SubtotalCommand, "Subtotal");
			AddString(XtraSpreadsheetStringId.MenuCmd_UngroupCommandDescription, "Ungroup a range of cells that were previously grouped.");
			AddString(XtraSpreadsheetStringId.MenuCmd_UngroupCommand, "Ungroup");
			AddString(XtraSpreadsheetStringId.MenuCmd_UngroupFormCaption, "Ungroup");
			AddString(XtraSpreadsheetStringId.MenuCmd_GroupFormCaption, "Group");
			AddString(XtraSpreadsheetStringId.MenuCmd_OutlineGroupCommandGroupDescription, "Group data or automatically outline a worksheet.");
			AddString(XtraSpreadsheetStringId.MenuCmd_OutlineUngroupCommandGroupDescription, "Ungroup data or remove an outline.");
			AddString(XtraSpreadsheetStringId.MenuCmd_OutlineDialogCommand, "Outline");
			AddString(XtraSpreadsheetStringId.MenuCmd_OutlineDialogCommandDescription, "Show the Outline dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommandGroup, "Manage Relations");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommandGroupDescription, "Add new queries to a data source or edit the existing queries. If a data source contains multiple data tables, create or modify master-detail relationships between data tables.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageQueriesCommand, "Manage Queries");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommand, "Manage Relations");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourceCommandGroup, "Manage Data Source");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourceCommandGroupDescription, "Add a new connection or select a data source from the list of existing connections. If a data source contains multiple data tables, select the required data member to perform mail merge.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourcesCommand, "Manage Data Sources");
			AddString(XtraSpreadsheetStringId.NewSpreadsheetMailMergeParameter_Text, "New Spreadsheet Parameter...");
			AddString(XtraSpreadsheetStringId.EditSpreadsheetMailMergeParameter_Text, "Spreadsheet Parameter");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToHideAllColumns, "Cannot hide all columns in a worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorAttemptToHideAllRows, "Cannot hide all rows in a worksheet.");
			AddString(XtraSpreadsheetStringId.Msg_ParametersInvalidCharacters, "Cannot create parameters with invalid names: ");
			AddString(XtraSpreadsheetStringId.Msg_ParametersNoName, "Cannot create a parameter without specifying its name.");
			AddString(XtraSpreadsheetStringId.Msg_ParametersIdenticalNames, "Cannot create parameters with identical names: ");
			AddString(XtraSpreadsheetStringId.Msg_ErrorUnionRangesWithDifferentWorksheets, "Union ranges containing ranges with different worksheets are not supported.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidCreateFromSelectionRange, "This selection is not valid.\nThe row or column containing the proposed names will not be included in the definitions of the names.\nIf the names you are creating are listed in a row or column, you must select more than one row or column.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorFirstDVCriteriaMustNotBeEmpty, "First data validation criteria must be non empty.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSecondDVCriteriaMustNotBeEmpty, "Second data validation criteria must be non empty.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorFirstDVCriteriaMustBeEmpty, "First data validation criteria must be empty.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSecondDVCriteriaMustBeEmpty, "Second data validation criteria must be empty.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorIndexOutOfRange, "Index value out of range.");
			AddString(XtraSpreadsheetStringId.Caption_GroupDataTools, "Data Tools");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidation, "Data Validation");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidationDescription, "Pick from a list of rules to limit the type of data that can be entered in a cell.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidationCircleInvalidData, "Circle Invalid Data");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidationCircleInvalidDataDescription, "Display circles around invalid data.");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidationClearValidationCircle, "Clear Validation Circles");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataValidationClearValidationCircleDescription, "Hide circles around invalid data.");
			AddString(XtraSpreadsheetStringId.Msg_SelectionContainsMoreThanOneDataValidation, "The selection contains more than one type of validation.\n\nErase current settings and continue?");
			AddString(XtraSpreadsheetStringId.Msg_SelectionContainsCellsWithoutDataValidation, "The selection contains some cells without Data Validation settings. Do you want to extend Data Validation to these cells?");
			AddString(XtraSpreadsheetStringId.Msg_MoreThan255InvalidDataCircles, "Your worksheet contains more than 255 invalid cells. Only the first 255 cells will be marked.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationFormulaIsEmpty, "You must enter {0}.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationBothFormulasAreEmpty, "You must enter both {0} and {1}.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationMinGreaterThanMax, "The {0} must be greater or equal to the {1}.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationDefinedNameNotFound, "Named range you specified cannot be found.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidNonnumericValue, "The {0} must be a numeric value, numeric expression, or refer to a cell containing a numeric value.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidDecimalValue, "Decimal values cannot be used for {0} conditions.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidNegativeValue, "Negative values cannot be used for {0} conditions.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationUnionRangeNotAllowed, "You may not use reference operators (such as unions, intersections, and ranges) or array constants for Data Validation criteria.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidReference, "This type of reference cannot be used in a Data Validation formula.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationMoreThanOneCellInRange, "This type of reference cannot be used in a Data Validation formula.\n\nChange the reference to a single cell, or use the reference with a worksheet function, such as {0}.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationMustBeRowOrColumnRange, "The list source must be a delimited list, or a reference to single row or column.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidDate, "The date you entered for the {0} is invalid.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationInvalidTime, "The time you entered for the {0} is invalid.");
			AddString(XtraSpreadsheetStringId.Msg_DataValidationFailed, "The value you entered is not valid.\n\nA user has restricted values that can be entered into this cell.");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationMinimum, "Minimum");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationMaximum, "Maximum");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationValue, "Value");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationStartDate, "Start date");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationEndDate, "End date");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationDate, "Date");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationStartTime, "Start time");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationEndTime, "End time");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationTime, "Time");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationLength, "Length");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationFormula, "Formula");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationSource, "Source");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationAnyValue, "Any value");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationWholeNumber, "Whole number");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationDecimal, "Decimal");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationList, "List");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationTextLength, "Text length");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationCustom, "Custom");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationStop, "Stop");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationWarning, "Warning");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationInformation, "Information");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationBetween, "between");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationNotBetween, "not between");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationEqual, "equal to");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationNotEqual, "not equal to");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThan, "greater than");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationLessThan, "less than");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThanOrEqual, "greater than or equal to");
			AddString(XtraSpreadsheetStringId.Caption_DataValidationLessThanOrEqual, "less than or equal to");
			AddString(XtraSpreadsheetStringId.DefaultInitialTableColumnNamePrefix, "Column");
			AddString(XtraSpreadsheetStringId.Msg_ErrorInvalidTableStyleType, "Style type is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorStyleTypeMustBeTableOrPivot, "Style type must be table, pivot or both.");
			AddString(XtraSpreadsheetStringId.Msg_CannotChangeDefaultOrPredefinedStyleType, "Type of default or predefined style cannot be changed.");
			AddString(XtraSpreadsheetStringId.Msg_CannotChangeAppliedStyleType, "Style is already applied to pivot or table. It's type can be only changed to support both.");
			#endregion
			AddString(XtraSpreadsheetStringId.CaptionAllFunctionsGroup, "All");
			AddString(XtraSpreadsheetStringId.CaptionDatabaseFunctionsGroup, "Database");
			AddString(XtraSpreadsheetStringId.CaptionUserDefinedFunctionsGroup, "User Defined");
			AddString(XtraSpreadsheetStringId.DefaultWorksheetName, "Sheet");
			AddString(XtraSpreadsheetStringId.DefaultRecoveredWorksheetName, "Recovered_Sheet");
			AddString(XtraSpreadsheetStringId.Msg_CommandRequiresAtLeastTwoRows, "This command requires at least two rows of source data. You cannot use this command on a selection in only one row.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableWillNotFitOnTheSheet, "The PivotTable report will not fit on the sheet. Do You want to show as much as possible?");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableWillNotFitOnTheSheetSelectNewLocation, "The PivotTable report will not fit on the sheet. Please select a different location.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableWillOverrideSheetCells, "There's already data on worksheet. Do you want to replace it?");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableIndentOverflow, "Because the maximum indentation limit for a cell has been reached, the next field will be displayed in a separate column.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableFieldNameIsInvalid, "The PivotTable field name is not valid. To create a PivotTable report, you must use data that organized as a list with labeled columns. If you a changing the name of a PivotTable field, you must type a new name for the field.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged, "We can't make this change for the selected cells because it will affect a PivotTable. Use the field list to change the report. If you are trying to insert or delete cells, move the PivotTable and try again.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTablePartCanNotBeChanged, "Cannot change this part of a PivotTable report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeBuiltFromEmptyCache, "The PivotTable report can not be built from empty data cache. Please refresh data cache.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow, "The pivot table groupped and calculated fields are not supported now.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableReportCanNotOverlapTable, "A PivotTable report cannot overlap a table or XML mapping.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableDataSourceAndDestinationReferencesAreBothInvalid, "References to data source and destination are both invalid.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableDataSourceReferenceNotValid, "Data source reference is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableDestinationReferenceNotValid, "Destination reference is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableNameAlreadyExists, "A PivotTable report with that name already exists on the destination sheet.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableNameIsInvalid, "Pivot table name is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableCanNotOverlapPivotTable, "A PivotTable report cannot overlap another PivotTable table.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableTooMuchDataFields, "A PivotTable report cannot contain more than 256 data fields.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableTooMuchPageFields, "A PivotTable report cannot contain more than 256 page fields.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableNotEnoughDataFields, "This operation requires at least two data fields on PivotTable report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCacheStringVeryLong, "Some cells or style in this workbook contain formatting that is not supported by the selected file format. These formats will be converted to the closest format available.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldNameAlreadyExists, "PivotTable field name already exists.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldCannotBePlacedOnThatAxis, "The field you are moving cannot be placed in that area of report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldHasTooMuchItems, "A field in your source data has more unique items than can be used in a PivotTable report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldHasTooMuchItems_ColumnField, "You cannot place a field that has more than 16384 items in the column area. If you want to use this field in the report, move the field to either the row area or page area.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldNameIsInvalid, "Cannot enter a null value as an item or field name in PivotTable report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCacheSourceTypeIsInvalid, "Pivot cache source must be of worksheet type."); 
			AddString(XtraSpreadsheetStringId.Msg_PivotCalculationRequiresField, "This calculation requires to specify a pivot field.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCalculationRequiresItem, "This calculation requires to specify a pivot item or set item type to previous/next.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCannotHideLastVisibleItem, "Cannot hide last visible pivot item.");
			AddString(XtraSpreadsheetStringId.Msg_CantExportPivotCacheSourceExternal, "Pivot cache with external source is not supported.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFieldContainsTooMuschUniqueItems, "Only 32500 unique items per field are supported.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterRequiresValue, "This pivot filter type requires to specify a value.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterRequiresSecondValue, "This pivot filter type requires to specify two values.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterRequiresMeasureField, "This pivot filter type requires to specify measure field.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterTop10CountMustBeInteger, "This filter's value must be integer.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterCannotAddFilterToPageField, "Cannot add pivot filter to page field.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterCannotChangeTop10TypeProperty, "To change this property pivot filter type must be count, sum or percent.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCacheFieldContainsNonDateItems, "Cannot add date filter. Pivot cache field contains non date items.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableSavedWithoutUnderlyingData, "The PivotTable report was saved without the underlying data. Use the Refresh Data command to update the report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotCanNotDetermineField, "Cannot detemine which PivotTable field to sort by.");
			AddString(XtraSpreadsheetStringId.Caption_PivotDefaultColumnHeader, "Column Labels");
			AddString(XtraSpreadsheetStringId.Caption_PivotDefaultDataCaption, "Values");
			AddString(XtraSpreadsheetStringId.Caption_PivotDefaultRowHeader, "Row Labels");
			AddString(XtraSpreadsheetStringId.Caption_PivotGrandTotal, "Grand Total");
			AddString(XtraSpreadsheetStringId.Caption_PivotEmptyValue, "(blank)");
			AddString(XtraSpreadsheetStringId.Caption_PivotTrueValue, "TRUE");
			AddString(XtraSpreadsheetStringId.Caption_PivotFalseValue, "FALSE");
			AddString(XtraSpreadsheetStringId.Msg_ErrorRangeMustConsistOfASingleRowOrColumn, "Specified range must consist of a single row or column.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSparklinePositionsMustBeUnique, "Specified position range is invalid. Sparkline positions must be unique.");
			AddString(XtraSpreadsheetStringId.Msg_ErrorSparklinePositionOrDataRangeIsInvalid, "Specified position or data range is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_CantDoThatToAMergedCell, "We can't do that to a merged cell.");
			AddString(XtraSpreadsheetStringId.Msg_SelectedCellsAffectsPivotTableAndCanNotBeChanged, "We can't make this change for the selected cells because it will affect a PivotTable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetup, "Page Setup");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupDescription, "Show the Page tab of the Page Setup dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupMargins, "Page Setup: Margins");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupMarginsDescription, "Show the Margins tab of the Page Setup dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupHeaderFooter, "Page Setup: Header/Footer");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupHeaderFooterDescription, "Show the Header/Footer tab of the Page Setup dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSheet, "Page Setup: Sheet");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupSheetDescription, "Show the Sheet tab of the Page Setup dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupMorePaperSizes, "More Paper Sizes...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PageSetupCustomMargins, "Custom Margins...");
			AddString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeNone, "(None)");
			AddString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeAtEndOfSheet, "At end of sheet");
			AddString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeAsDisplayedOnSheet, "As displayed on sheet");
			AddString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeDisplayed, "displayed");
			AddString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeBlank, "<blank>");
			AddString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeDash, "--");
			AddString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeNA, "#N/A");
			AddString(XtraSpreadsheetStringId.Msg_IncorrectNumberRange, "The number must be between {0} and {1}. Try again by entering a number in this range.");
			AddString(XtraSpreadsheetStringId.Msg_InvalidNumber, "Integer is not valid.");
			AddString(XtraSpreadsheetStringId.Msg_PageSetupMarginsNotFitPageSize, "Margins do not fit page size.");
			AddString(XtraSpreadsheetStringId.Msg_HeaderFooterTooLongTextString, "The text string you entered is too long. Reduce the number of characters used.");
			AddString(XtraSpreadsheetStringId.Msg_PageSetupProblemFormula, "The formula you typed contains an error. The range reference or defined name is missing.");
			AddString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_None, "(none)");
			AddString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_Page, "Page");
			AddString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_Of, "of");
			AddString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_Confidential, "Confidential");
			AddString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_PreparedBy, "Prepared by");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_PageNumberAnalog, "&[Page]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_PageTotalAnalog, "&[Pages]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_DateAnalog, "&[Date]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_TimeAnalog, "&[Time]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorkbookFilePathAnalog, "&[Path]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorkbookFileNameAnalog, "&[File]");
			AddString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorksheetNameAnalog, "&[Tab]");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertPivotTable, "PivotTable");
			AddString(XtraSpreadsheetStringId.MenuCmd_InsertPivotTableDescription, "Insert PivotTable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_OptionsPivotTable, "PivotTable Options");
			AddString(XtraSpreadsheetStringId.MenuCmd_OptionsPivotTableDescription, "Show the PivotTable Options dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_OptionsPivotTableContextMenuItem, "PivotTable Options...");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotTable, "Move PivotTable");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotTableDescription, "Move the PivotTable to another location in the workbook.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChangeDataSourcePivotTable, "Change Data Source");
			AddString(XtraSpreadsheetStringId.MenuCmd_ChangeDataSourcePivotTableDescription, "Change the source data for this PivotTable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectFieldTypePivotTable, "Field Settings");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectFieldTypePivotTableDescription, "Show the Field Settings dialog box.");
			AddString(XtraSpreadsheetStringId.MenuCmd_FieldSettingsPivotTableContextMenuItem, "Field Settings...");
			AddString(XtraSpreadsheetStringId.MenuCmd_DataFieldSettingsPivotTableContextMenuItem, "Value Field Settings...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableExpandField, "Expand Field");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableExpandFieldDescription, "Expand all items of the active field.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableExpandFieldContextMenuItem, "Expand");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseField, "Collapse Field");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseFieldDescription, "Collapses all items of the active field.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseFieldContextMenuItem, "Collapse");
			AddString(XtraSpreadsheetStringId.MenuCmd_FieldListPanelPivotTable, "Field List");
			AddString(XtraSpreadsheetStringId.MenuCmd_FieldListPanelPivotTableDescription, "Show or hide the Field List.\n\nThe field list allows you to add and remove fields from your PivotTable report.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowFieldListPanelPivotTableContextMenuItem, "Show Field List");
			AddString(XtraSpreadsheetStringId.MenuCmd_HideFieldListPanelPivotTableContextMenuItem, "Hide Field List");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshCommandAndCommandGroup, "Refresh");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshAllCommand, "Refresh All");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshDescription, "Get the latest data by refreshing all sources in the workbook.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowPivotTableExpandCollapseButton, "+/- Buttons");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowPivotTableExpandCollapseButtonDescription, "These buttons allow you to expand or collapse items within the PivotTable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowPivotTableFieldHeaders, "Field Headers");
			AddString(XtraSpreadsheetStringId.MenuCmd_ShowPivotTableFieldHeadersDescription, "Display the field headers for rows and columns.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableClearCommandGroup, "Clear PivotTable");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableClearCommandGroupDescription, "Remove fields, formatting, and filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_ClearAllPivotTable, "Clear All");
			AddString(XtraSpreadsheetStringId.MenuCmd_ClearAllPivotTableDescription, "Pivot Clear All");
			AddString(XtraSpreadsheetStringId.MenuCmd_ClearFiltersPivotTable, "Clear Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSelectCommandGroup, "Select");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSelectCommandGroupDescription, "Select an element of the PivotTable.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectLabelsAndValuesPivotTable, "Labels and Values");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectLabelsAndValuesPivotTableDescription, "Select Labels and Values");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectValuesPivotTable, "Values");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectValuesPivotTableDescription, "Select PivotTable Values");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectLabelsPivotTable, "Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectLabelsPivotTableDescription, "Select Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectEntirePivotTable, "Entire PivotTable");
			AddString(XtraSpreadsheetStringId.MenuCmd_SelectEntirePivotTableDescription, "Select Entire PivotTable");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSubtotalsCommandGroup, "Subtotals");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSubtotalsCommandGroupDescription, "Show or hide subtotals.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsCommandGroup, "Grand Totals");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsCommandGroupDescription, "Show or hide grand totals.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableReportLayoutCommandGroup, "Report Layout");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableReportLayoutCommandGroupDescription, "Adjust the report layout.\n\nThe compact form optimizes for readability, while the tabular and outline forms include field headers.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableBlankRowsCommandGroup, "Blank Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableBlankRowsCommandGroupDescription, "Emphasize groups by adding a blank line between each grouped item.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableDoNotShowSubtotals, "Do Not Show Subtotals");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowAllSubtotalsAtBottom, "Show all Subtotals at Bottom of Group");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowAllSubtotalsAtTop, "Show all Subtotals at Top of Group");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOffRowsColumns, "Off for Rows and Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnRowsColumns, "On for Rows and Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnRowsOnly, "On for Rows Only");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnColumnsOnly, "On for Columns Only");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowCompactForm, "Show in Compact Form");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowOutlineForm, "Show in Outline Form");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowTabularForm, "Show in Tabular Form");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableRepeatAllItemLabels, "Repeat All Item Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableDoNotRepeatItemLabels, "Do Not Repeat Item Labels");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableInsertBlankLine, "Insert Blank Line after Each Item");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableRemoveBlankLine, "Remove Blank Line after Each Item");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleRowHeaders, "Row Headers");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleRowHeadersDescription, "Display special formatting for the first row of the table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleColumnHeaders, "Column Headers");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleColumnHeadersDescription, "Display special formatting for the first column of the table.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedRows, "Banded Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedRowsDescription, "Display banded rows, in which even rows are formatted differently from odd rows.\n\nThis banding can make tables easier to read.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedColumns, "Banded Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedColumnsDescription, "Display banded columns, in which even columns are formatted differently from odd columns.\n\nThis banding can make tables easier to read.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableFieldsFilters, "Select Field:");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableFieldsFiltersDescription, "Select Field.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableLabelFilters, "Label Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableLabelFiltersDescription, "Label Filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableDateFilters, "Date Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableDateFiltersDescription, "Date Filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableValueFilters, "Value Filters");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableValueFiltersDescription, "Value Filters.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableItemFilter, "Item Filter...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableItemFilterDescription, "Item Filter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSortCommandGroup, "Sort");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSortCommandGroupDescription, "Sort.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableExpandCollapseCommandGroup, "Expand/Collapse");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableExpandCollapseCommandGroupDescription, "Expand/Collapse.");
			AddString(XtraSpreadsheetStringId.OptionsPivotTableForm_PageDownThenOver, "Down, Then Over");
			AddString(XtraSpreadsheetStringId.OptionsPivotTableForm_PageOverThenDown, "Over, Then Down");
			AddString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitAutomatic, "Automatic");
			AddString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitNone, "None");
			AddString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitMax, "Max");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionDefault, "Total");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum, "Sum");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount, "Count");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage, "Average");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax, "Max");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin, "Min");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct, "Product");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCountNumbers, "Count Numbers");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDev, "StdDev");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDevp, "StdDevp");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVar, "Var");
			AddString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVarp, "Varp");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueAsNoCalculation, "No Calculation");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfTotal, "% of Grand Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfColumn, "% of Column Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRow, "% of Row Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercent, "% Of");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentRow, "% of Parent Row Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentColumn, "% of Parent Column Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParent, "% of Parent Total");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueDifference, "Difference From");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentDifference, "% Difference From");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRunningTotal, "Running Total In");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRunningTotal, "% Running Total In");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankAscending, "Rank Smallest to Largest");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankDescending, "Rank Largest to Smallest");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueIndex, "Index");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemPrevious, "(previous)");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemNext, "(next)");
			AddString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_PartOfCustomName, " of ");
			AddString(XtraSpreadsheetStringId.PivotPageFieldAllItemsCaption, "(All)");
			AddString(XtraSpreadsheetStringId.PivotPageFieldMultipleItemsCaption, "(Multiple Items)");
			AddString(XtraSpreadsheetStringId.Msg_InvalidBorderStyleRemoved, "Border style is invalid, has been removed.");
			AddString(XtraSpreadsheetStringId.Msg_PasteAreaNotSameSizeAsSelectionClarification, "The data you're pasting isn't the same size as your selection. Do you want to paste anyway?");
			AddString(XtraSpreadsheetStringId.Msg_ChartDataRangeIntersectPivotTable, "Chart data range can not intersect pivot table(s).");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotBeginWith, "Does Not Begin With...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotBeginWithDescription, "Does Not Begin With.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotEndWith, "Does Not End With...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotEndWithDescription, "Does Not End With.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetween, "Not Between...");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetweenDescription, "Not Between.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotClearFieldFilter, "Clear Filter");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotClearFieldFilterDescription, "Clear filter.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyNumbers, "Sort Smallest to Largest");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyNumbersDescription, "Sort smallest to largest.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyNumbers, "Sort Largest to Smallest");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyNumbersDescription, "Sort largest to smallest.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyDates, "Sort Oldest to Newest");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyDatesDescription, "Sort oldest to newest.");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyDates, "Sort Newest to Oldest");
			AddString(XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyDatesDescription, "Sort newest to oldest.");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemovePivotField, "Remove '{0}'");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemovePivotFieldDescription, "Remove '{0}'.");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableSummarizeValuesBy, "Summarize Values By");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableMoreOptions, "More Options...");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveGrandTotalPivotTable, "Remove Grand Total");
			AddString(XtraSpreadsheetStringId.MenuCmd_RemoveGrandTotalPivotTableDescription, "Remove Grand Total.");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReference, "Move");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceUp, "Move '{0}' Up");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceDown, "Move '{0}' Down");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToBeginning, "Move '{0}' to Beginning");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToEnd, "Move '{0}' to End");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToLeft, "Move '{0}' to Left");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToRight, "Move '{0}' to Right");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToRows, "Move '{0}' to Rows");
			AddString(XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToColumns, "Move '{0}' to Columns");
			AddString(XtraSpreadsheetStringId.MenuCmd_PivotTableShowValuesAs, "Show Values As");
			AddString(XtraSpreadsheetStringId.MenuCmd_SubtotalPivotField, "Subtotal '{0}'");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBetween, "is between");
			AddString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNotBetween, "is not between");
			AddString(XtraSpreadsheetStringId.Caption_Top10FilterTypeSum, "Sum");
			AddString(XtraSpreadsheetStringId.Caption_ShowValuesAsCalculation, "Calculation:");
			AddString(XtraSpreadsheetStringId.Msg_PivotValueFilterNeedAtLeastOneDataField, "You need at least one field in the Values area in order to apply a value filter.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterValueMustBeBetween, "You must enter {0} between {1} and {2}. Try again by entering {0} in this range.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterNumber, "a number");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterNumbers, "numbers");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterInteger, "an integer");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterEndNumberMustBeGreaterThanStartNumber, "The end number must be greater than the start number.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableCanNotApplyDataValidation, "You cannot apply data validation to cells in a PivotTable report.");
			AddString(XtraSpreadsheetStringId.Msg_PivotFilterInvalidDate, "The date you entered is not a valid date. Please try again.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableSubtotalListPlace, "This command subtotals lists in place. To display and remove subtotals in a PivotTable report, use the Field Settings command.");
			AddString(XtraSpreadsheetStringId.Msg_PivotTableFieldMoveRestricted, "The field you are moving is restricted so it cannot be removed from the PivotTable.");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetResLocalizer();
		}
		public static string GetString(XtraSpreadsheetStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetStringId id, string str) {
			Dictionary<XtraSpreadsheetStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetResLocalizer
	public class XtraSpreadsheetResLocalizer : XtraResXLocalizer<XtraSpreadsheetStringId> {
		public XtraSpreadsheetResLocalizer()
			: base(new XtraSpreadsheetLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationRes", typeof(XtraSpreadsheetResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationRes", typeof(XtraSpreadsheetResLocalizer).GetAssembly());
#endif
		}
	}
	#endregion
}
