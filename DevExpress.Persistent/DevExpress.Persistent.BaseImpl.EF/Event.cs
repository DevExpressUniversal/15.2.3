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

using System;
using System.Xml;
using System.Linq;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.ExpressApp.Filtering;
using System.ComponentModel.DataAnnotations;
namespace DevExpress.Persistent.BaseImpl.EF {
	[DefaultProperty("Subject")]
	[NavigationItem("Default")]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	public class Event : IEvent, IRecurrentEvent, IObjectSpaceLink, IXafEntityObject, IReminderEvent, INotifyPropertyChanged, INotifyPropertyChanging {
		private const int NoneReminder = -1;
		private String resourceIds;
		private Boolean isUpdateResourcesDelayed;
		private Boolean allDay;
		private String recurrenceInfoXml;
		private IObjectSpace objectSpace;
		private string reminderInfoXml;
		private int remindInSeconds;
		private IList<PostponeTime> postponeTimes;
		private Nullable<DateTime> alarmTime;
		private void UpdateResources() {
			Resources.Clear();
			if(!String.IsNullOrEmpty(resourceIds)) {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(resourceIds);
				foreach(XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
					AppointmentResourceIdXmlLoader loader = new AppointmentResourceIdXmlLoader(xmlNode);
					Object keyMemberValue = loader.ObjectFromXml();
					Object obj = objectSpace.GetObjectByKey(typeof(Resource), keyMemberValue);
					if(obj != null) {
						Resources.Add((Resource)obj);
					}
				}
			}
		}
		private void UpdateRemindersInfoXml(bool UpdateAlarmTime) {
			 if(RemindIn.HasValue && AlarmTime.HasValue) {
				AppointmentReminderInfo apptReminder = new AppointmentReminderInfo();
				ReminderInfo reminderInfo = new ReminderInfo();
				reminderInfo.TimeBeforeStart = RemindIn.Value;
				DateTime notNullableStartOn = StartOn.HasValue ? StartOn.Value : DateTime.MinValue;
				if(UpdateAlarmTime) {
					reminderInfo.AlertTime = AlarmTime.Value;
				}
				else {
					reminderInfo.AlertTime = notNullableStartOn - RemindIn.Value;
				}
				apptReminder.ReminderInfos.Add(reminderInfo);
				reminderInfoXml = apptReminder.ToXml();
			}
			else {
				reminderInfoXml = null;
			}
		}
		private void UpdateAlarmTime() {
			if(!string.IsNullOrEmpty(reminderInfoXml)) {
				AppointmentReminderInfo appointmentReminderInfo = new AppointmentReminderInfo();
				try {
					appointmentReminderInfo.FromXml(reminderInfoXml);
					alarmTime = appointmentReminderInfo.ReminderInfos[0].AlertTime;
				}
				catch(XmlException e) {
					Tracing.Tracer.LogError(e);
				}
			}
			else {
				alarmTime = null;
				remindInSeconds = NoneReminder;
				IsPostponed = false;
			}
		}
		private IList<PostponeTime> CreatePostponeTimes() {
			IList<PostponeTime> result = PostponeTime.CreateDefaultPostponeTimesList();
			result.Add(new PostponeTime("None", null, "None"));
			result.Add(new PostponeTime("AtStartTime", TimeSpan.Zero, "0 minutes"));
			CustomizeNotificationsPostponeTimeListEventArgs args = new CustomizeNotificationsPostponeTimeListEventArgs(result);
			if(CustomizeReminderTimeLookup != null) {
				CustomizeReminderTimeLookup(this, args);
			}
			PostponeTime.SortPostponeTimesList(args.PostponeTimesList);
			return args.PostponeTimesList;
		}
		public Event() {
			Resources = new List<Resource>();
			RecurrenceEvents = new List<Event>();
			remindInSeconds = NoneReminder;
		}
		public void UpdateResourceIds() {
			resourceIds = "<ResourceIds>\r\n";
			foreach(Resource resource in Resources) {
				resourceIds += String.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Key.GetType().FullName, resource.Key);
			}
			resourceIds += "</ResourceIds>";
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[FieldSize(250)]
		public String Subject { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String Description { get; set; }
		public Nullable<DateTime> StartOn { get; set; }
		public Nullable<DateTime> EndOn { get; set; }
		[ImmediatePostData]
		public Boolean AllDay {
			get {
				return allDay;
			}
			set {
				if(allDay != value && objectSpace != null) {
					if(PropertyChanging != null) {
						PropertyChangingEventArgs args = new PropertyChangingEventArgs("AllDay");
						PropertyChanging(this, args);
					}
					allDay = value;
					if(PropertyChanged != null) {
						PropertyChangedEventArgs args = new PropertyChangedEventArgs("AllDay");
						PropertyChanged(this, args);
					}
				}
				else {
					allDay = value;
				}
			}
		}
		public String Location { get; set; }
		public Int32 Label { get; set; }
		public Int32 Status { get; set; }
		[Browsable(false)]
		public Int32 Type { get; set; }
		[StringLength(300)]
		[NonCloneable, DisplayName("Recurrence"), FieldSize(FieldSizeAttribute.Unlimited)]
		public String RecurrenceInfoXml {
			get {
				return recurrenceInfoXml;
			}
			set {
				if(recurrenceInfoXml != value && objectSpace != null) {
					if(PropertyChanging != null) {
						PropertyChangingEventArgs args = new PropertyChangingEventArgs("RecurrenceInfoXml");
						PropertyChanging(this, args);
					}
					recurrenceInfoXml = value;
					if(PropertyChanged != null) {
						PropertyChangedEventArgs args = new PropertyChangedEventArgs("RecurrenceInfoXml");
						PropertyChanged(this, args);
					}
				}
				else {
					recurrenceInfoXml = value;
				}
			}
		}
		public virtual IList<Resource> Resources { get; set; }
		[Browsable(false)]
		public virtual Event RecurrencePattern { get; set; }
		[Browsable(false)]
		public virtual IList<Event> RecurrenceEvents { get; set; }
		[NotMapped, Browsable(false)]
		public String ResourceId {
			get {
				if(resourceIds == null) {
					UpdateResourceIds();
				}
				return resourceIds;
			}
			set {
				if(resourceIds != value) {
					resourceIds = value;
					if(objectSpace != null) {
						UpdateResources();
					}
					else {
						isUpdateResourcesDelayed = true;
					}
				}
			}
		}
		[NotMapped, Browsable(false)]
		public Object AppointmentId {
			get { return ID; }
		}
		[NotMapped, Browsable(false), RuleFromBoolProperty("EventIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues = false, UsedProperties = "StartOn, EndOn")]
		public Boolean IsIntervalValid {
			get { return StartOn <= EndOn; }
		}
		IRecurrentEvent IRecurrentEvent.RecurrencePattern {
			get { return RecurrencePattern; }
			set { RecurrencePattern = (Event)value; }
		}
		DateTime IEvent.StartOn {
			get {
				if(StartOn.HasValue) {
					return StartOn.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set {
				if(StartOn != value && objectSpace != null) {
					if(PropertyChanging != null) {
						PropertyChangingEventArgs args = new PropertyChangingEventArgs("StartOn");
						PropertyChanging(this, args);
					}
					StartOn = value;
					if(PropertyChanged != null) {
						PropertyChangedEventArgs args = new PropertyChangedEventArgs("StartOn");
						PropertyChanged(this, args);
					}
				}
				else {
					StartOn = value;
				}
			}
		}
		DateTime IEvent.EndOn {
			get {
				if(EndOn.HasValue) {
					return EndOn.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { EndOn = value; }
		}
		[ImmediatePostData]
		[NotMapped]
		[ModelDefault("AllowClear", "False")]
		[DataSourceProperty("PostponeTimeList")]
		[SearchMemberOptions(SearchMemberMode.Exclude)]
		[VisibleInDetailView(false), VisibleInListView(false)]
		public PostponeTime ReminderTime {
			get {
				if(RemindIn.HasValue) {
					return PostponeTimeList.Where(x => (x.RemindIn != null && x.RemindIn.Value == RemindIn.Value)).FirstOrDefault();
				}
				else {
					return PostponeTimeList.Where(x => x.RemindIn == null).FirstOrDefault();
				}
			}
			set {
				if(value.RemindIn.HasValue) {
					RemindIn = value.RemindIn.Value;
				}
				else {
					RemindIn = null;
				}
			}
		}
		[Browsable(false), NotMapped]
		public IEnumerable<PostponeTime> PostponeTimeList {
			get {
				if(postponeTimes == null) {
					postponeTimes = CreatePostponeTimes();
				}
				return postponeTimes;
			}
		}
		[NonCloneable]
		[Browsable(false)]
		[StringLength(200)]
		public string ReminderInfoXml {
			get { return reminderInfoXml; }
			set {
				if(reminderInfoXml != value) {
					reminderInfoXml = value;
					if(objectSpace != null) {
						UpdateAlarmTime();
					}
				}
			}
		}
		[Browsable(false)]
		[NotMapped]
		public TimeSpan? RemindIn {
			get {
				if(remindInSeconds < 0) {
					return null; 
				}
				else { 
					return TimeSpan.FromSeconds(remindInSeconds); 
				}
			}
			set {
				if(RemindIn.HasValue && value.HasValue && RemindIn.Value != value.Value || RemindIn.HasValue && !value.HasValue || !RemindIn.HasValue && value.HasValue) {
					if(PropertyChanging != null) {
						PropertyChangingEventArgs args = new PropertyChangingEventArgs("RemindIn");
						PropertyChanging(this, args);
					}
					if(value.HasValue) {
						remindInSeconds = (int)value.Value.TotalSeconds;
					}
					else {
						remindInSeconds = NoneReminder;
					}
					if(PropertyChanged != null) {
						PropertyChangedEventArgs args = new PropertyChangedEventArgs("RemindIn");
						PropertyChanged(this, args);
					}
				}
			}
		}
		[Browsable(false)]
		public int RemindInSeconds {
			get { return remindInSeconds; }
			set {
				remindInSeconds = value;
			}
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		[NotMapped]
		public string NotificationMessage {
			get { return Subject; }
		}
		[Browsable(false), NotMapped]
		public object UniqueId {
			get { return ID; }
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		public Nullable<DateTime> AlarmTime {
			get { return alarmTime; }
			set {
				if(alarmTime != value) {
					alarmTime = value;
					if(objectSpace != null) {
						if(value == null) {
							remindInSeconds = NoneReminder;
							IsPostponed = false;
						}
						UpdateRemindersInfoXml(true);
					}
				}
			}
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		public bool IsPostponed {
			get;
			set;
		}
		[NotMapped]
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		void IXafEntityObject.OnCreated() {
		}
		void IXafEntityObject.OnSaving() {
			if((objectSpace != null) && isUpdateResourcesDelayed) {
				isUpdateResourcesDelayed = false;
				UpdateResources();
			}
		}
		void IXafEntityObject.OnLoaded() {
			Int32 count = Resources.Count;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangingEventHandler PropertyChanging;
		public event EventHandler<CustomizeNotificationsPostponeTimeListEventArgs> CustomizeReminderTimeLookup;
	}
}
