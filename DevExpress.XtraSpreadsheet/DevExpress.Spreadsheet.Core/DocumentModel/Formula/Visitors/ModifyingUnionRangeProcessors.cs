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
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModifyingUnionRangeProcessor (abstract)
	public abstract class ModifyingUnionRangeProcessor {
		#region Static
		public static ModifyingUnionRangeProcessor GetProcessor(InsertRangeNotificationContext notificationContext) {
			InsertCellMode mode = notificationContext.Mode;
			if (mode == InsertCellMode.ShiftCellsRight)
				return new InsertRangeShiftRightUnionRangeProcessor(notificationContext);
			else if (mode == InsertCellMode.ShiftCellsDown)
				return new InsertRangeShiftDownUnionRangeProcessor(notificationContext);
			else return null;
		}
		public static ModifyingUnionRangeProcessor GetProcessor(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				return new RemoveRangeShiftLeftUnionRangeProcessor(notificationContext);
			else if (mode == RemoveCellMode.ShiftCellsUp)
				return new RemoveRangeShiftUpUnionRangeProcessor(notificationContext);
			else if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return new RemoveRangeNoShiftUnionRangeProcessor(notificationContext);
			else return null;
		}
		#endregion
		#region Fields
		InsertRemoveRangeNotificationContextBase notificationContext;
		RemovedDefaultRPNVisitor walker;
		CellRange resizingRange;
		#endregion
		protected ModifyingUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext) {
			this.notificationContext = notificationContext;
			walker = GetWalker(notificationContext);
		}
		#region properties
		protected CellRange ChangingRange { get { return notificationContext.Range; } }
		internal protected RemovedDefaultRPNVisitor Walker { get { return walker; } }
		protected ICellTable Sheet { get { return ChangingRange.Worksheet; } }
		protected CellRange ResizingRange { get { return resizingRange; } set { resizingRange = value; } }
		protected bool RemovingBehaviourChanged { get; set; }
		#endregion
		public CellRangeBase ProcessRange(CellRangeBase modifiedRange) {
			resizingRange = null;
			if (modifiedRange.RangeType == CellRangeType.UnionRange) {
				CellUnion resultRange = new CellUnion(new CellRangeList());
				foreach (CellRange currentRange in (modifiedRange as CellUnion).InnerCellRanges) {
					CellRangeBase range = ProcessSingleRange(currentRange);
					if (range == null)
						continue;
					if (range.RangeType == CellRangeType.UnionRange)
						resultRange.InnerCellRanges.AddRange((range as CellUnion).InnerCellRanges);
					else resultRange.InnerCellRanges.Add(range);
				}
				return resultRange;
			}
			return ProcessSingleRange(modifiedRange as CellRange);
		}
		CellRangeBase ProcessSingleRange(CellRange modifiedSingleRange) {
			CellRangeBase addedRange = GetAddedRange(modifiedSingleRange);
			if (addedRange != null)
				return addedRange;
			if (!walker.IsResizingRange(modifiedSingleRange)) {
				resizingRange = walker.ProcessCellRange(modifiedSingleRange) as CellRange;
				return resizingRange;
			}
			List<int> grid = GetGrid(modifiedSingleRange);
			if (grid.Count == 2)
				return walker.ProcessCellRange(modifiedSingleRange);
			return GetResultRange(modifiedSingleRange, grid);
		}
		public bool IsResizingRange(CellRangeBase modifiedRange) {
			if (modifiedRange.RangeType == CellRangeType.UnionRange) {
				foreach (CellRange currentRange in (modifiedRange as CellUnion).InnerCellRanges)
					if (IsResizingSingleRange(currentRange))
						return true;
				return false;
			}
			return IsResizingSingleRange(modifiedRange as CellRange);
		}
		bool IsResizingSingleRange(CellRange modifiedSingleRange) {
			return walker.IsResizingRange(modifiedSingleRange);
		}
		public virtual ParsedExpression Process(ParsedExpression expression) {
			return Walker.Process(expression);
		}
		#region Data Validation removing range behaviour
		protected bool FormulaAffectsRemoving(ParsedExpression expression) {
			List<CellRangeBase> ranges = expression.GetInvolvedCellRanges(Sheet.Workbook.DataContext);
			if (ranges == null)
				return false;
			foreach (CellRangeBase formulaRange in ranges)
				if (ChangingRange.Intersects(formulaRange) || FormulaIsRightOrBelowChangingRange(formulaRange))
					return true;
			return false;
		}
		protected virtual bool FormulaIsRightOrBelowChangingRange(CellRangeBase formulaRange) {
			return false;
		}
		#endregion
		protected abstract RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext);
		protected abstract CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid);
		protected abstract List<int> GetGrid(CellRange modifiedSingleRange);
		protected abstract CellRangeBase GetAddedRange(CellRange modifiedSingleRange);
	}
	#endregion
	#region InsertRangeShiftRightUnionRangeProcessor
	public class InsertRangeShiftRightUnionRangeProcessor : ModifyingUnionRangeProcessor {
		public InsertRangeShiftRightUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext)
			: base(notificationContext) {
		}
		protected override RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new InsertedShiftRightDataValidationRPNVisitor(notificationContext, Sheet.Workbook.DataContext);
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			CellUnion resultRange = new CellUnion(new List<CellRangeBase>());
			PositionType columnType = modifiedSingleRange.TopLeft.ColumnType;
			PositionType rowType = modifiedSingleRange.TopLeft.RowType;
			int left = modifiedSingleRange.TopLeft.Column;
			int right = modifiedSingleRange.BottomRight.Column;
			for (int i = 0; i < grid.Count - 1; i++) {
				int top = grid[i];
				int bottom = grid[i + 1] - ((i + 1 != grid.Count - 1) ? 1 : 0);
				CellRange range = new CellRange(Sheet, new CellPosition(left, top, columnType, rowType), new CellPosition(right, bottom, columnType, rowType));
				if (top >= ChangingRange.TopLeft.Row && bottom <= ChangingRange.BottomRight.Row) {
					range = Walker.ProcessCellRange(range) as CellRange;
					ResizingRange = range;
					if (range != null)
						resultRange.InnerCellRanges.Insert(0, range);
				}
				else resultRange.InnerCellRanges.Add(range);
			}
			return resultRange.InnerCellRanges.Count == 1 ? resultRange.InnerCellRanges[0] : resultRange;
		}
		protected override List<int> GetGrid(CellRange modifiedSingleRange) {
			List<int> grid = new List<int>();
			int minGrid = modifiedSingleRange.TopLeft.Row;
			int maxGrid = modifiedSingleRange.BottomRight.Row;
			grid.Add(minGrid);
			int topRow = ChangingRange.TopLeft.Row;
			int bottomRow = ChangingRange.BottomRight.Row + 1;
			if (topRow <= maxGrid && topRow >= minGrid && !grid.Contains(topRow))
				grid.Add(topRow);
			if (bottomRow <= maxGrid && bottomRow >= minGrid && !grid.Contains(bottomRow))
				grid.Add(bottomRow);
			grid.Add(maxGrid);
			return grid;
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			if (ChangingRange.TopLeft.Column == modifiedSingleRange.BottomRight.Column + 1)
				if (ChangingRange.RangeType == CellRangeType.IntervalRange) {
					modifiedSingleRange.BottomRight = new CellPosition(ChangingRange.BottomRight.Column, modifiedSingleRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType);
					return modifiedSingleRange;
				}
				else if (ChangingRange.TopLeft.Row <= modifiedSingleRange.BottomRight.Row && ChangingRange.BottomRight.Row >= modifiedSingleRange.TopLeft.Row) {
					int topRow = Math.Max(modifiedSingleRange.TopLeft.Row, ChangingRange.TopLeft.Row);
					int bottomRow = Math.Min(modifiedSingleRange.BottomRight.Row, ChangingRange.BottomRight.Row);
					CellRange range = new CellRange(Sheet,
						new CellPosition(ChangingRange.TopLeft.Column, topRow, modifiedSingleRange.TopLeft.ColumnType, modifiedSingleRange.TopLeft.RowType),
						new CellPosition(ChangingRange.BottomRight.Column, bottomRow, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType)
						);
					CellUnion unionRange = new CellUnion(new List<CellRangeBase>());
					unionRange.InnerCellRanges.Add(modifiedSingleRange);
					unionRange.InnerCellRanges.Add(range);
					return unionRange;
				}
			return null;
		}
	}
	#endregion
	#region InsertRangeShiftDownUnionRangeProcessor
	public class InsertRangeShiftDownUnionRangeProcessor : ModifyingUnionRangeProcessor {
		public InsertRangeShiftDownUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext)
			: base(notificationContext) {
		}
		protected override RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new InsertedShiftDownDataValidationRPNVisitor(notificationContext, Sheet.Workbook.DataContext);
		}
		protected override List<int> GetGrid(CellRange modifiedSingleRange) {
			List<int> grid = new List<int>();
			int minGrid = modifiedSingleRange.TopLeft.Column;
			int maxGrid = modifiedSingleRange.BottomRight.Column;
			grid.Add(minGrid);
			int leftColumn = ChangingRange.TopLeft.Column;
			int rightColumn = ChangingRange.BottomRight.Column + 1;
			if (leftColumn <= maxGrid && leftColumn >= minGrid && !grid.Contains(leftColumn))
				grid.Add(leftColumn);
			if (rightColumn <= maxGrid && rightColumn >= minGrid && !grid.Contains(rightColumn))
				grid.Add(rightColumn);
			grid.Add(maxGrid);
			return grid;
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			if (ChangingRange.TopLeft.Row == modifiedSingleRange.BottomRight.Row + 1)
				if (ChangingRange.RangeType == CellRangeType.IntervalRange) {
					modifiedSingleRange.BottomRight = new CellPosition(modifiedSingleRange.BottomRight.Column, ChangingRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType);
					return modifiedSingleRange;
				}
				else if (ChangingRange.TopLeft.Column <= modifiedSingleRange.BottomRight.Column && ChangingRange.BottomRight.Column >= modifiedSingleRange.TopLeft.Column) {
					int leftColumn = Math.Max(modifiedSingleRange.TopLeft.Column, ChangingRange.TopLeft.Column);
					int rightColumn = Math.Min(modifiedSingleRange.BottomRight.Column, ChangingRange.BottomRight.Column);
					CellRange range = new CellRange(Sheet,
						new CellPosition(leftColumn, ChangingRange.TopLeft.Row, modifiedSingleRange.TopLeft.ColumnType, modifiedSingleRange.TopLeft.RowType),
						new CellPosition(rightColumn, ChangingRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType)
						);
					CellUnion unionRange = new CellUnion(new List<CellRangeBase>());
					unionRange.InnerCellRanges.Add(modifiedSingleRange);
					unionRange.InnerCellRanges.Add(range);
					return unionRange;
				}
			return null;
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			CellUnion resultRange = new CellUnion(new List<CellRangeBase>());
			PositionType columnType = modifiedSingleRange.TopLeft.ColumnType;
			PositionType rowType = modifiedSingleRange.TopLeft.RowType;
			int top = modifiedSingleRange.TopLeft.Row;
			int bottom = modifiedSingleRange.BottomRight.Row;
			for (int i = 0; i < grid.Count - 1; i++) {
				int left = grid[i];
				int right = grid[i + 1] - ((i + 1 != grid.Count - 1) ? 1 : 0);
				CellRange range = new CellRange(Sheet, new CellPosition(left, top, columnType, rowType), new CellPosition(right, bottom, columnType, rowType));
				if (left >= ChangingRange.TopLeft.Column && right <= ChangingRange.BottomRight.Column) {
					CellRangeBase processedRange = Walker.ProcessCellRange(range);
					range = processedRange as CellRange;
					if (range != null)
						resultRange.InnerCellRanges.Insert(0, range);
					else {
						CellUnion resultUnion = processedRange as CellUnion;
						if (resultUnion != null)
							resultRange.InnerCellRanges.InsertRange(0, resultUnion.InnerCellRanges);
					}
				}
				else resultRange.InnerCellRanges.Add(range);
			}
			return resultRange.InnerCellRanges.Count == 1 ? resultRange.InnerCellRanges[0] : resultRange; ;
		}
	}
	#endregion
	#region RemoveRangeNoShiftUnionRangeProcessor
	public class RemoveRangeNoShiftUnionRangeProcessor : InsertRangeShiftDownUnionRangeProcessor {
		public RemoveRangeNoShiftUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext)
			: base(notificationContext) {
		}
		protected override RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedNoShiftDataValidationRPNVisitor(notificationContext, Sheet.Workbook.DataContext);
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
	}
	#endregion
	#region RemoveRangeShiftUpUnionRangeProcessor
	public class RemoveRangeShiftUpUnionRangeProcessor : InsertRangeShiftDownUnionRangeProcessor {
		public RemoveRangeShiftUpUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext)
			: base(notificationContext) {
		}
		protected override RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedShiftUpDataValidationRPNVisitor(notificationContext, Sheet.Workbook.DataContext);
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
	}
	#endregion
	#region RemoveRangeShiftLeftUnionRangeProcessor
	public class RemoveRangeShiftLeftUnionRangeProcessor : InsertRangeShiftRightUnionRangeProcessor {
		public RemoveRangeShiftLeftUnionRangeProcessor(InsertRemoveRangeNotificationContextBase notificationContext)
			: base(notificationContext) {
		}
		protected override RemovedDefaultRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedShiftLeftDataValidationRPNVisitor(notificationContext, Sheet.Workbook.DataContext);
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
	}
	#endregion
}
