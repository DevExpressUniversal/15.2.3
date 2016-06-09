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

using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.PivotGrid;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGroupItem : IThreadSafeGroup {
		int index;
		int visibleCount;
		PivotArea area;
		string caption;
		string hierarchy;
		bool visible;
		bool filterValuesHasFilter;
		string toString;
		PivotFieldItemCollection fields;
		public PivotGroupItem(PivotGridGroup group, PivotFieldItemCollection fieldItems) {
			this.index = group.Index;
			this.visibleCount = group.VisibleCount;
			this.area = group.Area;
			this.caption = group.Caption;
			this.hierarchy = group.Hierarchy;
			this.visible = group.Visible;
			this.filterValuesHasFilter = group.FilterValues.HasFilter;
			this.toString = group.ToString();
			this.fields = new PivotFieldItemCollection();
			foreach(PivotGridFieldBase field in group.Fields) {
				if(field.Index < 0) continue;
				this.fields.Add(fieldItems.GetFieldItem(field));
			}
		}
		public int Index {
			get { return index; }
		}
		public int VisibleCount {
			get { return visibleCount; }
		}
		public PivotArea Area {
			get { return area; }
		}
		public string Caption {
			get { return caption; }
		}
		public string Hierarchy {
			get { return hierarchy; }
		}
		public bool Visible {
			get { return visible; }
		}
		public bool FilterValuesHasFilter {
			get { return filterValuesHasFilter; }
		}
		public override string ToString() {
			return toString;
		}
		public PivotFieldItemCollection Fields {
			get { return fields; }
		}
		public List<PivotFieldItemBase> GetVisibleFields() {
			List<PivotFieldItemBase> res = new List<PivotFieldItemBase>();
			foreach(PivotFieldItemBase fieldItem in Fields) {
				if(!fieldItem.ExpandedInFieldsGroup || !fieldItem.Visible)
					break;
				res.Add(fieldItem);
			}
			return res;
		}
		#region IThreadSafeGroup Members
		int IThreadSafeGroup.Index { get { return Index; } }
		PivotArea IThreadSafeGroup.Area { get { return Area; } }
		string IThreadSafeGroup.Caption { get { return Caption; } }
		string IThreadSafeGroup.Hierarhcy { get { return Hierarchy; } }
		bool IThreadSafeGroup.Visible { get { return Visible; } }
		int IThreadSafeGroup.VisibleCount { get { return VisibleCount; } }
		IThreadSafeFieldCollection IThreadSafeGroup.Fields { get { return Fields as IThreadSafeFieldCollection; } }
		#endregion
	}
	public class PivotGroupItemCollection : Collection<PivotGroupItem>, IThreadSafeGroupCollection {
		public void Populate(PivotGridGroupCollection groups, PivotFieldItemCollection fieldItems) {
			foreach(PivotGridGroup group in groups)
				Add(new PivotGroupItem(group, fieldItems));
		}
		#region IThreadSafeGroupCollection Members
		IThreadSafeGroup IThreadSafeGroupCollection.this[int index] {
			get { return this[index] as IThreadSafeGroup; }
		}
		int IThreadSafeGroupCollection.Count { get { return Count; } }
		#endregion
	}
	public class PivotFieldItemBase : IThreadSafeField {
		bool isRowTreeFieldItem;
		string fieldName;
		string name;
		bool visible;
		PivotSummaryType summaryType;
		PivotArea area;
		int areaIndex;
		int index;
		Type fieldType;
		PivotSummaryDisplayType summaryDisplayType;
		PivotKPIGraphic fKPIGraphic;
		PivotKPIType fKPIType;
#if !SL
		bool selectedAtDesignTime;
#endif
		bool isPercentageCalculation;
		bool isIntegerCalculation;
		bool isNonCurrencyDecimalCalculation;
		string emptyCellText;
		string emptyValueText;
		FormatInfo valueFormat;
		FormatInfo totalValueFormat;
		FormatInfo cellFormat;
		FormatInfo totalCellFormat;
		FormatInfo grandTotalCellFormat;
		PivotGridFieldOptions options;
		PivotGridCustomTotalCollectionBase customTotals;
		PivotTotalsVisibility totalsVisibility;
		int totalsSummaryCountTrue, totalsSummaryCountFalse;
		int width;
		int minWidth;
		int groupIntervalNumericRange;
		int groupIntervalColumnHandle;
		PivotGroupInterval groupInterval;
		PivotTopValueType topValueType;
		int topValueCount;
		bool topValueShowOthers;	
		bool isDataField;
		bool isColumn;
		bool isColumnOrRow;
		int columnValueLineCount;
		int rowValueLineCount;
		bool canSortBySummary;
		PivotGridFieldSortBySummaryInfo sortBySummaryInfo;
		string headerDisplayText;
		int filterValuesCount;
		bool isNextVisibleFieldInSameGroup;
		PivotSortOrder sortOrder;
		bool filterValuesHasFilter;
		bool filterValuesIsEmpty;
		bool expandedInFieldsGroup;
		bool showSortButton;
		bool showFilterButton;
		bool canHide;
		bool canDrag;
		bool showCollapsedButton;
		bool canSortHeader;
		bool canSortCore;
		bool isOLAPSortModeNone;
		bool runningTotal;
		bool showNewValues;
		DefaultBoolean useNativeFormat;
		bool isFirstFieldInGroup;
		bool canShowInCustomizationForm;
		string toString;
		bool[] isAreaAllowed;
		bool canDragInCustomizationForm;
		bool showActiveFilterButton;
		bool isFieldVisibleInGroup;
		int groupIndex;
		int innerGroupIndex;
		PivotGroupItemCollection groupItems;
		bool isOLAPSorted;
		bool canSortOLAP;
		PivotGridAllowedAreas allowedAreas;
		bool isUnbound;
		PivotGroupFilterMode groupFilterMode;
		Type dataType;
		UnboundColumnType unboundType;
		string expressionFieldName;
		string dataControllerColumnName;
		string prefilterColumnName;
		string hierarchy;
		string caption;
		string unboundFieldName;
		string unboundExpression;
		string uniqueName;
		string displayFolder;
		PivotSortMode actualSortMode;
		PivotSortMode sortMode;
		object tag;
		string[] autoPopulatedProperties;
		string sortByAttribute;
		public PivotFieldItemBase(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase field) {
			this.isRowTreeFieldItem = field.IsRowTreeField;
			this.fieldName = field.FieldName;
			this.name = field.Name;
			this.visible = field.Visible;
			this.area = field.Area;
			this.areaIndex = field.AreaIndex;
			this.index = field.Index;
			this.toString = field.ToString();
			this.width = field.Width;
			this.minWidth = field.MinWidth;
			if(IsRowTreeFieldItem)
				return;
			this.summaryType = field.SummaryType;
			this.fieldType = data.GetFieldType(field);
			this.fKPIGraphic = data.GetKPIGraphic(field);
			this.fKPIType = field.KPIType;
			this.emptyCellText = field.EmptyCellText;
			this.emptyValueText = field.EmptyValueText;
			this.valueFormat = field.ValueFormat;
			this.totalValueFormat = field.TotalValueFormat;
			this.cellFormat = field.CellFormat;
			this.totalCellFormat = field.TotalCellFormat;
			this.grandTotalCellFormat = field.GrandTotalCellFormat;
			this.summaryDisplayType = field.SummaryDisplayType;
			PivotCalculationBase actualCalcualtion = data.CellValuesProvider.GetLastCalculation(field);
			this.isPercentageCalculation = (actualCalcualtion is PivotPercentageCalculationBase) || (actualCalcualtion is PivotVariationCalculationBase && (actualCalcualtion as PivotVariationCalculationBase).VariationType == PivotVariationCalculationType.Percent);
			this.isIntegerCalculation = actualCalcualtion is PivotRankCalculationBase;
			this.isNonCurrencyDecimalCalculation = actualCalcualtion is PivotIndexCalculationBase;
			this.options = field.Options;
			this.customTotals = field.CustomTotals;
			this.totalsVisibility = field.GetTotalsVisibility();
			this.totalsSummaryCountTrue = field.GetTotalSummaryCount(true);
			this.totalsSummaryCountFalse = field.GetTotalSummaryCount(false);
			this.groupIntervalNumericRange = field.GroupIntervalNumericRange;
			this.groupIntervalColumnHandle = field.GroupIntervalColumnHandle;
			this.groupInterval = field.GroupInterval;
			this.topValueType = field.TopValueType;
			this.topValueCount = field.TopValueCount;
			this.topValueShowOthers = field.TopValueShowOthers;
			this.isDataField = field.IsDataField;
			this.isColumn = field.IsColumn;
			this.isColumnOrRow = field.IsColumnOrRow;
			this.columnValueLineCount = field.ColumnValueLineCount;
			this.rowValueLineCount = field.RowValueLineCount;
			this.canSortBySummary = field.CanSortBySummary;
			this.sortBySummaryInfo = field.SortBySummaryInfo;
			this.headerDisplayText = field.HeaderDisplayText;
			this.filterValuesCount = field.FilterValues.Count;
			this.isNextVisibleFieldInSameGroup = field.IsNextVisibleFieldInSameGroup;
			this.sortOrder = field.SortOrder;
			this.filterValuesHasFilter = field.FilterValues.HasFilter;
			this.filterValuesIsEmpty = field.FilterValues.IsEmpty;
			this.expandedInFieldsGroup = field.ExpandedInFieldsGroup;
			this.showSortButton = field.ShowSortButton;
			this.showFilterButton = field.ShowFilterButton;
			this.canHide = field.CanHide;
			this.canDrag = field.CanDrag;
			this.showCollapsedButton = field.Group != null && field.Group.CanExpandField(field);
#if !SL
			this.selectedAtDesignTime = field.SelectedAtDesignTime;
#endif
			this.canSortHeader = field.CanSortCore || !field.SortBySummaryInfo.IsEmpty;
			this.canSortCore = field.CanSortCore;
			this.isOLAPSortModeNone = field.IsOLAPSortModeNone;
			this.runningTotal = field.RunningTotal;
			this.showNewValues = field.ShowNewValues;
			this.useNativeFormat = field.UseNativeFormat;
			this.isFirstFieldInGroup = field.Group != null && field.Group.Fields[0] == field;
			this.canShowInCustomizationForm = field.CanShowInCustomizationForm;
			this.isAreaAllowed = new bool[4];
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea)))
				this.isAreaAllowed[(int)area] = field.IsAreaAllowed(area);
			this.canDragInCustomizationForm  = field.CanDragInCustomizationForm;
			this.showActiveFilterButton = field.ShowActiveFilterButton;
			this.isFieldVisibleInGroup = field.Group == null ? false : field.Group.IsFieldVisible(field);
			this.groupIndex = field.GroupIndex;
			this.innerGroupIndex = field.InnerGroupIndex;
			this.groupItems = groupItems;
			this.isOLAPSorted = field.IsOLAPSorted;
			this.canSortOLAP = field.CanSortOLAP;
			this.allowedAreas = field.AllowedAreas;
			this.groupFilterMode = field.GroupFilterMode;
			this.isUnbound = field.IsUnbound;
			this.dataType = field.DataType;
			this.expressionFieldName = field.ExpressionFieldName;
			this.dataControllerColumnName = field.DataControllerColumnName;
			this.prefilterColumnName = field.PrefilterColumnName;
			this.hierarchy = field.Hierarchy;
			this.caption = field.Caption;
			this.displayFolder = field.DisplayFolder;
			this.unboundFieldName = field.UnboundFieldName;
			this.unboundExpression = field.UnboundExpression;
			this.uniqueName = field.UniqueName;
			this.unboundType = field.UnboundType;
			this.actualSortMode = field.ActualSortMode;
			this.sortMode = field.SortMode;
			this.tag = field.Tag;
			this.autoPopulatedProperties = field.AutoPopulatedProperties;
			this.sortByAttribute = field.SortByAttribute;
		}
		public override string ToString() {
			return toString;
		}
		public override bool Equals(object obj) {
			PivotFieldItemBase field = obj as PivotFieldItemBase;
			if(field == null)
				return false;
			if(Index < 0)
				return IsDataField && field.IsDataField || IsRowTreeFieldItem && field.IsRowTreeFieldItem;
			return Index == field.Index;
		}
		public string FieldName {
			get { return fieldName; }
		}
		public string Name {
			get { return name; }
		}
		public bool Visible {
			get { return visible; }
		}
		public PivotArea Area {
			get { return area; }
		}
		public int AreaIndex {
			get { return areaIndex; }
		}
		public int Index {
			get { return index; }
		}
		public PivotSummaryType SummaryType {
			get { return summaryType; }
		}
		public Type FieldType {
			get { return fieldType; }
		}
		public PivotSummaryDisplayType SummaryDisplayType {
			get { return summaryDisplayType; }
		}
		public bool IsPercentageCalculation {
			get { return isPercentageCalculation; }
		}
		public bool IsIntegerCalculation {
			get { return isIntegerCalculation; }
		}
		public bool IsNonCurrencyDecimalCalculation {
			get { return isNonCurrencyDecimalCalculation; }
		}
		public PivotKPIGraphic KPIGraphic {
			get { return fKPIGraphic; }
		}
		public string EmptyCellText {
			get { return emptyCellText; }
		}
		public string EmptyValueText {
			get { return emptyValueText; }
		}
		public FormatInfo CellFormat {
			get { return cellFormat; }
		}
		public FormatInfo TotalCellFormat {
			get { return totalCellFormat; }
		}
		public FormatInfo GrandTotalCellFormat {
			get { return grandTotalCellFormat; }
		}
		public PivotGridFieldOptions Options {
			get { return options; }
		}
		public PivotGridCustomTotalCollectionBase CustomTotals {
			get { return customTotals; }
		}
		public PivotTotalsVisibility TotalsVisibility {
			get { return totalsVisibility; }
		}
		public int GetTotalSummaryCount(bool singleValue) {
			return singleValue ? totalsSummaryCountTrue : totalsSummaryCountFalse;
		}
		public int Width {
			get { return width; }
			internal set { width = value; } 
		}
		public int MinWidth {
			get { return minWidth; }
		}
		public int GroupIntervalNumericRange {
			get { return groupIntervalNumericRange; }
		}
		public int GroupIntervalColumnHandle {
			get { return groupIntervalColumnHandle; }
		}
		public PivotGroupInterval GroupInterval {
			get { return groupInterval; }
		}
		public PivotTopValueType TopValueType {
			get { return topValueType; }
		}
		public int TopValueCount {
			get { return topValueCount; }
		}
		public bool TopValueShowOthers {
			get { return topValueShowOthers; }
		}
		public PivotKPIType KPIType {
			get { return fKPIType; }
		}
		public bool IsDataField {
			get { return isDataField; }
		}
		public bool IsColumn {
			get { return isColumn; }
		}
		public bool IsColumnOrRow {
			get { return isColumnOrRow; }
		}
		public int ColumnValueLineCount {
			get { return columnValueLineCount; }
		}
		public int RowValueLineCount {
			get { return rowValueLineCount; }
		}
		public bool CanSortBySummary {
			get { return canSortBySummary; }
		}
		public PivotGridFieldSortBySummaryInfo SortBySummaryInfo {
			get { return sortBySummaryInfo; }
		}
		public string HeaderDisplayText {
			get { return headerDisplayText; }
		}
		PivotGroupItemCollection GroupItems {
			get { return groupItems; }
		}
		public PivotGroupItem Group {
			get { return GroupIndex < 0 || GroupItems == null ? null : GroupItems[groupIndex]; }
		}
		public int FilterValuesCount {
			get { return filterValuesCount; }
		}
		public bool IsNextVisibleFieldInSameGroup {
			get { return isNextVisibleFieldInSameGroup; }
		}
		public PivotSortOrder SortOrder {
			get { return sortOrder; }
		}
		public bool FilterValuesHasFilter {
			get { return filterValuesHasFilter; }
		}
		public bool ExpandedInFieldsGroup {
			get { return expandedInFieldsGroup; }
		}
		public bool ShowSortButton {
			get { return showSortButton; }
		}
		public bool ShowFilterButton {
			get { return showFilterButton; }
		}
		public bool ShowActiveFilterButton {
			get { return showActiveFilterButton; }
		}
		public bool CanHide {
			get { return canHide; }
		}
		public bool CanDrag {
			get { return canDrag; }
		}
		public bool ShowCollapsedButton {
			get { return showCollapsedButton; }
		}
