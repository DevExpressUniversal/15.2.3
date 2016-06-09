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
	#region WorksheetGenericAggregateFunctionBase (abstract class)
	public abstract class WorksheetGenericAggregateFunctionBase : WorksheetGenericFunctionBase {
		#region Properties
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected abstract int StartOperandIndex { get; }
		#endregion
		protected VariantValue GetFunctionNumber(VariantValue value, WorkbookDataContext context) {
			VariantValue numberValue = value.ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			int result = (int)numberValue.NumericValue;
			return CheckValidFunctionNumber(result) ? result : -1;
		}
		protected override ConditionCalculationResult ShouldProcessCell(ICellBase cell, FunctionResult result, int relativeRowIndex, int relativeColumnIndex) {
			ConditionCalculationResult shouldProcessCell = base.ShouldProcessCell(cell, result, relativeRowIndex, relativeColumnIndex);
			if (shouldProcessCell != ConditionCalculationResult.True)
				return shouldProcessCell;
			if (result.IgnoredHiddenRows && ((Worksheet)cell.Sheet).Rows[cell.RowIndex].IsHidden)
				return ConditionCalculationResult.False;
			ICell workbookCell = cell as ICell;
			if (workbookCell == null || !workbookCell.HasFormula)
				return ConditionCalculationResult.True;
			ContainsFunctionsPredicateBase containsFunctionsPredicate = result.ContainsFunctionsPredicate;
			if (containsFunctionsPredicate != null) { 
				ParsedExpression expression = workbookCell.GetFormula().Expression;
				ConditionCalculationResult conditionCalculationResult = containsFunctionsPredicate.Calculate(expression);
				if (containsFunctionsPredicate.Error.IsError)
					result.Error = containsFunctionsPredicate.Error;
				return conditionCalculationResult; 
			}
			return ConditionCalculationResult.True;
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return null;
		}
		protected override bool ProcessValuesCore(IList<VariantValue> operands, int startOperandIndex, FunctionResult result) {
			return base.ProcessValuesCore(operands, StartOperandIndex, result);
		}
		protected VariantValue GetResultCore(IList<VariantValue> arguments, WorkbookDataContext context, FunctionResult functionResult) {
			ProcessExpressions(arguments, context, functionResult);
			VariantValue error = functionResult.Error;
			return error.IsError ? error : functionResult.GetFinalValue();
		}
		protected abstract FunctionResult CreateInstanceFunctionResult(int functionNumber, WorkbookDataContext context);
		protected abstract bool CheckValidFunctionNumber(int functionNumber);
		protected abstract bool GetIgnoredHiddenRows(int number);
	}
	#endregion
	#region ContainsFunctionsPredicateBase (absract class)
	public abstract class ContainsFunctionsPredicateBase : ParsedThingVisitor {
		#region Fields
		ConditionCalculationResult result;
		VariantValue error;
		#endregion
		protected internal ConditionCalculationResult Result { get { return result; } protected set { result = value; } }
		protected internal VariantValue Error { get { return error; } protected set { error = value; } }
		protected internal ConditionCalculationResult Calculate(ParsedExpression expression) {
			SetFunctionCodes();
			result = ConditionCalculationResult.True;
			Process(expression);
			return result;
		}
		public override void Visit(ParsedThingFuncVar expression) {
			if (CheckFunctionCodes(expression))
				result = ConditionCalculationResult.False;
			base.Visit(expression);
		}
		protected abstract void SetFunctionCodes();
		protected abstract bool CheckFunctionCodes(ParsedThingFuncVar expression);
	}
	#endregion
	#region FunctionSubtotal
	public class FunctionSubtotal : WorksheetGenericAggregateFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "SUBTOTAL"; } }
		public override int Code { get { return 0x0158; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override int StartOperandIndex { get { return 1; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = GetFunctionNumber(arguments[0], context);
			if (value.IsError) 
				return value;
			int functionNumber = (int)value.NumericValue;
			if (functionNumber == -1)
				return VariantValue.ErrorInvalidValueInFunction;
			bool ignoredHiddenRows = GetIgnoredHiddenRows(functionNumber);
			if (ignoredHiddenRows)
				functionNumber -= 100;
			FunctionResult functionResult = CreateFunctionResult(context, functionNumber, ignoredHiddenRows);
			return GetResultCore(arguments, context, functionResult);
		}
		protected FunctionResult CreateFunctionResult(WorkbookDataContext context, int functionNumber, bool ignoredHiddenRows) {
			FunctionResult result = CreateInstanceFunctionResult(functionNumber, context);
			result.IgnoredHiddenRows = ignoredHiddenRows;
			result.ContainsFunctionsPredicate = new ContainsSubtotalFunctionPredicate();
			return result;
		}
		protected override FunctionResult CreateInstanceFunctionResult(int functionNumber, WorkbookDataContext context) {
			switch (functionNumber) {
				case 1: return new FunctionAverageResult(context);
				case 2:
					FunctionCountResult functionCountResult = new FunctionCountResult(context);
					functionCountResult.ProcessErrorValues = true;
					return functionCountResult;
				case 3:
					FunctionCountAResult functionCountAResult = new FunctionCountAResult(context);
					functionCountAResult.ProcessErrorValues = true;
					return functionCountAResult;
				case 4: return new FunctionMaxResult(context);
				case 5: return new FunctionMinResult(context);
				case 6: return new FunctionProductResult(context);
				case 7: return new FunctionStdevResult(context);
				case 8: return new FunctionStdevPResult(context);
				case 9: return new FunctionSumResult(context);
				case 10: return new FunctionVarResult(context);
				case 11: return new FunctionVarPResult(context); 
				default: return null;
			}
		}
		protected override bool GetIgnoredHiddenRows(int number) {
			return number > 100;
		} 
		protected override bool CheckValidFunctionNumber(int functionNumber) {
			return (functionNumber > 0 && functionNumber < 12) || (functionNumber > 100 && functionNumber < 112);
		}
	}
	#endregion
	#region ContainsSubtotalFunctionPredicate
	public class ContainsSubtotalFunctionPredicate : ContainsFunctionsPredicateBase {
		int subtotalCode;
		protected override void SetFunctionCodes() {
			subtotalCode = FormulaCalculator.FuncSubtotalCode;
		}
		protected override bool CheckFunctionCodes(ParsedThingFuncVar expression) {
			return expression.FuncCode == subtotalCode;
		}
	}
	#endregion
}
