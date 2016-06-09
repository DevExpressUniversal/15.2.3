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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Csv {
	using DevExpress.Internal;
	using DevExpress.Utils;
	using DevExpress.XtraExport.Csv;
	public class CsvSourceDataReader : SpreadsheetDataReaderBase {
		#region FormattedValue
		class FormattedValue {
			readonly static FormattedValue empty = new FormattedValue();
			public static FormattedValue Empty { get { return empty; } }
			protected FormattedValue() {
				Value = XlVariantValue.Empty;
				NumberFormatId = 0;
			}
			public FormattedValue(XlVariantValue value, int numberFormatId) {
				Value = value;
				NumberFormatId = numberFormatId;
			}
			public XlVariantValue Value { get; private set; }
			public int NumberFormatId { get; private set; }
			public bool IsEmpty { get { return Value.IsEmpty; } }
		}
		#endregion
		#region Error conversion table
		static Dictionary<string, XlVariantValue> errorByInvariantNameTable = CreateErrorByInvariantNameTable();
		static Dictionary<string, XlVariantValue> CreateErrorByInvariantNameTable() {
			Dictionary<string, XlVariantValue> result = new Dictionary<string, XlVariantValue>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			result.Add("#NULL!", XlVariantValue.ErrorNullIntersection);
			result.Add("#DIV/0!", XlVariantValue.ErrorDivisionByZero);
			result.Add("#VALUE!", XlVariantValue.ErrorInvalidValueInFunction);
			result.Add("#REF!", XlVariantValue.ErrorReference);
			result.Add("#NAME?", XlVariantValue.ErrorName);
			result.Add("#NUM!", XlVariantValue.ErrorNumber);
			result.Add("#N/A", XlVariantValue.ErrorValueNotAvailable);
			return result;
		}
		#endregion
		#region Fields
		const int bufferSize = 1024;
		const int maxQuotedTextLength = 32768;
		CsvSpreadsheetSource source;
		Stream stream;
		Encoding actualEncoding;
		CsvStreamReader reader;
		readonly List<string> lineFields = new List<string>();
		readonly StringBuilder accumulator = new StringBuilder();
		Predicate<char> endOfLine;
		char delimiter;
		char quote;
		string quoteAsString;
		string doubleQuoteAsString;
		int firstRowInRangeIndex = 0;
		int lastRowInRangeIndex = Int32.MaxValue - 1;
		long savedStreamPosition;
		ICsvSourceValueConverter valueConverter = null;
		CsvNewlineType actualNewlineType;
		#endregion
		public CsvSourceDataReader(CsvSpreadsheetSource source, Stream stream)
			: base() {
			Guard.ArgumentNotNull(source, "source");
			Guard.ArgumentNotNull(stream, "stream");
			this.source = source;
			this.stream = stream;
			this.reader = null;
			this.actualEncoding = Options.Encoding;
			this.actualNewlineType = Options.NewlineType;
			this.endOfLine = null;
			this.delimiter = Options.ValueSeparator;
			this.quote = Options.TextQualifier;
			this.quoteAsString = new string(this.quote, 1);
			this.doubleQuoteAsString = new string(this.quote, 2);
		}
		#region Properties
		CsvSpreadsheetSourceOptions Options { get { return this.source.InnerOptions; } }
		protected override List<int> NumberFormatIds { get { return null; } } 
		protected override Dictionary<int, string> NumberFormatCodes { get { return null; } } 
		protected override bool UseDate1904 { get { return false; } } 
		protected override int DefaultCellFormatIndex { get { return -1; } } 
		protected internal Encoding ActualEncoding { get { return actualEncoding; } }
		protected internal CsvNewlineType ActualNewlineType { get { return actualNewlineType; } }
		protected internal char ActualValueSeparator { get { return delimiter; } }
		#endregion
		public override void Open(IWorksheet worksheet, XlCellRange range) {
			base.Open(worksheet, range);
			this.savedStreamPosition = this.stream.Position;
			CalculateRowRange();
			DetectEncoding();
			SkipPreamble();
			DetectDelimiter();
			this.reader = new CsvStreamReader(this.stream, this.actualEncoding);
			CurrentRowIndex = -1;
		}
		public override void Close() {
			this.stream.Position = this.savedStreamPosition;
			this.reader = null;
			this.stream = null;
			this.source = null;
			base.Close();
		}
		protected override bool ReadCore() {
			while(MoveToNextRow()) {
				ReadCells();
				if(!Options.SkipEmptyRows || (ExistingCells.Count > 0))
					return true;
			}
			return false;
		}
		protected override string GetDisplayTextCore(Cell cell, CultureInfo culture) {
			int numberFormatId = cell.FormatIndex;
			IXlValueFormatter formatter = FormatterFactory.CreateFormatter(numberFormatId);
			if(formatter != null)
				return formatter.Format(cell.Value, culture);
			return cell.Value.ToText(culture).TextValue;
		}
		bool MoveToNextRow() {
			if(CurrentRowIndex == -1) {
				while(CurrentRowIndex < firstRowInRangeIndex) {
					CurrentRowIndex++;
					ReadLine();
					if(this.reader.EndOfStream && this.lineFields.Count == 0)
						return false;
				}
			}
			else {
				CurrentRowIndex++;
				if(CurrentRowIndex > lastRowInRangeIndex)
					return false;
				ReadLine();
				if(this.reader.EndOfStream && this.lineFields.Count == 0)
					return false;
			}
			return true;
		}
		void ReadCells() {
			int firstColumnIndex = 0;
			int lastColumnIndex = this.lineFields.Count - 1;
			if(Range != null && !Range.TopLeft.IsRow) {
				firstColumnIndex = Math.Max(firstColumnIndex, Range.TopLeft.Column);
				lastColumnIndex = Math.Min(lastColumnIndex, Range.BottomRight.Column);
				if(lastColumnIndex < firstColumnIndex)
					return;
			}
			this.valueConverter = Options.ValueConverter;
			for(int columnIndex = firstColumnIndex; columnIndex <= lastColumnIndex; columnIndex++) {
				FormattedValue formattedValue = GetFieldValue(columnIndex);
				if(!formattedValue.IsEmpty)
					AddCell(new Cell(columnIndex - firstColumnIndex, formattedValue.Value, columnIndex, formattedValue.NumberFormatId));
			}
		}
		void CalculateRowRange() {
			if(Range == null || Range.TopLeft.IsColumn) {
				firstRowInRangeIndex = 0;
				lastRowInRangeIndex = Int32.MaxValue - 1;
			}
			else {
				firstRowInRangeIndex = Range.FirstRow;
				lastRowInRangeIndex = Range.LastRow;
			}
		}
		#region Encoding and preamble
		void DetectEncoding() {
			if(Options.DetectEncoding) {
				InternalEncodingDetector detector = new InternalEncodingDetector();
				Encoding encoding = detector.Detect(this.stream);
				if(encoding != null)
					this.actualEncoding = encoding;
			}
		}
		void SkipPreamble() {
			long position = this.stream.Position;
			byte[] preamble = this.actualEncoding.GetPreamble();
			if((preamble.Length > 0) && (this.stream.Length >= (position + preamble.Length))) {
				byte[] bytes = new byte[preamble.Length];
				this.stream.Read(bytes, 0, preamble.Length);
				if(!ArrayEquals(bytes, preamble))
					this.stream.Position = position;
			}
		}
		bool ArrayEquals(byte[] array1, byte[] array2) {
			if(array1.Length != array2.Length)
				return false;
			for(int i = 0; i < array1.Length; i++)
				if(array1[i] != array2[i])
					return false;
			return true;
		}
		#endregion
		void ReadLine() {
			this.lineFields.Clear();
			this.accumulator.Clear();
			if(this.endOfLine == null)
				this.endOfLine = GetEndOfLinePredicate();
			bool quoted = false;
			int charCode = this.reader.Read();
			while(charCode >= 0) {
				char ch = (char)charCode;
				if(quoted) {
					this.accumulator.Append(ch);
					if(this.accumulator.Length >= maxQuotedTextLength) {
						this.lineFields.Add(this.accumulator.ToString());
						this.accumulator.Clear();
						quoted = false;
					}
					else if(ch == quote)
						quoted = false;
				}
				else {
					if(ch == quote) {
						quoted = true;
						this.accumulator.Append(ch);
					}
					else if(ch == delimiter) {
						this.lineFields.Add(this.accumulator.ToString());
						this.accumulator.Clear();
					}
					else if(endOfLine(ch)) {
						this.lineFields.Add(this.accumulator.ToString());
						this.accumulator.Clear();
						return;
					}
					else
						this.accumulator.Append(ch);
				}
				charCode = this.reader.Read();
			}
			if(this.accumulator.Length > 0)
				this.lineFields.Add(this.accumulator.ToString());
		}
		#region EndOfLine predicates
		Predicate<char> GetEndOfLinePredicate() {
			if(Options.DetectNewlineType)
				return AutoEndOfLine;
			switch(Options.NewlineType) {
				case CsvNewlineType.Lf:
					return IsLf;
				case CsvNewlineType.LfCr:
					return IsLfCr;
				case CsvNewlineType.Cr:
					return IsCr;
				case CsvNewlineType.FormFeed:
					return IsFormFeed;
				case CsvNewlineType.VerticalTab:
					return IsVerticalTab;
			}
			return IsCrLf;
		}
		bool AutoEndOfLine(char ch) {
			if(IsCrLf(ch)) {
				this.endOfLine = IsCrLf;
				this.actualNewlineType = CsvNewlineType.CrLf;
				return true;
			}
			if(IsLfCr(ch)) {
				this.endOfLine = IsLfCr;
				this.actualNewlineType = CsvNewlineType.LfCr;
				return true;
			}
			if(IsCr(ch)) {
				this.endOfLine = IsCr;
				this.actualNewlineType = CsvNewlineType.Cr;
				return true;
			}
			if(IsLf(ch)) {
				this.endOfLine = IsLf;
				this.actualNewlineType = CsvNewlineType.Lf;
				return true;
			}
			return false;
		}
		bool IsCrLf(char ch) {
			if(ch == '\r') {
				int charCode = this.reader.Peek();
				if(charCode >= 0 && (char)charCode == '\n') {
					this.reader.Read();
					return true;
				}
			}
			return false;
		}
		bool IsLfCr(char ch) {
			if(ch == '\n') {
				int charCode = this.reader.Peek();
				if(charCode >= 0 && (char)charCode == '\r') {
					this.reader.Read();
					return true;
				}
			}
			return false;
		}
		bool IsLf(char ch) {
			return ch == '\n';
		}
		bool IsCr(char ch) {
			return ch == '\r';
		}
		bool IsFormFeed(char ch) {
			return ch == '\f';
		}
		bool IsVerticalTab(char ch) {
			return ch == '\v';
		}
		#endregion
		string StripQuotesAndBlanks(string text) {
			if(string.IsNullOrEmpty(text))
				return string.Empty;
			int length = text.Length;
			if(length > 1 && text[0] == quote) {
				length--;
				if(text[length] == quote)
					length--;
				if(length < 0)
					return string.Empty;
				text = text.Substring(1, length).Replace(doubleQuoteAsString, quoteAsString);
			}
			if(Options.TrimBlanks)
				text = text.Trim();
			return text;
		}
		FormattedValue GetFieldValue(int index) {
			string fieldText = StripQuotesAndBlanks(lineFields[index]);
			if(this.valueConverter != null)
				return GetConvertedValue(fieldText, index);
			if(string.IsNullOrEmpty(fieldText))
				return FormattedValue.Empty;
			XlVariantValueType valueType = Options.Schema[index];
			FormattedValue result = TryParseAsDouble(fieldText, valueType);
			if(!result.IsEmpty)
				return result;
			result = TryParseAsDoubleWithThousands(fieldText, valueType);
			if(!result.IsEmpty)
				return result;
			result = TryParseAsDateTime(fieldText, valueType);
			if(!result.IsEmpty)
				return result;
			result = TryParseAsBoolean(fieldText, valueType);
			if(!result.IsEmpty)
				return result;
			result = TryParseAsError(fieldText, valueType);
			if(!result.IsEmpty)
				return result;
			return new FormattedValue(fieldText, 0);
		}
		FormattedValue GetConvertedValue(string text, int columnIndex) {
			object value = this.valueConverter.Convert(text, columnIndex);
			if(value == null)
				return FormattedValue.Empty;
			if(DXConvert.IsDBNull(value))
				return FormattedValue.Empty;
			Type type = value.GetType();
			if(type == typeof(DateTime)) {
				DateTime dateTimeValue = (DateTime)value;
				XlVariantValue variantValue = dateTimeValue;
				variantValue.SetDateTimeSerial(variantValue.NumericValue, false);
				return new FormattedValue(variantValue, CalculateDateTimeFormatId(dateTimeValue));
			}
			if(type == typeof(TimeSpan))
				return new FormattedValue((TimeSpan)value, 46);
			return new FormattedValue(XlVariantValue.FromObject(value), 0);
		}
		#region Parse
		FormattedValue TryParseAsDouble(string text, XlVariantValueType valueType) {
			if(valueType != XlVariantValueType.None && valueType != XlVariantValueType.Numeric)
				return FormattedValue.Empty;
			double doubleResult;
			if(double.TryParse(text, NumberStyles.Float, Options.Culture, out doubleResult)) {
				if(double.IsNaN(doubleResult) || double.IsInfinity(doubleResult))
					return FormattedValue.Empty;
				return new FormattedValue(doubleResult, 0);
			}
			string trimmedText = text.Trim();
			if(!string.IsNullOrEmpty(trimmedText)) {
				if(trimmedText[0] == '%')
					trimmedText = trimmedText.Substring(1, trimmedText.Length - 1);
				else if(trimmedText[trimmedText.Length - 1] == '%')
					trimmedText = trimmedText.Substring(0, trimmedText.Length - 1);
				else
					return TryParseFractionNumbers(trimmedText);
				if(double.TryParse(trimmedText, NumberStyles.Float, Options.Culture, out doubleResult)) {
					if(doubleResult == Math.Round(doubleResult))
						return new FormattedValue(doubleResult / 100.0, 9);
					else
						return new FormattedValue(doubleResult / 100.0, 10);
				}
			}
			return FormattedValue.Empty;
		}
		FormattedValue TryParseAsDoubleWithThousands(string text, XlVariantValueType valueType) {
			if(valueType != XlVariantValueType.None && valueType != XlVariantValueType.Numeric)
				return FormattedValue.Empty;
			CultureInfo culture = Options.Culture;
			double doubleResult;
			if(double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, culture, out doubleResult)
#if DXPORTABLE
				|| double.TryParse(RemoveThousandSeparators(text, culture), NumberStyles.Float | NumberStyles.AllowThousands, culture, out doubleResult)
#endif
				) {
				if(!double.IsNaN(doubleResult) && !double.IsInfinity(doubleResult)) {
					NumberFormatInfo numberFormat = Options.Culture.NumberFormat;
					int decimalSeparatorIndex = text.IndexOf(numberFormat.NumberDecimalSeparator);
					string integerPart = decimalSeparatorIndex > 0 ? text.Substring(0, decimalSeparatorIndex) : text;
					if(IsCorrectThousandsDouble(integerPart, numberFormat.NumberGroupSeparator, numberFormat.NumberGroupSizes))
						return new FormattedValue(doubleResult, (doubleResult == Math.Round(doubleResult)) ? 3 : 4);
				}
			}
			return FormattedValue.Empty;
		}
#if DXPORTABLE
		string RemoveThousandSeparators(string value, CultureInfo culture) {
			string separator = culture.NumberFormat.NumberGroupSeparator;
			if(String.IsNullOrEmpty(separator))
				return value;
			return value.Replace(separator, String.Empty).Replace(" ", String.Empty);
		}
#endif
		bool IsCorrectThousandsDouble(string text, string separator, int[] numberGroupSizes) {
			if(numberGroupSizes.Length == 0)
				return false;
			int oldPosition = text.Length - 1;
			int newPosition = 0;
			int lastNumberGroupIndex = numberGroupSizes.Length - 1;
			for(int i = 0; ; i++) {
				int current = i > lastNumberGroupIndex ? numberGroupSizes[lastNumberGroupIndex] : numberGroupSizes[i];
				newPosition = text.LastIndexOf(separator, oldPosition);
				if(newPosition < 0 || current == 0)
					return true;
				if(oldPosition - newPosition < current)
					return false;
				oldPosition = newPosition - 1;
			}
		}
		FormattedValue TryParseFractionNumbers(string text) {
			int position = text.IndexOf('/');
			if(position <= 0)
				return FormattedValue.Empty;
			string dividentText = text.Substring(0, position);
			string divisorText = text.Substring(position + 1);
			double divident;
			if(!double.TryParse(dividentText, NumberStyles.Float, Options.Culture, out divident))
				return FormattedValue.Empty;
			double divisor;
			if(!double.TryParse(divisorText, NumberStyles.Float, Options.Culture, out divisor))
				return FormattedValue.Empty;
			if((divident == 0 && divisor == 0) || divisor <= 0)
				return FormattedValue.Empty;
			int numberFormatId = (dividentText.Trim().Length == 1 && divisorText.Trim().Length == 1) ? 12 : 13;
			return new FormattedValue(divident / divisor, numberFormatId);
		}
		FormattedValue TryParseAsBoolean(string text, XlVariantValueType valueType) {
			if(valueType != XlVariantValueType.None && valueType != XlVariantValueType.Boolean)
				return FormattedValue.Empty;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(text, XlVariantValue.TrueConstant) == 0)
				return new FormattedValue(true, 0);
			if(StringExtensions.CompareInvariantCultureIgnoreCase(text, XlVariantValue.FalseConstant) == 0)
				return new FormattedValue(false, 0);
			return FormattedValue.Empty;
		}
		FormattedValue TryParseAsError(string text, XlVariantValueType valueType) {
			if(valueType != XlVariantValueType.None && valueType != XlVariantValueType.Error)
				return FormattedValue.Empty;
			XlVariantValue errorValue;
			if(!errorByInvariantNameTable.TryGetValue(text, out errorValue))
				return FormattedValue.Empty;
			return new FormattedValue(errorValue, 0);
		}
		FormattedValue TryParseAsDateTime(string text, XlVariantValueType valueType) {
			if(valueType != XlVariantValueType.None && valueType != XlVariantValueType.DateTime)
				return FormattedValue.Empty;
			DateTime dateTimeResult;
			if(DateTime.TryParse(text, Options.Culture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault, out dateTimeResult)) {
				XlVariantValue value = dateTimeResult;
				value.SetDateTimeSerial(value.NumericValue, false);
				return new FormattedValue(value, CalculateDateTimeFormatId(text, dateTimeResult));
			}
			return FormattedValue.Empty;
		}
		#endregion
		#region Detect NumberFormat
		int CalculateDateTimeFormatId(string text, DateTime value) {
			DateTime dateOnly = value.Date;
			if(dateOnly == value)
				return IsShortDateFormat(text) ? 14 : 15;
			if(dateOnly == DateTime.MinValue)
				return CalculateTimeFormatId(text, value);
			return 22;
		}
		int CalculateDateTimeFormatId(DateTime value) {
			DateTime dateOnly = value.Date;
			if(dateOnly == value)
				return 14;
			if(dateOnly == DateTime.MinValue)
				return 21;
			return 22;
		}
		bool IsShortDateFormat(string text) {
			string trimmedValue = text.Trim();
			int count = trimmedValue.Length;
			for(int i = 0; i < count; i++) {
				int current = (int)trimmedValue[i];
				if(current < 0x2d || current > 0x39)
					return false;
			}
			return true;
		}
		int CalculateTimeFormatId(string text, DateTime value) {
			int timeDesignatorIndex = CalculateTimeDesignatorIndex(text);
			int hourIndex = CalculateHourIndex(text, value.Hour, timeDesignatorIndex > 0);
			if(hourIndex < 0)
				return 0;
			int minuteIndex = IndexOf(text, hourIndex, value.Minute.ToString());
			if(minuteIndex < 0)
				return timeDesignatorIndex < 0 ? 0 : 18;
			int secondIndex = IndexOf(text, minuteIndex, value.Second.ToString());
			if(timeDesignatorIndex <= 0)
				return (secondIndex > minuteIndex) ? 21 : 20;
			return (secondIndex > minuteIndex) ? 19 : 18;
		}
		int CalculateHourIndex(string text, int hour, bool hasTimeDesignator) {
			if(hasTimeDesignator) {
				if(hour >= 13) {
					hour -= 12;
					return IndexOf(text, 0, hour.ToString());
				}
				if(hour == 0)
					return IndexOf(text, 0, "12");
				if(hour == 12)
					return IndexOf(text, 0, "0");
				return IndexOf(text, 0, hour.ToString());
			}
			int index = IndexOf(text, 0, hour.ToString("0"));
			if(index < 0)
				index = IndexOf(text, 0, hour.ToString("00"));
			return index;
		}
		int IndexOf(string where, int from, string what) {
			int result = where.IndexOfInvariantCultureIgnoreCase(what, from);
			if(result >= 0)
				result += what.Length;
			return result;
		}
		int CalculateTimeDesignatorIndex(string text) {
			int index = -1;
			DateTimeFormatInfo dateTimeFormat = Options.Culture.DateTimeFormat;
			if(!string.IsNullOrEmpty(dateTimeFormat.AMDesignator))
				index = Math.Max(index, IndexOf(text, 0, dateTimeFormat.AMDesignator));
			if(!string.IsNullOrEmpty(dateTimeFormat.PMDesignator))
				index = Math.Max(index, IndexOf(text, 0, dateTimeFormat.PMDesignator));
			index = Math.Max(index, IndexOf(text, 0, "AM"));
			index = Math.Max(index, IndexOf(text, 0, "PM"));
			return index;
		}
		#endregion
		#region Detect delimiter
		const int delimiterDetectorMaxRows = 10;
		const int delimiterDetectorMaxChars = 32768;
		class DelimiterStatistic {
			const char comma = ',';
			const char semicolon = ';';
			const char tab = '\t';
			const char verticalLine = '|';
			public int CommaCount { get; private set; }
			public int SemicolonCount { get; private set; }
			public int TabCount { get; private set; }
			public int VerticalLineCount { get; private set; }
			public void Add(char ch) {
				if(ch == comma)
					CommaCount++;
				else if(ch == semicolon)
					SemicolonCount++;
				else if(ch == tab)
					TabCount++;
				else if(ch == verticalLine)
					VerticalLineCount++;
			}
			public char GetOne() {
				if(CommaCount > 0 && SemicolonCount == 0 && TabCount == 0 && VerticalLineCount == 0)
					return comma;
				if(SemicolonCount > 0 && CommaCount == 0 && TabCount == 0 && VerticalLineCount == 0)
					return semicolon;
				if(TabCount > 0 && CommaCount == 0 && SemicolonCount == 0 && VerticalLineCount == 0)
					return tab;
				if(VerticalLineCount > 0 && CommaCount == 0 && SemicolonCount == 0 && TabCount == 0)
					return verticalLine;
				return '\0';
			}
			public char GetMostFrequent(CultureInfo culture) {
				char decimalSeparator = culture.NumberFormat.NumberDecimalSeparator.Length == 1 ? culture.NumberFormat.NumberDecimalSeparator[0] : '\0';
				char result = GetMostFrequentCore();
				if(result == comma && result == decimalSeparator) {
					if(SemicolonCount > TabCount && SemicolonCount > VerticalLineCount)
						result = semicolon;
					else if(TabCount > SemicolonCount && TabCount > VerticalLineCount)
						result = tab;
					else if(VerticalLineCount > SemicolonCount && VerticalLineCount > TabCount)
						result = verticalLine;
					else
						result = '\0';
				}
				return result;
			}
			char GetMostFrequentCore() {
				if(CommaCount > SemicolonCount && CommaCount > TabCount && CommaCount > VerticalLineCount)
					return comma;
				if(SemicolonCount > CommaCount && SemicolonCount > TabCount && SemicolonCount > VerticalLineCount)
					return semicolon;
				if(TabCount > CommaCount && TabCount > SemicolonCount && TabCount > VerticalLineCount)
					return tab;
				if(VerticalLineCount > CommaCount && VerticalLineCount > SemicolonCount && VerticalLineCount > TabCount)
					return verticalLine;
				return '\0';
			}
			public static DelimiterStatistic operator + (DelimiterStatistic first, DelimiterStatistic second) {
				DelimiterStatistic result = new DelimiterStatistic();
				result.CommaCount = first.CommaCount + second.CommaCount;
				result.SemicolonCount = first.SemicolonCount + second.SemicolonCount;
				result.TabCount = first.TabCount + second.TabCount;
				result.VerticalLineCount = first.VerticalLineCount + second.VerticalLineCount;
				return result;
			}
		}
		DelimiterStatistic total;
		DelimiterStatistic beforeQuote;
		DelimiterStatistic afterQuote;
		void DetectDelimiter() {
			if(!Options.DetectValueSeparator)
				return;
			char detected = DetectDelimiterCore();
			if(detected != '\0')
				this.delimiter = detected;
		}
		protected internal char DetectDelimiterCore() {
			total = new DelimiterStatistic();
			beforeQuote = new DelimiterStatistic();
			afterQuote = new DelimiterStatistic();
			long savedPosition = stream.Position;
			this.reader = new CsvStreamReader(this.stream, this.actualEncoding);
			try {
				CollectStatistics(delimiterDetectorMaxRows);
				return AnalyzeStatistics();
			}
			finally {
				this.reader = null;
				stream.Position = savedPosition;
				afterQuote = null;
				beforeQuote = null;
				total = null;
			}
		}
		void CollectStatistics(int maxRowCount) {
			int charCode = this.reader.Read();
			bool quoted = false;
			char previousChar = '\0';
			int rowCount = 0;
			int charCount = 0;
			if(this.endOfLine == null)
				this.endOfLine = GetEndOfLinePredicate();
			while(charCode >= 0 && rowCount < maxRowCount && charCount < delimiterDetectorMaxChars) {
				charCount++;
				char ch = (char)charCode;
				if(quoted) {
					if(ch == quote)
						quoted = false;
				}
				else {
					if(ch == quote) {
						beforeQuote.Add(previousChar);
						quoted = true;
					}
					else if(endOfLine(ch))
						rowCount++;
					else {
						if(previousChar == quote)
							afterQuote.Add(ch);
						else
							total.Add(ch);
					}
				}
				previousChar = ch;
				charCode = this.reader.Read();
			}
		}
		char AnalyzeStatistics() {
			char result = total.GetOne();
			if(result != '\0')
				return result;
			DelimiterStatistic nearQuote = beforeQuote + afterQuote;
			result = nearQuote.GetOne();
			if(result != '\0')
				return result;
			result = beforeQuote.GetOne();
			if(result != '\0')
				return result;
			return total.GetMostFrequent(Options.Culture);
		}
		#endregion
	}
}
