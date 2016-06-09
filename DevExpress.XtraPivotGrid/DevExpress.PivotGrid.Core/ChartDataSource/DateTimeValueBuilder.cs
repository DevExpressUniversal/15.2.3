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

using DevExpress.Charts.Native;
using System.Collections.Generic;
using System.Collections;
using System;
using DevExpress.Data.Utils;
using DevExpress.XtraPivotGrid.Utils.DateHelpers;
using DevExpress.PivotGrid.Utils;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid {
	class DateTimeMeasureUnitComparer : IComparer<DateTimeMeasureUnitNative?> {
		readonly static Dictionary<DateTimeMeasureUnitNative, int> orderByMeasureUnit;
		static DateTimeMeasureUnitComparer() {
			orderByMeasureUnit = new Dictionary<DateTimeMeasureUnitNative, int>();
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Millisecond, 0);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Second, 1);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Minute, 2);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Hour, 3);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Day, 4);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Week, 5);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Month, 6);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Quarter, 7);
			orderByMeasureUnit.Add(DateTimeMeasureUnitNative.Year, 8);
		}
		static int GetOrder(DateTimeMeasureUnitNative? unit) {
			return unit.HasValue ? orderByMeasureUnit[unit.Value] : -1;
		}
		#region IComparer<DateTimeMeasureUnitNative> Members
		public int Compare(DateTimeMeasureUnitNative? x, DateTimeMeasureUnitNative? y) {
			return Comparer.Default.Compare(GetOrder(x), GetOrder(y));
		}
		#endregion
	}
	static class DateTimeValueBuilderHelper {
		static Dictionary<PivotGroupInterval, List<PivotGroupInterval>> groupIntervalDeps;
		static Dictionary<PivotGroupInterval, DateTimeMeasureUnitNative?> dateTimeMeasureUnitByGroupInterval;
		static Dictionary<PivotGroupInterval, bool> isDateTimeGroupInterval;
		static Dictionary<PivotGroupInterval, bool> isGroupIntervalWithYear;
		static Dictionary<PivotGroupInterval, Type> valueTypeByGroupInterval;
		static DateTimeValueBuilderHelper() {
			groupIntervalDeps = null;
			dateTimeMeasureUnitByGroupInterval = null;
			isDateTimeGroupInterval = null;
			isGroupIntervalWithYear = null;
			valueTypeByGroupInterval = null;
		}
		public static Dictionary<PivotGroupInterval, List<PivotGroupInterval>> GroupIntervalDeps {
			get {
				if (groupIntervalDeps == null) {
					lock (typeof(DateTimeValueBuilderHelper)) {
						if (groupIntervalDeps == null)
							groupIntervalDeps = CreateGroupIntervalDeps();
					}
				}
				return groupIntervalDeps;
			}
		}
		static Dictionary<PivotGroupInterval, List<PivotGroupInterval>> CreateGroupIntervalDeps() {
			Dictionary<PivotGroupInterval, List<PivotGroupInterval>>  result = new Dictionary<PivotGroupInterval, List<PivotGroupInterval>>();
			result.Add(PivotGroupInterval.Date,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateMonthYear,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateQuarterYear,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateHour,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateHourMinute,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateHourMinuteSecond,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateYear,
				new List<PivotGroupInterval>());
			result.Add(PivotGroupInterval.DateDay,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateMonthYear,
					PivotGroupInterval.DateMonth
				)
			);
			result.Add(PivotGroupInterval.DateDayOfWeek,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateWeekOfYear,
					PivotGroupInterval.DateWeekOfMonth
				)
			);
			result.Add(PivotGroupInterval.DateDayOfYear,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateYear,
					PivotGroupInterval.DateMonthYear,
					PivotGroupInterval.DateQuarterYear
				)
			);
			result.Add(PivotGroupInterval.DateMonth,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateQuarterYear,
					PivotGroupInterval.DateYear
				)
			);
			result.Add(PivotGroupInterval.DateQuarter,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateYear
				)
			);
			result.Add(PivotGroupInterval.DateWeekOfMonth,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateMonthYear,
					PivotGroupInterval.DateMonth
				)
			);
			result.Add(PivotGroupInterval.DateWeekOfYear,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateMonthYear,
					PivotGroupInterval.DateQuarterYear,
					PivotGroupInterval.DateYear
				)
			);
			result.Add(PivotGroupInterval.Hour,
				CreateGroupIntervalSet(
					PivotGroupInterval.Date,
					PivotGroupInterval.DateDay,
					PivotGroupInterval.DateDayOfYear,
					PivotGroupInterval.DateWeekOfYear,
					PivotGroupInterval.DateWeekOfMonth,
					PivotGroupInterval.DateDayOfWeek
				)
			);
			result.Add(PivotGroupInterval.Minute,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateHour,
					PivotGroupInterval.Hour
				)
			);
			result.Add(PivotGroupInterval.Second,
				CreateGroupIntervalSet(
					PivotGroupInterval.DateHourMinute,
					PivotGroupInterval.Minute
				)
			);
			return result;
		}
		static List<PivotGroupInterval> CreateGroupIntervalSet(params PivotGroupInterval[] groupIntervals) {
			List<PivotGroupInterval> set = new List<PivotGroupInterval>();
			foreach(PivotGroupInterval groupInterval in groupIntervals)
				set.Add(groupInterval);
			return set;
		}
		public static Dictionary<PivotGroupInterval, DateTimeMeasureUnitNative?> DateTimeMeasureUnitByGroupInterval {
			get {
				if(dateTimeMeasureUnitByGroupInterval == null)
					CreateDateTimeMeasureUnitByGroupInterval();
				return dateTimeMeasureUnitByGroupInterval;
			}
		}
		static void CreateDateTimeMeasureUnitByGroupInterval() {
			lock (typeof(DateTimeValueBuilderHelper)) {
				if (dateTimeMeasureUnitByGroupInterval != null)
					return;
				Dictionary<PivotGroupInterval, DateTimeMeasureUnitNative?> result = new Dictionary<PivotGroupInterval, DateTimeMeasureUnitNative?>();
				foreach (PivotGroupInterval gi in Helpers.GetEnumValues(typeof(PivotGroupInterval)))
					result[gi] = GetDateTimeMeasureUnit(gi);
				dateTimeMeasureUnitByGroupInterval = result;
			}
		}
		static DateTimeMeasureUnitNative? GetDateTimeMeasureUnit(PivotGroupInterval gI) {
			switch(gI) {
				case PivotGroupInterval.Date:
				case PivotGroupInterval.DateDay:
				case PivotGroupInterval.DateDayOfWeek:
				case PivotGroupInterval.DateDayOfYear:
					return DateTimeMeasureUnitNative.Day;
				case PivotGroupInterval.DateMonth:
				case PivotGroupInterval.DateMonthYear:
					return DateTimeMeasureUnitNative.Month;
				case PivotGroupInterval.DateQuarter:
				case PivotGroupInterval.DateQuarterYear:
					return DateTimeMeasureUnitNative.Quarter;
				case PivotGroupInterval.DateWeekOfMonth:
				case PivotGroupInterval.DateWeekOfYear:
					return DateTimeMeasureUnitNative.Week;
				case PivotGroupInterval.DateYear:
					return DateTimeMeasureUnitNative.Year;
				case PivotGroupInterval.Hour:
				case PivotGroupInterval.DateHour:
					return DateTimeMeasureUnitNative.Hour;
				case PivotGroupInterval.Minute:
				case PivotGroupInterval.DateHourMinute:
					return DateTimeMeasureUnitNative.Minute;
				case PivotGroupInterval.Second:
				case PivotGroupInterval.DateHourMinuteSecond:
					return DateTimeMeasureUnitNative.Second;
				case PivotGroupInterval.Default:
					return new Nullable<DateTimeMeasureUnitNative>();
				default:
					return DateTimeMeasureUnitNative.Year;
			}
		}
		public static Dictionary<PivotGroupInterval, bool> IsDateTimeGroupInterval {
			get {
				if(isDateTimeGroupInterval == null)
					CreateIsDateTimeGroupInterval();
				return isDateTimeGroupInterval;
			}
		}
		static void CreateIsDateTimeGroupInterval() {
			lock (typeof(DateTimeValueBuilderHelper)) {
				if (isDateTimeGroupInterval != null)
					return;
				isDateTimeGroupInterval = new Dictionary<PivotGroupInterval, bool>();
				foreach (PivotGroupInterval gi in Helpers.GetEnumValues(typeof(PivotGroupInterval)))
					isDateTimeGroupInterval[gi] = GroupIntervalHelper.IsDateTimeInterval(gi);
			}
		}
		public static Dictionary<PivotGroupInterval, bool> IsGroupIntervalWithYear {
			get {
				if(isGroupIntervalWithYear == null)
					CreateIsGroupIntervalWithYear();
				return isGroupIntervalWithYear;
			}
		}
		static void CreateIsGroupIntervalWithYear() {
			lock (typeof(DateTimeValueBuilderHelper)) {
				if (isGroupIntervalWithYear != null)
					return;
				isGroupIntervalWithYear = new Dictionary<PivotGroupInterval, bool>();
				foreach (PivotGroupInterval gi in Helpers.GetEnumValues(typeof(PivotGroupInterval)))
					isGroupIntervalWithYear[gi] = GetIsGroupIntervalWithYear(gi);
			}
		}
		static bool GetIsGroupIntervalWithYear(PivotGroupInterval groupInterval) {
			switch(groupInterval) {
				case PivotGroupInterval.DateYear:
				case PivotGroupInterval.Date:
				case PivotGroupInterval.DateMonthYear:
				case PivotGroupInterval.DateQuarterYear:
				case PivotGroupInterval.DateHour:
				case PivotGroupInterval.DateHourMinute:
				case PivotGroupInterval.DateHourMinuteSecond:
					return true;
				default:
					return false;
			}
		}
		public static Dictionary<PivotGroupInterval, Type> ValueTypeByGroupInterval {
			get {
				if(valueTypeByGroupInterval == null)
					CreateValueTypeByGroupInterval();
				return valueTypeByGroupInterval;
			}
		}
		static void CreateValueTypeByGroupInterval() {
			lock (typeof(DateTimeValueBuilderHelper)) {
				if (valueTypeByGroupInterval != null)
					return;
				valueTypeByGroupInterval = new Dictionary<PivotGroupInterval, Type>();
				foreach (PivotGroupInterval gi in Helpers.GetEnumValues(typeof(PivotGroupInterval)))
					valueTypeByGroupInterval[gi] = GroupIntervalHelper.GetValueType(gi);
			}
		}
	}
	class DateTimeValue {
		readonly DateTime value;
		readonly DateTimeMeasureUnitNative? measureUnit;
		public DateTimeValue(DateTime value, DateTimeMeasureUnitNative? measureUnit) {
			this.value = value;
			this.measureUnit = measureUnit;
		}
		public DateTime Value { get { return value; } }
		public DateTimeMeasureUnitNative? MeasureUnit { get { return measureUnit; } }
	}
	class DateTimeValueBuilder {
		public static DateTimeValueBuilder CreateInstance(bool buildMiddleValue) {
			return buildMiddleValue ? new DateTimeMiddleValueBuilder() : new DateTimeValueBuilder();
		}
		int? year, month, day, hour, minute, second, quarter;
		DayOfWeek? dayOfWeek = null;
		int? dayOfYear, weekOfMonth, weekOfYear;
		DateTime? origDateTime;
		List<DateTimeMeasureUnitNative?> measureUnits;
		DateTimeMeasureUnitNative? minMeasureUnit;
		bool isNull;
		public DateTimeValueBuilder() {
			Clear();
		}
		public void Clear() {
			isNull = false;
			Year = null;
			Month = null;
			this.day = null;
			DayOfWeek = null;
			DayOfYear = null;
			WeekOfMonth = null;
			WeekOfYear = null;
			Hour = null;
			Minute = null;
			Second = null;
			Quarter = null;
			OrigDateTime = null;
			MeasureUnits = new List<DateTimeMeasureUnitNative?>();
			this.minMeasureUnit = null;
		}
		protected DateTimeMeasureUnitNative? MinMeasureUnit {
			get {
				if(!minMeasureUnit.HasValue) {
					if(MeasureUnits.Count == 0)
						throw new Exception("No data has been provided to the DateTimeValueBuilder!");
					MeasureUnits.Sort(new DateTimeMeasureUnitComparer());
					minMeasureUnit = MeasureUnits[0];
				}
				return minMeasureUnit;
			}
		}
		public void AddValue(PivotGroupInterval groupInterval, object value) {
			if(value == null) {
				isNull = true;
				return;
			}
			DateTime date;
			switch(groupInterval) {
				case PivotGroupInterval.Default:
					OrigDateTime = (DateTime)value;
					break;
				case PivotGroupInterval.Date:
					date = (DateTime)value;
					Year = date.Year;
					Month = date.Month;
					Day = date.Day;
					break;
				case PivotGroupInterval.DateDay:
					Day = (int)value;
					break;
				case PivotGroupInterval.DateDayOfWeek:
					DayOfWeek = (DayOfWeek)value;
					break;
				case PivotGroupInterval.DateDayOfYear:
					DayOfYear = (int)value;
					break;
				case PivotGroupInterval.DateMonth:
					Month = (int)value;
					break;
				case PivotGroupInterval.DateQuarter:
					Quarter = (int)value;
					break;
				case PivotGroupInterval.DateWeekOfMonth:
					WeekOfMonth = (int)value;
					break;
				case PivotGroupInterval.DateWeekOfYear:
					WeekOfYear = (int)value;
					break;
				case PivotGroupInterval.DateYear:
					Year = (int)value;
					break;
				case PivotGroupInterval.Hour:
					Hour = (int)value;
					break;
				case PivotGroupInterval.Minute:
					Minute = (int)value;
					break;
				case PivotGroupInterval.Second:
					Second = (int)value;
					break;
				case PivotGroupInterval.DateMonthYear:
					date = (DateTime)value;
					Year = date.Year;
					Month = date.Month;
					break;
				case PivotGroupInterval.DateQuarterYear:
					date = (DateTime)value;
					Year = date.Year;
					Quarter = (date.Month - 1) / 3 + 1;
					break;
				case PivotGroupInterval.DateHour:
					date = (DateTime)value;
					Year = date.Year;
					Month = date.Month;
					Day = date.Day;
					Hour = date.Hour;
					break;
				case PivotGroupInterval.DateHourMinute:
					date = (DateTime)value;
					Year = date.Year;
					Month = date.Month;
					Day = date.Day;
					Hour = date.Hour;
					Minute = date.Minute;
					break;
				case PivotGroupInterval.DateHourMinuteSecond:
					date = (DateTime)value;
					Year = date.Year;
					Month = date.Month;
					Day = date.Day;
					Hour = date.Hour;
					Minute = date.Minute;
					Second = date.Second;
					break;
				default:
					break;
			}
			MeasureUnits.Add(DateTimeValueBuilderHelper.DateTimeMeasureUnitByGroupInterval[groupInterval]);
		}
		public DateTimeValue GetValue() {
			if(isNull)
				return null;
			if(OrigDateTime.HasValue) {
				DateTime origDateTime = OrigDateTime.Value;
				return CreateDateTimeValue(ref origDateTime);
			}
			DateTime dateTime = new DateTime(Year.Value, 1, 1);
			if(Month.HasValue && Day.HasValue) {
				dateTime = BuildValueByDay(Year.Value, Month.Value, Day.Value);
			}
			else if(DayOfYear.HasValue) {
				dateTime = BuildValueByDayOfYear(Year.Value, DayOfYear.Value);
			}
			else if(Month.HasValue && WeekOfMonth.HasValue && DayOfWeek.HasValue) {
				dateTime = BuildValueByDayOfWeekAndWeekOfMonth(Year.Value, Month.Value, WeekOfMonth.Value, DayOfWeek.Value);
			}
			else if(WeekOfYear.HasValue && DayOfWeek.HasValue) {
				dateTime = BuildValueByDayOfWeekAndWeekOfYear(Year.Value, WeekOfYear.Value, DayOfWeek.Value);
			}
			else if(Month.HasValue && WeekOfMonth.HasValue) {
				dateTime = BuildValueByWeekOfMonth(Year.Value, Month.Value, WeekOfMonth.Value);
			}
			else if(WeekOfYear.HasValue) {
				dateTime = BuildValueByWeekOfYear(Year.Value, WeekOfYear.Value);
			}
			else if(Month.HasValue) {
				dateTime = BuildValueByMonth(Year.Value, Month.Value, dateTime.Day);
			}
			else if(Quarter.HasValue) {
				dateTime = BuildValueByMonth(Year.Value, (Quarter.Value - 1) * 3 + 1, dateTime.Day);
			}
			if(Hour.HasValue) {
				dateTime = BuildValueByHour(dateTime.Year, dateTime.Month, dateTime.Day, Hour.Value);
				if(Minute.HasValue) {
					dateTime = BuildValueByMinute(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, Minute.Value);
					if(Second.HasValue)
						dateTime = BuildValueBySecond(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, Second.Value);
				}
			}
			return CreateDateTimeValue(ref dateTime);
		}
		protected virtual DateTimeValue CreateDateTimeValue(ref DateTime dateTime) {
			return new DateTimeValue(dateTime, MinMeasureUnit);
		}
		DateTime BuildValueByWeekOfYear(int year, int weekOfYear) {
			return DateHelper.GetDateFromWeekOfYear(year, weekOfYear);
		}
		DateTime BuildValueByDayOfWeekAndWeekOfYear(int year, int weekOfYear, DayOfWeek dayOfWeek) {
			return DateHelper.GetDateFromDayOfWeekAndWeekOfYear(year, weekOfYear, dayOfWeek);
		}
		DateTime BuildValueByWeekOfMonth(int year, int month, int weekOfMonth) {
			return DateHelper.GetDateFromWeekOfMonth(year, month, weekOfMonth);
		}
		DateTime BuildValueByDayOfWeekAndWeekOfMonth(int year, int month, int weekOfMonth, DayOfWeek dayOfWeek) {
			return DateHelper.GetDateFromDayOfWeekAndWeekOfMonth(year, month, weekOfMonth, dayOfWeek);
		}
		DateTime BuildValueByDayOfYear(int year, int dayOfYear) {
			return DateHelper.GetDateFromDayOfYear(year, dayOfYear);
		}
		DateTime BuildValueByMonth(int year, int month, int day) {
			return CreateDateTime(year, month, day);
		}
		DateTime BuildValueByDay(int year, int month, int day) {
			return CreateDateTime(year, month, day);
		}
		DateTime BuildValueByHour(int year, int month, int day, int hour) {
			DateTime dateTime = CreateDateTime(year, month, day);
			return dateTime.AddHours(hour);
		}
		DateTime BuildValueByMinute(int year, int month, int day, int hour, int minute) {
			DateTime dateTime = BuildValueByHour(year, month, day, hour);
			return dateTime.AddMinutes(minute);
		}
		DateTime BuildValueBySecond(int year, int month, int day, int hour, int minute, int second) {
			DateTime dateTime = BuildValueByMinute(year, month, day, hour, minute);
			return dateTime.AddSeconds(second);
		}
		DateTime CreateDateTime(int year, int month, int day) {
			return new DateTime(year, month, day);
		}
		int? Year {
			get { return year; }
			set { year = value; }
		}
		int? Month {
			get { return month; }
			set { month = value; }
		}
		int? Day {
			get { return day; }
			set { day = value; }
		}
		int? Hour {
			get { return hour; }
			set { hour = value; }
		}
		int? Minute {
			get { return minute; }
			set { minute = value; }
		}
		int? Second {
			get { return second; }
			set { second = value; }
		}
		int? Quarter {
			get { return quarter; }
			set { quarter = value; }
		}
		DayOfWeek? DayOfWeek {
			get { return dayOfWeek; }
			set { dayOfWeek = value; }
		}
		int? DayOfYear {
			get { return dayOfYear; }
			set { dayOfYear = value; }
		}
		int? WeekOfMonth {
			get { return weekOfMonth; }
			set { weekOfMonth = value; }
		}
		int? WeekOfYear {
			get { return weekOfYear; }
			set { weekOfYear = value; }
		}
		DateTime? OrigDateTime {
			get { return origDateTime; }
			set { origDateTime = value; }
		}
		List<DateTimeMeasureUnitNative?> MeasureUnits {
			get { return measureUnits; }
			set { measureUnits = value; }
		}
	}
	class DateTimeMiddleValueBuilder : DateTimeValueBuilder {
		protected override DateTimeValue CreateDateTimeValue(ref DateTime dateTime) {
			DateTime correctedDateTime = CorrectValue(ref dateTime);
			return base.CreateDateTimeValue(ref correctedDateTime);
		}
		DateTime CorrectValue(ref DateTime dateTime) {
			DateTimeMeasureUnitNative? measureUnit = MinMeasureUnit;
			if(!measureUnit.HasValue)
				return dateTime;
			switch(MinMeasureUnit) {
				case DateTimeMeasureUnitNative.Day:
					return SetMiddleDay(ref dateTime);
				case DateTimeMeasureUnitNative.Month:
					return SetMiddleMonth(ref dateTime);
				case DateTimeMeasureUnitNative.Quarter:
					return SetMiddleQuarter(ref dateTime);
				case DateTimeMeasureUnitNative.Week:
					return SetMiddleWeek(ref dateTime);
				case DateTimeMeasureUnitNative.Year:
					return SetMiddleYear(ref dateTime);
				case DateTimeMeasureUnitNative.Hour:
					return SetMiddleHour(ref dateTime);
				case DateTimeMeasureUnitNative.Minute:
					return SetMiddleMinute(ref dateTime);
				case DateTimeMeasureUnitNative.Second:
					return SetMiddleSecond(ref dateTime);
				case DateTimeMeasureUnitNative.Millisecond:
					return dateTime;
				default:
					throw new Exception("Unexpected MinMeasureUnit");
			}
		}
		DateTime SetMiddleDay(ref DateTime dateTime) {
			return dateTime.AddHours(12);
		}
		DateTime SetMiddleMonth(ref DateTime dateTime) {
			return dateTime.AddDays(15);
		}
		DateTime SetMiddleQuarter(ref DateTime dateTime) {
			return dateTime.AddDays(45);
		}
		DateTime SetMiddleWeek(ref DateTime dateTime) {
			return dateTime.AddDays(3);
		}
		DateTime SetMiddleYear(ref DateTime dateTime) {
			return dateTime.AddMonths(6);
		}
		DateTime SetMiddleHour(ref DateTime dateTime) {
			return dateTime.AddMinutes(30);
		}
		DateTime SetMiddleMinute(ref DateTime dateTime) {
			return dateTime.AddSeconds(30);
		}
		DateTime SetMiddleSecond(ref DateTime dateTime) {
			return dateTime.AddMilliseconds(500);
		}
	}
}
