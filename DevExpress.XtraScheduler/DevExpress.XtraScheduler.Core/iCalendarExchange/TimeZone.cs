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
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	#region VTimeZoneCollection
	public class VTimeZoneCollection : List<VTimeZone> {
	}
	#endregion
	#region VTimeZone
	public class VTimeZone : iCalendarComponentBase {
		#region Static
		public const string TokenName = "VTIMEZONE";
		static readonly Dictionary<Type, ObtainPropertyDelegate> supportedPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
		static readonly Dictionary<Type, ObtainComponentDelegate> supportedComponentHandlers = new Dictionary<Type, ObtainComponentDelegate>();
		static VTimeZone() {
			supportedPropertyHandlers.Add(typeof(TimeZoneIdentifierProperty), AddTimeZoneIdentifier);
			supportedComponentHandlers.Add(typeof(TimeZoneDaylight), AddTimeZoneDaylight);
			supportedComponentHandlers.Add(typeof(TimeZoneStandard), AddTimeZoneStandard);
		}
		#endregion
		#region Fields
		TimeZoneIdentifierProperty tzid;
		VTimeZoneRuleCollection standards;
		VTimeZoneRuleCollection daylights;
		#endregion
		#region ctor
		public VTimeZone() {
			this.tzid = new TimeZoneIdentifierProperty(null, null);
			this.standards = new VTimeZoneRuleCollection();
			this.daylights = new VTimeZoneRuleCollection();
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public TimeZoneIdentifierProperty TimeZoneIdentifier { get { return tzid; } }
		public VTimeZoneRuleCollection Standards { get { return standards; } }
		public VTimeZoneRuleCollection Daylights { get { return daylights; } }
		protected internal override Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers {
			get { return supportedPropertyHandlers; }
		}
		protected internal override Dictionary<Type, ObtainComponentDelegate> SupportedComponentHandlers {
			get { return supportedComponentHandlers; }
		}
		#endregion
		#region Supported Property Handlers
		static void AddTimeZoneIdentifier(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZone)container).tzid = (TimeZoneIdentifierProperty)value;
		}
		#endregion
		#region Supported Component Handlers
		static void AddTimeZoneDaylight(ICalendarBodyItemContainer container, iCalendarComponentBase value) {
			((VTimeZone)container).Daylights.Add((TimeZoneDaylight)value);
		}
		static void AddTimeZoneStandard(ICalendarBodyItemContainer container, iCalendarComponentBase value) {
			((VTimeZone)container).Standards.Add((TimeZoneStandard)value);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
			TimeZoneIdentifier.WriteToStream(cw);
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
			Standards.WriteToStream(cw);
			Daylights.WriteToStream(cw);
		}
		#endregion
	}
	#endregion
	#region TimeZoneIdentifierProperty
	public class TimeZoneIdentifierProperty : StringPropertyBase {
		public const string TokenName = "TZID";
		public TimeZoneIdentifierProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region TimeZoneNamePropertyCollection
	public class TimeZoneNamePropertyCollection : List<TimeZoneNameProperty>, IWritable {
		void IWritable.WriteToStream(iCalendarWriter cw) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].WriteToStream(cw);
		}
	}
	#endregion
	#region TimeZoneNameProperty
	public class TimeZoneNameProperty : LanguageTextPropertyBase {
		public const string TokenName = "TZNAME";
		#region ctor
		public TimeZoneNameProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public TimeZoneNameProperty(string value)
			: base(value) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region RecurrenceDateTimePropertyCollection
	public class RecurrenceDateTimePropertyCollection : List<RecurrenceDateTimeProperty>, ISupportCalendarTimeZone, IWritable {
		void IWritable.WriteToStream(iCalendarWriter cw) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].WriteToStream(cw);
		}
		protected void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				ISupportCalendarTimeZone impl = this[i] as ISupportCalendarTimeZone;
				if (impl == null)
					continue;
				impl.ApplyTimeZone(timeZoneManager);
			}
		}
		void ISupportCalendarTimeZone.ApplyTimeZone(TimeZoneManager tzm) {
			ApplyTimeZone(tzm);
		}
	}
	#endregion
	#region RecurrenceDateTimeProperty
	public class RecurrenceDateTimeProperty : TimeZoneDateTimePropertyBase {
		public const string TokenName = "RDATE";
		#region Fields
		TimeIntervalCollection values;
		#endregion
		#region ctor
		public RecurrenceDateTimeProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.values = ParseAsTimeIntervals(stringValue);
		}
		#endregion
		#region Properties
		public TimeIntervalCollection Values { get { return values; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ParseStringValues
		protected internal override DateTimeCollection ParseStringValues(string stringValue) {
			return new DateTimeCollection();
		}
		#endregion
		#region ParseStringValue
		protected internal TimeIntervalCollection ParseAsTimeIntervals(string stringValue) {
			return iCalendarConvert.ToTimeIntervalCollection(stringValue);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager manager) {
			int count = Values.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = Values[i];
				DateTime start = interval.Start;
				if (start.Kind == DateTimeKind.Local && !String.IsNullOrEmpty(TimeZoneIdentifier)) {
					DateTime localTime = manager.ToLocalTime(start, TimeZoneIdentifier);
					interval.Start = localTime;
				} else if (start.Kind == DateTimeKind.Utc) {
					interval.Start = manager.ToLocalTimeFromUtc(start);
				}
			}
		}
		#endregion
	}
	#endregion
	#region VTimeZoneRuleCollection
	public class VTimeZoneRuleCollection : List<VTimeZoneRule>, IWritable {
		void IWritable.WriteToStream(iCalendarWriter cw) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				this[i].WriteToStream(cw);
			}
		}
	}
	#endregion
	#region VTimeZoneRule (abstract class)
	public abstract class VTimeZoneRule : iCalendarComponentBase {
		static readonly Dictionary<Type, ObtainPropertyDelegate> supportedPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
		static VTimeZoneRule() {
			supportedPropertyHandlers.Add(typeof(TimeZoneOffsetToProperty), AddTimeZoneOffsetTo);
			supportedPropertyHandlers.Add(typeof(TimeZoneOffsetFromProperty), AddTimeZoneOffsetFrom);
			supportedPropertyHandlers.Add(typeof(TimeZoneNameProperty), AddTimeZoneName);
			supportedPropertyHandlers.Add(typeof(DateTimeStartProperty), AddDateTimeStart);
			supportedPropertyHandlers.Add(typeof(RecurrenceDateTimeProperty), AddRecurrenceDateTime);
			supportedPropertyHandlers.Add(typeof(RecurrenceRuleProperty), AddRecurrenceRuleProperty);
		}
		#region Fields
		DateTimeStartProperty start;
		TimeZoneOffsetToProperty tzoffsetto;
		TimeZoneOffsetFromProperty tzoffsetfrom;
		TimeZoneNamePropertyCollection tznames;
		RecurrenceDateTimePropertyCollection rdates;
		RecurrenceRulePropertyCollection rrules;
		#endregion
		#region ctor
		protected VTimeZoneRule() {
			this.tzoffsetto = new TimeZoneOffsetToProperty(null, null);
			this.tzoffsetfrom = new TimeZoneOffsetFromProperty(null, null);
			this.tznames = new TimeZoneNamePropertyCollection();
			this.start = new DateTimeStartProperty(null, null);
			this.rdates = new RecurrenceDateTimePropertyCollection();
			this.rrules = new RecurrenceRulePropertyCollection();
		}
		#endregion
		#region Properties
		public DateTimeStartProperty Start { get { return start; } }
		public TimeZoneOffsetToProperty TimeZoneOffsetTo { get { return tzoffsetto; } }
		public TimeZoneOffsetFromProperty TimeZoneOffsetFrom { get { return tzoffsetfrom; } }
		public TimeZoneNamePropertyCollection TimeZoneNames { get { return tznames; } }
		public RecurrenceDateTimePropertyCollection RecurrenceDates { get { return rdates; } }
		public RecurrenceRulePropertyCollection RecurrenceRules { get { return rrules; } }
		protected internal override Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers {
			get { return supportedPropertyHandlers; }
		}
		#endregion
		#region Supported Property Handlers
		static void AddDateTimeStart(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).start = (DateTimeStartProperty)value;
		}
		static void AddTimeZoneOffsetTo(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).tzoffsetto = (TimeZoneOffsetToProperty)value;
		}
		static void AddTimeZoneOffsetFrom(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).tzoffsetfrom = (TimeZoneOffsetFromProperty)value;
		}
		static void AddTimeZoneName(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).TimeZoneNames.Add((TimeZoneNameProperty)value);
		}
		static void AddRecurrenceDateTime(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).RecurrenceDates.Add((RecurrenceDateTimeProperty)value);
		}
		static void AddRecurrenceRuleProperty(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VTimeZoneRule)container).RecurrenceRules.Add((RecurrenceRuleProperty)value);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			ISupportCalendarTimeZone impl = this.rdates as ISupportCalendarTimeZone;
			if (impl == null)
				return;
			impl.ApplyTimeZone(timeZoneManager);
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
			Start.WriteToStream(cw);
			RecurrenceRules.WriteToStream(cw);
			TimeZoneOffsetFrom.WriteToStream(cw);
			TimeZoneOffsetTo.WriteToStream(cw);
			TimeZoneNames.WriteToStream(cw);
			RecurrenceDates.WriteToStream(cw);
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region TimeZoneStandard
	public class TimeZoneStandard : VTimeZoneRule {
		public const string TokenName = "STANDARD";
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region TimeZoneDaylight
	public class TimeZoneDaylight : VTimeZoneRule {
		public const string TokenName = "DAYLIGHT";
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region TimeZoneOffsetToProperty
	public class TimeZoneOffsetToProperty : UtcOffsetPropertyBase {
		public const string TokenName = "TZOFFSETTO";
		public TimeZoneOffsetToProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region TimeZoneOffsetFromProperty
	public class TimeZoneOffsetFromProperty : UtcOffsetPropertyBase {
		public const string TokenName = "TZOFFSETFROM";
		public TimeZoneOffsetFromProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.iCalendar.Native {
	#region TimeZoneConverter
	public static class TimeZoneConverter {
		static System.TimeZoneInfo.AdjustmentRule ObtainBaseRule(TimeZoneInfo timeZone) {
			System.TimeZoneInfo.AdjustmentRule[] rules = timeZone.GetAdjustmentRules();
			if (rules.Length > 0)
				return rules[rules.Length - 1];				
			return null;
		}
		#region ConvertToVTimeZone
		[System.Diagnostics.CodeAnalysis.SuppressMessage("DevExpress.Design", "DCA0002")]
		public static VTimeZone ConvertToVTimeZone(TimeZoneInfo timeZone) {
			System.TimeZoneInfo.AdjustmentRule baseRule = ObtainBaseRule(timeZone);
			TimeSpan bias = timeZone.BaseUtcOffset;
			if (baseRule != null)
				bias += baseRule.DaylightDelta;
			bool useOnlyStandardTime = !timeZone.SupportsDaylightSavingTime || (baseRule.DateEnd.Year - baseRule.DateStart.Year == 0);
			DateTime standardDate = DateTime.MinValue;
			DayOfWeek dayOfWeek = DayOfWeek.Sunday;
			if (baseRule != null) {
				standardDate = new DateTime(baseRule.DateStart.Year, baseRule.DaylightTransitionEnd.Month, baseRule.DaylightTransitionEnd.Week, baseRule.DaylightTransitionEnd.TimeOfDay.Hour, baseRule.DaylightTransitionEnd.TimeOfDay.Minute, baseRule.DaylightTransitionEnd.TimeOfDay.Second);
				dayOfWeek = baseRule.DaylightTransitionStart.DayOfWeek;
			}
			VTimeZoneRule standard = new TimeZoneStandard();
			FillTimeZoneRule(standard, standardDate, dayOfWeek, timeZone.BaseUtcOffset, bias, useOnlyStandardTime);
			VTimeZoneRule daylight = null;
			if (!useOnlyStandardTime) {
				daylight = new TimeZoneDaylight();
				DateTime endDate = new DateTime(baseRule.DateEnd.Year, baseRule.DaylightTransitionStart.Month, baseRule.DaylightTransitionStart.Week, baseRule.DaylightTransitionStart.TimeOfDay.Hour, baseRule.DaylightTransitionStart.TimeOfDay.Minute, baseRule.DaylightTransitionStart.TimeOfDay.Second);
				FillTimeZoneRule(daylight, endDate, baseRule.DaylightTransitionEnd.DayOfWeek, bias, timeZone.BaseUtcOffset, useOnlyStandardTime);
			}
			return CreateVTimeZone(timeZone.DisplayName, standard, daylight);
		}
		#endregion
		static VTimeZone CreateVTimeZone(string displayName, VTimeZoneRule standard, VTimeZoneRule daylight) {
			VTimeZone result = new VTimeZone();
			result.TimeZoneIdentifier.Value = displayName;
			result.Standards.Add(standard);
			if (daylight != null)
				result.Daylights.Add(daylight);
			return result;
		}
		#region FillTimeZoneRule
		static void FillTimeZoneRule(VTimeZoneRule timeZoneRule, DateTime date, DayOfWeek dayOfWeek, TimeSpan offsetTo, TimeSpan offsetFrom, bool useOnlyStandardTime) {
			timeZoneRule.Start.Value = new DateTime(1601, date.Month, date.Day, date.Hour, date.Minute, date.Second);
			timeZoneRule.Start.TimeZoneIdentifier = iCalendarUtils.ClientTimeZoneIdentifier;
			timeZoneRule.TimeZoneOffsetTo.Value = offsetTo;
			timeZoneRule.TimeZoneOffsetFrom.Value = offsetFrom;
			if (!useOnlyStandardTime) {
				VRecurrenceRule recur = new VRecurrenceRule(VRecurrenceFrequency.Yearly);
				VDayOfWeek vDayOfWeek = CreateVDayOfWeek(dayOfWeek, date.Day);
				recur.ByDay.Add(vDayOfWeek);
				recur.ByMonth.Add(date.Month);
				timeZoneRule.RecurrenceRules.Add(recur);
			}
		}
		#endregion
		#region CreateVDayOfWeek
		static VDayOfWeek CreateVDayOfWeek(DayOfWeek dayOfWeek, int dayNumber) {
			bool isNegative = (dayNumber == 5);
			int ordwk = (isNegative) ? 1 : dayNumber;
			return new VDayOfWeek(dayOfWeek, ordwk, isNegative);
		}
		#endregion
		#region ConvertFromVTimeZone
		public static TimeZoneInfo ConvertFromVTimeZone(VTimeZone vTimeZone) {
			if (!CanConvert(vTimeZone))
				return null;
			TimeZoneCreator creator = new TimeZoneCreator(vTimeZone);
			TimeZoneInfo result = creator.ConvertFromIANA();
			if (result == null)
				result = creator.CreateTimeZoneInfo();
			return result;
		}
		#endregion
		#region GetMonthNumber
		static int GetMonthNumber(RecurrenceRuleByMonthParameter byMonth) {
			IntValueCollection values = byMonth.Values;
			if (values.Count != 1)
				return -1;
			return values[0];
		}
		#endregion
		#region GetDayNumber
		static int GetDayNumber(RecurrenceRuleByDayParameter day) {
			if (day.Values.Count != 1)
				return -1;
			VDayOfWeek vDay = day.Values[0];
			if (vDay.Negative && vDay.Ordwk == 1)
				return 5;
			if (!vDay.Negative && vDay.Ordwk < 5)
				return vDay.Ordwk;
			return -1;
		}
		#endregion
		#region CanConvert
		static bool CanConvert(VTimeZone vTimeZone) {
			VTimeZoneRuleCollection standards = vTimeZone.Standards;
			if (standards.Count != 0 && !CheckTimeZoneSubComponent(standards[0].RecurrenceRules))
				return false;
			VTimeZoneRuleCollection daylights = vTimeZone.Daylights;
			if (daylights.Count != 0 && !CheckTimeZoneSubComponent(daylights[0].RecurrenceRules))
				return false;
			return true;
		}
		static bool CheckTimeZoneSubComponent(RecurrenceRulePropertyCollection rules) {
			if (rules.Count == 0)
				return true;
			if (rules.Count != 1)
				return false;
			VRecurrenceRule rrule = rules[0].Value;
			if (GetDayNumber(rrule.ByDayParam) == -1)
				return false;
			if (GetMonthNumber(rrule.ByMonthParam) == -1)
				return false;
			return true;
		}
		#endregion
	}
	#endregion    
	#region TimeZoneManager
	public class TimeZoneManager {
		#region Fields
		Dictionary<string, TimeZoneInfo> timeZones;
		TimeZoneInfo clientTimeZone;
		TimeZoneEngine timeZoneEngine;
		Func<TimeZoneInfo, TimeZoneInfo> customizeTimeZoneMethod;
		#endregion
		#region ctor
		public TimeZoneManager(TimeZoneEngine timeZoneEngine) {
			this.timeZones = new Dictionary<string, TimeZoneInfo>();
			this.timeZoneEngine = timeZoneEngine;
			this.clientTimeZone = timeZoneEngine.OperationTimeZone;
			TimeZones.Add(String.Empty, ClientTimeZone);
		}
		#endregion
		#region Properties
		public Dictionary<string, TimeZoneInfo> TimeZones { get { return timeZones; } }
		public TimeZoneInfo ClientTimeZone { get { return clientTimeZone; } set { clientTimeZone = value; } }
		public TimeZoneInfo CurrentTimeZone { get { return this.timeZoneEngine.OperationTimeZone; } }
		#endregion
		#region CollectTimeZones
		public virtual void CollectTimeZones(VTimeZoneCollection timeZones) {
			int count = timeZones.Count;
			for (int i = 0; i < count; i++) {
				VTimeZone timeZone = timeZones[i];
				Add(timeZone);
			}
		}
		#endregion
		public void SetCustomizeTimeZoneMethod(Func<TimeZoneInfo, TimeZoneInfo> customizeTimeZoneMethod) {
			this.customizeTimeZoneMethod = customizeTimeZoneMethod;
		}
		#region Add
		public virtual void Add(VTimeZone timeZone) {
			string tzid = timeZone.TimeZoneIdentifier.Value;
			if (TimeZones.ContainsKey(tzid))
				return;
			TimeZoneInfo schedulerTimeZone = TimeZoneConverter.ConvertFromVTimeZone(timeZone);
			TimeZoneInfo customizedTimeZone = null;
			if (customizeTimeZoneMethod != null)
				customizedTimeZone = customizeTimeZoneMethod(schedulerTimeZone);
			if (customizedTimeZone != null)
				TimeZones.Add(tzid, customizedTimeZone);
			else
				TimeZones.Add(tzid, schedulerTimeZone);
		}
		#endregion
		#region Clear
		public void Clear() {
			TimeZones.Clear();
			TimeZones.Add(String.Empty, ClientTimeZone);
		}
		#endregion
		#region FromLocalTime
		public DateTime FromLocalTime(DateTime date, string tzid) {
			TimeZoneInfo iCalendarTimeZone = GetTimeZoneById(tzid);
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), CurrentTimeZone, iCalendarTimeZone);
		}		
		#endregion
		#region ToLocalTime
		public DateTime ToLocalTime(DateTime date, string tzid) {
			if (tzid == null)
				tzid = String.Empty;
			TimeZoneInfo iCalendarTimeZone = GetTimeZoneById(tzid);
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), iCalendarTimeZone, CurrentTimeZone);
		}
		public DateTime ToLocalTimeFromUtc(DateTime date) {
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), TimeZoneInfo.Utc, CurrentTimeZone);
		}
		#endregion
		protected virtual TimeZoneInfo GetTimeZoneById(string tzid) {
			TimeZoneInfo iCalendarTimeZone = TimeZones.ContainsKey(tzid) ? TimeZones[tzid] : CurrentTimeZone;
			return iCalendarTimeZone;
		}
	}
	#endregion
	public class LazyTimeZoneManager : TimeZoneManager {
		public LazyTimeZoneManager() : base(new TimeZoneEngine()) {
		}
		protected override TimeZoneInfo GetTimeZoneById(string tzid) {
			if (TimeZones.ContainsKey(tzid))
				return TimeZones[tzid];
			TimeZoneInfo timeZone = TimeZoneCreator.CreateTimeZoneById(tzid);
			if (timeZone == null)
				return CurrentTimeZone;
				TimeZones.Add(tzid, timeZone);
			return timeZone;
		}
	}
	public class TimeZoneCreator {
		public static TimeZoneInfo CreateTimeZoneById(string tzid) {
			TimeZoneInfo tzi = null;
			if (TimeZoneIdValidator.IsIdValid(tzid))
				tzi = TimeZoneInfo.FindSystemTimeZoneById(tzid);
			if (tzi != null)
				return tzi;
			tzid = IANAToTimeZoneIdConverter.GetTimeZone(tzid);
			return TimeZoneInfo.FindSystemTimeZoneById(tzid);
		}
		public TimeZoneCreator(VTimeZone vTimeZone) {
			VTimeZone = vTimeZone;
		}
		public VTimeZone VTimeZone { get; private set; }
		public TimeZoneInfo ConvertFromIANA() {
			if (TimeZoneIdValidator.IsIdValid(VTimeZone.TimeZoneIdentifier.Value))
				return TimeZoneInfo.FindSystemTimeZoneById(VTimeZone.TimeZoneIdentifier.Value);
			string timeZoneId = IANAToTimeZoneIdConverter.GetTimeZone(VTimeZone.TimeZoneIdentifier.Value);
			if (TimeZoneIdValidator.IsIdValid(timeZoneId))
				return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			else
				return null;
		}
		public TimeZoneInfo CreateTimeZoneInfo() {
			TimeSpan baseUtcOffset = CalculateBaseTimeZoneOffset(VTimeZone.Standards);
			int lastStandard = VTimeZone.Standards.Count - 1;
			int lastDaylight = VTimeZone.Daylights.Count - 1;
			VTimeZoneRule standardRule = VTimeZone.Standards[lastStandard];
			VTimeZoneRule daylightRule = (lastDaylight < 0) ? null : VTimeZone.Daylights[lastDaylight];
			DateTime start = DateTimeHelper.Min((daylightRule != null) ? daylightRule.Start.Value : DateTime.MaxValue, standardRule.Start.Value).Date;
			DateTime end = DateTime.MaxValue.Date;
			string timeZoneId = VTimeZone.TimeZoneIdentifier.Value;
			if (daylightRule != null) {
				System.TimeZoneInfo.TransitionTime daylightTransitionStart = CreateTransitionTime(daylightRule);
				System.TimeZoneInfo.TransitionTime daylightTransitionEnd = CreateTransitionTime(standardRule);
				System.TimeZoneInfo.AdjustmentRule rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(DateTime.SpecifyKind(start, DateTimeKind.Unspecified), DateTime.SpecifyKind(end, DateTimeKind.Unspecified), daylightRule.TimeZoneOffsetTo.Value - baseUtcOffset, daylightTransitionStart, daylightTransitionEnd);
				return TimeZoneInfo.CreateCustomTimeZone(timeZoneId, baseUtcOffset, VTimeZone.TimeZoneIdentifier.Value, String.Empty, String.Empty, new TimeZoneInfo.AdjustmentRule[] { rule });
			}
			return TimeZoneInfo.CreateCustomTimeZone(timeZoneId, baseUtcOffset, timeZoneId, timeZoneId);
		}
		System.TimeZoneInfo.TransitionTime CreateTransitionTime(VTimeZoneRule rule) {
			DateTime timeOfDay = CalcTimeZoneDate(rule);
			VRecurrenceRule rrule = rule.RecurrenceRules[0].Value;
			int month = GetMonthNumber(rrule.ByMonthParam);
			int week = GetDayNumber(rrule.ByDayParam);
			DayOfWeek dayOfWeek = GetDayOfWeek(rule);
			return System.TimeZoneInfo.TransitionTime.CreateFloatingDateRule(timeOfDay, month, week, dayOfWeek);
		}
		protected virtual TimeSpan CalculateBaseTimeZoneOffset(VTimeZoneRuleCollection vTimeZoneRuleCollection) {
			int count = vTimeZoneRuleCollection.Count;
			if (count == 0)
				return TimeSpan.Zero;
			return vTimeZoneRuleCollection[count - 1].TimeZoneOffsetTo.Value;
		}
		#region GetDayOfWeek
		DayOfWeek GetDayOfWeek(VTimeZoneRule rule) {
			VRecurrenceRule rrule = rule.RecurrenceRules[0].Value;
			return rrule.ByDay[0].WeekDay;
		}
		#endregion
		#region CalcTimeZoneDate
		DateTime CalcTimeZoneDate(VTimeZoneRule rule) {
			DateTime start = rule.Start.Value;
			return new DateTime(1, 1, 1, start.Hour, start.Minute, start.Second);
		}
		#endregion
		#region GetMonthNumber
		int GetMonthNumber(RecurrenceRuleByMonthParameter byMonth) {
			IntValueCollection values = byMonth.Values;
			if (values.Count != 1)
				return -1;
			return values[0];
		}
		#endregion
		#region GetDayNumber
		int GetDayNumber(RecurrenceRuleByDayParameter day) {
			if (day.Values.Count != 1)
				return -1;
			VDayOfWeek vDay = day.Values[0];
			if (vDay.Negative && vDay.Ordwk == 1)
				return 5;
			if (!vDay.Negative && vDay.Ordwk < 5)
				return vDay.Ordwk;
			return -1;
		}
		#endregion        
	}
}
