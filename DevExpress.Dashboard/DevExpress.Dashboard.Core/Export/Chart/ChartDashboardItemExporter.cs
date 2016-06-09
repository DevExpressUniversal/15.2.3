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

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.DashboardExport {
	public class ChartDashboardItemExporter : ChartDashboardItemExporterBase {
		readonly DashboardChartControlViewer controlViewer;
		internal static void UpdateChartSelectionByTargetAxis(DashboardChartControlViewerBase controlViewer, MultiDimensionalData data, IList selectedValues, ChartDashboardItemBaseViewModel viewModel, IDictionary<string, IList> drillDownState) {
			if(selectedValues != null && selectedValues.Count > 0) {
				if(viewModel.SelectionMode == ChartSelectionModeViewModel.Argument)
					controlViewer.InteractivityMode = ChartInteractivityMode.Argument;
				else if(viewModel.SelectionMode == ChartSelectionModeViewModel.Series)
					controlViewer.InteractivityMode = ChartInteractivityMode.Series;
				else
					controlViewer.InteractivityMode = ChartInteractivityMode.Point;
				List<AxisPointTuple> selectedAxisPoint = new List<AxisPointTuple>();
				Dictionary<string, int> axisDimensionCount = new Dictionary<string, int>();
				foreach(string name in data.GetAxisNames()) {
					int dimensionCount = drillDownState != null && drillDownState.ContainsKey(name) ? 1 : data.GetAxis(name).Dimensions.Count;
					axisDimensionCount.Add(name, dimensionCount);
				}
				foreach(List<object> selectedValue in selectedValues) {
					int valueIndex = 0;
					List<AxisPoint> axisPoints = new List<AxisPoint>();
					if(controlViewer.InteractivityMode.HasFlag(ChartInteractivityMode.Argument)) {
						IList axisValues = new List<object>();
						for(int i = 0; i < axisDimensionCount[DashboardDataAxisNames.ChartArgumentAxis]; i++) 
							axisValues.Add(selectedValue[valueIndex++]);
						axisPoints.Add(GetAxisPoint(data, axisValues, DashboardDataAxisNames.ChartArgumentAxis, drillDownState));
					}
					if(controlViewer.InteractivityMode.HasFlag(ChartInteractivityMode.Series)) {
						IList axisValues = new List<object>();
						for(int i = 0; i < axisDimensionCount[DashboardDataAxisNames.ChartSeriesAxis]; i++)
							axisValues.Add(selectedValue[valueIndex++]);
						axisPoints.Add(GetAxisPoint(data, axisValues, DashboardDataAxisNames.ChartSeriesAxis, drillDownState));
					}
					selectedAxisPoint.Add(data.CreateTuple(axisPoints));
				}
				controlViewer.SelectValues(selectedAxisPoint);
			}
		}
		static AxisPoint GetAxisPoint(MultiDimensionalData data, IList axisValues, string axisName, IDictionary<string, IList> drillDownState) {
			object[] value = DataUtils.GetFullValue(axisName, axisValues, drillDownState).ToArray();
			return data.GetAxisPointByUniqueValues(axisName, value);
		}
		protected override string[] TargetAxisNames {
			get {
				IList<string> axisNames = new List<string>();
				ChartDashboardItemBaseViewModel viewModel = ServerData.ViewModel as ChartDashboardItemBaseViewModel;
				if(viewModel != null) {
					if(viewModel.SelectionMode.HasFlag(ChartSelectionModeViewModel.Argument))
						axisNames.Add(DashboardDataAxisNames.ChartArgumentAxis);
					if(viewModel.SelectionMode.HasFlag(ChartSelectionModeViewModel.Series))
						axisNames.Add(DashboardDataAxisNames.ChartSeriesAxis);
				}
				return axisNames.ToArray();
			}
		}
		public ChartDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
			IDashboardChartControl iChart = (IDashboardChartControl)ChartControl;
			iChart.Size = new Size(ClientState.ViewerArea.Width, ClientState.ViewerArea.Height);
			controlViewer = CreateControlViewer();
			controlViewer.InitializeControl();
			MultiDimensionalData multiDimensionalData = CreateMultiDimensionalData();
			IDictionary<string, IList> drillDownState = GetDrillDownState();
			controlViewer.Update(ServerData.ViewModel, multiDimensionalData, drillDownState);
			UpdateChartSelectionByTargetAxis(controlViewer, multiDimensionalData, SelectedValues, ServerData.ViewModel as ChartDashboardItemBaseViewModel, drillDownState);
			if(FontHelper.HasValue(data.FontInfo)) {
				Chart chart = ChartControl.ChartContainer.Chart;
				XYDiagram diargam = chart.Diagram as XYDiagram;
				chart.Legend.Font = FontHelper.GetFont(chart.Legend.Font, data.FontInfo);
				if(diargam != null) {
					foreach(AxisBase axis in diargam.GetAllAxesX()) {
						axis.Label.Font = FontHelper.GetFont(axis.Label.Font, data.FontInfo);
					}
					foreach(AxisBase axis in diargam.GetAllAxesY()) {
						axis.Label.Font = FontHelper.GetFont(axis.Label.Font, data.FontInfo);
					}
					foreach(Series series in chart.Series) {
						series.Label.Font = FontHelper.GetFont(series.Label.Font, data.FontInfo);
					}
				}
			}
			if(ClientState.SpecificState != null) {
				XYDiagram diagram = (XYDiagram)iChart.Diagram;
				object minValue = null, maxValue = null;
				if(ClientState.SpecificState.TryGetValue("ChartAxisXMinValue", out minValue)) {
					if(minValue != null)
						diagram.AxisX.VisualRange.MinValue = controlViewer.GetArgumentValue((IList)minValue);
				}
				if(ClientState.SpecificState.TryGetValue("ChartAxisXMaxValue", out maxValue)) {
					if(maxValue != null)
						diagram.AxisX.VisualRange.MaxValue = controlViewer.GetArgumentValue((IList)maxValue);
				}
			}
		}
		protected virtual DashboardChartControlViewer CreateControlViewer() {
			return new DashboardChartControlViewer(ChartControl);
		}
	}
	public class ScatterChartDashboardItemExporter : ChartDashboardItemExporter {
		public ScatterChartDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
		}
		protected override DashboardChartControlViewer CreateControlViewer() {
			return new DashboardScatterChartControlViewer(ChartControl);
		}
	}
}
