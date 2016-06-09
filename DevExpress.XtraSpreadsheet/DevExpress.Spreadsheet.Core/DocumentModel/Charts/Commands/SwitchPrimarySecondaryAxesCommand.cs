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
namespace DevExpress.XtraSpreadsheet.Model {
	#region SwitchPrimarySecondaryAxesCommand
	public class SwitchPrimarySecondaryAxesCommand : ErrorHandledWorksheetCommand {
		#region Fields
		readonly ISeries series;
		readonly AxisGroup axisGroup;
		readonly SwitchAxesHelper validationHelper;
		readonly SecondaryAxesBuilder axesBuilder;
		int oldViewIndex;
		#endregion
		public SwitchPrimarySecondaryAxesCommand(ISeries series, AxisGroup axisGroup, IErrorHandler errorHandler)
			: base(axisGroup.DocumentModel.MainPart, errorHandler) {
			this.axisGroup = axisGroup; 
			this.series = series;
			this.validationHelper = new SwitchAxesHelper(OldView, axisGroup);
			this.axesBuilder = new SecondaryAxesBuilder(OldView);
		}
		#region Properties
		IChartView OldView { get { return series.View; } }
		IChart Chart { get { return OldView.Parent; } }
		ChartViewCollection Views { get { return OldView.Parent.Views; } }
		#endregion
		protected internal override void ExecuteCore() {
			ClearSecondaryAxes();
			RemoveSeriesAndView();
			SwitchAxes();
		}
		#region Internal
		void ClearSecondaryAxes() {
			bool lastSecondaryAxisSeries = SwitchAxesHelper.IsLastAxisGroupSeries(Views, Chart.SecondaryAxes);
			if (axisGroup == Chart.PrimaryAxes && lastSecondaryAxisSeries)
				Chart.SecondaryAxes.Clear();
		}
		void RemoveSeriesAndView() {
			SeriesCollection oldSeriesCollection = OldView.Series;
			int serieIndex = oldSeriesCollection.IndexOf(series);
			oldSeriesCollection.RemoveAt(serieIndex);
			oldViewIndex = OldView.IndexOfView;
			if (oldSeriesCollection.Count == 0 && Views.Count > 1)
				Views.RemoveAt(oldViewIndex);
			else
				oldViewIndex++;
		}
		void SwitchAxes() {
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++) {
				ChartViewBase view = Views[i] as ChartViewBase;
				if (view.ViewType == OldView.ViewType && view.Axes == axisGroup) {
					view.Series.Add(series.CloneTo(view));
					return;
				}
			}
			IChartView newView = ((ChartViewBase)OldView).Duplicate();
			newView.Axes = CreateSecondaryAxes();
			newView.Series.Add(series.CloneTo(newView));
			Views.Insert(oldViewIndex, newView);
		}
		AxisGroup CreateSecondaryAxes() {
			if (axisGroup == Chart.PrimaryAxes || Chart.SecondaryAxes.Count > 0)
				return axisGroup;
			return axesBuilder.CreateSecondaryAxes();
		}
		#endregion
		protected internal override bool Validate() {
			if (OldView.Axes == axisGroup)
				return false;
			OldView.Visit(validationHelper);
			return HandleError(validationHelper.Result);
		}
	}
	#endregion
	#region SwitchAxesHelper
	public class SwitchAxesHelper : IChartViewVisitor {
		#region Static Members
		readonly static ModelErrorInfo InvalidViewError = new ModelErrorInfo(ModelErrorType.AxisGroupCannotBeChanged);
		internal static bool IsLastAxisGroupSeries(ChartViewCollection views, AxisGroup axisGroup) {
			int result = 0;
			int viewsCount = views.Count;
			for (int i = 0; i < viewsCount; i++) {
				IChartView view = views[i];
				bool isEqualAxes = Object.ReferenceEquals(view.Axes, axisGroup);
				if (isEqualAxes && view.Series.Count == 1)
					result++;
				else if (isEqualAxes && result == 1)
					return false;
			}
			return result == 1;
		}
		internal static void ClearSecondaryAxes(IChartView view) {
			IChart chart = view.Parent;
			AxisGroup axes = chart.SecondaryAxes;
			bool lastSecondaryAxisSeries = IsLastAxisGroupSeries(chart.Views, axes);
			if (lastSecondaryAxisSeries)
				axes.Clear();
		}
		#endregion
		#region Fields
		readonly IChart chart;
		readonly IChartView selectedView;
		readonly AxisGroup axisGroup;
		IModelErrorInfo result = null;
		#endregion
		public SwitchAxesHelper(IChartView selectedView, AxisGroup axisGroup) {
			this.selectedView = selectedView;
			this.chart = selectedView.Parent;
			this.axisGroup = axisGroup;
		}
		#region Properties
		public IModelErrorInfo Result { get { return result; } }
		ChartViewCollection Views { get { return selectedView.Parent.Views; } }
		int ViewsCount { get { return Views.Count; } }
		#endregion
		#region Helper Methods
		delegate bool SupportedViewTypeValidation(IChartView currentView);
		void ValidateViews(SupportedViewTypeValidation supportsViewType) {
			bool lastPrimaryAxisSeries = IsLastAxisGroupSeries(Views, chart.PrimaryAxes);
			if (axisGroup == chart.SecondaryAxes && lastPrimaryAxisSeries) {
				result = new ModelErrorInfo(ModelErrorType.LastPrimaryAxisCannotBeChanged);
				return;
			}
			for (int i = 0; i < ViewsCount; i++) {
				if (!supportsViewType(Views[i]))
					result = InvalidViewError;
			}
		}
		bool SupportsAreaColumnBarLine(IChartView currentView) {
			BarChartView barView = currentView as BarChartView;
			if (barView != null)
				return barView.BarDirection == BarChartDirection.Column;
			ChartViewType viewType = currentView.ViewType;
			return viewType == ChartViewType.Line || viewType == ChartViewType.Area;
		}
		bool CannotSupportOtherViews(IChartView currentView) {
			return currentView.ViewType == selectedView.ViewType;
		}
		bool BarCannotSupportOtherViews(IChartView currentView) {
			BarChartView barView = currentView as BarChartView;
			if (barView != null)
				return barView.BarDirection == BarChartDirection.Bar;
			return CannotSupportOtherViews(currentView);
		}
		#endregion
		#region IChartViewVisitor Members
		public void Visit(AreaChartView view) {
			ValidateViews(SupportsAreaColumnBarLine);
		}
		public void Visit(BarChartView view) {
			if (view.BarDirection == BarChartDirection.Column)
				ValidateViews(SupportsAreaColumnBarLine);
			else
				ValidateViews(BarCannotSupportOtherViews);
		}
		public void Visit(LineChartView view) {
			ValidateViews(SupportsAreaColumnBarLine);
		}
		public void Visit(ScatterChartView view) {
			ValidateViews(CannotSupportOtherViews);
		}
		public void Visit(BubbleChartView view) {
			ValidateViews(CannotSupportOtherViews);
		}
		public void Visit(RadarChartView view) {
			ValidateViews(CannotSupportOtherViews);
		}
		#region Unsupported
		public void Visit(DoughnutChartView view) {
			result = InvalidViewError;
		}
		public void Visit(OfPieChartView view) {
			result = InvalidViewError;
		}
		public void Visit(PieChartView view) {
			result = InvalidViewError;
		}
		public void Visit(StockChartView view) {
			result = InvalidViewError;
		}
		public void Visit(Area3DChartView view) {
			result = InvalidViewError;
		}
		public void Visit(Bar3DChartView view) {
			result = InvalidViewError;
		}
		public void Visit(Line3DChartView view) {
			result = InvalidViewError;
		}
		public void Visit(Pie3DChartView view) {
			result = InvalidViewError;
		}
		public void Visit(Surface3DChartView view) {
			result = InvalidViewError;
		}
		public void Visit(SurfaceChartView view) {
			result = InvalidViewError;
		}
		#endregion
		#endregion
	}
	#endregion
	#region SecondaryAxesBuilder
	public class SecondaryAxesBuilder : IChartViewVisitor {
		#region Fields
		readonly Chart chart;
		readonly IChartView selectedView;
		#endregion
		public SecondaryAxesBuilder(IChartView selectedView) {
			this.selectedView = selectedView;
			this.chart = selectedView.Parent as Chart;
		}
		#region Properties
		AxisPosition ArgumentAxisPosition { get; set; }
		AxisPosition ValueAxisPosition { get; set; }
		AxisCrossBetween ValueAxisCrossBetween { get; set; }
		bool ShowMajorGridlines { get; set; }
		bool IsComboChart { get { return CheckIfComboChart(); } }
		bool HasDateAxis { get { return CheckDateAxis(); } }
		#endregion
		#region Internal
		bool CheckIfComboChart() {
			ChartViewCollection views = chart.Views;
			int viewCount = views.Count;
			for (int i = 0; i < viewCount; i++) {
				if (views[i].ViewType != selectedView.ViewType)
					return true;
			}
			return false;
		}
		bool CheckDateAxis() {
			if (chart.PrimaryAxes.Count == 0)
				return false;
			DateAxis axis = chart.PrimaryAxes[0] as DateAxis;
			return axis != null;
		}
		#endregion
		#region CreateAxes
		public AxisGroup CreateSecondaryAxes() {
			selectedView.Visit(this);
			return chart.SecondaryAxes;
		}
		void CreateCatValueAxes(Chart chart) {
			AxisBase argAxis = CreateArgumentAxis();
			AxisBase valAxis = CreateValueAxis(ValueAxisPosition, ValueAxisCrossBetween, false);
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.SecondaryAxes.Add(argAxis);
			chart.SecondaryAxes.Add(valAxis);
		}
		void CreateXYAxes(Chart chart) {
			ValueAxis argAxis = CreateValueAxis(AxisPosition.Right, AxisCrossBetween.Midpoint, false);
			ValueAxis valAxis = CreateValueAxis(AxisPosition.Bottom, AxisCrossBetween.Midpoint, true);
			argAxis.CrossesAxis = valAxis;
			valAxis.CrossesAxis = argAxis;
			chart.SecondaryAxes.Add(argAxis);
			chart.SecondaryAxes.Add(valAxis);
		}
		AxisBase CreateArgumentAxis() {
			if (HasDateAxis)
				return CreateDateAxis();
			return CreateCategoryAxis();
		}
		DateAxis CreateDateAxis() {
			DateAxis axis = new DateAxis(chart);
			axis.BeginUpdate();
			try {
				axis.Auto = true;
				axis.Delete = true;
				axis.Position = ArgumentAxisPosition;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.ShowMajorGridlines = ShowMajorGridlines;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		CategoryAxis CreateCategoryAxis() {
			CategoryAxis axis = new CategoryAxis(chart);
			axis.BeginUpdate();
			try {
				axis.Auto = true;
				axis.Delete = true;
				axis.Position = ArgumentAxisPosition;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.ShowMajorGridlines = ShowMajorGridlines;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		ValueAxis CreateValueAxis(AxisPosition position, AxisCrossBetween crossBetween, bool valueDeleted) {
			ValueAxis axis = new ValueAxis(chart);
			axis.BeginUpdate();
			try {
				axis.CrossBetween = crossBetween;
				axis.Position = position;
				axis.Delete = valueDeleted;
				axis.MajorTickMark = TickMark.Outside;
				axis.MinorTickMark = TickMark.None;
				axis.TickLabelPos = TickLabelPosition.NextTo;
				axis.Crosses = AxisCrosses.Max;
			}
			finally {
				axis.EndUpdate();
			}
			return axis;
		}
		void SetupAxisNumberFormat(AxisBase axis, int formatId) {
			axis.NumberFormat.NumberFormatCode = axis.DocumentModel.Cache.NumberFormatCache[formatId].FormatCode;
			axis.NumberFormat.SourceLinked = true;
		}
		void SetupAxisNumberFormat(ChartGrouping grouping) {
			if (grouping == ChartGrouping.PercentStacked)
				SetupAxisNumberFormat(chart.SecondaryAxes[1], ChartBuilderBase.PercentageFormatId);
		}
		void SetupAxisNumberFormatForBars(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.PercentStacked)
				SetupAxisNumberFormat(chart.SecondaryAxes[1], ChartBuilderBase.PercentageFormatId);
		}
		void SetupPositionAndCrossBetween(AxisPosition valueAxisPosition, AxisPosition argumentAxisPosition, AxisCrossBetween crossBetween) {
			ValueAxisPosition = valueAxisPosition;
			ArgumentAxisPosition = argumentAxisPosition;
			ValueAxisCrossBetween = crossBetween;
		}
		#endregion
		#region IChartViewVisitor Members
		public void Visit(AreaChartView view) {
			if (IsComboChart)
				SetupPositionAndCrossBetween(AxisPosition.Right, AxisPosition.Bottom, AxisCrossBetween.Between);
			else 
				SetupPositionAndCrossBetween(AxisPosition.Right, AxisPosition.Bottom, AxisCrossBetween.Midpoint);
			CreateCatValueAxes(chart);
			SetupAxisNumberFormat(view.Grouping);
		}
		public void Visit(BarChartView view) {
			if (view.BarDirection == BarChartDirection.Bar)
				SetupPositionAndCrossBetween(AxisPosition.Top, AxisPosition.Left, AxisCrossBetween.Between);
			else
				SetupPositionAndCrossBetween(AxisPosition.Right, AxisPosition.Bottom, AxisCrossBetween.Between);
			CreateCatValueAxes(chart);
			SetupAxisNumberFormatForBars(view.Grouping);
		}
		public void Visit(LineChartView view) {
			SetupPositionAndCrossBetween(AxisPosition.Right, AxisPosition.Bottom, AxisCrossBetween.Between);
			CreateCatValueAxes(chart);
			SetupAxisNumberFormat(view.Grouping);
		}
		public void Visit(RadarChartView view) {
			SetupPositionAndCrossBetween(AxisPosition.Left, AxisPosition.Bottom, AxisCrossBetween.Between);
			ShowMajorGridlines = true;
			CreateCatValueAxes(chart);
		}
		public void Visit(ScatterChartView view) {
			CreateXYAxes(chart);
		}
		public void Visit(BubbleChartView view) {
			CreateXYAxes(chart);
		}
		#region Unsupported
		public void Visit(DoughnutChartView view) {
		}
		public void Visit(OfPieChartView view) {
		}
		public void Visit(PieChartView view) {
		}
		public void Visit(StockChartView view) {
		}
		public void Visit(Area3DChartView view) {
		}
		public void Visit(Bar3DChartView view) {
		}
		public void Visit(Line3DChartView view) {
		}
		public void Visit(Pie3DChartView view) {
		}
		public void Visit(Surface3DChartView view) {
		}
		public void Visit(SurfaceChartView view) {
		}
		#endregion
		#endregion
	}
	#endregion
}
