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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Kpi {
	public class DateRangeRepository {
		private static Dictionary<string, IDateRange> registeredRanges = new Dictionary<string, IDateRange>();
		static DateRangeRepository() {
			Reset();
		}
		public static void Reset() {
			registeredRanges.Clear();
			Now = new DateRange("Now", new RangePoint(DateBase.Now), new RangePoint(DateBase.Now));
			RegisterRange(Now);
			Today = new DateRange("Today", new RangePoint(DateBase.DayStart), new RangePoint(DateBase.DayEnd));
			RegisterRange(Today);
			Yesterday = new DateRange("Yesterday", new RangePoint(DateBase.DayStart, -1, TimeIntervalType.Day), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Day));
			RegisterRange(Yesterday);
			DayWeekAgo = new DateRange("DayWeekAgo", new RangePoint(DateBase.DayStart, -1, TimeIntervalType.Week), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Week));
			RegisterRange(DayWeekAgo);
			DayMonthAgo = new DateRange("DayMonthAgo", new RangePoint(DateBase.DayStart, -1, TimeIntervalType.Month), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Month));
			RegisterRange(DayMonthAgo);
			DayYearAgo = new DateRange("DayYearAgo", new RangePoint(DateBase.DayStart, -1, TimeIntervalType.Year), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Year));
			RegisterRange(DayYearAgo);
			ThisWeekToDate = new DateRange("ThisWeekToDate", new RangePoint(DateBase.WeekStart), new RangePoint(DateBase.DayEnd));
			RegisterRange(ThisWeekToDate);
			LastWeekToDate = new DateRange("LastWeekToDate", new RangePoint(DateBase.WeekStart, -1, TimeIntervalType.Week), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Week));
			RegisterRange(LastWeekToDate);
			ThisMonthToDate = new DateRange("ThisMonthToDate", new RangePoint(DateBase.MonthStart), new RangePoint(DateBase.DayEnd));
			RegisterRange(ThisMonthToDate);
			LastMonthToDate = new DateRange("LastMonthToDate", new RangePoint(DateBase.MonthStart, -1, TimeIntervalType.Month), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Month));
			RegisterRange(LastMonthToDate);
			ThisQuarterToDate = new DateRange("ThisQuarterToDate", new RangePoint(DateBase.QuarterStart), new RangePoint(DateBase.DayEnd));
			RegisterRange(ThisQuarterToDate);
			LastQuarterToDate = new DateRange("LastQuarterToDate", new RangePoint(DateBase.QuarterStart, -1, TimeIntervalType.Quarter), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Quarter));
			RegisterRange(LastQuarterToDate);
			ThisYearToDate = new DateRange("ThisYearToDate", new RangePoint(DateBase.YearStart), new RangePoint(DateBase.DayEnd));
			RegisterRange(ThisYearToDate);
			LastYearToDate = new DateRange("LastYearToDate", new RangePoint(DateBase.YearStart, -1, TimeIntervalType.Year), new RangePoint(DateBase.DayEnd, -1, TimeIntervalType.Year));
			RegisterRange(LastYearToDate);
			LastThreeDays = new DateRange("LastThreeDays", new RangePoint(DateBase.DayStart, -2, TimeIntervalType.Day), new RangePoint(DateBase.DayEnd));
			RegisterRange(LastThreeDays);
			LastTenDays = new DateRange("LastTenDays", new RangePoint(DateBase.DayStart, -9, TimeIntervalType.Day), new RangePoint(DateBase.DayEnd));
			RegisterRange(LastTenDays);
			LastThirtyDays = new DateRange("LastThirtyDays", new RangePoint(DateBase.DayStart, -29, TimeIntervalType.Day), new RangePoint(DateBase.DayEnd));
			RegisterRange(LastThirtyDays);
		}
		public static IDateRange Now { get; private set; }
		public static IDateRange Today { get; private set; }
		public static IDateRange Yesterday { get; private set; }
		public static IDateRange DayWeekAgo { get; private set; }
		public static IDateRange DayMonthAgo { get; private set; }
		public static IDateRange DayYearAgo { get; private set; }
		public static IDateRange ThisWeekToDate { get; private set; }
		public static IDateRange LastWeekToDate { get; private set; }
		public static IDateRange ThisMonthToDate { get; private set; }
		public static IDateRange LastMonthToDate { get; private set; }
		public static IDateRange ThisQuarterToDate { get; private set; }
		public static IDateRange LastQuarterToDate { get; private set; }
		public static IDateRange ThisYearToDate { get; private set; }
		public static IDateRange LastYearToDate { get; private set; }
		public static IDateRange LastThreeDays { get; private set; }
		public static IDateRange LastTenDays { get; private set; }
		public static IDateRange LastThirtyDays { get; private set; }
		public static void RegisterRange(IDateRange dateRange) {
			IDateRange registeredRange = null;
			registeredRanges.TryGetValue(dateRange.Name, out registeredRange);
			if(registeredRange != null) {
					Tracing.Tracer.LogWarning("The '{0}' date range is replaced.", dateRange.Name);
			}
			registeredRanges[dateRange.Name] = dateRange;
		}
		public static IDateRange FindRange(string rangeName) {
			if(string.IsNullOrEmpty(rangeName)) {
				return null;
			}
			IDateRange range;
			if(registeredRanges.TryGetValue(rangeName, out range)) {
				return range;
			}
			else {
				return null;
			}
		}
		public static ReadOnlyCollection<string> GetRegisteredRangeNames() {
			return new ReadOnlyCollection<string>(new List<string>(registeredRanges.Keys));
		}
		public static ReadOnlyCollection<IDateRange> GetRegisteredRanges() {
			List<IDateRange> result = new List<IDateRange>();
			foreach(IDateRange value in registeredRanges.Values) {
				result.Add(value);
			}
			return new ReadOnlyCollection<IDateRange>(result);
		}
	}
}
