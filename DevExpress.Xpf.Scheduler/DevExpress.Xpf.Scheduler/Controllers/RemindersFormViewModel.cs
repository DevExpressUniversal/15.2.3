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
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using DevExpress.Utils.Commands;
using System.Collections;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	public class RemindersFormViewModel : ViewModelBase, IDocumentContent {
		public ObservableCollection<ReminderInfo> Alerts { get; set; }
		public object Title { get; set; }
		public IDocument ActiveDocument { get; set; }
		protected SchedulerControl Control { get; set; }
		protected SchedulerStorage Storage { get { return Control.Storage; } }
		protected TimeZoneHelper TimeZoneHelper { get { return Control.InnerControl.TimeZoneHelper; } }
		protected Timer SnoozeListUpdateTimer { get; set; }
		public RemindersFormViewModel(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.Control = control;
			this.SnoozeList = CreateSnoozeList(DateTime.MaxValue);
			Alerts = new ObservableReminderInfoCollection();
			Alerts.CollectionChanged += new NotifyCollectionChangedEventHandler(OnRemindersChanged);
			SubscribeToControlEvents();
			SubscribeStorageEvents();
			InitializesnoozeListUpdateTimer();
		}
		static Dictionary<SchedulerControl, RemindersFormViewModel> currentInstance = new Dictionary<SchedulerControl, RemindersFormViewModel>();
		public static Dictionary<SchedulerControl, RemindersFormViewModel> CurrentInstances {
			get { return currentInstance; }
			protected set { currentInstance = value; }
		}
		public static RemindersFormViewModel Create(SchedulerControl control) {
			return ViewModelSource.Create(() => new RemindersFormViewModel(control));
		}
		bool isRemindersSubjectVisible = false;
		public bool IsRemindersSubjectVisible {
			get { return isRemindersSubjectVisible; }
			set {
				if(isRemindersSubjectVisible != value) {
					isRemindersSubjectVisible = value;
					RaisePropertiesChanged("IsRemindersSubjectVisible");
				}
			}
		}
		string appointmentStartDisplayText = string.Empty;
		public string AppointmentStartDisplayText {
			get { return appointmentStartDisplayText; }
			set {
				if(appointmentStartDisplayText != value) {
					appointmentStartDisplayText = value;
					RaisePropertiesChanged("AppointmentStartDisplayText");
				}
			}
		}
		bool canOpenEditAppointmentForm = false;
		public bool CanOpenEditAppointmentForm {
			get { return canOpenEditAppointmentForm; }
			set {
				if(canOpenEditAppointmentForm != value) {
					canOpenEditAppointmentForm = value;
					RaisePropertiesChanged("CanOpenEditAppointmentForm");
				}
			}
		}
		List<SnoozeItem> snoozeList = null;
		public List<SnoozeItem> SnoozeList {
			get { return snoozeList; }
			set {
				if(snoozeList != value) {
					snoozeList = value;
					RaisePropertiesChanged("SnoozeList");
				}
			}
		}
		TimeSpan snoozeValue;
		public TimeSpan SnoozeValue {
			get { return snoozeValue; }
			set {
				if(snoozeValue != value) {
					snoozeValue = value;
					RaisePropertiesChanged("SnoozeValue");
				}
			}
		}
		ObservableCollection<ReminderInfo> selectedReminders = new ObservableCollection<ReminderInfo>();
		public ObservableCollection<ReminderInfo> SelectedReminders {
			get { return selectedReminders; }
			set {
				if(!CheckSelectedRemindersCollectionsEquality(selectedReminders, value)) {
					selectedReminders = value;
					OnSelectedRemindersChanged();
					RaisePropertiesChanged("SelectedReminders");
				} else {
					selectedReminders = value;
				}
			}
		}
		bool CheckSelectedRemindersCollectionsEquality(ObservableCollection<ReminderInfo> firstCollection, ObservableCollection<ReminderInfo> secondCollection) {
			if(firstCollection.Count != secondCollection.Count)
				return false;
			bool result = true;
			for(int i = 0; i < firstCollection.Count; i++) {
				if(firstCollection[i] != secondCollection[i]) {
					result = false;
				}
			}
			return result;
		}
		int selectedReminderIndex = 0;
		public int SelectedReminderIndex {
			get { return selectedReminderIndex; }
			set {
				if(selectedReminderIndex != value) {
					selectedReminderIndex = value;
					RaisePropertiesChanged("SelectedReminderIndex");
				}
			}
		}
		void OnSelectedRemindersChanged() {
			CanOpenEditAppointmentForm = CanEditAppointment();
			int selectedItemsCount = SelectedReminders.Count;
			if(selectedItemsCount == 1) {
				string format = SchedulerLocalizer.GetString(SchedulerStringId.Caption_StartTime);
				Appointment selectedAppointment = GetSelectedAppointment();
				DateTime aptStart = ToClientTime(((IInternalAppointment)selectedAppointment).GetInterval(), selectedAppointment.TimeZoneId).Start;
				AppointmentStartDisplayText = String.Format(format, aptStart.ToString());
				IsRemindersSubjectVisible = true;
			} else if(selectedItemsCount > 1) {
				AppointmentStartDisplayText = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Caption_NAppointmentsAreSelected), selectedItemsCount);
				IsRemindersSubjectVisible = false;
			} else {
				IsRemindersSubjectVisible = false;
			}
			UpdateSnoozeList();
		}
		protected internal virtual TimeInterval ToClientTime(TimeInterval appointmentInterval, string timeZoneId) {
			if(TimeZoneHelper != null)
				return TimeZoneHelper.ToClientTime(appointmentInterval, timeZoneId, true);
			else
				return appointmentInterval;
		}
		void InitializesnoozeListUpdateTimer() {
			SnoozeListUpdateTimer = new Timer();
		}
		protected virtual List<SnoozeItem> CreateSnoozeList(DateTime minAppointmentTime) {
			List<SnoozeItem> result = new List<SnoozeItem>();
			DateTime now = DateTime.Now;
			int count = ReminderTimeSpans.BeforeStartTimeSpanValues.Length;
			for(int i = 0; i < count; i++) {
				TimeSpan spanToSnooze = ReminderTimeSpans.BeforeStartTimeSpanValues[i];
				if(minAppointmentTime + spanToSnooze <= now)
					continue;
				string timeSpan = HumanReadableTimeSpanHelper.ToString(spanToSnooze);
				timeSpan = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_TimeBeforeStart), timeSpan);
				result.Add(new SnoozeItem(timeSpan, spanToSnooze));
			}
			int activeSelectedIndex = result.Count;
			TimeSpan[] availableSnoozeTimes = ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues;
			count = availableSnoozeTimes.Length;
			for(int i = 0; i < count; i++)
				result.Add(new SnoozeItem(HumanReadableTimeSpanHelper.ToString(availableSnoozeTimes[i]), availableSnoozeTimes[i]));
			if(activeSelectedIndex < result.Count)
				SnoozeValue = result[activeSelectedIndex].TimeSpan;
			return result;
		}
		void OnRemindersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			int count = Alerts.Count;
			string format = (count == 1) ? SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminder) : SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminders);
			Title = String.Format(format, count);
		}
		protected virtual void SubscribeToControlEvents() {
			Control.InnerControl.StorageChanged += new EventHandler(OnStorageChanged);
#if !SL
			Control.OptionsCustomization.Changed += new EventHandler(OnOptionsCustomizationChanged);
#endif
		}
		protected virtual void UsubscribeToControlEvents() {
			Control.InnerControl.StorageChanged -= new EventHandler(OnStorageChanged);
#if !SL
			Control.OptionsCustomization.Changed += new EventHandler(OnOptionsCustomizationChanged);
#endif
		}
		void OnOptionsCustomizationChanged(object sender, EventArgs e) {
			CanOpenEditAppointmentForm = CanEditAppointment();
		}
		protected virtual bool CanEditAppointment() {
			return SelectedReminders.Count == 1 && CanEditAppointmentCore(GetSelectedAppointment());
		}
		protected virtual bool CanEditAppointmentCore(Appointment apt) {
			if(Control == null)
				return false;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			return helper.CanEditAppointment(apt);
		}
		Appointment GetSelectedAppointment() {
			return Alerts[SelectedReminderIndex].Notification.ActualAppointment;
		}
		protected virtual void OnStorageChanged(object sender, EventArgs e) {
			if(Storage != null)
				UnsubscribeStorageEvents();
			GetReminders().ForEach(UnsubscribeReminderEvents);
			if(Storage != null)
				SubscribeStorageEvents();
			Alerts.Clear();
			Control.Storage.TriggerAlerts();
			CloseIfEmpty();
		}
		protected virtual ReminderCollection GetReminders() {
			ReminderCollection result = new ReminderCollection();
			int count = Alerts.Count;
			for(int i = 0; i < count; i++)
				result.Add(GetReminder(i));
			return result;
		}
		protected virtual void SubscribeStorageEvents() {
			Storage.AppointmentsChanged += OnAppointmentsChanged;
			Storage.AppointmentsDeleted += OnAppointmentsDeleted;
			Storage.AppointmentCollectionCleared += OnStorageChanged;
			Storage.AppointmentCollectionLoaded += OnStorageChanged;
		}
		protected virtual void UnsubscribeStorageEvents() {
			Storage.AppointmentsDeleted -= OnAppointmentsDeleted;
			Storage.AppointmentsChanged -= OnAppointmentsChanged;
			Storage.AppointmentCollectionCleared -= OnStorageChanged;
			Storage.AppointmentCollectionLoaded -= OnStorageChanged;
		}
		protected virtual void OnAppointmentsDeleted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentDeleted);
		}
		protected virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentChanged);
		}
		protected virtual void OnAppointmentsChangedCore(IList changedObjects, Action<Appointment> handler) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(changedObjects);
			appointments.ForEach(handler);
			Control.Storage.TriggerAlerts();
			CloseIfEmpty();
		}
		protected virtual void OnAppointmentDeleted(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentDeletedHandleItemIndex);
		}
		protected virtual void OnAppointmentChanged(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentChangedHandleItemIndex);
		}
		protected virtual void OnAppointmentDeletedHandleItemIndex(int itemIndex) {
			Reminder reminder = GetReminder(itemIndex);
			UnsubscribeReminderEvents(reminder);
			Alerts.RemoveAt(itemIndex);
		}
		protected virtual void OnAppointmentChangedHandleItemIndex(int itemIndex) {
			ReminderAlertNotification alertNotification = GetReminderItem(itemIndex);
			Reminder reminder = alertNotification.Reminder;
			if(!reminder.Appointment.Reminders.Contains(reminder)) {
				UnsubscribeReminderEvents(reminder);
				Alerts.RemoveAt(itemIndex);
			}
		}
		protected virtual List<int> FindAppointmentRemindersIndexes(Appointment apt) {
			List<int> result = new List<int>();
			int count = Alerts.Count;
			for(int i = count - 1; i >= 0; i--) {
				Reminder reminder = GetReminder(i);
				if(reminder.Appointment == apt)
					result.Add(i);
			}
			return result;
		}
		protected virtual Reminder GetReminder(int index) {
			return GetReminderItem(index).Reminder;
		}
		protected virtual ReminderAlertNotification GetReminderItem(int index) {
			return Alerts[index].Notification as ReminderAlertNotification;
		}
		protected virtual void SubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged += new EventHandler(OnReminderChanged);
		}
		protected virtual void UnsubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged -= new EventHandler(OnReminderChanged);
		}
		protected virtual void OnReminderChanged(object sender, EventArgs e) {
			Reminder changedReminder = (Reminder)sender;
			int count = Alerts.Count;
			for(int i = 0; i < count; i++) {
				Reminder reminder = GetReminder(i);
				if(changedReminder == reminder) {
					UnsubscribeReminderEvents(reminder);
					Alerts.RemoveAt(i);
					CloseIfEmpty();
					break;
				}
			}
		}
		protected virtual void DismissAllCore() {
			ForAllItems(DismissReminder);
		}
		protected virtual void OpenItemCore() {
			Control.ShowEditAppointmentForm(Alerts[SelectedReminderIndex].Notification.ActualAppointment, false, CommandSourceType.Unknown);
		}
		protected virtual void SnoozeCore() {
			ForAllSelectedItems(SnoozeReminder);
		}
		protected virtual void DismissCore() {
			ForAllSelectedItems(DismissReminder);
		}
		protected virtual void SnoozeReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			SnoozeReminderCore(reminder, SnoozeValue);
		}
		protected virtual void SnoozeReminderCore(Reminder reminder, TimeSpan span) {
			if(span.Ticks <= 0) {
				if(-span.Ticks > reminder.Appointment.Start.Ticks)
					reminder.AlertTime = DateTime.MinValue;
				else
					reminder.AlertTime = reminder.Appointment.Start + span;
			} else
				reminder.AlertTime = DateTime.Now + span;
			(reminder).Suspended = false;
		}
		protected virtual void DismissReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			if(!reminder.Dismiss())
				(reminder).Suspended = false;
		}
		protected virtual void ForAllItems(Action<Reminder> handler) {
			UnsubscribeStorageEvents();
			try {
				Storage.BeginUpdate();
				try {
					GetReminders().ForEach(handler);
					Alerts.Clear();
				} finally {
					Storage.EndUpdate();
				}
			} finally {
				SubscribeStorageEvents();
			}
		}
		protected virtual void ForAllSelectedItems(Action<Reminder> handler) {
			UnsubscribeStorageEvents();
			try {
				Storage.BeginUpdate();
				try {
					GetSelectedReminders().ForEach(handler);
					RemoveSelectedItems();
				} finally {
					Storage.EndUpdate();
				}
			} finally {
				SubscribeStorageEvents();
			}
		}
		protected virtual ReminderCollection GetSelectedReminders() {
			ReminderCollection result = new ReminderCollection();
			int count = SelectedReminders.Count;
			for(int i = 0; i < count; i++) {
				ReminderInfo selectedReminderInfo = SelectedReminders[i];
				result.Add(selectedReminderInfo.Notification.Reminder);
			}
			return result;
		}
		protected virtual void RemoveSelectedItems() {
			List<ReminderInfo> selectedAlerts = GetSelectedAlerts();
			int count = selectedAlerts.Count;
			for(int i = 0; i < count; i++)
				Alerts.Remove(selectedAlerts[i]);
			SetFirstReminderSelected();
		}
		protected void SetFirstReminderSelected() {
			SelectedReminderIndex = 0;
		}
		protected virtual List<ReminderInfo> GetSelectedAlerts() {
			List<ReminderInfo> result = new List<ReminderInfo>();
			int count = SelectedReminders.Count;
			for(int i = 0; i < count; i++)
				result.Add(SelectedReminders[i]);
			return result;
		}
		public virtual void AddReminderAlert(ReminderAlertNotificationCollection notifications) {
			InsertRemindersItems(notifications);
		}
		public virtual void InsertRemindersItems(ReminderAlertNotificationCollection notifications) {
			ClearObsoleteReminders(notifications);
			int count = notifications.Count;
			if(count <= 0)
				return;
			for(int i = 0; i < count; i++) {
				ReminderAlertNotification notification = notifications[i];
				notification.Handled = true;
				Reminder reminder = notification.Reminder;
				SubscribeReminderEvents(reminder);
				Alerts.Add(new ReminderInfo(notification));
			}
			UpdateSnoozeList();
		}
		void ClearObsoleteReminders(ReminderAlertNotificationCollection notifications) {
			int count = Alerts.Count;
			for(int i = count - 1; i >= 0; i--) {
				CheckObsoleteReminder(i, notifications);
			}
		}
		void CheckObsoleteReminder(int reminderIndex, ReminderAlertNotificationCollection newNotifications) {
			ReminderAlertNotification alertNotification = GetReminderItem(reminderIndex);
			Reminder reminder = alertNotification.Reminder;
			Appointment appointment = alertNotification.ActualAppointment;
			if(reminder.Appointment.Type != AppointmentType.Pattern)
				return;
			int count = newNotifications.Count;
			for(int i = 0; i < count; i++) {
				ReminderAlertNotification notification = newNotifications[i];
				if(notification.ActualAppointment.IsRecurring && appointment.RecurrenceIndex == notification.ActualAppointment.RecurrenceIndex) {
					UnsubscribeReminderEvents(reminder);
					Alerts.RemoveAt(reminderIndex);
				}
			}
		}
		void UpdateSnoozeList() {
			int count = SelectedReminders.Count;
			DateTime minDateTime = DateTime.MaxValue;
			for(int i = 0; i < count; i++) {
				ReminderInfo reminderInfo = SelectedReminders[i] as ReminderInfo;
				if(reminderInfo == null)
					continue;
				Appointment selectedAppointment = reminderInfo.Notification.ActualAppointment;
				minDateTime = DateTimeHelper.Min(selectedAppointment.Start, minDateTime);
			}
			SnoozeList = CreateSnoozeList(minDateTime);
			SetupUpdateSnoozeListTimer(minDateTime);
		}
		void SetupUpdateSnoozeListTimer(DateTime minDateTime) {
			SnoozeListUpdateTimer.Stop();
			if(minDateTime == DateTime.MaxValue)
				return;
			int count = SnoozeList.Count;
			TimeSpan minTimeSpan = TimeSpan.MaxValue;
			for(int i = 0; i < count; i++) {
				SnoozeItem item = SnoozeList[i];
				TimeSpan itemTimeSpan = item.TimeSpan;
				if(itemTimeSpan.Ticks <= 0)
					minTimeSpan = DateTimeHelper.Min(itemTimeSpan, minTimeSpan);
			}
			if(minTimeSpan.Ticks > 0)
				return;
			TimeSpan spanToUpdate = (minDateTime + minTimeSpan) - DateTime.Now;
			if(spanToUpdate.Ticks < 0)
				return;
			SnoozeListUpdateTimer.Interval = spanToUpdate;
			SnoozeListUpdateTimer.Tick += OnSnoozeListUpdateTimerTick;
			SnoozeListUpdateTimer.Start();
		}
		void OnSnoozeListUpdateTimerTick(object sender, EventArgs e) {
			UpdateSnoozeList();
		}
		protected virtual void CloseIfEmpty() {
			if(Alerts.Count > 0)
				return;
			CloseForm();
		}
		public void CloseForm() {
			ActiveDocument.Close(false);
		}
		protected virtual bool PerformRemindersFormDefaultAction() {
			RemindersFormDefaultAction action = CalculateRemindersFormDefaultAction();
			switch(action) {
				default:
				case RemindersFormDefaultAction.DismissAll:
					DismissAllCore();
					return true;
				case RemindersFormDefaultAction.SnoozeAll:
					SnoozeAll();
					return true;
				case RemindersFormDefaultAction.Custom:
					return RaiseRemindersFormDefaultActionEvent();
			}
		}
		protected virtual bool RaiseRemindersFormDefaultActionEvent() {
			if(Control == null) {
				DismissAllCore();
				return true;
			}
			ReminderAlertNotificationCollection alertNotifications = CreateAlertNotifications();
			if(alertNotifications.Count <= 0)
				return true;
			RemindersFormDefaultActionEventArgs args = new RemindersFormDefaultActionEventArgs(alertNotifications);
			Control.InnerControl.RaiseRemindersFormDefaultAction(args);
			if(!args.Handled) {
				DismissAllCore();
				return true;
			} else
				return !args.Cancel;
		}
		protected virtual ReminderAlertNotificationCollection CreateAlertNotifications() {
			ReminderAlertNotificationCollection result = new ReminderAlertNotificationCollection();
			int count = Alerts.Count;
			for(int i = 0; i < count; i++) {
				ReminderAlertNotification notification = GetReminderItem(i);
				result.Add(new ReminderAlertNotification(notification.Reminder, notification.ActualAppointment));
			}
			return result;
		}
		protected virtual void SnoozeAll() {
			ForAllItems(SnoozeReminder);
		}
		protected virtual RemindersFormDefaultAction CalculateRemindersFormDefaultAction() {
			if(Control != null)
				return Control.OptionsBehavior.RemindersFormDefaultAction;
			return RemindersFormDefaultAction.DismissAll;
		}
		void DisposeTimer() {
			SnoozeListUpdateTimer.Stop();
			SnoozeListUpdateTimer.Dispose();
			SnoozeListUpdateTimer = null;
		}
		public bool Close() {
			bool result = true;
			if(Storage != null)
				result = PerformRemindersFormDefaultAction();
			if(result) {
#if DEBUGTEST
				if(DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.ContainsKey(this))
					DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Remove(this);
#endif
				CurrentInstances.Remove(Control);
				Alerts.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnRemindersChanged);
				UsubscribeToControlEvents();
				UnsubscribeStorageEvents();
				DisposeTimer();
			}
			return result;
		}
		#region Commands
