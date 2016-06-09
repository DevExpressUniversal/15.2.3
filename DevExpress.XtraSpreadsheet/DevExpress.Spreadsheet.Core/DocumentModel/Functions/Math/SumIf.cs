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
	#region FunctionSumIf
	public class FunctionSumIf : FunctionSum {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUMIF"; } }
		public override int Code { get { return 0x0159; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue conditionRangeValue = arguments[0];
			if (conditionRangeValue.IsError)
				return conditionRangeValue;
			CellRangeBase conditionRangeBase = conditionRangeValue.CellRangeValue;
			if (conditionRangeBase.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			if (conditionRangeBase.Worksheet == null || conditionRangeBase.Worksheet is ExternalWorksheet)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange conditionRange = conditionRangeBase.GetFirstInnerCellRange();
			VariantValue condition = arguments[1];
			condition = context.DereferenceValue(condition, true);
			CellRange resultRange;
			if (arguments.Count == 3) {
				VariantValue resultRangeValue = arguments[2];
				resultRangeValue = ValidateResultRange(resultRangeValue, conditionRangeBase);
				if (resultRangeValue.IsError)
					return resultRangeValue;
				resultRange = resultRangeValue.CellRangeValue.GetFirstInnerCellRange();
			}
			else
				resultRange = conditionRange;
			FunctionResult result = CreateInitialFunctionResult(context);
			result.AddCondition(conditionRange, condition);
			return EvaluateCellRange(resultRange, result);
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			FunctionSumResult result = new FunctionSumResult(context);
			result.ProcessErrorValues = true;
			return result;
		}
		VariantValue ValidateResultRange(VariantValue resultRange, CellRangeBase conditionRange) {
			if (resultRange.IsError)
				return resultRange;
			CellRangeBase rangeBase = resultRange.CellRangeValue;
			if (rangeBase.RangeType == CellRangeType.UnionRange)
				return VariantValue.ErrorInvalidValueInFunction;
			CellRange range = rangeBase.GetFirstInnerCellRange();
			CellRange conditionCellRange = conditionRange.GetFirstInnerCellRange();
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			resultRange.CellRangeValue = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(topLeft.Column + conditionCellRange.Width - 1, topLeft.Row + conditionCellRange.Height - 1, bottomRight.ColumnType, bottomRight.RowType)); ;
			return resultRange;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
