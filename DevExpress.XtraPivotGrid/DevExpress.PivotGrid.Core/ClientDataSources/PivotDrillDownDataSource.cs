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
using System.Drawing;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Storage;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#if !SL
using System.Data;
using System.Windows.Forms;
#else
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Utils;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotDrillDownPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor propertyDescriptor;
		bool isBrowsable;
		int columnIndex;
		string displayName;
		internal PivotDrillDownPropertyDescriptor(string name, PropertyDescriptor propertyDescriptor, string displayName, int columnIndex, bool isBrowsable)
			: base(string.IsNullOrEmpty(name) ? propertyDescriptor.Name : name, null) {
			this.propertyDescriptor = propertyDescriptor;
			this.displayName = displayName;
			this.columnIndex = columnIndex;
			this.isBrowsable = isBrowsable;
		}
		protected PropertyDescriptor PropertyDescriptor { get { return propertyDescriptor; } }
		protected virtual int ColumnIndex { get { return columnIndex; } }
		public override bool IsBrowsable { get { return isBrowsable; } }
		public override bool IsReadOnly { get { return PropertyDescriptor.IsReadOnly; } }
		public override string Category { get { return string.Empty; } }
		public override string DisplayName { get { return string.IsNullOrEmpty(displayName) ? PropertyDescriptor.DisplayName : displayName; } }
		public override Type PropertyType { get { return PropertyDescriptor.PropertyType; } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			PivotDrillDownDataRow row = component as PivotDrillDownDataRow;
			if(row == null) return null;
			return row[ColumnIndex];
		}
		public override void SetValue(object component, object value) {
			PivotDrillDownDataRow row = component as PivotDrillDownDataRow;
			if(row == null) return;
			row[ColumnIndex] = value;
		}
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return Name;
		}
	}
	public abstract class PivotDataSource : IBindingList, IEnumerable, IEnumerator, ITypedList, IList, IDisposable {
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDataSourceIsLive")]
#endif
		public virtual bool IsLive { get { return false;} set { } }
		internal virtual bool IsLiveWhenDataSourceChanged { get { return false; } }
		internal virtual void Refresh() { }
		internal virtual void ResetData() { }
		internal virtual void DataSourceChanged() { }
		protected virtual void ResetDataSource() { }
		protected virtual void Clear() {
			ClearRows();
			OnResetData();
		}
		protected internal abstract void Clear(bool sourceDisposing);
		protected internal virtual void ClearRows() { }
		protected internal abstract PropertyDescriptorCollection GetDescriptorCollection();
		protected void OnRefresh() {
			if(this.ListChanged != null) {
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}
		protected void OnResetData() {
			if(this.ListChanged != null)
				this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}
		protected void OnDataSourceChanged() {
			OnRefresh();
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDataSourceRowCount")]
#endif
		public virtual int RowCount { get { return 0; } }
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
		public event ListChangedEventHandler ListChanged;
		#endregion
		#region IList Members
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		int IList.Add(object value) { return -1; }
		void IList.Clear() { }
		bool IList.Contains(object value) { return GetRowIndex(value) > -1; }
		void IList.Insert(int index, object value) { }
		void IList.Remove(object value) { }
		void IList.RemoveAt(int index) { }
		object IList.this[int index] { get { return GetItem(index); } set { } }
		protected virtual object GetItem(int index) { return null; }
		int IList.IndexOf(object value) { return GetRowIndex(value); }
		public virtual int GetRowIndex(object value) { return -1; }
		#endregion
		#region ITypedList Members
		public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) { return null; }
		public virtual string GetListName(PropertyDescriptor[] listAccessors) { return "PivotDataSource"; }
		#endregion
		#region ICollection
		void ICollection.CopyTo(Array array, int count) { }
		int ICollection.Count { get { return RowCount; } }
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			((IEnumerator)this).Reset();
			return this;
		}
		#endregion
		#region IEnumerator Members
		int currentPosition;
		object IEnumerator.Current {
			get { return GetItem(currentPosition); }
		}
		bool IEnumerator.MoveNext() {
			return ++currentPosition < RowCount;
		}
		void IEnumerator.Reset() { currentPosition = -1; }
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
			if(disposing)
				ResetDataSource();
			isDisposed = true;
		}
		#endregion
	}
	public class PivotDrillDownDataRow
