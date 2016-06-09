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
	#region FunctionSeriesSum
	public class FunctionSeriesSum : WorksheetFunctionBase {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array | OperandDataType.Reference));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		#region Properties
		public override string Name { get { return "SERIESSUM"; } }
		public override int Code { get { return 0x019E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = context.ToNumericWithoutCrossing(arguments[0]);
			if (number.IsError)
				return number;
			VariantValue powerN = context.ToNumericWithoutCrossing(arguments[1]);
			if (powerN.IsError)
				return powerN;
			VariantValue powerM = context.ToNumericWithoutCrossing(arguments[2]);
			if (powerM.IsError)
				return powerM;
			VariantValue coeffArray = arguments[3];
			if (coeffArray.IsError)
				return coeffArray;
			return ProcessArray(number.NumericValue, powerN.NumericValue, powerM.NumericValue, coeffArray, context);
		}
		VariantValue ProcessArray(double number, double powerN, double powerM, VariantValue coeffArray, WorkbookDataContext context) {
			IVector<VariantValue> coeffVector;
			if (coeffArray.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			else if (coeffArray.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			else if (coeffArray.IsArray)
				coeffVector = new ArrayZVector(coeffArray.ArrayValue);
			else if (coeffArray.IsCellRange) {
				CellRangeBase cellRange = coeffArray.CellRangeValue;
				if (cellRange.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorInvalidValueInFunction;
				coeffVector = new RangeZVector(cellRange.GetFirstInnerCellRange());
			}
			else {
				VariantValue coeff = coeffArray.ToNumeric(context);
				if (coeff.IsError)
					return coeff;
				return GetSeriesSumMember(number, powerN, powerM, 1, coeff.NumericValue);
			}
			return GetResult(number, powerN, powerM, coeffVector, context);
		}
		VariantValue GetSeriesSumMember(double number, double powerN, double powerM, int step, double coeff) {
			double power = powerN + (step - 1) * powerM;
			if (number == 0 && power <= 0)
				return VariantValue.ErrorNumber;
			bool rootFromNegative = number < 0 && Math.Abs(power) != (int)Math.Abs(power);
			double result = rootFromNegative ? 0 : coeff * Math.Pow(number, power);
			return Double.IsInfinity(result) ? VariantValue.ErrorNumber : result;
		}
		VariantValue GetResult(double number, double powerN, double powerM, IVector<VariantValue> coeffVector, WorkbookDataContext context) {
			double seriesSum = 0;
			int nonEmptyValues = 0;
			int count = coeffVector.Count;
			if (count == 1)
				return GetSingleElementResult(number, powerN, powerM, coeffVector[0], context);
			for (int i = 0; i < count; i++) {
				VariantValue coeff = coeffVector[i];
				if (coeff.IsError)
					return coeff;
				else if (coeff.IsEmpty)
					continue;
				else if (!coeff.IsNumeric)
					return VariantValue.ErrorInvalidValueInFunction;
				else {
					nonEmptyValues++;
					VariantValue current = GetSeriesSumMember(number, powerN, powerM, nonEmptyValues, coeff.NumericValue);
					if (current.IsError)
						return current;
					seriesSum += current.NumericValue;
					if (Double.IsInfinity(seriesSum))
						return VariantValue.ErrorNumber;
				}
			}
			return seriesSum;
		}
		VariantValue GetSingleElementResult(double number, double powerN, double powerM, VariantValue coeff, WorkbookDataContext context) {
			if (coeff.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			coeff = coeff.ToNumeric(context);
			if (coeff.IsError)
				return coeff;
			return GetSeriesSumMember(number, powerN, powerM, 1, coeff.NumericValue);
		}
	}
	#endregion
}
