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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public abstract class DateTimeFormatter : FormatterBase {
		public static DateTimeFormatter CreateInstance(DateTimeFormatViewModel format) {
			return CreateInstance(Helper.CurrentCulture, format);
		}
		public static DateTimeFormatter CreateInstance(CultureInfo clientCulture, DateTimeFormatViewModel format) {
			switch(format.GroupInterval) {
				case DateTimeGroupInterval.MonthYear:
					return new DateTimeMonthYearFormatter(clientCulture);
				case DateTimeGroupInterval.DayMonthYear:
					return new DateTimeDayMonthYearFormatter(clientCulture, format.DateFormat);
				case DateTimeGroupInterval.Hour:
					return new DateTimeHourFormatter(clientCulture, format.HourFormat);
				case DateTimeGroupInterval.DateHour:
					return new DateTimeDateHourFormatter(clientCulture, format.DateHourFormat);
				case DateTimeGroupInterval.DateHourMinute:
					return new DateTimeDateHourMinuteFormatter(clientCulture, format.DateHourMinuteFormat);
				case DateTimeGroupInterval.DateHourMinuteSecond:
					return new DateTimeDateHourMinuteSecondFormatter(clientCulture, format.DateTimeFormat);
				case DateTimeGroupInterval.Year:
					return new DateTimeYearFormatter(clientCulture, format.YearFormat);
				case DateTimeGroupInterval.Quarter:
					return new DateTimeQuarterFormatter(clientCulture, format.QuarterFormat);
				case DateTimeGroupInterval.Month:
					return new DateTimeMonthFormatter(clientCulture, format.MonthFormat);
				case DateTimeGroupInterval.DayOfWeek:
					return new DateTimeDayOfWeekFormatter(clientCulture, format.DayOfWeekFormat);
				case DateTimeGroupInterval.QuarterYear:
					return new DateTimeQuarterYearFormatter(clientCulture);
				case DateTimeGroupInterval.None:
					switch(format.ExactDateFormat) {
						case ExactDateFormat.Year:
							return new DateTimeExactDateYearFormatter(clientCulture, format.YearFormat);
						case ExactDateFormat.Quarter:
							return new DateTimeQuarterYearFormatter(clientCulture);
						case ExactDateFormat.Month:
							return new DateTimeMonthYearFormatter(clientCulture);
						case ExactDateFormat.Day:
							return new DateTimeDayMonthYearFormatter(clientCulture, format.DateFormat);
						case ExactDateFormat.Hour:
							return new DateTimeDateHourFormatter(clientCulture, format.DateHourFormat);
						case ExactDateFormat.Minute:
							return new DateTimeDateHourMinuteFormatter(clientCulture, format.DateHourMinuteFormat);
						case ExactDateFormat.Second:
							return new DateTimeDateHourMinuteSecondFormatter(clientCulture, format.DateTimeFormat);
						default:
							return new DateTimeIntegerValueFormatter(clientCulture);
					}
				default:
					return new DateTimeIntegerValueFormatter(clientCulture);
			}
		}
		readonly CultureInfo clientCulture;
		protected DateTimeFormatInfo DateTimeFormatInfo { get { return clientCulture.DateTimeFormat; } }
		protected DateTimeFormatter(CultureInfo clientCulture) {
			this.clientCulture = clientCulture;
		}
		protected override string FormatInternal(object value) {
			return String.Format(String.Format("{{0:{0}}}", FormatPattern), value);
		}
	}
	public class DateTimeHourFormatter : DateTimeFormatter {
		public static bool TryConvertIntegerToDateTime(object value, out DateTime dateTimeValue) {
			int intValue;
			if(Int32.TryParse(value.ToString(), out intValue)) {
				dateTimeValue = new DateTime(1, 1, 1, intValue, 0, 0);
				return true;
			}
			dateTimeValue = DateTime.MinValue;
			return false;
		}
		readonly HourFormat hourFormat;
		public override string FormatPattern { 
			get {
				return DateTimeFormatInfo.ShortTimePattern.Replace("m", "0").Replace(":ss", String.Empty);
			}
		}
		public DateTimeHourFormatter(CultureInfo clientCulture, HourFormat hourFormat)
			: base(clientCulture) {
			this.hourFormat = hourFormat;
		}
		protected override string FormatInternal(object value) {
			if(hourFormat == HourFormat.Long) {
				DateTime dateTimeValue;
				if(TryConvertIntegerToDateTime(value, out dateTimeValue))
					return String.Format(String.Format("{{0:{0}}}", FormatPattern), dateTimeValue);
			}
			return value.ToString();
		}
	}
	public class DateTimeIntegerValueFormatter : DateTimeFormatter {
		public override string FormatPattern { get { return "0"; } }
		public DateTimeIntegerValueFormatter(CultureInfo clientCulture) : base(clientCulture) {
		}
	}
	public class DateTimeYearFormatter : DateTimeIntegerValueFormatter {
		readonly YearFormat yearFormat;
		public DateTimeYearFormatter(CultureInfo clientCulture, YearFormat yearFormat) : base(clientCulture) {
			this.yearFormat = yearFormat;
		}
		protected override string FormatInternal(object value) {
			string valueString = value.ToString();
			int intValue;
			if(Int32.TryParse(valueString, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, null, out intValue)) {
				if(yearFormat != YearFormat.Full)
					return String.Format("{0:00}", intValue % 100);
				return intValue.ToString();
			}
			return valueString;
		}
	}
	public class DateTimeMonthFormatter : DateTimeIntegerValueFormatter {
		readonly MonthFormat monthFormat;
		public DateTimeMonthFormatter(CultureInfo clientCulture, MonthFormat monthFormat) : base(clientCulture) {
			this.monthFormat = monthFormat;
		}
		protected override string FormatInternal(object value) {
			string valueString = value.ToString();
			int monthNumber;
			if(Int32.TryParse(valueString, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, null, out monthNumber) && monthNumber > 0 && monthNumber < 14) {
				switch(monthFormat) {
					case MonthFormat.Numeric:
						return monthNumber.ToString();
					case MonthFormat.Abbreviated:
						return DateTimeFormatInfo.GetAbbreviatedMonthName(monthNumber);
					default:
						return DateTimeFormatInfo.GetMonthName(monthNumber);
				}
			}
			return valueString;
		}
	}
	public class DateTimeMonthYearFormatter : DateTimeFormatter {
		public override string FormatPattern { get { return DateTimeFormatInfo.YearMonthPattern; } }
		public DateTimeMonthYearFormatter(CultureInfo clientCulture) : base(clientCulture) {
		}
	}
	public class DateTimeDayMonthYearFormatter : DateTimeFormatter {
		readonly DateFormat dateFormat;
		public override string FormatPattern { get { return dateFormat == DateFormat.Long ? DateTimeFormatInfo.LongDatePattern : DateTimeFormatInfo.ShortDatePattern; } }
		public DateTimeDayMonthYearFormatter(CultureInfo clientCulture, DateFormat dateFormat) : base(clientCulture) {
			this.dateFormat = dateFormat;
		}
	}
	public abstract class DateTimeBaseDateTimeFormatter : DateTimeFormatter {
		readonly DateTimeFormat dateTimeFormat;
		protected DateTimeFormat DateTimeFormat { get { return dateTimeFormat; } }
		protected abstract string TimePattern { get; }
		public override string FormatPattern {
			get {
				string dateFormat = String.Empty;
				switch(dateTimeFormat) {
					case DateTimeFormat.Long:
						dateFormat = DateTimeFormatInfo.LongDatePattern;
						break;
					case DateTimeFormat.TimeOnly:
						break;
					default:
						dateFormat = DateTimeFormatInfo.ShortDatePattern;
						break;
				}
				return !String.IsNullOrEmpty(dateFormat) ? String.Join(" ", new string[] { dateFormat, TimePattern }) : TimePattern;
			}
		}
		protected DateTimeBaseDateTimeFormatter(CultureInfo cultureInfo, DateTimeFormat dateTimeFormat) : base (cultureInfo) {
			this.dateTimeFormat = dateTimeFormat;
		}
	}
	public class DateTimeDateHourFormatter : DateTimeBaseDateTimeFormatter {
		protected override string TimePattern {
			get { return DateTimeFormatInfo.ShortTimePattern.Replace("m", "0").Replace(":ss", String.Empty); }
		}
		public DateTimeDateHourFormatter(CultureInfo clientCulture, DateTimeFormat dateHourFormat)
			: base(clientCulture, dateHourFormat) {
		}
	}
	public class DateTimeDateHourMinuteFormatter : DateTimeBaseDateTimeFormatter {
		protected override string TimePattern {
			get { return DateTimeFormatInfo.ShortTimePattern; }
		}
		public DateTimeDateHourMinuteFormatter(CultureInfo clientCulture, DateTimeFormat dateHourMinuteFormat) : base(clientCulture, dateHourMinuteFormat) {
		}
	}
	public class DateTimeDateHourMinuteSecondFormatter : DateTimeBaseDateTimeFormatter {
		protected override string TimePattern {
			get { return DateTimeFormatInfo.LongTimePattern; }
		}
		public DateTimeDateHourMinuteSecondFormatter(CultureInfo clientCulture, DateTimeFormat dateTimeFormat)
			: base(clientCulture, dateTimeFormat) {
		}
	}
	public class DateTimeExactDateYearFormatter : DateTimeFormatter {
		readonly string formatPattern;
		public DateTimeExactDateYearFormatter(CultureInfo clientCulture, YearFormat yearFormat)
			: base(clientCulture) {
			this.formatPattern = yearFormat == YearFormat.Full ? "yyyy" : "yy";
		}
		public override string FormatPattern { get { return formatPattern; } }
	}
	public class DateTimeDayOfWeekFormatter : DateTimeFormatter {
		readonly DayOfWeekFormat dayOfWeekFormat;
		public override string FormatPattern { get { return String.Empty; } }
		public DateTimeDayOfWeekFormatter(CultureInfo clientCulture, DayOfWeekFormat dayOfWeekFormat) : base(clientCulture) {
			this.dayOfWeekFormat = dayOfWeekFormat;
		}
		protected override string FormatInternal(object value) {
			DayOfWeek dayOfWeek;
			if(Enum.TryParse<DayOfWeek>(value.ToString(), out dayOfWeek)) { 
				switch(dayOfWeekFormat) {
					case DayOfWeekFormat.Numeric:
						return GetDayOfWeekNumber((DayOfWeek)value).ToString();
					case DayOfWeekFormat.Abbreviated:
						return DateTimeFormatInfo.GetAbbreviatedDayName((DayOfWeek)value);
					default:
						return DateTimeFormatInfo.GetDayName((DayOfWeek)value);
				}
			}
			return value.ToString();
		}
		int GetDayOfWeekNumber(DayOfWeek day) {
			DayOfWeek firstDay = DateTimeFormatInfo.FirstDayOfWeek;
			if(firstDay.Equals(day))
				return 1;
			return 1 + (day > firstDay ? (int)day - (int)firstDay : 7 - (int)firstDay + (int)day);
		}
	}
	public abstract class DateTimeQuarterBaseFormatter : DateTimeFormatter {
		protected static string QuarterFormatPattern { get { return DashboardLocalizer.GetString(DashboardStringId.DateTimeQuarterFormat); } }
		protected static string QuarterFormatSpecifier { get { return DashboardLocalizer.GetString(DashboardStringId.DateTimeQuarterFormatSpecifier); } }
		public override string FormatPattern { get { return String.Empty; } }
		protected DateTimeQuarterBaseFormatter(CultureInfo clientCulture) : base(clientCulture) {
		}
	}
	public class DateTimeQuarterFormatter : DateTimeQuarterBaseFormatter {
		readonly QuarterFormat quarterFormat;
		public DateTimeQuarterFormatter(CultureInfo clientCulture, QuarterFormat quarterFormat) : base(clientCulture) {
			this.quarterFormat = quarterFormat;
		}
		protected override string FormatInternal(object value) {
			int quarterNumber;
			if(Int32.TryParse(value.ToString(), NumberStyles.Integer | NumberStyles.AllowDecimalPoint, null, out quarterNumber) && quarterNumber > 0 && quarterNumber < 5)
				return quarterFormat == QuarterFormat.Full ? quarterNumber.ToString(QuarterFormatter.FormatDateTime(quarterNumber, QuarterFormatSpecifier, QuarterFormatPattern)) :
															 quarterNumber.ToString();
			return value.ToString();
		}
	}
	public class DateTimeQuarterYearFormatter : DateTimeQuarterBaseFormatter {
		public DateTimeQuarterYearFormatter(CultureInfo clientCulture) : base(clientCulture) {
		}
		protected override string FormatInternal(object value) {
			DateTime dateTimeValue;
			if(DateTime.TryParse(value.ToString(), out dateTimeValue))
				return dateTimeValue.ToString(QuarterFormatter.FormatDateTime(dateTimeValue, QuarterFormatSpecifier, QuarterFormatPattern) + " yyyy");
			return value.ToString();
		}
	}
}
