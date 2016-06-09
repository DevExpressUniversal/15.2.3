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
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	public class FilterCriteria {
		#region Fields
		readonly Worksheet sheet;
		bool filterByBlank;
		CalendarType calendarType;
		FilterCollection filters;
		DateGroupingCollection dateGroupings;
		#endregion
		public FilterCriteria(Worksheet sheet) {
			this.sheet = sheet;
			this.filters = new FilterCollection(sheet);
			this.dateGroupings = new DateGroupingCollection(sheet);
		}
		#region Properties;
		public Worksheet Sheet { get { return sheet; } }
		#region FilterByBlank
		public bool FilterByBlank {
			get { return filterByBlank; }
			set {
				if (filterByBlank == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				FilterCriteriaFilterByBlankHistoryItem item = new FilterCriteriaFilterByBlankHistoryItem(this, value, filterByBlank);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetFilterByBlankCore(bool value) {
			filterByBlank = value;
		}
		#endregion
		public CalendarType CalendarType { get { return calendarType; } set { calendarType = value; } }
		public FilterCollection Filters { get { return filters; } }
		public DateGroupingCollection DateGroupings { get { return dateGroupings; } }
		#endregion
		public void CopyFrom(FilterCriteria source, Worksheet targetWorksheet) {
			this.filterByBlank = source.filterByBlank;
			this.calendarType = source.calendarType;
			filters.Clear();
			foreach (string filterValue in source.Filters)
				filters.Add(filterValue);
			dateGroupings.Clear();
			foreach (DateGrouping sourceItem in source.DateGroupings) {
				DateGrouping targetDateGrouping = new DateGrouping(targetWorksheet);
				targetDateGrouping.CopyFrom(sourceItem);
				dateGroupings.Add(targetDateGrouping);
			}
		}
		public bool HasFilter() {
			return FilterByBlank || filters.Count > 0 || dateGroupings.Count > 0;
		}
		public bool IsValueVisible(IAutoFilterValue autoFilterValue) {
			string text;
			if (autoFilterValue != null) {
				if (autoFilterValue.Value.IsNumeric && autoFilterValue.IsDateTime && !WorkbookDataContext.IsErrorDateTimeSerial(autoFilterValue.Value.NumericValue, sheet.DataContext.DateSystem))
					return IsDateTimeCellVisible(autoFilterValue);
				text = autoFilterValue.Text;
			}
			else
				text = String.Empty;
			bool filtersContainText = filters.Contains(text);
			if (FilterByBlank)
				return String.IsNullOrEmpty(text) || filtersContainText;
			return filtersContainText;
		}
		bool IsDateTimeCellVisible(IAutoFilterValue autoFilterValue) {
			DateTime date = autoFilterValue.Value.ToDateTime(Sheet.DataContext);
			int count = dateGroupings.Count;
			for (int i = 0; i < count; i++)
				if (DateGroupings[i].IsVisible(date))
					return true;
			return filters.Contains(autoFilterValue.Text);
		}
		public void Clear() {
			sheet.Workbook.BeginUpdate();
			try {
				FilterByBlank = false;
				CalendarType = Model.CalendarType.None;
				Filters.Clear();
				DateGroupings.Clear();
			}
			finally {
				sheet.Workbook.EndUpdate();
			}
		}
	}
}
