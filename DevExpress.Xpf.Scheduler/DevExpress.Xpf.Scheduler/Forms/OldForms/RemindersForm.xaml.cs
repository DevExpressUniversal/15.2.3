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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core;
using System.Collections;
using DevExpress.Utils.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Native;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	public interface ISchedulerFormClosingSupport {
		void RaiseClosingEvent(CancelEventArgs e);
	}
	public delegate void ReminderAction(Reminder reminder);
	public class RemindersFormBase : UserControl, ISchedulerFormClosingSupport {
		public virtual void OnReminderAlert(ReminderAlertNotificationCollection notifications) {
		}
		public virtual void OnClose() {
		}
		CancelEventHandler onClosingEvent;
		public event CancelEventHandler Closing { add { onClosingEvent += value; } remove { onClosingEvent -= value; } }
		void ISchedulerFormClosingSupport.RaiseClosingEvent(CancelEventArgs e) {
			if (onClosingEvent != null)
				onClosingEvent(this, e);
		}
	}
	public partial class RemindersForm : RemindersFormBase {
		#region Fields
		readonly SchedulerControl control;
		FloatingContainer container;
		readonly List<SnoozeItem> snoozeList;
		TimeZoneHelper timeZoneEngine;
		SchedulerStorage storage;
		OptionsCustomization optionsCustomization;
		ObservableReminderInfoCollection alerts;
		Timer snoozeListUpdateTimer;
		#endregion
		public RemindersForm(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.storage = control.Storage;
			this.snoozeList = CreateSnoozeList(DateTime.MaxValue);
			this.timeZoneEngine = control.InnerControl.TimeZoneHelper;
			this.optionsCustomization = control.OptionsCustomization;
			this.alerts = new ObservableReminderInfoCollection();
			this.DataContext = this;
			InitializeComponent();
			this.DataContext = this;
			SubscribeToEvents();
			SubscribeToControlEvents();
			SubscribeStorageEvents();
			InitializesnoozeListUpdateTimer();
		}
		#region Properties
		#region CanOpenEditAppointmentForm
		public static readonly DependencyProperty CanOpenEditAppointmentFormProperty = DependencyPropertyHelper.RegisterProperty<RemindersForm, bool>("CanOpenEditAppointmentForm", false);
		public bool CanOpenEditAppointmentForm {
			get { return (bool)GetValue(CanOpenEditAppointmentFormProperty); }
			set { SetValue(CanOpenEditAppointmentFormProperty, value); }
		}
		#endregion
		#region AppointmentStartDisplayText
		public static readonly DependencyProperty AppointmentStartDisplayTextProperty = DependencyPropertyHelper.RegisterProperty<RemindersForm, string>("AppointmentStartDisplayText", String.Empty);
		public string AppointmentStartDisplayText {
			get { return (string)GetValue(AppointmentStartDisplayTextProperty); }
			set { SetValue(AppointmentStartDisplayTextProperty, value); }
		}
		#endregion
		#region SnoozeValue
		public static readonly DependencyProperty SnoozeValueProperty = DependencyPropertyHelper.RegisterProperty<RemindersForm, TimeSpan>("SnoozeValue", new TimeSpan());
		public TimeSpan SnoozeValue {
			get { return (TimeSpan)GetValue(SnoozeValueProperty); }
			set { SetValue(SnoozeValueProperty, value); }
		}
		#endregion
		#region SnoozeList
		public static readonly DependencyProperty SnoozeListProperty = DependencyPropertyHelper.RegisterProperty<RemindersForm, List<SnoozeItem>>("SnoozeList", null);
		public List<SnoozeItem> SnoozeList {
			get { return (List<SnoozeItem>)GetValue(SnoozeListProperty); }
			set { SetValue(SnoozeListProperty, value); }
		}
		#endregion
		protected internal SchedulerControl Control { get { return control; } }
		protected internal FloatingContainer Container { get { return container; } set { container = value; } }
		public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } set { timeZoneEngine = value; } }
		public ObservableReminderInfoCollection Alerts { get { return alerts; } }
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RemindersForm, String>("Title", String.Empty);
		#endregion
		#endregion
		void InitializesnoozeListUpdateTimer() {
			this.snoozeListUpdateTimer = new Timer();
		}
		void SetupUpdateSnoozeListTimer(DateTime minDateTime) {
			this.snoozeListUpdateTimer.Stop();
			if (minDateTime == DateTime.MaxValue)
				return;
			int count = SnoozeList.Count;
			TimeSpan minTimeSpan = TimeSpan.MaxValue;
			for (int i = 0; i < count; i++) {
				SnoozeItem item = SnoozeList[i];
				TimeSpan itemTimeSpan = item.TimeSpan;
				if (itemTimeSpan.Ticks <= 0)
					minTimeSpan = DateTimeHelper.Min(itemTimeSpan, minTimeSpan);
			}
			if (minTimeSpan.Ticks > 0)
				return;
			TimeSpan spanToUpdate = (minDateTime + minTimeSpan) - DateTime.Now;
			if (spanToUpdate.Ticks < 0)
				return;
			this.snoozeListUpdateTimer.Interval = spanToUpdate;
			this.snoozeListUpdateTimer.Tick += OnSnoozeListUpdateTimerTick;
			this.snoozeListUpdateTimer.Start();
		}
		void OnSnoozeListUpdateTimerTick(object sender, EventArgs e) {
			UpdateSnoozeList();
		}
		void DisposeTimer() {
			this.snoozeListUpdateTimer.Stop();
			this.snoozeListUpdateTimer.Dispose();
			this.snoozeListUpdateTimer = null;
		}
		public override void OnClose() {
			UnsubscribeToEvents();
			UsubscribeToControlEvents();
			UnsubscribeStorageEvents();
			DisposeTimer();
		}		
		protected internal virtual void SubscribeToEvents() {
			this.lbReminders.SelectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(Reminders_SelectionChanged);
#if !SL
			this.lbReminders.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(OnLbRemindersMouseDoubleClick);
#endif
			Alerts.CollectionChanged += new NotifyCollectionChangedEventHandler(OnRemindersChanged);
			Closing += new CancelEventHandler(OnFormClosing);
		}
		void OnFormClosing(object sender, CancelEventArgs e) {
			if (storage != null)
				e.Cancel = !PerformRemindersFormDefaultAction();
		}
		protected internal virtual void UnsubscribeToEvents() {
			Closing -= new CancelEventHandler(OnFormClosing);
			this.lbReminders.SelectedItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(Reminders_SelectionChanged);
#if !SL
			this.lbReminders.MouseDoubleClick -= new System.Windows.Input.MouseButtonEventHandler(OnLbRemindersMouseDoubleClick);
#endif
			Alerts.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnRemindersChanged);
		}
		void OnRemindersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			int count = Alerts.Count;
			string format = (count == 1) ? SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminder) : SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminders);
			Title = String.Format(format, count);
		}
		void Reminders_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			CanOpenEditAppointmentForm = CanEditAppointment();
			int selectedItemsCount = this.lbReminders.SelectedItems.Count;
			if (selectedItemsCount == 1) {
				string format = SchedulerLocalizer.GetString(SchedulerStringId.Caption_StartTime);
				Appointment selectedAppointment = GetSelectedAppointment();
				DateTime aptStart = ToClientTime(((IInternalAppointment)selectedAppointment).GetInterval(), selectedAppointment.TimeZoneId).Start;
				AppointmentStartDisplayText = String.Format(format, aptStart.ToString());
				VisualStateManagerHelper.GoToState(this, "OneReminderSelectedState", false);
			}
			else if (selectedItemsCount > 1) {
				AppointmentStartDisplayText = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Caption_NAppointmentsAreSelected), selectedItemsCount);
				VisualStateManagerHelper.GoToState(this, "MoreThanOneReminderSelectedState", false);
			}
			else
				VisualStateManagerHelper.GoToState(this, "NoRemindersSelectedState", false);
			UpdateSnoozeList();
		}
		void UpdateSnoozeList() {
			int count = this.lbReminders.SelectedItems.Count;
			DateTime minDateTime = DateTime.MaxValue;
			for (int i = 0; i < count; i++) {
				ReminderInfo reminderInfo = this.lbReminders.SelectedItems[i] as ReminderInfo;
				if (reminderInfo == null)
					continue;
				Appointment selectedAppointment = reminderInfo.Notification.ActualAppointment;
				minDateTime = DateTimeHelper.Min(selectedAppointment.Start, minDateTime);
			}
			SnoozeList = CreateSnoozeList(minDateTime);
			SetupUpdateSnoozeListTimer(minDateTime);
		}		
		ReminderInfo GetSelectedItem() {
			return this.lbReminders.SelectedItem as ReminderInfo;
		}
		Appointment GetSelectedAppointment() {
			return GetSelectedItem().Notification.ActualAppointment;
		}
		protected internal virtual TimeInterval ToClientTime(TimeInterval interval, string timeZoneId) {
			if (TimeZoneHelper != null)
				return TimeZoneHelper.ToClientTime(interval, timeZoneId, true);
			else
				return interval;
		}
		protected virtual void SubscribeToControlEvents() {
			control.InnerControl.StorageChanged += new EventHandler(OnStorageChanged);
#if !SL
			optionsCustomization.Changed += new EventHandler(OnOptionsCustomizationChanged);
#endif
		}
		protected virtual void UsubscribeToControlEvents() {
			control.InnerControl.StorageChanged -= new EventHandler(OnStorageChanged);
#if !SL
			optionsCustomization.Changed += new EventHandler(OnOptionsCustomizationChanged);
#endif
		}
		void OnOptionsCustomizationChanged(object sender, EventArgs e) {
			CanOpenEditAppointmentForm = CanEditAppointment();
		}
		protected internal virtual bool CanEditAppointment() {
			return this.lbReminders.SelectedItems.Count == 1 && CanEditAppointmentCore(GetSelectedAppointment());
		}
		protected virtual List<SnoozeItem> CreateSnoozeList(DateTime minAppointmentTime) {
			List<SnoozeItem> result = new List<SnoozeItem>();
			DateTime now = DateTime.Now;
			int count = ReminderTimeSpans.BeforeStartTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan spanToSnooze = ReminderTimeSpans.BeforeStartTimeSpanValues[i];
				if (minAppointmentTime + spanToSnooze <= now)
					continue;
				string timeSpan = HumanReadableTimeSpanHelper.ToString(spanToSnooze);
				timeSpan = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_TimeBeforeStart), timeSpan);
				result.Add(new SnoozeItem(timeSpan, spanToSnooze));
			}
			int activeSelectedIndex = result.Count;
			TimeSpan[] availableSnoozeTimes = ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues;
			count = availableSnoozeTimes.Length;
			for (int i = 0; i < count; i++)
				result.Add(new SnoozeItem(HumanReadableTimeSpanHelper.ToString(availableSnoozeTimes[i]), availableSnoozeTimes[i]));
			if (activeSelectedIndex < result.Count)
				SnoozeValue = result[activeSelectedIndex].TimeSpan;
			return result;
		}
		public override void OnReminderAlert(ReminderAlertNotificationCollection notifications) {
			InsertRemindersItems(notifications);
		}
		protected internal virtual void InsertRemindersItems(ReminderAlertNotificationCollection notifications) {
			ClearObsoleteReminders(notifications);
			int count = notifications.Count;
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++) {
				ReminderAlertNotification notification = notifications[i];
				notification.Handled = true;
				Reminder reminder = notification.Reminder;
				SubscribeReminderEvents(reminder);
				Alerts.Add(notification);
			}
			UpdateSnoozeList();
		}
		protected void SetFirstReminderSelected() {
			this.lbReminders.SelectedIndex = 0;
		}
		protected internal virtual bool PerformRemindersFormDefaultAction() {
			RemindersFormDefaultAction action = CalculateRemindersFormDefaultAction();
			switch (action) {
				default:
				case RemindersFormDefaultAction.DismissAll:
					DismissAll();
					return true;
				case RemindersFormDefaultAction.SnoozeAll:
					SnoozeAll();
					return true;
				case RemindersFormDefaultAction.Custom:
					return RaiseRemindersFormDefaultActionEvent();
			}
		}
		protected internal virtual bool CanEditAppointmentCore(Appointment apt) {
			if (Control == null)
				return false;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			return helper.CanEditAppointment(apt);
		}
		protected internal virtual RemindersFormDefaultAction CalculateRemindersFormDefaultAction() {
			if (Control != null)
				return Control.OptionsBehavior.RemindersFormDefaultAction;
			return RemindersFormDefaultAction.DismissAll;
		}
		protected internal virtual bool RaiseRemindersFormDefaultActionEvent() {
			if (Control == null) {
				DismissAll();
				return true;
			}
			ReminderAlertNotificationCollection alertNotifications = CreateAlertNotifications();
			if (alertNotifications.Count <= 0)
				return true;
			RemindersFormDefaultActionEventArgs args = new RemindersFormDefaultActionEventArgs(alertNotifications);
			Control.InnerControl.RaiseRemindersFormDefaultAction(args);
			if (!args.Handled) {
				DismissAll();
				return true;
			}
			else
				return !args.Cancel;
		}
		protected internal virtual ReminderAlertNotificationCollection CreateAlertNotifications() {
			ReminderAlertNotificationCollection result = new ReminderAlertNotificationCollection();
			int count = Alerts.Count;
			for (int i = 0; i < count; i++) {
				ReminderAlertNotification notification = GetReminderItem(i);
				result.Add(new ReminderAlertNotification(notification.Reminder, notification.ActualAppointment));
			}
			return result;
		}
		protected internal virtual void ForAllItems(Action<Reminder> handler) {
			UnsubscribeStorageEvents();
			try {
				storage.BeginUpdate();
				try {
					GetReminders().ForEach(handler);
					Alerts.Clear();
				}
				finally {
					storage.EndUpdate();
				}
			}
			finally {
				SubscribeStorageEvents();
			}
		}
		protected internal virtual void SubscribeStorageEvents() {
			storage.AppointmentsChanged += OnAppointmentsChanged;
			storage.AppointmentsDeleted += OnAppointmentsDeleted;
			storage.AppointmentCollectionCleared += OnStorageChanged;
			storage.AppointmentCollectionLoaded += OnStorageChanged;
		}
		protected internal virtual void UnsubscribeStorageEvents() {
			storage.AppointmentsDeleted -= OnAppointmentsDeleted;
			storage.AppointmentsChanged -= OnAppointmentsChanged;
			storage.AppointmentCollectionCleared -= OnStorageChanged;
			storage.AppointmentCollectionLoaded -= OnStorageChanged;
		}
		protected internal virtual void OnAppointmentsDeleted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentDeleted);
		}
		protected internal virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentChanged);
		}
		protected internal virtual void OnAppointmentsChangedCore(IList changedObjects, Action<Appointment> handler) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(changedObjects);
			appointments.ForEach(handler);
			Control.Storage.TriggerAlerts();
			Dispatcher.BeginInvoke(new Action(CloseIfEmpty));
		}
		protected internal virtual void OnAppointmentDeleted(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentDeletedHandleItemIndex);
		}
		protected internal virtual void OnAppointmentDeletedHandleItemIndex(int itemIndex) {
			Reminder reminder = GetReminder(itemIndex);
			UnsubscribeReminderEvents(reminder);
			Alerts.RemoveAt(itemIndex);
		}
		protected internal virtual void OnAppointmentChanged(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentChangedHandleItemIndex);
		}
		protected internal virtual void OnAppointmentChangedHandleItemIndex(int itemIndex) {
			ReminderAlertNotification alertNotification = GetReminderItem(itemIndex);
			Reminder reminder = alertNotification.Reminder;
			if (!reminder.Appointment.Reminders.Contains(reminder)) {
				UnsubscribeReminderEvents(reminder);
				Alerts.RemoveAt(itemIndex);
			}
		}
		void ClearObsoleteReminders(ReminderAlertNotificationCollection notifications) {
			int count = Alerts.Count;
			for (int i = count - 1; i >= 0; i--) {
				CheckObsoleteReminder(i, notifications);
			}
		}
		void CheckObsoleteReminder(int reminderIndex, ReminderAlertNotificationCollection newNotifications) {
			ReminderAlertNotification alertNotification = GetReminderItem(reminderIndex);
			Reminder reminder = alertNotification.Reminder;
			Appointment appointment = alertNotification.ActualAppointment;
			if (reminder.Appointment.Type != AppointmentType.Pattern)
				return;
			int count = newNotifications.Count;
			for (int i = 0; i < count; i++) {
				ReminderAlertNotification notification = newNotifications[i];
				if (notification.ActualAppointment.IsRecurring && appointment.RecurrenceIndex == notification.ActualAppointment.RecurrenceIndex) {
					UnsubscribeReminderEvents(reminder);
					Alerts.RemoveAt(reminderIndex);
				}
			}
		}
		protected internal virtual List<int> FindAppointmentRemindersIndexes(Appointment apt) {
			List<int> result = new List<int>();
			int count = Alerts.Count;
			for (int i = count - 1; i >= 0; i--) {
				Reminder reminder = GetReminder(i);
				if (reminder.Appointment == apt)
					result.Add(i);
			}
			return result;
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			if (storage != null)
				UnsubscribeStorageEvents();
			GetReminders().ForEach(UnsubscribeReminderEvents);
			storage = Control.Storage;
			if (storage != null)
				SubscribeStorageEvents();
			Alerts.Clear();
			Control.Storage.TriggerAlerts();
			Dispatcher.BeginInvoke(new Action(CloseIfEmpty));
		}
		protected internal virtual ReminderCollection GetReminders() {
			ReminderCollection result = new ReminderCollection();
			int count = Alerts.Count;
			for (int i = 0; i < count; i++)
				result.Add(GetReminder(i));
			return result;
		}
		protected internal virtual Reminder GetReminder(int index) {
			return GetReminderItem(index).Reminder;
		}
		protected internal virtual ReminderAlertNotification GetReminderItem(int index) {
			return Alerts[index].Notification as ReminderAlertNotification;
		}
		protected internal virtual void SnoozeAll() {
			ForAllItems(SnoozeReminder);
		}
		protected internal virtual void DismissAll() {
			ForAllItems(DismissReminder);
		}
		protected internal virtual void SnoozeReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			SnoozeReminderCore(reminder, SnoozeValue);
		}
		protected internal virtual void SnoozeReminderCore(Reminder reminder, TimeSpan span) {
			if (span.Ticks <= 0) {
				if (-span.Ticks > reminder.Appointment.Start.Ticks)
					reminder.AlertTime = DateTime.MinValue;
				else
					reminder.AlertTime = reminder.Appointment.Start + span;
			}
			else
				reminder.AlertTime = DateTime.Now + span;
			(reminder).Suspended = false;
		}
		protected internal virtual void DismissReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			if (!reminder.Dismiss())
				(reminder).Suspended = false;
		}
		protected internal virtual void DoSnooze() {
			ForAllSelectedItems(SnoozeReminder);
		}
		protected internal virtual void DoDismiss() {
			ForAllSelectedItems(DismissReminder);
		}
		protected internal virtual void ForAllSelectedItems(Action<Reminder> handler) {
			UnsubscribeStorageEvents();
			try {
				storage.BeginUpdate();
				try {
					GetSelectedReminders().ForEach(handler);
					RemoveSelectedItems();
				}
				finally {
					storage.EndUpdate();
				}
			}
			finally {
				SubscribeStorageEvents();
			}
		}
		protected internal virtual ReminderCollection GetSelectedReminders() {
			ReminderCollection result = new ReminderCollection();
			int count = this.lbReminders.SelectedItems.Count;
			for (int i = 0; i < count; i++) {
				ReminderInfo selectedReminderInfo = (ReminderInfo)this.lbReminders.SelectedItems[i];
				result.Add(selectedReminderInfo.Notification.Reminder);
			}
			return result;
		}
		protected internal virtual List<ReminderInfo> GetSelectedAlerts() {
			List<ReminderInfo> result = new List<ReminderInfo>();
			int count = this.lbReminders.SelectedItems.Count;
			for (int i = 0; i < count; i++)
				result.Add(((ReminderInfo)this.lbReminders.SelectedItems[i]));
			return result;
		}
		protected internal virtual void RemoveSelectedItems() {
			List<ReminderInfo> selectedAlerts = GetSelectedAlerts();
			int count = selectedAlerts.Count;
			for (int i = 0; i < count; i++)
				Alerts.Remove(selectedAlerts[i]);
			SetFirstReminderSelected();
		}
		protected internal virtual void SubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged += new EventHandler(OnReminderChanged);
		}
		protected internal virtual void UnsubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged -= new EventHandler(OnReminderChanged);
		}
		protected internal virtual void OnReminderChanged(object sender, EventArgs e) {
			Reminder changedReminder = (Reminder)sender;
			int count = Alerts.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = GetReminder(i);
				if (changedReminder == reminder) {
					UnsubscribeReminderEvents(reminder);
					Alerts.RemoveAt(i);
					CloseIfEmpty();
					break;
				}
			}
		}
		protected internal virtual void CloseIfEmpty() {
			if (Alerts.Count > 0)
				return;
			Close();
		}
		protected internal virtual void Close() {
			if (Alerts.Count <= 0)
				SchedulerFormBehavior.Close(this, false);
		}
		private void DismissAll_Click(object sender, RoutedEventArgs e) {
			DismissAll();
			CloseIfEmpty();
		}
		private void OpenItem_Click(object sender, RoutedEventArgs e) {
			OpenItem();
		}
		protected internal virtual void OpenItem() {
			ReminderInfo reminderInfo = (ReminderInfo)this.lbReminders.SelectedItem;
			Control.ShowEditAppointmentForm(reminderInfo.Notification.ActualAppointment, false, CommandSourceType.Unknown);
		}
		private void Snooze_Click(object sender, RoutedEventArgs e) {
			DoSnooze();
			CloseIfEmpty();
		}
		private void Dismiss_Click(object sender, RoutedEventArgs e) {
			DoDismiss();
			CloseIfEmpty();
		}
#if !SL
		private void OnLbRemindersMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
				OpenItem();
		}
#endif
	}
	public class ObservableReminderAlertNotificationCollection : ObservableCollection<ReminderAlertNotification> {
	}
	public class ObservableReminderInfoCollection : ObservableCollection<ReminderInfo> {
		public void Add(ReminderAlertNotification notification) {
			ReminderInfo reminderInfo = new ReminderInfo(notification);
			base.Add(reminderInfo);
		}
	}
}
