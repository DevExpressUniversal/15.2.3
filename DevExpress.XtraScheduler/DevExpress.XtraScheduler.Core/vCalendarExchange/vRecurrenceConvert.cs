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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraScheduler.VCalendar {
	public class VRecurrenceConvert {
		static SortedList freqTypeHT;
		static SortedList rangeHT;
		static SortedList occurrenceNumberHT;
		static VRecurrenceConvert() {
			freqTypeHT = new SortedList();
			freqTypeHT[VRecurrenceFrequency.Daily] = RecurrenceType.Daily;
			freqTypeHT[VRecurrenceFrequency.Weekly] = RecurrenceType.Weekly;
			freqTypeHT[VRecurrenceFrequency.Monthly] = RecurrenceType.Monthly;
			freqTypeHT[VRecurrenceFrequency.Yearly] = RecurrenceType.Yearly;
			rangeHT = new SortedList();
			rangeHT[VRecurrenceRange.OccurrenceCount] = RecurrenceRange.OccurrenceCount;
			rangeHT[VRecurrenceRange.EndByDate] = RecurrenceRange.EndByDate;
			rangeHT[VRecurrenceRange.Forever] = RecurrenceRange.NoEndDate;
			occurrenceNumberHT = new SortedList();
			occurrenceNumberHT[1] = WeekOfMonth.First;
			occurrenceNumberHT[2] = WeekOfMonth.Second;
			occurrenceNumberHT[3] = WeekOfMonth.Third;
			occurrenceNumberHT[4] = WeekOfMonth.Fourth;
			occurrenceNumberHT[-1] = WeekOfMonth.Last;
		}
		public VRecurrenceConvert() {
		}
		object GetKeyByValue(SortedList list, object val) {
			int index = list.IndexOfValue(val);
			return (index >= 0) ? list.GetKey(index) : null;
		}
		WeekOfMonth FromOccurrenceNumber(int occurrenceNumber) {
			return occurrenceNumberHT.ContainsKey(occurrenceNumber) ? (WeekOfMonth)occurrenceNumberHT[occurrenceNumber] : WeekOfMonth.None;
		}
		int ToOccurrenceNumber(WeekOfMonth weekOfMonth) {
			int index = occurrenceNumberHT.IndexOfValue(weekOfMonth);
			return (index >= 0) ? (int)occurrenceNumberHT.GetKey(index) : 0;
		}
		DateTime DayOfYearToDateTime(int year, int day) {
			DateTime dt = new DateTime(year, 1, 1);
			return dt.AddDays(day);
		}
		public string ConvertToString(VRecurrenceRule rule) {
			VRecurrenceRuleConverter conv = CreateConverter(rule);
			return conv != null ? conv.ConvertToString(rule) : String.Empty;
		}
		VRecurrenceRuleConverter CreateConverter(VRecurrenceRule rule) {
			switch (rule.Frequency) {
				case VRecurrenceFrequency.Daily:
					return new DailyRuleConverter();
				case VRecurrenceFrequency.Weekly:
					return new WeeklyRuleConverter();
				case VRecurrenceFrequency.Monthly: {
						if (rule.WeekDaysList.Count > 0)
							return new MonthlyByPosRuleConverter();
						else
							return (rule.DayNumberList.Count > 0) ? new MonthlyByDayRuleConverter() : null;
					}
				case VRecurrenceFrequency.Yearly:
					if (rule.MonthList.Count > 0)
						return new YearlyByMonthRuleConverter();
					else
						return (rule.DayList.Count > 0) ? new YearlyByDayRuleConverter() : null;
				default:
					return null;
			}
		}
		public VRecurrenceRule ConvertFromString(DateTime startDate, string val) {
			if (String.IsNullOrEmpty(val))
				return null;
			VRecurrenceRuleConverter conv = CreateConverter(val);
			return conv != null ? conv.ConvertFromString(startDate, val) : null;
		}
		VRecurrenceRuleConverter CreateConverter(string text) {
			if (text.StartsWith(VRecurrenceFrequencyTag.Daily))
				return new DailyRuleConverter();
			else
				if (text.StartsWith(VRecurrenceFrequencyTag.Weekly))
					return new WeeklyRuleConverter();
				else
					if (text.StartsWith(VRecurrenceFrequencyTag.MonthlyByPosition))
						return new MonthlyByPosRuleConverter();
					else
						if (text.StartsWith(VRecurrenceFrequencyTag.MonthlyByDay))
							return new MonthlyByDayRuleConverter();
						else
							if (text.StartsWith(VRecurrenceFrequencyTag.YearlyByMonth))
								return new YearlyByMonthRuleConverter();
			if (text.StartsWith(VRecurrenceFrequencyTag.YearlyByDay))
				return new YearlyByDayRuleConverter();
			return null;
		}
		public VRecurrenceRule FromRecurrenceInfo(IRecurrenceInfo info) {
			VRecurrenceRule rule = new VRecurrenceRule();
			RecurrenceType actualType = info.Type;
			if (actualType == RecurrenceType.Daily && info.WeekDays != WeekDays.EveryDay) {
				actualType = RecurrenceType.Weekly;
			}
			rule.Frequency = (VRecurrenceFrequency)GetKeyByValue(freqTypeHT, actualType);
			rule.StartDate = info.Start;
			rule.Range = (VRecurrenceRange)GetKeyByValue(rangeHT, info.Range);
			rule.EndDate = info.End;
			rule.Duration = info.OccurrenceCount;
			rule.Interval = info.Periodicity;
			if (rule.Frequency == VRecurrenceFrequency.Weekly) {
				rule.WeekDaysList.Add((VRecurrenceWeekDays)info.WeekDays, 1);
				rule.Interval = info.Periodicity;
			}
			if (rule.Frequency == VRecurrenceFrequency.Monthly) {
				if (info.WeekOfMonth != WeekOfMonth.None && info.WeekDays != 0) {
					int occNum = ToOccurrenceNumber(info.WeekOfMonth);
					rule.WeekDaysList.Add((VRecurrenceWeekDays)info.WeekDays, occNum);
				}
				else
					if (info.DayNumber > 0) {
						rule.DayNumberList.Add(new VRecurrenceRuleDayNumber(info.DayNumber, false));
					}
			}
			if (rule.Frequency == VRecurrenceFrequency.Yearly) {
				if (info.Month > 0) {
					if (info.DayNumber > 0) {
						DateTime dt = new DateTime(info.Start.Year, info.Month, info.DayNumber);
						rule.DayList.Add(new VRecurrenceRuleDay(dt.DayOfYear));
					}
					rule.MonthList.Add(new VRecurrenceRuleMonth(info.Month));
				}
			}
			return rule;
		}
		public void AssignRecurrenceInfo(VRecurrenceRule rule, IRecurrenceInfo dest) {
			dest.Type = (RecurrenceType)freqTypeHT[rule.Frequency];
			dest.Start = rule.StartDate;
			dest.Range = (RecurrenceRange)rangeHT[rule.Range];
			if (rule.Duration > 0)
				dest.OccurrenceCount = rule.Duration;
			if (rule.EndDate != DateTime.MinValue)
				dest.End = rule.EndDate;
			dest.Periodicity = rule.Interval;
			if (rule.Frequency == VRecurrenceFrequency.Weekly) {
				if (rule.WeekDaysList.Count > 0) {
					dest.WeekDays = (WeekDays)rule.WeekDaysList[0].WeekDays;
				}
			}
			if (rule.Frequency == VRecurrenceFrequency.Monthly) {
				if (rule.DayNumberList.Count > 0) {
					VRecurrenceRuleDayNumber num = rule.DayNumberList[0];
					if (!num.Negative) {
						dest.DayNumber = num.DayNumber;
					}
					else {
					}
				}
				if (rule.WeekDaysList.Count > 0) {
					dest.WeekDays = (WeekDays)rule.WeekDaysList[0].WeekDays;
					dest.WeekOfMonth = FromOccurrenceNumber(rule.WeekDaysList[0].GetSignedValue());
				}
			}
			if (rule.Frequency == VRecurrenceFrequency.Yearly) {
				if (rule.MonthList.Count > 0) {
					dest.Month = rule.MonthList[0].Month;
					dest.DayNumber = 1;
				}
				if (rule.DayList.Count > 0) {
					DateTime dt = DayOfYearToDateTime(rule.StartDate.Year, rule.DayList[0].Day);
					dest.DayNumber = dt.Day;
					dest.Month = dt.Month;
				}
			}
		}
	}
	public abstract class VRecurrenceRuleConverter {
		static SortedList weekDaysHash;
		VRecurrenceRule rule;
		string bodyText = String.Empty;
		static VRecurrenceRuleConverter() {
			weekDaysHash = new SortedList();
			weekDaysHash[VRecurrenceWeekDays.Sunday] = "SU";
			weekDaysHash[VRecurrenceWeekDays.Monday] = "MO";
			weekDaysHash[VRecurrenceWeekDays.Tuesday] = "TU";
			weekDaysHash[VRecurrenceWeekDays.Wednesday] = "WE";
			weekDaysHash[VRecurrenceWeekDays.Thursday] = "TH";
			weekDaysHash[VRecurrenceWeekDays.Friday] = "FR";
			weekDaysHash[VRecurrenceWeekDays.Saturday] = "SA";
		}
		protected VRecurrenceRuleConverter() {
		}
		protected VRecurrenceRule Rule { get { return rule; } }
		protected string BodyText { get { return bodyText; } set { bodyText = value; } }
		protected abstract string BeginTag { get; }
		protected abstract VRecurrenceFrequency RecurrenceFrequency { get; }
		protected VRecurrenceWeekDays ToWeekDays(string modifier) {
			int index = weekDaysHash.IndexOfValue(modifier);
			return index >= 0 ? (VRecurrenceWeekDays)weekDaysHash.GetKey(index) : 0;
		}
		protected string[] FromWeekDays(VRecurrenceWeekDays weekDays) {
			List<string> modifiers = new List<string>();
			foreach (VRecurrenceWeekDays key in weekDaysHash.Keys) {
				if ((key & weekDays) > 0)
					modifiers.Add((string)weekDaysHash[key]);
			}
			return modifiers.ToArray();
		}
		public string ConvertToString(VRecurrenceRule rule) {
			this.rule = rule;
			StringCollection c = new StringCollection();
			c.Add(BeginTag + IntervalToString());
			c.Add(RuleContentToString());
			c.Add(DurationToString());
			string result = String.Empty;
			for (int i = 0; i < c.Count; i++) {
				if (!String.IsNullOrEmpty(c[i])) {
					if (result.Length > 0)
						result += " ";
					result += c[i];
				}
			}
			return result.Trim();
		}
		protected string IntervalToString() {
			return Rule.Interval.ToString();
		}
		protected string DurationToString() {
			if (Rule.Duration == 2)
				return String.Empty;
			switch (Rule.Range) {
				case VRecurrenceRange.OccurrenceCount:
					return "#" + Convert.ToString(Rule.Duration);
				case VRecurrenceRange.EndByDate:
					return VCalendarUtils.ToVCalendarDateTimeExact(Rule.EndDate, true);
				case VRecurrenceRange.Forever:
					return "#0";
			}
			return string.Empty;
		}
		protected virtual string RuleContentToString() {
			return String.Empty;
		}
		protected string CreateWeekDaysString(VRecurrenceWeekDays weekDays) {
			string[] modifiers = FromWeekDays(weekDays);
			string result = String.Empty;
			for (int i = 0; i < modifiers.Length; i++) {
				if (result.Length > 0) result += " ";
				result += modifiers[i];
			}
			return result;
		}
		public VRecurrenceRule ConvertFromString(DateTime startDate, string text) {
			this.rule = new VRecurrenceRule();
			this.rule.StartDate = startDate;
			rule.Frequency = RecurrenceFrequency;
			text = text.Remove(0, BeginTag.Length);
			string[] parts = text.Split(' ');
			string interval = parts[0];
			ParseInterval(interval);
			this.BodyText = text.Remove(0, interval.Length);
			int tokenCount = parts.Length;
			if (tokenCount > 1) {
				string duration = parts[tokenCount - 1];
				ParseDuration(duration);
				int index = BodyText.IndexOf(duration);
				this.BodyText = BodyText.Remove(index, duration.Length);
			}
			else {
				Rule.Duration = 2;
				return Rule;
			}
			BodyText = BodyText.Trim();
			ParseBody();
			OnParseComplete();
			return Rule;
		}
		protected virtual void ParseBody() {
			if (String.IsNullOrEmpty(BodyText))
				return;
			string[] parts = BodyText.Split(' ');
			int count = parts.Length;
			for (int i = 0; i < count; i++)
				ParseBodyItem(parts[i]);
		}
		protected virtual void ParseBodyItem(string text) {
		}
		protected virtual void OnParseComplete() {
		}
		protected void ParseInterval(string text) {
			Rule.Interval = Convert.ToInt32(text);
		}
		protected void ParseDuration(string text) {
			if (text.StartsWith("#")) {
				text = text.Remove(0, 1);
				Rule.Duration = Convert.ToInt32(text);
				Rule.Range = Rule.Duration == 0 ? VRecurrenceRange.Forever : VRecurrenceRange.OccurrenceCount;
			}
			else if (IsIso8601Date(text)) {
				Rule.Range = VRecurrenceRange.EndByDate;
				Rule.EndDate = VCalendarUtils.FromVCalendarDateTimeExact(text);
			}
		}
		protected string ExtractDigits(string s) {
			string result = String.Empty;
			for (int i = 0; i < s.Length; i++) {
				Char ch = s[i];
				if (Char.IsDigit(ch))
					result += ch.ToString();
				else
					break;
			}
			return result;
		}
		bool IsIso8601Date(string token) {
			int lenght = VCalendarConsts.Iso8601UtcDateTimeFormat.Length;
			if (token.Length != lenght)
				return false;
			for (int i = 0; i < lenght; i++) {
				Char ch = token[i];
				if (i != 8 && i != lenght - 1) {
					if (!Char.IsDigit(ch))
						return false;
				}
				else
					if (i == 8 && token[i] != 'T') {
						return false;
					}
					else
						if (i == lenght - 1 && token[i] != 'Z') {
							return false;
						}
			}
			return true;
		}
	}
	public class DailyRuleConverter : VRecurrenceRuleConverter {
		public DailyRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.Daily; } }
		protected override VRecurrenceFrequency RecurrenceFrequency { get { return VRecurrenceFrequency.Daily; } }
	}
	public class WeeklyRuleConverter : VRecurrenceRuleConverter {
		VRecurrenceWeekDays weekDaySet = 0;
		public WeeklyRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.Weekly; } }
		protected override VRecurrenceFrequency RecurrenceFrequency { get { return VRecurrenceFrequency.Weekly; } }
		protected override string RuleContentToString() {
			if (Rule.WeekDaysList.Count == 0)
				return String.Empty;
			return CreateWeekDaysString(Rule.WeekDaysList[0].WeekDays);
		}
		protected override void ParseBodyItem(string text) {
			this.weekDaySet |= ToWeekDays(text);
		}
		protected override void OnParseComplete() {
			if (weekDaySet != 0) Rule.WeekDaysList.Add(weekDaySet, 1);
		}
	}
	public abstract class MonthlyRuleConverter : VRecurrenceRuleConverter {
		protected MonthlyRuleConverter() {
		}
		protected override VRecurrenceFrequency RecurrenceFrequency { get { return VRecurrenceFrequency.Monthly; } }
	}
	public class MonthlyByPosRuleConverter : MonthlyRuleConverter {
		public MonthlyByPosRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.MonthlyByPosition; } }
		protected override string RuleContentToString() {
			StringBuilder result = new StringBuilder();
			WeekDaysOccurrenceList weekDaysList = Rule.WeekDaysList;
			int count = weekDaysList.Count;
			for (int i = 0; i < count; i++) {
				WeekDaysOccurrence occurrence = weekDaysList[i];
				string sign = occurrence.Negative ? "-" : "+";
				if (result.Length > 0)
					result.Append(' ');
				result.AppendFormat("{0}{1} {2}", occurrence.OccurrenceNumber, sign, CreateWeekDaysString(occurrence.WeekDays));
			}
			return result.ToString();
		}
		protected override void ParseBody() {
			if (String.IsNullOrEmpty(BodyText))
				return;
			WeekDaysOccurrence item = null;
			string[] parts = BodyText.Split(' ');
			for (int i = 0; i < parts.Length; i++) {
				string s = parts[i];
				VRecurrenceWeekDays weekDay = ToWeekDays(s);
				if (weekDay != 0 && item != null) {
					item.WeekDays |= weekDay;
					continue;
				}
				int num = ParseOccurrenceNumber(s);
				if (num != 0) {
					if (item != null)
						Rule.WeekDaysList.Add(item);
					item = new WeekDaysOccurrence(Math.Abs(num));
					item.Negative = num < 0;
				}
			}
			if (item != null)
				Rule.WeekDaysList.Add(item);
		}
		protected int ParseOccurrenceNumber(string text) {
			if (text.Length > 2)
				return 0;
			int number = 0;
			string digits = ExtractDigits(text);
			string sign = text.Remove(0, digits.Length);
			if (digits.Length == 1 && "12345".IndexOf(digits) != -1) {
				number = Convert.ToInt32(digits);
				if (sign == "-") number = -number;
			}
			return number;
		}
	}
	public class MonthlyByDayRuleConverter : MonthlyRuleConverter {
		public MonthlyByDayRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.MonthlyByDay; } }
		protected override string RuleContentToString() {
			StringBuilder result = new StringBuilder();
			VRecurrenceRuleDayNumberList dayNumberList = Rule.DayNumberList;
			int count = dayNumberList.Count;
			for (int i = 0; i < count; i++) {
				VRecurrenceRuleDayNumber num = dayNumberList[i];
				if (result.Length > 0)
					result.Append(' ');
				result.Append(num.LastDay ? "LD" : num.GetSignedValueString());
			}
			return result.ToString();
		}
		protected override void ParseBodyItem(string text) {
			VRecurrenceRuleDayNumber item = CreateDayNumber(text);
			Rule.DayNumberList.Add(item);
		}
		VRecurrenceRuleDayNumber CreateDayNumber(string text) {
			VRecurrenceRuleDayNumber dayNum = null;
			if (text == "LD" || text == "1-") {
				dayNum = new VRecurrenceRuleDayNumber(1, true);
				dayNum.Negative = true;
				return dayNum;
			}
			string digits = ExtractDigits(text);
			dayNum = new VRecurrenceRuleDayNumber(Convert.ToInt32(digits), false);
			string tail = text.Remove(0, digits.Length);
			if (tail.Length > 0) {
				if (tail.Length > 1 || !IsValidSign(tail[0]))
					return null;
				dayNum.Negative = tail[0] == '-';
			}
			return dayNum;
		}
		bool IsValidSign(Char c) {
			return c == '-' || c == '+';
		}
	}
	public abstract class YearlyRuleConverter : VRecurrenceRuleConverter {
		protected YearlyRuleConverter() {
		}
		protected override VRecurrenceFrequency RecurrenceFrequency { get { return VRecurrenceFrequency.Yearly; } }
	}
	public class YearlyByMonthRuleConverter : YearlyRuleConverter {
		public YearlyByMonthRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.YearlyByMonth; } }
		protected override void ParseBodyItem(string text) {
			VRecurrenceRuleMonth item = new VRecurrenceRuleMonth(Convert.ToInt32(text));
			Rule.MonthList.Add(item);
		}
		protected override string RuleContentToString() {
			StringBuilder result = new StringBuilder();
			VRecurrenceRuleMonthList monthList = Rule.MonthList;
			int count = monthList.Count;
			for (int i = 0; i < count; i++) {
				if (result.Length > 0)
					result.Append(' ');
				result.Append(monthList[i].Month.ToString());
			}
			return result.ToString();
		}
	}
	public class YearlyByDayRuleConverter : YearlyRuleConverter {
		public YearlyByDayRuleConverter() {
		}
		protected override string BeginTag { get { return VRecurrenceFrequencyTag.YearlyByDay; } }
		protected override void ParseBodyItem(string text) {
			VRecurrenceRuleDay item = new VRecurrenceRuleDay(Convert.ToInt32(text));
			Rule.DayList.Add(item);
		}
		protected override string RuleContentToString() {
			StringBuilder result = new StringBuilder();
			VRecurrenceRuleDayList dayList = Rule.DayList;
			int count = dayList.Count;
			for (int i = 0; i < count; i++) {
				if (result.Length > 0)
					result.Append(' ');
				result.Append(dayList[i].Day.ToString());
			}
			return result.ToString();
		}
	}
}
