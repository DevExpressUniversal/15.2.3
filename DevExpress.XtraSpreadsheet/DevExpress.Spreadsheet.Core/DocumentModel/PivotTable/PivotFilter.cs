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

using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotFilter
	public class PivotFilter {
		#region Static
		public static bool GetIsMeasureFilter(PivotFilterType filterType) {
			return (filterType >= PivotFilterType.Count && filterType <= PivotFilterType.Sum) ||
				(filterType >= PivotFilterType.ValueEqual && filterType <= PivotFilterType.ValueNotBetween);
		}
		#endregion
		#region Fields
		int? measureFieldIndex;
		int? measureIndex;
		int? memberPropertyFieldId;
		int fieldIndex;
		int pivotFilterId;
		int evalOrder;
		string description;
		string name;
		string labelPivot;
		string labelPivotFilter;
		PivotFilterType filterType;
		readonly DocumentModel documentModel;
		PivotAutoFilter autoFilter;
		#endregion
		#region Constructors
		public PivotFilter(DocumentModel documentModel) {
			this.documentModel = documentModel;
			evalOrder = 0;
			autoFilter = new PivotAutoFilter(DocumentModel.ActiveSheet);
		}
		#endregion
		#region Properties
		DocumentModel DocumentModel { get { return documentModel; } }
		public PivotAutoFilter AutoFilter { get { return autoFilter; } }
		public PivotFilterType FilterType { get { return filterType; } set { SetFilterType(value); } }
		public int? MeasureFieldIndex { get { return measureFieldIndex; } set { SetMeasureFieldIndex(value); } }
		public int? MeasureIndex { get { return measureIndex; } set { SetMeasureIndex(value); } }
		public int? MemberPropertyFieldId { get { return memberPropertyFieldId; } set { SetMemberPropertyFieldId(value); } }
		public int FieldIndex { get { return fieldIndex; } set { SetFieldIndex(value); } }
		public int PivotFilterId { get { return pivotFilterId; } set { SetPivotFilterId(value); } }
		public int EvalOrder { get { return evalOrder; } set { SetEvalOrder(value); } }
		public string Description { get { return description; } set { SetDescription(value); } }
		public string Name { get { return name; } set { SetName(value); } }
		public string LabelPivot { get { return labelPivot; } set { SetLabelPivot(value); } }
		public string LabelPivotFilter { get { return labelPivotFilter; } set { SetLabelPivotFilter(value); } }
		public bool IsMeasureFilter { get { return GetIsMeasureFilter(filterType); } }
		public bool IsLabelFilter { get { return !IsMeasureFilter; } }
		public bool IsTop10Filter { get { return filterType == PivotFilterType.Sum || filterType == PivotFilterType.Percent || filterType == PivotFilterType.Count; } }
		#endregion
		bool CheckString(string oldValue, string newValue) {
			if (oldValue == null && newValue == null)
				return false;
			if (oldValue != null && newValue != null)
				if (string.Compare(oldValue, newValue, false) == 0)
					return false;
			return true;
		}
		void SetFilterType(PivotFilterType value) {
			if (FilterType != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, (int)FilterType, (int)value, SetFilterTypeCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetMeasureFieldIndex(int? value) {
			if (MeasureFieldIndex != value) {
				ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, MeasureFieldIndex, value, SetMeasureFieldIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetMeasureIndex(int? value) {
			if (MeasureIndex != value) {
				ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, MeasureIndex, value, SetMeasureIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetMemberPropertyFieldId(int? value) {
			if (MemberPropertyFieldId != value) {
				ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, MemberPropertyFieldId, value, SetMemberPropertyFieldIdCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetFieldIndex(int value) {
			if (FieldIndex != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, FieldIndex, value, SetFieldIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetPivotFilterId(int value) {
			if (PivotFilterId != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, PivotFilterId, value, SetPivotFilterIdCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetEvalOrder(int value) {
			if (EvalOrder != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, EvalOrder, value, SetEvalOrderCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetDescription(string value) {
			if (CheckString(Description, value)) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, Description, value, SetDescriptionCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetName(string value) {
			if (CheckString(Name, value)) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, Name, value, SetNameCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetLabelPivot(string value) {
			if (CheckString(LabelPivot, value)) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, LabelPivot, value, SetLabelPivotCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetLabelPivotFilter(string value) {
			if (CheckString(LabelPivotFilter, value)) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, LabelPivotFilter, value, SetLabelPivotFilterCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetFilterTypeCore(int value) {
			filterType = (PivotFilterType)value;
		}
		protected internal void SetMeasureFieldIndexCore(int? value) {
			measureFieldIndex = value;
		}
		protected internal void SetMeasureIndexCore(int? value) {
			measureIndex = value;
		}
		protected internal void SetMemberPropertyFieldIdCore(int? value) {
			memberPropertyFieldId = value;
		}
		protected internal void SetFieldIndexCore(int value) {
			fieldIndex = value;
		}
		protected internal void SetPivotFilterIdCore(int value) {
			pivotFilterId = value;
		}
		protected internal void SetEvalOrderCore(int value) {
			evalOrder = value;
		}
		protected internal void SetDescriptionCore(string value) {
			description = value;
		}
		protected internal void SetNameCore(string value) {
			name = value;
		}
		protected internal void SetLabelPivotCore(string value) {
			labelPivot = value;
		}
		protected internal void SetLabelPivotFilterCore(string value) {
			labelPivotFilter = value;
		}
		public void CopyFrom(PivotFilter source) {
			this.description = source.description;
			this.evalOrder = source.evalOrder;
			this.fieldIndex = source.fieldIndex;
			this.filterType = source.filterType;
			this.labelPivot = source.labelPivot;
			this.labelPivotFilter = source.labelPivotFilter;
			this.measureFieldIndex = source.measureFieldIndex;
			this.measureIndex = source.measureIndex;
			this.memberPropertyFieldId = source.memberPropertyFieldId;
			this.name = source.name; 
			this.pivotFilterId = source.pivotFilterId;
			this.autoFilter.CopyFrom(source.autoFilter);
		}
		internal void ApplyFilter(PivotTable pivotTable, PivotTableFieldItemsVisibilityData visibilityData) {
			autoFilter.ReApplyFilter(pivotTable, FieldIndex, visibilityData);
		}
		internal void ApplyFilter(PivotTable pivotTable, AxisGrandTotalsLevel grandTotalsLevel) {
			int dataFieldIndex = MeasureFieldIndex.Value;
			PivotDataField dataField = pivotTable.DataFields[dataFieldIndex];
			autoFilter.ReApplyFilter(pivotTable, grandTotalsLevel, dataFieldIndex, dataField.Subtotal);
		}
		public void CopyFromNoHistory(PivotFilter source) {
			measureFieldIndex = source.measureFieldIndex;
			measureIndex = source.measureIndex;
			memberPropertyFieldId = source.memberPropertyFieldId;
			fieldIndex = source.fieldIndex;
			pivotFilterId = source.pivotFilterId;
			evalOrder = source.evalOrder;
			description = source.description;
			name = source.name;
			labelPivot = source.labelPivot;
			labelPivotFilter = source.labelPivotFilter;
			filterType = source.filterType;
			autoFilter.CopyFrom(source.autoFilter);
		}
	}
	#endregion
	#region PivotAutoFilter
	public class PivotAutoFilter : AutoFilterBase {
		public PivotAutoFilter(Worksheet sheet)
			: base(sheet, CellRange.Create(sheet, "A1")) {
		}
		public override IFilteringBehaviour FilteringBehaviour { get { return PivotFilteringBehaviour.Instance; } }
		public override CellRange Range {
			get { return base.Range; }
			set { throw new InvalidOperationException("The pivot filter range can not be changed"); } 
		}
		protected override void CopyFilterRange(CellRange sourceAutoFilterRange) {
		}
		#region ReApply
		internal void ReApplyFilter(PivotTable pivotTable, int fieldIndex, PivotTableFieldItemsVisibilityData visibilityData) {
			PivotLabelFilterValueProvider valueProvider = new PivotLabelFilterValueProvider(pivotTable, fieldIndex, visibilityData, Workbook.DataContext);
			UpdateDynamicFilters(valueProvider);
			ReApplyFilterCore(pivotTable, valueProvider);
		}
		internal void ReApplyFilter(PivotTable pivotTable, AxisGrandTotalsLevel grandTotalsLevel, int dataFieldIndex, PivotDataConsolidateFunction function) {
			PivotMeasureFilterValueProvider valueProvider = new PivotMeasureFilterValueProvider(grandTotalsLevel, function, dataFieldIndex, Workbook.DataContext);
			UpdateDynamicFilters(valueProvider);
			ReApplyFilterCore(pivotTable, valueProvider);
		}
		public override void ReApplyFilter() {
			throw new InvalidOperationException();
		}
		void ReApplyFilterCore(PivotTable pivotTable, IAutoFilterValueProvider valueProvider) {
			foreach (IPivotAutoFilterValue value in valueProvider.GetAutoFilterValuesEnumerable()) {
				if (!IsValueVisible(value))
					value.Visible = false;
			}
		}
		internal bool IsValueVisible(IAutoFilterValue autoFilterValue) {
			int count = this.FilterColumns.Count;
			for (int i = 0; i < count; i++) {
				if (!FilterColumns[i].IsValueVisible(autoFilterValue, FilteringBehaviour))
					return false;
			}
			return true;
		}
		void UpdateDynamicFilters(IAutoFilterValueProvider valueProvider) {
			int count = FilterColumns.Count;
			for (int i = 0; i < count; i++) {
				FilterColumns[i].Update(valueProvider, FilteringBehaviour);
			}
		}
		#endregion
	}
	#endregion
	#region PivotFilterType
	public enum PivotFilterType {
		CaptionEqual = 0x0004,
		CaptionNotEqual = 0x0005,
		CaptionBeginsWith = 0x0006,
		CaptionNotBeginsWith = 0x0007,
		CaptionEndsWith = 0x0008,
		CaptionNotEndsWith = 0x0009,
		CaptionContains = 0x000a,
		CaptionNotContains = 0x000b,
		CaptionGreaterThan = 0x000c,
		CaptionGreaterThanOrEqual = 0x000d,
		CaptionLessThan = 0x000e,
		CaptionLessThanOrEqual = 0x000f,
		CaptionBetween = 0x0010,
		CaptionNotBetween = 0x0011,
		DateEqual = 0x001a,
		DateNotEqual = 0x003e,
		DateOlderThan = 0x001b,
		DateOlderThanOrEqual = 0x003f,
		DateNewerThan = 0x001c,
		DateNewerThanOrEqual = 0x0040,
		DateBetween = 0x001d,
		DateNotBetween = 0x0041,
		LastWeek = 0x0023,
		LastMonth = 0x0026,
		LastQuarter = 0x0029,
		LastYear = 0x002c,
		January = 0x0032,
		February = 0x0033,
		March = 0x0034,
		April = 0x0035,
		May = 0x0036,
		June = 0x0037,
		July = 0x0038,
		August = 0x0039,
		September = 0x003a,
		October = 0x003b,
		November = 0x003c,
		December = 0x003d,
		NextWeek = 0x0021,
		NextMonth = 0x0024,
		NextQuarter = 0x0027,
		NextYear = 0x002a,
		FirstQuarter = 0x002e,
		SecondQuarter = 0x002f,
		ThirdQuarter = 0x0030,
		FourthQuarter = 0x0031,
		Tomorrow = 0x001e,
		Today = 0x001f,
		ThisWeek = 0x0022,
		ThisMonth = 0x0025,
		ThisQuarter = 0x0028,
		ThisYear = 0x002b,
		Unknown = 0x0000,
		Yesterday = 0x0020,
		YearToDate = 0x002d,
		Count = 0x0001,
		Percent = 0x0002,
		Sum = 0x0003,
		ValueEqual = 0x0012,
		ValueNotEqual = 0x0013,
		ValueGreaterThan = 0x0014,
		ValueGreaterThanOrEqual = 0x0015,
		ValueLessThan = 0x0016,
		ValueLessThanOrEqual = 0x0017,
		ValueBetween = 0x0018,
		ValueNotBetween = 0x0019,
	}
	#endregion
	#region PivotFilteringBehaviour
	public class PivotFilteringBehaviour : IFilteringBehaviour {
		static PivotFilteringBehaviour instance = new PivotFilteringBehaviour();
		public static PivotFilteringBehaviour Instance {
			get {
				return instance;
			}
		}
		PivotFilteringBehaviour() {
		}
		#region FilteringBehaviour Members
		public bool AllowsNumericAndWildcardComparison { get { return true; } }
		public bool AllowsStringComparison { get { return true; } }
		public bool DefaultTop10Behaviour { get { return false; } }
		#endregion
	}
	#endregion
	#region PivotMeasureFilterValueProvider
	public class PivotMeasureFilterValueProvider : IAutoFilterValueProvider {
		readonly AxisGrandTotalsLevel grandTotalsLevel;
		readonly PivotDataConsolidateFunction function;
		readonly int dataFieldIndex;
		readonly WorkbookDataContext dataContext;
		public PivotMeasureFilterValueProvider(AxisGrandTotalsLevel grandTotalsLevel, PivotDataConsolidateFunction function, int dataFieldIndex, WorkbookDataContext dataContext) {
			this.grandTotalsLevel = grandTotalsLevel;
			this.function = function;
			this.dataFieldIndex = dataFieldIndex;
			this.dataContext = dataContext;
		}
		#region IAutoFilterValueProvider Members
		public IEnumerable<VariantValue> GetValuesEnumerable() {
			foreach (PivotCellCalculationInfo cellValue in GetPivotCellCalculationInfoEnumerable()) {
				yield return cellValue.DataFieldInfos[dataFieldIndex].GetValue(function);
			}
		}
		public IVariantArray GetVariantArray() {
			VariantArray array = new VariantArray();
			List<VariantValue> values = new List<VariantValue>(GetValuesEnumerable());
			array.SetValues(values, values.Count, 1);
			return array;
		}
		public IEnumerable<IAutoFilterValue> GetAutoFilterValuesEnumerable() {
			foreach (PivotCellCalculationInfo cellValue in GetPivotCellCalculationInfoEnumerable()) {
				VariantValue variantValue = cellValue.DataFieldInfos[dataFieldIndex].GetValue(function);
				IAutoFilterValue autoFilterValue = new PivotMeasureFilterAutoFilterValue(variantValue, cellValue, dataContext);
				yield return autoFilterValue;
			}
		}
		public double GetNumericValuesCount() {
			int i = 0;
			IEnumerator<PivotCellCalculationInfo> enumerator = GetPivotCellCalculationInfoEnumerable().GetEnumerator();
			while (enumerator.MoveNext()) {
				i++;
			}
			return i;
		}
		#endregion
		IEnumerable<PivotCellCalculationInfo> GetPivotCellCalculationInfoEnumerable() {
			foreach (KeyValuePair<PivotDataKey, PivotCellCalculationInfo> pair in grandTotalsLevel) {
				PivotCellCalculationInfo cellValue = pair.Value;
				if (!cellValue.PassedFilters)
					continue;
				yield return cellValue;
			}
		}
	}
	#endregion
	#region PivotLabelFilterValueProvider
	public class PivotLabelFilterValueProvider : IAutoFilterValueProvider {
		readonly PivotTable pivotTable;
		readonly WorkbookDataContext dataContext;
		readonly int fieldIndex;
		readonly PivotTableFieldItemsVisibilityData visibilityData;
		public PivotLabelFilterValueProvider(PivotTable pivotTable, int fieldIndex, PivotTableFieldItemsVisibilityData visibilityData, WorkbookDataContext dataContext) {
			this.pivotTable = pivotTable;
			this.fieldIndex = fieldIndex;
			this.visibilityData = visibilityData;
			this.dataContext = dataContext;
		}
		#region IAutoFilterValueProvider Members
		public IEnumerable<VariantValue> GetValuesEnumerable() {
			foreach (IAutoFilterValue autofilterValue in GetAutoFilterValuesEnumerable()) {
				yield return autofilterValue.Value;
			}
		}
		public IVariantArray GetVariantArray() {
			VariantArray array = new VariantArray();
			List<VariantValue> values = new List<VariantValue>(GetValuesEnumerable());
			array.SetValues(values, values.Count, 1);
			return array;
		}
		public IEnumerable<IAutoFilterValue> GetAutoFilterValuesEnumerable() {
			IPivotCacheField cacheField = pivotTable.Cache.CacheFields[fieldIndex];
			int dataItemsCount = pivotTable.Fields[fieldIndex].Items.DataItemsCount;
			for (int i = 0; i < dataItemsCount; i++) {
				PivotItemVisibility itemVisibility = visibilityData.GetItemVisibility(i);
				if (itemVisibility.Visible) {
					int sharedItemIndex = itemVisibility.Item.ItemIndex;
					IPivotCacheRecordValue sharedItem = cacheField.SharedItems[sharedItemIndex];
					IAutoFilterValue autoFilterValue = new PivotLabelFilterAutoFilterValue(sharedItem, sharedItemIndex, i, visibilityData, dataContext);
					yield return autoFilterValue;
				}
			}
		}
		public double GetNumericValuesCount() {
			double result = 0;
			foreach (VariantValue value in GetValuesEnumerable()) {
				if (value.IsNumeric)
					result += value.NumericValue;
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region PivotAutoFilterValue
	public interface IPivotAutoFilterValue : IAutoFilterValue {
		bool Visible { get; set; }
	}
	public class PivotMeasureFilterAutoFilterValue : IPivotAutoFilterValue {
		readonly VariantValue value;
		readonly WorkbookDataContext dataContext;
		readonly PivotCellCalculationInfo cellValue;
		public PivotMeasureFilterAutoFilterValue(VariantValue value, PivotCellCalculationInfo cellValue, WorkbookDataContext dataContext) {
			this.value = value;
			this.cellValue = cellValue;
			this.dataContext = dataContext;
		}
		#region IAutoFilterValue Members
		public VariantValue Value { get { return value; } }
		public string Text { get { return GetTextValue(); } }
		public bool IsDateTime { get { return false; } }
		public bool Visible { get { return cellValue.PassedFilters; } set { cellValue.PassedFilters = value; } }
		#endregion
		string GetTextValue() {
			if (Value.IsError)
				return Value.ErrorValue.Name;
			return value.ToText(dataContext).GetTextValue(null);
		}
	}
	public struct PivotLabelFilterAutoFilterValue : IPivotAutoFilterValue {
		#region Fields
		readonly WorkbookDataContext dataContext;
		readonly PivotTableFieldItemsVisibilityData visibilityData;
		readonly IPivotCacheRecordValue value;
		readonly int itemIndex;
		readonly int sharedItemIndex;
		#endregion
		public VariantValue Value { get { return value.ToVariantValue(null, dataContext); } }
		public string Text { get { return GetTextValue(); } }
		public bool IsDateTime { get { return value.ValueType == PivotCacheRecordValueType.DateTime; } }
		public bool Visible { get { return visibilityData[itemIndex]; } set { visibilityData.SetItemVisibility(itemIndex, sharedItemIndex, value); } }
		public PivotLabelFilterAutoFilterValue(IPivotCacheRecordValue value, int sharedItemIndex, int itemIndex, PivotTableFieldItemsVisibilityData visibilityData, WorkbookDataContext dataContext) {
			this.value = value;
			this.dataContext = dataContext;
			this.visibilityData = visibilityData;
			this.sharedItemIndex = sharedItemIndex;
			this.itemIndex = itemIndex;
		}
		string GetTextValue() {
			if (Value.IsError)
				return Value.ErrorValue.Name;
			return Value.ToText(dataContext).InlineTextValue;
		}
	}
	#endregion
}
