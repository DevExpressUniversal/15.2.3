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
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_XtraCharts_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				#region About
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraCharts %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraCharts.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				#endregion
				#region Bar Views
				new SimpleModule(demo,
					name: "Bar",
					displayName: @"Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.BarDemo",
					description: @"This demo illustrates a Side-by-Side Bar series view, which is useful when it's necessary to compare both the point values and their aggregate for the same point arguments. In this demo, you can show the series point labels (when the Show Labels option is enabled), and adjust their position relative to Bars (via the Label Position and Orientation drop-down lists). When the labels are positioned inside the Bars, you can specify the inner indent from the corresponding Bar edge (via the Label Indent editor). To rotate the diagram, right-click the chart, and toggle the ""Rotated"" option.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\Bar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\Bar.vb"
					}
				),
				new SimpleModule(demo,
					name: "StackedBar",
					displayName: @"Stacked Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.StackedBarDemo",
					description: @"This demo illustrates a Stacked Bar series view, which is useful when it's necessary to compare both the point values and their aggregate for the same point arguments. In this demo, you can show the series point labels (when the Show Labels option is enabled), and adjust their position relative to Bars (via the Label Position and Orientation drop-down lists). When the labels are positioned inside the Bars, you can specify the inner indent from the corresponding Bar edge (via the Label Indent editor). To rotate the diagram, right-click the chart, and toggle the ""Rotated"" option.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\StackedBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\StackedBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullStackedBar",
					displayName: @"Full-Stacked Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.FullStackedBarDemo",
					description: @"This demo illustrates a Full-Stacked Bar series view, which is useful when it's necessary to compare both the point values and their aggregate for the same point arguments. In this demo, you can show the series point labels (when the Show Labels option is enabled), and adjust their position relative to Bars (via the Label Position and Orientation drop-down lists). When the labels are positioned inside the Bars, you can specify the inner indent from the corresponding Bar edge (via the Label Indent editor). To rotate the diagram, right-click the chart, and toggle the ""Rotated"" option. In addition, you can choose whether the labels reflect the real values, or percentages (via the Value As Percent check box).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\FullStackedBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\FullStackedBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "StackedBarSideBySide",
					displayName: @"Stacked Bar Side-by-Side",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.StackedBarSideBySideDemo",
					description: @"This demo illustrates a Side-by-Side Stacked Bar series view, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Stacked Bar view types. In this demo, you can group series either by sex or age.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\StackedBarSideBySide.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\StackedBarSideBySide.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullStackedBarSideBySide",
					displayName: @"Full-Stacked Bar Side-by-Side",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.FullStackedBarSideBySideDemo",
					description: @"This demo illustrates a Side-by-Side Full-Stacked Bar series view, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Full-Stacked Bar view types. In this demo, you can group series either by sex or age.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\FullStackedBarSideBySide.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\FullStackedBarSideBySide.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DBar",
					displayName: @"3D Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.Bar3dDemo",
					description: @"This demo illustrates a Stacked Bar series view, which is useful when it's necessary to compare both the point values and their aggregate for the same point arguments. In this demo, you can specify the view's perspective angle, and choose a 3D model used to draw series points. The view can also be rotated via mouse, and zoomed via the mouse wheel. To restore default angles, click the corresponding button. And, to enable the top facet for flat-top 3D models, use the corresponding check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStackedBar",
					displayName: @"3D Stacked Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.StackedBar3dDemo",
					description: @"This demo illustrates the 3D series' Stacked Bars View. This view is useful when it's necessary to compare both the points values and their aggregate for the same points arguments. In this demo, you can specify the view's perspective angle, and choose a 3D model used to draw series points. The view can also be rotated via mouse, and zoomed via the mouse wheel. To restore default angles, click the corresponding button. And, to enable the top facet for flat-top 3D models, use the corresponding check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DStackedBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DStackedBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFullStackedBar",
					displayName: @"3D Full-Stacked Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.FullStackedBar3dDemo",
					description: @"This demo illustrates a Full-Stacked Bar series view, which is also called the 100% Stacked Bars. This view is useful for comparing the percent values of several series for the same point arguments. In this demo, you can specify the view's perspective angle, and choose a 3D model used to draw series points. The view can also be rotated via mouse, and zoomed via the mouse wheel. To restore default angles, click the corresponding button. And, to enable the top facet for flat-top 3D models, use the corresponding check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DFullStackedBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DFullStackedBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStackedBarSideBySide",
					displayName: @"3D Stacked Bar Side-by-Side",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.StackedBarSideBySide3dDemo",
					description: @"This demo illustrates a Side-by-Side Stacked Bar series view, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Stacked Bar view types. In this demo, you can group series either by sex or age.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DStackedBarSideBySide.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DStackedBarSideBySide.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFullStackedBarSideBySide",
					displayName: @"3D Full-Stacked Bar Side-by-Side",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.FullStackedBarSideBySide3dDemo",
					description: @"This demo illustrates a Side-by-Side Full-Stacked Bar series view, which allows you to stack series having a similar StackedGroup property value into the same bars. So, this view combines advantages of both the Side-by-Side Bar and Full-Stacked Bar view types. In this demo, you can group series either by sex or age.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DFullStackedBarSideBySide.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DFullStackedBarSideBySide.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DManhattanBar",
					displayName: @"3D Manhattan Bar",
					group: "Bar Views",
					type: "DevExpress.XtraCharts.Demos.BarViews.ManhattanBarDemo",
					description: @"This demo illustrates a Manhattan Bar series view, which displays series of individual bars, grouped by category. In this demo, you can specify the view's perspective angle, and choose a 3D model used to draw series points. The view can also be rotated via mouse, and zoomed via the mouse wheel. To restore default angles, click the corresponding button. And, to enable the top facet for flat-top 3D models, use the corresponding check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\BarViews\3DManhattanBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\BarViews\3DManhattanBar.vb"
					}
				),
				#endregion
				#region Point/Line Views
				new SimpleModule(demo,
					name: "Point",
					displayName: @"Point",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.PointDemo",
					description: @"This demo illustrates a Point series view, which shows points from different series on the same diagram. To generate random points, click the ""Create Points"" button. In this demo, you can select a series by clicking its point on the diagram. Then, for the currently selected series, you can specify the kind and size of the point markers, and the angle at which the point labels are rotated around the points.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\Point.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\Point.vb"
					}
				),
				new SimpleModule(demo,
					name: "Bubble",
					displayName: @"Bubble",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.BubbleDemo",
					description: @"This demo shows a Bubble series view. This chart, in addition to the XY point diagram capabilities, allows you to visually represent the Weight of a series point by a bubble's size. In this demo, you can change labels' position, choose the kind of a marker and define the minimum and maximum bubble size.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\Bubble.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\Bubble.vb"
					}
				),
				new SimpleModule(demo,
					name: "Line",
					displayName: @"Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.LineDemo",
					description: @"This demo illustrates a Line series view, which shows line trends for several series on the same diagram. In this demo, you can select a series, by clicking it on the diagram. Then, for the currently selected series, you can specify the kind and size of the point markers.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\Line.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\Line.vb"
					}
				),
				new SimpleModule(demo,
					name: "StackedLine",
					displayName: @"Stacked Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.StackedLineDemo",
					description: @"This demo illustrates the Stacked Line series view, which is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments. In this demo you can select any series, and then change the kind and size of its markers, as well as toggle the visibility of series labels and markers.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\StackedLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\StackedLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullStackedLine",
					displayName: @"Full-Stacked Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.FullStackedLineDemo",
					description: @"This demo illustrates the Full-Stacked Line series view, which is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments (as percents). In this demo you can select any series, and then change the kind and size of its markers, specify what should be displayed in series labels, as well as toggle the visibility of series labels and markers.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\FullStackedLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\FullStackedLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "StepLine",
					displayName: @"Step Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.StepLineDemo",
					description: @"This demo illustrates a Step Line series view, which shows by how much values have changed for different points of the same series. To invert steps, toggle the ""Inverted Step"" option. In this demo, you can specify the type and size of point markers, change their visibility, and also set the angle between the labels and the point markers for the currently selected series.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\StepLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\StepLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "Spline",
					displayName: @"Spline",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.SplineDemo",
					description: @"This demo illustrates a Spline series view, which is similar to the Line view, but draws a fitted curve through each series point. In this demo, you can change the line tension, specify the type and size of point markers, change their visibility, and also set an angle between labels and point markers for the current Spline series.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\Spline.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\Spline.vb"
					}
				),
				new SimpleModule(demo,
					name: "ScatterLine",
					displayName: @"Scatter Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.ScatterLineDemo",
					description: @"This demo illustrates a Scatter Line series view, whose main characteristic is that the series' points are inter-connected by lines in the same sequence they have in the points' collection, without sorting and aggregating them by arguments. In other aspects, this view is similar to the Line view. In this demo, the series represents the well-known Archimedean spiral, and you also can make it display a cardioid, or a Cartesian folium, using the ""Function"" combo box. In addition, you can specify the kind and size of points' markers, disable markers, and display labels for series points.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\ScatterLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\ScatterLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DLine",
					displayName: @"3D Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.Line3dDemo",
					description: @"This demo illustrates a 3D Line series view, which shows line trends for several series on the same diagram. In this demo, you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\3DLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\3DLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStackedLine",
					displayName: @"3D Stacked Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.StackedLine3dDemo",
					description: @"This demo illustrates the 3D Stacked Line series view, which is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments. In this demo you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\3DStackedLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\3DStackedLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFullStackedLine",
					displayName: @"3D Full-Stacked Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.FullStackedLine3dDemo",
					description: @"This demo illustrates the 3D Full-Stacked Line series view, which is useful when it's necessary to compare how much each series adds to the total aggregate value for specific arguments (as percents). In this demo you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\3DFullStackedLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\3DFullStackedLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStepLine",
					displayName: @"3D Step Line",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.StepLine3dDemo",
					description: @"This demo illustrates a Step Line series view, which shows by how much values have changed for different points of the same series. To invert steps, toggle the ""Inverted Step"" option. In this demo, you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\3DStepLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\3DStepLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DSpline",
					displayName: @"3D Spline",
					group: "Point/Line Views",
					type: "DevExpress.XtraCharts.Demos.PointLineViews.Spline3dDemo",
					description: @"This demo illustrates a Spline series view, which is similar to the 3D Line view, but draws a fitted curve through each series point. In this demo, you can specify the view's perspective angle. The view can be also rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PointLineViews\3DSpline.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PointLineViews\3DSpline.vb"
					}
				),
				#endregion
				#region Swift Plot View
				new SimpleModule(demo,
					name: "RealtimeChart",
					displayName: @"Real-time Chart",
					group: "Swift Plot View",
					type: "DevExpress.XtraCharts.Demos.SwiftPlotView.RealtimeChartDemo",
					description: @"This demo illustrates how to create a real-time Line chart with the help of XtraCharts. This chart is represented by the SwiftPlot series view, which is specially optimized to quickly display a very large data set, even if it is constantly changing over time. In this demo, you can Pause data updates, or Resume them again; as well as change the time interval that specifies the total range of a time axis. Also, you can enable the ""Show regression lines"" option to see how regression lines can be automatically calculated for a real-time chart.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\SwiftPlotView\RealtimeChart.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\SwiftPlotView\RealtimeChart.vb"
					}
				),
				new SimpleModule(demo,
					name: "LargeDataSource",
					displayName: @"Large Datasource",
					group: "Swift Plot View",
					type: "DevExpress.XtraCharts.Demos.SwiftPlotView.LargeDataSourceDemo",
					description: @"This demo illustrates how XtraCharts are capable of handling a large amount of data. This is possible thanks to the SwiftPlot series view, which is specially optimized to quickly display a very large data set. By default, this demo displays 50,000 points, and you can scroll the chart to see how fast is works. However, this is not the limit - you can choose even more points for the chart (up to 1,000,000), click Apply and test how fast XtraCharts displays this amount of data.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\SwiftPlotView\LargeDataSource.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\SwiftPlotView\LargeDataSource.vb"
					}
				),
				#endregion
				#region Pie/Doughnut Views
				new SimpleModule(demo,
					name: "Pie",
					displayName: @"Pie",
					group: "Pie/Doughnut Views",
					type: "DevExpress.XtraCharts.Demos.PieDoughnutViews.PieDemo",
					description: @"This demo illustrates a Pie series view, which is used to compare percentage values of different point arguments in the same series. In this demo, you can specify the pie label position, as well as to specify whether real values are shown for each point label or for the corresponding percentage. Also, you may choose which pie slices should be exploded via the drop-down box. Otherwise, you may manually explode pie slices by left clicking and dragging them.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PieDoughnutViews\Pie.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PieDoughnutViews\Pie.vb"
					}
				),
				new SimpleModule(demo,
					name: "Doughnut",
					displayName: @"Doughnut",
					group: "Pie/Doughnut Views",
					type: "DevExpress.XtraCharts.Demos.PieDoughnutViews.DoughnutDemo",
					description: @"This demo illustrates a Doughnut series view, which is similar to the Pie view, but displays a hole in its center. In this demo, you can change the chart hole's radius percentage to see how this affects the chart's look. Also, you may choose which pie slices should be exploded via the drop-down box. Otherwise, you may manually explode pie slices by left clicking and dragging them.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PieDoughnutViews\Doughnut.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PieDoughnutViews\Doughnut.vb"
					}
				),
				new SimpleModule(demo,
					name: "NestedDoughnut",
					displayName: @"Nested Doughnut",
					group: "Pie/Doughnut Views",
					type: "DevExpress.XtraCharts.Demos.PieDoughnutViews.NestedDoughnutDemo",
					description: @"This demo illustrates a Nested Doughnut series view, which is similar to the Doughnut series view, but compares series with one doughnut nested in another one. In this demo you can group series either by sex or age. To see percentage values of different point arguments in tooltips, hover the mouse cursor over a Nested Doughnut series. In addition, you can change the hole radius percentage and inner indent to see how this affects the nested doughnut look.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PieDoughnutViews\NestedDoughnut.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PieDoughnutViews\NestedDoughnut.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DPie",
					displayName: @"3D Pie",
					group: "Pie/Doughnut Views",
					type: "DevExpress.XtraCharts.Demos.PieDoughnutViews.Pie3dDemo",
					description: @"This demo illustrates a 3D Pie series view, which is used to compare percentage values of different point arguments in the same series. In this demo, you can rotate the view using the mouse. The view can also be zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button. Use the ""Label Position"" combo box to specify the way in which pie labels are positioned within pie slices. The ""Exploded Points"" and ""Explode Distance"" combo boxes allow you to define which pie slices should be exploded and specify the explode distance, respectively.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PieDoughnutViews\3DPie.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PieDoughnutViews\3DPie.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DDoughnut",
					displayName: @"3D Doughnut",
					group: "Pie/Doughnut Views",
					type: "DevExpress.XtraCharts.Demos.PieDoughnutViews.Doughnut3dDemo",
					description: @"This demo illustrates a 3D Doughnut series view, which is similar to the 3D Pie view, but displays a hole in its center. In this demo, you can change the chart hole's radius percentage to see how this affects the chart's look. Also, you may choose which pie slices should be exploded via the drop-down box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\PieDoughnutViews\3DDoughnut.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\PieDoughnutViews\3DDoughnut.vb"
					}
				),
				#endregion
				#region Funnel Views
				new SimpleModule(demo,
					name: "Funnel",
					displayName: @"Funnel",
					group: "Funnel Views",
					type: "DevExpress.XtraCharts.Demos.FunnelViews.FunnelDemo",
					description: @"This demo illustrates a Funnel series view, typically used to represent stages in a sales process and show the amount of potential revenue for each stage. A funnel chart displays values as progressively decreasing proportions. The size of the area is determined by the series value as a percentage of the total of all values, which is normally equal to the maximum value among the points' collection. In this demo, you can specify the position and text format of points' labels, enable auto-size for the series, or manually define its height-to-width ratio. In addition, you can check whether to align the series to center, and specify the distance between points.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\FunnelViews\Funnel.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\FunnelViews\Funnel.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFunnel",
					displayName: @"3D Funnel",
					group: "Funnel Views",
					type: "DevExpress.XtraCharts.Demos.FunnelViews.Funnel3dDemo",
					description: @"This demo illustrates a 3D Funnel series view, which is typically used to represent stages in a sales process and show the amount of potential revenue for each stage. A funnel chart displays values as progressively decreasing proportions. The size of the area is determined by the series value as a percentage of the total of all values, normally equal to the maximum value among the points' collection. In this demo, you can rotate the view using the mouse. The view also can be zoomed via the mouse wheel. To restore default angles, click the appropriate button. You can specify the position and text format of points' labels, define the ratio of the funnel's hole to its radius, the distance between points, the height-to-width ratio of the series, by using the appropriate editors.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\FunnelViews\3DFunnel.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\FunnelViews\3DFunnel.vb"
					}
				),
				#endregion
				#region Area Views
				new SimpleModule(demo,
					name: "Area",
					displayName: @"Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.AreaDemo",
					description: @"This demo illustrates the Area series view, which is intended to show trends for several series and also show the relationship of parts to a whole. In this demo you can change the kind and size of series markers, set the angle between labels and series markers, change the transparency of all Area series, as well as toggle the visibility of series labels and markers. In addition, this demo demonstrates multi-line labels for the X-axis.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\Area.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\Area.vb"
					}
				),
				new SimpleModule(demo,
					name: "StackedArea",
					displayName: @"Stacked Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StackedAreaDemo",
					description: @"This demo illustrates the series' Stacked Area View. This view is useful when it's necessary to compare both the points values and their aggregate for the same points arguments. In this demo you're able to change the visibility of series points labels, and enable or disable the selection of chart elements (axes, legend, etc.). You can also perform some specific actions (e.g., rotate a chart's diagram or specify the visibility of its items) via the chart's context menu.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\StackedArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\StackedArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullStackedArea",
					displayName: @"Full-Stacked Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.FullStackedAreaDemo",
					description: @"This demo illustrates the series' Full-Stacked Area View. This view is also called the 100% Stacked Area view, and is useful for comparing the percent values of several series for the same points arguments. In this demo you're able to specify whether real values are shown for each point label or the representative percentage, and also change the visibility of point labels. You can also enable the selection of chart elements (axes, legend, etc.), and right-click the chart or its elements to perform some specific actions.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\FullStackedArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\FullStackedArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "StepArea",
					displayName: @"Step Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StepAreaDemo",
					description: @"This demo illustrates the Step Area series view, which shows how much values have changed for different points of the same series. In this demo you can specify the type and size of series markers, set the angle between labels and series markers, invert the current step algorithm, as well as toggle the visibility of series labels and markers.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\StepArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\StepArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "SplineArea",
					displayName: @"Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.SplineAreaDemo",
					description: @"This demo shows a Spline Area series view, which is similar to the Area chart, but plots a fitted curve through each data point in a series. In this demo, you can specify the type and size of series point markers, change their visibility and set an angle between labels and point markers. Also, you can specify the transparency value for all Area series simultaneously.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\SplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\SplineArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "StackedSplineArea",
					displayName: @"Stacked Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StackedSplineAreaDemo",
					description: @"This demo shows a Stacked Spline Area series view, which is similar to the Stacked Area chart, but plots a fitted curve through each data point in a series. In this demo, you can specify the visibility of series point labels, and enable or disable the selection of chart elements (axes, legend, etc.). You can also perform some specific actions (e.g., rotate a chart's diagram or specify the visibility of its items) via the chart's context menu.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\StackedSplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\StackedSplineArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "FullStackedSplineArea",
					displayName: @"Full-Stacked Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.FullStackedSplineAreaDemo",
					description: @"This demo shows a Full-Stacked Spline Area series view, which is similar to the Full-Stacked Area chart, but plots a fitted curve through each data point in a series. In this demo, you can specify whether real values are shown for each point label or the representative percentage, and change the visibility of point labels. You can also enable the selection of chart elements, and right-click on the chart or its elements to perform some specific actions.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\FullStackedSplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\FullStackedSplineArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DArea",
					displayName: @"3D Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.Area3dDemo",
					description: @"This demo illustrates the 3D series' Area View, which is useful when you need to show trends for several series on the same diagram and also show the relationship of parts to a whole. In this demo you can specify series' transparency and perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStackedArea",
					displayName: @"3D Stacked Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StackedArea3dDemo",
					description: @"This demo illustrates the 3D series' Stacked Area View. This view is useful when it's necessary to compare both the points values and their aggregate for the same points arguments. In this demo you can specify series' transparency and perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DStackedArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DStackedArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFullStackedArea",
					displayName: @"3D Full-Stacked Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.FullStackedArea3dDemo",
					description: @"This demo illustrates the 3D series' Full-Stacked Area View. This view is also called the 100% Stacked Area view, and is useful for comparing the percent values of several series for the same points arguments. In this demo you can specify series' transparency and perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DFullStackedArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DFullStackedArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStepArea",
					displayName: @"3D Step Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StepArea3dDemo",
					description: @"This demo illustrates the 3D Step Area series view, which shows how much values have changed for different points of the same series. In this demo you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DStepArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DStepArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DSplineArea",
					displayName: @"3D Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.SplineArea3dDemo",
					description: @"This demo shows a 3D Spline Area series view, which is similar to the 3D Area chart, but plots a fitted curve through each data point in a series. In this demo, you can specify series' transparency and perspective angle. The view can be also rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DSplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DSplineArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DStackedSplineArea",
					displayName: @"3D Stacked Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.StackedSplineArea3dDemo",
					description: @"This demo shows a 3D Stacked Spline Area series view, which is similar to the 3D Stacked Area chart, but plots a fitted curve through each data point in a series. In this demo, you can specify series' transparency and perspective angle. The view can be also rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DStackedSplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DStackedSplineArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DFullStackedSplineArea",
					displayName: @"3D Full-stacked Spline Area",
					group: "Area Views",
					type: "DevExpress.XtraCharts.Demos.AreaViews.FullStackedSplineArea3dDemo",
					description: @"This demo shows a 3D Full-Stacked Spline Area series view, which is similar to the 3D Full-Stacked Area chart, but plots a fitted curve through each data point in a series. In this demo you can specify series' transparency and perspective angle. The view can be also rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AreaViews\3DFullStackedSplineArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AreaViews\3DFullStackedSplineArea.vb"
					}
				),
				#endregion
				#region Range Views
				new SimpleModule(demo,
					name: "RangeBar",
					displayName: @"Range Bar",
					group: "Range Views",
					type: "DevExpress.XtraCharts.Demos.RangeViews.RangeBarDemo",
					description: @"This demo illustrates an Overlapped Range Bar series view, which shows activity bars from different series one above another, to compare their duration. To rotate the diagram, right-click the chart, and toggle the ""Rotated"" option.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RangeViews\RangeBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RangeViews\RangeBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "SideBySideRangeBar",
					displayName: @"Range Bar Side-by-Side",
					group: "Range Views",
					type: "DevExpress.XtraCharts.Demos.RangeViews.SideBySideRangeBarDemo",
					description: @"This demo illustrates a Side-by-Side Range Bar series view, which shows activity bars from different series grouped by their arguments. To rotate the diagram, right-click the chart, and toggle the ""Rotated"" option.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RangeViews\SideBySideRangeBar.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RangeViews\SideBySideRangeBar.vb"
					}
				),
				new SimpleModule(demo,
					name: "RangeArea",
					displayName: @"Range Area",
					group: "Range Views",
					type: "DevExpress.XtraCharts.Demos.RangeViews.RangeAreaDemo",
					description: @"This demo illustrates the Range Area series view, which illustrates the difference between start and end values in a convenient manner. In this demo you can unhide markers for Value1 and Value 2, as well as change their kind and size. In addition, you can specify what should be shown in series labels, as well as toggle their visibility.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RangeViews\RangeArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RangeViews\RangeArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DRangeArea",
					displayName: @"3D Range Area",
					group: "Range Views",
					type: "DevExpress.XtraCharts.Demos.RangeViews.RangeArea3dDemo",
					description: @"This demo illustrates the 3D Range Area series view, which illustrates the difference between start and end values in a convenient manner. In this demo you can specify the view's perspective angle. The view can also be rotated using a mouse, and zoomed via the mouse wheel. To restore default angles, click the ""Restore Default Angles"" button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RangeViews\3DRangeArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RangeViews\3DRangeArea.vb"
					}
				),
				#endregion
				#region Radar/Polar Views
				new SimpleModule(demo,
					name: "RadarPoint",
					displayName: @"Radar Point",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.RadarPointDemo",
					description: @"This demo illustrates a Radar Point series view, which plots the average temperature in London. Months are charted on the angular axis, while the temperature is charted on the radial axis. A Radar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\RadarPoint.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\RadarPoint.vb"
					}
				),
				new SimpleModule(demo,
					name: "RadarLine",
					displayName: @"Radar Line",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.RadarLineDemo",
					description: @"This demo illustrates a Radar Line series view, which plots the average temperature in London. Months are charted on the angular axis, while the temperature is charted on the radial axis. A Radar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\RadarLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\RadarLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "RadarArea",
					displayName: @"Radar Area",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.RadarAreaDemo",
					description: @"This demo illustrates a Radar Area series view, which plots the average temperature in London. Months are charted on the angular axis, while the temperature is charted on the radial axis. A Radar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\RadarArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\RadarArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "ScatterRadarLine",
					displayName: @"Scatter Radar Line",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.ScatterRadarLineDemo",
					description: @"This demo illustrates a Scatter Radar Line series view, which displays function values in the same order that they have in the series points collection. In this demo, you can draw values of the Archimedean Spiral, Polar Rose or Polar Folium function. A Radar view can be drawn on a circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\ScatterRadarLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\ScatterRadarLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "PolarPoint",
					displayName: @"Polar Point",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.PolarPointDemo",
					description: @"This demo illustrates a Polar Point series view, which plots function values on the basis of angles. In this demo, you can plot the values of Lemniscate, Cardioid or Circles function. A Polar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\PolarPoint.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\PolarPoint.vb"
					}
				),
				new SimpleModule(demo,
					name: "PolarLine",
					displayName: @"Polar Line",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.PolarLineDemo",
					description: @"This demo illustrates a Polar Line series view, which plots function values on the basis of angles. In this demo, you can plot the values of Lemniscate, Cardioid or Circles function. A Polar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\PolarLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\PolarLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "PolarArea",
					displayName: @"Polar Area",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.PolarAreaDemo",
					description: @"This demo illustrates a Polar Area series view, which plots function values on the basis of angles. In this demo, you can plot the values of Lemniscate, Cardioid or Circles function. A Polar view can be drawn on circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\PolarArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\PolarArea.vb"
					}
				),
				new SimpleModule(demo,
					name: "ScatterPolarLine",
					displayName: @"Scatter Polar Line",
					group: "Radar/Polar Views",
					type: "DevExpress.XtraCharts.Demos.RadarPolarViews.ScatterPolarLineDemo",
					description: @"This demo illustrates a Scatter Polar Line series view, which displays function values in the same order that they have in the series points collection. In this demo, you can draw values of the Archimedean Spiral, Polar Rose or Polar Folium function. A Polar view can be drawn on a circular or polygon grid. This can be selected using the ""Diagram Style"" combo box.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\RadarPolarViews\ScatterPolarLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\RadarPolarViews\ScatterPolarLine.vb"
					}
				),
				#endregion
				#region Advanced Views
				new SimpleModule(demo,
					name: "Stock",
					displayName: @"Stock",
					group: "Advanced Views",
					type: "DevExpress.XtraCharts.Demos.AdvancedViews.StockDemo",
					description: @"This demo illustrates a Stock series view, which is also called the Low-High-Open-Close chart. It shows the stock price variation over the course of a day. The Open and Close prices are represented by left and right lines at each point, and the Low and High prices are represented by the bottom and top values of the vertical line shown at each point. In this demo, you can choose whether to exclude holidays and weekends from the X-axis scale. Also, you can select to which value level point labels are shown, specify if either or both Open and Close values are shown, and choose what value should be used to determine if the current value is less than its previous value (Red), or greater than (Black).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AdvancedViews\Stock.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AdvancedViews\Stock.vb"
					}
				),
				new SimpleModule(demo,
					name: "CandleStick",
					displayName: @"Candle Stick",
					group: "Advanced Views",
					type: "DevExpress.XtraCharts.Demos.AdvancedViews.CandleStickDemo",
					description: @"This demo illustrates a Candle Stick series view, which is also called the Low-High-Open-Close chart. It shows the stock price variation over the course of a day. The Open and Close prices are represented by a filled rectangle, and the Low and High prices are represented by the bottom and top values of the vertical line shown at each point. In this demo, you can choose whether to exclude holidays and weekends from the X-axis scale. Also, you can select to which value level point labels are shown, specify if either or both Open and Close values are shown, and choose what value should be used to determine if the current value is less than its previous value (Red), or greater than (Black).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AdvancedViews\CandleStick.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AdvancedViews\CandleStick.vb"
					}
				),
				new SimpleModule(demo,
					name: "Gantt",
					displayName: @"Gantt",
					group: "Advanced Views",
					type: "DevExpress.XtraCharts.Demos.AdvancedViews.GanttDemo",
					description: @"This demo illustrates a Gantt series view, which shows activity bars from different series one above another, to compare their duration. In this demo, you can drag the Progress Line, to change the current state of the ""Completed"" bars.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AdvancedViews\Gantt.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AdvancedViews\Gantt.vb"
					}
				),
				new SimpleModule(demo,
					name: "GanttSideBySide",
					displayName: @"Gantt Side-by-Side",
					group: "Advanced Views",
					type: "DevExpress.XtraCharts.Demos.AdvancedViews.GanttSideBySideDemo",
					description: @"This demo illustrates a Side-by-Side Gantt series view, which shows activity bars from different series grouped by their arguments. In this demo, to change the duration of a current plan for a project, move the mouse pointer to the starting or ending edge of its bar, and drag it.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\AdvancedViews\GanttSideBySide.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\AdvancedViews\GanttSideBySide.vb"
					}
				),
				#endregion
				#region View Combinations
				new SimpleModule(demo,
					name: "2DBarAndLine",
					displayName: @"2D Bar&Line",
					group: "View Combinations",
					type: "DevExpress.XtraCharts.Demos.ViewCombinations.BarAndLine2dDemo",
					description: @"This demo illustrates the combination of a 2D Bar and Line series views. In the same chart, you can display an unlimited number of different series, if the diagram type they support is similar (e.g. the XY-diagram in this example). In this demo, you can change the visibility of the labels shown for each series point, and enable or disable the selection of chart elements (axes, legend, etc.). You can also right-click the chart or its elements to perform some specific actions (e.g., rotate the chart's diagram or specify the visibility of its items).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ViewCombinations\2DBarAndLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ViewCombinations\2DBarAndLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "3DBarAndArea",
					displayName: @"3D Bar&Area",
					group: "View Combinations",
					type: "DevExpress.XtraCharts.Demos.ViewCombinations.BarAndArea3dDemo",
					description: @"This demo illustrates the combination of 3D Bar and Area series views. In the same chart, you can display an unlimited number of different series, if the diagram type they support is similar (e.g. the XY-diagram in this example). In addition, this demo demonstrates the capability to zoom and scroll a 3D chart.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ViewCombinations\3DBarAndArea.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ViewCombinations\3DBarAndArea.vb"
					}
				),
				#endregion
				#region Data Binding
				new SimpleModule(demo,
					name: "BindingIndividualSeries",
					displayName: @"Binding Individual Series",
					group: "Data Binding",
					type: "DevExpress.XtraCharts.Demos.DataBinding.BindingIndividualSeriesDemo",
					description: @"This demo illustrates one of the data binding approaches introduced in XtraCharts. First it's necessary to create static series in the chart, and then bind each of them to data. In this demo you're able to filter the series data by category, and also to set by what the data is sorted and the sort order. The selected category, its minimum, maximum and average prices are displayed at the chart's top-left corner. This information is dynamically updated for the selected category. To do this, the BoundDataChanged event is handled after a new filter is applied.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataBinding\BindingIndividualSeries.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataBinding\BindingIndividualSeries.vb"
					}
				),
				new SimpleModule(demo,
					name: "UsingSeriesTemplates",
					displayName: @"Using Series Templates",
					group: "Data Binding",
					type: "DevExpress.XtraCharts.Demos.DataBinding.UsingSeriesTemplatesDemo",
					description: @"This demo illustrates one of the data binding approaches introduced in XtraCharts. In this way the series data within series names are stored in the same datasource, and the series are all generated automatically after a chart has been bound to data. In this demo you're able to select the data member used to generate series. If the series member is ""Year"" then the chart shows GSP per Region for each Year, and if the series member is ""Region"" then the chart shows GSP per Year for each Region. The number of series and the selected series' name are displayed at the view's top left corner. Since the XtraCharts Suite provides runtime data bound series point access, it's possible to obtain point values and arguments. In this demo, click a series point to display its argument and value.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataBinding\UsingSeriesTemplates.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataBinding\UsingSeriesTemplates.vb"
					}
				),
				new SimpleModule(demo,
					name: "Colorizer",
					displayName: @"Colorizer",
					group: "Data Binding",
					type: "DevExpress.XtraCharts.Demos.DataBinding.ColorizerDemo",
					description: @"This demo illustrates the capability of the chart control to color its series based on bound data. In this demo, the chart control shows the Happy Planet Index (HPI) for G20 on the Bubble chart. The size of bubbles is proportional to the country population and their color is determined depending on the HPI value. The data for this demo is provided by www.happyplanetindex.org",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataBinding\Colorizer.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataBinding\Colorizer.vb"
					}
				),
				new SimpleModule(demo,
					name: "Summarization",
					displayName: @"Summarization",
					group: "Data Binding",
					type: "DevExpress.XtraCharts.Demos.DataBinding.SummarizationDemo",
					description: @"With XtraCharts, it is possible to automatically calculate and display summaries for a chart's datasource fields. In this demo, you can use a drop-down list above the chart to select a summary function to demonstrate. Note that in addition to built-in summary functions, you can create your own custom ones with XtraCharts, like the STDDEV function in this demo.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataBinding\Summarization.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataBinding\Summarization.vb"
					}
				),
				new SimpleModule(demo,
					name: "TagProperty",
					displayName: @"Tag Property",
					group: "Data Binding",
					type: "DevExpress.XtraCharts.Demos.DataBinding.TagPropertyDemo",
					description: @"This demo illustrates how to use the Tag property of the SeriesPoint object. In this example, the Tag property stores information from the underlying datasource (a country's official name), and then its value is used to populate legend items for corresponding series points. This feature is very useful when it's necessary to store some specific information associated with a particular series point.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataBinding\TagProperty.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataBinding\TagProperty.vb"
					}
				),
				#endregion
				#region Data Aggregation
				new SimpleModule(demo,
					name: "NumericDataAggreagation",
					displayName: @"Numeric Data Aggregation",
					group: "Data Aggregation",
					type: "DevExpress.XtraCharts.Demos.DataAggregation.NumericDataAggreagationDemo",
					description: @"This demo illustrates the capability of a chart to aggregate a large amount of data for the numeric scale type.  Zoom in/ out the diagram (by clicking a view and using the mouse wheel) and see how all data points are automatically aggregated on the Line chart.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAggregation\NumericDataAggreagation.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAggregation\NumericDataAggreagation.vb"
					}
				),
				new SimpleModule(demo,
					name: "DateTimeDataAggregation",
					displayName: @"Date-Time Data Aggregation",
					group: "Data Aggregation",
					type: "DevExpress.XtraCharts.Demos.DataAggregation.DateTimeDataAggregationDemo",
					description: @"This demo illustrates the capability of a chart to aggregate a large amount of data for the date-time scale type.  You can zoom in/out the diagram (by clicking a view and using the mouse wheel)  and  see how all data points are automatically aggregated on multiple Bar series.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAggregation\DateTimeDataAggregation.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAggregation\DateTimeDataAggregation.vb"
					}
				),
				new SimpleModule(demo,
					name: "CurrencyExcangeRates",
					displayName: @"Currency Exchange Rates",
					group: "Data Aggregation",
					type: "DevExpress.XtraCharts.Demos.DataAggregation.CurrencyExcangeRatesDemo",
					description: @"This demo illustrates the capability of a chart to aggregate a large amount of data for the date-time scale type.  You can zoom in/ out the diagram (by clicking a view and using the mouse wheel)  and change the visible axis range using the Range control to see how all data points are automatically aggregated on multiple Line series.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAggregation\CurrencyExcangeRates.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAggregation\CurrencyExcangeRates.vb"
					}
				),
				#endregion
				#region Data Analysis
				new SimpleModule(demo,
					name: "RegressionLine",
					displayName: @"Regression Line",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.RegressionLineDemo",
					description: @"This demo illustrates the line of linear regression analysis, which can be enabled for each value level of a series. In this demo, points are randomly generated after you click ""Create Points"". Then, based on the linear regression algorithm, a line is drawn across the points, providing a commonly used analytical capability. In addition, you can specify the color, dash style and thickness of the regression line.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\RegressionLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\RegressionLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "Trendlines",
					displayName: @"Trendlines",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.TrendlinesDemo",
					description: @"This demo illustrates how a trendline can be drawn between two series points. To define a starting date point for the trend analysis, click the chart and then drag the pointer to a series point, which is used to specify a trendline direction. A new trendline will be created using properties defined by controls above the chart. For the selected trendline, you can define the start and end value levels, as well as its color, using the corresponding drop-down lists above the chart, and check whether the trendline must extrapolate to infinity. Note that in this demo, holidays and weekends are excluded from the X-axis scale, via its WorkdaysOnly and WorkdaysOptions properties.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\Trendlines.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\Trendlines.vb"
					}
				),
				new SimpleModule(demo,
					name: "FibonacciIndicators",
					displayName: @"Fibonacci Indicators",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.FibonacciIndicatorsDemo",
					description: @"This demo demonstrates such commonly used financial indicators as Fibonacci Arcs, Fibonacci Fans and Fibonacci Retracement. In this demo, you can select the kind if indicator using the Kind drop-down list, and choose which of the values, available for this kind, should be drawn on the chart. Note that in this demo, holidays and weekends are excluded from the X-axis scale, via its WorkdaysOnly and WorkdaysOptions properties.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\FibonacciIndicators.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\FibonacciIndicators.vb"
					}
				),
				new SimpleModule(demo,
					name: "MovingAverage",
					updatedIn: KnownDXVersion.V152,
					displayName: @"Moving Average",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.MovingAverageDemo",
					description: @"This demo illustrates the Moving Average and Envelope financial indicators, that are commonly used in charting data for financial analysis. In this demo, use the Kind selector, to choose which indicator to show (Moving Average, Envelope, or both), and the Type selector, to choose the indicator's type (Simple, Exponential, Weighted, Triangular or Triple). To specify the number of days to be charted, use the Days Count editor. And, for the Envelope indicator, you can use the Envelope Percent editor, to define the indicator's scope.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\MovingAverage.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\MovingAverage.vb"
					}
				),
				new SimpleModule(demo,
					name: "TrendIndicators",
					displayName: @"Trend Indicators",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.TrendIndicatorsDemo",
					description: @"This demo demonstrates the Trend financial indicators. Currently the Chart Control supports the following indicators: Bollinger Bands, Mass Index, and Standard Deviation. ",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\TrendIndicators.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\TrendIndicators.vb"
					}
				),
				new SimpleModule(demo,
					name: "PriceIndicators",
					displayName: @"Price Indicators",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.PriceIndicatorsDemo",
					description: @"This demo shows the Price financial indicators. Currently the Chart Control supports the following indicators: Median Price, Typical Price, and Weighted Close. To specify the thickness of the indicator line, use the Thickness editor. To change the line’s color and style, use the Color and Dash Style editors, respectively.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\PriceIndicators.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\PriceIndicators.vb"
					}
				),
				new SimpleModule(demo,
					name: "Oscillators",
					displayName: @"Oscillator Indicators",
					group: "Data Analysis",
					type: "DevExpress.XtraCharts.Demos.DataAnalysis.OscillatorsDemo",
					description: @"This demo illustrates the Oscillator financial indicators. Currently the Chart Control supports the following indicators: Average True Range, Commodity Channel Index, Detrended Price Oscillator, Moving Average Convergence Divergence, Rate of Change, Relative Strength Index, Chaikins Volatility, and Williams % R.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\DataAnalysis\Oscillators.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\DataAnalysis\Oscillators.vb"
					}
				),
				#endregion
				#region Chart Elements
				new SimpleModule(demo,
					name: "VerticalPanes",
					displayName: @"Vertical Panes",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.VerticalPanesDemo",
					description: @"With XtraCharts, you can create a chart with multiple panes placed vertically and use them to display different series separately, each series in its own pane. Scroll the diagram and see that all panes are scrolled synchronously with each other and with the argument axis.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 2,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\VerticalPanes.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\VerticalPanes.vb"
					}
				),
				new SimpleModule(demo,
					name: "HorizontalPanes",
					displayName: @"Horizontal Panes",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.HorizontalPanesDemo",
					description: @"With XtraCharts, you can create a chart with multiple panes placed horizontally and use them to display different series separately, each series in its own pane. In this demo, you can use drop-down lists above the chart to select the required categories to be displayed in left and right panes.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\HorizontalPanes.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\HorizontalPanes.vb"
					}
				),
				new SimpleModule(demo,
					name: "ChartTitles",
					displayName: @"Chart Titles",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.ChartTitlesDemo",
					description: @"This demo illustrates chart titles. With XtraCharts, it is possible to create an unlimited number of titles on any chart side and fully customize their appearance. To customize any chart title, first select it and then change its alignment, dock or text. Note that you can use some HTML tags for text formatting (<b>bold</b>, <i>italic</i>, <u>underlined</u>, <color=blue>color</color>, <size=+2>size</size>). To define whether or not the word-wrapping should be applied to lengthy chart titles, use the appropriate check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\ChartTitles.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\ChartTitles.vb"
					}
				),
				new SimpleModule(demo,
					name: "SeriesTitles",
					displayName: @"Series Titles",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.SeriesTitlesDemo",
					description: @"This demo illustrates how to show individual titles for each Pie (or Doughnut) series. These titles can be specified via the Titles property of a particular pie/doughnut series view, or automatically generated using a display pattern for a title, which is added to a Titles collection of a series template. In this demo, every series title is generated using the ""GSP in {S}"" pattern, where {S} is changed to the current series name, obtained from the underlying datasource.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\SeriesTitles.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\SeriesTitles.vb"
					}
				),
				new SimpleModule(demo,
					name: "SecondaryAxes",
					displayName: @"Secondary Axes",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.SecondaryAxesDemo",
					description: @"This demo illustrates how to add secondary axes to your chart. This may be required, for instance, when it's necessary to show arguments data of a single series using a particular axis, and arguments data of another series using a different axis. In this demo you're able to show or hide both X and Y secondary axes, to see their visibility in effect. Also you may rotate the chart's diagram via the chart's context menu, and perform some other specific actions.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\SecondaryAxes.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\SecondaryAxes.vb"
					}
				),
				new SimpleModule(demo,
					name: "ScaleBreaks",
					displayName: @"Scale Breaks",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.ScaleBreaksDemo",
					description: @"This demo illustrates how an axis range can be divided by scale breaks, to remove useless space that appears when series points' values have a significant difference in ranges. In this demo, you can choose to enable automatic scale breaks, and define a limit for their count (up to four). The appearance of scale breaks is determined by their style, size, and color.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\ScaleBreaks.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\ScaleBreaks.vb"
					}
				),
				new SimpleModule(demo,
					name: "Legend",
					displayName: @"Legend",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.LegendDemo",
					description: @"This demo illustrates how you can customize a legend in XtraCharts. Try to resize a chart's window to see that a legend always tries to fit its height and width less than the specified maximum height and width values (in percents respective to a chart's size). In addition, in this demo you're able to specify a legend's alignment, direction, and how legend items are spaced in case they're arranged into several columns.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\Legend.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\Legend.vb"
					}
				),
				new SimpleModule(demo,
					name: "CheckBoxesInLegend",
					displayName: @"Check Boxes in Legend",
					group: "Chart Elements",
					type: "DevExpress.XtraCharts.Demos.ChartElements.CheckBoxesInLegendDemo",
					description: @"This demo illustrates the capability to show legend check boxes, which are used to toggle visibility of various chart elements.  Note that when you disable a series check box, the corresponding series trend is automatically disabled, too. To activate/deactivate check boxes in a legend, use the ""Use Check Boxes in Legend"" option. Also, you can deactivate check boxes for certain elements – e.g., disable the ""Checkable Indicators"" option to show markers instead of check boxes for financial indicators.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ChartElements\CheckBoxesInLegend.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ChartElements\CheckBoxesInLegend.vb"
					}
				),
				#endregion
				#region Custom Draw
				new SimpleModule(demo,
					name: "XYDiagramCustomPaint",
					displayName: @"XY-Diagram Custom Paint",
					group: "Custom Draw",
					type: "DevExpress.XtraCharts.Demos.CustomDraw.XYDiagramCustomPaintDemo",
					description: @"This demo demonstrates how to implement custom painting on an XY-Diagram, using its DiagramToPoint method in the ChartControl.CustomPaint event handler. In this demo, click Create Points to generate random points. When the Auto Mode is disabled, you can manually select a region on a chart, and the points that fall in the selected range are clustered, by connecting them with a custom painted shape. And, when Auto Mode is turned on, points are clustered automatically.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\CustomDraw\XYDiagramCustomPaint.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\CustomDraw\XYDiagramCustomPaint.vb"
					}
				),
				new SimpleModule(demo,
					name: "RadarDiagramCustomPaint",
					displayName: @"Radar Diagram Custom Paint",
					group: "Custom Draw",
					type: "DevExpress.XtraCharts.Demos.CustomDraw.RadarDiagramCustomPaintDemo",
					description: @"This demo demonstrates how to implement custom painting on an Radar Diagram, using its DiagramToPoint and PointToDiagram methods in the ChartControl.CustomPaint event handler. In this demo, three moving targets are painted on the diagram, and you can place the cross over a target and click to hit it.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\CustomDraw\RadarDiagramCustomPaint.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\CustomDraw\RadarDiagramCustomPaint.vb"
					}
				),
				new SimpleModule(demo,
					name: "SeriesPoints",
					displayName: @"Series Points",
					group: "Custom Draw",
					type: "DevExpress.XtraCharts.Demos.CustomDraw.SeriesPointsDemo",
					description: @"This demo illustrates how to implement custom drawing of series points in the Chart control for WinForms. Thanks to the ChartControl's CustomDrawSeriesPoint event, it's possible to get the color name of the series points and show it in series labels. Note that every interval of point values is represented by special chart elements – Strips. This demo is marked as 'updated' because the SeriesPoint.Color property has been introduced instead of series draw options to custom paint every series point.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\CustomDraw\SeriesPoints.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\CustomDraw\SeriesPoints.vb"
					}
				),
				new SimpleModule(demo,
					name: "AxisLabels",
					displayName: @"Axis Labels",
					group: "Custom Draw",
					type: "DevExpress.XtraCharts.Demos.CustomDraw.AxisLabelsDemo",
					description: @"This demo illustrates the capability to individually adjust the appearance of axis label items. In this demo, the values along the Y-axis have a different color and font size, depending on whether they are equal to, less than, or greater than zero. You can restore their default appearance, using the ""Custom Draw"" check box.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\CustomDraw\AxisLabels.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\CustomDraw\AxisLabels.vb"
					}
				),
				new SimpleModule(demo,
					name: "LegendItems",
					displayName: @"Legend Items",
					group: "Custom Draw",
					type: "DevExpress.XtraCharts.Demos.CustomDraw.LegendItemsDemo",
					description: @"This demo illustrates the capability to provide image legend markers for series points. In this demo, you can select a series point by pointing to the corresponding bar, or a legend item.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\CustomDraw\LegendItems.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\CustomDraw\LegendItems.vb"
					}
				),
				#endregion
				#region Annotations
				new SimpleModule(demo,
					name: "AnnotationLayout",
					displayName: @"Annotation Layout",
					group: "Annotations",
					type: "DevExpress.XtraCharts.Demos.Annotations.AnnotationLayoutDemo",
					description: @"This demo shows that an annotation’s layout can be adjusted interactively at runtime. In this demo, if you’ve enabled the corresponding option, you can move, resize, or rotate the annotations on the chart. If you also enable the Allow Anchoring option, you can move an annotation’s anchor point, providing that it has been anchored to the chart or pane. The annotations that highlight the maximum and minimum values are stationary since they are anchored to those points.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 6,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Annotations\AnnotationLayout.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Annotations\AnnotationLayout.vb"
					}
				),
				new SimpleModule(demo,
					name: "TextAnnotation",
					displayName: @"Text Annotation",
					group: "Annotations",
					type: "DevExpress.XtraCharts.Demos.Annotations.TextAnnotationDemo",
					description: @"This demo illustrates text annotations anchored to series points and highlighting points having the minimum and maximum values. To create random points, click the ""Generate Points"" button. In this demo, you can choose a shape and connector style for the annotations, and adjust the annotation's rotation angle. This demo also shows HTML formatting support in text annotations.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Annotations\TextAnnotation.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Annotations\TextAnnotation.vb"
					}
				),
				new SimpleModule(demo,
					name: "ImageAnnotation",
					displayName: @"Image Annotation",
					group: "Annotations",
					type: "DevExpress.XtraCharts.Demos.Annotations.ImageAnnotationDemo",
					description: @"This demo illustrates image annotations anchored to series points. To create random points, click the ""Generate Points"" button. In this demo, you can choose a shape and connector style for the annotations, and adjust the annotation's rotation angle.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Annotations\ImageAnnotation.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Annotations\ImageAnnotation.vb"
					}
				),
				#endregion
				#region Resolve Labels Overlapping
				new SimpleModule(demo,
					name: "ResolveOverlappingOfAxisLabels",
					displayName: @"Resolve Overlapping of Axis Labels",
					group: "Resolve Labels Overlapping",
					type: "DevExpress.XtraCharts.Demos.ResolveLabelsOverlapping.ResolveOverlappingOfAxisLabelsDemo",
					description: @"In this demo you can see the work of a resolve overlapping algorithm that prevents intersection of axis labels by making labels staggered. In addition, this algorithm provides rotation and hiding overlapped axis labels. To see how this works, resize this demo. You can also disable/enable the Resolve Overlapping option and specify the resolve overlapping indent between neighboring axis labels to achieve better results.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingOfAxisLabels.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingOfAxisLabels.vb"
					}
				),
				new SimpleModule(demo,
					name: "ResolveOverlappingForLine",
					displayName: @"Resolve Overlapping for Line",
					group: "Resolve Labels Overlapping",
					type: "DevExpress.XtraCharts.Demos.ResolveLabelsOverlapping.ResolveOverlappingForLineDemo",
					description: @"This demo illustrates the XtraCharts resolve overlapping feature, which is intended to keep labels from overlapping. In this demo, you can choose an overlapping resolving mode (a particular algorithm to be applied to points' labels), via the ""Mode"" combo box, and specify the angle at which labels are rotated, and the length of their lines. If any mode other than ""None"" is applied, you can also specify the minimum indent between adjacent labels. Note the improved performance, thanks to the changes made to the algorithms' programming interface.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingForLine.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingForLine.vb"
					}
				),
				new SimpleModule(demo,
					name: "ResolveOverlappingFor3DPie",
					displayName: @"Resolve Overlapping for 3D Pie",
					group: "Resolve Labels Overlapping",
					type: "DevExpress.XtraCharts.Demos.ResolveLabelsOverlapping.ResolveOverlappingFor3dPieDemo",
					description: @"This demo illustrates the 3D Pie resolve overlapping feature, which is intended to avoid labels overlapping. In this demo, you can check whether to resolve overlapping, and, if so, to define the required indent value, using the appropriate controls above the chart. Rotate the diagram using the mouse, to see this feature in action.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingFor3DPie.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\ResolveLabelsOverlapping\ResolveOverlappingFor3DPie.vb"
					}
				),
				#endregion
				#region Miscellaneous
				new SimpleModule(demo,
					name: "Selection",
					displayName: @"Selection",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.SelectionDemo",
					description: @"This demo illustrates the chart selection feature shipping as part of the DevExpress WinForms Chart Control. Use mouse or touch gestures to select US regions (pie segments) in the Pie chart and view GSP for these regions in the Bar chart. You can change current selection mode to Single (to select one pie segment), Extended (to select multiple pie segments while the SHIFT key is pressed) and select/deselect pie segments (while the Ctrl key is pressed) or None (selection is disabled). To deselect a pie segment, click on any region outside of the Pie chart.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\Selection.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\Selection.vb"
					}
				),
				new SimpleModule(demo,
					name: "DateTimeScale",
					displayName: @"Date-Time Scale",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.DateTimeScaleDemo",
					description: @"This demo illustrates how a chart can display date-time values according to the currently selected measure unit and grid alignment. This allows you to maintain date-time values at different detail levels. To change a measure unit, switch the scale mode from Automatic (in which the most optimal unit of measure for an axis is calculated automatically based on input data) to Manual (in which you can specify a measure unit and aggregate function). To disable data aggregation, select the Continuous scale mode.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\DateTimeScale.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\DateTimeScale.vb"
					}
				),
				new SimpleModule(demo,
					name: "ToolTips",
					displayName: @"ToolTips",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.ToolTipsDemo",
					description: @"XtraCharts provide built-in tooltips to show additional information for hovered chart elements. This demo illustrates how to implement custom tooltips for series points. In this demo, a tooltip appears for each bar and shows another chart with a GDP history for the selected country.In this demo, you can choose one of the following tooltip positions: Mouse Pointer (a tooltip is placed near the mouse pointer), Relative (a tooltip is placed near the element, for which it was invoked) and Free (a tooltip is always placed in the predefined position).",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\ToolTips.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\ToolTips.vb"
					}
				),
				new SimpleModule(demo,
					name: "HitTesting",
					displayName: @"Hit-Testing",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.HitTestingDemo",
					description: @"This demo illustrates the interactive capabilities of XtraCharts, in particular, a capability to access any chart's element by hit-testing. In this demo, after the mouse cursor crosses over a chart's element, a tooltip appears which specifies the object which has been highlighted, and shows its underlying data (if any). If there are multiple objects layered under a test-point, their hierarchy is reflected. This demo is marked as ""updated"" because a hyperlink element has become accessible by hit-testing.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\HitTesting.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\HitTesting.vb"
					}
				),
				new SimpleModule(demo,
					name: "DrillDown",
					displayName: @"Drill Down",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.DrillDownDemo",
					description: @"This demo illustrates how to create a drill-down chart. The main chart is a Pie, whose series points display min, max or average price in every category. Clicking any pie slice invokes the Bar chart, which displays all product prices in the selected category. In the detail chart you're able to click the ""Back to the main view..."" hyperlink, and the initial Pie chart will be restored.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\DrillDown.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\DrillDown.vb"
					}
				),
				new SimpleModule(demo,
					name: "ScrollingAndZooming",
					displayName: @"Scrolling and Zooming",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.ScrollingAndZoomingDemo",
					description: @"This demo illustrates scrolling and zooming features. You can enable scrolling and/or zooming individually for each axis, and adjust the position of both the horizontal and vertical scroll bars. A view can be scrolled using scrollbars, the CTRL with arrow keys, or by clicking a diagram and dragging it in the required direction. To zoom into a view, do one of the following: press the SHIFT key and click a diagram or select a region, press the CTRL and ""+"" key combination, use a mouse wheel. To zoom out: press the ALT key and click a diagram, press the CTRL and ""-"" key combination, use the mouse wheel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\ScrollingAndZooming.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\ScrollingAndZooming.vb"
					}
				),
				new SimpleModule(demo,
					name: "EmptyPoints",
					displayName: @"Empty Points",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.EmptyPointsDemo",
					description: @"This demo illustrates how XtraCharts processes missing values, which are often caused by skipped tests or measurements. These values are transformed into empty points represented by breaks in line/area graphs or missing points, bars etc. in other series view types. In this demo, you're able to change the current series view type to see how empty points are represented in different series views.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\EmptyPoints.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\EmptyPoints.vb"
					}
				),
				new SimpleModule(demo,
					name: "TopNAndOthers",
					displayName: @"""Top N"" and ""Others""",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.TopNAndOthersDemo",
					description: @"This demo illustrates the XtraCharts capability to show only ""Top N"" values and aggregate the rest into the ""Others"" category. In this demo, you can use the Mode drop-down list to define a rule for values to fall within ""Top N"" or ""Others"" category. Also, you can choose whether to display the ""Others"" argument, and define a custom name for it.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\TopNAndOthers.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\TopNAndOthers.vb"
					}
				),
				new SimpleModule(demo,
					name: "LogarithmicScale",
					displayName: @"Logarithmic Scale",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.LogarithmicScaleDemo",
					description: @"This demo illustrates the logarithmic scale feature. This feature is usually required when it is necessary to show different values, if some of them are much greater than others. In this demo, you can turn the logarithmic scale on and off, and also select the required logarithmic base.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\LogarithmicScale.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\LogarithmicScale.vb"
					}
				),
				new SimpleModule(demo,
					name: "DisplayPatterns",
					displayName: @"Display Patterns",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.DisplayPatternsDemo",
					description: @"This demo shows how to use patterns to customize a text of series labels, legend items, and axis labels. In this demo you can use a predefined set of patterns from a drop-down list  (e.g., {A} - to show arguments;  {V} - to show values;  {S} - to show series name), or specify a custom  pattern by combining patterns in any order with prefixes, suffixes and postfixes.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\DisplayPatterns.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\DisplayPatterns.vb"
					}
				),
				new SimpleModule(demo,
					name: "EqualSizeForPiesDoughnuts",
					displayName: @"Equal Size for Pies/ Doughnuts",
					group: "Miscellaneous",
					type: "DevExpress.XtraCharts.Demos.Miscellaneous.EqualSizeForPiesDoughnutsDemo",
					description: @"This demo illustrates the equal size feature for Pie/ Doughnut series views. To see this feature in action, click ""Equal Size"" and all pies (doughnuts) will be equal by their minimum size. In this demo, you can also change the minimum allowed size as a percentage for the ""Condiment"" series. To restore the default layout, click the corresponding button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ChartsMainDemo\Modules\Miscellaneous\EqualSizeForPiesDoughnuts.cs",
						@"\WinForms\VB\ChartsMainDemo\Modules\Miscellaneous\EqualSizeForPiesDoughnuts.vb"
					}
				)
				#endregion
			};
		}
	}
}
