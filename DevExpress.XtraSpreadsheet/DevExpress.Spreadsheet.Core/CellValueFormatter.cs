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

using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Diagnostics;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet {
	public static class CellValueFormatter {
		const DateTimeStyles defaultFlags = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal | DateTimeStyles.NoCurrentDateDefault;
		public static FormattedVariantValue GetValue(VariantValue previousValue, string value, WorkbookDataContext context, bool calculateFormat, NumberFormat format) {
			if (!previousValue.IsEmpty)
				return GetValueBasedOnPreviousType(value, previousValue.Type, context, calculateFormat, format);
			else
				return GetValueCore(value, context, calculateFormat, format);
		}
		public static FormattedVariantValue GetValue(VariantValue previousValue, string value, WorkbookDataContext context, bool calculateFormat) {
			return GetValue(previousValue, value, context, calculateFormat, NumberFormat.Generic);
		}
		static FormattedVariantValue GetValueBasedOnPreviousType(string value, VariantValueType previousType, WorkbookDataContext context, bool calculateFormat, NumberFormat format) {
			if (previousType == VariantValueType.Numeric) {
				FormattedVariantValue result = TryParseDouble(value, context, calculateFormat, format);
				if (!result.IsEmpty)
					return result;
				result = TryParseDoubleWithAllowThousands(value, context.Culture, calculateFormat);
				if (!result.IsEmpty)
					return result;
			}
			return GetValueCore(value, context, calculateFormat, format);
		}
		internal static FormattedVariantValue GetValueCore(string value, WorkbookDataContext context, bool calculateFormat) {
			return GetValueCore(value, context, calculateFormat, NumberFormat.Generic);
		}
		static FormattedVariantValue GetValueCore(string value, WorkbookDataContext context, bool calculateFormat, NumberFormat format) {
			FormattedVariantValue result;
			result = TryParseDouble(value, context, calculateFormat, format);
			if (!result.IsEmpty)
				return result;
			result = TryParseDateTime(value, context, calculateFormat);
			if (!result.IsEmpty)
				return result;
			result = TryParseDoubleWithAllowThousands(value, context.Culture, calculateFormat);
			if (!result.IsEmpty)
				return result;
			result = TryParseBoolean(value, context, calculateFormat);
			if (!result.IsEmpty)
				return result;
			result = TryParseError(value, context, calculateFormat);
			if (!result.IsEmpty)
				return result;
			return new FormattedVariantValue(value, 0);
		}
		internal static FormattedVariantValue TryParseDouble(string value, WorkbookDataContext context, bool calculateFormat) {
			FormattedVariantValue result = TryParseDouble(value, context, calculateFormat, NumberFormat.Generic);
			if (!result.IsEmpty)
				return result;
			return TryParseDoubleWithAllowThousands(value, context.Culture, calculateFormat);
		}
		static FormattedVariantValue TryParseDouble(string value, WorkbookDataContext context, bool calculateFormat, NumberFormat format) {
#if !DXPORTABLE
			Debug.Assert(!String.IsNullOrEmpty(value), "CellValueFormatter. TryParseDouble value is null or empty.");
#endif
			double doubleResult;
			if (double.TryParse(value, NumberStyles.Float, context.Culture, out doubleResult)) {
				if (double.IsNaN(doubleResult) || double.IsInfinity(doubleResult))
					return FormattedVariantValue.Empty;
				return new FormattedVariantValue(doubleResult, 0);
			}
			if (!calculateFormat)
				return FormattedVariantValue.Empty;
			string trimmedValue = value.Trim();
			if (String.IsNullOrEmpty(trimmedValue))
				return FormattedVariantValue.Empty;
			if (trimmedValue[0] == '%')
				trimmedValue = trimmedValue.Substring(1, trimmedValue.Length - 1);
			else if (trimmedValue[trimmedValue.Length - 1] == '%') {
				trimmedValue = trimmedValue.Substring(0, trimmedValue.Length - 1);
				if (!format.IsGeneric && TryParseFractionNumbers(trimmedValue, context.Culture, out doubleResult))
					return new FormattedVariantValue(doubleResult / 100.0, context.Workbook.Cache.NumberFormatCache.GetItemIndex(format));
			}
			else {
				trimmedValue = value.Trim();
				if (!format.IsGeneric && TryParseFractionNumbers(trimmedValue, context.Culture, out doubleResult))
					return new FormattedVariantValue(doubleResult, context.Workbook.Cache.NumberFormatCache.GetItemIndex(format));
				return FormattedVariantValue.Empty;
			}
			if (double.TryParse(trimmedValue, NumberStyles.Float, context.Culture, out doubleResult)) {
				if (doubleResult == Math.Round(doubleResult))
					return new FormattedVariantValue(doubleResult / 100.0, 9);
				else
					return new FormattedVariantValue(doubleResult / 100.0, 10);
			}
			return FormattedVariantValue.Empty;
		}
		static FormattedVariantValue TryParseDoubleWithAllowThousands(string value, CultureInfo culture, bool calculateFormat) {
			if (!calculateFormat)
				return FormattedVariantValue.Empty;
			double doubleResult;
			if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, culture, out doubleResult)
#if DXPORTABLE
				|| double.TryParse(RemoveThousandSeparators(value, culture), NumberStyles.Float | NumberStyles.AllowThousands, culture, out doubleResult)
#endif
				) {
				if (double.IsNaN(doubleResult) || double.IsInfinity(doubleResult))
					return FormattedVariantValue.Empty;
				NumberFormatInfo numberFormat = culture.NumberFormat;
				int decimalSeparatorIndex = value.IndexOf(numberFormat.NumberDecimalSeparator);
				string integerPart = decimalSeparatorIndex > 0 ? value.Substring(0, decimalSeparatorIndex) : value;
				if (IsCorrectThousandsDouble(integerPart, numberFormat.NumberGroupSeparator, numberFormat.NumberGroupSizes)) {
					if (doubleResult == Math.Round(doubleResult))
						return new FormattedVariantValue(doubleResult, 3);
					else
						return new FormattedVariantValue(doubleResult, 4);
				}
			}
			return FormattedVariantValue.Empty;
		}
