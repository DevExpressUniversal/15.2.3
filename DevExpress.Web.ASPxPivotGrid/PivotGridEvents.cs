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

using DevExpress.Data.PivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.ASPxPivotGrid.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxPivotGrid.Internal {
	public static class PivotFieldConverter {
		static PivotFieldConverter() {
		}
		internal static PivotGridField GetField(PivotGridData data, PivotFieldItemBase fieldItem) {
			return data.GetField(fieldItem) as PivotGridField;
		}
		internal static PivotGridField GetField(PivotFieldValueItem valueItem) {
			return GetField(valueItem.Data, valueItem.Field);
		}
		internal static PivotGridField GetDataField(PivotFieldValueItem valueItem) {
			return GetField(valueItem.Data, valueItem.DataField);
		}
		internal static PivotFieldItemBase GetItem(PivotGridData data, PivotGridFieldBase field) {
			return data.GetFieldItem(field);
		}
	}
}
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotFieldEventArgs : PivotFieldEventArgsBase<PivotGridField> {
		public PivotFieldEventArgs(PivotGridField field) : base(field) {
		}
	}
	public class PivotFieldPropertyChangedEventArgs : PivotFieldEventArgs {
		PivotFieldPropertyName propertyName;
		public PivotFieldPropertyChangedEventArgs(PivotGridField field, PivotFieldPropertyName propertyName)
			: base(field) {
			this.propertyName = propertyName;
		}
		public PivotFieldPropertyName PropertyName {
			get { return propertyName; }
		}
	}
	public class PivotCustomGroupIntervalEventArgs : PivotFieldEventArgs {
		object value, groupValue;
		public PivotCustomGroupIntervalEventArgs(PivotGridField field, object value)
			: base(field) {
			this.groupValue = this.value = value;
		}
		public object Value { get { return value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
	}
	public class PivotFieldValueEventArgs : PivotFieldValueEventArgsBase<PivotGridField> {
		public PivotFieldValueEventArgs(PivotFieldValueItem item)
			: base(item) {
		}
		public PivotFieldValueEventArgs(PivotGridField field)
			: base(field) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldValueItem FieldValueItem { get { return Item; } }
		public new PivotGridCustomTotal CustomTotal { get { return Item != null ? (PivotGridCustomTotal)Item.CustomTotal : null; } }
		public new PivotGridWebData Data { get { return (PivotGridWebData)Item.Data; } }
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldValueEventArgs {
		string displayText;
		object value;
		public PivotFieldDisplayTextEventArgs(PivotFieldValueItem fieldValueItem, string defaultText) 
			: base(fieldValueItem) {
			this.value = fieldValueItem.Value;
			this.displayText = defaultText;
		}
		public PivotFieldDisplayTextEventArgs(PivotGridField field, IOLAPMember member)
			: base(field) {
			this.value = member.Value;
			this.displayText = Field.GetValueText(member);
		}
		public PivotFieldDisplayTextEventArgs(PivotGridField field, object value)
			: base(field) {
			this.value = value;
			this.displayText = Field.GetValueText(value);
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return FieldValueItem == null; } }
	}
	public class PivotFieldFilterChangingEventArgs : PivotFieldEventArgs {
		bool cancel;
		IList<object> values;
		PivotFilterType filterType;
		public PivotFieldFilterChangingEventArgs(PivotGridField field, PivotFilterType filterType, bool showBlanks, IList<object> values)
			: base(field) {
			this.filterType = filterType;
			this.values = values;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public PivotFilterType FilterType { get { return filterType; } }
		public IList<object> Values { get { return values; } }
	}
	public class PivotCustomChartDataSourceDataEventArgs : EventArgs {
		PivotChartItemType itemType;
		PivotChartItemDataMember itemDataMember;
		PivotFieldValueEventArgs fieldValueInfo;
		PivotCellValueEventArgs cellInfo;
		object value;
		public PivotCustomChartDataSourceDataEventArgs(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			this.itemType = itemType;
			this.itemDataMember = itemDataMember;
			this.fieldValueInfo = null;
			this.cellInfo = null;
			switch(ItemType) {
				case PivotChartItemType.ColumnItem:
				case PivotChartItemType.RowItem:
					this.fieldValueInfo = new PivotFieldValueEventArgs(fieldValueItem);
					this.cellInfo = null;
					break;
				case PivotChartItemType.CellItem:
					this.fieldValueInfo = null;
					this.cellInfo = new PivotCellValueEventArgs(cellItem);
					break;
			}
			this.value = value;
		}
		public PivotChartItemType ItemType { get { return itemType; } }
		public PivotChartItemDataMember ItemDataMember { get { return itemDataMember; } }
		public PivotFieldValueEventArgs FieldValueInfo { get { return fieldValueInfo; } }
		public PivotCellValueEventArgs CellInfo { get { return cellInfo; } }
		public object Value {
			get { return value; }
			set { this.value = value; }
		}
	}
	public class PivotCustomChartDataSourceRowsEventArgs : EventArgs {
		readonly PivotChartDataSource ds;
		readonly IList<PivotChartDataSourceRow> rows;
		internal PivotCustomChartDataSourceRowsEventArgs(PivotChartDataSource ds, IList<PivotChartDataSourceRowBase> rows) {
			this.ds = ds;
			this.rows = new PivotChartDataSourceRowBaseListWrapper<PivotChartDataSourceRow>(rows);
		}
		public IList<PivotChartDataSourceRow> Rows {
			get { return rows; }
		}
		public PivotChartDataSourceRow CreateRow(object series, object argument, object value) {
			return new PivotChartDataSourceRow(ds) {
				Series = series,
				Argument = argument,
				Value = value
			};
		}
	}
	public class PivotFieldStateChangedEventArgs : PivotFieldEventArgs {
		object[] values;
		PivotFieldValueItem item;
		public PivotFieldStateChangedEventArgs(PivotFieldValueItem item)
			: base(PivotFieldConverter.GetField(item)) {
			this.item = item;
		}
		public PivotFieldStateChangedEventArgs(PivotGridField field)
			: base(field) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotFieldValueItem Item { get { return item; } }
		public PivotGridField DataField { get { return Item != null ? PivotFieldConverter.GetDataField(item) : null; } }
		public bool IsColumn { get { return Item != null ? Item.IsColumn : true; } }
		public int MinIndex { get { return Item != null ? Item.MinLastLevelIndex : -1; } }
		public int MaxIndex { get { return Item != null ? Item.MaxLastLevelIndex : -1; } }
		public int FieldIndex { get { return Item != null ? Item.VisibleIndex : -1; } }
		public virtual object Value { get { return Item != null ? Item.Value : null; } }
		public bool IsOthersValue { get { return Item != null ? Item.IsOthersRow : false; } }
		public PivotGridValueType ValueType { get { return Item != null ? Item.ValueType : PivotGridValueType.Value; } }
		public PivotGridCustomTotal CustomTotal { get { return Item != null ? (PivotGridCustomTotal)Item.CustomTotal : null; } }
		public bool IsCollapsed { get { return Data != null ? Data.IsObjectCollapsed(IsColumn, Item.VisibleIndex) : false; } }
		public object[] Values {
			get {
				if(values == null)
					values = Data.VisualItems.GetItemValues(Item);
				return values;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridWebData Data { get { return (PivotGridWebData)Item.Data; } }
		public PivotGridField[] GetHigherLevelFields() {
			if(Field == null) return new PivotGridField[0];
			if(Field.Area == PivotArea.DataArea) {
				PivotGridField[] fields = new PivotGridField[Data.GetFieldCountByArea(Data.OptionsDataField.DataFieldArea)];
				for(int i = 0; i < fields.Length; i++)
					fields[i] = (PivotGridField)Data.GetFieldByArea(Data.OptionsDataField.DataFieldArea, i);
				return fields;
			} else {
				PivotGridField[] fields = new PivotGridField[Field.AreaIndex];
				for(int i = Field.AreaIndex - 1; i >= 0; i--) {
					fields[i] = Data.GetFieldByLevel(IsColumn, i) as PivotGridField;
				}
				return fields;
			}
		}
		public object GetHigherLevelFieldValue(PivotGridField field) {
			if(field.Area != Field.Area || field.AreaIndex > Field.AreaIndex || !field.Visible) return null;
			return Data.GetFieldValue(field, FieldIndex, FieldIndex);
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
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
	}
	public class PivotFieldStateChangedCancelEventArgs : PivotFieldStateChangedEventArgs {
		bool cancel = false;
		public PivotFieldStateChangedCancelEventArgs(PivotFieldValueItem item) 
			: base(item) { }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PivotCellBaseEventArgs : PivotCellEventArgsBase<PivotGridField, PivotGridWebData, PivotGridCustomTotal> {
		public PivotCellBaseEventArgs(PivotGridCellItem cellItem) : base(cellItem) {
		}
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		public PivotCellDisplayTextEventArgs(PivotGridCellItem cellItem) : base(cellItem) {
			this.displayText = cellItem.Text;
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	public class PivotCellValueEventArgs : PivotCellBaseEventArgs {
		object value;
		public PivotCellValueEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
			this.value = cellItem.ValueInternal;
		}
		public new object Value { get { return value; } set { this.value = value; } }
	}
	public class PivotCustomFilterPopupItemsEventArgs : EventArgs {
		PivotGridFilterItems items;
		public PivotCustomFilterPopupItemsEventArgs(PivotGridFilterItems items) {
			this.items = items;
		}
		public void CheckAllItems(bool isChecked) {
			items.CheckAllItems(isChecked);
		}
		public PivotGridField Field { get { return items.Field as PivotGridField; } }
		public IList<PivotGridFilterItem> Items { get { return items; } }
		public PivotGridFilterItem ShowBlanksItem { get { return items.BlankItem; } }
	}
	public class FieldValueCell : FieldValueCellBase {
		protected internal static FieldValueCell CreateFieldValueCell(PivotFieldValueItem item) {
			return new FieldValueCell(item);
		}
		public FieldValueCell(PivotFieldValueItem item) : base(item) { }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("FieldValueCellField")]
#endif
		public new PivotGridField Field { get { return PivotFieldConverter.GetField(Item); } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("FieldValueCellDataField")]
#endif
		public new PivotGridField DataField { get { return PivotFieldConverter.GetDataField(Item); } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("FieldValueCellParent")]
#endif
		public FieldValueCell Parent { get { return base.ParentItem == null ? null : CreateFieldValueCell(base.ParentItem); } }
	}
	public class PivotCustomFieldValueCellsEventArgs : PivotCustomFieldValueCellsEventArgsBase {
		internal PivotCustomFieldValueCellsEventArgs(PivotVisualItemsBase items) : base(items) { }
		public FieldValueCell GetCell(bool isColumn, int index) {
			return GetCellCore(base.GetItem(isColumn, index));
		}
		protected FieldValueCell GetCellCore(PivotFieldValueItem item) {
			if(item == null) return null;
			return FieldValueCell.CreateFieldValueCell(item);
		}
		List<FieldValueCell> FindAllCells(bool isColumn, Predicate<object[]> match) {
			List<FieldValueCell> cells = new List<FieldValueCell>();
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				delegate(int cellIndex) {
					cells.Add(GetCell(isColumn, cellIndex));
					return false;
				}
			));
			return cells;
		}
		public FieldValueCell FindCell(bool isColumn, Predicate<object[]> match) {
			int index = -1;
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				 delegate(int cellIndex) {
					 index = cellIndex;
					 return true;
				 }
			 ));
			return GetCell(isColumn, index);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, bool firstCellOnly, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, firstCellOnly, (IList<FieldValueSplitData>)cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, false, (IList<FieldValueSplitData>)cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, IList<FieldValueSplitData> cells) {
			this.Split(isColumn, match, false, cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, bool firstCellOnly, IList<FieldValueSplitData> cells) {
			base.Split(isColumn, delegate(int index) { return match(this.GetCell(isColumn, index)); }, firstCellOnly, cells);
		}
	}
	public class PivotCustomCellStyleEventArgs : PivotCellBaseEventArgs {
		PivotCellStyle cellStyle;
		public PivotCustomCellStyleEventArgs(PivotGridCellItem cellItem, PivotCellStyle cellStyle)
			: base(cellItem) {
			this.cellStyle = cellStyle;
		}
		public PivotCellStyle CellStyle { get { return cellStyle; } }
	}
	public enum MenuItemEnum { HeaderRefresh, HeaderHide, HeaderShowList, 
		HeaderShowPrefilter, FieldValueExpand, FieldValueExpandAll, FieldValueSortBySummaryFields, HeaderSortAscending, HeaderSortDescending, HeaderClearSorting }
	public class PivotAddPopupMenuItemEventArgs : EventArgs {
		readonly MenuItemEnum menuItem;
		bool add;
		public MenuItemEnum MenuItem { get { return menuItem; } }
		public bool Add { get { return add; } set { add = value; } }
		public PivotAddPopupMenuItemEventArgs(MenuItemEnum menuItem) {
			this.menuItem = menuItem;
			this.add = true;
		}
	}
	public enum PivotGridPopupMenuType { HeaderMenu, FieldValueMenu, FieldListMenu };
	public class PivotPopupMenuCreatedEventArgs : EventArgs {
		readonly ASPxPivotGridPopupMenu menu;
		public ASPxPivotGridPopupMenu Menu { get { return menu; } }
		public PivotGridPopupMenuType MenuType { get { return Menu.MenuType; } }
		public PivotPopupMenuCreatedEventArgs(ASPxPivotGridPopupMenu menu) {
			this.menu = menu;
		}
	}
	public class PivotDataAreaPopupCreatedEventArgs : EventArgs {
		readonly PivotGridDataAreaPopup popup;
		public PivotGridDataAreaPopup Popup { get { return popup; } }
		public PivotDataAreaPopupCreatedEventArgs(PivotGridDataAreaPopup popup) {
			this.popup = popup;
		}
	}
	public class CustomFieldDataEventArgs : PivotFieldEventArgs {
		object _value = null;
		int listSourceRow;
		PivotGridData data;
		public CustomFieldDataEventArgs(PivotGridData data, PivotGridField field, int listSourceRow, object _value)
			: base(field) {
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.data = data;
		}
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
	public class CustomServerModeSortEventArgs : CustomServerModeSortEventArgsBase<PivotGridField> {
		public CustomServerModeSortEventArgs(PivotGridField field) : base(field) { }
		internal new void SetArgs(PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			base.SetArgs(value0, value1, helper);
		}
	}
	public class PivotGridCustomFieldSortEventArgs : EventArgs {
		PivotGridData data;
		PivotGridField field;
		PivotSortOrder sortOrder;
		bool handled = false;
		internal object value1, value2;
		int result = 0;
		int listSourceRow1, listSourceRow2;
		public PivotGridCustomFieldSortEventArgs(PivotGridData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			SetArgs(-1, -1, null, null, PivotSortOrder.Ascending);
		}
		public PivotSortOrder SortOrder { get { return sortOrder; } }
		protected internal PivotGridData Data { get { return data; } }
		public PivotGridField Field { get { return field; } }
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
		internal int? GetSortResult() {
			if(!Handled) return null;
			return Result;
		}
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotSortOrder sortOrder) {
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
	public class PivotGridCustomSummaryEventArgs : PivotGridCustomSummaryEventArgsBase<PivotGridField> {
		public PivotGridCustomSummaryEventArgs(PivotGridData data, PivotGridField field, PivotCustomSummaryInfo customSummaryInfo) 
			: base(data, field, customSummaryInfo) {
		}
	}
	public class PivotGridCustomCallbackEventArgs : EventArgs {
		string parameters;
		public string Parameters { get { return parameters; } }
		public PivotGridCustomCallbackEventArgs(string parameters) {
			this.parameters = parameters;
		}
	}
	public class PivotHtmlCellPreparedEventArgs : PivotCellBaseEventArgs {
		PivotGridHtmlDataCell cell;
		public PivotHtmlCellPreparedEventArgs(PivotGridHtmlDataCell cell)
			: base(cell.CellItem) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
	}
	public class PivotHtmlFieldValuePreparedEventArgs : PivotFieldValueEventArgs {
		PivotGridHtmlTableCell cell;
		public PivotHtmlFieldValuePreparedEventArgs(PivotGridHtmlFieldValueCellBase cell)
			: base(cell.Item) {
			this.cell = cell;
		}
		public TableCell Cell { get { return cell; } }
	}
	public class PivotGridCallbackStateEventArgs : EventArgs {
		string callbackState;
		bool handled;
		public PivotGridCallbackStateEventArgs(string callbackState) {
			this.callbackState = callbackState;
		}
		public string CallbackState {
			get { return callbackState; }
			set { callbackState = value;}
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class PivotAreaChangingEventArgs : PivotFieldEventArgs {
		int newAreaIndex;
		PivotArea newArea;
		bool allow;
		public PivotAreaChangingEventArgs(PivotGridField field, PivotArea newArea, int newAreaIndex)
			: base(field) {
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
			this.allow = true;
		}
		public int NewAreaIndex { get { return newAreaIndex; } }
		public PivotArea NewArea { get { return newArea; } }
		public bool Allow { get { return allow; } set { allow = value; } }
	}
	public class PivotGroupEventArgs : EventArgs {
		PivotGridGroup group;
		public PivotGroupEventArgs(PivotGridGroup group) {
			this.group = group;
		}
		public PivotGridGroup Group { get { return group; } }
	}
	public class PivotQueryExceptionEventArgs : EventArgs {
		Exception ex;
		bool handled;
		internal PivotQueryExceptionEventArgs(Exception ex) {
			this.ex = ex;
			this.handled = false;
		}
		public Exception Exception {
			get { return ex; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class PivotOlapExceptionEventArgs : PivotQueryExceptionEventArgs {
		internal PivotOlapExceptionEventArgs(Exception ex) : base(ex) {
		}
	}
	public class GridLayoutEventHelper {
		readonly ASPxPivotGrid pivotGrid;
		string initialState;
		public GridLayoutEventHelper(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			this.initialState = CurrentState;
		}
		public bool IsRaiseNecessary {
			get { return this.initialState != CurrentState; }
		}
		protected string CurrentState {
			get {
				return pivotGrid.CallbackState[CallbackState.SerializedLayoutName] +
				   pivotGrid.CallbackState[CallbackState.CollapsedStateName];
			}
		}
	}
	public delegate void CustomFieldDataEventHandler(object sender, CustomFieldDataEventArgs e);
	public delegate void PivotGridCustomFieldSortEventHandler(object sender, PivotGridCustomFieldSortEventArgs e);
	public delegate void PivotGridCustomGroupIntervalEventHandler(object sender, PivotCustomGroupIntervalEventArgs e);
	public delegate void PivotGridCustomSummaryEventHandler(object sender, PivotGridCustomSummaryEventArgs e);
	public delegate void PivotFieldEventHandler(object sender, PivotFieldEventArgs e);
	public delegate void PivotFieldPropertyChangedEventHandler(object sender, PivotFieldPropertyChangedEventArgs e);
	public delegate void PivotFieldStateChangedEventHandler(object sender, PivotFieldStateChangedEventArgs e);
	public delegate void PivotFieldStateChangedCancelEventHandler(object sender, PivotFieldStateChangedCancelEventArgs e);
	public delegate void PivotFieldDisplayTextEventHandler(object sender, PivotFieldDisplayTextEventArgs e);
	public delegate void PivotFieldFilterChangingEventHandler(object sender, PivotFieldFilterChangingEventArgs e);  
	public delegate void PivotCustomChartDataSourceDataEventHandler(object sender, PivotCustomChartDataSourceDataEventArgs e);
	public delegate void PivotCustomChartDataSourceRowsEventHandler(object sender, PivotCustomChartDataSourceRowsEventArgs e);
	public delegate void PivotCellDisplayTextEventHandler(object sender, PivotCellDisplayTextEventArgs e);
	public delegate void PivotCustomCellStyleEventHandler(object sender, PivotCustomCellStyleEventArgs e);
	public delegate void PivotAddPopupMenuItemEventHandler(object sender, PivotAddPopupMenuItemEventArgs e);
	public delegate void PivotPopupMenuCreatedEventHandler(object sender, PivotPopupMenuCreatedEventArgs e);
	public delegate void PivotFieldValueEventHandler(object sender, PivotFieldValueEventArgs e);
	public delegate void PivotCustomCallbackEventHandler(object sender, PivotGridCustomCallbackEventArgs e);
	public delegate void PivotHtmlCellPreparedEventHandler(object sender, PivotHtmlCellPreparedEventArgs e);
	public delegate void PivotHtmlFieldValuePreparedEventHandler(object sender, PivotHtmlFieldValuePreparedEventArgs e);
	public delegate void PivotGridCallbackStateEventHandler(object sender, PivotGridCallbackStateEventArgs e);
	public delegate void PivotAreaChangingEventHandler(object sender, PivotAreaChangingEventArgs e);
	public delegate void PivotGroupEventHandler(object sender, PivotGroupEventArgs e);
	public delegate void PivotOlapExceptionEventHandler(object sender, PivotOlapExceptionEventArgs e);
	public delegate void PivotQueryExceptionEventHandler(object sender, PivotQueryExceptionEventArgs e);
}
