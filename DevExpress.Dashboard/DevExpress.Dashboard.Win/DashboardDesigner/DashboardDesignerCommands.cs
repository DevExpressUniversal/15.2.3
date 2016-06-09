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

using DevExpress.DashboardWin.Commands;
namespace DevExpress.DashboardWin {
	public partial class DashboardDesigner {
		void AddCommands() {
			AddCommand(DashboardCommandId.None, typeof(UnsupportedCommand));
			AddCommand(DashboardCommandId.TextBoxEditText, typeof(TextBoxEditTextCommand));
			AddFileOperationsCommands();
			AddHistoryCommands();
			AddDataSourceCommands();
			AddInsertCommands();
			AddCommonCommands();
			AddGroupCommands();
			AddDashboardCommands();
			AddFilterCommands();
			AddInteractionCommands();
			AddContentArrangementCommands();
			AddGridCommands();
			AddChartCommands();
			AddPiesCommands();
			AddGaugeCommands();
			AddImageCommands();
			AddRangeFilterCommands();
			AddPivotCommands();
			AddMapCommands();
			AddFilterElementCommands();
			AddColoringCommands();
		}
		void AddDashboardCommands() {
			AddCommand(DashboardCommandId.SetDashboardCurrencyCulture, typeof(SetDashboardCurrencyCultureCommand));
			AddCommand(DashboardCommandId.DashboardTitle, typeof(DashboardTitleCommand));
			AddCommand(DashboardCommandId.EditDashboardParameters, typeof(EditDashboardParametersCommand));
			AddCommand(DashboardCommandId.EditDashboardColorScheme, typeof(EditDashboardColorSchemeCommand));
			AddCommand(DashboardCommandId.UpdateData, typeof(UpdateDataCommand));
			AddCommand(DashboardCommandId.AutomaticUpdates, typeof(AutomaticUpdatesCommand));
		}
		void AddHistoryCommands() {
			AddCommand(DashboardCommandId.Undo, typeof(UndoCommand));
			AddCommand(DashboardCommandId.Redo, typeof(RedoCommand));
		}
		void AddFileOperationsCommands() {
			AddCommand(DashboardCommandId.NewDashboard, typeof(NewDashboardCommand));
			AddCommand(DashboardCommandId.OpenDashboard, typeof(OpenDashboardCommand));
			AddCommand(DashboardCommandId.OpenDashboardPath, typeof(OpenDashboardPathCommand));
			AddCommand(DashboardCommandId.SaveDashboard, typeof(SaveDashboardCommand));
			AddCommand(DashboardCommandId.SaveAsDashboard, typeof(SaveAsDashboardCommand));
		}
		void AddInsertCommands() {
			AddCommand(DashboardCommandId.InsertPivot, typeof(InsertPivotCommand));
			AddCommand(DashboardCommandId.InsertGrid, typeof(InsertGridCommand));
			AddCommand(DashboardCommandId.InsertChart, typeof(InsertChartCommand));
			AddCommand(DashboardCommandId.InsertScatterChart, typeof(InsertScatterChartCommand));
			AddCommand(DashboardCommandId.InsertPies, typeof(InsertPiesCommand));
			AddCommand(DashboardCommandId.InsertGauges, typeof(InsertGaugesCommand));
			AddCommand(DashboardCommandId.InsertCards, typeof(InsertCardsCommand));
			AddCommand(DashboardCommandId.InsertChoroplethMap, typeof(InsertChoroplethMapCommand));
			AddCommand(DashboardCommandId.SelectGeoPointDashboardItemType, typeof(InsertGeoPointMapBaseCommand));
			AddCommand(DashboardCommandId.InsertGeoPointMap, typeof(InsertGeoPointMapCommand));
			AddCommand(DashboardCommandId.InsertBubbleMap, typeof(InsertBubbleMapCommand));
			AddCommand(DashboardCommandId.InsertPieMap, typeof(InsertPieMapCommand));
			AddCommand(DashboardCommandId.InsertRangeFilter, typeof(InsertRangeFilterCommand));
			AddCommand(DashboardCommandId.SelectFilterElementType, typeof(InsertFilterElementBaseCommand));
			AddCommand(DashboardCommandId.InsertComboBox, typeof(InsertComboBoxCommand));
			AddCommand(DashboardCommandId.InsertListBox, typeof(InsertListBoxCommand));
			AddCommand(DashboardCommandId.InsertTreeView, typeof(InsertTreeViewCommand));
			AddCommand(DashboardCommandId.InsertGroup, typeof(InsertGroupCommand));
			AddCommand(DashboardCommandId.InsertImage, typeof(InsertImageCommand));
			AddCommand(DashboardCommandId.InsertTextBox, typeof(InsertTextBoxCommand));
		}
		void AddFilterCommands() {
			AddCommand(DashboardCommandId.EditFilter, typeof(EditFilterCommand));
			AddCommand(DashboardCommandId.ClearFilter, typeof(ClearFilterCommand));
		}
		void AddContentArrangementCommands() {
			AddCommand(DashboardCommandId.ContentAutoArrange, typeof(ContentAutoArrangeCommand));
			AddCommand(DashboardCommandId.ContentArrangeInColumns, typeof(ContentArrangeInColumnsCommand));
			AddCommand(DashboardCommandId.ContentArrangeInRows, typeof(ContentArrangeInRowsCommand));
			AddCommand(DashboardCommandId.ContentArrangementCount, typeof(ContentArrangementCountCommand));
		}
		void AddChartCommands() {
			AddCommand(DashboardCommandId.ChartRotate, typeof(ChartRotateCommand));
			AddCommand(DashboardCommandId.ScatterChartRotate, typeof(ScatterChartRotateCommand));
			AddCommand(DashboardCommandId.ChartSeriesType, typeof(ChartSeriesTypeCommand));
			AddCommand(DashboardCommandId.ChartShowLegend, typeof(ChartShowLegendCommand));
			AddCommand(DashboardCommandId.ScatterChartShowLegend, typeof(ScatterChartShowLegendCommand));
			AddCommand(DashboardCommandId.ChartLegendPosition, typeof(ChartLegendPositionCommand));
			AddCommand(DashboardCommandId.ScatterChartLegendPosition, typeof(ScatterChartLegendPositionCommand));
			AddCommand(DashboardCommandId.ChartYAxisSettings, typeof(ChartYAxisSettingsCommand));
			AddCommand(DashboardCommandId.ScatterChartXAxisSettings, typeof(ScatterChartXAxisSettingsCommand));
			AddCommand(DashboardCommandId.ScatterChartYAxisSettings, typeof(ScatterChartYAxisSettingsCommand));
			AddCommand(DashboardCommandId.ScatterChartLabelOptions, typeof(ScatterChartLabelOptionsCommand));
			AddCommand(DashboardCommandId.ChartXAxisSettings, typeof(ChartXAxisSettingsCommand));
		}
		void AddCommonCommands() {
			AddCommand(DashboardCommandId.DeleteItem, typeof(DeleteItemCommand));
			AddCommand(DashboardCommandId.DuplicateItem, typeof(DuplicateItemCommand));
			AddCommand(DashboardCommandId.EditNames, typeof(EditNamesCommand));
			AddCommand(DashboardCommandId.EditRules, typeof(EditRulesCommand));
			AddCommand(DashboardCommandId.ShowCaption, typeof(ShowCaptionCommand));
			AddCommand(DashboardCommandId.RemoveDataItems, typeof(RemoveDataItemsCommand));
			AddCommand(DashboardCommandId.TransposeItem, typeof(TransposeItemCommand));
			AddCommand(DashboardCommandId.ConvertDashboardItemType, typeof(ConvertDashboardItemTypeCommand));
			AddCommand(DashboardCommandId.ConvertToPivot, typeof(ConvertToPivotCommand));
			AddCommand(DashboardCommandId.ConvertToGrid, typeof(ConvertToGridCommand));
			AddCommand(DashboardCommandId.ConvertToChart, typeof(ConvertToChartCommand));
			AddCommand(DashboardCommandId.ConvertToScatterChart, typeof(ConvertToScatterChartCommand));
			AddCommand(DashboardCommandId.ConvertToPie, typeof(ConvertToPieCommand));
			AddCommand(DashboardCommandId.ConvertToGauge, typeof(ConvertToGaugeCommand));
			AddCommand(DashboardCommandId.ConvertToCard, typeof(ConvertToCardCommand));
			AddCommand(DashboardCommandId.ConvertToChoroplethMap, typeof(ConvertToChoroplethMapCommand));
			AddCommand(DashboardCommandId.ConvertGeoPointMapBase, typeof(ConvertGeoPointMapBaseCommandGroup));
			AddCommand(DashboardCommandId.ConvertToGeoPointMap, typeof(ConvertToGeoPointMapCommand));
			AddCommand(DashboardCommandId.ConvertToBubbleMap, typeof(ConvertToBubbleMapCommand));
			AddCommand(DashboardCommandId.ConvertToPieMap, typeof(ConvertToPieMapCommand));
			AddCommand(DashboardCommandId.ConvertToRangeFilter, typeof(ConvertToRangeFilterCommand));
			AddCommand(DashboardCommandId.ConvertToComboBox, typeof(ConvertToComboBoxCommand));
			AddCommand(DashboardCommandId.ConvertToListBox, typeof(ConvertToListBoxCommand));
			AddCommand(DashboardCommandId.ConvertToTreeView, typeof(ConvertToTreeViewCommand));
		}
		void AddGroupCommands() {
			AddCommand(DashboardCommandId.DeleteGroup, typeof(DeleteGroupCommand));
		}
		void AddDataSourceCommands() {
			AddCommand(DashboardCommandId.NewDataSource, typeof(NewDataSourceCommand));
			AddCommand(DashboardCommandId.RenameDataSource, typeof(RenameDataSourceCommand));
			AddCommand(DashboardCommandId.DeleteDataSource, typeof(DeleteDataSourceCommand));
			AddCommand(DashboardCommandId.ServerMode, typeof(ServerModeCommand));
			AddCommand(DashboardCommandId.EditDataSourceFilter, typeof(EditDataSourceFilterCommand));
			AddCommand(DashboardCommandId.ClearDataSourceFilter, typeof(ClearDataSourceFilterCommand));
			AddCommand(DashboardCommandId.AddCalculatedField, typeof(AddCalculatedFieldCommand));
			AddCommand(DashboardCommandId.EditCalculatedField, typeof(EditCalculatedFieldCommand));
			AddCommand(DashboardCommandId.EditSqlConnection, typeof(EditSqlConnectionCommand));
			AddCommand(DashboardCommandId.EditOlapConnection, typeof(EditOlapConnectionCommand));
			AddCommand(DashboardCommandId.EditObjectDataSource, typeof(EditObjectDataSourceCommand));
			AddCommand(DashboardCommandId.EditExcelDataSource, typeof(EditExcelDataSourceCommand));
			AddCommand(DashboardCommandId.EditEFDataSource, typeof(EditEFDataSourceCommand));
			AddCommand(DashboardCommandId.AddQuery, typeof(AddQueryCommand));
			AddCommand(DashboardCommandId.EditQuery, typeof(EditQueryCommand));
			AddCommand(DashboardCommandId.RenameQuery, typeof(RenameQueryCommand));
			AddCommand(DashboardCommandId.EditQueryFilter, typeof(EditQueryFilterCommand));
			AddCommand(DashboardCommandId.DeleteQuery, typeof(DeleteQueryCommand));
		}
		void AddGaugeCommands() {
			AddCommand(DashboardCommandId.GaugeStyleFullCircular, typeof(GaugeStyleFullCircularCommand));
			AddCommand(DashboardCommandId.GaugeStyleHalfCircular, typeof(GaugeStyleHalfCircularCommand));
			AddCommand(DashboardCommandId.GaugeStyleLeftQuarterCircular, typeof(GaugeStyleLeftQuarterCircularCommand));
			AddCommand(DashboardCommandId.GaugeStyleRightQuarterCircular, typeof(GaugeStyleRightQuarterCircularCommand));
			AddCommand(DashboardCommandId.GaugeStyleThreeFourthCircular, typeof(GaugeStyleThreeFourthCircularCommand));
			AddCommand(DashboardCommandId.GaugeStyleLinearHorizontal, typeof(GaugeStyleLinearHorizontalCommand));
			AddCommand(DashboardCommandId.GaugeStyleLinearVertical, typeof(GaugeStyleLinearVerticalCommand));
			AddCommand(DashboardCommandId.GaugeShowCaptions, typeof(GaugeShowCaptionsCommand));
		}
		void AddGridCommands() {
			AddCommand(DashboardCommandId.GridHorizontalLines, typeof(GridHorizontalLinesCommand));
			AddCommand(DashboardCommandId.GridVerticalLines, typeof(GridVerticalLinesCommand));
			AddCommand(DashboardCommandId.GridMergeCells, typeof(GridMergeCellsCommand));
			AddCommand(DashboardCommandId.GridBandedRows, typeof(GridBandedRowsCommand));
			AddCommand(DashboardCommandId.GridColumnHeaders, typeof(GridColumnHeadersCommand));
			AddCommand(DashboardCommandId.GridAutoFitToContentsColumnWidthMode, typeof(GridAutoFitToContentsColumnWidthModeCommand));
			AddCommand(DashboardCommandId.GridAutoFitToGridColumnWidthMode, typeof(GridAutoFitToGridColumnWidthModeCommand));
			AddCommand(DashboardCommandId.GridManualGridColumnWidthMode, typeof(GridManualGridColumnWidthModeCommand));
			AddCommand(DashboardCommandId.GridWordWrap, typeof(GridWordWrapCommand));
		}
		void AddImageCommands() {
			AddCommand(DashboardCommandId.ImageLoad, typeof(ImageLoadCommand));
			AddCommand(DashboardCommandId.ImageImport, typeof(ImageImportCommand));
			AddCommand(DashboardCommandId.ImageSizeModeClip, typeof(ImageSizeModeClipCommand));
			AddCommand(DashboardCommandId.ImageSizeModeStretch, typeof(ImageSizeModeStretchCommand));
			AddCommand(DashboardCommandId.ImageSizeModeSqueeze, typeof(ImageSizeModeSqueezeCommand));
			AddCommand(DashboardCommandId.ImageSizeModeZoom, typeof(ImageSizeModeZoomCommand));
			AddCommand(DashboardCommandId.ImageAlignmentTopLeft, typeof(ImageAlignmentTopLeftCommand));
			AddCommand(DashboardCommandId.ImageAlignmentCenterLeft, typeof(ImageAlignmentCenterLeftCommand));
			AddCommand(DashboardCommandId.ImageAlignmentBottomLeft, typeof(ImageAlignmentBottomLeftCommand));
			AddCommand(DashboardCommandId.ImageAlignmentTopCenter, typeof(ImageAlignmentTopCenterCommand));
			AddCommand(DashboardCommandId.ImageAlignmentCenterCenter, typeof(ImageAlignmentCenterCenterCommand));
			AddCommand(DashboardCommandId.ImageAlignmentBottomCenter, typeof(ImageAlignmentBottomCenterCommand));
			AddCommand(DashboardCommandId.ImageAlignmentTopRight, typeof(ImageAlignmentTopRightCommand));
			AddCommand(DashboardCommandId.ImageAlignmentCenterRight, typeof(ImageAlignmentCenterRightCommand));
			AddCommand(DashboardCommandId.ImageAlignmentBottomRight, typeof(ImageAlignmentBottomRightCommand));
		}
		void AddInteractionCommands() {
			AddCommand(DashboardCommandId.MasterFilter, typeof(SingleMasterFilterCommand));
			AddCommand(DashboardCommandId.MultipleValuesMasterFilter, typeof(MultipleMasterFilterCommand));
			AddCommand(DashboardCommandId.ChartTargetDimensionsArguments, typeof(ChartTargetDimensionsArgumentsCommand));
			AddCommand(DashboardCommandId.ChartTargetDimensionsSeries, typeof(ChartTargetDimensionsSeriesCommand));
			AddCommand(DashboardCommandId.ChartTargetDimensionsPoints, typeof(ChartTargetDimensionsPointsCommand));
			AddCommand(DashboardCommandId.PieTargetDimensionsArguments, typeof(PieTargetDimensionsArgumentsCommand));
			AddCommand(DashboardCommandId.PieTargetDimensionsSeries, typeof(PieTargetDimensionsSeriesCommand));
			AddCommand(DashboardCommandId.PieTargetDimensionsPoints, typeof(PieTargetDimensionsPointsCommand));
			AddCommand(DashboardCommandId.CrossDataSourceFiltering, typeof(CrossDataSourceFilteringCommand));
			AddCommand(DashboardCommandId.IgnoreMasterFilters, typeof(IgnoreMasterFiltersCommand));
			AddCommand(DashboardCommandId.DrillDown, typeof(DrillDownCommand));
			AddCommand(DashboardCommandId.GroupIgnoreMasterFilters, typeof(GroupIgnoreMasterFiltersCommand));
			AddCommand(DashboardCommandId.GroupMasterFilter, typeof(GroupMasterFilterCommand));
		}
		void AddMapCommands() {
			AddCommand(DashboardCommandId.MapLoad, typeof(MapLoadCommand));
			AddCommand(DashboardCommandId.MapImport, typeof(MapImportCommand));
			AddCommand(DashboardCommandId.MapDefaultShapefile, typeof(MapDefaultShapefileCommand));
			AddCommand(DashboardCommandId.MapWorldCountries, typeof(MapWorldCountriesLoadCommand));
			AddCommand(DashboardCommandId.MapEurope, typeof(MapEuropeLoadCommand));
			AddCommand(DashboardCommandId.MapAsia, typeof(MapAsiaLoadCommand));
			AddCommand(DashboardCommandId.MapNorthAmerica, typeof(MapNorthAmericaLoadCommand));
			AddCommand(DashboardCommandId.MapSouthAmerica, typeof(MapSouthAmericaLoadCommand));
			AddCommand(DashboardCommandId.MapAfrica, typeof(MapAfricaLoadCommand));
			AddCommand(DashboardCommandId.MapUSA, typeof(MapUSALoadCommand));
			AddCommand(DashboardCommandId.MapCanada, typeof(MapCanadaLoadCommand));
			AddCommand(DashboardCommandId.MapShowLegend, typeof(MapShowLegendCommand));
			AddCommand(DashboardCommandId.WeightedLegendNoneType, typeof(WeightedLegendNoneCommand));
			AddCommand(DashboardCommandId.MapLegendPosition, typeof(MapLegendPositionCommand));
			AddCommand(DashboardCommandId.WeightedLegendPosition, typeof(WeightedLegendPositionCommand));
			AddCommand(DashboardCommandId.WeightedLegendChangeType, typeof(WeightedLegendChangeTypeCommand));
			AddCommand(DashboardCommandId.WeightedLegendLinearType, typeof(WeightedLegendLinearTypeCommand));
			AddCommand(DashboardCommandId.WeightedLegendNestedType, typeof(WeightedLegendNestedTypeCommand));
			AddCommand(DashboardCommandId.MapFullExtent, typeof(MapFullExtentCommand));
			AddCommand(DashboardCommandId.GeoPointMapClusterization, typeof(GeoPointMapClusterizationCommand));
			AddCommand(DashboardCommandId.PieMapIsWeightedCommand, typeof(PieMapIsWeightedCommand));
			AddCommand(DashboardCommandId.MapLockNavigationCommand, typeof(MapLockNavigationCommand));
			AddCommand(DashboardCommandId.MapShapeTitleAttributeCommand, typeof(MapShapeTitleAttributeCommand));
			AddCommand(DashboardCommandId.ChoroplethMapShapeLabelsAttributeCommand, typeof(ChoroplethMapShapeLabelsAttributeCommand));
		}
		void AddPiesCommands() {
			AddCommand(DashboardCommandId.PieStylePie, typeof(PieStylePieCommand));
			AddCommand(DashboardCommandId.PieStyleDonut, typeof(PieStyleDonutCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabels, typeof(PieLabelsDataLabelsCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltips, typeof(PieLabelsTooltipsCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsNone, typeof(PieLabelsDataLabelsNoneCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsArgument, typeof(PieLabelsDataLabelsArgumentCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsValue, typeof(PieLabelsDataLabelsValueCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsArgumentAndValue, typeof(PieLabelsDataLabelsArgumentAndValueCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsPercent, typeof(PieLabelsDataLabelsPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsValueAndPercent, typeof(PieLabelsDataLabelsValueAndPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsArgumentAndPercent, typeof(PieLabelsDataLabelsArgumentAndPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsDataLabelsArgumentValueAndPercent, typeof(PieLabelsDataLabelsArgumentValueAndPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsNone, typeof(PieLabelsTooltipsNoneCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsArgument, typeof(PieLabelsTooltipsArgumentCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsValue, typeof(PieLabelsTooltipsValueCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsArgumentAndValue, typeof(PieLabelsTooltipsArgumentAndValueCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsPercent, typeof(PieLabelsTooltipsPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsValueAndPercent, typeof(PieLabelsTooltipsValueAndPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsArgumentAndPercent, typeof(PieLabelsTooltipsArgumentAndPercentCommand));
			AddCommand(DashboardCommandId.PieLabelsTooltipsArgumentValueAndPercent, typeof(PieLabelsTooltipsArgumentValueAndPercentCommand));
			AddCommand(DashboardCommandId.PieShowCaptions, typeof(PieShowCaptionsCommand));
		}
		void AddPivotCommands() {
			AddCommand(DashboardCommandId.PivotInitialState, typeof(PivotInitialStateCommand));
			AddCommand(DashboardCommandId.PivotAutoExpandColumn, typeof(PivotAutoExpandColumnCommand));
			AddCommand(DashboardCommandId.PivotAutoExpandRow, typeof(PivotAutoExpandRowCommand));
			AddCommand(DashboardCommandId.PivotShowGrandTotals, typeof(PivotShowGrandTotalsCommand));
			AddCommand(DashboardCommandId.PivotShowColumnGrandTotals, typeof(PivotShowColumnGrandTotalsCommand));
			AddCommand(DashboardCommandId.PivotShowRowGrandTotals, typeof(PivotShowRowGrandTotalsCommand));
			AddCommand(DashboardCommandId.PivotShowTotals, typeof(PivotShowTotalsCommand));
			AddCommand(DashboardCommandId.PivotShowColumnTotals, typeof(PivotShowColumnTotalsCommand));
			AddCommand(DashboardCommandId.PivotShowRowTotals, typeof(PivotShowRowTotalsCommand));
		}
		void AddRangeFilterCommands() {
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeLine, typeof(RangeFilterSeriesTypeLineCommand));
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeStackedLine, typeof(RangeFilterSeriesTypeStackedLineCommand));
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeFullStackedLine, typeof(RangeFilterSeriesTypeFullStackedLineCommand));
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeArea, typeof(RangeFilterSeriesTypeAreaCommand));
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeStackedArea, typeof(RangeFilterSeriesTypeStackedAreaCommand));
			AddCommand(DashboardCommandId.RangeFilterSeriesTypeFullStackedArea, typeof(RangeFilterSeriesTypeFullStackedAreaCommand));
		}
		void AddFilterElementCommands() {
			AddCommand(DashboardCommandId.ComboBoxTypeStandard, typeof(ComboBoxStandardTypeCommand));
			AddCommand(DashboardCommandId.ComboBoxTypeChecked, typeof(ComboBoxCheckedTypeCommand));
			AddCommand(DashboardCommandId.ListBoxTypeChecked, typeof(ListBoxCheckedTypeCommand));
			AddCommand(DashboardCommandId.ListBoxTypeRadio, typeof(ListBoxRadioTypeCommand));
			AddCommand(DashboardCommandId.TreeViewTypeStandard, typeof(TreeViewStandardTypeCommand));
			AddCommand(DashboardCommandId.TreeViewTypeChecked, typeof(TreeViewCheckedTypeCommand));
			AddCommand(DashboardCommandId.TreeViewAutoExpand, typeof(TreeViewAutoExpandCommand));
			AddCommand(DashboardCommandId.FilterElementShowAllValue, typeof(FilterElementShowAllValueCommand));
		}
		void AddColoringCommands() {
			AddCommand(DashboardCommandId.UseGlobalColors, typeof(UseGlobalColorsCommand));
			AddCommand(DashboardCommandId.UseLocalColors, typeof(UseLocalColorsCommand));
			AddCommand(DashboardCommandId.EditActualColorScheme, typeof(EditActualColorSchemeCommand));
		}
	}
}
