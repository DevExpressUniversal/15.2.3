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
	#region FunctionAggregate
	public class FunctionAggregate : WorksheetGenericAggregateFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited, FakeParameterType.Number));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "AGGREGATE"; } }
		public override int Code { get { return 0x406E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override int StartOperandIndex { get { return 2; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue functionNumberValue = GetFunctionNumber(arguments[0], context);
			if (functionNumberValue.IsError)
				return functionNumberValue;
			int functionNumber = (int)functionNumberValue.NumericValue;
			VariantValue optionsValue = GetOptionsNumber(arguments[1], context);
			if (optionsValue.IsError)
				return optionsValue;
			int optionsNumber = (int)optionsValue.NumericValue;
			if (functionNumber == -1 || optionsNumber == -1)
				return VariantValue.ErrorInvalidValueInFunction;
			FunctionResult functionResult = CreateFunctionResult(context, functionNumber, optionsNumber);
			return GetResult(arguments, context, functionResult, functionNumber < 14);
		}
		protected override FunctionResult CreateInstanceFunctionResult(int functionNumber, WorkbookDataContext context) {
			switch (functionNumber) {
			case 1: return new FunctionAverageResult(context);
			case 2: return new FunctionCountResult(context);
			case 3: return new FunctionCountAResult(context);
			case 4: return new FunctionMaxResult(context);
			case 5: return new FunctionMinResult(context);
			case 6: return new FunctionProductResult(context);
			case 7: return new FunctionStdevResult(context);
			case 8: return new FunctionStdevPResult(context);
			case 9: return new FunctionSumResult(context);
			case 10: return new FunctionVarResult(context);
			case 11: return new FunctionVarPResult(context);
			case 12: return new FunctionMedianResult(context);
			case 13: return new FunctionModeResult(context);
			case 16: return new FunctionPercentileResult(context);
			case 17: return new FunctionQuartileResult(context);
			case 18: return new FunctionPercentileExcResult(context);
			case 19: return new FunctionQuartileExcResult(context);
			default: return null;
			}
		}
		protected override bool CheckValidFunctionNumber(int functionNumber) {
			return functionNumber > 0 && functionNumber < 20;
		}
		protected override bool GetIgnoredHiddenRows(int number) {
			return number % 2 != 0;
		}
		VariantValue GetOptionsNumber(VariantValue value, WorkbookDataContext context) {
			VariantValue numberValue = value.ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			int result = (int)numberValue.NumericValue;
			return CheckValidOptionsNumber(result) ? result : -1;
		}
		bool CheckValidOptionsNumber(int optionsNumber) {
			return optionsNumber >= 0 && optionsNumber < 8;
		}
		FunctionResult CreateFunctionResult(WorkbookDataContext context, int functionNumber, int optionsNumber) {
			FunctionResult result = CreateInstanceFunctionResult(functionNumber, context);
			result.IgnoredHiddenRows = GetIgnoredHiddenRows(optionsNumber);
			result.ProcessErrorValues = GetIgnoredErrors(optionsNumber);
			result.ContainsFunctionsPredicate = GetPredicate(optionsNumber); 
			return result;
		}
		bool GetIgnoredErrors(int number) {
			return number % 2 == 0;
		}
		ContainsFunctionsPredicateBase GetPredicate(int optionsNumber) {
			if (optionsNumber >= 0 && optionsNumber < 4)
				return new ContainsSubtotalOrAggregateFunctionPredicate();
			return null;
		}
		VariantValue GetResult(IList<VariantValue> arguments, WorkbookDataContext context, FunctionResult functionResult, bool isFirstPattern) {
			if (isFirstPattern) 
				return GetResultCore(arguments, context, functionResult);
			return GetResultForSecondPattern(arguments, context, functionResult);
		}
		VariantValue GetResultForSecondPattern(IList<VariantValue> arguments, WorkbookDataContext context, FunctionResult functionResult) {
			if (arguments.Count > StartOperandIndex + 2)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue array = arguments[StartOperandIndex];
			if (array.IsError)
				return array;
			VariantValue numberValue = arguments[StartOperandIndex + 1].ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			return EvaluateValue(array, functionResult);
		}
	}
	#endregion
	#region ContainsSubtotalOrAggregateFunctionPredicate
	public class ContainsSubtotalOrAggregateFunctionPredicate : ContainsFunctionsPredicateBase {
		int subtotalCode;
		int aggregateCode;
		protected override void SetFunctionCodes() {
			subtotalCode = FormulaCalculator.FuncSubtotalCode;
			aggregateCode = FormulaCalculator.FuncAggregateCode;
		}
		protected override bool CheckFunctionCodes(ParsedThingFuncVar expression) {
			int expressionFuncCode = expression.FuncCode;
			return expressionFuncCode == subtotalCode || expressionFuncCode == aggregateCode;
		}
		public override void Visit(ParsedThingArea3d thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingErr3d thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingNameX thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingRef3d thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingTableExt thing) {
			base.Visit(thing);
			SetError();
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			base.Visit(thing);
			SetError();
		}
		void SetError() {
			Error = VariantValue.ErrorInvalidValueInFunction;
		}
	}
	#endregion
}
