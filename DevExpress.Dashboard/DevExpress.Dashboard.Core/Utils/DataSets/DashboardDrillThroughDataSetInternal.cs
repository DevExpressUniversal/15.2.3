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
using DevExpress.XtraPivotGrid;
using ComplexType = System.Collections.Generic.KeyValuePair<DevExpress.XtraPivotGrid.PivotGridFieldBase, DevExpress.DashboardCommon.Native.DashboardUnderlyingDataPropertyDescriptor>;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardUnderlyingDataSetInternal : IDashboardDataSet {
		readonly PropertyDescriptorCollection props;
		readonly List<PivotGridFieldBase> actualFields;
		readonly List<IDashboardDataRow> rows;
		readonly bool isQueryMode;
		readonly bool isOlap;
		readonly PropertyDescriptorCollection propertyDescriptors;
		internal bool IsQueryMode { get { return isQueryMode; } }
		internal bool IsOlap { get { return isOlap; } }
		internal DashboardUnderlyingDataSetInternal(PivotDrillDownDataSource pivotDataSource, List<PivotGridFieldBase> actualFields, bool isQueryMode, bool isOlap) {
			this.props = new PropertyDescriptorCollection(((ITypedList)pivotDataSource).GetItemProperties(null).OfType<PropertyDescriptor>().ToArray());
			this.actualFields = actualFields;
			this.rows = new List<IDashboardDataRow>(pivotDataSource.RowCount);
			this.isQueryMode = isQueryMode;
			this.isOlap = isOlap;
			this.propertyDescriptors = GetItemProperties(null);
			foreach(PivotDrillDownDataRow pivotRow in pivotDataSource)
				rows.Add(new DashboardUnderlyingDataRowInternal(this, pivotRow, actualFields));
		}
		public int GetRowIndex(object value) {
			return rows.IndexOf(value);
		}
		public object this[int rowIndex, int columnIndex] {
			get { return rows[rowIndex][columnIndex]; }
		}
		public object this[int rowIndex, string columnName] {
			get { return rows[rowIndex][columnName]; }
		}
		public int RowCount { get { return rows.Count; } }
		public IDashboardDataRow this[int index] { get { return rows[index]; } }
		public event ListChangedEventHandler ListChanged { add { } remove { } }
		public List<string> GetColumnNames() {
			return propertyDescriptors.Cast<PropertyDescriptor>().Where(d => d.IsBrowsable).Select(d => d.Name).ToList();
		}
		internal int GetColumnIndex(string columnName) {
			for(int i = 0; i < actualFields.Count; i++) {
				PivotGridFieldBase field = actualFields[i];
				if(field.FieldName == columnName || field.UnboundFieldName == columnName || field.DrillDownColumnName == columnName)
					return i;
			}
			return -1;
		}
		PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			List<PropertyDescriptor> pivotProperties = props.OfType<PropertyDescriptor>().ToList();
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			int index = 0;
			Dictionary<PivotGridFieldBase, int> order = new Dictionary<PivotGridFieldBase, int>();
			for(int i = 0; i < actualFields.Count; i++) {
				var pivotField = actualFields[i];
				PropertyDescriptor prop = pivotProperties.FirstOrDefault((pivotProperty) => pivotField.FieldName == pivotProperty.Name || pivotField.DrillDownColumnName == pivotProperty.Name);
				if(prop != null) {
					result.Add(prop);
					order.Add(pivotField, isQueryMode ? index++ : pivotProperties.IndexOf(prop));
				}
			}
			if(!isQueryMode)
				result.Sort((a, b) => Comparer<int>.Default.Compare(pivotProperties.IndexOf(a), pivotProperties.IndexOf(b)));
			actualFields.Sort((a, b) => Comparer<int>.Default.Compare(order[a], order[b]));
			return new PropertyDescriptorCollection(result.ToArray());
		}
		IDashboardDataRow IDashboardDataSet.this[int index] { get { return rows[index]; } }
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() { 
			return rows.GetEnumerator();
		}
		IEnumerator<IDashboardDataRow> IEnumerable<IDashboardDataRow>.GetEnumerator() {
			return rows.GetEnumerator();
		}
		#endregion
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) { return propertyDescriptors; }
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "DashboardUnderlyingDataSetInternal"; }
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
	public class DashboardUnderlyingDataRowInternal : IDashboardDataRow {
		DashboardUnderlyingDataSetInternal dataSource;
		object[] values;
		int index;
		int listSourceRowIndex;
		internal DashboardUnderlyingDataRowInternal(DashboardUnderlyingDataSetInternal dataSource, PivotDrillDownDataRow pivotRow, IList<PivotGridFieldBase> actualFields) {
			this.dataSource = dataSource;
			this.index = pivotRow.Index;
			this.listSourceRowIndex = pivotRow.ListSourceRowIndex;
			values = new object[actualFields.Count];
			if(dataSource.IsQueryMode || dataSource.IsOlap) {
				for(int i = 0; i < actualFields.Count; i++)
					values[i] = pivotRow[actualFields[i]];
			} else {
				for(int i = 0; i < actualFields.Count; i++) {
					PivotGridFieldBase field = actualFields[i];
					if(!string.IsNullOrEmpty(field.FieldName))
						values[i] = pivotRow[field.FieldName];
					else
						values[i] = pivotRow[field];
				}
			}
		}
		#region IDashboardDataRow
		IDashboardDataSet IDashboardDataRow.DataSource { get { return dataSource; } }
		int IDashboardDataRow.Length { get { return values.Length; } }
		int IDashboardDataRow.Index { get { return index; } }
		int IDashboardDataRow.ListSourceRowIndex { get { return listSourceRowIndex; } }
		object IDashboardDataRow.this[int index] { get { return values[index]; } }
		object IDashboardDataRow.this[string columnName] {
			get {
				int cindex = dataSource.GetColumnIndex(columnName);
				return cindex >= 0 ? values[cindex] : null;
			}
		}
		IList IDashboardDataRow.ToList() {
			return values;
		}
		#endregion
	}
	public class DashboardUnderlyingDataPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor propertyDescriptor;
		bool isBrowsable;
		int columnIndex;
		string displayName;
		internal DashboardUnderlyingDataPropertyDescriptor(PropertyDescriptor propertyDescriptor, string displayName, int columnIndex, bool isBrowsable)
			: base(propertyDescriptor.Name, null) {
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
		public override string Name { get { return PropertyDescriptor.Name; } }
		public override string DisplayName { get { return string.IsNullOrEmpty(displayName) ? PropertyDescriptor.DisplayName : displayName; } }
		public override Type PropertyType { get { return PropertyDescriptor.PropertyType; } }
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
