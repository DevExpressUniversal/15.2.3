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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Editors.DateNavigator;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
using DevExpress.Xpf.Editors.DateNavigator.Internal;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Editors.DateNavigator {
	public enum DateNavigatorCalendarView { Month, Year, Years, YearsRange }
	public interface IDateNavigatorContentContainer {
		DateNavigatorContent GetContent(DateNavigatorCalendarView state);
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class DateNavigator : Control, IDateNavigatorContentContainer, IServiceProvider, ILogicalOwner {
		protected enum NavigationDirection { Left, Up, Right, Down }
		#region Static
		public static readonly DependencyProperty ActualFirstDayOfWeekProperty;
		public static readonly DependencyProperty CalendarViewProperty;
		public static readonly DependencyProperty MaxSelectionLengthProperty;
		public static readonly DependencyProperty CalendarStyleProperty;
		public static readonly DependencyProperty ColumnCountProperty;
		public static readonly DependencyProperty CurrentDateTextProperty;
		public static readonly DependencyProperty ExactWorkdaysProperty;
		public static readonly DependencyProperty FirstDayOfWeekProperty;
		public static readonly DependencyProperty FocusedDateProperty;
		public static readonly DependencyProperty HighlightHolidaysProperty;
		public static readonly DependencyProperty HighlightSpecialDatesProperty;
		public static readonly DependencyProperty HolidaysProperty;
		public static readonly DependencyProperty IsMultiSelectProperty;
		public static readonly DependencyProperty NavigatorProperty;
		public static readonly DependencyProperty RowCountProperty;
		public static readonly DependencyProperty SelectedDatesProperty;
		public static readonly DependencyProperty ShowTodayButtonProperty;
		public static readonly DependencyProperty ShowWeekNumbersProperty;
		public static readonly DependencyProperty SpecialDatesProperty;
		public static readonly DependencyProperty StyleSettingsProperty;
		public static readonly DependencyProperty WeekNumberRuleProperty;
		public static readonly DependencyProperty WorkdaysProperty;
		static readonly DependencyPropertyKey ActualFirstDayOfWeekPropertyKey;
		static readonly DependencyPropertyKey CurrentDateTextPropertyKey;
		static DateNavigator() {
			Type ownerType = typeof(DateNavigator);
			ActualFirstDayOfWeekPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualFirstDayOfWeek", typeof(DayOfWeek), ownerType, new PropertyMetadata((PropertyChangedCallback)null));
			ActualFirstDayOfWeekProperty = ActualFirstDayOfWeekPropertyKey.DependencyProperty;
			MaxSelectionLengthProperty = DependencyPropertyManager.Register("MaxSelectionLength", typeof(int), ownerType,
				new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.None, (d, e) => ((DateNavigator)d).MaxSelectionLengthChanged((int)e.OldValue, (int)e.NewValue)));
			CalendarViewProperty = DependencyPropertyManager.Register("CalendarView", typeof(DateNavigatorCalendarView), ownerType,
				new FrameworkPropertyMetadata(DateNavigatorCalendarView.Month, FrameworkPropertyMetadataOptions.None, (d, e) => ((DateNavigator)d).CalendarViewChanged((DateNavigatorCalendarView)e.OldValue, (DateNavigatorCalendarView)e.NewValue)));
			CalendarStyleProperty = DependencyPropertyManager.Register("CalendarStyle", typeof(Style), ownerType,
			  new PropertyMetadata(null, (d, e) => ((DateNavigator)d).PropertyChangedCalendarStyle((Style)e.OldValue)));
			ColumnCountProperty = DependencyPropertyManager.Register("ColumnCount", typeof(int), ownerType, new PropertyMetadata(0), ValidatePropertyValueColumnCount);
			CurrentDateTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("CurrentDateText", typeof(string), ownerType, new PropertyMetadata(null));
			CurrentDateTextProperty = CurrentDateTextPropertyKey.DependencyProperty;
			ExactWorkdaysProperty = DependencyPropertyManager.Register("ExactWorkdays", typeof(IList<DateTime>), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigator)d).PropertyChangedExactWorkdays((IList<DateTime>)e.OldValue)));
			FirstDayOfWeekProperty = DependencyPropertyManager.Register("FirstDayOfWeek", typeof(DayOfWeek?), ownerType,
			  new PropertyMetadata(null, (d, e) => ((DateNavigator)d).PropertyChangedFirstDayOfWeek((DayOfWeek?)e.OldValue)));
			FocusedDateProperty = DependencyPropertyManager.Register("FocusedDate", typeof(DateTime), ownerType,
				new FrameworkPropertyMetadata(System.DateTime.Today, FrameworkPropertyMetadataOptions.None, (d, e) => ((DateNavigator)d).FocusedDateChanged((DateTime)e.OldValue, (DateTime)e.NewValue)));
			HighlightHolidaysProperty = DependencyPropertyManager.Register("HighlightHolidays", typeof(bool), ownerType,
			  new PropertyMetadata(true, (d, e) => ((DateNavigator)d).PropertyChangedHighlightHolidays((bool)e.OldValue)));
			HighlightSpecialDatesProperty = DependencyPropertyManager.Register("HighlightSpecialDates", typeof(bool), ownerType,
			  new PropertyMetadata(true, (d, e) => ((DateNavigator)d).PropertyChangedHighlightSpecialDates((bool)e.OldValue)));
			HolidaysProperty = DependencyPropertyManager.Register("Holidays", typeof(IList<DateTime>), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigator)d).PropertyChangedHolidays((IList<DateTime>)e.OldValue)));
			IsMultiSelectProperty = DependencyPropertyManager.Register("IsMultiSelect", typeof(bool), ownerType,
				new PropertyMetadata(true, (d, e) => ((DateNavigator)d).IsMultiSelectChanged((bool)e.OldValue, (bool)e.NewValue)));
			NavigatorProperty = DependencyPropertyManager.RegisterAttached("Navigator", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			RowCountProperty = DependencyPropertyManager.Register("RowCount", typeof(int), ownerType, new PropertyMetadata(0), ValidatePropertyValueRowCount);
			SelectedDatesProperty = DependencyPropertyManager.Register("SelectedDates", typeof(IList<DateTime>), ownerType,
				new PropertyMetadata((d, e) => ((DateNavigator)d).PropertyChangedSelectedDates((IList<DateTime>)e.OldValue, (IList<DateTime>)e.NewValue)));
			ShowTodayButtonProperty = DependencyPropertyManager.Register("ShowTodayButton", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowWeekNumbersProperty = DependencyPropertyManager.Register("ShowWeekNumbers", typeof(bool), ownerType,
			  new PropertyMetadata(true, (d, e) => ((DateNavigator)d).PropertyChangedShowWeekNumbers()));
			SpecialDatesProperty = DependencyPropertyManager.Register("SpecialDates", typeof(IList<DateTime>), ownerType,
				new PropertyMetadata((d, e) => ((DateNavigator)d).PropertyChangedSpecialDates((IList<DateTime>)e.OldValue)));
			StyleSettingsProperty = DependencyPropertyManager.Register("StyleSettings", typeof(DateNavigatorStyleSettingsBase), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
					(d, e) => ((DateNavigator)d).StyleSettingsChanged((DateNavigatorStyleSettingsBase)e.OldValue, (DateNavigatorStyleSettingsBase)e.NewValue),
					(d, value) => ((DateNavigator)d).CreateDefaultStyleSettings(value)));
			WeekNumberRuleProperty = DependencyPropertyManager.Register("WeekNumberRule", typeof(CalendarWeekRule?), ownerType,
			  new PropertyMetadata(null, (d, e) => ((DateNavigator)d).PropertyChangedWeekNumberRule((CalendarWeekRule?)e.OldValue)));
			WorkdaysProperty = DependencyPropertyManager.Register("Workdays", typeof(IList<DayOfWeek>), ownerType,
			  new PropertyMetadata((d, e) => ((DateNavigator)d).PropertyChangedWorkdays((IList<DayOfWeek>)e.OldValue)));
		}
		public static DateNavigator GetNavigator(DependencyObject obj) {
			return (DateNavigator)DependencyObjectHelper.GetValueWithInheritance(obj, NavigatorProperty);
		}
		public static void SetNavigator(DependencyObject obj, DateNavigator value) { obj.SetValue(NavigatorProperty, value); }
		static bool ValidatePropertyValueColumnCount(object value) {
			return ((int)value) >= 0;
		}
		static bool ValidatePropertyValueRowCount(object value) {
			return ((int)value) >= 0;
		}
		#endregion
		protected internal SelectionManager SelectionManager { get; private set; }
		protected DateTime? shiftSelectionFirstDate;
		ButtonBase arrowLeft;
		ButtonBase arrowRight;
		Dictionary<DateNavigatorCalendarView, DateNavigatorContent> contents = new Dictionary<DateNavigatorCalendarView, DateNavigatorContent>();
		ButtonBase currentDateButton;
		Locker lockerDateTime = new Locker();
		Locker lockerSelectedDateList = new Locker();
		Locker lockerSelectedDates = new Locker();
		Locker lockerSyncSelectedDatesWithSelectedDateList = new Locker();
		WeakReference optionsProviderServiceReference;
		ButtonBase todayButton;
		public DateNavigator() {
			this.SetDefaultStyleKey(typeof(DateNavigator));
			SelectionManager = CreateSelectionManager();
			SetNavigator(this, this);
			this.SetCurrentValue(StyleSettingsProperty, CreateDefaultStyleSettings(null));
			this.SetCurrentValue(SpecialDatesProperty, new ObservableCollection<System.DateTime>());
			this.SetCurrentValue(SelectedDatesProperty, new ObservableCollection<System.DateTime>());
			this.SetCurrentValue(ExactWorkdaysProperty, new ObservableCollection<System.DateTime>());
			this.SetCurrentValue(HolidaysProperty, new ObservableCollection<DateTime>());
			this.SetCurrentValue(WorkdaysProperty, new ObservableCollection<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
			UpdateActualFirstDayOfWeek();
			UpdateActualWorkdaysProperties();
		}
		protected virtual SelectionManager CreateSelectionManager() {
			return new SelectionManager(this);
		}
		public DayOfWeek ActualFirstDayOfWeek {
			get { return (DayOfWeek)GetValue(ActualFirstDayOfWeekProperty); }
			protected set { this.SetValue(ActualFirstDayOfWeekPropertyKey, value); }
		}
		public int MaxSelectionLength {
			get { return (int)GetValue(MaxSelectionLengthProperty); }
			set { SetValue(MaxSelectionLengthProperty, value); }
		}
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		public string CurrentDateText {
			get { return (string)GetValue(CurrentDateTextProperty); }
			protected set { this.SetValue(CurrentDateTextPropertyKey, value); }
		}
		public IList<DateTime> ExactWorkdays {
			get { return (IList<DateTime>)GetValue(ExactWorkdaysProperty); }
			set { SetValue(ExactWorkdaysProperty, value); }
		}
		public DayOfWeek? FirstDayOfWeek {
			get { return (DayOfWeek?)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime FocusedDate {
			get { return (DateTime)GetValue(FocusedDateProperty); }
			set { SetValue(FocusedDateProperty, value); }
		}
		public IList<DateTime> SpecialDates {
			get { return (IList<DateTime>)GetValue(SpecialDatesProperty); }
			set { SetValue(SpecialDatesProperty, value); }
		}
		public bool HighlightHolidays {
			get { return (bool)GetValue(HighlightHolidaysProperty); }
			set { SetValue(HighlightHolidaysProperty, value); }
		}
		public bool HighlightSpecialDates {
			get { return (bool)GetValue(HighlightSpecialDatesProperty); }
			set { SetValue(HighlightSpecialDatesProperty, value); }
		}
		public IList<DateTime> Holidays {
			get { return (IList<DateTime>)GetValue(HolidaysProperty); }
			set { SetValue(HolidaysProperty, value); }
		}
		public bool IsMultiSelect {
			get { return (bool)GetValue(IsMultiSelectProperty); }
			set { SetValue(IsMultiSelectProperty, value); }
		}
		public int RowCount {
			get { return (int)GetValue(RowCountProperty); }
			set { SetValue(RowCountProperty, value); }
		}
		public IList<DateTime> SelectedDates {
			get { return (IList<DateTime>)GetValue(SelectedDatesProperty); }
			set { SetValue(SelectedDatesProperty, value); }
		}
		public bool ShowTodayButton {
			get { return (bool)GetValue(ShowTodayButtonProperty); }
			set { SetValue(ShowTodayButtonProperty, value); }
		}
		public bool ShowWeekNumbers {
			get { return (bool)GetValue(ShowWeekNumbersProperty); }
			set { SetValue(ShowWeekNumbersProperty, value); }
		}
		public DateNavigatorStyleSettingsBase StyleSettings {
			get { return (DateNavigatorStyleSettingsBase)GetValue(StyleSettingsProperty); }
			set { SetValue(StyleSettingsProperty, value); }
		}
		public CalendarWeekRule? WeekNumberRule {
			get { return (CalendarWeekRule?)GetValue(WeekNumberRuleProperty); }
			set { SetValue(WeekNumberRuleProperty, value); }
		}
		public IList<DayOfWeek> Workdays {
			get { return (IList<DayOfWeek>)GetValue(WorkdaysProperty); }
			set { SetValue(WorkdaysProperty, value); }
		}
		public Style CalendarStyle {
			get { return (Style)GetValue(CalendarStyleProperty); }
			set { SetValue(CalendarStyleProperty, value); }
		}
		public DateNavigatorCalendarView CalendarView {
			get { return (DateNavigatorCalendarView)GetValue(CalendarViewProperty); }
			set { SetValue(CalendarViewProperty, value); }
		}
		public event EventHandler SelectedDatesChanged;
		protected internal DateNavigatorContent ActiveContent { get { return GetContent(CalendarView); } }
		protected internal bool AreWorkdaysCollectionsInvalid { get; set; }
		DateNavigatorContent GetContent(DateNavigatorCalendarView view) {
			DateNavigatorContent content;
			if (contents.TryGetValue(view, out content))
				return content;
			return null;
		}
		protected internal List<DateTime> SpecialDateList { get; private set; }
		protected bool IsDateTimeChangedLocked { get { return lockerDateTime.IsLocked; } }
		protected bool IsSelectedDateListChangedLocked { get { return lockerSelectedDateList.IsLocked; } }
		protected bool IsSelectedDatesChangedLocked { get { return lockerSelectedDates.IsLocked; } }
		protected bool IsSyncSelectedDatesWithSelectedDateListLocked { get { return lockerSyncSelectedDatesWithSelectedDateList.IsLocked; } }
		protected internal IOptionsProviderService OptionsProviderService { get { return (optionsProviderServiceReference != null && optionsProviderServiceReference.IsAlive) ? (IOptionsProviderService)optionsProviderServiceReference.Target : null; } }
		public void CalculateVisibleDateRange(bool excludeInactiveDates, out DateTime startDate, out DateTime endDate) {
			if (ActiveContent != null)
				((IDateNavigatorContent)ActiveContent).GetDateRange(excludeInactiveDates, out startDate, out endDate);
			else
				startDate = endDate = new DateTime();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			foreach (DateNavigatorContent content in contents.Values) {
				((IDateNavigatorContent)content).CalendarButtonClick -= OnCalendarButtonClick;
				((IDateNavigatorContent)content).DateRangeChanged -= OnContentDateRangeChanged;
			}
			contents.Clear();
			foreach (DateNavigatorCalendarView state in DevExpress.Utils.EnumExtensions.GetValues(typeof(DateNavigatorCalendarView))) {
				DateNavigatorContent content = (DateNavigatorContent)GetTemplateChild("PART_Content" + state.ToString());
				((IDateNavigatorContent)content).CalendarButtonClick += OnCalendarButtonClick;
				((IDateNavigatorContent)content).DateRangeChanged += OnContentDateRangeChanged;
				contents.Add(state, content);
			}
			InitButton(ref arrowLeft, "PART_ArrowLeft", OnArrowLeftClick);
			InitButton(ref arrowRight, "PART_ArrowRight", OnArrowRightClick);
			InitButton(ref currentDateButton, "PART_CurrentDateButton", OnCurrentDateButtonClick);
			InitButton(ref todayButton, "PART_TodayButton", OnTodayButtonClick);
			ShowContent(CalendarView, CalendarView, false);
		}
		protected virtual object CreateDefaultStyleSettings(object value) {
			return value != null ? value : new DateNavigatorStyleSettings();
		}
		protected virtual void MaxSelectionLengthChanged(int oldValue, int newValue) {
			ValueEditingStrategy.UpdateSelection();
		}
		protected virtual void PropertyChangedSelectedDates(IList<DateTime> oldValue, IList<DateTime> newValue) {
			if (!IsSelectedDatesChangedLocked)
				NavigationService.CheckSelectedDates();
			UpdateCollectionChangedSubscription(oldValue, newValue, OnSelectedDatesChanged);
			ValueEditingStrategy.SelectedDatesChanged(newValue);
			NavigationCallbackService.Select(newValue);
			InvalidateSelection();
			RaiseSelectedDatesChanged();
		}
		protected virtual void DateTimeChanged(DateTime? oldValue, DateTime? newValue) {
			ValueEditingStrategy.DateTimeChanged(oldValue, newValue);
		}
		protected virtual void FocusedDateChanged(DateTime oldValue, DateTime newValue) {
			ValueEditingStrategy.FocusedDateChanged(oldValue, newValue);
			NavigationService.Move(newValue);
			NavigationCallbackService.Move(newValue);
			InvalidateFocusedDate();
		}
		protected virtual DoubleAnimation CreateDoubleAnimation(double from, double to) {
			DoubleAnimation da = new DoubleAnimation();
			da.From = from;
			da.To = to;
			da.Duration = new Duration(TimeSpan.FromMilliseconds(500));
			da.FillBehavior = FillBehavior.HoldEnd;
			return da;
		}
		protected virtual void MakeZoomInOutAnimation(DateNavigatorContent control, bool show) {
			if (control == null)
				return;
			DoubleAnimation widthAnimation;
			DoubleAnimation heightAnimation;
			DoubleAnimation opacityAnimation;
			if (show) {
				widthAnimation = CreateDoubleAnimation(0, 1);
				heightAnimation = CreateDoubleAnimation(0, 1);
				opacityAnimation = CreateDoubleAnimation(0, 1);
			} else {
				widthAnimation = CreateDoubleAnimation(1, 0);
				heightAnimation = CreateDoubleAnimation(1, 0);
				opacityAnimation = CreateDoubleAnimation(1, 0);
			}
			ScaleTransform scale = new ScaleTransform();
			control.RenderTransform = scale;
			scale.CenterX = control.ActualWidth / 2;
			scale.CenterY = control.ActualHeight / 2;
			BeginAnimation(control, widthAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
			BeginAnimation(control, heightAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
			BeginAnimation(control, opacityAnimation, "(UIElement.Opacity)");
		}
		protected internal virtual void ArrowClick(bool isRight) {
			if (ActiveContent == null)
				return;
			if (isRight)
				NavigationService.ScrollNextPage();
			else
				NavigationService.ScrollPreviousPage();
		}
		protected void LockSelectedDatesChanged(bool isLock) {
			if (isLock)
				lockerSelectedDates.Lock();
			else
				lockerSelectedDates.Unlock();
		}
		protected internal void SetSelectedDates(IList<DateTime> value) {
			LockSelectedDatesChanged(true);
			try {
				if (SelectedDates != null && value != null) {
					if (!SelectedDatesHelper.AreEquals(value, SelectedDates))
						SelectedDates = value;
				} else
					SelectedDates = value;
			} finally {
				LockSelectedDatesChanged(false);
			}
		}
		protected virtual string GetCurrentDateText() {
			DateTime startDate, endDate;
			if (ActiveContent != null) {
				CalculateVisibleDateRange(true, out startDate, out endDate);
				return DateNavigatorHelper.GetDateText(CalendarView, startDate, endDate);
			} else
				return "";
		}
		protected virtual void HandleSpecialDatesChanges() {
			UpdateSpecialDateList();
			UpdateMonthContentCalendarsSpecialDates();
		}
		protected internal virtual void OnArrowLeftClick(object sender, RoutedEventArgs e) {
			ArrowClick(false);
		}
		protected internal virtual void OnArrowRightClick(object sender, RoutedEventArgs e) {
			ArrowClick(true);
		}
		protected virtual void OnCalendarButtonClick(object sender, DateNavigatorCalendarButtonClickEventArgs e) {
			Focus();
			NavigationService.ProcessMouseDown(e.ButtonDate, e.ButtonKind);
		}
		protected virtual void OnContentDateRangeChanged(object sender, DateNavigatorContentDateRangeChangedEventArgs e) {
			if (((DateNavigatorContent)sender).State == CalendarView) {
				NavigationCallbackService.VisibleDateRangeChanged(e.IsScrolling);
				UpdateCurrentDateText();
			}
		}
		protected internal virtual void OnCurrentDateButtonClick(object sender, RoutedEventArgs e) {
			if (CalendarView != DateNavigatorCalendarView.YearsRange)
				NavigationService.ToView((DateNavigatorCalendarView)((((int)CalendarView) + 1) % contents.Count));
		}
		protected virtual void OnExactWorkdaysChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateCalendarsHolidays();
		}
		protected virtual void OnHolidaysChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateCalendarsHolidays();
		}	 
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (e.Handled) {
				return;
			}
			NavigationService.ProcessKeyDown(e);
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			base.OnPreviewKeyUp(e);
			if (e.Handled)
				return;
			NavigationService.ProcessKeyUp(e);
		}
		protected virtual void OnSpecialDatesChanged(object sender, NotifyCollectionChangedEventArgs e) {
			HandleSpecialDatesChanges();
		}
		internal bool SetActiveContentDateTime(DateTime dt, bool scrollIfValueInactive) {
			IDateNavigatorContent activeContent = ActiveContent as IDateNavigatorContent;
			if (activeContent == null)
				return false;
			DateTime startDate = activeContent.StartDate;
			ActiveContent.SetDateTime(dt, scrollIfValueInactive);
			return !object.Equals(startDate, activeContent.StartDate);
		}
		protected internal bool IsDateVisibleAndActive(DateTime date) {
			return ((IDateNavigatorContent)ActiveContent).GetCalendar(date) != null;
		}
		internal DateTime GetDateTimeForContent(DateNavigatorCalendarView state, DateTime dt) {
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
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			DateTime? buttonDate;
			DateNavigatorCalendarButtonKind buttonKind;
			HitTest(e, out buttonDate, out buttonKind);
			NavigationService.ProcessMouseMove(buttonDate, buttonKind);
		}
		protected void HitTest(MouseEventArgs e, out DateTime? buttonDate, out DateNavigatorCalendarButtonKind buttonKind) {
			UIElement element =
#if !SL
				InputHitTest(e.GetPosition(this)) as UIElement;
#else
				this.FindElement(e.GetPosition(null), (elem) => true);
#endif
			((IDateNavigatorContent)ActiveContent).HitTest(element, out buttonDate, out buttonKind);
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			if (e.OriginalSource == this) {
				DateTime? buttonDate;
				DateNavigatorCalendarButtonKind buttonKind;
				HitTest(e, out buttonDate, out buttonKind);
				NavigationService.ProcessMouseUp(buttonDate, buttonKind);
			}
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			Focus();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			ReleaseMouseCapture();
		}
		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewLostKeyboardFocus(e);
			SelectionManager.Post();
		}
		protected virtual void OnSelectedDatesChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (!IsSelectedDatesChangedLocked)
				NavigationService.CheckSelectedDates();
			ValueEditingStrategy.SelectedDatesChanged(SelectedDates);
			RaiseSelectedDatesChanged();
		}
		protected virtual void OnTodayButtonClick(object sender, RoutedEventArgs e) {
			if (CalendarView != DateNavigatorCalendarView.Month)
				NavigationService.ToView(DateNavigatorCalendarView.Month);
			NavigationService.Move(System.DateTime.Today);
			NavigationService.Select(System.DateTime.Today, true);
		}
		protected virtual void OnWorkdaysChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateCalendarsHolidays();
		}
		protected virtual void PropertyChangedCalendarStyle(Style oldValue) {
			foreach (IDateNavigatorContent content in contents.Values)
				if (content != null)
					content.UpdateCalendarsStyle();
		}
		protected virtual void PropertyChangedExactWorkdays(IList<DateTime> oldValue) {
			UpdateCollectionChangedSubscription(oldValue, ExactWorkdays, OnExactWorkdaysChanged);
			UpdateCalendarsHolidays();
		}
		protected virtual void PropertyChangedFirstDayOfWeek(DayOfWeek? oldValue) {
			UpdateActualFirstDayOfWeek();
		}
		protected void UpdateActualFirstDayOfWeek() {
			ActualFirstDayOfWeek = GetActualFirstDayOfWeek();
		}
		protected virtual DayOfWeek GetActualFirstDayOfWeek() {
			if (OptionsProviderService != null)
				return OptionsProviderService.FirstDayOfWeek;
			else
				return (FirstDayOfWeek != null) ? (DayOfWeek)FirstDayOfWeek : CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
		}
		protected virtual void PropertyChangedHolidays(IList<DateTime> oldValue) {
			UpdateCollectionChangedSubscription(oldValue, Holidays, OnHolidaysChanged);
			UpdateCalendarsHolidays();
		}
		protected virtual void IsMultiSelectChanged(bool oldValue, bool newValue) {
			StyleSettings.RegisterNavigationService();
		}
		protected virtual void PropertyChangedHighlightHolidays(bool oldValue) { }
		protected virtual void PropertyChangedHighlightSpecialDates(bool oldValue) {
			if (OptionsProviderService != null)
				OptionsProviderService.HighlightSpecialDates = HighlightSpecialDates;
		}
		protected virtual void PropertyChangedShowWeekNumbers() { }
		protected virtual void PropertyChangedSpecialDates(IList<System.DateTime> oldValue) {
			UpdateCollectionChangedSubscription(oldValue, SpecialDates, OnSpecialDatesChanged);
			HandleSpecialDatesChanges();
		}
		protected virtual void PropertyChangedWeekNumberRule(CalendarWeekRule? oldValue) { }
		protected virtual void PropertyChangedWorkdays(IList<DayOfWeek> oldValue) {
			UpdateCollectionChangedSubscription(oldValue, Workdays, OnWorkdaysChanged);
			UpdateCalendarsHolidays();
		}
		protected void UpdateCalendarsHolidays() {
			UpdateActualWorkdaysProperties();
			if (ActiveContent != null)
				((IDateNavigatorContent)ActiveContent).UpdateCalendarsHolidays();
		}
		protected virtual void StyleSettingsChanged(DateNavigatorStyleSettingsBase oldSettings, DateNavigatorStyleSettingsBase newSettings) {
			ILogicalOwner instance = (ILogicalOwner)this;
			if (oldSettings != null)
				instance.RemoveChild(oldSettings);
			if (newSettings != null) {
				instance.AddChild(newSettings);
				newSettings.Initialize(this);
			}
			UpdateOptionsProviderServiceChangesSubscription();
		}
		protected virtual void UpdateOptionsProviderServiceChangesSubscription() {
			if (OptionsProviderService != null)
				OptionsProviderService.OptionsChanged -= OnOptionsProviderServiceOptionsChanged;
			IOptionsProviderService service = (IOptionsProviderService)((IServiceProvider)this).GetService(typeof(IOptionsProviderService));
			if (service != null) {
				optionsProviderServiceReference = new WeakReference(service);
				service.OptionsChanged += OnOptionsProviderServiceOptionsChanged;
				UpdateOptionsProviderServiceProperties();
			} else
				optionsProviderServiceReference = null;
			UpdateActualFirstDayOfWeek();
			UpdateActualWorkdaysProperties();
		}
		protected virtual void UpdateOptionsProviderServiceProperties() {
			if (OptionsProviderService != null) {
				OptionsProviderService.HighlightSpecialDates = HighlightSpecialDates;
			}
		}
		protected virtual void UpdateActualWorkdaysProperties() {
			AreWorkdaysCollectionsInvalid = true;
		}
		protected virtual void OnOptionsProviderServiceOptionsChanged(object sender, EventArgs e) {
			UpdateActualFirstDayOfWeek();
			UpdateActualWorkdaysProperties();
			UpdateCalendarsHolidays();
		}
		protected DateTime ScrollDateTime(DateTime dt, bool isRight) {
			int factor = isRight ? 1 : -1;
			int calendarCount = ((IDateNavigatorContent)ActiveContent).CalendarCount;
			dt = new DateTime(dt.Year, dt.Month, 1);
			switch (CalendarView) {
				case DateNavigatorCalendarView.Month:
					int months = (dt.Year - 1) * 12 + dt.Month;
					if (months + calendarCount * factor < 1)
						return dt.AddMonths(-(months - 1));
					if (months + calendarCount * factor > 9998 * 12 + 12)
						return dt.AddMonths(9998 * 12 + 12 - months);
					return dt.AddMonths(calendarCount * factor);
				case DateNavigatorCalendarView.Year:
					return CreateDate(dt.Year + calendarCount * factor, dt.Month, dt.Day);
				case DateNavigatorCalendarView.Years:
					return CreateDate(dt.Year + calendarCount * 10 * factor, dt.Month, dt.Day);
				case DateNavigatorCalendarView.YearsRange:
					return CreateDate(dt.Year + calendarCount * 100 * factor, dt.Month, dt.Day);
			}
			throw new Exception();
		}
		protected virtual void RaiseSelectedDatesChanged() {
			if (SelectedDatesChanged != null)
				SelectedDatesChanged(this, EventArgs.Empty);
		}
		protected internal virtual void ShowContent(DateNavigatorCalendarView oldState, DateNavigatorCalendarView state, bool makeAnimation) {
			ValueEditingStrategy.ResetFocusedCellButtonFocusedState(oldState);
			if (ActiveContent == null)
				return;
			if (makeAnimation) {
				(ActiveContent as IDateNavigatorContent).VisibilityChanged();
				DateTime dt = (state > CalendarView) ? ActiveContent.DateTime : FocusedDate;
				DateNavigatorContent content = GetContent(state);
				if (content == null)
					return;
				content.SetDateTime(GetDateTimeForContent(state, dt), true);
				MakeZoomInOutAnimation(GetContent(oldState), false);
				GetContent(oldState).IsHitTestVisible = false;
				GetContent(state).IsHitTestVisible = true;
				MakeZoomInOutAnimation(GetContent(state), true);
				if (CalendarView == DateNavigatorCalendarView.Month)
					UpdateMonthContentCalendarsSelectedDates();
			} else {
				if (ActiveContent != null) {
					ActiveContent.Opacity = 1;
					ActiveContent.IsHitTestVisible = true;
					NavigationService.BringToView();
				}
			}
			ActiveContent.FocusedDate = FocusedDate;
			ValueEditingStrategy.SetFocusedCellButtonFocusedState();
			UpdateCurrentDateText();
			NavigationCallbackService.VisibleDateRangeChanged(true);
		}
		protected void UpdateCurrentDateText() {
			CurrentDateText = GetCurrentDateText();
		}
		protected virtual void UpdateMonthContentCalendarsSelectedDates() {
			if (contents.ContainsKey(DateNavigatorCalendarView.Month))
				((IDateNavigatorContent)contents[DateNavigatorCalendarView.Month]).UpdateCalendarsSelectedDates();
		}
		protected virtual void UpdateMonthContentCalendarsSpecialDates() {
			if (contents.ContainsKey(DateNavigatorCalendarView.Month))
				((IDateNavigatorContent)contents[DateNavigatorCalendarView.Month]).UpdateCalendarsSpecialDates();
		}
		protected void UpdateSpecialDateList() {
			SpecialDateList = new List<DateTime>();
			if (SpecialDates != null)
				foreach (DateTime date in SpecialDates)
					SpecialDateList.Add(date.Date);
		}
		protected virtual void CalendarViewChanged(DateNavigatorCalendarView oldState, DateNavigatorCalendarView newState) {
			NavigationService.ToView(newState);
			ShowContent(oldState, newState, true);
			NavigationCallbackService.ChangeView(newState);
		}
		void BeginAnimation(DateNavigatorContent value, DoubleAnimation element, string strPropertyPath) {
			Storyboard.SetTarget(element, value);
			Storyboard.SetTargetProperty(element, new PropertyPath(strPropertyPath));
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(element);
			storyboard.Begin();
		}
		DateTime CreateDate(int year, int month, int day) {
			if (year < 1)
				return new DateTime(1, 1, 1);
			if (year > 9999)
				return new DateTime(9999, 12, 31);
			return new DateTime(year, month, day);
		}
		void GetDateRange(DateTime dt1, DateTime dt2, out DateTime rangeStart, out DateTime rangeFinish) {
			if (dt1 <= dt2) {
				rangeStart = dt1;
				rangeFinish = dt2;
			} else {
				rangeStart = dt2;
				rangeFinish = dt1;
			}
		}
		void InitButton(ref ButtonBase button, string templatePartName, RoutedEventHandler clickHandler) {
			if (button != null)
				button.Click -= clickHandler;
			button = GetTemplateChild(templatePartName) as ButtonBase;
			if (button != null)
				button.Click += clickHandler;
		}
		void UpdateCollectionChangedSubscription(object oldValue, object newValue, NotifyCollectionChangedEventHandler collectionChangedHandler) {
			if (oldValue is INotifyCollectionChanged)
				(oldValue as INotifyCollectionChanged).CollectionChanged -= collectionChangedHandler;
			if (newValue is INotifyCollectionChanged)
				(newValue as INotifyCollectionChanged).CollectionChanged += collectionChangedHandler;
		}
		protected internal virtual void InvalidateFocusedDate() {
			IDateNavigatorContent activeContent = ActiveContent as IDateNavigatorContent;
			if (activeContent == null)
				return;
			activeContent.FocusedDate = ValueEditingService.FocusedDate;
		}
		protected internal virtual void InvalidateSelection() {
			IDateNavigatorContent activeContent = ActiveContent as IDateNavigatorContent;
			if (activeContent == null)
				return;
			activeContent.UpdateCalendarsSelectedDates();
		}
		#region IDateNavigatorContentContainer Members
		DateNavigatorContent IDateNavigatorContentContainer.GetContent(DateNavigatorCalendarView state) {
			return GetContent(state);
		}
		#endregion
		#region IServiceContainer Members
		DateNavigatorStyleSettingsBase ServiceContainer { get { return StyleSettings != null ? (DateNavigatorStyleSettingsBase)StyleSettings : (DateNavigatorStyleSettingsBase)new DateNavigatorStyleSettings(); } }
		object IServiceProvider.GetService(Type serviceType) {
			return ((IServiceProvider)ServiceContainer).GetService(serviceType);
		}
		ValueEditingStrategy ValueEditingStrategy { get { return (ValueEditingStrategy)ServiceContainer.GetService<IValueEditingService>(); } }
		internal IValueEditingService ValueEditingService { get { return ServiceContainer.GetService<IValueEditingService>(); } }
		internal INavigationService NavigationService { get { return ServiceContainer.GetService<INavigationService>(); } }
		internal INavigationCallbackService NavigationCallbackService { get { return ServiceContainer.GetService<INavigationCallbackService>(); } }
		internal IValueValidatingService ValueValidatingService { get { return ServiceContainer.GetService<IValueValidatingService>(); } }
		#endregion
		#region ILogicalOwner Members
		List<object> logicalChildren = new List<object>();