#if !SL
		public bool SelectedAtDesignTime {
			get { return selectedAtDesignTime; }
			internal set { selectedAtDesignTime = value; }
		}
#endif
		public bool CanSortHeader {
			get { return canSortHeader; }
		}
		public bool CanSortCore {
			get { return canSortCore; }
		}
		public bool IsOLAPSortModeNone {
			get { return isOLAPSortModeNone; }
		}
		public bool RunningTotal {
			get { return runningTotal; }
		}
		public bool ShowNewValues {
			get { return showNewValues; }
		}
		public FormatInfo GetValueFormat(PivotGridValueType valueType) {
			if(valueType == PivotGridValueType.Total)
				return !TotalValueFormat.IsEmpty ? TotalValueFormat : PivotGridFieldBase.defaultTotalFormat;
			if(valueType == PivotGridValueType.Value)
				return ValueFormat;
			return null;
		}
		public FormatInfo ValueFormat {
			get { return valueFormat; }
		}
		public FormatInfo TotalValueFormat {
			get { return totalValueFormat; }
		}
		public DefaultBoolean UseNativeFormat {
			get { return useNativeFormat; }
		}
		public bool IsFirstFieldInGroup {
			get { return isFirstFieldInGroup; }
		}
		public bool CanShowInCustomizationForm {
			get { return canShowInCustomizationForm; }
		}
		public bool IsAreaAllowed(PivotArea area) {
			return isAreaAllowed[(int)area];
		}
		public bool CanDragInCustomizationForm {
			get { return canDragInCustomizationForm; }
		}
		internal bool IsRowTreeFieldItem {
			get { return isRowTreeFieldItem; }
		}
		public bool IsFieldVisibleInGroup {
			get { return isFieldVisibleInGroup; }
		}
		public int GroupIndex {
			get { return groupIndex; }
		}
		public int InnerGroupIndex {
			get { return innerGroupIndex; }
		}
		public bool IsOLAPSorted {
			get { return isOLAPSorted; }
		}
		public bool CanSortOLAP { 
			get { return canSortOLAP; } 
		}
		public PivotGridAllowedAreas AllowedAreas {
			get { return allowedAreas; }
		}
		public PivotGroupFilterMode GroupFilterMode {
			get { return groupFilterMode; }
		}
		public bool IsUnbound {
			get { return isUnbound; }
		}
		public Type DataType {
			get { return dataType; }
		}
		public string ExpressionFieldName {
			get { return expressionFieldName; }
		}
		public string DataControllerColumnName {
			get { return dataControllerColumnName; }
		}
		public string PrefilterColumnName {
			get { return prefilterColumnName; }
		}
		public string Hierarchy {
			get { return hierarchy; }
		}
		public string Caption {
			get { return caption; }
		}
		public string UnboundExpression {
			get { return unboundExpression; }
		}
		public string UnboundFieldName {
			get { return unboundFieldName; }
		}
		public string UniqueName {
			get { return uniqueName; }
		}
		public string DisplayFolder {
			get { return displayFolder; }
		}
		public UnboundColumnType UnboundType {
			get { return unboundType; }
		}
		public bool FilterValuesIsEmpty {
			get { return filterValuesIsEmpty; }
		}
		public PivotSortMode ActualSortMode {
			get { return actualSortMode; }
		}
		public PivotSortMode SortMode {
			get { return sortMode; }
		}
		public object Tag {
			get { return tag; }
		}
		public override int GetHashCode() {
			if(Index < 0) {
				if(isDataField)
					return -1;
				if(isRowTreeFieldItem)
					return -2;
			}
			return Index;
		}
		public string[] AutoPopulatedProperties {
			get { return autoPopulatedProperties; }
		}
		public string SortByAttribute {
			get { return sortByAttribute; }
		}
		#region IThreadSafeField Members
		PivotArea IThreadSafeField.Area { get { return Area; } }
		int IThreadSafeField.AreaIndex { get{return AreaIndex;} }
		bool IThreadSafeField.Visible { get { return Visible; } }
		string IThreadSafeField.Name { get { return Name; } }
		string IThreadSafeField.FieldName { get { return FieldName; } }
		string IThreadSafeField.UnboundFieldName { get { return UnboundFieldName; } }
		string IThreadSafeField.PrefilterColumnName { get { return PrefilterColumnName; } }
		string IThreadSafeField.Caption { get { return Caption; } }
		PivotSummaryType IThreadSafeField.SummaryType { get { return SummaryType; } }
		PivotSortOrder IThreadSafeField.SortOrder { get { return SortOrder; } }
		PivotSortMode IThreadSafeField.SortMode { get { return SortMode; } }
		PivotGridAllowedAreas IThreadSafeField.AllowedAreas { get { return AllowedAreas; } }
		int IThreadSafeField.GroupIndex { get { return GroupIndex; } }
		IThreadSafeGroup IThreadSafeField.Group { get { return Group as IThreadSafeGroup; } }
		bool IThreadSafeField.TopValueShowOthers { get { return TopValueShowOthers; } }
		int IThreadSafeField.TopValueCount { get { return TopValueCount; } }
		bool IThreadSafeField.RunningTotal { get { return RunningTotal; } }
		PivotTopValueType IThreadSafeField.TopValueType { get { return TopValueType; } }
		PivotGroupInterval IThreadSafeField.GroupInterval { get { return GroupInterval; } }
		int IThreadSafeField.GroupIntervalNumericRange { get { return GroupIntervalNumericRange; } }
		string IThreadSafeField.UnboundExpression { get { return UnboundExpression; } }
		bool IThreadSafeField.ExpandedInFieldsGroup { get { return ExpandedInFieldsGroup; } }
		UnboundColumnType IThreadSafeField.UnboundType { get { return UnboundType; } }
		Type IThreadSafeField.DataType { get { return DataType; } }
		object IThreadSafeField.Tag { get { return Tag; } }
		#endregion
	}
	public class PivotFieldItemCollection : Collection<PivotFieldItemBase>, IThreadSafeFieldCollection {
		PivotFieldItemBase rowTreeFieldItem;
		PivotFieldItemBase dataFieldItem;
		Dictionary<PivotArea, List<PivotFieldItemBase>> itemsByAreaWithDataField;
		Dictionary<PivotArea, List<PivotFieldItemBase>> itemsByAreaWithoutDataField;
		Dictionary<int, PivotFieldItemBase> columnItemsByLevel;
		Dictionary<int, PivotFieldItemBase> rowItemsByLevel;
		PivotGroupItemCollection groupItems;
		int dataFieldCount;
		int rowFieldCount;
		int columnFieldCount;
		public PivotFieldItemCollection() {
			groupItems = new PivotGroupItemCollection();
		}
		public void RePopulate(PivotGridData data, PivotVisualItemsBase visualItems) {
			RePopulateFields(data, visualItems);
			RePopulateHashes(data);
		}
		void RePopulateFields(PivotGridData data, PivotVisualItemsBase visualItems) {
			data.BeginUpdate();
			data.EnsureFieldCollections();
			GroupItems.Clear();
			dataFieldCount = data.DataFieldCount;
			rowFieldCount = data.RowFieldCount;
			columnFieldCount = data.ColumnFieldCount;
			this.Clear();
			foreach(PivotGridFieldBase field in data.Fields)
				this.Add(visualItems.CreateFieldItem(data, GroupItems, field));
			rowTreeFieldItem = visualItems.CreateFieldItem(data, GroupItems, visualItems.RowTreeField);
			dataFieldItem = visualItems.CreateFieldItem(data, GroupItems, data.DataField);
			GroupItems.Populate(data.Groups, this);
			data.CancelUpdate();
		}
		void RePopulateHashes(PivotGridData data){
			itemsByAreaWithDataField = CreateItemsByAreaHash(data, true);
			itemsByAreaWithoutDataField = CreateItemsByAreaHash(data, false);
			columnItemsByLevel = CreateItemsByLevelHash(data, true);
			rowItemsByLevel = CreateItemsByLevelHash(data, false);
		}
		Dictionary<int, PivotFieldItemBase> CreateItemsByLevelHash(PivotGridData data, bool isColumn) {
			Dictionary<int, PivotFieldItemBase> items = new Dictionary<int, PivotFieldItemBase>();
			List<PivotGridFieldBase> fields = data.GetFieldsByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea, false);
			PivotDataArea area = data.OptionsDataField.DataFieldsLocationArea;
			if(area != PivotDataArea.None) {
				if((isColumn && area == PivotDataArea.ColumnArea) || (!isColumn && area == PivotDataArea.RowArea)) {
					if(data.DataField.Visible)
						fields.Insert(data.DataField.AreaIndex, data.DataField);
					else
						fields.Add(data.DataField);
				}
			} else if(isColumn && fields.Count == 0) {
				fields.Add(data.DataField);
			}
			int levelCount = fields.Count;
			for(int level = 0; level < levelCount; level++)
				items.Add(level, GetFieldItem(fields[level]));
			return items;
		}
		Dictionary<PivotArea, List<PivotFieldItemBase>> CreateItemsByAreaHash(PivotGridData data, bool includeDataField) {
			Dictionary<PivotArea, List<PivotFieldItemBase>>  itemsByArea = new Dictionary<PivotArea, List<PivotFieldItemBase>>();
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea)))
				itemsByArea.Add(area, ToItems(data.GetFieldsByArea(area, includeDataField)));
			return itemsByArea;
		}
		List<PivotFieldItemBase> ToItems(List<PivotGridFieldBase> fields) {
			List<PivotFieldItemBase> items = new List<PivotFieldItemBase>();
			fields.ForEach(delegate(PivotGridFieldBase field) {
				items.Add(GetFieldItem(field));
			});
			return items;
		}
		public PivotGroupItemCollection GroupItems {
			get { return groupItems; }
		}
		public PivotFieldItemBase GetFieldItem(PivotGridFieldBase field) {
			int index = field.Index;
			if(index < 0) {
				if(field.IsDataField)
					return DataFieldItem;
				if(field.IsRowTreeField)
					return RowTreeFieldItem;
				throw new Exception("Incorrect field index");
			}
			return this[index];
		}
		public PivotFieldItemBase RowTreeFieldItem {
			get { return rowTreeFieldItem; }
		}
		public PivotFieldItemBase DataFieldItem {
			get { return dataFieldItem; }
		}
		Dictionary<PivotArea, List<PivotFieldItemBase>> GetItemsByAreaHash(bool includeDataField) {
			return includeDataField ? itemsByAreaWithDataField : itemsByAreaWithoutDataField;			
		}
		Dictionary<int, PivotFieldItemBase> GetItemsByLevelHash(bool isColumn) {
			return isColumn ? columnItemsByLevel : rowItemsByLevel;
		}
		public PivotFieldItemBase GetFieldItemByArea(PivotArea area, int index) {
			List<PivotFieldItemBase> items = GetItemsByAreaHash(false)[area];
			if(index >=0 && index <items.Count)
				return items[index];
			return null;
		}
		public int GetFieldCountByArea(PivotArea area) {
			return GetItemsByAreaHash(false)[area].Count;
		}
		public int DataFieldCount {
			get { return dataFieldCount; }
		}
		public int ColumnFieldCount {
			get { return columnFieldCount; }
		}
		public int RowFieldCount {
			get { return rowFieldCount; }
		}
		public List<PivotFieldItemBase> GetFieldItemsByArea(PivotArea area, bool includeDataField) {
			Dictionary<PivotArea, List<PivotFieldItemBase>> areaItems = GetItemsByAreaHash(includeDataField);
			if(areaItems == null) return new List<PivotFieldItemBase>(0);
			return new List<PivotFieldItemBase>(areaItems[area].ToArray());
		}
		public PivotFieldItemBase GetFieldItemByLevel(bool isColumn, int level) {
			if(level < 0 || level >= GetLevelCount(isColumn))
				return null;
			return GetItemsByLevelHash(isColumn)[level];
		}
		public int GetLevelCount(bool isColumn) {
			return GetItemsByLevelHash(isColumn).Count;
		}
		public new PivotFieldItemBase this[int index] {
			get {
				if(index < 0)
					return DataFieldItem;
				return base[index];
			}
		}
		public PivotFieldItemBase this[string fieldName] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].FieldName == fieldName)
						return this[i];
				}
				for(int i = 0; i < Count; i++) {
					if(this[i].ExpressionFieldName == fieldName)
						return this[i];
					if(this[i].DataControllerColumnName == fieldName)
						return this[i];
				}
				return null;
			}
		}
		#region IThreadSafeFieldCollection Members
		IThreadSafeField IThreadSafeFieldCollection.this[int index] {
			get { return this[index] as IThreadSafeField; }
		}
		IThreadSafeField IThreadSafeFieldCollection.this[string fieldName] {
			get { return this[fieldName] as IThreadSafeField; }
		}
		int IThreadSafeFieldCollection.Count { get { return Count; } }
		int IThreadSafeFieldCollection.GetVisibleFieldCount(PivotArea area) {
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].Visible && this[i].Area == area)
					count++;
			}
			return count;
		}
		#endregion
	}
	public class PivotGridFieldPair {
		public static PivotGridFieldPair LoadPair(PivotGridData data, TypedBinaryReader reader) {
			return new PivotGridFieldPair(data, reader);
		}
		PivotFieldItemBase fieldItem, dataFieldItem;
		PivotGridData data;
		protected PivotGridFieldPair(PivotGridData data) {
			this.data = data;
		}
		public PivotGridFieldPair(PivotGridData data, PivotFieldItemBase fieldItem, PivotFieldItemBase dataFieldItem)
			: this(data) {
			this.fieldItem = fieldItem;
			this.dataFieldItem = dataFieldItem;
		}
		public PivotGridFieldPair(PivotGridData data, TypedBinaryReader reader)
			: this(data) {
			LoadFromStream(reader, data.FieldItems);
		}
		PivotGridData Data { get { return data; } }
		public PivotFieldItemBase FieldItem { get { return fieldItem; } }
		public PivotFieldItemBase DataFieldItem { get { return dataFieldItem; } }
		public PivotGridFieldBase Field { get { return Data != null ? Data.GetField(FieldItem) : null; } }
		public PivotGridFieldBase DataField { get { return Data != null ? Data.GetField(DataFieldItem) : null; } }
		public void SaveToStream(TypedBinaryWriter writer) {
			SaveFieldToStream(writer, fieldItem);
			SaveFieldToStream(writer, dataFieldItem);
		}
		public void LoadFromStream(TypedBinaryReader reader, PivotFieldItemCollection fields) {
			fieldItem = LoadFieldFromStream(reader, fields);
			dataFieldItem = LoadFieldFromStream(reader, fields);
		}
		void SaveFieldToStream(TypedBinaryWriter writer, PivotFieldItemBase field) {
			writer.Write(field != null ? field.Index : -1);
		}
		PivotFieldItemBase LoadFieldFromStream(TypedBinaryReader reader, PivotFieldItemCollection fields) {
			int index = reader.ReadInt32();
			return index >= 0 ? fields[index] : null;
		}
		public override bool Equals(object obj) {
			PivotGridFieldPair source = obj as PivotGridFieldPair;
			if(source == null) return false;
			return object.Equals(FieldItem, source.FieldItem) && object.Equals(DataFieldItem, source.DataFieldItem);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class PivotGridUIActionBase {
		PivotGridData data;
		public PivotGridUIActionBase(PivotGridData data) {
			this.data = data;
		}
		protected PivotGridData Data {
			get { return data; }
		}
	}
	public abstract class PivotGridFieldUIActionBase : PivotGridUIActionBase {
		PivotFieldItemBase fd;
		public PivotGridFieldUIActionBase(PivotFieldItemBase fd, PivotGridData data)
			: base(data) {
			this.fd = fd;
		}
		protected PivotFieldItemBase FieldItem {
			get { return fd; }
		}
		protected PivotGridFieldBase Field {
			get { return Data.GetField(FieldItem); }
		}
	}
	public class PivotGridFieldUISetWidthAction : PivotGridFieldUIActionBase {
		public PivotGridFieldUISetWidthAction(PivotFieldItemBase fd, PivotGridData data)
			: base(fd, data) {
		}
		public void SetWidth(int width) {
			FieldItem.Width = width;
			Field.Width = width;
		}
	}
	public class PivotGridFieldUISelectedAtDesignTimeAction : PivotGridFieldUIActionBase {
		public PivotGridFieldUISelectedAtDesignTimeAction(PivotFieldItemBase fd, PivotGridData data)
			: base(fd, data) {
		}
		public void SetSelectedAtDesignTimeAction(bool selectedAtDesignTime) {
			FieldItem.SelectedAtDesignTime = selectedAtDesignTime;
		}
	}
}
