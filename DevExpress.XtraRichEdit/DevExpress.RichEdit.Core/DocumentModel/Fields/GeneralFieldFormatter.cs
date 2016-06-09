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
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Model {
	#region GeneralFieldFormatter
	public class GeneralFieldFormatter : FieldFormatter {
		#region Fields
		GeneralNumberFormatter numberFormatter;
		GeneralStringFormatter stringFormatter;
		GeneralDocumentModelStringFormatter documentModelStringFormatter;
		#endregion
		public virtual string Format(object value, string formattedValue, string keyword) {
			string result = String.Empty;
			BeginFormat();
			try {
				result = FormatCore(value, formattedValue, keyword);
			}
			finally {
				EndFormat();
			}
			return result;
		}
		protected internal virtual void BeginFormat() {
			this.numberFormatter = new GeneralNumberFormatter(Culture);
			this.stringFormatter = new GeneralStringFormatter(Culture);
			this.documentModelStringFormatter = new GeneralDocumentModelStringFormatter(Culture);
		}
		protected internal virtual void EndFormat() {
			this.numberFormatter = null;
			this.stringFormatter = null;
			this.documentModelStringFormatter = null;
		}
		public virtual bool IsGeneralDocumentModelStringFormatter(string keyword) {
			return documentModelStringFormatter.ContainsKeyword(keyword);
		}
		protected internal virtual void FormatPieceTable(PieceTable pieceTable, string keyword) {
			if (this.documentModelStringFormatter.ContainsKeyword(keyword))
				documentModelStringFormatter.Format(pieceTable, keyword);
		}
		protected internal virtual string FormatCore(object value, string formattedValue, string keyword) {
			if (this.numberFormatter.ContainsKeyword(keyword))
				return FormatAsNumber(value, formattedValue, keyword);
			else if (this.stringFormatter.ContainsKeyword(keyword))
				return FormatAsString(value, formattedValue, keyword);
			else
				FieldFormatter.ThrowUnknownSwitchArgumentError();
			return formattedValue;
		}
		protected internal virtual string FormatAsNumber(object value, string formattedValue, string keyword) {
			try {
				double doubleValue = Convert.ToDouble(value);
				return this.numberFormatter.Format(doubleValue, keyword);
			}
			catch {
				double processedValue;
				if (TryGetValue(formattedValue, out processedValue))
					return this.numberFormatter.Format(processedValue, keyword);
			}
			return formattedValue;
		}
		protected internal virtual string FormatAsString(object value, string formattedValue, string keyword) {
			if (!String.IsNullOrEmpty(formattedValue))
				return this.stringFormatter.Format(formattedValue, keyword);
			return formattedValue;
		}
		protected internal virtual bool TryGetValue(string notation, out double result) {
			result = 0;
			if (String.IsNullOrEmpty(notation))
				return false;
			try {
				MathematicalCalculator calculator = new MathematicalCalculator();
				result = calculator.Calculate(notation, Culture);
				return true;
			}
			catch (ArgumentException) {
				return false;
			}
		}
	}
	#endregion
	public delegate string FormatKeywordHandler<T>(T value, CultureInfo culture);
	public class FormatKeywordTable<T> : Dictionary<string, FormatKeywordHandler<T>> { }
	#region GeneralFormatterBase<T>
	public abstract class GeneralFormatterBase<T> {
		internal static string GetUpperCaseString(string str, CultureInfo culture) {
			return str.ToUpper(culture);
		}
		internal static string GetLowerCaseString(string str, CultureInfo culture) {
			return str.ToLower(culture);
		}
		readonly CultureInfo culture;
		protected GeneralFormatterBase(CultureInfo culture) {
			Guard.ArgumentNotNull(culture, "culture");
			this.culture = culture;
		}
		protected internal CultureInfo Culture { get { return culture; } }
		protected abstract FormatKeywordTable<T> Keywords { get; }
		public virtual string Format(T value, string keyword) {
			FormatKeywordHandler<T> handler;
			string caseSensitiveKeyword = GetCaseSensitiveKeyword(keyword);
			if (Keywords.TryGetValue(caseSensitiveKeyword, out handler))
				return handler(value, Culture);
			else {
				string lowerCaseKeyword = GetLowerCaseString(keyword, Culture);
				if (Keywords.TryGetValue(lowerCaseKeyword, out handler))
					return handler(value, Culture);
			}
			return String.Empty;
		}
		public virtual bool ContainsKeyword(string keyword) {
			if (Keywords.ContainsKey(GetUpperCaseString(keyword, Culture)))
				return true;
			return Keywords.ContainsKey(GetLowerCaseString(keyword, Culture));
		}
		protected internal string GetCaseSensitiveKeyword(string keyword) {
			if (Char.IsUpper(keyword[0]))
				return GetUpperCaseString(keyword, Culture);
			else
				return GetLowerCaseString(keyword, Culture);
		}
	}
	#endregion
	#region GeneralNumberFormatter
	public class GeneralNumberFormatter : GeneralFormatterBase<double> {
		static FormatKeywordTable<double> numberFormatKeywords = CreateNumberFormatKeywordTable();
		#region CreateNumberFormatKeywordTable
		private static FormatKeywordTable<double> CreateNumberFormatKeywordTable() {
			FormatKeywordTable<double> result = new FormatKeywordTable<double>();
			result.Add("ALPHABETIC", ConvertToUppercaseAlphabeticLatinCharacters);
			result.Add("alphabetic", ConvertToLowercaseAlphabeticLatinCharacters);
			result.Add("arabic", ConvertToArabicCardinalNumerals);
			result.Add("arabicdash", ConverToArabicCardinalNumeralsWithDashes);
			result.Add("cardtext", ConvertToLowercaseCardinalText);
			result.Add("circlenum", ConvertToNumberEnclosedInCircle);
			result.Add("dbchar", ConvertToDoubleByteArabicNumber);
			result.Add("dbnum1", ConvertToSequentialDigitalIdeographs);
			result.Add("kanjinum1", ConvertToSequentialDigitalIdeographs);
			result.Add("dollartext", ConvertToDollarText);
			result.Add("gb1", ConvertToNumberFollowedByPeriod);
			result.Add("gb2", ConvertToNumberEnclosedInBrackets);
			result.Add("gb3", ConvertToNumberEnclosedInCircleTruncated);
			result.Add("gb4", ConvertToIdeographsEnclosedInBrackets);
			result.Add("hex", ConvertToHexadecimalNumber);
			result.Add("ordinal", ConvertToOrdinalNumber);
			result.Add("ordtext", ConvertToLowercaseOrdinalText);
			result.Add("ROMAN", ConvertToUppercaseRomanNumber);
			result.Add("roman", ConvertToLowercaseRomanNumber);
			result.Add("sbchar", ConvertToArabicNumber);
			result.Add("zodiac1", ConvertToNumericalTraditionalIdeographs);
			result.Add("zodiac2", ConvertToNumericalZodiacIdeographs);
			result.Add("zodiac3", ConvertToNumericalTraditionalZodiacIdeographs);
			return result;
		}
		#endregion
		#region Number Format Handlers
		static string ConvertToUppercaseAlphabeticLatinCharacters(double value, CultureInfo culture) {
			int integerValue = ValidateValueForConvertingToLatinChars(value);
			UpperLatinLetterNumberConverter converter = new UpperLatinLetterNumberConverter();
			return converter.ConvertNumber(integerValue);
		}
		static string ConvertToLowercaseAlphabeticLatinCharacters(double value, CultureInfo culture) {
			int integerValue = ValidateValueForConvertingToLatinChars(value);
			LowerLatinLetterNumberConverter converter = new LowerLatinLetterNumberConverter();
			return converter.ConvertNumber(integerValue);
		}
		static int ValidateValueForConvertingToLatinChars(double value) {
			int integerValue = ConvertToInteger(value);
			if (integerValue < 0 || 780 < integerValue)
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
			return integerValue;
		}
		static string ConvertToArabicCardinalNumerals(double value, CultureInfo culture) {
			int integerValue = ConvertToInteger(value);
			return GetNumberString(integerValue, culture);
		}
		static string ConverToArabicCardinalNumeralsWithDashes(double value, CultureInfo culture) {
			int integerValue = ConvertToInteger(value);
			return GetFormattedNumberString("- {0} -", integerValue, culture);
		}
		static string ConvertToLowercaseCardinalText(double value, CultureInfo culture) {
			DescriptiveNumberConverterBase converter = GetDescriptiveCardinalNumberConverter(culture.Parent.EnglishName);
			return ConvertToLowercaseText(value, converter, culture);
		}
		static string ConvertToLowercaseOrdinalText(double value, CultureInfo culture) {
			DescriptiveNumberConverterBase converter = GetDescriptiveOrdinalNumberConverter(culture.Parent.EnglishName);
			return ConvertToLowercaseText(value, converter, culture);
		}
		static string ConvertToLowercaseText(double value, DescriptiveNumberConverterBase converter, CultureInfo culture) {
			int integerValue = ConvertToInteger(value);
			if (integerValue < 0 || 999999 < integerValue)
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
			return GetLowerCaseString(converter.ConvertNumber(integerValue), culture);
		}
		static DescriptiveNumberConverterBase GetDescriptiveCardinalNumberConverter(string cultureName) {
			switch (cultureName) {
				default:
				case "English":
					return new DescriptiveCardinalEnglishNumberConverter();
				case "French":
					return new DescriptiveCardinalFrenchNumberConverter();
				case "German":
					return new DescriptiveCardinalGermanNumberConverter();
				case "Italian":
					return new DescriptiveCardinalItalianNumberConverter();
				case "Russian":
					return new DescriptiveCardinalRussianNumberConverter();
				case "Swedish":
					return new DescriptiveCardinalSwedishNumberConverter();
				case "Turkish":
					return new DescriptiveCardinalTurkishNumberConverter();
				case "Greek":
					return new DescriptiveCardinalGreekNumberConverter();
				case "Spanish":
					return new DescriptiveCardinalSpanishNumberConverter();
				case "Portuguese":
					return new DescriptiveCardinalPortugueseNumberConverter();
				case "Ukrainian":
					return new DescriptiveCardinalUkrainianNumberConverter();
				case "Hindi":
					return new DescriptiveCardinalHindiNumberConverter();
			}
		}
		static DescriptiveNumberConverterBase GetDescriptiveOrdinalNumberConverter(string cultureName) {
			switch (cultureName) {
				default:
				case "English":
					return new DescriptiveOrdinalEnglishNumberConverter();
				case "French":
					return new DescriptiveOrdinalFrenchNumberConverter();
				case "German":
					return new DescriptiveOrdinalGermanNumberConverter();
				case "Italian":
					return new DescriptiveOrdinalItalianNumberConverter();
				case "Russian":
					return new DescriptiveOrdinalRussianNumberConverter();
				case "Swedish":
					return new DescriptiveOrdinalSwedishNumberConverter();
				case "Turkish":
					return new DescriptiveOrdinalTurkishNumberConverter();
				case "Greek":
					return new DescriptiveOrdinalGreekNumberConverter();
				case "Spanish":
					return new DescriptiveOrdinalSpanishNumberConverter();
				case "Portuguese":
					return new DescriptiveOrdinalPortugueseNumberConverter();
				case "Ukrainian":
					return new DescriptiveOrdinalUkrainianNumberConverter();
				case "Hindi":
					return new DescriptiveOrdinalHindiNumberConverter();
			}
		}
		static string ConvertToOrdinalNumber(double value, CultureInfo culture) {
			int integerValue = ConvertToInteger(value);
			OrdinalBasedNumberConverter converter = GetOrdinalNumberConverter(culture.Parent.EnglishName);
			return converter.ConvertNumber(integerValue);
		}
		static OrdinalBasedNumberConverter GetOrdinalNumberConverter(string cultureName) {
			switch (cultureName) {
				default:
				case "English":
					return new OrdinalEnglishNumberConverter();
				case "French":
					return new OrdinalFrenchNumberConverter();
				case "German":
					return new OrdinalGermanNumberConverter();
				case "Italian":
					return new OrdinalItalianNumberConverter();
				case "Russian":
					return new OrdinalRussianNumberConverter();
				case "Swedish":
					return new OrdinalSwedishNumberConverter();
				case "Turkish":
					return new OrdinalTurkishNumberConverter();
				case "Greek":
					return new OrdinalGreekNumberConverter();
				case "Spanish":
					return new OrdinalSpanishNumberConverter();
				case "Portuguese":
					return new OrdinalPortugueseNumberConverter();
				case "Ukrainian":
					return new OrdinalUkrainianNumberConverter();
				case "Hindi":
					return new OrdinalHindiNumberConverter();
			}
		}
		static string ConvertToNumberEnclosedInCircle(double value, CultureInfo culture) {
			NumberToEnclosedCircleNumberConverter converter = new NumberToEnclosedCircleNumberConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToNumberFollowedByPeriod(double value, CultureInfo culture) {
			NumberToNumberFollowedByPeriodConverter converter = new NumberToNumberFollowedByPeriodConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToNumberEnclosedInBrackets(double value, CultureInfo culture) {
			NumberToEnclosedBracketsNumberConverter converter = new NumberToEnclosedBracketsNumberConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToIdeographsEnclosedInBrackets(double value, CultureInfo culture) {
			NumberToIdeographsEnclosedBracketsConverter converter = new NumberToIdeographsEnclosedBracketsConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToNumericalTraditionalIdeographs(double value, CultureInfo culture) {
			NumberToTraditionalIdeographsConverter converter = new NumberToTraditionalIdeographsConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToNumericalZodiacIdeographs(double value, CultureInfo culture) {
			NumberToZodiacIdeographsConverter converter = new NumberToZodiacIdeographsConverter();
			return ConvertNumberToCharacter(value, converter, culture);
		}
		static string ConvertToNumericalTraditionalZodiacIdeographs(double value, CultureInfo culture) {
			if (value < 0)
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
			if (value == 0)
				return GetNumberString(value, culture);
			int integerValue = ConvertToInteger(value);
			NumberToTraditionalZodiacIdeographsConverter converter = new NumberToTraditionalZodiacIdeographsConverter();
			return converter.Convert(integerValue);
		}
		static string ConvertNumberToCharacter(double value, NumberToSingleCharConverter converter, CultureInfo culture) {
			if (value < 0)
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
			int integerValue = ConvertToInteger(value);
			string result = String.Empty;
			if (converter.TryConvert(integerValue, out result))
				return result;
			else
				return GetNumberString(integerValue, culture);
		}
		static string ConvertToDoubleByteArabicNumber(double value, CultureInfo culture) {
			NumberToDoubleByteNumberConverter converter = new NumberToDoubleByteNumberConverter();
			return converter.Convert(value, culture);
		}
		static string ConvertToSequentialDigitalIdeographs(double value, CultureInfo culture) {
			double roundValue = ConvertToInteger(value);
			NumberToDigitalIdeographsConverter converter = new NumberToDigitalIdeographsConverter();
			return converter.Convert(roundValue, culture);
		}
		static string ConvertToDollarText(double value, CultureInfo culture) {
			if (value < 0 || value >= 1000000)
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
			NumberToDollarTextConverter converter = GetDollarTextConverter(culture.Parent.EnglishName);
			return GetLowerCaseString(converter.Convert(value), culture);
		}
		static NumberToDollarTextConverter GetDollarTextConverter(string cultureName) {
			switch (cultureName) {
				default:
				case "English":
					return new NumberToEnglishDollarTextConverter();
				case "French":
					return new NumberToFrenchDollarTextConverter();
				case "German":
					return new NumberToGermanDollarTextConverter();
				case "Italian":
					return new NumberToItalianDollarTextConverter();
				case "Russian":
					return new NumberToRussianDollarTextConverter();
				case "Swedish":
					return new NumberToSwedishDollarTextConverter();
				case "Turkish":
					return new NumberToTurkishDollarTextConverter();
				case "Greek":
					return new NumberToGreekDollarTextConverter();
				case "Spanish":
					return new NumberToSpanishDollarTextConverter();
				case "Portuguese":
					return new NumberToPortugueseDollarTextConverter();
				case "Ukrainian":
					return new NumberToUkrainianDollarTextConverter();
				case "Hindi":
					return new NumberToHindiDollarTextConverter();
			}
		}
		static string ConvertToNumberEnclosedInCircleTruncated(double value, CultureInfo culture) {
			int integerValue = ConvertToInteger(value);
			if (integerValue < 11)
				return ConvertToNumberEnclosedInCircle(integerValue, culture);
			else
				return GetNumberString(integerValue, culture);
		}
		static string ConvertToHexadecimalNumber(double value, CultureInfo culture) {
			int integerValue = ConvertToShortInteger(value);
			return GetNumberString(integerValue, "X", culture);
		}
		static string ConvertToUppercaseRomanNumber(double value, CultureInfo culture) {
			int integerValue = ConvertToShortInteger(value);
			UpperRomanNumberConverterClassic converter = new UpperRomanNumberConverterClassic();
			return converter.ConvertNumber(integerValue);
		}
		static string ConvertToLowercaseRomanNumber(double value, CultureInfo culture) {
			int integerValue = ConvertToShortInteger(value);
			LowerRomanNumberConverterClassic converter = new LowerRomanNumberConverterClassic();
			return converter.ConvertNumber(integerValue);
		}
		static string ConvertToArabicNumber(double value, CultureInfo culture) {
			return GetNumberString(value, culture);
		}
		#endregion
		static int ConvertToInteger(double value) {
			try {
				return checked((int)Math.Round(value));
			}
			catch (OverflowException) {
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
				return -1;
			}
		}
		static int ConvertToShortInteger(double value) {
			try {
				return checked((short)Math.Round(value));
			}
			catch (OverflowException) {
				FieldFormatter.ThrowIncorrectNumericFieldFormatError();
				return -1;
			}
		}
		static string GetNumberString(double value, CultureInfo culture) {
			string formattingString = "0" + culture.NumberFormat.NumberDecimalSeparator + new String('#', 14);
			return value.ToString(formattingString, culture);
		}
		static string GetNumberString(int value, CultureInfo culture) {
			return value.ToString(culture);
		}
		static string GetNumberString(int value, string format, CultureInfo culture) {
			return value.ToString(format, culture);
		}
		static string GetFormattedNumberString(string format, int value, CultureInfo culture) {
			return String.Format(culture, format, value);
		}
		public GeneralNumberFormatter(CultureInfo culture)
			: base(culture) {
		}
		protected override FormatKeywordTable<double> Keywords { get { return numberFormatKeywords; } }
	}
	#endregion
	#region GeneralStringFormatter
	public class GeneralStringFormatter : GeneralFormatterBase<string> {
		static FormatKeywordTable<string> stringFormatKeywords = CreateStringFormatKeywordsTable();
		#region CreateStringFormatKeywordsTable
		static FormatKeywordTable<string> CreateStringFormatKeywordsTable() {
			FormatKeywordTable<string> result = new FormatKeywordTable<string>();
			result.Add("caps", CapitalizesFirstLetterOfEachWord);
			result.Add("firstcap", CapitalizesFirstLetterOfFirstWord);
			result.Add("lower", ConvertToLowercaseString);
			result.Add("upper", ConvertToUppercaseString);
			return result;
		}
		#endregion
		#region String Format Handlers
		static string CapitalizesFirstLetterOfEachWord(string value, CultureInfo culture){
			StringBuilder result = new StringBuilder(value.ToLower(culture));
			int index = SkipCharacters(0, result, IsNotLetterOrDigit);
			while (index < result.Length) {
				result[index] = CharHelper.ToUpper(result[index], culture);
				index = SkipCharacters(index, result, IsLetterOrDigit);
				index = SkipCharacters(index, result, IsNotLetterOrDigit);
			}
			return result.ToString();
		}
		static int SkipCharacters(int index, StringBuilder str, Predicate<char> predicate) {
			while (index < str.Length && predicate(str[index]))
				index++;
			return index;
		}
		static bool IsLetterOrDigit(char ch) {
			return Char.IsLetterOrDigit(ch);
		}
		static bool IsNotLetterOrDigit(char ch) {
			return !IsLetterOrDigit(ch);
		}
		static string CapitalizesFirstLetterOfFirstWord(string value, CultureInfo culture) {
			StringBuilder result = new StringBuilder(value);
			if (result.Length > 0)
				result[0] = CharHelper.ToUpper(result[0], culture);
			return result.ToString();
		}
		static string ConvertToLowercaseString(string value, CultureInfo culture) {
			return value.ToLower(culture);
		}
		static string ConvertToUppercaseString(string value, CultureInfo culture) {
			return value.ToUpper(culture);
		}
		#endregion
		public GeneralStringFormatter(CultureInfo culture)
			: base(culture) {
		}
		protected override FormatKeywordTable<string> Keywords { get { return stringFormatKeywords; } }
	}
	#endregion
	#region GeneralDocumentModelStringFormatter
	public class GeneralDocumentModelStringFormatter : GeneralFormatterBase<PieceTable> {
		static FormatKeywordTable<PieceTable> stringFormatKeywords = CreateStringFormatKeywordsTable();
		#region CreateStringFormatKeywordsTable
		static FormatKeywordTable<PieceTable> CreateStringFormatKeywordsTable() {
			FormatKeywordTable<PieceTable> result = new FormatKeywordTable<PieceTable>();
			result.Add("caps", CapitalizesFirstLetterOfEachWord);
			result.Add("firstcap", CapitalizesFirstLetterOfFirstWord);
			result.Add("lower", ConvertToLowercaseString);
			result.Add("upper", ConvertToUppercaseString);
			return result;
		}
		#endregion
		#region String Format Handlers
		static string CapitalizesFirstLetterOfEachWord(PieceTable pieceTable, CultureInfo culture) {
			WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(pieceTable);
			DocumentModelPosition pos = new DocumentModelPosition(pieceTable);
			if (!iterator.IsInsideWord(pos)) {
				pos = iterator.MoveForward(pos);
			}
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			TextRunCollection ranges = pieceTable.Runs;
			while (pos.LogPosition < pieceTable.DocumentEndLogPosition) {
				TextRunBase range = ranges[pos.RunIndex];
				int position = range.StartIndex + pos.RunOffset;
				textBuffer[position] = CharHelper.ToUpper(textBuffer[position], culture);
				pos = iterator.MoveForward(pos);
			}
			return String.Empty;
		}
		static string CapitalizesFirstLetterOfFirstWord(PieceTable pieceTable, CultureInfo culture) {
			WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(pieceTable);
			DocumentModelPosition pos = new DocumentModelPosition(pieceTable);
			if (!iterator.IsInsideWord(pos)) {
				pos = iterator.MoveForward(pos);
			}
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			TextRunCollection ranges = pieceTable.Runs;
			if (pos.LogPosition < pieceTable.DocumentEndLogPosition) {
				TextRunBase range = ranges[pos.RunIndex];
				int position = range.StartIndex + pos.RunOffset;
				textBuffer[position] = CharHelper.ToUpper(textBuffer[position], culture);
			}
			return null;
		}
		static string ConvertToLowercaseString(PieceTable pieceTable, CultureInfo culture) {
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			int count = textBuffer.Length;
			for (int i = 0; i < count; i++) {
				textBuffer[i] = CharHelper.ToLower(textBuffer[i], culture);
			}
			return String.Empty;
		}
		static string ConvertToUppercaseString(PieceTable pieceTable, CultureInfo culture) {
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			int count = textBuffer.Length;
			for (int i = 0; i < count; i++) {
				textBuffer[i] = CharHelper.ToUpper(textBuffer[i], culture);
			}
			return String.Empty;
		}
		#endregion
		public GeneralDocumentModelStringFormatter(CultureInfo culture)
			: base(culture) {
		}
		protected override FormatKeywordTable<PieceTable> Keywords { get { return stringFormatKeywords; } }
	}
	#endregion
	#region NumberToSingleCharConverter (abstract class)
	public abstract class NumberToSingleCharConverter {
		readonly char[] alphanumericChars;
		protected NumberToSingleCharConverter() {
			this.alphanumericChars = CreateAlphanumericCharsTable();
		}
		public virtual bool TryConvert(int number, out string result) {
			result = String.Empty;
			if (number <= 0 || this.alphanumericChars.Length < number)
				return false;
			char resultChar = this.alphanumericChars[number - 1];
			result = resultChar.ToString(CultureInfo.InvariantCulture);
			return true;
		}
		protected abstract char[] CreateAlphanumericCharsTable();
	}
	#endregion
	#region NumberToEnclosedCircleNumberConverter
	public class NumberToEnclosedCircleNumberConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u2460',
				'\u2461',
				'\u2462',
				'\u2463',
				'\u2464',
				'\u2465',
				'\u2466',
				'\u2467',
				'\u2468',
				'\u2469',
				'\u246A',
				'\u246B',
				'\u246C',
				'\u246D',
				'\u246E',
				'\u246F',
				'\u2470',
				'\u2471',
				'\u2472',
				'\u2473'
			};
		}
	}
	#endregion
	#region NumberToNumberFollowedByPeriodConverter
	public class NumberToNumberFollowedByPeriodConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u2488',
				'\u2489',
				'\u248A',
				'\u248B',
				'\u248C',
				'\u248D',
				'\u248E',
				'\u248F',
				'\u2490',
				'\u2491',
				'\u2492',
				'\u2493',
				'\u2494',
				'\u2495',
				'\u2496',
				'\u2497',
				'\u2498',
				'\u2499',
				'\u249A',
				'\u249B'
			};
		}
	}
	#endregion
	#region NumberToEnclosedBracketsNumberConverter
	public class NumberToEnclosedBracketsNumberConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u2474',
				'\u2475',
				'\u2476',
				'\u2477',
				'\u2478',
				'\u2479',
				'\u247A',
				'\u247B',
				'\u247C',
				'\u247D',
				'\u247E',
				'\u247F',
				'\u2480',
				'\u2481',
				'\u2482',
				'\u2483',
				'\u2484',
				'\u2485',
				'\u2486',
				'\u2487'
			};
		}
	}
	#endregion
	#region NumberToIdeographsEnclosedBracketsConverter
	public class NumberToIdeographsEnclosedBracketsConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u3220',
				'\u3221',
				'\u3222',
				'\u3223',
				'\u3224',
				'\u3225',
				'\u3226',
				'\u3227',
				'\u3228',
				'\u3229'
			};
		}
	}
	#endregion
	#region NumberToTraditionalIdeographsConverter
	public class NumberToTraditionalIdeographsConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u7532',
				'\u4E59',
				'\u4E19',
				'\u4E01',
				'\u620A',
				'\u5DF1',
				'\u5E9A',
				'\u8F9B',
				'\u58EC',
				'\u7678'
			};
		}
	}
	#endregion
	#region NumberToZodiacIdeographsConverter
	public class NumberToZodiacIdeographsConverter : NumberToSingleCharConverter {
		protected override char[] CreateAlphanumericCharsTable() {
			return new char[] {
				'\u5B50',
				'\u4E11',
				'\u5BC5',
				'\u536F',
				'\u8FB0',
				'\u5DF3',
				'\u5348', 
				'\u672A',
				'\u7533',
				'\u9149',
				'\u620D',
				'\u4EA5'
			};
		}
	}
	#endregion
	#region NumberToMultipleCharsConverter (abstract class)
	public abstract class NumberToMultipleCharsConverter {
		public virtual string Convert(double number, CultureInfo culture) {
			string numberStr = number.ToString(culture);
			StringBuilder result = new StringBuilder();
			foreach (char ch in numberStr) {
				char newChar = ConvertSingleChar(ch);
				result.Append(newChar);
			}
			return result.ToString();
		}
		protected abstract char ConvertSingleChar(char ch);
	}
	#endregion
	#region NumberToDoubleByteNumberConverter
	public class NumberToDoubleByteNumberConverter : NumberToMultipleCharsConverter {
		const int DoubleByteNumberCharOffset = 65248;
		protected override char ConvertSingleChar(char ch) {
			int newCharCode = ch + DoubleByteNumberCharOffset;
			if (newCharCode > 65536)
				Exceptions.ThrowArgumentException("numeric character", ch);
			return (char)newCharCode;
		}
	}
	#endregion
	#region NumberToDigitalIdeographsConverter
	public class NumberToDigitalIdeographsConverter : NumberToMultipleCharsConverter {
		const int ZeroCharCode = 48;
		readonly char[] digitalIdeographs = { '\u3007', '\u4E00', '\u4E8C', '\u4E09', '\u56DB', '\u4E94', '\u516D', '\u4E03', '\u516B', '\u4E5D' };
		protected override char ConvertSingleChar(char ch) {
			int digit = ch - ZeroCharCode;
			if (digit < 0 || digit > 9)
				Exceptions.ThrowArgumentException("digit", ch);
			return digitalIdeographs[digit];
		}
	}
	#endregion
	#region NumberToDollarTextConverter (abstract class)
	public abstract class NumberToDollarTextConverter {
		public virtual string Convert(double value) {
			int integerPart = (int)value;
			double remainder = Math.Round((value - integerPart), 2);
			int fractionPart = (int)Math.Round(remainder * 100);
			return ConvertCore(integerPart, fractionPart);
		}
		protected virtual string ConvertCore(int integerPart, int fractionPart) {
			string cardinalText = GetCardinalText(integerPart);
			string contactString = GetContactString();
			if (0 < fractionPart && fractionPart < 100)
				return String.Format("{0} {1} {2}/100", cardinalText, contactString, fractionPart);
			else
				return String.Format("{0} {1} 00/100", cardinalText, contactString);
		}
		protected string GetCardinalText(int integerPart) {
			DescriptiveNumberConverterBase converter = GetNumberConverter();
			return converter.ConvertNumber(integerPart);
		}
		protected abstract string GetContactString();
		protected abstract DescriptiveNumberConverterBase GetNumberConverter();
	}
	#endregion
	#region NumberToEnglishDollarTextConverter
	public class NumberToEnglishDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalEnglishNumberConverter();
		}
		protected override string GetContactString() {
			return "and";
		}
	}
	#endregion
	#region NumberToFrenchDollarTextConverter
	public class NumberToFrenchDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalFrenchNumberConverter();
		}
		protected override string GetContactString() {
			return "et";
		}
	}
	#endregion
	#region NumberToGermanDollarTextConverter
	public class NumberToGermanDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalGermanNumberConverter();
		}
		protected override string GetContactString() {
			return "und";
		}
	}
	#endregion
	#region NumberToItalianDollarTextConverter
	public class NumberToItalianDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalItalianNumberConverter();
		}
		protected override string GetContactString() {
			return "e";
		}
	}
	#endregion
	#region NumberToRussianDollarTextConverter
	public class NumberToRussianDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalRussianNumberConverter();
		}
		protected override string GetContactString() {
			return "и";
		}
	}
	#endregion
	#region NumberToSwedishDollarTextConverter
	public class NumberToSwedishDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalSwedishNumberConverter();
		}
		protected override string GetContactString() {
			return "och";
		}
	}
	#endregion
	#region NumberToTurkishDollarTextConverter
	public class NumberToTurkishDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalTurkishNumberConverter();
		}
		protected override string GetContactString() {
			return "ve";
		}
	}
	#endregion
	#region NumberToGreekDollarTextConverter
	public class NumberToGreekDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalGreekNumberConverter();
		}
		protected override string GetContactString() {
			return "και";
		}
	}
	#endregion
	#region NumberToSpanishDollarTextConverter
	public class NumberToSpanishDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalSpanishNumberConverter();
		}
		protected override string GetContactString() {
			return "y";
		}
	}
	#endregion
	#region NumberToPortugueseDollarTextConverter
	public class NumberToPortugueseDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalPortugueseNumberConverter();
		}
		protected override string GetContactString() {
			return "e";
		}
	}
	#endregion
	#region NumberToUkrainianDollarTextConverter
	public class NumberToUkrainianDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalUkrainianNumberConverter();
		}
		protected override string GetContactString() {
			return "і";
		}
	}
	#endregion
	#region NumberToHindiDollarTextConverter
	public class NumberToHindiDollarTextConverter : NumberToDollarTextConverter {
		protected override DescriptiveNumberConverterBase GetNumberConverter() {
			return new DescriptiveCardinalHindiNumberConverter();
		}
		protected override string GetContactString() {
			return "और";
		}
	}
	#endregion
	#region NumberToTraditionalZodiacIdeographsConverter
	public class NumberToTraditionalZodiacIdeographsConverter {
		readonly char[] heavenlyTrunksIdeographs = { '\u7532', '\u4E59', '\u4E19', '\u4E01', '\u620A', '\u5DF1', '\u5E9A', '\u8F9B', '\u58EC', '\u7678' };
		readonly char[] earthlyBranchesIdeographs = { '\u5B50', '\u4E11', '\u5BC5', '\u536F', '\u8FB0', '\u5DF3', '\u5348', '\u672A', '\u7533', '\u9149', '\u620D', '\u4EA5' };
		public string Convert(int number) {
			int value = number - 1;
			StringBuilder result = new StringBuilder(2);
			result.Append(this.heavenlyTrunksIdeographs[value % this.heavenlyTrunksIdeographs.Length]);
			result.Append(this.earthlyBranchesIdeographs[value % this.earthlyBranchesIdeographs.Length]);
			return result.ToString();
		}
	}
	#endregion
}
