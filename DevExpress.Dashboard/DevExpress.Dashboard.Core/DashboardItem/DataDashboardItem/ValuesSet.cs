#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public interface ISelectionRow : IEnumerable<object>, IIndexAccess<object>, IEquatable<ISelectionRow> { }
	public interface ISelectionColumn : IEnumerable<object>, IIndexAccess<object>, IEquatable<ISelectionColumn> { }
	public interface IValuesSet : IEnumerable<ISelectionRow>, IEnumerable<ISelectionColumn>, IEquatable<IValuesSet> {
		int ColumnsCount { get; }
		int RowsCount { get; }
		bool IsEmpty { get; }
		ISelectionRow RowByIndex(int rowIndex);
		ISelectionColumn ColumnByIndex(int dimIndex);
		IList<IList> RowListEmptyNull { get; }
	}
	public interface IDimensionValuesSet : IValuesSet {
		IEnumerable<Dimension> Dimensions { get; }
		IValuesSet DimValuesByDimension(Dimension dimension);
	}
	public class SelectionRowWrapper : ISelectionRow {
		object[] row; 
		public SelectionRowWrapper(IEnumerable<object> row) {
			this.row = row.ToArray();
		}
		public SelectionRowWrapper(IEnumerable<object> row, int columnCount) {
			this.row = row.ToArray();
			int add = columnCount - this.row.Length;
			Guard.ArgumentNonNegative(add, "");
			if(add > 0)
				this.row = row.Concat(Enumerable.Repeat(DashboardSpecialValues.OlapNullValue, add)).ToArray();
		}
		public override int GetHashCode() {
			return Helper.EnumerableObjectComparer.GetHashCode(this);
		}
		IEnumerator<object> IEnumerable<object>.GetEnumerator() {
			return row.AsEnumerable().GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return row.GetEnumerator();
		}
		object IAssocArray<int, object>.this[int index] { get { return row[index]; } }
		bool IEquatable<ISelectionRow>.Equals(ISelectionRow other) {
			return this.SequenceEqual(other);
		}
	}
	public class SelectionColumnWrapper : ISelectionColumn {
		object[] column;
		public SelectionColumnWrapper(IEnumerable<object> column) {
			this.column = column.ToArray();
		}
		public IEnumerator<object> GetEnumerator() {
			return column.AsEnumerable().GetEnumerator();
		}
		public override int GetHashCode() {
			return Helper.EnumerableObjectComparer.GetHashCode(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return column.GetEnumerator();
		}
		object IAssocArray<int, object>.this[int index] { get { return column[index]; } }
		bool IEquatable<ISelectionColumn>.Equals(ISelectionColumn other) {
			return Helper.EnumerableObjectComparer.Equals(this, other);
		}
	}
	public static class ValuesSetHelper {
		public static ISelectionRow AsSelectionRow(this IEnumerable<object> enumerable) {
			return new SelectionRowWrapper(enumerable);
		}
		public static ISelectionRow AsSelectionRow(this IEnumerable<object> enumerable, int columnCount) {
			return new SelectionRowWrapper(enumerable, columnCount);
		}
		public static ISelectionColumn AsSelectionColumn(this IEnumerable<object> enumerable) {
			return new SelectionColumnWrapper(enumerable);
		}
		public static IValuesSet AsValuesSet(this IEnumerable<ISelectionRow> selectionRows) {
			return new ValuesSet(selectionRows);
		}
		public static IValuesSet AsValuesSet(this IEnumerable<ISelectionColumn> selectionColumns) {
			return new ValuesSet(Helper.Transpose(selectionColumns).Select(x => x.AsSelectionRow())); 
		}
		public static IValuesSet AsValuesSet(this object[,] values) {
			return Helper.MatrixToList(values).Select(x => x.AsSelectionRow()).AsValuesSet();
		}
		public static IEnumerable<ISelectionRow> AsSelectionRows(this IValuesSet valuesSet) {
			return valuesSet;
		}
		public static IEnumerable<ISelectionColumn> AsSelectionColumns(this IValuesSet valuesSet) {
			return valuesSet;
		}
		public static IValuesSet EmptyValuesSet() {
			return new ValuesSet(new ISelectionRow[0] { });
		}
		public static IValuesSet FromFlatRowList(IEnumerable rows) {
			IList<IEnumerable<object>> rowList = new List<IEnumerable<object>>();
			foreach (object row in rows) {
				IEnumerable rowEnumerable = (IEnumerable)row;
				rowList.Add(rowEnumerable.Cast<object>());
			}
			return rowList.Select(x => x.AsSelectionRow()).AsValuesSet();
		}
	}
	public class ValuesSet : IValuesSet, IEnumerable<ISelectionRow>, IEnumerable<ISelectionColumn> {
		readonly IList<ISelectionRow> rowList;
		readonly int columnCount;
		readonly bool isEmpty;
		IList<ISelectionColumn> ColumnsList {
			get { return Helper.Transpose(rowList).Select(x => x.AsSelectionColumn()).ToList(); }
		}
		public ValuesSet(IEnumerable<ISelectionRow> values) {
			rowList = values.ToList();
			columnCount = rowList.Count == 0 ? 0 : rowList.Select(x => x.Count()).Max();
			for (int i = 0; i < rowList.Count; i++)
				rowList[i] = rowList[i].AsSelectionRow(columnCount);
			isEmpty = this.rowList.Count == 0 || columnCount == 0;
		}
		public override bool Equals(object obj) {
			return Equals((IEquatable<IValuesSet>)obj);
		}
		public override int GetHashCode() {
			return rowList.GetHashCode();
		}
		#region IValuesSet implementation
		bool IValuesSet.IsEmpty { get { return isEmpty; } }
		int IValuesSet.ColumnsCount { get { return isEmpty ? 0 : columnCount; } }
		int IValuesSet.RowsCount { get { return isEmpty ? 0 : rowList.Count; } }
		ISelectionRow IValuesSet.RowByIndex(int rowIndex) {
			return rowList[rowIndex];
		}
		ISelectionColumn IValuesSet.ColumnByIndex(int columnIndex) {
			return ColumnsList[columnIndex];
		}
		IList<IList> IValuesSet.RowListEmptyNull {
			get {
				IList<IList> result = rowList.Select(x => (IList)x.ToList()).ToList();
				return isEmpty ? null : result;
			}
		}
		bool IEquatable<IValuesSet>.Equals(IValuesSet obj) {
			IValuesSet obj1 = this;
			IValuesSet obj2 = obj;
			bool isLengthEquals = obj1.ColumnsCount == obj2.ColumnsCount &&
				obj1.RowsCount == obj2.RowsCount;
			if (isLengthEquals) {
				IList<ISelectionRow> values1 = rowList;
				IList<ISelectionRow> values2 = ((ValuesSet)obj2).rowList;
				for (int i = 0; i < values1.Count; i++)
					if (!Helper.IsEqualEnums(values1[i], values2[i]))
						return false;
				return true;
			} else
				return false;
		}
		IEnumerator<ISelectionRow> IEnumerable<ISelectionRow>.GetEnumerator() {
			return rowList.AsEnumerable().GetEnumerator(); 
		}
		IEnumerator<ISelectionColumn> IEnumerable<ISelectionColumn>.GetEnumerator() {
			return ColumnsList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<ISelectionRow>)this).GetEnumerator();
		}
		#endregion
		public static IValuesSet FromPointsArray(object[,] values) {
			return values.AsValuesSet();
		}
	}
}
