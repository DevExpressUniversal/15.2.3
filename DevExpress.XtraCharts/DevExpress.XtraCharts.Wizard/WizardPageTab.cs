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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Wizard {
	public class SeriesViewPageTabCollection : CollectionBase {
		internal SeriesViewPageTabCollection() {
		}
		public void Add(SeriesViewPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(SeriesViewPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(SeriesViewPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(SeriesViewPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class AxisPageTabCollection : CollectionBase {
		internal AxisPageTabCollection() {
		}
		public void Add(AxisPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(AxisPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(AxisPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(AxisPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class PanePageTabCollection : CollectionBase {
		internal PanePageTabCollection() {
		}
		public void Add(PanePageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(PanePageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(PanePageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(PanePageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class AnnotationPageTabCollection : CollectionBase {
		internal AnnotationPageTabCollection() {
		}
		public void Add(AnnotationPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(AnnotationPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(AnnotationPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(AnnotationPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class DiagramPageTabCollection : CollectionBase {
		internal DiagramPageTabCollection() {
		}
		public void Add(DiagramPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(DiagramPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(DiagramPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(DiagramPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class DataPageTabCollection : CollectionBase {
		internal DataPageTabCollection() {
		}
		public void Add(DataPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(DataPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(DataPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(DataPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class SeriesLabelsPageTabCollection : CollectionBase {
		internal SeriesLabelsPageTabCollection() {
		}
		public void Add(SeriesLabelsPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(SeriesLabelsPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(SeriesLabelsPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(SeriesLabelsPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class LegendPageTabCollection : CollectionBase {
		internal LegendPageTabCollection() {
		}
		public void Add(LegendPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(LegendPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(LegendPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(LegendPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class TitlePageTabCollection : CollectionBase {
		internal TitlePageTabCollection() {
		}
		public void Add(TitlePageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(TitlePageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(TitlePageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(TitlePageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class ChartPageTabCollection : CollectionBase {
		internal ChartPageTabCollection() {
		}
		public void Add(ChartPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(ChartPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(ChartPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(ChartPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public class SeriesPageTabCollection : CollectionBase {
		internal SeriesPageTabCollection() {
		}
		public void Add(SeriesPageTab hiddenTab) {
			InnerList.Add(hiddenTab);
		}
		public void AddRange(SeriesPageTab[] hiddenTabs) {
			InnerList.AddRange(hiddenTabs);
		}
		public void Remove(SeriesPageTab hiddenTab) {
			InnerList.Remove(hiddenTab);
		}
		internal bool Contains(SeriesPageTab tabPage) {
			return InnerList.Contains(tabPage);
		}
	}
	public enum SeriesViewPageTab {
		BarGeneral,
		BarAppearance,
		BarBorder,
		BarShadow,
		BarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BarIndicators field instead.")]
		BarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BarIndicators field instead.")]
		BarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BarIndicators field instead.")]
		BarRegressionLines,
		StackedBarGeneral,
		StackedBarAppearance,
		StackedBarBorder,
		StackedBarShadow,
		StackedBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedBarIndicators field instead.")]
		StackedBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedBarIndicators field instead.")]
		StackedBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedBarIndicators field instead.")]
		StackedBarRegressionLines,
		FullStackedBarGeneral,
		FullStackedBarAppearance,
		FullStackedBarBorder,
		FullStackedBarShadow,
		FullStackedBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedBarIndicators field instead.")]
		FullStackedBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedBarIndicators field instead.")]
		FullStackedBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedBarIndicators field instead.")]
		FullStackedBarRegressionLines,
		SideBySideStackedBarGeneral,
		SideBySideStackedBarAppearance,
		SideBySideStackedBarBorder,
		SideBySideStackedBarShadow,
		SideBySideStackedBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideStackedBarIndicators field instead.")]
		SideBySideStackedBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideStackedBarIndicators field instead.")]
		SideBySideStackedBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideStackedBarIndicators field instead.")]
		SideBySideStackedBarRegressionLines,
		SideBySideFullStackedBarGeneral,
		SideBySideFullStackedBarAppearance,
		SideBySideFullStackedBarBorder,
		SideBySideFullStackedBarShadow,
		SideBySideFullStackedBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideFullStackedBarIndicators field instead.")]
		SideBySideFullStackedBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideFullStackedBarIndicators field instead.")]
		SideBySideFullStackedBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideFullStackedBarIndicators field instead.")]
		SideBySideFullStackedBarRegressionLines,
		PieGeneral,
		PieAppearance,
		PieBorder,
		PieExplodedPoints,
		PieTitles,
		DoughnutGeneral,
		DoughnutAppearance,
		DoughnutBorder,
		DoughnutExplodedPoints,
		DoughnutTitles,
		NestedDoughnutGeneral,
		NestedDoughnutAppearance,
		NestedDoughnutBorder,
		NestedDoughnutExplodedPoints,
		NestedDoughnutTitles,
		FunnelGeneral,
		FunnelAppearance,
		FunnelBorder,
		FunnelTitles,
		PointGeneral,
		PointAppearance,
		PointShadow,
		PointMarker,
		PointIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.PointIndicators field instead.")]
		PointTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.PointIndicators field instead.")]
		PointFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.PointIndicators field instead.")]
		PointRegressionLines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.RadarPointAppearance field instead.")]
		RadarPointGeneral,
		RadarPointAppearance,
		RadarPointShadow,
		RadarPointMarker,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.PolarPointAppearance field instead.")]
		PolarPointGeneral,
		PolarPointAppearance,
		PolarPointShadow,
		PolarPointMarker,
		LineGeneral,
		LineAppearance,
		LineShadow,
		LineMarker,
		LineIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.LineIndicators field instead.")]
		LineTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.LineIndicators field instead.")]
		LineFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.LineIndicators field instead.")]
		LineRegressionLines,
		StackedLineGeneral,
		StackedLineAppearance,
		StackedLineShadow,
		StackedLineMarker,
		StackedLineIndicators,
		FullStackedLineGeneral,
		FullStackedLineAppearance,
		FullStackedLineShadow,
		FullStackedLineMarker,
		FullStackedLineIndicators,
		SplineGeneral,
		SplineAppearance,
		SplineShadow, 
		SplineMarker,
		SplineIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineIndicators field instead.")]
		SplineTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineIndicators field instead.")]
		SplineFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineIndicators field instead.")]
		SplineRegressionLines,
		StepLineGeneral,
		StepLineAppearance,
		StepLineShadow,
		StepLineMarker,
		StepLineIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StepLineIndicators field instead.")]
		StepLineTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StepLineIndicators field instead.")]
		StepLineFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StepLineIndicators field instead.")]
		StepLineRegressionLines,
		ScatterLineGeneral,
		ScatterLineAppearance,
		ScatterLineShadow,
		ScatterLineMarker,
		ScatterLineIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.ScatterLineIndicators field instead.")]
		ScatterLineTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.ScatterLineIndicators field instead.")]
		ScatterLineFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.ScatterLineIndicators field instead.")]
		ScatterLineRegressionLines,
		SwiftPlotGeneral,
		SwiftPlotAppearance,
		SwiftPlotIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SwiftPlotIndicators field instead.")]
		SwiftPlotTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SwiftPlotIndicators field instead.")]
		SwiftPlotFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SwiftPlotIndicators field instead.")]
		SwiftPlotRegressionLines,
		RadarLineGeneral,
		RadarLineMarker,
		RadarLineShadow,
		RadarLineAppearance,
		ScatterRadarLineGeneral,
		ScatterRadarLineMarker,
		ScatterRadarLineShadow,
		ScatterRadarLineAppearance,
		PolarLineGeneral,
		PolarLineMarker,
		PolarLineShadow,
		PolarLineAppearance,
		ScatterPolarLineGeneral,
		ScatterPolarLineMarker,
		ScatterPolarLineShadow,
		ScatterPolarLineAppearance,
		AreaGeneral,
		AreaAppearance,
		AreaBorder,
		AreaShadow,
		AreaMarker,
		AreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.AreaIndicators field instead.")]
		AreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.AreaIndicators field instead.")]
		AreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.AreaIndicators field instead.")]
		AreaRegressionLines,
		StepAreaGeneral,
		StepAreaAppearance,
		StepAreaBorder,
		StepAreaShadow,
		StepAreaMarker,
		StepAreaIndicators,
		SplineAreaGeneral,
		SplineAreaAppearance,
		SplineAreaBorder,
		SplineAreaShadow,
		SplineAreaMarker,
		SplineAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineAreaIndicators field instead.")]
		SplineAreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineAreaIndicators field instead.")]
		SplineAreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineAreaIndicators field instead.")]
		SplineAreaRegressionLines,
		StackedAreaGeneral,
		StackedAreaAppearance,
		StackedAreaBorder,
		StackedAreaShadow,
		StackedAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedAreaIndicators field instead.")]
		StackedAreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedAreaIndicators field instead.")]
		StackedAreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedAreaIndicators field instead.")]
		StackedAreaRegressionLines,
		StackedSplineAreaGeneral,
		StackedSplineAreaBorder,
		StackedSplineAreaAppearance,
		StackedSplineAreaShadow,
		StackedSplineAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedSplineAreaIndicators field instead.")]
		StackedSplineAreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedSplineAreaIndicators field instead.")]
		StackedSplineAreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedSplineAreaIndicators field instead.")]
		StackedSplineAreaRegressionLines,
		FullStackedAreaGeneral,
		FullStackedAreaAppearance,
		FullStackedAreaShadow,
		FullStackedAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedAreaIndicators field instead.")]
		FullStackedAreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedAreaIndicators field instead.")]
		FullStackedAreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedAreaIndicators field instead.")]
		FullStackedAreaRegressionLines,
		FullStackedSplineAreaGeneral,
		FullStackedSplineAreaAppearance,
		FullStackedSplineAreaShadow,
		FullStackedSplineAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedSplineAreaIndicators field instead.")]
		FullStackedSplineAreaTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedSplineAreaIndicators field instead.")]
		FullStackedSplineAreaFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedSplineAreaIndicators field instead.")]
		FullStackedSplineAreaRegressionLines,
		RangeAreaGeneral,
		RangeAreaAppearance,
		RangeAreaBorder,
		RangeAreaShadow,
		RangeAreaMarker1,
		RangeAreaMarker2,
		RangeAreaIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.RadarAreaAppearance field instead.")]
		RadarAreaGeneral,
		RadarAreaAppearance,
		RadarAreaBorder,
		RadarAreaShadow,
		RadarAreaMarker,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.PolarAreaAppearance field instead.")]
		PolarAreaGeneral,
		PolarAreaAppearance,
		PolarAreaBorder,
		PolarAreaShadow,
		PolarAreaMarker,
		StockGeneral,
		StockShadow,
		StockIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StockIndicators field instead.")]
		StockTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StockIndicators field instead.")]
		StockFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StockIndicators field instead.")]
		StockRegressionLines,
		CandleStickGeneral,
		CandleStickShadow,
		CandleStickIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.CandleStickIndicators field instead.")]
		CandleStickTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.CandleStickIndicators field instead.")]
		CandleStickFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.CandleStickIndicators field instead.")]
		CandleStickRegressionLines,
		SideBySideRangeBarGeneral,
		SideBySideRangeBarAppearance,
		SideBySideRangeBarBorder,
		SideBySideRangeBarShadow,
		SideBySideRangeBarMinMarker,
		SideBySideRangeBarMaxMarker,
		SideBySideRangeBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideRangeBarIndicators field instead.")]
		SideBySideRangeBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideRangeBarIndicators field instead.")]
		SideBySideRangeBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideRangeBarIndicators field instead.")]
		SideBySideRangeBarRegressionLines,
		RangeBarGeneral,
		RangeBarAppearance,
		RangeBarBorder,
		RangeBarShadow,
		RangeBarMinMarker,
		RangeBarMaxMarker,
		RangeBarIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.RangeBarIndicators field instead.")]
		RangeBarTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.RangeBarIndicators field instead.")]
		RangeBarFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.RangeBarIndicators field instead.")]
		RangeBarRegressionLines,
		SideBySideGanttGeneral,
		SideBySideGanttAppearance,
		SideBySideGanttBorder,
		SideBySideGanttShadow,
		SideBySideGanttMinMarker,
		SideBySideGanttMaxMarker,
		SideBySideGanttIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideGanttIndicators field instead.")]
		SideBySideGanttTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideGanttIndicators field instead.")]
		SideBySideGanttFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SideBySideGanttIndicators field instead.")]
		SideBySideGanttRegressionLines,
		GanttGeneral,
		GanttAppearance,
		GanttBorder,
		GanttShadow,
		GanttMinMarker,
		GanttMaxMarker,
		GanttIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.GanttIndicators field instead.")]
		GanttTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.GanttIndicators field instead.")]
		GanttFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.GanttIndicators field instead.")]
		GanttRegressionLines,
		Bar3DGeneral,
		Bar3DAppearance,
		StackedBar3DGeneral,
		StackedBar3DAppearance,
		FullStackedBar3DGeneral,
		FullStackedBar3DAppearance,
		ManhattanBarGeneral,
		ManhattanBarAppearance,
		SideBySideStackedBar3DGeneral,
		SideBySideStackedBar3DAppearance,
		SideBySideFullStackedBar3DGeneral,
		SideBySideFullStackedBar3DAppearance,
		Pie3DGeneral,
		Pie3DAppearance,
		Pie3DExplodedPoints,
		Pie3DTitles,
		Doughnut3DGeneral,
		Doughnut3DAppearance,
		Doughnut3DExplodedPoints,
		Doughnut3DTitles,
		Funnel3DGeneral,
		[Obsolete("This field is obsolete now.")]
		Funnel3DAppearance,
		Funnel3DTitles,
		Line3DGeneral,
		Line3DAppearance,
		Line3DMarkerLine,
		StackedLine3DGeneral,
		StackedLine3DAppearance,
		StackedLine3DMarkerLine,
		FullStackedLine3DGeneral,
		FullStackedLine3DAppearance,
		FullStackedLine3DMarkerLine,
		StepLine3DGeneral,
		StepLine3DAppearance,
		StepLine3DMarkerLine,		
		Area3DGeneral,
		Area3DAppearance,
		Area3DMarkerLine,
		StackedArea3DGeneral,
		StackedArea3DAppearance,
		StackedArea3DMarkerLine,
		FullStackedArea3DGeneral,
		FullStackedArea3DAppearance,
		FullStackedArea3DMarkerLine,
		StepArea3DGeneral,
		StepArea3DAppearance,
		StepArea3DMarkerLine,
		Spline3DGeneral,
		Spline3DAppearance,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.Spline3DMarkerLine field instead.")]
		Spline3DMarker,
		Spline3DMarkerLine,
		SplineArea3DGeneral,
		SplineArea3DAppearance,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.SplineArea3DMarkerLine field instead.")]
		SplineArea3DMarker,
		SplineArea3DMarkerLine,
		StackedSplineArea3DGeneral,
		StackedSplineArea3DAppearance,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.StackedSplineArea3DMarkerLine field instead.")]
		StackedSplineArea3DMarker,
		StackedSplineArea3DMarkerLine,
		FullStackedSplineArea3DGeneral,
		FullStackedSplineArea3DAppearance,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.FullStackedSplineArea3DMarkerLine field instead.")]
		FullStackedSplineArea3DMarker,
		FullStackedSplineArea3DMarkerLine,
		RangeArea3DGeneral,
		RangeArea3DAppearance,
		RangeArea3DMarkerLine,
		BubbleGeneral,
		BubbleAppearance,
		BubbleShadow,
		BubbleMarker,
		BubbleIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BubbleIndicators field instead.")]
		BubbleTrendlines,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BubbleIndicators field instead.")]
		BubbleFibonacciIndicators,
		[Obsolete("This field is obsolete now. Use the SeriesViewPageTab.BubbleIndicators field instead.")]
		BubbleRegressionLines
	}
	public enum DiagramPageTab {
		XYDiagramGeneral,
		[Obsolete("This field is obsolete now. Use the PanePageTab.Appearance field instead.")]
		XYDiagramAppearance,
		[Obsolete("This field is obsolete now. Use the PanePageTab.Appearance field instead.")]
		XYDiagramBackground,
		[Obsolete("This field is obsolete now. Use the PanePageTab.Border field instead.")]
		XYDiagramBorder,
		[Obsolete("This field is obsolete now. Use the PanePageTab.Shadow field instead.")]
		XYDiagramShadow,
		XYDiagramElements,
		SimpleDiagramGeneral,
		XYDiagram3DGeneral,
		XYDiagram3DRotate,
		XYDiagram3DScrollingZooming,
		XYDiagramScrollingZooming,
		XYDiagram3DBackground,
		SimpleDiagram3DGeneral,
		SimpleDiagram3DRotate,
		SimpleDiagram3DScrollingZooming,		
		RadarDiagramGeneral,
		RadarDiagramAppearance,
		RadarDiagramBorder,
		RadarDiagramShadow
	}
	public enum PanePageTab {
		General,
		Appearance,
		Border,
		Shadow,
		ScrollingZooming,
		ScrollBarOptions
	}
	public enum AnnotationPageTab {
		General,
		AnchorPoint,
		ShapePosition,
		Content,
		Padding,
		Appearance,
		Border,
		Shadow
	}
	public enum AxisPageTab {
		General,
		ScaleOptions,
		Appearance,
		Elements,
		Labels,
		Strips,
		ConstantLines,
		ScaleBreaks
	}
	public enum DataPageTab {
		Points,
		SeriesBinding,
		AutoCreatedSeries,
		PivotGridOptions
	}
	public enum SeriesLabelsPageTab {
		BarGeneral,
		BarPointOptions,
		BarLine,
		BarAppearance,
		BarBorder,
		BarShadow,
		StackedBarGeneral,
		StackedBarPointOptions,
		StackedBarAppearance,
		StackedBarBorder,
		StackedBarShadow,
		FullStackedBarGeneral,
		FullStackedBarPointOptions,
		FullStackedBarAppearance,
		FullStackedBarBorder,
		FullStackedBarShadow,
		SideBySideStackedBarGeneral,
		SideBySideStackedBarPointOptions,
		SideBySideStackedBarAppearance,
		SideBySideStackedBarBorder,
		SideBySideStackedBarShadow,
		SideBySideFullStackedBarGeneral,
		SideBySideFullStackedBarPointOptions,
		SideBySideFullStackedBarAppearance,
		SideBySideFullStackedBarBorder,
		SideBySideFullStackedBarShadow,
		Bar3DGeneral,
		Bar3DPointOptions,
		Bar3DLine,
		Bar3DAppearance,
		Bar3DBorder,
		StackedBar3DGeneral,
		StackedBar3DPointOptions,
		StackedBar3DAppearance,
		StackedBar3DBorder,
		FullStackedBar3DGeneral,
		FullStackedBar3DPointOptions,
		FullStackedBar3DAppearance,
		FullStackedBar3DBorder,
		ManhattanBarGeneral,
		ManhattanBarPointOptions,
		ManhattanBarLine,
		ManhattanBarAppearance,
		ManhattanBarBorder,
		SideBySideStackedBar3DGeneral,
		SideBySideStackedBar3DPointOptions,
		SideBySideStackedBar3DAppearance,
		SideBySideStackedBar3DBorder,
		SideBySideFullStackedBar3DGeneral,
		SideBySideFullStackedBar3DPointOptions,
		SideBySideFullStackedBar3DAppearance,
		SideBySideFullStackedBar3DBorder,
		PointGeneral,
		PointPointOptions,
		PointLine,
		PointAppearance,
		PointBorder,
		PointShadow,
		RadarPointGeneral,
		RadarPointPointOptions,
		RadarPointLine,
		RadarPointAppearance,
		RadarPointBorder,
		RadarPointShadow,
		PolarPointGeneral,
		PolarPointPointOptions,
		PolarPointLine,
		PolarPointAppearance,
		PolarPointBorder,
		PolarPointShadow,
		LineGeneral,
		LinePointOptions,
		LineLine,
		LineAppearance,
		LineBorder,
		LineShadow,
		StackedLineGeneral,
		StackedLinePointOptions,
		StackedLineLine,
		StackedLineAppearance,
		StackedLineBorder,
		StackedLineShadow,
		FullStackedLineGeneral,
		FullStackedLinePointOptions,
		FullStackedLineLine,
		FullStackedLineAppearance,
		FullStackedLineBorder,
		FullStackedLineShadow,
		SplineGeneral,
		SplinePointOptions,
		SplineLine,
		SplineAppearance,
		SplineBorder,
		SplineShadow,
		StepLineGeneral,
		StepLinePointOptions,
		StepLineLine,
		StepLineAppearance,
		StepLineBorder,
		StepLineShadow,
		ScatterLineGeneral,
		ScatterLinePointOptions,
		ScatterLineLine,
		ScatterLineAppearance,
		ScatterLineBorder,
		ScatterLineShadow,
		RadarLineGeneral,
		RadarLinePointOptions,
		RadarLineLine,
		RadarLineAppearance,
		RadarLineBorder,
		RadarLineShadow,
		ScatterRadarLineGeneral,
		ScatterRadarLinePointOptions,
		ScatterRadarLineLine,
		ScatterRadarLineAppearance,
		ScatterRadarLineBorder,
		ScatterRadarLineShadow,
		PolarLineGeneral,
		PolarLinePointOptions,
		PolarLineLine,
		PolarLineAppearance,
		PolarLineBorder,
		PolarLineShadow,
		ScatterPolarLineGeneral,
		ScatterPolarLinePointOptions,
		ScatterPolarLineLine,
		ScatterPolarLineAppearance,
		ScatterPolarLineBorder,
		ScatterPolarLineShadow,
		Line3DGeneral,
		Line3DPointOptions,
		Line3DLine,
		Line3DAppearance,
		Line3DBorder,
		StackedLine3DGeneral,
		StackedLine3DPointOptions,
		StackedLine3DLine,
		StackedLine3DAppearance,
		StackedLine3DBorder,
		FullStackedLine3DGeneral,
		FullStackedLine3DPointOptions,
		FullStackedLine3DLine,
		FullStackedLine3DAppearance,
		FullStackedLine3DBorder,
		StepLine3DGeneral,
		StepLine3DPointOptions,
		StepLine3DLine,
		StepLine3DAppearance,
		StepLine3DBorder,
		Spline3DGeneral,
		Spline3DPointOptions,
		Spline3DLine,
		Spline3DAppearance,
		Spline3DBorder,
		[Obsolete("This field is obsolete now.")]
		Spline3DShadow,
		AreaGeneral,
		AreaPointOptions,
		AreaLine,
		AreaAppearance,
		AreaBorder,
		AreaShadow,
		StepAreaGeneral,
		StepAreaPointOptions,
		StepAreaLine,
		StepAreaAppearance,
		StepAreaBorder,
		StepAreaShadow,
		SplineAreaGeneral,
		SplineAreaPointOptions,
		SplineAreaLine,
		SplineAreaAppearance,
		SplineAreaBorder,
		SplineAreaShadow,
		StackedAreaGeneral,
		StackedAreaPointOptions,
		StackedAreaLine,
		StackedAreaAppearance,
		StackedAreaBorder,
		StackedAreaShadow,
		StackedSplineAreaGeneral,
		StackedSplineAreaPointOptions,
		StackedSplineAreaLine,
		StackedSplineAreaAppearance,
		StackedSplineAreaBorder,
		StackedSplineAreaShadow,
		FullStackedAreaGeneral,
		FullStackedAreaPointOptions,
		FullStackedAreaAppearance,
		FullStackedAreaBorder,
		FullStackedAreaShadow,
		FullStackedSplineAreaGeneral,
		FullStackedSplineAreaPointOptions,
		FullStackedSplineAreaAppearance,
		FullStackedSplineAreaBorder,
		FullStackedSplineAreaShadow,
		RangeAreaGeneral,
		RangeAreaPointOptions,
		RangeAreaLine,
		RangeAreaAppearance,
		RangeAreaBorder,
		RangeAreaShadow,
		RadarAreaGeneral,
		RadarAreaPointOptions,
		RadarAreaLine,
		RadarAreaAppearance,
		RadarAreaBorder,
		RadarAreaShadow,
		PolarAreaGeneral,
		PolarAreaPointOptions,
		PolarAreaLine,
		PolarAreaAppearance,
		PolarAreaBorder,
		PolarAreaShadow,
		Area3DGeneral,
		Area3DPointOptions,
		Area3DLine,
		Area3DAppearance,
		Area3DBorder,
		StackedArea3DGeneral,
		StackedArea3DPointOptions,
		StackedArea3DAppearance,
		StackedArea3DBorder,
		FullStackedArea3DGeneral,
		FullStackedArea3DPointOptions,
		FullStackedArea3DAppearance,
		FullStackedArea3DBorder,
		StepArea3DGeneral,
		StepArea3DPointOptions,
		StepArea3DLine,
		StepArea3DAppearance,
		StepArea3DBorder,
		SplineArea3DGeneral,
		SplineArea3DPointOptions,
		SplineArea3DLine,
		SplineArea3DAppearance,
		SplineArea3DBorder,
		[Obsolete("This field is obsolete now.")]
		SplineArea3DShadow,
		StackedSplineArea3DGeneral,
		StackedSplineArea3DPointOptions,
		[Obsolete("This field is obsolete now.")]
		StackedSplineArea3DLine,
		StackedSplineArea3DAppearance,
		StackedSplineArea3DBorder,
		[Obsolete("This field is obsolete now.")]
		StackedSplineArea3DShadow,
		FullStackedSplineArea3DGeneral,
		FullStackedSplineArea3DPointOptions,
		[Obsolete("This field is obsolete now.")]
		FullStackedSplineArea3DLine,
		FullStackedSplineArea3DAppearance,
		FullStackedSplineArea3DBorder,
		[Obsolete("This field is obsolete now.")]
		FullStackedSplineArea3DShadow,
		RangeArea3DGeneral,
		RangeArea3DPointOptions,
		RangeArea3DLine,
		RangeArea3DAppearance,
		RangeArea3DBorder,
		PieGeneral,
		PiePointOptions,
		PieLine,
		PieAppearance,
		PieBorder,
		PieShadow,
		DoughnutGeneral,
		DoughnutPointOptions,
		DoughnutLine,
		DoughnutAppearance,
		DoughnutBorder,
		DoughnutShadow,
		NestedDoughnutGeneral,
		NestedDoughnutPointOptions,
		NestedDoughnutLine,
		NestedDoughnutAppearance,
		NestedDoughnutBorder,
		NestedDoughnutShadow,
		Pie3DGeneral,
		Pie3DPointOptions,
		Pie3DLine,
		Pie3DAppearance,
		Pie3DBorder,
		Doughnut3DGeneral,
		Doughnut3DPointOptions,
		Doughnut3DLine,
		Doughnut3DAppearance,
		Doughnut3DBorder,
		FunnelGeneral,
		FunnelPointOptions,
		FunnelLine,
		FunnelAppearance,
		FunnelBorder,
		FunnelShadow,
		Funnel3DGeneral,
		Funnel3DPointOptions,
		Funnel3DLine,
		Funnel3DAppearance,
		Funnel3DBorder,
		StockGeneral,
		StockPointOptions,
		StockAppearance,
		StockBorder,
		StockShadow,
		CandleStickGeneral,
		CandleStickPointOptions,
		CandleStickAppearance,
		CandleStickBorder,
		CandleStickShadow,
		RangeBarGeneral,
		RangeBarPointOptions,
		RangeBarAppearance,
		RangeBarBorder,
		RangeBarShadow,
		SideBySideRangeBarGeneral,
		SideBySideRangeBarPointOptions,
		SideBySideRangeBarAppearance,
		SideBySideRangeBarBorder,
		SideBySideRangeBarShadow,
		GanttGeneral,
		GanttPointOptions,
		GanttAppearance,
		GanttBorder,
		GanttShadow,
		SideBySideGanttGeneral,
		SideBySideGanttPointOptions,
		SideBySideGanttAppearance,
		SideBySideGanttBorder,
		SideBySideGanttShadow,
		BubbleGeneral,
		BubblePointOptions,
		BubbleLine,
		BubbleAppearance,
		BubbleBorder,
		BubbleShadow
	}
	public enum LegendPageTab {
		General,
		Appearance,
		Interior,
		Marker,
		Text,		
		Background,
		Border,
		Shadow
	}
	public enum TitlePageTab {
		General,
		Text
	}
	public enum ChartPageTab {
		General,
		Border,
		Padding,
		EmptyChartText,
		SmallChartText
	}
	public enum SeriesPageTab {
		SeriesOptions,
		TopNOptions,
		[Obsolete("This field is obsolete now.")]
		PointOptions,
		[Obsolete("This field is obsolete now.")]
		LegendPointOptions,
		LegendTextPattern,
		General
	}
}
