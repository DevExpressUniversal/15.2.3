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
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public static class ASPxSchedulerFormDataHelper {
		#region CreateLabelDataSource
		public static IEnumerable CreateLabelDataSource(ASPxScheduler scheduler) {
			if (scheduler == null)
				return FormTemplateContainerHelper.CreateEmptyListEditData();
			return FormTemplateContainerHelper.CreateListEditData(scheduler.Storage.Appointments.Labels, GetDisplayNameFromUserInterfaceObjectItem);
		}
		#endregion
		#region CreateShowTimeAsDataSource
		public static IEnumerable CreateStatusesDataSource(ASPxScheduler scheduler) {
			if (scheduler == null)
				return FormTemplateContainerHelper.CreateEmptyListEditData();
			return FormTemplateContainerHelper.CreateListEditData(scheduler.Storage.Appointments.Statuses, GetDisplayNameFromUserInterfaceObjectItem);
		}
		#endregion
		#region CreateResourceDataSource
		public static IEnumerable CreateResourceDataSource(ASPxScheduler scheduler) {
			if (scheduler == null)
				return FormTemplateContainerHelper.CreateEmptyListEditData();
			ResourceBaseCollection resources = scheduler.Storage.GetFilteredResources(true);
			ListEditItemCollection collection = (ListEditItemCollection)FormTemplateContainerHelper.CreateListEditData(resources);
			if (scheduler.Storage.ResourceSharing)
				return collection;
			ListEditItem anyResource = new ListEditItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_EmptyResource), "null");
			collection.Insert(0, anyResource);
			return collection;
		}
		#endregion
		#region CreateReminderDataSource
		public static IEnumerable CreateReminderDataSource() {
			ListEditItemCollection collection = new ListEditItemCollection();
			int count = ReminderTimeSpans.ReminderTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = ReminderTimeSpans.ReminderTimeSpanValues[i];
				collection.Add(HumanReadableTimeSpanHelper.ToString(timeSpan), timeSpan.ToString());
			}
			return collection;
		}
		#endregion
		#region GetDisplayNameFromUserInterfaceObjectItem
		static string GetDisplayNameFromUserInterfaceObjectItem(object item) {
			UserInterfaceObject uiObject = (UserInterfaceObject)item;
			return uiObject.DisplayName;
		}
		#endregion
	}
	#region AppointmentFormTemplateContainerBase
	public abstract class AppointmentFormTemplateContainerBase : SchedulerFormTemplateContainer {
		readonly Appointment apt;
		readonly bool isNewAppointment;
		readonly IDefaultUserData defaultUserData;
		[Obsolete("You should use the 'AppointmentFormTemplateContainerBase(ASPxScheduler control)' constructor instead", false)]
		protected AppointmentFormTemplateContainerBase(ASPxScheduler control, Appointment apt)
			: this(control) {
		}
		protected AppointmentFormTemplateContainerBase(ASPxScheduler control)
			: base(control) {
			this.apt = Control.GetEditedAppointment();
			this.defaultUserData = Control.GetDefaultUserData();
			if (this.apt == null)
				this.apt = CreateStubAppointment();
			this.isNewAppointment = Control.EditableAppointment.IsNewlyCreated;
		}
		#region Properties
		protected internal IDefaultUserData DefaultUserData { get { return defaultUserData; } }
		public Appointment Appointment { get { return apt; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseIsNewAppointment")]
#endif
		public bool IsNewAppointment { get { return isNewAppointment; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseCustomFields")]
#endif
		public CustomFieldCollection CustomFields { get { return Appointment.CustomFields; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseSaveHandler")]
#endif
		public abstract string SaveHandler { get; }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseSaveScript")]
#endif
		public abstract string SaveScript { get; }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseCancelHandler")]
#endif
		public override string CancelHandler { get { return String.Format("function() {{ ASPx.AppointmentCancel(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerBaseCancelScript")]
#endif
		public override string CancelScript { get { return String.Format("ASPx.AppointmentCancel(\"{0}\")", ControlClientId); } }
		#endregion
		#region CreateStubAppointment
		private Appointment CreateStubAppointment() {
			Appointment stub = Control.Storage.CreateAppointment(AppointmentType.Normal);
			stub.SetId(SchedulerIdHelper.NewAppointmentId);
			return stub;
		}
		#endregion
	}
	#endregion
	#region AppointmentFormTemplateContainer
	public class AppointmentFormTemplateContainer : AppointmentFormTemplateContainerBase {
		#region Fields
		IEnumerable labelDataSource;
		IEnumerable statusDataSource;
		IEnumerable resourceDataSource;
		IEnumerable reminderDataSource;
		AppointmentFormController controller;
		#endregion
		[Obsolete("You should use the 'AppointmentFormTemplateContainer(ASPxScheduler control)' constructor instead", false)]
		public AppointmentFormTemplateContainer(ASPxScheduler control, Appointment apt)
			: this(control) {
		}
		public AppointmentFormTemplateContainer(ASPxScheduler control)
			: base(control) {
			this.labelDataSource = CreateLabelDataSource();
			this.statusDataSource = CreateStatusesDataSource();
			this.resourceDataSource = CreateResourceDataSource();
			this.reminderDataSource = CreateReminderDataSource();
			this.controller = CreateController(control, Appointment);
			this.LoadPostData(control);
			this.ID = SchedulerIdHelper.AppointmentFormTemplateContainerId;
		}
		protected virtual AppointmentFormController CreateController(ASPxScheduler control, Appointment appointment) {
			return new AppointmentFormController(control, appointment);
		}
		#region Properties
		protected internal AppointmentFormController Controller { get { return controller; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerLabelDataSource")]
#endif
		public IEnumerable LabelDataSource { get { return labelDataSource; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerStatusDataSource")]
#endif
		public IEnumerable StatusDataSource { get { return statusDataSource; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerResourceDataSource")]
#endif
		public IEnumerable ResourceDataSource { get { return resourceDataSource; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerReminderDataSource")]
#endif
		public IEnumerable ReminderDataSource { get { return reminderDataSource; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerSaveHandler")]
#endif
		public override string SaveHandler { get { return String.Format("function() {{ ASPx.AppointmentSave(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerSaveScript")]
#endif
		public override string SaveScript { get { return String.Format("ASPx.AppointmentSave(\"{0}\")", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerDeleteHandler")]
#endif
		public virtual string DeleteHandler { get { return String.Format("function() {{ ASPx.AppointmentDelete(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerDeleteScript")]
#endif
		public virtual string DeleteScript { get { return String.Format("ASPx.AppointmentDelete(\"{0}\")", ControlClientId); } }
		#region Appointment Properties
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerStart")]
#endif
		public DateTime Start { get { return Controller.DisplayStart; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerEnd")]
#endif
		public DateTime End { get { return Controller.End; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerSubject")]
#endif
		public string Subject {
			get {
				if (DefaultUserData == null)
					return Appointment.Subject;
				object defaultUserSubject = DefaultUserData.GetProperty("Subject");
				if (defaultUserSubject == null)
					return Appointment.Subject;
				return (string)defaultUserSubject;
			}
		}
		#endregion
		#region Recurrence properties
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceDayNumber")]
#endif
		public int RecurrenceDayNumber {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.DayNumber : 1; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceEnd")]
#endif
		public DateTime RecurrenceEnd {
			get { return TimeZoneHelper.ToClientTime(Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.End : Start.AddDays(1), Controller.TimeZoneId).Date; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceMonth")]
#endif
		public int RecurrenceMonth {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.Month : 1; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceOccurrenceCount")]
#endif
		public int RecurrenceOccurrenceCount {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.OccurrenceCount : 10; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrencePeriodicity")]
#endif
		public int RecurrencePeriodicity {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.Periodicity : 1; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceRange")]
#endif
		public RecurrenceRange RecurrenceRange {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.Range : RecurrenceRange.NoEndDate; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceStart")]
#endif
		public DateTime RecurrenceStart {
			get { return TimeZoneHelper.ToClientTime(Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.Start : Start, controller.TimeZoneId); }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceWeekDays")]
#endif
		public WeekDays RecurrenceWeekDays {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.WeekDays : WeekDays.EveryDay; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceWeekOfMonth")]
#endif
		public WeekOfMonth RecurrenceWeekOfMonth {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.WeekOfMonth : WeekOfMonth.First; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerRecurrenceType")]
#endif
		public RecurrenceType RecurrenceType {
			get { return Controller.EditedPattern != null ? Controller.EditedPattern.RecurrenceInfo.Type : RecurrenceType.Daily; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerShouldShowRecurrence")]
#endif
		public bool ShouldShowRecurrence {
			get { return Controller.ShouldShowRecurrence; }
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerIsRecurring")]
#endif
		public bool IsRecurring { get { return Controller.EditedPattern.IsRecurring; } }
		#endregion
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerCanDeleteAppointment")]
#endif
		public bool CanDeleteAppointment { get { return !IsNewAppointment && Controller.CanDeleteAppointment; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentFormTemplateContainerCanEditResource")]
#endif
		public bool CanEditResource { get { return Controller.CanEditResource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
		#region CreateLabelDataSource
		protected internal virtual IEnumerable CreateLabelDataSource() {
			return FormTemplateContainerHelper.CreateListEditData(Control.Storage.Appointments.Labels, GetDisplayNameFromUserInterfaceObjectItem);
		}
		#endregion
		#region CreateShowTimeAsDataSource
		protected internal virtual IEnumerable CreateStatusesDataSource() {
			return FormTemplateContainerHelper.CreateListEditData(Control.Storage.Appointments.Statuses, GetDisplayNameFromUserInterfaceObjectItem);
		}
		#endregion
		#region CreateResourceDataSource
		protected internal virtual IEnumerable CreateResourceDataSource() {
			return ASPxSchedulerFormDataHelper.CreateResourceDataSource(Control);
		}
		#endregion
		#region CreateReminderDataSource
		protected internal virtual IEnumerable CreateReminderDataSource() {
			ListEditItemCollection collection = new ListEditItemCollection();
			int count = ReminderTimeSpans.ReminderTimeSpanValues.Length;
			for (int i = 0; i < count; i++) {
				TimeSpan timeSpan = ReminderTimeSpans.ReminderTimeSpanValues[i];
				collection.Add(HumanReadableTimeSpanHelper.ToString(timeSpan), timeSpan.ToString());
			}
			return collection;
		}
		#endregion
		#region GetDisplayNameFromUserInterfaceObjectItem
		protected internal virtual string GetDisplayNameFromUserInterfaceObjectItem(object item) {
			UserInterfaceObject uiObject = (UserInterfaceObject)item;
			return uiObject.DisplayName;
		}
		#endregion
	}
	#endregion
	#region RecurrentAppointmentDeleteFormTemplateContainer
	public class RecurrentAppointmentDeleteFormTemplateContainer : SchedulerFormTemplateContainer {
		Appointment apt;
		public RecurrentAppointmentDeleteFormTemplateContainer(ASPxScheduler control, Appointment apt)
			: base(control) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			this.apt = apt;
		}
		public Appointment Appointment { get { return apt; } }
		public virtual string ApplyHandler { get { return String.Format("function() {{ ASPx.RecurrentAptDelete(\"{0}\"); }}", ControlClientId); } }
		public virtual string ApplyScript { get { return String.Format("ASPx.RecurrentAptDelete(\"{0}\")", ControlClientId); } }
		public override string CancelHandler { get { return String.Format("function() {{ ASPx.RecurrentAptDeleteCancel(\"{0}\"); }}", ControlClientId); } }
		public override string CancelScript { get { return String.Format("ASPx.RecurrentAptDeleteCancel(\"{0}\")", ControlClientId); } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
	}
	#endregion
	#region RecurrentAppointmentEditFormTemplateContainer
	public class RecurrentAppointmentEditFormTemplateContainer : SchedulerFormTemplateContainer {
		Appointment apt;
		public RecurrentAppointmentEditFormTemplateContainer(ASPxScheduler control, Appointment apt)
			: base(control) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			this.apt = apt;
		}
		public Appointment Appointment { get { return apt; } }
		public virtual string ApplyHandler { get { return String.Format("function() {{ ASPx.RecurrentAptEdit(\"{0}\"); }}", ControlClientId); } }
		public virtual string ApplyScript { get { return String.Format("ASPx.RecurrentAptEdit(\"{0}\")", ControlClientId); } }
		public override string CancelHandler { get { return String.Format("function() {{ ASPx.RecurrentAptEditCancel(\"{0}\"); }}", ControlClientId); } }
		public override string CancelScript { get { return String.Format("ASPx.RecurrentAptEditCancel(\"{0}\")", ControlClientId); } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
	}
	#endregion
}
