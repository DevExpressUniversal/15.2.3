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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.XtraScheduler.Data;
	using DevExpress.XtraScheduler;
	using DevExpress.XtraScheduler.Native;
	using DevExpress.XtraScheduler.iCalendar;
	using DevExpress.XtraScheduler.iCalendar.Native;
	using DevExpress.XtraScheduler.Exchange;
	using DevExpress.Web.Internal;
	using DevExpress.Utils.Controls;
	public class MVCxSchedulerStorage: ASPxSchedulerStorage {
		public MVCxSchedulerStorage()
			: base() {
		}
		protected internal MVCxSchedulerStorage(MVCxScheduler owner)
			: base(owner) {
		}
		public new MVCxAppointmentStorage Appointments { get { return (MVCxAppointmentStorage)base.Appointments; } }
		public new MVCxResourceStorage Resources { get { return (MVCxResourceStorage)base.Resources; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ExportToICalendar(Stream stream) {
			base.ExportToICalendar(stream);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ExportToICalendar(string path) {
			base.ExportToICalendar(path);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void ExportToOutlook() {
			base.ExportToOutlook();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ExportToVCalendar(Stream stream) {
			base.ExportToVCalendar(stream);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ExportToVCalendar(string path) {
			base.ExportToVCalendar(path);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppointmentExporter CreateOutlookExporter() {
			return base.CreateOutlookExporter();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppointmentExportSynchronizer CreateOutlookExportSynchronizer() {
			return base.CreateOutlookExportSynchronizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppointmentImporter CreateOutlookImporter() {
			return base.CreateOutlookImporter();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppointmentImportSynchronizer CreateOutlookImportSynchronizer() {
			return base.CreateOutlookImportSynchronizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ImportFromICalendar(Stream stream) {
			base.ImportFromICalendar(stream);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ImportFromICalendar(string path) {
			base.ImportFromICalendar(path);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void ImportFromOutlook() {
			base.ImportFromOutlook();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ImportFromVCalendar(Stream stream) {
			base.ImportFromVCalendar(stream);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void ImportFromVCalendar(string path) {
			base.ImportFromVCalendar(path);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SynchronizeOutlookWithStorage(string outlookEntryIdFieldName) {
			base.SynchronizeOutlookWithStorage(outlookEntryIdFieldName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SynchronizeStorageWithOutlook(string outlookEntryIdFieldName) {
			base.SynchronizeStorageWithOutlook(outlookEntryIdFieldName);
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected override AppointmentStorageBase CreateAppointmentStorage() {
			return new MVCxAppointmentStorage(this);
		}
		protected override ResourceStorageBase CreateResourceStorage() {
			return new MVCxResourceStorage(this);
		}
		protected internal new void Assign(ISchedulerStorageBase source) {
			base.Assign(source);
		}
	}
	public class MVCxAppointmentStorage: ASPxAppointmentStorage {
		public MVCxAppointmentStorage()
			: base(null) {
		}
		protected internal MVCxAppointmentStorage(MVCxSchedulerStorage storage)
			: base(storage) {
		}
		public override bool UnboundMode { get { return MvcUtils.RenderMode != MvcRenderMode.None ? base.UnboundMode : false; } }
		public new MVCxAppointmentCustomFieldMappingCollection CustomFieldMappings { get { return (MVCxAppointmentCustomFieldMappingCollection)base.CustomFieldMappings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new AppointmentDataManager DataManager { get { return base.DataManager; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool CommitIdToDataSource { get { return base.CommitIdToDataSource; } set { base.CommitIdToDataSource = value; } }
		protected internal new MappingCollection ActualMappings { get { return base.ActualMappings; } }
		public void Assign(MVCxAppointmentStorage source) {
			Assign((PersistentObjectStorage<Appointment>)source);
		}
		protected override void CheckForExistenceStorage(ISchedulerStorageBase storage) { }
		protected override DataManager<Appointment> CreateDataManager() {
			return new MVCxAppointmentDataManager();
		}
		protected override CustomFieldMappingCollectionBase<Appointment> CreateCustomMappingsCollection() {
			return new MVCxAppointmentCustomFieldMappingCollection();
		}
		protected override void OnObjectStateChanged(object sender, PersistentObjectStateChangedEventArgs e) { }
		internal static Appointment[] GetAppointmentsToInsert(params Appointment[] editableAppointments) {
			var appointmentsToInsert = new List<Appointment>();
			foreach(var appointment in editableAppointments) {
				var newAppointment = GetAppointmentToInsert(appointment, false);
				if(newAppointment != null)
					appointmentsToInsert.Add(newAppointment);
			}
			return appointmentsToInsert.ToArray();
		}
		internal static Appointment GetAppointmentToInsert(Appointment appointment, bool isDeleteCommand) {
			if (appointment == null)
				return null;
			Appointment appointmentToInsert = null;
			if (isDeleteCommand)
				appointmentToInsert = appointment.IsOccurrence ? appointment : null;
			else
				appointmentToInsert = appointment.Id == null ? appointment : null;
			return appointmentToInsert;
		}
		internal static Appointment[] GetAppointmentsToUpdate(params Appointment[] appointments) {
			var apptsToUpdate = new List<Appointment>();
			foreach(var appt in appointments) {
				if(appt == null || appt.Id == null) continue;
				apptsToUpdate.Add(appt);
			}
			return apptsToUpdate.ToArray();
		}
		internal static Appointment[] GetAppointmentsToRemove(Appointment appointment) {
			if (appointment == null)
				return new Appointment[0];
			List<Appointment> appointmentsToRemove = new List<Appointment>();
			switch (appointment.Type) {
				case AppointmentType.Normal:
					appointmentsToRemove.Add(appointment);
					break;
				case AppointmentType.Pattern:
					appointmentsToRemove.Add(appointment);
					appointmentsToRemove.AddRange(appointment.GetExceptions());
					break;
			}
			return appointmentsToRemove.ToArray();
		}
		public override void CommitNewObject(Appointment obj) { }
	}
	public class MVCxAppointmentCustomFieldMappingCollection: ASPxAppointmentCustomFieldMappingCollection {
		public MVCxAppointmentCustomFieldMappingCollection()
			: base() {
		}
		public new ASPxAppointmentCustomFieldMapping Add(ASPxAppointmentCustomFieldMapping mapping) {
			base.Add(mapping);
			return mapping;
		}
		public ASPxAppointmentCustomFieldMapping Add() {
			ASPxAppointmentCustomFieldMapping mapping = new ASPxAppointmentCustomFieldMapping();
			return Add(mapping);
		}
		public void Add(Action<ASPxAppointmentCustomFieldMapping> method) {
			method(Add());
		}
		public ASPxAppointmentCustomFieldMapping Add(string name, string member) {
			ASPxAppointmentCustomFieldMapping mapping = new ASPxAppointmentCustomFieldMapping(name, member);
			return Add(mapping);
		}
	}
	public class MVCxResourceStorage: ASPxResourceStorage {
		public MVCxResourceStorage()
			: base(null) {
		}
		protected internal MVCxResourceStorage(MVCxSchedulerStorage storage)
			: base(storage) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ResourceDataManager DataManager { get { return base.DataManager; } }
		public void Assign(MVCxResourceStorage source) {
			base.Assign(source);
		}
		protected override void CheckForExistenceStorage(ISchedulerStorageBase storage) { }
	}
	public class MVCxSchedulerOptionsToolTips : ASPxSchedulerOptionsToolTips {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string AppointmentToolTipUrl { get { return base.AppointmentToolTipUrl; } set { base.AppointmentToolTipUrl = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string AppointmentDragToolTipUrl { get { return base.AppointmentDragToolTipUrl; } set { base.AppointmentDragToolTipUrl = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string SelectionToolTipUrl { get { return base.SelectionToolTipUrl; } set { base.SelectionToolTipUrl = value; } }
		protected internal string AppointmentToolTipTemplateContent { get; set; }
		protected internal Action AppointmentToolTipTemplateContentMethod { get; set; }
		protected internal string AppointmentDragToolTipTemplateContent { get; set; }
		protected internal Action AppointmentDragToolTipTemplateContentMethod { get; set; }
		protected internal string SelectionToolTipTemplateContent { get; set; }
		protected internal Action SelectionToolTipTemplateContentMethod { get; set; }
		public void SetAppointmentToolTipTemplateContent(string content) { AppointmentToolTipTemplateContent = content; }
		public void SetAppointmentToolTipTemplateContent(Action method) { AppointmentToolTipTemplateContentMethod = method; }
		public void SetAppointmentDragToolTipTemplateContent(string content) { AppointmentDragToolTipTemplateContent = content; }
		public void SetAppointmentDragToolTipTemplateContent(Action method) { AppointmentDragToolTipTemplateContentMethod = method; }
		public void SetSelectionToolTipTemplateContent(string content) { SelectionToolTipTemplateContent = content; }
		public void SetSelectionToolTipTemplateContent(Action method) { SelectionToolTipTemplateContentMethod = method; }
	}
	public class MVCxSchedulerClientSideEvents : SchedulerClientSideEvents {
		const string ToolTipDisplayingName = "ToolTipDisplaying";
		public string ToolTipDisplaying {
			get { return GetEventHandler(ToolTipDisplayingName); }
			set { SetEventHandler(ToolTipDisplayingName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(ToolTipDisplayingName);
		}
	}
	public class MVCxSchedulerOptionsForms: ASPxSchedulerOptionsForms {
		public MVCxSchedulerOptionsForms()
			: base() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string AppointmentFormTemplateUrl {
			get { return base.AppointmentFormTemplateUrl; }
			set { base.AppointmentFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string AppointmentInplaceEditorFormTemplateUrl {
			get { return base.AppointmentInplaceEditorFormTemplateUrl; }
			set { base.AppointmentInplaceEditorFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string GotoDateFormTemplateUrl {
			get { return base.GotoDateFormTemplateUrl; }
			set { base.GotoDateFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string RecurrentAppointmentDeleteFormTemplateUrl {
			get { return base.RecurrentAppointmentDeleteFormTemplateUrl; }
			set { base.RecurrentAppointmentDeleteFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string RecurrentAppointmentEditFormTemplateUrl {
			get { return base.RecurrentAppointmentDeleteFormTemplateUrl; }
			set { base.RecurrentAppointmentDeleteFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string RemindersFormTemplateUrl {
			get { return base.RemindersFormTemplateUrl; }
			set { base.RemindersFormTemplateUrl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string MessageBoxTemplateUrl {
			get { return base.MessageBoxTemplateUrl; }
			set { base.MessageBoxTemplateUrl = value; }
		}
		public string RecurrenceFormName { get; set; }
		public object AppointmentFormRouteValues { get; set; }
		public object AppointmentInplaceEditorFormRouteValues { get; set; }
		public object GotoDateFormRouteValues { get; set; }
		public object RecurrentAppointmentDeleteFormRouteValues { get; set; }
		public object RemindersFormRouteValues { get; set; }
		protected internal string AppointmentFormTemplateContent { get; set; }
		protected internal Action<AppointmentFormTemplateContainer> AppointmentFormTemplateContentMethod { get; set; }
		protected internal string AppointmentInplaceEditorFormTemplateContent { get; set; }
		protected internal Action<AppointmentInplaceEditorTemplateContainer> AppointmentInplaceEditorFormTemplateContentMethod { get; set; }
		protected internal string GotoDateFormTemplateContent { get; set; }
		protected internal Action<GotoDateFormTemplateContainer> GotoDateFormTemplateContentMethod { get; set; }
		protected internal string RecurrentAppointmentDeleteFormTemplateContent { get; set; }
		protected internal Action<RecurrentAppointmentDeleteFormTemplateContainer> RecurrentAppointmentDeleteFormTemplateContentMethod { get; set; }
		protected internal string RecurrentAppointmentEditFormTemplateContent { get; set; }
		protected internal Action<RecurrentAppointmentDeleteFormTemplateContainer> RecurrentAppointmentEditFormTemplateContentMethod { get; set; }
		protected internal string RemindersFormTemplateContent { get; set; }
		protected internal Action<RemindersFormTemplateContainer> RemindersFormTemplateContentMethod { get; set; }
		public void SetAppointmentFormTemplateContent(string content) {
			AppointmentFormTemplateContent = content;
		}
		public void SetAppointmentFormTemplateContent(Action<AppointmentFormTemplateContainer> method) {
			AppointmentFormTemplateContentMethod = method;
		}
		public void SetAppointmentInplaceEditorFormTemplateContent(string content) {
			AppointmentInplaceEditorFormTemplateContent = content;
		}
		public void SetAppointmentInplaceEditorFormTemplateContent(Action<AppointmentInplaceEditorTemplateContainer> method) {
			AppointmentInplaceEditorFormTemplateContentMethod = method;
		}
		public void SetGotoDateFormTemplateContent(string content) {
			GotoDateFormTemplateContent = content;
		}
		public void SetGotoDateFormTemplateContent(Action<GotoDateFormTemplateContainer> method) {
			GotoDateFormTemplateContentMethod = method;
		}
		public void SetRecurrentAppointmentDeleteFormTemplateContent(string content) {
			RecurrentAppointmentDeleteFormTemplateContent = content;
		}
		public void SetRecurrentAppointmentDeleteFormTemplateContent(Action<RecurrentAppointmentDeleteFormTemplateContainer> method) {
			RecurrentAppointmentDeleteFormTemplateContentMethod = method;
		}
		public void SetRecurrentAppointmentEditFormTemplateContent(string content) {
			RecurrentAppointmentEditFormTemplateContent = content;
		}
		public void SetRecurrentAppointmentEditFormTemplateContent(Action<RecurrentAppointmentDeleteFormTemplateContainer> method) {
			RecurrentAppointmentEditFormTemplateContentMethod = method;
		}
		public void SetRemindersFormTemplateContent(string content) {
			RemindersFormTemplateContent = content;
		}
		public void SetRemindersFormTemplateContent(Action<RemindersFormTemplateContainer> method) {
			RemindersFormTemplateContentMethod = method;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			MVCxSchedulerOptionsForms optionsForms = options as MVCxSchedulerOptionsForms;
			if (optionsForms != null) {
				RecurrenceFormName = optionsForms.RecurrenceFormName;
				AppointmentFormRouteValues = optionsForms.AppointmentFormRouteValues;
				AppointmentInplaceEditorFormRouteValues = optionsForms.AppointmentInplaceEditorFormRouteValues;
				GotoDateFormRouteValues = optionsForms.GotoDateFormRouteValues;
				RecurrentAppointmentDeleteFormRouteValues = optionsForms.RecurrentAppointmentDeleteFormRouteValues;
				RemindersFormRouteValues = optionsForms.RemindersFormRouteValues;
			}
		}
	}
	public class MVCxTimeRulerCollection: TimeRulerCollection {
		public MVCxTimeRulerCollection()
			: base() {
		}
		public void Add(Action<TimeRuler> method) {
			method(Add());
		}
		public TimeRuler Add() {
			TimeRuler ruler = new TimeRuler();
			return Add(ruler); ;
		}
		public new TimeRuler Add(TimeRuler ruler) {
			base.Add(ruler);
			return ruler;
		}
		public virtual void Assign(MVCxTimeRulerCollection timeRulerCollection) {
			base.Assign(timeRulerCollection);
		}
	}
	public class MVCxSchedulerExportSettings {
		MVCxICalendarExportSettings iCalendarExportSettings;
		public MVCxSchedulerExportSettings() {
			this.iCalendarExportSettings = new MVCxICalendarExportSettings();
		}
		public MVCxICalendarExportSettings ICalendar { get { return iCalendarExportSettings; } }
	}
	public class MVCxICalendarExportSettings: MVCxAppointmentExchangerSettings {
		public MVCxICalendarExportSettings() {
			CustomPropertyIdentifier = iCalendarSR.DXPropertyIdentifier;
			ProductIdentifier = iCalendarSR.DXProductIdentifier;
		}
		public string CustomPropertyIdentifier { get; set; }
		public string ProductIdentifier { get; set; }
		public iCalendarStructureCreatedEventHandler CalendarStructureCreated { get; set; }
		public AppointmentExportingEventHandler AppointmentExporting { get; set; }
		public AppointmentExportedEventHandler AppointmentExported { get; set; }
		protected internal override void AssignToAppointmentExchanger(AppointmentExchanger appointmentExchanger) {
			base.AssignToAppointmentExchanger(appointmentExchanger);
			iCalendarExporter iCalendarExporter = appointmentExchanger as iCalendarExporter;
			if (iCalendarExporter != null) {
				iCalendarExporter.CustomPropertyIdentifier = CustomPropertyIdentifier;
				iCalendarExporter.ProductIdentifier = ProductIdentifier;
				iCalendarExporter.CalendarStructureCreated += CalendarStructureCreated;
				iCalendarExporter.AppointmentExporting += AppointmentExporting;
				iCalendarExporter.AppointmentExported += AppointmentExported;
			}
		}
	}
	public class MVCxSchedulerImportSettings {
		MVCxICalendarImportSettings iCalendarImportSettings;
		public MVCxSchedulerImportSettings() {
			this.iCalendarImportSettings = new MVCxICalendarImportSettings();
		}
		public MVCxICalendarImportSettings ICalendar { get { return iCalendarImportSettings; } }
	}
	public class MVCxICalendarImportSettings: MVCxAppointmentExchangerSettings {
		public MVCxICalendarImportSettings() {
			CustomPropertyIdentifier = iCalendarSR.DXPropertyIdentifier;
		}
		public string CustomPropertyIdentifier { get; set; }
		public iCalendarStructureCreatedEventHandler CalendarStructureCreated { get; set; }
		protected internal override void AssignToAppointmentExchanger(AppointmentExchanger appointmentExchanger) {
			base.AssignToAppointmentExchanger(appointmentExchanger);
			iCalendarImporter importer = appointmentExchanger as iCalendarImporter;
			if (importer != null) {
				importer.CustomPropertyIdentifier = CustomPropertyIdentifier;
				importer.CalendarStructureCreated += CalendarStructureCreated;
			}
		}
	}
	public abstract class MVCxAppointmentExchangerSettings {
		public ExchangeAppointmentEventHandler GetAppointmentForeignId { get; set; }
		public ExchangeExceptionEventHandler OnException { get; set; }
		protected internal virtual void AssignToAppointmentExchanger(AppointmentExchanger appointmentExchanger) {
			if (appointmentExchanger == null)
				return;
			appointmentExchanger.GetAppointmentForeignId += GetAppointmentForeignId;
			appointmentExchanger.OnException += OnException;
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.XtraScheduler.Data;
	using DevExpress.XtraScheduler;
	using System.Reflection;
	using DevExpress.Web.ASPxScheduler.Localization;
	using DevExpress.Utils.Localization;
	using DevExpress.XtraScheduler.Localization;
	public class MVCxAppointmentDataManager: ASPxAppointmentDataManager {
		public MVCxAppointmentDataManager()
			: base() {
		}
		protected override SchedulerDataController CreateDataController() {
			return new MVCxSchedulerDataController();
		}
	}
	public class SchedulerReminderInfoHelper {
		public static IEnumerable<string> GetReminderEditMemberNames() {
			List<string> result = new List<string>();
			const string ReminderPrefix = "Reminder.";
			foreach (PropertyInfo propertyInfo in typeof(Reminder).GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
				if (propertyInfo.CanWrite && propertyInfo.CanRead)
					result.Add(ReminderPrefix + propertyInfo.Name);
			}
			return result;
		}
	}
	public class MVCxSchedulerCoreResLocalizer: ASPxSchedulerCoreResLocalizer {
		public MVCxSchedulerCoreResLocalizer()
			: base() {
		}
		internal static XtraLocalizer<SchedulerStringId> CreateResLocalizerInstance() {
			return new MVCxSchedulerCoreResLocalizer();
		}
	}
}
