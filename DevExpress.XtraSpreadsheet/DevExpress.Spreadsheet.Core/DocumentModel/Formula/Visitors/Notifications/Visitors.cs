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

using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region RemovedShiftLeftRPNVisitor
	public class RemovedShiftLeftRPNVisitor : RemovedDefaultRPNVisitor {
		RemoveShiftLeftRPNLogic logic;
		public RemovedShiftLeftRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
			logic = GetLogic(notificationContext.Range);
		}
		public RemoveShiftLeftRPNLogic Logic { get { return logic; } }
		protected virtual RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftLeftRPNLogic(cellRange);
		}
		public override CellRangeBase ProcessCellRange(CellRange range) {
			if (NotificationContext.Range.Includes(range) && logic.RemoveBehavior)
				return null;
			logic.SetRange(range);
			if (logic.ProcessRange())
				return logic.Range;
			else
				return range;
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			if (logic.RemoveBehavior && NotificationContext.Range.ContainsCell(thing.Position.Column, thing.Position.Row))
				return thing.GetRefErrorEquivalent();
			CellPosition updatedPosition = logic.ProcessCellPosition(thing.Position);
			if (!updatedPosition.Equals(thing.Position)) {
				thing.Position = updatedPosition;
				FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			CellRange range = new CellRange(DataContext.CurrentWorksheet, thing.TopLeft, thing.BottomRight);
			if (NotificationContext.Range.Includes(range) && logic.RemoveBehavior) {
				ReplaceCurrentExpression(thing.GetRefErrorEquivalent());
				return thing;
			}
			logic.SetRange(range);
			logic.ProcessRange();
			if (!thing.CellRange.Equals(logic.Range)) {
				thing.CellRange = logic.Range;
				FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			Worksheet sheet = DataContext.CurrentWorksheet as Worksheet;
			int top = 0;
			int left = 0;
			int bottom = 0;
			int right = 0;
			if (thing.First.ColumnType == CellOffsetType.Offset || thing.Last.ColumnType == CellOffsetType.Offset)
				right = sheet.MaxColumnCount - 1;
			else {
				left = thing.First.Column;
				right = thing.Last.Column;
			}
			if (thing.First.RowType == CellOffsetType.Offset || thing.Last.RowType == CellOffsetType.Offset)
				bottom = sheet.MaxRowCount - 1;
			else {
				top = thing.First.Row;
				bottom = thing.Last.Row;
			}
			CellRange range = new CellRange(DataContext.CurrentWorksheet, new CellPosition(left, top), new CellPosition(right, bottom));
			if (Logic.RemoveBehavior && NotificationContext.Range.Includes(range))
				return thing.GetRefErrorEquivalent();
			Logic.SetRange(range);
			if (Logic.ProcessRange()) {
				CellOffset first = thing.First;
				CellOffset last = thing.Last;
				if (first.ColumnType != CellOffsetType.Offset && last.ColumnType != CellOffsetType.Offset) {
					first.Column = Logic.Range.TopLeft.Column;
					last.Column = Logic.Range.BottomRight.Column;
				}
				if (first.RowType != CellOffsetType.Offset && last.RowType != CellOffsetType.Offset) {
					first.Row = Logic.Range.TopLeft.Row;
					last.Row = Logic.Range.BottomRight.Row;
				}
				thing.First = first;
				thing.Last = last;
				FormulaChanged = true;
			}
			return thing;
		}
		public override void Visit(ParsedThingTable thing) {
			Table table = DataContext.GetDefinedTableRange(thing.TableName);
			if (table == null)
				ReplaceCurrentExpression(new ParsedThingRefErr());
			else if (thing.ColumnsDefined) {
				TableColumn startColumn = table.Columns[thing.ColumnStart];
				if (startColumn == null || NotificationContext.Range.ContainsRange(startColumn.GetColumnRange()))
					ReplaceCurrentExpression(new ParsedThingRefErr());
				else {
					if (!string.IsNullOrEmpty(thing.ColumnEnd)) {
						TableColumn endColumn = table.Columns[thing.ColumnEnd];
						if (endColumn == null || NotificationContext.Range.ContainsRange(endColumn.GetColumnRange()))
							ReplaceCurrentExpression(new ParsedThingRefErr());
					}
				}
			}
			base.Visit(thing);
		}
		protected internal override bool IsResizingRange(CellRange range) {
			return logic.IsResizingRange(range);
		}
		protected internal virtual bool IsMovingRange(CellRange range) {
			return logic.IsMovingRange(range);
		}
	}
	#endregion
	#region RemovedShiftUpRPNVisitor
	public class RemovedShiftUpRPNVisitor : RemovedShiftLeftRPNVisitor {
		public RemovedShiftUpRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftUpRPNLogic(cellRange);
		}
	}
	#endregion
	public class RemovedNoShiftRPNVisitor : RemovedShiftLeftRPNVisitor {
		public RemovedNoShiftRPNVisitor(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveNoShiftRPNLogic(cellRange);
		}
	}
	#region RemovedDefaultRPNVisitor
	public class RemovedDefaultRPNVisitor : ReferenceThingsRPNVisitor {
		public static RemovedDefaultRPNVisitor GetWalker(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				return new RemovedShiftLeftRPNVisitor(notificationContext, dataContext);
			else if (mode == RemoveCellMode.ShiftCellsUp)
				return new RemovedShiftUpRPNVisitor(notificationContext, dataContext);
			else
				return new RemovedDefaultRPNVisitor(notificationContext, dataContext);
		}
		public static RemovedDefaultRPNVisitor GetInsertWalker(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext) {
			if (notificationContext.Mode == InsertCellMode.ShiftCellsDown)
				return new InsertedShiftDownRPNVisitor(notificationContext, dataContext);
			else
				return new InsertedShiftRightRPNVisitor(notificationContext, dataContext);
		}
		#region fields
		readonly InsertRemoveRangeNotificationContextBase notificationContext;
		#endregion
		public RemovedDefaultRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(dataContext) {
			this.notificationContext = notificationContext;
		}
		#region Properties
		public InsertRemoveRangeNotificationContextBase NotificationContext { get { return notificationContext; } }
		#endregion
		public override CellRangeBase ProcessCellRange(CellRange range) {
			if (notificationContext.Range.Includes(range))
				return null;
			return range;
		}
		public virtual CellRangeBase GetShiftedPartOfRange(CellRange modifiedRange) {
			return null;
		}
		public virtual CellRangeBase GetNotShiftedPartOfRange(CellRange modifiedRange) {
			return null;
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			return thing;
		}
		internal protected virtual bool IsResizingRange(CellRange range) {
			return false;
		}
		protected internal virtual bool CellIsDefaultRemoving(ICellBase cell) {
			if (cell.Sheet.SheetId == notificationContext.Range.SheetId)
				return notificationContext.Range.ContainsCell(cell.Position.Column, cell.Position.Row);
			return false;
		}
	}
	#endregion
	#region InsertedShiftRightRPNVisitor
	public class InsertedShiftRightRPNVisitor : RemovedShiftLeftRPNVisitor {
		public InsertedShiftRightRPNVisitor(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftRightRPNLogic(cellRange);
		}
		protected internal override bool CellIsDefaultRemoving(ICellBase cell) {
			return false;
		}
	}
	#endregion
	#region InsertedShiftDownRPNVisitor
	public class InsertedShiftDownRPNVisitor : RemovedShiftUpRPNVisitor {
		public InsertedShiftDownRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftDownRPNLogic(cellRange);
		}
		protected internal override bool CellIsDefaultRemoving(ICellBase cell) {
			return false;
		}
	}
	#endregion
	#region SortRangeFormulaReferencesVisitor
	public class SortRangeFormulaReferencesVisitor : ReferenceThingsRPNVisitor {
		#region Fields
		readonly int filterSheetId;
		CellPositionOffset offset;
		#endregion
		public SortRangeFormulaReferencesVisitor(int filterSheetId, CellPositionOffset offset, WorkbookDataContext dataContext)
			: base(dataContext) {
			this.filterSheetId = filterSheetId;
			this.offset = offset;
		}
		public override CellRangeBase ProcessCellRange(CellRange range) {
			throw new System.InvalidOperationException();
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			CellPosition modifiedPosition = thing.Position.GetShifted(offset, DataContext.CurrentWorksheet);
			if (modifiedPosition.EqualsPosition(CellPosition.InvalidValue))
				return thing.GetRefErrorEquivalent();
			if (!modifiedPosition.EqualsPosition(thing.Position)) {
				thing.Position = modifiedPosition;
				this.FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			DevExpress.Office.Utils.Exceptions.ThrowInvalidOperationException("RefRel");
			return thing;
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			CellRange newRange = thing.PreEvaluateReference(DataContext).GetShifted(offset);
			if (newRange == null)
				return thing.GetRefErrorEquivalent();
			thing.CellRange = newRange;
			this.FormulaChanged = true;
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			DevExpress.Office.Utils.Exceptions.ThrowInvalidOperationException("AreaN");
			return base.ProcessAreaRel(thing);
		}
		protected override bool ShouldProcessSheetDefinitionCore(SheetDefinition expression, int sheetId) {
			return sheetId == filterSheetId;
		}
	}
	#endregion
}
