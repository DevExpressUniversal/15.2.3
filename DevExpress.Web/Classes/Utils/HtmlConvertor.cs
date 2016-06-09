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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;
#if ASP
using System.Web.UI.WebControls;
#else
using DevExpress.vNext;
#endif
namespace DevExpress.Web.Internal {
	public class JSONScriptValue {
		public JSONScriptValue() { }
		public JSONScriptValue(string script) {
			JSScript = script;
		}
		public string JSScript { get; set; }
	}
	public interface IJSONCustomObject {
		int PropertiesCount { get; }
		string GetPropertyName(int index);
		object GetPropertyValue(int index);
	}
	public class JSONOptions {
		public bool UseDoubleQuotesMark { get; set; }
		public JSONDateFormat DateFormat { get; set; }
		public JSONNullValueFormat NullValueFormat { get; set; }
		public bool AddRoundBrackets { get; set; }
		public bool ProcessStructs { get; set; }
		public JSONOptions(bool useExtendedDateTimeFormat = true, bool nullsAsUndefined = false, bool clearRoundBrackets = false) {
			UseDoubleQuotesMark = false;
			DateFormat = useExtendedDateTimeFormat ? JSONDateFormat.JSDateExtended : JSONDateFormat.JSDateShort;
			NullValueFormat = nullsAsUndefined ? JSONNullValueFormat.Undefined : JSONNullValueFormat.Null;
			AddRoundBrackets = !clearRoundBrackets;
			ProcessStructs = false;
		}
	}
	public enum JSONDateFormat {
		JSDateShort,
		JSDateExtended,
		JSONIsoDateFormat
	}
	public enum JSONNullValueFormat {
		Null,
		Undefined
	}
	public class HtmlConvertor {
		protected const string ExtendedDateTimeFormat = "MM/dd/yyyy HH:mm:ss fff";
		private static readonly Dictionary<string, string> HtmlDecodeReplacementTable = new Dictionary<string, string>();
		static HtmlConvertor() {
			HtmlDecodeReplacementTable.Add("&gt;", ">");
			HtmlDecodeReplacementTable.Add("&gtx;", "&gt;");
			HtmlDecodeReplacementTable.Add("&lt;", "<");
			HtmlDecodeReplacementTable.Add("&ltx;", "&lt;");
			HtmlDecodeReplacementTable.Add("&quot;", "\"");
			HtmlDecodeReplacementTable.Add("&quotx;", "&quot;");
			HtmlDecodeReplacementTable.Add("&amp;", "&");
			HtmlDecodeReplacementTable.Add("&ampx;", "&amp;");
		}
		public static string DecodeHtml(string html) {
			if(string.IsNullOrEmpty(html))
				return html;
			string encoded = html;
			foreach(KeyValuePair<string, string> pair in HtmlDecodeReplacementTable)
				encoded = encoded.Replace(pair.Key, pair.Value);
			return encoded;
		}
		public static string EncodeUnit(Unit unit) {
			return unit.ToString(CultureInfo.InvariantCulture);
		}
		public static string ToHtml(object value) {
			if(value is System.Drawing.Color)
				return System.Drawing.ColorTranslator.ToHtml((System.Drawing.Color)value);
			else if(value is Unit)
				return EncodeUnit((Unit)value);
			else
				return value.ToString();
		}
		public static string ToMultilineHtml(string value) {
			if(value == null && string.IsNullOrEmpty(value))
				return value;
			else
				return value.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
		}
		public static string ToJSON(object value) {
			return ToJSON(value, true, false);
		}
		public static string ToJSON(object value, bool nullsAsUndefined) {
			return ToJSON(value, true, nullsAsUndefined);
		}
		public static string ToJSON(object value, bool useExtendedDateTimeFormat, bool nullsAsUndefined) {
			return ToJSON(value, useExtendedDateTimeFormat, nullsAsUndefined, false);
		}
		public static string ToJSON(object value, bool useExtendedDateTimeFormat, bool nullsAsUndefined, bool clearRoundBrackets) {
			return ToJSON(value, new JSONOptions(useExtendedDateTimeFormat, nullsAsUndefined, clearRoundBrackets));
		}
		public static string ToJSON(object value, JSONOptions opts) {
			StringBuilder builder = new StringBuilder();
			ToJSON(builder, value, opts);
			return builder.ToString();
		}
		public static void ToJSON(StringBuilder builder, object value, bool useExtendedDateTimeFormat, bool nullsAsUndefined, bool clearRoundBrackets) {
			ToJSON(builder, value, new JSONOptions(useExtendedDateTimeFormat, nullsAsUndefined, clearRoundBrackets));
		}
		public static void ToJSON(StringBuilder builder, object value, JSONOptions opts) {
			if(value == null || value is DBNull || value is ValueType || value is string)
				builder.Append(HtmlConvertor.ToScript(value, null, opts));
			else if(value is IDictionary) {
				bool needRoundBracket = (builder.Length < 1 && opts.AddRoundBrackets);
				if(needRoundBracket) builder.Append("(");
				builder.Append("{");
				bool first = true;
				foreach(DictionaryEntry entry in (IDictionary)value) {
					if(!first) builder.Append(",");
					builder.Append(ToScript(entry.Key.ToString(), opts));
					builder.AppendFormat(":");
					ToJSON(builder, entry.Value, opts);
					first = false;
				}
				builder.Append("}");
				if(needRoundBracket) builder.Append(")");
			}
			else if(value is IEnumerable) {
				builder.Append("[");
				bool first = true;
				foreach(object item in (IEnumerable)value) {
					if(!first) builder.Append(",");
					if(item != null || opts.NullValueFormat == JSONNullValueFormat.Null)
						ToJSON(builder, item, opts);
					first = false;
				}
				builder.Append("]");
			}
			else if(value is JSONScriptValue) {
				builder.Append(((JSONScriptValue)value).JSScript);
			}
			else if(value is IJSONCustomObject)  {
				IJSONCustomObject customObject = value as IJSONCustomObject;
				builder.Append("{");
				for (int i = 0; i < customObject.PropertiesCount; i++) {
					if (i > 0) builder.Append(',');
					builder.Append(ToScript(customObject.GetPropertyName(i), opts));
					builder.Append(':');
					ToJSON(builder, customObject.GetPropertyValue(i), opts);
				}
				builder.Append("}");
			} else {
				AppendObject(builder, value, opts);
			}
		}
		public static object FromJSON(string jsonString) {
			JsonReader reader = new JsonReader();
			return reader.Read(jsonString);
		}
		public static T FromJSON<T>(string jsonString) {
			JsonReader reader = new JsonReader();
			return reader.Read<T>(jsonString);
		}
		public static string ToScript(object value) {
			return ToScript(value, (Type)null);
		}
		public static string ToScript(object value, Type type, JSONOptions opts) {
			StringBuilder sb = new StringBuilder();
			ToScript(sb, value, type, opts);
			return sb.Length == 0 ? null : sb.ToString();
		}
		public static string EscapeString(string input) {
			StringBuilder output = new StringBuilder(input.Length);
			EscapeChars(input, output);
			return output.ToString();
		}
		public static string ToScript(object value, Type type) {
			return ToScript(value, type, new JSONOptions {
				DateFormat = JSONDateFormat.JSDateShort
			});
		}
		static string ToScript(object value, JSONOptions opts) {
			return ToScript(value, null, opts);
		}
		static void ToScript(StringBuilder sb, object value, Type type, JSONOptions opts) {
			if(value == null || value == DBNull.Value) {
				sb.Append("null");
				return;
			}
			if(type == null)
				type = value.GetType();
			if(type == typeof(string) || type == typeof(char) || type == typeof(Guid) || typeof(Enum).IsAssignableFrom(type)) {
				AppendStringLiteral(sb, value, opts.UseDoubleQuotesMark);
			} else if(type == typeof(DateTime)) {
				AppendDate(sb, value, opts.DateFormat);
			} else if(type == typeof(bool)) {
				sb.Append(((bool)value) ? "true" : "false");
			} else if(type == typeof(double)) {
				sb.Append(((double)value).ToString("R", CultureInfo.InvariantCulture));
			} else if(type == typeof(Decimal)) {
				sb.Append(((Decimal)value).ToString(CultureInfo.InvariantCulture));
			} else if(type == typeof(Single)) {
				sb.Append(((Single)value).ToString(CultureInfo.InvariantCulture));
			} else if(type == typeof(Regex)) {
				AppendRegex(sb, value);
			} else if(type.IsPrimitive) {
				sb.Append(value.ToString());
			} else {
				if(opts.ProcessStructs)
					AppendObject(sb, value, opts);
				else
					ToScript(sb, value.ToString(), typeof(string), opts);
			}
		}
		static void AppendObject(StringBuilder builder, object value, JSONOptions opts) {
			PropertyInfo[] properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			if(properties.Length > 0) {
				builder.Append("{");
				bool first = true;
				foreach(PropertyInfo property in properties) {
					if(property.GetIndexParameters().Length > 0 || !property.CanRead || !IsPropertyBrowsable(property))
						continue;
					if(!first)
						builder.Append(",");
					builder.Append(ToScript(property.Name, opts));
					builder.Append(':');
					object propertyValue = property.GetValue(value, new object[0] { });
					if(Object.ReferenceEquals(propertyValue, value))
						propertyValue = null;
					ToJSON(builder, propertyValue, opts);
					first = false;
				}
				builder.Append("}");
			} else {
				throw new ArgumentException(string.Format("Cannot convert {0} type to script", value.GetType()));
			}
		}
		static bool IsPropertyBrowsable(PropertyInfo property) {
			object[] attribures = property.GetCustomAttributes(typeof(BrowsableAttribute), true);
			if(attribures == null || attribures.Length == 0)
				return true;
			return ((BrowsableAttribute)attribures[0]).Browsable;
		}
		static void AppendStringLiteral(StringBuilder sb, object value, bool useDoubleQuotesMark) {
			string input = value.ToString();
			char quotesMark = GetQuotesMarkChar(useDoubleQuotesMark);
			sb.Append(quotesMark);
			EscapeChars(input, sb, GetQuotesMarkChar(!useDoubleQuotesMark));
			EscapeScriptTag(sb);
			sb.Append(quotesMark);
		}
		static void AppendDate(StringBuilder sb, object value, JSONDateFormat format) {
			DateTime date = (DateTime)value;
			if(format == JSONDateFormat.JSONIsoDateFormat) {
				sb.AppendFormat("\"{0}\"", date.ToString("o"));
			} else {
				sb.Append("new Date(");
				sb.Append(date.Year);
				sb.Append(',');
				sb.Append((date.Month - 1));
				sb.Append(',');
				sb.Append(date.Day);
				bool extended = format == JSONDateFormat.JSDateExtended;
				if(date.Hour > 0 || date.Minute > 0 || date.Second > 0 || date.Millisecond > 0 || extended) {
					sb.Append(',');
					sb.Append(date.Hour);
				}
				if(date.Minute > 0 || date.Second > 0 || date.Millisecond > 0 || extended) {
					sb.Append(',');
					sb.Append(date.Minute);
				}
				if(date.Second > 0 || date.Millisecond > 0 || extended) {
					sb.Append(',');
					sb.Append(date.Second);
				}
				if(date.Millisecond > 0 || extended) {
					sb.Append(',');
					sb.Append(date.Millisecond);
				}
				sb.Append(')');
			}
		}
		static void AppendRegex(StringBuilder sb, object value) {
			Regex regex = (Regex)value;
			string pattern = regex.ToString();
			if(pattern.Length == 0)
				return;
			sb.Append('/');
			sb.Append(regex.ToString());
			sb.Append('/');
			if((regex.Options & RegexOptions.IgnoreCase) > 0)
				sb.Append('i');
			if((regex.Options & RegexOptions.Multiline) > 0)
				sb.Append('m');
		}
		static Regex ScriptEndTagRegex = new Regex("</script[\\s]*>", RegexOptions.IgnoreCase);
		protected static void EscapeScriptTag(StringBuilder output) {
			string outputString = output.ToString();
			output.Remove(0, outputString.Length);
			output.Append(ScriptEndTagRegex.Replace(outputString, "<\\/script>"));
		}
		protected static void EscapeChars(string input, StringBuilder output) {
			EscapeChars(input, output, GetQuotesMarkChar(true));
		}
		static char GetQuotesMarkChar(bool doubleQuotes) {
			return doubleQuotes ? '\"' : '\'';
		}
		protected static void EscapeChars(string input, StringBuilder output, char ignoreChar) {
			for(int i = 0; i < input.Length; i++) {
				char ch = input[i];
				if(ch == ignoreChar) {
					output.Append(ch);
					continue;
				}
				if(ch == '\n')
					output.Append("\\n");
				else if(ch == '\r')
					output.Append("\\r");
				else if(ch == '\t')
					output.Append("\\t");
				else if(ch == '\u2028')
					output.Append("\\u2028");
				else if(ch == '\u2029')
					output.Append("\\u2029");
				else if((int)ch < 32)
					output.AppendFormat(CultureInfo.InvariantCulture, @"\x{0:x2}", (int)ch);
				else {
					switch(ch) {
						case '\\':
						case '\"':
						case '\'':
							output.Append('\\');
							break;
						case '-':
							if(i > 0 && input[i - 1] == '-')
								output.Append('\\');
							break;
						default:
							break;
					}
					output.Append(ch);
				}
			}
		}
	}
	public class JsonReader {
		int pos;
		string jsonString;
		protected int Pos {
			get { return pos; }
			set { pos = value; }
		}
		protected string JsonString { get { return jsonString; } }
		protected char CharAtPos { get { return JsonString[Pos]; } }
		protected bool IsEnd { get { return Pos >= JsonString.Length; } }
		public object Read(string jsonString) {
			this.jsonString = jsonString;
			this.pos = 0;
			SkipWhiteSpaces();
			return ReadCore();
		}
		public T Read<T>(string jsonString) {
			return (T)Read(jsonString);
		}
		object ReadCore() {
			if(IsObjectAtPos())
				return ReadObject();
			if(IsArrayAtPos())
				return ReadArray();
			if(IsBoolean())
				return ReadBoolean();
			if(IsNullAtPos())
				return ReadNull();
			if(IsNumberAtPos())
				return ReadNumber();
			if(IsStringAtPos())
				return ReadString(false);
			if(IsDateAtPos())
				return ReadDate();
			if(IsRegexAtPos())
				return ReadRegex();
			throw CreateUnexpectedException();
		}
		bool IsNullAtPos() {
			return GetSubstringAtPos(4) == "null";
		}
		bool IsBoolean() {
			return GetSubstringAtPos(4) == "true" || GetSubstringAtPos(5) == "false";
		}
		bool IsNumberAtPos() {
			if(CharAtPos == '-')
				return true;
			if(CharAtPos >= '0' && CharAtPos <= '9')
				return true;
			return false;
		}
		bool IsStringAtPos() {
			return CharAtPos == '"' || CharAtPos == '\'';
		}
		bool IsRegexAtPos() {
			return CharAtPos == '/';
		}
		bool IsDateAtPos() {
			return GetSubstringAtPos(8) == "new Date";
		}
		bool IsArrayAtPos() {
			return CharAtPos == '[';
		}
		bool IsObjectAtPos() {
			return CharAtPos == '{';
		}
		void SkipWhiteSpaces() {
			while(!IsEnd && Char.IsWhiteSpace(CharAtPos)) Pos++;
		}
		string GetSubstringAtPos(int length) {
			return GetSubstringAtPos(length, Pos);
		}
		string GetSubstringAtPos(int length, int startPos) {
			int maxLength = JsonString.Length - startPos;
			if(length > maxLength)
				length = maxLength;
			return JsonString.Substring(startPos, length);
		}
		Exception CreateUnexpectedException() {
			return new FormatException(string.Format("Unexpected '{0}' at {1}", CharAtPos, Pos));
		}
		Hashtable ReadObject() {
			Hashtable result = new Hashtable();
			while(true) {
				Pos++;
				SkipWhiteSpaces();
				if(CharAtPos == '}') break;
				string key = ReadString(false);
				SkipWhiteSpaces();
				if(CharAtPos != ':')
					throw CreateUnexpectedException();
				Pos++;
				SkipWhiteSpaces();
				result.Add(key, ReadCore());
				SkipWhiteSpaces();
				if(CharAtPos == ',') continue;
				if(CharAtPos == '}') break;
				throw CreateUnexpectedException();
			}
			Pos++;
			return result;
		}
		ArrayList ReadArray() {
			ArrayList result = new ArrayList();
			while(true) {
				Pos++;
				SkipWhiteSpaces();
				if(CharAtPos == ']') break;
				result.Add(ReadCore());
				SkipWhiteSpaces();
				if(CharAtPos == ',') continue;
				if(CharAtPos == ']') break;
				throw CreateUnexpectedException();
			}
			Pos++;
			return result;
		}
		bool ReadBoolean() {
			bool value = CharAtPos == 't';
			Pos += value ? 4 : 5;
			return value;
		}
		object ReadNull() {
			Pos += 4;
			return null;
		}
		object ReadNumber() {
			StringBuilder builder = new StringBuilder();
			while(!IsEnd && CanContinueReadNumber(CharAtPos)) {
				builder.Append(CharAtPos);
				Pos++;
			}
			string value = builder.ToString();
			int intValue;
			if(Int32.TryParse(value, out intValue))
				return intValue;
			return Double.Parse(value, CultureInfo.InvariantCulture);
		}
		bool CanContinueReadNumber(char ch) {
			return !Char.IsWhiteSpace(ch) && ch != ',' && ch != ']' && ch != '}' && ch != ')';
		}
		string ReadString(bool isRegex) {
			StringBuilder builder = new StringBuilder();
			bool isEscaping = false;
			bool complete = false;
			char quote = CharAtPos;
			Pos++;
			while(!complete) {
				if(isEscaping) {
					AppendEscapedChar(builder, isRegex);
					isEscaping = false;
				}
				else {
					if(CharAtPos == '\\')
						isEscaping = true;
					else if(CharAtPos == quote)
						complete = true;
					else
						builder.Append(CharAtPos);
					Pos++;
				}
			}
			return builder.ToString();
		}
		void AppendEscapedChar(StringBuilder builder, bool isRegex) {
			if(CharAtPos == 'u') {
				builder.Append((char)int.Parse(JsonString.Substring(Pos + 1, 4), NumberStyles.HexNumber));
				Pos += 5;
			}
			else {
				builder.Append(DecodeEscapedChar(CharAtPos, isRegex));
				Pos++;
			}
		}
		string DecodeEscapedChar(char ch, bool isRegex) {
			switch(ch) {
				case '"':
				case '\'':
				case '\\':
				case '/':
					return ch.ToString();
				case 'b':
					return "\b";
				case 'f':
					return "\f";
				case 'n':
					return "\n";
				case 'r':
					return "\r";
				case 't':
					return "\t";
			}
			if(isRegex)
				return @"\" + ch;
			return ch.ToString();
		}
		Regex ReadRegex() {
			string pattern = ReadString(true);
			List<char> flags = new List<char>();
			List<char> availableFlags = new List<char>();
			availableFlags.Add('i');
			availableFlags.Add('g');
			availableFlags.Add('m');
			while(availableFlags.Contains(CharAtPos)) {
				if(!flags.Contains(CharAtPos))
					flags.Add(CharAtPos);
				Pos++;
			}
			RegexOptions options = new RegexOptions();
			if(flags.Contains('i'))
				options |= RegexOptions.IgnoreCase;
			if(flags.Contains('m'))
				options |= RegexOptions.Multiline;
			return new Regex(pattern, options);
		}
		DateTime ReadDate() { 
			Pos += 8;
			SkipWhiteSpaces();
			Pos++; 
			SkipWhiteSpaces();
			int startPos = Pos;
			while(CharAtPos != ')') Pos++;
			string datePartsStr = GetSubstringAtPos(Pos - startPos, startPos);
			Pos++; 
			string[] dateParts = datePartsStr.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			int year = int.Parse(dateParts[0]);
			int month = int.Parse(dateParts[1]) + 1;
			int day = int.Parse(dateParts[2]);
			int hour = dateParts.Length > 3
				? int.Parse(dateParts[3])
				: 0;
			int minute = dateParts.Length > 4
				? int.Parse(dateParts[4])
				: 0;
			int second = dateParts.Length > 5
				? int.Parse(dateParts[5])
				: 0;
			int millisecond = dateParts.Length > 6
				? int.Parse(dateParts[6])
				: 0;
			return new DateTime(year, month, day, hour, minute, second, millisecond);
		}
	}
}
