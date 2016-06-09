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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.DateNavigator.Internal;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Validation.Native;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors.DateNavigator;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TransferContentControl = DevExpress.Xpf.Core.TransitionContentControl;
using TransferControl = DevExpress.Xpf.Core.TransitionControl;
#endif
namespace DevExpress.Xpf.Editors.DateNavigator.Controls {
	[Flags]
	public enum DateNavigatorCalendarCellState {
		Focused = 1,
		Holiday = 2,
		Inactive = 4,
		Selected = 8,
		Special = 16,
		Today = 32
	}
	public enum DateNavigatorCalendarButtonKind {
		Date,
		None,
		WeekNumber
	}
	public partial class DateNavigatorCalendarCellButton : Button
#if SL
		, IDependencyPropertyChangeListener
#endif
	{
		public static readonly DependencyProperty CalendarViewProperty;
		static DateNavigatorCalendarCellButton() {
			Type ownerType = typeof(DateNavigatorCalendarCellButton);
			CalendarViewProperty = DependencyPropertyManager.Register("CalendarView", typeof(DateNavigatorCalendarView), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButton)d).PropertyChangedCalendarView((DateNavigatorCalendarView)e.OldValue)));
		}
		public DateNavigatorCalendarCellButton() {
			this.SetDefaultStyleKey(typeof(DateNavigatorCalendarCellButton));
		}
		public DateNavigatorCalendarView CalendarView {
			get { return (DateNavigatorCalendarView)GetValue(CalendarViewProperty); }
			set { SetValue(CalendarViewProperty, value); }
		}
		protected DateNavigatorCalendarCellButtonContent ElementContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
			ElementContent = GetTemplateChild("PART_Content") as DateNavigatorCalendarCellButtonContent;
			UpdateElementContentState();
			UpdateElementContentText();
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			UpdateElementContentText();
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
#else
		protected  void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
#endif
			if (e.Property == DateNavigatorCalendar.CellStateProperty) {
				UpdateVisualState(true);
				UpdateElementContentState();
			}
		}
		protected virtual void PropertyChangedCalendarView(DateNavigatorCalendarView oldValue) { }
		protected virtual void UpdateVisualState(bool useTransitions) {
			DateNavigatorCalendarCellState state = DateNavigatorCalendar.GetCellState(this);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Special) ? "CellStateSpecial" : "CellStateNotSpecial", true);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Holiday) ? "CellStateHoliday" : "CellStateNotHoliday", true);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Selected) ? "CellStateSelected" : "CellStateNotSelected", true);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Today) ? "CellStateToday" : "CellStateNotToday", true);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Focused) ? "CellStateFocused" : "CellStateNotFocused", true);
			VisualStateManager.GoToState(this, state.HasFlag(DateNavigatorCalendarCellState.Inactive) ? "CellStateInactive" : "CellStateActive", true);
		}
		void UpdateElementContentState() {
			if (ElementContent != null)
				DateNavigatorCalendar.SetCellState(ElementContent, DateNavigatorCalendar.GetCellState(this));
		}
		void UpdateElementContentText() {
			if (ElementContent != null)
				ElementContent.Text = Content != null ? Content.ToString() : null;
		}
#if SL
		internal void ResetIsMouseOver() {
			if (ElementContent != null)
				ElementContent.ResetIsMouseOver();
		}
		#region IDependencyPropertyChangeListener Members
		void IDependencyPropertyChangeListener.OnPropertyAssigning(DependencyProperty dp, object value) { }
		void IDependencyPropertyChangeListener.OnPropertyChanged(SLDependencyPropertyChangedEventArgs e) {
			OnPropertyChanged(e);
		}
		#endregion
#endif
	}
	public interface IDateNavigatorCalendarOwner {
		int CalendarCount { get; }
		int GetCalendarIndex(DateNavigatorCalendar calendar);
	}
	public enum DateNavigatorCalendarPosition { Single, First, Intermediate, Last }
	public class DateNavigatorCalendarButtonClickEventArgs : EventArgs {
		public DateNavigatorCalendarButtonClickEventArgs(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			ButtonDate = buttonDate;
			ButtonKind = buttonKind;
		}
		public DateTime ButtonDate { get; private set; }
		public DateNavigatorCalendarButtonKind ButtonKind { get; private set; }
	}
	public delegate void DateNavigatorCalendarButtonClickEventHandler(object sender, DateNavigatorCalendarButtonClickEventArgs e);
	public partial class DateNavigatorCalendar : ContentControl {
		#region static
		protected static readonly DependencyPropertyKey DateTimeTextPropertyKey;
		public static readonly DependencyProperty DateTimeTextProperty;
		protected static readonly DependencyPropertyKey CurrentDateTextPropertyKey;
		public static readonly DependencyProperty CurrentDateTextProperty;
		public static readonly DependencyProperty WeekdayAbbreviationStyleProperty;
		public static readonly DependencyProperty WeekNumberStyleProperty;
		public static readonly DependencyProperty CellButtonStyleProperty;
		public static readonly DependencyProperty MonthInfoTemplateProperty;
		public static readonly DependencyProperty YearInfoTemplateProperty;
		public static readonly DependencyProperty YearsInfoTemplateProperty;
		public static readonly DependencyProperty YearsRangeInfoTemplateProperty;
		public static readonly DependencyProperty CalendarTransferStyleProperty;
		public static readonly DependencyProperty DateTimeProperty;
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty CalendarProperty;
		public static readonly DependencyProperty ShowWeekNumbersProperty;
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty ResizeToMaxContentProperty;
		public static readonly DependencyProperty CellStateProperty;
		public static readonly DependencyProperty WeekNumberRuleProperty;
		public static readonly DependencyProperty HighlightSpecialDatesProperty;
		public static readonly DependencyProperty HighlightHolidaysProperty;
		public static readonly DependencyProperty FirstDayOfWeekProperty;
		public static readonly DependencyProperty WeekFirstDateProperty;
		static DateNavigatorCalendar() {
			Type ownerType = typeof(DateNavigatorCalendar);
			DateTimeProperty = DependencyPropertyManager.RegisterAttached("DateTime", typeof(DateTime), ownerType, new FrameworkPropertyMetadata(DateTime.MinValue, OnDateTimePropertyChanged));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DateNavigatorCalendar), new FrameworkPropertyMetadata(ownerType));
#endif
			CurrentDateTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("CurrentDateText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
			CurrentDateTextProperty = CurrentDateTextPropertyKey.DependencyProperty;
			DateTimeTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DateTimeText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));
			DateTimeTextProperty = DateTimeTextPropertyKey.DependencyProperty;
			WeekdayAbbreviationStyleProperty = DependencyPropertyManager.Register("WeekdayAbbreviationStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CellButtonStyleProperty = DependencyPropertyManager.Register("CellButtonStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			WeekNumberStyleProperty = DependencyPropertyManager.Register("WeekNumberStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MonthInfoTemplateProperty = DependencyPropertyManager.Register("MonthInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearInfoTemplateProperty = DependencyPropertyManager.Register("YearInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearsInfoTemplateProperty = DependencyPropertyManager.Register("YearsInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			YearsRangeInfoTemplateProperty = DependencyPropertyManager.Register("YearsRangeInfoTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CalendarTransferStyleProperty = DependencyPropertyManager.Register("CalendarTransferStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			CalendarProperty = DependencyPropertyManager.RegisterAttached("Calendar", typeof(DateNavigatorCalendar), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			ShowWeekNumbersProperty = DependencyPropertyManager.RegisterAttached("ShowWeekNumbers", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnShowWeekNumbersChanged));
			StateProperty = DependencyPropertyManager.Register("State", typeof(DateNavigatorCalendarView), ownerType, new FrameworkPropertyMetadata(OnStatePropertyChanged));
			ResizeToMaxContentProperty = DependencyPropertyManager.Register("ResizeToMaxContent", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnResizeToMaxContentPropertyChanged));
			CellStateProperty = DependencyPropertyManager.RegisterAttached("CellState", typeof(DateNavigatorCalendarCellState), ownerType, new PropertyMetadata(null));
			WeekNumberRuleProperty = DependencyPropertyManager.Register("WeekNumberRule", typeof(CalendarWeekRule?), typeof(DateNavigatorCalendar),
			  new PropertyMetadata(null, (d, e) => ((DateNavigatorCalendar)d).PropertyChangedWeekNumberRule((CalendarWeekRule?)e.OldValue)));
			HighlightSpecialDatesProperty = DependencyPropertyManager.Register("HighlightSpecialDates", typeof(bool), ownerType,
			  new PropertyMetadata(true, (d, e) => ((DateNavigatorCalendar)d).PropertyChangedHighlightSpecialDates((bool)e.OldValue)));
			HighlightHolidaysProperty = DependencyPropertyManager.Register("HighlightHolidays", typeof(bool), ownerType,
			  new PropertyMetadata(true, (d, e) => ((DateNavigatorCalendar)d).PropertyChangedHighlightHolidays((bool)e.OldValue)));
			FirstDayOfWeekProperty = DependencyPropertyManager.Register("FirstDayOfWeek", typeof(DayOfWeek?), ownerType,
			  new PropertyMetadata(null, (d, e) => ((DateNavigatorCalendar)d).PropertyChangedFirstDayOfWeek((DayOfWeek?)e.OldValue)));
			WeekFirstDateProperty = DependencyPropertyManager.RegisterAttached("WeekFirstDate", typeof(DateTime), ownerType, new PropertyMetadata(DateTime.MinValue));
		}
		public static DateNavigatorCalendar GetCalendar(DependencyObject obj) {
			return (DateNavigatorCalendar)DependencyObjectHelper.GetValueWithInheritance(obj, CalendarProperty);
		}
		private static void SetCalendar(DependencyObject obj, DateNavigatorCalendar value) { obj.SetValue(CalendarProperty, value); }
		public static DateTime GetDateTime(DependencyObject obj) { return (DateTime)obj.GetValue(DateTimeProperty); }
		public static void SetDateTime(DependencyObject obj, object value) { obj.SetValue(DateTimeProperty, value); }
		public static DateTime GetWeekFirstDate(DependencyObject obj) { return (DateTime)obj.GetValue(WeekFirstDateProperty); }
		public static void SetWeekFirstDate(DependencyObject obj, object value) { obj.SetValue(WeekFirstDateProperty, value); }
		public static DateNavigatorCalendarCellState GetCellState(DependencyObject obj) { return (DateNavigatorCalendarCellState)obj.GetValue(CellStateProperty); }
		public static void SetCellState(DependencyObject obj, DateNavigatorCalendarCellState value) { obj.SetValue(CellStateProperty, value); }
		protected internal static void SetCellFocused(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Focused, value);
		}
		protected internal static void SetCellHoliday(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Holiday, value);
		}
		protected internal static void SetCellInactive(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Inactive, value);
		}
		protected internal static void SetCellSelected(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Selected, value);
		}
		protected internal static void SetCellSpecial(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Special, value);
		}
		protected internal static void SetCellToday(DependencyObject obj, bool value) {
			SetCellStateFlag(obj, DateNavigatorCalendarCellState.Today, value);
		}
		protected internal static void SetCellStateFlag(DependencyObject obj, DateNavigatorCalendarCellState flag, bool value) {
			DateNavigatorCalendarCellState state = GetCellState(obj);
			if (value)
				state |= flag;
			else
				state &= ~flag;
			obj.SetValue(CellStateProperty, state);
		}
