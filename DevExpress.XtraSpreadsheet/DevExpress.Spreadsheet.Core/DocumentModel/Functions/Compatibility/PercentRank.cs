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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionPercentRank
	public class FunctionPercentRank : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "PERCENTRANK"; } }
		public override int Code { get { return 0x0149; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array = arguments[0];
			if (array.IsError)
				return array;
			if (!array.IsArray && !array.IsCellRange) {
				array = array.ToNumeric(context);
				if (array.IsError)
					return array;
			}
			VariantValue number = arguments[1].ToNumeric(context);
			if (number.IsError)
				return number;
			VariantValue significanceValue;
			int significance = 3;
			if (arguments.Count > 2 && !arguments[2].IsEmpty) {
				significanceValue = arguments[2].ToNumeric(context);
				if (significanceValue.IsError)
					return significanceValue;
				significance = (int)significanceValue.NumericValue;
			}
			if ((array.IsArray && array.ArrayValue.Count == 1 && array.ToNumeric(context) == number) || 
				(array.IsCellRange && array.CellRangeValue.CellCount == 1 && array.ToNumeric(context) == number) || 
				(array.IsNumeric && array.NumericValue == number))
				return 1;
			if (arguments[0].IsEmpty && number != 0)
				return VariantValue.ErrorValueNotAvailable;
			if (significance <= 0)
				return VariantValue.ErrorNumber;
			FunctionPercentRankResult result = CreateInitialFunctionResult(context) as FunctionPercentRankResult;
			result.Number = number.NumericValue;
			result.Significance = significance;
			if (array.IsArray)
				return EvaluateArray(array.ArrayValue, result);
			if (array.IsCellRange)
				return EvaluateCellRange(array.CellRangeValue, result);
			return VariantValue.ErrorInvalidValueInFunction;
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionPercentRankResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
	#region FunctionPercentRankResult
	public class FunctionPercentRankResult : FunctionSumResultBase {
		#region Fields
		const double eps = 0.000000000000001;
		int numberCount = 0;
		int significance;
		int lesserThanNumberCount = 0;
		bool arrayContainsNumber = false;
		bool firstLesser = true;
		bool firstGreater = true;
		double minLesserNumber, maxLesserNumber, minGreaterNumber, maxGreaterNumber;
		double number;
		#endregion
		public FunctionPercentRankResult(WorkbookDataContext context)
			: base(context) {
		}
		internal int Significance { get { return significance; } set { significance = value; } }
		internal double Number { get { return number; } set { number = value; } }
		protected bool ArrayContainsNumber { get { return arrayContainsNumber; } }
		protected int LesserThanNumberCount { get { return lesserThanNumberCount; } }
		protected int NumberCount { get { return numberCount; } }
		protected double MaxLesserNumber { get { return maxLesserNumber; } }
		protected double MinGreaterNumber { get { return minGreaterNumber; } }
		public override bool ProcessConvertedValue(VariantValue value) {
			numberCount++;
			double arrayValue = value.NumericValue;
			if (arrayValue < number) {
				maxLesserNumber = (firstLesser) ? arrayValue : Math.Max(maxLesserNumber, arrayValue);
				minLesserNumber = (firstLesser) ? arrayValue : Math.Min(minLesserNumber, arrayValue);
				firstLesser = false;
				lesserThanNumberCount++;
			}
			else if (arrayValue > number) {
				maxGreaterNumber = (firstGreater) ? arrayValue : Math.Max(maxGreaterNumber, arrayValue);
				minGreaterNumber = (firstGreater) ? arrayValue : Math.Min(minGreaterNumber, arrayValue);
				firstGreater = false;
			}
			else
				arrayContainsNumber = true;
			return true;
		}
		public override VariantValue GetFinalValue() {
			if (firstLesser && arrayContainsNumber)
				minLesserNumber = number;
			if (firstGreater && arrayContainsNumber)
				maxGreaterNumber = number;
			if (firstLesser && !arrayContainsNumber)
				minLesserNumber = minGreaterNumber;
			if (firstGreater && !arrayContainsNumber)
				maxGreaterNumber = maxLesserNumber;
			if (number > maxGreaterNumber || number < minLesserNumber || numberCount == 0)
				return VariantValue.ErrorValueNotAvailable;
			if (maxGreaterNumber == minLesserNumber || numberCount == 1)
				return 1.0;
			double result = GetResult();
			if (Math.Round(result, 15) - result <= eps)
				result = Math.Round(result, 15);
			return WorksheetFunctionBase.Truncate(result * Math.Pow(10, significance)) / Math.Pow(10, significance);
		}
		protected virtual double GetResult() {
			if (arrayContainsNumber)
				return (double)lesserThanNumberCount / (numberCount - 1);
			return (lesserThanNumberCount - 1 + (number - maxLesserNumber) / (minGreaterNumber - maxLesserNumber)) / (numberCount - 1);
		}
	}
	#endregion
}
