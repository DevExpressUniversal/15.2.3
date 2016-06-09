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
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.DashboardCommon.Localization {
	public enum DashboardStringId {
		DefaultNamePivotItem,
		DefaultNameGridItem,
		DefaultNameChartItem,
		DefaultNameScatterChartItem,
		DefaultNamePieItem,
		DefaultNameGaugeItem,
		DefaultNameCardItem,
		DefaultNameImageItem,
		DefaultNameTextBoxItem,
		DefaultNameRangeFilterItem,
		DefaultNameMapItem,
		DefaultNameComboBoxItem,
		DefaultNameListBoxItem,
		DefaultNameTreeViewItem,
		DefaultNameItemGroup,
		SummaryTypeCount,
		SummaryTypeCountDistinct,
		SummaryTypeSum,
		SummaryTypeMin,
		SummaryTypeMax,
		SummaryTypeAverage,
		SummaryTypeStdDev,
		SummaryTypeStdDevp,
		SummaryTypeVar,
		SummaryTypeVarp,
		GridTotalAutoTemplate,
		GridTotalValueTemplate,
		GridTotalTemplate,
		GridTotalTypeCount,
		GridTotalTypeMin,
		GridTotalTypeMax,
		GridTotalTypeAvg,
		GridTotalTypeSum,
		GroupIntervalNone,
		TextGroupIntervalAlphabetical,
		NumericGroupIntervalDiscrete,
		NumericGroupIntervalContinuous,
		DateTimeGroupIntervalYear,
		DateTimeGroupIntervalQuarter,
		DateTimeGroupIntervalMonth,
		DateTimeGroupIntervalDay,
		DateTimeGroupIntervalHour,
		DateTimeGroupIntervalMinute,
		DateTimeGroupIntervalSecond,
		DateTimeGroupIntervalDayOfYear,
		DateTimeGroupIntervalDayOfWeek,
		DateTimeGroupIntervalWeekOfYear,
		DateTimeGroupIntervalWeekOfMonth,
		DateTimeGroupIntervalMonthYear,
		DateTimeGroupIntervalQuarterYear,
		DateTimeGroupIntervalDayMonthYear,
		DateTimeGroupIntervalDateHour,
		DateTimeGroupIntervalDateHourMinute,
		DateTimeGroupIntervalDateHourMinuteSecond,
		DateTimeGroupIntervalExactDate,
		DateTimeQuarterFormat,
		DateTimeQuarterFormatSpecifier,
		ColoringModeDefault,
		ColoringModeNone,
		ColoringModeHue,
		ColorAutoAssignedNotApplied,
		ColorAutoAssigned,
		ColorInherited,
		ColorPaletteIndex,
		AxisYNameCount,
		AxisYNameValues,
		AxisXNameArguments,
		ValueCaption,
		Value1Caption,
		Value2Caption,
		WeightCaption,
		OpenCaption,
		HighCaption,
		LowCaption,
		CloseCaption,
		ActualValueCaption,
		TargetValueCaption,
		SeriesTypeBar,
		SeriesTypeStackedBar,
		SeriesTypeFullStackedBar,
		SeriesTypeSideBySideStackedBar,
		SeriesTypeSideBySideFullStackedBar,
		SeriesTypePoint,
		SeriesTypeLine,
		SeriesTypeStackedLine,
		SeriesTypeFullStackedLine,
		SeriesTypeStepLine,
		SeriesTypeSpline,
		SeriesTypeArea,
		SeriesTypeStackedArea,
		SeriesTypeFullStackedArea,
		SeriesTypeStepArea,
		SeriesTypeSplineArea,
		SeriesTypeStackedSplineArea,
		SeriesTypeFullStackedSplineArea,
		SeriesTypeSideBySideRangeBar,
		SeriesTypeRangeArea,
		SeriesTypeBubble,
		SeriesTypeStock,
		SeriesTypeCandleStick,
		SeriesTypeHighLowClose,
		SeriesTypeOpenHighLowClose,
		SeriesTypeGroupBar,
		SeriesTypeGroupPointLine,
		SeriesTypeGroupArea,
		SeriesTypeGroupRange,
		SeriesTypeGroupBubble,
		SeriesTypeGroupFinancial,
		SeriesTypeGroupRangeFilter,
		DescriptionValues,
		DescriptionMaps,
		DescriptionMapValue,
		DescriptionMapValues,
		DescriptionArgument,
		DescriptionArguments,
		DescriptionSeries,
		DescriptionSparkline,
		DescriptionSparklineArgument,
		DescriptionColumns,
		DescriptionRows,
		DescriptionDimensions,
		DescriptionMeasures,
		DescriptionGauges,
		DescriptionCards,
		DescriptionItemValue,
		DescriptionItemArgument,
		DescriptionItemSeries,
		DescriptionItemColumn,
		DescriptionItemRow,
		DescriptionItemDimension,
		DescriptionItemAuto,
		DescriptionItemMeasure,
		DescriptionItemColor,
		DescriptionItemWeight,
		DescriptionItemAxisXMeasure,
		DescriptionItemAxisYMeasure,
		DescriptionItemMeasureWeight,
		DescriptionDashboardItemName,
		DescriptionDashboardItemComponentName,
		DescriptionTooltipDimensions,
		DescriptionTooltipMeasures,
		DataEngineGrandTotal,
		RangeFilterEmptySeries,
		RangeFilterIncompatibleSeriesType,
		RangeFilterEmptyArgument,
		RangeFilterIncompatibleArgument,
		RangeFilterIncompatibleNumericScale,
		RangeFilterIncompatibleDateTimeGroupInterval,
		RangeFilterIncompatibleArgumentSorting,
		RangeFilterIncompatibleTopN,
		MessageLoading,
		MessageInvalidStream,
		MessageInternalError,
		MessageDashboardNotFoundError,
		MessageDashboardNotRelevantError,
		MessageDashboardInternalError,
		MessageIncorrectDataAssign,
		MessageDuplicatedDataItem,
		MessageIncorrectValueTopNCount,
		MessageIncorrectNumericPrecision,
		MessageInvalidCultureName,
		MessageIncorrectLayoutItemWeight,
		MessageDuplicatedLayoutItem,
		MessageLayoutDuplicatedDashboardItem,
		MessageIncorrectMoveTarget,
		MessageMasterFilterIncorrectNumberOfValues,
		MessageIncorrectFilterValueLength,
		MessageGridHasNoData,
		MessagePivotHasNoData,
		MessageIncorrectGridDashboardControlOperation,
		MessageUnassignedDashboardItemGroup,
		MessageIncorrectDashboardItemGroupAssign,
		MessageWrongAxisName,
		MessageInvalidLayoutClientWidth,
		MessageInvalidLayoutClientHeight,
		MessageSummaryCalculatedFieldInDimension,
		FormatStringDashboardItemCaption,
		FormatStringKpiElementCaption,
		FormatStringSeriesName,
		FormatStringDataItemName,
		FormatRangeFilterDisplayText,
		FormatMasterFilterDisplayText,
		DataItemsHeader,
		HiddenDataItemsHeader,
		TooltipDataItemsHeader,
		NumericFormatUnitSymbolThousands,
		NumericFormatUnitSymbolMillions,
		NumericFormatUnitSymbolBillions,
		TopNOthersValue,
		DashboardNullValue,
		DashboardErrorValue,
		ActionClearMasterFilter,
		ActionClearSelection,
		ActionDrillUp,
		ActionOtherValues,
		ActionExportTemplate,
		ActionExportTo,
		ActionExportToExcel,
		ActionExportToPdf,
		ActionExportToImage,
		ActionAllowMultiselection,
		ElementNamePies,
		ElementNameGauges,
		ElementNameCards,
		DefaultDashboardTitleText,
		DefaultNameChartPane,
		MessageInvalidFilterExpression,
		MeasureNamesDimensionName,
		EmptyColumnAliasPattern,
		PivotGridTotal,
		PivotGridGrandTotal,
		ChartTotalValue,
		LoadingDataError,
		CalculatedFieldsTreeItemName,
		CalculatedFieldTypeObject,
		CalculatedFieldTypeString,
		CalculatedFieldTypeDateTime,
		CalculatedFieldTypeInteger,
		CalculatedFieldTypeDecimal,
		CalculatedFieldTypeBoolean,
		MapAttributeDimension,
		Latitude,
		Longitude,
		Delta,
		NewCalculatedFieldNamePrefix,
		NewParameterNamePrefix,
		ParametersFormCaption,
		ParametersSelectorText,
		ButtonOK,
		ButtonCancel,
		ButtonReset,
		ButtonSubmit,
		ButtonExport,
		GridResetColumnWidths,
		GridSortAscending,
		GridSortDescending,
		GridClearSorting,
		PageLayout,
		PageLayoutAuto,
		PageLayoutPortrait,
		PageLayoutLandscape,
		PaperKind,
		PaperKindLetter,
		PaperKindLegal,
		PaperKindExecutive,
		PaperKindA5,
		PaperKindA4,
		PaperKindA3,
		ScaleMode,
		ScaleModeNone,
		ScaleModeUseScaleFactor,
		ScaleModeAutoFitToPageWidth,
		AutoFitPageCount,
		ScaleFactor,
		PrintHeadersOnEveryPage,
		FitToPageWidth,
		SizeMode,
		SizeModeNone,
		SizeModeStretch,
		SizeModeZoom,
		AutoArrangeContent,
		ImageFormat,
		ExcelFormat,
		Resolution,
		ShowTitle,
		Title,
		CsvValueSeparator,
		FileName,
		FilterStatePresentation,
		FilterStatePresentationNone,
		FilterStatePresentationAfter,
		FilterStatePresentationAfterAndSplitPage,
		MessageIncorrectMapLatitude,
		IncorrectParameterType,
		SparklineTooltipStartValue,
		SparklineTooltipEndValue,
		SparklineTooltipMinValue,
		SparklineTooltipMaxValue,
		RangeFilterOLAPDataSource,
		InitialExtent,
		FilterElementShowAllItem,
		DefaultSqlDataSourceName,
		DefaultEFDataSourceName,
		DefaultOlapDataSourceName,
		DefaultObjectDataSourceName,
		DefaultExcelDataSourceName,
		DefaultFileExtractDataSourceName,
		DefaultDataSourceName,
		DashboardDataUpdating
	}
	public class DashboardLocalizer : XtraLocalizer<DashboardStringId> {
		static DashboardLocalizer() {
			if (GetActiveLocalizerProvider() == null)
#if !DXPORTABLE
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DashboardStringId>(new DashboardsResLocalizer()));
#else
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DashboardStringId>(new DashboardLocalizer()));
#endif
		}
		public static new XtraLocalizer<DashboardStringId> Active {
			get { return XtraLocalizer<DashboardStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<DashboardStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DashboardStringId>(value));
					RaiseActiveChanged();
				} 
				else
					XtraLocalizer<DashboardStringId>.Active = value; 
			}
		}
		public static string GetString(DashboardStringId id) {
			return Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			AddString(DashboardStringId.DefaultDashboardTitleText, "Dashboard");
			AddString(DashboardStringId.DefaultNameChartPane, "Pane");
			AddString(DashboardStringId.DefaultNamePivotItem, "Pivot");
			AddString(DashboardStringId.DefaultNameGridItem, "Grid");
			AddString(DashboardStringId.DefaultNameChartItem, "Chart");
			AddString(DashboardStringId.DefaultNameScatterChartItem, "Scatter Chart");
			AddString(DashboardStringId.DefaultNamePieItem, "Pies");
			AddString(DashboardStringId.DefaultNameGaugeItem, "Gauges");
			AddString(DashboardStringId.DefaultNameCardItem, "Cards");
			AddString(DashboardStringId.DefaultNameImageItem, "Image");
			AddString(DashboardStringId.DefaultNameTextBoxItem, "Text Box");
			AddString(DashboardStringId.DefaultNameRangeFilterItem, "Range Filter");
			AddString(DashboardStringId.DefaultNameMapItem, "Map");
			AddString(DashboardStringId.DefaultNameComboBoxItem, "Combo Box");
			AddString(DashboardStringId.DefaultNameListBoxItem, "List Box");
			AddString(DashboardStringId.DefaultNameTreeViewItem, "Tree View");
			AddString(DashboardStringId.DefaultNameItemGroup, "Group");
			AddString(DashboardStringId.SummaryTypeCount, "Count");
			AddString(DashboardStringId.SummaryTypeCountDistinct, "Count Distinct");
			AddString(DashboardStringId.SummaryTypeSum, "Sum");
			AddString(DashboardStringId.SummaryTypeMin, "Min");
			AddString(DashboardStringId.SummaryTypeMax, "Max");
			AddString(DashboardStringId.SummaryTypeAverage, "Average");
			AddString(DashboardStringId.SummaryTypeStdDev, "StdDev");
			AddString(DashboardStringId.SummaryTypeStdDevp, "StdDevP");
			AddString(DashboardStringId.SummaryTypeVar, "Var");
			AddString(DashboardStringId.SummaryTypeVarp, "VarP");
			AddString(DashboardStringId.GridTotalAutoTemplate, "{0}");
			AddString(DashboardStringId.GridTotalValueTemplate, "{0}");
			AddString(DashboardStringId.GridTotalTemplate, "{0} = {1}");
			AddString(DashboardStringId.GridTotalTypeCount, "Count");
			AddString(DashboardStringId.GridTotalTypeMin, "Min");
			AddString(DashboardStringId.GridTotalTypeMax, "Max");
			AddString(DashboardStringId.GridTotalTypeAvg, "Avg");
			AddString(DashboardStringId.GridTotalTypeSum, "Sum");
			AddString(DashboardStringId.GroupIntervalNone, "No Grouping");
			AddString(DashboardStringId.TextGroupIntervalAlphabetical, "Alphabetical");
			AddString(DashboardStringId.NumericGroupIntervalDiscrete, "Discrete");
			AddString(DashboardStringId.NumericGroupIntervalContinuous, "Continuous");
			AddString(DashboardStringId.DateTimeGroupIntervalYear, "Year");
			AddString(DashboardStringId.DateTimeGroupIntervalQuarter, "Quarter");
			AddString(DashboardStringId.DateTimeGroupIntervalMonth, "Month");
			AddString(DashboardStringId.DateTimeGroupIntervalDay, "Day");
			AddString(DashboardStringId.DateTimeGroupIntervalHour, "Hour");
			AddString(DashboardStringId.DateTimeGroupIntervalMinute, "Minute");
			AddString(DashboardStringId.DateTimeGroupIntervalSecond, "Second");
			AddString(DashboardStringId.DateTimeGroupIntervalDayOfYear, "Day of Year");
			AddString(DashboardStringId.DateTimeGroupIntervalDayOfWeek, "Day of Week");
			AddString(DashboardStringId.DateTimeGroupIntervalWeekOfYear, "Week of Year");
			AddString(DashboardStringId.DateTimeGroupIntervalWeekOfMonth, "Week of Month");
			AddString(DashboardStringId.DateTimeGroupIntervalMonthYear, "Month-Year");
			AddString(DashboardStringId.DateTimeGroupIntervalQuarterYear, "Quarter-Year");
			AddString(DashboardStringId.DateTimeGroupIntervalDayMonthYear, "Day-Month-Year");
			AddString(DashboardStringId.DateTimeGroupIntervalDateHour, "Date-Hour");
			AddString(DashboardStringId.DateTimeGroupIntervalDateHourMinute, "Date-Hour-Minute");
			AddString(DashboardStringId.DateTimeGroupIntervalDateHourMinuteSecond, "Date-Hour-Minute-Second");
			AddString(DashboardStringId.DateTimeGroupIntervalExactDate, "Exact Date");
			AddString(DashboardStringId.DateTimeQuarterFormat, "Q{0}");
			AddString(DashboardStringId.DateTimeQuarterFormatSpecifier, "QQ");
			AddString(DashboardStringId.ColoringModeDefault, "Default");
			AddString(DashboardStringId.ColoringModeNone, "None");
			AddString(DashboardStringId.ColoringModeHue, "Hue");
			AddString(DashboardStringId.AxisYNameCount, "Count");
			AddString(DashboardStringId.AxisYNameValues, "Values");
			AddString(DashboardStringId.AxisXNameArguments, "Arguments");
			AddString(DashboardStringId.ValueCaption, "Value");
			AddString(DashboardStringId.Value1Caption, "Value 1");
			AddString(DashboardStringId.Value2Caption, "Value 2");
			AddString(DashboardStringId.WeightCaption, "Weight");
			AddString(DashboardStringId.OpenCaption, "Open");
			AddString(DashboardStringId.HighCaption, "High");
			AddString(DashboardStringId.LowCaption, "Low");
			AddString(DashboardStringId.CloseCaption, "Close");
			AddString(DashboardStringId.ActualValueCaption, "Actual");
			AddString(DashboardStringId.TargetValueCaption, "Target");
			AddString(DashboardStringId.SeriesTypeBar, "Bar");
			AddString(DashboardStringId.SeriesTypeStackedBar, "Stacked Bar");
			AddString(DashboardStringId.SeriesTypeFullStackedBar, "Full-Stacked Bar");
			AddString(DashboardStringId.SeriesTypeSideBySideStackedBar, "Stacked Bar Side-by-Side");
			AddString(DashboardStringId.SeriesTypeSideBySideFullStackedBar, "Full-Stacked Bar Side-by-Side");
			AddString(DashboardStringId.SeriesTypePoint, "Point");
			AddString(DashboardStringId.SeriesTypeLine, "Line");
			AddString(DashboardStringId.SeriesTypeStackedLine, "Stacked Line");
			AddString(DashboardStringId.SeriesTypeFullStackedLine, "Full-Stacked Line");
			AddString(DashboardStringId.SeriesTypeStepLine, "Step Line");
			AddString(DashboardStringId.SeriesTypeSpline, "Spline");
			AddString(DashboardStringId.SeriesTypeArea, "Area");
			AddString(DashboardStringId.SeriesTypeStackedArea, "Stacked Area");
			AddString(DashboardStringId.SeriesTypeFullStackedArea, "Full-Stacked Area");
			AddString(DashboardStringId.SeriesTypeStepArea, "Step Area");
			AddString(DashboardStringId.SeriesTypeSplineArea, "Spline Area");
			AddString(DashboardStringId.SeriesTypeStackedSplineArea, "Stacked Spline Area");
			AddString(DashboardStringId.SeriesTypeFullStackedSplineArea, "Full-Stacked Spline Area");
			AddString(DashboardStringId.SeriesTypeSideBySideRangeBar, "Range Bar Side-by-Side");
			AddString(DashboardStringId.SeriesTypeRangeArea, "Range Area");
			AddString(DashboardStringId.SeriesTypeBubble, "Bubble");
			AddString(DashboardStringId.SeriesTypeStock, "Stock");
			AddString(DashboardStringId.SeriesTypeCandleStick, "Candle Stick");
			AddString(DashboardStringId.SeriesTypeHighLowClose, "High-Low-Close");
			AddString(DashboardStringId.SeriesTypeOpenHighLowClose, "Open-High-Low-Close");
			AddString(DashboardStringId.SeriesTypeGroupBar, "Bar");
			AddString(DashboardStringId.SeriesTypeGroupPointLine, "Point / Line");
			AddString(DashboardStringId.SeriesTypeGroupArea, "Area");
			AddString(DashboardStringId.SeriesTypeGroupRange, "Range");
			AddString(DashboardStringId.SeriesTypeGroupBubble, "Bubble");
			AddString(DashboardStringId.SeriesTypeGroupFinancial, "Financial");
			AddString(DashboardStringId.SeriesTypeGroupRangeFilter, "Range Filter");
			AddString(DashboardStringId.DescriptionValues, "Values");
			AddString(DashboardStringId.DescriptionMaps, "Maps");
			AddString(DashboardStringId.DescriptionMapValue, "Map / Value");
			AddString(DashboardStringId.DescriptionMapValues, "Map / Values");
			AddString(DashboardStringId.DescriptionArgument, "Argument");
			AddString(DashboardStringId.DescriptionArguments, "Arguments");
			AddString(DashboardStringId.DescriptionSeries, "Series");
			AddString(DashboardStringId.DescriptionSparkline, "Sparkline");
			AddString(DashboardStringId.DescriptionSparklineArgument, "Argument");
			AddString(DashboardStringId.DescriptionColumns, "Columns");
			AddString(DashboardStringId.DescriptionRows, "Rows");
			AddString(DashboardStringId.DescriptionDimensions, "Dimensions");
			AddString(DashboardStringId.DescriptionMeasures, "Measures");
			AddString(DashboardStringId.DescriptionGauges, "Gauges");
			AddString(DashboardStringId.DescriptionCards, "Cards");
			AddString(DashboardStringId.DescriptionItemValue, "Value");
			AddString(DashboardStringId.DescriptionItemArgument, "Argument");
			AddString(DashboardStringId.DescriptionItemSeries, "Series");
			AddString(DashboardStringId.DescriptionItemColumn, "Column");
			AddString(DashboardStringId.DescriptionItemRow, "Row");
			AddString(DashboardStringId.DescriptionItemDimension, "Dimension");
			AddString(DashboardStringId.DescriptionItemMeasure, "Measure");
			AddString(DashboardStringId.DescriptionItemAuto, "New Column");
			AddString(DashboardStringId.DataEngineGrandTotal, "Grand Total");
			AddString(DashboardStringId.DescriptionItemColor, "Color");
			AddString(DashboardStringId.DescriptionItemWeight, "Weight");
			AddString(DashboardStringId.DescriptionItemAxisXMeasure, "X-axis");
			AddString(DashboardStringId.DescriptionItemAxisYMeasure, "Y-axis");
			AddString(DashboardStringId.DescriptionItemMeasureWeight, "Weight");
			AddString(DashboardStringId.DescriptionDashboardItemName, "Dashboard item name");
			AddString(DashboardStringId.DescriptionDashboardItemComponentName, "Dashboard item component name");
			AddString(DashboardStringId.DescriptionTooltipDimensions, "Tooltip dimensions");
			AddString(DashboardStringId.DescriptionTooltipMeasures, "Tooltip measures");
			AddString(DashboardStringId.RangeFilterEmptySeries, "Please specify Range Filter values");
			AddString(DashboardStringId.RangeFilterIncompatibleSeriesType, "A Range Filter can contain Line and Area series only");
			AddString(DashboardStringId.RangeFilterEmptyArgument, "Please specify the Range Filter's argument");
			AddString(DashboardStringId.RangeFilterIncompatibleArgument, "Range Filter's argument field should have Numeric, DateTime, or TimeSpan type");
			AddString(DashboardStringId.RangeFilterIncompatibleNumericScale, "Range Filter's numeric argument values should produce a continuous axis. Set the numeric scale to Continuous");
			AddString(DashboardStringId.RangeFilterIncompatibleDateTimeGroupInterval, "Date-time argument values should identify actual calendar dates or time spans rather than grouping (i.e., they should contain a year part). Change the group interval accordingly");
			AddString(DashboardStringId.RangeFilterIncompatibleArgumentSorting, "Range Filter's argument should not be sorted in the descending order or by another data item");
			AddString(DashboardStringId.RangeFilterIncompatibleTopN, "Range Filter's argument does not support the Top N feature");
			AddString(DashboardStringId.MessageLoading, "Loading...");
			AddString(DashboardStringId.MessageInvalidStream, "The 'stream' parameter can't be null");
			AddString(DashboardStringId.MessageInternalError, "Internal error. Please contact the application vendor or your system administrator and provide the following information.\r\n\r\n{0}");
			AddString(DashboardStringId.MessageDashboardNotFoundError, "Dashboard has not been found.");
			AddString(DashboardStringId.MessageDashboardNotRelevantError, "Dashboard object model was changed. Refresh operation is required.");
			AddString(DashboardStringId.MessageDashboardInternalError, "Dashboard internal error. Please contact the application vendor or your system administrator.");
			AddString(DashboardStringId.MessageInvalidFilterExpression, "Incorrect column identifier: {0}. A valid column identifier must consist of the column name preceeded by the table name (or alias, if specified). These names must be dot-separated.");
			AddString(DashboardStringId.MessageIncorrectDataAssign, "Data cannot be assigned if a data provider is used");
			AddString(DashboardStringId.MessageDuplicatedDataItem, "This instance of the '{0}' data item has already been added to '{1}'. To add a similar data item to '{1}', create a new instance of this data item.");
			AddString(DashboardStringId.MessageIncorrectValueTopNCount, "The TopNCount property value cannot be negative or zero.");
			AddString(DashboardStringId.MessageIncorrectNumericPrecision, "The numeric value precision cannot be negative.");
			AddString(DashboardStringId.MessageInvalidCultureName, "Cannot find a specific culture with the following name: {0}");
			AddString(DashboardStringId.MessageIncorrectLayoutItemWeight, "Layout item weight cannot be negative or equal to zero");
			AddString(DashboardStringId.MessageDuplicatedLayoutItem, "This layout item is already contained in the tree");
			AddString(DashboardStringId.MessageLayoutDuplicatedDashboardItem, "A layout item for the specified dashboard item has already been added to the layout tree");
			AddString(DashboardStringId.MessageIncorrectMoveTarget, "Cannot move a layout item to a position within its own branch");
			AddString(DashboardStringId.MessageMasterFilterIncorrectNumberOfValues, "Cannot identify an element for which to set the Master Filter due to an incorrect number of provided values.");
			AddString(DashboardStringId.MessageIncorrectFilterValueLength, "The list of values that has been assigned as the filter value has a length that does not match the expected length of the filter value.");
			AddString(DashboardStringId.MessageGridHasNoData, "The grid has no data.");
			AddString(DashboardStringId.MessagePivotHasNoData, "The pivot has no data.");
			AddString(DashboardStringId.MessageIncorrectGridDashboardControlOperation, "An invalid operation is performed in GridDashboardControl.");
			AddString(DashboardStringId.MessageUnassignedDashboardItemGroup, "The dashboard item group should belong to the dashboard. To add a group to the dashboard, use the Dashboard.Groups property.");
			AddString(DashboardStringId.MessageIncorrectDashboardItemGroupAssign, "A dashboard item group cannot be added to a collection of dashboard items. Use the Dashboard.Groups property to add the group to the dashboard.");
			AddString(DashboardStringId.MessageWrongAxisName, "A data axis with the specified name does not exist in this dashboard item. See the DevExpress.DashboardCommon.DashboardDataAxisNames static class for valid data axis names.");
			AddString(DashboardStringId.MessageInvalidLayoutClientWidth, "The layout width must be greater than zero.");
			AddString(DashboardStringId.MessageInvalidLayoutClientHeight, "The layout height must be greater than zero.");
			AddString(DashboardStringId.MessageSummaryCalculatedFieldInDimension, "Cannot change the expression of the '{0}' calculated field that provides data for the '{1}' dimension. Summary-based calculated fields can provide data for measures only.");
			AddString(DashboardStringId.FormatStringDashboardItemCaption, " - {0}");
			AddString(DashboardStringId.FormatStringKpiElementCaption, "{0} vs {1}");
			AddString(DashboardStringId.FormatStringSeriesName, " - {0}");
			AddString(DashboardStringId.FormatStringDataItemName, "{0} ({1})");
			AddString(DashboardStringId.FormatRangeFilterDisplayText, "{0} - {1}");
			AddString(DashboardStringId.FormatMasterFilterDisplayText, ", {0}");
			AddString(DashboardStringId.DataItemsHeader, "DATA ITEMS");
			AddString(DashboardStringId.HiddenDataItemsHeader, "HIDDEN DATA ITEMS");
			AddString(DashboardStringId.TooltipDataItemsHeader, "TOOLTIP DATA ITEMS");
			AddString(DashboardStringId.NumericFormatUnitSymbolThousands, "K");
			AddString(DashboardStringId.NumericFormatUnitSymbolMillions, "M");
			AddString(DashboardStringId.NumericFormatUnitSymbolBillions, "B");
			AddString(DashboardStringId.TopNOthersValue, "Others");
			AddString(DashboardStringId.DashboardNullValue, " ");
			AddString(DashboardStringId.ActionClearMasterFilter, "Clear Master Filter");
			AddString(DashboardStringId.ActionClearSelection, "Clear Selection");
			AddString(DashboardStringId.ActionDrillUp, "Drill Up");
			AddString(DashboardStringId.ActionOtherValues, "Values");
			AddString(DashboardStringId.ActionExportTemplate, "{0} - {1}");
			AddString(DashboardStringId.ActionExportTo, "Export To");
			AddString(DashboardStringId.ActionExportToExcel, "Export To Excel");
			AddString(DashboardStringId.ActionExportToPdf, "Export To PDF");
			AddString(DashboardStringId.ActionExportToImage, "Export To Image");
			AddString(DashboardStringId.ActionAllowMultiselection, "Multi-Select");
			AddString(DashboardStringId.ElementNamePies, "Pies");
			AddString(DashboardStringId.ElementNameCards, "Cards");
			AddString(DashboardStringId.ElementNameGauges, "Gauges");
			AddString(DashboardStringId.EmptyColumnAliasPattern, "Column{0}");
			AddString(DashboardStringId.PivotGridTotal, "{0} Total");
			AddString(DashboardStringId.PivotGridGrandTotal, "Grand Total");
			AddString(DashboardStringId.ChartTotalValue, "Total");
			AddString(DashboardStringId.LoadingDataError, "The Dashboard encountered an issue while loading data. One or several dashboard items may appear empty.\r\nPlease contact the application vendor or your system administrator and provide the following information.\r\n\r\nThe Web application server failed to establish the following data connections:\r\n{0}");
			AddString(DashboardStringId.CalculatedFieldsTreeItemName, "Calculated Fields");
			AddString(DashboardStringId.CalculatedFieldTypeObject, "Object");
			AddString(DashboardStringId.CalculatedFieldTypeString, "String");
			AddString(DashboardStringId.CalculatedFieldTypeDateTime, "DateTime");
			AddString(DashboardStringId.CalculatedFieldTypeInteger, "Integer");
			AddString(DashboardStringId.CalculatedFieldTypeDecimal, "Decimal");
			AddString(DashboardStringId.CalculatedFieldTypeBoolean, "Boolean");
			AddString(DashboardStringId.MapAttributeDimension, "Attribute");
			AddString(DashboardStringId.Latitude, "Latitude");
			AddString(DashboardStringId.Longitude, "Longitude");
			AddString(DashboardStringId.Delta, "Delta");
			AddString(DashboardStringId.NewCalculatedFieldNamePrefix, "Calculated Field");
			AddString(DashboardStringId.NewParameterNamePrefix, "Parameter");
			AddString(DashboardStringId.ParametersFormCaption, "Dashboard Parameters");
			AddString(DashboardStringId.ParametersSelectorText, "Select...");
			AddString(DashboardStringId.ButtonOK, "OK");
			AddString(DashboardStringId.ButtonCancel, "Cancel");
			AddString(DashboardStringId.ButtonReset, "Reset");
			AddString(DashboardStringId.ButtonSubmit, "Submit");
			AddString(DashboardStringId.ButtonExport, "Export");
			AddString(DashboardStringId.GridResetColumnWidths, "Reset column widths");
			AddString(DashboardStringId.GridSortAscending, "Sort Ascending");
			AddString(DashboardStringId.GridSortDescending, "Sort Descending");
			AddString(DashboardStringId.GridClearSorting, "Clear Sorting");
			AddString(DashboardStringId.PageLayout, "Page Layout");
			AddString(DashboardStringId.PageLayoutAuto, "Auto");
			AddString(DashboardStringId.PageLayoutPortrait, "Portrait");
			AddString(DashboardStringId.PageLayoutLandscape, "Landscape");
			AddString(DashboardStringId.PaperKind, "Size");
			AddString(DashboardStringId.PaperKindLetter, "Letter");
			AddString(DashboardStringId.PaperKindLegal, "Legal");
			AddString(DashboardStringId.PaperKindExecutive, "Executive");
			AddString(DashboardStringId.PaperKindA5, "A5");
			AddString(DashboardStringId.PaperKindA4, "A4");
			AddString(DashboardStringId.PaperKindA3, "A3");
			AddString(DashboardStringId.ScaleMode, "Scale Mode");
			AddString(DashboardStringId.ScaleModeNone, "None");
			AddString(DashboardStringId.ScaleModeUseScaleFactor, "Use Scale Factor");
			AddString(DashboardStringId.ScaleModeAutoFitToPageWidth, "Auto Fit to Page Width");
			AddString(DashboardStringId.AutoFitPageCount, "Auto Fit Page Count");
			AddString(DashboardStringId.ScaleFactor, "Scale Factor");
			AddString(DashboardStringId.PrintHeadersOnEveryPage, "Print Headers on Every Page");
			AddString(DashboardStringId.FitToPageWidth, "Fit to Page Width");
			AddString(DashboardStringId.SizeMode, "Size Mode");
			AddString(DashboardStringId.SizeModeNone, "None");
			AddString(DashboardStringId.SizeModeStretch, "Stretch");
			AddString(DashboardStringId.SizeModeZoom, "Zoom");
			AddString(DashboardStringId.AutoArrangeContent, "Auto Arrange Content");
			AddString(DashboardStringId.ImageFormat, "Image Format");
			AddString(DashboardStringId.ExcelFormat, "Excel Format");
			AddString(DashboardStringId.Resolution, "Resolution (dpi)");
			AddString(DashboardStringId.ShowTitle, "Show Title");
			AddString(DashboardStringId.Title, "Title");
			AddString(DashboardStringId.CsvValueSeparator, "Separator");
			AddString(DashboardStringId.FileName, "File Name");
			AddString(DashboardStringId.FilterStatePresentation, "Filter State");
			AddString(DashboardStringId.FilterStatePresentationNone, "None");
			AddString(DashboardStringId.FilterStatePresentationAfter, "Below");
			AddString(DashboardStringId.FilterStatePresentationAfterAndSplitPage, "Separate Page");
			AddString(DashboardStringId.MessageIncorrectMapLatitude, "The latitude should be greater than or equal to {0} and less than or equal to {1}.");
			AddString(DashboardStringId.IncorrectParameterType, "The dashboard parameter values type does not correspond to Parameter.Type.");
			AddString(DashboardStringId.SparklineTooltipStartValue, "Start:");
			AddString(DashboardStringId.SparklineTooltipEndValue, "End:");
			AddString(DashboardStringId.SparklineTooltipMinValue, "Min:");
			AddString(DashboardStringId.SparklineTooltipMaxValue, "Max:");
			AddString(DashboardStringId.RangeFilterOLAPDataSource, "The Range Filter is not supported in OLAP mode.");
			AddString(DashboardStringId.InitialExtent, "Initial Extent");
			AddString(DashboardStringId.FilterElementShowAllItem, "(All)");
			AddString(DashboardStringId.MeasureNamesDimensionName, "MeasureNames");
			AddString(DashboardStringId.ColorAutoAssignedNotApplied, "Auto (not applied)");
			AddString(DashboardStringId.ColorAutoAssigned, "Auto");
			AddString(DashboardStringId.ColorPaletteIndex, "Palette index:");
			AddString(DashboardStringId.ColorInherited, "(Inherited)");
			AddString(DashboardStringId.DashboardErrorValue, "Error");
			AddString(DashboardStringId.DefaultSqlDataSourceName, "SQL Data Source");
			AddString(DashboardStringId.DefaultEFDataSourceName, "EF Data Source");
			AddString(DashboardStringId.DefaultOlapDataSourceName, "OLAP Data Source");
			AddString(DashboardStringId.DefaultObjectDataSourceName, "Object Data Source");
			AddString(DashboardStringId.DefaultExcelDataSourceName, "Excel Data Source");
			AddString(DashboardStringId.DefaultFileExtractDataSourceName, "Extract File Data Source");
			AddString(DashboardStringId.DefaultDataSourceName, "Data Source");
			AddString(DashboardStringId.DashboardDataUpdating, "Updating");
		}
		public override XtraLocalizer<DashboardStringId> CreateResXLocalizer() {
#if !DXPORTABLE
			return new DashboardsResLocalizer();
#else
			return new DashboardLocalizer();
#endif
		}
	}
#if !DXPORTABLE
	public class DashboardsResLocalizer : DashboardLocalizer {
		ResourceManager manager;
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public DashboardsResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if (manager != null) 
				manager.ReleaseAllResources();
			manager = new ResourceManager("DevExpress.DashboardCommon.LocalizationRes", GetType().Assembly);
		}
		public override string GetLocalizedString(DashboardStringId id) {
			return Manager.GetString("DashboardStringId." + id) ?? String.Empty;
		}
	}
#endif
}
