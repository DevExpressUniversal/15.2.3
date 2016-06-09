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
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	using DevExpress.XtraSpreadsheet.Model;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	public interface IPivotLayoutItemAccessor {
		int RangeSize { get; }
		int OtherRangeSize { get; }
		int FirstDataItemIndex { get; }
		int FirstHeaderItemIndex { get; }
		int KeyFieldCount { get; }
		int GetOtherItemIndexByPosition(CellPosition currentCell);
		int GetOtherItemIndexByPositionIncludingHeaders(CellPosition currentCell);
		int GetActiveLayoutItemIndex(CellPosition currentCell);
		IPivotLayoutItem GetItemByIndex(int itemIndex);
		PivotField GetFieldByAxisKeyFieldIndex(int currentFieldIndex, int dataFieldIndex);
		bool FieldIsOutline(PivotField field);
		bool FieldIsTabular(PivotField field);
		bool HasFields { get; }
		PivotTableColumnRowFieldIndices FieldIndices { get; }
		bool AxisContainsDataField { get; }
		bool OtherAxisHasFields { get; }
		int GetFieldIndex(int index);
		int GetFieldIndex(int index, int dataFieldIndex);
		PivotFieldZoneInfo GetFieldZoneInfo(int index, int dataFieldIndex);
		bool AxisHasGrandTotals { get; }
		bool SupportsBlankRow { get; }
		void AddLayoutItem(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex, PivotFieldItemType itemType);
		IPivotReportLayoutFormBuilder GetInitialLayoutBuilder();
		PivotCacheHasKeyResponse HasKeySequence(List<int> key);
	}
	class PivotLayoutItemRowAccessor : IPivotLayoutItemAccessor {
		readonly PivotTable table;
		public PivotLayoutItemRowAccessor(PivotTable table) {
			this.table = table;
		}
		#region IPivotLayoutItemAccessor Members
		public int OtherRangeSize { get { return table.Location.Range.Width; } }
		public int RangeSize { get { return table.Location.Range.Height; } }
		public int FirstDataItemIndex { get { return table.Location.FirstDataColumn; } }
		public int FirstHeaderItemIndex { get { return 0; } }
		public int KeyFieldCount { get { return table.RowFields.Count; } }
		public bool HasFields { get { return table.RowFields.Count > 0; } }
		public bool OtherAxisHasFields { get { return table.ColumnFields.Count > 0; } }
		public bool AxisContainsDataField { get { return table.DataOnRows; } }
		public PivotTableColumnRowFieldIndices FieldIndices { get { return table.RowFields; } }
		public bool AxisHasGrandTotals { get { return table.ColumnGrandTotals; } }
		public bool SupportsBlankRow { get { return true; } }
		public int GetActiveLayoutItemIndex(CellPosition currentCell) {
			return currentCell.Row - table.Range.TopRowIndex - table.Location.FirstDataRow;
		}
		public IPivotLayoutItem GetItemByIndex(int itemIndex) {
			if (itemIndex >= table.RowItems.Count || itemIndex < 0)
				return null;
			return table.RowItems[itemIndex];
		}
		public int GetOtherItemIndexByPosition(CellPosition currentCell) {
			return currentCell.Column - table.Range.LeftColumnIndex - table.Location.FirstDataColumn;
		}
		public int GetOtherItemIndexByPositionIncludingHeaders(CellPosition currentCell) {
			return currentCell.Column - table.Range.LeftColumnIndex;
		}
		public int GetFieldIndex(int index) {
			return table.RowFields[index];
		}
		public int GetFieldIndex(int index, int dataFieldIndex) {
			int fieldIndex = table.RowFields[index].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				fieldIndex = table.DataFields[dataFieldIndex].FieldIndex;
			return fieldIndex;
		}
		public PivotFieldZoneInfo GetFieldZoneInfo(int index, int dataFieldIndex) {
			int fieldIndex = table.RowFields[index].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[dataFieldIndex].FieldIndex, dataFieldIndex);
			return new PivotFieldZoneInfo(PivotTableAxis.Row, fieldIndex, index);
		}
		public PivotField GetFieldByAxisKeyFieldIndex(int keyfieldIndex, int dataFieldIndex) {
			int fieldIndex = GetFieldIndex(keyfieldIndex, dataFieldIndex);
			return table.Fields[fieldIndex];
		}
		public bool FieldIsOutline(PivotField field) {
			return field.Compact && field.Outline;
		}
		public bool FieldIsTabular(PivotField field) {
			return !field.Outline;
		}
		public void AddLayoutItem(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex, PivotFieldItemType itemType) {
			table.RowItems.Add(PivotLayoutItemFactory.CreateInstance(pivotFieldItemIndices, repeatedItemsCount, dataFieldIndex, itemType));
		}
		public IPivotReportLayoutFormBuilder GetInitialLayoutBuilder() {
			return new PivotReportLayoutAutoFormBuilder(this);
		}
		public PivotCacheHasKeyResponse HasKeySequence(List<int> key) {
			return table.CalculatedCache.HasRowKeySequence(key);
		}
		#endregion
	}
	class PivotLayoutItemColumnAccessor : IPivotLayoutItemAccessor {
		readonly PivotTable table;
		public PivotLayoutItemColumnAccessor(PivotTable table) {
			this.table = table;
		}
		#region IPivotLayoutItemAccessor Members
		public int OtherRangeSize { get { return table.Location.Range.Height; } }
		public int RangeSize { get { return table.Location.Range.Width; } }
		public int FirstDataItemIndex { get { return table.Location.FirstDataRow; } }
		public int FirstHeaderItemIndex { get { return table.Location.FirstHeaderRow; } }
		public int KeyFieldCount { get { return table.ColumnFields.Count; } }
		public bool HasFields { get { return table.ColumnFields.Count > 0; } }
		public bool AxisContainsDataField { get { return !table.DataOnRows; } }
		public bool OtherAxisHasFields { get { return table.RowFields.Count > 0; } }
		public PivotTableColumnRowFieldIndices FieldIndices { get { return table.ColumnFields; } }
		public bool AxisHasGrandTotals { get { return table.RowGrandTotals; } }
		public bool SupportsBlankRow { get { return false; } }
		public int GetActiveLayoutItemIndex(CellPosition currentCell) {
			return currentCell.Column - table.Range.LeftColumnIndex - table.Location.FirstDataColumn;
		}
		public int GetOtherItemIndexByPosition(CellPosition currentCell) {
			return currentCell.Row - table.Range.TopRowIndex - table.Location.FirstDataRow;
		}
		public int GetOtherItemIndexByPositionIncludingHeaders(CellPosition currentCell) {
			return currentCell.Row - table.Range.TopRowIndex - table.Location.FirstHeaderRow;
		}
		public IPivotLayoutItem GetItemByIndex(int itemIndex) {
			if (itemIndex >= table.ColumnItems.Count || itemIndex < 0)
				return null;
			return table.ColumnItems[itemIndex];
		}
		public int GetFieldIndex(int index) {
			return table.ColumnFields[index];
		}
		public int GetFieldIndex(int index, int dataFieldIndex) {
			int fieldIndex = table.ColumnFields[index].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				fieldIndex = table.DataFields[dataFieldIndex].FieldIndex;
			return fieldIndex;
		}
		public PivotFieldZoneInfo GetFieldZoneInfo(int index, int dataFieldIndex) {
			int fieldIndex = table.ColumnFields[index].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[dataFieldIndex].FieldIndex, dataFieldIndex);
			return new PivotFieldZoneInfo(PivotTableAxis.Column, fieldIndex, index);
		}
		public PivotField GetFieldByAxisKeyFieldIndex(int keyFieldIndex, int dataFieldIndex) {
			int fieldIndex = GetFieldIndex(keyFieldIndex, dataFieldIndex);
			return table.Fields[fieldIndex];
		}
		public bool FieldIsOutline(PivotField field) {
			return false;
		}
		public bool FieldIsTabular(PivotField field) {
			return true;
		}
		public void AddLayoutItem(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex, PivotFieldItemType itemType) {
			table.ColumnItems.Add(PivotLayoutItemFactory.CreateInstance(pivotFieldItemIndices, repeatedItemsCount, dataFieldIndex, itemType));
		}
		public IPivotReportLayoutFormBuilder GetInitialLayoutBuilder() {
			return new PivotReportLayoutTabularFormBuilder(this);
		}
		public PivotCacheHasKeyResponse HasKeySequence(List<int> key) {
			return table.CalculatedCache.HasColumnKeySequence(key);
		}
		#endregion
	}
}
