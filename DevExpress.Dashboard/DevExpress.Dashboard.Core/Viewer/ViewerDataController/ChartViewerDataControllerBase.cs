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
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Native.Performance;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class ChartViewerDataControllerBase : IChartPointColorGetter {
		protected const string PerformanceStatisticsPath = "DevExpress.DashboardCommon.Viewer.Chart.Performance";
		static ApproximationController approximationControllerInstace = null;
		static ApproximationController ApproximationControllerInstance {
			get {
				if(approximationControllerInstace == null) {
					approximationControllerInstace = new ApproximationController();
					Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PerformanceStatisticsPath + ".ChartPerformanceStatistics.csv");
					StreamReader reader = new StreamReader(stream);
					approximationControllerInstace.RestoreFromCsv(reader.ReadToEnd());
				}
				return approximationControllerInstace;
			}
		}
		public static ChartViewerDataControllerBase CreateInstance(DashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode) {
			ScatterChartDashboardItemViewModel scatterViewModel = viewModel as ScatterChartDashboardItemViewModel;
			if(scatterViewModel != null)
				return new ScatterChartViewerDataController(scatterViewModel, data, configuratorCache, drillDownState, isDesignMode);
			ChartDashboardItemViewModel chartViewModel = viewModel as ChartDashboardItemViewModel;
			if(chartViewModel != null)
				return new ChartViewerDataController(chartViewModel, data, configuratorCache, drillDownState, isDesignMode);
			PieDashboardItemViewModel pieViewModel = viewModel as PieDashboardItemViewModel;
			if(pieViewModel != null) {
				if(pieViewModel.ProvideValuesAsArguments)
					return new PieViewerOnlySeriesDataController(pieViewModel, data, configuratorCache, drillDownState, isDesignMode);
				if(pieViewModel.SummarySeriesMember == null)
					return new PieViewerOnlyArgumentsDataController(pieViewModel, data, configuratorCache, drillDownState, isDesignMode);
				ContentDescriptionViewModel contentDescription = pieViewModel.ContentDescription;
				if(contentDescription != null && contentDescription.ElementSelectionEnabled)
					return new PieViewerElementSelectionEnabledDataController(pieViewModel, data, configuratorCache, drillDownState, isDesignMode);
				return new PieViewerArgumentsAndSeriesDataController(pieViewModel, data, configuratorCache, drillDownState, isDesignMode);
			}
			RangeFilterDashboardItemViewModel rangeFilterViewModel = viewModel as RangeFilterDashboardItemViewModel;
			if(rangeFilterViewModel != null)
				return new RangeFilterViewerDataController(rangeFilterViewModel, data, configuratorCache, drillDownState, isDesignMode);
			return null;
		}
		readonly MultiDimensionalData data;
		readonly IDictionary<string, IList> drillDownState;
		readonly DashboardItemViewModel viewModel;
		readonly ChartSeriesConfiguratorCache configuratorCache;
		int argumentLimit;
		int seriesLimit;
		bool isDataReduced = false;
		protected abstract ChartArgumentViewModel ArgumentViewModel { get; }
		public ChartArgumentType ArgumentType { get { return ArgumentViewModel.Type; } }
		public bool IsDataReduced { get { return isDataReduced; } }
		public bool IsEmpty { get { return Data.IsEmpty; } }
		protected abstract int SeriesTemplateCount { get; }
		protected MultiDimensionalData Data { get { return data; } }
		protected DashboardItemViewModel ViewModel { get { return viewModel; } }
		protected virtual ApproximationController ApproximationController { get { return ApproximationControllerInstance; } }
		protected int ArgumentValuesLimit { get { return isDataReduced ? argumentLimit : -1; } }
		int SeriesValuesLimit { get { return isDataReduced ? seriesLimit : -1; } }
		ChartDashboardItemBaseViewModel ChartBaseViewModel { get { return ViewModel as ChartDashboardItemBaseViewModel; } }
		protected ChartViewerDataControllerBase(DashboardItemViewModel viewModel, MultiDimensionalData data, ChartSeriesConfiguratorCache configuratorCache, IDictionary<string, IList> drillDownState, bool isDesignMode) {
			this.data = data;
			this.drillDownState = drillDownState;
			this.viewModel = viewModel;
			this.configuratorCache = configuratorCache;
			InitializeAproximationParameters(isDesignMode);
		}
		public virtual IList GetArgumentUniqueValues(SeriesPoint seriesPoint) {
			AxisPoint axisPoint = (AxisPoint)seriesPoint.Tag;
			return GetRootPath(axisPoint).Select(p => p.UniqueValue).ToList();
		}
		public IList GetSeriesUniqueValue(Series series) {
			AxisPoint point = (AxisPoint)series.Tag;
			return GetRootPathUniqueValues(point);
		}
		public ChartMultiDimensionalDataSourceBase GetDataSource(Series series, ChartSeriesTemplateViewModel seriesViewModel) {
			return GetDataSourceInternal(GetSeriesAxisPoint(series), seriesViewModel, ArgumentValuesLimit, new ChartSeriesPointColorHelper(series, this));
		}
		public virtual IList<string> GetValueDataMembers(Series series, SeriesPoint seriesPoint) {
			ChartSeriesTemplateViewModel viewModel = GetSeriesViewModel(series);
			return viewModel != null && viewModel.DataMembers != null ? viewModel.DataMembers.Distinct().ToList() : null;
		}
		public IEnumerable<AxisPoint> GetSeriesAxisPoints(string id) {
			AxisPoint root = data.GetAxisRoot(DashboardDataAxisNames.ChartSeriesAxis);
			List<AxisPoint> seriesAxisPoints = root.GetAxisPointsByDimension(data.GetDimensionDescriptorByID(DashboardDataAxisNames.ChartSeriesAxis, id), true);
			return CheckSeriesThreshold(seriesAxisPoints);
		}
		public IList GetRootPathUniqueValues(AxisPoint axisPoint) {
			return GetRootPath(axisPoint).Select(p => p.UniqueValue).ToList();
		}
		public IEnumerable<string> GetRootPathDisplayTexts(AxisPoint axisPoint) {
			return GetRootPath(axisPoint).Select(p => p.DisplayText);
		}
		public Color GetColor(Series series, SeriesPoint seriesPoint) {
			return GetColor(series, ChartBaseViewModel != null ? GetArgumentAxisPoint(seriesPoint) : null, seriesPoint.Tag);
		}
		public object[] GetSeriesPointValues(Series series, SeriesPoint seriesPoint) {
			return GetSeriesPoints(series, seriesPoint).Select(point => point.Value).ToArray();
		}
		public virtual string[] GetSeriesPointDisplayTexts(Series series, SeriesPoint seriesPoint) {
			return GetSeriesPoints(series, seriesPoint).Select(point => point.DisplayText).ToArray();
		}
		public virtual string GetSeriesPointArgumentText(object component, string separator) {
			AxisPoint point = (AxisPoint)component;
			return String.Join(separator, GetRootPathDisplayTexts(point).Reverse());
		}
		public string GetArgumentDisplayText(object value) {
			string strValue = value as string;
			int index = -1;
			if(!Int32.TryParse(strValue, out index))
				return strValue;
			AxisPoint rootPoint = Data.GetAxisRoot(DashboardDataAxisNames.ChartArgumentAxis);
			AxisPoint point = rootPoint.FindPointByIndex(index);
			return GetSeriesPointArgumentText(point, Environment.NewLine);
		}
		public virtual AxisPoint GetArgumentAxisPoint(SeriesPoint seriesPoint) {
			return (AxisPoint)seriesPoint.Tag;
		}
		public AxisPoint GetSeriesAxisPoint(Series series) {
			return (AxisPoint)series.Tag;
		}
		public ElementCustomColorEventArgs PrepareElementCustomColorEventArgs(Series series, SeriesPoint seriesPoint) {
			AxisPoint seriesAxisPoint = GetSeriesAxisPoint(series);
			AxisPoint argumentAxisPoint = GetArgumentAxisPoint(seriesPoint);
			AxisPointTuple tuple = data.CreateTuple(seriesAxisPoint, argumentAxisPoint);
			List<MeasureDescriptor> measures = GetMeasureDescriptors(series, seriesPoint);
			Color color = GetColor(series, seriesPoint);
			return new ElementCustomColorEventArgs(tuple, measures, color);
		}
		public DataItemNumericUnit GetArgumentDataItemNumericUnit() {
			DataAxis axis = data.Axes[DashboardDataAxisNames.ChartArgumentAxis];
			AxisPoint root = axis.RootPoint;
			if(root.ChildItems.Count == 0)
				return DataItemNumericUnit.Ones;
			return DevExpress.DashboardCommon.Native.NumericFormatter.CalculateUnit(root.ChildItems.First().Value, root.ChildItems.Last().Value);
		}
		protected abstract ChartMultiDimensionalDataSourceBase GetDataSourceInternal(AxisPoint axisPoint, ChartSeriesTemplateViewModel seriesViewModel, int argumentThreshold, ChartSeriesPointColorHelper colorHelper);
		protected virtual object GetColorValue(Series series, AxisPoint argumentAxisPoint, object seriesPointTag) {
			if(ChartBaseViewModel != null) {
				AxisPoint seriesAxisPoint = GetSeriesAxisPoint(series);
				string colorArgument = ChartBaseViewModel.ArgumentColorDimension;
				string colorSeries = ChartBaseViewModel.SeriesColorDimension;
				AxisPoint colorArgumentAxisPoint = argumentAxisPoint.GetParentByDimensionID(colorArgument);
				AxisPoint colorSeriesAxisPoint = seriesAxisPoint.GetParentByDimensionID(colorSeries);
				string colorMeasureId = GetColorMeasureId(series, seriesPointTag);
				if(colorMeasureId != null)
					return Data.GetColorValue(colorArgumentAxisPoint, colorSeriesAxisPoint, Data.GetMeasureDescriptorByID(colorMeasureId));
			}
			return null;
		}
		protected Color GetColor(Series series, AxisPoint argumentAxisPoint, object seriesPointTag) {
			object colorValue = GetColorValue(series, argumentAxisPoint, seriesPointTag);
			return colorValue != null ? Color.FromArgb(Convert.ToInt32(colorValue)) : Color.Empty;
		}
		protected virtual string GetColorMeasureId(Series series, object seriesPointTag) {
			return GetSeriesViewModel(series).ColorMeasureID;
		}
		protected Color GetColor(Series series, AxisPoint argumentAxisPoint) {
			return GetColor(series, argumentAxisPoint, null);
		}
		protected List<MeasureDescriptor> GetMeasureDescriptors(Series series, SeriesPoint seriesPoint) {
			List<MeasureDescriptor> measureDescriptors = new List<MeasureDescriptor>();
			IList<string> dataMembers = GetValueDataMembers(series, seriesPoint);
			if(dataMembers != null) {
				foreach(string dataMember in dataMembers)
					measureDescriptors.Add(Data.GetMeasureDescriptorByID(dataMember));
			}
			return measureDescriptors;
		}
		protected IEnumerable<AxisPoint> GetRootPath(AxisPoint axisPoint) {
			if(drillDownState == null || !drillDownState.Keys.Contains(axisPoint.AxisName))
				return axisPoint.RootPath;
			return new[] { axisPoint };
		}
		protected ChartSeriesTemplateViewModel GetSeriesViewModel(Series series) {
			return configuratorCache.GetViewModel(series);
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
		MeasureValue[] GetSeriesPoints(Series series, SeriesPoint seriesPoint) {
			MultiDimensionalData currentData = Data.GetSlice((AxisPoint)series.Tag).GetSlice((AxisPoint)seriesPoint.Tag);
			return GetSeriesPoints(GetSeriesViewModel(series).DataMembers, currentData);
		}
		MeasureValue[] GetSeriesPoints(string[] dataMembers, MultiDimensionalData slice) {
			MeasureValue[] result = new MeasureValue[dataMembers.Length];
			for(int i = 0; i < dataMembers.Length; i++) {
				MeasureDescriptor measureDescriptor = Data.GetMeasureDescriptorByID(dataMembers[i]);
				result[i] = slice.GetValue(measureDescriptor);
			}
			return result;
		}
		void InitializeAproximationParameters(bool isDesignMode) {
			isDataReduced = false;
			if(isDesignMode && !data.IsEmpty) {
				int columnPointCount = data.GetAxisPoints(DashboardDataAxisNames.ChartArgumentAxis).Count;
				int rowPointCount = data.GetAxisPoints(DashboardDataAxisNames.ChartSeriesAxis).Count * SeriesTemplateCount;
				argumentLimit = Math.Min(data.GetAxisPoints(DashboardDataAxisNames.ChartArgumentAxis).Count, ApproximationController.MaxParameter);
				if(columnPointCount > argumentLimit)
					isDataReduced = true;
				seriesLimit = Math.Min(rowPointCount, ApproximationController.GetApproximation(argumentLimit));
				if(rowPointCount > seriesLimit) {
					if(seriesLimit >= SeriesTemplateCount)
						seriesLimit /= SeriesTemplateCount;
					isDataReduced = true;
				}
			}
		}
		protected IEnumerable<AxisPoint> CheckSeriesThreshold(IEnumerable<AxisPoint> axisPoints) {
			return SeriesValuesLimit > 0 ? axisPoints.Take(SeriesValuesLimit) : axisPoints;
		}
		protected IEnumerable<AxisPoint> CheckArgumentsThreshold(IEnumerable<AxisPoint> axisPoints) {
			return ArgumentValuesLimit > 0 ? axisPoints.Take(ArgumentValuesLimit) : axisPoints;
		}
		object IChartPointColorGetter.GetColorValue(Series series, AxisPoint argumentAxisPoint, object seriesPointTag) {
			return GetColorValue(series, argumentAxisPoint, seriesPointTag);
		}
	}
}
