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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraExport;
namespace DevExpress.Export.Xl {
	#region XlExportNetFormatParser
	public class XlExportNetFormatParser {
		public XlExportNetFormatParser(string format) {
			Prefix = string.Empty;
			Postfix = string.Empty;
			FormatString = string.Empty;
			if(string.IsNullOrEmpty(format))
				OriginalFormat = string.Empty;
			else {
				OriginalFormat = format;
				Parse(format);
			}
		}
		public string Prefix { get; private set; }
		public string Postfix { get; private set; }
		public string FormatString { get; private set; }
		public string OriginalFormat { get; private set; }
		void Parse(string format) {
			string prefix = string.Empty;
			string postfix = string.Empty;
			int leadingBraceIndex = GetBraceIndex(format, '{');
			int trailingBraceIndex = GetMatchingBraceIndex(format, '}', leadingBraceIndex);
			if(leadingBraceIndex != -1) {
				if(trailingBraceIndex < leadingBraceIndex)
					return;
				prefix = CleanupPrefix(format.Substring(0, leadingBraceIndex));
				postfix = CleanupPostfix(format.Substring(trailingBraceIndex + 1));
				format = format.Substring(leadingBraceIndex + 1, trailingBraceIndex - leadingBraceIndex - 1);
				int colonIndex = format.IndexOf(":");
				if(colonIndex == -1)
					format = string.Empty;
				else
					format = format.Remove(0, colonIndex + 1);
			}
			else if(trailingBraceIndex != -1)
				return;
			Prefix = prefix;
			Postfix = postfix;
			FormatString = format;
		}
		int GetBraceIndex(string formatString, char brace) {
			int count = formatString.Length;
			int result = -1;
			bool hasBrace = false;
			for(int i = 0; i < count; i++) {
				if(formatString[i] == brace) {
					if(hasBrace) {
						hasBrace = false;
						result = -1;
					}
					else {
						hasBrace = true;
						result = i;
					}
				}
				else if(hasBrace)
					break;
			}
			return result;
		}
		int GetMatchingBraceIndex(string formatString, char brace, int startIndex) {
			if(startIndex == -1)
				return GetBraceIndex(formatString, brace);
			return formatString.IndexOf(brace, startIndex);
		}
		string CleanupPrefix(string text) {
			return text.Replace("{{", "{").Replace("}}", "}");
		}
		string CleanupPostfix(string text) {
			int braceIndex = GetBraceIndex(text, '{');
			if(braceIndex != -1)
				text = text.Substring(0, braceIndex).TrimEnd(new char[] { ' ' });
			return text.Replace("{{", "{").Replace("}}", "}");
		}
	}
	#endregion
	#region XlExportNetFormatComposer
	public static class XlExportNetFormatComposer {
		static readonly string leadingBrace = "{";
		static readonly string trailingBrace = "}";
		static readonly string escapedLeadingBrace = "{{";
		static readonly string escapedTrailingBrace = "}}";
		public static string CreateFormat(string formatString) {
			return CreateFormat(string.Empty, formatString, string.Empty);
		}
		public static string CreateFormat(string prefix, string formatString, string postfix) {
			StringBuilder sb = new StringBuilder();
			if(!string.IsNullOrEmpty(prefix))
				sb.Append(prefix.Replace(leadingBrace, escapedLeadingBrace).Replace(trailingBrace, escapedTrailingBrace));
			sb.Append(leadingBrace);
			sb.Append("0");
			if(!string.IsNullOrEmpty(formatString)) {
				sb.Append(":");
				sb.Append(formatString);
			}
			sb.Append(trailingBrace);
			if(!string.IsNullOrEmpty(postfix))
				sb.Append(postfix.Replace(leadingBrace, escapedLeadingBrace).Replace(trailingBrace, escapedTrailingBrace));
			return sb.ToString();
		}
	}
	#endregion
	#region XlExportNetFormatType
	public enum XlExportNetFormatType {
		General,
		Standard,
		Custom,
		NotSupported
	}
	#endregion
	#region XlExportNumberFormatConverter
	public class XlExportNumberFormatConverter : FormatStringToExcelNumberFormatConverter {
		#region LocalDateFormat
		class LocalDateFormatInfo {
			public LocalDateFormatInfo(char daySymbol, char monthSymbol, char yearSymbol, char hourSymbol, char minuteSymbol, char secondSymbol, char dateSeparator, char timeSeparator) {
				DaySymbol = daySymbol;
				MonthSymbol = monthSymbol;
				YearSymbol = yearSymbol;
				HourSymbol = hourSymbol;
				MinuteSymbol = minuteSymbol;
				SecondSymbol = secondSymbol;
				DateSeparator = dateSeparator;
				TimeSeparator = timeSeparator;
			}
			public char DaySymbol { get; private set; }
			public char MonthSymbol { get; private set; }
			public char YearSymbol { get; private set; }
			public char HourSymbol { get; private set; }
			public char MinuteSymbol { get; private set; }
			public char SecondSymbol { get; private set; }
			public char DateSeparator { get; private set; }
			public char TimeSeparator { get; private set; }
		}
		static LocalDateFormatInfo invariantFormatInfo = new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '/', ':');
		static LocalDateFormatInfo latinFormatInfo = new LocalDateFormatInfo('d', 'm', 'a', 'h', 'm', 's', '/', ':');
		static LocalDateFormatInfo russianFormatInfo = new LocalDateFormatInfo('Д', 'М', 'Г', 'ч', 'м', 'с', '.', ':');
		static LocalDateFormatInfo deutchFormatInfo = new LocalDateFormatInfo('T', 'M', 'J', 'h', 'm', 's', '.', ':');
		static LocalDateFormatInfo italianoFormatInfo = new LocalDateFormatInfo('g', 'm', 'a', 'h', 'm', 's', '/', ':');
		static LocalDateFormatInfo norwegianFormatInfo = new LocalDateFormatInfo('d', 'm', 'å', 't', 'm', 's', '.', ':');
		static LocalDateFormatInfo finnishFormatInfo = new LocalDateFormatInfo('p', 'k', 'v', 't', 'm', 's', '.', ':');
		static LocalDateFormatInfo greekFormatInfo = new LocalDateFormatInfo('η', 'μ', 'ε', 'ω', 'λ', 'δ', '/', ':');
		static Dictionary<string, LocalDateFormatInfo> localDateFormatTable = CreateLocalDateFormatTable();
		static Dictionary<string, LocalDateFormatInfo> CreateLocalDateFormatTable() {
			Dictionary<string, LocalDateFormatInfo> result = new Dictionary<string, LocalDateFormatInfo>();
			result.Add("en", invariantFormatInfo);
			result.Add("en-US", invariantFormatInfo);
			result.Add("en-IN", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("ga-IE", invariantFormatInfo);
			result.Add("cy-GB", invariantFormatInfo);
			result.Add("ru", russianFormatInfo);
			result.Add("ru-RU", russianFormatInfo);
			result.Add("de", deutchFormatInfo);
			result.Add("de-DE", deutchFormatInfo);
			result.Add("de-LU", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("it", italianoFormatInfo);
			result.Add("it-IT", italianoFormatInfo);
			result.Add("it-CH", new LocalDateFormatInfo('g', 'm', 'a', 'h', 'm', 's', '.', ':'));
			result.Add("es", latinFormatInfo);
			result.Add("es-AR", invariantFormatInfo);
			result.Add("es-BO", invariantFormatInfo);
			result.Add("es-CL", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("es-CO", invariantFormatInfo);
			result.Add("es-CR", invariantFormatInfo);
			result.Add("es-DO", invariantFormatInfo);
			result.Add("es-EC", invariantFormatInfo);
			result.Add("es-ES", latinFormatInfo);
			result.Add("es-SV", invariantFormatInfo);
			result.Add("es-GT", invariantFormatInfo);
			result.Add("es-HN", invariantFormatInfo);
			result.Add("es-MX", latinFormatInfo);
			result.Add("es-NI", invariantFormatInfo);
			result.Add("es-PA", invariantFormatInfo);
			result.Add("es-PY", invariantFormatInfo);
			result.Add("es-PE", invariantFormatInfo);
			result.Add("es-PR", invariantFormatInfo);
			result.Add("es-US", invariantFormatInfo);
			result.Add("es-UY", invariantFormatInfo);
			result.Add("es-VE", invariantFormatInfo);
			result.Add("pt", latinFormatInfo);
			result.Add("pt-BR", latinFormatInfo);
			result.Add("pt-PT", new LocalDateFormatInfo('d', 'm', 'a', 'h', 'm', 's', '-', ':'));
			result.Add("fr", new LocalDateFormatInfo('j', 'm', 'a', 'h', 'm', 's', '/', ':'));
			result.Add("fr-CA", new LocalDateFormatInfo('j', 'm', 'a', 'h', 'm', 's', '-', ':'));
			result.Add("fr-FR", new LocalDateFormatInfo('j', 'm', 'a', 'h', 'm', 's', '/', ':'));
			result.Add("fr-LU", invariantFormatInfo);
			result.Add("fr-MC", invariantFormatInfo);
			result.Add("fr-CH", new LocalDateFormatInfo('j', 'm', 'a', 'h', 'm', 's', '.', ':'));
			result.Add("uk", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("uk-UA", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("be", new LocalDateFormatInfo('Д', 'М', 'Г', 'ч', 'м', 'с', '.', ':'));
			result.Add("be-BY", new LocalDateFormatInfo('Д', 'М', 'Г', 'ч', 'м', 'с', '.', ':'));
			result.Add("pl", new LocalDateFormatInfo('d', 'm', 'r', 'g', 'm', 's', '-', ':'));
			result.Add("pl-PL", new LocalDateFormatInfo('d', 'm', 'r', 'g', 'm', 's', '-', ':'));
			result.Add("cs", new LocalDateFormatInfo('d', 'm', 'r', 'h', 'm', 's', '.', ':'));
			result.Add("cs-CZ", new LocalDateFormatInfo('d', 'm', 'r', 'h', 'm', 's', '.', ':'));
			result.Add("da", new LocalDateFormatInfo('d', 'm', 'å', 't', 'm', 's', '-', ':'));
			result.Add("da-DK", new LocalDateFormatInfo('d', 'm', 'å', 't', 'm', 's', '-', ':'));
			result.Add("nl", new LocalDateFormatInfo('d', 'm', 'j', 'u', 'm', 's', '-', ':'));
			result.Add("nl-BE", new LocalDateFormatInfo('d', 'm', 'j', 'u', 'm', 's', '/', ':'));
			result.Add("nl-NL", new LocalDateFormatInfo('d', 'm', 'j', 'u', 'm', 's', '-', ':'));
			result.Add("fi", finnishFormatInfo);
			result.Add("fi-FI", finnishFormatInfo);
			result.Add("sv", new LocalDateFormatInfo('D', 'M', 'Å', 't', 'm', 's', '-', ':'));
			result.Add("sv-FI", finnishFormatInfo);
			result.Add("sv-SE", new LocalDateFormatInfo('D', 'M', 'Å', 't', 'm', 's', '-', ':'));
			result.Add("el", greekFormatInfo);
			result.Add("el-GR", greekFormatInfo);
			result.Add("he", invariantFormatInfo);
			result.Add("he-IL", invariantFormatInfo);
			result.Add("id", invariantFormatInfo);
			result.Add("id-ID", invariantFormatInfo);
			result.Add("no", norwegianFormatInfo);
			result.Add("nb-NO", norwegianFormatInfo);
			result.Add("nn-NO", norwegianFormatInfo);
			result.Add("sk", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("sk-SK", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("sl", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("sl-SI", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("bg", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("bg-BG", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("hr", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("hr-HR", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("hr-BA", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '.', ':'));
			result.Add("hu", new LocalDateFormatInfo('n', 'h', 'é', 'ó', 'p', 'm', '.', ':'));
			result.Add("hu-HU", new LocalDateFormatInfo('n', 'h', 'é', 'ó', 'p', 'm', '.', ':'));
			result.Add("ro", invariantFormatInfo);
			result.Add("ro-RO", invariantFormatInfo);
			result.Add("et", invariantFormatInfo);
			result.Add("et-EE", invariantFormatInfo);
			result.Add("lv", invariantFormatInfo);
			result.Add("lv-LV", invariantFormatInfo);
			result.Add("lt", invariantFormatInfo);
			result.Add("lt-LT", invariantFormatInfo);
			result.Add("hy", invariantFormatInfo);
			result.Add("hy-AM", invariantFormatInfo);
			result.Add("az", invariantFormatInfo);
			result.Add("az-Cyrl-AZ", invariantFormatInfo);
			result.Add("az-Latn-AZ", invariantFormatInfo);
			result.Add("ka", invariantFormatInfo);
			result.Add("ka-GE", invariantFormatInfo);
			result.Add("ja", invariantFormatInfo);
			result.Add("ja-JP", invariantFormatInfo);
			result.Add("th", invariantFormatInfo);
			result.Add("th-TH", invariantFormatInfo);
			result.Add("vi", invariantFormatInfo);
			result.Add("vi-VN", invariantFormatInfo);
			result.Add("ko", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("ko-KR", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("kk", russianFormatInfo);
			result.Add("kk-KZ", russianFormatInfo);
			result.Add("zh", invariantFormatInfo);
			result.Add("zh-CN", invariantFormatInfo);
			result.Add("zh-SG", invariantFormatInfo);
			result.Add("zh-HK", invariantFormatInfo);
			result.Add("zh-MO", invariantFormatInfo);
			result.Add("zh-TW", invariantFormatInfo);
			result.Add("tr", new LocalDateFormatInfo('g', 'a', 'y', 's', 'd', 'n', '.', ':'));
			result.Add("tr-TR", new LocalDateFormatInfo('g', 'a', 'y', 's', 'd', 'n', '.', ':'));
			result.Add("ar", invariantFormatInfo);
			result.Add("ar-DZ", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("ar-BH", invariantFormatInfo);
			result.Add("ar-EG", invariantFormatInfo);
			result.Add("ar-IQ", invariantFormatInfo);
			result.Add("ar-JO", invariantFormatInfo);
			result.Add("ar-KW", invariantFormatInfo);
			result.Add("ar-LB", invariantFormatInfo);
			result.Add("ar-LY", invariantFormatInfo);
			result.Add("ar-SA", invariantFormatInfo);
			result.Add("ar-MA", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("ar-OM", invariantFormatInfo);
			result.Add("ar-QA", invariantFormatInfo);
			result.Add("ar-SY", invariantFormatInfo);
			result.Add("ar-TN", new LocalDateFormatInfo('d', 'm', 'y', 'h', 'm', 's', '-', ':'));
			result.Add("ar-YE", invariantFormatInfo);
			result.Add("fa", invariantFormatInfo);
			result.Add("fa-IR", invariantFormatInfo);
			return result;
		}
		#endregion
		#region Convert
		public XlExportNetFormatType GetFormatType(string formatString, bool isDateTimeFormat, CultureInfo culture) {
			if(string.IsNullOrEmpty(formatString))
				return XlExportNetFormatType.General;
			XlExportNetFormatParser parser = new XlExportNetFormatParser(formatString);
			if(string.IsNullOrEmpty(parser.FormatString))
				return XlExportNetFormatType.General;
			if(IsNotSupportedNumberFormat(parser.FormatString, isDateTimeFormat))
				return XlExportNetFormatType.NotSupported;
			if(!string.IsNullOrEmpty(parser.Prefix) || !string.IsNullOrEmpty(parser.Postfix))
				return XlExportNetFormatType.Custom;
			ExcelNumberFormat format = isDateTimeFormat ? ConvertDateTime(parser.FormatString, culture) : ConvertNumeric(parser.FormatString, culture);
			if(format == null || format.Id == 0)
				return XlExportNetFormatType.General;
			return format.Id == -1 ? XlExportNetFormatType.Custom : XlExportNetFormatType.Standard;
		}
		public ExcelNumberFormat Convert(string formatString, bool isDateTimeFormat, CultureInfo culture) {
			if(string.IsNullOrEmpty(formatString))
				return null;
			XlExportNetFormatParser parser = new XlExportNetFormatParser(formatString);
			if(string.IsNullOrEmpty(parser.FormatString))
				return null;
			if(IsNotSupportedNumberFormat(parser.FormatString, isDateTimeFormat))
				return null;
			ExcelNumberFormat format = isDateTimeFormat ? ConvertDateTime(parser.FormatString, culture) : ConvertNumeric(parser.FormatString, culture);
			if(format == null)
				return format;
			if(format.Id != -1 && string.IsNullOrEmpty(parser.Prefix) && string.IsNullOrEmpty(parser.Postfix))
				return format;
			if(!isDateTimeFormat && HasThousandBeforeDecimalSeparator(format.FormatString))
				format = new ExcelNumberFormat(-1, RelocateThousand(format.FormatString));
			return new ExcelNumberFormat(-1, ComposeFormatString(parser.Prefix, format.FormatString, parser.Postfix));
		}
		bool HasThousandBeforeDecimalSeparator(string formatCode) {
			bool hasQuotationMark = false;
			bool hasBackSlash = false;
			char prevChar = '\0';
			for(int i = 0; i < formatCode.Length; i++) {
				char ch = formatCode[i];
				if(ch == '"') {
					if(hasBackSlash)
						hasBackSlash = false;
					else
						hasQuotationMark = !hasQuotationMark;
				}
				else if(ch == '\\') {
					if(!hasQuotationMark)
						hasBackSlash = !hasBackSlash;
				}
				else if(hasBackSlash) {
					hasBackSlash = false;
					prevChar = '\0';
					continue;
				}
				else if(!hasQuotationMark) {
					if(prevChar == ',' && ch == '.')
						return true;
					if(ch == 'E' || ch == 'e' || ch == '%')
						return false;
					prevChar = ch;
				}
			}
			return false;
		}
		string RelocateThousand(string formatString) {
			StringBuilder sb = new StringBuilder();
			List<string> parts = XlNumberFormat.SplitFormatCode(formatString);
			for(int i = 0; i < parts.Count; i++) {
				if(i > 0)
					sb.Append(";");
				sb.Append(RelocateThousandCore(parts[i]));
			}
			return sb.ToString();
		}
		string RelocateThousandCore(string formatString) {
			if(string.IsNullOrEmpty(formatString))
				return formatString;
			List<string> tokens = ParseFormatCode(formatString);
			RelocateTokens(tokens);
			StringBuilder sb = new StringBuilder();
			foreach(string token in tokens)
				sb.Append(token);
			return sb.ToString();
		}
		List<string> ParseFormatCode(string formatCode) {
			List<string> result = new List<string>();
			bool hasQuotationMark = false;
			bool hasBackSlash = false;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < formatCode.Length; i++) {
				char ch = formatCode[i];
				sb.Append(ch);
				if(ch == '"') {
					if(hasBackSlash) {
						hasBackSlash = false;
						result.Add(sb.ToString());
						sb.Clear();
					}
					else {
						hasQuotationMark = !hasQuotationMark;
						if(!hasQuotationMark) {
							result.Add(sb.ToString());
							sb.Clear();
						}
					}
				}
				else if(ch == '\\') {
					if(!hasQuotationMark) {
						hasBackSlash = !hasBackSlash;
						if(!hasBackSlash) {
							result.Add(sb.ToString());
							sb.Clear();
						}
					}
				}
				else {
					if(hasBackSlash) {
						hasBackSlash = false;
						result.Add(sb.ToString());
						sb.Clear();
					}
					else if(!hasQuotationMark) {
						result.Add(sb.ToString());
						sb.Clear();
					}
				}
			}
			if(sb.Length > 0)
				result.Add(sb.ToString());
			return result;
		}
		void RelocateTokens(List<string> tokens) {
			int separatorIndex = tokens.IndexOf(".");
			while(separatorIndex > 0 && tokens[separatorIndex - 1] == ",") {
				separatorIndex--;
				tokens.RemoveAt(separatorIndex);
				int index = separatorIndex + 1;
				while(index < tokens.Count && (tokens[index] == "0" || tokens[index] == "#"))
					index++;
				tokens.Insert(index, ",");
			}
		}
		string ComposeFormatString(string prefix, string formatString, string postfix) {
			StringBuilder sb = new StringBuilder();
			List<string> parts = XlNumberFormat.SplitFormatCode(formatString);
			for(int i = 0; i < parts.Count; i++) {
				if(i > 0)
					sb.Append(";");
				string part = parts[i];
				if(string.IsNullOrEmpty(part)) {
					if(i == 1) {
						part = parts[0];
						if(!string.IsNullOrEmpty(part))
							part = "-" + part;
					}
					else if(i > 1)
						part = parts[0];
				}
				if(!string.IsNullOrEmpty(part)) {
					if(!string.IsNullOrEmpty(prefix))
						sb.AppendFormat("\"{0}\"", prefix);
					sb.Append(part);
					if(!string.IsNullOrEmpty(postfix))
						sb.AppendFormat("\"{0}\"", postfix);
				}
				else if(!string.IsNullOrEmpty(prefix) || !string.IsNullOrEmpty(postfix)) {
					sb.Append("\"");
					if(!string.IsNullOrEmpty(prefix))
						sb.Append(prefix);
					if(!string.IsNullOrEmpty(postfix))
						sb.Append(postfix);
					sb.Append("\"");
				}
			}
			return sb.ToString();
		}
		bool IsNotSupportedNumberFormat(string formatString, bool isDateTimeFormat) {
			string prefix = new String(formatString[0], 1);
			bool isRoundtrip = (prefix == "r" || prefix == "R");
			if(isDateTimeFormat) {
				isRoundtrip |= (prefix == "o" || prefix == "O");
				bool isLongUniversal = prefix == "U";
				return isRoundtrip || isLongUniversal;
			}
			bool isHexadecimal = (prefix == "x" || prefix == "X");
			return isHexadecimal || isRoundtrip;
		}
		List<char> StripExpChars(List<char> patternChars) {
			List<char> result = new List<char>();
			foreach(char item in patternChars) {
				if(item != 'e' && item != 'E')
					result.Add(item);
			}
			return result;
		}
		bool IsExponent(string pattern, int index) {
			int count = pattern.Length;
			char currentChar = pattern[index];
			bool result = currentChar == 'e' || currentChar == 'E';
			if(result) {
				char prevChar = index > 0 ? pattern[index - 1] : ' ';
				char nextChar = index < (count - 1) ? pattern[index + 1] : ' ';
				result = (prevChar == '0' || prevChar == '#' || prevChar == '.') && (nextChar == '+' || nextChar == '-' || nextChar == '0');
			}
			return result;
		}
		protected override string ProcessNetCustomPattern(string pattern, List<char> patternChars) {
			bool quoted = false;
			bool escaped = false;
			bool backSlashEscaped = false;
			patternChars = StripExpChars(patternChars);
			StringBuilder sb = new StringBuilder();
			int count = pattern.Length;
			for(int i = 0; i < count; i++) {
				char currentChar = pattern[i];
				if(quoted) {
					if(currentChar == '\'' || currentChar == '\"') {
						sb.Append('\"');
						quoted = false;
					}
					else sb.Append(currentChar);
				}
				else if(backSlashEscaped) {
					sb.Append(currentChar);
					backSlashEscaped = false;
				}
				else if(currentChar == '\\') {
					sb.Append(currentChar);
					backSlashEscaped = true;
				}
				else if(patternChars.Contains(currentChar)) {
					if(escaped) {
						sb.Append('\"');
						sb.Append(currentChar);
						escaped = false;
					}
					else
						sb.Append(currentChar);
				}
				else if(IsExponent(pattern, i)) {
					if(escaped) {
						sb.Append('\"');
						sb.Append(currentChar == 'e' ? 'E' : currentChar);
						escaped = false;
					}
					else
						sb.Append(currentChar == 'e' ? 'E' : currentChar);
				}
				else if(IsControlChar(currentChar)) {
					if(escaped)
						sb.Append(currentChar);
					else {
						sb.Append('\\');
						sb.Append(currentChar);
					}
				}
				else if(IsNonEscapedChar(currentChar)) {
					if(currentChar == '\'' || currentChar == '\"') {
						sb.Append('\"');
						quoted = !quoted;
					}
					else
						sb.Append(currentChar);
				}
				else {
					if(currentChar == ';') {
						if(escaped) {
							sb.Append('\"');
							escaped = false;
						}
						backSlashEscaped = false;
					}
					else if(!escaped) {
						sb.Append('\"');
						escaped = true;
					}
					sb.Append(pattern[i]);
				}
			}
			if(quoted || escaped)
				sb.Append('\"');
			string result = sb.ToString();
			result = result.Replace("\"tt\"", "AM/PM");
			return result;
		}
		#endregion
		#region Local format string
		public string GetLocalFormatString(string formatString, CultureInfo culture) {
			Guard.ArgumentNotNull(culture, "culture");
			if(string.IsNullOrEmpty(formatString))
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			bool quoted = false;
			bool escaped = false;
			bool braced = false;
			int count = formatString.Length;
			for(int i = 0; i < count; i++) {
				char currentChar = formatString[i];
				if(quoted) {
					sb.Append(currentChar);
					if(currentChar == '\"')
						quoted = false;
				}
				else if(braced) {
					sb.Append(currentChar);
					if(currentChar == ']')
						braced = false;
				}
				else {
					if(escaped) {
						sb.Append(currentChar);
						escaped = false;
					}
					else if(currentChar == '\\') {
						sb.Append(currentChar);
						escaped = true;
					}
					else if(currentChar == '[') {
						sb.Append(currentChar);
						braced = true;
					}
					else if(currentChar == '\"') {
						sb.Append(formatString[i]);
						quoted = true;
					}
					else if(currentChar == '.')
						sb.Append(culture.NumberFormat.NumberDecimalSeparator);
					else if(currentChar == ',')
						sb.Append(culture.NumberFormat.NumberGroupSeparator.Replace('\x00a0', ' '));
					else
						sb.Append(currentChar);
				}
			}
			return sb.ToString();
		}
		public string GetLocalDateFormatString(string formatString, CultureInfo culture) {
			Guard.ArgumentNotNull(culture, "culture");
			if(string.IsNullOrEmpty(formatString))
				return string.Empty;
			LocalDateFormatInfo localDateFormat = GetLocalDateFormat(culture);
			StringBuilder sb = new StringBuilder();
			bool quoted = false;
			bool escaped = false;
			bool braced = false;
			bool isDatePart = true;
			int count = formatString.Length;
			for(int i = 0; i < count; i++) {
				char currentChar = formatString[i];
				if(quoted) {
					sb.Append(currentChar);
					if(currentChar == '\"')
						quoted = false;
				}
				else if(braced) {
					sb.Append(currentChar);
					if(currentChar == ']')
						braced = false;
				}
				else {
					if(escaped) {
						sb.Append(currentChar);
						escaped = false;
					}
					else if(currentChar == '\\') {
						sb.Append(currentChar);
						escaped = true;
					}
					else if(currentChar == '[') {
						sb.Append(currentChar);
						if((i + 1) < count && (formatString[i + 1] != 'h' && formatString[i + 1] != 'm' && formatString[i + 1] != 's'))
							braced = true;
					}
					else if(currentChar == '\"') {
						sb.Append(formatString[i]);
						quoted = true;
					}
					else if(currentChar == '/') {
						if((i + 1) < count && formatString[i + 1] == 'P')
							sb.Append(currentChar);
						else {
							sb.Append(localDateFormat.DateSeparator);
							isDatePart = true;
						}
					}
					else if(currentChar == ':') {
						sb.Append(localDateFormat.TimeSeparator);
						isDatePart = false;
					}
					else if(currentChar == 'd') {
						sb.Append(localDateFormat.DaySymbol);
						isDatePart = true;
					}
					else if(currentChar == 'y') {
						sb.Append(localDateFormat.YearSymbol);
						isDatePart = true;
					}
					else if(currentChar == 'M') {
						if((i > 0 && formatString[i - 1] == 'A') || (i > 0 && formatString[i - 1] == 'P'))
							sb.Append(currentChar);
						else {
							sb.Append(localDateFormat.MonthSymbol);
							isDatePart = true;
						}
					}
					else if(currentChar == 'm') {
						if(isDatePart && (((i + 1) < count && formatString[i + 1] == ':') || ((i + 2) < count && formatString[i + 2] == ':')))
							isDatePart = false;
						sb.Append(isDatePart ? localDateFormat.MonthSymbol : localDateFormat.MinuteSymbol);
					}
					else if(currentChar == 'h' || currentChar == 'H') {
						sb.Append(localDateFormat.HourSymbol);
						isDatePart = false;
					}
					else if(currentChar == 's') {
						sb.Append(localDateFormat.SecondSymbol);
						isDatePart = false;
					}
					else if(currentChar == '.')
						sb.Append(culture.NumberFormat.NumberDecimalSeparator);
					else {
						sb.Append(currentChar);
						isDatePart = true;
					}
				}
			}
			return sb.ToString();
		}
		LocalDateFormatInfo GetLocalDateFormat(CultureInfo culture) {
			LocalDateFormatInfo result;
			if(!localDateFormatTable.TryGetValue(culture.Name, out result)) {
				string[] cultureNameParts = culture.Name.Split(new char[] { '-' }, StringSplitOptions.None);
				if(!localDateFormatTable.TryGetValue(cultureNameParts[0], out result)) 
					result = localDateFormatTable["en-US"];
			}
			return result;
		}
		#endregion
	}
	#endregion
}
