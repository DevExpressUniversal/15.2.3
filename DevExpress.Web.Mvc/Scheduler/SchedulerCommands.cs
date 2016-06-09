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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.Web.ASPxScheduler.Commands;
	using DevExpress.Web.ASPxScheduler.Controls;
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.Web.Internal;
	using DevExpress.XtraScheduler;
	using DevExpress.XtraScheduler.Commands;
	using DevExpress.XtraScheduler.Native;
	interface IEditableAppointments {
		Appointment[] NewAppointments { get; }
		Appointment[] UpdatedAppointments { get; }
		Appointment[] DeletedAppointments { get; }
		MVCxSchedulerStorage Storage { get; }
		void Execute(string parameters);
		void FinalizeExecute();
	}
	public class MVCxSchedulerCallbackCommandManager: SchedulerCallbackCommandManager {
		SchedulerCallbackCommandCollection commandsForEditableActions;
		public MVCxSchedulerCallbackCommandManager(MVCxScheduler scheduler)
			: base(scheduler) {
			this.commandsForEditableActions = CreateCommandsForEditableActions();
		}
		protected internal SchedulerCallbackCommandCollection CommandsForEditableActions {
			get { return commandsForEditableActions; }
		}
		protected internal SchedulerCallbackCommand GetCommandForEditableActions(CallbackCommandInfo commandInfo) {
			return LookupCallbackCommand(CommandsForEditableActions, commandInfo);
		}
		protected internal new void PerformParseParametersForEditableCommand(CallbackCommandInfo commandInfo, SchedulerCallbackCommand command) {
			base.PerformParseParametersForEditableCommand(commandInfo, command);
		}
		protected override RecurrentAppointmentEditCallbackCommand CreateRecurrentAppointmentEditCallbackCommand() {
			return new MVCxRecurrentAppointmentEditCallbackCommand(Control);
		}
		protected override MenuAppointmentCallbackCommand CreateMenuAppointmentCallbackCommand() {
			return new MVCxMenuAppointmentCallbackCommand(Control);
		}
		protected override AppointmentInplaceEditorEditFormCallbackCommand CreateAppointmentInplaceEditorEditFormCallbackCommand() {
			return new MVCxAppointmentInplaceEditorEditFormCallbackCommand(Control);
		}
		protected override SchedulerCallbackCommandCollection CreateEditableCallbackCommands() {
			return new SchedulerCallbackCommandCollection();
		}
		SchedulerCallbackCommandCollection CreateCommandsForEditableActions() {
			SchedulerCallbackCommandCollection commandCollection = new SchedulerCallbackCommandCollection();
			commandCollection.Add(new MVCxAppointmentFormSaveCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentDeleteCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentInplaceEditorSaveCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentsChangeCommand(Control));
			commandCollection.Add(CreateMenuAppointmentCallbackCommand());
			commandCollection.Add(new MVCxRecurrentAppointmentDeleteCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentClientSideInsertCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentClientSideUpdateCallbackCommand(Control));
			commandCollection.Add(new MVCxAppointmentClientSideDeleteCallbackCommand(Control));
			commandCollection.Add(new MVCxSnoozeReminderCallbackCommand(Control));
			commandCollection.Add(new MVCxDismissReminderCallbackCommand(Control));
			commandCollection.Add(new MVCxDismissAllRemindersCallbackCommand(Control));
			commandCollection.Add(new MVCxCloseRemindersFormCallbackCommand(Control));
			return commandCollection;
		}
	}
	public class MVCxAppointmentFormSaveCallbackCommand: AppointmentFormSaveCallbackCommand, IEditableAppointments {
		MVCxAppointmentRecurrenceControl recurrenceControl;
		public MVCxAppointmentFormSaveCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		protected internal new AppointmentFormTemplateContainer TemplateContainer { get { return (AppointmentFormTemplateContainer)base.TemplateContainer; } }
		protected internal new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected MVCxAppointmentRecurrenceControl RecurrenceControl {
			get {
				if (recurrenceControl == null)
					recurrenceControl = CreateRecurrenceControl();
				return recurrenceControl;
			}
		}
		SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		protected virtual MVCxAppointmentRecurrenceControl CreateRecurrenceControl() {
			Hashtable clientData = AppointmentEditValues.GetValue<Hashtable>("RecurrenceFormValue");
			MVCxAppointmentRecurrenceControl control = clientData != null ? new MVCxAppointmentRecurrenceControl(clientData) : null;
			if (control != null)
				control.EnsureChildControls();
			return control;
		}
		protected override AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new MVCxAppointmentFormController(Control, apt);
		}
		protected override bool CanApplyChanges() {
			return Controller != null && base.CanApplyChanges();
		}
		protected override void AssignControllerValues() {
			if (MVCxSchedulerCommandHelper.IsUsingCustomTemplateContainer(Control))
				MVCxSchedulerCommandHelper.AssignControllerValuesInCustomForm(Controller, FromClientTime);
			else
				AssignControllerValuesInDefaultForm();
			PrepareControllerResources();
			AssignControllerRecurrenceValues(Controller.Start);
		}
		protected void AssignControllerValuesInDefaultForm() {
			Controller.AllDay = AppointmentEditValues.GetValue<bool>("chkAllDay");
			Controller.EditedAppointmentCopy.Start = FromClientTime(AppointmentEditValues.GetValue<DateTime>("edtStartDate"));
			Controller.EditedAppointmentCopy.End = FromClientTime(AppointmentEditValues.GetValue<DateTime>("edtEndDate"));
			Controller.Subject = AppointmentEditValues.GetValue<string>("tbSubject");
			Controller.Location = AppointmentEditValues.GetValue<string>("tbLocation");
			Controller.Description = AppointmentEditValues.GetValue<string>("tbDescription");
			Controller.StatusId = AppointmentEditValues.GetValue<Int32>("edtStatus");
			Controller.LabelId = AppointmentEditValues.GetValue<Int32>("edtLabel");
			if(AppointmentEditValues.Contains("chkReminder")) {
				Controller.HasReminder = AppointmentEditValues.GetValue<bool>("chkReminder");
				if(Controller.HasReminder)
					Controller.ReminderTimeBeforeStart = AppointmentEditValues.GetValue<TimeSpan>("cbReminder");
			}
		}
		protected void PrepareControllerResources() {
			bool isUsingCustomTemplateContainer = MVCxSchedulerCommandHelper.IsUsingCustomTemplateContainer(Control);
			if (Control.Storage.ResourceSharing) {
				Controller.ResourceIds.Clear();
				string resourceEditorName = isUsingCustomTemplateContainer ? Control.Storage.Appointments.Mappings.ResourceId : "edtMultiResource";
				object[] resourcesIdList = AppointmentEditValues.Contains(resourceEditorName)
					? AppointmentEditValues.GetValue<ArrayList>(resourceEditorName).ToArray()
					: new object[0];
				foreach (object rawRecourceId in resourcesIdList) {
					object resourceId = ExtensionValueProviderBase.ConvertValue(rawRecourceId, SchedulerIdHelper.GetResourceIdType(Control.Storage.Resources));
					if(resourceId == null)
						resourceId = EmptyResourceId.Id;
					Controller.ResourceIds.Add(resourceId);
				}
			}
			else {
				string resourceEditorName = isUsingCustomTemplateContainer ? Control.Storage.Appointments.Mappings.ResourceId : "edtResource";
				if(AppointmentEditValues.Contains(resourceEditorName))
					Controller.ResourceId = CalculateResouceId(AppointmentEditValues.GetValue<string>(resourceEditorName));
			}
		}
		protected override bool ShouldCreateRecurrence(AppointmentRecurrenceForm frm) {
			Hashtable clientRecurrenceFormData = AppointmentEditValues.GetValue<Hashtable>("RecurrenceFormValue");
			return clientRecurrenceFormData != null ? (bool)clientRecurrenceFormData["Reccurence"] : false;
		}
		protected override bool ShouldShowRecurrence() {
			return Controller.ShouldShowRecurrence;
		}
		protected override AppointmentRecurrenceForm FindRecurrenceFormByID() {
			return null;
		}
		protected override AppointmentRecurrenceControl GetAppointmentRecurrenceControlByForm(AppointmentRecurrenceForm form) {
			return RecurrenceControl;
		}
		protected override AppointmentFormTemplateContainerBase GetTemplateContainer() {
			return new AppointmentFormTemplateContainer(Control);
		}
		protected internal static void CalculateRecurrenceInfo(TimeZoneHelper timeZoneHelper, AppointmentRecurrenceControl recurrenceControl, Appointment pattern, RecurrenceInfo rinfo) {
			AssignRecurrenceInfoProperties(timeZoneHelper, recurrenceControl, pattern, rinfo, pattern.Start);
		}
		#region IEditedAppointment Members
		Appointment[] IEditableAppointments.NewAppointments {
			get {
				Appointment appt = Controller.IsConflictResolved() ? Controller.SourceAppointment : Controller.EditedAppointmentCopy;
				return MVCxAppointmentStorage.GetAppointmentsToInsert(appt); }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get {
				Appointment appt = Controller.IsConflictResolved() ? Controller.SourceAppointment : Controller.EditedAppointmentCopy;
				return MVCxAppointmentStorage.GetAppointmentsToUpdate(appt);
			}
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return Controller.DeletedExceptions.ToArray(); }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			base.Execute(parameters);
		}
		void IEditableAppointments.FinalizeExecute() {
			CanCloseForm = MVCxSchedulerCommandHelper.CanCloseCurrentForm(Control);
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxAppointmentFormAppointmentCopyHelper: AppointmentFormAppointmentCopyHelper {
		public MVCxAppointmentFormAppointmentCopyHelper(MVCxAppointmentFormController controller)
			: base(controller) {
		}
		public new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected override void DeleteExceptions(Appointment appointment) {
			Controller.DeletedExceptions.AddRange(appointment.GetExceptions());
			base.DeleteExceptions(appointment);
		}
	}
	public class MVCxAppointmentDeleteCallbackCommand: AppointmentDeleteCallbackCommand, IEditableAppointments {
		Appointment[] appointmentsToInsert;
		Appointment[] appointmentsToRemove;
		public MVCxAppointmentDeleteCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.appointmentsToInsert = new Appointment[0];
			this.appointmentsToRemove = new Appointment[0];
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected internal new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected override AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new MVCxAppointmentFormController(Control, apt);
		}
		protected override void ExecuteCore() {
			var newAppointment = MVCxAppointmentStorage.GetAppointmentToInsert(Controller.SourceAppointment, true);
			if(newAppointment != null)
				this.appointmentsToInsert = new Appointment[] { newAppointment };
			this.appointmentsToRemove = MVCxAppointmentStorage.GetAppointmentsToRemove(Controller.SourceAppointment);
			base.ExecuteCore();
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return this.appointmentsToInsert; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return this.appointmentsToRemove; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			base.Execute(parameters);
		}
		void IEditableAppointments.FinalizeExecute() {
			CanCloseForm = true;
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxAppointmentInplaceEditorSaveCallbackCommand: AppointmentInplaceEditorSaveCallbackCommand, IEditableAppointments {
		public MVCxAppointmentInplaceEditorSaveCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		protected override bool CanCloseForm {
			get { return MVCxSchedulerCommandHelper.CanCloseCurrentForm(Control); }
		}
		Appointment EditableAppointment {
			get { return ((MVCxAppointmentFormController)Controller).SourceAppointment; }
		}
		protected override AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new MVCxAppointmentFormController(Control, apt);
		}
		protected override bool CanApplyChanges() {
			return Controller != null && base.CanApplyChanges();
		}
		protected override void AssignControllerValues() {
			if (MVCxSchedulerCommandHelper.IsUsingCustomTemplateContainer(Control))
				MVCxSchedulerCommandHelper.AssignControllerValuesInCustomForm((MVCxAppointmentFormController)Controller, FromClientTime);
			else
				Controller.Subject = AppointmentEditValues.GetValue<string>("memSubject");
		}
		protected override void ParseBehindAppointmentData() {
			DateTime? start = null;
			DateTime? end = null;
			string resourceIds = null;
			if(AppointmentEditValues.Contains(InplaceEditorHiddenFieldId.StartDate))
				start = AppointmentEditValues.GetValue<DateTime>(InplaceEditorHiddenFieldId.StartDate);
			if(AppointmentEditValues.Contains(InplaceEditorHiddenFieldId.EndDate))
				end = AppointmentEditValues.GetValue<DateTime>(InplaceEditorHiddenFieldId.EndDate);
			if(AppointmentEditValues.Contains(InplaceEditorHiddenFieldId.ResourceId))
				resourceIds = AppointmentEditValues.GetValue<string>(InplaceEditorHiddenFieldId.ResourceId);
			if (start != null && end != null && resourceIds != null) {
				SetBehindAppointmentData(start.Value, end.Value, resourceIds);
			}
		}
		#region IEditedAppointment Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToInsert(EditableAppointment); }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToUpdate(EditableAppointment); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			base.Execute(parameters);
		}
		void IEditableAppointments.FinalizeExecute() {
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxAppointmentInplaceEditorEditFormCallbackCommand: AppointmentInplaceEditorEditFormCallbackCommand {
		public MVCxAppointmentInplaceEditorEditFormCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		protected override void AssignControllerValues() {
			if (MVCxSchedulerCommandHelper.IsUsingCustomTemplateContainer(Control))
				MVCxSchedulerCommandHelper.AssignControllerValuesInCustomForm((MVCxAppointmentFormController)Controller, FromClientTime);
			else
				Controller.Subject = SchedulerAppointmentEditValues.Current.GetValue<string>("memSubject");
		}
		protected override AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new MVCxAppointmentFormController(Control, apt);
		}
	}
	public class MVCxAppointmentsChangeCommand: AppointmentsChangeCommand, IEditableAppointments {
		public MVCxAppointmentsChangeCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			EditableAppointments = new List<Appointment>();
		}
		protected List<Appointment> EditableAppointments { get; private set; }
		protected override AppointmentFormController CreateAppointmentFormController(Appointment appointment) {
			return new MVCxAppointmentFormController(Control, appointment);
		}
		protected override void ChangeAppointment(AppointmentFormController controller) {
			base.ChangeAppointment(controller);
			var aptFormController = (MVCxAppointmentFormController)controller;
			EditableAppointments.Add(aptFormController.SourceAppointment);
		}
		protected override void CopyAppointment(AppointmentFormController controller) {
			base.CopyAppointment(controller);
			EditableAppointments.Add(controller.EditedAppointmentCopy);
		}
		#region IEditedAppointment Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToInsert(EditableAppointments.ToArray()); }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToUpdate(EditableAppointments.ToArray()); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			base.Execute(parameters);
		}
		void IEditableAppointments.FinalizeExecute() {
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxRecurrentAppointmentDeleteCallbackCommand: RecurrentAppointmentDeleteCallbackCommand, IEditableAppointments {
		Appointment[] appointmentsToInsert;
		Appointment[] appointmentsToUpdate;
		Appointment[] appointmentsToRemove;
		public MVCxRecurrentAppointmentDeleteCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			this.appointmentsToInsert = new Appointment[0];
			this.appointmentsToUpdate = new Appointment[0];
			this.appointmentsToRemove = new Appointment[0];
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		protected override void ParseParameters(string parameters) {
			Action = (RecurrentAppointmentAction)AppointmentEditValues.GetValue<int>("rbAction");
		}
		protected override void DeleteSelectedAppointments() { }
		protected override void ExecuteCore() {
			Appointment appointment = Control.GetEditedAppointment();
			if (Action == RecurrentAppointmentAction.Occurrence) {
				Appointment apptToInsert = MVCxAppointmentStorage.GetAppointmentToInsert(appointment, true);
				if(apptToInsert != null)
					this.appointmentsToInsert = new Appointment[] { apptToInsert };
				else
					this.appointmentsToUpdate = MVCxAppointmentStorage.GetAppointmentsToUpdate(appointment);
			}
			if (Action == RecurrentAppointmentAction.Series)
				this.appointmentsToRemove = MVCxAppointmentStorage.GetAppointmentsToRemove(appointment.RecurrencePattern);
			base.ExecuteCore();
		}
		#region IEditedAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return this.appointmentsToInsert; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.appointmentsToUpdate; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return this.appointmentsToRemove; }
		}
		void IEditableAppointments.Execute(string parameters) {
			base.Execute(parameters);
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			CanCloseForm = true;
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxRecurrentAppointmentEditCallbackCommand: RecurrentAppointmentEditCallbackCommand {
		public MVCxRecurrentAppointmentEditCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		protected override void ParseParameters(string parameters) {
			Action = (RecurrentAppointmentAction)AppointmentEditValues.GetValue<int>("rbAction");
		}
	}
	public class MVCxMenuAppointmentCallbackCommand: MenuAppointmentCallbackCommand, IEditableAppointments {
		IEditableAppointments command;
		public MVCxMenuAppointmentCallbackCommand(ASPxScheduler sheduler)
			: base(sheduler) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		IEditableAppointments Command { get { return command; } }
		protected override SchedulerCommand CreateWebDeleteAppointmentsCommand() {
			return MvcUtils.RenderMode == MvcRenderMode.None ? new MVCxWebDeleteAppointmentsCommand(Control) : base.CreateWebDeleteAppointmentsCommand();
		}
		protected override SchedulerCommand CreateInstanceChangeAppointmentLabelCommand(ISchedulerCommandTarget target, AppointmentLabel label, int labelId) {
			return MvcUtils.RenderMode == MvcRenderMode.None ? new MVCxChangeAppointmentLabelCommand(target, label, labelId) : null;
		}
		protected override SchedulerCommand CreateInstanseChangeAppointmentStatusCommand(ISchedulerCommandTarget target, AppointmentStatus status, int statusId) {
			return MvcUtils.RenderMode == MvcRenderMode.None ? new MVCxChangeAppointmentStatusCommand(target, status, statusId) : null;
		}
		protected override SchedulerCommand CreateRestoreOccurrenceCommand() {
			return MvcUtils.RenderMode == MvcRenderMode.None ? new MVCxRestoreOccurrenceCommand(Control.InnerControl) : null;
		}
		protected void PrepareMenuCommand(string parameters) {
			ParseParameters(parameters);
			this.command = LookupCommandByMenuItemId(MenuItemId, Info) as IEditableAppointments;
		}
		#region IEditedAppointments Members
		Appointment[] newAppointments;
		Appointment[] IEditableAppointments.NewAppointments {
			get { return newAppointments; }
		}
		Appointment[] updatedAppointments = new Appointment[0];
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return updatedAppointments; }
		}
		Appointment[] deletedAppointments = new Appointment[0];
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return deletedAppointments; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			PrepareMenuCommand(parameters);
			if (Command != null) {
				Command.Execute(null);
				this.newAppointments = Command.NewAppointments;
				this.updatedAppointments = Command.UpdatedAppointments;
				this.deletedAppointments = Command.DeletedAppointments;
			}
		}
		void IEditableAppointments.FinalizeExecute() {
			if (Command != null)
				Command.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxWebDeleteAppointmentsCommand: WebDeleteAppointmentsCommand, IEditableAppointments {
		public MVCxWebDeleteAppointmentsCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		bool IsRecurringDeleteCommand(AppointmentBaseCollection appointments) {
			foreach (Appointment appointment in appointments) {
				if (appointment.IsRecurring)
					return true;
			}
			return false;
		}
		#region IEditedAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return !IsRecurringDeleteCommand(Appointments) ? Appointments.ToArray() : new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			Execute();
		}
		void IEditableAppointments.FinalizeExecute() {
		}
		#endregion
	}
	public class MVCxChangeAppointmentLabelCommand: ChangeAppointmentLabelCommand, IEditableAppointments {
		public MVCxChangeAppointmentLabelCommand(ISchedulerCommandTarget target, AppointmentLabel label, int labelId)
			: base(target, label, labelId) {
		}
		public override void Execute() {
			ChangeAppointmentsValues();
		}
		Appointment[] GetNewAppointments() {
			List<Appointment> newAppointments = new List<Appointment>();
			foreach(Appointment appointment in GetEditAppointments()){
				if (appointment.Id == null)
					newAppointments.Add(appointment);
			}
			return newAppointments.ToArray();
		}
		Appointment[] GetUpdatedAppointments() {
			List<Appointment> result = new List<Appointment>();
			foreach (Appointment appointment in GetEditAppointments()) {
				if (appointment.Id != null)
					result.Add(appointment);
			}
			return result.ToArray();
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return GetNewAppointments(); }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return GetUpdatedAppointments(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			Execute();
		}
		void IEditableAppointments.FinalizeExecute() {
		}
		#endregion
	}
	public class MVCxChangeAppointmentStatusCommand: ChangeAppointmentStatusCommand, IEditableAppointments {
		Appointment appointmentToInsert;
		List<Appointment> appointmentsToUpdate;
		public MVCxChangeAppointmentStatusCommand(ISchedulerCommandTarget target, AppointmentStatus status, int statusId)
			: base(target, status, statusId) {
			this.appointmentsToUpdate = new List<Appointment>();
		}
		public override void Execute() {
			ChangeAppointmentsValues();
			foreach(Appointment appointment in GetEditAppointments()){
				if (appointment.Id == null)
					this.appointmentToInsert = appointment;
				else
					this.appointmentsToUpdate.Add(appointment);
			}
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[] { this.appointmentToInsert }; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.appointmentsToUpdate.ToArray(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			Execute();
		}
		void IEditableAppointments.FinalizeExecute() {
		}
		#endregion
	}
	public class MVCxRestoreOccurrenceCommand: RestoreOccurrenceCommand, IEditableAppointments {
		public MVCxRestoreOccurrenceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[] { GetSelectedAppointment() }; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return ((MVCxScheduler)(InnerControl.Owner)).Storage; }
		}
		void IEditableAppointments.Execute(string parameters) {
			Execute();
		}
		void IEditableAppointments.FinalizeExecute() {
		}
		#endregion
	}
	public class MVCxAppointmentClientSideInsertCallbackCommand: AppointmentClientSideInsertCallbackCommand, IEditableAppointments {
		public MVCxAppointmentClientSideInsertCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected override AppointmentFormController CreateAppointmentFormController(Appointment appointment) {
			return new MVCxAppointmentFormController(Control, appointment);
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[] { Controller.SourceAppointment }; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxAppointmentClientSideUpdateCallbackCommand: AppointmentClientSideUpdateCallbackCommand, IEditableAppointments {
		public MVCxAppointmentClientSideUpdateCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected override AppointmentFormController CreateAppointmentFormController(Appointment appointment) {
			return new MVCxAppointmentFormController(Control, appointment);
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToInsert(Controller.SourceAppointment); }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return MVCxAppointmentStorage.GetAppointmentsToUpdate(Controller.SourceAppointment); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxAppointmentClientSideDeleteCallbackCommand: AppointmentClientSideDeleteCallbackCommand, IEditableAppointments {
		Appointment appointmentToInsert;
		Appointment[] appointmentsToRemove;
		public MVCxAppointmentClientSideDeleteCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public new MVCxAppointmentFormController Controller { get { return (MVCxAppointmentFormController)base.Controller; } }
		protected override AppointmentFormController CreateAppointmentFormController(Appointment appoinment) {
			return new MVCxAppointmentFormController(Control, appoinment);
		}
		protected override void ExecuteCore() {
			this.appointmentToInsert = MVCxAppointmentStorage.GetAppointmentToInsert(Controller.SourceAppointment, true);
			this.appointmentsToRemove = MVCxAppointmentStorage.GetAppointmentsToRemove(Controller.SourceAppointment);
			base.ExecuteCore();
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[] { this.appointmentToInsert }; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return this.appointmentsToRemove; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			base.FinalizeExecute();
		}
		#endregion
	}
	public class MVCxDismissReminderCallbackCommand: DismissReminderCallbackCommand, IEditableAppointments {
		List<Appointment> updatedAppointment;
		public MVCxDismissReminderCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			this.updatedAppointment = new List<Appointment>();
		}
		protected override List<int> GetSelectedIndexes() {
			int index = SchedulerAppointmentEditValues.Current.GetValue<int>("lbItems");
			return new List<int>() { index };
		}
		protected override void ProcessReminder(Reminder reminder) {
			base.ProcessReminder(reminder);
			this.updatedAppointment.Add(reminder.Appointment);
		}
		public override void Execute(string parameters) {
			this.updatedAppointment.Clear();
			base.Execute(parameters);
		}
		protected override bool IsCloseReminderForm() {
			return Reminders.Count <= 0;
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.updatedAppointment.ToArray(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			CloseIfEmpty();
		}
		#endregion
	}
	public class MVCxSnoozeReminderCallbackCommand: SnoozeReminderCallbackCommand, IEditableAppointments {
		List<Appointment> updatedAppointment;
		public MVCxSnoozeReminderCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			this.updatedAppointment = new List<Appointment>();
		}
		SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		protected override List<int> GetSelectedIndexes() {
			int index = AppointmentEditValues.GetValue<int>("lbItems");
			return new List<int>() { index };
		}
		protected override TimeSpan GetSnoozeSpanValue() {
			return AppointmentEditValues.GetValue<TimeSpan>("cbSnooze");
		}
		protected override void ProcessReminder(Reminder reminder) {
			base.ProcessReminder(reminder);
			this.updatedAppointment.Add(reminder.Appointment);
		}
		public override void Execute(string parameters) {
			this.updatedAppointment.Clear();
			base.Execute(parameters);
		}
		protected override bool IsCloseReminderForm() {
			return Reminders.Count <= 0;
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.updatedAppointment.ToArray(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			CloseIfEmpty();
		}
		#endregion
	}
	public class MVCxDismissAllRemindersCallbackCommand: DismissAllRemindersCallbackCommand, IEditableAppointments {
		List<Appointment> updatedAppointment;
		public MVCxDismissAllRemindersCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			this.updatedAppointment = new List<Appointment>();
		}
		protected override void ProcessReminder(Reminder reminder) {
			base.ProcessReminder(reminder);
			this.updatedAppointment.Add(reminder.Appointment);
		}
		public override void Execute(string parameters) {
			this.updatedAppointment.Clear();
			base.Execute(parameters);
		}
		protected override bool IsCloseReminderForm() {
			return Reminders.Count <= 0;
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.updatedAppointment.ToArray(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			CloseIfEmpty();
		}
		#endregion
	}
	public class MVCxSnoozeAllRemindersCallbackCommand: SnoozeAllRemindersCallbackCommand, IEditableAppointments {
		List<Appointment> updatedAppointment;
		public MVCxSnoozeAllRemindersCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
			this.updatedAppointment = new List<Appointment>();
		}
		protected override void ProcessReminder(Reminder reminder) {
			base.ProcessReminder(reminder);
			this.updatedAppointment.Add(reminder.Appointment);
		}
		public override void Execute(string parameters) {
			this.updatedAppointment.Clear();
			base.Execute(parameters);
		}
		protected override bool IsCloseReminderForm() {
			return Reminders.Count <= 0;
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return new Appointment[0]; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return this.updatedAppointment.ToArray(); }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return new Appointment[0]; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return (MVCxSchedulerStorage)Control.Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			CloseIfEmpty();
		}
		#endregion
	}
	public class MVCxCloseRemindersFormCallbackCommand: CloseRemindersFormCallbackCommand, IEditableAppointments {
		public MVCxCloseRemindersFormCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		protected override DismissAllRemindersCallbackCommand CreateDismissAllRemindersCallbackCommand() {
			return new MVCxDismissAllRemindersCallbackCommand(Control);
		}
		protected override SnoozeAllRemindersCallbackCommand CreateSnoozeAllRemindersCallbackCommand() {
			return new MVCxSnoozeAllRemindersCallbackCommand(Control);
		}
		#region IEditableAppointments Members
		Appointment[] IEditableAppointments.NewAppointments {
			get { return ((IEditableAppointments)Command).NewAppointments; }
		}
		Appointment[] IEditableAppointments.UpdatedAppointments {
			get { return ((IEditableAppointments)Command).UpdatedAppointments; }
		}
		Appointment[] IEditableAppointments.DeletedAppointments {
			get { return ((IEditableAppointments)Command).DeletedAppointments; }
		}
		MVCxSchedulerStorage IEditableAppointments.Storage {
			get { return ((IEditableAppointments)Command).Storage; }
		}
		void IEditableAppointments.FinalizeExecute() {
			if (Command != null)
				((IEditableAppointments)Command).FinalizeExecute();
		}
		#endregion
	}
	public class MVCxSchedulerCommandHelper {
		static SchedulerAppointmentEditValues AppointmentEditValues { get { return SchedulerAppointmentEditValues.Current; } }
		public static bool CanCloseCurrentForm(ASPxScheduler scheduler) {
			SchedulerStatusInfoCollection statusInfoCollection = GetStatusInfoCore(scheduler);
			ViewContext viewContext = ((IViewContext)scheduler).ViewContext;
			return (statusInfoCollection == null || statusInfoCollection.Count == 0) && (viewContext == null || viewContext.ViewData.ModelState.IsValid);
		}
		public static SchedulerStatusInfoCollection GetStatusInfo(string name) {
			var scheduler = HttpContext.Current.Items.Keys.OfType<ASPxScheduler>().Where(s => s.ClientID == name).FirstOrDefault();
			return scheduler != null ? SchedulerStatusInfoHelper.GetSortedSchedulerStatusInfos(scheduler) : null;
		}
		static SchedulerStatusInfoCollection GetStatusInfoCore(ASPxScheduler scheduler) {
			SchedulerStatusInfoCollection statusInfoCollection = SchedulerStatusInfoHelper.GetSortedSchedulerStatusInfos(scheduler);
			if(statusInfoCollection != null)
				return statusInfoCollection;
			var context = HttpContext.Current.Items;
			foreach (object key in context.Keys) {
				var contextScheduler = key as ASPxScheduler;
				if(contextScheduler != null && contextScheduler.ClientID == scheduler.ClientID) {
					CopyStatusInfoCollection(scheduler, SchedulerStatusInfoHelper.GetSortedSchedulerStatusInfos(contextScheduler));
					break;
				}
			}
			return SchedulerStatusInfoHelper.GetSortedSchedulerStatusInfos(scheduler);
		}
		static void CopyStatusInfoCollection(ASPxScheduler scheduler, SchedulerStatusInfoCollection statusInfoCollection) {
			foreach(var statusInfo in statusInfoCollection)
				SchedulerStatusInfoHelper.AddStatusInfo(scheduler, statusInfo);
		}
		public static bool IsUsingCustomTemplateContainer(ASPxScheduler scheduler) {
			string rawValue = HttpContext.Current.Request[scheduler.ID + "_enabledAppointmentFormTemplateControl"];
			return !string.IsNullOrEmpty(rawValue) ? bool.Parse(rawValue) : false;
		}
		public static void AssignControllerValuesInCustomForm(MVCxAppointmentFormController controller, Func<DateTime, DateTime> fromClientTime) {
			ASPxAppointmentMappingInfo mappings = controller.Control.Storage.Appointments.Mappings;
			if(AppointmentEditValues.Contains(mappings.AllDay))
				controller.AllDay = AppointmentEditValues.GetValue<bool>(mappings.AllDay);
			if(AppointmentEditValues.Contains(mappings.Start))
				controller.EditedAppointmentCopy.Start = fromClientTime(AppointmentEditValues.GetValue<DateTime>(mappings.Start));
			if(AppointmentEditValues.Contains(mappings.End))
				controller.EditedAppointmentCopy.End = fromClientTime(AppointmentEditValues.GetValue<DateTime>(mappings.End));
			if(AppointmentEditValues.Contains(mappings.Subject))
				controller.Subject = AppointmentEditValues.GetValue<string>(mappings.Subject);
			if(AppointmentEditValues.Contains(mappings.Location))
				controller.Location = AppointmentEditValues.GetValue<string>(mappings.Location);
			if(AppointmentEditValues.Contains(mappings.Description))
				controller.Description = AppointmentEditValues.GetValue<string>(mappings.Description);
			if(AppointmentEditValues.Contains(mappings.Status))
				controller.StatusId = AppointmentEditValues.GetValue<Int32>(mappings.Status);
			if(AppointmentEditValues.Contains(mappings.Label))
				controller.LabelId = AppointmentEditValues.GetValue<Int32>(mappings.Label);
			if(AppointmentEditValues.Contains("HasReminder"))
				controller.HasReminder = AppointmentEditValues.GetValue<bool>("HasReminder");
			foreach(string memberName in SchedulerReminderInfoHelper.GetReminderEditMemberNames()) {
				if(AppointmentEditValues.Contains(memberName))
					controller.ReminderTimeBeforeStart = AppointmentEditValues.GetValue<TimeSpan>(memberName);
			}
		}
	}
}
