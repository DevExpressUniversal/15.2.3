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
	#region RemovedShiftLeftSharedFormulaRPNVisitor
	public class RemovedShiftLeftSharedFormulaRPNVisitor : RemovedShiftLeftRPNVisitor {
		public static RemovedShiftLeftSharedFormulaRPNVisitor GetWalkerShared(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				return new RemovedShiftLeftSharedFormulaRPNVisitor(notificationContext, dataContext);
			if (mode == RemoveCellMode.ShiftCellsUp)
				return new RemovedShiftUpSharedFormulaRPNVisitor(notificationContext, dataContext);
			if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return new RemovedNoShiftOrRangeToPasteCutRangeSharedFormulaRPNVisitor(notificationContext, dataContext);
			return new RemovedDefaultSharedFormulaRPNVisitor(notificationContext, dataContext);
		}
		int hostCellColumnOffset;
		int hostCellRowOffset;
		CellPosition formulaTopLeft;
		ICellTable formulaSheet;
		public RemovedShiftLeftSharedFormulaRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public void SetHostCellProperties(int hostCellColumnOffset, int hostCellRowOffset, CellPosition formulaTopLeft, Worksheet formulaSheet) {
			this.hostCellColumnOffset = hostCellColumnOffset;
			this.hostCellRowOffset = hostCellRowOffset;
			this.formulaTopLeft = formulaTopLeft;
			this.formulaSheet = formulaSheet;
		}
		public virtual bool RemoveBehavour { get { return Logic.RemoveBehavior; } }
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			bool checkHostCellSheetId = formulaSheet.SheetId == NotificationContext.Range.SheetId;
			if (!checkHostCellSheetId)
				DataContext.PushCurrentCell(formulaTopLeft);
			try {
				CellPosition position = thing.Location.ToCellPosition(DataContext);
				CellRange range = new CellRange(DataContext.CurrentWorksheet, position, position);
				Logic.SetRange(range);
				if (NotificationContext.Range.Includes(range) && Logic.RemoveBehavior)
					return thing.GetRefErrorEquivalent();
				if (Logic.ProcessRange()) {
					CellPosition modifiedPosition = Logic.Range.TopLeft;
					int dColumn = modifiedPosition.Column - position.Column;
					int dRow = modifiedPosition.Row - position.Row;
					bool columnNotMoved = dColumn == hostCellColumnOffset && (hostCellColumnOffset == 0 || modifiedPosition.ColumnType == PositionType.Relative);
					bool rowNotMoved = dRow == hostCellRowOffset && (hostCellRowOffset == 0 || modifiedPosition.RowType == PositionType.Relative);
					if (columnNotMoved && rowNotMoved && checkHostCellSheetId)
						return thing;
					CellOffset location = thing.Location;
					if (location.ColumnType != CellOffsetType.Offset && checkHostCellSheetId)
						location.Column = modifiedPosition.Column;
					else
						location.Column += dColumn - hostCellColumnOffset;
					if (location.RowType != CellOffsetType.Offset && checkHostCellSheetId)
						location.Row = modifiedPosition.Row;
					else
						location.Row += dRow - hostCellRowOffset;
					thing.Location = location;
					FormulaChanged = true;
				}
				else if (hostCellColumnOffset != 0 || hostCellRowOffset != 0)
					thing.Location = TryMoveSourceCell(thing.Location);
				return thing;
			}
			finally {
				if (!checkHostCellSheetId)
					DataContext.PopCurrentCell();
			}
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			bool checkHostCellSheetId = formulaSheet.SheetId == NotificationContext.Range.SheetId;
			if (!checkHostCellSheetId)
				DataContext.PushCurrentCell(formulaTopLeft);
			try {
				CellPosition topLeft = thing.First.ToCellPosition(DataContext);
				CellPosition bottomRight = thing.Last.ToCellPosition(DataContext);
				CellRange range = new CellRange(DataContext.CurrentWorksheet, topLeft, bottomRight);
				if (Logic.RemoveBehavior && NotificationContext.Range.Includes(range))
					return thing.GetRefErrorEquivalent();
				Logic.SetRange(range);
				if (Logic.ProcessRange()) {
					CellOffset first = thing.First;
					CellOffset last = thing.Last;
					first.Column = Logic.Range.TopLeft.Column;
					last.Column = Logic.Range.BottomRight.Column;
					first.Row = Logic.Range.TopLeft.Row;
					last.Row = Logic.Range.BottomRight.Row;
					if (first.ColumnType == CellOffsetType.Offset)
						first.Column -= formulaTopLeft.Column + hostCellColumnOffset;
					if (last.ColumnType == CellOffsetType.Offset)
						last.Column -= formulaTopLeft.Column + hostCellColumnOffset;
					if (first.RowType == CellOffsetType.Offset)
						first.Row -= formulaTopLeft.Row + hostCellRowOffset;
					if (last.RowType == CellOffsetType.Offset)
						last.Row -= formulaTopLeft.Row + hostCellRowOffset;
					thing.First = first;
					thing.Last = last;
					FormulaChanged = true;
				}
				else if (hostCellColumnOffset != 0 || hostCellRowOffset != 0) {
					thing.First = TryMoveSourceCell(thing.First);
					thing.Last = TryMoveSourceCell(thing.Last);
				}
				return thing;
			}
			finally {
				if (!checkHostCellSheetId)
					DataContext.PopCurrentCell();
			}
		}
		CellOffset TryMoveSourceCell(CellOffset location) {
			if (location.ColumnType == CellOffsetType.Offset && hostCellColumnOffset != 0) {
				location.Column -= hostCellColumnOffset;
				FormulaChanged = true;
			}
			if (location.RowType == CellOffsetType.Offset && hostCellRowOffset != 0) {
				location.Row -= hostCellRowOffset;
				FormulaChanged = true;
			}
			return location;
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftLeftSharedFormulaRPNLogic(cellRange);
		}
	}
	#endregion
	#region RemovedShiftUpSharedFormulaRPNVisitor
	public class RemovedShiftUpSharedFormulaRPNVisitor : RemovedShiftLeftSharedFormulaRPNVisitor {
		public RemovedShiftUpSharedFormulaRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftUpSharedFormulaRPNLogic(cellRange);
		}
	}
	#endregion
	#region RemovedDefaultSharedFormulaRPNVisitor
	public class RemovedDefaultSharedFormulaRPNVisitor : RemovedShiftLeftSharedFormulaRPNVisitor {
		public RemovedDefaultSharedFormulaRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override bool RemoveBehavour { get { return false; } }
		internal protected override bool IsResizingRange(CellRange range) {
			return false;
		}
		protected internal override bool IsMovingRange(CellRange range) {
			return false;
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			return thing;
		}
	}
	#endregion
	#region RemovedDefaultSharedFormulaRPNVisitor
	public class RemovedNoShiftOrRangeToPasteCutRangeSharedFormulaRPNVisitor : RemovedShiftLeftSharedFormulaRPNVisitor {
		public RemovedNoShiftOrRangeToPasteCutRangeSharedFormulaRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override bool RemoveBehavour { get { return false; } }
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			return thing;
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveNoShiftRPNLogic(cellRange);
		}
	}
	#endregion
	#region InsertedShiftRightSharedFormulaRPNVisitor
	public class InsertedShiftRightSharedFormulaRPNVisitor : RemovedShiftLeftSharedFormulaRPNVisitor {
		public InsertedShiftRightSharedFormulaRPNVisitor(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftRightSharedFormulaRPNLogic(cellRange);
		}
		protected internal override bool CellIsDefaultRemoving(ICellBase cell) {
			return false;
		}
	}
	#endregion
	#region InsertedShiftDownSharedFormulaRPNVisitor
	public class InsertedShiftDownSharedFormulaRPNVisitor : InsertedShiftRightSharedFormulaRPNVisitor {
		public InsertedShiftDownSharedFormulaRPNVisitor(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftDownSharedFormulaRPNLogic(cellRange);
		}
	}
	#endregion
}
