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

using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System.Resources;
using System;
using System.Windows.Markup;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid {
	public enum GridControlStringId {
		CellPeerName,
		GridGroupPanelText,
		GridGroupRowDisplayTextFormat,
		ErrorWindowTitle,
		InvalidRowExceptionMessage,
		GridOutlookIntervals,
		DefaultGroupSummaryFormatString_Count,
		DefaultGroupSummaryFormatString_Min,
		DefaultGroupSummaryFormatString_Max,
		DefaultGroupSummaryFormatString_Avg,
		DefaultGroupSummaryFormatString_Sum,
		DefaultTotalSummaryFormatStringInSameColumn_Count,
		DefaultTotalSummaryFormatStringInSameColumn_Min,
		DefaultTotalSummaryFormatStringInSameColumn_Max,
		DefaultTotalSummaryFormatStringInSameColumn_Avg,
		DefaultTotalSummaryFormatStringInSameColumn_Sum,
		DefaultTotalSummaryFormatString_Count,
		DefaultTotalSummaryFormatString_Min,
		DefaultTotalSummaryFormatString_Max,
		DefaultTotalSummaryFormatString_Avg,
		DefaultTotalSummaryFormatString_Sum,
		DefaultGroupColumnSummaryFormatStringInSameColumn_Count,
		DefaultGroupColumnSummaryFormatStringInSameColumn_Min,
		DefaultGroupColumnSummaryFormatStringInSameColumn_Max,
		DefaultGroupColumnSummaryFormatStringInSameColumn_Avg,
		DefaultGroupColumnSummaryFormatStringInSameColumn_Sum,
		DefaultGroupColumnSummaryFormatString_Count,
		DefaultGroupColumnSummaryFormatString_Min,
		DefaultGroupColumnSummaryFormatString_Max,
		DefaultGroupColumnSummaryFormatString_Avg,
		DefaultGroupColumnSummaryFormatString_Sum,
		PopupFilterAll,
		PopupFilterBlanks,
		PopupFilterNonBlanks,
		ColumnChooserCaption,
		ColumnBandChooserCaption,
		ColumnChooserCaptionForMasterDetail,
		ColumnChooserDragText,
		BandChooserDragText,
		ColumnBandChooserColumnsTabCaption,
		ColumnBandChooserBandsTabCaption,
		GridNewRowText,
		MenuGroupPanelFullExpand,
		MenuGroupPanelFullCollapse,
		MenuGroupPanelClearGrouping,
		MenuColumnSortAscending,
		MenuColumnSortDescending,
		MenuColumnSortBySummaryAscending,
		MenuColumnSortBySummaryDescending,
		MenuColumnSortBySummaryMax,
		MenuColumnSortBySummaryMin,
		MenuColumnSortBySummaryCount,
		MenuColumnSortBySummaryAverage,
		MenuColumnSortBySummarySum,
		MenuColumnClearSorting,
		MenuColumnUnGroup,
		MenuColumnGroup,
		MenuColumnShowGroupPanel,
		MenuColumnHideGroupPanel,
		MenuColumnGroupInterval,
		MenuColumnGroupIntervalNone,
		MenuColumnGroupIntervalDay,
		MenuColumnGroupIntervalMonth,
		MenuColumnGroupIntervalYear,
		MenuColumnGroupIntervalSmart,
		MenuColumnShowColumnChooser,
		MenuColumnHideColumnChooser,
		MenuColumnShowColumnBandChooser,
		MenuColumnHideColumnBandChooser,
		MenuColumnResetGroupSummarySort,
		MenuColumnSortGroupBySummaryMenu,
		MenuColumnGroupSummarySortFormat,
		MenuColumnGroupSummaryEditor,
		MenuColumnBestFit,
		MenuColumnBestFitColumns,
		MenuColumnUnboundExpressionEditor,
		MenuColumnClearFilter,
		MenuColumnFilterEditor,
		MenuColumnFixedStyle,
		MenuColumnFixedNone,
		MenuColumnFixedLeft,
		MenuColumnFixedRight,
		MenuColumnShowSearchPanel,
		MenuColumnHideSearchPanel,
		MenuFooterSum,
		MenuFooterMin,
		MenuFooterMax,
		MenuFooterCount,
		MenuFooterAverage,
		MenuFooterCustom,
		MenuFooterCustomize,
		MenuFooterRowCount,
		GroupSummaryEditorFormCaption,
		TotalSummaryEditorFormCaption,
		TotalSummaryPanelEditorFormCaption,
		SummaryEditorFormItemsTabCaption,
		SummaryEditorFormOrderTabCaption,
		SummaryEditorFormOrderLeftSide,
		SummaryEditorFormOrderRightSide,
		SummaryEditorFormOrderAndAlignmentTabCaption,
		SummaryEditorFormMoveItemUpCaption,
		SummaryEditorFormMoveItemDownCaption,
		FilterEditorTitle,
		FilterPanelCaptionFormatStringForMasterDetail,
		GroupPanelDisplayFormatStringForMasterDetail,
		ErrorPanelTextFormatString,
		ProgressWindowTitle,
		ProgressWindowCancel,
		SummaryItemsSeparator,
		InvalidValueErrorMessage,
		CheckboxSelectorColumnCaption,
		GridCardExpandButtonTooltip,
		GridCardCollapseButtonTooltip,
		NavigationMoveFirstRow,
		NavigationMovePrevPage,
		NavigationMovePrevRow,
		NavigationMoveNextRow,
		NavigationMoveNextPage,
		NavigationMoveLastRow,
		NavigationAddNewRow,
		NavigationDeleteFocusedRow,
		NavigationEditFocusedRow,
		NavigationRecord,
		#region conditional formatting
		#region menu
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_GreaterThan,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_LessThan,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_Between,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_EqualTo,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_TextThatContains,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_ADateOccurring,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_HighlightCellsRules_CustomCondition,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_Top10Items,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_Bottom10Items,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_Top10Percent,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_Bottom10Percent,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_AboveAverage,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_TopBottomRules_BelowAverage,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_DataBars,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_ColorScales,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_IconSets,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_ClearRules,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_ClearRules_FromAllColumns,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_ClearRules_FromCurrentColumns,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		MenuColumnConditionalFormatting_ManageRules,
		#endregion
		#region predefined formats
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_LightRedFillWithDarkRedText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_YellowFillWithDarkYellowText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_GreenFillWithDarkGreenText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_LightRedFill,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_LightGreenFill,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_RedText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_GreenText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_BoldText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_ItalicText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedFormat_StrikethroughText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowRedColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_RedYellowGreenColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteRedColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteGreenColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_BlueWhiteRedColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteBlueColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_WhiteRedColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_WhiteGreenColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_YellowGreenColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedColorScaleFormat_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_BlueSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_GreenSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_RedSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_OrangeSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_LightBlueSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_PurpleSolidDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_BlueGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_GreenGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_RedGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_OrangeGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_LightBlueGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_PurpleGradientDataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_SolidFillGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedDataBarFormat_GradientFillGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows3ColoredIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows4ColoredIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows5ColoredIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows3GrayIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows4GrayIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Arrows5GrayIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Boxes5IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Flags3IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Quarters5IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Ratings4IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Ratings5IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_RedToBlackIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Signs3IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Stars3IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Symbols3CircledIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Symbols3UncircledIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3UnrimmedIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3RimmedIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_TrafficLights4IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Triangles3IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeTrianglesIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsGrayIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsColoredIconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_DirectionalGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_ShapesGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_IndicatorsGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_RatingsGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeGroup,
		#endregion
		#region dialogs
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_GreaterThanDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_LessThanDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BetweenDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_EqualToDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_TextThatContainsDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10ItemsDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10ItemsDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10PercentDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10PercentDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_AboveAverageDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BelowAverageDialog_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_GreaterThanDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_LessThanDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BetweenDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_EqualToDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_TextThatContainsDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10ItemsDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10ItemsDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10PercentDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10PercentDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_AboveAverageDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BelowAverageDialog_Description,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Dialog_ApplyFormatToWholeRowText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BetweenDialog_RangeValuesConnector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionEditor_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessage,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageEx,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_GreaterThanDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_LessThanDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BetweenDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_EqualToDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_TextThatContainsDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_CustomConditionDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_BelowAverageDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_AboveAverageDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10PercentDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10PercentDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Bottom10ItemsDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Top10ItemsDialog_Connector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalToday,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalYesterday,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalTomorrow,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalInTheLast7Days,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalLastWeek,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalThisWeek,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalNextWeek,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalLastMonth,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalThisMonth,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_DateOccurringDialog_IntervalNextMonth,
		#endregion
		#region conditional formatting manager
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Title,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NewFormattingRule,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_EditFormattingRule,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_CellsContainDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DatesOccurring,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_CellValue,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Blanks,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NoBlanks,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Containing,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NotContaining,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_BeginningWith,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_EndingWith,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_SpecificText,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Between,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NotBetween,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_EqualTo,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NotEqualTo,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_GreaterThan,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_LessThan,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_GreaterOrEqual,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_LessOrEqual,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatCells,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Font,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Fill,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormulaDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_GradedColorScale,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBar,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Above,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Below,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Top,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Bottom,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_TopBottomDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_AboveBelowDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ValueBasedDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_2ColorScaleDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_3ColorScaleDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconDescriptionConnector,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconDescriptionCondition,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconDescriptionValueCondition,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_SelectRuleType,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_EditRuleDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ContainTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_AboveBelowTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_TopBottomTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormulaTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Preview,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ClearButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatBackgroundColor,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatStrikethrough,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatUnderline,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatFontFamily,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatFontStyle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatFontWeight,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatFontSize,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatColor,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NoFormatSet,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ShowRulesFor,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NewButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_EditButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DeleteButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Minimum,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Midpoint,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Maximum,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarPositive,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarNegative,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarFill,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarColor,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarBorder,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_DataBarNoBorder,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ApplySameAppearance,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconSetStyle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ReverseButtonDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconSetValueType,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_IconSetRulesTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ValueBasedTitle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ValueBasedStyle,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ValueBasedType,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ValueBasedExpression,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_AboveBelowRangeDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_TopBottomRangeDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ShowModeAll,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ShowModeCurrent,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FieldNameMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_ExpressionMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FillModeSolid,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FillModeGradient,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_AboveMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_BelowMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_PercentMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_NumberMode,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Apply,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Rule,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_AppliesTo,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Format,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_MinValue,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_MaxValue,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_MinMaxValueType,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Auto,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Numeric,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Date,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Icon,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_CustomIconGroup,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_FormatIconDescription,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_CustomFormat,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Up,
		[Obsolete(GridControlLocalizer.ConditionalFormattingObsoleteMessage)]
		ConditionalFormatting_Manager_Down,
		#endregion
		#endregion
		#region Date Filters
		DateFiltering_ShowAllFilterName,
		DateFiltering_FilterBySpecificDateFilterName,
		DateFiltering_PriorToThisYearFilterName,
		DateFiltering_EarlierThisYearFilterName,
		DateFiltering_EarlierThisMonthFilterName,
		DateFiltering_LastWeekFilterName,
		DateFiltering_EarlierThisWeekFilterName,
		DateFiltering_YesterdayFilterName,
		DateFiltering_TodayFilterName,
		DateFiltering_TomorrowFilterName,
		DateFiltering_LaterThisWeekFilterName,
		DateFiltering_NextWeekFilterName,
		DateFiltering_LaterThisMonthFilterName,
		DateFiltering_LaterThisYearFilterName,
		DateFiltering_BeyondThisYearFilterName,
		DateFiltering_BeyondFilterName,
		DateFiltering_ThisWeekFilterName,
		DateFiltering_ThisMonthFilterName,
		DateFiltering_EarlierFilterName,
		DateFiltering_EmptyFilterName,
		#endregion
		#region DragDrop
		DDExtensionsAddRows,
		DDExtensionsCannotDropHere,
		DDExtensionsDraggingMultipleRows,
		DDExtensionsDraggingOneRow,
		DDExtensionsInsertAfter,
		DDExtensionsInsertBefore,
		DDExtensionsMoveToChildrenCollection,
		DDExtensionsMoveToGroup,
		DDExtensionsRow,
		#endregion
		#region Inline Edit Form
		EditForm_UpdateButton,
		EditForm_CancelButton,
		EditForm_Modified,
		EditForm_Cancel,
		EditForm_Warning
		#endregion
	}
	public enum GridControlRuntimeStringId {
		CellPeerName,
		GridGroupPanelText,
		GridGroupRowDisplayTextFormat,
		ErrorWindowTitle,
		InvalidRowExceptionMessage,
		PopupFilterAll,
		PopupFilterBlanks,
		PopupFilterNonBlanks,
		ColumnChooserCaption,
		ColumnBandChooserCaption,
		ColumnChooserCaptionForMasterDetail,
		ColumnChooserDragText,
		BandChooserDragText,
		ColumnBandChooserColumnsTabCaption,
		ColumnBandChooserBandsTabCaption,
		GridNewRowText,
		MenuGroupPanelFullExpand,
		MenuGroupPanelFullCollapse,
		MenuGroupPanelClearGrouping,
		MenuColumnSortAscending,
		MenuColumnSortDescending,
		MenuColumnSortBySummaryAscending,
		MenuColumnSortBySummaryDescending,
		MenuColumnSortBySummaryMax,
		MenuColumnSortBySummaryMin,
		MenuColumnSortBySummaryCount,
		MenuColumnSortBySummaryAverage,
		MenuColumnSortBySummarySum,
		MenuColumnClearSorting,
		MenuColumnUnGroup,
		MenuColumnGroup,
		MenuColumnShowGroupPanel,
		MenuColumnHideGroupPanel,
		MenuColumnGroupInterval,
		MenuColumnGroupIntervalNone,
		MenuColumnGroupIntervalDay,
		MenuColumnGroupIntervalMonth,
		MenuColumnGroupIntervalYear,
		MenuColumnGroupIntervalSmart,
		MenuColumnShowColumnChooser,
		MenuColumnHideColumnChooser,
		MenuColumnShowColumnBandChooser,
		MenuColumnHideColumnBandChooser,
		MenuColumnResetGroupSummarySort,
		MenuColumnSortGroupBySummaryMenu,
		MenuColumnShowSearchPanel,
		MenuColumnHideSearchPanel,
		MenuColumnGroupSummaryEditor,
		MenuColumnBestFit,
		MenuColumnBestFitColumns,
		MenuColumnUnboundExpressionEditor,
		MenuColumnClearFilter,
		MenuColumnFilterEditor,
		MenuColumnFixedStyle,
		MenuColumnFixedNone,
		MenuColumnFixedLeft,
		MenuColumnFixedRight,
		MenuFooterSum,
		MenuFooterMin,
		MenuFooterMax,
		MenuFooterCount,
		MenuFooterAverage,
		MenuFooterCustomize,
		MenuFooterRowCount,
		GroupSummaryEditorFormCaption,
		SummaryEditorFormItemsTabCaption,
		SummaryEditorFormOrderTabCaption,
		SummaryEditorFormOrderLeftSide,
		SummaryEditorFormOrderRightSide,
		SummaryEditorFormOrderAndAlignmentTabCaption,
		SummaryEditorFormMoveItemUpCaption,
		SummaryEditorFormMoveItemDownCaption,
		FilterEditorTitle,
		ProgressWindowTitle,
		ProgressWindowCancel,
		GridCardExpandButtonTooltip,
		GridCardCollapseButtonTooltip,
		NavigationMoveFirstRow,
		NavigationMovePrevPage,
		NavigationMovePrevRow,
		NavigationMoveNextRow,
		NavigationMoveNextPage,
		NavigationMoveLastRow,
		NavigationAddNewRow,
		NavigationDeleteFocusedRow,
		NavigationEditFocusedRow,
		NavigationRecord,
	}
	public class GridRuntimeStringCollection : ObservableCollection<RuntimeStringIdInfo> {
		protected override void InsertItem(int index, RuntimeStringIdInfo item) {
			if(!this.Contains(item)) {
				base.InsertItem(index, item);
			}
			else {
				throw new ArgumentException("Element with such stringId already exists in collection");
			}
		}
	}
	public class RuntimeStringIdInfo {
		public RuntimeStringIdInfo() { }
		public RuntimeStringIdInfo(GridControlRuntimeStringId id, string value) {
			Id = id;
			Value = value;
		}
		public string Value { get; set; }
		public GridControlRuntimeStringId Id { get; set; }
		public override bool Equals(object obj) {
			RuntimeStringIdInfo info = obj as RuntimeStringIdInfo;
			if(info == null) return false;
			else return Id == info.Id;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class LocalizationDescriptor {
		GridRuntimeStringCollection localizationStrings;
		public LocalizationDescriptor(GridRuntimeStringCollection localizationStrings) {
			this.localizationStrings = localizationStrings;
		}
		public void SetValue(string name, string value) {
			if(localizationStrings == null) return;
			GridControlRuntimeStringId id = (GridControlRuntimeStringId)Enum.Parse(typeof(GridControlRuntimeStringId), name, false);
			RuntimeStringIdInfo info = new RuntimeStringIdInfo(id, value);
			int infoIndex = localizationStrings.IndexOf(info);
			if(infoIndex > -1) {
				localizationStrings[infoIndex] = info;
			}
			else {
				localizationStrings.Add(info);
			}
		}
		public string GetValue(string name) {
			GridControlRuntimeStringId id;
			if(localizationStrings != null && Enum.TryParse<GridControlRuntimeStringId>(name, out id)) {
				RuntimeStringIdInfo info = new RuntimeStringIdInfo(id, "");
				int infoIndex = localizationStrings.IndexOf(info);
				if(infoIndex > -1) {
					return localizationStrings[infoIndex].Value;
				}
			}
			GridControlStringId idStatic = (GridControlStringId)Enum.Parse(typeof(GridControlStringId), name, false);
			return GridControlLocalizer.GetString(idStatic);
		}
	}
	public class GridControlLocalizer : DXLocalizer<GridControlStringId> {
		static GridControlLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<GridControlStringId>(CreateDefaultLocalizer()));
		}
		internal const string ConditionalFormattingObsoleteMessage = "Use the DevExpress.Xpf.Core.ConditionalFormatting.ConditionalFormattingLocalizer instead";
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(GridControlStringId.CellPeerName, "{2}, Item: {0}, Column Display Index: {1}");
			AddString(GridControlStringId.GridGroupPanelText, "Drag a column header here to group by that column");
			AddString(GridControlStringId.GridGroupRowDisplayTextFormat, "{0}: {1}");
			AddString(GridControlStringId.ErrorWindowTitle, "Error");
			AddString(GridControlStringId.InvalidRowExceptionMessage, "Do you want to correct the value?");
			AddString(GridControlStringId.GridOutlookIntervals, "Older;Last Month;Earlier this Month;Three Weeks Ago;Two Weeks Ago;Last Week;;;;;;;;Yesterday;Today;Tomorrow;;;;;;;;Next Week;Two Weeks Away;Three Weeks Away;Later this Month;Next Month;Beyond Next Month;");
			#region summary
			AddString(GridControlStringId.DefaultGroupSummaryFormatString_Count, "Count={0}");
			AddString(GridControlStringId.DefaultGroupSummaryFormatString_Min, "Min of {1} is {0}");
			AddString(GridControlStringId.DefaultGroupSummaryFormatString_Max, "Max of {1} is {0}");
			AddString(GridControlStringId.DefaultGroupSummaryFormatString_Avg, "Avg of {1} is {0:0.##}");
			AddString(GridControlStringId.DefaultGroupSummaryFormatString_Sum, "Sum of {1} is {0:0.##}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Count, "Count={0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Min, "Min={0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Max, "Max={0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Avg, "Avg={0:0.##}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatStringInSameColumn_Sum, "Sum={0:0.##}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatString_Count, "Count={0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatString_Min, "Min of {1} is {0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatString_Max, "Max of {1} is {0}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatString_Avg, "Avg of {1} is {0:0.##}");
			AddString(GridControlStringId.DefaultTotalSummaryFormatString_Sum, "Sum of {1} is {0:0.##}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Count, "Count={0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Min, "Min={0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Max, "Max={0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Avg, "Avg={0:0.##}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatStringInSameColumn_Sum, "Sum={0:0.##}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatString_Count, "Count={0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatString_Min, "Min of {1} is {0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatString_Max, "Max of {1} is {0}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatString_Avg, "Avg of {1} is {0:0.##}");
			AddString(GridControlStringId.DefaultGroupColumnSummaryFormatString_Sum, "Sum of {1} is {0:0.##}"); 
			#endregion
			AddString(GridControlStringId.PopupFilterAll, "(All)");
			AddString(GridControlStringId.PopupFilterBlanks, "(Blanks)");
			AddString(GridControlStringId.PopupFilterNonBlanks, "(Non blanks)");
			AddString(GridControlStringId.ColumnChooserCaption, "Column Chooser");
			AddString(GridControlStringId.ColumnBandChooserCaption, "Customization");
			AddString(GridControlStringId.ColumnChooserCaptionForMasterDetail, "{0}: Column Chooser");
			AddString(GridControlStringId.ColumnChooserDragText, "Drag a column here to customize layout");
			AddString(GridControlStringId.BandChooserDragText, "Drag a band here to customize layout");
			AddString(GridControlStringId.ColumnBandChooserColumnsTabCaption, "Columns");
			AddString(GridControlStringId.ColumnBandChooserBandsTabCaption, "Bands");
			AddString(GridControlStringId.GridNewRowText, "Click here to add a new row");
			#region menu
			AddString(GridControlStringId.MenuGroupPanelFullCollapse, "Full Collapse");
			AddString(GridControlStringId.MenuGroupPanelFullExpand, "Full Expand");
			AddString(GridControlStringId.MenuGroupPanelClearGrouping, "Clear Grouping");
			AddString(GridControlStringId.MenuColumnSortAscending, "Sort Ascending");
			AddString(GridControlStringId.MenuColumnSortDescending, "Sort Descending");
			AddString(GridControlStringId.MenuColumnSortBySummaryAverage, "Average");
			AddString(GridControlStringId.MenuColumnSortBySummaryCount, "Count");
			AddString(GridControlStringId.MenuColumnSortBySummarySum, "Sum");
			AddString(GridControlStringId.MenuColumnSortBySummaryMax, "Max");
			AddString(GridControlStringId.MenuColumnSortBySummaryMin, "Min");
			AddString(GridControlStringId.MenuColumnSortBySummaryAscending, "Ascending");
			AddString(GridControlStringId.MenuColumnSortBySummaryDescending, "Descending");
			AddString(GridControlStringId.MenuColumnClearSorting, "Clear Sorting");
			AddString(GridControlStringId.MenuColumnUnGroup, "Ungroup");
			AddString(GridControlStringId.MenuColumnGroup, "Group By This Column");
			AddString(GridControlStringId.MenuColumnShowGroupPanel, "Show Group Panel");
			AddString(GridControlStringId.MenuColumnHideGroupPanel, "Hide Group Panel");
			AddString(GridControlStringId.MenuColumnGroupInterval, "Group Interval");
			AddString(GridControlStringId.MenuColumnGroupIntervalNone, "None");
			AddString(GridControlStringId.MenuColumnGroupIntervalDay, "Day");
			AddString(GridControlStringId.MenuColumnGroupIntervalMonth, "Month");
			AddString(GridControlStringId.MenuColumnGroupIntervalYear, "Year");
			AddString(GridControlStringId.MenuColumnGroupIntervalSmart, "Smart");
			AddString(GridControlStringId.MenuColumnResetGroupSummarySort, "Clear Summary Sorting");
			AddString(GridControlStringId.MenuColumnSortGroupBySummaryMenu, "Sort By Summary");
			AddString(GridControlStringId.MenuColumnGroupSummarySortFormat, "{1} by '{0}' - {2}");
			AddString(GridControlStringId.MenuColumnGroupSummaryEditor, "Group Summary Editor...");
			AddString(GridControlStringId.MenuColumnShowColumnChooser, "Show Column Chooser");
			AddString(GridControlStringId.MenuColumnHideColumnChooser, "Hide Column Chooser");
			AddString(GridControlStringId.MenuColumnShowColumnBandChooser, "Show Column/Band Chooser");
			AddString(GridControlStringId.MenuColumnHideColumnBandChooser, "Hide Column/Band Chooser");
			AddString(GridControlStringId.MenuColumnBestFit, "Best Fit");
			AddString(GridControlStringId.MenuColumnBestFitColumns, "Best Fit (all columns)");
			AddString(GridControlStringId.MenuColumnUnboundExpressionEditor, "Expression Editor...");
			AddString(GridControlStringId.MenuColumnClearFilter, "Clear Filter");
			AddString(GridControlStringId.MenuColumnFilterEditor, "Filter Editor...");
			AddString(GridControlStringId.MenuColumnFixedStyle, "Fixed Style");
			AddString(GridControlStringId.MenuColumnFixedNone, "None");
			AddString(GridControlStringId.MenuColumnFixedLeft, "Left");
			AddString(GridControlStringId.MenuColumnFixedRight, "Right");
			AddString(GridControlStringId.MenuColumnShowSearchPanel, "Show Search Panel");
			AddString(GridControlStringId.MenuColumnHideSearchPanel, "Hide Search Panel");
			AddString(GridControlStringId.MenuFooterSum, "Sum");
			AddString(GridControlStringId.MenuFooterMax, "Max");
			AddString(GridControlStringId.MenuFooterMin, "Min");
			AddString(GridControlStringId.MenuFooterCount, "Count");
			AddString(GridControlStringId.MenuFooterAverage, "Average");
			AddString(GridControlStringId.MenuFooterCustom, "Custom");
			AddString(GridControlStringId.MenuFooterCustomize, "Customize...");
			AddString(GridControlStringId.MenuFooterRowCount, "Show row count"); 
			#endregion
			AddString(GridControlStringId.GroupSummaryEditorFormCaption, "Group Summaries");
			AddString(GridControlStringId.TotalSummaryEditorFormCaption, "Totals for '{0}'");
			AddString(GridControlStringId.TotalSummaryPanelEditorFormCaption, "View Totals");
			AddString(GridControlStringId.SummaryEditorFormItemsTabCaption, "Items");
			AddString(GridControlStringId.SummaryEditorFormOrderTabCaption, "Order");
			AddString(GridControlStringId.SummaryEditorFormOrderLeftSide, "Left side:");
			AddString(GridControlStringId.SummaryEditorFormOrderRightSide, "Right side:");
			AddString(GridControlStringId.SummaryEditorFormOrderAndAlignmentTabCaption, "Order and Alignment");
			AddString(GridControlStringId.SummaryEditorFormMoveItemUpCaption, "Up");
			AddString(GridControlStringId.SummaryEditorFormMoveItemDownCaption, "Down");
			AddString(GridControlStringId.FilterEditorTitle, "Filter Editor");
			AddString(GridControlStringId.FilterPanelCaptionFormatStringForMasterDetail, "{0} filter:");
			AddString(GridControlStringId.GroupPanelDisplayFormatStringForMasterDetail, "{0}:");
			AddString(GridControlStringId.ProgressWindowTitle, "Loading data");
			AddString(GridControlStringId.ProgressWindowCancel, "Cancel");
			AddString(GridControlStringId.ErrorPanelTextFormatString, "Error occurred during processing server request ({0})");
			AddString(GridControlStringId.SummaryItemsSeparator, ", ");
			AddString(GridControlStringId.InvalidValueErrorMessage, "Invalid Value");
			AddString(GridControlStringId.CheckboxSelectorColumnCaption, "Selection");
			AddString(GridControlStringId.GridCardExpandButtonTooltip, "Expand a card");
			AddString(GridControlStringId.GridCardCollapseButtonTooltip, "Collapse a card");
			AddString(GridControlStringId.NavigationMoveFirstRow, "First");
			AddString(GridControlStringId.NavigationMovePrevPage, "Previous Page");
			AddString(GridControlStringId.NavigationMovePrevRow, "Previous");
			AddString(GridControlStringId.NavigationMoveNextRow, "Next");
			AddString(GridControlStringId.NavigationMoveNextPage, "Next Page");
			AddString(GridControlStringId.NavigationMoveLastRow, "Last");
			AddString(GridControlStringId.NavigationAddNewRow, "Append");
			AddString(GridControlStringId.NavigationDeleteFocusedRow, "Delete");
			AddString(GridControlStringId.NavigationEditFocusedRow, "Edit");
			AddString(GridControlStringId.NavigationRecord, "Record {0} of {1}");
			#region Date Filters
			AddString(GridControlStringId.DateFiltering_ShowAllFilterName, "Show all");
			AddString(GridControlStringId.DateFiltering_FilterBySpecificDateFilterName, "Filter by a specific date");
			AddString(GridControlStringId.DateFiltering_PriorToThisYearFilterName, "Prior to this year");
			AddString(GridControlStringId.DateFiltering_EarlierThisYearFilterName, "Earlier this year");
			AddString(GridControlStringId.DateFiltering_EarlierThisMonthFilterName, "Earlier this month");
			AddString(GridControlStringId.DateFiltering_LastWeekFilterName, "Last week");
			AddString(GridControlStringId.DateFiltering_EarlierThisWeekFilterName, "Earlier this week");
			AddString(GridControlStringId.DateFiltering_YesterdayFilterName, "Yesterday");
			AddString(GridControlStringId.DateFiltering_TodayFilterName, "Today");
			AddString(GridControlStringId.DateFiltering_TomorrowFilterName, "Tomorrow");
			AddString(GridControlStringId.DateFiltering_LaterThisWeekFilterName, "Later this week");
			AddString(GridControlStringId.DateFiltering_NextWeekFilterName, "Next week");
			AddString(GridControlStringId.DateFiltering_LaterThisMonthFilterName, "Later this month");
			AddString(GridControlStringId.DateFiltering_LaterThisYearFilterName, "Later this year");
			AddString(GridControlStringId.DateFiltering_BeyondThisYearFilterName, "Beyond this year");
			AddString(GridControlStringId.DateFiltering_BeyondFilterName, "Beyond");
			AddString(GridControlStringId.DateFiltering_ThisWeekFilterName, "This week");
			AddString(GridControlStringId.DateFiltering_ThisMonthFilterName, "This month");
			AddString(GridControlStringId.DateFiltering_EarlierFilterName, "Earlier");
			AddString(GridControlStringId.DateFiltering_EmptyFilterName, "Empty");
			#endregion
			#region DragDrop
			AddString(GridControlStringId.DDExtensionsAddRows, "Add rows");
			AddString(GridControlStringId.DDExtensionsCannotDropHere, "Cannot drop here");
			AddString(GridControlStringId.DDExtensionsDraggingMultipleRows, "Dragging {0} rows");
			AddString(GridControlStringId.DDExtensionsDraggingOneRow, "Dragging 1 row:");
			AddString(GridControlStringId.DDExtensionsInsertAfter, "Insert after row:");
			AddString(GridControlStringId.DDExtensionsInsertBefore, "Insert before row:");
			AddString(GridControlStringId.DDExtensionsMoveToChildrenCollection, "Move to children collection:");
			AddString(GridControlStringId.DDExtensionsMoveToGroup, "Move to group:");
			AddString(GridControlStringId.DDExtensionsRow, "Row");
			#endregion
			#region Inline Edit Form
			AddString(GridControlStringId.EditForm_UpdateButton, "Update");
			AddString(GridControlStringId.EditForm_CancelButton, "Cancel");
			AddString(GridControlStringId.EditForm_Modified, "Your data is modified. Do you want to save the changes?");
			AddString(GridControlStringId.EditForm_Cancel, "Do you want to cancel editing?");
			AddString(GridControlStringId.EditForm_Warning, "Warning");
			#endregion
		}
		#endregion
		public static XtraLocalizer<GridControlStringId> CreateDefaultLocalizer() {
			return new GridControlResXLocalizer();
		}
		public static string GetString(GridControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		internal static string GetString(string stringId) {
			return GetString((GridControlStringId)Enum.Parse(typeof(GridControlStringId), stringId, false));
		}
		public override XtraLocalizer<GridControlStringId> CreateResXLocalizer() {
			return new GridControlResXLocalizer();
		}
		internal static GridControlStringId? GetMenuSortByGroupSummaryStringId(DevExpress.Data.SummaryItemType summaryType) {
			switch(summaryType) {
				case DevExpress.Data.SummaryItemType.Average:
					return GridControlStringId.MenuColumnSortBySummaryAverage;
				case DevExpress.Data.SummaryItemType.Count:
					return GridControlStringId.MenuColumnSortBySummaryCount;
				case DevExpress.Data.SummaryItemType.Max:
					return GridControlStringId.MenuColumnSortBySummaryMax;
				case DevExpress.Data.SummaryItemType.Min:
					return GridControlStringId.MenuColumnSortBySummaryMin;
				case DevExpress.Data.SummaryItemType.Sum:
					return GridControlStringId.MenuColumnSortBySummarySum;
				default:
					return null;
			}
		}
	}
	public class GridControlResXLocalizer : DXResXLocalizer<GridControlStringId> {
		public GridControlResXLocalizer()
			: base(new GridControlLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Grid.Core.LocalizationRes", typeof(GridControlResXLocalizer).Assembly);
		}
	}
	public class GridControlStringIdConverter : StringIdConverter<GridControlStringId> {
		protected override XtraLocalizer<GridControlStringId> Localizer { get { return GridControlLocalizer.Active; } }
	}
	public class DataControlStringIdExtension : MarkupExtension {
		public GridControlStringId StringId { get; set; }
		public DataControlStringIdExtension(GridControlStringId stringId) {
			StringId = stringId;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return GridControlLocalizer.GetString(StringId);
		}
	} 
}
