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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class FilterValue : CollectionItem {
		string displayText, query, value;
		public const string
			FilterAllQuery = "(ShowAll)",
			FilterCalendarQuery = "(Calendar)",
			FilterDateRangePickerQuery = "(DateRangePicker)";
		public static FilterValue CreateShowAllValue(string text) {
			return new FilterValue(text, "", FilterValue.FilterAllQuery);
		}
		[Obsolete("This method is now obsolete. Use the CreateShowBlanksValue(GridViewDataColumn column, string text) method instead.")]
		public static FilterValue CreateShowBlanksValue(string fieldName, string text) {
			OperandProperty operand = new OperandProperty(fieldName);
			CriteriaOperator crit = CriteriaOperator.Or(operand.IsNull(), operand == string.Empty);
			return new FilterValue(text, string.Empty, crit.ToString());
		}
		[Obsolete("This method is now obsolete. Use the CreateShowNonBlanksValue(GridViewDataColumn column, string text) method instead.")]
		public static FilterValue CreateShowNonBlanksValue(string fieldName, string text) {
			OperandProperty operand = new OperandProperty(fieldName);
			CriteriaOperator crit = CriteriaOperator.And(operand.IsNotNull(), operand != string.Empty);
			return new FilterValue(text, string.Empty, crit.ToString());
		}
		public static FilterValue CreateShowBlanksValue(GridViewDataColumn column, string text) {
			return GetBlanksValue(column, text, false);
		}
		public static FilterValue CreateShowNonBlanksValue(GridViewDataColumn column, string text) {
			return GetBlanksValue(column, text, true);
		}
		public static FilterValue CreateShowBlanksValue(CardViewColumn column, string text) {
			return GetBlanksValue(column, text, false);
		}
		public static FilterValue CreateShowNonBlanksValue(CardViewColumn column, string text) {
			return GetBlanksValue(column, text, true);
		}
		internal static FilterValue GetBlanksValue(IWebGridDataColumn column, string text, bool nonBlanks) {
			bool filterByText = column.Adapter.FilterMode == ColumnFilterMode.DisplayText || column.Adapter.DataType == typeof(string);
			return GetBlanksValue(column.FieldName, text, filterByText, nonBlanks);
		}
		internal static FilterValue GetBlanksValue(string fieldName, string text, bool filterByText, bool nonBlanks) {
			return new FilterValue(text, string.Empty, GetBlanksCriteria(fieldName, filterByText, nonBlanks).ToString());
		}
		static CriteriaOperator GetBlanksCriteria(string fieldName, bool filterByText, bool nonBlanks) {
			CriteriaOperator result;
			CriteriaOperator op = new OperandProperty(fieldName);
			if(filterByText)
				result = new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, op);
			else
				result = op.IsNull();
			if(nonBlanks)
				result = !result;
			return result;
		}
		internal static FilterValue CreateTomorrowValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeTomorrow);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeDayAfterTomorrow);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateTodayValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeToday);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeTomorrow);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateYesterdayValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeYesterday);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeToday);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateNextWeekValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextWeek);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeTwoWeeksAway);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateThisWeekValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisWeek);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextWeek);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateLastWeekValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeLastWeek);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisWeek);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateNextMonthValue(IWebGridDataColumn column, string text) { 
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextMonth);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.AddMonths, startDateCriteria, 1);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateThisMonthValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisMonth);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextMonth);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateLastMonthValue(IWebGridDataColumn column, string text) { 
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisMonth);
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.AddMonths, endDateCriteria, -1);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateNextYearValue(IWebGridDataColumn column, string text) { 
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextYear);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.AddYears, startDateCriteria, 1);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateThisYearValue(IWebGridDataColumn column, string text) {
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisYear);
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeNextYear);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		internal static FilterValue CreateLastYearValue(IWebGridDataColumn column, string text) { 
			CriteriaOperator endDateCriteria = new FunctionOperator(FunctionOperatorType.LocalDateTimeThisYear);
			CriteriaOperator startDateCriteria = new FunctionOperator(FunctionOperatorType.AddYears, endDateCriteria, -1);
			return CreateRelativeDateRangeValue(column, startDateCriteria, endDateCriteria, text);
		}
		static FilterValue CreateDateRangeValueCore(IWebGridDataColumn column, FunctionOperatorType operatorType, string text) {
			var query = new FunctionOperator(operatorType, new OperandProperty(column.FieldName));
			return new FilterValue(text, string.Empty, query.ToString());
		}
		static FilterValue CreateRelativeDateRangeValue(IWebGridDataColumn column, CriteriaOperator startDateCriteria, CriteriaOperator endDateCriteria, string text) {
			var prop = new OperandProperty(column.FieldName);
			var query = prop >= startDateCriteria & prop < endDateCriteria;
			return new FilterValue(text, string.Empty, query.ToString());
		}
		internal static FilterValue CreateDateRangeValue(IWebGridDataColumn column, FunctionOperatorType operatorType, string text) {
			var query = new FunctionOperator(operatorType, new OperandProperty(column.FieldName));
			return new FilterValue(text, string.Empty, query.ToString());
		}
		public FilterValue(string displayText, string value, string query) {
			this.displayText = displayText;
			this.query = query;
			this.value = value;
		}
		public FilterValue(string displayText, string value) : this(displayText, value, "") { }
		public FilterValue() : this("", "", "") { }
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueDisplayText")]
#endif
		public string DisplayText {
			get { return displayText; }
			set {
				if(value == null) value = string.Empty;
				displayText = value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueQuery")]
#endif
		public string Query {
			get { return query; }
			set {
				if(value == null) value = string.Empty;
				query = value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueValue")]
#endif
		public string Value {
			get { return value; }
			set {
				if(value == null) value = string.Empty;
				this.value = value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueIsFilterByValue")]
#endif
		public bool IsFilterByValue { get { return string.IsNullOrEmpty(Query); } }
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueIsFilterByQuery")]
#endif
		public bool IsFilterByQuery { get { return !string.IsNullOrEmpty(Query); } }
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueIsShowAllFilter")]
#endif
		public bool IsShowAllFilter { get { return Query == FilterAllQuery; } }
		public bool IsCalendarFilter { get { return Query == FilterCalendarQuery; } }
		public bool IsDateRangePickerFilter { get { return Query == FilterDateRangePickerQuery; } }
#if !SL
	[DevExpressWebLocalizedDescription("FilterValueIsEmpty")]
#endif
		public bool IsEmpty { get { return IsShowAllFilter; } }
		public override string ToString() { return DisplayText; }
		internal string HtmlValue {
			get {
				if(IsFilterByQuery) return "#" + Query;
				return "!" + Value;
			}
		}
		internal static FilterValue FromHtmlValue(string value) {
			if(string.IsNullOrEmpty(value))
				return new FilterValue("", "", FilterAllQuery);
			if(value[0] == '#')
				return new FilterValue(value, "", value.Substring(1));
			if(value.StartsWith(FilterCalendarQuery))
				return new FilterValue("", value, FilterCalendarQuery);
			if(value.StartsWith(FilterDateRangePickerQuery))
				return new FilterValue("", value, FilterDateRangePickerQuery);
			return new FilterValue(value.Substring(1), value.Substring(1), "");
		}
	}
	public class GridHeaderFilterValues : Collection<FilterValue> {
		public GridHeaderFilterValues()
			: base() {
			SeparatorIndeces = new List<int>();
		}
		protected internal List<int> SeparatorIndeces { get; private set; }
		public void AddSeparator() {
			AddSeparator(Count - 1);
		}
		public int IndexOf(FilterValue item, int index) {
			return IndexOf(i => i.Index >= index && i == item);
		}
		public bool Exists(Predicate<FilterValue> match) {
			return IndexOf(match) != -1;
		}
		protected override void OnClear() {
			base.OnClear();
			SeparatorIndeces.Clear();
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			SyncSeparators(index, false);
		}
		protected override void OnRemove(int index, object value) {
			base.OnRemove(index, value);
			SyncSeparators(index, true);
		}
		protected void AddSeparator(int index) {
			if(IsValidSeparatorIndex(index))
				SeparatorIndeces.Add(index);
		}
		protected void SyncSeparators(int index, bool isRemove) {
			if(index < 0) return;
			for(int i = 0; i < SeparatorIndeces.Count; i++) {
				if(index > SeparatorIndeces[i]) continue;
				int newIndex = SeparatorIndeces[i] + (isRemove ? -1 : 1);
				if(IsValidSeparatorIndex(newIndex))
					SeparatorIndeces[i] = newIndex;
				else{
					SeparatorIndeces.RemoveAt(i);
					i--;
				}
			}
		}
		bool IsValidSeparatorIndex(int index) {
			return index >= 0 && index <= Count && !SeparatorIndeces.Contains(index);
		}
		#region Hidden Members
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new void Move(int oldIndex, int newIndex) { base.Move(oldIndex, newIndex); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new FilterValue GetVisibleItem(int index) { return base.GetVisibleItem(index); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new int GetVisibleItemCount() { return base.GetVisibleItemCount(); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new IEnumerable GetVisibleItems() { return base.GetVisibleItems(); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new void Assign(IAssignableCollection source) { base.Assign(source); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new IWebControlObject Owner { get { return base.Owner; } }
		#endregion
	}
}
namespace DevExpress.Web.Internal {
	public enum GridColumnEditKind { Text, DateEdit, ComboBox, CheckBox };
	public enum FilterRowTypeKind { String, SingleOption, Ordinal };
	public class BaseFilterHelper {
		public static string LikeConditionSymbol = "L";
		internal static string GetFilterRowTypeKindSymbol(FilterRowTypeKind kind) {
			if(kind == FilterRowTypeKind.Ordinal)
				return "N";
			if(kind == FilterRowTypeKind.SingleOption)
				return string.Empty;
			return "S";
		}
		public static bool IsValidCondition(FilterRowTypeKind kind, AutoFilterCondition condition) {
			switch(condition) {
				case AutoFilterCondition.BeginsWith:
				case AutoFilterCondition.Contains:
				case AutoFilterCondition.DoesNotContain:
				case AutoFilterCondition.EndsWith:
					return kind == FilterRowTypeKind.String;
				case AutoFilterCondition.Less:
				case AutoFilterCondition.LessOrEqual:
				case AutoFilterCondition.Greater:
				case AutoFilterCondition.GreaterOrEqual:
					return kind == FilterRowTypeKind.Ordinal;
				case AutoFilterCondition.NotEqual:
					return kind != FilterRowTypeKind.SingleOption;
				case AutoFilterCondition.Like:
					return false;
				default:
					return true;
			}
		}
		protected static string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, CriteriaOperator currentFilter, bool dateField) {
			if(currentFilter is GroupOperator && dateField)
				return GetColumnAutoFilterText(fieldName, (GroupOperator)currentFilter);
			return GetColumnAutoFilterText(condition, fieldName, currentFilter);
		}
		protected static string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, CriteriaOperator currentFilter) {
			if(currentFilter is BinaryOperator)
				return GetColumnAutoFilterText(condition, fieldName, (BinaryOperator)currentFilter);
			if(currentFilter is FunctionOperator)
				return GetColumnAutoFilterText(condition, fieldName, (FunctionOperator)currentFilter);
			if(currentFilter is UnaryOperator && (condition == AutoFilterCondition.DoesNotContain || condition == AutoFilterCondition.Contains))
				return GetColumnAutoFilterText(AutoFilterCondition.Default, fieldName, (currentFilter as UnaryOperator).Operand);
			return String.Empty;
		}
		static string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, BinaryOperator op) {
			if(op.OperatorType == BinaryOperatorType.Like && condition != AutoFilterCondition.Like)
				return String.Empty;
			OperandProperty fieldOp = op.LeftOperand as OperandProperty;
			OperandValue valueOp = op.RightOperand as OperandValue;
			if(ReferenceEquals(fieldOp, null) || ReferenceEquals(valueOp, null))
				return String.Empty;
			object value = valueOp.Value;
			if(fieldOp.PropertyName != fieldName || value == null)
				return String.Empty;
			if(value is DateTime)
				value = ((DateTime)value).ToString("d", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			return value.ToString();
		}
		static string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, FunctionOperator op) {
			switch(op.OperatorType) {
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					if(op.Operands.Count < 2)
						return String.Empty;
					OperandProperty fieldOp = op.Operands[0] as OperandProperty;
					OperandValue valueOp = op.Operands[1] as OperandValue;
					if(ReferenceEquals(fieldOp, null) || ReferenceEquals(valueOp, null))
						return String.Empty;
					return (valueOp.Value ?? String.Empty).ToString();
				case FunctionOperatorType.Custom:
					if(condition == AutoFilterCondition.Like
						&& DevExpress.Data.Filtering.Helpers.LikeCustomFunction.IsBinaryCompatibleLikeFunction(op)
						&& op.Operands[1] is OperandProperty && op.Operands[2] is OperandValue)
						return (((OperandValue)op.Operands[2]).Value ?? string.Empty).ToString();
					break;
			}
			return String.Empty;
		}
		static string GetColumnAutoFilterText(string fieldName, GroupOperator op) {
			if(op.Operands.Count < 1)
				return String.Empty;
			return GetColumnAutoFilterText(AutoFilterCondition.Default, fieldName, op.Operands[0]);
		}
		protected static CriteriaOperator CreateAutoFilter(AutoFilterCondition condition, string fieldName, Type dataType, string _value, bool roundDateTime) {
			if(string.IsNullOrEmpty(_value))
				return null;
			if(condition == AutoFilterCondition.Equals) {
				return DevExpress.Data.Helpers.FilterHelper.CalcColumnFilterCriteriaByValue(fieldName, dataType, ChangeTypeSafe(_value, dataType), roundDateTime, System.Globalization.CultureInfo.InvariantCulture);
			}
			if(condition == AutoFilterCondition.NotEqual && dataType == typeof(DateTime) && roundDateTime) {
				object value = ChangeTypeSafe(_value, dataType);
				if(value is DateTime)
					return GenerateDateTimeNotEqualsCondition(fieldName, (DateTime)value);
			}
			return GetOperatorForCondition(condition, fieldName, _value, dataType);
		}
		static CriteriaOperator GenerateDateTimeNotEqualsCondition(string fieldName, DateTime value) {
			var start = value.Date;
			var prop = new OperandProperty(fieldName);
			try {
				var end = value.AddDays(1);
				return prop < start | prop >= end;
			} catch {
				return prop < start;
			}
		}
		static CriteriaOperator GetOperatorForCondition(AutoFilterCondition condition, string fieldName, string value, Type dataType) {
			switch(condition) {
				case AutoFilterCondition.NotEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.NotEqual);
				case AutoFilterCondition.Less:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.Less);
				case AutoFilterCondition.Greater:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.Greater);
				case AutoFilterCondition.LessOrEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.LessOrEqual);
				case AutoFilterCondition.GreaterOrEqual:
					return new BinaryOperator(fieldName, ChangeTypeSafe(value, dataType), BinaryOperatorType.GreaterOrEqual);
				case AutoFilterCondition.DoesNotContain:
					return new UnaryOperator(UnaryOperatorType.Not, GetOperatorForCondition(AutoFilterCondition.Contains, fieldName, value, dataType));
				case AutoFilterCondition.BeginsWith:
					return new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(fieldName), value);
				case AutoFilterCondition.EndsWith:
					return new FunctionOperator(FunctionOperatorType.EndsWith, new OperandProperty(fieldName), value);
				case AutoFilterCondition.Contains:
					return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(fieldName), value);
				case AutoFilterCondition.Like:
					return new BinaryOperator(fieldName, value, BinaryOperatorType.Like);
				default:
					throw new NotSupportedException();
			}
		}
		public static object ChangeTypeSafe(object value, Type targetType) {
			return ChangeTypeSafe(value, targetType, false);
		}
		public static object ChangeTypeSafe(object value, Type targetType, bool tryParseDateWithCurrentCulture) {
			if(value == null)
				return null;
			if(tryParseDateWithCurrentCulture && targetType == typeof(DateTime)) {
				try {
					value = DateTime.Parse(value.ToString(), CultureInfo.CurrentCulture);
				} catch { }
			}
			Type sourceType = value.GetType();
			if(sourceType == targetType)
				return value;
			try {
				targetType = ReflectionUtils.StripNullableType(targetType);
				value = FixFloatingPoint(value, targetType); 
				TypeConverter convertor = TypeDescriptor.GetConverter(targetType);
				if(convertor.CanConvertFrom(sourceType))
					return convertor.ConvertFrom(null, CultureInfo.InvariantCulture, value);
				return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
			} catch {
				return value;
			}
		}
		public static object FixFloatingPoint(object value, Type type) {
			if(DataUtils.IsFloatType(type) && value is string)
				return DataUtils.FixFloatingPoint(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
			return value;
		}
		protected static AutoFilterCondition GetColumnAutoFilterCondition(AutoFilterCondition condition, ColumnFilterMode sourceMode, GridColumnEditKind editKind, Type dataType, bool serverMode) {
			if(editKind == GridColumnEditKind.CheckBox)
				return AutoFilterCondition.Equals;
			ColumnFilterMode mode = GetColumnFilterMode(sourceMode, false, serverMode);
			if(condition == AutoFilterCondition.Default)
				return GetDefaultAutoFilterCondition(dataType, editKind, mode, serverMode);
			if(condition == AutoFilterCondition.BeginsWith || condition == AutoFilterCondition.Contains ||
				condition == AutoFilterCondition.DoesNotContain || condition == AutoFilterCondition.EndsWith) { 
				if((editKind == GridColumnEditKind.DateEdit || editKind == GridColumnEditKind.ComboBox) && mode != ColumnFilterMode.DisplayText)
					condition = AutoFilterCondition.Equals;
			}
			return condition;
		}
		public static ColumnFilterMode GetColumnFilterMode(ColumnFilterMode mode, bool isCheckBox, bool serverMode) {
			if(serverMode) return ColumnFilterMode.Value;
			if(isCheckBox) return ColumnFilterMode.Value;
			return mode;
		}
		protected static AutoFilterCondition GetDefaultAutoFilterCondition(Type type, GridColumnEditKind editKind, ColumnFilterMode sourceMode, bool serverMode) {
			if(type != typeof(string) && serverMode)
				return AutoFilterCondition.Equals;
			ColumnFilterMode mode = GetColumnFilterMode(sourceMode, editKind == GridColumnEditKind.CheckBox, serverMode);
			if(mode != ColumnFilterMode.DisplayText && (IsOrdinalKind(type, editKind) || IsSingleOptionKind(type, editKind)))
				return AutoFilterCondition.Equals;
			return AutoFilterCondition.BeginsWith;
		}
		protected FilterRowTypeKind GetFilterRowTypeKind(Type type, GridColumnEditKind editKind, ColumnFilterMode sourceMode, bool serverMode) {
			ColumnFilterMode mode = GetColumnFilterMode(sourceMode, editKind == GridColumnEditKind.CheckBox, serverMode);
			if(mode != ColumnFilterMode.DisplayText) {
				if(IsSingleOptionKind(type, editKind))
					return FilterRowTypeKind.SingleOption;
				if(IsOrdinalKind(type, editKind) || type != typeof(string) && serverMode)
					return FilterRowTypeKind.Ordinal;
			}
			return FilterRowTypeKind.String;
		}
		static bool IsSingleOptionKind(Type type, GridColumnEditKind editKind) {
			return editKind == GridColumnEditKind.ComboBox || editKind == GridColumnEditKind.CheckBox || type == typeof(Boolean);
		}
		static bool IsOrdinalKind(Type type, GridColumnEditKind editKind) {
			return editKind == GridColumnEditKind.DateEdit || type == typeof(DateTime) || DataUtils.IsNumericType(type);
		}
		protected static CriteriaOperator CreateHeaderFilter(string fieldName, Type type, string[] stringValues, bool roundDateTime) {
			List<object> values = new List<object>();
			List<CriteriaOperator> queries = new List<CriteriaOperator>();
			foreach(string stringValue in stringValues) {
				FilterValue filterValue = FilterValue.FromHtmlValue(stringValue);
				if(filterValue.IsFilterByValue && !string.IsNullOrEmpty(filterValue.Value)) {
					values.Add(ChangeTypeSafe(filterValue.Value, type, true));
				} else if(filterValue.IsFilterByQuery) {
					if(filterValue.IsShowAllFilter)
						return null;
					CriteriaOperator op;
					if(filterValue.IsCalendarFilter)
						op = GetHeaderFilterCalendarCriteria(fieldName, filterValue);
					else if(filterValue.IsDateRangePickerFilter)
						op = GetHeaderFilterDateRangePickerCriteria(fieldName, filterValue);
					else
						op = CriteriaOperator.TryParse(filterValue.Query);
					if(!ReferenceEquals(op, null))
						queries.Add(op);
				}
			}
			CriteriaOperator criteria = CreateHeaderFilter(fieldName, type, values, roundDateTime);
			if(!ReferenceEquals(criteria, null))
				queries.Add(criteria);
			return GroupOperator.Or(queries);
		}
		static CriteriaOperator GetHeaderFilterCalendarCriteria(string fieldName, FilterValue filterValue) {
			List<CriteriaOperator> queries = new List<CriteriaOperator>();
			List<DateTime> dateRanges = filterValue.Value.Split('|').Skip(1).ToList().ConvertAll<DateTime>(source => DateTime.Parse(source));
			for(int i = 0; i < dateRanges.Count; i += 2) {
				DateTime start = dateRanges[i];
				DateTime end = dateRanges[i + 1].AddDays(1);
				queries.Add((new OperandProperty(fieldName) >= start) & (new OperandProperty(fieldName) < end));
			}
			return GroupOperator.Or(queries);
		}
		static CriteriaOperator GetHeaderFilterDateRangePickerCriteria(string fieldName, FilterValue filterValue) {
			List<DateTime?> dateRanges = filterValue.Value.Split('|')
				.Skip(1).ToList()
				.ConvertAll<DateTime?>(s => !string.IsNullOrEmpty(s) ? DateTime.Parse(s) : (DateTime?)null);
			var leftOp = dateRanges[0].HasValue ? new OperandProperty(fieldName) >= dateRanges[0] : null;
			var rightOp = dateRanges[1].HasValue ? new OperandProperty(fieldName) < dateRanges[1].Value.AddDays(1) : null;
			return leftOp & rightOp;
		}
		protected static CriteriaOperator CreateHeaderFilter(string fieldName, Type type, List<object> objectValues, bool roundDateTime) {
			type = ReflectionUtils.StripNullableType(type);
			if(type == typeof(DateTime) && roundDateTime) {
				List<DateTime> dates = objectValues.ConvertAll<DateTime>(new Converter<object, DateTime>(ObjectToDateTime));
				return MultiselectRoundedDateTimeFilterHelper.DatesToCriteria(fieldName, dates);
			}
			InOperator op = new InOperator(new OperandProperty(fieldName));
			foreach(object value in objectValues)
				op.Operands.Add(new OperandValue(value));
			switch(op.Operands.Count) {
				case 0:
					return null;
				case 1:
					return op.LeftOperand == op.Operands[0];
				case 2:
					return op.LeftOperand == op.Operands[0] | op.LeftOperand == op.Operands[1];
				default:
					return op;
			}
		}
		static DateTime ObjectToDateTime(object obj) {
			if(obj == null || !(obj is DateTime))
				return DateTime.MinValue;
			return Convert.ToDateTime(obj);
		}
	}
	public class GridFilterHelper : BaseFilterHelper {
		CriteriaOperator searchFilterCriteria = null;
		public GridFilterHelper(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public string ActiveFilter {
			get {
				var criteria = CriteriaOperator.Parse(EnabledFilterExpression) & SearchFilterCriteria;
				return !object.ReferenceEquals(criteria, null) ? criteria.ToString() : string.Empty;
			}
		}
		protected string EnabledFilterExpression { get { return Grid.FilterEnabled ? Grid.FilterExpression : string.Empty; } }
		protected CriteriaOperator SearchFilterCriteria {
			get {
				if(object.ReferenceEquals(searchFilterCriteria, null))
					searchFilterCriteria = CreateSearchFilterCriteria();
				return searchFilterCriteria;
			}
		}
		public virtual CriteriaOperator CreateHeaderFilter(IWebGridDataColumn column, string[] values) {
			var type = column.Adapter.DataType;
			if(column.Adapter.FilterMode == ColumnFilterMode.DisplayText)
				type = typeof(string);
			return CreateHeaderFilter(column.FieldName, type, values, GetEditKind(column) == GridColumnEditKind.DateEdit);
		}
		public List<Tuple<int, int>> GetHighlightTextPositions(IWebGridDataColumn column, string text, bool encode) {
			if(string.IsNullOrEmpty(text) || object.ReferenceEquals(SearchFilterCriteria, null) || HightlightHelper == null)
				return new List<Tuple<int, int>>(0);
			return HightlightHelper.GetHighlightTextPositions(text, encode, column.FieldName);
		}
		public void Invalidate() {
			this.searchFilterCriteria = null;
			HightlightHelper = null;
		}
		protected virtual GridSearchPanelHighlighHelper HightlightHelper { get; set; }
		protected virtual bool IsServerMode { get { return DataProxy.IsServerMode; } }
		protected virtual string SearchPanelFilter { get { return Grid.SearchPanelFilter; } }
		protected virtual ICollection SearchPanelColumnInfos { get { return ColumnHelper.SearchPanelColumnInfos; } }
		protected virtual GridViewSearchPanelGroupOperator SearchPanelGroupOperator { get { return Grid.SettingsSearchPanel.GroupOperator; } }
		protected virtual CriteriaOperator CreateSearchFilterCriteria() {
			HightlightHelper = null;
			if(string.IsNullOrEmpty(Grid.SearchPanelFilter))
				return null;
			var parseResult = ParseSearchPanelFilter();
			HightlightHelper = new GridSearchPanelHighlighHelper(parseResult);
			return CreateCriteriaOperator(parseResult, FilterCondition.Contains, IsServerMode);
		}
		protected virtual FindSearchParserResults ParseSearchPanelFilter() {
			return ParseSearchPanelFilter(SearchPanelFilter, SearchPanelColumnInfos, SearchPanelGroupOperator, !IsServerMode);
		}
		protected internal string GetSearchPanelColumnInfoKey() {
			if(ColumnHelper.SearchPanelColumnInfos.Count == 0)
				return string.Empty;
			return string.Join(";", ColumnHelper.SearchPanelColumnInfos.Select(i => i.Name).OrderBy(i => i).ToArray());
		}
		public GridColumnEditKind GetEditKind(IWebGridDataColumn column) {
			var prop = Grid.RenderHelper.GetColumnEdit(column);
			if(prop is CheckBoxProperties) return GridColumnEditKind.CheckBox;
			if(column.Adapter.FilterMode != ColumnFilterMode.DisplayText) {
				if(prop is DateEditProperties) return GridColumnEditKind.DateEdit;
				if(prop is ComboBoxProperties) return GridColumnEditKind.ComboBox;
			} else {
				return GridColumnEditKind.Text;
			}
			return GridColumnEditKind.Text;
		}
		public static CriteriaOperator CreateCriteriaOperator(FindSearchParserResults parseResult, FilterCondition filterCondition, bool isServerMode) {
			return DxFtsContainsHelperAlt.Create(parseResult, filterCondition, isServerMode);
		}
		public static FindSearchParserResults ParseSearchPanelFilter(string text, ICollection columns, GridViewSearchPanelGroupOperator groupOperator, bool needAppendColumnFieldPrefixes) {
			var parseResult = new FindSearchParser().Parse(text, columns);
			ApplyGroupSettings(parseResult, groupOperator);
			if(needAppendColumnFieldPrefixes)
				parseResult.AppendColumnFieldPrefixes();
			return parseResult;
		}
		protected static void ApplyGroupSettings(FindSearchParserResults parseResult, GridViewSearchPanelGroupOperator groupOperator) {
			if(groupOperator != GridViewSearchPanelGroupOperator.And)
				return;
			AddPlusSymbolToSearchText(parseResult.SearchTexts);
			foreach(var field in parseResult.Fields)
				AddPlusSymbolToSearchText(field.Values);
		}
		static void AddPlusSymbolToSearchText(string[] texts) {
			for(int i = 0; i < texts.Length; i++) {
				var text = texts[i];
				if(!string.IsNullOrEmpty(text) && text.Length > 0 && text[0] != '+' && text[0] != '-')
					texts[i] = "+" + text;
			}
		}
	}
	public class GridHighlightTextProcessor : IHighlightTextProcessor {
		public GridHighlightTextProcessor(ASPxGridBase grid, IWebGridDataColumn column) {
			Grid = grid;
			Column = column;
		}
		public ASPxGridBase Grid { get; private set; }
		public IWebGridDataColumn Column { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		string IHighlightTextProcessor.HighlightText(string text, bool encode) {
			return RenderHelper.HighlightSearchPanelText(Column, text, encode);
		}
	}
	public class GridSearchPanelHighlighHelper {
		static Regex HtmlEncodeRegex = new Regex("&#?[a-z0-9]{2,10};", RegexOptions.IgnoreCase);
		public GridSearchPanelHighlighHelper(FindSearchParserResults parseResult) {
			ParseResult = parseResult;
			SearchText = RemoveSpecialSymbols(ParseResult.SearchTexts);
			EncodedSearchText = SearchText.Select(t => HttpUtility.HtmlEncode(t)).ToList();
			ColumnSearchText = ParseResult.Fields.ToDictionary(f => f.Name.Replace(DxFtsContainsHelper.DxFtsPropertyPrefix, string.Empty), f => RemoveSpecialSymbols(f.Values));
			EncodedColumnSearchText = ColumnSearchText.ToDictionary(p => p.Key, p => p.Value.Select(t => HttpUtility.HtmlEncode(t)).ToList());
		}
		protected FindSearchParserResults ParseResult { get; private set; }
		public List<string> SearchText { get; private set; }
		public List<string> EncodedSearchText { get; private set; }
		public Dictionary<string, List<string>> ColumnSearchText { get; private set; }
		public Dictionary<string, List<string>> EncodedColumnSearchText { get; private set; }
		public List<Tuple<int, int>> GetHighlightTextPositions(string text) {
			return GetHighlightTextPositions(text, false);
		}
		public List<Tuple<int, int>> GetHighlightTextPositions(string text, bool encode) {
			return GetHighlightTextPositions(text, encode, string.Empty);
		}
		public List<Tuple<int, int>> GetHighlightTextPositions(string text, bool encode, string fieldName) {
			var result = new List<Tuple<int, int>>();
			text = text.ToLower();
			var searchParts = GetSearchParts(fieldName, encode);
			var encodedParts = GetEncodedParts(text, encode);
			foreach(var part in searchParts) {
				var startIndex = text.IndexOf(part);
				while(startIndex > -1) {
					var endIndex = startIndex + part.Length;
					if(!encodedParts.Any(p => p.Value != part && startIndex >= p.Start && endIndex <= p.End && p.Encoded))
						result.Add(new Tuple<int, int>(startIndex, endIndex));
					startIndex = text.IndexOf(part, endIndex);
				}
			}
			return NormalizePositions(result);
		}
		protected virtual List<Tuple<int, int>> NormalizePositions(List<Tuple<int, int>> source) {
			if(source.Count < 2)
				return source;
			var result = new List<Tuple<int, int>>(source.Count);
			var sortedList = source.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
			Tuple<int, int> sumPart = sortedList[0];
			for(int i = 1; i < sortedList.Count; i++) {
				var currentPart = sortedList[i];
				if(sumPart.Item2 >= currentPart.Item1) {
					sumPart = new Tuple<int, int>(sumPart.Item1, Math.Max(sumPart.Item2, currentPart.Item2));
				} else {
					result.Add(sumPart);
					sumPart = currentPart;
				}
			}
			result.Add(sumPart);
			return result;
		}
		protected virtual List<string> GetSearchParts(string fieldName, bool encode) {
			var result = new List<string>();
			result.AddRange(encode ? EncodedSearchText : SearchText);
			var columnSearchTexts = encode ? EncodedColumnSearchText : ColumnSearchText;
			if(columnSearchTexts.ContainsKey(fieldName))
				result.AddRange(columnSearchTexts[fieldName]);
			return result;
		}
		static List<string> RemoveSpecialSymbols(string[] parts) {
			var result = new List<string>();
			for(int i = 0; i < parts.Length; i++) {
				var part = parts[i];
				if(string.IsNullOrEmpty(part) || part[0] == '-')
					continue;
				if(part[0] == '+')
					part = part.Substring(1);
				if(!string.IsNullOrEmpty(part))
					result.Add(part);
			}
			return result;
		}
		static List<HtmlEncodePartInfo> GetEncodedParts(string text, bool encode) {
			if(!encode || !text.Contains('&'))
				return new List<HtmlEncodePartInfo>(0);
			return HtmlEncodeRegex.Matches(text).OfType<Match>().Where(m => m.Success).Select(m => new HtmlEncodePartInfo(m)).ToList();
		}
		class HtmlEncodePartInfo {
			Match Match;
			bool? encoded;
			public HtmlEncodePartInfo(Match match) {
				Match = match;
				Value = Match.Value;
			}
			public int Start { get { return Match.Index; } }
			public int End { get { return Start + Match.Length; } }
			public string Value { get; private set; }
			public bool Encoded {
				get {
					if(!encoded.HasValue)
						encoded = Value != HttpUtility.HtmlDecode(Value);
					return encoded.Value;
				}
			}
		}
	}
	public class GridHeaderFilterHelper {
		GridHeaderFilterValues filterValues;
		object[] uniqueValues;
		Type dataType;
		bool includeFilteredOut;
		List<object> activeValues;
		List<string> activeQueries;
		public GridHeaderFilterHelper(IWebGridDataColumn column, bool includeFilteredOut) {
			Column = column;
			this.includeFilteredOut = includeFilteredOut;
		}
		internal GridHeaderFilterHelper() { }
		public GridHeaderFilterValues FilterValues {
			get {
				if(filterValues == null) {
					filterValues = CreateFilterValues();
				}
				return filterValues;
			}
		}
		protected GridColumnDateRangePeriodsSettings DateRangePeriodsVisibility { 
			get { return Column.Adapter.SettingsHeaderFilter.DateRangePeriodsSettings; } 
		}
		protected internal List<int> SeparatorIndeces { get { return FilterValues.SeparatorIndeces; } }
		public bool IsFilterValueActive(FilterValue filterValue) {
			EnsureActiveFilterValues();
			if(!IsMultiSelect && filterValue.IsShowAllFilter)
				return ActiveQueries.Count == 0 && ActiveValues.Count == 0;
			if(filterValue.IsCalendarFilter)
				return GetActiveDateRanges().Count > 0;
			if(filterValue.IsDateRangePickerFilter)
				return GetDateRangePickerBoundaries() != null;
			if(filterValue.IsFilterByQuery) {
				if(ActiveQueries.Count == 0)
					return false;
				CriteriaOperator query = CriteriaOperator.Parse(filterValue.Query);
				return ActiveQueries.Contains(query.ToString());
			}
			object value = CertifyValueType(filterValue.Value);
			return ActiveValues.Contains(value);
		}
		public Tuple<DateTime?, DateTime?> GetDateRangePickerBoundaries() {
			EnsureActiveFilterValues();
			Tuple<DateTime?, DateTime?> range = new Tuple<DateTime?, DateTime?>(null, null);
			foreach(string query in ActiveQueries) {
				DateTime start, end;
				var criteria = CriteriaOperator.Parse(query);
				if(TryGetActiveDateRangeValues(criteria as GroupOperator, out start, out end))
					return new Tuple<DateTime?, DateTime?>(start, end);
				if(IsGreaterOrEqualDateTimeOperator(criteria as BinaryOperator, out start))
					return new Tuple<DateTime?, DateTime?>(start, null);
				if(IsLessDateTimeOperator(criteria as BinaryOperator, out end))
					return new Tuple<DateTime?, DateTime?>(null, end);
			}
			return null;
		}
		public List<Tuple<DateTime, DateTime>> GetActiveDateRanges() {
			EnsureActiveFilterValues();
			List<Tuple<DateTime, DateTime>> ranges = new List<Tuple<DateTime, DateTime>>();
			foreach(string query in ActiveQueries) {
				var op = CriteriaOperator.Parse(query) as GroupOperator;
				if(ReferenceEquals(op, null)) continue;
				DateTime start, end;
				if(TryGetActiveDateRangeValues(op, out start, out end))
					ranges.Add(new Tuple<DateTime, DateTime>(start, end));
			}
			return ranges;
		}
		protected List<object> ActiveValues {
			get {
				if(activeValues == null)
					activeValues = new List<object>();
				return activeValues;
			}
		}
		protected List<string> ActiveQueries {
			get {
				if(activeQueries == null)
					activeQueries = new List<string>();
				return activeQueries;
			}
		}
		protected Type DataType {
			get {
				if(dataType == null)
					dataType = GetDataType();
				return dataType;
			}
		}
		protected object[] UniqueValues {
			get {
				if(uniqueValues == null)
					uniqueValues = GetUniqueValues();
				return uniqueValues;
			}
		}
		protected IWebGridDataColumn Column { get; private set; }
		protected ASPxGridBase Grid { get { return Column.Adapter.Grid; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected virtual string FieldName { get { return Column.FieldName; } }
		protected virtual bool IsMultiSelect { get { return Column.Adapter.IsMultiSelectHeaderFilter; } }
		protected internal virtual bool ShowPredefinedDateRanges { get { return Column.Adapter.IsDateRangeHeaderFilterMode; } }
		protected virtual CriteriaOperator ActiveFilter { get { return Grid.GetColumnFilter(Column); } }
		protected bool IncludeFilteredOut {
			get { return includeFilteredOut || !string.IsNullOrEmpty(Column.Adapter.FilterExpression); }
		}
		protected virtual bool ShowBlankItems { get { return !IsMultiSelect && Grid.Settings.ShowHeaderFilterBlankItems; } }
		protected virtual FilterValue ShowAllItem { get { return FilterValue.CreateShowAllValue(ShowAllText); } }
		protected virtual FilterValue BlankItem { get { return FilterValue.GetBlanksValue(Column, BlankItemText, false); } }
		protected virtual FilterValue NonBlankItem { get { return FilterValue.GetBlanksValue(Column, NonBlankItemText, true); } }
		protected virtual string ShowAllText { get { return Grid.SettingsText.GetHeaderFilterShowAll(); } }
		protected virtual string BlankItemText { get { return Grid.SettingsText.GetHeaderFilterShowBlanks(); } }
		protected virtual string NonBlankItemText { get { return Grid.SettingsText.GetHeaderFilterShowNonBlanks(); } }
		protected virtual string YesterdayText { get { return Grid.SettingsText.GetHeaderFilterYesterday(); } }
		protected virtual string TodayText { get { return Grid.SettingsText.GetHeaderFilterToday(); } }
		protected virtual string TomorrowText { get { return Grid.SettingsText.GetHeaderFilterTomorrow(); } }
		protected virtual string LastWeekText { get { return Grid.SettingsText.GetHeaderFilterLastWeek(); } }
		protected virtual string ThisWeekText { get { return Grid.SettingsText.GetHeaderFilterThisWeek(); } }
		protected virtual string NextWeekText { get { return Grid.SettingsText.GetHeaderFilterNextWeek(); } }
		protected virtual string LastMonthText { get { return Grid.SettingsText.GetHeaderFilterLastMonth(); } }
		protected virtual string ThisMonthText { get { return Grid.SettingsText.GetHeaderFilterThisMonth(); } }
		protected virtual string NextMonthText { get { return Grid.SettingsText.GetHeaderFilterNextMonth(); } }
		protected virtual string LastYearText { get { return Grid.SettingsText.GetHeaderFilterLastYear(); } }
		protected virtual string ThisYearText { get { return Grid.SettingsText.GetHeaderFilterThisYear(); } }
		protected virtual string NextYearText { get { return Grid.SettingsText.GetHeaderFilterNextYear(); } }
		protected virtual bool IsDateTime { get { return DataType == typeof(DateTime); } }
		protected virtual GridHeaderFilterValues CreateFilterValues() {
			var args = RaiseBeforeHeaderFilterFillItems();
			if(args.Handled)
				return args.Values;
			GridHeaderFilterValues result = new GridHeaderFilterValues();
			if(!IsMultiSelect)
				result.Add(ShowAllItem);
			if(ShowBlankItems) {
				result.Add(BlankItem);
				result.Add(NonBlankItem);
			}
			if(UniqueValues != null) {
				if(ShowPredefinedDateRanges)
					PopulateDateRangeFilterValues(result);
				else
					PopulateFilterValues(result);
			}
			RaiseHeaderFilterFillItems(result);
			return result;
		}
		void PopulateDateRangeFilterValues(GridHeaderFilterValues result) {
			if(DateRangePeriodsVisibility.ShowDaysSection) {
				if(DateRangePeriodsVisibility.ShowPastPeriods)
					result.Add(FilterValue.CreateYesterdayValue(Column, YesterdayText));
				if(DateRangePeriodsVisibility.ShowPresentPeriods)
					result.Add(FilterValue.CreateTodayValue(Column, TodayText));
				if(DateRangePeriodsVisibility.ShowFuturePeriods)
					result.Add(FilterValue.CreateTomorrowValue(Column, TomorrowText));
				if(DateRangePeriodsVisibility.ShowWeeksSection || DateRangePeriodsVisibility.ShowMonthsSection || DateRangePeriodsVisibility.ShowYearsSection)
					result.AddSeparator();
			}
			if(DateRangePeriodsVisibility.ShowWeeksSection) {
				if(DateRangePeriodsVisibility.ShowPastPeriods)
					result.Add(FilterValue.CreateLastWeekValue(Column, LastWeekText));
				if(DateRangePeriodsVisibility.ShowPresentPeriods)
					result.Add(FilterValue.CreateThisWeekValue(Column, ThisWeekText));
				if(DateRangePeriodsVisibility.ShowFuturePeriods)
					result.Add(FilterValue.CreateNextWeekValue(Column, NextWeekText));
				if(DateRangePeriodsVisibility.ShowMonthsSection || DateRangePeriodsVisibility.ShowYearsSection)
					result.AddSeparator();
			}
			if(DateRangePeriodsVisibility.ShowMonthsSection) {
				if(DateRangePeriodsVisibility.ShowPastPeriods)
					result.Add(FilterValue.CreateLastMonthValue(Column, LastMonthText));
				if(DateRangePeriodsVisibility.ShowPresentPeriods)
					result.Add(FilterValue.CreateThisMonthValue(Column, ThisMonthText));
				if(DateRangePeriodsVisibility.ShowFuturePeriods)
					result.Add(FilterValue.CreateNextMonthValue(Column, NextMonthText));
				if(DateRangePeriodsVisibility.ShowYearsSection)
					result.AddSeparator();
			}
			if(DateRangePeriodsVisibility.ShowYearsSection) {
				if(DateRangePeriodsVisibility.ShowPastPeriods)
					result.Add(FilterValue.CreateLastYearValue(Column, LastYearText));
				if(DateRangePeriodsVisibility.ShowPresentPeriods)
					result.Add(FilterValue.CreateThisYearValue(Column, ThisYearText));
				if(DateRangePeriodsVisibility.ShowFuturePeriods)
					result.Add(FilterValue.CreateNextYearValue(Column, NextYearText));
			}
		}
		void PopulateFilterValues(GridHeaderFilterValues result) {
			foreach(object value in UniqueValues) {
				string stringValue = GetStringValue(value);
				if(!IsMultiSelect && string.IsNullOrEmpty(stringValue)) {
					if(ShowBlankItems)
						continue;
					result.Add(BlankItem);
				} else {
					result.Add(new FilterValue(GetDisplayText(value, stringValue), stringValue));
				}
			}
		}
		protected virtual ASPxGridBeforeHeaderFilterFillItemsEventArgs RaiseBeforeHeaderFilterFillItems() {
			var args = Grid.CreateBeforeHeaderFilterFillItemsEventArgs(Column);
			Grid.RaiseBeforeHeaderFilterFillItems(args);
			return args;
		}
		protected virtual void RaiseHeaderFilterFillItems(GridHeaderFilterValues result) {
			Grid.RaiseHeaderFilterFillItems(Grid.CreateHeaderFilterFillItemsEventArgs(Column, result));
		}
		protected virtual Type GetDataType() {
			if(Column.Adapter.FilterMode == ColumnFilterMode.DisplayText)
				return typeof(string);
			return Column.Adapter.DataType;
		}
		protected virtual object[] GetUniqueValues() {
			Grid.DataBindNoControls();
			return Grid.DataProxy.GetUniqueColumnValues(FieldName, Grid.SettingsBehavior.HeaderFilterMaxRowCount, IncludeFilteredOut);
		}
		protected virtual string GetStringValue(object value) {
			string result = "";
			if(value == null || value == DBNull.Value)
				return result;
			try {
				result = value.ToString();
			} catch { }
			return result;
		}
		protected virtual string GetDisplayText(object value, string stringValue) {
			if(Column.Adapter.FilterMode == ColumnFilterMode.DisplayText)
				return stringValue;
			return RenderHelper.TextBuilder.GetFilterPopupItemText(Column, value);
		}
		bool activeValuesPopuplated = false;
		protected virtual void EnsureActiveFilterValues() {
			if(activeValuesPopuplated)
				return;
			activeValuesPopuplated = true;
			CriteriaOperator op = ActiveFilter;
			if(IsNull(op))
				return;
			ActiveQueries.Add(op.ToString());
			if(IsMultiSelect && IsDateTime)
				ActiveValues.AddRange(GetActiveDateValues(op));
			GroupOperator grOp = op as GroupOperator;
			if(!IsNull(grOp)) {
				ProcessGroupOperator(grOp);
				return;
			}
			BinaryOperator binOp = op as BinaryOperator;
			if(!IsNull(binOp)) {
				ProcessBinaryOperator(binOp);
				return;
			}
			InOperator inOp = op as InOperator;
			if(!IsNull(inOp) && IsMultiSelect) {
				ProcessInOperator(inOp);
				return;
			}
		}
		void ProcessGroupOperator(GroupOperator op) {
			if(op.OperatorType == GroupOperatorType.And) {
				ProcessAndGroupOperator(op);
				return;
			}
			if(IsMultiSelect) {
				ProcessOrGroupOperator(op);
				return;
			}
			ActiveQueries.Add(op.ToString());
		}
		void ProcessAndGroupOperator(GroupOperator op) {
			object value;
			if(IsDateTime && TryGetActiveDateValue(op, out value))
				ActiveValues.Add(CertifyValueType(value));
			ActiveQueries.Add(op.ToString());
		}
		void ProcessOrGroupOperator(GroupOperator op) {
			foreach(CriteriaOperator operand in op.Operands) {
				BinaryOperator binOp = operand as BinaryOperator;
				if(!IsNull(binOp)) {
					ProcessBinaryOperator(binOp);
					continue;
				}
				InOperator inOp = operand as InOperator;
				if(!IsNull(inOp)) {
					ProcessInOperator(inOp);
					continue;
				}
				ActiveQueries.Add(operand.ToString());
			}
		}
		void ProcessBinaryOperator(BinaryOperator op) {
			object value;
			if(!IsDateTime && TryGetActiveValue(op, out value))
				ActiveValues.Add(CertifyValueType(value));
			ActiveQueries.Add(op.ToString());
		}
		void ProcessInOperator(InOperator op) {
			object[] values;
			if(!IsDateTime && TryGetActiveValues(op, out values))
				ActiveValues.AddRange(CertifyValueType(values));
			ActiveQueries.Add(op.ToString());
		}
		protected IEnumerable<object> GetActiveDateValues(CriteriaOperator criteria) {
			List<DateTime> dates = new List<DateTime>();
			foreach(FilterValue filterValue in FilterValues) {
				if(filterValue.IsFilterByValue && !string.IsNullOrEmpty(filterValue.Value)) {
					object val = CertifyValueType(filterValue.Value);
					if(val is DateTime)
						dates.Add((DateTime)val);
				}
			}
			List<object> result = new List<object>();
			foreach(DateTime date in MultiselectRoundedDateTimeFilterHelper.GetCheckedDates(criteria, FieldName, dates))
				result.Add(date);
			return result;
		}
		protected static bool TryGetActiveValue(BinaryOperator op, out object value) {
			value = null;
			if(op.OperatorType == BinaryOperatorType.Equal) {
				OperandValue opValue = op.RightOperand as OperandValue;
				if(!IsNull(opValue)) {
					value = opValue.Value;
					return true;
				}
			}
			return false;
		}
		protected static bool TryGetActiveValues(InOperator op, out object[] values) {
			values = null;
			if(op.Operands.Count > 1) {
				List<object> list = new List<object>();
				foreach(CriteriaOperator operand in op.Operands) {
					OperandValue opValue = operand as OperandValue;
					if(!IsNull(opValue))
						list.Add(opValue.Value);
				}
				if(list.Count == op.Operands.Count) {
					values = list.ToArray();
					return true;
				}
			}
			return false;
		}
		protected static bool TryGetActiveDateRangeValues(GroupOperator op, out DateTime startDate, out DateTime endDate) {
			startDate = DateTime.MinValue;
			endDate = DateTime.MinValue;
			if(ReferenceEquals(op, null))
				return false;
			if(op.OperatorType != GroupOperatorType.And || op.Operands.Count != 2)
				return false;
			if(!IsGreaterOrEqualDateTimeOperator(op.Operands[0] as BinaryOperator, out startDate))
				return false;
			if(!IsLessDateTimeOperator(op.Operands[1] as BinaryOperator, out endDate))
				return false;
			return true;
		}
		protected static bool TryGetActiveDateValue(GroupOperator op, out object value) {
			value = null;
			DateTime date1, date2;
			if(!TryGetActiveDateRangeValues(op, out date1, out date2))
				return false;
			if(date1.Date.AddDays(1) == date2.Date)
				value = date1.Date;
			return value != null;
		}
		static bool IsGreaterOrEqualDateTimeOperator(BinaryOperator binaryOperator, out DateTime date) {
			return IsBinaryDateTimeOperatorCore(binaryOperator, BinaryOperatorType.GreaterOrEqual, out date);
		}
		static bool IsLessDateTimeOperator(BinaryOperator binaryOperator, out DateTime date) {
			return IsBinaryDateTimeOperatorCore(binaryOperator, BinaryOperatorType.Less, out date);
		}
		static bool IsBinaryDateTimeOperatorCore(BinaryOperator binaryOperator, BinaryOperatorType operatorType, out DateTime date) {
			date = DateTime.MinValue;
			if(IsNull(binaryOperator) || binaryOperator.OperatorType != operatorType)
				return false;
			OperandValue opValue = binaryOperator.RightOperand as OperandValue;
			if(IsNull(opValue) || opValue.Value == null || !(opValue.Value is DateTime))
				return false;
			try {
				date = Convert.ToDateTime(opValue.Value);
			} catch {
				return false;
			}
			return true;
		}
		object[] CertifyValueType(object[] values) {
			for(int i = 0; i < values.Length; i++)
				values[i] = CertifyValueType(values[i]);
			return values;
		}
		object CertifyValueType(object value) {
			return BaseFilterHelper.ChangeTypeSafe(value, DataType, IsDateTime);
		}
		static bool IsNull(CriteriaOperator op) {
			return ReferenceEquals(op, null);
		}
	}
	class FilterEvaluatorContext : EvaluatorContextDescriptor {
		public object Value { get; set; }
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) { return Value; }
		public override IEnumerable GetCollectionContexts(object source, string collectionName) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
