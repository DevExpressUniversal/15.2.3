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
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraPivotGrid.Data {
	public abstract class PivotGridFieldValueItemsDataProviderBase {
		bool isColumn;
		PivotGridData data;
		Type[] fieldTypes = new Type[0];
		public PivotGridFieldValueItemsDataProviderBase(PivotGridData data, bool isColumn) {
			this.data = data;
			this.isColumn = isColumn;
		}
		public PivotGridData Data { get { return data; } }
		public bool IsColumn { get { return isColumn; } }
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public abstract int LevelCount { get; }
		public abstract int CellCount { get; }
		public abstract int GetObjectLevel(int visibleIndex);
		public abstract object GetObjectValue(int visibleIndex);
		public abstract bool IsObjectCollapsed(int visibleIndex);
		public abstract bool IsOthersValue(int visibleIndex);
		public virtual void Reset() { }
		public abstract Type GetFieldColumnType(PivotGridFieldBase field);
		protected virtual void SaveFieldTypesToStream(TypedBinaryWriter writer) {
			writer.Write(fieldTypes.Length);
			for(int i = 0; i < fieldTypes.Length; i++) {
				writer.WriteType(fieldTypes[i]);
			}
		}
		public bool IsObjectVisible(int visibleIndex, int level, bool isObjectCollapsed) {
			if(level + 1 == LevelCount)
				return true;
			if(isObjectCollapsed)
				return true;
			bool singleValue = GetLevelValueCount(level + 1, visibleIndex + 1) == 1;
			Data.EnsureFieldCollections();
			return Data.GetFieldByArea(Area, level).GetTotalSummaryCount(singleValue) > 0;
		}
		public int GetLevelValueCount(int level, int visibleIndex) {
			int count = CellCount;
			int cellCount = 0;
			for(int i = visibleIndex; i < count; i++) {
				int objectLevel = GetObjectLevel(i);
				if(objectLevel == level)
					cellCount++;
				if(objectLevel < level)
					break;
			}
			return cellCount;
		}
		public virtual void SaveToStream(TypedBinaryWriter writer) {
			writer.Write(LevelCount);
			SaveFieldTypesToStream(writer);
			int count = CellCount;
			writer.Write(count);
		}
		protected void SetFieldTypes(Type[] types) {
			fieldTypes = types;
		}
		public Type GetFieldColumnType(int level) {
			return fieldTypes[level];
		}
	}
	public class PivotGridFieldValueItemsDataProvider : PivotGridFieldValueItemsDataProviderBase {
		int levelCount;
		int cellCount;
		public PivotGridFieldValueItemsDataProvider(PivotGridData data, bool isColumn)
			: base(data, isColumn) {
			CalcCellAndLevelCount();
		}
		public override int LevelCount { get { return levelCount; } }
		public override int CellCount { get { return cellCount; } }
		public override object GetObjectValue(int visibleIndex) {
			return Data.GetObjectValue(IsColumn, visibleIndex);
		}
		public override int GetObjectLevel(int visibleIndex) {
			return Data.GetObjectLevel(IsColumn, visibleIndex);
		}
		public override bool IsObjectCollapsed(int visibleIndex) {
			return Data.IsObjectCollapsed(IsColumn, visibleIndex);
		}
		public override bool IsOthersValue(int visibleIndex) {
			return Data.GetIsOthersValue(IsColumn, visibleIndex);
		}
		protected override void SaveFieldTypesToStream(TypedBinaryWriter writer) {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(Area, false);
			if(Data.GetIsDataFieldsVisible(IsColumn) && Data.OptionsDataField.DataFieldArea == Area)
				fields.Insert(Data.DataField.AreaIndex, Data.DataField);
			SetFieldTypes(fields.Select((f) => GetFieldColumnType(f)).ToArray());
			base.SaveFieldTypesToStream(writer);
		}
		public override Type GetFieldColumnType(PivotGridFieldBase field) {
			Type res = Data.GetFieldType(field);
			if(res == typeof(object) && Data.GetCellCount(IsColumn) > 0 && Data.IsFieldTypeCheckRequired(field)) {
				object value = GetObjectValueByLevel(field.AreaIndex);
				if(value != null)
					res = value.GetType();
			}
			return res;
		}
		protected object GetObjectValueByLevel(int level) {
			int count = Data.GetCellCount(IsColumn);
			for(int i = 0; i < count; i++) {
				if(GetObjectLevel(i) == level)
					return GetObjectValue(i);
			}
			return null;
		}
		public override void Reset() {
			CalcCellAndLevelCount();
		}
		protected void CalcCellAndLevelCount() {
			this.levelCount = Data.GetLevelCount(IsColumn);
			this.cellCount = Data.GetCellCount(IsColumn);
		}
	}
	public class PivotGridFieldValueItemsStreamDataProvider : PivotGridFieldValueItemsDataProviderBase {
		int levelCount, cellCount;
		public PivotGridFieldValueItemsStreamDataProvider(PivotGridData data, bool isColumn)
			: base(data, isColumn) {
			this.cellCount = 0;
			this.levelCount = 0;
		}
		public override int LevelCount { get { return levelCount; } }
		public override int CellCount { get { return cellCount; } }
		public override object GetObjectValue(int visibleIndex) {
			return null;
		}
		public override int GetObjectLevel(int visibleIndex) {
			return -1;
		}
		public override bool IsObjectCollapsed(int visibleIndex) {
			return false;
		}
		public override bool IsOthersValue(int visibleIndex) {
			return false;
		}
		public override Type GetFieldColumnType(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		public void LoadFromStream(TypedBinaryReader reader) {
			levelCount = reader.ReadInt32();
			int fieldTypesCount = reader.ReadInt32();
			Type[] fieldTypes = new Type[fieldTypesCount];
			for(int i = 0; i < fieldTypesCount; i++) {
				fieldTypes[i] = reader.ReadType();
			}
			cellCount = reader.ReadInt32();
			SetFieldTypes(fieldTypes);
		}
	}
	public class PivotFieldValueItemBase {
		PivotGridFieldValueItemsDataProviderBase dataProvider;
		int state;
		protected bool GetState(int flag) {
			return (state & flag) != 0;
		}
		protected void SetState(int flag, bool value) {
			this.state = value ? (this.state | flag) : (this.state & ~flag);
		}
		public PivotFieldValueItemBase(PivotGridFieldValueItemsDataProviderBase dataProvider) {
			this.dataProvider = dataProvider;
			IsColumn = dataProvider.IsColumn;
		}
		public virtual PivotGridFieldValueItemsDataProviderBase DataProvider {
			get {
				if(dataProvider == null)
					dataProvider = new PivotGridFieldValueItemsDataProvider(Data, IsColumn);
				return dataProvider;
			}
			protected set { dataProvider = value; }
		}
		public virtual PivotGridData Data { get { return dataProvider.Data; } }
		public bool IsColumn {
			get { return GetState(0x1); }
			protected set { SetState(0x1, value); }
		}
		public PivotArea Area { get { return IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea; } }
		public PivotArea CrossArea { get { return IsColumn ? PivotArea.RowArea : PivotArea.ColumnArea; } }
		protected PivotGridOptionsDataField OptionsDataField { get { return Data.OptionsDataField; } }
		protected int LastFieldLevel {
			get {
				return GetCorrectedFieldValueLevel(LevelCount - 1);
			}
		}
		protected int LevelCount { get { return DataProvider.LevelCount; } }
		protected int GetCorrectedFieldValueLevel(int level) {
			if(IsLevelAfterDataField(level))
				level++;
			return level;
		}
		protected bool IsLevelBeforeDataField(int level) {
			return IsDataFieldsVisible && OptionsDataField.DataFieldAreaIndex > level;
		}
		public bool IsLevelAfterDataField(int level) {
			return IsDataFieldsVisible && OptionsDataField.DataFieldAreaIndex <= level;
		}
		public bool IsDataFieldsVisible {
			get {
				return Data.GetIsDataFieldsVisible(IsColumn);
			}
		}
		public bool IsDataLocatedInThisArea { get { return OptionsDataField.DataFieldArea == Area; } }
		public int DataLevel {
			get {
				int index = OptionsDataField.DataFieldAreaIndex;
				if(index == 0 && LevelCount == 0 && !Data.ShouldRemoveGrandTotalHeader(IsColumn))
					index++;
				return index;
			}
		}
		public bool IsRowTree {
			get { return !IsColumn && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree; }
		}
		protected string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		public void ResetDataProvider() {
			dataProvider = null;
		}
		public virtual void SaveToStream(TypedBinaryWriter writer) {
			writer.Write(IsColumn);
		}
		protected virtual void LoadFromStream(TypedBinaryReader reader) {
			IsColumn = reader.ReadBoolean();
		}
	}
	public enum PivotFieldValueItemType { Cell, TotalCell, CustomTotalCell, GrandTotalCell, DataCell, TopDataCell };
	public abstract class PivotFieldValueItem : PivotFieldValueItemBase {
		public static PivotFieldValueItem LoadItem(PivotFieldValueItemType type,
									PivotGridFieldValueItemsDataProviderBase dataProvider,
									TypedBinaryReader reader) {
			switch(type) {
				case PivotFieldValueItemType.Cell:
					return new PivotFieldCellValueItem(dataProvider, reader);
				case PivotFieldValueItemType.TotalCell:
					return new PivotFieldTotalCellValueItem(dataProvider, reader);
				case PivotFieldValueItemType.CustomTotalCell:
					return new PivotFieldCustomTotalCellValueItem(dataProvider, reader);
				case PivotFieldValueItemType.GrandTotalCell:
					return new PivotFieldGrandTotalCellValueItem(dataProvider, reader);
				case PivotFieldValueItemType.DataCell:
					return new PivotFieldDataCellValueItem(dataProvider, reader);
				case PivotFieldValueItemType.TopDataCell:
					return new PivotFieldTopDataCellValueItem(dataProvider, reader);
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}
		int visibleIndex;
		int index;
		int dataIndex;
		int uniqueIndex;
		int minLastLevelIndex;
		int maxLastLevelIndex;
		int startLevel;
		int endLevel;
		int fieldLevel;
		object value;
		object customValue;
		List<PivotFieldValueItem> cells;
		List<PivotFieldValueItem> unpagedCells;
		PivotFieldValueItem parent;
		List<PivotGridFieldPair> savedSortedBySummaryFields;
		public PivotFieldValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int level, int dataIndex)
			: base(dataProvider) {
			this.visibleIndex = visibleIndex;
			this.dataIndex = dataIndex;
			if(OptionsDataField.DataFieldVisible) {
				this.fieldLevel = GetCorrectedFieldValueLevel(level);
				this.startLevel = this.endLevel = FieldLevel;
			} else {
				this.fieldLevel = level;
				this.startLevel = this.endLevel = GetCorrectedFieldValueLevel(level);
			}
			this.cells = null;
			this.unpagedCells = null;
		}
		public PivotFieldValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider) {
			LoadFromStream(reader);
		}
		protected internal PivotFieldValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: this(dataProvider, parentItem.VisibleIndex, dataProvider.GetObjectLevel(parentItem.VisibleIndex), parentItem.DataIndex) {
			this.StartLevel = parentItem.StartLevel;
			this.EndLevel = parentItem.EndLevel;
			this.SetCustomValue(value);
		}
		PivotFieldItemBase GetFieldByLevel() {
			return FieldItems.GetFieldItemByLevel(IsColumn, FieldLevel);
		}
		protected PivotFieldItemCollection FieldItems {
			get { return Data.FieldItems; }
		}
		public abstract PivotFieldValueItemType ItemType { get; }
		public int CellCount { get { return cells != null ? cells.Count : 0; } }
		public int UnpagedCellCount {
			get {
				if(unpagedCells != null)
					return unpagedCells.Count;
				else
					return CellCount;
			}
		}
		public int TotalsCount {
			get {
				if(IsCollapsed || Field == null)
					return 0;
				switch(Field.TotalsVisibility) {
					case PivotTotalsVisibility.AutomaticTotals:
						if(IsDataFieldsVisible && IsDataLocatedInThisArea && Data.OptionsDataField.AreaIndex > Field.AreaIndex)
							return Data.DataFieldCount;
						else {
							bool singleValue = (CellCount == 1);
							return Field.GetTotalSummaryCount(singleValue);
						}
					case PivotTotalsVisibility.CustomTotals:
						return Field.CustomTotals.Count;
				}
				return 0;
			}
		}
		public PivotFieldValueItem GetCell(int index) {
			return cells[index];
		}
		public PivotFieldValueItem GetUnpagedCell(int index) {
			if(unpagedCells != null)
				return unpagedCells[index];
			else
				return GetCell(index);
		}
		internal PivotFieldValueItem Parent {
			get { return parent; }
			set { parent = value; }
		}
		public PivotFieldValueItem[] GetLastLevelCells() {
			List<PivotFieldValueItem> list = new List<PivotFieldValueItem>();
			CopyLastLevelCells(this, list);
			return list.ToArray();
		}
		protected void CopyLastLevelCells(PivotFieldValueItem root, List<PivotFieldValueItem> list) {
			if(CellCount == 0) {
				if(root != this)
					list.Add(this);
				return;
			}
			for(int i = 0; i < CellCount; i++) {
				GetCell(i).CopyLastLevelCells(root, list);
			}
		}
		public void AddCell(PivotFieldValueItem cell) {
			if(this.cells == null)
				this.cells = new List<PivotFieldValueItem>();
			cells.Add(cell);
		}
		protected internal bool RemoveCell(PivotFieldValueItem cell) {
			if(cell == null || cells == null)
				return false;
			if(unpagedCells != null)
				unpagedCells.Remove(cell);
			return cells.Remove(cell);
		}
		protected internal void ClearCells() {
			if(cells == null)
				return;
			foreach(PivotFieldValueItem cell in cells)
				cell.Parent = null;
			cells.Clear();
		}
		public void SaveCellsAsUnpagedCells() {
			if(cells != null)
				unpagedCells = new List<PivotFieldValueItem>(cells);
		}
		public int VisibleIndex { get { return visibleIndex; } }
		public int UniqueIndex { get { return uniqueIndex; } set { uniqueIndex = value; } }
		public int MinLastLevelIndex { get { return minLastLevelIndex; } set { minLastLevelIndex = value; } }
		public int MaxLastLevelIndex { get { return maxLastLevelIndex; } set { maxLastLevelIndex = value; } }
		public int Level { get { return StartLevel; } }
		public int StartLevel { get { return startLevel; } set { startLevel = value; } }
		public int EndLevel { get { return endLevel; } set { endLevel = value; } }
		public bool IsLastFieldLevel {
			get { return GetState(0x20); }
			set { SetState(0x20, value); }
		}
		public bool IsLastRowTreeLevel {
			get { return IsRowTree && (EndLevel == EndRowTreeLevel || LevelCount == 0); }
		}
		public int StartRowTreeLevel { get { return IsRowTree ? GetCorrectedFieldValueLevel(0) : -1; } }
		public int EndRowTreeLevel { get { return IsRowTree ? LastFieldLevel : -1; } }
		public int SpanCount {
			get {
				if(IsLastFieldLevel)
					return 1;
				int count = 0;
				for(int i = 0; i < CellCount; i++) {
					count += GetCell(i).SpanCount;
				}
				return count;
			}
		}
		protected internal int FieldLevel { get { return fieldLevel; } }
		public virtual bool ContainsLevel(int level) { return level >= StartLevel && level <= EndLevel; }
		public virtual int DataIndex { get { return dataIndex; } set { dataIndex = value; } }
		public virtual PivotSummaryType SummaryType {
			get {
				PivotFieldItemBase field = DataField;
				if(field != null)
					return field.SummaryType;
				else
					return PivotSummaryType.Sum;
			}
		}
		public virtual PivotGridCustomTotalBase CustomTotal { get { return null; } }
		string text;
		internal void SetCloneText() {
			text = Text;
		}
		public string Text {
			get {
				if(!IsTextCalculated && !Data.IsLocked) {
					IsTextCalculated = true;
					text = CalculateText();
				}
				return text;
			}
		}
		bool IsTextCalculated {
			get { return GetState(0x80); }
			set { SetState(0x80, value); }
		}
		bool HasValue {
			get { return GetState(0x100); }
			set { SetState(0x100, value); }
		}
		public string DisplayText {
			get {
				string res = Text;
				if(Field != null && Field.Area == PivotArea.DataArea && Field.Options.ShowSummaryTypeName)
					res += " (" + PivotGridLocalizer.GetSummaryTypeText(Field.SummaryType) + ")";
				return res;
			}
		}
		public bool IsCustomDisplayText {
			get { return GetState(0x4); }
			private set { SetState(0x4, value); }
		}
		public bool IsVisible {
			get {
				if(!IsRowTree)
					return true;
				if(ValueType == PivotGridValueType.Value && StartLevel >= StartRowTreeLevel && EndLevel < EndRowTreeLevel)
					return false;
				return true;
			}
		}
		protected virtual string GetTextCore() {
			return string.Empty;
		}
		bool HasCustomValue {
			get { return GetState(0x2); }
			set { SetState(0x2, value); }
		}
		protected string CustomTextInternal {
			get {
				return (HasCustomValue && customValue != null) ? customValue.ToString() : string.Empty;
			}
		}
		internal string GetCustomText(bool calculateIsCustomDisplayText) {
			if(HasCustomValue) {
				if(calculateIsCustomDisplayText)
					IsCustomDisplayText = HasCustomValue;
				return CustomTextInternal;
			}
			string defaultText = GetTextCore();
			string result = Data.GetCustomFieldValueText(this, defaultText);
			if(calculateIsCustomDisplayText) {
				IsCustomDisplayText = defaultText != result;
			}
			return result;
		}
		string CalculateText() {
			if(IsVisible) {
				return GetCustomText(true);
			} else {
				IsCustomDisplayText = false;
				return string.Empty;
			}
		}
		public virtual bool IsOthersRow {
			get {
				if(!GetState(0x8)) {
					IsOthersRow = DataProvider.IsOthersValue(VisibleIndex);
				}
				return GetState(0x10);
			}
			set {
				SetState(0x8, true);
				SetState(0x10, value);
			}
		}
		public virtual PivotGridValueType ValueType { get { return PivotGridValueType.Value; } }
		public virtual PivotFieldItemBase Field { get { return GetFieldByLevel(); } }
		public virtual PivotFieldItemBase ResizingField { get { return ResizingFieldInternal; } }
		protected PivotFieldItemBase ResizingFieldInternal {
			get {
				if(IsRowTree)
					return IsLastRowTreeLevel ? FieldItems.RowTreeFieldItem : null;
				return Field;
			}
		}
		public PivotFieldItemBase ColumnField { get { return IsColumn ? GetFieldByLevel() : null; } }
		public PivotFieldItemBase RowField { get { return !IsColumn ? GetFieldByLevel() : null; } }
		public PivotFieldItemBase DataField {
			get { return FieldItems.GetFieldItemByArea(PivotArea.DataArea, DataIndex); }
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(IsColumn ? VisibleIndex : -1, !IsColumn ? VisibleIndex : -1, DataIndex);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateQueryModeDrillDownDataSource(maxRowCount, customColumns);
		}
		public PivotDrillDownDataSource CreateQueryModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetQueryModeDrillDownDataSource(IsColumn ? VisibleIndex : -1, !IsColumn ? VisibleIndex : -1, DataIndex,
				maxRowCount, customColumns);
		}
		public virtual object Value {
			get {
				if(HasCustomValue)
					return customValue;
				if(HasValue)
					return value;
				return VisibleIndex > -1 ? DataProvider.GetObjectValue(VisibleIndex) : null;
			}
		}
		protected void InitLevels(int level) {
			StartLevel = EndLevel = level;
		}
		public int CellLevelCount { get { return EndLevel - StartLevel + 1; } }
		public virtual bool ShowCollapsedButton { get { return false; } }
		public virtual bool IsCollapsed { get { return false; } }
		public bool IsTotal { get { return ValueType != PivotGridValueType.Value; } }
		public bool CanShowSortBySummary {
			get {
				if(!IsLastFieldLevel || Data == null)
					return false;
				if(Data.IsOLAP && CustomTotal != null)
					return false;
				if(Field != null)
					return !(IsCollapsed && Field.TotalsVisibility == PivotTotalsVisibility.CustomTotals);
				return true;
			}
		}
		public bool AllowExpand {
			get {
				if(Field == null || Field.Options.AllowExpand == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowExpand;
				return Field.Options.AllowExpand == DefaultBoolean.True;
			}
		}
		protected bool CanShowCollapsedButton {
			get { return !LockExpand && StartLevel != LastFieldLevel && AllowExpand && Data.CanExpandValue(this); }
		}
		internal bool LockExpand {
			get { return GetState(0x40); }
			set { SetState(0x40, value); }
		}
		public bool AllowExpandOnDoubleClick {
			get {
				return !LockExpand && (CellCount > 0 && cells[0].Field != null && cells[0].Field.Area != PivotArea.DataArea || ShowCollapsedButton || IsTotal && ValueType != PivotGridValueType.GrandTotal);
			}
		}
		protected bool IsTotalsLocationFarOrClosed {
			get {
				return Data.OptionsView.IsTotalsFar(IsColumn, PivotGridValueType.Value) || IsCollapsed;
			}
		}
		protected bool IsTotalsVisible {
			get {
				return Field.GetTotalSummaryCount(CellCount <= 1) > 0;
			}
		}
		public int Index { get { return index; } }
		internal void SetIndex(int value) { index = value; }
		protected internal List<PivotGridFieldPair> SavedSortedBySummaryFields { get { return savedSortedBySummaryFields; } set { savedSortedBySummaryFields = value; } }
		public List<PivotFieldItemBase> GetCrossAreaFields() {
			return FieldItems.GetFieldItemsByArea(CrossArea, false);
		}
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(visibleIndex);
			writer.Write(dataIndex);
			writer.Write(uniqueIndex);
			writer.Write(startLevel);
			writer.Write(endLevel);
			writer.Write(fieldLevel);
			writer.Write(IsOthersRow);
			writer.Write(minLastLevelIndex);
			writer.Write(maxLastLevelIndex);
			SaveCustomValueToStream(writer);
			SaveValueToStream(writer);
			SaveSortedBySummaryFieldsToStream(writer);
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			visibleIndex = reader.ReadInt32();
			dataIndex = reader.ReadInt32();
			uniqueIndex = reader.ReadInt32();
			startLevel = reader.ReadInt32();
			endLevel = reader.ReadInt32();
			fieldLevel = reader.ReadInt32();
			IsOthersRow = reader.ReadBoolean();
			minLastLevelIndex = reader.ReadInt32();
			maxLastLevelIndex = reader.ReadInt32();
			LoadCustomValueFromStream(reader);
			value = LoadValueFromStream(reader);
			savedSortedBySummaryFields = LoadSortedBySummaryFieldsFromStream(reader);
			HasValue = true;
		}
		protected virtual void SaveCustomValueToStream(TypedBinaryWriter writer) {
			writer.Write(HasCustomValue);
			if(HasCustomValue)
				writer.WriteTypedObject(this.customValue);
		}
		protected virtual void LoadCustomValueFromStream(TypedBinaryReader reader) {
			HasCustomValue = reader.ReadBoolean();
			if(HasCustomValue)
				this.customValue = reader.ReadTypedObject();
		}
		protected virtual void SaveValueToStream(TypedBinaryWriter writer) {
			if(IsOthersRow)
				return;
			if(DataProvider.GetFieldColumnType(FieldLevel) == typeof(object))
				writer.WriteTypedObject(Value);
			else
				writer.WriteObject(Value);
		}
		protected virtual object LoadValueFromStream(TypedBinaryReader reader) {
			if(IsOthersRow)
				return DataControllerGroupHelperBase.OthersValue;
			Type fieldType = DataProvider.GetFieldColumnType(FieldLevel);
			if(fieldType == typeof(object))
				return reader.ReadTypedObject();
			else
				return reader.ReadObject(fieldType);
		}
		protected void SetCustomValue(object value) {
			this.customValue = value;
			HasCustomValue = true;
		}
		void SaveSortedBySummaryFieldsToStream(TypedBinaryWriter writer) {
			int count = savedSortedBySummaryFields != null ? savedSortedBySummaryFields.Count : 0;
			writer.Write(count);
			if(count > 0) {
				foreach(PivotGridFieldPair pair in savedSortedBySummaryFields) {
					pair.SaveToStream(writer);
				}
			}
		}
		List<PivotGridFieldPair> LoadSortedBySummaryFieldsFromStream(TypedBinaryReader reader) {
			List<PivotGridFieldPair> res = null;
			int count = reader.ReadInt32();
			if(count > 0) {
				res = new List<PivotGridFieldPair>(count);
				for(int i = 0; i < count; i++) {
					res.Add(PivotGridFieldPair.LoadPair(Data, reader));
				}
			}
			return res;
		}
	}
	public class PivotFieldCellValueItem : PivotFieldValueItem {
		public PivotFieldCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int level, bool isObjectCollapsed, int dataIndex)
			: base(dataProvider, visibleIndex, level, dataIndex) {
			SetIsCollapsed(isObjectCollapsed);
			if(IsCollapsed) {
				if(IsDataLocatedInThisArea && Data.DataFieldCount > 1 && Level < OptionsDataField.DataFieldAreaIndex)
					EndLevel = LevelCount - 1;
				else
					EndLevel = LastFieldLevel;
			}
		}
		protected internal PivotFieldCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
			SetIsCollapsed(parentItem.IsCollapsed);
		}
		public PivotFieldCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.Cell; } }
		public override bool IsCollapsed {
			get { return GetState(0x800); }
		}
		void SetIsCollapsed(bool value) {
			SetState(0x800, value);
		}
		public override bool ShowCollapsedButton {
			get { return CanShowCollapsedButton && (IsTotalsLocationFarOrClosed || !IsTotalsVisible) && IsVisible; }
		}
		protected override string GetTextCore() {
			if(IsOthersRow)
				return GetLocalizedString(PivotGridStringId.TopValueOthersRow);
			return Data.GetField(Field).GetValueText(IsColumn, VisibleIndex, Value);
		}
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(IsCollapsed);
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			SetIsCollapsed(reader.ReadBoolean());
		}
	}
	public class PivotFieldTotalCellValueItem : PivotFieldValueItem {
		bool IsBeforeData {
			get { return GetState(0x400); }
			set { SetState(0x400, value); }
		}
		public PivotFieldTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex)
			: base(dataProvider, visibleIndex, dataProvider.GetObjectLevel(visibleIndex), dataIndex) {
			EndLevel = LastFieldLevel;
			IsBeforeData = IsLevelBeforeDataField(Level) && OptionsDataField.DataFieldAreaIndex < LevelCount;
			if(IsBeforeData)
				EndLevel -= 1;
		}
		protected internal PivotFieldTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
		}
		public PivotFieldTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.TotalCell; } }
		public override bool ShowCollapsedButton { get { return CanShowCollapsedButton && !IsTotalsLocationFarOrClosed; } }
		public override PivotGridValueType ValueType { get { return PivotGridValueType.Total; } }
		protected override string GetTextCore() {
			if(IsOthersRow)
				return Data.GetField(Field).GetTotalOthersText();
			return Data.GetField(Field).GetTotalValueText(IsColumn, VisibleIndex, Value);
		}
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(IsBeforeData);
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			IsBeforeData = reader.ReadBoolean();
		}
	}
	public class PivotFieldCustomTotalCellValueItem : PivotFieldTotalCellValueItem {
		PivotGridCustomTotalBase customTotal;
		public bool IsFirst {
			get { return GetState(0x200); }
			private set { SetState(0x200, value); }
		}
		public PivotFieldCustomTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex,
			PivotGridCustomTotalBase customTotal, int dataIndex, bool isFirst)
			: base(dataProvider, visibleIndex, dataIndex) {
			this.customTotal = customTotal;
			IsFirst = isFirst;
			EndLevel = LastFieldLevel;
			if(IsLevelBeforeDataField(Level) && OptionsDataField.DataFieldAreaIndex < LevelCount)
				EndLevel -= 1;
		}
		protected internal PivotFieldCustomTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
		}
		public PivotFieldCustomTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		public override bool ShowCollapsedButton {
			get {
				bool res = CanShowCollapsedButton && !IsTotalsLocationFarOrClosed && IsFirst;
				if(res && !IsColumn && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
					res = false;
				return res;
			}
		}
		PivotFieldItemBase CustomTotalField { get { return base.Field; } }
		public override PivotGridCustomTotalBase CustomTotal { get { return customTotal; } }
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.CustomTotalCell; } }
		public override PivotGridValueType ValueType { get { return PivotGridValueType.CustomTotal; } }
		public override PivotSummaryType SummaryType { get { return customTotal.SummaryType; } }
		protected override string GetTextCore() { return CustomTotal.GetValueText(IsColumn, VisibleIndex, Value); }
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(IsFirst);
			bool hasCustomTotal = CustomTotal != null;
			writer.Write(hasCustomTotal);
			if(hasCustomTotal)
				writer.Write(CustomTotalField.CustomTotals.IndexOf(CustomTotal));
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			IsFirst = reader.ReadBoolean();
			if(reader.ReadBoolean())
				customTotal = CustomTotalField.CustomTotals[reader.ReadInt32()];
		}
	}
	public class PivotFieldGrandTotalCellValueItem : PivotFieldValueItem {
		PivotFieldItemBase childDataField;
		public PivotFieldGrandTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int dataIndex)
			: base(dataProvider, -1, dataProvider.GetObjectLevel(-1), dataIndex) {
			StartLevel = 0;
			EndLevel = Math.Max(0, LevelCount - 1);
			if(!IsColumn && DataProvider.CellCount == 0 && Data.OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree)
				EndLevel = Math.Max(0, Data.GetFieldCountByArea(PivotArea.RowArea) - 1);
		}
		public PivotFieldGrandTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		protected internal PivotFieldGrandTotalCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
		}
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.GrandTotalCell; } }
		public override PivotFieldItemBase Field {
			get {
				if(FieldItems.GetFieldCountByArea(Area) == 0 && FieldItems.DataFieldCount == 1)
					return FieldItems.GetFieldItemByArea(PivotArea.DataArea, 0);
				return null;
			}
		}
		protected PivotFieldItemBase ChildDataField {
			get {
				if(childDataField == null)
					childDataField = GetChildDataField();
				return childDataField;
			}
		}
		protected PivotFieldItemBase GetChildDataField() {
			if(!IsDataLocatedInThisArea)
				return null;
			PivotFieldItemBase res = null;
			for(int i = 0; i < FieldItems.Count; i++) {
				PivotFieldItemBase field = FieldItems[i];
				if(field.Area == PivotArea.DataArea && field.Options.ShowGrandTotal) {
					if(res == null)
						res = field;
					else
						return null;	
				}
			}
			return res;
		}
		public override PivotFieldItemBase ResizingField {
			get {
				if(Field != null && !IsRowTree)
					return Field;
				if(!IsColumn)
					return GetRowAreaResizingField();
				else
					return GetColumnAreaResizingField();
			}
		}
		PivotFieldItemBase GetColumnAreaResizingField() {
			if(ChildDataField != null && FieldItems.DataFieldCount > 1)
				return ChildDataField;
			if(FieldItems.ColumnFieldCount == 0 && Data.DataFieldCount < 1)
				return FieldItems.GetFieldItemByArea(PivotArea.DataArea, 0);
			if(FieldItems.DataFieldCount < 2 && Data.ColumnFieldCount > 0)
				return FieldItems.GetFieldItemByArea(PivotArea.ColumnArea, FieldItems.ColumnFieldCount - 1);
			if(Data.ColumnFieldCount > 0)
				return FieldItems.GetFieldItemByLevel(true, 0);
			return FieldItems.DataFieldItem;
		}
		PivotFieldItemBase GetRowAreaResizingField() {
			if(IsRowTree)
				return IsLastRowTreeLevel && !(FieldItems.RowFieldCount == 0 && FieldItems.DataFieldCount > 1 && FieldItems.DataFieldItem.Area == PivotArea.RowArea) ? FieldItems.RowTreeFieldItem : FieldItems.DataFieldItem;
			if(IsDataFieldsVisible && OptionsDataField.DataFieldAreaIndex == Level)
				return FieldItems.DataFieldItem;
			if(Data.RowFieldCount > 0)
				return FieldItems.GetFieldItemByArea(PivotArea.RowArea, 0);
			else if(!IsDataFieldsVisible)
				return FieldItems.DataFieldItem;
			return FieldItems.RowTreeFieldItem;
		}
		public override PivotGridValueType ValueType { get { return PivotGridValueType.GrandTotal; } }
		protected override string GetTextCore() {
			if(Field != null)
				return Data.GetField(Field).GetGrandTotalText();
			return GetLocalizedString(PivotGridStringId.GrandTotal);
		}
		public override bool IsOthersRow { get { return false; } }
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(ChildDataField != null ? ChildDataField.Index : -1);
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			int childFieldIndex = reader.ReadInt32();
			childDataField = childFieldIndex >= 0 ? FieldItems[childFieldIndex] : null;
		}
		protected override void SaveValueToStream(TypedBinaryWriter writer) {
		}
		protected override object LoadValueFromStream(TypedBinaryReader reader) {
			return null;
		}
	}
	public class PivotFieldDataCellValueItem : PivotFieldValueItem {
		PivotGridValueType valueType;
		public PivotFieldDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex,
							PivotGridValueType valueType, int level)
			: base(dataProvider, visibleIndex, dataProvider.GetObjectLevel(visibleIndex), dataIndex) {
			InitLevels(level);
			this.valueType = valueType;
		}
		public PivotFieldDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		protected internal PivotFieldDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
			this.valueType = parentItem.ValueType;
		}
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.DataCell; } }
		public override PivotFieldItemBase Field { get { return DataField; } }
		public override PivotFieldItemBase ResizingField {
			get {
				if(IsColumn)
					return base.ResizingField;
				if(Level == DataLevel)
					return FieldItems.DataFieldItem;
				if(IsLastRowTreeLevel)
					return FieldItems.RowTreeFieldItem;
				if(FieldItems.GetFieldCountByArea(Area) > 0)
					return FieldItems.GetFieldItemByArea(Area, FieldItems.GetFieldCountByArea(Area) - 1);
				return base.ResizingField;
			}
		}
		public override object Value { get { return null; } }
		public override PivotGridValueType ValueType { get { return this.valueType; } }
		protected override string GetTextCore() { return DataField.ToString(); }
		public override void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write((byte)valueType);
		}
		protected override void LoadFromStream(TypedBinaryReader reader) {
			base.LoadFromStream(reader);
			valueType = (PivotGridValueType)reader.ReadByte();
		}
		protected override void SaveValueToStream(TypedBinaryWriter writer) {
		}
		protected override object LoadValueFromStream(TypedBinaryReader reader) {
			return null;
		}
	}
	public class PivotFieldCustomTotalDataCellValueItem : PivotFieldCustomTotalCellValueItem {
		public PivotFieldCustomTotalDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex,
			PivotGridCustomTotalBase customTotal, int dataIndex)
			: base(dataProvider, visibleIndex, customTotal, dataIndex, false) {
			InitLevels(LevelCount);
			if(StartLevel == 0)
				InitLevels(1);
		}
		public override bool ShowCollapsedButton { get { return false; } }
		public override PivotFieldItemBase Field { get { return DataField; } }
		protected override string GetTextCore() { return DataField.ToString(); }
		public override object Value { get { return null; } }
	}
	public class PivotFieldTopDataCellValueItem : PivotFieldValueItem {
		public PivotFieldTopDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, int visibleIndex, int dataIndex)
			: base(dataProvider, visibleIndex, dataProvider.GetObjectLevel(visibleIndex), dataIndex) {
			InitLevels(DataLevel);
		}
		public PivotFieldTopDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, TypedBinaryReader reader)
			: base(dataProvider, reader) {
		}
		protected internal PivotFieldTopDataCellValueItem(PivotGridFieldValueItemsDataProviderBase dataProvider, object value, PivotFieldValueItem parentItem)
			: base(dataProvider, value, parentItem) {
		}
		public override PivotFieldValueItemType ItemType { get { return PivotFieldValueItemType.TopDataCell; } }
		public override PivotFieldItemBase Field { get { return DataField; } }
		public override PivotFieldItemBase ResizingField { get { return FieldItems.DataFieldItem; } }
		protected override string GetTextCore() { return DataField.ToString(); }
		protected override void SaveValueToStream(TypedBinaryWriter writer) {
		}
		protected override object LoadValueFromStream(TypedBinaryReader reader) {
			return null;
		}
		public override object Value { get { return null; } }
	}
	public class PivotDenyExpandValues {
		PivotGridData data;
		public PivotDenyExpandValues(PivotGridData data) {
			this.data = data;
		}
		Dictionary<PivotGridFieldBase, Dictionary<string, object>> denyExpand;
		Dictionary<PivotGridFieldBase, Dictionary<string, object>> DenyExpand {
			get {
				if(denyExpand == null)
					denyExpand = new Dictionary<PivotGridFieldBase, Dictionary<string, object>>();
				return denyExpand;
			}
		}
		protected PivotGridData Data { get { return data; } }
		bool DenyExpandValuesAllowed { get { return data.PivotDataSource.Capabilities.HasFlag(PivotDataSourceCaps.DenyExpandValuesAllowed); } }
		public int Count { get { return denyExpand != null ? denyExpand.Count : 0; } }
		public virtual void ClearDenyExpandValues() {
			if(!DenyExpandValuesAllowed)
				return;
			if(denyExpand != null)
				denyExpand.Clear();
		}
		public virtual void DenyExpandValue(PivotGridFieldBase field, int visibleIndex) {
			if(!DenyExpandValuesAllowed || field == null)
				return;
			string value = GetFieldValueItemUniquePath(field.IsColumn, visibleIndex, field.AreaIndex);
			Dictionary<string, object> values;
			if(!DenyExpand.TryGetValue(field, out values)) {
				values = new Dictionary<string, object>();
				DenyExpand.Add(field, values);
			}
			if(value != null && !values.ContainsKey(value))
				values.Add(value, null);
		}
		public virtual bool CanExpandValue(PivotFieldValueItem item) {
			if(!DenyExpandValuesAllowed)
				return true;
			PivotGridFieldBase field = Data.GetField(item.Field);
			string value = GetFieldValueItemUniquePath(item.IsColumn, item.VisibleIndex, field.AreaIndex);
			Dictionary<string, object> values;
			if(DenyExpand.TryGetValue(field, out values))
				return !values.ContainsKey(value);
			else
				return true;
		}
		public virtual string GetDenyExpandState() {
			if(!DenyExpandValuesAllowed)
				return string.Empty;
			if(DenyExpand.Count == 0)
				return string.Empty;
			return PivotGridSerializeHelper.ToBase64String(SaveDenyExpandToStream, null);
		}
		public virtual void SetDenyExpandState(string state) {
			if(string.IsNullOrEmpty(state))
				return;
			Stream stream = new MemoryStream(Convert.FromBase64String(state));
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			int fieldsCount = reader.ReadInt32();
			for(int i = 0; i < fieldsCount; i++) {
				PivotGridFieldBase field = Data.Fields[reader.ReadInt32()];
				int valuesCount = reader.ReadInt32();
				Dictionary<string, object> values = new Dictionary<string, object>();
				for(int j = 0; j < valuesCount; j++)
					values.Add(reader.ReadString(), null);
				DenyExpand.Add(field, values);
			}
			reader.Dispose();
			stream.Dispose();
		}
		void SaveDenyExpandToStream(TypedBinaryWriter writer) {			
			writer.Write(DenyExpand.Count);
			foreach(KeyValuePair<PivotGridFieldBase, Dictionary<string, object>> pair in DenyExpand) {
				writer.Write(Data.Fields.IndexOf(pair.Key));
				writer.Write(pair.Value.Count);
				foreach(KeyValuePair<string, object> pair2 in pair.Value)
					writer.Write(pair2.Key);
			}
		}
		string GetFieldValueItemUniquePath(bool isColumn, int visibleIndex, int areaIndex) {
			StringBuilder builder = new StringBuilder();
			while(visibleIndex != -1) {
				builder.Append(GetFieldValueMember(isColumn, visibleIndex, areaIndex));
				visibleIndex = Data.GetParentIndex(isColumn, visibleIndex);
			}
			return builder.ToString();
		}
		string GetFieldValueMember(bool isColumn, int visibleIndex, int areaIndex) {
			if(Data.IsOLAP)
				return Data.OLAPDataSource.GetMember(isColumn, visibleIndex).UniqueName;
			object fieldValue = Data.PivotDataSource.GetFieldValue(isColumn, visibleIndex, areaIndex);
			return fieldValue != null ? fieldValue.ToString() : String.Empty;
		}
	}
}
