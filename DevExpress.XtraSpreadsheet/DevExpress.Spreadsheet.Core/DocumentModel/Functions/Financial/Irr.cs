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
	#region FunctionIrr
	public class FunctionIrr : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		public override string Name { get { return "IRR"; } }
		public override int Code { get { return 0x003E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if (value.IsError)
				return value;
			if (value.IsNumeric)
				return VariantValue.ErrorNumber;
			if (!value.IsArray && !value.IsCellRange)
				return VariantValue.ErrorInvalidValueInFunction;
			else {
				IVector<VariantValue> values;
				if (value.IsArray)
					values = new ArrayZVector(value.ArrayValue);
				else
					values = new RangeZNVector(value.CellRangeValue);
				double guess = 0.1;
				if (arguments.Count == 2) {
					value = arguments[1].ToNumeric(context);
					if (value.IsError)
						return value;
					guess = value.NumericValue;
				}
				List<double> listValues = new List<double>();
				int count = values.Count;
				for (int i = 0; i < count; i++) {
					VariantValue currentValue = values[i];
					if (currentValue == VariantValue.ErrorGettingData)
						return currentValue;
					if (currentValue.IsError)
						return VariantValue.ErrorInvalidValueInFunction;
					if(currentValue.IsNumeric)
						listValues.Add(currentValue.NumericValue);
				}
				VariantValue result = GetNumericResult(listValues, guess);
				if (result.IsError)
					return result;
				if (result.IsText)
					result = GetNumericResultForSpecialCase(listValues, 0);
				if (result.IsText)
					result = GetNumericResultForSpecialCase(listValues, -0.9);
				if (result.IsText)
					return VariantValue.ErrorNumber;
				return result;
			}
		}
		VariantValue GetNumericResult(IList<double> values, double guess) {
			int maxIteration = 20;
			double precision = 1e-15;
			double currentApproximationRate = guess;
			int numberIteration = 0;
			while (numberIteration < maxIteration) {
				VariantValue value = GetResultNewtonIteration(values, currentApproximationRate);
				if (value.IsError || value.IsText)
					return value;
				double nextApproximationRate = value.NumericValue;
				if (Math.Abs(nextApproximationRate - currentApproximationRate) < precision)
					return Math.Round((nextApproximationRate + currentApproximationRate) / 2, 15);
				currentApproximationRate = nextApproximationRate;
				numberIteration++;
			}
			return String.Empty;
		}
		VariantValue GetNumericResultForSpecialCase(IList<double> values, double guess) {
			double delta = 0.001;
			double precision = 1e-06;
			int maxIteration = 1000;
			double rate1 = guess;
			double rate2 = rate1 + delta;
			for (int i = 0; i < maxIteration; i++) {
				double npvValue1 = GetNPV(values, rate1);
				double npvValue2 = GetNPV(values, rate2);
				if (Math.Abs(npvValue1) < precision)
					return rate1;
				if (Math.Abs(npvValue2) < precision)
					return rate2;
				if (Math.Sign(npvValue1) != Math.Sign(npvValue2))
					return GetResultWithBisectionMethod(values, rate1, rate2);
				if (npvValue1 > 0) {
					if (npvValue2 < npvValue1) {
						rate1 = rate2;
						rate2 = rate1 + delta;
					}
					else {
						rate2 = rate1;
						rate1 = rate2 - delta;
					}
				}
				else {
					if (npvValue2 > npvValue1) {
						rate1 = rate2;
						rate2 = rate1 + delta;
					}
					else {
						rate2 = rate1;
						rate1 = rate2 - delta;
					}
				}
			}
			return String.Empty;
		}
		VariantValue GetResultWithBisectionMethod(IList<double> values, double rate1, double rate2) {
			double precision = 1e-06;
			int maxIteration = 20;
			double minRate = Math.Min(rate1, rate2);
			double maxRate = Math.Max(rate1, rate2);
			for (int i = 0; i < maxIteration; i++) {
				double result = (minRate + maxRate) / 2;
				if (Math.Abs(maxRate - minRate) < precision)
					return result;
				if (Math.Sign(GetNPV(values, minRate)) != Math.Sign(GetNPV(values, result)))
					maxRate = result;
				else
					minRate = result;
			}
			return VariantValue.ErrorNumber;
		}
		double GetNPV(IList<double> values, double rate) {
			double result = 0;
			double powerRate = 1;
			int count = values.Count;
			for (int i = 0; i < count; i++) {
				result += values[i] / powerRate;
				powerRate *= 1 + rate;
			}
			return result;
		}
		VariantValue GetResultNewtonIteration(IList<double> values, double rate) {
			if (rate == -1)
				return VariantValue.ErrorInvalidValueInFunction;
			double benefit = 0;
			double cost = 0;
			double resultDiffNPV = 0;
			double result2DiffNPV = 0;
			double powerRate = 1;
			double countPower = 0;
			for (int i = 0; i < values.Count; i++) {
				double number = values[i];
					if (number >= 0)
						benefit += number / powerRate;
					else
						cost += number / powerRate;
					if (i > 0) {
						resultDiffNPV += -countPower * number / powerRate / (1 + rate);
						result2DiffNPV += -(countPower + 1) * resultDiffNPV / (1 + rate);
					}
					if (Math.Pow(1 + rate, countPower + 1) > double.MaxValue)
						return String.Empty;
					powerRate *= 1 + rate;
					countPower += 1;
			}
			if (benefit == 0 || cost == 0)
				return VariantValue.ErrorNumber;
			double result = rate - (benefit + cost) / resultDiffNPV;
			if (Math.Abs(result * result2DiffNPV / (resultDiffNPV * resultDiffNPV)) > 1)
				return String.Empty;
			return result;
		}
	}
	#endregion
}
