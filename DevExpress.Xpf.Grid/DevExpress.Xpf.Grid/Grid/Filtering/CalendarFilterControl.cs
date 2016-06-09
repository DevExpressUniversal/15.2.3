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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.DateNavigator;
using DevExpress.Xpf.Grid.Filtering;
namespace DevExpress.Xpf.Grid {
	[DXBrowsable(false)]
	[DXToolboxBrowsable(false)]
	public class CalendarFilterControl : ContentControl, ICalendarFilter {
		public const string ClassName = "CalendarFilterControl";
		readonly Locker eventLocker = new Locker();
		public static readonly DependencyProperty OwnerProperty =
			DependencyProperty.Register("Owner", typeof(ICalendarFilterOwner), typeof(CalendarFilterControl), new PropertyMetadata(null, (d, e) => (d as CalendarFilterControl).OnOwnerChanged()));
		public static readonly DependencyProperty DateFilterStyleProperty =
			DependencyProperty.Register("DateFilterStyle", typeof(Style), typeof(CalendarFilterControl), new PropertyMetadata(null));
		public CalendarFilterControl() {
			DefaultStyleKey = typeof(CalendarFilterControl);
		}
		public ICalendarFilterOwner Owner {
			get { return (ICalendarFilterOwner)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		public Style DateFilterStyle {
			get { return (Style)GetValue(DateFilterStyleProperty); }
			set { SetValue(DateFilterStyleProperty, value); }
		}
		public DateTime FocusedDate {
			get { return this.dateNavigator != null ? this.dateNavigator.FocusedDate : DateTime.Now.Date; }
		}
		public IList<DateTime> SelectedDates {
			get { return this.dateNavigator != null ? this.dateNavigator.SelectedDates : null; }
			set {
				if(this.dateNavigator == null) return;
				if(value == null || value.Count == 0) {
					if(!HasSelectedDates) return;
					this.dateNavigator.SelectedDates = null;
					return;
				}
				Locked(() => this.dateNavigator.SelectedDates = value);
			}
		}
		FilterData[] UpperFiltersData {
			get { return Owner != null ? Owner.UpperFilters : new FilterData[] { }; }
		}
		FilterData[] BottomFiltersData {
			get { return Owner != null ? Owner.BottomFilters : new FilterData[] { }; }
		}
		public bool HasCheckedFilters {
			get { return CheckedFilterElements.Any(); }
		}
		public bool HasSelectedDates {
			get { return SelectedDates != null && SelectedDates.Count != 0; }
		}
		public FilterData[] CheckedFilters {
			get { return CheckedFilterElements.Select(cb => cb.Tag as FilterData).ToArray(); }
		}
		public bool IsShowAllChecked {
			get { return this.cbShowAll != null ? (bool)this.cbShowAll.IsChecked : false; }
		}
		CheckBox cbShowAll;
		CheckBox cbFilterBySpecificDate;
		DateNavigator dateNavigator;
		Panel upperFiltersPanel;
		Panel bottomFiltersPanel;
		IEnumerable<CheckBox> CheckedFilterElements {
			get { return FilterElements.Where(cb => (bool)cb.IsChecked); }
		}
		IEnumerable<CheckBox> FilterElements {
			get { return UpperFilterElements.Concat(BottomFilterElements).Where(x => x != this.cbShowAll && x != this.cbFilterBySpecificDate); }
		}
		IEnumerable<CheckBox> BottomFilterElements {
			get { return this.bottomFiltersPanel != null ? this.bottomFiltersPanel.Children.Cast<CheckBox>() : Enumerable.Empty<CheckBox>(); }
		}
		IEnumerable<CheckBox> UpperFilterElements {
			get { return this.upperFiltersPanel != null ? this.upperFiltersPanel.Children.Cast<CheckBox>() : Enumerable.Empty<CheckBox>(); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Unsubscribe();
			this.upperFiltersPanel = GetTemplateChild("PART_UpperFiltersPanel") as Panel;
			this.bottomFiltersPanel = GetTemplateChild("PART_BottomFiltersPanel") as Panel;
			this.dateNavigator = GetTemplateChild("PART_DateNavigator") as DateNavigator;
			PopulateUpperFilters();
			PopulateBottomFilters();
			Subscribe();
		}
		void Subscribe() {
			SubscribeShowAll();
			SubscribeFilterBySpecificDate();
			SubscribeDateNavigator();
			SubscribeFilters();
		}
		void SubscribeDateNavigator() {
			if(this.dateNavigator == null) return;
			this.dateNavigator.SelectedDatesChanged += OnSelectedDatesChanged;
		}
		void SubscribeFilters() {
			ForEachFilterElement(SubscribeFilter);
		}
		void SubscribeFilter(CheckBox filter) {
			if(filter == null) return;
			filter.Checked += OnFilterChecked;
			filter.Unchecked += OnFilterUnchecked;
		}
		void SubscribeShowAll() {
			if(this.cbShowAll == null) return;
			this.cbShowAll.Checked += OnShowAll;
		}
		void SubscribeFilterBySpecificDate() {
			if(this.cbFilterBySpecificDate == null) return;
			this.cbFilterBySpecificDate.Checked += OnFilterBySpecificDate;
			this.cbFilterBySpecificDate.Unchecked += OnFilterBySpecificDateUnchecked;
		}
		void Unsubscribe() {
			UnsubscribeShowAll();
			UnsubscribeFilterBySpecificDate();
			UnsubscribeDateNavigator();
			UnsubscribeFilters();
		}
		void UnsubscribeDateNavigator() {
			if(this.dateNavigator == null) return;
			this.dateNavigator.SelectedDatesChanged -= OnSelectedDatesChanged;
		}
		void UnsubscribeFilters() {
			ForEachFilterElement(filter => UnsubscribeFilter(filter));
		}
		void UnsubscribeFilter(CheckBox filter) {
			if(filter == null) return;
			filter.Checked -= OnFilterChecked;
			filter.Unchecked -= OnFilterUnchecked;
		}
		void UnsubscribeShowAll() {
			if(this.cbShowAll == null) return;
			this.cbShowAll.Checked -= OnShowAll;
		}
		void UnsubscribeFilterBySpecificDate() {
			if(this.cbFilterBySpecificDate == null) return;
			this.cbFilterBySpecificDate.Checked -= OnFilterBySpecificDate;
			this.cbFilterBySpecificDate.Unchecked -= OnFilterBySpecificDateUnchecked;
		}
		void PopulateUpperFilters() {
			if(this.upperFiltersPanel == null || UpperFiltersData == null) return;
			var filters = CreateFilters(UpperFiltersData);
			filters.ForEach(x => this.upperFiltersPanel.Children.Add(x));
			this.cbShowAll = FindShowAll();
			this.cbFilterBySpecificDate = FindFilterBySpecificDate();
		}
		CheckBox FindShowAll() {
			return FindFilter(FilterDateType.None);
		}
		CheckBox FindFilterBySpecificDate() {
			return FindFilter(FilterDateType.SpecificDate);
		}
		void PopulateBottomFilters() {
			if(this.bottomFiltersPanel == null || BottomFiltersData == null) return;
			var filters = CreateFilters(BottomFiltersData);
			filters.ForEach(filter => bottomFiltersPanel.Children.Add(filter));
		}
		IEnumerable<CheckBox> CreateFilters(IEnumerable<FilterData> data) {
			if(data == null) return Enumerable.Empty<CheckBox>();
			return data.Select(CreateFilterElement);
		}
		CheckBox CreateFilterElement(FilterData filter) {
			return new CheckBox {
				Content = filter.Caption,
				ToolTip = !string.IsNullOrEmpty(filter.Tooltip) ? filter.Tooltip : null,
				Style = DateFilterStyle,
				Tag = filter,
				IsThreeState = false,
			};
		}
		CheckBox FindFilter(FilterDateType type) {
			return UpperFilterElements.Concat(BottomFilterElements).FirstOrDefault(x => x.Tag is FilterData && (x.Tag as FilterData).FilterType == type);
		}
		CheckBox FindFilter(FilterData filter) {
			return FilterElements.FirstOrDefault(x => x.Tag == filter);
		}
		FilterData ToFilterData(object filterElement) {
			var cb = filterElement as CheckBox;
			if(cb == null) return null;
			var data = cb.Tag as FilterData;
			if(data == null) return FilterData.Null;
			return data;
		}
		void ForEachFilterElement(Action<CheckBox> visitor) {
			FilterElements.ForEach(visitor);
		}
		void OnOwnerChanged() {
			if(Owner == null) return;
			Owner.SetCalendarFilter(this);
		}
		void OnShowAll(object sender, RoutedEventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnShowAll());
		}
		void OnFilterBySpecificDate(object sender, RoutedEventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnFilterBySpecificDateChecked());
		}
		void OnFilterBySpecificDateUnchecked(object sender, RoutedEventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnFilterBySpecificDateUnchecked());
		}
		void OnFilterChecked(object sender, RoutedEventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnFilterChecked(ToFilterData(sender)));
		}
		void OnFilterUnchecked(object sender, RoutedEventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnFilterUnchecked(ToFilterData(sender)));
		}
		void OnSelectedDatesChanged(object sender, EventArgs e) {
			if(Owner == null) return;
			LockedIfCan(() => Owner.OnSelectedDatesChanged());
		}
		void LockedIfCan(Action action) {
			this.eventLocker.DoLockedActionIfNotLocked(action);
		}
		void Locked(Action action) {
			this.eventLocker.DoLockedAction(action);
		}
		public void UncheckAllFilters() {
			ForEachFilterElement(x => Uncheck(x));
		}
		public void CheckFilters(params FilterData[] filters) {
			filters.ForEach(x => SetFilterChecked(x, @checked: true));
		}
		public void UncheckFilters(params FilterData[] filters) {
			filters.ForEach(x => SetFilterChecked(x, @checked: false));
		}
		void SetFilterChecked(FilterData filter, bool @checked) {
			var filterElement = FindFilter(filter);
			SetChecked(filterElement, @checked);
		}
		public void CheckShowAll() {
			Check(this.cbShowAll);
		}
		public void UncheckShowAll() {
			Uncheck(this.cbShowAll);
		}
		public void CheckFilterBySpecificDate() {
			Check(this.cbFilterBySpecificDate);
		}
		public void UncheckFilterBySpecificDate() {
			Uncheck(this.cbFilterBySpecificDate);
		}
		void Check(CheckBox cb) {
			SetChecked(cb, @checked: true);
		}
		void Uncheck(CheckBox cb) {
			SetChecked(cb, @checked: false);
		}
		void SetChecked(CheckBox cb, bool @checked) {
			if(cb == null) return;
			Locked(() => cb.IsChecked = @checked);
		}
		public void RebuildFilters() {
			UnsubscribeFilters();
			ClearBottomFilters();
			PopulateBottomFilters();
			SubscribeFilters();
		}
		void ClearBottomFilters() {
			if(this.bottomFiltersPanel == null) return;
			this.bottomFiltersPanel.Children.Clear();
		}
		public void Dispose() {
			Unsubscribe();
			if(Owner == null) return;
			Owner.SetCalendarFilter(null);
			Owner = null;
			ClearValue(OwnerProperty);
		}
	}
}
