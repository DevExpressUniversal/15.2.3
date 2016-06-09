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
	#region FunctionCalculationIfsBase (abstract class)
	public abstract class FunctionCalculationIfsBase : WorksheetGenericFunctionBase {
		protected abstract int CellRangeValueStartIndex { get; }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue targetRangeValue = GetTargetRange(arguments, context);
			if (targetRangeValue.IsError)
				return targetRangeValue;
			CellRangeBase targetRangeBase = targetRangeValue.CellRangeValue;
			if (targetRangeBase.Worksheet is ExternalWorksheet)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange targetCellRange = targetRangeBase.GetFirstInnerCellRange();
			FunctionResult result = CreateInitialFunctionResult(context);
			int width = targetCellRange.Width;
			int height = targetCellRange.Height;
			for (int i = CellRangeValueStartIndex; i < arguments.Count - CellRangeValueStartIndex; i += 2) {
				VariantValue value = GetCellRangeValue(arguments[i], context);
				if (value.IsError)
					return value;
				if (value.CellRangeValue.Worksheet is ExternalWorksheet)
					return VariantValue.ErrorInvalidValueInFunction;
				CellRange cellRangeValue = value.CellRangeValue.GetFirstInnerCellRange();
				if (cellRangeValue.Width != width || cellRangeValue.Height != height)
					return VariantValue.ErrorInvalidValueInFunction;
				VariantValue criteriaValue = arguments[i + 1];
				criteriaValue = context.DereferenceValue(criteriaValue, true);
				result.AddCondition(value.CellRangeValue, criteriaValue);
			}
			result.SortConditions();
			return EvaluateCellRange(targetCellRange, result);
		}
		protected VariantValue GetCellRangeValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (value.CellRangeValue.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			return value;
		}
		protected VariantValue GetTargetRange(IList<VariantValue> arguments, WorkbookDataContext context) {
			return GetCellRangeValue(arguments[0], context);
		}
	}
	#endregion
	#region FunctionCountIfs
	public class FunctionCountIfs : FunctionCalculationIfsBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "COUNTIFS"; } }
		public override int Code { get { return 0x01E1; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override int CellRangeValueStartIndex { get { return 0; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			FunctionCountIfsResult result = new FunctionCountIfsResult(context);
			result.ProcessErrorValues = true;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			for (int i = 0; i < 127; i++) {
				collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.NonRequired));
				collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			}
			return collection;
		}
	}
	#endregion
	#region FunctionCountIfsResult
	public class FunctionCountIfsResult : FunctionCountIfResult {
		public FunctionCountIfsResult(WorkbookDataContext context)
			: base(context) {
		}
	}
	#endregion
}
