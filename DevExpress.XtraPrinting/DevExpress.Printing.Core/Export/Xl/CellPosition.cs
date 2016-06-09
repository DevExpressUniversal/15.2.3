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
using System.Diagnostics;
using System.Text;
namespace DevExpress.Export.Xl {
	#region XlPositionType
	public enum XlPositionType {
		Relative = DevExpress.XtraSpreadsheet.Model.PositionType.Relative,
		Absolute = DevExpress.XtraSpreadsheet.Model.PositionType.Absolute
	}
	#endregion
	#region XlCellPosition
	public struct XlCellPosition {
		#region Statics
		static readonly XlCellPosition invalidValue = new XlCellPosition(-1, -1);
		static readonly XlCellPosition topLeft = new XlCellPosition(0, 0);
		public static XlCellPosition InvalidValue { get { return invalidValue; } }
		public static XlCellPosition TopLeft { get { return topLeft; } }
		public static int MaxColumnCount { get { return 16384; } }
		public static int MaxRowCount { get { return 1048576; } }
		#endregion
		#region Fields
		readonly int column; 
		readonly int row; 
		#endregion
		public XlCellPosition(int column, int row, XlPositionType columnType, XlPositionType rowType) {
			if (column >= MaxColumnCount)
				throw new ArgumentException(string.Format("Column index exceeds the maximum number of columns (16384). The actual value is {0}", column));
			if (row >= MaxRowCount)
				throw new ArgumentException(string.Format("Row index exceeds the maximum number of rows (1048576). The actual value is {0}", row));
			this.column = column < 0 ? -1 : column;
			this.row = row < 0 ? -1 : row;
			if (columnType == XlPositionType.Absolute)
				this.column |= 0x100000;
			if (rowType == XlPositionType.Absolute)
				this.row |= 0x1000000;
		}
		public XlCellPosition(int column, int row)
			: this(column, row, XlPositionType.Relative, XlPositionType.Relative) {
		}
		#region Properties
		public XlPositionType ColumnType { [DebuggerStepThrough] get { return (XlPositionType)((column >> 20) & 1); } }
		public XlPositionType RowType { [DebuggerStepThrough] get { return (XlPositionType)((row >> 24) & 1); } }
		public int Column { [DebuggerStepThrough] get { return column < 0 ? -1 : (column & 0xFFFFF); } } 
		public int Row { [DebuggerStepThrough] get { return row < 0 ? -1 : (row & 0xFFFFFF); } } 
		public bool IsValid { get { return column >= 0 || row >= 0; } }
		public bool IsColumn { get { return column >= 0 && row < 0; } }
		public bool IsRow { get { return row >= 0 && column < 0; } }
		public bool IsColumnOrRow { get { return IsColumn || IsRow; } }
		public bool IsAbsolute {
			get {
				if (IsColumn)
					return ColumnType == XlPositionType.Absolute;
				if (IsRow)
					return RowType == XlPositionType.Absolute;
				return ColumnType == XlPositionType.Absolute && RowType == XlPositionType.Absolute;
			}
		}
		public bool IsRelative {
			get {
				if (IsColumn)
					return ColumnType == XlPositionType.Relative;
				if (IsRow)
					return RowType == XlPositionType.Relative;
				return ColumnType == XlPositionType.Relative && RowType == XlPositionType.Relative;
			}
		}
		#endregion
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			if (!IsRow) {
				int columnIndex = Column + 1;
				if (ColumnType == XlPositionType.Absolute)
					stringBuilder.Append('$');
				int lastPart = columnIndex % 26;
				if (lastPart == 0)
					lastPart = 26;
				if (columnIndex > 702) {
					int middlePart = (columnIndex - lastPart) % 676;
					if (middlePart == 0)
						middlePart = 676;
					stringBuilder.Append((char)('@' + (columnIndex - middlePart - lastPart) / 676));
					stringBuilder.Append((char)('@' + middlePart / 26));
				}
				else if (columnIndex > 26) {
					stringBuilder.Append((char)('@' + (columnIndex - lastPart) / 26));
				}
				stringBuilder.Append((char)('@' + lastPart));
			}
			if (!IsColumn) {
				int rowIndex = Row;
				rowIndex++;
				if (RowType == XlPositionType.Absolute)
					stringBuilder.Append('$');
				stringBuilder.Append(rowIndex);
			}
			return stringBuilder.ToString();
		}
		public override bool Equals(object obj) {
			if (obj is XlCellPosition) {
				XlCellPosition other = (XlCellPosition)obj; 
				return column == other.column && row == other.row;
			}
			else
				return false;
		}
		internal bool EqualsPosition(XlCellPosition otherPosition) {
			return Column == otherPosition.Column && Row == otherPosition.Row;
		}
		public override int GetHashCode() {
			return column ^ row;
		}
		public XlCellPosition AsAbsolute() {
			return new XlCellPosition(Column, Row, XlPositionType.Absolute, XlPositionType.Absolute);
		}
		public XlCellPosition AsRelative() {
			return new XlCellPosition(Column, Row, XlPositionType.Relative, XlPositionType.Relative);
		}
	}
	#endregion
	#region XlCellOffsetType
	public enum XlCellOffsetType {
		Position = 0,
		Offset = 1
	}
	#endregion
	#region XlCellOffset
	public struct XlCellOffset {
		int row;
		int column;
		XlCellOffsetType rowType;
		XlCellOffsetType columnType;
		public int Row { get { return row; } set { row = value; } }
		public int Column { get { return column; } set { column = value; } }
		public XlCellOffsetType RowType { get { return rowType; } set { rowType = value; } }
		public XlCellOffsetType ColumnType { get { return columnType; } set { columnType = value; } }
		public XlCellOffset(int column, int row) {
			this.column = column;
			this.row = row;
			this.columnType = XlCellOffsetType.Position;
			this.rowType = XlCellOffsetType.Position;
		}
		public XlCellOffset(int column, int row, XlCellOffsetType columnType, XlCellOffsetType rowType)
			: this(column, row) {
			this.columnType = columnType;
			this.rowType = rowType;
		}
		public override bool Equals(object obj) {
			if (obj is XlCellOffset) {
				XlCellOffset other = (XlCellOffset)obj;
				return column == other.column && row == other.row && columnType == other.columnType && rowType == other.rowType;
			}
			else
				return false;
		}
		public override int GetHashCode() {
			return column ^ row;
		}
		public XlCellPosition ToCellPosition(IXlExpressionContext context) {
			int rowOffset = context == null ? 1 : context.CurrentCell.Row;
			int columnOffset = context == null ? 0 : context.CurrentCell.Column;
			int column = Column;
			int row = Row;
			if(ColumnType == XlCellOffsetType.Offset)
				column += columnOffset;
			if(RowType == XlCellOffsetType.Offset)
				row += rowOffset;
			if(context != null) {
				if (column < 0)
					column += context.MaxColumnCount;
				if (column >= context.MaxColumnCount)
					column -= context.MaxColumnCount;
				if (row < 0)
					row += context.MaxRowCount;
				if (row >= context.MaxRowCount)
					row -= context.MaxRowCount;
			}
			XlCellPosition position = new XlCellPosition(
				column, row,
				ColumnType == XlCellOffsetType.Offset ? XlPositionType.Relative : XlPositionType.Absolute,
				RowType == XlCellOffsetType.Offset ? XlPositionType.Relative : XlPositionType.Absolute);
			return position;
		}
	}
	#endregion
}
