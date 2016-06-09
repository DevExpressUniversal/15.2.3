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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Excel;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native.Excel {
	public class DataView : ITypedList, IBindingList {
		readonly ExcelDataSource dataSource;
		class DataViewEnumerator : IEnumerator<ViewRow> {
			int position = -1;
			readonly int count;
			public DataViewEnumerator(int count) {
				this.count = count;
			}
			public bool MoveNext() {
				return ++this.position < this.count;
			}
			public void Reset() {
				this.position = -1;
			}
			object IEnumerator.Current {
				get { return Current; }
			}
			public ViewRow Current {
				get { return new ViewRow(this.position); }
			}
			public void Dispose() {
			}
		}
		public List<ViewColumn> Columns { get; private set; }
		public DataView(ExcelDataSource excelDataSource, SelectedDataEx selectedData) {
			dataSource = excelDataSource;
			Guard.ArgumentNotNull(selectedData, "selectedData");
			Columns = new List<ViewColumn>();
			SetColumns(selectedData);
		}
		internal void SetColumns(SelectedDataEx selectedData) {
			IEnumerable<ViewColumn> columnsToRemove = Columns.Select(t => t).ToArray();
			foreach(var column in columnsToRemove) {
				Columns.Remove(column);
			}
			for(int i = 0; i < selectedData.Schema.Length; i++) {
				string name = selectedData.Schema[i].Name;
				ViewColumn columnEx = new ViewColumn(name, selectedData.Schema[i].Type, selectedData.Lists[i]);
				Columns.Add(columnEx);
			}
			foreach(ViewColumn viewColumn in Columns) {
				OnListChanged(ListChangedType.PropertyDescriptorAdded, viewColumn);
			}
			OnListChanged(ListChangedType.Reset, -1);
		}
		public IEnumerator GetEnumerator() {
			return new DataViewEnumerator(Count);
		}
		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public int Count {
			get { return this.Columns.Count > 0 ? this.Columns[0].Count : 0; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public bool Contains(object value) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public int IndexOf(object value) {
			throw new NotSupportedException();
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public object this[int index] {
			get { return new ViewRow(index); }
			set { throw new NotSupportedException(); }
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public bool IsFixedSize {
			get { return true; }
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return dataSource.Name;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return new PropertyDescriptorCollection(Columns.ToArray());
		}
		void OnListChanged(ListChangedType changedType, int newIndex) {
			if(ListChanged != null) {
				ListChanged(this, new ListChangedEventArgs(changedType, newIndex));
			}
		}
		void OnListChanged(ListChangedType changedType, PropertyDescriptor propertyDescriptor) {
			if(ListChanged != null) {
				ListChanged(this, new ListChangedEventArgs(changedType, propertyDescriptor));
			}
		}
		#region Implementation of IBindingList
		public object AddNew() {
			return null;
		}
		public void AddIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		public int Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public void RemoveIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void RemoveSort() {
			throw new NotSupportedException();
		}
		public bool AllowNew {
			get { return false; }
		}
		public bool AllowEdit {
			get { return false; }
		}
		public bool AllowRemove {
			get { return false; }
		}
		public bool SupportsChangeNotification {
			get { return true; }
		}
		public bool SupportsSearching {
			get { return false; }
		}
		public bool SupportsSorting {
			get { return false; }
		}
		public bool IsSorted {
			get { throw new NotSupportedException(); }
		}
		public PropertyDescriptor SortProperty {
			get { throw new NotSupportedException(); }
		}
		public ListSortDirection SortDirection {
			get { throw new NotSupportedException(); }
		}
		public event ListChangedEventHandler ListChanged;
		#endregion
	}
}
