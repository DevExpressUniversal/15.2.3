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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Kpi {
	[DomainComponent]
	[XafDisplayName("KPI")]
	[ImageName("BO_KPI_Definition")]
	public interface IDCKpiInstance : IKpiInstance, IKpiHistoryCacheManager, ISettingsProvider {
		[Browsable(false)]
		new DateTime ForceMeasurementDateTime { get; set; }
		[Browsable(false)]
		IDCKpiDefinition KpiDefinition { get; set; }
		IList<IDCKpiHistoryItem> HistoryItems { get; }
		IList<IDCKpiScorecard> Scorecards { get; }
		[Browsable(false)]
		[Size(SizeAttribute.Unlimited)]
		new string Settings { get; set; }
	}
	[DomainLogic(typeof(IDCKpiInstance))]
	public class DCKpiInstanceLogic {
		private KpiChartDataSource kpiChartDataSource;
		private int getDataLockCount = 0;
		private void EnsureKpiChartDataSource(IDCKpiInstance entity) {
			if(kpiChartDataSource == null) {
				kpiChartDataSource = new KpiChartDataSource(entity);
			}
		}
		private void RefreshKpiChartDataSource() {
			if(kpiChartDataSource != null) {
				kpiChartDataSource.Refresh();
			}
		}
		private void LockData() {
			getDataLockCount++;
		}
		private void UnLockData() {
			getDataLockCount--;
		}
		private static IObjectSpace GetObjectSpace(IDCKpiInstance entity) {
			return XPObjectSpace.FindObjectSpaceByObject(entity);
		}
		private static int SortHistoryItems(IDCKpiHistoryItem item1, IDCKpiHistoryItem item2) {
			if(item1.RangeStart > item2.RangeStart) {
				return 1;
			}
			if(item1.RangeStart < item2.RangeStart) {
				return -1;
			}
			return 0;
		}
		private static void CleanupOutdatedCacheItems(IDCKpiInstance entity) {
			List<IDCKpiHistoryItem> itemsToDelete = new List<IDCKpiHistoryItem>();
			foreach(IDCKpiHistoryItem item in entity.HistoryItems) {
				if(item.RangeStart <= GetCacheStartBoundary(entity)) {
					itemsToDelete.Add(item);
				}
			}
			IObjectSpace objectSpace = GetObjectSpace(entity);
			foreach(IDCKpiHistoryItem item in itemsToDelete) {
				objectSpace.Delete(item);
			}
		}
		private static void ClenupLastMeasurementIfOutdated(IDCKpiInstance entity, bool forceUpdate) {
			if(forceUpdate || entity.ForceMeasurementDateTime <= DateTime.Now) {
				int historyItemsCount = entity.HistoryItems.Count;
				if(historyItemsCount > 0) {
					DateTime rangeEnd = entity.HistoryItems[0].RangeEnd;
					foreach(IDCKpiHistoryItem item in entity.HistoryItems) {
						if(item.RangeEnd > rangeEnd) {
							rangeEnd = item.RangeEnd;
						}
					}
					List<IDCKpiHistoryItem> itemsToDelete = new List<IDCKpiHistoryItem>();
					foreach(IDCKpiHistoryItem item in entity.HistoryItems) {
						if(item.RangeEnd == rangeEnd) {
							itemsToDelete.Add(item);
						}
					}
					IObjectSpace objectSpace = GetObjectSpace(entity);
					foreach(IDCKpiHistoryItem item in itemsToDelete) {
						objectSpace.Delete(item);
					}
				}
			}
		}
		private void EnsureCache(IDCKpiInstance entity) {
			float dummy = entity.Current;
			if(entity.KpiDefinition.Compare) {
				dummy = entity.Previous.Value;
			}
			object obj = entity.ChartDataSource.DataSource;
		}
		private static DateTime GetCacheStartBoundary(IDCKpiInstance entity) {
			return KpiInstanceHelper.GetCacheStartBoundary(entity);
		}
		private static DateTime GetCacheEndBoundary(IDCKpiInstance entity) {
			return KpiInstanceHelper.GetCacheEndBoundary(entity);
		}
		public float GetValueFromHistory(IDCKpiInstance entity, IDateRange range) {
			return GetValueFromHistory(entity, range.StartPoint, range.EndPoint);
		}
		public float GetValueFromHistory(IDCKpiInstance entity, DateTime rangeStart, DateTime rangeEnd) {
			return GetValueFromHistory(entity, rangeStart, rangeEnd, rangeStart, rangeEnd);
		}
		public IList GetValuesFromHistory(IDCKpiInstance entity, MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd) {
			return new KpiMeasurementHistoryItem[] { new KpiMeasurementHistoryItem(measurementType, rangeEnd, entity.GetValueFromHistory(rangeStart, rangeEnd)) };
		}
		public float GetValueFromHistory(IDCKpiInstance entity, DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			IDCKpiHistoryItem historyItem = (IDCKpiHistoryItem)FindHistoryItem(entity, rangeStart, rangeEnd);
			if(historyItem == null) {
				float result = entity.KpiDefinition.Evaluate(evaluationStart, evaluationEnd);
				if(KpiModule.EnableSelfRefresh) {
					historyItem = GetObjectSpace(entity).CreateObject<IDCKpiHistoryItem>();
					historyItem.KpiInstance = entity;
					historyItem.RangeStart = rangeStart;
					historyItem.RangeEnd = rangeEnd;
					historyItem.Value = result;
					AddMeasurement(entity, historyItem);
				}
				RefreshKpiChartDataSource();
			}
			return historyItem.Value;
		}
		public IList GetValuesFromHistory(IDCKpiInstance entity, MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd) {
			return new KpiMeasurementHistoryItem[] { new KpiMeasurementHistoryItem(measurementType, rangeEnd, entity.GetValueFromHistory(rangeStart, rangeEnd, evaluationStart, evaluationEnd)) };
		}
		private void AddMeasurement(IDCKpiInstance entity, IDCKpiHistoryItem kpiHistoryItem) {
			entity.ForceMeasurementDateTime = KpiInstanceHelper.AddDuration(DateTime.Now, entity.KpiDefinition.MeasurementFrequency, false);
			entity.HistoryItems.Add(kpiHistoryItem);
			if(getDataLockCount < 2) { 
				((DevExpress.ExpressApp.DC.ClassGeneration.IPropertyChangeNotificationReceiver)entity).PropertyChanged("ForceMeasurementDateTime", null, null);
			}
		}
		private static float CalculateDelta(IDCKpiInstance entity) {
			if(!entity.Previous.HasValue) {
				return 0;
			}
			if(entity.Previous.Value == entity.Current) {
				return 0;
			}
			float percentResult = 1;
			if(entity.Previous.Value != 0) {
				percentResult = (entity.Current - entity.Previous.Value) / entity.Previous.Value;
			}
			return percentResult;
		}
		public static IKpiDefinition GetKpiDefinition(IDCKpiInstance entity) {
			return entity.KpiDefinition;
		}
		public void CleanupCache(IDCKpiInstance entity, bool forceUpdate) {
			CleanupOutdatedCacheItems(entity);
			ClenupLastMeasurementIfOutdated(entity, forceUpdate);
		}
		public static void Reset(IDCKpiInstance entity) {
			List<IDCKpiHistoryItem> itemsToDelete = new List<IDCKpiHistoryItem>();
			itemsToDelete.AddRange(entity.HistoryItems);
			IObjectSpace objectSpace = GetObjectSpace(entity);
			foreach(IDCKpiHistoryItem item in itemsToDelete) {
				objectSpace.Delete(item);
			}
		}
		public static void ResetHistoryForInvalidInstance(IDCKpiInstance entity) {
			if(KpiInstanceHelper.AddDuration(entity.KpiDefinition.ChangedOn, entity.KpiDefinition.MeasurementFrequency, false) > entity.ForceMeasurementDateTime) {
				Reset(entity);
			}
		}
		public static IKpiHistoryItem FindHistoryItem(IDCKpiInstance entity, DateTime startDate, DateTime endDate) {
			return KpiInstanceHelper.FindHistoryItem(entity, startDate, endDate);
		}
		public void Refresh(IDCKpiInstance entity, bool forceMeasurement) {
			bool enableSelfRefresh = KpiModule.EnableSelfRefresh;
			try {
				KpiModule.EnableSelfRefresh = true;
				if(entity.KpiDefinition.ChangedOn > KpiInstanceHelper.AddDuration(entity.ForceMeasurementDateTime, entity.KpiDefinition.MeasurementFrequency, true)) {
					Reset(entity);
				}
				CleanupCache(entity, forceMeasurement);
				EnsureCache(entity);
			}
			finally {
				KpiModule.EnableSelfRefresh = enableSelfRefresh;
			}
		}
		public static string Get_KpiName(IDCKpiInstance entity) {
			return entity.KpiDefinition.Name;
		}
		public static string Get_Period(IDCKpiInstance entity) {
			return entity.KpiDefinition.Period;
		}
		public float Get_Current(IDCKpiInstance entity) {
			try {
				LockData();
				return KpiInstanceHelper.GetValueByRange(entity, entity.KpiDefinition.Range, entity.KpiDefinition.MeasurementFrequency);
			}
			finally {
				UnLockData();
			}
		}
		public float? Get_Previous(IDCKpiInstance entity) {
			try {
				LockData();
				if(entity.KpiDefinition.Compare) {
					return KpiInstanceHelper.GetValueByRange(entity, entity.KpiDefinition.RangeToCompare, entity.KpiDefinition.MeasurementFrequency);
				}
				return null;
			}
			finally {
				UnLockData();
			}
		}
		public float? Get_Change(IDCKpiInstance entity) {
			try {
				LockData();
				if(entity.KpiDefinition.Compare) {
					return CalculateDelta(entity);
				}
				return null;
			}
			finally {
				UnLockData();
			}
		}
		public Trend? Get_Trend(IDCKpiInstance entity) {
			try {
				LockData();
				if(!entity.Change.HasValue) {
					return null;
				}
				float delta = entity.Change.Value;
				if(delta > 0) return entity.KpiDefinition.Direction == Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Better : DevExpress.ExpressApp.Editors.Trend.Worse;
				if(delta < 0) return entity.KpiDefinition.Direction == Direction.HighIsBetter ? DevExpress.ExpressApp.Editors.Trend.Worse : DevExpress.ExpressApp.Editors.Trend.Better;
				return DevExpress.ExpressApp.Editors.Trend.NoChange;
			}
			finally {
				UnLockData();
			}
		}
		public static IList<IKpiHistoryItem> GetHistoryItems(IDCKpiInstance entity) {
			List<IKpiHistoryItem> result = new List<IKpiHistoryItem>();
			foreach(IDCKpiHistoryItem item in entity.HistoryItems) {
				result.Add(item);
			}
			return result;
		}
		public ISparklineProvider Get_Sparkline(IDCKpiInstance entity) {
			EnsureKpiChartDataSource(entity);
			return kpiChartDataSource;
		}
		public IChartDataSourceProvider Get_ChartDataSource(IDCKpiInstance entity) {
			EnsureKpiChartDataSource(entity);
			return kpiChartDataSource;
		}
	}
}
