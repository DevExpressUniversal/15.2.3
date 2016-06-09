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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Editors.DateNavigator;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Schedule;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Native {
	public class SchedulerNavigationCallbackService : INavigationCallbackService {
		readonly SchedulerDateNavigatorController controller;
		DateNavigatorCalendarView lastCalendarViewState;
		public SchedulerNavigationCallbackService(SchedulerDateNavigatorController controller) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
		}
		public SchedulerDateNavigatorController Controller { get { return controller; } }
		#region INavigationCallbackService Members
		public void Move(DateTime dateTime) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->Move {0}", dateTime.ToShortDateString());
		}
		public void Select(IList<DateTime> selectedDates) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerNavigationCallbackService.Select start={0} count={1}", (selectedDates != null && selectedDates.Count > 0) ? selectedDates[0].ToShortDateString() : "null", (selectedDates != null) ? selectedDates.Count : 0);
			SyncSchedulerWithNavigator();
		}
		public void Scroll(TimeSpan offset) {
		}		
		public void ChangeView(DateNavigatorCalendarView state) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerNavigationCallbackService.ChangeView: state={0}, lastState={1}", state, this.lastCalendarViewState);
			this.lastCalendarViewState = state;
			if (state == DateNavigatorCalendarView.Month) {
				int deltaYear = this.controller.Owner.StartDate.Year - Controller.InnerControl.Start.Year;
				int deltaMonth = this.controller.Owner.StartDate.Month - Controller.InnerControl.Start.Month;
				this.controller.ShiftVisibleIntervals(deltaYear, deltaMonth, 0);
				this.controller.UpdateSpecialDates();
			}
		}
		public void VisibleDateRangeChanged(bool isScrolling) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerNavigationCallbackService.VisibleDateRangeChanged: isScrolling={0}", isScrolling);
			if (this.lastCalendarViewState != DateNavigatorCalendarView.Month)
				return;
			if (isScrolling && !Controller.IsNavigatorVisibleIntervalChanged())
				return;
			Controller.NavigatorVisibleDateRangeChanged(isScrolling);			
		}
		#endregion
		protected virtual void SyncSchedulerWithNavigator() {
			Controller.SyncSchedulerSelection();
		}
		protected virtual void ShiftSchedulerIntervals(TimeSpan offset) {
			Controller.ShiftSchedulerIntervals(offset);
		}
	}
	public class SchedulerValueValidatingService : IValueValidatingService {
		readonly SchedulerDateNavigatorController controller;
		readonly DateNavigator navigator;
		public SchedulerValueValidatingService(SchedulerDateNavigatorController controller, DateNavigator navigator, IDateValidationAdditionalValueProvider additionalValueProvider) {
			Guard.ArgumentNotNull(controller, "controller");
			Guard.ArgumentNotNull(navigator, "navigator");
			Guard.ArgumentNotNull(navigator, "additionalValueProvider");
			this.controller = controller;
			this.navigator = navigator;
			AdditionalValueProvider = additionalValueProvider;
		}
		public SchedulerDateNavigatorController Controller { get { return controller; } }
		public DateNavigator Navigator { get { return navigator; } }
		InnerSchedulerControl InnerControl { get { return this.controller.InnerControl; } }
		public IDateValidationAdditionalValueProvider AdditionalValueProvider { get; private set; }
		SchedulerDateNavigatorStyleSettings StyleSettings { get { return Navigator.StyleSettings as SchedulerDateNavigatorStyleSettings; } }
		int MaxSelectedConsecutiveWeeks { get { return (StyleSettings != null) ? StyleSettings.MaxSelectedConsecutiveWeeks : -1; } }
		int MaxSelectedNonConsecutiveDates { get { return (StyleSettings != null) ? StyleSettings.MaxSelectedNonConsecutiveDates : -1; } }
		public IList<DateTime> Validate(IList<DateTime> selectedDates) {
			int maxSelectionLength = Navigator.MaxSelectionLength;
			if (maxSelectionLength > 0)
				selectedDates = DateNavigationUtils.CreateDateList(selectedDates, maxSelectionLength);
			IList<DateTime> result = ValidateCore(selectedDates);
			if (maxSelectionLength < 0)
				return result;
			return DateNavigationUtils.CreateDateList(result, maxSelectionLength);
		}
		IList<DateTime> ValidateCore(IList<DateTime> selectedDates) {
			if (Controller.IsNavigatorChanges(DateNavigatorChangeType.SuppressValidation))
				return selectedDates;
			if (!Controller.RoundSelectionToEntireWeek || InnerControl == null)
				return selectedDates;
			DayIntervalCollection selectedDays = DateNavigationUtils.ConvertToDays(selectedDates);
			SchedulerViewType newViewType = CalculateNewViewType(selectedDays);
			InnerSchedulerViewBase newActiveView = InnerControl.Views.GetInnerView(newViewType);
			TimeIntervalCollection newVisibleInterval = newActiveView.CreateValidIntervals(selectedDays);
			return GetRestrictedDateList(newVisibleInterval, newViewType);
		}
		SchedulerViewType CalculateNewViewType(DayIntervalCollection selectedDays) {
			if (AdditionalValueProvider.IsLastSelectionAction && CanSelectLikeWorkWeekView(selectedDays))
				return SchedulerViewType.WorkWeek;
			if (CanSelectLikeDayView(selectedDays))
				return SchedulerViewType.Day;
			if (CanSelectLikeWeekView(selectedDays))
				return (IsFullWeekEnabled()) ? SchedulerViewType.FullWeek : SchedulerViewType.Week;
			if (CanSelectLikeMonthView(selectedDays))
				return SchedulerViewType.Month;
			return InnerControl.ActiveViewType;
		}
		bool IsFullWeekEnabled() {
			if (Controller == null)
				return false;
			if (Controller.SchedulerControl == null)
				return false;
			if (Controller.SchedulerControl.FullWeekView == null)
				return false;
			return Controller.SchedulerControl.FullWeekView.Enabled;
		}
		IList<DateTime> GetRestrictedDateList(TimeIntervalCollection visibleInterval, SchedulerViewType newViewType) {
			IList<DateTime> result = new List<DateTime>();
			if (newViewType == SchedulerViewType.Day) {
				result = DateNavigationUtils.ConvertToDateList(visibleInterval);
				if (MaxSelectedNonConsecutiveDates < 0)
					return result;
				IList<DateTime> lastSelectedDates = AdditionalValueProvider.LastSelectedDates;
				int count = Math.Min(lastSelectedDates.Count, result.Count - MaxSelectedNonConsecutiveDates);
				for (int i = 0; i < count; i++) {
					if (result.Count < 1)
						break;
					result.Remove(lastSelectedDates[i]);
				}
				return result;
			}
			if (newViewType == SchedulerViewType.Month) {
				if (MaxSelectedConsecutiveWeeks < 0)
					return DateNavigationUtils.ConvertToDateList(visibleInterval);
				int weekCount = Math.Min(MaxSelectedConsecutiveWeeks, DateTimeHelper.CalcWeekCount(visibleInterval.Duration));
				WeekIntervalCollection weeks = new WeekIntervalCollection();
				weeks.CompressWeekend = InnerControl.MonthView.CompressWeekend;
				weeks.FirstDayOfWeek = InnerControl.MonthView.GetVisibleIntervals().Start.DayOfWeek;
				weeks.Add(new TimeInterval(visibleInterval.Start, TimeSpan.FromDays(7 * weekCount)));
				DayIntervalCollection actualSelectedDays = DateNavigationUtils.ConvertToDays(weeks);
				return DateNavigationUtils.ConvertToDateList(actualSelectedDays);
			}
			return DateNavigationUtils.ConvertToDateList(visibleInterval);
		}
		SchedulerViewType CalculateInitialViewType() {
			SchedulerViewRepositoryBase views = Controller.InnerControl.Views;
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				InnerSchedulerViewBase view = views.GetInnerView(i);
				if (view.Enabled)
					return view.Type;
			}
			return SchedulerViewType.Day;
		}
		bool CanSelectLikeWorkWeekView(DayIntervalCollection selectedDays) {
			if (!InnerControl.WorkWeekView.Enabled)
				return false;
			return (InnerControl.ActiveViewType == SchedulerViewType.WorkWeek) && selectedDays.Count == 1;
		}
		bool CanSelectLikeDayView(DayIntervalCollection selectedDays) {
			if (!InnerControl.DayView.Enabled)
				return false;
			return !selectedDays.IsContinuous() || selectedDays.Count < 7;
		}
		bool CanSelectLikeMonthView(DayIntervalCollection selectedDays) {
			if (!InnerControl.MonthView.Enabled)
				return false;
			return true;
		}
		bool CanSelectLikeWeekView(DayIntervalCollection selectedDays) {
			if (!InnerControl.WeekView.Enabled)
				return false;
			int selectedDayCount = selectedDays.Count;
			if (selectedDayCount != 7 || !selectedDays.IsContinuous())
				return false;
			DayOfWeek viewFirstDayOfWeek = InnerControl.WeekView.GetVisibleIntervals().Start.DayOfWeek;
			if (selectedDayCount > 0 && selectedDays[0].Start.DayOfWeek == viewFirstDayOfWeek)
				return true;
			return false;
		}
		protected virtual IList<DateTime> RoundToWeeks(DayIntervalCollection days) {
			WeekIntervalCollection weeks = new WeekIntervalCollection();
			weeks.FirstDayOfWeek = Controller.FirstDayOfWeek;
			weeks.Add(days.Interval);
			return DateNavigationUtils.CreateDateList(weeks.Start, weeks.End);
		}
	}
	public class SchedulerOptionsProviderService : IOptionsProviderService {
		readonly SchedulerDateNavigatorController controller;
		WorkDaysCollectionSplitter workDaysSplitter;
		public SchedulerOptionsProviderService(SchedulerDateNavigatorController controller) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
			workDaysSplitter = new WorkDaysCollectionSplitter();
		}
		public SchedulerDateNavigatorController Controller { get { return controller; } }
		protected InnerSchedulerControl Control { get { return Controller.InnerControl; } }
		public WorkDaysCollectionSplitter WorkDaysSplitter { get { return workDaysSplitter; } }
		#region IOptionsProviderService Members
		DayOfWeek IOptionsProviderService.FirstDayOfWeek { get { return Controller.FirstDayOfWeek; } }
		bool IOptionsProviderService.HighlightSpecialDates { set { Controller.BoldAppointmentDates = value; } }
		IList<DateTime> IOptionsProviderService.ExactWorkdays { get { return GetExactWorkdaysCollection(); } }
		IList<DateTime> IOptionsProviderService.Holidays { get { return GetHolidaysCollection(); } }
		bool IOptionsProviderService.ScrollSelection { get { return false; } }
		IList<DayOfWeek> IOptionsProviderService.Workdays { get { return GetWorkDaysCollection(); } }
		void IOptionsProviderService.Start() { StartCore(); }
		void IOptionsProviderService.Stop() { StopCore(); }
		EventHandler optionsChanged;
		event EventHandler IOptionsProviderService.OptionsChanged {
			add { optionsChanged += value; }
			remove { optionsChanged -= value; }
		}
		protected void RaiseOnOptionChanged() {
			if (optionsChanged != null)
				optionsChanged(this, EventArgs.Empty);
		}
		#endregion
		protected virtual void StartCore() {
			XtraSchedulerDebug.Assert(Control != null);
			UpdateWorkDaysCollections();
			SubscribeControlEvents();
			RaiseOnOptionChanged();
		}
		protected virtual void StopCore() {
			UnsubscribeControlEvents();
		}
		protected virtual ObservableCollection<DayOfWeek> GetWorkDaysCollection() {
			if (Control == null) {
				DayOfWeek[] defaultDaysOfWeek = DateTimeHelper.ToDayOfWeeks(WeekDays.WorkDays);
				return new ObservableCollection<DayOfWeek>(defaultDaysOfWeek);
			}
			WeekDays weekDays = Control.WorkDays.GetWeekDays();
			DayOfWeek[] daysOfWeek = DateTimeHelper.ToDayOfWeeks(weekDays);
			return new ObservableCollection<DayOfWeek>(daysOfWeek);
		}
		protected virtual ObservableCollection<DateTime> GetExactWorkdaysCollection() {
			return WrapCollectionToObservableCollection<DateTime>(WorkDaysSplitter.ExactWorkDays);
		}
		ObservableCollection<T> WrapCollectionToObservableCollection<T>(List<T> list) {
#if SL            
			IList<T> slDTWorkaround = list;
			return new ObservableCollection<T>(slDTWorkaround);
#else
			return new ObservableCollection<T>(list);
#endif
		}
		protected virtual ObservableCollection<DateTime> GetHolidaysCollection() {
			return WrapCollectionToObservableCollection<DateTime>(WorkDaysSplitter.Holidays);
		}
		protected internal virtual void SubscribeControlEvents() {
			if (Control == null)
				return;
			if (Control.OptionsView != null)
				Control.OptionsView.Changed += OnOptionsViewChanged;
			if (Control.WorkDaysCollectionListener != null)
				Control.WorkDaysCollectionListener.Changed += new EventHandler(OnWorkDaysCollectionChanged);
		}
		void OnWorkDaysCollectionChanged(object sender, EventArgs e) {
			UpdateWorkDaysCollections();
			RaiseOnOptionChanged();
		}
		protected virtual void UpdateWorkDaysCollections() {
			WorkDaysSplitter.Split(Control.WorkDays);
		}
		protected internal virtual void UnsubscribeControlEvents() {
			if (Control == null)
				return;
			if (Control.OptionsView != null)
				Control.OptionsView.Changed -= OnOptionsViewChanged;
			if (Control.WorkDaysCollectionListener != null)
				Control.WorkDaysCollectionListener.Changed -= new EventHandler(OnWorkDaysCollectionChanged);
		}
		void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "FirstDayOfWeek")
				RaiseOnOptionChanged();
		}
	}
	public interface IDateValidationAdditionalValueProvider {
		IList<DateTime> LastSelectedDates { get; }
		bool IsLastSelectionAction { get; }
	}
	public class SchedulerMultipleSelectionNavigationStrategy : MultipleSelectionNavigationStrategy, IDateValidationAdditionalValueProvider {
		public SchedulerMultipleSelectionNavigationStrategy(DateNavigator navigator)
			: base(navigator) {
			LastSelectedDates = new List<DateTime>();
			IsLastSelectionAction = false;
		}
		public List<DateTime> LastSelectedDates { get; private set; }
		public bool IsLastSelectionAction { get; private set; }
		public override bool Select(DateTime dateTime, bool clearSelection) {
			return base.Select(dateTime, clearSelection);
		}
		public override bool Select(IList<DateTime> selectedDates, bool clearSelection) {
			LastSelectedDates.Clear();
			LastSelectedDates.AddRange(selectedDates);
			bool result = base.Select(selectedDates, clearSelection);
			LastSelectedDates.Clear();
			return result;
		}
		#region IDateValidationValueProvider
		IList<DateTime> IDateValidationAdditionalValueProvider.LastSelectedDates { get { return LastSelectedDates; } }
		bool IDateValidationAdditionalValueProvider.IsLastSelectionAction { get { return IsLastSelectionAction; } }
		#endregion
		protected override void ProcessMouseUpCore(DateTime? date) {
			IsLastSelectionAction = true;
			try {
				base.ProcessMouseUpCore(date);
			} finally {
				IsLastSelectionAction = false;
			}
		}
	}
	public class WorkDaysCollectionSplitter {
		List<DateTime> holidays = new List<DateTime>();
		List<DateTime> exactWorkDays = new List<DateTime>();
		public List<DateTime> Holidays { get { return holidays; } }
		public List<DateTime> ExactWorkDays { get { return exactWorkDays; } }
		private void ClearLists() {
			holidays = new List<DateTime>();
			exactWorkDays = new List<DateTime>();
		}
		public void Split(WorkDaysCollection collection) {
			ClearLists();
			if (collection == null)
				return;
			collection.ForEach(ArrangeWorkDay);
		}
		protected void ArrangeWorkDay(WorkDay workDay) {
			KnownDateDay date = workDay as KnownDateDay;
			if (date == null)
				return;
			switch (date.Type) {
				case WorkDayType.Holiday:
					Holidays.Add(date.Date);
					break;
				case WorkDayType.ExactWorkDay:
					ExactWorkDays.Add(date.Date);
					break;
				case WorkDayType.WeekDay:
					Exceptions.ThrowInternalException();
					break;
			}
		}
	}
}
