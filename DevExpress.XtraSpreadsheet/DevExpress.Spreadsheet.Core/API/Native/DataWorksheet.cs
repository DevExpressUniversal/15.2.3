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
using System.Data;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Data;
namespace DevExpress.Spreadsheet {
#if DEBUGTEST
	public interface VirtualWorksheet : Worksheet {
		IVirtualData Data { get; }
	}
	public interface IVirtualData {
		IVirtualRowCollection Rows { get; }
		IEnumerator<IVirtualCell> GetZOrderEnumerator();
		IVirtualCell GetCell(int columnIndex, int rowIndex);
	}
	public interface IVirtualRowCollection : IList<IVirtualRow> {
	}
	public interface IVirtualRow {
		IVirtualCellCollection Cells { get; }
		int Index { get; }
	}
	public interface IVirtualCellCollection : IList<IVirtualCell> {
	}
	public interface IVirtualCell {
		CellValue Value { get; }
		int ColumnIndex { get; }
		int RowIndex { get; }
	}
	#region IArrayData
	public interface IArrayData {
		int RowsCount { get; }
		int ColumnsCount { get; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		object this[int rowIndex, int columnIndex] { get; }
		IEnumerator<IVirtualCell> GetEnumerator(int firstRowIndex, int firstColumnIndex, IDataValueConverter converter);
	}
	#endregion
#if !DXPORTABLE
	#region DataTableVirtualData
	public class DataTableVirtualData : ObjectArrayVirtualData {
		public DataTableVirtualData(DataTable dataTable, int firstRowIndex, int firstColumnIndex, bool addHeader) 
			: base(new DataTableVirtualDataHelper(dataTable, addHeader), firstRowIndex, firstColumnIndex, null) {
		}
		public DataTableVirtualData(DataTable dataTable, int firstRowIndex, int firstColumnIndex, bool addHeader, IDataValueConverter converter)
			: base(new DataTableVirtualDataHelper(dataTable, addHeader), firstRowIndex, firstColumnIndex, converter) { 
		}
	}
	#endregion
	#region DataTableVirtualDataHelper
	class DataTableVirtualDataHelper : IArrayData {
		readonly DataTable dataTable;
		readonly bool addHeader;
		public DataTableVirtualDataHelper(DataTable dataTable, bool addHeader) {
			this.dataTable = dataTable;
			this.addHeader = addHeader;
		}
	#region IArrayData Members
		public int ColumnsCount { get { return dataTable.Columns.Count; } }
		public int RowsCount {
			get {
				int rowsCount = dataTable.Rows.Count;
				return addHeader ? rowsCount + 1 : rowsCount;
			}
		}
		public object this[int rowIndex, int columnIndex] {
			get {
				return addHeader ? AddHeader(rowIndex, columnIndex) : dataTable.Rows[rowIndex][columnIndex];
			}
		}
		object AddHeader(int rowIndex, int columnIndex) {
			return rowIndex == 0 ? dataTable.Columns[columnIndex].Caption : dataTable.Rows[rowIndex - 1][columnIndex];
		}
		public IEnumerator<IVirtualCell> GetEnumerator(int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			return new DataTableZOrderEnumerator(dataTable, firstRowIndex, firstColumnIndex, addHeader, converter);
		}
	#endregion
	}
	#endregion
	#region DataTableZOrderEnumerator
	public class DataTableZOrderEnumerator : IEnumerator<IVirtualCell> {
	#region Fields
		readonly DataTable dataTable;
		readonly IDataValueConverter converter;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		readonly bool addHeader;
		const int startColumnIndex = 0;
		const int startRowIndex = 0;
		DataTableReader reader;
		int currentRowIndex;
		int currentColumnIndex;
	#endregion
		public DataTableZOrderEnumerator(DataTable dataTable, int firstRowIndex, int firstColumnIndex, bool addHeader, IDataValueConverter converter) {
			this.dataTable = dataTable;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.addHeader = addHeader;
			this.converter = converter;
			Reset();
		}
	#region Properties
		public int RowIndex { get { return currentRowIndex; } }
		public int ColumnIndex { get { return currentColumnIndex; } }
	#endregion
	#region IEnumerator<WorksheetDataLayerCell> Members
		public IVirtualCell Current {
			get {
				bool isHeaderRow = addHeader && currentRowIndex == 0;
				object objectValue = isHeaderRow ? dataTable.Columns[currentColumnIndex].Caption : reader[currentColumnIndex];
				CellValue value = CellValue.CreateForVirtualCell(objectValue, currentColumnIndex, converter);
				return new TestVirtualCell(firstColumnIndex + currentColumnIndex, firstRowIndex + currentRowIndex, value);
			}
		}
	#endregion
	#region IDisposable Members
		public void Dispose() {
		}
	#endregion
	#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			currentColumnIndex++;
			if (currentColumnIndex > reader.FieldCount - 1) {
				currentRowIndex++;
				currentColumnIndex = startColumnIndex;
				return reader.Read();
			}
			return true;
		}
		public void Reset() {
			this.reader = dataTable.CreateDataReader();
			if (!addHeader)
				reader.Read();
			currentRowIndex = startRowIndex;
			currentColumnIndex = startColumnIndex - 1;
		}
	#endregion
	}
	#endregion
#endif
	#region ArrayVirtualData
	public class ArrayVirtualData : ObjectArrayVirtualData {
		public ArrayVirtualData(object[,] array, int firstRowIndex, int firstColumnIndex)
			: base(new ArrayVirtualDataHelper(array), firstRowIndex, firstColumnIndex, null) {
		}
		public ArrayVirtualData(object[,] array, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter)
			: base(new ArrayVirtualDataHelper(array), firstRowIndex, firstColumnIndex, converter) {
		}
	}
	#endregion
	#region ArrayVirtualDataHelper
	class ArrayVirtualDataHelper : IArrayData {
		readonly object[,] array;
		public ArrayVirtualDataHelper(object[,] array) {
			this.array = array;
		}
		#region IArrayData Members
		public int RowsCount { get { return array.GetLength(0); } }
		public int ColumnsCount { get { return array.GetLength(1); } }
		public object this[int rowIndex, int columnIndex] { get { return array[rowIndex, columnIndex]; } }
		public IEnumerator<IVirtualCell> GetEnumerator(int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			return new ArrayZOrderEnumerator(array, firstRowIndex, firstColumnIndex, converter);
		}
		#endregion
	}
	#endregion
	#region ArrayZOrderEnumerator
	public class ArrayZOrderEnumerator : IEnumerator<IVirtualCell> {
		#region Fields
		readonly object[,] twoDimensionalArray;
		readonly IDataValueConverter converter;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		readonly int startColumnIndex;
		readonly int endColumnIndex;
		readonly int startRowIndex;
		readonly int endRowIndex;
		int currentRowIndex;
		int currentColumnIndex;
		#endregion
		public ArrayZOrderEnumerator(object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.twoDimensionalArray = twoDimensionalArray;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
			this.startRowIndex = twoDimensionalArray.GetLowerBound(0);
			this.startColumnIndex = twoDimensionalArray.GetLowerBound(1);
			this.endRowIndex = twoDimensionalArray.GetUpperBound(0);
			this.endColumnIndex = twoDimensionalArray.GetUpperBound(1);
			Reset();
		}
		#region Properties
		public int RowIndex { get { return currentRowIndex; } }
		public int ColumnIndex { get { return currentColumnIndex; } }
		#endregion
		#region IEnumerator<WorksheetDataLayerCell> Members
		public IVirtualCell Current {
			get {
				object objectValue = twoDimensionalArray[currentRowIndex, currentColumnIndex];
				CellValue value = CellValue.CreateForVirtualCell(objectValue, currentColumnIndex, converter);
				return new TestVirtualCell(firstColumnIndex + currentColumnIndex, firstRowIndex + currentRowIndex, value);
			}
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			currentColumnIndex++;
			if (currentColumnIndex > endColumnIndex) {
				currentRowIndex++;
				currentColumnIndex = startColumnIndex;
				return currentRowIndex <= endRowIndex;
			}
			return true;
		}
		public void Reset() {
			currentRowIndex = startRowIndex;
			currentColumnIndex = startColumnIndex - 1;
		}
		#endregion
	}
	#endregion
	#region ObjectArrayVirtualData
	public abstract class ObjectArrayVirtualData : IVirtualData {
		#region Fields
		readonly IArrayData data;
		readonly IDataValueConverter converter;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		ObjectArrayVirtualRowCollection rows;
		#endregion
		protected ObjectArrayVirtualData(IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			CheckIndexValid(firstRowIndex, firstColumnIndex);
			this.data = data;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
			rows = new ObjectArrayVirtualRowCollection(this.data, this.firstRowIndex, this.firstColumnIndex, this.converter);
		}
		#region IVirtualData Members
		public IVirtualRowCollection Rows { get { return rows; } }
		public IEnumerator<IVirtualCell> GetZOrderEnumerator() {
			return data.GetEnumerator(firstRowIndex, firstColumnIndex, converter);
		}
		public IVirtualCell GetCell(int columnIndex, int rowIndex) {
			if (columnIndex < firstColumnIndex || columnIndex > firstColumnIndex + data.ColumnsCount
				|| rowIndex < firstRowIndex || rowIndex > firstRowIndex + data.RowsCount)
				return null;
			object objectValue = data[rowIndex - firstRowIndex, columnIndex - firstColumnIndex];
			CellValue value = CellValue.CreateForVirtualCell(objectValue, columnIndex - firstColumnIndex, converter);
			return new TestVirtualCell(columnIndex, rowIndex, value);
		}
		#endregion
		void CheckIndexValid(int firstRowIndex, int firstColumnIndex) {
			if (firstColumnIndex < 0 || firstRowIndex < 0)
				throw new IndexOutOfRangeException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorNegativeIndexNotAllowed));
		}
	}
	#endregion
	#region ObjectArrayVirtualRowCollection
	class ObjectArrayVirtualRowCollection : IVirtualRowCollection {
		#region Fields
		readonly IDataValueConverter converter;
		readonly IArrayData data;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		#endregion
		public ObjectArrayVirtualRowCollection(IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.data = data;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
		}
		IVirtualRow CreateVirtualRow(int index) {
			return new ObjectArrayVirtualRow(index, data, firstRowIndex, firstColumnIndex, converter);
		}
		#region IList<IVirtualRow> Members
		public int IndexOf(IVirtualRow item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return -1;
		}
		public void Insert(int index, IVirtualRow item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public void RemoveAt(int index) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public IVirtualRow this[int index] {
			get { return CreateVirtualRow(index); }
			set { XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError(); }
		}
		#endregion
		#region ICollection<IVirtualRow> Members
		public void Add(IVirtualRow item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public void Clear() {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public bool Contains(IVirtualRow item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return false;
		}
		public void CopyTo(IVirtualRow[] array, int arrayIndex) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public int Count { get { return data.RowsCount; } }
		public bool IsReadOnly { get { return true; } }
		public bool Remove(IVirtualRow item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return false;
		}
		#endregion
		#region IEnumerable<IVirtualRow> Members
		public IEnumerator<IVirtualRow> GetEnumerator() {
			return new ObjectArrayVirtualRowCollectionEnumerator(data, firstRowIndex, firstColumnIndex, converter);
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region ObjectArrayVirtualRowCollectionEnumerator
	class ObjectArrayVirtualRowCollectionEnumerator : IEnumerator<IVirtualRow> {
		readonly IDataValueConverter converter;
		readonly IArrayData data;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		readonly int rowCount;
		int currentIndex = -1;
		public ObjectArrayVirtualRowCollectionEnumerator(IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.data = data;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
			this.rowCount = data.RowsCount;
		}
		IVirtualRow CreateVirtualRow(int index) {
			if (index < 0)
				return null;
			return new ObjectArrayVirtualRow(index, data, firstRowIndex, firstColumnIndex, converter);
		}
		#region IEnumerator<IVirtualRow> Members
		public IVirtualRow Current { get { return CreateVirtualRow(currentIndex); } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			currentIndex++;
			return currentIndex < rowCount;
		}
		public void Reset() {
			currentIndex = -1;
		}
		#endregion
	}
	#endregion
	#region ObjectArrayVirtualRow
	class ObjectArrayVirtualRow : IVirtualRow {
		readonly int index;
		readonly int firstRowIndex;
		ObjectArrayVirtualCellCollection cells;
		public ObjectArrayVirtualRow(int index, IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.index = index;
			this.firstRowIndex = firstRowIndex;
			cells = new ObjectArrayVirtualCellCollection(index, data, firstRowIndex, firstColumnIndex, converter);
		}
		#region IVirtualRow Members
		public IVirtualCellCollection Cells { get { return cells; } }
		public int Index { get { return index + firstRowIndex; } }
		#endregion
	}
	#endregion
	#region ObjectArrayVirtualCellCollection
	class ObjectArrayVirtualCellCollection : IVirtualCellCollection {
		readonly IDataValueConverter converter;
		readonly int rowIndex;
		readonly IArrayData data;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		public ObjectArrayVirtualCellCollection(int rowIndex, IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.rowIndex = rowIndex;
			this.data = data;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
		}
		IVirtualCell CreateVirtualCell(int index) {
			CellValue value = CellValue.CreateForVirtualCell(data[rowIndex, index], index, converter);
			return new TestVirtualCell(firstColumnIndex + index, rowIndex + firstRowIndex, value);
		}
		#region IList<IVirtualCell> Members
		public int IndexOf(IVirtualCell item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return -1;
		}
		public void Insert(int index, IVirtualCell item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public void RemoveAt(int index) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public IVirtualCell this[int index] {
			get {
				return CreateVirtualCell(index);
			}
			set {
				XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			}
		}
		#endregion
		#region ICollection<IVirtualCell> Members
		public void Add(IVirtualCell item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public void Clear() {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public bool Contains(IVirtualCell item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return false;
		}
		public void CopyTo(IVirtualCell[] array, int arrayIndex) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
		}
		public int Count { get { return data.ColumnsCount; } }
		public bool IsReadOnly { get { return true; } }
		public bool Remove(IVirtualCell item) {
			XtraSpreadsheet.Model.VirtualWorksheet.InvalidOperationError();
			return false;
		}
		#endregion
		#region IEnumerable<IVirtualCell> Members
		public IEnumerator<IVirtualCell> GetEnumerator() {
			return new ObjectArrayVirtualCellCollectionEnumerator(rowIndex, data, firstRowIndex, firstColumnIndex, converter);
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (this as IEnumerable<IVirtualCell>).GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region ObjectArrayVirtualCellCollectionEnumerator
	class ObjectArrayVirtualCellCollectionEnumerator : IEnumerator<IVirtualCell> {
		readonly IDataValueConverter converter;
		readonly IArrayData data;
		readonly int firstRowIndex;
		readonly int firstColumnIndex;
		readonly int columnCount;
		readonly int rowIndex;
		int currentIndex = -1;
		public ObjectArrayVirtualCellCollectionEnumerator(int rowIndex, IArrayData data, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			this.data = data;
			this.rowIndex = rowIndex;
			this.firstRowIndex = firstRowIndex;
			this.firstColumnIndex = firstColumnIndex;
			this.converter = converter;
			this.columnCount = data.ColumnsCount;
		}
		IVirtualCell CreateVirtualCell(int index) {
			if (index < 0)
				return null;
			CellValue value = CellValue.CreateForVirtualCell(data[rowIndex, index], index, converter);
			return new TestVirtualCell(firstColumnIndex + index, rowIndex + firstRowIndex, value);
		}
		#region IEnumerator<IVirtualCell> Members
		public IVirtualCell Current { get { return CreateVirtualCell(currentIndex); } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			currentIndex++;
			return currentIndex < columnCount;
		}
		public void Reset() {
			currentIndex = -1;
		}
		#endregion
	}
	#endregion
	#region TestVirtualCell
	class TestVirtualCell : IVirtualCell {
		#region Fields
		readonly int columnIndex;
		readonly int rowIndex;
		CellValue value;
		#endregion
		public TestVirtualCell(int columnIndex, int rowIndex, CellValue value) {
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
			this.value = value;
		}
		#region IVirtualCell Members
		public CellValue Value { get { return value; } }
		public int ColumnIndex { get { return columnIndex; } }
		public int RowIndex { get { return rowIndex; } }
		#endregion
	}
	#endregion
#endif
}
