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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.Office;
using System.Xml;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region LeafAutoFilterDestinationBase
	public abstract class LeafAutoFilterColumnDestinationBase : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly AutoFilterColumn autoFilterColumn;
		#endregion
		protected LeafAutoFilterColumnDestinationBase(SpreadsheetMLBaseImporter importer, AutoFilterColumn autoFilterColumn)
			: base(importer) {
			Guard.ArgumentNotNull(autoFilterColumn, "autoFilterColumn");
			this.autoFilterColumn = autoFilterColumn;
		}
		#region Properties
		protected internal AutoFilterColumn AutoFilterColumn { get { return autoFilterColumn; } }
		#endregion
	}
	#endregion
	#region AutoFilterDestination
	public class AutoFilterDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("filterColumn", OnFilterColumn);
			result.Add("sortState", OnSortState);
			return result;
		}
		static AutoFilterDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (AutoFilterDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Worksheet sheet;
		readonly AutoFilterBase autoFilter;
		#endregion
		public AutoFilterDestination(SpreadsheetMLBaseImporter importer, AutoFilterBase autoFilter, Worksheet sheet)
			: base(importer) {
			Guard.ArgumentNotNull(sheet, "sheet");
			Guard.ArgumentNotNull(autoFilter, "autoFilter");
			this.sheet = sheet;
			this.autoFilter = autoFilter;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal Worksheet Sheet { get { return sheet; } }
		protected internal AutoFilterBase AutoFilter { get { return autoFilter; } }
		#endregion
		static Destination OnFilterColumn(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FilterColumnDestination(importer, GetThis(importer).AutoFilter.FilterColumns);
		}
		static Destination OnSortState(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AutoFilterDestination destination = GetThis(importer);
			return new SortStateDestination(importer, destination.AutoFilter.SortState, destination.Sheet);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellRange range = Importer.ReadCellRange(reader, "ref", Sheet);
			if (range != null) {
				AutoFilter.SetRange(range);
				AutoFilter.CreateFilterColumnsForRange(range);
			}
		}
	}
	#endregion
	#region FilterColumnDestination
	public class FilterColumnDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("colorFilter", OnColorFilter);
			result.Add("customFilters", OnCustomFilters);
			result.Add("dynamicFilter", OnDynamicFilter);
			result.Add("filters", OnFilters);
			result.Add("iconFilter", OnIconFilter);
			result.Add("top10", OnTop10);
			return result;
		}
		static FilterColumnDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (FilterColumnDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly AutoFilterColumnCollection autoFilterColumns;
		AutoFilterColumn autoFilterColumn;
		int differentialFormatIndex = CellFormatCache.DefaultDifferentialFormatIndex;
		#endregion
		public FilterColumnDestination(SpreadsheetMLBaseImporter importer, AutoFilterColumnCollection autoFilterColumns)
			: base(importer) {
			Guard.ArgumentNotNull(autoFilterColumns, "autoFilterColumns");
			this.autoFilterColumns = autoFilterColumns;
		}
		#region Properties
		public AutoFilterColumn AutoFilterColumn { get { return autoFilterColumn; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal int DifferentialFotmatIndex { get { return differentialFormatIndex; } set { differentialFormatIndex = value; } }
		#endregion
		static Destination OnColorFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorFilterDestination(importer, GetThis(importer));
		}
		static Destination OnCustomFilters(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CustomFiltersDestination(importer, GetThis(importer).AutoFilterColumn.CustomFilters);
		}
		static Destination OnDynamicFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DynamicFilterDestination(importer, GetThis(importer).AutoFilterColumn);
		}
		static Destination OnFilters(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FilterCriteriaDestination(importer, GetThis(importer).AutoFilterColumn.FilterCriteria);
		}
		static Destination OnIconFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IconFilterDestination(importer, GetThis(importer).AutoFilterColumn);
		}
		static Destination OnTop10(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new Top10Destination(importer, GetThis(importer).AutoFilterColumn);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int columnId = Importer.GetIntegerValue(reader, "colId", Int32.MinValue);
			if (columnId == Int32.MinValue)
				Importer.ThrowInvalidFile("Filter column destination: no colId");
			autoFilterColumn = autoFilterColumns[columnId];
			autoFilterColumn.BeginUpdate();
			autoFilterColumn.HiddenAutoFilterButton = Importer.GetOnOffValue(reader, "hiddenButton", false);
			autoFilterColumn.ShowFilterButton = Importer.GetOnOffValue(reader, "showButton", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			autoFilterColumn.EndUpdate();
			autoFilterColumn.AssignFormatIndex(differentialFormatIndex);
		}
	}
	#endregion
	#region ColorFilterDestination
	public class ColorFilterDestination : LeafAutoFilterColumnDestinationBase {
		readonly FilterColumnDestination filterColumnDestination;
		public ColorFilterDestination(SpreadsheetMLBaseImporter importer, FilterColumnDestination filterColumnDestination)
			: base(importer, filterColumnDestination.AutoFilterColumn) {
			this.filterColumnDestination = filterColumnDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int dxfId = Importer.GetIntegerValue(reader, "dxfId", Int32.MinValue);
			filterColumnDestination.DifferentialFotmatIndex = Importer.StyleSheet.GetDifferentialFormatIndex(dxfId);
			AutoFilterColumn.FilterByCellFill = Importer.GetOnOffValue(reader, "cellColor", true);
		}
	}
	#endregion
	#region CustomFiltersDestination
	public class CustomFiltersDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("customFilter", OnCustomFilter);
			return result;
		}
		static CustomFiltersDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CustomFiltersDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly CustomFilterCollection customFilters;
		#endregion
		public CustomFiltersDestination(SpreadsheetMLBaseImporter importer, CustomFilterCollection customFilters)
			: base(importer) {
			Guard.ArgumentNotNull(customFilters, "customFilters");
			this.customFilters = customFilters;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal CustomFilterCollection CustomFilters { get { return customFilters; } }
		#endregion
		static Destination OnCustomFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CustomFilterDestination(importer, GetThis(importer).CustomFilters);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CustomFilters.CriterionAnd = Importer.GetOnOffValue(reader, "and", false);
		}
	}
	#endregion
	#region CustomFilterDestination
	public class CustomFilterDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<FilterComparisonOperator, string> filterComparisonOperatorTable = OpenXmlExporter.FilterComparisonOperatorTable;
		#endregion
		#region Fields
		readonly CustomFilterCollection customFilters;
		#endregion
		public CustomFilterDestination(SpreadsheetMLBaseImporter importer, CustomFilterCollection customFilters)
			: base(importer) {
			Guard.ArgumentNotNull(customFilters, "customFilters");
			this.customFilters = customFilters;
		}
		#region Properties
		protected internal CustomFilterCollection CustomFilters { get { return customFilters; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CustomFilter customFilter = new CustomFilter();
			customFilter.FilterOperator = Importer.GetWpEnumValue<FilterComparisonOperator>(reader, "operator", filterComparisonOperatorTable, FilterComparisonOperator.Equal);
			string value = Importer.ReadAttribute(reader, "val");
			if (!String.IsNullOrEmpty(value))
				customFilter.Value = value;
			customFilter.UpdateNumericValue(Importer.DocumentModel.DataContext, false);
			CustomFilters.Add(customFilter);
		}
	}
	#endregion
	#region DynamicFilterDestination
	public class DynamicFilterDestination : LeafAutoFilterColumnDestinationBase {
		#region Static members
		static Dictionary<DynamicFilterType, string> dynamicFilterTypeTable = OpenXmlExporter.DynamicFilterTypeTable;
		#endregion
		public DynamicFilterDestination(SpreadsheetMLBaseImporter importer, AutoFilterColumn autoFilterColumn)
			: base(importer, autoFilterColumn) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string dynamicFilterType = Importer.ReadAttribute(reader, "type");
			if (String.IsNullOrEmpty(dynamicFilterType))
				Importer.ThrowInvalidFile("Has no dymamic filter type");
			AutoFilterColumn.DynamicFilterType = Importer.GetWpEnumValueCore<DynamicFilterType>(dynamicFilterType, dynamicFilterTypeTable, DynamicFilterType.Null);
			AutoFilterColumn.DynamicMinValue = Importer.GetWpSTFloatValue(reader, "val", -1.0f);
			AutoFilterColumn.DynamicMaxValue = Importer.GetWpSTFloatValue(reader, "maxVal", -1.0f);
		}
	}
	#endregion
	#region FilterCriteriaDestination
	public class FilterCriteriaDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("filter", OnFilter);
			result.Add("dateGroupItem", OnDateGroupItem);
			return result;
		}
		static FilterCriteriaDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (FilterCriteriaDestination)importer.PeekDestination();
		}
		static Dictionary<CalendarType, string> calendarTypeTable = OpenXmlExporter.CalendarTypeTable;
		#endregion
		#region Fields
		readonly FilterCriteria filterCriteria;
		#endregion
		public FilterCriteriaDestination(SpreadsheetMLBaseImporter importer, FilterCriteria filterCriteria)
			: base(importer) {
			Guard.ArgumentNotNull(filterCriteria, "filterCriteria");
			this.filterCriteria = filterCriteria;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal FilterCriteria FilterCriteria { get { return filterCriteria; } }
		#endregion
		static Destination OnFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FilterDestination(importer, GetThis(importer).FilterCriteria.Filters);
		}
		static Destination OnDateGroupItem(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DateGroupItemDestination(importer, GetThis(importer).FilterCriteria.DateGroupings);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FilterCriteria.FilterByBlank = Importer.GetWpSTOnOffValue(reader, "blank", false);
			FilterCriteria.CalendarType = Importer.GetWpEnumValue<CalendarType>(reader, "calendarType", calendarTypeTable, CalendarType.None);
		}
	}
	#endregion
	#region FilterDestination
	public class FilterDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly FilterCollection filters;
		#endregion
		public FilterDestination(SpreadsheetMLBaseImporter importer, FilterCollection filters)
			: base(importer) {
			Guard.ArgumentNotNull(filters, "filters");
			this.filters = filters;
		}
		#region Properties
		protected internal FilterCollection Filters { get { return filters; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string value = Importer.ReadAttribute(reader, "val");
			if (String.IsNullOrEmpty(value))
				return;
			Filters.AddWithoutHistoryAndNotifications(value);
		}
	}
	#endregion
	#region DateGroupItemDestination
	public class DateGroupItemDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<DateTimeGroupingType, string> dateTimeGroupingTable = OpenXmlExporter.DateTimeGroupingTable;
		#endregion
		#region Fields
		readonly DateGroupingCollection dateGroupings;
		#endregion
		public DateGroupItemDestination(SpreadsheetMLBaseImporter importer, DateGroupingCollection dateGroupings)
			: base(importer) {
			Guard.ArgumentNotNull(dateGroupings, "dateGroupings");
			this.dateGroupings = dateGroupings;
		}
		#region Properties
		protected internal DateGroupingCollection DateGroupings { get { return dateGroupings; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string dateTimeGrouping = Importer.ReadAttribute(reader, "dateTimeGrouping");
			if (String.IsNullOrEmpty(dateTimeGrouping))
				Importer.ThrowInvalidFile("Has no dateTimeGrouping");
			int year = Importer.GetWpSTIntegerValue(reader, "year", Int32.MinValue);
			if (year == Int32.MinValue)
				Importer.ThrowInvalidFile("Has no year");
			DateGrouping dateGrouping = new DateGrouping(Importer.CurrentWorksheet);
			dateGrouping.BeginUpdate();
			try {
				dateGrouping.DateTimeGrouping = Importer.GetWpEnumValueCore<DateTimeGroupingType>(dateTimeGrouping, dateTimeGroupingTable, DateTimeGroupingType.Year);
				dateGrouping.Year = year;
				dateGrouping.Month = Importer.GetWpSTIntegerValue(reader, "month", 0);
				dateGrouping.Day = Importer.GetWpSTIntegerValue(reader, "day", 0);
				dateGrouping.Hour = Importer.GetWpSTIntegerValue(reader, "hour", -1);
				dateGrouping.Minute = Importer.GetWpSTIntegerValue(reader, "minute", -1);
				dateGrouping.Second = Importer.GetWpSTIntegerValue(reader, "second", -1);
			}
			finally {
				dateGrouping.EndUpdate();
			}
			DateGroupings.AddWithoutHistoryAndNotifications(dateGrouping);
		}
	}
	#endregion
	#region IconFilterDestination
	public class IconFilterDestination : LeafAutoFilterColumnDestinationBase {
		#region Static members
		static Dictionary<IconSetType, string> iconSetTypeTable = OpenXmlExporter.IconSetTypeTable;
		#endregion
		public IconFilterDestination(SpreadsheetMLBaseImporter importer, AutoFilterColumn autoFilterColumn)
			: base(importer, autoFilterColumn) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string iconSet = Importer.ReadAttribute(reader, "iconSet");
			if (!String.IsNullOrEmpty(iconSet))
				AutoFilterColumn.IconSetType = Importer.GetWpEnumValueCore<IconSetType>(iconSet, iconSetTypeTable, IconSetType.Arrows3);
			AutoFilterColumn.IconId = Importer.GetWpSTIntegerValue(reader, "iconId", -1);
		}
	}
	#endregion
	#region Top10Destination
	public class Top10Destination : LeafAutoFilterColumnDestinationBase {
		public Top10Destination(SpreadsheetMLBaseImporter importer, AutoFilterColumn autoFilterColumn)
			: base(importer, autoFilterColumn) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			AutoFilterColumn.FilterByTopOrder = Importer.GetWpSTOnOffValue(reader, "top", true);
			bool filterByPercent = Importer.GetWpSTOnOffValue(reader, "percent", false);
			AutoFilterColumn.Top10FilterType = filterByPercent ? Top10FilterType.Percent : Top10FilterType.Count;
			float topOrBottomValue = Importer.GetWpSTFloatValue(reader, "val", float.MinValue);
			if (topOrBottomValue == float.MinValue)
				Importer.ThrowInvalidFile("Has no top10 value");
			AutoFilterColumn.TopOrBottomDoubleValue = topOrBottomValue;
			AutoFilterColumn.FilterDoubleValue = Importer.GetWpSTFloatValue(reader, "filterVal", -1);
		}
	}
	#endregion
}
