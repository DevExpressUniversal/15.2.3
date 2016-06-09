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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraExport.Csv {
	using DevExpress.XtraExport.Implementation;
#if !SL
	using System.Drawing;
#else
	using System.Windows.Media;
#endif
	#region IXlValueFormatter
	public interface IXlValueFormatter {
		string Format(XlVariantValue value, CultureInfo culture);
	}
	#endregion
	#region XlValueFormatterBase (abstract)
	abstract class XlValueFormatterBase : IXlValueFormatter {
		public virtual string Format(XlVariantValue value, CultureInfo culture) {
			if(value.IsNumeric)
				return FormatNumeric(value, culture);
			if(value.IsText)
				return value.TextValue;
			if(value.IsBoolean)
				return value.BooleanValue ? "TRUE" : "FALSE";
			if (value.IsError)
				return value.ErrorValue.Name;
			return string.Empty;
		}
		protected abstract string GetFormatString(CultureInfo culture);
		protected abstract string FormatNumeric(XlVariantValue value, CultureInfo culture);
	}
	#endregion
	#region XlNumericFormatterBase (abstract)
	abstract class XlNumericFormatterBase : XlValueFormatterBase {
		protected override string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			string formatString = GetFormatString(culture);
			if(string.IsNullOrEmpty(formatString))
				formatString = "G";
			string formattedValue;
			try {
				formattedValue = string.Format(culture, XlExportNetFormatComposer.CreateFormat(formatString), value.NumericValue);
			}
			catch(FormatException) {
				formattedValue = string.Format(culture, XlExportNetFormatComposer.CreateFormat("G"), value.NumericValue);
			}
			return formattedValue;
		}
	}
	#endregion
	#region XlDateTimeFormatterBase (abstract)
	abstract class XlDateTimeFormatterBase : XlValueFormatterBase {
		public override string Format(XlVariantValue value, CultureInfo culture) {
			if (value.IsNumeric && value.NumericValue < 0 || XlVariantValue.IsErrorDateTimeSerial(value.NumericValue, false))
				return new string('#', 255);
			return base.Format(value, culture);
		}
		protected override string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			string formatString = GetFormatString(culture);
			if(string.IsNullOrEmpty(formatString))
				formatString = "d";
			string formattedValue;
			try {
				formattedValue = string.Format(culture, XlExportNetFormatComposer.CreateFormat(formatString), value.DateTimeValue);
			}
			catch(FormatException) {
				formattedValue = string.Format(culture, XlExportNetFormatComposer.CreateFormat("d"), value.DateTimeValue);
			}
			return formattedValue;
		}
	}
	#endregion
	#region XlNumericFormatter
	class XlNumericFormatter : XlNumericFormatterBase {
		string formatString;
		public XlNumericFormatter(string formatString) {
			this.formatString = formatString;
		}
		protected override string GetFormatString(CultureInfo culture) {
			return formatString;
		}
	}
	#endregion
	#region XlTextFormatter
	class XlTextFormatter : XlNumericFormatterBase {
		protected override string GetFormatString(CultureInfo culture) {
			return "G";
		}
	}
	#endregion
	#region XlDateTimeFormatter
	class XlDateTimeFormatter : XlDateTimeFormatterBase {
		string formatString;
		public XlDateTimeFormatter(string formatString) {
			this.formatString = formatString;
		}
		protected override string GetFormatString(CultureInfo culture) {
			return formatString;
		}
	}
	#endregion
	#region XlShortDateFormatter
	class XlShortDateFormatter : XlDateTimeFormatterBase {
		protected override string GetFormatString(CultureInfo culture) {
			return culture.DateTimeFormat.ShortDatePattern;
		}
	}
	#endregion
	#region XlLongDateFormatter
	class XlLongDateFormatter : XlDateTimeFormatterBase {
		protected override string GetFormatString(CultureInfo culture) {
			return culture.DateTimeFormat.LongDatePattern;
		}
	}
	#endregion
	#region XlShortDateTimeFormatter
	class XlShortDateTimeFormatter : XlDateTimeFormatterBase {
		protected override string GetFormatString(CultureInfo culture) {
			return culture.DateTimeFormat.ShortDatePattern + " H:mm";
		}
	}
	#endregion
	#region XlShortTime12Formatter
	class XlShortTime12Formatter : XlDateTimeFormatterBase {
		protected override string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			DateTime dateTimeValue = value.DateTimeValue;
			return dateTimeValue.ToString(GetFormatString(culture), culture) + (dateTimeValue.Hour < 12 ? " AM" : " PM");
		}
		protected override string GetFormatString(CultureInfo culture) {
			return "h:mm";
		}
	}
	#endregion
	#region XlLongTime12Formatter
	class XlLongTime12Formatter : XlShortTime12Formatter {
		protected override string GetFormatString(CultureInfo culture) {
			return "h:mm:ss";
		}
	}
	#endregion
	#region XlMinuteSecondsMsFormatter
	class XlMinuteSecondsMsFormatter : XlDateTimeFormatterBase {
		protected override string GetFormatString(CultureInfo culture) {
			return string.Format("mm:ss{0}f", culture.NumberFormat.NumberDecimalSeparator);
		}
	}
	#endregion
	#region XlTimeSpanFormatter
	class XlTimeSpanFormatter : XlDateTimeFormatterBase {
		protected override string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			long hours = (long)TimeSpan.FromDays(value.NumericValue).TotalHours;
			return string.Format("{0:0}{1}{2}", hours, culture.GetTimeSeparator(), value.DateTimeValue.ToString(GetFormatString(culture), culture));
		}
		protected override string GetFormatString(CultureInfo culture) {
			return "mm:ss";
		}
	}
	#endregion
	#region XlFractionFormatter
	class XlFractionFormatter : XlNumericFormatterBase {
		class FractionData {
			public int IntegerPart {get; set; }
			public int Divident { get; set; }
			public int Divisor { get; set; }
		}
		readonly int divisorCount;
		public XlFractionFormatter(int divisorCount) 
			: base() {
			this.divisorCount = divisorCount;
		}
		protected override string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			FractionData data = CalculateFractionParts(Math.Abs((decimal)value.NumericValue));
			if(data.IntegerPart == 0 && data.Divident == 0)
				return "0";
			StringBuilder sb = new StringBuilder();
			if(value.NumericValue < 0)
				sb.Append('-');
			if(data.IntegerPart != 0)
				sb.Append(data.IntegerPart.ToString(culture));
			if(data.Divident != 0) {
				if(sb.Length > 0)
					sb.Append(' ');
				sb.Append(data.Divident.ToString(culture));
				sb.Append('/');
				sb.Append(data.Divisor.ToString(culture));
			}
			return sb.ToString();
		}
		protected override string GetFormatString(CultureInfo culture) {
			return string.Empty;
		}
		FractionData CalculateFractionParts(decimal value) {
			FractionData result = new FractionData();
			if(value == 0)
				return result;
			result.IntegerPart = (int)value;
			value = value - result.IntegerPart;
			int leftDivident = 0;
			int leftDivisor = 1;
			int rightDivident = 1;
			int rightDivisor = 1;
			int bestDivident;
			int bestDivisor;
			decimal minimalError;
			if(Math.Abs(value) < Math.Abs(value - rightDivident)) {
				minimalError = Math.Abs(value);
				bestDivident = leftDivident;
				bestDivisor = leftDivisor;
			}
			else {
				minimalError = Math.Abs(value - rightDivident);
				bestDivident = rightDivident;
				bestDivisor = rightDivisor;
			}
			for( ; ; ) {
				int divisor = leftDivisor + rightDivisor;
				if(!IsDivisorOk(divisor))
					break;
				int divident = leftDivident + rightDivident;
				if(value * divisor < divident) {
					rightDivident = divident;
					rightDivisor = divisor;
				}
				else {
					leftDivident = divident;
					leftDivisor = divisor;
				}
				decimal approximation = CalculateApproximation(divident, divisor, value);
				if(Math.Abs(value - approximation) < minimalError) {
					minimalError = Math.Abs(value - approximation);
					bestDivident = divident;
					bestDivisor = divisor;
				}
			}
			result.Divident = bestDivident;
			result.Divisor = bestDivisor;
			return result;
		}
		protected virtual decimal CalculateApproximation(int divident, int divisor, decimal value) {
			return divident / (decimal)divisor;
		}
		protected virtual bool IsDivisorOk(int divisor) {
			return divisor <= Math.Max(2, Math.Pow(10, divisorCount));
		}
	}
	#endregion
	#region XlFormatterFactory
	public class XlFormatterFactory {
		readonly Dictionary<int, IXlValueFormatter> predefinedFormatters;
		readonly Dictionary<string, IXlValueFormatter> customFormatters = new Dictionary<string, IXlValueFormatter>();
		readonly XlNumFmtParser parser = new XlNumFmtParser();
		public XlFormatterFactory() {
			predefinedFormatters = new Dictionary<int, IXlValueFormatter>();
			predefinedFormatters.Add(0, new XlNumericFormatter("G"));
			predefinedFormatters.Add(1, new XlNumericFormatter("0"));
			predefinedFormatters.Add(2, new XlNumericFormatter("0.00"));
			predefinedFormatters.Add(3, new XlNumericFormatter("#,##0"));
			predefinedFormatters.Add(4, new XlNumericFormatter("#,##0.00"));
			predefinedFormatters.Add(9, new XlNumericFormatter("0%"));
			predefinedFormatters.Add(10, new XlNumericFormatter("0.00%"));
			predefinedFormatters.Add(11, new XlNumericFormatter("0.00E+00"));
			predefinedFormatters.Add(12, new XlFractionFormatter(1));
			predefinedFormatters.Add(13, new XlFractionFormatter(2));
			predefinedFormatters.Add(14, new XlShortDateFormatter());
			predefinedFormatters.Add(15, new XlLongDateFormatter());
			predefinedFormatters.Add(16, new XlDateTimeFormatter("dd/MMM")); 
			predefinedFormatters.Add(17, new XlDateTimeFormatter("MMM/yy")); 
			predefinedFormatters.Add(18, new XlShortTime12Formatter());
			predefinedFormatters.Add(19, new XlLongTime12Formatter());
			predefinedFormatters.Add(20, new XlDateTimeFormatter("H:mm"));
			predefinedFormatters.Add(21, new XlDateTimeFormatter("H:mm:ss"));
			predefinedFormatters.Add(22, new XlShortDateTimeFormatter());
			predefinedFormatters.Add(37, new XlNumericFormatter("#,##0;(#,##0)"));
			predefinedFormatters.Add(38, new XlNumericFormatter("#,##0;(#,##0)"));
			predefinedFormatters.Add(39, new XlNumericFormatter("#,##0.00;(#,##0.00)"));
			predefinedFormatters.Add(40, new XlNumericFormatter("#,##0.00;(#,##0.00)"));
			predefinedFormatters.Add(45, new XlDateTimeFormatter("mm:ss"));
			predefinedFormatters.Add(46, new XlTimeSpanFormatter());
			predefinedFormatters.Add(47, new XlMinuteSecondsMsFormatter());
			predefinedFormatters.Add(48, new XlNumericFormatter("##0.0E+0"));
			predefinedFormatters.Add(49, new XlTextFormatter()); 
		}
		public IXlValueFormatter CreateFormatter(int numberFormatId) {
			if(predefinedFormatters.ContainsKey(numberFormatId))
				return predefinedFormatters[numberFormatId];
			return predefinedFormatters[0];
		}
		public IXlValueFormatter CreateFormatter(XlNumberFormat numberFormat) {
			if(predefinedFormatters.ContainsKey(numberFormat.FormatId))
				return predefinedFormatters[numberFormat.FormatId];
			if(customFormatters.ContainsKey(numberFormat.FormatCode))
				return customFormatters[numberFormat.FormatCode];
			IXlValueFormatter formatter = CreateCustomFormatter(numberFormat);
			if(formatter == null)
				formatter = numberFormat.IsDateTime ? predefinedFormatters[14] : predefinedFormatters[0];
			customFormatters.Add(numberFormat.FormatCode, formatter);
			return formatter;
		}
		IXlValueFormatter CreateCustomFormatter(XlNumberFormat numberFormat) {
			try {
				IXlNumFmt provider = parser.Parse(numberFormat.FormatCode);
				return new XlCustomFormatter(provider);
			}
			catch {
				return null;
			}
		}
	}
	#endregion
	#region XlCustomFormatter
	class XlCustomFormatter : IXlValueFormatter {
		readonly IXlNumFmt provider;
		public XlCustomFormatter(IXlNumFmt provider) {
			Guard.ArgumentNotNull(provider, "provider");
			this.provider = provider;
		}
		#region IXlValueFormatter Members
		public string Format(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = provider.Format(value, culture);
			return result.Text;
		}
		#endregion
	}
	#endregion
	#region XlNumberFormatParser
	#region XlNumFmtResult
	class XlNumFmtResult {
		public string Text { get; set; }
		public bool IsError { get; set; }
	}
	#endregion
	#region XlNumFmtType
	[Flags]
	enum XlNumFmtType {
		DateTime = 1,
		General = 2,
		Numeric = 4,
		Text = 8
	}
	#endregion
	#region IXlNumFmt
	interface IXlNumFmt {
		XlNumFmtType Type { get; }
		XlNumFmtResult Format(XlVariantValue value, CultureInfo culture);
		XlVariantValue Round(XlVariantValue value, CultureInfo culture);
		bool EnclosedInParenthesesForPositive();
	}
	#endregion
	#region IXlNumFmtElement
	interface IXlNumFmtElement {
		bool IsDigit { get; }
		XlNumFmtDesignator Designator { get; }
		void Format(XlVariantValue value, CultureInfo culture, XlNumFmtResult result);
		void FormatEmpty(CultureInfo culture, XlNumFmtResult result);
	}
	#endregion
	#region XlNumFmtSimple
	abstract class XlNumFmtSimple : List<IXlNumFmtElement>, IXlNumFmt {
		protected XlNumFmtSimple() {
		}
		protected XlNumFmtSimple(IEnumerable<IXlNumFmtElement> elements) {
			this.AddRange(elements);
		}
		public abstract XlNumFmtType Type { get; }
		public virtual XlNumFmtResult Format(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result;
			if(value.IsEmpty) {
				result = new XlNumFmtResult();
				result.Text = string.Empty;
				return result;
			}
			if (value.IsError) {
				result = new XlNumFmtResult();
				result.Text = value.ErrorValue.Name;
				return result;
			}
			try {
				result = FormatCore(value, culture);
				if(result.IsError)
					result.Text = "#";
			}
			catch {
				result = new XlNumFmtResult();
				result.IsError = true;
				result.Text = "#";
			}
			return result;
		}
		public abstract XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture);
		public virtual XlVariantValue Round(XlVariantValue value, CultureInfo culture) {
			return value;
		}
		public bool EnclosedInParenthesesForPositive() {
			bool opened = false;
			bool closed = false;
			IXlNumFmtElement element;
			for(int i = 0; i < Count; ++i) {
				element = this[i];
				XlNumFmtQuotedText text = element as XlNumFmtQuotedText;
				if(text != null)
					foreach(char c in text.Text) {
						if(c == '(')
							if(closed)
								return true;
							else
								opened = true;
						if(c == ')')
							if(opened)
								return true;
							else
								closed = true;
					}
				else {
					XlNumFmtBackslashedText bText = element as XlNumFmtBackslashedText;
					if(bText != null) {
						if(bText.Text == '(')
							if(closed)
								return true;
							else
								opened = true;
						if(bText.Text == ')')
							if(opened)
								return true;
							else
								closed = true;
					}
				}
			}
			return false;
		}
	}
	#endregion
	#region XlNumFmtComposite
	class XlNumFmtComposite : List<XlNumFmtSimple>, IXlNumFmt {
		public XlNumFmtType Type { get { return this[0].Type; } }
		public XlNumFmtResult Format(XlVariantValue value, CultureInfo culture) {
			XlNumFmtSimple actualProvider = CalculateActualPart(value);
			if(actualProvider == null) {
				XlNumFmtResult result = new XlNumFmtResult();
				result.Text = value.ToText(culture).TextValue;
				return result;
			}
			return actualProvider.Format(value, culture);
		}
		public XlVariantValue Round(XlVariantValue value, CultureInfo culture) {
			XlNumFmtSimple actualPart = CalculateActualPart(value);
			return actualPart == null ? value : actualPart.Round(value, culture);
		}
		XlNumFmtSimple CalculateActualPart(XlVariantValue value) {
			int count = Count;
			if(count == 0)
				return null;
			if(!value.IsNumeric) {
				if(count == 4)
					return this[3];
				XlNumFmtSimple part = this[count - 1];
				if(part.Type == XlNumFmtType.Text)
					return part;
				return null;
			}
			if(this[count - 1].Type == XlNumFmtType.Text)
				--count;
			if(count <= 1)
				return this[0];
			if(count > 2 && value.NumericValue == 0)
				return this[2];
			return value.NumericValue >= 0 ? this[0] : this[1];
		}
		public bool EnclosedInParenthesesForPositive() {
			if(Count < 1)
				return false;
			XlNumFmtSimple part = this[0];
			return part.EnclosedInParenthesesForPositive();
		}
	}
	#endregion
	#region XlNumFmtDateTimeBase
	abstract class XlNumFmtDateTimeBase : XlNumFmtSimple {
		public const int SystemLongDate = 0xF800;
		public const int SystemLongTime = 0xF400;
		protected XlNumFmtDateTimeBase() {
		}
		protected XlNumFmtDateTimeBase(IEnumerable<IXlNumFmtElement> elements)
			: base(elements) {
		}
		public override XlNumFmtType Type { get { return XlNumFmtType.DateTime; } }
		protected virtual bool HasMilliseconds { get { return false; } }
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = new XlNumFmtResult();
			if(!value.IsNumeric) {
				result.Text = value.ToText(culture).TextValue;
				return result;
			}
			result.Text = string.Empty;
			if(value.NumericValue < 0) {
				result.IsError = true;
				return result;
			}
			if(!HasMilliseconds)
				value.NumericValue += TimeSpan.FromMilliseconds(500).TotalDays;
			FormatDateTime(value, culture, result);
			return result;
		}
		protected virtual void FormatDateTime(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			foreach(IXlNumFmtElement element in this)
				element.Format(value, culture, result);
		}
	}
	#endregion
	#region XlNumFmtDateTime
	class XlNumFmtDateTime : XlNumFmtDateTimeBase {
		bool hasMilliseconds;
		XlNumFmtDisplayLocale locale;
		protected XlNumFmtDateTime() {
		}
		public XlNumFmtDateTime(IEnumerable<IXlNumFmtElement> elements, XlNumFmtDisplayLocale locale, bool hasMilliseconds)
			: base(elements) {
			this.locale = locale;
			this.hasMilliseconds = hasMilliseconds;
		}
		protected override bool HasMilliseconds { get { return hasMilliseconds; } }
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			if(locale == null || locale.HexCode < 0)
				return base.FormatCore(value, culture);
			CultureInfo cultureReplace = CalculateSpecificCulture(locale.HexCode); 
			return base.FormatCore(value, cultureReplace);
		}
		CultureInfo CalculateSpecificCulture(int hexCode) {
			int languageId = hexCode & 0x0000FFFF; 
			return LanguageIdToCultureConverter.Convert(languageId);
		}
	}
	#endregion
	#region XlNumFmtDateTimeSystemLongDate
	class XlNumFmtDateTimeSystemLongDate : XlNumFmtDateTimeBase {
		protected XlNumFmtDateTimeSystemLongDate() {
		}
		public XlNumFmtDateTimeSystemLongDate(IEnumerable<IXlNumFmtElement> elements)
			: base(elements) {
		}
		protected override void FormatDateTime(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			base.FormatDateTime(value, culture, result);
			result.Text = value.DateTimeValue.ToString(culture.DateTimeFormat.LongDatePattern, culture);
		}
	}
	#endregion
	#region XlNumFmtDateTimeSystemLongTime
	class XlNumFmtDateTimeSystemLongTime : XlNumFmtDateTimeBase {
		protected XlNumFmtDateTimeSystemLongTime() {
		}
		public XlNumFmtDateTimeSystemLongTime(IEnumerable<IXlNumFmtElement> elements)
			: base(elements) {
		}
		protected override void FormatDateTime(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			base.FormatDateTime(value, culture, result);
			result.Text = value.DateTimeValue.ToString(culture.DateTimeFormat.LongTimePattern, culture);
		}
	}
	#endregion
	#region XlNumFmtNumericSimple
	class XlNumFmtNumericSimple : XlNumFmtSimple {
		bool isNegativePart;
		bool grouping;
		int percentCount;
		int integerCount;
		int decimalCount;
		int displayFactor;
		int decimalSeparatorIndex;
		protected XlNumFmtNumericSimple() {
		}
		public XlNumFmtNumericSimple(int percentCount, int integerCount, int decimalCount, int displayFactor, int decimalSeparatorIndex, bool grouping, bool isNegativePart) {
			this.percentCount = percentCount;
			this.integerCount = integerCount;
			this.decimalCount = decimalCount;
			this.displayFactor = displayFactor;
			this.decimalSeparatorIndex = decimalSeparatorIndex;
			this.grouping = grouping;
			this.isNegativePart = isNegativePart;
		}
		public XlNumFmtNumericSimple(IEnumerable<IXlNumFmtElement> elements, int percentCount, int integerCount, int decimalCount, int displayFactor, int decimalSeparatorIndex, bool grouping, bool isNegativePart)
			: base(elements) {
			this.percentCount = percentCount;
			this.integerCount = integerCount;
			this.decimalCount = decimalCount;
			this.displayFactor = displayFactor;
			this.decimalSeparatorIndex = decimalSeparatorIndex;
			this.grouping = grouping;
			this.isNegativePart = isNegativePart;
		}
		public override XlNumFmtType Type { get { return XlNumFmtType.Numeric; } }
		protected bool IsNegativePart { get { return isNegativePart; } set { isNegativePart = value; } }
		protected bool Grouping { get { return grouping; } set { grouping = value; } }
		protected int DecimalSeparatorIndex { get { return decimalSeparatorIndex; } set { decimalSeparatorIndex = value; } }
		protected int PercentCount { get { return percentCount; } set { percentCount = value; } }
		protected int IntegerCount { get { return integerCount; } set { integerCount = value; } }
		protected int DecimalCount { get { return decimalCount; } set { decimalCount = value; } }
		protected int DisplayFactor { get { return displayFactor; } set { displayFactor = value; } }
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = new XlNumFmtResult();
			if(!value.IsNumeric) {
				result.Text = value.ToText(culture).TextValue;
				return result;
			}
			result.Text = string.Empty;
			FormatSimple(value, culture, result, Count - 1);
			return result;
		}
		protected void FormatSimple(XlVariantValue value, CultureInfo culture, XlNumFmtResult result, int endIndex) {
			if(value.NumericValue == 0)
				for(int i = 0; i <= endIndex; ++i)
					this[i].FormatEmpty(culture, result);
			else {
				double numericValue = Math.Abs(value.NumericValue);
				numericValue = double.Parse(numericValue.ToString()); 
				numericValue = Math.Round(numericValue * Math.Pow(100, percentCount) * Math.Pow(0.001, displayFactor), decimalCount, MidpointRounding.AwayFromZero);
				if(decimalSeparatorIndex < 0)
					FormatIntegerPart(numericValue, endIndex, 0, integerCount, grouping, culture, result);
				else {
					FormatIntegerPart(Math.Truncate(numericValue), decimalSeparatorIndex - 1, 0, integerCount, grouping, culture, result);
					XlNumFmtDecimalSeparator.Instance.FormatEmpty(culture, result);
					FormatDecimalPart(numericValue, decimalSeparatorIndex + 1, endIndex, decimalCount, culture, result);
				}
			}
			if(!isNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, culture);
		}
		protected void FormatIntegerPart(double numericValue, int startIndex, int endIndex, int digitsCount, bool grouping, CultureInfo culture, XlNumFmtResult result) {
			XlNumFmtResult resultT = new XlNumFmtResult();
			int i = startIndex;
			if(numericValue > 0) {
				int digitsCountInQueue = digitsCount;
				IXlNumFmtElement element;
				int numIndex = 0;
				for(; numericValue > 0 && digitsCountInQueue > 0; --i) {
					element = this[i];
					if(element.IsDigit) {
						element.Format(numericValue % 10, culture, resultT);
						numericValue = Math.Truncate(numericValue / 10);
						--digitsCountInQueue;
						if(grouping && numIndex % 3 == 0 && numIndex != 0)
							XlNumFmtGroupSeparator.Instance.FormatEmpty(culture, resultT);
						++numIndex;
					}
					else
						element.FormatEmpty(culture, resultT);
					result.Text = resultT.Text + result.Text;
					resultT.Text = string.Empty;
				}
				if(digitsCount > 0 || decimalSeparatorIndex >= 0)
					while(numericValue > 0) {
						XlNumFmtDigitZero.Instance.Format(numericValue % 10, culture, resultT);
						numericValue = Math.Truncate(numericValue / 10);
						if(grouping && numIndex % 3 == 0 && numIndex != 0)
							XlNumFmtGroupSeparator.Instance.FormatEmpty(culture, resultT);
						++numIndex;
						result.Text = resultT.Text + result.Text;
						resultT.Text = string.Empty;
					}
			}
			for(; i >= endIndex; --i) {
				this[i].FormatEmpty(culture, resultT);
				result.Text = resultT.Text + result.Text;
				resultT.Text = string.Empty;
			}
			result.IsError = result.IsError || resultT.IsError;
		}
		protected void FormatDecimalPart(double numericValue, int startIndex, int endIndex, int digitsCount, CultureInfo culture, XlNumFmtResult result) {
			int digit;
			IXlNumFmtElement element;
			decimal pow = (decimal)Math.Pow(10, digitsCount);
			decimal numericValueDecimal = (decimal)numericValue;
			numericValueDecimal = Math.Round((numericValueDecimal - Math.Truncate(numericValueDecimal)) * pow, 0, MidpointRounding.AwayFromZero);
			int i = startIndex;
			for(; numericValueDecimal > 0 && i < Count; ++i) {
				element = this[i];
				if(element.IsDigit) {
					pow /= 10;
					digit = (int)(numericValueDecimal / pow);
					element.Format(digit, culture, result);
					numericValueDecimal -= digit * pow;
				}
				else
					element.FormatEmpty(culture, result);
			}
			for(; i <= endIndex; ++i)
				this[i].FormatEmpty(culture, result);
		}
		protected string ApplyNegativeSign(string text, CultureInfo culture) {
			return '-' + text;
		}
		public override XlVariantValue Round(XlVariantValue value, CultureInfo culture) {
			if(!value.IsNumeric)
				return value;
			return Math.Round(value.NumericValue, decimalCount + percentCount * 2 + displayFactor * 3, MidpointRounding.AwayFromZero);
		}
	}
	#endregion
	#region XlNumFmtNumericExponent
	class XlNumFmtNumericExponent : XlNumFmtNumericSimple {
		bool explicitSign;
		int expIndex;
		int expCount;
		protected XlNumFmtNumericExponent() {
		}
		public XlNumFmtNumericExponent(IEnumerable<IXlNumFmtElement> elements, int integerCount, int decimalCount, int decimalSeparatorIndex, int expIndex, int expCount, bool explicitSign, bool grouping, bool isNegativePart)
			: base(elements, 0, integerCount, decimalCount, 0, decimalSeparatorIndex, grouping, isNegativePart) {
			this.explicitSign = explicitSign;
			this.expIndex = expIndex;
			this.expCount = expCount;
		}
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = new XlNumFmtResult();
			if(!value.IsNumeric) {
				result.Text = value.ToText(culture).TextValue;
				return result;
			}
			result.Text = string.Empty;
			double exponent;
			if(value.NumericValue == 0) {
				exponent = 0;
				for(int i = 0; i < expIndex; ++i)
					this[i].Format(value, culture, result);
			}
			else {
				Tuple<double, double> numericValue = CalculateExponent(value.NumericValue);
				exponent = numericValue.Item2;
				FormatSimple(numericValue.Item1, culture, result, expIndex - 1);
			}
			result.Text += "E";
			if(exponent < 0)
				result.Text += '-';
			else
				if(explicitSign)
					result.Text += '+';
			XlNumFmtResult resultT = new XlNumFmtResult();
			resultT.Text = string.Empty;
			FormatIntegerPart(Math.Abs(exponent), Count - 1, expIndex, expCount, false, culture, resultT);
			result.Text += resultT.Text;
			if(!IsNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, culture);
			return result;
		}
		Tuple<double, double> CalculateExponent(double numericValue) {
			double mantissa = Math.Abs(numericValue);
			double exponent = Math.Floor(Math.Floor(Math.Log10(mantissa)) / IntegerCount) * IntegerCount;
			mantissa = Math.Round(mantissa / Math.Pow(10, exponent), DecimalCount, MidpointRounding.AwayFromZero);
			return new Tuple<double, double>(mantissa, exponent);
		}
		public override XlVariantValue Round(XlVariantValue value, CultureInfo culture) {
			if(!value.IsNumeric || value.NumericValue == 0)
				return value;
			Tuple<double, double> numericValue = CalculateExponent(value.NumericValue);
			return Math.Round(numericValue.Item1, DecimalCount, MidpointRounding.AwayFromZero) * Math.Pow(10, numericValue.Item2);
		}
	}
	#endregion
	#region XlNumFmtNumericFraction
	class XlNumFmtNumericFraction : XlNumFmtNumericSimple {
		int dividentCount;
		int divisorCount;
		protected XlNumFmtNumericFraction() {
		}
		public XlNumFmtNumericFraction(IEnumerable<IXlNumFmtElement> elements, int percentCount, int integerCount, int preFractionIndex, int fractionSeparatorIndex, int divisorIndex, int dividentCount, int divisorCount, bool grouping, bool isNegativePart)
			: base(elements, percentCount, integerCount, fractionSeparatorIndex, divisorIndex, preFractionIndex, grouping, isNegativePart) {
			this.dividentCount = dividentCount;
			this.divisorCount = divisorCount;
		}
		protected int PreFractionIndex { get { return DecimalSeparatorIndex; } }
		protected int FractionSeparatorIndex { get { return DecimalCount; } }
		protected int DivisorIndex { get { return DisplayFactor; } }
		protected int DivisorCount { get { return divisorCount; } set { divisorCount = value; } }
		protected int DividentCount { get { return dividentCount; } set { dividentCount = value; } }
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = new XlNumFmtResult();
			if(!value.IsNumeric) {
				result.Text = value.ToText(culture).TextValue;
				return result;
			}
			result.Text = string.Empty;
			XlNumFmtResult resultT = new XlNumFmtResult();
			resultT.Text = string.Empty;
			double numericValue = Math.Abs(value.NumericValue) * Math.Pow(100, PercentCount);
			double integerPart = Math.Truncate(numericValue);
			Size fracValues;
			int i = 0;
			if(IntegerCount > 0) {
				fracValues = CalculateRationalApproximation((decimal)(numericValue - integerPart));
				IXlNumFmtElement element;
				for(; ; ++i) {
					element = this[i];
					if(element.IsDigit)
						break;
					element.FormatEmpty(culture, result);
				}
				if(integerPart == 0) {
					FormatZeroIntegerPart(PreFractionIndex, i, culture, resultT);
					if(fracValues.Width != 0)
						resultT.Text = new string(' ', resultT.Text.Length);
				}
				else
					FormatIntegerPart(integerPart, PreFractionIndex, i, IntegerCount, Grouping, culture, resultT);
				result.Text += resultT.Text;
			}
			else
				fracValues = CalculateRationalApproximation((decimal)(numericValue));
			int divident = fracValues.Width;
			int divisor = fracValues.Height;
			resultT.Text = string.Empty;
			if(divident == 0) {
				FormatZeroIntegerPart(FractionSeparatorIndex - 1, PreFractionIndex + 1, culture, resultT);
				resultT.Text += "/";
				FormatDivisor(divisor, DivisorIndex, culture, resultT);
				if(IntegerCount > 0)
					resultT.Text = new string(' ', resultT.Text.Length);
			}
			else {
				FormatIntegerPart(divident, FractionSeparatorIndex - 1, PreFractionIndex + 1, dividentCount, false, culture, resultT);
				resultT.Text += "/";
				FormatDivisor(divisor, DivisorIndex, culture, resultT);
			}
			result.Text += resultT.Text;
			for(i = DivisorIndex; i < Count; ++i)
				this[i].FormatEmpty(culture, result);
			if(!IsNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, culture);
			return result;
		}
		void FormatZeroIntegerPart(int startIndex, int endIndex, CultureInfo culture, XlNumFmtResult result) {
			XlNumFmtResult resultT = new XlNumFmtResult();
			resultT.Text = string.Empty;
			int i = startIndex;
			IXlNumFmtElement element;
			for(; i >= endIndex; --i) {
				element = this[i];
				if(element.IsDigit) {
					--i;
					break;
				}
				else {
					element.FormatEmpty(culture, resultT);
					result.Text = resultT.Text + result.Text;
					resultT.Text = string.Empty;
				}
			}
			for(; i >= endIndex; --i) {
				this[i].FormatEmpty(culture, resultT);
				result.Text = resultT.Text + result.Text;
				resultT.Text = string.Empty;
			}
			result.IsError = result.IsError || resultT.IsError;
		}
		protected virtual void FormatDivisor(int divisor, int endIndex, CultureInfo culture, XlNumFmtResult result) {
			int pow = (int)Math.Pow(10, Math.Truncate(Math.Log10(divisor)) + 1);
			int divisorTemp;
			int i = FractionSeparatorIndex;
			IXlNumFmtElement element;
			for(; pow > 1 && i < endIndex; ++i) {
				element = this[i];
				if(element.IsDigit) {
					pow /= 10;
					divisorTemp = divisor % pow;
					element.Format((divisor - divisorTemp) / pow, culture, result);
					divisor = divisorTemp;
				}
				else
					element.FormatEmpty(culture, result);
			}
			for(; i < endIndex; ++i)
				this[i].FormatEmpty(culture, result);
		}
		protected virtual Size CalculateRationalApproximation(decimal value) {
			int integerPart = (int)value;
			value = value - integerPart;
			int leftDivident = 0;
			int leftDivisor = 1;
			int rightDivident = 1;
			int rightDivisor = 1;
			int bestDivident;
			int bestDivisor;
			decimal minimalError;
			if(Math.Abs(value) < Math.Abs(value - rightDivident)) {
				minimalError = Math.Abs(value);
				bestDivident = leftDivident;
				bestDivisor = leftDivisor;
			}
			else {
				minimalError = Math.Abs(value - rightDivident);
				bestDivident = rightDivident;
				bestDivisor = rightDivisor;
			}
			int maxDivisor = (int)Math.Pow(10, divisorCount);
			for(; ; ) {
				int divisor = leftDivisor + rightDivisor;
				if(divisor >= maxDivisor)
					break;
				int divident = leftDivident + rightDivident;
				if(value * divisor < divident) {
					rightDivident = divident;
					rightDivisor = divisor;
				}
				else {
					leftDivident = divident;
					leftDivisor = divisor;
				}
				decimal approximation = (decimal)divident / divisor;
				if(Math.Abs(value - approximation) < minimalError) {
					minimalError = Math.Abs(value - approximation);
					bestDivident = divident;
					bestDivisor = divisor;
				}
			}
			return new Size(bestDivident + integerPart * bestDivisor, bestDivisor);
		}
		public override XlVariantValue Round(XlVariantValue value, CultureInfo culture) {
			if(!value.IsNumeric)
				return value;
			double numericValue = value.NumericValue;
			Size fracValues = CalculateRationalApproximation((decimal)Math.Abs(numericValue));
			return Math.Sign(numericValue) * ((double)fracValues.Width / fracValues.Height) * Math.Pow(10, PercentCount * 2);
		}
	}
	#endregion
	#region XlNumFmtNumericFractionExplicit
	class XlNumFmtNumericFractionExplicit : XlNumFmtNumericFraction {
		protected XlNumFmtNumericFractionExplicit() {
		}
		public XlNumFmtNumericFractionExplicit(IEnumerable<IXlNumFmtElement> elements, int percentCount, int integerCount, int preFractionIndex, int fractionSeparatorIndex, int divisorIndex, int dividentCount, int divisor, bool grouping, bool isNegativePart)
			: base(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, divisorIndex, dividentCount, divisor, grouping, isNegativePart) {
		}
		int ExplicitDivisor { get { return DivisorCount; } }
		protected override void FormatDivisor(int divisor, int endIndex, CultureInfo culture, XlNumFmtResult result) {
			for(int i = FractionSeparatorIndex; i < endIndex; ++i)
				this[i].FormatEmpty(culture, result);
			result.Text += ExplicitDivisor;
		}
		protected override Size CalculateRationalApproximation(decimal value) {
			int integerPart = (int)value;
			value -= integerPart;
			int divident = (int)Math.Round(value * ExplicitDivisor, 0, MidpointRounding.AwayFromZero);
			return new Size(divident + integerPart * ExplicitDivisor, ExplicitDivisor);
		}
	}
	#endregion
	#region XlNumFmtGeneral
	class XlNumFmtGeneral : XlNumFmtSimple {
		int generalIndex;
		public XlNumFmtGeneral() {
		}
		public XlNumFmtGeneral(IEnumerable<IXlNumFmtElement> elements, int generalIndex) {
			this.AddRange(elements);
			this.generalIndex = generalIndex;
		}
		public override XlNumFmtType Type { get { return XlNumFmtType.General; } }
		public override XlNumFmtResult Format(XlVariantValue value, CultureInfo culture) { 
			XlNumFmtResult result = new XlNumFmtResult();
			result.Text = value.ToText(culture).TextValue;
			return result;
		}
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			throw new InvalidOperationException();
		}
		public string FormatNumeric(XlVariantValue value, CultureInfo culture) {
			return value.ToText(culture).TextValue;
		}
	}
	#endregion
	#region XlNumFmtText
	class XlNumFmtText : XlNumFmtSimple {
		protected XlNumFmtText() {
		}
		public XlNumFmtText(IEnumerable<IXlNumFmtElement> elements)
			: base(elements) {
		}
		public override XlNumFmtType Type { get { return XlNumFmtType.Text; } }
		public override XlNumFmtResult FormatCore(XlVariantValue value, CultureInfo culture) {
			XlNumFmtResult result = new XlNumFmtResult();
			result.Text = string.Empty;
			foreach(IXlNumFmtElement element in this)
				element.Format(value, culture, result);
			return result;
		}
	}
	#endregion
	#region XlNumFmtParser
	enum XlNumFmtDesignator {
		Default = 0x0,
		AmPm = 0x1, 
		Asterisk = 0x2, 
		At = 0x4, 
		Backslash = 0x8, 
		Bracket = 0x10, 
		DateSeparator = 0x20,
		Day = 0x40,
		DayOfWeek = 0x80,
		DigitEmpty = 0x100, 
		DigitSpace = 0x200, 
		DigitZero = 0x400, 
		DecimalSeparator = 0x800,
		EndOfPart = 0x1000, 
		Exponent = 0x2000,
		FractionOrDateSeparator = 0x4000, 
		General = 0x8000,
		GroupSeparator = 0x10000,
		Hour = 0x20000,
		InvariantYear = 0x40000, 
		JapaneseEra = 0x80000, 
		Minute = 0x100000,
		Month = 0x200000,
		Percent = 0x400000, 
		Quote = 0x800000, 
		Second = 0x1000000,
		ThaiYear = 0x2000000, 
		TimeSeparator = 0x4000000,
		Underline = 0x8000000, 
		Year = 0x10000000,
	}
	class XlNumFmtParser {
		delegate void NumberFormatDesignatorParseMethod();
		readonly XlNumFmtLocalizer localizer;
		readonly List<Color> AllowedColors = new List<Color>() { DXColor.Red, DXColor.Black, DXColor.White, DXColor.Blue, DXColor.Magenta, DXColor.Yellow, DXColor.Cyan, DXColor.Green };
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods2;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods3;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods4;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods5;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods6;
		readonly Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod> parseMethods7;
		string formatString;
		List<XlNumFmtSimple> formats;
		List<IXlNumFmtElement> elements;
		XlNumFmtDisplayLocale locale;
		XlNumFmtSimple part;
		string elementString;
		IXlNumFmtElement element;
		int currentIndex;
		char currentSymbol;
		bool errorState;
		XlNumFmtDesignator designator;
		NumberFormatDesignatorParseMethod designatorParser;
		bool elapsed;
		bool hasMilliseconds;
		int elementLength;
		bool grouping;
		bool isDecimal;
		int displayFactor;
		int percentCount;
		int integerCount;
		int decimalCount;
		int dividentCount;
		int decimalSeparatorIndex;
		int preFractionIndex;
		bool explicitSign;
		int expCount;
		int expIndex;
		int divisorCount;
		int divisor;
		int divisorPow;
		int fractionSeparatorIndex;
		public XlNumFmtParser() {
			localizer = new XlNumFmtLocalizer();
			parseMethods = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods.Add(XlNumFmtDesignator.At, OnAt);
			parseMethods.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods.Add(XlNumFmtDesignator.Bracket, OnBracket);
			parseMethods.Add(XlNumFmtDesignator.DateSeparator, OnError);
			parseMethods.Add(XlNumFmtDesignator.Day, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault);
			parseMethods.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods.Add(XlNumFmtDesignator.DecimalSeparator, OnNumericSymbol);
			parseMethods.Add(XlNumFmtDesignator.DigitEmpty, OnNumericSymbol);
			parseMethods.Add(XlNumFmtDesignator.DigitSpace, OnNumericSymbol);
			parseMethods.Add(XlNumFmtDesignator.DigitZero, OnNumericSymbol);
			parseMethods.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart);
			parseMethods.Add(XlNumFmtDesignator.Exponent, OnESymbol);
			parseMethods.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnError);
			parseMethods.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods.Add(XlNumFmtDesignator.General, OnGeneral);
			parseMethods.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnGeneralOrDateTime);
			parseMethods.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnGeneralOrDateTime);
			parseMethods.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnGeneralOrDateTime);
			parseMethods.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnGeneralOrDateTime);
			parseMethods.Add(XlNumFmtDesignator.Hour, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods[XlNumFmtDesignator.Hour]);
			parseMethods.Add(XlNumFmtDesignator.InvariantYear, OnESymbol);
			parseMethods.Add(XlNumFmtDesignator.JapaneseEra, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.Minute, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.Percent, OnNumericSymbol);
			parseMethods.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods.Add(XlNumFmtDesignator.Second, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.ThaiYear, OnDateTimeSymbol);
			parseMethods.Add(XlNumFmtDesignator.TimeSeparator, OnDefault);
			parseMethods.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods.Add(XlNumFmtDesignator.Year, OnDateTimeSymbol);
			parseMethods2 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods2.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnAmPmOrDayOfWeek);
			parseMethods2.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnAmPmOrMonth);
			parseMethods2.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnAmPmOrYear);
			parseMethods2.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods2[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods2.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods2.Add(XlNumFmtDesignator.At, OnError);
			parseMethods2.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods2.Add(XlNumFmtDesignator.Bracket, OnBracket2);
			parseMethods2.Add(XlNumFmtDesignator.DateSeparator, OnDateSeparator);
			parseMethods2.Add(XlNumFmtDesignator.Day, OnDay);
			parseMethods2.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault2);
			parseMethods2.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods2.Add(XlNumFmtDesignator.DecimalSeparator, OnMilisecond);
			parseMethods2.Add(XlNumFmtDesignator.DigitEmpty, OnError);
			parseMethods2.Add(XlNumFmtDesignator.DigitSpace, OnError);
			parseMethods2.Add(XlNumFmtDesignator.DigitZero, OnError);
			parseMethods2.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart2);
			parseMethods2.Add(XlNumFmtDesignator.Exponent, OnESymbol2);
			parseMethods2.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnDefaultDateSeparator);
			parseMethods2.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods2.Add(XlNumFmtDesignator.General, OnGeneral2);
			parseMethods2.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnGeneralOrDay);
			parseMethods2.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods2[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods2.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnGeneralOrSecond);
			parseMethods2.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnGeneralOrInvariantYear);
			parseMethods2.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnGeneralOrJapaneseEra);
			parseMethods2.Add(XlNumFmtDesignator.Hour, OnHour);
			parseMethods2.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods2[XlNumFmtDesignator.Hour]);
			parseMethods2.Add(XlNumFmtDesignator.InvariantYear, OnESymbol2);
			parseMethods2.Add(XlNumFmtDesignator.JapaneseEra, OnJapaneseEra);
			parseMethods2.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnMonthOrMinute);
			parseMethods2.Add(XlNumFmtDesignator.Minute, OnMinute);
			parseMethods2.Add(XlNumFmtDesignator.Month, OnMonth);
			parseMethods2.Add(XlNumFmtDesignator.Percent, OnError);
			parseMethods2.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods2.Add(XlNumFmtDesignator.Second, OnSecond);
			parseMethods2.Add(XlNumFmtDesignator.ThaiYear, OnThaiYear);
			parseMethods2.Add(XlNumFmtDesignator.TimeSeparator, OnTimeSeparator);
			parseMethods2.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods2.Add(XlNumFmtDesignator.Year, OnYear);
			parseMethods3 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods3.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnError);
			parseMethods3.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnError);
			parseMethods3.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnError);
			parseMethods3.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods3[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods3.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods3.Add(XlNumFmtDesignator.At, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods3.Add(XlNumFmtDesignator.Bracket, OnBracket3);
			parseMethods3.Add(XlNumFmtDesignator.DateSeparator, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Day, OnError);
			parseMethods3.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods3.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods3.Add(XlNumFmtDesignator.DecimalSeparator, OnDecimalSeparator);
			parseMethods3.Add(XlNumFmtDesignator.DigitEmpty, OnDigitEmpty);
			parseMethods3.Add(XlNumFmtDesignator.DigitSpace, OnDigitSpace);
			parseMethods3.Add(XlNumFmtDesignator.DigitZero, OnDigitZero);
			parseMethods3.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart3);
			parseMethods3.Add(XlNumFmtDesignator.Exponent, OnESymbol3);
			parseMethods3.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnFractionSeparator);
			parseMethods3.Add(XlNumFmtDesignator.GroupSeparator, OnGroupSeparator);
			parseMethods3.Add(XlNumFmtDesignator.General, OnGeneral3);
			parseMethods3.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnError);
			parseMethods3.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods3[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods3.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnError);
			parseMethods3.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods3.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Hour, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods3[XlNumFmtDesignator.Hour]);
			parseMethods3.Add(XlNumFmtDesignator.InvariantYear, OnESymbol3);
			parseMethods3.Add(XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Minute, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Month, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Percent, OnPercent);
			parseMethods3.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods3.Add(XlNumFmtDesignator.Second, OnError);
			parseMethods3.Add(XlNumFmtDesignator.ThaiYear, OnError);
			parseMethods3.Add(XlNumFmtDesignator.TimeSeparator, OnError);
			parseMethods3.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods3.Add(XlNumFmtDesignator.Year, OnError);
			parseMethods4 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods4.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnError);
			parseMethods4.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnError);
			parseMethods4.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnError);
			parseMethods4.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods4[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods4.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods4.Add(XlNumFmtDesignator.At, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods4.Add(XlNumFmtDesignator.Bracket, OnBracket3);
			parseMethods4.Add(XlNumFmtDesignator.DateSeparator, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Day, OnError);
			parseMethods4.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods4.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods4.Add(XlNumFmtDesignator.DecimalSeparator, OnDecimalSeparator);
			parseMethods4.Add(XlNumFmtDesignator.DigitEmpty, OnDigitEmpty4);
			parseMethods4.Add(XlNumFmtDesignator.DigitSpace, OnDigitSpace4);
			parseMethods4.Add(XlNumFmtDesignator.DigitZero, OnDigitZero4);
			parseMethods4.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart4);
			parseMethods4.Add(XlNumFmtDesignator.Exponent, OnError);
			parseMethods4.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnError);
			parseMethods4.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods4.Add(XlNumFmtDesignator.General, OnGeneral3);
			parseMethods4.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnError);
			parseMethods4.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods4[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods4.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnError);
			parseMethods4.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods4.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Hour, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods4[XlNumFmtDesignator.Hour]);
			parseMethods4.Add(XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods4.Add(XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Minute, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Month, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Percent, OnDefault);
			parseMethods4.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods4.Add(XlNumFmtDesignator.Second, OnError);
			parseMethods4.Add(XlNumFmtDesignator.ThaiYear, OnError);
			parseMethods4.Add(XlNumFmtDesignator.TimeSeparator, OnError);
			parseMethods4.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods4.Add(XlNumFmtDesignator.Year, OnError);
			parseMethods5 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods5.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnError);
			parseMethods5.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnError);
			parseMethods5.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnError);
			parseMethods5.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods5[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods5.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods5.Add(XlNumFmtDesignator.At, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods5.Add(XlNumFmtDesignator.Bracket, OnBracket3);
			parseMethods5.Add(XlNumFmtDesignator.DateSeparator, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Day, OnError);
			parseMethods5.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods5.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods5.Add(XlNumFmtDesignator.DecimalSeparator, OnError);
			parseMethods5.Add(XlNumFmtDesignator.DigitEmpty, OnDigitEmpty5);
			parseMethods5.Add(XlNumFmtDesignator.DigitSpace, OnDigitSpace5);
			parseMethods5.Add(XlNumFmtDesignator.DigitZero, OnDigitZero5);
			parseMethods5.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart5);
			parseMethods5.Add(XlNumFmtDesignator.Exponent, OnError);
			parseMethods5.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnError);
			parseMethods5.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods5.Add(XlNumFmtDesignator.General, OnGeneral3);
			parseMethods5.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnError);
			parseMethods5.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods5[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods5.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnError);
			parseMethods5.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods5.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Hour, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods5[XlNumFmtDesignator.Hour]);
			parseMethods5.Add(XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods5.Add(XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Minute, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Month, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Percent, OnPercent);
			parseMethods5.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods5.Add(XlNumFmtDesignator.Second, OnError);
			parseMethods5.Add(XlNumFmtDesignator.ThaiYear, OnError);
			parseMethods5.Add(XlNumFmtDesignator.TimeSeparator, OnError);
			parseMethods5.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods5.Add(XlNumFmtDesignator.Year, OnError);
			parseMethods6 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods6.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnError);
			parseMethods6.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnError);
			parseMethods6.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnError);
			parseMethods6.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods6[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods6.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods6.Add(XlNumFmtDesignator.At, OnAt6);
			parseMethods6.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods6.Add(XlNumFmtDesignator.Bracket, OnBracket3);
			parseMethods6.Add(XlNumFmtDesignator.DateSeparator, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Day, OnError);
			parseMethods6.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods6.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods6.Add(XlNumFmtDesignator.DecimalSeparator, OnError);
			parseMethods6.Add(XlNumFmtDesignator.DigitEmpty, OnError);
			parseMethods6.Add(XlNumFmtDesignator.DigitSpace, OnError);
			parseMethods6.Add(XlNumFmtDesignator.DigitZero, OnError);
			parseMethods6.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart6);
			parseMethods6.Add(XlNumFmtDesignator.Exponent, OnError);
			parseMethods6.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnError);
			parseMethods6.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods6.Add(XlNumFmtDesignator.General, OnGeneral3);
			parseMethods6.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnError);
			parseMethods6.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods6[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods6.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnError);
			parseMethods6.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods6.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Hour, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods6[XlNumFmtDesignator.Hour]);
			parseMethods6.Add(XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods6.Add(XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Minute, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Month, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Percent, OnError);
			parseMethods6.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods6.Add(XlNumFmtDesignator.Second, OnError);
			parseMethods6.Add(XlNumFmtDesignator.ThaiYear, OnError);
			parseMethods6.Add(XlNumFmtDesignator.TimeSeparator, OnDefault);
			parseMethods6.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods6.Add(XlNumFmtDesignator.Year, OnError);
			parseMethods7 = new Dictionary<XlNumFmtDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods7.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek, OnError);
			parseMethods7.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month, OnError);
			parseMethods7.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Year, OnError);
			parseMethods7.Add(XlNumFmtDesignator.AmPm | XlNumFmtDesignator.DayOfWeek | XlNumFmtDesignator.Month, parseMethods7[XlNumFmtDesignator.AmPm | XlNumFmtDesignator.Month]);
			parseMethods7.Add(XlNumFmtDesignator.Asterisk, OnAsterisk);
			parseMethods7.Add(XlNumFmtDesignator.At, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Backslash, OnBackslash);
			parseMethods7.Add(XlNumFmtDesignator.Bracket, OnBracket3);
			parseMethods7.Add(XlNumFmtDesignator.DateSeparator, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.Day, OnError);
			parseMethods7.Add(XlNumFmtDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods7.Add(XlNumFmtDesignator.Default, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.DecimalSeparator, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.DigitEmpty, OnError);
			parseMethods7.Add(XlNumFmtDesignator.DigitSpace, OnError);
			parseMethods7.Add(XlNumFmtDesignator.DigitZero, OnError);
			parseMethods7.Add(XlNumFmtDesignator.EndOfPart, OnEndOfPart7);
			parseMethods7.Add(XlNumFmtDesignator.Exponent, OnError);
			parseMethods7.Add(XlNumFmtDesignator.FractionOrDateSeparator, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.GroupSeparator, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.General, OnGeneral7);
			parseMethods7.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day, OnGeneralOrDateTime7);
			parseMethods7.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Day | XlNumFmtDesignator.JapaneseEra, parseMethods7[XlNumFmtDesignator.General | XlNumFmtDesignator.Day]);
			parseMethods7.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.Second, OnGeneralOrDateTime7);
			parseMethods7.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.InvariantYear, OnGeneralOrDateTime7);
			parseMethods7.Add(XlNumFmtDesignator.General | XlNumFmtDesignator.JapaneseEra, OnGeneralOrDateTime7);
			parseMethods7.Add(XlNumFmtDesignator.Hour, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Hour | XlNumFmtDesignator.JapaneseEra, parseMethods7[XlNumFmtDesignator.Hour]);
			parseMethods7.Add(XlNumFmtDesignator.InvariantYear, OnError);
			parseMethods7.Add(XlNumFmtDesignator.JapaneseEra, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Minute, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Month, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Percent, OnError);
			parseMethods7.Add(XlNumFmtDesignator.Quote, OnQuote);
			parseMethods7.Add(XlNumFmtDesignator.Second, OnError);
			parseMethods7.Add(XlNumFmtDesignator.ThaiYear, OnError);
			parseMethods7.Add(XlNumFmtDesignator.TimeSeparator, OnDefault);
			parseMethods7.Add(XlNumFmtDesignator.Underline, OnUnderline);
			parseMethods7.Add(XlNumFmtDesignator.Year, OnError);
		}
		public XlNumFmtLocalizer Localizer { get { return localizer; } }
		public IXlNumFmt Parse(string formatString) {
			return Parse(formatString, CultureInfo.InvariantCulture);
		}
		IXlNumFmt Parse(string formatString, CultureInfo culture) {
			if(!string.IsNullOrEmpty(formatString)) {
				this.formatString = formatString;
				localizer.SetCulture(culture);
				IXlNumFmt format = ParseCore(culture);
				ClearLocals();
				return format;
			}
			return null;
		}
		void ClearLocals() {
			formats = null;
			elements = null;
			locale = null;
			part = null;
			elementString = null;
			element = null;
			formatString = null;
			currentIndex = -1;
			currentSymbol = '\0';
			errorState = false;
			elapsed = false;
			hasMilliseconds = false;
			elementLength = -1;
			grouping = false;
			isDecimal = false;
			displayFactor = 0;
			percentCount = 0;
			integerCount = 0;
			decimalCount = 0;
			dividentCount = 0;
			decimalSeparatorIndex = -1;
			preFractionIndex = -1;
			explicitSign = false;
			expCount = 0;
			expIndex = -1;
			divisorCount = 0;
			divisor = 0;
			divisorPow = 0;
			fractionSeparatorIndex = -1;
		}
		#region ParseCore
		IXlNumFmt ParseCore(CultureInfo culture) {
			formatString += ';';
			formats = new List<XlNumFmtSimple>();
			elements = new List<IXlNumFmtElement>();
			for (currentIndex = 0; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = XlNumFmtDesignator.Default;
				if (!parseMethods.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator(designator);
					designatorParser = parseMethods[designator];
				}
				designatorParser();
				if (errorState)
					return null;
				if (part == null)
					continue;
				formats.Add(part);
				part = null;
				elements = new List<IXlNumFmtElement>();
				if (formats.Count == 3) {
					int index = ++currentIndex;
					if (index == formatString.Length)
						break;
					ParseText();
					if (errorState) {
						errorState = false;
						elements.Clear();
						currentIndex = index;
						ParseGeneral();
						if (errorState)
							return null;
					}
					if (currentIndex < formatString.Length - 1) 
						return null;
					if (part.Type != XlNumFmtType.General || part.Count != 0)
						formats.Add(part);
					break;
				}
			}
			if (formats.Count == 1) {
				XlNumFmtSimple simpleResult = formats[0];
				return simpleResult;
			}
			XlNumFmtComposite result = new XlNumFmtComposite();
			result.AddRange(formats);
			return result;
		}
		void OnEndOfPart() {
			part = new XlNumFmtNumericSimple(elements, 0, 0, 0, 0, -1, false, formats.Count == 1);
		}
		void OnBackslash() {
			currentSymbol = formatString[++currentIndex];
			OnDefault();
		}
		void OnQuote() {
			StringBuilder sb = new StringBuilder();
			for(++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(currentSymbol == '"')
					break;
				sb.Append(currentSymbol);
			}
			elements.Add(new XlNumFmtQuotedText(sb.ToString()));
		}
		void OnUnderline() {
			elements.Add(new XlNumFmtUnderline(formatString[++currentIndex]));
		}
		void OnAsterisk() {
			elements.Add(new XlNumFmtAsterisk(formatString[++currentIndex]));
		}
		void OnBracket() {
			int i = currentIndex + 1;
			int closeBrackeIndex = formatString.IndexOf(']', i);
			if(closeBrackeIndex < 0) {
				OnDefault();
				return;
			}
			elementString = formatString.Substring(i, closeBrackeIndex - i);
			element = TryParseColor(elementString);
			if(element == null) {
				locale = TryParseLocale(elementString);
				element = locale;
				if(element == null) {
					int count = elements.Count;
					OnDateTimeSymbol();
					if(part == null) {
						errorState = false;
						currentIndex = i - 1;
						elements.RemoveRange(count, elements.Count - count); 
						element = TryParseExpr(elementString);
						if(element == null) {
							errorState = true;
							return;
						}
					}
					else
						return;
				}
			}
			elements.Add(element);
			currentIndex += elementString.Length + 1; 
		}
		void OnAt() {
			part = ParseText();
		}
		void OnNumericSymbol() {
			part = ParseNumeric();
		}
		void OnDateTimeSymbol() {
			part = ParseDateTime();
		}
		void OnDefault() {
			elements.Add(new XlNumFmtBackslashedText(currentSymbol));
		}
		void OnError() {
			errorState = true;
		}
		void OnESymbol() {
			if(char.IsLower(currentSymbol))
				OnDateTimeSymbol();
			else
				OnError();
		}
		void OnGeneral() {
			part = ParseGeneral();
		}
		void OnGeneralOrDateTime() {
			if(CheckIsGeneral())
				OnGeneral();
			else
				OnDateTimeSymbol();
		}
		void OnDayOfWeekOrDefault() {
			if(OnDayOfWeekCore()) {
				++currentIndex;
				OnDateTimeSymbol();
			}
			else
				OnDefault();
		}
		XlNumFmtDesignator ParseSeparator(XlNumFmtDesignator designator) {
			if((designator & XlNumFmtDesignator.DateSeparator) > 0)
				return XlNumFmtDesignator.DateSeparator;
			if((designator & XlNumFmtDesignator.TimeSeparator) > 0)
				return XlNumFmtDesignator.TimeSeparator;
			if((designator & XlNumFmtDesignator.DecimalSeparator) > 0)
				return XlNumFmtDesignator.DecimalSeparator;
			if((designator & XlNumFmtDesignator.GroupSeparator) > 0)
				return XlNumFmtDesignator.GroupSeparator;
			if((designator & XlNumFmtDesignator.FractionOrDateSeparator) > 0)
				return XlNumFmtDesignator.FractionOrDateSeparator;
			return XlNumFmtDesignator.Default;
		}
		#endregion
		#region ParseDateTime
		XlNumFmtSimple ParseDateTime() {
			elapsed = false;
			hasMilliseconds = false;
			elementLength = -1;
			for(; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = XlNumFmtDesignator.Default;
				if(!parseMethods2.TryGetValue(designator, out designatorParser))
					ParseSeparator2(designator);
				else
					designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart2() {
			if(locale != null) {
				if(locale.HexCode == XlNumFmtDateTimeBase.SystemLongDate) {
					part = new XlNumFmtDateTimeSystemLongDate(elements);
					return;
				}
				if(locale.HexCode == XlNumFmtDateTimeBase.SystemLongTime) {
					part = new XlNumFmtDateTimeSystemLongTime(elements);
					return;
				}
			}
			part = new XlNumFmtDateTime(elements, locale, hasMilliseconds);
		}
		void OnBracket2() {
			TryParseDateTimeCondition(ref elapsed);
		}
		void OnYear() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtYear(elementLength > 2 ? 4 : 2));
		}
		void OnMonthOrMinute() {
			elementLength = GetDateTimeBlockLength();
			if(elementLength > 2)
				elements.Add(new XlNumFmtMonth(elementLength > 5 ? 4 : elementLength));
			else {
				bool isMinute = false;
				for(int j = elements.Count - 1; j >= 0; --j) {
					element = elements[j];
					if(element is XlNumFmtDateBase) {
						if(element is XlNumFmtSeconds || element is XlNumFmtHours)
							isMinute = true;
						if(!(element is XlNumFmtAmPm))
							break;
					}
				}
				if(isMinute)
					elements.Add(new XlNumFmtMinutes(elementLength, false));
				else
					elements.Add(new XlNumFmtMonth(elementLength));
			}
		}
		void OnMonth() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtMonth(elementLength > 5 ? 4 : elementLength));
		}
		void OnMinute() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtMinutes(elementLength > 1 ? 2 : 1, false));
		}
		void OnDay() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtDay(elementLength > 4 ? 4 : elementLength));
		}
		void OnHour() {
			bool is12HourTime = false;
			for(int j = elements.Count - 1; j >= 0; --j) {
				element = elements[j];
				if(element is XlNumFmtDateBase) {
					if(element is XlNumFmtAmPm)
						is12HourTime = true;
					if(!(element is XlNumFmtTimeBase) || element is XlNumFmtHours)
						break;
				}
			}
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtHours(elementLength > 1 ? 2 : 1, false, is12HourTime));
		}
		void OnSecond() {
			for(int j = elements.Count - 1; j >= 0; --j) {
				XlNumFmtDateBase elementBase = elements[j] as XlNumFmtDateBase;
				if(elementBase != null) {
					if(elementBase is XlNumFmtMonth)
						elements[j] = new XlNumFmtMinutes(elementBase.Count, false);
					if(!(elementBase is XlNumFmtAmPm))
						break;
				}
			}
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtSeconds(elementLength > 1 ? 2 : 1, false));
		}
		void OnAmPmOrDayOfWeek() {
			if(!OnAmPmCore())
				OnDayOfWeekOrDefault2();
		}
		bool OnAmPmCore() {
			if((currentIndex + 5) <= formatString.Length && formatString.Substring(currentIndex, 5).ToLowerInvariant() == "am/pm") {
				if(elapsed) {
					errorState = true;
					return true;
				}
				currentIndex += 4;
				elements.Add(new XlNumFmtAmPm());
			}
			else
				if((currentIndex + 3) <= formatString.Length) {
					string ampm = formatString.Substring(currentIndex, 3);
					if(ampm.ToLowerInvariant() == "a/p") {
						if(elapsed) {
							errorState = true;
							return true;
						}
						currentIndex += 2;
						elements.Add(new XlNumFmtAmPm(char.IsLower(ampm[0]), char.IsLower(ampm[2])));
					}
					else
						return false;
				}
				else
					return false;
			for(int j = elements.Count - 2; j >= 0; --j) { 
				element = elements[j];
				if(element is XlNumFmtTimeBase) {
					XlNumFmtHours hours = element as XlNumFmtHours;
					if(hours != null) {
						hours.Is12HourTime = true;
						break;
					}
				}
				else
					if(element is XlNumFmtDateBase)
						break;
			}
			return true;
		}
		void OnDefaultDateSeparator() {
			elements.Add(XlNumFmtDefaultDateSeparator.Instance);
		}
		void OnDateSeparator() {
			elements.Add(XlNumFmtDateSeparator.Instance);
		}
		void OnTimeSeparator() {
			elements.Add(XlNumFmtTimeSeparator.Instance);
		}
		void OnMilisecond() {
			if(!OnMilisecondCore())
				OnDefault();
		}
		bool OnMilisecondCore() {
			elementLength = currentIndex;
			++currentIndex;
			while(formatString.Length > currentIndex)
				if(formatString[currentIndex] == '0')
					++currentIndex;
				else
					break;
			--currentIndex;
			elementLength = currentIndex - elementLength;
			if(elementLength == 0)
				return false;
			if(elementLength > 3)
				errorState = true;
			else {
				hasMilliseconds = true;
				elements.Add(new XlNumFmtMilliseconds(elementLength));
			}
			return true;
		}
		void OnESymbol2() {
			if(char.IsLower(currentSymbol))
				OnInvariantYear();
			else
				OnError();
		}
		void OnInvariantYear() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtInvariantYear(elementLength > 1 ? 2 : 1));
		}
		void OnThaiYear() {
			elementLength = GetDateTimeBlockLength();
			++currentIndex;
			if(currentIndex < formatString.Length) {
				char c = formatString[currentIndex];
				if(c == '1' || c == '2') {
					if(elements.Count > 0 || elementLength > 1)
						errorState = true;
					elements.Add(new XlNumFmtNotImplementedLocale(c - '0'));
					return;
				}
			}
			--currentIndex;
			elements.Add(new XlNumFmtThaiYear(elementLength > 2 ? 4 : 2));
		}
		void OnAmPmOrMonth() {
			if(!OnAmPmCore())
				OnMonth();
		}
		void OnAmPmOrYear() {
			if(!OnAmPmCore())
				OnYear();
		}
		void OnGeneral2() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnDefault();
		}
		void OnGeneralOrDay() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnDay();
		}
		void OnGeneralOrSecond() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnSecond();
		}
		void OnGeneralOrInvariantYear() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnInvariantYear();
		}
		void OnGeneralOrJapaneseEra() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnJapaneseEra();
		}
		void OnJapaneseEra() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new XlNumFmtJapaneseEra(elementLength > 2 ? 3 : elementLength));
		}
		bool OnDayOfWeekCore() {
			int index = currentIndex;
			elementLength = GetDateTimeBlockLength();
			if(elementLength < 3) {
				currentIndex = index;
				elementLength = 1;
				return false;
			}
			elements.Add(new XlNumFmtDayOfWeek(elementLength > 3 ? 4 : 3));
			return true;
		}
		void OnDayOfWeekOrDefault2() {
			if(!OnDayOfWeekCore())
				OnDefault();
		}
		bool ParseSeparator2(XlNumFmtDesignator designator) {
			if((designator & XlNumFmtDesignator.DecimalSeparator) > 0)
				if(OnMilisecondCore())
					return true;
			if((designator & XlNumFmtDesignator.DateSeparator) > 0) {
				OnDateSeparator();
				return true;
			}
			if((designator & XlNumFmtDesignator.FractionOrDateSeparator) > 0) {
				OnDateSeparator();
				return true;
			}
			if((designator & XlNumFmtDesignator.TimeSeparator) > 0) {
				OnTimeSeparator();
				return true;
			}
			if((designator & XlNumFmtDesignator.GroupSeparator) > 0) {
				OnDefault();
				return true;
			}
			OnError();
			return true;
		}
		#endregion
		#region ParseNumeric
		XlNumFmtSimple ParseNumeric() {
			grouping = false;
			isDecimal = false;
			displayFactor = 0;
			percentCount = 0;
			integerCount = 0;
			decimalCount = 0;
			dividentCount = 0;
			decimalSeparatorIndex = -1;
			preFractionIndex = -1;
			for(; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = XlNumFmtDesignator.Default;
				if(!parseMethods3.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods3[designator];
				}
				designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnBracket3() {
			int elementLength = TryParseCondition();
			if(elementLength < 0)
				errorState = true;
		}
		void OnDecimalSeparator() {
			isDecimal = true;
			if(decimalSeparatorIndex < 0)
				decimalSeparatorIndex = elements.Count;
			elements.Add(XlNumFmtDecimalSeparator.Instance);
		}
		void BeforeDigit() {
			if(elements.Count > 0)
				if(elements[elements.Count - 1].IsDigit) {
					if(displayFactor == 1)
						grouping = true;
				}
				else {
					preFractionIndex = elements.Count - 1;
					integerCount += dividentCount;
					dividentCount = 0;
				}
			displayFactor = 0;
			if(isDecimal)
				++decimalCount;
			else
				++dividentCount;
		}
		void OnDigitSpace() {
			BeforeDigit();
			elements.Add(XlNumFmtDigitSpace.Instance);
		}
		void OnDigitEmpty() {
			BeforeDigit();
			elements.Add(XlNumFmtDigitEmpty.Instance);
		}
		void OnDigitZero() {
			BeforeDigit();
			elements.Add(XlNumFmtDigitZero.Instance);
		}
		void OnEndOfPart3() {
			integerCount += dividentCount;
			PrepareGrouping();
			part = new XlNumFmtNumericSimple(elements, percentCount, integerCount, decimalCount, displayFactor, decimalSeparatorIndex, grouping, formats.Count == 1);
		}
		void OnExponent() {
			integerCount += dividentCount;
			PrepareGrouping();
			part = ParseNumericExponent(integerCount, decimalSeparatorIndex, decimalCount, grouping);
		}
		void OnGroupSeparator() {
			if(elements.Count > 0 && elements[elements.Count - 1].IsDigit)
				++displayFactor;
			else
				OnDefault();
		}
		void OnPercent() {
			++percentCount;
			elements.Add(XlNumFmtPercent.Instance);
		}
		void OnFractionSeparator() {
			if(isDecimal || dividentCount <= 0) {
				errorState = true;
				return;
			}
			if(integerCount > 0)
				PrepareGrouping();
			else
				if(grouping) {
					errorState = true;
					return;
				}
			part = ParseNumericFraction(integerCount, preFractionIndex, dividentCount, percentCount, grouping);
		}
		void OnESymbol3() {
			if(char.IsLower(currentSymbol))
				OnError();
			else
				OnExponent();
		}
		void OnGeneral3() {
			if(CheckIsGeneral())
				errorState = true;
			else
				OnDefault();
		}
		void OnDayOfWeekOrDefault3() {
			if(OnDayOfWeekCore()) {
				elements.RemoveAt(elements.Count - 1);
				OnError();
			}
			else
				OnDefault();
		}
		XlNumFmtDesignator ParseSeparator3(XlNumFmtDesignator designator) {
			if((designator & XlNumFmtDesignator.DecimalSeparator) > 0)
				return XlNumFmtDesignator.DecimalSeparator;
			if((designator & XlNumFmtDesignator.GroupSeparator) > 0)
				return XlNumFmtDesignator.GroupSeparator;
			if((designator & XlNumFmtDesignator.FractionOrDateSeparator) > 0)
				return XlNumFmtDesignator.FractionOrDateSeparator;
			if((designator & XlNumFmtDesignator.DateSeparator) > 0)
				return XlNumFmtDesignator.DateSeparator;
			if((designator & XlNumFmtDesignator.TimeSeparator) > 0)
				return XlNumFmtDesignator.TimeSeparator;
			return XlNumFmtDesignator.Default;
		}
		void PrepareGrouping() {
			int integerCountOld = integerCount;
			if(grouping && integerCount < 4) {
				int i;
				IXlNumFmtElement element;
				for(i = 0; i < elements.Count; ++i) {
					element = elements[i];
					if(element.IsDigit)
						break;
				}
				while(integerCount < 4) {
					elements.Insert(i, XlNumFmtDigitEmpty.Instance);
					++integerCount;
				}
				if(decimalSeparatorIndex >= 0)
					decimalSeparatorIndex += integerCount - integerCountOld;
			}
		}
		#endregion
		#region ParseNumericExponent
		XlNumFmtSimple ParseNumericExponent(int integerCount, int decimalSeparatorIndex, int decimalCount, bool grouping) {
			++currentIndex;
			if(currentIndex + 1 >= formatString.Length)
				return null;
			currentSymbol = formatString[currentIndex];
			if(currentSymbol == '+')
				explicitSign = true;
			else {
				explicitSign = false;
				if(currentSymbol != '-')
					return null;
			}
			expIndex = elements.Count;
			expCount = 0;
			for(++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = XlNumFmtDesignator.Default;
				if(!parseMethods4.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods4[designator];
				}
				designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnDigitEmpty4() {
			++expCount;
			elements.Add(XlNumFmtDigitEmpty.Instance);
		}
		void OnDigitZero4() {
			++expCount;
			elements.Add(XlNumFmtDigitZero.Instance);
		}
		void OnDigitSpace4() {
			++expCount;
			elements.Add(XlNumFmtDigitSpace.Instance);
		}
		void OnEndOfPart4() {
			part = new XlNumFmtNumericExponent(elements, integerCount, decimalCount, decimalSeparatorIndex, expIndex, expCount, explicitSign, grouping, formats.Count == 1);
		}
		#endregion
		#region ParseNumericFraction
		XlNumFmtSimple ParseNumericFraction(int integerCount, int preFractionIndex, int dividentCount, int percentCount, bool grouping) {
			displayFactor = -1; 
			divisorCount = 0;
			divisor = 0;
			divisorPow = 10000;
			fractionSeparatorIndex = elements.Count;
			for(++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator)) {
					if(char.IsNumber(currentSymbol)) {
						if(divisorPow <= 0)
							return null;
						divisor += ((int)currentSymbol - 48) * divisorPow;
						divisorPow /= 10;
						displayFactor = elements.Count;
						continue;
					}
					designator = XlNumFmtDesignator.Default;
				}
				if(!parseMethods5.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods5[designator];
				}
				designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart5() {
			if(divisor > 0) {
				while(divisor % 10 == 0)
					divisor /= 10;
				part = new XlNumFmtNumericFractionExplicit(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, displayFactor, dividentCount, divisor, grouping, formats.Count == 1);
				return;
			}
			if(divisorCount == 0) {
				errorState = true;
				return;
			}
			part = new XlNumFmtNumericFraction(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, displayFactor, dividentCount, divisorCount, grouping, formats.Count == 1);
		}
		void OnDigitSpace5() {
			++divisorCount;
			elements.Add(XlNumFmtDigitSpace.Instance);
			displayFactor = elements.Count;
		}
		void OnDigitZero5() {
			if(divisor > 0)
				divisorPow *= 10;
			else {
				++divisorCount;
				elements.Add(XlNumFmtDigitZero.Instance);
				displayFactor = elements.Count;
			}
		}
		void OnDigitEmpty5() {
			++divisorCount;
			elements.Add(XlNumFmtDigitEmpty.Instance);
			displayFactor = elements.Count;
		}
		#endregion
		#region ParseText
		XlNumFmtSimple ParseText() {
			for(; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator)) {
					if (char.IsNumber(currentSymbol)) {
						part = null;
						break;
					}
					designator = XlNumFmtDesignator.Default;
				}
				if(!parseMethods6.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator6(designator);
					designatorParser = parseMethods6[designator];
				}
				designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnAt6() {
			elements.Add(XlNumFmtTextContent.Instance);
		}
		void OnEndOfPart6() {
			part = new XlNumFmtText(elements);
		}
		XlNumFmtDesignator ParseSeparator6(XlNumFmtDesignator designator) {
			if((designator & XlNumFmtDesignator.FractionOrDateSeparator) > 0)
				return XlNumFmtDesignator.FractionOrDateSeparator;
			if((designator & XlNumFmtDesignator.DecimalSeparator) > 0)
				return XlNumFmtDesignator.DecimalSeparator;
			if((designator & XlNumFmtDesignator.GroupSeparator) > 0)
				return XlNumFmtDesignator.GroupSeparator;
			if((designator & XlNumFmtDesignator.DateSeparator) > 0)
				return XlNumFmtDesignator.DateSeparator;
			if((designator & XlNumFmtDesignator.TimeSeparator) > 0)
				return XlNumFmtDesignator.TimeSeparator;
			return XlNumFmtDesignator.Default;
		}
		#endregion
		#region ParseGeneral
		XlNumFmtSimple ParseGeneral() {
			fractionSeparatorIndex = -1;
			for(; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if(!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = XlNumFmtDesignator.Default;
				if(!parseMethods7.TryGetValue(designator, out designatorParser))
					designatorParser = OnDefault;
				designatorParser();
				if(errorState)
					return null;
				if(currentSymbol == ';')
					return part;
			}
			if(part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart7() {
			if (fractionSeparatorIndex < 0)
				errorState = true;
			part = new XlNumFmtGeneral(elements, fractionSeparatorIndex);
		}
		void OnGeneral7() {
			if(CheckIsGeneral())
				OnGeneralCore();
			else
				OnDefault();
		}
		void OnGeneralOrDateTime7() {
			if(CheckIsGeneral())
				OnGeneralCore();
			else
				errorState = true;
		}
		void OnGeneralCore() {
			if(fractionSeparatorIndex >= 0) {
				errorState = true;
				return;
			}
			currentIndex += localizer.GeneralDesignator.Length - 1;
			fractionSeparatorIndex = elements.Count;
		}
		#endregion
		#region Helpers
		string TryGetConditionString() {
			int i = currentIndex + 1;
			int closeBrackeIndex = formatString.IndexOf(']', i);
			if(closeBrackeIndex < 0)
				return null;
			int elementLength = closeBrackeIndex - i;
			return formatString.Substring(i, elementLength);
		}
		int TryParseCondition() {
			string elementString = TryGetConditionString();
			if(string.IsNullOrEmpty(elementString)) {
				OnDefault();
				return 1;
			}
			element = TryParseColor(elementString);
			if(element == null) {
				element = TryParseLocale(elementString);
				if(element == null) {
					element = TryParseExpr(elementString);
					if(element == null)
						return -1;
					else
						elements.Add(element);
				}
				else
					elements.Add(element);
			}
			else
				elements.Insert(0, element);
			currentIndex += elementString.Length + 1; 
			return elementString.Length + 2; 
		}
		int TryParseDateTimeCondition(ref bool elapsed) {
			locale = null;
			string elementString = TryGetConditionString();
			if(string.IsNullOrEmpty(elementString)) {
				OnDefault();
				return 1;
			}
			element = TryParseColor(elementString);
			if(element == null) {
				element = TryParseLocale(elementString);
				if(element == null) {
					element = TryParseElapsed(elementString);
					if(element == null) {
						element = TryParseExpr(elementString);
						if(element == null)
							return -1;
						else
							elements.Add(element);
					}
					else {
						if(elapsed)
							return -1;
						elapsed = true;
						elements.Add(element);
					}
				}
				else {
					if(locale != null)
						return -1;
					else
						locale = element as XlNumFmtDisplayLocale;
					elements.Add(element);
				}
			}
			else
				elements.Insert(0, element);
			currentIndex += elementString.Length + 1;
			return elementString.Length + 2; 
		}
		XlNumFmtColor TryParseColor(string colorString) {
			Color color = Color.FromName(colorString);
			return AllowedColors.Contains(color) ? new XlNumFmtColor(color) : null;
		}
		XlNumFmtDisplayLocale TryParseLocale(string localeString) {
			if(localeString[0] == '$') {
				int dashIndex = localeString.IndexOf('-', 1);
				if(dashIndex > 0) {
					string currency = localeString.Substring(1, dashIndex - 1);
					localeString = localeString.Remove(0, dashIndex + 1);
					int localeCode;
					if(int.TryParse(localeString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out localeCode))
						return new XlNumFmtDisplayLocale(localeCode, currency);
				}
				else
					return new XlNumFmtDisplayLocale(-1, localeString.Remove(0, 1));
			}
			return null;
		}
		XlNumFmtTimeBase TryParseElapsed(string elapsedString) {
			XlNumFmtTimeBase result = null;
			currentSymbol = char.ToLowerInvariant(elapsedString[0]);
			if(currentSymbol == 'h' || currentSymbol == 'm' || currentSymbol == 's') {
				int blockLength = GetDateTimeBlockLength(elapsedString, 0);
				if(blockLength == elapsedString.Length) {
					switch(currentSymbol) {
						case 's':
							result = new XlNumFmtSeconds(blockLength, true);
							break;
						case 'm':
							result = new XlNumFmtMinutes(blockLength, true);
							break;
						case 'h':
							result = new XlNumFmtHours(blockLength, true, false); 
							break;
					}
				}
			}
			return result;
		}
		XlNumFmtExprCondiiton TryParseExpr(string expression) {
			return new XlNumFmtExprCondiiton(expression);
		}
		int GetDateTimeBlockLength() {
			int elementLength = GetDateTimeBlockLength(formatString, currentIndex);
			currentIndex += elementLength - 1;
			return elementLength;
		}
		int GetDateTimeBlockLength(string formatString, int currentIndex) {
			int startIndex = currentIndex;
			currentSymbol = char.ToLowerInvariant(currentSymbol);
			for(; currentIndex < formatString.Length; ++currentIndex)
				if(char.ToLowerInvariant(formatString[currentIndex]) != currentSymbol) {
					--currentIndex;
					return currentIndex - startIndex + 1;
				}
			return formatString.Length - startIndex;
		}
		bool CheckIsGeneral() {
			string general = localizer.GeneralDesignator;
			if(formatString.Length < currentIndex + general.Length)
				return false;
			string posibleGeneral = formatString.Substring(currentIndex, general.Length);
			return string.Compare(general, posibleGeneral, StringComparison.OrdinalIgnoreCase) == 0;
		}
		#endregion
	}
	#endregion
	#region XlNumFmtLocalizer
	class XlNumFmtLocalizer {
		delegate string TableGenerator(Dictionary<char, XlNumFmtDesignator> designators);
		CultureInfo lastCulture;
		string generalDesignator;
		Dictionary<char, XlNumFmtDesignator> designators = new Dictionary<char, XlNumFmtDesignator>();
		Dictionary<XlNumFmtDesignator, char> chars = new Dictionary<XlNumFmtDesignator, char>();
		public CultureInfo Culture { get { return lastCulture; } }
		public Dictionary<char, XlNumFmtDesignator> Designators { get { return designators; } }
		public Dictionary<XlNumFmtDesignator, char> Chars { get { return chars; } }
		public string GeneralDesignator { get { return generalDesignator; } }
		public void SetCulture(CultureInfo culture) {
			if(lastCulture == culture)
				return;
			lastCulture = culture;
			designators.Clear();
			chars.Clear();
			GenerateDesignators(culture);
			foreach(char key in designators.Keys) {
				int count = chars.Count;
				XlNumFmtDesignator designator = designators[key];
				if((designator & XlNumFmtDesignator.AmPm) > 0)
					chars.Add(XlNumFmtDesignator.AmPm, key);
				if((designator & XlNumFmtDesignator.Year) > 0)
					chars.Add(XlNumFmtDesignator.Year, key);
				if((designator & XlNumFmtDesignator.InvariantYear) > 0)
					chars.Add(XlNumFmtDesignator.InvariantYear, key);
				if((designator & XlNumFmtDesignator.Month) > 0)
					chars.Add(XlNumFmtDesignator.Month, key);
				if((designator & XlNumFmtDesignator.Minute) > 0)
					chars.Add(XlNumFmtDesignator.Minute, key);
				if((designator & XlNumFmtDesignator.DateSeparator) > 0)
					chars.Add(XlNumFmtDesignator.DateSeparator, key);
				if((designator & XlNumFmtDesignator.DecimalSeparator) > 0)
					chars.Add(XlNumFmtDesignator.DecimalSeparator, key);
				if((designator & XlNumFmtDesignator.FractionOrDateSeparator) > 0)
					chars.Add(XlNumFmtDesignator.FractionOrDateSeparator, key);
				if((designator & XlNumFmtDesignator.GroupSeparator) > 0)
					chars.Add(XlNumFmtDesignator.GroupSeparator, key);
				if((designator & XlNumFmtDesignator.TimeSeparator) > 0)
					chars.Add(XlNumFmtDesignator.TimeSeparator, key);
				if((designator & XlNumFmtDesignator.Day) > 0)
					chars.Add(XlNumFmtDesignator.Day, key);
				if((designator & XlNumFmtDesignator.Second) > 0)
					chars.Add(XlNumFmtDesignator.Second, key);
				if((designator & XlNumFmtDesignator.General) > 0)
					chars.Add(XlNumFmtDesignator.General, key);
				if((designator & XlNumFmtDesignator.JapaneseEra) > 0)
					chars.Add(XlNumFmtDesignator.JapaneseEra, key);
				if((designator & XlNumFmtDesignator.DayOfWeek) > 0)
					chars.Add(XlNumFmtDesignator.DayOfWeek, key);
				if((designator & XlNumFmtDesignator.Hour) > 0)
					chars.Add(XlNumFmtDesignator.Hour, key);
				if(chars.Count == count)
					chars.Add(designator, key);
			}
		}
		void GenerateDesignators(CultureInfo culture) {
			char decimalSeparator = culture.NumberFormat.NumberDecimalSeparator[0];
			char dateSeparator = culture.GetDateSeparator()[0];
			char groupSeparator = culture.NumberFormat.NumberGroupSeparator[0];
			char timeSeparator = culture.GetTimeSeparator()[0];
			if(char.IsWhiteSpace(groupSeparator))
				groupSeparator = ' ';
			AddSeparator(decimalSeparator, XlNumFmtDesignator.DecimalSeparator);
			AddSeparator(dateSeparator, XlNumFmtDesignator.DateSeparator);
			AddSeparator(groupSeparator, XlNumFmtDesignator.GroupSeparator);
			AddSeparator(timeSeparator, XlNumFmtDesignator.TimeSeparator);
			AddSeparator('/', XlNumFmtDesignator.FractionOrDateSeparator);
			designators.Add('a', XlNumFmtDesignator.AmPm);
			designators.Add('*', XlNumFmtDesignator.Asterisk);
			designators.Add('@', XlNumFmtDesignator.At);
			designators.Add('\\', XlNumFmtDesignator.Backslash);
			designators.Add('[', XlNumFmtDesignator.Bracket);
			designators.Add('#', XlNumFmtDesignator.DigitEmpty);
			designators.Add('?', XlNumFmtDesignator.DigitSpace);
			designators.Add('0', XlNumFmtDesignator.DigitZero);
			designators.Add(';', XlNumFmtDesignator.EndOfPart);
			designators.Add('E', XlNumFmtDesignator.Exponent);
			designators.Add('e', XlNumFmtDesignator.InvariantYear);
			designators.Add('%', XlNumFmtDesignator.Percent);
			designators.Add('"', XlNumFmtDesignator.Quote);
			designators.Add('b', XlNumFmtDesignator.ThaiYear);
			designators.Add('_', XlNumFmtDesignator.Underline);
			generalDesignator = GenerateInvariant(designators);
		}
		void AddSeparator(char symbol, XlNumFmtDesignator designator) {
			if(designators.ContainsKey(symbol))
				designators[symbol] |= designator;
			else
				designators.Add(symbol, designator);
		}
		static string GenerateInvariant(Dictionary<char, XlNumFmtDesignator> designators) {
			designators['a'] |= XlNumFmtDesignator.DayOfWeek;
			designators.Add('d', XlNumFmtDesignator.Day);
			designators.Add('h', XlNumFmtDesignator.Hour);
			designators.Add('m', XlNumFmtDesignator.Minute | XlNumFmtDesignator.Month);
			designators.Add('s', XlNumFmtDesignator.Second);
			designators.Add('y', XlNumFmtDesignator.Year);
			designators.Add('g', XlNumFmtDesignator.JapaneseEra | XlNumFmtDesignator.General);
			return "General";
		}
	}
	#endregion
	#region DateTime elements
	class XlNumFmtDefaultDateSeparator : XlNumFmtDecimalSeparator {
		public new static XlNumFmtDefaultDateSeparator Instance = new XlNumFmtDefaultDateSeparator();
		protected XlNumFmtDefaultDateSeparator() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.FractionOrDateSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return "/";
		}
	}
	class XlNumFmtDateSeparator : XlNumFmtDecimalSeparator {
		public new static XlNumFmtDateSeparator Instance = new XlNumFmtDateSeparator();
		protected XlNumFmtDateSeparator() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DateSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.GetDateSeparator();
		}
	}
	class XlNumFmtTimeSeparator : XlNumFmtDateSeparator {
		public new static XlNumFmtTimeSeparator Instance = new XlNumFmtTimeSeparator();
		protected XlNumFmtTimeSeparator() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.TimeSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.GetTimeSeparator();
		}
	}
	abstract class XlNumFmtDateBase : XlNumFmtElementBase {
		int count;
		protected XlNumFmtDateBase(int count) {
			this.count = count;
		}
		public int Count { get { return count; } }
		protected bool IsDefaultNetDateTimeFormat { get { return count == 1; } }
	}
	class XlNumFmtJapaneseEra : XlNumFmtDateBase {
		public XlNumFmtJapaneseEra(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.JapaneseEra; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
		}
	}
	class XlNumFmtYear : XlNumFmtDateBase {
		public XlNumFmtYear(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Year; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = GetDateTime(value, culture);
			string year = dateTime.Year.ToString();
			result.Text += year.Substring(year.Length - Count, Count);
		}
		protected virtual DateTime GetDateTime(XlVariantValue value, CultureInfo culture) {
			return value.GetDateTimeForMonthName();
		}
	}
	class XlNumFmtInvariantYear : XlNumFmtYear {
		public XlNumFmtInvariantYear(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.InvariantYear; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = GetDateTime(value, culture);
			result.Text += dateTime.Year.ToString();
		}
	}
	class XlNumFmtThaiYear : XlNumFmtYear {
		public XlNumFmtThaiYear(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.ThaiYear; } }
		protected override DateTime GetDateTime(XlVariantValue value, CultureInfo culture) {
			DateTime result = base.GetDateTime(value, culture);
			return result.AddYears(543);
		}
	}
	class XlNumFmtMonth : XlNumFmtDateBase {
		public XlNumFmtMonth(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Month; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = value.GetDateTimeForMonthName();
			string month;
			if(IsDefaultNetDateTimeFormat)
				month = dateTime.ToString("%M", culture);
			else {
				month = dateTime.ToString(new string('M', Count), culture);
				if(Count == 5)
					month = month.Substring(0, 1);
			}
			result.Text += month;
		}
	}
	class XlNumFmtDayOfWeek : XlNumFmtDateBase {
		public XlNumFmtDayOfWeek(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DayOfWeek; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = value.GetDateTimeForMonthName();
			string dayOfWeek = dateTime.ToString(new string('d', Count), culture);
			result.Text += dayOfWeek;
		}
	}
	class XlNumFmtDay : XlNumFmtDateBase {
		public XlNumFmtDay(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Day; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime;
			if(Count <= 2) {
				double numericValue = value.NumericValue;
				if(numericValue < 1) {
					result.Text += new string('0', Count);
					return;
				}
				else if((int)numericValue == 60) {
					result.Text += "29";
					return;
				}
				dateTime = value.DateTimeValue;
			}
			else
				dateTime = value.GetDateTimeForDayOfWeek();
			string day;
			if(IsDefaultNetDateTimeFormat)
				day = dateTime.ToString("%d", culture);
			else
				day = dateTime.ToString(new string('d', Count), culture);
			result.Text += day;
		}
	}
	class XlNumFmtAmPm : XlNumFmtDateBase {
		bool amIsLower;
		bool pmIsLower;
		public XlNumFmtAmPm()
			: base(2) {
		}
		public XlNumFmtAmPm(bool amIsLower, bool pmIsLower)
			: base(1) {
			this.amIsLower = amIsLower;
			this.pmIsLower = pmIsLower;
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.AmPm; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			double numericValue = value.NumericValue;
			numericValue = numericValue - Math.Floor(numericValue);
			result.Text += GetDesignator(numericValue < 0.5);
		}
		string GetDesignator(bool isAM) { 
			if(isAM)
				if(IsDefaultNetDateTimeFormat)
					return amIsLower ? "a" : "A";
				else
					return amIsLower ? "am" : "AM";
			else
				if(IsDefaultNetDateTimeFormat)
					return pmIsLower ? "p" : "P";
				else
					return pmIsLower ? "pm" : "PM";
		}
	}
	class XlNumFmtMilliseconds : XlNumFmtDateBase {
		public XlNumFmtMilliseconds(int count)
			: base(count) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DigitZero; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = value.DateTimeValue;
			double millisecond = Math.Round((double)dateTime.Millisecond / 1000, Count, MidpointRounding.AwayFromZero);
			result.Text += millisecond.ToString('.' + new string('0', Count), culture);
		}
	}
	abstract class XlNumFmtTimeBase : XlNumFmtDateBase {
		bool elapsed;
		public XlNumFmtTimeBase(int count, bool elapsed)
			: base(count) {
			this.elapsed = elapsed;
		}
		protected bool Elapsed { get { return elapsed; } }
	}
	class XlNumFmtHours : XlNumFmtTimeBase {
		bool is12HourTime;
		public XlNumFmtHours(int count, bool elapsed, bool is12HourTime)
			: base(count, elapsed) {
			this.is12HourTime = is12HourTime;
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Hour; } }
		public bool Is12HourTime { get { return is12HourTime; } set { is12HourTime = value; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			long hour = Elapsed ? (long)TimeSpan.FromDays(value.NumericValue).TotalHours : value.DateTimeValue.Hour;
			if(Is12HourTime) {
				hour = hour % 12;
				if(hour == 0)
					hour = 12;
			}
			result.Text += hour.ToString(new string('0', Count), culture);
		}
	}
	class XlNumFmtMinutes : XlNumFmtTimeBase {
		public XlNumFmtMinutes(int count, bool elapsed)
			: base(count, elapsed) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Minute; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			string actualString;
			if(Elapsed) {
				long actualValue = (long)(value.NumericValue * 24 * 60);
				actualString = actualValue.ToString(new string('0', Count), culture);
			}
			else {
				DateTime dateTime = value.DateTimeValue;
				if(IsDefaultNetDateTimeFormat)
					actualString = dateTime.ToString(string.Format("%{0}", 'm'), culture);
				else
					actualString = dateTime.ToString(new string('m', 2), culture);
			}
			result.Text += actualString;
		}
	}
	class XlNumFmtSeconds : XlNumFmtTimeBase {
		public XlNumFmtSeconds(int count, bool elapsed)
			: base(count, elapsed) {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Second; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			DateTime dateTime = value.DateTimeValue;
			long second = Elapsed ? (long)(value.NumericValue * 24 * 60 * 60) : dateTime.Second;
			result.Text += second.ToString(new string('0', Count), culture);
		}
	}
	#endregion
	#region Numeric elements
	class XlNumFmtDigitZero : IXlNumFmtElement {
		public static XlNumFmtDigitZero Instance = new XlNumFmtDigitZero();
		protected XlNumFmtDigitZero() {
		}
		public bool IsDigit { get { return true; } }
		public virtual char NonLocalizedDesignator { get { return '0'; } }
		public virtual XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DigitZero; } }
		public void Format(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += value.NumericValue;
		}
		public virtual void FormatEmpty(CultureInfo culture, XlNumFmtResult result) {
			result.Text += NonLocalizedDesignator;
		}
	}
	class XlNumFmtDigitEmpty : XlNumFmtDigitZero {
		public new static XlNumFmtDigitEmpty Instance = new XlNumFmtDigitEmpty();
		protected XlNumFmtDigitEmpty() {
		}
		public override char NonLocalizedDesignator { get { return '#'; } }
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DigitEmpty; } }
		public override void FormatEmpty(CultureInfo culture, XlNumFmtResult result) {
		}
	}
	class XlNumFmtDigitSpace : XlNumFmtDigitZero {
		public new static XlNumFmtDigitSpace Instance = new XlNumFmtDigitSpace();
		protected XlNumFmtDigitSpace() {
		}
		public override char NonLocalizedDesignator { get { return '?'; } }
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DigitSpace; } }
		public override void FormatEmpty(CultureInfo culture, XlNumFmtResult result) {
			result.Text += ' ';
		}
	}
	class XlNumFmtDecimalSeparator : IXlNumFmtElement {
		public static XlNumFmtDecimalSeparator Instance = new XlNumFmtDecimalSeparator();
		protected XlNumFmtDecimalSeparator() {
		}
		public bool IsDigit { get { return false; } }
		public virtual XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.DecimalSeparator; } }
		public void Format(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			FormatEmpty(culture, result);
		}
		public void FormatEmpty(CultureInfo culture, XlNumFmtResult result) {
			result.Text += GetDesignator(culture);
		}
		protected virtual string GetDesignator(CultureInfo culture) {
			return culture.NumberFormat.NumberDecimalSeparator;
		}
	}
	class XlNumFmtGroupSeparator : XlNumFmtDecimalSeparator {
		public new static XlNumFmtGroupSeparator Instance = new XlNumFmtGroupSeparator();
		protected XlNumFmtGroupSeparator() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.GroupSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.NumberFormat.NumberGroupSeparator.Replace('\x00a0', ' ');
		}
	}
	class XlNumFmtPercent : XlNumFmtElementBase {
		public static XlNumFmtPercent Instance = new XlNumFmtPercent();
		XlNumFmtPercent() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Percent; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += '%';
		}
	}
	#endregion
	#region Text elements
	class XlNumFmtTextContent : XlNumFmtElementBase {
		public static XlNumFmtTextContent Instance = new XlNumFmtTextContent();
		XlNumFmtTextContent() {
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.At; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += value.ToText(culture).TextValue;
		}
	}
	#endregion
	#region Common elements
	abstract class XlNumFmtElementBase : IXlNumFmtElement {
		public bool IsDigit { get { return false; } }
		public abstract XlNumFmtDesignator Designator { get; }
		public void Format(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			FormatCore(value, culture, result);
		}
		public void FormatEmpty(CultureInfo culture, XlNumFmtResult result) {
			FormatCore(XlVariantValue.Empty, culture, result);
		}
		protected abstract void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result);
	}
	class XlNumFmtUnderline : XlNumFmtElementBase {
		char symbol;
		public XlNumFmtUnderline(char symbol) {
			this.symbol = symbol;
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Underline; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += ' ';
		}
	}
	class XlNumFmtAsterisk : XlNumFmtElementBase {
		char symbol;
		public XlNumFmtAsterisk(char symbol) {
			this.symbol = symbol;
		}
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Asterisk; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += symbol;
		}
	}
	class XlNumFmtQuotedText : XlNumFmtElementBase {
		string text;
		public XlNumFmtQuotedText(string text) {
			this.text = text;
		}
		public string Text { get { return text; } set { text = value; } }
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Quote; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += text;
		}
	}
	class XlNumFmtBackslashedText : XlNumFmtElementBase {
		char text;
		public XlNumFmtBackslashedText(char text) {
			this.text = text;
		}
		public char Text { get { return text; } set { text = value; } }
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Backslash; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += text;
		}
	}
	abstract class XlNumFmtCondition : XlNumFmtElementBase {
		public override XlNumFmtDesignator Designator { get { return XlNumFmtDesignator.Bracket; } }
	}
	class XlNumFmtColor : XlNumFmtCondition {
		Color color;
		public XlNumFmtColor(Color color) {
			this.color = color;
		}
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
		}
	}
	class XlNumFmtDisplayLocale : XlNumFmtCondition {
		int hexCode;
		string currency;
		public XlNumFmtDisplayLocale(int code, string currency) {
			this.hexCode = code;
			this.currency = currency;
		}
		public int HexCode { get { return hexCode; } }
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += currency;
		}
	}
	class XlNumFmtExprCondiiton : XlNumFmtCondition {
		string expression;
		public XlNumFmtExprCondiiton(string expression) {
			this.expression = expression;
		}
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += '[' + expression + ']';
		}
	}
	class XlNumFmtNotImplementedLocale : XlNumFmtCondition {
		int locale;
		public XlNumFmtNotImplementedLocale(int locale) {
			this.locale = locale;
		}
		protected override void FormatCore(XlVariantValue value, CultureInfo culture, XlNumFmtResult result) {
			result.Text += 'B' + locale.ToString(culture);
		}
	}
	#endregion
	#endregion
	public partial class CsvDataAwareExporter {
		readonly XlFormatterFactory formatFactory = new XlFormatterFactory();
		string FormatValue(XlNumberFormat numberFormat, XlVariantValue value) {
			IXlValueFormatter formatter = formatFactory.CreateFormatter(numberFormat);
			return formatter.Format(value, CurrentCulture);
		}
	}
}
