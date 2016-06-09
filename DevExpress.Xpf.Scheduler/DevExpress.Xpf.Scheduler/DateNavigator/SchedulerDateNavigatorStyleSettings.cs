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
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Editors.DateNavigator;
using System.Collections.Generic;
using DevExpress.XtraScheduler;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler {
	public interface ISchedulerDateNavigatorControllerOwner {
		bool IsReady { get; }
		bool HasSelection { get; }
		bool RoundSelectionToEntireWeek { get; }
		INavigationService Navigation { get; }
	}
	public class SchedulerDateNavigatorStyleSettings : DateNavigatorStyleSettings, IDateNavigatorControllerOwner, ISchedulerDateNavigatorControllerOwner {
		readonly DateNavigatorControllerBase controller;
		IValueEditingService valueEditing;
		IValueValidatingService valueValidating;
		INavigationService navigation;
		INavigationCallbackService navigationCallback;
		IOptionsProviderService optionsProvider;
		XtraScheduler.DayIntervalCollection deferredSelectedDays = null;
		static SchedulerDateNavigatorStyleSettings() {
		}
		public SchedulerDateNavigatorStyleSettings() {
			this.controller = CreateDateNavigatorController();
			Initialized += OnInitialized;
		}
		#region Properties
		protected internal SchedulerDateNavigatorController Controller { get { return (SchedulerDateNavigatorController)controller; } }
		#region SchedulerControl
#if SL
		[System.ComponentModel.TypeConverter(typeof(DevExpress.Xpf.Core.WPFCompatibility.ObjectConverter))]
#endif
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = CreateSchedulerControlProperty();
		static DependencyProperty CreateSchedulerControlProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDateNavigatorStyleSettings, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue), null);
		}
		private void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			UnsubscribeControllerEvents();
			if (oldValue != null)
				OptionsProvider.Stop();
			Controller.SchedulerControl = newValue != null ? newValue : null;
			if (newValue != null)
				OptionsProvider.Start();
			UpdateSpecialDates();
			SubscribeControllerEvents();
		}
		#endregion
		#region RoundSelectionToEntireWeek
		public bool RoundSelectionToEntireWeek {
			get { return (bool)GetValue(RoundSelectionToEntireWeekProperty); }
			set { SetValue(RoundSelectionToEntireWeekProperty, value); }
		}
		public static readonly DependencyProperty RoundSelectionToEntireWeekProperty = CreateRoundSelectionToEntireWeekProperty();
		static DependencyProperty CreateRoundSelectionToEntireWeekProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDateNavigatorStyleSettings, bool>("RoundSelectionToEntireWeek", true, (d, e) => d.OnRoundSelectionToEntireWeekChanged(e.OldValue, e.NewValue), null);
		}
		private void OnRoundSelectionToEntireWeekChanged(bool oldValue, bool newValue) {
			if (oldValue != newValue) {
				RefreshNavigator();
			}
		}
		#endregion
		#region MaxSelectedConsecutiveWeeks
		public int MaxSelectedConsecutiveWeeks {
			get { return (int)GetValue(MaxSelectedConsecutiveWeeksProperty); }
			set { SetValue(MaxSelectedConsecutiveWeeksProperty, value); }
		}
		public static readonly DependencyProperty MaxSelectedConsecutiveWeeksProperty = CreateMaxSelectedConsecutiveWeeksProperty();
		static DependencyProperty CreateMaxSelectedConsecutiveWeeksProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDateNavigatorStyleSettings, int>("MaxSelectedConsecutiveWeeks", 6, (d, e) => d.OnMaxSelectedConsecutiveWeeksChanged(e.OldValue, e.NewValue), null);
		}
		void OnMaxSelectedConsecutiveWeeksChanged(int oldValue, int newValue) {
			if (oldValue != newValue) {
				RefreshNavigator();
			}
		}
		#endregion
		#region MaxSelectedNonConsecutiveDates
		public int MaxSelectedNonConsecutiveDates {
			get { return (int)GetValue(MaxSelectedNonConsecutiveDatesProperty); }
			set { SetValue(MaxSelectedNonConsecutiveDatesProperty, value); }
		}
		public static readonly DependencyProperty MaxSelectedNonConsecutiveDatesProperty = CreateMaxSelectedNonConsecutiveDatesProperty();
		static DependencyProperty CreateMaxSelectedNonConsecutiveDatesProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDateNavigatorStyleSettings, int>("MaxSelectedNonConsecutiveDates", 14, (d, e) => d.OnMaxSelectedNonConsecutiveDatesChanged(e.OldValue, e.NewValue), null);
		}
		void OnMaxSelectedNonConsecutiveDatesChanged(int oldValue, int newValue) {
			if (oldValue != newValue) {
				RefreshNavigator();
			}
		}
		#endregion
		#endregion
		#region Events
		#region CustomizeSpecialDates
