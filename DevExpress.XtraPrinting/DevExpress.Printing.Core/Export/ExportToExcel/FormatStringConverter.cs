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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.XtraExport {
	internal class FormatStringConverter {
		const double number = -123456789.987654321;
		const int noneDecimalNumber = 123456789;
		const string endFormatSymbol = "}";
		const string startFormatString = "{0:";
		public static FormatStringConverter CreateInstance(ushort preparedCellType, string formatStr) {
			string format = formatStr;			
			if(string.IsNullOrEmpty(format))
				return new FormatStringConverter(format);
			if(formatStr.IndexOf(startFormatString) != -1){
				int startIndex = formatStr.IndexOf(startFormatString) + 3;
				int length = formatStr.IndexOf(endFormatSymbol) - startIndex;
				format = formatStr.Substring(startIndex, length);
			}
			if(preparedCellType != XlsConsts.CurrencyNoneDecimalFormat && preparedCellType != XlsConsts.GeneralFormat)
				return new DateTimeFormatStringConverter(format);
			try {
				if(noneDecimalNumber.ToString(format).IndexOf("1") != -1 && preparedCellType == XlsConsts.GeneralFormat)
					return new FormatStringConverter(format);
				if(number.ToString(format) == format || number.ToString(format).IndexOf("1") == -1)
					return new DateTimeFormatStringConverter(format);
			}
			catch {
				return new DateTimeFormatStringConverter(format);
			}
			return new FormatStringConverter(format);
		}
		protected string formatString;
		ushort preparedCellType;
		public FormatStringConverter(string formatString) {
			this.formatString = formatString;
		}
		protected CultureInfo Culture {
			get {
#if !SL && !DXPORTABLE
				return System.Windows.Forms.Application.CurrentCulture;
#else
				return CultureInfo.CurrentCulture;
#endif
			}
		}
		public virtual ushort GetCellType(ushort preparedCellType){
			this.preparedCellType = preparedCellType;
			return GetCellType();
		}
		protected virtual ushort GetCellType() {
			if(string.IsNullOrEmpty(formatString))
				return preparedCellType;
			try {
			if(CurrencyFormat())
				if(DecimalSeparated(Culture.NumberFormat.CurrencyDecimalSeparator))
					return XlsConsts.CurrencyDecimalFormat;
				else
					return XlsConsts.CurrencyNoneDecimalFormat;
			if(PercentFormat())
				if(DecimalSeparated(Culture.NumberFormat.PercentDecimalSeparator))
					return XlsConsts.PercentDecimalFormat;
				else
					return XlsConsts.PercentNoneDecimalFormat;
			if(ExponentialFormat())
				return GetExponentialType();
			if(AccountFormat())
				if(DecimalSeparated(Culture.NumberFormat.NumberDecimalSeparator))
					return XlsConsts.AccountDecimalFormat;
				else
					return XlsConsts.AccountFormat;
			if(DigitSeparated())
				if(DecimalSeparated(Culture.NumberFormat.NumberDecimalSeparator))
					return XlsConsts.DigitDecimalFormat;
				else
					return XlsConsts.DigitNoneDecimalFormat;
			if(DecimalSeparated(Culture.NumberFormat.NumberDecimalSeparator))
				return XlsConsts.DecimalFormat;
			if(RealFormat(Culture.NumberFormat.NumberDecimalSeparator))
				return XlsConsts.RealFormat;
			return XlsConsts.NoneDecimalFormat;
			} catch {
				return XlsConsts.GeneralFormat;
			}
		}
		bool DecimalSeparated(string decimalSeparator) {
			string formated = noneDecimalNumber.ToString(formatString);
			return formated.LastIndexOf(decimalSeparator) != -1;
		}
		bool DigitSeparated() {
			return number.ToString(formatString).IndexOf(Culture.NumberFormat.NumberGroupSeparator, StringComparison.Ordinal) != -1;
		}
		bool CurrencyFormat() {
			string currencyString = Culture.NumberFormat.CurrencySymbol;
			return number.ToString(formatString).IndexOf(currencyString, StringComparison.Ordinal) != -1 || formatString.IndexOf(currencyString, StringComparison.Ordinal) != -1;
		}
		bool PercentFormat() {
			return number.ToString(formatString).IndexOf(Culture.NumberFormat.PercentSymbol, StringComparison.Ordinal) != -1;
		}
		bool ExponentialFormat() {
			string formated = number.ToString(formatString);
			return formated.IndexOf("E+") != -1 || formated.IndexOf("e+") != -1 || formated.IndexOf("E-") != -1 || formated.IndexOf("e-") != -1;
		}
		bool AccountFormat() {
			string formated = number.ToString(formatString);
			return formated.IndexOf("(") != -1 && formated.IndexOf(")") != -1;
		}
		bool RealFormat(string decimalSeparator) {
			string noneDecFormated = noneDecimalNumber.ToString(formatString);
			string decFormated = number.ToString(formatString);
			return decFormated.LastIndexOf(decimalSeparator) != -1 && noneDecFormated.LastIndexOf(decimalSeparator) == -1;
		}
		ushort GetExponentialType() {
			string formated = number.ToString(formatString);
			if(Math.Max(formated.IndexOf("e"), formated.IndexOf("E")) - formated.IndexOf(Culture.NumberFormat.NumberDecimalSeparator) < 3)
				return XlsConsts.ExponentialDecimalOneFormat;
			return XlsConsts.ExponentialDecimalFormat;
		}
	}
	internal class DateTimeFormatStringConverter : FormatStringConverter {
		public DateTimeFormatStringConverter(string formatString)
			: base(formatString) {
		}
		public override ushort GetCellType(ushort preparedCellType) {
			ushort result = GetCellType();
			return result == XlsConsts.GeneralFormat ? preparedCellType : result;
		}
		protected override ushort GetCellType() {
			if(string.IsNullOrEmpty(formatString))
				return XlsConsts.GeneralFormat;
			try {
			if(DateFormat() && TimeFormat())
				return XlsConsts.DateTimeFormat;
				if(TimeFormat())
				return GetTimeCellType();
			if(DateFormat())
				return GetDateCellType();
				if(DateAndTimeFormat())
					return GetDateTimeCellType();
			return XlsConsts.DateTimeFormat;
			} catch {
				return XlsConsts.GeneralFormat;
			}
		}
		ushort GetDateTimeCellType() {
			if(HaveYear() || HaveMonth())
				return XlsConsts.DateTimeFormat;
			if(HaveAbsoluteHours() || HaveDayAndHours())
				return XlsConsts.AbsoluteHourTimeFormat;
			if(HaveHours())
				return XlsConsts.HourMinuteSecondFormat;
			return XlsConsts.DateTimeFormat;
		}
		ushort GetTimeCellType() {
			if(AMPMFormat()) {
				if(HaveSeconds())
					return XlsConsts.HourMinuteSecondAMPMFormat;
				return XlsConsts.HourMinuteAMPMFormat;
			}
			if(!HaveSeconds())
				return XlsConsts.HourMinuteFormat;
			if(HaveMillisecond())
				return XlsConsts.MinuteSecondMilFormat;
			if(HaveAbsoluteHours() || HaveDayAndHours())
				return XlsConsts.AbsoluteHourTimeFormat;
			if(HaveHours())
				return XlsConsts.HourMinuteSecondFormat;
			return XlsConsts.MinuteSecondFormat;
		}
		ushort GetDateCellType() {
			if(!HaveYear())
				return XlsConsts.DayMonthFormat;
			if(!HaveDay())
				return XlsConsts.MonthYearFormat;
			if(!StandartDateSeparate())
				return XlsConsts.DayMonthYearFormat;
			return XlsConsts.DateFormat;
		}
		bool DateFormat() {
			DateTime date = new DateTime(5, 5, 5, 1, 1, 1);
			return date.ToString(formatString).IndexOf("5") != -1 && date.ToString(formatString).IndexOf("1") == -1;
		}
		bool TimeFormat() {
			DateTime time = new DateTime(1, 1, 1, 5, 5, 5);
			return time.ToString(formatString).IndexOf("5") != -1 && time.ToString(formatString).IndexOf("1") == -1;
		}
		bool DateAndTimeFormat() {
			DateTime time = new DateTime(1, 1, 23, 5, 5, 5);
			return time.ToString(formatString).IndexOf("23") != -1 && time.ToString(formatString).IndexOf("5") != -1;
		}
		bool AMPMFormat() {
			if(string.IsNullOrEmpty(Culture.DateTimeFormat.AMDesignator))
				return false;
			DateTime time = new DateTime(1, 1, 1, 1, 0, 0);
			return time.ToString(formatString).IndexOf(Culture.DateTimeFormat.AMDesignator) != -1;
		}
		bool HaveAbsoluteHours() {
			DateTime time = new DateTime(1, 1, 1, 5, 1, 1);
			return time.ToString(formatString).IndexOf("39") != -1;
		}
		bool HaveDayAndHours() {
			DateTime time = new DateTime(1, 1, 23, 5, 1, 1);
			return time.ToString(formatString).IndexOf("23") != -1;
		}
		bool HaveHours() {
			DateTime time = new DateTime(1, 1, 1, 5, 1, 1);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveSeconds() {
			DateTime time = new DateTime(1, 1, 1, 1, 1, 5);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveMillisecond() {
			DateTime time = new DateTime(1, 1, 1, 1, 1, 1, 5);
			return time.ToString(formatString).IndexOf("5") != -1;
		}
		bool HaveYear() {
			DateTime date = new DateTime(55, 1, 1);
			return date.ToString(formatString).IndexOf("55") != -1;
		}
		bool HaveMonth() {
			DateTime date = new DateTime(55, 7, 1);
			return date.ToString(formatString).IndexOf("7") != -1;
		}
		bool HaveDay() {
			DateTime date = new DateTime(1, 1, 5);
			return date.ToString(formatString).IndexOf("5") != -1;
		}
		bool StandartDateSeparate() {
#if !SL && !DXPORTABLE
			DateTime date = new DateTime(1, 1, 1);
			return date.ToString(formatString).IndexOf(Culture.DateTimeFormat.DateSeparator) != -1;
#else
			return false;
#endif
		}
	}
	public class ExcelNumberFormat {
		public ExcelNumberFormat(int id, string formatString) {
			this.Id = id;
			this.FormatString = formatString;
		}
		public int Id { get; internal set; }
		public string FormatString { get; private set; }
	}
	public class FormatStringToExcelNumberFormatConverter {
		static readonly string[] currencyPositivePatterns = new string[] { "$n", "n$", "$ n", "n $" };
		static readonly string[] currencyNegativePatterns = new string[] { "($n)", "-$n", "$-n", "$n-", "(n$)", "-n$", "n-$", "n$-", "-n $", "-$ n", "n $-", "$ n-", "$ -n", "n- $", "($ n)", "(n $)" };
		static readonly string[] percentPositivePatterns = new string[] { "n %", "n%", "%n", "% n" };
		static readonly string[] percentNegativePatterns = new string[] { "-n %", "-n%", "-%n", "%-n", "%n-", "n-%", "n%-", "-% n", "n %-", "% n-", "% -n", "n- %" };
		static readonly List<char> numberChars = new List<char>(new char[] { '0', '#', '.', ',', '%', '‰', 'e', 'E', '+', '-' });
		static readonly List<char> dateTimeChars = new List<char>(new char[] { 'd', 'D', 'm', 'M', 'y', 'Y', 'h', 'H', 's', 'S' });
		static readonly List<char> nonEscapedChars = new List<char>(new char[] { '$', '-', '+', '/', '(', ')', ':', '!', '^', '&', '\'', '~', '{', '}', '<', '>', '=', '\"' });
		static readonly List<char> controlChars = new List<char>(new char[] { '0', '#', '?', '.', '%', ',', '\\', '*', '_', ' ' });
		static readonly Dictionary<string, ExcelNumberFormat> numericStandardFormats = CreateNumericStandardFormats();
		static Dictionary<string, ExcelNumberFormat> CreateNumericStandardFormats() {
			Dictionary<string, ExcelNumberFormat> result = new Dictionary<string, ExcelNumberFormat>();
			result.Add("n", new ExcelNumberFormat(4, "#,##0.00"));
			result.Add("N", new ExcelNumberFormat(4, "#,##0.00"));
			result.Add("n0", new ExcelNumberFormat(3, "#,##0"));
			result.Add("N0", new ExcelNumberFormat(3, "#,##0"));
			result.Add("n2", new ExcelNumberFormat(4, "#,##0.00"));
			result.Add("N2", new ExcelNumberFormat(4, "#,##0.00"));
			result.Add("d", new ExcelNumberFormat(1, "0"));
			result.Add("D", new ExcelNumberFormat(1, "0"));
			result.Add("d0", new ExcelNumberFormat(1, "0"));
			result.Add("D0", new ExcelNumberFormat(1, "0"));
			result.Add("d1", new ExcelNumberFormat(1, "0"));
			result.Add("D1", new ExcelNumberFormat(1, "0"));
			result.Add("e", new ExcelNumberFormat(-1, "0.000000E+000"));
			result.Add("E", new ExcelNumberFormat(-1, "0.000000E+000"));
			result.Add("e0", new ExcelNumberFormat(-1, "0E+000"));
			result.Add("E0", new ExcelNumberFormat(-1, "0E+000"));
			result.Add("f", new ExcelNumberFormat(2, "0.00"));
			result.Add("F", new ExcelNumberFormat(2, "0.00"));
			result.Add("f0", new ExcelNumberFormat(1, "0"));
			result.Add("F0", new ExcelNumberFormat(1, "0"));
			result.Add("f2", new ExcelNumberFormat(2, "0.00"));
			result.Add("F2", new ExcelNumberFormat(2, "0.00"));
			result.Add("g", new ExcelNumberFormat(0, ""));
			result.Add("G", new ExcelNumberFormat(0, ""));
			result.Add("g0", new ExcelNumberFormat(0, ""));
			result.Add("G0", new ExcelNumberFormat(0, ""));
			result.Add("c", new ExcelNumberFormat(-1, ""));
			result.Add("C", new ExcelNumberFormat(-1, ""));
			return result;
		}
		protected CultureInfo Culture {
			get {
#if !SL && !DXPORTABLE
				return System.Windows.Forms.Application.CurrentCulture;
#else
				return CultureInfo.CurrentCulture;
#endif
			}
		}
#if !SL
		public ExcelNumberFormat GetExcelFormat(ExportCacheCellStyle style) {
			string formatString = style.FormatString;
			if(string.IsNullOrEmpty(formatString)) return new ExcelNumberFormat(style.PreparedCellType, "");
			if(style.PreparedCellType == XlsConsts.RealFormat) {
				return new ExcelNumberFormat(-1, "\"" + Regex.Replace(formatString, @"{[^}]*}", "\"@\"") + "\"");
			}
			string leftPart = "";
			string rightPart = "";
			if(formatString.IndexOf('{') != -1) {
				leftPart = formatString.Substring(0, formatString.IndexOf('{'));
				rightPart = formatString.Substring(formatString.IndexOf('}') + 1, formatString.Length - formatString.IndexOf('}') - 1);
				formatString = Regex.Match(formatString, @"{([^}]*)}").Groups[1].Value;
				if(formatString.IndexOf(':') != -1) {
					formatString = formatString.Remove(0, formatString.IndexOf(':') + 1);
				}
			}
			ExcelNumberFormat format = style.PreparedCellType == XlsConsts.DateTimeFormat ? ConvertDateTime(formatString, Culture) : ConvertNumeric(formatString, Culture);
			if(string.IsNullOrEmpty(leftPart) && string.IsNullOrEmpty(rightPart)) return format;
			string newFormatString = format.FormatString;
			if(!string.IsNullOrEmpty(leftPart)) newFormatString = "\"" + leftPart + "\"" + newFormatString;
			if(!string.IsNullOrEmpty(rightPart)) newFormatString = newFormatString + "\"" + rightPart + "\"";
			return new ExcelNumberFormat(-1, newFormatString);
		}
#endif
		public ExcelNumberFormat ConvertNumeric(string formatString, CultureInfo culture) {
			if (String.IsNullOrEmpty(formatString))
				return null;
			ExcelNumberFormat numericFormat = TryConvertFromStandardNumericFormat(formatString, culture);
			if (numericFormat != null)
				return numericFormat;
			return new ExcelNumberFormat(-1, ProcessNetNumberPattern(formatString));
		}
		public ExcelNumberFormat ConvertDateTime(string formatString, CultureInfo culture) {
			DateTimeFormatInfo dtfi = culture.DateTimeFormat;
			if (formatString == "d")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.ShortDatePattern));
			if (formatString == "D")
				return new ExcelNumberFormat(-1, "[$-F800]" + ProcessNetDateTimePattern(dtfi.LongDatePattern));
			if (formatString == "f")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.LongDatePattern + " " + dtfi.ShortTimePattern));
			if (formatString == "F")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.FullDateTimePattern));
			if (formatString == "g")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern));
			if (formatString == "G")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.ShortDatePattern + " " + dtfi.LongTimePattern));
			if (formatString == "m" || formatString == "M")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.MonthDayPattern));
			if (formatString == "s")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.SortableDateTimePattern));
			if (formatString == "t")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.ShortTimePattern));
			if (formatString == "T")
				return new ExcelNumberFormat(-1, "[$-F400]" + ProcessNetDateTimePattern(dtfi.LongTimePattern));
			if (formatString == "u")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.UniversalSortableDateTimePattern));
			if (formatString == "y" || formatString == "Y")
				return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(dtfi.YearMonthPattern));
			return new ExcelNumberFormat(-1, ProcessNetDateTimePattern(formatString));
		}
		string ProcessNetDateTimePattern(string pattern) {
			return ProcessNetCustomPattern(pattern, dateTimeChars);
		}
		string ProcessNetNumberPattern(string pattern) {
			return ProcessNetCustomPattern(pattern, numberChars);
		}
		protected virtual string ProcessNetCustomPattern(string pattern, List<char> patternChars) {
			bool insideQuotedString = false;
			StringBuilder sb = new StringBuilder();
			int count = pattern.Length;
			for (int i = 0; i < count; i++) {
				if(insideQuotedString) {
					if(pattern[i] == '\'' || pattern[i] == '\"') {
						sb.Append('\"');
						insideQuotedString = false;
					} else sb.Append(pattern[i]);
				} 
				else if (patternChars.Contains(pattern[i])) {
					if (insideQuotedString) {
						sb.Append('\"');
						sb.Append(pattern[i]);
						insideQuotedString = false;
					}
					else
						sb.Append(pattern[i] == 'e' ? 'E' : pattern[i]);
				}
				else if (IsControlChar(pattern[i])) {
					if (insideQuotedString)
						sb.Append(pattern[i]);
					else {
						sb.Append('\\');
						sb.Append(pattern[i]);
					}
				}
				else if (IsNonEscapedChar(pattern[i])) {
					if(pattern[i] == '\'' || pattern[i] == '\"') {
						sb.Append('\"');
						insideQuotedString = !insideQuotedString;
					}
					else
						sb.Append(pattern[i]);
				}
				else {
					if (pattern[i] == ';') {
						if (insideQuotedString) {
							sb.Append('\"');
							insideQuotedString = false;
						}
					}
					else if (!insideQuotedString) {
						sb.Append('\"');
						insideQuotedString = true;
					}
					sb.Append(pattern[i]);
				}
			}
			if (insideQuotedString)
				sb.Append('\"');
			string result = sb.ToString();
			result = result.Replace("\"tt\"", "AM/PM");
			return result;
		}
		protected bool IsControlChar(char item) {
			return controlChars.Contains(item);
		}
		protected bool IsNonEscapedChar(char item) {
			return nonEscapedChars.Contains(item);
		}
		ExcelNumberFormat TryConvertFromStandardNumericFormat(string formatString, CultureInfo culture) {
			string prefix = new String(formatString[0], 1);
			bool isCurrency = (prefix == "c" || prefix == "C");
			bool isPercent = (prefix == "p" || prefix == "P");
			if (!isCurrency && !isPercent)
				return TryConvertFromStandardNumericFormatCore(formatString, culture);
			ExcelNumberFormat numberFormat = TryConvertFromStandardNumericFormatCore("n" + formatString.Substring(1), culture);
			if (numberFormat == null)
				return null;
			NumberFormatInfo cultureNumberFormat = culture.NumberFormat;
			if (isCurrency) {
				string positiveFormat = CalculatePositiveCurrencyFormat(numberFormat.FormatString, culture);
				string negativeFormat = CalculateNegativeCurrencyFormat(numberFormat.FormatString, culture);
				ExcelNumberFormat currencyFormat = CreateCompositeFormat(positiveFormat, negativeFormat, cultureNumberFormat.NegativeSign, cultureNumberFormat.CurrencySymbol);
				return new ExcelNumberFormat(-1, string.IsNullOrEmpty(cultureNumberFormat.CurrencySymbol) ? currencyFormat.FormatString : 
					currencyFormat.FormatString.Replace(cultureNumberFormat.CurrencySymbol, CreateExcelLocaleString(culture)));
			}
			else {
				string positiveFormat = CalculatePositivePercentFormat(numberFormat.FormatString, culture);
				string negativeFormat = CalculateNegativePercentFormat(numberFormat.FormatString, culture);
				return CreateCompositeFormat(positiveFormat, negativeFormat, cultureNumberFormat.NegativeSign, cultureNumberFormat.PercentSymbol);
			}
		}
		string CreateExcelLocaleString(CultureInfo culture) {
			int lcid = LanguageIdToCultureConverter.Convert(culture);
			if (lcid == 0)
				lcid = LanguageIdToCultureConverter.Convert(new CultureInfo("en-US"));
			int code = (lcid & 0x0000FFFF);
			return String.Format("[${0}-{1:X4}]", culture.NumberFormat.CurrencySymbol, code);
		}
		ExcelNumberFormat CreateCompositeFormat(string positiveFormat, string negativeFormat, string negativeSign, string specialSign) {
			if (!String.IsNullOrEmpty(negativeSign) && !String.IsNullOrEmpty(specialSign)) {
				if ((negativeFormat.StartsWith(negativeSign) && negativeFormat.EndsWith(specialSign) && positiveFormat.EndsWith(specialSign))) {
					if (positiveFormat == negativeFormat.TrimStart(negativeSign[0]))
						return new ExcelNumberFormat(-1, positiveFormat);
				}
				if ((negativeFormat.EndsWith(negativeSign) && negativeFormat.StartsWith(specialSign) && positiveFormat.StartsWith(specialSign))) {
					if (positiveFormat == negativeFormat.TrimEnd(negativeSign[0]))
						return new ExcelNumberFormat(-1, positiveFormat);
				}
			}
			string zeroFormat = positiveFormat;
			string compositeFormat = positiveFormat + ";" + negativeFormat + ";" + zeroFormat + ";@";
			return new ExcelNumberFormat(-1, compositeFormat);
		}
		string CalculateNegativeCurrencyFormat(string numberFormatString, CultureInfo culture) {
			return CalculatePatternFormatCore(numberFormatString, culture, currencyNegativePatterns, culture.NumberFormat.CurrencyNegativePattern);
		}
		string CalculatePositiveCurrencyFormat(string numberFormatString, CultureInfo culture) {
			return CalculatePatternFormatCore(numberFormatString, culture, currencyPositivePatterns, culture.NumberFormat.CurrencyPositivePattern);
		}
		string CalculateNegativePercentFormat(string numberFormatString, CultureInfo culture) {
			return CalculatePatternFormatCore(numberFormatString, culture, percentNegativePatterns, culture.NumberFormat.PercentNegativePattern);
		}
		string CalculatePositivePercentFormat(string numberFormatString, CultureInfo culture) {
			return CalculatePatternFormatCore(numberFormatString, culture, percentPositivePatterns, culture.NumberFormat.PercentPositivePattern);
		}
		string CalculatePatternFormatCore(string numberFormatString, CultureInfo culture, string[] patterns, int patternIndex) {
			if (patternIndex < 0 || patternIndex >= patterns.Length)
				patternIndex = 0;
			NumberFormatInfo numberFormat = culture.NumberFormat;
			StringBuilder result = new StringBuilder();
			string pattern = patterns[patternIndex];
			int count = pattern.Length;
			for (int i = 0; i < count; i++) {
				switch (pattern[i]) {
					case '-':
						result.Append(numberFormat.NegativeSign);
						break;
					case 'n':
						result.Append(numberFormatString);
						break;
					case '$':
						result.Append(numberFormat.CurrencySymbol);
						break;
					case '%':
						result.Append(numberFormat.PercentSymbol);
						break;
					default:
						result.Append(pattern[i]);
						break;
				}
			}
			return result.ToString();
		}
		ExcelNumberFormat TryConvertFromStandardNumericFormatCore(string formatString, CultureInfo culture) {
			ExcelNumberFormat result;
			if (numericStandardFormats.TryGetValue(formatString, out result))
				return result;
			string prefix = new String(formatString[0], 1);
			if (!numericStandardFormats.TryGetValue(prefix, out result))
				return null;
			if (formatString.Length == 1)
				return result;
			string suffix = formatString.Substring(1, formatString.Length - 1);
			int decimals;
			if (!Int32.TryParse(suffix, NumberStyles.Integer, culture, out decimals))
				return null;
			int decimalPointIndex = result.FormatString.LastIndexOf('.');
			if (decimalPointIndex < 0) {
				if (result.FormatString.Length == 0)
					return result;
				if (result.FormatString.Length != 1)
					return null;
				return new ExcelNumberFormat(-1, new String(result.FormatString[0], Math.Max(1, decimals)));
			}
			int exponentIndex = result.FormatString.LastIndexOf('e');
			if (exponentIndex < 0)
				exponentIndex = result.FormatString.LastIndexOf('E');
			if (exponentIndex < 0) {
				int suffixIndex = CalculateFirstNonDigitIndex(result.FormatString, decimalPointIndex + 1);
				if (suffixIndex < 0)
					return new ExcelNumberFormat(-1, result.FormatString.Substring(0, decimalPointIndex + 1) + new String('0', decimals));
				else
					return new ExcelNumberFormat(-1, result.FormatString.Substring(0, decimalPointIndex + 1) + new String('0', decimals) + result.FormatString.Substring(suffixIndex));
			}
			else
				return new ExcelNumberFormat(-1, result.FormatString.Substring(0, decimalPointIndex + 1) + new String('0', decimals) + result.FormatString.Substring(exponentIndex));
		}
		public ExcelNumberFormat CalculateFinalGenericFormat(string formattedValue, CultureInfo culture) {
			int exponentIndex = formattedValue.LastIndexOf('e');
			if (exponentIndex < 0)
				exponentIndex = formattedValue.LastIndexOf('E');
			int decimalPointIndex = formattedValue.LastIndexOf(culture.NumberFormat.NumberDecimalSeparator);
			if (exponentIndex < 0 && decimalPointIndex < 0)
				return new ExcelNumberFormat(1, "0");
			if (exponentIndex < 0) {
				int decimals = formattedValue.Length - 1 - decimalPointIndex;
				if (decimals == 2)
					return new ExcelNumberFormat(2, "0.00");
				else
					return new ExcelNumberFormat(-1, "0." + new String('0', decimals));
			}
			else {
				if (decimalPointIndex < 0) {
					int orderIndex = CalculateFirstDigitIndex(formattedValue, exponentIndex);
					if (orderIndex < 0) 
						return new ExcelNumberFormat(0, "");
					else
						return new ExcelNumberFormat(-1,
							"0" +
							formattedValue[exponentIndex] +
							formattedValue.Substring(exponentIndex + 1, orderIndex - exponentIndex - 1) +
							new String('0', formattedValue.Length - orderIndex)
							);
				}
				else {
					if (exponentIndex <= decimalPointIndex) 
						return new ExcelNumberFormat(0, "");
					int orderIndex = CalculateFirstDigitIndex(formattedValue, exponentIndex);
					if (orderIndex < 0) 
						return new ExcelNumberFormat(0, "");
					else
						return new ExcelNumberFormat(-1,
							"0." +
							new String('0', exponentIndex - decimalPointIndex - 1) + 
							formattedValue[exponentIndex] +
							formattedValue.Substring(exponentIndex + 1, orderIndex - exponentIndex - 1) +
							new String('0', formattedValue.Length - orderIndex)
							);
				}
			}
		}
		int CalculateFirstDigitIndex(string text, int from) {
			int count = text.Length;
			for (int i = from; i < count; i++)
				if (text[i] >= '0' && text[i] <= '9')
					return i;
			return -1;
		}
		int CalculateFirstNonDigitIndex(string text, int from) {
			int count = text.Length;
			for (int i = from; i < count; i++)
				if (text[i] < '0' || text[i] > '9')
					return i;
			return -1;
		}
	}
}
