#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public static class DateTimeFormatCaptionProvider {
		public static string GetDimensionDateTimeFormatCaption(Dimension dimension) {
			switch(dimension.DateTimeGroupInterval) {
				case DateTimeGroupInterval.Year:
					return GetCaption(dimension.DateTimeFormat.YearFormat);
				case DateTimeGroupInterval.Quarter:
					return GetCaption(dimension.DateTimeFormat.QuarterFormat);
				case DateTimeGroupInterval.Month:
					return GetCaption(dimension.DateTimeFormat.MonthFormat);
				case DateTimeGroupInterval.DayOfWeek:
					return GetCaption(dimension.DateTimeFormat.DayOfWeekFormat);
				case DateTimeGroupInterval.DayMonthYear:
					return GetCaption(dimension.DateTimeFormat.DateFormat);
				case DateTimeGroupInterval.DateHour:
					return GetCaption(dimension.DateTimeFormat.DateHourFormat);
				case DateTimeGroupInterval.Hour:
					return GetCaption(dimension.DateTimeFormat.HourFormat);
				case DateTimeGroupInterval.DateHourMinute:
					return GetCaption(dimension.DateTimeFormat.DateHourMinuteFormat);
				case DateTimeGroupInterval.DateHourMinuteSecond:
					return GetCaption(dimension.DateTimeFormat.DateTimeFormat);
				case DateTimeGroupInterval.None:
					return GetDimensionExactDateFormatCaption(dimension.DateTimeFormat);
				default:
					return String.Empty;
			}
		}
		static string GetDimensionExactDateFormatCaption(DataItemDateTimeFormat format) {
			string exactDateFormatCaption = GetCaption(format.ExactDateFormat);
			string formatCaption = string.Empty;
			switch(format.ExactDateFormat) {
				case ExactDateFormat.Year:
					formatCaption = GetCaption(format.YearFormat);
					break;
				case ExactDateFormat.Quarter:
				case ExactDateFormat.Month:
					return exactDateFormatCaption;
				case ExactDateFormat.Day:
					formatCaption = GetCaption(format.DateFormat);
					break;
				case ExactDateFormat.Hour:
					formatCaption = GetCaption(format.DateHourFormat);
					break;
				case ExactDateFormat.Minute:
					formatCaption = GetCaption(format.DateHourMinuteFormat);
					break;
				case ExactDateFormat.Second:
					formatCaption = GetCaption(format.DateTimeFormat);
					break;
			}
			return string.Format(
				DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateDateTimeFormatPattern),
				exactDateFormatCaption, formatCaption
			);
		}
		public static string GetCaption(ExactDateFormat ExactDateFormat) {
			switch(ExactDateFormat) {
				case ExactDateFormat.Year:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatYear);
				case ExactDateFormat.Quarter:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatQuarter);
				case ExactDateFormat.Month:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatMonth);
				case ExactDateFormat.Day:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatDay);
				case ExactDateFormat.Hour:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatHour);
				case ExactDateFormat.Minute:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatMinute);
				case ExactDateFormat.Second:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.ExactDateFormatSecond);
				default:
					return String.Empty;
			}
		}
		public static string GetMeasureDateTimeFormatCaption(Measure measure) {
			return GetCaption(measure.DateTimeFormat.DateFormat);
		}
		public static string GetCaption(YearFormat yearFormat) {
			if(yearFormat == YearFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatYearFormatDefaultCaption);
			if(yearFormat == YearFormat.Abbreviated)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatYearFormatAbbreviatedCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatYearFormatFullCaption);
		}
		public static string GetCaption(QuarterFormat quarterFormat) {
			if(quarterFormat == QuarterFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatQuarterFormatDefaultCaption);
			if(quarterFormat == QuarterFormat.Numeric)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatQuarterFormatNumericCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatQuarterFormatFullCaption);
		}
		public static string GetCaption(MonthFormat monthFormat) {
			if(monthFormat == MonthFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatMonthFormatDefaultCaption);
			if(monthFormat == MonthFormat.Numeric)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatMonthFormatNumericCaption);
			if(monthFormat == MonthFormat.Abbreviated)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatMonthFormatAbbreviatedCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatMonthFormatFullCaption);
		}
		public static string GetCaption(DayOfWeekFormat dayOfWeekFormat) {
			if(dayOfWeekFormat == DayOfWeekFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatDefaultCaption);
			if(dayOfWeekFormat == DayOfWeekFormat.Numeric)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatNumericCaption);
			if(dayOfWeekFormat == DayOfWeekFormat.Abbreviated)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatAbbreviatedCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDayOfWeekFormatFullCaption);
		}
		public static string GetCaption(DateFormat dateFormat) {
			if(dateFormat == DateFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateFormatDefaultCaption);
			if(dateFormat == DateFormat.Short)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateFormatShortCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateFormatLongCaption);
		}
		public static string GetCaption(DateTimeFormat dateTimeFormat) {
			if(dateTimeFormat == DateTimeFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatDefaultCaption);
			if(dateTimeFormat == DateTimeFormat.Short)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatShortCaption);
			if(dateTimeFormat == DateTimeFormat.TimeOnly)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatTimeOnlyCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatLongCaption);
		}
		public static string GetCaption(HourFormat hourFormat) {
			if (hourFormat == HourFormat.Default)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatDefaultCaption);
			if (hourFormat == HourFormat.Short)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatShortCaption);
			return DashboardWinLocalizer.GetString(DashboardWinStringId.DateTimeFormatDateTimeFormatLongCaption);
		}
	}
}
