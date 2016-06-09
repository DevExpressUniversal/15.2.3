#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.ExpressApp.Scheduler {
	public class NotificationsProvider : IDisposable, INotificationsProvider {
		private ISchedulerStorageBase schedulerStorage;
		private XafApplication application;
		protected IList<INotificationItem> items = new List<INotificationItem>();
		protected IDictionary<Type, IObjectSpace> eventObjectSpaces;
		private void schedulerStorage_ReminderAlert(object sender, ReminderEventArgs e) {
			schedulerStorage.EnableReminders = false;
			foreach(ReminderAlertNotification item in e.AlertNotifications) {
				items.Add(new ReminderWrapper(item.Reminder, schedulerStorage));
				item.Handled = true;
			}
			schedulerStorage.EnableReminders = true;
		}
		private ReminderBaseCollection GetReminders() {
			ReminderBaseCollection reminders = new ReminderBaseCollection();
			IAppointmentStorageBase Appointments = this.schedulerStorage.Appointments;
			for(int i = 0; i < Appointments.Count; i++) {
				if(Appointments[i].Type == AppointmentType.Pattern) {
					Appointment pattern = Appointments[i];
					AppointmentBaseCollection exceptions = pattern.GetExceptions();
					for(int exceptionIndex = 0; exceptionIndex < exceptions.Count; exceptionIndex++) {
						if(exceptions[exceptionIndex].Type != AppointmentType.DeletedOccurrence) {
							reminders.AddRange(exceptions[exceptionIndex].Reminders);
						}
					}
				}
				reminders.AddRange(Appointments[i].Reminders);
			}
			return reminders;
		}
		private void CreateAppointmentMapping() {
			IAppointmentStorageBase appointments = this.schedulerStorage.Appointments;
			AppointmentMappingInfo appointmentMappingInfo = ((IAppointmentMappingsProvider)appointments).Mappings;
			appointmentMappingInfo.Subject = "Subject";
			appointmentMappingInfo.AllDay = "AllDay";
			appointmentMappingInfo.Start = "StartOn";
			appointmentMappingInfo.End = "EndOn";
			appointmentMappingInfo.Location = "Location";
			appointmentMappingInfo.Label = "Label";
			appointmentMappingInfo.Status = "Status";
			appointmentMappingInfo.Description = "Description";
			appointmentMappingInfo.Type = "Type";
			appointmentMappingInfo.ResourceId = "ResourceId";
			appointmentMappingInfo.AppointmentId = "AppointmentId";
			appointmentMappingInfo.RecurrenceInfo = "RecurrenceInfoXml";
			appointmentMappingInfo.ReminderInfo = "ReminderInfoXml";
		}
		private CriteriaOperator GetCustomNotificationsCollectionCriteria(Type type) {
			CustomizeCollectionCriteriaEventArgs args = new CustomizeCollectionCriteriaEventArgs(type);
			if(CustomizeNotificationCollectionCriteria != null) {
				CustomizeNotificationCollectionCriteria(this, args);
			}
			return args.Criteria;
		}
		protected virtual object GetEventsSource() {
			IList allEvents = new ArrayList();
			foreach(ITypeInfo typeInfo in NotificationTypesInfo) {
				if(!typeInfo.Implements<IReminderEvent>() || !typeInfo.Implements<IEvent>()) {
					continue;
				}
				IObjectSpace objectSpace;
				if(eventObjectSpaces.TryGetValue(typeInfo.Type, out objectSpace)) {
					objectSpace.Refresh();
				}
				else {
					objectSpace = application.CreateObjectSpace(typeInfo.Type);
					eventObjectSpaces.Add(typeInfo.Type, objectSpace);
				}
				IList currentEventTypeCollection = objectSpace.CreateCollection(typeInfo.Type, GetCustomNotificationsCollectionCriteria(typeInfo.Type));
				if(currentEventTypeCollection != null) {
					allEvents = CollectionsHelper.MergeCollections(currentEventTypeCollection, allEvents);
				}
			}
			return allEvents;
		}
		protected virtual void Dismiss(INotificationItem item) {
			Type notificationSourceType = XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)item).NotificationSource.GetType()).Type;
			((ReminderWrapper)item).Dismiss();
			item.NotificationSource.IsPostponed = false;
		}
		protected virtual void Postpone(INotificationItem itemToPostpone, TimeSpan postponeTime) {
			Type notificationSourceType = XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)itemToPostpone).NotificationSource.GetType()).Type;
			((ReminderWrapper)itemToPostpone).Postpone(DateTime.Now, postponeTime);
			itemToPostpone.NotificationSource.IsPostponed = true;
		}
		protected IList<INotificationItem> GetPostponedNotificationItems() {
			IEnumerable<INotificationItem> notificationItems = GetReminders()
				.Select(x => { return new ReminderWrapper(x as Reminder, schedulerStorage); })
				.Where(x => x.NotificationSource.IsPostponed == true && x.NotificationSource.AlarmTime > DateTime.Now);
			return notificationItems.ToList();
		}
		protected IList<INotificationItem> GetActiveNotificationItems() {
			lock(items) {
				items.Clear();
				schedulerStorage.TriggerAlerts(DateTime.Now);
				return items;
			}
		}
		public NotificationsProvider(XafApplication application, HashSet<ITypeInfo> eventsType) {
			Guard.ArgumentNotNull(application, "application");
			Guard.ArgumentNotNull(eventsType, "eventsType");
			eventObjectSpaces = new Dictionary<Type, IObjectSpace>();
			this.application = application;
			NotificationTypesInfo = eventsType;
		}
		public void SetSchedulerStorage(SchedulerStorageBase schedulerStorage) {
			Guard.ArgumentNotNull(schedulerStorage, "schedulerStorage");
			if(this.schedulerStorage != null) {
				this.schedulerStorage.ReminderAlert -= schedulerStorage_ReminderAlert;
			}
			this.schedulerStorage = schedulerStorage;
			this.schedulerStorage.EnableReminders = true;
			schedulerStorage.ReminderAlert += schedulerStorage_ReminderAlert;
			CreateAppointmentMapping();
		}
		public void Dispose() {
			if(schedulerStorage != null) {
				schedulerStorage.ReminderAlert -= schedulerStorage_ReminderAlert;
				schedulerStorage = null;
			}
			CustomizeNotificationCollectionCriteria = null;
			if(items != null) {
				items.Clear();
				items = null;
			}
			if(NotificationTypesInfo != null) {
				NotificationTypesInfo.Clear();
				NotificationTypesInfo = null;
			}
			if(eventObjectSpaces != null) {
				foreach(IObjectSpace objectSpace in eventObjectSpaces.Values) {
					objectSpace.Dispose();
				}
				eventObjectSpaces.Clear();
				eventObjectSpaces = null;
			}
		}
		public IList<INotificationItem> GetNotificationItems() {
			IList<INotificationItem> activeReminders = new List<INotificationItem>();
			IList<INotificationItem> postponedReminders = new List<INotificationItem>();
			if(schedulerStorage != null) {
				this.schedulerStorage.Appointments.DataSource = GetEventsSource();
				activeReminders = GetActiveNotificationItems();
				postponedReminders = GetPostponedNotificationItems();
			}
			return CollectionsHelper.MergeCollections(activeReminders, postponedReminders);
		}
		public int GetActiveNotificationsCount() {
			this.schedulerStorage.Appointments.DataSource = GetEventsSource();
			return GetActiveNotificationItems().Count; 
		}
		public int GetPostponedNotificationsCount() {
			int result = 0;
			CriteriaOperator notActiveCriteria = CriteriaOperator.Or(new NullOperator("AlarmTime"), new BinaryOperator("AlarmTime", DateTime.Now, BinaryOperatorType.Greater));
			CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IsPostponed", true), notActiveCriteria);
			if(NotificationTypesInfo != null) {
				foreach(ITypeInfo typeInfo in NotificationTypesInfo) {
					if(typeInfo.Implements<ISupportNotifications>()) {
						using(IObjectSpace objectSpace = application.CreateObjectSpace()) {
							CriteriaOperator resultCriteria = CriteriaOperator.And(criteria, GetCustomNotificationsCollectionCriteria(typeInfo.Type));
							result += objectSpace.GetObjectsCount(typeInfo.Type, resultCriteria);
						}
					}
				}
			}
			return result;
		}
		public void Dismiss(IEnumerable<INotificationItem> notificationItems) {
			IList<IObjectSpace> objectSpacesToCommit = new List<IObjectSpace>();
			foreach(INotificationItem notificationItem in notificationItems) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(notificationItem.NotificationSource.GetType());
				if(schedulerStorage != null && typeInfo.Implements<IReminderEvent>() && NotificationTypesInfo.Contains(typeInfo)) {
					Dismiss(notificationItem);
					if(eventObjectSpaces.Keys.Contains(XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)notificationItem).NotificationSource.GetType()).Type)) {
						objectSpacesToCommit.Add(eventObjectSpaces[XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)notificationItem).NotificationSource.GetType()).Type]);
					}
				}
			}
			foreach(IObjectSpace objectspace in objectSpacesToCommit) {
				objectspace.CommitChanges();
			}
		}
		public void Postpone(IEnumerable<INotificationItem> notificationItems, TimeSpan postponeTime) {
			IList<IObjectSpace> objectSpacesToCommit = new List<IObjectSpace>();
			foreach(INotificationItem notificationItem in notificationItems) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(notificationItem.NotificationSource.GetType());
				if(schedulerStorage != null && typeInfo.Implements<IReminderEvent>() && NotificationTypesInfo.Contains(typeInfo)) {
					Postpone(notificationItem, postponeTime);
					if(eventObjectSpaces.Keys.Contains(XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)notificationItem).NotificationSource.GetType()).Type)) {
						objectSpacesToCommit.Add(eventObjectSpaces[XafTypesInfo.Instance.FindTypeInfo(((ReminderWrapper)notificationItem).NotificationSource.GetType()).Type]);
					}
				}
			}
			foreach(IObjectSpace objectspace in objectSpacesToCommit) {
				objectspace.CommitChanges();
			}
		}
		public HashSet<ITypeInfo> NotificationTypesInfo {
			get;
			private set;
		}
		public event EventHandler<CustomizeCollectionCriteriaEventArgs> CustomizeNotificationCollectionCriteria;
	}
	[DomainComponent]
	public class ReminderWrapper : INotificationItem {
		protected internal Reminder reminder;
		protected internal ISchedulerStorageBase schedulerStorage;
		public ReminderWrapper(Reminder reminder, ISchedulerStorageBase schedulerStorage) {
			Guard.ArgumentNotNull(reminder, "reminder");
			Guard.ArgumentNotNull(schedulerStorage, "schedulerStorage");
			this.reminder = reminder;
			this.schedulerStorage = schedulerStorage;
		}
		public ISupportNotifications NotificationSource {
			get {
				return this.reminder.Appointment.GetSourceObject(schedulerStorage) as ISupportNotifications;
			}
		}
		public void Dismiss() {
			if(reminder != null) {
				this.reminder.Dismiss();
			}
		}
		public void Postpone(DateTime now, TimeSpan time) {
			if(reminder != null) {
				this.reminder.Snooze(now, time);
			}
		}
	}
}
