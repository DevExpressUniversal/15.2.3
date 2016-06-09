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
using System.ComponentModel;
using System.Xml;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.Text;
using System.Collections.Generic;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
using System.Linq;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.ExpressApp.Filtering;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("Subject")]
	[NavigationItem("Default")]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	public class Event : BaseObject, IRecurrentEvent, IReminderEvent {
#if MediumTrust
		[Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String resourceIds;
#else
		[Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		private String resourceIds;
#endif
		private EventImpl appointmentImpl = new EventImpl();
		[Persistent("RecurrencePattern")]
		private Event recurrencePattern;
		private string recurrenceInfoXml;
		private string reminderInfoXml;
		private TimeSpan? remindIn;
		private Nullable<DateTime> alarmTime;
		private IList<PostponeTime> postponeTimes;
		private bool isPostponed;
		private void UpdateResources() {
			Resources.SuspendChangedEvents();
			try {
				while(Resources.Count > 0) {
					Resources.Remove(Resources[0]);
				}
				if(!String.IsNullOrEmpty(resourceIds)) {
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(resourceIds);
					foreach(XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
						AppointmentResourceIdXmlLoader loader = new AppointmentResourceIdXmlLoader(xmlNode);
						Object keyMemberValue = loader.ObjectFromXml();
						Resource resource = Session.GetObjectByKey<Resource>(new Guid(keyMemberValue.ToString()));
						if(resource != null) {
							Resources.Add(resource);
						}
					}
				}
			}
			finally {
				Resources.ResumeChangedEvents();
			}
		}
		private void Resources_ListChanged(object sender, ListChangedEventArgs e) {
			if((e.ListChangedType == ListChangedType.ItemAdded) ||
				(e.ListChangedType == ListChangedType.ItemDeleted)) {
				UpdateResourceIds();
				OnChanged("ResourceId");
			}
		}
		private void UpdateRemindersInfoXml(bool UpdateAlarmTime) {
			if(RemindIn.HasValue && AlarmTime.HasValue) {
				AppointmentReminderInfo apptReminder = new AppointmentReminderInfo();
				ReminderInfo reminderInfo = new ReminderInfo();
				reminderInfo.TimeBeforeStart = RemindIn.Value;
				if(UpdateAlarmTime) {
					reminderInfo.AlertTime = AlarmTime.Value;
				}
				else {
					reminderInfo.AlertTime = StartOn - RemindIn.Value;
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
				remindIn = null;
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
		protected override void OnLoaded() {
			base.OnLoaded();
			if(Resources.IsLoaded && !Session.IsNewObject(this)) {
				Resources.Reload();
			}
		}
		public Event(Session session)
			: base(session) {
			Resources.ListChanged += new ListChangedEventHandler(Resources_ListChanged);
		}
		public override void AfterConstruction() {
			base.AfterConstruction();
			appointmentImpl.AfterConstruction();
		}
		public void UpdateResourceIds() {
			resourceIds = "<ResourceIds>\r\n";
			foreach(Resource resource in Resources) {
				resourceIds += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
			}
			resourceIds += "</ResourceIds>";
		}
		[NonPersistent, Browsable(false)]
		public object AppointmentId {
			get { return Oid; }
		}
		[Size(250)]
		public string Subject {
			get { return appointmentImpl.Subject; }
			set {
				string oldValue = appointmentImpl.Subject;
				appointmentImpl.Subject = value;
				OnChanged("Subject", oldValue, appointmentImpl.Subject);
			}
		}
		[Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string Description {
			get { return appointmentImpl.Description; }
			set {
				string oldValue = appointmentImpl.Description;
				appointmentImpl.Description = value;
				OnChanged("Description", oldValue, appointmentImpl.Description);
			}
		}
		[Indexed]
		public DateTime StartOn {
			get { return appointmentImpl.StartOn; }
			set {
				DateTime oldValue = appointmentImpl.StartOn;
				appointmentImpl.StartOn = value;
				OnChanged("StartOn", oldValue, appointmentImpl.StartOn);
			}
		}
		[Indexed]
		public DateTime EndOn {
			get { return appointmentImpl.EndOn; }
			set {
				DateTime oldValue = appointmentImpl.EndOn;
				appointmentImpl.EndOn = value;
				OnChanged("EndOn", oldValue, appointmentImpl.EndOn);
			}
		}
		[ImmediatePostData]
		public bool AllDay {
			get { return appointmentImpl.AllDay; }
			set {
				bool oldValue = appointmentImpl.AllDay;
				appointmentImpl.AllDay = value;
				OnChanged("AllDay", oldValue, appointmentImpl.AllDay);
			}
		}
		public string Location {
			get { return appointmentImpl.Location; }
			set {
				string oldValue = appointmentImpl.Location;
				appointmentImpl.Location = value;
				OnChanged("Location", oldValue, appointmentImpl.Location);
			}
		}
		public int Label {
			get { return appointmentImpl.Label; }
			set {
				int oldValue = appointmentImpl.Label;
				appointmentImpl.Label = value;
				OnChanged("Label", oldValue, appointmentImpl.Label);
			}
		}
		public int Status {
			get { return appointmentImpl.Status; }
			set {
				int oldValue = appointmentImpl.Status;
				appointmentImpl.Status = value;
				OnChanged("Status", oldValue, appointmentImpl.Status);
			}
		}
		[Browsable(false)]
		public int Type {
			get { return appointmentImpl.Type; }
			set {
				int oldValue = appointmentImpl.Type;
				appointmentImpl.Type = value;
				OnChanged("Type", oldValue, appointmentImpl.Type);
			}
		}
		[Association("Event-Resource")]
		public XPCollection<Resource> Resources {
			get { return GetCollection<Resource>("Resources"); }
		}
		[NonPersistent(), Browsable(false)]
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
					UpdateResources();
				}
			}
		}
		[ImmediatePostData]
		[NonPersistent]
		[ModelDefault("AllowClear", "False")]
		[DataSourceProperty("PostponeTimeList")]
		[SearchMemberOptions(SearchMemberMode.Exclude)]
		[VisibleInDetailView(false), VisibleInListView(false)]
		public PostponeTime ReminderTime {
			get {
				if(RemindIn.HasValue) {
					return PostponeTimeList.Where(x => (x.RemindIn != null && x.RemindIn.Value == remindIn.Value)).FirstOrDefault();
				}
				else {
					return PostponeTimeList.Where(x => x.RemindIn == null).FirstOrDefault();
				} 
			}
			set {
				if(!IsLoading) {
					if (value.RemindIn.HasValue){
						RemindIn = value.RemindIn.Value;
					}
					else{
						RemindIn = null;
					}
				}
			}
		}
		[Browsable(false), NonPersistent]
		public IEnumerable<PostponeTime> PostponeTimeList {
			get {
				if(postponeTimes == null) {
					postponeTimes = CreatePostponeTimes();
				}
				return postponeTimes;
			}
		}
		[Browsable(false)]
		public TimeSpan? RemindIn {
			get { return remindIn; }
			set {
				SetPropertyValue("RemindIn", ref remindIn, value);
			}
		}
		[NonCloneable]
		[Browsable(false)]
		[Size(200)]
		public string ReminderInfoXml {
			get { return reminderInfoXml; }
			set {
				SetPropertyValue("ReminderInfoXml", ref reminderInfoXml, value);
				if(!IsLoading) {
					UpdateAlarmTime();
				}
			}
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		[NonPersistent]
		public string NotificationMessage {
			get { return Subject; }
		}
		[Browsable(false)]
		[NonPersistent]
		public object UniqueId {
			get { return Oid; }
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		public DateTime? AlarmTime {
			get { return alarmTime; }
			set { 
				SetPropertyValue("AlarmTime", ref alarmTime, value);
				if(!IsLoading) {
					if(value == null) {
						remindIn = null;
						IsPostponed = false;
					}
					UpdateRemindersInfoXml(true);
				}
			}
		}
		[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
		public bool IsPostponed {
			get { return isPostponed; }
			set { SetPropertyValue("IsPostponed", ref isPostponed, value); }
		}
		[NonCloneable]
		[DevExpress.Xpo.DisplayName("Recurrence"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string RecurrenceInfoXml {
			get { return recurrenceInfoXml; }
			set { SetPropertyValue("RecurrenceInfoXml", ref recurrenceInfoXml, value); }
		}
		[PersistentAlias("recurrencePattern")]
		public IRecurrentEvent RecurrencePattern {
			get { return recurrencePattern; }
			set { SetPropertyValue("RecurrencePattern", ref recurrencePattern, (Event)value); }
		}
		[NonPersistent]
		[Browsable(false)]
		[RuleFromBoolProperty("EventIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues = false, UsedProperties = "StartOn, EndOn")]
		public bool IsIntervalValid { get { return StartOn <= EndOn; } }
		public event EventHandler<CustomizeNotificationsPostponeTimeListEventArgs> CustomizeReminderTimeLookup;
	}
}
