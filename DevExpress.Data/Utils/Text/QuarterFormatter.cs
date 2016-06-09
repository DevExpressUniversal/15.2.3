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
using System.Text;
using System.Diagnostics;
namespace DevExpress.Utils {
	public static class QuarterFormatter {
		const int monthsInQuarter = 3;
		const char arabicQuarterSymbol = 'Q';
		const char romanQuarterSymbol = 'q';
		const char singleQuote = '\'';
		const char doubleQuote = '\"';
		const char backSlash = '\\';
		const char percent = '%';
		static readonly string[] arabicDigits = new string[] { "1", "2", "3", "4" };
		static readonly string[] romanDigits = new string[] { "I", "II", "III", "IV" };
		static readonly char[] specialChars = new char[] { singleQuote, doubleQuote, backSlash, percent };
		static string MakeQuarterString(char quarterSymbol, int digitIndex, int counter, string predefinedFormat) {
			string digit = quarterSymbol == arabicQuarterSymbol ? arabicDigits[digitIndex] : romanDigits[digitIndex];
			if (counter > 1)
				digit = String.Format(predefinedFormat, digit);
			return "'" + digit + "'";
		}
		static bool IsQuarterSymbol(char c) {
			return c == arabicQuarterSymbol || c == romanQuarterSymbol;
		}
		public static string FormatQuarter(int quarter, string format, string predefinedFormat) {
			if (String.IsNullOrEmpty(format))
				return format; 
			Debug.Assert(quarter >= 1 && quarter <= 4, "Incorrect quarter number");
			int digitIndex = quarter - 1;
			string result = String.Empty;
			int quarterSymbolCounter = 0;
			char previous = '\0';
			foreach (char symbol in format) {
				if (symbol != previous && quarterSymbolCounter > 0) {
					result += MakeQuarterString(previous, digitIndex, quarterSymbolCounter, predefinedFormat);
					quarterSymbolCounter = 0;
				}
				if (IsQuarterSymbol(symbol))
					quarterSymbolCounter++;
				else
					result += symbol;
				previous = symbol;
			}
			if (quarterSymbolCounter > 0)
				result += MakeQuarterString(previous, digitIndex, quarterSymbolCounter, predefinedFormat);
			return result;
		}
		public static string FormatDateTime(int quarterNumber, string formatString, string quarterFormat) {
			if (String.IsNullOrEmpty(formatString))
				return String.Empty;
			StringBuilder builder = new StringBuilder();
			int index = 0;
			do {
				int nextIndex = formatString.IndexOfAny(specialChars, index);
				if (nextIndex < 0) {
					builder.Append(FormatQuarter(quarterNumber, formatString.Substring(index), quarterFormat));
					break;
				}
				if (nextIndex > index) {
					string str = FormatQuarter(quarterNumber, formatString.Substring(index, nextIndex - index), quarterFormat);
					builder.Append(str);
					index = nextIndex;
				}
				switch (formatString[index]) {
					case backSlash:
						builder.Append(backSlash);
						if (++index < formatString.Length)
							builder.Append(formatString[index++]);
						break;
					case percent:
						if (++index < formatString.Length) {
							char chr = formatString[index++];
							if (IsQuarterSymbol(chr))
								builder.Append(FormatQuarter(quarterNumber, new String(chr, 1), quarterFormat));
							else {
								builder.Append(percent);
								builder.Append(chr);
							}
						}
						else 
							builder.Append(percent);
						break;
					default:
						int endQuotation = formatString.IndexOf(formatString[index], index + 1);
						if (endQuotation < 0) {
							builder.Append(formatString.Substring(index));
							index = formatString.Length;
						}
						else {
							builder.Append(formatString.Substring(index, endQuotation - index + 1));
							index = endQuotation + 1;
						}
						break;
				}
			} while (index < formatString.Length);
			return builder.ToString();
		}
		public static string FormatDateTime(DateTime dateTime, string formatString, string quarterFormat) {
			return FormatDateTime((dateTime.Month - 1) / monthsInQuarter + 1, formatString, quarterFormat);
		}
	}
}
