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
namespace DevExpress.XtraPivotGrid.Utils.DateHelpers {
	public static class DateHelper {
		static CultureInfo CultureInfo { get { return CultureInfo.CurrentCulture; } }
		public static int GetWeekOfMonth(DateTime dateTime) {
			DateTime firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			int weekOfYear = GetWeekOfYear(dateTime);
			int weekOfMonth = weekOfYear - GetWeekOfYear(firstDayOfMonth) + 1;
			return weekOfMonth < 0 ? weekOfYear + 1 : weekOfMonth;
		}
		public static int GetWeekOfYear(DateTime dateTime) {
			return CultureInfo.Calendar.GetWeekOfYear(dateTime, CultureInfo.DateTimeFormat.CalendarWeekRule,
												CultureInfo.DateTimeFormat.FirstDayOfWeek);
		}
		public static int GetFullYears(DateTime start, DateTime end) {
			int fullYear = end.Year - start.Year;
			if(fullYear > 0) {
				if(end.Month < start.Month) {
					fullYear--;
				} else {
					if(end.Month == start.Month && end.Day < start.Day) {
						fullYear--;
					}
				}
			}
			return fullYear;
		}
		public static int GetFullMonths(DateTime start, DateTime end) {
			int fullMonth = 0;
			if(start.Year == end.Year) {
				fullMonth = end.Month - start.Month;
			} else {
				fullMonth = 12 - start.Month + end.Month;
				if(end.Year > start.Year + 1) {
					fullMonth += 12 * (end.Year - start.Year - 1);
				}
			}
			if(fullMonth > 0) {
				if(end.Day < start.Day) {
					fullMonth--;
				}
			}
			return fullMonth;
		}
		public static int GetFullWeeks(DateTime start, DateTime end) {
			return (int)(GetFullDays(start, end) / 7);
		}
		public static int GetFullDays(DateTime start, DateTime end) {
			return (end - start).Days;
		}
		public static int CompareDayOfWeek(DayOfWeek val1, DayOfWeek val2) {
			return Comparer<int>.Default.Compare(GetAbsDayOfWeek(val1), GetAbsDayOfWeek(val2));
		}
		static int GetAbsDayOfWeek(DayOfWeek val) {
			if(val < CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
				return 7 + (int)val;
			else
				return (int)val;
		}
		public static DateTime GetDateFromWeekOfMonth(int year, int month, int weekOfMonth) {
			DateTime firstDate = new DateTime(year, month, 1);
			if(weekOfMonth <= 1 || weekOfMonth > 6)
				return firstDate;
			DateTime date = CultureInfo.Calendar.AddWeeks(firstDate, weekOfMonth - 1);
			while(CultureInfo.Calendar.GetDayOfWeek(date) != CultureInfo.DateTimeFormat.FirstDayOfWeek || date.Month > month) {
				date = date.AddDays(-1);
			}
			return date;
		}
		public static DateTime GetDateFromWeekOfYear(int year, int weekOfYear) {
			DateTime date = new DateTime(year, 1, 1);
			if(weekOfYear == 1)
				return date;
			bool isFirstWeekPartial = false;
			while(CultureInfo.Calendar.GetDayOfWeek(date) != CultureInfo.DateTimeFormat.FirstDayOfWeek) {
				isFirstWeekPartial = true;
				date = date.AddDays(1);
			}
			if(isFirstWeekPartial) 
				weekOfYear--;
			return CultureInfo.Calendar.AddWeeks(date, weekOfYear - 1);
		}
		public static DateTime GetDateFromDayOfYear(int year, int dayOfYear) {
			DateTime date = new DateTime(year, 1, 1);
			return date.AddDays(dayOfYear - 1);
		}
		public static DateTime GetDateFromDayOfWeekAndWeekOfYear(int year, int weekOfYear, DayOfWeek dayOfWeek) {
			DateTime date = GetDateFromWeekOfYear(year, weekOfYear);
			while(CultureInfo.Calendar.GetDayOfWeek(date) != dayOfWeek)
				date = date.AddDays(1);
			return date;
		}
		public static DateTime GetDateFromDayOfWeekAndWeekOfMonth(int year, int month, int weekOfMonth, DayOfWeek dayOfWeek) {
			DateTime date = GetDateFromWeekOfMonth(year, month, weekOfMonth);
			while(CultureInfo.Calendar.GetDayOfWeek(date) != dayOfWeek)
				date = date.AddDays(1);
			return date;
		}
	}
}
