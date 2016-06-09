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
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.Data;
using System.Drawing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Localization;
using System.IO;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Data;
#else
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using PropertyDescriptorCollection = DevExpress.Data.Browsing.PropertyDescriptorCollection;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotSummaryPropertyDescriptor : PropertyDescriptor {
		readonly PivotGridData data;
		readonly PivotGridFieldBase field;
		protected PivotGridData Data { get { return data; } }
		public PivotGridFieldBase Field { get { return field; } }
		internal PivotSummaryPropertyDescriptor(PivotGridData data, PivotGridFieldBase field)
			: base(data.GetFieldName(field), null) {
			this.data = data;
			this.field = field;
		}
		public override bool IsBrowsable { get { return Field.Visible; } }
		public override bool IsReadOnly { get { return true; } }
		public override string Category { get { return string.Empty; } }
		public override string Name { get { return Field.SummaryColumnName; } }
		public override string DisplayName {
			get { return string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption; }
		}
		public override Type PropertyType { get { return Data.GetFieldType(Field, false); } }
		public override Type ComponentType { get { return typeof(IList); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			PivotSummaryDataRow row = component as PivotSummaryDataRow;
			if(row == null) return null;
			if(Field.IsColumnOrRow) {
				if(Data.GetIsOthersValue(Field, row.ColumnIndex, row.RowIndex))
					return Field.DataType == typeof(string) ? PivotGridLocalizer.GetString(PivotGridStringId.TopValueOthersRow) : null;
				return Data.GetFieldValue(Field, row.ColumnIndex, row.RowIndex);
			} else
				return Data.GetCellValue(row.ColumnIndex, row.RowIndex, Field);
		}
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override string ToString() {
			return DisplayName;
		}
	}
	public class PivotSummaryPropertyDescriptorCollection : PropertyDescriptorCollection {
		Dictionary<PivotGridFieldBase, PivotSummaryPropertyDescriptor> fieldDescriptorsCache;
		public PivotSummaryPropertyDescriptorCollection(PropertyDescriptor[] properties) 
			: base(properties) { }
		public PivotSummaryPropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly) 
			: base(properties, readOnly) { }
		protected Dictionary<PivotGridFieldBase, PivotSummaryPropertyDescriptor> FieldDescriptorsCache {
			get {
				if(fieldDescriptorsCache == null)
					fieldDescriptorsCache = new Dictionary<PivotGridFieldBase, PivotSummaryPropertyDescriptor>();
				return fieldDescriptorsCache;
			}
		}
		public new PivotSummaryPropertyDescriptor this[int index] { get { return (PivotSummaryPropertyDescriptor)base[index]; } }
		public new PivotSummaryPropertyDescriptor this[string name] { get { return (PivotSummaryPropertyDescriptor)base[name]; } }
		public PivotSummaryPropertyDescriptor this[PivotGridFieldBase field] {
			get {
				PivotSummaryPropertyDescriptor res;
				if(FieldDescriptorsCache.TryGetValue(field, out res))
					return res;
				res = GetDescriptorCore(field);
				FieldDescriptorsCache.Add(field, res);
				return res;
			}
		}
		PivotSummaryPropertyDescriptor GetDescriptorCore(PivotGridFieldBase field) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Field == field)
					return this[i];
			}
			return null;
		}
	}
	public class PivotSummaryDataSource : PivotDataSource{
		readonly PivotGridData data;
		readonly int columnIndex, rowIndex;
		List<Point> children;
		PivotSummaryDataRow[] rows;
		PivotSummaryPropertyDescriptorCollection propertyDescriptors;
		bool isLiveWhenDataSourceChanged = true;
		internal override bool IsLiveWhenDataSourceChanged {
			get {
				return isLiveWhenDataSourceChanged;
			}
		}
		protected PivotGridData Data { get { return data; } }
		protected int ColumnIndex { get { return columnIndex; } }
		protected int RowIndex { get { return rowIndex; } }
		protected List<Point> Children { get { return children; } }
		protected PivotSummaryDataRow[] Rows { get { return rows; } }
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			if(propertyDescriptors == null) {
				propertyDescriptors = CreatePropertyDescriptors();
			}
			return propertyDescriptors;
		}
		protected PivotSummaryPropertyDescriptorCollection PropertyDescriptors {
			get { return (PivotSummaryPropertyDescriptorCollection)GetDescriptorCollection(); }
		}
		PivotSummaryPropertyDescriptorCollection CreatePropertyDescriptors() {
			PivotGridFieldReadOnlyCollection sortedFields = Data.GetSortedFields(false);
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PivotGridFieldBase field in sortedFields) {
				if(field.Area != PivotArea.FilterArea)
					props.Add(new PivotSummaryPropertyDescriptor(Data, field));
			}
			return new PivotSummaryPropertyDescriptorCollection(props.ToArray());
		}
		protected internal override void Clear(bool sourceDisposing) {
			if(sourceDisposing)
				ResetData();
			else 
				DataSourceChanged();
		}
		public PivotSummaryDataSource(PivotGridData data, int columnIndex, int rowIndex) {
			this.data = data;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			SetupDataSource(data, columnIndex, rowIndex);
		}
		~PivotSummaryDataSource() {
			if(IsDisposed) return;
			Dispose(false);
		}
		void SetupDataSource(PivotGridData data, int columnIndex, int rowIndex) {
			this.children = data.GetCellChildren(columnIndex, rowIndex);
			this.rows = new PivotSummaryDataRow[RowCount];
			this.propertyDescriptors = null;
		}
		protected override void ResetDataSource() {
			this.children = new List<Point>();
			this.rows = new PivotSummaryDataRow[0];
			this.propertyDescriptors = new PivotSummaryPropertyDescriptorCollection(new PropertyDescriptor[0]);
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataSourceRowCount")]
#endif
		public override int RowCount { get { return Children.Count; } }
		public object GetValue(int rowIndex, int columnIndex) {
			if(rowIndex < 0 || rowIndex >= RowCount || columnIndex < 0 || columnIndex >= PropertyDescriptors.Count) return null;
			return PropertyDescriptors[columnIndex].GetValue(GetRow(rowIndex));
		}
		public object GetValue(int rowIndex, string columnName) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[columnName] == null) return null;
			return PropertyDescriptors[columnName].GetValue(GetRow(rowIndex));
		}
		public object GetValue(int rowIndex, PivotGridFieldBase field) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[field] == null) return null;
			return PropertyDescriptors[field].GetValue(GetRow(rowIndex));
		}
		public bool GetIsOthersValue(int rowIndex, int columnIndex) {
			if(rowIndex < 0 || rowIndex >= RowCount || columnIndex < 0 || columnIndex >= PropertyDescriptors.Count) return false;
			return Data.GetIsOthersValue(PropertyDescriptors[columnIndex].Field, Children[rowIndex].X, Children[rowIndex].Y);
		}
		public bool GetIsOthersValue(int rowIndex, string columnName) {
			if(rowIndex < 0 || rowIndex >= RowCount || PropertyDescriptors[columnName] == null) return false;
			return Data.GetIsOthersValue(PropertyDescriptors[columnName].Field, Children[rowIndex].X, Children[rowIndex].Y);
		}
		protected PivotSummaryDataRow GetRow(int index) {
			if(index < 0 || index >= RowCount) return null;
			if(Rows[index] == null)
				Rows[index] = CreateSummaryDataRow(index, Children[index].X, Children[index].Y);
			return Rows[index];
		}
		protected virtual PivotSummaryDataRow CreateSummaryDataRow(int index, int columnIndex, int rowIndex) {
			return new PivotSummaryDataRow(this, index, columnIndex, rowIndex);
		}
		public PivotGridFieldBase GetField(int columnIndex) {
			return PropertyDescriptors[columnIndex].Field;
		}
