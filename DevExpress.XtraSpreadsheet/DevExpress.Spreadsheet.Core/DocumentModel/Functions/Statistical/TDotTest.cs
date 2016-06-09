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
	#region FunctionTDotTest
	public class FunctionTDotTest : FunctionFDotTest {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "T.TEST"; } }
		public override int Code { get { return 0x4059; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue firstArray = arguments[0];
			ConditionCalculationResult isInvalid = IsInvalid(firstArray);
			if (isInvalid == ConditionCalculationResult.ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (!firstArray.IsError && (isInvalid == ConditionCalculationResult.True))
				return VariantValue.ErrorInvalidValueInFunction;
			if (firstArray.IsError)
				return firstArray;
			VariantValue secondArray = arguments[1];
			isInvalid = IsInvalid(secondArray);
			if (isInvalid == ConditionCalculationResult.ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (!secondArray.IsError && (isInvalid == ConditionCalculationResult.True))
				return VariantValue.ErrorInvalidValueInFunction;
			if (secondArray.IsError)
				return secondArray;
			VariantValue value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double tails = value.NumericValue;
			value = arguments[3].ToNumeric(context);
			if (value.IsError)
				return value;
			double type = value.NumericValue;
			if (tails < 1 || tails >= 3 || type < 1 || type >= 4)
				return VariantValue.ErrorNumber;
			return GetNumericResult(firstArray, secondArray, (int)tails, (int)type, context);
		}
		VariantValue GetNumericResult(VariantValue firstArray, VariantValue secondArray, int tails, int type, WorkbookDataContext context) {
			VariantValue result = type == 1 ? GetResultForPairedTwoSampleForMeans(firstArray, secondArray, context) : GetResultForTwoSampleAssumingVariances(firstArray, secondArray, type, context);
			if (result.IsError)
				return result;
			return tails * result.NumericValue;
		}
		VariantValue GetResultForPairedTwoSampleForMeans(VariantValue firstArray, VariantValue secondArray, WorkbookDataContext context) {
			VariantValue array = GetDifferenceArray(ConvertToArray(firstArray), ConvertToArray(secondArray));
			if (array.IsError)
				return array;
			long count = array.ArrayValue.Count;
			if (count < 2)
				return VariantValue.ErrorDivisionByZero;
			VariantValue averageValue = GetAverage(array, context);
			if (averageValue.IsError)
				return averageValue;
			VariantValue varianceValue = GetVariance(array, context);
			if (varianceValue.IsError)
				return varianceValue;
			double variance = varianceValue.NumericValue;
			if (variance == 0)
				return VariantValue.ErrorDivisionByZero;
			return GetTDotDist(count, Math.Abs(averageValue.NumericValue), variance, count - 1);
		}
		IVariantArray ConvertToArray(VariantValue value) {
			if (value.IsNumeric) {
				VariantArray array = VariantArray.Create(1, 1);
				array[0] = value;
				return array;
			}
			if (value.IsCellRange)
				return new RangeVariantArray(value.CellRangeValue.GetFirstInnerCellRange());
			return value.ArrayValue;
		}
		VariantValue GetDifferenceArray(IVariantArray firstArray, IVariantArray secondArray) {
			if (firstArray.Count != secondArray.Count)
				return VariantValue.ErrorValueNotAvailable;
			IList<VariantValue> values = new List<VariantValue>();
			long count = firstArray.Count;
			for (int i = 0; i < count; i++) {
				VariantValue first = firstArray[i];
				if (first.IsError)
					return first;
				VariantValue second = secondArray[i];
				if (second.IsError)
					return second;
				if (first.IsNumeric && second.IsNumeric)
					values.Add(first.NumericValue - second.NumericValue);
			}
			VariantArray array = new VariantArray();
			array.SetValues(values, 1, values.Count);
			return VariantValue.FromArray(array);
		}
		VariantValue GetResultForTwoSampleAssumingVariances(VariantValue firstArray, VariantValue secondArray, int type, WorkbookDataContext context) {
			int firstCount = GetNumericCount(firstArray);
			int secondCount = GetNumericCount(secondArray);
			if (firstCount < 2 || secondCount < 2)
				return VariantValue.ErrorDivisionByZero;
			VariantValue firstAverageValue = GetAverage(firstArray, context);
			if (firstAverageValue.IsError)
				return firstAverageValue;
			VariantValue secondAverageValue = GetAverage(secondArray, context);
			if (secondAverageValue.IsError)
				return secondAverageValue;
			VariantValue firstVarianceValue = GetVariance(firstArray, context);
			if (firstVarianceValue.IsError)
				return firstVarianceValue;
			VariantValue secondVarianceValue = GetVariance(secondArray, context);
			if (secondVarianceValue.IsError)
				return secondVarianceValue;
			double firstVariance = firstVarianceValue.NumericValue;
			double secondVariance = secondVarianceValue.NumericValue;
			if (firstVariance == 0 && secondVariance == 0)
				return VariantValue.ErrorDivisionByZero;
			double firstAverage = firstAverageValue.NumericValue;
			double secondAverage = secondAverageValue.NumericValue;
			if (type == 2)
				return GetResultForTwoSampleAssumingEqualVariances(firstCount, firstAverage, firstVariance, secondCount, secondAverage, secondVariance);
			return GetResultForTwoSampleAssumingUnequalVariances(firstCount, firstAverage, firstVariance, secondCount, secondAverage, secondVariance);
		}
		double GetResultForTwoSampleAssumingEqualVariances(double firstCount, double firstAverage, double firstVariance, double secondCount, double secondAverage, double secondVariance) {
			double count = firstCount * secondCount / (firstCount + secondCount);
			double average = Math.Abs(firstAverage - secondAverage);
			double degreeFreedom = firstCount + secondCount - 2;
			double variance = ((firstCount - 1) * firstVariance + (secondCount - 1) * secondVariance) / degreeFreedom;
			return GetTDotDist(count, average, variance, degreeFreedom);
		}
		double GetTDotDist(double count, double average, double variance, double degreeFreedom) {
			double tstat = average * Math.Sqrt(count / variance);
			return 1.0 - FunctionTDotDist.GetResult(degreeFreedom, tstat);
		}
		VariantValue GetResultForTwoSampleAssumingUnequalVariances(double firstCount, double firstAverage, double firstVariance, double secondCount, double secondAverage, double secondVariance) {
			double firstCoef = firstVariance / firstCount;
			double secondCoef = secondVariance / secondCount;
			double stat = (firstAverage - secondAverage) / Math.Sqrt(firstCoef + secondCoef);
			double degreeFreedom = (firstCoef + secondCoef) * (firstCoef + secondCoef) / (firstCoef * firstCoef / (firstCount - 1) + secondCoef * secondCoef / (secondCount - 1));
			return 0.5 * FunctionBetaDist.GetResult(0.5 * degreeFreedom, 0.5, degreeFreedom / (degreeFreedom + stat * stat));
		}
		VariantValue GetAverage(VariantValue value, WorkbookDataContext context) {
			return GetFunctionResult(value, context, 0x0005);
		}
	}
	#endregion
}
