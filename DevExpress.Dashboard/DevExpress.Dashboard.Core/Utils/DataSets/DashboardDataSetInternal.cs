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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardDataSet<T> : IBindingList, IEnumerable, IEnumerable<T>, ITypedList, IList, IDisposable where T : DashboardDataRow {
		IDashboardDataSet dataSource;
		T[] rows;
		internal DashboardDataSet(IDashboardDataSet dataSource) {
			this.dataSource = dataSource;
			rows = new T[dataSource.RowCount];
		}
		public object GetValue(int rowIndex, int columnIndex) {
			return dataSource[rowIndex, columnIndex];
		}
		public object GetValue(int rowIndex, string columnName) {
			return dataSource[rowIndex, columnName];
		}
		public int RowCount { get { return dataSource.RowCount; } }
		public T this[int index] { get { return rows[index]; } }
		public event ListChangedEventHandler ListChanged {
			add { dataSource.ListChanged += value; }
			remove { dataSource.ListChanged -= value; }
		}
		public List<string> GetColumnNames() {
			return dataSource.GetColumnNames();
		}
		protected IDashboardDataSet DataSource { get { return dataSource; } }
		protected T[] Rows { get { return rows; } }
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { }
		object IBindingList.AddNew() { return null; }
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) { }
		int IBindingList.Find(PropertyDescriptor property, object key) { return -1; }
		bool IBindingList.IsSorted { get { return false; } }
		void IBindingList.RemoveIndex(PropertyDescriptor property) { }
		void IBindingList.RemoveSort() { }
		ListSortDirection IBindingList.SortDirection { get { return new ListSortDirection(); } }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		#endregion
		#region IList Members
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		int IList.Add(object value) { return -1; }
		internal int Add(T value) { return ((IList)this).Add(value); }
		void IList.Clear() { }
		bool IList.Contains(object value) {
			T row = value as T;
			if(row != null)
				return Contains(row);
			return false;
		}
		public bool Contains(T value) { return rows.Contains<T>(value); }
		void IList.Insert(int index, object value) { }
		internal void Insert(int index, T value) { ((IList)this).Insert(index, value); }
		void IList.Remove(object value) { }
		internal void Remove(T value) { ((IList)this).Remove(value); }
		void IList.RemoveAt(int index) { }
		object IList.this[int index] { get { return rows[index]; } set { } }
		int IList.IndexOf(object value) {
			T row = value as T;
			if(row != null)
				return IndexOf(row);
			return -1;
		}
		public int IndexOf(T value) { return Array.IndexOf(rows, value); }
		#endregion
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) { return dataSource.GetItemProperties(listAccessors); }
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "DashboardDataSet"; }
		#endregion
		#region ICollection
		void ICollection.CopyTo(Array array, int index) {
			IList source = this;
			for(int i = 0; i < source.Count; i++)
				array.SetValue(source[i], index + i);
		}
		int ICollection.Count { get { return RowCount; } }
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return rows.GetEnumerator();
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			IEnumerable<T> val = rows;
			return val.GetEnumerator();
		}
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			dataSource.Dispose();
		}
		#endregion
	}
	public class DashboardDataSetInternal : IDashboardDataSet {
		List<DashboardDataRowInternal> rows = new List<DashboardDataRowInternal>();
		List<string> columns = new List<string>();
		ListChangedEventHandler listChanged;
		internal DashboardDataSetInternal() { }
		internal DashboardDataSetInternal(params string[] columnNames)
			: this(new List<string>(columnNames)) {
		}
		internal DashboardDataSetInternal(IList<string> columnNames) {
			columns.AddRange(columnNames);
		}
		internal DashboardDataSetInternal(IList<string> columnNames, IList row)
			: this(columnNames) {
			rows.Add(new DashboardDataRowInternal(this, row));
		}
		public void AddRow(DashboardDataRowInternal row) {
			rows.Add(row);
			row.Changed += OnItemChanged;
			RaiseListChanged(ListChangedType.ItemAdded, rows.Count - 1);
		}
		public void AddRow(IList values) {
			DashboardDataRowInternal row = new DashboardDataRowInternal(this, values);
			AddRow(row);
		}
		public int GetRowIndex(object row) {
			DashboardDataRowInternal dashboardDataRow = row as DashboardDataRowInternal;
			if(dashboardDataRow != null) {
				return rows.IndexOf(dashboardDataRow);
			}
			return -1;
		}
		public object GetRow(int index) {
			if(index >= 0 && index < RowCount) {
				return rows[index];
			}
			return null;
		}
		public object this[int rowIndex, int columnIndex] {
			get {
				if(rowIndex >= 0 && rowIndex < rows.Count && columnIndex >= 0 && columnIndex < columns.Count)
					return rows[rowIndex][columnIndex];
				return null;
			}
		}
		public object this[int rowIndex, string columnName] {
			get {
				if(rowIndex >= 0 && rowIndex < rows.Count && columns.Contains(columnName))
					return rows[rowIndex][columnName];
				return null;
			}
		}
		public int RowCount { get { return rows.Count; } }
		IDashboardDataRow IDashboardDataSet.this[int index] { get { return rows[index]; } }
		public event ListChangedEventHandler ListChanged { add { listChanged += value; } remove { listChanged -= value; } }
		public List<string> GetColumnNames() { return new List<string>(columns); }
		public DashboardDataRowInternal this[int index] {
			get {
				if(index > 0 && index < rows.Count)
					return rows[index];
				throw new ArgumentException();
			}
		}
		internal List<string> Columns { get { return GetColumnNames(); } }
		void RaiseListChanged(ListChangedType changedType, int index) {
			if(listChanged != null)
				listChanged(this, new ListChangedEventArgs(changedType, index));
		}
		void OnItemChanged(object sender, EventArgs e) {
			RaiseListChanged(ListChangedType.ItemChanged, GetRowIndex(sender));
		}
		PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			DashboardDataSetPropertyDescriptor[] properties =
				new DashboardDataSetPropertyDescriptor[columns.Count];
			int i = 0;
			foreach(string columnName in columns) {
				properties[i] = new DashboardDataSetPropertyDescriptor(columnName, columnName, i);
				i++;
			}
			return new PropertyDescriptorCollection(properties);
		}
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return rows.GetEnumerator();
		}
		IEnumerator<IDashboardDataRow> IEnumerable<IDashboardDataRow>.GetEnumerator() {
			return rows.GetEnumerator();
		}
		#endregion
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) { return GetItemProperties(listAccessors); }
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "DashboardDataSetInternal"; }
		#endregion
		#region IDisposable implementation
		bool isDisposed;
		protected bool IsDisposed { get { return isDisposed; } }
		public void Dispose() {
			if(IsDisposed) return;
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			isDisposed = true;
		}
		#endregion
	}
	public class DashboardDataRowInternal : IDashboardDataRow {
		readonly string messageIndexOutOfRange = "DashboardDataRowInternal: Index out of range.";
		DashboardDataSetInternal dataSet;
		object[] values;
		internal DashboardDataRowInternal(DashboardDataSetInternal dataSet, IList values) {
			this.dataSet = dataSet;
			if(values == null)
				return;
			int columnCount = dataSet.Columns.Count;
			this.values = new object[values.Count];
			for(int i = 0; i < values.Count; i++)
				this.values[i] = values[i];
		}
		internal DashboardDataRowInternal(DashboardDataSetInternal dataSet)
			: this(dataSet, null) {
		}
		public object this[string columnName] {
			get {
				if(dataSet.Columns.Contains(columnName))
					return values[dataSet.Columns.IndexOf(columnName)];
				throw new InvalidOperationException(messageIndexOutOfRange);
			}
			set {
				if(dataSet.Columns.Contains(columnName) && values[dataSet.Columns.IndexOf(columnName)] != value) {
					values[dataSet.Columns.IndexOf(columnName)] = value;
					RaiseChanged();
				}
				else {
					throw new InvalidOperationException(messageIndexOutOfRange);
				}
			}
		}
		public object this[int index] {
			get {
				if(index >= 0 && index < values.GetLength(0))
					return values[index];
				throw new InvalidOperationException(messageIndexOutOfRange);
			}
			set {
				if(index >= 0 && index < values.GetLength(0) && values[index] != value) {
					values[index] = value;
					RaiseChanged();
				}
				else {
					throw new InvalidOperationException(messageIndexOutOfRange);
				}
			}
		}
		IDashboardDataSet IDashboardDataRow.DataSource { get { return dataSet; } }
		int IDashboardDataRow.Index { get { return dataSet.GetRowIndex(this); } }
		int IDashboardDataRow.ListSourceRowIndex { get { return -1; } }
		public int Length { get { return dataSet.Columns.Count; } }
		public event EventHandler Changed;
		public IList ToList() {
			return values.ToList<object>();
		}
		void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}
	public class DashboardDataSetPropertyDescriptor : PropertyDescriptor {
		int columnIndex;
		string displayName;
		internal DashboardDataSetPropertyDescriptor(string name, string displayName, int columnIndex)
			: base(name, null) {
			this.displayName = displayName;
			this.columnIndex = columnIndex;
		}
		protected virtual int ColumnIndex { get { return columnIndex; } }
		public override bool IsBrowsable { get { return true; } }
		public override bool IsReadOnly { get { return true; } }
		public override string Category { get { return string.Empty; } }
		public override string DisplayName { get { return displayName; } }
		public override Type PropertyType { get { return typeof(object); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			DashboardDataRow row = component as DashboardDataRow;
			if(row == null) return null;
			return row[ColumnIndex];
		}
		public override void SetValue(object component, object value) {
			throw new InvalidOperationException("This operation is not supported.");
		}
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return Name;
		}
	}
}
