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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionFDotTest
	public class FunctionFDotTest : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "F.TEST"; } }
		public override int Code { get { return 0x4056; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue firstArray = arguments[0];
			if (firstArray.IsError)
				return firstArray;
			ConditionCalculationResult isInvalid = IsInvalid(firstArray);
			if (isInvalid == ConditionCalculationResult.ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (isInvalid == ConditionCalculationResult.True)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue secondArray = arguments[1];
			if (secondArray.IsError)
				return secondArray;
			isInvalid = IsInvalid(secondArray);
			if (isInvalid == ConditionCalculationResult.ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (isInvalid == ConditionCalculationResult.True)
				return VariantValue.ErrorInvalidValueInFunction;
			return GetNumericResult(firstArray, secondArray, context);
		}
		protected ConditionCalculationResult IsInvalid(VariantValue value) {
			bool isUnionRange = value.IsCellRange && value.CellRangeValue.RangeType == CellRangeType.UnionRange;
			bool hasNonNumericCellRangeValue = false;
			if (value.IsCellRange && value.CellRangeValue.CellCount == 1) {
				VariantValue firstValue = value.CellRangeValue.GetFirstCellValue();
				if (firstValue == VariantValue.ErrorGettingData)
					return ConditionCalculationResult.ErrorGettingData;
				if (!firstValue.IsNumeric)
					hasNonNumericCellRangeValue = true;
			}
			if (!(value.IsNumeric || value.IsArray || value.IsCellRange) || isUnionRange || hasNonNumericCellRangeValue)
				return ConditionCalculationResult.True;
			return ConditionCalculationResult.False;
		}
		protected int GetNumericCount(VariantValue value) {
			if (value.IsNumeric)
				return 1;
			return value.IsArray ? GetNumericCount(value.ArrayValue) : GetNumericCount(value.CellRangeValue);
		}
		int GetNumericCount(IVariantArray array) {
			int result = 0;
			long count = array.Count;
			for (int i = 0; i < count; i++)
				if (array[i].IsNumeric)
					result++;
			return result;
		}
		int GetNumericCount(CellRangeBase range) {
			IEnumerator<VariantValue> enumerator = range.GetExistingValuesEnumerator();
			int result = 0;
			while (enumerator.MoveNext()) {
				VariantValue current = enumerator.Current;
				if (current == VariantValue.ErrorGettingData)
					return int.MinValue;
				if (current.IsNumeric)
					result++;
			}
			return result;
		}
		VariantValue GetNumericResult(VariantValue firstArray, VariantValue secondArray, WorkbookDataContext context) {
			int firstArrayCount = GetNumericCount(firstArray);
			int secondArrayCount = GetNumericCount(secondArray);
			if (firstArrayCount == int.MinValue || secondArrayCount == int.MinValue)
				return VariantValue.ErrorGettingData;
			if (firstArrayCount < 2 || secondArrayCount < 2)
				return VariantValue.ErrorDivisionByZero;
			VariantValue xvarValue = GetVariance(firstArray, context);
			if (xvarValue.IsError)
				return xvarValue;
			VariantValue yvarValue = GetVariance(secondArray, context);
			if (yvarValue.IsError)
				return yvarValue;
			double xvar = xvarValue.NumericValue;
			double yvar = yvarValue.NumericValue;
			if (xvar == 0 || yvar == 0)
				return VariantValue.ErrorDivisionByZero;
			if (xvar > yvar)
				return 2 * (1.0 - FunctionFDotDist.GetResult(firstArrayCount - 1, secondArrayCount - 1, xvar / yvar));
			return 2 * (1.0 - FunctionFDotDist.GetResult(secondArrayCount - 1, firstArrayCount - 1, yvar / xvar));
		}
		protected IList<VariantValue> GetListArgument(VariantValue value, WorkbookDataContext context) {
			IList<VariantValue> result = new List<VariantValue>();
			result.Add(value);
			return result;
		}
		protected VariantValue GetVariance(VariantValue value, WorkbookDataContext context) {
			return GetFunctionResult(value, context, 0x002E);
		}
		protected VariantValue GetFunctionResult(VariantValue value, WorkbookDataContext context, int code) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByCode(code);
			return function.Evaluate(GetListArgument(value, context), context, false);
		}
	}
	#endregion
}
