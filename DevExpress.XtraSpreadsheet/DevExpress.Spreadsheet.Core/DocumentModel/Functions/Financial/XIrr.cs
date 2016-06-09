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
	#region FunctionXIrr
	public class FunctionXIrr : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "XIRR"; } }
		public override int Code { get { return 0x01AD; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			double guess = 0.1;
			if (arguments.Count == 3) {
				VariantValue valueGuess = arguments[2];
				if (valueGuess.IsBoolean || (valueGuess.IsCellRange && valueGuess.CellRangeValue.CellCount > 1))
					return VariantValue.ErrorInvalidValueInFunction;
				valueGuess = valueGuess.ToNumeric(context);
				if (valueGuess.IsError)
					return valueGuess;
				guess = valueGuess.NumericValue;
			}
			VariantValue value = arguments[0];
			if (value.IsError)
				return value;
			if (value.IsCellRange && (value.CellRangeValue.RangeType == CellRangeType.UnionRange))
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue valueDate = arguments[1];
			if (valueDate.IsError)
				return valueDate;
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (valueDate.IsCellRange && valueDate.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			IVector<VariantValue> valuesVector = null;
			int valuesVectorCount = 1;
			if (value.IsArray || value.IsCellRange) {
				valuesVector = GetVector(value);
				valuesVectorCount = valuesVector.Count;
			}
			if (valueDate.IsEmpty) {
				if (value.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				if (valuesVectorCount == 1) {
					value = value.ToNumeric(context);
					if (value.IsNumeric)
						return VariantValue.ErrorValueNotAvailable;
					return VariantValue.ErrorInvalidValueInFunction;
				}
				return VariantValue.ErrorValueNotAvailable;
			}
			IVector<VariantValue> datesVector = null;
			int datesVectorCount = 1;
			if (valueDate.IsArray || valueDate.IsCellRange) {
				datesVector = GetVector(valueDate);
				datesVectorCount = datesVector.Count; 
			}
			if (valuesVector == null && datesVector == null) {
				if (value.IsBoolean || valueDate.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				value = value.ToNumeric(context);
				valueDate = valueDate.ToNumeric(context);
				if (value.IsNumeric && valueDate.IsNumeric)
					return VariantValue.ErrorValueNotAvailable;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			if (valuesVector == null) {
				if (value.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				if (datesVectorCount == 1) {
					valueDate = valueDate.ToNumeric(context);
					if (valueDate.IsNumeric)
						return VariantValue.ErrorValueNotAvailable;
					return VariantValue.ErrorInvalidValueInFunction;
				}
				value = value.ToNumeric(context);
				if (value.IsNumeric)
					return VariantValue.ErrorNumber;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			if (datesVector == null) {
				if (valueDate.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				if (valuesVectorCount == 1) {
					value = value.ToNumeric(context);
					if (value.IsNumeric)
						return VariantValue.ErrorValueNotAvailable;
					return VariantValue.ErrorInvalidValueInFunction;
				}
				valueDate = valueDate.ToNumeric(context);
				if (valueDate.IsNumeric)
					return VariantValue.ErrorNumber;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			if (valuesVectorCount == 1 && datesVectorCount == 1)
				return VariantValue.ErrorValueNotAvailable;
			if (valuesVectorCount != datesVectorCount)
				return VariantValue.ErrorNumber;
			List<Tuple<double, double>> calculationArguments = new List<Tuple<double, double>>();					   
			for (int i = 0; i < valuesVectorCount; i++) {
				VariantValue currentValue = valuesVector[i];
				VariantValue currentData = datesVector[i];
				if (currentValue == VariantValue.ErrorGettingData)
					return currentValue;
				if (currentData == VariantValue.ErrorGettingData)
					return currentData;
				if (currentValue.IsError)
					return currentValue;
				if (currentData.IsError)
					return currentData;
				if (currentValue.IsBoolean || currentData.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
				currentValue = currentValue.ToNumeric(context);
				currentData = currentData.ToNumeric(context);
				if (currentValue.IsNumeric && currentData.IsNumeric) {
					if (currentData.NumericValue < 0)
						return VariantValue.ErrorNumber;
					calculationArguments.Add(new Tuple<double, double>(currentValue.NumericValue, currentData.NumericValue));
				}
				else return VariantValue.ErrorInvalidValueInFunction;
			}
			VariantValue result;
			result = CheckValues(calculationArguments);
			if (result.IsError)
				return result;
			result = CalculateRateForLinearSearch(calculationArguments, -0.999999999999999, 1, 1e-3);
			if (!result.IsEmpty)
				result = CalculateRateForBisection(calculationArguments, -0.999999999999999, 1, 0);
			else {
				result = CalculateRateForNewton(calculationArguments, guess);
				if (result.IsError)
					return result;
			}
			if (result.IsEmpty)
				return VariantValue.ErrorNumber; 
			return result.NumericValue;
		}
		IVector<VariantValue> GetVector(VariantValue value) {
			if (value.IsArray)
				return new ArrayZVector(value.ArrayValue);
			return new RangeZVector(value.CellRangeValue.GetFirstInnerCellRange());		  
		}
		VariantValue CheckValues(List<Tuple<double, double>> calculationArguments) {
			double benefit=0;
			double cost=0;	 
			for (int i = 0; i < calculationArguments.Count; i++) {
				if (calculationArguments[i].Item1 >= 0)
					benefit += calculationArguments[i].Item1;
				else
					cost += calculationArguments[i].Item1;
			}
			if (benefit == 0 || cost == 0)
				return VariantValue.ErrorNumber;
			return 0;
		}
		VariantValue CalculateRateForBisection(List<Tuple<double, double>> calculationArguments, double rate1,double rate2,int iteration){
			int maxIteration = 100;		   
			double precision = 1e-15;
			double middleRate;
			double npvStart;
			double npvEnd;
			double npvMiddle;
			VariantValue result;
			while (iteration < maxIteration) {
				middleRate = (rate1 + rate2) / 2;
				result = Npv(calculationArguments, rate1);
				if (result.IsEmpty)
					return result;
				npvStart = result.NumericValue;
				result = Npv(calculationArguments, rate2);
				if (result.IsEmpty)
					return result;
				npvEnd = result.NumericValue;
				if (Math.Abs(rate1 - rate2) < precision)
					if (npvStart * npvEnd < 0)
						return middleRate;
					else
						return VariantValue.Empty;
				result = Npv(calculationArguments, middleRate);
				if (result.IsEmpty)
					return result;
				npvMiddle = result.NumericValue;
				if (npvStart * npvMiddle < 0)
					rate2 = middleRate;
				else
					rate1 = middleRate;
			}
			return VariantValue.Empty;
		}
		VariantValue CalculateRateForLinearSearch(List<Tuple<double, double>> calculationArguments, double leftGuess, double rigthGuess, double delta) {
			double currentRate = leftGuess;		   
			VariantValue npvValue = Npv(calculationArguments, currentRate);
			if (npvValue.IsEmpty) 
				return npvValue;
			double npvStart = npvValue.NumericValue;
			while (currentRate <= rigthGuess) {
				npvValue = Npv(calculationArguments, currentRate + delta);
				if (npvValue.IsEmpty)
					return npvValue;
				double npvEnd = npvValue.NumericValue;
				if (npvStart * npvEnd < 0)
					return (currentRate + delta / 2);
				currentRate += delta;
				npvStart = npvEnd;
			}			   
			return VariantValue.Empty;
		}
		VariantValue CalculateRateForNewton(List<Tuple<double, double>> calculationArguments, double guess) {
			int maxIteration = 100;
			double precision = 1e-15;
			double currentRate = guess;
			double nextRate;
			int numberIteration = 0;			
			double currentData;
			double previsiousData ;
			double firstData = calculationArguments[0].Item2;			
			double resultDiffNPV; 
			double result2DiffNPV;
			double elementNPV;
			double elementDiffNPV;
			double element2DiffNPV;
			double powerRate ;			
			double benefit;
			double cost;			
			while (numberIteration < maxIteration) {
				previsiousData = calculationArguments[0].Item2;
				resultDiffNPV = 0;
				result2DiffNPV = 0;
				benefit=0;
				cost = 0;
				powerRate = 1;
				double power;
				for (int i = 0; i < calculationArguments.Count; i++) {
					double number = calculationArguments[i].Item1;
					currentData = calculationArguments[i].Item2;
					power = (currentData - firstData) / 365;
					powerRate = Math.Pow(Math.Abs(1 + currentRate), power);
					if (double.IsInfinity(powerRate) || double.IsNaN(powerRate))
						return VariantValue.Empty;
					if (1 + currentRate < 0 && ((currentData - firstData) % 2) != 0)
						powerRate *= -1;										   
					elementNPV = number / powerRate;					
					if (number >= 0)
						benefit += elementNPV;
					else
						cost += elementNPV;
					if (i > 0) {
						elementDiffNPV = -elementNPV * power / (1 + currentRate);
						resultDiffNPV += elementDiffNPV;
						element2DiffNPV = -elementDiffNPV * (power + 1) / (1 + currentRate);
						result2DiffNPV += element2DiffNPV;
					}										
					previsiousData = currentData;				   
				}	   
				nextRate = currentRate - (benefit + cost) / resultDiffNPV;
				if (Math.Abs(nextRate - currentRate) < precision) {
					return nextRate;
				}
				currentRate = nextRate;
				numberIteration++;
			}
			return VariantValue.Empty;
		}
		VariantValue Npv(List<Tuple<double, double>> calculationArguments, double rate) {
			double npv = calculationArguments[0].Item1;			
			double powerRate = 1;
			double firstData=calculationArguments[0].Item2;
			for (int i = 1; i < calculationArguments.Count; i++) {								
				powerRate = Math.Pow(Math.Abs(1 + rate), (calculationArguments[i].Item2 - firstData) / 365);
				if (double.IsInfinity(powerRate) || double.IsNaN(powerRate))
					return VariantValue.Empty;
				if (1 + rate < 0 && ((calculationArguments[i].Item2 - firstData) % 2) != 0)
					powerRate *= -1;
				npv += calculationArguments[i].Item1 / powerRate;
			}
			return npv;
		}
	}
	#endregion
}
