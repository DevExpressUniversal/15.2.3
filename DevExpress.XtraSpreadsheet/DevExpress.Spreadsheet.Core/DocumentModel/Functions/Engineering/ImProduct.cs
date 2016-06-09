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
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionImProduct
	public class FunctionImProduct : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "IMPRODUCT"; } }
		public override int Code { get { return 0x019D; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionImProductResult(context); 
		}
		protected override bool ProcessExpressions(System.Collections.Generic.IList<VariantValue> arguments, WorkbookDataContext context, FunctionResult result) {
			if (arguments[0].IsEmpty) {
				result.Error = VariantValue.ErrorValueNotAvailable;
				return false;
			}
			return base.ProcessExpressions(arguments, context, result);
		}
		protected override bool ProcessCellRangeValuesCore(CellRangeBase range, FunctionResult result) {
			bool continueProcessing = true;
			int existingCellsCount = 0;
			result.BeginArrayProcessing(range.CellCount);
			CellPosition rangeTopLeft = range.TopLeft;
			int topLeftColumn = rangeTopLeft.Column;
			int topLeftRow = rangeTopLeft.Row;
			CalculationChain chain = range.GetFirstInnerCellRange().Worksheet.Workbook.CalculationChain;
			foreach (ICellBase cell in range.GetExistingCellsEnumerable()) {
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
					if (!PerformProcessValue(cell.Value, result)) {
						continueProcessing = false;
						break;
					}
				}
				existingCellsCount++;
			}
			long emptyCellsCount = range.CellCount - existingCellsCount;
			if (emptyCellsCount > 0)
				result.ProcessSingleValue(0);
			return continueProcessing;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
	#region FunctionImProductResult
	public class FunctionImProductResult : FunctionMultiArgImResult {
		public FunctionImProductResult(WorkbookDataContext context)
			: base(context) {
				SetResultValue(1);
		}
		protected override bool PerformAction(Utils.SpreadsheetComplex argument) {
			SetResultValue(Complex.Multiply(Result.Value, argument.Value));
			return true;
		}
		protected override TriStateFlowControlValue EmptyValueAction() {
			return TriStateFlowControlValue.Continue;
		}
	}
	#endregion
}
