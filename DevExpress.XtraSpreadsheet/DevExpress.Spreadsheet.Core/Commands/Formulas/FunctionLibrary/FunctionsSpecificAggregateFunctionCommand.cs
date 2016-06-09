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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FunctionsSpecificAggregateFunctionCommand (abstract class)
	public abstract class FunctionsSpecificAggregateFunctionCommand : FunctionsInsertSpecificFunctionCommand {
		protected FunctionsSpecificAggregateFunctionCommand(ISpreadsheetControl control, string functionName)
			: base(control, functionName) {
		}
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			if (!InnerControl.TryEditActiveCellContent())
				return;
			if (!InsertAggregate())
				InsertFunctionFormulaAndOpenInplaceEditor();
		}
		bool InsertAggregate() {
			bool isAggregateInserted = false;
			DocumentModel.BeginUpdate();
			try {
				SheetViewSelection selection = ActiveSheet.Selection;
				List<CellRangeBase> newRanges = new List<CellRangeBase>();
				int activeRangeIndex = selection.ActiveRangeIndex;
				foreach (CellRange range in selection.SelectedRanges) {
					CellRange extendedRange = TryInsertAggregateFunction(range);
					if (extendedRange != null) {
						newRanges.Add(extendedRange);
						isAggregateInserted = true;
					}
					else
						newRanges.Add(range);
				}
				if (activeRangeIndex >= newRanges.Count)
					activeRangeIndex = 0;
				CellPosition activeCell = selection.ActiveCell;
				if (!newRanges[activeRangeIndex].ContainsCell(activeCell.Column, activeCell.Row))
					activeCell = newRanges[activeRangeIndex].TopLeft;
				selection.SetSelectionCore(activeCell, newRanges, false);
				selection.SetActiveRangeIndex(activeRangeIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return isAggregateInserted;
		}
		CellRange TryInsertAggregateFunction(CellRange range) {
			if (range.CellCount <= 1)
				return null;
			if (range is CellIntervalRange)
				return null;
			if (range.Width == DocumentModel.ActiveSheet.MaxColumnCount)
				return null;
			if(range.Height == DocumentModel.ActiveSheet.MaxColumnCount)
				return null;
			if (!ContainsAggregateValues(range))
				return null;
			if (range.Height == 1)
				return InsertHorizontalVectorAggregateFunction(range);
			else
				return InsertAggregateFunctionCore(range);
		}
		CellRange InsertHorizontalVectorAggregateFunction(CellRange range) {
			CellRange horizontalResultsRange = CalculateHorizontalAggregationResultsRange(range);
			if (horizontalResultsRange.BottomRight.Column == range.BottomRight.Column) {
				CellRange valuesRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column - 1, range.BottomRight.Row));
				return AggregateHorizontally(valuesRange, horizontalResultsRange);
			}
			else
				return AggregateHorizontally(range, horizontalResultsRange);
		}
		CellRange InsertAggregateFunctionCore(CellRange range) {
			CellRange verticalResultsRange = CalculateVerticalAggregationResultsRange(range);
			CellRange horizontalResultsRange;
			if (verticalResultsRange != null) {
				CellRange vvvRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column, verticalResultsRange.BottomRight.Row));
				horizontalResultsRange = CalculateHorizontalAggregationResultsRange(vvvRange);
			}
			else
				horizontalResultsRange = CalculateHorizontalAggregationResultsRange(range);
			if (horizontalResultsRange == null && verticalResultsRange == null)
				return null;
			if (verticalResultsRange == null)
				return AggregateHorizontally(range, horizontalResultsRange);
			if (horizontalResultsRange == null)
				return AggregateVertically(range, verticalResultsRange);
			if (verticalResultsRange.BottomRight.Row == range.BottomRight.Row) {
				CellRange valuesRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column, range.BottomRight.Row - 1));
				CellRange extendedRange = AggregateVertically(valuesRange, verticalResultsRange);
				if (horizontalResultsRange.BottomRight.Column == range.BottomRight.Column) {
					valuesRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column - 1, range.BottomRight.Row));
					return AggregateHorizontally(valuesRange, horizontalResultsRange);
				}
				return extendedRange;
			}
			else {
				if (horizontalResultsRange.BottomRight.Column == range.BottomRight.Column) {
					CellRange valuesRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column - 1, range.BottomRight.Row));
					return AggregateHorizontally(valuesRange, horizontalResultsRange);
				}
				else
					return AggregateVertically(range, verticalResultsRange);
			}
		}
		CellRange AggregateVertically(CellRange range, CellRange resultsRange) {
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			int leftAggregatedColumn = rightColumn;
			for (int column = rightColumn; column >= leftColumn; column--) {
				CellRange aggregateRange = new CellRange(range.Worksheet, new CellPosition(column, topRow), new CellPosition(column, bottomRow));
				CellRange extendedAggregateRange = new CellRange(range.Worksheet, aggregateRange.TopLeft, new CellPosition(column, resultsRange.BottomRight.Row));
				if (ContainsAggregateValues(extendedAggregateRange)) {
					InsertAggregateFormula(column, resultsRange.BottomRight.Row, aggregateRange);
					leftAggregatedColumn = column;
				}
			}
			return new CellRange(range.Worksheet, new CellPosition(leftAggregatedColumn, topRow), new CellPosition(rightColumn, resultsRange.BottomRight.Row));
		}
		CellRange AggregateHorizontally(CellRange range, CellRange resultsRange) {
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			int topAggregatedRow = bottomRow;
			for (int row = bottomRow; row >= topRow; row--) {
				CellRange aggregateRange = new CellRange(range.Worksheet, new CellPosition(leftColumn, row), new CellPosition(rightColumn, row));
				CellRange extendedAggregateRange = new CellRange(range.Worksheet, aggregateRange.TopLeft, new CellPosition(resultsRange.BottomRight.Column, row));
				if (ContainsAggregateValues(extendedAggregateRange)) {
					InsertAggregateFormula(resultsRange.BottomRight.Column, row, aggregateRange);
					topAggregatedRow = row;
				}
			}
			return new CellRange(range.Worksheet, new CellPosition(leftColumn, topAggregatedRow), new CellPosition(resultsRange.BottomRight.Column, bottomRow));
		}
		CellRange CalculateVerticalAggregationResultsRange(CellRange range) {
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			for (int row = range.BottomRight.Row; row < ActiveSheet.MaxRowCount; row++) {
				CellRange resultsRange = new CellRange(range.Worksheet, new CellPosition(leftColumn, row), new CellPosition(rightColumn, row));
				if (ContainsNoValues(resultsRange))
					return resultsRange;
				if (ContainsOnlyAggregatesOrEmptyValues(resultsRange))
					return resultsRange;
			}
			return null;
		}
		CellRange CalculateHorizontalAggregationResultsRange(CellRange range) {
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			for (int column = range.BottomRight.Column; column < ActiveSheet.MaxColumnCount; column++) {
				CellRange resultsRange = new CellRange(range.Worksheet, new CellPosition(column, topRow), new CellPosition(column, bottomRow));
				if (ContainsNoValues(resultsRange))
					return resultsRange;
				if (ContainsOnlyAggregatesOrEmptyValues(resultsRange))
					return resultsRange;
			}
			return null;
		}
		void InsertAggregateFormula(int column, int row, CellRange range) {
			string value = "=" + FunctionName + "(" + range.ToString(DocumentModel.DataContext) + ")";
			ActiveSheet[column, row].Text = value;
		}
		bool ContainsAggregateValues(CellRange range) {
			foreach (VariantValue value in new Enumerable<VariantValue>(range.GetExistingValuesEnumerator())) {
				if (value.IsBoolean || value.IsNumeric)
					return true;
			}
			return false;
		}
		bool ContainsNoValues(CellRange range) {
			foreach (VariantValue value in new Enumerable<VariantValue>(range.GetExistingValuesEnumerator())) {
				if (!value.IsEmpty)
					return false;
			}
			return true;
		}
		bool ContainsOnlyAggregatesOrEmptyValues(CellRange range) {
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell == null || !ContainsOnlyAggregatesOrEmptyValue(cell))
					return false;
			}
			return true;
		}
		bool ContainsOnlyAggregatesOrEmptyValue(ICell cell) {
			if (!cell.HasFormula)
				return cell.Value.IsEmpty;
			return ContainsAggregate(cell);
		}
		bool ContainsAggregate(ICell cell) {
			if (cell.HasFormula) {
				ParsedExpression expression = cell.GetFormula().Expression;
				if (expression.Count > 0) {
					ParsedThingFunc function = expression[expression.Count - 1] as ParsedThingFunc;
					if (function != null && StringExtensions.CompareInvariantCultureIgnoreCase(function.Function.Name, this.FunctionNameInvariant) == 0)
						return true;
				}
			}
			return false;
		}
	}
	#endregion
}