#if !SL
		public event EventHandler<CustomizeSpecialDatesEventArgs> CustomizeSpecialDates;
		public IList<DateTime> RaiseCustomizeSpecialDates(IList<DateTime> specialDates) {
			if (CustomizeSpecialDates == null)
				return specialDates;
			CustomizeSpecialDatesEventArgs ea = new CustomizeSpecialDatesEventArgs(specialDates);
			CustomizeSpecialDates(this, ea);
			return ea.SpecialDates ?? specialDates;
		}
#endif
		#endregion
		#endregion
		void RefreshNavigator() {
			if (!GetIsReady())
				return;
			Navigation.Select(ValueEditing.SelectedDates, true);
		}
		protected virtual void UnsubscribeControllerEvents() {
			Controller.Changed -= OnControllerChanged;
			Controller.DateNavigatorQueryActiveViewType -= OnControllerDateNavigatorQueryActiveViewType;
		}
		protected virtual void SubscribeControllerEvents() {
			Controller.Changed += OnControllerChanged;
			Controller.DateNavigatorQueryActiveViewType += OnControllerDateNavigatorQueryActiveViewType;
		}
		void OnControllerDateNavigatorQueryActiveViewType(object sender, DateNavigatorQueryActiveViewTypeEventArgs e) {
			if (SchedulerControl == null)
				return;
			e.NewViewType = SchedulerControl.RaiseDateNavigatorQueryActiveViewType(e.OldViewType, e.NewViewType, e.SelectedDays);
		}
		#region Services
		protected internal IValueEditingService ValueEditing { get { return valueEditing; } }
		protected internal IValueValidatingService ValueValidating { get { return valueValidating; } }
		protected internal INavigationService Navigation { get { return navigation; } }
		INavigationService ISchedulerDateNavigatorControllerOwner.Navigation { get { return Navigation; } }
		protected internal INavigationCallbackService NavigationCallback { get { return navigationCallback; } }
		protected internal IOptionsProviderService OptionsProvider { get { return optionsProvider; } }
		#endregion
		#region IServiceProvider Members
		protected internal T GetService<T>() {
			return (T)this.GetService(typeof(T));
		}
		protected internal object GetService(Type serviceType) {
			return ServiceContainer != null ? ServiceContainer.GetService(serviceType) : null;
		}
		#endregion
		#region IDateNavigatorControllerOwner Members
		DateTime IDateNavigatorControllerOwner.EndDate {
			get { return ValueEditing.EndDate; }
		}
		DateTime IDateNavigatorControllerOwner.GetFirstDayOfMonth(DateTime date) {
			return new DateTime(date.Year, date.Month, 1, date.Hour, date.Minute, date.Second, date.Millisecond);
		}
		XtraScheduler.DayIntervalCollection IDateNavigatorControllerOwner.GetSelection() {
			return DateNavigationUtils.ConvertToIntervalCollection(ValueEditing.SelectedDates);
		}		
		void OnInitialized(object sender, EventArgs e) {
			if (this.deferredSelectedDays == null)
				return;
			if (this.deferredSelectedDays.Count > 0)
				Navigator.FocusedDate = this.deferredSelectedDays.Start;
			Controller.Lock();
			try {
				SetSelectionCore(this.deferredSelectedDays);
				Controller.SyncSchedulerSelection();			
			} finally {
				Controller.Unlock();
			}
			this.deferredSelectedDays = null;
		}
		void IDateNavigatorControllerOwner.SetSelection(XtraScheduler.DayIntervalCollection days) {
			if (!GetIsReady()) {
				this.deferredSelectedDays = days;
				return;
			}
			SetSelectionCore(days);
		}
		void SetSelectionCore(XtraScheduler.DayIntervalCollection days) {
			if (Controller.IsNavigatorChanges(DateNavigatorChangeType.SyncSelection)) {
				if (IsFollowUpSelectionChanges())
					ReplaceSelectedDates(DateNavigationUtils.ConvertFromIntervalCollection(days));
			} else {
				ReplaceSelectedDates(DateNavigationUtils.ConvertFromIntervalCollection(days));
			}
		}
		DateTime IDateNavigatorControllerOwner.SelectionEnd {
			get { return DateNavigationUtils.GetLastDate(ValueEditing.SelectedDates); }
		}
		DateTime IDateNavigatorControllerOwner.SelectionStart {
			get { return DateNavigationUtils.GetFirstDate(ValueEditing.SelectedDates); }
		}
		DateTime IDateNavigatorControllerOwner.StartDate {
			get {
				return ValueEditing.StartDate;
			}
			set {
				if (!GetIsReady())
					return;
				if (!Controller.IsNavigatorChanges(DateNavigatorChangeType.SuppressScroll))
					Navigation.ScrollTo(value, true);
			}
		}
		#endregion
		#region ISchedulerDateNavigatorControllerOwner
		bool ISchedulerDateNavigatorControllerOwner.IsReady { get { return GetIsReady(); } }
		bool ISchedulerDateNavigatorControllerOwner.HasSelection { get { return GetIsReady() && ValueEditing.SelectedDates != null && ValueEditing.SelectedDates.Count > 0; } }
		#endregion
		protected override void Initialize(DateNavigator navigator) {
			base.Initialize(navigator);
			PrepareServices();
		}
		protected virtual bool GetIsReady() {
#if !SL
			if (!IsInitialized)
				return false;
#endif
			return ValueEditing != null && ValueValidating != null && Navigation != null && NavigationCallback != null;
		}
		protected virtual void PrepareServices() {
			this.valueEditing = GetService<IValueEditingService>();
			this.navigation = GetService<INavigationService>();
			SchedulerMultipleSelectionNavigationStrategy navigationSelectionService = new SchedulerMultipleSelectionNavigationStrategy(Navigator);
			RegisterService(typeof(INavigationService), navigationSelectionService);
			RegisterService(typeof(IValueValidatingService), new SchedulerValueValidatingService(Controller, Navigator, navigationSelectionService));
			this.valueValidating = GetService<IValueValidatingService>();
			RegisterService(typeof(INavigationCallbackService), new SchedulerNavigationCallbackService(Controller));
			this.navigationCallback = GetService<INavigationCallbackService>();
			RegisterService(typeof(IOptionsProviderService), new SchedulerOptionsProviderService(Controller));
			this.optionsProvider = GetService<IOptionsProviderService>();
		}
		protected override void RegisterNavigationService() {
			RegisterService(typeof(INavigationService), new SchedulerMultipleSelectionNavigationStrategy(Navigator));
		}
		protected virtual DateNavigatorControllerBase CreateDateNavigatorController() {
			return new SchedulerDateNavigatorController(this, this);
		}
		public override void BeginInit() {
			base.BeginInit();
			Controller.BeginUpdate();
		}
		public override void EndInit() {
			base.EndInit();
			Controller.EndUpdate();
		}
		protected void OnControllerChanged(object sender, EventArgs e) {
			if (Controller.IsNavigatorChanges(DateNavigatorChangeType.SyncSelection)) {
				ApplyChangesToNavigator();
			}
			if (Controller.IsNavigatorChanges(DateNavigatorChangeType.UpdateSpecialDates)) {
				UpdateSpecialDates();
			}
		}
		protected void ApplyChangesToNavigator() {
			XtraSchedulerDebug.Assert(Controller.IsNavigatorChanges(DateNavigatorChangeType.SyncSelection));
			UnsubscribeControllerEvents();
			Controller.BeginNavigatorChanges(DateNavigatorChangeType.SuppressValidation);
			try {
				Controller.SyncNavigatorWithScheduler();  
			} finally {
				Controller.EndNavigatorChanges(DateNavigatorChangeType.SuppressValidation);
				SubscribeControllerEvents();
			}
		}
		protected bool IsFollowUpSelectionChanges() {
			return Controller.IsNavigatorChanges(DateNavigatorChangeType.SyncSelection | DateNavigatorChangeType.SuppressValidation);
		}
		protected virtual void ReplaceSelectedDates(ObservableCollection<DateTime> dates) {
			ValueEditing.SetSelectedDates(dates, true);
		}
		protected virtual void UpdateSpecialDates() {
			IList<DateTime> specialDates = DateNavigationUtils.CreateDateCollection(Controller.AppointmentDatesMap);
			Navigator.SpecialDates = RaiseCustomizeSpecialDates(specialDates);
			Controller.EndNavigatorChanges(DateNavigatorChangeType.UpdateSpecialDates);
		}
		public TimeInterval CalculateVisibleInterval() {
			if (Navigator == null)
				return TimeInterval.Empty;
			DateTime start = DateTime.MinValue;
			DateTime end = DateTime.MinValue;
			Navigator.CalculateVisibleDateRange(false, out start, out end);
			return new TimeInterval(start, end);
		}
	}
}
