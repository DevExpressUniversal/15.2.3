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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	[Flags]
	public enum ChartInteractivityMode { 
		None = 0,
		Series = 1,
		Argument = 2,
		Point = Series | Argument
	}
	public abstract class DashboardChartControlViewerBase {
		protected const string EmptyArgumentMember = "Argument";
		readonly Dictionary<SeriesPoint, Color> pointCustomColorTable = new Dictionary<SeriesPoint, Color>();
		readonly ChartSeriesConfiguratorCache configuratorCache = new ChartSeriesConfiguratorCache();
		readonly IDashboardChartControl chartControl;
		readonly List<AxisPointTuple> highlight = new List<AxisPointTuple>();
		readonly List<AxisPointTuple> selection = new List<AxisPointTuple>();
		DashboardItemViewModel viewModel;
		ChartInteractivityMode interactivityMode;
		ChartViewerDataControllerBase dataController;
		public abstract ChartLabelsConfiguratorBase LabelsConfigurator { get; }
		public IDashboardChartControl ChartControl { get { return chartControl; } }
		protected DashboardItemViewModel ViewModel { get { return viewModel; } }
		protected ChartDashboardItemBaseViewModel ChartBaseViewModel { get { return ViewModel as ChartDashboardItemBaseViewModel; } }
		public ChartViewerDataControllerBase DataController { get { return dataController; } }
		public abstract bool ShouldProcessInteractivity { get; }
		public bool IsDataReduced { get { return dataController.IsDataReduced; } }
		public ChartInteractivityMode InteractivityMode { 
			get { return interactivityMode; } 
			set { 
				interactivityMode = value;
				UpdateInteractivityMode();
			} 
		}
		public event EventHandler<ElementCustomColorEventArgs> ElementCustomColor;
		protected DashboardChartControlViewerBase(IDashboardChartControl chartControl) {
			this.chartControl = chartControl;
			this.chartControl.CustomDrawSeriesPoint += OnCustomDrawSeriesPoint;
		}
		public IList GetArgumentUniqueValues(SeriesPoint seriesPoint) {
			return DataController.GetArgumentUniqueValues(seriesPoint);
		}
		public IList GetSeriesUniqueValues(Series series) {
			return DataController.GetSeriesUniqueValue(series);
		}
		public ChartSeriesTemplateViewModel GetSeriesViewModel(Series series) {
			return configuratorCache.GetViewModel(series);
		}
		public ChartSeriesConfigurator GetSeriesConfigurator(Series series) {
			return configuratorCache.GetConfigurator(series);
		}
		public IList<string> GetValueDataMembers(Series series, SeriesPoint seriesPoint) {
			return DataController.GetValueDataMembers(series, seriesPoint);
		}
		public void Update(DashboardItemViewModel viewModel, MultiDimensionalData data, IDictionary<string, IList> drillDownState) {
			Update(viewModel, data, drillDownState, false);
		}
		public virtual void Update(DashboardItemViewModel viewModel, MultiDimensionalData data, IDictionary<string, IList> drillDownState, bool isDesignMode) {
			dataController = ChartViewerDataControllerBase.CreateInstance(viewModel, data, configuratorCache, drillDownState, isDesignMode);
			PerformChartOperation(() => {
				UpdateViewModel(viewModel);
				UpdateData();
			});
			UpdateElementCustomColor();
			UpdateInteractivityMode();
			highlight.Clear();
			selection.Clear();
		}
		public virtual Color GetPointColor(Series series, SeriesPoint seriesPoint) {
			Color color = Color.Empty;
			if(!pointCustomColorTable.TryGetValue(seriesPoint, out color))
				color = DataController.GetColor(series, seriesPoint);
			return color;
		}
		protected bool IsCustomElementColor(SeriesPoint seriesPoint) {
			return pointCustomColorTable.ContainsKey(seriesPoint);
		}
		public void InitializeControl() {
			PerformChartOperation(() => InitializeControlInternal());
		}
		public void HighlightValues(List<AxisPointTuple> newHighlight) {
			if(ShouldProcessInteractivity) {
				highlight.Clear();
				highlight.AddRange(newHighlight);
				chartControl.Invalidate();
			}
		}
		public void SelectValues(List<AxisPointTuple> newSelection) {
			if(ShouldProcessInteractivity) {
				selection.Clear();
				selection.AddRange(newSelection);
				chartControl.Invalidate();
			}
		}
		IEnumerable<Series> GetSeriesByAxis(AxisYBase axisY) {
			return configuratorCache.Keys.Where(series => ((XYDiagramSeriesViewBase)series.View).AxisY == axisY);
		}
		public virtual bool IsPercentAxis(AxisBase axisY) {
			return GetSeriesByAxis(axisY as AxisYBase).All(series => { 
					ChartSeriesConfigurator configurator = configuratorCache[series];
					return configurator.OnlyPercentValues || IsFullStackedSeriesType(configurator.ViewModel.SeriesType);
				});
		}
		protected bool AllSeriesFullStacked(AxisYBase axisY) {
			return GetSeriesByAxis(axisY).All(series => {
				ChartSeriesConfigurator configurator = configuratorCache[series];
				return IsFullStackedSeriesType(configurator.ViewModel.SeriesType);
			});
		}
		protected bool HasIgnoreEmptyPoints {
			get {
				return configuratorCache.Keys.Any(series => {
					ChartSeriesConfigurator configurator = configuratorCache[series];
					return configurator.ViewModel.IgnoreEmptyPoints;
				});
			}
		}
		bool IsFullStackedSeriesType(ChartSeriesViewModelType seriesType) {
			switch(seriesType){
				case ChartSeriesViewModelType.FullStackedArea:
				case ChartSeriesViewModelType.FullStackedBar:
				case ChartSeriesViewModelType.FullStackedLine:
				case ChartSeriesViewModelType.FullStackedSplineArea:
					return true;
				default:
					return false;
			}
		}
		protected Type GetArgumentType(ChartArgumentType type) {
			switch(type) {
				case ChartArgumentType.DateTime:
				return typeof(DateTime);
				case ChartArgumentType.Integer:
				return typeof(int);
				case ChartArgumentType.Float:
				return typeof(float);
				case ChartArgumentType.Double:
				return typeof(double);
				case ChartArgumentType.Decimal:
				return typeof(decimal);
				case ChartArgumentType.String:
				default:
				return typeof(string);
			}
		}
		protected IEnumerable<AxisPoint> GetSeriesAxisPoints(string id) {
			return DataController.GetSeriesAxisPoints(id);
		}
		protected abstract void UpdateViewModelInternal();
		protected abstract void UpdateDataInternal();
		protected virtual void CompleteChartOperation() {
		}
		protected virtual void InitializeControlInternal() {
			chartControl.BorderVisible = false;
		}
		protected void PerformChartOperation(Action action) {
			chartControl.BeginUpdate();
			try {
				action();
			} finally {
				chartControl.EndUpdate();
			}
			if(!chartControl.IsInUpdate)
				CompleteChartOperation();
		}
		protected void AddSeries(Series series, ChartSeriesTemplateViewModel viewModel, ChartSeriesConfigurator configurator) {
			chartControl.Series.Add(series);
			if(configurator != null)
				configuratorCache.Add(series, configurator);
			BarSeriesView barView = series.View as BarSeriesView;
			if(barView != null)
				barView.FillStyle.FillMode = FillMode.Solid;
			PieSeriesView pieView = series.View as PieSeriesView;
			if(pieView != null)
				pieView.FillStyle.FillMode = FillMode.Solid;
		}
		protected void ClearSeries() {
			configuratorCache.Clear();
			chartControl.Series.Clear();
		}
		void UpdateElementCustomColor() {
			if(ElementCustomColor != null && ShouldProcessInteractivity)
				UpdateElementCustomColorsInternal();
		}
		protected virtual void UpdateElementCustomColorsInternal() {
			pointCustomColorTable.Clear();
			foreach(Series series in chartControl.Series) {
				foreach(SeriesPoint seriesPoint in series.Points) {
					ElementCustomColorEventArgs eventArgs = dataController.PrepareElementCustomColorEventArgs(series, seriesPoint);
					RaiseElementCustomColor(eventArgs);
					if(eventArgs.ColorChanged) {
						pointCustomColorTable.Add(seriesPoint, eventArgs.Color);
						seriesPoint.Color = eventArgs.Color;
					}
				}
			}
		}
		protected void RaiseElementCustomColor(ElementCustomColorEventArgs eventArgs) {
			ElementCustomColor(this, eventArgs);
		}
		void UpdateViewModel(DashboardItemViewModel viewModel) {
			this.viewModel = viewModel;
			UpdateViewModelInternal();
		}
		void UpdateInteractivityMode() {
			chartControl.SeriesPointSelectionEnabled = interactivityMode != ChartInteractivityMode.Series;
		}
		void UpdateData() {
			UpdateDataInternal();
		}
		void OnCustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e) {
			if(ShouldProcessInteractivity) {
				bool hasSelection = CheckSeriesPointValues(e, selection);
				bool hasHover = CheckSeriesPointValues(e, highlight);
				if(hasSelection)
					e.SelectionState = SelectionState.Selected;
				else if(hasHover)
					e.SelectionState = SelectionState.HotTracked;
				else
					e.SelectionState = SelectionState.Normal;
			}
		}
		bool CheckSeriesPointValues(CustomDrawSeriesPointEventArgs e, List<AxisPointTuple> tuples) {
			if(ChartBaseViewModel == null || tuples == null || tuples.Count == 0 || interactivityMode == ChartInteractivityMode.None)
				return false;
			foreach(AxisPointTuple tuple in tuples) {
				bool argumentFit = false;
				bool seriesFit = false;
				SeriesPoint seriesPoint = e.SeriesPoint;
				if(seriesPoint != null) 
					argumentFit = !tuple.AvailableAxisNames.Contains(DashboardDataAxisNames.ChartArgumentAxis) || DataController.GetArgumentAxisPoint(e.SeriesPoint) == tuple.GetAxisPoint(DashboardDataAxisNames.ChartArgumentAxis);
				Series series = e.Series;
				if(series != null)
					seriesFit = !tuple.AvailableAxisNames.Contains(DashboardDataAxisNames.ChartSeriesAxis) || DataController.GetSeriesAxisPoint(series) == tuple.GetAxisPoint(DashboardDataAxisNames.ChartSeriesAxis);
				if(interactivityMode == ChartInteractivityMode.Argument && argumentFit)
					return true;
				if(interactivityMode == ChartInteractivityMode.Series && seriesFit)
					return true;
				if(interactivityMode == ChartInteractivityMode.Point && seriesFit && argumentFit)
					return true;
			}
			return false;
		}
	}
}
