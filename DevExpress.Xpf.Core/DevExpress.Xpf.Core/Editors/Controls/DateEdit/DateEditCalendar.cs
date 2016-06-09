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
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.Validation.Native;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TransferContentControl = DevExpress.Xpf.Core.TransitionContentControl;
using TransferControl = DevExpress.Xpf.Core.TransitionControl;
#endif
namespace DevExpress.Xpf.Editors.Popups.Calendar {
	public enum DateEditCalendarState { Month, Year, Years, YearsGroup }
	public partial class CalendarCellButton : Button {
		static CalendarCellButton() {
			Type ownerType = typeof(CalendarCellButton);
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
#endif
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
#else
		protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
#endif
			if (e.Property == DateEditCalendar.CellInactiveProperty || e.Property == DateEditCalendar.CellTodayProperty || e.Property == DateEditCalendar.CellFocusedProperty)
				UpdateVisualState(true);
		}
		protected virtual void UpdateVisualState(bool useTransitions) {
			if(DateEditCalendar.GetCellFocused(this))
				VisualStateManager.GoToState(this, "CellFocusedState", true);
			else if(DateEditCalendar.GetCellToday(this))
				VisualStateManager.GoToState(this, "CellTodayState", true);
			else if (DateEditCalendar.GetCellInactive(this))
				VisualStateManager.GoToState(this, "CellInactiveState", true);
			else
				VisualStateManager.GoToState(this, "CellNormalState", true);
		}
	}
	public partial class DateEditCalendar : DateEditCalendarBase {
		#region static
		protected static readonly DependencyPropertyKey DateTimeTextPropertyKey;
		public static readonly DependencyProperty DateTimeTextProperty;
		protected static readonly DependencyPropertyKey CurrentDateTextPropertyKey;
		public static readonly DependencyProperty CurrentDateTextProperty;
		public static readonly DependencyProperty WeekdayAbbreviationStyleProperty;
		public static readonly DependencyProperty WeekNumbersStyleProperty;
		public static readonly DependencyProperty CellButtonStyleProperty;
		public static readonly DependencyProperty MonthInfoTemplateProperty;
		public static readonly DependencyProperty YearInfoTemplateProperty;
		public static readonly DependencyProperty YearsInfoTemplateProperty;
		public static readonly DependencyProperty YearsGroupInfoTemplateProperty;
		public static readonly DependencyProperty CalendarTransferStyleProperty;
		public static readonly DependencyProperty CellInactiveProperty;
		public static readonly DependencyProperty CellTodayProperty;
		public static readonly DependencyProperty CellFocusedProperty;
		public static readonly DependencyProperty CalendarProperty;
		static DateEditCalendar() {
			Type ownerType = typeof(DateEditCalendar);
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DateEditCalendar), new FrameworkPropertyMetadata(ownerType));
#endif
			CurrentDateTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("CurrentDateText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
			CurrentDateTextProperty = CurrentDateTextPropertyKey.DependencyProperty;
			DateTimeTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DateTimeText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
			DateTimeTextProperty = DateTimeTextPropertyKey.DependencyProperty;
			WeekdayAbbreviationStyleProperty = DependencyPropertyManager.Register("WeekdayAbbreviationStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CellButtonStyleProperty = DependencyPropertyManager.Register("CellButtonStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			WeekNumbersStyleProperty = DependencyPropertyManager.Register("WeekNumbersStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MonthInfoTemplateProperty = DependencyPropertyManager.Register("MonthInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearInfoTemplateProperty = DependencyPropertyManager.Register("YearInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearsInfoTemplateProperty = DependencyPropertyManager.Register("YearsInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearsGroupInfoTemplateProperty = DependencyPropertyManager.Register("YearsGroupInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CalendarTransferStyleProperty = DependencyPropertyManager.Register("CalendarTransferStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CellInactiveProperty = DependencyPropertyManager.RegisterAttached("CellInactive", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			CellTodayProperty = DependencyPropertyManager.RegisterAttached("CellToday", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			CellFocusedProperty = DependencyPropertyManager.RegisterAttached("CellFocused", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			CalendarProperty = DependencyPropertyManager.RegisterAttached("Calendar", typeof(DateEditCalendar), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		}
		public static DateEditCalendar GetCalendar(DependencyObject obj) {
			return (DateEditCalendar)DependencyObjectHelper.GetValueWithInheritance(obj, CalendarProperty);
		}
		private static void SetCalendar(DependencyObject obj, DateEditCalendar value) { obj.SetValue(CalendarProperty, value); }
		public static bool GetCellInactive(DependencyObject obj) { return (bool)obj.GetValue(CellInactiveProperty); }
		public static void SetCellInactive(DependencyObject obj, bool value) { obj.SetValue(CellInactiveProperty, value); }
		public static bool GetCellToday(DependencyObject obj) { return (bool)obj.GetValue(CellTodayProperty); }
		public static void SetCellToday(DependencyObject obj, bool value) { obj.SetValue(CellTodayProperty, value); }
		public static bool GetCellFocused(DependencyObject obj) { return (bool)obj.GetValue(CellFocusedProperty); }
		public static void SetCellFocused(DependencyObject obj, bool value) { obj.SetValue(CellFocusedProperty, value); }
#if SL
		public ContentControl WeekNumberDelimeter { get { return FindName("WeekNumberDelimeter") as ContentControl; } }
#endif
		#endregion
		protected override void OnShowClearButtonChanged() {
			UpdateClearButtonVisibility();
		}
		protected override void OnShowTodayChanged() {
			UpdateTodayButtonVisibility();
		}
		void UpdateClearButtonVisibility() {
			UpdateButtonVisibility(ClearButton, ShowClearButton);
		}
		void UpdateTodayButtonVisibility() {
			UpdateButtonVisibility(TodayButton, ShowToday);
		}
		void UpdateButtonVisibility(Button button, bool show) {
			if (button != null)
				button.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
		}
		public DateEditCalendar() {
			UpdateDateTimeText();
			SetCalendar(this, this);
#if SL
			DefaultStyleKey = typeof(DateEditCalendar);
#endif
		}
		public void SetNewDateTime(DateTime dateTime) {
			ActiveContent.DateTime = dateTime;
			SetNewContent(dateTime, DateEditCalendarTransferType.None);
		}
		protected override void ShowWeekNumbersPropertySet(bool value) {
		}
		public ControlTemplate MonthInfoTemplate {
			get { return (ControlTemplate)GetValue(MonthInfoTemplateProperty); }
			set { SetValue(MonthInfoTemplateProperty, value); }
		}
		public ControlTemplate YearInfoTemplate {
			get { return (ControlTemplate)GetValue(YearInfoTemplateProperty); }
			set { SetValue(YearInfoTemplateProperty, value); }
		}
		public ControlTemplate YearsInfoTemplate {
			get { return (ControlTemplate)GetValue(YearsInfoTemplateProperty); }
			set { SetValue(YearsInfoTemplateProperty, value); }
		}
		public ControlTemplate YearsGroupInfoTemplate {
			get { return (ControlTemplate)GetValue(YearsGroupInfoTemplateProperty); }
			set { SetValue(YearsGroupInfoTemplateProperty, value); }
		}
		public Style CalendarTransferStyle {
			get { return (Style)GetValue(CalendarTransferStyleProperty); }
			set { SetValue(CalendarTransferStyleProperty, value); }
		}
		public Style WeekNumbersStyle {
			get { return (Style)GetValue(WeekNumbersStyleProperty); }
			set { SetValue(WeekNumbersStyleProperty, value); }
		}
		public Style WeekdayAbbreviationStyle {
			get { return (Style)GetValue(WeekdayAbbreviationStyleProperty); }
			set { SetValue(WeekdayAbbreviationStyleProperty, value); }
		}
		public Style CellButtonStyle {
			get { return (Style)GetValue(CellButtonStyleProperty); }
			set { SetValue(CellButtonStyleProperty, value); }
		}
		public string CurrentDateText {
			get { return (string)GetValue(CurrentDateTextProperty); }
			protected internal set { this.SetValue(CurrentDateTextPropertyKey, value); }
		}
		public string DateTimeText {
			get { return (string)GetValue(DateTimeTextProperty); }
			protected internal set { this.SetValue(DateTimeTextPropertyKey, value); }
		}
		DateEditCalendarContent prevContent;
		protected internal DateEditCalendarContent ActiveContent { get { return CalendarTransfer == null ? null : CalendarTransfer.Content as DateEditCalendarContent; } }
		protected internal DateEditCalendarContent PrevContent { get { return prevContent; } }
		public DateEdit OwnerDateEdit { get { return BaseEdit.GetOwnerEdit(this) as DateEdit; } }
		Button todayButton;
		protected Button TodayButton {
			get { return todayButton; }
			set {
				if (TodayButton != null)
					UnsubscribeTodayButtonEvent();
				todayButton = value;
				if (TodayButton != null)
					SubscribeTodayButtonEvents();
			}
		}
		Button currentDateButton;
		protected internal Button CurrentDateButton {
			get { return currentDateButton; }
			set {
				if (CurrentDateButton != null)
					UnsubscribeCurrentDateButtonEvent();
				currentDateButton = value;
				if (CurrentDateButton != null)
					SubscribeCurrentDateButtonEvent();
			}
		}
		Button clearButton;
		protected internal Button ClearButton {
			get { return clearButton; }
			set {
				if (ClearButton != null)
					UnsubscribeClearButtonEvent();
				clearButton = value;
				if (ClearButton != null)
					SubscribeClearButtonEvent();
			}
		}
		RepeatButton leftArrowButton;
		protected internal RepeatButton LeftArrowButton {
			get { return leftArrowButton; }
			set {
				if (LeftArrowButton != null)
					UnsubscribeLeftArrowButtonEvent();
				leftArrowButton = value;
				if (LeftArrowButton != null)
					SubscribeLeftArrowButtonEvent();
			}
		}
		RepeatButton rightArrowButton;
		protected internal RepeatButton RightArrowButton {
			get { return rightArrowButton; }
			set {
				if (RightArrowButton != null)
					UnsubscribeRightArrowButtonEvent();
				rightArrowButton = value;
				if (RightArrowButton != null)
					SubscribeRightArrowButtonEvent();
			}
		}
		protected virtual void UpdateDateTimeText() {
			DateTimeText = GetTodayText();
		}
		protected virtual void UpdateActiveContent() {
			if (ActiveContent == null)
				return;
			SetNewContent(DateTime, DateEditCalendarTransferType.None);
		}
		protected override void MinValueChanged() {
			UpdateActiveContent();
		}
		protected override void MaxValueChanged() {
			UpdateActiveContent();
		}
		protected override void OnDateTimeChanged() {
			UpdateDateTimeText();
			if (OwnerDateEdit == null)
				SetNewContent(DateTime, DateEditCalendarState.Month, MonthInfoTemplate, DateEditCalendarTransferType.None);
		}
		protected virtual void SubscribeLeftArrowButtonEvent() {
			LeftArrowButton.Click += new RoutedEventHandler(OnLeftArrowButtonClick);
		}
		protected virtual void UnsubscribeLeftArrowButtonEvent() {
			LeftArrowButton.Click -= new RoutedEventHandler(OnLeftArrowButtonClick);
		}
		protected virtual ControlTemplate GetTemplate(DateEditCalendarState state) {
			switch (state) {
				case DateEditCalendarState.Month:
					return MonthInfoTemplate;
				case DateEditCalendarState.Year:
					return YearInfoTemplate;
				case DateEditCalendarState.Years:
					return YearsInfoTemplate;
				case DateEditCalendarState.YearsGroup:
					return YearsGroupInfoTemplate;
			}
			return null;
		}
		protected bool IsSamePage(DateEditCalendarState state, DateTime dt1, DateTime dt2) {
			switch (state) {
				case DateEditCalendarState.Month:
					return dt1.Month == dt2.Month && dt1.Year == dt2.Year;
				case DateEditCalendarState.Year:
					return dt1.Year == dt2.Year;
				case DateEditCalendarState.Years:
					return dt1.Year / 10 == dt2.Year / 10;
				case DateEditCalendarState.YearsGroup:
					return dt1.Year / 100 == dt2.Year / 100;
			}
			return false;
		}
		protected internal virtual DateTime UpdateDate(DateEditCalendarState state, DateTime dt, int dir) {
			int years = 0, months = 0;
			if (state == DateEditCalendarState.Month)
				months = dir;
			else if (state == DateEditCalendarState.Year)
				years = dir;
			else if (state == DateEditCalendarState.Years) {
				if ((dt.Year == 10 && dir < 0) || (dt.Year == 1 && dir > 0))
					years = dir * 9;
				else
					years = dir * 10;
			} else if (state == DateEditCalendarState.YearsGroup) {
				if ((dt.Year == 100 && dir < 0) || (dt.Year == 1 && dir > 0))
					years = dir * 99;
				else
					years = dir * 100;
			}
			return AddDate(dt, years, months, 0);
		}
		protected internal DateTime GetMinValue() {
			if(MinValue.HasValue)
				return MinValue.Value;
			return DateTime.MinValue;
		}
		protected internal DateTime GetMaxValue() {
			if(MaxValue.HasValue)
				return MaxValue.Value;
			return DateTime.MaxValue;
		}
		protected internal virtual DateTime AddDate(DateTime dt, int years, int months, int days) {
			if (dt == new DateTime() && (years < 0 || months < 0 || days < 0))
				dt = GetMinValue();
			else
				try {
					dt = dt.AddDays(days);
					dt = dt.AddMonths(months);
					dt = dt.AddYears(years);
				} catch {
					if(years < 0 || (years == 0 && months < 0) || (years == 0 && months == 0 && days < 0)) {
						dt = GetMinValue();
					}
					else
						dt = GetMaxValue();
				}
			if(dt < GetMinValue())
				dt = GetMinValue();
			if (dt > GetMaxValue())
				dt = GetMaxValue();
			return dt;
		}
		protected virtual bool CanShift(int dir, DateTime currDate) {
			DateTime dt = UpdateDate(ActiveContent.State, currDate, dir);
			return !IsSamePage(ActiveContent.State, currDate, dt);
		}
		protected virtual bool CanShiftLeft(DateTime dt) {
			return CanShift(-1, dt);
		}
		protected virtual bool CanShiftRight(DateTime dt) {
			return CanShift(1, dt);
		}
		protected virtual void OnLeftArrowButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			NavigateLeft();
		}
		protected virtual void NavigateLeft() {
			if (!CanShiftLeft(DateTime))
				return;
			DateTime = UpdateDate(ActiveContent.State, DateTime, -1);
			SetNewContent(DateTime, ActiveContent.State, GetTemplate(ActiveContent.State), DateEditCalendarTransferType.ShiftRight);
		}
		protected virtual void SubscribeRightArrowButtonEvent() {
			RightArrowButton.Click += OnRightArrowButtonClick;
		}
		protected virtual void UnsubscribeRightArrowButtonEvent() {
			RightArrowButton.Click -= OnRightArrowButtonClick;
		}
		protected virtual void OnRightArrowButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			NavigateRight();
		}
		protected virtual void NavigateRight() {
			if (!CanShiftRight(DateTime))
				return;
			DateTime = UpdateDate(ActiveContent.State, DateTime, +1);
			SetNewContent(DateTime, ActiveContent.State, GetTemplate(ActiveContent.State), DateEditCalendarTransferType.ShiftLeft);
		}
		protected virtual void SubscribeClearButtonEvent() {
			ClearButton.Click += new RoutedEventHandler(OnClearButtonClick);
		}
		protected virtual void UnsubscribeClearButtonEvent() {
			ClearButton.Click -= new RoutedEventHandler(OnClearButtonClick);
		}
		protected virtual void OnClearButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (OwnerDateEdit == null)
				return;
			OwnerDateEdit.ClearError();
			OwnerDateEdit.EditValue = OwnerDateEdit.NullValue;
			OwnerDateEdit.IsPopupOpen = false;
		}
		protected virtual void SubscribeCurrentDateButtonEvent() {
			CurrentDateButton.Click += new RoutedEventHandler(OnCurrentDateButtonClick);
		}
		protected virtual void UnsubscribeCurrentDateButtonEvent() {
			CurrentDateButton.Click -= new RoutedEventHandler(OnCurrentDateButtonClick);
		}
		protected virtual void OnCurrentDateButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (ActiveContent.State == DateEditCalendarState.Month)
				SetNewContent(DateTime, DateEditCalendarState.Year, YearInfoTemplate, DateEditCalendarTransferType.ZoomOut);
			else if (ActiveContent.State == DateEditCalendarState.Year)
				SetNewContent(DateTime, DateEditCalendarState.Years, YearsInfoTemplate, DateEditCalendarTransferType.ZoomOut);
			else if (ActiveContent.State == DateEditCalendarState.Years)
				SetNewContent(DateTime, DateEditCalendarState.YearsGroup, YearsGroupInfoTemplate, DateEditCalendarTransferType.ZoomOut);
		}
		protected virtual void SubscribeTodayButtonEvents() {
			TodayButton.Click += new RoutedEventHandler(OnTodayButtonClick);
		}
		protected virtual void UnsubscribeTodayButtonEvent() {
			TodayButton.Click -= new RoutedEventHandler(OnTodayButtonClick);
		}
		protected virtual void OnTodayButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			DateEditCalendarTransferType animationType;
			if(ActiveContent.State == DateEditCalendarState.Month)
				animationType = DateTime.Month == DateTime.Now.Month && DateTime.Year == DateTime.Now.Year ? DateEditCalendarTransferType.None : DateEditCalendarTransferType.ShiftLeft;
			else
				animationType = DateEditCalendarTransferType.ZoomIn;
			SetNewContent(
				new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond), 
				DateEditCalendarState.Month,
				MonthInfoTemplate,
				animationType
			);
			DateTime = ActiveContent.DateTime;
		}
		protected internal virtual void OnCellButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (ActiveContent.State == DateEditCalendarState.Month)
				OnDayCellButtonClick(sender as Button);
			else if (ActiveContent.State == DateEditCalendarState.Year)
				OnMonthCellButtonClick(sender as Button);
			else if (ActiveContent.State == DateEditCalendarState.Years)
				OnYearCellButtonClick(sender as Button);
			else
				OnYearsGroupCellButtonClick(sender as Button);
		}
		int GetRoundDay(int year, int month, int day) {
			int maxDay = DateTime.DaysInMonth(year, month);
			return day > maxDay ? maxDay : day;
		}
		protected void SetMonth(DateTime newDateTime) {
			DateTime = new DateTime(DateTime.Year, newDateTime.Month, GetRoundDay(DateTime.Year, newDateTime.Month, DateTime.Day), DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);
		}
		protected void SetYear(DateTime newDateTime) {
			DateTime = new DateTime(newDateTime.Year, DateTime.Month, GetRoundDay(newDateTime.Year, DateTime.Month, DateTime.Day), DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);
		}
		protected void SetYearGroup(DateTime newDateTime) {
			int startYear = Math.Max(newDateTime.Year, 1);
			DateTime = new DateTime(DateTime.Year % 10 + startYear, DateTime.Month, GetRoundDay(DateTime.Year % 10 + startYear, DateTime.Month, DateTime.Day), DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);
		}
		protected virtual void OnMonthCellButtonClick(Button button) {
			SetMonth((DateTime)DateEditCalendar.GetDateTime(button));
			SetNewContent(DateTime, DateEditCalendarState.Month, MonthInfoTemplate, DateEditCalendarTransferType.ZoomIn);
		}
		protected virtual void OnYearCellButtonClick(Button button) {
			SetYear((DateTime)DateEditCalendar.GetDateTime(button));
			SetNewContent(DateTime, DateEditCalendarState.Year, YearInfoTemplate, DateEditCalendarTransferType.ZoomIn);
		}
		protected virtual void OnYearsGroupCellButtonClick(Button button) {
			SetYearGroup((DateTime)DateEditCalendar.GetDateTime(button));
			SetNewContent(DateTime, DateEditCalendarState.Years, YearsInfoTemplate, DateEditCalendarTransferType.ZoomIn);
		}
		protected virtual void OnDayCellButtonClick(Button button) {
			if(OwnerDateEdit == null) {
				if (button != null)
					DateTime = ((DateTime)DateEditCalendar.GetDateTime(button));
				return;
			}
			if(!OwnerDateEdit.IsReadOnly)
				OwnerDateEdit.SetDateTime((DateTime)DateEditCalendar.GetDateTime(button), UpdateEditorSource.ValueChanging);
			OwnerDateEdit.IsPopupOpen = false;
		}
		protected virtual void FindButtonsInTemplate() {
			TodayButton = (Button)GetTemplateChild("PART_Today");
			UpdateTodayButtonVisibility();
			CurrentDateButton = (Button)GetTemplateChild("PART_CurrentDate");
			ClearButton = (Button)GetTemplateChild("PART_Clear");
			UpdateClearButtonVisibility();
			LeftArrowButton = (RepeatButton)GetTemplateChild("PART_LeftArrow");
			RightArrowButton = (RepeatButton)GetTemplateChild("PART_RightArrow");
		}
		public DateEditCalendar Calendar {
			get { return (DateEditCalendar)GetValue(CalendarProperty); }
			set { SetValue(CalendarProperty, value); }
		}
		protected internal DateEditCalendarTransferControl CalendarTransfer { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (OwnerDateEdit != null)
				OwnerDateEdit.OnApplyCalendarTemplate(this);
			FindButtonsInTemplate();
			this.CalendarTransfer = GetTemplateChild("PART_CalendarTransferContent") as DateEditCalendarTransferControl;
			DateTime = UpdateDate(DateEditCalendarState.Month, DateTime, 0);
			SetNewContent(DateTime, DateEditCalendarState.Month, MonthInfoTemplate, DateEditCalendarTransferType.None);
		}
		protected internal virtual void SetNewContent(DateTime dt, DateEditCalendarTransferType transferType) {
			SetNewContent(dt, ActiveContent.State, GetTemplate(ActiveContent.State), transferType);
		}
		protected internal virtual void SetNewContent(DateTime dt, DateEditCalendarState state, ControlTemplate template, DateEditCalendarTransferType transferType) {
			prevContent = ActiveContent;
			if(CalendarTransfer == null)
				return;
			DateEditCalendarContent calendarContent = new DateEditCalendarContent();
			CalendarTransfer.TransferType = transferType;
			calendarContent.State = state;
			calendarContent.Template = template;
			calendarContent.DateTime = dt;
			CalendarTransfer.Content = calendarContent;
			if (LeftArrowButton != null)
				LeftArrowButton.IsEnabled = CanShiftLeft(dt);
			if (RightArrowButton != null)
				RightArrowButton.IsEnabled = CanShiftRight(dt);
		}
		protected virtual string GetTodayText() {
			return DateTime.Today.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture);
		}
		protected virtual string GetMonthName(int month) {
			return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1];
		}
		protected internal virtual void OnApplyContentTemplate(DateEditCalendarContent content) {
			if (ActiveContent == content) {
				CurrentDateText = content.GetCurrentDateText();
			}
		}
		DateEditCalendarNavigatorBase navigator;
		protected DateEditCalendarNavigatorBase Navigator {
			get {
				if (navigator == null)
					navigator = CreateNavigator(this);
				return navigator;
			}
		}
		protected virtual DateEditCalendarNavigatorBase CreateNavigator(DateEditCalendar dateEditCalendar) {
			return new DateEditCalendarNavigator(this);
		}
		protected internal override bool ProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
			return Navigator.ProcessKeyDown(e);
		}
	}
	public class DateEditCalendarNavigatorBase {
		DateEditCalendar calendar;
		public DateEditCalendarNavigatorBase(DateEditCalendar calendar) {
			this.calendar = calendar;
		}
		public DateEditCalendar Calendar { get { return calendar; } }
		protected virtual bool OnLeft() { return false; }
		protected virtual bool OnRight() { return false; }
		protected virtual bool OnUp() { return false; }
		protected virtual bool OnDown() { return false; }
		protected virtual bool OnPageUp() { return false; }
		protected virtual bool OnPageDown() { return false; }
		protected virtual bool OnHome() { return false; }
		protected virtual bool OnEnd() { return false; }
		protected virtual bool OnEnter() { return false; }
		public virtual bool ProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
			switch (e.Key) {
				case System.Windows.Input.Key.Up:
					return OnUp();
				case System.Windows.Input.Key.Down:
					return OnDown();
				case System.Windows.Input.Key.Left:
					return (Calendar.FlowDirection == FlowDirection.LeftToRight) ? OnLeft() : OnRight();
				case System.Windows.Input.Key.Right:
					return (Calendar.FlowDirection == FlowDirection.LeftToRight) ? OnRight() : OnLeft();
				case System.Windows.Input.Key.PageUp:
					return OnPageUp();
				case System.Windows.Input.Key.PageDown:
					return OnPageDown();
				case System.Windows.Input.Key.Home:
					return OnHome();
				case System.Windows.Input.Key.End:
					return OnEnd();
				case System.Windows.Input.Key.Space:
				case System.Windows.Input.Key.Enter:
					return OnEnter();
			}
			return false;
		}
	}
	public class DateEditCalendarNavigator : DateEditCalendarNavigatorBase {
		public DateEditCalendarNavigator(DateEditCalendar calendar) : base(calendar) { }
		protected virtual FrameworkElement FindFocusedCell() {
			if (Calendar.ActiveContent.ContentGrid == null)
				return null;
			return Calendar.ActiveContent.GetFocusedCell();
		}
		protected virtual FrameworkElement FindCell(int row, int col) {
			if (Calendar.ActiveContent.ContentGrid == null)
				return null;
			foreach (UIElement elem in Calendar.ActiveContent.ContentGrid.Children) {
				if (Grid.GetColumn(elem as FrameworkElement) == col && Grid.GetRow(elem as FrameworkElement) == row && DateEditCalendar.GetDateTime(elem) != null)
					return elem as FrameworkElement;
			}
			return null;
		}
		protected delegate FrameworkElement FindNextCell();
		protected delegate void ShiftMethod(DateTime dt);
		protected delegate bool DateRelation(DateTime current, DateTime dt, DateTime bound);
		protected virtual bool IsNearLess(DateTime current, DateTime dt, DateTime bound) {
			return dt < current && dt > bound;
		}
		protected virtual bool IsNearGreater(DateTime current, DateTime dt, DateTime bound) {
			return dt > current && dt < bound;
		}
		protected virtual FrameworkElement FindCellWithDate(DateTime dt, DateRelation relation, DateTime startBound) {
			DateTime boundDate = startBound;
			FrameworkElement lastElem = null;
			if (Calendar.ActiveContent.ContentGrid == null)
				return null;
			foreach (UIElement elem in Calendar.ActiveContent.ContentGrid.Children) {
				if (DateEditCalendar.GetDateTime(elem) == null)
					continue;
				DateTime currDate = (DateTime)DateEditCalendar.GetDateTime(elem);
				if (relation(dt, currDate, boundDate)) {
					boundDate = currDate;
					lastElem = elem as FrameworkElement;
				}
			}
			return lastElem;
		}
		protected virtual FrameworkElement FindCellWithLessDate() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCellWithDate((DateTime)DateEditCalendar.GetDateTime(elem), IsNearLess, DateTime.MinValue);
		}
		protected virtual FrameworkElement FindCellWithGreaterDate() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCellWithDate((DateTime)DateEditCalendar.GetDateTime(elem), IsNearGreater, DateTime.MaxValue);
		}
		protected virtual FrameworkElement FindRightCellFromFocused() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCell(Grid.GetRow(elem), Grid.GetColumn(elem) + 1);
		}
		protected virtual FrameworkElement FindLeftCellFromFocused() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCell(Grid.GetRow(elem), Grid.GetColumn(elem) - 1);
		}
		protected virtual FrameworkElement FindUpCellFromFocused() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCell(Grid.GetRow(elem) - 1, Grid.GetColumn(elem));
		}
		protected virtual FrameworkElement FindDownCellFromFocused() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCell(Grid.GetRow(elem) + 1, Grid.GetColumn(elem));
		}
		protected virtual void ShiftLeft(DateTime dt) {
			Calendar.SetNewContent(dt, DateEditCalendarTransferType.ShiftLeft);
		}
		protected virtual void ShiftRight(DateTime dt) {
			Calendar.SetNewContent(dt, DateEditCalendarTransferType.ShiftRight);
		}
		protected virtual void Shift(DateTime dt, int dir) {
			if (dir > 0)
				ShiftRight(dt);
			else
				ShiftLeft(dt);
		}
		protected virtual DateTime GetNextDate(FrameworkElement focusedCell, FrameworkElement nextCell, int upDownDir, int leftRightDir) {
			if (nextCell != null)
				return (DateTime)DateEditCalendar.GetDateTime(nextCell);
			DateTime dt = (DateTime)DateEditCalendar.GetDateTime(focusedCell);
			switch (Calendar.ActiveContent.State) {
				case DateEditCalendarState.Month:
					return Calendar.AddDate(dt, 0, 0, upDownDir != 0 ? 7 * upDownDir : leftRightDir);
				case DateEditCalendarState.Year:
					return Calendar.AddDate(dt, 0, upDownDir != 0 ? 4 * upDownDir : leftRightDir, 0);
				case DateEditCalendarState.Years:
					return Calendar.AddDate(dt, upDownDir != 0 ? 4 * upDownDir : leftRightDir, 0, 0);
				case DateEditCalendarState.YearsGroup:
					return Calendar.AddDate(dt, upDownDir != 0 ? 40 * upDownDir : leftRightDir, 0, 0);
			}
			return dt;
		}
		protected virtual bool OnArrowKey(FindNextCell method, int upDownDir, int leftRightDir) {
			FrameworkElement focusedCell = FindFocusedCell();
			if (focusedCell == null)
				return true;
			FrameworkElement nextCell = method();
			if (nextCell != null) {
				DateEditCalendar.SetCellFocused(focusedCell, false);
				if (!DateEditCalendar.GetCellInactive(nextCell)) {
					DateEditCalendar.SetCellFocused(nextCell, true);
					return true;
				}
			}
			Shift(GetNextDate(focusedCell, nextCell, upDownDir, leftRightDir), -(upDownDir != 0 ? upDownDir : leftRightDir));
			return true;
		}
		protected override bool OnRight() {
			return OnArrowKey(FindCellWithGreaterDate, 0, 1);
		}
		protected override bool OnLeft() {
			return OnArrowKey(FindCellWithLessDate, 0, -1);
		}
		protected override bool OnUp() {
			return OnArrowKey(FindUpCellFromFocused, -1, 0);
		}
		protected override bool OnDown() {
			return OnArrowKey(FindDownCellFromFocused, 1, 0);
		}
		protected override bool OnEnter() {
			FrameworkElement focusedCell = FindFocusedCell();
			if (focusedCell == null)
				return false;
			Calendar.OnCellButtonClick(focusedCell, new System.Windows.RoutedEventArgs());
			return true;
		}
	}
	public enum DateEditCalendarTransferType { None, ShiftLeft, ShiftRight, ZoomIn, ZoomOut }
	[ToolboxItem(false)]
	public class DateEditCalendarContent : ContentControl {
		#region static
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty DateTimeProperty;
		static DateEditCalendarContent() {
			StateProperty = DependencyPropertyManager.Register("State", typeof(DateEditCalendarState), typeof(DateEditCalendarContent), new FrameworkPropertyMetadata(DateEditCalendarState.Month, FrameworkPropertyMetadataOptions.None));
			DateTimeProperty = DependencyPropertyManager.Register("DateTime", typeof(DateTime), typeof(DateEditCalendarContent), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDateTimePropertyChanged)));
		}
		protected static void OnDateTimePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEditCalendarContent)obj).OnDateTimeChanged();
		}
		#endregion
		bool makePrevZoomOut = false;
		bool makeCurrZoomOut = false;
		bool makePrevZoomIn = false;
		bool makeCurrZoomIn = false;
		TransferContentControl currTransferContent = null;
		TransferContentControl prevTransferContent = null;
		public DateTime DateTime {
			get { return (DateTime)GetValue(DateTimeProperty); }
			set { SetValue(DateTimeProperty, value); }
		}
		public DateEditCalendarState State {
			get { return (DateEditCalendarState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public DateEdit OwnerDateEdit { get { return (DateEdit)BaseEdit.GetOwnerEdit(this); } }
		public DateEditCalendar Calendar { get { return DateEditCalendar.GetCalendar(this); } }
		protected int FirstCellIndex { get; set; }
		protected DateTime FirstVisibleDate { get; set; }
		protected DateTimeFormatInfo DateTimeFormat { get { return CultureInfo.CurrentCulture.DateTimeFormat; } }
		protected DayOfWeek FirstDayOfWeek { get { return DateTimeFormat.FirstDayOfWeek; } }
		protected internal virtual string GetCurrentDateText() {
			if (State == DateEditCalendarState.Month)
				return string.Format("{0:" + CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern + "}", DateTime);
			if (State == DateEditCalendarState.Year)
				return DateTime.ToString("yyyy", CultureInfo.CurrentCulture);
			if (State == DateEditCalendarState.Years)
				return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}", new DateTime(Math.Max((DateTime.Year / 10) * 10, 1), 1, 1), new DateTime((DateTime.Year / 10) * 10 + 9, 1, 1));
			return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}", new DateTime(Math.Max((DateTime.Year / 100) * 100, 1), 1, 1), new DateTime((DateTime.Year / 100) * 100 + 99, 1, 1));
		}
		protected virtual string GetMonthName(int month) {
			return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1];
		}
		protected virtual void OnDateTimeChanged() {
			if (Calendar != null)
				Calendar.DateTime = DateTime;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (Calendar != null)
				Calendar.OnApplyContentTemplate(this);
			FillContent();
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Size sz = base.ArrangeOverride(arrangeBounds);
			ApplyDelayedAnimations();
			return sz;
		}
		internal void SetDelayedZoomOut(TransferContentControl control) {
			currTransferContent = control;
			this.makeCurrZoomOut = true;
		}
		internal void SetDelayedPrevZoomOut(TransferContentControl control) {
			prevTransferContent = control;
			this.makePrevZoomOut = true;
		}
		internal void SetDelayedZoomIn(TransferContentControl control) {
			currTransferContent = control;
			this.makeCurrZoomIn = true;
		}
		internal void SetDelayedNone(TransferContentControl control) {
			prevTransferContent = control;
#if !SL
			prevTransferContent.Visibility = Visibility.Hidden;
#else
			prevTransferContent.Opacity = 0;
#endif
		}
		internal void SetDelayedPrevZoomIn(TransferContentControl control) {
			prevTransferContent = control;
			this.makePrevZoomIn = true;
		}
		protected virtual void ApplyDelayedAnimations() {
			if (this.makeCurrZoomOut) {
				Calendar.CalendarTransfer.ZoomOutAnimation(currTransferContent);
			}
			if (this.makePrevZoomOut) {
				Calendar.CalendarTransfer.PrevZoomOutAnimation(prevTransferContent);
			}
			if (this.makeCurrZoomIn) {
				Calendar.CalendarTransfer.ZoomInAnimation(currTransferContent);
			}
			if (this.makePrevZoomIn) {
				Calendar.CalendarTransfer.PrevZoomInAnimation(prevTransferContent);
			}
			this.makeCurrZoomOut = false;
			this.makePrevZoomOut = false;
			this.makeCurrZoomIn = false;
			this.makePrevZoomIn = false;
		}
		protected virtual void CalcFirstVisibleDate() {
			SetFirstVisibleDate(GetFirstVisibleDate(DateTime));
		}
		public virtual void SetFirstVisibleDate(DateTime firstVisibleDate) {
			FirstCellIndex = 0;
			FirstVisibleDate = firstVisibleDate;
			if (FirstVisibleDate == DateTime.MinValue)
				FirstCellIndex = GetFirstDayOffset(FirstVisibleDate) - (FirstVisibleDate - FirstVisibleDate).Days;
		}
		protected DateTime GetFirstVisibleDate(DateTime value) {
			DateTime firstDay = GetFirstMonthDate(value);
			TimeSpan delta = TimeSpan.FromDays(-GetFirstDayOffset(firstDay));
			if (firstDay.Ticks + delta.Ticks < 0)
				return DateTime.MinValue;
			else {
				try {
					return firstDay + delta;
				} catch (ArgumentOutOfRangeException) {
					return Calendar.GetMinValue();
				}
			}
		}
		protected virtual void FillMonthInfo() {
			ContentGrid.BeginInit();
			CalcDayNumberCells();
			ContentGrid.EndInit();
		}
		protected virtual void CreateMonthCellInfo(int row, int col) {
			DateTime dt = new DateTime(DateTime.Year, 1 + row * 4 + col, 1);
			AddCellInfo(row, col, dt, GetMonthName(dt.Month), dt > Calendar.GetMaxValue() || dt < Calendar.GetMinValue() && dt.Month < Calendar.GetMinValue().Month);
		}
		protected virtual void CreateYearCellInfo(int row, int col) {
			int beginYear = (DateTime.Year / 10) * 10 - 1;
			int currYear = beginYear + row * 4 + col;
			if (currYear <= 0 || currYear >= 10000)
				return;
			DateTime dt = new DateTime(currYear, 1, 1);
			AddCellInfo(row, col, dt, dt.ToString("yyyy", CultureInfo.CurrentCulture), (dt < Calendar.GetMinValue() && dt.Year < Calendar.GetMinValue().Year) || dt > Calendar.GetMaxValue());
		}
		protected virtual void CreateYearsGroupCellInfo(int row, int col) {
			int beginYearGroup = (DateTime.Year / 100) * 100 - 10;
			int currYearGroup = beginYearGroup + (row * 4 + col) * 10;
			if (currYearGroup < 0 || currYearGroup >= 10000)
				return;
			int endYearGroup = currYearGroup + 9;
			if (currYearGroup == 0)
				currYearGroup = 1;
			DateTime dt = new DateTime(currYearGroup, 1, 1);
			AddCellInfo(row, col, dt, new DateTime(currYearGroup, 1, 1).ToString("yyyy", CultureInfo.CurrentCulture) + "-\n" + new DateTime(endYearGroup, 1, 1).ToString("yyyy", CultureInfo.CurrentCulture), (dt < Calendar.GetMinValue() && endYearGroup < Calendar.GetMinValue().Year) || dt > Calendar.GetMaxValue());
		}
		protected delegate void CreateCellInfo(int row, int col);
		protected virtual void CreateInfoCells(CreateCellInfo method) {
			for (int row = 0; row < 3; row++) {
				for (int col = 0; col < 4; col++) {
					method(row, col);
				}
			}
		}
		protected virtual void CalcYearInfoCells() {
			CreateInfoCells(CreateMonthCellInfo);
		}
		protected virtual void CalcYearsInfoCells() {
			CreateInfoCells(CreateYearCellInfo);
		}
		protected virtual void CalcYearsGroupInfoCells() {
			CreateInfoCells(CreateYearsGroupCellInfo);
		}
		protected virtual void FillYearInfo() {
			ContentGrid.BeginInit();
			CalcYearInfoCells();
			ContentGrid.EndInit();
		}
		protected virtual void FillYearsInfo() {
			ContentGrid.BeginInit();
			CalcYearsInfoCells();
			ContentGrid.EndInit();
		}
		protected virtual void FillYearsGroupInfo() {
			ContentGrid.BeginInit();
			CalcYearsGroupInfoCells();
			ContentGrid.EndInit();
		}
		protected virtual bool CanAddDate(DateTime date) {
			if (date < Calendar.MinValue || date > Calendar.MaxValue)
				return false;
			return true;
		}
		protected virtual void CalcDayNumberCells() {
			int row, col;
			DateTime current = Calendar.GetMinValue();
			for (row = 0; row < 6; row++) {
				int firstCell = row == 0 ? FirstCellIndex : 0;
				for (col = 0; col < 7; col++) {
					if (row == 0 && col < firstCell) {
						AddCellInfo(row, col);
						continue;
					}
					try {
						current = FirstVisibleDate.AddDays(row * 7 + col - FirstCellIndex);
					} catch {
						row = 6;
						break;
					}
					AddCellInfo(row, col, current, current.Day.ToString(CultureInfo.CurrentCulture), !CanAddDate(current));
				}
				if (Calendar.ShowWeekNumbers && current != Calendar.MinValue)
					AddWeekNumber(row, current);
			}
		}
		protected virtual int CellFirstRow {
			get {
				if (State == DateEditCalendarState.Month)
					return 2;
				return 0;
			}
		}
		protected virtual int CellFirstCol {
			get {
				if (State == DateEditCalendarState.Month)
					return 2;
				return 0;
			}
		}
		protected virtual void AddWeekNumber(int row, DateTime current) {
			TextBlock block = new TextBlock();
			block.Text = GetWeekNumber(current).ToString(CultureInfo.CurrentCulture);
			block.Style = Calendar.WeekNumbersStyle;
			Grid.SetRow(block, CellFirstRow + row);
			Grid.SetColumn(block, 0);
			ContentGrid.Children.Add(block);
		}
		protected virtual void UpdateMonthInfoCell(DependencyObject obj, DateTime current) {
			if (Calendar == null) return;
			if (current.Month != DateTime.Month)
				DateEditCalendar.SetCellInactive(obj, true);
			if(Calendar.CalendarTransfer.HasExecutingAnimations) return;
			if (current.Month == DateTime.Now.Month && current.Year == DateTime.Now.Year && current.Day == DateTime.Now.Day)
				DateEditCalendar.SetCellToday(obj, true);
			if (current.Month == DateTime.Month && current.Year == DateTime.Year && current.Day == DateTime.Day)
				DateEditCalendar.SetCellFocused(obj, true);
		}
		protected virtual void UpdateYearInfoCell(DependencyObject obj, DateTime current) {
			if (current.Month == DateTime.Month && current.Year == DateTime.Year)
				DateEditCalendar.SetCellFocused(obj, true);
		}
		protected virtual void UpdateYearsInfoCell(DependencyObject obj, DateTime current) {
			if (current.Year / 10 != DateTime.Year / 10)
				DateEditCalendar.SetCellInactive(obj, true);
			if (current.Year == DateTime.Year)
				DateEditCalendar.SetCellFocused(obj, true);
		}
		protected virtual void UpdateYearsGroupInfoCell(DependencyObject obj, DateTime current) {
			if (current.Year / 100 != DateTime.Year / 100)
				DateEditCalendar.SetCellInactive(obj, true);
			if (current.Year / 10 == DateTime.Year / 10)
				DateEditCalendar.SetCellFocused(obj, true);
		}
		protected virtual void UpdateCellInfo(DependencyObject obj, DateTime current) {
			switch (State) {
				case DateEditCalendarState.Month:
					UpdateMonthInfoCell(obj, current);
					break;
				case DateEditCalendarState.Year:
					UpdateYearInfoCell(obj, current);
					break;
				case DateEditCalendarState.Years:
					UpdateYearsInfoCell(obj, current);
					break;
				case DateEditCalendarState.YearsGroup:
					UpdateYearsGroupInfoCell(obj, current);
					break;
			}
		}
		protected internal void UpdateCellInfos() {
			if(ContentGrid != null)
				foreach(DependencyObject o in ContentGrid.Children)
					UpdateCellInfo(o, (DateTime)DateEditCalendar.GetDateTime(o));
		}
		protected virtual void AddCellInfo(int row, int col) {
			AddCellInfo(row, col, DateTime.MinValue, string.Empty, false);
		}
		protected virtual void AddCellInfo(int row, int col, DateTime current, string content, bool isHidden) {
			current = new DateTime(current.Ticks, DateTimeKind.Local);
			CalendarCellButton button = new CalendarCellButton { Content = content, Focusable = false };
			if(string.IsNullOrEmpty(content))
				UIElementHelper.Collapse(button);
			DateEditCalendar.SetDateTime(button, current);
			button.Click += OnCellButtonClick;
			Grid.SetRow(button, CellFirstRow + row);
			Grid.SetColumn(button, CellFirstCol + col);
			if (isHidden) {
				button.Opacity = 0;
				button.IsEnabled = false;
			}
			ContentGrid.Children.Add(button);
			UpdateCellInfo(button, current);
		}
		protected virtual void UpdateClickedButton(Button button) {
			if (button == null)
				return;
			DateEditCalendar.SetCellFocused(button, true);
		}
		protected virtual void OnCellButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (Calendar == null || Calendar.ActiveContent != this)
				return;
			ClearSelection();
			UpdateClickedButton(sender as Button);
			Calendar.OnCellButtonClick(sender, e);
		}
		protected virtual void FillContentInfo() {
			CalcFirstVisibleDate();
			switch (State) {
				case DateEditCalendarState.Month:
					FillMonthInfo();
					break;
				case DateEditCalendarState.Year:
					FillYearInfo();
					break;
				case DateEditCalendarState.Years:
					FillYearsInfo();
					break;
				case DateEditCalendarState.YearsGroup:
					FillYearsGroupInfo();
					break;
			}
		}
		protected DateTime GetFirstMonthDate(DateTime value) {
			return new DateTime(value.Year, value.Month, 1, value.Hour, value.Minute, value.Second, value.Millisecond);
		}
		protected int GetFirstDayOffset(DateTime firstMonthDate) {
			return (FirstDayOfWeek == firstMonthDate.DayOfWeek ? 7 : (7 + firstMonthDate.DayOfWeek - FirstDayOfWeek) % 7);
		}
		protected virtual void FillWeekDaysAbbreviation() {
			if (State != DateEditCalendarState.Month)
				return;
			int gridCol = 2;
			for (int i = 0; i < DateTimeFormat.ShortestDayNames.Length; i++) {
				string abbr = DateTimeFormat.ShortestDayNames[(Convert.ToInt32(DateTimeFormat.FirstDayOfWeek) + i) % 7];
				TextBlock block = new TextBlock();
				block.Text = abbr;
				Grid.SetRow(block, 0);
				Grid.SetColumn(block, gridCol);
				block.Style = Calendar.WeekdayAbbreviationStyle;
				ContentGrid.Children.Add(block);
				gridCol++;
			}
		}
		protected virtual int GetWeekNumber(DateTime date) {
			return DateTimeFormat.Calendar.GetWeekOfYear(date, GetWeekNumberRule(), FirstDayOfWeek);
		}
		protected virtual CalendarWeekRule GetWeekNumberRule() {
			return DateTimeFormat.CalendarWeekRule;
		}
		protected virtual void FillContent() {
			if (ContentGrid == null)
				return;
			FillWeekDaysAbbreviation();
			FillContentInfo();
		}
		protected internal object GetTemplateChildCore(string name) { return GetTemplateChild(name); }
		Grid contentGrid;
		public Grid ContentGrid {
			get {
				if (contentGrid == null)
					contentGrid = GetTemplateChild("PART_ContentGrid") as Grid;
				return contentGrid;
			}
		}
		public FrameworkElement GetFocusedCell() {
			if (ContentGrid != null) {
				foreach (UIElement elem in ContentGrid.Children) {
					if (DateEditCalendar.GetCellFocused(elem))
						return elem as FrameworkElement;
				}
			}
			return null;
		}
		protected virtual void ClearSelection() {
			if (ContentGrid == null)
				return;
			foreach (UIElement elem in ContentGrid.Children) {
				DateEditCalendar.SetCellFocused(elem, false);
				DateEditCalendar.SetCellToday(elem, false);
			}
		}
	}
	[ToolboxItem(false)]
	public partial class DateEditCalendarTransferControl : TransferControl {
		#region static
		public static readonly DependencyProperty TransferTypeProperty;
		public static readonly DependencyProperty AnimationTimeProperty;
		static DateEditCalendarTransferControl() {
			Type ownerType = typeof(DateEditCalendarTransferControl);
			TransferTypeProperty = DependencyPropertyManager.Register("TransferType", typeof(DateEditCalendarTransferType), typeof(DateEditCalendarTransferControl), new FrameworkPropertyMetadata(DateEditCalendarTransferType.None, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnTransferTypeChanged)));
			AnimationTimeProperty = DependencyPropertyManager.Register("AnimationTime", typeof(double), typeof(DateEditCalendarTransferControl), new FrameworkPropertyMetadata(300.0, FrameworkPropertyMetadataOptions.None));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
#endif
		}
		protected static void OnTransferTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEditCalendarTransferControl)obj).OnTransferTypeChanged();
		}
		#endregion
		List<Storyboard> executingAnimations = new List<Storyboard>();
		Storyboard lastExecutingAnimation;
		public double AnimationTime {
			get { return (double)GetValue(AnimationTimeProperty); }
			set { SetValue(AnimationTimeProperty, value); }
		}
		public DateEditCalendarTransferType TransferType {
			get { return (DateEditCalendarTransferType)GetValue(TransferTypeProperty); }
			set { SetValue(TransferTypeProperty, value); }
		}
		public DateEdit OwnerDateEdit { get { return (DateEdit)BaseEdit.GetOwnerEdit(this); } }
		public DateEditCalendar Calendar { get { return DateEditCalendar.GetCalendar(this); } }
		protected internal bool HasExecutingAnimations { get { return lastExecutingAnimation != null; } }
		protected override bool SkipLongAnimations { get { return false; } }
		protected virtual void OnTransferTypeChanged() {
		}
