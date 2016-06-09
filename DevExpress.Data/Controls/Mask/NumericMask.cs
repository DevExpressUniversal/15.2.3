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
using System.Globalization;
using System.Text.RegularExpressions;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
using System.Collections.Generic;
namespace DevExpress.Data.Mask {
	public class NumericFormatter {
		readonly int[] groupSizes;
		readonly string groupSeparator;
		readonly string decimalSeparator;
		readonly List<string> formatMask = new List<string>();
		readonly int maxDigitsBeforeDecimalSeparator = 0;
		readonly int maxDigitsAfterDecimalSeparator = 0;
		readonly int minDigitsBeforeDecimalSeparator = 0;
		readonly int minDigitsAfterDecimalSeparator = 0;
		public readonly bool Is100Multiplied;
		string GetSeparator(int positionFromDecimalSeparator) {
			if(positionFromDecimalSeparator <= 1 || groupSizes == null || groupSizes.Length == 0)
				return string.Empty;
			int currentOffset = 1;
			for(int i = 0; i < groupSizes.Length - 1; ++i) {
				int nextOffset = currentOffset + groupSizes[i];
				if(nextOffset == positionFromDecimalSeparator)
					return groupSeparator;
				if(nextOffset > positionFromDecimalSeparator)
					return string.Empty;
				currentOffset = nextOffset;
			}
			int finalGroupLength = groupSizes[groupSizes.Length - 1];
			if(finalGroupLength == 0)
				return string.Empty;
			if((positionFromDecimalSeparator - currentOffset) % finalGroupLength == 0)
				return groupSeparator;
			else
				return string.Empty;
		}
		public string Format(string source) {
			return Format(source, -1);
		}
		string Format(string source, int sourcePositionForTerminate) { 
			int decimalPointPosition = source.IndexOf('.');
			if(decimalPointPosition < 0)
				decimalPointPosition = source.Length;
			string result = string.Empty;
			int sourcePosition = 0;
			int placeholderIndex = maxDigitsBeforeDecimalSeparator;
			foreach(string formatElement in formatMask) {
				if(formatElement != null) {
					result += formatElement;
				} else {
					if(sourcePosition == sourcePositionForTerminate)
						return result;
					if(placeholderIndex == 0) {
						++sourcePosition;
						result += decimalSeparator;
					} else {
						if(placeholderIndex == maxDigitsBeforeDecimalSeparator) {
							while(decimalPointPosition - sourcePosition > maxDigitsBeforeDecimalSeparator) {
								result += source[sourcePosition];
								result += GetSeparator(decimalPointPosition - sourcePosition);
								++sourcePosition;
								if(sourcePosition == sourcePositionForTerminate)
									return result;
							}
						}
						if(decimalPointPosition >= placeholderIndex && sourcePosition < source.Length) {
							result += source[sourcePosition];
							result += GetSeparator(decimalPointPosition - sourcePosition);
							++sourcePosition;
							if(sourcePosition == sourcePositionForTerminate)
								return result;
						}
					}
					--placeholderIndex;
				}
			}
			return result;
		}
		public int GetPositionFormatted(string source, int sourcePosition) {
			return Format(source, sourcePosition).Length;
		}
		public int GetPositionSource(string source, int formattedPosition) {
			for(int i = 0; i < source.Length; ++i) {
				int testPos = GetPositionFormatted(source, i);
				if(formattedPosition <= testPos)
					return i;
			}
			return source.Length;
		}
		public int MaxDigitsBeforeDecimalSeparator { get { return maxDigitsBeforeDecimalSeparator; } }
		public int MinDigitsBeforeDecimalSeparator { get { return minDigitsBeforeDecimalSeparator; } }
		public int MaxDigitsAfterDecimalSeparator { get { return maxDigitsAfterDecimalSeparator; } }
		public int MinDigitsAfterDecimalSeparator { get { return minDigitsAfterDecimalSeparator; } }
		enum NumericMaskSubType { Number, Currency, Percent, PercentPercent }
		public NumericFormatter(string formatString, CultureInfo formattingCulture) {
			CharEnumerator chars = formatString.GetEnumerator();
			bool decimalSeparatorProcessed = false;
			bool decimalGroupProcessed = false;
			NumericMaskSubType subType = NumericMaskSubType.Number;
			while(chars.MoveNext()) {
				switch(chars.Current) {
					case '0':
						if(decimalSeparatorProcessed) {
							maxDigitsAfterDecimalSeparator++;
							minDigitsAfterDecimalSeparator++;
						} else {
							maxDigitsBeforeDecimalSeparator++;
							minDigitsBeforeDecimalSeparator++;
						}
						formatMask.Add(null);
						break;
					case '#':
						if(decimalSeparatorProcessed) {
							maxDigitsAfterDecimalSeparator++;
						} else {
							maxDigitsBeforeDecimalSeparator++;
						}
						formatMask.Add(null);
						break;
					case '.':
						if(decimalSeparatorProcessed)
							goto default;
						decimalSeparatorProcessed = true;
						formatMask.Add(null);
						break;
					case ',':
						if(decimalSeparatorProcessed)
							goto default;
						decimalGroupProcessed = true;
						break;
					case '\\':
						if(chars.MoveNext())
							goto default;
						break;
					case '$':
						subType = NumericMaskSubType.Currency;
						formatMask.Add(formattingCulture.NumberFormat.CurrencySymbol);
						break;
					case '%':
						CharEnumerator clonedChars = (CharEnumerator)chars.Clone();
						if(clonedChars.MoveNext() && clonedChars.Current == '%') {
							chars.MoveNext();
							subType = NumericMaskSubType.PercentPercent;
						} else {
							subType = NumericMaskSubType.Percent;
						}
						formatMask.Add(formattingCulture.NumberFormat.PercentSymbol);
						break;
					default:
						char ch = chars.Current;
						if(formatMask.Count > 0 && formatMask[formatMask.Count - 1] != null)
							formatMask[formatMask.Count - 1] = formatMask[formatMask.Count - 1] + ch;
						else
							formatMask.Add(ch.ToString(formattingCulture));
						break;
				}
			}
			switch(subType) {
				default:
				case NumericMaskSubType.Number:
					decimalSeparator = formattingCulture.NumberFormat.NumberDecimalSeparator;
					groupSizes = formattingCulture.NumberFormat.NumberGroupSizes;
					groupSeparator = formattingCulture.NumberFormat.NumberGroupSeparator;
					break;
				case NumericMaskSubType.Currency:
					decimalSeparator = formattingCulture.NumberFormat.CurrencyDecimalSeparator;
					groupSizes = formattingCulture.NumberFormat.CurrencyGroupSizes;
					groupSeparator = formattingCulture.NumberFormat.CurrencyGroupSeparator;
					break;
				case NumericMaskSubType.Percent:
				case NumericMaskSubType.PercentPercent:
					decimalSeparator = formattingCulture.NumberFormat.PercentDecimalSeparator;
					groupSizes = formattingCulture.NumberFormat.PercentGroupSizes;
					groupSeparator = formattingCulture.NumberFormat.PercentGroupSeparator;
					break;
			}
			if(!decimalGroupProcessed)
				groupSizes = null;
			Is100Multiplied = (subType == NumericMaskSubType.Percent);
		}
		static string GetNegativeSymbolPattern(CultureInfo culture) {
			string formattedNegativeSymbol = string.Empty;
			foreach(char ch in culture.NumberFormat.NegativeSign) {
				formattedNegativeSymbol += '\\';
				formattedNegativeSymbol += ch;
			}
			return formattedNegativeSymbol;
		}
		static string CreateCurrencyFormat(int precision, CultureInfo culture) {
			if(precision < 0)
				precision = culture.NumberFormat.CurrencyDecimalDigits;
			int beforePoint = 29 - precision;
			if(beforePoint < 1)
				beforePoint = 1;
			string numberFormat = new string('#', beforePoint - 1) + ",0";
			if(precision > 0)
				numberFormat += ('.' + new string('0', precision));
			string positivePattern, negativePattern;
			switch(culture.NumberFormat.CurrencyPositivePattern) {
				default:
				case 0:
					positivePattern = "${0}";
					break;
				case 1:
					positivePattern = "{0}$";
					break;
				case 2:
					positivePattern = "$ {0}";
					break;
				case 3:
					positivePattern = "{0} $";
					break;
			}
			switch(culture.NumberFormat.CurrencyNegativePattern) {
				default:
				case 0:
					negativePattern = "(${0})";
					break;
				case 1:
					negativePattern = "{1}${0}";
					break;
				case 2:
					negativePattern = "${1}{0}";
					break;
				case 3:
					negativePattern = "${0}{1}";
					break;
				case 4:
					negativePattern = "({0}$)";
					break;
				case 5:
					negativePattern = "{1}{0}$";
					break;
				case 6:
					negativePattern = "{0}{1}$";
					break;
				case 7:
					negativePattern = "{0}${1}";
					break;
				case 8:
					negativePattern = "{1}{0} $";
					break;
				case 9:
					negativePattern = "{1}$ {0}";
					break;
				case 10:
					negativePattern = "{0} ${1}";
					break;
				case 11:
					negativePattern = "$ {0}{1}";
					break;
				case 12:
					negativePattern = "$ {1}{0}";
					break;
				case 13:
					negativePattern = "{0}{1} $";
					break;
				case 14:
					negativePattern = "($ {0})";
					break;
				case 15:
					negativePattern = "({0} $)";
					break;
			}
			return string.Format(CultureInfo.InvariantCulture, positivePattern + ';' + negativePattern, numberFormat, GetNegativeSymbolPattern(culture));
		}
		static string CreateFullNumberFormatFromPositiveFormat(string numberFormat, CultureInfo culture) {
			string negativePattern;
			switch(culture.NumberFormat.NumberNegativePattern) {
				default:
				case 0:
					negativePattern = "({0})";
					break;
				case 1:
					negativePattern = "{1}{0}";
					break;
				case 2:
					negativePattern = "{1} {0}";
					break;
				case 3:
					negativePattern = "{0}{1}";
					break;
				case 4:
					negativePattern = "{0} {1}";
					break;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0};" + negativePattern, numberFormat, GetNegativeSymbolPattern(culture));
		}
		static string CreateNumberFormat(int precision, CultureInfo culture) {
			if(precision < 0)
				precision = culture.NumberFormat.NumberDecimalDigits;
			int beforePoint = 29 - precision;
			if(beforePoint < 1)
				beforePoint = 1;
			string numberFormat = new string('#', beforePoint - 1) + ",0";
			if(precision > 0)
				numberFormat += ('.' + new string('0', precision));
			return CreateFullNumberFormatFromPositiveFormat(numberFormat, culture);
		}
		static string CreateDecimalFormat(int precision, CultureInfo culture) {
			string numberFormat;
			if(precision <= 0)
				numberFormat = new string('#', 28) + "0";
			else
				numberFormat = new string('0', precision);
			return string.Format(CultureInfo.InvariantCulture, "{0};{1}{0}", numberFormat, GetNegativeSymbolPattern(culture));
		}
		static string CreateFixedPointFormat(int precision, CultureInfo culture) {
			if(precision < 0)
				precision = culture.NumberFormat.NumberDecimalDigits;
			int beforePoint = 29 - precision;
			if(beforePoint < 1)
				beforePoint = 1;
			string numberFormat = new string('#', beforePoint - 1) + "0";
			if(precision > 0)
				numberFormat += ('.' + new string('0', precision));
			return string.Format(CultureInfo.InvariantCulture, "{0};{1}{0}", numberFormat, GetNegativeSymbolPattern(culture));
		}
		static string CreatePercentFormat(int precision, CultureInfo culture, string percentSymbol) {
			if(precision < 0)
				precision = culture.NumberFormat.PercentDecimalDigits;
			int beforePoint = 29 - precision;
			if(beforePoint < 1)
				beforePoint = 1;
			string numberFormat = new string('#', beforePoint - 1) + ",0";
			if(precision > 0)
				numberFormat += ('.' + new string('0', precision));
			string positivePattern, negativePattern;
			switch(culture.NumberFormat.PercentPositivePattern) {
				default:
				case 0:
					positivePattern = "{0} {1}";
					break;
				case 1:
					positivePattern = "{0}{1}";
					break;
				case 2:
					positivePattern = "{1}{0}";
					break;
			}
			switch(culture.NumberFormat.PercentNegativePattern) {
				default:
				case 0:
					negativePattern = "{2}{0} {1}";
					break;
				case 1:
					negativePattern = "{2}{0}{1}";
					break;
				case 2:
					negativePattern = "{2}{1}{0}";
					break;
			}
			return string.Format(CultureInfo.InvariantCulture, positivePattern + ';' + negativePattern, numberFormat, percentSymbol, GetNegativeSymbolPattern(culture));
		}
		public static string Expand(string formatString, CultureInfo culture) {
			if(formatString == null || formatString.Length == 0)
				formatString = new string('#', 29) + ",0." + new string('#', 30);
			if(Regex.IsMatch(formatString, "^[cCdDgGfFnNpP][0-9]{0,2}$")) {
				int precision = formatString.Length > 1 ? Convert.ToInt32(formatString.Substring(1)) : -1;
				switch(formatString[0]) {
					case 'c':
					case 'C':
						formatString = CreateCurrencyFormat(precision, culture);
						break;
					case 'd':
					case 'D':
						formatString = CreateDecimalFormat(precision, culture);
						break;
					case 'g':
					case 'G':
					case 'f':
					case 'F':
						formatString = CreateFixedPointFormat(precision, culture);
						break;
					case 'n':
					case 'N':
						formatString = CreateNumberFormat(precision, culture);
						break;
					case 'p':
						formatString = CreatePercentFormat(precision, culture, "%");
						break;
					case 'P':
						formatString = CreatePercentFormat(precision, culture, "%%");
						break;
					default:
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.InternalErrorNonCoveredCase, formatString));
				}
			}
			int masksSeparatorPosition = formatString.Replace("\\\\", "//").Replace("\\;", "/:").IndexOf(';');
			if(masksSeparatorPosition < 0) {
				formatString = NumericFormatter.CreateFullNumberFormatFromPositiveFormat(formatString, culture);
			} else if(masksSeparatorPosition == formatString.Length - 1) {
				formatString = formatString.Substring(0, formatString.Length - 1);
			}
			return formatString;
		}
	}
	public class NumericMaskLogic {
		int maxDigitsBeforeDecimalSeparator;
		int maxDigitsAfterDecimalSeparator;
		int minDigitsBeforeDecimalSeparator;
		int minDigitsAfterDecimalSeparator;
		CultureInfo culture;
		static string RefineInput(string dirtyInput, CultureInfo refineCulture) {
			if(dirtyInput.Length == 1 && (dirtyInput[0] == '.' || dirtyInput[0] == ','))
				return ".";
			if(!string.IsNullOrEmpty(refineCulture.NumberFormat.CurrencySymbol))
				dirtyInput = dirtyInput.Replace(refineCulture.NumberFormat.CurrencySymbol, string.Empty);
			string refined = string.Empty;
			bool decimalSeparator = false;
			foreach(char inputChar in dirtyInput) {
				if(inputChar >= '0' && inputChar <= '9')
					refined += inputChar;
				else if(inputChar == refineCulture.NumberFormat.NumberDecimalSeparator[0] || inputChar == refineCulture.NumberFormat.CurrencyDecimalSeparator[0] || (inputChar == '.' && '.' != refineCulture.NumberFormat.NumberGroupSeparator[0] && '.' != refineCulture.NumberFormat.CurrencyGroupSeparator[0])) {
					if(!decimalSeparator) {
						refined += '.';
						decimalSeparator = true;
					}
				}
			}
			return refined;
		}
		string PatchTailIfEmpty(string tail) {
			if(tail.Length <= 0)
				return tail;
			if(tail[0] != '.')
				return tail;
			for(int i = 1; i < tail.Length; ++i) {
				if(tail[i] != '0') {
					return tail;
				}
			}
			return string.Empty;
		}
		public MaskLogicResult GetEditResult(string head, string replaced, string tail, string inserted) {
			string refinedInput = RefineInput(inserted, culture);
			if(refinedInput.Length == 0 && inserted.Length != 0)
				return null;
			if(refinedInput == "." && replaced.Length == 0) {
				string fullBody = head + tail;
				int pos = fullBody.IndexOf('.');
				if(pos >= 0)
					return CreateResult(fullBody, pos + 1);
			}
			tail = PatchTailIfEmpty(tail);
			if(refinedInput.IndexOf('.') >= 0 && (maxDigitsAfterDecimalSeparator == 0 || head.IndexOf('.') >= 0 || tail.IndexOf('.') >= 0))
				return null;
			if(replaced.IndexOf('.') >= 0 && head.Length > 0 && tail.Length > 0 && refinedInput.IndexOf('.') < 0)
				return null;
			MaskLogicResult result = CreateResult(head + refinedInput + tail, head.Length + refinedInput.Length);
			return result;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestGetEditResultBug21912() {
			Init(10, 1, 2, 2, CultureInfo.InvariantCulture);
			Assert.AreEqual("123.00", GetEditResult("0", "", ".00", "123.00").EditText);
			Assert.IsNull(GetEditResult("0", "", ".10", "123.00"));
			Assert.AreEqual("123.10", GetEditResult("0", "", ".10", "123").EditText);
		}
#endif
		#endregion
		MaskLogicResult CreateResult(string resultCandidate, int cursorBase) {
			int decimalPosition = resultCandidate.IndexOf('.');
			if(decimalPosition < 0) {
				decimalPosition = resultCandidate.Length;
				if(maxDigitsAfterDecimalSeparator > 0)
					resultCandidate += '.';
			}
			while(decimalPosition > minDigitsBeforeDecimalSeparator && resultCandidate[0] == '0' && cursorBase > 0) {
				resultCandidate = resultCandidate.Substring(1);
				cursorBase--;
				decimalPosition--;
			}
			if(decimalPosition > maxDigitsBeforeDecimalSeparator)
				return null;
			while(decimalPosition < minDigitsBeforeDecimalSeparator) {
				decimalPosition++;
				cursorBase++;
				resultCandidate = '0' + resultCandidate;
			}
			if(resultCandidate.Length - decimalPosition > maxDigitsAfterDecimalSeparator) {
				resultCandidate = resultCandidate.Substring(0, decimalPosition + maxDigitsAfterDecimalSeparator + 1);
				if(cursorBase > resultCandidate.Length)
					cursorBase = resultCandidate.Length;
			}
			while(resultCandidate.Length > cursorBase && minDigitsAfterDecimalSeparator < resultCandidate.Length - decimalPosition - 1 && resultCandidate.EndsWith("0"))
				resultCandidate = resultCandidate.Substring(0, resultCandidate.Length - 1);
			while(minDigitsAfterDecimalSeparator > 0 && resultCandidate.Length - decimalPosition <= minDigitsAfterDecimalSeparator) {
				resultCandidate += '0';
			}
			return new MaskLogicResult(resultCandidate, cursorBase);
		}
		protected virtual void Init(int maxDigitsBeforeDecimalSeparator, int minDigitsBeforeDecimalSeparator, int minDigitsAfterDecimalSeparator, int maxDigitsAfterDecimalSeparator, CultureInfo culture) {
			this.maxDigitsBeforeDecimalSeparator = maxDigitsBeforeDecimalSeparator;
			this.maxDigitsAfterDecimalSeparator = maxDigitsAfterDecimalSeparator;
			this.minDigitsBeforeDecimalSeparator = minDigitsBeforeDecimalSeparator;
			this.minDigitsAfterDecimalSeparator = minDigitsAfterDecimalSeparator;
			this.culture = culture;
		}
		public NumericMaskLogic(int maxDigitsBeforeDecimalSeparator, int minDigitsBeforeDecimalSeparator, int minDigitsAfterDecimalSeparator, int maxDigitsAfterDecimalSeparator, CultureInfo culture) {
			Init(maxDigitsBeforeDecimalSeparator, minDigitsBeforeDecimalSeparator, minDigitsAfterDecimalSeparator, maxDigitsAfterDecimalSeparator, culture);
		}
		static string Increment(string number) {
			string result = string.Empty;
			for(int pos = number.Length - 1; ; --pos) {
				if(pos < 0) {
					result = '1' + result;
					return result;
				}
				char currentChar = number[pos];
				if(currentChar == '9') {
					result = '0' + result;
				} else if(currentChar >= '0' && currentChar <= '9') {
					char nextChar = (char)(currentChar + 1);
					result = number.Substring(0, pos) + nextChar + result;
					return result;
				} else {
					result = currentChar + result;
				}
			}
		}
		static string Decrement(string number) {
			string result = string.Empty;
			for(int pos = number.Length - 1; ; --pos) {
				if(pos < 0) {
					return null;
				}
				char currentChar = number[pos];
				if(currentChar == '0') {
					result = '9' + result;
				} else if(currentChar >= '0' && currentChar <= '9') {
					char nextChar = (char)(currentChar - 1);
					result = number.Substring(0, pos) + nextChar + result;
					return result;
				} else {
					result = currentChar + result;
				}
			}
		}
		static string SubtractWithCarry(string number) {
			bool carry = false;
			string result = string.Empty;
			for(int pos = number.Length - 1; ; --pos) {
				if(pos < 0) {
					if(carry)
						return result;
					else
						return null;	
				}
				char currentChar = number[pos];
				if(currentChar == '0' && !carry) {
					result = '0' + result;
				} else if(currentChar >= '0' && currentChar <= '9' && !carry) {
					carry = true;
					char nextChar = (char)('9' - currentChar + '1');
					result = nextChar + result;
				} else if(currentChar >= '0' && currentChar <= '9' && carry) {
					char nextChar = (char)('9' - currentChar + '0');
					result = nextChar + result;
				} else {
					result = currentChar + result;
				}
			}
		}
		MaskLogicResult GetClimbModuloResult(string head, string tail) {
			string newHead = Increment(head);
			MaskLogicResult result = CreateResult(newHead + tail, newHead.Length);
			return result;
		}
		MaskLogicResult GetDiveModuloResult(string head, string tail, bool canChSign, out bool chSign) {
			chSign = false;
			string newHead = Decrement(head);
			if(newHead != null) {
				MaskLogicResult result = CreateResult(newHead + tail, newHead.Length);
				return result;
			}
			if(!canChSign) {
				MaskLogicResult result = CreateResult(string.Empty, 0);
				return result;
			}
			chSign = true;
			string newTail = SubtractWithCarry(tail);
			if(newTail != null) {
				MaskLogicResult result = CreateResult(head + newTail, head.Length);
				return result;
			} else {
				int lastZeroIndex = head.LastIndexOf('0');
				if(lastZeroIndex < 0) {
					newHead = '1' + head;
				} else {
					newHead = head.Substring(0, lastZeroIndex) + '1' + head.Substring(lastZeroIndex + 1);
				}
				MaskLogicResult result = CreateResult(newHead + tail, newHead.Length);
				return result;
			}
		}
		public MaskLogicResult GetSpinResult(string head, string tail, bool isModuloDecrement, bool canChSign, out bool chSign) {
			chSign = false;
			if(isModuloDecrement) {
				MaskLogicResult result = GetDiveModuloResult(head, tail, canChSign, out chSign);
				return result;
			} else {
				MaskLogicResult result = GetClimbModuloResult(head, tail);
				return result;
			}
		}
		static char[] allDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		static string Mul10(string input) {
			int dotPos = input.IndexOf('.');
			if(dotPos < 0) {
				dotPos = input.Length;
				input += '.';
			}
			int afterDotDigitPos = input.IndexOfAny(allDigits, dotPos);
			if(afterDotDigitPos < 0)
				return input.Substring(0, dotPos) + '0' + input.Substring(dotPos);
			else
				return input.Substring(0, dotPos) + input.Substring(dotPos + 1, afterDotDigitPos - dotPos) + '.' + input.Substring(afterDotDigitPos + 1);
		}
		public static string Mul100(string input) {
			return Mul10(Mul10(input));
		}
		public static string Div100(string input) {
			if(input.IndexOf('.') < 0)
				input += '.';
			string reversedInput = string.Empty;
			foreach(char ch in input) {
				reversedInput = ch + reversedInput;
			}
			string reversedResult = Mul100(reversedInput);
			string result = string.Empty;
			foreach(char ch in reversedResult) {
				result = ch + result;
			}
			return result;
		}
		#region Mask_Tests
#if DEBUGTEST
		protected NumericMaskLogic() { }
#endif
		#endregion
	}
		#region Mask_Tests
#if DEBUGTEST
		[TestFixture]
		public class NumericMaskLogicTests: NumericMaskLogic {
			public NumericMaskLogicTests() : base() { }
		}
#endif
		#endregion
	public class NumericMaskManagerState: MaskManagerPlainTextState {
		readonly bool fIsNull;
		readonly bool fIsNegative;
		readonly bool fIsSelectAll;
		public bool IsNegative { get { return fIsNegative; } }
		public bool IsNull { get { return fIsNull; } }
		public bool IsSelectAll { get { return fIsSelectAll; } }
		public NumericMaskManagerState(string editText, int cursorPosition, int selectionAnchor, bool isNegative)
			: base(editText, cursorPosition, selectionAnchor) {
			this.fIsNegative = isNegative;
			this.fIsNull = false;
		}
		public NumericMaskManagerState(string editText, bool isNegative) : this(editText, 0, editText.Length, isNegative) {
			this.fIsSelectAll = true;
		}
		public override bool IsSame(MaskManagerState comparedState) {
			return base.IsSame(comparedState) && this.IsNegative == ((NumericMaskManagerState)comparedState).IsNegative && this.IsNull == ((NumericMaskManagerState)comparedState).IsNull && this.IsSelectAll == ((NumericMaskManagerState)comparedState).IsSelectAll;
		}
		NumericMaskManagerState()
			: base(string.Empty, 0, 0) {
			this.fIsNegative = false;
			this.fIsNull = true;
		}
		public static readonly NumericMaskManagerState NullInstance = new NumericMaskManagerState();
	}
	public class NumericMaskManager: MaskManagerStated<NumericMaskManagerState> {
		protected readonly bool AllowNull;
		NumericMaskLogic logic;
		readonly string negativeSignString;
		NumericFormatter[] formatters = new NumericFormatter[2];
		TypeCode? editValueTypeCode;
		NumericFormatter GetFormatter(NumericMaskManagerState state) {
			return formatters[state.IsNegative ? 1 : 0];
		}
		bool ForceEditValueTypeCode(TypeCode forcedCode) {
			switch(forcedCode) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					editValueTypeCode = forcedCode;
					return true;
				default:
					editValueTypeCode = TypeCode.String;
					return true;
			}
		}
		bool IsSignedMask { get { return formatters[1] != null; } }
		[Obsolete("Use NumericMaskManager(string formatString, CultureInfo managerCultureInfo, bool allowNull) instead")]
		public NumericMaskManager(string formatString, CultureInfo managerCultureInfo) : this(formatString, managerCultureInfo, false) { }
		public NumericMaskManager(string formatString, CultureInfo managerCultureInfo, bool allowNull)
			: base(NumericMaskManagerState.NullInstance) {
			this.AllowNull = allowNull;
			formatString = NumericFormatter.Expand(formatString, managerCultureInfo);
			negativeSignString = managerCultureInfo.NumberFormat.NegativeSign;
			int masksSeparatorPosition = formatString.Replace("\\\\", "//").Replace("\\;", "/:").IndexOf(';');
			if(masksSeparatorPosition < 0) {
				formatters[0] = new NumericFormatter(formatString, managerCultureInfo);
				formatters[1] = null;
			} else {
				formatters[0] = new NumericFormatter(formatString.Substring(0, masksSeparatorPosition), managerCultureInfo);
				formatters[1] = new NumericFormatter(formatString.Substring(masksSeparatorPosition + 1), managerCultureInfo);
				if(formatters[0].MaxDigitsBeforeDecimalSeparator != formatters[1].MaxDigitsBeforeDecimalSeparator)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectNumericMaskSignedMaskNotMatchMaxDigitsBeforeDecimalSeparator);
				if(formatters[0].MaxDigitsAfterDecimalSeparator != formatters[1].MaxDigitsAfterDecimalSeparator)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectNumericMaskSignedMaskNotMatchMaxDigitsAfterDecimalSeparator);
				if(formatters[0].MinDigitsBeforeDecimalSeparator != formatters[1].MinDigitsBeforeDecimalSeparator)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectNumericMaskSignedMaskNotMatchMinDigitsBeforeDecimalSeparator);
				if(formatters[0].MinDigitsAfterDecimalSeparator != formatters[1].MinDigitsAfterDecimalSeparator)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectNumericMaskSignedMaskNotMatchMinDigitsAfterDecimalSeparator);
				if(formatters[0].Is100Multiplied != formatters[1].Is100Multiplied)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectNumericMaskSignedMaskNotMatchIs100Multiplied);
			}
			logic = new NumericMaskLogic(formatters[0].MaxDigitsBeforeDecimalSeparator, formatters[0].MinDigitsBeforeDecimalSeparator, formatters[0].MinDigitsAfterDecimalSeparator, formatters[0].MaxDigitsAfterDecimalSeparator, managerCultureInfo);
			if(formatters[0].MaxDigitsAfterDecimalSeparator > 0 || formatters[0].Is100Multiplied)
				editValueTypeCode = TypeCode.Decimal;
		}
		public override bool IsFinal {
			get {
				if(CurrentState.IsNull)
					return false;
				if(CurrentState.EditText.Length != CurrentState.CursorPosition)
					return false;
				int pointPosition = CurrentState.EditText.IndexOf('.');
				if(pointPosition < 0) {
					return formatters[0].MaxDigitsAfterDecimalSeparator == 0 && formatters[0].MaxDigitsBeforeDecimalSeparator == CurrentState.EditText.Length;
				} else {
					return formatters[0].MaxDigitsAfterDecimalSeparator == CurrentState.EditText.Length - pointPosition - 1;
				}
			}
		}
		protected override int GetCursorPosition(NumericMaskManagerState numericState) {
			if(numericState.IsNull)
				return 0;
			if(numericState.IsSelectAll)
				return 0;
			return GetFormatter(numericState).GetPositionFormatted(numericState.EditText, numericState.CursorPosition);
		}
		protected override int GetSelectionAnchor(NumericMaskManagerState numericState) {
			if(numericState.IsNull)
				return 0;
			if(numericState.IsSelectAll)
				return GetDisplayText(numericState).Length;
			return GetFormatter(numericState).GetPositionFormatted(numericState.EditText, numericState.SelectionAnchor);
		}
		protected override string GetDisplayText(NumericMaskManagerState numericState) {
			if(numericState.IsNull)
				return string.Empty;
			return GetFormatter(numericState).Format(numericState.EditText);
		}
		protected override string GetEditText(NumericMaskManagerState numericState) {
			if(numericState.IsNegative)
				return '-' + numericState.EditText;
			else
				return numericState.EditText;
		}
		public override bool Delete() {
			if(CurrentState.IsNull)
				return false;
			if(CurrentState.IsSelectAll || IsSelection) {
				if(AllowNull) {
					if(CurrentState.IsSelectAll ||
						(Math.Min(CurrentState.SelectionAnchor, CurrentState.CursorPosition) == 0 && Math.Max(CurrentState.SelectionAnchor, CurrentState.CursorPosition) == CurrentState.EditText.Length))
						return Apply(NumericMaskManagerState.NullInstance, StateChangeType.Delete);
				}
				return Insert(string.Empty);
			}
			MaskLogicResult result;
			if(CurrentState.CursorPosition >= CurrentState.EditText.Length) {
				result = logic.GetEditResult(CurrentState.EditText, string.Empty, string.Empty, string.Empty);
			} else {
				result = logic.GetEditResult(CurrentState.EditText.Substring(0, CurrentState.CursorPosition), CurrentState.EditText.Substring(CurrentState.CursorPosition, 1), CurrentState.EditText.Substring(CurrentState.CursorPosition + 1), string.Empty);
			}
			if(result == null)
				return CursorRight(false);
			bool applyResult = Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, CurrentState.IsNegative), StateChangeType.Delete);
			if(applyResult)
				return true;
			if(CurrentState.IsNegative) {
				return Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, false), StateChangeType.Delete);
			}
			if(AllowNull && !IsSomethingExceptDotsAndZeros(result.EditText))
				return Apply(NumericMaskManagerState.NullInstance, StateChangeType.Delete);
			return false;
		}
		public override bool Backspace() {
			if(CurrentState.IsNull)
				return false;
			if(CurrentState.IsSelectAll || IsSelection) {
				if(AllowNull) {
					if(CurrentState.IsSelectAll ||
						(Math.Min(CurrentState.SelectionAnchor, CurrentState.CursorPosition) == 0 && Math.Max(CurrentState.SelectionAnchor, CurrentState.CursorPosition) == CurrentState.EditText.Length))
						return Apply(NumericMaskManagerState.NullInstance, StateChangeType.Delete);
				}
				return Insert(string.Empty);
			}
			MaskLogicResult result;
			if(CurrentState.CursorPosition <= 0) {
				result = logic.GetEditResult(string.Empty, string.Empty, CurrentState.EditText, string.Empty);
			} else {
				result = logic.GetEditResult(CurrentState.EditText.Substring(0, CurrentState.CursorPosition - 1), CurrentState.EditText.Substring(CurrentState.CursorPosition - 1, 1), CurrentState.EditText.Substring(CurrentState.CursorPosition), string.Empty);
			}
			if(result == null) {
				return CursorLeft(false);
			}
			bool applyResult = Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, CurrentState.IsNegative), StateChangeType.Delete);
			if(applyResult)
				return true;
			if(CurrentState.IsNegative) {
				return Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, false), StateChangeType.Delete);
			}
			if(AllowNull && !IsSomethingExceptDotsAndZeros(result.EditText))
				return Apply(NumericMaskManagerState.NullInstance, StateChangeType.Delete);
			return false;
		}
		static bool IsSomethingExceptDotsAndZeros(string input) {
			for(int i = 0; i < input.Length; ++i) {
				char ch = input[i];
				if(ch != '0' && ch != '.')
					return true;
			}
			return false;
		}
		bool StateCursorPositionTo(int newPosition, bool forceSelection, bool isNeededKeyCheck) {
			if(CurrentState.IsNull)
				return false;
			if(newPosition < 0) {
				newPosition = 0;
			} else if(newPosition > CurrentState.EditText.Length) {
				newPosition = CurrentState.EditText.Length;
			}
			return Apply(new NumericMaskManagerState(CurrentState.EditText, newPosition, forceSelection ? CurrentState.SelectionAnchor : newPosition, CurrentState.IsNegative), StateChangeType.Terminator, isNeededKeyCheck);
		}
		bool StateCursorPositionTo(int newPosition, bool forceSelection) {
			return StateCursorPositionTo(newPosition, forceSelection, false);
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			return StateCursorPositionTo(GetFormatter(CurrentState).GetPositionSource(CurrentState.EditText, newPosition), forceSelection);
		}
		public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck) {
			return StateCursorPositionTo(CurrentState.CursorPosition - 1, forceSelection, isNeededKeyCheck);
		}
		public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck) {
			return StateCursorPositionTo(CurrentState.CursorPosition + 1, forceSelection, isNeededKeyCheck);
		}
		bool SpinKeys(bool isUp) {
			int cursor = CurrentState.CursorPosition;
			if(CurrentState.SelectionAnchor != CurrentState.CursorPosition) {
				int selStart = CurrentState.SelectionAnchor < CurrentState.CursorPosition ? CurrentState.SelectionAnchor : CurrentState.CursorPosition;
				int selEnd = CurrentState.SelectionAnchor < CurrentState.CursorPosition ? CurrentState.CursorPosition : CurrentState.SelectionAnchor;
				if((selStart == 0 && selEnd == CurrentState.EditText.Length) || (CurrentState.EditText.Substring(selStart, selEnd - selStart).IndexOf('.') >= 0)) {
					cursor = CurrentState.EditText.IndexOf('.');
					if(cursor < 0)
						cursor = CurrentState.EditText.Length;
				} else {
					cursor = selEnd;
				}
			}
			bool chSignRequired;
			bool isModuloDecrement = isUp ? CurrentState.IsNegative : !CurrentState.IsNegative;
			MaskLogicResult result = logic.GetSpinResult(CurrentState.EditText.Substring(0, cursor), CurrentState.EditText.Substring(cursor), isModuloDecrement, IsSignedMask, out chSignRequired);
			if(result == null)
				return false;
			bool sign = chSignRequired ? !CurrentState.IsNegative : CurrentState.IsNegative;
			return Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, sign), StateChangeType.Insert);
		}
		public override bool SpinUp() {
			return SpinKeys(true);
		}
		public override bool SpinDown() {
			return SpinKeys(false);
		}
		public override bool CursorHome(bool forceSelection) {
			return StateCursorPositionTo(0, forceSelection);
		}
		public override bool CursorEnd(bool forceSelection) {
			return StateCursorPositionTo(CurrentState.EditText.Length, forceSelection);
		}
		public override bool Insert(string insertion) {
			if(IsSignedMask && (insertion == "-" || insertion == negativeSignString)) {
				if(CurrentState.IsNull) {
					MaskLogicResult res = logic.GetEditResult(string.Empty, string.Empty, string.Empty, string.Empty);
					if(res == null)
						return false;
					return Apply(new NumericMaskManagerState(res.EditText, 0, res.EditText.Length, true), StateChangeType.Insert);
				} else {
					bool newNegative = CurrentState.IsSelectAll? true : !CurrentState.IsNegative;
					return Apply(new NumericMaskManagerState(CurrentState.EditText, CurrentState.CursorPosition, CurrentState.SelectionAnchor, newNegative), StateChangeType.Insert);
				}
			}
			int selStart = CurrentState.CursorPosition < CurrentState.SelectionAnchor ? CurrentState.CursorPosition : CurrentState.SelectionAnchor;
			int selEnd = CurrentState.CursorPosition < CurrentState.SelectionAnchor ? CurrentState.SelectionAnchor : CurrentState.CursorPosition;
			MaskLogicResult result = logic.GetEditResult(CurrentState.EditText.Substring(0, selStart), CurrentState.EditText.Substring(selStart, selEnd - selStart), CurrentState.EditText.Substring(selEnd), insertion);
			if(result == null)
				return false;
			bool isNegative = CurrentState.IsNegative && !CurrentState.IsSelectAll;
			if(IsSignedMask) {
				if(insertion.IndexOf("-") >= 0 || insertion.IndexOf(negativeSignString) >= 0)
					isNegative = !isNegative;
			}
			return Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, isNegative), StateChangeType.Insert);
		}
		bool IsValidInvariantCultureDecimal(string testedString) {
			int dotPosition = testedString.IndexOf('.');
			for(int i = 0; i < testedString.Length; ++i) {
				if(i == dotPosition)
					continue;
				char testedChar = testedString[i];
				if(testedChar < '0' || testedChar > '9')
					return false;
			}
			return true;
		}
		public override void SetInitialEditText(string initialEditText) {
			if(initialEditText == null || (AllowNull && initialEditText.Length == 0)) {
				SetInitialState(NumericMaskManagerState.NullInstance);
				return;
			}
			string refined = initialEditText.Trim();
			bool isNegative = false;
			if(refined.StartsWith("-")) {
				refined = refined.Substring(1);
				if(IsSignedMask) {
					isNegative = true;
				}
			}
			MaskLogicResult result;
			if(IsValidInvariantCultureDecimal(refined)) {
				result = logic.GetEditResult(refined, string.Empty, string.Empty, string.Empty);
				if(result != null)
					result = logic.GetEditResult(string.Empty, string.Empty, result.EditText, string.Empty);
			} else {
				result = logic.GetEditResult(string.Empty, string.Empty, string.Empty, initialEditText);
			}
			if(result == null) {
				result = logic.GetEditResult(string.Empty, string.Empty, string.Empty, string.Empty);
			}
			int position = result.EditText.IndexOf('.');
			if(position < 0)
				position = result.EditText.Length;
			SetInitialState(new NumericMaskManagerState(result.EditText, position, position, isNegative));
		}
		protected override object GetEditValue(NumericMaskManagerState state) {
			if(state.IsNull)
				return null;
			string work = GetEditText(state);
			if(formatters[0].Is100Multiplied) {
				work = NumericMaskLogic.Div100(work);
			}
			if(work.IndexOf('.') >= 0) {
				while(work.EndsWith("0")) {
					work = work.Substring(0, work.Length - 1);
				}
			}
			if(work.EndsWith("."))
				work = work.Substring(0, work.Length - 1);
			if(work.Length == 0 || work == "-")
				work += '0';
			if(editValueTypeCode.HasValue) {
				try {
					return Convert.ChangeType(work, editValueTypeCode.Value, CultureInfo.InvariantCulture);
				} catch {
				}
			} else {
				try {
					return Convert.ChangeType(work, TypeCode.Int32, CultureInfo.InvariantCulture);
				} catch {
				}
				try {
					return Convert.ChangeType(work, TypeCode.Decimal, CultureInfo.InvariantCulture);
				} catch {
				}
			}
			return null;
		}
		protected override bool IsValid(NumericMaskManagerState newState) {
			if(newState == null)
				return false;
			if(!newState.IsNull && GetEditValue(newState) == null)
				return false;
			return true;
		}
		public override void SetInitialEditValue(object initialEditValue) {
			if(initialEditValue == null) {
				SetInitialEditText(null);
				return;
			}
			ForceEditValueTypeCode(Type.GetTypeCode(initialEditValue.GetType()));
			int digitsAfterDecimal = formatters[0].MaxDigitsAfterDecimalSeparator;
			if(formatters[0].Is100Multiplied) {
				digitsAfterDecimal += 2;
			}
			string formatString = "{0:f" + digitsAfterDecimal.ToString(CultureInfo.InvariantCulture) + "}";
			string work = string.Format(CultureInfo.InvariantCulture, formatString, initialEditValue);
			if(formatters[0].Is100Multiplied) {
				work = NumericMaskLogic.Mul100(work);
			}
			SetInitialEditText(work);
		}
		public override void SelectAll() {
			if(CurrentState.IsNull)
				return;
			Apply(new NumericMaskManagerState(CurrentState.EditText, CurrentState.IsNegative), StateChangeType.Terminator);
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class NumericMaskManagerTests {
		[Test]
		public void NullFunctionalTest1() {
			NumericMaskManager m = new NumericMaskManager("#,##0.00", CultureInfo.InvariantCulture, true);
			m.SetInitialEditValue(null);
			Assert.AreEqual("", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("12"));
			Assert.IsTrue(m.Insert("3"));
			Assert.AreEqual("123.00", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("."));
			Assert.IsTrue(m.Insert("5"));
			Assert.AreEqual("123.50", m.GetCurrentEditText());
			Assert.IsTrue(m.CursorLeft(false));
			Assert.IsTrue(m.CursorLeft(false));
			Assert.AreEqual("123.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Backspace());
			Assert.AreEqual("12.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Backspace());
			Assert.AreEqual("1.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-1.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Backspace());
			Assert.AreEqual("-0.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Backspace());
			Assert.AreEqual("0.50", m.GetCurrentEditText());
			Assert.IsFalse(m.Backspace());
			Assert.IsTrue(m.Delete());
			Assert.AreEqual("0.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-0.50", m.GetCurrentEditText());
			Assert.IsTrue(m.Delete());
			Assert.AreEqual("-0.00", m.GetCurrentEditText());
			Assert.IsTrue(m.Delete());
			Assert.AreEqual("0.00", m.GetCurrentEditText());
			Assert.IsTrue(m.Delete());
			Assert.AreEqual("", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-0.00", m.GetCurrentEditText());
		}
		[Test]
		public void Bug_B139637_IncorrectNegativeFromNonnullableSelectAllInput() {
			NumericMaskManager m = new NumericMaskManager("#,##0.00#####", CultureInfo.InvariantCulture, false);
			m.SetInitialEditValue(1.1);
			Assert.AreEqual("1.10", m.GetCurrentEditText());
			m.SelectAll();
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-1.10", m.GetCurrentEditText());
			Assert.IsTrue(m.Insert("5"));
			Assert.AreEqual("-5.00", m.GetCurrentEditText());
		}
		[Test]
		public void ChangedFromSelectAllNegative() {
			string log = string.Empty;
			NumericMaskManager m = new NumericMaskManager("###0.00##", CultureInfo.InvariantCulture, false);
			m.SetInitialEditValue(1.1);
			Assert.AreEqual("1.10", m.GetCurrentEditText());
			m.EditTextChanged += (s, e) => {
				log += m.DisplayText + ";";
			};
			m.SelectAll();
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-1.10", m.GetCurrentEditText());
			Assert.AreEqual(1, m.DisplaySelectionStart);
			Assert.AreEqual(5, m.DisplaySelectionEnd);
			Assert.IsTrue(m.Insert("5"));
			Assert.AreEqual("-5.00", m.GetCurrentEditText());
			Assert.AreEqual(2, m.DisplayCursorPosition);
			Assert.AreEqual(2, m.DisplaySelectionAnchor);
			m.SelectAll();
			Assert.AreEqual("-5.00", m.GetCurrentEditText());
			Assert.AreEqual(0, m.DisplaySelectionStart);
			Assert.AreEqual(5, m.DisplaySelectionEnd);
			Assert.IsFalse(m.Insert("q"));
			Assert.AreEqual("-5.00", m.GetCurrentEditText());
			Assert.AreEqual(0, m.DisplaySelectionStart);
			Assert.AreEqual(5, m.DisplaySelectionEnd);
			Assert.IsTrue(m.Insert("9"));
			Assert.AreEqual("9.00", m.GetCurrentEditText());
			Assert.AreEqual(1, m.DisplayCursorPosition);
			Assert.AreEqual(1, m.DisplaySelectionAnchor);
			Assert.AreEqual("-1.10;-5.00;9.00;", log);
		}
		[Test]
		public void CursorMoveFromSelectAll() {
			NumericMaskManager m = new NumericMaskManager("###0.00##", CultureInfo.InvariantCulture, false);
			m.SetInitialEditValue(1.1m);
			m.SelectAll();
			Assert.AreEqual("1.10", m.DisplayText);
			Assert.AreEqual(0, m.DisplayCursorPosition);
			Assert.AreEqual(4, m.DisplaySelectionAnchor);
			Assert.IsTrue(m.CursorLeft(false));
			Assert.AreEqual(0, m.DisplayCursorPosition);
			Assert.AreEqual(0, m.DisplaySelectionAnchor);
			m.SelectAll();
			Assert.AreEqual("1.10", m.DisplayText);
			Assert.AreEqual(0, m.DisplayCursorPosition);
			Assert.AreEqual(4, m.DisplaySelectionAnchor);
			Assert.IsTrue(m.CursorRight(false));
			Assert.AreEqual(1, m.DisplayCursorPosition);
			Assert.AreEqual(1, m.DisplaySelectionAnchor);
		}
		[Test]
		public void Q530980_MinusFromSelectAllShouldAlwaysProduceNegativeNonSelectAll() {
			NumericMaskManager m = new NumericMaskManager("###0", CultureInfo.InvariantCulture, false);
			m.SetInitialEditValue("12");
			m.SelectAll();
			Assert.AreEqual("12", m.DisplayText);
			Assert.AreEqual(0, m.DisplaySelectionStart);
			Assert.AreEqual(2, m.DisplaySelectionEnd);
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-12", m.DisplayText);
			Assert.AreEqual(1, m.DisplaySelectionStart);
			Assert.AreEqual(3, m.DisplaySelectionEnd);
			m.SelectAll();
			Assert.AreEqual("-12", m.DisplayText);
			Assert.AreEqual(0, m.DisplaySelectionStart);
			Assert.AreEqual(3, m.DisplaySelectionEnd);
			Assert.IsTrue(m.Insert("-"));
			Assert.AreEqual("-12", m.DisplayText);
			Assert.AreEqual(1, m.DisplaySelectionStart);
			Assert.AreEqual(3, m.DisplaySelectionEnd);
		}
	}
#endif
	#endregion
}
