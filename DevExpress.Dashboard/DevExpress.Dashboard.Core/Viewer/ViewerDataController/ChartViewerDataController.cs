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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
using System.Runtime.Serialization;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class AxesChartViewerDataController : ChartViewerDataControllerBase {
		internal const int TransparentAlpha = 135;
		static ChartSeriesViewModelType[] TransparentColorsSeriesTypes {
			get {
				return new ChartSeriesViewModelType[] { 
					ChartSeriesViewModelType.Area, 
					ChartSeriesViewModelType.StepArea, 
					ChartSeriesViewModelType.SplineArea, 
					ChartSeriesViewModelType.RangeArea, 
					ChartSeriesViewModelType.Weighted 
				};
			}
		}
		static ChartSeriesViewModelType[] FinancialTypes {
			get { return new ChartSeriesViewModelType[] { ChartSeriesViewModelType.Stock, ChartSeriesViewModelType.CandleStick, ChartSeriesViewModelType.HighLowClose }; }
		}
		static Tuple<string, Color>[] ConvertToTuples(Dictionary<IList<object>, Color> colors, Dictionary<IList<object>, string> texts) {
			int i = 0;
			Tuple<string, Color>[] legendInfo = new Tuple<string, Color>[colors.Keys.Count];
			foreach(var key in colors.Keys)
				legendInfo[i++] = new Tuple<string, Color>(texts[key], colors[key]);
			return legendInfo;
		}
		protected override ChartArgumentViewModel ArgumentViewModel {
			get { return ChartViewModel.Argument; }
		}
		protected ChartDashboardItemViewModel ChartViewModel { get { return (ChartDashboardItemViewModel)ViewModel; } }
		protected override int SeriesTemplateCount { get { return ChartViewModel.Panes.SelectMany(p => p.SeriesTemplates).Count(); } }
		protected AxesChartViewerDataController(ChartDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode)
			: base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		public abstract ChartMultiDimensionalDataSourceBase GetDataSource(AxisPoint axisPoint);
		protected override object GetColorValue(Series series, AxisPoint argumentAxisPoint, object seriesPointTag) {
			ChartSeriesTemplateViewModel viewModel = GetSeriesViewModel(series);
			if(viewModel == null || FinancialTypes.Contains(viewModel.SeriesType))
				return null;
			return base.GetColorValue(series, argumentAxisPoint, seriesPointTag);
		}
		public abstract ChartMultiDimensionalDataSourceBase GetDataSource();
		public ElementCustomColorEventArgs PrepareElementCustomColorEventArgs(Series series) {
			AxisPoint seriesAxisPoint = GetSeriesAxisPoint(series);
			AxisPointTuple tuple = Data.CreateTuple(seriesAxisPoint);
			List<MeasureDescriptor> measures = GetMeasureDescriptors(series, null);
			Color color = GetColor(series);
			return new ElementCustomColorEventArgs(tuple, measures, color);
		}
		public Color GetColor(Series series) {
			if(series.DataSource != null) {
				ChartMultiDimensionalDataSourceBase multuDataSource = (ChartMultiDimensionalDataSourceBase)series.DataSource;
				Color color = multuDataSource.GetFirstArgumentColor();
				if(color != Color.Empty)
					return PrepareSeriesColor(series, color);
			}
			return Color.Empty;
		}
		public Color PrepareSeriesColor(Series series, Color color) {
			return PrepareSeriesColor(GetSeriesViewModel(series), color);
		}
		public Tuple<string, Color>[] GetLegendInfo() {
			MultiDimensionalData data;
			IEnumerable<AxisPoint> argumentPoints = GetArgumentAxisPoints();
			IEnumerable<AxisPoint> seriesPoints = GetSeriesAxisPoints();
			Dictionary<IList<object>, string> texts = new Dictionary<IList<object>, string>(new EnumerableEqualityComparer<object>());
			Dictionary<IList<object>, Color> colors = new Dictionary<IList<object>, Color>(new EnumerableEqualityComparer<object>());
			foreach(AxisPoint argumentPoint in argumentPoints) {
				data = Data.GetSlice(argumentPoint);
				foreach(AxisPoint seriesPoint in seriesPoints) {
					data = data.GetSlice(seriesPoint);
					IEnumerable<ChartSeriesTemplateViewModel> seriesTemplates = ChartViewModel.Panes.SelectMany(pane => pane.SeriesTemplates);
					foreach(ChartSeriesTemplateViewModel seriesTemplate in seriesTemplates.GroupBy(temp => temp.ColorMeasureID).Select(group => group.First())) {
						string measureId = seriesTemplate.ColorMeasureID;
						MeasureDescriptor measureDescriptor = Data.GetMeasureDescriptorByID(measureId);
						object colorValue = data.GetValue(measureDescriptor).Value;
						if(colorValue != null) {
							Color color = Color.FromArgb(Convert.ToInt32(colorValue));
							if(ChartViewModel.ArgumentColorDimension == null)
								color = PrepareSeriesColor(seriesTemplates.Last(temp => temp.ColorMeasureID == measureId), color);
							IEnumerable<AxisPoint> axisPoints = GetAxisPoints(argumentPoint, seriesPoint);
							IList<object> values = axisPoints.Select(point => point.Value).ToList();
							IList<string> displayTexts = axisPoints.Select(point => point.DisplayText).ToList();
							if(!String.IsNullOrEmpty(measureDescriptor.Name)) {
								string seriesName = seriesTemplate.Name;
								values.Add(seriesName);
								displayTexts.Add(seriesName);
							}
							if(!colors.ContainsKey(values)) {
								colors.Add(values, color);
								string text = String.Join(" - ", displayTexts);
								if(displayTexts.Count == 0)
									text = string.Join(", ", ChartViewModel.Panes.SelectMany(pane => pane.SeriesTemplates).Select(temp => temp.Name).Distinct());
								if(string.IsNullOrEmpty(text))
									text = " ";
								texts.Add(values, text);
							}
						}
					}
				}
			}
			return ConvertToTuples(colors, texts);
		}
		IEnumerable<AxisPoint> GetArgumentAxisPoints() {
			string argumentID = ChartViewModel.ArgumentColorDimension;
			DimensionDescriptor argumentDescriptor = Data.GetDimensionDescriptorByID(DashboardDataAxisNames.ChartArgumentAxis, argumentID);
			IEnumerable<AxisPoint> argumentsAxisPoints = argumentDescriptor != null ?
				Data.GetAxisPointsByDimension(argumentDescriptor, true).ToList() :
				new List<AxisPoint> { Data.GetAxisRoot(DashboardDataAxisNames.ChartArgumentAxis) };
			return CheckArgumentsThreshold(argumentsAxisPoints);
		}
		IEnumerable<AxisPoint> GetSeriesAxisPoints() {
			string seriesID = ChartViewModel.SeriesColorDimension;
			DimensionDescriptor seriesDescriptor = Data.GetDimensionDescriptorByID(DashboardDataAxisNames.ChartSeriesAxis, seriesID);
			IEnumerable<AxisPoint> seriesAxisPoints = seriesDescriptor != null ?
				Data.GetAxisPointsByDimension(seriesDescriptor, true).ToList() :
				new List<AxisPoint> { Data.GetAxisRoot(DashboardDataAxisNames.ChartSeriesAxis) };
			return CheckSeriesThreshold(seriesAxisPoints);
		}
		IEnumerable<AxisPoint> GetAxisPoints(AxisPoint argumentPoint, AxisPoint seriesPoint) {
			IEnumerable<AxisPoint> argumentAxisPoints = GetRootPath(argumentPoint)
				.Where(point => point.Dimension != null && ChartViewModel.ColorPathMembers.Contains(point.Dimension.ID));
			IEnumerable<AxisPoint> seriesAxisPoints = GetRootPath(seriesPoint)
				.Where(point => point.Dimension != null && ChartViewModel.ColorPathMembers.Contains(point.Dimension.ID));
			IList<AxisPoint> axisPoints = new List<AxisPoint>(argumentAxisPoints);
			foreach(AxisPoint axisPoint in seriesAxisPoints)
				if(argumentAxisPoints.All(point => !point.DimensionValueEquals(axisPoint)))
					axisPoints.Add(axisPoint);
			return axisPoints;
		}
		Color PrepareSeriesColor(ChartSeriesTemplateViewModel seriesViewModel, Color color) {
			if(seriesViewModel != null && TransparentColorsSeriesTypes.Contains(seriesViewModel.SeriesType))
				color = Color.FromArgb(TransparentAlpha, color.R, color.G, color.B);
			return color;
		}
	}
	public class ChartViewerDataController : AxesChartViewerDataController {
		public ChartViewerDataController(ChartDashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode)
			: base(viewModel, data, configuratorCache, drillDownState, isDesignMode) {
		}
		protected override ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper) {
			return new ChartMultiDimensionalDataSource(Data.GetSlice(axisPoint), ChartViewModel.Argument.SummaryArgumentMember,
				GetArgumentType(ArgumentType), seriesViewModel.DataMembers, argumentThreshold, colorHelper);
		}
		public override ChartMultiDimensionalDataSourceBase GetDataSource(AxisPoint axisPoint) {
			return new ChartMultiDimensionalDataSource(Data, ChartViewModel.Argument.SummaryArgumentMember, GetArgumentType(ArgumentType), ArgumentValuesLimit);
		}
		public override ChartMultiDimensionalDataSourceBase GetDataSource() {
			return new ChartMultiDimensionalDataSource(Data, ChartViewModel.Argument.SummaryArgumentMember, GetArgumentType(ArgumentType), new string[0], -1, null);
		}
	}
}
