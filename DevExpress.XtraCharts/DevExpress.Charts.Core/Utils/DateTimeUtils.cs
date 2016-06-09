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
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils;
using System.Text.RegularExpressions;
namespace DevExpress.Charts.Native {
	[Flags]
	public enum WeekdayCore {
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesdey = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64
	}
	public interface IKnownDate {
		DateTime Date { get; }
	}
	public interface IWorkdaysOptions {
		bool WorkdaysOnly { get; }
		DayOfWeek FirstDayOfWeek { get; }
		WeekdayCore Workdays { get; }
		IEnumerable<DateTime> Holidays { get; }
		IEnumerable<DateTime> ExactWorkdays { get; }
		IEnumerable<CustomDate> CustomDates { get; }
	}
	public struct CustomDate {
		readonly bool isHoliday;
		readonly DateTime date;
		public bool IsHoliday {
			get {
				return isHoliday;
			}
		}
		public DateTime Date {
			get {
				return date;
			}
		}
		public CustomDate(bool isHoliday, DateTime date) {
			this.isHoliday = isHoliday;
			this.date = date;
		}
	}
	public class CustomDateCollection {
		class CustomDateComparer : IComparer<CustomDate> {
			public int Compare(CustomDate x, CustomDate y) {
				return x.Date.CompareTo(y.Date);
			}
		}
		readonly List<CustomDate> dates;
		readonly CustomDateComparer comparer;
		public IEnumerable<CustomDate> Dates {
			get {
				return dates;
			}
		}
		public CustomDateCollection() {
			this.dates = new List<CustomDate>();
			this.comparer = new CustomDateComparer();
		}
		public void Clear() {
			dates.Clear();
		}
		public void SetDate(DateTime date, bool isHoliday) {
			CustomDate customDate = new CustomDate(isHoliday, date);
			int index = dates.BinarySearch(customDate, comparer);
			if (index < 0) {
				index = ~index;
				dates.Insert(index, customDate);
			} else {
				CustomDate existingDate = dates[index];
				if (existingDate.IsHoliday)
					dates[index] = customDate;
			}
		}
		public void ResetDate(DateTime date) {
			CustomDate customDate = new CustomDate(false, date);
			int index = dates.BinarySearch(customDate, comparer);
			if (index > 0)
				dates.RemoveAt(index);
		}
	}
	public static class DateTimeUtils {
		const int DaysInWeek = 7;
		const int MonthsInYear = MonthsInQuarter * 4;
		const int HoursInDay = 24;
		const int MinutesInDay = HoursInDay * 60;
		const int SecondsInDay = MinutesInDay * 60;
		const int MillisecondsInDay = SecondsInDay * 1000;
		const double TicksInDay = MillisecondsInDay * 10000.0;
		public const int MonthsInQuarter = 3;
		static DateTime Add(DateTime dateTime, DateTimeMeasureUnitNative measureUnit, bool isNegative) {
			int sign = isNegative ? -1 : 1;
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					return dateTime.AddYears(sign);
				case DateTimeMeasureUnitNative.Quarter:
					return dateTime.AddMonths(sign * MonthsInQuarter);
				case DateTimeMeasureUnitNative.Month:
					return dateTime.AddMonths(sign);
				case DateTimeMeasureUnitNative.Week:
					return dateTime.AddDays(sign * DaysInWeek);
				case DateTimeMeasureUnitNative.Day:
					return dateTime.AddDays(sign);
				case DateTimeMeasureUnitNative.Hour:
					return dateTime.AddHours(sign);
				case DateTimeMeasureUnitNative.Minute:
					return dateTime.AddMinutes(sign);
				case DateTimeMeasureUnitNative.Second:
					return dateTime.AddSeconds(sign);
				case DateTimeMeasureUnitNative.Millisecond:
					return dateTime.AddMilliseconds(sign);
				default:
					ChartDebug.Fail("Unkown DateTimeMeasureUnit");
					return dateTime;
			}
		}
		static int CalcHolidaysCountInWeek(IWorkdaysOptions workdaysOptions) {
			int result = 0;
			WeekdayCore workdays = workdaysOptions.Workdays;
			if ((workdays & WeekdayCore.Sunday) == 0)
				result++;
			if ((workdays & WeekdayCore.Monday) == 0)
				result++;
			if ((workdays & WeekdayCore.Tuesday) == 0)
				result++;
			if ((workdays & WeekdayCore.Wednesdey) == 0)
				result++;
			if ((workdays & WeekdayCore.Thursday) == 0)
				result++;
			if ((workdays & WeekdayCore.Friday) == 0)
				result++;
			if ((workdays & WeekdayCore.Saturday) == 0)
				result++;
			return result;
		}
		static double TotalUnits(DateTime dateTime, bool workdaysOnly, IWorkdaysOptions wordkaysOptions, int majorUnit) {
			DateTime monthRounded = Floor(dateTime, DateTimeMeasureUnitNative.Month);
			DateTime dayRounded = Floor(dateTime, DateTimeMeasureUnitNative.Day);
			int daysInMonth = (monthRounded.AddMonths(1) - monthRounded).Days;
			double monthAddition;
			if (workdaysOnly) {
				int dayCount = 0;
				int actualDaysInMonth = 0;
				DateTime currentDate = monthRounded;
				for (int i = 0; i < daysInMonth; i++) {
					if (!IsHoliday(wordkaysOptions, currentDate, true, true)) {
						actualDaysInMonth++;
						if (currentDate < dayRounded)
							dayCount++;
					}
					currentDate = currentDate.AddDays(1);
				}
				monthAddition = actualDaysInMonth == 0 ? 0 : ((double)dayCount / actualDaysInMonth);
				daysInMonth = actualDaysInMonth;
			} else
				monthAddition = (double)(dayRounded - monthRounded).Days / daysInMonth;
			double divider = TicksInDay * daysInMonth;
			double ticks = daysInMonth == 0 ? 0 : (double)(dateTime.Ticks - dayRounded.Ticks) / divider;
			return (monthRounded.Year * MonthsInYear + monthRounded.Month - 1 + monthAddition + ticks) / majorUnit;
		}
		static double CalcWeekAddition(IWorkdaysOptions workdaysOptions, DateTime date, DateTime roundedToWeek) {
			DateTime roundedToDay = Floor(date, DateTimeMeasureUnitNative.Day);
			int weekDaysCount = 0;
			int spentDaysCount = 0;
			DateTime tempDate = roundedToWeek;
			for (int i = 0; i < 7; i++) {
				if (!IsHoliday(workdaysOptions, tempDate, true, true)) {
					weekDaysCount++;
					if (tempDate < roundedToDay)
						spentDaysCount++;
				}
				tempDate = tempDate.AddDays(1);
			}
			return weekDaysCount == 0 ? 0 : (spentDaysCount + (date - roundedToDay).TotalDays) / weekDaysCount;
		}
		static double CorrectDifferenceBasedOnHolidays(IWorkdaysOptions workdaysOptions, DateTime fromDate, double value, int multiplier) {
			int holidaysCount = CalcHolidaysCountInWeek(workdaysOptions);
			if (holidaysCount == DaysInWeek)
				return value;
			DateTime actualDate = fromDate.Date;
			int totalDays = (int)Math.Floor((value + (fromDate - actualDate).TotalDays * multiplier) / multiplier);
			int fullWeeks = totalDays / DaysInWeek;
			value -= fullWeeks * CalcHolidaysCountInWeek(workdaysOptions) * multiplier;
			int remainWeekDays = totalDays % DaysInWeek;
			if (remainWeekDays > 0)
				for (int i = 0; i < remainWeekDays; i++) {
					if (IsHoliday(workdaysOptions, actualDate, false, false))
						value -= multiplier;
					actualDate = actualDate.AddDays(1);
				} else if (remainWeekDays < 0)
				for (int i = 0; i > remainWeekDays; i--) {
					if (IsHoliday(workdaysOptions, actualDate, false, false))
						value += multiplier;
					actualDate = actualDate.AddDays(-1);
				}
			actualDate = fromDate.Date;
			DateTime finishDate = actualDate.AddDays(totalDays);
			if (finishDate < actualDate) {
				foreach (DateTime holiday in workdaysOptions.Holidays)
					if (holiday <= actualDate && holiday > finishDate && !IsHoliday(workdaysOptions, holiday, false, false))
						value += multiplier;
				foreach (DateTime workday in workdaysOptions.ExactWorkdays)
					if (workday <= actualDate && workday > finishDate && IsHoliday(workdaysOptions, workday, true, false))
						value -= multiplier;
			} else {
				foreach (DateTime holiday in workdaysOptions.Holidays)
					if (holiday >= actualDate && holiday < finishDate && !IsHoliday(workdaysOptions, holiday, false, false))
						value -= multiplier;
				foreach (DateTime workday in workdaysOptions.ExactWorkdays)
					if (workday >= actualDate && workday < finishDate && IsHoliday(workdaysOptions, workday, true, false))
						value += multiplier;
			}
			return value;
		}
		static DateTime AddUnits(DateTimeMeasureUnitNative measureUnit, DateTime date, double range, double factor, IWorkdaysOptions workdaysOptions) {
			bool workdaysOnly = (workdaysOptions != null) ? workdaysOptions.WorkdaysOnly : false;
			range *= factor;
			DateTime monthRounded = Floor(date, DateTimeMeasureUnitNative.Month);
			range += GetDifference(measureUnit, DateTimeMeasureUnitNative.Month, monthRounded, date, workdaysOptions);
			int integerRange = (int)Math.Floor(range);
			double remain = range - integerRange;
			DateTime result = monthRounded.AddMonths(integerRange);
			int daysInMonth = (result.AddMonths(1) - result).Days;
			if (!workdaysOnly)
				return result.AddDays(daysInMonth * remain);
			int dayCount = 0;
			DateTime currentDate = result;
			for (int i = 0; i < daysInMonth; i++) {
				if (!IsHoliday(workdaysOptions, currentDate, true, true))
					dayCount++;
				currentDate = currentDate.AddDays(1);
			}
			remain *= dayCount;
			while (remain >= 0.5) {
				if (!IsHoliday(workdaysOptions, result, true, true))
					remain -= 1;
				result = result.AddDays(1);
			}
			if (remain > 0)
				while (IsHoliday(workdaysOptions, result, true, true))
					result = result.AddDays(1);
			return result.AddDays(remain);
		}
		static double CorrectRangeBasedOnHolidays(IWorkdaysOptions workdaysOptions, DateTime date, double range, int multiplier) {
			if (range == 0)
				return range;
			int holidaysCount = CalcHolidaysCountInWeek(workdaysOptions);
			if (holidaysCount == DaysInWeek)
				return range;
			int workdaysCount = DaysInWeek - holidaysCount;
			DateTime startDate = date.Date;
			DateTime actualDate = startDate;
			int dayRange = (int)Math.Floor((range + (date - actualDate).TotalDays * multiplier) / multiplier);
			double remain = range - (dayRange * multiplier);
			int fullWeeks = dayRange / workdaysCount;
			int remainWeekDays = dayRange % workdaysCount;
			if (dayRange > 0) {
				for (; remainWeekDays > 0; ) {
					if (IsHoliday(workdaysOptions, actualDate, false, false))
						dayRange++;
					else
						remainWeekDays--;
					actualDate = actualDate.AddDays(1);
				}
				dayRange += fullWeeks * holidaysCount;
				actualDate = startDate.AddDays(dayRange);
				while (IsHoliday(workdaysOptions, actualDate, false, false)) {
					dayRange++;
					actualDate = actualDate.AddDays(1);
				}
				foreach (DateTime holiday in workdaysOptions.Holidays)
					if (holiday >= startDate && holiday <= actualDate && !IsHoliday(workdaysOptions, holiday, false, false))
						do {
							dayRange++;
							actualDate = actualDate.AddDays(1);
						} while (IsHoliday(workdaysOptions, actualDate, false, false));
				foreach (DateTime workday in workdaysOptions.ExactWorkdays)
					if (workday >= startDate && workday <= actualDate && IsHoliday(workdaysOptions, workday, true, false))
						do {
							dayRange--;
							actualDate = actualDate.AddDays(-1);
						} while (IsHoliday(workdaysOptions, actualDate, true, true));
				range = dayRange * multiplier + remain;
			} else {
				for (; remainWeekDays < 0; ) {
					if (IsHoliday(workdaysOptions, actualDate, false, false))
						dayRange--;
					else
						remainWeekDays++;
					actualDate = actualDate.AddDays(-1);
				}
				dayRange += fullWeeks * holidaysCount;
				actualDate = startDate.AddDays(dayRange);
				while (IsHoliday(workdaysOptions, actualDate, false, false)) {
					dayRange--;
					actualDate = actualDate.AddDays(-1);
				}
				List<DateTime> holidays = new List<DateTime>();
				foreach (DateTime holiday in workdaysOptions.Holidays)
					holidays.Insert(0, holiday);
				foreach (DateTime holiday in holidays)
					if (holiday >= actualDate && holiday <= startDate && !IsHoliday(workdaysOptions, holiday, false, false))
						do {
							dayRange--;
							actualDate = actualDate.AddDays(-1);
						} while (IsHoliday(workdaysOptions, actualDate, false, false));
				foreach (DateTime workday in workdaysOptions.ExactWorkdays)
					if (workday >= actualDate && workday <= startDate && IsHoliday(workdaysOptions, workday, true, false))
						do {
							dayRange++;
							actualDate = actualDate.AddDays(1);
						} while (IsHoliday(workdaysOptions, actualDate, true, true));
				range = dayRange * multiplier + remain;
			}
			return range;
		}
		public static DateTime Round(DateTime dateTime, DateTimeMeasureUnitNative measureUnit) {
			int half = dateTime.Month == 2 ? 14 : 15;
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					if (dateTime.Month > 6)
						dateTime = dateTime.AddYears(1);
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnitNative.Month:
					if (dateTime.Day > half)
						dateTime = dateTime.AddMonths(1);
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnitNative.Day:
					if (dateTime.Hour > 12 || (dateTime.Hour == 12 && (dateTime.Minute > 0 || dateTime.Second > 0 || dateTime.Millisecond > 0)))
						dateTime = dateTime.AddDays(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnitNative.Hour:
					if (dateTime.Minute >= 30)
						dateTime = dateTime.AddHours(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnitNative.Minute:
					if (dateTime.Second >= 30)
						dateTime = dateTime.AddMinutes(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnitNative.Second:
					if (dateTime.Millisecond >= 500)
						dateTime = dateTime.AddSeconds(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnitNative.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnitNative.Week: {
						if (dateTime.Hour >= 12)
							dateTime = dateTime.AddDays(1);
						int offset = (int)dateTime.DayOfWeek - (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
						DateTime rounded = dateTime.AddDays(-1 * (offset >= 0 ? offset : offset + DaysInWeek));
						return new DateTime(rounded.Year, rounded.Month, rounded.Day);
					}
				case DateTimeMeasureUnitNative.Quarter: {
						if (dateTime.Day > half)
							dateTime = dateTime.AddMonths(1);
						DateTime rounded = dateTime.AddMonths(-1 * ((dateTime.Month - 1) % MonthsInQuarter));
						return new DateTime(rounded.Year, rounded.Month, 1);
					}
				default:
					ChartDebug.Fail("Invalid MeasureUnit.");
					return dateTime;
			}
		}
		public static bool IsHoliday(IWorkdaysOptions workdaysOptions, DateTime dateTime, bool applyHolidays, bool applyExactWorkdays) {
			bool isHoliday = false;
			DayOfWeek dayOfWeek = dateTime.DayOfWeek;
			switch (dayOfWeek) {
				case DayOfWeek.Sunday:
					if ((workdaysOptions.Workdays & WeekdayCore.Sunday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Monday:
					if ((workdaysOptions.Workdays & WeekdayCore.Monday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Tuesday:
					if ((workdaysOptions.Workdays & WeekdayCore.Tuesday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Wednesday:
					if ((workdaysOptions.Workdays & WeekdayCore.Wednesdey) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Thursday:
					if ((workdaysOptions.Workdays & WeekdayCore.Thursday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Friday:
					if ((workdaysOptions.Workdays & WeekdayCore.Friday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Saturday:
					if ((workdaysOptions.Workdays & WeekdayCore.Saturday) == 0)
						isHoliday = true;
					break;
			}
			if (!applyHolidays)
				return isHoliday;
			foreach (DateTime holiday in workdaysOptions.Holidays)
				if (dateTime.Date == holiday.Date) {
					isHoliday = true;
					break;
				}
			if (!applyExactWorkdays)
				return isHoliday;
			foreach (DateTime workday in workdaysOptions.ExactWorkdays)
				if (dateTime.Date == workday.Date) {
					isHoliday = false;
					break;
				}
			return isHoliday;
		}
		public static DateTime SkipHoliday(IWorkdaysOptions workdaysOptions, DateTime dateTime) {
			if ((int)workdaysOptions.Workdays == 0)
				return dateTime;
			while (IsHoliday(workdaysOptions, dateTime, true, true))
				dateTime = dateTime.Date.AddDays(1);
			return dateTime;
		}
		public static DateTime SkipHolidayReverted(IWorkdaysOptions workdaysOptions, DateTime dateTime) {
			if ((int)workdaysOptions.Workdays == 0)
				return dateTime;
			while (IsHoliday(workdaysOptions, dateTime, true, true))
				dateTime = dateTime.Date.AddMilliseconds(-1);
			return dateTime;
		}
		public static DateTime Floor(DateTime dateTime, DateTimeMeasureUnitNative measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnitNative.Month:
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnitNative.Day:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnitNative.Hour:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnitNative.Minute:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnitNative.Second:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnitNative.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnitNative.Week: {
						int offset = (int)dateTime.DayOfWeek - (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
						offset = offset >= 0 ? offset : offset + DaysInWeek;
						TimeSpan timeOffset = new TimeSpan(offset, 0, 0, 0);
						DateTime rounded = (dateTime.Ticks >= timeOffset.Ticks) ? dateTime - timeOffset : DateTime.MinValue;
						return new DateTime(rounded.Year, rounded.Month, rounded.Day);
					}
				case DateTimeMeasureUnitNative.Quarter: {
						DateTime rounded = dateTime.AddMonths(-((dateTime.Month - 1) % MonthsInQuarter));
						return new DateTime(rounded.Year, rounded.Month, 1);
					}
				default:
					ChartDebug.Fail("Invalid MeasureUnit.");
					return dateTime;
			}
		}
		public static DateTime AddRange(DateTimeMeasureUnitNative measureUnit, DateTimeMeasureUnitNative gridAlignment, DateTime date, double range, IWorkdaysOptions workdaysOptions) {
			date = Floor(date, measureUnit);
			bool workdaysOnly = (workdaysOptions != null) ? workdaysOptions.WorkdaysOnly : false;
			if (workdaysOnly && gridAlignment <= DateTimeMeasureUnitNative.Day)
				date = SkipHoliday(workdaysOptions, date);
			DateTime result;
			switch (gridAlignment) {
				case DateTimeMeasureUnitNative.Year:
					result = AddUnits(measureUnit, date, range, MonthsInYear, workdaysOptions);
					break;
				case DateTimeMeasureUnitNative.Quarter:
					result = AddUnits(measureUnit, date, range, MonthsInQuarter, workdaysOptions);
					break;
				case DateTimeMeasureUnitNative.Month:
					result = AddUnits(measureUnit, date, range, 1, workdaysOptions);
					break;
				case DateTimeMeasureUnitNative.Week:
					if (workdaysOnly) {
						DateTime weekRounded = Floor(date, DateTimeMeasureUnitNative.Week);
						range += GetDifference(measureUnit, gridAlignment, weekRounded, date, workdaysOptions);
						int fullWeeks = (int)Math.Floor(range);
						double remain = range - fullWeeks;
						result = weekRounded.AddDays(fullWeeks * DaysInWeek);
						int weekDaysCount = 0;
						for (int i = 0; i < DaysInWeek; i++)
							if (!IsHoliday(workdaysOptions, result.AddDays(i), true, true))
								weekDaysCount++;
						remain *= weekDaysCount;
						while (remain >= 0.5) {
							if (!IsHoliday(workdaysOptions, result, true, true))
								remain -= 1;
							result = result.AddDays(1);
						}
						result = result.AddDays(remain);
					} else
						result = date.AddDays(range * DaysInWeek);
					break;
				case DateTimeMeasureUnitNative.Day:
					result = workdaysOnly ? date.AddDays(CorrectRangeBasedOnHolidays(workdaysOptions, date, range, 1)) : date.AddDays(range);
					break;
				case DateTimeMeasureUnitNative.Hour:
					result = workdaysOnly ? date.AddHours(CorrectRangeBasedOnHolidays(workdaysOptions, date, range, HoursInDay)) :
						date.AddHours(range);
					break;
				case DateTimeMeasureUnitNative.Minute:
					result = workdaysOnly ? date.AddMinutes(CorrectRangeBasedOnHolidays(workdaysOptions, date, range, MinutesInDay)) :
						date.AddMinutes(range);
					break;
				case DateTimeMeasureUnitNative.Second:
					result = workdaysOnly ? date.AddSeconds(CorrectRangeBasedOnHolidays(workdaysOptions, date, range, SecondsInDay)) :
						date.AddSeconds(range);
					break;
				case DateTimeMeasureUnitNative.Millisecond:
					result = workdaysOnly ? date.AddMilliseconds(CorrectRangeBasedOnHolidays(workdaysOptions, date, range, MillisecondsInDay)) :
						date.AddMilliseconds(range);
					break;
				default:
					ChartDebug.Fail("Invalid MeasureUnit.");
					result = date;
					break;
			}
			result = Round(result, measureUnit);
			if (workdaysOnly)
				result = SkipHoliday(workdaysOptions, result);
			return result;
		}
		public static DateTime Increase(DateTime dateTime, DateTimeMeasureUnitNative measureUnit) {
			return Add(dateTime, measureUnit, false);
		}
		public static DateTime Decrease(DateTime dateTime, DateTimeMeasureUnitNative measureUnit) {
			return Add(dateTime, measureUnit, true);
		}
		public static double GetDifference(DateTimeMeasureUnitNative measureUnit, DateTimeMeasureUnitNative gridAlignment, DateTime fromDate, DateTime toDate, IWorkdaysOptions workdaysOptions) {
			bool workdaysOnly = (workdaysOptions != null) ? workdaysOptions.WorkdaysOnly : false;
			if (workdaysOnly) {
				fromDate = SkipHoliday(workdaysOptions, fromDate);
				toDate = SkipHoliday(workdaysOptions, toDate);
			}
			DateTime roundedFromDate = Floor(fromDate, measureUnit);
			DateTime roundedToDate = Floor(toDate, measureUnit);
			switch (gridAlignment) {
				case DateTimeMeasureUnitNative.Year:
					return TotalUnits(roundedToDate, workdaysOnly, workdaysOptions, MonthsInYear) -
						   TotalUnits(roundedFromDate, workdaysOnly, workdaysOptions, MonthsInYear);
				case DateTimeMeasureUnitNative.Quarter:
					return TotalUnits(roundedToDate, workdaysOnly, workdaysOptions, MonthsInQuarter) -
						   TotalUnits(roundedFromDate, workdaysOnly, workdaysOptions, MonthsInQuarter);
				case DateTimeMeasureUnitNative.Month:
					return TotalUnits(roundedToDate, workdaysOnly, workdaysOptions, 1) -
						   TotalUnits(roundedFromDate, workdaysOnly, workdaysOptions, 1);
				case DateTimeMeasureUnitNative.Week: {
						if (!workdaysOnly)
							return (roundedToDate - roundedFromDate).TotalDays / DaysInWeek;
						DateTime roundedToWeekFrom = Floor(roundedFromDate, DateTimeMeasureUnitNative.Week);
						DateTime roundedToWeekTo = Floor(roundedToDate, DateTimeMeasureUnitNative.Week);
						return (roundedToWeekTo - roundedToWeekFrom).TotalDays / DaysInWeek + CalcWeekAddition(workdaysOptions, roundedToDate, roundedToWeekTo) -
							CalcWeekAddition(workdaysOptions, roundedFromDate, roundedToWeekFrom);
					}
				case DateTimeMeasureUnitNative.Day: {
						double totalDays = (roundedToDate - roundedFromDate).TotalDays;
						if (workdaysOnly)
							totalDays = CorrectDifferenceBasedOnHolidays(workdaysOptions, roundedFromDate, totalDays, 1);
						return totalDays;
					}
				case DateTimeMeasureUnitNative.Hour: {
						double totalHours = (roundedToDate - roundedFromDate).TotalHours;
						if (workdaysOnly)
							totalHours = CorrectDifferenceBasedOnHolidays(workdaysOptions, roundedFromDate, totalHours, HoursInDay);
						return totalHours;
					}
				case DateTimeMeasureUnitNative.Minute: {
						double totalMinutes = (roundedToDate - roundedFromDate).TotalMinutes;
						if (workdaysOnly)
							totalMinutes = CorrectDifferenceBasedOnHolidays(workdaysOptions, roundedFromDate, totalMinutes, MinutesInDay);
						return totalMinutes;
					}
				case DateTimeMeasureUnitNative.Second: {
						double totalSeconds = (roundedToDate - roundedFromDate).TotalSeconds;
						if (workdaysOnly)
							totalSeconds = CorrectDifferenceBasedOnHolidays(workdaysOptions, roundedFromDate, totalSeconds, SecondsInDay);
						return totalSeconds;
					}
				case DateTimeMeasureUnitNative.Millisecond: {
						double totalMilliseconds = (roundedToDate - roundedFromDate).TotalMilliseconds;
						if (workdaysOnly)
							totalMilliseconds = CorrectDifferenceBasedOnHolidays(workdaysOptions, roundedFromDate, totalMilliseconds, MillisecondsInDay);
						return totalMilliseconds;
					}
				default:
					ChartDebug.Fail("Invalid MeasureUnit.");
					return 0;
			}
		}
		public static double GetPeriod(DateTime from, DateTime to, DateTimeMeasureUnitNative measureUnit) {
			DateTime actualFrom = from;
			DateTime actualTo = to;
			if (to < from) {
				actualFrom = to;
				actualTo = from;
			}
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Millisecond:
					return (actualTo - actualFrom).TotalMilliseconds;
				case DateTimeMeasureUnitNative.Second:
					return (actualTo - actualFrom).TotalSeconds;
				case DateTimeMeasureUnitNative.Minute:
					return (actualTo - actualFrom).TotalMinutes;
				case DateTimeMeasureUnitNative.Hour:
					return (actualTo - actualFrom).TotalHours;
				case DateTimeMeasureUnitNative.Day:
					return (actualTo - actualFrom).TotalDays;
				case DateTimeMeasureUnitNative.Week:
					return Math.Floor((actualTo - actualFrom).TotalDays / 7);
				case DateTimeMeasureUnitNative.Month:
					return (actualTo.Month - actualFrom.Month) + 12 * (actualTo.Year - actualFrom.Year);
				case DateTimeMeasureUnitNative.Quarter:
					return (Math.Floor((double)actualTo.Month / 4) - Math.Floor((double)actualFrom.Month / 4)) + 4 * (actualTo.Year - actualFrom.Year);
				case DateTimeMeasureUnitNative.Year:
					return actualTo.Year - actualFrom.Year;
			}
			return -1;
		}
	}
	public struct DateTimeFormatParts {
		readonly DateTimeOptionsFormat format;
		readonly string formatString;
		public DateTimeOptionsFormat Format {
			get {
				return format;
			}
		}
		public string FormatString {
			get {
				return formatString;
			}
		}
		public DateTimeFormatParts(DateTimeOptionsFormat format, string formatString) {
			this.format = format;
			this.formatString = formatString;
		}
	}
	public static class DateTimeUtilsExt {
		const int DaysInWeek = 7;
		const int MonthsInQuarter = 3;
		const int MonthsInYear = 12;
		const int HoursInDay = 24;
		const int MinutesInDay = HoursInDay * 60;
		const int SecondsInDay = MinutesInDay * 60;
		const int MillisecondsInDay = SecondsInDay * 1000;
		const double TicksInDay = MillisecondsInDay * 10000.0;
		public static DateTime MinDateTime {
			get { return DateTime.MinValue; }
		}
		static int MondayBasedDay(DayOfWeek day) {
			int offset = (int)day - 1;
			if (offset < 0)
				offset = 6;
			return offset;
		}
		static int WeekOffset(DayOfWeek firstDayOfWeek) {
			return 7 - MondayBasedDay(firstDayOfWeek);
		}
		public static bool MaskDay(DayOfWeek day, WeekdayCore flags) {
			return (((int)flags >> (int)day) & 1) == 1;
		}
		public static WeekdayCore Invert(WeekdayCore flags) {
			return (WeekdayCore)((~(int)flags) & 127);
		}
		static double TotalFloorMonths(DateTime dateTime) {
			return dateTime.Year * 12 + dateTime.Month - 13;
		}
		static double TotalMonths(DateTime dateTime) {
			DateTime monthRounded = Floor(dateTime, DateTimeMeasureUnitNative.Month, DayOfWeek.Monday);
			DateTime dayRounded = Floor(dateTime, DateTimeMeasureUnitNative.Day, DayOfWeek.Monday);
			int daysInMonth = (monthRounded.AddMonths(1) - monthRounded).Days;
			double monthAddition = (double)(dayRounded - monthRounded).Days / daysInMonth;
			double divider = TicksInDay * daysInMonth;
			double ticks = daysInMonth == 0 ? 0 : (double)(dateTime.Ticks - dayRounded.Ticks) / divider;
			return (monthRounded.Year * MonthsInYear + monthRounded.Month - 1 + monthAddition + ticks);
		}
		static DateTime AddMonths(DateTimeMeasureUnitNative measureUnit, DateTime date, double range, double factor) {
			range *= factor;
			DateTime monthRounded = Floor(date, DateTimeMeasureUnitNative.Month, DayOfWeek.Monday);
			range += Difference(monthRounded, date, measureUnit, null);
			int integerRange = (int)Math.Floor(range);
			double remain = range - integerRange;
			DateTime result = monthRounded.AddMonths(integerRange);
			int daysInMonth = (result.AddMonths(1) - result).Days;
			return result.AddDays(daysInMonth * remain);
		}
		static double SizeOfDay(DateTimeMeasureUnitNative measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Millisecond:
					return MillisecondsInDay;
				case DateTimeMeasureUnitNative.Second:
					return SecondsInDay;
				case DateTimeMeasureUnitNative.Minute:
					return MinutesInDay;
				case DateTimeMeasureUnitNative.Hour:
					return HoursInDay;
				case DateTimeMeasureUnitNative.Day:
					return 1;
			}
			return 0;
		}
		static DateTime RestoreCustomDays(DateTime baseDate, IEnumerable<CustomDate> customDays, WeekdayCore workdays) {
			DateTime currentDate = baseDate;
			foreach (CustomDate customDate in customDays) {
				if (customDate.Date <= currentDate) {
					bool dayMasked = MaskDay(customDate.Date.DayOfWeek, workdays);
					if (customDate.IsHoliday && dayMasked)
						do {
							currentDate = currentDate.AddDays(1);
						} while (IsHoliday(currentDate, workdays));
					else if (!customDate.IsHoliday && !dayMasked)
						do {
							currentDate = currentDate.AddDays(-1);
						} while (IsHoliday(currentDate, workdays) && (currentDate != customDate.Date));
				}
			}
			return currentDate;
		}
		public static double SizeOfMeasureUnit(DateTimeMeasureUnitNative measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Millisecond:
					return 1;
				case DateTimeMeasureUnitNative.Second:
					return 1000;
				case DateTimeMeasureUnitNative.Minute:
					return 60000;
				case DateTimeMeasureUnitNative.Hour:
					return 3600000;
				case DateTimeMeasureUnitNative.Day:
					return MillisecondsInDay;
				case DateTimeMeasureUnitNative.Week:
					return MillisecondsInDay * DaysInWeek;
				case DateTimeMeasureUnitNative.Month:
					return 30.0 * MillisecondsInDay;
				case DateTimeMeasureUnitNative.Quarter:
					return 30.0 * MillisecondsInDay * MonthsInQuarter;
				case DateTimeMeasureUnitNative.Year:
					return 30.0 * MillisecondsInDay * MonthsInYear;
				default:
					return 1;
			}			
		}
		public static DateTime Floor(DateTime dateTime, DateTimeMeasureUnitNative measureUnit, DayOfWeek firstDayOfWeek) {
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnitNative.Month:
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnitNative.Day:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnitNative.Hour:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnitNative.Minute:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnitNative.Second:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnitNative.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnitNative.Week:
					int offset = MondayBasedDay(dateTime.DayOfWeek) + (7 - MondayBasedDay(firstDayOfWeek));
					if (offset >= 7)
						offset = offset - 7;
					TimeSpan dateTimeSpan = dateTime - DateTime.MinValue;
					if (dateTimeSpan.TotalDays < offset)
						return DateTime.MinValue;
					DateTime alignedDate = dateTime.AddDays(-offset);
					return new DateTime(alignedDate.Year, alignedDate.Month, alignedDate.Day);
				case DateTimeMeasureUnitNative.Quarter:
					DateTime rounded = dateTime.AddMonths(-((dateTime.Month - 1) % MonthsInQuarter));
					return new DateTime(rounded.Year, rounded.Month, 1);
				default:
					return dateTime;
			}
		}
		internal static DateTime Round(DateTime dateTime, DateTimeMeasureUnitNative measureUnit, DayOfWeek firstDayOfWeek) {
			int half = dateTime.Month == 2 ? 14 : 15;
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					if (dateTime.Month > 6)
						dateTime = dateTime.AddYears(1);
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnitNative.Month:
					if (dateTime.Day > half)
						dateTime = dateTime.AddMonths(1);
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnitNative.Day:
					if (dateTime.Hour > 12 || (dateTime.Hour == 12 && (dateTime.Minute > 0 || dateTime.Second > 0 || dateTime.Millisecond > 0)))
						dateTime = dateTime.AddDays(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnitNative.Hour:
					if (dateTime.Minute >= 30)
						dateTime = dateTime.AddHours(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnitNative.Minute:
					if (dateTime.Second >= 30)
						dateTime = dateTime.AddMinutes(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnitNative.Second:
					if (dateTime.Millisecond >= 500)
						dateTime = dateTime.AddSeconds(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnitNative.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnitNative.Week:
					int offset = MondayBasedDay(dateTime.DayOfWeek) + (7 - MondayBasedDay(firstDayOfWeek));
					if (offset >= 7)
						offset = offset - 7;
					DateTime alignedDate;
					if (offset > 2)
						alignedDate = dateTime.AddDays(7 - offset);
					else {
						TimeSpan span = dateTime - DateTime.MinValue;
						if (span.TotalDays >= offset)
							alignedDate = dateTime.AddDays(-offset);
						else
							alignedDate = DateTime.MinValue;
					}
					return new DateTime(alignedDate.Year, alignedDate.Month, alignedDate.Day);
				case DateTimeMeasureUnitNative.Quarter: {
						if (dateTime.Day > half)
							dateTime = dateTime.AddMonths(1);
						DateTime rounded = dateTime.AddMonths(-1 * ((dateTime.Month - 1) % MonthsInQuarter));
						return new DateTime(rounded.Year, rounded.Month, 1);
					}
				default:
					return dateTime;
			}
		}
		public static DateTime Add(DateTime dateTime, DateTimeMeasureUnitNative measureUnit, double range, DayOfWeek firstDayOfWeek) {
			DateTime roundedDate = Floor(dateTime, measureUnit, firstDayOfWeek);
			DateTime result;
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					result = AddMonths(measureUnit, roundedDate, range, MonthsInYear);
					break;
				case DateTimeMeasureUnitNative.Quarter:
					result = AddMonths(measureUnit, roundedDate, range, MonthsInQuarter);
					break;
				case DateTimeMeasureUnitNative.Month:
					result = AddMonths(measureUnit, roundedDate, range, 1);
					break;
				case DateTimeMeasureUnitNative.Week:
					int actualDayOfWeek = 0;
					if (roundedDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(dateTime, firstDayOfWeek);
					double daysToAdd = Math.Max(range * DaysInWeek - actualDayOfWeek, 0);
					result = roundedDate.AddDays(daysToAdd);
					break;
				case DateTimeMeasureUnitNative.Day:
					result = roundedDate.AddDays(range);
					break;
				case DateTimeMeasureUnitNative.Hour:
					result = roundedDate.AddHours(range);
					break;
				case DateTimeMeasureUnitNative.Minute:
					result = roundedDate.AddMinutes(range);
					break;
				case DateTimeMeasureUnitNative.Second:
					result = roundedDate.AddSeconds(range);
					break;
				case DateTimeMeasureUnitNative.Millisecond:
					result = roundedDate.AddMilliseconds(range);
					break;
				default:
					result = roundedDate;
					break;
			}
			return Max(MinDateTime, Round(result, measureUnit, firstDayOfWeek));
		}
		static DateTime Max(DateTime a, DateTime b) {
			if (a > b)
				return a;
			return b;
		}
		static double GetDiffInMonths(DateTime fromDate, DateTime toDate) {
			return TotalFloorMonths(toDate) - TotalFloorMonths(fromDate);
		}
		static int GetActualDayOfWeek(DateTime dateTime, DayOfWeek firstDayOfWeek) {
			int actualDayOfWeek = (int)dateTime.DayOfWeek - (int)firstDayOfWeek;
			if (actualDayOfWeek < 0)
				actualDayOfWeek += 7;
			return actualDayOfWeek;
		}
		public static double Difference(DateTime fromDate, DateTime toDate, DateTimeMeasureUnitNative measureUnit, IWorkdaysOptions workdaysOptions) {
			DayOfWeek firstDayOfWeek = GetFirstDayOfWeek(workdaysOptions);
			DateTime roundedFromDate = Floor(fromDate, measureUnit, firstDayOfWeek);
			DateTime roundedToDate = Floor(toDate, measureUnit, firstDayOfWeek);
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Year:
					return roundedToDate.Year - roundedFromDate.Year;
				case DateTimeMeasureUnitNative.Quarter:
					return GetDiffInMonths(roundedFromDate, roundedToDate) / MonthsInQuarter;
				case DateTimeMeasureUnitNative.Month:
					return GetDiffInMonths(roundedFromDate, roundedToDate);
				case DateTimeMeasureUnitNative.Week:
					int actualDayOfWeek = 0;
					if (roundedFromDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(fromDate, firstDayOfWeek);
					else if (roundedToDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(toDate, firstDayOfWeek);
					return ((roundedToDate - roundedFromDate).TotalDays + actualDayOfWeek) / DaysInWeek;
				case DateTimeMeasureUnitNative.Day:
					return (roundedToDate - roundedFromDate).TotalDays;
				case DateTimeMeasureUnitNative.Hour:
					return (roundedToDate - roundedFromDate).TotalHours;
				case DateTimeMeasureUnitNative.Minute:
					return (roundedToDate - roundedFromDate).TotalMinutes;
				case DateTimeMeasureUnitNative.Second:
					return (roundedToDate - roundedFromDate).TotalSeconds;
				case DateTimeMeasureUnitNative.Millisecond:
					return (roundedToDate - roundedFromDate).TotalMilliseconds;
				default:
					return 0;
			}
		}
		public static DayOfWeek GetFirstDayOfWeek(IWorkdaysOptions workdaysOptions) {
			return (workdaysOptions != null) ? workdaysOptions.FirstDayOfWeek : DayOfWeek.Monday;
		}
		internal static DateTime GetMonday(DateTime date) {
			DayOfWeek day = date.DayOfWeek;
			int daysToMonday = ((day == DayOfWeek.Sunday) ? 7 : (int)day) - (int)DayOfWeek.Monday;
			return date.AddDays(-daysToMonday);
		}
		internal static int GetNumberOfHolidaysInWeek(WeekdayCore holidays) {
			int number = 0;
			for (int i = 0; i < sizeof(int) * 8; i++)
				number += ((int)holidays >> i) & 1;
			return number;
		}
		internal static int GetHolidaysCountInWeek(DayOfWeek day, WeekdayCore holidays) {
			int correctDayNumber = ((day == DayOfWeek.Sunday) ? 6 : (int)day - 1);
			int correctHolidays = (((int)holidays >> 1) & 63) | (((int)holidays & 1) << 6);
			int number = 0;
			if (correctHolidays != 127) {
				for (int i = 0; i <= correctDayNumber; i++)
					number += (correctHolidays >> i) & 1;
			}
			return number;
		}
		internal static int RestoreDay(int day, WeekdayCore holidays) {
			int correctHolidays = (((int)holidays >> 1) & 63) | (((int)holidays & 1) << 6);
			int i = 0;
			while (i < 7) {
				day -= 1 - (correctHolidays >> i) & 1;
				if (day < 0)
					break;
				i++;
			}
			return i;
		}
		internal static int GetHolidaysCount(DateTime date, WeekdayCore holidays) {
			TimeSpan weekPart = date - GetMonday(date);
			TimeSpan wholePeriod = GetMonday(date) - MinDateTime;
			int numberOfHolidays = GetNumberOfHolidaysInWeek(holidays);
			return (int)(wholePeriod.TotalDays / 7) * numberOfHolidays + GetHolidaysCountInWeek(date.DayOfWeek, holidays);
		}
		internal static int GetHolidaysCount(DateTime date, IWorkdaysOptions options) {
			if ((options != null) && options.WorkdaysOnly) {
				WeekdayCore workdays = options.Workdays;
				int holidaysNumber = GetHolidaysCount(date, Invert(workdays));
				foreach (CustomDate customDate in options.CustomDates) {
					if (customDate.Date <= date) {
						bool dayMasked = MaskDay(customDate.Date.DayOfWeek, workdays);
						if (customDate.IsHoliday && dayMasked)
							holidaysNumber++;
						else if (!customDate.IsHoliday && !dayMasked)
							holidaysNumber--;
					}
				}
				return holidaysNumber;
			}
			return 0;
		}
		internal static bool IsHoliday(DateTime date, WeekdayCore workdays) {
			int day = (int)date.DayOfWeek;
			return (((int)workdays >> day) & 1) == 0;
		}
		internal static DateTime ReverseHolidays(double value, IWorkdaysOptions options, DateTimeMeasureUnitNative measureUnit) {
			DayOfWeek firstDayOfWeek = (options != null) ? options.FirstDayOfWeek : DayOfWeek.Monday;
			if ((options != null) && options.WorkdaysOnly && ((int)measureUnit <= (int)DateTimeMeasureUnitNative.Day)) {
				WeekdayCore workdays = options.Workdays;
				if (workdays != 0) {
					double inDays = Math.Floor(value / SizeOfDay(measureUnit));
					double workdaysInWeek = 7 - GetHolidaysCountInWeek(DayOfWeek.Sunday, Invert(workdays));
					double weeks = Math.Floor(inDays / workdaysInWeek);
					double dayInWeek = inDays - weeks * workdaysInWeek;
					double partOfDay = (value - (inDays * SizeOfDay(measureUnit))) / SizeOfDay(measureUnit);
					DateTime restoredDay = DateTimeUtilsExt.Add(MinDateTime, DateTimeMeasureUnitNative.Day, weeks * 7 + RestoreDay((int)dayInWeek, Invert(options.Workdays)), firstDayOfWeek);
					restoredDay = RestoreCustomDays(restoredDay, options.CustomDates, workdays);
					return DateTimeUtilsExt.Add(restoredDay, DateTimeMeasureUnitNative.Millisecond, partOfDay * SizeOfDay(DateTimeMeasureUnitNative.Millisecond), firstDayOfWeek);
				} else
					return MinDateTime;
			}
			return DateTimeUtilsExt.Add(MinDateTime, measureUnit, value, firstDayOfWeek);
		}
		internal static double GetHolidaysCount(DateTime date, IWorkdaysOptions options, DateTimeMeasureUnitNative measureUnit) {
			if ((int)measureUnit <= (int)DateTimeMeasureUnitNative.Day) {
				return GetHolidaysCount(date, options) * SizeOfDay(measureUnit);
			}
			return 0;
		}
		public static DateTimeFormatParts SelectFormat(DateTimeGridAlignmentNative measureUnit) {
			switch (measureUnit) {
				case DateTimeGridAlignmentNative.Year:
					return new DateTimeFormatParts(DateTimeOptionsFormat.Custom, "yyyy");
				case DateTimeGridAlignmentNative.Quarter:
				case DateTimeGridAlignmentNative.Month:
					return new DateTimeFormatParts(DateTimeOptionsFormat.MonthAndYear, string.Empty);
				case DateTimeGridAlignmentNative.Week:
				case DateTimeGridAlignmentNative.Day:
					return new DateTimeFormatParts(DateTimeOptionsFormat.ShortDate, string.Empty);
				case DateTimeGridAlignmentNative.Hour:
				case DateTimeGridAlignmentNative.Minute:
					return new DateTimeFormatParts(DateTimeOptionsFormat.Custom, "t");
				case DateTimeGridAlignmentNative.Second:
					return new DateTimeFormatParts(DateTimeOptionsFormat.Custom, "T");
				case DateTimeGridAlignmentNative.Millisecond:
					return new DateTimeFormatParts(DateTimeOptionsFormat.Custom, CreateLongTimePatternWithMillisecond());
				default:
					return new DateTimeFormatParts(DateTimeOptionsFormat.General, string.Empty);
			}
		}
		public static string CreateLongTimePatternWithMillisecond() {
			string timePattern = DateTimeFormatInfo.CurrentInfo.LongTimePattern;
			return Regex.Replace(timePattern, "(:ss|:s)", "$1.ffff");
		}
	}
}
