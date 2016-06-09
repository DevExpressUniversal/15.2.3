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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.VCalendar {
	public abstract class VObjectEncoder {
		readonly VObjectBase vObject;
		TextWriter writer;
		protected VObjectEncoder(VObjectBase vObject) {
			this.vObject = vObject;
		}
		protected VObjectBase VObject { get { return vObject; } }
		protected TextWriter Writer { get { return writer; } }
		public virtual void Encode(TextWriter writer) {
			this.writer = writer;
			EncodeHeader();
			EncodeProperties();
			EncodeSubItems();
			EncodeFooter();
		}
		protected virtual void EncodeHeader() {
		}
		protected virtual void EncodeFooter() {
		}
		protected virtual void EncodeProperties() {
		}
		protected virtual void EncodeSubItems() {
		}
		protected string CreatePropertyParameters(string propName, string val, VCalendarEncoding encoding, string charSet, string language) {
			return String.Empty;
		}
		protected VCalendarParameter CreateQuotedPrintableParameter() {
			return new VCalendarParameter(VPropertyParameterNames.Encoding, VEncodingParameterValues.QuotedPrintable);
		}
		protected string CreateQuotedPrintablePropertyPrefix(string name) {
			return CreatePropertyString(name, new VCalendarParameter[] { CreateQuotedPrintableParameter() }, String.Empty);
		}
		protected string CreatePropertyString(string name, VCalendarParameter[] parameters, string val) {
			string s = name;
			for (int i = 0; i < parameters.Length; i++) {
				s += ";" + parameters[i].ToString();
			}
			return String.Format("{0}:{1}", s, val);
		}
		protected void WriteProperty(string name, string val) {
			WriteProperty(name, new VCalendarParameter[0], val);
		}
		protected void WriteProperty(string name, VCalendarParameter[] parameters, string val) {
			WriteStringCore(CreatePropertyString(name, parameters, val));
		}
		protected void WriteStringCore(string val) {
			Writer.WriteLine(val);
		}
	}
	public class VCalendarEncoder : VObjectEncoder {
		public VCalendarEncoder(VCalendarObject calendar)
			: base(calendar) {
		}
		protected VCalendarObject Calendar { get { return (VCalendarObject)VObject; } }
		protected override void EncodeHeader() {
			Writer.WriteLine(VCalendarTag.BeginCalendar);
		}
		protected override void EncodeFooter() {
			Writer.WriteLine(VCalendarTag.EndCalendar);
		}
		protected override void EncodeProperties() {
			WriteProperty(VCalendarPropertyNames.Version, Calendar.Version);
			WriteProperty(VCalendarPropertyNames.ProductIdentifier, Calendar.ProductId);
		}
		protected override void EncodeSubItems() {
			EncodeEvents();
		}
		void EncodeEvents() {
			foreach (VEvent item in Calendar.EventList) {
				VObjectEncoder encoder = item.GetEncoder();
				encoder.Encode(Writer);
			}
		}
	}
	public class VEventEncoder : VObjectEncoder {
		public VEventEncoder(VEvent ev)
			: base(ev) {
		}
		protected VEvent Event { get { return (VEvent)VObject; } }
		protected override void EncodeHeader() {
			Writer.WriteLine(VCalendarTag.BeginEvent);
		}
		protected override void EncodeFooter() {
			Writer.WriteLine(VCalendarTag.EndEvent);
		}
		protected override void EncodeProperties() {
			WriteProperty(VEventPropertyNames.DtStart, VCalendarUtils.ToVCalendarDateTime(Event.DTStart));
			WriteProperty(VEventPropertyNames.DtEnd, VCalendarUtils.ToVCalendarDateTime(Event.DTEnd));
			if (Event.TimeTransparency > 0)
				WriteProperty(VEventPropertyNames.TimeTransparency, Event.TimeTransparency.ToString());
			EncodeQuotedPrintableProperty(VEventPropertyNames.Location, Event.Location);
			EncodeQuotedPrintableProperty(VEventPropertyNames.Description, Event.Description);
			EncodeQuotedPrintableProperty(VEventPropertyNames.Summary, Event.Summary);
			WriteProperty(VEventPropertyNames.Priority, Event.Priority.ToString());
			EncodeRecurrenceRule(VEventPropertyNames.RecurrenceRule, Event.RecurrenceRules);
			EncodeRecurrenceRule(VEventPropertyNames.ExceptionRule, Event.ExceptionRules);
			EncodeRecurrenceDateTimes(VEventPropertyNames.ExceptionDateTimes, Event.ExceptionDateTimes);
			EncodeExtensions(Event.Extensions);
		}
		protected virtual void EncodeRecurrenceRule(string name, VRecurrenceRuleCollection rules) {
			VRecurrenceConvert conv = new VRecurrenceConvert();
			int count = rules.Count;
			for (int i = 0; i < count; i++)
				WriteProperty(name, conv.ConvertToString(rules[i]));
		}
		protected virtual void EncodeRecurrenceDateTimes(string name, DateTimeCollection dateTimes) {
			if (dateTimes.Count == 0)
				return;
			StringBuilder sb = new StringBuilder();
			int count = dateTimes.Count;
			for (int i = 0; i < count; i++) {
				if (sb.Length > 0)
					sb.Append(';');
				sb.Append(VCalendarUtils.ToVCalendarDateTimeExact(dateTimes[i], true));
			}
			WriteProperty(name, sb.ToString());
		}
		protected virtual void EncodeExtensions(VEventExtensionCollection extensions) {
			int count = extensions.Count;
			for (int i = 0; i < count; i++) {
				VEventExtension ext = extensions[i];
				string name = VCalendarUtils.FormatExtensionName(ext.Name);
				if (String.IsNullOrEmpty(name))
					continue;
				if (ext.Encoding == VCalendarEncoding.QuotedPrintable)
					EncodeQuotedPrintableProperty(name, ext.Value);
				else
					WriteProperty(name, ext.Parameters.ToArray(), ext.Value);
			}
		}
		protected void EncodeQuotedPrintableProperty(string name, string val) {
			if (val == null || val.Length == 0)
				return;
			string propHeader = CreateQuotedPrintablePropertyPrefix(name);
			string propValue = VCalendarUtils.ToQuotedPrintable(val, propHeader.Length, QuotedPrintableConverter.QuotedPrintableLineWidth);
			WriteStringCore(propHeader + propValue);
		}
	}
	public abstract class VObjectDecoder {
		object obj;
		protected VObjectDecoder() {
			obj = CreateVObject();
		}
		public object Object { get { return obj; } }
		public abstract string ObjectTag { get; }
		#region Events
		EventHandler onDecodeComplete;
		DecodeEventHandler onBeginSubItem;
		public event EventHandler DecodeComplete {
			add { onDecodeComplete += value; }
			remove { onDecodeComplete += value; }
		}
		public event DecodeEventHandler BeginSubItem {
			add { onBeginSubItem += value; }
			remove { onBeginSubItem += value; }
		}
		protected void OnDecodeComplete() {
			if (onDecodeComplete != null) onDecodeComplete(this, EventArgs.Empty);
		}
		protected void OnBeginSubItem(DecodeEventArgs e) {
			if (onBeginSubItem != null) onBeginSubItem(this, e);
		}
		#endregion
		public abstract object CreateVObject();
		protected abstract string[] GetValidPropertyNames();
		protected abstract string[] GetValidSubItemNames();
		public static VObjectDecoder GetDecoder(string objectTag) {
			switch (objectTag) {
				case VCalendarTag.Calendar:
					return new VCalendarDecoder();
				case VCalendarTag.Event:
					return new VEventDecoder();
				default:
					return null; 
			}
		}
		public virtual void Decode(TextReader textReader) {
			string line = null;
			VCalendarPropertyInfo lastMultilinePropInfo = null;
			bool multilineMode = false;
			while (true) {
				line = textReader.ReadLine();
				if (line == null)
					break;
				if (multilineMode) {
					if (!IsKeyword(line)) {
						lastMultilinePropInfo.Value += "\r\n" + line;
						continue;
					}
					else {
						AssignProperty(lastMultilinePropInfo);
						multilineMode = false;
						lastMultilinePropInfo = null;
					}
				}
				if (IsSubItem(line)) {
					OnBeginSubItem(new DecodeEventArgs(line));
					break;
				}
				if (IsEndOfObject(line)) {
					OnDecodeComplete();
					break;
				}
				if (IsProperty(line)) {
					VCalendarPropertyInfo info = ParseProperty(line);
					if (!info.MultiLine) {
						AssignProperty(info);
					}
					else {
						lastMultilinePropInfo = info;
						multilineMode = true;
					}
				}
			}
		}
		protected virtual bool IsEndOfObject(string token) {
			return String.Compare(VCalendarTag.End + ObjectTag, token) == 0;
		}
		bool IsProperty(string token) {
			return IsValidElement(token, GetValidPropertyNames());
		}
		bool IsKeyword(string token) {
			return token.StartsWith(VCalendarTag.Begin) ||
				token.StartsWith(VCalendarTag.End) || IsProperty(token);
		}
		bool IsSubItem(string token) {
			string s = VCalendarUtils.RemoveBeginTag(token);
			return IsValidElement(s, GetValidSubItemNames());
		}
		bool IsValidElement(string token, string[] names) {
			foreach (string name in names) {
				if (token.StartsWith(name))
					return true;
			}
			return false;
		}
		protected virtual void AssignProperty(VCalendarPropertyInfo info) {
		}
		protected VCalendarPropertyInfo ParseProperty(string token) {
			string[] parts = token.Split(':');
			string propHeader = parts[0];
			VCalendarPropertyInfo info = new VCalendarPropertyInfo();
			info.Value = token.Remove(0, propHeader.Length + 1);
			parts = propHeader.Split(';');
			info.Name = parts[0];
			if (parts.Length > 1) {
				VCalendarParameter[] parameters = ParseParameters(propHeader.Remove(0, info.Name.Length + 1));
				info.Parameters.AddRange(parameters);
			}
			info.MultiLine = IsQuotedPrintableMultiline(info.Value);
			return info;
		}
		bool IsQuotedPrintableMultiline(string val) {
			return val.EndsWith(QuotedPrintableConverter.QuotedPrintableCRLF) || val.EndsWith(QuotedPrintableConverter.QuotedPrintableSoftLineBreak.ToString());
		}
		protected VCalendarParameter[] ParseParameters(string token) {
			List<VCalendarParameter> info = new List<VCalendarParameter>();
			string[] parameters = token.Split(';');
			for (int i = 0; i < parameters.Length; i++) {
				string[] parts = parameters[i].Split('=');
				if (parts.Length == 2) {
					info.Add(new VCalendarParameter(parts[0], parts[1]));
				}
			}
			return info.ToArray();
		}
		protected string DecodeStringProperty(VCalendarPropertyInfo info) {
			VCalendarParameter encodingParam = info.Parameters[VPropertyParameterNames.Encoding];
			if (encodingParam != null) {
				switch (encodingParam.Value) {
					case VEncodingParameterValues.QuotedPrintable: {
							return VCalendarUtils.FromQuotedPrintable(info.Value);
						}
					case VEncodingParameterValues.Base64: {
							return VCalendarUtils.FromBase64(info.Value);
						}
					default:
						break;
				}
			}
			return info.Value;
		}
		protected bool HasQuotedPrintableParameter(VCalendarPropertyInfo info) {
			if (info == null) return false;
			VCalendarParameter encodingParam = info.Parameters[VPropertyParameterNames.Encoding];
			return (encodingParam != null) ? encodingParam.Value == VEncodingParameterValues.QuotedPrintable : false;
		}
	}
	public class VCalendarDecoder : VObjectDecoder {
		public VCalendarDecoder()
			: base() {
		}
		public VCalendarObject Calendar { get { return (VCalendarObject)Object; } }
		public override string ObjectTag { get { return VCalendarTag.Calendar; } }
		protected override string[] GetValidPropertyNames() {
			return new string[] {	VCalendarPropertyNames.DaylightSavingsRule, 
									VCalendarPropertyNames.GeographicPosition, 
									VCalendarPropertyNames.ProductIdentifier, 
									VCalendarPropertyNames.TimeZone, 
									VCalendarPropertyNames.Version  };
		}
		protected override string[] GetValidSubItemNames() {
			return new string[] { VCalendarTag.Event };
		}
		public override object CreateVObject() {
			return new VCalendarObject();
		}
		protected override void AssignProperty(VCalendarPropertyInfo info) {
			switch (info.Name) {
				case VCalendarPropertyNames.Version:
					if (info.Value != VCalendarConsts.DefaultCalendarVersion)
						throw new Exception("Can't import this version of vCalendar");
					Calendar.Version = info.Value;
					break;
				case VCalendarPropertyNames.ProductIdentifier:
					Calendar.ProductId = info.Value;
					break;
			}
		}
	}
	public class VEventDecoder : VObjectDecoder {
		public VEventDecoder() {
		}
		public VEvent Event { get { return (VEvent)Object; } }
		public override string ObjectTag { get { return VCalendarTag.Event; } }
		protected override string[] GetValidPropertyNames() {
			return new string[] {	VEventPropertyNames.ExtensionPrefix,
									VEventPropertyNames.DtStart, VEventPropertyNames.DtEnd, 
									VEventPropertyNames.Summary, VEventPropertyNames.Location, 
									VEventPropertyNames.Description, VEventPropertyNames.TimeTransparency,
									VEventPropertyNames.Priority, 
									VEventPropertyNames.RecurrenceRule, 
									VEventPropertyNames.ExceptionRule, VEventPropertyNames.ExceptionDateTimes, 
								};
		}
		protected override string[] GetValidSubItemNames() {
			return new string[0];
		}
		public override object CreateVObject() {
			return new VEvent();
		}
		protected override void AssignProperty(VCalendarPropertyInfo info) {
			switch (info.Name) {
				case VEventPropertyNames.DtStart:
					Event.DTStart = VCalendarUtils.FromVCalendarDateTime(info.Value);
					break;
				case VEventPropertyNames.DtEnd:
					Event.DTEnd = VCalendarUtils.FromVCalendarDateTime(info.Value);
					break;
				case VEventPropertyNames.TimeTransparency:
					Event.TimeTransparency = Convert.ToInt32(info.Value);
					break;
				case VEventPropertyNames.Summary:
					Event.Summary = DecodeStringProperty(info);
					break;
				case VEventPropertyNames.Location:
					Event.Location = DecodeStringProperty(info);
					break;
				case VEventPropertyNames.Description:
					Event.Description = DecodeStringProperty(info);
					break;
				case VEventPropertyNames.RecurrenceRule:
					DecodeRecurrenceRule(info.Value, Event.RecurrenceRules);
					break;
				case VEventPropertyNames.ExceptionRule:
					DecodeRecurrenceRule(info.Value, Event.ExceptionRules);
					break;
				case VEventPropertyNames.ExceptionDateTimes:
					DecodeDateTimeList(info.Value, Event.ExceptionDateTimes);
					break;
			}
			if (info.Name.StartsWith(VEventPropertyNames.ExtensionPrefix)) {
				DecodeExtension(info, Event.Extensions);
			}
		}
		protected void DecodeRecurrenceRule(string text, VRecurrenceRuleCollection dest) {
			VRecurrenceConvert conv = new VRecurrenceConvert();
			VRecurrenceRule rule = conv.ConvertFromString(Event.DTStart, text);
			if (rule != null)
				dest.Add(rule);
		}
		protected void DecodeDateTimeList(string text, DateTimeCollection dest) {
			string[] parts = text.Split(';');
			int count = parts.Length;
			for (int i = 0; i < count; i++) {
				DateTime dateTime = VCalendarUtils.FromVCalendarDateTimeExact(parts[i]);
				if (dateTime != DateTime.MinValue)
					dest.Add(dateTime);
			}
		}
		protected void DecodeExtension(VCalendarPropertyInfo info, VEventExtensionCollection extensions) {
			string name = VCalendarUtils.ExtractExtensionName(info.Name);
			if (extensions[info.Name] != null)
				return;
			VEventExtension ext = new VEventExtension(name, DecodeStringProperty(info));
			if (HasQuotedPrintableParameter(info))
				ext.Encoding = VCalendarEncoding.QuotedPrintable;
			VCalendarParameterCollection infoParameters = info.Parameters;
			for (int i = 0; i < infoParameters.Count; i++) {
				VCalendarParameter item = infoParameters[i];
				ext.Parameters.Add(new VCalendarParameter(item.Name.Trim(), item.Value));
			}
			extensions.Add(ext);
		}
	}
	public class VCalendarParameter {
		string name;
		string val;
		public VCalendarParameter() {
		}
		public VCalendarParameter(string name, string val) {
			this.name = name;
			this.val = val;
		}
		public string Name { get { return name; } set { name = value; } }
		public string Value { get { return val; } set { val = value; } }
		public override string ToString() {
			return String.Format("{0}={1}", Name, Value);
		}
	}
	#region VCalendarParameterCollection
	public class VCalendarParameterCollection : DXNamedItemCollection<VCalendarParameter> {
		public VCalendarParameterCollection()
			: base(DXCollectionUniquenessProviderType.MaximizePerformance) {
		}
		protected override string GetItemName(VCalendarParameter item) {
			return item.Name;
		}
	}
	#endregion
	public class VCalendarPropertyInfo : VCalendarParameter {
		readonly VCalendarParameterCollection parameters;
		bool multiLine;
		public VCalendarPropertyInfo() {
			parameters = new VCalendarParameterCollection();
		}
		public VCalendarParameterCollection Parameters { get { return parameters; } }
		public bool MultiLine { get { return multiLine; } set { multiLine = value; } }
	}
	public delegate void DecodeEventHandler(object sender, DecodeEventArgs e);
	public class DecodeEventArgs {
		string token;
		public DecodeEventArgs(string token) {
			this.token = token;
		}
		public string Token { get { return token; } }
	}
	public static class VCalendarUtils {
		static QuotedPrintableConverter qpConverter = new QuotedPrintableConverter();
		static Encoding encodingBase64 = Encoding.GetEncoding("iso-8859-1");
		static QuotedPrintableConverter QPConverter { get { return qpConverter; } }
		static Encoding EncodingBase64 { get { return encodingBase64; } }
		public static string RemoveBeginTag(string token) {
			if (token.StartsWith(VCalendarTag.Begin))
				return token.Remove(0, VCalendarTag.Begin.Length);
			return token;
		}
		public static string[] SplitByCRLF(string token) {
			return Regex.Split(token, "\r\n");
		}
		public static string ExtractExtensionName(string name) {
			if (name == null || name.Length == 0)
				return String.Empty;
			if (name.StartsWith(VEventPropertyNames.ExtensionPrefix))
				return name.Remove(0, VEventPropertyNames.ExtensionPrefix.Length);
			return name;
		}
		public static string FormatExtensionName(string name) {
			if (name == null || name.Length == 0)
				return String.Empty;
			return VEventPropertyNames.ExtensionPrefix + name;
		}
		public static string ToVCalendarDateTime(DateTime val) {
			val = val.ToUniversalTime();
			return ToVCalendarDateTimeExact(val, true);
		}
		internal static string ToVCalendarDateTimeExact(DateTime val, bool utcFormat) {
			return VCalendarConvert.FromDateTime(val, utcFormat);
		}
		internal static DateTime FromVCalendarDateTimeExact(string val) {
			if (val.EndsWith("Z")) val = val.Replace("Z", String.Empty);
			return FromVCalendarDateTime(val);
		}
		public static DateTime FromVCalendarDateTime(string val) {
			if (string.IsNullOrEmpty(val))
				return DateTime.MinValue;
			try {
				bool utc = VCalendarConvert.IsUtcDateTime(val);
				DateTime result = DateTime.ParseExact(val, VCalendarConvert.GetDateTimeFormat(utc), CultureInfo.InvariantCulture);
				return new DateTime(result.Ticks);
			}
			catch {
			}
			return DateTime.MinValue;
		}
		public static string ToQuotedPrintable(string text, int startIndex, int maxLineWidth) {
			text = QPConverter.Encode(text, startIndex, maxLineWidth);
			return text;
		}
		public static string FromQuotedPrintable(string text) {
			return QPConverter.Decode(text);
		}
		public static string ToBase64(string text) {
			if (text == null || text.Length == 0)
				return text;
			byte[] binaryData = EncodingBase64.GetBytes(text);
			return Convert.ToBase64String(binaryData);
		}
		public static string FromBase64(string text) {
			if (text == null || text.Length == 0)
				return text;
			Encoding encoding = Encoding.GetEncoding("iso-8859-1");
			return (encoding != null) ? encoding.GetString(Convert.FromBase64String(text)) : String.Empty;
		}
		public static string DecodeFromUtf8(string val) {
			return string.Empty;
		}
		public static string FromCharset(string val) {
			return string.Empty;
		}
	}
}
