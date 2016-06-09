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
using DevExpress.Xpf.PivotGrid.Internal;
using System.Collections;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class PivotSummaryDataRow : CoreXtraPivotGrid.PivotSummaryDataRow {
		public PivotSummaryDataRow(CoreXtraPivotGrid.PivotSummaryDataSource dataSource, int index, int columnIndex, int rowIndex)
			: base(dataSource, index, columnIndex, rowIndex) { }
		protected new object this[CoreXtraPivotGrid.PivotGridFieldBase field] {
			get { return base[field]; }
		}
		public object this[PivotGridField field] {
			get { return base[field]; }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotSummaryDataRowDataSource")]
#endif
		public new PivotSummaryDataSource DataSource {
			get { return base.DataSource as PivotSummaryDataSource; }
		}
	}
	public class PivotSummaryDataSource : DevExpress.XtraPivotGrid.PivotSummaryDataSource {
		public PivotSummaryDataSource(PivotGridWpfData data, int columnIndex, int rowIndex)
			: base(data, columnIndex, rowIndex) { }
		protected override CoreXtraPivotGrid.PivotSummaryDataRow CreateSummaryDataRow(int index, int columnIndex, int rowIndex) {
			return new PivotSummaryDataRow(this, index, columnIndex, rowIndex);
		}
		public new PivotSummaryDataRow this[int index] {
			get { return base[index] as PivotSummaryDataRow; }
			set { base[index] = value; }
		}
		public new PivotGridField GetField(int columnIndex) {
			return base.GetField(columnIndex).GetWrapper();
		}
		protected new int GetColumnIndex(CoreXtraPivotGrid.PivotGridFieldBase field) {
			return base.GetColumnIndex(field);
		}
		public int GetColumnIndex(PivotGridField field) {
			return base.GetColumnIndex(field);
		}
		protected new object GetValue(int rowIndex, CoreXtraPivotGrid.PivotGridFieldBase field) {
			return base.GetValue(rowIndex, field);
		}
		public object GetValue(int rowIndex, PivotGridField field) {
			return base.GetValue(rowIndex, field);
		}
	}
	public class PivotDrillDownDataRow : ICustomTypeDescriptor {
		CoreXtraPivotGrid.PivotDrillDownDataRow dataRowInternal;
		PivotDrillDownDataSource dataSource;
		public PivotDrillDownDataRow(PivotDrillDownDataSource dataSource, CoreXtraPivotGrid.PivotDrillDownDataRow dataRow) {
			this.dataSource = dataSource;
			this.dataRowInternal = dataRow;
		}
		protected internal CoreXtraPivotGrid.PivotDrillDownDataRow DataRowInternal {
			get { return dataRowInternal; }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowListSourceRowIndex")]
#endif
		public int ListSourceRowIndex { get { return DataRowInternal.ListSourceRowIndex; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[int index] {
			get { return DataRowInternal[index]; }
			set { DataRowInternal[index] = value; }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[string fieldName] {
			get { return DataRowInternal[fieldName]; }
			set { DataRowInternal[fieldName] = value; }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[PivotGridField field] {
			get { return DataRowInternal[field]; }
			set { DataRowInternal[field] = value; }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowDataSource")]
#endif
		public PivotDrillDownDataSource DataSource { get { return dataSource; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataRowIndex")]
#endif
		public int Index { get { return DataRowInternal.Index; } }
		ICustomTypeDescriptor TypeDescriptor { get { return DataRowInternal as ICustomTypeDescriptor; } }
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(editorBaseType); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return TypeDescriptor.GetProperties(); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) { return TypeDescriptor.GetProperties(attributes); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return TypeDescriptor.GetPropertyOwner(pd); }
#if !SL
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(attributes); }
#endif
		#endregion
	}
	public class PivotDrillDownDataSource : IBindingList, IEnumerator, ITypedList, IList, ICollection, IEnumerable, IDisposable {
		CoreXtraPivotGrid.PivotDrillDownDataSource dataSourceInternal;
		protected internal CoreXtraPivotGrid.PivotDrillDownDataSource DataSourceInternal {
			get { return dataSourceInternal; }
		}
		internal PivotDrillDownDataSource(CoreXtraPivotGrid.PivotDrillDownDataSource dataSource) {
			this.dataSourceInternal = dataSource;
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataSourceRowCount")]
#endif
		public int RowCount { get { return DataSourceInternal.RowCount; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataSourceItem")]
#endif
		public PivotDrillDownDataRow this[int index] {
			get { return GetRow(DataSourceInternal[index]); }
			set { DataSourceInternal[index] = value.DataRowInternal; }
		}
		protected PivotDrillDownDataRow GetRow(CoreXtraPivotGrid.PivotDrillDownDataRow coreRow) {
			return new PivotDrillDownDataRow(this, coreRow);
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotDrillDownDataSourceIsLive")]
#endif
		public bool IsLive {
			get { return DataSourceInternal.IsLive; }
			set { DataSourceInternal.IsLive = value; }
		}
		public int GetRowIndex(object value) {
			return DataSourceInternal.GetRowIndex(value);
		}
		public object GetValue(int rowIndex, PivotGridField field) {
			return DataSourceInternal.GetValue(rowIndex, field);
		}
		public object GetValue(int rowIndex, int columnIndex) {
			return DataSourceInternal.GetValue(rowIndex, columnIndex);
		}
		public object GetValue(int rowIndex, string fieldName) {
			return DataSourceInternal.GetValue(rowIndex, fieldName);
		}
		public void SetValue(int rowIndex, PivotGridField field, object value) {
			DataSourceInternal.SetValue(rowIndex, field, value);
		}
		public void SetValue(int rowIndex, int columnIndex, object value) {
			DataSourceInternal.SetValue(rowIndex, columnIndex, value);
		}
		public void SetValue(int rowIndex, string fieldName, object value) {
			DataSourceInternal.SetValue(rowIndex, fieldName, value);
		}
		protected IBindingList IBindingListImpl { get { return DataSourceInternal as IBindingList; } }
		protected IEnumerable IEnumerableImpl { get { return DataSourceInternal as IEnumerable; } }
		protected IEnumerator IEnumeratorImpl { get { return DataSourceInternal as IEnumerator; } }
		protected ITypedList ITypedListImpl { get { return DataSourceInternal as ITypedList; } }
		protected IList IListImpl { get { return DataSourceInternal as IList; } }
		protected IDisposable IDisposableImpl { get { return DataSourceInternal as IDisposable; } }
		protected ICollection ICollectionImpl { get { return DataSourceInternal as ICollection; } }
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { IBindingListImpl.AddIndex(property); }
		object IBindingList.AddNew() { return IBindingListImpl.AddNew(); }
		bool IBindingList.AllowEdit { get { return IBindingListImpl.AllowEdit; } }
		bool IBindingList.AllowNew { get { return IBindingListImpl.AllowNew; } }
		bool IBindingList.AllowRemove { get { return IBindingListImpl.AllowRemove; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) { IBindingListImpl.ApplySort(property, direction); }
		int IBindingList.Find(PropertyDescriptor property, object key) { return IBindingListImpl.Find(property, key); }
		bool IBindingList.IsSorted { get { return IBindingListImpl.IsSorted; } }
		void IBindingList.RemoveIndex(PropertyDescriptor property) { IBindingListImpl.RemoveIndex(property); }
		void IBindingList.RemoveSort() { IBindingListImpl.RemoveSort(); }
		ListSortDirection IBindingList.SortDirection { get { return IBindingListImpl.SortDirection; } }
		PropertyDescriptor IBindingList.SortProperty { get { return IBindingListImpl.SortProperty; } }
		bool IBindingList.SupportsChangeNotification { get { return IBindingListImpl.SupportsChangeNotification; } }
		bool IBindingList.SupportsSearching { get { return IBindingListImpl.SupportsSearching; } }
		bool IBindingList.SupportsSorting { get { return IBindingListImpl.SupportsSorting; } }
		public event ListChangedEventHandler ListChanged {
			add { DataSourceInternal.ListChanged += value; }
			remove { DataSourceInternal.ListChanged -= value; }
		}
		#endregion
		#region IList Members
		bool IList.IsFixedSize { get { return IListImpl.IsFixedSize; } }
		bool IList.IsReadOnly { get { return IListImpl.IsReadOnly; } }
		int IList.Add(object value) { return IListImpl.Add(value); }
		void IList.Clear() { IListImpl.Clear(); }
		bool IList.Contains(object value) { return IListImpl.Contains(value); }
		void IList.Insert(int index, object value) { IListImpl.Insert(index, value); }
		void IList.Remove(object value) { IListImpl.Remove(value); }
		void IList.RemoveAt(int index) { IListImpl.RemoveAt(index); }
		object IList.this[int index] { get { return IListImpl[index]; } set { IListImpl[index] = value; } }
		int IList.IndexOf(object value) { return IListImpl.IndexOf(value); }
		#endregion
		#region ITypedList Members
		public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) { return ITypedListImpl.GetItemProperties(listAccessors); }
		public virtual string GetListName(PropertyDescriptor[] listAccessors) { return ITypedListImpl.GetListName(listAccessors); }
		#endregion
		#region ICollection
		void ICollection.CopyTo(Array array, int count) { ICollectionImpl.CopyTo(array, count); }
		int ICollection.Count { get { return ICollectionImpl.Count; } }
		bool ICollection.IsSynchronized { get { return ICollectionImpl.IsSynchronized; } }
		object ICollection.SyncRoot { get { return ICollectionImpl.SyncRoot; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			((IEnumerator)this).Reset();
			return this;
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { 
			get {
				CoreXtraPivotGrid.PivotDrillDownDataRow coreRow = (CoreXtraPivotGrid.PivotDrillDownDataRow)IEnumeratorImpl.Current;
				return GetRow(coreRow);
			} 
		}
		bool IEnumerator.MoveNext() { return IEnumeratorImpl.MoveNext(); }
		void IEnumerator.Reset() { IEnumeratorImpl.Reset(); }
		#endregion
		#region IDisposable implementation
		public void Dispose() { IDisposableImpl.Dispose(); }
		#endregion
	}
}