#if !SL && !DXPORTABLE
		public void ExportToXml(string fileName, bool writeSchema) {
			GetDataTable().WriteXml(fileName, writeSchema ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema);
		}
		public void ExportToXml(Stream stream, bool writeSchema) {
			GetDataTable().WriteXml(stream, writeSchema ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema);
		}
		DataTable GetDataTable() {
			DataTable table = new DataTable("SummaryDataSource");
			for(int i = 0; i < PropertyDescriptors.Count; i++)
				table.Columns.Add(PropertyDescriptors[i].Name, PropertyDescriptors[i].PropertyType);
			object[] values = new object[PropertyDescriptors.Count];
			for(int i = 0; i < RowCount; i++) {
				for(int j = 0; j < values.Length; j++)
					values[j] = GetValue(i, j);
				table.Rows.Add(values);
			}
			return table;
		}
#endif
		#region PivotDataSource Members
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataSourceIsLive")]
#endif
		override public bool IsLive { get { return rowIndex == -1 && columnIndex == -1 && !IsDisposed; } set { } }
		internal override void Refresh() {
			SetupDataSource(Data, ColumnIndex, RowIndex);
			if(!Data.IsRefreshing)
			Data.DoActionInMainThread(OnRefresh);
		}
		internal override void ResetData() {
			ResetDataSource();
			if(!Data.IsRefreshing)
			Data.DoActionInMainThread(OnResetData);
		}
		internal override void DataSourceChanged() {
			OnDataSourceChanged();
		}
		#endregion
		#region ITypedList Members
		public override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) { return PropertyDescriptors; }
		public override string GetListName(PropertyDescriptor[] listAccessors) { return string.Empty; }
		#endregion
		public override int GetRowIndex(object value) {
			PivotSummaryDataRow row = value as PivotSummaryDataRow;
			if(row == null) return -1;
			return Children.IndexOf(row.Location);
		}
		public int GetColumnIndex(PivotGridFieldBase field) {
			for(int i = 0; i < PropertyDescriptors.Count; i++) {
				if(PropertyDescriptors[i].Field == field)
					return i;
			}
			return -1;
		}
		public PivotSummaryDataRow this[int index] {
			get { return GetRow(index); }
			set { }
		}
		protected override object GetItem(int index) {
			return GetRow(index);
		}
	}
	public class PivotSummaryDataRow {
		readonly int columnIndex, rowIndex;
		readonly int index;
		readonly PivotSummaryDataSource dataSource;
		public PivotSummaryDataRow(PivotSummaryDataSource dataSource, int index, 
					int columnIndex, int rowIndex) {
			this.dataSource = dataSource;
			this.index = index;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowColumnIndex")]
#endif
		public int ColumnIndex { get { return columnIndex; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowRowIndex")]
#endif
		public int RowIndex { get { return rowIndex; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowLocation")]
#endif
		public Point Location { get { return new Point(ColumnIndex, RowIndex); } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowDataSource")]
#endif
		public PivotSummaryDataSource DataSource { get { return dataSource; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowIndex")]
#endif
		public int Index { get { return index; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowItem")]
#endif
		public object this[int columnIndex] {
			get { return DataSource.GetValue(Index, columnIndex); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowItem")]
#endif
		public object this[string fieldName] {
			get { return DataSource.GetValue(Index, fieldName); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotSummaryDataRowItem")]
#endif
		public object this[PivotGridFieldBase field] {
			get { return DataSource.GetValue(Index, field); }
		}
	}
}
