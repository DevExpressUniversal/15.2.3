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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Filtering;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public abstract class DateColumnFilterInfoBase : ColumnFilterInfoBase, ICalendarFilterOwner {
		public DateColumnFilterInfoBase(ColumnBase column) : base(column) { }
		protected abstract FilterData[] CreateUpperFilters();
		protected abstract FilterData[] CreateBottomFilters();
		protected DataControlBase Grid {
			get { return View.DataControl; }
		}
		protected string FieldName {
			get { return Column.FieldName; }
		}
		protected bool IsFiltered {
			get { return !ReferenceEquals(ColumnCriteria, null); }
		}
		protected CriteriaOperator ColumnCriteria {
			get { return Grid.GetColumnFilterCriteria(Column); }
		}
		FilterData[] UpperFilters { get; set; }
		FilterData[] BottomFilters { get; set; }
		IEnumerable<FilterData> Filters {
			get {
				var upper = UpperFilters ?? new FilterData[] { };
				var bottom = BottomFilters ?? new FilterData[] { };
				return upper.Concat(bottom).Distinct(
					new AnonymousEqualityComparer<FilterData>(
						equals: (x, y) => x.FilterType == y.FilterType,
						getHashCode: x => x.GetHashCode()));
			}
		}
		protected bool CalendarFilterExists {
			get { return CalendarFilter != null; }
		}
		protected ICalendarFilter CalendarFilter { get; private set; }
		protected override bool ImmediateUpdateFilter {
			get { return Column.ImmediateUpdateColumnFilter; }
		}
		internal override PopupBaseEdit CreateColumnFilterPopup() {
			return new PopupBaseEdit {
				ShowNullText = false,
				IsTextEditable = false,
				DataContext = this,
				PopupContentTemplate = CreatePopupTemplate(),
			};
		}
		ControlTemplate CreatePopupTemplate() {
			string template =
				@"<ControlTemplate " +
				@"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""" + " " +
				@"xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">" +
				@"<dxg:" + CalendarFilterControl.ClassName + " " + @"Owner=""{Binding DataContext, RelativeSource={RelativeSource TemplatedParent}}""/>" +
				@"</ControlTemplate>";
			return XamlReader.Parse(template) as ControlTemplate;
		}
		protected override void UpdatePopupData(PopupBaseEdit popup) {
			if(UpperFilters == null) {
				UpdateUpperFilters();
			}
			if(BottomFilters == null) {
				UpdateBottomFilters();
			}
		}
		protected virtual void UpdateUpperFilters() {
			UpperFilters = CreateUpperFilters();
		}
		protected virtual void UpdateBottomFilters() {
			BottomFilters = CreateBottomFilters();
		}
		protected override void UpdateSizeGripVisibility(PopupBaseEdit popup) { }
		protected override void OnPopupOpenedCore(PopupBaseEdit popup) {
			UpdateShowAllCheckState();
			UpdateFilterBySpecificDateCheckState();
			UpdateSelectedDates();
			UpdateFiltersCheckState();
		}
		protected virtual void UpdateShowAllCheckState() {
			if(!CalendarFilterExists) return;
			if(!IsFiltered) {
				CalendarFilter.CheckShowAll();
				return;
			}
			CalendarFilter.UncheckShowAll();
		}
		protected virtual void UpdateFilterBySpecificDateCheckState() {
			if(!CalendarFilterExists) return;
			if(!IsFiltered) {
				CalendarFilter.UncheckFilterBySpecificDate();
				return;
			}
			var dates = ColumnCriteria.ToDates(FieldName);
			if(dates.Length == 0) return;
			CalendarFilter.CheckFilterBySpecificDate();
		}
		protected virtual void UpdateSelectedDates() {
			if(!CalendarFilterExists) return;
			if(!IsFiltered) {
				CalendarFilter.SelectedDates = null;
				return;
			}
			var dates = ColumnCriteria.ToDates(FieldName);
			CalendarFilter.SelectedDates = dates;
		}
		protected virtual void UpdateFiltersCheckState() {
			if(!CalendarFilterExists || !IsFiltered) return;
			var types = ColumnCriteria.ToFilters(FieldName);
			var data = MapToFilterData(types);
			CalendarFilter.CheckFilters(data);
		}
		FilterData[] MapToFilterData(FilterDateType[] types) {
			var map = Filters.ToDictionary(pair => pair.FilterType);
			return types.Where(x => map.ContainsKey(x)).Select(x => map[x]).ToArray();
		}
		protected override CriteriaOperator GetFilterCriteria(PopupBaseEdit popup) {
			return GetCriteria();
		}
		protected override void ClearPopupData(PopupBaseEdit popup) {
			if(!CalendarFilterExists) return;
			CalendarFilter.Dispose();
			CalendarFilter = null;
		}
		void OnShowAll() {
			if(!CalendarFilterExists) return;
			CalendarFilter.CheckShowAll();
			CalendarFilter.UncheckFilterBySpecificDate();
			CalendarFilter.UncheckAllFilters();
			ClearColumnFilter();
		}
		void OnFilterBySpecificDateChecked() {
			if(!CalendarFilterExists) return;
			CalendarFilter.UncheckShowAll();
			CalendarFilter.CheckFilterBySpecificDate();
			CalendarFilter.UncheckAllFilters();
			if(CalendarFilter.HasSelectedDates) {
				FilterBySelectedDates();
				return;
			}
			FilterByFocusedDate();
		}
		void OnFilterBySpecificDateUnchecked() {
			if(!CalendarFilterExists) return;
			CalendarFilter.CheckShowAll();
			CalendarFilter.UncheckFilterBySpecificDate();
			CalendarFilter.UncheckAllFilters();
			ClearColumnFilter();
		}
		void OnSelectedDatesChanged() {
			if(!CalendarFilterExists) return;
			CalendarFilter.UncheckShowAll();
			CalendarFilter.CheckFilterBySpecificDate();
			CalendarFilter.UncheckAllFilters();
			FilterBySelectedDates();
		}
		void OnFilterChecked(FilterData filter) {
			if(!CalendarFilterExists) return;
			CalendarFilter.UncheckShowAll();
			CalendarFilter.UncheckFilterBySpecificDate();
			CalendarFilter.SelectedDates = null;
			CalendarFilter.CheckFilters(filter);
			SetColumnFilter();
		}
		void OnFilterUnchecked(FilterData filter) {
			if(!CalendarFilterExists) return;
			CalendarFilter.UncheckShowAll();
			CalendarFilter.UncheckFilterBySpecificDate();
			CalendarFilter.UncheckFilters(filter);
			if(!CalendarFilter.HasCheckedFilters) {
				CalendarFilter.CheckShowAll();
			}
			SetColumnFilter();
		}
		void SetColumnFilter() {
			UpdateColumnFilterIfNeeded(() => GetCriteria());
		}
		void ClearColumnFilter() {
			UpdateColumnFilterIfNeeded(() => null);
		}
		void FilterByFocusedDate() {
			UpdateColumnFilterIfNeeded(() => GetCriteriaForFocusedDate());
		}
		void FilterBySelectedDates() {
			UpdateColumnFilterIfNeeded(() => GetCriteriaForSelectedDates());
		}
		CriteriaOperator GetCriteriaForFocusedDate() {
			if(!CalendarFilterExists) return null;
			return DatesToCriteria(new[] { CalendarFilter.FocusedDate });
		}
		CriteriaOperator GetCriteriaForSelectedDates() {
			if(!CalendarFilterExists || CalendarFilter.SelectedDates == null) return null;
			return DatesToCriteria(CalendarFilter.SelectedDates);
		}
		CriteriaOperator DatesToCriteria(IEnumerable<DateTime> dates) {
			return MultiselectRoundedDateTimeFilterHelper.DatesToCriteria(FieldName, dates);
		}
		CriteriaOperator GetCriteria() {
			if(!CalendarFilterExists) return null;
			if(CalendarFilter.IsShowAllChecked) return null;
			var activeFilters = CalendarFilter.CheckedFilters;
			if(activeFilters.Length == 0) return GetCriteriaForSelectedDates();
			var filters = activeFilters.Select(f => f.FilterType);
			return filters.ToCriteria(FieldName);
		}
		#region ICalendarFilterOwner members
		FilterData[] ICalendarFilterOwner.UpperFilters {
			get { return UpperFilters; }
		}
		FilterData[] ICalendarFilterOwner.BottomFilters {
			get { return BottomFilters; }
		}
		void ICalendarFilterOwner.SetCalendarFilter(ICalendarFilter calendar) {
			CalendarFilter = calendar;
		}
		void ICalendarFilterOwner.OnShowAll() {
			OnShowAll();
		}
		void ICalendarFilterOwner.OnFilterBySpecificDateChecked() {
			OnFilterBySpecificDateChecked();
		}
		void ICalendarFilterOwner.OnFilterBySpecificDateUnchecked() {
			OnFilterBySpecificDateUnchecked();
		}
		void ICalendarFilterOwner.OnFilterChecked(FilterData filter) {
			OnFilterChecked(filter);
		}
		void ICalendarFilterOwner.OnFilterUnchecked(FilterData filter) {
			OnFilterUnchecked(filter);
		}
		void ICalendarFilterOwner.OnSelectedDatesChanged() {
			OnSelectedDatesChanged();
		}
		#endregion
	}
}
