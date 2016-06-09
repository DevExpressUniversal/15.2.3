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
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardRangeFilterControlViewer : DashboardChartControlViewerBase {
		readonly RangeFilterLabelsConfigurator labelsConfigurator;
		readonly IRangeControlClientExtension rangeControlClient;
		readonly IRangeControl rangeControl;
		RangeFilterDashboardItemViewModel RangeFilterViewModel { get { return (RangeFilterDashboardItemViewModel)ViewModel; } }
		RangeFilterViewerDataController RangeFilterDataController { get { return (RangeFilterViewerDataController)DataController; } }
		public override bool ShouldProcessInteractivity { get { return false; } }
		public IRangeControlClientExtension RangeControlClient { get { return rangeControlClient; } }
		public override ChartLabelsConfiguratorBase LabelsConfigurator { get { return labelsConfigurator; } }
		public object[] SelectedRangeValues {
			get {
				XYDiagram diagram = ChartControl.Diagram as XYDiagram;
				if(diagram != null) {
					RangeControlRange range = rangeControl.SelectedRange;
					if(!object.Equals(range.Maximum, double.NaN) && !object.Equals(range.Minimum, double.NaN)) {
						object minValue = rangeControlClient.NativeValue(rangeControlClient.GetNormalizedValue(range.Minimum));
						object maxValue = rangeControlClient.NativeValue(rangeControlClient.GetNormalizedValue(range.Maximum));
						return new object[] { RangeFilterDataController.GetArgumentValue(minValue), RangeFilterDataController.GetArgumentValue(maxValue) };
					}
				}
				return new object[] { null, null };
			}
		}
		public DashboardRangeFilterControlViewer(IRangeControl rangeControl, IDashboardChartControl chartControl)
			: base(chartControl) {
			this.labelsConfigurator = new RangeFilterLabelsConfigurator(this);
			this.rangeControlClient = new DashboardRangeControlClientImplementation(chartControl, labelsConfigurator);
			this.rangeControl = rangeControl;
			rangeControl.Client = rangeControlClient;
		}
		public void UpdateMinMaxValues(object min, object max) {
			XYDiagram diagram = ChartControl.Diagram as XYDiagram;
			if(diagram != null) {
				diagram.AxisX.VisualRange.SetMinMaxValues(RangeFilterDataController.GetRangeValue(min), RangeFilterDataController.GetRangeValue(max));
			}
		}
		void AddSeries() {
			string argumentDataMember = RangeFilterViewModel.Argument.SummaryArgumentMember ?? ChartArgumentMultiDimensionalDataPropertyDescriptor.EmptyArgumentMember;
			Type argumentType = GetArgumentType(DataController.ArgumentType);
			IEnumerable<AxisPoint> axisPoints = GetSeriesAxisPoints(RangeFilterViewModel.SummarySeriesMember);
			foreach(ChartSeriesTemplateViewModel seriesTemplate in RangeFilterViewModel.SeriesTemplates) {
				foreach(AxisPoint axisPoint in axisPoints) {
					Series series = new Series();
					series.Tag = axisPoint;
					series.ArgumentDataMember = argumentDataMember;
					series.ColorDataMember = ChartColorMultiDimensionalDataPropertyDescriptor.ColorMember;
					series.ValueDataMembers[0] = seriesTemplate.DataMembers[0];
					ChangeView(series, seriesTemplate.SeriesType);
					AddSeries(series, seriesTemplate, null);
					series.DataSource = RangeFilterDataController.GetDataSource(series, seriesTemplate);
				}
			}
		}
		void ChangeView(Series series, ChartSeriesViewModelType seriesType) {
			XYDiagram2DSeriesViewBase view;
			switch(seriesType) {
				case ChartSeriesViewModelType.StackedLine:
					view = new StackedLineSeriesView();
					break;
				case ChartSeriesViewModelType.FullStackedLine:
					view = new FullStackedLineSeriesView();
					break;
				case ChartSeriesViewModelType.Area:
					view = new AreaSeriesView();
					view.RangeControlOptions.ViewType = RangeControlViewType.Area;
					break;
				case ChartSeriesViewModelType.StackedArea:
					view = new StackedAreaSeriesView();
					view.RangeControlOptions.ViewType = RangeControlViewType.Area;
					break;
				case ChartSeriesViewModelType.FullStackedArea:
					view = new FullStackedAreaSeriesView();
					view.RangeControlOptions.ViewType = RangeControlViewType.Area;
					break;
				default:
					view = new LineSeriesView();
					break;
			}
			series.View = view;
		}
		void CustomizeDiagram() {
			if(ChartControl.Series.Count == 0)
				ChartControl.Diagram = null;
			else {
				XYDiagram diagram = ChartControl.Diagram as XYDiagram;
				if(diagram == null) {
					diagram = new XYDiagram();
					ChartControl.Diagram = diagram;
				}
				if(RangeFilterViewModel.Argument.IsContinuousDateTimeScale) {
					diagram.RangeControlDateTimeGridOptions.SnapMode = ChartRangeControlClientSnapMode.Manual;
					diagram.RangeControlDateTimeGridOptions.SnapAlignment = DateTimeGridAlignment.Second;
				}
				else {
					diagram.RangeControlDateTimeGridOptions.SnapMode = ChartRangeControlClientSnapMode.ChartMeasureUnit;
				}
				diagram.RangeControlNumericGridOptions.SnapMode = ChartRangeControlClientSnapMode.Manual;
				AxisX axisX = diagram.AxisX;
				axisX.VisualRange.Auto = true;
				Range range = axisX.VisualRange;
				Range wholeRange = axisX.WholeRange;
				range.AutoSideMargins = false;
				range.SideMarginsValue = 0;
				wholeRange.AutoSideMargins = false;
				wholeRange.SideMarginsValue = 0;
			}
		}
		void UpdateLabelsConfigurator() {
			labelsConfigurator.Update(RangeFilterViewModel);
		}
		protected override void UpdateViewModelInternal() {
		}
		protected override void UpdateDataInternal() {
			ClearSeries();
			if(RangeFilterViewModel.Argument.HasArguments && RangeFilterViewModel.SeriesTemplates.Count > 0)
				AddSeries();
			CustomizeDiagram();
			UpdateLabelsConfigurator();
		}
	}
}