#if !SL
		protected internal override void OnPrevContentChanged(TransferContentControl control) {
#else
		protected override void OnPrevContentChanged(TransferContentControl control) {
#endif
			switch (TransferType) {
				case DateEditCalendarTransferType.ShiftRight:
					PrevLeftToRightAnimation(control);
					break;
				case DateEditCalendarTransferType.ShiftLeft:
					PrevRightToLeftAnimation(control);
					break;
				case DateEditCalendarTransferType.ZoomOut:
					DelayedPrevZoomOutAnimation(control);
					break;
				case DateEditCalendarTransferType.ZoomIn:
					DelayedPrevZoomInAnimation(control);
					break;
				case DateEditCalendarTransferType.None:
					DelayedNoneAnimation(control);
					break;
			}
		}
#if !SL
		protected internal override void OnCurrentContentChanged(TransferContentControl control) {
#else
		protected override void OnCurrentContentChanged(TransferContentControl control) {
#endif
			FinishAllExecutingAnimations();
			if (Calendar == null)
				return;
			switch (TransferType) {
				case DateEditCalendarTransferType.ShiftRight:
					LeftToRightAnimation(control);
					break;
				case DateEditCalendarTransferType.ShiftLeft:
					RightToLeftAnimation(control);
					break;
				case DateEditCalendarTransferType.ZoomOut:
					DelayedZoomOutAnimation(control);
					break;
				case DateEditCalendarTransferType.ZoomIn:
					DelayedZoomInAnimation(control);
					break;
			}
		}
		protected virtual DateEditCalendarContent GetCalendarContent(TransferContentControl control) {
			return control.Content as DateEditCalendarContent;
		}
		protected virtual void MakeTranslateXAnimaion(TransferContentControl control, double from, double to) {
			DateEditCalendarContent content = GetCalendarContent(control);
			if (content == null)
				return;
			DoubleAnimation da = CreateDoubleAnimationCore(from, to);
			content.RenderTransform = new TranslateTransform();
			BeginAnimation(content, da, "(UIElement.RenderTransform).(TranslateTransform.X)");
		}
		DoubleAnimation CreateDoubleAnimationCore(double from, double to) {
			DoubleAnimation da = new DoubleAnimation();
			da.From = from;
			da.To = to;
			da.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime));
			da.FillBehavior = FillBehavior.HoldEnd;
			return da;
		}
		protected internal virtual void LeftToRightAnimation(TransferContentControl control) {
			MakeTranslateXAnimaion(control, -control.ContentPresenter.ActualWidth, 0);
		}
		protected internal virtual void PrevLeftToRightAnimation(TransferContentControl control) {
			MakeTranslateXAnimaion(control, 0, control.ContentPresenter.ActualWidth);
		}
		protected internal virtual void PrevRightToLeftAnimation(TransferContentControl control) {
			MakeTranslateXAnimaion(control, 0, -control.ContentPresenter.ActualWidth);
		}
		protected internal virtual void RightToLeftAnimation(TransferContentControl control) {
			MakeTranslateXAnimaion(control, control.ContentPresenter.ActualWidth, 0);
		}
		protected virtual Rect GetFocusedCellRect(DateEditCalendarContent content) {
			FrameworkElement obj = content.GetFocusedCell();
			if (obj == null)
				return Rect.Empty;
			return LayoutHelper.GetRelativeElementRect(obj, content);
		}
