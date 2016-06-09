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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotAddFilterCommand
	public class PivotAddFilterCommand : PivotTableTransactedCommand {
		#region Static
		static Dictionary<PivotFilterType, DynamicFilterType> dynamicFilterTypes = CreateDynamicFilterTypesTable();
		static Dictionary<PivotFilterType, DynamicFilterType> CreateDynamicFilterTypesTable() {
			Dictionary<PivotFilterType, DynamicFilterType> result = new Dictionary<PivotFilterType, DynamicFilterType>();
			result.Add(PivotFilterType.LastWeek, DynamicFilterType.LastWeek);
			result.Add(PivotFilterType.LastMonth, DynamicFilterType.LastMonth);
			result.Add(PivotFilterType.LastQuarter, DynamicFilterType.LastQuarter);
			result.Add(PivotFilterType.LastYear, DynamicFilterType.LastYear);
			result.Add(PivotFilterType.ThisWeek, DynamicFilterType.ThisWeek);
			result.Add(PivotFilterType.ThisMonth, DynamicFilterType.ThisMonth);
			result.Add(PivotFilterType.ThisQuarter, DynamicFilterType.ThisQuarter);
			result.Add(PivotFilterType.ThisYear, DynamicFilterType.ThisYear);
			result.Add(PivotFilterType.NextWeek, DynamicFilterType.NextWeek);
			result.Add(PivotFilterType.NextMonth, DynamicFilterType.NextMonth);
			result.Add(PivotFilterType.NextQuarter, DynamicFilterType.NextQuarter);
			result.Add(PivotFilterType.NextYear, DynamicFilterType.NextYear);
			result.Add(PivotFilterType.FirstQuarter, DynamicFilterType.Q1);
			result.Add(PivotFilterType.SecondQuarter, DynamicFilterType.Q2);
			result.Add(PivotFilterType.ThirdQuarter, DynamicFilterType.Q3);
			result.Add(PivotFilterType.FourthQuarter, DynamicFilterType.Q4);
			result.Add(PivotFilterType.January, DynamicFilterType.M1);
			result.Add(PivotFilterType.February, DynamicFilterType.M2);
			result.Add(PivotFilterType.March, DynamicFilterType.M3);
			result.Add(PivotFilterType.April, DynamicFilterType.M4);
			result.Add(PivotFilterType.May, DynamicFilterType.M5);
			result.Add(PivotFilterType.June, DynamicFilterType.M6);
			result.Add(PivotFilterType.July, DynamicFilterType.M7);
			result.Add(PivotFilterType.August, DynamicFilterType.M8);
			result.Add(PivotFilterType.September, DynamicFilterType.M9);
			result.Add(PivotFilterType.October, DynamicFilterType.M10);
			result.Add(PivotFilterType.November, DynamicFilterType.M11);
			result.Add(PivotFilterType.December, DynamicFilterType.M12);
			result.Add(PivotFilterType.Yesterday, DynamicFilterType.Yesterday);
			result.Add(PivotFilterType.Today, DynamicFilterType.Today);
			result.Add(PivotFilterType.Tomorrow, DynamicFilterType.Tomorrow);
			result.Add(PivotFilterType.YearToDate, DynamicFilterType.YearToDate);
			return result;
		}
		#endregion
		#region Fields
		readonly int fieldIndex;
		readonly PivotFilterType filterType;
		PivotFilter pivotFilter;
		VariantValue value1;
		VariantValue value2;
		bool value1IsDate;
		bool value2IsDate;
		bool filterByTop;
		bool shouldUnHideItems;
		int measureFieldIndex;
		#endregion
		public PivotAddFilterCommand(PivotTable pivotTable, int fieldIndex, PivotFilterType filterType, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.fieldIndex = fieldIndex;
			this.filterType = filterType;
			this.value1 = VariantValue.Missing;
			this.value2 = VariantValue.Missing;
			this.measureFieldIndex = -1;
			this.filterByTop = true;
		}
		#region Properties
		public VariantValue FirstValue { get { return value1; } set { value1 = value; } }
		public bool FirstValueIsDate { get { return value1IsDate; } set { value1IsDate = value; } }
		public VariantValue SecondValue { get { return value2; } set { value2 = value; } }
		public bool SecondValueIsDate { get { return value2IsDate; } set { value2IsDate = value; } }
		public int MeasureFieldIndex { get { return measureFieldIndex; } set { measureFieldIndex = value; } }
		public bool FilterByTop { get { return filterByTop; } set { filterByTop = value; } }
		public bool ShouldUnHideItems { get { return shouldUnHideItems; } set { shouldUnHideItems = value; } }
		AutoFilterColumn FilterColumn { get { return pivotFilter.AutoFilter.FilterColumns[0]; } }
		WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		CultureInfo Culture { get { return Context.Culture; } }
		bool ShouldPreserveStringValue {
			get {
				PivotCacheSharedItemsCollection sharedItems = PivotTable.Cache.CacheFields[fieldIndex].SharedItems;
				return sharedItems.ContainsString || sharedItems.ContainsMixedTypes;
			}
		}
		#endregion
		protected internal override void ExecuteCore() {
			Debug.Assert(!value1.IsArray && !value2.IsArray && !value1.IsCellRange && !value2.IsCellRange && !value1.IsEmpty && !value2.IsEmpty);
			ClearExistingFilters();
			CreateFilter();
			PivotTable.Filters.Add(pivotFilter);
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void ClearExistingFilters() {
			if (PivotTable.MultipleFieldFilters) {
				PivotFilterClearType clearType = PivotFilter.GetIsMeasureFilter(filterType) ? PivotFilterClearType.Value : PivotFilterClearType.Label;
				PivotTable.ClearFieldFilters(fieldIndex, clearType, ErrorHandler);
			}
			else
				PivotTable.ClearFieldFilters(fieldIndex, ShouldUnHideItems ? PivotFilterClearType.All : PivotFilterClearType.AllExceptManual, ErrorHandler);
		}
		#region CreateFilter
		void CreateFilter() {
			pivotFilter = new PivotFilter(DocumentModel);
			pivotFilter.FilterType = filterType;
			pivotFilter.FieldIndex = fieldIndex;
			pivotFilter.EvalOrder = -1;
			if (measureFieldIndex >= 0) {
				pivotFilter.MeasureFieldIndex = measureFieldIndex;
				PivotTable.Fields[fieldIndex].MeasureFilter = true;
			}
			CreateFilterCore();
		}
		void CreateFilterCore() {
			switch (filterType) {
				case PivotFilterType.DateEqual:
				case PivotFilterType.ValueEqual:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.Equal);
					break;
				case PivotFilterType.CaptionEqual:
					AddLabelFilter(FilterComparisonOperator.Equal);
					break;
				case PivotFilterType.DateNotEqual:
				case PivotFilterType.ValueNotEqual:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.NotEqual);
					break;
				case PivotFilterType.CaptionNotEqual:
					AddSingleValueCaptionCustomFilter(FilterComparisonOperator.NotEqual);
					break;
				case PivotFilterType.DateNewerThan:
				case PivotFilterType.ValueGreaterThan:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.GreaterThan);
					break;
				case PivotFilterType.CaptionGreaterThan:
					AddSingleValueCaptionCustomFilter(FilterComparisonOperator.GreaterThan);
					break;
				case PivotFilterType.DateNewerThanOrEqual:
				case PivotFilterType.ValueGreaterThanOrEqual:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.GreaterThanOrEqual);
					break;
				case PivotFilterType.CaptionGreaterThanOrEqual:
					AddSingleValueCaptionCustomFilter(FilterComparisonOperator.GreaterThanOrEqual);
					break;
				case PivotFilterType.DateOlderThan:
				case PivotFilterType.ValueLessThan:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.LessThan);
					break;
				case PivotFilterType.CaptionLessThan:
					AddSingleValueCaptionCustomFilter(FilterComparisonOperator.LessThan);
					break;
				case PivotFilterType.DateOlderThanOrEqual:
				case PivotFilterType.ValueLessThanOrEqual:
					AddSingleValueNumericCustomFilter(FilterComparisonOperator.LessThanOrEqual);
					break;
				case PivotFilterType.CaptionLessThanOrEqual:
					AddSingleValueCaptionCustomFilter(FilterComparisonOperator.LessThanOrEqual);
					break;
				case PivotFilterType.DateBetween:
				case PivotFilterType.ValueBetween:
					AddTwoValueNumericCustomFilter(FilterComparisonOperator.GreaterThanOrEqual, FilterComparisonOperator.LessThanOrEqual, true);
					break;
				case PivotFilterType.CaptionBetween:
					AddTwoValueCaptionCustomFilter(FilterComparisonOperator.GreaterThanOrEqual, FilterComparisonOperator.LessThanOrEqual, true);
					break;
				case PivotFilterType.DateNotBetween:
				case PivotFilterType.ValueNotBetween:
					AddTwoValueNumericCustomFilter(FilterComparisonOperator.LessThan, FilterComparisonOperator.GreaterThan, false);
					break;
				case PivotFilterType.CaptionNotBetween:
					AddTwoValueCaptionCustomFilter(FilterComparisonOperator.LessThan, FilterComparisonOperator.GreaterThan, false);
					break;
				case PivotFilterType.CaptionBeginsWith:
					AddSingleValueCaptionCustomFilter(GetFirstStringValue() + "*", FilterComparisonOperator.Equal);
					break;
				case PivotFilterType.CaptionNotBeginsWith:
					AddSingleValueCaptionCustomFilter(GetFirstStringValue() + "*", FilterComparisonOperator.NotEqual);
					break;
				case PivotFilterType.CaptionEndsWith:
					AddSingleValueCaptionCustomFilter("*" + GetFirstStringValue(), FilterComparisonOperator.Equal);
					break;
				case PivotFilterType.CaptionNotEndsWith:
					AddSingleValueCaptionCustomFilter("*" + GetFirstStringValue(), FilterComparisonOperator.NotEqual);
					break;
				case PivotFilterType.CaptionContains:
					AddSingleValueCaptionCustomFilter("*" + GetFirstStringValue() + "*", FilterComparisonOperator.Equal);
					break;
				case PivotFilterType.CaptionNotContains:
					AddSingleValueCaptionCustomFilter("*" + GetFirstStringValue() + "*", FilterComparisonOperator.NotEqual);
					break;
				case PivotFilterType.Count:
				case PivotFilterType.Percent:
				case PivotFilterType.Sum:
					AddTop10Filter();
					break;
				default:
					AddDynamicFilter();
					break;
			}
		}
		string GetFirstStringValue() {
			return VariantToString(value1, value1IsDate);
		}
		string GetSecondStringValue() {
			return VariantToString(value2, value2IsDate);
		}
		void AddLabelFilter(FilterComparisonOperator filterOperator) {
			string stringValue1 = GetFirstStringValue();
			pivotFilter.LabelPivot = stringValue1;
			FilterCriteria criteria = FilterColumn.FilterCriteria;
			if (String.IsNullOrEmpty(stringValue1))
				criteria.FilterByBlank = true;
			else if ((ShouldPreserveStringValue || !value1.IsNumeric) && !CustomFilter.ContainsWildcardCharacters(stringValue1))
				criteria.Filters.Add(stringValue1);
			else
				AddCustomFilter(value1, stringValue1, value1IsDate, filterOperator);
		}
		void AddSingleValueCaptionCustomFilter(FilterComparisonOperator filterOperator) {
			string stringValue1 = GetFirstStringValue();
			pivotFilter.LabelPivot = stringValue1;
			AddCustomFilter(value1, stringValue1, value1IsDate, filterOperator);
		}
		void AddSingleValueCaptionCustomFilter(string customFilterValue, FilterComparisonOperator filterOperator) {
			pivotFilter.LabelPivot = GetFirstStringValue();
			AddCustomFilter(customFilterValue, value1IsDate, filterOperator);
		}
		void AddTwoValueCaptionCustomFilter(FilterComparisonOperator filterOperator1, FilterComparisonOperator filterOperator2, bool criterionAdd) {
			string stringValue1 = GetFirstStringValue();
			string stringValue2 = GetSecondStringValue();
			pivotFilter.LabelPivot = stringValue1;
			pivotFilter.LabelPivotFilter = stringValue2;
			AddCustomFilter(value1, stringValue1, value1IsDate, filterOperator1);
			AddCustomFilter(value2, stringValue2, value2IsDate, filterOperator2);
			FilterColumn.CustomFilters.CriterionAnd = criterionAdd;
		}
		void AddSingleValueNumericCustomFilter(FilterComparisonOperator filterOperator) {
			AddCustomFilterInvariant(value1, value1IsDate, filterOperator);
		}
		void AddTwoValueNumericCustomFilter(FilterComparisonOperator filterOperator1, FilterComparisonOperator filterOperator2, bool criterionAdd) {
			AddCustomFilterInvariant(value1, value1IsDate, filterOperator1);
			AddCustomFilterInvariant(value2, value2IsDate, filterOperator2);
			FilterColumn.CustomFilters.CriterionAnd = criterionAdd;
		}
		void AddCustomFilter(VariantValue value, string stringValue, bool isDateTime, FilterComparisonOperator filterOperator) {
			if (!value.IsNumeric || ShouldPreserveStringValue)
				AddCustomFilter(stringValue, isDateTime, filterOperator);
			else
				AddCustomFilterInvariant(value, isDateTime, filterOperator);
		}
		void AddCustomFilter(string stringValue, bool isDateTime, FilterComparisonOperator filterOperator) {
			CustomFilter customFilter = new CustomFilter();
			customFilter.FilterOperator = filterOperator;
			customFilter.Value = stringValue;
			customFilter.UpdateNumericValue(DataContext, isDateTime);
			FilterColumn.CustomFilters.Add(customFilter);
		}
		void AddCustomFilterInvariant(VariantValue value, bool isDateTime, FilterComparisonOperator filterOperator) {
			CustomFilter customFilter = new CustomFilter();
			customFilter.FilterOperator = filterOperator;
			customFilter.SetupFromInvariantValue(value, isDateTime);
			FilterColumn.CustomFilters.Add(customFilter);
		}
		void AddTop10Filter() {
			FilterColumn.FilterByTopOrder = FilterByTop;
			FilterColumn.Top10FilterType = ConvertTop10FilterType(filterType);
			FilterColumn.TopOrBottomDoubleValue = value1.NumericValue;
		}
		Top10FilterType ConvertTop10FilterType(PivotFilterType filterType) {
			switch(filterType){
				case PivotFilterType.Percent:
					return Top10FilterType.Percent;
				case PivotFilterType.Sum:
					return Top10FilterType.Sum;
				case PivotFilterType.Count:
					return Top10FilterType.Count;
			}
			return Top10FilterType.Count;
		}
		void AddDynamicFilter() {
			FilterColumn.DynamicFilterType = dynamicFilterTypes[filterType];
		}
		#region ToString
		string VariantToString(VariantValue value, bool isDateTime) {
			switch (value.Type) {
				default:
					return String.Empty;
				case VariantValueType.Missing:
				case VariantValueType.None:
					return String.Empty;
				case VariantValueType.Error:
					return value.ErrorValue.Name;
				case VariantValueType.Boolean:
					return BooleanToString(value.BooleanValue);
				case VariantValueType.Numeric:
					return NumericToString(value, isDateTime);
				case VariantValueType.InlineText:
					return value.InlineTextValue;
			}
		}
		string BooleanToString(bool value) {
			string result = value.ToString(Culture);
			return  result.ToUpper();
		}
		string NumericToString(VariantValue value, bool isDateTime) {
			double numericValue = value.NumericValue;
			if (isDateTime) {
				DateTime dateTime;
				if (WorkbookDataContext.IsErrorDateTimeSerial(numericValue, Context.DateSystem))
					dateTime = DateTime.MinValue;
				else
					dateTime = value.ToDateTime(Context.DateSystem);
				return DateTimeToString(dateTime);
			}
			return numericValue.ToString(Culture);
		}
		string DateTimeToString(DateTime date) {
			string shortDate = date.ToString(Culture.DateTimeFormat.ShortDatePattern, Culture);
			if (date.Hour > 0 || date.Minute > 0 || date.Second > 0)
				return shortDate + " " + date.ToString(Culture.DateTimeFormat.LongTimePattern, Culture);
			return shortDate;
		}
		#endregion
		#endregion
	}
	#endregion
}