#if SL
		public ContentControl WeekNumberDelimeter { get { return FindName("WeekNumberDelimeter") as ContentControl; } }
#endif
		static void OnDateTimePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is DateNavigatorCalendar)
				((DateNavigatorCalendar)obj).OnDateTimeChanged();
		}
		static void OnStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateNavigatorCalendar)obj).OnStateChanged();
		}
		static void OnResizeToMaxContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateNavigatorCalendar)obj).OnResizeToMaxContentChanged();
		}
		static void OnShowWeekNumbersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is DateNavigatorCalendar)
				((DateNavigatorCalendar)obj).OnShowWeekNumbersChanged();
		}
		#endregion
		WeakReference ownerReference;
		DateNavigatorCalendarPosition position;
		bool wasLayoutUpdated;
		public event DateNavigatorCalendarButtonClickEventHandler ButtonClick;
		void UpdateButtonVisibility(Button button, bool show) {
			if (button != null)
				button.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
		}
		public DateNavigatorCalendar(IDateNavigatorCalendarOwner owner) {
			ownerReference = new WeakReference(owner);
			UpdateDateTimeText();
			SetCalendar(this, this);
#if SL
			DefaultStyleKey = typeof(DateNavigatorCalendar);
#endif
			LayoutUpdated += OnLayoutUpdated;
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
			wasLayoutUpdated = true;
			if (!ResizeToMaxContent)
				UpdateContent();
		}
		public void SetNewDateTime(DateTime dateTime) {
			ActiveContent.DateTime = dateTime;
			SetNewContent(dateTime, DateNavigatorCalendarTransferType.None);
		}
		public bool ShowWeekNumbers {
			get { return (bool)GetValue(ShowWeekNumbersProperty); }
			set {
				SetValue(ShowWeekNumbersProperty, value);
#if SL
				if (WeekNumberDelimeter != null)
					WeekNumberDelimeter.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
#endif
			}
		}
		public DateNavigatorCalendarView State {
			get { return (DateNavigatorCalendarView)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
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
		public ControlTemplate YearsRangeInfoTemplate {
			get { return (ControlTemplate)GetValue(YearsRangeInfoTemplateProperty); }
			set { SetValue(YearsRangeInfoTemplateProperty, value); }
		}
		public Style CalendarTransferStyle {
			get { return (Style)GetValue(CalendarTransferStyleProperty); }
			set { SetValue(CalendarTransferStyleProperty, value); }
		}
		public Style WeekNumberStyle {
			get { return (Style)GetValue(WeekNumberStyleProperty); }
			set { SetValue(WeekNumberStyleProperty, value); }
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
		public bool ResizeToMaxContent {
			get { return (bool)GetValue(ResizeToMaxContentProperty); }
			set { SetValue(ResizeToMaxContentProperty, value); }
		}
		protected IDateNavigatorCalendarOwner Owner {
			get { return ownerReference.IsAlive ? (IDateNavigatorCalendarOwner)ownerReference.Target : null; }
		}
		public DateNavigatorCalendarPosition Position {
			get { return position; }
			set {
				if (position != value) {
					position = value;
					OnPositionChanged();
				}
			}
		}
		public CalendarWeekRule? WeekNumberRule {
			get { return (CalendarWeekRule?)GetValue(WeekNumberRuleProperty); }
			set { SetValue(WeekNumberRuleProperty, value); }
		}
		public bool HighlightSpecialDates {
			get { return (bool)GetValue(HighlightSpecialDatesProperty); }
			set { SetValue(HighlightSpecialDatesProperty, value); }
		}
		public bool HighlightHolidays {
			get { return (bool)GetValue(HighlightHolidaysProperty); }
			set { SetValue(HighlightHolidaysProperty, value); }
		}
		public DayOfWeek? FirstDayOfWeek {
			get { return (DayOfWeek?)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		protected internal DateNavigatorCalendarContent ActiveContent { get { return null; } }
		protected internal new DateNavigatorCalendarContent Content { get; private set; }
		public DateEdit OwnerDateEdit { get { return (DateEdit)BaseEdit.GetOwnerEdit(this); } }
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MinValue {
			get { return (DateTime?)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MaxValue {
			get { return (DateTime?)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime DateTime {
			get { return (DateTime)GetValue(DateTimeProperty); }
			set { SetValue(DateTimeProperty, value); }
		}
		public DateNavigatorCalendarCellButton GetCellButton(DateTime dt) {
			return Content.GetCellButton(dt);
		}
		public void GetDateRange(bool excludeInactiveContent, out DateTime firstDate, out DateTime lastDate) {
			if (Content.ReadLocalValue(Control.TemplateProperty) == DependencyProperty.UnsetValue) {
				wasLayoutUpdated = true;
				UpdateContent();
			}
			Content.GetDateRange(excludeInactiveContent, out firstDate, out lastDate);
		}
		protected internal virtual void HitTest(UIElement element, out DateTime? buttonDate, out DateNavigatorCalendarButtonKind buttonKind) {
			DateNavigatorCalendarCellButton button = LayoutHelper.FindParentObject<DateNavigatorCalendarCellButton>(element);
			if (button != null && LayoutHelper.IsChildElement(this, button)) {
				buttonDate = GetDateTime(button);
				buttonKind = DateNavigatorCalendarButtonKind.Date;
				return;
			}
			TextBlock tb = element as TextBlock;
			if (tb != null && tb.ReadLocalValue(WeekFirstDateProperty) != DependencyProperty.UnsetValue) {
				buttonDate = GetWeekFirstDate(tb);
				buttonKind = DateNavigatorCalendarButtonKind.WeekNumber;
				return;
			}
			buttonDate = null;
			buttonKind = DateNavigatorCalendarButtonKind.None;
		}
		protected internal DateTime GetWeekFirstDateByDate(DateTime dt) {
			return Content.GetWeekFirstDateByDate(dt);
		}
		protected virtual void UpdateDateTimeText() {
			DateTimeText = GetTodayText();
		}
		protected virtual void OnDateTimeChanged() {
			UpdateDateTimeText();
			if (Content != null && Content.DateTime != DateTime) {
				Content.DateTime = DateTime;
				Content.FillContent();
			}
			UpdateCurrentDateText();
		}
		protected virtual void OnPositionChanged() {
			if (Content != null)
				Content.FillContent();
		}
		protected virtual void OnStateChanged() {
			UpdateContent();
		}
		protected virtual void OnResizeToMaxContentChanged() {
			UpdateContentPaddingPanel();
		}
		protected virtual void OnShowWeekNumbersChanged() {
			if (Content != null)
				Content.FillContent();
		}
		protected internal virtual bool CanAddDate(DateTime date) {
			DateTime dt;
			switch (State) {
				case DateNavigatorCalendarView.Month:
					if (new DateTime(date.Year, date.Month, date.Day, 23, 59, 59) < MinValue)
						return false;
					if (new DateTime(date.Year, date.Month, date.Day, 0, 0, 0) > MaxValue)
						return false;
					date = new DateTime(date.Year, date.Month, 1);
					dt = new DateTime(DateTime.Year, DateTime.Month, 1);
					if (Position == DateNavigatorCalendarPosition.First && date > dt)
						return false;
					if (Position == DateNavigatorCalendarPosition.Intermediate && date != dt)
						return false;
					if (Position == DateNavigatorCalendarPosition.Last && date < dt)
						return false;
					break;
				case DateNavigatorCalendarView.Years:
					if (date < GetMinValue() && date.Year < GetMinValue().Year || date > GetMaxValue())
						return false;
					if (Position == DateNavigatorCalendarPosition.First && date.Year > DateTime.Year + 9)
						return false;
					if (Position == DateNavigatorCalendarPosition.Intermediate && (date.Year < DateTime.Year || date.Year > DateTime.Year + 9))
						return false;
					if (Position == DateNavigatorCalendarPosition.Last && date.Year < DateTime.Year)
						return false;
					break;
				case DateNavigatorCalendarView.YearsRange:
					int endYearGroup = date.Year / 10 * 10 + 9;
					if (date < GetMinValue() && endYearGroup < GetMinValue().Year || date > GetMaxValue())
						return false;
					if (Position == DateNavigatorCalendarPosition.First && date.Year > DateTime.Year + 99)
						return false;
					if (Position == DateNavigatorCalendarPosition.Intermediate && (date.Year < DateTime.Year || date.Year > DateTime.Year + 99))
						return false;
					if (Position == DateNavigatorCalendarPosition.Last && date.Year < DateTime.Year)
						return false;
					break;
			}
			return true;
		}
		protected virtual void PropertyChangedWeekNumberRule(CalendarWeekRule? oldValue) {
			RecreateContent();
		}
		protected virtual void PropertyChangedHighlightSpecialDates(bool oldValue) {
			UpdateSpecialDates();
		}
		protected virtual void PropertyChangedHighlightHolidays(bool oldValue) {
			UpdateHolidays();
		}
		protected virtual void PropertyChangedFirstDayOfWeek(DayOfWeek? oldValue) {
			RecreateContent();
		}
		protected virtual void RecreateContent() {
			if (Content != null)
				Content.FillContent();
		}
		void UpdateContentPaddingPanel() {
			Panel contentPaddingPanel = GetTemplateChild("PART_ContentPaddingPanel") as Panel;
			if (contentPaddingPanel == null) return;
			if (ResizeToMaxContent) {
				if (contentPaddingPanel.Children.Count == 0)
					foreach (DateNavigatorCalendarView state in DevExpress.Utils.EnumExtensions.GetValues(typeof(DateNavigatorCalendarView))) {
						DateNavigatorCalendarContent content = new DateNavigatorCalendarContent() { IsEnabled = false, Opacity = 0, State = state };
						content.SetBinding(TemplateProperty, new Binding(state.ToString() + "InfoTemplate") { Source = this });
						contentPaddingPanel.Children.Add(content);
					}
			} else
				contentPaddingPanel.Children.Clear();
		}
		protected virtual ControlTemplate GetTemplate(DateNavigatorCalendarView state) {
			switch (state) {
				case DateNavigatorCalendarView.Month:
					return MonthInfoTemplate;
				case DateNavigatorCalendarView.Year:
					return YearInfoTemplate;
				case DateNavigatorCalendarView.Years:
					return YearsInfoTemplate;
				case DateNavigatorCalendarView.YearsRange:
					return YearsRangeInfoTemplate;
			}
			return null;
		}
		protected bool IsSamePage(DateNavigatorCalendarView state, DateTime dt1, DateTime dt2) {
			switch (state) {
				case DateNavigatorCalendarView.Month:
					return dt1.Month == dt2.Month && dt1.Year == dt2.Year;
				case DateNavigatorCalendarView.Year:
					return dt1.Year == dt2.Year;
				case DateNavigatorCalendarView.Years:
					return dt1.Year / 10 == dt2.Year / 10;
				case DateNavigatorCalendarView.YearsRange:
					return dt1.Year / 100 == dt2.Year / 100;
			}
			return false;
		}
		protected internal virtual DateTime UpdateDate(DateNavigatorCalendarView state, DateTime dt, int dir) {
			int years = 0, months = 0;
			if (state == DateNavigatorCalendarView.Month)
				months = dir;
			else if (state == DateNavigatorCalendarView.Year)
				years = dir;
			else if (state == DateNavigatorCalendarView.Years) {
				if ((dt.Year == 10 && dir < 0) || (dt.Year == 1 && dir > 0))
					years = dir * 9;
				else
					years = dir * 10;
			} else if (state == DateNavigatorCalendarView.YearsRange) {
				if ((dt.Year == 100 && dir < 0) || (dt.Year == 1 && dir > 0))
					years = dir * 99;
				else
					years = dir * 100;
			}
			return AddDate(dt, years, months, 0);
		}
		protected internal DateTime GetMinValue() {
			if (MinValue.HasValue)
				return MinValue.Value;
			return DateTime.MinValue;
		}
		protected internal DateTime GetMaxValue() {
			if (MaxValue.HasValue)
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
					if (years < 0 || (years == 0 && months < 0) || (years == 0 && months == 0 && days < 0)) {
						dt = GetMinValue();
					} else
						dt = GetMaxValue();
				}
			if (dt < GetMinValue())
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
		protected internal virtual void OnButtonClick(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			RaiseButtonClick(buttonDate, buttonKind);
		}
		protected virtual void RaiseButtonClick(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (ButtonClick != null)
				ButtonClick(this, new DateNavigatorCalendarButtonClickEventArgs(buttonDate, buttonKind));
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
			SetMonth((DateTime)DateNavigatorCalendar.GetDateTime(button));
		}
		protected virtual void OnYearCellButtonClick(Button button) {
			SetYear((DateTime)DateNavigatorCalendar.GetDateTime(button));
		}
		protected virtual void OnYearsGroupCellButtonClick(Button button) {
			SetYearGroup((DateTime)DateNavigatorCalendar.GetDateTime(button));
		}
		protected virtual void OnDayCellButtonClick(Button button) {
			if (OwnerDateEdit == null) {
				if (button != null)
					DateTime = ((DateTime)DateNavigatorCalendar.GetDateTime(button));
				return;
			}
			if (!OwnerDateEdit.IsReadOnly)
				OwnerDateEdit.SetDateTime((DateTime)DateNavigatorCalendar.GetDateTime(button), UpdateEditorSource.ValueChanging);
			OwnerDateEdit.IsPopupOpen = false;
		}
		protected virtual void FindButtonsInTemplate() {
		}
		public DateNavigatorCalendar Calendar {
			get { return (DateNavigatorCalendar)GetValue(CalendarProperty); }
			set { SetValue(CalendarProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Content = GetTemplateChild("PART_Content") as DateNavigatorCalendarContent;
			UpdateContentPaddingPanel();
			FindButtonsInTemplate();
			if (ResizeToMaxContent)
				UpdateContent();
		}
		protected internal virtual void SetNewContent(DateTime dt, DateNavigatorCalendarTransferType transferType) {
		}
		protected internal virtual void UpdateContent() {
			if (Content != null && (wasLayoutUpdated || ResizeToMaxContent)) {
				Content.DateTime = DateTime;
				Content.State = State;
				Content.Template = GetTemplate(State);
			}
		}
		protected virtual string GetTodayText() {
			return string.Format(CultureInfo.CurrentCulture, "{0:d MMMM yyyy}", DateTime.Today);
		}
		protected virtual string GetMonthName(int month) {
			return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1];
		}
		protected internal virtual void OnApplyContentTemplate(DateNavigatorCalendarContent content) {
			UpdateCurrentDateText();
		}
		protected void UpdateCurrentDateText() {
			if (Content != null)
				CurrentDateText = Content.GetCurrentDateText();
		}
		DateNavigatorCalendarNavigatorBase navigator;
		protected DateNavigatorCalendarNavigatorBase Navigator {
			get {
				if (navigator == null)
					navigator = CreateNavigator(this);
				return navigator;
			}
		}
		protected virtual DateNavigatorCalendarNavigatorBase CreateNavigator(DateNavigatorCalendar dateEditCalendar) {
			return new DateNavigatorCalendarNavigator(this);
		}
		protected internal void UpdateSpecialDates() {
			if (Content != null)
				Content.UpdateSpecialDates();
		}
		protected internal void UpdateHolidays() {
			if (Content != null)
				Content.UpdateHolidays();
		}
		protected internal void UpdateSelectedDates() {
			if (Content != null)
				Content.UpdateSelectedDates();
		}
	}
	public class DateNavigatorCalendarNavigatorBase {
		DateNavigatorCalendar calendar;
		public DateNavigatorCalendarNavigatorBase(DateNavigatorCalendar calendar) {
			this.calendar = calendar;
		}
		public DateNavigatorCalendar Calendar { get { return calendar; } }
		protected virtual bool OnLeft() { return false; }
		protected virtual bool OnRight() { return false; }
		protected virtual bool OnUp() { return false; }
		protected virtual bool OnDown() { return false; }
		protected virtual bool OnPageUp() { return false; }
		protected virtual bool OnPageDown() { return false; }
		protected virtual bool OnHome() { return false; }
		protected virtual bool OnEnd() { return false; }
		protected virtual bool OnEnter() { return false; }
	}
	public class DateNavigatorCalendarNavigator : DateNavigatorCalendarNavigatorBase {
		public DateNavigatorCalendarNavigator(DateNavigatorCalendar calendar) : base(calendar) { }
		protected virtual FrameworkElement FindFocusedCell() {
			if (Calendar.ActiveContent.ContentGrid == null)
				return null;
			return Calendar.ActiveContent.GetFocusedCell();
		}
		protected virtual FrameworkElement FindCell(int row, int col) {
			if (Calendar.ActiveContent.ContentGrid == null)
				return null;
			foreach (UIElement elem in Calendar.ActiveContent.ContentGrid.Children) {
				if (Grid.GetColumn(elem as FrameworkElement) == col && Grid.GetRow(elem as FrameworkElement) == row)
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
				DateTime currDate = (DateTime)DateNavigatorCalendar.GetDateTime(elem);
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
			return FindCellWithDate((DateTime)DateNavigatorCalendar.GetDateTime(elem), IsNearLess, DateTime.MinValue);
		}
		protected virtual FrameworkElement FindCellWithGreaterDate() {
			FrameworkElement elem = FindFocusedCell();
			if (elem == null)
				return null;
			return FindCellWithDate((DateTime)DateNavigatorCalendar.GetDateTime(elem), IsNearGreater, DateTime.MaxValue);
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
			Calendar.SetNewContent(dt, DateNavigatorCalendarTransferType.ShiftLeft);
		}
		protected virtual void ShiftRight(DateTime dt) {
			Calendar.SetNewContent(dt, DateNavigatorCalendarTransferType.ShiftRight);
		}
		protected virtual void Shift(DateTime dt, int dir) {
			if (dir > 0)
				ShiftRight(dt);
			else
				ShiftLeft(dt);
		}
		protected virtual DateTime GetNextDate(FrameworkElement focusedCell, FrameworkElement nextCell, int upDownDir, int leftRightDir) {
			if (nextCell != null)
				return (DateTime)DateNavigatorCalendar.GetDateTime(nextCell);
			DateTime dt = (DateTime)DateNavigatorCalendar.GetDateTime(focusedCell);
			switch (Calendar.ActiveContent.State) {
				case DateNavigatorCalendarView.Month:
					return Calendar.AddDate(dt, 0, 0, upDownDir != 0 ? 7 * upDownDir : leftRightDir);
				case DateNavigatorCalendarView.Year:
					return Calendar.AddDate(dt, 0, upDownDir != 0 ? 4 * upDownDir : leftRightDir, 0);
				case DateNavigatorCalendarView.Years:
					return Calendar.AddDate(dt, upDownDir != 0 ? 4 * upDownDir : leftRightDir, 0, 0);
				case DateNavigatorCalendarView.YearsRange:
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
				DateNavigatorCalendar.SetCellFocused(focusedCell, false);
				if (!DateNavigatorCalendar.GetCellState(nextCell).HasFlag(DateNavigatorCalendarCellState.Inactive)) {
					DateNavigatorCalendar.SetCellFocused(nextCell, true);
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
			Calendar.OnButtonClick(DateNavigatorCalendar.GetDateTime(focusedCell), DateNavigatorCalendarButtonKind.Date);
			return true;
		}
	}
	public enum DateNavigatorCalendarTransferType { None, ShiftLeft, ShiftRight, ZoomIn, ZoomOut }
	[ToolboxItem(false)]
	public class DateNavigatorCalendarContent : ContentControl {
		#region static
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty DateTimeProperty;
		static DateNavigatorCalendarContent() {
			StateProperty = DependencyPropertyManager.Register("State", typeof(DateNavigatorCalendarView), typeof(DateNavigatorCalendarContent), new FrameworkPropertyMetadata(DateNavigatorCalendarView.Month, FrameworkPropertyMetadataOptions.None));
			DateTimeProperty = DependencyPropertyManager.Register("DateTime", typeof(DateTime), typeof(DateNavigatorCalendarContent), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDateTimePropertyChanged)));
		}
		protected static void OnDateTimePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateNavigatorCalendarContent)obj).OnDateTimeChanged();
		}
		#endregion
		public DateNavigatorCalendarContent() {
			CellButtonIndexList = new List<int>();
		}
		public DateTime DateTime {
			get { return (DateTime)GetValue(DateTimeProperty); }
			set { SetValue(DateTimeProperty, value); }
		}
		public DateNavigatorCalendarView State {
			get { return (DateNavigatorCalendarView)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public DateEdit OwnerDateEdit { get { return (DateEdit)BaseEdit.GetOwnerEdit(this); } }
		public DateNavigatorCalendar Calendar { get { return DateNavigatorCalendar.GetCalendar(this); } }
		protected int FirstCellIndex { get; set; }
		protected DateTime FirstVisibleDate { get; set; }
		protected DateTimeFormatInfo DateTimeFormat { get { return CultureInfo.CurrentCulture.DateTimeFormat; } }
		protected DayOfWeek FirstDayOfWeek { get { return Calendar == null || Calendar.FirstDayOfWeek == null ? DateTimeFormat.FirstDayOfWeek : (DayOfWeek)Calendar.FirstDayOfWeek; } }
		protected List<int> CellButtonIndexList { get; private set; }
		protected internal virtual string GetCurrentDateText() {
			return DateNavigatorHelper.GetDateText(State, DateTime);
		}
		protected virtual string GetMonthName(int month) {
			return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[month - 1];
		}
		protected virtual void OnDateTimeChanged() {
			if (Calendar != null)
				Calendar.DateTime = DateTime;
		}
		public void GetDateRange(bool excludeInactiveContent, out DateTime firstDate, out DateTime lastDate) {
			if (excludeInactiveContent) {
				firstDate = DateTime;
				switch (State) {
					case DateNavigatorCalendarView.Month:
						lastDate = new DateTime(DateTime.Year, DateTime.Month, DateTime.DaysInMonth(DateTime.Year, DateTime.Month));
						break;
					case DateNavigatorCalendarView.Year:
						lastDate = new DateTime(DateTime.Year, 12, 1);
						break;
					case DateNavigatorCalendarView.Years:
						lastDate = new DateTime(DateTime.Year / 10 * 10 + 9, 1, 1);
						break;
					case DateNavigatorCalendarView.YearsRange:
						lastDate = new DateTime(DateTime.Year / 100 * 100 + 99, 1, 1);
						break;
					default:
						throw new Exception();
				}
			} else {
				if (ContentGrid == null)
					ApplyTemplate();
				firstDate = DateNavigatorCalendar.GetDateTime(ContentGrid.Children[CellButtonIndexList[0]]);
				lastDate = DateNavigatorCalendar.GetDateTime(ContentGrid.Children[CellButtonIndexList[CellButtonIndexList.Count - 1]]);
				switch (State) {
					case DateNavigatorCalendarView.Year:
						lastDate = new DateTime(lastDate.Year, lastDate.Month, DateTime.DaysInMonth(lastDate.Year, lastDate.Month));
						break;
					case DateNavigatorCalendarView.Years:
						lastDate = new DateTime(lastDate.Year, 12, 31);
						break;
					case DateNavigatorCalendarView.YearsRange:
						lastDate = new DateTime(lastDate.Year + 9, 12, 31);
						break;
				}
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentGrid = GetTemplateChild("PART_ContentGrid") as Grid;
			if (Calendar != null)
				Calendar.OnApplyContentTemplate(this);
			FillContent();
			if (State == DateNavigatorCalendarView.Month)
				UpdateSelectedDates();
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
			if (currYear <= 0 || currYear >= 10000) {
				AddCellInfo(row, col);
				return;
			}
			DateTime dt = new DateTime(currYear, 1, 1);
			AddCellInfo(row, col, dt, dt.ToString("yyyy", CultureInfo.CurrentCulture), !Calendar.CanAddDate(dt));
		}
		protected virtual void CreateYearsGroupCellInfo(int row, int col) {
			int beginYearGroup = (DateTime.Year / 100) * 100 - 10;
			int currYearGroup = beginYearGroup + (row * 4 + col) * 10;
			if (currYearGroup < 0 || currYearGroup >= 10000) {
				AddCellInfo(row, col);
				return;
			}
			int endYearGroup = currYearGroup + 9;
			if (currYearGroup == 0)
				currYearGroup = 1;
			DateTime dt = new DateTime(currYearGroup, 1, 1);
			AddCellInfo(row, col, dt, new DateTime(currYearGroup, 1, 1).ToString("yyyy", CultureInfo.CurrentCulture) + "-\n" + new DateTime(endYearGroup, 1, 1).ToString("yyyy", CultureInfo.CurrentCulture), !Calendar.CanAddDate(dt));
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
					AddCellInfo(row, col, current, current.Day.ToString(CultureInfo.CurrentCulture), !Calendar.CanAddDate(current));
				}
				if (current != Calendar.MinValue)
					AddWeekNumber(row, current, Calendar.ShowWeekNumbers);
			}
		}
		protected virtual int CellFirstRow {
			get {
				if (State == DateNavigatorCalendarView.Month)
					return 2;
				return 0;
			}
		}
		protected virtual int CellFirstCol {
			get {
				if (State == DateNavigatorCalendarView.Month)
					return 2;
				return 0;
			}
		}
		protected virtual void AddWeekNumber(int row, DateTime current, bool showWeekNumbers) {
			TextBlock block = (TextBlock)FindChild(0, CellFirstRow + row, typeof(TextBlock));
			if (!showWeekNumbers) {
				if (block != null)
					ContentGrid.Children.Remove(block);
				return;
			}
			bool isBlockFound = block != null;
			if (!isBlockFound) {
				block = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Right };
				Grid.SetRow(block, CellFirstRow + row);
				Grid.SetColumn(block, 0);
			}
			block.Text = GetWeekNumber(current).ToString(CultureInfo.CurrentCulture);
			DateNavigatorCalendarCellButton button = (DateNavigatorCalendarCellButton)FindChild(CellFirstCol, CellFirstRow + row, typeof(DateNavigatorCalendarCellButton));
			DateNavigatorCalendar.SetWeekFirstDate(block, DateNavigatorCalendar.GetDateTime(button));
			block.Style = Calendar.WeekNumberStyle;
			block.MouseLeftButtonDown += OnWeekNumberMouseLeftButtonDown;
			if (!isBlockFound)
				ContentGrid.Children.Add(block);
		}
		protected virtual void OnWeekNumberMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Calendar.OnButtonClick(DateNavigatorCalendar.GetWeekFirstDate((TextBlock)sender), DateNavigatorCalendarButtonKind.WeekNumber);
		}
		protected virtual void UpdateMonthInfoCell(DependencyObject obj, DateTime current) {
			if (Calendar == null) return;
			DateNavigatorCalendar.SetCellInactive(obj, current.Month != DateTime.Month);
			DateNavigatorCalendar.SetCellToday(obj, current.Month == DateTime.Now.Month && current.Year == DateTime.Now.Year && current.Day == DateTime.Now.Day);
			DateNavigator navigator = DateNavigator.GetNavigator(this);
			if (navigator != null) {
				List<DateTime> specialDateList = DateNavigatorHelper.GetSpecialDateList(navigator);
				DateTime focusedDateTime = navigator.FocusedDate;
				DateNavigatorCalendar.SetCellFocused(obj, DateComparer.Equals(State, current, focusedDateTime));
				DateNavigatorCalendar.SetCellSpecial(obj, specialDateList.Contains(current.Date) && Calendar.HighlightSpecialDates);
				IDateCalculationService dateCalculationService = (IDateCalculationService)((IServiceProvider)navigator).GetService(typeof(IDateCalculationService));
				if (dateCalculationService != null)
					DateNavigatorCalendar.SetCellHoliday(obj, !dateCalculationService.IsWorkday(current) && Calendar.HighlightHolidays);
				IValueEditingService valueEditingService = (IValueEditingService)((IServiceProvider)navigator).GetService(typeof(IValueEditingService));
				if (valueEditingService != null)
					DateNavigatorCalendar.SetCellSelected(obj, SelectedDatesHelper.Contains(valueEditingService.SelectedDates, current.Date));
			}
		}
		protected virtual void UpdateYearInfoCell(DependencyObject obj, DateTime current) {
			UpdateCellFocusedState(obj, current);
		}
		protected virtual void UpdateYearsInfoCell(DependencyObject obj, DateTime current) {
			if (current.Year / 10 != DateTime.Year / 10)
				DateNavigatorCalendar.SetCellInactive(obj, true);
			UpdateCellFocusedState(obj, current);
		}
		protected virtual void UpdateYearsGroupInfoCell(DependencyObject obj, DateTime current) {
			if (current.Year / 100 != DateTime.Year / 100)
				DateNavigatorCalendar.SetCellInactive(obj, true);
			UpdateCellFocusedState(obj, current);
		}
		void UpdateCellFocusedState(DependencyObject obj, DateTime current) {
			DateNavigator navigator = DateNavigator.GetNavigator(this);
			if (navigator != null)
				DateNavigatorCalendar.SetCellFocused(obj, DateComparer.Equals(State, current, navigator.FocusedDate));
		}
		protected virtual void UpdateCellInfo(DependencyObject obj, DateTime current) {
			switch (State) {
				case DateNavigatorCalendarView.Month:
					UpdateMonthInfoCell(obj, current);
					break;
				case DateNavigatorCalendarView.Year:
					UpdateYearInfoCell(obj, current);
					break;
				case DateNavigatorCalendarView.Years:
					UpdateYearsInfoCell(obj, current);
					break;
				case DateNavigatorCalendarView.YearsRange:
					UpdateYearsGroupInfoCell(obj, current);
					break;
			}
		}
		protected internal void UpdateCellInfos() {
			if (ContentGrid != null)
				foreach (DependencyObject o in ContentGrid.Children)
					UpdateCellInfo(o, (DateTime)DateNavigatorCalendar.GetDateTime(o));
		}
		protected virtual void AddCellInfo(int row, int col) {
			AddCellInfo(row, col, DateTime.MinValue, string.Empty, false);
		}
		protected virtual void AddCellInfo(int row, int col, DateTime current, string content, bool isHidden) {
			DateNavigatorCalendarCellButton button = (DateNavigatorCalendarCellButton)FindChild(CellFirstCol + col, CellFirstRow + row, typeof(DateNavigatorCalendarCellButton));
			bool buttonFound = button != null;
			if (!buttonFound) {
				button = new DateNavigatorCalendarCellButton() { CalendarView = State };
				FocusHelper2.SetFocusable(button, false);
				button.Click += OnCellButtonClick;
				Grid.SetRow(button, CellFirstRow + row);
				Grid.SetColumn(button, CellFirstCol + col);
				ContentGrid.Children.Add(button);
			}
			button.Content = content;
			if (string.IsNullOrEmpty(content))
				UIElementHelper.Collapse(button);
			else
				UIElementHelper.Show(button);
			DateNavigatorCalendar.SetDateTime(button, current);
			button.IsEnabled = !isHidden;
			button.Opacity = isHidden ? 0 : 1;
			UpdateCellInfo(button, current);
		}
		UIElement FindChild(int col, int row, Type childType) {
			foreach (UIElement child in ContentGrid.Children)
				if (Grid.GetColumn((FrameworkElement)child) == col && Grid.GetRow((FrameworkElement)child) == row) {
					if (childType.IsInstanceOfType(child))
						return child;
					ContentGrid.Children.Clear();
					break;
				}
			return null;
		}
		protected virtual void OnCellButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			DateNavigatorCalendarCellButton button = (DateNavigatorCalendarCellButton)sender;
			button.ReleaseMouseCapture();
#if SL
			button.ResetIsMouseOver();
#endif      
			Calendar.OnButtonClick(DateNavigatorCalendar.GetDateTime(button), DateNavigatorCalendarButtonKind.Date);
		}
		protected virtual void FillContentInfo() {
			CalcFirstVisibleDate();
			switch (State) {
				case DateNavigatorCalendarView.Month:
					FillMonthInfo();
					break;
				case DateNavigatorCalendarView.Year:
					FillYearInfo();
					break;
				case DateNavigatorCalendarView.Years:
					FillYearsInfo();
					break;
				case DateNavigatorCalendarView.YearsRange:
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
			if (State != DateNavigatorCalendarView.Month)
				return;
			int gridCol = 2;
			for (int i = 0; i < DateTimeFormat.ShortestDayNames.Length; i++) {
				TextBlock block = (TextBlock)FindChild(gridCol, 0, typeof(TextBlock));
				bool isBlockFound = block != null;
				string abbr = DateTimeFormat.ShortestDayNames[(Convert.ToInt32(FirstDayOfWeek) + i) % 7];
				if (!isBlockFound) {
					block = new TextBlock();
					Grid.SetRow(block, 0);
					Grid.SetColumn(block, gridCol);
					ContentGrid.Children.Add(block);
				}
				block.Text = abbr;
				block.Style = Calendar.WeekdayAbbreviationStyle;
				gridCol++;
			}
		}
		protected virtual int GetWeekNumber(DateTime date) {
			return DateTimeFormat.Calendar.GetWeekOfYear(date, GetWeekNumberRule(), FirstDayOfWeek);
		}
		protected virtual CalendarWeekRule GetWeekNumberRule() {
			return (Calendar.WeekNumberRule == null) ? DateTimeFormat.CalendarWeekRule : (CalendarWeekRule)Calendar.WeekNumberRule;
		}
		protected internal virtual void FillContent() {
			if (ContentGrid == null || Calendar == null)
				return;
			FillWeekDaysAbbreviation();
			FillContentInfo();
			FillCellButtonIndexList();
		}
		protected void FillCellButtonIndexList() {
			CellButtonIndexList.Clear();
			for (int i = 0; i < ContentGrid.Children.Count; i++) {
				UIElement child = ContentGrid.Children[i];
				if (child is DateNavigatorCalendarCellButton && !String.IsNullOrEmpty((string)(child as DateNavigatorCalendarCellButton).Content))
					CellButtonIndexList.Add(i);
			}
		}
		protected internal object GetTemplateChildCore(string name) { return GetTemplateChild(name); }
		public Grid ContentGrid { get; private set; }
		public FrameworkElement GetFocusedCell() {
			if (ContentGrid != null) {
				foreach (UIElement elem in ContentGrid.Children) {
					if (DateNavigatorCalendar.GetCellState(elem).HasFlag(DateNavigatorCalendarCellState.Focused))
						return elem as FrameworkElement;
				}
			}
			return null;
		}
		internal DateNavigatorCalendarCellButton FindCellButton(DateTime cellDateTime) {
			if (ContentGrid == null) return null;
			foreach (UIElement child in ContentGrid.Children)
				if (child is DateNavigatorCalendarCellButton && IsCellButtonDateTime(cellDateTime, DateNavigatorCalendar.GetDateTime(child)))
					return (DateNavigatorCalendarCellButton)child;
			return null;
		}
		bool IsCellButtonDateTime(DateTime dt, DateTime cellButtonDateTime) {
			switch (State) {
				case DateNavigatorCalendarView.Month:
					return dt == cellButtonDateTime;
				case DateNavigatorCalendarView.Year:
					return dt.Year == cellButtonDateTime.Year && dt.Month == cellButtonDateTime.Month;
				case DateNavigatorCalendarView.Years:
					return dt.Year == cellButtonDateTime.Year;
				case DateNavigatorCalendarView.YearsRange:
					return dt.Year / 10 == cellButtonDateTime.Year / 10;
			}
			throw new Exception();
		}
		protected internal void UpdateSpecialDates() {
			DateNavigator navigator = DateNavigator.GetNavigator(this);
			if (ContentGrid == null || navigator == null) return;
			List<DateTime> specialDates = DateNavigatorHelper.GetSpecialDateList(navigator);
			foreach (UIElement child in ContentGrid.Children)
				if (child is DateNavigatorCalendarCellButton)
					DateNavigatorCalendar.SetCellStateFlag(child, DateNavigatorCalendarCellState.Special, SelectedDatesHelper.Contains(specialDates, DateNavigatorCalendar.GetDateTime(child)) && Calendar.HighlightSpecialDates);
		}
		protected internal void UpdateHolidays() {
			DateNavigator navigator = DateNavigator.GetNavigator(this);
			if (ContentGrid == null || navigator == null) return;
			IDateCalculationService service = (IDateCalculationService)((IServiceProvider)navigator).GetService(typeof(IDateCalculationService));
			if (service != null)
				foreach (UIElement child in ContentGrid.Children)
					if (child is DateNavigatorCalendarCellButton)
						DateNavigatorCalendar.SetCellStateFlag(child, DateNavigatorCalendarCellState.Holiday, !service.IsWorkday(DateNavigatorCalendar.GetDateTime(child)) && Calendar.HighlightHolidays);
		}
		protected internal void UpdateSelectedDates() {
			DateNavigator navigator = DateNavigator.GetNavigator(this);
			if (ContentGrid == null || navigator == null) return;
			IValueEditingService valueEditingService = (IValueEditingService)((IServiceProvider)navigator).GetService(typeof(IValueEditingService));
			if (valueEditingService != null)
				UpdateDates(valueEditingService.SelectedDates, DateNavigatorCalendarCellState.Selected);
		}
		protected void UpdateDates(IList<DateTime> dates, DateNavigatorCalendarCellState flag) {
			if (ContentGrid == null) return;
			IList<DateTime> datesInCalendar = GetDatesForCalendar(dates).ToList();
			foreach (UIElement child in ContentGrid.Children)
				if (child is DateNavigatorCalendarCellButton)
					DateNavigatorCalendar.SetCellStateFlag(child, flag, SelectedDatesHelper.Contains(datesInCalendar, DateNavigatorCalendar.GetDateTime(child)));
		}
		IEnumerable<DateTime> GetDatesForCalendar(IEnumerable<DateTime> dates) {
			if (Calendar == null || dates == null)
				return new List<DateTime>();
			DateTime minValue, maxValue;
			GetDateRange(false, out minValue, out maxValue);
			return dates.Where(dt => dt >= minValue && dt <= maxValue);
		}
		public DateNavigatorCalendarCellButton GetCellButton(System.DateTime dt) {
			int index = -1;
			DateTime firstButtonDateTime = DateNavigatorCalendar.GetDateTime(ContentGrid.Children[CellButtonIndexList[0]]);
			switch (State) {
				case DateNavigatorCalendarView.Month:
					index = (dt - firstButtonDateTime).Days;
					break;
				case DateNavigatorCalendarView.Year:
					index = (dt.Month - firstButtonDateTime.Month);
					break;
				case DateNavigatorCalendarView.Years:
					index = (dt.Year - firstButtonDateTime.Year);
					break;
				case DateNavigatorCalendarView.YearsRange:
					index = (dt.Year / 10 - firstButtonDateTime.Year / 10);
					break;
			}
			return (DateNavigatorCalendarCellButton)ContentGrid.Children[CellButtonIndexList[index]];
		}
		protected internal DateTime GetWeekFirstDateByDate(DateTime dt) {
			TextBlock tb = (TextBlock)FindChild(0, Grid.GetRow(GetCellButton(dt)), typeof(TextBlock));
			return DateNavigatorCalendar.GetWeekFirstDate(tb);
		}
	}
	public class DateNavigatorContentDateRangeChangedEventArgs : EventArgs {
		public DateNavigatorContentDateRangeChangedEventArgs(bool isScrolling) {
			IsScrolling = isScrolling;
		}
		public bool IsScrolling { get; private set; }
	}
	public delegate void DateNavigatorContentDateRangeChangedEventHandler(object sender, DateNavigatorContentDateRangeChangedEventArgs e);
	public interface IDateNavigatorContent {
		int CalendarCount { get; }
		DateTime DateTime { get; }
		DateTime EndDate { get; }
		DateTime FocusedDate { get; set; }
		DateTime StartDate { get; }
		event DateNavigatorCalendarButtonClickEventHandler CalendarButtonClick;
		event DateNavigatorContentDateRangeChangedEventHandler DateRangeChanged;
		DateNavigatorCalendar GetCalendar(int index);
		DateNavigatorCalendar GetCalendar(DateTime dt);
		DateNavigatorCalendar GetCalendar(DateTime dt, bool excludeInactiveContent);
		void GetDateRange(bool excludeInactiveContent, out DateTime firstDate, out DateTime lastDate);
		DateTime GetWeekFirstDateByDate(DateTime dt);
		void HitTest(UIElement element, out DateTime? buttonDate, out DateNavigatorCalendarButtonKind buttonKind);
		void UpdateCalendarsSpecialDates();
		void UpdateCalendarsHolidays();
		void UpdateCalendarsSelectedDates();
		void UpdateCalendarsStyle();
		void VisibilityChanged();
	}
	public class DateNavigatorContent : Control, IDateNavigatorContentPanelOwner, IDateNavigatorContent, IDateNavigatorCalendarOwner {
		DateTime dateTime;
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty FocusedDateProperty;
		public static readonly DependencyProperty PanelOwnerProperty;
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty RowCountProperty;
		static DateNavigatorContent() {
			Type ownerType = typeof(DateNavigatorContent);
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType, new PropertyMetadata(0), ValidatePropertyValueColumnCount);
			FocusedDateProperty = DependencyPropertyManager.Register("FocusedDate", typeof(DateTime), ownerType,
				new PropertyMetadata((d, e) => ((DateNavigatorContent)d).PropertyChangedFocusedDate((DateTime)e.OldValue)));
			PanelOwnerProperty = DependencyPropertyManager.RegisterAttached("PanelOwner", typeof(IDateNavigatorContentPanelOwner), ownerType, new PropertyMetadata(null));
			RowCountProperty = DependencyPropertyManager.Register("RowCount", typeof(int), ownerType, new PropertyMetadata(0), ValidatePropertyValueRowCount);
			StateProperty = DependencyPropertyManager.Register("State", typeof(DateNavigatorCalendarView), ownerType,
				new PropertyMetadata((d, e) => ((DateNavigatorContent)d).PropertyChangedState()));
		}
		public static IDateNavigatorContentPanelOwner GetPanelOwner(DependencyObject obj) {
			return (IDateNavigatorContentPanelOwner)obj.GetValue(PanelOwnerProperty);
		}
		public static void SetPanelOwner(DependencyObject obj, IDateNavigatorContentPanelOwner value) {
			obj.SetValue(PanelOwnerProperty, value);
		}
		static bool ValidatePropertyValueColumnCount(object value) {
			return ((int)value) >= 0;
		}
		static bool ValidatePropertyValueRowCount(object value) {
			return ((int)value) >= 0;
		}
		Locker supportInitializeLocker = new Locker();
		public DateNavigatorContent() {
			this.SetDefaultStyleKey(typeof(DateNavigatorContent));
		}
		event DateNavigatorCalendarButtonClickEventHandler calendarButtonClick;
		event DateNavigatorContentDateRangeChangedEventHandler dateRangeChanged;
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		public DateTime DateTime {
			get { return dateTime; }
		}
		public void SetDateTime(DateTime value, bool scrollIfValueInactive) {
			if (value == dateTime && !(!IsInSupportInitializing && GetCalendar(value, scrollIfValueInactive) == null)) return;
			dateTime = value;
			UpdateDateTime(scrollIfValueInactive);
		}
		public DateTime FocusedDate {
			get { return (DateTime)GetValue(FocusedDateProperty); }
			set { SetValue(FocusedDateProperty, value); }
		}
		public DateNavigator Navigator { get { return DateNavigator.GetNavigator(this); } }
		public int RowCount {
			get { return (int)GetValue(RowCountProperty); }
			set { SetValue(RowCountProperty, value); }
		}
		public DateNavigatorCalendarView State {
			get { return (DateNavigatorCalendarView)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		protected DateTime BaseDateTime {
			get {
				return GetBaseDateTime(State, DateTime);
			}
		}
		protected internal int ItemCount {
			get { return (Panel != null) ? Panel.Children.Count : 0; }
		}
		protected bool IsInSupportInitializing { get { return supportInitializeLocker.IsLocked; } }
		protected Panel Panel { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Panel = GetTemplateChild("PART_Panel") as Panel;
			if (Panel != null)
				SetPanelOwner(Panel, this);
		}
		protected void ClearCalendarCellButtonFocusedState(DateTime dt) {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(dt);
			if (button != null)
				DateNavigatorCalendar.SetCellFocused(button, false);
		}
		protected DateTime GetBaseDateTime(DateNavigatorCalendarView state, DateTime dt) {
			switch (state) {
				case DateNavigatorCalendarView.Month:
					return new DateTime(dt.Year, dt.Month, 1);
				case DateNavigatorCalendarView.Year:
					return new DateTime(dt.Year, 1, 1);
				case DateNavigatorCalendarView.Years:
					return new DateTime(Math.Max(dt.Year / 10 * 10, 1), 1, 1);
				case DateNavigatorCalendarView.YearsRange:
					return new DateTime(Math.Max(dt.Year / 100 * 100, 1), 1, 1);
			}
			throw new Exception();
		}
		protected virtual DateNavigatorCalendar GetCalendar(DateTime dt, bool excludeInactiveContent) {
			if (ItemCount == 0) return null;
			int index = -1;
			DateTime firstItemDateTime = GetItem(0).DateTime;
			switch (State) {
				case DateNavigatorCalendarView.Month:
					index = (dt.Year - firstItemDateTime.Year) * 12 + dt.Month - firstItemDateTime.Month;
					break;
				case DateNavigatorCalendarView.Year:
					index = dt.Year - firstItemDateTime.Year;
					break;
				case DateNavigatorCalendarView.Years:
					index = dt.Year / 10 - firstItemDateTime.Year / 10;
					break;
				case DateNavigatorCalendarView.YearsRange:
					index = dt.Year / 100 - firstItemDateTime.Year / 100;
					break;
			}
			DateNavigatorCalendar result = (index >= 0 && index < ItemCount) ? GetItem(index) : null;
			if (result == null && !excludeInactiveContent) {
				if (CheckCalendarDateRange(0, dt))
					result = GetItem(0);
				else
					if (ItemCount > 1 && CheckCalendarDateRange(ItemCount - 1, dt))
						result = GetItem(ItemCount - 1);
			}
			return result;
		}
		protected DateNavigatorCalendarCellButton GetCalendarCellButton(DateTime dt) {
			DateNavigatorCalendar calendar = GetCalendar(dt, false);
			return (calendar != null) ? calendar.GetCellButton(dt) : null;
		}
		protected DateTime GetDateTimeForRightBringToView() {
			switch (State) {
				case DateNavigatorCalendarView.Month:
					return BaseDateTime.AddMonths(-(ItemCount - 1));
				case DateNavigatorCalendarView.Year:
					return BaseDateTime.AddYears(-(ItemCount - 1));
				case DateNavigatorCalendarView.Years:
					return BaseDateTime.AddYears(-(ItemCount - 1) * 10);
				case DateNavigatorCalendarView.YearsRange:
					return BaseDateTime.AddYears(-(ItemCount - 1) * 100);
			}
			throw new Exception();
		}
		protected internal DateNavigatorCalendar GetItem(int itemIndex) {
			return (DateNavigatorCalendar)Panel.Children[itemIndex];
		}
		protected virtual void OnCalendarButtonClick(object sender, DateNavigatorCalendarButtonClickEventArgs e) {
			RaiseCalendarButtonClick(e);
		}
		protected virtual void PropertyChangedFocusedDate(DateTime oldValue) {
			ClearCalendarCellButtonFocusedState(oldValue);
			SetCalendarCellButtonFocusedState(FocusedDate);
		}
		protected virtual void PropertyChangedState() {
			for (int i = 0; i < ItemCount; i++)
				GetItem(i).State = State;
			UpdateDateTime(false);
		}
		protected virtual void RaiseCalendarButtonClick(DateNavigatorCalendarButtonClickEventArgs e) {
			if (calendarButtonClick != null)
				calendarButtonClick(this, e);
		}
		protected void RaiseDateRangeChanged(bool isScrolling) {
			if (dateRangeChanged != null)
				dateRangeChanged(this, new DateNavigatorContentDateRangeChangedEventArgs(isScrolling));
		}
		protected void SetCalendarCellButtonFocusedState(DateTime dt) {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(dt);
			if (button != null)
				DateNavigatorCalendar.SetCellFocused(button, true);
		}
		protected void UpdateCalendarSpecialDates(DateNavigatorCalendar calendar) {
			calendar.UpdateSpecialDates();
		}
		protected void UpdateCalendarSelectedDates(DateNavigatorCalendar calendar) {
			calendar.UpdateSelectedDates();
		}
		bool CheckCalendarDateRange(int calendarIndex, DateTime dt) {
			DateNavigatorCalendar calendar = GetItem(calendarIndex);
			DateTime firstDate, lastDate;
			calendar.GetDateRange(false, out firstDate, out lastDate);
			return firstDate <= dt && dt <= lastDate;
		}
		void GetDateRange(bool excludeInactiveContent, out DateTime firstDate, out DateTime lastDate) {
			if (ItemCount == 0) {
				firstDate = BaseDateTime;
				lastDate = BaseDateTime;
			} else {
				DateTime tempDate;
				GetItem(0).GetDateRange(excludeInactiveContent, out firstDate, out tempDate);
				GetItem(ItemCount - 1).GetDateRange(excludeInactiveContent, out tempDate, out lastDate);
			}
		}
		DateTime GetItemDateTime(DateTime baseDateTime, int i) {
			if (ItemCount > 0 && i > 0)
				baseDateTime = GetItem(0).DateTime;
			switch (State) {
				case DateNavigatorCalendarView.Month:
					return baseDateTime.AddMonths(i);
				case DateNavigatorCalendarView.Year:
					return baseDateTime.AddYears(i);
				case DateNavigatorCalendarView.Years:
					return new DateTime(Math.Max(baseDateTime.Year / 10 * 10 + i * 10, 1), 1, 1);
				case DateNavigatorCalendarView.YearsRange:
					return new DateTime(Math.Max(baseDateTime.Year / 100 * 100 + i * 100, 1), 1, 1);
				default:
					return baseDateTime;
			}
		}
		protected void UpdateDateTime(bool scrollIfValueInactive) {
			if (ItemCount == 0) return;
			DateTime baseDateTime;
			if (!IsInSupportInitializing) {
				if (GetCalendar(DateTime, scrollIfValueInactive) != null) return;
				if (DateTime < GetItem(0).DateTime)
					baseDateTime = BaseDateTime;
				else
					baseDateTime = GetDateTimeForRightBringToView();
			} else
				baseDateTime = BaseDateTime;
			for (int i = 0; i < ItemCount; i++) {
				DateNavigatorCalendar item = GetItem(i);
				DateTime itemPrevDateTime = item.DateTime;
				item.DateTime = GetItemDateTime(baseDateTime, i);
				if (i == ItemCount - 1 && itemPrevDateTime != item.DateTime)
					RaiseDateRangeChanged(true);
			}
		}
		#region IDateNavigatorPanelOwner Members
		bool IDateNavigatorContentPanelOwner.IsHidden {
			get {
				DateNavigator navigator = Navigator;
				return navigator != null && State != navigator.CalendarView;
			}
		}
		UIElement IDateNavigatorContentPanelOwner.CreateItem() {
			DateNavigatorCalendar result = new DateNavigatorCalendar(this);
			result.ButtonClick += OnCalendarButtonClick;
			result.DateTime = GetItemDateTime(BaseDateTime, ItemCount);
			result.State = State;
			DateNavigator navigator = Navigator;
			if (navigator != null && navigator.CalendarStyle != null)
				result.Style = navigator.CalendarStyle;
			DateNavigator.SetNavigator(result, navigator);
			if (ItemCount == 0 && State == DateNavigatorCalendarView.Month)
				result.ResizeToMaxContent = true;
			return result;
		}
		Size IDateNavigatorContentPanelOwner.GetItemSize() {
			DateNavigator navigator = Navigator;
			if (navigator != null) {
				DateNavigatorContent content = ((IDateNavigatorContentContainer)navigator).GetContent(DateNavigatorCalendarView.Month);
				if (content != null)
					return content.GetItem(0).DesiredSize;
			}
			return GetItem(0).DesiredSize;
		}
		void IDateNavigatorContentPanelOwner.ItemCountChanged() {
			RaiseDateRangeChanged(false);
		}
		void IDateNavigatorContentPanelOwner.UninitializeItem(UIElement item) {
			((DateNavigatorCalendar)item).ButtonClick -= OnCalendarButtonClick;
		}
		void IDateNavigatorContentPanelOwner.UpdateItemPositions() {
			if (ItemCount == 1)
				GetItem(0).Position = DateNavigatorCalendarPosition.Single;
			else {
				GetItem(0).Position = DateNavigatorCalendarPosition.First;
				GetItem(ItemCount - 1).Position = DateNavigatorCalendarPosition.Last;
				for (int i = 1; i < ItemCount - 1; i++)
					GetItem(i).Position = DateNavigatorCalendarPosition.Intermediate;
			}
		}
		#endregion
		#region IDateNavigatorContent Members
		int IDateNavigatorContent.CalendarCount {
			get { return ItemCount; }
		}
		DateTime IDateNavigatorContent.EndDate {
			get {
				DateTime firstDate, lastDate;
				GetDateRange(true, out firstDate, out lastDate);
				return lastDate;
			}
		}
		DateTime IDateNavigatorContent.StartDate {
			get {
				DateTime firstDate, lastDate;
				GetDateRange(true, out firstDate, out lastDate);
				return firstDate;
			}
		}
		event DateNavigatorCalendarButtonClickEventHandler IDateNavigatorContent.CalendarButtonClick {
			add { calendarButtonClick += value; }
			remove { calendarButtonClick -= value; }
		}
		event DateNavigatorContentDateRangeChangedEventHandler IDateNavigatorContent.DateRangeChanged {
			add { dateRangeChanged += value; }
			remove { dateRangeChanged -= value; }
		}
		DateNavigatorCalendar IDateNavigatorContent.GetCalendar(int index) {
			return GetItem(index);
		}
		DateNavigatorCalendar IDateNavigatorContent.GetCalendar(DateTime dt) {
			return GetCalendar(dt, true);
		}
		DateNavigatorCalendar IDateNavigatorContent.GetCalendar(DateTime dt, bool excludeInactiveContent) {
			return GetCalendar(dt, excludeInactiveContent);
		}
		void IDateNavigatorContent.GetDateRange(bool excludeInactiveContent, out DateTime firstDate, out DateTime lastDate) {
			GetDateRange(excludeInactiveContent, out firstDate, out lastDate);
		}
		DateTime IDateNavigatorContent.GetWeekFirstDateByDate(DateTime dt) {
			return GetCalendar(dt, false).GetWeekFirstDateByDate(dt);
		}
		void IDateNavigatorContent.HitTest(UIElement element, out DateTime? buttonDate, out DateNavigatorCalendarButtonKind buttonKind) {
			buttonDate = null;
			buttonKind = DateNavigatorCalendarButtonKind.None;
			if (element == null) return;
			DateNavigatorCalendar calendar = LayoutHelper.FindParentObject<DateNavigatorCalendar>(element);
			if (calendar != null && LayoutHelper.IsChildElement(this, calendar))
				calendar.HitTest(element, out buttonDate, out buttonKind);
		}
		void IDateNavigatorContent.UpdateCalendarsSpecialDates() {
			if (State != DateNavigatorCalendarView.Month)
				throw new Exception();
			for (int i = 0; i < ItemCount; i++)
				UpdateCalendarSpecialDates(GetItem(i));
		}
		void IDateNavigatorContent.UpdateCalendarsHolidays() {
			for (int i = 0; i < ItemCount; i++)
				GetItem(i).UpdateHolidays();
		}
		void IDateNavigatorContent.UpdateCalendarsSelectedDates() {
			if (State != DateNavigatorCalendarView.Month) return;
			for (int i = 0; i < ItemCount; i++)
				UpdateCalendarSelectedDates(GetItem(i));
		}
		void IDateNavigatorContent.UpdateCalendarsStyle() {
			DateNavigator navigator = Navigator;
			if (navigator != null)
				for (int i = 0; i < ItemCount; i++)
					GetItem(i).Style = navigator.CalendarStyle;
		}
		void IDateNavigatorContent.VisibilityChanged() {
			if (Panel != null) {
				Panel.InvalidateMeasure();
				Panel.UpdateLayout();
			}
		}
		#endregion
		#region IDateNavigatorCalendarOwner Members
		int IDateNavigatorCalendarOwner.CalendarCount {
			get {
				return ItemCount;
			}
		}
		int IDateNavigatorCalendarOwner.GetCalendarIndex(DateNavigatorCalendar calendar) {
			return Panel.Children.IndexOf(calendar);
		}
		#endregion
		#region ISupportInitialize
		public override void BeginInit() {
			if (!IsInSupportInitializing)
				base.BeginInit();
			supportInitializeLocker.Lock();
		}
		public override void EndInit() {
			supportInitializeLocker.Unlock();
			if (!IsInSupportInitializing)
				base.EndInit();
		}
		#endregion
	}
	public class DateNavigatorCalendarViewToClickModeConverter : IValueConverter {
		public DateNavigatorCalendarViewToClickModeConverter() {
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string[] list = ((string)parameter).Split(new char[] { ';' });
			return Enum.Parse(typeof(ClickMode), list[(int)((DateNavigatorCalendarView)value)], false);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DateNavigatorCalendarCellButtonContent : Control {
		public static readonly DependencyProperty DefaultForegroundSolidColorProperty;
		public static readonly DependencyProperty FocusedForegroundSolidColorProperty;
		public static readonly DependencyProperty HolidayForegroundSolidColorProperty;
		public static readonly DependencyProperty InactiveForegroundSolidColorProperty;
		public static readonly DependencyProperty MouseOverForegroundSolidColorProperty;
		public static readonly DependencyProperty SelectedForegroundSolidColorProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TodayForegroundSolidColorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsMouseOverInternalProperty;
		static DateNavigatorCalendarCellButtonContent() {
			Type ownerType = typeof(DateNavigatorCalendarCellButtonContent);
			DefaultForegroundSolidColorProperty = DependencyPropertyManager.Register("DefaultForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			FocusedForegroundSolidColorProperty = DependencyPropertyManager.Register("FocusedForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			HolidayForegroundSolidColorProperty = DependencyPropertyManager.Register("HolidayForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			InactiveForegroundSolidColorProperty = DependencyPropertyManager.Register("InactiveForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			IsMouseOverInternalProperty = DependencyPropertyManager.Register("IsMouseOverInternal", typeof(bool), ownerType,
			  new PropertyMetadata(false, (d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedIsMouseOverInternal()));
			MouseOverForegroundSolidColorProperty = DependencyPropertyManager.Register("MouseOverForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			SelectedForegroundSolidColorProperty = DependencyPropertyManager.Register("SelectedForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
			TextProperty = DependencyPropertyManager.Register("Text", typeof(string), ownerType,
			  new PropertyMetadata(null, (d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedText()));
			TodayForegroundSolidColorProperty = DependencyPropertyManager.Register("TodayForegroundSolidColor", typeof(Color), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigatorCalendarCellButtonContent)d).PropertyChangedForegroundSolidColor()));
		}
		public DateNavigatorCalendarCellButtonContent() {
#if !SL
			SnapsToDevicePixels = true;
#else
			UseLayoutRounding = true;
#endif
			SetBinding(IsMouseOverInternalProperty, new Binding("IsMouseOver") { Source = this });
		}
		public Color DefaultForegroundSolidColor {
			get { return (Color)GetValue(DefaultForegroundSolidColorProperty); }
			set { SetValue(DefaultForegroundSolidColorProperty, value); }
		}
		public Color FocusedForegroundSolidColor {
			get { return (Color)GetValue(FocusedForegroundSolidColorProperty); }
			set { SetValue(FocusedForegroundSolidColorProperty, value); }
		}
		public Color HolidayForegroundSolidColor {
			get { return (Color)GetValue(HolidayForegroundSolidColorProperty); }
			set { SetValue(HolidayForegroundSolidColorProperty, value); }
		}
		public Color InactiveForegroundSolidColor {
			get { return (Color)GetValue(InactiveForegroundSolidColorProperty); }
			set { SetValue(InactiveForegroundSolidColorProperty, value); }
		}
		public Color MouseOverForegroundSolidColor {
			get { return (Color)GetValue(MouseOverForegroundSolidColorProperty); }
			set { SetValue(MouseOverForegroundSolidColorProperty, value); }
		}
		public Color SelectedForegroundSolidColor {
			get { return (Color)GetValue(SelectedForegroundSolidColorProperty); }
			set { SetValue(SelectedForegroundSolidColorProperty, value); }
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public Color TodayForegroundSolidColor {
			get { return (Color)GetValue(TodayForegroundSolidColorProperty); }
			set { SetValue(TodayForegroundSolidColorProperty, value); }
		}
		protected UIElement ElementFocused { get; private set; }
		protected UIElement ElementMouseOver { get; private set; }
		protected UIElement ElementSelected { get; private set; }
		protected TextBlock ElementText { get; private set; }
		protected UIElement ElementToday { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ElementFocused = GetTemplateChild("PART_Focused") as UIElement;
			ElementMouseOver = GetTemplateChild("PART_MouseOver") as UIElement;
			ElementSelected = GetTemplateChild("PART_Selected") as UIElement;
			ElementText = GetTemplateChild("PART_Text") as TextBlock;
			ElementToday = GetTemplateChild("PART_Today") as UIElement;
			UpdateElements();
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == DateNavigatorCalendar.CellStateProperty)
				UpdateElements();
		}
		protected virtual void PropertyChangedForegroundSolidColor() {
			UpdateForeground();
		}
		protected virtual void PropertyChangedIsMouseOverInternal() {
			UpdateElements();
		}
		protected virtual void PropertyChangedText() {
			UpdateElements();
		}
		bool IsForegroundSolidColorPropertyAssigned(DependencyProperty dp) {
			return ((Color)GetValue(dp)) != default(Color);
		}
		void UpdateElements() {
			DateNavigatorCalendarCellState state = DateNavigatorCalendar.GetCellState(this);
			if (ElementFocused != null)
				ElementFocused.SetVisible(state.HasFlag(DateNavigatorCalendarCellState.Focused));
			if (ElementMouseOver != null)
				ElementMouseOver.SetVisible(IsMouseOver);
			if (ElementSelected != null)
				ElementSelected.SetVisible(state.HasFlag(DateNavigatorCalendarCellState.Selected));
			if (ElementText != null)
				ElementText.Text = Text;
			if (ElementToday != null)
				ElementToday.SetVisible(state.HasFlag(DateNavigatorCalendarCellState.Today));
			if (state.HasFlag(DateNavigatorCalendarCellState.Special))
				FontWeight = FontWeights.Bold;
			else
				ClearValue(FontWeightProperty);
			UpdateForeground();
		}
		void UpdateForeground() {
			DateNavigatorCalendarCellState state = DateNavigatorCalendar.GetCellState(this);
			if (state.HasFlag(DateNavigatorCalendarCellState.Focused) && IsForegroundSolidColorPropertyAssigned(FocusedForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(FocusedForegroundSolidColor);
				return;
			}
			if (state.HasFlag(DateNavigatorCalendarCellState.Selected) && IsForegroundSolidColorPropertyAssigned(SelectedForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(SelectedForegroundSolidColor);
				return;
			}
			if (IsMouseOver && IsForegroundSolidColorPropertyAssigned(MouseOverForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(MouseOverForegroundSolidColor);
				return;
			}
			if (state.HasFlag(DateNavigatorCalendarCellState.Today) && IsForegroundSolidColorPropertyAssigned(TodayForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(TodayForegroundSolidColor);
				return;
			}
			if (state.HasFlag(DateNavigatorCalendarCellState.Inactive) && IsForegroundSolidColorPropertyAssigned(InactiveForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(InactiveForegroundSolidColor);
				return;
			}
			if (state.HasFlag(DateNavigatorCalendarCellState.Holiday) && IsForegroundSolidColorPropertyAssigned(HolidayForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(HolidayForegroundSolidColor);
				return;
			}
			if (IsForegroundSolidColorPropertyAssigned(DefaultForegroundSolidColorProperty)) {
				Foreground = new SolidColorBrush(DefaultForegroundSolidColor);
				return;
			}
			ClearValue(ForegroundProperty);
		}
#if SL
		internal void ResetIsMouseOver() {
			SetValue(IsMouseOverProperty, false);
		}
#endif
	}
}
namespace DevExpress.Xpf.Editors.DateNavigator {
	public static class DateComparer {
		public static bool Equals(DateNavigatorCalendarView view, DateTime x, DateTime y) {
			switch (view) {
				case DateNavigatorCalendarView.Month:
					return x.Date == y.Date;
				case DateNavigatorCalendarView.Year:
					return x.Year == y.Year && x.Month == y.Month;
				case DateNavigatorCalendarView.Years:
					return x.Year == y.Year;
				case DateNavigatorCalendarView.YearsRange:
					return x.Year / 10 == y.Year / 10;
			}
			throw new Exception();
		}
	}
}
