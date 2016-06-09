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
using DevExpress.Office;
using DevExpress.Spreadsheet;
using System.Collections.Generic;
using DevExpress.Utils;
using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
using ModelVariantValueType = DevExpress.XtraSpreadsheet.Model.VariantValueType;
using ModelCellErrorType = DevExpress.XtraSpreadsheet.Model.ModelCellErrorType;
using ModelIVariantArray = DevExpress.XtraSpreadsheet.Model.IVariantArray;
using ModelVariantArray = DevExpress.XtraSpreadsheet.Model.VariantArray;
using ModelDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
using ModelDateSystem = DevExpress.XtraSpreadsheet.Model.DateSystem;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using System.Globalization;
using System.Diagnostics;
namespace DevExpress.Spreadsheet {
	#region FilterComparisonOperator
	public enum FilterComparisonOperator {
		LessThan = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.LessThan,
		Equal = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.Equal,
		LessThanOrEqual = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.LessThanOrEqual,
		GreaterThan = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.GreaterThan,
		NotEqual = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.NotEqual,
		GreaterThanOrEqual = DevExpress.XtraSpreadsheet.Model.FilterComparisonOperator.GreaterThanOrEqual
	}
	#endregion
	#region DateTimeGroupingType
	public enum DateTimeGroupingType {
		None = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.None,
		Year = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Year,
		Month = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Month,
		Day = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Day,
		Hour = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Hour,
		Minute = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Minute,
		Second = DevExpress.XtraSpreadsheet.Model.DateTimeGroupingType.Second
	}
	#endregion
	#region DynamicFilterType
	public enum DynamicFilterType {
		Null = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Null,
		AboveAverage = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.AboveAverage,
		BelowAverage = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.BelowAverage,
		Tomorrow = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Tomorrow,
		Today = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Today,
		Yesterday = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Yesterday,
		NextWeek = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.NextWeek,
		ThisWeek = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.ThisWeek,
		LastWeek = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.LastWeek,
		NextMonth = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.NextMonth,
		ThisMonth = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.ThisMonth,
		LastMonth = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.LastMonth,
		NextQuarter = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.NextQuarter,
		ThisQuarter = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.ThisQuarter,
		LastQuarter = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.LastQuarter,
		NextYear = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.NextYear,
		ThisYear = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.ThisYear,
		LastYear = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.LastYear,
		YearToDate = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.YearToDate,
		Q1 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Q1,
		Q2 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Q2,
		Q3 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Q3,
		Q4 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.Q4,
		M1 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M1,
		M2 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M2,
		M3 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M3,
		M4 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M4,
		M5 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M5,
		M6 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M6,
		M7 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M7,
		M8 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M8,
		M9 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M9,
		M10 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M10,
		M11 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M11,
		M12 = DevExpress.XtraSpreadsheet.Model.DynamicFilterType.M12
	}
	#endregion
	#region Top10Type
	public enum Top10Type {
		None = 0,
		Top10Items = 1,
		Top10Percent = 2,
		Bottom10Items = 3,
		Bottom10Percent = 4
	}
	#endregion
	#region FilterType
	public enum FilterType {
		None = 0,
		IconFilter = 1,
		Top10Filter = 2,
		CustomFilter = 3,
		FilterCriteria = 4,
		DynamicFilter = 5
	}
	#endregion
	public interface AutoFilterBase {
		AutoFilterColumnCollection Columns { get; }
		Range Range { get; }
		SortState SortState { get; }
		bool Enabled { get; }
		void ReApply();
		void Clear();
		void Disable();
	}
	public interface TableAutoFilter : AutoFilterBase {
		void Apply();
	}
	public interface SheetAutoFilter : AutoFilterBase {
		void Apply(Range range);
	}
	public interface AutoFilterColumnCollection : ISimpleCollection<AutoFilterColumn> {
		bool Contains(AutoFilterColumn filterColumn);
		int IndexOf(AutoFilterColumn filterColumn);
	}
	public interface AutoFilterColumn {
		FilterType FilterType { get; }
		CustomFilter CustomFilter { get; }
		FilterCriteria FilterCriteria { get; }
		bool HiddenButton { get; set; }
		Top10Type Top10Type { get; }
		int Top10Value { get; }
		DynamicFilterType DynamicFilterType { get; }
		void ApplyCustomFilter(FilterValue criteria, FilterComparisonOperator criteriaOperator);
		void ApplyCustomFilter(FilterValue firstCriteria, FilterComparisonOperator firstCriteriaOperator, FilterValue secondCriteria, FilterComparisonOperator secondCriteriaOperator, bool criterionAnd);
		void ApplyFilterCriteria(FilterValue filters);
		void ApplyFilterCriteria(IList<DateGrouping> dateGroupings);
		void ApplyFilterCriteria(FilterValue filters, IList<DateGrouping> dateGroupings);
		void ApplyTop10Filter(Top10Type top10Type, int value);
		void ApplyDynamicFilter(DynamicFilterType dynamicFilterType);
		void Clear();
	}
	public interface CustomFilter {
		FilterValue FirstCriteria { get; }
		FilterValue SecondCriteria { get; }
		FilterComparisonOperator FirstCriteriaOperator { get; }
		FilterComparisonOperator SecondCriteriaOperator { get; }
		bool CriterionAnd { get; }
	}
	public interface FilterCriteria {
		IList<DateGrouping> DateGroupings { get; }
		FilterValue Filters { get; }
		bool FilterByBlanks { get; }
	}
	public class DateGrouping {
		DateTime value;
		public DateGrouping(DateTime value, DateTimeGroupingType groupingType) {
			this.value = value;
			GroupingType = groupingType;
		}
		public DateTime Value { get { return value; } }
		public DateTimeGroupingType GroupingType { get; private set; }
	}
	#region FilterValue
	public class FilterValue {
		#region Static Members
		static readonly DateTime date1900Zero = new DateTime(1899, 12, 31);
		static readonly FilterValue filterByBlank = new FilterValue(String.Empty);
		public static FilterValue FilterByBlank { get { return FilterValue.filterByBlank; } }
		#endregion
		#region Fields
		readonly List<FilterValue> innerList = new List<FilterValue>();
		readonly ModelVariantValue modelValue;
		ModelDataContext context;
		#endregion
		internal FilterValue(ModelVariantValue modelValue) {
			Debug.Assert(!modelValue.IsSharedString && !modelValue.IsCellRange);
			this.modelValue = modelValue;
		}
		internal FilterValue(ModelVariantValue modelValue, ModelDataContext context) : this(modelValue) {
			this.context = context;
		}
		internal FilterValue(ModelVariantValue modelValue, ModelDataContext context, bool isDateTime)
			: this(modelValue, isDateTime) {
			this.context = context;
		}
		internal FilterValue(ModelVariantValue modelValue, bool isDateTime) : this(modelValue) {
			IsDateTime = isDateTime;
		}
		#region Properties
		public string[] Values { get { return ToStringArray(); } }
		internal ModelVariantValue VariantValue { get { return modelValue; } }
		internal ModelDataContext DataContext { get { return context; } set { context = value; } }
		internal bool IsDateTime { get; set; }
		internal bool IsEmpty { get { return modelValue.IsEmpty && !IsArray; } }
		internal bool IsArray { get { return innerList.Count > 0; } }
		#endregion
		#region Implicit conversions
		[DebuggerStepThrough]
		public static implicit operator FilterValue(int value) {
			return new FilterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(double value) {
			return new FilterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(char value) {
			return new FilterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(string value) {
			return new FilterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(bool value) {
			return new FilterValue(value);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(DateTime value) {
			return new FilterValue(CreateModelDateTime(value, false), true);
		}
		[DebuggerStepThrough]
		public static implicit operator FilterValue(CellValue value) {
			ModelVariantValue modelValue = value.ModelVariantValue;
			if (modelValue.IsSharedString)
				return new FilterValue(value.TextValue, value.ModelDataContext);
			return new FilterValue(modelValue, value.ModelDataContext, value.IsDateTime);
		}
		public static implicit operator FilterValue(CellValue[] values) {
			FilterValue filterArray = new FilterValue(ModelVariantValue.Empty);
			if (values == null || values.Length == 0)
				return filterArray;
			int length = values.Length;
			for (int i = 0; i < length; i++) {
				CellValue cellValue = values[i];
				if (!cellValue.IsEmpty) {
					ModelDataContext context = cellValue.ModelDataContext;
					if (context != null)
						filterArray.DataContext = context;
					filterArray.innerList.Add(cellValue);
				}
			}
			return filterArray;
		}
		public static FilterValue FromDateTime(DateTime value, bool use1904DateSystem) {
			return new FilterValue(CreateModelDateTime(value, use1904DateSystem), true);
		}
		#endregion
		static ModelVariantValue CreateModelDateTime(DateTime value, bool use1904DateSystem) {
			ModelVariantValue result = new ModelVariantValue();
			if (!use1904DateSystem && (value == date1900Zero))
				value = DateTime.MinValue;
			result.SetDateTime(value, use1904DateSystem ? ModelDateSystem.Date1904 : ModelDateSystem.Date1900);
			return result;
		}
		#region ToString
		string[] ToStringArray() {
			if (IsArray)
				return ArrayToStringArray();
			if (modelValue.IsEmpty)
				return new string[0];
			return new string[1] { ValueToString(context) };
		}
		string[] ArrayToStringArray() {
			int count = innerList.Count;
			string[] result = new string[count];
			for (int i = 0; i < count; i++)
				result[i] = innerList[i].ValueToString(context);
			return result;
		}
		string ValueToString(ModelDataContext context) {
			if (context == null)
				return ValueToString(CultureInfo.CurrentCulture, ModelDateSystem.Date1900);
			return ValueToString(context.Culture, context.DateSystem);
		}
		string ValueToString(CultureInfo culture, ModelDateSystem dateSystem) {
			switch (modelValue.Type) {
				default:
					return String.Empty;
				case ModelVariantValueType.Missing:
				case ModelVariantValueType.None:
					return String.Empty;
				case ModelVariantValueType.Error:
					return modelValue.ErrorValue.Name;
				case ModelVariantValueType.Boolean:
					return modelValue.BooleanValue.ToString(culture).ToUpper();
				case ModelVariantValueType.Numeric:
					return NumericToString(modelValue.NumericValue, culture, dateSystem);
				case ModelVariantValueType.InlineText:
					return modelValue.InlineTextValue;
			}
		}
		string NumericToString(double value, CultureInfo culture, ModelDateSystem dateSystem) {
			if (IsDateTime)
				return DateTimeToString(GetDate(value, dateSystem), culture);
			return value.ToString(culture);
		}
		DateTime GetDate(double value, ModelDateSystem dateSystem) {
			if (ModelWorkbookDataContext.IsErrorDateTimeSerial(value, dateSystem))
				return DateTime.MinValue;
			return modelValue.ToDateTime(dateSystem);
		}
		string DateTimeToString(DateTime date, CultureInfo culture) {
			string shortDate = date.ToString(culture.DateTimeFormat.ShortDatePattern, culture);
			if (date.Hour > 0 || date.Minute > 0 || date.Second > 0)
				return shortDate + " " + date.ToString(culture.DateTimeFormat.LongTimePattern, culture);
			return shortDate;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using System.Collections;
	using DevExpress.XtraSpreadsheet.Utils;
	#region NativeCustomFilter
	partial class NativeCustomFilter : NativeObjectBase, CustomFilter {
		readonly Model.CustomFilterCollection modelCustomFilters;
		public NativeCustomFilter(Model.CustomFilterCollection modelCustomFilters) {
			this.modelCustomFilters = modelCustomFilters;
		}
		#region CustomFilter Members
		int Count { get { return modelCustomFilters.Count; } }
		Model.CustomFilter FirstFilter { get { return Count > 0 ? modelCustomFilters[0] : null; } }
		Model.CustomFilter SecondFilter { get { return Count > 1 ? modelCustomFilters[1] : null; } }
		Model.WorkbookDataContext Context { get { return modelCustomFilters.Sheet.Workbook.DataContext; } }
		public FilterValue FirstCriteria {
			get {
				CheckValid();
				if (FirstFilter == null)
					return null;
				return new FilterValue(FirstFilter.Value, Context, FirstFilter.IsDateTime);
			}
		}
		public FilterValue SecondCriteria {
			get {
				CheckValid();
				if (SecondFilter == null)
					return null;
				return new FilterValue(SecondFilter.Value, Context, SecondFilter.IsDateTime);
			}
		}
		public FilterComparisonOperator FirstCriteriaOperator {
			get {
				CheckValid();
				if (FirstFilter == null)
					return FilterComparisonOperator.Equal;
				return (FilterComparisonOperator)FirstFilter.FilterOperator;
			}
		}
		public FilterComparisonOperator SecondCriteriaOperator {
			get {
				CheckValid();
				if (SecondFilter == null)
					return FilterComparisonOperator.Equal;
				return (FilterComparisonOperator)SecondFilter.FilterOperator;
			}
		}
		public bool CriterionAnd {
			get {
				CheckValid();
				return modelCustomFilters.CriterionAnd;
			}
		}
		#endregion
	}
	#endregion
	#region NativeFilterCriteria
	partial class NativeFilterCriteria : NativeObjectBase, FilterCriteria {
		readonly Model.FilterCriteria modelFilterCriteria;
		public NativeFilterCriteria(Model.FilterCriteria modelFilterCriteria) {
			this.modelFilterCriteria = modelFilterCriteria;
		}
		#region FilterCriteria Members
		public IList<DateGrouping> DateGroupings {
			get {
				CheckValid();
				return CreateDateGroupings(modelFilterCriteria.DateGroupings);
			}
		}
		public FilterValue Filters {
			get {
				CheckValid();
				return CreateFilters(modelFilterCriteria.Filters);
			}
		}
		public bool FilterByBlanks {
			get {
				CheckValid();
				return modelFilterCriteria.FilterByBlank;
			}
		}
		#endregion
		#region Internal
		List<DateGrouping> CreateDateGroupings(Model.DateGroupingCollection modelDateGroupings) {
			List<DateGrouping> result = new List<DateGrouping>();
			int count = modelDateGroupings.Count;
			for (int i = 0; i < count; i++)
				result.Add(CreateDateGrouping(modelDateGroupings[i]));
			return result;
		}
		DateGrouping CreateDateGrouping(Model.DateGrouping modelDateGrouping) {
			DateTime dateTime = modelDateGrouping.CreateDateTime();
			return new DateGrouping(dateTime, (DateTimeGroupingType)modelDateGrouping.DateTimeGrouping);
		}
		FilterValue CreateFilters(Model.FilterCollection filters) {
			int count = filters.Count;
			if (count == 0)
				return null;
			CellValue[] array = new CellValue[count];
			for (int i = 0; i < count; i++) {
				array[i] = filters[i];
				array[i].ModelDataContext = modelFilterCriteria.Sheet.Workbook.DataContext;
			}
			return array;
		}
		#endregion
	}
	#endregion
	#region NativeAutoFilterColumn
	partial class NativeAutoFilterColumn : NativeObjectBase, AutoFilterColumn {
		#region Fields
		readonly Model.AutoFilterBase parent;
		readonly Model.AutoFilterColumn modelFilterColumn;
		NativeCustomFilter nativeCustomFilter;
		NativeFilterCriteria nativeFilterCriteria;
		#endregion
		public NativeAutoFilterColumn(Model.AutoFilterColumn modelFilterColumn, Model.AutoFilterBase parent) {
			this.parent = parent;
			this.modelFilterColumn = modelFilterColumn;
		}
		#region Properties
		protected internal Model.AutoFilterColumn ModelFilterColumn { get { return modelFilterColumn; } }
		protected internal Model.DocumentModel DocumentModel { get { return modelFilterColumn.DocumentModel; } }
		protected internal Model.WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		#endregion
		#region AutoFilterColumn Members
		#region Properties
		public FilterType FilterType {
			get {
				CheckValid();
				return GetFilterType();
			}
		}
		public CustomFilter CustomFilter {
			get {
				CheckValid();
				if (nativeCustomFilter == null)
					nativeCustomFilter = new NativeCustomFilter(modelFilterColumn.CustomFilters);
				return nativeCustomFilter;
			}
		}
		public FilterCriteria FilterCriteria {
			get {
				CheckValid();
				if (nativeFilterCriteria == null)
					nativeFilterCriteria = new NativeFilterCriteria(modelFilterColumn.FilterCriteria);
				return nativeFilterCriteria;
			}
		}
		public bool HiddenButton {
			get {
				CheckValid();
				return modelFilterColumn.HiddenAutoFilterButton;
			}
			set {
				CheckValid();
				modelFilterColumn.HiddenAutoFilterButton = value;
			}
		}
		public Top10Type Top10Type {
			get {
				CheckValid();
				if (!modelFilterColumn.IsTop10Filter)
					return Top10Type.None;
				return GetTop10Type();
			}
		}
		public int Top10Value {
			get {
				CheckValid();
				return (int)modelFilterColumn.TopOrBottomDoubleValue;
			}
		}
		public DynamicFilterType DynamicFilterType {
			get {
				CheckValid();
				return (DynamicFilterType)modelFilterColumn.DynamicFilterType;
			}
		}
		public IconSetType IconType {
			get {
				CheckValid();
				return (IconSetType)modelFilterColumn.IconSetType;
			}
		}
		public int IconId {
			get {
				CheckValid();
				return modelFilterColumn.IconId;
			}
		}
		#endregion
		#region ApplyCustomFilter
		public void ApplyCustomFilter(FilterValue criteria, FilterComparisonOperator criteriaOperator) {
			CheckValid();
			CheckValid(criteria);
			if (criteria.IsEmpty)
				return;
			criteria.DataContext = Context;
			if (ShouldAssignToFilterCriteria(criteria.Values[0], criteriaOperator)) {
				ApplyFilterCriteria(criteria);
				return;
			}
			ApplyCustomFilterCore(criteria, criteriaOperator);
		}
		void ApplyCustomFilterCore(FilterValue criteria, FilterComparisonOperator criteriaOperator) {
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				modelFilterColumn.CustomFilters.Add(CreateCustomFilter(criteria, criteriaOperator));
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void ApplyCustomFilter(FilterValue firstCriteria, FilterComparisonOperator firstCriteriaOperator, FilterValue secondCriteria, FilterComparisonOperator secondCriteriaOperator, bool criterionAnd) {
			CheckValid();
			CheckValid(firstCriteria);
			CheckValid(secondCriteria);
			if (firstCriteria.IsEmpty) {
				ApplyCustomFilter(secondCriteria, secondCriteriaOperator);
				return;
			}
			if (secondCriteria.IsEmpty) {
				ApplyCustomFilter(firstCriteria, firstCriteriaOperator);
				return;
			}
			firstCriteria.DataContext = Context;
			secondCriteria.DataContext = Context;
			string firstCriteriaValue = firstCriteria.Values[0];
			string secondCriteriaValue = secondCriteria.Values[0];
			if (ShouldAssignToFilterCriteria(firstCriteriaValue, firstCriteriaOperator) && !criterionAnd &&
				ShouldAssignToFilterCriteria(secondCriteriaValue, secondCriteriaOperator)) {
				ApplyFilterCriteria(new CellValue[] { firstCriteriaValue, secondCriteriaValue });
				return;
			}
			ApplyCustomFilterCore(firstCriteria, firstCriteriaOperator, secondCriteria, secondCriteriaOperator, criterionAnd);
		}
		void ApplyCustomFilterCore(FilterValue firstCriteria, FilterComparisonOperator firstCriteriaOperator, FilterValue secondCriteria, FilterComparisonOperator secondCriteriaOperator, bool criterionAnd) {
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				modelFilterColumn.CustomFilters.CriterionAnd = criterionAnd;
				modelFilterColumn.CustomFilters.Add(CreateCustomFilter(firstCriteria, firstCriteriaOperator));
				if (firstCriteria.Values[0] != secondCriteria.Values[0] || firstCriteriaOperator != secondCriteriaOperator)
					modelFilterColumn.CustomFilters.Add(CreateCustomFilter(secondCriteria, secondCriteriaOperator));
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void CheckValid(FilterValue criteria) {
			if (criteria == null || criteria.Values.Length > 1)
				ThrowInvalidFilterArgumentError();
		}
		bool ShouldAssignToFilterCriteria(string criteriaValue, FilterComparisonOperator criteriaOperator) {
			return criteriaOperator == FilterComparisonOperator.Equal && !Model.CustomFilter.ContainsWildcardCharacters(criteriaValue);
		}
		Model.CustomFilter CreateCustomFilter(FilterValue criteria, FilterComparisonOperator criteriaOperator) {
			Model.CustomFilter result = new Model.CustomFilter();
			result.FilterOperator = (Model.FilterComparisonOperator)criteriaOperator;
			result.Value = criteria.Values[0];
			result.UpdateNumericValue(DocumentModel.DataContext, criteria.IsDateTime);
			return result;
		}
		#endregion
		#region ApplyFilterCriteria
		public void ApplyFilterCriteria(FilterValue filters) {
			CheckValid();
			if (filters == null)
				ThrowInvalidFilterArgumentError();
			if (filters.IsEmpty)
				return;
			filters.DataContext = Context;
			ApplyFilterCriteriaCore(filters.Values, null);
		}
		public void ApplyFilterCriteria(IList<DateGrouping> dateGroupings) {
			CheckValid();
			if (dateGroupings == null)
				ThrowInvalidFilterArgumentError();
			if (dateGroupings.Count == 0)
				return;
			ApplyFilterCriteriaCore(null, dateGroupings);
		}
		public void ApplyFilterCriteria(FilterValue filters, IList<DateGrouping> dateGroupings) {
			CheckValid();
			if (filters == null || dateGroupings == null)
				ThrowInvalidFilterArgumentError();
			filters.DataContext = Context;
			string[] filterValues = filters.Values;
			int datesCount = dateGroupings.Count;
			if (filters.IsEmpty && datesCount == 0)
				return;
			else if (filters.IsEmpty)
				ApplyFilterCriteriaCore(null, dateGroupings);
			else if (datesCount == 0)
				ApplyFilterCriteriaCore(filterValues, null);
			else
				ApplyFilterCriteriaCore(filterValues, dateGroupings);
		}
		void ApplyFilterCriteriaCore(string[] filterValues, IList<DateGrouping> dateGroupings) {
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				if (filterValues != null)
					foreach (string filterValue in filterValues)
						AssignFilter(modelFilterColumn.FilterCriteria, filterValue);
				if (dateGroupings != null)
					AssignDateGroupings(modelFilterColumn.FilterCriteria.DateGroupings, dateGroupings);
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void AssignFilter(Model.FilterCriteria modelCriteria, string filterValue) {
			if (!String.IsNullOrEmpty(filterValue)) {
				Model.FilterCollection modelFilters = modelCriteria.Filters;
				if (!modelFilters.Contains(filterValue))
					modelFilters.Add(filterValue);
			}
			else
				modelCriteria.FilterByBlank = true;
		}
		void AssignDateGroupings(Model.DateGroupingCollection modelGroupings, IList<DateGrouping> dateGroupings) {
			int count = dateGroupings.Count;
			for (int i = 0; i < count; i++) {
				DateGrouping dateGrouping = dateGroupings[i];
				if (dateGrouping.GroupingType == DateTimeGroupingType.None)
					continue;
				Model.DateTimeGroupingType modelGroupingType = (Model.DateTimeGroupingType)dateGrouping.GroupingType;
				Model.DateGrouping modelDateGrouping = Model.DateGrouping.Create(modelFilterColumn.Sheet, dateGrouping.Value, modelGroupingType);
				if (!modelGroupings.ContainsInfo(modelDateGrouping.Info))
					modelGroupings.Add(modelDateGrouping);
			}
		}
		#endregion
		#region ApplyTop10Filter
		public void ApplyTop10Filter(Top10Type top10Type, int value) {
			if (top10Type == Top10Type.None)
				return;
			CheckValid();
			if (value < 1 || value > 500)
				ThrowInvalidFilterArgumentError();
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				modelFilterColumn.FilterByTopOrder = top10Type == Top10Type.Top10Items || top10Type == Top10Type.Top10Percent;
				if(top10Type == Top10Type.Bottom10Percent || top10Type == Top10Type.Top10Percent)
					modelFilterColumn.Top10FilterType = DevExpress.XtraSpreadsheet.Model.Top10FilterType.Percent;
				else
					modelFilterColumn.Top10FilterType = DevExpress.XtraSpreadsheet.Model.Top10FilterType.Count;
				modelFilterColumn.TopOrBottomDoubleValue = value;
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region ApplyDynamicFilter
		public void ApplyDynamicFilter(DynamicFilterType dynamicFilterType) {
			if (dynamicFilterType == DynamicFilterType.Null)
				return;
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				modelFilterColumn.DynamicFilterType = (Model.DynamicFilterType)dynamicFilterType;
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Clear
		public void Clear() {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				modelFilterColumn.Clear();
				parent.ReApplyFilter();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#endregion
		#region Internal
		void ThrowInvalidFilterArgumentError() {
			ApiErrorHandler.Instance.HandleError(new Model.ModelErrorInfo(Model.ModelErrorType.ErrorInvalidFilterArgument));
		}
		FilterType GetFilterType() {
			if (modelFilterColumn.IsDynamicFilter)
				return FilterType.DynamicFilter;
			else if (modelFilterColumn.IsTop10Filter)
				return FilterType.Top10Filter;
			else if (modelFilterColumn.IsIconFilter)
				return FilterType.IconFilter;
			else if (modelFilterColumn.FilterCriteria.HasFilter())
				return FilterType.FilterCriteria;
			else if (modelFilterColumn.CustomFilters.Count > 0)
				return FilterType.CustomFilter;
			return FilterType.None;
		}
		Top10Type GetTop10Type() {
			bool filterByPercent = modelFilterColumn.Top10FilterType == Model.Top10FilterType.Percent;
			if (modelFilterColumn.FilterByTopOrder)
				return filterByPercent ? Top10Type.Top10Percent : Top10Type.Top10Items;
			return filterByPercent ? Top10Type.Bottom10Percent : Top10Type.Bottom10Items;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeFilterCriteria != null)
				nativeFilterCriteria.IsValid = value;
			if (nativeCustomFilter != null)
				nativeCustomFilter.IsValid = value;
		}
		#endregion
	}
	#endregion
	#region NativeAutoFilterColumnCollection
	partial class NativeAutoFilterColumnCollection : NativeObjectBase, AutoFilterColumnCollection {
		readonly List<NativeAutoFilterColumn> innerList = new List<NativeAutoFilterColumn>();
		public NativeAutoFilterColumnCollection() { }
		internal List<NativeAutoFilterColumn> InnerList { get { return innerList; } }
		#region AutoFilterColumnCollection Members
		public bool Contains(AutoFilterColumn filterColumn) {
			return IndexOf(filterColumn) != -1;
		}
		public int IndexOf(AutoFilterColumn filterColumn) {
			CheckValid();
			NativeAutoFilterColumn nativeFilterColumn = filterColumn as NativeAutoFilterColumn;
			if (nativeFilterColumn != null)
				return InnerList.IndexOf(nativeFilterColumn);
			return -1;
		}
		#endregion
		#region ISimpleCollection<AutoFilterColumn> Members
		public AutoFilterColumn this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		#endregion
		#region IEnumerable<AutoFilterColumn> Members
		public IEnumerator<AutoFilterColumn> GetEnumerator() {
			CheckValid();
			return new EnumeratorConverter<NativeAutoFilterColumn, AutoFilterColumn>(innerList.GetEnumerator(), ConvertNativeColumnToColumn);
		}
		AutoFilterColumn ConvertNativeColumnToColumn(NativeAutoFilterColumn item) {
			return item;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public bool IsSynchronized {
			get {
				CheckValid();
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		public object SyncRoot {
			get {
				CheckValid();
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		#endregion
		internal void AddCore(NativeAutoFilterColumn item) {
			innerList.Add(item);
		}
		internal void ClearCore() {
			InvalidateColumns();
			innerList.Clear();
		}
		void InvalidateColumns() {
			foreach (NativeAutoFilterColumn item in innerList)
				item.IsValid = false;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value)
				InvalidateColumns();
		}
	}
	#endregion
	#region NativeAutoFilterBase
	abstract partial class NativeAutoFilterBase : NativeObjectBase, AutoFilterBase {
		#region Fields
		readonly Model.AutoFilterBase modelAutoFilter;
		readonly NativeWorksheet nativeWorksheet;
		NativeAutoFilterColumnCollection nativeColumns;
		NativeSortState nativeSortState;
		NativeRange nativeRange;
		#endregion
		protected NativeAutoFilterBase(Model.AutoFilterBase modelAutoFilter, NativeWorksheet nativeWorksheet) {
			this.modelAutoFilter = modelAutoFilter;
			this.nativeWorksheet = nativeWorksheet;
			this.nativeColumns = new NativeAutoFilterColumnCollection();
			modelAutoFilter.OnRangeChanged += OnRangeChanged;
		}
		#region Properties
		protected Model.AutoFilterBase ModelAutoFilter { get { return modelAutoFilter; } }
		protected Model.DocumentModel DocumentModel { get { return modelAutoFilter.Workbook; } }
		protected Model.CellRange ModelRange { get { return modelAutoFilter.Range; } }
		protected NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		#region AutoFilterBase Members
		public bool Enabled { get { return modelAutoFilter.Enabled; } }
		public Range Range {
			get {
				CheckValid();
				if (ModelRange == null)
					return null;
				if (nativeRange == null)
					nativeRange = new NativeRange(ModelRange, nativeWorksheet);
				return nativeRange;
			}
		}
		public AutoFilterColumnCollection Columns {
			get {
				CheckValid();
				return nativeColumns;
			}
		}
		public SortState SortState {
			get {
				CheckValid();
				if (!Enabled)
					return null;
				if (nativeSortState == null)
					nativeSortState = new NativeSortState(modelAutoFilter.SortState, ModelRange.GetResized(0, 1, 0, 0));
				return nativeSortState;
			}
		}
		public void ReApply() {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				modelAutoFilter.ReApplyFilter();
				if (SortState != null)
					modelAutoFilter.SortState.Apply(ApiErrorHandler.Instance);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void Clear() {
			CheckValid();
			modelAutoFilter.ClearFilter();
		}
		public void Disable() {
			CheckValid();
			modelAutoFilter.Disable();
			nativeSortState = null;
		}
		#endregion
		#endregion
		#region OnRangeChanged
		void OnRangeChanged(object sender, EventArgs e) {
			Model.AutoFilterRangeChangedEventArgs modelArgs = e as Model.AutoFilterRangeChangedEventArgs;
			if (modelArgs != null) {
				nativeColumns.ClearCore();
				RegisterFilterColumns(modelArgs.FilterColumns);
				UpdateFilterRange();
			}
		}
		void UpdateFilterRange() {
			if (ModelRange != null)
				nativeRange = new NativeRange(ModelRange, nativeWorksheet);
		}
		protected void RegisterFilterColumns(Model.AutoFilterColumnCollection filterColumns) {
			int count = filterColumns.Count;
			for (int i = 0; i < count; i++) {
				NativeAutoFilterColumn nativeFilterColumn = new NativeAutoFilterColumn(filterColumns[i], modelAutoFilter);
				nativeColumns.AddCore(nativeFilterColumn);
			}
		}
		#endregion
		#region SetIsValid
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value)
				modelAutoFilter.OnRangeChanged -= OnRangeChanged;
			if (nativeColumns != null)
				nativeColumns.IsValid = value;
			if (nativeSortState != null)
				nativeSortState.IsValid = value;
		}
		#endregion
	}
	#endregion
	#region NativeSheetAutoFilter
	partial class NativeSheetAutoFilter : NativeAutoFilterBase, SheetAutoFilter {
		public NativeSheetAutoFilter(Model.SheetAutoFilter modelAutoFilter, NativeWorksheet nativeWorksheet)
			: base(modelAutoFilter, nativeWorksheet) {
		}
		#region SheetAutoFilter Members
		public void Apply(Range range) {
			CheckValid();
			if (NativeWorksheet.ContainsPivotTable(range)) {
				ApiErrorHandler errorHandler = ApiErrorHandler.Instance;
				errorHandler.HandleError(new Model.ModelErrorInfo(Model.ModelErrorType.PivotTableCanNotBeChanged));
				return;
			}
			Model.CellRange modifiedRange = GetModifiedRange(range);
			DocumentModel.BeginUpdate();
			try {
				if (Enabled)
					Disable();
				ModelAutoFilter.Range = modifiedRange.Clone();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		Model.CellRange GetModifiedRange(Range range) {
			ApiErrorHandler errorHandler = ApiErrorHandler.Instance;
			if (range == null)
				errorHandler.HandleError(new Model.ModelErrorInfo(Model.ModelErrorType.ErrorInvalidRange));
			IList<Table> tables = range.Worksheet.Tables.GetTables(range);
			if (tables.Count > 0)
				errorHandler.HandleError(new Model.ModelErrorInfo(Model.ModelErrorType.ErrorRangeContainsTable));
			Model.CellRange modelRange = NativeWorksheet.GetModelSingleRange(range);
			Commands.SortOrFilterRangeHelper helper = new Commands.SortOrFilterRangeHelper(modelRange);
			Model.CellRange result = helper.GetSheetSortOrFilterRange();
			if (result == null)
				errorHandler.HandleError(new Model.ModelErrorInfo(Model.ModelErrorType.ErrorRangeConsistsOfEmptyCells));
			return result;
		}
		#endregion
	}
	#endregion
	#region NativeTableAutoFilter
	partial class NativeTableAutoFilter : NativeAutoFilterBase, TableAutoFilter {
		public NativeTableAutoFilter(Model.TableAutoFilter modelAutoFilter, NativeWorksheet nativeWorksheet)
			: base(modelAutoFilter, nativeWorksheet) {
			TableRange = modelAutoFilter.Range;
			RegisterFilterColumns(modelAutoFilter.FilterColumns);
		}
		Model.CellRange TableRange { get; set; }
		#region TableAutoFilter Members
		public void Apply() {
			CheckValid();
			if (Enabled)
				return;
			Model.Table table = ModelAutoFilter.Sheet.Tables.TryGetItem(TableRange.BottomRight);
			table.ChangeHeaders(true, true, true, ApiErrorHandler.Instance);
		}
		#endregion
	}
	#endregion
}
