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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionCoupNum
	public class FunctionCoupNum : FunctionCouponBase {
		#region Properties
		public override string Name { get { return "COUPNUM"; } }
		public override int Code { get { return 0x01C8; } }
		#endregion
		protected override VariantValue GetResult(int settlement, int maturity, CouponFrequency frequency, int basisId, WorkbookDataContext context) {
			DateTime startDate = ConvertDate(settlement, context);
			DateTime endDate = ConvertDate(maturity, context);
			double monthCount = GetMonthsCount(endDate.Year - startDate.Year, endDate.Month - startDate.Month);
			if (!CheckValidDates(startDate, endDate, (int)frequency))
				return VariantValue.ErrorNumber;
			if (monthCount % 3 == 0 && AddMonthCondition(startDate, endDate, context.DateSystem))
				monthCount++;
			return GetResultCore(Math.Ceiling(monthCount / 3.0), frequency);
		}
		double GetResultCore(double yearQuarters, CouponFrequency frequency) {
			if (frequency == CouponFrequency.Annual)
				return Math.Ceiling(yearQuarters / 4.0);
			if (frequency == CouponFrequency.Semiannual)
				return Math.Ceiling(yearQuarters / 2.0);
			return yearQuarters;
		}
		bool AddMonthCondition(DateTime startDate, DateTime endDate, DateSystem dateSystem) {
			if (dateSystem == DateSystem.Date1904) {
				int daysInStartMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
				int daysInEndMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
				if (daysInStartMonth == startDate.Day + 1 && !IsLastDayOfMonth(endDate))
					return false;
				if (daysInEndMonth == endDate.Day + 1 && !IsLastDayOfMonth(startDate))
					return true;
			}
			return EndDayIsGreaterThanStartDay(startDate, endDate);
		}
		bool CheckValidDates(DateTime startDate, DateTime endDate, int frequency) {
			if (startDate.Year == 1899)
				return false;
			if (startDate.Year == 1900)
				return Year1900Case(startDate, endDate, 12 / frequency, frequency);
			return true;
		}
		bool Year1900Case(DateTime startDate, DateTime endDate, int monthsInPeriod, int frequency) {
			int startDateMonth = startDate.Month;
			int endDateMonth = endDate.Month;
			if (startDateMonth > monthsInPeriod)
				return true;
			bool endMonthHaveOnlyValidDates = CheckEndMonthConditions(startDateMonth, endDateMonth, monthsInPeriod, frequency);
			bool endMonthHaveSomeValidDates = CheckEndMonthConditionsCore(startDateMonth, endDateMonth, monthsInPeriod, frequency);
			return endMonthHaveOnlyValidDates || (endMonthHaveSomeValidDates && !EndDayIsGreaterThanStartDay(startDate, endDate));
		}
		bool CheckEndMonthConditions(int startDateMonth, int endDateMonth, int monthsInPeriod, int frequency) {
			for (int i = startDateMonth - 1; i > 0; i--) {
				if (CheckEndMonthConditionsCore(i, endDateMonth, monthsInPeriod, frequency))
					return true;
			}
			return false;
		}
		bool CheckEndMonthConditionsCore(int startDateMonth, int endDateMonth, int monthsInPeriod, int frequency) {
			for (int i = 0; i < frequency; i++) {
				if (endDateMonth == startDateMonth + monthsInPeriod * i)
					return true;
			}
			return false;
		}
		int GetMonthsCount(int yearDifference, int monthDifference) {
			if (yearDifference == 0 && monthDifference == 0)
				return 1;
			return 12 * yearDifference + monthDifference;
		}
		bool EndDayIsGreaterThanStartDay(DateTime startDate, DateTime endDate) {
			if (IsLastDayOfMonth(endDate))
				return !IsLastDayOfMonth(startDate);
			return endDate.Day > startDate.Day;
		}
		bool IsLastDayOfMonth(DateTime date) {
			return DateTime.DaysInMonth(date.Year, date.Month) == date.Day;
		}
		DateTime ConvertDate(int serialNumber, WorkbookDataContext context) {
			if (context.DateSystem == DateSystem.Date1900 && serialNumber == 60)
				return new DateTime(1900, 2, 28);
			return context.FromDateTimeSerial(serialNumber);
		}
	}
	#endregion
}
