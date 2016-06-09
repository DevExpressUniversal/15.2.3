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
using System.Threading;
using DevExpress.Utils;
using System.Globalization;
namespace DevExpress.Sparkline.Core {
	public static class SparklineMathUtils {
		public static bool IsValidDouble(double value) {
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}
		public static int Round(double value) {
			return (int)Math.Round(value);
		}
		public static bool AreDoublesEqual(double a, double b) {
			return Math.Abs(a - b) <= double.Epsilon;
		}
	}
	public enum DateTimeMeasureUnit {
		Millisecond,
		Second,
		Minute,
		Hour,
		Day,
		Week,
		Month,
		Quarter,
		Year
	}
	public static class SparklineDateTimeUtils {
		const int HoursInDay = 24;
		const int MinutesInDay = HoursInDay * 60;
		const int SecondsInDay = MinutesInDay * 60;
		const int MillisecondsInDay = SecondsInDay * 1000;
		static int GetActualDayOfWeek(DateTime dateTime) {
			int actualDayOfWeek = (int)dateTime.DayOfWeek - (int)FirstDayOfWeek;
			if (actualDayOfWeek < 0)
				actualDayOfWeek += 7;
			return actualDayOfWeek;
		}
		static DayOfWeek FirstDayOfWeek {
			get {
				return CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek;
			}
		}
		static double TotalFloorMonths(DateTime dateTime) {
			return dateTime.Year * 12 + dateTime.Month - 13;
		}
		static DateTime AddMonths(DateTimeMeasureUnit measureUnit, DateTime date, double range, double factor) {
			range *= factor;
			DateTime monthRounded = Floor(date, DateTimeMeasureUnit.Month);
			range += Difference(monthRounded, date, measureUnit);
			int integerRange = (int)Math.Floor(range);
			double remain = range - integerRange;
			DateTime result = monthRounded.AddMonths(integerRange);
			int daysInMonth = (result.AddMonths(1) - result).Days;
			return result.AddDays(daysInMonth * remain);
		}
		public static DateTime Floor(DateTime dateTime, DateTimeMeasureUnit measureUnit) {
			DayOfWeek firstDayOfWeek = FirstDayOfWeek;
			switch (measureUnit) {
				case DateTimeMeasureUnit.Year:
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnit.Month:
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnit.Day:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnit.Hour:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnit.Minute:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnit.Second:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnit.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnit.Week:
					int actualDayOfWeek = GetActualDayOfWeek(dateTime);
					TimeSpan dateTimeSpan = dateTime - DateTime.MinValue;
					DateTime alignedDate;
					if (dateTimeSpan.TotalDays < actualDayOfWeek)
						alignedDate = DateTime.MinValue;
					else
						alignedDate = dateTime.AddDays(-actualDayOfWeek);
					return new DateTime(alignedDate.Year, alignedDate.Month, alignedDate.Day);
				case DateTimeMeasureUnit.Quarter:
					DateTime rounded = dateTime.AddMonths(-((dateTime.Month - 1) % 3));
					return new DateTime(rounded.Year, rounded.Month, 1);
				default:
					return dateTime;
			}
		}
		public static DateTime Add(DateTime dateTime, DateTimeMeasureUnit measureUnit, double range) {
			DayOfWeek firstDayOfWeek = FirstDayOfWeek;
			DateTime roundedDate = Floor(dateTime, measureUnit);
			DateTime result;
			switch (measureUnit) {
				case DateTimeMeasureUnit.Year:
					result = AddMonths(measureUnit, roundedDate, range, 12);
					break;
				case DateTimeMeasureUnit.Quarter:
					result = AddMonths(measureUnit, roundedDate, range, 3);
					break;
				case DateTimeMeasureUnit.Month:
					result = AddMonths(measureUnit, roundedDate, range, 1);
					break;
				case DateTimeMeasureUnit.Week:
					int actualDayOfWeek = 0;
					if (roundedDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(dateTime);
					double daysToAdd = Math.Max(range * 7 - actualDayOfWeek, 0);
					result = roundedDate.AddDays(daysToAdd);
					break;
				case DateTimeMeasureUnit.Day:
					result = roundedDate.AddDays(range);
					break;
				case DateTimeMeasureUnit.Hour:
					result = roundedDate.AddHours(range);
					break;
				case DateTimeMeasureUnit.Minute:
					result = roundedDate.AddMinutes(range);
					break;
				case DateTimeMeasureUnit.Second:
					result = roundedDate.AddSeconds(range);
					break;
				case DateTimeMeasureUnit.Millisecond:
					result = roundedDate.AddMilliseconds(range);
					break;
				default:
					result = roundedDate;
					break;
			}
			return Round(result, measureUnit);
		}
		public static double Difference(DateTime fromDate, DateTime toDate, DateTimeMeasureUnit measureUnit) {
			DateTime roundedFromDate = Floor(fromDate, measureUnit);
			DateTime roundedToDate = Floor(toDate, measureUnit);
			switch (measureUnit) {
				case DateTimeMeasureUnit.Year:
					return roundedToDate.Year - roundedFromDate.Year;
				case DateTimeMeasureUnit.Quarter:
					return (TotalFloorMonths(roundedToDate) - TotalFloorMonths(roundedFromDate)) / 3;
				case DateTimeMeasureUnit.Month:
					return TotalFloorMonths(roundedToDate) - TotalFloorMonths(roundedFromDate);
				case DateTimeMeasureUnit.Week:
					int actualDayOfWeek = 0;
					if (roundedFromDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(fromDate);
					else if (roundedToDate == DateTime.MinValue)
						actualDayOfWeek = GetActualDayOfWeek(toDate);
					return ((roundedToDate - roundedFromDate).TotalDays + actualDayOfWeek) / 7;
				case DateTimeMeasureUnit.Day:
					return (roundedToDate - roundedFromDate).TotalDays;
				case DateTimeMeasureUnit.Hour:
					return (roundedToDate - roundedFromDate).TotalHours;
				case DateTimeMeasureUnit.Minute:
					return (roundedToDate - roundedFromDate).TotalMinutes;
				case DateTimeMeasureUnit.Second:
					return (roundedToDate - roundedFromDate).TotalSeconds;
				case DateTimeMeasureUnit.Millisecond:
					return (roundedToDate - roundedFromDate).TotalMilliseconds;
				default:
					return 0;
			}
		}
		public static DateTime Round(DateTime dateTime, DateTimeMeasureUnit measureUnit) {
			DayOfWeek firstDayOfWeek = FirstDayOfWeek;
			int half = dateTime.Month == 2 ? 14 : 15;
			switch (measureUnit) {
				case DateTimeMeasureUnit.Year:
					if (dateTime.Month > 6)
						dateTime = dateTime.AddYears(1);
					return new DateTime(dateTime.Year, 1, 1);
				case DateTimeMeasureUnit.Month:
					if (dateTime.Day > half)
						dateTime = dateTime.AddMonths(1);
					return new DateTime(dateTime.Year, dateTime.Month, 1);
				case DateTimeMeasureUnit.Day:
					if (dateTime.Hour > 12 || (dateTime.Hour == 12 && (dateTime.Minute > 0 || dateTime.Second > 0 || dateTime.Millisecond > 0)))
						dateTime = dateTime.AddDays(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				case DateTimeMeasureUnit.Hour:
					if (dateTime.Minute >= 30)
						dateTime = dateTime.AddHours(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
				case DateTimeMeasureUnit.Minute:
					if (dateTime.Second >= 30)
						dateTime = dateTime.AddMinutes(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
				case DateTimeMeasureUnit.Second:
					if (dateTime.Millisecond >= 500)
						dateTime = dateTime.AddSeconds(1);
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
				case DateTimeMeasureUnit.Millisecond:
					return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
				case DateTimeMeasureUnit.Week:
					int actualDayOfWeek = (int)dateTime.DayOfWeek - (int)firstDayOfWeek;
					if (actualDayOfWeek > 0) {
						if (actualDayOfWeek > 3)
							actualDayOfWeek += (7 - actualDayOfWeek);
					} else {
						if (actualDayOfWeek <= -4)
							actualDayOfWeek -= (7 + actualDayOfWeek);
					}
					DateTime alignedDate = dateTime.AddDays(actualDayOfWeek);
					return new DateTime(alignedDate.Year, alignedDate.Month, alignedDate.Day);
				case DateTimeMeasureUnit.Quarter: {
						if (dateTime.Day > half)
							dateTime = dateTime.AddMonths(1);
						DateTime rounded = dateTime.AddMonths(-1 * ((dateTime.Month - 1) % 3));
						return new DateTime(rounded.Year, rounded.Month, 1);
					}
				default:
					return dateTime;
			}
		}
		public static double SizeOfMeasureUnit(DateTimeMeasureUnit measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnit.Millisecond:
					return 1;
				case DateTimeMeasureUnit.Second:
					return 1000;
				case DateTimeMeasureUnit.Minute:
					return 60000;
				case DateTimeMeasureUnit.Hour:
					return 3600000;
				case DateTimeMeasureUnit.Day:
					return MillisecondsInDay;
				case DateTimeMeasureUnit.Week:
					return MillisecondsInDay * 7;
				case DateTimeMeasureUnit.Month:
					return 30.0 * MillisecondsInDay;
				case DateTimeMeasureUnit.Quarter:
					return 30.0 * MillisecondsInDay * 3;
				case DateTimeMeasureUnit.Year:
					return 30.0 * MillisecondsInDay * 12;
				default:
					return 1;
			}
		}
	}
}
