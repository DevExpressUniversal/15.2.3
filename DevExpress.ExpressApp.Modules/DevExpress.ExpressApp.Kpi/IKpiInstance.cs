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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Kpi {
	[Appearance("KpiTrendBetter", TargetItems = "Trend, Change", FontColor = "Green", Criteria = "Trend = 'Better'")]
	[Appearance("KpiTrendWorse", TargetItems = "Trend, Change", FontColor = "Red", Criteria = "Trend = 'Worse'")]
	[Appearance("KpiRedZone", TargetItems = "*", BackColor = "Pink", Criteria = "(KpiDefinition.RedZone <> 0) And ((KpiDefinition.Direction = 'HighIsBetter' And Current <= KpiDefinition.RedZone) Or (KpiDefinition.Direction = 'LowIsBetter' And Current >= KpiDefinition.RedZone))", Context = "ListView")]
	[CreatableItem(false)]
	public interface IKpiInstance {
		[Browsable(false)]
		DateTime ForceMeasurementDateTime { get; set; }
		IKpiDefinition GetKpiDefinition();
		IList<IKpiHistoryItem> GetHistoryItems();
		void Refresh(bool forceMeasurement);
		[System.ComponentModel.DisplayName("Name")]
		string KpiName { get; }
		string Period { get; }
		float Current { get; }
		float? Previous { get; }
		float? Change { get; }
		Trend? Trend { get; }
		[VisibleInDetailView(false), VisibleInLookupListView(false)]
		ISparklineProvider Sparkline { get; }
		[VisibleInListView(false), VisibleInLookupListView(false)]
		IChartDataSourceProvider ChartDataSource { get; }
	}
	public interface IKpiHistoryCacheManager {
		IKpiHistoryItem FindHistoryItem(DateTime startDate, DateTime endDate);
		float GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd);
		IList GetValuesFromHistory(MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd);
		float GetValueFromHistory(DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd);
		IList GetValuesFromHistory(MeasurementType measurementType, DateTime rangeStart, DateTime rangeEnd, DateTime evaluationStart, DateTime evaluationEnd);
		void ResetHistoryForInvalidInstance();
		void CleanupCache(bool forceUpdate);
		void Reset();
	}
	public class KpiInstanceDeactivateNewDeleteViewController : ObjectViewController {
		protected override void OnActivated() {
			base.OnActivated();
			Frame.GetController<NewObjectViewController>().Active["AllowCreateKpiInstance"] = false;
			Frame.GetController<DeleteObjectsViewController>().Active["AllowDeleteKpiInstance"] = false;
		}
		protected override void OnDeactivated() {
			Frame.GetController<NewObjectViewController>().Active.RemoveItem("AllowCreateKpiInstance");
			Frame.GetController<DeleteObjectsViewController>().Active.RemoveItem("AllowDeleteKpiInstance");
			base.OnDeactivated();
		}
		public KpiInstanceDeactivateNewDeleteViewController() {
			TargetObjectType = typeof(IKpiInstance);
		}
	}
	public static class KpiInstanceHelper {
		public static DateTime AddDuration(DateTime dateTime, TimeIntervalType measurementFrequency, bool backward) {
			if(dateTime.Year == DateTime.MinValue.Year || dateTime.Year == DateTime.MaxValue.Year) {
				return dateTime;
			}
			int direction = 1;
			if(backward) {
				direction = -1;
			}
			switch(measurementFrequency) {
				case TimeIntervalType.Hour:
					return dateTime.AddHours(direction * 1);
				case TimeIntervalType.Day:
					return dateTime.AddDays(direction * 1);
				case TimeIntervalType.Week:
					return dateTime.AddDays(direction * 7);
				case TimeIntervalType.Month:
					return dateTime.AddMonths(direction * 1);
				case TimeIntervalType.Quarter:
					if(dateTime.Day == DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) {
						return dateTime.AddDays(1).AddMonths(direction * 3).AddDays(-1);
					}
					return dateTime.AddMonths(direction * 3);
				case TimeIntervalType.Year:
					return dateTime.AddYears(direction * 1);
			}
			return dateTime;
		}
		public static DateTime GetCacheStartBoundary(IKpiInstance entity) {
			IDateRange range = null;
			IKpiDefinition kpiDefinition = entity.GetKpiDefinition();
			if(!kpiDefinition.Compare) {
				range = kpiDefinition.Range;
			} else {
				range = kpiDefinition.Range.StartPoint > kpiDefinition.RangeToCompare.StartPoint ? kpiDefinition.RangeToCompare : kpiDefinition.Range;
			}
			return AddDuration(range.StartPoint, kpiDefinition.MeasurementFrequency, true);
		}
		public static DateTime GetCacheEndBoundary(IKpiInstance entity) {
			IDateRange range = null;
			IKpiDefinition kpiDefinition = entity.GetKpiDefinition();
			if(!kpiDefinition.Compare) {
				range = kpiDefinition.Range;
			}
			else {
				range = kpiDefinition.Range.EndPoint > kpiDefinition.RangeToCompare.EndPoint ? kpiDefinition.Range : kpiDefinition.RangeToCompare;
			}
			return AddDuration(range.EndPoint, kpiDefinition.MeasurementFrequency, true);
		}
		public static float GetValueByRange(IKpiHistoryCacheManager kpiInstance, IDateRange range, TimeIntervalType measurementFrequency) {
			kpiInstance.ResetHistoryForInvalidInstance();
			DateTime intervalStart = range.StartPoint;
			DateTime intervalEnd = AddDuration(range.StartPoint, measurementFrequency, false);
			while(intervalEnd < range.EndPoint) {
				intervalEnd = AddDuration(intervalEnd, measurementFrequency, false);
			}
			intervalEnd = intervalEnd.AddSeconds(-1);
			if((range.EndPoint - range.StartPoint) < new TimeSpan(0, 0, 1)) {
				intervalStart = intervalEnd;
			}
			return kpiInstance.GetValueFromHistory(intervalStart, intervalEnd, range.StartPoint, range.EndPoint);
		}
		public static IKpiHistoryItem FindHistoryItem(IKpiInstance kpiInstance, DateTime startDate, DateTime endDate) {
			bool isNowRange = (endDate - startDate) < new TimeSpan(0, 0, 1);
			foreach(IKpiHistoryItem item in kpiInstance.GetHistoryItems()) {
				if(isNowRange) {
					if(item.RangeStart >= GetCacheEndBoundary(kpiInstance)
						&& item.RangeEnd >= GetCacheEndBoundary(kpiInstance)) {
						return item;
					}
				} else {
					if(item.RangeStart >= startDate.AddSeconds(-1) && item.RangeStart <= startDate.AddSeconds(1)
						&& item.RangeEnd >= endDate.AddSeconds(-1) && item.RangeEnd <= endDate.AddSeconds(1)) {
						return item;
					}
				}
			}
			return null;
		}
	}
}
