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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardChartControlViewer : DashboardChartControlViewerBase {
		static ChartSeriesViewModelType[] CustomizeSeriesColorsSeriesTypes {
			get {
				return new ChartSeriesViewModelType[] {
					ChartSeriesViewModelType.Line,
					ChartSeriesViewModelType.StackedLine,
					ChartSeriesViewModelType.FullStackedLine,
					ChartSeriesViewModelType.StepLine,
					ChartSeriesViewModelType.Spline,
					ChartSeriesViewModelType.Area,
					ChartSeriesViewModelType.FullStackedArea,
					ChartSeriesViewModelType.FullStackedSplineArea,
					ChartSeriesViewModelType.SplineArea,
					ChartSeriesViewModelType.StackedArea,
					ChartSeriesViewModelType.StackedSplineArea,
					ChartSeriesViewModelType.StepArea,
					ChartSeriesViewModelType.RangeArea
				};
			}
		}
		struct PaneAxisY {
			readonly XYDiagramPane pane;
			readonly SecondaryAxisY primaryAxisY;
			readonly SecondaryAxisY secondaryAxisY;
			public XYDiagramPane Pane { get { return pane; } }
			public SecondaryAxisY PrimaryAxisY { get { return primaryAxisY; } }
			public SecondaryAxisY SecondaryAxisY { get { return secondaryAxisY; } }
			public PaneAxisY(XYDiagramPane pane, SecondaryAxisY primaryAxisY, SecondaryAxisY secondaryAxisY) {
				this.pane = pane;
				this.primaryAxisY = primaryAxisY;
				this.secondaryAxisY = secondaryAxisY;
			}
		}
		static void SetPointLabelOptions(ChartSeriesTemplateViewModel viewModel, Series series) {
			if(viewModel.PointLabel != null) {
				SeriesLabelBase label = series.Label;
				series.LabelsVisibility = viewModel.PointLabel.ShowPointLabels ? DefaultBoolean.True : DefaultBoolean.False;
				if(viewModel.PointLabel.ShowPointLabels) {
					BarSeriesLabel barSeriesLabel = label as BarSeriesLabel;
					if(barSeriesLabel != null) {
						barSeriesLabel.ShowForZeroValues = viewModel.PointLabel.ShowForZeroValues;
						if(SeriesViewFactory.GetViewType(series.View) == ViewType.Bar) {
							switch(viewModel.PointLabel.Position) {
								case PointLabelPosition.Inside:
									barSeriesLabel.Position = BarSeriesLabelPosition.Center;
									break;
								default:
									barSeriesLabel.Position = BarSeriesLabelPosition.Top;
									break;
							}
						}
					}
					BubbleSeriesLabel bubbleLabel = label as BubbleSeriesLabel;
					if(bubbleLabel != null) {
						switch(viewModel.PointLabel.Position) {
							case PointLabelPosition.Inside:
								bubbleLabel.Position = DevExpress.XtraCharts.PointLabelPosition.Center;
								break;
							default:
								bubbleLabel.Position = DevExpress.XtraCharts.PointLabelPosition.Outside;
								break;
						}
					}
					switch(viewModel.PointLabel.OverlappingMode) {
						case PointLabelOverlappingMode.Hide:
							label.ResolveOverlappingMode = ResolveOverlappingMode.HideOverlapped;
							break;
						case PointLabelOverlappingMode.None:
							label.ResolveOverlappingMode = ResolveOverlappingMode.None;
							break;
						default:
							label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
							break;
					}
					switch(viewModel.PointLabel.Orientation) {
						case PointLabelOrientation.RotateLeft:
							label.TextOrientation = TextOrientation.BottomToTop;
							break;
						case PointLabelOrientation.RotateRight:
							label.TextOrientation = TextOrientation.TopToBottom;
							break;
						default:
							label.TextOrientation = TextOrientation.Horizontal;
							break;
					}
				}
			}
		}
		readonly Dictionary<Series, Color> seriesCustomColorTable = new Dictionary<Series, Color>();
		readonly ChartElementOrderedCache<PaneAxisY> paneAxisYCache = new ChartElementOrderedCache<PaneAxisY>();
		ChartLabelsConfiguratorBase labelsConfigurator;
		ChartDashboardItemViewModel ChartViewModel { get { return (ChartDashboardItemViewModel)ViewModel; } }
		public override bool ShouldProcessInteractivity { get { return true; } }
		public override ChartLabelsConfiguratorBase LabelsConfigurator { get { return labelsConfigurator; } }
		AxesChartViewerDataController ChartDataController { get { return (AxesChartViewerDataController)DataController; } }
		string ArgumentDataMember {
			get {
				ChartDashboardItemViewModel chartViewModel = (ChartDashboardItemViewModel)ViewModel;
				return chartViewModel.Argument.SummaryArgumentMember ?? ChartArgumentMultiDimensionalDataPropertyDescriptor.EmptyArgumentMember;
			}
		}
		bool IgnoreEmptyArgument {
			get {
				ChartDashboardItemViewModel chartViewModel = (ChartDashboardItemViewModel)ViewModel;
				if(string.IsNullOrEmpty(chartViewModel.Argument.SummaryArgumentMember))
					return false;
				return chartViewModel.Argument.Type != ChartArgumentType.String;
			}
		}
		public DashboardChartControlViewer(IDashboardChartControl chartControl)
			: base(chartControl) {
			labelsConfigurator = CreateLabelsConfigurator();
		}
		public override void Update(DashboardItemViewModel viewModel, MultiDimensionalData data, IDictionary<string, IList> drillDownState, bool isDesignMode) {
			base.Update(viewModel, data, drillDownState, isDesignMode);
			UpdateAxisXVisualRange();
			foreach(Series series in ChartControl.Series) {
				if(!series.ShowInLegend)
					series.View.Color = GetSeriesColor(series);
			}
		}
		protected virtual ChartLabelsConfiguratorBase CreateLabelsConfigurator() {
			return new ChartLabelsConfigurator(this);
		}
		void UpdateLabelsConfigurator() {
			labelsConfigurator.Update(ChartViewModel, DataController.ArgumentType);
		}
		void UpdateAxisYTitles() {
			foreach(KeyValuePair<object, PaneAxisY> pane in paneAxisYCache) {
				ChartPaneViewModel paneViewModel = (ChartPaneViewModel)(pane.Key);
				PaneAxisY paneAxisY = (PaneAxisY)pane.Value;
				SecondaryAxisY primaryAxisY = paneAxisY.PrimaryAxisY;
				if(primaryAxisY != null) {
					AxisTitle axisTitle = primaryAxisY.Title;
					axisTitle.Visibility = paneViewModel.PrimaryAxisY.Title != null ? DefaultBoolean.True : DefaultBoolean.False;
					if(axisTitle.Visibility == DefaultBoolean.True)
						axisTitle.Text = paneViewModel.PrimaryAxisY.Title;
				}
				SecondaryAxisY secondaryAxisY = paneAxisY.SecondaryAxisY;
				if(paneViewModel.SecondaryAxisY != null && secondaryAxisY != null) {
					AxisTitle axisTitle = secondaryAxisY.Title;
					axisTitle.Visibility = paneViewModel.SecondaryAxisY.Title != null ? DefaultBoolean.True : DefaultBoolean.False;
					if(axisTitle.Visibility == DefaultBoolean.True)
						axisTitle.Text = paneViewModel.SecondaryAxisY.Title;
				}
			}
		}
		void SetMarkerVisibility(ChartSeriesTemplateViewModel viewModel, Series series) {
			DefaultBoolean showPointMarkers = viewModel.ShowPointMarkers ? DefaultBoolean.True : DefaultBoolean.False;
			LineSeriesView line = series.View as LineSeriesView;
			if(line != null)
				line.MarkerVisibility = showPointMarkers;
			RangeAreaSeriesView rangeArea = series.View as RangeAreaSeriesView;
			if(rangeArea != null)
				rangeArea.Marker1Visibility = rangeArea.Marker2Visibility = showPointMarkers;
		}
		void UpdateSeries(ChartPaneViewModel paneViewModel, ChartSeriesTemplateViewModel viewModel, Series series) {
			PaneAxisY paneAxisY = paneAxisYCache[paneViewModel];
			XYDiagramPane pane = paneAxisY.Pane;
			XYDiagramSeriesViewBase xyView = series.View as XYDiagramSeriesViewBase;
			if(xyView != null) {
				xyView.Pane = pane;
				xyView.AxisY = viewModel.PlotOnSecondaryAxis ? paneAxisY.SecondaryAxisY : paneAxisY.PrimaryAxisY;
			}
			SetMarkerVisibility(viewModel, series);
			SetPointLabelOptions(viewModel, series);
			series.ShowInLegend = series.View is StockSeriesView || series.View is CandleStickSeriesView;
		}
		void SetLegendAlignment(ChartLegendViewModel legend) {
			IDashboardChartLegend controlLegend = ChartControl.Legend;
			controlLegend.Visibility = legend.Visible ? DefaultBoolean.Default : DefaultBoolean.False;
			if(legend.IsInsideDiagram) {
				switch(legend.InsidePosition) {
					case ChartLegendInsidePosition.TopLeftHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.TopCenterHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.TopRightHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.BottomLeftHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.BottomCenterHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.BottomRightHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendInsidePosition.TopLeftVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendInsidePosition.TopCenterVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendInsidePosition.TopRightVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendInsidePosition.BottomLeftVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendInsidePosition.BottomCenterVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendInsidePosition.BottomRightVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
				}
			}
			else {
				switch(legend.OutsidePosition) {
					case ChartLegendOutsidePosition.TopLeftHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.TopCenterHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.TopRightHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.BottomLeftHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.BottomCenterHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.BottomRightHorizontal:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
						controlLegend.Direction = LegendDirection.LeftToRight;
						break;
					case ChartLegendOutsidePosition.TopLeftVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendOutsidePosition.TopRightVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Top;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendOutsidePosition.BottomLeftVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
					case ChartLegendOutsidePosition.BottomRightVertical:
						controlLegend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
						controlLegend.AlignmentVertical = LegendAlignmentVertical.Bottom;
						controlLegend.Direction = LegendDirection.TopToBottom;
						break;
				}
			}
		}
		protected override void UpdateElementCustomColorsInternal() {
			seriesCustomColorTable.Clear();
			foreach(Series series in ChartControl.Series) {
				ChartSeriesTemplateViewModel seriesViewModel = GetSeriesViewModel(series);
				if(seriesViewModel != null && CustomizeSeriesColorsSeriesTypes.Contains(seriesViewModel.SeriesType)) {
					ElementCustomColorEventArgs eventArgs = ChartDataController.PrepareElementCustomColorEventArgs(series);
					RaiseElementCustomColor(eventArgs);
					if(eventArgs.ColorChanged) {
						seriesCustomColorTable.Add(series, eventArgs.Color);
						series.View.Color = eventArgs.Color;
					}
				}
			}
			base.UpdateElementCustomColorsInternal();
		}
		XYDiagram CreateDiagram(ChartAxisViewModel axisXViewModel, bool diagramRotated) {
			XYDiagram diagram = new XYDiagram();
			diagram.PaneDistance = 5;
			diagram.DefaultPane.Visible = false;
			diagram.Rotated = diagramRotated;
			diagram.PaneLayoutDirection = diagramRotated ? PaneLayoutDirection.Horizontal : PaneLayoutDirection.Vertical;
			diagram.AxisY.Visibility = DefaultBoolean.False;
			diagram.AxisY.GridLines.Visible = false;
			diagram.EnableAxisXZooming = axisXViewModel.EnableZooming;
			diagram.EnableAxisXScrolling = axisXViewModel.EnableZooming || axisXViewModel.LimitVisiblePoints;
			return diagram;
		}
		SecondaryAxisY CreateAxisY(string paneName, ChartAxisViewModel viewModel, AxisAlignment alignment) {
			SecondaryAxisY axisY = new SecondaryAxisY(paneName);
			ConfigureAxis(axisY, viewModel);
			axisY.Alignment = alignment;
			axisY.Visibility = viewModel.Visible ? DefaultBoolean.True : DefaultBoolean.False;
			axisY.WholeRange.AlwaysShowZeroLevel = viewModel.ShowZeroLevel;
			axisY.Label.ResolveOverlappingOptions.AllowRotate = false;
			axisY.Label.ResolveOverlappingOptions.AllowStagger = false;
			return axisY;
		}
		void ConfigureAxis(Axis axis, ChartAxisViewModel viewModel) {
			axis.Reverse = viewModel.Reverse;
			axis.GridLines.Visible = viewModel.ShowGridLines;
			axis.Logarithmic = viewModel.Logarithmic;
			if(axis.Logarithmic)
				axis.LogarithmicBase = viewModel.LogarithmicBase;
		}
		protected override void UpdateViewModelInternal() {
			paneAxisYCache.Clear();
			PerformChartOperation(() => {
				ChartDashboardItemViewModel chartViewModel = ChartViewModel;
				SetLegendAlignment(chartViewModel.Legend);
				XYDiagram diagram = CreateDiagram(chartViewModel.AxisX, chartViewModel.Rotated);
				ChartControl.Diagram = diagram;
			});
		}
		void AddLegendSeries() {
			IList<Tuple<string, Color>> legendSeriesDict = ChartDataController.GetLegendInfo();
			foreach(Tuple<string, Color> tuple in legendSeriesDict) {
				Series legendSeries = new Series();
				AddSeries(legendSeries, null, null);
				legendSeries.LegendText = tuple.Item1;
				legendSeries.View.Color = tuple.Item2;
			}
		}
		public Color GetSeriesColor(Series series) {
			Color color = Color.Empty;
			return seriesCustomColorTable.TryGetValue(series, out color) ? ChartDataController.PrepareSeriesColor(series, color) : ChartDataController.GetColor(series);
		}
		public override Color GetPointColor(Series series, SeriesPoint seriesPoint) {
			if(ChartViewModel.ArgumentColorDimension == null && !IsCustomElementColor(seriesPoint))
				return GetSeriesColor(series);
			return base.GetPointColor(series, seriesPoint);
		}
		protected override void UpdateDataInternal() {
			PerformChartOperation(() => {
				PrepareSeries((ChartDashboardItemViewModel)ViewModel);
			});
			UpdateLabelsConfigurator();
		}
		void SetAxisMargins(AxisYBase axisY) {
			if(AllSeriesFullStacked(axisY)) {
				axisY.VisualRange.SideMarginsValue = 0;
				axisY.VisualRange.AutoSideMargins = false;
				axisY.WholeRange.SideMarginsValue = 0;
				axisY.WholeRange.AutoSideMargins = false;
			}
		}
		void PrepareSeries(ChartDashboardItemViewModel chartViewModel) {
			ClearSeries();
			if(DataController.IsEmpty)
				return;
			string seriesDataMember = chartViewModel.SummarySeriesMember;
			IEnumerable<AxisPoint> axisPoints = GetSeriesAxisPoints(seriesDataMember);
			foreach(ChartPaneViewModel chartPane in chartViewModel.Panes) {
				string paneName = !string.IsNullOrEmpty(chartPane.Name) ? chartPane.Name : "Pane";
				SecondaryAxisY primaryAxisY = CreateAxisY(paneName, chartPane.PrimaryAxisY, AxisAlignment.Near);
				XYDiagram diagram = (XYDiagram)ChartControl.Diagram;
				diagram.SecondaryAxesY.Add(primaryAxisY);
				SecondaryAxisY secondaryAxisY = null;
				if(chartPane.SecondaryAxisY != null) {
					secondaryAxisY = CreateAxisY(paneName, chartPane.SecondaryAxisY, AxisAlignment.Far);
					diagram.SecondaryAxesY.Add(secondaryAxisY);
				}
				XYDiagramPane pane = new XYDiagramPane(paneName);
				pane.ScrollBarOptions.XAxisScrollBarAlignment = chartViewModel.Rotated ? ScrollBarAlignment.Far : ScrollBarAlignment.Near;
				paneAxisYCache.Add(chartPane, chartViewModel.Panes, new PaneAxisY(pane, primaryAxisY, secondaryAxisY));
				diagram.Panes.Add(pane);
				foreach(ChartSeriesTemplateViewModel seriesViewModel in chartPane.SeriesTemplates) {
					ChartSeriesConfigurator configurator = ChartSeriesConfigurator.CreateInstance(seriesViewModel, ArgumentDataMember, IgnoreEmptyArgument);
					foreach(AxisPoint axisPoint in axisPoints) {
						string seriesName = GetSeriesName(seriesViewModel, axisPoint, chartPane.SpecifySeriesTitlesWithSeriesName);
						Series series = configurator.CreateSeries(axisPoint, seriesName);
						series.DataSource = DataController.GetDataSource(series, seriesViewModel);
						AddSeries(series, seriesViewModel, configurator);
						UpdateSeries(chartPane, seriesViewModel, series);
					}
				}
				SetAxisMargins(primaryAxisY);
				if(secondaryAxisY != null) {
					SetAxisMargins(secondaryAxisY);
				}
			}
			if(ChartControl.Series.Count > 0) {
				if(HasIgnoreEmptyPoints)
					AddSortingSeries(axisPoints.First()); 
				if(chartViewModel.Legend.Visible)
					AddLegendSeries();
			}
			UpdateAxisYTitles();
			AxisX axisX = ((XYDiagram)ChartControl.Diagram).AxisX;
			ChartAxisViewModel axisXViewModel = chartViewModel.AxisX;
			if(axisXViewModel != null) {
				ConfigureAxis(axisX, axisXViewModel);
				axisX.Visibility = axisXViewModel.Visible ? DefaultBoolean.Default : DefaultBoolean.False;
				axisX.Title.Visibility = axisXViewModel.Title != null ? DefaultBoolean.Default : DefaultBoolean.False;
				axisX.Title.Text = axisX.Title.Visibility == DefaultBoolean.Default ? axisXViewModel.Title : string.Empty;
			}
		}
		void AddSortingSeries(AxisPoint point) {
			Series series = new Series();
			series.ArgumentDataMember = ArgumentDataMember;
			series.ValueDataMembers[0] = ChartSortingPropertyDescriptor.PropertyName;
			series.ShowInLegend = false;
			series.Tag = point;
			series.DataSource = ChartDataController.GetDataSource(point);
			ChartControl.Series.Insert(0, series);
		}
		void UpdateAxisXVisualRange() {
			if(ChartControl.Diagram != null) {
				XYDiagram diagram = (XYDiagram)ChartControl.Diagram;
				ChartAxisViewModel axisXViewModel = ((ChartDashboardItemViewModel)ViewModel).AxisX;
				if(axisXViewModel.LimitVisiblePoints) {
					ChartMultiDimensionalDataSourceBase dataSource = ChartDataController.GetDataSource();
					if(dataSource.Count > axisXViewModel.VisiblePointsCount) {
						PropertyDescriptor argumentDescriptor = dataSource.Properties[ArgumentDataMember];
						diagram.AxisX.VisualRange.MaxValue = argumentDescriptor.GetValue(dataSource[axisXViewModel.VisiblePointsCount - 1]);
					}
				}
			}
		}
		public IList<object> GetAxisPointUniqueValue(object value) {
			ChartMultiDimensionalDataSourceBase dataSource = ChartDataController.GetDataSource();
			PropertyDescriptor argumentDescriptor = dataSource.Properties[ArgumentDataMember];
			foreach(AxisPoint axisPoint in dataSource) {
				if(argumentDescriptor.GetValue(axisPoint).ToString() == value.ToString()) 
					return axisPoint.RootPath.Select(point => point.UniqueValue).ToList();
			}
			return null;
		}
		public object GetArgumentValue(IList axisPointUniqueValues) {
			ChartMultiDimensionalDataSourceBase dataSource = ChartDataController.GetDataSource();
			PropertyDescriptor argumentDescriptor = dataSource.Properties[ArgumentDataMember];
			foreach(AxisPoint axisPoint in dataSource) {
				if(Helper.DataEqualsWithConversion(axisPoint.RootPath.Select(point => point.UniqueValue).ToList(), axisPointUniqueValues))
					return argumentDescriptor.GetValue(axisPoint);
			}
			return null;
		}
		string GetSeriesName(ChartSeriesTemplateViewModel seriesViewModel, AxisPoint axisPoint, bool specifySeriesTitleWithSeriesName) {
			string seriesTitle;
			if(ChartViewModel.SummarySeriesMember != null)
				seriesTitle = String.Join(" - ", DataController.GetRootPathDisplayTexts(axisPoint));
			else
				seriesTitle = String.IsNullOrEmpty(seriesViewModel.Name) ? "Series" : seriesViewModel.Name;
			if(specifySeriesTitleWithSeriesName)
				seriesTitle = String.Format("{0} - {1}", seriesTitle, seriesViewModel.Name);
			return seriesTitle;
		}
		protected override void CompleteChartOperation() {
			base.CompleteChartOperation();
			XYDiagram diagram = ChartControl.Diagram as XYDiagram;
			if(diagram != null) {
				AxisX axisX = diagram.AxisX;
				XYDiagramPaneCollection panes = diagram.Panes;
				for(int i = 0; i < panes.Count; i++)
					axisX.SetVisibilityInPane(false, panes[i]);
				axisX.SetVisibilityInPane(true, panes[diagram.Rotated ? 0 : panes.Count - 1]);
			}
		}
		protected override void InitializeControlInternal() {
			base.InitializeControlInternal();
			IDashboardChartLegend legend = ChartControl.Legend;
			legend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
			legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
			legend.Direction = LegendDirection.LeftToRight;
			legend.BorderVisibility = DefaultBoolean.False;
		}
	}
	public class DashboardScatterChartControlViewer : DashboardChartControlViewer {
		ScatterChartDashboardItemViewModel ScatterViewModel { get { return (ScatterChartDashboardItemViewModel)ViewModel; } }
		public DashboardScatterChartControlViewer(IDashboardChartControl chartControl)
			: base(chartControl) {
		}
		protected override ChartLabelsConfiguratorBase CreateLabelsConfigurator() {
			return new ScatterChartLabelsConfigurator(this);
		}
		public override bool IsPercentAxis(AxisBase axis) {
			if(axis is AxisXBase)
				return ScatterViewModel.AxisXPercentValues;
			if(axis is AxisYBase) {
				ChartPaneViewModel pane = ScatterViewModel.Panes[0];
				return pane.SeriesTemplates.Count > 0 && pane.SeriesTemplates[0].OnlyPercentValues;
			}
			return false;
		}	
	}
}
