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
using System.Windows;
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Data.PivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows.Input;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.Filtering;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.Windows.Media;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Bars;
#if SL
using IInputElement = System.Windows.UIElement;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class PivotFieldEventArgs : RoutedEventArgs {
		PivotGridField field;
		internal PivotFieldEventArgs(RoutedEvent routedEvent, PivotGridField field)
			: base(routedEvent) {
			this.field = field;
		}
		public PivotGridField Field { get { return field; } }
	}
	public class PivotFieldPropertyChangedEventArgs : PivotFieldEventArgs {
		FieldPropertyName propertyName;
		public PivotFieldPropertyChangedEventArgs(RoutedEvent routedEvent, PivotGridField field, FieldPropertyName propertyName)
			: base(routedEvent, field) {
				this.propertyName = propertyName;
		}
		public FieldPropertyName PropertyName {
			get { return propertyName; }
		}
	}
	public class PivotFieldFilterChangingEventArgs : PivotFieldEventArgs {
		bool cancel;
		IList<object> values;
		FieldFilterType filterType;
		public PivotFieldFilterChangingEventArgs(RoutedEvent routedEvent, PivotGridField field, FieldFilterType filterType, IList<object> values)
			: base(routedEvent, field) {
			this.filterType = filterType;
			this.values = values;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public FieldFilterType FilterType { get { return filterType; } }
		public IList<object> Values { get { return values; } }
	}
	public class PivotGroupEventArgs : RoutedEventArgs {
		PivotGridGroup group;
		internal PivotGroupEventArgs(RoutedEvent routedEvent, PivotGridGroup group)
			: base(routedEvent) {
			this.group = group;
		}
		public PivotGridGroup Group { get { return group; } }
	}
	public class PivotCustomFilterPopupItemsEventArgs : RoutedEventArgs {
		PivotGridFilterItems items;
		public PivotCustomFilterPopupItemsEventArgs(RoutedEvent routedEvent, PivotGridFilterItems items)
			: base(routedEvent) {
			this.items = items;
		}
		public void CheckAllItems(bool isChecked) {
			items.CheckAllItems(isChecked);
		}
		public PivotGridField Field { get { return items.Field.GetWrapper(); } }
		public IList<PivotGridFilterItem> Items { get { return items; } }
		public PivotGridFilterItem ShowBlanksItem { get { return items.BlankItem; } }
	}
	public class FieldValueCell : FieldValueCellBase {
		protected internal static FieldValueCell CreateFieldValueCell(PivotFieldValueItem item) {
			return new FieldValueCell(item);
		}
		public FieldValueCell(PivotFieldValueItem item) : base(item) { }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("FieldValueCellField")]
#endif
		public new PivotGridField Field { get { return ((PivotFieldItem)Item.Field).GetWrapper(); } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("FieldValueCellDataField")]
#endif
		public new PivotGridField DataField { get { return ((PivotFieldItem)base.DataField).GetWrapper(); } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("FieldValueCellValueType")]
#endif
		public new FieldValueType ValueType { get { return Item.ValueType.ToFieldValueType(); } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("FieldValueCellParent")]
#endif
		public FieldValueCell Parent { get { return base.ParentItem == null ? null : CreateFieldValueCell(base.ParentItem); } }
	}
	class PivotCustomFieldValueCellsEventArgsCore : PivotCustomFieldValueCellsEventArgsBase {
		internal PivotCustomFieldValueCellsEventArgsCore(PivotVisualItemsBase items) : base(items) { }
		internal FieldValueCell GetCell(bool isColumn, int index) { 
			return GetCellCore(base.GetItem(isColumn, index)); 
		}
		FieldValueCell GetCellCore(PivotFieldValueItem item) {
			if(item == null) return null;
			return FieldValueCell.CreateFieldValueCell(item);
		}
		internal List<FieldValueCell> FindAllCells(bool isColumn, Predicate<object[]> match) {
			List<FieldValueCell> cells = new List<FieldValueCell>();
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				delegate(int cellIndex) {
					cells.Add(GetCell(isColumn, cellIndex));
					return false;
				}
			));
			return cells;
		}
		internal FieldValueCell FindCell(bool isColumn, Predicate<object[]> match) {
			int index = -1;
			base.DoCellIndexes(isColumn, match, new Predicate<int>(
				delegate(int cellIndex) {
					index = cellIndex;
					return true;
				}
			));
			return GetCell(isColumn, index);
		}
		internal void Split(bool isColumn, Predicate<FieldValueCell> match, bool firstCellOnly, IList<FieldValueSplitData> cells) {
			base.Split(isColumn, delegate(int index) { return match(this.GetCell(isColumn, index)); }, firstCellOnly, cells);
		}
	}
	public class PivotCustomFieldValueCellsEventArgs : RoutedEventArgs {
		PivotCustomFieldValueCellsEventArgsCore coreArgs;
		internal PivotCustomFieldValueCellsEventArgs(RoutedEvent routedEvent, PivotVisualItemsBase items)
			: base(routedEvent) {
			this.coreArgs = new PivotCustomFieldValueCellsEventArgsCore(items);
		}
		public int GetCellCount(bool isColumn) { return coreArgs.GetCellCount(isColumn); }
		public int GetLevelCount(bool isColumn) { return coreArgs.GetLevelCount(isColumn); }
		public GrandTotalLocation GetGrandTotalLocation(bool isColumn) { return coreArgs.GetGrandTotalLocation(isColumn); }
		public void SetGrandTotalLocation(bool isColumn, GrandTotalLocation location) { coreArgs.SetGrandTotalLocation(isColumn, location); }
		public bool IsUpdateRequired { get { return coreArgs.IsUpdateRequired; } }
		public FieldValueCell GetCell(bool isColumn, int index) { return coreArgs.GetCell(isColumn, index); }
		public object GetCellValue(int columnIndex, int rowIndex) {
			return coreArgs.GetCellValue(columnIndex, rowIndex);
		}
		public int ColumnCount { get { return coreArgs.ColumnCount; } }
		public int RowCount { get { return coreArgs.RowCount; } }
		public bool Remove(FieldValueCell item) {
			return coreArgs.Remove(item);
		}
		List<FieldValueCell> FindAllCells(bool isColumn, Predicate<object[]> match) {
			return coreArgs.FindAllCells(isColumn, match);
		}
		public FieldValueCell FindCell(bool isColumn, Predicate<object[]> match) {
			return coreArgs.FindCell(isColumn, match);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, bool firstCellOnly, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, firstCellOnly, (IList<FieldValueSplitData>)cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, params FieldValueSplitData[] cells) {
			this.Split(isColumn, match, false, cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, IList<FieldValueSplitData> cells) {
			this.Split(isColumn, match, false, cells);
		}
		public void Split(bool isColumn, Predicate<FieldValueCell> match, bool firstCellOnly, IList<FieldValueSplitData> cells) {
			coreArgs.Split(isColumn, match, firstCellOnly, cells);
		}
	}
	public class PivotFieldAreaChangingEventArgs : PivotFieldEventArgs {
		int newAreaIndex;
		FieldArea newArea;
		bool allow;
		internal PivotFieldAreaChangingEventArgs(RoutedEvent routedEvent, PivotGridField field, 
				FieldArea newArea, int newAreaIndex)
			: base(routedEvent, field) {
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
			this.allow = true;
		}
		public int NewAreaIndex { get { return newAreaIndex; } }
		public FieldArea NewArea { get { return newArea; } }
		public bool Allow { get { return allow; } set { allow = value; } }
	}
	public class PivotFieldValueEventArgs : PivotFieldEventArgs {
		PivotFieldValueItem item;
		internal PivotFieldValueEventArgs(RoutedEvent routedEvent, PivotFieldValueItem item)
			: base(routedEvent, ((PivotFieldItem)item.Field).GetWrapper()) {
			this.item = item;
		}
		internal PivotFieldValueEventArgs(RoutedEvent routedEvent, PivotGridField field)
			: base(routedEvent, field) { }
		protected PivotFieldValueItem Item { get { return item; } }
		protected PivotGridWpfData Data { get { return (PivotGridWpfData)Item.Data; } }
		public PivotGridField DataField { get { return Item != null ? ((PivotFieldItem)Item.DataField).GetWrapper() : null; } }
		public bool IsColumn { get { return Item != null ? Item.IsColumn : true; } }
		public int MinIndex { get { return Item != null ? Item.MinLastLevelIndex : -1; } }
		public virtual int MaxIndex { get { return Item != null ? Item.MaxLastLevelIndex : -1; } }
		public int FieldIndex { get { return Item != null ? Item.VisibleIndex : -1; } }
		public virtual object Value { get { return Item != null ? Item.Value : null; } }
		public bool IsOthersValue { get { return Item != null ? Item.IsOthersRow : false; } }
		public FieldValueType ValueType { get { return Item != null ? Item.ValueType.ToFieldValueType() : FieldValueType.Value; } }
		public PivotGridCustomTotal CustomTotal { get { return Item != null ? Item.CustomTotal.GetWrapper() : null; } }
		public bool IsCollapsed { get { return Data != null ? Data.IsObjectCollapsed(IsColumn, Item.VisibleIndex) : false; } }
		public bool Selected { get { return  Item != null && Data != null ? Data.VisualItems.IsFieldValueSelected(Item) : false; } }
		public void ChangeExpandedState() { Data.ChangeExpanded(Item); }
		public PivotGridField[] GetHigherLevelFields() {
			if(Field == null) return new PivotGridField[0];
			if(Field.Area == FieldArea.DataArea) {
				PivotFieldValueItem parent = Data.VisualItems.GetParentItem(IsColumn, Item);
				List<PivotGridField> fields = new List<PivotGridField>();
				while(parent != null) {
					if(parent.Field != null)
						fields.Insert(0, (PivotGridField)Data.GetField(parent.Field));
					parent = Data.VisualItems.GetParentItem(IsColumn, parent);
				}
				return fields.ToArray();
			} else {
				List<PivotGridField> fields = Data.GetFieldsByArea(Field.Area, true);
				int index = fields.IndexOf(Field);
				for(int i = 0; i < index; i++) {
					if(fields[i] == Data.DataField)
						fields[i] = DataField;
				}
				fields.RemoveRange(index, fields.Count - index);
				return fields.ToArray();
			}
		}
		FieldArea GetActualArea(PivotGridField field) {
			return (field.Area == FieldArea.DataArea) ? Data.OptionsDataField.DataFieldArea.ToFieldArea() : field.Area;
		}
		int GetActualAreaIndex(PivotGridField field) {
			return (field.Area == FieldArea.DataArea) ? Data.OptionsDataField.AreaIndex : field.AreaIndex;
		}
		bool AreFieldsInTheSameAreaAndHigherLevel(PivotGridField field1, PivotGridField field2) {
			return GetActualArea(field1) == GetActualArea(field2) && GetActualAreaIndex(field1) <= GetActualAreaIndex(field2);
		}
		bool IsFieldCorrect(PivotGridField field) {
			if(!AreFieldsInTheSameAreaAndHigherLevel(field, Field) || !field.Visible)
				return false;
			return true;
		}
		public object GetHigherLevelFieldValue(PivotGridField field) {
			if(!IsFieldCorrect(field))
				return null;
			return Data.GetFieldValue(field.GetInternalField(), FieldIndex, FieldIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.CreateDrillDownDataSourceWrapper(Item.CreateDrillDownDataSource());
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.CreateDrillDownDataSourceWrapper(Item.CreateQueryModeDrillDownDataSource(maxRowCount, customColumns));
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOlapDrillDownDataSource(int maxRowCount, List<string> customColumns) {
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
	public class PivotFieldValueExpandEventArgs : PivotFieldValueEventArgs {
		internal PivotFieldValueExpandEventArgs(RoutedEvent routedEvent, PivotFieldValueItem item)
			: base(routedEvent, item) {
		}
		PivotFieldValueItem newItem;
		public override int MaxIndex {
			get {
				if(Item == null)
					return -1;
				newItem = GetNewItem();
				if(newItem == null)
					return -1;
				return newItem.MaxLastLevelIndex; 
			} 
		}
		PivotFieldValueItem GetNewItem() {
			List<object> values = new List<object>();
			PivotFieldValueItem item = Item;
			while(item != null) {
				values.Add(item.Value);
				item = Field.Data.VisualItems.GetParentItem(item.IsColumn, item);
			}
			object[] vals = values.ToArray();
			Array.Reverse(vals);
			return Field.Data.VisualItems.GetItem(Item.IsColumn, vals);
		}
	}
	public class PivotFieldValueCollapseEventArgs : PivotFieldValueEventArgs {
		internal PivotFieldValueCollapseEventArgs(RoutedEvent routedEvent, PivotFieldValueItem item)
			: base(routedEvent, item) {
		}
		public override int MaxIndex { get { return MinIndex; } }
	}
	public class PivotFieldValueCancelEventArgs : PivotFieldValueEventArgs {
		bool cancel;
		internal PivotFieldValueCancelEventArgs(RoutedEvent routedEvent, PivotFieldValueItem item)
			: base(routedEvent, item) {
			this.cancel = false;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldValueEventArgs {
		string displayText;
		object value;
		internal PivotFieldDisplayTextEventArgs(RoutedEvent routedEvent, PivotFieldValueItem item, string defaultText)
			: base(routedEvent, item) {
			this.value = Item.Value;
			this.displayText = defaultText;
		}
		internal PivotFieldDisplayTextEventArgs(RoutedEvent routedEvent, PivotGridField field, DevExpress.XtraPivotGrid.IOLAPMember member)
			: base(routedEvent, field) {
			this.value = member.Value;
			this.displayText = field.GetValueText(member);
		}
		internal PivotFieldDisplayTextEventArgs(RoutedEvent routedEvent, PivotGridField field, object value)
			: base(routedEvent, field) {
			this.value = value;
			this.displayText = field.GetValueText(value);
		}
		public override object Value { get { return value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public bool IsPopulatingFilterDropdown { get { return Item == null; } }
		public IThreadSafeField ThreadSafeDataField { get { return DataField; } }
		public IThreadSafeField ThreadSafeField { get { return Field; } }
	}
	public class PivotCustomSummaryEventArgs : EventArgs {
		PivotGridField field;
		PivotCustomSummaryInfo customSummaryInfo;
		PivotGridWpfData data;
		PivotDrillDownDataSource dataSource;
		internal PivotCustomSummaryEventArgs(PivotGridWpfData data,
				PivotGridField field, PivotCustomSummaryInfo customSummaryInfo)
			: base() {
			this.data = data;
			this.field = field;
			this.customSummaryInfo = customSummaryInfo;
			this.dataSource = null;
		}
		protected PivotCustomSummaryInfo CustomSummaryInfo { get { return customSummaryInfo; } }
		public object CustomValue { get { return SummaryValue.CustomValue; } set { SummaryValue.CustomValue = value; } }
		public PivotSummaryValue SummaryValue { get { return CustomSummaryInfo.SummaryValue; } }
		public PivotGridField DataField { get { return field; } }
		public IThreadSafeField ThreadSafeDataField { get { return DataField; } }
		public string FieldName { get { return CustomSummaryInfo.DataColumn.Name; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return DataSource;
		}
		public PivotGridField ColumnField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.ColColumn).GetWrapper(); }
		}
		public IThreadSafeField ThreadSafeColumnField { get { return ColumnField; } }
		public PivotGridField RowField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.RowColumn).GetWrapper(); }
		}
		public IThreadSafeField ThreadSafeRowField { get { return RowField; } }
		public object ColumnFieldValue { get { return GetFieldValue(CustomSummaryInfo.ColColumn); } }
		public object RowFieldValue { get { return GetFieldValue(CustomSummaryInfo.RowColumn); } }
		protected PivotDrillDownDataSource DataSource {
			get {
				if(this.dataSource == null)
					this.dataSource = data.GetDrillDownDataSource(CustomSummaryInfo.GroupRow, CustomSummaryInfo.VisibleListSourceRows);
				return this.dataSource;
			}
		}
		protected object GetFieldValue(PivotColumnInfo columnInfo) {
			if(columnInfo == null) return null;
			return DataSource[0][columnInfo.ColumnInfo.Name];
		}
	}
	public class PivotCustomGroupIntervalEventArgs : EventArgs {
		PivotGridField field;
		object value, groupValue;
		internal PivotCustomGroupIntervalEventArgs(PivotGridField field, object value)
			: base() {
			this.field = field;
			this.groupValue = this.value = value;
		}
		public PivotGridField Field { get { return field; } }
		public IThreadSafeField ThreadSafeField { get { return Field; } }
		public object Value { get { return value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
	}
	public class CustomServerModeSortEventArgs : EventArgs {
		PivotGridField field;
		DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider val0;
		DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider val1;
		int? result;
		DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper;
		public object Value1 { get { return val0.Member.Value; } }
		public object Value2 { get { return val1.Member.Value; } }
		public PivotGridField Field { get { return field; } }
		public DevExpress.XtraPivotGrid.IOLAPMember OlapMember1 { get { return val0.Member as DevExpress.XtraPivotGrid.IOLAPMember; } }
		public DevExpress.XtraPivotGrid.IOLAPMember OlapMember2 { get { return val1.Member as DevExpress.XtraPivotGrid.IOLAPMember; } }
		public int? Result {
			get { return result; }
			set { result = value; }
		}
		public CustomServerModeSortEventArgs()
			: base() {
		}
		public void SetArgs(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider val0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider val1, PivotGridField field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			this.val0 = val0;
			this.val1 = val1;
			this.helper = helper;
			this.field = field;
			this.result = null;
		}
		public CrossAreaKey GetCrossAreaKey(object[] crossAreaValues) {
			return new CrossAreaKey(helper.GetSortByObject(crossAreaValues, !Field.InternalField.IsColumn));
		}
		public object GetCellValue1(CrossAreaKey crossAreaKey, PivotGridField dataField) {
			return GetCellValue(crossAreaKey.Data, val0, Field.InternalField.IsColumn, dataField);
		}
		public object GetCellValue2(CrossAreaKey crossAreaKey, PivotGridField dataField) {
			return GetCellValue(crossAreaKey.Data, val1, Field.InternalField.IsColumn, dataField);
		}
		public object GetCellValue1(object[] crossAreaValues, PivotGridField dataField) {
			object token = helper.GetSortByObject(crossAreaValues, !Field.InternalField.IsColumn);
			if(token == null)
				return null;
			return GetCellValue(token, val0, Field.InternalField.IsColumn, dataField);
		}
		public object GetCellValue2(object[] crossAreaValues, PivotGridField dataField) {
			object token = helper.GetSortByObject(crossAreaValues, !Field.InternalField.IsColumn);
			if(token == null)
				return null;
			return GetCellValue(token, val1, Field.InternalField.IsColumn, dataField);
		}
		object GetCellValue(object val1, object val2, bool isColumn, PivotGridField dataField) {
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
	public class PivotCustomFieldSortEventArgs : EventArgs {
		PivotGridWpfData data;
		PivotGridField field;
		FieldSortOrder sortOrder;
		bool handled = false;
		internal object value1, value2;
		int result = 0;
		int listSourceRow1, listSourceRow2;
		internal PivotCustomFieldSortEventArgs(PivotGridWpfData data, PivotGridField field) {
			this.data = data;
			this.field = field;
			SetArgs(-1, -1, null, null, FieldSortOrder.Ascending);
		}
		internal PivotGridWpfData Data { get { return data; } }
		public FieldSortOrder SortOrder { get { return sortOrder; } }
		public PivotGridField Field { get { return field; } }
		public IThreadSafeField ThreadSafeField { get { return Field; } }
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
		public int ListSourceRowIndex1 { get { return listSourceRow1; } }
		public int ListSourceRowIndex2 { get { return listSourceRow2; } }
		internal void SetArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, FieldSortOrder sortOrder) {
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
		internal int? GetSortResult() {
			if(!Handled) return null;
			return Result;
		}
	}
	public class PivotCustomFieldDataEventArgs : EventArgs {
		PivotGridField field;
		object _value = null;
		int listSourceRow;
		PivotGridData data;
		internal PivotCustomFieldDataEventArgs(PivotGridWpfData data, PivotGridField field, int listSourceRow, object _value) {
			this.field = field;
			this.listSourceRow = listSourceRow;
			this._value = _value;
			this.data = data;
		}
		public PivotGridField Field { get { return field; } }
		public IThreadSafeField ThreadSafeField { get { return Field; } }
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
	public class PivotCellBaseEventArgs : RoutedEventArgs {
		PivotGridCellItem cellItem;
		internal PivotCellBaseEventArgs(RoutedEvent routedEvent, PivotGridCellItem cellItem)
			: base(routedEvent) {
			this.cellItem = cellItem;
		}
		protected PivotGridCellItem Item { get { return cellItem; } }
		protected PivotGridWpfData Data { get { return (PivotGridWpfData)Item.Data; } }
		public PivotGridField DataField { get { return ((PivotFieldItem)Item.DataField).GetWrapper(); } }
		public int ColumnIndex { get { return Item.ColumnIndex; } }
		public int RowIndex { get { return Item.RowIndex; } }
		public int ColumnFieldIndex { get { return Item.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return Item.RowFieldIndex; } }
		public PivotGridField ColumnField { get { return ((PivotFieldItem)Item.ColumnField).GetWrapper(); } }
		public PivotGridField RowField { get { return ((PivotFieldItem)Item.RowField).GetWrapper(); } }
		public object Value { get { return Item.Value; } }
		public string DisplayText { get { return Item.Text; } }
		public bool Focused { get { return Data.VisualItems.IsCellFocused(Item); } }
		public bool Selected { get { return Data.VisualItems.IsCellSelected(Item); } }		
		public PivotSummaryType SummaryType { get { return Item.SummaryType; } }
		public PivotSummaryValue SummaryValue { get { return Data.GetCellSummaryValue(ColumnFieldIndex, RowFieldIndex, DataField.GetInternalField()); } }
		public FieldValueType ColumnValueType { get { return Item.ColumnValueType.ToFieldValueType(); } }
		public FieldValueType RowValueType { get { return Item.RowValueType.ToFieldValueType(); } }
		public PivotGridCustomTotal ColumnCustomTotal { get { return Item.ColumnCustomTotal.GetWrapper(); } }
		public PivotGridCustomTotal RowCustomTotal { get { return Item.RowCustomTotal.GetWrapper(); } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetQueryModeDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex, maxRowCount, customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(-1, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOlapDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOlapDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource(ColumnFieldIndex, RowFieldIndex);
		}
		public object GetFieldValue(PivotGridField field) {
			return Item.GetFieldValue(field.FieldItem);
		}
		public object GetFieldValue(PivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
		public bool IsOthersFieldValue(PivotGridField field) {
			return Item.IsOthersFieldValue(field.FieldItem);
		}
		public bool IsFieldValueExpanded(PivotGridField field) {
			return Item.IsFieldValueExpanded(field.FieldItem);
		}
		public bool IsFieldValueRetrievable(PivotGridField field) {
			return Item.IsFieldValueRetrievable(field.FieldItem);
		}
		public PivotGridField[] GetColumnFields() {
			return GetFields(CoreXtraPivotGrid.PivotArea.ColumnArea, Data.GetColumnLevel(ColumnFieldIndex) + 1);
		}
		public PivotGridField[] GetRowFields() {
			return GetFields(CoreXtraPivotGrid.PivotArea.RowArea, Data.GetRowLevel(RowFieldIndex) + 1);
		}
		PivotGridField[] GetFields(CoreXtraPivotGrid.PivotArea area, int fieldCount) {
			if(fieldCount <= 0 || fieldCount > Data.GetFieldCountByArea(area)) return new PivotGridField[0];
			PivotGridField[] fields = new PivotGridField[fieldCount];
			for(int i = 0; i < fields.Length; i++)
				fields[i] = Data.GetFieldByArea(area, i) as PivotGridField;
			return fields;
		}
		public object GetCellValue(PivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetPrevRowCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField.GetInternalField(), false);
		}
		public object GetNextRowCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField.GetInternalField(), true);
		}
		public object GetPrevColumnCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField.GetInternalField(), false);
		}
		public object GetNextColumnCellValue(PivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField.GetInternalField(), true);
		}
		public object GetColumnGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(-1, RowFieldIndex, dataField);
		}
		public object GetColumnGrandTotal(object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(null, rowValues, dataField);
		}
		public object GetRowGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, -1, dataField);
		}
		public object GetRowGrandTotal(object[] columnValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, null, dataField);
		}
		public object GetGrandTotal(PivotGridField dataField) {
			return Data.GetCellValue(-1, -1, dataField);
		}
	}
	public class PivotCustomCellAppearanceEventArgs : PivotCellBaseEventArgs {
		Brush background;
		Brush foreground;
		internal PivotCustomCellAppearanceEventArgs(RoutedEvent routedEvent, PivotGridCellItem cellItem, bool isExporting)
			: base(routedEvent, cellItem) {
				IsExporting = isExporting;
		}
		public Brush Foreground { get { return foreground; } set { foreground = value; } }
		public Brush Background { get { return background; } set { background = value; } }
		public virtual bool IsExporting { get; protected set; }
	}
	public class PivotCustomValueAppearanceEventArgs : PivotFieldValueEventArgs {
		Brush background;
		Brush foreground;
		internal PivotCustomValueAppearanceEventArgs(RoutedEvent routedEvent, PivotFieldValueItem valueItem, bool isExporting)
			: base(routedEvent, valueItem) {
				IsExporting = isExporting;
		}
		public Brush Foreground { get { return foreground; } set { foreground = value; } }
		public Brush Background { get { return background; } set { background = value; } }
		public virtual bool IsExporting { get; protected set; }
	}
	public class PivotCellEventArgs : PivotCellBaseEventArgs {
		UIElement cellElement;
		MouseButton button;
		internal PivotCellEventArgs(RoutedEvent routedEvent, UIElement cellElement, PivotGridCellItem cellItem,
				MouseButton button)
			: base(routedEvent, cellItem) {
				this.cellElement = cellElement;
				this.button = button;
		}
		public UIElement Element { get { return cellElement; } }
		public MouseButton Button { get { return button; } }
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		internal PivotCellDisplayTextEventArgs(RoutedEvent routedEvent, PivotGridCellItem cellItem)
			: base(routedEvent, cellItem) {
			this.displayText = Item.Text;
		}
		public new string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	public class PivotCellValueEventArgs : PivotCellBaseEventArgs {
		object value;
		internal PivotCellValueEventArgs(RoutedEvent routedEvent, PivotGridCellItem cellItem)
			: base(routedEvent, cellItem) {
			this.value = Item.Value;
		}
		public new object Value { get { return this.value; } set { this.value = value; } }
	}
	public class PivotLayoutUpgradeEventArgs : RoutedEventArgs {
		DevExpress.Utils.LayoutUpgradeEventArgs e;
		internal PivotLayoutUpgradeEventArgs(RoutedEvent routedEvent, DevExpress.Utils.LayoutUpgradeEventArgs e)
			: base(routedEvent, e) {
			this.e = e;
		}
		public string PreviousVersion { get { return e.PreviousVersion; } }
	}
	public class PivotUnboundExpressionEditorEventArgs : RoutedEventArgs {
		ExpressionEditorControl control;
		PivotGridField field;
		internal PivotUnboundExpressionEditorEventArgs(RoutedEvent routedEvent, ExpressionEditorControl control, PivotGridField field) 
			: base(routedEvent) {
			this.control = control;
			this.field = field;
		}
		public ExpressionEditorControl ExpressionEditorControl { get { return control; } }
		public PivotGridField Field { get { return field; } }
	}
	public class PivotFilterEditorEventArgs : RoutedEventArgs {
		internal PivotFilterEditorEventArgs(FilterControl control) {
			this.FilterControl = control;
		}
		public FilterControl FilterControl { get; private set; }
	}
	public class PivotCustomChartDataSourceDataEventArgs : EventArgs {
		PivotChartItemType itemType;
		PivotChartItemDataMember itemDataMember;
		PivotFieldValueEventArgs fieldValueInfo;
		PivotCellValueEventArgs cellInfo;
		object _value;
		internal PivotCustomChartDataSourceDataEventArgs(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			this.itemType = itemType;
			this.itemDataMember = itemDataMember;
			this.fieldValueInfo = null;
			this.cellInfo = null;
			switch(ItemType) {
				case PivotChartItemType.ColumnItem:
				case PivotChartItemType.RowItem:
					this.fieldValueInfo = new PivotFieldValueEventArgs(null, fieldValueItem);
					this.cellInfo = null;
					break;
				case PivotChartItemType.CellItem:
					this.fieldValueInfo = null;
					this.cellInfo = new PivotCellValueEventArgs(null, cellItem);
					break;
			}
			this._value = value;
		}
		public PivotChartItemType ItemType { get { return itemType; } }
		public PivotChartItemDataMember ItemDataMember { get { return itemDataMember; } }
		public PivotFieldValueEventArgs FieldValueInfo { get { return fieldValueInfo; } }
		public PivotCellValueEventArgs CellInfo { get { return cellInfo; } }
		public object Value {
			get { return _value; }
			set { _value = value; }
		}
	}
	public class PivotCustomChartDataSourceRowsEventArgs : EventArgs {
		readonly PivotWpfChartDataSource ds;
		readonly IList<PivotChartDataSourceRow> rows;
		internal PivotCustomChartDataSourceRowsEventArgs(PivotWpfChartDataSource ds, IList<CoreXtraPivotGrid.PivotChartDataSourceRowBase> rows) {
			this.ds = ds;
			this.rows = new CoreXtraPivotGrid.PivotChartDataSourceRowBaseListWrapper<PivotChartDataSourceRow>(rows);
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
	public class CustomPrefilterDisplayTextEventArgs : RoutedEventArgs {
		public CustomPrefilterDisplayTextEventArgs(object value) {
			Value = value;
		}
		public object Value { get; set; }
	}
	public class PivotPropertyChangedEventArgs : RoutedEventArgs {
		public PivotPropertyChangedEventArgs(RoutedEvent evt, DependencyPropertyChangedEventArgs args, object source)
			: base(evt, source) {
			this.Args = args;
		}
		public object OldValue { get { return Args.OldValue; } }
		public object NewValue { get { return Args.NewValue; } }
		public DependencyProperty Property { get { return Args.Property; } }
		protected DependencyPropertyChangedEventArgs Args { get; set; }
	}
	public class PopupMenuShowingEventArgs : RoutedEventArgs {
		PivotGridPopupMenu menu;
		ReadOnlyCollection<BarItem> items;
		public PopupMenuShowingEventArgs(PivotGridPopupMenu menu) {
			this.menu = menu;
			this.items = menu.GetItems();
		}
		protected PivotGridPopupMenu GridMenu { get { return menu; } }
		public PivotGridMenuType MenuType { get { return GridMenu.MenuType; } }
		public ReadOnlyCollection<BarItem> Items { get { return items; } }
		public BarManagerActionCollection Customizations { get { return GridMenu.Customizations; } }
		public IInputElement TargetElement { get { return GridMenu.PlacementTarget; } }
		PivotGridMenuInfo MenuInfo { get { return GridMenu.MenuInfo; } }
		public new PivotGridControl Source { get { return GridMenu.PivotGrid; } }
		public virtual PivotCellBaseEventArgs GetCellInfo() {
			return MenuInfo.GetCellInfo();
		}
		public virtual PivotFieldValueEventArgs GetFieldValueInfo() {
			return MenuInfo.GetFieldValueInfo();
		}
		public virtual PivotFieldEventArgs GetFieldInfo() {
			return MenuInfo.GetFieldInfo();
		}
	}
	public enum PivotBrushType {
		CellBrush,
		ValueBrush
	}
	public class PivotBrushChangedEventArgs : RoutedEventArgs {
		public PivotBrushChangedEventArgs(RoutedEvent evt, object source, PivotBrushType changedType)
			: base(evt, source) {
				BrushType = changedType;	
		}
		public PivotBrushType BrushType { get; protected set; }
	}
	public class PivotLayoutAllowEventArgs : PivotLayoutUpgradeEventArgs {
		public PivotLayoutAllowEventArgs(RoutedEvent evt, DevExpress.Utils.LayoutAllowEventArgs e)
			: base(evt, e) {
			Allow = true;
		}
		public bool Allow { get; set; }
	}
	 public class PivotQueryExceptionEventArgs : RoutedEventArgs {
		Exception ex;
		internal PivotQueryExceptionEventArgs(RoutedEvent evt, object source, Exception ex)
			: base(evt, source) {
			this.ex = ex;
		}
		public Exception Exception {
			get { return ex; }
		}
	}
	public class PivotOlapExceptionEventArgs : PivotQueryExceptionEventArgs {
		internal PivotOlapExceptionEventArgs(RoutedEvent evt, object source, Exception ex)
			: base(evt, source, ex) {
		}
	}
	public delegate void PivotGroupEventHandler(object sender, PivotGroupEventArgs e);
	public delegate void PivotFieldEventHandler(object sender, PivotFieldEventArgs e);
	public delegate void PivotFieldPropertyChangedEventHandler(object sender, PivotFieldPropertyChangedEventArgs e);
	public delegate void PivotFieldFilterChangingEventHandler(object sender, PivotFieldFilterChangingEventArgs e);	
	public delegate void PivotFieldValueEventHandler(object sender, PivotFieldValueEventArgs e);
	public delegate void PivotFieldValueCancelEventHandler(object sender, PivotFieldValueCancelEventArgs e);
	public delegate void PivotFieldDisplayTextEventHandler(object sender, PivotFieldDisplayTextEventArgs e);
	public delegate void PivotCustomSummaryEventHandler(object sender, PivotCustomSummaryEventArgs e);
	public delegate void PivotCustomGroupIntervalEventHandler(object sender, PivotCustomGroupIntervalEventArgs e);
	public delegate void PivotCustomFieldSortEventHandler(object sender, PivotCustomFieldSortEventArgs e);
	public delegate void PivotCustomFieldDataEventHandler(object sender, PivotCustomFieldDataEventArgs e);
	public delegate void PivotCellEventHandler(object sender, PivotCellEventArgs e);
	public delegate void PivotCellDisplayTextEventHandler(object sender, PivotCellDisplayTextEventArgs e);
	public delegate void PivotCellValueEventHandler(object sender, PivotCellValueEventArgs e);
	public delegate void PivotCustomCellAppearanceEventHandler(object sender, PivotCustomCellAppearanceEventArgs e);
	public delegate void PivotCustomValueAppearanceEventHandler(object sender, PivotCustomValueAppearanceEventArgs e);
	public delegate void PivotFieldAreaChangingEventHandler(object sender, PivotFieldAreaChangingEventArgs e);
	public delegate void PivotLayoutUpgradeEventHandler(object sender, PivotLayoutUpgradeEventArgs e);
	public delegate void PivotUnboundExpressionEditorEventHandler(object sender, PivotUnboundExpressionEditorEventArgs e);
	public delegate void PivotFilterEditorEventHandler(object sender, PivotFilterEditorEventArgs e);
	public delegate void PivotCustomChartDataSourceDataEventHandler(object sender, PivotCustomChartDataSourceDataEventArgs e);
	public delegate void PivotCustomChartDataSourceRowsEventHandler(object sender, PivotCustomChartDataSourceRowsEventArgs e);
	public delegate void PivotCustomFilterPopupItemsEventHandler(object sender, PivotCustomFilterPopupItemsEventArgs e);
	public delegate void PivotCustomFieldValueCellsEventHandler(object sender, PivotCustomFieldValueCellsEventArgs e);
	public delegate void CustomPrefilterDisplayTextEventHandler(object sender, CustomPrefilterDisplayTextEventArgs e);
	public delegate void PivotPropertyChangedEventHandler(object sender, PivotPropertyChangedEventArgs e);
	public delegate void PivotBrushChangedEventHandler(object sender, PivotBrushChangedEventArgs e);
	public delegate void PivotLayoutAllowEventHandler(object sender, PivotLayoutAllowEventArgs e);
	public delegate void PivotOlapExceptionEventHandler(object sender, PivotOlapExceptionEventArgs e);
	public delegate void PivotQueryExceptionEventHandler(object sender, PivotQueryExceptionEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
}
