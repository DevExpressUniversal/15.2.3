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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Linq;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PrintAreaCalculator : PrintRangeCalculator {
		public const string PrintAreaDefinedName = "_xlnm.Print_Area";
		public PrintAreaCalculator(Worksheet sheet)
			: base(sheet) {
		}
		CellIntervalRange printAreaDefinedNameIntervalRange = null;
		VariantValue EvaluateDefinedNameExpression(DefinedNameBase definedName) {
			WorkbookDataContext context = Sheet.Workbook.DataContext;
			context.PushCurrentWorksheet(Sheet);
			context.PushCurrentCell(0, 0);
			try {
				return definedName.Expression.Evaluate(context);
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		public CellRangeBase CalculateUsingDefinedName(bool returnFirstCellFromRangeForEmptyDocument) {
			CellRangeBase definedNameRange = GetDefinedNameRange();
			if (definedNameRange != null) {
				return ExcludeCellIntervalRangesFromPrintArea(definedNameRange, returnFirstCellFromRangeForEmptyDocument);
			}
			return CalculateWithoutDefindeName();
		}
		public CellRangeBase GetDefinedNameRange() {
			DefinedNameBase definedName = null;
			if (!Sheet.DefinedNames.TryGetItemByName(PrintAreaDefinedName, out definedName))
				return null;
			VariantValue resultValue = EvaluateDefinedNameExpression(definedName);
			if (!resultValue.IsCellRange)
				return null;
			return resultValue.CellRangeValue;
		}
		CellRangeBase ExcludeCellIntervalRangesFromPrintArea(CellRangeBase range, bool returnFirstCellFromRangeForEmptyDocument) {
			CellRangeBase resultRange = null;
			if (range.RangeType != CellRangeType.UnionRange) {
				resultRange = ProcessDefinedNameSingleRange((CellRange)range, returnFirstCellFromRangeForEmptyDocument);
			}
			else {
				int count = range.AreasCount;
				List<CellRangeBase> ranges = new List<CellRangeBase>(count);
				foreach (var innerRange in range.GetAreasEnumerable()) {
					CellRange validated = ProcessDefinedNameSingleRange(innerRange, returnFirstCellFromRangeForEmptyDocument);
					if (validated != null)
						ranges.Add(validated);
				}
				resultRange = new CellUnion(ranges);
			}
			return resultRange;
		}
		CellRange ProcessDefinedNameSingleRange(CellRange range, bool returnFirstCellFromRangeForEmptyDocument) {
			CellRange resultRange = range;
			if (range.RangeType == CellRangeType.IntervalRange) {
				CellRange calculated = CalculatePrintRangeWhenDefinedNameIsIntervalRange(range as CellIntervalRange, returnFirstCellFromRangeForEmptyDocument);
				if (calculated != null)
					resultRange = range.Intersection(calculated);
				else
					resultRange = null;
			}
			return resultRange;
		}
		CellRange CalculatePrintRangeWhenDefinedNameIsIntervalRange(CellIntervalRange definedNameIntervalRange, bool returnFirstCellFromRangeForEmptyDocument) {
			SetPrintAreaDefinedName(definedNameIntervalRange);
			return CalculateWithoutDefindeName(returnFirstCellFromRangeForEmptyDocument);
		}
		protected internal void SetPrintAreaDefinedName(CellIntervalRange range){
			this.printAreaDefinedNameIntervalRange = range;
		}
		protected override void CalculateMergedCellsPrintRange(RangeIntermediateInfo result) {
			if (printAreaDefinedNameIntervalRange == null) {
				base.CalculateMergedCellsPrintRange(result);
				return;
			}
			List<CellRange> mergedRanges = Sheet.MergedCells.GetMergedCellRangesIntersectsRange(printAreaDefinedNameIntervalRange);
			foreach (CellRange mergedRange in mergedRanges) {
				if (mergedRange.IsColumnRangeInterval() || mergedRange.IsRowRangeInterval())
					continue;
				ProcessMergedRange(result, mergedRange);
			}
		}
		protected override void CalculateRangeIncludesEveryDrawingObject(RangeIntermediateInfo result) {
			if (printAreaDefinedNameIntervalRange == null) {
				base.CalculateRangeIncludesEveryDrawingObject(result);
				return;
			}
			List<IDrawingObject> drawings = Sheet.DrawingObjects.GetIntersectedWithPrintArea(printAreaDefinedNameIntervalRange);
			foreach (IDrawingObject drawing in drawings)
				ProcessDrawingObject(result, drawing);
		}
		protected override void CalculateVisibleRangeUntilLastTableBottomRight(RangeIntermediateInfo info) {
			if (printAreaDefinedNameIntervalRange == null) {
				base.CalculateVisibleRangeUntilLastTableBottomRight(info);
				return;
			}
			bool intersects = false;
			List<Table> foundTables = Sheet.Tables.GetItems(printAreaDefinedNameIntervalRange, intersects);
			foreach (Table table in foundTables)
				ProcessTable(info, table.Range);
		}
		public override int GetStopTopRowIndex() {
			if (printAreaDefinedNameIntervalRange != null)
				return printAreaDefinedNameIntervalRange.TopRowIndex - 1;
			return base.GetStopTopRowIndex();
		}
		protected override int GetStopBottomRowIndex() {
			if (printAreaDefinedNameIntervalRange != null)
				return printAreaDefinedNameIntervalRange.BottomRowIndex + 1;
			return base.GetStopBottomRowIndex();
		}
		protected override int GetStopLeftColumnIndex() {
			if (printAreaDefinedNameIntervalRange != null)
				return printAreaDefinedNameIntervalRange.LeftColumnIndex - 1;
			return base.GetStopLeftColumnIndex();
		}
		protected override int GetStopRightColumnIndex() {
			if (printAreaDefinedNameIntervalRange != null)
				return printAreaDefinedNameIntervalRange.RightColumnIndex + 1;
			return base.GetStopRightColumnIndex();
		}
		protected override void ProcessFoundCell(RangeIntermediateInfo result, CellPosition pos) {
			if (printAreaDefinedNameIntervalRange == null)
				return;
			int from = Int32.MinValue;
			int to = Int32.MinValue;
			if (printAreaDefinedNameIntervalRange.BottomRowIndex != Sheet.MaxRowCount - 1) {
				from = pos.Row;
				to = printAreaDefinedNameIntervalRange.BottomRowIndex;
				int bottomVisibleRow = CalculateLastVisibleRow(from, to);
				result.ExpandToBottomRow(bottomVisibleRow);
			}
			if (printAreaDefinedNameIntervalRange.RightColumnIndex != Sheet.MaxColumnCount - 1) {
				from = pos.Column;
				to = printAreaDefinedNameIntervalRange.RightColumnIndex;
				int rightVisibleColumn = CalculateLastVisibleColumnRightFromTo(from, to);
				result.ExpandToRightColumn(rightVisibleColumn);
			}
		}
		public static bool PrintRangeIsValid(Worksheet sheet, bool useOnlyDefinedName) {
		   DefinedNameBase printRange;
			if (!sheet.DefinedNames.TryGetItemByName(PrintAreaCalculator.PrintAreaDefinedName, out printRange))
				return !useOnlyDefinedName;
			return PrintRangeIsValid(sheet, ((DefinedName)printRange).GetReferencedRange());
		}
		static bool PrintRangeIsValid(Worksheet sheet, CellRangeBase range) {
			if (range == null)
				return false;
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				foreach (CellRangeBase rangeBase in union.InnerCellRanges)
					if (!object.ReferenceEquals(sheet, rangeBase.Worksheet))
						return false;
				return true;
			}
			return object.ReferenceEquals(sheet, range.Worksheet);
		}
	}
}
