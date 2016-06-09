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
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DayCountBasisFactory
	public static class DayCountBasisFactory {
		static Dictionary<int, DayCountBasisBase> itemsTable = GetItemsTable();
		static Dictionary<int, DayCountBasisBase> GetItemsTable() {
			Dictionary<int, DayCountBasisBase> result = new Dictionary<int, DayCountBasisBase>();
			result.Add(0, new DaysCountBasisUS360());
			result.Add(1, new DaysCountBasisActual());
			result.Add(2, new DaysCountBasisActual360());
			result.Add(3, new DaysCountBasisActual365());
			result.Add(4, new DaysCountBasisEU360());
			return result;
		}
		internal static DayCountBasisBase GetBasis(int descriptor) {
			if (itemsTable.ContainsKey(descriptor))
				return itemsTable[descriptor];
			return null;
		}
	}
	#endregion
	#region DayCountBasisBase
	public abstract class DayCountBasisBase {
		public const int MaxDateTimeSerialNumber1904 = 2955541;
		protected internal virtual int GetDays(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			return endSerialNumber - startSerialNumber;
		}
		protected internal virtual int GetDaysInMonth(int month, int year) {
			return 30;
		}
		protected internal abstract int GetDaysInYear(int year);
		protected internal abstract double GetAverageDaysInYear(int startSerialNumber, int endSerialNumber, WorkbookDataContext context);
		protected internal double GetYearFrac(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			if (startSerialNumber == endSerialNumber)
				return 0;
			return GetDays(startSerialNumber, endSerialNumber, context) / GetAverageDaysInYear(startSerialNumber, endSerialNumber, context);
		}
		protected internal virtual int GetMaxDateTimeSerialNumber(DateSystem dateSystem) {
			return dateSystem == DateSystem.Date1900 ? (int)WorkbookDataContext.MaxDateTimeSerialNumber : (int)WorkbookDataContext.MaxDateTimeSerialNumber1904;
		}
	}
	#endregion
	#region DayCountBasis360Base
	public abstract class DayCountBasis360Base : DayCountBasisBase {
		protected internal override int GetDaysInYear(int year) {
			return 360;
		}
		protected internal override double GetAverageDaysInYear(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			return 360;
		}
		protected internal override int GetDays(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			if (startSerialNumber == endSerialNumber)
				return 0;
			if (context.DateSystem == DateSystem.Date1900)
				return GetDaysCore1900(startSerialNumber, endSerialNumber, context);
			return GetDaysCore1904(startSerialNumber, endSerialNumber, context);
		}
		protected DateTime ConvertToDate1900(int serialNumber, WorkbookDataContext context) {
			return serialNumber == 60 ? new DateTime(1900, 2, 1) : ConvertToDate(serialNumber, context);
		}
		protected DateTime ConvertToDate(int serialNumber, WorkbookDataContext context) {
			return context.FromDateTimeSerial(serialNumber);
		}
		protected int GetDaysResult(DateTime startDate, DateTime endDate, int startDateDay, int endDateDay) {
			return DateToDays(endDate.Year, endDate.Month, endDateDay) - DateToDays(startDate.Year, startDate.Month, startDateDay);
		}
		int DateToDays(int year, int month, int day) {
			return year * 360 + month * 30 + day;
		}
		protected bool IsLastDayOfFebruary(DateTime date) {
			return date.Month == 2 && IsLastDayOfMonth(date);
		}
		protected bool IsLastDayOfMonth(DateTime date) {
			return DaysInMonth(date) == date.Day;
		}
		protected int DaysInMonth(DateTime date) {
			return DateTime.DaysInMonth(date.Year, date.Month);
		}
		protected abstract int GetDaysCore1900(int startSerialNumber, int endSerialNumber, WorkbookDataContext context);
		protected abstract int GetDaysCore1904(int startSerialNumber, int endSerialNumber, WorkbookDataContext context);
		protected internal override int GetMaxDateTimeSerialNumber(DateSystem dateSystem) {
			return dateSystem == DateSystem.Date1900 ? (int)WorkbookDataContext.MaxDateTimeSerialNumber : MaxDateTimeSerialNumber1904;
		}
	}
	#endregion
	#region DaysCountBasisUS360
	public class DaysCountBasisUS360 : DayCountBasis360Base {
		protected override int GetDaysCore1900(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			DateTime startDate = ConvertToDate1900(startSerialNumber, context);
			DateTime endDate = ConvertToDate1900(endSerialNumber, context);
			int startDateDay = startDate.Day;
			int endDateDay = endDate.Day;
			if (endDateDay == 31 && (startDateDay == 30 || startDateDay == 31) && startDate.Year != 1899)
				endDateDay = 30;
			if (startDateDay == 31)
				startDateDay = 30;
			if (IsLastDayOfFebruary(startDate)) {
				startDateDay = 30;
				if (IsLastDayOfFebruary(endDate))
					endDateDay = 30;
			}
			if (startSerialNumber == 60)
				startDateDay = 29;
			if (endSerialNumber == 60)
				endDateDay = 29;
			return GetDaysResult(startDate, endDate, startDateDay, endDateDay);
		}
		protected override int GetDaysCore1904(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			DateTime startDate = ConvertToDate(startSerialNumber, context);
			DateTime endDate = ConvertToDate(endSerialNumber, context);
			int maxDaysInStartMonth = DaysInMonth(startDate);
			int maxDaysInEndMonth = DaysInMonth(endDate);
			if (endDate.Year == startDate.Year && endDate.Month == startDate.Month)
				return GetDaysForEqualYearsAndMonths(startDate, endDate, maxDaysInEndMonth);
			int startDateDay = ConvertDays(startDate);
			int endDateDay = ConvertDays(endDate);
			bool startDayIsSecondFromBack = IsDayOfMonthBackwise(startDate.Day, maxDaysInStartMonth, 1);
			bool endDayIsSecondFromBack = IsDayOfMonthBackwise(endDate.Day, maxDaysInEndMonth, 1);
			if (startDayIsSecondFromBack) {
				if (maxDaysInStartMonth != 30 || maxDaysInEndMonth != 31 || !endDayIsSecondFromBack)
					startDateDay = 29;
				if (endDayIsSecondFromBack && ((maxDaysInStartMonth == maxDaysInEndMonth) || (startDate.Month == 2 && endDate.Month == 2))) {
					startDateDay = 29;
					endDateDay = 29;
				}
			}
			if (endDayIsSecondFromBack && EndDayIs29Condition(startDate.Day, endDate.Month, maxDaysInStartMonth))
				endDateDay = 29;
			return GetDaysResult(startDate, endDate, startDateDay, endDateDay);
		}
		bool EndDayIs29Condition(int startDateDay, int endDateMonth, int maxDaysInStartMonth) {
			return ((maxDaysInStartMonth == 31 && IsDayOfMonthBackwise(startDateDay, maxDaysInStartMonth, 2)) ||
				   (maxDaysInStartMonth == 30 && IsDayOfMonthBackwise(startDateDay, maxDaysInStartMonth, 1))) &&
				   endDateMonth != 2;
		}
		int GetDaysForEqualYearsAndMonths(DateTime startDate, DateTime endDate, int maxDaysInEndMonth) {
			int dayDifference = endDate.Day - startDate.Day;
			bool secondDayFromBackOf31DayMonth = IsDayOfMonthBackwise(endDate.Day, maxDaysInEndMonth, 1) && maxDaysInEndMonth == 31;
			if (dayDifference == 1) 
				return secondDayFromBackOf31DayMonth ? 0 : dayDifference;
			return ConvertDays(endDate) - ConvertDays(startDate);
		}
		int ConvertDays(DateTime date) {
			return IsLastDayOfMonth(date) ? 30 : date.Day;
		}
		bool IsDayOfMonthBackwise(int day, int numberOfDaysInMonth, int position) {
			return day == numberOfDaysInMonth - position;
		}
	}
	#endregion
	#region DaysCountBasisEU360
	public class DaysCountBasisEU360 : DayCountBasis360Base {
		protected override int GetDaysCore1900(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			DateTime startDate = ConvertToDate1900(startSerialNumber, context);
			DateTime endDate = ConvertToDate1900(endSerialNumber, context);
			int startDateDay = startDate.Day;
			int endDateDay = endDate.Day;
			if (startDate.Day == 31)
				startDateDay = 30;
			if (endDate.Day == 31)
				endDateDay = 30;
			if (startSerialNumber == 60)
				startDateDay = 29;
			if (endSerialNumber == 60)
				endDateDay = 29;
			return GetDaysResult(startDate, endDate, startDateDay, endDateDay);
		}
		protected override int GetDaysCore1904(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			DateTime startDate = ConvertToDate(startSerialNumber, context);
			DateTime endDate = ConvertToDate(endSerialNumber, context);
			int startDateDay = startDate.Day;
			int endDateDay = endDate.Day;
			int maxDaysInEndMonth = DaysInMonth(endDate);
			if ((endDateDay == 30 || endDateDay == 31) && maxDaysInEndMonth == 31)
				endDateDay--;
			if (IsLastDayOfFebruary(endDate))
				endDateDay = 30;
			int maxDaysInStartMonth = DaysInMonth(startDate);
			if (startDateDay >= maxDaysInStartMonth - 1 && startDate.Month != 2)
				startDateDay += 30 - maxDaysInStartMonth;
			if (IsLastDayOfFebruary(startDate))
				startDateDay = 30;
			return GetDaysResult(startDate, endDate, startDateDay, endDateDay);
		}
	}
	#endregion
	#region DaysCountBasisActual
	public class DaysCountBasisActual : DayCountBasisBase {
		protected internal override int GetDaysInYear(int year) {
			return DateTime.IsLeapYear(year) ? 366 : 365;
		}
		protected internal override double GetAverageDaysInYear(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			Debug.Assert(startSerialNumber != endSerialNumber);
			DateTime startDate = context.FromDateTimeSerial(startSerialNumber);
			DateTime endDate = context.FromDateTimeSerial(endSerialNumber);
			int startDateYear = startDate.Year;
			int endDateYear = endDate.Year;
			bool isDate1904 = context.DateSystem == DateSystem.Date1904;
			bool maxPossibleYear = endDateYear == DateTime.MaxValue.Year;
			if (OneYearOrLess(startDate, endDate, startSerialNumber, endSerialNumber, isDate1904))
				return GetAverageDaysInOneYear(startDate, endDate, isDate1904);
			DateTime firstDayFirstYear = new DateTime(startDateYear, 1, 1);
			DateTime firstDayAfterLastYear = maxPossibleYear ? new DateTime(endDateYear, 12, 31) : new DateTime(endDateYear + 1, 1, 1);
			if (isDate1904) {
				firstDayFirstYear = ConvertToDate1904(startDate.AddYears(-1), firstDayFirstYear);
				firstDayAfterLastYear = ConvertToDate1904(endDate, firstDayAfterLastYear);
			}
			int yearsCount = firstDayAfterLastYear.Year - firstDayFirstYear.Year;
			double daysInYears = GetDaysInYears(firstDayFirstYear, firstDayAfterLastYear, context);
			if (startDateYear == 1904 && isDate1904)
				daysInYears++;
			if (startDateYear == 1900 || maxPossibleYear)
				daysInYears--;
			return daysInYears / yearsCount;
		}
		bool OneYearOrLess(DateTime startDate, DateTime endDate, int startSerialNumber, int endSerialNumber, bool isDate1904) {
			if (isDate1904) {
				int dayCount = endSerialNumber - startSerialNumber;
				DateTime leapDate = new DateTime(startDate.Year, 2, 28);
				bool leapDayCondition = ((DateTime.IsLeapYear(startDate.Year) && startDate < leapDate) ||
										(DateTime.IsLeapYear(endDate.Year) && endDate > leapDate)) && 
										dayCount == 366;
				return dayCount <= 365 || leapDayCondition;
			}
			return startDate.Year == endDate.Year || (startDate.Year + 1 == endDate.Year && 
				   (startDate.Month > endDate.Month || (startDate.Month == endDate.Month && startDate.Day >= endDate.Day)));
		}
		int GetAverageDaysInOneYear(DateTime startDate, DateTime endDate, bool isDate1904) {
			int leapDay = isDate1904 ? 28 : 29;
			bool startsAndEndsInLeapYear = StartsAndEndsInLeapYear(startDate, endDate, isDate1904);
			return startsAndEndsInLeapYear || DateIncludesLeapDay(startDate, endDate, leapDay) ? 366 : 365;
		}
		bool StartsAndEndsInLeapYear(DateTime startDate, DateTime endDate, bool isDate1904) {
			if (isDate1904) {
				if (IsFirstDayOfYear1904(startDate))
					return DateTime.IsLeapYear(startDate.Year + 1);
				return DateTime.IsLeapYear(endDate.Year) && !IsFirstDayOfYear1904(endDate);
			}
			return DateTime.IsLeapYear(startDate.Year) && startDate.Year == endDate.Year;
		}
		bool IsFirstDayOfYear1904(DateTime date) {
			return date.Month == 12 && date.Day == 31;
		}
		bool DateIncludesLeapDay(DateTime startDate, DateTime endDate, int leapDay) {
			if (DateTime.IsLeapYear(startDate.Year))
				return DateIncludesLeapDayCore(startDate, endDate, new DateTime(startDate.Year, 2, leapDay));
			if (DateTime.IsLeapYear(endDate.Year))
				return DateIncludesLeapDayCore(startDate, endDate, new DateTime(endDate.Year, 2, leapDay));
			return false;
		}
		bool DateIncludesLeapDayCore(DateTime startDate, DateTime endDate, DateTime leapDate) {
			return startDate == leapDate || endDate == leapDate || (startDate < leapDate && endDate > leapDate);
		}
		DateTime ConvertToDate1904(DateTime date, DateTime firstDayOfYear) {
			firstDayOfYear = firstDayOfYear.AddDays(-1);
			return date == firstDayOfYear ? firstDayOfYear.AddYears(1) : firstDayOfYear;
		}
		double GetDaysInYears(DateTime firstDayFirstYear, DateTime firstDayAfterLastYear, WorkbookDataContext context) {
			return DateToSerial(firstDayAfterLastYear, context) - DateToSerial(firstDayFirstYear, context);
		}
		double DateToSerial(DateTime date, WorkbookDataContext context) {
			return context.FromDateTime(date).NumericValue;
		}
		protected internal override int GetDaysInMonth(int month, int year) {
			return DateTime.DaysInMonth(month, year);
		}
		protected internal override int GetMaxDateTimeSerialNumber(DateSystem dateSystem) {
			return dateSystem == DateSystem.Date1900 ? (int)WorkbookDataContext.MaxDateTimeSerialNumber : MaxDateTimeSerialNumber1904;
		}
	}
	#endregion
	#region DaysCountBasisActual365
	public class DaysCountBasisActual365 : DayCountBasisBase {
		protected internal override int GetDaysInYear(int year) {
			return 365;
		}
		protected internal override double GetAverageDaysInYear(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			return 365;
		}
		protected internal override int GetDaysInMonth(int month, int year) {
			return DateTime.DaysInMonth(month, year);
		}
	}
	#endregion
	#region DaysCountBasisActual360
	public class DaysCountBasisActual360 : DayCountBasisBase {
		protected internal override int GetDaysInYear(int year) {
			return 360;
		}
		protected internal override double GetAverageDaysInYear(int startSerialNumber, int endSerialNumber, WorkbookDataContext context) {
			return 360;
		}
	}
	#endregion
}
