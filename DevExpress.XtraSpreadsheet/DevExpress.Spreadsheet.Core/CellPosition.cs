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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellPosition
	public struct CellPosition : IEquatable<CellPosition> {
		static readonly CellPosition invalidValue = new CellPosition(-1, -1);
		public static CellPosition InvalidValue { get { return invalidValue; } }
		#region Fields
		readonly int column; 
		readonly int row; 
		#endregion
		public CellPosition(int column, int row, PositionType columnType, PositionType rowType) {
			this.column = column;
			this.row = row;
			if (columnType == PositionType.Absolute)
				this.column |= 0x100000;
			if (rowType == PositionType.Absolute)
				this.row |= 0x1000000;
		}
		public CellPosition(int column, int row) {
			this.column = column;
			this.row = row;
		}
		#region Properties
		public PositionType ColumnType { [DebuggerStepThrough] get { return (PositionType)((this.column >> 20) & 1); } }
		public PositionType RowType { [DebuggerStepThrough] get { return (PositionType)((this.row >> 24) & 1); } }
		public int Column { [DebuggerStepThrough] get { return this.column & 0xFFFFF; } } 
		public int Row { [DebuggerStepThrough] get { return this.row & 0xFFFFFF; } } 
		public bool IsValid { get { return column >= 0 && row >= 0 && IndicesChecker.CheckIsColumnIndexValid(Column) && IndicesChecker.CheckIsRowIndexValid(Row); } }
		public bool IsValidValue { get { return column >= 0 && row >= 0; } }
		public bool IsColumn { get { return column >= 0 && row < 0; } }
		public bool IsRow { get { return row >= 0 && column < 0; } }
		public bool IsColumnOrRow { get { return IsColumn || IsRow; } }
		public bool IsAbsolute { get { return ColumnType == PositionType.Absolute && RowType == PositionType.Absolute; } }
		public bool IsRelative { get { return ColumnType == PositionType.Relative && RowType == PositionType.Relative; } }
		#endregion
		public static CellPosition TryCreate(string reference) {
			return CellReferenceParser.TryParse(reference);
		}
		public bool IsCell(ICell cell) {
			return cell.ColumnIndex == column && cell.RowIndex == row;
		}
		public override string ToString() {
			return CellReferenceParser.ToString(this);
		}
		public string ToString(WorkbookDataContext context) {
			if (context == null)
				return CellReferenceParser.ToString(this);
			else
				return CellReferenceParser.ToString(this, context);
		}
		public string ToString(bool useR1C1, int currentColumn, int currentRow) {
			return CellReferenceParser.ToString(this, useR1C1, currentColumn, currentRow);
		}
		public CellPosition GetRelativeToCurrent(int currentColumnIndex, int currentRowIndex) {
			return CellReferenceParser.GetRelativeToCurrent(this, currentColumnIndex, currentRowIndex);
		}
		public CellPosition AssumeToCurrent(int currentColumnIndex, int currentRowIndex) {
			return CellReferenceParser.AssumeToCurrent(this, currentColumnIndex, currentRowIndex);
		}
		public static void ValidateColumnIndex(int index) {
			if (!IndicesChecker.CheckIsColumnIndexValid(index))
				throw new ArgumentOutOfRangeException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex));
		}
		public static void ValidateRowIndex(int index) {
			if (!IndicesChecker.CheckIsRowIndexValid(index))
				throw new ArgumentOutOfRangeException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex));
		}
		public bool Equals(CellPosition other) {
			return column == other.column && row == other.row;
		}
		public override bool Equals(object obj) {
			if (obj is CellPosition) {
				CellPosition other = (CellPosition)obj;
				return column == other.column && row == other.row;
			}
			else
				return false;
		}
		internal bool EqualsPosition(CellPosition otherPosition) {
			return Column == otherPosition.Column && Row == otherPosition.Row;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(column, row) ;
		}
		public static CellPosition UnionPosition(CellPosition firstPosition, CellPosition secondPosition, bool getSmallest) {
			int column = (firstPosition.Column < secondPosition.Column) == getSmallest ? firstPosition.Column : secondPosition.Column;
			int row = (firstPosition.Row < secondPosition.Row) == getSmallest ? firstPosition.Row : secondPosition.Row;
			PositionType rowPosition = firstPosition.RowType & secondPosition.RowType;
			PositionType columnPosition = firstPosition.ColumnType & secondPosition.ColumnType;
			return new CellPosition(column, row, columnPosition, rowPosition);
		}
		internal CellPosition GetShifted(CellPositionOffset offset, ICellTable worksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column;
			if (ColumnType == PositionType.Relative)
				column += offset.ColumnOffset;
			int row = Row;
			if (RowType == PositionType.Relative)
				row += offset.RowOffset;
			if (!IsValidInWorksheet(column, row, worksheet))
				return CellPosition.InvalidValue;
			return new CellPosition(column, row, ColumnType, RowType);
		}
		internal CellPosition GetShiftedAbsolute(CellPositionOffset offset, ICellTable worksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column;
			if (ColumnType == PositionType.Absolute)
				column += offset.ColumnOffset;
			int row = Row;
			if (RowType == PositionType.Absolute)
				row += offset.RowOffset;
			if (!IsValidInWorksheet(column, row, worksheet))
				return CellPosition.InvalidValue;
			return new CellPosition(column, row, ColumnType, RowType);
		}
		internal CellPosition GetShiftedAny(CellPositionOffset offset, ICellTable worksheet) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column + offset.ColumnOffset;
			int row = Row + offset.RowOffset;
			if (!IsValidInWorksheet(column, row, worksheet))
				return CellPosition.InvalidValue;
			return new CellPosition(column, row, ColumnType, RowType);
		}
		public static bool IsValidInWorksheet(int column, int row, ICellTable worksheet) {
			if (column < 0 || column >= worksheet.MaxColumnCount)
				return false;
			if (row < 0 || row >= worksheet.MaxRowCount)
				return false;
			return true;
		}
		public CellOffset ToCellOffset(WorkbookDataContext context) {
			return context != null ? ToCellOffset(context.CurrentColumnIndex, context.CurrentRowIndex) : ToCellOffset(0, 0);
		}
		public CellOffset ToCellOffset() {
			return ToCellOffset(0, 0);
		}
		public CellOffset ToCellOffset(int currentColumnIndex, int currentRowIndex) {
			CellOffsetType rowType = RowType == PositionType.Absolute ? CellOffsetType.Position : CellOffsetType.Offset;
			CellOffsetType columnType = ColumnType == PositionType.Absolute ? CellOffsetType.Position : CellOffsetType.Offset;
			int row_index = Row;
			int column_index = Column;
			if (rowType == CellOffsetType.Offset)
				row_index -= currentRowIndex;
			if (columnType == CellOffsetType.Offset)
				column_index -= currentColumnIndex;
			return new CellOffset(column_index, row_index, columnType, rowType);
		}
		public CellPosition AsAbsolute() {
			return new CellPosition(Column, Row, PositionType.Absolute, PositionType.Absolute);
		}
		public CellPosition AsRelative() {
			return new CellPosition(Column, Row, PositionType.Relative, PositionType.Relative);
		}
		public CellKey ToKey(int sheetId) {
			return new CellKey(sheetId, Column, Row);
		}
	}
	#endregion
	#region CellPositionOffset
	public struct CellPositionOffset {
		static readonly CellPositionOffset none = new CellPositionOffset(0, 0);
		readonly int columnOffset;
		readonly int rowOffset;
		public CellPositionOffset(int columnOffset, int rowOffset) {
			this.columnOffset = columnOffset;
			this.rowOffset = rowOffset;
		}
		public CellPositionOffset(CellPosition basePosition, CellPosition position) {
			this.columnOffset = position.Column - basePosition.Column;
			this.rowOffset = position.Row - basePosition.Row;
		}
		public static CellPositionOffset None { get { return none; } }
		public int ColumnOffset { get { return columnOffset; } }
		public int RowOffset { get { return rowOffset; } }
	}
	#endregion
	#region CellLocation
	public enum CellOffsetType {
		Position = 0,
		Offset = 1
	}
	public struct CellOffset {
		int row;
		int column;
		CellOffsetType rowType;
		CellOffsetType columnType;
		public int Row { get { return row; } set { row = value; } }
		public int Column { get { return column; } set { column = value; } }
		public CellOffsetType RowType { get { return rowType; } set { rowType = value; } }
		public CellOffsetType ColumnType { get { return columnType; } set { columnType = value; } }
		public CellOffset(int column, int row) {
			this.column = column;
			this.row = row;
			this.columnType = CellOffsetType.Position;
			this.rowType = CellOffsetType.Position;
		}
		public CellOffset(int column, int row, CellOffsetType columnType, CellOffsetType rowType)
			: this(column, row) {
			this.columnType = columnType;
			this.rowType = rowType;
		}
		public override bool Equals(object obj) {
			if (obj is CellOffset) {
				CellOffset other = (CellOffset)obj;
				return column == other.column && row == other.row && columnType == other.columnType && rowType == other.rowType;
			}
			else
				return false;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(column, row);
		}
		public CellPosition ToCellPosition(WorkbookDataContext context) {
			PositionType columnType = ColumnType == CellOffsetType.Offset ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = RowType == CellOffsetType.Offset ? PositionType.Relative : PositionType.Absolute;
			int row_index = row;
			int column_index = column;
			if (context != null) {
				if (columnType == PositionType.Relative) {
					column_index += context.CurrentColumnIndex;
					if (column_index < 0)
						column_index += context.MaxColumnCount;
					if (column_index >= context.MaxColumnCount)
						column_index -= context.MaxColumnCount;
				}
				if (rowType == PositionType.Relative) {
					row_index += context.CurrentRowIndex;
					if (row_index < 0)
						row_index += context.MaxRowCount;
					if (row_index >= context.MaxRowCount)
						row_index -= context.MaxRowCount;
				}
			}
			return new CellPosition(column_index, row_index, columnType, rowType);
		}
		public CellPosition ToCellPositionWithoutCorrection(WorkbookDataContext context) {
			PositionType columnType = ColumnType == CellOffsetType.Offset ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = RowType == CellOffsetType.Offset ? PositionType.Relative : PositionType.Absolute;
			int row_index = row;
			int column_index = column;
			if (columnType == PositionType.Relative && context != null) {
				column_index += context.CurrentColumnIndex;
				if (column_index < 0 || column_index >= context.MaxColumnCount)
					return CellPosition.InvalidValue;
			}
			if (rowType == PositionType.Relative && context != null) {
				row_index += context.CurrentRowIndex;
				if (row_index < 0 || row_index >= context.MaxRowCount)
					return CellPosition.InvalidValue;
			}
			return new CellPosition(column_index, row_index, columnType, rowType);
		}
		public CellPosition ToCellPosition() {
			return ToCellPosition(null);
		}
		internal CellOffset GetShiftedOffsetOnly(CellPositionOffset offset) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column;
			if (ColumnType == CellOffsetType.Offset)
				column += offset.ColumnOffset;
			int row = Row;
			if (RowType == CellOffsetType.Offset)
				row += offset.RowOffset;
			return new CellOffset(column, row, ColumnType, RowType);
		}
		internal CellOffset GetShiftedCellOffsetPositionOnly(CellPositionOffset offset) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column;
			if (ColumnType == CellOffsetType.Position)
				column += offset.ColumnOffset;
			int row = Row;
			if (RowType == CellOffsetType.Position)
				row += offset.RowOffset;
			return new CellOffset(column, row, ColumnType, RowType);
		}
		internal CellOffset GetShiftedAny(CellPositionOffset offset) {
			if (offset.ColumnOffset == 0 && offset.RowOffset == 0)
				return this;
			int column = Column;
			column += offset.ColumnOffset;
			int row = Row;
			row += offset.RowOffset;
			return new CellOffset(column, row, ColumnType, RowType);
		}
	}
	#endregion
}
