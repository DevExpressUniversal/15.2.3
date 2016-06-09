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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region IDateNavigatorControllerOwner
	public interface IDateNavigatorControllerOwner {
		DateTime StartDate { get; set; }
		DateTime EndDate { get; }
		DateTime SelectionStart { get; }
		DateTime SelectionEnd { get; }
		DateTime GetFirstDayOfMonth(DateTime date);
		DayIntervalCollection GetSelection();
		void SetSelection(DayIntervalCollection days);
	}
	#endregion
	#region DateNavigatorControllerBase (abstract class)
	public abstract class DateNavigatorControllerBase {
		Dictionary<DateTime, bool> appointmentDatesMap;
		protected DateNavigatorControllerBase() {
			this.appointmentDatesMap = new Dictionary<DateTime, bool>();
		}
		protected internal Dictionary<DateTime, bool> AppointmentDatesMap { get { return appointmentDatesMap; } }
		protected abstract TimeZoneHelper TimeZoneHelper { get; }
		protected abstract bool CanFillAppointmentDatesMap();
		protected abstract bool CanObtainVisibleAppointments();
		protected abstract bool CanUpdateAppointmentDatesMap();
		protected abstract TimeInterval GetVisibleInterval();
		protected abstract ResourceBaseCollection GetVisibleResources();
		protected abstract void PopulateAppointmentDatesMap(TimeInterval filterInterval, ResourceBaseCollection visibleResources);
		protected internal virtual void UpdateAppointmentDatesMap() {
			if (UpdateAppointmentDatesMapCore())
				RaiseChanged();
		}
		protected internal virtual bool UpdateAppointmentDatesMapCore() {
			AppointmentDatesMap.Clear();
			if (!CanUpdateAppointmentDatesMap())
				return false;
			if (!CanFillAppointmentDatesMap()) {
				return true;
			}
			if (CanObtainVisibleAppointments()) {
				TimeInterval filterInterval = GetVisibleInterval();
				ResourceBaseCollection visibleResources = GetVisibleResources();
				SchedulerLogger.Trace(LoggerTraceLevel.DateNavigator, "->UpdateAppointmentDatesMapCore: interval={0}", filterInterval);
				PopulateAppointmentDatesMap(filterInterval, visibleResources);
			}
			return true;
		}
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected virtual TimeIntervalCollectionEx CreateTotalAppointmentsIntervalsAtInterval(AppointmentBaseCollection appointments, TimeInterval filterInterval) {
			TimeZoneHelper timeZoneHelper = TimeZoneHelper;
			TimeIntervalCollectionEx result = new TimeIntervalCollectionEx();
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				TimeInterval interval = ((IInternalAppointment)apt).GetInterval();
				if (interval.Contains(filterInterval))
					interval = filterInterval;
				else if (interval.Start < filterInterval.Start)
					interval = new TimeInterval(filterInterval.Start, interval.End);
				else if (interval.End > filterInterval.End)
					interval = new TimeInterval(interval.Start, filterInterval.End);
				TimeInterval clientInterval = timeZoneHelper.ToClientTime(interval);
				result.Add(new TimeInterval(clientInterval.Start.Date, clientInterval.CalculateAllDayDuration()));
			}
			return result;
		}
		protected virtual void AddDaysToAppointmentDatesMap(DayIntervalCollection days) {
			int count = days.Count;
			for (int i = 0; i < count; i++)
				appointmentDatesMap.Add(days[i].Start, true);
		}
	}
	#endregion
	#region DateNavigatorController
	public class DateNavigatorController : DateNavigatorControllerBase, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		InnerSchedulerControl control;
		ISchedulerStorageBase storage;
		bool boldAppointmentDates = true;
		BatchUpdateHelper batchUpdateHelper;
		IDateNavigatorControllerOwner owner;
		#endregion
		public DateNavigatorController(IDateNavigatorControllerOwner owner) {
			if (owner == null)
				Exceptions.ThrowArgumentNullException("owner");
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.owner = owner;
		}
		#region Properties
		protected internal virtual bool CanSyncNavigatorWithScheduler { get { return true; } }
		protected internal DayOfWeek FirstDayOfWeek { get { return control != null ? control.FirstDayOfWeek : DateTimeHelper.FirstDayOfWeek; } }
		protected internal IDateNavigatorControllerOwner Owner { get { return owner; } }
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
		#region InnerControl
		public InnerSchedulerControl InnerControl {
			get { return control; }
			set {
				if (control == value)
					return;
				SetInnerControlCore(value);
				OnInnerControlChanged();
			}
		}
		#endregion
		#region BoldAppointmentDates
		public bool BoldAppointmentDates {
			get { return boldAppointmentDates; }
			set {
				if (boldAppointmentDates == value)
					return;
				boldAppointmentDates = value;
				UpdateAppointmentDatesMap();
			}
		}
		#endregion
		#endregion
		#region Events
		public event DateNavigatorQueryActiveViewTypeHandler DateNavigatorQueryActiveViewType;
		protected internal virtual SchedulerViewType RaiseDateNavigatorQueryActiveViewType(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			if (DateNavigatorQueryActiveViewType == null)
				return newViewType;
			DateNavigatorQueryActiveViewTypeEventArgs e = new DateNavigatorQueryActiveViewTypeEventArgs(oldViewType, newViewType, selectedDays);
			DateNavigatorQueryActiveViewType(this, e);
			return e.NewViewType;
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			UpdateAppointmentDatesMap();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			UpdateAppointmentDatesMap();
		}
		#endregion
		#region SubscribeEvents
		protected internal virtual void SubscribeEvents() {
			if (control == null)
				return;
			SubscribeControlEvents();
			if (storage != null) {
				SubscribeStorageEvents();
			}
		}
		#endregion
		#region UnsubscribeEvents
		protected internal virtual void UnsubscribeEvents() {
			if (control == null)
				return;
			UnsubscribeControlEvents();
			if (storage != null) {
				UnsubscribeStorageEvents();
			}
		}
		#endregion
		#region SubscribeControlEvents
		protected internal virtual void SubscribeControlEvents() {
			control.InnerVisibleIntervalChanged += new EventHandler(OnVisibleIntervalChanged);
			control.ActiveViewChanged += new EventHandler(OnActiveViewChanged);
			control.StorageChanged += new EventHandler(OnStorageChanged);
			if (control.OptionsBehavior != null)
				control.OptionsBehavior.Changed += new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
		}
		#endregion
		#region UnsubscribeControlEvents
		protected internal virtual void UnsubscribeControlEvents() {
			control.ActiveViewChanged -= new EventHandler(OnActiveViewChanged);
			control.InnerVisibleIntervalChanged -= new EventHandler(OnVisibleIntervalChanged);
			control.StorageChanged -= new EventHandler(OnStorageChanged);
			if (control.OptionsBehavior != null)
				control.OptionsBehavior.Changed -= new BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
		}
		#endregion
		#region SubscribeStorageEvents
		protected internal virtual void SubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared += new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentCollectionLoaded += new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentsDeleted += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsInserted += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsChanged += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalResourcesChanged += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared += new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded += new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged += new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications += new EventHandler(OnDeferredNotifications);
		}
		#endregion
		#region UnsubscribeStorageEvents
		protected internal virtual void UnsubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared -= new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentCollectionLoaded -= new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentsDeleted -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsInserted -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsChanged -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalResourcesChanged -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared -= new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded -= new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged -= new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications -= new EventHandler(OnDeferredNotifications);
		}
		#endregion
		protected override void PopulateAppointmentDatesMap(TimeInterval filterInterval, ResourceBaseCollection visibleResources) {
			IList<DateTime> dates = GetVisibleAppointmentDates(filterInterval, visibleResources);
			int count = dates.Count;
			for (int i = 0; i < count; i++)
				AppointmentDatesMap.Add(dates[i], true);
		}
		protected internal virtual void SetInnerControlCore(InnerSchedulerControl value) {
			UnsubscribeEvents();
			if (storage != null)
				DeleteAppointmentCache();
			control = value;
			storage = control != null ? control.Storage : null;
			SubscribeEvents();
			if (storage != null)
				CreateAppointmentCache();
		}
		protected internal virtual void CreateAppointmentCache() {
			Storage.RegisterClient(Owner);
		}
		protected internal virtual void DeleteAppointmentCache() {
			Storage.UnregisterClient(Owner);
		}
		protected internal virtual void OnVisibleIntervalChanged(object sender, EventArgs e) {
			if (CanSyncNavigatorWithScheduler)
				SyncNavigatorWithScheduler();
		}
		protected internal virtual void OnActiveViewChanged(object sender, EventArgs e) {
			if (CanSyncNavigatorWithScheduler)
				SyncNavigatorWithScheduler();
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			if (storage != null) {
				UnsubscribeStorageEvents();
				DeleteAppointmentCache();
			}
			storage = InnerControl.Storage;
			if (storage != null) {
				SubscribeStorageEvents();
				CreateAppointmentCache();
			}
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnAppointmentsCollectionChanged(object sender, EventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, EventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnDeferredNotifications(object sender, EventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnResourcesCollectionChanged(object sender, EventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected internal virtual void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			UpdateAppointmentDatesMap();
		}
		protected override TimeZoneHelper TimeZoneHelper { get { return InnerControl.TimeZoneHelper; } }
		protected override bool CanUpdateAppointmentDatesMap() {
			return !IsUpdateLocked;
		}
		protected override bool CanFillAppointmentDatesMap() {
			return boldAppointmentDates;
		}
		protected override bool CanObtainVisibleAppointments() {
			return control != null;
		}
		protected override TimeInterval GetVisibleInterval() {
			DateTime startDate = DateTimeHelper.GetStartOfWeekUI(Owner.StartDate, control.FirstDayOfWeek);
			DateTime endDate = DateTimeHelper.GetStartOfWeekUI(Owner.EndDate, control.FirstDayOfWeek);
			endDate += DateTimeHelper.WeekSpan;
			return new TimeInterval(startDate, endDate);
		}
		protected override ResourceBaseCollection GetVisibleResources() {
			return InnerControl.GetFilteredResources();
		}
		protected IList<DateTime> GetVisibleAppointmentDates(TimeInterval interval, ResourceBaseCollection resources) {
			UnsubscribeEvents();
			try {
				return InnerControl.GetAppointmentTreeFilteredAppointments(interval, resources, TimeZoneHelper, Owner);
			} finally {
				SubscribeEvents();
			}
		}
		protected internal virtual void SyncNavigatorWithScheduler() {
			InnerSchedulerViewBase view = InnerControl.ActiveView;
			TimeInterval interval = new TimeInterval(view.InnerVisibleIntervals.Start, view.InnerVisibleIntervals.End);
			bool updateAppointmentDateMap = ValidateNavigatorInterval(interval);
			UpdateSelection();
			updateAppointmentDateMap |= ValidateSelectionVisiblity();
			if (updateAppointmentDateMap)
				UpdateAppointmentDatesMapCore();
			RaiseChanged();
		}
		protected virtual bool ValidateSelectionVisiblity() {
			return false;
		}
		protected virtual bool ValidateNavigatorInterval(TimeInterval visibleInterval) {
			DateTime start = visibleInterval.Start.Date;
			DateTime end = visibleInterval.End.Date - DateTimeHelper.DaySpan;
			bool updateAppointmentDateMap = true;
			if (start < Owner.StartDate || end > Owner.EndDate)
				Owner.StartDate = Owner.GetFirstDayOfMonth(start);
			return updateAppointmentDateMap;
		}
		protected internal virtual void ConnectToControl(InnerSchedulerControl control) {
			SetInnerControlCore(control);
			OnInnerControlChanged();
		}
		protected internal virtual void OnInnerControlChanged() {
			if (InnerControl != null) {
				InnerSchedulerViewBase view = InnerControl.ActiveView;
				Owner.StartDate = Owner.GetFirstDayOfMonth(view.InnerVisibleIntervals.Start.Date);
			}
			UpdateAppointmentDatesMapCore();
			UpdateSelection();
			RaiseChanged();
		}
		void UpdateSelection() {
			DayIntervalCollection days = new DayIntervalCollection();
			if (InnerControl != null)
				days.AddRange(InnerControl.ActiveView.GetVisibleIntervals());
			Owner.SetSelection(days);
		}
		public void SyncSelection() {
			if (InnerControl == null)
				return;
			UnsubscribeEvents();
			try {
				DayIntervalCollection selectedDays = CreateSelectedDaysCollection();
				SchedulerViewType newViewType = CalculateNewViewType(selectedDays);
				SchedulerLogger.Trace(LoggerTraceLevel.DateNavigator, "->DateNavigatorController.SyncSelection: newViewType={0}, selectedDays={1}", newViewType, selectedDays.Count);
				InnerControl.BeginUpdate();
				try {
					SwitchSchedulerControlActiveView(newViewType);
				} finally {
					InnerControl.EndUpdate();
				}
				InnerControl.BeginUpdate();
				try {
					InnerControl.ActiveView.SetVisibleDays(selectedDays, InnerControl.Selection);
				} finally {
					InnerControl.EndUpdate();
				}
			} finally {
				SubscribeEvents();
			}
			UpdateSelection();
			RaiseChanged();
		}
		protected virtual SchedulerViewType CalculateNewViewType(DayIntervalCollection selectedDays) {
			SchedulerViewType oldViewType = InnerControl.ActiveViewType;
			SchedulerViewType newViewType = SchedulerViewAutomaticAdjustHelper.SelectAdjustedView(InnerControl, selectedDays);
			newViewType = SelectAdjustedView(oldViewType, newViewType, selectedDays);
			newViewType = RaiseDateNavigatorQueryActiveViewType(oldViewType, newViewType, selectedDays);
			return newViewType;
		}
		SchedulerViewType SelectAdjustedView(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			if (oldViewType == SchedulerViewType.Timeline || oldViewType == SchedulerViewType.Gantt)
				if (selectedDays.Count < 15 && selectedDays.IsContinuous())
					return oldViewType;
			return newViewType;
		}
		protected internal virtual bool SwitchSchedulerControlActiveView(SchedulerViewType viewType) {
			InnerControl.ActiveViewType = viewType;
			return InnerControl.ActiveViewType == viewType;
		}
		protected internal virtual DayIntervalCollection CreateSelectedDaysCollection() {
			return Owner.GetSelection();
		}
		protected internal virtual TimeInterval CreateSelectionInterval() {
			return CreateSelectionInterval(Owner.SelectionStart.Date, Owner.SelectionEnd.Date);
		}
		protected internal virtual TimeInterval CreateSelectionInterval(DateTime selectionStart, DateTime selectionEnd) {
			if (selectionStart < selectionEnd)
				return new TimeInterval(selectionStart, selectionEnd + DateTimeHelper.DaySpan);
			else
				return new TimeInterval(selectionEnd, selectionStart + DateTimeHelper.DaySpan);
		}
		public DateTime AdjustSelectionStart(DateTime start, DateTime end) {
			TimeSpan weekSpan = TimeSpan.FromDays(6);
			DayOfWeek actualFirstDayOfWeek = FirstDayOfWeek;
			if (InnerControl != null && InnerControl.MonthView.Enabled)
				actualFirstDayOfWeek = InnerControl.MonthView.ActualFirstDayOfWeek;
			if (end >= start) {
				if (end - start < weekSpan)
					return start;
				else {
					return DateTimeHelper.GetStartOfWeekUI(start, actualFirstDayOfWeek);
				}
			} else {
				if (start - end < weekSpan)
					return start;
				else
					return DateTimeHelper.GetStartOfWeekUI(start, actualFirstDayOfWeek) + weekSpan;
			}
		}
		public DateTime AdjustSelectionEnd(DateTime start, DateTime end) {
			DayOfWeek actualFirstDayOfWeek = FirstDayOfWeek;
			if (InnerControl != null && InnerControl.MonthView.Enabled)
				actualFirstDayOfWeek = InnerControl.MonthView.ActualFirstDayOfWeek;
			TimeSpan weekSpan = TimeSpan.FromDays(6);
			if (end >= start) {
				if (end - start < weekSpan)
					return end;
				return DateTimeHelper.GetStartOfWeekUI(end, actualFirstDayOfWeek) + weekSpan;
			} else {
				if (start - end < weekSpan)
					return end;
				return DateTimeHelper.GetStartOfWeekUI(end, actualFirstDayOfWeek);
			}
		}
		public virtual void ShiftVisibleIntervals(int yearOffset, int monthOffset, int daysOffset) {
			if (InnerControl == null)
				return;
			TimeSpan offset = CalculateVisibleIntervalsOffset(yearOffset, monthOffset, daysOffset);
			ShiftVisibleIntervalsCore(offset);
		}
		public virtual void ShiftVisibleIntervals(TimeSpan offset) {
			if (InnerControl == null)
				return;
			DateTime visibleStart = InnerControl.ActiveView.InnerVisibleIntervals.Start;
			DateTime newVisibleStart = visibleStart.Add(offset);
			TimeSpan span = GetValidOffset(newVisibleStart - visibleStart);
			ShiftVisibleIntervalsCore(span);
		}
		protected virtual void ShiftVisibleIntervalsCore(TimeSpan span) {
			if (InnerControl == null || span == TimeSpan.Zero)
				return;
			TimeIntervalCollection visibleIntervals = InnerControl.ActiveView.GetVisibleIntervals();
			visibleIntervals.Shift(span);
			InnerControl.ActiveView.SetVisibleIntervals(visibleIntervals, InnerControl.Selection);
		}
		protected internal virtual TimeSpan CalculateVisibleIntervalsOffset(int yearOffset, int monthOffset, int daysOffset) {
			DateTime visibleStart = InnerControl.ActiveView.InnerVisibleIntervals.Start;
			DateTime newVisibleStart = visibleStart;
			newVisibleStart = newVisibleStart.AddYears(yearOffset);
			newVisibleStart = newVisibleStart.AddMonths(monthOffset);
			newVisibleStart = newVisibleStart.AddDays(daysOffset);
			return GetValidOffset(newVisibleStart - visibleStart);
		}
		protected internal virtual TimeSpan GetValidOffset(TimeSpan offset) {
			if (InnerControl.ActiveViewType == SchedulerViewType.Day)
				return offset;
			long remainder = offset.Ticks % DateTimeHelper.WeekSpan.Ticks;
			return remainder == 0 ? offset : new TimeSpan(offset.Ticks - remainder);
		}
		public virtual bool IsWorkDay(DateTime date) {
			return control.WorkDays.IsWorkDay(date.Date);
		}
	}
	#endregion
}
