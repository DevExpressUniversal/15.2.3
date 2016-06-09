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
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.UI {
	public class AppointmentFormController : AppointmentFormControllerBase {
		readonly SchedulerControl control;
		IResourceStorageBase resources;
		Appointment patternCopy;
		public AppointmentFormController(SchedulerControl control, Appointment apt)
			: base(control.InnerControl, apt) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.resources = control.Storage.ResourceStorage.InnerResources;
			this.patternCopy = PrepareToRecurrenceEdit();
			PropertyChanged += new PropertyChangedEventHandler(OnAppointmentFormControllerPropertyChanged);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerControl")]
#endif
		public SchedulerControl Control { get { return control; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerStorage")]
#endif
		public SchedulerStorage Storage { get { return Control.Storage; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerTimeZoneHelper")]
#endif
		public TimeZoneHelper TimeZoneHelper { get { return base.InnerControl.TimeZoneHelper; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerShouldShowRecurrence")]
#endif
		public bool ShouldShowRecurrence { get { return !SourceAppointment.IsOccurrence && ShouldShowRecurrenceButton; } }
		protected internal IAppointmentStorage Appointments { get { return (IAppointmentStorage)InnerAppointments; } }
		protected internal IResourceStorageBase Resources { get { return resources; } }
		protected internal AppointmentLabelCollection Labels { get { return Appointments.Labels; } }
		protected internal AppointmentStatusCollection Statuses { get { return Appointments.Statuses; } }
		public AppointmentStatus Status { get { return Statuses.GetById(StatusKey); } set { OnStatusChanged(value); } }
		public AppointmentLabel Label { get { return Labels.GetById(LabelKey); } set { OnLabelChanged(value); } }
		public Resource AppointmentResource { get { return Resources.GetResourceById(ResourceId); } set { OnResourceChanged(value); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerNoneString")]
#endif
		public string NoneString { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneReminder); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerDisplayStartDate")]
#endif
		public DateTime DisplayStartDate {
			get { return DisplayStart.Date; }
			set {
				SetDisplayStartDate(value);
				NotifyPropertyChanged("DisplayStartDate");
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerDisplayStartTime")]
#endif
		public TimeSpan DisplayStartTime {
			get { return new DateTime(DisplayStart.TimeOfDay.Ticks).TimeOfDay; }
			set {
				SetDisplayStartTime(value);
				NotifyPropertyChanged("DisplayStartTime");
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerDisplayEndDate")]
#endif
		public DateTime DisplayEndDate {
			get { return DisplayEnd.Date; }
			set {
				SetDisplayEndDate(value);
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerDisplayEndTime")]
#endif
		public TimeSpan DisplayEndTime {
			get {
				return new DateTime(DisplayEnd.TimeOfDay.Ticks).TimeOfDay;
			}
			set {
				SetDisplayEndTime(value);
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
		void OnAppointmentFormControllerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "AllDay") {
				NotifyPropertyChanged("DisplayStartTime");
				NotifyPropertyChanged("DisplayEndTime");
				NotifyPropertyChanged("DisplayStartDate");
				NotifyPropertyChanged("DisplayEndDate");
			}
			else if (e.PropertyName == "DisplayStartTime") {
				NotifyPropertyChanged("DisplayEndTime");
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
		protected internal virtual void SetDisplayStartDate(DateTime value) {
			UpdateStart(value.Date, DisplayStartTime);
		}
		protected internal virtual void SetDisplayStartTime(TimeSpan value) {
			UpdateStart(DisplayStartDate, value);
		}
		protected internal virtual void UpdateStart(DateTime date, TimeSpan timeOfDay) {
			DisplayStart = date + timeOfDay;
		}
		protected internal virtual void SetDisplayEndDate(DateTime value) {
			UpdateEnd(value.Date, DisplayEndTime);
		}
		protected internal virtual void SetDisplayEndTime(TimeSpan value) {
			UpdateEnd(DisplayEndDate, value);
		}
		protected internal virtual void UpdateEnd(DateTime date, TimeSpan timeOfDay) {
			DisplayEnd = date + timeOfDay;
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerAppointmentResources")]
#endif
		public ResourceBaseCollection AppointmentResources { get { return GetAppointmentResources(); } set { } }
		private ResourceBaseCollection GetAppointmentResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(ResourceBase.Empty);
			result.AddRange(Resources.Items);
			return result;
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerAppointmentResourceIds")]
#endif
		public IList AppointmentResourceIds { get { return EditedAppointmentCopy.ResourceIds; } set { SetAppointmentResourceIds(value); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerReminderSpans")]
#endif
		public IList ReminderSpans { get { return GetReminderSpans(); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentFormControllerReminderSpan")]
#endif
		public string ReminderSpan {
			get { return GetReminderTimeBeforeStart(); }
			set { SetReminderTimeBeforeStart(value); }
		}
		private string GetReminderTimeBeforeStart() {
			return HasReminder ? HumanReadableTimeSpanHelper.ToString(ReminderTimeBeforeStart) : NoneString;
		}
		private void SetReminderTimeBeforeStart(string value) {
			if (value == NoneString) {
				HasReminder = false;
				return;
			}
			HasReminder = true;
			ReminderTimeBeforeStart = HumanReadableTimeSpanHelper.Parse(value);
		}
		public Appointment PatternCopy {
			get {
				if (patternCopy == null)
					patternCopy = PrepareToRecurrenceEdit();
				return patternCopy;
			}
			set { }
		}
		public RecurrenceInfo PatternRecurrenceInfo {
			get { return SourceAppointment.IsRecurring ? (RecurrenceInfo)PatternCopy.RecurrenceInfo : null; }
		}
		private void SetAppointmentResourceIds(IList value) {
			EditedAppointmentCopy.BeginUpdate();
			try {
				AppointmentResourceIdCollection resIds = EditedAppointmentCopy.ResourceIds;
				resIds.BeginUpdate();
				try {
					resIds.Clear();
					resIds.AddRange(value);
				}
				finally {
					resIds.EndUpdate();
				}
			}
			finally {
				EditedAppointmentCopy.EndUpdate();
			}
		}
		protected override void OnStatusIdChanged() {
			base.OnStatusIdChanged();
			NotifyPropertyChanged("Status");
		}
		private void OnStatusChanged(IAppointmentStatus status) {
			StatusKey = status.Id;
		}
		protected override void OnLabelIdChanged() {
			base.OnLabelIdChanged();
			NotifyPropertyChanged("Label");
		}
		private void OnLabelChanged(IAppointmentLabel label) {
			LabelKey = label.Id;
		}
		private void OnResourceChanged(Resource resource) {
			ResourceId = resource.Id;
		}
		private IList GetReminderSpans() {
			List<string> result = new List<string>();
			TimeSpan maxDuration = TimeSpan.MaxValue;
			result.Add(NoneString);
			int count = ReminderTimeSpans.ReminderTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = ReminderTimeSpans.ReminderTimeSpanValues[i];
				if (timeSpan <= maxDuration)
					result.Add(HumanReadableTimeSpanHelper.ToString(timeSpan));
			}
			return result;
		}
	}
}
