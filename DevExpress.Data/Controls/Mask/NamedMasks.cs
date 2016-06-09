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
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
namespace DevExpress.Data.Mask {
	public abstract class NamedMasks {
		public static CultureInfo DefaultCulture = CultureInfo.CurrentCulture;
		public static string Escape(string input) {
			string result = string.Empty;
			foreach(char inputChar in input) {
				result += '[';
				if(inputChar == '^' || inputChar == '\\')
					result += '\\';
				result += inputChar;
				result += ']';
			}
			return result;
		}
		public static string Escape(string[] inputStrings, bool ignoreZeros) {
			bool optionalGroup = false;
			string result = string.Empty;
			foreach(string input in inputStrings) {
				if(input.Length == 0) {
					if(!ignoreZeros)
						optionalGroup = true;
				} else {
					if(result.Length != 0)
						result += '|';
					result += Escape(input);
				}
			}
			if(result.Length > 0) {
				result = '(' + result + ')';
				if(optionalGroup)
					result += '?';
			}
			return result;
		}
		public static string GetDateSeparator(CultureInfo culture) {
			return Escape(DateTimeFormatHelper.GetDateSeparator(culture));
		}
		public static string GetTimeSeparator(CultureInfo culture) {
			return Escape(DateTimeFormatHelper.GetTimeSeparator(culture));
		}
		public static string GetAbbreviatedDayNames(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.AbbreviatedDayNames, false);
		}
		public static string GetAbbreviatedMonthNames(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.AbbreviatedMonthNames, true);
		}
		public static string GetAMDesignator(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.AMDesignator);
		}
		public static string GetDayNames(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.DayNames, false);
		}
		public static string GetMonthNames(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.MonthNames, true);
		}
		public static string GetPMDesignator(CultureInfo culture) {
			return Escape(culture.DateTimeFormat.PMDesignator);
		}
		public static string GetNumberDecimalSeparator(CultureInfo culture) {
			return Escape(culture.NumberFormat.NumberDecimalSeparator);
		}
		public static string GetCurrencyDecimalSeparator(CultureInfo culture) {
			return Escape(culture.NumberFormat.CurrencyDecimalSeparator);
		}
		public static string GetCurrencySymbol(CultureInfo culture) {
			return Escape(culture.NumberFormat.CurrencySymbol);
		}
		public static string GetNumberPattern(CultureInfo culture) {
			int decimalDigits = culture.NumberFormat.NumberDecimalDigits;
			return decimalDigits > 0 ? @"\d*\R{NumberDecimalSeparator}\d{" + decimalDigits.ToString() + "}" : @"\d+";
		}
		public static string GetCurrencyPattern(CultureInfo culture) {
			int decimalDigits = culture.NumberFormat.CurrencyDecimalDigits;
			return decimalDigits > 0 ? @"\d*\R{CurrencyDecimalSeparator}\d{" + decimalDigits.ToString() + "}" : @"\d+";
		}
		public static string DateSeparator {
			get {
				return GetDateSeparator(DefaultCulture);
			}
		}
		public static string TimeSeparator {
			get {
				return GetTimeSeparator(DefaultCulture);
			}
		}
		public static string AbbreviatedDayNames {
			get {
				return GetAbbreviatedDayNames(DefaultCulture);
			}
		}
		public static string AbbreviatedMonthNames {
			get {
				return GetAbbreviatedMonthNames(DefaultCulture);
			}
		}
		public static string AMDesignator {
			get {
				return GetAMDesignator(DefaultCulture);
			}
		}
		public static string DayNames {
			get {
				return GetDayNames(DefaultCulture);
			}
		}
		public static string MonthNames {
			get {
				return GetMonthNames(DefaultCulture);
			}
		}
		public static string PMDesignator {
			get {
				return GetPMDesignator(DefaultCulture);
			}
		}
		public static string NumberDecimalSeparator {
			get {
				return GetNumberDecimalSeparator(DefaultCulture);
			}
		}
		public static string CurrencyDecimalSeparator {
			get {
				return GetCurrencyDecimalSeparator(DefaultCulture);
			}
		}
		public static string CurrencySymbol {
			get {
				return GetCurrencySymbol(DefaultCulture);
			}
		}
		public static string NumberPattern {
			get {
				return GetNumberPattern(DefaultCulture);
			}
		}
		public static string CurrencyPattern {
			get {
				return GetCurrencyPattern(DefaultCulture);
			}
		}
		public static string GetNamedMask(string maskName, CultureInfo culture) {
			switch(maskName) {
				case "DateSeparator": return GetDateSeparator(culture);
				case "TimeSeparator": return GetTimeSeparator(culture);
				case "AbbreviatedDayNames": return GetAbbreviatedDayNames(culture);
				case "AbbreviatedMonthNames": return GetAbbreviatedMonthNames(culture);
				case "AMDesignator": return GetAMDesignator(culture);
				case "DayNames": return GetDayNames(culture);
				case "MonthNames": return GetMonthNames(culture);
				case "PMDesignator": return GetPMDesignator(culture);
				case "NumberDecimalSeparator": return GetNumberDecimalSeparator(culture);
				case "CurrencyDecimalSeparator": return GetCurrencyDecimalSeparator(culture);
				case "CurrencySymbol": return GetCurrencySymbol(culture);
				case "NumberPattern": return GetNumberPattern(culture);
				case "CurrencyPattern": return GetCurrencyPattern(culture);
				default:
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.IncorrectMaskUnknownNamedMask, maskName), "maskName");
			}
		}
		public static string GetNamedMask(string maskName) {
			return GetNamedMask(maskName, DefaultCulture);
		}
	}
}
