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
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid {
#if SL
	public enum MouseButton {
		Left,
		Right
	}
#endif
	public enum FieldColumnTotalsLocation {
		Near = 0,
		Far = 1,
	}
	public enum FieldRowTotalsLocation {
		Near = 0,
		Far = 1,
		Tree = 2,
	}
	public enum PivotDefaultMemberFields {
		NonVisibleFilterFields = 0,
		AllFilterFields = 1,
	}
	public enum FieldListOrientation { Vertical, Horizontal, Auto }
	public enum FieldListArea {
		RowArea = 0,
		ColumnArea = 1,
		FilterArea = 2,
		DataArea = 3,
		All = 4
	}
	public enum AllowHideFieldsType { Never, Always, WhenFieldListVisible };
	public enum FieldArea {
		RowArea = 0,
		ColumnArea = 1,
		FilterArea = 2,
		DataArea = 3
	}
	public enum DataFieldArea { 
		None = 0, 
		ColumnArea = 1, 
		RowArea = 2
	}
	public enum FieldSummaryType {
		Count = 0,
		Sum = 1,
		Min = 2,
		Max = 3,
		Average = 4,
		StdDev = 5,
		StdDevp = 6,
		Var = 7,
		Varp = 8,
		Custom = 9,
	}
	public enum FieldSortOrder {
		Ascending = 0,
		Descending = 1,
	}
	public enum FieldSortBySummaryOrder {
		Ascending = 0,
		Descending = 1,
		Default = 2,
	}
	public enum FieldSortMode {
		Default = 0,
		Value = 1,
		DisplayText = 2,
		Custom = 3,
		Key = 4,
		ID = 5,
		None = 6,
		DimensionAttribute = 7,
	}
	public enum FieldSummaryDisplayType {
		Default = 0,
		AbsoluteVariation = 1,
		PercentVariation = 2,
		PercentOfColumn = 3,
		PercentOfRow = 4,
		PercentOfColumnGrandTotal = 5,
		PercentOfRowGrandTotal = 6,
		PercentOfGrandTotal = 7,
		RankInColumnSmallestToLargest = 8,
		RankInRowSmallestToLargest = 9,
		RankInColumnLargestToSmallest = 10,
		RankInRowLargestToSmallest = 11,
		Index = 12
	}
	public enum FieldGroupInterval {
		Default = 0,
		Date = 1,
		DateDay = 2,
		DateDayOfWeek = 3,
		DateDayOfYear = 4,
		DateWeekOfMonth = 5,
		DateWeekOfYear = 6,
		DateMonth = 7,
		DateQuarter = 8,
		DateYear = 9,
		YearAge = 10,
		MonthAge = 11,
		WeekAge = 12,
		DayAge = 13,
		Alphabetical = 14,
		Numeric = 15,
		Hour = 16,
		Custom = 17,
		Minute = 18,
		Second = 19,
		DateMonthYear = 20,
		DateQuarterYear = 21,
		DateHour = 22,
		DateHourMinute = 23,
		DateHourMinuteSecond = 24
	}
	[Flags]
	public enum FieldAllowedAreas {
		RowArea = 1,
		ColumnArea = 2,
		FilterArea = 4,
		DataArea = 8,
		All = 15,
	}
	public enum FieldTotalsVisibility {
		AutomaticTotals = 0,
		CustomTotals = 1,
		None = 2,
	}
	public enum FieldTopValueType {
		Absolute = 0,
		Percent = 1,
		Sum = 2,
	}
	public enum FieldTopValueMode {
		Default = 0,
		AllValues = 1,
		ParentFieldValues = 2
	}
	public enum FieldUnboundColumnType {
		Bound = 0,
		Integer = 1,
		Decimal = 2,
		DateTime = 3,
		String = 4,
		Boolean = 5,
		Object = 6,
	}
	public enum FieldValueType {
		Value = 0,
		Total = 1,
		GrandTotal = 2,
		CustomTotal = 3 
	}
	public enum FieldPropertyName {
		SortOrder = 0,
		FieldName = 1
	}
	public enum FieldOLAPFilterUsingWhereClause {
		SingleValuesOnly,
		Always,
		Never,
		Auto
	}
	public enum SummaryDataSourceFieldNaming { 
		FieldName = 0, 
		Name = 1
	}
	public enum FieldFilterType {
		Excluded = 0,
		Included = 1,
	}
	public enum GroupFilterMode {
		List = 0, 
		Tree = 1
	}
	[Flags]
	public enum FieldBestFitArea { None = 0, FieldValue = 1, FieldHeader = 2, Cell = 4, All = FieldValue | FieldHeader | Cell };
	public enum SelectMode { None, SolidSelection, MultiSelection };
	public enum UnboundExpressionMode {
		Default = 0,
		UseSummaryValues = 1,
	}
	public enum FieldUnboundExpressionMode {
		Default = 0,
		UseSummaryValues = 1,
		DataSource = 2,
		UseAggregateFunctions = 3
	}
	public enum PivotChartExportFieldValueMode {
		Default = 0,
		DisplayText = 1,
		Value = 2
	};
	public enum PivotChartFieldValuesProvideMode {
		Default = 0,
		DisplayText = 1,
		Value = 2
	};
	public enum PivotChartDataProvideMode {
		ProvideLastLevelData = CoreXtraPivotGrid.PivotChartDataProvideMode.ProvideLastLevelData,
		UseCustomSettings = CoreXtraPivotGrid.PivotChartDataProvideMode.UseCustomSettings
	};
	public enum PivotChartDataProvidePriority {
		Columns = CoreXtraPivotGrid.PivotChartDataProvidePriority.Columns,
		Rows = CoreXtraPivotGrid.PivotChartDataProvidePriority.Rows
	};
	public enum PivotChartItemType {
		RowItem = 0,
		ColumnItem = 1,
		CellItem = 2
	};
	public enum PivotChartItemDataMember {
		Argument = 0,
		Series = 1,
		Value = 2
	};
	public enum PivotKpiType { 
		None, 
		Value, 
		Goal, 
		Status, 
		Trend, 
		Weight 
	}
	public enum PivotKpiGraphic {
		None,
		ServerDefined,
		Shapes,
		TrafficLights,
		RoadSigns,
		Gauge,
		ReversedGauge,
		Thermometer,
		ReversedThermometer,
		Cylinder,
		ReversedCylinder,
		Faces,
		VarianceArrow,
		StandardArrow,
		StatusArrow,
		ReversedStatusArrow
	};
	public enum CopyCollapsedValuesMode {
		DuplicateCollapsedValues,
		PreserveCollapsedLevels,
		RemoveCollapsedLevels
	};
	public enum CopyMultiSelectionMode { 
		IncludeIntermediateColumnsAndRows, 
		DiscardIntermediateColumnsAndRows 
	};
#if !SL
	public enum OlapDataProvider {
		Default,
		Adomd,
		OleDb,
		Xmla
	}
#endif
	public enum UserAction { 
		None = 0,
		Prefilter = 1, 
		FieldFilter= 2,
		FieldResize = 3,
		FieldDrag = 4, 
		FieldUnboundExpression = 5,
		MenuOpen = 6,
	}
	public enum ScrollingMode {
		Line,
		Pixel,
	}
	public static class PivotEnumExtensions {
#if !SL
		public static OLAPDataProvider ToOLAPDataProvider(this OlapDataProvider pr) {
			switch(pr) {
				case OlapDataProvider.Adomd:
					return OLAPDataProvider.Adomd;
				case OlapDataProvider.Default:
					return OLAPDataProvider.Default;
				case OlapDataProvider.OleDb:
					return OLAPDataProvider.OleDb;
				case OlapDataProvider.Xmla:
					return OLAPDataProvider.Xmla;
				default:
					throw new ArgumentException("OlapDataProvider");
			}
		}
#endif
		public static PivotKpiType ToKpiType(this CoreXtraPivotGrid.PivotKPIType gr) {
			switch(gr) {
				case CoreXtraPivotGrid.PivotKPIType.Goal:
					return PivotKpiType.Goal;
				case CoreXtraPivotGrid.PivotKPIType.None:
					return PivotKpiType.None;
				case CoreXtraPivotGrid.PivotKPIType.Status:
					return PivotKpiType.Status;
				case CoreXtraPivotGrid.PivotKPIType.Trend:
					return PivotKpiType.Trend;
				case CoreXtraPivotGrid.PivotKPIType.Value:
					return PivotKpiType.Value;
				case CoreXtraPivotGrid.PivotKPIType.Weight:
					return PivotKpiType.Weight;
				default:
					throw new ArgumentException("PivotKPIType");
			}
		}
		public static CoreXtraPivotGrid.PivotKPIType ToXtraType(this PivotKpiType gr) {
			switch(gr) {
				case PivotKpiType.Goal:
					return CoreXtraPivotGrid.PivotKPIType.Goal;
				case PivotKpiType.None:
					return CoreXtraPivotGrid.PivotKPIType.None;
				case PivotKpiType.Status:
					return CoreXtraPivotGrid.PivotKPIType.Status;
				case PivotKpiType.Trend:
					return CoreXtraPivotGrid.PivotKPIType.Trend;
				case PivotKpiType.Value:
					return CoreXtraPivotGrid.PivotKPIType.Value;
				case PivotKpiType.Weight:
					return CoreXtraPivotGrid.PivotKPIType.Weight;
				default:
					throw new ArgumentException("PivotKPIType");
			}
		}
		public static bool NeedGraphic(this PivotKpiType gr) {
			return gr == PivotKpiType.Trend || gr == PivotKpiType.Status;
		}
		public static PivotGridStringId GetStringId(this PivotKpiGraphic gr) {
			switch(gr) {
				case PivotKpiGraphic.Cylinder:
					return PivotGridStringId.PopupMenuKPIGraphicCylinder;
				case PivotKpiGraphic.Faces:
					return PivotGridStringId.PopupMenuKPIGraphicFaces;
				case PivotKpiGraphic.Gauge:
					return PivotGridStringId.PopupMenuKPIGraphicGauge;
				case PivotKpiGraphic.None:
					return PivotGridStringId.PopupMenuKPIGraphicNone;
				case PivotKpiGraphic.ReversedCylinder:
					return PivotGridStringId.PopupMenuKPIGraphicReversedCylinder;
				case PivotKpiGraphic.ReversedGauge:
					return PivotGridStringId.PopupMenuKPIGraphicReversedGauge;
				case PivotKpiGraphic.ReversedStatusArrow:
					return PivotGridStringId.PopupMenuKPIGraphicReversedStatusArrow;
				case PivotKpiGraphic.ReversedThermometer:
					return PivotGridStringId.PopupMenuKPIGraphicReversedThermometer;
				case PivotKpiGraphic.RoadSigns:
					return PivotGridStringId.PopupMenuKPIGraphicRoadSigns;
				case PivotKpiGraphic.ServerDefined:
					return PivotGridStringId.PopupMenuKPIGraphicServerDefined;
				case PivotKpiGraphic.Shapes:
					return PivotGridStringId.PopupMenuKPIGraphicShapes;
				case PivotKpiGraphic.StandardArrow:
					return PivotGridStringId.PopupMenuKPIGraphicStandardArrow;
				case PivotKpiGraphic.StatusArrow:
					return PivotGridStringId.PopupMenuKPIGraphicStatusArrow;
				case PivotKpiGraphic.Thermometer:
					return PivotGridStringId.PopupMenuKPIGraphicThermometer;
				case PivotKpiGraphic.TrafficLights:
					return PivotGridStringId.PopupMenuKPIGraphicTrafficLights;
				case PivotKpiGraphic.VarianceArrow:
					return PivotGridStringId.PopupMenuKPIGraphicVarianceArrow;
				default:
					throw new ArgumentException("KpiGraphic");
			}
		}
		public static PivotKpiGraphic ToKpiGraphic(this CoreXtraPivotGrid.PivotKPIGraphic gr) {
			switch(gr) {
				case CoreXtraPivotGrid.PivotKPIGraphic.Cylinder:
					return PivotKpiGraphic.Cylinder;
				case CoreXtraPivotGrid.PivotKPIGraphic.Faces:
					return PivotKpiGraphic.Faces;
				case CoreXtraPivotGrid.PivotKPIGraphic.Gauge:
					return PivotKpiGraphic.Gauge;
				case CoreXtraPivotGrid.PivotKPIGraphic.None:
					return PivotKpiGraphic.None;
				case CoreXtraPivotGrid.PivotKPIGraphic.ReversedCylinder:
					return PivotKpiGraphic.ReversedCylinder;
				case CoreXtraPivotGrid.PivotKPIGraphic.ReversedGauge:
					return PivotKpiGraphic.ReversedGauge;
				case CoreXtraPivotGrid.PivotKPIGraphic.ReversedStatusArrow:
					return PivotKpiGraphic.ReversedStatusArrow;
				case CoreXtraPivotGrid.PivotKPIGraphic.ReversedThermometer:
					return PivotKpiGraphic.ReversedThermometer;
				case CoreXtraPivotGrid.PivotKPIGraphic.RoadSigns:
					return PivotKpiGraphic.RoadSigns;
				case CoreXtraPivotGrid.PivotKPIGraphic.ServerDefined:
					return PivotKpiGraphic.ServerDefined;
				case CoreXtraPivotGrid.PivotKPIGraphic.Shapes:
					return PivotKpiGraphic.Shapes;
				case CoreXtraPivotGrid.PivotKPIGraphic.StandardArrow:
					return PivotKpiGraphic.StandardArrow;
				case CoreXtraPivotGrid.PivotKPIGraphic.StatusArrow:
					return PivotKpiGraphic.StatusArrow;
				case CoreXtraPivotGrid.PivotKPIGraphic.Thermometer:
					return PivotKpiGraphic.Thermometer;
				case CoreXtraPivotGrid.PivotKPIGraphic.TrafficLights:
					return PivotKpiGraphic.TrafficLights;
				case CoreXtraPivotGrid.PivotKPIGraphic.VarianceArrow:
					return PivotKpiGraphic.VarianceArrow;
				default:
					throw new ArgumentException("PivotKpiGraphic");
			}
		}
		public static CoreXtraPivotGrid.PivotArea ToPivotArea(this FieldListArea presenterType) {
			switch(presenterType) {
				case FieldListArea.FilterArea:
					return CoreXtraPivotGrid.PivotArea.FilterArea;
				case FieldListArea.ColumnArea:
					return CoreXtraPivotGrid.PivotArea.ColumnArea;
				case FieldListArea.RowArea:
					return CoreXtraPivotGrid.PivotArea.RowArea;
				case FieldListArea.DataArea:
					return CoreXtraPivotGrid.PivotArea.DataArea;
				default:
					throw new ArgumentException("presenterType");
			}
		}
		public static FieldArea ToFieldArea(this FieldListArea presenterType) {
			switch(presenterType) {
				case FieldListArea.FilterArea:
					return FieldArea.FilterArea;
				case FieldListArea.ColumnArea:
					return FieldArea.ColumnArea;
				case FieldListArea.RowArea:
					return FieldArea.RowArea;
				case FieldListArea.DataArea:
					return FieldArea.DataArea;
				default:
					throw new ArgumentException("presenterType");
			}
		}
		public static FieldListArea ToFieldListArea(this CoreXtraPivotGrid.PivotArea area) {
			switch(area) {
				case CoreXtraPivotGrid.PivotArea.ColumnArea:
					return FieldListArea.ColumnArea;
				case CoreXtraPivotGrid.PivotArea.DataArea:
					return FieldListArea.DataArea;
				case CoreXtraPivotGrid.PivotArea.FilterArea:
					return FieldListArea.FilterArea;
				case CoreXtraPivotGrid.PivotArea.RowArea:
					return FieldListArea.RowArea;
				default:
					throw new ArgumentException("area");
			}
		}
		public static FieldListArea ToFieldListArea(this FieldArea area) {
			switch(area) {
				case FieldArea.ColumnArea:
					return FieldListArea.ColumnArea;
				case FieldArea.DataArea:
					return FieldListArea.DataArea;
				case FieldArea.FilterArea:
					return FieldListArea.FilterArea;
				case FieldArea.RowArea:
					return FieldListArea.RowArea;
				default:
					throw new ArgumentException("area");
			}
		}
		public static bool IsColumnOrRow(this FieldArea area) {
			return area == FieldArea.ColumnArea || area == FieldArea.RowArea;
		}
		public static bool IsPivotArea(this FieldListArea presenterType) {
			return presenterType == FieldListArea.FilterArea ||
				presenterType == FieldListArea.ColumnArea ||
				presenterType == FieldListArea.RowArea ||
				presenterType == FieldListArea.DataArea;
		}
		public static CoreXtraPivotGrid.PivotArea ToPivotArea(this FieldArea value) {
			return (CoreXtraPivotGrid.PivotArea)value;
		}
		public static FieldArea ToFieldArea(this CoreXtraPivotGrid.PivotArea value) {
			return (FieldArea)value;
		}
		public static CoreXtraPivotGrid.PivotSortOrder ToPivotSortOrder(this FieldSortOrder value) {
			return (CoreXtraPivotGrid.PivotSortOrder)value;
		}
		public static FieldSortOrder ToFieldSortOrder(this CoreXtraPivotGrid.PivotSortOrder value) {
			return (FieldSortOrder)value;
		}
		public static CoreXtraPivotGrid.PivotSortMode ToPivotSortMode(this FieldSortMode value) {
			return (CoreXtraPivotGrid.PivotSortMode)value;
		}
		public static FieldSortMode ToFieldSortMode(this CoreXtraPivotGrid.PivotSortMode value) {
			return (FieldSortMode)value;
		}
		public static PivotSummaryType ToPivotSummaryType(this FieldSummaryType value) {
			return (PivotSummaryType)value;
		}
		public static PivotSummaryType? ToNullablePivotSummaryType(this FieldSummaryType? value) {
			return (PivotSummaryType?)value;
		}
		public static FieldSummaryType ToFieldSummaryType(this PivotSummaryType value) {
			return (FieldSummaryType)value;
		}
		public static FieldSummaryType? ToNullableFieldSummaryType(this PivotSummaryType? value) {
			return (FieldSummaryType?)value;
		}
		public static PivotSummaryDisplayType ToPivotSummaryDisplayType(this FieldSummaryDisplayType value) {
			return (PivotSummaryDisplayType)value;
		}
		public static FieldSummaryDisplayType ToFieldSummaryDisplayType(this PivotSummaryDisplayType value) {
			return (FieldSummaryDisplayType)value;
		}
		public static CoreXtraPivotGrid.PivotGroupInterval ToPivotGroupInterval(this FieldGroupInterval value) {
			return (CoreXtraPivotGrid.PivotGroupInterval)value;
		}
		public static FieldGroupInterval ToFieldGroupInterval(this CoreXtraPivotGrid.PivotGroupInterval value) {
			return (FieldGroupInterval)value;
		}
		public static CoreXtraPivotGrid.PivotGridAllowedAreas ToPivotAllowedAreas(this FieldAllowedAreas value) {
			return (CoreXtraPivotGrid.PivotGridAllowedAreas)value;
		}
		public static FieldAllowedAreas ToFieldAllowedAreas(this CoreXtraPivotGrid.PivotGridAllowedAreas value) {
			return (FieldAllowedAreas)value;
		}
		public static CoreXtraPivotGrid.PivotTotalsVisibility ToPivotTotalsVisibility(this FieldTotalsVisibility value) {
			return (CoreXtraPivotGrid.PivotTotalsVisibility)value;
		}
		public static FieldTotalsVisibility ToFieldTotalsVisibility(this CoreXtraPivotGrid.PivotTotalsVisibility value) {
			return (FieldTotalsVisibility)value;
		}
		public static CoreXtraPivotGrid.PivotTopValueType ToPivotTopValueType(this FieldTopValueType value) {
			return (CoreXtraPivotGrid.PivotTopValueType)value;
		}
		public static FieldTopValueType ToFieldTopValueType(this CoreXtraPivotGrid.PivotTopValueType value) {
			return (FieldTopValueType)value;
		}
		public static CoreXtraPivotGrid.TopValueMode ToPivotTopValueType(this FieldTopValueMode value) {
			switch(value) {
				case FieldTopValueMode.AllValues:
					return TopValueMode.AllValues;
				case FieldTopValueMode.Default:
					return TopValueMode.Default;
				case FieldTopValueMode.ParentFieldValues:
					return TopValueMode.ParentFieldValues;
				default:
					throw new ArgumentException(value.ToString());
			}
		}
		public static FieldTopValueMode ToFieldTopValueType(this CoreXtraPivotGrid.TopValueMode value) {
			switch(value) {
				case TopValueMode.ParentFieldValues:
					return FieldTopValueMode.ParentFieldValues;
				case TopValueMode.Default:
					return FieldTopValueMode.Default;
				case TopValueMode.AllValues:
					return FieldTopValueMode.AllValues;
				default:
					throw new ArgumentException(value.ToString());
			}
		}
		public static UnboundColumnType ToUnboundColumnType(this FieldUnboundColumnType value) {
			return (UnboundColumnType)value;
		}
		public static FieldUnboundColumnType ToFieldUnboundColumnType(this UnboundColumnType value) {
			return (FieldUnboundColumnType)value;
		}
		public static CoreXtraPivotGrid.PivotGridValueType ToPivotGridValueType(this FieldValueType value) {
			return (CoreXtraPivotGrid.PivotGridValueType)value;
		}
		public static FieldValueType ToFieldValueType(this CoreXtraPivotGrid.PivotGridValueType value) {
			return (FieldValueType)value;
		}
		public static CoreXtraPivotGrid.PivotFieldPropertyName ToPivotFieldPropertyName(this FieldPropertyName value) {
			return (CoreXtraPivotGrid.PivotFieldPropertyName)value;
		}
		public static FieldPropertyName ToFieldPropertyName(this CoreXtraPivotGrid.PivotFieldPropertyName value) {
			return (FieldPropertyName)value;
		}
		public static CoreXtraPivotGrid.PivotOLAPFilterUsingWhereClause ToPivotOLAPFilterUsingWhereClause(this FieldOLAPFilterUsingWhereClause value) {
			return (CoreXtraPivotGrid.PivotOLAPFilterUsingWhereClause)value;
		}
		public static FieldOLAPFilterUsingWhereClause ToFieldOLAPFilterUsingWhereClause(this CoreXtraPivotGrid.PivotOLAPFilterUsingWhereClause value) {
			return (FieldOLAPFilterUsingWhereClause)value;
		}
		public static CoreXtraPivotGrid.PivotTotalsLocation ToPivotTotalsLocationType(this FieldColumnTotalsLocation value) {
			return (CoreXtraPivotGrid.PivotTotalsLocation)value;
		}
		public static FieldColumnTotalsLocation ToFieldTotalsLocationType(this CoreXtraPivotGrid.PivotTotalsLocation value) {
			return (FieldColumnTotalsLocation)value;
		}
		public static CoreXtraPivotGrid.PivotRowTotalsLocation ToPivotTotalsLocationType(this FieldRowTotalsLocation value) {
			return (CoreXtraPivotGrid.PivotRowTotalsLocation)value;
		}
		public static FieldRowTotalsLocation ToFieldTotalsLocationType(this CoreXtraPivotGrid.PivotRowTotalsLocation value) {
			return (FieldRowTotalsLocation)value;
		}
		public static CoreXtraPivotGrid.PivotDataArea ToPivotDataAreaType(this DataFieldArea value) {
			return (CoreXtraPivotGrid.PivotDataArea)value;
		}
		public static DataFieldArea ToDataFieldAreaType(this CoreXtraPivotGrid.PivotDataArea value) {
			return (DataFieldArea)value;
		}
		public static CoreXtraPivotGrid.DataFieldNaming ToDataFieldNamingType(this SummaryDataSourceFieldNaming value) {
			return (CoreXtraPivotGrid.DataFieldNaming)value;
		}
		public static SummaryDataSourceFieldNaming ToSummaryDataSourceFieldNamingType(this CoreXtraPivotGrid.DataFieldNaming value) {
			return (SummaryDataSourceFieldNaming)value;
		}
		public static FieldFilterType ToFieldFilterType(this CoreXtraPivotGrid.PivotFilterType value) {
			return (FieldFilterType)value;
		}
		public static CoreXtraPivotGrid.PivotFilterType ToPivotFilterType(this FieldFilterType value) {
			return (CoreXtraPivotGrid.PivotFilterType)value;
		}
		public static AllowHideFieldsType ToAllowHideFieldsType(this XtraPivotGrid.AllowHideFieldsType value) {
			return (AllowHideFieldsType)value;
		}
		public static XtraPivotGrid.AllowHideFieldsType ToAllowHideFieldsType(this AllowHideFieldsType value) {
			return (XtraPivotGrid.AllowHideFieldsType)value;
		}
		public static DefaultBoolean ToDefaultBoolean(this bool b) {
			return b ? DefaultBoolean.True : DefaultBoolean.False;
		}
		public static DefaultBoolean ToDefaultBoolean(this bool? b) {
			if(!b.HasValue) return DefaultBoolean.Default;
			return b.Value == true ? DefaultBoolean.True : DefaultBoolean.False;
		}
		public static bool? ToNullableBoolean(this DefaultBoolean db) {
			if(db == DefaultBoolean.Default) return null;
			return db == DefaultBoolean.True;
		}
		public static CoreXtraPivotGrid.PivotChartFieldValuesProvideMode ToCorePivotChartFieldValuesProvideMode(this PivotChartFieldValuesProvideMode value) {
			return (CoreXtraPivotGrid.PivotChartFieldValuesProvideMode)value;
		}
		public static CoreXtraPivotGrid.PivotChartDataProvideMode ToCorePivotChartDataProvideMode(this PivotChartDataProvideMode value) {
			return (CoreXtraPivotGrid.PivotChartDataProvideMode)value;
		}
		public static CoreXtraPivotGrid.PivotChartDataProvidePriority ToCorePivotChartDataProvidePriority(this PivotChartDataProvidePriority value) {
			return (CoreXtraPivotGrid.PivotChartDataProvidePriority)value;
		}
		public static PivotChartFieldValuesProvideMode ToFieldValuesProvideMode(this PivotChartExportFieldValueMode value) {
			return (PivotChartFieldValuesProvideMode)value;
		}
		public static PivotChartExportFieldValueMode ToExportFieldValueMode(this PivotChartFieldValuesProvideMode value) {
			return (PivotChartExportFieldValueMode)value;
		}
		public static PivotChartItemType ToXpfPivotChartItemType(this CoreXtraPivotGrid.PivotChartItemType value) {
			return (PivotChartItemType)value;
		}
		public static CoreXtraPivotGrid.PivotChartItemType ToCorePivotChartItemType(this PivotChartItemType value) {
			return (CoreXtraPivotGrid.PivotChartItemType)value;
		}
		public static PivotChartItemDataMember ToXpfPivotChartItemDataMember(this CoreXtraPivotGrid.PivotChartItemDataMember value) {
			return (PivotChartItemDataMember)value;
		}
		public static UnboundExpressionMode ToUnboundExpressionMode(this CoreXtraPivotGrid.DataFieldUnboundExpressionMode value) {
			return (UnboundExpressionMode)value;
		}
		public static CoreXtraPivotGrid.DataFieldUnboundExpressionMode ToDataFieldUnboundExpressionMode(this UnboundExpressionMode value) {
			return (CoreXtraPivotGrid.DataFieldUnboundExpressionMode)value;
		}
		public static FieldUnboundExpressionMode ToUnboundExpressionMode(this CoreXtraPivotGrid.UnboundExpressionMode value) {
			switch(value) {
				case CoreXtraPivotGrid.UnboundExpressionMode.DataSource:
					return FieldUnboundExpressionMode.DataSource;
				case CoreXtraPivotGrid.UnboundExpressionMode.Default:
					return FieldUnboundExpressionMode.Default;
				case CoreXtraPivotGrid.UnboundExpressionMode.UseSummaryValues:
					return FieldUnboundExpressionMode.UseSummaryValues;
				case CoreXtraPivotGrid.UnboundExpressionMode.UseAggregateFunctions:
					return FieldUnboundExpressionMode.UseAggregateFunctions;
				default:
					throw new ArgumentException(value.ToString());
			}
		}
		public static CoreXtraPivotGrid.UnboundExpressionMode ToDataFieldUnboundExpressionMode(this FieldUnboundExpressionMode value) {
			switch(value) {
				case FieldUnboundExpressionMode.UseSummaryValues:
					return CoreXtraPivotGrid.UnboundExpressionMode.UseSummaryValues;
				case FieldUnboundExpressionMode.Default:
					return CoreXtraPivotGrid.UnboundExpressionMode.Default;
				case FieldUnboundExpressionMode.DataSource:
					return CoreXtraPivotGrid.UnboundExpressionMode.DataSource;
				default:
					throw new ArgumentException(value.ToString());
			}
		}
		public static XtraPivotGrid.PivotGroupFilterMode ToGroupFilterMode(this GroupFilterMode value) {
			if(value == GroupFilterMode.Tree)
				return XtraPivotGrid.PivotGroupFilterMode.Tree;
			else
				return XtraPivotGrid.PivotGroupFilterMode.List;
		}
		public static FieldSortOrder ToFieldSortOrder(this FieldSortBySummaryOrder sortBySummaryOrder, FieldSortOrder defaultOrder) {
			switch(sortBySummaryOrder) {
				case FieldSortBySummaryOrder.Ascending:
					return FieldSortOrder.Ascending;
				case FieldSortBySummaryOrder.Descending:
					return FieldSortOrder.Descending;
				case FieldSortBySummaryOrder.Default:
					return defaultOrder;
				default:
					throw new ArgumentException("FieldSortBySummaryOrder");
			}
		}
		public static CopyCollapsedValuesMode ToXpfClipboardCopyCollapsedValuesMode(this CoreXtraPivotGrid.CopyCollapsedValuesMode clipboardCopyCollapsedValuesMode) {
			switch (clipboardCopyCollapsedValuesMode) {
				case CoreXtraPivotGrid.CopyCollapsedValuesMode.PreserveCollapsedLevels:
					return CopyCollapsedValuesMode.PreserveCollapsedLevels;
				case CoreXtraPivotGrid.CopyCollapsedValuesMode.RemoveCollapsedLevels:
					return CopyCollapsedValuesMode.RemoveCollapsedLevels;
				case CoreXtraPivotGrid.CopyCollapsedValuesMode.DuplicateCollapsedValues:
					return CopyCollapsedValuesMode.DuplicateCollapsedValues;
				default:
					throw new ArgumentException("copyCollapsedValuesMode");
			}
		}
		public static CoreXtraPivotGrid.CopyCollapsedValuesMode ToCoreCopyCollapsedValuesMode(this CopyCollapsedValuesMode clipboardCopyCollapsedValuesMode) {
			switch (clipboardCopyCollapsedValuesMode) {
				case CopyCollapsedValuesMode.PreserveCollapsedLevels:
					return CoreXtraPivotGrid.CopyCollapsedValuesMode.PreserveCollapsedLevels;
				case CopyCollapsedValuesMode.RemoveCollapsedLevels:
					return CoreXtraPivotGrid.CopyCollapsedValuesMode.RemoveCollapsedLevels;
				case CopyCollapsedValuesMode.DuplicateCollapsedValues:
					return CoreXtraPivotGrid.CopyCollapsedValuesMode.DuplicateCollapsedValues;
				default:
					throw new ArgumentException("copyCollapsedValuesMode");
			}
		}
		public static CopyMultiSelectionMode ToXpfMultiSelectionMode(this CoreXtraPivotGrid.CopyMultiSelectionMode multiSelectionMode) {
			switch(multiSelectionMode) {
				case CoreXtraPivotGrid.CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows:
					return CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows;
				case CoreXtraPivotGrid.CopyMultiSelectionMode.IncludeIntermediateColumnsAndRows:
					return CopyMultiSelectionMode.IncludeIntermediateColumnsAndRows;
				default:
					throw new ArgumentException("multiSelectionMode");
			}
		}
		public static CoreXtraPivotGrid.CopyMultiSelectionMode ToCoreMultiSelectionMode(this CopyMultiSelectionMode multiSelectionMode) {
			switch(multiSelectionMode) {
				case CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows:
					return CoreXtraPivotGrid.CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows;
				case CopyMultiSelectionMode.IncludeIntermediateColumnsAndRows:
					return CoreXtraPivotGrid.CopyMultiSelectionMode.IncludeIntermediateColumnsAndRows;
				default:
					throw new ArgumentException("multiSelectionMode");
			}
		}
	}	
}
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum FieldListActualArea {
		RowArea = 0,
		ColumnArea = 1,
		FilterArea = 2,
		DataArea = 3,
		HiddenFields = 5,
		AllFields = 6
	}
}
