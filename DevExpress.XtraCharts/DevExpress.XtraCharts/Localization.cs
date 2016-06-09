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

using System.ComponentModel;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraCharts.Localization {
	#region enum ChartStringId
	public enum ChartStringId {
		SeriesPrefix,
		PalettePrefix,
		XYDiagramPanePrefix,
		SecondaryAxisXPrefix,
		SecondaryAxisYPrefix,
		ConstantLinePrefix,
		CustomAxisLabelPrefix,
		ScaleBreakPrefix,
		AnnotationPrefix,
		ImageAnnotationPrefix,
		TextAnnotationPrefix,
		StripPrefix,
		StackedGroupPrefix,
		ArgumentMember,
		ValueMember,
		WeightMember,
		LowValueMember,
		HighValueMember,
		OpenValueMember,
		CloseValueMember,
		AutocreatedSeriesName,
		DefaultDataFilterName,
		DefaultChartTitle,
		DefaultAnnotation,
		DefaultSeriesPointFilterName,
		MsgEquallySpacedItemsNotUsable,
		MsgSeriesViewDoesNotExist,
		MsgEmptyArrayOfValues,
		MsgIncorrectArrayOfValues,
		MsgItemNotInCollection,
		MsgIncorrectIndentFromMarker,
		MsgIncompatibleArgument,
		MsgIncompatibleValue,
		MsgIncompatiblePointType,
		MsgIncompatibleArgumentDataMember,
		MsgIncompatibleValueDataMember,
		MsgDesignTimeOnlySetting,
		MsgInvalidDataSource,
		MsgIncorrectDataMember,
		MsgInvalidSortingKey,
		MsgInvalidFilterCondition,
		MsgIncorrectDataAdapter,
		MsgDataSnapshot,
		MsgModifyDefaultPaletteError,
		MsgAddExistingPaletteError,
		MsgInternalPropertyChangeError,
		MsgPaletteNotFound,
		MsgLabelSettingRuntimeError,
		MsgPointOptionsSettingRuntimeError,
		MsgLegendPointOptionsSettingRuntimeError,
		MsgSynchronizePointOptionsSettingRuntimeError,
		MsgIncorrectNumericPrecision,
		MsgIncorrectAxisThickness,
		MsgIncorrectBarWidth,
		MsgIncorrectBarDepth,
		MsgIncorrectLineWidth,
		MsgIncorrectAreaWidth,
		MsgIncorrectBorderThickness,
		MsgIncorrectChartTitleIndent,
		MsgIncorrectMaxLineCount,
		MsgIncorrectLegendMarkerSize,
		MsgIncorrectLegendHorizontalIndent,
		MsgIncorrectLegendVerticalIndent,
		MsgIncorrectLegendTextOffset,
		MsgIncorrectMarkerSize,
		MsgIncorrectMarkerStarPointCount,
		MsgIncorrectPieSeriesLabelColumnIndent,
		MsgIncorrectRangeBarSeriesLabelIndent,
		MsgIncorrectPercentPrecision,
		MsgIncorrectPercentageAccuracy,
		MsgIncorrectSeriesLabelLineLength,
		MsgIncorrectStripConstructorParameters,
		MsgIncorrectStripLimitAxisValue,
		MsgIncorrectStripMinLimit,
		MsgIncorrectStripMaxLimit,
		MsgIncorrectLineThickness,
		MsgIncorrectShadowSize,
		MsgIncorrectTickmarkThickness,
		MsgIncorrectTickmarkLength,
		MsgIncorrectTickmarkMinorThickness,
		MsgIncorrectTickmarkMinorLength,
		MsgIncorrectMinorCount,
		MsgIncorrectPercentValue,
		MsgIncorrectPointDistance,
		MsgIncorrectSimpleDiagramDimension,
		MsgIncorrectStockLevelLineLengthValue,
		MsgIncorrectReductionColorValue,
		MsgIncorrectLabelAngle,
		MsgIncorrectImageBounds,
		MsgIncorrectUseImageUrlProperty,
		MsgIncorrectSeriesDistance,
		MsgIncorrectSeriesDistanceFixed,
		MsgIncorrectSeriesIndentFixed,
		MsgIncorrectPlaneDepthFixed,
		MsgIncorrectShapeFillet,
		MsgIncorrectAnnotationHeight,
		MsgIncorrectAnnotationWidth,
		MsgIncorrectTextAnnotationAngle,
		MsgIncorrectRelativePositionConnectorLength,
		MsgIncorrectRelativePositionAngle,
		MsgIncorrectAxisCoordinateAxisValue,
		MsgIncorrectAnnotationZOrder,
		MsgIncorrectLabelMaxWidth,
		MsgNullPaneAnchorPointPane,
		MsgIncorrectPaneAnchorPointPane,
		MsgNullAxisXCoordinateAxis,
		MsgIncorrectAxisXCoordinateAxis,
		MsgNullAxisYCoordinateAxis,
		MsgIncorrectAxisYCoordinateAxis,
		MsgNullAnchorPointSeriesPoint,
		MsgIncorrectAnchorPointSeriesPoint,
		MsgIncorrectFreePositionDockTarget,
		MsgEmptyArgument,
		MsgIncorrectImageFormat,
		MsgIncorrectValueDataMemberCount,
		MsgPaletteEditingIsNotAllowed,
		MsgPaletteDoubleClickToEdit,
		MsgInvalidPaletteName,
		MsgInvalidScaleType,
		MsgIncorrectTransformationMatrix,
		MsgIncorrectPerspectiveAngle,
		MsgIncorrectPieDepth,
		MsgIncorrectGridSpacing,
		MsgIncompatibleArgumentScaleType,
		MsgIncompatibleValueScaleType,
		MsgIncompatibleSummaryFunction,
		MsgIncorrectConstantLineAxisValue,
		MsgIncorrectCustomAxisLabelAxisValue,
		Msg3DRotationToolTip,
		Msg3DScrollingToolTip,
		Msg2DScrollingToolTip,
		Msg2DPieExplodingToolTip,
		MsgIncorrectTaskLinkMinIndent,
		MsgIncorrectArrowWidth,
		MsgIncorrectArrowHeight,
		MsgInvalidZeroAxisAlignment,
		MsgNullSeriesViewAxisX,
		MsgNullSeriesViewAxisY,
		MsgNullSeriesViewPane,
		MsgIncorrectSeriesViewPane,
		MsgIncorrectSeriesViewAxisX,
		MsgIncorrectSeriesViewAxisY,
		MsgIncorrectIndicatorPane,
		MsgIncorrectIndicatorAxisY,
		MsgIncorrectParentSeriesPointOwner,
		MsgSeriesViewNotSupportRelations,
		MsgIncorrectChildSeriesPointOwner,
		MsgIncorrectChildSeriesPointID,
		MsgIncorrectSeriesOfParentAndChildPoints,
		MsgSelfRelatedSeriesPoint,
		MsgSeriesPointRelationAlreadyExists,
		MsgChildSeriesPointNotExist,
		MsgRelationChildPointIDNotUnique,
		MsgSeriesPointIDNotUnique,
		MsgIncorrectFont,
		MsgIncorrectScrollBarThickness,
		MsgIncorrectZoomPercent,
		MsgIncorrectHorizontalScrollPercent,
		MsgIncorrectVerticalScrollPercent,
		MsgIncorrectAnchorPoint,
		MsgIncorrectShapePosition,
		MsgIncorrectToolTipPosition,
		MsgIncorrectCrosshairPosition,
		MsgIncorrectPath,
		MsgRegisterPageInUnregisterGroup,
		MsgUnregisterPageError,
		MsgUnregisterGroupError,
		MsgWizardAbstractPageType,
		MsgWizardIncorrectBasePageType,
		MsgWizardNonUniquePageType,
		MsgWizardNonUniqueGroupName,
		MsgOrderArrayLengthMismatch,
		MsgOrderUnregisteredElementFound,
		MsgOrderRepeatedElementFound,
		MsgNotChartControl,
		MsgNotBelongingChart,
		MsgInitializeChartNotFound,
		MsgAddPresentViewType,
		MsgAddLastViewType,
		MsgCalcHitInfoNotSupported,
		MsgIncorrectAppearanceName,
		MsgIncompatibleByViewType,
		MsgIncompatibleByArgumentScaleType,
		MsgIncompatibleByValueScaleType,
		MsgInvalidExplodedSeriesPoint,
		MsgInvalidExplodedModeAdd,
		MsgInvalidExplodedModeRemove,
		MsgIncorrectExplodedDistancePercentage,
		MsgIncorrectPaletteBaseColorNumber,
		MsgDenyChangeSeriesPointCollection,
		MsgDenyChangeSeriesPointArgument,
		MsgDenyChangeSeriesPointValue,
		MsgIncorrectStartAngle,
		MsgPolarAxisXRangeChanged,
		MsgPolarAxisXGridSpacingChanged,
		MsgPolarAxisXLogarithmic,
		MsgIncorrectPieArgumentScaleType,
		MsgIncorrectDoughnutHolePercent,
		MsgIncorrectNestedDoughnutInnerIndent,
		MsgIncorrectNestedDoughnutWeight,
		MsgIncorrectFunnelHolePercent,
		MsgIncorrectLineTensionPercent,
		MsgEmptyChart,
		MsgNoPanes,
		MsgChartLoadingException,
		MsgIncorrectPaneWeight,
		MsgIncorrectPaneDistance,
		MsgEmptyPaneTextForVerticalLayout,
		MsgEmptyPaneTextForHorizontalLayout,
		MsgInvalidPaneSizeInPixels,
		MsgIncorrectTopNCount,
		MsgIncorrectTopNThresholdValue,
		MsgIncorrectTopNThresholdPercent,
		MsgInvalidPane,
		MsgIncorrectSummaryFunction,
		MsgNullFinancialIndicatorArgument,
		MsgUnsupportedValueLevel,
		MsgSummaryFunctionIsNotRegistered,
		MsgSummaryFunctionParameterIsNotSpecified,
		MsgIncompatibleSummaryFunctionDimension,
		MsgIncorrectSummaryFunctionParametersCount,
		MsgWebInvalidWidthUnit,
		MsgWebInvalidHeightUnit,
		MsgIncorrectBubbleMaxSize,
		MsgIncorrectBubbleMinSize,
		MsgInvalidLogarithmicBase,
		MsgUnsupportedTopNOptions,
		MsgUnsupportedResolveOverlappingMode,
		MsgIncorrectDateTimeMeasureUnitPropertyUsing,
		MsgMeasureUnitCanNotBeSetForAxisY,
		MsgIncorrectDateTimeGridAlignmentPropertyUsing,
		MsgIncorrectDateTimeGridAlignment,
		MsgIncorrectNumericMeasureUnit,
		MsgIncorrectNumericGridAlignment,
		MsgIncorrectNumericMeasureUnitPropertyUsing,
		MsgIncorrectNumericGridAlignmentPropertyUsing,
		MsgIncorrectValueLevel,
		MsgUnsupportedDateTimeScaleModeWithScrollingZooming,
		MsgUnsupportedDateTimeScaleModeForGanttDiagram,
		MsgIncorrectAxisRangeMinValue,
		MsgIncorrectAxisRangeMaxValue,
		MsgIncorrectAxisRangeMinValueInternal,
		MsgIncorrectAxisRangeMaxValueInternal,
		MsgIncorrectPropertyValue,
		MsgValueMustBeGreaterThenZero,
		MsgMinMaxDifferentTypes,
		MsgIncorrectAxisRange,
		MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode,
		MsgUnsupportedWorkdaysForWorkdaysOptions,
		MsgCantSwapSeries,
		MsgInvalidEdge1,
		MsgInvalidEdge2,
		MsgInvalidSizeInPixels,
		MsgInvalidMaxCount,
		MsgInvalidGradientMode,
		MsgIncorrectDashStyle,
		MsgAnnotationMovingToolTip,
		MsgAnnotationResizingToolTip,
		MsgAnnotationRotationToolTip,
		MsgAnchorPointMovingToolTip,
		MsgPivotGridDataSourceOptionsNotSupportedProperty,
		MsgDiagramToPointIncorrectValue,
		MsgIncorrectBarDistancePropertyUsing,
		MsgIncorrectBarDistanceFixedPropertyUsing,
		MsgIncorrectEqualBarWidthPropertyUsing,
		MsgFileNotFound,
		MsgCantImportHolidays,
		MsgIncorrectIndicator,
		MsgIncorrectBarSeriesLabelPosition,
		MsgIncorrectBarSeriesLabelIndent,
		MsgIncorrectPointsCount,
		MsgIncorrectEnvelopePercent,
		MsgEmptySecondaryAxisName,
		MsgEmptyPaneName,
		MsgIncorrectRangeControlClientSpacing,
		MsgIncorrectDateTimeRangeControlClientSpacing,
		MsgProcessMissingPointsForValueAxis,
		MsgProcessMissingPointsForContinuousScale,
		MsgIncorrectMaxZoomPercent,
		VerbAbout,
		VerbAboutDescription,
		VerbPopulate,
		VerbPopulateDescription,
		VerbClearDataSource,
		VerbClearDataSourceDescription,
		VerbDataSnapshot,
		VerbDataSnapshotDescription,
		VerbAnnotations,
		VerbAnnotationsDescription,
		VerbSeries,
		VerbSeriesDescription,
		VerbResetLegendPointOptions,
		VerbResetLegendPointOptionsDescription,
		VerbEditPalettes,
		VerbEditPalettesDescription,
		VerbWizard,
		VerbWizardDescription,
		VerbSaveLayout,
		VerbSaveLayoutDescription,
		VerbLoadLayout,
		VerbLoadLayoutDescription,
		PieIncorrectValuesText,
		FontFormat,
		TrnSeriesChanged,
		TrnDataFiltersChanged,
		TrnChartTitlesChanged,
		TrnPalettesChanged,
		TrnConstantLinesChanged,
		TrnStripsChanged,
		TrnCustomAxisLabelChanged,
		TrnSecondaryAxesXChanged,
		TrnSecondaryAxesYChanged,
		TrnXYDiagramPanesChanged,
		TrnChartWizard,
		TrnSeriesDeleted,
		TrnChartTitleDeleted,
		TrnAnnotationDeleted,
		TrnConstantLineDeleted,
		TrnPaneDeleted,
		TrnSecondaryAxisXDeleted,
		TrnSecondaryAxisYDeleted,
		TrnExplodedPoints,
		TrnExplodedPointsFilters,
		TrnLegendPointOptionsReset,
		TrnLoadLayout,
		TrnSeriesTitleChanged,
		TrnSeriesTitleDeleted,
		TrnAxisVisibilityChanged,
		TrnSummaryFunctionChanged,
		TrnIndicatorDeleted,
		TrnIndicatorsChanged,
		TrnScaleBreaksChanged,
		TrnAnnotationsChanged,
		TrnHolidaysChanged,
		TrnExactWorkdaysChanged,
		AxisXDefaultTitle,
		AxisYDefaultTitle,
		DefaultWizardPageLabel,
		MenuItemAdd,
		MenuItemInsert,
		MenuItemDelete,
		MenuItemClear,
		MenuItemMoveUp,
		MenuItemMoveDown,
		WizAutoCreatedSeries,
		WizSpecifyDataFilters,
		WizDataFiltersDisabled,
		WizDataFiltersEntered,
		WizBackImageFileNameFilter,
		WizNoBackImage,
		WizConstructionGroupName,
		WizPresentationGroupName,
		WizChartTypePageName,
		WizAppearancePageName,
		WizSeriesPageName,
		WizDataPageName,
		WizChartPageName,
		WizDiagramPageName,
		WizAxesPageName,
		WizAxesAlignmentNear,
		WizAxesAlignmentFar,
		WizAxesAlignmentZero,
		WizPanesPageName,
		WizAnnotationsPageName,
		WizSeriesViewPageName,
		WizSeriesLabelsPageName,
		WizChartTitlesPageName,
		WizLegendPageName,
		WizSeriesPointPageName,
		WizSeriesDataBindingPageName,
		WizPivotGridDataSourcePageName,
		WizChartTypePageDescription,
		WizAppearancePageDescription,
		WizSeriesPageDescription,
		WizDataPageDescription,
		WizChartPageDescription,
		WizDiagramPageDescription,
		WizAxesPageDescription,
		WizPanesPageDescription,
		WizAnnotationsPageDescription,
		WizSeriesViewPageDescription,
		WizSeriesLabelsPageDescription,
		WizChartTitlesPageDescription,
		WizLegendPageDescription,
		WizFormTitle,
		WizHatchMin,
		WizHatchHorizontal,
		WizHatchVertical,
		WizHatchForwardDiagonal,
		WizHatchBackwardDiagonal,
		WizHatchMax,
		WizHatchCross,
		WizHatchLargeGrid,
		WizHatchDiagonalCross,
		WizHatchPercent05,
		WizHatchPercent10,
		WizHatchPercent20,
		WizHatchPercent25,
		WizHatchPercent30,
		WizHatchPercent40,
		WizHatchPercent50,
		WizHatchPercent60,
		WizHatchPercent70,
		WizHatchPercent75,
		WizHatchPercent80,
		WizHatchPercent90,
		WizHatchLightDownwardDiagonal,
		WizHatchLightUpwardDiagonal,
		WizHatchDarkDownwardDiagonal,
		WizHatchDarkUpwardDiagonal,
		WizHatchWideDownwardDiagonal,
		WizHatchWideUpwardDiagonal,
		WizHatchLightVertical,
		WizHatchLightHorizontal,
		WizHatchNarrowVertical,
		WizHatchNarrowHorizontal,
		WizHatchDarkVertical,
		WizHatchDarkHorizontal,
		WizHatchDashedDownwardDiagonal,
		WizHatchDashedUpwardDiagonal,
		WizHatchDashedHorizontal,
		WizHatchDashedVertical,
		WizHatchSmallConfetti,
		WizHatchLargeConfetti,
		WizHatchZigZag,
		WizHatchWave,
		WizHatchDiagonalBrick,
		WizHatchHorizontalBrick,
		WizHatchWeave,
		WizHatchPlaid,
		WizHatchDivot,
		WizHatchDottedGrid,
		WizHatchDottedDiamond,
		WizHatchShingle,
		WizHatchTrellis,
		WizHatchSphere,
		WizHatchSmallGrid,
		WizHatchSmallCheckerBoard,
		WizHatchLargeCheckerBoard,
		WizHatchOutlinedDiamond,
		WizHatchSolidDiamond,
		WizDataMemberNoneString,
		WizPositionLeftColumn,
		WizPositionLeft,
		WizPositionCenter,
		WizPositionRight,
		WizPositionRightColumn,
		WizGradientTopToBottom,
		WizGradientBottomToTop,
		WizGradientLeftToRight,
		WizGradientRightToLeft,
		WizGradientTopLeftToBottomRight,
		WizGradientBottomRightToTopLeft,
		WizGradientTopRightToBottomLeft,
		WizGradientBottomLeftToTopRight,
		WizGradientToCenter,
		WizGradientFromCenter,
		WizGradientToCenterHorizontal,
		WizGradientFromCenterHorizontal,
		WizGradientToCenterVertical,
		WizGradientFromCenterVertical,
		WizValueLevelValue,
		WizValueLevelValue_1,
		WizValueLevelValue_2,
		WizValueLevelLow,
		WizValueLevelHigh,
		WizValueLevelOpen,
		WizValueLevelClose,
		WizDateTimeMeasureUnitYear,
		WizDateTimeMeasureUnitQuarter,
		WizDateTimeMeasureUnitMonth,
		WizDateTimeMeasureUnitWeek,
		WizDateTimeMeasureUnitDay,
		WizDateTimeMeasureUnitHour,
		WizDateTimeMeasureUnitMinute,
		WizDateTimeMeasureUnitSecond,
		WizDateTimeMeasureUnitMillisecond,
		WizDateTimeGridAlignmentAuto,
		WizNumericMeasureUnitOnes,
		WizNumericMeasureUnitTens,
		WizNumericMeasureUnitHundreds,
		WizNumericMeasureUnitThousands,
		WizNumericMeasureUnitMillions,
		WizNumericMeasureUnitBillions,
		WizNumericMeasureUnitCustom,
		WizNumericGridAlignmentAuto,
		WizAggregateFunctionFinancial,
		WizAggregateFunctionNone,
		WizAggregateFunctionSum,
		WizAggregateFunctionAverage,
		WizAggregateFunctionCount,
		WizAggregateFunctionMaximum,
		WizAggregateFunctionMinimum,
		WizScaleModeManual,
		WizScaleModeAutomatic,
		WizScaleModeContinuous,
		WizResolveOverlappingModeNone,
		WizResolveOverlappingModeDefault,
		WizResolveOverlappingModeHideOverlapping,
		WizResolveOverlappingModeJustifyAroundPoint,
		WizResolveOverlappingModeJustifyAllAroundPoints,
		WizAxisLabelResolveOverlappingModeNone,
		WizAxisLabelResolveOverlappingModeHideOverlapping,
		WizErrorMessageTitle,
		WizInvalidBackgroundImage,
		WizScrollBarAlignmentNear,
		WizScrollBarAlignmentFar,
		WizDateTimeScaleModeManual,
		WizDateTimeScaleModeAutomaticAverage,
		WizDateTimeScaleModeAutomaticIntegral,
		WizScaleBreakStyleRagged,
		WizScaleBreakStyleStraight,
		WizScaleBreakStyleWaved,
		WizBarSeriesLabelPositionAuto,
		WizBarSeriesLabelPositionTop,
		WizBarSeriesLabelPositionCenter,
		WizBarSeriesLabelPositionTopInside,
		WizBarSeriesLabelPositionBottomInside,
		WizPieSeriesLabelPositionInside,
		WizPieSeriesLabelPositionOutside,
		WizPieSeriesLabelPositionRadial,
		WizPieSeriesLabelPositionTangent,
		WizPieSeriesLabelPositionTwoColumns,
		WizBubbleLabelValueToDisplayValue,
		WizBubbleLabelValueToDisplayWeight,
		WizBubbleLabelValueToDisplayValueAndWeight,
		WizBubbleLabelPositionCenter,
		WizBubbleLabelPositionOutside,
		WizFunnelSeriesLabelPositionCenter,
		WizFunnelSeriesLabelPositionLeft,
		WizFunnelSeriesLabelPositionLeftColumn,
		WizFunnelSeriesLabelPositionRight,
		WizFunnelSeriesLabelPositionRightColumn,
		WizSeriesLabelTextOrientationHorizontal,
		WizSeriesLabelTextOrientationTopToBottom,
		WizSeriesLabelTextOrientationBottomToTop,
		WizBar3DModelBox,
		WizBar3DModelCylinder,
		WizBar3DModelCone,
		WizBar3DModelPyramid,
		WizEnableScrollingTrue,
		WizEnableScrollingFalse,
		WizEnableZoomingTrue,
		WizEnableZoomingFalse,
		WizShapeKindRectangle,
		WizShapeKindRoundedRectangle,
		WizShapeKindEllipse,
		WizAnnotationConnectorStyleArrow,
		WizAnnotationConnectorStyleLine,
		WizAnnotationConnectorStyleNone,
		WizAnnotationConnectorStyleNotchedArrow,
		WizAnnotationConnectorStyleTail,
		WizStringAlignmentCenter,
		WizStringAlignmentNear,
		WizStringAlignmentFar,
		WizChartImageSizeModeAutoSize,
		WizChartImageSizeModeStretch,
		WizChartImageSizeModeTile,
		WizChartImageSizeModeZoom,
		WizDockCornerLeftTop,
		WizDockCornerLeftBottom,
		WizDockCornerRightTop,
		WizDockCornerRightBottom,
		WizShapePositionKindFree,
		WizShapePositionKindRelative,
		WizAnchorPointChart,
		WizAnchorPointPane,
		WizAnchorPointSeriesPoint,
		WizIndentUndefined,
		WizIndentDefault,
		SvnSideBySideBar,
		SvnStackedBar,
		SvnFullStackedBar,
		SvnSideBySideStackedBar,
		SvnSideBySideFullStackedBar,
		SvnPie,
		SvnFunnel,
		SvnDoughnut,
		SvnNestedDoughnut,
		SvnPoint,
		SvnBubble,
		SvnLine,
		SvnStackedLine,
		SvnFullStackedLine,
		SvnStepLine,
		SvnSpline,
		SvnScatterLine,
		SvnSpline3D,
		SvnArea,
		SvnStepArea,
		SvnSplineArea,
		SvnStackedArea,
		SvnSplineStackedArea,
		SvnFullStackedArea,
		SvnSplineFullStackedArea,
		SvnRangeArea,
		SvnRangeArea3D,
		SvnSpline3DArea,
		SvnSplineAreaStacked3D,
		SvnSplineAreaFullStacked3D,
		SvnStock,
		SvnCandleStick,
		SvnSideBySideRangeBar,
		SvnOverlappedRangeBar,
		SvnSideBySideGantt,
		SvnOverlappedGantt,
		SvnSideBySideBar3D,
		SvnStackedBar3D,
		SvnFullStackedBar3D,
		SvnManhattanBar,
		SvnSideBySideStackedBar3D,
		SvnSideBySideFullStackedBar3D,
		SvnPie3D,
		SvnDoughnut3D,
		SvnFunnel3D,
		SvnLine3D,
		SvnStackedLine3D,
		SvnFullStackedLine3D,
		SvnStepLine3D,
		SvnArea3D,
		SvnStackedArea3D,
		SvnFullStackedArea3D,
		SvnStepArea3D,
		SvnRadarPoint,
		SvnRadarLine,
		SvnScatterRadarLine,
		SvnRadarArea,
		SvnPolarPoint,
		SvnPolarLine,
		SvnScatterPolarLine,
		SvnPolarArea,
		SvnSwiftPlot,
		IndRegressionLine,
		IndTrendLine,
		IndFibonacciIndicator,
		IndSimpleMovingAverage,
		IndExponentialMovingAverage,
		IndWeightedMovingAverage,
		IndTriangularMovingAverage,
		IndStandardDeviation,
		IndAverageTrueRange,
		IndCommodityChannelIndex,
		IndDetrendedPriceOscillator,
		IndEaseOfMovement,
		IndMassIndex,
		IndMedianPrice,
		IndMovingAverageConvergenceDivergence,
		IndRateOfChange,
		IndRelativeStrengthIndex,
		IndTripleExponentialMovingAverageTema,
		IndTypicalPrice,
		IndChaikinsVolatility,
		IndWeightedClose,
		IndWilliamsR,
		IndTripleExponentialMovingAverageTrix,
		IndBollingerBands,
		AppDefault,
		AppNatureColors,
		AppPastelKit,
		AppInAFog,
		AppTerracottaPie,
		AppNorthernLights,
		AppChameleon,
		AppTheTrees,
		AppLight,
		AppGray,
		AppDark,
		AppDarkFlat,
		PltDefault,
		PltNatureColors,
		PltPastelKit,
		PltInAFog,
		PltTerracottaPie,
		PltNorthernLights,
		PltChameleon,
		PltTheTrees,
		PltMixed,
		PltOffice,
		PltBlackAndWhite,
		PltGrayscale,
		PltApex,
		PltAspect,
		PltCivic,
		PltConcourse,
		PltEquity,
		PltFlow,
		PltFoundry,
		PltMedian,
		PltMetro,
		PltModule,
		PltOpulent,
		PltOriel,
		PltOrigin,
		PltPaper,
		PltSolstice,
		PltTechnic,
		PltTrek,
		PltUrban,
		PltVerve,
		PltIndDefault,
		PltOffice2013,
		PltBlueWarm,
		PltBlue,
		PltBlueII,
		PltBlueGreen,
		PltGreen,
		PltGreenYellow,
		PltYellow,
		PltYellowOrange,
		PltOrange,
		PltOrangeRed,
		PltRedOrange,
		PltRed,
		PltRedViolet,
		PltViolet,
		PltVioletII,
		PltMarquee,
		PltSlipstream,
		DefaultMinValue,
		DefaultMaxValue,
		IncompatibleSeriesView,
		InvisibleSeriesView,
		IncompatibleSeriesHeader,
		IncompatibleSeriesMessage,
		PrimaryAxisXName,
		PrimaryAxisYName,
		IOCaption,
		IODeleteAxis,
		IODeleteDefaultPane,
		PrintSizeModeNone,
		PrintSizeModeStretch,
		PrintSizeModeZoom,
		StyleAllColors,
		StyleColorNumber,
		DefaultPaneName,
		QuarterFormat,
		OthersArgument,
		ExplodedPointsDialogExplodedColumn,
		ScaleTypeAuto,
		ScaleTypeQualitative,
		ScaleTypeNumerical,
		ScaleTypeDateTime,
		FunctionNameMin,
		FunctionNameMax,
		FunctionNameSum,
		FunctionNameAverage,
		FunctionNameCount,
		FunctionArgumentName,
		TitleSummaryFunction,
		PanesVisibilityDialogVisibleColumn,
		PanesVisibilityDialogPanesColumn,
		FibonacciArcs,
		FibonacciFans,
		FibonacciRetracement,
		AnnotationChartAnchorPoint,
		AnnotationPaneAnchorPoint,
		AnnotationSeriesPointAnchorPoint,
		AnnotationFreePosition,
		AnnotationRelativePosition,
		TextAnnotation,
		ImageAnnotation,
		ToolTipMousePosition,
		ToolTipFreePosition,
		ToolTipRelativePosition,
		CrosshairLabelMousePosition,
		CrosshairLabelFreePosition,
		IncorrectSeriesCollectionToolTipText,
		IncorrectDiagramTypeToolTipText,
		DefaultSmallChartText,
		ChartControlDockTarget,
		Holidays,
		ExactWorkdays,
		Holiday,
		Workday,
		HolidaysImportFilter,
		AllHolidays,
		AlternateTextPlaceholder,
		AlternateTextSeriesPlaceholder,
		AlternateTextSeriesText,
		ColumnAnnotations,
		ColumnColor,
		ColumnLinks,
		InvalidRangeControlText,
		CmdEmptyMenuCaption,
		CmdEmptyDescription,
		CmdCreateBarChartMenuCaption,
		CmdCreateBarChartDescription,
		CmdCreateBar3DChartMenuCaption,
		CmdCreateBar3DChartDescription,
		CmdCreateFullStackedBarChartMenuCaption,
		CmdCreateFullStackedBarChartDescription,
		CmdCreateFullStackedBar3DChartMenuCaption,
		CmdCreateFullStackedBar3DChartDescription,
		CmdCreateSideBySideFullStackedBarChartMenuCaption,
		CmdCreateSideBySideFullStackedBarChartDescription,
		CmdCreateSideBySideFullStackedBar3DChartMenuCaption,
		CmdCreateSideBySideFullStackedBar3DChartDescription,
		CmdCreateSideBySideStackedBarChartMenuCaption,
		CmdCreateSideBySideStackedBarChartDescription,
		CmdCreateSideBySideStackedBar3DChartMenuCaption,
		CmdCreateSideBySideStackedBar3DChartDescription,
		CmdCreateStackedBarChartMenuCaption,
		CmdCreateStackedBarChartDescription,
		CmdCreateStackedBar3DChartMenuCaption,
		CmdCreateStackedBar3DChartDescription,
		CmdCreateManhattanBarChartMenuCaption,
		CmdCreateManhattanBarChartDescription,
		CmdCreateRangeBarChartMenuCaption,
		CmdCreateRangeBarChartDescription,
		CmdCreateSideBySideRangeBarChartMenuCaption,
		CmdCreateSideBySideRangeBarChartDescription,
		CmdCreateConeBar3DChartMenuCaption,
		CmdCreateConeBar3DChartDescription,
		CmdCreateConeFullStackedBar3DChartMenuCaption,
		CmdCreateConeFullStackedBar3DChartDescription,
		CmdCreateConeSideBySideFullStackedBar3DChartMenuCaption,
		CmdCreateConeSideBySideFullStackedBar3DChartDescription,
		CmdCreateConeSideBySideStackedBar3DChartMenuCaption,
		CmdCreateConeSideBySideStackedBar3DChartDescription,
		CmdCreateConeStackedBar3DChartMenuCaption,
		CmdCreateConeStackedBar3DChartDescription,
		CmdCreateConeManhattanBarChartMenuCaption,
		CmdCreateConeManhattanBarChartDescription,
		CmdCreateCylinderBar3DChartMenuCaption,
		CmdCreateCylinderBar3DChartDescription,
		CmdCreateCylinderFullStackedBar3DChartMenuCaption,
		CmdCreateCylinderFullStackedBar3DChartDescription,
		CmdCreateCylinderSideBySideFullStackedBar3DChartMenuCaption,
		CmdCreateCylinderSideBySideFullStackedBar3DChartDescription,
		CmdCreateCylinderSideBySideStackedBar3DChartMenuCaption,
		CmdCreateCylinderSideBySideStackedBar3DChartDescription,
		CmdCreateCylinderStackedBar3DChartMenuCaption,
		CmdCreateCylinderStackedBar3DChartDescription,
		CmdCreateCylinderManhattanBarChartMenuCaption,
		CmdCreateCylinderManhattanBarChartDescription,
		CmdCreatePyramidBar3DChartMenuCaption,
		CmdCreatePyramidBar3DChartDescription,
		CmdCreatePyramidFullStackedBar3DChartMenuCaption,
		CmdCreatePyramidFullStackedBar3DChartDescription,
		CmdCreatePyramidSideBySideFullStackedBar3DChartMenuCaption,
		CmdCreatePyramidSideBySideFullStackedBar3DChartDescription,
		CmdCreatePyramidSideBySideStackedBar3DChartMenuCaption,
		CmdCreatePyramidSideBySideStackedBar3DChartDescription,
		CmdCreatePyramidStackedBar3DChartMenuCaption,
		CmdCreatePyramidStackedBar3DChartDescription,
		CmdCreatePyramidManhattanBarChartMenuCaption,
		CmdCreatePyramidManhattanBarChartDescription,
		CmdCreateAreaChartMenuCaption,
		CmdCreateAreaChartDescription,
		CmdCreateArea3DChartMenuCaption,
		CmdCreateArea3DChartDescription,
		CmdCreateFullStackedAreaChartMenuCaption,
		CmdCreateFullStackedAreaChartDescription,
		CmdCreateFullStackedArea3DChartMenuCaption,
		CmdCreateFullStackedArea3DChartDescription,
		CmdCreateFullStackedSplineAreaChartMenuCaption,
		CmdCreateFullStackedSplineAreaChartDescription,
		CmdCreateFullStackedSplineArea3DChartMenuCaption,
		CmdCreateFullStackedSplineArea3DChartDescription,
		CmdCreateSplineAreaChartMenuCaption,
		CmdCreateSplineAreaChartDescription,
		CmdCreateSplineArea3DChartMenuCaption,
		CmdCreateSplineArea3DChartDescription,
		CmdCreateStackedAreaChartMenuCaption,
		CmdCreateStackedAreaChartDescription,
		CmdCreateStackedArea3DChartMenuCaption,
		CmdCreateStackedArea3DChartDescription,
		CmdCreateStackedSplineAreaChartMenuCaption,
		CmdCreateStackedSplineAreaChartDescription,
		CmdCreateStackedSplineArea3DChartMenuCaption,
		CmdCreateStackedSplineArea3DChartDescription,
		CmdCreateStepAreaChartMenuCaption,
		CmdCreateStepAreaChartDescription,
		CmdCreateStepArea3DChartMenuCaption,
		CmdCreateStepArea3DChartDescription,
		CmdCreateRangeAreaChartMenuCaption,
		CmdCreateRangeAreaChartDescription,
		CmdCreateRangeArea3DChartMenuCaption,
		CmdCreateRangeArea3DChartDescription,
		CmdCreateStockChartMenuCaption,
		CmdCreateStockChartDescription,
		CmdCreateCandleStickChartMenuCaption,
		CmdCreateCandleStickChartDescription,
		CmdCreateFunnelChartMenuCaption,
		CmdCreateFunnelChartDescription,
		CmdCreateFunnel3DChartMenuCaption,
		CmdCreateFunnel3DChartDescription,
		CmdCreateGanttChartMenuCaption,
		CmdCreateGanttChartDescription,
		CmdCreateSideBySideGanttChartMenuCaption,
		CmdCreateSideBySideGanttChartDescription,
		CmdCreateLineChartMenuCaption,
		CmdCreateLineChartDescription,
		CmdCreateLine3DChartMenuCaption,
		CmdCreateLine3DChartDescription,
		CmdCreateFullStackedLineChartMenuCaption,
		CmdCreateFullStackedLineChartDescription,
		CmdCreateFullStackedLine3DChartMenuCaption,
		CmdCreateFullStackedLine3DChartDescription,
		CmdCreateScatterLineChartMenuCaption,
		CmdCreateScatterLineChartDescription,
		CmdCreateSplineChartMenuCaption,
		CmdCreateSplineChartDescription,
		CmdCreateSpline3DChartMenuCaption,
		CmdCreateSpline3DChartDescription,
		CmdCreateStackedLineChartMenuCaption,
		CmdCreateStackedLineChartDescription,
		CmdCreateStackedLine3DChartMenuCaption,
		CmdCreateStackedLine3DChartDescription,
		CmdCreateStepLineChartMenuCaption,
		CmdCreateStepLineChartDescription,
		CmdCreateStepLine3DChartMenuCaption,
		CmdCreateStepLine3DChartDescription,
		CmdCreatePieChartMenuCaption,
		CmdCreatePieChartDescription,
		CmdCreatePie3DChartMenuCaption,
		CmdCreatePie3DChartDescription,
		CmdCreateDoughnutChartMenuCaption,
		CmdCreateDoughnutChartDescription,
		CmdCreateNestedDoughnutChartMenuCaption,
		CmdCreateNestedDoughnutChartDescription,
		CmdCreateDoughnut3DChartMenuCaption,
		CmdCreateDoughnut3DChartDescription,
		CmdCreatePointChartMenuCaption,
		CmdCreatePointChartDescription,
		CmdCreateBubbleChartMenuCaption,
		CmdCreateBubbleChartDescription,
		CmdCreateRadarPointChartMenuCaption,
		CmdCreateRadarPointChartDescription,
		CmdCreateRadarLineChartMenuCaption,
		CmdCreateRadarLineChartDescription,
		CmdCreateScatterRadarLineChartMenuCaption,
		CmdCreateScatterRadarLineChartDescription,
		CmdCreateRadarAreaChartMenuCaption,
		CmdCreateRadarAreaChartDescription,
		CmdCreatePolarPointChartMenuCaption,
		CmdCreatePolarPointChartDescription,
		CmdCreatePolarLineChartMenuCaption,
		CmdCreatePolarLineChartDescription,
		CmdCreateScatterPolarLineChartMenuCaption,
		CmdCreateScatterPolarLineChartDescription,
		CmdCreatePolarAreaChartMenuCaption,
		CmdCreatePolarAreaChartDescription,
		CmdCreateRotatedBarChartMenuCaption,
		CmdCreateRotatedBarChartDescription,
		CmdCreateRotatedFullStackedBarChartMenuCaption,
		CmdCreateRotatedFullStackedBarChartDescription,
		CmdCreateRotatedSideBySideFullStackedBarChartMenuCaption,
		CmdCreateRotatedSideBySideFullStackedBarChartDescription,
		CmdCreateRotatedSideBySideStackedBarChartMenuCaption,
		CmdCreateRotatedSideBySideStackedBarChartDescription,
		CmdCreateRotatedStackedBarChartMenuCaption,
		CmdCreateRotatedStackedBarChartDescription,
		CmdPrintMenuCaption,
		CmdPrintDescription,
		CmdPrintPreviewMenuCaption,
		CmdPrintPreviewDescription,
		CmdExportPlaceHolderMenuCaption,
		CmdExportPlaceHolderDescription,
		CmdExportToPDFMenuCaption,
		CmdExportToPDFDescription,
		CmdExportToHTMLMenuCaption,
		CmdExportToHTMLDescription,
		CmdExportToMHTMenuCaption,
		CmdExportToMHTDescription,
		CmdExportToXLSMenuCaption,
		CmdExportToXLSDescription,
		CmdExportToXLSXMenuCaption,
		CmdExportToXLSXDescription,
		CmdExportToRTFMenuCaption,
		CmdExportToRTFDescription,
		CmdExportToImagePlaceHolderMenuCaption,
		CmdExportToBMPMenuCaption,
		CmdExportToBMPDescription,
		CmdExportToGIFMenuCaption,
		CmdExportToGIFDescription,
		CmdExportToJPEGMenuCaption,
		CmdExportToJPEGDescription,
		CmdExportToPNGMenuCaption,
		CmdExportToPNGDescription,
		CmdExportToTIFFMenuCaption,
		CmdExportToTIFFDescription,
		CmdRunWizardMenuCaption,
		CmdRunWizardDescription,
		CmdSaveAsTemplateMenuCaption,
		CmdSaveAsTemplateDescription,
		CmdLoadTemplateMenuCaption,
		CmdLoadTemplateDescription,
		CmdCreateBarChartPlaceHolderMenuCaption,
		CmdCreateBarChartPlaceHolderDescription,
		CmdCreateAreaChartPlaceHolderMenuCaption,
		CmdCreateAreaChartPlaceHolderDescription,
		CmdCreatePieChartPlaceHolderMenuCaption,
		CmdCreatePieChartPlaceHolderDescription,
		CmdCreateLineChartPlaceHolderMenuCaption,
		CmdCreateLineChartPlaceHolderDescription,
		CmdCreateOtherSeriesTypesChartPlaceHolderMenuCaption,
		CmdCreateOtherSeriesTypesChartPlaceHolderDescription,
		CmdCreateRotatedBarChartPlaceHolderMenuCaption,
		CmdCreateRotatedBarChartPlaceHolderDescription,
		CmdChangePalettePlaceHolderMenuCaption,
		CmdChangePalettePlaceHolderDescription,
		CmdChangeAppearancePlaceHolderMenuCaption,
		CmdChangeAppearancePlaceHolderDescription,
		CmdColumn2DGroupPlaceHolderMenuCaption,
		CmdColumn3DGroupPlaceHolderMenuCaption,
		CmdColumnCylinderGroupPlaceHolderMenuCaption,
		CmdColumnConeGroupPlaceHolderMenuCaption,
		CmdColumnPyramidGroupPlaceHolderMenuCaption,
		CmdLine2DGroupPlaceHolderMenuCaption,
		CmdLine3DGroupPlaceHolderMenuCaption,
		CmdPie2DGroupPlaceHolderMenuCaption,
		CmdPie3DGroupPlaceHolderMenuCaption,
		CmdBar2DGroupPlaceHolderMenuCaption,
		CmdArea2DGroupPlaceHolderMenuCaption,
		CmdArea3DGroupPlaceHolderMenuCaption,
		CmdPointGroupPlaceHolderMenuCaption,
		CmdFunnelGroupPlaceHolderMenuCaption,
		CmdFinancialGroupPlaceHolderMenuCaption,
		CmdRadarGroupPlaceHolderMenuCaption,
		CmdPolarGroupPlaceHolderMenuCaption,
		CmdRangeGroupPlaceHolderMenuCaption,
		CmdGanttGroupPlaceHolderMenuCaption,
		RibbonOtherPageCaption,
		RibbonDesignPageCaption,
		RibbonAppearanceGroupCaption,
		RibbonTypesGroupCaption,
		RibbonWizardGroupCaption,
		RibbonTemplatesGroupCaption,
		RibbonPrintExportGroupCaption,
		RibbonPageCategoryCaption,
		ArgumentPatternDescription,
		ValuePatternDescription,
		SeriesNamePatternDescription,
		StackedGroupPatternDescription,
		Value1PatternDescription,
		Value2PatternDescription,
		WeightPatternDescription,
		HighValuePatternDescription,
		LowValuePatternDescription,
		OpenValuePatternDescription,
		CloseValuePatternDescription,
		PercentValuePatternDescription,
		PointHintPatternDescription,
		ValueDurationPatternDescription,
		InvalidPlaceholder,
		ErrorTitle,
		PatternEditorPreviewCaption,
		MsgIncorrectDoubleValue,
		MsgAttemptToSetScaleModeForAxisY,
		MsgPaletteEditorTitle,
		MsgPaletteEditorInvalidFile,
		VerbDesigner,
		VerbDesignerDescription,
		CmdRunDesignerDescription,
		CmdRunDesignerMenuCaption,
		RibbonDesignerGroupCaption,
	}
	#endregion
	public class ChartResLocalizer : XtraResXLocalizer<ChartStringId> {
		public ChartResLocalizer() : base(new ChartLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraCharts.LocalizationRes", typeof(ChartResLocalizer).Assembly);
		}
	}
	public class ChartLocalizer : XtraLocalizer<ChartStringId> {
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartLocalizerActive")]
#endif
		public new static XtraLocalizer<ChartStringId> Active {
			get { return XtraLocalizer<ChartStringId>.Active; }
			set { XtraLocalizer<ChartStringId>.Active = value; }
		}
		static ChartLocalizer() {
			var defaultLocalizerProvider = new DefaultActiveLocalizerProvider<ChartStringId>(CreateDefaultLocalizer());
			SetActiveLocalizerProvider(defaultLocalizerProvider);
		}
		protected override void PopulateStringTable() {
			AddString(ChartStringId.SeriesPrefix, "Series ");
			AddString(ChartStringId.PalettePrefix, "Palette ");
			AddString(ChartStringId.XYDiagramPanePrefix, "Pane ");
			AddString(ChartStringId.SecondaryAxisXPrefix, "Secondary AxisX ");
			AddString(ChartStringId.SecondaryAxisYPrefix, "Secondary AxisY ");
			AddString(ChartStringId.ConstantLinePrefix, "Constant Line ");
			AddString(ChartStringId.CustomAxisLabelPrefix, "Label ");
			AddString(ChartStringId.ScaleBreakPrefix, "Scale Break ");
			AddString(ChartStringId.AnnotationPrefix, "Annotation ");
			AddString(ChartStringId.ImageAnnotationPrefix, "Image Annotation ");
			AddString(ChartStringId.TextAnnotationPrefix, "Text Annotation ");
			AddString(ChartStringId.StripPrefix, "Strip ");
			AddString(ChartStringId.StackedGroupPrefix, "Group");
			AddString(ChartStringId.ArgumentMember, "Argument");
			AddString(ChartStringId.ValueMember, "Value");
			AddString(ChartStringId.WeightMember, "Weight");
			AddString(ChartStringId.LowValueMember, "Low");
			AddString(ChartStringId.HighValueMember, "High");
			AddString(ChartStringId.OpenValueMember, "Open");
			AddString(ChartStringId.CloseValueMember, "Close");
			AddString(ChartStringId.AutocreatedSeriesName, "Auto-created Series");
			AddString(ChartStringId.DefaultDataFilterName, "DataFilter");
			AddString(ChartStringId.DefaultChartTitle, "Chart Title");
			AddString(ChartStringId.DefaultAnnotation, "Annotation");
			AddString(ChartStringId.DefaultSeriesPointFilterName, "SeriesPointFilter");
			AddString(ChartStringId.MsgEquallySpacedItemsNotUsable, "This property can't be used if the Direction property is set to {0}.");
			AddString(ChartStringId.MsgSeriesViewDoesNotExist, "The {0} SeriesView doesn't exist.");
			AddString(ChartStringId.MsgEmptyArrayOfValues, "The array of values is empty.");
			AddString(ChartStringId.MsgIncorrectArrayOfValues, "The array of values must contain either numerical or date-time values.");
			AddString(ChartStringId.MsgIncompatibleArgument, "An argument {0} isn't compatible with the current argument scale type.");
			AddString(ChartStringId.MsgIncompatibleValue, "A value {0} isn't compatible with the current value scale type.");
			AddString(ChartStringId.MsgItemNotInCollection, "The collection doesn't contain the specified item.");
			AddString(ChartStringId.MsgIncompatiblePointType, "The type of the \"{0}\" point isn't compatible with the {1} scale.");
			AddString(ChartStringId.MsgIncompatibleArgumentDataMember, "The type of the \"{0}\" argument data member isn't compatible with the {1} scale.");
			AddString(ChartStringId.MsgIncompatibleValueDataMember, "The type of the \"{0}\" value data member isn't compatible with the {1} scale.");
			AddString(ChartStringId.MsgDesignTimeOnlySetting, "This property can't be customized at runtime.");
			AddString(ChartStringId.MsgInvalidDataSource, "Invalid datasource type (no supported interfaces are implemented).");
			AddString(ChartStringId.MsgIncorrectDataMember, "The datasource doesn't contain a datamember with the \"{0}\" name.");
			AddString(ChartStringId.MsgInvalidSortingKey, "It's impossible to set the sorting key's value to {0}.");
			AddString(ChartStringId.MsgInvalidFilterCondition, "The {0} condition can't be applied to the \"{1}\" data.");
			AddString(ChartStringId.MsgIncorrectDataAdapter, "The {0} object isn't a data adapter.");
			AddString(ChartStringId.MsgDataSnapshot, "The data snapshot operation is complete. All series data now statically persist in the chart.\nThis also means that now the chart is in unbound mode.\n\nNOTE: You can undo this operation by pressing Ctrl+Z in the Visual Studio designer.");
			AddString(ChartStringId.MsgModifyDefaultPaletteError, "The palette is default and then can't be modified.");
			AddString(ChartStringId.MsgAddExistingPaletteError, "The palette with the {0} name already exists in the repository.");
			AddString(ChartStringId.MsgInternalPropertyChangeError, "This property is intended for internal use only. You're not allowed to change its value.");
			AddString(ChartStringId.MsgPaletteNotFound, "The chart doesn't contain a palette with the {0} name.");
			AddString(ChartStringId.MsgLabelSettingRuntimeError, "The \"Label\" property can't be set at runtime.");
			AddString(ChartStringId.MsgPointOptionsSettingRuntimeError, "The \"PointOptions\" property can't be set at runtime.");
			AddString(ChartStringId.MsgLegendPointOptionsSettingRuntimeError, "The \"LegendPointOptions\" property can't be set at runtime.");
			AddString(ChartStringId.MsgSynchronizePointOptionsSettingRuntimeError, "The \"SynchronizePointOptions\" property can't be set at runtime.");
			AddString(ChartStringId.MsgIncorrectNumericPrecision, "The precision should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectAxisThickness, "The axis thickness should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectBarWidth, "The bar width should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectBarDepth, "The bar depth should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectLineWidth, "The line width should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectAreaWidth, "The area width should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectBorderThickness, "The border width should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectChartTitleIndent, "The title indent should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectMaxLineCount, "The maximum line count should be greater than or equal to 0 and less than or equal to 20.");
			AddString(ChartStringId.MsgIncorrectLegendMarkerSize, "The legend marker size should be greater than 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectLegendHorizontalIndent, "The legend horizontal indent should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectLegendVerticalIndent, "The legend vertical indent should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectLegendTextOffset, "The legend text offset should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectMarkerSize, "The marker size should be greater than 1.");
			AddString(ChartStringId.MsgIncorrectMarkerStarPointCount, "The number of star points should be greater than 3 and less than 101.");
			AddString(ChartStringId.MsgIncorrectPieSeriesLabelColumnIndent, "The column indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectRangeBarSeriesLabelIndent, "The indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectPercentPrecision, "The precision of the percent value should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectPercentageAccuracy, "The percentage accuracy should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectSeriesLabelLineLength, "The line length should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectStripLimitAxisValue, "The AxisValue property cannot be set to null for the StripLimit object.");
			AddString(ChartStringId.MsgIncorrectStripConstructorParameters, "The minimum and maximum limits of the Strip can not be the same.");
			AddString(ChartStringId.MsgIncorrectStripMinLimit, "The min limit of the strip should be less than the max limit.");
			AddString(ChartStringId.MsgIncorrectStripMaxLimit, "The max limit of the strip should be greater than the min limit.");
			AddString(ChartStringId.MsgIncorrectLineThickness, "The line thickness should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectShadowSize, "The shadow size should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTickmarkThickness, "The tickmark thickness should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTickmarkLength, "The tickmark length should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTickmarkMinorThickness, "The thickness of the minor tickmark should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTickmarkMinorLength, "The length of the minor tickmark should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectMinorCount, "The number of minor count should be greater than 0 and less than 100.");
			AddString(ChartStringId.MsgIncorrectPercentValue, "The percent value should be greater than or equal to 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgIncorrectPointDistance, "The point distance value should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectSimpleDiagramDimension, "The dimension of the simple diagram should be greater than 0 and less than 100.");
			AddString(ChartStringId.MsgIncorrectStockLevelLineLengthValue, "The stock level line length value should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectReductionColorValue, "The reduction color value can't be empty.");
			AddString(ChartStringId.MsgIncorrectLabelAngle, "The angle of the label should be greater than or equal to -360 and less than or equal to 360.");
			AddString(ChartStringId.MsgIncorrectImageFormat, "Impossible to export a chart to the specified image format.");
			AddString(ChartStringId.MsgIncorrectImageBounds, "Can't create an image for the specified size.");
			AddString(ChartStringId.MsgIncorrectUseImageUrlProperty, "ImageUrl property can be used for the WebChartControl only. Please, use the Image property instead.");
			AddString(ChartStringId.MsgIncorrectValueDataMemberCount, "It's necessary to specify {0} value data members for the current series view.");
			AddString(ChartStringId.MsgIncorrectShapeFillet, "The shape fillet should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectAnnotationHeight, "The annotation height should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectAnnotationWidth, "The annotation width should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTextAnnotationAngle, "The angle of the annotation should be greater than or equal to -360 and less than or equal to 360.");
			AddString(ChartStringId.MsgIncorrectRelativePositionConnectorLength, "The connector length should be greater than or equal to 0 and less than 1000.");
			AddString(ChartStringId.MsgIncorrectRelativePositionAngle, "The angle should be greater than or equal to -360 and less than or equal to 360.");
			AddString(ChartStringId.MsgIncorrectAxisCoordinateAxisValue, "AxisValue can't be set to null for the AxisCoordinate object.");
			AddString(ChartStringId.MsgIncorrectAnnotationZOrder, "The ZOrder should be greater than or equal to 0 and less than 100.");
			AddString(ChartStringId.MsgIncorrectLabelMaxWidth, "The maximum width of the label should be greater than or equal to 0.");
			AddString(ChartStringId.MsgNullPaneAnchorPointPane, "Pane can't be set to null for the PaneAnchorPoint object.");
			AddString(ChartStringId.MsgIncorrectPaneAnchorPointPane, "Can't set the PaneAnchorPoint's pane, because the specified pane isn't default and isn't contained in the diagram's collection of panes.");
			AddString(ChartStringId.MsgNullAxisXCoordinateAxis, "Axis can't be set to null for the AxisXCoordinate object.");
			AddString(ChartStringId.MsgIncorrectAxisXCoordinateAxis, "Can't set the AxisXCoordinate's axis, because the specified axis isn't primary and isn't contained in the diagram's collection of secondary X-axes.");
			AddString(ChartStringId.MsgNullAxisYCoordinateAxis, "Axis can't be set to null for the AxisYCoordinate object.");
			AddString(ChartStringId.MsgIncorrectAxisYCoordinateAxis, "Can't set the AxisYCoordinate's axis, because the specified axis isn't primary and isn't contained in the diagram's collection of secondary Y-axes.");
			AddString(ChartStringId.MsgNullAnchorPointSeriesPoint, "Series point can't be set to null for the SeriesPointAncherPoint object.");
			AddString(ChartStringId.MsgIncorrectAnchorPointSeriesPoint, "Can't set the series point, because it should belong to a series, and the series should be contained in the chart's collection.");
			AddString(ChartStringId.MsgIncorrectFreePositionDockTarget, "An incorrect value is specified. A dock target can only be a pane, or null (meaning the chart control itself).");
			AddString(ChartStringId.MsgPaletteEditingIsNotAllowed, "Editing isn't allowed!");
			AddString(ChartStringId.MsgPaletteDoubleClickToEdit, "Double-click to edit...");
			AddString(ChartStringId.MsgEmptyArgument, "An argument can't be empty.");
			AddString(ChartStringId.MsgInvalidPaletteName, "Can't add a palette which has an empty name (\"\") to the palette repository. Please, specify a name for the palette.");
			AddString(ChartStringId.MsgInvalidScaleType, "The specified value to convert to the scale's internal representation isn't compatible with the current scale type.");
			AddString(ChartStringId.MsgIncorrectTransformationMatrix, "Incorrect transformation matrix.");
			AddString(ChartStringId.MsgIncorrectPerspectiveAngle, "The perspective angle should be greater than or equal to 0 and less than 180.");
			AddString(ChartStringId.MsgIncorrectPieDepth, "The pie depth should be greater than 0 and less than or equal to 100, since its value is measured in percents.");
			AddString(ChartStringId.MsgIncorrectSeriesDistance, "The series distance should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectSeriesDistanceFixed, "The fixed series distance should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectSeriesIndentFixed, "The fixed series indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectPlaneDepthFixed, "The fixed plane depth should be greater than or equal to 1.");
			AddString(ChartStringId.MsgIncorrectGridSpacing, "The grid spacing should be greater than 0.");
			AddString(ChartStringId.MsgIncompatibleArgumentScaleType, "The {0} argument scale type is incompatible with the {1} series view.");
			AddString(ChartStringId.MsgIncompatibleValueScaleType, "The {0} value scale type is incompatible with the {1} series view.");
			AddString(ChartStringId.MsgIncompatibleSummaryFunction, "The '{0}' summary function is incompatible with the {1} scale.");
			AddString(ChartStringId.MsgIncorrectConstantLineAxisValue, "AxisValue can't be set to null for the ConstantLine object.");
			AddString(ChartStringId.MsgIncorrectCustomAxisLabelAxisValue, "AxisValue can't be set to null for the CustomAxisLabel object.");
			AddString(ChartStringId.MsgIncorrectTaskLinkMinIndent, "The task link's minimum indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectArrowWidth, "The arrow width should be always odd and greater than 0.");
			AddString(ChartStringId.MsgIncorrectArrowHeight, "The arrow height should be greater than 0.");
			AddString(ChartStringId.Msg3DRotationToolTip, "Use Ctrl with the left mouse button\nto rotate the chart.");
			AddString(ChartStringId.Msg3DScrollingToolTip, "Use Ctrl with the center (wheel) mouse button\nto scroll the chart.");
			AddString(ChartStringId.Msg2DScrollingToolTip, "Use Ctrl with the left mouse button\nto scroll the chart.");
			AddString(ChartStringId.Msg2DPieExplodingToolTip, "Use Ctrl with the left mouse button\nto explode or collapse slices.");
			AddString(ChartStringId.MsgInvalidZeroAxisAlignment, "The Alignment can't be set to Alignment.Zero for the secondary axis.");
			AddString(ChartStringId.MsgNullSeriesViewAxisX, "The series view's X-axis can't be set to null.");
			AddString(ChartStringId.MsgNullSeriesViewAxisY, "The series view's Y-axis can't be set to null.");
			AddString(ChartStringId.MsgNullSeriesViewPane, "The series view's pane can't be set to null.");
			AddString(ChartStringId.MsgIncorrectIndentFromMarker, "The indent value should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectSeriesViewPane, "Can't set the series view's pane, because the specified pane isn't default and isn't contained in the diagram's collection of panes.");
			AddString(ChartStringId.MsgIncorrectSeriesViewAxisX, "Can't set the series view's X-axis, because the specified axis isn't primary and isn't contained in the diagram's collection of secondary X-axes.");
			AddString(ChartStringId.MsgIncorrectSeriesViewAxisY, "Can't set the series view's Y-axis, because the specified axis isn't primary and isn't contained in the diagram's collection of secondary Y-axes.");
			AddString(ChartStringId.MsgIncorrectIndicatorPane, "Can't set the indicator's pane, because the specified pane isn't default and isn't contained in the diagram's collection of panes.");
			AddString(ChartStringId.MsgIncorrectIndicatorAxisY, "Can't set the indicator's Y-axis, because the specified axis isn't primary and isn't contained in the diagram's collection of secondary Y-axes.");
			AddString(ChartStringId.MsgIncorrectParentSeriesPointOwner, "Owner of the parent series point can't be null and must be of the Series type.");
			AddString(ChartStringId.MsgSeriesViewNotSupportRelations, "This series view doesn't support relations.");
			AddString(ChartStringId.MsgIncorrectChildSeriesPointOwner, "Owner of the child series point can't be null and must be of the Series type.");
			AddString(ChartStringId.MsgIncorrectChildSeriesPointID, "Child series point's ID must be positive or equal to zero.");
			AddString(ChartStringId.MsgIncorrectSeriesOfParentAndChildPoints, "Parent and child points must belong to the same series.");
			AddString(ChartStringId.MsgSelfRelatedSeriesPoint, "Series point can't have a relation to itself.");
			AddString(ChartStringId.MsgSeriesPointRelationAlreadyExists, "The SeriesPointRelations collection already contains this relation.");
			AddString(ChartStringId.MsgChildSeriesPointNotExist, "Child series point with ID equal to {0} doesn't exist.");
			AddString(ChartStringId.MsgRelationChildPointIDNotUnique, "Relation's ChildPointID must be unique.");
			AddString(ChartStringId.MsgSeriesPointIDNotUnique, "Series point's ID must be unique.");
			AddString(ChartStringId.MsgIncorrectFont, "Font can't be null.");
			AddString(ChartStringId.MsgIncorrectScrollBarThickness, "The scroll bar thickness should be greater than or equal to 3 and less than or equal to 25.");
			AddString(ChartStringId.MsgIncorrectZoomPercent, "The zoom percent should be greater than 0 and less than or equal to {0}.");
			AddString(ChartStringId.MsgIncorrectHorizontalScrollPercent, "The horizontal scroll percent should be greater than or equal to -{0} and less than or equal to {0}.");
			AddString(ChartStringId.MsgIncorrectVerticalScrollPercent, "The vertical scroll percent should be greater than or equal to -{0} and less than or equal to {0}.");
			AddString(ChartStringId.MsgIncorrectAnchorPoint, "Anchor Point can't be null.");
			AddString(ChartStringId.MsgIncorrectShapePosition, "Shape Position can't be null.");
			AddString(ChartStringId.MsgIncorrectToolTipPosition, "Tool Tip Position can't be null.");
			AddString(ChartStringId.MsgIncorrectCrosshairPosition, "Crosshair Position can't be null.");
			AddString(ChartStringId.MsgIncorrectPath, "The specified path cannot be resolved: {0}.");
			AddString(ChartStringId.MsgRegisterPageInUnregisterGroup, "The page can't be registered in the unregistered group");
			AddString(ChartStringId.MsgUnregisterPageError, "This page was already unregistered.");
			AddString(ChartStringId.MsgUnregisterGroupError, "This group was already unregistered.");
			AddString(ChartStringId.MsgWizardAbstractPageType, "The {0} is abstract, and so an object of this type can't be instantiated and added as a wizard page.");
			AddString(ChartStringId.MsgWizardIncorrectBasePageType, "The {0} must be inherited from the {1} class.");
			AddString(ChartStringId.MsgWizardNonUniquePageType, "The page of the {0} type is already registered. You can't add more than one page of a particular type.");
			AddString(ChartStringId.MsgWizardNonUniqueGroupName, "The group with the {0} name is already registered.");
			AddString(ChartStringId.MsgOrderArrayLengthMismatch, "The length of the order array isn't equal to the total number of registered elements.");
			AddString(ChartStringId.MsgOrderUnregisteredElementFound, "The unregistered element is found.");
			AddString(ChartStringId.MsgOrderRepeatedElementFound, "The same element is repeated several times in the order array.");
			AddString(ChartStringId.MsgNotChartControl, "The specified object isn't a ChartControl.");
			AddString(ChartStringId.MsgNotBelongingChart, "This control doesn't contain the specified chart.");
			AddString(ChartStringId.MsgInitializeChartNotFound, "The ChartControl isn't found, or there are several charts on this control. To solve the problem, you should handle the WizardPage.InitializePage event and manually specify the chart.");
			AddString(ChartStringId.MsgAddPresentViewType, "The specified view type is already present in the collection.");
			AddString(ChartStringId.MsgAddLastViewType, "You can't add any view type in this collection, because at least one view type must be available in the Wizard.");
			AddString(ChartStringId.MsgCalcHitInfoNotSupported, "Hit testing for 3D Chart Types isn't supported. So, this method is supported for 2D Chart Types only.");
			AddString(ChartStringId.MsgIncorrectAppearanceName, "The chart doesn't contain an appearance with the {0} name.");
			AddString(ChartStringId.MsgIncompatibleByViewType, "the view type");
			AddString(ChartStringId.MsgIncompatibleByArgumentScaleType, "the argument scale type");
			AddString(ChartStringId.MsgIncompatibleByValueScaleType, "the value scale type");
			AddString(ChartStringId.MsgValueMustBeGreaterThenZero, "Value must be equal or greater then 0.");
			AddString(ChartStringId.MsgInvalidExplodedSeriesPoint, "The specified series point doesn't belong to the current Pie series views' collection of series points, and so it can't be added to the collection of exploded points.");
			AddString(ChartStringId.MsgInvalidExplodedModeAdd, "Since the current Pie series view displays the series created using a series template, the specified series point can't be added to the collection of exploded points. You need to use another Explode Mode instead.");
			AddString(ChartStringId.MsgInvalidExplodedModeRemove, "Since the current Pie series view displays the series created using a series template, the specified series point can't be removed from the collection of exploded points. You need to use another Explode Mode instead.");
			AddString(ChartStringId.MsgIncorrectExplodedDistancePercentage, "The exploded distance percentage value should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectPaletteBaseColorNumber, "The palette base color number should be greater than or equal to 0 and less than or equal to the total number of palette colors.");
			AddString(ChartStringId.MsgDenyChangeSeriesPointCollection, "You can't manually change this series point collection, because a chart is bound to data.");
			AddString(ChartStringId.MsgDenyChangeSeriesPointArgument, "You can't manually change the series point's argument, because a chart is bound to data.");
			AddString(ChartStringId.MsgDenyChangeSeriesPointValue, "You can't manually change the series point's value, because a chart is bound to data.");
			AddString(ChartStringId.MsgIncorrectStartAngle, "The start angle value should be greater than or equal to -360 and less than or equal to 360 degrees.");
			AddString(ChartStringId.MsgPolarAxisXRangeChanged, "The range of a polar X-axis can't be changed.");
			AddString(ChartStringId.MsgPolarAxisXGridSpacingChanged, "The grid spacing of a polar X-axis can't be changed.");
			AddString(ChartStringId.MsgPolarAxisXLogarithmic, "The PolarAxisX doesn't support logarithmic mode.");
			AddString(ChartStringId.MsgIncorrectPieArgumentScaleType, "{0} argument scale type cannot be specified, because the existing exploded point filters don't correspond to it.");
			AddString(ChartStringId.MsgIncorrectDoughnutHolePercent, "The doughnut hole percentage should be greater than or equal to 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgIncorrectNestedDoughnutInnerIndent, "The nested doughnut inner indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectNestedDoughnutWeight, "The nested doughnut weight should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectFunnelHolePercent, "The funnel hole percentage should be greater than or equal to 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgIncorrectLineTensionPercent, "The line tension percentage should be greater than or equal to 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgEmptyChart, "There are no visible series to represent in a chart.\nTry to add new series, or make sure that\nat least one of them is visible.");
			AddString(ChartStringId.MsgNoPanes, "There are no visible panes to show in a chart.\nTry to set the chart's Diagram.DefaultPane.Visible property to True,\nor show other panes from the Diagram.Panes collection.");
			AddString(ChartStringId.MsgChartLoadingException, "The specified XML file can't be opened,\nbecause it is either not a supported file type,\nor because the file has been damaged.");
			AddString(ChartStringId.MsgIncorrectPaneWeight, "The weight of the pane should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectPaneDistance, "The distance between the panes should be greater than or equal to 0.");
			AddString(ChartStringId.MsgEmptyPaneTextForVerticalLayout, "Assign this pane to the Series.View.Pane property,\nto show a series on this pane");
			AddString(ChartStringId.MsgEmptyPaneTextForHorizontalLayout, "Assign this pane to the\nSeries.View.Pane property,\nto show a series on this pane");
			AddString(ChartStringId.MsgInvalidPaneSizeInPixels, "The pane's size in pixels should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectTopNCount, "The top N values count should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTopNThresholdValue, "The top N threshold value should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectTopNThresholdPercent, "The top N threshold percent should be greater than 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgInvalidPane, "The specified pane either doesn't belong to a chart, or doesn't show the current axis whose visibility should be changed.");
			AddString(ChartStringId.MsgIncorrectSummaryFunction, "The specified summary function string is in an incorrect format.");
			AddString(ChartStringId.MsgNullFinancialIndicatorArgument, "The argument of the financial indicator's point can't be set to null.");
			AddString(ChartStringId.MsgUnsupportedValueLevel, "The {0} value level isn't supported by the {1}.");
			AddString(ChartStringId.MsgSummaryFunctionIsNotRegistered, "A summary function with the name '{0}' is not registered.");
			AddString(ChartStringId.MsgSummaryFunctionParameterIsNotSpecified, "You should specify all of the summary function parameters.");
			AddString(ChartStringId.MsgIncompatibleSummaryFunctionDimension, "The dimension of the {0} summary function isn't compatible with the {1} series view ({2} but should be {3}).");
			AddString(ChartStringId.MsgIncorrectSummaryFunctionParametersCount, "The summary function '{0}' accepts {1} parameters instead of {2}.");
			AddString(ChartStringId.MsgWebInvalidWidthUnit, "The chart width must be set in pixels.");
			AddString(ChartStringId.MsgWebInvalidHeightUnit, "The chart height must be set in pixels.");
			AddString(ChartStringId.MsgIncorrectBubbleMaxSize, "The maximum size should be greater than the minimum size.");
			AddString(ChartStringId.MsgIncorrectBubbleMinSize, "The minimum size should be greater than or equal to 0, and less than the maximum size.");
			AddString(ChartStringId.MsgInvalidLogarithmicBase, "The logarithmic base should be greater than 1.");
			AddString(ChartStringId.MsgUnsupportedTopNOptions, "TopNOptions can't be enabled for this series, because either its ValueScaleType is not Numerical or its data points have more than 1 value.");
			AddString(ChartStringId.MsgUnsupportedResolveOverlappingMode, "The specified overlapping mode isn't supported by the current series view.");
			AddString(ChartStringId.MsgIncorrectDateTimeMeasureUnitPropertyUsing, "The MeasureUnit property can't be modified in both the automatic and continuous date-time scale modes.");
			AddString(ChartStringId.MsgIncorrectDateTimeGridAlignmentPropertyUsing, "The GridAlignment property can't be modified in the automatic date-time scale mode.");
			AddString(ChartStringId.MsgIncorrectDateTimeGridAlignment, "The GridAlignment property must be greater than or equal to the current measure unit.");
			AddString(ChartStringId.MsgIncorrectNumericMeasureUnit, "The current measure unit should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectNumericGridAlignment, "The grid alignment should be greater than or equal to the current measure unit.");
			AddString(ChartStringId.MsgIncorrectNumericMeasureUnitPropertyUsing, "The MeasureUnit property can't be modified in both the automatic and continuous numeric scale modes.");
			AddString(ChartStringId.MsgIncorrectNumericGridAlignmentPropertyUsing, "The GridAlignment property can't be modified in the automatic numeric scale mode.");
			AddString(ChartStringId.MsgIncorrectValueLevel, "The {0} ValueLevel is invalid for the current regression line.");
			AddString(ChartStringId.MsgUnsupportedDateTimeScaleModeWithScrollingZooming, "Automatic DateTimeScaleMode can't work together with zooming and scrolling.");
			AddString(ChartStringId.MsgUnsupportedDateTimeScaleModeForGanttDiagram, "DateTimeScaleMode isn't supported for the GanttDiagram.");
			AddString(ChartStringId.MsgIncorrectAxisRangeMinValue, "MinValue can't be set to null.");
			AddString(ChartStringId.MsgIncorrectAxisRangeMaxValue, "MaxValue can't be set to null.");
			AddString(ChartStringId.MsgIncorrectAxisRangeMinValueInternal, "MinValueInternal can't be set to NaN and Infinity values.");
			AddString(ChartStringId.MsgIncorrectAxisRangeMaxValueInternal, "MaxValueInternal can't be set to NaN and Infinity values.");
			AddString(ChartStringId.MsgIncorrectPropertyValue, "Incorrect value \"{0}\" for the property \"{1}\".");
			AddString(ChartStringId.MsgMinMaxDifferentTypes, "The types of the MinValue and MaxValue don't match.");
			AddString(ChartStringId.MsgIncorrectAxisRange, "The min value of the axis range should be less than its max value.");
			AddString(ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode, "It is impossible to set a custom range, if the DateTimeScaleMode is not Manual.");
			AddString(ChartStringId.MsgUnsupportedWorkdaysForWorkdaysOptions, "The zero value is not acceptable for the workdays.  Use work days of a week.");
			AddString(ChartStringId.MsgCantSwapSeries, "It's impossible to swap autocreated and fixed series.");
			AddString(ChartStringId.MsgInvalidEdge1, "The edge1 value can't be null.");
			AddString(ChartStringId.MsgInvalidEdge2, "The edge2 value can't be null.");
			AddString(ChartStringId.MsgInvalidSizeInPixels, "Size in pixels should be greater than or equal to -1 and less than 50.");
			AddString(ChartStringId.MsgInvalidMaxCount, "Max count should be greater than 0.");
			AddString(ChartStringId.MsgInvalidGradientMode, "This PolygonGradientMode isn't compatible with AreaSeriesView.");
			AddString(ChartStringId.MsgIncorrectDashStyle, "DashStyle.Empty can only be assigned to a constant line's LineStyle property.");
			AddString(ChartStringId.MsgAnnotationMovingToolTip, "Use Ctrl with the left mouse button\nto move the annotation.");
			AddString(ChartStringId.MsgAnnotationResizingToolTip, "Use Ctrl with the left mouse button\nto resize the annotation.");
			AddString(ChartStringId.MsgAnnotationRotationToolTip, "Use Ctrl with the left mouse button\nto rotate the annotation.");
			AddString(ChartStringId.MsgAnchorPointMovingToolTip, "Use Ctrl with the left mouse button\nto move the anchor point.");
			AddString(ChartStringId.MsgPivotGridDataSourceOptionsNotSupportedProperty, "The PivotGridDataSourceOptions.{0} property is available only if the chart's data source is a PivotGrid.");
			AddString(ChartStringId.MsgDiagramToPointIncorrectValue, "The specified {0} parameter type doesn't match the appropriate scale type, which is {1} for this axis.");
			AddString(ChartStringId.MsgIncorrectBarDistancePropertyUsing, "Cannot set the BarDistance property unless the series is added to the chart's collection.");
			AddString(ChartStringId.MsgIncorrectBarDistanceFixedPropertyUsing, "Cannot set the BarDistanceFixed property unless the series is added to the chart's collection.");
			AddString(ChartStringId.MsgIncorrectEqualBarWidthPropertyUsing, "Cannot set the EqualBarWidth property unless the series is added to the chart's collection.");
			AddString(ChartStringId.MsgFileNotFound, "File '{0}' isn't found.");
			AddString(ChartStringId.MsgCantImportHolidays, "Failed to import holydays from the '{0}' file.");
			AddString(ChartStringId.MsgIncorrectIndicator, "You can add only Indicator objects to this collection.");
			AddString(ChartStringId.MsgIncorrectBarSeriesLabelPosition, "The BarSeriesLabelPosition.Top value isn't supported for this series view type.");
			AddString(ChartStringId.MsgIncorrectBarSeriesLabelIndent, "The indent should be greater than or equal to 0.");
			AddString(ChartStringId.MsgIncorrectPointsCount, "The points count should be greater than 1.");
			AddString(ChartStringId.MsgIncorrectEnvelopePercent, "The envelope percent should be greater than 0 and less than or equal to 100.");
			AddString(ChartStringId.MsgEmptySecondaryAxisName, "A secondary axis name can't be empty.");
			AddString(ChartStringId.MsgEmptyPaneName, "A pane's name can't be empty.");
			AddString(ChartStringId.MsgIncorrectRangeControlClientSpacing, "The {0} property should be greater than 0.");
			AddString(ChartStringId.MsgIncorrectDateTimeRangeControlClientSpacing, "The {0} property  can't be set to non-integer for the date-time scale.");
			AddString(ChartStringId.MsgProcessMissingPointsForValueAxis, "The ProcessMissingPoints property operates with the X-axis scale only.");
			AddString(ChartStringId.MsgProcessMissingPointsForContinuousScale, "The ProcessMissingPoints property can't be specified in the continuous scale mode.");
			AddString(ChartStringId.MsgIncorrectMaxZoomPercent, "The zoom percent should be greater than or equal to 100.");
			AddString(ChartStringId.VerbAbout, "About");
			AddString(ChartStringId.VerbAboutDescription, "See basic information on XtraCharts.");
			AddString(ChartStringId.VerbPopulate, "Populate");
			AddString(ChartStringId.VerbPopulateDescription, "Populate the chart's datasource with data.");
			AddString(ChartStringId.VerbClearDataSource, "Clear Data Source");
			AddString(ChartStringId.VerbClearDataSourceDescription, "Clear the chart's datasource.");
			AddString(ChartStringId.VerbDataSnapshot, "Data Snapshot");
			AddString(ChartStringId.VerbDataSnapshotDescription, "Copy all the data from the bound datasource to the chart, and then unbind the chart.");
			AddString(ChartStringId.VerbAnnotations, "Annotations...");
			AddString(ChartStringId.VerbAnnotationsDescription, "Open the Annotations Collection Editor.");
			AddString(ChartStringId.VerbSeries, "Series...");
			AddString(ChartStringId.VerbSeriesDescription, "Open the Series Collection Editor.");
			AddString(ChartStringId.VerbResetLegendPointOptions, "Reset legend point options");
			AddString(ChartStringId.VerbResetLegendPointOptionsDescription, "Revert the legend point options to their default values.");
			AddString(ChartStringId.VerbEditPalettes, "Palettes...");
			AddString(ChartStringId.VerbEditPalettesDescription, "Open the Palettes Editor.");
			AddString(ChartStringId.VerbWizard, "Run Wizard...");
			AddString(ChartStringId.VerbWizardDescription, "Run the Chart Wizard, which allows the properties of the chart to be edited.");
			AddString(ChartStringId.VerbSaveLayout, "Save...");
			AddString(ChartStringId.VerbSaveLayoutDescription, "Save a chart to an XML file.");
			AddString(ChartStringId.VerbLoadLayout, "Load...");
			AddString(ChartStringId.VerbLoadLayoutDescription, "Load a chart from an XML file.");
			AddString(ChartStringId.PieIncorrectValuesText, "This view can't represent negative\nvalues. All values must be either greater\nthan or equal to zero.");
			AddString(ChartStringId.FontFormat, "{0}, {1}pt, {2}");
			AddString(ChartStringId.TrnSeriesChanged, "Series changed");
			AddString(ChartStringId.TrnDataFiltersChanged, "DataFilters changed");
			AddString(ChartStringId.TrnChartTitlesChanged, "Chart titles changed");
			AddString(ChartStringId.TrnPalettesChanged, "Palettes changed");
			AddString(ChartStringId.TrnConstantLinesChanged, "Constant Lines changed");
			AddString(ChartStringId.TrnStripsChanged, "Strips changed");
			AddString(ChartStringId.TrnCustomAxisLabelChanged, "Custom Axis Labels changed");
			AddString(ChartStringId.TrnSecondaryAxesXChanged, "Secondary axes X changed");
			AddString(ChartStringId.TrnSecondaryAxesYChanged, "Secondary axes Y changed");
			AddString(ChartStringId.TrnXYDiagramPanesChanged, "Panes changed");
			AddString(ChartStringId.TrnChartWizard, "Chart wizard settings applied");
			AddString(ChartStringId.TrnSeriesDeleted, "Series deleted");
			AddString(ChartStringId.TrnChartTitleDeleted, "Chart title deleted");
			AddString(ChartStringId.TrnAnnotationDeleted, "Annotation deleted");
			AddString(ChartStringId.TrnConstantLineDeleted, "Constant line deleted");
			AddString(ChartStringId.TrnPaneDeleted, "Pane deleted");
			AddString(ChartStringId.TrnSecondaryAxisXDeleted, "Secondary axis X deleted");
			AddString(ChartStringId.TrnSecondaryAxisYDeleted, "Secondary axis Y deleted");
			AddString(ChartStringId.TrnExplodedPoints, "Exploded points changed");
			AddString(ChartStringId.TrnExplodedPointsFilters, "ExplodedPointsFilters changed");
			AddString(ChartStringId.TrnLegendPointOptionsReset, "LegendPointOptions reset");
			AddString(ChartStringId.TrnLoadLayout, "Chart layout loaded");
			AddString(ChartStringId.TrnSeriesTitleChanged, "Series title changed");
			AddString(ChartStringId.TrnSeriesTitleDeleted, "Series title deleted");
			AddString(ChartStringId.TrnAxisVisibilityChanged, "Axis visibility changed");
			AddString(ChartStringId.TrnSummaryFunctionChanged, "Summary function changed");
			AddString(ChartStringId.TrnIndicatorDeleted, "Indicator deleted");
			AddString(ChartStringId.TrnIndicatorsChanged, "Indicators changed");
			AddString(ChartStringId.TrnScaleBreaksChanged, "Scale breaks changed");
			AddString(ChartStringId.TrnAnnotationsChanged, "Annotations changed");
			AddString(ChartStringId.TrnHolidaysChanged, "Holidays changed");
			AddString(ChartStringId.TrnExactWorkdaysChanged, "Exact workdays changed");
			AddString(ChartStringId.AxisXDefaultTitle, "Axis of arguments");
			AddString(ChartStringId.AxisYDefaultTitle, "Axis of values");
			AddString(ChartStringId.DefaultWizardPageLabel, "Wizard Page");
			AddString(ChartStringId.MenuItemAdd, "Add");
			AddString(ChartStringId.MenuItemInsert, "Insert");
			AddString(ChartStringId.MenuItemDelete, "Delete");
			AddString(ChartStringId.MenuItemClear, "Clear");
			AddString(ChartStringId.MenuItemMoveUp, "Move Up");
			AddString(ChartStringId.MenuItemMoveDown, "Move Down");
			AddString(ChartStringId.WizAutoCreatedSeries, "Auto-created Series");
			AddString(ChartStringId.WizSpecifyDataFilters, "Click the ellipsis button...");
			AddString(ChartStringId.WizDataFiltersDisabled, "(none)");
			AddString(ChartStringId.WizDataFiltersEntered, "{0} data filter(s)");
			AddString(ChartStringId.WizBackImageFileNameFilter, "Image Files(*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png|All files(*.*)|*.*");
			AddString(ChartStringId.WizNoBackImage, "(None)");
			AddString(ChartStringId.WizConstructionGroupName, "Construction");
			AddString(ChartStringId.WizPresentationGroupName, "Presentation");
			AddString(ChartStringId.WizChartTypePageName, "Chart Type");
			AddString(ChartStringId.WizAppearancePageName, "Appearance");
			AddString(ChartStringId.WizSeriesPageName, "Series");
			AddString(ChartStringId.WizDataPageName, "Data");
			AddString(ChartStringId.WizChartPageName, "Chart");
			AddString(ChartStringId.WizDiagramPageName, "Diagram");
			AddString(ChartStringId.WizAxesPageName, "Axes");
			AddString(ChartStringId.WizAxesAlignmentNear, "Near");
			AddString(ChartStringId.WizAxesAlignmentFar, "Far");
			AddString(ChartStringId.WizAxesAlignmentZero, "Zero");
			AddString(ChartStringId.WizPanesPageName, "Panes");
			AddString(ChartStringId.WizAnnotationsPageName, "Annotations");
			AddString(ChartStringId.WizSeriesViewPageName, "Series View");
			AddString(ChartStringId.WizSeriesLabelsPageName, "Point Labels");
			AddString(ChartStringId.WizChartTitlesPageName, "Chart Titles");
			AddString(ChartStringId.WizLegendPageName, "Legend");
			AddString(ChartStringId.WizSeriesPointPageName, "Points");
			AddString(ChartStringId.WizSeriesDataBindingPageName, "Series Binding");
			AddString(ChartStringId.WizPivotGridDataSourcePageName, "Pivot Grid Datasource");
			AddString(ChartStringId.WizChartTypePageDescription, "Choose a chart type you want to use. To filter chart types by their groups, use the values in the drop-down box.");
			AddString(ChartStringId.WizAppearancePageDescription, "Choose a palette to color series or their data points. Also choose the style, which specifies the chart's appearance depending on the current palette.");
			AddString(ChartStringId.WizSeriesPageDescription, "Create series, and adjust their general properties.\r\nNote that the view type of the first visible series determines the diagram type and its set of specific options.");
			AddString(ChartStringId.WizDataPageDescription, "To enter data points manually, use the Points tab. Or, use other tabs, to specify data source settings for individual or auto-created series.");
			AddString(ChartStringId.WizChartPageDescription, "Customize the chart's properties.");
			AddString(ChartStringId.WizDiagramPageDescription, "Customize the diagram's properties.");
			AddString(ChartStringId.WizAxesPageDescription, "Customize the X and Y axes of the diagram.\r\nNote that you may select an axis by clicking it in the chart preview.");
			AddString(ChartStringId.WizPanesPageDescription, "Customize the diagram's panes.\r\nNote that you may select a pane by clicking it in the chart preview.");
			AddString(ChartStringId.WizAnnotationsPageDescription, "Create and customize annotations anchored to a chart, pane or series point.\r\nNote that you may select an annotation by clicking it in the chart preview.");
			AddString(ChartStringId.WizSeriesViewPageDescription, "Customize the view-type-specific options of a series.\r\nNote that you may select a series by clicking it in the chart preview.");
			AddString(ChartStringId.WizSeriesLabelsPageDescription, "Customize the point labels of a series.\r\nNote that you may select labels of a series by clicking them in the chart preview.");
			AddString(ChartStringId.WizChartTitlesPageDescription, "Add titles to your chart, and customize their options.");
			AddString(ChartStringId.WizLegendPageDescription, "Customize the legend's properties.");
			AddString(ChartStringId.WizFormTitle, "Chart Wizard");
			AddString(ChartStringId.WizHatchMin, "Min");
			AddString(ChartStringId.WizHatchHorizontal, "Horizontal");
			AddString(ChartStringId.WizHatchVertical, "Vertical");
			AddString(ChartStringId.WizHatchForwardDiagonal, "ForwardDiagonal");
			AddString(ChartStringId.WizHatchBackwardDiagonal, "BackwardDiagonal");
			AddString(ChartStringId.WizHatchMax, "Max");
			AddString(ChartStringId.WizHatchCross, "Cross");
			AddString(ChartStringId.WizHatchLargeGrid, "LargeGrid");
			AddString(ChartStringId.WizHatchDiagonalCross, "DiagonalCross");
			AddString(ChartStringId.WizHatchPercent05, "Percent05");
			AddString(ChartStringId.WizHatchPercent10, "Percent10");
			AddString(ChartStringId.WizHatchPercent20, "Percent20");
			AddString(ChartStringId.WizHatchPercent25, "Percent25");
			AddString(ChartStringId.WizHatchPercent30, "Percent30");
			AddString(ChartStringId.WizHatchPercent40, "Percent40");
			AddString(ChartStringId.WizHatchPercent50, "Percent50");
			AddString(ChartStringId.WizHatchPercent60, "Percent60");
			AddString(ChartStringId.WizHatchPercent70, "Percent70");
			AddString(ChartStringId.WizHatchPercent75, "Percent75");
			AddString(ChartStringId.WizHatchPercent80, "Percent80");
			AddString(ChartStringId.WizHatchPercent90, "Percent90");
			AddString(ChartStringId.WizHatchLightDownwardDiagonal, "LightDownwardDiagonal");
			AddString(ChartStringId.WizHatchLightUpwardDiagonal, "LightUpwardDiagonal");
			AddString(ChartStringId.WizHatchDarkDownwardDiagonal, "DarkDownwardDiagonal");
			AddString(ChartStringId.WizHatchDarkUpwardDiagonal, "DarkUpwardDiagonal");
			AddString(ChartStringId.WizHatchWideDownwardDiagonal, "WideDownwardDiagonal");
			AddString(ChartStringId.WizHatchWideUpwardDiagonal, "WideUpwardDiagonal");
			AddString(ChartStringId.WizHatchLightVertical, "LightVertical");
			AddString(ChartStringId.WizHatchLightHorizontal, "LightHorizontal");
			AddString(ChartStringId.WizHatchNarrowVertical, "NarrowVertical");
			AddString(ChartStringId.WizHatchNarrowHorizontal, "NarrowHorizontal");
			AddString(ChartStringId.WizHatchDarkVertical, "DarkVertical");
			AddString(ChartStringId.WizHatchDarkHorizontal, "DarkHorizontal");
			AddString(ChartStringId.WizHatchDashedDownwardDiagonal, "DashedDownwardDiagonal");
			AddString(ChartStringId.WizHatchDashedUpwardDiagonal, "DashedUpwardDiagonal");
			AddString(ChartStringId.WizHatchDashedHorizontal, "DashedHorizontal");
			AddString(ChartStringId.WizHatchDashedVertical, "DashedVertical");
			AddString(ChartStringId.WizHatchSmallConfetti, "SmallConfetti");
			AddString(ChartStringId.WizHatchLargeConfetti, "LargeConfetti");
			AddString(ChartStringId.WizHatchZigZag, "ZigZag");
			AddString(ChartStringId.WizHatchWave, "Wave");
			AddString(ChartStringId.WizHatchDiagonalBrick, "DiagonalBrick");
			AddString(ChartStringId.WizHatchHorizontalBrick, "HorizontalBrick");
			AddString(ChartStringId.WizHatchWeave, "Weave");
			AddString(ChartStringId.WizHatchPlaid, "Plaid");
			AddString(ChartStringId.WizHatchDivot, "Divot");
			AddString(ChartStringId.WizHatchDottedGrid, "DottedGrid");
			AddString(ChartStringId.WizHatchDottedDiamond, "DottedDiamond");
			AddString(ChartStringId.WizHatchShingle, "Shingle");
			AddString(ChartStringId.WizHatchTrellis, "Trellis");
			AddString(ChartStringId.WizHatchSphere, "Sphere");
			AddString(ChartStringId.WizHatchSmallGrid, "SmallGrid");
			AddString(ChartStringId.WizHatchSmallCheckerBoard, "SmallCheckerBoard");
			AddString(ChartStringId.WizHatchLargeCheckerBoard, "LargeCheckerBoard");
			AddString(ChartStringId.WizHatchOutlinedDiamond, "OutlinedDiamond");
			AddString(ChartStringId.WizHatchSolidDiamond, "SolidDiamond");
			AddString(ChartStringId.WizDataMemberNoneString, "(None)");
			AddString(ChartStringId.WizPositionLeftColumn, "LeftColumn");
			AddString(ChartStringId.WizPositionLeft, "Left");
			AddString(ChartStringId.WizPositionCenter, "Center");
			AddString(ChartStringId.WizPositionRight, "Right");
			AddString(ChartStringId.WizPositionRightColumn, "RightColumn");
			AddString(ChartStringId.WizGradientBottomLeftToTopRight, "BottomLeftToTopRight");
			AddString(ChartStringId.WizGradientBottomRightToTopLeft, "BottomRightToTopLeft");
			AddString(ChartStringId.WizGradientBottomToTop, "BottomToTop");
			AddString(ChartStringId.WizGradientFromCenter, "FromCenter");
			AddString(ChartStringId.WizGradientLeftToRight, "LeftToRight");
			AddString(ChartStringId.WizGradientRightToLeft, "RightToLeft");
			AddString(ChartStringId.WizGradientToCenter, "ToCenter");
			AddString(ChartStringId.WizGradientTopLeftToBottomRight, "TopLeftToBottomRight");
			AddString(ChartStringId.WizGradientTopRightToBottomLeft, "TopRightToBottomLeft");
			AddString(ChartStringId.WizGradientTopToBottom, "TopToBottom");
			AddString(ChartStringId.WizGradientToCenterHorizontal, "ToCenterHorizontal");
			AddString(ChartStringId.WizGradientFromCenterHorizontal, "FromCenterHorizontal");
			AddString(ChartStringId.WizGradientToCenterVertical, "ToCenterVertical");
			AddString(ChartStringId.WizGradientFromCenterVertical, "FromCenterVertical");
			AddString(ChartStringId.WizValueLevelValue, "Value");
			AddString(ChartStringId.WizValueLevelValue_1, "Value_1");
			AddString(ChartStringId.WizValueLevelValue_2, "Value_2");
			AddString(ChartStringId.WizValueLevelLow, "Low");
			AddString(ChartStringId.WizValueLevelHigh, "High");
			AddString(ChartStringId.WizValueLevelOpen, "Open");
			AddString(ChartStringId.WizValueLevelClose, "Close");
			AddString(ChartStringId.WizDateTimeMeasureUnitYear, "Year");
			AddString(ChartStringId.WizDateTimeMeasureUnitQuarter, "Quarter");
			AddString(ChartStringId.WizDateTimeMeasureUnitMonth, "Month");
			AddString(ChartStringId.WizDateTimeMeasureUnitWeek, "Week");
			AddString(ChartStringId.WizDateTimeMeasureUnitDay, "Day");
			AddString(ChartStringId.WizDateTimeMeasureUnitHour, "Hour");
			AddString(ChartStringId.WizDateTimeMeasureUnitMinute, "Minute");
			AddString(ChartStringId.WizDateTimeMeasureUnitSecond, "Second");
			AddString(ChartStringId.WizDateTimeMeasureUnitMillisecond, "Millisecond");
			AddString(ChartStringId.WizDateTimeGridAlignmentAuto, "Auto");
			AddString(ChartStringId.WizNumericMeasureUnitOnes, "Ones");
			AddString(ChartStringId.WizNumericMeasureUnitTens, "Tens");
			AddString(ChartStringId.WizNumericMeasureUnitHundreds, "Hundreds");
			AddString(ChartStringId.WizNumericMeasureUnitThousands, "Thousands");
			AddString(ChartStringId.WizNumericMeasureUnitMillions, "Millions");
			AddString(ChartStringId.WizNumericMeasureUnitBillions, "Billions");
			AddString(ChartStringId.WizNumericMeasureUnitCustom, "Custom");
			AddString(ChartStringId.WizNumericGridAlignmentAuto, "Auto");
			AddString(ChartStringId.WizResolveOverlappingModeNone, "None");
			AddString(ChartStringId.WizResolveOverlappingModeDefault, "Default");
			AddString(ChartStringId.WizResolveOverlappingModeHideOverlapping, "HideOverlapping");
			AddString(ChartStringId.WizResolveOverlappingModeJustifyAroundPoint, "JustifyAroundPoint");
			AddString(ChartStringId.WizResolveOverlappingModeJustifyAllAroundPoints, "JustifyAllAroundPoints");
			AddString(ChartStringId.WizAxisLabelResolveOverlappingModeNone, "None");
			AddString(ChartStringId.WizAxisLabelResolveOverlappingModeHideOverlapping, "HideOverlapping");
			AddString(ChartStringId.WizErrorMessageTitle, "Chart Wizard");
			AddString(ChartStringId.WizInvalidBackgroundImage, "The specified file isn't a correct image file. Please choose another one.");
			AddString(ChartStringId.WizScrollBarAlignmentNear, "Near");
			AddString(ChartStringId.WizScrollBarAlignmentFar, "Far");
			AddString(ChartStringId.WizDateTimeScaleModeManual, "Manual");
			AddString(ChartStringId.WizDateTimeScaleModeAutomaticAverage, "Automatic: Average");
			AddString(ChartStringId.WizDateTimeScaleModeAutomaticIntegral, "Automatic: Integral");
			AddString(ChartStringId.WizScaleBreakStyleRagged, "Ragged");
			AddString(ChartStringId.WizScaleBreakStyleStraight, "Straight");
			AddString(ChartStringId.WizScaleBreakStyleWaved, "Waved");
			AddString(ChartStringId.WizBarSeriesLabelPositionAuto, "Auto");
			AddString(ChartStringId.WizBarSeriesLabelPositionTop, "Top");
			AddString(ChartStringId.WizBarSeriesLabelPositionCenter, "Center");
			AddString(ChartStringId.WizBarSeriesLabelPositionTopInside, "Top Inside");
			AddString(ChartStringId.WizBarSeriesLabelPositionBottomInside, "Bottom Inside");
			AddString(ChartStringId.WizPieSeriesLabelPositionInside, "Inside");
			AddString(ChartStringId.WizPieSeriesLabelPositionOutside, "Outside");
			AddString(ChartStringId.WizPieSeriesLabelPositionRadial, "Radial");
			AddString(ChartStringId.WizPieSeriesLabelPositionTangent, "Tangent");
			AddString(ChartStringId.WizPieSeriesLabelPositionTwoColumns, "Two Columns");
			AddString(ChartStringId.WizBubbleLabelValueToDisplayValue, "Value");
			AddString(ChartStringId.WizBubbleLabelValueToDisplayWeight, "Weight");
			AddString(ChartStringId.WizBubbleLabelValueToDisplayValueAndWeight, "Value and Weight");
			AddString(ChartStringId.WizBubbleLabelPositionCenter, "Center");
			AddString(ChartStringId.WizBubbleLabelPositionOutside, "Outside");
			AddString(ChartStringId.WizFunnelSeriesLabelPositionCenter, "Center");
			AddString(ChartStringId.WizFunnelSeriesLabelPositionLeft, "Left");
			AddString(ChartStringId.WizFunnelSeriesLabelPositionLeftColumn, "Left Column");
			AddString(ChartStringId.WizFunnelSeriesLabelPositionRight, "Right");
			AddString(ChartStringId.WizFunnelSeriesLabelPositionRightColumn, "Right Column");
			AddString(ChartStringId.WizSeriesLabelTextOrientationHorizontal, "Horizontal");
			AddString(ChartStringId.WizSeriesLabelTextOrientationTopToBottom, "TopToBottom");
			AddString(ChartStringId.WizSeriesLabelTextOrientationBottomToTop, "BottomToTop");
			AddString(ChartStringId.WizBar3DModelBox, "Box");
			AddString(ChartStringId.WizBar3DModelCylinder, "Cylinder");
			AddString(ChartStringId.WizBar3DModelCone, "Cone");
			AddString(ChartStringId.WizBar3DModelPyramid, "Pyramid");
			AddString(ChartStringId.WizEnableScrollingTrue, "Enable scrolling (true)");
			AddString(ChartStringId.WizEnableScrollingFalse, "Enable scrolling (false)");
			AddString(ChartStringId.WizEnableZoomingTrue, "Enable zooming (true)");
			AddString(ChartStringId.WizEnableZoomingFalse, "Enable zooming (false)");
			AddString(ChartStringId.WizShapeKindRectangle, "Rectangle");
			AddString(ChartStringId.WizShapeKindRoundedRectangle, "Rounded Rectangle");
			AddString(ChartStringId.WizShapeKindEllipse, "Ellipse");
			AddString(ChartStringId.WizAnnotationConnectorStyleArrow, "Arrow");
			AddString(ChartStringId.WizAnnotationConnectorStyleLine, "Line");
			AddString(ChartStringId.WizAnnotationConnectorStyleNone, "None");
			AddString(ChartStringId.WizAnnotationConnectorStyleNotchedArrow, "Notched Arrow");
			AddString(ChartStringId.WizAnnotationConnectorStyleTail, "Tail");
			AddString(ChartStringId.WizStringAlignmentCenter, "Center");
			AddString(ChartStringId.WizStringAlignmentNear, "Near");
			AddString(ChartStringId.WizStringAlignmentFar, "Far");
			AddString(ChartStringId.WizChartImageSizeModeAutoSize, "Auto Size");
			AddString(ChartStringId.WizChartImageSizeModeStretch, "Stretch");
			AddString(ChartStringId.WizChartImageSizeModeTile, "Tile");
			AddString(ChartStringId.WizChartImageSizeModeZoom, "Zoom");
			AddString(ChartStringId.WizDockCornerLeftTop, "Left-top");
			AddString(ChartStringId.WizDockCornerLeftBottom, "Left-bottom");
			AddString(ChartStringId.WizDockCornerRightTop, "Right-top");
			AddString(ChartStringId.WizDockCornerRightBottom, "Right-bottom");
			AddString(ChartStringId.WizShapePositionKindFree, "Free");
			AddString(ChartStringId.WizShapePositionKindRelative, "Relative");
			AddString(ChartStringId.WizAnchorPointChart, "Chart");
			AddString(ChartStringId.WizAnchorPointPane, "Pane");
			AddString(ChartStringId.WizAnchorPointSeriesPoint, "Series Point");
			AddString(ChartStringId.WizIndentUndefined, "Undefined");
			AddString(ChartStringId.WizIndentDefault, "Default");
			AddString(ChartStringId.SvnSideBySideBar, "Bar");
			AddString(ChartStringId.SvnStackedBar, "Bar Stacked");
			AddString(ChartStringId.SvnFullStackedBar, "Bar Stacked 100%");
			AddString(ChartStringId.SvnSideBySideStackedBar, "Side By Side Bar Stacked");
			AddString(ChartStringId.SvnSideBySideFullStackedBar, "Side By Side Bar Stacked 100%");
			AddString(ChartStringId.SvnPie, "Pie");
			AddString(ChartStringId.SvnFunnel, "Funnel");
			AddString(ChartStringId.SvnDoughnut, "Doughnut");
			AddString(ChartStringId.SvnNestedDoughnut, "Nested Doughnut");
			AddString(ChartStringId.SvnPoint, "Point");
			AddString(ChartStringId.SvnBubble, "Bubble");
			AddString(ChartStringId.SvnLine, "Line");
			AddString(ChartStringId.SvnStackedLine, "Line Stacked");
			AddString(ChartStringId.SvnFullStackedLine, "Line Stacked 100%");
			AddString(ChartStringId.SvnStepLine, "Step Line");
			AddString(ChartStringId.SvnSpline, "Spline");
			AddString(ChartStringId.SvnScatterLine, "Scatter Line");
			AddString(ChartStringId.SvnSpline3D, "Spline 3D");
			AddString(ChartStringId.SvnArea, "Area");
			AddString(ChartStringId.SvnStepArea, "Step Area");
			AddString(ChartStringId.SvnSplineArea, "Spline Area");
			AddString(ChartStringId.SvnStackedArea, "Area Stacked");
			AddString(ChartStringId.SvnSplineStackedArea, "Spline Area Stacked");
			AddString(ChartStringId.SvnFullStackedArea, "Area Stacked 100%");
			AddString(ChartStringId.SvnSplineFullStackedArea, "Spline Area Stacked 100%");
			AddString(ChartStringId.SvnRangeArea, "Range Area");
			AddString(ChartStringId.SvnRangeArea3D, "Range Area 3D");
			AddString(ChartStringId.SvnArea3D, "Area 3D");
			AddString(ChartStringId.SvnStackedArea3D, "Area 3D Stacked");
			AddString(ChartStringId.SvnFullStackedArea3D, "Area 3D Stacked 100%");
			AddString(ChartStringId.SvnStepArea3D, "Step Area 3D");
			AddString(ChartStringId.SvnSpline3DArea, "Spline Area 3D");
			AddString(ChartStringId.SvnSplineAreaStacked3D, "Spline Area 3D Stacked");
			AddString(ChartStringId.SvnSplineAreaFullStacked3D, "Spline Area 3D Stacked 100%");
			AddString(ChartStringId.SvnStock, "Stock");
			AddString(ChartStringId.SvnCandleStick, "Candle Stick");
			AddString(ChartStringId.SvnSideBySideRangeBar, "Side By Side Range Bar");
			AddString(ChartStringId.SvnOverlappedRangeBar, "Range Bar");
			AddString(ChartStringId.SvnSideBySideGantt, "Side By Side Gantt");
			AddString(ChartStringId.SvnOverlappedGantt, "Gantt");
			AddString(ChartStringId.SvnSideBySideBar3D, "Bar 3D");
			AddString(ChartStringId.SvnStackedBar3D, "Bar 3D Stacked");
			AddString(ChartStringId.SvnFullStackedBar3D, "Bar 3D Stacked 100%");
			AddString(ChartStringId.SvnManhattanBar, "Manhattan Bar");
			AddString(ChartStringId.SvnSideBySideStackedBar3D, "Side By Side Bar 3D Stacked");
			AddString(ChartStringId.SvnSideBySideFullStackedBar3D, "Side By Side Bar 3D Stacked 100%");
			AddString(ChartStringId.SvnPie3D, "Pie 3D");
			AddString(ChartStringId.SvnDoughnut3D, "Doughnut 3D");
			AddString(ChartStringId.SvnFunnel3D, "Funnel 3D");
			AddString(ChartStringId.SvnLine3D, "Line 3D");
			AddString(ChartStringId.SvnStackedLine3D, "Line 3D Stacked");
			AddString(ChartStringId.SvnFullStackedLine3D, "Line 3D Stacked 100%");
			AddString(ChartStringId.SvnStepLine3D, "Step Line 3D");
			AddString(ChartStringId.SvnRadarPoint, "Radar Point");
			AddString(ChartStringId.SvnRadarLine, "Radar Line");
			AddString(ChartStringId.SvnScatterRadarLine, "Scatter Radar Line");
			AddString(ChartStringId.SvnRadarArea, "Radar Area");
			AddString(ChartStringId.SvnPolarPoint, "Polar Point");
			AddString(ChartStringId.SvnPolarLine, "Polar Line");
			AddString(ChartStringId.SvnScatterPolarLine, "Scatter Polar Line");
			AddString(ChartStringId.SvnPolarArea, "Polar Area");
			AddString(ChartStringId.SvnSwiftPlot, "Swift Plot");
			AddString(ChartStringId.IndFibonacciIndicator, "Indicator");
			AddString(ChartStringId.IndRegressionLine, "Regression Line");
			AddString(ChartStringId.IndTrendLine, "Trend Line");
			AddString(ChartStringId.IndFibonacciIndicator, "Fibonacci Indicator");
			AddString(ChartStringId.IndSimpleMovingAverage, "Simple Moving Average");
			AddString(ChartStringId.IndExponentialMovingAverage, "Exponential Moving Average");
			AddString(ChartStringId.IndWeightedMovingAverage, "Weighted Moving Average");
			AddString(ChartStringId.IndTriangularMovingAverage, "Triangular Moving Average");
			AddString(ChartStringId.IndStandardDeviation, "Standard Deviation");
			AddString(ChartStringId.IndAverageTrueRange, "Average True Range");
			AddString(ChartStringId.IndCommodityChannelIndex, "Commodity Channel Index");
			AddString(ChartStringId.IndDetrendedPriceOscillator, "Detrended Price Oscillator");
			AddString(ChartStringId.IndEaseOfMovement, "Ease Of Movement");
			AddString(ChartStringId.IndMassIndex, "Mass Index");
			AddString(ChartStringId.IndMedianPrice, "Median Price");
			AddString(ChartStringId.IndMovingAverageConvergenceDivergence, "Moving Average Convergence/Divergence");
			AddString(ChartStringId.IndRateOfChange, "Rate of Change");
			AddString(ChartStringId.IndRelativeStrengthIndex, "Relative Strength Index");
			AddString(ChartStringId.IndTripleExponentialMovingAverageTema, "Triple Exponential Moving Average (TEMA)");
			AddString(ChartStringId.IndTypicalPrice, "Typical Price");
			AddString(ChartStringId.IndChaikinsVolatility, "Chaikin's Volatility");
			AddString(ChartStringId.IndWeightedClose, "Weighted Close");
			AddString(ChartStringId.IndWilliamsR, "Williams %R");
			AddString(ChartStringId.IndTripleExponentialMovingAverageTrix, "Triple Exponential Moving Average (TRIX)");
			AddString(ChartStringId.IndBollingerBands, "Bollinger Bands");
			AddString(ChartStringId.AppDefault, "Default");
			AddString(ChartStringId.AppNatureColors, "Nature Colors");
			AddString(ChartStringId.AppPastelKit, "Pastel Kit");
			AddString(ChartStringId.AppInAFog, "In A Fog");
			AddString(ChartStringId.AppTerracottaPie, "Terracotta Pie");
			AddString(ChartStringId.AppNorthernLights, "Northern Lights");
			AddString(ChartStringId.AppChameleon, "Chameleon");
			AddString(ChartStringId.AppTheTrees, "The Trees");
			AddString(ChartStringId.AppLight, "Light");
			AddString(ChartStringId.AppGray, "Gray");
			AddString(ChartStringId.AppDark, "Dark");
			AddString(ChartStringId.AppDarkFlat, "Dark Flat");
			AddString(ChartStringId.PltDefault, "Default");
			AddString(ChartStringId.PltNatureColors, "Nature Colors");
			AddString(ChartStringId.PltPastelKit, "Pastel Kit");
			AddString(ChartStringId.PltInAFog, "In A Fog");
			AddString(ChartStringId.PltTerracottaPie, "Terracotta Pie");
			AddString(ChartStringId.PltNorthernLights, "Northern Lights");
			AddString(ChartStringId.PltChameleon, "Chameleon");
			AddString(ChartStringId.PltTheTrees, "The Trees");
			AddString(ChartStringId.PltMixed, "Mixed");
			AddString(ChartStringId.PltOffice, "Office");
			AddString(ChartStringId.PltBlackAndWhite, "Black and White");
			AddString(ChartStringId.PltGrayscale, "Grayscale");
			AddString(ChartStringId.PltApex, "Apex");
			AddString(ChartStringId.PltAspect, "Aspect");
			AddString(ChartStringId.PltCivic, "Civic");
			AddString(ChartStringId.PltConcourse, "Concourse");
			AddString(ChartStringId.PltEquity, "Equity");
			AddString(ChartStringId.PltFlow, "Flow");
			AddString(ChartStringId.PltFoundry, "Foundry");
			AddString(ChartStringId.PltMedian, "Median");
			AddString(ChartStringId.PltMetro, "Metro");
			AddString(ChartStringId.PltModule, "Module");
			AddString(ChartStringId.PltOpulent, "Opulent");
			AddString(ChartStringId.PltOriel, "Oriel");
			AddString(ChartStringId.PltOrigin, "Origin");
			AddString(ChartStringId.PltPaper, "Paper");
			AddString(ChartStringId.PltSolstice, "Solstice");
			AddString(ChartStringId.PltTechnic, "Technic");
			AddString(ChartStringId.PltTrek, "Trek");
			AddString(ChartStringId.PltUrban, "Urban");
			AddString(ChartStringId.PltVerve, "Verve");
			AddString(ChartStringId.PltIndDefault, "Default");
			AddString(ChartStringId.PltOffice2013, "Office 2013");
			AddString(ChartStringId.PltBlueWarm, "Blue Warm");
			AddString(ChartStringId.PltBlue, "Blue");
			AddString(ChartStringId.PltBlueII, "Blue II");
			AddString(ChartStringId.PltBlueGreen, "Blue Green");
			AddString(ChartStringId.PltGreen, "Green");
			AddString(ChartStringId.PltGreenYellow, "Green Yellow");
			AddString(ChartStringId.PltYellow, "Yellow");
			AddString(ChartStringId.PltYellowOrange, "Yellow Orange");
			AddString(ChartStringId.PltOrange, "Orange");
			AddString(ChartStringId.PltOrangeRed, "Orange Red");
			AddString(ChartStringId.PltRedOrange, "Red Orange");
			AddString(ChartStringId.PltRed, "Red");
			AddString(ChartStringId.PltRedViolet, "Red Violet");
			AddString(ChartStringId.PltViolet, "Violet");
			AddString(ChartStringId.PltVioletII, "Violet II");
			AddString(ChartStringId.PltMarquee, "Marquee");
			AddString(ChartStringId.PltSlipstream, "Slipstream");
			AddString(ChartStringId.DefaultMinValue, "Min");
			AddString(ChartStringId.DefaultMaxValue, "Max");
			AddString(ChartStringId.IncompatibleSeriesView, "(incompatible)");
			AddString(ChartStringId.InvisibleSeriesView, "(invisible)");
			AddString(ChartStringId.IncompatibleSeriesHeader, "This series is incompatible:\r\n");
			AddString(ChartStringId.IncompatibleSeriesMessage, "by {0} with \"{1}\"");
			AddString(ChartStringId.PrimaryAxisXName, "Primary AxisX");
			AddString(ChartStringId.PrimaryAxisYName, "Primary AxisY");
			AddString(ChartStringId.IOCaption, "Illegal Operation");
			AddString(ChartStringId.IODeleteAxis, "The primary axis can't be deleted. If you want to hide it, set its Visible property to false.");
			AddString(ChartStringId.IODeleteDefaultPane, "The default pane can't be deleted.");
			AddString(ChartStringId.PrintSizeModeNone, "None (a chart is printed with the size identical to that\nshown on the form)");
			AddString(ChartStringId.PrintSizeModeStretch, "Stretch (a chart is stretched or shrunk to fit the page\non which it is printed)");
			AddString(ChartStringId.PrintSizeModeZoom, "Zoom (a chart is resized proportionally (without clipping),\nso that it best fits the page on which it is printed)");
			AddString(ChartStringId.StyleAllColors, "All Colors");
			AddString(ChartStringId.StyleColorNumber, "Color {0}");
			AddString(ChartStringId.DefaultPaneName, "Default Pane");
			AddString(ChartStringId.QuarterFormat, "Q{0}");
			AddString(ChartStringId.OthersArgument, "Others");
			AddString(ChartStringId.ExplodedPointsDialogExplodedColumn, "Exploded");
			AddString(ChartStringId.ScaleTypeAuto, "auto");
			AddString(ChartStringId.ScaleTypeQualitative, "qualitative");
			AddString(ChartStringId.ScaleTypeNumerical, "numeric");
			AddString(ChartStringId.ScaleTypeDateTime, "date-time");
			AddString(ChartStringId.FunctionNameMin, "MIN");
			AddString(ChartStringId.FunctionNameMax, "MAX");
			AddString(ChartStringId.FunctionNameSum, "SUM");
			AddString(ChartStringId.FunctionNameAverage, "AVERAGE");
			AddString(ChartStringId.FunctionNameCount, "COUNT");
			AddString(ChartStringId.FunctionArgumentName, "Argument");
			AddString(ChartStringId.TitleSummaryFunction, "Summary Function");
			AddString(ChartStringId.PanesVisibilityDialogVisibleColumn, "Visible");
			AddString(ChartStringId.PanesVisibilityDialogPanesColumn, "Panes");
			AddString(ChartStringId.FibonacciArcs, "Fibonacci Arcs");
			AddString(ChartStringId.FibonacciFans, "Fibonacci Fans");
			AddString(ChartStringId.FibonacciRetracement, "Fibonacci Retracement");
			AddString(ChartStringId.DefaultSmallChartText, "Increase the chart's size,\nto view its layout.");
			AddString(ChartStringId.AnnotationChartAnchorPoint, "Chart Anchor Point");
			AddString(ChartStringId.AnnotationPaneAnchorPoint, "Pane Anchor Point");
			AddString(ChartStringId.AnnotationSeriesPointAnchorPoint, "Series Point Anchor Point");
			AddString(ChartStringId.AnnotationFreePosition, "Free Position");
			AddString(ChartStringId.AnnotationRelativePosition, "Relative Position");
			AddString(ChartStringId.TextAnnotation, "Text Annotation");
			AddString(ChartStringId.ImageAnnotation, "Image Annotation");
			AddString(ChartStringId.ToolTipMousePosition, "Mouse Position");
			AddString(ChartStringId.ToolTipFreePosition, "Free Position");
			AddString(ChartStringId.ToolTipRelativePosition, "Relative Position");
			AddString(ChartStringId.CrosshairLabelMousePosition, "Mouse Position");
			AddString(ChartStringId.CrosshairLabelFreePosition, "Free Position");
			AddString(ChartStringId.IncorrectSeriesCollectionToolTipText, "There is no series in the chart's collection with at least one series point.");
			AddString(ChartStringId.IncorrectDiagramTypeToolTipText, "There is no panes to anchor to, because the chart's diagram type doesn't support panes.");
			AddString(ChartStringId.ChartControlDockTarget, "Chart Control");
			AddString(ChartStringId.Holidays, "Holidays");
			AddString(ChartStringId.ExactWorkdays, "Exact Workdays");
			AddString(ChartStringId.Holiday, "Holiday");
			AddString(ChartStringId.Workday, "Workday");
			AddString(ChartStringId.HolidaysImportFilter, "DevExpress Scheduler holidays files (*.xml)|*.xml|Microsoft Office Outlook holidays files (*.hol)|*.hol|Text files (*.txt)|*.txt|All files (*.*)|*.*");
			AddString(ChartStringId.AllHolidays, "All Holidays");
			AddString(ChartStringId.AlternateTextPlaceholder, "The{0} chart{1}.");
			AddString(ChartStringId.AlternateTextSeriesPlaceholder, " showing {0}");
			AddString(ChartStringId.AlternateTextSeriesText, "{0} series");
			AddString(ChartStringId.ColumnAnnotations, "Annotations");
			AddString(ChartStringId.ColumnColor, "Color");
			AddString(ChartStringId.ColumnLinks, "Links");
			AddString(ChartStringId.InvalidRangeControlText, "This chart diagram can not be displayed in a Range Control.");
			AddString(ChartStringId.CmdEmptyMenuCaption, "");
			AddString(ChartStringId.CmdEmptyDescription, "");
			AddString(ChartStringId.CmdCreateBarChartMenuCaption, "Clustered Column");
			AddString(ChartStringId.CmdCreateBarChartDescription, "Compare values across categories by using vertical rectangles.\n\nUse it when the order of categories is not important or for displaying item counts such as a histogram.");
			AddString(ChartStringId.CmdCreateBar3DChartMenuCaption, "3-D Clustered Column");
			AddString(ChartStringId.CmdCreateBar3DChartDescription, "Compare values across categories and display clustered columns in 3-D format.");
			AddString(ChartStringId.CmdCreateFullStackedBarChartMenuCaption, "100% Stacked Column");
			AddString(ChartStringId.CmdCreateFullStackedBarChartDescription, "Compare the percentage that each value contributes to a total across categories by using vertical rectangles.\n\nUse it to emphasize the proportion of each data series.");
			AddString(ChartStringId.CmdCreateFullStackedBar3DChartMenuCaption, "100% Stacked Column in 3-D");
			AddString(ChartStringId.CmdCreateFullStackedBar3DChartDescription, "Compare the percentage each value contributes to a total across categories and display 100% stacked columns in 3-D format.");
			AddString(ChartStringId.CmdCreateSideBySideFullStackedBarChartMenuCaption, "Clustered 100% Stacked Column");
			AddString(ChartStringId.CmdCreateSideBySideFullStackedBarChartDescription, "Combine the advantages of both the 100% Stacked Column and Clustered Column chart types, so that you can stack different columns, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateSideBySideFullStackedBar3DChartMenuCaption, "Clustered 100% Stacked Column in 3-D");
			AddString(ChartStringId.CmdCreateSideBySideFullStackedBar3DChartDescription, "Combine the advantages of both the 100% Stacked Column and Clustered Column chart types in 3-D format, so that you can stack different columns, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateSideBySideStackedBarChartMenuCaption, "Clustered Stacked Column");
			AddString(ChartStringId.CmdCreateSideBySideStackedBarChartDescription, "Combine the advantages of both the Stacked Column and Clustered Column chart types, so that you can stack different columns, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateSideBySideStackedBar3DChartMenuCaption, "Clustered Stacked Column in 3-D");
			AddString(ChartStringId.CmdCreateSideBySideStackedBar3DChartDescription, "Combine the advantages of both the Stacked Column and Clustered Column chart types in 3-D format, so that you can stack different columns, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateStackedBarChartMenuCaption, "Stacked Column");
			AddString(ChartStringId.CmdCreateStackedBarChartDescription, "Compare the contribution of each value to a total across categories by using vertical rectangles.\n\nUse it to emphasize the total across series for one category.");
			AddString(ChartStringId.CmdCreateStackedBar3DChartMenuCaption, "Stacked Column in 3-D");
			AddString(ChartStringId.CmdCreateStackedBar3DChartDescription, "Compare the contribution of each value to a total across categories and display stacked columns in 3-D format.");
			AddString(ChartStringId.CmdCreateManhattanBarChartMenuCaption, "3-D Column");
			AddString(ChartStringId.CmdCreateManhattanBarChartDescription, "Compare values across categories and across series on three axes.\n\nUse it when the categories and series are equally important.");
			AddString(ChartStringId.CmdCreateRangeBarChartMenuCaption, "Range Column");
			AddString(ChartStringId.CmdCreateRangeBarChartDescription, "Display vertical columns along the Y-axis (the axis of values). Each column represents a range of data for each argument value.");
			AddString(ChartStringId.CmdCreateSideBySideRangeBarChartMenuCaption, "Clustered Range Column");
			AddString(ChartStringId.CmdCreateSideBySideRangeBarChartDescription, "Show activity columns from different series grouped by their arguments. Each column represents a range of data with two values for each argument value.");
			AddString(ChartStringId.CmdCreateConeBar3DChartMenuCaption, "Clustered Cone");
			AddString(ChartStringId.CmdCreateConeBar3DChartDescription, "Compare values across categories.");
			AddString(ChartStringId.CmdCreateConeFullStackedBar3DChartMenuCaption, "100% Stacked Cone");
			AddString(ChartStringId.CmdCreateConeFullStackedBar3DChartDescription, "Compare the percentage each value contributes to a total across categories.");
			AddString(ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartMenuCaption, "Clustered 100% Stacked Cone");
			AddString(ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartDescription, "Combine the advantages of both the 100% Stacked Cone and Clustered Cone chart types, so that you can stack different cones, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateConeSideBySideStackedBar3DChartMenuCaption, "Clustered Stacked Cone");
			AddString(ChartStringId.CmdCreateConeSideBySideStackedBar3DChartDescription, "Combine the advantages of both the Stacked Cone and Clustered Cone chart types, so that you can stack different cones, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateConeStackedBar3DChartMenuCaption, "Stacked Cone");
			AddString(ChartStringId.CmdCreateConeStackedBar3DChartDescription, "Compare the contribution of each value to a total across categories.");
			AddString(ChartStringId.CmdCreateConeManhattanBarChartMenuCaption, "3-D Cone");
			AddString(ChartStringId.CmdCreateConeManhattanBarChartDescription, "Compare values across categories and across series and display a cone chart on three axes.");
			AddString(ChartStringId.CmdCreateCylinderBar3DChartMenuCaption, "Clustered Cylinder");
			AddString(ChartStringId.CmdCreateCylinderBar3DChartDescription, "Compare values across categories.");
			AddString(ChartStringId.CmdCreateCylinderFullStackedBar3DChartMenuCaption, "100% Stacked Cylinder");
			AddString(ChartStringId.CmdCreateCylinderFullStackedBar3DChartDescription, "Compare the percentage each value contributes to a total across categories.");
			AddString(ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartMenuCaption, "Clustered 100% Stacked Cylinder");
			AddString(ChartStringId.CmdCreateCylinderSideBySideFullStackedBar3DChartDescription, "Combine the advantages of both the 100% Stacked Cylinder and Clustered Cylinder chart types, so that you can stack different cylinders, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartMenuCaption, "Clustered Stacked Cylinder");
			AddString(ChartStringId.CmdCreateCylinderSideBySideStackedBar3DChartDescription, "Combine the advantages of both the Stacked Cylinder and Clustered Cylinder chart types, so that you can stack different cylinders, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateCylinderStackedBar3DChartMenuCaption, "Stacked Cylinder");
			AddString(ChartStringId.CmdCreateCylinderStackedBar3DChartDescription, "Compare the contribution of each value to a total across categories.");
			AddString(ChartStringId.CmdCreateCylinderManhattanBarChartMenuCaption, "3-D Cylinder");
			AddString(ChartStringId.CmdCreateCylinderManhattanBarChartDescription, "Compare values across categories and across series and display a cylinder chart on three axes.");
			AddString(ChartStringId.CmdCreatePyramidBar3DChartMenuCaption, "Clustered Pyramid");
			AddString(ChartStringId.CmdCreatePyramidBar3DChartDescription, "Compare values across categories.");
			AddString(ChartStringId.CmdCreatePyramidFullStackedBar3DChartMenuCaption, "100% Stacked Pyramid");
			AddString(ChartStringId.CmdCreatePyramidFullStackedBar3DChartDescription, "Compare the percentage each value contributes to a total across categories.");
			AddString(ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartMenuCaption, "Clustered 100% Stacked Pyramid");
			AddString(ChartStringId.CmdCreatePyramidSideBySideFullStackedBar3DChartDescription, "Combine the advantages of both the 100% Stacked Pyramid and Clustered Pyramid chart types, so that you can stack different pyramids, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartMenuCaption, "Clustered Stacked Pyramid");
			AddString(ChartStringId.CmdCreatePyramidSideBySideStackedBar3DChartDescription, "Combine the advantages of both the Stacked Pyramid and Clustered Pyramid chart types, so that you can stack different pyramids, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreatePyramidStackedBar3DChartMenuCaption, "Stacked Pyramid");
			AddString(ChartStringId.CmdCreatePyramidStackedBar3DChartDescription, "Compare the contribution of each value to a total across categories.");
			AddString(ChartStringId.CmdCreatePyramidManhattanBarChartMenuCaption, "3-D Pyramid");
			AddString(ChartStringId.CmdCreatePyramidManhattanBarChartDescription, "Compare values across categories and across series and display a pyramid chart on three axes.");
			AddString(ChartStringId.CmdCreateAreaChartMenuCaption, "Area");
			AddString(ChartStringId.CmdCreateAreaChartDescription, "Display the trend of values over time or categories.");
			AddString(ChartStringId.CmdCreateArea3DChartMenuCaption, "3-D Area");
			AddString(ChartStringId.CmdCreateArea3DChartDescription, "Display data as filled areas on a diagram, with each data point displayed as a peak or hollow in the area.\n\nUse it when you need to show trends for several series on the same diagram, and also show the relationship of the parts to the whole.");
			AddString(ChartStringId.CmdCreateFullStackedAreaChartMenuCaption, "100% Stacked Area");
			AddString(ChartStringId.CmdCreateFullStackedAreaChartDescription, "Display the trend of the percentage each value contributes over time or categories.\n\nUse it to emphasize the trend in the proportion of each series.");
			AddString(ChartStringId.CmdCreateFullStackedArea3DChartMenuCaption, "100% Stacked Area in 3-D");
			AddString(ChartStringId.CmdCreateFullStackedArea3DChartDescription, "Display data as areas on a diagram, so that the value of each data point is stacked with all the other corresponding data points' values.\n\nUse it for comparing the percentage values of several series for the same point arguments.");
			AddString(ChartStringId.CmdCreateFullStackedSplineAreaChartMenuCaption, "100% Stacked Spline Area");
			AddString(ChartStringId.CmdCreateFullStackedSplineAreaChartDescription, "Behave similar to 100% Stacked Area, but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateFullStackedSplineArea3DChartMenuCaption, "100% Stacked Spline Area in 3-D");
			AddString(ChartStringId.CmdCreateFullStackedSplineArea3DChartDescription, "Behave similar to 100% Stacked Area Chart in 3D, but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateSplineAreaChartMenuCaption, "Spline Area");
			AddString(ChartStringId.CmdCreateSplineAreaChartDescription, "Behave similar to Area Chart but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateSplineArea3DChartMenuCaption, "Spline Area in 3-D");
			AddString(ChartStringId.CmdCreateSplineArea3DChartDescription, "Behave similar to 3D Area Chart, but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateStackedAreaChartMenuCaption, "Stacked Area");
			AddString(ChartStringId.CmdCreateStackedAreaChartDescription, "Display the trend of the contribution of each value over time or categories.\n\nUse it to emphasize the trend in the total across series for one category.");
			AddString(ChartStringId.CmdCreateStackedArea3DChartMenuCaption, "Stacked Area in 3-D");
			AddString(ChartStringId.CmdCreateStackedArea3DChartDescription, "Display series as areas on a diagram, so that the value of each data point is aggregated with the underlying data points' values.");
			AddString(ChartStringId.CmdCreateStackedSplineAreaChartMenuCaption, "Stacked Spline Area");
			AddString(ChartStringId.CmdCreateStackedSplineAreaChartDescription, "Behave similar to Stacked Area Chart but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateStackedSplineArea3DChartMenuCaption, "Stacked Spline Area in 3-D");
			AddString(ChartStringId.CmdCreateStackedSplineArea3DChartDescription, "Behave similar to Stacked Area in 3D chart, but plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateStepAreaChartMenuCaption, "Step Area");
			AddString(ChartStringId.CmdCreateStepAreaChartDescription, "Show how much values have changed for different points of the same series.");
			AddString(ChartStringId.CmdCreateStepArea3DChartMenuCaption, "Step Area in 3-D");
			AddString(ChartStringId.CmdCreateStepArea3DChartDescription, "Show to what extent values have changed for different points in the same series.");
			AddString(ChartStringId.CmdCreateRangeAreaChartMenuCaption, "Range Area");
			AddString(ChartStringId.CmdCreateRangeAreaChartDescription, "Display series as filled areas on a diagram, with two data points that define minimum and maximum limits.\n\nUse it when you need to accentuate the delta between start and end values.");
			AddString(ChartStringId.CmdCreateRangeArea3DChartMenuCaption, "Range Area in 3-D");
			AddString(ChartStringId.CmdCreateRangeArea3DChartDescription, "Display series as filled areas on a diagram, with two data points that define minimum and maximum limits.\n\nUse it when you need to accentuate the delta between start and end values.");
			AddString(ChartStringId.CmdCreateStockChartMenuCaption, "Stock");
			AddString(ChartStringId.CmdCreateStockChartDescription, "Show variation in stock prices over the course of a day. The Open and Close prices are represented by left and right lines on each point, and the Low and High prices are represented by the bottom and top values of the vertical line which is shown at each point.");
			AddString(ChartStringId.CmdCreateCandleStickChartMenuCaption, "Candle Stick");
			AddString(ChartStringId.CmdCreateCandleStickChartDescription, "Show the variation in the price of stock over the course of a day. The Open and Close prices are represented by a filled rectangle, and the Low and High prices are represented by the bottom and top values of the vertical line which is shown at each point.");
			AddString(ChartStringId.CmdCreateFunnelChartMenuCaption, "Funnel");
			AddString(ChartStringId.CmdCreateFunnelChartDescription, "Display a wide area at the top, indicating the total points' value, while other areas are proportionally smaller.\n\nUse it when it is necessary to represent stages in a sales process, show the amount of potential revenue for each stage, as well as identify potential problem areas in an organization's sales processes.");
			AddString(ChartStringId.CmdCreateFunnel3DChartMenuCaption, "3-D Funnel");
			AddString(ChartStringId.CmdCreateFunnel3DChartDescription, "Display a wide area at the top, indicating the total points' value, while other areas are proportionally smaller.\n\nUse it when it is necessary to represent stages in a sales process, show the amount of potential revenue for each stage, as well as identify potential problem areas in an organization's sales processes.");
			AddString(ChartStringId.CmdCreateGanttChartMenuCaption, "Gantt");
			AddString(ChartStringId.CmdCreateGanttChartDescription, "Track different activities during the time frame.");
			AddString(ChartStringId.CmdCreateSideBySideGanttChartMenuCaption, "Clustered Gantt");
			AddString(ChartStringId.CmdCreateSideBySideGanttChartDescription, "Display horizontal bars along the time axis. Each bar represents a separate event with the start and end values, hence these charts are used to track different activities during the time frame.\n\nUse it when it's necessary to show activity bars from different series one above another, to compare their duration.");
			AddString(ChartStringId.CmdCreateLineChartMenuCaption, "Line");
			AddString(ChartStringId.CmdCreateLineChartDescription, "Display trend overtime (dates, years) or ordered categories. Useful when there are many data points and the order is important.");
			AddString(ChartStringId.CmdCreateLine3DChartMenuCaption, "3-D Line");
			AddString(ChartStringId.CmdCreateLine3DChartDescription, "Display each row or column of data as a 3-D ribbon on three axes.");
			AddString(ChartStringId.CmdCreateFullStackedLineChartMenuCaption, "100% Stacked Line");
			AddString(ChartStringId.CmdCreateFullStackedLineChartDescription, "Display the trend of the percentage each value contributes over time or ordered categories.");
			AddString(ChartStringId.CmdCreateFullStackedLine3DChartMenuCaption, "100% Stacked Line in 3-D");
			AddString(ChartStringId.CmdCreateFullStackedLine3DChartDescription, "Display all series stacked and is useful when it is necessary to compare how much each series adds to the total aggregate value for specific arguments (as percents).");
			AddString(ChartStringId.CmdCreateScatterLineChartMenuCaption, "Scatter Line");
			AddString(ChartStringId.CmdCreateScatterLineChartDescription, "Represent series points in the same order that they have in the collection.");
			AddString(ChartStringId.CmdCreateSplineChartMenuCaption, "Spline");
			AddString(ChartStringId.CmdCreateSplineChartDescription, "Plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateSpline3DChartMenuCaption, "3-D Spline");
			AddString(ChartStringId.CmdCreateSpline3DChartDescription, "Plot a fitted curve through each data point in a series.");
			AddString(ChartStringId.CmdCreateStackedLineChartMenuCaption, "Stacked Line");
			AddString(ChartStringId.CmdCreateStackedLineChartDescription, "Display the trend of the contribution of each value over time or ordered categories.");
			AddString(ChartStringId.CmdCreateStackedLine3DChartMenuCaption, "Stacked Line in 3-D");
			AddString(ChartStringId.CmdCreateStackedLine3DChartDescription, "Display all points from different series in a stacked manner and is useful when it is necessary to compare how much each series adds to the total aggregate value for specific arguments.");
			AddString(ChartStringId.CmdCreateStepLineChartMenuCaption, "Step Line");
			AddString(ChartStringId.CmdCreateStepLineChartDescription, "Show to what extent values have changed for different points in the same series.");
			AddString(ChartStringId.CmdCreateStepLine3DChartMenuCaption, "Step Line in 3-D");
			AddString(ChartStringId.CmdCreateStepLine3DChartDescription, "Show to what extent values have changed for different points in the same series.");
			AddString(ChartStringId.CmdCreatePieChartMenuCaption, "Pie");
			AddString(ChartStringId.CmdCreatePieChartDescription, "Display the contribution of each value to a total.\n\nUse it when the values can be added together or when you have only one data series and all values are positive.");
			AddString(ChartStringId.CmdCreatePie3DChartMenuCaption, "Pie in 3-D");
			AddString(ChartStringId.CmdCreatePie3DChartDescription, "Display the contribution of each value to a total.");
			AddString(ChartStringId.CmdCreateDoughnutChartMenuCaption, "Doughnut");
			AddString(ChartStringId.CmdCreateDoughnutChartDescription, "Display the contribution of each value to a total like a pie chart, but it can contain multiple series.");
			AddString(ChartStringId.CmdCreateNestedDoughnutChartMenuCaption, "Nested Doughnut");
			AddString(ChartStringId.CmdCreateNestedDoughnutChartDescription, "Display the contribution of each value to a total while comparing series with one doughnut nested in another one.");
			AddString(ChartStringId.CmdCreateDoughnut3DChartMenuCaption, "Doughnut in 3-D");
			AddString(ChartStringId.CmdCreateDoughnut3DChartDescription, "Compare the percentage values of different point arguments in the same series, and illustrate these values as easy to understand pie slices, but with a hole in its center.");
			AddString(ChartStringId.CmdCreatePointChartMenuCaption, "Point");
			AddString(ChartStringId.CmdCreatePointChartDescription, "Use it when it's necessary to show stand-alone data points on the same chart plot.");
			AddString(ChartStringId.CmdCreateBubbleChartMenuCaption, "Bubble");
			AddString(ChartStringId.CmdCreateBubbleChartDescription, "Resemble a Scatter chart, but compare sets of three values instead of two. The third value determines the size of the bubble marker.");
			AddString(ChartStringId.CmdCreateRadarPointChartMenuCaption, "Radar Point");
			AddString(ChartStringId.CmdCreateRadarPointChartDescription, "Show points from two or more different series on the same points arguments on a circular grid that has multiple axes along which data can be plotted.");
			AddString(ChartStringId.CmdCreateRadarLineChartMenuCaption, "Radar Line");
			AddString(ChartStringId.CmdCreateRadarLineChartDescription, "Show trends for several series and compare their values for the same points arguments on a circular grid that has multiple axes along which data can be plotted.");
			AddString(ChartStringId.CmdCreateScatterRadarLineChartMenuCaption, "Scatter Radar Line");
			AddString(ChartStringId.CmdCreateScatterRadarLineChartDescription, "Display data as a line on a circular grid that has multiple axis along which data can be plotted. The series points are drawn in the circular grid in the same order that they have in the series point collection.");
			AddString(ChartStringId.CmdCreateRadarAreaChartMenuCaption, "Radar Area");
			AddString(ChartStringId.CmdCreateRadarAreaChartDescription, "Display series as filled area on a circular grid that has multiple axes along which data can be plotted.");
			AddString(ChartStringId.CmdCreatePolarPointChartMenuCaption, "Polar Point");
			AddString(ChartStringId.CmdCreatePolarPointChartDescription, "Show points from two or more different series on the same circular diagram on the basis of angles.");
			AddString(ChartStringId.CmdCreatePolarLineChartMenuCaption, "Polar Line");
			AddString(ChartStringId.CmdCreatePolarLineChartDescription, "Show trends for several series and compare their values for the same points arguments on a circular diagram on the basis of angles.");
			AddString(ChartStringId.CmdCreateScatterPolarLineChartMenuCaption, "Scatte Polar Line");
			AddString(ChartStringId.CmdCreateScatterPolarLineChartDescription, "Show trends for several series and compare their values for the same point arguments on a circular diagram on the basis of angles. The series points are drawn in the circular grid in the same order that they have in the series point collection.");
			AddString(ChartStringId.CmdCreatePolarAreaChartMenuCaption, "Polar Area");
			AddString(ChartStringId.CmdCreatePolarAreaChartDescription, "Display series as filled area on a circular diagram on the basis of angles.");
			AddString(ChartStringId.CmdCreateRotatedBarChartMenuCaption, "Bar");
			AddString(ChartStringId.CmdCreateRotatedBarChartDescription, "Insert a bar chart.\n\nBar charts are the best chart type for comparing multiple values.");
			AddString(ChartStringId.CmdCreateRotatedBarChartMenuCaption, "Bar");
			AddString(ChartStringId.CmdCreateRotatedBarChartDescription, "Summarize and display categories of data and compare amounts or values between different categories.");
			AddString(ChartStringId.CmdCreateRotatedFullStackedBarChartMenuCaption, "100% Stacked Bar");
			AddString(ChartStringId.CmdCreateRotatedFullStackedBarChartDescription, "Compare the percentage each value contributes to a total across categories using horizontal rectangles.\n\nUse it when the values on the chart represent durations or when the category text is very long.");
			AddString(ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartMenuCaption, "Clustered 100% Stacked Bar");
			AddString(ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartDescription, "Combine the advantages of both the 100% Stacked Bar and Clustered Bar chart types, so you can stack different bars, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateRotatedSideBySideStackedBarChartMenuCaption, "Clustered Stacked Bar");
			AddString(ChartStringId.CmdCreateRotatedSideBySideStackedBarChartDescription, "Combine the advantages of both the Stacked Bar and Clustered Bar chart types, so that you can stack different bars, and combine them into groups across the same axis value.");
			AddString(ChartStringId.CmdCreateRotatedStackedBarChartMenuCaption, "Stacked Bar");
			AddString(ChartStringId.CmdCreateRotatedStackedBarChartDescription, "Compare the contribution of each value to a total across categories by using horizontal rectangles.");
			AddString(ChartStringId.CmdPrintMenuCaption, "Print");
			AddString(ChartStringId.CmdPrintDescription, "Select a printer, number of copies and other printing options before printing.");
			AddString(ChartStringId.CmdPrintPreviewMenuCaption, "Print Preview");
			AddString(ChartStringId.CmdPrintPreviewDescription, "Preview and make changes to pages before printing.");
			AddString(ChartStringId.CmdExportPlaceHolderMenuCaption, "Export");
			AddString(ChartStringId.CmdExportPlaceHolderDescription, "Export the current document in one of the available formats, and save it to the file on a disk.");
			AddString(ChartStringId.CmdExportToPDFMenuCaption, "Export to PDF");
			AddString(ChartStringId.CmdExportToPDFDescription, "Adobe Portable Document Format");
			AddString(ChartStringId.CmdExportToHTMLMenuCaption, "Export to HTML");
			AddString(ChartStringId.CmdExportToHTMLDescription, "Web Page");
			AddString(ChartStringId.CmdExportToMHTMenuCaption, "Export to MHT");
			AddString(ChartStringId.CmdExportToMHTDescription, "Single File Web Page");
			AddString(ChartStringId.CmdExportToXLSMenuCaption, "Export to XLS");
			AddString(ChartStringId.CmdExportToXLSDescription, "Microsoft Excel 2000-2003 Work Book");
			AddString(ChartStringId.CmdExportToXLSXMenuCaption, "Export to XLSX");
			AddString(ChartStringId.CmdExportToXLSXDescription, "Microsoft Excel 2007 Work Book");
			AddString(ChartStringId.CmdExportToRTFMenuCaption, "Export to RTF");
			AddString(ChartStringId.CmdExportToRTFDescription, "Rich Text Format");
			AddString(ChartStringId.CmdExportToImagePlaceHolderMenuCaption, "Export to Image");
			AddString(ChartStringId.CmdExportToBMPMenuCaption, "BMP");
			AddString(ChartStringId.CmdExportToBMPDescription, "Bitmap Picture");
			AddString(ChartStringId.CmdExportToGIFMenuCaption, "GIF");
			AddString(ChartStringId.CmdExportToGIFDescription, "Graphics Interchange Format");
			AddString(ChartStringId.CmdExportToJPEGMenuCaption, "JPEG");
			AddString(ChartStringId.CmdExportToJPEGDescription, "JPEG Image");
			AddString(ChartStringId.CmdExportToPNGMenuCaption, "PNG");
			AddString(ChartStringId.CmdExportToPNGDescription, "Portable Network Graphics");
			AddString(ChartStringId.CmdExportToTIFFMenuCaption, "TIFF");
			AddString(ChartStringId.CmdExportToTIFFDescription, "Tagged Image File Format");
			AddString(ChartStringId.CmdRunWizardMenuCaption, "Run Chart Wizard...");
			AddString(ChartStringId.CmdRunWizardDescription, "The Chart Wizard is invoked to help you adjust the main chart settings in one place.");
			AddString(ChartStringId.CmdSaveAsTemplateMenuCaption, "Save As\nTemplate");
			AddString(ChartStringId.CmdSaveAsTemplateDescription, "Create a template with the same setting as the current chart.");
			AddString(ChartStringId.CmdLoadTemplateMenuCaption, "Load\nTemplate");
			AddString(ChartStringId.CmdLoadTemplateDescription, "Load a chart from template");
			AddString(ChartStringId.CmdCreateBarChartPlaceHolderMenuCaption, "Column");
			AddString(ChartStringId.CmdCreateBarChartPlaceHolderDescription, "Insert a column chart.\n\nColumn charts are used to compare values across categories.");
			AddString(ChartStringId.CmdCreateAreaChartPlaceHolderMenuCaption, "Area");
			AddString(ChartStringId.CmdCreateAreaChartPlaceHolderDescription, "Insert an area chart.\n\nArea charts emphasize differences between several sets of data over a period of time.");
			AddString(ChartStringId.CmdCreatePieChartPlaceHolderMenuCaption, "Pie");
			AddString(ChartStringId.CmdCreatePieChartPlaceHolderDescription, "Insert a pie chart.\n\nPie charts display the contribution of each value to a total.\n\nUse it when values can be added together or when you have only one data series and all values are positive.");
			AddString(ChartStringId.CmdCreateLineChartPlaceHolderMenuCaption, "Line");
			AddString(ChartStringId.CmdCreateLineChartPlaceHolderDescription, "Insert a line chart.\n\nLine charts are used to display trends overtime.");
			AddString(ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderMenuCaption, "Other Charts");
			AddString(ChartStringId.CmdCreateOtherSeriesTypesChartPlaceHolderDescription, "Insert a point, funnel, financial, radar, polar, range, or gantt chart.");
			AddString(ChartStringId.CmdCreateRotatedBarChartPlaceHolderMenuCaption, "Bar");
			AddString(ChartStringId.CmdCreateRotatedBarChartPlaceHolderDescription, "Insert a bar chart.\n\nBar charts are the best chart type for comparing multiple values.");
			AddString(ChartStringId.CmdChangePalettePlaceHolderMenuCaption, "Palette");
			AddString(ChartStringId.CmdChangePalettePlaceHolderDescription, "Current: Office\n\nChange the palette for the current chart.");
			AddString(ChartStringId.CmdChangeAppearancePlaceHolderMenuCaption, "Appearance");
			AddString(ChartStringId.CmdChangeAppearancePlaceHolderDescription, "Use the drop-down list at the top, to set a palette for the chart. It determines the overall look of the chart, by painting its key elements.");
			AddString(ChartStringId.CmdColumn2DGroupPlaceHolderMenuCaption, "2-D Column");
			AddString(ChartStringId.CmdColumn3DGroupPlaceHolderMenuCaption, "3-D Column");
			AddString(ChartStringId.CmdColumnCylinderGroupPlaceHolderMenuCaption, "Cylinder");
			AddString(ChartStringId.CmdColumnConeGroupPlaceHolderMenuCaption, "Cone");
			AddString(ChartStringId.CmdColumnPyramidGroupPlaceHolderMenuCaption, "Pyramid");
			AddString(ChartStringId.CmdLine2DGroupPlaceHolderMenuCaption, "2-D Line");
			AddString(ChartStringId.CmdLine3DGroupPlaceHolderMenuCaption, "3-D Line");
			AddString(ChartStringId.CmdPie2DGroupPlaceHolderMenuCaption, "2-D Pie");
			AddString(ChartStringId.CmdPie3DGroupPlaceHolderMenuCaption, "3-D Pie");
			AddString(ChartStringId.CmdBar2DGroupPlaceHolderMenuCaption, "2-D Bar");
			AddString(ChartStringId.CmdArea2DGroupPlaceHolderMenuCaption, "2-D Area");
			AddString(ChartStringId.CmdArea3DGroupPlaceHolderMenuCaption, "3-D Area");
			AddString(ChartStringId.CmdPointGroupPlaceHolderMenuCaption, "Point");
			AddString(ChartStringId.CmdFunnelGroupPlaceHolderMenuCaption, "Funnel");
			AddString(ChartStringId.CmdFinancialGroupPlaceHolderMenuCaption, "Financial");
			AddString(ChartStringId.CmdRadarGroupPlaceHolderMenuCaption, "Radar");
			AddString(ChartStringId.CmdPolarGroupPlaceHolderMenuCaption, "Polar");
			AddString(ChartStringId.CmdRangeGroupPlaceHolderMenuCaption, "Range");
			AddString(ChartStringId.CmdGanttGroupPlaceHolderMenuCaption, "Gantt");
			AddString(ChartStringId.RibbonOtherPageCaption, "Other");
			AddString(ChartStringId.RibbonDesignPageCaption, "Design");
			AddString(ChartStringId.RibbonAppearanceGroupCaption, "Appearance");
			AddString(ChartStringId.RibbonTypesGroupCaption, "Chart Type");
			AddString(ChartStringId.RibbonWizardGroupCaption, "Wizard");
			AddString(ChartStringId.RibbonTemplatesGroupCaption, "Templates");
			AddString(ChartStringId.RibbonPrintExportGroupCaption, "Print and Export");
			AddString(ChartStringId.RibbonPageCategoryCaption, "Chart Tools");
			AddString(ChartStringId.ArgumentPatternDescription, "Argument");
			AddString(ChartStringId.ValuePatternDescription, "Value");
			AddString(ChartStringId.SeriesNamePatternDescription, "Series Name");
			AddString(ChartStringId.StackedGroupPatternDescription, "Series Group");
			AddString(ChartStringId.Value1PatternDescription, "Value 1");
			AddString(ChartStringId.Value2PatternDescription, "Value 2");
			AddString(ChartStringId.WeightPatternDescription, "Weight");
			AddString(ChartStringId.HighValuePatternDescription, "High Value");
			AddString(ChartStringId.LowValuePatternDescription, "Low Value");
			AddString(ChartStringId.OpenValuePatternDescription, "Open Value");
			AddString(ChartStringId.CloseValuePatternDescription, "Close Value");
			AddString(ChartStringId.PercentValuePatternDescription, "Percent Value");
			AddString(ChartStringId.PointHintPatternDescription, "Hint");
			AddString(ChartStringId.ValueDurationPatternDescription, "Value Duration");
			AddString(ChartStringId.InvalidPlaceholder, "Invalid placeholder");
			AddString(ChartStringId.ErrorTitle, "Error");
			AddString(ChartStringId.PatternEditorPreviewCaption, "Select a placeholder to see the preview");
			AddString(ChartStringId.WizAggregateFunctionFinancial, "Financial");
			AddString(ChartStringId.WizAggregateFunctionAverage, "Average");
			AddString(ChartStringId.WizAggregateFunctionSum, "Sum");
			AddString(ChartStringId.WizAggregateFunctionNone, "None");
			AddString(ChartStringId.WizAggregateFunctionCount, "Count");
			AddString(ChartStringId.WizAggregateFunctionMaximum, "Maximum");
			AddString(ChartStringId.WizAggregateFunctionMinimum, "Minimum");
			AddString(ChartStringId.WizScaleModeManual, "Manual");
			AddString(ChartStringId.WizScaleModeAutomatic, "Automatic");
			AddString(ChartStringId.WizScaleModeContinuous, "Continuous");
			AddString(ChartStringId.PatternEditorPreviewCaption, "Select a placeholder to see the preview");
			AddString(ChartStringId.MsgIncorrectDoubleValue, "The value can't be equal to Double.NaN, Double.PositiveInfinity, or Double.NegativeInfinity.");
			AddString(ChartStringId.MsgPaletteEditorTitle, "Palettes Editor");
			AddString(ChartStringId.MsgPaletteEditorInvalidFile, "An invalid chart palette file");
			AddString(ChartStringId.VerbDesigner, "Run Designer...");
			AddString(ChartStringId.VerbDesignerDescription, "Run the Chart Designer, which allows the properties of the chart to be edited.");
			AddString(ChartStringId.CmdRunDesignerDescription, "The Chart Designer is invoked to help you adjust the main chart settings in one place.");
			AddString(ChartStringId.CmdRunDesignerMenuCaption, "Run Chart Designer...");
			AddString(ChartStringId.RibbonDesignerGroupCaption, "Designer");
		}
		public static XtraLocalizer<ChartStringId> CreateDefaultLocalizer() {
			return new ChartResLocalizer();
		}
		public override XtraLocalizer<ChartStringId> CreateResXLocalizer() {
			return new ChartResLocalizer();
		}
		public static string GetString(ChartStringId id) {
			return Active.GetLocalizedString(id);
		}
		public static string GetDefaultString(ChartStringId id) {
			var localizer = Active as XtraResXLocalizer<ChartStringId>;
			if (localizer != null)
				return localizer.GetInvariantString(id);
			else
				return string.Empty;
		}
	}
}
