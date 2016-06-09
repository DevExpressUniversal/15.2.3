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

using DevExpress.XtraPivotGrid;
using System;
using DevExpress.XtraPivotGrid.Utils.DateHelpers;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Utils;
using System.Globalization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Compatibility.System;
namespace DevExpress.PivotGrid.Utils {
	static class GroupIntervalHelper {
		readonly static string UnexpectedGroupInterval = "Unexpected GroupInterval";
		public static bool IsNumericInterval(PivotGroupInterval interval) {
			return interval == PivotGroupInterval.Numeric ||
					interval == PivotGroupInterval.YearAge || interval == PivotGroupInterval.MonthAge ||
					interval == PivotGroupInterval.WeekAge || interval == PivotGroupInterval.DayAge;
		}
		public static bool IsStringInterval(PivotGroupInterval interval) {
			return interval == PivotGroupInterval.Alphabetical;
		}
		public static bool IsDateTimeInterval(PivotGroupInterval interval) {
			switch(interval) {
				case PivotGroupInterval.Date:
				case PivotGroupInterval.DateDay:
				case PivotGroupInterval.DateDayOfWeek:
				case PivotGroupInterval.DateDayOfYear:
				case PivotGroupInterval.DateMonth:
				case PivotGroupInterval.DateQuarter:
				case PivotGroupInterval.DateWeekOfMonth:
				case PivotGroupInterval.DateWeekOfYear:
				case PivotGroupInterval.DateYear:
				case PivotGroupInterval.Hour:
				case PivotGroupInterval.Minute:
				case PivotGroupInterval.Second:
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
		public static object GetValue(PivotGroupInterval interval, object value, int numericRange, DateTime baseTime) {
			try {
				if(interval == PivotGroupInterval.Default)
					return value;
				if(IsStringInterval(interval))
					return GetStringValue(interval, value);
				IConvertible convertible = value as IConvertible;
				if(convertible == null)
					return null;
				if(IsNumericInterval(interval))
					return GetNumericValue(interval, convertible, numericRange, baseTime);
				if(value == DBNull.Value)
					return null;
				return GetOtherValue(interval, convertible);
			} catch {  
			}
			return null;
		}
		public static UnboundColumnType GetUnboundType(PivotGroupInterval interval) {
			if(interval == PivotGroupInterval.Alphabetical)
				return UnboundColumnType.String;
			if(
				interval == PivotGroupInterval.Date ||
				interval == PivotGroupInterval.DateMonthYear ||
				interval == PivotGroupInterval.DateQuarterYear ||
				interval == PivotGroupInterval.DateHour ||
				interval == PivotGroupInterval.DateHourMinute ||
				interval == PivotGroupInterval.DateHourMinuteSecond
			)
				return UnboundColumnType.DateTime;
			if(interval == PivotGroupInterval.Custom)
				return UnboundColumnType.Object;
			if(interval == PivotGroupInterval.DateDayOfWeek)
				return UnboundColumnType.Object;
			return UnboundColumnType.Integer;
		}
		public static Type GetValueType(PivotGroupInterval interval) {
			switch(interval) {
				case PivotGroupInterval.Date:
				case PivotGroupInterval.DateMonthYear:
				case PivotGroupInterval.DateQuarterYear:
				case PivotGroupInterval.DateHour:
				case PivotGroupInterval.DateHourMinute:
				case PivotGroupInterval.DateHourMinuteSecond:
					return typeof(DateTime);
				case PivotGroupInterval.Alphabetical:
					return typeof(string);
				case PivotGroupInterval.Custom:
				case PivotGroupInterval.Default:
					return typeof(object);
				case PivotGroupInterval.DateDayOfWeek:
					return typeof(DayOfWeek);
				default:
					return typeof(int);
			}
		}
		public static string GetTextValue(PivotGroupInterval interval, CultureInfo cultureInfo, object value, int numericRange, FormatInfo format, FormatInfo dateFormat) {
			if(IsNumericInterval(interval))
				return string.Format("{0} - {1}", format.GetDisplayText((int)value * numericRange), format.GetDisplayText(((int)value + 1) * numericRange - 1));
			if(format.IsEmpty) {
				FormatInfo tempDateFormat = new FormatInfo();
				tempDateFormat.FormatType = FormatType.DateTime;
				switch(interval) {
					case PivotGroupInterval.DateDayOfWeek:
						return cultureInfo.DateTimeFormat.DayNames[(int)value];
					case PivotGroupInterval.DateMonth:
						if(value is int && (int)value >= 1 && (int)value <= 12)
							return cultureInfo.DateTimeFormat.GetMonthName((int)value);
						break;
					case PivotGroupInterval.Date:
						return dateFormat.GetDisplayText(value);
					case PivotGroupInterval.DateMonthYear:
						tempDateFormat.FormatString = CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern;
						return tempDateFormat.GetDisplayText(value);
					case PivotGroupInterval.DateQuarterYear:
						if(value is DateTime) {
							DateTime dateTime = (DateTime)value;
							string predefinedQuarterFormat = PivotGridLocalizer.GetString(PivotGridStringId.DateTimeQuarterFormat);
							return dateTime.ToString(QuarterFormatter.FormatDateTime(dateTime, "QQ ", predefinedQuarterFormat) + "yyyy");
						}
						break;
					case PivotGroupInterval.DateHour:
					case PivotGroupInterval.DateHourMinute:
						tempDateFormat.FormatString = "g";
						return tempDateFormat.GetDisplayText(value);
					case PivotGroupInterval.DateHourMinuteSecond:
						tempDateFormat.FormatString = "G";
						return tempDateFormat.GetDisplayText(value);
				}
			}
			else if(interval == PivotGroupInterval.DateMonth && value is int && (int)value >= 1 && (int)value <= 12) {
				return format.GetDisplayText(new DateTime(2000, (int)value, 1));
			}
			return null;
		}
		static string GetStringValue(PivotGroupInterval interval, object value) {
			if(interval == PivotGroupInterval.Alphabetical) {
				string text = value.ToString();
				return text.Length > 1 ? text.Substring(0, 1) : text;
			} else {
				throw new Exception(UnexpectedGroupInterval);
			}
		}
		static int GetNumericValue(PivotGroupInterval interval, IConvertible value, int numericRange, DateTime baseTime) {
			int intValue = 0;
			if(interval == PivotGroupInterval.Numeric) {
				intValue = (int)value.ToDecimal(null);
			} else {
				DateTime dateTime = value.ToDateTime(null);
				if(interval == PivotGroupInterval.YearAge)
					intValue = DateHelper.GetFullYears(dateTime, baseTime);
				if(interval == PivotGroupInterval.MonthAge)
					intValue = DateHelper.GetFullMonths(dateTime, baseTime);
				if(interval == PivotGroupInterval.WeekAge)
					intValue = DateHelper.GetFullWeeks(dateTime, baseTime);
				if(interval == PivotGroupInterval.DayAge)
					intValue = DateHelper.GetFullDays(dateTime, baseTime);
			}
			if(intValue < 0) {
				if(Math.Abs(intValue) % numericRange != 0) {
					intValue = -((Math.Abs(intValue) / numericRange + 1) * numericRange);
				}
			}
			return (int)(intValue / numericRange);
		}
		static object GetOtherValue(PivotGroupInterval interval, IConvertible value) {
			DateTime dateTime = value.ToDateTime(null);
			if(interval == PivotGroupInterval.Date)
				return dateTime.Date;
			if(interval == PivotGroupInterval.DateYear)
				return dateTime.Year;
			if(interval == PivotGroupInterval.DateMonth)
				return dateTime.Month;
			if(interval == PivotGroupInterval.DateQuarter)
				return (dateTime.Month - 1) / 3 + 1;
			if(interval == PivotGroupInterval.DateDay)
				return dateTime.Day;
			if(interval == PivotGroupInterval.DateDayOfWeek)
				return dateTime.DayOfWeek;
			if(interval == PivotGroupInterval.DateDayOfYear)
				return dateTime.DayOfYear;
			if(interval == PivotGroupInterval.DateWeekOfMonth)
				return DateHelper.GetWeekOfMonth(dateTime);
			if(interval == PivotGroupInterval.DateWeekOfYear)
				return DateHelper.GetWeekOfYear(dateTime);
			if(interval == PivotGroupInterval.Hour)
				return dateTime.Hour;
			if(interval == PivotGroupInterval.Minute)
				return dateTime.Minute;
			if(interval == PivotGroupInterval.Second)
				return dateTime.Second;
			if(interval == PivotGroupInterval.DateMonthYear)
				return new DateTime(dateTime.Year, dateTime.Month, 1);
			if(interval == PivotGroupInterval.DateQuarterYear)
				return new DateTime(dateTime.Year, (dateTime.Month - 1) / 3 * 3 + 1, 1);
			if(interval == PivotGroupInterval.DateHour)
				return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
			if(interval == PivotGroupInterval.DateHourMinute)
				return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
			if(interval == PivotGroupInterval.DateHourMinuteSecond)
				return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
			return null;
		}
	}
}
