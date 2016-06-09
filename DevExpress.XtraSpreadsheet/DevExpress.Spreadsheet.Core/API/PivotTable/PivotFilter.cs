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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotFilterType
	public enum PivotFilterType {
		CaptionEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionEqual,
		CaptionNotEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionNotEqual,
		CaptionBeginsWith = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionBeginsWith,
		CaptionNotBeginsWith = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionNotBeginsWith,
		CaptionEndsWith = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionEndsWith,
		CaptionNotEndsWith = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionNotEndsWith,
		CaptionContains = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionContains,
		CaptionNotContains = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionNotContains,
		CaptionGreaterThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionGreaterThan,
		CaptionGreaterThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionGreaterThanOrEqual,
		CaptionLessThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionLessThan,
		CaptionLessThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionLessThanOrEqual,
		CaptionBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionBetween,
		CaptionNotBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.CaptionNotBetween,
		Count = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Count,
		DateEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateEqual,
		DateNotEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateNotEqual,
		DateOlderThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateOlderThan,
		DateOlderThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateOlderThanOrEqual,
		DateNewerThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateNewerThan,
		DateNewerThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateNewerThanOrEqual,
		DateBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateBetween,
		DateNotBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.DateNotBetween,
		LastWeek = DevExpress.XtraSpreadsheet.Model.PivotFilterType.LastWeek,
		LastMonth = DevExpress.XtraSpreadsheet.Model.PivotFilterType.LastMonth,
		LastQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.LastQuarter,
		LastYear = DevExpress.XtraSpreadsheet.Model.PivotFilterType.LastYear,
		January = DevExpress.XtraSpreadsheet.Model.PivotFilterType.January,
		February = DevExpress.XtraSpreadsheet.Model.PivotFilterType.February,
		March = DevExpress.XtraSpreadsheet.Model.PivotFilterType.March,
		April = DevExpress.XtraSpreadsheet.Model.PivotFilterType.April,
		May = DevExpress.XtraSpreadsheet.Model.PivotFilterType.May,
		June = DevExpress.XtraSpreadsheet.Model.PivotFilterType.June,
		July = DevExpress.XtraSpreadsheet.Model.PivotFilterType.July,
		August = DevExpress.XtraSpreadsheet.Model.PivotFilterType.August,
		September = DevExpress.XtraSpreadsheet.Model.PivotFilterType.September,
		October = DevExpress.XtraSpreadsheet.Model.PivotFilterType.October,
		November = DevExpress.XtraSpreadsheet.Model.PivotFilterType.November,
		December = DevExpress.XtraSpreadsheet.Model.PivotFilterType.December,
		NextWeek = DevExpress.XtraSpreadsheet.Model.PivotFilterType.NextWeek,
		NextMonth = DevExpress.XtraSpreadsheet.Model.PivotFilterType.NextMonth,
		NextQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.NextQuarter,
		NextYear = DevExpress.XtraSpreadsheet.Model.PivotFilterType.NextYear,
		Percent = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Percent,
		FirstQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.FirstQuarter,
		SecondQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.SecondQuarter,
		ThirdQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ThirdQuarter,
		FourthQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.FourthQuarter,
		Sum = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Sum,
		Tomorrow = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Tomorrow,
		Today = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Today,
		ThisWeek = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ThisWeek,
		ThisMonth = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ThisMonth,
		ThisQuarter = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ThisQuarter,
		ThisYear = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ThisYear,
		ValueEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueEqual,
		ValueNotEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueNotEqual,
		ValueGreaterThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueGreaterThan,
		ValueGreaterThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueGreaterThanOrEqual,
		ValueLessThan = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueLessThan,
		ValueLessThanOrEqual = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueLessThanOrEqual,
		ValueBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueBetween,
		ValueNotBetween = DevExpress.XtraSpreadsheet.Model.PivotFilterType.ValueNotBetween,
		Yesterday = DevExpress.XtraSpreadsheet.Model.PivotFilterType.Yesterday,
		YearToDate = DevExpress.XtraSpreadsheet.Model.PivotFilterType.YearToDate
	}
	#endregion
	#region Top10FilterType
	public enum PivotFilterTop10Type {
		None,
		Top,
		Bottom
	}
	#endregion
	#region PivotFilter
	public interface PivotFilter {
		string Name { get; set; }
		string Description { get; set; }
		PivotFilterType FilterType { get; }
		PivotField Field { get; }
		PivotDataField MeasureField { get; }
		FilterValue Value { get; }
		FilterValue SecondValue { get; }
		PivotFilterTop10Type Top10Type { get; set; }
		void Delete();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	partial class NativePivotFilter : NativeObjectBase, PivotFilter {
		readonly NativePivotTable parentTable;
		readonly Model.PivotFilter modelFilter;
		public NativePivotFilter(Model.PivotFilter modelFilter, NativePivotTable parentTable) {
			Guard.ArgumentNotNull(modelFilter, "modelFilter");
			Guard.ArgumentNotNull(parentTable, "parentTable");
			this.parentTable = parentTable;
			this.modelFilter = modelFilter;
		}
		#region Properties
		protected internal Model.PivotFilter ModelItem { get { return modelFilter; } }
		Model.PivotTable ModelPivotTable { get { return parentTable.ModelItem; } }
		Model.WorkbookDataContext DataContext { get { return modelFilter.AutoFilter.Sheet.DataContext; } }
		Model.AutoFilterColumn AutoFilterColumn { get { return modelFilter.AutoFilter.FilterColumns[0]; } }
		public string Name {
			get {
				CheckValid();
				return modelFilter.Name;
			}
			set {
				CheckValid();
				modelFilter.Name = value;
			}
		}
		public string Description {
			get {
				CheckValid();
				return modelFilter.Description;
			}
			set {
				CheckValid();
				modelFilter.Description = value;
			}
		}
		public PivotFilterType FilterType {
			get {
				CheckValid();
				return (PivotFilterType)modelFilter.FilterType;
			}
		}
		public PivotField Field {
			get {
				CheckValid();
				return parentTable.Fields[modelFilter.FieldIndex];
			}
		}
		public PivotDataField MeasureField {
			get {
				CheckValid();
				int? index = modelFilter.MeasureFieldIndex;
				if (!index.HasValue)
					return null;
				return parentTable.DataFields[index.Value];
			}
		}
		public PivotFilterTop10Type Top10Type {
			get {
				CheckValid();
				if (!AutoFilterColumn.IsTop10Filter)
					return PivotFilterTop10Type.None;
				return AutoFilterColumn.FilterByTopOrder ? PivotFilterTop10Type.Top : PivotFilterTop10Type.Bottom;
			}
			set {
				CheckValid();
				if (!AutoFilterColumn.IsTop10Filter)
					ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.PivotFilterCannotChangeTop10TypeProperty);
				bool filterByTop = AutoFilterColumn.FilterByTopOrder;
				if (value == PivotFilterTop10Type.None ||
					(value == PivotFilterTop10Type.Top && filterByTop) ||
					(value == PivotFilterTop10Type.Bottom && !filterByTop))
					return;
				ModelPivotTable.BeginTransaction(ApiErrorHandler.Instance);
				try {
					AutoFilterColumn.FilterByTopOrder = !filterByTop;
					ModelPivotTable.CalculationInfo.InvalidateCalculatedCache();
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		public FilterValue Value {
			get {
				CheckValid();
				return GetFirstValue();
			}
		}
		public FilterValue SecondValue {
			get {
				CheckValid();
				return GetCustomFilterValue(1);
			}
		}
		#endregion
		#region GetValues
		FilterValue GetFirstValue() {
			if (Top10Type != PivotFilterTop10Type.None)
				return new FilterValue(AutoFilterColumn.TopOrBottomDoubleValue, DataContext);
			Model.FilterCollection filters = AutoFilterColumn.FilterCriteria.Filters;
			if (filters.Count > 0)
				return new FilterValue(filters[0], DataContext);
			if (AutoFilterColumn.FilterCriteria.FilterByBlank)
				return FilterValue.FilterByBlank;
			return GetCustomFilterValue(0);
		}
		FilterValue GetCustomFilterValue(int index) {
			Model.CustomFilterCollection customFilters = AutoFilterColumn.CustomFilters;
			if (customFilters.Count <= index)
				return null;
			Model.CustomFilter customFilter = customFilters[index];
			Model.VariantValue numericValue = customFilter.NumericValue;
			if (numericValue.IsEmpty || numericValue.IsMissing)
				return new FilterValue(customFilter.Value, DataContext, customFilter.IsDateTime);
			else
				return new FilterValue(numericValue, DataContext, customFilter.IsDateTime);
		}
		#endregion
		public void Delete() {
			CheckValid();
			parentTable.Filters.Remove(this);
		}
	}
}
