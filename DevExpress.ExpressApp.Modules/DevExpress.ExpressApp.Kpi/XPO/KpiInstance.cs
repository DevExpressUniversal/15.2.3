#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.Collections;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Kpi {
	[Appearance("KpiXpoTrendBetter", TargetItems = "Trend, Change", FontColor = "Green", Criteria = "Trend = 'Better'")]
	[Appearance("KpiXpoTrendWorse", TargetItems = "Trend, Change", FontColor = "Red", Criteria = "Trend = 'Worse'")]
	[Appearance("KpiXpoRedZone", TargetItems = "*", BackColor = "Pink", Criteria = "(KpiDefinition.RedZone <> 0) And ((KpiDefinition.Direction = 'HighIsBetter' And Current <= KpiDefinition.RedZone) Or (KpiDefinition.Direction = 'LowIsBetter' And Current >= KpiDefinition.RedZone))", Context = "ListView")]
	[CreatableItem(false)]
	[System.ComponentModel.DisplayName("KPI")]
	[ImageName("BO_KPI_Definition")]
	public class KpiInstance : BaseKpiObject, IKpiInstance, IKpiHistoryCacheManager, ISettingsProvider {
		private KpiDefinition kpiDefinition;
		[Persistent("ForceMeasurementDateTime")]
		[Browsable(false)]
		private DateTime forceMeasurementDateTime;
		private KpiChartDataSource kpiChartDataSource;
		private int getDataLockCount = 0;
		private void EnsureKpiChartDataSource() {
			if(kpiChartDataSource == null) {
				kpiChartDataSource = new KpiChartDataSource(this);
			}
		}
		protected void RefreshKpiChartDataSource() {
			if(kpiChartDataSource != null) {
				kpiChartDataSource.Refresh();
			}
		}
		private static int SortHistoryItems(KpiHistoryItem item1, KpiHistoryItem item2) {
			if(item1.RangeStart > item2.RangeStart) {
				return 1;
			}
			if(item1.RangeStart < item2.RangeStart) {
				return -1;
			}
			return 0;
		}
		private void CleanupOutdatedCacheItems() {
			List<KpiHistoryItem> itemsToDelete = new List<KpiHistoryItem>();
			foreach(KpiHistoryItem item in HistoryItems) {
				if(item.RangeStart <= GetCacheStartBoundary()) {
					itemsToDelete.Add(item);
				}
			}
			foreach(KpiHistoryItem item in itemsToDelete) {
				item.Delete();
			}
		}
		private void ClenupLastMeasurementIfOutdated(bool forceUpdate) {
			if(forceUpdate || ForceMeasurementDateTime <= DateTime.Now) {
				int historyItemsCount = HistoryItems.Count;
				if(historyItemsCount > 0) {
					HistoryItems.Sorting.Add(new SortProperty("RangeEnd", DevExpress.Xpo.DB.SortingDirection.Descending));
					List<KpiHistoryItem> itemsToDelete = new List<KpiHistoryItem>(); 
					itemsToDelete.Add(HistoryItems[0]);
					if(HistoryItems.Count > 1) {
						itemsToDelete.Add(HistoryItems[1]);
					}
					foreach(KpiHistoryItem item in itemsToDelete) {
						item.Delete();
					}
				}
			}
		}
		private void EnsureCache() {
			float dummy = Current;
			if(kpiDefinition.Compare) {
				dummy = Previous.Value;
			}
			object obj = ChartDataSource.DataSource;
		}
		protected virtual KpiChartDataSource CreateKpiChartDataSource() {
			return new KpiChartDataSource(this);
		}
		public void ResetHistoryForInvalidInstance() {
			if(KpiInstanceHelper.AddDuration(KpiDefinition.ChangedOn, KpiDefinition.MeasurementFrequency, false) > ForceMeasurementDateTime) {
				Reset();
			}
		}
		protected virtual DateTime GetCacheStartBoundary() {
			return KpiInstanceHelper.GetCacheStartBoundary(this);
		}
		protected virtual DateTime GetCacheEndBoundary() {
			return KpiInstanceHelper.GetCacheEndBoundary(this);
		}
		public virtual float GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd) {
			return GetValueFromHistory(rangeStart, rangeEnd, rangeStart, rangeEnd);
		}
		public virtual IList GetValuesFromHistory(MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd) {
			return new KpiMeasurementHistoryItem[] { new KpiMeasurementHistoryItem(measurementType, rangeEnd, GetValueFromHistory(rangeStart, rangeEnd)) };
		}
		public virtual float GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			KpiHistoryItem historyItem = (KpiHistoryItem)FindHistoryItem(rangeStart, rangeEnd);
			if(historyItem == null) {
				float result = kpiDefinition.Evaluate(evaluationStart, evaluationEnd);
				if(KpiModule.EnableSelfRefresh) {
					historyItem = new KpiHistoryItem(Session, this, result, rangeStart, rangeEnd);
					AddMeasurement(historyItem);
					historyItem.Save();
				}
				RefreshKpiChartDataSource();
			}
			return historyItem.Value;
		}
		public virtual IList GetValuesFromHistory(MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			return new KpiMeasurementHistoryItem[] { new KpiMeasurementHistoryItem(measurementType, rangeEnd, GetValueFromHistory(rangeStart, rangeEnd, evaluationStart, evaluationEnd)) };
		}
		protected void AddMeasurement(KpiHistoryItem kpiHistoryItem) {
			forceMeasurementDateTime = KpiInstanceHelper.AddDuration(DateTime.Now, KpiDefinition.MeasurementFrequency, false);
			HistoryItems.Add(kpiHistoryItem);
			Save();
			if(getDataLockCount < 2) { 
				OnChanged("ForceMeasurementDateTime");
			}
		}
		private float CalculateDelta() {
			if(!Previous.HasValue) {
				return 0;
			}
			if(Previous.Value == Current) {
				return 0;
			}
			float percentResult = 1;
			if(Previous.Value != 0) {
				percentResult = (Current - Previous.Value) / Previous.Value;
			}
			return percentResult;
		}
		public KpiInstance(DevExpress.Xpo.Session session)
			: base(session) {
		}
		public KpiInstance(DevExpress.Xpo.Session session, KpiDefinition kpi)
			: this(session) {
			KpiDefinition = kpi;
		}
		[Browsable(false)]
		public KpiDefinition KpiDefinition {
			get { return kpiDefinition; }
			set { SetPropertyValue("KpiDefinition", ref kpiDefinition, value); }
		}
		[Association("HistoryCache-HistoryItems"), Aggregated]
		public XPCollection<KpiHistoryItem> HistoryItems {
			get { return GetCollection<KpiHistoryItem>("HistoryItems"); }
		}
		public IList<IKpiHistoryItem> GetHistoryItems() {
			List<IKpiHistoryItem> result = new List<IKpiHistoryItem>();
			foreach(KpiHistoryItem item in HistoryItems) {
				result.Add(item);
			}
			return result;
		}
		[PersistentAlias("forceMeasurementDateTime")]
		[Browsable(false)]
		public DateTime ForceMeasurementDateTime {
			get { return forceMeasurementDateTime; }
			set { SetPropertyValue("ForceMeasurementDateTime", ref forceMeasurementDateTime, value); }
		}
		[Association("Scorecards-Indicators")]
		public XPCollection<KpiScorecard> Scorecards {
			get { return GetCollection<KpiScorecard>("Scorecards"); }
		}
		public IKpiDefinition GetKpiDefinition() {
			return KpiDefinition;
		}
		public void CleanupCache(bool forceUpdate) {
			CleanupOutdatedCacheItems();
			ClenupLastMeasurementIfOutdated(forceUpdate);
		}
		public void Reset() {
			List<KpiHistoryItem> itemsToDelete = new List<KpiHistoryItem>();
			itemsToDelete.AddRange(HistoryItems);
			foreach(KpiHistoryItem item in itemsToDelete) {
				item.Delete();
			}
		}
		public IKpiHistoryItem FindHistoryItem(DateTime startDate, DateTime endDate) {
			return KpiInstanceHelper.FindHistoryItem(this, startDate, endDate);
		}
		public void Refresh(bool forceMeasurement) {
			bool enableSelfRefresh = KpiModule.EnableSelfRefresh;
			try {
				KpiModule.EnableSelfRefresh = true;
				ResetHistoryForInvalidInstance();
				CleanupCache(forceMeasurement);
				EnsureCache();
			}
			finally {
				KpiModule.EnableSelfRefresh = enableSelfRefresh;
			}
		}
		[System.ComponentModel.DisplayName("Name")]
		public string KpiName {
			get { return KpiDefinition.Name; }
		}
		public string Period {
			get { return KpiDefinition.Period; }
		}
		public float Current {
			get {
				try {
					LockData();
					return KpiInstanceHelper.GetValueByRange(this, kpiDefinition.Range, kpiDefinition.MeasurementFrequency);
				}
				finally {
					UnLockData();
				}
			}
		}
		public float? Previous {
			get {
				try {
					LockData();
					if(kpiDefinition.Compare) {
						return KpiInstanceHelper.GetValueByRange(this, kpiDefinition.RangeToCompare, kpiDefinition.MeasurementFrequency);
					}
					return null;
				}
				finally {
					UnLockData();
				}
			}
		}
		public float? Change {
			get {
				try {
					LockData();
					if(kpiDefinition.Compare) {
						return CalculateDelta();
					}
					return null;
				}
				finally {
					UnLockData();
				}
			}
		}
		public Trend? Trend {
			get {
				try {
					LockData();
					if(!Change.HasValue) {
						return null;
					}
					float delta = Change.Value;
					if(delta > 0) return kpiDefinition.Direction == Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Better : DevExpress.ExpressApp.Editors.Trend.Worse;
					if(delta < 0) return kpiDefinition.Direction == Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Worse : DevExpress.ExpressApp.Editors.Trend.Better;
					return DevExpress.ExpressApp.Editors.Trend.NoChange;
				}
				finally {
					UnLockData();
				}
			}
		}
		private void LockData() {
			getDataLockCount++;
		}
		private void UnLockData() {
			getDataLockCount--;
		}
		[VisibleInDetailView(false), VisibleInLookupListView(false)]
		public ISparklineProvider Sparkline {
			get {
				EnsureKpiChartDataSource();
				return kpiChartDataSource; 
			}
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public IChartDataSourceProvider ChartDataSource {
			get {
				EnsureKpiChartDataSource();
				return kpiChartDataSource; 
			}
		}
		[VisibleInListView(false), VisibleInLookupListView(false)]
		public IValueWithTrendProvider GaugeDataSource {
			get { return new KpiGaugeDataSource(this); }
		}
		#region ISettingsProvider Members
		private string settings;
		[Size(SizeAttribute.Unlimited)]
		[Browsable(false)]
		public virtual string Settings {
			get { return settings; }
			set { SetPropertyValue("Settings", ref settings, value); }
		}
		#endregion
	}
}
