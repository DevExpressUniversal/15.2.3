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
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
namespace DevExpress.XtraCharts.Native {
	class ExpandingRepository {
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		public bool TryAdd(string name) {
			if (string.IsNullOrEmpty(name))
				return true;
			if (dictionary.ContainsKey(name))
				return false;
			else {
				dictionary.Add(name, null);
				return true;
			}
		}
	}
	public static class BindingHelper {
		public static readonly Type[] ListTypes = new Type[] { typeof(IList), typeof(IListSource), typeof(IBindingList) };
		public static bool HasBoundSeries(Chart chart) {
			return chart.DataContainer.ActualDataSource != null && !String.IsNullOrEmpty(chart.DataContainer.SeriesDataMember);
		}
		static Type[] ConvertToTypeArray(ScaleType scaleType) {
			switch (scaleType) {
				case ScaleType.Numerical:
					return BindingHelperCore.NumericTypes;
				case ScaleType.DateTime:
					return new Type[] { typeof(DateTime) };
				default:
					return null;
			}
		}
		public static string ExtractDataMember(string dataMember, string chartDataMember) {
			return (String.IsNullOrEmpty(chartDataMember) || !dataMember.StartsWith(chartDataMember + '.')) ?
				dataMember : dataMember.Substring(chartDataMember.Length + 1);
		}
		public static bool CheckDataType(ScaleType scaleType, Type type) {
			return scaleType == ScaleType.Auto || BindingHelperCore.IsAssignableFrom(type, ConvertToTypeArray(scaleType)) ||
				BindingHelperCore.IsAssignableFrom(type, ListTypes);
		}
		public static bool AreAvailableFieldsPresent(object dataSource, ScaleType scaleType) {
			return dataSource != null && AvailableFieldsFinder.Find(dataSource, ConvertToTypeArray(scaleType));
		}
		public static bool CheckDataMember(DataContext dataContext, object dataSource, string dataMember, Type[] types) {
			if (!BindingHelperCore.IsValidDataParams(dataSource, dataMember))
				return true;
			DataContextBase actualDataContext = dataContext;
			bool shouldDisposeDataContext = false;
			if (actualDataContext == null) {
				actualDataContext = new DataContextBase();
				shouldDisposeDataContext = true;
			}
			try {
				DataBrowser dataBrowser = actualDataContext.GetDataBrowser(dataSource, dataMember, true);
				return dataBrowser != null && BindingHelperCore.IsAssignableFrom(dataBrowser.DataSourceType, types);
			}
			finally {
				if (shouldDisposeDataContext)
					actualDataContext.Dispose();
			}
		}
		public static bool CheckDataMember(DataContext dataContext, object dataSource, string dataMember) {
			return BindingHelper.CheckDataMember(dataContext, dataSource, dataMember, null);
		}
		public static bool CheckDataMember(DataContext dataContext, object dataSource, string dataMember, ScaleType scaleType) {
			return CheckDataMember(dataContext, dataSource, dataMember, ConvertToTypeArray(scaleType));
		}
		public static string GetDataMemberName(DataContext dataContext, object dataSource, string chartDataMember, string dataMember) {
			DataContext actualDataContext = dataContext;
			bool shouldDisposeDataContext = false;
			if (actualDataContext == null) {
				actualDataContext = new DataContext();
				shouldDisposeDataContext = true;
			}
			try {
				if (String.IsNullOrEmpty(dataMember))
					return String.Empty;
				string chartDataMemberDisplayName = String.IsNullOrEmpty(chartDataMember) ?
					String.Empty : actualDataContext.GetDataMemberDisplayName(dataSource, chartDataMember);
				return actualDataContext.GetDataMemberDisplayName(dataSource, chartDataMemberDisplayName,
					BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember));
			}
			finally {
				if (shouldDisposeDataContext)
					actualDataContext.Dispose();
			}
		}
	}
	public class AvailableFieldsFinder : IDisposable {
		readonly object dataSource;
		readonly Type[] types;
		readonly DataContextBase dataContext;
		public static bool Find(object dataSource, Type[] types) {
			using (AvailableFieldsFinder finder = new AvailableFieldsFinder(dataSource, types))
				return finder.Find(String.Empty, null, new PropertyDescriptor[0]);
		}
		AvailableFieldsFinder(object dataSource, Type[] types) {
			this.dataSource = dataSource;
			this.types = types;
			dataContext = new DataContextBase();
		}
		bool Find(string name, PropertyDescriptor desc, IList<PropertyDescriptor> parentDescs) {
			if (desc != null) {
				Type type = desc.PropertyType;
				if (type.IsPrimitive || type.Equals(typeof(String)) || type.Equals(typeof(DateTime)) || type.Equals(typeof(Decimal)) || type.Equals(typeof(TimeSpan)))
					return BindingHelperCore.IsAssignableFrom(desc.PropertyType, types);
			}
			DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, name, true);
			if (dataBrowser == null)
				return false;
			if (!String.IsNullOrEmpty(name))
				name += ".";
			PropertyDescriptorCollection coll = dataBrowser.GetItemProperties();
			foreach (PropertyDescriptor dsc in coll)
				if (!parentDescs.Contains(dsc)) {
					PropertyDescriptor[] nextParentDescs = new PropertyDescriptor[parentDescs.Count + 1];
					parentDescs.CopyTo(nextParentDescs, 0);
					nextParentDescs[parentDescs.Count] = dsc;
					if (Find(name + dsc.Name, dsc, nextParentDescs))
						return true;
				}
			return false;
		}
		public void Dispose() {
			dataContext.Dispose();
		}
	}
	public class BoundDataItem {
		readonly object argument;
		readonly string toolTipHint;
		readonly List<DataSourceValues> values = new List<DataSourceValues>();
		readonly List<object> colorKeys = new List<object>();
		public object Argument { get { return argument; } }
		public List<DataSourceValues> Values { get { return values; } }
		public List<object> ColorKeys { get { return colorKeys; } }
		public string ToolTipHint { get { return toolTipHint; } }
		public BoundDataItem(object argument, string toolTipHint) {
			this.argument = argument;
			this.toolTipHint = toolTipHint;
		}
	}
	public class BoundData {
		readonly Dictionary<object, BoundDataItem> boundData = new Dictionary<object, BoundDataItem>();
		readonly List<BoundDataItem> items = new List<BoundDataItem>();
		public BoundDataItem this[object argument] { get { return boundData[argument]; } }
		public List<BoundDataItem> Items { get { return items; } }
		public bool ContainsArgument(object argument) { return boundData.ContainsKey(argument); }
		public void Add(object argument, string toolTipHint) {
			BoundDataItem item = new BoundDataItem(argument, toolTipHint);
			boundData.Add(argument, item);
			items.Add(item);
		}
	}
	public class SummarySeriesBindingProcedure : SeriesBindingProcedure {
		readonly Series series;
		readonly SummaryFunction function;
		readonly string[] summaryDataMembers;
		readonly BoundData boundData = new BoundData();
		readonly Dictionary<ISeries, BoundData> seriesBoundData = new Dictionary<ISeries, BoundData>();
		public SummarySeriesBindingProcedure(Series series, DataContextBase dataContext, object dataSource, string dataMember, SummaryFunction function)
			: base(series, dataContext, dataSource, dataMember) {
			this.function = function;
			if (series != null) {
				this.series = series;
				summaryDataMembers = new SummaryFunctionParser(series.SummaryFunction).Arguments;
			}
		}
		string GetFirstToolTipHint(object argument, string toolTipHintDataMember) {
			object currentToolTipHint = GetToolTipHint(toolTipHintDataMember);
			return currentToolTipHint != null ? currentToolTipHint.ToString() : string.Empty;
		}
		protected override void Complete() {
			base.Complete();
			foreach (BoundDataItem item in boundData.Items) {
				ISeriesPoint[] points = function(series, item.Argument, summaryDataMembers, item.Values.ToArray(), item.ColorKeys.ToArray());
				if (points != null) {
					AddPoints(points);
					if (!String.IsNullOrEmpty(item.ToolTipHint))
						foreach (SeriesPoint point in points)
							point.ToolTipHint = item.ToolTipHint;
				}
			}
		}
		protected override void PerformPointBinding(object argument, object dataRow) {
			DataSourceValues values = new DataSourceValues();
			for (int i = 0; i < ValueDataMembers.Count; i++) {
				DataBrowser valueBrowser = GetDataBrowser(ValueDataMembers[i]);
				if (valueBrowser != null)
					values[summaryDataMembers[i]] = valueBrowser.Current;
			}
			values[String.Empty] = dataRow;
			if (series.ActualArgumentScaleType == ScaleType.DateTime) {
				if (!(argument is DateTime))
					return;
				IXYSeriesView view = series.View as IXYSeriesView;
				if (view != null && view.AxisXData != null)
					argument = DateTimeUtils.Floor((DateTime)argument, view.AxisXData.DateTimeScaleOptions.MeasureUnit);
			}
			if (!boundData.ContainsArgument(argument))
				boundData.Add(argument, GetFirstToolTipHint(argument, ToolTipHintDataMember));
			object colorKey = null;
			if (!string.IsNullOrEmpty(ColorDataMember)) {
				colorKey = GetColor(ColorDataMember);
				if (colorKey != null)
					boundData[argument].ColorKeys.Add(colorKey);
			}
			boundData[argument].Values.Add(values);
		}
	}
	public class SummaryChartBindingProcedure : ChartBindingProcedure {
		readonly SummaryFunction function;
		readonly Dictionary<ISeries, BoundData> seriesBoundData = new Dictionary<ISeries, BoundData>();
		public SummaryChartBindingProcedure(Series series, DataContextBase dataContext, object dataSource, string dataMember, SummaryFunction function)
			: base(dataContext, dataSource, dataMember) {
			this.function = function;
		}
		string GetFirstToolTipHint(object argument, string toolTipHintDataMember) {
			object currentToolTipHint = GetToolTipHint(toolTipHintDataMember);
			return currentToolTipHint != null ? currentToolTipHint.ToString() : string.Empty;
		}
		protected override void Complete(ISeries series) {
			base.Complete(series);
			string[] summaryDataMembers = new string[0];
			if (seriesBoundData.ContainsKey(series)) {
				BoundData boundData = seriesBoundData[series];
				Series currentSeries = series as Series;
				if (currentSeries != null)
					summaryDataMembers = new SummaryFunctionParser(currentSeries.SummaryFunction).Arguments;
				foreach (BoundDataItem item in boundData.Items) {
					ISeriesPoint[] points = function(currentSeries, item.Argument, summaryDataMembers, item.Values.ToArray(), null);
					if (points != null) {
						foreach (SeriesPoint point in points)
							if (!String.IsNullOrEmpty(item.ToolTipHint))
								point.ToolTipHint = item.ToolTipHint;
						foreach (SeriesPoint point in points)
							currentSeries.Points.Add(point);
					}
				}
			}
		}
		protected override void PerformPointBinding(ISeriesTemplate seriesTemplate, object argument, object dataRow, IList<string> valueDataMembers, string toolTipHintDataMember, ISeries series) {
			DataSourceValues values = new DataSourceValues();
			Series currentSeries = series as Series;
			if (currentSeries != null) {
				string[] summaryDataMembers = new SummaryFunctionParser(currentSeries.SummaryFunction).Arguments;
				for (int i = 0; i < valueDataMembers.Count; i++) {
					DataBrowser valueBrowser = GetDataBrowser(valueDataMembers[i]);
					if (valueBrowser != null)
						values[summaryDataMembers[i]] = valueBrowser.Current;
				}
				values[String.Empty] = dataRow;
				if (series.ArgumentScaleType == Scale.DateTime) {
					if (!(argument is DateTime))
						return;
					IXYSeriesView xyView = series.SeriesView as IXYSeriesView;
					if ((xyView != null) && (xyView.AxisXData != null))
						argument = DateTimeUtils.Floor((DateTime)argument, xyView.AxisXData.DateTimeScaleOptions.MeasureUnit);
				}
				if (!seriesBoundData.ContainsKey(series))
					seriesBoundData.Add(series, new BoundData());
				if (!seriesBoundData[series].ContainsArgument(argument))
					seriesBoundData[series].Add(argument, GetFirstToolTipHint(argument, toolTipHintDataMember));
				seriesBoundData[series][argument].Values.Add(values);
			}
		}
	}
}
