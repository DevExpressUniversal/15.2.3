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
using System.Globalization;
namespace DevExpress.Export.Xl {
	#region IXlCellRange
	public interface IXlCellRange {
		XlCellPosition TopLeft { get; }
		XlCellPosition BottomRight { get; }
	}
	#endregion
	#region XlCellRange
	public class XlCellRange : IXlFormulaParameter, IXlCellRange {
		string sheetName;
		XlCellPosition topLeft;
		XlCellPosition bottomRight;
		public XlCellRange(string sheetName, XlCellPosition topLeft, XlCellPosition bottomRight) {
			if ((topLeft.IsColumn && bottomRight.IsRow) || (topLeft.IsRow && bottomRight.IsColumn))
				throw new ArgumentException("Invalid or incompatible range positions.");
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
			this.sheetName = sheetName;
			Normalize();
		}
		public XlCellRange(XlCellPosition topLeft, XlCellPosition bottomRight)
			: this(String.Empty, topLeft, bottomRight) {
		}
		public XlCellRange(XlCellPosition singleCell) {
			this.topLeft = new XlCellPosition(singleCell.Column, singleCell.Row, singleCell.ColumnType, singleCell.RowType);
			this.bottomRight = new XlCellPosition(singleCell.Column, singleCell.Row, singleCell.ColumnType, singleCell.RowType);
			this.sheetName = String.Empty;
			Normalize();
		}
		#region Properties
		public XlCellPosition TopLeft { [DebuggerStepThrough] get { return topLeft; } set { topLeft = value; } }
		public XlCellPosition BottomRight { [DebuggerStepThrough] get { return bottomRight; } set { bottomRight = value; } }
		public string SheetName {
			[DebuggerStepThrough]
			get { return sheetName; }
			set {
				if (string.IsNullOrEmpty(value))
					sheetName = string.Empty;
				else
					sheetName = value;
			}
		}
		internal int RowCount { get { return bottomRight.Row - topLeft.Row + 1; } }
		internal int ColumnCount { get { return bottomRight.Column - topLeft.Column + 1; } }
		internal int FirstRow { get { return topLeft.IsColumn ? 0 : topLeft.Row; } }
		internal int LastRow { get { return bottomRight.IsColumn ? 1048575 : bottomRight.Row; } }
		internal int FirstColumn { get { return topLeft.IsRow ? 0 : topLeft.Column; } }
		internal int LastColumn { get { return bottomRight.IsRow ? 16383 : bottomRight.Column; } }
		#endregion
		public static XlCellRange Parse(string reference) {
			return XlRangeReferenceParser.Parse(reference);
		}
		public static XlCellRange FromLTRB(int left, int top, int right, int bottom) {
			return new XlCellRange(new XlCellPosition(left, top), new XlCellPosition(right, bottom));
		}
		public static XlCellRange ColumnInterval(int left, int right) {
			return new XlCellRange(new XlCellPosition(left, -1), new XlCellPosition(right, -1));
		}
		public static XlCellRange ColumnInterval(int left, int right, XlPositionType columnType) {
			return new XlCellRange(new XlCellPosition(left, -1, columnType, XlPositionType.Absolute), new XlCellPosition(right, -1, columnType, XlPositionType.Absolute));
		}
		public static XlCellRange RowInterval(int top, int bottom) {
			return new XlCellRange(new XlCellPosition(-1, top), new XlCellPosition(-1, bottom));
		}
		public static XlCellRange RowInterval(int top, int bottom, XlPositionType rowType) {
			return new XlCellRange(new XlCellPosition(-1, top, XlPositionType.Absolute, rowType), new XlCellPosition(-1, bottom, XlPositionType.Absolute, rowType));
		}
		void Normalize() {
			if (topLeft.Row > bottomRight.Row) {
				int topLeftRow = bottomRight.Row;
				XlPositionType topLeftType = bottomRight.RowType;
				int bottomRightRow = topLeft.Row;
				XlPositionType bottomRightType = topLeft.RowType;
				topLeft = new XlCellPosition(topLeft.Column, topLeftRow, topLeft.ColumnType, topLeftType);
				bottomRight = new XlCellPosition(bottomRight.Column, bottomRightRow, bottomRight.ColumnType, bottomRightType);
			}
			if (topLeft.Column > bottomRight.Column) {
				int topLeftColumn = bottomRight.Column;
				XlPositionType topLeftType = bottomRight.ColumnType;
				int bottomRightColumn = topLeft.Column;
				XlPositionType bottomRightType = topLeft.ColumnType;
				topLeft = new XlCellPosition(topLeftColumn, topLeft.Row, topLeftType, topLeft.RowType);
				bottomRight = new XlCellPosition(bottomRightColumn, bottomRight.Row, bottomRightType, bottomRight.RowType);
			}
		}
		public override bool Equals(object obj) {
			XlCellRange other = obj as XlCellRange;
			if (other == null)
				return false;
			return topLeft.Equals(other.topLeft) && bottomRight.Equals(other.bottomRight) && SheetName == other.SheetName;
		}
		public override int GetHashCode() {
			return topLeft.GetHashCode() ^ bottomRight.GetHashCode() ^ sheetName.GetHashCode();
		}
		public override string ToString() {
			return ToString(false);
		}
		public string ToString(bool intervalRange) {
			string result;
			if (topLeft.Equals(bottomRight) && !intervalRange)
				result = topLeft.ToString();
			else
				result = topLeft.ToString() + ':' + bottomRight.ToString();
			if (!String.IsNullOrEmpty(SheetName)) {
				if (ShouldAddQuotes(SheetName))
					result = '\'' + SheetName.Replace("'", "''") + "\'!" + result;
				else
					result = SheetName + '!' + result;
			}
			return result;
		}
		public string ToString(CultureInfo culture) {
			return ToString(false);
		}
		public XlCellRange AsAbsolute() {
			XlCellRange result = new XlCellRange(topLeft.AsAbsolute(), bottomRight.AsAbsolute());
			result.SheetName = SheetName;
			return result;
		}
		public XlCellRange AsRelative() {
			XlCellRange result = new XlCellRange(topLeft.AsRelative(), bottomRight.AsRelative());
			result.SheetName = SheetName;
			return result;
		}
		static bool ShouldAddQuotes(string value) {
			return !IsValidIndentifier(value);
		}
		static bool IsValidIndentifier(string value) {
			if (String.IsNullOrEmpty(value))
				return false;
			for (int i = 0; i < value.Length; i++)
				if (!IsValidIndentifierChar(value[i], i, value))
					return false;
			return true;
		}
		static bool IsValidIndentifierChar(char curChar, int index, string value) {
			if (index == 0) {
				if (curChar == '\\') {
					if (value.Length == 1)
						return true;
					if (value[1] != '\\' && value[1] != '.' && value[1] != '?' && value[1] != '_')
						return false;
				}
				else
					if (!char.IsLetter(curChar) && curChar != '_')
						return false;
			}
			return !(!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '.' && curChar != '\\' && curChar != '?');
		}
		internal bool HasCommonCells(XlCellRange other) {
			bool hasCommonColumns = Math.Max(FirstColumn, other.FirstColumn) <= Math.Min(LastColumn, other.LastColumn);
			bool hasCommonRows = Math.Max(FirstRow, other.FirstRow) <= Math.Min(LastRow, other.LastRow);
			return hasCommonColumns && hasCommonRows;
		}
		internal bool Contains(XlCellPosition position) {
			return position.Column >= FirstColumn && position.Column <= LastColumn && position.Row >= FirstRow && position.Row <= LastRow;
		}
		internal bool Contains(int column, int row) {
			return column >= FirstColumn && column <= LastColumn && row >= FirstRow && row <= LastRow;
		}
	}
	#endregion
}
