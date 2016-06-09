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
using System.Globalization;
namespace DevExpress.Charts.Native {
	public enum Condition {
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotEqual
	}
	public enum Conjunction {
		And,
		Or
	}
	public enum SortMode {
		None,
		Ascending,
		Descending
	}
	public interface IDataFilter {
		string ColumnName { get; }
		Type DataType { get; }
		Condition Condition { get; }
		object Value { get; }
		bool Required { get; }
	}
}
#if !DXPORTABLE
namespace DevExpress.Charts.Native {
	using DevExpress.Data.Browsing;
	public static class BindingHelperCore {
		public static readonly Type[] NumericTypes = new Type[] { typeof(double), 
																  typeof(float),
																  typeof(int),
																  typeof(decimal),
																  typeof(uint),
																  typeof(long),
																  typeof(ulong),
																  typeof(short),
																  typeof(ushort),
																  typeof(byte),
																  typeof(sbyte),
																  typeof(char)};
		public static int CompareValues(object x, object y) {
			if (x is DBNull)
				return (y is DBNull) ? 0 : -1;
			if (y is DBNull)
				return 1;
			IComparable comparable = x as IComparable;
			try {
				return comparable.CompareTo(y);
			}
			catch {
				return 0;
			}
		}
		public static bool IsValidDataParams(object dataSource, string dataMember) {
			return dataSource != null && !String.IsNullOrEmpty(dataMember);
		}
		public static bool IsAssignableFrom(Type type, Type[] types) {
			if (type == null)
				return false;
			if (types == null)
				return true;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType == null)
				underlyingType = type.UnderlyingSystemType;
			foreach (Type numericType in types)
				if (numericType.IsAssignableFrom(underlyingType))
					return true;
			return false;
		}
		public static bool CheckNumericDataType(Type type) {
			return IsAssignableFrom(type, NumericTypes);
		}
		public static Type GetMemberType(object dataSource, string dataMember) {
			if (IsValidDataParams(dataSource, dataMember)) {
				using (DataContextBase dataContext = new DataContextBase()) {
					DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true);
					if (dataBrowser != null)
						return dataBrowser.DataSourceType;
				}
			}
			return null;
		}
		public static Scale GetScaleType(object dataSource, string dataMember) {
			Type fieldType = GetMemberType(dataSource, dataMember);
			if (fieldType != null) {
				if (IsAssignableFrom(fieldType, new Type[] { typeof(DateTime) }))
					return Scale.DateTime;
				else if (IsAssignableFrom(fieldType, NumericTypes))
					return Scale.Numerical;
				return Scale.Qualitative;
			}
			return Scale.Auto;
		}
		public static IList GetValues(DataContextBase context, object dataSource, string dataMember, SortMode sorting) {
			bool needClearContext = false;
			if (context == null) {
				context = new DataContextBase();
				needClearContext = true;
			}
			try {
				List<object> list = new List<object>();
				DataBrowser browser = BindingProcedure.CreateDataBrowser(context, dataSource, dataMember);
				if (browser != null && browser.Position >= 0) {
					int currentPosition = browser.Position;
					try {
						for (int pos = 0; pos < browser.Count; pos++) {
							browser.Position = pos;
							DataBrowser valueBrowser = context.GetDataBrowser(dataSource, dataMember, true);
							if (valueBrowser != null && valueBrowser.Current != null && list.IndexOf(valueBrowser.Current) == -1)
								list.Add(valueBrowser.Current);
						}
					}
					finally {
						browser.Position = currentPosition;
					}
				}
				if (sorting != SortMode.None) {
					foreach (object obj in list)
						if (!(obj is DBNull)) {
							IComparable comparable = obj as IComparable;
							if (comparable != null) {
								list.Sort(CompareValues);
								if (sorting == SortMode.Descending)
									list.Reverse();
							}
							break;
						}
				}
				return list;
			}
			finally {
				if (needClearContext)
					context.Clear();
			}
		}
	}
	public abstract class BindingProcedure {
		public static string ConvertToActualDataMember(string rootDataMember, string dataMember) {
			return (String.IsNullOrEmpty(rootDataMember) || String.IsNullOrEmpty(dataMember)) ?
				dataMember : (rootDataMember + '.' + dataMember);
		}
		public static DataBrowser CreateDataBrowser(DataContextBase dataContext, object dataSource, string dataMember) {
			DataBrowser browser = dataContext.GetDataBrowser(dataSource, String.Empty, true);
			if (browser == null)
				return null;
			string propertyName = dataMember;
			string parentDataMember = String.Empty;
			ListBrowser listBrowser = browser as ListBrowser;
			while (propertyName != null && browser.FindItemProperty(propertyName, false) == null) {
				try {
					int dotPos = propertyName.IndexOf('.');
					if (dotPos == -1)
						break;
					if (parentDataMember != String.Empty)
						parentDataMember += '.';
					parentDataMember += propertyName.Substring(0, dotPos);
					browser = dataContext[dataSource, parentDataMember];
					if (browser is ListBrowser)
						listBrowser = (ListBrowser)browser;
					propertyName = propertyName.Substring(dotPos + 1, propertyName.Length - dotPos - 1);
				}
				catch {
					break;
				}
			}
			return listBrowser != null ? listBrowser : browser;
		}
		public static object ConvertToCompatibleValue(IDataFilter dataFilter, object value) {
			if (dataFilter.DataType != null && value != null && value.GetType() != dataFilter.DataType)
				return TypeConversion.ConvertToCompatibleValue(value, dataFilter.DataType);
			else
				return value;
		}
		readonly DataContextBase dataContext;
		readonly object dataSource;
		readonly string dataMember;
		readonly bool shouldClearDataContext;
		Dictionary<string, DataBrowser> dataBrowsersCache = new Dictionary<string, DataBrowser>();
		protected DataContextBase DataContext { get { return dataContext; } }
		protected object DataSource { get { return dataSource; } }
		protected string DataMember { get { return dataMember; } }
		protected bool ShouldClearDataContext { get { return shouldClearDataContext; } }
		public BindingProcedure(DataContextBase dataContext, object dataSource, string dataMember) {
			if (dataContext == null) {
				this.dataContext = new DataContextBase();
				shouldClearDataContext = true;
			}
			else
				this.dataContext = dataContext;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		bool CheckCurrentRecord(ISeries series, IDataFilter dataFilter) {
			if (String.IsNullOrEmpty(dataFilter.ColumnName))
				return series.DataFiltersConjunction == Conjunction.And;
			DataBrowser browser = GetDataBrowser(ConvertToActualDataMember(dataMember, dataFilter.ColumnName));
			if (browser == null)
				return true;
			if (dataFilter.Value == null)
				switch (dataFilter.Condition) {
					case Condition.Equal:
						return browser.Current == null || browser.Current == DBNull.Value;
					case Condition.NotEqual:
						return browser.Current != null && browser.Current != DBNull.Value;
					default:
						return false;
				}
			if (browser.Current == null)
				return false;
			switch (dataFilter.Condition) {
				case Condition.Equal:
					return browser.Current.Equals(dataFilter.Value);
				case Condition.NotEqual:
					return !browser.Current.Equals(dataFilter.Value);
			}
			IComparable comparable = dataFilter.Value as IComparable;
			if (comparable == null)
				return false;
			try {
				int compareResult = comparable.CompareTo(ConvertToCompatibleValue(dataFilter, browser.Current));
				switch (dataFilter.Condition) {
					case Condition.GreaterThan:
						return compareResult < 0;
					case Condition.GreaterThanOrEqual:
						return compareResult <= 0;
					case Condition.LessThan:
						return compareResult > 0;
					case Condition.LessThanOrEqual:
						return compareResult >= 0;
				}
			}
			catch {
			}
			return false;
		}
		bool CheckCurrentRecordByIntersection(ISeries series) {
			foreach (IDataFilter filter in series.DataFilters)
				if (!CheckCurrentRecord(series, filter))
					return false;
			return true;
		}
		bool CheckCurrentRecordByMerging(ISeries series) {
			bool accept = false;
			int count = 0;
			foreach (IDataFilter filter in series.DataFilters) {
				bool checkResult = CheckCurrentRecord(series, filter);
				if (filter.Required) {
					if (!checkResult)
						return false;
				}
				else {
					count++;
					if (checkResult)
						accept = true;
				}
			}
			return accept || count == 0;
		}
		protected DataBrowser GetDataBrowser(string dataMember) {
			DataBrowser browser;
			if (!dataBrowsersCache.TryGetValue(dataMember, out browser)) {
				browser = dataContext.GetDataBrowser(dataSource, dataMember, true);
				dataBrowsersCache.Add(dataMember, browser);
			}
			return browser;
		}
		protected bool CheckCurrentRecord(ISeries series) {
			if (series.DataFilters == null)
				return true;
			return series.DataFiltersConjunction == Conjunction.Or ? CheckCurrentRecordByMerging(series) : CheckCurrentRecordByIntersection(series);
		}
		protected object GetToolTipHint(string toolTipHintDataMember) {
			if (!String.IsNullOrEmpty(toolTipHintDataMember)) {
				DataBrowser toolTipHintBrowser = DataContext.GetDataBrowser(DataSource, toolTipHintDataMember, true);
				if (toolTipHintBrowser != null)
					return toolTipHintBrowser.Current;
			}
			return null;
		}
		protected object GetColor(string colorDataMember) {
			if (!String.IsNullOrEmpty(colorDataMember)) {
				DataBrowser toolTipHintBrowser = DataContext.GetDataBrowser(DataSource, colorDataMember, true);
				if (toolTipHintBrowser != null)
					return toolTipHintBrowser.Current;
			}
			return null;
		}
		protected ISeriesPoint GetBindingPoint(ISeriesPointFactory factory, object argument, object dataRow, ISeries series, IList<string> valueDataMembers, string toolTipHintDataMember) {
			return GetBindingPoint(factory, argument, dataRow, series, valueDataMembers, toolTipHintDataMember, string.Empty);
		}
		protected ISeriesPoint GetBindingPoint(ISeriesPointFactory factory, object argument, object dataRow, ISeries series, IList<string> valueDataMembers, string toolTipHintDataMember, string colorDataMember) {
			string stringArg = argument as string;
			if (stringArg != null && String.IsNullOrEmpty(stringArg))
				return null;
			bool isEmpty = false;
			object[] values = new object[valueDataMembers.Count];
			for (int i = 0; i < valueDataMembers.Count; i++) {
				DataBrowser valueBrowser = GetDataBrowser(valueDataMembers[i]);
				if (valueBrowser == null)
					return null;
				object currentValue = valueBrowser.Current;
				if (currentValue == DBNull.Value || currentValue == null) {
					isEmpty = true;
					break;
				}
				string str = Convert.ToString(currentValue, CultureInfo.InvariantCulture);
				if (String.IsNullOrEmpty(str)) {
					isEmpty = true;
					break;
				}
				if (series.ValueScaleType == Scale.DateTime)
					values[i] = (currentValue is DateTime) ? currentValue : Convert.ToDateTime(str, CultureInfo.InvariantCulture);
				else {
					double val = Convert.ToDouble(str, CultureInfo.InvariantCulture);
					if (Double.IsNaN(val)) {
						isEmpty = true;
						break;
					}
					else
						values[i] = val;
				}
			}
			object currentToolTipHint = GetToolTipHint(toolTipHintDataMember);
			object color = GetColor(colorDataMember);
			return factory.CreateSeriesPoint(series, argument, 0.0, isEmpty ? null : values, null, dataRow, currentToolTipHint, color);
		}
		protected object ConvertToActualArgument(object argument) {
			if (argument == null || (argument is DBNull))
				return String.Empty;
			else if (argument is Enum)
				return argument.ToString();
			else if ((argument is DateTime) || (argument is string))
				return argument;
			return TryConvertToDouble(argument);
		}
		object TryConvertToDouble(object argument) {
			object actualArgument = null;
			if (argument is IConvertible) {
				try {
					actualArgument = Convert.ToDouble(argument, CultureInfo.InvariantCulture);
				}
				catch {
					actualArgument = argument.ToString();
				}
			}
			else
				actualArgument = argument.ToString();
			return actualArgument;
		}
	}
	public abstract class SeriesBindingProcedure : BindingProcedure {
		readonly ISeries series;
		readonly IList<string> valueDataMembers;
		readonly string toolTipHintDataMember;
		readonly string colorDataMember = string.Empty;
		readonly Scale valueScaleType;
		readonly string argumentDataMember;
		readonly IEnumerable<IDataFilter> dataFilters;
		readonly List<ISeriesPoint> points = new List<ISeriesPoint>();
		protected ISeries Series { get { return series; } }
		protected IList<string> ValueDataMembers { get { return valueDataMembers; } }
		protected string ToolTipHintDataMember { get { return toolTipHintDataMember; } }
		protected string ColorDataMember { get { return colorDataMember; } }
		protected Scale ValueScaleType { get { return valueScaleType; } }
		protected IList<ISeriesPoint> Points { get { return points; } }
		public SeriesBindingProcedure(ISeries series, DataContextBase dataContext, object dataSource, string dataMember)
			: base(dataContext, dataSource, dataMember) {
			if (series != null) {
				this.series = series;
				argumentDataMember = series.ArgumentDataMember;
				valueDataMembers = series.ValueDataMembers;
				toolTipHintDataMember = series.ToolTipHintDataMember;
				colorDataMember = series.ColorDataMember;
				if (!String.IsNullOrEmpty(dataMember)) {
					argumentDataMember = ConvertToActualDataMember(dataMember, argumentDataMember);
					List<string> actualValueDataMembers = new List<string>(valueDataMembers.Count);
					foreach (string item in valueDataMembers)
						actualValueDataMembers.Add(ConvertToActualDataMember(dataMember, item));
					valueDataMembers = actualValueDataMembers;
				}
				dataFilters = series.DataFilters;
				valueScaleType = series.ValueScaleType;
			}
		}
		protected void AddPoints(IEnumerable<ISeriesPoint> newPoints) {
			points.AddRange(newPoints);
		}
		protected virtual void Complete() {
		}
		protected abstract void PerformPointBinding(object argument, object dataRow);
		public IList<ISeriesPoint> CreateBindingPoints() {
			try {
				DataBrowser browser = CreateDataBrowser(DataContext, DataSource, argumentDataMember);
				if (browser == null || browser.Position < 0)
					return Points;
				DataBrowser argumentBrowser = DataContext.GetDataBrowser(DataSource, argumentDataMember, true);
				if (argumentBrowser == null || (argumentBrowser is ListBrowser))
					return Points;
				int currentPosition = browser.Position;
				try {
					for (int pos = 0; pos < browser.Count; pos++) {
						browser.Position = pos;
						if (CheckCurrentRecord(Series))
							PerformPointBinding(ConvertToActualArgument(argumentBrowser.Current), browser.Current);
					}
				}
				finally {
					browser.Position = currentPosition;
				}
			}
			finally {
				if (ShouldClearDataContext)
					DataContext.Clear();
			}
			Complete();
			return Points;
		}
	}
	public abstract class ChartBindingProcedure : BindingProcedure {
		readonly List<ISeries> autoCreatedSeries = new List<ISeries>();
		protected IList<ISeries> AutoCreatedSeries { get { return autoCreatedSeries; } }
		public ChartBindingProcedure(DataContextBase dataContext, object dataSource, string dataMember)
			: base(dataContext, dataSource, dataMember) {
		}
		IEnumerable<ISeries> CreateSortedSeriesList(Dictionary<object, ISeries> seriesList, SortMode sorting) {
			List<ISeries> sortedSeries = new List<ISeries>();
			List<object> keysList = new List<object>();
			keysList.AddRange(seriesList.Keys);
			foreach (object obj in keysList)
				if (!(obj is DBNull)) {
					IComparable comparable = obj as IComparable;
					if (comparable != null) {
						keysList.Sort(BindingHelperCore.CompareValues);
						if (sorting == SortMode.Descending)
							keysList.Reverse();
					}
					break;
				}
			foreach (object key in keysList) {
				ISeries series = seriesList[key];
				sortedSeries.Add(series);
			}
			return sortedSeries;
		}
		protected IList<string> GetActualValueDataMembers(IList<string> valueDataMembers) {
			if (!String.IsNullOrEmpty(DataMember)) {
				List<string> actualValueDataMembers = new List<string>(valueDataMembers.Count);
				foreach (string item in valueDataMembers)
					actualValueDataMembers.Add(ConvertToActualDataMember(DataMember, item));
				valueDataMembers = actualValueDataMembers;
			}
			return valueDataMembers;
		}
		protected virtual void Complete(ISeries series) {
		}
		protected abstract void PerformPointBinding(ISeriesTemplate seriesTemplate, object argument, object dataRow, IList<string> valueDataMambers, string toolTipHintDataMember, ISeries series);
		public IList<ISeries> CreateBindingSeries(ISeriesTemplate seriesTemplate, string seriesDataMember, SortMode sorting, string seriesNameBeginText, string seriesNameEndText) {
			try {
				Dictionary<object, ISeries> seriesList = new Dictionary<object, ISeries>();
				DataBrowser seriesBrowser = CreateDataBrowser(DataContext, DataSource, seriesDataMember);
				string argumentDataMember = (!String.IsNullOrEmpty(DataMember)) ?
					ConvertToActualDataMember(DataMember, seriesTemplate.ArgumentDataMember) :
					seriesTemplate.ArgumentDataMember;
				DataBrowser argumentBrowser = CreateDataBrowser(DataContext, DataSource, argumentDataMember);
				int currentPosition = seriesBrowser.Position;
				if (seriesBrowser != null && seriesBrowser.Position >= 0) {
					try {
						for (int pos = 0; pos < seriesBrowser.Count; pos++) {
							seriesBrowser.Position = pos;
							DataBrowser seriesValueBrowser = GetDataBrowser(seriesDataMember);
							if (seriesValueBrowser != null && seriesValueBrowser.Current != null) {
								ISeries currentSeries;
								seriesList.TryGetValue(seriesValueBrowser.Current, out currentSeries);
								if (currentSeries == null) {
									string seriesName = String.Format("{0}{1}{2}", seriesNameBeginText, seriesValueBrowser.Current.ToString(), seriesNameEndText);
									currentSeries = seriesTemplate.CreateSeriesForBinding(seriesName, seriesValueBrowser.Current);
									seriesList.Add(seriesValueBrowser.Current, currentSeries);
								}
								if (argumentBrowser != null && argumentBrowser.Position >= 0) {
									argumentBrowser.Position = pos;
									DataBrowser argumentValueBrowser = GetDataBrowser(argumentDataMember);
									if (argumentValueBrowser != null && !(argumentValueBrowser is ListBrowser)) {
										if (CheckCurrentRecord(currentSeries))
											PerformPointBinding(
												seriesTemplate,
												ConvertToActualArgument(argumentValueBrowser.Current),
												argumentBrowser.Current,
												GetActualValueDataMembers(seriesTemplate.ValueDataMembers),
												seriesTemplate.ToolTipHintDataMember,
												currentSeries
											);
									}
								}
							}
						}
					}
					finally {
						seriesBrowser.Position = currentPosition;
					}
				}
				foreach (ISeries series in seriesList.Values)
					Complete(series);
				IEnumerable<ISeries> allSeries = sorting != SortMode.None ? CreateSortedSeriesList(seriesList, sorting) : seriesList.Values;
				foreach (ISeries series in allSeries) {
					if (series.Points.GetEnumerator().MoveNext())
						autoCreatedSeries.Add(series);
				}
			}
			finally {
				if (ShouldClearDataContext)
					DataContext.Clear();
			}
			return autoCreatedSeries;
		}
		public IList<ISeries> CreateBindingSeries(ISeriesTemplate seriesTemplate, string seriesDataMember) {
			return CreateBindingSeries(seriesTemplate, seriesDataMember, SortMode.None, String.Empty, String.Empty);
		}
	}
	public class SimpleSeriesBindingProcedure : SeriesBindingProcedure {
		public SimpleSeriesBindingProcedure(ISeries series, DataContextBase dataContext, object dataSource, string dataMember)
			: base(series, dataContext, dataSource, dataMember) {
		}
		protected override void PerformPointBinding(object argument, object dataRow) {
			ISeriesPoint point = GetBindingPoint(Series, argument, dataRow, Series, ValueDataMembers, ToolTipHintDataMember, ColorDataMember);
			if (point != null)
				Points.Add(point);
		}
	}
	public class SimpleChartBindingProcedure : ChartBindingProcedure {
		public SimpleChartBindingProcedure(DataContextBase dataContext, object dataSource, string dataMember)
			: base(dataContext, dataSource, dataMember) {
		}
		protected override void PerformPointBinding(ISeriesTemplate seriesTemplate, object argument, object dataRow, IList<string> valueDataMambers, string toolTipHintDataMember, ISeries series) {
			ISeriesPoint point = GetBindingPoint(seriesTemplate, argument, dataRow, series, valueDataMambers, toolTipHintDataMember, series.ColorDataMember);
			if (point != null)
				series.AddSeriesPoint(point);
		}
	}
}
#endif
