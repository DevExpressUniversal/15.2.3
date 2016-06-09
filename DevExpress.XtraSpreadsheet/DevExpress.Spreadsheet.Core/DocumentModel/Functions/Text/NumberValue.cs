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

using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionNumberValue
	public class FunctionNumberValue : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "NUMBERVALUE"; } }
		public override int Code { get { return 0x4037; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue text = arguments[0].ToText(context);
			if (text.IsError)
				return text;
			string strDecSeparator = context.Culture.NumberFormat.CurrencyDecimalSeparator;
			string strGrSeparator  = context.Culture.NumberFormat.CurrencyGroupSeparator;
			if (arguments.Count > 1) {
				VariantValue decSeparator = arguments[1].ToText(context);
				if (decSeparator.IsError)
					return decSeparator;
				strDecSeparator = decSeparator.InlineTextValue;
				if (string.IsNullOrEmpty(strDecSeparator))
					return VariantValue.ErrorInvalidValueInFunction;
				strDecSeparator = strDecSeparator.Substring(0, 1);
			}
			if (arguments.Count > 2) {
				VariantValue grSeparator = arguments[2].ToText(context);
				if (grSeparator.IsError)
					return grSeparator;
				strGrSeparator = grSeparator.InlineTextValue;
				if (string.IsNullOrEmpty(strGrSeparator))
					return VariantValue.ErrorInvalidValueInFunction;
				strGrSeparator = strGrSeparator.Substring(0, 1);
				if (strGrSeparator == strDecSeparator)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return GetResult(text.InlineTextValue, strDecSeparator, strGrSeparator);
		}
		VariantValue GetResult(string value, string decSeparator, string grSeparator) {
			string number = value.Replace(" ", string.Empty);
			if (!number.Contains(decSeparator))
				number = number.TrimEnd(grSeparator[0]);
			if (string.IsNullOrEmpty(number))
				return 0;
			bool isCurrency = false;
			string temp = number.TrimEnd('%');
			if (temp.Contains("%"))
				if (temp[temp.Length - 1] == '€')
					return VariantValue.ErrorInvalidValueInFunction;
			DeleteCurrencySymbol(number, out number, ref isCurrency);
			CultureInfo culture = new CultureInfo(CultureInfo.InvariantCulture.Name);
			NumberFormatInfo numberFormat = culture.NumberFormat;
			string tempSeparator = "D";
			if (grSeparator == tempSeparator) {
				tempSeparator = "d";
			}
			number = number.Replace(decSeparator, tempSeparator);
			decSeparator = tempSeparator;
			numberFormat.NumberDecimalSeparator = tempSeparator;
			number = number.Replace(grSeparator, "G");
			grSeparator = "G";
			numberFormat.NumberGroupSeparator = "G";
			bool isNegative = false;
			if (!DeleteNegativeSignAndParentheses(number, out number, ref isNegative, numberFormat))
				return VariantValue.ErrorInvalidValueInFunction;
			if (!DeleteCurrencySymbol(number, out number, ref isCurrency))
				return VariantValue.ErrorInvalidValueInFunction;
			number = number.TrimStart(grSeparator[0]);
			if (!number.Contains(decSeparator))
				number = number.TrimEnd(grSeparator[0]);
			int percentDenominator = 1;
			if (!DeletePercentSymbol(number, out number, ref percentDenominator, isCurrency))
				return VariantValue.ErrorInvalidValueInFunction;
			number = number.TrimStart(grSeparator[0]);
			if (!isNegative && !isCurrency && string.IsNullOrEmpty(number))
				return 0;
			double result = 0;
			if (!double.TryParse(number, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, culture, out result))
				return VariantValue.ErrorInvalidValueInFunction;
			result /= percentDenominator;
			return isNegative ? -result : result;
		}
		bool DeleteNegativeSignAndParentheses(string number, out string result, ref bool isNegative, NumberFormatInfo numberFormat){
			result = number;
			if (result[0] == numberFormat.NegativeSign[0]) {
				isNegative = true;
				result = result.Substring(1, result.Length - 1);
			}
			result = result.TrimStart(numberFormat.NumberGroupSeparator[0]);
			string temp = result.Replace("%", string.Empty);
			if (result.IndexOf('(') == 0) {
				if (temp.LastIndexOf(')') == temp.Length - 1) {
					if (isNegative)
						return false;
					isNegative = true;
					result = result.Substring(1, result.Length - 1);
					result = result.Remove(result.LastIndexOf(')'), 1);
				}
			}
			return true;
		}
		bool DeletePercentSymbol(string number, out string result, ref int percentDenominator, bool isCurrency) {
			result = number;
			int index = result.IndexOf('%');
			if (isCurrency && index == 0)
				return false;
			while (index == 0) {
				percentDenominator *= 100;
				result = result.Substring(1, result.Length - 1);
				index = result.IndexOf('%');
			}
			percentDenominator *= index > 0 ? (int)System.Math.Pow(100, result.Length - index) : 1;
			result = result.TrimEnd('%');
			return true;
		}
		bool DeleteCurrencySymbol(string number, out string result, ref bool isCurrency) {
			result = number;
			if (number.Length == 0)
				return true;
			if (number[0] == '$' || number[0] == '€') {
				if (isCurrency)
					return false;
				isCurrency = true;
				result = result.Substring(1);
			}
			int indOfEuro = number.LastIndexOf('€');
			number = number.TrimEnd('%');
			if (string.IsNullOrEmpty(number))
				return true;
			if (number[number.Length - 1] == '€') {
				if (isCurrency)
					return false;
				isCurrency = true;
				result = result.Remove(indOfEuro, 1);
			}
			return true;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
