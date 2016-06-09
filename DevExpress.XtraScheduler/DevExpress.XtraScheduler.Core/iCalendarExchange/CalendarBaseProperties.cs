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
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public delegate void ObtainParameterDelegate(iCalendarBodyItem item, string[] values);
	#region iCalendarPropertyCollection
	public class iCalendarPropertyCollection : iCalendarNamedObjectCollection<iCalendarPropertyBase> {
	}
	#endregion
	#region iCalendarPropertyBase (abstract class)
	public abstract class iCalendarPropertyBase : iCalendarBodyItem {
		static readonly Dictionary<string, ObtainParameterDelegate> emptyParameterHandlers;
		static iCalendarPropertyBase() {
			emptyParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		}
		#region Fields
		readonly iCalendarNamedObjectCollection<CustomParameter> customParameters;
		string stringValue;
		#endregion
		protected iCalendarPropertyBase(iContentLineParameters parameters, string stringValue) {
			this.customParameters = new iCalendarNamedObjectCollection<CustomParameter>();
			this.stringValue = stringValue;
			AddParameters(parameters);
		}
		#region Properties
		protected internal virtual Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers { get { return emptyParameterHandlers; } }
		public iCalendarNamedObjectCollection<CustomParameter> CustomParameters { get { return customParameters; } }
		internal string StringValue { get { return stringValue; } }
		#endregion
		#region AddParameters
		protected virtual void AddParameters(iContentLineParameters parameters) {
			if (parameters == null)
				return;
			int count = parameters.Count;
			for (int i = 0; i < count; i++) {
				iContentLineParam param = parameters[i];
				if (CanAddParameter(param))
					AddParameter(param);
			}
		}
		#endregion
		#region CanAddParameter
		protected internal virtual bool CanAddParameter(iContentLineParam param) {
			return true;
		}
		#endregion
		#region AddParameter
		protected internal virtual void AddParameter(iContentLineParam param) {
			ObtainParameterDelegate handler;
			if (SupportedParameterHandlers.TryGetValue(param.Name, out handler))
				handler(this, param.Values.ToArray());
			else
				AddCustomParameter(param);
		}
		#endregion
		#region AddCustomParameter
		protected void AddCustomParameter(iContentLineParam parameter) {
			customParameters.Add(new CustomParameter(parameter.Name, parameter.Values.ToArray()));
		}
		#endregion
		#region WriteToStream
		protected override void WriteToStream(iCalendarWriter cw) {
			string stringValue = ConvertValueToString();
			if (String.IsNullOrEmpty(stringValue))
				return;
			cw.Write(Name);
			WriteParametersToStream(cw);
			WriteCustomParametersToStream(cw);
			WriteValueToStream(cw, stringValue);
			cw.WriteLine();
		}
		#endregion
		#region WriteCustomParametersToStream
		protected void WriteCustomParametersToStream(iCalendarWriter cw) {
			int count = CustomParameters.Count;
			for (int i = 0; i < count; i++) {
				CustomParameter param = CustomParameters[i];
				param.WriteToStream(cw);
			}
		}
		#endregion
		#region WriteParametersToStream
		protected abstract void WriteParametersToStream(iCalendarWriter cw);
		#endregion
		#region WriteValueToStream
		protected virtual void WriteValueToStream(iCalendarWriter cw, string stringValue) {
			cw.Write(iCalendarSymbols.ValueStartChar + stringValue);
		}
		#endregion
		#region ConvertValueToString
		protected virtual string ConvertValueToString() {
			return String.Empty;
		}
		#endregion
		public abstract object GetValue();
	}
	#endregion
	#region StringPropertyBase (abstract class)
	public abstract class StringPropertyBase : iCalendarPropertyBase {
		string value;
		#region ctor
		protected StringPropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.value = iCalendarConvert.UnescapeString(stringValue);
		}
		protected StringPropertyBase(string value)
			: base(null, String.Empty) {
			this.value = value;
		}
		#endregion
		public virtual string Value { get { return value; } set { this.value = value; } }
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			return iCalendarConvert.EscapeString(value);
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region UtcOffsetPropertyBase (abstract class)
	public abstract class UtcOffsetPropertyBase : iCalendarPropertyBase {
		TimeSpan value;
		protected UtcOffsetPropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.value = ParseStringValue(stringValue);
		}
		public TimeSpan Value { get { return value; } set { this.value = value; } }
		#region ParseStringValue
		protected virtual TimeSpan ParseStringValue(string stringValue) {
			return iCalendarConvert.ToUtcOffset(stringValue);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			if (Value == iCalendarUtils.DefaultTimeSpan)
				return String.Empty;
			return iCalendarConvert.FromUtcOffset(Value);
		}
		#endregion
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region DateTimePropertyBase (abstract class)
	public abstract class DateTimePropertyBase : iCalendarPropertyBase {
		#region Fields
		DateTimeCollection values;
		TimeZoneManager timeZoneManager;
		#endregion
		#region ctor
		protected DateTimePropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
			this.values = ParseStringValues(stringValue);
		}
		#endregion
		#region Properties
		#region Value
		public DateTime Value {
			get {
				if (values.Count < 1)
					return iCalendarUtils.DefaultDateTime;
				return values[0];
			}
			set {
				if (InnerValues.Count < 1)
					InnerValues.Add(value);
				InnerValues[0] = value;
			}
		}
		#endregion
		internal DateTimeCollection InnerValues { get { return values; }  }
		internal TimeZoneManager TimeZoneManager { get { return timeZoneManager; } }
		#endregion
		#region ParseStringValues
		protected internal virtual DateTimeCollection ParseStringValues(string stringValue) {
			DateTimeCollection result = new DateTimeCollection();
			string[] stringValues = iCalendarConvert.SeparateParameterValues(stringValue);
			int count = stringValues.Length;
			for (int i = 0; i < count; i++) {
				string itemStringValue = stringValues[i];
				DateTime itemValue = iCalendarConvert.ToDateTime(itemStringValue);
				result.Add(itemValue);
			}
			return result;
		}
		#endregion
		#region ConvertValueToString
		protected override string ConvertValueToString() {
			List<string> result = new List<string>();
			int count = InnerValues.Count;
			for (int i = 0; i < count; i++) {
				string itemStringValue = ConvertItemValueToString(InnerValues[i]);
				result.Add(itemStringValue);
			}
			return String.Join(iCalendarSymbols.ParamValueSeparator, result.ToArray());
		}
		#endregion
		#region ConvertItemValueToString
		protected virtual string ConvertItemValueToString(DateTime itemValue) {
			string itemStringValue = iCalendarConvert.FromDateTime(itemValue, true);
			return itemStringValue;
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			SetTimeZoneManager(timeZoneManager);
			int count = InnerValues.Count;
			for (int i = 0; i < count; i++) {
				DateTime date = InnerValues[i];
				if (date.Kind == DateTimeKind.Utc) {
					InnerValues[i] = timeZoneManager.ToLocalTimeFromUtc(date);
				}
			}
		}
		#endregion
		internal void SetTimeZoneManager(TimeZoneManager timeZoneManager) {
			this.timeZoneManager = timeZoneManager;			
		}
		#region GetValue
		public override object GetValue() {
			return Value;
		}
		#endregion
	}
	#endregion
	#region TimeZoneDateTimePropertyBase (abstract class)
	public abstract class TimeZoneDateTimePropertyBase : DateTimePropertyBase {
		#region Static
		public static ValueDataTypes DefaultValueDataType = ValueDataTypes.DateTime;
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static TimeZoneDateTimePropertyBase() {
			TimeZoneDateTimePropertyBaseCore(supportedParameterHandlers);
		}
		protected static void TimeZoneDateTimePropertyBaseCore(Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers) {
			supportedParameterHandlers.Add(TimeZoneIdentifierParameter.TokenName, AddTimeZoneIdentifier);
			supportedParameterHandlers.Add(ValueDataTypeParameter.TokenName, AddValueDataType);
		}
		#endregion
		#region Fields
		TimeZoneIdentifierParameter timeZoneIdentifierParam;
		ValueDataTypeParameter valueDataTypeParam;		
		#endregion
		#region ctor
		protected TimeZoneDateTimePropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		#endregion
		#region Properties
		protected internal ValueDataTypeParameter ValueDataTypeParam { get { return valueDataTypeParam; } }
		protected internal TimeZoneIdentifierParameter TimeZoneIdentifierParam { get { return timeZoneIdentifierParam; } }
		public string TimeZoneIdentifier {
			get { return TimeZoneIdentifierParam != null ? TimeZoneIdentifierParam.Value : string.Empty; }
			set {
				timeZoneIdentifierParam = new TimeZoneIdentifierParameter(value);
			}
		}
		public ValueDataTypes ValueDataType {
			get {
				return ValueDataTypeParam != null ? ValueDataTypeParam.Value : DefaultValueDataType;
			}
			set {
				if (value != DefaultValueDataType) 
					valueDataTypeParam = new ValueDataTypeParameter(value);
			}
		}
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		TimeZoneInfo CurrentTimeZone {
			get {
				if (TimeZoneManager == null)
					return TimeZoneEngine.Local;
				return TimeZoneManager.CurrentTimeZone;
			}
		}
		#endregion
		#region Supported Property Handlers
		static void AddTimeZoneIdentifier(iCalendarBodyItem item, string[] values) {
			((TimeZoneDateTimePropertyBase)item).timeZoneIdentifierParam = new TimeZoneIdentifierParameter(values);
		}
		static void AddValueDataType(iCalendarBodyItem item, string[] values) {
			((TimeZoneDateTimePropertyBase)item).valueDataTypeParam = new ValueDataTypeParameter(values);
		}
		#endregion
		#region ConvertItemValueToString
		protected override string ConvertItemValueToString(DateTime itemValue) {
			if (ValueDataType == ValueDataTypes.Date)
				return iCalendarConvert.FromDate(Value);
			bool utc = (TimeZoneIdentifierParam == null) ? true : false;
			DateTime dateTime = (utc && Value.Kind != DateTimeKind.Utc) ? TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(Value, DateTimeKind.Unspecified), CurrentTimeZone) : Value;
			return iCalendarConvert.FromDateTime(dateTime, utc);
		}
		#endregion
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
			if (TimeZoneIdentifierParam != null)
				TimeZoneIdentifierParam.WriteToStream(cw);
			if (ValueDataTypeParam != null)
				ValueDataTypeParam.WriteToStream(cw);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			SetTimeZoneManager(timeZoneManager);
			int count = InnerValues.Count;
			for (int i = 0; i < count; i++) {
				DateTime date = InnerValues[i];
				if (date.Kind == DateTimeKind.Local)
					InnerValues[i] = timeZoneManager.ToLocalTime(date, TimeZoneIdentifier);
				else if (date.Kind == DateTimeKind.Utc) {
					InnerValues[i] = timeZoneManager.ToLocalTimeFromUtc(date);
				}
			}
		}
		public void ApplyTimeZone() {
			LazyTimeZoneManager tzm = new LazyTimeZoneManager();
			ApplyTimeZone(tzm);
		}
		#endregion
	}
	#endregion
	#region TextPropertyBase (abstract class)
	public abstract class TextPropertyBase : StringPropertyBase {
		#region Static
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static TextPropertyBase() {
			supportedParameterHandlers.Add(LanguageParameter.TokenName, AddLanguage);
			supportedParameterHandlers.Add(AlternateTextRepresentationParameter.TokenName, AddAlternateTextRepresentation);
		}
		#endregion
		#region Fields
		LanguageParameter language;
		AlternateTextRepresentationParameter altTextRepresent;
		#endregion
		#region ctor
		protected TextPropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		protected TextPropertyBase(string value)
			: base(value) {
		}
		#endregion
		#region Properties
		protected internal LanguageParameter LanguageParam { get { return language; } }
		protected internal AlternateTextRepresentationParameter AltTextRepresentationParam { get { return altTextRepresent; } }
		public string Language {
			get { return LanguageParam != null ? LanguageParam.Value : string.Empty; }
			set { this.language = new LanguageParameter(value); }
		}
		public Uri AlternateTextRepresentation {
			get { return AltTextRepresentationParam != null ? AltTextRepresentationParam.Value : null; }
			set { this.altTextRepresent = new AlternateTextRepresentationParameter(value); }
		}
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		#endregion
		#region Supported Property Handlers
		static void AddLanguage(iCalendarBodyItem item, string[] values) {
			((TextPropertyBase)item).language = new LanguageParameter(values);
		}
		static void AddAlternateTextRepresentation(iCalendarBodyItem item, string[] values) {
			((TextPropertyBase)item).altTextRepresent = new AlternateTextRepresentationParameter(values);
		}
		#endregion
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
			if (LanguageParam != null)
				LanguageParam.WriteToStream(cw);
			if (AltTextRepresentationParam != null)
				AltTextRepresentationParam.WriteToStream(cw);
		}
		#endregion
	}
	#endregion
	#region LanguageTextPropertyBase (abstract class)
	public abstract class LanguageTextPropertyBase : StringPropertyBase {
		#region static
		static readonly Dictionary<string, ObtainParameterDelegate> supportedParameterHandlers = new Dictionary<string, ObtainParameterDelegate>();
		static LanguageTextPropertyBase() {
			supportedParameterHandlers.Add(LanguageParameter.TokenName, AddLanguage);
		}
		#endregion
		LanguageParameter language;
		#region ctor
		protected LanguageTextPropertyBase(iContentLineParameters parameters, string stringValue)
			: base(parameters, stringValue) {
		}
		protected LanguageTextPropertyBase(string value)
			: base(value) {
		}
		#endregion
		#region Prperties
		protected internal LanguageParameter LanguageParam { get { return language; } }
		public string Language { get { return LanguageParam != null ? LanguageParam.Value : string.Empty; } }
		protected internal override Dictionary<string, ObtainParameterDelegate> SupportedParameterHandlers {
			get { return supportedParameterHandlers; }
		}
		#endregion
		#region Supported Property Handlers
		static void AddLanguage(iCalendarBodyItem item, string[] values) {
			((LanguageTextPropertyBase)item).language = new LanguageParameter(values);
		}
		#endregion
	}
	#endregion
	#region CustomProperty
	public class CustomProperty : StringPropertyBase, ICalendarNamedObject {
		string name;
		internal CustomProperty(string name, iContentLineParameters parameters, string value)
			: base(parameters, value) {
			this.name = name;
		}
		public override string Name { get { return name; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
	public class TextProperty : StringPropertyBase, ICalendarNamedObject {
		string name;
		public TextProperty(string name, string value)
			: base(value) {
			if (string.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentException("name", name);
			this.name = name;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new iCalendarNamedObjectCollection<CustomParameter> CustomParameters { get { return base.CustomParameters; } }
		public void AddParameter(string name, string value) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentNullException("name");
			List<string> list = new List<string>();
			list.Add(value);
			AddParameter(name, list);
		}
		public void AddParameter(string name, List<string> values) {
			if (String.IsNullOrEmpty(name))
				Exceptions.ThrowArgumentNullException("name");
			iContentLineParam param = new iContentLineParam();
			param.Name = name;
			param.Values.AddRange(values);
			base.AddParameter(param);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TextPropertyName")]
#endif
		public override string Name { get { return name; } }
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
	}
}
