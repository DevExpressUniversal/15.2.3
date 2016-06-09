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
using System.Text.RegularExpressions;
using DevExpress.XtraScheduler.Native;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Native {
	public static class iCalendarSR {
		public const string DXPropertyIdentifier = "DEVEXPRESS";
		public const string DXProductIdentifier = DevExpress.XtraScheduler.VCalendar.VCalendarConsts.DefaultCalendarProductID;
		public const string MicrosoftCDO = "MICROSOFT-CDO";
		public const string OutlookStatusProperty = "BUSYSTATUS";
		public const string ResourceIdProperty = "RESOURCEID";
		public const string CustomFieldProperty = "CUSTOMFIELD";
		public const string XPrefix = "X-";
		public const string AppointmentStatusProperty = "STATUS";
		public const string AppointmentLabelProperty = "LABEL";
	}
	#region iCalendarConvert
	public static class iCalendarConvert {
		#region Static
		static readonly TimeSpan WeekDuration = TimeSpan.FromDays(7);
		static Dictionary<string, AppointmentStatusType> appointmentStatusTypeHash = new Dictionary<string, AppointmentStatusType>();
		static Dictionary<AppointmentStatusType, string> eventStatusTypeHash = new Dictionary<AppointmentStatusType, string>();
		static Dictionary<string, DayOfWeek> dayOfWeekHT = new Dictionary<string, DayOfWeek>();
		static Dictionary<DayOfWeek, string> dayOfWeekInvertHT = new Dictionary<DayOfWeek, string>();
		static iCalendarConvert() {
			appointmentStatusTypeHash.Add("FREE", AppointmentStatusType.Free);
			appointmentStatusTypeHash.Add("TENTATIVE", AppointmentStatusType.Tentative);
			appointmentStatusTypeHash.Add("BUSY", AppointmentStatusType.Busy);
			appointmentStatusTypeHash.Add("OOF", AppointmentStatusType.OutOfOffice);
			foreach (string key in appointmentStatusTypeHash.Keys)
				eventStatusTypeHash.Add(appointmentStatusTypeHash[key], key);
			eventStatusTypeHash.Add(AppointmentStatusType.Custom, "FREE");
			dayOfWeekHT.Add("SU", DayOfWeek.Sunday);
			dayOfWeekHT.Add("MO", DayOfWeek.Monday);
			dayOfWeekHT.Add("TU", DayOfWeek.Tuesday);
			dayOfWeekHT.Add("WE", DayOfWeek.Wednesday);
			dayOfWeekHT.Add("TH", DayOfWeek.Thursday);
			dayOfWeekHT.Add("FR", DayOfWeek.Friday);
			dayOfWeekHT.Add("SA", DayOfWeek.Saturday);
			foreach (string key in dayOfWeekHT.Keys)
				dayOfWeekInvertHT.Add(dayOfWeekHT[key], key);
		}
		#endregion
		#region ToUtcOffset
		public static TimeSpan ToUtcOffset(string value) {
			if (string.IsNullOrEmpty(value))
				return iCalendarUtils.DefaultTimeSpan;
			XtraSchedulerDebug.Assert(value.StartsWith("+") || value.StartsWith("-"));
			XtraSchedulerDebug.Assert(value.Length == 5 || value.Length == 7);
			Match match = Regex.Match(value, @"(\+|-)(\d{2})(\d{2})(\d{2})?");
			if (match.Success) {
				int hour = Convert.ToInt32(match.Groups[2].Value);
				int min = Convert.ToInt32(match.Groups[3].Value);
				int sec = match.Groups[4].Success ? Convert.ToInt32(match.Groups[4].Value) : 0;
				TimeSpan result = new TimeSpan(hour, min, sec);
				if (match.Groups[1].Value == "-")
					result = result.Negate();
				return result;
			}
			Exceptions.ThrowArgumentException("value", value);
			return iCalendarUtils.DefaultTimeSpan;
		}
		#endregion
		#region SeparateParameterValues
		public static string[] SeparateParameterValues(string parameterValues) {
			if (String.IsNullOrEmpty(parameterValues))
				return new string[] { };
			return SplitStringValue(parameterValues, iCalendarSymbols.ParamValueSeparatorChar);
		}
		#endregion
		#region SplitStringValue
		static string[] SplitStringValue(string stringValue, Char separator) {
			return stringValue.Split(new char[] { separator });
		}
		#endregion
		#region ToTimeIntervalCollection
		public static TimeIntervalCollection ToTimeIntervalCollection(string stringValue) {
			TimeIntervalCollection result = new TimeIntervalCollection();
			string[] items = stringValue.Split(new char[] { ',' });
			int count = items.Length;
			for (int i = 0; i < count; i++) {
				string item = items[i];
				result.Add(ToTimeInterval(item));
			}
			return result;
		}
		static internal TimeInterval ToTimeInterval(string stringValue) {
			string[] items = stringValue.Split(new char[] { '/' });
			if (items.Length > 2)
				return TimeInterval.Empty;
			DateTime start = ToDateTime(items[0]);
			if (items.Length == 1)
				return new TimeInterval(start, TimeSpan.Zero);
			TimeSpan duration = TimeSpan.Zero;
			if (TryParseDuration(items[1], out duration))
				return new TimeInterval(start, duration);
			DateTime end = ToDateTime(items[1]);
			return new TimeInterval(start, end);
		}
		#endregion
		internal static int GetRegExpGroupIntValue(Group group) {
			if (group.Success)
				return Convert.ToInt32(group.Value);
			return 0;
		}
		internal static bool IsRegExpGroupNegative(Group group) {
			if (group.Success)
				return group.Value == "+" ? false : true;
			return false;
		}
		#region TryParseDuration
		static internal bool TryParseDuration(string stringValue, out TimeSpan value) {
			Match match = Regex.Match(stringValue, @"^(?<sign>\+|-)?P(((?<weeks>\d+)W)|(?<date>((?<days>\d+)D)?(?<time>T((?<hours>\d+)H)?((?<minutes>\d+)M)?((?<seconds>\d+)S)?)?))$");
			value = iCalendarUtils.DefaultTimeSpan;
			if (match.Success == false)
				return false;
			GroupCollection groups = match.Groups;
			Group signGroup = groups["sign"];
			Group weekGroup = groups["weeks"];
			Group dateGroup = groups["date"];
			Group dayGroup = groups["days"];
			Group hourGroup = groups["hours"];
			Group timeGroup = groups["time"];
			Group minuteGroup = groups["minutes"];
			Group secondGroup = groups["seconds"];
			int days = 0;
			int hours = 0;
			int minutes = 0;
			int seconds = 0;
			if (weekGroup.Success)
				days = GetRegExpGroupIntValue(weekGroup) * 7;
			else if (dateGroup.Success) {
				days = GetRegExpGroupIntValue(dayGroup);
				if (timeGroup.Success) {
					hours = GetRegExpGroupIntValue(hourGroup);
					minutes = GetRegExpGroupIntValue(minuteGroup);
					seconds = GetRegExpGroupIntValue(secondGroup);
				}
			}
			value = new TimeSpan(days, hours, minutes, seconds);
			if (signGroup.Success && signGroup.Value == "-")
				value = value.Negate();
			return true;
		}
		#endregion
		#region FromDuration
		internal static string FromDuration(TimeSpan value) {
			TimeSpan duration = (value.Ticks < 0) ? value.Negate() : value;
			string prefixString = (value.Ticks < 0) ? "-P" : "P";
			if (IsWeekDuration(duration))
				return String.Format("{0}{1}W", prefixString, duration.Days / 7);
			StringBuilder result = new StringBuilder(prefixString);
			string dayPart = GetDurationDayPart(duration);
			string timePart = GetDurationTimePart(duration);
			result.Append(dayPart);
			result.Append(timePart);
			return result.ToString();
		}
		static bool IsWeekDuration(TimeSpan duration) {
			if (duration.Ticks % WeekDuration.Ticks == 0)
				return true;
			return false;
		}
		static string GetDurationDayPart(TimeSpan duration) {
			return (duration.Days != 0) ? String.Format("{0}D", duration.Days) : String.Empty;
		}
		static string GetDurationTimePart(TimeSpan duration) {
			StringBuilder timePart = new StringBuilder();
			if (duration.Hours != 0)
				timePart.AppendFormat("{0}H", duration.Hours);
			if (duration.Minutes != 0 || (duration.Hours != 0 && duration.Seconds != 0))
				timePart.AppendFormat("{0}M", duration.Minutes);
			if (duration.Seconds != 0)
				timePart.AppendFormat("{0}S", duration.Seconds);
			string result = timePart.ToString();
			if (String.IsNullOrEmpty(result))
				return String.Empty;
			return "T" + result;
		}
		#endregion
		#region ToDateTime
		static internal DateTime ToDateTime(string stringValue) {
			DateTime dateTime = DateTime.MinValue;
			if (TryParseDate(stringValue, out dateTime))
				return dateTime;
			return ToDateTimeCore(stringValue);
		}
		#endregion
		#region ToDateTimeCore
		static DateTime ToDateTimeCore(string value) {
			if (string.IsNullOrEmpty(value))
				return DateTime.MinValue;
			try {
				bool utc = DevExpress.XtraScheduler.VCalendar.VCalendarConvert.IsUtcDateTime(value);
				DateTime result = DateTime.MinValue;
				bool isParsed = TryParseDateTime(value, out result);
				if (!isParsed)
					throw new iCalendarException(String.Format("Can't parse date {0}.", value));
				if (utc) 
					return new DateTime(result.Ticks, DateTimeKind.Utc);
				else
					return new DateTime(result.Ticks, DateTimeKind.Local);
			}
			catch {
				throw new iCalendarParseErrorException();
			}
		}
		#endregion
		#region TryParseDate
		static internal bool TryParseDate(string stringValue, out DateTime value) {
			Match match = Regex.Match(stringValue, @"(\d{4})(\d{2})(\d{2})$");
			value = DateTime.MinValue;
			if (match.Success == false)
				return false;
			GroupCollection groups = match.Groups;
			int year = GetRegExpGroupIntValue(groups[1]);
			int month = GetRegExpGroupIntValue(groups[2]);
			int day = GetRegExpGroupIntValue(groups[3]);
			value = new DateTime(year, month, day);
			return true;
		}
		#endregion
		static internal bool TryParseDateTime(string stringValue, out DateTime value) {
			Match match = Regex.Match(stringValue, @"(\d{4})(\d{2})(\d{2})(T)(\d{2})(\d{2})(\d{2})(Z)?$");
			value = DateTime.MinValue;
			if (match.Success == false)
				return false;
			GroupCollection groups = match.Groups;
			int year = GetRegExpGroupIntValue(groups[1]);
			int month = GetRegExpGroupIntValue(groups[2]);
			int day = GetRegExpGroupIntValue(groups[3]);
			int hour = GetRegExpGroupIntValue(groups[5]);
			int minute = GetRegExpGroupIntValue(groups[6]);
			int second = GetRegExpGroupIntValue(groups[7]);
			if (year <= 0)
				year = 1;
			value = new DateTime(year, month, day, hour, minute, second);
			return true;
		}
		#region ToInt
		public static int ToInt(string value, string errorMessage) {
			try {
				return Convert.ToInt32(value);
			}
			catch (FormatException) {
				throw new iCalendarParseErrorException();
			}
		}
		#endregion
		#region ToDayOfWeek
		public static DayOfWeek ToDayOfWeek(string value) {
			value = value.ToUpper();
			if (dayOfWeekHT.ContainsKey(value))
				return dayOfWeekHT[value];
			throw new iCalendarParseErrorException();
		}
		#endregion
		#region FromDayOfWeek
		public static string FromDayOfWeek(DayOfWeek weekDay) {
			if (dayOfWeekInvertHT.ContainsKey(weekDay))
				return dayOfWeekInvertHT[weekDay];
			return String.Empty;
		}
		#endregion
		#region ToAppointmentStatus
		public static AppointmentStatusType ToAppointmentStatus(string stringValue) {
			stringValue = stringValue.ToUpper();
			if (appointmentStatusTypeHash.ContainsKey(stringValue))
				return (AppointmentStatusType)appointmentStatusTypeHash[stringValue];
			return AppointmentStatusType.Custom;
		}
		#endregion
		#region FromAppointmentStatus
		public static string FromAppointmentStatus(AppointmentStatusType value) {
			if (eventStatusTypeHash.ContainsKey(value))
				return (string)eventStatusTypeHash[value];
			return String.Empty;
		}
		#endregion
		#region FromDateTime
		public static string FromDateTime(DateTime value, bool utc) {
			return DevExpress.XtraScheduler.VCalendar.VCalendarConvert.FromDateTime(value, utc);
		}
		#endregion
		#region FromDate
		public static string FromDate(DateTime value) {
			return String.Format("{0}{1:d2}{2:d2}", value.Year, value.Month, value.Day);
		}
		#endregion
		#region UnescapeString
		public static string UnescapeString(string str) {
			if (String.IsNullOrEmpty(str))
				return str;
			StringBuilder sb = new StringBuilder(str);
			sb.Replace(@"\\", "\\");
			sb.Replace(@"\;", ";");
			sb.Replace(@"\,", ",");
			sb.Replace(@"\N", "\r\n");
			sb.Replace(@"\n", "\r\n");
			return sb.ToString();
		}
		#endregion
		#region EscapeString
		public static string EscapeString(string str) {
			if (String.IsNullOrEmpty(str))
				return str;
			StringBuilder sb = new StringBuilder(str);
			sb.Replace("\\", @"\\");
			sb.Replace(";", @"\;");
			sb.Replace(",", @"\,");
			sb.Replace("\r\n", @"\n");
			sb.Replace("\n", @"\n");
			return sb.ToString();
		}
		#endregion
		#region DquoteString
		public static string DquoteString(string str) {
			if (String.IsNullOrEmpty(str))
				return str;
			return String.Format(@"""{0}""", str);
		}
		#endregion
		#region UnquoteString
		public static string UnquoteString(string str) {
			if (Regex.Match(str, @"^"".+""$").Success)
				return str.Substring(1, str.Length - 2);
			return str;
		}
		#endregion
		#region FromUtcOffset
		public static string FromUtcOffset(TimeSpan value) {
			string prefix = "+";
			if (value.Ticks < 0) {
				prefix = "-";
				value = value.Negate();
			}
			DateTime dateTime = DateTimeHelper.ToDateTime(value);
			string stringValue = String.Format("{0:d2}{1:d2}", dateTime.Hour, dateTime.Minute);
			return prefix + stringValue;
		}
		#endregion
	}
	#endregion
	#region iCalendarUtils
	public static class iCalendarUtils {
		public static readonly DateTime DefaultDateTime = DateTime.MinValue;
		public static readonly TimeSpan DefaultTimeSpan = TimeSpan.Zero;
		public static readonly VRecurrenceFrequency DefaultFrequency = VRecurrenceFrequency.Daily;
		public static readonly int DefaultIntZeroValue = 0;
		public static readonly int DefaultIntervalValue = 1;
		public static readonly string ClientTimeZoneIdentifier = String.Empty;
		#region GetCustomPropertyName
		public static string CreateXString(String str) {
			return iCalendarSR.XPrefix + str;
		}
		#endregion
	}
	#endregion
	#region VRecurrenceConverter (abstract class)
	public abstract class VRecurrenceConverter {
		#region Static
		static Dictionary<VRecurrenceFrequency, RecurrenceType> freqTypeHT;
		static Dictionary<RecurrenceType, VRecurrenceFrequency> freqTypeInvertHT;
		static Dictionary<int, WeekOfMonth> occurrenceNumberHT;
		static Dictionary<WeekOfMonth, int> occurrenceNumberInvertHT;
		static VRecurrenceConverter() {
			freqTypeHT = new Dictionary<VRecurrenceFrequency, RecurrenceType>();
			freqTypeInvertHT = new Dictionary<RecurrenceType, VRecurrenceFrequency>();
			occurrenceNumberHT = new Dictionary<int, WeekOfMonth>();
			occurrenceNumberInvertHT = new Dictionary<WeekOfMonth, int>();
			freqTypeHT.Add(VRecurrenceFrequency.Hourly, RecurrenceType.Hourly);
			freqTypeHT.Add(VRecurrenceFrequency.Minutely, RecurrenceType.Minutely);
			freqTypeHT.Add(VRecurrenceFrequency.Daily, RecurrenceType.Daily);
			freqTypeHT.Add(VRecurrenceFrequency.Weekly, RecurrenceType.Weekly);
			freqTypeHT.Add(VRecurrenceFrequency.Monthly, RecurrenceType.Monthly);
			freqTypeHT.Add(VRecurrenceFrequency.Yearly, RecurrenceType.Yearly);
			foreach (VRecurrenceFrequency key in freqTypeHT.Keys)
				freqTypeInvertHT.Add(freqTypeHT[key], key);
			occurrenceNumberHT.Add(1, WeekOfMonth.First);
			occurrenceNumberHT.Add(2, WeekOfMonth.Second);
			occurrenceNumberHT.Add(3, WeekOfMonth.Third);
			occurrenceNumberHT.Add(4, WeekOfMonth.Fourth);
			occurrenceNumberHT.Add(-1, WeekOfMonth.Last);
			foreach (int key in occurrenceNumberHT.Keys)
				occurrenceNumberInvertHT.Add(occurrenceNumberHT[key], key);
		}
		public static VRecurrenceConverter CreateInstance(VRecurrenceFrequency frequency) {
			if (!freqTypeHT.ContainsKey(frequency))
				return null;
			RecurrenceType type = (RecurrenceType)freqTypeHT[frequency];
			return CreateInstance(type);
		}
		public static VRecurrenceConverter CreateInstance(RecurrenceType type) {
			switch (type) {
				case RecurrenceType.Minutely:
					return new MinutelyRuleConverter();
				case RecurrenceType.Hourly:
					return new HourlyRuleConverter();
				case RecurrenceType.Daily:
					return new DailyRuleConverter();
				case RecurrenceType.Weekly:
					return new WeeklyRuleConverter();
				case RecurrenceType.Monthly:
					return new MonthlyRuleConverter();
				case RecurrenceType.Yearly:
					return new YearlyRuleConverter();
				default:
					return null;
			}
		}
		#endregion
		#region FromRecurrenceInfo
		public virtual VRecurrenceRule FromRecurrenceInfo(IRecurrenceInfo info, Appointment pattern) {
			VRecurrenceFrequency freq = GetRecurrenceFrequency(info);
			VRecurrenceRule rule = new VRecurrenceRule(freq);
			if (info.Range != RecurrenceRange.NoEndDate) {
				int occurrenceCount = (info.Range == RecurrenceRange.EndByDate) ? ((IInternalRecurrenceInfo)info).CalcOccurrenceCountInRange(pattern) : info.OccurrenceCount;
				rule.Count = occurrenceCount;
			}
			rule.Interval = info.Periodicity;
			return rule;
		}
		#endregion
		#region AssignRecurrenceInfo
		public virtual bool AssignRecurrenceInfo(IRecurrenceInfo dest, DateTime srcStart, VRecurrenceRule rule) {
			if (!CanConvert(rule, dest))
				return false;
			AssignRecurrenceInfoCore(rule, srcStart, dest);
			return true;
		}
		#endregion
		#region CanConvert
		protected virtual bool CanConvert(VRecurrenceRule rule, IRecurrenceInfo dest) {
			return rule != null && dest != null;
		}
		#endregion
		#region AssignRecurrenceInfoCore
		protected virtual void AssignRecurrenceInfoCore(VRecurrenceRule rule, DateTime srcStart, IRecurrenceInfo dest) {
			dest.Type = (RecurrenceType)freqTypeHT[rule.Frequency];
			dest.Start = srcStart;
			dest.Range = CalculateRecurrenceRange(rule);
			switch (dest.Range) {
				case RecurrenceRange.OccurrenceCount:
					dest.OccurrenceCount = rule.CountParam.Value;
					break;
				case RecurrenceRange.EndByDate:
					dest.End = rule.Until.Date.AddDays(1);
					break;
			}
			dest.Periodicity = rule.Interval;
		}
		#region GetRecurrenceRange
		protected internal virtual RecurrenceRange CalculateRecurrenceRange(VRecurrenceRule rule) {
			if (rule.CountParam != null && rule.CountParam.Value > 0) {
				return RecurrenceRange.OccurrenceCount;
			}
			if (rule.UntilParam != null)
				return RecurrenceRange.EndByDate;
			return RecurrenceRange.NoEndDate;
		}
		#endregion
		#endregion
		#region FromOccurrenceNumber
		protected WeekOfMonth FromOccurrenceNumber(int occurrenceNumber) {
			return occurrenceNumberHT.ContainsKey(occurrenceNumber) ? (WeekOfMonth)occurrenceNumberHT[occurrenceNumber] : WeekOfMonth.None;
		}
		#endregion
		#region ToOccurrenceNumber
		protected int ToOccurrenceNumber(WeekOfMonth weekOfMonth) {
			return occurrenceNumberInvertHT.ContainsKey(weekOfMonth) ? (int)occurrenceNumberInvertHT[weekOfMonth] : 1;
		}
		#endregion
		#region GetOccurrenceNumber
		protected int GetOccurrenceNumber(RecurrenceRuleByDayParameter byDay, RecurrenceRuleBySetPosParameter bySetPos) {
			int occurrenceNumberFromSetPos = GetOccurrenceNumberFromSetPos(bySetPos);
			int occurrenceNumberFromDay = GetOccurrenceNumberFromDay(byDay);
			if (occurrenceNumberFromSetPos != 0 && occurrenceNumberFromDay != 0)
				return 0;
			if (occurrenceNumberFromSetPos == 0 && occurrenceNumberFromDay == 0)
				return 0;
			if (occurrenceNumberFromSetPos != 0)
				return occurrenceNumberFromSetPos;
			return occurrenceNumberFromDay;
		}
		#endregion
		#region GetOccurrenceNumberFromDay
		protected int GetOccurrenceNumberFromDay(RecurrenceRuleByDayParameter byDay) {
			if (byDay == null)
				return 0;
			VDayOfWeekCollection days = byDay.Values;
			if (days.Count > 1)
				return 0;
			VDayOfWeek day = days[0];
			return day.Negative ? -day.Ordwk : day.Ordwk;
		}
		#endregion
		#region GetOccurrenceNumberFromSetPos
		protected int GetOccurrenceNumberFromSetPos(RecurrenceRuleBySetPosParameter bySetPosParam) {
			if (bySetPosParam == null)
				return 0;
			IntValueCollection setPosValues = bySetPosParam.Values;
			if (setPosValues.Count < 1)
				return 0;
			return setPosValues[0];
		}
		#endregion
		protected internal abstract VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info);
	}
	#endregion
	#region WeeklyRuleConverter
	public class WeeklyRuleConverter : VRecurrenceConverter {
		#region AssignRecurrenceInfoCore
		protected override void AssignRecurrenceInfoCore(VRecurrenceRule rule, DateTime srcStart, IRecurrenceInfo dest) {
			base.AssignRecurrenceInfoCore(rule, srcStart, dest);
			RecurrenceRuleByDayParameter byDay = rule.ByDayParam;
			VDayOfWeekCollection vDayOfWeeks = (byDay != null) ? byDay.Values : null;
			if (vDayOfWeeks != null && vDayOfWeeks.Count > 0)
				dest.WeekDays = vDayOfWeeks.ToWeekDays();
			else
				dest.WeekDays = DateTimeHelper.ToWeekDays(srcStart.DayOfWeek);
			dest.FirstDayOfWeek = rule.WeekStart;
		}
		#endregion
		#region FromRecurrenceInfo
		public override VRecurrenceRule FromRecurrenceInfo(IRecurrenceInfo info, Appointment pattern) {
			VRecurrenceRule rule = base.FromRecurrenceInfo(info, pattern);
			VDayOfWeekCollection days = VDayOfWeekCollection.FromWeekDays(info.WeekDays);
			rule.ByDay.AddRange(days);
			rule.WeekStart = info.FirstDayOfWeek;
			return rule;
		}
		#endregion
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Weekly;
		}
	}
	#endregion
	#region MonthlyRuleConverter
	public class MonthlyRuleConverter : VRecurrenceConverter {
		#region AssignRecurrenceInfo
		protected override void AssignRecurrenceInfoCore(VRecurrenceRule rule, DateTime srcStart, IRecurrenceInfo dest) {
			base.AssignRecurrenceInfoCore(rule, srcStart, dest);
			AssignRecurrenceInfoBasePart(rule, srcStart, dest);
			AssignRecurrenceWeekPart(rule, dest);
		}
		#endregion
		#region CanConvert
		protected override bool CanConvert(VRecurrenceRule rule, IRecurrenceInfo dest) {
			if (!base.CanConvert(rule, dest))
				return false;
			RecurrenceRuleByDayParameter byDay = rule.ByDayParam;
			int occurrenceNumber = GetOccurrenceNumber(byDay, rule.BySetPosParam);
			if (occurrenceNumber == 0) {
				int periodicity = rule.Interval;
				if (!IsWeeklyRecurrence(rule, occurrenceNumber, periodicity ))
					return IsSupportedRecurrence(rule, occurrenceNumber, periodicity );
			}
			return true;
		}
		#endregion
		#region AssignRecurrenceInfoBasePart
		protected virtual void AssignRecurrenceInfoBasePart(VRecurrenceRule rule, DateTime srcDate, IRecurrenceInfo dest) {
			dest.DayNumber = CalcDayNumber(rule.ByMonthDayParam, srcDate);
			dest.WeekOfMonth = WeekOfMonth.None;
		}
		#endregion
		#region AssignRecurrenceWeekPart
		protected virtual bool AssignRecurrenceWeekPart(VRecurrenceRule rule, IRecurrenceInfo dest) {
			RecurrenceRuleByDayParameter byDay = rule.ByDayParam;
			int occurrenceNumber = GetOccurrenceNumber(byDay, rule.BySetPosParam);
			if (occurrenceNumber != 0)
				dest.WeekOfMonth = FromOccurrenceNumber(occurrenceNumber);
			else if (IsWeeklyRecurrence(rule, occurrenceNumber, rule.Interval ))
				dest.Type = RecurrenceType.Weekly;
			if (rule.ByDay.Count > 0)
				dest.WeekDays = rule.ByDay.ToWeekDays();
			return true;
		}
		#endregion
		#region IsWeeklyRecurrence
		protected bool IsWeeklyRecurrence(VRecurrenceRule rule, int occurrenceNumber, int periodicity) {
			if (occurrenceNumber == 0
				&& rule.ByMonthDayParam == null
				&& rule.ByDayParam != null
				&& periodicity == 1)
				return true;
			return false;
		}
		#endregion
		#region IsSupportedRecurrence
		protected virtual bool IsSupportedRecurrence(VRecurrenceRule rule, int occurrenceNumber, int periodicity) {
			bool isEmptyByMonthDay = rule.ByMonthDayParam == null;
			bool isEmptyByDay = rule.ByDayParam == null;
			bool isEmptyBySetPos = rule.BySetPosParam == null;
			if (isEmptyByMonthDay && isEmptyByDay && isEmptyBySetPos)
				return true;
			if (occurrenceNumber == 0 && isEmptyByMonthDay && !isEmptyByDay && periodicity > 1)
				return false;
			if (!isEmptyByMonthDay && isEmptyByDay)
				return true;
			return false;
		}
		#endregion
		#region CalcDayNumber
		protected internal int CalcDayNumber(RecurrenceRuleByMonthDayParameter byMonthDay, DateTime start) {
			if (byMonthDay != null) {
				int dayNumber = byMonthDay.Values[0];
				if (dayNumber > 0)
					return dayNumber;
			}
			return start.Day;
		}
		#endregion
		#region FromRecurrenceInfo
		public override VRecurrenceRule FromRecurrenceInfo(IRecurrenceInfo info, Appointment pattern) {
			VRecurrenceRule rule = base.FromRecurrenceInfo(info, pattern);
			if (info.WeekOfMonth != WeekOfMonth.None && info.WeekDays != 0) {
				VDayOfWeekCollection days = VDayOfWeekCollection.FromWeekDays(info.WeekDays);
				rule.ByDay.AddRange(days);
				rule.BySetPos.Add(ToOccurrenceNumber(info.WeekOfMonth));
			}
			if (info.WeekOfMonth == WeekOfMonth.None && info.DayNumber > 0) {
				rule.ByMonthDay.Add(info.DayNumber);
			}
			return rule;
		}
		#endregion
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Monthly;
		}
	}
	#endregion
	#region YearlyRuleConverter
	public class YearlyRuleConverter : MonthlyRuleConverter {
		#region AssignRecurrenceInfoBasePart
		protected override void AssignRecurrenceInfoBasePart(VRecurrenceRule rule, DateTime srcStart, IRecurrenceInfo dest) {
			dest.WeekOfMonth = WeekOfMonth.None;
			dest.DayNumber = srcStart.Day;
			dest.Month = srcStart.Month;
			if (rule.ByMonthParam != null)
				dest.Month = rule.ByMonthParam.Values[0];
			if (rule.ByMonthDayParam != null)
				dest.DayNumber = rule.ByMonthDayParam.Values[0];
			if (rule.ByYearDayParam != null) {
				DateTime dt = DayOfYearToDateTime(srcStart.Year, rule.ByYearDay[0]);
				dest.DayNumber = dt.Day;
				dest.Month = dt.Month;
			}
		}
		#endregion
		#region IsSupportedRecurrence
		protected override bool IsSupportedRecurrence(VRecurrenceRule rule, int occurrenceNumber, int periodicity) {
			if (!base.IsSupportedRecurrence(rule, occurrenceNumber, periodicity))
				return false;
			bool useByMonth = rule.ByMonthParam != null;
			bool useByMonthDay = rule.ByMonthDayParam != null;
			bool useByYearDay = rule.ByYearDayParam != null;
			if (useByYearDay && (useByMonth || useByMonthDay))
				return false;
			return true;
		}
		#endregion
		#region DayOfYearToDateTime
		DateTime DayOfYearToDateTime(int year, int day) {
			DateTime dt = new DateTime(year, 1, 1);
			return dt.AddDays(day);
		}
		#endregion
		#region FromRecurrenceInfo
		public override VRecurrenceRule FromRecurrenceInfo(IRecurrenceInfo info, Appointment pattern) {
			VRecurrenceRule rule = base.FromRecurrenceInfo(info, pattern);
			if (info.Month > 0)
				rule.ByMonth.Add(info.Month);
			return rule;
		}
		#endregion
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Yearly;
		}
	}
	#endregion
	#region HourlyRuleConverter
	public class HourlyRuleConverter : VRecurrenceConverter {
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Hourly;
		}
	}
	#endregion
	#region DailyRuleConverter
	public class DailyRuleConverter : VRecurrenceConverter {
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Daily;
		}
	}
	#endregion
	#region MinutelyRuleConverter
	public class MinutelyRuleConverter : VRecurrenceConverter {
		protected internal override VRecurrenceFrequency GetRecurrenceFrequency(IRecurrenceInfo info) {
			return VRecurrenceFrequency.Minutely;
		}
	}
	#endregion
	#region iCalendarWriter
	public class iCalendarWriter {
		const int MaxContentLineLength = 75;
		#region Fields
		TextWriter textWriter;
		int octetCount;
		#endregion
		public iCalendarWriter(TextWriter tw) {
			this.textWriter = tw;
			this.octetCount = 0;
		}
		#region Properties
		public TextWriter TextWriter { get { return textWriter; } }
		internal int OctetCount { get { return octetCount; } set { octetCount = value; } }
		#endregion
		#region ToString
		public override string ToString() {
			return TextWriter.ToString();
		}
		#endregion
		#region Reset
		public void Reset() {
			OctetCount = 0;
		}
		#endregion
		#region WriteLine
		public void WriteLine(string value) {
			Write(value);
			WriteLineCore();
		}
		public void WriteLine() {
			WriteLineCore();
		}
		#endregion
		#region Write
		public void Write(string value) {
			using (StringReader sr = new StringReader(value)) {
				char[] buf = new char[MaxContentLineLength];
				int length = sr.Read(buf, 0, GetAvailableLength());
				WriteCore(buf, length);
				while ((length = sr.Read(buf, 0, MaxContentLineLength - 1)) != 0) {
					WriteLineCore();
					WriteCore(iCalendarSymbols.Tab);
					WriteCore(buf, length);
				}
			}
		}
		#endregion
		#region WriteLineCore
		void WriteCore(char[] buf, int length) {
			XtraSchedulerDebug.Assert(buf.Length >= length);
			TextWriter.Write(buf, 0, length);
			OctetCount += length;
		}
		void WriteLineCore() {
			TextWriter.WriteLine();
			OctetCount = 0;
		}
		#endregion
		#region WriteCore
		void WriteCore(char symbol) {
			TextWriter.Write(symbol);
			OctetCount += 1;
		}
		#endregion
		#region GetAvailableLength
		int GetAvailableLength() {
			return MaxContentLineLength - OctetCount;
		}
		#endregion
	}
	#endregion
	#region IUniqueIdentifierProvider
	public interface IUniqueIdentifierProvider {
		void Reset();
		string NextUid();
	}
	#endregion
	#region GuidUniqueIdentifierProvider
	public class GuidUniqueIdentifierProvider : IUniqueIdentifierProvider {
		public virtual void Reset() {
		}
		public virtual string NextUid() {
			return Guid.NewGuid().ToString();
		}
	}
	#endregion
	#region TokenValueMatcher
	public class TokenValueMatcher<TValue> {
		#region Fields
		Dictionary<string, TValue> ht = new Dictionary<string, TValue>();
		Dictionary<TValue, string> invertHt = new Dictionary<TValue, string>();
		#endregion
		#region Properties
		internal Dictionary<string, TValue> Ht { get { return ht; } }
		internal Dictionary<TValue, string> InvertHt { get { return invertHt; } }
		#endregion
		#region RegistrToken
		public void RegisterToken(string tokenString, TValue tokenValue) {
			ht.Add(tokenString, tokenValue);
			invertHt.Add(tokenValue, tokenString);
		}
		#endregion
		#region IsTokenRegistered
		public bool IsTokenRegistered(string tokenString) {
			return ht.ContainsKey(tokenString);
		}
		#endregion
		#region IsValueRegistered
		public bool IsValueRegistered(TValue tokenValue) {
			return invertHt.ContainsKey(tokenValue);
		}
		#endregion
		#region ValueFromToken
		public TValue ValueFromToken(string tokenString) {
			return ht[tokenString];
		}
		#endregion
		public string TokenFromValue(TValue tokenValue) {
			return invertHt[tokenValue];
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.iCalendar {
	public static class iCalendarHelper {
		public static void ApplyRecurrenceInfo(IRecurrenceInfo recurrenceInfo, DateTime start, VRecurrenceRule rule) {
			VRecurrenceConverter conv = VRecurrenceConverter.CreateInstance(rule.Frequency);
			conv.AssignRecurrenceInfo(recurrenceInfo, start, rule);
		}
	}
}