#if !SL && !DXPORTABLE
		: ICustomTypeDescriptor
#endif
		{
		PivotDrillDownDataSource dataSource;
		int index;
		public PivotDrillDownDataRow(PivotDrillDownDataSource dataSource, int index) {
			this.dataSource = dataSource;
			this.index = index;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowListSourceRowIndex")]
#endif
		public int ListSourceRowIndex { get { return DataSource.GetListSourceRowIndex(Index); } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[int index] {
			get { return DataSource.GetValue(Index, index); }
			set { DataSource.SetValue(Index, index, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[string fieldName] {
			get { return DataSource.GetValue(Index, fieldName); }
			set { DataSource.SetValue(Index, fieldName, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowItem")]
#endif
		public object this[PivotGridFieldBase field] {
			get { return DataSource.GetValue(Index, field); }
			set { DataSource.SetValue(Index, field, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowDataSource")]
#endif
		public PivotDrillDownDataSource DataSource { get { return dataSource; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataRowIndex")]
#endif
		public int Index { get { return index; } }
#if !SL && !DXPORTABLE
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType		   , true			);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return DataSource.GetDescriptorCollection();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return DataSource.GetDescriptorCollection();
		}
		#endregion
#endif
	}
	public abstract class PivotDrillDownDataSource : PivotDataSource {
		public const int AllRows = -1;
		PivotDrillDownDataRow[] rows;
		public PivotDrillDownDataSource() {
		}
		~PivotDrillDownDataSource() {
			if(IsDisposed) return;
			Dispose(false);
		}
		protected PivotDrillDownDataRow[] Rows {
			get {
				if(rows == null) rows = new PivotDrillDownDataRow[RowCountInternal];
				return rows;
			}
		}
		protected abstract int RowCountInternal { get; }
		bool isLiveWhenDataSourceChanged = false;
		internal override bool IsLiveWhenDataSourceChanged {
			get {
				return isLiveWhenDataSourceChanged;
			}
		}
		protected internal override void ClearRows() {
			IsLive = false;
			rows = new PivotDrillDownDataRow[0];
		}
		internal abstract int GetListSourceRowIndex(int rowIndex);
		#region PivotDataSource Members
		internal override void DataSourceChanged() {
			IsLive = false;
			OnDataSourceChanged();
		}
		protected internal override void Clear(bool sourceDisposing) {
			if(sourceDisposing)
				Clear();
			else {
				ClearRows();
				DataSourceChanged();
			}
		}
		#endregion
		public override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return GetDescriptorCollection();
		}
		public override string GetListName(PropertyDescriptor[] listAccessors) { return "PivotDrillDownDataSource"; }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataSourceRowCount")]
#endif
		public override int RowCount { get { return Rows.Length; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotDrillDownDataSourceItem")]
#endif
		public PivotDrillDownDataRow this[int index] {
			get { return GetRow(index); }
			set { }
		}
		protected override object GetItem(int index) {
			return GetRow(index);
		}
		protected PivotDrillDownDataRow GetRow(int index) {
			if(index < 0 || index >= RowCount) return null;
			if(Rows[index] == null) Rows[index] = new PivotDrillDownDataRow(this, index);
			return Rows[index];
		}
		public override int GetRowIndex(object value) {
			for(int i = 0; i < RowCount; i++)
				if(Rows[i] == value) return i;
			return -1;
		}
		public abstract object GetValue(int rowIndex, PivotGridFieldBase field);
		public abstract object GetValue(int rowIndex, int columnIndex);
		public abstract object GetValue(int rowIndex, string fieldName);
		public abstract void SetValue(int rowIndex, PivotGridFieldBase field, object value);
		public abstract void SetValue(int rowIndex, int columnIndex, object value);
		public abstract void SetValue(int rowIndex, string fieldName, object value);
		protected bool IsRowIndexValid(int rowIndex) {
			return rowIndex >= 0 && rowIndex < RowCount;
		}
	}
	class EmptyDataTable : IDataTable {
		IList<object[]> list;
		IList<IColumn> columns;
		public EmptyDataTable() { }
		#region IDataTable Members
		IList<IColumn> IDataTable.Columns {
			get {
				if(columns == null)
					columns = new List<IColumn>();
						return columns;
			}
		}
		IList<object[]> IDataTable.Rows {
			get {
				if(list == null)
					list = new List<object[]>();
				return list;
			}
		}
		#endregion
	}
	public class SimplePropertyDescriptor : PropertyDescriptor {
		string displayName;
		Type propertyType;
		public SimplePropertyDescriptor(IColumn column)
			: this(column.Name, column.DisplayName, column.ColumnType) { }
		public SimplePropertyDescriptor(string name, string displayName, Type propertyType)
			: base(name, null) {
			this.displayName = displayName;
			this.propertyType = propertyType;
		}
		public override Type PropertyType { get { return propertyType; } }
		public override bool IsReadOnly { get { return true; } }
		public override string DisplayName { get { return displayName; } }
		public override bool CanResetValue(object component) { throw new Exception("The method or operation is not implemented."); }
		public override Type ComponentType { get { throw new Exception("The method or operation is not implemented."); } }
		public override object GetValue(object component) { throw new Exception("The method or operation is not implemented."); }
		public override void ResetValue(object component) { throw new Exception("The method or operation is not implemented."); }
		public override void SetValue(object component, object value) { throw new Exception("The method or operation is not implemented."); }
		public override bool ShouldSerializeValue(object component) { throw new Exception("The method or operation is not implemented."); }
	}
	public class QueryDrillDownDataSource : PivotDrillDownDataSource {
		IDataTable innerTable;
		protected IDataTable InnerTable { get { return innerTable; } }
		public QueryDrillDownDataSource(IDataTable innerTable) {
			this.innerTable = innerTable ?? new EmptyDataTable();
		}
		protected override int RowCountInternal { get { return innerTable.Rows.Count; } }
		internal override int GetListSourceRowIndex(int rowIndex) { return rowIndex; }
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			IList<IColumn> props = innerTable.Columns;
			PivotDrillDownPropertyDescriptor[] properties = new PivotDrillDownPropertyDescriptor[props.Count];
			for(int i = 0; i < props.Count; i++)
				properties[i] = new PivotDrillDownPropertyDescriptor(null, new SimplePropertyDescriptor(props[i]), null, i, true);
			return new PropertyDescriptorCollection(properties);
		}
		public override object GetValue(int rowIndex, PivotGridFieldBase field) {
			return GetValue(rowIndex, field.DrillDownColumnName ?? field.FieldName);
		}
		public override object GetValue(int rowIndex, int columnIndex) {
			return innerTable.Rows[rowIndex][columnIndex];
		}
		public override object GetValue(int rowIndex, string fieldName) {
			int index = DataTableWrapper.GetColumnIndex(innerTable.Columns, fieldName);
			if(index == -1)
				return null;
			return innerTable.Rows[rowIndex][index];
		}
		public override void SetValue(int rowIndex, PivotGridFieldBase field, object value) {
			throw new Exception("The operation is not supported.");
		}
		public override void SetValue(int rowIndex, int columnIndex, object value) {
			throw new Exception("The operation is not supported.");
		}
		public override void SetValue(int rowIndex, string fieldName, object value) {
			throw new Exception("The operation is not supported.");
		}
	}
	public abstract class NativePivotDrillDownDataSource : PivotDrillDownDataSource {
		public static object GetDefaultValue(Type type) {
			if(!type.IsValueType())
				return null;
			else
				return Activator.CreateInstance(type);
		}
		readonly PivotDataController controller;
		Hashtable defaultValues = new Hashtable();
		Dictionary<string, string> aliases = new Dictionary<string, string>();
		protected PivotDataController Controller { get { return controller; } }
		public NativePivotDrillDownDataSource(PivotDataController controller) {
			this.controller = controller;
			(this as IEnumerator).Reset();
		}
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			PropertyDescriptor[] properties = new PropertyDescriptor[Controller.Columns.Count];
			defaultValues.Clear();
			for(int i = 0; i < properties.Length; i++) {
				DataColumnInfo columnInfo = Controller.Columns[i];
				PivotGridFieldBase field = columnInfo.Tag as PivotGridFieldBase;
				if(field != null) {
					if(field.IsSummaryExpressionDataField) {
						object defaultValue = GetDefaultValue(columnInfo.PropertyDescriptor.PropertyType);
						defaultValues.Add(field, defaultValue);
						defaultValues.Add(i, defaultValue);
						defaultValues.Add(field.DataControllerColumnName, defaultValue);
					} else {
						if(!string.IsNullOrEmpty(field.DrillDownColumnName) && !string.IsNullOrEmpty(columnInfo.Name))
							aliases[field.DrillDownColumnName] = columnInfo.Name;
					}
				}
				properties[i] = new PivotDrillDownPropertyDescriptor(field != null ? field.DrillDownColumnName : null, columnInfo.PropertyDescriptor, field != null ? field.Caption : null, columnInfo.Index, columnInfo.Visible);
			}
			return new PropertyDescriptorCollection(properties);
		}		
		public override object GetValue(int rowIndex, PivotGridFieldBase field) {
			if(defaultValues.Contains(field))
				return defaultValues[field];
			return GetValue(rowIndex, field.ColumnHandle);
		}
		public override object GetValue(int rowIndex, int columnIndex) {
			if(!IsRowIndexValid(rowIndex)) return null;
			if(defaultValues.Contains(columnIndex))
				return defaultValues[columnIndex];
			Controller.EnsureStorageIsCreated(columnIndex);
			return Controller.GetRowValue(GetListSourceRowIndex(rowIndex), columnIndex);
		}
		public override object GetValue(int rowIndex, string fieldName) {
			if(!IsRowIndexValid(rowIndex)) return null;
			if(defaultValues.Contains(fieldName))
				return defaultValues[fieldName];
			string value;
			if(aliases.TryGetValue(fieldName, out value))
				fieldName = value;
			Controller.EnsureStorageIsCreated(fieldName);
			return Controller.GetRowValue(GetListSourceRowIndex(rowIndex), fieldName);
		}
		public override void SetValue(int rowIndex, PivotGridFieldBase field, object value) {
			if(!IsRowIndexValid(rowIndex) || defaultValues.Contains(field))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), field.ColumnHandle, value);
		}
		public override void SetValue(int rowIndex, int columnIndex, object value) {
			if(!IsRowIndexValid(rowIndex) || defaultValues.Contains(columnIndex))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), columnIndex, value);
		}
		public override void SetValue(int rowIndex, string fieldName, object value) {
			if(!IsRowIndexValid(rowIndex) || defaultValues.Contains(fieldName))
				return;
			Controller.SetRowValue(GetListSourceRowIndex(rowIndex), fieldName, value);
		}
	}
	public class ClientPivotDrillDownDataSource : NativePivotDrillDownDataSource {
		VisibleListSourceRowCollection visibleListSourceRows;
		int startVisibleIndex;
		GroupRowInfo groupRow;
		int fMaxRowCount;
		int lifeCount;
		bool isCloned;
		public ClientPivotDrillDownDataSource(PivotDataController controller, VisibleListSourceRowCollection visibleListSourceRows, GroupRowInfo groupRow, int maxRowCount)
			: base(controller) {
			this.visibleListSourceRows = visibleListSourceRows;
			this.startVisibleIndex = groupRow != null ? groupRow.ChildControllerRow : 0;
			this.groupRow = groupRow;
			this.fMaxRowCount = maxRowCount;
			this.lifeCount = 0;
			this.isCloned = false;
		}
		public override bool IsLive {
			get { return lifeCount == 0 && !IsDisposed; }
			set {
				if(IsLive == value)
					return;
				lifeCount ++;
			}
		}
		protected new PivotDataController Controller { get { return (PivotDataController)base.Controller; } }
		protected override int RowCountInternal {
			get {
				int actualRowCount = groupRow != null ? groupRow.ChildControllerRowCount : visibleListSourceRows != null ? visibleListSourceRows.VisibleRowCount : 0;
				if(fMaxRowCount == PivotDrillDownDataSource.AllRows)
					return actualRowCount;
				else
					return Math.Min(fMaxRowCount, actualRowCount);
			}
		}
		internal override int GetListSourceRowIndex(int rowIndex) {
			return Controller.GetListSourceRowByControllerRow(visibleListSourceRows, startVisibleIndex + rowIndex);
		}
		internal override void ResetData() {
			base.ResetData();
			if(visibleListSourceRows != null && !isCloned) {
				visibleListSourceRows = visibleListSourceRows.ClonePersistent();
				isCloned = true;
			}
		}
		internal bool IsCloned { get { return isCloned; } }
	}
}
