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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Security;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.btnDismissAll")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.btnDismiss")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.btnOpenItem")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.lblSnooze")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.cbSnooze")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.btnSnooze")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.lbItems")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.lblSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RemindersForm.lblStart")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	#region RemindersForm
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class RemindersForm : DevExpress.XtraEditors.XtraForm {
		#region Fields
		bool isClosingInternal;
		SchedulerControl control;
		ISchedulerStorage storage;
		SchedulerOptionsCustomization optionsCustomization;
		TimeZoneHelper timeZoneEngine;
		Timer snoozeListUpdateTimer;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RemindersForm() {
			InitializeComponent();
		}
		public RemindersForm(SchedulerStorage storage) {
			SetSchedulerStorage(storage);
			InitializeComponent();
			InitializeFormControls();
			InitializesnoozeListUpdateTimer();
		}
		public RemindersForm(SchedulerControl control) {
			SetSchedulerControl(control);
			SetSchedulerStorage(control.DataStorage);
			InitializeComponent();
			InitializeFormControls();
			InitializesnoozeListUpdateTimer();
		}
		#region Properties
		protected internal bool HasReminders { get { return lbItems.Items.Count > 0 && lbItems.SelectedIndices.Count > 0; } }
		protected internal SchedulerControl SchedulerControl { get { return control; } }
		protected internal ISchedulerStorage Storage { get { return storage; } }
		protected internal SchedulerOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		internal bool IsClosingInternal { get { return isClosingInternal; } set { isClosingInternal = value; } }
		protected internal bool IsClosing { get { return IsClosingInternal; } }
		protected internal virtual DateTime Now {
			get {
#if DEBUGTEST                
				return DevExpress.XtraScheduler.Tests.TestEnvironment.GetNowTime();
#else
				return DateTime.Now;
#endif
			}
		}
		protected internal TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } set { timeZoneEngine = value; } }
		#endregion
		void InitializesnoozeListUpdateTimer() {
			this.snoozeListUpdateTimer = new Timer();
		}
		internal virtual void SetupUpdateSnoozeListTimer(DateTime minDateTime) {
			this.snoozeListUpdateTimer.Stop();
			if (minDateTime == DateTime.MaxValue)
				return;
			int count = this.cbSnooze.Properties.Items.Count;
			TimeSpan minTimeSpan = TimeSpan.MaxValue;
			for (int i = 0; i < count; i++) {
				ImageComboBoxItem item = this.cbSnooze.Properties.Items[i] as ImageComboBoxItem;
				TimeSpan itemTimeSpan = (TimeSpan)item.Value;
				if (itemTimeSpan.Ticks <= 0)
					minTimeSpan = DateTimeHelper.Min(itemTimeSpan, minTimeSpan);
			}
			if (minTimeSpan.Ticks > 0)
				return;
			TimeSpan spanToUpdate = (minDateTime + minTimeSpan) - Now;
			if (spanToUpdate.Ticks < 0)
				return;
			this.snoozeListUpdateTimer.Interval = int.MaxValue;
			double totalMilliseconds = spanToUpdate.TotalMilliseconds;
			int timerInterval = (totalMilliseconds < int.MaxValue) ? (int)totalMilliseconds : int.MaxValue;
			StartSnoozeListUpdateTimer(timerInterval > 0 ? timerInterval : 1);
		}
		internal virtual void StartSnoozeListUpdateTimer(int timerInterval) {
			System.Diagnostics.Debug.Assert(timerInterval > 0);
			this.snoozeListUpdateTimer.Interval = timerInterval;
			this.snoozeListUpdateTimer.Tick += OnSnoozeListUpdateTimerTick;
			this.snoozeListUpdateTimer.Start();
		}
		void OnSnoozeListUpdateTimerTick(object sender, EventArgs e) {
			UpdateSnoozeList();
		}
		void UpdateSnoozeList() {
			DateTime minDateTime = DateTime.MaxValue;
			List<ReminderItem> reminderItems = GetSelectedReminderItems();
			int count = reminderItems.Count;
			for (int i = 0; i < count; i++) {
				Appointment appointment = reminderItems[i].ActualAppointment;
				minDateTime = DateTimeHelper.Min(appointment.Start, minDateTime);
			}
			FillSnoozeCombo(minDateTime);
			SetupUpdateSnoozeListTimer(minDateTime);
		}
		void DisposeTimer() {
			if (this.snoozeListUpdateTimer == null)
				return;
			this.snoozeListUpdateTimer.Stop();
			this.snoozeListUpdateTimer.Dispose();
			this.snoozeListUpdateTimer = null;
		}
		protected internal virtual void SetSchedulerControl(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.optionsCustomization = control.OptionsCustomization;
			this.timeZoneEngine = control.InnerControl.TimeZoneHelper;
			SubscribeControlEvents();
		}
		protected internal virtual void SetSchedulerStorage(ISchedulerStorage storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
			SubscribeStorageEvents();
		}
		protected internal virtual void InitializeFormControls() {
			SubscribeSelectionChanged();
			UpdateIcon();
			FillSnoozeCombo(DateTime.MaxValue);
		}
		protected internal virtual void SubscribeControlEvents() {
			control.StorageChanged += OnStorageChanged;
			control.BeforeDispose += OnBeforeControlDispose;
			optionsCustomization.Changed += OnControlOptionsCustomizationChanged;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			control.StorageChanged -= OnStorageChanged;
			control.BeforeDispose -= OnBeforeControlDispose;
			optionsCustomization.Changed -= OnControlOptionsCustomizationChanged;
		}
		protected internal virtual void SubscribeStorageEvents() {
			((IInternalSchedulerStorageBase)storage).BeforeDispose += OnBeforeStorageDispose;
			storage.AppointmentsChanged += OnAppointmentsChanged;
			storage.AppointmentsDeleted += OnAppointmentsDeleted;
			storage.AppointmentCollectionCleared += OnAppointmentCollectionReloaded;
			storage.AppointmentCollectionLoaded += OnAppointmentCollectionReloaded;
		}
		protected internal virtual void UnsubscribeStorageEvents() {
			((IInternalSchedulerStorageBase)storage).BeforeDispose -= OnBeforeStorageDispose;
			storage.AppointmentsDeleted -= OnAppointmentsDeleted;
			storage.AppointmentsChanged -= OnAppointmentsChanged;
			storage.AppointmentCollectionCleared -= OnAppointmentCollectionReloaded;
			storage.AppointmentCollectionLoaded -= OnAppointmentCollectionReloaded;
		}
		protected internal virtual void OnBeforeControlDispose(object sender, EventArgs e) {
			UnsubscribeControlEvents();
			optionsCustomization = null;
			control = null;
		}
		protected internal virtual void OnControlOptionsCustomizationChanged(object sender, BaseOptionChangedEventArgs e) {
			UpdateControls();
		}
		protected internal virtual void OnBeforeStorageDispose(object sender, EventArgs e) {
			if (storage != null)
				UnsubscribeStorageEvents();
			if (control != null)
				UnsubscribeControlEvents();
			storage = null;
			control = null;
			optionsCustomization = null;
			ClearRemindersAndCloseIfEmpty();
		}
		protected internal virtual void OnAppointmentCollectionReloaded(object sender, EventArgs e) {
			GetReminders().ForEach(UnsubscribeReminderEvents);
			ClearRemindersAndCloseIfEmpty();
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			if (storage != null)
				UnsubscribeStorageEvents();
			GetReminders().ForEach(UnsubscribeReminderEvents);
			XtraSchedulerDebug.Assert(SchedulerControl != null);
			storage = SchedulerControl.DataStorage;
			if (storage != null)
				SubscribeStorageEvents();  
			ClearRemindersAndCloseIfEmpty();
		}
		void ClearRemindersAndCloseIfEmpty() {
			lbItems.Items.Clear();
			CloseIfEmpty();
		}
		protected internal virtual void UpdateIcon() {
			this.Icon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Reminder, Assembly.GetExecutingAssembly());
		}
		protected virtual void FillSnoozeCombo(DateTime minAppointmentTime) {
			this.cbSnooze.Properties.Items.Clear();
			DateTime now = Now;
			int count = ReminderTimeSpans.BeforeStartTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan spanToSnooze = ReminderTimeSpans.BeforeStartTimeSpanValues[i];
				if (minAppointmentTime + spanToSnooze <= now)
					continue;
				string timeSpan = HumanReadableTimeSpanHelper.ToString(spanToSnooze);
				timeSpan = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_TimeBeforeStart), timeSpan);
				this.cbSnooze.Properties.Items.Add(new ImageComboBoxItem(timeSpan, spanToSnooze));
			}
			TimeSpan[] availableReminderSnoozeValues = ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues;
			count = availableReminderSnoozeValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = availableReminderSnoozeValues[i];
				this.cbSnooze.Properties.Items.Add(new ImageComboBoxItem(HumanReadableTimeSpanHelper.ToString(timeSpan), timeSpan));
			}
		}
		protected internal virtual Reminder GetReminder(int index) {
			return GetReminderItem(index).Reminder;
		}
		protected internal virtual ReminderItem GetReminderItem(int index) {
			ImageListBoxItem item = lbItems.Items[index] as ImageListBoxItem;
			ReminderItem reminderItem = (ReminderItem)item.Value;
			return reminderItem;
		}
		protected internal virtual ReminderCollection GetSelectedReminders() {
			ReminderCollection result = new ReminderCollection(DXCollectionUniquenessProviderType.None);
			int count = lbItems.SelectedIndices.Count;
			for (int i = 0; i < count; i++)
				result.Add(GetReminder(lbItems.SelectedIndices[i]));
			return result;
		}
		protected internal virtual List<ReminderItem> GetSelectedReminderItems() {
			List<ReminderItem> result = new List<ReminderItem>();
			int count = lbItems.SelectedIndices.Count;
			for (int i = 0; i < count; i++)
				result.Add(GetReminderItem(lbItems.SelectedIndices[i]));
			return result;
		}
		protected internal virtual ReminderCollection GetReminders() {
			ReminderCollection result = new ReminderCollection(DXCollectionUniquenessProviderType.None);
			int count = lbItems.Items.Count;
			for (int i = 0; i < count; i++)
				result.Add(GetReminder(i));
			return result;
		}
		protected internal virtual List<ReminderItem> GetReminderItems() {
			List<ReminderItem> result = new List<ReminderItem>();
			int count = lbItems.Items.Count;
			for (int i = 0; i < count; i++)
				result.Add(GetReminderItem(i));
			return result;
		}
		protected internal virtual void UpdateCaptions() {
			int count = lbItems.Items.Count;
			string format = (count == 1) ? SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminder) : SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminders);
			this.Text = String.Format(format, count);
			List<ReminderItem> reminderItems = GetSelectedReminderItems();
			count = reminderItems.Count;
			if (count > 0) { 
				if (count == 1) {
					ReminderItem reminderItem = reminderItems[0];
					Reminder rem = reminderItem.Reminder;
					lblSubject.Text = rem.Subject;
					format = SchedulerLocalizer.GetString(SchedulerStringId.Caption_StartTime);
					DateTime aptStart = ToClientTime(reminderItem.ActualAppointment.Start);
					lblStart.Text = String.Format(format, aptStart.ToString());
				} else {
					lblSubject.Text = String.Empty;
					lblStart.Text = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Caption_NAppointmentsAreSelected), count);
				}
			}
		}
		protected internal virtual DateTime ToClientTime(DateTime value) {
			if (TimeZoneHelper != null)
				return TimeZoneHelper.ToClientTime(value);
			else
				return value;
		}
		protected internal virtual void UpdateControls() {
			UpdateCaptions();
			bool hasReminders = HasReminders;
			btnDismiss.Enabled = hasReminders;
			btnDismissAll.Enabled = hasReminders;
			btnSnooze.Enabled = hasReminders;
			cbSnooze.Enabled = hasReminders;
			if (hasReminders && lbItems.SelectedIndices.Count == 1)
				btnOpenItem.Enabled = CanEditAppointment(GetReminder(lbItems.SelectedIndices[0]).Appointment);
			else
				btnOpenItem.Enabled = false;
		}
		protected internal virtual bool CanEditAppointment(Appointment apt) {
			if (SchedulerControl == null)
				return false;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(SchedulerControl.InnerControl);
			return helper.CanEditAppointment(apt);
		}
		protected internal virtual void RemoveSelectedItems() {
			int selectedIndicesCount = lbItems.SelectedIndices.Count;
			ImageListBoxItemCollection items = lbItems.Items;
			List<object> list = new List<object>();
			for (int i = 0; i < selectedIndicesCount; i++) {
				int selIndex = lbItems.SelectedIndices[i];
				list.Add(items[selIndex]);
			}
			items.BeginUpdate();
			try {
				for (int i = 0; i < selectedIndicesCount; i++)
					items.Remove(list[i]);
			} finally {
				items.EndUpdate();
			}
		}
		protected internal virtual void CloseIfEmpty() {
			if (IsClosing)
				return;
			if (lbItems.Items.Count <= 0) {
				DisposeTimer();
				Close();
			}
		}
		protected internal virtual void ForAllSelectedItems(Action<Reminder> handler) {
			UnsubscribeSelectionChanged();
			try {
				UnsubscribeStorageEvents();
				try {
					storage.BeginUpdate();
					try {
						GetSelectedReminders().ForEach(handler);
						RemoveSelectedItems();
						UpdateControls();
					} finally {
						storage.EndUpdate();
					}
				} finally {
					SubscribeStorageEvents();
				}
			} finally {
				SubscribeSelectionChanged();
			}
		}
		protected internal virtual void DoDismiss() {
			ForAllSelectedItems(DismissReminder);
		}
		protected internal virtual void btnDismiss_Click(object sender, System.EventArgs e) {
			OnDismissButton();
		}
		protected internal virtual void OnDismissButton() {
			DoDismiss();
			CloseIfEmpty();
		}
		protected internal virtual void ForAllItems(Action<Reminder> handler) {
			UnsubscribeSelectionChanged();
			try {
				UnsubscribeStorageEvents();
				try {
					storage.BeginUpdate();
					try {
						GetReminders().ForEach(handler);
						lbItems.Items.Clear();
						UpdateControls();
					} finally {
						storage.EndUpdate();
					}
				} finally {
					SubscribeStorageEvents();
				}
			} finally {
				SubscribeSelectionChanged();
			}
		}
		protected internal virtual void DismissAll() {
			ForAllItems(DismissReminder);
		}
		protected internal virtual void btnDismissAll_Click(object sender, EventArgs e) {
			OnDismissAllButton();
		}
		protected internal virtual void OnDismissAllButton() {
			DismissAll();
			CloseIfEmpty();
		}
		protected internal virtual void DismissReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			if (!reminder.Dismiss())
				(reminder).Suspended = false;
		}
		protected internal virtual void SnoozeReminder(Reminder reminder) {
			UnsubscribeReminderEvents(reminder);
			TimeSpan span = (TimeSpan)cbSnooze.EditValue;
			SnoozeReminderCore(reminder, span);
		}
		protected internal virtual void SnoozeReminderCore(Reminder reminder, TimeSpan span) {
			if (span.Ticks <= 0) {
				if (-span.Ticks > reminder.Appointment.Start.Ticks)
					reminder.AlertTime = DateTime.MinValue;
				else
					reminder.AlertTime = reminder.Appointment.Start + span;
			} else
				reminder.AlertTime = Now + span;
			(reminder).Suspended = false;
		}
		protected internal virtual void btnSnooze_Click(object sender, System.EventArgs e) {
			OnSnoozeButton();
		}
		protected internal virtual void SnoozeAll() {
			ForAllItems(SnoozeReminder);
		}
		protected internal virtual void DoSnooze() {
			ForAllSelectedItems(SnoozeReminder);
		}
		protected internal virtual void OnSnoozeButton() {
			DoSnooze();
			CloseIfEmpty();
		}
		protected internal virtual void btnOpenItem_Click(object sender, System.EventArgs e) {
			OnOpenItemButton();
		}
		protected internal virtual void OnOpenItemButton() {
			OpenSelectedItem();
		}
		protected internal virtual void OpenSelectedItem() {
			ReminderCollection selectedReminders = GetSelectedReminders();
			if (selectedReminders.Count <= 0)
				return;
			EditAppointment(selectedReminders[0].Appointment);
		}
		protected internal virtual void EditAppointment(Appointment apt) {
			if (SchedulerControl != null) {
				EditAppointmentQueryCommand command = new EditAppointmentQueryCommand(SchedulerControl.InnerControl);
				command.Execute(apt);
			}
		}
		protected internal virtual void lbItems_SelectedValueChanged(object sender, EventArgs e) {
			UpdateSnoozeList();
			UpdateControls();
		}
		protected internal virtual void RemindersForm_Closing(object sender, CancelEventArgs e) {
			if (storage != null ) {
				this.isClosingInternal = true;
				try {
					e.Cancel = !PerformRemindersFormDefaultAction();
				} finally {
					this.isClosingInternal = false;
				}
			}
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
		protected internal virtual RemindersFormDefaultAction CalculateRemindersFormDefaultAction() {
			if (SchedulerControl != null)
				return SchedulerControl.OptionsBehavior.RemindersFormDefaultAction;
			return RemindersFormDefaultAction.DismissAll;
		}
		protected internal virtual bool RaiseRemindersFormDefaultActionEvent() {
			if (SchedulerControl == null) {
				DismissAll();
				return true;
			}
			List<ReminderItem> reminderItems = GetReminderItems();
			ReminderAlertNotificationCollection alertNotifications = CreateAlertNotifications(reminderItems);
			if (alertNotifications.Count <= 0)
				return true;
			RemindersFormDefaultActionEventArgs args = new RemindersFormDefaultActionEventArgs(alertNotifications);
			SchedulerControl.InnerControl.RaiseRemindersFormDefaultAction(args);
			if (!args.Handled) {
				DismissAll();
				return true;
			} else
				return !args.Cancel;
		}
		protected internal virtual ReminderAlertNotificationCollection CreateAlertNotifications(List<ReminderItem> reminderItems) {
			ReminderAlertNotificationCollection result = new ReminderAlertNotificationCollection();
			int count = reminderItems.Count;
			for (int i = 0; i < count; i++)
				result.Add(new ReminderAlertNotification(reminderItems[i].Reminder, reminderItems[i].ActualAppointment));
			return result;
		}
		[SecuritySafeCritical]
		public virtual void OnReminderAlert(ReminderEventArgs e) {
			InsertRemindersItems(e.AlertNotifications);
			UpdateSnoozeList();
			UpdateControls();
			cbSnooze.EditValue = ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues[0];
			SetParentLookAndFeel();
			DoFlashWindow();
			Show();
		}
		protected virtual void DoFlashWindow() {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.FlashWindowEx(Handle, Utils.Drawing.Helpers.NativeMethods.FLASHW.FLASHW_TIMERNOFG | Utils.Drawing.Helpers.NativeMethods.FLASHW.FLASHW_TRAY, 0, 0);
		}
		protected internal virtual void SetParentLookAndFeel() {
			if (SchedulerControl == null)
				return;
			this.LookAndFeel.ParentLookAndFeel = SchedulerControl.PaintStyle.UserLookAndFeel;
		}
		protected internal virtual void InsertRemindersItems(ReminderAlertNotificationCollection alertNotifications) {
			int count = alertNotifications.Count;
			if (count <= 0)
				return;
			UnsubscribeSelectionChanged();
			try {
				lbItems.SuspendLayout();
				try {
					ImageListBoxItemCollection listBoxItems = lbItems.Items;
					for (int i = 0; i < count; i++) {
						ReminderAlertNotification notification = alertNotifications[i];
						notification.Handled = true;
						Reminder reminder = notification.Reminder;
						SubscribeReminderEvents(reminder);
						ImageListBoxItem item = new ImageListBoxItem(CreateReminderItem(reminder, notification.ActualAppointment));
						listBoxItems.Add(item);
					}
					lbItems.SelectedItem = listBoxItems[listBoxItems.Count - 1];
				} finally {
					lbItems.ResumeLayout();
				}
			} finally {
				SubscribeSelectionChanged();
			}
		}
		bool IsCtrlAPressed(KeyPressEventArgs e) {
			return (e.KeyChar == 'a' || e.KeyChar == 'A') && (Control.ModifierKeys & Keys.Control) != 0;
		}
		private void lbItems_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete)
				OnDismissButton();
		}
		private void lbItems_DoubleClick(object sender, System.EventArgs e) {
			int index = lbItems.IndexFromPoint(lbItems.PointToClient(SchedulerControl.MousePosition));
			if (index >= 0)
				OpenSelectedItem();
		}
		protected internal virtual void SubscribeSelectionChanged() {
			lbItems.SelectedValueChanged += new EventHandler(lbItems_SelectedValueChanged);
		}
		protected internal virtual void UnsubscribeSelectionChanged() {
			lbItems.SelectedValueChanged -= new EventHandler(lbItems_SelectedValueChanged);
		}
		protected internal virtual void SubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged += new EventHandler(OnReminderChanged);
		}
		protected internal virtual void UnsubscribeReminderEvents(Reminder reminder) {
			reminder.RemindTimeChanged -= new EventHandler(OnReminderChanged);
		}
		protected internal virtual void OnReminderChanged(object sender, EventArgs e) {
			Reminder changedReminder = (Reminder)sender;
			int count = lbItems.Items.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = GetReminder(i);
				if (changedReminder == reminder) {
					UnsubscribeReminderEvents(reminder);
					lbItems.Items.RemoveAt(i);
					CloseIfEmpty();
					break;
				}
			}
		}
		protected internal virtual void OnAppointmentsChangedCore(IList changedObjects, Action<Appointment> handler) {
			UnsubscribeSelectionChanged();
			try {
				lbItems.BeginUpdate();
				try {
					AppointmentBaseCollection appointments = new AppointmentBaseCollection();
					appointments.AddRange(changedObjects);
					appointments.ForEach(handler);
					UpdateControls();
				} finally {
					lbItems.EndUpdate();
				}
				Storage.TriggerAlerts(DateTime.Now);
				CloseIfEmpty();
			} finally {
				SubscribeSelectionChanged();
			}
		}
		protected internal virtual void OnAppointmentsDeleted(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentDeleted);
		}
		protected internal virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			OnAppointmentsChangedCore(e.Objects, OnAppointmentChanged);
		}
		protected internal virtual void OnAppointmentDeleted(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentDeletedHandleItemIndex);
		}
		protected internal virtual void OnAppointmentDeletedHandleItemIndex(int itemIndex) {
			Reminder reminder = GetReminder(itemIndex);
			UnsubscribeReminderEvents(reminder);
			lbItems.Items.RemoveAt(itemIndex);
		}
		protected internal virtual void OnAppointmentChanged(Appointment apt) {
			FindAppointmentRemindersIndexes(apt).ForEach(OnAppointmentChangedHandleItemIndex);
		}
		protected internal virtual void OnAppointmentChangedHandleItemIndex(int itemIndex) {
			Reminder reminder = GetReminder(itemIndex);
			if (!reminder.Appointment.Reminders.Contains(reminder)) {
				UnsubscribeReminderEvents(reminder);
				lbItems.Items.RemoveAt(itemIndex);
			}
		}
		protected internal virtual List<int> FindAppointmentRemindersIndexes(Appointment apt) {
			List<int> result = new List<int>();
			for (int i = lbItems.Items.Count - 1; i >= 0; i--) {
				Reminder reminder = GetReminder(i);
				if (reminder.Appointment == apt)
					result.Add(i);
			}
			return result;
		}
		protected internal virtual ReminderItem CreateReminderItem(Reminder reminder, Appointment actualAppointment) {
			return new ReminderItem(reminder, actualAppointment);
		}
	}
	#endregion
	#region ReminderItem
	public class ReminderItem {
		readonly Reminder reminder;
		readonly Appointment actualAppointment;
		public ReminderItem(Reminder reminder, Appointment actualAppointment) {
			Guard.ArgumentNotNull(reminder, "reminder");
			Guard.ArgumentNotNull(actualAppointment, "actualAppointment");
			this.reminder = reminder;
			this.actualAppointment = actualAppointment;
		}
		public Reminder Reminder { get { return reminder; } }
		public Appointment ActualAppointment { get { return actualAppointment; } }
		public override string ToString() {
			return reminder.Subject;
		}
	}
	#endregion
}
