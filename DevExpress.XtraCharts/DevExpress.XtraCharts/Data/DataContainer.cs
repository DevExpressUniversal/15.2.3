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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Data.ChartDataSources;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.XtraCharts.Native {
	public class DataContainer : ChartElement, IChartDataContainer, IXtraSupportDeserializeCollectionItem {
		#region DataContainer Notifications
		public class BindingNotification : Notification {
			readonly bool updateChart;
			public bool ShouldUpdateChart {
				get { return updateChart; }
			}
			public BindingNotification(DataContainer sender, bool updateChart) : base(sender, null) {
				this.updateChart = updateChart;
			}
		}
		public class BindingWillUpdateNotification : BindingNotification {
			public BindingWillUpdateNotification(DataContainer sender, bool updateChart) : base(sender, updateChart) {
			}
		}
		public class BindingWasUpdatedNotification : BindingNotification {
			public BindingWasUpdatedNotification(DataContainer sender, bool updateChart) : base(sender, updateChart) {
			}
		}		
		#endregion
		const int tempAutoCreatedSeriesCount = 3;
		readonly Chart chart;
		readonly SeriesCollection series;
		readonly SeriesBase seriesTemplate;
		readonly SeriesNameTemplate seriesNameTemplate;
		readonly PivotGridDataSourceOptions pivotGridDataSourceOptions;
		object dataSource;
		object dataAdapter;
		string dataMember;
		string seriesDataMember;
		SortingMode boundSeriesSorting;
		bool lockDataSourceEventsSubscription;
		bool bindingLocked;
		bool refreshDataOnRepaint;
		IList<ISeries> autocreatedSeries;
		IList<ISeries> tempAutocreatedSeries;
		IDataAdapter ActualDataAdapter {
			get {
				return Data.Native.BindingHelper.ConvertToIDataAdapter(dataAdapter);
			}
		}
		bool CanGetAutocreatedSeries {
			get {
				if (SeriesTemplate == null || SeriesTemplate.Owner == null || SeriesTemplate.GetDataSource() == null ||
					String.IsNullOrEmpty(SeriesTemplate.ArgumentDataMember) ||
					(!SeriesTemplate.IsSummaryBinding && SeriesTemplate.ValueDataMembers.Count == 0))
					return false;
				foreach (string valueDataMember in SeriesTemplate.ActualValueDataMembers)
					if (String.IsNullOrEmpty(valueDataMember))
						return false;
				return true;
			}
		}
		internal bool ShouldUseSeriesTemplate { 
			get { return SeriesTemplate.Visible && !String.IsNullOrEmpty(SeriesDataMember); } 
		}
		internal bool HasAutocreatedSeries { 
			get { return ShouldUseSeriesTemplate && autocreatedSeries != tempAutocreatedSeries; } 
		}
		internal Chart Chart { 
			get { return chart; } 
		}
		public IList<ISeries> AutocreatedSeries { 
			get { return autocreatedSeries; } 
		}
		public ViewController ViewController { 
			get { return chart.ViewController; } 
		}
		[NonTestableProperty]
		public object DataSource {
			get { return dataSource; }
			set { AttachToDataSource(value, false); }
		}
		public object ActualDataSource {
			get {
				if (DataSource != null)
					return DataSource;
				return (Chart.ContainerAdapter.DataProvider == null) ? null : Chart.ContainerAdapter.DataProvider.ParentDataSource;
			}
		}
		public object DataAdapter {
			get { return dataAdapter; }
			set {
				if (value != null && !Data.Native.BindingHelper.IsDataAdapter(value))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataAdapter), value.ToString()));
				if (value != dataAdapter) {
					SendNotification(new ElementWillChangeNotification(this));
					dataAdapter = value;
					UpdateBinding(true, true);
				}
			}
		}
		public string ActualSeriesDataMember { 
			get { return BindingProcedure.ConvertToActualDataMember(DataMember, SeriesDataMember); } 
		}
		public SeriesCollection Series {
			get {
				return series;
			}
		}
		[
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public Series[] SeriesSerializable {
			get {
				return CreateSeriesSerializableArray();
			}
			set {
				ReadSeriesSeralizableArray(value);
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public SeriesBase SeriesTemplate {
			get {
				return seriesTemplate;
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public SeriesNameTemplate SeriesNameTemplate { 
			get { return seriesNameTemplate; } 
		}
		[XtraSerializableProperty, NonTestableProperty]
		public string DataMember {
			get { return dataMember; }
			set {
				if (dataMember != value) {
					CheckDataMember(ActualDataSource, value);
					SendNotification(new ElementWillChangeNotification(this));
					dataMember = value;
					ProcessChangeBinding();
				}
			}
		}
		[XtraSerializableProperty, NonTestableProperty]
		public string SeriesDataMember {
			get { return seriesDataMember; }
			set {
				if (seriesDataMember != value) {
					CheckSeriesDataMember(ActualDataSource, value);
					SendNotification(new ElementWillChangeNotification(this));
					seriesDataMember = value;
					UpdateBinding(true, false);
				}
			}
		}
		[XtraSerializableProperty]
		public SortingMode BoundSeriesSorting {
			get { return boundSeriesSorting; }
			set {
				if (value != boundSeriesSorting) {
					SendNotification(new ElementWillChangeNotification(this));
					boundSeriesSorting = value;
					UpdateBinding(true, false);
				}
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PivotGridDataSourceOptions PivotGridDataSourceOptions {
			get {
				return pivotGridDataSourceOptions;
			}
		}
		[XtraSerializableProperty]
		public bool RefreshDataOnRepaint {
			get { return refreshDataOnRepaint; }
			set { refreshDataOnRepaint = value; }
		}
		public DataContainer(Chart chart) : base(chart) {
			this.chart = chart;
			this.series = new SeriesCollection(this);
			this.seriesTemplate = new SeriesBase();
			this.seriesTemplate.Owner = this;
			this.seriesNameTemplate = new SeriesNameTemplate(this);
			this.autocreatedSeries = new ISeries[0];
			this.refreshDataOnRepaint = true;
			this.bindingLocked = false;
			this.lockDataSourceEventsSubscription = false;
			this.dataMember = String.Empty;
			this.boundSeriesSorting = SortingMode.None;
			this.pivotGridDataSourceOptions = new PivotGridDataSourceOptions(this);
		}
		#region ShouldSerialize & Reset
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeSeriesTemplate() ||
				ShouldSerializeSeriesNameTemplate() ||
				ShouldSerializePivotGridDataSourceOptions() ||
				ShouldSerializeRefreshDataOnRepaint() ||
				ShouldSerializeDataMember() ||
				ShouldSerializeSeriesDataMember() ||
				ShouldSerializeSeriesSerializable() ||
				ShouldSerializeBoundSeriesSorting();
		}
		bool ShouldSerializeSeriesTemplate() {
			return SeriesTemplate.ShouldSerialize();
		}
		bool ShouldSerializeSeriesNameTemplate() {
			return SeriesNameTemplate.ShouldSerialize();
		}
		bool ShouldSerializePivotGridDataSourceOptions() {
			return PivotGridDataSourceOptions.ShouldSerialize();
		}
		public bool ShouldSerializeBoundSeriesSorting() {
			return boundSeriesSorting != SortingMode.None;
		}
		public bool ShouldSerializeDataMember() {
			return !String.IsNullOrEmpty(DataMember);
		}
		public bool ShouldSerializeSeriesDataMember() {
			return !String.IsNullOrEmpty(SeriesDataMember);
		}
		public bool ShouldSerializeSeriesSerializable() {
			return true;
		}
		public bool ShouldSerializeRefreshDataOnRepaint() {
			return !RefreshDataOnRepaint;
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "SeriesSerializable":
					return new Series();
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "SeriesSerializable":
					Series ser = e.Item.Value as Series;
					if (ser != null)
						Series.Add(ser);
					break;
			}
		}
		#endregion
		#region IChartDataContainer
		bool IChartDataContainer.DesignMode { get { return chart.Container.DesignMode; } }
		ISeriesBase IChartDataContainer.SeriesTemplate { get { return this.SeriesTemplate; } }
		bool IChartDataContainer.ShouldUseSeriesTemplate { get { return this.ShouldUseSeriesTemplate; } }
		#endregion
		void CheckDataSourcesDeleting(object source) {
			if (source == DataSource)
				AttachToDataSource(null, true);
			foreach (Series series in Series)
				if (source == series.DataSource)
					series.DataSource = null;
		}
		void CheckDataMember(object source, string member) {
			if (!Chart.Loading && (source != null) && !String.IsNullOrEmpty(member)) {
				if (!BindingHelper.CheckDataMember(Chart.ContainerAdapter.DataContext, source, member))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), member));
			}
		}
		void CheckSeriesDataMember(object source, string member) {
			if (!Chart.Loading && (source != null) && !String.IsNullOrEmpty(member)) {
				if (!BindingHelper.CheckDataMember(Chart.ContainerAdapter.DataContext, source, BindingProcedure.ConvertToActualDataMember(DataMember, member)))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), member));
			}
		}
		void SubscribeDataSourceEvents() {
			if (!lockDataSourceEventsSubscription) {
				IChartDataSource chartDataSource = DataSource as IChartDataSource;
				if (chartDataSource == null) {
					IBindingList bindingList = DataSource as IBindingList;
					if (bindingList != null)
						bindingList.ListChanged += new ListChangedEventHandler(DataWasChanged);
				} else
					chartDataSource.DataChanged += new DataChangedEventHandler(DataWasChanged);
			}
		}
		void UnsubscribeDataSourceEvents() {
			IChartDataSource chartDataSource = DataSource as IChartDataSource;
			if (chartDataSource == null) {
				IBindingList bindingList = DataSource as IBindingList;
				if (bindingList != null)
					bindingList.ListChanged -= new ListChangedEventHandler(DataWasChanged);
			} else
				chartDataSource.DataChanged -= new DataChangedEventHandler(DataWasChanged);
		}
		void SetDataSource(object source, bool isSourceDeleted) {
			UnsubscribeDataSourceEvents();
			if (source == null) {
				IChartDataSource chartDataSource = dataSource as IChartDataSource;
				if (chartDataSource != null) {
					chart.EnableLoadingMode();
					try {
						if (isSourceDeleted || SeriesDataMember == chartDataSource.SeriesDataMember)
							SeriesDataMember = null;
						if (isSourceDeleted || SeriesTemplate.ArgumentDataMember == chartDataSource.ArgumentDataMember)
							SeriesTemplate.ArgumentDataMember = null;
						for (int i = 0; i < SeriesTemplate.ValueDataMembers.Count; i++)
							if (isSourceDeleted || SeriesTemplate.ValueDataMembers[i] == chartDataSource.ValueDataMember)
								SeriesTemplate.ValueDataMembers[i] = String.Empty;
					} finally {
						chart.DisableLoadingMode();
					}
				}
			}
			dataSource = source;
			PivotGridDataSourceOptions.Initialize(source, isSourceDeleted);
			SubscribeDataSourceEvents();
		}
		void DataWasChanged() {
			IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
			if (dataProvider != null && dataProvider.DataContext != null)
				dataProvider.DataContext.Clear();
			if (Chart.ViewController.RefreshDataInternal(true))
				Chart.ContainerAdapter.Invalidate();
		}
		void DataWasChanged(object sender, DataChangedEventArgs args) {
			if (args.DataChangedType == DataChangedType.Reset)
				PivotGridDataSourceOptions.UpdateAutoBindingSettings();
			DataWasChanged();
		}
		void DataWasChanged(object sender, ListChangedEventArgs args) {
			DataWasChanged();
		}
		IList<ISeries> GetActualAutocreatedSeries() {
			IList<ISeries> seriesList = new List<ISeries>();
			IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
			DataContext dataContext = dataProvider == null ? null : dataProvider.DataContext;
			ChartBindingProcedure bindingProcedure;
			if (Chart.Container != null && SeriesTemplate.IsSummaryBinding) {
				string functionName = new SummaryFunctionParser(SeriesTemplate.SummaryFunction).FunctionName;
				SummaryFunctionDescription desc = Chart.Container.Chart.SummaryFunctions[functionName];
				if (desc == null)
					if (Chart.Container.DesignMode)
						return new List<ISeries>();
					else {
						string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgSummaryFunctionIsNotRegistered), functionName);
						throw new InvalidOperationException(message);
					}
				bindingProcedure = new SummaryChartBindingProcedure(null, dataContext, ActualDataSource, DataMember, desc.Function);
			} else
				bindingProcedure = new SimpleChartBindingProcedure(dataContext, ActualDataSource, DataMember);
			seriesList = bindingProcedure.CreateBindingSeries(SeriesTemplate, ActualSeriesDataMember, (SortMode)BoundSeriesSorting, SeriesNameTemplate.BeginText, SeriesNameTemplate.EndText);
			return seriesList;
		}
		void CreateTempAutocreatedSeries() {
			tempAutocreatedSeries = new List<ISeries>();
			string prefix = ChartLocalizer.GetString(ChartStringId.SeriesPrefix);
			for (int i = 0; i < tempAutoCreatedSeriesCount; i++) {
				Series tempSeries = CreateSeries(prefix + (i + 1).ToString());
				tempSeries.Autocreated = true;
				tempSeries.Permanent = true;
				tempAutocreatedSeries.Add(tempSeries);
			}
		}
		void UpdateTempAutocreatedSeries() {
			foreach (Series series in tempAutocreatedSeries) {
				series.Owner = null;
				series.Assign(SeriesTemplate);
				series.Owner = SeriesTemplate.Owner;
			}
		}
		IList<ISeries> GetTempAutocreatedSeries() {
			if (tempAutocreatedSeries == null)
				CreateTempAutocreatedSeries();
			else
				UpdateTempAutocreatedSeries();
			return tempAutocreatedSeries;
		}
		IList<ISeries> GetAutocreatedSeries() {
			if (ShouldUseSeriesTemplate) {
				if (ActualDataSource != null && Chart.CanUseBoundPoints && CanGetAutocreatedSeries) {
					IList<ISeries> autoCreatedSeries = GetActualAutocreatedSeries();
					if (autoCreatedSeries.Count > 0)
						return autoCreatedSeries;
				}
				if (Chart.Container.DesignMode)
					return GetTempAutocreatedSeries();
			}
			return new ISeries[0];
		}
		void AssignTempAutocreatedSeries(IList<ISeries> sourceSeries) {
			if (sourceSeries != null) {
				tempAutocreatedSeries = new List<ISeries>(tempAutoCreatedSeriesCount);
				for (int i = 0; i < tempAutoCreatedSeriesCount; i++) {
					Series tempSeries = new Series() { Autocreated = true, Permanent = true };
					tempSeries.Assign((Series)sourceSeries[i]);
					tempAutocreatedSeries.Add(tempSeries);
				}
			}
		}
		void AddAutocreatedSeriesToSeriesCollection(IList<ISeries> realAutocreatedSeries) {
			try {
				for (int i = realAutocreatedSeries.Count - 1; i >= 0; i--)
					Series.Insert(0, (Series)realAutocreatedSeries[i]);
			} catch {
			}
		}
		void RemoveAutocreatedSeriesFromSeriesCollection() {
			try {
				int index = 0;
				while (index < Series.Count) {
					if (Series[index].Autocreated)
						Series.RemoveAt(index);
					else
						index++;
				}
			} catch {
			}
		}
		bool ShouldRefreshData() {
			if (!refreshDataOnRepaint)
				return false;
			if (!(DataSource is IBindingList))
				return true;
			foreach (Series ser in Series)
				if (ser.ShouldRefreshData())
					return true;
			return false;
		}
		Series[] CreateSeriesSerializableArray() {
			List<Series> list = new List<Series>(Series.Count);
			foreach (Series item in Series)
				if (!item.Autocreated)
					list.Add(item);
			return list.ToArray();
		}
		void AttachToDataSource(object source, bool isDataSourceDeleted) {
			foreach (Series series in Series)
				series.ResetDataSourceArgumentScaleType();
			if (source == Convert.DBNull)
				source = null;
			if ((source != null) && !(source is IList) && !(source is IListSource))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidDataSource));
			if (((dataSource == null) && (source != null)) || ((dataSource != null) && !dataSource.Equals(source))) {
				if (!Chart.Loading && !Chart.Container.DesignMode)
					CheckBindingRuntime(source);
				SendNotification(new ElementWillChangeNotification(this));
				SetDataSource(source, isDataSourceDeleted);
				if ((dataSource != null) && Chart.Container.DesignMode) {
					DataTable table = DataSource as DataTable;
					if ((table != null) && (Chart.Container.Site != null) && (Chart.Container.Site.Container != null)) {
						ArrayList objects = new ArrayList();
						foreach (object component in Chart.Container.Site.Container.Components)
							objects.Add(component);
						dataAdapter = DevExpress.Data.Native.BindingHelper.DetermineDataAdapter(objects, table);
					}
				}
				ProcessChangeBinding();
				SendNotification(new ElementChangedNotification(this, new PropertyUpdateInfo(this, "DataSource")));
			}
		}
		void ReadSeriesSeralizableArray(Series[] data) {
			Series.BeginUpdate();
			try {
				Series.Clear();
				Series.AddRange(data);
			} finally {
				Series.EndUpdate();
			}
		}
		void UpdateFixedSeriesBinding() {
			foreach (Series series in Series)
				if (!series.Autocreated)
					series.UpdateBoundPoints();
		}
		void UpdateAutocreatedSeries() {
			RemoveAutocreatedSeriesFromSeriesCollection();
			PivotGridDataSourceOptions.UpdateDataMembers(true);
			autocreatedSeries = GetAutocreatedSeries();
			AddAutocreatedSeriesToSeriesCollection(autocreatedSeries);
			PivotGridDataSourceOptions.CheckData();
			PivotGridDataSourceOptions.UpdateAutoLayoutSettings();
		}
		public void DataChanged(ChartUpdateInfoBase changeInfo) {
			SendNotification(new ElementChangedNotification(this, changeInfo));
		}
		protected override ChartElement CreateObjectForClone() {
			throw new NotImplementedException();
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed) {
				UnsubscribeDataSourceEvents();
				DisposeTempAutocreatedSeries();
			}
			base.Dispose(disposing);
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			DataChanged(changeInfo);
			return false;
		}
		protected internal override void ChildCollectionChanged(ChartCollectionBase collection, ChartUpdateInfoBase changeInfo) {
			if (!IsDisposed) {
				if (collection == series)
					DataChanged(changeInfo);
			}
		}
		internal bool IsAutocreatedSeriesChanged() {
			IList<ISeries> newAutocreatedSeries = GetAutocreatedSeries();
			if (newAutocreatedSeries == autocreatedSeries)
				return false;
			if (newAutocreatedSeries.Count != autocreatedSeries.Count)
				return true;
			for (int i = 0; i < autocreatedSeries.Count; i++) {
				Series oldSeries = (Series)autocreatedSeries[i];
				Series newSeries = (Series)newAutocreatedSeries[i];
				if (oldSeries.Name != newSeries.Name || !oldSeries.Points.ArePointEquals(newSeries.Points))
					return true;
			}
			return false;
		}
		internal void DisposeTempAutocreatedSeries() {
			if (tempAutocreatedSeries != null)
				foreach (Series item in tempAutocreatedSeries)
					item.Dispose();
			tempAutocreatedSeries = null;
		}
		internal void EndLoadSeries(bool validatePointsId) {
			SeriesTemplate.OnEndLoading(validatePointsId);
			foreach (Series series in Series)
				if (!series.Autocreated)
					series.OnEndLoading(validatePointsId);
			if (tempAutocreatedSeries != null)
				foreach (Series series in tempAutocreatedSeries)
					series.OnEndLoading(validatePointsId);
		}
		internal bool IsFixedSeriesChanged() {
			if (Chart.Diagram != null) {
				foreach (Series sereis in Series)
					if (!sereis.Autocreated && sereis.IsBoundDataChanged())
						return true;
			}
			return false;
		}
		internal bool ForceRefresh() {
			return ShouldRefreshData() && (IsAutocreatedSeriesChanged() || IsFixedSeriesChanged());
		}
		public void LockDataSourceEventsSubscription() {
			lockDataSourceEventsSubscription = true;
		}
		public Series CreateSeries(string name) {
			return SeriesTemplate.CreateSeries(name);
		}
		[Obsolete("This method is obsolete now. Use the ResetLegendTextPattern method instead.")]
		public void ResetLegendPointOptions() {
			if (SeriesTemplate != null)
				SeriesTemplate.ResetLegendPointOptions();
			if (Series != null)
				foreach (Series item in Series)
					item.ResetLegendPointOptions();
		}
		public void ResetLegendTextPattern() {
			if (SeriesTemplate != null)
				SeriesTemplate.LegendTextPattern = String.Empty;
			if (Series != null)
				foreach (Series item in Series)
					item.LegendTextPattern = String.Empty;
		}
		public void CheckBindingRuntime(object source) {
			CheckDataMember(source, DataMember);
			CheckSeriesDataMember(source, SeriesDataMember);
			SeriesTemplate.CheckBindingRuntime(source);
			foreach (Series series in Series)
				if (series.DataSource == null)
					series.CheckBindingRuntime(source);
		}
		public void OnObjectDeleted(object obj) {
			if (obj == DataAdapter)
				dataAdapter = null;
			else {
				CheckDataSourcesDeleting(obj);
				DataSet dataSet = obj as DataSet;
				if (dataSet != null)
					foreach (DataTable table in dataSet.Tables)
						CheckDataSourcesDeleting(table);
			}
		}
		public bool CanFillDataSource() {
			if (Chart.Container.ControlType == ChartContainerType.WebControl)
				return DataSource != null;
			return !Chart.Container.Loading && (ActualDataAdapter != null) && (Data.Native.BindingHelper.ConvertToDataSet(ActualDataSource) != null);
		}
		public bool CanClearDataSource() {
			if (Chart.Container.ControlType != ChartContainerType.WebControl)
				return Data.Native.BindingHelper.ConvertToDataSet(DataSource) != null;
			if (DataSource == null)
				return false;
			ICollection collection = DataSource as ICollection;
			return collection != null && collection.Count > 0;
		}
		public void FillDataSource() {
			ActualDataAdapter.Fill(Data.Native.BindingHelper.ConvertToDataSet(ActualDataSource));
		}
		public bool CanPerformDataSnapshot() {
			foreach (Series series in Series)
				if (!series.Permanent && (series.Autocreated || series.CanDataSnapshot))
					return true;
			return false;
		}
		public void ProcessChangeBinding() {
			object actualDataSource = ActualDataSource;
			if (actualDataSource != null && !Chart.Container.Loading) {
				IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
				DataContext dataContext = dataProvider != null ? dataProvider.DataContext : null;
				if (!BindingHelper.CheckDataMember(dataContext, actualDataSource, DataMember))
					DataMember = null;
				if (!BindingHelper.CheckDataMember(dataContext, actualDataSource, ActualSeriesDataMember))
					SeriesDataMember = null;
				SeriesTemplate.CheckBinding(actualDataSource);
				foreach (Series series in Series)
					if (series.DataSource == null)
						series.CheckBinding(actualDataSource);
			}
			UpdateBinding(true, true);
		}
		public void DataSnapshot() {
			if (CanPerformDataSnapshot()) {
				SendNotification(new ElementWillChangeNotification(this));
				foreach (Series series in Series)
					series.DataSnapshot();
				DataAdapter = null;
				SetDataSource(null, false);
				SeriesDataMember = null;
				UpdateBinding(true, false);
			}
		}
		public void BindToData(SeriesViewBase view, object dataSource, string dataMember, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			CheckBindingRuntime(dataSource);
			IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
			object data = (dataSource != null) ? dataSource :
				((dataProvider == null) ? null : dataProvider.ParentDataSource);
			if (SeriesDataMember != seriesDataMember)
				CheckSeriesDataMember(data, seriesDataMember);
			if (SeriesDataMember != seriesDataMember)
				CheckSeriesDataMember(data, seriesDataMember);
			if (DataMember != dataMember)
				CheckDataMember(data, dataMember);
			if (argumentDataMember != SeriesTemplate.ArgumentDataMember)
				SeriesTemplate.CheckDataMember(data, argumentDataMember, SeriesTemplate.ArgumentScaleType);
			foreach (string valueDataMember in valueDataMembers)
				SeriesTemplate.CheckDataMember(data, valueDataMember, SeriesTemplate.ValueScaleType);
			SendNotification(new ElementWillChangeNotification(this));
			SeriesTemplate.SetView(view, false);
			SetDataSource(dataSource, false);
			this.DataMember = dataMember;
			this.SeriesDataMember = seriesDataMember;
			SeriesTemplate.SetArgumentDataMember(argumentDataMember);
			SeriesTemplate.ValueDataMembers.Full(valueDataMembers);
			ProcessChangeBinding();
		}
		public void BindToData(ViewType viewType, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			BindToData(viewType, dataSource, string.Empty, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(ViewType viewType, object dataSource, string dataMember, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			SeriesViewBase view = SeriesViewFactory.CreateInstance(viewType);
			BindToData(view, dataSource, dataMember, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void BindToData(SeriesViewBase view, object dataSource, string seriesDataMember, string argumentDataMember, params string[] valueDataMembers) {
			BindToData(view, dataSource, string.Empty, seriesDataMember, argumentDataMember, valueDataMembers);
		}
		public void UpdateBinding(bool updateSmartBinding, bool updateSeriesBinding) {
			UpdateBinding(updateSmartBinding, updateSeriesBinding, true);
		}
		public void UpdateBinding(bool updateSmartBinding, bool updateSeriesBinding, bool updateChart) {
			UpdateBinding(updateSmartBinding, updateSeriesBinding, updateChart, null);
		}
		public bool UpdateBinding(bool updateSmartBinding, bool updateSeriesBinding, bool updateChart, ChartUpdateInfoBase changeInfo) {
			if (bindingLocked || !(updateSmartBinding || updateSeriesBinding) || Loading)
				return false;
			bool result = false;
			LockBinding();
			SendNotification(new BindingWillUpdateNotification(this, updateChart));
			try {
				if (updateSmartBinding)
					UpdateAutocreatedSeries();
				if (updateSeriesBinding)
					UpdateFixedSeriesBinding();
				PivotGridDataSourceOptions.UpdateXAxisLabels();
				if (changeInfo != null) {
					DataChanged(changeInfo);
					result = true;
				}
				SendNotification(new BindingWasUpdatedNotification(this, updateChart));
			}
			finally {
				UnlockBinding();
				chart.SelectionController.RestoreSelectedItems();
			}
			return result;
		}
		public void LockBinding() {
			bindingLocked = true;
		}
		public void UnlockBinding() {
			bindingLocked = false;
		}
		public Series GetSeriesByName(string seriesName) {
			foreach (Series series in Series)
				if (series.Name == seriesName)
					return series;
			return null;
		}
		public override void Assign(ChartElement obj) {
			DataContainer container = obj as DataContainer;
			if (container == null)
				return;
			boundSeriesSorting = container.boundSeriesSorting;
			DataAdapter = container.DataAdapter;
			DataSource = container.DataSource;
			SeriesDataMember = container.SeriesDataMember;
			SeriesTemplate.Assign(container.SeriesTemplate);
			SeriesNameTemplate.Assign(container.SeriesNameTemplate);
			Series.Assign(container.Series);
			AssignTempAutocreatedSeries(container.tempAutocreatedSeries);
			RemoveAutocreatedSeriesFromSeriesCollection();
			PivotGridDataSourceOptions.Assign(container.PivotGridDataSourceOptions);
			RefreshDataOnRepaint = container.RefreshDataOnRepaint;
		}
	}
}
