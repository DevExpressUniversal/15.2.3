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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl.EF.Kpi {
	[Appearance("KpiTrendBetter", TargetItems = "Trend, Change", FontColor = "Green", Criteria = "Trend = 'Better'")]
	[Appearance("KpiTrendWorse", TargetItems = "Trend, Change", FontColor = "Red", Criteria = "Trend = 'Worse'")]
	[Appearance("KpiRedZone", TargetItems = "*", BackColor = "Pink", Criteria = "(KpiDefinition.RedZone <> 0) And ((KpiDefinition.Direction = 'HighIsBetter' And Current <= KpiDefinition.RedZone) Or (KpiDefinition.Direction = 'LowIsBetter' And Current >= KpiDefinition.RedZone))", Context = "ListView")]
	[CreatableItem(false)]
	[System.ComponentModel.DisplayName("KPI")]
	[ImageName("BO_KPI_Definition")]
	public class KpiInstance : BaseKpiObject, DevExpress.ExpressApp.Kpi.IKpiInstance, DevExpress.ExpressApp.Kpi.IKpiHistoryCacheManager, ISettingsProvider, IObjectSpaceLink {
		private DevExpress.ExpressApp.Kpi.KpiChartDataSource kpiChartDataSource;
		private Int32 getDataLockCount;
		private IObjectSpace objectSpace;
		private void EnsureKpiChartDataSource() {
			if(kpiChartDataSource == null) {
				kpiChartDataSource = new DevExpress.ExpressApp.Kpi.KpiChartDataSource(this);
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
		private void LockData() {
			getDataLockCount++;
		}
		private void UnLockData() {
			getDataLockCount--;
		}
		private void CleanupOutdatedCacheItems() {
			List<KpiHistoryItem> itemsToDelete = new List<KpiHistoryItem>();
			foreach(KpiHistoryItem item in HistoryItems) {
				if(item.RangeStart <= GetCacheStartBoundary()) {
					itemsToDelete.Add(item);
				}
			}
			foreach(KpiHistoryItem item in itemsToDelete) {
				HistoryItems.Remove(item);
				objectSpace.Delete(item);
			}
		}
		private void ClenupLastMeasurementIfOutdated(Boolean forceUpdate) {
			if(forceUpdate || ForceMeasurementDateTime <= DateTime.Now) {
				int historyItemsCount = HistoryItems.Count;
				if(historyItemsCount > 0) {
					IList<KpiHistoryItem> sortedHistoryItems = HistoryItems.OrderByDescending(hi => hi.RangeEnd).ToList();
					List<KpiHistoryItem> itemsToDelete = new List<KpiHistoryItem>();
					itemsToDelete.Add(sortedHistoryItems[0]);
					if(HistoryItems.Count > 1) {
						itemsToDelete.Add(sortedHistoryItems[1]);
					}
					foreach(KpiHistoryItem item in itemsToDelete) {
						HistoryItems.Remove(item);
					}
					objectSpace.Delete(itemsToDelete);
				}
			}
		}
		private void EnsureCache() {
			Single val = Current;
			if(KpiDefinition.Compare) {
				val = Previous.Value;
			}
			Object obj = ChartDataSource.DataSource;
		}
		private Single CalculateDelta() {
			if(!Previous.HasValue) {
				return 0;
			}
			if(Previous.Value == Current) {
				return 0;
			}
			Single percentResult = 1;
			if(Previous.Value != 0) {
				percentResult = (Current - Previous.Value) / Previous.Value;
			}
			return percentResult;
		}
		protected void RefreshKpiChartDataSource() {
			if(kpiChartDataSource != null) {
				kpiChartDataSource.Refresh();
			}
		}
		protected void AddMeasurement(KpiHistoryItem kpiHistoryItem) {
			ForceMeasurementDateTime = DevExpress.ExpressApp.Kpi.KpiInstanceHelper.AddDuration(DateTime.Now, KpiDefinition.MeasurementFrequency, false);
			HistoryItems.Add(kpiHistoryItem);
			objectSpace.SetModified(this, objectSpace.TypesInfo.FindTypeInfo(typeof(KpiInstance)).FindMember("ForceMeasurementDateTime"));
		}
		protected virtual DevExpress.ExpressApp.Kpi.KpiChartDataSource CreateKpiChartDataSource() {
			return new DevExpress.ExpressApp.Kpi.KpiChartDataSource(this);
		}
		protected virtual DateTime GetCacheEndBoundary() {
			return DevExpress.ExpressApp.Kpi.KpiInstanceHelper.GetCacheEndBoundary(this);
		}
		protected virtual DateTime GetCacheStartBoundary() {
			return DevExpress.ExpressApp.Kpi.KpiInstanceHelper.GetCacheStartBoundary(this);
		}
		public KpiInstance()
			: base() {
			HistoryItems = new List<KpiHistoryItem>();
			Scorecards = new List<KpiScorecard>();
		}
		[Browsable(false)]
		[InverseProperty("KpiInstances"), Required]
		public virtual KpiDefinition KpiDefinition { get; set; }
		[InverseProperty("KpiInstance"), Aggregated]
		public virtual IList<KpiHistoryItem> HistoryItems { get; set; }
		[Browsable(false), Column("ForceMeasurementDateTime")]
		public DateTime? ForceMeasurementDateTime2 { get; set; }
		[Browsable(false), NotMapped]
		public DateTime ForceMeasurementDateTime {
			get {
				if(ForceMeasurementDateTime2.HasValue) {
					return ForceMeasurementDateTime2.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set {
				ForceMeasurementDateTime2 = value;
			}
		}
		[InverseProperty("Indicators")]
		public virtual IList<KpiScorecard> Scorecards { get; set; }
		[System.ComponentModel.DisplayName("Name")]
		public String KpiName {
			get { return KpiDefinition.Name; }
		}
		public String Period {
			get { return KpiDefinition.Period; }
		}
		public Single Current {
			get {
				try {
					LockData();
					return DevExpress.ExpressApp.Kpi.KpiInstanceHelper.GetValueByRange(this, KpiDefinition.Range, KpiDefinition.MeasurementFrequency);
				}
				finally {
					UnLockData();
				}
			}
		}
		public Single? Previous {
			get {
				try {
					LockData();
					if(KpiDefinition.Compare) {
						return DevExpress.ExpressApp.Kpi.KpiInstanceHelper.GetValueByRange(this, KpiDefinition.RangeToCompare, KpiDefinition.MeasurementFrequency);
					}
					return null;
				}
				finally {
					UnLockData();
				}
			}
		}
		public Single? Change {
			get {
				try {
					LockData();
					if(KpiDefinition.Compare) {
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
					Single delta = Change.Value;
					if(delta > 0) {
						return KpiDefinition.Direction == DevExpress.ExpressApp.Kpi.Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Better : DevExpress.ExpressApp.Editors.Trend.Worse;
					}
					if(delta < 0) {
						return KpiDefinition.Direction == DevExpress.ExpressApp.Kpi.Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Worse : DevExpress.ExpressApp.Editors.Trend.Better;
					}
					return DevExpress.ExpressApp.Editors.Trend.NoChange;
				}
				finally {
					UnLockData();
				}
			}
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
			get { return new DevExpress.ExpressApp.Kpi.KpiGaugeDataSource(this); }
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[Browsable(false)]
		public virtual String Settings { get; set; }
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public void ResetHistoryForInvalidInstance() {
			if(DevExpress.ExpressApp.Kpi.KpiInstanceHelper.AddDuration(KpiDefinition.ChangedOn, KpiDefinition.MeasurementFrequency, false) > ForceMeasurementDateTime) {
				Reset();
			}
		}
		public virtual Single GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd) {
			return GetValueFromHistory(rangeStart, rangeEnd, rangeStart, rangeEnd);
		}
		public virtual IList GetValuesFromHistory(DevExpress.ExpressApp.Kpi.MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd) {
			return new DevExpress.ExpressApp.Kpi.KpiMeasurementHistoryItem[] { new DevExpress.ExpressApp.Kpi.KpiMeasurementHistoryItem(measurementType, rangeEnd, GetValueFromHistory(rangeStart, rangeEnd)) };
		}
		public virtual Single GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			KpiHistoryItem historyItem = (KpiHistoryItem)FindHistoryItem(rangeStart, rangeEnd);
			if(historyItem == null) {
				Single result = KpiDefinition.Evaluate(evaluationStart, evaluationEnd);
				if(DevExpress.ExpressApp.Kpi.KpiModule.EnableSelfRefresh) {
					historyItem = objectSpace.CreateObject<KpiHistoryItem>();
					historyItem.Init(this, result, rangeStart, rangeEnd);
					AddMeasurement(historyItem);
				}
				RefreshKpiChartDataSource();
			}
			return historyItem.Value;
		}
		public virtual IList GetValuesFromHistory(DevExpress.ExpressApp.Kpi.MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			return new DevExpress.ExpressApp.Kpi.KpiMeasurementHistoryItem[] { new DevExpress.ExpressApp.Kpi.KpiMeasurementHistoryItem(measurementType, rangeEnd, GetValueFromHistory(rangeStart, rangeEnd, evaluationStart, evaluationEnd)) };
		}
		public IList<DevExpress.ExpressApp.Kpi.IKpiHistoryItem> GetHistoryItems() {
			List<DevExpress.ExpressApp.Kpi.IKpiHistoryItem> result = new List<DevExpress.ExpressApp.Kpi.IKpiHistoryItem>();
			foreach(KpiHistoryItem item in HistoryItems) {
				result.Add(item);
			}
			return result;
		}
		public DevExpress.ExpressApp.Kpi.IKpiDefinition GetKpiDefinition() {
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
				HistoryItems.Remove(item);
				objectSpace.Delete(item);
			}
		}
		public DevExpress.ExpressApp.Kpi.IKpiHistoryItem FindHistoryItem(DateTime startDate, DateTime endDate) {
			return DevExpress.ExpressApp.Kpi.KpiInstanceHelper.FindHistoryItem(this, startDate, endDate);
		}
		public void Refresh(bool forceMeasurement) {
			bool enableSelfRefresh = DevExpress.ExpressApp.Kpi.KpiModule.EnableSelfRefresh;
			try {
				DevExpress.ExpressApp.Kpi.KpiModule.EnableSelfRefresh = true;
				ResetHistoryForInvalidInstance();
				CleanupCache(forceMeasurement);
				EnsureCache();
			}
			finally {
				DevExpress.ExpressApp.Kpi.KpiModule.EnableSelfRefresh = enableSelfRefresh;
			}
		}
	}
}