#if SL
		protected override bool AddLogicalChildrenToResources { get { return true; } }
#endif
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return logicalChildren.GetEnumerator(); }
		}
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.DateNavigator.Internal {
	public static class DateNavigatorHelper {
		public static DateNavigatorCalendarCellButton GetCalendarCellButton(DateNavigator navigator, DateTime dt) {
			if (navigator.ActiveContent == null)
				return null;
			DateNavigatorCalendar calendar = ((IDateNavigatorContent)navigator.ActiveContent).GetCalendar(dt, false);
			return (calendar != null) ? calendar.GetCellButton(dt) : null;
		}
		public static string GetDateText(DateNavigatorCalendarView state, DateTime dt) {
			if (state == DateNavigatorCalendarView.Month)
				return dt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern);
			if (state == DateNavigatorCalendarView.Year)
				return dt.ToString("yyyy", CultureInfo.CurrentCulture);
			if (state == DateNavigatorCalendarView.Years)
				return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}", new DateTime(Math.Max((dt.Year / 10) * 10, 1), 1, 1), new DateTime((dt.Year / 10) * 10 + 9, 1, 1));
			return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}-{1:yyyy}", new DateTime(Math.Max((dt.Year / 100) * 100, 1), 1, 1), new DateTime((dt.Year / 100) * 100 + 99, 1, 1));
		}
		public static string GetDateText(DateNavigatorCalendarView state, DateTime startDate, DateTime endDate) {
			if (SelectedDatesHelper.AreEquals(startDate, endDate, state)) {
				if (state == DateNavigatorCalendarView.Month)
					return startDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern);
				else
					return string.Format(CultureInfo.CurrentCulture, "{0:yyyy}", startDate);
			} else {
				if (state == DateNavigatorCalendarView.Month)
					return startDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern) + " - " + endDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern);
				else
					return string.Format(CultureInfo.CurrentCulture, "{0:yyyy} - {1:yyyy}", startDate, endDate);
			}
		}
		public static IList<DateTime> GetSelectedDateList(DateNavigator navigator) {
			return navigator.ValueEditingService.SelectedDates;
		}
		public static List<DateTime> GetSpecialDateList(DateNavigator navigator) {
			return navigator.SpecialDateList;
		}
	}
	public class DateNavigatorWorkdayCalculator : IDateCalculationService {
		DateNavigator navigator;
		public DateNavigatorWorkdayCalculator(DateNavigator navigator) {
			this.navigator = navigator;
		}
		protected IList<DateTime> ActualExactWorkdays { get; set; }
		protected IList<DateTime> ActualHolidays { get; set; }
		protected IList<DayOfWeek> ActualWorkdays { get; set; }
		public bool IsWorkday(DateTime date) {
			UpdateActualWorkdaysCollections();
			if (ActualExactWorkdays != null && SelectedDatesHelper.Contains(ActualExactWorkdays, date))
				return true;
			if (ActualHolidays != null && SelectedDatesHelper.Contains(ActualHolidays, date))
				return false;
			if (ActualWorkdays != null)
				return ActualWorkdays.Contains(date.DayOfWeek);
			return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
		}
		protected virtual void UpdateActualWorkdaysCollections() {
			if (!navigator.AreWorkdaysCollectionsInvalid) return;
			if (navigator.OptionsProviderService != null) {
				ActualExactWorkdays = navigator.OptionsProviderService.ExactWorkdays;
				ActualHolidays = navigator.OptionsProviderService.Holidays;
				ActualWorkdays = navigator.OptionsProviderService.Workdays;
			} else {
				ActualExactWorkdays = navigator.ExactWorkdays;
				ActualHolidays = navigator.Holidays;
				ActualWorkdays = navigator.Workdays;
			}
			navigator.AreWorkdaysCollectionsInvalid = false;
		}
	}
}
