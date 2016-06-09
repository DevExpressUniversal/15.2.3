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
	#region FunctionMirr
	public class FunctionMirr : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "MIRR"; } }
		public override int Code { get { return 0x003D; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue values = arguments[0];
			if (values.IsError)
				return values;
			if (values.IsText || values.IsEmpty || values.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue financeRate = arguments[1].ToNumeric(context);
			if (financeRate.IsError)
				return financeRate;
			VariantValue reinvestRate = arguments[2].ToNumeric(context);
			if (reinvestRate.IsError)
				return reinvestRate;
			FunctionMirrResult functionResult = CreateInitialFunctionResult(context) as FunctionMirrResult;
			functionResult.FinanceRate = financeRate.NumericValue;
			functionResult.ReinvestRate = reinvestRate.NumericValue;
			VariantValue result = GetResult(values, functionResult);
			if (reinvestRate == -1 && !result.IsError)
				return VariantValue.ErrorDivisionByZero;
			return result;
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionMirrResult(context);
		}
		VariantValue GetResult(VariantValue argument, FunctionMirrResult function) {
			if (argument.IsNumeric)
				return EvaluateValue(argument, function);
			if (argument.IsArray)
				return EvaluateArray(argument.ArrayValue, function);
			if (argument.IsCellRange)
				return EvaluateCellRange(argument.CellRangeValue, function);
			return VariantValue.ErrorInvalidValueInFunction;
		}
	}
	#endregion
	#region FunctionMirrResult
	public class FunctionMirrResult : FunctionSumResultBase {
		#region Fields
		double financeRate;
		double reinvestRate;
		double numberCount;
		double negativeNumberCount;
		double npvPositiveResult;
		double npvNegativeResult;
		#endregion
		public FunctionMirrResult(WorkbookDataContext context)
			: base(context) {
		}
		#region Properties
		internal double FinanceRate { set { financeRate = value; } }
		internal double ReinvestRate { set { reinvestRate = value; } }
		#endregion
		public override bool ProcessConvertedValue(VariantValue value) {
			double number = value.NumericValue;
			if (number > 0) 
				CalculateNPVPositive(number);
			if (number < 0)
				CalculateNPVNegative(number);
			if (Error.IsError)
				return false;
			numberCount++;
			return true;
		}
		void CalculateNPVPositive(double number) {
			if (reinvestRate != -1)
				npvPositiveResult += number / Math.Pow(1 + reinvestRate, numberCount);
		}
		void CalculateNPVNegative(double number) {
			if (financeRate == -1 && negativeNumberCount > 0) {
				Error = VariantValue.ErrorDivisionByZero;
				return;
			} 
			if (negativeNumberCount == 0) 
				npvNegativeResult = number;
			if (negativeNumberCount > 0)
				npvNegativeResult += number / Math.Pow(1 + financeRate, numberCount);
			negativeNumberCount++;
		}
		public override VariantValue GetFinalValue() {
			if (numberCount <= 1)
				return VariantValue.ErrorDivisionByZero;
			numberCount--;
			npvPositiveResult *= Math.Pow(1 + reinvestRate, numberCount); 
			npvNegativeResult = Math.Abs(npvNegativeResult);
			double result = Math.Pow(npvPositiveResult / npvNegativeResult, 1 / numberCount) - 1;
			return result;
		}
	}
	#endregion 
}
