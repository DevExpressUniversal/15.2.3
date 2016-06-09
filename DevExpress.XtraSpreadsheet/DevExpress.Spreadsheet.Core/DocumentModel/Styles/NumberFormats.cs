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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.Globalization;
using System.Text;
using DevExpress.XtraSpreadsheet.Localization;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region NumberFormatHelper
	public static class NumberFormatHelper {
		public static int GetNumberFormatIndex(string formatString, DocumentModel documentModel) {
			NumberFormat numberFormat = NumberFormatParser.Parse(formatString);
			if (numberFormat == null)
				throw new InvalidNumberFormatException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidNumberFormat), formatString);
			return documentModel.Cache.NumberFormatCache.GetItemIndex(numberFormat);
		}
	}
	#endregion
	#region NumberFormatResult
	public class NumberFormatResult {
		string text;
		Color color;
		bool isError;
		public string Text { get { return text; } set { text = value; } }
		public Color Color { get { return color; } set { color = value; } }
		public bool IsError { get { return isError; } set { isError = value; } }
	}
	#endregion
	#region NumberFormatParameters
	public class NumberFormatParameters {
		static readonly NumberFormatParameters empty = new NumberFormatParameters();
		public static NumberFormatParameters Empty { get { return empty; } }
		public int AvailableSpaceWidth { get; set; }
		public INumberFormatStringMeasurer Measurer { get; set; }
	}
	#endregion
	#region INumberFormatStringMeasurer
	public interface INumberFormatStringMeasurer {
		int MeasureStringWidth(string text);
		float MaxDigitWidth { get; }
	}
	#endregion
	#region NumberFormat
	public class NumberFormat : ICloneable<NumberFormat>, ISupportsCopyFrom<NumberFormat>, ISupportsSizeOf {
		public static NumberFormat Generic = new NumberFormat(GenericNumberFormat.Instance);
		INumberFormat format;
		internal NumberFormat(INumberFormat format) {
			this.format = format;
		}
		public NumberFormat(string formatCode) {
			format = NumberFormatParser.Parse(formatCode).format;
		}
		public NumberFormat(int unnecessaryId, string formatCode)
			: this(formatCode) {
		}
		#region Properties
		protected internal INumberFormat InnerFormat { get { return format; } set { format = value; } }
		public string FormatCode { get { return GetFormatCode(CultureInfo.InvariantCulture); } }
		public NumberFormatType Type { get { return format.Type; } }
		public bool IsGeneral { get { return Type == NumberFormatType.General; } }
		public bool IsDateTime { get { return Type == NumberFormatType.DateTime; } }
		public bool IsNumeric { get { return Type == NumberFormatType.Numeric; } }
		public bool IsText { get { return Type == NumberFormatType.Text; } }
		public bool IsGeneric { get { return format is GenericNumberFormat; } }
		protected internal bool IsTime { get { return format.IsTimeFormat; } }
		#endregion
		public string GetFormatCode(CultureInfo culture) {
			NumberFormatParser.Localizer.SetCulture(culture); 
			StringBuilder builder = new StringBuilder();
			format.AppendFormat(builder);
			return builder.ToString();
		}
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context) {
			return Format(value, context, NumberFormatParameters.Empty);
		}
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			return format.Format(value, context, parameters);
		}
		public VariantValue Round(VariantValue value, WorkbookDataContext context) {
			return format.Round(value, context);
		}
		#region Increase/Decrease Decimal
		public static string IncreaseDecimal(string formatCode) {
			NumberFormat format;
			if (string.IsNullOrEmpty(formatCode)) {
				NumericNumberFormatSimple part = new NumericNumberFormatSimple(0, 1, 1, 0, 1, false, false);
				part.Add(NumberFormatDigitZero.Instance);
				part.Add(NumberFormatDecimalSeparator.Instance);
				part.Add(NumberFormatDigitZero.Instance);
				format = new NumberFormat(part);
				return format.FormatCode;
			}
			else
				format = NumberFormatParser.Parse(formatCode);
			format.format.IncreaseDecimal();
			return format.FormatCode;
		}
		public static string DecreaseDecimal(string formatCode) {
			NumberFormat format = NumberFormatParser.Parse(formatCode);
			format.format.DecreaseDecimal();
			return format.FormatCode;
		}
		#endregion
		public bool HasColorForPositiveOrNegative() {
			return format.HasColorForPositiveOrNegative();
		}
		public bool EnclosedInParenthesesForPositive() {
			return format.EnclosedInParenthesesForPositive();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormat other = obj as NumberFormat;
			if (object.ReferenceEquals(other, null))
				return false;
			return format.Equals(other.format);
		}
		public static bool operator ==(NumberFormat left, NumberFormat right) {
			if (object.ReferenceEquals(left, right))
				return true;
			if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
				return false;
			return left.format.Equals(right.format);
		}
		public static bool operator !=(NumberFormat left, NumberFormat right) {
			return !(left == right);
		}
		public void CopyFrom(NumberFormat value) {
			format = value.format.Clone();
		}
		public NumberFormat Clone() {
			return new NumberFormat(format.Clone());
		}
		int ISupportsSizeOf.SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
	}
	#endregion
	#region NumberFormatType
	[Flags]
	public enum NumberFormatType {
		DateTime = 1,
		General = 2,
		Numeric = 4,
		Text = 8
	}
	#endregion
	#region INumberFormat
	public interface INumberFormat : ICloneable<INumberFormat>, ISupportsCopyFrom<INumberFormat> {
		NumberFormatType Type { get; }
		void AppendFormat(StringBuilder builder);
		NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters);
		VariantValue Round(VariantValue value, WorkbookDataContext context);
		void IncreaseDecimal();
		void DecreaseDecimal();
		bool HasColorForPositiveOrNegative();
		bool EnclosedInParenthesesForPositive();
		bool IsTimeFormat { get; }
	}
	#endregion
	#region INumberFormatElement
	public interface INumberFormatElement : ICloneable<INumberFormatElement> {
		bool IsDigit { get; }
		NumberFormatDesignator Designator { get; }
		void AppendFormat(StringBuilder builder);
		void Format(VariantValue value, WorkbookDataContext context, NumberFormatResult result);
		void FormatEmpty(WorkbookDataContext context, NumberFormatResult result);
	}
	#endregion
	#region SimpleNumberFormat
	public abstract class SimpleNumberFormat : List<INumberFormatElement>, INumberFormat, ICloneable<SimpleNumberFormat> {
		protected SimpleNumberFormat() {
		}
		protected SimpleNumberFormat(IEnumerable<INumberFormatElement> elements) {
			this.AddRange(elements);
		}
		public abstract NumberFormatType Type { get; }
		public bool IsTimeFormat {
			get {
				if (Type != NumberFormatType.DateTime)
					return false;
				foreach (INumberFormatElement item in this) {
					if (item.Designator == NumberFormatDesignator.Day ||
						item.Designator == NumberFormatDesignator.Month ||
						item.Designator == NumberFormatDesignator.Year ||
						item.Designator == NumberFormatDesignator.DayOfWeek ||
						item.Designator == NumberFormatDesignator.InvariantYear ||
						item.Designator == NumberFormatDesignator.JapaneseEra ||
						item.Designator == NumberFormatDesignator.ThaiYear)
						return false;
				}
				return true;
			}
		}
		public virtual void AppendFormat(StringBuilder builder) {
			foreach (INumberFormatElement element in this)
				element.AppendFormat(builder);
		}
		public virtual NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			NumberFormatResult result;
			if (value.IsEmpty) {
				result = new NumberFormatResult();
				result.Text = string.Empty;
				return result;
			}
			try {
				result = FormatCore(value, context);
				if (result.IsError || ShouldShowNotEnoughSpaceText(value, result, parameters))
					result.Text = CalculateNotEnoughSpaceText(parameters);
			}
			catch {
				result = new NumberFormatResult();
				result.IsError = true;
				result.Text = CalculateNotEnoughSpaceText(parameters);
			}
			return result;
		}
		public abstract NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context);
		bool ShouldShowNotEnoughSpaceText(VariantValue value, NumberFormatResult result, NumberFormatParameters parameters) {
			if (parameters.Measurer == null)
				return false;
			if (value.IsText)
				return false;
			if (String.IsNullOrEmpty(result.Text))
				return false;
			if (result.Text.Length * parameters.Measurer.MaxDigitWidth * 3 <= parameters.AvailableSpaceWidth)
				return false;
			return parameters.Measurer.MeasureStringWidth(result.Text) > parameters.AvailableSpaceWidth;
		}
		string CalculateNotEnoughSpaceText(NumberFormatParameters parameters) {
			INumberFormatStringMeasurer measurer = parameters.Measurer;
			if (measurer == null)
				return "#";
			if (parameters.AvailableSpaceWidth >= Int32.MaxValue / 2)
				return "#";
			int availableSpaceWidth = parameters.AvailableSpaceWidth - (int)Math.Round(measurer.MaxDigitWidth / 2.0f);
			if (availableSpaceWidth > 32000)
				return "#";
			int count = Math.Max(1, (int)(availableSpaceWidth / measurer.MaxDigitWidth));
			for (int i = count; i >= 1; i--) {
				string result = new String('#', i);
				if (measurer.MeasureStringWidth(result) <= availableSpaceWidth)
					return result;
			}
			return "#";
		}
		public virtual VariantValue Round(VariantValue value, WorkbookDataContext context) {
			return value;
		}
		public virtual void IncreaseDecimal() { }
		public virtual void DecreaseDecimal() { }
		public bool HasColorForPositiveOrNegative() {
			return false;
		}
		public bool EnclosedInParenthesesForPositive() {
			bool opened = false;
			bool closed = false;
			INumberFormatElement element;
			for (int i = 0; i < Count; ++i) {
				element = this[i];
				NumberFormatQuotedText text = element as NumberFormatQuotedText;
				if (text != null)
					foreach (char c in text.Text) {
						if (c == '(')
							if (closed)
								return true;
							else
								opened = true;
						if (c == ')')
							if (opened)
								return true;
							else
								closed = true;
					}
				else {
					NumberFormatBackslashedText bText = element as NumberFormatBackslashedText;
					if (bText != null) {
						if (bText.Text == '(')
							if (closed)
								return true;
							else
								opened = true;
						if (bText.Text == ')')
							if (opened)
								return true;
							else
								closed = true;
					}
				}
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			SimpleNumberFormat other = obj as SimpleNumberFormat;
			if (other == null)
				return false;
			if (Type != other.Type || Count != other.Count)
				return false;
			for (int i = 0; i < Count; ++i)
				if (!this[i].Equals(other[i]))
					return false;
			return true;
		}
		public virtual void CopyFrom(INumberFormat value) {
			SimpleNumberFormat format = value as SimpleNumberFormat;
			this.Clear();
			for (int i = 0; i < format.Count; ++i)
				this.Add(format[i].Clone());
		}
		INumberFormat ICloneable<INumberFormat>.Clone() {
			return Clone();
		}
		public abstract SimpleNumberFormat Clone();
	}
	#endregion
	#region ConditionalNumberFormat
	public class ConditionalNumberFormat : List<SimpleNumberFormat>, INumberFormat {
		public NumberFormatType Type { get { return this[0].Type; } }
		public bool IsTimeFormat { get { return this[0].IsTimeFormat; } }
		public void AppendFormat(StringBuilder builder) {
			foreach (SimpleNumberFormat format in this) {
				format.AppendFormat(builder);
				builder.Append(';');
			}
			if (builder.Length > 0)
				builder.Remove(builder.Length - 1, 1);
		}
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			SimpleNumberFormat actualProvider = CalculateActualPart(value);
			if (actualProvider == null) {
				NumberFormatResult result = new NumberFormatResult();
				result.Text = value.ToText(context).InlineTextValue;
				return result;
			}
			return actualProvider.Format(value, context, parameters);
		}
		public VariantValue Round(VariantValue value, WorkbookDataContext context) {
			SimpleNumberFormat actualPart = CalculateActualPart(value);
			return actualPart == null ? value : actualPart.Round(value, context);
		}
		SimpleNumberFormat CalculateActualPart(VariantValue value) {
			int count = Count;
			if (count == 0)
				return null;
			if (!value.IsNumeric) {
				if (count == 4)
					return this[3];
				SimpleNumberFormat part = this[count - 1];
				if (part.Type == NumberFormatType.Text)
					return part;
				return null;
			}
			if (this[count - 1].Type == NumberFormatType.Text)
				--count;
			if (count <= 1)
				return this[0];
			if (count > 2 && value.NumericValue == 0)
				return this[2];
			return value.NumericValue >= 0 ? this[0] : this[1];
		}
		public void IncreaseDecimal() {
			if (Count > 0)
				this[0].IncreaseDecimal();
			if (Count > 1)
				this[1].IncreaseDecimal();
		}
		public void DecreaseDecimal() {
			if (Count > 0)
				this[0].DecreaseDecimal();
			if (Count > 1)
				this[1].DecreaseDecimal();
		}
		public bool HasColorForPositiveOrNegative() {
			if (Count < 2)
				return false;
			int count = Count;
			SimpleNumberFormat part = this[count - 1];
			if (part.Type == NumberFormatType.Text)
				--count;
			if (count == 2 || count == 3)
				return HasColor(this[0]) || HasColor(this[1]);
			return false;
		}
		bool HasColor(SimpleNumberFormat part) {
			if (part.Count < 1)
				return false;
			return part[0] is NumberFormatColor;
		}
		public bool EnclosedInParenthesesForPositive() {
			if (Count < 1)
				return false;
			SimpleNumberFormat part = this[0];
			return part.EnclosedInParenthesesForPositive();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!object.ReferenceEquals(this, obj)) {
				ConditionalNumberFormat format = obj as ConditionalNumberFormat;
				if (format == null || Count != format.Count)
					return false;
				for (int i = 0; i < Count; ++i)
					if (!this[i].Equals(format[i]))
						return false;
			}
			return true;
		}
		public void CopyFrom(INumberFormat value) {
			ConditionalNumberFormat format = value as ConditionalNumberFormat;
			this.Clear();
			for (int i = 0; i < format.Count; ++i)
				Add(format[i].Clone());
		}
		public INumberFormat Clone() {
			ConditionalNumberFormat clone = new ConditionalNumberFormat();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region CultureDependentDateTimeNumberFormat
	public abstract class CultureDependentDateTimeNumberFormat : INumberFormat {
		readonly static List<char> delimiterChars = new List<char>(new char[] { '-', '.', '/', ' ' });
		[ThreadStatic]
		public static bool SupressAreSameChecks;
		Dictionary<CultureInfo, NumberFormat> formatTable = new Dictionary<CultureInfo, NumberFormat>();
		protected CultureDependentDateTimeNumberFormat() {
		}
		public NumberFormatType Type { get { return NumberFormatType.DateTime; } }
		public virtual bool IsTimeFormat { get { return false; } }
		public bool AreSame(SimpleNumberFormat value, CultureInfo culture) {
			if (SupressAreSameChecks)
				return false;
			NumberFormat actualFormat = GetActualFormat(culture);
			SimpleNumberFormat format = actualFormat.InnerFormat as SimpleNumberFormat;
			if (format.Count != value.Count)
				return false;
			for (int i = 0; i < format.Count; ++i)
				if (!format[i].Equals(value[i]))
					return false;
			return true;
		}
		public void AppendFormat(StringBuilder builder) {
			NumberFormat actualFormat = GetActualFormat(NumberFormatParser.Localizer.Culture);
			actualFormat.InnerFormat.AppendFormat(builder);
		}
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			NumberFormat actualFormat = GetActualFormat(context.Culture);
			return actualFormat.Format(value, context, parameters);
		}
		public NumberFormat GetActualFormat(CultureInfo culture) {
			NumberFormat format;
			if (!formatTable.TryGetValue(culture, out format)) {
				SupressAreSameChecks = true;
				format = CreateNumberFormat(culture);
				NumberFormatParser.Localizer.SetCulture(culture);
				formatTable.Add(culture, format);
				SupressAreSameChecks = false;
			}
			return format;
		}
		public VariantValue Round(VariantValue value, WorkbookDataContext context) {
			return value;
		}
		public void IncreaseDecimal() {
		}
		public void DecreaseDecimal() {
		}
		public bool HasColorForPositiveOrNegative() {
			return false;
		}
		public bool EnclosedInParenthesesForPositive() {
			return false;
		}
		protected static int SkipDelimitersAtLeft(string pattern, int index) {
			return SkipCharsAtLeft(pattern, index, delimiterChars);
		}
		protected static int SkipDelimitersAtRight(string pattern, int index) {
			return SkipCharsAtRight(pattern, index, delimiterChars);
		}
		protected static int SkipCharsAtLeft(string pattern, int index, List<char> chars) {
			for (int i = index + 1; i < pattern.Length; i++)
				if (!chars.Contains(pattern[i]))
					return i;
			return index;
		}
		protected static int SkipCharsAtRight(string pattern, int index, List<char> chars) {
			for (int i = index - 1; i >= 0; i--)
				if (!chars.Contains(pattern[i]))
					return i;
			return index;
		}
		protected static char FindDelimiterCharFromRight(string pattern) {
			for (int i = pattern.Length - 1; i >= 0; i--)
				if (delimiterChars.Contains(pattern[i]))
					return pattern[i];
			return '\0';
		}
		protected abstract NumberFormat CreateNumberFormat(CultureInfo culture);
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public void CopyFrom(INumberFormat value) {
		}
		public INumberFormat Clone() {
			return this;
		}
	}
	#endregion
	#region DayMonthYearNumberFormat
	public class DayMonthYearNumberFormat : DayMonthNumberFormat {
		[ThreadStatic]
		static DayMonthYearNumberFormat instance;
		public new static DayMonthYearNumberFormat Instance {
			get {
				if (instance == null)
					instance = new DayMonthYearNumberFormat();
				return instance;
			}
		}
		DayMonthYearNumberFormat() {
		}
		protected override NumberFormat CreateNumberFormat(CultureInfo culture) {
			NumberFormat format = base.CreateNumberFormat(culture);
			DateTimeNumberFormat part = format.InnerFormat as DateTimeNumberFormat;
			INumberFormatElement separator = null;
			for (int i = 0; i < part.Count; ++i) {
				separator = part[i];
				if (!(separator is NumberFormatDateBase))
					break;
			}
			part.Add(separator.Clone());
			part.Add(new NumberFormatYear(2));
			return format;
		}
	}
	#endregion
	#region DayMonthNumberFormat
	public class DayMonthNumberFormat : CultureDependentDateTimeNumberFormat {
		readonly static List<char> yearChars = new List<char>(new char[] { 'y' });
		const string primaryFormatCode = "d-mmm";
		const string secondaryFormatCode = "dd-mmm";
		[ThreadStatic]
		static DayMonthNumberFormat instance;
		public static DayMonthNumberFormat Instance {
			get {
				if (instance == null)
					instance = new DayMonthNumberFormat();
				return instance;
			}
		}
		protected DayMonthNumberFormat() {
		}
		protected override NumberFormat CreateNumberFormat(CultureInfo culture) {
			string[] patterns = DateTimeParanoicParser.GetAllDateTimePatterns(culture);
			foreach (string pattern in patterns) {
				if ((pattern.StartsWithInvariantCultureIgnoreCase(secondaryFormatCode) && StringExtensions.CompareInvariantCultureIgnoreCase(StripYearAtRight(pattern), secondaryFormatCode) == 0))
					return new NumberFormat(secondaryFormatCode);
				if (pattern.StartsWithInvariantCultureIgnoreCase(primaryFormatCode) && StringExtensions.CompareInvariantCultureIgnoreCase(StripYearAtRight(pattern), primaryFormatCode) == 0)
					return new NumberFormat(primaryFormatCode);
			}
			string strippedShortDatePattern = StripYearAtRight(culture.DateTimeFormat.ShortDatePattern);
			char delimiter = FindDelimiterCharFromRight(strippedShortDatePattern);
			if (delimiter == '\0')
				return new NumberFormat(primaryFormatCode);
			if (strippedShortDatePattern.Contains("dd"))
				return new NumberFormat(secondaryFormatCode.Replace('-', delimiter));
			else
				return new NumberFormat(primaryFormatCode.Replace('-', delimiter));
		}
		public static string StripYearAtRight(string pattern) {
			int index = SkipYearAtRight(pattern, pattern.Length);
			if (index >= pattern.Length - 1)
				return pattern;
			int lastValidIndex = SkipDelimitersAtRight(pattern, index);
			if (lastValidIndex >= index - 1)
				return pattern.Substring(0, index);
			else
				return pattern.Substring(0, lastValidIndex);
		}
		protected static int SkipYearAtRight(string pattern, int index) {
			return SkipCharsAtRight(pattern, index, yearChars);
		}
	}
	#endregion
	#region MonthYearNumberFormat
	public class MonthYearNumberFormat : CultureDependentDateTimeNumberFormat {
		readonly static List<char> dayChars = new List<char>(new char[] { 'd' });
		const string primaryFormatCode = "mmm-yy";
		[ThreadStatic]
		static MonthYearNumberFormat instance;
		public static MonthYearNumberFormat Instance {
			get {
				if (instance == null)
					instance = new MonthYearNumberFormat();
				return instance;
			}
		}
		MonthYearNumberFormat() {
		}
		protected override NumberFormat CreateNumberFormat(CultureInfo culture) {
			string[] patterns = DateTimeParanoicParser.GetAllDateTimePatterns(culture);
			foreach (string pattern in patterns) {
				if (pattern.EndsWithInvariantCultureIgnoreCase(primaryFormatCode) && StringExtensions.CompareInvariantCultureIgnoreCase(StripDayAtLeft(pattern), primaryFormatCode) == 0)
					return new NumberFormat(primaryFormatCode);
			}
			char delimiterChar = DayMonthNumberFormat.FindDelimiterCharFromRight(culture.DateTimeFormat.LongDatePattern);
			if (delimiterChar == '\0')
				return new NumberFormat(primaryFormatCode);
			else
				return new NumberFormat(primaryFormatCode.Replace('-', delimiterChar));
		}
		public static string StripDayAtLeft(string pattern) {
			int index = SkipDayAtLeft(pattern, -1);
			if (index < 0)
				return pattern;
			int lastValidIndex = SkipDelimitersAtLeft(pattern, index - 1);
			if (lastValidIndex <= index)
				return pattern.Substring(index);
			else
				return pattern.Substring(lastValidIndex);
		}
		protected static int SkipDayAtLeft(string pattern, int index) {
			return SkipCharsAtLeft(pattern, index, dayChars);
		}
	}
	#endregion
	#region ShortDateNumberFormat
	public class ShortDateNumberFormat : CultureDependentDateTimeNumberFormat {
		[ThreadStatic]
		static ShortDateNumberFormat instance;
		public static ShortDateNumberFormat Instance {
			get {
				if (instance == null)
					instance = new ShortDateNumberFormat();
				return instance;
			}
		}
		protected override NumberFormat CreateNumberFormat(CultureInfo culture) {
			return new NumberFormat(culture.DateTimeFormat.ShortDatePattern);
		}
	}
	#endregion
	#region ShortTimeNumberFormat
	public class ShortTimeNumberFormat : CultureDependentDateTimeNumberFormat {
		[ThreadStatic]
		static ShortTimeNumberFormat instance;
		public static ShortTimeNumberFormat Instance {
			get {
				if (instance == null)
					instance = new ShortTimeNumberFormat();
				return instance;
			}
		}
		ShortTimeNumberFormat() {
		}
		public override bool IsTimeFormat { get { return true; } }
		protected override NumberFormat CreateNumberFormat(CultureInfo culture) {
			string pattern = culture.DateTimeFormat.ShortTimePattern;
			const string timeDesignator = " tt";
			if (pattern.EndsWith(timeDesignator))
				pattern = pattern.Substring(0, pattern.Length - timeDesignator.Length).Replace('h', 'H');
			return new NumberFormat(pattern);
		}
	}
	#endregion
	#region FullDateTimeNumberFormat
	public class FullDateTimeNumberFormat : INumberFormat {
		[ThreadStatic]
		static FullDateTimeNumberFormat instance;
		public static FullDateTimeNumberFormat Instance {
			get {
				if (instance == null)
					instance = new FullDateTimeNumberFormat();
				return instance;
			}
		}
		FullDateTimeNumberFormat() {
		}
		public NumberFormatType Type { get { return NumberFormatType.DateTime; } }
		public bool IsTimeFormat { get { return false; } }
		public bool AreSame(SimpleNumberFormat value, CultureInfo culture) {
			if (CultureDependentDateTimeNumberFormat.SupressAreSameChecks)
				return false;
			SimpleNumberFormat actualFormat1 = ShortDateNumberFormat.Instance.GetActualFormat(culture).InnerFormat as SimpleNumberFormat;
			SimpleNumberFormat actualFormat2 = ShortTimeNumberFormat.Instance.GetActualFormat(culture).InnerFormat as SimpleNumberFormat;
			if (actualFormat1.Count + actualFormat2.Count + 1 != value.Count)
				return false;
			int i = 0;
			for (; i < actualFormat1.Count; ++i)
				if (!actualFormat1[i].Equals(value[i]))
					return false;
			++i;
			for (; i < actualFormat2.Count; ++i)
				if (!actualFormat2[i + actualFormat1.Count + 1].Equals(value[i]))
					return false;
			return true;
		}
		public void AppendFormat(StringBuilder builder) {
			ShortDateNumberFormat.Instance.AppendFormat(builder);
			builder.Append(' ');
			ShortTimeNumberFormat.Instance.AppendFormat(builder);
		}
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			NumberFormatResult result1 = ShortDateNumberFormat.Instance.Format(value, context, parameters);
			if (result1.IsError)
				return result1;
			NumberFormatResult result2 = ShortTimeNumberFormat.Instance.Format(value, context, parameters);
			if (result1.Text.Length == result2.Text.Length) 
				return result1;
			result1.Text += " " + result2.Text;
			return result1;
		}
		public VariantValue Round(VariantValue value, WorkbookDataContext context) {
			return value;
		}
		public void IncreaseDecimal() {
		}
		public void DecreaseDecimal() {
		}
		public bool HasColorForPositiveOrNegative() {
			return false;
		}
		public bool EnclosedInParenthesesForPositive() {
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public void CopyFrom(INumberFormat value) {
		}
		public INumberFormat Clone() {
			return this;
		}
	}
	#endregion
	#region DateTimeNumberFormatBase
	public abstract class DateTimeNumberFormatBase : SimpleNumberFormat {
		public const int SystemLongDate = 0xF800;
		public const int SystemLongTime = 0xF400;
		protected DateTimeNumberFormatBase() {
		}
		protected DateTimeNumberFormatBase(IEnumerable<INumberFormatElement> elements)
			: base(elements) {
		}
		public override NumberFormatType Type { get { return NumberFormatType.DateTime; } }
		protected virtual bool HasMilliseconds { get { return false; } }
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			NumberFormatResult result = new NumberFormatResult();
			if (!value.IsNumeric) {
				result.Text = value.ToText(context).InlineTextValue;
				return result;
			}
			result.Text = string.Empty;
			if (WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue, context.DateSystem)) {
				result.IsError = true;
				return result;
			}
			if (!HasMilliseconds)
				value.NumericValue += TimeSpan.FromMilliseconds(500).TotalDays;
			FormatDateTime(value, context, result);
			return result;
		}
		protected virtual void FormatDateTime(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			foreach (INumberFormatElement element in this)
				element.Format(value, context, result);
		}
	}
	#endregion
	#region DateTimeNumberFormat
	public class DateTimeNumberFormat : DateTimeNumberFormatBase {
		bool hasMilliseconds;
		NumberFormatDisplayLocale locale;
		protected DateTimeNumberFormat() {
		}
		public DateTimeNumberFormat(IEnumerable<INumberFormatElement> elements, NumberFormatDisplayLocale locale, bool hasMilliseconds)
			: base(elements) {
			this.locale = locale;
			this.hasMilliseconds = hasMilliseconds;
		}
		protected override bool HasMilliseconds { get { return hasMilliseconds; } }
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			if (locale == null || locale.HexCode < 0)
				return base.FormatCore(value, context);
			CultureInfo culture = CalculateSpecificCulture(locale.HexCode); 
			context.PushCulture(culture);
			try {
				return base.FormatCore(value, context);
			}
			finally {
				context.PopCulture();
			}
		}
		CultureInfo CalculateSpecificCulture(int hexCode) {
			int languageId = hexCode & 0x0000FFFF; 
			return LanguageIdToCultureConverter.Convert(languageId);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			DateTimeNumberFormat other = obj as DateTimeNumberFormat;
			if (locale != other.locale || hasMilliseconds != other.hasMilliseconds)
				return false;
			return true;
		}
		public override void CopyFrom(INumberFormat value) {
			DateTimeNumberFormat format = value as DateTimeNumberFormat;
			locale = format.locale;
			hasMilliseconds = format.hasMilliseconds;
			base.CopyFrom(value);
		}
		public override SimpleNumberFormat Clone() {
			DateTimeNumberFormat clone = new DateTimeNumberFormat();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region DateTimeSystemLongDateNumberFormat
	public class DateTimeSystemLongDateNumberFormat : DateTimeNumberFormatBase {
		protected DateTimeSystemLongDateNumberFormat() {
		}
		public DateTimeSystemLongDateNumberFormat(IEnumerable<INumberFormatElement> elements)
			: base(elements) {
		}
		protected override void FormatDateTime(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			base.FormatDateTime(value, context, result); 
			result.Text = value.ToDateTime(context).ToString(context.Culture.DateTimeFormat.LongDatePattern, context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!(obj is DateTimeSystemLongDateNumberFormat))
				return false;
			return base.Equals(obj);
		}
		public override SimpleNumberFormat Clone() {
			DateTimeSystemLongDateNumberFormat clone = new DateTimeSystemLongDateNumberFormat();
			clone.AddRange(this);
			return clone;
		}
	}
	#endregion
	#region DateTimeSystemLongTimeNumberFormat
	public class DateTimeSystemLongTimeNumberFormat : DateTimeNumberFormatBase {
		protected DateTimeSystemLongTimeNumberFormat() {
		}
		public DateTimeSystemLongTimeNumberFormat(IEnumerable<INumberFormatElement> elements)
			: base(elements) {
		}
		protected override void FormatDateTime(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			base.FormatDateTime(value, context, result); 
			result.Text = value.ToDateTime(context).ToString(context.Culture.DateTimeFormat.LongTimePattern, context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!(obj is DateTimeSystemLongTimeNumberFormat))
				return false;
			return base.Equals(obj);
		}
		public override SimpleNumberFormat Clone() {
			DateTimeSystemLongTimeNumberFormat clone = new DateTimeSystemLongTimeNumberFormat();
			clone.AddRange(this);
			return clone;
		}
	}
	#endregion
	#region NumericNumberFormat
	public class NumericNumberFormatSimple : SimpleNumberFormat {
		bool isNegativePart;
		bool grouping;
		int percentCount;
		int integerCount;
		int decimalCount;
		int displayFactor;
		int decimalSeparatorIndex;
		protected NumericNumberFormatSimple() {
		}
		public NumericNumberFormatSimple(int percentCount, int integerCount, int decimalCount, int displayFactor, int decimalSeparatorIndex, bool grouping, bool isNegativePart) {
			this.percentCount = percentCount;
			this.integerCount = integerCount;
			this.decimalCount = decimalCount;
			this.displayFactor = displayFactor;
			this.decimalSeparatorIndex = decimalSeparatorIndex;
			this.grouping = grouping;
			this.isNegativePart = isNegativePart;
		}
		public NumericNumberFormatSimple(IEnumerable<INumberFormatElement> elements, int percentCount, int integerCount, int decimalCount, int displayFactor, int decimalSeparatorIndex, bool grouping, bool isNegativePart)
			: base(elements) {
			this.percentCount = percentCount;
			this.integerCount = integerCount;
			this.decimalCount = decimalCount;
			this.displayFactor = displayFactor;
			this.decimalSeparatorIndex = decimalSeparatorIndex;
			this.grouping = grouping;
			this.isNegativePart = isNegativePart;
		}
		public override NumberFormatType Type { get { return NumberFormatType.Numeric; } }
		protected bool IsNegativePart { get { return isNegativePart; } set { isNegativePart = value; } }
		protected bool Grouping { get { return grouping; } set { grouping = value; } }
		protected int DecimalSeparatorIndex { get { return decimalSeparatorIndex; } set { decimalSeparatorIndex = value; } }
		protected int PercentCount { get { return percentCount; } set { percentCount = value; } }
		protected int IntegerCount { get { return integerCount; } set { integerCount = value; } }
		protected int DecimalCount { get { return decimalCount; } set { decimalCount = value; } }
		protected int DisplayFactor { get { return displayFactor; } set { displayFactor = value; } }
		public override void AppendFormat(StringBuilder builder) {
			AppendSimple(builder, Count - 1);
		}
		protected void AppendSimple(StringBuilder builder, int endIndex) {
			int i = 0;
			INumberFormatElement element;
			int digitCount = this.integerCount;
			if (grouping)
				for (; digitCount > 0; ++i) {
					element = this[i];
					element.AppendFormat(builder);
					if (element.IsDigit) {
						--digitCount;
						if (digitCount % 3 == 0 && digitCount != 0)
							NumberFormatGroupSeparator.Instance.AppendFormat(builder);
					}
				}
			if (displayFactor > 0) {
				digitCount += this.decimalCount;
				for (; digitCount > 0; ++i) {
					element = this[i];
					element.AppendFormat(builder);
					if (element.IsDigit)
						--digitCount;
				}
				for (int j = displayFactor; j > 0; --j)
					NumberFormatGroupSeparator.Instance.AppendFormat(builder);
			}
			for (; i <= endIndex; ++i)
				this[i].AppendFormat(builder);
		}
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			NumberFormatResult result = new NumberFormatResult();
			if (!value.IsNumeric) {
				result.Text = value.ToText(context).InlineTextValue;
				return result;
			}
			result.Text = string.Empty;
			FormatSimple(value, context, result, Count - 1);
			return result;
		}
		protected void FormatSimple(VariantValue value, WorkbookDataContext context, NumberFormatResult result, int endIndex) {
			if (value.NumericValue == 0)
				for (int i = 0; i <= endIndex; ++i)
					this[i].FormatEmpty(context, result);
			else {
				double numericValue = Math.Abs(value.NumericValue);
				numericValue = double.Parse(numericValue.ToString()); 
				numericValue = Math.Round(numericValue * Math.Pow(100, percentCount) * Math.Pow(0.001, displayFactor), decimalCount, MidpointRounding.AwayFromZero);
				if (decimalSeparatorIndex < 0)
					FormatIntegerPart(numericValue, endIndex, 0, integerCount, grouping, context, result);
				else {
					FormatIntegerPart(Math.Truncate(numericValue), decimalSeparatorIndex - 1, 0, integerCount, grouping, context, result);
					NumberFormatDecimalSeparator.Instance.FormatEmpty(context, result);
					FormatDecimalPart(numericValue, decimalSeparatorIndex + 1, endIndex, decimalCount, context, result);
				}
			}
			if (!isNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, context.Culture);
		}
		protected void FormatIntegerPart(double numericValue, int startIndex, int endIndex, int digitsCount, bool grouping, WorkbookDataContext context, NumberFormatResult result) {
			NumberFormatResult resultT = new NumberFormatResult();
			int i = startIndex;
			if (numericValue > 0) {
				int digitsCountInQueue = digitsCount;
				INumberFormatElement element;
				int numIndex = 0;
				for (; numericValue > 0 && digitsCountInQueue > 0; --i) {
					element = this[i];
					if (element.IsDigit) {
						element.Format(numericValue % 10, context, resultT);
						numericValue = Math.Truncate(numericValue / 10);
						--digitsCountInQueue;
						if (grouping && numIndex % 3 == 0 && numIndex != 0)
							NumberFormatGroupSeparator.Instance.FormatEmpty(context, resultT);
						++numIndex;
					}
					else
						element.FormatEmpty(context, resultT);
					result.Text = resultT.Text + result.Text;
					resultT.Text = string.Empty;
				}
				if (digitsCount > 0 || decimalSeparatorIndex >= 0)
					while (numericValue > 0) {
						NumberFormatDigitZero.Instance.Format(numericValue % 10, context, resultT);
						numericValue = Math.Truncate(numericValue / 10);
						if (grouping && numIndex % 3 == 0 && numIndex != 0)
							NumberFormatGroupSeparator.Instance.FormatEmpty(context, resultT);
						++numIndex;
						result.Text = resultT.Text + result.Text;
						resultT.Text = string.Empty;
					}
			}
			for (; i >= endIndex; --i) {
				this[i].FormatEmpty(context, resultT);
				result.Text = resultT.Text + result.Text;
				resultT.Text = string.Empty;
			}
			result.IsError = result.IsError || resultT.IsError;
			if (resultT.Color != null)
				result.Color = resultT.Color;
		}
		protected void FormatDecimalPart(double numericValue, int startIndex, int endIndex, int digitsCount, WorkbookDataContext context, NumberFormatResult result) {
			int digit;
			INumberFormatElement element;
			decimal pow = (decimal)Math.Pow(10, digitsCount);
			decimal numericValueDecimal = (decimal)numericValue;
			numericValueDecimal = Math.Round((numericValueDecimal - Math.Truncate(numericValueDecimal)) * pow, 0, MidpointRounding.AwayFromZero);
			int i = startIndex;
			for (; numericValueDecimal > 0 && i < Count; ++i) {
				element = this[i];
				if (element.IsDigit) {
					pow /= 10;
					digit = (int)(numericValueDecimal / pow);
					element.Format(digit, context, result);
					numericValueDecimal -= digit * pow;
				}
				else
					element.FormatEmpty(context, result);
			}
			for (; i <= endIndex; ++i)
				this[i].FormatEmpty(context, result);
		}
		protected string ApplyNegativeSign(string text, CultureInfo culture) {
			return '-' + text;
		}
		public override VariantValue Round(VariantValue value, WorkbookDataContext context) {
			if (!value.IsNumeric)
				return value;
			return Math.Round(value.NumericValue, decimalCount + percentCount * 2 + displayFactor * 3, MidpointRounding.AwayFromZero);
		}
		public override void IncreaseDecimal() {
			IncreaseDecimal(Count - 1);
		}
		protected void IncreaseDecimal(int startIndex) {
			for (int i = startIndex; i >= 0; --i) {
				INumberFormatElement element = this[i];
				if (element.IsDigit) {
					if (decimalCount == 0) {
						Insert(++i, NumberFormatDecimalSeparator.Instance);
						Insert(++i, NumberFormatDigitZero.Instance);
					}
					else
						Insert(++i, element.Clone());
					break;
				}
			}
			++decimalCount;
		}
		public override void DecreaseDecimal() {
			if (decimalCount == 0)
				return;
			DecreaseDecimal(Count - 1);
		}
		protected void DecreaseDecimal(int startIndex) {
			for (int i = startIndex; i >= 0; --i)
				if (this[i].IsDigit) {
					RemoveAt(i);
					break;
				}
			if (decimalCount == 1)
				RemoveAt(decimalSeparatorIndex);
			--decimalCount;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			NumericNumberFormatSimple other = obj as NumericNumberFormatSimple;
			if (obj == null)
				return false;
			if (decimalCount != other.decimalCount || decimalSeparatorIndex != other.decimalSeparatorIndex || displayFactor != other.displayFactor ||
				grouping != other.grouping || integerCount != other.integerCount || isNegativePart != other.isNegativePart || percentCount != other.percentCount)
				return false;
			return true;
		}
		public override void CopyFrom(INumberFormat value) {
			NumericNumberFormatSimple format = value as NumericNumberFormatSimple;
			isNegativePart = format.isNegativePart;
			grouping = format.grouping;
			percentCount = format.percentCount;
			integerCount = format.integerCount;
			decimalCount = format.decimalCount;
			displayFactor = format.displayFactor;
			decimalSeparatorIndex = format.decimalSeparatorIndex;
			AddRange(this);
			base.CopyFrom(value);
		}
		public override SimpleNumberFormat Clone() {
			NumericNumberFormatSimple clone = new NumericNumberFormatSimple();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region NumericNumberFormatExponent
	public class NumericNumberFormatExponent : NumericNumberFormatSimple {
		bool explicitSign;
		int expIndex;
		int expCount;
		protected NumericNumberFormatExponent() {
		}
		public NumericNumberFormatExponent(IEnumerable<INumberFormatElement> elements, int integerCount, int decimalCount, int decimalSeparatorIndex, int expIndex, int expCount, bool explicitSign, bool grouping, bool isNegativePart)
			: base(elements, 0, integerCount, decimalCount, 0, decimalSeparatorIndex, grouping, isNegativePart) {
			this.explicitSign = explicitSign;
			this.expIndex = expIndex;
			this.expCount = expCount;
		}
		public override void AppendFormat(StringBuilder builder) {
			AppendSimple(builder, expIndex - 1);
			builder.Append('E');
			builder.Append(explicitSign ? '+' : '-');
			for (int i = expIndex; i < Count; ++i)
				this[i].AppendFormat(builder);
		}
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			NumberFormatResult result = new NumberFormatResult();
			if (!value.IsNumeric) {
				result.Text = value.ToText(context).InlineTextValue;
				return result;
			}
			result.Text = string.Empty;
			double exponent;
			if (value.NumericValue == 0) {
				exponent = 0;
				for (int i = 0; i < expIndex; ++i)
					this[i].Format(value, context, result);
			}
			else {
				Tuple<double, double> numericValue = CalculateExponent(value.NumericValue);
				exponent = numericValue.Item2;
				FormatSimple(numericValue.Item1, context, result, expIndex - 1);
			}
			result.Text += "E";
			if (exponent < 0)
				result.Text += '-';
			else
				if (explicitSign)
					result.Text += '+';
			NumberFormatResult resultT = new NumberFormatResult();
			resultT.Text = string.Empty;
			FormatIntegerPart(Math.Abs(exponent), Count - 1, expIndex, expCount, false, context, resultT);
			result.Text += resultT.Text;
			if (!IsNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, context.Culture);
			return result;
		}
		Tuple<double, double> CalculateExponent(double numericValue) {
			double mantissa = Math.Abs(numericValue);
			double exponent = Math.Floor(Math.Floor(Math.Log10(mantissa)) / IntegerCount) * IntegerCount;
			mantissa = Math.Round(mantissa / Math.Pow(10, exponent), DecimalCount, MidpointRounding.AwayFromZero);
			return new Tuple<double, double>(mantissa, exponent);
		}
		public override VariantValue Round(VariantValue value, WorkbookDataContext context) {
			if (!value.IsNumeric || value.NumericValue == 0)
				return value;
			Tuple<double, double> numericValue = CalculateExponent(value.NumericValue);
			return Math.Round(numericValue.Item1, DecimalCount, MidpointRounding.AwayFromZero) * Math.Pow(10, numericValue.Item2);
		}
		public override void IncreaseDecimal() {
			IncreaseDecimal(expIndex - 1);
			expIndex += DecimalCount == 1 ? 2 : 1;
		}
		public override void DecreaseDecimal() {
			if (DecimalCount == 0)
				return;
			DecreaseDecimal(expIndex - 1);
			expIndex -= DecimalCount == 0 ? 2 : 1;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumericNumberFormatExponent format = obj as NumericNumberFormatExponent;
			if (format == null || expIndex != format.expIndex || expCount != format.expCount || explicitSign != format.explicitSign)
				return false;
			return base.Equals(obj);
		}
		public override void CopyFrom(INumberFormat value) {
			NumericNumberFormatExponent format = value as NumericNumberFormatExponent;
			expIndex = format.expIndex;
			expCount = format.expCount;
			explicitSign = format.explicitSign;
			base.CopyFrom(value);
		}
		public override SimpleNumberFormat Clone() {
			NumericNumberFormatExponent clone = new NumericNumberFormatExponent();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region NumericNumberFormatFraction
	public class NumericNumberFormatFraction : NumericNumberFormatSimple {
		int dividentCount;
		int divisorCount;
		int divisorIndex;
		protected NumericNumberFormatFraction() {
		}
		public NumericNumberFormatFraction(IEnumerable<INumberFormatElement> elements, int percentCount, int integerCount, int preFractionIndex, int fractionSeparatorIndex, int divisorIndex, int dividentCount, int divisorCount, bool grouping, bool isNegativePart)
			: base(elements, percentCount, integerCount, fractionSeparatorIndex, 0, preFractionIndex, grouping, isNegativePart) {
			this.dividentCount = dividentCount;
			this.divisorCount = divisorCount;
			this.divisorIndex = divisorIndex;
		}
		protected int PreFractionIndex { get { return DecimalSeparatorIndex; } }
		protected int FractionSeparatorIndex { get { return DecimalCount; } }
		protected int DivisorIndex { get { return divisorIndex; } }
		protected int DivisorCount { get { return divisorCount; } set { divisorCount = value; } }
		protected int DividentCount { get { return dividentCount; } set { dividentCount = value; } }
		public override void AppendFormat(StringBuilder builder) {
			AppendSimple(builder, FractionSeparatorIndex - 1);
			builder.Append('/');
			for (int i = FractionSeparatorIndex; i < Count; ++i)
				this[i].AppendFormat(builder);
		}
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			NumberFormatResult result = new NumberFormatResult();
			if (!value.IsNumeric) {
				result.Text = value.ToText(context).InlineTextValue;
				return result;
			}
			result.Text = string.Empty;
			NumberFormatResult resultT = new NumberFormatResult();
			resultT.Text = string.Empty;
			double numericValue = Math.Abs(value.NumericValue) * Math.Pow(100, PercentCount);
			double integerPart = Math.Truncate(numericValue);
			Size fracValues;
			int i = 0;
			if (IntegerCount > 0) {
				fracValues = CalculateRationalApproximation((decimal)(numericValue - integerPart));
				INumberFormatElement element;
				for (; ; ++i) {
					element = this[i];
					if (element.IsDigit)
						break;
					element.FormatEmpty(context, result);
				}
				if (integerPart == 0) {
					FormatZeroIntegerPart(PreFractionIndex, i, context, resultT);
					if (fracValues.Width != 0)
						resultT.Text = new string(' ', resultT.Text.Length);
				}
				else
					FormatIntegerPart(integerPart, PreFractionIndex, i, IntegerCount, Grouping, context, resultT);
				result.Text += resultT.Text;
			}
			else
				fracValues = CalculateRationalApproximation((decimal)(numericValue));
			int divident = fracValues.Width;
			int divisor = fracValues.Height;
			resultT.Text = string.Empty;
			if (divident == 0) {
				FormatZeroIntegerPart(FractionSeparatorIndex - 1, PreFractionIndex + 1, context, resultT);
				resultT.Text += "/";
				FormatDivisor(divisor, DivisorIndex, context, resultT);
				if (IntegerCount > 0)
					resultT.Text = new string(' ', resultT.Text.Length);
			}
			else {
				FormatIntegerPart(divident, FractionSeparatorIndex - 1, PreFractionIndex + 1, dividentCount, false, context, resultT);
				resultT.Text += "/";
				FormatDivisor(divisor, DivisorIndex, context, resultT);
			}
			result.Text += resultT.Text;
			for (i = DivisorIndex; i < Count; ++i)
				this[i].FormatEmpty(context, result);
			if (!IsNegativePart && value.NumericValue < 0)
				result.Text = ApplyNegativeSign(result.Text, context.Culture);
			return result;
		}
		void FormatZeroIntegerPart(int startIndex, int endIndex, WorkbookDataContext context, NumberFormatResult result) {
			NumberFormatResult resultT = new NumberFormatResult();
			resultT.Text = string.Empty;
			int i = startIndex;
			INumberFormatElement element;
			for (; i >= endIndex; --i) {
				element = this[i];
				if (element.IsDigit) {
					result.Text = '0' + result.Text;
					--i;
					break;
				}
				else {
					element.FormatEmpty(context, resultT);
					result.Text = resultT.Text + result.Text;
					resultT.Text = string.Empty;
				}
			}
			for (; i >= endIndex; --i) {
				this[i].FormatEmpty(context, resultT);
				result.Text = resultT.Text + result.Text;
				resultT.Text = string.Empty;
			}
			result.IsError = result.IsError || resultT.IsError;
			if (resultT.Color != null)
				result.Color = resultT.Color;
		}
		protected virtual void FormatDivisor(int divisor, int endIndex, WorkbookDataContext context, NumberFormatResult result) {
			int pow = (int)Math.Pow(10, Math.Truncate(Math.Log10(divisor)) + 1);
			int divisorTemp;
			int i = FractionSeparatorIndex;
			INumberFormatElement element;
			for (; pow > 1 && i < endIndex; ++i) {
				element = this[i];
				if (element.IsDigit) {
					pow /= 10;
					divisorTemp = divisor % pow;
					element.Format((divisor - divisorTemp) / pow, context, result);
					divisor = divisorTemp;
				}
				else
					element.FormatEmpty(context, result);
			}
			for (; i < endIndex; ++i)
				this[i].FormatEmpty(context, result);
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
			if (Math.Abs(value) < Math.Abs(value - rightDivident)) {
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
			for (; ; ) {
				int divisor = leftDivisor + rightDivisor;
				if (divisor >= maxDivisor)
					break;
				int divident = leftDivident + rightDivident;
				if (value * divisor < divident) {
					rightDivident = divident;
					rightDivisor = divisor;
				}
				else {
					leftDivident = divident;
					leftDivisor = divisor;
				}
				decimal approximation = (decimal)divident / divisor;
				if (Math.Abs(value - approximation) < minimalError) {
					minimalError = Math.Abs(value - approximation);
					bestDivident = divident;
					bestDivisor = divisor;
				}
			}
			return new Size(bestDivident + integerPart * bestDivisor, bestDivisor);
		}
		public override VariantValue Round(VariantValue value, WorkbookDataContext context) {
			if (!value.IsNumeric)
				return value;
			double numericValue = value.NumericValue;
			Size fracValues = CalculateRationalApproximation((decimal)Math.Abs(numericValue));
			return Math.Sign(numericValue) * ((double)fracValues.Width / fracValues.Height) * Math.Pow(10, PercentCount * 2);
		}
		public override void IncreaseDecimal() { }
		public override void DecreaseDecimal() { }
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			NumericNumberFormatFraction other = obj as NumericNumberFormatFraction;
			if (dividentCount != other.dividentCount || divisorCount != other.divisorCount || divisorIndex != other.divisorIndex)
				return false;
			return true;
		}
		public override void CopyFrom(INumberFormat value) {
			NumericNumberFormatFraction format = value as NumericNumberFormatFraction;
			dividentCount = format.dividentCount;
			divisorCount = format.divisorCount;
			divisorIndex = format.divisorIndex;
			base.CopyFrom(value);
		}
		public override SimpleNumberFormat Clone() {
			NumericNumberFormatFraction clone = new NumericNumberFormatFraction();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region NumericNumberFormatFractionExcplicit
	public class NumericNumberFormatFractionExcplicit : NumericNumberFormatFraction {
		protected NumericNumberFormatFractionExcplicit() {
		}
		public NumericNumberFormatFractionExcplicit(IEnumerable<INumberFormatElement> elements, int percentCount, int integerCount, int preFractionIndex, int fractionSeparatorIndex, int divisorIndex, int dividentCount, int divisor, bool grouping, bool isNegativePart)
			: base(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, divisorIndex, dividentCount, divisor, grouping, isNegativePart) {
		}
		int ExplicitDivisor { get { return DivisorCount; } }
		public override void AppendFormat(StringBuilder builder) {
			AppendSimple(builder, FractionSeparatorIndex - 1);
			builder.Append('/');
			for (int i = FractionSeparatorIndex; i < DivisorIndex; ++i)
				this[i].AppendFormat(builder);
			builder.Append(ExplicitDivisor);
			for (int i = DivisorIndex; i < Count; ++i)
				this[i].AppendFormat(builder);
		}
		protected override void FormatDivisor(int divisor, int endIndex, WorkbookDataContext context, NumberFormatResult result) {
			for (int i = FractionSeparatorIndex; i < endIndex; ++i)
				this[i].FormatEmpty(context, result);
			result.Text += ExplicitDivisor;
		}
		protected override Size CalculateRationalApproximation(decimal value) {
			int integerPart = (int)value;
			value -= integerPart;
			int divident = (int)Math.Round(value * ExplicitDivisor, 0, MidpointRounding.AwayFromZero);
			return new Size(divident + integerPart * ExplicitDivisor, ExplicitDivisor);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!(obj is NumericNumberFormatFractionExcplicit))
				return false;
			return base.Equals(obj);
		}
		public override SimpleNumberFormat Clone() {
			NumericNumberFormatFractionExcplicit clone = new NumericNumberFormatFractionExcplicit();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region GeneralNumberFormat
	public class GeneralNumberFormat : SimpleNumberFormat {
		int generalIndex;
		public GeneralNumberFormat() {
		}
		public GeneralNumberFormat(IEnumerable<INumberFormatElement> elements, int generalIndex) {
			this.AddRange(elements);
			this.generalIndex = generalIndex;
		}
		public override NumberFormatType Type { get { return NumberFormatType.General; } }
		public override void AppendFormat(StringBuilder builder) {
			int i = 0;
			for (; i < generalIndex; ++i)
				this[i].AppendFormat(builder);
			builder.Append(NumberFormatParser.Localizer.GeneralDesignator);
			for (; i < Count; ++i)
				this[i].AppendFormat(builder);
		}
		public override NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) { 
			NumberFormatResult result = new NumberFormatResult();
			switch (value.Type) {
				case VariantValueType.None:
					result.Text = String.Empty;
					break;
				case VariantValueType.Numeric:
					result.Text = FormatNumeric(value, context, parameters);
					break;
				case VariantValueType.Boolean: {
						VariantValue text = value.ToText(context);
						if (text.IsError)
							result.Text = CellErrorFactory.GetErrorName(text.ErrorValue, context);
						else
							result.Text = text.InlineTextValue;
					}
					break;
				case VariantValueType.InlineText:
					result.Text = value.InlineTextValue;
					break;
				case VariantValueType.SharedString:
					result.Text = value.GetTextValue(context.StringTable);
					break;
				case VariantValueType.Array:
					result.Text = String.Empty;
					break;
				case VariantValueType.Error:
					result.Text = CellErrorFactory.GetErrorName(value.ErrorValue, context);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return result;
		}
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			throw new InvalidOperationException();
		}
		public string FormatNumeric(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			GeneralFormatter formatter = new GeneralFormatter(context.Culture);
			int maxWidthInChars;
			if (parameters.Measurer == null)
				maxWidthInChars = 100;
			else
				maxWidthInChars = (int)Math.Floor(parameters.AvailableSpaceWidth / parameters.Measurer.MaxDigitWidth);
			return formatter.Format(value.NumericValue, maxWidthInChars);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			GeneralNumberFormat other = obj as GeneralNumberFormat;
			if (generalIndex != other.generalIndex)
				return false;
			return true;
		}
		public override void CopyFrom(INumberFormat value) {
			GeneralNumberFormat format = value as GeneralNumberFormat;
			generalIndex = format.generalIndex;
			base.CopyFrom(value);
		}
		public override SimpleNumberFormat Clone() {
			GeneralNumberFormat clone = new GeneralNumberFormat();
			clone.CopyFrom(this);
			return clone;
		}
	}
	#endregion
	#region GenericNumberFormat
	public class GenericNumberFormat : INumberFormat {
		public static GenericNumberFormat Instance = new GenericNumberFormat();
		public GenericNumberFormat() {
		}
		public NumberFormatType Type { get { return NumberFormatType.General; } }
		public bool IsTimeFormat { get { return false; } }
		public void AppendFormat(StringBuilder builder) { }
		public NumberFormatResult Format(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			NumberFormatResult result = new NumberFormatResult();
			switch (value.Type) {
				case VariantValueType.None:
					result.Text = String.Empty;
					break;
				case VariantValueType.Numeric:
					result.Text = FormatNumeric(value, context, parameters);
					break;
				case VariantValueType.Boolean: {
						VariantValue text = value.ToText(context);
						if (text.IsError)
							result.Text = CellErrorFactory.GetErrorName(text.ErrorValue, context);
						else
							result.Text = text.InlineTextValue;
					}
					break;
				case VariantValueType.InlineText:
					result.Text = value.InlineTextValue;
					break;
				case VariantValueType.SharedString:
					result.Text = value.GetTextValue(context.StringTable);
					break;
				case VariantValueType.Array:
					result.Text = String.Empty;
					break;
				case VariantValueType.Error:
					result.Text = CellErrorFactory.GetErrorName(value.ErrorValue, context);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return result;
		}
		public string FormatNumeric(VariantValue value, WorkbookDataContext context, NumberFormatParameters parameters) {
			GeneralFormatter formatter = new GeneralFormatter(context.Culture);
			int maxWidthInChars;
			if (parameters.Measurer == null)
				maxWidthInChars = 100;
			else
				maxWidthInChars = (int)Math.Floor(parameters.AvailableSpaceWidth / parameters.Measurer.MaxDigitWidth);
			return formatter.Format(value.NumericValue, maxWidthInChars);
		}
		public VariantValue Round(VariantValue value, WorkbookDataContext context) {
			return value;
		}
		public void IncreaseDecimal() { }
		public void DecreaseDecimal() { }
		public bool HasColorForPositiveOrNegative() {
			return false;
		}
		public bool EnclosedInParenthesesForPositive() {
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public void CopyFrom(INumberFormat value) { }
		public INumberFormat Clone() {
			return this;
		}
	}
	#endregion
	#region TextNumberFormat
	public class TextNumberFormat : SimpleNumberFormat {
		protected TextNumberFormat() {
		}
		public TextNumberFormat(IEnumerable<INumberFormatElement> elements)
			: base(elements) {
		}
		public override NumberFormatType Type { get { return NumberFormatType.Text; } }
		public override NumberFormatResult FormatCore(VariantValue value, WorkbookDataContext context) {
			NumberFormatResult result = new NumberFormatResult();
			result.Text = string.Empty;
			foreach (INumberFormatElement element in this)
				element.Format(value, context, result);
			return result;
		}
		public override SimpleNumberFormat Clone() {
			TextNumberFormat clone = new TextNumberFormat();
			clone.AddRange(this);
			return clone;
		}
	}
	#endregion
	#region NumberFormatParser
	public enum NumberFormatDesignator {
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
	public static class NumberFormatParser {
		[ThreadStatic]
		static NumberFormatParser2 parser;
		static NumberFormatParser2 Parser {
			get {
				if (parser == null)
					parser = new NumberFormatParser2();
				return parser;
			}
		}
		public static NumberFormatLocalizer Localizer { get { return Parser.Localizer; } }
		public static NumberFormat Parse(string formatString, CultureInfo culture) {
			return Parser.Parse(formatString, culture);
		}
		public static NumberFormat Parse(string formatString) {
			return Parser.Parse(formatString);
		}
	}
	public class NumberFormatParser2 {
		delegate void NumberFormatDesignatorParseMethod();
		readonly NumberFormatLocalizer localizer;
		readonly List<Color> AllowedColors = new List<Color>() { DXColor.Red, DXColor.Black, DXColor.White, DXColor.Blue, DXColor.Magenta, DXColor.Yellow, DXColor.Cyan, DXColor.Green };
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods2;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods3;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods4;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods5;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods6;
		readonly Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod> parseMethods7;
		string formatString;
		List<SimpleNumberFormat> formats;
		List<INumberFormatElement> elements;
		NumberFormatDisplayLocale locale;
		SimpleNumberFormat part;
		string elementString;
		INumberFormatElement element;
		int currentIndex;
		char currentSymbol;
		bool errorState;
		NumberFormatDesignator designator;
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
		public NumberFormatParser2() {
			localizer = new NumberFormatLocalizer();
			parseMethods = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods.Add(NumberFormatDesignator.At, OnAt);
			parseMethods.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods.Add(NumberFormatDesignator.Bracket, OnBracket);
			parseMethods.Add(NumberFormatDesignator.DateSeparator, OnError);
			parseMethods.Add(NumberFormatDesignator.Day, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault);
			parseMethods.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods.Add(NumberFormatDesignator.DecimalSeparator, OnNumericSymbol);
			parseMethods.Add(NumberFormatDesignator.DigitEmpty, OnNumericSymbol);
			parseMethods.Add(NumberFormatDesignator.DigitSpace, OnNumericSymbol);
			parseMethods.Add(NumberFormatDesignator.DigitZero, OnNumericSymbol);
			parseMethods.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart);
			parseMethods.Add(NumberFormatDesignator.Exponent, OnESymbol);
			parseMethods.Add(NumberFormatDesignator.FractionOrDateSeparator, OnError);
			parseMethods.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods.Add(NumberFormatDesignator.General, OnGeneral);
			parseMethods.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnGeneralOrDateTime);
			parseMethods.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnGeneralOrDateTime);
			parseMethods.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnGeneralOrDateTime);
			parseMethods.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnGeneralOrDateTime);
			parseMethods.Add(NumberFormatDesignator.Hour, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods[NumberFormatDesignator.Hour]);
			parseMethods.Add(NumberFormatDesignator.InvariantYear, OnESymbol);
			parseMethods.Add(NumberFormatDesignator.JapaneseEra, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.Minute, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.Month, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.Percent, OnNumericSymbol);
			parseMethods.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods.Add(NumberFormatDesignator.Second, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.ThaiYear, OnDateTimeSymbol);
			parseMethods.Add(NumberFormatDesignator.TimeSeparator, OnDefault);
			parseMethods.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods.Add(NumberFormatDesignator.Year, OnDateTimeSymbol);
			parseMethods2 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods2.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnAmPmOrDayOfWeek);
			parseMethods2.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnAmPmOrMonth);
			parseMethods2.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnAmPmOrYear);
			parseMethods2.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods2[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods2.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods2.Add(NumberFormatDesignator.At, OnError);
			parseMethods2.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods2.Add(NumberFormatDesignator.Bracket, OnBracket2);
			parseMethods2.Add(NumberFormatDesignator.DateSeparator, OnDateSeparator);
			parseMethods2.Add(NumberFormatDesignator.Day, OnDay);
			parseMethods2.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault2);
			parseMethods2.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods2.Add(NumberFormatDesignator.DecimalSeparator, OnMilisecond);
			parseMethods2.Add(NumberFormatDesignator.DigitEmpty, OnError);
			parseMethods2.Add(NumberFormatDesignator.DigitSpace, OnError);
			parseMethods2.Add(NumberFormatDesignator.DigitZero, OnError);
			parseMethods2.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart2);
			parseMethods2.Add(NumberFormatDesignator.Exponent, OnESymbol2);
			parseMethods2.Add(NumberFormatDesignator.FractionOrDateSeparator, OnDefaultDateSeparator);
			parseMethods2.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods2.Add(NumberFormatDesignator.General, OnGeneral2);
			parseMethods2.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnGeneralOrDay);
			parseMethods2.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods2[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods2.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnGeneralOrSecond);
			parseMethods2.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnGeneralOrInvariantYear);
			parseMethods2.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnGeneralOrJapaneseEra);
			parseMethods2.Add(NumberFormatDesignator.Hour, OnHour);
			parseMethods2.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods2[NumberFormatDesignator.Hour]);
			parseMethods2.Add(NumberFormatDesignator.InvariantYear, OnESymbol2);
			parseMethods2.Add(NumberFormatDesignator.JapaneseEra, OnJapaneseEra);
			parseMethods2.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnMonthOrMinute);
			parseMethods2.Add(NumberFormatDesignator.Minute, OnMinute);
			parseMethods2.Add(NumberFormatDesignator.Month, OnMonth);
			parseMethods2.Add(NumberFormatDesignator.Percent, OnError);
			parseMethods2.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods2.Add(NumberFormatDesignator.Second, OnSecond);
			parseMethods2.Add(NumberFormatDesignator.ThaiYear, OnThaiYear);
			parseMethods2.Add(NumberFormatDesignator.TimeSeparator, OnTimeSeparator);
			parseMethods2.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods2.Add(NumberFormatDesignator.Year, OnYear);
			parseMethods3 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods3.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnError);
			parseMethods3.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnError);
			parseMethods3.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnError);
			parseMethods3.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods3[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods3.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods3.Add(NumberFormatDesignator.At, OnError);
			parseMethods3.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods3.Add(NumberFormatDesignator.Bracket, OnBracket3);
			parseMethods3.Add(NumberFormatDesignator.DateSeparator, OnError);
			parseMethods3.Add(NumberFormatDesignator.Day, OnError);
			parseMethods3.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods3.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods3.Add(NumberFormatDesignator.DecimalSeparator, OnDecimalSeparator);
			parseMethods3.Add(NumberFormatDesignator.DigitEmpty, OnDigitEmpty);
			parseMethods3.Add(NumberFormatDesignator.DigitSpace, OnDigitSpace);
			parseMethods3.Add(NumberFormatDesignator.DigitZero, OnDigitZero);
			parseMethods3.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart3);
			parseMethods3.Add(NumberFormatDesignator.Exponent, OnESymbol3);
			parseMethods3.Add(NumberFormatDesignator.FractionOrDateSeparator, OnFractionSeparator);
			parseMethods3.Add(NumberFormatDesignator.GroupSeparator, OnGroupSeparator);
			parseMethods3.Add(NumberFormatDesignator.General, OnGeneral3);
			parseMethods3.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnError);
			parseMethods3.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods3[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods3.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnError);
			parseMethods3.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnError);
			parseMethods3.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods3.Add(NumberFormatDesignator.Hour, OnError);
			parseMethods3.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods3[NumberFormatDesignator.Hour]);
			parseMethods3.Add(NumberFormatDesignator.InvariantYear, OnESymbol3);
			parseMethods3.Add(NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods3.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnError);
			parseMethods3.Add(NumberFormatDesignator.Minute, OnError);
			parseMethods3.Add(NumberFormatDesignator.Month, OnError);
			parseMethods3.Add(NumberFormatDesignator.Percent, OnPercent);
			parseMethods3.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods3.Add(NumberFormatDesignator.Second, OnError);
			parseMethods3.Add(NumberFormatDesignator.ThaiYear, OnError);
			parseMethods3.Add(NumberFormatDesignator.TimeSeparator, OnError);
			parseMethods3.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods3.Add(NumberFormatDesignator.Year, OnError);
			parseMethods4 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods4.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnError);
			parseMethods4.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnError);
			parseMethods4.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnError);
			parseMethods4.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods4[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods4.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods4.Add(NumberFormatDesignator.At, OnError);
			parseMethods4.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods4.Add(NumberFormatDesignator.Bracket, OnBracket3);
			parseMethods4.Add(NumberFormatDesignator.DateSeparator, OnError);
			parseMethods4.Add(NumberFormatDesignator.Day, OnError);
			parseMethods4.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods4.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods4.Add(NumberFormatDesignator.DecimalSeparator, OnDecimalSeparator);
			parseMethods4.Add(NumberFormatDesignator.DigitEmpty, OnDigitEmpty4);
			parseMethods4.Add(NumberFormatDesignator.DigitSpace, OnDigitSpace4);
			parseMethods4.Add(NumberFormatDesignator.DigitZero, OnDigitZero4);
			parseMethods4.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart4);
			parseMethods4.Add(NumberFormatDesignator.Exponent, OnError);
			parseMethods4.Add(NumberFormatDesignator.FractionOrDateSeparator, OnError);
			parseMethods4.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods4.Add(NumberFormatDesignator.General, OnGeneral3);
			parseMethods4.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnError);
			parseMethods4.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods4[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods4.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnError);
			parseMethods4.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnError);
			parseMethods4.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods4.Add(NumberFormatDesignator.Hour, OnError);
			parseMethods4.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods4[NumberFormatDesignator.Hour]);
			parseMethods4.Add(NumberFormatDesignator.InvariantYear, OnError);
			parseMethods4.Add(NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods4.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnError);
			parseMethods4.Add(NumberFormatDesignator.Minute, OnError);
			parseMethods4.Add(NumberFormatDesignator.Month, OnError);
			parseMethods4.Add(NumberFormatDesignator.Percent, OnDefault);
			parseMethods4.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods4.Add(NumberFormatDesignator.Second, OnError);
			parseMethods4.Add(NumberFormatDesignator.ThaiYear, OnError);
			parseMethods4.Add(NumberFormatDesignator.TimeSeparator, OnError);
			parseMethods4.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods4.Add(NumberFormatDesignator.Year, OnError);
			parseMethods5 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods5.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnError);
			parseMethods5.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnError);
			parseMethods5.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnError);
			parseMethods5.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods5[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods5.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods5.Add(NumberFormatDesignator.At, OnError);
			parseMethods5.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods5.Add(NumberFormatDesignator.Bracket, OnBracket3);
			parseMethods5.Add(NumberFormatDesignator.DateSeparator, OnError);
			parseMethods5.Add(NumberFormatDesignator.Day, OnError);
			parseMethods5.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods5.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods5.Add(NumberFormatDesignator.DecimalSeparator, OnError);
			parseMethods5.Add(NumberFormatDesignator.DigitEmpty, OnDigitEmpty5);
			parseMethods5.Add(NumberFormatDesignator.DigitSpace, OnDigitSpace5);
			parseMethods5.Add(NumberFormatDesignator.DigitZero, OnDigitZero5);
			parseMethods5.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart5);
			parseMethods5.Add(NumberFormatDesignator.Exponent, OnError);
			parseMethods5.Add(NumberFormatDesignator.FractionOrDateSeparator, OnError);
			parseMethods5.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods5.Add(NumberFormatDesignator.General, OnGeneral3);
			parseMethods5.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnError);
			parseMethods5.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods5[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods5.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnError);
			parseMethods5.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnError);
			parseMethods5.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods5.Add(NumberFormatDesignator.Hour, OnError);
			parseMethods5.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods5[NumberFormatDesignator.Hour]);
			parseMethods5.Add(NumberFormatDesignator.InvariantYear, OnError);
			parseMethods5.Add(NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods5.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnError);
			parseMethods5.Add(NumberFormatDesignator.Minute, OnError);
			parseMethods5.Add(NumberFormatDesignator.Month, OnError);
			parseMethods5.Add(NumberFormatDesignator.Percent, OnPercent);
			parseMethods5.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods5.Add(NumberFormatDesignator.Second, OnError);
			parseMethods5.Add(NumberFormatDesignator.ThaiYear, OnError);
			parseMethods5.Add(NumberFormatDesignator.TimeSeparator, OnError);
			parseMethods5.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods5.Add(NumberFormatDesignator.Year, OnError);
			parseMethods6 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods6.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnError);
			parseMethods6.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnError);
			parseMethods6.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnError);
			parseMethods6.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods6[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods6.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods6.Add(NumberFormatDesignator.At, OnAt6);
			parseMethods6.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods6.Add(NumberFormatDesignator.Bracket, OnBracket3);
			parseMethods6.Add(NumberFormatDesignator.DateSeparator, OnError);
			parseMethods6.Add(NumberFormatDesignator.Day, OnError);
			parseMethods6.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods6.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods6.Add(NumberFormatDesignator.DecimalSeparator, OnError);
			parseMethods6.Add(NumberFormatDesignator.DigitEmpty, OnError);
			parseMethods6.Add(NumberFormatDesignator.DigitSpace, OnError);
			parseMethods6.Add(NumberFormatDesignator.DigitZero, OnError);
			parseMethods6.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart6);
			parseMethods6.Add(NumberFormatDesignator.Exponent, OnError);
			parseMethods6.Add(NumberFormatDesignator.FractionOrDateSeparator, OnError);
			parseMethods6.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods6.Add(NumberFormatDesignator.General, OnGeneral3);
			parseMethods6.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnError);
			parseMethods6.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods6[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods6.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnError);
			parseMethods6.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnError);
			parseMethods6.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods6.Add(NumberFormatDesignator.Hour, OnError);
			parseMethods6.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods6[NumberFormatDesignator.Hour]);
			parseMethods6.Add(NumberFormatDesignator.InvariantYear, OnError);
			parseMethods6.Add(NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods6.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnError);
			parseMethods6.Add(NumberFormatDesignator.Minute, OnError);
			parseMethods6.Add(NumberFormatDesignator.Month, OnError);
			parseMethods6.Add(NumberFormatDesignator.Percent, OnError);
			parseMethods6.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods6.Add(NumberFormatDesignator.Second, OnError);
			parseMethods6.Add(NumberFormatDesignator.ThaiYear, OnError);
			parseMethods6.Add(NumberFormatDesignator.TimeSeparator, OnDefault);
			parseMethods6.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods6.Add(NumberFormatDesignator.Year, OnError);
			parseMethods7 = new Dictionary<NumberFormatDesignator, NumberFormatDesignatorParseMethod>();
			parseMethods7.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek, OnError);
			parseMethods7.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Month, OnError);
			parseMethods7.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.Year, OnError);
			parseMethods7.Add(NumberFormatDesignator.AmPm | NumberFormatDesignator.DayOfWeek | NumberFormatDesignator.Month, parseMethods7[NumberFormatDesignator.AmPm | NumberFormatDesignator.Month]);
			parseMethods7.Add(NumberFormatDesignator.Asterisk, OnAsterisk);
			parseMethods7.Add(NumberFormatDesignator.At, OnError);
			parseMethods7.Add(NumberFormatDesignator.Backslash, OnBackslash);
			parseMethods7.Add(NumberFormatDesignator.Bracket, OnBracket3);
			parseMethods7.Add(NumberFormatDesignator.DateSeparator, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.Day, OnError);
			parseMethods7.Add(NumberFormatDesignator.DayOfWeek, OnDayOfWeekOrDefault3);
			parseMethods7.Add(NumberFormatDesignator.Default, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.DecimalSeparator, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.DigitEmpty, OnError);
			parseMethods7.Add(NumberFormatDesignator.DigitSpace, OnError);
			parseMethods7.Add(NumberFormatDesignator.DigitZero, OnError);
			parseMethods7.Add(NumberFormatDesignator.EndOfPart, OnEndOfPart7);
			parseMethods7.Add(NumberFormatDesignator.Exponent, OnError);
			parseMethods7.Add(NumberFormatDesignator.FractionOrDateSeparator, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.GroupSeparator, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.General, OnGeneral7);
			parseMethods7.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day, OnGeneralOrDateTime7);
			parseMethods7.Add(NumberFormatDesignator.General | NumberFormatDesignator.Day | NumberFormatDesignator.JapaneseEra, parseMethods7[NumberFormatDesignator.General | NumberFormatDesignator.Day]);
			parseMethods7.Add(NumberFormatDesignator.General | NumberFormatDesignator.Second, OnGeneralOrDateTime7);
			parseMethods7.Add(NumberFormatDesignator.General | NumberFormatDesignator.InvariantYear, OnGeneralOrDateTime7);
			parseMethods7.Add(NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra, OnGeneralOrDateTime7);
			parseMethods7.Add(NumberFormatDesignator.Hour, OnError);
			parseMethods7.Add(NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra, parseMethods7[NumberFormatDesignator.Hour]);
			parseMethods7.Add(NumberFormatDesignator.InvariantYear, OnError);
			parseMethods7.Add(NumberFormatDesignator.JapaneseEra, OnError);
			parseMethods7.Add(NumberFormatDesignator.Minute | NumberFormatDesignator.Month, OnError);
			parseMethods7.Add(NumberFormatDesignator.Minute, OnError);
			parseMethods7.Add(NumberFormatDesignator.Month, OnError);
			parseMethods7.Add(NumberFormatDesignator.Percent, OnError);
			parseMethods7.Add(NumberFormatDesignator.Quote, OnQuote);
			parseMethods7.Add(NumberFormatDesignator.Second, OnError);
			parseMethods7.Add(NumberFormatDesignator.ThaiYear, OnError);
			parseMethods7.Add(NumberFormatDesignator.TimeSeparator, OnDefault);
			parseMethods7.Add(NumberFormatDesignator.Underline, OnUnderline);
			parseMethods7.Add(NumberFormatDesignator.Year, OnError);
		}
		public NumberFormatLocalizer Localizer { get { return localizer; } }
		public NumberFormat Parse(string formatString) {
			return Parse(formatString, CultureInfo.InvariantCulture);
		}
		public NumberFormat Parse(string formatString, CultureInfo culture) {
			if (!string.IsNullOrEmpty(formatString)) {
				this.formatString = formatString;
				localizer.SetCulture(culture);
				INumberFormat format = ParseCore(culture);
				ClearLocals();
				if (format == null)
					return null;
				return new NumberFormat(format);
			}
			return NumberFormat.Generic;
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
		INumberFormat ParseCore(CultureInfo culture) {
			formatString += ';';
			formats = new List<SimpleNumberFormat>();
			elements = new List<INumberFormatElement>();
			for (currentIndex = 0; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = NumberFormatDesignator.Default;
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
				elements = new List<INumberFormatElement>();
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
					if (part.Type != NumberFormatType.General || part.Count != 0)
						formats.Add(part);
					break;
				}
			}
			if (formats.Count == 1) {
				SimpleNumberFormat simpleResult = formats[0];
				if (simpleResult.Type == NumberFormatType.DateTime) {
					if (DayMonthYearNumberFormat.Instance.AreSame(simpleResult, culture))
						return DayMonthYearNumberFormat.Instance;
					if (DayMonthNumberFormat.Instance.AreSame(simpleResult, culture))
						return DayMonthNumberFormat.Instance;
					if (MonthYearNumberFormat.Instance.AreSame(simpleResult, culture))
						return MonthYearNumberFormat.Instance;
					if (ShortDateNumberFormat.Instance.AreSame(simpleResult, culture))
						return ShortDateNumberFormat.Instance;
					if (ShortTimeNumberFormat.Instance.AreSame(simpleResult, culture))
						return ShortTimeNumberFormat.Instance;
					if (FullDateTimeNumberFormat.Instance.AreSame(simpleResult, culture))
						return FullDateTimeNumberFormat.Instance;
				}
				else if (simpleResult.Type == NumberFormatType.General && simpleResult.Count == 0)
					return GenericNumberFormat.Instance;
				return simpleResult;
			}
			ConditionalNumberFormat result = new ConditionalNumberFormat();
			result.AddRange(formats);
			return result;
		}
		void OnEndOfPart() {
			part = new NumericNumberFormatSimple(elements, 0, 0, 0, 0, -1, false, formats.Count == 1);
		}
		void OnBackslash() {
			currentSymbol = formatString[++currentIndex];
			OnDefault();
		}
		void OnQuote() {
			StringBuilder sb = new StringBuilder();
			for (++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (currentSymbol == '"')
					break;
				sb.Append(currentSymbol);
			}
			elements.Add(new NumberFormatQuotedText(sb.ToString()));
		}
		void OnUnderline() {
			elements.Add(new NumberFormatUnderline(formatString[++currentIndex]));
		}
		void OnAsterisk() {
			elements.Add(new NumberFormatAsterisk(formatString[++currentIndex]));
		}
		void OnBracket() {
			int i = currentIndex + 1;
			int closeBrackeIndex = formatString.IndexOf(']', i);
			if (closeBrackeIndex < 0) {
				OnDefault();
				return;
			}
			elementString = formatString.Substring(i, closeBrackeIndex - i);
			element = TryParseColor(elementString);
			if (element == null) {
				locale = TryParseLocale(elementString);
				element = locale;
				if (element == null) {
					int count = elements.Count;
					OnDateTimeSymbol();
					if (part == null) {
						errorState = false;
						currentIndex = i - 1;
						elements.RemoveRange(count, elements.Count - count); 
						element = TryParseExpr(elementString);
						if (element == null) {
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
			elements.Add(new NumberFormatBackslashedText(currentSymbol));
		}
		void OnError() {
			errorState = true;
		}
		void OnESymbol() {
			if (char.IsLower(currentSymbol))
				OnDateTimeSymbol();
			else
				OnError();
		}
		void OnGeneral() {
			part = ParseGeneral();
		}
		void OnGeneralOrDateTime() {
			if (CheckIsGeneral())
				OnGeneral();
			else
				OnDateTimeSymbol();
		}
		void OnDayOfWeekOrDefault() {
			if (OnDayOfWeekCore()) {
				++currentIndex;
				OnDateTimeSymbol();
			}
			else
				OnDefault();
		}
		NumberFormatDesignator ParseSeparator(NumberFormatDesignator designator) {
			if ((designator & NumberFormatDesignator.DateSeparator) > 0)
				return NumberFormatDesignator.DateSeparator;
			if ((designator & NumberFormatDesignator.TimeSeparator) > 0)
				return NumberFormatDesignator.TimeSeparator;
			if ((designator & NumberFormatDesignator.DecimalSeparator) > 0)
				return NumberFormatDesignator.DecimalSeparator;
			if ((designator & NumberFormatDesignator.GroupSeparator) > 0)
				return NumberFormatDesignator.GroupSeparator;
			if ((designator & NumberFormatDesignator.FractionOrDateSeparator) > 0)
				return NumberFormatDesignator.FractionOrDateSeparator;
			return NumberFormatDesignator.Default;
		}
		#endregion
		#region ParseDateTime
		SimpleNumberFormat ParseDateTime() {
			elapsed = false;
			hasMilliseconds = false;
			elementLength = -1;
			for (; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = NumberFormatDesignator.Default;
				if (!parseMethods2.TryGetValue(designator, out designatorParser))
					ParseSeparator2(designator);
				else
					designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart2() {
			if (locale != null) {
				if (locale.HexCode == DateTimeNumberFormatBase.SystemLongDate) {
					part = new DateTimeSystemLongDateNumberFormat(elements);
					return;
				}
				if (locale.HexCode == DateTimeNumberFormatBase.SystemLongTime) {
					part = new DateTimeSystemLongTimeNumberFormat(elements);
					return;
				}
			}
			part = new DateTimeNumberFormat(elements, locale, hasMilliseconds);
		}
		void OnBracket2() {
			TryParseDateTimeCondition(ref elapsed);
		}
		void OnYear() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatYear(elementLength > 2 ? 4 : 2));
		}
		void OnMonthOrMinute() {
			elementLength = GetDateTimeBlockLength();
			if (elementLength > 2)
				elements.Add(new NumberFormatMonth(elementLength > 5 ? 4 : elementLength));
			else {
				bool isMinute = false;
				for (int j = elements.Count - 1; j >= 0; --j) {
					element = elements[j];
					if (element is NumberFormatDateBase) {
						if (element is NumberFormatSeconds || element is NumberFormatHours)
							isMinute = true;
						if (!(element is NumberFormatAmPm))
							break;
					}
				}
				if (isMinute)
					elements.Add(new NumberFormatMinutes(elementLength, false));
				else
					elements.Add(new NumberFormatMonth(elementLength));
			}
		}
		void OnMonth() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatMonth(elementLength > 5 ? 4 : elementLength));
		}
		void OnMinute() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatMinutes(elementLength > 1 ? 2 : 1, false));
		}
		void OnDay() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatDay(elementLength > 4 ? 4 : elementLength));
		}
		void OnHour() {
			bool is12HourTime = false;
			for (int j = elements.Count - 1; j >= 0; --j) {
				element = elements[j];
				if (element is NumberFormatDateBase) {
					if (element is NumberFormatAmPm)
						is12HourTime = true;
					if (!(element is NumberFormatTimeBase) || element is NumberFormatHours)
						break;
				}
			}
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatHours(elementLength > 1 ? 2 : 1, false, is12HourTime));
		}
		void OnSecond() {
			for (int j = elements.Count - 1; j >= 0; --j) {
				NumberFormatDateBase elementBase = elements[j] as NumberFormatDateBase;
				if (elementBase != null) {
					if (elementBase is NumberFormatMonth)
						elements[j] = new NumberFormatMinutes(elementBase.Count, false);
					if (!(elementBase is NumberFormatAmPm))
						break;
				}
			}
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatSeconds(elementLength > 1 ? 2 : 1, false));
		}
		void OnAmPmOrDayOfWeek() {
			if (!OnAmPmCore())
				OnDayOfWeekOrDefault2();
		}
		bool OnAmPmCore() {
			if ((currentIndex + 5) <= formatString.Length && formatString.Substring(currentIndex, 5).ToLowerInvariant() == "am/pm") {
				if (elapsed) {
					errorState = true;
					return true;
				}
				currentIndex += 4;
				elements.Add(new NumberFormatAmPm());
			}
			else
				if ((currentIndex + 3) <= formatString.Length) {
					string ampm = formatString.Substring(currentIndex, 3);
					if (ampm.ToLowerInvariant() == "a/p") {
						if (elapsed) {
							errorState = true;
							return true;
						}
						currentIndex += 2;
						elements.Add(new NumberFormatAmPm(char.IsLower(ampm[0]), char.IsLower(ampm[2])));
					}
					else
						return false;
				}
				else
					return false;
			for (int j = elements.Count - 2; j >= 0; --j) { 
				element = elements[j];
				if (element is NumberFormatTimeBase) {
					NumberFormatHours hours = element as NumberFormatHours;
					if (hours != null) {
						hours.Is12HourTime = true;
						break;
					}
				}
				else
					if (element is NumberFormatDateBase)
						break;
			}
			return true;
		}
		void OnDefaultDateSeparator() {
			elements.Add(NumberFormatDefaultDateSeparator.Instance);
		}
		void OnDateSeparator() {
			elements.Add(NumberFormatDateSeparator.Instance);
		}
		void OnTimeSeparator() {
			elements.Add(NumberFormatTimeSeparator.Instance);
		}
		void OnDateOrTimeSeparator() {
			if (elements.Count > 0 && elements[elements.Count - 1] is NumberFormatTimeBase)
				OnTimeSeparator();
			else
				OnDateSeparator();
		}
		void OnMilisecond() {
			if (!OnMilisecondCore())
				OnDefault();
		}
		bool OnMilisecondCore() {
			elementLength = currentIndex;
			++currentIndex;
			while (formatString.Length > currentIndex)
				if (formatString[currentIndex] == '0')
					++currentIndex;
				else
					break;
			--currentIndex;
			elementLength = currentIndex - elementLength;
			if (elementLength == 0)
				return false;
			if (elementLength > 3)
				errorState = true;
			else {
				hasMilliseconds = true;
				elements.Add(new NumberFormatMilliseconds(elementLength));
			}
			return true;
		}
		void OnESymbol2() {
			if (char.IsLower(currentSymbol))
				OnInvariantYear();
			else
				OnError();
		}
		void OnInvariantYear() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatInvariantYear(elementLength > 1 ? 2 : 1));
		}
		void OnThaiYear() {
			elementLength = GetDateTimeBlockLength();
			++currentIndex;
			if (currentIndex < formatString.Length) {
				char c = formatString[currentIndex];
				if (c == '1' || c == '2') {
					if (elements.Count > 0 || elementLength > 1)
						errorState = true;
					elements.Add(new NumberFormatNotImplementedLocale(c - '0'));
					return;
				}
			}
			--currentIndex;
			elements.Add(new NumberFormatThaiYear(elementLength > 2 ? 4 : 2));
		}
		void OnAmPmOrMonth() {
			if (!OnAmPmCore())
				OnMonth();
		}
		void OnAmPmOrYear() {
			if (!OnAmPmCore())
				OnYear();
		}
		void OnGeneral2() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnDefault();
		}
		void OnGeneralOrDay() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnDay();
		}
		void OnGeneralOrSecond() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnSecond();
		}
		void OnGeneralOrInvariantYear() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnInvariantYear();
		}
		void OnGeneralOrJapaneseEra() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnJapaneseEra();
		}
		void OnJapaneseEra() {
			elementLength = GetDateTimeBlockLength();
			elements.Add(new NumberFormatJapaneseEra(elementLength > 2 ? 3 : elementLength));
		}
		bool OnDayOfWeekCore() {
			int index = currentIndex;
			elementLength = GetDateTimeBlockLength();
			if (elementLength < 3) {
				currentIndex = index;
				elementLength = 1;
				return false;
			}
			elements.Add(new NumberFormatDayOfWeek(elementLength > 3 ? 4 : 3));
			return true;
		}
		void OnDayOfWeekOrDefault2() {
			if (!OnDayOfWeekCore())
				OnDefault();
		}
		void ParseSeparator2(NumberFormatDesignator designator) {
			if ((designator & NumberFormatDesignator.DecimalSeparator) > 0)
				if (OnMilisecondCore())
					return;
			bool isTime = (designator & NumberFormatDesignator.TimeSeparator) > 0;
			if ((designator & NumberFormatDesignator.DateSeparator) > 0)
				if (isTime)
					OnDateOrTimeSeparator();
				else
					OnDateSeparator();
			else if ((designator & NumberFormatDesignator.FractionOrDateSeparator) > 0)
				OnDefaultDateSeparator();
			else if (isTime)
				OnTimeSeparator();
			else if ((designator & NumberFormatDesignator.GroupSeparator) > 0)
				OnDefault();
			else
				OnError();
		}
		#endregion
		#region ParseNumeric
		SimpleNumberFormat ParseNumeric() {
			grouping = false;
			isDecimal = false;
			displayFactor = 0;
			percentCount = 0;
			integerCount = 0;
			decimalCount = 0;
			dividentCount = 0;
			decimalSeparatorIndex = -1;
			preFractionIndex = -1;
			for (; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = NumberFormatDesignator.Default;
				if (!parseMethods3.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods3[designator];
				}
				designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnBracket3() {
			int elementLength = TryParseCondition();
			if (elementLength < 0)
				errorState = true;
		}
		void OnDecimalSeparator() {
			isDecimal = true;
			if (decimalSeparatorIndex < 0)
				decimalSeparatorIndex = elements.Count;
			elements.Add(NumberFormatDecimalSeparator.Instance);
		}
		void BeforeDigit() {
			if (elements.Count > 0)
				if (elements[elements.Count - 1].IsDigit) {
					if (displayFactor == 1)
						grouping = true;
				}
				else {
					preFractionIndex = elements.Count - 1;
					integerCount += dividentCount;
					dividentCount = 0;
				}
			displayFactor = 0;
			if (isDecimal)
				++decimalCount;
			else
				++dividentCount;
		}
		void OnDigitSpace() {
			BeforeDigit();
			elements.Add(NumberFormatDigitSpace.Instance);
		}
		void OnDigitEmpty() {
			BeforeDigit();
			elements.Add(NumberFormatDigitEmpty.Instance);
		}
		void OnDigitZero() {
			BeforeDigit();
			elements.Add(NumberFormatDigitZero.Instance);
		}
		void OnEndOfPart3() {
			integerCount += dividentCount;
			PrepareGrouping();
			part = new NumericNumberFormatSimple(elements, percentCount, integerCount, decimalCount, displayFactor, decimalSeparatorIndex, grouping, formats.Count == 1);
		}
		void OnExponent() {
			integerCount += dividentCount;
			PrepareGrouping();
			part = ParseNumericExponent(integerCount, decimalSeparatorIndex, decimalCount, grouping);
		}
		void OnGroupSeparator() {
			if (elements.Count > 0 && elements[elements.Count - 1].IsDigit)
				++displayFactor;
			else
				OnDefault();
		}
		void OnPercent() {
			++percentCount;
			elements.Add(NumberFormatPercent.Instance);
		}
		void OnFractionSeparator() {
			if (isDecimal || dividentCount <= 0) {
				errorState = true;
				return;
			}
			if (integerCount > 0)
				PrepareGrouping();
			else
				if (grouping) {
					errorState = true;
					return;
				}
			part = ParseNumericFraction(integerCount, preFractionIndex, dividentCount, percentCount, grouping);
		}
		void OnESymbol3() {
			if (char.IsLower(currentSymbol))
				OnError();
			else
				OnExponent();
		}
		void OnGeneral3() {
			if (CheckIsGeneral())
				errorState = true;
			else
				OnDefault();
		}
		void OnDayOfWeekOrDefault3() {
			if (OnDayOfWeekCore()) {
				elements.RemoveAt(elements.Count - 1);
				OnError();
			}
			else
				OnDefault();
		}
		NumberFormatDesignator ParseSeparator3(NumberFormatDesignator designator) {
			if ((designator & NumberFormatDesignator.DecimalSeparator) > 0)
				return NumberFormatDesignator.DecimalSeparator;
			if ((designator & NumberFormatDesignator.GroupSeparator) > 0)
				return NumberFormatDesignator.GroupSeparator;
			if ((designator & NumberFormatDesignator.FractionOrDateSeparator) > 0)
				return NumberFormatDesignator.FractionOrDateSeparator;
			if ((designator & NumberFormatDesignator.DateSeparator) > 0)
				return NumberFormatDesignator.DateSeparator;
			if ((designator & NumberFormatDesignator.TimeSeparator) > 0)
				return NumberFormatDesignator.TimeSeparator;
			return NumberFormatDesignator.Default;
		}
		void PrepareGrouping() {
			int integerCountOld = integerCount;
			if (grouping && integerCount < 4) {
				int i;
				INumberFormatElement element;
				for (i = 0; i < elements.Count; ++i) {
					element = elements[i];
					if (element.IsDigit)
						break;
				}
				while (integerCount < 4) {
					elements.Insert(i, NumberFormatDigitEmpty.Instance);
					++integerCount;
				}
				if (decimalSeparatorIndex >= 0)
					decimalSeparatorIndex += integerCount - integerCountOld;
			}
		}
		#endregion
		#region ParseNumericExponent
		SimpleNumberFormat ParseNumericExponent(int integerCount, int decimalSeparatorIndex, int decimalCount, bool grouping) {
			++currentIndex;
			if (currentIndex + 1 >= formatString.Length)
				return null;
			currentSymbol = formatString[currentIndex];
			if (currentSymbol == '+')
				explicitSign = true;
			else {
				explicitSign = false;
				if (currentSymbol != '-')
					return null;
			}
			expIndex = elements.Count;
			expCount = 0;
			for (++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = NumberFormatDesignator.Default;
				if (!parseMethods4.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods4[designator];
				}
				designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnDigitEmpty4() {
			++expCount;
			elements.Add(NumberFormatDigitEmpty.Instance);
		}
		void OnDigitZero4() {
			++expCount;
			elements.Add(NumberFormatDigitZero.Instance);
		}
		void OnDigitSpace4() {
			++expCount;
			elements.Add(NumberFormatDigitSpace.Instance);
		}
		void OnEndOfPart4() {
			part = new NumericNumberFormatExponent(elements, integerCount, decimalCount, decimalSeparatorIndex, expIndex, expCount, explicitSign, grouping, formats.Count == 1);
		}
		#endregion
		#region ParseNumericFraction
		SimpleNumberFormat ParseNumericFraction(int integerCount, int preFractionIndex, int dividentCount, int percentCount, bool grouping) {
			displayFactor = -1; 
			divisorCount = 0;
			divisor = 0;
			divisorPow = 10000;
			fractionSeparatorIndex = elements.Count;
			for (++currentIndex; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator)) {
					if (char.IsNumber(currentSymbol)) {
						if (divisorPow <= 0)
							return null;
						divisor += ((int)currentSymbol - 48) * divisorPow;
						divisorPow /= 10;
						displayFactor = elements.Count;
						continue;
					}
					designator = NumberFormatDesignator.Default;
				}
				if (!parseMethods5.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator3(designator);
					designatorParser = parseMethods5[designator];
				}
				designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart5() {
			if (divisor > 0) {
				while (divisor % 10 == 0)
					divisor /= 10;
				part = new NumericNumberFormatFractionExcplicit(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, displayFactor, dividentCount, divisor, grouping, formats.Count == 1);
				return;
			}
			if (divisorCount == 0) {
				errorState = true;
				return;
			}
			part = new NumericNumberFormatFraction(elements, percentCount, integerCount, preFractionIndex, fractionSeparatorIndex, displayFactor, dividentCount, divisorCount, grouping, formats.Count == 1);
		}
		void OnDigitSpace5() {
			++divisorCount;
			elements.Add(NumberFormatDigitSpace.Instance);
			displayFactor = elements.Count;
		}
		void OnDigitZero5() {
			if (divisor > 0)
				divisorPow *= 10;
			else {
				++divisorCount;
				elements.Add(NumberFormatDigitZero.Instance);
				displayFactor = elements.Count;
			}
		}
		void OnDigitEmpty5() {
			++divisorCount;
			elements.Add(NumberFormatDigitEmpty.Instance);
			displayFactor = elements.Count;
		}
		#endregion
		#region ParseText
		SimpleNumberFormat ParseText() {
			for (; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator)) {
					if (char.IsNumber(currentSymbol)) {
						part = null;
						break;
					}
					designator = NumberFormatDesignator.Default;
				}
				if (!parseMethods6.TryGetValue(designator, out designatorParser)) {
					designator = ParseSeparator6(designator);
					designatorParser = parseMethods6[designator];
				}
				designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnAt6() {
			elements.Add(NumberFormatTextContent.Instance);
		}
		void OnEndOfPart6() {
			part = new TextNumberFormat(elements);
		}
		NumberFormatDesignator ParseSeparator6(NumberFormatDesignator designator) {
			if ((designator & NumberFormatDesignator.FractionOrDateSeparator) > 0)
				return NumberFormatDesignator.FractionOrDateSeparator;
			if ((designator & NumberFormatDesignator.DecimalSeparator) > 0)
				return NumberFormatDesignator.DecimalSeparator;
			if ((designator & NumberFormatDesignator.GroupSeparator) > 0)
				return NumberFormatDesignator.GroupSeparator;
			if ((designator & NumberFormatDesignator.DateSeparator) > 0)
				return NumberFormatDesignator.DateSeparator;
			if ((designator & NumberFormatDesignator.TimeSeparator) > 0)
				return NumberFormatDesignator.TimeSeparator;
			return NumberFormatDesignator.Default;
		}
		#endregion
		#region ParseGeneral
		SimpleNumberFormat ParseGeneral() {
			fractionSeparatorIndex = -1;
			for (; currentIndex < formatString.Length; ++currentIndex) {
				currentSymbol = formatString[currentIndex];
				if (!localizer.Designators.TryGetValue(char.ToLowerInvariant(currentSymbol), out designator))
					designator = NumberFormatDesignator.Default;
				if (!parseMethods7.TryGetValue(designator, out designatorParser))
					designatorParser = OnDefault;
				designatorParser();
				if (errorState)
					return null;
				if (currentSymbol == ';')
					return part;
			}
			if (part == null)
				errorState = true;
			return part;
		}
		void OnEndOfPart7() {
			if (fractionSeparatorIndex < 0)
				errorState = true;
			part = new GeneralNumberFormat(elements, fractionSeparatorIndex);
		}
		void OnGeneral7() {
			if (CheckIsGeneral())
				OnGeneralCore();
			else
				OnDefault();
		}
		void OnGeneralOrDateTime7() {
			if (CheckIsGeneral())
				OnGeneralCore();
			else
				errorState = true;
		}
		void OnGeneralCore() {
			if (fractionSeparatorIndex >= 0) {
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
			if (closeBrackeIndex < 0)
				return null;
			int elementLength = closeBrackeIndex - i;
			return formatString.Substring(i, elementLength);
		}
		int TryParseCondition() {
			string elementString = TryGetConditionString();
			if (string.IsNullOrEmpty(elementString)) {
				OnDefault();
				return 1;
			}
			element = TryParseColor(elementString);
			if (element == null) {
				element = TryParseLocale(elementString);
				if (element == null) {
					element = TryParseExpr(elementString);
					if (element == null)
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
			if (string.IsNullOrEmpty(elementString)) {
				OnDefault();
				return 1;
			}
			element = TryParseColor(elementString);
			if (element == null) {
				element = TryParseLocale(elementString);
				if (element == null) {
					element = TryParseElapsed(elementString);
					if (element == null) {
						element = TryParseExpr(elementString);
						if (element == null)
							return -1;
						else
							elements.Add(element);
					}
					else {
						if (elapsed)
							return -1;
						elapsed = true;
						elements.Add(element);
					}
				}
				else {
					if (locale != null)
						return -1;
					else
						locale = element as NumberFormatDisplayLocale;
					elements.Add(element);
				}
			}
			else
				elements.Insert(0, element);
			currentIndex += elementString.Length + 1;
			return elementString.Length + 2; 
		}
		NumberFormatColor TryParseColor(string colorString) {
			Color color = Color.FromName(colorString);
			return AllowedColors.Contains(color) ? new NumberFormatColor(color) : null;
		}
		NumberFormatDisplayLocale TryParseLocale(string localeString) {
			if (localeString[0] == '$') {
				int dashIndex = localeString.IndexOf('-', 1);
				if (dashIndex > 0) {
					string currency = localeString.Substring(1, dashIndex - 1);
					localeString = localeString.Remove(0, dashIndex + 1);
					int localeCode;
					if (int.TryParse(localeString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out localeCode))
						return new NumberFormatDisplayLocale(localeCode, currency);
				}
				else
					return new NumberFormatDisplayLocale(-1, localeString.Remove(0, 1));
			}
			return null;
		}
		NumberFormatTimeBase TryParseElapsed(string elapsedString) {
			NumberFormatTimeBase result = null;
			currentSymbol = char.ToLowerInvariant(elapsedString[0]);
			if (currentSymbol == 'h' || currentSymbol == 'm' || currentSymbol == 's') {
				int blockLength = GetDateTimeBlockLength(elapsedString, 0);
				if (blockLength == elapsedString.Length) {
					switch (currentSymbol) {
						case 's':
							result = new NumberFormatSeconds(blockLength, true);
							break;
						case 'm':
							result = new NumberFormatMinutes(blockLength, true);
							break;
						case 'h':
							result = new NumberFormatHours(blockLength, true, false); 
							break;
					}
				}
			}
			return result;
		}
		NumberFormatExprCondiiton TryParseExpr(string expression) {
			return new NumberFormatExprCondiiton(expression);
		}
		int GetDateTimeBlockLength() {
			int elementLength = GetDateTimeBlockLength(formatString, currentIndex);
			currentIndex += elementLength - 1;
			return elementLength;
		}
		int GetDateTimeBlockLength(string formatString, int currentIndex) {
			int startIndex = currentIndex;
			currentSymbol = char.ToLowerInvariant(currentSymbol);
			for (; currentIndex < formatString.Length; ++currentIndex)
				if (char.ToLowerInvariant(formatString[currentIndex]) != currentSymbol) {
					--currentIndex;
					return currentIndex - startIndex + 1;
				}
			return formatString.Length - startIndex;
		}
		bool CheckIsGeneral() {
			string general = localizer.GeneralDesignator;
			if (formatString.Length < currentIndex + general.Length)
				return false;
			string posibleGeneral = formatString.Substring(currentIndex, general.Length);
			return string.Compare(general, posibleGeneral, StringComparison.OrdinalIgnoreCase) == 0;
		}
		#endregion
	}
	#endregion
	#region NumberFormatLocaliser
	public class NumberFormatLocalizer {
		delegate string TableGenerator(Dictionary<char, NumberFormatDesignator> designators);
		static Dictionary<string, TableGenerator> tableGenerators = Generate();
		CultureInfo lastCulture;
		string generalDesignator;
		Dictionary<char, NumberFormatDesignator> designators = new Dictionary<char, NumberFormatDesignator>();
		Dictionary<NumberFormatDesignator, char> chars = new Dictionary<NumberFormatDesignator, char>();
		public CultureInfo Culture { get { return lastCulture; } }
		public Dictionary<char, NumberFormatDesignator> Designators { get { return designators; } }
		public Dictionary<NumberFormatDesignator, char> Chars { get { return chars; } }
		public string GeneralDesignator { get { return generalDesignator; } }
		public void SetCulture(CultureInfo culture) {
			if (lastCulture == culture)
				return;
			lastCulture = culture;
			designators.Clear();
			chars.Clear();
			GenerateDesignators(culture);
			foreach (char key in designators.Keys) {
				int count = chars.Count;
				NumberFormatDesignator designator = designators[key];
				if ((designator & NumberFormatDesignator.AmPm) > 0)
					chars.Add(NumberFormatDesignator.AmPm, key);
				if ((designator & NumberFormatDesignator.Year) > 0)
					chars.Add(NumberFormatDesignator.Year, key);
				if ((designator & NumberFormatDesignator.InvariantYear) > 0)
					chars.Add(NumberFormatDesignator.InvariantYear, key);
				if ((designator & NumberFormatDesignator.Month) > 0)
					chars.Add(NumberFormatDesignator.Month, key);
				if ((designator & NumberFormatDesignator.Minute) > 0)
					chars.Add(NumberFormatDesignator.Minute, key);
				if ((designator & NumberFormatDesignator.DateSeparator) > 0)
					chars.Add(NumberFormatDesignator.DateSeparator, key);
				if ((designator & NumberFormatDesignator.DecimalSeparator) > 0)
					chars.Add(NumberFormatDesignator.DecimalSeparator, key);
				if ((designator & NumberFormatDesignator.FractionOrDateSeparator) > 0)
					chars.Add(NumberFormatDesignator.FractionOrDateSeparator, key);
				if ((designator & NumberFormatDesignator.GroupSeparator) > 0)
					chars.Add(NumberFormatDesignator.GroupSeparator, key);
				if ((designator & NumberFormatDesignator.TimeSeparator) > 0)
					chars.Add(NumberFormatDesignator.TimeSeparator, key);
				if ((designator & NumberFormatDesignator.Day) > 0)
					chars.Add(NumberFormatDesignator.Day, key);
				if ((designator & NumberFormatDesignator.Second) > 0)
					chars.Add(NumberFormatDesignator.Second, key);
				if ((designator & NumberFormatDesignator.General) > 0)
					chars.Add(NumberFormatDesignator.General, key);
				if ((designator & NumberFormatDesignator.JapaneseEra) > 0)
					chars.Add(NumberFormatDesignator.JapaneseEra, key);
				if ((designator & NumberFormatDesignator.DayOfWeek) > 0)
					chars.Add(NumberFormatDesignator.DayOfWeek, key);
				if ((designator & NumberFormatDesignator.Hour) > 0)
					chars.Add(NumberFormatDesignator.Hour, key);
				if (chars.Count == count)
					chars.Add(designator, key);
			}
		}
		void GenerateDesignators(CultureInfo culture) {
			char decimalSeparator = culture.NumberFormat.NumberDecimalSeparator[0];
			char dateSeparator = culture.GetDateSeparator()[0];
			char groupSeparator = culture.NumberFormat.NumberGroupSeparator[0];
			if (groupSeparator == decimalSeparator) {
				if (decimalSeparator == '.')
					groupSeparator = ',';
				if (decimalSeparator == ',')
					groupSeparator = '.';
			}
			char timeSeparator = culture.GetTimeSeparator()[0];
			if (char.IsWhiteSpace(groupSeparator))
				groupSeparator = ' ';
			AddSeparator(decimalSeparator, NumberFormatDesignator.DecimalSeparator);
			AddSeparator(dateSeparator, NumberFormatDesignator.DateSeparator);
			AddSeparator(groupSeparator, NumberFormatDesignator.GroupSeparator);
			AddSeparator(timeSeparator, NumberFormatDesignator.TimeSeparator);
			AddSeparator('/', NumberFormatDesignator.FractionOrDateSeparator);
			designators.Add('a', NumberFormatDesignator.AmPm);
			designators.Add('*', NumberFormatDesignator.Asterisk);
			designators.Add('@', NumberFormatDesignator.At);
			designators.Add('\\', NumberFormatDesignator.Backslash);
			designators.Add('[', NumberFormatDesignator.Bracket);
			designators.Add('#', NumberFormatDesignator.DigitEmpty);
			designators.Add('?', NumberFormatDesignator.DigitSpace);
			designators.Add('0', NumberFormatDesignator.DigitZero);
			designators.Add(';', NumberFormatDesignator.EndOfPart);
			designators.Add('E', NumberFormatDesignator.Exponent);
			designators.Add('e', NumberFormatDesignator.InvariantYear);
			designators.Add('%', NumberFormatDesignator.Percent);
			designators.Add('"', NumberFormatDesignator.Quote);
			designators.Add('b', NumberFormatDesignator.ThaiYear);
			designators.Add('_', NumberFormatDesignator.Underline);
			TableGenerator generate;
			if (!tableGenerators.TryGetValue(culture.EnglishName, out generate) && !tableGenerators.TryGetValue(culture.Name, out generate))
				generate = GenerateInvariant;
			generalDesignator = generate(designators);
		}
		void AddSeparator(char symbol, NumberFormatDesignator designator) {
			if (designators.ContainsKey(symbol))
				designators[symbol] |= designator;
			else
				designators.Add(symbol, designator);
		}
		static Dictionary<string, TableGenerator> Generate() {
			Dictionary<string, TableGenerator> result = new Dictionary<string, TableGenerator>();
			result.Add("Bashkir (Russia)", GenerateRussian);
			result.Add("ba-RU", GenerateRussian);
			result.Add("Belarusian (Belarus)", GenerateRussian);
			result.Add("be-BY", GenerateRussian);
			result.Add("Kazakh (Kazakhstan)", GenerateRussian);
			result.Add("kk-KZ", GenerateRussian);
			result.Add("Russian (Russia)", GenerateRussian);
			result.Add("ru-RU", GenerateRussian);
			result.Add("Sakha (Russia)", GenerateRussian);
			result.Add("sah-RU", GenerateRussian);
			result.Add("Tatar (Russia)", GenerateRussian);
			result.Add("tt-RU", GenerateRussian);
			result.Add("Uzbek (Cyrillic, Uzbekistan)", GenerateRussian);
			result.Add("uz-Cyrl-UZ", GenerateRussian);
			result.Add("Uzbek (Latin, Uzbekistan)", GenerateRussian);
			result.Add("uz-Latn-UZ", GenerateRussian);
			result.Add("Alsatian (France)", GenerateFrench);
			result.Add("gsw-FR", GenerateFrench);
			result.Add("Breton (France)", GenerateFrench);
			result.Add("br-FR", GenerateFrench);
			result.Add("Corsican (France)", GenerateFrench);
			result.Add("co-FR", GenerateFrench);
			result.Add("French (Belgium)", GenerateFrench);
			result.Add("fr-BE", GenerateFrench);
			result.Add("French (Canada)", GenerateFrench);
			result.Add("fr-CA", GenerateFrench);
			result.Add("French (France)", GenerateFrench);
			result.Add("fr-FR", GenerateFrench);
			result.Add("French (Switzerland)", GenerateFrench);
			result.Add("fr-CH", GenerateFrench);
			result.Add("Occitan (France)", GenerateFrench);
			result.Add("oc-FR", GenerateFrench);
			result.Add("Basque (Basque)", GenerateSpanish);
			result.Add("eu-ES", GenerateSpanish);
			result.Add("Catalan (Catalan)", GenerateSpanish);
			result.Add("ca-ES", GenerateSpanish);
			result.Add("Galician (Galician)", GenerateSpanish);
			result.Add("gl-ES", GenerateSpanish);
			result.Add("Spanish (Mexico)", GenerateSpanish);
			result.Add("es-MX", GenerateSpanish);
			result.Add("Spanish (Spain, International Sort)", GenerateSpanish);
			result.Add("es-ES", GenerateSpanish);
			result.Add("Valencian (Spain)", GenerateSpanish);
			result.Add("Portuguese (Brazil)", GeneratePortugueseBrazil);
			result.Add("pt-BR", GeneratePortugueseBrazil);
			result.Add("Portuguese (Portugal)", GeneratePortuguesePortugal);
			result.Add("pt-PT", GeneratePortuguesePortugal);
			result.Add("Czech (Czech Republic)", GenerateCzech);
			result.Add("cs-CZ", GenerateCzech);
			result.Add("Danish (Denmark)", GenereateDanish);
			result.Add("da-DK", GenereateDanish);
			result.Add("Norwegian, Bokmål (Norway)", GenereateDanish);
			result.Add("nb-NO", GenereateDanish);
			result.Add("Norwegian, Nynorsk (Norway)", GenereateDanish);
			result.Add("nn-NO", GenereateDanish);
			result.Add("Sami, Lule (Norway)", GenereateDanish);
			result.Add("smj-NO", GenereateDanish);
			result.Add("Sami, Lule (Sweden)", GenereateDanish);
			result.Add("smj-SE", GenereateDanish);
			result.Add("Sami, Northern (Norway)", GenereateDanish);
			result.Add("se-NO", GenereateDanish);
			result.Add("Sami, Northern (Sweden)", GenereateDanish);
			result.Add("se-SE", GenereateDanish);
			result.Add("Sami, Southern (Norway)", GenereateDanish);
			result.Add("sma-NO", GenereateDanish);
			result.Add("Sami, Southern (Sweden)", GenereateDanish);
			result.Add("sma-SE", GenereateDanish);
			result.Add("Swedish (Sweden)", GenereateDanish);
			result.Add("sv-SE", GenereateDanish);
			result.Add("Dutch (Belgium)", GenerateDutch);
			result.Add("nl-BE", GenerateDutch);
			result.Add("Dutch (Netherlands)", GenerateDutch);
			result.Add("nl-NL", GenerateDutch);
			result.Add("Frisian (Netherlands)", GenerateDutch);
			result.Add("fy-NL", GenerateDutch);
			result.Add("Finnish (Finland)", GenerateFinnish);
			result.Add("fi-FI", GenerateFinnish);
			result.Add("Sami, Inari (Finland)", GenerateFinnish);
			result.Add("smn-FI", GenerateFinnish);
			result.Add("Sami, Northern (Finland)", GenerateFinnish);
			result.Add("se-FI", GenerateFinnish);
			result.Add("Sami, Skolt (Finland)", GenerateFinnish);
			result.Add("sms-FI", GenerateFinnish);
			result.Add("Swedish (Finland)", GenerateFinnish);
			result.Add("sv-FI", GenerateFinnish);
			result.Add("German (Austria)", GenerateGerman);
			result.Add("de-AT", GenerateGerman);
			result.Add("German (Germany)", GenerateGerman);
			result.Add("de-DE", GenerateGerman);
			result.Add("German (Liechtenstein)", GenerateGerman);
			result.Add("de-LI", GenerateGerman);
			result.Add("German (Switzerland)", GenerateGerman);
			result.Add("de-CH", GenerateGerman);
			result.Add("Lower Sorbian (Germany)", GenerateGerman);
			result.Add("dsb-DE", GenerateGerman);
			result.Add("Upper Sorbian (Germany)", GenerateGerman);
			result.Add("hsb-DE", GenerateGerman);
			result.Add("Greek (Greece)", GenerateGreek);
			result.Add("el-GR", GenerateGreek);
			result.Add("Hungarian (Hungary)", GenerateHungarian);
			result.Add("hu-HU", GenerateHungarian);
			result.Add("Italian (Italy)", GenerateItalian);
			result.Add("it-IT", GenerateItalian);
			result.Add("Italian (Switzerland)", GenerateItalian);
			result.Add("it-CH", GenerateItalian);
			result.Add("Polish (Poland)", GeneratePolish);
			result.Add("pl-PL", GeneratePolish);
			result.Add("Turkish (Turkey)", GenerateTurkish);
			result.Add("tr-TR", GenerateTurkish);
			#region Invariant
			#endregion
			return result;
		}
		static string GenerateInvariant(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators.Add('y', NumberFormatDesignator.Year);
			designators.Add('g', NumberFormatDesignator.JapaneseEra | NumberFormatDesignator.General);
			return "General";
		}
		static string GenerateRussian(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('д', NumberFormatDesignator.Day);
			designators.Add('ч', NumberFormatDesignator.Hour);
			designators.Add('м', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('с', NumberFormatDesignator.Second);
			designators.Add('г', NumberFormatDesignator.Year);
			designators.Add('о', NumberFormatDesignator.General);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Основной";
		}
		static string GenerateFrench(Dictionary<char, NumberFormatDesignator> designators) {
			designators.Add('o', NumberFormatDesignator.DayOfWeek);
			designators['a'] |= NumberFormatDesignator.Year;
			designators.Add('j', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Standard";
		}
		static string GenerateSpanish(Dictionary<char, NumberFormatDesignator> designators) {
			designators.Add('o', NumberFormatDesignator.DayOfWeek);
			designators['a'] |= NumberFormatDesignator.Year;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators['e'] |= NumberFormatDesignator.General;
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Estándar";
		}
		static string GeneratePortugueseBrazil(Dictionary<char, NumberFormatDesignator> designators) {
			designators.Add('o', NumberFormatDesignator.DayOfWeek);
			designators['a'] |= NumberFormatDesignator.Year;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators.Add('g', NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra);
			return "Geral";
		}
		static string GeneratePortuguesePortugal(Dictionary<char, NumberFormatDesignator> designators) {
			designators.Add('o', NumberFormatDesignator.DayOfWeek);
			designators['a'] |= NumberFormatDesignator.Year;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators['e'] |= NumberFormatDesignator.General;
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Estandar";
		}
		static string GenerateCzech(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators.Add('r', NumberFormatDesignator.Year);
			designators.Add('v', NumberFormatDesignator.General);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Vęeobecný";
		}
		static string GenereateDanish(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('t', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('å', NumberFormatDesignator.Year);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Standard";
		}
		static string GenerateDutch(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('u', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('j', NumberFormatDesignator.Year);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Standaard";
		}
		static string GenerateFinnish(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('p', NumberFormatDesignator.Day);
			designators.Add('t', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute);
			designators.Add('k', NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second);
			designators.Add('v', NumberFormatDesignator.Year);
			designators.Add('y', NumberFormatDesignator.General);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Yleinen";
		}
		static string GenerateGerman(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('t', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('j', NumberFormatDesignator.Year);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Standard";
		}
		static string GenerateGreek(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('η', NumberFormatDesignator.Day);
			designators.Add('ω', NumberFormatDesignator.Hour);
			designators.Add('λ', NumberFormatDesignator.Minute);
			designators.Add('μ', NumberFormatDesignator.Month);
			designators.Add('δ', NumberFormatDesignator.Second);
			designators.Add('ε', NumberFormatDesignator.Year);
			designators.Add('γ', NumberFormatDesignator.General);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Γενικός τύπος";
		}
		static string GenerateHungarian(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('n', NumberFormatDesignator.Day | NumberFormatDesignator.General);
			designators.Add('ó', NumberFormatDesignator.Hour);
			designators.Add('p', NumberFormatDesignator.Minute);
			designators.Add('h', NumberFormatDesignator.Month);
			designators.Add('m', NumberFormatDesignator.Second);
			designators.Add('é', NumberFormatDesignator.Year);
			designators.Add('g', NumberFormatDesignator.JapaneseEra);
			return "Normál";
		}
		static string GenerateItalian(Dictionary<char, NumberFormatDesignator> designators) {
			designators.Add('o', NumberFormatDesignator.DayOfWeek);
			designators['a'] |= NumberFormatDesignator.Year;
			designators.Add('g', NumberFormatDesignator.Day);
			designators.Add('h', NumberFormatDesignator.Hour);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('x', NumberFormatDesignator.JapaneseEra);
			return "Standard";
		}
		static string GeneratePolish(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.DayOfWeek;
			designators.Add('d', NumberFormatDesignator.Day);
			designators.Add('g', NumberFormatDesignator.Hour | NumberFormatDesignator.JapaneseEra);
			designators.Add('m', NumberFormatDesignator.Minute | NumberFormatDesignator.Month);
			designators.Add('s', NumberFormatDesignator.Second | NumberFormatDesignator.General);
			designators.Add('r', NumberFormatDesignator.Year);
			return "Standardowy";
		}
		static string GenerateTurkish(Dictionary<char, NumberFormatDesignator> designators) {
			designators['a'] |= NumberFormatDesignator.Month | NumberFormatDesignator.DayOfWeek;
			designators.Add('g', NumberFormatDesignator.Day | NumberFormatDesignator.General | NumberFormatDesignator.JapaneseEra);
			designators.Add('s', NumberFormatDesignator.Hour);
			designators.Add('d', NumberFormatDesignator.Minute);
			designators.Add('n', NumberFormatDesignator.Second);
			designators.Add('y', NumberFormatDesignator.Year);
			return "Genel";
		}
	}
	#endregion
	#region DateTimeNumberFormatElements
	class NumberFormatDefaultDateSeparator : NumberFormatDecimalSeparator {
		public new static NumberFormatDefaultDateSeparator Instance = new NumberFormatDefaultDateSeparator();
		protected NumberFormatDefaultDateSeparator() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.FractionOrDateSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return "/";
		}
	}
	class NumberFormatDateSeparator : NumberFormatDecimalSeparator {
		public new static NumberFormatDateSeparator Instance = new NumberFormatDateSeparator();
		protected NumberFormatDateSeparator() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.DateSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.GetDateSeparator();
		}
	}
	class NumberFormatTimeSeparator : NumberFormatDateSeparator {
		public new static NumberFormatTimeSeparator Instance = new NumberFormatTimeSeparator();
		protected NumberFormatTimeSeparator() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.TimeSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.GetTimeSeparator();
		}
	}
	abstract class NumberFormatDateBase : NumberFormatElementBase {
		int count;
		protected NumberFormatDateBase(int count) {
			this.count = count;
		}
		public int Count { get { return count; } }
		protected bool IsDefaultNetDateTimeFormat { get { return count == 1; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append(NumberFormatParser.Localizer.Chars[Designator], Count);
		}
	}
	class NumberFormatJapaneseEra : NumberFormatDateBase {
		public NumberFormatJapaneseEra(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.JapaneseEra; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatJapaneseEra other = obj as NumberFormatJapaneseEra;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatJapaneseEra(Count);
		}
	}
	class NumberFormatYear : NumberFormatDateBase {
		public NumberFormatYear(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Year; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = GetDateTime(value, context);
			string year = dateTime.Year.ToString();
			result.Text += year.Substring(year.Length - Count, Count);
		}
		protected virtual DateTime GetDateTime(VariantValue value, WorkbookDataContext context) {
			return context.FromDateTimeSerialForMonthName(value.NumericValue);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatYear other = obj as NumberFormatYear;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatYear(Count);
		}
	}
	class NumberFormatInvariantYear : NumberFormatYear {
		public NumberFormatInvariantYear(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.InvariantYear; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = GetDateTime(value, context);
			result.Text += dateTime.Year.ToString();
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatInvariantYear other = obj as NumberFormatInvariantYear;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatInvariantYear(Count);
		}
	}
	class NumberFormatThaiYear : NumberFormatYear {
		public NumberFormatThaiYear(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.ThaiYear; } }
		protected override DateTime GetDateTime(VariantValue value, WorkbookDataContext context) {
			DateTime result = base.GetDateTime(value, context);
			return result.AddYears(543);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatThaiYear other = obj as NumberFormatThaiYear;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatThaiYear(Count);
		}
	}
	class NumberFormatMonth : NumberFormatDateBase {
		public NumberFormatMonth(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Month; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = context.FromDateTimeSerialForMonthName(value.NumericValue);
			string month;
			if (IsDefaultNetDateTimeFormat)
				month = dateTime.ToString("%M", context.Culture);
			else {
				month = dateTime.ToString(new string('M', Count), context.Culture);
				if (Count == 3 && month.Length > Count)
					month = month.Substring(0, Count);
				if (Count == 5)
					month = month.Substring(0, 1);
			}
			result.Text += month;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatMonth other = obj as NumberFormatMonth;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatMonth(Count);
		}
	}
	class NumberFormatDayOfWeek : NumberFormatDateBase {
		public NumberFormatDayOfWeek(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.DayOfWeek; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = context.FromDateTimeSerialForDayOfWeek(value.NumericValue);
			string dayOfWeek = dateTime.ToString(new string('d', Count), context.Culture);
			result.Text += dayOfWeek;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatDayOfWeek other = obj as NumberFormatDayOfWeek;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatDayOfWeek(Count);
		}
	}
	class NumberFormatDay : NumberFormatDateBase {
		public NumberFormatDay(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Day; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime;
			if (Count <= 2) {
				double numericValue = value.NumericValue;
				if (numericValue < 1) {
					result.Text += new string('0', Count);
					return;
				}
				else if ((int)numericValue == 60) {
					result.Text += "29";
					return;
				}
				dateTime = context.FromDateTimeSerial(value.NumericValue);
			}
			else
				dateTime = context.FromDateTimeSerialForDayOfWeek(value.NumericValue);
			string day;
			if (IsDefaultNetDateTimeFormat)
				day = dateTime.ToString("%d", context.Culture);
			else
				day = dateTime.ToString(new string('d', Count), context.Culture);
			result.Text += day;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatDay other = obj as NumberFormatDay;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatDay(Count);
		}
	}
	class NumberFormatAmPm : NumberFormatDateBase {
		bool amIsLower;
		bool pmIsLower;
		public NumberFormatAmPm()
			: base(2) {
		}
		public NumberFormatAmPm(bool amIsLower, bool pmIsLower)
			: base(1) {
			this.amIsLower = amIsLower;
			this.pmIsLower = pmIsLower;
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.AmPm; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append(GetDesignator(true));
			builder.Append('/');
			builder.Append(GetDesignator(false));
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			double numericValue = value.NumericValue;
			numericValue = numericValue - Math.Floor(numericValue);
			result.Text += GetDesignator(numericValue < 0.5);
		}
		string GetDesignator(bool isAM) { 
			if (isAM)
				if (IsDefaultNetDateTimeFormat)
					return amIsLower ? "a" : "A";
				else
					return amIsLower ? "am" : "AM";
			else
				if (IsDefaultNetDateTimeFormat)
					return pmIsLower ? "p" : "P";
				else
					return pmIsLower ? "pm" : "PM";
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatAmPm other = obj as NumberFormatAmPm;
			if (other == null || Count != other.Count || amIsLower != other.amIsLower || pmIsLower != other.pmIsLower)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return Count == 2 ? new NumberFormatAmPm() : new NumberFormatAmPm(amIsLower, pmIsLower);
		}
	}
	class NumberFormatMilliseconds : NumberFormatDateBase {
		public NumberFormatMilliseconds(int count)
			: base(count) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.DigitZero; } }
		public override void AppendFormat(StringBuilder builder) {
			NumberFormatDecimalSeparator.Instance.AppendFormat(builder);
			base.AppendFormat(builder);
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = value.ToDateTime(context);
			double millisecond = Math.Round((double)dateTime.Millisecond / 1000, Count, MidpointRounding.AwayFromZero);
			result.Text += millisecond.ToString('.' + new string('0', Count), context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatMilliseconds other = obj as NumberFormatMilliseconds;
			if (other == null || Count != other.Count)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatMilliseconds(Count);
		}
	}
	abstract class NumberFormatTimeBase : NumberFormatDateBase {
		bool elapsed;
		public NumberFormatTimeBase(int count, bool elapsed)
			: base(count) {
			this.elapsed = elapsed;
		}
		protected bool Elapsed { get { return elapsed; } }
		public override void AppendFormat(StringBuilder builder) {
			if (elapsed) {
				builder.Append('[');
				base.AppendFormat(builder);
				builder.Append(']');
			}
			else
				base.AppendFormat(builder);
		}
	}
	class NumberFormatHours : NumberFormatTimeBase {
		bool is12HourTime;
		public NumberFormatHours(int count, bool elapsed, bool is12HourTime)
			: base(count, elapsed) {
			this.is12HourTime = is12HourTime;
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Hour; } }
		public bool Is12HourTime { get { return is12HourTime; } set { is12HourTime = value; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			long hour = Elapsed ? (long)TimeSpan.FromDays(value.NumericValue).TotalHours : value.ToDateTime(context).Hour;
			if (Is12HourTime) {
				hour = hour % 12;
				if (hour == 0)
					hour = 12;
			}
			result.Text += hour.ToString(new string('0', Count), context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatHours other = obj as NumberFormatHours;
			if (other == null || Count != other.Count || Elapsed != other.Elapsed || is12HourTime != other.is12HourTime)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatHours(Count, Elapsed, is12HourTime);
		}
	}
	class NumberFormatMinutes : NumberFormatTimeBase {
		public NumberFormatMinutes(int count, bool elapsed)
			: base(count, elapsed) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Minute; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			string actualString;
			if (Elapsed) {
				long actualValue = (long)(value.NumericValue * 24 * 60);
				actualString = actualValue.ToString(new string('0', Count), context.Culture);
			}
			else {
				DateTime dateTime = value.ToDateTime(context);
				if (IsDefaultNetDateTimeFormat)
					actualString = dateTime.ToString(string.Format("%{0}", 'm'), context.Culture);
				else
					actualString = dateTime.ToString(new string('m', 2), context.Culture);
			}
			result.Text += actualString;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatMinutes other = obj as NumberFormatMinutes;
			if (other == null || Count != other.Count || Elapsed != other.Elapsed)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatMinutes(Count, Elapsed);
		}
	}
	class NumberFormatSeconds : NumberFormatTimeBase {
		public NumberFormatSeconds(int count, bool elapsed)
			: base(count, elapsed) {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Second; } }
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			DateTime dateTime = value.ToDateTime(context);
			long second = Elapsed ? (long)(value.NumericValue * 24 * 60 * 60) : dateTime.Second;
			result.Text += second.ToString(new string('0', Count), context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatSeconds other = obj as NumberFormatSeconds;
			if (other == null || Count != other.Count || Elapsed != other.Elapsed)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatSeconds(Count, Elapsed);
		}
	}
	#endregion
	#region NumericNumberFormatElements
	class NumberFormatDigitZero : INumberFormatElement {
		public static NumberFormatDigitZero Instance = new NumberFormatDigitZero();
		protected NumberFormatDigitZero() {
		}
		public bool IsDigit { get { return true; } }
		public virtual char NonLocalizedDesignator { get { return '0'; } }
		public virtual NumberFormatDesignator Designator { get { return NumberFormatDesignator.DigitZero; } }
		public void AppendFormat(StringBuilder builder) {
			builder.Append(NonLocalizedDesignator);
		}
		public void Format(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += value.NumericValue;
		}
		public virtual void FormatEmpty(WorkbookDataContext context, NumberFormatResult result) {
			result.Text += NonLocalizedDesignator;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public INumberFormatElement Clone() {
			return this;
		}
	}
	class NumberFormatDigitEmpty : NumberFormatDigitZero {
		public new static NumberFormatDigitEmpty Instance = new NumberFormatDigitEmpty();
		protected NumberFormatDigitEmpty() {
		}
		public override char NonLocalizedDesignator { get { return '#'; } }
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.DigitEmpty; } }
		public override void FormatEmpty(WorkbookDataContext context, NumberFormatResult result) {
		}
	}
	class NumberFormatDigitSpace : NumberFormatDigitZero {
		public new static NumberFormatDigitSpace Instance = new NumberFormatDigitSpace();
		protected NumberFormatDigitSpace() {
		}
		public override char NonLocalizedDesignator { get { return '?'; } }
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.DigitSpace; } }
		public override void FormatEmpty(WorkbookDataContext context, NumberFormatResult result) {
			result.Text += ' ';
		}
	}
	class NumberFormatDecimalSeparator : INumberFormatElement {
		public static NumberFormatDecimalSeparator Instance = new NumberFormatDecimalSeparator();
		protected NumberFormatDecimalSeparator() {
		}
		public bool IsDigit { get { return false; } }
		public virtual NumberFormatDesignator Designator { get { return NumberFormatDesignator.DecimalSeparator; } }
		public void AppendFormat(StringBuilder builder) {
			builder.Append(NumberFormatParser.Localizer.Chars[Designator]);
		}
		public void Format(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			FormatEmpty(context, result);
		}
		public void FormatEmpty(WorkbookDataContext context, NumberFormatResult result) {
			result.Text += GetDesignator(context.Culture);
		}
		protected virtual string GetDesignator(CultureInfo culture) {
			return culture.NumberFormat.NumberDecimalSeparator;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public INumberFormatElement Clone() {
			return this;
		}
	}
	class NumberFormatGroupSeparator : NumberFormatDecimalSeparator {
		public new static NumberFormatGroupSeparator Instance = new NumberFormatGroupSeparator();
		protected NumberFormatGroupSeparator() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.GroupSeparator; } }
		protected override string GetDesignator(CultureInfo culture) {
			return culture.NumberFormat.NumberGroupSeparator;
		}
	}
	class NumberFormatPercent : NumberFormatElementBase {
		public static NumberFormatPercent Instance = new NumberFormatPercent();
		NumberFormatPercent() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Percent; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('%');
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += '%';
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return object.ReferenceEquals(this, obj);
		}
		public override INumberFormatElement Clone() {
			return this;
		}
	}
	#endregion
	#region TextNumberFormatElements
	class NumberFormatTextContent : NumberFormatElementBase {
		public static NumberFormatTextContent Instance = new NumberFormatTextContent();
		NumberFormatTextContent() {
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.At; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('@');
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += value.ToText(context).InlineTextValue;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return obj is NumberFormatTextContent;
		}
		public override INumberFormatElement Clone() {
			return this;
		}
	}
	#endregion
	#region CommonNumberFormatElements
	public abstract class NumberFormatElementBase : INumberFormatElement {
		public bool IsDigit { get { return false; } }
		public abstract NumberFormatDesignator Designator { get; }
		public abstract void AppendFormat(StringBuilder builder);
		public void Format(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			FormatCore(value, context, result);
		}
		public void FormatEmpty(WorkbookDataContext context, NumberFormatResult result) {
			FormatCore(VariantValue.Empty, context, result);
		}
		protected abstract void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result);
		public abstract INumberFormatElement Clone();
	}
	public class NumberFormatUnderline : NumberFormatElementBase {
		char symbol;
		public NumberFormatUnderline(char symbol) {
			this.symbol = symbol;
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Underline; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('_');
			builder.Append(symbol);
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += ' ';
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatUnderline other = obj as NumberFormatUnderline;
			if (other == null || symbol != other.symbol)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatUnderline(symbol);
		}
	}
	public class NumberFormatAsterisk : NumberFormatElementBase {
		char symbol;
		public NumberFormatAsterisk(char symbol) {
			this.symbol = symbol;
		}
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Asterisk; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('*');
			builder.Append(symbol);
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += symbol;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatAsterisk other = obj as NumberFormatAsterisk;
			if (other == null || symbol != other.symbol)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatAsterisk(symbol);
		}
	}
	public class NumberFormatQuotedText : NumberFormatElementBase {
		string text;
		public NumberFormatQuotedText(string text) {
			this.text = text;
		}
		public string Text { get { return text; } set { text = value; } }
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Quote; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('"');
			builder.Append(text);
			builder.Append('"');
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += text;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatQuotedText other = obj as NumberFormatQuotedText;
			if (other == null || text != other.text)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatQuotedText(text);
		}
	}
	public class NumberFormatBackslashedText : NumberFormatElementBase {
		char text;
		public NumberFormatBackslashedText(char text) {
			this.text = text;
		}
		public char Text { get { return text; } set { text = value; } }
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Backslash; } }
		public override void AppendFormat(StringBuilder builder) {
			string textString = text.ToString();
			if (Equals(textString, 'n'))
				builder.Append('\\');
			else
				foreach (char key in NumberFormatParser.Localizer.Designators.Keys)
					if (Equals(textString, key)) {
						builder.Append('\\');
						break;
					}
			builder.Append(text);
		}
		bool Equals(string x, char y) {
			return CultureInfo.InvariantCulture.CompareInfo.Compare(x, y.ToString(), CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += text;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatBackslashedText other = obj as NumberFormatBackslashedText;
			if (other == null || text != other.text)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatBackslashedText(text);
		}
	}
	public abstract class NumberFormatCondition : NumberFormatElementBase {
		public override NumberFormatDesignator Designator { get { return NumberFormatDesignator.Bracket; } }
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('[');
			AppendFormatCore(builder);
			builder.Append(']');
		}
		protected abstract void AppendFormatCore(StringBuilder builder);
	}
	public class NumberFormatColor : NumberFormatCondition {
		Color color;
		public NumberFormatColor(Color color) {
			this.color = color;
		}
		protected override void AppendFormatCore(StringBuilder builder) {
			ColorConverter s = new ColorConverter();
			string colorString = s.ConvertToString(null, CultureInfo.InvariantCulture, color); 
			builder.Append(colorString);
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Color = color;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatColor other = obj as NumberFormatColor;
			if (other == null || color != other.color)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatColor(color);
		}
	}
	public class NumberFormatDisplayLocale : NumberFormatCondition {
		int hexCode;
		string currency;
		public NumberFormatDisplayLocale(int code, string currency) {
			this.hexCode = code;
			this.currency = currency;
		}
		public int HexCode { get { return hexCode; } }
		protected override void AppendFormatCore(StringBuilder builder) {
			builder.Append('$');
			builder.Append(currency);
			builder.Append('-');
			builder.Append(hexCode.ToString("X"));
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += currency;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatDisplayLocale other = obj as NumberFormatDisplayLocale;
			if (other == null || hexCode != other.hexCode || currency != other.currency)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatDisplayLocale(hexCode, currency);
		}
	}
	public class NumberFormatExprCondiiton : NumberFormatCondition {
		string expression;
		public NumberFormatExprCondiiton(string expression) {
			this.expression = expression;
		}
		protected override void AppendFormatCore(StringBuilder builder) {
			builder.Append(expression);
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += '[' + expression + ']';
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatExprCondiiton other = obj as NumberFormatExprCondiiton;
			if (other == null || expression != other.expression)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatExprCondiiton(expression);
		}
	}
	public class NumberFormatNotImplementedLocale : NumberFormatCondition {
		int locale;
		public NumberFormatNotImplementedLocale(int locale) {
			this.locale = locale;
		}
		public override void AppendFormat(StringBuilder builder) {
			builder.Append('B');
			builder.Append(locale);
		}
		protected override void AppendFormatCore(StringBuilder builder) {
		}
		protected override void FormatCore(VariantValue value, WorkbookDataContext context, NumberFormatResult result) {
			result.Text += 'B' + locale.ToString(context.Culture);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			NumberFormatNotImplementedLocale other = obj as NumberFormatNotImplementedLocale;
			if (other == null || locale != other.locale)
				return false;
			return true;
		}
		public override INumberFormatElement Clone() {
			return new NumberFormatNotImplementedLocale(locale);
		}
	}
	#endregion
	#region NumberFormatCollection
	public class NumberFormatCollection : UniqueItemsCache<NumberFormat> {
		public const int DefaultItemIndex = 0;
		int defaultItemCount;
		public NumberFormatCollection(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public int DefaultItemCount { get { return defaultItemCount; } }
		protected override NumberFormat CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return NumberFormat.Generic;
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			AppendItem(NumberFormat.Generic); 
			AppendItem(NumberFormatParser.Parse("0")); 
			AppendItem(NumberFormatParser.Parse("0.00")); 
			AppendItem(NumberFormatParser.Parse("#,##0")); 
			AppendItem(NumberFormatParser.Parse("#,##0.00")); 
			AppendItem(NumberFormatParser.Parse("_($#,##0_);($#,##0)")); 
			AppendItem(NumberFormatParser.Parse("_($#,##0_);[Red]($#,##0)")); 
			AppendItem(NumberFormatParser.Parse("_($#,##0.00_);($#,##0.00)")); 
			AppendItem(NumberFormatParser.Parse("_($#,##0.00_);[Red]($#,##0.00)")); 
			AppendItem(NumberFormatParser.Parse("0%")); 
			AppendItem(NumberFormatParser.Parse("0.00%")); 
			AppendItem(NumberFormatParser.Parse("0.00E+00")); 
			AppendItem(NumberFormatParser.Parse("# ?/?")); 
			AppendItem(NumberFormatParser.Parse("# ??/??")); 
			AppendItem(new NumberFormat(ShortDateNumberFormat.Instance)); 
			AppendItem(new NumberFormat(DayMonthYearNumberFormat.Instance)); 
			AppendItem(new NumberFormat(DayMonthNumberFormat.Instance)); 
			AppendItem(new NumberFormat(MonthYearNumberFormat.Instance)); 
			AppendItem(NumberFormatParser.Parse("hh:mm AM/PM")); 
			AppendItem(NumberFormatParser.Parse("hh:mm:ss AM/PM")); 
			AppendItem(NumberFormatParser.Parse("h:mm")); 
			AppendItem(NumberFormatParser.Parse("h:mm:ss")); 
			AppendItem(new NumberFormat(FullDateTimeNumberFormat.Instance)); 
			for (int i = 23; i <= 36; i++)
				AppendItem(NumberFormat.Generic);
			AppendItem(NumberFormatParser.Parse("#,##0;(#,##0)")); 
			AppendItem(NumberFormatParser.Parse("#,##0;[Red](#,##0)")); 
			AppendItem(NumberFormatParser.Parse("#,##0.00;(#,##0.00)")); 
			AppendItem(NumberFormatParser.Parse("#,##0.00;[Red](#,##0.00)")); 
			AppendItem(NumberFormatParser.Parse(@"_(* #,##0_);_(* \(#,##0\);_(* ""-""_);_(@_)")); 
			AppendItem(NumberFormatParser.Parse(@"_(""$""* #,##0_);_(""$""* \(#,##0\);_(""$""* ""-""_);_(@_)")); 
			AppendItem(NumberFormatParser.Parse(@"_(* #,##0.00_);_(* \(#,##0.00\);_(* ""-""??_);_(@_)")); 
			AppendItem(NumberFormatParser.Parse(@"_(""$""* #,##0.00_);_(""$""* \(#,##0.00\);_(""$""* ""-""??_);_(@_)")); 
			AppendItem(NumberFormatParser.Parse("mm:ss")); 
			AppendItem(NumberFormatParser.Parse("[h]:mm:ss")); 
			AppendItem(NumberFormatParser.Parse("mmss.0")); 
			AppendItem(NumberFormatParser.Parse("##0.0E+0")); 
			AppendItem(NumberFormatParser.Parse("@")); 
			for (int i = 50; i <= 81; i++)
				AppendItem(NumberFormat.Generic);
			for (int i = 82; i < 164; i++)
				AppendItem(NumberFormat.Generic);
			this.defaultItemCount = Count;
		}
#if DEBUGTEST
		public static bool CheckDefault2(NumberFormatCollection numberFormats) {
			bool result = true;
			result &= numberFormats != null;
			result &= 164 == numberFormats.Count;
			int n = 0;
			NumberFormatCollection formats = numberFormats;
			NumberFormat numberFormat;
			result &= 0 == n;
			numberFormat = formats[n++];
			result &= String.IsNullOrEmpty(numberFormat.FormatCode);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 1 == n;
			numberFormat = formats[n++];
			result &= String.Equals("0", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 2 == n;
			numberFormat = formats[n++];
			result &= String.Equals("0.00", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 3 == n;
			numberFormat = formats[n++];
			result &= String.Equals("#,##0", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 4 == n;
			numberFormat = formats[n++];
			result &= String.Equals("#,##0.00", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 5 == n;
			numberFormat = formats[n++];
			result &= String.Equals("_($#,##0_);($#,##0)", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 6 == n;
			numberFormat = formats[n++];
			result &= String.Equals("_($#,##0_);[Red]($#,##0)", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 7 == n;
			numberFormat = formats[n++];
			result &= String.Equals("_($#,##0.00_);($#,##0.00)", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 8 == n;
			numberFormat = formats[n++];
			result &= String.Equals("_($#,##0.00_);[Red]($#,##0.00)", numberFormat.FormatCode, StringComparison.OrdinalIgnoreCase);
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 9 == n;
			numberFormat = formats[n++];
			result &= "0%" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 10 == n;
			numberFormat = formats[n++];
			result &= "0.00%" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 11 == n;
			numberFormat = formats[n++];
			result &= "0.00E+00" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 12 == n;
			numberFormat = formats[n++];
			result &= "# ?/?" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 13 == n;
			numberFormat = formats[n++];
			result &= "# ??/??" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 14 == n;
			numberFormat = formats[n++];
			result &= "mm/dd/yyyy" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 15 == n;
			numberFormat = formats[n++];
			result &= "dd/mmm/yy" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 16 == n;
			numberFormat = formats[n++];
			result &= "dd/mmm" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 17 == n;
			numberFormat = formats[n++];
			result &= "mmm yy" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 18 == n;
			numberFormat = formats[n++];
			result &= "hh:mm AM/PM" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 19 == n;
			numberFormat = formats[n++];
			result &= "hh:mm:ss AM/PM" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 20 == n;
			numberFormat = formats[n++];
			result &= "h:mm" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 21 == n;
			numberFormat = formats[n++];
			result &= "h:mm:ss" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 22 == n;
			numberFormat = formats[n++];
			result &= "mm/dd/yyyy hh:mm" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			for (int i = 23; i <= 36; i++) {
				result &= i == n;
				numberFormat = formats[n++];
				result &= String.IsNullOrEmpty(numberFormat.FormatCode);
				result &= !numberFormat.IsDateTime;
				result &= !numberFormat.IsText;
			}
			result &= 37 == n;
			numberFormat = formats[n++];
			result &= "#,##0;(#,##0)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 38 == n;
			numberFormat = formats[n++];
			result &= "#,##0;[Red](#,##0)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 39 == n;
			numberFormat = formats[n++];
			result &= "#,##0.00;(#,##0.00)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 40 == n;
			numberFormat = formats[n++];
			result &= "#,##0.00;[Red](#,##0.00)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 41 == n;
			numberFormat = formats[n++];
			result &= @"_(* #,##0_);_(* (#,##0);_(* ""-""_);_(@_)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 42 == n;
			numberFormat = formats[n++];
			result &= @"_(""$""* #,##0_);_(""$""* (#,##0);_(""$""* ""-""_);_(@_)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 43 == n;
			numberFormat = formats[n++];
			result &= @"_(* #,##0.00_);_(* (#,##0.00);_(* ""-""??_);_(@_)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 44 == n;
			numberFormat = formats[n++];
			result &= @"_(""$""* #,##0.00_);_(""$""* (#,##0.00);_(""$""* ""-""??_);_(@_)" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 45 == n;
			numberFormat = formats[n++];
			result &= "mm:ss" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 46 == n;
			numberFormat = formats[n++];
			result &= "[h]:mm:ss" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 47 == n;
			numberFormat = formats[n++];
			result &= "mmss.0" == numberFormat.FormatCode;
			result &= numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 48 == n;
			numberFormat = formats[n++];
			result &= "##0.0E+0" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= !numberFormat.IsText;
			result &= 49 == n;
			numberFormat = formats[n++];
			result &= "@" == numberFormat.FormatCode;
			result &= !numberFormat.IsDateTime;
			result &= numberFormat.IsText;
			for (int i = 50; i < 164; i++) {
				result &= i == n;
				numberFormat = formats[n++];
				result &= String.IsNullOrEmpty(numberFormat.FormatCode);
				result &= !numberFormat.IsDateTime;
				result &= !numberFormat.IsText;
			}
			return result;
		}
#endif
	}
	#endregion
}
