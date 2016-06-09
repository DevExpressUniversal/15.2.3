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
using System.IO;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	#region iCalendarParameterBase (abstract class)
	public abstract class iCalendarParameterBase : iCalendarObject, IWritable {
		#region Fields
		readonly string[] stringValues;
		#endregion
		#region ctor
		protected iCalendarParameterBase(string[] stringValues) {
			this.stringValues = new string[stringValues.Length];
			stringValues.CopyTo(this.stringValues, 0);
		}
		protected iCalendarParameterBase() {
			this.stringValues = new string[] { String.Empty };
		}
		#endregion
		#region Properties
		internal string[] StringValues { get { return stringValues; } }
		protected virtual string ParameterStartString { get { return iCalendarSymbols.ParameterStart; } }
		#endregion
		#region WriteToStream
		protected virtual void WriteToStream(iCalendarWriter cw) {
			string values = GetStringValues();
			if (String.IsNullOrEmpty(values))
				return;
			WriteNameToStream(cw);
			WriteValuesToStream(cw, values);
		}
		#endregion
		#region ValuesToString
		protected abstract string[] ValuesToString();
		#endregion
		#region WriteNameToStream
		protected virtual void WriteNameToStream(iCalendarWriter cw) {
			string parameterString = ParameterStartString + Name + iCalendarSymbols.ParamValueStartChar;
			cw.Write(parameterString);
		}
		#endregion
		#region GetStringValues
		protected string GetStringValues() {
			string[] values = ValuesToString();
			return String.Join(iCalendarSymbols.ParamValueSeparator, values);
		}
		#endregion
		#region WriteValuesToStream
		protected virtual void WriteValuesToStream(iCalendarWriter cw, string values) {
			XtraSchedulerDebug.Assert(!String.IsNullOrEmpty(values));
			cw.Write(values);
		}
		#endregion
		void IWritable.WriteToStream(iCalendarWriter cw) {
			WriteToStream(cw);
		}
	}
	#endregion
	#region CustomParameter
	public class CustomParameter : iCalendarParameterBase {
		string name;
		public CustomParameter(string name, string[] values)
			: base(values) {
			this.name = name;
		}
		public override string Name { get { return name; } }
		#region ValuesToString
		protected override string[] ValuesToString() {
			return StringValues;
		}
		#endregion
	}
	#endregion
	#region TimeZoneIdentifierParameter
	public class TimeZoneIdentifierParameter : iCalendarParameterBase {
		public const string TokenName = "TZID";
		string value;
		#region ctor
		public TimeZoneIdentifierParameter(string[] values)
			: base(values) {
			this.value = iCalendarConvert.UnquoteString(values[0]);
		}
		public TimeZoneIdentifierParameter(string value) {
			this.value = iCalendarConvert.DquoteString(value);
		}
		#endregion
		#region Properties
		public string Value { get { return value; } set { this.value = value; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			return new string[] { Value };
		}
		#endregion
	}
	#endregion
	#region LanguageParameter
	public class LanguageParameter : iCalendarParameterBase {
		public const string TokenName = "LANGUAGE";
		string value;
		#region ctor
		public LanguageParameter(string[] values)
			: base(values) {
			this.value = StringValues[0];
		}
		public LanguageParameter(string value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public string Value { get { return value; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			return new string[] { value };
		}
		#endregion
	}
	#endregion
	#region AlternateTextRepresentationParameter
	public class AlternateTextRepresentationParameter : iCalendarParameterBase {
		public const string TokenName = "ALTREP";
		Uri uri;
		#region ctor
		public AlternateTextRepresentationParameter(string[] values)
			: base(values) {
			if (!string.IsNullOrEmpty(values[0]))
				this.uri = new Uri(values[0]);
		}
		public AlternateTextRepresentationParameter(Uri value) {
			this.uri = value;
		}
		#endregion
		#region Properties
		public Uri Value { get { return uri; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			return new string[] { String.Format("\"{0}\"", uri.ToString()) };
		}
		#endregion
	}
	#endregion
	#region ValueDataType
	public enum ValueDataTypes {
		Binary,
		Boolean,
		CalAddress,
		Date,
		DateTime,
		Duration,
		Float,
		Integer,
		Period,
		Recur,
		Text,
		Time,
		Uri,
		UtcOffset,
		Custom
	}
	#endregion
	#region ValueDataTypeParameter
	public class ValueDataTypeParameter : iCalendarParameterBase {
		public const string TokenName = "VALUE";
		#region Static
		static TokenValueMatcher<ValueDataTypes> supportedValues = new TokenValueMatcher<ValueDataTypes>();
		static ValueDataTypeParameter() {
			SupportedValues.RegisterToken("BINARY", ValueDataTypes.Binary);
			SupportedValues.RegisterToken("BOOLEAN", ValueDataTypes.Boolean);
			SupportedValues.RegisterToken("CAL-ADDRESS", ValueDataTypes.CalAddress);
			SupportedValues.RegisterToken("DATE", ValueDataTypes.Date);
			SupportedValues.RegisterToken("DATE-TIME", ValueDataTypes.DateTime);
			SupportedValues.RegisterToken("DURATION", ValueDataTypes.Duration);
			SupportedValues.RegisterToken("FLOAT", ValueDataTypes.Float);
			SupportedValues.RegisterToken("INTEGER", ValueDataTypes.Integer);
			SupportedValues.RegisterToken("PERIOD", ValueDataTypes.Period);
			SupportedValues.RegisterToken("RECUR", ValueDataTypes.Recur);
			SupportedValues.RegisterToken("TEXT", ValueDataTypes.Text);
			SupportedValues.RegisterToken("TIME", ValueDataTypes.Time);
			SupportedValues.RegisterToken("URI", ValueDataTypes.Uri);
			SupportedValues.RegisterToken("UTC-OFFSET", ValueDataTypes.UtcOffset);
		}
		static TokenValueMatcher<ValueDataTypes> SupportedValues { get { return supportedValues; } }
		#endregion
		#region Fields
		ValueDataTypes value;
		#endregion
		#region ctor
		public ValueDataTypeParameter(string[] values)
			: base(values) {
			this.value = ParseStringValue(values[0]);
		}
		public ValueDataTypeParameter(ValueDataTypes value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public ValueDataTypes Value { get { return value; } set { this.value = value; } }
		#endregion
		#region ParseStringValue
		protected internal virtual ValueDataTypes ParseStringValue(string stringValue) {
			stringValue = stringValue.ToUpper();
			if (SupportedValues.IsTokenRegistered(stringValue))
				return SupportedValues.ValueFromToken(stringValue);
			return ValueDataTypes.Custom;
		}
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			string valueString = SupportedValues.IsValueRegistered(Value) ? SupportedValues.TokenFromValue(Value) : String.Empty;
			return new string[] { valueString };
		}
		#endregion
	}
	#endregion
	#region RecurrenceIdentifierRange
	public enum RecurrenceIdentifierRange { Default, ThisAndPrior, ThisAndFuture };
	#endregion
	#region RecurrenceIdentifierRangeParameter
	public class RecurrenceIdentifierRangeParameter : iCalendarParameterBase {
		public const string TokenName = "RANGE";
		#region Static
		static TokenValueMatcher<RecurrenceIdentifierRange> supportedValues = new TokenValueMatcher<RecurrenceIdentifierRange>();
		static RecurrenceIdentifierRangeParameter empty = new RecurrenceIdentifierRangeParameter();
		static RecurrenceIdentifierRangeParameter() {
			supportedValues.RegisterToken("THISANDPRIOR", RecurrenceIdentifierRange.ThisAndPrior);
			supportedValues.RegisterToken("THISANDFUTURE", RecurrenceIdentifierRange.ThisAndFuture);
		}
		static TokenValueMatcher<RecurrenceIdentifierRange> SupportedValues { get { return supportedValues; } }
		public static RecurrenceIdentifierRangeParameter Empty { get { return empty; } }
		#endregion
		#region Fields
		RecurrenceIdentifierRange value;
		#endregion
		public RecurrenceIdentifierRangeParameter(string[] stringValues)
			: base(stringValues) {
			this.value = ParseStringValue(stringValues[0]);
		}
		RecurrenceIdentifierRangeParameter()
			: base(new string[] { String.Empty }) {
			this.value = RecurrenceIdentifierRange.Default;
		}
		#region Properties
		public RecurrenceIdentifierRange Value { get { return value; } }
		public override string Name { get { return TokenName; } }
		#endregion
		#region ParseStringValue
		protected internal virtual RecurrenceIdentifierRange ParseStringValue(string stringValue) {
			if (SupportedValues.IsTokenRegistered(stringValue))
				return SupportedValues.ValueFromToken(stringValue);
			throw new iCalendarParseErrorException();
		}
		#endregion
		protected override string[] ValuesToString() {
			throw new Exception("The method or operation is not implemented.");
		}
	}
	#endregion
	#region AlarmTriggerRelationship
	public enum AlarmTriggerRelationship { Start, End };
	#endregion
	#region AlarmTriggerRelationshipParameter
	public class AlarmTriggerRelationshipParameter : iCalendarParameterBase {
		public const string TokenName = "RELATED";
		#region Static
		static TokenValueMatcher<AlarmTriggerRelationship> supportedValues = new TokenValueMatcher<AlarmTriggerRelationship>();
		static AlarmTriggerRelationshipParameter() {
			SupportedValues.RegisterToken("START", AlarmTriggerRelationship.Start);
			SupportedValues.RegisterToken("END", AlarmTriggerRelationship.End);
		}
		static TokenValueMatcher<AlarmTriggerRelationship> SupportedValues { get { return supportedValues; } }
		#endregion
		#region Fields
		AlarmTriggerRelationship value;
		#endregion
		#region ctor
		public AlarmTriggerRelationshipParameter(string[] values)
			: base(values) {
			this.value = ParseStringValue(values[0]);
		}
		public AlarmTriggerRelationshipParameter(AlarmTriggerRelationship value) {
			this.value = value;
		}
		#endregion
		#region Properties
		public override string Name { get { return TokenName; } }
		public AlarmTriggerRelationship Value { get { return value; } }
		#endregion
		#region ParseStringValue
		protected internal virtual AlarmTriggerRelationship ParseStringValue(string p) {
			if (String.IsNullOrEmpty(p))
				return AlarmTriggerRelationship.Start;
			if (SupportedValues.IsTokenRegistered(p))
				return SupportedValues.ValueFromToken(p);
			throw new iCalendarParseErrorException();
		}
		#endregion
		#region ValuesToString
		protected override string[] ValuesToString() {
			if (Value != AlarmTriggerRelationship.Start && SupportedValues.IsValueRegistered(Value))
				return new string[] { SupportedValues.TokenFromValue(Value) };
			return new string[] { String.Empty };
		}
		#endregion
	}
	#endregion
}
