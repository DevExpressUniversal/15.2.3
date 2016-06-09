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
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<FilterComparisonOperator, string> FilterComparisonOperatorTable = CreateFilterComparisonOperatorTable();
		internal static Dictionary<DynamicFilterType, string> DynamicFilterTypeTable = CreateDynamicFilterTypeTable();
		internal static Dictionary<CalendarType, string> CalendarTypeTable = CreateCalendarTypeTable();
		internal static Dictionary<DateTimeGroupingType, string> DateTimeGroupingTable = CreateDateTimeGroupingTable();
		static Dictionary<FilterComparisonOperator, string> CreateFilterComparisonOperatorTable() {
			Dictionary<FilterComparisonOperator, string> result = new Dictionary<FilterComparisonOperator, string>();
			result.Add(FilterComparisonOperator.Equal, "equal");
			result.Add(FilterComparisonOperator.LessThan, "lessThan");
			result.Add(FilterComparisonOperator.LessThanOrEqual, "lessThanOrEqual");
			result.Add(FilterComparisonOperator.NotEqual, "notEqual");
			result.Add(FilterComparisonOperator.GreaterThanOrEqual, "greaterThanOrEqual");
			result.Add(FilterComparisonOperator.GreaterThan, "greaterThan");
			return result;
		}
		static Dictionary<DynamicFilterType, string> CreateDynamicFilterTypeTable() {
			Dictionary<DynamicFilterType, string> result = new Dictionary<DynamicFilterType, string>();
			result.Add(DynamicFilterType.Null, "null");
			result.Add(DynamicFilterType.AboveAverage, "aboveAverage");
			result.Add(DynamicFilterType.BelowAverage, "belowAverage");
			result.Add(DynamicFilterType.Tomorrow, "tomorrow");
			result.Add(DynamicFilterType.Today, "today");
			result.Add(DynamicFilterType.Yesterday, "yesterday");
			result.Add(DynamicFilterType.NextWeek, "nextWeek");
			result.Add(DynamicFilterType.ThisWeek, "thisWeek");
			result.Add(DynamicFilterType.LastWeek, "lastWeek");
			result.Add(DynamicFilterType.NextMonth, "nextMonth");
			result.Add(DynamicFilterType.ThisMonth, "thisMonth");
			result.Add(DynamicFilterType.LastMonth, "lastMonth");
			result.Add(DynamicFilterType.NextQuarter, "nextQuarter");
			result.Add(DynamicFilterType.ThisQuarter, "thisQuarter");
			result.Add(DynamicFilterType.LastQuarter, "lastQuarter");
			result.Add(DynamicFilterType.NextYear, "nextYear");
			result.Add(DynamicFilterType.ThisYear, "thisYear");
			result.Add(DynamicFilterType.LastYear, "lastYear");
			result.Add(DynamicFilterType.YearToDate, "yearToDate");
			result.Add(DynamicFilterType.Q1, "Q1");
			result.Add(DynamicFilterType.Q2, "Q2");
			result.Add(DynamicFilterType.Q3, "Q3");
			result.Add(DynamicFilterType.Q4, "Q4");
			result.Add(DynamicFilterType.M1, "M1");
			result.Add(DynamicFilterType.M2, "M2");
			result.Add(DynamicFilterType.M3, "M3");
			result.Add(DynamicFilterType.M4, "M4");
			result.Add(DynamicFilterType.M5, "M5");
			result.Add(DynamicFilterType.M6, "M6");
			result.Add(DynamicFilterType.M7, "M7");
			result.Add(DynamicFilterType.M8, "M8");
			result.Add(DynamicFilterType.M9, "M9");
			result.Add(DynamicFilterType.M10, "M10");
			result.Add(DynamicFilterType.M11, "M11");
			result.Add(DynamicFilterType.M12, "M12");
			return result;
		}
		static Dictionary<CalendarType, string> CreateCalendarTypeTable() {
			Dictionary<CalendarType, string> result = new Dictionary<CalendarType, string>();
			result.Add(CalendarType.None, "none");
			result.Add(CalendarType.Gregorian, "gregorian");
			result.Add(CalendarType.GregorianUs, "gregorianUs");
			result.Add(CalendarType.Japan, "japan");
			result.Add(CalendarType.Taiwan, "taiwan");
			result.Add(CalendarType.Korea, "korea");
			result.Add(CalendarType.Hijri, "hijri");
			result.Add(CalendarType.Thai, "thai");
			result.Add(CalendarType.Hebrew, "hebrew");
			result.Add(CalendarType.GregorianMeFrench, "gregorianMeFrench");
			result.Add(CalendarType.GregorianArabic, "gregorianArabic");
			result.Add(CalendarType.GregorianXlitEnglish, "gregorianXlitEnglish");
			result.Add(CalendarType.GregorianXlitFrench, "gregorianXlitFrench");
			return result;
		}
		static Dictionary<DateTimeGroupingType, string> CreateDateTimeGroupingTable() {
			Dictionary<DateTimeGroupingType, string> result = new Dictionary<DateTimeGroupingType, string>();
			result.Add(DateTimeGroupingType.Year, "year");
			result.Add(DateTimeGroupingType.Month, "month");
			result.Add(DateTimeGroupingType.Day, "day");
			result.Add(DateTimeGroupingType.Hour, "hour");
			result.Add(DateTimeGroupingType.Minute, "minute");
			result.Add(DateTimeGroupingType.Second, "second");
			return result;
		}
		#endregion
		#region Properties
		protected internal AutoFilterColumnInfo DefaultAutoFilterColumnInfo { get { return Workbook.Cache.AutoFilterColumnInfoCache.DefaultItem; } }
		#endregion
		protected internal virtual void GenerateSheetAutoFilterContent(SheetAutoFilter autoFilter) {
			if (!ShouldExportAutoFilter(autoFilter)) {
				GenerateSortStateContent(autoFilter.SortState);
				return;
			}
			GenerateAutoFilterContentCore(autoFilter);
		}
		protected internal virtual void GenerateTableAutoFilterContent(TableAutoFilter autoFilter) {
			if (!ShouldExportAutoFilter(autoFilter))
				return;
			GenerateAutoFilterContentCore(autoFilter);
		}
		void GenerateAutoFilterContentCore(AutoFilterBase autoFilter) {
			WriteShStartElement("autoFilter");
			try {
				WriteStringValue("ref", autoFilter.Range.ToString());
				GenerateFilterColumnsContent(autoFilter);
				if (autoFilter is SheetAutoFilter)
					GenerateSortStateContent(autoFilter.SortState);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportAutoFilter(AutoFilterBase autoFilter) {
			return autoFilter.Enabled;
		}
		private void GenerateFilterColumnsContent(AutoFilterBase autoFilter) {
			foreach (AutoFilterColumn item in autoFilter.FilterColumns)
				GenerateFilterColumnContent(item);
		}
		protected internal virtual void GenerateFilterColumnContent(AutoFilterColumn autoFilterColumn) {
			if (!ShouldExportFilterColumn(autoFilterColumn))
				return;
			WriteShStartElement("filterColumn");
			try {
				GenerateTableFilterColumnAttributesContent(autoFilterColumn);
				GenerateColorFilterContent(autoFilterColumn);
				GenerateCustomFiltersContent(autoFilterColumn.CustomFilters);
				GenerateDynamicFilter(autoFilterColumn);
				GenerateFilterCriteriaContent(autoFilterColumn.FilterCriteria);
				GenerateIconFilterContent(autoFilterColumn);
				GenerateTop10Content(autoFilterColumn);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportFilterColumn(AutoFilterColumn autoFilterColumn) {
			return autoFilterColumn.IsNonDefault;
		}
		protected internal virtual void GenerateTableFilterColumnAttributesContent(AutoFilterColumn autoFilterColumn) {
			WriteIntValue("colId", autoFilterColumn.ColumnId);
			if (autoFilterColumn.HiddenAutoFilterButton != DefaultAutoFilterColumnInfo.HiddenAutoFilterButton)
				WriteBoolValue("hiddenButton", autoFilterColumn.HiddenAutoFilterButton);
			if (autoFilterColumn.ShowFilterButton != DefaultAutoFilterColumnInfo.ShowFilterButton)
				WriteBoolValue("showButton", autoFilterColumn.ShowFilterButton);
		}
		protected internal virtual void GenerateColorFilterContent(AutoFilterColumn autoFilterColumn) {
			if (!ShouldExportColorFilter(autoFilterColumn))
				return;
			WriteShStartElement("colorFilter");
			try {
				int dxfId = exportStyleSheet.GetDifferentialFormatId(autoFilterColumn.FormatIndex);
				if (dxfId >= 0)
					WriteIntValue("dxfId", dxfId);
				if (autoFilterColumn.FilterByCellFill != DefaultAutoFilterColumnInfo.FilterByCellFill)
					WriteBoolValue("cellColor", autoFilterColumn.FilterByCellFill);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportColorFilter(AutoFilterColumn autoFilterColumn) {
			return autoFilterColumn.FilterByCellFill != DefaultAutoFilterColumnInfo.FilterByCellFill ||
				autoFilterColumn.FormatIndex != CellFormatCache.DefaultDifferentialFormatIndex;
		}
		protected internal virtual void GenerateCustomFiltersContent(CustomFilterCollection customFilters) {
			if (customFilters.Count == 0)
				return;
			WriteShStartElement("customFilters");
			try {
				if (customFilters.CriterionAnd != false)
					WriteBoolValue("and", customFilters.CriterionAnd);
				customFilters.ForEach(GenerateCustomFilterContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCustomFilterContent(CustomFilter customFilter) {
			WriteShStartElement("customFilter");
			try {
				if (customFilter.FilterOperator != FilterComparisonOperator.Equal)
					WriteStringValue("operator", FilterComparisonOperatorTable[customFilter.FilterOperator]);
				WriteStringValue("val", customFilter.Value);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateDynamicFilter(AutoFilterColumn autoFilterColumn) {
			if (!ShouldExportDynamicFilter(autoFilterColumn))
				return;
			WriteShStartElement("dynamicFilter");
			try {
				if (autoFilterColumn.DynamicFilterType != DefaultAutoFilterColumnInfo.DynamicFilterType)
					WriteStringValue("type", DynamicFilterTypeTable[autoFilterColumn.DynamicFilterType]);
				if (autoFilterColumn.DynamicMinValue != DefaultAutoFilterColumnInfo.DynamicMinValue)
					WriteStringValue("val", autoFilterColumn.DynamicMinValue.ToString(CultureInfo.InvariantCulture));
				if (autoFilterColumn.DynamicMaxValue != DefaultAutoFilterColumnInfo.DynamicMaxValue)
					WriteStringValue("maxVal", autoFilterColumn.DynamicMaxValue.ToString(CultureInfo.InvariantCulture));
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportDynamicFilter(AutoFilterColumn autoFilterColumn) {
			return autoFilterColumn.DynamicFilterType != DefaultAutoFilterColumnInfo.DynamicFilterType ||
				autoFilterColumn.DynamicMinValue != DefaultAutoFilterColumnInfo.DynamicMinValue ||
				autoFilterColumn.DynamicMaxValue != DefaultAutoFilterColumnInfo.DynamicMaxValue;
		}
		protected internal virtual void GenerateFilterCriteriaContent(FilterCriteria filterCriteria) {
			if (!ShouldExportFilterCriteria(filterCriteria))
				return;
			WriteShStartElement("filters");
			try {
				if (filterCriteria.FilterByBlank != false)
					WriteBoolValue("blank", filterCriteria.FilterByBlank);
				if (filterCriteria.CalendarType != CalendarType.None)
					WriteStringValue("calendarType", CalendarTypeTable[filterCriteria.CalendarType]);
				filterCriteria.Filters.ForEach(GenerateFilterContent);
				filterCriteria.DateGroupings.ForEach(GenerateDateGroupingContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportFilterCriteria(FilterCriteria filterCriteria) {
			return filterCriteria.FilterByBlank != false ||
				filterCriteria.CalendarType != CalendarType.None ||
			filterCriteria.Filters.Count != 0 || filterCriteria.DateGroupings.Count != 0;
		}
		protected internal virtual void GenerateFilterContent(string value) {
			Debug.Assert(!String.IsNullOrEmpty(value));
			WriteShStartElement("filter");
			try {
				WriteStringValue("val", value);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateDateGroupingContent(DateGrouping dateGrouping) {
			DateGroupingInfo defaultItem = Workbook.Cache.DateGroupingInfoCache.DefaultItem;
			Debug.Assert(dateGrouping.Year != defaultItem.Year);
			Debug.Assert(dateGrouping.DateTimeGrouping != defaultItem.DateTimeGrouping);
			WriteShStartElement("dateGroupItem");
			try {
				WriteIntValue("year", dateGrouping.Year);
				if(dateGrouping.DateTimeGrouping >= DateTimeGroupingType.Month && dateGrouping.HasMonth)
					WriteIntValue("month", dateGrouping.Month);
				if(dateGrouping.DateTimeGrouping >= DateTimeGroupingType.Day && dateGrouping.HasDay)
					WriteIntValue("day", dateGrouping.Day);
				if(dateGrouping.DateTimeGrouping >= DateTimeGroupingType.Hour && dateGrouping.HasHour)
					WriteIntValue("hour", dateGrouping.Hour);
				if(dateGrouping.DateTimeGrouping >= DateTimeGroupingType.Minute && dateGrouping.HasMinute)
					WriteIntValue("minute", dateGrouping.Minute);
				if(dateGrouping.DateTimeGrouping == DateTimeGroupingType.Second && dateGrouping.HasSecond)
					WriteIntValue("second", dateGrouping.Second);
				WriteStringValue("dateTimeGrouping", DateTimeGroupingTable[dateGrouping.DateTimeGrouping]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateIconFilterContent(AutoFilterColumn autoFilterColumn) {
			if (!ShouldExportIconFilter(autoFilterColumn))
				return;
			WriteShStartElement("iconFilter");
			try {
				WriteStringValue("iconSet", IconSetTypeTable[autoFilterColumn.IconSetType]);
				if (autoFilterColumn.IconId != DefaultAutoFilterColumnInfo.IconId)
					WriteIntValue("iconId", autoFilterColumn.IconId);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportIconFilter(AutoFilterColumn autoFilterColumn) {
			return autoFilterColumn.IconSetType != DefaultAutoFilterColumnInfo.IconSetType;
		}
		protected internal virtual void GenerateTop10Content(AutoFilterColumn autoFilterColumn) {
			if (!ShouldExportTop10(autoFilterColumn))
				return;
			WriteShStartElement("top10");
			try {
				WriteStringValue("val", autoFilterColumn.TopOrBottomDoubleValue.ToString(CultureInfo.InvariantCulture));
				if (autoFilterColumn.FilterByTopOrder != DefaultAutoFilterColumnInfo.FilterByTopOrder)
					WriteBoolValue("top", autoFilterColumn.FilterByTopOrder);
				if (autoFilterColumn.Top10FilterType == Top10FilterType.Percent)
					WriteBoolValue("percent", true);
				if (autoFilterColumn.FilterDoubleValue != DefaultAutoFilterColumnInfo.FilterDoubleValue)
					WriteStringValue("filterVal", autoFilterColumn.FilterDoubleValue.ToString(CultureInfo.InvariantCulture));
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportTop10(AutoFilterColumn autoFilterColumn) {
			return autoFilterColumn.IsTop10Filter;
		}
	}
}
