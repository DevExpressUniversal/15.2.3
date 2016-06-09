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
using DevExpress.XtraSpreadsheet.Model.External;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionCountIf
	public class FunctionCountIf : FunctionCount {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "COUNTIF"; } }
		public override int Code { get { return 0x015A; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue conditionRange = arguments[0];
			if (conditionRange.IsError)
				return conditionRange;
			CellRangeBase conditionRangeBase = conditionRange.CellRangeValue;
			if (conditionRangeBase.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			if (conditionRangeBase.Worksheet is ExternalWorksheet)
				return VariantValue.ErrorInvalidValueInFunction;
			VariantValue condition = arguments[1];
			condition = context.DereferenceValue(condition, true);
			FunctionResult result = CreateInitialFunctionResult(context);
			result.AddCondition(conditionRangeBase, condition);
			return EvaluateCellRange(conditionRangeBase.GetFirstInnerCellRange(), result);
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			FunctionCountIfResult result = new FunctionCountIfResult(context);
			result.ProcessErrorValues = true;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}   
	#endregion
	public class FunctionCountIfResult : FunctionCountResultBase {
		double difference;
		public FunctionCountIfResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			return true;
		}
		public override void BeginArrayProcessingCore(long itemsCount) {
			difference = itemsCount;
		}
		public override VariantValue GetFinalValue() {
			ConditionCalculationResult calculateConditionsResult = CalculateConditions(String.Empty);
			if (calculateConditionsResult == ConditionCalculationResult.ErrorGettingData)
				return VariantValue.ErrorGettingData;
			if (calculateConditionsResult == ConditionCalculationResult.True)
				Count += difference;
			return Count;
		}
		public override ConditionCalculationResult CalculateConditions(int relativeRowIndex, int relativeColumnIndex) {
			if (ArrayOrRangeProcessing)
				--difference;
			return base.CalculateConditions(relativeRowIndex, relativeColumnIndex);
		}
	}
}
