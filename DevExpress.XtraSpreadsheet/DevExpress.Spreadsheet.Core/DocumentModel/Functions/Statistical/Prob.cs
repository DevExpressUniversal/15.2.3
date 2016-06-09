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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionProb
	public class FunctionProb : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Fields
		public override string Name { get { return "PROB"; } }
		public override int Code { get { return 0x13D; } }
		#endregion
		#region Properties
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array1 = arguments[0];
			if (array1.IsError)
				return array1;
			if (IsNotArrayCellRangeOrNumeric(array1))
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue array2 = arguments[1];
			if (array2.IsError)
				return array2;
			if (IsNotArrayCellRangeOrNumeric(array2))
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double lowerLimit = value.NumericValue;
			double upperLimit = lowerLimit;
			if (arguments.Count > 3) {
				value = arguments[3].ToNumeric(context);
				if (value.IsError)
					return value;
				upperLimit = value.NumericValue;
			}
			return GetResult(array1, array2, lowerLimit, upperLimit);
		}
		bool IsNotArrayCellRangeOrNumeric(VariantValue value) {
			if (value.IsCellRange)
				return value.CellRangeValue.RangeType == CellRangeType.UnionRange;
			return !(value.IsArray || value.IsNumeric);
		}
		VariantValue GetResult(VariantValue array1, VariantValue array2, double lowerLimit, double upperLimit) {
			IVector<VariantValue> xRange = CreateVector(array1);
			IVector<VariantValue> probabilityRange = CreateVector(array2);
			if (xRange.Count != probabilityRange.Count)
				return VariantValue.ErrorValueNotAvailable;
			if (lowerLimit == upperLimit)
				return CalculateResult(xRange, probabilityRange, lowerLimit, upperLimit, EqualLimits);
			else if (lowerLimit > upperLimit)
				return CalculateResult(xRange, probabilityRange, lowerLimit, upperLimit, LowerLimitIsBigger);
			return CalculateResult(xRange, probabilityRange, lowerLimit, upperLimit, UpperLimitIsBigger);
		}
		IVector<VariantValue> CreateVector(VariantValue value) {
			if (value.IsArray)
				return new ArrayZVector(value.ArrayValue);
			else if (value.IsCellRange)
				return new RangeZVector(value.CellRangeValue.GetFirstInnerCellRange());
			return CreateArrayFromNumeric(value);
		}
		ArrayZVector CreateArrayFromNumeric(VariantValue value) {
			VariantArray array = VariantArray.Create(1, 1);
			array.SetValue(0, 0, value);
			return new ArrayZVector(array);
		}
		delegate bool GetConditions(double x, double lowerLimit, double upperLimit);
		bool EqualLimits(double x, double lowerLimit, double upperLimit) {
			return x == lowerLimit;
		}
		bool LowerLimitIsBigger(double x, double lowerLimit, double upperLimit) {
			return false;
		}
		bool UpperLimitIsBigger(double x, double lowerLimit, double upperLimit) {
			return x >= lowerLimit && x <= upperLimit;
		}
		VariantValue CalculateResult(IVector<VariantValue> xRange, IVector<VariantValue> probabilityRange, double lowerLimit, double upperLimit, GetConditions getConditions) {
			double result = 0;
			double probabilitySum = 0;
			int count = xRange.Count;
			for (int i = 0; i < count; i++) {
				VariantValue currentX = xRange[i];
				if (currentX.IsError)
					return currentX;
				VariantValue currentProbability = probabilityRange[i];
				if (currentProbability.IsError)
					return currentProbability;
				if (currentX.IsNumeric && currentProbability.IsNumeric) {
					double currentProbabilityValue = currentProbability.NumericValue;
					bool xFound = getConditions(currentX.NumericValue, lowerLimit, upperLimit);
					probabilitySum += currentProbabilityValue;
					if (xFound)
						result += currentProbabilityValue;
				}
			}
			if (Math.Abs(probabilitySum - 1.0) < 1e-14)
				probabilitySum = 1.0;
			return probabilitySum != 1.0 ? VariantValue.ErrorNumber : result;
		}
	}
	#endregion
}
