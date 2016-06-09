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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Data.Browsing;
using DevExpress.Sparkline.Core;
using DevExpress.Sparkline;
using System.Windows.Forms;
using System.Data;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.ChartRangeControlClient.Core {
	public class BindingSourceClientSeries : IClientSeries {
		readonly List<SparklinePoint> points;
		readonly SparklineRange range;
		SparklineViewBase view;
		public SparklineViewBase View {
			get { return view; }
			set { view = value; }
		}
		public List<SparklinePoint> Points {
			get { return points; }
		}
		public BindingSourceClientSeries() {
			this.points = new List<SparklinePoint>();
			this.range = new SparklineRange();
		}
		#region IClientSeries
		SparklineViewBase ISparklineSettings.View {
			get { return view; }
		}
		Padding ISparklineSettings.Padding {
			get { return Padding.Empty; }
		}
		SparklineRange ISparklineSettings.ValueRange {
			get { return range; }
		}
		#endregion
		#region ISparklineExtendedData
		IList<SparklinePoint> ISparklineExtendedData.Points {
			get { return points; }
		}
		#endregion
	}
	public class BindingSource : IDisposable {
		#region Nested Classes: SeriesAccumulator
		class SeriesAccumulator {
			BindingSource bindingSource;
			DataBrowser browser;
			Dictionary<Object, BindingSourceClientSeries> seriesCache;
			int seriesCounter;
			public List<BindingSourceClientSeries> Series {
				get {
					return new List<BindingSourceClientSeries>(seriesCache.Values);
				}
			}
			public BindingSourceClientSeries CurrentSeries {
				get {
					object currentKey = browser != null ? browser.Current : bindingSource.ItemsSource;
					if (!IsNull(currentKey)) {
						BindingSourceClientSeries series;
						if (!seriesCache.TryGetValue(currentKey, out series)) {
							series = new BindingSourceClientSeries();
							bindingSource.AdjustSeries(currentKey, series, seriesCounter++);
							seriesCache.Add(currentKey, series);
						}
						return series;
					}
					return null;
				}
			}
			public SeriesAccumulator(BindingSource bindingSource) {
				this.bindingSource = bindingSource;
				this.seriesCounter = 0;
				seriesCache = new Dictionary<object, BindingSourceClientSeries>();
				if (!string.IsNullOrEmpty(bindingSource.seriesDataMember))
					browser = bindingSource.GetDataBrowser(bindingSource.seriesDataMember);
			}
		}
		#endregion
		static bool IsNumericType(Type type) {
			Type underlyingType = type != null ? Nullable.GetUnderlyingType(type) : null;
			Type prType = underlyingType != null ? underlyingType : type;
			if (prType != null) {
				TypeCode typeCode = DXTypeExtensions.GetTypeCode(prType);
				return ((int)typeCode >= (int)TypeCode.SByte) && ((int)typeCode <= (int)TypeCode.Decimal);
			}
			return false;
		}
		static bool IsDateTimeType(Type type) {
			Type underlyingType = type != null ? Nullable.GetUnderlyingType(type) : null;
			Type prType = underlyingType != null ? underlyingType : type;
			return prType == typeof(DateTime);
		}
		static bool IsNull(object value) {
			return (value == null) || (value is DBNull);
		}
		static PropertyDescriptor GetPropertyDescriptor(DataBrowser browser, string dataMember) {
			if (!string.IsNullOrEmpty(dataMember)) {
				var columnProperties = browser.GetItemProperties();
				for (int i = 0; i < columnProperties.Count; i++) {
					var descriptor = columnProperties[i];
					if (descriptor.Name == dataMember)
						return descriptor;
				}
			}
			if (dataMember.Contains(".")) {
				int index = dataMember.IndexOf('.');
				return GetPropertyDescriptor(browser, dataMember.Substring(index+1));
			}
			return null;
		}
		internal static double MapDateTimeValue(DateTime value) {
			return (value - DateTime.MinValue).TotalMilliseconds;
		}
		readonly SparklineScaleType scaleType;
		readonly Dictionary<string, DataBrowser> dataBrowserCache;
		readonly DataContextBase dataContext;
		readonly IBindingSourceDelegate sourceDelegate;
		List<IClientSeries> seriesCache;
		bool isDisposed;
		object itemsSource;
		string valueDataMember;
		string seriesDataMember;
		string argumentDataMember;
		bool IsBindingPossible {
			get { return (argumentDataMember != null) && (valueDataMember != null) && (itemsSource != null); }
		}
		public bool IsDisposed {
			get { return isDisposed; }
		}
		public DataContextBase DataContext {
			get { return dataContext; }
		}
		public object ItemsSource {
			get {
				return itemsSource;
			}
			set {
				if (itemsSource != value) {
					UnsubscribeSource();
					itemsSource = value;
					SubscribeSource();
					ClearContext();
					RaiseBindingChanged();
				}
			}
		}
		public string SeriesDataMember {
			get {
				return seriesDataMember;
			}
			set {
				if (seriesDataMember != value) {
					seriesDataMember = value;
					RaiseBindingChanged();
				}
			}
		}
		public string ArgumentDataMember {
			get {
				return argumentDataMember;
			}
			set {
				if (argumentDataMember != value) {
					argumentDataMember = value;
					RaiseBindingChanged();
				}
			}
		}
		public string ValueDataMember {
			get {
				return valueDataMember;
			}
			set {
				if (valueDataMember != value) {
					valueDataMember = value;
					RaiseBindingChanged();
				}
			}
		}
		public SparklineScaleType ScaleType {
			get { return scaleType; }
		}
		public BindingSource(SparklineScaleType scaleType, IBindingSourceDelegate sourceDelegate) {
			this.sourceDelegate = sourceDelegate;
			this.scaleType = scaleType;
			this.dataBrowserCache = new Dictionary<string, DataBrowser>();
			this.dataContext = new DataContextBase();
		}
		#region IDisposable
		public void Dispose() {
			if (!isDisposed) {
				if (dataContext != null)
					dataContext.Dispose();
				UnsubscribeSource();
				ClearContext();
				isDisposed = true;
			}
		}
		#endregion
		#region Data Source Subscriptions
		void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseBindingChanged();
		}
		void ItemsSourceBindingListChanged(object sender, ListChangedEventArgs e) {
			RaiseBindingChanged();
		}
		void ItemsSourceDataSetCollectionChanged(object sender, CollectionChangeEventArgs e) {
			DataTable table = e.Element as DataTable;
			if (table != null) {
				UnsubscribeDataTable(table);
				switch (e.Action) {
					case CollectionChangeAction.Add:
					case CollectionChangeAction.Refresh:
						SubscirbeDataTable(table);
						break;
				}
			}
		}
		void ItemsSourceDataSetColumnChanged(object sender, DataColumnChangeEventArgs e) {
			RaiseBindingChanged();
		}
		void ItemsSourceDataSetTableCleared(object sender, DataTableClearEventArgs e) {
			RaiseBindingChanged();
		}
		void ItemsSourceDataSetRowDeleted(object sender, DataRowChangeEventArgs e) {
			RaiseBindingChanged();
		}
		void ItemsSourceDataSetRowChanged(object sender, DataRowChangeEventArgs e) {
			RaiseBindingChanged();
		}
		void RaiseBindingChanged() {
			seriesCache = null;
			if (sourceDelegate != null)
				sourceDelegate.BindingChanged();
		}
		void SubscirbeDataTable(DataTable table) {
			UnsubscribeDataTable(table);
			table.ColumnChanged += ItemsSourceDataSetColumnChanged;
			table.RowChanged += ItemsSourceDataSetRowChanged;
			table.RowDeleted += ItemsSourceDataSetRowDeleted;
			table.TableCleared += ItemsSourceDataSetTableCleared;
		}
		void UnsubscribeDataTable(DataTable table) {
			table.ColumnChanged -= ItemsSourceDataSetColumnChanged;
			table.RowChanged -= ItemsSourceDataSetRowChanged;
			table.RowDeleted -= ItemsSourceDataSetRowDeleted;
			table.TableCleared -= ItemsSourceDataSetTableCleared;
		}
		void SubscribeSource() {
			if (itemsSource != null) {
				if (itemsSource is INotifyCollectionChanged)
					((INotifyCollectionChanged)itemsSource).CollectionChanged += ItemsSourceCollectionChanged;
				else if (itemsSource is IBindingList)
					((IBindingList)itemsSource).ListChanged += ItemsSourceBindingListChanged;
				else if (itemsSource is DataSet) {
					DataSet dataSet = (DataSet)itemsSource;
					dataSet.Tables.CollectionChanged += ItemsSourceDataSetCollectionChanged;
					for (int i = 0; i < dataSet.Tables.Count; i++)
						SubscirbeDataTable(dataSet.Tables[i]);
				}
			}
		}
		void UnsubscribeSource() {
			if (itemsSource != null) {
				if (itemsSource is INotifyCollectionChanged)
					((INotifyCollectionChanged)itemsSource).CollectionChanged -= ItemsSourceCollectionChanged;
				else if (itemsSource is IBindingList)
					((IBindingList)itemsSource).ListChanged -= ItemsSourceBindingListChanged;
				else if (itemsSource is DataSet) {
					DataSet dataSet = (DataSet)itemsSource;
					dataSet.Tables.CollectionChanged -= ItemsSourceDataSetCollectionChanged;
					for (int i = 0; i < dataSet.Tables.Count; i++)
						UnsubscribeDataTable(dataSet.Tables[i]);
				}
			}
		}
		#endregion
		void ClearContext() {
			dataContext.Clear();
			dataBrowserCache.Clear();
			seriesCache = null;
		}
		DataBrowser GetDataBrowser(string dataMember) {
			DataBrowser browser;
			if (!dataBrowserCache.TryGetValue(dataMember, out browser)) {
				browser = dataContext.GetDataBrowser(itemsSource, dataMember, true);
				dataBrowserCache.Add(dataMember, browser);
			}
			return browser;
		}
		DataBrowser CreateDataBrowser(string dataMember) {
			DataBrowser browser = dataContext.GetDataBrowser(itemsSource, String.Empty, true);
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
					browser = dataContext[itemsSource, parentDataMember];
					if (browser is ListBrowser)
						listBrowser = (ListBrowser)browser;
					propertyName = propertyName.Substring(dotPos + 1, propertyName.Length - dotPos - 1);
				} catch {
					break;
				}
			}
			return listBrowser != null ? listBrowser : browser;
		}
		bool CheckArgumentCompatibility(DataBrowser rootBrowser) {
			PropertyDescriptor descriptor = GetPropertyDescriptor(rootBrowser, argumentDataMember);
			if (descriptor != null) {
				if (scaleType == SparklineScaleType.Numeric)
					return IsNumericType(descriptor.PropertyType);
				else if (scaleType == SparklineScaleType.DateTime)
					return IsDateTimeType(descriptor.PropertyType);
			}
			return false;
		}
		bool CheckValueCompatibility(DataBrowser rootBrowser) {
			PropertyDescriptor descriptor = GetPropertyDescriptor(rootBrowser, valueDataMember);
			if (descriptor != null)
				return IsNumericType(descriptor.PropertyType);
			return false;
		}
		bool CheckItemsSourceCompatibility(DataBrowser rootBrowser) {
			return CheckArgumentCompatibility(rootBrowser) && CheckValueCompatibility(rootBrowser);
		}
		void AdjustSeries(object dataSourceValue, BindingSourceClientSeries series, int seriesCounter) {
			if (sourceDelegate != null)
				sourceDelegate.AdjustSeries(dataSourceValue, series, seriesCounter);
			else
				series.View = new LineSparklineView();
		}
		void ReadSource() {
			if (IsBindingPossible) {
				DataBrowser rootBrowser = CreateDataBrowser(String.IsNullOrEmpty(seriesDataMember) ? argumentDataMember : seriesDataMember);
				if (CheckItemsSourceCompatibility(rootBrowser)) {
					DataBrowser valueBrowser = GetDataBrowser(valueDataMember);
					if (valueBrowser != null) {
						SeriesAccumulator accumulator = new SeriesAccumulator(this);
						for (int i = 0; i < rootBrowser.Count; i++) {
							rootBrowser.Position = i;
							if (accumulator.CurrentSeries == null)
								continue;
							DataBrowser argumentValueBrowser = GetDataBrowser(argumentDataMember);
							if (argumentValueBrowser == null)
								continue;
							object nativeValue = valueBrowser.Current;
							object nativeArgument = argumentValueBrowser.Current;
							if (IsNull(nativeArgument) || IsNull(nativeValue))
								continue;
							double value = Convert.ToDouble(nativeValue);
							double argument;
							if (scaleType == SparklineScaleType.Numeric)
								argument = Convert.ToDouble(nativeArgument);
							else
								argument = MapDateTimeValue(Convert.ToDateTime(nativeArgument));
							accumulator.CurrentSeries.Points.Add(new SparklinePoint(argument, value));
						}
						seriesCache.AddRange(accumulator.Series);
					}
				}
			}
		}
		public IList<IClientSeries> GetSeries() {
			if (seriesCache == null) {
				seriesCache = new List<IClientSeries>();
				ReadSource();
			}
			return seriesCache;
		}
		public void RefreshData() {
			seriesCache = null;
			RaiseBindingChanged();
		}
	}
}
