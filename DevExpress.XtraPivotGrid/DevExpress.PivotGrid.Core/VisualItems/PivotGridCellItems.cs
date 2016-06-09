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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraPivotGrid.Data { 
	public abstract class PivotGridCellDataProviderBase {
#if DEBUGTEST
		protected const int CheckMark = 0x12345678;
#endif        
		PointTextCache textCache;
		PointValueCache valueCache;
		PivotGridData data;
		public PivotGridCellDataProviderBase(PivotGridData data) {
			this.data = data;
			this.textCache = new PointTextCache(1000);
			this.valueCache = new PointValueCache(1000);
		}
		public PointTextCache TextCache {
			get { return textCache; }
			protected set { textCache = value; }
		}
		public PointValueCache ValueCache {
			get { return valueCache; }
			protected set { valueCache = value; }
		}
		public PivotGridData Data { get { return data; } }
		public abstract PivotCellValue GetCellValueEx(PivotGridCellItem cellItem);
		public abstract PivotCellValue GetCellValueEx(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem);
		public abstract object GetCustomCellValue(PivotGridCellItem cellItem);
		public abstract string GetCustomCellText(PivotGridCellItem cellItem);
		public virtual string GetDisplayText(PivotGridCellItem cellItem, PivotCellValue cellValue) {
			string text = GetDisplayTextCore(cellItem, cellValue);
			TextCache.AddPointValue(new Point(cellItem.ColumnIndex, cellItem.RowIndex), text);
			return text;
		}
		protected string GetDisplayTextCore(PivotGridCellItem cellItem, PivotCellValue cellValue) {
			object value = PivotCellValue.GetValue(cellValue);
			string serverText = PivotCellValue.GetDisplayText(cellValue);
			if(value == null) {
				return cellItem.DataField != null ? cellItem.DataField.EmptyCellText : string.Empty;
			}
			if(value == PivotSummaryValue.ErrorValue)
				return PivotGridLocalizer.GetString(PivotGridStringId.CellError);
			FormatInfo formatInfo = cellItem.GetCellFormatInfo();
			if(serverText != null && (formatInfo == null || PivotGridFieldBase.IsDefaultFormat(formatInfo))) {
				return cellValue.DisplayText;
			}
			return formatInfo != null ? formatInfo.GetDisplayText(value) : value.ToString();
		}
		public static int GetDataIndex(PivotArea dataFieldArea, PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			return dataFieldArea == PivotArea.ColumnArea ? columnItem.DataIndex : rowItem.DataIndex;
		}
		public static PivotGridCustomTotalBase GetCustomTotal(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			if(rowItem.CustomTotal != null)
				return rowItem.CustomTotal;
			if(columnItem.CustomTotal != null)
				return columnItem.CustomTotal;
			return null;
		}
		public static PivotSummaryType GetSummaryType(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, PivotGridFieldBase dataField) {
			PivotSummaryType summaryType = dataField != null ? dataField.SummaryType : PivotSummaryType.Sum;
			PivotGridCustomTotalBase customTotal = GetCustomTotal(columnItem, rowItem);
			if(customTotal != null)
				summaryType = customTotal.SummaryType;
			return summaryType;
		}
		PivotGridFieldBase GetField(PivotFieldItemBase dataField) {
			return Data.GetField(dataField);
		}
		public object GetCellValue(int columnIndex, int rowIndex, PivotFieldItemBase dataField) {
			return Data.GetCellValue(columnIndex, rowIndex, GetField(dataField));
		}
		public object GetFieldValue(PivotFieldItemBase field, int columnIndex, int rowIndex) {
			return Data.GetFieldValue(GetField(field), columnIndex, rowIndex);
		}
		public bool GetIsOthersValue(PivotFieldItemBase field, int columnIndex, int rowIndex) {
			return Data.GetIsOthersValue(GetField(field), columnIndex, rowIndex);
		}
		public bool IsFieldValueExpanded(PivotFieldItemBase field, int columnIndex, int rowIndex) {
			return Data.IsFieldValueExpanded(GetField(field), columnIndex, rowIndex);
		}
		public void SaveToStream(TypedBinaryWriter writer, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItemsCreator rowItemCreator) {
			for(int i = 0; i < rowItemCreator.LastLevelItemCount; i ++) {
				SaveRowToStream(writer, columnItemCreator, rowItemCreator.GetLastLevelItem(i), i);
			}
		}
		void SaveRowToStream(TypedBinaryWriter writer, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItem rowItem, int rowIndex) {
			for(int i = 0; i < columnItemCreator.LastLevelItemCount; i ++)
				SaveCellToStream(writer, columnItemCreator.GetLastLevelItem(i), rowItem, i, rowIndex);
		}
		void SaveCellToStream(TypedBinaryWriter writer, PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, int colIndex, int rowIndex) {
			PivotGridCellItem cellItem = Data.VisualItems.CreateCellItem(columnItem, rowItem, colIndex, rowIndex);
			writer.WriteTypedObject(cellItem.Value);
			bool saveText = cellItem.IsCustomDisplayText || cellItem.IsServerDisplayText;
			writer.Write(saveText);
			if(saveText) 
				writer.WriteNullableString(cellItem.Text);
#if DEBUGTEST
			writer.Write(CheckMark);
#endif
		}
	}
	public class PivotGridCellDataProvider : PivotGridCellDataProviderBase {
		public PivotGridCellDataProvider(PivotGridData data) : base(data) {
		}
		public override PivotCellValue GetCellValueEx(PivotGridCellItem cellItem) {
			PivotCellValue cellValue = Data.GetCellValueEx(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex, cellItem.SummaryType);
			if(cellValue != null)
				ValueCache.AddPointValue(new Point(cellItem.ColumnIndex, cellItem.RowIndex), cellValue.Value);
			return cellValue;
		}
		public override object GetCustomCellValue(PivotGridCellItem cellItem) {
			return Data.GetCustomCellValue(cellItem);
		}
		public override string GetCustomCellText(PivotGridCellItem cellItem) {
			return Data.GetCustomCellText(cellItem);
		}
		public override PivotCellValue GetCellValueEx(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			int dataIndex = GetDataIndex(Data.OptionsDataField.DataFieldArea, columnItem, rowItem);
			PivotGridFieldBase dataField = dataIndex > -1 ? Data.GetFieldByArea(PivotArea.DataArea, dataIndex) : null;
			PivotSummaryType summaryType = GetSummaryType(columnItem, rowItem, dataField);
			PivotCellValue cellValue = Data.GetCellValueEx(columnItem.VisibleIndex, rowItem.VisibleIndex, dataIndex, summaryType);
			if(cellValue != null)
				ValueCache.AddPointValue(new Point(columnItem.VisibleIndex, rowItem.VisibleIndex), cellValue.Value);
			return cellValue;
		}
	}
	public class PivotGridCellStreamDataProvider : PivotGridCellDataProviderBase {
		int columnCount, rowCount;
		object[,] values;
		string[,] texts;
		bool[,] customTexts;
		public PivotGridCellStreamDataProvider(PivotGridData data) : base(data) {
			this.columnCount = 0;
			this.rowCount = 0;
		}
		public int ColumnCount { get { return columnCount; } }
		public int RowCount { get { return rowCount; } }
		public override PivotCellValue GetCellValueEx(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			return new PivotCellValue(GetCellValueCore(columnItem.MinLastLevelIndex, rowItem.MinLastLevelIndex));
		}
		public override PivotCellValue GetCellValueEx(PivotGridCellItem cellItem) {
			return new PivotCellValue(GetCellValueCore(cellItem.ColumnIndex, cellItem.RowIndex));
		}
		public override object GetCustomCellValue(PivotGridCellItem cellItem) {
			return cellItem.Value;
		}
		public override string GetCustomCellText(PivotGridCellItem cellItem) {
			return GetCustomCellDisplayTextCore(cellItem.Text, cellItem.ColumnIndex, cellItem.RowIndex);
		}
		protected object GetCellValueCore(int columnIndex, int rowIndex) {
			if(this.values == null) return null;
			if(columnIndex >= this.columnCount) return null;
			if(rowIndex >= this.rowCount) return null;
			return values[rowIndex, columnIndex];
		}
		protected string GetCustomCellDisplayTextCore(string defaultText, int columnIndex, int rowIndex) {
			if(this.texts != null && columnIndex < this.columnCount && rowIndex < this.rowCount
					&& this.customTexts[rowIndex, columnIndex]) {
				return this.texts[rowIndex, columnIndex];
			}
			return defaultText;
		}
		public void LoadFromStream(Stream stream, PivotFieldValueItemsCreator columnItemCreator, PivotFieldValueItemsCreator rowItemCreator) {
			this.columnCount = columnItemCreator.LastLevelItemCount;
			this.rowCount = rowItemCreator.LastLevelItemCount;
			this.values = new object[RowCount, ColumnCount];
			this.texts = new string[RowCount, ColumnCount];
			this.customTexts = new bool[RowCount, ColumnCount];
			TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, Data.OptionsData.CustomObjectConverter);
			for(int i = 0; i < RowCount; i ++) {
				for(int j = 0; j < ColumnCount; j ++) {
					this.values[i, j] = reader.ReadTypedObject();
					this.customTexts[i, j] = reader.ReadBoolean();
					if(this.customTexts[i, j]) 
						this.texts[i, j] = reader.ReadNullableString();
#if DEBUGTEST
					int mark = reader.ReadInt32();
					if(mark != CheckMark)
						throw new Exception("file corrupted");
#endif
				}
			}
		}
	}
	public enum PivotGridCellType { Cell, Total, GrandTotal, CustomTotal };
	public class PivotGridCellItem : IEvaluatorDataAccess {
		internal delegate object GetValueDelegate();
		internal static FormatInfo GetFormatInfo(GetValueDelegate getValue, PivotFieldItemBase dataField, 
				PivotGridCustomTotalBase customTotal, bool isTotal, bool isGrandTotal) {
			if(dataField == null) return null;
			FormatInfo cellFormat = dataField.CellFormat.IsEmpty ? null : dataField.CellFormat;
			FormatInfo totalCellFormat = dataField.TotalCellFormat.IsEmpty ? cellFormat : dataField.TotalCellFormat;
			if(customTotal != null)
				cellFormat = customTotal.GetCellFormat().IsEmpty ? totalCellFormat : customTotal.GetCellFormat();
			else {
				if(isGrandTotal)
					cellFormat = dataField.GrandTotalCellFormat.IsEmpty ? totalCellFormat : dataField.GrandTotalCellFormat;
				if(isTotal)
					cellFormat = totalCellFormat ?? cellFormat;
			}
			if(cellFormat == null || cellFormat.IsEmpty) {
				if(dataField.IsPercentageCalculation)
					cellFormat = PivotGridFieldBase.DefaultPercentFormat;
				else {
					if(dataField.IsNonCurrencyDecimalCalculation)
						cellFormat = PivotGridFieldBase.DefaultDecimalNonCurrencyFormat;
					else if(dataField.SummaryType != PivotSummaryType.Count && !dataField.IsIntegerCalculation) {
						Type fieldType = dataField.FieldType;
						if((fieldType == null || fieldType == typeof(object)) && getValue() != null)
							fieldType = getValue().GetType();
						if(fieldType == typeof(decimal) && (!dataField.IsUnbound || dataField.UnboundType == UnboundColumnType.Decimal))
							cellFormat = PivotGridFieldBase.DefaultDecimalFormat;
					}
				}
			}
			return cellFormat;
		}
		PivotGridCellDataProviderBase dataProvider;
		PivotFieldValueItem columnFieldValueItem;
		PivotFieldValueItem rowFieldValueItem;
		object _value;
		string text;
		PivotFieldItemBase dataField;
		int columnIndex, rowIndex;
		bool isValueSet;
		bool isCustomText, isServerText;
		public PivotGridCellItem(PivotGridCellDataProviderBase dataProvider, 
				PivotFieldValueItem columnFieldValueItem, PivotFieldValueItem rowFieldValueItem, 
				int columnIndex, int rowIndex){
			this.dataProvider = dataProvider;
			this.columnFieldValueItem = columnFieldValueItem;
			this.rowFieldValueItem = rowFieldValueItem;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			this.dataField = DataIndex > -1 ?  FieldItems.GetFieldItemByArea(PivotArea.DataArea, DataIndex) : null;			
		}
		protected PivotFieldItemCollection FieldItems {
			get { return Data.FieldItems; }
		}
		protected PivotGridCellDataProviderBase DataProvider { get { return dataProvider; } }
		public PivotGridData Data { get { return DataProvider.Data; } }
		public PivotFieldValueItem ColumnFieldValueItem { get { return columnFieldValueItem; } }
		public PivotFieldValueItem RowFieldValueItem { get { return rowFieldValueItem; } }
		public int ColumnIndex { get { return columnIndex; }}
		public int RowIndex { get { return rowIndex; } }
		void EnsureValue() {
			if(isValueSet) return;
			isValueSet = true;
			PivotCellValue cellValue = DataProvider.GetCellValueEx(this);
			if(IsValueValid) {
				this._value = PivotCellValue.GetValue(cellValue);
				this.text = GetDisplayText(cellValue);
			} else {
				this._value = null;
				this.text = GetDisplayText(null);
			}
			object customValue = DataProvider.GetCustomCellValue(this);
			if(!object.Equals(this._value, customValue)) {
				this._value = customValue;
				this.text = GetDisplayText(new PivotCellValue(customValue));
			}
			string customText = DataProvider.GetCustomCellText(this);
			this.isCustomText = customText != this.text;			
			this.text = customText;
			this.isServerText = this.text == PivotCellValue.GetDisplayText(cellValue);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object ValueInternal {
			get {
				if(!IsValueValid) return null;
				PivotCellValue cellValue = DataProvider.GetCellValueEx(this);
				return PivotCellValue.GetValue(cellValue);
			}
		}
		public object Value {
			get {
				EnsureValue();
				return _value; 
			}
		}
		public string Text { 
			get {
				EnsureValue();
				return text; 
			} 
		}
		public bool IsCustomDisplayText { get { return isCustomText; } }
		public bool IsServerDisplayText { get { return isServerText; } }
		public bool IsEmpty { get { return Text == null || Text == string.Empty; } }
		public int ColumnFieldIndex { get { return ColumnFieldValueItem.VisibleIndex; }}
		public int RowFieldIndex { get { return RowFieldValueItem.VisibleIndex; } }
		public PivotFieldItemBase ColumnField { get { return ColumnFieldValueItem.ColumnField; } }
		public PivotFieldItemBase RowField { get { return RowFieldValueItem.RowField; } }
		public int DataIndex { 
			get { 
				return Data.OptionsDataField.DataFieldArea == PivotArea.ColumnArea ? 
					ColumnFieldValueItem.DataIndex : 
					RowFieldValueItem.DataIndex;
			} 
		}
		public PivotFieldItemBase DataField { get { return dataField; } }
		public Type CellObjectType {
			get {
				if(DataField == null || !IsValueValid) return null;
				if(SummaryType == PivotSummaryType.Count) return typeof(int);
				if(IsObjectTypeChangedBySummaryType) return typeof(decimal);
				return DataField.FieldType;
			}
		}
		bool IsObjectTypeChangedBySummaryType {
			get {
				return SummaryType != PivotSummaryType.Average && SummaryType != PivotSummaryType.Max && SummaryType != PivotSummaryType.Min && SummaryType != PivotSummaryType.Custom;
			}
		}
		public PivotGridValueType ColumnValueType { get { return ColumnFieldValueItem.ValueType; } }
		public PivotGridValueType RowValueType { get { return RowFieldValueItem.ValueType; } }
		public PivotSummaryType SummaryType {
			get {
				PivotSummaryType summaryType = DataField != null ? DataField.SummaryType : PivotSummaryType.Sum;
				if(CustomTotal != null)
					summaryType = CustomTotal.SummaryType;
				return summaryType;
			}
		}
		public PivotGridCustomTotalBase ColumnCustomTotal { get { return ColumnFieldValueItem.CustomTotal; } }
		public PivotGridCustomTotalBase RowCustomTotal { get { return RowFieldValueItem.CustomTotal; } }
		public PivotKPIGraphic KPIGraphic {
			get {
				if(Data == null || DataField == null) return PivotKPIGraphic.None;
				return DataField.KPIGraphic;
			}
		}
		public int KPIValue {
			get {
				try {
					if(Value != null) {
						int state = Convert.ToInt32(Value);
						if(PivotGridData.IsValidKPIState(state)) return state;
					}
				} catch { }
				return PivotGridData.InvalidKPIValue;
			}
		}
		public bool ShowKPIGraphic {
			get {
				return Data != null && KPIGraphic != PivotKPIGraphic.None && Value != null && KPIValue != PivotGridData.InvalidKPIValue;
			}
		}
		public bool IsValueValid { get { return ColumnCustomTotal == null || RowCustomTotal == null; } }
		public bool IsColumnCustomTotal { get { return ColumnCustomTotal != null; } }
		public bool IsRowCustomTotal { get { return RowCustomTotal != null; } }
		public PivotGridCustomTotalBase CustomTotal { 
			get { 
				if(RowCustomTotal != null)
					return RowCustomTotal;
				if(ColumnCustomTotal != null)
					return ColumnCustomTotal;
				return null;
			} 
		}
		public bool IsTotalAppearance { get { return IsValueType(PivotGridValueType.Total); } }
		public bool IsGrandTotalAppearance { get { return IsValueType(PivotGridValueType.GrandTotal); } }
		public bool IsCustomTotalAppearance { get { return CustomTotal != null; } }
		public object GetCellValue(PivotFieldItemBase dataField) {
			return DataProvider.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetFieldValue(PivotFieldItemBase field) {
			if(field.Area == PivotArea.DataArea) return GetCellValue(field);
			return IsFieldValueRetrievable(field) ? DataProvider.GetFieldValue(field, ColumnFieldIndex, RowFieldIndex) : null;
		}
		public bool IsOthersFieldValue(PivotFieldItemBase field) {
			if(field.Area == PivotArea.DataArea || !IsFieldValueRetrievable(field)) return false;
			return DataProvider.GetIsOthersValue(field, ColumnFieldIndex, RowFieldIndex);
		}
		public bool IsFieldValueExpanded(PivotFieldItemBase field) {
			if(!IsFieldValueRetrievable(field)) return false;
			return DataProvider.IsFieldValueExpanded(field, ColumnFieldIndex, RowFieldIndex);
		}
		int GetFieldAreaIndex(PivotFieldItemBase field) {
			return field.Area != PivotArea.DataArea ? field.AreaIndex : FieldItems.DataFieldItem.AreaIndex;
		}
		public bool IsFieldValueRetrievable(PivotFieldItemBase field) {
			if(!field.Visible) return false;
			if(field.Area == PivotArea.ColumnArea)
				return ColumnField != null ? field.AreaIndex <= GetFieldAreaIndex(ColumnField) : false;
			if(field.Area == PivotArea.RowArea)
				return RowField != null ? field.AreaIndex <= GetFieldAreaIndex(RowField) : false;
			return false;
		}
		protected bool IsValueType(PivotGridValueType valueType) {
			if(RowValueType == valueType) return true;
			if(ColumnValueType == valueType) return true;
			return false;
		}
		protected virtual string GetDisplayText(PivotCellValue cellValue) {
			return DataProvider.GetDisplayText(this, cellValue);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FormatInfo GetCellFormatInfo() {
			if(DataField == null) return null;
			return PivotGridCellItem.GetFormatInfo(() => Value, DataField, CustomTotal, IsTotalAppearance, IsGrandTotalAppearance);
		}
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			if(Data.Disposing) return null;
			PivotGridCellItem cellItem = theObject as PivotGridCellItem;
			if(cellItem == null) return null;
			PivotGridFieldPropertyDescriptor fieldDescriptor = descriptor as PivotGridFieldPropertyDescriptor;
			if(fieldDescriptor == null) return null;
			switch(fieldDescriptor.Field.Area) {
				case PivotArea.RowArea:
					return Data.VisualItems.GetFieldValue(fieldDescriptor.Field, cellItem.RowIndex);
				case PivotArea.ColumnArea:
					return Data.VisualItems.GetFieldValue(fieldDescriptor.Field, cellItem.ColumnIndex);
				case PivotArea.DataArea:
					return Data.GetCellValue(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, fieldDescriptor.Field);
				case PivotArea.FilterArea:
					return null;
				default:
					throw new ArgumentException("fieldDescriptor.Field.Area");
			}
		}
		#endregion
		public PivotGridCellType GetCellType() {
			if(IsCustomTotalAppearance)
				return PivotGridCellType.CustomTotal;
			if(IsTotalAppearance)
				return PivotGridCellType.Total;
			if(IsGrandTotalAppearance)
				return PivotGridCellType.GrandTotal;
			return PivotGridCellType.Cell;
		}
	}
#if SL
	public class Queue : List<Point> {
		public Queue() {
		}
		public Point Dequeue() {
			Point val = this[0];
			RemoveAt(0);
			return val;
		}
		public void Enqueue(Point point) {
			Add(point);
		}
	}
#endif
	public abstract class PointObjectCache<T> {
		int capacity;
		Queue queue;
		Dictionary<Point, T> hash;
		public PointObjectCache(int capacity) {
			this.capacity = capacity;
			this.queue = new Queue();
			hash = new Dictionary<Point, T>();
		}
		int Capacity { get { return capacity; } }
		Queue Queue { get { return queue; } }
		Dictionary<Point, T> Hash { get { return hash; } }
		public void AddPointValue(Point point, T value) {
			if(!Contains(point)) {
				if(Queue.Count > Capacity) {
					Point lastPoint = (Point)Queue.Dequeue();
					hash.Remove(lastPoint);
				}
				Queue.Enqueue(point);
			}
			Hash[point] = value;
		}
		public T GetPointValue(Point point) {
			if(!Contains(point))
				return EmptyValue;
			return Hash[point];
		}
		protected abstract T EmptyValue { get; }
		public bool Contains(Point point) {
			return Hash.ContainsKey(point);
		}
		public void Clear() {
			Queue.Clear();
			Hash.Clear();
		}
	}
	public class PointTextCache : PointObjectCache<string> {
		public PointTextCache(int capacity)
			: base(capacity) { }
		protected override string EmptyValue {
			get { return String.Empty; }
		}
	}
	public class PointValueCache : PointObjectCache<object> {
		public PointValueCache(int capacity)
			: base(capacity) { }
		protected override object EmptyValue {
			get { return null; }
		}
	}
	public class PivotGridEmptyCellsDataProvider : PivotGridCellDataProviderBase {
		public PivotGridEmptyCellsDataProvider(PivotGridData data, PivotGridCellDataProviderBase dataProvider)
			: base(data) {
			TextCache = dataProvider.TextCache;
			ValueCache = dataProvider.ValueCache;
		}
		public override PivotCellValue GetCellValueEx(PivotGridCellItem cellItem) {
			Point point = new Point(cellItem.ColumnIndex, cellItem.RowIndex);
			return new PivotCellValue(ValueCache.GetPointValue(point));
		}
		public override object GetCustomCellValue(PivotGridCellItem cellItem) {
			PivotCellValue cellValue = GetCellValueEx(cellItem);
			return cellValue != null ? cellValue.Value : null;
		}
		public override string GetCustomCellText(PivotGridCellItem cellItem) {
			return GetDisplayText(cellItem, null);
		}
		public override PivotCellValue GetCellValueEx(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem) {
			Point point = new Point(columnItem.VisibleIndex, rowItem.VisibleIndex);
			return new PivotCellValue(ValueCache.GetPointValue(point));
		}
		public override string GetDisplayText(PivotGridCellItem cellItem, PivotCellValue cellValue) {
			Point point = new Point(cellItem.ColumnIndex, cellItem.RowIndex);
			return TextCache.GetPointValue(point);
		}
	}
}
