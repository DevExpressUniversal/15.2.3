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
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetNotationFunctionBase
	public abstract class WorksheetNotationFunctionBase : WorksheetFunctionBase {
		#region Static
		static readonly Dictionary<char, byte> hexToDecTranslationTable = CreateHexToDecTranslationTable();
		static readonly Dictionary<char, byte> octToDecTranslationTable = CreateOctToDecTranslationTable();
		static readonly Dictionary<char, byte> binToDecTranslationTable = CreateBinToDecTranslationTable();
		static readonly Dictionary<byte, string> decToHexTranslationTable = CreateDecToHexTranslationTable();
		#region CreateHexToDecTranslationTable
		static Dictionary<char, byte> CreateHexToDecTranslationTable() {
			Dictionary<char, byte> result = new Dictionary<char, byte>();
			result.Add('0', 0);
			result.Add('1', 1);
			result.Add('2', 2);
			result.Add('3', 3);
			result.Add('4', 4);
			result.Add('5', 5);
			result.Add('6', 6);
			result.Add('7', 7);
			result.Add('8', 8);
			result.Add('9', 9);
			result.Add('A', 10);
			result.Add('B', 11);
			result.Add('C', 12);
			result.Add('D', 13);
			result.Add('E', 14);
			result.Add('F', 15);
			result.Add('a', 10);
			result.Add('b', 11);
			result.Add('c', 12);
			result.Add('d', 13);
			result.Add('e', 14);
			result.Add('f', 15);
			return result;
		}
		#endregion
		#region CreateOctToDecTranslationTable
		static Dictionary<char, byte> CreateOctToDecTranslationTable() {
			Dictionary<char, byte> result = new Dictionary<char, byte>();
			result.Add('0', 0);
			result.Add('1', 1);
			result.Add('2', 2);
			result.Add('3', 3);
			result.Add('4', 4);
			result.Add('5', 5);
			result.Add('6', 6);
			result.Add('7', 7);
			return result;
		}
		#endregion
		#region CreateBinToDecTranslationTable
		static Dictionary<char, byte> CreateBinToDecTranslationTable() {
			Dictionary<char, byte> result = new Dictionary<char, byte>();
			result.Add('0', 0);
			result.Add('1', 1);
			return result;
		}
		#endregion
		#region CreateDecToHexTranslationTable
		static Dictionary<byte, string> CreateDecToHexTranslationTable() {
			Dictionary<byte, string> result = new Dictionary<byte, string>();
			result.Add(0, "0");
			result.Add(1, "1");
			result.Add(2, "2");
			result.Add(3, "3");
			result.Add(4, "4");
			result.Add(5, "5");
			result.Add(6, "6");
			result.Add(7, "7");
			result.Add(8, "8");
			result.Add(9, "9");
			result.Add(10, "A");
			result.Add(11, "B");
			result.Add(12, "C");
			result.Add(13, "D");
			result.Add(14, "E");
			result.Add(15, "F");
			return result;
		}
		#endregion
		static protected Dictionary<char, byte> HexToDecTranslationTable { get { return hexToDecTranslationTable; } }
		static protected Dictionary<char, byte> OctToDecTranslationTable { get { return octToDecTranslationTable; } }
		static protected Dictionary<char, byte> BinToDecTranslationTable { get { return binToDecTranslationTable; } }
		static protected Dictionary<byte, string> DecToHexTranslationTable { get { return decToHexTranslationTable; } }
		#endregion
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected VariantValue ConvertFromDec(long value, byte baseTo) {
			string result = String.Empty;
			value = value >= 0 ? value : 2 * GetMaxValue(baseTo) + value;
			do {
				result = DecToHexTranslationTable[(byte)(value % baseTo)] + result;
				value /= baseTo;
			} while (value != 0);
			return result;
		}
		protected VariantValue ConvertToDec(string value, byte baseFrom, Dictionary<char, byte> translationTable) {
			long result = 0;
			long pow = 1;
			int length = value.Length;
			if (length > 10)
				return VariantValue.ErrorNumber;
			for (int i = length - 1; i >= 0; i--) {
				char currentDigit = value[i];
				if (!translationTable.ContainsKey(currentDigit))
					return VariantValue.ErrorNumber;
				byte fractionValue = translationTable[currentDigit];
				result += fractionValue * pow;
				pow *= baseFrom;
			}
			long maxValue = GetMaxValue(baseFrom);
			if (result >= maxValue)
				result -= maxValue * 2;
			return result;
		}
		protected virtual VariantValue GetNumberValue(VariantValue value, WorkbookDataContext context) {
			VariantValue number = context.DereferenceWithoutCrossing(value);
			return number.ToNumeric(context);
		}
		protected VariantValue GetTextValue(VariantValue value, WorkbookDataContext context) {
			VariantValue number = context.DereferenceWithoutCrossing(value);
			return number.ToText(context);
		}
		protected VariantValue AddLeadingZeros(string value, int intPlaces) {
			if (intPlaces != 0 && intPlaces < value.Length)
				return VariantValue.ErrorNumber;
			return GetZeroPrefix(intPlaces - value.Length) + value;
		}
		protected string GetZeroPrefix(int countZeroes) {
			string result = string.Empty;
			if (countZeroes > 0)
				result += new String('0', countZeroes);
			return result;
		}
		protected long GetMaxValue(byte baseTo) {
			int power = (int)Math.Log(baseTo, 2) * 10 - 1;
			return (long)1 << power;
		}
		protected VariantValue GetStringResultFromDec(long value, int placesCount, byte baseTo) {
			VariantValue result = ConvertFromDec(value, baseTo);
			if (value >= 0)
				result = AddLeadingZeros(result.InlineTextValue, placesCount);
			return result;
		}
	}
	#endregion
	#region WorksheetFromDecNumerationFunctionBase
	public abstract class WorksheetFromDecNotationFunctionBase : WorksheetNotationFunctionBase {
		protected abstract byte BaseTo { get; }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int intPlaces = 0;
			if (arguments.Count > 1 && !arguments[1].IsEmpty) {
				VariantValue numericPlaces = GetNumberValue(arguments[1], context);
				if (numericPlaces.IsError)
					return numericPlaces;
				intPlaces = (int)numericPlaces.NumericValue;
				if (intPlaces < 1 || intPlaces > 10)
					return VariantValue.ErrorNumber;
			}
			if (arguments[0].IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			VariantValue number = GetNumberValue(arguments[0], context);
			if (number.IsError)
				return number;
			long value = (long)number.NumericValue;
			long maxValue = GetMaxValue(BaseTo);
			if (value < -maxValue || value > maxValue - 1)
				return VariantValue.ErrorNumber;
			return GetStringResultFromDec(value, intPlaces, BaseTo);
		}
	}
	#endregion
	#region WorksheetSingleArgumentNumerationFunctionBase
	public abstract class WorksheetSingleArgumentNotationFunctionBase : WorksheetNotationFunctionBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue textValue = GetTextValue(arguments[0], context);
			if (textValue.IsError)
				return textValue;
			return EvaluateArguments(textValue.InlineTextValue);
		}
		protected abstract VariantValue EvaluateArguments(string value);
	}
	#endregion
	#region WorksheetDoubleArgumentNumerationFunctionBase
	public abstract class WorksheetDoubleArgumentNotationFunctionBase : WorksheetNotationFunctionBase {
		protected abstract Dictionary<char, byte> ToDecTranslationTable { get; }
		protected abstract byte BaseFrom { get; }
		protected abstract byte BaseTo { get; }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int intPlaces = 0;
			if (arguments.Count > 1 && !arguments[1].IsEmpty) {
				VariantValue numericPlaces = GetNumberValue(arguments[1], context);
				if (numericPlaces.IsError)
					return numericPlaces;
				intPlaces = (int)numericPlaces.NumericValue;
				if (intPlaces < 1 || intPlaces > 10)
					return VariantValue.ErrorNumber;
			}
			if (arguments[0].IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			VariantValue textValue = GetTextValue(arguments[0], context);
			if (textValue.IsError)
				return textValue;
			VariantValue toDecConvertValue = ConvertToDec(textValue.InlineTextValue, BaseFrom, ToDecTranslationTable);
			if (toDecConvertValue.IsError)
				return toDecConvertValue;
			long toDecResult = (long)toDecConvertValue.ToNumeric(context).NumericValue;
			long maxValue = GetMaxValue(Math.Min(BaseFrom, BaseTo));
			if (toDecResult < -maxValue || toDecResult > maxValue - 1)
				return VariantValue.ErrorNumber;
			return GetStringResultFromDec(toDecResult, intPlaces, BaseTo);
		}
	}
	#endregion
	#region WorksheetLogicShiftOperationFunctionBase (abstract base)
	public abstract class WorksheetLogicShiftOperationFunctionBase : WorksheetNotationFunctionBase {
		protected const long maxNumber = 281474976710655;
		const long maxShiftNumber = 53;
		protected virtual bool IsLeftShift { get { return true; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue firstNumberValue = GetNumberValue(arguments[0], context);
			if (firstNumberValue.IsError)
				return firstNumberValue;
			VariantValue secondNumberValue = GetSecondNumberValue(arguments[1], context);
			if (secondNumberValue.IsError)
				return secondNumberValue;
			return GetResult((long)firstNumberValue.NumericValue, (long)secondNumberValue.NumericValue);
		}
		protected virtual VariantValue GetResult(long firstNumber, long secondNumber) {
			if (firstNumber < 0 || firstNumber > maxNumber || Math.Abs(secondNumber) > maxShiftNumber)
				return VariantValue.ErrorNumber;
			string stringValue = ConvertToBin(firstNumber);
			int shiftNumber = (int)(IsLeftShift ? secondNumber : -secondNumber);
			stringValue = ShiftString(stringValue, shiftNumber);
			long result = 0;
			for (int i = 0; i < stringValue.Length; i++) {
				if (stringValue[i] == '1')
					result += (long)Math.Pow(2, i);
			}
			if (result > maxNumber)
				return VariantValue.ErrorNumber;
			return result;
		}
		string ShiftString(string stringValue, int shiftNumber) {
			if (shiftNumber >= 0)
				for (int i = 0; i < shiftNumber; i++)
					stringValue = '0' + stringValue;
			else {
				if (Math.Abs(shiftNumber) > stringValue.Length)
					return "0";
				stringValue = stringValue.Substring(-shiftNumber, stringValue.Length + shiftNumber);
			}
			return stringValue;
		}
		protected string ConvertToBin(long value) {
			string result = String.Empty;
			do {
				result = result + (value % 2);
				value /= 2;
			} while (value != 0);
			return result;
		}
		protected override VariantValue GetNumberValue(VariantValue argument, WorkbookDataContext context) {
			VariantValue value = argument.ToNumeric(context);
			double numericValue = value.NumericValue;
			if (numericValue - (long)numericValue != 0)
				return VariantValue.ErrorNumber;
			return value;
		}
		protected virtual VariantValue GetSecondNumberValue(VariantValue argument, WorkbookDataContext context) {
			return argument.ToNumeric(context);
		}
	}
	#endregion
	#region WorksheetLogicBitFunctionBase (abstract base)
	public abstract class WorksheetLogicBitOperationFunctionBase : WorksheetLogicShiftOperationFunctionBase {
		protected override VariantValue GetResult(long firstNumber, long secondNumber) {
			if (firstNumber < 0 || secondNumber < 0 || firstNumber > maxNumber || secondNumber > maxNumber)
				return VariantValue.ErrorNumber;
			string firstBinString = ConvertToBin(firstNumber);
			string secondBinString = ConvertToBin(secondNumber);
			int maxLength = 0;
			if (firstBinString.Length > secondBinString.Length) {
				maxLength = firstBinString.Length;
				secondBinString += GetZeroPrefix(maxLength - secondBinString.Length);
			}
			else {
				maxLength = secondBinString.Length;
				firstBinString += GetZeroPrefix(maxLength - firstBinString.Length);
			}
			long result = 0;
			for (int i = 0; i < maxLength; i++) {
				if (GetResultBitOperation(firstBinString[i] == '1', secondBinString[i] == '1'))
					result += (long)Math.Pow(2, i);
			}
			return result;
		}
		protected override VariantValue GetSecondNumberValue(VariantValue argument, WorkbookDataContext context) {
			return GetNumberValue(argument, context);
		}
		protected abstract bool GetResultBitOperation(bool firstOperand, bool secondOperand);
	}
	#endregion
}
