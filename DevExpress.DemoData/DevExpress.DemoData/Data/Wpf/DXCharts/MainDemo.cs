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
using System.Collections.Generic;
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_DXCharts_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "CrosshairCursorControl",
					displayName: @"Crosshair Cursor",
					group: "Interactivity",
					type: "ChartsDemo.CrosshairCursorControl",
					shortDescription: @"DXCharts provides you with API to add interactivity to your charts, e.g., to show a crosshair cursor.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to add a crosshair cursor to your charting application. To perform this task, the following steps are required:
                        <LineBreak/> <Bold>•</Bold> draw a vertical line for the current cursor position;
                        <LineBreak/> <Bold>•</Bold> convert the current cursor position from screen coordinates into chart coordinates via the XYDiagram2D.PointToDiagram method;
                        <LineBreak/> <Bold>•</Bold> find a series point that is closest to an argument from chart coordinates;
                        <LineBreak/> <Bold>•</Bold> for this series point, convert its chart coordinates into screen coordinates via the XYDiagram2D.DiagramToPoint method and draw a horizontal line for this point;
                        <LineBreak/> <Bold>•</Bold> show tooltips for corresponding X-axis and Y-axis values.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ToolTipControl",
					displayName: @"Tooltips",
					group: "Interactivity",
					type: "ChartsDemo.ToolTipControl",
					shortDescription: @"This demo illustrates how to implement custom tooltips for the chart control.",
					description: @"
                        <Paragraph>
                        DXCharts provide built-in tooltips to show additional information for hovered chart elements.
                        </Paragraph>
                        <Paragraph>
                        By default, series point tooltips display information about arguments and values, and this demo illustrates how to implement custom tooltips for series points. In this demo, a tooltip appears for each bar and shows another chart with a GDP history for the selected country.
                        </Paragraph>
                        <Paragraph>
                        Use the demo's pane options to specify both a tooltip position and tooltip location, and to define whether to show or hide the tooltip beak in the chart control.
                        </Paragraph>
                        <Paragraph>
                        Note that when the tooltip position is set either to Mouse Pointer (a tooltip is placed near the mouse pointer) or to Relative (a tooltip is placed near the element, for which it was invoked, e.g., to the bar top in this demo), tooltips location cannot correspond to the chosen location. This occurs when the tooltip doesn't have enough space in the chart region, and thus it is moved to the opposite side.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "AnimationControl",
					displayName: @"Animation",
					group: "Interactivity",
					type: "ChartsDemo.AnimationControl",
					shortDescription: @"This demo illustrates the capability to animate a chart's series, their points, or both.",
					description: @"
                        <Paragraph>
                        Each chart type supports a special set of animation presets that can be applied to series, its points, or both. For a specific animation, you can adjust its various parameters (such as the time interval, easy function and so on).
                        </Paragraph>
                        <Paragraph>
                        Moreover, you can create your own custom animations, by inheriting from the pre-defined animations.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "SelectionControl",
					displayName: @"Selection",
					group: "Interactivity",
					type: "ChartsDemo.SelectionControl",
					shortDescription: @"This demo shows how to implement selection based on the MVVM pattern.",
					description: @"
                        <Paragraph>
                        In this demo, you can see the population dashboard built with several chart controls, along with the map control.
                        </Paragraph>
                        <Paragraph>
                        Use the mouse or touch gestures to select a country either on a map or a pie chart to see population trends and urban/rural changes for this country.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ScrollingZoomingControl",
					displayName: @"Scrolling and Zooming",
					group: "Interactivity",
					type: "ChartsDemo.ScrollingZoomingControl",
					shortDescription: @"This demo illustrates scrolling and zooming features of a 2D XY-Diagram.",
					description: @"
                        <Paragraph>
                        This demo illustrates how end-users can zoom in or out of a 2D XY-chart, as well as scroll along its axes.
                        </Paragraph>
                        <Paragraph>
                        The chart in this demo represents 3 series, each containing 3,000 data points with date-time arguments and numeric values.
                        </Paragraph>
                        <Paragraph>
                        To <Bold>zoom in</Bold>, do the following:<LineBreak/> <Bold>•</Bold> Hold down SHIFT and click.<LineBreak/> <Bold>•</Bold> Press SHIFT and select a region on a diagram.<LineBreak/> <Bold>•</Bold> Use CTRL+PLUS SIGN.<LineBreak/> <Bold>•</Bold> Use the mouse wheel.<LineBreak/> <Bold>•</Bold> Resize a corresponding scroll bar.
                        </Paragraph>
                        <Paragraph>
                        To <Bold>zoom out</Bold>, do the following:<LineBreak/> <Bold>•</Bold> Use CTRL+MINUS SIGN.<LineBreak/> <Bold>•</Bold> Use the mouse wheel.<LineBreak/> <Bold>•</Bold> Resize a corresponding scroll bar.
                        </Paragraph>
                        <Paragraph>
                        To <Bold>scroll</Bold>, do the following:<LineBreak/> <Bold>•</Bold> Click a chart and drag it.<LineBreak/> <Bold>•</Bold> Use scrollbars.<LineBreak/> <Bold>•</Bold> Use CTRL+ARROW combinations.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CurrencyExchangeRates",
					displayName: @"Currency Exchange Rates",
					group: "Interactivity",
					type: "ChartsDemo.CurrencyExchangeRates",
					shortDescription: @"This demo illustrates the data aggregation feature for the date-time scale type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the chart's capability to aggregate a large amount of data for the date-time scale type.
                        You can zoom in/out the diagram (by clicking a view and using the mouse wheel) and scroll along diagram axes (by using the scrollbars or clicking a chart and drag it) to see how all data points are automatically aggregated on multiple Line series.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "NumericDataAggregation",
					displayName: @"Numeric Data Aggregation",
					group: "Interactivity",
					type: "ChartsDemo.NumericDataAggregation",
					shortDescription: @"This demo illustrates the data aggregation feature for the numeric scale type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capability of a chart to aggregate a large amount of data for the numeric scale type.  Zoom in/ out the diagram (by clicking a view and using the mouse wheel) and see how all data points are automatically aggregated on the Line chart.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ChartClientForRangeControlModule",
					displayName: @"Chart Client for Range Control",
					group: "Interactivity",
					type: "ChartsDemo.ChartClientForRangeControlModule",
					shortDescription: @"This demo shows a range control that is visualized by the date-time and numeric chart clients.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use date-time and numeric chart clients of the range control to paint chart data within the range control's viewport.
                        You can customize grid spacing in each chart view by using track bars in the numeric and date-time chart settings.   To change the date-time grid alignment, use the list displayed in the date-time chart settings.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RealtimeChart",
					displayName: @"Real-time Chart",
					group: "Performance",
					type: "ChartsDemo.RealtimeChartControl",
					shortDescription: @"This demo illustrates how to create a real-time Line chart with the help of DXCharts.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a real-time Line chart with the help of DXCharts.
                        This chart is represented by the Line series view, which is specially optimized to quickly display a very large data set, even if it is constantly changing over time.
                        In this demo, you can Pause data updates, or Resume them again; as well as change the time interval that specifies the total range of a time axis.
                        Also, you can enable the ""Show regression lines"" option to see how regression lines can be automatically calculated for a real-time chart.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "LargeDataSource",
					displayName: @"Large Datasource",
					group: "Performance",
					type: "ChartsDemo.LargeDataSourceControl",
					shortDescription: @"This demo illustrates how XtraCharts are capable of handling a large amount of data.",
					description: @"
                        <Paragraph>
                        This demo illustrates how XtraCharts are capable of handling a large amount of data.
                        This is possible thanks to the Line series view, which is specially optimized to quickly display a very large data set.
                        By default, this demo displays 50,000 points, and you can scroll the chart to see how fast is works.
                        However, this is not the limit - you can choose even more points for the chart (up to 1,000,000), click Apply and test how fast XtraCharts displays this amount of data.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "OverlappedRangeBar2DControl",
					displayName: @"2D Overlapped Range Bar",
					group: "Bar Series",
					type: "ChartsDemo.OverlappedRangeBar2DControl",
					shortDescription: @"This demo illustrates a 2D Overlapped Range Bar series type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>2D Overlapped Range Bar</Bold> chart, which displays either vertical or horizontal bars along the Y-axis (the axis of values). Each bar represents a range of data with two values for each argument value.
                        </Paragraph>
                        <Paragraph>
                        This chart type is useful, for instance, when it's necessary to show activity bars from different series – one above another to precisely compare their duration.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SideBySideRangeBar2DControl",
					displayName: @"2D Side-by-Side Range Bar",
					group: "Bar Series",
					type: "ChartsDemo.SideBySideRangeBar2DControl",
					shortDescription: @"This demo illustrates a 2D Side-by-Side Range Bar series type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>2D Side-by-Side Range Bar</Bold> chart, which displays either vertical or horizontal bars along the Y-axis (the axis of values). Each bar represents a range of data with two values for each argument value.
                        </Paragraph>
                        <Paragraph>
                        This chart type is useful, for instance, when it's necessary to show activity bars from different series -grouped by their arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarSideBySide2DControl",
					displayName: @"2D Side-by-Side Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarSideBySide2DControl",
					shortDescription: @"This demo illustrates a 2D Side-by-Side Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Side-by-Side Bar</Bold> chart. This is useful for showing the values of several series for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarStacked2DControl",
					displayName: @"2D Stacked Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarStacked2DControl",
					shortDescription: @"This demo illustrates a 2D Stacked Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Stacked Bar</Bold> chart. This is useful when it's necessary to compare both data point values and their aggregate, for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarFullStacked2DControl",
					displayName: @"2D Full-Stacked Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarFullStacked2DControl",
					shortDescription: @"This demo illustrates a 2D Full-Stacked Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Full-Stacked Bar</Bold> chart. This chart is also called a 100% Stacked Bar chart, and is useful for comparing the percentages of several series for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarSideBySideStacked2DControl",
					displayName: @"2D Side-by-Side Stacked Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarSideBySideStacked2DControl",
					shortDescription: @"This demo illustrates a 2D Side-by-Side Stacked Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Side-by-Side Stacked Bar</Bold> chart, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Stacked Bar chart types.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can group series either by sex or age.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarSideBySideFullStacked2DControl",
					displayName: @"2D Side-by-Side Full-Stacked Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarSideBySideFullStacked2DControl",
					shortDescription: @"This demo illustrates a 2D Side-by-Side Full-Stacked Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Side-by-Side Full-Stacked Bar</Bold> chart, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Full-Stacked Bar chart types.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can group series either by sex or age.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarSideBySide3DControl",
					displayName: @"3D Side-by-Side Bar",
					group: "Bar Series",
					type: "ChartsDemo.BarSideBySide3DControl",
					shortDescription: @"This demo illustrates a 3D Side-by-Side Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Side-by-side Bar</Bold> chart. This is useful for showing the values of several series for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ManhattanBar3DControl",
					displayName: @"3D Manhattan Bar",
					group: "Bar Series",
					type: "ChartsDemo.ManhattanBar3DControl",
					shortDescription: @"This demo illustrates a 3D Manhattan Bar series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Bar</Bold> chart. This chart is also called a Manhattan Bar chart, and is useful to display series of individual bars, grouped by category.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ScatterLine2DControl",
					displayName: @"2D Scatter Line",
					group: "Point/Line Series",
					type: "ChartsDemo.ScatterLine2DControl",
					shortDescription: @"This demo illustrates a 2D Scatter Line series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Scatter Line</Bold> chart, which displays series points in the same order that they have in the collection. That is in contrast to other view types that sort their points by arguments, and some aggregate points with equal arguments into a single entry along the X-axis.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PointScatter2DControl",
					displayName: @"2D Point/Scatter",
					group: "Point/Line Series",
					type: "ChartsDemo.PointScatter2DControl",
					shortDescription: @"This demo illustrates a 2D Point/Scatter series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Point</Bold> chart. This chart is also called a Scatter chart, and is useful when it's necessary to show points from different series on the same chart plot.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Line2DControl",
					displayName: @"2D Line",
					group: "Point/Line Series",
					type: "ChartsDemo.Line2DControl",
					shortDescription: @"This demo illustrates a 2D Line series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Line</Bold> chart. This is useful when you need to show line trends for several series on the same diagram and to compare the values of several series for the same points arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Spline2DControl",
					displayName: @"2D Spline",
					group: "Point/Line Series",
					type: "ChartsDemo.Spline2DControl",
					shortDescription: @"This demo illustrates use of the 2D Spline series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of a Spline chart. Similar to a Line chart, Spline charts draw a fitted curve through each series point. In this example, you can set an angle between labels and point markers, change line tension, specify point marker type/size and change their visibility.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "SplineArea2DControl",
					displayName: @"2D Spline Area",
					group: "Area Series",
					type: "ChartsDemo.SplineArea2DControl",
					shortDescription: @"This demo illustrates use of the 2D Spline Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of a Spline Area chart. Similar to an Area chart, Spline Area charts plot a fitted curve through each data point in a series. In this example, you can set an angle between labels and point markers, change line tension, specify point marker type/size and change their visibility. You can also specify the transparency value for all Area series simultaneously.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "StackedSplineArea2DControl",
					displayName: @"2D Stacked Spline Area",
					group: "Area Series",
					type: "ChartsDemo.StackedSplineArea2DControl",
					shortDescription: @"This demo illustrates use of the 2D Spline Area Stacked series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of a Stacked Spline Area chart. Similar to a Stacked Area chart, Spline Area Stacked charts plot a fitted curve through each data point in a series. In this example, you can specify the visibility of series point labels, set an angle between labels and point markers.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "FullStackedSplineArea2DControl",
					displayName: @"2D Full-Stacked Spline Area",
					group: "Area Series",
					type: "ChartsDemo.FullStackedSplineArea2DControl",
					shortDescription: @"This demo illustrates use of the 2D Full-Stacked Spline Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of a Full-Stacked Spline Area chart. Similar to a Full-Stacked Area chart, Full-Stacked Spline Area charts plot a fitted curve through each data point in a series. In this demo, you can specify whether real values are displayed for each point label or its representative percentage. You can also change the visibility of point labels, modify line tension and specify the transparency value for all Full-Stacked Spline Area series simultaneously.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "StackedLine2DControl",
					displayName: @"2D Stacked Line",
					group: "Point/Line Series",
					type: "ChartsDemo.StackedLine2DControl",
					shortDescription: @"This demo illustrates a 2D Stacked Line series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Stacked Line</Bold> chart. This chart is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FullStackedLine2DControl",
					displayName: @"2D Full-Stacked Line",
					group: "Point/Line Series",
					type: "ChartsDemo.FullStackedLine2DControl",
					shortDescription: @"This demo illustrates a 2D Full-Stacked Line series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Full-Stacked Line</Bold> chart. This chart is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments (as percents).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "StepLine2DControl",
					displayName: @"2D Step Line",
					group: "Point/Line Series",
					type: "ChartsDemo.StepLine2DControl",
					shortDescription: @"This demo illustrates a 2D Step Line series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Step Line</Bold> chart. This chart shows how much values have changed for different points of the same series.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Bubble2DControl",
					displayName: @"2D Bubble",
					group: "Point/Line Series",
					type: "ChartsDemo.Bubble2DControl",
					shortDescription: @"This demo illustrates a 2D Bubble series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Bubble</Bold> chart. This chart, in addition to the XY point diagram capabilities, allows you to visually represent the weight of a series data point as the size of a bubble.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Point3DControl",
					displayName: @"3D Point",
					group: "Point/Line Series",
					type: "ChartsDemo.Point3DControl",
					shortDescription: @"This demo illustrates a 3D Point series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Point</Bold> chart. This chart is useful when it's necessary to show points from different series on the same chart plot.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Bubble3DControl",
					displayName: @"3D Bubble",
					group: "Point/Line Series",
					type: "ChartsDemo.Bubble3DControl",
					shortDescription: @"This demo illustrates a 3D Bubble series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Bubble</Bold> chart. This chart, in addition to the 3D Point chart capabilities, allows you to visually represent the weight of a series data point as the size of a bubble.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RangeArea2DControl",
					displayName: @"2D Range Area",
					group: "Area Series",
					type: "ChartsDemo.RangeArea2DControl",
					shortDescription: @"This demo illustrates a 2D Range Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Range Area</Bold> chart, which displays series as filled areas on a diagram, with two data points that define minimum and maximum limits. This view is useful when you need to illustrate the difference between start and end values.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Area2DControl",
					displayName: @"2D Area",
					group: "Area Series",
					type: "ChartsDemo.Area2DControl",
					shortDescription: @"This demo illustrates a 2D Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Area</Bold> chart, which is useful when you need to show trends for several series on the same diagram and also show the relationship of the parts to a whole.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "StackedArea2DControl",
					displayName: @"2D Stacked Area",
					group: "Area Series",
					type: "ChartsDemo.StackedArea2DControl",
					shortDescription: @"This demo illustrates a 2D Stacked Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Stacked Area</Bold> chart, which is useful when it's necessary to compare both data point values and their aggregate, for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FullStackedArea2DControl",
					displayName: @"2D Full-Stacked Area",
					group: "Area Series",
					type: "ChartsDemo.FullStackedArea2DControl",
					shortDescription: @"This demo illustrates a 2D Full-Stacked Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Full-Stacked Area</Bold> chart. This chart is also called a 100% Stacked Area chart, and is useful for comparing the percentages of several series for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "StepArea2DControl",
					displayName: @"2D Step Area",
					group: "Area Series",
					type: "ChartsDemo.StepArea2DControl",
					shortDescription: @"This demo illustrates a 2D Step Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Step Area</Bold> chart. This chart shows how much values have changed for different points of the same series.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Area3DControl",
					displayName: @"3D Area",
					group: "Area Series",
					type: "ChartsDemo.Area3DControl",
					shortDescription: @"This demo illustrates a 3D Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Area</Bold> chart. This is useful when you need to show trends for several series on the same diagram and also show the relationship of parts to a whole.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "StackedArea3DControl",
					displayName: @"3D Stacked Area",
					group: "Area Series",
					type: "ChartsDemo.StackedArea3DControl",
					shortDescription: @"This demo illustrates a 3D Stacked Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Stacked Area</Bold> chart. This is useful when it's necessary to compare both data point values and their aggregate, for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FullStackedArea3DControl",
					displayName: @"3D Full-Stacked Area",
					group: "Area Series",
					type: "ChartsDemo.FullStackedArea3DControl",
					shortDescription: @"This demo illustrates a 3D Full-Stacked Area series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Full-Stacked Area</Bold> chart. This chart is also called a 100% Stacked Area chart, and is useful for comparing the percentages of several series for the same point arguments.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PieDonut2DControl",
					displayName: @"2D Pie/Donut",
					group: "Pie/Donut Series",
					type: "ChartsDemo.PieDonut2DControl",
					shortDescription: @"This demo illustrates a 2D Pie/Donut series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Pie</Bold> chart. This is useful to compare percentage values of different point arguments in the same series. Note that if the Hole Radius Percent value is non-zero, you will get a Donut chart instead.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "NestedDonut2DControl",
					displayName: @"Nested Donut",
					group: "Pie/Donut Series",
					type: "ChartsDemo.NestedDonut2DControl",
					shortDescription: @"This demo illustrates 2D Nested Donut series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Nested Donut</Bold> chart, which is similar to the Pie/Donut chart, but compares series with one donut nested in another one.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can group series either by sex or age. To see percentage values of different point arguments in tooltips, hover the mouse cursor over a Nested Donut series. In addition, you can change the hole radius percentage and inner indent to see how this affects the nested donut look.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Funnel2DControl",
					displayName: @"2D Funnel",
					group: "Funnel Series",
					type: "ChartsDemo.Funnel2DControl",
					shortDescription: @"This demo illustrates a Funnel series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Funnel chart.  This is useful to represent stages in a sales process and show the amount of potential revenue for each stage.  The Funnel chart displays values as progressively decreasing proportions. The size of the area is determined by the series value as a percentage of the total of all values, which is normally equal to the maximum value among the point's collection.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can specify the position and text format of points' labels, enable auto-size for the series, or manually define its height-to-width ratio. In addition, you can check whether to align the series to center, specify the distance between points and connector length.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PieDonut3DControl",
					displayName: @"3D Pie/Donut",
					group: "Pie/Donut Series",
					type: "ChartsDemo.PieDonut3DControl",
					shortDescription: @"This demo illustrates a 3D Pie/Donut series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>3D Donut</Bold> chart. This is useful to compare percentage values of different point arguments in the same series. Note that if you choose one of the ""Semi-"" models in the demo options, the Donut chart will look like a Pie chart.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Stock2DControl",
					displayName: @"2D Stock",
					group: "Financial Series",
					type: "ChartsDemo.Stock2DControl",
					shortDescription: @"This demo illustrates a 2D Stock series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Stock</Bold> chart, a financial tool which displays the High-Low-Open-Close price of a stock on a particular trading day. For each day, High and Low prices are represented by the top and bottom points of the line, while Open and Close prices are determined by left and right ticks respectively.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CandleStick2DControl",
					displayName: @"2D Candle Stick",
					group: "Financial Series",
					type: "ChartsDemo.CandleStick2DControl",
					shortDescription: @"This demo illustrates a 2D Candle Stick series view type.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>Candle Stick</Bold> chart, a financial tool which displays the High-Low-Open-Close price of a stock on a particular trading day. For each day, High and Low prices are represented by the top and bottom points of the line, while Open and Close prices are determined by the filled rectangle.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RadarAreaSeriesControl",
					displayName: @"Radar Area",
					group: "Radar/Polar Series",
					type: "ChartsDemo.RadarAreaSeriesControl",
					shortDescription: @"This demo illustrates a Radar Area series view that plots the Wind Rose chart.",
					description: @"
                        <Paragraph>
                        In this demo, the Radar Area chart displays the wind direction on the angular axis, and the wind observation time (in days) on the radial axis.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Radar view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        You can also rotate the Radar Area chart using the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RadarLineSeriesControl",
					displayName: @"Radar Line",
					group: "Radar/Polar Series",
					type: "ChartsDemo.RadarLineSeriesControl",
					shortDescription: @"This demo illustrates a Radar Line series view that plots the Wind Rose chart.",
					description: @"
                        <Paragraph>
                        In this demo, the Radar Line chart displays the wind direction on the angular axis, and the wind observation time (in days) on the radial axis.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Radar view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        You can also rotate the Radar Line chart using the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RadarPointSeriesControl",
					displayName: @"Radar Point",
					group: "Radar/Polar Series",
					type: "ChartsDemo.RadarPointSeriesControl",
					shortDescription: @"This demo shows a Radar Point series view that plots the average temperature in London.",
					description: @"
                        <Paragraph>
                        In this demo, the Radar Point chart displays the calendar date on the angular axis, and the temperature's value on the radial axis.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Radar view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        This demo also allows you to rotate the Radar Point chart either in a clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RadarLineScatterSeriesControl",
					displayName: @"Scatter Radar Line",
					group: "Radar/Polar Series",
					type: "ChartsDemo.RadarLineScatterSeriesControl",
					shortDescription: @"This demo shows a Scatter Radar Line series view, which displays function values in polar coordinates.",
					description: @"
                        <Paragraph>
                        In this demo, the Scatter Radar Line chart can display values of the Archimedean Spiral, Polar Rose or Polar Folium function in the same order that they have in the series points collection.
                        </Paragraph>
                        <Paragraph>
                        This demo allows you to rotate the Scatter Radar Line chart either in clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "PolarAreaSeriesControl",
					displayName: @"Polar Area",
					group: "Radar/Polar Series",
					type: "ChartsDemo.PolarAreaSeriesControl",
					shortDescription: @"This demo shows a Polar Area series view that plots function values in polar coordinates.",
					description: @"
                        <Paragraph>
                        In this demo, the Polar Area chart can display the values of Taubin's Heart, Cardioid or Lemniscate function.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Polar Area view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        This demo also allows you to rotate the Polar Area chart either in clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PolarLineSeriesControl",
					displayName: @"Polar Line",
					group: "Radar/Polar Series",
					type: "ChartsDemo.PolarLineSeriesControl",
					shortDescription: @"This demo shows a Polar Line series view that plots function values in polar coordinates.",
					description: @"
                        <Paragraph>
                        In this demo, the Polar Line chart can display the values of Taubin's Heart, Cardioid or Lemniscate function.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Polar Line view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        This demo also allows you to rotate the Polar Line chart either in clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PolarPointSeriesControl",
					displayName: @"Polar Point",
					group: "Radar/Polar Series",
					type: "ChartsDemo.PolarPointSeriesControl",
					shortDescription: @"This demo shows a Polar Point series view that plots function values in polar coordinates.",
					description: @"
                        <Paragraph>
                        In this demo, the Polar Point chart can display the values of Taubin's Heart, Cardioid or Lemniscate function.
                        </Paragraph>
                        <Paragraph>
                        You can select on which grid the Polar Point view should be drawn (either Circle or Polygon) in the demo's ""Shape Style"" option.
                        </Paragraph>
                        <Paragraph>
                        This demo also allows you to rotate the Polar Point chart either in clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PolarLineScatterSeriesControl",
					displayName: @"Scatter Polar Line",
					group: "Radar/Polar Series",
					type: "ChartsDemo.PolarLineScatterSeriesControl",
					shortDescription: @"This demo shows a Scatter Polar Line series view, which displays function values in polar coordinates.",
					description: @"
                        <Paragraph>
                        In this demo, the Scatter Polar Line chart can display values of the Archimedean Spiral, Polar Rose or Polar Folium function in the same order that they have in the series points collection.
                        </Paragraph>
                        <Paragraph>
                        This demo allows you to rotate the Scatter Polar Line chart either in clockwise or counterclockwise direction. This can be done by choosing the ""Rotation Direction"" option and changing the angle value in the ""Start Angle"" pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "BindingIndividualSeriesControl",
					displayName: @"Binding Individual Series",
					group: "Data Binding",
					type: "ChartsDemo.BindingIndividualSeriesControl",
					shortDescription: @"This demo illustrates the capability to bind each series individually.",
					description: @"
                        <Paragraph>
                        This demo illustrates one of the data binding approaches introduced in DXCharts. First it's necessary to create static series in the chart, and then bind each of them to data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BindingUsingSeriesTemplate",
					displayName: @"Binding Using Series Template",
					group: "Data Binding",
					type: "ChartsDemo.BindingUsingSeriesTemplate",
					shortDescription: @"This demo illustrates the capability to create series automatically, based on a common pre-defined template.",
					description: @"
                        <Paragraph>
                        This demo illustrates one of the data binding approaches introduced in DXCharts. In this way the series data within series names are stored in the same data source, and the series are all generated automatically after a chart has been bound to data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TagPropertyControl",
					displayName: @"Tag Property",
					group: "Data Binding",
					type: "ChartsDemo.TagPropertyControl",
					shortDescription: @"This demo illustrates the use of the tag property for series points, to store extra information from the underlying data source.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the Tag property of the SeriesPoint object. In this example, the Tag property stores information from the underlying datasource (a country's official name), and then its value is used to populate legend items for corresponding series points. This feature is very useful when it's necessary to store some specific information associated with a particular series point.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ColorizerControl",
					displayName: @"Colorizer",
					group: "Data Binding",
					type: "ChartsDemo.ColorizerControl",
					shortDescription: @"This demo illustrates the capability of the chart control to color its series based on bound data.",
					description: @"
                        <Paragraph>
                        In this demo, the chart control shows the Happy Planet Index (HPI) for G20 on the Bubble chart. The size of bubbles is proportional to the country population and their color is determined depending on the HPI value. The data for this demo is provided by www.happyplanetindex.org
                        </Paragraph>",
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "EmptyPointsControl",
					displayName: @"Empty Points",
					group: "Data Representation",
					type: "ChartsDemo.EmptyPointsControl",
					shortDescription: @"This demo illustrates how points with missing values are represented in various chart types.",
					description: @"
                        <Paragraph>
                        This demo illustrates how DXCharts processes missing values, which are often caused by skipped tests or measurements. These values are transformed into empty points, represented by breaks in line/area graphs or missing points, bars etc. in other series view types.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you're able to change the current series view type to see how empty points are represented in different series views.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LogarithmicScaleControl",
					displayName: @"Logarithmic Scale",
					group: "Data Representation",
					type: "ChartsDemo.LogarithmicScaleControl",
					shortDescription: @"This demo illustrates the logarithmic scale feature.",
					description: @"
                        <Paragraph>
                        This demo illustrates the logarithmic scale, which is very useful when it is necessary to show different values, if some of them are much greater than others.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ValueDateTimeScaleControl",
					displayName: @"Value Date-Time Scale",
					group: "Data Representation",
					type: "ChartsDemo.ValueDateTimeScaleControl",
					shortDescription: @"This demo illustrates the capability to represent date-time values in the required format.",
					description: @"
                        <Paragraph>
                        This demo illustrates how date-time values can be represented with DXCharts. Note that in this demo the axis of values is displayed as an X-axis, and the axis of arguments is displayed as an Y-axis, because the XYDiagram's Rotated property is set to true.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PanesControl",
					displayName: @"Panes",
					group: "Chart Elements",
					type: "ChartsDemo.PanesControl",
					shortDescription: @"This demo illustrates XY-series’ capability to be spread onto different panes on a diagram.",
					description: @"
                        <Paragraph>
                        With DXCharts you can create a chart with multiple panes, stacked either vertically or horizontally. Each pane can plot one or more XY-series, and can share either the X or Y axis with other panes. This allows you, for example, to simultaneously scroll data on all panes using a single axis scrollbar.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "XYDiagram2DControl",
					displayName: @"2D XY-Diagram",
					group: "Chart Elements",
					type: "ChartsDemo.XYDiagram2DControl",
					shortDescription: @"This demo shows the capability to plot multiple series onto a single diagram object.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to adjust the representation of the 2D XY-Diagram, as an example of the rich customization capabilities provided by DXCharts for each visual element.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SecondaryAxesControl",
					displayName: @"Secondary Axes",
					group: "Chart Elements",
					type: "ChartsDemo.SecondaryAxesControl",
					shortDescription: @"This demo illustrates the capability to specify an unlimited number of secondary axes for each series.",
					description: @"
                        <Paragraph>
                        This demo illustrates secondary axes, which allow you to assign separate axes for each series. For example, this is useful when your series use different arguments, or their value ranges vary significantly.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can choose which axes should be assigned to the second series, and compare the result.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ConstantLinesControl",
					displayName: @"Constant Lines",
					group: "Chart Elements",
					type: "ChartsDemo.ConstantLinesControl",
					shortDescription: @"This demo illustrates constant lines, to visually represent static values across an axis.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capability to highlight an axis value by a drawing a constant line across it.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can adjust the alignment and position of the constant lines titles.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomAxisLabelsControl",
					displayName: @"Custom Axis Labels",
					group: "Chart Elements",
					type: "ChartsDemo.CustomAxisLabelsControl",
					shortDescription: @"This demo illustrates the capability to provide custom labels for each axis.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capability to provide custom labels for an axis, instead of the auto-generated default axis labels.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SeriesTitlesControl",
					displayName: @"Series Titles",
					group: "Chart Elements",
					type: "ChartsDemo.SeriesTitlesControl",
					shortDescription: @"This demo illustrates the capability to accompany each series with an explanatory title.",
					description: @"
                        <Paragraph>
                        This demo illustrates series titles. Each 2D or 3D Pie (Doughnut) series can be accompanied by an unlimited number of explanatory titles. Then, series titles can be positioned anywhere around a series.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ChartTitlesControl",
					displayName: @"Chart Titles",
					group: "Chart Elements",
					type: "ChartsDemo.ChartTitlesControl",
					shortDescription: @"This demo illustrates the capability to accompany your chart with an unlimited number of titles that can be variously positioned.",
					description: @"
                        <Paragraph>
                        This demo illustrates chart titles. You can create an unlimited number of chart titles, and position them anywhere around the diagram.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomBar3DControl",
					displayName: @"3D Custom Bar Models",
					group: "Appearance",
					type: "ChartsDemo.CustomBar3DControl",
					shortDescription: @"This demo illustrates the capability to implement custom models for 3D Bar series.",
					description: @"
                        <Paragraph>
                        This demo illustrates that it is possible to use any custom model instead of simple bars in the Chart control.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "CustomDrawControl",
					displayName: @"Custom Draw",
					group: "Appearance",
					type: "ChartsDemo.CustomDrawControl",
					shortDescription: @"This demo illustrates the use of the CustomDraw event handler, to paint series points according to their values.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to implement custom drawing in DXCharts. Thanks to the ChartControl's CustomDraw events, it's possible to change drawing options before painting the entire series or its individual points.
                        </Paragraph>
                        <Paragraph>
                        This example demonstrates how to change fill colors for every series point according to their values. In addition, point labels text is changed to show the color of the current interval (Green, Yellow or Red).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomBar2DControl",
					displayName: @"2D Custom Bar Models",
					group: "Appearance",
					type: "ChartsDemo.CustomBar2DControl",
					shortDescription: @"This demo illustrates the capability to implement custom models for 2D Bar series.",
					description: @"
                        <Paragraph>
                        For 2D Bar series, you can create your own custom models, by defining a control template that represents an individual Bar. Then, you can embed any visual element into this control template.
                        </Paragraph>
                        <Paragraph>
                        In addition, this demo shows an implementation of a custom point animation.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomPie2DControl",
					displayName: @"2D Custom Pie Models",
					group: "Appearance",
					type: "ChartsDemo.CustomPie2DControl",
					shortDescription: @"This demo illustrates the capability to implement custom models for 2D Pie (Doughnut) series.",
					description: @"
                        <Paragraph>
                        For 2D Pie series, you can create your own custom models, by defining a control template that represents a Pie. Then, you can embed any visual element into this control template.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomMarker2DControl",
					displayName: @"2D Custom Marker Models",
					group: "Appearance",
					type: "ChartsDemo.CustomMarker2DControl",
					shortDescription: @"This demo illustrates the capability to implement custom models for markers of 2D series.",
					description: @"
                        <Paragraph>
                        For 2D series, you can create your own custom marker models, by defining a control template that represents an individual marker. Then, you can embed any visual element into this control template.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomFinancial2DControl",
					displayName: @"2D Custom Financial Models",
					group: "Appearance",
					type: "ChartsDemo.CustomFinancial2DControl",
					shortDescription: @"This demo illustrates the capability to implement custom models for financial series.",
					description: @"
                        <Paragraph>
                        For financial series (Stock and Candle Stick), you can create your own custom models, by defining a control template that represents an individual point, or its element (TopStick, BottomStick, Candle and InvertedCandle for a Candle Stick model, or OpenLine, CloseLine and CenterLine - for a Stock model).
                        </Paragraph>
                        <Paragraph>
                        Then, you can embed any visual element into this control template.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ResolveLabelsOverlappingfor3DPieSeries",
					displayName: @"Resolve Labels Overlapping for 3D Pie Series",
					group: "Resolve Labels Overlapping",
					type: "ChartsDemo.ResolveLabelsOverlappingfor3DPieSeries",
					shortDescription: @"This demo shows how to avoid overlapping labels in 3D Pie series.",
					description: @"
                        <Paragraph>
                        In this demo, you can see 3D Pie series labels automatically arranged to avoid their overlapping, irrespective of the Pie rotation angle.
                        </Paragraph>
                        <Paragraph>
                        To see how the resolve overlapping feature affects 3D Pie series labels, deactivate the Enable Resolve Overlapping option on the demo's pane.
                        </Paragraph>
                        <Paragraph>
                        You can also modify the Resolve Overlapping Indent value to achieve the better results.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ResolveLabelsOverlappingForXYSeries",
					displayName: @"Resolve Labels Overlapping for XY Series",
					group: "Resolve Labels Overlapping",
					type: "ChartsDemo.ResolveLabelsOverlappingForXYSeries",
					shortDescription: @"This demo illustrates the capability of automatic detection, and resolving of intersecting series point labels.",
					description: @"
                        <Paragraph>
                        This demo illustrates the DXCharts capability to keep series labels from overlapping. In this demo's options, you can select one of the available resolution modes, and modify corresponding parameters to achieve the most appropriate and pleasing results.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ResolveOverlappingForAxisLabels",
					displayName: @"Resolve Overlapping for Axis Labels",
					group: "Resolve Labels Overlapping",
					type: "ChartsDemo.ResolveOverlappingForAxisLabels",
					shortDescription: @"This demo illustrates automatic detection and resolving of intersecting axis labels.",
					description: @"
                        <Paragraph>
                        In this demo you can see the work of a resolve overlapping algorithm that prevents intersection of axis labels by making labels staggered. In addition, this algorithm provides rotation and hiding overlapped axis labels. To see how this works, resize this demo.
                        </Paragraph>
                        <Paragraph>
                        You can also disable/enable the Resolve Overlapping option and specify the resolve overlapping indent between neighboring axis labels to achieve better results.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "BasicFinancialIndicatorsControl",
					displayName: @"Basic Financial Indicators",
					group: "Data Analysis",
					type: "ChartsDemo.BasicFinancialIndicatorsControl",
					shortDescription: @"This demo illustrates basic financial indicators calculated for a stock series.",
					description: @"
                        <Paragraph>
                        This demo shows indicator types that are commonly used in charting data for financial analysis:  Trend Line, Regression Line and Fibonacci Indicators.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can select the kind of indicator that should be drawn on a chart using the ""Indicator"" option.
                        </Paragraph>
                        <Paragraph>
                        In addition, you can enable the ""Show indicator in legend"" option to see this indicator in Legend.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "MovingAveragesControl",
					displayName: @"Moving Averages",
					group: "Data Analysis",
					type: "ChartsDemo.MovingAveragesControl",
					shortDescription: @"This demo illustrates the Moving Average and Envelope financial indicators.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Moving Average and Envelope financial indicators, that are commonly used in charting data for financial analysis.
                        </Paragraph>
                        <Paragraph>
                         In this demo, use the Kind selector, to choose which indicator to show (Moving Average, Envelope, or both), and the Type selector, to choose the indicator's type (Simple, Exponential, Weighted, Triangular or Triple).
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "OscillatorIndicatorsControl",
					displayName: @"Oscillator Indicators",
					group: "Data Analysis",
					type: "ChartsDemo.OscillatorIndicatorsControl",
					shortDescription: @"This demo illustrates the Oscillator financial indicators.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Oscillator financial indicators.
                        </Paragraph>
                        <Paragraph>
                        Currently the Chart Control supports the following indicators: Average True Range, Commodity Channel Index, Detrended Price Oscillator, Moving Average Convergence Divergence, Rate of Change, Relative Strength Index, Chaikins Volatility, and Williams % R.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "PriceIndicatorsControl",
					displayName: @"Price Indicators",
					group: "Data Analysis",
					type: "ChartsDemo.PriceIndicatorsControl",
					shortDescription: @"This demo shows the Price financial indicators.",
					description: @"
                        <Paragraph>
                        This demo shows the Price financial indicators.
                        </Paragraph>
                        <Paragraph>
                        Currently the Chart Control supports the following indicators: Median Price, Typical Price, and Weighted Close.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "TrendIndicatorsControl",
					displayName: @"Trend Indicators",
					group: "Data Analysis",
					type: "ChartsDemo.TrendIndicatorsControl",
					shortDescription: @"This demo demonstrates the Trend financial indicators.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the Trend financial indicators.
                        </Paragraph>
                        <Paragraph>
                        Currently the Chart Control supports the following indicators: Bollinger Bands, Mass Index, and Standard Deviation. 
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
			};
		}
	}
}
