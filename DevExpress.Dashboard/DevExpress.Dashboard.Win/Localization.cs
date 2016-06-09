#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Globalization;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.DashboardWin.Localization {
	public enum DashboardWinStringId {
		MenuDimensionSortBy,
		MenuFormatRulesAdd,
		MenuMore,
		MenuColorBy,
		MenuDateTimeFormatFormat,
		MenuResetColor,
		MenuRemoveColor,
		MenuRetainColor,
		FileFilterAll,
		FileFilterDashboards,
		FileFilterAllImages,
		TooltipGroupByType,
		TooltipSortAscending,
		TooltipSortDescending,
		TooltipRefreshFieldList,
		MessageBoxWarningTitle,
		MessageBoxConfirmationTitle,
		MessageBoxErrorTitle,
		MessageNullDashboard,
		GridColumnFitToContentMenuItemCaption,
		GridColumnFitToContentMenuItemDescription,
		GridColumnWidthMenuItemCaption,
		GridColumnWidthMenuItemDescription,
		GridColumnFixedWidthMenuItemCaption,
		GridColumnFixedWidthMenuItemDescription,
		GridSortAscendingMenuItemCaption,
		GridSortAscendingMenuItemDescription,
		GridSortDescendingMenuItemCaption,
		GridSortDescendingMenuItemDescription,
		AddGridColumnTotalBarItemCaption,
		ClearGridColumnTotalsBarItemCaption,
		RemoveGridColumnTotalBarItemCaption,
		GridColumnTotalCountBarItemCaption,
		GridColumnTotalMinBarItemCaption,
		GridColumnTotalMaxBarItemCaption,
		GridColumnTotalAvgBarItemCaption,
		GridColumnTotalSumBarItemCaption,
		GridColumnTotalAutoBarItemCaption,
		GridClearSortingMenuItemCaption,
		GridClearSortingMenuItemDescription,
		ResetGridColumnsWidthMenuItemCaption,
		ResetGridColumnsWidthMenuItemDescription,
		MessageExternalDashboardItem,
		DashboardDesignerCaption,
		DashboardDesignerDefaultDashboardName,
		DashboardDesignerEmptyMessage,
		DashboardDesignerUnableCreateRibbonMessage,
		DashboardDesignerUnableCreateBarsMessage,
		DashboardDesignerConfirmSaveMessage,
		DashboardDesignerDataReducedMessage,
		DashboardDesignerDataObsoleteMessage,
		DragAreaDashboardItemHasNotDataItemsMessage,
		DragAreaHasNotDashboardItemMessage,
		MessageFileNotFound,
		MessageConfirmUnpinnedItemsRemove,
		MessageCollectionTypesNotSupported,
		MessageInteractivityDashboardItemNotFound,
		MessageInteractivityDataDashboardItemRequired,
		MessageInteractivityRangeFilterRequired,
		MessageInteractivityOperationNotAvailable,
		MessageEmptyDrillDownValue,
		MessageServerFailedCalculatedFields,
		MessageDataProcessingModeChanging,
		MessageNewColorTableDialogSwitchingDataSource,
		MessageNewColorTableDialogSwitchingDataSourceCaption,
		MessageColorTableAlreadyExists,
		MessageColorTableAlreadyExistsCaption,
		MessageColorSchemeValueAlreadyExists,
		MessageColorSchemeValueAlreadyExistsCaption,
		MessageDashboardLocked,
		MessageSummaryCalculatedFieldInDimension,
		DashboardViewerEmptyDesignerMessage,
		DashboardLoadingError,
		DashboardSavingError,
		BarDashboardCaption,
		RibbonPageCategoryPivotToolsName,
		RibbonPageCategoryGridToolsName,
		RibbonPageCategoryChartToolsName,
		RibbonPageCategoryScatterChartToolsName,
		RibbonPageCategoryPiesToolsName,
		RibbonPageCategoryGaugesToolsName,
		RibbonPageCategoryCardsToolsName,
		RibbonPageCategoryImageToolsName,
		RibbonPageCategoryTextBoxToolsName,
		RibbonPageCategoryRangeFilterToolsName,
		RibbonPageCategoryMapToolsName,
		RibbonPageCategoryChoroplethMapToolsName,
		RibbonPageCategoryGeoPointMapToolsName,
		RibbonPageCategoryBubbleMapToolsName,
		RibbonPageCategoryPieMapToolsName,
		RibbonPageCategoryFilterElementToolsName,
		RibbonPageCategoryGroupToolsName,
		RibbonPageDashboardCaption,
		RibbonPageDataCaption,
		RibbonPageLayoutAndStyleCaption,
		RibbonPageImageOptionsCaption,
		RibbonPageTextBoxFormatCaption,
		RibbonPageRangeFilterStyleCaption,
		RibbonGroupFileCaption,
		RibbonGroupHistoryCaption,
		RibbonGroupDataSourceCaption,
		RibbonGroupInsertCaption,
		RibbonGroupItemsCaption,
		RibbonGroupGroupsCaption,
		RibbonGroupSkinsCaption,
		RibbonGroupFilteringCaption,
		RibbonGroupInteractivityCaption,
		RibbonGroupInteractivitySettingsCaption,
		RibbonGroupTargetDimensions,
		RibbonGroupContentArrangementCaption,
		RibbonGroupGridCellsCaption,
		RibbonGroupGridStyleCaption,
		RibbonGroupGridLayoutCaption,
		RibbonGroupGridColumnWidthModeCaption,
		RibbonGroupChartLayoutCaption,
		RibbonGroupScatterChartLabelsCaption,
		RibbonGroupChartSeriesTypeCaption,
		RibbonGroupChartLegendPositionCaption,
		RibbonGroupMapLegendPositionCaption,
		RibbonGroupWeightedLegendPositionCaption,
		RibbonGroupPieStyleCaption,
		RibbonGroupPieLabelsCaption,
		RibbonGroupGaugeStyleCaption,
		RibbonGroupGaugeLabelsCaption,
		RibbonGroupImageOpenCaption,
		RibbonGroupImageSizeModeCaption,
		RibbonGroupImageAlignmentCaption,
		RibbonGroupTextBoxSettingsCaption,
		RibbonGroupRangeFilterSeriesTypeCaption,
		RibbonGroupMapCaption,
		RibbonGroupMapShapefileCaption,
		RibbonGroupGeoPointMapClusterizationCaption,
		RibbonGroupPieMapOptionsCaption,
		RibbonGroupMapNavigationCaption,
		RibbonGroupMapShapeLabelsAttributeCaption,
		RibbonGroupDashboardCaption,
		RibbonGroupPivotLayoutCaption,
		RibbonGroupFilterElementTypeCaption,
		RibbonGroupTreeViewLayoutCaption,
		RibbonGroupElementItemPropertiesCaption,
		CommandUnsupportedCaption,
		CommandUnsupportedDescription,
		CommandNewDashboardCaption,
		CommandNewDashboardDescription,
		CommandOpenDashboardCaption,
		CommandOpenDashboardDescription,
		CommandSaveDashboardCaption,
		CommandSaveDashboardDescription,
		CommandSaveAsDashboardCaption,
		CommandSaveAsDashboardDescription,
		CommandUndoCaption,
		CommandUndoDescription,
		CommandRedoCaption,
		CommandRedoDescription,
		CommandNewDataSourceCaption,
		CommandNewDataSourceDescription,
		CommandEditDataSourceCaption,
		CommandEditDataSourceDescription,
		CommandRenameDataSourceCaption,
		CommandRenameDataSourceDescription,
		CommandServerModeCaption,
		CommandServerModeDescription,
		CommandDeleteDataSourceCaption,
		CommandDeleteDataSourceDescription,
		CommandSetCurrencyCultureCaption,
		CommandSetCurrencyCultureDescription,
		CommandUseGlobalColorsCaption,
		CommandUseGlobalColorsDescription,
		CommandUseLocalColorsCaption,
		CommandUseLocalColorsDescription,
		CommandEditLocalColorSchemeCaption,
		CommandEditLocalColorSchemeDescription,
		CommandEditDashboardColorSchemeCaption,
		CommandEditDashboardColorSchemeDescription,
		CommandDashboardTitleCaption,
		CommandDashboardTitleDescription,
		CommandInsertPivotCaption,
		CommandInsertPivotDescription,
		CommandInsertGridCaption,
		CommandInsertGridDescription,
		CommandInsertChartCaption,
		CommandInsertChartDescription,
		CommandInsertScatterChartCaption,
		CommandInsertScatterChartDescription,
		CommandInsertPiesCaption,
		CommandInsertPiesDescription,
		CommandInsertGaugesCaption,
		CommandInsertGaugesDescription,
		CommandInsertCardsCaption,
		CommandInsertCardsDescription,
		CommandInsertImageCaption,
		CommandInsertImageDescription,
		CommandInsertTextBoxCaption,
		CommandInsertTextBoxDescription,
		CommandInsertRangeFilterCaption,
		CommandInsertRangeFilterDescription,
		CommandInsertChoroplethMapCaption,
		CommandInsertChoroplethMapDescription,
		CommandInsertGeoPointMapsCaption,
		CommandInsertGeoPointMapsDescription,
		CommandInsertGeoPointMapCaption,
		CommandInsertGeoPointMapDescription,
		CommandInsertBubbleMapCaption,
		CommandInsertBubbleMapDescription,
		CommandInsertPieMapCaption,
		CommandInsertPieMapDescription,
		CommandInsertFilterElementsCaption,
		CommandInsertFilterElementsDescription,
		CommandInsertComboboxCaption,
		CommandInsertComboboxDescription,
		CommandInsertListBoxCaption,
		CommandInsertListBoxDescription,
		CommandInsertTreeViewCaption,
		CommandInsertTreeViewDescription,
		CommandInsertGroupCaption,
		CommandInsertGroupDescription,
		CommandDuplicateItemCaption,
		CommandDuplicateItemDescription,
		CommandDeleteItemCaption,
		CommandDeleteItemDescription,
		CommandDeleteGroupCaption,
		CommandDeleteGroupDescription,
		CommandRemoveDataItemsCaption,
		CommandRemoveDataItemsDescription,
		CommandTransposeItemCaption,
		CommandDefaultTransposeItemDescription,
		CommandTransposePivotDescription,
		CommandTransposePieDescription,
		CommandTransposeChartDescription,
		CommandTransposeScatterChartDescription,
		CommandConvertDashboardItemTypeCaption,
		CommandConvertDashboardItemTypeDescription,
		CommandConvertToGeoPointMapBaseCaption,
		CommandConvertToGeoPointMapBaseDescription,
		CommandConvertToPivotCaption,
		CommandConvertToPivotDescription,
		CommandConvertToGridCaption,
		CommandConvertToGridDescription,
		CommandConvertToChartCaption,
		CommandConvertToChartDescription,
		CommandConvertToScatterChartCaption,
		CommandConvertToScatterChartDescription,
		CommandConvertToPieCaption,
		CommandConvertToPieDescription,
		CommandConvertToGaugeCaption,
		CommandConvertToGaugeDescription,
		CommandConvertToCardCaption,
		CommandConvertToCardDescription,
		CommandConvertToChoroplethMapCaption,
		CommandConvertToChoroplethMapDescription,
		CommandConvertToGeoPointMapCaption,
		CommandConvertToGeoPointMapDescription,
		CommandConvertToBubbleMapCaption,
		CommandConvertToBubbleMapDescription,
		CommandConvertToPieMapCaption,
		CommandConvertToPieMapDescription,
		CommandConvertToRangeFilterCaption,
		CommandConvertToRangeFilterDescription,
		CommandConvertToComboBoxCaption,
		CommandConvertToComboBoxDescription,
		CommandConvertToListBoxCaption,
		CommandConvertToListBoxDescription,
		CommandConvertToTreeViewCaption,
		CommandConvertToTreeViewDescription,
		CommandEditNamesCaption,
		CommandEditNamesDescription,
		CommandEditRulesCaption,
		CommandEditRulesDescription,
		CommandShowCaptionCaption,
		CommandShowCaptionDescription,
		CommandEditFilterCaption,
		CommandEditFilterDescription,
		CommandClearFilterCaption,
		CommandClearFilterDescription,
		CommandSingleMasterFilterCaption,
		CommandMultipleMasterFilterCaption,
		CommandDrillDownCaption,
		CommandSingleMasterFilterDescription,
		CommandMultipleMasterFilterDescription,
		CommandDrillDownDescription,
		CommandDrillDownDisabledDescription,
		CommandTargetDimensionsArgumentsCaption,
		CommandTargetDimensionsSeriesCaption,
		CommandTargetDimensionsPointsCaption,
		CommandTargetDimensionsArgumentsDescription,
		CommandTargetDimensionsSeriesDescription,
		CommandTargetDimensionsPointsDescription,
		CommandCrossDataSourceFilteringCaption,
		CommandCrossDataSourceFilteringDescription,
		CommandIgnoreMasterFiltersCaption,
		CommandIgnoreMasterFiltersDescription,
		CommandGroupIgnoreMasterFiltersCaption,
		CommandGroupIgnoreMasterFiltersDescription,
		CommandGroupMasterFilterCaption,
		CommandGroupMasterFilterDescription,
		CommandContentAutoArrangeCaption,
		CommandContentAutoArrangeDescription,
		CommandContentArrangeInColumnsCaption,
		CommandContentArrangeInColumnsDescription,
		CommandContentArrangeInRowsCaption,
		CommandContentArrangeInRowsDescription,
		CommandContentArrangementCountCaption,
		CommandContentArrangementCountDescription,
		CommandGridHorizontalLinesCaption,
		CommandGridHorizontalLinesDescription,
		CommandGridWordWrapCaption,
		CommandGridWordWrapDescription,
		CommandGridAutoFitToContentsColumnWidthModeCaption,
		CommandGridAutoFitToContentsColumnWidthModeDescription,
		CommandGridVerticalLinesCaption,
		CommandGridVerticalLinesDescription,
		CommandGridAutoFitToGridColumnWidthModeCaption,
		CommandGridAutoFitToGridColumnWidthModeDescription,
		CommandGridManualGridColumnWidthModeCaption,
		CommandGridManualGridColumnWidthModeDescription,
		CommandGridMergeCellsCaption,
		CommandGridMergeCellsDescription,
		CommandGridBandedRowsCaption,
		CommandGridBandedRowsDescription,
		CommandGridColumnHeadersCaption,
		CommandGridColumnHeadersDescription,
		CommandChartRotateCaption,
		CommandChartRotateDescription,
		CommandChartSeriesTypeCaption,
		CommandChartSeriesTypeDescription,
		CommandChartShowLegendCaption,
		CommandChartShowLegendDescription,
		CommandScatterChartShowLegendCaption,
		CommandScatterChartShowLegendDescription,
		CommandScatterChartPointLabelOptionsCaption,
		CommandScatterChartPointLabelOptionsDescription,
		CommandChartYAxisSettingsCaption,
		CommandChartYAxisSettingsDescription,
		CommandChartXAxisSettingsCaption,
		CommandChartXAxisSettingsDescription,
		CommandScatterChartYAxisSettingsCaption,
		CommandScatterChartYAxisSettingsDescription,
		CommandScatterChartXAxisSettingsCaption,
		CommandScatterChartXAxisSettingsDescription,
		CommandChartLegendPositionCaption,
		CommandChartLegendPositionDescription,
		CommandScatterChartLegendPositionCaption,
		CommandScatterChartLegendPositionDescription,
		CommandChartAddPaneCaption,
		CommandChartAddPaneDescription,
		CommandChartRemovePaneCaption,
		CommandPieStylePieCaption,
		CommandPieStylePieDescription,
		CommandPieStyleDonutCaption,
		CommandPieStyleDonutDescription,
		CommandPieLabelsDataLabelsCaption,
		CommandPieLabelsDataLabelsDescription,
		CommandPieLabelsDataLabelsNoneCaption,
		CommandPieLabelsDataLabelsNoneDescription,
		CommandPieLabelsDataLabelsArgumentCaption,
		CommandPieLabelsDataLabelsArgumentDescription,
		CommandPieLabelsDataLabelsPercentCaption,
		CommandPieLabelsDataLabelsPercentDescription,
		CommandPieLabelsDataLabelsValueCaption,
		CommandPieLabelsDataLabelsValueDescription,
		CommandPieLabelsDataLabelsValueAndPercentCaption,
		CommandPieLabelsDataLabelsValueAndPercentDescription,
		CommandPieLabelsDataLabelsArgumentAndPercentCaption,
		CommandPieLabelsDataLabelsArgumentAndPercentDescription,
		CommandPieLabelsDataLabelsArgumentAndValueCaption,
		CommandPieLabelsDataLabelsArgumentAndValueDescription,
		CommandPieLabelsDataLabelsArgumentValueAndPercentCaption,
		CommandPieLabelsDataLabelsArgumentValueAndPercentDescription,
		CommandPieLabelsTooltipsCaption,
		CommandPieLabelsTooltipsDescription,
		CommandPieLabelsTooltipsNoneCaption,
		CommandPieLabelsTooltipsNoneDescription,
		CommandPieLabelsTooltipsArgumentCaption,
		CommandPieLabelsTooltipsArgumentDescription,
		CommandPieLabelsTooltipsPercentCaption,
		CommandPieLabelsTooltipsPercentDescription,
		CommandPieLabelsTooltipsValueCaption,
		CommandPieLabelsTooltipsValueDescription,
		CommandPieLabelsTooltipsValueAndPercentCaption,
		CommandPieLabelsTooltipsValueAndPercentDescription,
		CommandPieLabelsTooltipsArgumentAndPercentCaption,
		CommandPieLabelsTooltipsArgumentAndPercentDescription,
		CommandPieLabelsTooltipsArgumentAndValueCaption,
		CommandPieLabelsTooltipsArgumentAndValueDescription,
		CommandPieLabelsTooltipsArgumentValueAndPercentCaption,
		CommandPieLabelsTooltipsArgumentValueAndPercentDescription,
		CommandPieLabelsLegendPositionCaption,
		CommandPieLabelsLegendPositionDescription,
		CommandPieLabelsLegendPositionNoneCaption,
		CommandPieLabelsLegendPositionNoneDescription,
		CommandPieLabelsLegendPositionLeftCaption,
		CommandPieLabelsLegendPositionLeftDescription,
		CommandPieLabelsLegendPositionRightCaption,
		CommandPieLabelsLegendPositionRightDescription,
		CommandPieLabelsLegendPositionTopCaption,
		CommandPieLabelsLegendPositionTopDescription,
		CommandPieLabelsLegendPositionBottomCaption,
		CommandPieLabelsLegendPositionBottomDescription,
		CommandPieLabelsLegendPositionTopRightCaption,
		CommandPieLabelsLegendPositionTopRightDescription,
		CommandPieShowCaptionsCaption,
		CommandPieShowCaptionsDescription,
		CommandGaugeStyleFullCircularCaption,
		CommandGaugeStyleFullCircularDescription,
		CommandGaugeStyleHalfCircularCaption,
		CommandGaugeStyleHalfCircularDescription,
		CommandGaugeStyleLeftQuarterCircularCaption,
		CommandGaugeStyleLeftQuarterCircularDescription,
		CommandGaugeStyleRightQuarterCircularCaption,
		CommandGaugeStyleRightQuarterCircularDescription,
		CommandGaugeStyleThreeFourthCircularCaption,
		CommandGaugeStyleThreeFourthCircularDescription,
		CommandGaugeStyleLinearHorizontalCaption,
		CommandGaugeStyleLinearHorizontalDescription,
		CommandGaugeStyleLinearVerticalCaption,
		CommandGaugeStyleLinearVerticalDescription,
		CommandGaugeShowCaptionsCaption,
		CommandGaugeShowCaptionsDescription,
		CommandImageLoadCaption,
		CommandImageLoadDescription,
		CommandImageImportCaption,
		CommandImageImportDescription,
		CommandImageSizeMode,
		CommandImageSizeModeClipCaption,
		CommandImageSizeModeClipDescription,
		CommandImageSizeModeStretchCaption,
		CommandImageSizeModeStretchDescription,
		CommandImageSizeModeSqueezeCaption,
		CommandImageSizeModeSqueezeDescription,
		CommandImageSizeModeZoomCaption,
		CommandImageSizeModeZoomDescription,
		CommandImageAlignmentCaption,
		CommandImageAlignmentTopLeftCaption,
		CommandImageAlignmentTopLeftDescription,
		CommandImageAlignmentCenterLeftCaption,
		CommandImageAlignmentCenterLeftDescription,
		CommandImageAlignmentBottomLeftCaption,
		CommandImageAlignmentBottomLeftDescription,
		CommandImageAlignmentTopCenterCaption,
		CommandImageAlignmentTopCenterDescription,
		CommandImageAlignmentCenterCenterCaption,
		CommandImageAlignmentCenterCenterDescription,
		CommandImageAlignmentBottomCenterCaption,
		CommandImageAlignmentBottomCenterDescription,
		CommandImageAlignmentTopRightCaption,
		CommandImageAlignmentTopRightDescription,
		CommandImageAlignmentCenterRightCaption,
		CommandImageAlignmentCenterRightDescription,
		CommandImageAlignmentBottomRightCaption,
		CommandImageAlignmentBottomRightDescription,
		CommandMapDefaultShapefileCaption,
		CommandMapDefaultShapefileDescription,
		CommandMapLoadCaption,
		CommandMapLoadDescription,
		CommandMapImportCaption,
		CommandMapImportDescription,
		CommandMapWorldCountriesCaption,
		CommandMapWorldCountriesDescription,
		CommandMapEuropeCaption,
		CommandMapEuropeDescription,
		CommandMapAsiaCaption,
		CommandMapAsiaDescription,
		CommandMapNorthAmericaCaption,
		CommandMapNorthAmericaDescription,
		CommandMapSouthAmericaCaption,
		CommandMapSouthAmericaDescription,
		CommandMapAfricaCaption,
		CommandMapAfricaDescription,
		CommandMapUSACaption,
		CommandMapUSADescription,
		CommandMapCanadaCaption,
		CommandMapCanadaDescription,
		CommandMapShowLegendCaption,
		CommandMapShowLegendDescription,
		CommandMapLegendPositionCaption,
		CommandMapLegendPositionDescription,
		CommandMapFullExtentCaption,
		CommandMapFullExtentDescription,
		CommandMapInitialExtentCaption,
		CommandMapInitialExtentDescription,
		CommandMapShapeTitleAttributeCaption,
		CommandMapShapeTitleAttributeDescription,
		CommandChoroplethMapShapeLabelsAttributeCaption,
		CommandChoroplethMapShapeLabelsAttributeDescription,
		CommandWeightedLegendLinearTypeCaption,
		CommandWeightedLegendLinearTypeDescription,
		CommandWeightedLegendNestedTypeCaption,
		CommandWeightedLegendNestedTypeDescription,
		CommandWeightedLegendNoneTypeCaption,
		CommandWeightedLegendNoneTypeDescription,
		CommandChangeWeightedLegendTypeCaption,
		CommandChangeWeightedLegendTypeDescription,
		CommandGeoPointMapClusterizationCaption,
		CommandGeoPointMapClusterizationDescription,
		CommandPieMapIsWeightedCaption,
		CommandPieMapIsWeightedDescription,
		CommandMapLockNavigationCaption,
		CommandMapLockNavigationDescription,
		CommandTextBoxEditTextCaption,
		CommandTextBoxEditTextDescription,
		CommandRangeFilterSeriesTypeLineCaption,
		CommandRangeFilterSeriesTypeLineDescription,
		CommandRangeFilterSeriesTypeStackedLineCaption,
		CommandRangeFilterSeriesTypeStackedLineDescription,
		CommandRangeFilterSeriesTypeFullStackedLineCaption,
		CommandRangeFilterSeriesTypeFullStackedLineDescription,
		CommandRangeFilterSeriesTypeAreaCaption,
		CommandRangeFilterSeriesTypeAreaDescription,
		CommandRangeFilterSeriesTypeStackedAreaCaption,
		CommandRangeFilterSeriesTypeStackedAreaDescription,
		CommandRangeFilterSeriesTypeFullStackedAreaCaption,
		CommandRangeFilterSeriesTypeFullStackedAreaDescription,
		CommandDimensionSortAscending,
		CommandDimensionSortDescending,
		CommandDimensionSortNone,
		CommandDimensionSortModeDisplayText,
		CommandDimensionSortModeValue,
		CommandDimensionSortModeID,
		CommandDimensionSortModeKey,
		CommandDimensionTopN,
		CommandNumericFormat,
		CommandDataItemRename,
		CommandFormatRuleManager,
		CommandFormatRuleClearDataItem,
		CommandFormatRuleValue,
		CommandFormatRuleTopBottom,
		CommandFormatRuleAboveBelowAverage,
		CommandFormatRuleRange,
		CommandFormatRuleTopN,
		CommandFormatRuleBottomN,
		CommandFormatRuleAboveAverage,
		CommandFormatRuleAboveOrEqualAverage,
		CommandFormatRuleBelowAverage,
		CommandFormatRuleBelowOrEqualAverage,
		CommandFormatRuleGreaterThan,
		CommandFormatRuleGreaterThanOrEqualTo,
		CommandFormatRuleLessThanOrEqualTo,
		CommandFormatRuleLessThan,
		CommandFormatRuleBetween,
		CommandFormatRuleNotBetween,
		CommandFormatRuleBetweenOrEqual,
		CommandFormatRuleNotBetweenOrEqual,
		CommandFormatRuleEqualTo,
		CommandFormatRuleNotEqualTo,
		CommandFormatRuleExpression,
		CommandFormatRuleContains,
		CommandFormatRuleDateOccurring,
		CommandFormatRuleRangeSet,
		CommandFormatRuleColorRangeBar,
		CommandFormatRuleRangeIcons,
		CommandFormatRuleRangeColors,
		CommandFormatRuleBarRangeColors,
		CommandFormatRuleRangeGradient,
		CommandFormatRuleGradientRangeBar,
		CommandFormatRuleBar,
		CommandFormatRuleTopNDescription,
		CommandFormatRuleBottomNDescription,
		CommandFormatRuleAboveAverageDescription,
		CommandFormatRuleAboveOrEqualAverageDescription,
		CommandFormatRuleBelowAverageDescription,
		CommandFormatRuleBelowOrEqualAverageDescription,
		CommandFormatRuleGreaterThanDescription,
		CommandFormatRuleGreaterThanOrEqualToDescription,
		CommandFormatRuleLessThanOrEqualToDescription,
		CommandFormatRuleLessThanDescription,
		CommandFormatRuleBetweenDescription,
		CommandFormatRuleNotBetweenDescription,
		CommandFormatRuleBetweenOrEqualDescription,
		CommandFormatRuleNotBetweenOrEqualDescription,
		CommandFormatRuleEqualToDescription,
		CommandFormatRuleNotEqualToDescription,
		CommandFormatRuleExpressionDescription,
		CommandFormatRuleContainsDescription,
		CommandFormatRuleDateOccurringDescription,
		CommandFormatRuleRangeSetDescription,
		CommandFormatRuleColorRangeBarDescription,
		CommandFormatRuleRangeGradientDescription,
		CommandFormatRuleGradientRangeBarDescription,
		CommandFormatRuleBarDescription,
		CommandPivotInitialStateCaption,
		CommandPivotInitialStateDescription,
		CommandPivotAutoExpandColumnCaption,
		CommandPivotAutoExpandColumnDescription,
		CommandPivotAutoExpandRowCaption,
		CommandPivotAutoExpandRowDescription,
		CommandPivotShowGrandTotalsCaption,
		CommandPivotShowGrandTotalsDescription,
		CommandPivotShowColumnGrandTotalsCaption,
		CommandPivotShowColumnGrandTotalsDescription,
		CommandPivotShowRowGrandTotalsCaption,
		CommandPivotShowRowGrandTotalsDescription,
		CommandPivotShowTotalsCaption,
		CommandPivotShowTotalsDescription,
		CommandPivotShowColumnTotalsCaption,
		CommandPivotShowColumnTotalsDescription,
		CommandPivotShowRowTotalsCaption,
		CommandPivotShowRowTotalsDescription,
		CommandComboBoxStandardTypeCaption,
		CommandComboBoxStandardTypeDescription,
		CommandComboBoxCheckedTypeCaption,
		CommandComboBoxCheckedTypeDescription,
		CommandListBoxRadioTypeCaption,
		CommandListBoxRadioTypeDescription,
		CommandListBoxCheckedTypeCaption,
		CommandListBoxCheckedTypeDescription,
		CommandTreeViewAutoExpandCaption,
		CommandTreeViewAutoExpandDescription,
		CommandFilterElementShowAllValueCaption,
		CommandFilterElementShowAllValueDescription,
		CommandEditEFDataSourceCaption,
		CommandEditEFDataSourceDescription,
		CommandEditConnectionCaption,
		CommandEditConnectionDescription,
		CommandEditObjectDataSourceCaption,
		CommandEditObjectDataSourceDescription,
		CommandEditExcelDataSourceCaption,
		CommandEditExcelDataSourceDescription,
		DesignerActionListCreateRibbonTransaction,
		DesignerActionListUpdateRibbonTransaction,
		DesignerActionListCreateBarsTransaction,
		DesignerActionListUpdateBarsTransaction,
		DesignerActionMethodCreateRibbonItem,
		DesignerActionMethodUpdateRibbonItem,
		DesignerActionMethodCreateBarsItem,
		DesignerActionMethodUpdateBarsItem,
		HistoryItemEnabling,
		HistoryItemDisabling,
		HistoryItemNewDataSource,
		HistoryItemEditDataSource,
		HistoryItemEditConnection,
		HistoryItemServerMode,
		HistoryItemDeleteDataSource,
		HistoryItemRenameDataSource,
		HistoryItemInsertItem,
		HistoryItemInsertGroup,
		HistoryItemDuplicateItem,
		HistoryItemConvertDashboardItemType,
		HistoryItemDeleteItem,
		HistoryItemSetCurrencyCulture,
		HistoryItemEditDashboardColorScheme,
		HistoryItemEditLocalColorScheme,
		HistoryItemEditDashboardTitle,
		HistoryItemEditNames,
		HistoryItemLayoutChanged,
		HistoryItemShowCaption,
		HistoryItemEditFilter,
		HistoryItemSingleMasterFilter,
		HistoryItemMultipleMasterFilter,
		HistoryItemDrillDown,
		HistoryItemTargetDimensions,
		HistoryItemCrossDataSourceFiltering,
		HistoryItemIgnoreMasterFilters,
		HistoryItemGroupIgnoreMasterFilters,
		HistoryItemGroupMasterFilter,
		HistoryItemContentArrangement,
		HistoryItemGridHorizontalLines,
		HistoryItemEnableGridColumnFitToContent,
		HistoryItemDisableGridColumnFitToContent,
		HistoryItemEnableGridColumnFixedWidth,
		HistoryItemDisableGridColumnFixedWidth,
		HistoryItemGridResizeColumns,
		HistoryItemGridAutoFitToContentsColumnWidthMode,
		HistoryItemGridAutoFitToGridColumnWidthMode,
		HistoryItemGridManualColumnWidthMode,
		HistoryItemGridWordWrap,
		HistoryItemGridVerticalLines,
		HistoryItemGridMergeCells,
		HistoryItemGridBandedRows,
		HistoryItemGridColumnHeaders,
		HistoryItemGridColumnOptions,
		HistoryItemChartSeriesType,
		HistoryItemChartRotate,
		HistoryItemChartShowLegend,
		HistoryItemChartYAxisSettings,
		HistoryItemChartXAxisSettings,
		HistoryItemScatterChartPointLabelOptions,
		HistoryItemChartLegendPosition,
		HistoryItemChartAddPane,
		HistoryItemChartRemovePane,
		HistoryItemPieStyle,
		HistoryItemPieLabelContentType,
		HistoryItemPieTooltipContentType,
		HistoryItemPieLegendPosition,
		HistoryItemPieShowCaptions,
		HistoryItemGaugeViewType,
		HistoryItemGaugeRange,
		HistoryItemGaugeShowCaptions,
		HistoryItemImageOpen,
		HistoryItemImageSizeMode,
		HistoryItemImageAlignment,
		HistoryItemTextBoxEditText,
		HistoryItemRangeFilterSeriesType,
		HistoryItemMapOpen,
		HistoryItemMapAttribute,
		HistoryItemMapShapeTitleAttribute,
		HistoryItemMapDelta,
		HistoryItemChangeWeightedLegendType,
		HistoryItemDisableWeightedLegend,
		HistoryItemMapShowLegend,
		HistoryItemGeoPointMapClusterization,
		HistoryItemPieMapIsWeighted,
		HistoryItemMapLockNavigation,
		HistoryItemMapUnLockNavigation,
		HistoryItemBubbleMapOptions,
		HistoryItemRestoreMapViewport,
		HistoryItemChangeMapViewport,
		HistoryItemModifyBindings,
		HistoryItemMeasureSummaryType,
		HistoryItemDimensionSortOrder,
		HistoryItemDimensionSorting,
		HistoryItemDimensionSpecifyTopN,
		HistoryItemDataItemNumericFormat,
		HistoryItemDataItemDateTimeFormat,
		HistoryItemDataItemRename,
		HistoryItemDimensionGroupInterval,
		HistoryItemDimensionColoringMode,
		HistoryItemMeasureColoringMode,
		HistoryItemUseGlobalColorsEnable,
		HistoryItemUseGlobalColorsDisable,
		HistoryItemPivotAutoExpandColumnEnable,
		HistoryItemPivotAutoExpandColumnDisable,
		HistoryItemPivotAutoExpandRowEnable,
		HistoryItemPivotAutoExpandRowDisable,
		HistoryItemPivotShowColumnGrandTotalsEnable,
		HistoryItemPivotShowColumnGrandTotalsDisable,
		HistoryItemPivotShowRowGrandTotalsEnable,
		HistoryItemPivotShowRowGrandTotalsDisable,
		HistoryItemPivotShowColumnTotalsEnable,
		HistoryItemPivotShowColumnTotalsDisable,
		HistoryItemPivotShowRowTotalsEnable,
		HistoryItemPivotShowRowTotalsDisable,
		HistoryItemComboBoxChangeType,
		HistoryItemListBoxChangeType,
		HistoryItemTreeViewAutoExpandChanged,
		HistoryItemFormatRuleCreate,
		HistoryItemFormatRuleEdit,
		HistoryItemFormatRuleClear,
		HistoryItemFormatRulesMove,
		HistoryItemFormatRulesDelete,
		HistoryItemFormatRulePropertyEnabled,
		HistoryItemAddQuery,
		HistoryItemEditQuery,
		HistoryItemEditQueryFilter,
		HistoryItemRenameQuery,
		HistoryItemDeleteQuery,
		HistoryItemAddGridColumnTotal,
		HistoryItemClearGridColumnTotals,
		HistoryItemRemoveGridColumnTotal,
		HistoryItemChangeGridColumnTotal,
		FormatRuleDataItemsAll,
		UndoText,
		RedoText,
		UndoRedoSingleAction,
		UndoRedoActionsCount,
		ImageWrongFormatMessage,
		ImageFileCorruptedMessage,
		ImageNotFoundMessage,
		ImageGeneralExceptionMessage,
		ImageCorruptedMessage,
		DataSourceNameExistsMessage,
		DataSourceEmptyNameMessage,
		RecentDashboardsCaption,
		RecentItemsMenuEmptyText,
		RecentBarButtonItemOpenCommand,
		RecentBarButtonItemPinCommand,
		RecentBarButtonItemUnpinCommand,
		RecentBarButtonItemRemoveCommand,
		RecentBarButtonItemClearFilesCommand,
		RecentBarButtonItemClearFoldersCommand,
		DeltaValueTypeActualValueCaption,
		DeltaValueTypeAbsoluteVariationCaption,
		DeltaValueTypePercentVariationCaption,
		DeltaValueTypePercentOfTargetCaption,
		DeltaIndicationModeGreaterIsGoodCaption,
		DeltaIndicationModeLessIsGoodCaption,
		DeltaIndicationModeWarningIfGreaterCaption,
		DeltaIndicationModeWarningIfLessCaption,
		DeltaIndicationModeNoIndicationCaption,
		DeltaThresholdTypeAbsolute,
		DeltaThresholdTypePercent,
		NumericFormatUnitAutoCaption,
		NumericFormatUnitOnesCaption,
		NumericFormatUnitThousandsCaption,
		NumericFormatUnitMillionsCaption,
		NumericFormatUnitBillionsCaption,
		NumericFormatFormatTypeAutoCaption,
		NumericFormatFormatTypeGeneralCaption,
		NumericFormatFormatTypeNumberCaption,
		NumericFormatFormatTypeCurrencyCaption,
		NumericFormatFormatTypeScientificCaption,
		NumericFormatFormatTypePercentCaption,
		CurrencyInvariantRegion,
		DashboardCurrencyUseCurrentCurrency,
		DataItemCurrencyUseDashboardCurrency,
		DateTimeFormatYearFormatDefaultCaption,
		DateTimeFormatYearFormatFullCaption,
		DateTimeFormatYearFormatAbbreviatedCaption,
		DateTimeFormatQuarterFormatDefaultCaption,
		DateTimeFormatQuarterFormatNumericCaption,
		DateTimeFormatQuarterFormatFullCaption,
		DateTimeFormatMonthFormatDefaultCaption,
		DateTimeFormatMonthFormatFullCaption,
		DateTimeFormatMonthFormatAbbreviatedCaption,
		DateTimeFormatMonthFormatNumericCaption,
		DateTimeFormatDayOfWeekFormatDefaultCaption,
		DateTimeFormatDayOfWeekFormatFullCaption,
		DateTimeFormatDayOfWeekFormatAbbreviatedCaption,
		DateTimeFormatDayOfWeekFormatNumericCaption,
		DateTimeFormatDateFormatDefaultCaption,
		DateTimeFormatDateFormatShortCaption,
		DateTimeFormatDateFormatLongCaption,
		DateTimeFormatDateTimeFormatDefaultCaption,
		DateTimeFormatDateTimeFormatShortCaption,
		DateTimeFormatDateTimeFormatLongCaption,
		DateTimeFormatDateTimeFormatTimeOnlyCaption,
		ExactDateFormatYear,
		ExactDateFormatQuarter,
		ExactDateFormatMonth,
		ExactDateFormatDay,
		ExactDateFormatHour,
		ExactDateFormatMinute,
		ExactDateFormatSecond,
		ExactDateDateTimeFormatPattern,
		ChartCannotRenderData,
		ConnectionErrorFormDetailPattern,
		CommandPrintPreview,
		CommandExportTo,
		CommandExportToPdf,
		CommandExportToImage,
		CommandExportToExcel,
		CommandDrillUp,
		CommandClearMasterFilter,
		CommandClearSelection,
		CommandSelectOtherValues,
		ChartInsideHorizontalGalleryGroupCaption,
		ChartInsideVerticalGalleryGroupCaption,
		ChartOutsideHorizontalGalleryGroupCaption,
		ChartOutsideVerticalGalleryGroupCaption,
		ChartTopLeftHorizontalInsideLegendPosition,
		ChartTopCenterHorizontalInsideLegendPosition,
		ChartTopRightHorizontalInsideLegendPosition,
		ChartBottomLeftHorizontalInsideLegendPosition,
		ChartBottomCenterHorizontalInsideLegendPosition,
		ChartBottomRightHorizontalInsideLegendPosition,
		ChartTopLeftVerticalInsideLegendPosition,
		ChartTopCenterVerticalInsideLegendPosition,
		ChartTopRightVerticalInsideLegendPosition,
		ChartBottomLeftVerticalInsideLegendPosition,
		ChartBottomCenterVerticalInsideLegendPosition,
		ChartBottomRightVerticalInsideLegendPosition,
		ChartTopLeftHorizontalOutsideLegendPosition,
		ChartTopCenterHorizontalOutsideLegendPosition,
		ChartTopRightHorizontalOutsideLegendPosition,
		ChartBottomLeftHorizontalOutsideLegendPosition,
		ChartBottomCenterHorizontalOutsideLegendPosition,
		ChartBottomRightHorizontalOutsideLegendPosition,
		ChartTopLeftVerticalOutsideLegendPosition,
		ChartTopRightVerticalOutsideLegendPosition,
		ChartBottomLeftVerticalOutsideLegendPosition,
		ChartBottomRightVerticalOutsideLegendPosition,
		ChartPrimaryAxisTypeName,
		ChartSecondaryAxisTypeName,
		DataFieldBrowserRemoveCalculatedFieldMenuItem,
		DataFieldBrowserRenameCalculatedFieldMenuItem,
		DataFieldBrowserCalculatedFieldTypeMenuItem,
		DataFieldBrowserCalculatedFieldExpressionEditorMenuItem,
		HistoryItemAddCalculatedField,
		HistoryItemRemoveCalculatedField,
		HistoryItemCalculatedFieldName,
		HistoryItemCalculatedFieldExpression,
		HistoryItemCalculatedFieldType,
		CommandAddCalculatedFieldCaption,
		CommandAddCalculatedFieldDescription,
		CommandEditDataSourceFilterCaption,
		CommandEditDataSourceFilterDescription,
		CommandClearDataSourceFilterCaption,
		CommandClearDataSourceFilterDescription,
		CommandDashboardParametersCaption,
		CommandDashboardParametersDescription,
		CommandEditDashboardParametersCaption,
		CommandEditDashboardParametersDescription,
		HistoryItemEditDashboardParameters,
		HistoryItemFilterDataSource,
		BarDataSourceName,
		RibbonGroupDataSource,
		RibbonGroupFiltering,
		BarViewName,
		RibbonGroupView,
		RibbonGroupDesign,
		RibbonGroupHome,
		RibbonGroupSqlDataSourceQuery,
		BarHomeName,
		CommandEditNamesRibbonCaption,
		CommandEditFilterRibbonCaption,
		CommandImageLoadRibbonCaption,
		CommandImageImportRibbonCaption,
		CommandTextBoxEditTextRibbonCaption,
		CommandMapLoadRibbonCaption,
		CommandMapImportRibbonCaption,
		SparklineIndicationModeArea,
		SparklineIndicationModeLine,
		SparklineIndicationModeBar,
		SparklineIndicationModeWinLoss,
		RibbonGroupCommonCaption,
		RibbonGroupColoringCaption,
		CommandSelectDashboardCaption,
		CommandSelectDashboardDescription,
		CommandSelectDashboardItemGroupCaption,
		CommandSelectDashboardItemGroupDescription,
		WeightedLegendGalleryGroupCaption,
		MapGalleryVerticalGroupCaption,
		MapGalleryHorizontalGroupCaption,
		MapTopLeftWeightedLegendPosition,
		MapTopCenterWeightedLegendPosition,
		MapTopRightWeightedLegendPosition,
		MapBottomLeftWeightedLegendPosition,
		MapBottomCenterWeightedLegendPosition,
		MapBottomRightWeightedLegendPosition,
		MapTopLeftVerticalLegendPosition,
		MapTopCenterVerticalLegendPosition,
		MapTopRightVerticalLegendPosition,
		MapBottomLeftVerticalLegendPosition,
		MapBottomCenterVerticalLegendPosition,
		MapBottomRightVerticalLegendPosition,
		MapTopLeftHorizontalLegendPosition,
		MapTopCenterHorizontalLegendPosition,
		MapTopRightHorizontalLegendPosition,
		MapBottomLeftHorizontalLegendPosition,
		MapBottomCenterHorizontalLegendPosition,
		MapBottomRightHorizontalLegendPosition,
		MapLayerOptionsResetCustomScale,
		HistoryItemMapLayerOptions,
		PropertyGridParametersLookUpStaticList,
		PropertyGridParametersLookUpDynamicList,
		PropertyGridNoDataSource,
		PropertyGridNoDataMember,
		PropertyGridNoDashboardItemGroup,
		PrintPreviewOptionsFormCaption,
		MapShapeNoneAttribute,
		LocalColorSchemeEditFormCaption,
		GlobalColorSchemeEditFormCaption,
		ColorPickerPaletteColorsSectionCaption,
		ColorSchemeGridEmptyText,
		EditorEmptyEnter,
		EditorAutomaticValue,
		EditorCustomValue,
		FormatConditionRangeSetNoStyleCaption,
		FormatConditionTopBottomNCaption,
		FormatConditionBetweenAndCaption,
		FormatConditionAutomaticValueType,
		FormatConditionNumberValueType,
		FormatConditionPercentValueType,
		FormatConditionMinMaxValueCaption,
		FormatConditionMinimumCaption,
		FormatConditionMaximumCaption,
		FormatRuleInvalidRule,
		FormatRuleInvalidRuleChanging,
		FormatRuleDataItemIsNotAvailable,
		FormatRuleApplyTo,
		FormatRuleApplyToRow,
		FormatRuleApplyToColumn,
		FormatRulePercentOf,
		FormatRuleFormatStyle,
		FormatRuleStyleAppearance,
		FormatRuleStyle,
		FormatRuleNegativeStyle,
		FormatRuleStyleIcons,
		FormatRuleRangeAdd,
		FormatRuleRangeDelete,
		FormatRuleRangeReverseStyles,
		FormatRuleRangeUsePercent,
		FormatRuleAllowNegativeAxis,
		FormatRuleDrawAxis,
		FormatRuleShowBarOnly,
		FormatRuleRangeGenerateRanges,
		FormatRuleRangeSegmentNumber,
		IntersectionLevelRowCaption,
		IntersectionLevelColumnCaption,
		IntersectionLevelModeCaption,
		IntersectionLevelModeAuto,
		IntersectionLevelModeFirst,
		IntersectionLevelModeLast,
		IntersectionLevelModeAll,
		IntersectionLevelModeSpecific,
		RangeSetDescription,
		RangeSetIconSigns,
		RangeSetIconStars,
		RangeSetIconQuarters,
		RangeSetIconBars,
		RangeSetIconBoxes,
		RangeSetIconFlags,
		RangeSetIconArrowsColored,
		RangeSetIconArrowsGray,
		RangeSetIconTriangles,
		RangeSetIconCircles,
		RangeSetIconCirclesRedToBlack,
		RangeSetIconTrafficLights,
		RangeSetIconSymbolsCircled,
		RangeSetIconSymbolsUncircled,
		RangeSetIconPositiveNegativeTriangles,
		RangeSetColorsPaleRedGreen,
		RangeSetColorsPaleRedGreenBlue,
		RangeSetColorsPaleRedYellowGreenBlue,
		RangeSetColorsPaleRedOrangeYellowGreenBlue,
		RangeSetColorsRedGreen,
		RangeSetColorsRedGreenBlue,
		RangeSetColorsRedYellowGreenBlue,
		RangeSetColorsRedOrangeYellowGreenBlue,
		RangeGradientDescription,
		RangeGradientGreenWhite,
		RangeGradientWhiteGreen,
		RangeGradientRedWhite,
		RangeGradientWhiteRed,
		RangeGradientYellowGreen,
		RangeGradientGreenYellow,
		RangeGradientYellowRed,
		RangeGradientRedYellow,
		RangeGradientBlueWhite,
		RangeGradientWhiteBlue,
		RangeGradientBlueRed,
		RangeGradientRedBlue,
		RangeGradientYellowBlue,
		RangeGradientBlueYellow,
		RangeGradientGreenBlue,
		RangeGradientBlueGreen,
		RangeGradientGreenWhiteBlue,
		RangeGradientBlueWhiteGreen,
		RangeGradientBlueWhiteRed,
		RangeGradientRedWhiteBlue,
		RangeGradientGreenWhiteRed,
		RangeGradientRedWhiteGreen,
		RangeGradientGreenYellowRed,
		RangeGradientRedYellowGreen,
		RangeGradientBlueYellowRed,
		RangeGradientRedYellowBlue,
		RangeGradientGreenYellowBlue,
		RangeGradientBlueYellowGreen,
		FormatConditionCustomAppearanceCaption,
		FormatConditionAppearanceNone,
		FormatConditionAppearanceCustom,
		FormatConditionAppearancePaleRed,
		FormatConditionAppearancePaleYellow,
		FormatConditionAppearancePaleGreen,
		FormatConditionAppearancePaleBlue,
		FormatConditionAppearancePalePurple,
		FormatConditionAppearancePaleCyan,
		FormatConditionAppearancePaleOrange,
		FormatConditionAppearancePaleGray,
		FormatConditionAppearanceRed,
		FormatConditionAppearanceYellow,
		FormatConditionAppearanceGreen,
		FormatConditionAppearanceBlue,
		FormatConditionAppearancePurple,
		FormatConditionAppearanceCyan,
		FormatConditionAppearanceOrange,
		FormatConditionAppearanceGray,
		FormatConditionAppearanceGradientRed,
		FormatConditionAppearanceGradientYellow,
		FormatConditionAppearanceGradientGreen,
		FormatConditionAppearanceGradientBlue,
		FormatConditionAppearanceGradientPurple,
		FormatConditionAppearanceGradientCyan,
		FormatConditionAppearanceGradientOrange,
		FormatConditionAppearanceGradientTransparent,
		FormatConditionAppearanceFontBold,
		FormatConditionAppearanceFontItalic,
		FormatConditionAppearanceFontUnderline,
		FormatConditionAppearanceFontGrayed,
		FormatConditionAppearanceFontRed,
		FormatConditionAppearanceFontYellow,
		FormatConditionAppearanceFontGreen,
		FormatConditionAppearanceFontBlue,
		FormatConditionTopBottomTemplate,
		FormatConditionRangeSetTemplate,
		FormatConditionBarTemplate,
		FormatConditionColorRangeBarTemplate,
		FormatConditionGradientRangeBarTemplate,
		FormatConditionRangeGradientTemplate,
		FormatConditionUndefinedConditionException,
		DashboardFormatConditionPercentValue,
		DashboardFormatConditionTemplate,
		DashboardFormatConditionBetweenTemplate,
		DashboardFormatConditionBetweenOrEqualTemplate,
		FormatConditionRangeSetRanges2Caption,
		FormatConditionRangeSetRanges3Caption,
		FormatConditionRangeSetRanges4Caption,
		FormatConditionRangeSetRanges5Caption,
		FormatConditionRangeGradientTwoColorsCaption,
		FormatConditionRangeGradientThreeColorsCaption,
		ConfigureOlapParametersPageViewHeaderDescription,
		DataSourceTypeSql,
		DataSourceTypeSqlDescription,
		DataSourceTypeOlap,
		DataSourceTypeOlapDescription,
		DataSourceTypeEF,
		DataSourceTypeEFDescription,
		DataSourceTypeObject,
		DataSourceTypeObjectDescription,
		DataSourceTypeXmlSchema,
		DataSourceTypeXmlSchemaDescription,
		DataSourceTypeExcel,
		DataSourceTypeExcelDescription,
		DataSourceQuery,
		DataSourceDataMember,
		AddDashboardParameter,
		ParameterFormCaption,
		RangeEditorControlBetweenValidateMessage,
		RangeEditorControlGreaterValidateMessage,
		RangeEditorControlLessValidateMessage,
		CommandAddQueryCaption,
		CommandAddQueryDescription,
		CommandDeleteQueryCaption,
		CommandDeleteQueryDescription,
		CommandEditQueryFilterDescription,
		CommandEditQueryFilterCaption,
		CommandRenameQueryCaption,
		CommandRenameQueryDescription,
		CommandEditQueryCaption,
		CommandEditQueryDescription,
		RenameQueryFormCaption,
		RenameDataSourceFormCaption,
		AutomaticUpdatesCaption,
		AutomaticUpdatesDescription,
		UpdateDataCaption,
		UpdateDataDescription,
	}
	public class DashboardWinLocalizer : XtraLocalizer<DashboardWinStringId> {
		static DashboardWinLocalizer() {
			if (GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DashboardWinStringId>(new DashboardWinResLocalizer()));
		}
		public static new XtraLocalizer<DashboardWinStringId> Active {
			get { return XtraLocalizer<DashboardWinStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<DashboardWinStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DashboardWinStringId>(value));
					RaiseActiveChanged();
				} else
					XtraLocalizer<DashboardWinStringId>.Active = value;
			}
		}
		public static string GetString(DashboardWinStringId id) {
			return Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			AddString(DashboardWinStringId.MenuFormatRulesAdd, "Add Format Rule");
			AddString(DashboardWinStringId.MenuDimensionSortBy, "Sort by");
			AddString(DashboardWinStringId.MenuMore, "More");
			AddString(DashboardWinStringId.MenuColorBy, "Color by");
			AddString(DashboardWinStringId.MenuDateTimeFormatFormat, "Format");
			AddString(DashboardWinStringId.FileFilterAll, "All Files");
			AddString(DashboardWinStringId.FileFilterDashboards, "Dashboard Files");
			AddString(DashboardWinStringId.FileFilterAllImages, "All Image Files");
			AddString(DashboardWinStringId.TooltipGroupByType, "Group Fields by Type");
			AddString(DashboardWinStringId.TooltipSortAscending, "Sort Fields A to Z");
			AddString(DashboardWinStringId.TooltipSortDescending, "Sort Fields Z to A");
			AddString(DashboardWinStringId.TooltipRefreshFieldList, "Refresh Field List");
			AddString(DashboardWinStringId.MessageBoxWarningTitle, "Warning");
			AddString(DashboardWinStringId.MessageBoxConfirmationTitle, "Confirmation");
			AddString(DashboardWinStringId.MessageBoxErrorTitle, "Error");
			AddString(DashboardWinStringId.MessageNullDashboard, "The Dashboard property cannot be set to null.");
			AddString(DashboardWinStringId.MessageExternalDashboardItem, "The dashboard does not contain the {0} dashboard item.");
			AddString(DashboardWinStringId.DashboardDesignerCaption, "Dashboard Designer");
			AddString(DashboardWinStringId.DashboardDesignerDefaultDashboardName, "Dashboard");
			AddString(DashboardWinStringId.DashboardDesignerEmptyMessage, "To add a dashboard item to your dashboard, click the corresponding button in the Ribbon or the Toolbar");
			AddString(DashboardWinStringId.DashboardDesignerUnableCreateRibbonMessage, "Unable to create a ribbon. The Dashboard Designer has not been assigned a parent control");
			AddString(DashboardWinStringId.DashboardDesignerUnableCreateBarsMessage, "Unable to create bars. The Dashboard Designer has not been assigned a parent control");
			AddString(DashboardWinStringId.DashboardDesignerConfirmSaveMessage, "Do you want to save changes to {0}?");
			AddString(DashboardWinStringId.DashboardDesignerDataReducedMessage, "This dashboard item is bound to a large dataset and\r\ndoes not currently display all data to retain performance.\r\nIn the Dashboard Viewer, all data will be displayed.");
			AddString(DashboardWinStringId.DashboardDesignerDataObsoleteMessage, "The dashboard item does not reflect changes made since the last dashboard update. Use the Update button to update\r\nthe dashboard according to the last modifications or enable Automatic Updates.");
			AddString(DashboardWinStringId.DragAreaDashboardItemHasNotDataItemsMessage, "The '{0}' dashboard item has no data items");
			AddString(DashboardWinStringId.DragAreaHasNotDashboardItemMessage, "Select a dashboard item to see available data items");
			AddString(DashboardWinStringId.MessageFileNotFound, "This file could not be found.\r\n({0})");
			AddString(DashboardWinStringId.MessageConfirmUnpinnedItemsRemove, "Are you sure you want to remove all unpinned items from the list?");
			AddString(DashboardWinStringId.MessageCollectionTypesNotSupported, "Dashboard does not support collection types.");
			AddString(DashboardWinStringId.MessageInteractivityDashboardItemNotFound, "Cannot find a dashboard item with the provided name. Please specify a correct name.");
			AddString(DashboardWinStringId.MessageInteractivityDataDashboardItemRequired, "Action is called for an unsupported dashboard item. Interactivity actions can be performed for data-aware dashboard items only.");
			AddString(DashboardWinStringId.MessageInteractivityRangeFilterRequired, "Action is called for an unsupported dashboard item. This action can be performed only for Range Filter.");
			AddString(DashboardWinStringId.MessageInteractivityOperationNotAvailable, "The required action is not currently available for this dashboard item.");
			AddString(DashboardWinStringId.MessageEmptyDrillDownValue, "An empty or null value has been passed to the PerformDrillDown method.");
			AddString(DashboardWinStringId.MessageServerFailedCalculatedFields, "The following calculated fields cannot be evaluated in server mode:");
			AddString(DashboardWinStringId.MessageDataProcessingModeChanging, "This operation can take a significant amount of time. Do you want to continue?");
			AddString(DashboardWinStringId.MessageSummaryCalculatedFieldInDimension, "Cannot change the expression of the '{0}' calculated field that provides data for the '{1}' dimension. Summary-based calculated fields can provide data for measures only.");
			AddString(DashboardWinStringId.DashboardViewerEmptyDesignerMessage, "This Dashboard Viewer is empty. To provide a dashboard, do one of the following:\r\n\r\n- Click the control's smart tag and choose the existing dashboard XML file as a Dashboard Source.\r\n- Create a new dashboard at design time and select this dashboard as a Dashboard Source.");
			AddString(DashboardWinStringId.DashboardLoadingError, "Unable to load the dashboard because the following file is not available or it is not a valid dashboard file.\r\n{0}\r\nPlease ensure that the application can access this file and that it is valid, or specify a different file.");
			AddString(DashboardWinStringId.DashboardSavingError, "Unable to save the dashboard.\r\nPlease specify a different path or ensure that you have permissions to write to the specified path.\r\n{0}");
			AddString(DashboardWinStringId.BarDashboardCaption, "Dashboard");
			AddString(DashboardWinStringId.RibbonPageCategoryPivotToolsName, "Pivot Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryGridToolsName, "Grid Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryChartToolsName, "Chart Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryScatterChartToolsName, "Scatter Chart Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryPiesToolsName, "Pies Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryGaugesToolsName, "Gauges Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryCardsToolsName, "Cards Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryImageToolsName, "Image Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryTextBoxToolsName, "Text Box Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryRangeFilterToolsName, "Range Filter Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryMapToolsName, "Map Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryChoroplethMapToolsName, "Choropleth Map Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryGeoPointMapToolsName, "Geo Point Map Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryBubbleMapToolsName, "Bubble Map Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryPieMapToolsName, "Pie Map Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryFilterElementToolsName, "Filter Element Tools");
			AddString(DashboardWinStringId.RibbonPageCategoryGroupToolsName, "Group Tools");
			AddString(DashboardWinStringId.RibbonPageDashboardCaption, "Dashboard");
			AddString(DashboardWinStringId.RibbonPageDataCaption, "Data");
			AddString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption, "Design");
			AddString(DashboardWinStringId.RibbonPageImageOptionsCaption, "Options");
			AddString(DashboardWinStringId.RibbonPageTextBoxFormatCaption, "Format");
			AddString(DashboardWinStringId.RibbonPageRangeFilterStyleCaption, "Style");
			AddString(DashboardWinStringId.RibbonGroupFileCaption, "File");
			AddString(DashboardWinStringId.RibbonGroupHistoryCaption, "History");
			AddString(DashboardWinStringId.RibbonGroupDataSourceCaption, "Data Source");
			AddString(DashboardWinStringId.RibbonGroupInsertCaption, "Insert");
			AddString(DashboardWinStringId.RibbonGroupItemsCaption, "Item");
			AddString(DashboardWinStringId.RibbonGroupGroupsCaption, "Group");
			AddString(DashboardWinStringId.RibbonGroupSkinsCaption, "Skins");
			AddString(DashboardWinStringId.RibbonGroupFilteringCaption, "Filtering");
			AddString(DashboardWinStringId.RibbonGroupInteractivityCaption, "Interactivity");
			AddString(DashboardWinStringId.RibbonGroupInteractivitySettingsCaption, "Interactivity settings");
			AddString(DashboardWinStringId.RibbonGroupTargetDimensions, "Target dimensions");
			AddString(DashboardWinStringId.RibbonGroupContentArrangementCaption, "Content Arrangement");
			AddString(DashboardWinStringId.RibbonGroupGridCellsCaption, "Cells");
			AddString(DashboardWinStringId.RibbonGroupGridStyleCaption, "Style");
			AddString(DashboardWinStringId.RibbonGroupGridColumnWidthModeCaption, "Column Width Mode");
			AddString(DashboardWinStringId.RibbonGroupGridLayoutCaption, "Layout");
			AddString(DashboardWinStringId.RibbonGroupChartLayoutCaption, "Diagram");
			AddString(DashboardWinStringId.RibbonGroupScatterChartLabelsCaption, "Labels");
			AddString(DashboardWinStringId.RibbonGroupChartSeriesTypeCaption, "Series Type");
			AddString(DashboardWinStringId.RibbonGroupChartLegendPositionCaption, "Legend");
			AddString(DashboardWinStringId.RibbonGroupMapLegendPositionCaption, "Color Legend");
			AddString(DashboardWinStringId.RibbonGroupWeightedLegendPositionCaption, "Weighted Legend");
			AddString(DashboardWinStringId.RibbonGroupPieStyleCaption, "Style");
			AddString(DashboardWinStringId.RibbonGroupPieLabelsCaption, "Labels");
			AddString(DashboardWinStringId.RibbonGroupGaugeStyleCaption, "Style");
			AddString(DashboardWinStringId.RibbonGroupGaugeLabelsCaption, "Labels");
			AddString(DashboardWinStringId.RibbonGroupImageOpenCaption, "Open");
			AddString(DashboardWinStringId.RibbonGroupImageSizeModeCaption, "Size Mode");
			AddString(DashboardWinStringId.RibbonGroupImageAlignmentCaption, "Alignment");
			AddString(DashboardWinStringId.RibbonGroupTextBoxSettingsCaption, "Settings");
			AddString(DashboardWinStringId.RibbonGroupRangeFilterSeriesTypeCaption, "Series Type");
			AddString(DashboardWinStringId.RibbonGroupMapCaption, "Map");
			AddString(DashboardWinStringId.RibbonGroupMapShapefileCaption, "Open");
			AddString(DashboardWinStringId.RibbonGroupGeoPointMapClusterizationCaption, "Clustering");
			AddString(DashboardWinStringId.RibbonGroupPieMapOptionsCaption, "Pie options");
			AddString(DashboardWinStringId.RibbonGroupMapNavigationCaption, "Navigation");
			AddString(DashboardWinStringId.RibbonGroupMapShapeLabelsAttributeCaption, "Labels");
			AddString(DashboardWinStringId.RibbonGroupDashboardCaption, "Dashboard");
			AddString(DashboardWinStringId.RibbonGroupPivotLayoutCaption, "Layout");
			AddString(DashboardWinStringId.RibbonGroupFilterElementTypeCaption, "Item Type");
			AddString(DashboardWinStringId.RibbonGroupTreeViewLayoutCaption, "Layout");
			AddString(DashboardWinStringId.RibbonGroupElementItemPropertiesCaption, "Item Settings");
			AddString(DashboardWinStringId.CommandUnsupportedCaption, "Unsupported Action");
			AddString(DashboardWinStringId.CommandUnsupportedDescription, "This action is not supported by the current version of the Dashboard Designer.\r\n\r\nPlease contact the application vendor.");
			AddString(DashboardWinStringId.CommandNewDashboardCaption, "New");
			AddString(DashboardWinStringId.CommandNewDashboardDescription, "Create a new dashboard.");
			AddString(DashboardWinStringId.CommandOpenDashboardCaption, "Open");
			AddString(DashboardWinStringId.CommandOpenDashboardDescription, "Open a dashboard file.");
			AddString(DashboardWinStringId.CommandSaveDashboardCaption, "Save");
			AddString(DashboardWinStringId.CommandSaveDashboardDescription, "Save the dashboard.");
			AddString(DashboardWinStringId.CommandSaveAsDashboardCaption, "Save As");
			AddString(DashboardWinStringId.CommandSaveAsDashboardDescription, "Save a copy of the dashboard.");
			AddString(DashboardWinStringId.CommandUndoCaption, "Undo");
			AddString(DashboardWinStringId.CommandUndoDescription, "Undo '{0}'");
			AddString(DashboardWinStringId.CommandRedoCaption, "Redo");
			AddString(DashboardWinStringId.CommandRedoDescription, "Redo '{0}'");
			AddString(DashboardWinStringId.CommandNewDataSourceCaption, "New Data Source");
			AddString(DashboardWinStringId.CommandNewDataSourceDescription, "Create a new data source.\r\n\r\nConfigure connection settings using the Create Data Source wizard and compose a data source in the Query Designer.");
			AddString(DashboardWinStringId.CommandEditDataSourceCaption, "Edit");
			AddString(DashboardWinStringId.CommandEditDataSourceDescription, "Edit the current data source in the Query Designer by selecting tables or individual columns to be included in the data source.");
			AddString(DashboardWinStringId.CommandServerModeCaption, "Server Mode");
			AddString(DashboardWinStringId.CommandServerModeDescription, "Enable server mode.");
			AddString(DashboardWinStringId.CommandDeleteDataSourceCaption, "Delete");
			AddString(DashboardWinStringId.CommandDeleteDataSourceDescription, "Remove the current data source.");
			AddString(DashboardWinStringId.CommandRenameDataSourceCaption, "Rename");
			AddString(DashboardWinStringId.CommandRenameDataSourceDescription, "Rename the current data source.");
			AddString(DashboardWinStringId.CommandSetCurrencyCultureCaption, "Currency");
			AddString(DashboardWinStringId.CommandSetCurrencyCultureDescription, "Specify the currency that will be used to display all currency values in the dashboard by default.");
			AddString(DashboardWinStringId.CommandDashboardTitleCaption, "Title");
			AddString(DashboardWinStringId.CommandDashboardTitleDescription, "Enter or edit text and select an image for the dashboard title.");
			AddString(DashboardWinStringId.CommandInsertPivotCaption, "Pivot");
			AddString(DashboardWinStringId.CommandInsertPivotDescription, "Create a pivot grid dashboard item and insert it into the dashboard.\r\n\r\nThe pivot grid displays a cross-tabular report. Use it to present multi-dimensional data in a human-readable way.");
			AddString(DashboardWinStringId.CommandInsertGridCaption, "Grid");
			AddString(DashboardWinStringId.CommandInsertGridDescription, "Create a grid dashboard item and insert it into the dashboard.\r\n\r\nThe grid shows data in a tabular form while allowing you to summarize against specific measures or calculate differences between them.");
			AddString(DashboardWinStringId.CommandInsertChartCaption, "Chart");
			AddString(DashboardWinStringId.CommandInsertChartDescription, "Create a chart dashboard item and insert it into the dashboard.\r\n\r\nThe chart visualizes data in an XY-diagram allowing you to render a wide range of diagram types – from simple bar or line charts to financial Open-High-Low-Close graphs.");
			AddString(DashboardWinStringId.CommandInsertScatterChartCaption, "Scatter Chart");
			AddString(DashboardWinStringId.CommandInsertScatterChartDescription, "Create a scatter chart dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertPiesCaption, "Pies");
			AddString(DashboardWinStringId.CommandInsertPiesDescription, "Create a pie dashboard item and insert it into the dashboard.\r\n\r\nThis item displays a series of pies or donuts that represent the contribution of each value to a total.");
			AddString(DashboardWinStringId.CommandInsertGaugesCaption, "Gauges");
			AddString(DashboardWinStringId.CommandInsertGaugesDescription, "Create a gauge dashboard item and insert it into the dashboard.\r\n\r\nThis item displays a series of gauges. Each gauge can communicate two values – one with a needle and the other with a marker on the scale.");
			AddString(DashboardWinStringId.CommandInsertCardsCaption, "Cards");
			AddString(DashboardWinStringId.CommandInsertCardsDescription, "Create a card dashboard item and insert it into the dashboard. This item displays a series of cards each illustrating the difference between two values. This difference can be expressed in an absolute or percent variation.");
			AddString(DashboardWinStringId.CommandInsertImageCaption, "Image");
			AddString(DashboardWinStringId.CommandInsertImageDescription, "Create an image dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertTextBoxCaption, "Text Box");
			AddString(DashboardWinStringId.CommandInsertTextBoxDescription, "Create a text box dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertRangeFilterCaption, "Range Filter");
			AddString(DashboardWinStringId.CommandInsertRangeFilterDescription, "Create a range filter dashboard item and insert it into the dashboard.\r\n\r\nThe Range Filter allows you to apply filtering to other dashboard items. Displays a chart with selection thumbs over it that allow you to filter out values displayed along the argument axis.");
			AddString(DashboardWinStringId.CommandInsertChoroplethMapCaption, "Choropleth Map");
			AddString(DashboardWinStringId.CommandInsertChoroplethMapDescription, "Create a choropleth map dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertGeoPointMapsCaption, "Geo Point Maps");
			AddString(DashboardWinStringId.CommandInsertGeoPointMapsDescription, "Create a geo point map dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertGeoPointMapCaption, "Geo Point Map");
			AddString(DashboardWinStringId.CommandInsertGeoPointMapDescription, "Create a geo point map dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertBubbleMapCaption, "Bubble Map");
			AddString(DashboardWinStringId.CommandInsertBubbleMapDescription, "Create a bubble map dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertPieMapCaption, "Pie Map");
			AddString(DashboardWinStringId.CommandInsertPieMapDescription, "Create a pie map dashboard item and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertFilterElementsCaption, "Filter Elements");
			AddString(DashboardWinStringId.CommandInsertFilterElementsDescription, "Insert a filter element into the dashboard.\r\n\r\nFilter elements allow you to apply filtering to other dashboard items.");
			AddString(DashboardWinStringId.CommandInsertComboboxCaption, "Combo Box");
			AddString(DashboardWinStringId.CommandInsertComboboxDescription, "Create a combo box filter element and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertListBoxCaption, "List Box");
			AddString(DashboardWinStringId.CommandInsertListBoxDescription, "Create a list box filter element and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertTreeViewCaption, "Tree View");
			AddString(DashboardWinStringId.CommandInsertTreeViewDescription, "Create a tree view filter element and insert it into the dashboard.");
			AddString(DashboardWinStringId.CommandInsertGroupCaption, "Group");
			AddString(DashboardWinStringId.CommandInsertGroupDescription, "Create a group that arranges dashboard items and allows you to manage interaction between dashboard items within and outside the group.");
			AddString(DashboardWinStringId.CommandDuplicateItemCaption, "Duplicate");
			AddString(DashboardWinStringId.CommandDuplicateItemDescription, "Create a copy of the selected dashboard item.");
			AddString(DashboardWinStringId.CommandDeleteItemCaption, "Delete");
			AddString(DashboardWinStringId.CommandDeleteItemDescription, "Delete the selected dashboard item.");
			AddString(DashboardWinStringId.CommandDeleteGroupCaption, "Delete");
			AddString(DashboardWinStringId.CommandDeleteGroupDescription, "Delete the selected dashboard item group.");
			AddString(DashboardWinStringId.CommandRemoveDataItemsCaption, "Remove Data Items");
			AddString(DashboardWinStringId.CommandRemoveDataItemsDescription, "Remove bindings for the dashboard item.");
			AddString(DashboardWinStringId.CommandTransposeItemCaption, "Transpose");
			AddString(DashboardWinStringId.CommandDefaultTransposeItemDescription, "Transpose the dashboard item.");
			AddString(DashboardWinStringId.CommandTransposePivotDescription, "Transpose pivot rows and columns.");
			AddString(DashboardWinStringId.CommandTransposePieDescription, "Transpose pie arguments and series.");
			AddString(DashboardWinStringId.CommandTransposeChartDescription, "Transpose chart arguments and series.");
			AddString(DashboardWinStringId.CommandTransposeScatterChartDescription, "Transpose the chart's X and Y axes.");
			AddString(DashboardWinStringId.CommandConvertDashboardItemTypeCaption, "Convert To");
			AddString(DashboardWinStringId.CommandConvertDashboardItemTypeDescription, "Convert the selected dashboard item to another type.");
			AddString(DashboardWinStringId.CommandConvertToPivotCaption, "Pivot");
			AddString(DashboardWinStringId.CommandConvertToPivotDescription, "Convert the selected dashboard item to a pivot dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToGridCaption, "Grid");
			AddString(DashboardWinStringId.CommandConvertToGridDescription, "Convert the selected dashboard item to a grid dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToChartCaption, "Chart");
			AddString(DashboardWinStringId.CommandConvertToChartDescription, "Convert the selected dashboard item to a chart dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToScatterChartCaption, "Scatter Chart");
			AddString(DashboardWinStringId.CommandConvertToScatterChartDescription, "Convert the selected dashboard item to a scatter chart dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToPieCaption, "Pies");
			AddString(DashboardWinStringId.CommandConvertToPieDescription, "Convert the selected dashboard item to a pie dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToGaugeCaption, "Gauges");
			AddString(DashboardWinStringId.CommandConvertToGaugeDescription, "Convert the selected dashboard item to a gauge dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToCardCaption, "Cards");
			AddString(DashboardWinStringId.CommandConvertToCardDescription, "Convert the selected dashboard item to a card dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToChoroplethMapCaption, "Choropleth Map");
			AddString(DashboardWinStringId.CommandConvertToChoroplethMapDescription, "Convert the selected dashboard item to a choropleth map dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToGeoPointMapCaption, "Geo Point Map");
			AddString(DashboardWinStringId.CommandConvertToGeoPointMapDescription, "Convert the selected dashboard item to a geo point map dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToBubbleMapCaption, "Bubble Map");
			AddString(DashboardWinStringId.CommandConvertToBubbleMapDescription, "Convert the selected dashboard item to a bubble map dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToPieMapCaption, "Pie Map");
			AddString(DashboardWinStringId.CommandConvertToPieMapDescription, "Convert the selected dashboard item to a pie map dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToGeoPointMapBaseCaption, "Geo Point Map");
			AddString(DashboardWinStringId.CommandConvertToGeoPointMapBaseDescription, "Convert the selected dashboard item to a geo point map dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToRangeFilterCaption, "Range Filter");
			AddString(DashboardWinStringId.CommandConvertToRangeFilterDescription, "Convert the selected dashboard item to a range filter dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToComboBoxCaption, "Combo Box");
			AddString(DashboardWinStringId.CommandConvertToComboBoxDescription, "Convert the selected dashboard item to a combo box dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToListBoxCaption, "List Box");
			AddString(DashboardWinStringId.CommandConvertToListBoxDescription, "Convert the selected dashboard item to a list box dashboard item.");
			AddString(DashboardWinStringId.CommandConvertToTreeViewCaption, "Tree View");
			AddString(DashboardWinStringId.CommandConvertToTreeViewDescription, "Convert the selected dashboard item to a tree view dashboard item.");
			AddString(DashboardWinStringId.CommandEditNamesCaption, "Edit Names...");
			AddString(DashboardWinStringId.CommandEditNamesDescription, "Specify names for the dashboard item and its data item containers.");
			AddString(DashboardWinStringId.CommandEditRulesCaption, "Edit Rules");
			AddString(DashboardWinStringId.CommandEditRulesDescription, "Edit rules used to apply conditional formatting to dashboard item elements.");
			AddString(DashboardWinStringId.CommandShowCaptionCaption, "Show Caption");
			AddString(DashboardWinStringId.CommandShowCaptionDescription, "Show the dashboard item's caption.");
			AddString(DashboardWinStringId.CommandEditFilterCaption, "Edit Filter...");
			AddString(DashboardWinStringId.CommandEditFilterDescription, "Specify a criterion that defines which data should be displayed within the dashboard item.");
			AddString(DashboardWinStringId.CommandClearFilterCaption, "Clear");
			AddString(DashboardWinStringId.CommandClearFilterDescription, "Reset the filter criterion and turn off filtering.");
			AddString(DashboardWinStringId.CommandSingleMasterFilterCaption, "Single Master Filter");
			AddString(DashboardWinStringId.CommandMultipleMasterFilterCaption, "Multiple Master Filter");
			AddString(DashboardWinStringId.CommandDrillDownCaption, "Drill Down");
			AddString(DashboardWinStringId.CommandSingleMasterFilterDescription, " Enable Single Master Filter.\r\nWhen you select an element within the dashboard item, other dashboard items show only data corresponding to the element you have selected.");
			AddString(DashboardWinStringId.CommandMultipleMasterFilterDescription, "Enable Multiple Master Filter.\r\nWhen you select multiple elements within a dashboard item, other dashboard items show only data corresponding to elements you have selected.");
			AddString(DashboardWinStringId.CommandDrillDownDescription, "Enable Drill Down.\r\nClick an element within the dashboard item to display detailed data related to this element.");
			AddString(DashboardWinStringId.CommandDrillDownDisabledDescription, "The Drill Down is not supported when points are used to perform interactivity actions.");
			AddString(DashboardWinStringId.CommandTargetDimensionsArgumentsCaption, "Arguments");
			AddString(DashboardWinStringId.CommandTargetDimensionsSeriesCaption, "Series");
			AddString(DashboardWinStringId.CommandTargetDimensionsPointsCaption, "Points");
			AddString(DashboardWinStringId.CommandTargetDimensionsArgumentsDescription, "Arguments are used to perform Master Filtering and drill-down.");
			AddString(DashboardWinStringId.CommandTargetDimensionsSeriesDescription, "Series are used to perform Master Filtering and drill-down.");
			AddString(DashboardWinStringId.CommandTargetDimensionsPointsDescription, "Points are used to perform interactivity actions.");
			AddString(DashboardWinStringId.CommandCrossDataSourceFilteringCaption, "Cross-Data-Source Filtering");
			AddString(DashboardWinStringId.CommandCrossDataSourceFilteringDescription, "Make dashboard items that display data from other data sources affected by this Master Filter. In this instance, filtering is performed if full names of data source fields match.");
			AddString(DashboardWinStringId.CommandIgnoreMasterFiltersCaption, "Ignore Master Filters");
			AddString(DashboardWinStringId.CommandIgnoreMasterFiltersDescription, "Make this dashboard item unaffected by other Master Filters.");
			AddString(DashboardWinStringId.CommandGroupIgnoreMasterFiltersCaption, "Ignore Master Filters");
			AddString(DashboardWinStringId.CommandGroupIgnoreMasterFiltersDescription, "Make dashboard items contained within the current dashboard item group unaffected by external master filter items.");
			AddString(DashboardWinStringId.CommandGroupMasterFilterCaption, "Master Filter");
			AddString(DashboardWinStringId.CommandGroupMasterFilterDescription, "Enable the capability to filter external dashboard items using master filter items contained within the current dashboard item group.");
			AddString(DashboardWinStringId.CommandContentAutoArrangeCaption, "Auto Arrange");
			AddString(DashboardWinStringId.CommandContentAutoArrangeDescription, "The number of columns and rows is defined automatically.");
			AddString(DashboardWinStringId.CommandContentArrangeInColumnsCaption, "Arrange in Columns");
			AddString(DashboardWinStringId.CommandContentArrangeInColumnsDescription, "Arrange the elements in the specified number of columns.");
			AddString(DashboardWinStringId.CommandContentArrangeInRowsCaption, "Arrange in Rows");
			AddString(DashboardWinStringId.CommandContentArrangeInRowsDescription, "Arrange the elements in the specified number of rows.");
			AddString(DashboardWinStringId.CommandContentArrangementCountCaption, "Count ");
			AddString(DashboardWinStringId.CommandContentArrangementCountDescription, "The number of columns or rows.");
			AddString(DashboardWinStringId.CommandGridHorizontalLinesCaption, "Horizontal Lines");
			AddString(DashboardWinStringId.CommandGridHorizontalLinesDescription, "Show horizontal grid lines.");
			AddString(DashboardWinStringId.CommandGridWordWrapCaption, "Word Wrap");
			AddString(DashboardWinStringId.CommandGridWordWrapDescription, "Enable word wrapping that allows displaying cell content on multiple lines.");
			AddString(DashboardWinStringId.CommandGridAutoFitToContentsColumnWidthModeCaption, "AutoFit to Contents");
			AddString(DashboardWinStringId.CommandGridAutoFitToContentsColumnWidthModeDescription, "Adjust the width of columns automatically to display their content entirely. If the grid cannot display the entire content, horizontal scrolling is enabled.");
			AddString(DashboardWinStringId.CommandGridVerticalLinesCaption, "Vertical Lines");
			AddString(DashboardWinStringId.CommandGridVerticalLinesDescription, "Show vertical grid lines.");
			AddString(DashboardWinStringId.GridColumnFitToContentMenuItemCaption, "Fit to Content");
			AddString(DashboardWinStringId.GridColumnFitToContentMenuItemDescription, "Fit to Content.");
			AddString(DashboardWinStringId.GridColumnWidthMenuItemCaption, "Column Width...");
			AddString(DashboardWinStringId.GridColumnWidthMenuItemDescription, "Column Width...");
			AddString(DashboardWinStringId.GridColumnFixedWidthMenuItemCaption, "Fix Width");
			AddString(DashboardWinStringId.GridColumnFixedWidthMenuItemDescription, "Fix Width.");
			AddString(DashboardWinStringId.ResetGridColumnsWidthMenuItemCaption, "Reset Column Widths");
			AddString(DashboardWinStringId.ResetGridColumnsWidthMenuItemDescription, "Reset column widths.");
			AddString(DashboardWinStringId.GridSortAscendingMenuItemCaption, "Sort Ascending");
			AddString(DashboardWinStringId.GridSortAscendingMenuItemDescription, "Sort Ascending");
			AddString(DashboardWinStringId.GridSortDescendingMenuItemCaption, "Sort Descending");
			AddString(DashboardWinStringId.GridSortDescendingMenuItemDescription, "Sort Descending");
			AddString(DashboardWinStringId.AddGridColumnTotalBarItemCaption, "Add Total");
			AddString(DashboardWinStringId.ClearGridColumnTotalsBarItemCaption, "Clear Totals");
			AddString(DashboardWinStringId.RemoveGridColumnTotalBarItemCaption, "Remove");
			AddString(DashboardWinStringId.GridColumnTotalCountBarItemCaption, "Count");
			AddString(DashboardWinStringId.GridColumnTotalMinBarItemCaption, "Min");
			AddString(DashboardWinStringId.GridColumnTotalMaxBarItemCaption, "Max");
			AddString(DashboardWinStringId.GridColumnTotalAvgBarItemCaption, "Average");
			AddString(DashboardWinStringId.GridColumnTotalSumBarItemCaption, "Sum");
			AddString(DashboardWinStringId.GridColumnTotalAutoBarItemCaption, "Auto");
			AddString(DashboardWinStringId.GridClearSortingMenuItemCaption, "Clear Sorting");
			AddString(DashboardWinStringId.GridClearSortingMenuItemDescription, "Clear Sorting");
			AddString(DashboardWinStringId.CommandGridAutoFitToGridColumnWidthModeCaption, "AutoFit to Grid");
			AddString(DashboardWinStringId.CommandGridAutoFitToGridColumnWidthModeDescription, "Adjust the width of columns automatically to display their content in an optimal way depending on grid width.");
			AddString(DashboardWinStringId.CommandGridManualGridColumnWidthModeCaption, "Manual");
			AddString(DashboardWinStringId.CommandGridManualGridColumnWidthModeDescription, "Adjust the width of columns manually.");
			AddString(DashboardWinStringId.CommandGridMergeCellsCaption, "Merge Cells");
			AddString(DashboardWinStringId.CommandGridMergeCellsDescription, "Merge adjacent cells with identical data.");
			AddString(DashboardWinStringId.CommandGridBandedRowsCaption, "Banded Rows");
			AddString(DashboardWinStringId.CommandGridBandedRowsDescription, "Paint the background of even and odd rows differently to make the grid easier to read.");
			AddString(DashboardWinStringId.CommandGridColumnHeadersCaption, "Column Headers");
			AddString(DashboardWinStringId.CommandGridColumnHeadersDescription, "Show column headers.");
			AddString(DashboardWinStringId.CommandChartRotateCaption, "Rotate");
			AddString(DashboardWinStringId.CommandChartRotateDescription, "Rotate the diagram at 90°.");
			AddString(DashboardWinStringId.CommandChartSeriesTypeCaption, "Series Type");
			AddString(DashboardWinStringId.CommandChartSeriesTypeDescription, "Change the type of chart series.");
			AddString(DashboardWinStringId.CommandChartShowLegendCaption, "Show Legend");
			AddString(DashboardWinStringId.CommandChartShowLegendDescription, "Show the legend that helps end-users identify chart elements.");
			AddString(DashboardWinStringId.CommandScatterChartShowLegendCaption, "Show Legend");
			AddString(DashboardWinStringId.CommandScatterChartShowLegendDescription, "Show the legend that helps end-users identify scatter chart elements.");
			AddString(DashboardWinStringId.CommandScatterChartPointLabelOptionsCaption, "Point Labels");
			AddString(DashboardWinStringId.CommandScatterChartPointLabelOptionsDescription, "Customize settings related to point labels.");
			AddString(DashboardWinStringId.CommandChartYAxisSettingsCaption, "Y-Axis Settings");
			AddString(DashboardWinStringId.CommandChartYAxisSettingsDescription, "Customize various Y-axis settings, such as the visibility, title and displayed range.");
			AddString(DashboardWinStringId.CommandChartXAxisSettingsCaption, "X-Axis Settings");
			AddString(DashboardWinStringId.CommandChartXAxisSettingsDescription, "Customize various X-axis settings, such as the visibility and axis title.");
			AddString(DashboardWinStringId.CommandScatterChartYAxisSettingsCaption, "Y-Axis Settings");
			AddString(DashboardWinStringId.CommandScatterChartYAxisSettingsDescription, "Customize various X-axis settings, such as the visibility and axis type.");
			AddString(DashboardWinStringId.CommandScatterChartXAxisSettingsCaption, "X-Axis Settings");
			AddString(DashboardWinStringId.CommandScatterChartXAxisSettingsDescription, "Customize various Y-axis settings, such as the visibility and axis type.");
			AddString(DashboardWinStringId.CommandChartLegendPositionCaption, "Legend Position");
			AddString(DashboardWinStringId.CommandChartLegendPositionDescription, "Change the legend position within the Chart.");
			AddString(DashboardWinStringId.CommandScatterChartLegendPositionCaption, "Legend Position");
			AddString(DashboardWinStringId.CommandScatterChartLegendPositionDescription, "Change the legend position within the Scatter Chart.");
			AddString(DashboardWinStringId.CommandChartAddPaneCaption, "Add Pane");
			AddString(DashboardWinStringId.CommandChartAddPaneDescription, "Create a new pane within this chart.");
			AddString(DashboardWinStringId.CommandChartRemovePaneCaption, "Remove Pane");
			AddString(DashboardWinStringId.CommandPieStylePieCaption, "Pie");
			AddString(DashboardWinStringId.CommandPieStylePieDescription, "Display pies.");
			AddString(DashboardWinStringId.CommandPieStyleDonutCaption, "Donut");
			AddString(DashboardWinStringId.CommandPieStyleDonutDescription, "Display donuts.");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsCaption, "Data Labels");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsDescription, "Specify which values should be displayed within data labels.");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsNoneCaption, "None");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsNoneDescription, "Do not display data labels");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentCaption, "Argument");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentDescription, "Argument");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsPercentCaption, "Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsPercentDescription, "Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsValueCaption, "Value");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsValueDescription, "Value");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsValueAndPercentCaption, "Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsValueAndPercentDescription, "Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndPercentCaption, "Argument And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndPercentDescription, "Argument And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndValueCaption, "Argument And Value");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndValueDescription, "Argument And Value");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentValueAndPercentCaption, "Argument, Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsDataLabelsArgumentValueAndPercentDescription, "Argument, Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsCaption, "Tooltips");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsDescription, "Specify which values should be displayed within tooltips.");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsNoneCaption, "None");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsNoneDescription, "None");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentCaption, "Argument");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentDescription, "Argument");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsPercentCaption, "Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsPercentDescription, "Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsValueCaption, "Value");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsValueDescription, "Value");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsValueAndPercentCaption, "Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsValueAndPercentDescription, "Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndPercentCaption, "Argument And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndPercentDescription, "Argument And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndValueCaption, "Argument And Value");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndValueDescription, "Argument And Value");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentValueAndPercentCaption, "Argument, Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsTooltipsArgumentValueAndPercentDescription, "Argument, Value And Percent");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionCaption, "Legend Position");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionDescription, "Select the position of the legend.");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionNoneCaption, "None");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionNoneDescription, "None");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionLeftCaption, "Left");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionLeftDescription, "Left");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionRightCaption, "Right");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionRightDescription, "Right");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionTopCaption, "Top");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionTopDescription, "Top");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionBottomCaption, "Bottom");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionBottomDescription, "Bottom");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionTopRightCaption, "TopRight");
			AddString(DashboardWinStringId.CommandPieLabelsLegendPositionTopRightDescription, "TopRight");
			AddString(DashboardWinStringId.CommandPieShowCaptionsCaption, "Show Pie Captions");
			AddString(DashboardWinStringId.CommandPieShowCaptionsDescription, "Show captions for individual diagrams within this dashboard item.");
			AddString(DashboardWinStringId.CommandGaugeStyleFullCircularCaption, "Full Circular");
			AddString(DashboardWinStringId.CommandGaugeStyleFullCircularDescription, "Display full circular gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleHalfCircularCaption, "Half-Circular");
			AddString(DashboardWinStringId.CommandGaugeStyleHalfCircularDescription, "Display half-circular gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleLeftQuarterCircularCaption, "Left-Quarter Circular");
			AddString(DashboardWinStringId.CommandGaugeStyleLeftQuarterCircularDescription, "Display left-quarter circular gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleRightQuarterCircularCaption, "Right-Quarter Circular");
			AddString(DashboardWinStringId.CommandGaugeStyleRightQuarterCircularDescription, "Display right-quarter circular gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleThreeFourthCircularCaption, "Three-Fourth Circular");
			AddString(DashboardWinStringId.CommandGaugeStyleThreeFourthCircularDescription, "Display three-fourth circular gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleLinearHorizontalCaption, "Linear Horizontal");
			AddString(DashboardWinStringId.CommandGaugeStyleLinearHorizontalDescription, "Display horizontal linear gauges.");
			AddString(DashboardWinStringId.CommandGaugeStyleLinearVerticalCaption, "Linear Vertical");
			AddString(DashboardWinStringId.CommandGaugeStyleLinearVerticalDescription, "Display vertical linear gauges.");
			AddString(DashboardWinStringId.CommandGaugeShowCaptionsCaption, "Show Gauge Captions");
			AddString(DashboardWinStringId.CommandGaugeShowCaptionsDescription, "Show captions for individual gauges within this dashboard item.");
			AddString(DashboardWinStringId.CommandImageLoadCaption, "Load Image...");
			AddString(DashboardWinStringId.CommandImageLoadDescription, "Load an image from a file. The dashboard will store the path to the image file and will not store the image itself.");
			AddString(DashboardWinStringId.CommandImageImportCaption, "Import Image...");
			AddString(DashboardWinStringId.CommandImageImportDescription, "Import an image from a file. The dashboard will store a copy of the image.");
			AddString(DashboardWinStringId.CommandImageSizeMode, "Size Mode");
			AddString(DashboardWinStringId.CommandImageSizeModeClipCaption, "Clip");
			AddString(DashboardWinStringId.CommandImageSizeModeClipDescription, "The image is placed in the upper-left corner of the dashboard item and clipped if necessary.");
			AddString(DashboardWinStringId.CommandImageSizeModeStretchCaption, "Stretch");
			AddString(DashboardWinStringId.CommandImageSizeModeStretchDescription, "The image within the dashboard item is stretched or shrunk as appropriate to fit the size of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageSizeModeSqueezeCaption, "Squeeze");
			AddString(DashboardWinStringId.CommandImageSizeModeSqueezeDescription, "If the dashboard item 's dimensions exceed that of the image it contains, the image is centered and shown full-size. Otherwise, the image is resized to fit into the dashboard item 's dimensions.");
			AddString(DashboardWinStringId.CommandImageSizeModeZoomCaption, "Zoom");
			AddString(DashboardWinStringId.CommandImageSizeModeZoomDescription, "The image is sized proportionally (without clipping), so that it is best fitted to the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentCaption, "Alignment");
			AddString(DashboardWinStringId.CommandImageAlignmentTopLeftCaption, "Align Top Left");
			AddString(DashboardWinStringId.CommandImageAlignmentTopLeftDescription, "Align image to the top left corner of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterLeftCaption, "Align Center Left");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterLeftDescription, "Center image vertically and align it to the left of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomLeftCaption, "Align Bottom Left");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomLeftDescription, "Align image to the bottom left corner of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentTopCenterCaption, "Align Top Center");
			AddString(DashboardWinStringId.CommandImageAlignmentTopCenterDescription, "Center image horizontally and align it to the top of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterCenterCaption, "Align Center");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterCenterDescription, "Center image horizontally and vertically within the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomCenterCaption, "Align Bottom Center");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomCenterDescription, "Center image horizontally and align it to the bottom of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentTopRightCaption, "Align Top Right");
			AddString(DashboardWinStringId.CommandImageAlignmentTopRightDescription, "Align image to the top right corner of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterRightCaption, "Align Center Right");
			AddString(DashboardWinStringId.CommandImageAlignmentCenterRightDescription, "Center image vertically and align it to the right of the dashboard item.");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomRightCaption, "Align Bottom Right");
			AddString(DashboardWinStringId.CommandImageAlignmentBottomRightDescription, "Align image to the bottom right corner of the dashboard item.");
			AddString(DashboardWinStringId.CommandMapDefaultShapefileCaption, "Default Map");
			AddString(DashboardWinStringId.CommandMapDefaultShapefileDescription, "Load a default map. ");
			AddString(DashboardWinStringId.CommandMapLoadCaption, "Load Map...");
			AddString(DashboardWinStringId.CommandMapLoadDescription, " Load a map from a file. The dashboard will store a path to the shape file.");
			AddString(DashboardWinStringId.CommandMapImportCaption, "Import Map...");
			AddString(DashboardWinStringId.CommandMapImportDescription, "Import a map from a file. The dashboard will store a copy of the map.");
			AddString(DashboardWinStringId.CommandMapWorldCountriesCaption, "World Countries");
			AddString(DashboardWinStringId.CommandMapWorldCountriesDescription, "Load a world map.");
			AddString(DashboardWinStringId.CommandMapEuropeCaption, "Europe");
			AddString(DashboardWinStringId.CommandMapEuropeDescription, "Load a map of Europe.");
			AddString(DashboardWinStringId.CommandMapAsiaCaption, "Asia");
			AddString(DashboardWinStringId.CommandMapAsiaDescription, "Load a map of Asia.");
			AddString(DashboardWinStringId.CommandMapNorthAmericaCaption, "North America");
			AddString(DashboardWinStringId.CommandMapNorthAmericaDescription, "Load a map of North America.");
			AddString(DashboardWinStringId.CommandMapSouthAmericaCaption, "South America");
			AddString(DashboardWinStringId.CommandMapSouthAmericaDescription, "Load a map of South America.");
			AddString(DashboardWinStringId.CommandMapAfricaCaption, "Africa");
			AddString(DashboardWinStringId.CommandMapAfricaDescription, " Load a map of Africa.");
			AddString(DashboardWinStringId.CommandMapUSACaption, "USA");
			AddString(DashboardWinStringId.CommandMapUSADescription, " Load a map of USA.");
			AddString(DashboardWinStringId.CommandMapCanadaCaption, "Canada");
			AddString(DashboardWinStringId.CommandMapCanadaDescription, "Load a map of Canada.");
			AddString(DashboardWinStringId.CommandMapShowLegendCaption, "Show Color Legend");
			AddString(DashboardWinStringId.CommandMapShowLegendDescription, "Show the color legend that helps end-users identify map elements.");
			AddString(DashboardWinStringId.CommandMapLegendPositionCaption, "Legend Position");
			AddString(DashboardWinStringId.CommandMapLegendPositionDescription, "Change the legend position within the Map.");
			AddString(DashboardWinStringId.CommandMapFullExtentCaption, "Full Extent");
			AddString(DashboardWinStringId.CommandMapFullExtentDescription, "Full Extent.");
			AddString(DashboardWinStringId.CommandMapInitialExtentCaption, "Initial Extent");
			AddString(DashboardWinStringId.CommandMapInitialExtentDescription, "Initial Extent.");
			AddString(DashboardWinStringId.CommandMapShapeTitleAttributeCaption, "Shape Title");
			AddString(DashboardWinStringId.CommandMapShapeTitleAttributeDescription, "Select the required map attribute to display its values within corresponding shape titles.");
			AddString(DashboardWinStringId.CommandChoroplethMapShapeLabelsAttributeCaption, "Shape Labels");
			AddString(DashboardWinStringId.CommandChoroplethMapShapeLabelsAttributeDescription, "Select the required map attributes to display its values within corresponding shape labels.");
			AddString(DashboardWinStringId.CommandWeightedLegendLinearTypeCaption, "Linear");
			AddString(DashboardWinStringId.CommandWeightedLegendLinearTypeDescription, "Show the linear weighted legend that helps end-users identify map elements.");
			AddString(DashboardWinStringId.CommandWeightedLegendNestedTypeCaption, "Nested");
			AddString(DashboardWinStringId.CommandWeightedLegendNestedTypeDescription, "Show the nested weighted legend that helps end-users identify map elements.");
			AddString(DashboardWinStringId.CommandWeightedLegendNoneTypeCaption, "None");
			AddString(DashboardWinStringId.CommandWeightedLegendNoneTypeDescription, "Disable the weighted legend.");
			AddString(DashboardWinStringId.CommandChangeWeightedLegendTypeCaption, "Show Weighted Legend");
			AddString(DashboardWinStringId.CommandChangeWeightedLegendTypeDescription, "Show the weighted legend that helps end-users identify map elements.");
			AddString(DashboardWinStringId.CommandGeoPointMapClusterizationCaption, "Enable Clustering");
			AddString(DashboardWinStringId.CommandGeoPointMapClusterizationDescription, "Enable clustering of map objects.");
			AddString(DashboardWinStringId.CommandPieMapIsWeightedCaption, "Weighted Pies");
			AddString(DashboardWinStringId.CommandPieMapIsWeightedDescription, "Enable weighted pies.");
			AddString(DashboardWinStringId.CommandMapLockNavigationCaption, "Lock Navigation");
			AddString(DashboardWinStringId.CommandMapLockNavigationDescription, "Lock map zooming and scrolling.");
			AddString(DashboardWinStringId.CommandTextBoxEditTextCaption, "Edit...");
			AddString(DashboardWinStringId.CommandTextBoxEditTextDescription, "Edit the content of the text box.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeLineCaption, "Line");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeLineDescription, "Set the Line type for all series.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeStackedLineCaption, "Stacked Line");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeStackedLineDescription, "Set the Stacked Line type for all series.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedLineCaption, "Full-Stacked Line");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedLineDescription, "Set the Full-Stacked Line type for all series.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeAreaCaption, "Area");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeAreaDescription, "Set the Area type for all series.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeStackedAreaCaption, "Stacked Area");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeStackedAreaDescription, "Set the Stacked Area type for all series.");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedAreaCaption, "Full-Stacked Area");
			AddString(DashboardWinStringId.CommandRangeFilterSeriesTypeFullStackedAreaDescription, "Set the Full-Stacked Area type for all series.");
			AddString(DashboardWinStringId.CommandDimensionTopN, "Top N...");
			AddString(DashboardWinStringId.CommandNumericFormat, "Format...");
			AddString(DashboardWinStringId.CommandDataItemRename, "Rename...");
			AddString(DashboardWinStringId.CommandFormatRuleManager, "Edit Rules...");
			AddString(DashboardWinStringId.CommandFormatRuleClearDataItem, "Clear Rules");
			AddString(DashboardWinStringId.CommandFormatRuleValue, "Value");
			AddString(DashboardWinStringId.CommandFormatRuleTopBottom, "Top/Bottom");
			AddString(DashboardWinStringId.CommandFormatRuleAboveBelowAverage, "Average");
			AddString(DashboardWinStringId.CommandFormatRuleRange, "Range");			
			AddString(DashboardWinStringId.CommandFormatRuleTopN, "Top N");
			AddString(DashboardWinStringId.CommandFormatRuleBottomN, "Bottom N");
			AddString(DashboardWinStringId.CommandFormatRuleAboveAverage, "Above Average");
			AddString(DashboardWinStringId.CommandFormatRuleAboveOrEqualAverage, "Above Or Equal Average");
			AddString(DashboardWinStringId.CommandFormatRuleBelowAverage, "Below Average");
			AddString(DashboardWinStringId.CommandFormatRuleBelowOrEqualAverage, "Below Or Equal Average");
			AddString(DashboardWinStringId.CommandFormatRuleGreaterThan, "Greater Than");
			AddString(DashboardWinStringId.CommandFormatRuleGreaterThanOrEqualTo, "Greater Than Or Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleLessThan, "Less Than");
			AddString(DashboardWinStringId.CommandFormatRuleLessThanOrEqualTo, "Less Than Or Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleBetween, "Between");
			AddString(DashboardWinStringId.CommandFormatRuleNotBetween, "Not Between");
			AddString(DashboardWinStringId.CommandFormatRuleBetweenOrEqual, "Between Or Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleNotBetweenOrEqual, "Not Between Or Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleEqualTo, "Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleNotEqualTo, "Not Equal To");
			AddString(DashboardWinStringId.CommandFormatRuleExpression, "Expression");
			AddString(DashboardWinStringId.CommandFormatRuleContains, "Text that Contains");
			AddString(DashboardWinStringId.CommandFormatRuleDateOccurring, "A Date Occurring");
			AddString(DashboardWinStringId.CommandFormatRuleRangeSet, "Range Set");
			AddString(DashboardWinStringId.CommandFormatRuleColorRangeBar, "Color Range Bar");
			AddString(DashboardWinStringId.CommandFormatRuleRangeIcons, "Icon Ranges");
			AddString(DashboardWinStringId.CommandFormatRuleRangeColors, "Color Ranges");
			AddString(DashboardWinStringId.CommandFormatRuleBarRangeColors, "Bar Color Ranges");
			AddString(DashboardWinStringId.CommandFormatRuleRangeGradient, "Gradient Ranges");
			AddString(DashboardWinStringId.CommandFormatRuleGradientRangeBar, "Bar Gradient Ranges");
			AddString(DashboardWinStringId.CommandFormatRuleBar, "Bar");
			AddString(DashboardWinStringId.CommandFormatRuleTopNDescription, "Format {0} values that rank in the top");
			AddString(DashboardWinStringId.CommandFormatRuleBottomNDescription, "Format {0} values that rank in the bottom");
			AddString(DashboardWinStringId.CommandFormatRuleAboveAverageDescription, "Format {0} values that are above average");
			AddString(DashboardWinStringId.CommandFormatRuleAboveOrEqualAverageDescription, "Format {0} values that are above or equal average");
			AddString(DashboardWinStringId.CommandFormatRuleBelowAverageDescription, "Format {0} values that are below average");
			AddString(DashboardWinStringId.CommandFormatRuleBelowOrEqualAverageDescription, "Format {0} values that are below or equal average");
			AddString(DashboardWinStringId.CommandFormatRuleGreaterThanDescription, "Format {0} values that are greater than");
			AddString(DashboardWinStringId.CommandFormatRuleGreaterThanOrEqualToDescription, "Format {0} values that are greater than or equal to");
			AddString(DashboardWinStringId.CommandFormatRuleLessThanDescription, "Format {0} values that are less than");
			AddString(DashboardWinStringId.CommandFormatRuleLessThanOrEqualToDescription, "Format {0} values that are less than or equal to");
			AddString(DashboardWinStringId.CommandFormatRuleBetweenDescription, "Format {0} values that are between");
			AddString(DashboardWinStringId.CommandFormatRuleNotBetweenDescription, "Format {0} values that are not between");
			AddString(DashboardWinStringId.CommandFormatRuleBetweenOrEqualDescription, "Format {0} values that are between or equal to");
			AddString(DashboardWinStringId.CommandFormatRuleNotBetweenOrEqualDescription, "Format {0} values that are not between or equal to");
			AddString(DashboardWinStringId.CommandFormatRuleEqualToDescription, "Format {0} values that are equal to");
			AddString(DashboardWinStringId.CommandFormatRuleNotEqualToDescription, "Format {0} values that are not equal to");
			AddString(DashboardWinStringId.CommandFormatRuleExpressionDescription, "Format values that match the following condition");
			AddString(DashboardWinStringId.CommandFormatRuleContainsDescription, "Format {0} values that contain the text");
			AddString(DashboardWinStringId.CommandFormatRuleDateOccurringDescription, "Format {0} values that contain a date matching these conditions");
			AddString(DashboardWinStringId.CommandFormatRuleRangeSetDescription, "Format {0} values using range conditions");
			AddString(DashboardWinStringId.CommandFormatRuleColorRangeBarDescription, "Format {0} values using color range bar conditions");
			AddString(DashboardWinStringId.CommandFormatRuleRangeGradientDescription, "Format {0} values using range conditions");
			AddString(DashboardWinStringId.CommandFormatRuleGradientRangeBarDescription, "Format {0} values using range bar conditions");
			AddString(DashboardWinStringId.CommandFormatRuleBarDescription, "Format {0} values using bar conditions");
			AddString(DashboardWinStringId.CommandDimensionSortAscending, "Sort Ascending");
			AddString(DashboardWinStringId.CommandDimensionSortDescending, "Sort Descending");
			AddString(DashboardWinStringId.CommandDimensionSortNone, "No Sorting");
			AddString(DashboardWinStringId.CommandDimensionSortModeDisplayText, "(Display Text)");
			AddString(DashboardWinStringId.CommandDimensionSortModeValue, "(Value)");
			AddString(DashboardWinStringId.CommandDimensionSortModeID, "(ID)");
			AddString(DashboardWinStringId.CommandDimensionSortModeKey, "(Key)");
			AddString(DashboardWinStringId.CommandPivotInitialStateCaption, "Initial State");
			AddString(DashboardWinStringId.CommandPivotInitialStateDescription, "Specify how the pivot dashboard item appears in the Dashboard Viewer by default.");
			AddString(DashboardWinStringId.CommandPivotAutoExpandColumnCaption, "Expand Column Groups");
			AddString(DashboardWinStringId.CommandPivotAutoExpandColumnDescription, "Expand Column Groups");
			AddString(DashboardWinStringId.CommandPivotAutoExpandRowCaption, "Expand Row Groups");
			AddString(DashboardWinStringId.CommandPivotAutoExpandRowDescription, "Expand Row Groups");
			AddString(DashboardWinStringId.CommandPivotShowGrandTotalsCaption, "Grand Totals");
			AddString(DashboardWinStringId.CommandPivotShowGrandTotalsDescription, "Show Grand Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandPivotShowColumnGrandTotalsCaption, "Show Column Grand Totals");
			AddString(DashboardWinStringId.CommandPivotShowColumnGrandTotalsDescription, "Show column Grand Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandPivotShowRowGrandTotalsCaption, "Show Row Grand Totals");
			AddString(DashboardWinStringId.CommandPivotShowRowGrandTotalsDescription, "Show row Grand Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandPivotShowTotalsCaption, "Totals");
			AddString(DashboardWinStringId.CommandPivotShowTotalsDescription, "Show Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandPivotShowColumnTotalsCaption, "Show Column Totals");
			AddString(DashboardWinStringId.CommandPivotShowColumnTotalsDescription, "Show column Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandPivotShowRowTotalsCaption, "Show Row Totals");
			AddString(DashboardWinStringId.CommandPivotShowRowTotalsDescription, "Show row Totals in the pivot dashboard item.");
			AddString(DashboardWinStringId.CommandComboBoxStandardTypeCaption, "Standard");
			AddString(DashboardWinStringId.CommandComboBoxStandardTypeDescription, "Set the type of a combo box to 'Standard'.");
			AddString(DashboardWinStringId.CommandComboBoxCheckedTypeCaption, "Checked");
			AddString(DashboardWinStringId.CommandComboBoxCheckedTypeDescription, "Set the type of a combo box to 'Checked'.");
			AddString(DashboardWinStringId.CommandListBoxCheckedTypeCaption, "Checked");
			AddString(DashboardWinStringId.CommandListBoxCheckedTypeDescription, "Set the type of a list box to 'Checked'.");
			AddString(DashboardWinStringId.CommandListBoxRadioTypeCaption, "Radio");
			AddString(DashboardWinStringId.CommandListBoxRadioTypeDescription, "Set the type of a list box to 'Radio'.");
			AddString(DashboardWinStringId.CommandTreeViewAutoExpandCaption, "Auto Expand");
			AddString(DashboardWinStringId.CommandTreeViewAutoExpandDescription, "Specify the initial expanded state of the tree view.");
			AddString(DashboardWinStringId.CommandFilterElementShowAllValueCaption, "Show 'All' Value");
			AddString(DashboardWinStringId.CommandFilterElementShowAllValueDescription, "Enable the (All) option that allows end-users to apply filtering by all values.");
			AddString(DashboardWinStringId.CommandUseGlobalColorsCaption, "Global Colors");
			AddString(DashboardWinStringId.CommandUseGlobalColorsDescription, "Use a global scheme to color elements of a dashboard item."); 
			AddString(DashboardWinStringId.CommandUseLocalColorsCaption, "Local Colors");
			AddString(DashboardWinStringId.CommandUseLocalColorsDescription, "Use a local scheme to color elements of a dashboard item."); 
			AddString(DashboardWinStringId.CommandEditLocalColorSchemeCaption, "Edit Colors");
			AddString(DashboardWinStringId.CommandEditLocalColorSchemeDescription, "Specify colors used to color elements of a dashboard item.");
			AddString(DashboardWinStringId.CommandEditDashboardColorSchemeCaption, "Edit Colors");
			AddString(DashboardWinStringId.CommandEditDashboardColorSchemeDescription, "Edit the dashboard color scheme by binding display values to specific colors.");
			AddString(DashboardWinStringId.CommandAddQueryCaption, "Add Query");
			AddString(DashboardWinStringId.CommandAddQueryDescription, "Add a new query to the selected data source.");
			AddString(DashboardWinStringId.CommandDeleteQueryCaption, "Delete");
			AddString(DashboardWinStringId.CommandDeleteQueryDescription, "Delete the selected query from the current data source.");
			AddString(DashboardWinStringId.CommandEditQueryFilterDescription, "Edit the current query filter.");
			AddString(DashboardWinStringId.CommandEditQueryFilterCaption, "Filter");
			AddString(DashboardWinStringId.CommandRenameQueryCaption, "Rename");
			AddString(DashboardWinStringId.CommandRenameQueryDescription, "Rename the selected query.");
			AddString(DashboardWinStringId.CommandEditQueryCaption, "Edit");
			AddString(DashboardWinStringId.CommandEditQueryDescription, "Edit the selected query.");
			AddString(DashboardWinStringId.CommandEditConnectionCaption, "Edit Connection");
			AddString(DashboardWinStringId.CommandEditConnectionDescription, "Customize parameters used to establish a connection to a data store.");
			AddString(DashboardWinStringId.CommandEditObjectDataSourceCaption, "Edit");
			AddString(DashboardWinStringId.CommandEditObjectDataSourceDescription, "Edit the current data source by selecting the required class definition and the data member.");
			AddString(DashboardWinStringId.CommandEditExcelDataSourceCaption, "Edit");
			AddString(DashboardWinStringId.CommandEditExcelDataSourceDescription, "Edit the current data source by specifying a data file and selecting the required data.");
			AddString(DashboardWinStringId.CommandEditEFDataSourceCaption, "Edit");
			AddString(DashboardWinStringId.CommandEditEFDataSourceDescription, "Edit the Entity Framework data source by selecting an existing data context or using an external assembly.");
			AddString(DashboardWinStringId.DesignerActionListCreateRibbonTransaction, "Create Ribbon");
			AddString(DashboardWinStringId.DesignerActionListUpdateRibbonTransaction, "Update Ribbon");
			AddString(DashboardWinStringId.DesignerActionListCreateBarsTransaction, "Create Bars");
			AddString(DashboardWinStringId.DesignerActionListUpdateBarsTransaction, "Update Bars");
			AddString(DashboardWinStringId.DesignerActionMethodCreateRibbonItem, "Create Ribbon");
			AddString(DashboardWinStringId.DesignerActionMethodUpdateRibbonItem, "Update Ribbon");
			AddString(DashboardWinStringId.DesignerActionMethodCreateBarsItem, "Create Bars");
			AddString(DashboardWinStringId.DesignerActionMethodUpdateBarsItem, "Update Bars");
			AddString(DashboardWinStringId.HistoryItemEnabling, "Enable");
			AddString(DashboardWinStringId.HistoryItemDisabling, "Disable");
			AddString(DashboardWinStringId.HistoryItemNewDataSource, "Create '{0}' data source");
			AddString(DashboardWinStringId.HistoryItemEditDataSource, "Modify the '{0}' data source");
			AddString(DashboardWinStringId.HistoryItemEditConnection, "Modify connection parameters for '{0}'");
			AddString(DashboardWinStringId.HistoryItemServerMode, "{0} server mode for '{1}'");
			AddString(DashboardWinStringId.HistoryItemDeleteDataSource, "Delete '{0}' data source");
			AddString(DashboardWinStringId.HistoryItemRenameDataSource, "Rename the '{0}' data source");
			AddString(DashboardWinStringId.HistoryItemInsertItem, "Insert '{0}'");
			AddString(DashboardWinStringId.HistoryItemInsertGroup, "Insert '{0}'");
			AddString(DashboardWinStringId.HistoryItemDuplicateItem, "Create a copy of '{0}'");
			AddString(DashboardWinStringId.HistoryItemConvertDashboardItemType, "Convert '{0}' to {1}");
			AddString(DashboardWinStringId.HistoryItemDeleteItem, "Delete '{0}'");
			AddString(DashboardWinStringId.HistoryItemSetCurrencyCulture, "Change the dashboard's currency culture");
			AddString(DashboardWinStringId.HistoryItemEditDashboardTitle, "Edit dashboard title");
			AddString(DashboardWinStringId.HistoryItemEditNames, "Edit names within '{0}'");
			AddString(DashboardWinStringId.HistoryItemLayoutChanged, "Layout changed");
			AddString(DashboardWinStringId.HistoryItemShowCaption, "Show/Hide caption for '{0}'");
			AddString(DashboardWinStringId.HistoryItemEditFilter, "Edit filter for '{0}'");
			AddString(DashboardWinStringId.HistoryItemSingleMasterFilter, "{0} Single Master Filter for '{1}'");
			AddString(DashboardWinStringId.HistoryItemMultipleMasterFilter, "{0} Multiple Master Filter for '{1}'");
			AddString(DashboardWinStringId.HistoryItemDrillDown, "{0} Drill-Down for '{1}'");
			AddString(DashboardWinStringId.HistoryItemTargetDimensions, "Modify interactivity options for '{0}'");
			AddString(DashboardWinStringId.HistoryItemCrossDataSourceFiltering, "{0} cross-data-source filtering for '{1}'");
			AddString(DashboardWinStringId.HistoryItemIgnoreMasterFilters, "{0} ignoring of Master Filters for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGroupIgnoreMasterFilters, "{0} ignoring external Master Filters for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGroupMasterFilter, "{0} Master Filter for '{1}'");
			AddString(DashboardWinStringId.HistoryItemContentArrangement, "Change arrangement settings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGridHorizontalLines, "{0} horizontal grid lines visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGridVerticalLines, "{0} vertical grid lines visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGridMergeCells, "{0} cell merging for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGridBandedRows, "{0} banded rows for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGridColumnHeaders, "{0} column headers visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGridColumnOptions, "Change column options for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGridResizeColumns, "Change the '{0}' and '{1}' column widths for '{2}'");
			AddString(DashboardWinStringId.HistoryItemGridAutoFitToContentsColumnWidthMode, "Enable the 'AutoFit to Contents' column width mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGridAutoFitToGridColumnWidthMode, "Enable the 'AutoFit to Grid' column width mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGridManualColumnWidthMode, "Enable the 'Manual' column width mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGridWordWrap, "{0} word wrapping for '{1}'");
			AddString(DashboardWinStringId.HistoryItemEnableGridColumnFitToContent, "Adjust the '{0}' column width to fit its content for '{1}'");
			AddString(DashboardWinStringId.HistoryItemDisableGridColumnFitToContent, "Disable fit to content of the '{0}' column for '{1}'");
			AddString(DashboardWinStringId.HistoryItemEnableGridColumnFixedWidth, "Fix the width of the '{0}' column for '{1}'");
			AddString(DashboardWinStringId.HistoryItemDisableGridColumnFixedWidth, "Disable fixed width of the '{0}' column for '{1}'");
			AddString(DashboardWinStringId.HistoryItemChartSeriesType, "Change series options for '{0}'");
			AddString(DashboardWinStringId.HistoryItemChartRotate, "{0} diagram rotation in '{1}'");
			AddString(DashboardWinStringId.HistoryItemChartShowLegend, "{0} legend visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemChartYAxisSettings, "Change Y axis settings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemChartXAxisSettings, "Change X-axis settings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemScatterChartPointLabelOptions, "Change point label settings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemChartLegendPosition, "Change legend position for '{0}'");
			AddString(DashboardWinStringId.HistoryItemChartAddPane, "Add new pane to '{0}'");
			AddString(DashboardWinStringId.HistoryItemChartRemovePane, "Remove '{0}' pane from '{1}'");
			AddString(DashboardWinStringId.HistoryItemPieStyle, "Change pie style for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPieLabelContentType, "Change data labels content for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPieTooltipContentType, "Change tooltips content for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPieLegendPosition, "Change legend position in '{0}'");
			AddString(DashboardWinStringId.HistoryItemPieShowCaptions, "{0} pie captions visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemGaugeViewType, "Change gauge view type for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGaugeRange, "Change settings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemGaugeShowCaptions, "{0} gauge captions visibility for '{1}'");
			AddString(DashboardWinStringId.HistoryItemImageOpen, "Load image to '{0}'");
			AddString(DashboardWinStringId.HistoryItemImageSizeMode, "Change size mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemImageAlignment, "Change alignment for '{0}'");
			AddString(DashboardWinStringId.HistoryItemTextBoxEditText, "Edit text in '{0}'");
			AddString(DashboardWinStringId.HistoryItemRangeFilterSeriesType, "Change series type for all series in '{0}'");
			AddString(DashboardWinStringId.HistoryItemGeoPointMapClusterization, "{0} clustering for '{1}'");
			AddString(DashboardWinStringId.HistoryItemPieMapIsWeighted, "'{0}' weighted pies for '{1}'");
			AddString(DashboardWinStringId.HistoryItemMapLockNavigation, "Lock map navigation for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapUnLockNavigation, "Unlock map navigation for '{0}'");
			AddString(DashboardWinStringId.HistoryItemBubbleMapOptions, "Change bubble map options for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapOpen, "Load map to '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapAttribute, "Change attribute binding in '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapShapeTitleAttribute, "Change attributes providing shape labels for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapDelta, "Change delta options in '{0}'");
			AddString(DashboardWinStringId.HistoryItemChangeWeightedLegendType, "Change weighted legend type for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDisableWeightedLegend, "Disable weighted Legend for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMapShowLegend, "{0} color legend for '{1}'");
			AddString(DashboardWinStringId.HistoryItemRestoreMapViewport, "Restore map extent in '{0}'");
			AddString(DashboardWinStringId.HistoryItemChangeMapViewport, "Change map extent in '{0}'");
			AddString(DashboardWinStringId.HistoryItemModifyBindings, "Modify bindings for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMeasureSummaryType, "Set summary type for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDimensionSortOrder, "Set sort order for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDimensionSorting, "Set sort mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDimensionSpecifyTopN, "Set Top N for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDataItemNumericFormat, "Set numeric format for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDataItemDateTimeFormat, "Set date-time format for '{0}'");
			AddString(DashboardWinStringId.HistoryItemDataItemRename, "Rename data item for {0}");
			AddString(DashboardWinStringId.HistoryItemDimensionGroupInterval, "Set group interval for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotAutoExpandColumnEnable, "Enable automatic expansion of column groups for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotAutoExpandColumnDisable, "Disable automatic expansion of column groups for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotAutoExpandRowEnable, "Enable automatic expansion of row groups for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotAutoExpandRowDisable, "Disable automatic expansion of row groups for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowColumnGrandTotalsEnable, "Enable the visibility of column Grand Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowColumnGrandTotalsDisable, "Disable the visibility of column Grand Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowRowGrandTotalsEnable, "Enable the visibility of row Grand Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowRowGrandTotalsDisable, "Disable the visibility of row Grand Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowColumnTotalsEnable, "Enable the visibility of column Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowColumnTotalsDisable, "Disable the visibility of column Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowRowTotalsEnable, "Enable the visibility of row Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemPivotShowRowTotalsDisable, "Disable the visibility of row Totals for '{0}'");
			AddString(DashboardWinStringId.HistoryItemComboBoxChangeType, "Change the type of a combo box in '{0}'");
			AddString(DashboardWinStringId.HistoryItemListBoxChangeType, "Change the type of a list box in '{0}'");
			AddString(DashboardWinStringId.HistoryItemTreeViewAutoExpandChanged, "{0} automatic expansion of nodes for '{1}'");
			AddString(DashboardWinStringId.HistoryItemDimensionColoringMode, "Change coloring mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemMeasureColoringMode, "Change measure coloring mode for '{0}'");
			AddString(DashboardWinStringId.HistoryItemUseGlobalColorsEnable, "Use a global scheme to color elements of the '{0}'");
			AddString(DashboardWinStringId.HistoryItemUseGlobalColorsDisable, "Use a local scheme to color elements of the '{0}'");
			AddString(DashboardWinStringId.HistoryItemEditDashboardColorScheme, "Modify a dashboard color scheme");
			AddString(DashboardWinStringId.HistoryItemEditLocalColorScheme, "Modify a local color scheme for '{0}'");
			AddString(DashboardWinStringId.HistoryItemFormatRuleCreate, "Create new '{0}' format rule for the '{1}'");
			AddString(DashboardWinStringId.HistoryItemFormatRuleEdit, "Edit '{0}' format rule of the '{1}'");
			AddString(DashboardWinStringId.HistoryItemFormatRuleClear, "Clear format rules calculated by '{0}' for the '{1}'");
			AddString(DashboardWinStringId.HistoryItemFormatRulesMove, "Change format rules order for the '{0}'");
			AddString(DashboardWinStringId.HistoryItemFormatRulesDelete, "Delete format rule from the '{0}'");
			AddString(DashboardWinStringId.HistoryItemFormatRulePropertyEnabled, "Enable/disable format rule for the '{0}'");
			AddString(DashboardWinStringId.HistoryItemAddQuery, "Add a new query to the '{0}' data source");
			AddString(DashboardWinStringId.HistoryItemEditQuery, "Edit the '{0}' query");
			AddString(DashboardWinStringId.HistoryItemEditQueryFilter, "Edit the current filter for the '{0}' query");
			AddString(DashboardWinStringId.HistoryItemRenameQuery, "Rename the '{0}' query");
			AddString(DashboardWinStringId.HistoryItemDeleteQuery, "Delete the '{0}' query from the '{1}' data source");
			AddString(DashboardWinStringId.HistoryItemAddGridColumnTotal, "Add the '{0}' summary for the '{1}' of '{2}'");
			AddString(DashboardWinStringId.HistoryItemClearGridColumnTotals, "Remove all totals for the '{0}'");
			AddString(DashboardWinStringId.HistoryItemRemoveGridColumnTotal, "Remove the '{0}' total for the '{1}' column of '{2}'");
			AddString(DashboardWinStringId.HistoryItemChangeGridColumnTotal, "Change the summary type for '{0}' to '{1}'");
			AddString(DashboardWinStringId.FormatRuleDataItemsAll, "[All]");
			AddString(DashboardWinStringId.UndoText, "Undo");
			AddString(DashboardWinStringId.RedoText, "Redo");
			AddString(DashboardWinStringId.UndoRedoSingleAction, "{0} {1} Action");
			AddString(DashboardWinStringId.UndoRedoActionsCount, "{0} {1} Actions");
			AddString(DashboardWinStringId.ImageWrongFormatMessage, "The selected image file is corrupted.");
			AddString(DashboardWinStringId.ImageFileCorruptedMessage, "An image file is corrupted.\r\nURL: {0}");
			AddString(DashboardWinStringId.ImageNotFoundMessage, "Cannot find an image file. This file may have been renamed, moved, or deleted.\r\nURL: {0}");
			AddString(DashboardWinStringId.ImageGeneralExceptionMessage, "Cannot open an image file at the following path:\r\n{0}");
			AddString(DashboardWinStringId.ImageCorruptedMessage, "The image file is corrupted.");
			AddString(DashboardWinStringId.DataSourceNameExistsMessage, "A data source with the specified name already exists.");
			AddString(DashboardWinStringId.DataSourceEmptyNameMessage, "The data source name cannot be empty.");
			AddString(DashboardWinStringId.RecentDashboardsCaption, "Recent");
			AddString(DashboardWinStringId.RecentItemsMenuEmptyText, "To browse for a dashboard, click Open.");
			AddString(DashboardWinStringId.RecentBarButtonItemOpenCommand, "&Open");
			AddString(DashboardWinStringId.RecentBarButtonItemPinCommand, "Pin");
			AddString(DashboardWinStringId.RecentBarButtonItemUnpinCommand, "Unpin");
			AddString(DashboardWinStringId.RecentBarButtonItemRemoveCommand, "&Remove from list");
			AddString(DashboardWinStringId.RecentBarButtonItemClearFilesCommand, "&Clear unpinned dashboards");
			AddString(DashboardWinStringId.RecentBarButtonItemClearFoldersCommand, "&Clear unpinned folders");
			AddString(DashboardWinStringId.DeltaValueTypeActualValueCaption, "Actual value");
			AddString(DashboardWinStringId.DeltaValueTypeAbsoluteVariationCaption, "Absolute variation");
			AddString(DashboardWinStringId.DeltaValueTypePercentVariationCaption, "Percent variation");
			AddString(DashboardWinStringId.DeltaValueTypePercentOfTargetCaption, "Percent of target");
			AddString(DashboardWinStringId.DeltaIndicationModeGreaterIsGoodCaption, "Greater is good");
			AddString(DashboardWinStringId.DeltaIndicationModeLessIsGoodCaption, "Less is good");
			AddString(DashboardWinStringId.DeltaIndicationModeWarningIfGreaterCaption, "Warning if greater");
			AddString(DashboardWinStringId.DeltaIndicationModeWarningIfLessCaption, "Warning if less");
			AddString(DashboardWinStringId.DeltaIndicationModeNoIndicationCaption, "No indication");
			AddString(DashboardWinStringId.DeltaThresholdTypeAbsolute, "Absolute");
			AddString(DashboardWinStringId.DeltaThresholdTypePercent, "Percent");
			AddString(DashboardWinStringId.NumericFormatUnitAutoCaption, "Auto");
			AddString(DashboardWinStringId.NumericFormatUnitOnesCaption, "Ones");
			AddString(DashboardWinStringId.NumericFormatUnitThousandsCaption, "Thousands");
			AddString(DashboardWinStringId.NumericFormatUnitMillionsCaption, "Millions");
			AddString(DashboardWinStringId.NumericFormatUnitBillionsCaption, "Billions");
			AddString(DashboardWinStringId.NumericFormatFormatTypeAutoCaption, "Auto");
			AddString(DashboardWinStringId.NumericFormatFormatTypeGeneralCaption, "General");
			AddString(DashboardWinStringId.NumericFormatFormatTypeNumberCaption, "Number");
			AddString(DashboardWinStringId.NumericFormatFormatTypeCurrencyCaption, "Currency");
			AddString(DashboardWinStringId.NumericFormatFormatTypeScientificCaption, "Scientific");
			AddString(DashboardWinStringId.NumericFormatFormatTypePercentCaption, "Percent");
			AddString(DashboardWinStringId.CurrencyInvariantRegion, "Undefined");
			AddString(DashboardWinStringId.DashboardCurrencyUseCurrentCurrency, "Use client system settings");
			AddString(DashboardWinStringId.DataItemCurrencyUseDashboardCurrency, "Use dashboard settings");
			AddString(DashboardWinStringId.DateTimeFormatYearFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatYearFormatFullCaption, "Full");
			AddString(DashboardWinStringId.DateTimeFormatYearFormatAbbreviatedCaption, "Abbreviated");
			AddString(DashboardWinStringId.DateTimeFormatQuarterFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatQuarterFormatFullCaption, "Full");
			AddString(DashboardWinStringId.DateTimeFormatQuarterFormatNumericCaption, "Numeric");
			AddString(DashboardWinStringId.DateTimeFormatMonthFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatMonthFormatNumericCaption, "Numeric");
			AddString(DashboardWinStringId.DateTimeFormatMonthFormatAbbreviatedCaption, "Abbreviated");
			AddString(DashboardWinStringId.DateTimeFormatMonthFormatFullCaption, "Full");
			AddString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatNumericCaption, "Numeric");
			AddString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatAbbreviatedCaption, "Abbreviated");
			AddString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatFullCaption, "Full");
			AddString(DashboardWinStringId.DateTimeFormatDateFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatDateFormatLongCaption, "Long");
			AddString(DashboardWinStringId.DateTimeFormatDateFormatShortCaption, "Short");
			AddString(DashboardWinStringId.DateTimeFormatDateTimeFormatDefaultCaption, "Default");
			AddString(DashboardWinStringId.DateTimeFormatDateTimeFormatLongCaption, "Long");
			AddString(DashboardWinStringId.DateTimeFormatDateTimeFormatShortCaption, "Short");
			AddString(DashboardWinStringId.DateTimeFormatDateTimeFormatTimeOnlyCaption, "Time only");
			AddString(DashboardWinStringId.ExactDateFormatYear, "Year");
			AddString(DashboardWinStringId.ExactDateFormatQuarter, "Quarter");
			AddString(DashboardWinStringId.ExactDateFormatMonth, "Month");
			AddString(DashboardWinStringId.ExactDateFormatDay, "Day");
			AddString(DashboardWinStringId.ExactDateFormatHour, "Hour");
			AddString(DashboardWinStringId.ExactDateFormatMinute, "Minute");
			AddString(DashboardWinStringId.ExactDateFormatSecond, "Second");
			AddString(DashboardWinStringId.ExactDateDateTimeFormatPattern, "{0}: {1}");
			AddString(DashboardWinStringId.ChartCannotRenderData, "Specified data cannot be rendered in a chart");
			AddString(DashboardWinStringId.ConnectionErrorFormDetailPattern, "Connection name: {0}\r\nError message: {1}");
			AddString(DashboardWinStringId.CommandPrintPreview, "Print Preview...");
			AddString(DashboardWinStringId.CommandExportTo, "Export To");
			AddString(DashboardWinStringId.CommandExportToPdf, "Export To PDF");
			AddString(DashboardWinStringId.CommandExportToImage, "Export To Image");
			AddString(DashboardWinStringId.CommandExportToExcel, "Export To Excel");
			AddString(DashboardWinStringId.CommandDrillUp, "Drill Up");
			AddString(DashboardWinStringId.CommandClearMasterFilter, "Clear Master Filter");
			AddString(DashboardWinStringId.CommandClearSelection, "Clear Selection");
			AddString(DashboardWinStringId.CommandSelectOtherValues, "Values");
			AddString(DashboardWinStringId.ChartInsideHorizontalGalleryGroupCaption, "Inside Horizontal");
			AddString(DashboardWinStringId.ChartInsideVerticalGalleryGroupCaption, "Inside Vertical");
			AddString(DashboardWinStringId.ChartOutsideHorizontalGalleryGroupCaption, "Outside Horizontal");
			AddString(DashboardWinStringId.ChartOutsideVerticalGalleryGroupCaption, "Outside Vertical");
			AddString(DashboardWinStringId.ChartTopLeftHorizontalInsideLegendPosition, "Align the legend to the top left corner of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopCenterHorizontalInsideLegendPosition, "Center the legend horizontally and align it to the top of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopRightHorizontalInsideLegendPosition, "Align the legend to the top right corner of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomLeftHorizontalInsideLegendPosition, "Align the legend to the bottom left corner of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomCenterHorizontalInsideLegendPosition, "Center the legend horizontally and align it to the bottom of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomRightHorizontalInsideLegendPosition, "Align the legend to the bottom right corner of the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopLeftVerticalInsideLegendPosition, "Align the legend to the top left corner of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartTopCenterVerticalInsideLegendPosition, "Center the legend horizontally and align it to the top of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartTopRightVerticalInsideLegendPosition, "Align the legend to the top right corner of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartBottomLeftVerticalInsideLegendPosition, "Align the legend to the bottom left corner of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartBottomCenterVerticalInsideLegendPosition, "Center the legend horizontally and align it to the bottom of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartBottomRightVerticalInsideLegendPosition, "Align the legend to the bottom right corner of the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartTopLeftHorizontalOutsideLegendPosition, "Align the legend to the top left corner of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopCenterHorizontalOutsideLegendPosition, "Center the legend horizontally and align it to the top of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopRightHorizontalOutsideLegendPosition, "Align the legend to the top right corner of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomLeftHorizontalOutsideLegendPosition, "Align the legend to the bottom left corner of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomCenterHorizontalOutsideLegendPosition, "Center the legend horizontally and align it to the bottom of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartBottomRightHorizontalOutsideLegendPosition, "Align the legend to the bottom right corner of the chart outside the diagram. Orient the legend horizontally.");
			AddString(DashboardWinStringId.ChartTopLeftVerticalOutsideLegendPosition, "Align the legend to the top left corner of the chart outside the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartTopRightVerticalOutsideLegendPosition, "Align the legend to the top right corner of the chart outside the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartBottomLeftVerticalOutsideLegendPosition, "Align the legend to the bottom left corner of the chart outside the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartBottomRightVerticalOutsideLegendPosition, "Align the legend to the bottom right corner of the chart outside the diagram. Orient the legend vertically.");
			AddString(DashboardWinStringId.ChartPrimaryAxisTypeName, "Primary axis");
			AddString(DashboardWinStringId.ChartSecondaryAxisTypeName, "Secondary axis");
			AddString(DashboardWinStringId.DataFieldBrowserRemoveCalculatedFieldMenuItem, "Delete");
			AddString(DashboardWinStringId.DataFieldBrowserRenameCalculatedFieldMenuItem, "Rename");
			AddString(DashboardWinStringId.DataFieldBrowserCalculatedFieldTypeMenuItem, "Field Type");
			AddString(DashboardWinStringId.DataFieldBrowserCalculatedFieldExpressionEditorMenuItem, "Edit Expression...");
			AddString(DashboardWinStringId.HistoryItemAddCalculatedField, "Add calculated field '{0}'");
			AddString(DashboardWinStringId.HistoryItemRemoveCalculatedField, "Remove calculated field '{0}'");
			AddString(DashboardWinStringId.HistoryItemCalculatedFieldName, "Rename '{0}' to '{1}'");
			AddString(DashboardWinStringId.HistoryItemCalculatedFieldExpression, "Change expression for '{0}'");
			AddString(DashboardWinStringId.HistoryItemCalculatedFieldType, "Change field type for '{0}'");
			AddString(DashboardWinStringId.CommandAddCalculatedFieldCaption, "Add Calculated Field");
			AddString(DashboardWinStringId.CommandAddCalculatedFieldDescription, "Add a new calculated field into the current data source.");
			AddString(DashboardWinStringId.CommandEditDataSourceFilterCaption, "Filter");
			AddString(DashboardWinStringId.CommandEditDataSourceFilterDescription, "Edit the current data source filter.");
			AddString(DashboardWinStringId.CommandClearDataSourceFilterCaption, "Clear");
			AddString(DashboardWinStringId.CommandClearDataSourceFilterDescription, "Clear the current data source filter.");
			AddString(DashboardWinStringId.CommandDashboardParametersCaption, "Parameters");
			AddString(DashboardWinStringId.CommandDashboardParametersDescription, "Edit the current dashboard parameters.");
			AddString(DashboardWinStringId.CommandEditDashboardParametersCaption, "Parameters");
			AddString(DashboardWinStringId.CommandEditDashboardParametersDescription, "Edit the current dashboard parameters.");
			AddString(DashboardWinStringId.HistoryItemEditDashboardParameters, "Edit dashboard parameters");
			AddString(DashboardWinStringId.HistoryItemFilterDataSource, "Filter data source '{0}'");
			AddString(DashboardWinStringId.BarDataSourceName, "Data Source");
			AddString(DashboardWinStringId.RibbonGroupDataSource, "Data Source");
			AddString(DashboardWinStringId.RibbonGroupFiltering, "Filtering");
			AddString(DashboardWinStringId.BarViewName, "View");
			AddString(DashboardWinStringId.RibbonGroupView, "View");
			AddString(DashboardWinStringId.RibbonGroupDesign, "Design");
			AddString(DashboardWinStringId.RibbonGroupHome, " Home");
			AddString(DashboardWinStringId.RibbonGroupSqlDataSourceQuery, "Query");
			AddString(DashboardWinStringId.BarHomeName, "Home");
			AddString(DashboardWinStringId.CommandEditNamesRibbonCaption, "Edit Names");
			AddString(DashboardWinStringId.CommandEditFilterRibbonCaption, "Edit Filter");
			AddString(DashboardWinStringId.CommandImageLoadRibbonCaption, "Load Image");
			AddString(DashboardWinStringId.CommandImageImportRibbonCaption, "Import Image");
			AddString(DashboardWinStringId.CommandMapLoadRibbonCaption, "Load Map");
			AddString(DashboardWinStringId.CommandMapImportRibbonCaption, "Import Map");
			AddString(DashboardWinStringId.CommandTextBoxEditTextRibbonCaption, "Edit");
			AddString(DashboardWinStringId.SparklineIndicationModeArea, "Area");
			AddString(DashboardWinStringId.SparklineIndicationModeLine, "Line");
			AddString(DashboardWinStringId.SparklineIndicationModeBar, "Bar");
			AddString(DashboardWinStringId.SparklineIndicationModeWinLoss, "Win/Loss");
			AddString(DashboardWinStringId.RibbonGroupCommonCaption, "Common");
			AddString(DashboardWinStringId.RibbonGroupColoringCaption, "Coloring");
			AddString(DashboardWinStringId.CommandSelectDashboardCaption, "Select Dashboard");
			AddString(DashboardWinStringId.CommandSelectDashboardDescription, "Select the current dashboard.");
			AddString(DashboardWinStringId.CommandSelectDashboardItemGroupCaption, "Select Group");
			AddString(DashboardWinStringId.CommandSelectDashboardItemGroupDescription, "Select the parent group.");
			AddString(DashboardWinStringId.WeightedLegendGalleryGroupCaption, "Position");
			AddString(DashboardWinStringId.MapGalleryVerticalGroupCaption, "Vertical");
			AddString(DashboardWinStringId.MapGalleryHorizontalGroupCaption, "Horizontal");
			AddString(DashboardWinStringId.MapTopLeftWeightedLegendPosition, "Align the legend to the top left corner of the map.");
			AddString(DashboardWinStringId.MapTopCenterWeightedLegendPosition, "Center the legend horizontally and align it to the top of the map.");
			AddString(DashboardWinStringId.MapTopRightWeightedLegendPosition, "Align the legend to the top right corner of the map.");
			AddString(DashboardWinStringId.MapBottomLeftWeightedLegendPosition, "Align the legend to the bottom left corner of the map.");
			AddString(DashboardWinStringId.MapBottomCenterWeightedLegendPosition, "Center the legend horizontally and align it to the bottom of the map.");
			AddString(DashboardWinStringId.MapBottomRightWeightedLegendPosition, "Align the legend to the bottom right corner of the map.");
			AddString(DashboardWinStringId.MapTopLeftVerticalLegendPosition, "Align the legend to the top left corner of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapTopCenterVerticalLegendPosition, "Center the legend horizontally and align it to the top of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapTopRightVerticalLegendPosition, "Align the legend to the top right corner of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapBottomLeftVerticalLegendPosition, "Align the legend to the bottom left corner of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapBottomCenterVerticalLegendPosition, "Center the legend horizontally and align it to the bottom of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapBottomRightVerticalLegendPosition, "Align the legend to the bottom right corner of the map. Orient the legend vertically.");
			AddString(DashboardWinStringId.MapTopLeftHorizontalLegendPosition, "Align the legend to the top left corner of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapTopCenterHorizontalLegendPosition, "Center the legend horizontally and align it to the top of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapTopRightHorizontalLegendPosition, "Align the legend to the top right corner of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapBottomLeftHorizontalLegendPosition, "Align the legend to the bottom left corner of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapBottomCenterHorizontalLegendPosition, "Center the legend horizontally and align it to the bottom of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapBottomRightHorizontalLegendPosition, "Align the legend to the bottom right corner of the map. Orient the legend horizontally.");
			AddString(DashboardWinStringId.MapLayerOptionsResetCustomScale, "This action will reset the colorizer scale, and all your changes will be discarded. Do you wish to continue?");
			AddString(DashboardWinStringId.HistoryItemMapLayerOptions, "Change map layer options for '{0}'");
			AddString(DashboardWinStringId.PropertyGridParametersLookUpStaticList, "Static List");
			AddString(DashboardWinStringId.PropertyGridParametersLookUpDynamicList, "Dynamic List");
			AddString(DashboardWinStringId.PropertyGridNoDataSource, "(none)");
			AddString(DashboardWinStringId.PropertyGridNoDataMember, "(none)");
			AddString(DashboardWinStringId.PropertyGridNoDashboardItemGroup, "(none)");
			AddString(DashboardWinStringId.PrintPreviewOptionsFormCaption, "Options");
			AddString(DashboardWinStringId.MapShapeNoneAttribute, "NONE");
			AddString(DashboardWinStringId.LocalColorSchemeEditFormCaption, "Local Color Scheme");
			AddString(DashboardWinStringId.GlobalColorSchemeEditFormCaption, "Global Color Scheme");
			AddString(DashboardWinStringId.MenuResetColor, "Reset");
			AddString(DashboardWinStringId.MenuRemoveColor, "Remove");
			AddString(DashboardWinStringId.MenuRetainColor, "Retain this color");
			AddString(DashboardWinStringId.ColorPickerPaletteColorsSectionCaption, "Palette Colors");
			AddString(DashboardWinStringId.MessageNewColorTableDialogSwitchingDataSource, "If you proceed and change the data source, the dimension list below will be cleared. Do you wish to continue?");
			AddString(DashboardWinStringId.MessageNewColorTableDialogSwitchingDataSourceCaption, "Change Data Source");
			AddString(DashboardWinStringId.ColorSchemeGridEmptyText, "Add a data source to the dashboard to create color schemes");
			AddString(DashboardWinStringId.EditorEmptyEnter, "<enter a value>");
			AddString(DashboardWinStringId.EditorAutomaticValue, "(Automatic)");
			AddString(DashboardWinStringId.EditorCustomValue, "(Custom)");
			AddString(DashboardWinStringId.FormatConditionRangeSetNoStyleCaption, "No Style");
			AddString(DashboardWinStringId.FormatConditionTopBottomNCaption, "N = ");
			AddString(DashboardWinStringId.FormatConditionBetweenAndCaption, "and");
			AddString(DashboardWinStringId.FormatConditionAutomaticValueType, "Automatic");
			AddString(DashboardWinStringId.FormatConditionNumberValueType, "Number");
			AddString(DashboardWinStringId.FormatConditionPercentValueType, "Percent");
			AddString(DashboardWinStringId.FormatConditionMinMaxValueCaption, "Value");
			AddString(DashboardWinStringId.FormatConditionMinimumCaption, "Min =");
			AddString(DashboardWinStringId.FormatConditionMaximumCaption, "Max =");
			AddString(DashboardWinStringId.FormatRuleInvalidRule, "<Invalid Rule>");
			AddString(DashboardWinStringId.FormatRuleInvalidRuleChanging, "Invalid Rule cannot be enabled.");
			AddString(DashboardWinStringId.FormatRuleDataItemIsNotAvailable, "(N/A)");
			AddString(DashboardWinStringId.FormatRuleApplyTo, "Apply to");
			AddString(DashboardWinStringId.FormatRuleApplyToRow, "Apply to row");
			AddString(DashboardWinStringId.FormatRuleApplyToColumn, "Apply to column");
			AddString(DashboardWinStringId.FormatRulePercentOf, "% of all values");
			AddString(DashboardWinStringId.FormatRuleFormatStyle, "Format style");
			AddString(DashboardWinStringId.FormatRuleStyleAppearance, "Appearance");
			AddString(DashboardWinStringId.FormatRuleStyle, "Style Settings");
			AddString(DashboardWinStringId.FormatRuleNegativeStyle, "Negative Style Settings");
			AddString(DashboardWinStringId.FormatRuleStyleIcons, "Icons");
			AddString(DashboardWinStringId.FormatRuleRangeAdd, "Add");
			AddString(DashboardWinStringId.FormatRuleRangeDelete, "Delete");
			AddString(DashboardWinStringId.FormatRuleRangeReverseStyles, "Reverse");
			AddString(DashboardWinStringId.FormatRuleRangeUsePercent, "Use % ranges");
			AddString(DashboardWinStringId.FormatRuleAllowNegativeAxis, "Allow negative axis");
			AddString(DashboardWinStringId.FormatRuleDrawAxis, "Draw axis");
			AddString(DashboardWinStringId.FormatRuleShowBarOnly, "Show bar only");
			AddString(DashboardWinStringId.FormatRuleRangeGenerateRanges, "Generate Ranges");
			AddString(DashboardWinStringId.FormatRuleRangeSegmentNumber, "Number of ranges:");
			AddString(DashboardWinStringId.IntersectionLevelColumnCaption, "Column dimension");
			AddString(DashboardWinStringId.IntersectionLevelRowCaption, "Row dimension");
			AddString(DashboardWinStringId.IntersectionLevelModeCaption, "Intersection mode");
			AddString(DashboardWinStringId.IntersectionLevelModeAuto, "(Auto)");
			AddString(DashboardWinStringId.IntersectionLevelModeFirst, "First level");
			AddString(DashboardWinStringId.IntersectionLevelModeLast, "Last level");
			AddString(DashboardWinStringId.IntersectionLevelModeAll, "All levels");
			AddString(DashboardWinStringId.IntersectionLevelModeSpecific, "Specific level");
			AddString(DashboardWinStringId.RangeSetDescription, "Apply formatting to values classified\r\ninto the specified number of ranges");
			AddString(DashboardWinStringId.RangeSetIconBars, "{0} Bars");
			AddString(DashboardWinStringId.RangeSetIconArrowsColored, "{0} Arrows (Colored)");
			AddString(DashboardWinStringId.RangeSetIconArrowsGray, "{0} Arrows (Gray)");
			AddString(DashboardWinStringId.RangeSetIconTriangles, "{0} Triangles");
			AddString(DashboardWinStringId.RangeSetIconCircles, "{0} Circles");
			AddString(DashboardWinStringId.RangeSetIconSymbolsCircled, "{0} Symbols (Circled)");
			AddString(DashboardWinStringId.RangeSetIconSymbolsUncircled, "{0} Symbols (Uncircled)");
			AddString(DashboardWinStringId.RangeSetIconFlags, "3 Flags");
			AddString(DashboardWinStringId.RangeSetIconSigns, "3 Signs");
			AddString(DashboardWinStringId.RangeSetIconStars, "3 Stars");
			AddString(DashboardWinStringId.RangeSetIconTrafficLights, "3 Traffic Lights");
			AddString(DashboardWinStringId.RangeSetIconQuarters, "5 Quarters");
			AddString(DashboardWinStringId.RangeSetIconBoxes, "5 Boxes");
			AddString(DashboardWinStringId.RangeSetIconCirclesRedToBlack, "4 Circles (Red To Black)");
			AddString(DashboardWinStringId.RangeSetIconPositiveNegativeTriangles, "3 Triangles");						
			AddString(DashboardWinStringId.RangeSetColorsPaleRedGreen, "Red/Green (Pale)");
			AddString(DashboardWinStringId.RangeSetColorsPaleRedGreenBlue, "Red/Green/Blue (Pale)");
			AddString(DashboardWinStringId.RangeSetColorsPaleRedYellowGreenBlue, "Red/Yellow/Green/Blue (Pale)");
			AddString(DashboardWinStringId.RangeSetColorsPaleRedOrangeYellowGreenBlue, "Red/Orange/Yellow/Green/Blue (Pale)");
			AddString(DashboardWinStringId.RangeSetColorsRedGreen, "Red/Green");
			AddString(DashboardWinStringId.RangeSetColorsRedGreenBlue, "Red/Green/Blue");
			AddString(DashboardWinStringId.RangeSetColorsRedYellowGreenBlue, "Red/Yellow/Green/Blue");
			AddString(DashboardWinStringId.RangeSetColorsRedOrangeYellowGreenBlue, "Red/Orange/Yellow/Green/Blue");
			AddString(DashboardWinStringId.RangeGradientDescription, "Apply formatting to values using\r\nthe specified color gradient.\r\nUnique colors indicate values\r\ncorresponding to the specified range");
			AddString(DashboardWinStringId.RangeGradientGreenWhite, "Green - White");
			AddString(DashboardWinStringId.RangeGradientWhiteGreen, "White - Green");
			AddString(DashboardWinStringId.RangeGradientRedWhite, "Red - White");
			AddString(DashboardWinStringId.RangeGradientWhiteRed, "White - Red");
			AddString(DashboardWinStringId.RangeGradientYellowGreen, "Yellow - Green");
			AddString(DashboardWinStringId.RangeGradientGreenYellow, "Green - Yellow");
			AddString(DashboardWinStringId.RangeGradientYellowRed, "Yellow - Red");
			AddString(DashboardWinStringId.RangeGradientRedYellow, "Red - Yellow");
			AddString(DashboardWinStringId.RangeGradientBlueWhite, "Blue - White");
			AddString(DashboardWinStringId.RangeGradientWhiteBlue, "White - Blue");
			AddString(DashboardWinStringId.RangeGradientBlueRed, "Blue - Red");
			AddString(DashboardWinStringId.RangeGradientRedBlue, "Red - Blue");
			AddString(DashboardWinStringId.RangeGradientYellowBlue, "Yellow - Blue");
			AddString(DashboardWinStringId.RangeGradientBlueYellow, "Blue - Yellow");
			AddString(DashboardWinStringId.RangeGradientGreenBlue, "Green - Blue");
			AddString(DashboardWinStringId.RangeGradientBlueGreen, "Blue - Green");
			AddString(DashboardWinStringId.RangeGradientGreenWhiteBlue, "Green - White - Blue");
			AddString(DashboardWinStringId.RangeGradientBlueWhiteGreen, "Blue - White - Green");
			AddString(DashboardWinStringId.RangeGradientBlueWhiteRed, "Blue - White - Red");
			AddString(DashboardWinStringId.RangeGradientRedWhiteBlue, "Red - White - Blue");
			AddString(DashboardWinStringId.RangeGradientGreenWhiteRed, "Green - White - Red");
			AddString(DashboardWinStringId.RangeGradientRedWhiteGreen, "Red - White - Green");
			AddString(DashboardWinStringId.RangeGradientGreenYellowRed, "Green - Yellow - Red");
			AddString(DashboardWinStringId.RangeGradientRedYellowGreen, "Red - Yellow - Green");
			AddString(DashboardWinStringId.RangeGradientBlueYellowRed, "Blue - Yellow - Red");
			AddString(DashboardWinStringId.RangeGradientRedYellowBlue, "Red - Yellow - Blue");
			AddString(DashboardWinStringId.RangeGradientGreenYellowBlue, "Green - Yellow - Blue");
			AddString(DashboardWinStringId.RangeGradientBlueYellowGreen, "Blue - Yellow - Green");
			AddString(DashboardWinStringId.FormatConditionCustomAppearanceCaption, "Custom Appearance");
			AddString(DashboardWinStringId.FormatConditionAppearanceNone, "None");
			AddString(DashboardWinStringId.FormatConditionAppearanceCustom, "Custom");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleRed, "Pale Red");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleYellow, "Pale Yellow");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleGreen, "Pale Green");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleBlue, "Pale Blue");
			AddString(DashboardWinStringId.FormatConditionAppearancePalePurple, "Pale Purple");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleCyan, "Pale Cyan");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleOrange, "Pale Orange");
			AddString(DashboardWinStringId.FormatConditionAppearancePaleGray, "Pale Gray");
			AddString(DashboardWinStringId.FormatConditionAppearanceRed, "Red");
			AddString(DashboardWinStringId.FormatConditionAppearanceYellow, "Yellow");
			AddString(DashboardWinStringId.FormatConditionAppearanceGreen, "Green");
			AddString(DashboardWinStringId.FormatConditionAppearanceBlue, "Blue");
			AddString(DashboardWinStringId.FormatConditionAppearancePurple, "Purple");
			AddString(DashboardWinStringId.FormatConditionAppearanceCyan, "Cyan");
			AddString(DashboardWinStringId.FormatConditionAppearanceOrange, "Orange");
			AddString(DashboardWinStringId.FormatConditionAppearanceGray, "Gray");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientRed, "Gradient Red");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientYellow, "Gradient Yellow");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientGreen, "Gradient Green");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientBlue, "Gradient Blue");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientPurple, "Gradient Purple");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientCyan, "Gradient Cyan");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientOrange, "Gradient Orange");
			AddString(DashboardWinStringId.FormatConditionAppearanceGradientTransparent, "Gradient Transparent");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontBold, "Bold");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontItalic, "Italic");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontUnderline, "Underline");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontGrayed, "Gray Font");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontRed, "Red Font");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontYellow, "Yellow Font");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontGreen, "Green Font");
			AddString(DashboardWinStringId.FormatConditionAppearanceFontBlue, "Blue Font");
			AddString(DashboardWinStringId.FormatConditionTopBottomTemplate, "{0} (N= {1}{2})");
			AddString(DashboardWinStringId.FormatConditionRangeSetTemplate, "Set of [{0}] ranges");
			AddString(DashboardWinStringId.FormatConditionBarTemplate, "Bars");
			AddString(DashboardWinStringId.FormatConditionColorRangeBarTemplate, "Set of [{0}] range bars");
			AddString(DashboardWinStringId.FormatConditionGradientRangeBarTemplate, "Gradient of [{0}] range bars");
			AddString(DashboardWinStringId.FormatConditionRangeGradientTemplate, "Gradient of [{0}] ranges");
			AddString(DashboardWinStringId.FormatConditionUndefinedConditionException, "Rule Caption: Undefined Condition");
			AddString(DashboardWinStringId.DashboardFormatConditionPercentValue, "%");
			AddString(DashboardWinStringId.DashboardFormatConditionTemplate, "{0} ({1}{2})");
			AddString(DashboardWinStringId.DashboardFormatConditionBetweenTemplate, "{0} ({1}, {2})");
			AddString(DashboardWinStringId.DashboardFormatConditionBetweenOrEqualTemplate, "{0} [{1}, {2}]");
			AddString(DashboardWinStringId.FormatConditionRangeSetRanges2Caption, "2 Ranges");
			AddString(DashboardWinStringId.FormatConditionRangeSetRanges3Caption, "3 Ranges");
			AddString(DashboardWinStringId.FormatConditionRangeSetRanges4Caption, "4 Ranges");
			AddString(DashboardWinStringId.FormatConditionRangeSetRanges5Caption, "5 Ranges");
			AddString(DashboardWinStringId.FormatConditionRangeGradientTwoColorsCaption, "2 Color Gradient Ranges");
			AddString(DashboardWinStringId.FormatConditionRangeGradientThreeColorsCaption, "3 Color Gradient Ranges");
			AddString(DashboardWinStringId.ConfigureOlapParametersPageViewHeaderDescription, "Specify the connection properties.");
			AddString(DashboardWinStringId.DataSourceTypeSql, "Database");
			AddString(DashboardWinStringId.DataSourceTypeSqlDescription, "Connect to an SQL database and select the required data using the Query Builder or SQL query.");
			AddString(DashboardWinStringId.DataSourceTypeOlap, "Olap");
			AddString(DashboardWinStringId.DataSourceTypeOlapDescription, "Connect to an OLAP cube in an MS Analysis Services database.");
			AddString(DashboardWinStringId.DataSourceTypeEF, "Entity Framework");
			AddString(DashboardWinStringId.DataSourceTypeEFDescription, "Connect to an Entity Framework data source.");
			AddString(DashboardWinStringId.DataSourceTypeObject, "Object Binding");
			AddString(DashboardWinStringId.DataSourceTypeObjectDescription, "Bind to an object that provides the required data.");
			AddString(DashboardWinStringId.DataSourceTypeXmlSchema, "XML Schema");
			AddString(DashboardWinStringId.DataSourceTypeXmlSchemaDescription, "Provide the data source schema using an XML/XSD file or a data class.");
			AddString(DashboardWinStringId.DataSourceTypeExcel, "Microsoft Excel workbook / CSV file");
			AddString(DashboardWinStringId.DataSourceTypeExcelDescription, "Connect to a Microsoft Excel workbook / CSV file and select the required data.");
			AddString(DashboardWinStringId.DataSourceQuery, "Query");
			AddString(DashboardWinStringId.DataSourceDataMember, "Data Member");
			AddString(DashboardWinStringId.MessageColorTableAlreadyExists, "A similar color table already exists in the scheme.");
			AddString(DashboardWinStringId.MessageColorTableAlreadyExistsCaption, "Warning");
			AddString(DashboardWinStringId.MessageColorSchemeValueAlreadyExists, "This value is already present in the table.");
			AddString(DashboardWinStringId.MessageColorSchemeValueAlreadyExistsCaption, "Warning");
			AddString(DashboardWinStringId.MessageDashboardLocked, "Client actions cannot be performed until the Dashboard object is locked. Call the Dashboard.EndUpdate method to unlock the Dashboard.");
			AddString(DashboardWinStringId.AddDashboardParameter, "Add Dashboard Parameter");
			AddString(DashboardWinStringId.ParameterFormCaption, "Dashboard Parameter");
			AddString(DashboardWinStringId.RangeEditorControlBetweenValidateMessage, "Specify a value between {0} and {1}");
			AddString(DashboardWinStringId.RangeEditorControlGreaterValidateMessage, "Specify a value greater than {0}");
			AddString(DashboardWinStringId.RangeEditorControlLessValidateMessage, "Specify a value less than {0}");
			AddString(DashboardWinStringId.RenameQueryFormCaption, "Rename Query");
			AddString(DashboardWinStringId.RenameDataSourceFormCaption, "Rename Data Source");
			AddString(DashboardWinStringId.AutomaticUpdatesCaption, "Automatic Updates"); 
			AddString(DashboardWinStringId.AutomaticUpdatesDescription, "Enable/disable automatic updates for the dashboard.");
			AddString(DashboardWinStringId.UpdateDataCaption, "Update");
			AddString(DashboardWinStringId.UpdateDataDescription, "Update the dashboard according to the last modifications.");
		}
		public override XtraLocalizer<DashboardWinStringId> CreateResXLocalizer() {
			return new DashboardWinResLocalizer();
		}
	}
	public class DashboardWinResLocalizer : DashboardWinLocalizer {
		readonly ResourceManager manager;
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public DashboardWinResLocalizer() {
			if(manager != null)
				manager.ReleaseAllResources();
			manager = new ResourceManager("DevExpress.DashboardWin.LocalizationRes", GetType().Assembly);
		}
		public override string GetLocalizedString(DashboardWinStringId id) {
			return manager.GetString("DashboardWinStringId." + id) ?? String.Empty;
		}
	}
}
