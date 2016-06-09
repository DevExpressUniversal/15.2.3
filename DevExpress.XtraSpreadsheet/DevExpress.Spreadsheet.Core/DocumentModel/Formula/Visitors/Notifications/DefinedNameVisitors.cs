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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model{
	#region RemovedShiftLeftDefinedNameRPNVisitor
	public class RemovedShiftLeftDefinedNameRPNVisitor : RemovedShiftLeftRPNVisitor {
		public RemovedShiftLeftDefinedNameRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override void Visit(ParsedThingRefRel thing) {
			if (IsRelative(thing.Location))
				return;
			base.Visit(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (IsRelative(thing.Location) || !NotificationContext.CheckEqualSheetStartDefinition(DataContext, thing.SheetDefinitionIndex))
				return;
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			if (IsRelative(thing.First) || IsRelative(thing.Last))
				return;
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (IsRelative(thing.First) || IsRelative(thing.Last) ||
				!NotificationContext.CheckEqualSheetStartDefinition(DataContext, thing.SheetDefinitionIndex))
				return;
			base.Visit(thing);
		}
		protected virtual bool IsRelative(CellOffset offset) {
			return offset.ColumnType == CellOffsetType.Offset;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			int top = 0;
			int left = 0;
			int bottom = 0;
			int right = 0;
			if (thing.Location.ColumnType == CellOffsetType.Offset)
				right = IndicesChecker.MaxColumnIndex;
			else left = right = thing.Location.Column;
			if (thing.Location.RowType == CellOffsetType.Offset)
				bottom = IndicesChecker.MaxRowIndex;
			else top = bottom = thing.Location.Row;
			CellRange range = new CellRange(DataContext.CurrentWorksheet, new CellPosition(left, top), new CellPosition(right, bottom));
			if (Logic.RemoveBehavior && NotificationContext.Range.Includes(range))
				return thing.GetRefErrorEquivalent();
			Logic.SetRange(range);
			if (Logic.ProcessRange()) {
				CellOffset location = thing.Location;
				if (thing.Location.ColumnType != CellOffsetType.Offset)
					location.Column = Logic.Range.TopLeft.Column;
				if (thing.Location.RowType != CellOffsetType.Offset)
					location.Row = Logic.Range.TopLeft.Row;
				thing.Location = location;
				FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			if (!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			int top = 0;
			int left = 0;
			int bottom = 0;
			int right = 0;
			if (thing.First.ColumnType == CellOffsetType.Offset || thing.Last.ColumnType == CellOffsetType.Offset)
				right = IndicesChecker.MaxColumnIndex;
			else {
				left = thing.First.Column;
				right = thing.Last.Column;
			}
			if (thing.First.RowType == CellOffsetType.Offset || thing.Last.RowType == CellOffsetType.Offset)
				bottom = IndicesChecker.MaxRowIndex;
			else {
				top = thing.First.Row;
				bottom = thing.Last.Row;
			}
			CellRange range;
			if (top == 0 && bottom == IndicesChecker.MaxRowIndex)
				range = CellIntervalRange.CreateColumnInterval(DataContext.CurrentWorksheet, left, PositionType.Relative, right, PositionType.Relative);
			else if (left == 0 && right == IndicesChecker.MaxColumnIndex)
				range = CellIntervalRange.CreateRowInterval(DataContext.CurrentWorksheet, top, PositionType.Relative, bottom, PositionType.Relative);
			else
				range = new CellRange(DataContext.CurrentWorksheet, new CellPosition(left, top), new CellPosition(right, bottom));
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
	}
	#endregion
	#region RemovedShiftUpDefinedNameRPNVisitor
	public class RemovedShiftUpDefinedNameRPNVisitor : RemovedShiftLeftDefinedNameRPNVisitor {
		public RemovedShiftUpDefinedNameRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftUpRPNLogic(cellRange);
		}
		protected override bool IsRelative(CellOffset offset) {
			return offset.RowType == CellOffsetType.Offset;
		}
	}
	#endregion
	#region InsertedShiftRightDefinedNameRPNVisitor
	public class InsertedShiftRightDefinedNameRPNVisitor : RemovedShiftLeftDefinedNameRPNVisitor {
		public InsertedShiftRightDefinedNameRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftRightDefinedNameRPNLogic(cellRange);
		}
	}
	#endregion
	#region InsertedShiftDownDefinedNameRPNVisitor
	public class InsertedShiftDownDefinedNameRPNVisitor : RemovedShiftLeftDefinedNameRPNVisitor {
		public InsertedShiftDownDefinedNameRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftDownRPNLogic(cellRange);
		}
	}
	#endregion
}