#if !SL
		public void OnRemindersDoubleClick(object parameter) {
			if(Convert.ToBoolean(parameter)) {
				OpenItemCore();
			}
		}
#endif
		public void DismissAll() {
			DismissAllCore();
			CloseIfEmpty();
		}
		public void OpenItem() {
			OpenItemCore();
		}
		public void Dismiss() {
			DismissCore();
			CloseIfEmpty();
		}
		public void Snooze() {
			SnoozeCore();
			CloseIfEmpty();
		}
		#endregion
		protected IDocumentOwner DocumentOwner { get; private set; }
		#region IDocumentContent
		void IDocumentContent.OnClose(CancelEventArgs e) {
			e.Cancel = !Close();
		}
		void IDocumentContent.OnDestroy() { }
		IDocumentOwner IDocumentContent.DocumentOwner {
			get { return DocumentOwner; }
			set { DocumentOwner = value; }
		}
		#endregion
	}
	public class SnoozeItem {
		readonly string displayText;
		readonly TimeSpan timeSpan;
		public SnoozeItem(string displayText, TimeSpan timeSpan) {
			this.displayText = displayText;
			this.timeSpan = timeSpan;
		}
		public string DisplayText { get { return displayText; } }
		public TimeSpan TimeSpan { get { return timeSpan; } }
	}
	public class ReminderInfo : INotifyContentChanged {
		ReminderAlertNotification notification;
		public ReminderInfo(ReminderAlertNotification notification) {
			this.notification = notification;
		}
		public ReminderAlertNotification Notification { get { return notification; } }
		public string Subject { get { return Notification.Reminder.Subject; } }
		#region INotifyContentChanged Members
		public event EventHandler ContentChanged { add { } remove { } }
		#endregion
	}
}
