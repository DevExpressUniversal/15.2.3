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
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	#region VAlarmCollection
	public class VAlarmCollection : List<VAlarm>, ISupportCalendarTimeZone, IWritable {
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
		void ISupportCalendarTimeZone.ApplyTimeZone(TimeZoneManager timeZoneManager) {
			ApplyTimeZone(timeZoneManager);
		}
	}
	#endregion
	#region VAlarm
	public class VAlarm : iCalendarComponentBase {
		public const string TokenName = "VALARM";
		#region Static
		static readonly Dictionary<Type, ObtainPropertyDelegate> supportedPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
		static VAlarm() {
			supportedPropertyHandlers.Add(typeof(TriggerProperty), AddTrigger);
		}
		#endregion
		#region Fields
		TriggerProperty trigger;
		ActionProperty action;
		#endregion
		public VAlarm() {
			this.trigger = new TriggerProperty(null, null);
			this.action = new ActionProperty(null, null);
		}
		#region Properties
		public override string Name { get { return TokenName; } }
		public TriggerProperty Trigger { get { return trigger; } }
		public ActionProperty Action { get { return action; } }
		protected internal override Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers {
			get { return supportedPropertyHandlers; }
		}
		#endregion
		#region Supported Property Handlers
		static void AddTrigger(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((VAlarm)container).trigger = (TriggerProperty)value;
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			ISupportCalendarTimeZone impl = Trigger as ISupportCalendarTimeZone;
			if (impl == null)
				return;
			impl.ApplyTimeZone(timeZoneManager);
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
			Trigger.WriteToStream(cw);
			Action.WriteToStream(cw);
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	#region TriggerProperty
	public class TriggerProperty : iCalendarPropertyBase {
		public const string TokenName = "TRIGGER";
		#region Static
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static TriggerProperty() {
			supportedParameterHandlers.Add(ValueDataTypeParameter.TokenName, AddValueDataType);
			supportedParameterHandlers.Add(AlarmTriggerRelationshipParameter.TokenName, AddAlarmTriggerRelationship);
		}
		#endregion
		#region Fields
		long value;
		ValueDataTypeParameter valueDataTypeParam;
		AlarmTriggerRelationshipParameter alarmTriggerRelationshipParam;
		#endregion
		public TriggerProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.value = ParseStringValue(stringValue);
		}
		#region Properties
		protected internal ValueDataTypeParameter ValueDataTypeParam { get { return valueDataTypeParam; } }
		protected internal AlarmTriggerRelationshipParameter AlarmTriggerRelationshipParam { get { return alarmTriggerRelationshipParam; } }
		public long Value { get { return value; } set { this.value = value; } }
		public override string Name { get { return TokenName; } }
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		public ValueDataTypes ValueDataType {
			get { return valueDataTypeParam != null ? valueDataTypeParam.Value : ValueDataTypes.Duration; }
			set { this.valueDataTypeParam = new ValueDataTypeParameter(value); }
		}
		public AlarmTriggerRelationship AlarmTriggerRelationship {
			get { return alarmTriggerRelationshipParam != null ? AlarmTriggerRelationshipParam.Value : AlarmTriggerRelationship.Start; }
			set { this.alarmTriggerRelationshipParam = new AlarmTriggerRelationshipParameter(value); }
		}
		#endregion
		#region ParseStringValue
		protected internal virtual long ParseStringValue(string stringValue) {
			if (String.IsNullOrEmpty(stringValue))
				return long.MinValue;
			TimeSpan duration = iCalendarUtils.DefaultTimeSpan;
			if (iCalendarConvert.TryParseDuration(stringValue, out duration))
				return duration.Ticks;
			DateTime dateTime = iCalendarConvert.ToDateTime(stringValue);
			return dateTime.Ticks;
		}
		#endregion
		#region Supported Parameter Handlers
		static void AddValueDataType(iCalendarBodyItem item, string[] values) {
			TriggerProperty triggerProperty = (TriggerProperty)item;
			triggerProperty.valueDataTypeParam = new ValueDataTypeParameter(values);
			ValueDataTypes dataType = triggerProperty.ValueDataType;
			if (dataType != ValueDataTypes.DateTime && dataType != ValueDataTypes.Duration)
				throw new iCalendarParseErrorException();
		}
		static void AddAlarmTriggerRelationship(iCalendarBodyItem item, string[] values) {
			((TriggerProperty)item).alarmTriggerRelationshipParam = new AlarmTriggerRelationshipParameter(values);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			if (ValueDataType == ValueDataTypes.DateTime) {
				DateTime dateTime = new DateTime(Value);
				this.value = timeZoneManager.ToLocalTimeFromUtc(dateTime).Ticks;
			}
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			return iCalendarConvert.FromDuration(new TimeSpan(Value));
		}
		#endregion
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
			if (ValueDataTypeParam != null)
				ValueDataTypeParam.WriteToStream(cw);
			if (AlarmTriggerRelationshipParam != null)
				AlarmTriggerRelationshipParam.WriteToStream(cw);
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region ActionValue
	public enum ActionValue {
		Audio,
		Display,
		Email,
		Procedure,
		Custom
	}
	#endregion
	#region ActionProperty
	public class ActionProperty : iCalendarPropertyBase {
		public const string TokenName = "ACTION";
		#region Static
		static readonly TokenValueMatcher<ActionValue> supportedValues = new TokenValueMatcher<ActionValue>();
		static ActionProperty() {
			SupportedValues.RegisterToken("AUDIO", ActionValue.Audio);
			SupportedValues.RegisterToken("DISPLAY", ActionValue.Display);
			SupportedValues.RegisterToken("EMAIL", ActionValue.Email);
			SupportedValues.RegisterToken("PROCEDURE", ActionValue.Procedure);
		}
		static TokenValueMatcher<ActionValue> SupportedValues { get { return supportedValues; } }
		#endregion
		#region Fields
		ActionValue value = ActionValue.Display;
		#endregion
		public ActionProperty(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.value = ParseStringValue(stringValue);
		}
		#region Properties
		public ActionValue Value { get { return value; } set { this.value = value; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ParseStringValue
		ActionValue ParseStringValue(string stringValue) {
			if (String.IsNullOrEmpty(stringValue))
				return ActionValue.Display;
			if (SupportedValues.IsTokenRegistered(stringValue))
				return SupportedValues.ValueFromToken(stringValue);
			return ActionValue.Custom;
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			if (SupportedValues.IsValueRegistered(Value))
				return SupportedValues.TokenFromValue(Value);
			return String.Empty;
		}
		#endregion
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
}
