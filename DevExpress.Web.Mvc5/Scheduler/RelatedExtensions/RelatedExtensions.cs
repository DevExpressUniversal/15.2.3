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
using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxScheduler.Controls;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraScheduler;
	public class TimeZoneEditExtension: ExtensionBase {
		public TimeZoneEditExtension(TimeZoneEditSettings settings)
			: base(settings) {
		}
		public TimeZoneEditExtension(TimeZoneEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxTimeZoneEdit Control {
			get { return (MVCxTimeZoneEdit)base.Control; }
		}
		protected internal new TimeZoneEditSettings Settings {
			get { return (TimeZoneEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.MasterControlID = Settings.SchedulerName;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			IMasterControl masterControl = CreateMasterControl();
			return new MVCxTimeZoneEdit(masterControl);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnLoad();
		}
		IMasterControl CreateMasterControl() {
			MVCxScheduler scheduler = new MVCxScheduler();
			scheduler.ID = Settings.SchedulerName;
			ApplyThemeToControl(scheduler);
			return scheduler;
		}
	}
	public class AppointmentRecurrenceFormExtension: ExtensionBase {
		public AppointmentRecurrenceFormExtension(AppointmentRecurrenceFormSettings settings)
			: base(settings) {
		}
		public AppointmentRecurrenceFormExtension(AppointmentRecurrenceFormSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new AppointmentRecurrenceForm Control {
			get { return (AppointmentRecurrenceForm)base.Control; }
		}
		protected internal new AppointmentRecurrenceFormSettings Settings {
			get { return (AppointmentRecurrenceFormSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.EnableScriptSupport = Settings.EnableScriptSupport;
			Control.WeekDays = Settings.WeekDays;
			Control.WeekOfMonth = Settings.WeekOfMonth;
			Control.DayNumber = Settings.DayNumber;
			Control.Month = Settings.Month;
			Control.OccurrenceCount = Settings.OccurrenceCount;
			Control.Start = Settings.Start;
			Control.End = Settings.End;
			Control.RecurrenceRange = Settings.RecurrenceRange;
			Control.RecurrenceType = Settings.RecurrenceType;
			Control.IsRecurring = Settings.IsRecurring;
			Control.IsFormRecreated = Settings.IsFormRecreated;
			Control.Periodicity = Settings.Periodicity;
			Control.EnableHourlyRecurrence = Settings.EnableHourlyRecurrence;
		}
		public AppointmentRecurrenceFormExtension Bind(RecurrenceInfo recurrenceInfo) {
			if (recurrenceInfo != null) {
				Control.DayNumber = recurrenceInfo.DayNumber;
				Control.Start = recurrenceInfo.Start;
				Control.End = recurrenceInfo.End;
				Control.Month = recurrenceInfo.Month;
				Control.OccurrenceCount = recurrenceInfo.OccurrenceCount;
				Control.Periodicity = recurrenceInfo.Periodicity;
				Control.WeekDays = recurrenceInfo.WeekDays;
				Control.WeekOfMonth = recurrenceInfo.WeekOfMonth;
				Control.RecurrenceType = recurrenceInfo.Type;
				Control.RecurrenceRange = recurrenceInfo.Range;
			}
			return this;
		}
		public static RecurrenceInfo GetValue(string name, Appointment pattern) {
			return GetValue(name, pattern, null);
		}
		public static RecurrenceInfo GetValue(string name, Appointment pattern, TimeZoneHelper timeZoneHelper) {
			Hashtable clientData = EditorValueProvider.GetValue(name) as Hashtable;
			if (clientData == null || !ParseIsRecurrenceProperty(clientData))
				return null;
			var control = new MVCxAppointmentRecurrenceControl(clientData);
			control.EnsureChildControls();
			return GetRecurrenceInfoByControl(control, pattern, timeZoneHelper);
		}
		static bool ParseIsRecurrenceProperty(IDictionary clientData) {
			const string ReccurencePropertyName = "Reccurence";
			return clientData.Contains(ReccurencePropertyName) ? Convert.ToBoolean(clientData[ReccurencePropertyName]) : false;
		}
		static RecurrenceInfo GetRecurrenceInfoByControl(MVCxAppointmentRecurrenceControl control, Appointment pattern, TimeZoneHelper timeZoneHelper) {
			RecurrenceInfo reccurenceInfo = new RecurrenceInfo();
			if (pattern != null)
				MVCxAppointmentFormSaveCallbackCommand.CalculateRecurrenceInfo(timeZoneHelper, control, pattern, reccurenceInfo);
			return reccurenceInfo;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			MVCxScheduler scheduler = new MVCxScheduler();
			ApplyThemeToControl(scheduler);
			AppointmentRecurrenceForm appointmentRecurrenceForm = new AppointmentRecurrenceForm();
			appointmentRecurrenceForm.EditorsInfo = new ASPxScheduler.Internal.EditorsInfo(scheduler, Settings.Styles.FormEditors, Settings.Images.FormEditors, Settings.Styles.Buttons);
			return appointmentRecurrenceForm;
		}
	}
	public class SchedulerStatusInfoExtension: ExtensionBase {
		public SchedulerStatusInfoExtension(SchedulerStatusInfoSettings settings)
			: base(settings) {
		}
		public SchedulerStatusInfoExtension(SchedulerStatusInfoSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxSchedulerStatusInfo Control {
			get { return (ASPxSchedulerStatusInfo)base.Control; }
		}
		protected internal new SchedulerStatusInfoSettings Settings {
			get { return (SchedulerStatusInfoSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.MasterControlID = Settings.SchedulerName;
			Control.Priority = Settings.Priority;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new ASPxSchedulerStatusInfo();
		}
	}
	public class DateNavigatorExtension: ExtensionBase {
		MVCxScheduler scheduler;
		SchedulerSettings schedulerSettings;
		public DateNavigatorExtension(SchedulerSettings settings)
			: base(settings) {
		}
		public DateNavigatorExtension(SchedulerSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxDateNavigator Control {
			get { return (MVCxDateNavigator)base.Control; }
		}
		protected internal MVCxScheduler Scheduler {
			get {
				if (scheduler == null)
					scheduler = ExtensionsFactory.RenderedExtensions.ContainsKey(SchedulerSettings.Name) && MvcUtils.CallbackName == SchedulerSettings.Name
						? ((SchedulerExtension)ExtensionsFactory.RenderedExtensions[SchedulerSettings.Name]).Control
						: SchedulerExtension.CreateExtension(SchedulerSettings, null, null).Control;
				return scheduler; 
			}
		}
		protected internal new DateNavigatorSettings Settings {
			get { return (DateNavigatorSettings)base.Settings; }
		}
		protected internal SchedulerSettings SchedulerSettings {
			get { return schedulerSettings; }
		}
		protected override void ApplySettings(SettingsBase schedulerSettings) {
			this.schedulerSettings = (SchedulerSettings)schedulerSettings;
			base.Settings = this.schedulerSettings.DateNavigatorExtensionSettings;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.MasterControlID = SchedulerSettings.Name;
			Control.Properties.Assign(Settings.Properties);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnLoad();
		}
		protected internal override void DisposeControl() {
			base.DisposeControl();
			Scheduler.Dispose();
		}
		protected internal static void RegisterControlInScheduler(SchedulerSettings settings) {
			DateNavigatorExtension dateNavigator = ExtensionsFactory.InstanceInternal.DateNavigator(settings);
			dateNavigator.AssignRenderProperties();
			dateNavigator.PrepareControl();
			RenderUtils.LoadPostDataRecursive(dateNavigator.Control, dateNavigator.PostDataCollection);
		}
		public DateNavigatorExtension Bind(object appointmentDataObject) {
			return Bind(appointmentDataObject, null);
		}
		public DateNavigatorExtension Bind(object appointmentDataObject, object resourceDataObject) {
			Scheduler.AppointmentDataSource = appointmentDataObject;
			Scheduler.ResourceDataSource = resourceDataObject;
			Scheduler.DataBind();
			return this;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxDateNavigator(Scheduler);
		}
	}
}
