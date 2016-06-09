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

#if OPENDOCUMENT
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenDocument {
	public partial class OpenDocumentExporter {
		#region Static
		static readonly List<char> NumberFormatSymbols = new List<char>() { '$', '%', '?', '#', '0', '@', 'y', 'm', 'd', 'h', 's', 'a', '\\', '"', '_', '*' };
		static readonly List<char> NumericNumberFormatSymbols = new List<char>() { '#', '0', '?' };
		static readonly List<char> HelperNumberFormatSymbols = new List<char>() { '\\', '_', '*' }; 
		public static bool IsNumericSymbol(char c) {
			if (NumericNumberFormatSymbols.Contains(c))
				return true;
			return false;
		}
		public static int GetNonTextSymbolPosition(string text, char symbol, bool fromBeginToEnd) {
			if (string.IsNullOrEmpty(text))
				return -1;
			if (fromBeginToEnd)
				return GetNonTextSymbolPositionFromBegin(text, symbol);
			return GetNonTextSymbolPositionFromEnd(text, symbol);
		}
		static int GetNonTextSymbolPositionFromBegin(string text, char symbol) {
			bool isQuoted = false;
			if (text[0] == symbol)
				return 0;
			int index = 1;
			for (; index < text.Length; ++index) {
				char current = text[index];
				if (!isQuoted && current == symbol) {
					char prev = text[index - 1];
					if (!HelperNumberFormatSymbols.Contains(prev))
						return index;
				}
				if (current == '"')
					isQuoted = !isQuoted;
			}
			return -1;
		}
		static int GetNonTextSymbolPositionFromEnd(string text, char symbol) {
			bool isQuoted = false;
			int index = text.Length - 1;
			for (; index > 0; --index) {
				char current = text[index];
				if (!isQuoted && current == symbol) {
					char prev = text[index - 1];
					if (!HelperNumberFormatSymbols.Contains(prev))
						return index;
				}
				if (current == '"')
					isQuoted = !isQuoted;
			}
			return -1;
		}
		#endregion
		protected internal CompressedStream ExportStyles() {
			return CreateXmlContent(GenerateStyles);
		}
		protected internal void GenerateStyles(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateStyles();
		}
		protected internal void GenerateStyles() {
			GenerateDocumentStylesStart();
			try {
				GenerateDocumentStylesAttrs();
				GenerateFontFaceDecls();
				GenerateDocumentStyles();
			}
			finally {
				GenerateDocumentStylesEnd();
			}
		}
		protected internal void GenerateDocumentStylesStart() {
			DocumentContentWriter.WriteStartElement("office", "document-styles", OfficeNamespace);
		}
		protected internal void GenerateDocumentStylesAttrs() {
			WriteStringAttr("version", OfficeNamespace, "1.2");
			WriteNs("tableooo", "http://openoffice.org/2009/table");
			WriteNs("grddl", "http://www.w3.org/2003/g/data-view#");
			WriteNs("xhtml", "http://www.w3.org/1999/xhtml");
			WriteNs("of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2");
			WriteNs("rpt", "http://openoffice.org/2005/report");
			WriteNs("dom", "http://www.w3.org/2001/xml-events");
			WriteNs("oooc", "http://openoffice.org/2004/calc");
			WriteNs("ooow", "http://openoffice.org/2004/writer");
			WriteNs("ooo", "http://openoffice.org/2004/office");
			WriteNs("script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0");
			WriteNs("form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0");
			WriteNs("math", "http://www.w3.org/1998/Math/MathML");
			WriteNs("dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0");
			WriteNs("chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0");
			WriteNs("svg", SvgNamespace);
			WriteNs("presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0");
			WriteNs("number", NumberNamespace);
			WriteNs("meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
			WriteNs("dc", "http://purl.org/dc/elements/1.1/");
			WriteNs("xlink", XLinkNamespace);
			WriteNs("fo", FoNamespace);
			WriteNs("draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
			WriteNs("table", TableNamespace);
			WriteNs("text", TextNamespace);
			WriteNs("style", StyleNamespace);
			WriteNs("office", OfficeNamespace);
		}
		protected internal void GenerateDocumentStylesEnd() {
			WriteEndElement();
		}
		protected internal void GenerateFontFaceDecls() {
			WriteStartElement("font-face-decls", OfficeNamespace);
			try {
				foreach (string fontName in exportStyleSheet.FontNameTable.Keys)
					GenerateFontFace(fontName);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateFontFace(string fontName) {
			WriteStartElement("font-face", StyleNamespace);
			try {
				WriteStringAttr("font-family", SvgNamespace, "'" + fontName + "'");
				WriteStringAttr("name", StyleNamespace, fontName);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDocumentStyles() {
			WriteStartElement("styles", OfficeNamespace);
			try {
				GenerateNumberFormats();
				GenerateCellStyles();
				GenerateDifferentialFormats();
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDocumentAutomaticStyles() {
			WriteStartElement("automatic-styles", OfficeNamespace);
			try {
				GenerateCellFormats();
				GenerateConditionalFormats();
				GenerateColumnFormats();
				GenerateRowFormats();
				GenerateSheetFormats();
			}
			finally {
				WriteEndElement();
			}
		}
		#region GenerateNumberFormats
		protected internal void GenerateNumberFormats() {
			foreach (int index in ExportStyleSheet.NumberFormatTable.Keys) {
				NumberFormat numberFormat = Workbook.Cache.NumberFormatCache[index];
				GenerateNumberFormat(ExportStyleSheet.GetNumberFormatName(index), numberFormat.FormatCode);
			}
		}
		protected internal void GenerateNumberFormat(string styleName, string formatString) {
			if (string.IsNullOrEmpty(formatString)) {
				GenerateDefaultNumberFormat(styleName);
				return;
			}
			StyleMap condition;
			List<StyleMap> conditions = new List<StyleMap>();
			int semicolonPosition = GetNonTextSymbolPosition(formatString, ';', true);
			while (semicolonPosition >= 0) {
				condition = StyleMap.Parse(formatString.Remove(semicolonPosition));
				conditions.Add(condition);
				formatString = formatString.Substring(semicolonPosition + 1);
				semicolonPosition = GetNonTextSymbolPosition(formatString, ';', true);
			}
			condition = StyleMap.Parse(formatString);
			if (condition.HasCondition) {
				conditions.Add(condition);
				formatString = "0";
			}
			switch (conditions.Count) {
				case 1:
					if (!conditions[0].HasCondition)
						conditions[0].SetCondition(">=0");
					break;
				case 2:
					if (!conditions[0].HasCondition)
						conditions[0].SetCondition(">0");
					if (!conditions[1].HasCondition)
						conditions[1].SetCondition("<0");
					break;
				case 3:
					if (!conditions[0].HasCondition)
						conditions[0].SetCondition(">0");
					if (!conditions[1].HasCondition)
						conditions[1].SetCondition("<0");
					if (!conditions[2].HasCondition)
						conditions[2].SetCondition("=0");
					break;
			}
			int i = 0;
			conditions.ForEach(x => GenerateNumberFormatCore(GetConditionName(styleName, i++), x.FormatString, new List<StyleMap>()));
			GenerateNumberFormatCore(styleName, formatString, conditions);
		}
		void GenerateDefaultNumberFormat(string numberFormatStyleName) {
			WriteStartElement("number-style", NumberNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, numberFormatStyleName);
				WriteStartElement("number", NumberNamespace);
				try {
					WriteNumericAttr("min-integer-digits", NumberNamespace, 1);
				}
				finally {
					WriteEndElement();
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateNumberFormatCore(string styleName, string formatString, List<StyleMap> conditions) {
			bool truncateOnOverflow;
			CellValueOdsType numberFormatType;
			List<NumberFormatElement> numberFormat = GenerateFormatString(formatString, out numberFormatType, out truncateOnOverflow);
			if (conditions.Count > 1)
				numberFormatType = CellValueOdsType.Float;
			WriteStartElement(GetTagNameForNumberFormat(numberFormatType), NumberNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				if (!truncateOnOverflow)
					WriteStringAttr("truncate-on-overflow", NumberNamespace, "false");
				int i = 0;
				numberFormat.ForEach(x => x.Generate(this));
				conditions.ForEach(x => x.Generate(this, GetConditionName(styleName, i++)));
			}
			finally {
				WriteEndElement();
			}
		}
		string GetConditionName(string styleName, int counter) {
			return styleName + "P" + counter.ToString();
		}
		string GetTagNameForNumberFormat(CellValueOdsType numberFormatType) {
			switch (numberFormatType) {
				case CellValueOdsType.Currency:  return "currency-style";
				case CellValueOdsType.Date:	  return "date-style";
				case CellValueOdsType.Float:	 return "number-style";
				case CellValueOdsType.Percentage:return "percentage-style";
				case CellValueOdsType.Time:	  return "time-style";
				case CellValueOdsType.String:	return "text-style";
				default:						 return "text-style";
			}
		}
		List<NumberFormatElement> GenerateFormatString(string formatString, out CellValueOdsType numberFormatType, out bool truncateOnOverflow) {
			List<NumberFormatElement> numberFormat = new List<NumberFormatElement>();
			truncateOnOverflow = true;
			bool hasCurrencySymbol = false;
			bool hasNumber = false;
			bool hasPercentage = false;
			bool hasDate = false;
			bool hasTime = false;
			int i = 0;
			while (i < formatString.Length) {
				int lastItem;
				int blockLength = 0;
				char symbol = formatString[i];
				StringBuilder textAfter = new StringBuilder();
				switch (symbol) {
					case '\\':
					case '"':
						break;
					case ';':
						blockLength = formatString.Length; 
						break;
					case '_':
						blockLength = OnUnderline(formatString, i, textAfter) - i + 1;
						break;
					case '*':
						blockLength = OnAsterisk(formatString, i, textAfter) - i + 1;
						break;
					case '%':
						blockLength = 1;
						hasPercentage = true;
						textAfter.Append('%');
						break;
					case '$':
						blockLength = 1;
						hasCurrencySymbol = true;
						numberFormat.Add(new NumberFormatCurrencySymbol("$"));
						break;
					case '.':
						blockLength = formatString.LastIndexOfAny(NumericNumberFormatSymbols.ToArray()) - i + 1;
						if (blockLength < 0)
							blockLength = 0;
						else {
							while (formatString.Length > i + blockLength && formatString[i + blockLength] == ',')
								++blockLength;
							hasNumber = true;
							numberFormat.Add(ParseNumber(formatString.Substring(i, blockLength), textAfter));
						}
						break;
					case '?':
					case '#':
					case '0':
						blockLength = formatString.LastIndexOfAny(NumericNumberFormatSymbols.ToArray()) - i + 1;
						while (formatString.Length > i + blockLength && formatString[i + blockLength] == ',')
							++blockLength;
						hasNumber = true;
						numberFormat.Add(ParseNumber(formatString.Substring(i, blockLength), textAfter));
						break;
					case '@':
						blockLength = 1;
						numberFormat.Add(new NumberFormatTextContent());
						break;
					case 'y':
					case 'Y':
						blockLength = GetDateTimeBlockLength(formatString, 'y', i);
						hasDate = true;
						numberFormat.Add(new NumberFormatYear(blockLength > 2));
						break;
					case 'm':
					case 'M':
						blockLength = GetDateTimeBlockLength(formatString, 'm', i);
						if (blockLength > 2) {
							hasDate = true;
							numberFormat.Add(new NumberFormatMonth(blockLength == 4 || blockLength > 5, true));
						}
						else {
							lastItem = numberFormat.Count - 1;
							if (lastItem >= 0 && numberFormat[lastItem] is NumberFormatHours ||
								lastItem - 1 >= 0 && numberFormat[lastItem] is NumberFormatText && numberFormat[lastItem - 1] is NumberFormatHours) {
								hasTime = true;
								numberFormat.Add(new NumberFormatMinutes(blockLength > 1));
							}
							else {
								hasDate = true;
								numberFormat.Add(new NumberFormatMonth(blockLength > 1, false));
							}
						}
						break;
					case 'd':
					case 'D':
						blockLength = GetDateTimeBlockLength(formatString, 'd', i);
						hasDate = true;
						if (blockLength > 2)
							numberFormat.Add(new NumberFormatDayOfWeek(blockLength > 3));
						else
							numberFormat.Add(new NumberFormatDay(blockLength > 1));
						break;
					case ']':
						blockLength = 1;
						break;
					case '[':
						NumberFormatElement element = ParseBracket(formatString, i, out blockLength);
						if (blockLength < 0)
							blockLength = 1;
						else
							if (element != null) {
								hasTime = true;
								numberFormat.Add(element);
								truncateOnOverflow = false;
							}
						break;
					case 'h':
					case 'H':
						blockLength = GetDateTimeBlockLength(formatString, 'h', i);
						hasTime = true;
						numberFormat.Add(new NumberFormatHours(blockLength > 1));
						break;
					case 's':
					case 'S':
						lastItem = numberFormat.Count - 1;
						if (lastItem >= 0) {
							NumberFormatMonth month = numberFormat[lastItem] as NumberFormatMonth;
							if (month != null) {
								hasTime = true;
								numberFormat[lastItem] = new NumberFormatMinutes(month.IsLong);
								hasDate = HasDate(numberFormat);
							}
							else
								if (lastItem - 1 >= 0 && numberFormat[lastItem] is NumberFormatText) {
									month = numberFormat[lastItem - 1] as NumberFormatMonth;
									if (month != null) {
										hasTime = true;
										numberFormat[lastItem - 1] = new NumberFormatMinutes(month.IsLong);
										hasDate = HasDate(numberFormat);
									}
								}
						}
						hasTime = true;
						numberFormat.Add(ParseSeconds(formatString, i, textAfter, out blockLength));
						break;
					case 'a':
					case 'A':
						if ((i + 5) <= formatString.Length && formatString.Substring(i, 5).ToLowerInvariant() == "am/pm") {
							blockLength = 5;
							hasTime = true;
							numberFormat.Add(new NumberFormatAmPm());
						}
						break;
				}
				if (blockLength == 0) {
					blockLength = GetText(formatString, i, textAfter);
					numberFormat.Add(new NumberFormatText(textAfter.ToString()));
				}
				else {
					string text = textAfter.ToString();
					if (!string.IsNullOrEmpty(text))
						numberFormat.Add(new NumberFormatText(text));
				}
				i += blockLength;
			}
			numberFormatType = GetNumberFormatType(hasCurrencySymbol, hasNumber, hasPercentage, hasDate, hasTime);
			return numberFormat;
		}
		CellValueOdsType GetNumberFormatType(bool hasCurrencySymbol, bool hasNumber, bool hasPercentage, bool hasDate, bool hasTime) {
			if (hasPercentage && hasNumber)
				return CellValueOdsType.Percentage;
			if (hasDate)
				return CellValueOdsType.Date;
			if (hasTime)
				return CellValueOdsType.Time;
			if (hasCurrencySymbol)
				return CellValueOdsType.Currency;
			if (hasNumber)
				return CellValueOdsType.Float;
			return CellValueOdsType.String;
		}
		bool HasDate(List<NumberFormatElement> elements) {
			foreach (NumberFormatElement element in elements)
				if (element is NumberFormatDay ||
					element is NumberFormatDayOfWeek ||
					element is NumberFormatMonth ||
					element is NumberFormatYear)
					return true;
			return false;
		}
		int GetDateTimeBlockLength(string text, char dateTimeSymbol, int index) {
			for (int i = index; i < text.Length; ++i)
				if (char.ToLowerInvariant(text[i]) != dateTimeSymbol)
					return i - index;
			return text.Length - index;
		}
		#region Parsing
		NumberFormatElement ParseSeconds(string formatString, int currentIndex, StringBuilder textAfter, out int blockLength) {
			int startIndex = currentIndex;
			int charSCount = GetDateTimeBlockLength(formatString, 's', currentIndex);
			currentIndex += charSCount;
			int msStart = formatString.IndexOf('.', currentIndex);
			if (msStart < 0 || formatString.Length <= msStart + 1) {
				blockLength = charSCount;
				return new NumberFormatSeconds(charSCount > 1, -1);
			}
			currentIndex = ++msStart;
			int expectedTextLength = msStart - currentIndex;
			int actualTextLength = GetText(formatString, currentIndex, textAfter);
			if (expectedTextLength != actualTextLength) {
				blockLength = charSCount;
				return new NumberFormatSeconds(charSCount > 1, -1);
			}
			currentIndex += actualTextLength;
			int zeroCount = currentIndex;
			while (currentIndex < formatString.Length && formatString[currentIndex] == '0')
				++currentIndex;
			zeroCount = currentIndex - zeroCount;
			if (zeroCount == 0)
				textAfter.Append('.');
			else
				if (zeroCount > 3)
					zeroCount = 3;
			blockLength = currentIndex - startIndex;
			return new NumberFormatSeconds(charSCount > 1, zeroCount);
		}
		NumberFormatElement ParseBracket(string formatString, int currentIndex, out int blockLength) {
			if (TryParseLocale(formatString, currentIndex, out blockLength) || TryParseColor(formatString, currentIndex, out blockLength))
				return null;
			return ParseTruncateOnOverflow(formatString, currentIndex, out blockLength);
		}
		bool TryParseColor(string formatString, int currentIndex, out int blockLength) {
			blockLength = -1;
			return false;
		}
		bool TryParseLocale(string formatString, int currentIndex, out int blockLength) {
			++currentIndex;
			if (formatString.Length <= currentIndex || formatString[currentIndex] != '$' || formatString[currentIndex + 1] != '-') {
				blockLength = -1;
				return false;
			}
			blockLength = formatString.IndexOf(']', currentIndex);
			if (blockLength < 0)
				return false;
			blockLength += 2 - currentIndex;
			return true;
		}
		NumberFormatElement ParseTruncateOnOverflow(string formatString, int currentIndex, out int blockLength) {
			++currentIndex;
			if (formatString.Length > currentIndex) {
				char currentLower = char.ToLowerInvariant(formatString[currentIndex]);
				if (currentLower == 'h' || currentLower == 'm' || currentLower == 's') {
					blockLength = GetDateTimeBlockLength(formatString, currentLower, currentIndex);
					if (formatString[currentIndex + blockLength] != ']') {
						blockLength = -1;
						return null;
					}
					bool isLong = blockLength > 1;
					blockLength += 2; 
					switch (currentLower) 
					{
						case 's': 
							NumberFormatElement seconds = ParseSeconds(formatString, currentIndex, new StringBuilder(), out blockLength);
							++blockLength; 
							return seconds;
						case 'm':
							return new NumberFormatMinutes(isLong);
						case 'h':
							return new NumberFormatHours(isLong);
						default:
							blockLength = -1;
							return null;
					}
				}
			}
			blockLength = -1;
			return null;
		}
		NumberFormatElement ParseNumber(string value, StringBuilder textAfter) {
			string[] number = SplitNumber(value);
			if (number == null)
				return ParseNumberCore(value, textAfter, true, false);
			if (number.Length == 2)
				return new NumberFormatFractionNumber(
					new NumberFormatSimpleNumber(false, -1, -1, null, -1, null),
					ParseNumberCore(number[0], textAfter, false, true),
					ParseNumberCore(number[1], textAfter, false, true));
			return new NumberFormatFractionNumber(
					ParseNumberCore(number[0], textAfter, false, false),
					ParseNumberCore(number[1], textAfter, false, true),
					ParseNumberCore(number[2], textAfter, false, true));
		}
		NumberFormatNumberBase ParseNumberCore(string value, StringBuilder textAfter, bool allowEmbeddedText, bool sharpsAsZeros) {
			bool grouping = false;
			bool isDecimal = false;
			int minIntegerDigits = 0;
			int minIntegerDigitsSharps = 0;
			int minExponentDigits = -1;
			int decimalPlaces = 0;
			int decimalPlacesSharps = 0;
			StringBuilder sb = new StringBuilder();
			string embText;
			int last = value.Length - 1;
			while (last > 0 && value[last] == ',')
				--last;
			++last;
			int displayFactor = 1;
			if (last < value.Length) {
				displayFactor = (int)Math.Pow(1000, value.Length - last);
				value = value.Remove(last);
			}
			List<NumberFormatEmbeddedText> embeddedTextList = new List<NumberFormatEmbeddedText>();
			for (int i = 0; i < value.Length; ++i) {
				char current = value[i];
				switch (current) {
					case '\\':
						i = OnBackslash(value, i, sb);
						break;
					case '"':
						i = OnQuotes(value, i, sb);
						break;
					case '_':
						i = OnUnderline(value, i, sb);
						break;
					case '*':
						i = OnAsterisk(value, i, sb);
						break;
					case '.':
						if (!isDecimal) {
							embText = sb.ToString();
							if (!string.IsNullOrEmpty(embText)) {
								embeddedTextList.Insert(0, new NumberFormatEmbeddedText(embText, GetPosForEmbeddedText(value, i)));
								sb.Clear();
							}
						}
						isDecimal = true;
						break;
					case '#':
						if (!isDecimal) {
							embText = sb.ToString();
							if (!string.IsNullOrEmpty(embText)) {
								embeddedTextList.Insert(0, new NumberFormatEmbeddedText(embText, GetPosForEmbeddedText(value, i)));
								sb.Clear();
							}
						}
						if (isDecimal)
							++decimalPlacesSharps;
						else
							++minIntegerDigitsSharps;
						break;
					case '0':
					case '?':
						if (!isDecimal) {
							embText = sb.ToString();
							if (!string.IsNullOrEmpty(embText)) {
								embeddedTextList.Insert(0, new NumberFormatEmbeddedText(embText, GetPosForEmbeddedText(value, i)));
								sb.Clear();
							}
						}
						if (isDecimal)
							++decimalPlaces;
						else
							++minIntegerDigits;
						break;
					case ',':
						if (i > 0 && value.Length > (i + 1)) {
							char prev = value[i - 1];
							char next = value[i + 1];
							if (IsNumericSymbol(prev) && IsNumericSymbol(next))
								grouping = true;
							else
								sb.Append(',');
						}
						if (!isDecimal) {
							embText = sb.ToString();
							if (!string.IsNullOrEmpty(embText)) {
								embeddedTextList.Insert(0, new NumberFormatEmbeddedText(embText, GetPosForEmbeddedText(value, i)));
								sb.Clear();
							}
						}
						break;
					case 'E':
						if (value.Length <= i + 2 || value[i + 1] != '+' && value[i + 1] != '-') {
							sb.Append('E');
							break;
						}
						minExponentDigits = 0;
						int lastZero = value.LastIndexOfAny(NumericNumberFormatSymbols.ToArray());
						while (lastZero > i) {
							++minExponentDigits;
							value = value.Remove(lastZero, 1);
							lastZero = value.LastIndexOfAny(NumericNumberFormatSymbols.ToArray());
						}
						++i;
						break;
					default:
						sb.Append(current);
						break;
				}
			}
			if (!allowEmbeddedText) {
				foreach (NumberFormatEmbeddedText text in embeddedTextList)
					textAfter.Append(text.Text);
				embeddedTextList.Clear();
			}
			textAfter.Append(sb.ToString());
			if (minExponentDigits >= 0) {
				minIntegerDigits = minIntegerDigits > 0 ? minIntegerDigits : minIntegerDigitsSharps;
				decimalPlaces = decimalPlaces > 0 ? decimalPlaces : decimalPlacesSharps;
				return new NumberFormatScientificNumber(grouping, minIntegerDigits, decimalPlaces, minExponentDigits, embeddedTextList);
			}
			if (sharpsAsZeros) {
				minIntegerDigits += minIntegerDigitsSharps;
			}
			string decimalReplacement = NumberFormatSimpleNumber.DefaultDecimalReplacement;
			if (decimalPlaces <= 0 && decimalPlacesSharps > 0) {
				decimalPlaces = decimalPlacesSharps;
				decimalReplacement = string.Empty;
			}
			return new NumberFormatSimpleNumber(grouping, minIntegerDigits, decimalPlaces, decimalReplacement, displayFactor, embeddedTextList);
		}
		string[] SplitNumber(string formatString) {
			int splitterIndex = GetNonTextSymbolPosition(formatString, '/', false);
			if (splitterIndex <= 0)
				return null;
			string integerAndNumerator = formatString.Substring(0, splitterIndex);
			++splitterIndex;
			string denominator = formatString.Substring(splitterIndex, formatString.Length - splitterIndex);
			splitterIndex = integerAndNumerator.Length - 1;
			while (splitterIndex >= 0 && IsNumericSymbol(integerAndNumerator[splitterIndex]))
				--splitterIndex;
			if (splitterIndex <= 0)
				return new string[] { integerAndNumerator, denominator };
			string integer = integerAndNumerator.Substring(0, splitterIndex);
			++splitterIndex;
			string numerator = integerAndNumerator.Substring(splitterIndex, integerAndNumerator.Length - splitterIndex);
			return new string[] { integer, numerator, denominator };
		}
		int GetText(string formatString, int currentIndex, StringBuilder text) {
			int i = currentIndex;
			for (; i < formatString.Length; ++i) {
				char current = formatString[i];
				if (current == '\\') {
					i = OnBackslash(formatString, i, text);
					continue;
				}
				if (current == '"') {
					i = OnQuotes(formatString, i, text);
					continue;
				}
				if (current == '_') {
					i = OnUnderline(formatString, i, text);
					continue;
				}
				if (current == '*') {
					i = OnAsterisk(formatString, i, text);
					continue;
				}
				if (NumberFormatSymbols.Contains(char.ToLowerInvariant(current)))
					break;
				else
					text.Append(current);
			}
			return i - currentIndex;
		}
		int OnBackslash(string formatString, int index, StringBuilder text) {
			++index;
			text.Append(formatString[index]);
			return index;
		}
		int OnQuotes(string formatString, int index, StringBuilder text) {
			++index;
			if (index >= formatString.Length)
				return index;
			int nextQuote = formatString.IndexOf('"', index);
			if (nextQuote < 0)
				return index;
			text.Append(formatString, index, nextQuote - index);
			return nextQuote;
		}
		int OnUnderline(string formatString, int index, StringBuilder text) {
			text.Append(' ');
			return index + 1;
		}
		int OnAsterisk(string formatString, int index, StringBuilder text) {
			return index + 1;
		}
		int GetPosForEmbeddedText(string formatString, int currentIndex) {
			bool isQuoted = false;
			int dotPos = -1;
			for (int i = 0; i < formatString.Length; ++i)
				if (isQuoted) {
					if (formatString[i] == '"' && formatString[i - 1] != '\\')
						isQuoted = false;
				}
				else {
					char current = formatString[i];
					if (i == 0 || formatString[i - 1] != '\\') {
						if (current == '.') {
							dotPos = i;
							break;
						}
						if (current == '"')
							isQuoted = true;
					}
				}
			if (dotPos >= 0)
				formatString = formatString.Remove(dotPos);
			int result = 0;
			for (int i = formatString.Length - 1; i >= currentIndex; --i)
				if (IsNumericSymbol(formatString[i]))
					++result;
			return result;
		}
		#endregion
		#region NumberFormatElements
		abstract class NumberFormatElement {
			protected abstract string TagName { get; }
			protected virtual string NS { get { return NumberNamespace; } }
			public void Generate(OpenDocumentExporter exporter) {
				exporter.WriteStartElement(TagName, NS);
				try {
					GenerateCore(exporter);
				}
				finally {
					exporter.WriteEndElement();
				}
			}
			protected virtual void GenerateCore(OpenDocumentExporter exporter) {
			}
		}
		class NumberFormatCurrencySymbol : NumberFormatElement {
			string currencySymbol;
			public NumberFormatCurrencySymbol(string currencySymbol) {
				this.currencySymbol = currencySymbol;
			}
			protected override string TagName { get { return "currency-symbol"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				exporter.WriteString(currencySymbol);
			}
		}
		abstract class NumberFormatNumberBase : NumberFormatElement {
			bool grouping;
			int minIntegerDigits;
			int decimalPlaces;
			List<NumberFormatEmbeddedText> embeddedTextList;
			protected NumberFormatNumberBase(bool grouping, int minIntegerDigits, int decimalPlaces, List<NumberFormatEmbeddedText> embeddedTextList) {
				this.grouping = grouping;
				this.minIntegerDigits = minIntegerDigits;
				this.decimalPlaces = decimalPlaces;
				this.embeddedTextList = embeddedTextList;
			}
			public bool Grouping { get { return grouping; } }
			public int MinIntegerDigits { get { return minIntegerDigits; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				if (grouping)
					exporter.WriteBoolAttr("grouping", NumberNamespace, true);
				exporter.WriteNumericAttr("min-integer-digits", NumberNamespace, minIntegerDigits);
				exporter.WriteNumericAttr("decimal-places", NumberNamespace, decimalPlaces);
				GenerateSpecifiedNumber(exporter);
				foreach (NumberFormatEmbeddedText embeddedText in embeddedTextList)
					embeddedText.Generate(exporter);
			}
			protected virtual void GenerateSpecifiedNumber(OpenDocumentExporter exporter) {
			}
		}
		class NumberFormatSimpleNumber : NumberFormatNumberBase {
			public const string DefaultDecimalReplacement = "0";
			int displayFactor;
			string decimalReplacement;
			public NumberFormatSimpleNumber(bool grouping, int minIntegerDigits, int decimalPlaces, string decimalReplacement, int displayFactor, List<NumberFormatEmbeddedText> embeddedTextList) :
				base(grouping, minIntegerDigits, decimalPlaces, embeddedTextList) {
				this.decimalReplacement = decimalReplacement;
				this.displayFactor = displayFactor;
			}
			protected override string TagName { get { return "number"; } }
			protected override void GenerateSpecifiedNumber(OpenDocumentExporter exporter) {
				if (!decimalReplacement.Equals(DefaultDecimalReplacement))
					exporter.WriteStringAttr("decimal-replacement", NumberNamespace, decimalReplacement);
				if (displayFactor > 1)
					exporter.WriteNumericAttr("display-factor", NumberNamespace, displayFactor);
			}
		}
		class NumberFormatScientificNumber : NumberFormatNumberBase {
			int minExponentDigits;
			public NumberFormatScientificNumber(bool grouping, int minIntegerDigits, int decimalPlaces, int minExponentDigits, List<NumberFormatEmbeddedText> embeddedTextList)
				: base(grouping, minIntegerDigits, decimalPlaces, embeddedTextList) {
				this.minExponentDigits = minExponentDigits;
			}
			protected override string TagName { get { return "scientific-number"; } }
			protected override void GenerateSpecifiedNumber(OpenDocumentExporter exporter) {
				exporter.WriteNumericAttr("min-exponent-digits", NumberNamespace, minExponentDigits);
			}
		}
		class NumberFormatFractionNumber : NumberFormatNumberBase {
			int minNumeratorDigits;
			int minDenominatorDigits;
			public NumberFormatFractionNumber(NumberFormatNumberBase integer, NumberFormatNumberBase numerator, NumberFormatNumberBase denominator) :
				base(integer.Grouping, integer.MinIntegerDigits, 0, new List<NumberFormatEmbeddedText>()) {
				this.minNumeratorDigits = numerator.MinIntegerDigits;
				this.minDenominatorDigits = denominator.MinIntegerDigits;
			}
			protected override string TagName { get { return "fraction"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				if (Grouping)
					exporter.WriteBoolAttr("grouping", NumberNamespace, true);
				if (MinIntegerDigits >= 0)
					exporter.WriteNumericAttr("min-integer-digits", NumberNamespace, MinIntegerDigits);
				exporter.WriteNumericAttr("min-numerator-digits", NumberNamespace, minNumeratorDigits);
				exporter.WriteNumericAttr("min-denominator-digits", NumberNamespace, minDenominatorDigits);
			}
		}
		class NumberFormatTextContent : NumberFormatElement {
			protected override string TagName { get { return "text-content"; } }
		}
		class NumberFormatText : NumberFormatElement {
			string text;
			public NumberFormatText(string text) {
				this.text = text;
			}
			public string Text { get { return text; } }
			protected override string TagName { get { return "text"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				exporter.WriteString(text);
			}
		}
		class NumberFormatEmbeddedText : NumberFormatText {
			int position;
			public NumberFormatEmbeddedText(string text, int position)
				: base(text) {
				this.position = position;
			}
			protected override string TagName { get { return "embedded-text"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				exporter.WriteNumericAttr("position", NumberNamespace, position);
				base.GenerateCore(exporter);
			}
		}
		abstract class NumberFormatTimeBase : NumberFormatElement {
			bool isLong;
			protected NumberFormatTimeBase(bool isLong) {
				this.isLong = isLong;
			}
			public bool IsLong { get { return isLong; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				if (isLong)
					exporter.WriteStringAttr("style", NumberNamespace, "long");
			}
		}
		class NumberFormatYear : NumberFormatTimeBase {
			public NumberFormatYear(bool isLong)
				: base(isLong) {
			}
			protected override string TagName { get { return "year"; } }
		}
		class NumberFormatMonth : NumberFormatTimeBase {
			bool isTextual;
			public NumberFormatMonth(bool isLong, bool isTextual)
				: base(isLong) {
				this.isTextual = isTextual;
			}
			protected override string TagName { get { return "month"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				base.GenerateCore(exporter);
				if (isTextual)
					exporter.WriteBoolAttr("textual", NumberNamespace, true);
			}
		}
		class NumberFormatDay : NumberFormatTimeBase {
			public NumberFormatDay(bool isLong)
				: base(isLong) {
			}
			protected override string TagName { get { return "day"; } }
		}
		class NumberFormatDayOfWeek : NumberFormatTimeBase {
			public NumberFormatDayOfWeek(bool isLong)
				: base(isLong) {
			}
			protected override string TagName { get { return "day-of-week"; } }
		}
		class NumberFormatHours : NumberFormatTimeBase {
			public NumberFormatHours(bool isLong)
				: base(isLong) {
			}
			protected override string TagName { get { return "hours"; } }
		}
		class NumberFormatMinutes : NumberFormatTimeBase {
			public NumberFormatMinutes(bool isLong)
				: base(isLong) {
			}
			protected override string TagName { get { return "minutes"; } }
		}
		class NumberFormatSeconds : NumberFormatTimeBase {
			int decimalPlaces;
			public NumberFormatSeconds(bool isLong, int decimalPlaces)
				: base(isLong) {
				this.decimalPlaces = decimalPlaces;
			}
			protected override string TagName { get { return "seconds"; } }
			protected override void GenerateCore(OpenDocumentExporter exporter) {
				if (decimalPlaces > 0)
					exporter.WriteNumericAttr("decimal-places", NumberNamespace, decimalPlaces);
				base.GenerateCore(exporter);
			}
		}
		class NumberFormatAmPm : NumberFormatElement {
			protected override string TagName { get { return "am-pm"; } }
		}
		class StyleMap {
			public static StyleMap Parse(string formatString) {
				if (string.IsNullOrEmpty(formatString) || formatString.Length < 4 || formatString[0] != '[')
					return new StyleMap(string.Empty, formatString);
				char currentSymbol = formatString[1];
				if (currentSymbol != '=' && currentSymbol != '!' && currentSymbol != '<' && currentSymbol != '>')
					return new StyleMap(string.Empty, formatString);
				int conditionEnd = formatString.IndexOf(']', 1);
				if (conditionEnd < 0)
					return new StyleMap(string.Empty, formatString);
				string condition = formatString.Substring(1, conditionEnd - 1);
				++conditionEnd;
				formatString = formatString.Substring(conditionEnd, formatString.Length - conditionEnd);
				return new StyleMap(condition, formatString);
			}
			string condition;
			string formatString;
			public StyleMap(string condition, string formatString) {
				this.condition = condition;
				this.formatString = formatString;
			}
			public bool HasCondition { get { return !string.IsNullOrEmpty(condition); } }
			public string FormatString { get { return formatString; } }
			public void SetCondition(string condition) {
				this.condition = condition;
			}
			public void Generate(OpenDocumentExporter exporter, string applyStyleName) {
				if (string.IsNullOrEmpty(condition))
					return;
				exporter.WriteStartElement("map", StyleNamespace);
				try {
					exporter.WriteStringAttr("apply-style-name", StyleNamespace, applyStyleName);
					exporter.WriteStringAttr("condition", StyleNamespace, "value()" + condition);
				}
				finally {
					exporter.WriteEndElement();
				}
			}
		}
		#endregion
		#endregion
		#region GenerateCellFormats
		protected internal void GenerateCellFormats() {
			foreach (int index in exportStyleSheet.CellFormatTable.Keys) {
				CellFormat cellFormat = (CellFormat)Workbook.Cache.CellFormatCache[index];
				GenerateCellFormat(exportStyleSheet.GetCellFormatName(index), cellFormat, null);
			}
		}
		protected internal void GenerateCellFormat(string styleName, CellFormat cellFormat, Action generateConditions) {
			WriteStartElement("style", StyleNamespace);
			try {
				CellStyleBase parentStyle = Workbook.StyleSheet.CellStyles[cellFormat.StyleIndex];
				CellFormatBase parentFormat = parentStyle.FormatInfo;
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table-cell");
				WriteStringAttr("parent-style-name", StyleNamespace, exportStyleSheet.GetCellStyleName(parentStyle));
				if (cellFormat.NumberFormatIndex != parentFormat.NumberFormatIndex)
					WriteStringAttr("data-style-name", StyleNamespace, exportStyleSheet.GetNumberFormatName(cellFormat.NumberFormatIndex));
				GenerateCellProperties(cellFormat, parentFormat);
				if (generateConditions != null)
					generateConditions();
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateCellProperties(CellFormatBase format, CellFormatBase parentFormat) {
			GenerateTableCellProperies(format, parentFormat);
			GenerateParagraphProperties(format, parentFormat);
			GenerateTextProperties(format, parentFormat);
		}
		#endregion
		#region GenerateDifferentialFormats
		protected internal void GenerateDifferentialFormats() {
			foreach (int index in exportStyleSheet.DifferentialFormatTable.Keys) {
				DifferentialFormat format = (DifferentialFormat)Workbook.Cache.CellFormatCache[index];
				GenerateDifferentialFormat(exportStyleSheet.GetDifferentialFormatName(index), format);
			}
		}
		protected internal void GenerateDifferentialFormat(string styleName, DifferentialFormat format) {
			WriteStartElement("style", StyleNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table-cell");
				if (!format.NumberFormatInfo.FormatCode.Equals(NumberFormat.Generic.FormatCode, StringComparison.Ordinal))
					WriteStringAttr("data-style-name", StyleNamespace, exportStyleSheet.GetNumberFormatName(format.NumberFormatIndex));
				if (Workbook.Cache.CellFormatCache.GetItemIndex(format) != CellFormatCache.DefaultDifferentialFormatIndex) {
					GenerateDifferentialTableCellProperties(format);
					GenerateDifferentialParagraphProperties(format);
					GenerateDifferentialTextProperties(format);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateCellStyles
		protected internal void GenerateCellStyles() {
			GenerateDefaultCellStyle();
			CellStyleFormat parentFormat = Workbook.StyleSheet.CellStyles.Normal.FormatInfo;
			foreach (int index in exportStyleSheet.CellStyleIndexTable.Keys) {
				CellStyleBase style = Workbook.StyleSheet.CellStyles[index];
				GenerateStyleFormat(exportStyleSheet.GetCellStyleName(style), style.FormatInfo, parentFormat);
			}
		}
		void GenerateDefaultCellStyle() {
			exportStyleSheet.CellStyleIndexTable.Remove(0);
			CellStyleBase style = Workbook.StyleSheet.CellStyles.Normal;
			CellStyleFormat parentFormat = new CellStyleFormat(Workbook);
			FillInfo differentFillInfo = new FillInfo() { PatternType = XlPatternType.Solid }; 
			CellAlignmentInfo differentAlignInfo = new CellAlignmentInfo() { VerticalAlignment = XlVerticalAlignment.Center }; 
			RunFontInfo differentFontInfo = new RunFontInfo() { ColorIndex = 100, Name = "sss", Size = 100 }; 
			parentFormat.AssignFillIndex(Workbook.Cache.FillInfoCache.GetItemIndex(differentFillInfo));
			parentFormat.AssignAlignmentIndex(Workbook.Cache.CellAlignmentInfoCache.GetItemIndex(differentAlignInfo));
			parentFormat.AssignFontIndex(Workbook.Cache.FontInfoCache.GetItemIndex(differentFontInfo));
			string temp = VerticalAlignmentTable[XlVerticalAlignment.Bottom];
			VerticalAlignmentTable[XlVerticalAlignment.Bottom] = "automatic";
			GenerateStyleFormat(exportStyleSheet.GetCellStyleName(style), style.FormatInfo, parentFormat);
			VerticalAlignmentTable[XlVerticalAlignment.Bottom] = temp;
		}
		void GenerateStyleFormat(string styleName, CellFormatBase format, CellFormatBase parentFormat) {
			WriteStartElement("style", StyleNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table-cell");
				WriteStringAttr("data-style-name", StyleNamespace, exportStyleSheet.GetNumberFormatName(format.NumberFormatIndex));
				GenerateCellProperties(format, parentFormat);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateColumnFormats
		protected internal void GenerateColumnFormats() {
			foreach (OdsColumnFormat format in exportStyleSheet.ColumnFormatTable.Keys)
				GenerateColumnFormat(exportStyleSheet.GetColumnFormatName(format), format);
		}
		protected internal void GenerateColumnFormat(string styleName, OdsColumnFormat format) {
			WriteStartElement("style", StyleNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table-column");
				format.Generate(this);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateRowFormats
		protected internal void GenerateRowFormats() {
			foreach (OdsRowFormat format in exportStyleSheet.RowFormatTable.Keys)
				GenerateRowFormat(exportStyleSheet.GetRowFormatName(format), format);
		}
		protected internal void GenerateRowFormat(string styleName, OdsRowFormat format) {
			WriteStartElement("style", StyleNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table-row");
				format.Generate(this);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateSheetFormats
		protected internal void GenerateSheetFormats() {
			foreach (OdsTableFormat format in exportStyleSheet.TableFormatTable.Keys) {
				GenerateSheetFormat(exportStyleSheet.GetTableFormatName(format), format);
			}
		}
		protected internal void GenerateSheetFormat(string styleName, OdsTableFormat format) {
			WriteStartElement("style", StyleNamespace);
			try {
				WriteStringAttr("name", StyleNamespace, styleName);
				WriteStringAttr("family", StyleNamespace, "table");
				format.Generate(this);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateTableCellProperties
		protected internal void GenerateTableCellProperies(CellFormatBase cellFormat, CellFormatBase parentFormat) {
			if (cellFormat.FillIndex != parentFormat.FillIndex ||
				cellFormat.BorderIndex != parentFormat.BorderIndex ||
				cellFormat.AlignmentIndex != parentFormat.AlignmentIndex ||
				cellFormat.Protection.Locked != parentFormat.Protection.Locked ||
				cellFormat.Protection.Hidden != parentFormat.Protection.Hidden) {
				WriteStartElement("table-cell-properties", StyleNamespace);
				try {
					GenerateTableCellPropertiesCore(cellFormat, parentFormat);
				}
				finally {
					WriteEndElement();
				}
			}
		}
		void GenerateTableCellPropertiesCore(CellFormatBase format, CellFormatBase parentFormat) {
			GenerateFillInfo(format.FillInfo, parentFormat.FillInfo);
			GenerateBorderInfo(format.BorderInfo, parentFormat.BorderInfo);
			GenerateCellAlignmentInfo(format.AlignmentInfo, parentFormat.AlignmentInfo);
			GenerateProtection(format.Protection, parentFormat.Protection);
		}
		void GenerateFillInfo(FillInfo info, FillInfo parentInfo) {
			if (info.PatternType == XlPatternType.Solid) {
				if (info.ForeColorIndex != parentInfo.ForeColorIndex)
					WriteStringAttr("background-color", FoNamespace, ConvertColorToString(info.ForeColorIndex, "transparent"));
			}
			else
				if (parentInfo.PatternType == XlPatternType.Solid)
					WriteStringAttr("background-color", FoNamespace, "transparent");
		}
		void GenerateBorderInfo(BorderInfo info, BorderInfo parentInfo) {
			bool baseLineDiffersfromParent;
			baseLineDiffersfromParent = info.VerticalLineStyle != parentInfo.VerticalLineStyle || info.VerticalColorIndex != parentInfo.VerticalColorIndex;
			if (info.LeftLineStyle != parentInfo.LeftLineStyle || info.LeftColorIndex != parentInfo.LeftColorIndex || baseLineDiffersfromParent)
				GenerateBorderInfoCore("border-left", FoNamespace, info.LeftLineStyle, info.LeftColorIndex, info.VerticalLineStyle, info.VerticalColorIndex);
			if (info.RightLineStyle != parentInfo.RightLineStyle || info.RightColorIndex != parentInfo.RightColorIndex || baseLineDiffersfromParent)
				GenerateBorderInfoCore("border-right", FoNamespace, info.RightLineStyle, info.RightColorIndex, info.VerticalLineStyle, info.VerticalColorIndex);
			baseLineDiffersfromParent = info.HorizontalLineStyle != parentInfo.HorizontalLineStyle || info.HorizontalColorIndex != parentInfo.HorizontalColorIndex;
			if (info.TopLineStyle != parentInfo.TopLineStyle || info.TopColorIndex != parentInfo.TopColorIndex || baseLineDiffersfromParent)
				GenerateBorderInfoCore("border-top", FoNamespace, info.TopLineStyle, info.TopColorIndex, info.HorizontalLineStyle, info.HorizontalColorIndex);
			if (info.BottomLineStyle != parentInfo.BottomLineStyle || info.BottomColorIndex != parentInfo.BottomColorIndex || baseLineDiffersfromParent)
				GenerateBorderInfoCore("border-bottom", FoNamespace, info.BottomLineStyle, info.BottomColorIndex, info.HorizontalLineStyle, info.HorizontalColorIndex);
			baseLineDiffersfromParent = info.DiagonalColorIndex != parentInfo.DiagonalColorIndex;
			if (info.DiagonalUpLineStyle != parentInfo.DiagonalUpLineStyle || baseLineDiffersfromParent)
				GenerateBorderInfoCore("diagonal-bl-tr", StyleNamespace, info.DiagonalUpLineStyle, info.DiagonalColorIndex, XlBorderLineStyle.None, 0);
			if (info.DiagonalDownLineStyle != parentInfo.DiagonalDownLineStyle || baseLineDiffersfromParent)
				GenerateBorderInfoCore("diagonal-tl-br", StyleNamespace, info.DiagonalDownLineStyle, info.DiagonalColorIndex, XlBorderLineStyle.None, 0);
		}
		void GenerateBorderInfoCore(string attributeName, string ns, XlBorderLineStyle lineStyleInfo, int colorIndex, XlBorderLineStyle baseLineStyleInfo, int baseColorIndex) {
			int lineWidth;
			string lineStyle;
			if (lineStyleInfo == XlBorderLineStyle.None) {
				lineStyleInfo = baseLineStyleInfo;
				colorIndex = baseColorIndex;
			}
			switch (lineStyleInfo) {
				case XlBorderLineStyle.Thin:
					lineStyle = "solid";
					lineWidth = 1;
					break;
				case XlBorderLineStyle.Medium:
					lineStyle = "solid";
					lineWidth = 2;
					break;
				case XlBorderLineStyle.Thick:
					lineStyle = "solid";
					lineWidth = 3;
					break;
				case XlBorderLineStyle.Dashed:
					lineStyle = "dashed";
					lineWidth = 1;
					break;
				case XlBorderLineStyle.MediumDashed:
					lineStyle = "dashed";
					lineWidth = 2;
					break;
				case XlBorderLineStyle.Dotted:
					lineStyle = "dotted";
					lineWidth = 1;
					break;
				case XlBorderLineStyle.Double:
					lineStyle = "double";
					lineWidth = 1;
					break;
				default:
					return;
			}
			string borderParams = lineWidth.ToString("0") + "pt " + lineStyle + " " + ConvertColorToString(colorIndex, "transparent");
			WriteStringAttr(attributeName, ns, borderParams);
		}
		void GenerateCellAlignmentInfo(CellAlignmentInfo info, CellAlignmentInfo parentInfo) {
			string temp;
			if (info.WrapText != parentInfo.WrapText)
				WriteStringAttr("wrap-option", FoNamespace, info.WrapText ? "wrap" : "no-wrap");
			if (info.VerticalAlignment != parentInfo.VerticalAlignment)
				if (VerticalAlignmentTable.TryGetValue(info.VerticalAlignment, out temp))
					WriteStringAttr("vertical-align", StyleNamespace, temp);
			if (info.ReadingOrder != parentInfo.ReadingOrder)
				if (ReadingOrderTable.TryGetValue(info.ReadingOrder, out temp))
					WriteStringAttr("writing-mode", StyleNamespace, temp);
			if (info.TextRotation != parentInfo.TextRotation) {
				int degree = Workbook.UnitConverter.ModelUnitsToDegree(info.TextRotation);
				WriteStringAttr("rotation-angle", StyleNamespace, degree.ToString());
			}
			if (info.ShrinkToFit != parentInfo.ShrinkToFit)
				WriteBoolAttr("shrink-to-fit", StyleNamespace, info.ShrinkToFit);
			if (info.HorizontalAlignment != parentInfo.HorizontalAlignment)
				if (info.HorizontalAlignment == XlHorizontalAlignment.Fill)
					WriteStringAttr("repeat-content", StyleNamespace, "true");
				else
					if (parentInfo.HorizontalAlignment == XlHorizontalAlignment.Fill)
						WriteStringAttr("repeat-content", StyleNamespace, "false");
		}
		void GenerateProtection(ICellProtectionInfo info, ICellProtectionInfo parentInfo) {
			if (info.Hidden == parentInfo.Hidden && info.Locked == parentInfo.Locked)
				return;
			if (info.Hidden) {
				string protectParams = info.Locked ? "protected formula-hidden" : "formula-hidden";
				WriteStringAttr("cell-protect", StyleNamespace, protectParams);
			}
			else
				if (!info.Locked)
					WriteStringAttr("cell-protect", StyleNamespace, "none");
		}
		#endregion
		#region GenerateDifferentialTableCellProperties
		protected internal void GenerateDifferentialTableCellProperties(DifferentialFormat format) {
			WriteStartElement("table-cell-properties", StyleNamespace);
			try {
				GenerateAlignment(format);
				GenerateBorder(format);
				GenerateFill(format);
				GenerateProtection(format);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateAlignment(DifferentialFormat format) {
			string temp;
			if (format.MultiOptionsInfo.ApplyAlignmentWrapText)
				if (format.AlignmentInfo.WrapText)
					WriteStringAttr("wrap-option", FoNamespace, "wrap");
				else
					WriteStringAttr("wrap-option", FoNamespace, "no-wrap");
			if (format.MultiOptionsInfo.ApplyAlignmentVertical)
				if (VerticalAlignmentTable.TryGetValue(format.AlignmentInfo.VerticalAlignment, out temp))
					WriteStringAttr("vertical-align", StyleNamespace, temp);
				else
					WriteStringAttr("vertical-align", StyleNamespace, "automatic");
			if (format.MultiOptionsInfo.ApplyAlignmentReadingOrder)
				if (ReadingOrderTable.TryGetValue(format.AlignmentInfo.ReadingOrder, out temp))
					WriteStringAttr("writing-mode", StyleNamespace, temp);
			if (format.MultiOptionsInfo.ApplyAlignmentTextRotation) {
				int degree = Workbook.UnitConverter.ModelUnitsToDegree(format.AlignmentInfo.TextRotation);
				WriteStringAttr("rotation-angle", StyleNamespace, degree.ToString());
			}
			if (format.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
				WriteBoolAttr("shrink-to-fit", StyleNamespace, format.AlignmentInfo.ShrinkToFit);
			if (format.MultiOptionsInfo.ApplyAlignmentHorizontal && format.Alignment.Horizontal == XlHorizontalAlignment.Fill)
				WriteStringAttr("repeat-content", StyleNamespace, "true");
		}
		void GenerateBorder(DifferentialFormat format) {
			if (format.BorderOptionsInfo.ApplyLeftColor || format.BorderOptionsInfo.ApplyLeftLineStyle || format.BorderOptionsInfo.ApplyVerticalColor || format.BorderOptionsInfo.ApplyVerticalLineStyle)
				GenerateBorderInfoCore("border-left", FoNamespace, format.BorderInfo.LeftLineStyle, format.BorderInfo.LeftColorIndex, format.BorderInfo.VerticalLineStyle, format.BorderInfo.VerticalColorIndex);
			if (format.BorderOptionsInfo.ApplyRightColor || format.BorderOptionsInfo.ApplyRightLineStyle || format.BorderOptionsInfo.ApplyVerticalColor || format.BorderOptionsInfo.ApplyVerticalLineStyle)
				GenerateBorderInfoCore("border-right", FoNamespace, format.BorderInfo.RightLineStyle, format.BorderInfo.RightColorIndex, format.BorderInfo.VerticalLineStyle, format.BorderInfo.VerticalColorIndex);
			if (format.BorderOptionsInfo.ApplyTopColor || format.BorderOptionsInfo.ApplyTopLineStyle || format.BorderOptionsInfo.ApplyHorizontalColor || format.BorderOptionsInfo.ApplyHorizontalLineStyle)
				GenerateBorderInfoCore("border-top", FoNamespace, format.BorderInfo.TopLineStyle, format.BorderInfo.TopColorIndex, format.BorderInfo.HorizontalLineStyle, format.BorderInfo.HorizontalColorIndex);
			if (format.BorderOptionsInfo.ApplyBottomColor || format.BorderOptionsInfo.ApplyBottomLineStyle || format.BorderOptionsInfo.ApplyHorizontalColor || format.BorderOptionsInfo.ApplyHorizontalLineStyle)
				GenerateBorderInfoCore("border-bottom", FoNamespace, format.BorderInfo.BottomLineStyle, format.BorderInfo.BottomColorIndex, format.BorderInfo.HorizontalLineStyle, format.BorderInfo.HorizontalColorIndex);
			if (format.BorderOptionsInfo.ApplyDiagonalUp || format.BorderOptionsInfo.ApplyDiagonalColor || format.BorderOptionsInfo.ApplyDiagonalLineStyle)
				GenerateBorderInfoCore("diagonal-bl-tr", StyleNamespace, format.BorderInfo.DiagonalUpLineStyle, format.BorderInfo.DiagonalColorIndex, XlBorderLineStyle.None, 0);
			if (format.BorderOptionsInfo.ApplyDiagonalDown || format.BorderOptionsInfo.ApplyDiagonalColor || format.BorderOptionsInfo.ApplyDiagonalLineStyle)
				GenerateBorderInfoCore("diagonal-tl-br", StyleNamespace, format.BorderInfo.DiagonalDownLineStyle, format.BorderInfo.DiagonalColorIndex, XlBorderLineStyle.None, 0);
		}
		void GenerateFill(DifferentialFormat format) {
			if (format.Fill.FillType == ModelFillType.Gradient)
				return;
			if (format.MultiOptionsInfo.ApplyFillForeColor && format.FillInfo.PatternType == XlPatternType.Solid)
				WriteStringAttr("background-color", FoNamespace, ConvertColorToString(format.FillInfo.ForeColorIndex, "transparent"));
		}
		void GenerateProtection(DifferentialFormat format) {
			string protectParams = string.Empty;
			if (format.MultiOptionsInfo.ApplyProtectionLocked)
				if (format.Protection.Locked)
					protectParams = "protected";
				else
					protectParams = "none";
			if (format.MultiOptionsInfo.ApplyProtectionHidden)
				if (format.Protection.Hidden)
					if (protectParams.Length == 4 || protectParams.Length == 0)
						protectParams = "formula-hidden";
					else
						protectParams += " formula-hidden";
				else
					if (protectParams.Length != 9)
						protectParams = "none";
			if (!string.IsNullOrEmpty(protectParams))
				WriteStringAttr("cell-protect", StyleNamespace, protectParams);
		}
		#endregion
		#region GenerateParagraphProperties
		protected internal void GenerateParagraphProperties(CellFormatBase format, CellFormatBase parentFormat) {
			if (!(format.AlignmentInfo.Indent != parentFormat.AlignmentInfo.Indent || format.AlignmentInfo.HorizontalAlignment != parentFormat.AlignmentInfo.HorizontalAlignment))
				return;
			WriteStartElement("paragraph-properties", StyleNamespace);
			try {
				string alignment;
				if (HorizontalAlignmentTable.TryGetValue(format.AlignmentInfo.HorizontalAlignment, out alignment)) {
					WriteStringAttr("text-align", FoNamespace, alignment);
					if ("start".Equals(alignment))
						WriteStringAttr("margin-left", FoNamespace, GetIndentInCentimeters(format.AlignmentInfo.Indent));
					else
						if ("end".Equals(alignment))
							WriteStringAttr("margin-right", FoNamespace, GetIndentInCentimeters(format.AlignmentInfo.Indent));
				}
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateDifferentialParagraphProperties
		protected internal void GenerateDifferentialParagraphProperties(DifferentialFormat format) {
			WriteStartElement("paragraph-properties", StyleNamespace);
			try {
				if (format.MultiOptionsInfo.ApplyAlignmentHorizontal) {
					string alignment;
					if (!HorizontalAlignmentTable.TryGetValue(format.Alignment.Horizontal, out alignment))
						return;
					WriteStringAttr("text-align", FoNamespace, alignment);
				}
				if (format.MultiOptionsInfo.ApplyAlignmentIndent) {
					if (format.Alignment.Horizontal == XlHorizontalAlignment.Left)
						WriteStringAttr("margin-left", FoNamespace, GetIndentInCentimeters(format.Alignment.Indent));
					else
						if (format.Alignment.Horizontal == XlHorizontalAlignment.Right)
							WriteStringAttr("margin-right", FoNamespace, GetIndentInCentimeters(format.Alignment.Indent));
				}
			}
			finally {
				WriteEndElement();
			}
		}
		string GetIndentInCentimeters(byte indent) {
			return string.Concat((indent * 0.353).ToString(CultureInfo.InvariantCulture), "cm");
		}
		#endregion
		#region GenerateTextProperties
		protected internal void GenerateTextProperties(CellFormatBase format, CellFormatBase parentFormat) {
			if (format.FontIndex == parentFormat.FontIndex)
				return;
			WriteStartElement("text-properties", StyleNamespace);
			try {
				GenerateFontInfo(format.FontInfo, parentFormat.FontInfo);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateFontInfo(RunFontInfo info, RunFontInfo parentInfo) {
			string attribute;
			if (info.Bold != parentInfo.Bold) {
				attribute = info.Bold ? "bold" : "normal";
				WriteStringAttr("font-weight-complex", StyleNamespace, attribute);
				WriteStringAttr("font-weight-asian", StyleNamespace, attribute);
				WriteStringAttr("font-weight", FoNamespace, attribute);
			}
			if (info.ColorIndex != parentInfo.ColorIndex)
				WriteStringAttr("color", FoNamespace, ConvertColorToString(info.ColorIndex, "#000000"));
			if (info.Italic != parentInfo.Italic) {
				attribute = info.Italic ? "italic" : "normal";
				WriteStringAttr("font-style-complex", StyleNamespace, attribute);
				WriteStringAttr("font-style-asian", StyleNamespace, attribute);
				WriteStringAttr("font-style", FoNamespace, attribute);
			}
			if (info.Name != parentInfo.Name) {
				WriteStringAttr("font-name-complex", StyleNamespace, info.Name);
				WriteStringAttr("font-name-asian", StyleNamespace, info.Name);
				WriteStringAttr("font-name", StyleNamespace, info.Name);
			}
			if (info.Script != parentInfo.Script) {
				switch (info.Script) {
					default:
					case XlScriptType.Baseline:
						attribute = "0%";
						break;
					case XlScriptType.Subscript:
						attribute = "-33%";
						break;
					case XlScriptType.Superscript:
						attribute = "33%";
						break;
				}
				WriteStringAttr("text-position", StyleNamespace, attribute);
			}
			if (info.Size != parentInfo.Size) {
				attribute = string.Concat(info.Size.ToString(CultureInfo.InvariantCulture), "pt");
				WriteStringAttr("font-size-complex", StyleNamespace, attribute);
				WriteStringAttr("font-size-asian", StyleNamespace, attribute);
				WriteStringAttr("font-size", FoNamespace, attribute);
			}
			if (info.StrikeThrough != parentInfo.StrikeThrough) {
				attribute = info.StrikeThrough ? "solid" : "none";
				WriteStringAttr("text-line-through-style", StyleNamespace, attribute);
			}
			if (info.Underline != parentInfo.Underline) {
				switch (info.Underline) {
					default:
					case XlUnderlineType.None:
						attribute = "none";
						break;
					case XlUnderlineType.Double:
					case XlUnderlineType.DoubleAccounting:
						attribute = "double";
						break;
					case XlUnderlineType.Single:
					case XlUnderlineType.SingleAccounting:
						attribute = "single";
						break;
				}
				WriteStringAttr("text-underline-type", StyleNamespace, attribute);
				WriteStringAttr("text-underline-style", StyleNamespace, "solid");
			}
		}
		#endregion
		#region GenerateDifferentialTextProperties
		protected internal void GenerateDifferentialTextProperties(DifferentialFormat format) {
			WriteStartElement("text-properties", StyleNamespace);
			try {
				GenerateDifferentialFont(format);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDifferentialFont(DifferentialFormat format) {
			string property;
			if (format.MultiOptionsInfo.ApplyFontBold) {
				property = format.Font.Bold ? "bold" : "normal";
				WriteStringAttr("font-weight-complex", StyleNamespace, property);
				WriteStringAttr("font-weight-asian", StyleNamespace, property);
				WriteStringAttr("font-weight", FoNamespace, property);
			}
			if (format.MultiOptionsInfo.ApplyFontColor)
				WriteStringAttr("color", FoNamespace, ConvertColorToString(format.FontInfo.ColorIndex, "#000000"));
			if (format.MultiOptionsInfo.ApplyFontItalic) {
				property = format.Font.Italic ? "italic" : "normal";
				WriteStringAttr("font-style-complex", StyleNamespace, property);
				WriteStringAttr("font-style-asian", StyleNamespace, property);
				WriteStringAttr("font-style", FoNamespace, property);
			}
			if (format.MultiOptionsInfo.ApplyFontName) {
				WriteStringAttr("font-name-complex", StyleNamespace, format.Font.Name);
				WriteStringAttr("font-name-asian", StyleNamespace, format.Font.Name);
				WriteStringAttr("font-name", StyleNamespace, format.Font.Name);
			}
			if (format.MultiOptionsInfo.ApplyFontScript) {
				switch (format.Font.Script) {
					case XlScriptType.Subscript:
						property = "-33%";
						break;
					case XlScriptType.Superscript:
						property = "33%";
						break;
					default:
						property = "0%";
						break;
				}
				WriteStringAttr("text-position", StyleNamespace, property);
			}
			if (format.MultiOptionsInfo.ApplyFontSize) {
				property = string.Concat(format.Font.Size.ToString(CultureInfo.InvariantCulture), "pt");
				WriteStringAttr("font-size-complex", StyleNamespace, property);
				WriteStringAttr("font-size-asian", StyleNamespace, property);
				WriteStringAttr("font-size", FoNamespace, property);
			}
			if (format.MultiOptionsInfo.ApplyFontStrikeThrough) {
				property = format.Font.StrikeThrough ? "solid" : "none";
				WriteStringAttr("text-line-through-style", StyleNamespace, property);
			}
			if (format.MultiOptionsInfo.ApplyFontUnderline) {
				switch (format.Font.Underline) {
					case XlUnderlineType.Double:
					case XlUnderlineType.DoubleAccounting:
						property = "double";
						break;
					case XlUnderlineType.Single:
					case XlUnderlineType.SingleAccounting:
						property = "single";
						break;
					default:
						property = "none";
						break;
				}
				WriteStringAttr("text-underline-type", StyleNamespace, property);
				WriteStringAttr("text-underline-style", StyleNamespace, "solid");
			}
		}
		#endregion
		#region GenerateConditionalFormats
		protected internal void GenerateConditionalFormats() {
			exportStyleSheet.ConditionalFormattings.Generate(this);
		}
		#endregion
	}
}
#endif
