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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using System;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotDataKey
	public struct PivotDataKey : IEquatable<PivotDataKey> {
		readonly int[] items;
		public int this[int index] { get { return Items[index]; } set { Items[index] = value; } }
		public int Length { get { return Items.Length; } }
		public int[] Items { get { return items; } }
		[DebuggerStepThrough]
		public static implicit operator PivotDataKey(int[] items) {
			return new PivotDataKey(items);
		}
		[DebuggerStepThrough]
		public PivotDataKey(int[] keyValueSharedItemIndices) {
			this.items = keyValueSharedItemIndices;
		}
		public override int GetHashCode() {
				CombinedHashCode result = new CombinedHashCode();
				for (int i = items.Length - 1; i >= 0; i--)
					result.AddInt(items[i]);
				return result.CombinedHash32;
		}
		public override bool Equals(object obj) {
			return obj is PivotDataKey && Equals((PivotDataKey)obj);
		}
		public bool Equals(PivotDataKey other) {
			int length = Items.Length;
			if (length != other.Length)
				return false;
			for (int i = 0; i < length; i++)
				if (Items[i] != other[i])
					return false;
			return true;
		}
		public override string ToString() {
			return Items.ToString();
		}
		public void Clear(int startIndex, int endIndex) {
			for (int i = startIndex; i <= endIndex; ++i)
				items[i] = -1;
		}
		public PivotDataKey Clone() {
			int[] cloneItems = new int[Length];
			Array.Copy(items, cloneItems, Length);
			return new PivotDataKey(cloneItems);
		}
	}
	#endregion
	#region PivotCellKey
	public class PivotCellKey : ICloneable<PivotCellKey> {
		PivotDataKey dataKey;
		List<int> columnKey;
		List<int> rowKey;
		public PivotCellKey(int dataKeyCount) {
			int[] sharedItemsKey = new int[dataKeyCount];
			this.dataKey = new PivotDataKey(sharedItemsKey);
			this.dataKey.Clear(0, sharedItemsKey.Length - 1);
			this.columnKey = new List<int>();
			this.rowKey = new List<int>();
		}
		public PivotCellKey(List<int> columnKey, List<int> rowKey, PivotDataKey dataKey) {
			this.dataKey = dataKey;
			this.columnKey = columnKey;
			this.rowKey = rowKey;
		}
		public PivotDataKey DataKey { get { return dataKey; } }
		public List<int> ColumnKey { get { return columnKey; } }
		public List<int> RowKey { get { return rowKey; } }
		public bool IsSubtotalKey(PivotTable table) {
			if (dataKey.Length == 0)
				return true;
			int rowKeyIndicesCount = table.RowFields.KeyIndicesCount;
			if (rowKeyIndicesCount > 0) {
				if (dataKey[rowKeyIndicesCount - 1] == -1)
					return true;
				if (dataKey[0] == -1)
					return true;
			}
			int columnKeyIndicesCount = table.ColumnFields.KeyIndicesCount;
			if (columnKeyIndicesCount > 0) {
				if (dataKey[dataKey.Length - 1] == -1)
					return true;
				if (dataKey[rowKeyIndicesCount] == -1)
					return true;
			}
			return false;
		}
		public void AddRowKey(IPivotLayoutItem row, PivotTable pivotTable) {
			AddKey(row, rowKey, 0, pivotTable, pivotTable.RowFields);
		}
		public void AddColumnKey(IPivotLayoutItem column, PivotTable pivotTable) {
			AddKey(column, columnKey, pivotTable.RowFields.KeyIndicesCount, pivotTable, pivotTable.ColumnFields);
		}
		void AddKey(IPivotLayoutItem item, List<int> key, int dataKeyStartIndex, PivotTable pivotTable, PivotTableColumnRowFieldIndices fieldIndices) {
			int endIndex = key.Count;
			key.RemoveRange(item.RepeatedItemsCount, key.Count - item.RepeatedItemsCount);
			key.AddRange(item.PivotFieldItemIndices);
			int dataKeyIndexPosition = dataKeyStartIndex + item.RepeatedItemsCount;
			if (fieldIndices.HasValuesField) {
				if (item.RepeatedItemsCount > fieldIndices.ValuesFieldIndex)
					--dataKeyIndexPosition;
				if (endIndex > fieldIndices.ValuesFieldIndex)
					--endIndex;
			}
			for (int i = 0; i < item.PivotFieldItemIndices.Length; ++i) {
				int fieldIndex = fieldIndices[item.RepeatedItemsCount + i];
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				PivotField field = pivotTable.Fields[fieldIndex];
				int itemIndex = item.PivotFieldItemIndices[i];
				int cacheIndex = itemIndex == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex ? -1 : field.Items[itemIndex].ItemIndex;
				dataKey[dataKeyIndexPosition++] = cacheIndex;
			}
			endIndex += dataKeyStartIndex;
			dataKey.Clear(dataKeyIndexPosition, endIndex - 1);
		}
		public void RemoveAtRow(int index, PivotTable pivotTable) {
			RemoveAt(index, pivotTable.RowFields, RowKey, 0);
		}
		public void RemoveAtColumn(int index, PivotTable pivotTable) {
			RemoveAt(index, pivotTable.ColumnFields, ColumnKey, pivotTable.RowFields.KeyIndicesCount);
		}
		void RemoveAt(int index, PivotTableColumnRowFieldIndices fieldIndices, List<int> key, int dataKeyStartIndex) {
			key.RemoveAt(index);
			if (fieldIndices.HasValuesField)
				if (index >= fieldIndices.ValuesFieldIndex)
					--index;
			index += dataKeyStartIndex;
			dataKey[index] = -1;
		}
		public void RemoveRowRange(int index, int count, PivotTable pivotTable) {
			RemoveRange(index, count, pivotTable.RowFields, RowKey, 0);
		}
		public void RemoveColumnRange(int index, int count, PivotTable pivotTable) {
			RemoveRange(index, count, pivotTable.ColumnFields, ColumnKey, pivotTable.RowFields.KeyIndicesCount);
		}
		void RemoveRange(int index, int count, PivotTableColumnRowFieldIndices fieldIndices, List<int> key, int dataKeyStartIndex) {
			key.RemoveRange(index, count);
			count += index;
			if (fieldIndices.HasValuesField) {
				if (index >= fieldIndices.ValuesFieldIndex)
					--index;
				if (count > fieldIndices.ValuesFieldIndex)
					--count;
			}
			index += dataKeyStartIndex;
			count += dataKeyStartIndex;
			dataKey.Clear(index, count - 1);
		}
		public void SetRowIndex(int index, int item, PivotTable pivotTable) {
			SetIndex(index, item, pivotTable, pivotTable.RowFields, RowKey, 0);
		}
		public void SetColumnIndex(int index, int item, PivotTable pivotTable) {
			SetIndex(index, item, pivotTable, pivotTable.ColumnFields, ColumnKey, pivotTable.RowFields.KeyIndicesCount);
		}
		void SetIndex(int index, int item, PivotTable pivotTable, PivotTableColumnRowFieldIndices fieldIndices, List<int> key, int dataKeyStartIndex) {
			key[index] = item;
			int fieldIndex = fieldIndices[index];
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return;
			if (fieldIndices.HasValuesField)
				if (index >= fieldIndices.ValuesFieldIndex)
					--index;
			PivotField field = pivotTable.Fields[fieldIndex];
			int cacheIndex = item == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex ? -1 : field.Items[item].ItemIndex;
			dataKey[dataKeyStartIndex + index] = cacheIndex;
		}
		public void ClearColumnKey(PivotTable table) {
			columnKey.Clear();
			dataKey.Clear(table.RowFields.KeyIndicesCount, dataKey.Length - 1);
		}
		public void ClearRowKey(PivotTable table) {
			rowKey.Clear();
			dataKey.Clear(0, table.RowFields.KeyIndicesCount - 1);
		}
		public PivotDataKey ToRowDataKey(PivotTable pivotTable) {
			return ToDataKey(0, pivotTable.RowFields.KeyIndicesCount - 1);
		}
		public PivotDataKey ToColumnDataKey(PivotTable pivotTable) {
			int rowKeyCount = pivotTable.RowFields.KeyIndicesCount;
			return ToDataKey(rowKeyCount, rowKeyCount + pivotTable.ColumnFields.KeyIndicesCount - 1);
		}
		PivotDataKey ToDataKey(int startIndex, int endIndex) {
			PivotDataKey dataKey = new PivotDataKey(new int[this.dataKey.Length]);
			dataKey.Clear(0, dataKey.Length - 1);
			for (int i = startIndex; i <= endIndex; ++i)
				dataKey[i] = this.dataKey[i];
			return dataKey;
		}
		public List<int> ToRowDataKey(PivotTable pivotTable, int count) { 
			return ToAxisDataKey(pivotTable, pivotTable.RowFields, rowKey, count);
		}
		public List<int> ToColumnDataKey(PivotTable pivotTable, int count) { 
			return ToAxisDataKey(pivotTable, pivotTable.ColumnFields, columnKey, count);
		}
		List<int> ToAxisDataKey(PivotTable pivotTable, PivotTableColumnRowFieldIndices fieldIndices, List<int> keysList, int count) { 
			List<int> cacheKey = new List<int>();
			for (int i = 0; i < count; ++i) {
				int fieldIndex = fieldIndices[i].FieldIndex;
				if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
					PivotField field = pivotTable.Fields[fieldIndex];
					int itemIndex = keysList[i];
					int cacheIndex;
					if (itemIndex == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
						cacheIndex = 0; 
					else
						cacheIndex = field.Items[itemIndex].ItemIndex;
					cacheKey.Add(cacheIndex);
				}
			}
			return cacheKey;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(HashCodeCalculator.CalcHashCode32(columnKey), HashCodeCalculator.CalcHashCode32(RowKey));
		}
		public override bool Equals(object obj) {
			PivotCellKey other = obj as PivotCellKey;
			if (other == null)
				return false;
			if (columnKey.Count != other.columnKey.Count || rowKey.Count != other.rowKey.Count)
				return false;
			for (int i = 0; i < rowKey.Count; i++)
				if (rowKey[i] != other.rowKey[i])
					return false;
			for (int i = 0; i < columnKey.Count; i++)
				if (columnKey[i] != other.columnKey[i])
					return false;
			return true;
		}
		public PivotCellKey Clone() {
			List<int> columnKey = new List<int>(this.columnKey);
			List<int> rowKey = new List<int>(this.rowKey);
			return new PivotCellKey(columnKey, rowKey, dataKey.Clone());
		}
	}
	#endregion
}
