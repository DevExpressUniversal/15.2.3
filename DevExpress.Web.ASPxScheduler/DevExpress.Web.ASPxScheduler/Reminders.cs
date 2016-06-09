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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Collections.Generic;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	#region RemindersFormTemplateContainer
	public class RemindersFormTemplateContainer : SchedulerFormTemplateContainer {
		public const int DefaultWidth = 500;
		public const int DefaultHeight = 150;
		ReminderCollection reminders;
		public RemindersFormTemplateContainer(ASPxScheduler control)
			: base(control) {
			this.reminders = Control.ReminderState.GetExpiredReminders();
			this.LoadPostData(control);
			this.ID = SchedulerIdHelper.RemindersFormTemplateContainerId;
		}
		#region Properties
		public override string CancelScript { get { return String.Format("aspxCancelRemindersForm(\"{0}\")", ControlClientId); } }
		public override string CancelHandler { get { return String.Format("function() {{ {0}; }}", CancelScript); } }
		public virtual string DismissReminderScript { get { return String.Format("ASPx.DismissReminders(\"{0}\")", ControlClientId); } }
		public virtual string DismissReminderHandler { get { return String.Format("function() {{ {0}; }}", DismissReminderScript); } }
		public virtual string DismissAllRemindersScript { get { return String.Format("ASPx.DismissAllReminders(\"{0}\")", ControlClientId); } }
		public virtual string DismissAllRemindersHandler { get { return String.Format("function() {{ {0}; }}", DismissAllRemindersScript); } }
		public virtual string SnoozeRemindersScript { get { return String.Format("ASPx.SnoozeReminders(\"{0}\")", ControlClientId); } }
		public virtual string SnoozeRemindersHandler { get { return String.Format("function() {{ {0}; }}", SnoozeRemindersScript); } }
		public ReminderCollection Reminders { get { return reminders; } }
		public TimeSpan[] SnoozeTimeSpans { get { return GetSnoozeTimeSpans(); } }
		#endregion
		#region CreateCommandEventArgs
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new EventArgs();
		}
		#endregion
		#region GetSnoozeTimeSpans
		TimeSpan[] GetSnoozeTimeSpans() {
			List<TimeSpan> result = new List<TimeSpan>();
			int count = ReminderTimeSpans.BeforeStartTimeSpanValues.Length;
			for(int i = 0; i < count; i++)
				result.Add(ReminderTimeSpans.BeforeStartTimeSpanValues[i]);
			count = ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues.Length;
			for(int i = 0; i < count; i++)
				result.Add(ReminderTimeSpans.ReminderTimeSpanWithoutZeroValues[i]);
			return result.ToArray();
		}
		#endregion
		#region ConvertSnoozeTimeSpanToString
		public string ConvertSnoozeTimeSpanToString(TimeSpan timeSpan) {
			if(timeSpan.Ticks < 0) {
				string result = HumanReadableTimeSpanHelper.ToString(timeSpan);
				return String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Format_TimeBeforeStart), result);
			}
			return HumanReadableTimeSpanHelper.ToString(timeSpan);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region RemindersFormRenderer
	public class RemindersFormRenderer : FormRenderer {
		public RemindersFormRenderer(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal override string FormTemplateUrl { get { return Control.OptionsForms.RemindersFormTemplateUrl; } }
		protected internal override string FormName { get { return SchedulerFormNames.RemindersForm; } }
		protected internal override string FormContainerId { get { return SchedulerIdHelper.RemindersFormContainerId; } }
		public override SchedulerFormVisibility FormContainerVisibility { get { return SchedulerFormVisibility.PopupWindow; } }
		#endregion
		#region CreateFormTemplateContainer
		protected internal override SchedulerFormTemplateContainer CreateFormTemplateContainer() {
			return new RemindersFormTemplateContainer(Control);
		}
		#endregion
		#region CreateFormShowingEventArgs
		protected internal override SchedulerFormEventArgs CreateFormShowingEventArgs(SchedulerFormTemplateContainer container) {
			return new RemindersFormEventArgs((RemindersFormTemplateContainer)container);
		}
		#endregion
		#region RaiseFormShowingEvent
		protected internal override void RaiseFormShowingEvent(SchedulerFormEventArgs args) {
			Control.RaiseReminderFormShowing((RemindersFormEventArgs)args);
		}
		#endregion
		#region BeforeExecute
		protected internal override void BeforeExecute() {
		}
		#endregion
		#region CanContinueExecute
		protected internal override bool CanContinueExecute() {
			if (true )
				return true;
		}
		#endregion
		#region PrepareFormContainerBuilder
		protected internal override void PrepareFormContainerBuilder(FormContainerBuilder formBuilder, SchedulerFormTemplateContainer container) {
			base.PrepareFormContainerBuilder(formBuilder, container);
			PopupFormContainerBuilder builder = formBuilder as PopupFormContainerBuilder;
			if (builder != null) {
				builder.DefaultWidth = RemindersFormTemplateContainer.DefaultWidth;
				builder.DefaultHeight = RemindersFormTemplateContainer.DefaultHeight;
				ReminderCollection expiredReminders = Control.ReminderState.GetExpiredReminders();
				int count = expiredReminders.Count;
				string format = (count == 1) ? SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminder) : SchedulerLocalizer.GetString(SchedulerStringId.Caption_Reminders);
				builder.HeaderText = (String.IsNullOrEmpty(container.Caption)) ? String.Format(format, count) : container.Caption;
			}
		}
		#endregion
		#region RaisePreparePopupContainer
		protected override void RaisePreparePopupContainer(FormContainerBuilder builder) {
			PopupFormBaseContainerBuilder popupBuilder = builder as PopupFormBaseContainerBuilder;
			if (popupBuilder == null)
				return;
			Control.RaisePrepareRemindersFormPopupContainer(popupBuilder.PopupControl);
		}
		#endregion
		protected override System.Web.UI.Control CreateDefaultFormControl() {
			return new DevExpress.Web.ASPxScheduler.Forms.Internal.ReminderForm();
		}
	}
	#endregion
	#region ReminderState
	public class ReminderState {
		public const int DefaultDelay = 1;
		#region Fields
		bool isRequireSetClientTimer;
		ASPxScheduler control;
		List<ReminderClientId> clientReminderIdCollection;
		List<ReminderClientId> clientIgnoredReminderIdCollection;
		#endregion
		public ReminderState(ASPxScheduler control) {
			this.control = control;
			this.isRequireSetClientTimer = true;
			this.clientReminderIdCollection = new List<ReminderClientId>();
			this.clientIgnoredReminderIdCollection = new List<ReminderClientId>();
		}
		#region Properties
		public bool IsRequireSetClientTimer { get { return isRequireSetClientTimer; } set { isRequireSetClientTimer = value; } }
		public ASPxScheduler Control { get { return control; } }
		public List<ReminderClientId> ClientReminderIdCollection { get { return clientReminderIdCollection; } }
		public List<ReminderClientId> ClientIgnoredReminderIdCollection { get { return clientIgnoredReminderIdCollection; } }
		#endregion
		#region LoadExpiredReminders
		public virtual void LoadExpiredReminders() {
			((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.TriggerAlerts();
		}
		#endregion
		#region IsRemindersExists
		public virtual bool IsRemindersExists() {
			if (!((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.Enabled)
				return false;
			ReminderBaseCollection reminders = ((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.Reminders;
			return reminders.Count > 0 ? true : false;
		}
		#endregion
		#region GetOccurredReminders
		internal ReminderCollection GetExpiredReminders() {
			return GetReminders(ClientReminderIdCollection);
		}
		#endregion
		#region GetNearestFutureReminderTime
		internal virtual DateTime GetNearestFutureReminderTime() {
			ReminderBaseCollection reminders = ((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.Reminders;
			int count = reminders.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = (Reminder)reminders[i];
				ReminderClientIdComparer comparer = new ReminderClientIdComparer(reminder, Control);
				if (!ClientIgnoredReminderIdCollection.Exists(comparer.Compare))
					return reminders[i].AlertTime;
			}
			return DateTime.Now.AddMinutes(DefaultDelay);
		}
		#endregion
		#region ToString
		public override string ToString() {
			List<string> result = new List<string>();
			int count = ClientReminderIdCollection.Count;
			for (int i = 0; i < count; i++) {
				ReminderClientId clientId = ClientReminderIdCollection[i];
				result.Add(clientId.ToString());
			}
			return String.Join(",", result.ToArray());
		}
		#endregion
		#region LoadFromString
		public void LoadFromString(string str) {
			Reset();
			ClientReminderIdCollection.AddRange(ParseReminderClientIds(str));
		}
		#endregion
		#region GetReminders
		ReminderCollection GetReminders(List<ReminderClientId> reminderIds) {
			ReminderCollection result = new ReminderCollection();
			ReminderBaseCollection sourceReminders = ((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.Reminders;
			int count = sourceReminders.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = (Reminder)sourceReminders[i];
				if (SearchInClientReminders(reminder, reminderIds))
					result.Add(reminder);
			}
			return result;
		}
		#endregion
		#region ParseReminderClientIds
		List<ReminderClientId> ParseReminderClientIds(string str) {
			List<ReminderClientId> reminderIds = new List<ReminderClientId>();
			if (String.IsNullOrEmpty(str))
				return reminderIds;
			string[] reminderStringIds = str.Split(',');
			int count = reminderStringIds.Length;
			for (int i = 0; i < count; i++) {
				ReminderClientId reminderId = new ReminderClientId(reminderStringIds[i]);
				reminderIds.Add(reminderId);
			}
			return reminderIds;
		}
		#endregion
		#region SearchInClientReminders
		bool SearchInClientReminders(Reminder reminder, List<ReminderClientId> reminderIds) {
			int count = reminderIds.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = reminder.Appointment;
				ReminderClientId reminderClientId = reminderIds[i];
				if (Control.GetAppointmentClientId(apt) == reminderClientId.ClientAppointmentId) {
					if (apt.Reminders.IndexOf(reminder) == reminderClientId.ReminderIndex)
						return true;
				}
			}
			return false;
		}
		#endregion
		#region AddReminder
		public void AddReminder(Reminder reminder) {
			ClientReminderIdCollection.Add(new ReminderClientId(reminder, Control));
		}
		#endregion
		#region AddIgnoredReminder
		public void AddIgnoredReminder(Reminder reminder) {
			ClientIgnoredReminderIdCollection.Add(new ReminderClientId(reminder, Control));
		}
		#endregion
		#region RemoveReminder
		public void RemoveReminder(Reminder reminder) {
			ReminderClientIdComparer comparer = new ReminderClientIdComparer(reminder, Control);
			ReminderClientId reminderClientIdToRemove = ClientReminderIdCollection.Find(comparer.Compare);
			ClientReminderIdCollection.Remove(reminderClientIdToRemove);
		}
		#endregion
		#region Reset
		public void Reset() {
			ClientReminderIdCollection.Clear();
			ClientIgnoredReminderIdCollection.Clear();
		}
		#endregion
	}
	#endregion
	#region ReminderClientId
	public class ReminderClientId {
		#region Fields
		string clientAppointmentId;
		int reminderIndex;
		#endregion
		public ReminderClientId(string id) {
			int separatorIndex = GetSeparatorIndex(id);
			if (separatorIndex == -1)
				return;
			this.clientAppointmentId = id.Substring(0, separatorIndex);
			this.reminderIndex = Int32.Parse(id.Substring(separatorIndex + 1));
		}
		public ReminderClientId(Reminder reminder, ASPxScheduler control) {
			Appointment apt = reminder.Appointment;
			this.reminderIndex = apt.Reminders.IndexOf(reminder);
			this.clientAppointmentId = control.GetAppointmentClientId(apt);
		}
		#region Properties
		public string ClientAppointmentId { get { return clientAppointmentId; } }
		public int ReminderIndex { get { return reminderIndex; } }
		#endregion
		#region GetSeparatorIndex
		int GetSeparatorIndex(string str) {
			int count = str.Length;
			int index = -1;
			for (int i = count - 1; i >= 0; i--) {
				if (str[i] == '_') {
					index = i;
					break;
				}
			}
			return index;
		}
		#endregion
		#region ToString
		public override string ToString() {
			return String.Format("{0}_{1}", ClientAppointmentId, ReminderIndex);
		}
		#endregion
	}
	#endregion
	#region ReminderClientIdComparer
	public class ReminderClientIdComparer {
		ReminderClientId clientId;
		public ReminderClientIdComparer(ReminderClientId clientId) {
			this.clientId = clientId;
		}
		public ReminderClientIdComparer(Reminder reminder, ASPxScheduler control) {
			this.clientId = new ReminderClientId(reminder, control);
		}
		public ReminderClientId ClientId { get { return clientId; } }
		public bool Compare(ReminderClientId item) {
			if (item.ClientAppointmentId == ClientId.ClientAppointmentId && item.ReminderIndex == ClientId.ReminderIndex)
				return true;
			return false;
		}
	}
	#endregion
}