#if !SL
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			if (oldContent != null && oldContent is FrameworkElement)
				((FrameworkElement)oldContent).IsHitTestVisible = false;
			if (newContent != null && newContent is FrameworkElement)
				((FrameworkElement)newContent).IsHitTestVisible = true;
		}
#endif
		protected virtual DoubleAnimation CreateDoubleAnimation(double from, double to) {
			if (double.IsInfinity(from))
				from = 0.0;
			return CreateDoubleAnimationCore(from, to);
		}
		protected virtual double GetScaleMinValue(Rect r, Size sz) {
			return Math.Min(r.Width / sz.Width, r.Height / sz.Height);
		}
		protected virtual void MakeZoomInOutAnimation(TransferContentControl control, double scaleFrom, double scaleTo, double opacityFrom, double opacityTo, bool zoomIn) {
			DateEditCalendarContent content = GetCalendarContent(control);
			if (content == null)
				return;
			Rect r = GetFocusedCellRect(zoomIn ? Calendar.PrevContent : Calendar.ActiveContent);
			if (double.IsNaN(scaleTo))
				scaleTo = GetScaleMinValue(r, content.DesiredSize);
			if (double.IsNaN(scaleFrom))
				scaleFrom = GetScaleMinValue(r, content.DesiredSize);
			DoubleAnimation width = CreateDoubleAnimation(scaleFrom, scaleTo);
			DoubleAnimation height = CreateDoubleAnimation(scaleFrom, scaleTo);
			DoubleAnimation opacity = CreateDoubleAnimation(opacityFrom, opacityTo);
			ScaleTransform scale = new ScaleTransform();
			content.RenderTransform = scale;
			scale.CenterX = r.X + r.Width / 2;
			scale.CenterY = r.Y + r.Height / 2;
			BeginAnimation(content, width, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
			BeginAnimation(content, height, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
			BeginAnimation(content, opacity, "(UIElement.Opacity)");
		}
		void BeginAnimation(DateEditCalendarContent value, DoubleAnimation element, string strPropertyPath) {
			Storyboard.SetTarget(element, value);
			Storyboard.SetTargetProperty(element, new PropertyPath(strPropertyPath));
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(element);
			executingAnimations.Add(storyboard);
			storyboard.Completed += delegate {
				executingAnimations.Remove(storyboard);
				if(storyboard == lastExecutingAnimation) {
					lastExecutingAnimation = null;
					((DateEditCalendarContent)Content).UpdateCellInfos();
				}
			};
			lastExecutingAnimation = storyboard;
			storyboard.Begin();
		}
		protected internal virtual void PrevZoomOutAnimation(TransferContentControl control) {
			MakeZoomInOutAnimation(control, 1, double.NaN, 1, 0, false);
		}
		protected internal virtual void ZoomOutAnimation(TransferContentControl control) {
			MakeZoomInOutAnimation(control, 5, 1, 0, 1, false);
		}
		protected internal void DelayedPrevZoomOutAnimation(TransferContentControl control) {
			Calendar.ActiveContent.SetDelayedPrevZoomOut(control);
		}
		protected internal void DelayedZoomOutAnimation(TransferContentControl control) {
			Calendar.ActiveContent.SetDelayedZoomOut(control);
		}
		protected internal virtual void PrevZoomInAnimation(TransferContentControl control) {
			MakeZoomInOutAnimation(control, 1, 5, 1, 0, true);
		}
		protected internal virtual void ZoomInAnimation(TransferContentControl control) {
			MakeZoomInOutAnimation(control, double.NaN, 1, 0, 1, true);
		}
		protected internal void DelayedPrevZoomInAnimation(TransferContentControl control) {
			Calendar.ActiveContent.SetDelayedPrevZoomIn(control);
		}
		protected internal void DelayedZoomInAnimation(TransferContentControl control) {
			Calendar.ActiveContent.SetDelayedZoomIn(control);
		}
		protected internal void DelayedNoneAnimation(TransferContentControl control) {
			if(Calendar != null && Calendar.ActiveContent != null && control != null)
				Calendar.ActiveContent.SetDelayedNone(control);
		}
		void FinishAllExecutingAnimations() {
			lastExecutingAnimation = null;
			foreach(Storyboard s in executingAnimations)
				s.SkipToFill();
			executingAnimations.Clear();
		}
	}
}
