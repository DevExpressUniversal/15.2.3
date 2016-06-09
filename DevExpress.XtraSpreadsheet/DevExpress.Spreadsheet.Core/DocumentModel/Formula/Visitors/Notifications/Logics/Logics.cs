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
	#region RemoveShiftLeftRPNLogic
	public class RemoveShiftLeftRPNLogic {
		#region static
		protected static CellPosition OffsetCellPostion(CellPosition original, int columnOffset, int rowOffset) {
			return new CellPosition(original.Column + columnOffset, original.Row + rowOffset, original.ColumnType, original.RowType);
		}
		#endregion
		#region Fields
		CellRange deletedRange;
		CellRange range;
		CellIntervalRange deletedIntervalRange;
		CellIntervalRange intervalRange;
		CellPosition startPosition;
		CellPosition endPosition;
		CellPosition startProcessedPosition;
		CellPosition endProcessedPosition;
		#endregion
		public RemoveShiftLeftRPNLogic(CellRange modyfingRange) {
			this.deletedRange = modyfingRange;
		}
		#region Properties
		bool IsColumnRange { get { return startPosition.Column == endPosition.Column; } }
		bool IsRowRange { get { return startPosition.Row == endPosition.Row; } }
		bool StartPositionNotChanged { get { return object.Equals(startPosition, startProcessedPosition); } }
		bool EndPositionNotChanged { get { return object.Equals(endPosition, endProcessedPosition); } }
		protected CellPosition StartProcessedPosition { get { return startProcessedPosition; } set { startProcessedPosition = value; } }
		protected CellPosition EndProcessedPosition { get { return endProcessedPosition; } set { endProcessedPosition = value; } }
		protected CellPosition StartPosition { get { return startPosition; } }
		protected CellPosition EndPosition { get { return endPosition; } }
		protected internal CellRange ModifyingRange { get { return deletedRange; } }
		protected CellIntervalRange DeletedIntervalRange { get { return deletedIntervalRange; } }
		protected CellIntervalRange IntervalRange { get { return intervalRange; } }
		protected internal virtual bool RemoveBehavior { get { return true; } }
		protected internal CellRange Range { get { return range; } }
		protected internal virtual bool IsProcessingByColumns { get { return true; } }
		#endregion
		public void SetRange(CellRange range) {
			this.range = range;
		}
		public bool ProcessRange() {
			if (range.Worksheet.SheetId != deletedRange.Worksheet.SheetId)
				return false;
			if (range.RangeType == CellRangeType.IntervalRange) {
				intervalRange = range as CellIntervalRange;
				deletedIntervalRange = deletedRange as CellIntervalRange;
				return ProcessIntervalRange();
			}
			else return ProcessSingleRange();
		}
		bool ProcessSingleRange() {
			ProcessPositions();
			if (StartPositionNotChanged && EndPositionNotChanged)
				return false;
			if (IsColumnRange)
				FindColumnRangePositions();
			else if (IsRowRange)
				FindRowRangePositions();
			else FindRectangleRangePositions();
			range.TopLeft = StartProcessedPosition;
			range.BottomRight = EndProcessedPosition;
			return true;
		}
		bool ProcessIntervalRange() {
			CellIntervalRange intervalRange = range as CellIntervalRange;
			if (ModifyingRange.RangeType != CellRangeType.IntervalRange)
				return false;
			if (intervalRange.IsColumnInterval)
				return ProcessColumnInterval();
			else if (intervalRange.IsRowInterval)
				return ProcessRowInterval();
			return true;
		}
		void ProcessPositions() {
			startPosition = range.TopLeft;
			endPosition = range.BottomRight;
			StartProcessedPosition = startPosition;
			EndProcessedPosition = endPosition;
			if (RemoveBehavior)
				CorrectRemovedPositions();
			StartProcessedPosition = ProcessCellPosition(StartProcessedPosition);
			EndProcessedPosition = ProcessCellPosition(EndProcessedPosition);
		}
		protected virtual void CorrectRemovedPositions() {
			if (ModifyingRange.TopLeft.Row <= startPosition.Row && ModifyingRange.BottomRight.Row >= EndPosition.Row) {
				if (ModifyingRange.BottomRight.Column >= startPosition.Column && ModifyingRange.TopLeft.Column <= startPosition.Column)
					StartProcessedPosition = new CellPosition(ModifyingRange.BottomRight.Column + 1, startPosition.Row, startPosition.ColumnType, startPosition.RowType);
				if (ModifyingRange.BottomRight.Column >= endPosition.Column && ModifyingRange.TopLeft.Column <= endPosition.Column)
					EndProcessedPosition = new CellPosition(ModifyingRange.TopLeft.Column - 1, endPosition.Row, endPosition.ColumnType, endPosition.RowType);
			}
			if (ModifyingRange.TopLeft.Column <= startPosition.Column && ModifyingRange.BottomRight.Column >= endPosition.Column) {
				if (ModifyingRange.BottomRight.Row >= startPosition.Row && ModifyingRange.TopLeft.Row <= startPosition.Row)
					StartProcessedPosition = new CellPosition(startPosition.Column, ModifyingRange.BottomRight.Row + 1, startPosition.ColumnType, startPosition.RowType);
				if (ModifyingRange.BottomRight.Row >= endPosition.Row && ModifyingRange.TopLeft.Row <= endPosition.Row)
					EndProcessedPosition = new CellPosition(endPosition.Column, ModifyingRange.TopLeft.Row - 1, endPosition.ColumnType, endPosition.RowType);
			}
		}
		protected internal virtual CellPosition ProcessCellPosition(CellPosition position) {
			int columnOffset = deletedRange.GetCellPositionColumnOffset(position, RemoveCellMode.ShiftCellsLeft);
			if (columnOffset != 0)
				return new CellPosition(position.Column - columnOffset, position.Row, position.ColumnType, position.RowType);
			return position;
		}
		void FindColumnRangePositions() {
			if (!StartPositionNotChanged && !EndPositionNotChanged)
				return;
			if (!StartPositionNotChanged)
				FindStartColumnPosition();
			if (!EndPositionNotChanged)
				FindEndColumnPosition();
		}
		protected virtual void FindStartColumnPosition() {
			if (deletedRange.BottomRight.Column == startPosition.Column)
				StartProcessedPosition = new CellPosition(startPosition.Column, deletedRange.BottomRight.Row + 1, startPosition.ColumnType, startPosition.RowType);
			else if (deletedRange.BottomRight.Column < startPosition.Column)
				StartProcessedPosition = OffsetCellPostion(startPosition, 0, 0);
		}
		protected virtual void FindEndColumnPosition() {
			if (deletedRange.BottomRight.Column < endPosition.Column)
				EndProcessedPosition = OffsetCellPostion(endPosition, 0, 0); ;
		}
		void FindRowRangePositions() {
			if (!StartPositionNotChanged && !EndPositionNotChanged)
				return;
			if (!StartPositionNotChanged)
				FindStartRowPosition();
			if (!EndPositionNotChanged)
				FindEndRowPosition();
		}
		protected virtual void FindEndRowPosition() {
		}
		protected virtual void FindStartRowPosition() {
		}
		void FindRectangleRangePositions() {
			if (StartPositionNotChanged || EndPositionNotChanged) {
				if ((!StartPositionNotChanged || !EndPositionNotChanged) && PositionMustChange())
					return;
				StartProcessedPosition = startPosition;
				EndProcessedPosition = endPosition;
			}
		}
		protected virtual bool PositionMustChange() {
			VariantValue intersection = range.IntersectionWith(deletedRange);
			if (intersection == VariantValue.ErrorNullIntersection)
				return false;
			CellRangeBase intersectionRange = intersection.CellRangeValue;
			return intersectionRange.Height == range.Height || (intersectionRange.Width == range.Width && 
				   (intersectionRange.TopLeft.Row == range.TopLeft.Row || intersectionRange.BottomRight.Row == range.BottomRight.Row));
		}
		protected virtual bool ProcessColumnInterval() {
			if (deletedIntervalRange.IsRowInterval)
				return false;
			int deletedLeftColumn = deletedIntervalRange.TopLeft.Column;
			int deletedRightColumn = deletedIntervalRange.BottomRight.Column;
			int leftColumn = intervalRange.TopLeft.Column;
			int rightColumn = intervalRange.BottomRight.Column;
			int offset = deletedIntervalRange.Width;
			if (deletedLeftColumn > rightColumn)
				return false;
			if (deletedRightColumn < leftColumn) {
				intervalRange.TopLeft = OffsetCellPostion(intervalRange.TopLeft, -offset, 0);
				intervalRange.BottomRight = OffsetCellPostion(intervalRange.BottomRight, -offset, 0);
				return true;
			}
			if (deletedLeftColumn < leftColumn && deletedRightColumn >= leftColumn) {
				int deletedOffset = deletedRightColumn - leftColumn + 1 - offset;
				intervalRange.TopLeft = OffsetCellPostion(intervalRange.TopLeft, deletedOffset, 0);
				intervalRange.BottomRight = OffsetCellPostion(intervalRange.BottomRight, -offset, 0);
				return true;
			}
			if (deletedLeftColumn >= leftColumn && deletedRightColumn <= rightColumn) {
				int width = intervalRange.Width - deletedIntervalRange.Width;
				if (width == 0) return false;
				if (deletedLeftColumn == leftColumn)
					intervalRange.TopLeft = OffsetCellPostion(intervalRange.TopLeft, deletedIntervalRange.Width - offset, 0);
				intervalRange.BottomRight = new CellPosition(leftColumn + width - 1, intervalRange.BottomRight.Row, intervalRange.BottomRight.ColumnType, intervalRange.BottomRight.RowType);
				return true;
			}
			if (deletedLeftColumn <= rightColumn && deletedRightColumn > rightColumn) {
				intervalRange.BottomRight = new CellPosition(deletedLeftColumn - 1, intervalRange.BottomRight.Row, intervalRange.BottomRight.ColumnType, intervalRange.BottomRight.RowType);
				return true;
			}
			return false;
		}
		protected virtual bool ProcessRowInterval() {
			if (deletedIntervalRange.IsColumnInterval)
				return false;
			return false;
		}
		protected internal virtual bool IsResizingRange(CellRange range) {
			if (ModifyingRange.Intersects(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(range.BottomRight.Column, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Height != prohibitedRange.Height;
			return false;
		}
		protected internal virtual bool IsMovingRange(CellRange range) {
			if (ModifyingRange.Intersects(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(range.TopLeft.Column - 1, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Height == prohibitedRange.Height;
			return false;
		}
		protected internal virtual CellRange GetChangingRange() {
			CellRange modifyingRange = ModifyingRange;
			CellPosition topLeft = modifyingRange.TopLeft;
			CellPosition bottomRight = new CellPosition(IndicesChecker.MaxColumnIndex, modifyingRange.BottomRowIndex);
			return new CellRange(modifyingRange.Worksheet, topLeft, bottomRight);
		}
	}
	#endregion
	#region RemoveShiftUpRPNLogic
	public class RemoveShiftUpRPNLogic : RemoveShiftLeftRPNLogic {
		public RemoveShiftUpRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override bool IsProcessingByColumns { get { return false; } }
		protected internal override CellPosition ProcessCellPosition(CellPosition position) {
			int rowOffset = ModifyingRange.GetCellPositionRowOffset(position, RemoveCellMode.ShiftCellsUp);
			if (rowOffset != 0)
				return new CellPosition(position.Column, position.Row - rowOffset, position.ColumnType, position.RowType);
			return position;
		}
		protected override bool ProcessRowInterval() {
			if (DeletedIntervalRange.IsColumnInterval)
				return false;
			int deletedTopRow = DeletedIntervalRange.TopLeft.Row;
			int deletedBottomRow = DeletedIntervalRange.BottomRight.Row;
			int topRow = IntervalRange.TopLeft.Row;
			int bottomRow = IntervalRange.BottomRight.Row;
			int offset = DeletedIntervalRange.Height;
			if (deletedTopRow > bottomRow)
				return false;
			if (deletedBottomRow < topRow) {
				IntervalRange.TopLeft = OffsetCellPostion(IntervalRange.TopLeft, 0, -offset);
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, 0, -offset);
				return true;
			}
			if (deletedTopRow < topRow && deletedBottomRow >= topRow) {
				int deletedOffset = deletedBottomRow - topRow + 1 - offset;
				IntervalRange.TopLeft = OffsetCellPostion(IntervalRange.TopLeft, 0, deletedOffset);
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, 0, -offset);
				return true;
			}
			if (deletedTopRow >= topRow && deletedBottomRow <= bottomRow) {
				int height = IntervalRange.Height - DeletedIntervalRange.Height;
				if (height == 0) return false;
				if (deletedTopRow == topRow)
					IntervalRange.TopLeft = OffsetCellPostion(IntervalRange.TopLeft, 0, DeletedIntervalRange.Height - offset);
				IntervalRange.BottomRight = new CellPosition(IntervalRange.BottomRight.Column, topRow + height - 1, IntervalRange.BottomRight.ColumnType, IntervalRange.BottomRight.RowType);
				return true;
			}
			if (deletedTopRow <= bottomRow && deletedBottomRow > bottomRow) {
				IntervalRange.BottomRight = new CellPosition(IntervalRange.BottomRight.Column, deletedTopRow - 1, IntervalRange.BottomRight.ColumnType, IntervalRange.BottomRight.RowType);
				return true;
			}
			return false;
		}
		protected override void CorrectRemovedPositions() {
			int modifyingLeftColumn = ModifyingRange.LeftColumnIndex;
			int modifyingRightColumn = ModifyingRange.RightColumnIndex;
			int modifyingTopRow = ModifyingRange.TopRowIndex;
			int modifyingBottomRow = ModifyingRange.BottomRowIndex;
			int startColumn = StartPosition.Column;
			int endColumn = EndPosition.Column;
			int startRow = StartPosition.Row;
			int endRow = EndPosition.Row;
			if (modifyingLeftColumn <= startColumn && modifyingRightColumn >= endColumn) {
				if (modifyingBottomRow >= startRow && modifyingTopRow <= startRow)
					StartProcessedPosition = new CellPosition(startColumn, modifyingBottomRow + 1, StartPosition.ColumnType, StartPosition.RowType);
				if (modifyingBottomRow >= endRow && modifyingTopRow <= endRow)
					EndProcessedPosition = new CellPosition(endColumn, modifyingTopRow - 1, EndPosition.ColumnType, EndPosition.RowType);
			}
			if (modifyingTopRow <= startRow && modifyingBottomRow >= endRow) {
				if (modifyingRightColumn >= startColumn && modifyingLeftColumn <= startColumn)
					StartProcessedPosition = new CellPosition(modifyingRightColumn + 1, startRow, StartPosition.ColumnType, StartPosition.RowType);
				if (modifyingRightColumn >= endColumn && modifyingLeftColumn <= endColumn)
					EndProcessedPosition = new CellPosition(modifyingLeftColumn - 1, endRow, EndPosition.ColumnType, EndPosition.RowType);
			}
		}
		protected override bool PositionMustChange() {
			VariantValue intersection = Range.IntersectionWith(ModifyingRange);
			if (intersection == VariantValue.ErrorNullIntersection)
				return false;
			CellRangeBase intersectionRange = intersection.CellRangeValue;
			return intersectionRange.Width == Range.Width || (intersectionRange.Height == Range.Height &&
				   (intersectionRange.TopLeft.Column == Range.TopLeft.Column || intersectionRange.BottomRight.Column == Range.BottomRight.Column));
		}
		protected override void FindEndColumnPosition() {
		}
		protected override void FindStartRowPosition() {
			if (ModifyingRange.BottomRight.Row == StartPosition.Row)
				StartProcessedPosition = new CellPosition(ModifyingRange.BottomRight.Column + 1, StartPosition.Row, StartPosition.ColumnType, StartPosition.ColumnType);
			else if (ModifyingRange.BottomRight.Row < StartPosition.Row)
				StartProcessedPosition = OffsetCellPostion(StartPosition, 0, 0);
		}
		protected override void FindEndRowPosition() {
			if (ModifyingRange.BottomRight.Row == EndPosition.Row)
				EndProcessedPosition = new CellPosition(ModifyingRange.TopLeft.Column - 1, EndPosition.Row, EndPosition.ColumnType, EndPosition.ColumnType);
			else if (ModifyingRange.BottomRight.Row < EndPosition.Row)
				EndProcessedPosition = OffsetCellPostion(EndPosition, 0, 0);
		}
		protected internal override bool IsResizingRange(CellRange range) {
			if (ModifyingRange.Intersects(range))
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, 0), new CellPosition(range.BottomRight.Column, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Width != prohibitedRange.Width;
			return false;
		}
		protected internal override CellRange GetChangingRange() {
			CellRange modifyingRange = ModifyingRange;
			CellPosition topLeft = modifyingRange.TopLeft;
			CellPosition bottomRight = new CellPosition(modifyingRange.RightColumnIndex, IndicesChecker.MaxRowIndex);
			return new CellRange(modifyingRange.Worksheet, topLeft, bottomRight);
		}
	}
	#endregion
	public class RemoveNoShiftRPNLogic : RemoveShiftUpRPNLogic {
		public RemoveNoShiftRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		protected internal override CellPosition ProcessCellPosition(CellPosition position) {
			return position;
		}
		protected override bool PositionMustChange() {
			return true;
		}
		protected override bool ProcessRowInterval() {
			return false;
		}
		protected override void CorrectRemovedPositions() {
			CellPosition topLeft = Range.TopLeft;
			CellPosition bottomRight = Range.BottomRight;
			CellPosition modifyingTopLeft = ModifyingRange.TopLeft;
			CellPosition modifyingBottomRight = ModifyingRange.BottomRight;
			if (ModifyingRange.Height >= Range.Height && modifyingTopLeft.Row <= topLeft.Row && modifyingBottomRight.Row >= bottomRight.Row) {
				if (modifyingTopLeft.Column < topLeft.Column && modifyingBottomRight.Column >= StartPosition.Column)
					StartProcessedPosition = new CellPosition(modifyingBottomRight.Column + 1, StartPosition.Row, StartPosition.ColumnType, StartPosition.RowType);
				if (modifyingBottomRight.Column > bottomRight.Column && modifyingTopLeft.Column <= EndPosition.Column)
					EndProcessedPosition = new CellPosition(modifyingTopLeft.Column - 1, EndPosition.Row, EndPosition.ColumnType, EndPosition.RowType);
			}
			if (ModifyingRange.Width >= Range.Width && ModifyingRange.TopLeft.Column <= Range.TopLeft.Column && ModifyingRange.BottomRight.Column >= Range.BottomRight.Column) {
				if (modifyingTopLeft.Row < topLeft.Row && modifyingBottomRight.Row >= StartPosition.Row)
					StartProcessedPosition = new CellPosition(StartPosition.Column, modifyingBottomRight.Row + 1, StartPosition.ColumnType, StartPosition.RowType);
				if (modifyingBottomRight.Row > bottomRight.Row && modifyingTopLeft.Row <= EndPosition.Row)
					EndProcessedPosition = new CellPosition(EndPosition.Column, modifyingTopLeft.Row - 1, EndPosition.ColumnType, EndPosition.RowType);
			}
		}
		protected internal override CellRange GetChangingRange() {
			return ModifyingRange;
		}
	}
	#region InsertShiftRightRPNLogic
	public class InsertShiftRightRPNLogic : RemoveShiftLeftRPNLogic {
		public InsertShiftRightRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		#region properties
		protected virtual int InsertCorection { get { return 1; } }
		protected internal override bool RemoveBehavior { get { return false; } }
		#endregion
		protected override bool ProcessColumnInterval() {
			CellIntervalRange insertedRange = ModifyingRange as CellIntervalRange;
			if (insertedRange.IsRowInterval)
				return false;
			int insertedLeftColumn = insertedRange.TopLeft.Column;
			int leftColumn = IntervalRange.TopLeft.Column;
			int rightColumn = IntervalRange.BottomRight.Column;
			int offset = insertedRange.Width;
			if (ModyfingStartAfterChanged(insertedLeftColumn, rightColumn + InsertCorection))
				return false;
			if (ModyfingStartBeforeChanged(insertedLeftColumn, leftColumn + InsertCorection)) {
				IntervalRange.TopLeft = OffsetCellPostion(IntervalRange.TopLeft, offset, 0);
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, offset, 0);
				return true;
			}
			if (ModyfingStartInChanged(insertedLeftColumn, leftColumn + InsertCorection)) {
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, offset, 0);
				return true;
			}
			return false;
		}
		protected virtual bool ModyfingStartAfterChanged(int insertedLeftColumn, int rightColumn) {
			return insertedLeftColumn >= rightColumn;
		}
		protected virtual bool ModyfingStartBeforeChanged(int insertedLeftColumn, int leftColumn) {
			return insertedLeftColumn < leftColumn;
		}
		protected virtual bool ModyfingStartInChanged(int insertedLeftColumn, int leftColumn) {
			return insertedLeftColumn >= leftColumn;
		}
		protected internal override CellPosition ProcessCellPosition(CellPosition position) {
			int columnOffset = ModifyingRange.GetCellPositionColumnOffset(position, InsertCellMode.ShiftCellsRight);
			if (columnOffset != 0)
				return new CellPosition(position.Column + columnOffset, position.Row, position.ColumnType, position.RowType);
			return position;
		}
		protected override void FindStartColumnPosition() {
			StartProcessedPosition = StartPosition;
		}
		protected override void FindEndColumnPosition() {
			EndProcessedPosition = EndPosition;
		}
		protected override bool PositionMustChange() {
			VariantValue valueRange = Range.IntersectionWith(ModifyingRange);
			if (valueRange != VariantValue.ErrorNullIntersection && valueRange.CellRangeValue.Height == Range.Height)
				return true;
			return false;
		}
		protected internal override bool IsResizingRange(CellRange range) {
			if (ModifyingRange.Intersects(range) && ModifyingRange.TopLeft.Column >= range.TopLeft.Column + 1)
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(range.TopLeft.Column + 1, range.BottomRight.Row));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Height != prohibitedRange.Height;
			return false;
		}
	}
	#endregion
	#region InsertShiftDownRPNLogic
	public class InsertShiftDownRPNLogic : RemoveShiftUpRPNLogic {
		public InsertShiftDownRPNLogic(CellRange modyfingRange)
			: base(modyfingRange) {
		}
		#region properties
		protected internal override bool RemoveBehavior { get { return false; } }
		#endregion
		protected internal override CellPosition ProcessCellPosition(CellPosition position) {
			int rowOffset = ModifyingRange.GetCellPositionRowOffset(position, InsertCellMode.ShiftCellsDown);
			if (rowOffset != 0)
				return new CellPosition(position.Column, position.Row + rowOffset, position.ColumnType, position.RowType);
			return position;
		}
		protected override bool ProcessRowInterval() {
			CellIntervalRange insertedRange = ModifyingRange as CellIntervalRange;
			if (insertedRange.IsColumnInterval)
				return false;
			int insertedTopRow = insertedRange.TopLeft.Row;
			int topRow = IntervalRange.TopLeft.Row;
			int bottomRow = IntervalRange.BottomRight.Row;
			int offset = insertedRange.Height;
			if (ModyfingStartAfterChanged(insertedTopRow, bottomRow))
				return false;
			if (ModyfingStartBeforeChanged(insertedTopRow, topRow)) {
				IntervalRange.TopLeft = OffsetCellPostion(IntervalRange.TopLeft, 0, offset);
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, 0, offset);
				return true;
			}
			if (ModyfingStartInChanged(insertedTopRow, topRow)) {
				IntervalRange.BottomRight = OffsetCellPostion(IntervalRange.BottomRight, 0, offset);
				return true;
			}
			return false;
		}
		protected virtual bool ModyfingStartAfterChanged(int insertedTopRow, int bottomRow) {
			return insertedTopRow > bottomRow;
		}
		protected virtual bool ModyfingStartBeforeChanged(int insertedTopRow, int topRow) {
			return insertedTopRow <= topRow;
		}
		protected virtual bool ModyfingStartInChanged(int insertedTopRow, int topRow) {
			return insertedTopRow > topRow;
		}
		protected override void FindStartRowPosition() {
			StartProcessedPosition = StartPosition;
		}
		protected override void FindEndRowPosition() {
			EndProcessedPosition = EndPosition;
		}
		protected override bool PositionMustChange() {
			VariantValue valueRange = Range.IntersectionWith(ModifyingRange);
			if (valueRange != VariantValue.ErrorNullIntersection && valueRange.CellRangeValue.Width == Range.Width)
				return true;
			return false;
		}
		protected internal override bool IsResizingRange(CellRange range) {
			if (ModifyingRange.Intersects(range) && ModifyingRange.TopLeft.Row >= range.TopLeft.Row + 1)
				return true;
			CellRange prohibitedRange = new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, 0), new CellPosition(range.BottomRight.Column, range.TopLeft.Row + 1));
			if (prohibitedRange.Intersects(ModifyingRange))
				return ModifyingRange.IntersectionWith(prohibitedRange).CellRangeValue.Width != prohibitedRange.Width;
			return false;
		}
	}
	#endregion
}