#if DXPORTABLE
		static string RemoveThousandSeparators(string value, CultureInfo culture) {
			string separator = culture.NumberFormat.NumberGroupSeparator;
			if (String.IsNullOrEmpty(separator))
				return value;
			return value.Replace(separator, String.Empty).Replace(" ", String.Empty);
		}
#endif
		static bool IsCorrectThousandsDouble(string value, string separator, int[] numberGroupSizes) {
			if (numberGroupSizes.Length == 0)
				return false;
			int oldPosition = value.Length - 1;
			int newPosition = 0;
			int lastNumberGroupIndex = numberGroupSizes.Length - 1;
			for (int i = 0; ; i++) {
				int current = i > lastNumberGroupIndex ? numberGroupSizes[lastNumberGroupIndex] : numberGroupSizes[i];
				newPosition = value.LastIndexOf(separator, oldPosition);
				if (newPosition < 0 || current == 0)
					return true;
				if (oldPosition - newPosition < current)
					return false;
				oldPosition = newPosition - 1;
			}
		}
		static bool TryParseFractionNumbers(string value, CultureInfo cultureInfo, out double doubleResult) {
			doubleResult = 0.0;
			int firstPosition = value.IndexOf('/');
			if (firstPosition > 0) {
				string dividentText = value.Substring(0, firstPosition);
				double divident;
				if (!double.TryParse(dividentText, NumberStyles.Float, cultureInfo, out divident))
					return false;
				string divisorText = value.Substring(firstPosition + 1);
				double divisor;
				if (!double.TryParse(divisorText, NumberStyles.Float, cultureInfo, out divisor))
					return false;
				if ((divident == 0 && divisor == 0) || divisor <= 0)
					return false;
				doubleResult = divident / divisor;
				return true;
			}
			return false;
		}
		static FormattedVariantValue TryParseDateTime(string value, WorkbookDataContext context, bool calculateFormat) {
			FormattedVariantValue formattedDateTimeValue = context.TryConvertStringToDateTimeValue(value, calculateFormat);
			VariantValue dateTimeValue = formattedDateTimeValue.Value;
			if (!dateTimeValue.IsEmpty && !dateTimeValue.IsError)
				return formattedDateTimeValue;
			else
				return FormattedVariantValue.Empty;
		}
		static FormattedVariantValue TryParseBoolean(string value, WorkbookDataContext context, bool calculateFormat) {
			bool boolValue;
			if (context.TryParseBoolean(value, out boolValue))
				return new FormattedVariantValue(boolValue, 0);
			else
				return FormattedVariantValue.Empty;
		}
		static FormattedVariantValue TryParseError(string value, WorkbookDataContext context, bool calculateFormat) {
			ICellError error = CellErrorFactory.CreateError(value, context);
			if (error != null)
				return new FormattedVariantValue(error.Value, 0);
			else
				return FormattedVariantValue.Empty;
		}
	}
}
