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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region RangeHorizontalVector
	public class RangeHorizontalVector : IVector<VariantValue>, IEquatable<RangeHorizontalVector> {
		readonly CellRange range;
		readonly int rowOffset;
		public RangeHorizontalVector(CellRange range, int rowOffset) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
			this.rowOffset = rowOffset;
		}
		#region IVector<VariantValue> Members
		public int Count { get { return range.Width; } }
		public VariantValue this[int index] {
			get {
				return range.Worksheet.GetCalculatedCellValue(range.TopLeft.Column + index, range.TopLeft.Row + rowOffset);
			}
			set {
				range.Worksheet.GetCell(range.TopLeft.Column + index, range.TopLeft.Row + rowOffset).Value = value;
			}
		}
		#endregion
		public override bool Equals(object obj) {
			RangeHorizontalVector other = obj as RangeHorizontalVector;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(RangeHorizontalVector other) {
			return object.ReferenceEquals(range.Worksheet, other.range.Worksheet) && range.EqualsPosition(other.range) && rowOffset == other.rowOffset;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(range.Worksheet.GetHashCode(), range.TopLeft.GetHashCode(), range.BottomRight.GetHashCode(), rowOffset);
		}
	}
	#endregion
	#region ArrayHorizontalVector
	public class ArrayHorizontalVector : IVector<VariantValue>, IEquatable<ArrayHorizontalVector> {
		readonly IVariantArray array;
		readonly int rowIndex;
		public ArrayHorizontalVector(IVariantArray array, int rowIndex) {
			Guard.ArgumentNotNull(array, "array");
			this.array = array;
			this.rowIndex = rowIndex;
		}
		#region IVector<VariantValue> Members
		public int Count { get { return array.Width; } }
		public VariantValue this[int index] { get { return array.GetValue(rowIndex, index); } set { throw new InvalidOperationException(); } }
		#endregion
		public override bool Equals(object obj) {
			ArrayHorizontalVector other = obj as ArrayHorizontalVector;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(ArrayHorizontalVector other) {
			return array.Equals(other.array) && rowIndex == other.rowIndex;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(array.GetHashCode(), rowIndex);
		}
	}
	#endregion
	#region RangeVerticalVector
	public class RangeVerticalVector : IVector<VariantValue> {
		readonly CellRange range;
		readonly int columnOffset;
		public RangeVerticalVector(CellRange range, int columnOffset) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
			this.columnOffset = columnOffset;
		}
		public CellRange Range { get { return range; } }
		#region IVector<VariantValue> Members
		public int Count { get { return range.Height; } }
		public VariantValue this[int index] {
			get {
				return range.Worksheet.GetCalculatedCellValue(range.TopLeft.Column + columnOffset, range.TopLeft.Row + index);
			}
			set {
				range.Worksheet.GetCell(range.TopLeft.Column + columnOffset, range.TopLeft.Row + index).Value = value;
			}
		}
		#endregion
		public override bool Equals(object obj) {
			RangeVerticalVector other = obj as RangeVerticalVector;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(RangeVerticalVector other) {
			return object.ReferenceEquals(range.Worksheet, other.Range.Worksheet) && range.EqualsPosition(other.range) && columnOffset == other.columnOffset;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(range.Worksheet.GetHashCode(), range.TopLeft.GetHashCode(), range.BottomRight.GetHashCode(), columnOffset);
		}
	}
	#endregion
	#region ArrayVerticalVector
	public class ArrayVerticalVector : IVector<VariantValue>, IEquatable<ArrayVerticalVector> {
		readonly IVariantArray array;
		readonly int columnIndex;
		public ArrayVerticalVector(IVariantArray array, int columnIndex) {
			Guard.ArgumentNotNull(array, "array");
			this.array = array;
			this.columnIndex = columnIndex;
		}
		#region IVector<VariantValue> Members
		public int Count { get { return array.Height; } }
		public VariantValue this[int index] { get { return array.GetValue(index, columnIndex); } set { throw new InvalidOperationException(); } }
		#endregion
		public override bool Equals(object obj) {
			ArrayVerticalVector other = obj as ArrayVerticalVector;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(ArrayVerticalVector other) {
			return array.Equals(other.array) && columnIndex == other.columnIndex;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(array.GetHashCode(), columnIndex);
		}
	}
	#endregion
	#region ArrayZVector
	public class ArrayZVector : IVector<VariantValue> {
		readonly IVariantArray array;
		public ArrayZVector(IVariantArray array) {
			Guard.ArgumentNotNull(array, "array");
			this.array = array;
		}
		#region IVector<VariantValue> Members
		public int Count { get { return (int)array.Count; } }
		public VariantValue this[int index] { get { return array[index]; } set { throw new InvalidOperationException(); } }
		#endregion
	}
	#endregion
	#region RangeZVector
	public class RangeZVector : IVector<VariantValue> {
		readonly CellRange cellRange;
		public RangeZVector(CellRange cellRange) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			this.cellRange = cellRange;
		}
		#region IVector<VariantValue> Members
		public int Count { get { return (int)cellRange.CellCount; } }
		public VariantValue this[int index] {
			get {
				return cellRange.Worksheet.GetCalculatedCellValue(cellRange.TopLeft.Column + index % cellRange.Width, cellRange.TopLeft.Row + (int)(index / cellRange.Width));
			}
			set {
				throw new InvalidOperationException();
			}
		}
		#endregion
	}
	#endregion
	#region RangeZVector
	public class RangeZNVector : IVector<VariantValue> {
		IVector<VariantValue>[] innerVectors;
		long[] positions;
		long count;
		public RangeZNVector(CellRangeBase cellRange) {
			Guard.ArgumentNotNull(cellRange, "cellRange");
			switch (cellRange.RangeType) {
				case CellRangeType.IntervalRange:
				case CellRangeType.SingleRange:
					CellRange range = cellRange.GetFirstInnerCellRange();
					innerVectors = new IVector<VariantValue>[1];
					positions = new long[1];
					innerVectors[0] = new RangeZVector(range);
					count = range.CellCount;
					positions[0] = count;
					break;
				case CellRangeType.UnionRange:
					CellUnion unionRange = cellRange as CellUnion;
					int innerRangesCount = unionRange.InnerCellRanges.Count;
					innerVectors = new IVector<VariantValue>[innerRangesCount];
					positions = new long[innerRangesCount];
					for (int i = 0; i < innerRangesCount; i++) {
						CellRangeBase innerRange = unionRange.InnerCellRanges[i];
						if (innerRange.RangeType == CellRangeType.UnionRange)
							innerVectors[i] = new RangeZNVector(innerRange);
						else
							innerVectors[i] = new RangeZVector(innerRange.GetFirstInnerCellRange());
						count += innerRange.CellCount;
						positions[i] = count;
					}
					break;
				default: throw new ArgumentException("Unknown CellRangeType " + cellRange.RangeType);
			}
		}
		#region IVector<VariantValue> Members
		public int Count { get { return (int)count; } }
		public VariantValue this[int index] {
			get {
				long curPosition = 0;
				while (index >= positions[curPosition])
					curPosition++;
				long innerIndex = index;
				if (curPosition > 0)
					innerIndex -= positions[curPosition - 1];
				VariantValue res = innerVectors[(int)curPosition][(int)innerIndex];
				return res;
			}
			set {
				throw new InvalidOperationException();
			}
		}
		#endregion
	}
	#endregion
}
