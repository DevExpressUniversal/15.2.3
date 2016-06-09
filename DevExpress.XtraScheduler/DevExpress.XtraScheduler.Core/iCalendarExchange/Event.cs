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
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.IO;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	#region VEventCollection
	public class VEventCollection : List<VEvent> {
		public VEventCollection() {
		}
		public VEventCollection(int capacity)
			: base(capacity) {
		}
	}
	#endregion
	#region VEvent
	public class VEvent : iCalendarComponentBase {
		public const string TokenName = "VEVENT";
		#region Static
		static readonly Dictionary<Type, ObtainPropertyDelegate> supportedPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
		static readonly Dictionary<Type, ObtainComponentDelegate> supportedComponentHandlers = new Dictionary<Type, ObtainComponentDelegate>();
		static VEvent() {
			supportedPropertyHandlers.Add(typeof(DateTimeCreatedProperty), AddDateTimeCreated);
			supportedPropertyHandlers.Add(typeof(DateTimeStampProperty), AddDateTimeStamp);
			supportedPropertyHandlers.Add(typeof(LastModifiedProperty), AddLastModified);
			supportedPropertyHandlers.Add(typeof(DateTimeStartProperty), AddDateTimeStart);
			supportedPropertyHandlers.Add(typeof(DateTimeEndProperty), AddDateTimeEnd);
			supportedPropertyHandlers.Add(typeof(UniqueIdentifierProperty), AddUniqueIdentifier);
			supportedPropertyHandlers.Add(typeof(DescriptionProperty), AddDescription);
			supportedPropertyHandlers.Add(typeof(LocationProperty), AddLocation);
			supportedPropertyHandlers.Add(typeof(SummaryProperty), AddSummary);
			supportedPropertyHandlers.Add(typeof(CategoriesProperty), AddCategories);
			supportedPropertyHandlers.Add(typeof(RecurrenceRuleProperty), AddRecurrenceRule);
			supportedPropertyHandlers.Add(typeof(ExceptionRuleProperty), AddExceptionRule);
			supportedPropertyHandlers.Add(typeof(ExceptionDateTimesProperty), AddExceptionDateTime);
			supportedPropertyHandlers.Add(typeof(RecurrenceIdProperty), AddRecurrenceId);
			supportedComponentHandlers.Add(typeof(VAlarm), AddAlarm);
		}
		#endregion
		#region Fields
		DateTimeCreatedProperty created;
		DateTimeStampProperty stamp;
		LastModifiedProperty lastModified;
		DateTimeStartProperty start;
		DateTimeEndProperty end;
		UniqueIdentifierProperty uid;
		DescriptionProperty description;
		LocationProperty location;
		SummaryProperty summary;
		CategoriesProperty categories;
		RecurrenceRulePropertyCollection recurrenceRules;
		RecurrenceRulePropertyCollection exceptionRules;
		ExceptionDateTimesPropertyCollection exceptionDateTimes;
		RecurrenceIdProperty recurrenceId;
		VAlarmCollection alarms;
		#endregion
		#region ctor
		public VEvent() {
			this.created = new DateTimeCreatedProperty(null, null);
			this.stamp = new DateTimeStampProperty(null, null);
			this.lastModified = new LastModifiedProperty(null, null);
			this.start = new DateTimeStartProperty(null, null);
			this.end = new DateTimeEndProperty(null, null);
			this.uid = new UniqueIdentifierProperty(null, null);
			this.description = new DescriptionProperty(null, null);
			this.location = new LocationProperty(null, null);
			this.summary = new SummaryProperty(null, null);
			this.categories = new CategoriesProperty(null, null);
			this.recurrenceRules = new RecurrenceRulePropertyCollection();
			this.exceptionRules = new RecurrenceRulePropertyCollection();
			this.exceptionDateTimes = new ExceptionDateTimesPropertyCollection();
			this.recurrenceId = new RecurrenceIdProperty(null, null);
			this.alarms = new VAlarmCollection();
		}
		#endregion
		#region Properties        
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventName")]
#endif
		public override string Name { get { return TokenName; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventCreated")]
#endif
		public DateTimeCreatedProperty Created { get { return created; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventStamp")]
#endif
		public DateTimeStampProperty Stamp { get { return stamp; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventLastModified")]
#endif
		public LastModifiedProperty LastModified { get { return lastModified; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventUniqueIdentifier")]
#endif
		public UniqueIdentifierProperty UniqueIdentifier { get { return uid; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventStart")]
#endif
		public DateTimeStartProperty Start { get { return start; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventEnd")]
#endif
		public DateTimeEndProperty End { get { return end; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventDescription")]
#endif
		public DescriptionProperty Description { get { return description; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventLocation")]
#endif
		public LocationProperty Location { get { return location; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventSummary")]
#endif
		public SummaryProperty Summary { get { return summary; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventCategories")]
#endif
		public CategoriesProperty Categories { get { return categories; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventRecurrenceRules")]
#endif
		public RecurrenceRulePropertyCollection RecurrenceRules { get { return recurrenceRules; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventExceptionRules")]
#endif
		public RecurrenceRulePropertyCollection ExceptionRules { get { return exceptionRules; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventExceptionDateTimes")]
#endif
		public ExceptionDateTimesPropertyCollection ExceptionDateTimes { get { return exceptionDateTimes; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventRecurrenceId")]
#endif
		public RecurrenceIdProperty RecurrenceId { get { return recurrenceId; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventAlarms")]
#endif
		public VAlarmCollection Alarms { get { return alarms; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventIsPattern")]
#endif
		public bool IsPattern { get { return RecurrenceRules.Count > 0; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventIsAllDay")]
#endif
		public bool IsAllDay { get { return CalculateIsAllDay(); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("VEventIsChangedOccurrence")]
#endif
		public bool IsChangedOccurrence { get { return !IsPattern && RecurrenceId != null && RecurrenceId.Value != DateTime.MinValue; } }
		protected internal override Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers {
			get { return supportedPropertyHandlers; }
		}
		protected internal override Dictionary<Type, ObtainComponentDelegate> SupportedComponentHandlers {
			get { return supportedComponentHandlers; }
		}
		#endregion
		void ApplyTimeZone(ISupportCalendarTimeZone impl, TimeZoneManager timeZoneManager) {
			if (impl == null)
				return;
			impl.ApplyTimeZone(timeZoneManager);
		}
		#region CalculateIsAllDay
		protected bool CalculateIsAllDay() {
			ValueDataTypes startDateType = Start.ValueDataType;
			if (End.Value == DateTime.MinValue) {
				if (startDateType == ValueDataTypes.Date)
					return true;
				return false;
			}
			ValueDataTypes endDateType = End.ValueDataType;
			if (Start.Value == End.Value)
				return false;
			if (endDateType == ValueDataTypes.Date && startDateType == ValueDataTypes.Date)
				return true;
			return false;
		}
		#endregion
		#region Supported Property Handlers
		static void AddDateTimeCreated(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).created = (DateTimeCreatedProperty)value;
		}
		static void AddDateTimeStamp(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).stamp = (DateTimeStampProperty)value;
		}
		static void AddLastModified(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).lastModified = (LastModifiedProperty)value;
		}
		static void AddDateTimeStart(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).start = (DateTimeStartProperty)value;
		}
		static void AddDateTimeEnd(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).end = (DateTimeEndProperty)value;
		}
		static void AddUniqueIdentifier(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).uid = (UniqueIdentifierProperty)value;
		}
		static void AddDescription(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).description = (DescriptionProperty)value;
		}
		static void AddLocation(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).location = (LocationProperty)value;
		}
		static void AddSummary(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).summary = (SummaryProperty)value;
		}
		static void AddRecurrenceRule(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).RecurrenceRules.Add((RecurrenceRuleProperty)value);
		}
		static void AddExceptionRule(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).ExceptionRules.Add((ExceptionRuleProperty)value);
		}
		static void AddExceptionDateTime(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).ExceptionDateTimes.Add((ExceptionDateTimesProperty)value);
		}
		static void AddRecurrenceId(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).recurrenceId = (RecurrenceIdProperty)value;
		}
		static void AddCategories(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VEvent)container).categories = (CategoriesProperty)value;
		}
		#endregion
		#region Supported Component Handlers
		static void AddAlarm(ICalendarBodyItemContainer container, iCalendarComponentBase value) {
			((VEvent)container).Alarms.Add((VAlarm)value);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			ApplyTimeZone(Start, timeZoneManager);
			ApplyTimeZone(End, timeZoneManager);
			ApplyTimeZone(ExceptionDateTimes, timeZoneManager);
			ApplyTimeZone(RecurrenceId, timeZoneManager);
			ApplyTimeZone(Alarms, timeZoneManager);			
		}
		#endregion
		#region SetTimeZoneManager
		internal void SetTimeZoneManager(TimeZoneManager timeZoneManager) {
			Start.SetTimeZoneManager(timeZoneManager);
			End.SetTimeZoneManager(timeZoneManager);
			ExceptionDateTimes.SetTimeZoneManager(timeZoneManager);
			RecurrenceId.SetTimeZoneManager(timeZoneManager);
		}
		#endregion
		#region CalculateSourceObjectCount
		protected internal virtual int CalculateSourceObjectCount() {
			int result = 1;
			if (!IsPattern)
				return result;
			result += ExceptionDateTimes.Count;
			int ruleCount = ExceptionRules.Count;
			for (int i = 0; i < ruleCount; i++) {
				result += CalculateRuleExceptionsCount(ExceptionRules[i].Value);
			}
			return result;
		}
		#endregion
		#region CalculateRuleExceptionsCount
		protected int CalculateRuleExceptionsCount(VRecurrenceRule rule) {
			int result = 0;
			return result;
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
			Created.WriteToStream(cw);
			Stamp.WriteToStream(cw);
			LastModified.WriteToStream(cw);
			Start.WriteToStream(cw);
			End.WriteToStream(cw);
			Description.WriteToStream(cw);
			Location.WriteToStream(cw);
			Summary.WriteToStream(cw);
			Categories.WriteToStream(cw);
			RecurrenceRules.WriteToStream(cw);
			ExceptionRules.WriteToStream(cw);
			ExceptionDateTimes.WriteToStream(cw);
			RecurrenceId.WriteToStream(cw);
			UniqueIdentifier.WriteToStream(cw);
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
			Alarms.WriteToString(cw);
		}
		#endregion
	}
	#endregion
	#region DateTimeCreatedProperty
	public class DateTimeCreatedProperty : DateTimePropertyBase {
		public const string TokenName = "CREATED";
		public DateTimeCreatedProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region DateTimeStampProperty
	public class DateTimeStampProperty : DateTimePropertyBase {
		public const string TokenName = "DTSTAMP";
		public DateTimeStampProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region LastModifiedProperty
	public class LastModifiedProperty : DateTimePropertyBase {
		public const string TokenName = "LAST-MODIFIED";
		public LastModifiedProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region DateTimeStartProperty
	public class DateTimeStartProperty : TimeZoneDateTimePropertyBase {
		public const string TokenName = "DTSTART";
		public DateTimeStartProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region DateTimeEndProperty
	public class DateTimeEndProperty : TimeZoneDateTimePropertyBase {
		public const string TokenName = "DTEND";
		public DateTimeEndProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region SummaryProperty
	public class SummaryProperty : TextPropertyBase {
		public const string TokenName = "SUMMARY";
		public SummaryProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region LocationProperty
	public class LocationProperty : TextPropertyBase {
		public const string TokenName = "LOCATION";
		public LocationProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region DescriptionProperty
	public class DescriptionProperty : TextPropertyBase {
		public const string TokenName = "DESCRIPTION";
		public DescriptionProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
	}
	#endregion
	#region UniqueIdentifierProperty
	public class UniqueIdentifierProperty : StringPropertyBase {
		public const string TokenName = "UID";
		public UniqueIdentifierProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region RecurrenceRulePropertyCollection
	public class RecurrenceRulePropertyCollection : List<RecurrenceRulePropertyBase>, IWritable {
		public void Add(VRecurrenceRule rule) {
			if (rule != null) {
				RecurrenceRuleProperty p = new RecurrenceRuleProperty(rule);
				p.Value = rule;
				this.Add(p);
			}
		}
		void IWritable.WriteToStream(iCalendarWriter cw) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].WriteToStream(cw);
		}
	}
	#endregion
	#region RecurrenceRulePropertyBase (abstract class)
	public abstract class RecurrenceRulePropertyBase : iCalendarPropertyBase {
		VRecurrenceRule value;
		#region ctor
		protected RecurrenceRulePropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.value = ParseStringValue(stringValue);
		}
		protected RecurrenceRulePropertyBase(VRecurrenceRule value)
			: base(null, null) {
			if (value == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentNullException("value");
			this.value = value;
		}
		#endregion
		public VRecurrenceRule Value { get { return value; } set { this.value = value; } }
		#region ParseStringValue
		protected internal virtual VRecurrenceRule ParseStringValue(string stringValue) {
			iContentLineParameters parameters = RecurrenceRuleContentLineParametersCreator.CreateContentLineParametersFromString(stringValue);
			return new VRecurrenceRule(parameters, String.Empty);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			string result = String.Empty;
			using (StringWriter sw = new StringWriter()) {
				iCalendarWriter cw = new iCalendarWriter(sw);
				Value.WriteToStream(cw);
				result = cw.ToString();
			}
			return result;
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region RecurrenceRuleProperty
	public class RecurrenceRuleProperty : RecurrenceRulePropertyBase {
		public const string TokenName = "RRULE";
		#region ctor
		public RecurrenceRuleProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public RecurrenceRuleProperty(VRecurrenceRule value)
			: base(value) {
		}
		#endregion
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region ExceptionRuleProperty
	public class ExceptionRuleProperty : RecurrenceRulePropertyBase {
		public const string TokenName = "EXRULE";
		public ExceptionRuleProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region DateTimeCollection
	public class DateTimeCollection : List<DateTime> {
	}
	#endregion
	#region ExceptionDateTimesProperty
	public class ExceptionDateTimesProperty : TimeZoneDateTimePropertyBase {
		public const string TokenName = "EXDATE";
		public ExceptionDateTimesProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		#region Properties
		public DateTimeCollection Values { get { return InnerValues; } }
		public override string Name { get { return TokenName; } }
		#endregion
	}
	#endregion
	#region ExceptionDateTimesPropertyCollection
	public class ExceptionDateTimesPropertyCollection : List<ExceptionDateTimesProperty>, ISupportCalendarTimeZone {
		TimeZoneManager timeZoneManager;
		void ApplyTimeZone(ISupportCalendarTimeZone impl, TimeZoneManager timeZoneManager) { 
			if (impl == null)
				return;
			impl.ApplyTimeZone(timeZoneManager);
		}
		protected void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				ApplyTimeZone(this[i], timeZoneManager);
			}
		}
		internal void SetTimeZoneManager(TimeZoneManager timeZoneManager) {
			this.timeZoneManager = timeZoneManager;
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].SetTimeZoneManager(timeZoneManager);
		}
		public int FindPropertyIndexByDate(DateTime val) {
			for (int i = 0; i < this.Count; i++) {
				if (this[i].Values.Contains(val))
					return i;
			}
			return -1;
		}
		internal void WriteToStream(iCalendarWriter cw) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].WriteToStream(cw);
		}
		public virtual void Add(DateTime dateTime) {
			ExceptionDateTimesProperty prop = new ExceptionDateTimesProperty(null, String.Empty);
			prop.Value = dateTime;
			prop.SetTimeZoneManager(timeZoneManager);
			Add(prop);
		}
		void ISupportCalendarTimeZone.ApplyTimeZone(TimeZoneManager timeZoneManager) {
			ApplyTimeZone(timeZoneManager);
		}
	}
	#endregion
	#region RecurrenceIdProperty
	public class RecurrenceIdProperty : TimeZoneDateTimePropertyBase {
		public const string TokenName = "RECURRENCE-ID";
		#region Static
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static RecurrenceIdProperty() {
			TimeZoneDateTimePropertyBaseCore(supportedParameterHandlers); 
			supportedParameterHandlers.Add(RecurrenceIdentifierRangeParameter.TokenName, AddRecurrenceIdentifierRange);
		}
		#endregion
		#region Fields
		RecurrenceIdentifierRangeParameter recurrenceIdentifierRange;
		#endregion
		public RecurrenceIdProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		#region Properties
		public RecurrenceIdentifierRangeParameter RecurrenceIdentifierRange { get { return recurrenceIdentifierRange ?? RecurrenceIdentifierRangeParameter.Empty; } }
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		public override string Name { get { return TokenName; } }
		#endregion
		#region Supported Parameter Handlers
		static void AddRecurrenceIdentifierRange(iCalendarBodyItem item, string[] values) {
			((RecurrenceIdProperty)item).recurrenceIdentifierRange = new RecurrenceIdentifierRangeParameter(values);
		}
		#endregion
	}
	#endregion
	#region CategoriesProperty
	public class CategoriesProperty : LanguageTextPropertyBase {
		public const string TokenName = "CATEGORIES";
		#region Fields
		List<string> categories;
		#endregion
		public CategoriesProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			string[] strings = iCalendarConvert.SeparateParameterValues(stringValue);
			this.categories = new List<string>(strings);
		}
		#region Properties
		public override string Name { get { return TokenName; } }
		public override string Value {
			get {
				if (Values != null && Values.Count > 0)
					return Values[0];
				return String.Empty;
			}
			set {
				if (Values != null && Values.Count > 0)
					Values[0] = value;
				else
					Values.Add(value);
			}
		}
		public List<string> Values { get { return categories; } }
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			return String.Join(iCalendarSymbols.ParamValueSeparator, Values.ToArray());
		}
		#endregion
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
}
