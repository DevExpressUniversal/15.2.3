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
	#region WorksheetGenericFunctionBase (abstract class)
	public abstract class WorksheetGenericFunctionBase : WorksheetFunctionBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			FunctionResult result = CreateInitialFunctionResult(context);
			ProcessExpressions(arguments, context, result);
			if (!result.Error.IsEmpty)
				return result.Error;
			return result.GetFinalValue();
		}
		protected VariantValue EvaluateCellRange(CellRangeBase range, FunctionResult result) {
			ProcessCellRangeValues(range, result);
			if (!result.Error.IsEmpty)
				return result.Error;
			return result.GetFinalValue();
		}
		protected VariantValue EvaluateValue(VariantValue value, FunctionResult result) {
			ProcessValue(value, result);
			if (!result.Error.IsEmpty)
				return result.Error;
			return result.GetFinalValue();
		}
		protected VariantValue EvaluateArray(IVariantArray value, FunctionResult result) {
			ProcessArrayValues(value, result);
			if (!result.Error.IsEmpty)
				return result.Error;
			return result.GetFinalValue();
		}
		protected abstract FunctionResult CreateInitialFunctionResult(WorkbookDataContext context);
		protected virtual bool ProcessExpressions(IList<VariantValue> arguments, WorkbookDataContext context, FunctionResult result) {
			if (arguments.Count <= 0)
				return false;
			return ProcessValuesCore(arguments, 0, result);
		}
		protected virtual bool ProcessValuesCore(IList<VariantValue> operands, int startOperandIndex, FunctionResult result) {
			for (int i = startOperandIndex; i < operands.Count; i++)
				if (!PerformProcessValue(operands[i], result))
					return false;
			return true;
		}
		protected bool ProcessCellRangeValues(CellRangeBase range, FunctionResult result) {
			if (result.Error.IsError)
				return true;
			if (range.RangeType != CellRangeType.UnionRange && range.Worksheet == null) {
				result.Error = VariantValue.ErrorReference;
				return false;
			}
			bool continueProcessing = ProcessCellRangeValuesCore(range, result);
			if (!result.EndCellRangeProcessing())
				return false;
			else
				return continueProcessing;
		}
		protected virtual bool ProcessCellRangeValuesCore(CellRangeBase range, FunctionResult result) {
			bool continueProcessing = true;
			result.BeginCellRangeProcessing(range.CellCount);
			int nonEmptyCellCount = 0;
			CellPosition rangeTopLeft = range.TopLeft;
			int topLeftColumn = rangeTopLeft.Column;
			int topLeftRow = rangeTopLeft.Row;
			CalculationChain chain = range.GetFirstInnerCellRange().Worksheet.Workbook.CalculationChain;
			foreach (ICellBase cell in range.GetExistingCellsEnumerable()) {
				nonEmptyCellCount++;
				ConditionCalculationResult shouldProcessCell = ShouldProcessCell(cell, result, cell.RowIndex - topLeftRow, cell.ColumnIndex - topLeftColumn);
				if (shouldProcessCell == ConditionCalculationResult.ErrorGettingData) {
					result.Error = VariantValue.ErrorGettingData;
					continueProcessing = false;
					break;
				}
				if (shouldProcessCell == ConditionCalculationResult.True) {
					VariantValue cellValue;
					if (!chain.TryGetCalculatedValue(cell, out cellValue)) {
						result.Error = cellValue;
						continueProcessing = false;
						continue;
					}
					if (cellValue.IsError && !result.ProcessErrorValues) {
						result.Error = cellValue;
						break;
					}
					if (!PerformProcessValue(cellValue, result)) {
						continueProcessing = false;
						break;
					}
				}
			}
			if (continueProcessing && nonEmptyCellCount <= 0 && range.Worksheet is ExternalWorksheet) {
				result.Error = VariantValue.ErrorReference;
				continueProcessing = false;
			}
			return continueProcessing;
		}
		protected bool ProcessArrayValues(IVariantArray array, FunctionResult result) {
			if (result.Error.IsError)
				return true;
			result.BeginArrayProcessing(array.Count);
			int height = array.Height;
			int width = array.Width;
			bool continueProcessing = true;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					ConditionCalculationResult calculatedConditions = result.CalculateConditions(y, x);
					if (calculatedConditions == ConditionCalculationResult.ErrorGettingData) {
						result.Error = VariantValue.ErrorGettingData;
						continueProcessing = false;
						break;
					}
					if (calculatedConditions == ConditionCalculationResult.False)
						continue;
					VariantValue curValue = array.GetValue(y, x);
					if (curValue.IsError && !result.ProcessErrorValues) {
						result.Error = curValue;
						break;
					}
					else if (!PerformProcessValue(curValue, result)) {
						continueProcessing = false;
						break;
					}
				}
			}
			if (!result.EndArrayProcessing())
				return false;
			else
				return continueProcessing;
		}
		protected bool PerformProcessValue(VariantValue value, FunctionResult result) {
			if (result.ShouldProcessValueCore(value))
				return ProcessValue(value, result);
			return true;
		}
		bool ProcessValue(VariantValue value, FunctionResult result) {
			if (value.IsCellRange)
				return ProcessCellRangeValues(value.CellRangeValue, result);
			else if (value.IsArray)
				return ProcessArrayValues(value.ArrayValue, result);
			else if (value.IsError)
				return result.ProcessErrorValue(value);
			else
				return result.ProcessSingleValue(value);
		}
		protected virtual ConditionCalculationResult ShouldProcessCell(ICellBase cell, FunctionResult result, int relativeRowIndex, int relativeColumnIndex) {
			return result.CalculateConditions(relativeRowIndex, relativeColumnIndex);
		}
	}
	#endregion
}
