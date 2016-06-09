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
using System.Text;
using DevExpress.XtraScheduler.Native;
using System.Text.RegularExpressions;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.iCalendar.Internal;
using System.Collections;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public enum VRecurrenceFrequency { Hourly, Minutely, Secondly, Daily, Weekly, Monthly, Yearly } 
	#region VDayOfWeekCollection
	public class VDayOfWeekCollection : List<VDayOfWeek> {
		#region FromWeekDays
		public static VDayOfWeekCollection FromWeekDays(WeekDays days) {
			VDayOfWeekCollection result = new VDayOfWeekCollection();
			DayOfWeek[] dayOfWeeks = DateTimeHelper.ToDayOfWeeks(days);
			int count = dayOfWeeks.Length;
			for (int i = 0; i < count; i++)
				result.Add(new VDayOfWeek(dayOfWeeks[i]));
			return result;
		}
		#endregion
		#region ctor
		public VDayOfWeekCollection(string value) {
			string[] args = value.Split(new char[] { ',' });
			int count = args.Length;
			for (int i = 0; i < count; i++) {
				VDayOfWeek item = new VDayOfWeek(args[i]);
				Add(item);
			}
		}
		public VDayOfWeekCollection() {
		}
		#endregion
		#region ToWeekDays
		public virtual WeekDays ToWeekDays() {
			WeekDays weekDays = 0;
			for (int i = 0; i < Count; i++) {
				DayOfWeek weekDay = this[i].WeekDay;
				weekDays |= DateTimeHelper.ToWeekDays(weekDay);
			}
			return weekDays;
		}
		#endregion
	}
	#endregion
	public class VDayOfWeekAscendingComparer : IComparer<VDayOfWeek> {
		public int Compare(VDayOfWeek x, VDayOfWeek y) {
			if (x.WeekDay == y.WeekDay)
				return 0;
			if (x.WeekDay < y.WeekDay)
				return -1;
			return 1;
		}
	}
	#region VDayOfWeek
	public class VDayOfWeek {
		#region Fields
		DayOfWeek weekDay;
		int ordwk;
		bool negative;
		#endregion
		#region ctor
		public VDayOfWeek(string value) {
			if (value == null)
				Exceptions.ThrowArgumentNullException("value");
			Match match = Regex.Match(value, @"^(\+|-)?(\d{1,2})?(\w{2})$");
			GroupCollection groups = match.Groups;
			if (!match.Success || (groups[1].Success && !groups[2].Success))
				throw new iCalendarParseErrorException();
			this.negative = iCalendarConvert.IsRegExpGroupNegative(groups[1]);
			this.ordwk = iCalendarConvert.GetRegExpGroupIntValue(groups[2]);
			if (Ordwk > 53)
				throw new iCalendarParseErrorException();
			this.weekDay = iCalendarConvert.ToDayOfWeek(groups[3].Value);
		}
		public VDayOfWeek(DayOfWeek dayOfWeek, int ordwk, bool negative) {
			this.weekDay = dayOfWeek;
			this.ordwk = ordwk;
			this.negative = negative;
		}
		public VDayOfWeek(DayOfWeek dayOfWeek, int ordwk)
			: this(dayOfWeek, ordwk, false) {
		}
		public VDayOfWeek(DayOfWeek dayOfWeek)
			: this(dayOfWeek, 0) {
		}
		#endregion
		#region Properties
		public DayOfWeek WeekDay { get { return weekDay; } }
		public int Ordwk { get { return ordwk; } }
		public bool Negative { get { return negative; } }
		#endregion
		#region ToString
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			if (Negative)
				result.Append("-");
			if (Ordwk != 0)
				result.Append(Ordwk.ToString());
			result.Append(iCalendarConvert.FromDayOfWeek(WeekDay));
			return result.ToString();
		}
		#endregion
	}
	#endregion
	#region IntValueCollection
	public class IntValueCollection : List<int> {
		#region Fields
		readonly int minAbsoluteValue;
		readonly int maxAbsoluteValue;
		readonly bool acceptNegativeValue;
		#endregion
		#region ctor
		public IntValueCollection(string value, int minAbsoluteValue, int maxAbsoluteValue, bool acceptNegativeValue) {
			this.minAbsoluteValue = minAbsoluteValue;
			this.maxAbsoluteValue = maxAbsoluteValue;
			this.acceptNegativeValue = acceptNegativeValue;
			string[] args = value.Split(new char[] { ',' });
			int count = args.Length;
			for (int i = 0; i < count; i++) {
				int item = ParseItem(args[i]);
				CheckItem(item);
				Add(item);
			}
		}
		public IntValueCollection(string value, int minAbsoluteValue, int maxAbsoluteValue)
			: this(value, minAbsoluteValue, maxAbsoluteValue, false) {
		}
		public IntValueCollection(int[] values, int minAbsoluteValue, int maxAbsoluteValue)
			: this(values, minAbsoluteValue, maxAbsoluteValue, false) {
		}
		public IntValueCollection(int[] values, int minAbsoluteValue, int maxAbsoluteValue, bool acceptNegativeValue) {
			this.minAbsoluteValue = minAbsoluteValue;
			this.maxAbsoluteValue = maxAbsoluteValue;
			this.acceptNegativeValue = acceptNegativeValue;
			AddRange(values);
		}
		#endregion
		#region Properties
		public int MinAbsoluteValue { get { return minAbsoluteValue; } }
		public int MaxAbsoluteValue { get { return maxAbsoluteValue; } }
		public bool AcceptNegativeValue { get { return acceptNegativeValue; } }
		#endregion
		#region ParseItem
		protected internal virtual int ParseItem(string value) {
			return iCalendarConvert.ToInt(value, String.Format("Can't parse value {0}", value));
		}
		#endregion
		#region CheckItem
		public void CheckItem(int itemValue) {
			int testItem = (AcceptNegativeValue) ? System.Math.Abs(itemValue) : itemValue;
			if (testItem < MinAbsoluteValue || MaxAbsoluteValue < testItem)
				throw new iCalendarParseErrorException();
		}
		#endregion
		#region ToString
		public override string ToString() {
			int count = Count;
			string[] stringValues = new string[count];
			for (int i = 0; i < count; i++) {
				int item = this[i];
				CheckItem(item);
				stringValues[i] = item.ToString();
			}
			return String.Join(iCalendarSymbols.ParamValueSeparator, stringValues);
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleContentLineParametersCreator
	public static class RecurrenceRuleContentLineParametersCreator {
		public static iContentLineParameters CreateContentLineParametersFromString(string value) {
			iContentLineParameters parameters = new iContentLineParameters();
			string[] args = value.Split(new char[] { ';' });
			int count = args.Length;
			for (int i = 0; i < count; i++) {
				iContentLineParam param = CreateParam(args[i]);
				parameters.Add(param);
			}
			return parameters;
		}
		internal static iContentLineParam CreateParam(string value) {
			string[] args = value.Split(new char[] { '=' });
			if (args.Length != 2)
				throw new iCalendarParseErrorException();
			iContentLineParam param = new iContentLineParam();
			param.Name = args[0];
			param.Values.Add(args[1]);
			return param;
		}
	}
	#endregion
	#region VRecurrenceRule
	public class VRecurrenceRule : iCalendarPropertyBase {
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static VRecurrenceRule() {
			supportedParameterHandlers.Add(RecurrenceRuleUntilParameter.TokenName, AddRecurrenceRuleUntil);
			supportedParameterHandlers.Add(RecurrenceRuleCountParameter.TokenName, AddRecurrenceRuleCount);
			supportedParameterHandlers.Add(RecurrenceRuleFrequencyParameter.TokenName, AddRecurrenceRuleFrequency);
			supportedParameterHandlers.Add(RecurrenceRuleIntervalParameter.TokenName, AddRecurrenceRuleInterval);
			supportedParameterHandlers.Add(RecurrenceRuleBySecondParameter.TokenName, AddRecurrenceRuleBySecond);
			supportedParameterHandlers.Add(RecurrenceRuleByMinuteParameter.TokenName, AddRecurrenceRuleByMinute);
			supportedParameterHandlers.Add(RecurrenceRuleByHourParameter.TokenName, AddRecurrenceRuleByHour);
			supportedParameterHandlers.Add(RecurrenceRuleByDayParameter.TokenName, AddRecurrenceRuleByDay);
			supportedParameterHandlers.Add(RecurrenceRuleByMonthDayParameter.TokenName, AddRecurrenceRuleByMonthDay);
			supportedParameterHandlers.Add(RecurrenceRuleByYearDayParameter.TokenName, AddRecurrenceRuleByYearDay);
			supportedParameterHandlers.Add(RecurrenceRuleByWeekNoParameter.TokenName, AddRecurrenceRuleByWeekNo);
			supportedParameterHandlers.Add(RecurrenceRuleByMonthParameter.TokenName, AddRecurrenceRuleByMonth);
			supportedParameterHandlers.Add(RecurrenceRuleBySetPosParameter.TokenName, AddRecurrenceRuleBySetPos);
			supportedParameterHandlers.Add(RecurrenceRuleWeekStartParameter.TokenName, AddRecurrenceRuleWkSt);
		}
		#region Fields
		RecurrenceRuleFrequencyParameter freqParam;
		RecurrenceRuleUntilParameter untilParam;
		RecurrenceRuleCountParameter countParam;
		RecurrenceRuleIntervalParameter intervalParam;
		RecurrenceRuleBySecondParameter bySecondParam;
		RecurrenceRuleByMinuteParameter byMinuteParam;
		RecurrenceRuleByHourParameter byHourParam;
		RecurrenceRuleByDayParameter byDayParam;
		RecurrenceRuleByMonthDayParameter byMonthDayParam;
		RecurrenceRuleByYearDayParameter byYearDayParam;
		RecurrenceRuleByWeekNoParameter byWeekNoParam;
		RecurrenceRuleByMonthParameter byMonthParam;
		RecurrenceRuleBySetPosParameter bySetPosParam;
		RecurrenceRuleWeekStartParameter wkStParam;
		Dictionary<string, bool> parameterNameHash;
		#endregion
		#region ctor
		public VRecurrenceRule(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public VRecurrenceRule(VRecurrenceFrequency frequency)
			: base(null, String.Empty) {
			Frequency = frequency;
		}
		#endregion
		#region Properties
		public RecurrenceRuleUntilParameter UntilParam { get { return untilParam; } }
		public RecurrenceRuleCountParameter CountParam { get { return countParam; } }
		public RecurrenceRuleFrequencyParameter FrequencyParam { get { return freqParam; } }
		public RecurrenceRuleIntervalParameter IntervalParam { get { return intervalParam; } }
		public RecurrenceRuleBySecondParameter BySecondParam { get { return bySecondParam; } }
		public RecurrenceRuleByMinuteParameter ByMinuteParam { get { return byMinuteParam; } }
		public RecurrenceRuleByHourParameter ByHourParam { get { return byHourParam; } }
		public RecurrenceRuleByDayParameter ByDayParam { get { return byDayParam; } }
		public RecurrenceRuleByMonthDayParameter ByMonthDayParam { get { return byMonthDayParam; } }
		public RecurrenceRuleByYearDayParameter ByYearDayParam { get { return byYearDayParam; } }
		public RecurrenceRuleByWeekNoParameter ByWeekNoParam { get { return byWeekNoParam; } }
		public RecurrenceRuleByMonthParameter ByMonthParam { get { return byMonthParam; } }
		public RecurrenceRuleBySetPosParameter BySetPosParam { get { return bySetPosParam; } }
		public RecurrenceRuleWeekStartParameter WeekStartParam { get { return wkStParam; } }
		#region Until
		public DateTime Until {
			get { return UntilParam != null ? UntilParam.Value : iCalendarUtils.DefaultDateTime; }
			set { this.untilParam = new RecurrenceRuleUntilParameter(value); }
		}
		#endregion
		#region Count
		public int Count {
			get { return CountParam != null ? CountParam.Value : iCalendarUtils.DefaultIntZeroValue; }
			set { this.countParam = new RecurrenceRuleCountParameter(value); }
		}
		#endregion
		#region Frequency
		public VRecurrenceFrequency Frequency {
			get { return FrequencyParam != null ? FrequencyParam.Value : iCalendarUtils.DefaultFrequency; }
			set { this.freqParam = new RecurrenceRuleFrequencyParameter(value); }
		}
		#endregion
		#region Interval
		public int Interval {
			get { return IntervalParam != null ? IntervalParam.Value : iCalendarUtils.DefaultIntervalValue; }
			set { this.intervalParam = new RecurrenceRuleIntervalParameter(value); }
		}
		#endregion
		#region BySecond
		public IntValueCollection BySecond {
			get {
				if (BySecondParam == null)
					this.bySecondParam = new RecurrenceRuleBySecondParameter(new int[] { });
				return BySecondParam.Values;
			}
			set { this.bySecondParam = value != null ? new RecurrenceRuleBySecondParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByMinute
		public IntValueCollection ByMinute {
			get {
				if (ByMinuteParam == null)
					this.byMinuteParam = new RecurrenceRuleByMinuteParameter(new int[] { });
				return ByMinuteParam.Values;
			}
			set { this.byMinuteParam = value != null ? new RecurrenceRuleByMinuteParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByHour
		public IntValueCollection ByHour {
			get {
				if (ByHourParam == null)
					this.byHourParam = new RecurrenceRuleByHourParameter(new int[] { });
				return ByHourParam.Values;
			}
			set { this.byHourParam = value != null ? new RecurrenceRuleByHourParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByDay
		public VDayOfWeekCollection ByDay {
			get {
				if (ByDayParam == null)
					this.byDayParam = new RecurrenceRuleByDayParameter(new VDayOfWeekCollection());
				return ByDayParam.Values;
			}
			set { this.byDayParam = new RecurrenceRuleByDayParameter(value); }
		}
		#endregion
		#region ByMonthDay
		public IntValueCollection ByMonthDay {
			get {
				if (ByMonthDayParam == null)
					this.byMonthDayParam = new RecurrenceRuleByMonthDayParameter(new int[] { });
				return ByMonthDayParam.Values;
			}
			set { this.byMonthDayParam = value != null ? new RecurrenceRuleByMonthDayParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByYearDay
		public IntValueCollection ByYearDay {
			get {
				if (ByYearDayParam == null)
					this.byYearDayParam = new RecurrenceRuleByYearDayParameter(new int[] { });
				return ByYearDayParam.Values;
			}
			set { this.byYearDayParam = (value != null) ? new RecurrenceRuleByYearDayParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByWeekNo
		public IntValueCollection ByWeekNo {
			get {
				if (ByWeekNoParam == null)
					this.byWeekNoParam = new RecurrenceRuleByWeekNoParameter(new int[] { });
				return ByWeekNoParam.Values;
			}
			set { this.byWeekNoParam = (value != null) ? new RecurrenceRuleByWeekNoParameter(value.ToArray()) : null; }
		}
		#endregion
		#region ByMonth
		public IntValueCollection ByMonth {
			get {
				if (ByMonthParam == null)
					this.byMonthParam = new RecurrenceRuleByMonthParameter(new int[] { });
				return ByMonthParam.Values;
			}
			set { this.byMonthParam = (value != null) ? new RecurrenceRuleByMonthParameter(value.ToArray()) : null; }
		}
		#endregion
		#region BySetPos
		public IntValueCollection BySetPos {
			get {
				if (BySetPosParam == null)
					this.bySetPosParam = new RecurrenceRuleBySetPosParameter(new int[] { });
				return BySetPosParam.Values;
			}
			set { this.bySetPosParam = (value != null) ? new RecurrenceRuleBySetPosParameter(value.ToArray()) : null; }
		}
		#endregion
		#region WeekStart
		public DayOfWeek WeekStart {
			get { return WeekStartParam != null ? WeekStartParam.Value : RecurrenceRuleWeekStartParameter.EmptyValue; }
			set {
				if (WeekStart == value)
					return;
				this.wkStParam = new RecurrenceRuleWeekStartParameter(value);
			}
		}
		#endregion
		public override string Name { get { return "RecurrenceRuleValue"; } }
		#region ParameterNameHash
		internal Dictionary<string, bool> ParameterNameHash {
			get {
				if (parameterNameHash == null)
					parameterNameHash = new Dictionary<string, bool>();
				return parameterNameHash;
			}
		}
		#endregion
		#region SupportedParameterHandlers
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		#endregion
		#endregion
		#region CanAddParameter
		protected internal override bool CanAddParameter(iContentLineParam param) {
			if (param.Name == "FREQ" && ParameterNameHash.Count != 0)
				throw new iCalendarParseErrorException();
			if ((param.Name == "UNTIL" && ParameterNameHash.ContainsKey("COUNT")) ||
				(param.Name == "COUNT" && ParameterNameHash.ContainsKey("UNTIL")))
				throw new iCalendarParseErrorException();
			if (!ParameterNameHash.ContainsKey(param.Name)) {
				ParameterNameHash.Add(param.Name, true);
				return true;
			}
			throw new iCalendarParseErrorException();
		}
		#endregion
		#region Supported Parameter Handlers
		static void AddRecurrenceRuleUntil(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).untilParam = new RecurrenceRuleUntilParameter(values);
		}
		static void AddRecurrenceRuleCount(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).countParam = new RecurrenceRuleCountParameter(values);
		}
		static void AddRecurrenceRuleFrequency(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).freqParam = new RecurrenceRuleFrequencyParameter(values);
		}
		static void AddRecurrenceRuleInterval(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).intervalParam = new RecurrenceRuleIntervalParameter(values);
		}
		static void AddRecurrenceRuleBySecond(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).bySecondParam = new RecurrenceRuleBySecondParameter(values);
		}
		static void AddRecurrenceRuleByMinute(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byMinuteParam = new RecurrenceRuleByMinuteParameter(values);
		}
		static void AddRecurrenceRuleByHour(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byHourParam = new RecurrenceRuleByHourParameter(values);
		}
		static void AddRecurrenceRuleByDay(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byDayParam = new RecurrenceRuleByDayParameter(values);
		}
		static void AddRecurrenceRuleByMonthDay(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byMonthDayParam = new RecurrenceRuleByMonthDayParameter(values);
		}
		static void AddRecurrenceRuleByYearDay(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byYearDayParam = new RecurrenceRuleByYearDayParameter(values);
		}
		static void AddRecurrenceRuleByWeekNo(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byWeekNoParam = new RecurrenceRuleByWeekNoParameter(values);
		}
		static void AddRecurrenceRuleByMonth(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).byMonthParam = new RecurrenceRuleByMonthParameter(values);
		}
		static void AddRecurrenceRuleBySetPos(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).bySetPosParam = new RecurrenceRuleBySetPosParameter(values);
		}
		static void AddRecurrenceRuleWkSt(iCalendarBodyItem item, string[] values) {
			((VRecurrenceRule)item).wkStParam = new RecurrenceRuleWeekStartParameter(values);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region WriteToStream
		protected override void WriteToStream(iCalendarWriter cw) {
			WriteParametersToStream(cw);
			WriteCustomParametersToStream(cw);
		}
		#endregion
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
			if (FrequencyParam != null)
				FrequencyParam.WriteToStream(cw);
			if (UntilParam != null)
				UntilParam.WriteToStream(cw);
			if (CountParam != null)
				CountParam.WriteToStream(cw);
			if (IntervalParam != null)
				IntervalParam.WriteToStream(cw);
			if (BySecondParam != null)
				BySecondParam.WriteToStream(cw);
			if (ByMinuteParam != null)
				ByMinuteParam.WriteToStream(cw);
			if (ByHourParam != null)
				ByHourParam.WriteToStream(cw);
			if (ByDayParam != null)
				ByDayParam.WriteToStream(cw);
			if (ByMonthDayParam != null)
				ByMonthDayParam.WriteToStream(cw);
			if (ByYearDayParam != null)
				ByYearDayParam.WriteToStream(cw);
			if (ByWeekNoParam != null)
				ByWeekNoParam.WriteToStream(cw);
			if (ByMonthParam != null)
				ByMonthParam.WriteToStream(cw);
			if (BySetPosParam != null)
				BySetPosParam.WriteToStream(cw);
			if (WeekStartParam != null)
				WeekStartParam.WriteToStream(cw);
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return this;
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleParameterBase (abstract class)
	public abstract class RecurrenceRuleParameterBase : iCalendarParameterBase {
		#region ctor
		protected RecurrenceRuleParameterBase(string[] stringValues)
			: base(stringValues) {
			if (stringValues.Length > 1)
				throw new iCalendarParseErrorException();
		}
		protected RecurrenceRuleParameterBase() {
		}
		#endregion
		#region Properties
		public string StringValue { get { return StringValues[0]; } }
		#endregion
	}
	#endregion
	#region RecurrenceRuleFrequencyParameter
	public class RecurrenceRuleFrequencyParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "FREQ";
		public override string Name { get { return TokenName; } }
		#region Static
		static Dictionary<string, VRecurrenceFrequency> requrrenceFrequencyHT = new Dictionary<string, VRecurrenceFrequency>();
		static Dictionary<VRecurrenceFrequency, string> requrrenceFrequencyInvertHT = new Dictionary<VRecurrenceFrequency, string>();
		static RecurrenceRuleFrequencyParameter() {
			RequrrenceFrequencyHT.Add("SECONDLY", VRecurrenceFrequency.Secondly);
			RequrrenceFrequencyHT.Add("MINUTELY", VRecurrenceFrequency.Minutely);
			RequrrenceFrequencyHT.Add("HOURLY", VRecurrenceFrequency.Hourly);
			RequrrenceFrequencyHT.Add("DAILY", VRecurrenceFrequency.Daily);
			RequrrenceFrequencyHT.Add("WEEKLY", VRecurrenceFrequency.Weekly);
			RequrrenceFrequencyHT.Add("MONTHLY", VRecurrenceFrequency.Monthly);
			RequrrenceFrequencyHT.Add("YEARLY", VRecurrenceFrequency.Yearly);
			foreach (string key in RequrrenceFrequencyHT.Keys)
				RequrrenceFrequencyInvertHT.Add(RequrrenceFrequencyHT[key], key);
		}
		static Dictionary<string, VRecurrenceFrequency> RequrrenceFrequencyHT { get { return requrrenceFrequencyHT; } }
		static Dictionary<VRecurrenceFrequency, string> RequrrenceFrequencyInvertHT { get { return requrrenceFrequencyInvertHT; } }
		#endregion
		#region Fields
		VRecurrenceFrequency value;
		#endregion
		#region ctor
		public RecurrenceRuleFrequencyParameter(string[] stringValues)
			: base(stringValues) {
			this.value = ParseFrequency(StringValue);
		}
		public RecurrenceRuleFrequencyParameter(VRecurrenceFrequency value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public VRecurrenceFrequency Value { get { return value; } }
		protected override string ParameterStartString { get { return String.Empty; } }
		#endregion
		#region ParseFrequency
		protected internal virtual VRecurrenceFrequency ParseFrequency(string stringValue) {
			if (RequrrenceFrequencyHT.ContainsKey(stringValue))
				return RequrrenceFrequencyHT[stringValue];
			throw new iCalendarParseErrorException();
		}
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			string stringValue = RequrrenceFrequencyInvertHT.ContainsKey(Value) ? RequrrenceFrequencyInvertHT[Value] : String.Empty;
			return new string[] { stringValue };
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleUntilParameter
	public class RecurrenceRuleUntilParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "UNTIL";
		#region Fields
		DateTime value;
		#endregion
		#region ctor
		public RecurrenceRuleUntilParameter(string[] stringValues)
			: base(stringValues) {
			this.value = iCalendarConvert.ToDateTime(StringValue);
		}
		public RecurrenceRuleUntilParameter(DateTime localTime) {
			this.value = localTime;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public DateTime Value { get { return value; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			return new string[] { iCalendarConvert.FromDateTime(Value.ToUniversalTime(), true) };
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleCountParameter
	public class RecurrenceRuleCountParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "COUNT";
		#region Fields
		int value;
		#endregion
		#region ctor
		public RecurrenceRuleCountParameter(string[] stringValues)
			: base(stringValues) {
			this.value = iCalendarConvert.ToInt(StringValue, String.Format("Parse error", StringValue));
		}
		public RecurrenceRuleCountParameter(int value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public int Value { get { return value; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			return new string[] { Value.ToString() };
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleIntervalParameter
	public class RecurrenceRuleIntervalParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "INTERVAL";
		const int DefaultValue = 1;
		#region Fields
		int value;
		#endregion
		#region ctor
		public RecurrenceRuleIntervalParameter(string[] stringValues)
			: base(stringValues) {
			this.value = iCalendarConvert.ToInt(StringValue, String.Format("Parse error", StringValue));
		}
		public RecurrenceRuleIntervalParameter(int value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public int Value { get { return value; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			if (Value == DefaultValue)
				return new string[] { String.Empty };
			return new string[] { Value.ToString() };
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleIntValueCollectionParameterBase (abstract class)
	public abstract class RecurrenceRuleIntValueCollectionParameterBase : RecurrenceRuleParameterBase {
		readonly IntValueCollection value;
		#region ctor
		protected RecurrenceRuleIntValueCollectionParameterBase(string[] stringValues, int minValue, int maxValue)
			: this(stringValues, minValue, maxValue, false) {
		}
		protected RecurrenceRuleIntValueCollectionParameterBase(string[] stringValues, int minAbsValue, int maxAbsValue, bool acceptNegativeValue)
			: base(stringValues) {
			this.value = new IntValueCollection(StringValue, minAbsValue, maxAbsValue, acceptNegativeValue);
		}
		protected RecurrenceRuleIntValueCollectionParameterBase(int[] values, int minAbsValue, int maxAbsValue, bool acceptNegativeValue) {
			this.value = new IntValueCollection(values, minAbsValue, maxAbsValue, acceptNegativeValue);
		}
		protected RecurrenceRuleIntValueCollectionParameterBase(int[] values, int minAbsValue, int maxAbsValue)
			: this(values, minAbsValue, maxAbsValue, false) {
		}
		#endregion
		public IntValueCollection Values { get { return value; } }
		#region ValuesToString
		protected override string[] ValuesToString() {
			string stringValue = Values.ToString();
			return new string[] { stringValue };
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleBySecondParameter
	public class RecurrenceRuleBySecondParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYSECOND";
		const int MinValue = 0;
		const int MaxValue = 59;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue); } }
		#endregion
		#region ctor
		public RecurrenceRuleBySecondParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue) {
		}
		public RecurrenceRuleBySecondParameter(int[] values)
			: base(values, MinValue, MaxValue) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByMinuteParameter
	public class RecurrenceRuleByMinuteParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYMINUTE";
		const int MinValue = 0;
		const int MaxValue = 59;
		#region Static
		static public IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue); } }
		#endregion
		#region ctor
		public RecurrenceRuleByMinuteParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue) {
		}
		public RecurrenceRuleByMinuteParameter(int[] values)
			: base(values, MinValue, MaxValue) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByHourParameter
	public class RecurrenceRuleByHourParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYHOUR";
		const int MinValue = 0;
		const int MaxValue = 23;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue); } }
		#endregion
		#region ctor
		public RecurrenceRuleByHourParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue) {
		}
		public RecurrenceRuleByHourParameter(int[] values)
			: base(values, MinValue, MaxValue) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByDayParameter
	public class RecurrenceRuleByDayParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "BYDAY";
		#region Static
		public static VDayOfWeekCollection EmptyValue { get { return new VDayOfWeekCollection(); } }
		#endregion
		#region Fields
		VDayOfWeekCollection values;
		#endregion
		#region ctor
		public RecurrenceRuleByDayParameter(string[] stringValues)
			: base(stringValues) {
			this.values = new VDayOfWeekCollection(StringValue);
		}
		public RecurrenceRuleByDayParameter(VDayOfWeekCollection values) {
			this.values = values;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public VDayOfWeekCollection Values { get { return values; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			int count = Values.Count;
			List<string> result = new List<string>();
			Values.Sort(new VDayOfWeekAscendingComparer());
			for (int i = 0; i < count; i++) {
				VDayOfWeek day = Values[i];
				result.Add(day.ToString());
			}
			return result.ToArray();
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleByMonthDayParameter
	public class RecurrenceRuleByMonthDayParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYMONTHDAY";
		const int MinValue = 1;
		const int MaxValue = 31;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue); } }
		#endregion
		#region ctor
		public RecurrenceRuleByMonthDayParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue, true) {
		}
		public RecurrenceRuleByMonthDayParameter(int[] values)
			: base(values, MinValue, MaxValue) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByYearDayParameter
	public class RecurrenceRuleByYearDayParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYYEARDAY";
		const int MinValue = 1;
		const int MaxValue = 366;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue, true); } }
		#endregion
		#region ctor
		public RecurrenceRuleByYearDayParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue, true) {
		}
		public RecurrenceRuleByYearDayParameter(int[] values)
			: base(values, MinValue, MaxValue, true) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByWeekNoParameter
	public class RecurrenceRuleByWeekNoParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYWEEKNO";
		const int MinValue = 1;
		const int MaxValue = 53;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue, true); } }
		#endregion
		#region ctor
		public RecurrenceRuleByWeekNoParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue, true) {
		}
		public RecurrenceRuleByWeekNoParameter(int[] values)
			: base(values, MinValue, MaxValue, true) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleByMonthParameter
	public class RecurrenceRuleByMonthParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYMONTH";
		const int MinValue = 0;
		const int MaxValue = 12;
		#region Static
		public static IntValueCollection EmptyValue = new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue);
		#endregion
		#region ctor
		public RecurrenceRuleByMonthParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue) {
		}
		public RecurrenceRuleByMonthParameter(int[] values)
			: base(values, MinValue, MaxValue) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleBySetPosParameter
	public class RecurrenceRuleBySetPosParameter : RecurrenceRuleIntValueCollectionParameterBase {
		public const string TokenName = "BYSETPOS";
		const int MinValue = 1;
		const int MaxValue = 366;
		#region Static
		public static IntValueCollection EmptyValue { get { return new IntValueCollection(new int[] { MinValue }, MinValue, MaxValue, true); } }
		#endregion
		#region ctor
		public RecurrenceRuleBySetPosParameter(string[] stringValues)
			: base(stringValues, MinValue, MaxValue, true) {
		}
		public RecurrenceRuleBySetPosParameter(int[] values)
			: base(values, MinValue, MaxValue, true) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region RecurrenceRuleWeekStartParameter
	public class RecurrenceRuleWeekStartParameter : RecurrenceRuleParameterBase {
		public const string TokenName = "WKST";
		#region Static
		public static DayOfWeek EmptyValue { get { return DayOfWeek.Monday; } }
		#endregion
		DayOfWeek value;
		#region ctor
		public RecurrenceRuleWeekStartParameter(string[] stringValues)
			: base(stringValues) {
			this.value = iCalendarConvert.ToDayOfWeek(StringValue);
		}
		public RecurrenceRuleWeekStartParameter(DayOfWeek value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public DayOfWeek Value { get { return value; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			string stringValue = iCalendarConvert.FromDayOfWeek(Value);
			return new string[] { stringValue };
		}
		#endregion
	}
	#endregion
}
