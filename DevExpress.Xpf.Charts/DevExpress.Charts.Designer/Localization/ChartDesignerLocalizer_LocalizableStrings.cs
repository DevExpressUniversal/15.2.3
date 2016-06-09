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
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed class ChartDesignerLocalizer : XtraLocalizer<ChartDesignerStringIDs> {
		internal const string DiagramTypeStringPrefix = "TreeDiagram";
		#region Hard class members
		static ChartDesignerLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ChartDesignerStringIDs>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<ChartDesignerStringIDs> CreateDefaultLocalizer() {
			return new ChartDesignerResXLocalizer();
		}
		public static string GetString(ChartDesignerStringIDs id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ChartDesignerStringIDs> CreateResXLocalizer() {
			return new ChartDesignerResXLocalizer();
		}
		#endregion
		public static string GetLocalizedDiagramTypeName(Type diagramType) {
			if (!(diagramType.IsSubclassOf(typeof(Diagram))))
				throw new ChartDesignerException("The 'diagramType' parameter must be a subclass of the Diagram type.");
			return GetString((ChartDesignerStringIDs)Enum.Parse(typeof(ChartDesignerStringIDs), DiagramTypeStringPrefix + diagramType.Name));
		}
		public static string GetLocalizedSeriesTypeName(Type seriesType) {
			if (!(seriesType.IsSubclassOf(typeof(Series))))
				throw new ChartDesignerException("The 'seriesType' parameter must be a subclass of the Series type.");
			return GetString((ChartDesignerStringIDs)Enum.Parse(typeof(ChartDesignerStringIDs), seriesType.Name));
		}
		public static string GetLocalizedIndicatorTypeName(Type indicatorType) {
			if (!(indicatorType.IsSubclassOf(typeof(Indicator))))
				throw new ChartDesignerException("The 'indicatorType' parameter must be a subclass of the Indicator type.");
			return GetString((ChartDesignerStringIDs)Enum.Parse(typeof(ChartDesignerStringIDs), indicatorType.Name));
		}
		protected override void PopulateStringTable() {
			#region Designer Window
			AddString(ChartDesignerStringIDs.ChartDesignerWindowTitle, "Chart Designer");
			AddString(ChartDesignerStringIDs.ChartStructureDockPanelTitle, "Chart Structure");
			AddString(ChartDesignerStringIDs.PropertiesDockPanelTitle, "Properties");
			AddString(ChartDesignerStringIDs.SeriesDataPanelTitle, "Series Data");
			AddString(ChartDesignerStringIDs.ClearSeriesDataButtonCaption, "Clear Series Data");
			AddString(ChartDesignerStringIDs.CancelButtonCaption, "Cancel");
			AddString(ChartDesignerStringIDs.SaveAndExitButtonCaption, "Save and Exit");
			AddString(ChartDesignerStringIDs.SaveAndExitWindowText, "Do you want to save the settings?");
			AddString(ChartDesignerStringIDs.EmptyDiagramHint, "There is no data to visualize. To see the chart, switch to the Chart ribbon tab and choose a desired series type from the Add New Series gallery.");
			#endregion
			#region Font
			AddString(ChartDesignerStringIDs.Font_Bold, "Bold");
			AddString(ChartDesignerStringIDs.Font_Italic, "Italic");
			AddString(ChartDesignerStringIDs.Font, "Font");
			AddString(ChartDesignerStringIDs.Font_Family, "Name:");
			AddString(ChartDesignerStringIDs.Font_Size, "Size:");
			#endregion
			#region Gallery Group Captions
			AddString(ChartDesignerStringIDs.ChartViewGallery_BarSeries, "Bar Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_LineSeries, "Line Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Area, "Area Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_PointBubble, "Point and Bubble Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Financial, "Financial Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Pie, "Pie Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Funnel, "Funnel Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Polar, "Polar Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_Radar, "Radar Series");
			AddString(ChartDesignerStringIDs.ChartViewGallery_RangeSeries, "Range Series");
			#endregion
			#region SeriesNames
			AddString(ChartDesignerStringIDs.SeriesOptions_ChangeSeriesView, "Change Series View");
			AddString(ChartDesignerStringIDs.PieSeries2D, "Pie 2D");
			AddString(ChartDesignerStringIDs.PieSeries3D, "Pie 3D");
			AddString(ChartDesignerStringIDs.PolarAreaSeries2D, "Polar Area 2D");
			AddString(ChartDesignerStringIDs.RadarAreaSeries2D, "Radar Area 2D");
			AddString(ChartDesignerStringIDs.PolarLineSeries2D, " Polar Line 2D");
			AddString(ChartDesignerStringIDs.RadarLineSeries2D, "Radar Line 2D");
			AddString(ChartDesignerStringIDs.PolarLineScatterSeries2D, "Scatter Polar Line 2D");
			AddString(ChartDesignerStringIDs.RadarLineScatterSeries2D, "Scatter Radar Line 2D");
			AddString(ChartDesignerStringIDs.PolarPointSeries2D, "Polar Point 2D");
			AddString(ChartDesignerStringIDs.RadarPointSeries2D, "Radar Point 2D");
			AddString(ChartDesignerStringIDs.RangeAreaSeries2D, "Range Area 2D");
			AddString(ChartDesignerStringIDs.BarSideBySideSeries2D, "Side-by-Side Bar 2D");
			AddString(ChartDesignerStringIDs.BarStackedSeries2D, "Stacked Bar 2D");
			AddString(ChartDesignerStringIDs.BarFullStackedSeries2D, "Full-Stacked Bar 2D");
			AddString(ChartDesignerStringIDs.BarSideBySideStackedSeries2D, "Side-by-Side Stacked Bar 2D");
			AddString(ChartDesignerStringIDs.BarSideBySideFullStackedSeries2D, "Side-By-Side Full Stacked Bar 2D");
			AddString(ChartDesignerStringIDs.RangeBarOverlappedSeries2D, "Overlapped Range Bar 2D");
			AddString(ChartDesignerStringIDs.RangeBarSideBySideSeries2D, "Side-By-Side Range Bar 2D");
			AddString(ChartDesignerStringIDs.CandleStickSeries2D, "Candle Stick 2D");
			AddString(ChartDesignerStringIDs.StockSeries2D, "Stock 2D");
			AddString(ChartDesignerStringIDs.AreaSeries2D, "Area 2D");
			AddString(ChartDesignerStringIDs.AreaStepSeries2D, "Step Area 2D");
			AddString(ChartDesignerStringIDs.AreaStackedSeries2D, "Stacked Series 2D");
			AddString(ChartDesignerStringIDs.AreaFullStackedSeries2D, "Full-Stacked Series 2D");
			AddString(ChartDesignerStringIDs.LineSeries2D, "Line 2D");
			AddString(ChartDesignerStringIDs.SplineSeries2D, "Spline 2D");
			AddString(ChartDesignerStringIDs.SplineAreaSeries2D, "Spline Area 2D");
			AddString(ChartDesignerStringIDs.SplineAreaStackedSeries2D, "Spline Area Stacked 2D");
			AddString(ChartDesignerStringIDs.SplineAreaFullStackedSeries2D, "Spline Area Full Stacked 2D");
			AddString(ChartDesignerStringIDs.LineScatterSeries2D, "Scatter Line 2D");
			AddString(ChartDesignerStringIDs.LineStackedSeries2D, "Stacked Line 2D");
			AddString(ChartDesignerStringIDs.LineStepSeries2D, "Step Line 2D");
			AddString(ChartDesignerStringIDs.LineFullStackedSeries2D, "Full-Stacked Line 2D");
			AddString(ChartDesignerStringIDs.NestedDonutSeries2D, "Nested Donut 2D");
			AddString(ChartDesignerStringIDs.FunnelSeries2D, "Funnel 2D");
			AddString(ChartDesignerStringIDs.BubbleSeries2D, "Bubble 2D");
			AddString(ChartDesignerStringIDs.PointSeries2D, "Point 2D");
			AddString(ChartDesignerStringIDs.AreaSeries3D, "Area 3D");
			AddString(ChartDesignerStringIDs.BarSeries3D, "Bar 3D (Manhattan Bar)");
			AddString(ChartDesignerStringIDs.AreaStackedSeries3D, "Stacked Area 3D");
			AddString(ChartDesignerStringIDs.AreaFullStackedSeries3D, "Full-Stacked Area 3D");
			AddString(ChartDesignerStringIDs.BarSideBySideSeries3D, "Side-by-Side Bar 3D");
			AddString(ChartDesignerStringIDs.BubbleSeries3D, "Bubble 3D");
			AddString(ChartDesignerStringIDs.PointSeries3D, "Point 3D");
			#endregion
			#region Default Page Category
			#region Main Page
			AddString(ChartDesignerStringIDs.Default_MainPageTitle, "Chart");
			AddString(ChartDesignerStringIDs.Default_Main_AddSeriesManualyGroupTitle, "Add New Series");
			AddString(ChartDesignerStringIDs.Default_Main_ChangeChartTypeGroupTitle, "Change Chart Type");
			AddString(ChartDesignerStringIDs.Default_Main_Palette, "Palette");
			AddString(ChartDesignerStringIDs.Default_Main_XYDiagram2DPropertiesGrop, "Diagram Options");
			AddString(ChartDesignerStringIDs.Default_Main_XYDiagramRotated, "Rotated");
			AddString(ChartDesignerStringIDs.Default_Main_PaneOrientationHorizontal, "Horizontal");
			AddString(ChartDesignerStringIDs.Default_Main_PaneOrientationVertical, "Vertical");
			AddString(ChartDesignerStringIDs.Default_Main_PaneOrientationButtonCaption, "Panes Orientation");
			AddString(ChartDesignerStringIDs.Default_Main_EnableAxisXNavigationCheckCaption, "Axis X");
			AddString(ChartDesignerStringIDs.Default_Main_EnableAxisYNavigationCheckCaption, "Axis Y");
			AddString(ChartDesignerStringIDs.Default_Main_NavigationHeader, "Zoom & Scroll");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDimensionWithColon, "Dimension:");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDimensionWithoutColon, "Dimension");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirectionHorizontalCaption, "Horizontal");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirectionVerticalCaption, "Vertical");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramPropertiesGrop, "Diagram Options");
			AddString(ChartDesignerStringIDs.Default_Main_SimpleDiagramLayoutDirection, "Layout Direction");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramStartAngle, "Start Angle");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirectionClocwise, "Clockwise");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirectionConterclocwise, "Conterclockwise");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramRotationDirection, "Rotation Direction");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramOptionsGroup, "Diagram Options");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramShapeStyle, "Shape Style");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramCircleShapeStyleCaption, "Circle");
			AddString(ChartDesignerStringIDs.Default_Main_CircularDiagramPolygonShapeStyleCaption, "Polygon");
			AddString(ChartDesignerStringIDs.Default_Main_Diagram3DOptionsGroup, "Diagram Options");
			AddString(ChartDesignerStringIDs.Default_Main_Diagram3DZoomPercent, "Zoom Percent:");
			AddString(ChartDesignerStringIDs.Default_Main_Diagram3DPercpectiveAngle, "Perspective Angle:");
			#endregion
			#region Elements Page
			AddString(ChartDesignerStringIDs.Default_ElementsPageTitle, "Elements");
			AddString(ChartDesignerStringIDs.Default_Elements_ChartAreaElementsGroupTitle, "Chart Area Elements");
			AddString(ChartDesignerStringIDs.Default_Elements_AxesGroupTitle, "Axes");
			AddString(ChartDesignerStringIDs.Default_Elements_AxesElementsGroupTitle, "Axes Elements");
			AddString(ChartDesignerStringIDs.Default_Elements_Legend, "Legend");
			AddString(ChartDesignerStringIDs.Default_Elements_AddPaneVertical, "Add Pane Below");
			AddString(ChartDesignerStringIDs.Default_Elements_AddPaneHorizontal, "Add Pane At Right");
			AddString(ChartDesignerStringIDs.Default_Elements_AddConstantLine, "Add Constant Line");
			AddString(ChartDesignerStringIDs.Default_Elements_AddStripX, "Add Strip To Axis X");
			AddString(ChartDesignerStringIDs.Default_Elements_AddStripY, "Add Strip To Axis Y");
			AddString(ChartDesignerStringIDs.Default_Elements_AddChartTitle, "Add Chart Title");
			AddString(ChartDesignerStringIDs.Default_Elements_AddSecondaryAxisX, "Add Axis X");
			AddString(ChartDesignerStringIDs.Default_Elements_AddSecondaryAxisY, "Add Axis Y");
			AddString(ChartDesignerStringIDs.Default_Elements_AddStripYDescription, "Add a strip to the primary Y-axis.");
			AddString(ChartDesignerStringIDs.Default_Elements_AddStripXDescription, "Add a strip to the primary X-axis.");
			AddString(ChartDesignerStringIDs.Default_Elements_AddConstantLineY, "Add Constant Line To Axis Y");
			AddString(ChartDesignerStringIDs.Default_Elements_AddConstntLineYDescription, "Add a constant line to the primary Y-axis.");
			AddString(ChartDesignerStringIDs.Default_Elements_AddConstantLineX, "Add Constant Line To Axis X");
			AddString(ChartDesignerStringIDs.Default_Elements_AddConstantLineXDescription, "Add a constant line to the primary X-axis.");
			AddString(ChartDesignerStringIDs.Default_Elements_AddPaneHorizontalDescription, "Add a new pane to the right of the current pane and stack them horizontally.");
			AddString(ChartDesignerStringIDs.Default_Elements_AddPaneVerticalDescription, "Add a new pane below the current pane and stack them vertically.");
			AddString(ChartDesignerStringIDs.Default_Elements_IndicatorsGroup, "Indicators");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator, "Add Indicator");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_SimpleIndicatorsGalleryGroup, "Simple");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_FibonacciIndicatorsGalleryGroup, "Fibonacci");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_MovingAverageIndicatorsGalleryGroup, "Moving Average");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_OscillatorsGalleryGroup, "Oscillator Indicators");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_TrendIndicatorsGalleryGroup, "Trend Indicators");
			AddString(ChartDesignerStringIDs.Default_Elements_AddIndicator_PriceIndicatorsGalleryGroup, "Price Indicators");
			#endregion
			#endregion
			#region Series Options Category
			AddString(ChartDesignerStringIDs.SeriesOptionsRibbonCategoryTitle, "Series Options");
			AddString(ChartDesignerStringIDs.SeriesOptions_SelectedSeries, "Series");
			AddString(ChartDesignerStringIDs.SeriesOptions_DataPage, "Data");
			AddString(ChartDesignerStringIDs.SeriesOptions_DataSourcePageGroup, "Data Source");
			AddString(ChartDesignerStringIDs.SeriesOptions_DataMembersPageGroup, "Data Members");
			AddString(ChartDesignerStringIDs.SeriesOptions_ChartDataSource, "Chart Data Source");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesDataSource, "Series Data Source");
			AddString(ChartDesignerStringIDs.SeriesOptions_DataSourceNone, "No Data Source");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesDataMember, "Series:");
			AddString(ChartDesignerStringIDs.SeriesOptions_ArgumentDataMember, "Argument:");
			AddString(ChartDesignerStringIDs.SeriesOptions_ValueDataMember, "Value:");
			AddString(ChartDesignerStringIDs.SeriesOptions_Value2DataMember, "Value 2:");
			AddString(ChartDesignerStringIDs.SeriesOptions_WeightDataMember, "Weight:");
			AddString(ChartDesignerStringIDs.SeriesOptions_LowValueDataMember, "Low Value:");
			AddString(ChartDesignerStringIDs.SeriesOptions_HighValueDataMember, "High Value:");
			AddString(ChartDesignerStringIDs.SeriesOptions_OpenValueDataMember, "Open Value:");
			AddString(ChartDesignerStringIDs.SeriesOptions_CloseValueDataMember, "Close Value:");
			AddString(ChartDesignerStringIDs.SeriesOptions_ColorDataMember, "Color:");
			AddString(ChartDesignerStringIDs.SeriesOptions_SelectedSeries_View, "View");
			AddString(ChartDesignerStringIDs.SeriesOptions_Main, "Main");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesName, "Name:");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesVisibility, "Visibility");
			AddString(ChartDesignerStringIDs.SeriesOptions_LabelsVisibility, "Labels Visibility");
			AddString(ChartDesignerStringIDs.SeriesOptions_MarkersCaption, "Appearance");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesLabelsPosition, "Labels");
			AddString(ChartDesignerStringIDs.SeriesOptions_ConnectorThickness, "Connector:");
			AddString(ChartDesignerStringIDs.SeriesOptions_Indent, "Indent:");
			AddString(ChartDesignerStringIDs.SeriesOptions_LayoutGroup, "Layout");
			AddString(ChartDesignerStringIDs.SeriesOptions_NoModelString, "None");
			AddString(ChartDesignerStringIDs.SeriesOptions_Model, "Model");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_225DegreesDescription, "Show data labels rotated at 225 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_225DegreesCaption, "225 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_180DegreesDescription, "Show data labels rotated at 180 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_180DegreesCaption, "180 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_135DegreesDescription, "Show data labels rotated at 135 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_135DegreesCaption, "135 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_90DegreesDescription, "Show data labels rotated at 90 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_90DegreesCaption, "90 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_45DegreesDescription, "Show data labels rotated at 45 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_45DegreesCaption, "45 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_0DegreesCaption, "0 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_0DegreesDescription, "Show data labels without rotation.");
			AddString(ChartDesignerStringIDs.SeriesOptions_ArgumentAxis, "Argument Axis:");
			AddString(ChartDesignerStringIDs.SeriesOptions_ValueAxis, "Value Axis:");
			AddString(ChartDesignerStringIDs.SeriesOptions_SeriesPane, "Pane:");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_StackedBarCenteredInBarDescription, "Show data labels in the center of bars.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_StackedBarCenteredInBarCaption, "Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_3DTopDescription, "Show data labels on the top of point markers.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_3DTopCaption, "Top");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Marker3DCenterDescription, "Show data labels in the center of point markers.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Marker3DCenterCaption, "Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Bubble2DCenterDescription, "Show data labels in the center of bubbles.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Bubble2DCenterCaption, "Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_TwoColumnsDescription, "Show data labels in two columns.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_TwoColumnsCaption, "Two Columns");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideSlicesDescription, "Show data labels outside pie slices.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideSlicesCaption, "Outside");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_InsideSlicesDescription, "Show data labels inside pie slices.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_InsideSlicesCaption, "Inside");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_CenteredInBarsDescription, "Show data labels in the center of bars.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_CenteredInBarsCaption, "Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideDescription, "Show data labels outside bars.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideCaption, "Outside");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_315DegreesDescription, "Show data labels rotated at 315 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_315DegreesCaption, "315 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_270DegreesDescription, "Show data labels rotated at 270 degrees.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_270DegreesCaption, "270 Degrees");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_EnabledDescription, "Turn on data labels.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_EnabledCaption, "Enabled");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_NoneDescription, "Turn off data labels.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_NoneCaption, "None");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Bar3DEnabledDescription, "Turn on data labels.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Bar3DEnabledCaption, "Enabled");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Area3DEnabledDescription, "Turn on data labels.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_Area3DEnabledCaption, "Enabled");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelCenterPointCaption, "Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftPointCaption, "Left");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftColumnPointCaption, "Left Column");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightPointCaption, "Right");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightColumnPointCaption, "Right Column");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelCenterPointDescription, "Show data labels in the center of funnel points.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftPointDescription, "Show data labels on the left of funnel points.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftColumnPointDescription, "The labels are organized into a straight column on the left of the funnel series points.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightPointDescription, "Show data labels on the right of funnel points.");
			AddString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightColumnPointDescription, "The labels are organized into a straight column on the right of the funnel series points.");
			AddString(ChartDesignerStringIDs.SeriesOptions_LabelsGroupTitle, "Labels");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMaxValueLabelPositionCaption, "Max");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMaxValueLabelPositionDescription, "Show only the label for the maximum value of a data point.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMinValueLabelPositionCaption, "Min");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMinValueLabelPositionDescription, "Show only the label for the minimum value of a data point.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DOneLabelPositionCaption, "Center ");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DOneLabelPositionDescription, "Show one label in the center of the range area with both minimum and maximum values.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DTwoLabelsPositionCaption, "Both ");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DTwoLabelsPositionDescription, "Show both labels with minimum and maximum values of a data point.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue1LabelPositionCaption, "Value");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue1LabelPositionDescription, "Show only the label for the first value of a data point.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue2LabelPositionCaption, "Value 2");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue2LabelPositionDescription, "Show only the label for the second value of a data point.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarMaxValueLabelPositionCaption, "Max");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarMaxValueLabelPositionDescription, "Show only the label for the maximum range bar value.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarMinValueLabelPositionCaption, "Min");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarMinValueLabelPositionDescription, "Show only the label for the minimum range bar value.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarOneLabelPositionCaption, "Center ");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarOneLabelPositionDescription, "Show one label in the center of the range bar with both minimum and maximum values.");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarTwoLabelsLabelPositionCaption, "Both");
			AddString(ChartDesignerStringIDs.SeriesOptions_RangeBarTwoLabelsLabelPositionDescription, "Show both labels with minimum and maximum values.");
			AddString(ChartDesignerStringIDs.SeriesOptions_AddIndicatorsGroupTitle, "Add Indicator");
			AddString(ChartDesignerStringIDs.SeriesOptions_HoleRadiusPercent, "Hole percent:");
			AddString(ChartDesignerStringIDs.SeriesOptions_PieDoughnutOptionsGroup, "Pie/Doughnut");
			AddString(ChartDesignerStringIDs.SeriesOptions_FunnelPointDistance, "Point Distance:");
			AddString(ChartDesignerStringIDs.SeriesOptions_FunnelOptionsGroup, "Funnel");
			AddString(ChartDesignerStringIDs.SeriesOptions_FunnelAlignToCenter, "Align to Center");
			AddString(ChartDesignerStringIDs.SeriesOptions_FunnelRatioAuto, "Height / Width Auto");
			AddString(ChartDesignerStringIDs.SeriesOptions_FunnelRatio, "Height / Width:");
			AddString(ChartDesignerStringIDs.SeriesOptions_NestedDonut2DOptionsGroup, "Nested Donut");
			AddString(ChartDesignerStringIDs.SeriesOptions_NestedDonutGroup, "Group");
			AddString(ChartDesignerStringIDs.SeriesOptions_NestedDonutInnerIndent, "Inner Indent");
			AddString(ChartDesignerStringIDs.SeriesOptions_NestedDonutWeight, "Weight");
			#endregion
			#region Axis Options Category
			AddString(ChartDesignerStringIDs.AxisCategoryTitle, "Axis Options");
			AddString(ChartDesignerStringIDs.AxisPageCategoryTitle, "Axis");
			AddString(ChartDesignerStringIDs.AxisOptions_GeneralGroup, "General");
			AddString(ChartDesignerStringIDs.AxisOptions_TitleGroup, "Axis Title");
			AddString(ChartDesignerStringIDs.AxisOptions_AppearanceGroup, "Appearance");
			AddString(ChartDesignerStringIDs.AxisOptions_ElementsGroup, "Elements");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind, "Position");
			AddString(ChartDesignerStringIDs.AxisOptions_Interlaced, "Interlaced");
			AddString(ChartDesignerStringIDs.AxisOptions_TitleContent, "Text:");
			AddString(ChartDesignerStringIDs.AxisOptions_TitlePosition, "Title");
			AddString(ChartDesignerStringIDs.AxisOptions_LabelsOrientation, "Labels");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisAlignment, "Alignment");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisReverse, "Reverse");
			AddString(ChartDesignerStringIDs.AxisOptions_TitleVisibility, "Title Visibility");
			AddString(ChartDesignerStringIDs.AxisOptions_MajorVisible, "Major");
			AddString(ChartDesignerStringIDs.AxisOptions_MinorVisible, "Minor");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKindNoneCaption, "None");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKindNoneDescription, "Do not display the axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_BottomCaption, "Show At Bottom");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_BottomDescription, "Show the axis at the bottom of a diagram.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_TopCaption, "Show On Top");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_TopDescription, "Show the axis on the top of a diagram.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_LeftCaption, "Show On Left");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_LeftDescription, "Show the axis on the left of a diagram.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_RightCaption, "Show On Right");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_RightDescription, "Show the axis on the right of a diagram.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_CircularAxisYVisibleCaption, "Show Axis");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisKind_CircularAxisYVisibleDescription, "Show the Axis Y.");
			AddString(ChartDesignerStringIDs.AxisOptions_TitlePosition_NoneCaption, "None");
			AddString(ChartDesignerStringIDs.AxisOptions_TitlePosition_NoneDescription, "Turn off the axis title.");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_LeftCaption, "Left");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_LeftDescription, "Show the title on the left of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_CenterCaption, "Center");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_CenterDescription, "Show the title in the center of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_RightCaption, "Right");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_RightDescription, "Show the title on the right of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_BottomCaption, "Bottom");
			AddString(ChartDesignerStringIDs.AxisOptions_XTitlePosition_BottomDescription, "Show the title at the bottom of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_CenterCaption, "Center");
			AddString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_CenterDescription, "Show the title in the center of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_TopCaption, "Top");
			AddString(ChartDesignerStringIDs.AxisOptions_YTitlePosition_TopDescription, "Show the title on the top of an axis.");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisLabelNoneCaption, "None");
			AddString(ChartDesignerStringIDs.AxisOptions_AxisLabelNoneDescription, "Turn off axis labels.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_NormalCaption, "Show Labels");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_NormalDescription, "Show axis labels.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_StaggeredCaption, "Staggered");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_StaggeredDescription, "Show axis labels in staggered order.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated90Caption, "90 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated90Description, "Show axis labels rotated at 90 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus90Caption, "-90 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus90Description, "Show axis labels rotated at -90 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated45Caption, "45 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_Rotated45Description, "Show axis labels rotated at 45 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus45Caption, "-45 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_XLabelOrientation_RotatedMinus45Description, "Show axis labels rotated at -45 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_NormalCaption, "Show Labels");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_DescriptionCaption, "Show axis labels.");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_Rotated90Caption, "90 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_Rotated90Description, "Show axis labels rotated at 90 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_RotatedMinus90Caption, "-90 Degrees");
			AddString(ChartDesignerStringIDs.AxisOptions_YLabelOrientation_RotatedMinus90Description, "Show axis labels rotated at -90 degrees.");
			AddString(ChartDesignerStringIDs.AxisOptions_CircularXLabelOrientation_NormalCaption, "Show Labels");
			AddString(ChartDesignerStringIDs.AxisOptions_CircularXLabelOrientation_NormalDescription, "Show axis labels.");
			AddString(ChartDesignerStringIDs.AxisOptions_CircularYLabelOrientation_NormalCaption, "Show Labels");
			AddString(ChartDesignerStringIDs.AxisOptions_CircularYLabelOrientation_NormalDescription, "Show axis labels.");
			AddString(ChartDesignerStringIDs.AxisOptions_RangeGroup, "Range");
			AddString(ChartDesignerStringIDs.AxisOptions_WholeRangeMaxValue, "Max:");
			AddString(ChartDesignerStringIDs.AxisOptions_WholeRangeMinValue, "Min:");
			AddString(ChartDesignerStringIDs.AxisOptions_GridlinesStaticText, "Grid Lines");
			AddString(ChartDesignerStringIDs.AxisOptions_TickMarksStaticText, "Thickmarks");
			AddString(ChartDesignerStringIDs.AxisOptions_WholeRangeHeader, "Whole Range");
			AddString(ChartDesignerStringIDs.AxisOptions_VisibleRangeHeader, "Visible Range");
			AddString(ChartDesignerStringIDs.AxisOptions_VisibleRangeMaxValue, "Max:");
			AddString(ChartDesignerStringIDs.AxisOptions_VisibleRangeMinValue, "Min:");
			AddString(ChartDesignerStringIDs.AxisOptions_DateTimeOptionsGroup, "Date&Time Options");
			AddString(ChartDesignerStringIDs.AxisOptions_DateTimeMeasureUnit, "Measure Unit:");
			AddString(ChartDesignerStringIDs.AxisOptions_DateTimeGridAlignment, "Grid Alignment:");
			AddString(ChartDesignerStringIDs.AxisOptions_DateTimeFormat, "Format:");
			#endregion
			#region Legend Options Category
			AddString(ChartDesignerStringIDs.LegendOptionsRibbonCategoryTitle, "Legend Options");
			AddString(ChartDesignerStringIDs.LegendOptions_SelectedLegend, "Legend");
			AddString(ChartDesignerStringIDs.LegendPosition, "Position");
			AddString(ChartDesignerStringIDs.LegendOptions_Layout, "Layout");
			AddString(ChartDesignerStringIDs.LegendOptions_ReverseItems, "Reverse Items");
			AddString(ChartDesignerStringIDs.LegendOptions_Orientation, "Orientation");
			AddString(ChartDesignerStringIDs.LegendOptions_Orientation_Horizontal, "Horizontal");
			AddString(ChartDesignerStringIDs.LegendOptions_Orientation_HorizontalDescription, "Order legend items from left to right.");
			AddString(ChartDesignerStringIDs.LegendOptions_Orientation_Vertical, "Vertical");
			AddString(ChartDesignerStringIDs.LegendOptions_Orientation_VerticalDescription, "Order legend items from top to bottom.");
			#endregion
			#region Constant Line Category
			AddString(ChartDesignerStringIDs.ConstantLineCategoryTitle, "Constant Line Options");
			AddString(ChartDesignerStringIDs.ConstantLinePageCategoryTitle, "Constant Line");
			AddString(ChartDesignerStringIDs.ConstantLine_GeneralGroupTitle, "General");
			AddString(ChartDesignerStringIDs.ConstantLine_TitleGroupTitle, "Title");
			AddString(ChartDesignerStringIDs.ConstantLine_Value, "Value:");
			AddString(ChartDesignerStringIDs.ConstantLine_LineColor, "Color");
			AddString(ChartDesignerStringIDs.ConstantLine_Thickness, "Thickness:");
			AddString(ChartDesignerStringIDs.ConstantLine_TitleText, "Text:");
			AddString(ChartDesignerStringIDs.ConstantLine_TitleForeground, "Color");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePosition, "Title");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_None, "None");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_None, "Turn off the title.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearAboveVertical, "Left Above");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearAboveVertical, "Show the title above the line on the left.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarAboveVertical, "Right Above");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarAboveVertical, "Show the title above the line on the right.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearBelowVertical, "Left Below");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearBelowVertical, "Show the title below the line on the left.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarBelowVertical, "Right Below");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarBelowVertical, "Show the title below the line on the right.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearAboveHorizontal, "Left Above");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearAboveHorizontal, "Show the title above the line on the left.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarAboveHorizontal, "Right Above");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarAboveHorizontal, "Show the title above the line on the right.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_NearBelowHorizontal, "Left Below");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_NearBelowHorizontal, "Show the title below the line on the left.");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositionCaption_FarBelowHorizontal, "Right Below");
			AddString(ChartDesignerStringIDs.ConstantLine_TitlePositonDescription_FarBelowHorizontal, "Show the title below the line on the right.");
			AddString(ChartDesignerStringIDs.ConstantLine_LegendText, "Text In Legend:");
			#endregion
			#region Strip Category
			AddString(ChartDesignerStringIDs.Strip_CategoryCaption, "Strip Options");
			AddString(ChartDesignerStringIDs.Strip_PageCaption, "Strip");
			AddString(ChartDesignerStringIDs.Strip_MinLimit, "Min Limit:");
			AddString(ChartDesignerStringIDs.Strip_MaxLimit, "Max Limit:");
			AddString(ChartDesignerStringIDs.Strip_AxisLabelText, "Text On Axis:");
			AddString(ChartDesignerStringIDs.Strip_LegendText, "Text In Legend:");
			AddString(ChartDesignerStringIDs.Strip_Brush, "Color");
			AddString(ChartDesignerStringIDs.Strip_GeneralGroupCaption, "General");
			AddString(ChartDesignerStringIDs.Strip_TextGroupCaption, "Text");
			#endregion
			#region Tree
			AddString(ChartDesignerStringIDs.TreeChart, "Chart");
			AddString(ChartDesignerStringIDs.TreeTitleCollection, "Title Collection");
			AddString(ChartDesignerStringIDs.TreeLegend, "Legend");
			AddString(ChartDesignerStringIDs.TreeDiagramPolarDiagram2D, "Polar Diagram");
			AddString(ChartDesignerStringIDs.TreeDiagramRadarDiagram2D, "Radar Diagram");
			AddString(ChartDesignerStringIDs.TreeDiagramSimpleDiagram2D, "Simple Diagram");
			AddString(ChartDesignerStringIDs.TreeDiagramXYDiagram2D, "Cartesian Diagram");
			AddString(ChartDesignerStringIDs.TreeDiagramSimpleDiagram3D, "3D Simple Diagram");
			AddString(ChartDesignerStringIDs.TreeDiagramXYDiagram3D, "3D Cartesian Diagram");
			AddString(ChartDesignerStringIDs.TreeAxesSubnode, "Axes");
			AddString(ChartDesignerStringIDs.TreePrimaryAxisX, "Primary Axis X");
			AddString(ChartDesignerStringIDs.TreePrimaryAxisY, "Primary Axis Y");
			AddString(ChartDesignerStringIDs.TreeSecondaryAxesCollectionX, "Secondary Axes X");
			AddString(ChartDesignerStringIDs.TreeSecondaryAxesCollectionY, "Secondary Axes Y");
			AddString(ChartDesignerStringIDs.TreeSeriesCollection, "Series Collection");
			AddString(ChartDesignerStringIDs.TreeConstantLineCollection, "Constant Lines");
			AddString(ChartDesignerStringIDs.TreePanesSubnode, "Panes");
			AddString(ChartDesignerStringIDs.TreeAdditionalPanelCollection, "Additional Panes");
			AddString(ChartDesignerStringIDs.DefaultPane, "Default Pane");
			AddString(ChartDesignerStringIDs.TreeIndicatorCollection, "Indicators");
			AddString(ChartDesignerStringIDs.TreeAxisCustomLabelCollection, "Custom Labels");
			AddString(ChartDesignerStringIDs.TreeStripModel, "Strip");
			AddString(ChartDesignerStringIDs.TreeStripCollectionModel, "Strips");
			AddString(ChartDesignerStringIDs.ComplexTitleContent, "Complex Content");
			AddString(ChartDesignerStringIDs.SeriesTemplate, "Template");
			#endregion
			#region Chart Titles
			AddString(ChartDesignerStringIDs.ChartTitleOptionsRibbonCategoryTitle, "Title Options");
			AddString(ChartDesignerStringIDs.ChartTitleOptions_SelectedTitle, "Title");
			AddString(ChartDesignerStringIDs.ChartTitleOptions_Position, "Position");
			AddString(ChartDesignerStringIDs.ChartTitleOptions_Text, "Text:");
			AddString(ChartDesignerStringIDs.ChartTitleOptions_General, "General");
			#endregion
			#region Command Messages
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeConstantLineValueCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeConstantLineValueMessage, "The entered value has an incorrect type. Must be DateTime");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericConstantLineValueCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericConstantLineValueMessage, "The entered value has an incorrect type. Must be Numerical");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMinLimitCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMinLimitMessage, "The entered value has an incorrect type. Must be DateTime");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMinLimitCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMinLimitMessage, "The entered value has an incorrect type. Must be Numerical");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMaxLimitCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithDateTimeStripMaxLimitMessage, "The entered value has an incorrect type. Must be DateTime");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMaxLimitCaption, "Incorrect Value Format");
			AddString(ChartDesignerStringIDs.CommandMessages_IncompatibleWithNumericalStripMaxLimitMessage, "The entered value has an incorrect type. Must be Numerical");
			#endregion
			#region PropertyGrid
			AddString(ChartDesignerStringIDs.Marker2DModelCircle, "Circle");
			AddString(ChartDesignerStringIDs.Marker2DModelCross, "Cross");
			AddString(ChartDesignerStringIDs.Marker2DModelDollar, "Dollar");
			AddString(ChartDesignerStringIDs.Marker2DModelPolygon, "Polygon");
			AddString(ChartDesignerStringIDs.Marker2DModelRing, "Ring");
			AddString(ChartDesignerStringIDs.Marker2DModelSquare, "Square");
			AddString(ChartDesignerStringIDs.Marker2DModelStar, "Star");
			AddString(ChartDesignerStringIDs.Marker2DModelTriangle, "Triangle");
			AddString(ChartDesignerStringIDs.Marker2DAnimationWiden, "Widen");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeft, "Slide From Left");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRight, "Slide From Right");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromTop, "Slide From Top");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromBottom, "Slide From Bottom");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftCenter, "Slide From Left Center");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightCenter, "Slide From Right Center");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromTopCenter, "Slide From Top Center");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromBottomCenter, "Slide From Bottom Center");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftTopCorner, "Slide From Left Top Corner");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightTopCorner, "Slide From Right Top Corner");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromRightBottomCorner, "Slide From Right Bottom Corner");
			AddString(ChartDesignerStringIDs.Marker2DAnimationSlideFromLeftBottomCorner, "Slide From Left Bottom Corner");
			AddString(ChartDesignerStringIDs.Marker2DAnimationFadeIn, "Fade In");
			AddString(ChartDesignerStringIDs.Bar2DGrowUpAnimation, "Grow Up");
			AddString(ChartDesignerStringIDs.Bar2DDropInAnimation, "Drop In");
			AddString(ChartDesignerStringIDs.Bar2DBounceAnimation, "Bounce");
			AddString(ChartDesignerStringIDs.Bar2DSlideFromLeftAnimation, "Slide From Left");
			AddString(ChartDesignerStringIDs.Bar2DSlideFromRightAnimation, "Slide From Right");
			AddString(ChartDesignerStringIDs.Bar2DSlideFromTopAnimation, "Slide From Top");
			AddString(ChartDesignerStringIDs.Bar2DSlideFromBottomAnimation, "Slide From Bottom");
			AddString(ChartDesignerStringIDs.Bar2DWidenAnimation, "Widen");
			AddString(ChartDesignerStringIDs.Bar2DFadeInAnimation, "Fade In");
			AddString(ChartDesignerStringIDs.Area2DGrowUpAnimation, "Grow Up");
			AddString(ChartDesignerStringIDs.Area2DStretchFromNearAnimation, "Stretch From Near");
			AddString(ChartDesignerStringIDs.Area2DStretchFromFarAnimation, "Stretch From Far");
			AddString(ChartDesignerStringIDs.Area2DStretchOutAnimation, "Stretch Out");
			AddString(ChartDesignerStringIDs.Area2DDropFromNearAnimation, "Drop From Near");
			AddString(ChartDesignerStringIDs.Area2DDropFromFarAnimation, "Drop From Far");
			AddString(ChartDesignerStringIDs.Area2DUnwindAnimation, "Unwind");
			AddString(ChartDesignerStringIDs.Line2DSlideFromLeftAnimation, "Slide From Left");
			AddString(ChartDesignerStringIDs.Line2DSlideFromRightAnimation, "Slide From Right");
			AddString(ChartDesignerStringIDs.Line2DSlideFromTopAnimation, "Slide From Top");
			AddString(ChartDesignerStringIDs.Line2DSlideFromBottomAnimation, "Slide From Bottom");
			AddString(ChartDesignerStringIDs.Line2DUnwrapVerticallyAnimation, "Unwrap Vertically");
			AddString(ChartDesignerStringIDs.Line2DUnwrapHorizontallyAnimation, "Unwrap Horizontally");
			AddString(ChartDesignerStringIDs.Line2DBlowUpAnimation, "Blow Up");
			AddString(ChartDesignerStringIDs.Line2DStretchFromNearAnimation, "Stretch From Near");
			AddString(ChartDesignerStringIDs.Line2DStretchFromFarAnimation, "Stretch From Far");
			AddString(ChartDesignerStringIDs.Line2DUnwindAnimation, "Unwind");
			AddString(ChartDesignerStringIDs.AreaStacked2DFadeInAnimation, "Fade In");
			AddString(ChartDesignerStringIDs.CircularMarkerWidenAnimation, "Widen");
			AddString(ChartDesignerStringIDs.CircularMarkerFadeInAnimation, "Fade In");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideFromLeftCenterAnimation, "Slide From Left Center");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideFromRightCenterAnimation, "Slide From Right Center");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideFromTopCenterAnimation, "Slide From Top Center");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideFromBottomCenterAnimation, "Slide From Bottom Center");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideFromCenterAnimation, "Slide From Center");
			AddString(ChartDesignerStringIDs.CircularMarkerSlideToCenterAnimation, "Slide To Center");
			AddString(ChartDesignerStringIDs.CircularAreaZoomInAnimation, "Zoom In");
			AddString(ChartDesignerStringIDs.CircularAreaSpinAnimation, "Spin");
			AddString(ChartDesignerStringIDs.CircularAreaSpinZoomInAnimation, "Spin Zoom In");
			AddString(ChartDesignerStringIDs.CircularAreaUnwindAnimation, "Unwind");
			AddString(ChartDesignerStringIDs.CircularLineZoomInAnimation, "Zoom In");
			AddString(ChartDesignerStringIDs.CircularLineSpinAnimation, "Spin");
			AddString(ChartDesignerStringIDs.CircularLineSpinZoomInAnimation, "Spin Zoom In");
			AddString(ChartDesignerStringIDs.CircularLineUnwindAnimation, "Unwind");
			AddString(ChartDesignerStringIDs.Stock2DSlideFromLeftAnimation, "Slide From Left");
			AddString(ChartDesignerStringIDs.Stock2DSlideFromRightAnimation, "Slide From Right");
			AddString(ChartDesignerStringIDs.Stock2DSlideFromTopAnimation, "Slide From Top");
			AddString(ChartDesignerStringIDs.Stock2DSlideFromBottomAnimation, "Slide From Bottom");
			AddString(ChartDesignerStringIDs.Stock2DExpandAnimation, "Expand");
			AddString(ChartDesignerStringIDs.Stock2DFadeInAnimation, "Fade In");
			AddString(ChartDesignerStringIDs.Pie2DGrowUpAnimation, "Grow Up");
			AddString(ChartDesignerStringIDs.Pie2DPopUpAnimation, "Pop Up");
			AddString(ChartDesignerStringIDs.Pie2DDropInAnimation, "Drop In");
			AddString(ChartDesignerStringIDs.Pie2DWidenAnimation, "Widen");
			AddString(ChartDesignerStringIDs.Pie2DFlyInAnimation, "Fly In");
			AddString(ChartDesignerStringIDs.Pie2DBurstAnimation, "Burst");
			AddString(ChartDesignerStringIDs.Pie2DFadeInAnimation, "Fade In");
			AddString(ChartDesignerStringIDs.Pie2DZoomInAnimation, "Zoom In");
			AddString(ChartDesignerStringIDs.Pie2DFanAnimation, "Fan");
			AddString(ChartDesignerStringIDs.Pie2DFanZoomInAnimation, "Fan Zoom In");
			AddString(ChartDesignerStringIDs.Pie2DSpinAnimation, "Spin");
			AddString(ChartDesignerStringIDs.Pie2DSpinZoomInAnimation, "Spin Zoom In");
			AddString(ChartDesignerStringIDs.DashStyle_Dash, "Dash");
			AddString(ChartDesignerStringIDs.DashStyle_DashDot, "Dash Dot");
			AddString(ChartDesignerStringIDs.DashStyle_DashDotDot, "Dash Dot Dot");
			AddString(ChartDesignerStringIDs.DashStyle_Dot, "Dot");
			AddString(ChartDesignerStringIDs.DashStyle_Solid, "Solid");
			AddString(ChartDesignerStringIDs.NewAxis, "New Axis");
			AddString(ChartDesignerStringIDs.NewPane, "New Pane");
			AddString(ChartDesignerStringIDs.NewSeriesPoint, "New SeriesPoint");
			AddString(ChartDesignerStringIDs.NewSeriesLabel, "New Series Label");
			AddString(ChartDesignerStringIDs.NewAxisLabel, "New Axis Label");
			AddString(ChartDesignerStringIDs.NewAxisRange, "New Axis Range");
			AddString(ChartDesignerStringIDs.NewNavigationOptions, "New Navigation Options");
			AddString(ChartDesignerStringIDs.NewNumericOptions, "New Numeric Options");
			AddString(ChartDesignerStringIDs.NewDateTimeOptions, "New Date Time Options");
			AddString(ChartDesignerStringIDs.NewLineStyle, "New Line Style");
			AddString(ChartDesignerStringIDs.NewTitle, "New Title");
			AddString(ChartDesignerStringIDs.ScaleOptionsAutomatic, "Automatic");
			AddString(ChartDesignerStringIDs.ScaleOptionsManual, "Manual");
			AddString(ChartDesignerStringIDs.ScaleOptionsContinuous, "Continuous");
			AddString(ChartDesignerStringIDs.NewResolveOverlappingOptions, "New Resolve Overlapping Options");
			AddString(ChartDesignerStringIDs.NewScrollBarOptions, "New Scroll Bar Options");
			AddString(ChartDesignerStringIDs.NewSeriesBorder, "New Series Border");
			AddString(ChartDesignerStringIDs.NewSimpleMovingAverage, "New Simple Moving Average");
			AddString(ChartDesignerStringIDs.NewWeightedMovingAverage, "New Weighted Moving Average");
			AddString(ChartDesignerStringIDs.NewExponentialMovingAverage, "New Exponential Moving Average");
			AddString(ChartDesignerStringIDs.NewTriangularMovingAverage, "New Triangular Moving Average");
			AddString(ChartDesignerStringIDs.NewRegressionLine, "New Regression Line");
			AddString(ChartDesignerStringIDs.NewTrendLine, "New Trend Line");
			AddString(ChartDesignerStringIDs.NewFibonacciRetracement, "New Fibonacci Retracement");
			AddString(ChartDesignerStringIDs.NewFibonacciFans, "New Fibonacci Fans");
			AddString(ChartDesignerStringIDs.NewFibonacciArcs, "New Fibonacci Arcs");
			AddString(ChartDesignerStringIDs.NewReductionStockOptions, "New Reduction Stock Options");
			AddString(ChartDesignerStringIDs.NewToolTipFreePosition, "New Tool Tip Free Position");
			AddString(ChartDesignerStringIDs.NewToolTipMousePosition, "New Tool Tip Mouse Position");
			AddString(ChartDesignerStringIDs.NewToolTipRelativePosition, "New Tool Tip Relative Position");
			AddString(ChartDesignerStringIDs.NewToolTipOptions, "New Tool Tip Options");
			AddString(ChartDesignerStringIDs.NewChartToolTipController, "New Tool Tip Controller");
			AddString(ChartDesignerStringIDs.NewCrosshairFreePosition, "New Crosshair Free Position");
			AddString(ChartDesignerStringIDs.NewCrosshairMousePosition, "New Crosshair Mouse Position");
			AddString(ChartDesignerStringIDs.NewCrosshairOptions, "New Crosshair Options");
			AddString(ChartDesignerStringIDs.NewCrosshairAxisLabelOptions, "New Crosshair Axis Label Options");
			AddString(ChartDesignerStringIDs.ColorObjectColorizer, "ColorObjectColorizer");
			AddString(ChartDesignerStringIDs.KeyColorColorizer, "KeyColorColorizer");
			AddString(ChartDesignerStringIDs.RangeColorizer, "RangeColorizer");
			AddString(ChartDesignerStringIDs.ColorizerKeyString, "Add String");
			AddString(ChartDesignerStringIDs.ColorizerKeyInt, "Add Integer");
			AddString(ChartDesignerStringIDs.ColorizerKeyDouble, "Add Double");
			#endregion
			#region Various
			AddString(ChartDesignerStringIDs.DefaultConstantLineTitle, "Constant Line");
			AddString(ChartDesignerStringIDs.DefaultStripLegendText, "Strip");
			AddString(ChartDesignerStringIDs.DefaultChartTitleContent, "Title");
			AddString(ChartDesignerStringIDs.PrimaryAxisXName, "Primary Axis X");
			AddString(ChartDesignerStringIDs.PrimaryAxisYName, "Primary Axis Y");
			AddString(ChartDesignerStringIDs.SecondaryAxisXPrefix, "Secondary Axis X #");
			AddString(ChartDesignerStringIDs.SecondaryAxisYPrefix, "Secondary Axis Y #");
			AddString(ChartDesignerStringIDs.PanePrefix, "Pane #");
			AddString(ChartDesignerStringIDs.Redo, "Redo");
			AddString(ChartDesignerStringIDs.Undo, "Undo");
			AddString(ChartDesignerStringIDs.ThicknessNone, "None");
			AddString(ChartDesignerStringIDs.Auto, "Auto");
			AddString(ChartDesignerStringIDs.DefaultSeriesNamePrefix, "Series");
			AddString(ChartDesignerStringIDs.DefaultAxisTitleText, "Axis");
			#endregion
			#region Indicator names
			AddString(ChartDesignerStringIDs.RegressionLine, "Regression Line");
			AddString(ChartDesignerStringIDs.TrendLine, "Trend Line");
			AddString(ChartDesignerStringIDs.FibonacciArcs, "Fibonacci Arcs");
			AddString(ChartDesignerStringIDs.FibonacciFans, "Fibonacci Fans");
			AddString(ChartDesignerStringIDs.FibonacciRetracement, "Fibonacci Retracement");
			AddString(ChartDesignerStringIDs.SimpleMovingAverage, "Simple Moving Average");
			AddString(ChartDesignerStringIDs.WeightedMovingAverage, "Weighted Moving Average");
			AddString(ChartDesignerStringIDs.ExponentialMovingAverage, "Exponential Moving Average");
			AddString(ChartDesignerStringIDs.TriangularMovingAverage, "Triangular Moving Average");
			AddString(ChartDesignerStringIDs.AverageTrueRange, "Average True Range");
			AddString(ChartDesignerStringIDs.ChaikinsVolatility, "Chaikin's Volatility");
			AddString(ChartDesignerStringIDs.CommodityChannelIndex, "Commodity Channel Index");
			AddString(ChartDesignerStringIDs.DetrendedPriceOscillator, "Detrended Price Oscillator");
			AddString(ChartDesignerStringIDs.MassIndex, "Mass Index");
			AddString(ChartDesignerStringIDs.MovingAverageConvergenceDivergence, "Moving Average Convergence/Divergence");
			AddString(ChartDesignerStringIDs.RateOfChange, "Rate of Change");
			AddString(ChartDesignerStringIDs.RelativeStrengthIndex, "Relative Strength Index");
			AddString(ChartDesignerStringIDs.StandardDeviation, "Standard Deviation");
			AddString(ChartDesignerStringIDs.TripleExponentialMovingAverageTrix, "Triple Exponential Moving Average (TriX)");
			AddString(ChartDesignerStringIDs.WilliamsR, "Williams %R");
			AddString(ChartDesignerStringIDs.BollingerBands, "Bollinger Bands");
			AddString(ChartDesignerStringIDs.MedianPrice, "Median Price");
			AddString(ChartDesignerStringIDs.TypicalPrice, "Typical Price");
			AddString(ChartDesignerStringIDs.WeightedClose, "Weighted Close");
			AddString(ChartDesignerStringIDs.TripleExponentialMovingAverageTema, "Triple Exponential Moving Average (TEMA)");
			#endregion
			#region Indicator Page Category
			AddString(ChartDesignerStringIDs.IndicatorCategoryTitle, "Indicator Options");
			AddString(ChartDesignerStringIDs.Indicator_IndicatorPageTitle, "Indicator");
			AddString(ChartDesignerStringIDs.Indicator_PresentationGroupCaption, "Presentation");
			AddString(ChartDesignerStringIDs.Indicator_Color, "Color");
			AddString(ChartDesignerStringIDs.Indicator_Thickness, "Thickness:");
			AddString(ChartDesignerStringIDs.Inicator_LegendText, "Text in Legend:");
			AddString(ChartDesignerStringIDs.Indicator_AdvancedGroupCaption, "Advanced");
			AddString(ChartDesignerStringIDs.Indicator_SeparatePaneGroupCaption, "Pane & Axis");
			AddString(ChartDesignerStringIDs.Indicator_VlaueLevel, "Value Level:");
			AddString(ChartDesignerStringIDs.Indicator_VlaueLevelForRegressionLine, "Value level");
			AddString(ChartDesignerStringIDs.Indicator_Argument1, "Argument:");
			AddString(ChartDesignerStringIDs.Indicator_Argument2, "Argument:");
			AddString(ChartDesignerStringIDs.Indicator_VlaueLevel1, "Value Level:");
			AddString(ChartDesignerStringIDs.Indicator_ValueLevel2, "Value Level:");
			AddString(ChartDesignerStringIDs.Indicator_Arg2ValLevel2Header, "End Point");
			AddString(ChartDesignerStringIDs.Indicator_Arg1ValLevel1Header, "Start Point");
			AddString(ChartDesignerStringIDs.Indicator_GeneralGroupCaption, "General");
			AddString(ChartDesignerStringIDs.Indicator_TrenlineExtrapolateToInfinity, "Extrapolate to Infinity");
			AddString(ChartDesignerStringIDs.Indicator_FibonacciIndicatorShowLevel23_6, "Show Level 23.6%");
			AddString(ChartDesignerStringIDs.Indicator_FibonacciIndicatorShowLevel76_4, "Show Level 76.4%");
			AddString(ChartDesignerStringIDs.Indicator_FibonacciRetracementShowAdditionalLevels, "Show Additional Levels");
			AddString(ChartDesignerStringIDs.Indicator_FibonacciFansShowLevel0, "Show Level 0%");
			AddString(ChartDesignerStringIDs.Indicator_FibonacciArcsShowLevel100, "Show Level 100%");
			AddString(ChartDesignerStringIDs.Indicator_MovingAverageEnvelopePercent, "Envelope Percent:");
			AddString(ChartDesignerStringIDs.Indicator_MovingAveragePointsCount, "Points Count:");
			AddString(ChartDesignerStringIDs.Indicator_MovingAverageKind, "Kind: ");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageAndEnvelope, "Moving Average nd Envelope");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverage, "Moving Average");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_Envelope, "Envelope");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_EnvelopeDescription, "Only the Envelope is shown.");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageDescription, "Only the Moving Average is shown.");
			AddString(ChartDesignerStringIDs.Indicators_MovingAverageKind_MovingAverageAndEnvelopeDescription, "Both the Moving Average and Envelope are shown.");
			AddString(ChartDesignerStringIDs.Indicator_Pane, "Pane");
			AddString(ChartDesignerStringIDs.Indicator_AxisY, "Axis Y");
			#endregion
			#region Series Data Editor
			AddString(ChartDesignerStringIDs.DataEditor_ArgumentColumnHeader, "Argument");
			AddString(ChartDesignerStringIDs.DataEditor_ValueColumnHeader, "Value");
			AddString(ChartDesignerStringIDs.DataEditor_OpenValueColumnHeader, "Open");
			AddString(ChartDesignerStringIDs.DataEditor_CloseValueColumnHeader, "Close");
			AddString(ChartDesignerStringIDs.DataEditor_LowValueColumnHeader, "Low");
			AddString(ChartDesignerStringIDs.DataEditor_Value2ColumnHeader, "Value 2");
			AddString(ChartDesignerStringIDs.DataEditor_WeightValueColumnHeader, "Weight");
			AddString(ChartDesignerStringIDs.DataEditor_HighValueColumnHeader, "High");
			AddString(ChartDesignerStringIDs.DataEditor_BrushColumnHeader, "Brush");
			#endregion
			#region Value Levels
			AddString(ChartDesignerStringIDs.ValueLevel_Close, "Close");
			AddString(ChartDesignerStringIDs.ValueLevel_High, "High");
			AddString(ChartDesignerStringIDs.ValueLeel_Low, "Low");
			AddString(ChartDesignerStringIDs.ValueLevel_Open, "Open");
			AddString(ChartDesignerStringIDs.ValueLevel_Value, "Value");
			AddString(ChartDesignerStringIDs.ValueLevel_Value2, "Value2");
			#endregion
		}
	}
}
