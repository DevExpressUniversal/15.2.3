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
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartType
	public enum ChartType {
		ColumnClustered,
		ColumnStacked,
		ColumnFullStacked,
		Column3DClustered,
		Column3DStacked,
		Column3DFullStacked,
		Column3DStandard,
		Column3DClusteredCylinder,
		Column3DStackedCylinder,
		Column3DFullStackedCylinder,
		Column3DStandardCylinder,
		Column3DClusteredCone,
		Column3DStackedCone,
		Column3DFullStackedCone,
		Column3DStandardCone,
		Column3DClusteredPyramid,
		Column3DStackedPyramid,
		Column3DFullStackedPyramid,
		Column3DStandardPyramid,
		Line,
		LineStacked,
		LineFullStacked,
		LineMarker,
		LineStackedMarker,
		LineFullStackedMarker,
		Line3D,
		Pie,
		Pie3D,
		PieExploded,
		Pie3DExploded,
		PieOfPie,
		BarOfPie,
		BarClustered,
		BarStacked,
		BarFullStacked,
		Bar3DClustered,
		Bar3DStacked,
		Bar3DFullStacked,
		Bar3DClusteredCylinder,
		Bar3DStackedCylinder,
		Bar3DFullStackedCylinder,
		Bar3DClusteredCone,
		Bar3DStackedCone,
		Bar3DFullStackedCone,
		Bar3DClusteredPyramid,
		Bar3DStackedPyramid,
		Bar3DFullStackedPyramid,
		Area,
		AreaStacked,
		AreaFullStacked,
		Area3D,
		Area3DStacked,
		Area3DFullStacked,
		ScatterMarkers,
		ScatterSmoothMarkers,
		ScatterSmooth,
		ScatterLine,
		ScatterLineMarkers,
		StockHighLowClose,
		StockOpenHighLowClose,
		StockVolumeHighLowClose,
		StockVolumeOpenHighLowClose,
		Surface,
		SurfaceWireframe,
		Surface3D,
		Surface3DWireframe,
		Doughnut,
		DoughnutExploded,
		Bubble,
		Bubble3D,
		Radar,
		RadarMarkers,
		RadarFilled
	}
	#endregion
	#region ChartBuilderBase
	public abstract class ChartBuilderBase {
		#region Fields
		public const int PercentageFormatId = 9;
		ChartSeriesRangeModelBase seriesModel;
		#endregion
		#region Properties
		protected virtual DisplayBlanksAs DispBlanksAs { get { return DisplayBlanksAs.Gap; } }
		protected virtual AxisPosition ArgumentAxisPosition { get { return AxisPosition.Bottom; } }
		protected virtual AxisPosition ValueAxisPosition { get { return AxisPosition.Left; } }
		protected virtual AxisCrossBetween ValueAxisCrossBetween { get { return AxisCrossBetween.Between; } }
		protected virtual TickMark ValueAxisMajorTickMark { get { return TickMark.Outside; } }
		protected virtual bool ArgumentAxisShowMajorGridlines { get { return false; } }
		protected virtual bool ValueAxisShowMajorGridlines { get { return true; } }
		protected internal abstract ChartViewType ViewType { get; }
		protected virtual bool PercentStacked { get { return false; } }
		#endregion
		public void Execute(Chart chart) {
			Execute(chart, null);
		}
		public void Execute(Chart chart, CellRange range) {
			CreateSeriesModel(range);
			chart.BeginUpdate();
			try {
				SetupChart(chart);
				CreateAxes(chart);
				CreateViews(chart);
				CreateSeries(chart);
			}
			finally {
				chart.EndUpdate();
				this.seriesModel = null;
			}
		}
		protected void SetupChart(Chart chart) {
			chart.Culture = CultureInfo.CurrentCulture;
			chart.DispBlanksAs = DispBlanksAs;
			SetupView3D(chart.View3D);
			SetupLegend(chart.Legend);
			if (seriesModel != null)
				chart.SeriesDirection = seriesModel.Direction;
		}
		void CreateSeriesModel(CellRange range) {
			if (range != null)
				this.seriesModel = ChartRangesCalculator.CreateModel(range, ViewType);
			else
				this.seriesModel = null;
		}
		protected virtual void CreateAxes(Chart chart) {
		}
		public void SetupAxes(Chart chart) {
			AxisBase savedArgAxis = chart.PrimaryAxes.Count > 0 ? chart.PrimaryAxes[0] : null;
			AxisBase savedValAxis = chart.PrimaryAxes.Count > 1 ? chart.PrimaryAxes[1] : null;
			AxisBase savedSerAxis = chart.PrimaryAxes.Count > 2 ? chart.PrimaryAxes[2] : null;
			chart.PrimaryAxes.Clear();
			chart.SecondaryAxes.Clear();
			CreateAxes(chart);
			if (chart.PrimaryAxes.Count > 0)
				CopyLayout(chart.PrimaryAxes[0], savedArgAxis, false);
			if (chart.PrimaryAxes.Count > 1)
				CopyLayout(chart.PrimaryAxes[1], savedValAxis, PercentStacked);
			if (chart.PrimaryAxes.Count > 2)
				CopyLayout(chart.PrimaryAxes[2], savedSerAxis, false);
		}
		void CopyLayout(AxisBase axis, AxisBase savedAxis, bool percentStacked) {
			axis.BeginUpdate();
			try {
				axis.CopyLayout(savedAxis, percentStacked);
			}
			finally {
				axis.EndUpdate();
			}
		}
		protected virtual void CreateViews(Chart chart) {
			IChartView view = CreateViewInstance(chart);
			SetupViewWithoutAxes(view);
			view.Axes = chart.PrimaryAxes;
			chart.Views.Add(view);
		}
		protected internal abstract IChartView CreateViewInstance(IChart chart);
		protected internal abstract void SetupViewWithoutAxes(IChartView view);
		void CreateSeries(Chart chart) {
			if (seriesModel == null)
				return;
			int count = seriesModel.SeriesCount;
			for (int i = 0; i < count; i++) {
				ISeries series = CreateSeries(chart, i);
				seriesModel.ApplyData(series, i);
			}
		}
		protected virtual ISeries CreateSeries(Chart chart, int seriesIndex) {
			ISeries result = chart.Views[0].CreateSeriesInstance();
			result.Index = seriesIndex;
			result.Order = seriesIndex;
			chart.Views[0].Series.Add(result);
			return result;
		}
		protected internal virtual void SetupSeries(ISeries series) {
		}
		protected internal virtual void SetupView3D(View3DOptions view3D) {
		}
		protected virtual void SetupLegend(Legend legend) {
			legend.Visible = true;
			legend.Overlay = false;
			legend.Position = LegendPosition.Right;
		}
		bool IsDateArguments() {
			if (seriesModel == null)
				return false;
			ChartDataReference dataRef = seriesModel.Data.SeriesArguments as ChartDataReference;
			if (dataRef == null)
				return false;
			dataRef.DetectValueType();
			return dataRef.ValueType == DataReferenceValueType.DateTime;
		}
		string GetArgumentsNumberFormatString() {
			if (seriesModel == null)
				return string.Empty;
			ChartDataReference dataRef = seriesModel.Data.SeriesArguments as ChartDataReference;
			if (dataRef == null)
				return string.Empty;
			return dataRef.GetNumberFormatString();
		}
		protected AxisBase CreateArgumentAxis(Chart chart, AxisPosition position, bool showMajorGridlines, bool delete) {
			if (IsDateArguments())
				return CreateDateAxis(chart, position, showMajorGridlines, delete);
			return CreateCategoryAxis(chart, position, showMajorGridlines, delete);
		}
		CategoryAxis CreateCategoryAxis(Chart chart, AxisPosition position, bool showMajorGridlines, bool delete) {
			CategoryAxis axis = new CategoryAxis(chart);
			axis.BeginUpdate();
			try {
				axis.Auto = true;
				axis.Delete = delete;
				axis.Position = position;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.ShowMajorGridlines = showMajorGridlines;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		DateAxis CreateDateAxis(Chart chart, AxisPosition position, bool showMajorGridlines, bool delete) {
			DateAxis axis = new DateAxis(chart);
			axis.BeginUpdate();
			try {
				axis.Auto = true;
				axis.Delete = delete;
				axis.Position = position;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.ShowMajorGridlines = showMajorGridlines;
				axis.NumberFormat.NumberFormatCode = GetArgumentsNumberFormatString();
				axis.NumberFormat.SourceLinked = true;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		protected ValueAxis CreateValueAxis(Chart chart, AxisPosition position, bool showMajorGridlines, AxisCrossBetween crossBetween) {
			ValueAxis axis = new ValueAxis(chart);
			axis.BeginUpdate();
			try {
				axis.CrossBetween = crossBetween;
				axis.Position = position;
				axis.MajorTickMark = ValueAxisMajorTickMark;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.ShowMajorGridlines = showMajorGridlines;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		protected void CreateSeriesAxis(Chart chart) {
			SeriesAxis axis = new SeriesAxis(chart);
			axis.BeginUpdate();
			try {
				axis.Position = AxisPosition.Bottom;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.CrossesAxis = chart.PrimaryAxes[1];
			}
			finally {
				axis.EndUpdate();
			}
			chart.PrimaryAxes.Add(axis);
		}
		protected void SetupAxisNumberFormat(AxisBase axis, int formatId) {
			axis.NumberFormat.NumberFormatCode = axis.DocumentModel.Cache.NumberFormatCache[formatId].FormatCode;
			axis.NumberFormat.SourceLinked = true;
		}
	}
	public abstract class CatValChartBuilder : ChartBuilderBase {
		protected override void CreateAxes(Chart chart) {
			AxisBase argAxis = CreateArgumentAxis(chart, ArgumentAxisPosition, ArgumentAxisShowMajorGridlines, false);
			AxisBase valAxis = CreateValueAxis(chart, ValueAxisPosition, ValueAxisShowMajorGridlines, ValueAxisCrossBetween);
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.PrimaryAxes.Add(argAxis);
			chart.PrimaryAxes.Add(valAxis);
		}
	}
	public abstract class XYChartBuilder : ChartBuilderBase {
		protected override void CreateAxes(Chart chart) {
			ValueAxis argAxis = CreateValueAxis(chart, ArgumentAxisPosition, ArgumentAxisShowMajorGridlines, AxisCrossBetween.Midpoint);
			ValueAxis valAxis = CreateValueAxis(chart, ValueAxisPosition, ValueAxisShowMajorGridlines, AxisCrossBetween.Midpoint);
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.PrimaryAxes.Add(argAxis);
			chart.PrimaryAxes.Add(valAxis);
		}
	}
	#endregion
	#region Bar2DChartBuilder
	public class Bar2DChartBuilder : CatValChartBuilder {
		#region Static Members
		internal static void SetupViewWithoutAxes(BarChartView view, BarChartDirection barDirection, BarChartGrouping grouping) {
			view.BeginUpdate();
			try {
				view.BarDirection = barDirection;
				view.Grouping = grouping;
				if (grouping == BarChartGrouping.Stacked || grouping == BarChartGrouping.PercentStacked)
					view.Overlap = 100;
			} finally {
				view.EndUpdate();
			}
		}
		#endregion
		public Bar2DChartBuilder(BarChartDirection barDirection, BarChartGrouping grouping) {
			BarDirection = barDirection;
			Grouping = grouping;
		}
		#region Properties
		public BarChartDirection BarDirection { get; private set; }
		public BarChartGrouping Grouping { get; private set; }
		protected override AxisPosition ArgumentAxisPosition {
			get {
				return BarDirection == BarChartDirection.Column ? AxisPosition.Bottom : AxisPosition.Left;
			}
		}
		protected override AxisPosition ValueAxisPosition {
			get {
				return BarDirection == BarChartDirection.Column ? AxisPosition.Left : AxisPosition.Bottom;
			}
		}
		protected internal override ChartViewType ViewType { get { return ChartViewType.Bar; } }
		protected override bool PercentStacked { get { return Grouping == BarChartGrouping.PercentStacked; } }
		#endregion
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			if (Grouping == BarChartGrouping.PercentStacked)
				SetupAxisNumberFormat(chart.PrimaryAxes[1], PercentageFormatId);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new BarChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			BarChartView barView = view as BarChartView;
			if (barView != null)
				SetupViewWithoutAxes(barView, BarDirection, Grouping);
		}
	}
	#endregion
	#region Bar3DChartBuilder
	public class Bar3DChartBuilder : Bar2DChartBuilder {
		public Bar3DChartBuilder(BarChartDirection barDirection, BarChartGrouping grouping, BarShape shape) 
			: base(barDirection, grouping) {
			Shape = shape;
		}
		#region Properties
		public BarShape Shape { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Bar3D; } }
		#endregion
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			if (Grouping == BarChartGrouping.Standard)
				CreateSeriesAxis(chart);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new Bar3DChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			Bar3DChartView bar3DView = view as Bar3DChartView;
			if (bar3DView == null)
				return;
			bar3DView.BeginUpdate();
			try {
				bar3DView.BarDirection = BarDirection;
				bar3DView.Grouping = Grouping;
				bar3DView.Shape = Shape;
			} finally {
				bar3DView.EndUpdate();
			}
		}
		protected internal override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				if (Grouping == BarChartGrouping.Standard) {
					view3D.RightAngleAxes = false;
					view3D.Perspective = 30;
				}
				else {
					view3D.RightAngleAxes = true;
					view3D.Perspective = 0;
				}
				view3D.XRotation = 15;
				view3D.YRotation = 20;
			}
			finally {
				view3D.EndUpdate();
			}
		}
	}
	#endregion
	#region Line2DChartBuilder
	public class Line2DChartBuilder : CatValChartBuilder {
		public Line2DChartBuilder(ChartGrouping grouping, bool showMarker) {
			Grouping = grouping;
			ShowMarker = showMarker;
		}
		#region Properties
		public ChartGrouping Grouping { get; private set; }
		public bool ShowMarker { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Line; } }
		protected override bool PercentStacked { get { return Grouping == ChartGrouping.PercentStacked; } }
		#endregion
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			if (Grouping == ChartGrouping.PercentStacked)
				SetupAxisNumberFormat(chart.PrimaryAxes[1], PercentageFormatId);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new LineChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			LineChartView lineView = view as LineChartView;
			if (lineView == null)
				return;
			lineView.BeginUpdate();
			try {
				lineView.Grouping = Grouping;
				lineView.ShowMarker = ShowMarker;
			} finally {
				lineView.EndUpdate();
			}
		}
		protected internal override void SetupSeries(ISeries series) {
			LineSeries lineSeries = series as LineSeries;
			if (lineSeries != null)
				lineSeries.Marker.Symbol = ShowMarker ? MarkerStyle.Auto : MarkerStyle.None;
		}
	}
	#endregion
	#region Line3DChartBuilder
	public class Line3DChartBuilder : CatValChartBuilder {
		protected internal override ChartViewType ViewType { get { return ChartViewType.Line3D; } }
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			CreateSeriesAxis(chart);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new Line3DChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			Line3DChartView line3DView = view as Line3DChartView;
			if (line3DView != null)
				line3DView.Grouping = ChartGrouping.Standard;
		}
		protected internal override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = false;
				view3D.XRotation = 15;
				view3D.YRotation = 20;
				view3D.Perspective = 30;
			}
			finally {
				view3D.EndUpdate();
			}
		}
	}
	#endregion
	#region Pie2DChartBuilder
	public class Pie2DChartBuilder : ChartBuilderBase {
		public Pie2DChartBuilder(bool exploded) {
			Exploded = exploded;
		}
		#region Properties
		public bool Exploded { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Pie; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new PieChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			PieChartView pieView = view as PieChartView;
			if (pieView == null)
				return;
			pieView.BeginUpdate();
			try {
				pieView.Exploded = Exploded;
				pieView.VaryColors = true;
			} finally {
				pieView.EndUpdate();
			}
		}
		protected internal override void SetupSeries(ISeries series) {
			PieSeries pieSeries = series as PieSeries;
			if (pieSeries != null && Exploded)
				pieSeries.SetDefaultExplosion();
		}
	}
	#endregion
	#region Pie3DChartBuilder
	public class Pie3DChartBuilder : Pie2DChartBuilder {
		public Pie3DChartBuilder(bool exploded) 
			: base(exploded) {
		}
		protected internal override ChartViewType ViewType { get { return ChartViewType.Pie3D; } }
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new Pie3DChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			Pie3DChartView pie3DView = view as Pie3DChartView;
			if (pie3DView == null)
				return;
			pie3DView.BeginUpdate();
			try {
				pie3DView.Exploded = Exploded;
				pie3DView.VaryColors = true;
			} finally {
				pie3DView.EndUpdate();
			}
		}
		protected internal override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = false;
				view3D.XRotation = 30;
				view3D.YRotation = 0;
				view3D.Perspective = 30;
			}
			finally {
				view3D.EndUpdate();
			}
		}
	}
	#endregion
	#region OfPieChartBuilder
	public class OfPieChartBuilder : ChartBuilderBase {
		public OfPieChartBuilder(ChartOfPieType ofPieType) {
			OfPieType = ofPieType;
		}
		#region Properties
		public ChartOfPieType OfPieType { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.OfPie; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new OfPieChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			OfPieChartView ofPieView = view as OfPieChartView;
			if (ofPieView == null)
				return;
			ofPieView.BeginUpdate();
			try {
				ofPieView.OfPieType = OfPieType;
				ofPieView.VaryColors = true;
				ofPieView.GapWidth = 100;
				ofPieView.SeriesLines.Add(new ShapeProperties(view.Parent.DocumentModel));
			} finally {
				ofPieView.EndUpdate();
			}
		}
	}
	#endregion
	#region Area2DChartBuilder
	public class Area2DChartBuilder : CatValChartBuilder {
		public Area2DChartBuilder(ChartGrouping grouping) {
			Grouping = grouping;
		}
		#region Properties
		public ChartGrouping Grouping { get; private set; }
		protected override DisplayBlanksAs DispBlanksAs { get { return DisplayBlanksAs.Zero; } }
		protected override AxisCrossBetween ValueAxisCrossBetween { get { return AxisCrossBetween.Midpoint; } }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Area; } }
		protected override bool PercentStacked { get { return Grouping == ChartGrouping.PercentStacked; } }
		#endregion
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			if (Grouping == ChartGrouping.PercentStacked)
				SetupAxisNumberFormat(chart.PrimaryAxes[1], PercentageFormatId);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new AreaChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			AreaChartView areaView = view as AreaChartView;
			if (areaView != null)
				areaView.Grouping = Grouping;
		}
	}
	#endregion
	#region Area3DChartBuilder
	public class Area3DChartBuilder : Area2DChartBuilder {
		public Area3DChartBuilder(ChartGrouping grouping)
			: base(grouping) {
		}
		protected internal override ChartViewType ViewType { get { return ChartViewType.Area3D; } }
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			if (Grouping == ChartGrouping.Standard)
				CreateSeriesAxis(chart);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new Area3DChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			Area3DChartView area3DView = view as Area3DChartView;
			if (area3DView != null)
				area3DView.Grouping = Grouping;
		}
		protected internal override void SetupView3D(View3DOptions view3D)
		{
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = false;
				view3D.XRotation = 15;
				view3D.YRotation = 20;
				view3D.Perspective = 30;
			}
			finally {
				view3D.EndUpdate();
			}
		}
	}
	#endregion
	#region ScatterChartBuilder
	public class ScatterChartBuilder : XYChartBuilder {
		public ScatterChartBuilder(ScatterChartStyle scatterStyle) {
			ScatterStyle = scatterStyle;
		}
		#region Properties
		public ScatterChartStyle ScatterStyle { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Scatter; } }
		bool HasMarker { get { return ScatterStyle == ScatterChartStyle.Marker || ScatterStyle == ScatterChartStyle.LineMarker || ScatterStyle == ScatterChartStyle.SmoothMarker; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new ScatterChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			ScatterChartView scatterView = view as ScatterChartView;
			if (scatterView != null)
				scatterView.ScatterStyle = ScatterStyle;
		}
		protected internal override void SetupSeries(ISeries series) {
			ScatterSeries scatterSeries = series as ScatterSeries;
			if (scatterSeries == null)
				return;
			scatterSeries.Marker.Symbol = HasMarker ? MarkerStyle.Auto : MarkerStyle.None;
			scatterSeries.Smooth = ScatterStyle == ScatterChartStyle.Smooth || ScatterStyle == ScatterChartStyle.SmoothMarker;
			if (ScatterStyle == ScatterChartStyle.Marker)
				scatterSeries.ShapeProperties.Outline.Fill = DrawingFill.None;  
		}
	}
	#endregion
	#region StockChartBuilder
	public class StockChartBuilder : CatValChartBuilder {
		public StockChartBuilder(bool showUpDownBars) {
			ShowUpDownBars = showUpDownBars;
		}
		#region Properties
		public bool ShowUpDownBars { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Stock; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new StockChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			StockChartView stockView = view as StockChartView;
			if (stockView == null)
				return;
			stockView.BeginUpdate();
			try {
				stockView.ShowHiLowLines = true;
				stockView.ShowUpDownBars = ShowUpDownBars;
			} finally {
				stockView.EndUpdate();
			}
		}
		protected internal override void SetupSeries(ISeries series) {
			LineSeries lineSeries = series as LineSeries;
			if (lineSeries == null)
				return;
			if (ShowUpDownBars) {
				lineSeries.ShapeProperties.Outline.Fill = DrawingFill.None;
				lineSeries.Marker.Symbol = MarkerStyle.None;
			} else {
				lineSeries.ShapeProperties.Outline.Fill = DrawingFill.None;
				if (lineSeries.Index == 2) {
					lineSeries.Marker.Symbol = MarkerStyle.Dot;
					lineSeries.Marker.Size = 3;
				} else
					lineSeries.Marker.Symbol = MarkerStyle.None;
			}
		}
		protected override ISeries CreateSeries(Chart chart, int seriesIndex) {
			LineSeries result = new LineSeries(chart.Views[0]);
			result.Index = seriesIndex;
			result.Order = seriesIndex;
			SetupSeries(result);
			chart.Views[0].Series.Add(result);
			return result;
		}
	}
	#endregion
	#region VolumeStockChartBuilder
	public class VolumeStockChartBuilder : StockChartBuilder {
		public VolumeStockChartBuilder(bool showUpDownBars)
			: base(showUpDownBars) {
		}
		protected override void CreateAxes(Chart chart) {
			AxisBase argAxis = CreateArgumentAxis(chart, AxisPosition.Bottom, false, false);
			AxisBase valAxis = CreateValueAxis(chart, AxisPosition.Left, true, ValueAxisCrossBetween);
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.PrimaryAxes.Add(argAxis);
			chart.PrimaryAxes.Add(valAxis);
			argAxis = CreateArgumentAxis(chart, AxisPosition.Bottom, false, true);
			valAxis = CreateValueAxis(chart, AxisPosition.Right, false, ValueAxisCrossBetween);
			valAxis.Crosses = AxisCrosses.Max;
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.SecondaryAxes.Add(argAxis);
			chart.SecondaryAxes.Add(valAxis);
		}
		protected override void CreateViews(Chart chart) {
			BarChartView view = CreateBarViewWithoutAxes(chart);
			view.Axes = chart.PrimaryAxes;
			chart.Views.Add(view);
			base.CreateViews(chart);
			chart.Views[1].Axes = chart.SecondaryAxes;
		}
		protected internal BarChartView CreateBarViewWithoutAxes(Chart chart) {
			BarChartView result = new BarChartView(chart);
			Bar2DChartBuilder.SetupViewWithoutAxes(result, BarChartDirection.Column, BarChartGrouping.Clustered);
			return result;
		}
		protected internal override void SetupSeries(ISeries series) {
			BarSeries barSeries = series as BarSeries;
			if (barSeries != null)
				SetupBarSeries(barSeries);
			else {
				LineSeries lineSeries = series as LineSeries;
				if (lineSeries != null)
					SetupLineSeries(lineSeries);
			}
		}
		void SetupBarSeries(BarSeries series) {
			series.ShapeProperties.Outline.Fill = DrawingFill.None;
		}
		void SetupLineSeries(LineSeries series) {
			if (ShowUpDownBars) {
				series.ShapeProperties.Outline.Fill = DrawingFill.None;
				series.Marker.Symbol = MarkerStyle.None;
			} else {
				series.ShapeProperties.Outline.Fill = DrawingFill.None;
				if (series.Index == 3) {
					series.Marker.Symbol = MarkerStyle.Dot;
					series.Marker.Size = 5;
				} else
					series.Marker.Symbol = MarkerStyle.None;
			}
		}
		protected override ISeries CreateSeries(Chart chart, int seriesIndex) {
			if (seriesIndex == 0) {
				BarSeries volumeSeries = new BarSeries(chart.Views[0]);
				volumeSeries.Index = seriesIndex;
				volumeSeries.Order = seriesIndex;
				SetupSeries(volumeSeries);
				chart.Views[0].Series.Add(volumeSeries);
				return volumeSeries;
			}
			LineSeries result = new LineSeries(chart.Views[1]);
			result.Index = seriesIndex;
			result.Order = seriesIndex;
			SetupSeries(result);
			chart.Views[1].Series.Add(result);
			return result;
		}
	}
	#endregion
	#region SurfaceChartBuilder
	public class SurfaceChartBuilder : CatValChartBuilder {
		public SurfaceChartBuilder(bool wireframe, bool is3D) {
			Wireframe = wireframe;
			Is3D = is3D;
		}
		#region Properties
		public bool Wireframe { get; private set; }
		public bool Is3D { get; private set; }
		protected override DisplayBlanksAs DispBlanksAs { get { return DisplayBlanksAs.Zero; } }
		protected override AxisCrossBetween ValueAxisCrossBetween { get { return AxisCrossBetween.Midpoint; } }
		protected internal override ChartViewType ViewType { get { return Is3D ? ChartViewType.Surface3D : ChartViewType.Surface; } }
		#endregion
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			CreateSeriesAxis(chart);
		}
		protected internal override IChartView CreateViewInstance(IChart chart) {
			if (Is3D)
				return new Surface3DChartView(chart);
			return new SurfaceChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			SurfaceChartViewBase surfaceView = view as SurfaceChartViewBase;
			if (surfaceView != null)
				surfaceView.Wireframe = Wireframe;
		}
		protected internal override void SetupView3D(View3DOptions view3D) {
			view3D.BeginUpdate();
			try {
				view3D.RightAngleAxes = false;
				if (Is3D) {
					view3D.XRotation = 15;
					view3D.YRotation = 20;
					view3D.Perspective = 30;
				}
				else {
					view3D.XRotation = 90;
					view3D.YRotation = 0;
					view3D.Perspective = 0;
				}
			}
			finally {
				view3D.EndUpdate();
			}
		}
	}
	#endregion
	#region DoughnutChartBuilder
	public class DoughnutChartBuilder : Pie2DChartBuilder {
		public DoughnutChartBuilder(bool exploded)
			: base(exploded) {
		}
		protected internal override ChartViewType ViewType { get { return ChartViewType.Doughnut; } }
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new DoughnutChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			DoughnutChartView doughnutView = view as DoughnutChartView;
			if (doughnutView == null)
				return;
			doughnutView.BeginUpdate();
			try {
				doughnutView.Exploded = Exploded;
				doughnutView.VaryColors = true;
				doughnutView.HoleSize = 50;
				doughnutView.DataLabels.Apply = true;
			} finally {
				doughnutView.EndUpdate();
			}
		}
	}
	#endregion
	#region BubbleChartBuilder
	public class BubbleChartBuilder : XYChartBuilder {
		public BubbleChartBuilder(bool bubble3D) {
			Bubble3D = bubble3D;
		}
		#region Properties
		public bool Bubble3D { get; private set; }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Bubble; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new BubbleChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			BubbleChartView bubbleView = view as BubbleChartView;
			if (bubbleView == null)
				return;
			bubbleView.BeginUpdate();
			try {
				bubbleView.Bubble3D = Bubble3D;
				bubbleView.ShowNegBubbles = false;
				bubbleView.DataLabels.Apply = true;
			} finally {
				bubbleView.EndUpdate();
			}
		}
		protected internal override void SetupSeries(ISeries series) {
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries != null)
				bubbleSeries.Bubble3D = Bubble3D;
		}
	}
	#endregion
	#region RadarChartBuilder
	public class RadarChartBuilder : CatValChartBuilder {
		public RadarChartBuilder(RadarChartStyle radarStyle) {
			RadarStyle = radarStyle;
		}
		#region Properties
		public RadarChartStyle RadarStyle { get; private set; }
		protected override TickMark ValueAxisMajorTickMark { get { return TickMark.Cross; } }
		protected internal override ChartViewType ViewType { get { return ChartViewType.Radar; } }
		#endregion
		protected internal override IChartView CreateViewInstance(IChart chart) {
			return new RadarChartView(chart);
		}
		protected internal override void SetupViewWithoutAxes(IChartView view) {
			RadarChartView radarView = view as RadarChartView;
			if (radarView != null) {
				radarView.RadarStyle = RadarStyle;
				radarView.DataLabels.Apply = true;
			}
		}
		protected internal override void SetupSeries(ISeries series) {
			RadarSeries radarSeries = series as RadarSeries;
			if (radarSeries != null && RadarStyle == RadarChartStyle.Standard)
				radarSeries.Marker.Symbol = MarkerStyle.None;
		}
	}
	#endregion
	#region ChartBuilderFactory
	public static class ChartBuilderFactory {
		static Dictionary<ChartType, ChartBuilderBase> builders = CreateBuilders();
		static Dictionary<ChartType, ChartBuilderBase> CreateBuilders() {
			Dictionary<ChartType, ChartBuilderBase> result = new Dictionary<ChartType, ChartBuilderBase>();
			result.Add(ChartType.ColumnClustered, new Bar2DChartBuilder(BarChartDirection.Column, BarChartGrouping.Clustered));
			result.Add(ChartType.ColumnStacked, new Bar2DChartBuilder(BarChartDirection.Column, BarChartGrouping.Stacked));
			result.Add(ChartType.ColumnFullStacked, new Bar2DChartBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked));
			result.Add(ChartType.Column3DClustered, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Box));
			result.Add(ChartType.Column3DStacked, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Box));
			result.Add(ChartType.Column3DFullStacked, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Box));
			result.Add(ChartType.Column3DStandard, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Box));
			result.Add(ChartType.Column3DClusteredCylinder, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Cylinder));
			result.Add(ChartType.Column3DStackedCylinder, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Cylinder));
			result.Add(ChartType.Column3DFullStackedCylinder, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Cylinder));
			result.Add(ChartType.Column3DStandardCylinder, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Cylinder));
			result.Add(ChartType.Column3DClusteredCone, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Cone));
			result.Add(ChartType.Column3DStackedCone, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Cone));
			result.Add(ChartType.Column3DFullStackedCone, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Cone));
			result.Add(ChartType.Column3DStandardCone, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Cone));
			result.Add(ChartType.Column3DClusteredPyramid, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Pyramid));
			result.Add(ChartType.Column3DStackedPyramid, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Pyramid));
			result.Add(ChartType.Column3DFullStackedPyramid, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Pyramid));
			result.Add(ChartType.Column3DStandardPyramid, new Bar3DChartBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Pyramid));
			result.Add(ChartType.BarClustered, new Bar2DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered));
			result.Add(ChartType.BarStacked, new Bar2DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked));
			result.Add(ChartType.BarFullStacked, new Bar2DChartBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked));
			result.Add(ChartType.Bar3DClustered, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Box));
			result.Add(ChartType.Bar3DStacked, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Box));
			result.Add(ChartType.Bar3DFullStacked, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Box));
			result.Add(ChartType.Bar3DClusteredCylinder, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Cylinder));
			result.Add(ChartType.Bar3DStackedCylinder, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Cylinder));
			result.Add(ChartType.Bar3DFullStackedCylinder, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Cylinder));
			result.Add(ChartType.Bar3DClusteredCone, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Cone));
			result.Add(ChartType.Bar3DStackedCone, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Cone));
			result.Add(ChartType.Bar3DFullStackedCone, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Cone));
			result.Add(ChartType.Bar3DClusteredPyramid, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Pyramid));
			result.Add(ChartType.Bar3DStackedPyramid, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Pyramid));
			result.Add(ChartType.Bar3DFullStackedPyramid, new Bar3DChartBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Pyramid));
			result.Add(ChartType.Line, new Line2DChartBuilder(ChartGrouping.Standard, false));
			result.Add(ChartType.LineMarker, new Line2DChartBuilder(ChartGrouping.Standard, true));
			result.Add(ChartType.LineStacked, new Line2DChartBuilder(ChartGrouping.Stacked, false));
			result.Add(ChartType.LineStackedMarker, new Line2DChartBuilder(ChartGrouping.Stacked, true));
			result.Add(ChartType.LineFullStacked, new Line2DChartBuilder(ChartGrouping.PercentStacked, false));
			result.Add(ChartType.LineFullStackedMarker, new Line2DChartBuilder(ChartGrouping.PercentStacked, true));
			result.Add(ChartType.Line3D, new Line3DChartBuilder());
			result.Add(ChartType.Pie, new Pie2DChartBuilder(false));
			result.Add(ChartType.PieExploded, new Pie2DChartBuilder(true));
			result.Add(ChartType.Pie3D, new Pie3DChartBuilder(false));
			result.Add(ChartType.Pie3DExploded, new Pie3DChartBuilder(true));
			result.Add(ChartType.BarOfPie, new OfPieChartBuilder(ChartOfPieType.Bar));
			result.Add(ChartType.PieOfPie, new OfPieChartBuilder(ChartOfPieType.Pie));
			result.Add(ChartType.Area, new Area2DChartBuilder(ChartGrouping.Standard));
			result.Add(ChartType.AreaStacked, new Area2DChartBuilder(ChartGrouping.Stacked));
			result.Add(ChartType.AreaFullStacked, new Area2DChartBuilder(ChartGrouping.PercentStacked));
			result.Add(ChartType.Area3D, new Area3DChartBuilder(ChartGrouping.Standard));
			result.Add(ChartType.Area3DStacked, new Area3DChartBuilder(ChartGrouping.Stacked));
			result.Add(ChartType.Area3DFullStacked, new Area3DChartBuilder(ChartGrouping.PercentStacked));
			result.Add(ChartType.ScatterMarkers, new ScatterChartBuilder(ScatterChartStyle.Marker));
			result.Add(ChartType.ScatterLine, new ScatterChartBuilder(ScatterChartStyle.Line));
			result.Add(ChartType.ScatterLineMarkers, new ScatterChartBuilder(ScatterChartStyle.LineMarker));
			result.Add(ChartType.ScatterSmooth, new ScatterChartBuilder(ScatterChartStyle.Smooth));
			result.Add(ChartType.ScatterSmoothMarkers, new ScatterChartBuilder(ScatterChartStyle.SmoothMarker));
			result.Add(ChartType.StockHighLowClose, new StockChartBuilder(false));
			result.Add(ChartType.StockOpenHighLowClose, new StockChartBuilder(true));
			result.Add(ChartType.StockVolumeHighLowClose, new VolumeStockChartBuilder(false));
			result.Add(ChartType.StockVolumeOpenHighLowClose, new VolumeStockChartBuilder(true));
			result.Add(ChartType.Surface, new SurfaceChartBuilder(false, false));
			result.Add(ChartType.Surface3D, new SurfaceChartBuilder(false, true));
			result.Add(ChartType.SurfaceWireframe, new SurfaceChartBuilder(true, false));
			result.Add(ChartType.Surface3DWireframe, new SurfaceChartBuilder(true, true));
			result.Add(ChartType.Doughnut, new DoughnutChartBuilder(false));
			result.Add(ChartType.DoughnutExploded, new DoughnutChartBuilder(true));
			result.Add(ChartType.Bubble, new BubbleChartBuilder(false));
			result.Add(ChartType.Bubble3D, new BubbleChartBuilder(true));
			result.Add(ChartType.Radar, new RadarChartBuilder(RadarChartStyle.Standard));
			result.Add(ChartType.RadarMarkers, new RadarChartBuilder(RadarChartStyle.Marker));
			result.Add(ChartType.RadarFilled, new RadarChartBuilder(RadarChartStyle.Filled));
			return result;
		}
		internal static ChartBuilderBase GetBuilder(ChartType chartType) {
			if (HasBuilder(chartType))
				return builders[chartType];
			return null;
		}
		internal static bool HasBuilder(ChartType chartType) {
			return builders.ContainsKey(chartType);
		}
	}
	#endregion
	#region CreateEmptyChartCommand
	public class CreateEmptyChartCommand : ErrorHandledWorksheetCommand {
		#region Fields
		ChartType chartType;
		#endregion
		public CreateEmptyChartCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, ChartType chartType)
			: base(documentModelPart, errorHandler) {
			this.chartType = chartType;
		}
		public Chart Chart { get { return Result as Chart; } }
		protected internal override void ExecuteCore() {
			Chart chart = new Chart(Worksheet);
			chart.AutoTitleDeleted = true;
			int id = Worksheet.DrawingObjects.GetMaxDrawingId();
			chart.DrawingObject.Properties.Id = id + 1;
			chart.DrawingObject.Properties.Name = string.Format("Chart {0}", id);
			chart.DrawingObject.AnchorType = AnchorType.TwoCell;
			chart.DrawingObject.Width = DocumentModel.UnitConverter.TwipsToModelUnits(5 * 1440);
			chart.DrawingObject.Height = DocumentModel.UnitConverter.TwipsToModelUnits(3 * 1440);
			BuildChart(chart, ChartBuilderFactory.GetBuilder(chartType));
			Result = chart;
		}
		protected virtual void BuildChart(Chart chart, ChartBuilderBase builder) {
			if (builder != null)
				builder.Execute(chart);
		}
	}
	#endregion
	#region CreateChartCommand
	public class CreateChartCommand : CreateEmptyChartCommand {
		#region Fields
		CellRange range;
		#endregion
		public CreateChartCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, ChartType chartType, CellRange range)
			: base(documentModelPart, errorHandler, chartType) {
			this.range = range;
		}
		protected override void BuildChart(Chart chart, ChartBuilderBase builder) {
			if (builder != null)
				builder.Execute(chart, range);
		}
		protected internal override bool Validate() {
			IModelErrorInfo error = ValidateRange();
			return HandleError(error);
		}
		IModelErrorInfo ValidateRange() {
			if (range != null) {
				Worksheet sheet = range.Worksheet as Worksheet;
				if (sheet != null && sheet.PivotTables.ContainsItemsInRange(range, true))
					return new ModelErrorInfo(ModelErrorType.ChartDataRangeIntersectPivotTable);
			}
			return null;
		}
	}
	#endregion
}
