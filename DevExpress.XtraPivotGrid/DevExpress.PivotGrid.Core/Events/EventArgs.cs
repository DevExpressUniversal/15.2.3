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
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Data.PivotGrid;
namespace DevExpress.XtraPivotGrid {
	public class CustomFieldDataEventArgsBase<T> : EventArgs where T : PivotGridFieldBase {
		T field;
		object _value = null;
		int listSourceRow;
		PivotGridData data;
		public CustomFieldDataEventArgsBase(PivotGridData data, T field, int listSourceRow, object _value) {
			this.field = field;
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.data = data;
		}
		public T Field { get { return field; } }
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public object Value { get { return _value; } set { _value = value; } }
		public object GetListSourceColumnValue(string columnName) {
			return GetListSourceColumnValue(ListSourceRowIndex, columnName);
		}
		public object GetListSourceColumnValue(int listSourceRowIndex, string columnName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, columnName);
		}
		protected PivotGridData Data { get { return data; } }
	}
	public class FieldValueCellBase<T> : FieldValueCellBase where T : PivotGridFieldBase {
		protected internal static FieldValueCellBase<T> CreateFieldValueCell(PivotFieldValueItem item) {
			return new FieldValueCellBase<T>(item);
		}
		public FieldValueCellBase(PivotFieldValueItem item) : base(item) { }
		public new T Field { get { return (T)Item.Data.GetField(base.Field); } }
		public new T DataField { get { return (T)Item.Data.GetField(base.DataField); } }
		public FieldValueCellBase<T> Parent { get { return base.ParentItem == null ? null : CreateFieldValueCell(base.ParentItem); } }
	}
	public abstract class PivotCustomFieldValueCellsEventArgsBase<T1, T2> : PivotCustomFieldValueCellsEventArgsBase 
		where T1 : PivotGridFieldBase
		where T2 : FieldValueCellBase {
		protected PivotCustomFieldValueCellsEventArgsBase(PivotVisualItemsBase items) : base(items) { }
		public T2 GetCell(bool isColumn, int index) {
			return GetCellCore(base.GetItem(isColumn, index));
		}
		protected abstract T2 GetCellCore(PivotFieldValueItem item);
		List<T2> FindAllCells(bool isColumn, Predicate<object[]> match) {
			List<T2> cells = new List<T2>();
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				delegate(int cellIndex) {
					cells.Add(GetCell(isColumn, cellIndex));
					return false;
				}
			));
			return cells;
		}
		public T2 FindCell(bool isColumn, Predicate<object[]> match) {
			int index = -1;
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				delegate(int cellIndex) {
					index = cellIndex;
					return true;
				}
			));
			return GetCell(isColumn, index);
		}
		public void Split(bool isColumn, Predicate<T2> match, bool firstCellOnly, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, firstCellOnly, (IList<FieldValueSplitData>)cells);
		}
		public void Split(bool isColumn, Predicate<T2> match, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, false, cells);
		}
		public void Split(bool isColumn, Predicate<T2> match, IList<FieldValueSplitData> cells) {
			this.Split(isColumn, match, false, cells);
		}
		public void Split(bool isColumn, Predicate<T2> match, bool firstCellOnly, IList<FieldValueSplitData> cells) {
			base.Split(isColumn, delegate(int index) { return match(this.GetCell(isColumn, index)); }, firstCellOnly, cells);
		}
	}
	public class PivotGridCustomFieldSortEventArgsBase<T> : EventArgs where T : PivotGridFieldBase {
		PivotGridData data;
		T field;
		PivotSortOrder sortOrder;
		bool handled = false;
		internal object value1, value2;
		int result = 0;
		int listSourceRow1, listSourceRow2;
		public PivotGridCustomFieldSortEventArgsBase(PivotGridData data, T field) {
			this.data = data;
			this.field = field;
			SetArgs(-1, -1, null, null, PivotSortOrder.Ascending);
		}
		public PivotSortOrder SortOrder { get { return sortOrder; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridData Data { get { return data; } }
		public T Field { get { return field; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		public int Result {
			get { return result; }
			set { result = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int? GetSortResult() {
			if(!Handled)
				return null;
			return Result;
		}
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.value1 = value1;
			this.value2 = value2;
			this.result = 0;
			this.handled = false;
			this.listSourceRow1 = listSourceRow1;
			this.listSourceRow2 = listSourceRow2;
		}
		public object GetListSourceColumnValue(int listSourceRowIndex, string columnName) {
			return Data.GetListSourceRowValue(listSourceRowIndex, columnName);
		}
	}
	public class PivotFieldEventArgsBase<T> : EventArgs where T : PivotGridFieldBase {
		T field;
		public PivotFieldEventArgsBase(T field) {
			this.field = field;
		}
		public T Field { get { return field; } }
	}
	public class PivotCustomGroupIntervalEventArgsBase<T> : PivotFieldEventArgsBase<T> where T : PivotGridFieldBase{
		object value, groupValue;
		public PivotCustomGroupIntervalEventArgsBase(T field, object value)
			: base(field) {
			this.groupValue = this.value = value;
		}
		public object Value { get { return value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
	}
	public class PivotFieldValueEventArgsBase<T> : PivotFieldEventArgsBase<T> where T : PivotGridFieldBase {
		PivotFieldValueItem item;
		public PivotFieldValueEventArgsBase(PivotFieldValueItem item)
			: base((T)item.Data.GetField(item.Field)) {
			this.item = item;
		}
		public PivotFieldValueEventArgsBase(T field) : base(field) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldValueItem Item { get { return item; } }
		public PivotGridCustomTotalBase CustomTotal { get { return Item != null ? Item.CustomTotal : null; } }
		public PivotGridData Data { get { return Item.Data; } }
		public T DataField { get { return Item != null ? (T)Data.GetField(Item.DataField) : null; } }
		public bool IsColumn { get { return Item != null ? Item.IsColumn : true; } }
		public int MinIndex { get { return Item != null ? Item.MinLastLevelIndex : -1; } }
		public int MaxIndex { get { return Item != null ? Item.MaxLastLevelIndex : -1; } }
		public int FieldIndex { get { return Item != null ? Item.VisibleIndex : -1; } }
		public virtual object Value { get { return Item != null ? Item.Value : null; } }
		public bool IsOthersValue { get { return Item != null ? Item.IsOthersRow : false; } }
		public PivotGridValueType ValueType { get { return Item != null ? Item.ValueType : PivotGridValueType.Value; } }
		public bool IsCollapsed { get { return Data != null ? Data.IsObjectCollapsed(IsColumn, Item.VisibleIndex) : false; } }
		public void ChangeExpandedState() { Data.ChangeExpanded(Item); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public T[] GetHigherLevelFields() {
			if(Field == null) return new T[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotFieldValueItem parent = Data.VisualItems.GetParentItem(IsColumn, Item);
				List<T> fields = new List<T>();
				while(parent != null) {
					if(parent.Field != null)
						fields.Insert(0, (T)Data.GetField(parent.Field));
					parent = Data.VisualItems.GetParentItem(IsColumn, parent);
				}
				return fields.ToArray();
			} else {
				List<PivotGridFieldBase> baseFields = Data.GetFieldsByArea(Field.Area, true);
				int index = baseFields.IndexOf(Field);
				List<T> fields = new List<T>();
				for(int i = 0; i < index; i++) {
					T field = baseFields[i] as T;
					if(field == Data.DataField)
						field = DataField;
					fields.Add(field);
				}
				return fields.ToArray();
			}
		}
		public object GetHigherLevelFieldValue(T field) {
			if(!IsFieldCorrect(field))
				return null;
			return Data.GetFieldValue(field, FieldIndex, FieldIndex);
		}
		bool IsFieldCorrect(T field) {
			if(!AreFieldsInTheSameAreaAndHigherLevel(field, Field) || !field.Visible)
				return false;
			return true;
		}
		bool AreFieldsInTheSameAreaAndHigherLevel(T field1, T field2) {
			return GetActualArea(field1) == GetActualArea(field2)
				&& (Data.OptionsDataField.AreaIndex < 0 || GetActualAreaIndex(field1) <= GetActualAreaIndex(field2));
		}
		int GetActualAreaIndex(T field) {
			return (field.Area == PivotArea.DataArea) ? Data.OptionsDataField.AreaIndex : field.AreaIndex;
		}
		PivotArea GetActualArea(T field) {
			return (field.Area == PivotArea.DataArea) ? Data.OptionsDataField.DataFieldArea : field.Area;
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Item.CreateDrillDownDataSource();
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Item.CreateQueryModeDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= Data.VisualItems.ColumnCount)
				throw new ArgumentOutOfRangeException("columnIndex");
			if(rowIndex < 0 || rowIndex >= Data.VisualItems.RowCount)
				throw new ArgumentOutOfRangeException("rowIndex");
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetFieldValue(T field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
	}
	public class PivotFieldDisplayTextEventArgsBase<T> : PivotFieldValueEventArgsBase<T> where T : PivotGridFieldBase {
		string displayText;
		object value;
		public PivotFieldDisplayTextEventArgsBase(PivotFieldValueItem item, string defaultText) : base(item) {
			this.value = Item.Value;
			this.displayText = defaultText;
		}
		public PivotFieldDisplayTextEventArgsBase(T field, IOLAPMember member) : base(field) {
			this.value = member.Value;
			this.displayText = field.GetValueText(member);
		}
		public PivotFieldDisplayTextEventArgsBase(T field, object value) : base(field) {
			this.value = value;
			this.displayText = field.GetValueText(value);
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return Item == null; } }
	}
	public class CustomServerModeSortEventArgsBase<T> : PivotFieldEventArgsBase<T> where T : PivotGridFieldBase {
		IQueryMemberProvider val0;
		IQueryMemberProvider val1;
		int? result;
		ICustomSortHelper helper;
		public object Value1 { get { return val0.Member.Value; } }
		public object Value2 { get { return val1.Member.Value; } }
		public IOLAPMember OLAPMember1 { get { return val0.Member as IOLAPMember; } }
		public IOLAPMember OLAPMember2 { get { return val1.Member as IOLAPMember; } }
		public int? Result {
			get { return result; }
			set { result = value; }
		}
		protected CustomServerModeSortEventArgsBase(T field) : base(field) {
		}
		protected internal CustomServerModeSortEventArgsBase(IQueryMemberProvider val0, IQueryMemberProvider val1, T field, ICustomSortHelper helper)
			: base(field) {
			SetArgs(val0, val1, helper);
		}
		protected void SetArgs(IQueryMemberProvider val0, IQueryMemberProvider val1, ICustomSortHelper helper) {
			this.val0 = val0;
			this.val1 = val1;
			this.helper = helper;
			this.result = null;
		}
		public CrossAreaKey GetCrossAreaKey(object[] crossAreaValues) {
			return new CrossAreaKey(helper.GetSortByObject(crossAreaValues, !Field.IsColumn));
		}
		public object GetCellValue1(CrossAreaKey crossAreaKey, PivotGridFieldBase dataField) {
			return GetCellValue(crossAreaKey.Data, val0, Field.IsColumn, dataField);
		}
		public object GetCellValue2(CrossAreaKey crossAreaKey, PivotGridFieldBase dataField) {
			return GetCellValue(crossAreaKey.Data, val1, Field.IsColumn, dataField);
		}
		public object GetCellValue1(object[] crossAreaValues, PivotGridFieldBase dataField) {
			object token = helper.GetSortByObject(crossAreaValues, !Field.IsColumn);
			if(token == null)
				return null;
			return GetCellValue(token, val0, Field.IsColumn, dataField);
		}
		public object GetCellValue2(object[] crossAreaValues, PivotGridFieldBase dataField) {
			object token = helper.GetSortByObject(crossAreaValues, !Field.IsColumn);
			if(token == null)
				return null;
			return GetCellValue(token, val1, Field.IsColumn, dataField);
		}
		object GetCellValue(object val1, object val2, bool isColumn, PivotGridFieldBase dataField) {
			return helper.GetValue(isColumn ? val1 : val2, isColumn ? val2 : val1, dataField);
		}
	}
	public class CrossAreaKey {
		object data;
		internal object Data { get { return data; } }
		internal CrossAreaKey(object data) {
			this.data = data;
		}
	}
	public abstract class PivotCellEventArgsBase<TField, TData, TCustomTotal> : EventArgs
		where TField : PivotGridFieldBase
		where TData : PivotGridData
		where TCustomTotal : PivotGridCustomTotalBase {
		PivotGridCellItem cellItem;
		protected PivotCellEventArgsBase(PivotGridCellItem cellItem) {
			this.cellItem = cellItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridCellItem Item { get { return cellItem; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TData Data { get { return Item.Data as TData; } }
		public TCustomTotal ColumnCustomTotal { get { return Item.ColumnCustomTotal as TCustomTotal; } }
		public TCustomTotal RowCustomTotal { get { return Item.RowCustomTotal as TCustomTotal; } }
		public TField DataField { get { return (TField)Data.GetField(Item.DataField); } }
		public int ColumnIndex { get { return Item.ColumnIndex; } }
		public int RowIndex { get { return Item.RowIndex; } }
		public int ColumnFieldIndex { get { return Item.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return Item.RowFieldIndex; } }
		public TField ColumnField { get { return (TField)Data.GetField(Item.ColumnField); } }
		public TField RowField { get { return (TField)Data.GetField(Item.RowField); } }
		public object Value { get { return Item.Value; } }
		public PivotSummaryType SummaryType { get { return Item.SummaryType; } }
		public PivotSummaryValue SummaryValue { get { return Data.GetCellSummaryValue(ColumnFieldIndex, RowFieldIndex, DataField); } }
		public PivotGridValueType ColumnValueType { get { return Item.ColumnValueType; } }
		public PivotGridValueType RowValueType { get { return Item.RowValueType; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetQueryModeDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex, maxRowCount,
				customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(-1, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(-1, customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource(ColumnFieldIndex, RowFieldIndex);
		}
		public object GetFieldValue(TField field) {
			return Item.GetFieldValue(Data.GetFieldItem(field));
		}
		public object GetFieldValue(TField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
		protected PivotFieldItemBase GetFieldItem(TField field) {
			return Data.GetFieldItem(field);
		}
		public bool IsOthersFieldValue(TField field) {
			return Item.IsOthersFieldValue(GetFieldItem(field));
		}
		public bool IsFieldValueExpanded(TField field) {
			return Item.IsFieldValueExpanded(GetFieldItem(field));
		}
		public bool IsFieldValueRetrievable(TField field) {
			return Item.IsFieldValueRetrievable(GetFieldItem(field));
		}
		public TField[] GetColumnFields() {
			return GetFields(PivotArea.ColumnArea, Data.GetColumnLevel(ColumnFieldIndex) + 1);
		}
		public TField[] GetRowFields() {
			return GetFields(PivotArea.RowArea, Data.GetRowLevel(RowFieldIndex) + 1);
		}
		TField[] GetFields(PivotArea area, int fieldCount) {
			if(fieldCount <= 0 || fieldCount > Data.GetFieldCountByArea(area)) return new TField[0];
			TField[] fields = new TField[fieldCount];
			for(int i = 0; i < fields.Length; i++)
				fields[i] = Data.GetFieldByArea(area, i) as TField;
			return fields;
		}
		public object GetCellValue(TField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, TField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetPrevRowCellValue(TField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextRowCellValue(TField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetPrevColumnCellValue(TField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextColumnCellValue(TField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetColumnGrandTotal(TField dataField) {
			return Data.GetCellValue(-1, RowFieldIndex, dataField);
		}
		public object GetColumnGrandTotal(object[] rowValues, TField dataField) {
			return Data.GetCellValue(null, rowValues, dataField);
		}
		public object GetRowGrandTotal(TField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, -1, dataField);
		}
		public object GetRowGrandTotal(object[] columnValues, TField dataField) {
			return Data.GetCellValue(columnValues, null, dataField);
		}
		public object GetGrandTotal(TField dataField) {
			return Data.GetCellValue(-1, -1, dataField);
		}
	}
}
