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
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Commands {
	public struct DashboardCommandId : IConvertToInt<DashboardCommandId>, IEquatable<DashboardCommandId> {
		public static readonly DashboardCommandId None = CreateCommand();
		public static readonly DashboardCommandId NewDashboard = CreateCommand();
		public static readonly DashboardCommandId OpenDashboard = CreateCommand();
		public static readonly DashboardCommandId OpenDashboardPath = CreateCommand();
		public static readonly DashboardCommandId SaveDashboard = CreateCommand();
		public static readonly DashboardCommandId SaveAsDashboard = CreateCommand();
		public static readonly DashboardCommandId Undo = CreateCommand();
		public static readonly DashboardCommandId Redo = CreateCommand();
		public static readonly DashboardCommandId NewDataSource = CreateCommand();
		public static readonly DashboardCommandId EditDataSource = CreateCommand();
		public static readonly DashboardCommandId DeleteDataSource = CreateCommand();
		public static readonly DashboardCommandId RenameDataSource = CreateCommand();
		public static readonly DashboardCommandId InsertPivot = CreateCommand();
		public static readonly DashboardCommandId InsertGrid = CreateCommand();
		public static readonly DashboardCommandId InsertChart = CreateCommand();
		public static readonly DashboardCommandId InsertScatterChart = CreateCommand();
		public static readonly DashboardCommandId InsertPies = CreateCommand();
		public static readonly DashboardCommandId InsertGauges = CreateCommand();
		public static readonly DashboardCommandId InsertCards = CreateCommand();
		public static readonly DashboardCommandId InsertImage = CreateCommand();
		public static readonly DashboardCommandId InsertTextBox = CreateCommand();
		public static readonly DashboardCommandId InsertChoroplethMap = CreateCommand();
		public static readonly DashboardCommandId InsertRangeFilter = CreateCommand();
		public static readonly DashboardCommandId InsertComboBox = CreateCommand();
		public static readonly DashboardCommandId InsertListBox = CreateCommand();
		public static readonly DashboardCommandId InsertTreeView = CreateCommand();
		public static readonly DashboardCommandId InsertGroup = CreateCommand();
		public static readonly DashboardCommandId DuplicateItem = CreateCommand();
		public static readonly DashboardCommandId TransposeItem = CreateCommand();
		public static readonly DashboardCommandId RemoveDataItems = CreateCommand();
		public static readonly DashboardCommandId ConvertDashboardItemType = CreateCommand();
		public static readonly DashboardCommandId ConvertToPivot = CreateCommand();
		public static readonly DashboardCommandId ConvertToGrid = CreateCommand();
		public static readonly DashboardCommandId ConvertToChart = CreateCommand();
		public static readonly DashboardCommandId ConvertToScatterChart = CreateCommand();
		public static readonly DashboardCommandId ConvertToRangeFilter = CreateCommand();
		public static readonly DashboardCommandId ConvertToGauge = CreateCommand();
		public static readonly DashboardCommandId ConvertToCard = CreateCommand();
		public static readonly DashboardCommandId ConvertGeoPointMapBase = CreateCommand();
		public static readonly DashboardCommandId ConvertToChoroplethMap = CreateCommand();
		public static readonly DashboardCommandId ConvertToGeoPointMap = CreateCommand();
		public static readonly DashboardCommandId ConvertToBubbleMap = CreateCommand();
		public static readonly DashboardCommandId ConvertToPieMap = CreateCommand();
		public static readonly DashboardCommandId ConvertToPie = CreateCommand();
		public static readonly DashboardCommandId ConvertToComboBox = CreateCommand();
		public static readonly DashboardCommandId ConvertToListBox = CreateCommand();
		public static readonly DashboardCommandId ConvertToTreeView = CreateCommand();
		public static readonly DashboardCommandId DeleteItem = CreateCommand();
		public static readonly DashboardCommandId DeleteGroup = CreateCommand();
		public static readonly DashboardCommandId SetDashboardCurrencyCulture = CreateCommand();
		public static readonly DashboardCommandId DashboardTitle = CreateCommand();
		public static readonly DashboardCommandId EditDashboardColorScheme = CreateCommand();
		public static readonly DashboardCommandId EditNames = CreateCommand();
		public static readonly DashboardCommandId EditRules = CreateCommand();
		public static readonly DashboardCommandId ShowCaption = CreateCommand();
		public static readonly DashboardCommandId EditFilter = CreateCommand();
		public static readonly DashboardCommandId ClearFilter = CreateCommand();
		public static readonly DashboardCommandId MasterFilter = CreateCommand();
		public static readonly DashboardCommandId MultipleValuesMasterFilter = CreateCommand();
		public static readonly DashboardCommandId ChartTargetDimensionsArguments = CreateCommand();
		public static readonly DashboardCommandId ChartTargetDimensionsSeries = CreateCommand();
		public static readonly DashboardCommandId ChartTargetDimensionsPoints = CreateCommand();
		public static readonly DashboardCommandId PieTargetDimensionsArguments = CreateCommand();
		public static readonly DashboardCommandId PieTargetDimensionsSeries = CreateCommand();
		public static readonly DashboardCommandId PieTargetDimensionsPoints = CreateCommand();
		public static readonly DashboardCommandId CrossDataSourceFiltering = CreateCommand();
		public static readonly DashboardCommandId IgnoreMasterFilters = CreateCommand();
		public static readonly DashboardCommandId DrillDown = CreateCommand();
		public static readonly DashboardCommandId GroupIgnoreMasterFilters = CreateCommand();
		public static readonly DashboardCommandId GroupMasterFilter = CreateCommand();
		public static readonly DashboardCommandId ContentAutoArrange = CreateCommand();
		public static readonly DashboardCommandId ContentArrangeInColumns = CreateCommand();
		public static readonly DashboardCommandId ContentArrangeInRows = CreateCommand();
		public static readonly DashboardCommandId ContentArrangementCount = CreateCommand();
		public static readonly DashboardCommandId GridHorizontalLines = CreateCommand();
		public static readonly DashboardCommandId GridWordWrap = CreateCommand();
		public static readonly DashboardCommandId GridAutoFitToContentsColumnWidthMode = CreateCommand();
		public static readonly DashboardCommandId GridAutoFitToGridColumnWidthMode = CreateCommand();
		public static readonly DashboardCommandId GridManualGridColumnWidthMode = CreateCommand();
		public static readonly DashboardCommandId GridVerticalLines = CreateCommand();
		public static readonly DashboardCommandId GridMergeCells = CreateCommand();
		public static readonly DashboardCommandId GridBandedRows = CreateCommand();
		public static readonly DashboardCommandId GridColumnHeaders = CreateCommand();
		public static readonly DashboardCommandId ChartRotate = CreateCommand();
		public static readonly DashboardCommandId ScatterChartRotate = CreateCommand();
		public static readonly DashboardCommandId ChartSeriesType = CreateCommand();
		public static readonly DashboardCommandId ChartShowLegend = CreateCommand();
		public static readonly DashboardCommandId ScatterChartShowLegend = CreateCommand();
		public static readonly DashboardCommandId ChartLegendPosition = CreateCommand();
		public static readonly DashboardCommandId ScatterChartLegendPosition = CreateCommand();
		public static readonly DashboardCommandId ChartYAxisSettings = CreateCommand();
		public static readonly DashboardCommandId ScatterChartXAxisSettings = CreateCommand();
		public static readonly DashboardCommandId ScatterChartYAxisSettings = CreateCommand();
		public static readonly DashboardCommandId ScatterChartLabelOptions = CreateCommand();
		public static readonly DashboardCommandId ChartXAxisSettings = CreateCommand();
		public static readonly DashboardCommandId PieStylePie = CreateCommand();
		public static readonly DashboardCommandId PieStyleDonut = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabels = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltips = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsNone = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsArgument = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsValue = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsArgumentAndValue = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsValueAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsArgumentAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsDataLabelsArgumentValueAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsNone = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsArgument = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsValue = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsArgumentAndValue = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsValueAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsArgumentAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieLabelsTooltipsArgumentValueAndPercent = CreateCommand();
		public static readonly DashboardCommandId PieShowCaptions = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleFullCircular = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleHalfCircular = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleLeftQuarterCircular = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleRightQuarterCircular = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleThreeFourthCircular = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleLinearHorizontal = CreateCommand();
		public static readonly DashboardCommandId GaugeStyleLinearVertical = CreateCommand();
		public static readonly DashboardCommandId GaugeShowCaptions = CreateCommand();
		public static readonly DashboardCommandId ImageLoad = CreateCommand();
		public static readonly DashboardCommandId ImageImport = CreateCommand();
		public static readonly DashboardCommandId ImageSizeModeClip = CreateCommand();
		public static readonly DashboardCommandId ImageSizeModeStretch = CreateCommand();
		public static readonly DashboardCommandId ImageSizeModeSqueeze = CreateCommand();
		public static readonly DashboardCommandId ImageSizeModeZoom = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentTopLeft = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentCenterLeft = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentBottomLeft = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentTopCenter = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentCenterCenter = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentBottomCenter = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentTopRight = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentCenterRight = CreateCommand();
		public static readonly DashboardCommandId ImageAlignmentBottomRight = CreateCommand();
		public static readonly DashboardCommandId MapLoad = CreateCommand();
		public static readonly DashboardCommandId MapImport = CreateCommand();
		public static readonly DashboardCommandId MapDefaultShapefile = CreateCommand();
		public static readonly DashboardCommandId MapWorldCountries = CreateCommand();
		public static readonly DashboardCommandId MapEurope = CreateCommand();
		public static readonly DashboardCommandId MapAsia = CreateCommand();
		public static readonly DashboardCommandId MapNorthAmerica = CreateCommand();
		public static readonly DashboardCommandId MapSouthAmerica = CreateCommand();
		public static readonly DashboardCommandId MapAfrica = CreateCommand();
		public static readonly DashboardCommandId MapUSA = CreateCommand();
		public static readonly DashboardCommandId MapCanada = CreateCommand();
		public static readonly DashboardCommandId MapShowLegend = CreateCommand();
		public static readonly DashboardCommandId MapLegendPosition = CreateCommand();
		public static readonly DashboardCommandId WeightedLegendPosition = CreateCommand();
		public static readonly DashboardCommandId WeightedLegendChangeType = CreateCommand();
		public static readonly DashboardCommandId WeightedLegendNoneType = CreateCommand();
		public static readonly DashboardCommandId WeightedLegendLinearType = CreateCommand();
		public static readonly DashboardCommandId WeightedLegendNestedType = CreateCommand();
		public static readonly DashboardCommandId MapFullExtent = CreateCommand();
		public static readonly DashboardCommandId GeoPointMapClusterization = CreateCommand();
		public static readonly DashboardCommandId PieMapIsWeightedCommand = CreateCommand();
		public static readonly DashboardCommandId MapLockNavigationCommand = CreateCommand();
		public static readonly DashboardCommandId MapShapeTitleAttributeCommand = CreateCommand();
		public static readonly DashboardCommandId ChoroplethMapShapeLabelsAttributeCommand = CreateCommand();
		public static readonly DashboardCommandId TextBoxEditText = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeLine = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeStackedLine = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeFullStackedLine = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeArea = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeStackedArea = CreateCommand();
		public static readonly DashboardCommandId RangeFilterSeriesTypeFullStackedArea = CreateCommand();
		public static readonly DashboardCommandId PivotInitialState = CreateCommand();
		public static readonly DashboardCommandId PivotAutoExpandColumn = CreateCommand();
		public static readonly DashboardCommandId PivotAutoExpandRow = CreateCommand();
		public static readonly DashboardCommandId PivotShowGrandTotals = CreateCommand();
		public static readonly DashboardCommandId PivotShowColumnGrandTotals = CreateCommand();
		public static readonly DashboardCommandId PivotShowRowGrandTotals = CreateCommand();
		public static readonly DashboardCommandId PivotShowTotals = CreateCommand();
		public static readonly DashboardCommandId PivotShowColumnTotals = CreateCommand();
		public static readonly DashboardCommandId PivotShowRowTotals = CreateCommand();
		public static readonly DashboardCommandId FilterPanelHorizontalElementsOrientation = CreateCommand();
		public static readonly DashboardCommandId FilterPanelVerticalElementsOrientation = CreateCommand();
		public static readonly DashboardCommandId FilterPanelShowFilterElementCaptions = CreateCommand();
		public static readonly DashboardCommandId AddCalculatedField = CreateCommand();
		public static readonly DashboardCommandId EditCalculatedField = CreateCommand();
		public static readonly DashboardCommandId EditDataSourceFilter = CreateCommand();
		public static readonly DashboardCommandId ClearDataSourceFilter = CreateCommand();
		public static readonly DashboardCommandId EditDashboardParameters = CreateCommand();
		public static readonly DashboardCommandId ServerMode = CreateCommand();
		public static readonly DashboardCommandId SelectGeoPointDashboardItemType = CreateCommand();
		public static readonly DashboardCommandId SelectFilterElementType = CreateCommand();
		public static readonly DashboardCommandId InsertGeoPointMap = CreateCommand();
		public static readonly DashboardCommandId InsertBubbleMap = CreateCommand();
		public static readonly DashboardCommandId InsertPieMap = CreateCommand();
		public static readonly DashboardCommandId ComboBoxTypeStandard = CreateCommand();
		public static readonly DashboardCommandId ComboBoxTypeChecked = CreateCommand();
		public static readonly DashboardCommandId ListBoxTypeChecked = CreateCommand();
		public static readonly DashboardCommandId ListBoxTypeRadio = CreateCommand();
		public static readonly DashboardCommandId TreeViewTypeStandard = CreateCommand();
		public static readonly DashboardCommandId TreeViewTypeChecked = CreateCommand();
		public static readonly DashboardCommandId TreeViewAutoExpand = CreateCommand();
		public static readonly DashboardCommandId FilterElementShowAllValue = CreateCommand();
		public static readonly DashboardCommandId UseGlobalColors = CreateCommand();
		public static readonly DashboardCommandId UseLocalColors = CreateCommand();
		public static readonly DashboardCommandId EditActualColorScheme = CreateCommand();
		public static readonly DashboardCommandId EditSqlConnection = CreateCommand();
		public static readonly DashboardCommandId EditObjectDataSource = CreateCommand();
		public static readonly DashboardCommandId EditExcelDataSource = CreateCommand();
		public static readonly DashboardCommandId EditOlapConnection = CreateCommand();
		public static readonly DashboardCommandId EditEFDataSource = CreateCommand();
		public static readonly DashboardCommandId AddQuery = CreateCommand();
		public static readonly DashboardCommandId EditQuery = CreateCommand();
		public static readonly DashboardCommandId RenameQuery = CreateCommand();
		public static readonly DashboardCommandId EditQueryFilter = CreateCommand();
		public static readonly DashboardCommandId DeleteQuery = CreateCommand();
		public static readonly DashboardCommandId AutomaticUpdates = CreateCommand();
		public static readonly DashboardCommandId UpdateData = CreateCommand();
		static int lastCommandId = -1;
		static DashboardCommandId CreateCommand() {
			return new DashboardCommandId(++lastCommandId);
		}
		readonly int value;
		public DashboardCommandId(int value) {
			this.value = value;
		}
		int IConvertToInt<DashboardCommandId>.ToInt() {
			return value;
		}
		DashboardCommandId IConvertToInt<DashboardCommandId>.FromInt(int value) {
			return new DashboardCommandId(value);
		}
		public override bool Equals(object obj) {
			return ((obj is DashboardCommandId) && value == ((DashboardCommandId)obj).value);
		}
		public override int GetHashCode() {
			return value.GetHashCode();
		}
		public override string ToString() {
			return value.ToString();
		}
		public bool Equals(DashboardCommandId other) {
			return value == other.value;
		}
	}
}
