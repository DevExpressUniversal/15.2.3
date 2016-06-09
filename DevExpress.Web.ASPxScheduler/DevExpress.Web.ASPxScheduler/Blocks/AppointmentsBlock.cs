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
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class AppointmentsBlock : ASPxSchedulerControlBlock {
		bool isAppointmentsRendered;
		public AppointmentsBlock(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string ContentControlID { get { return SchedulerIdHelper.AppointmentsBlockId; } }
		protected internal SchedulerViewBase View { get { return Owner.ActiveView; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderAppointments; } }
		public bool IsAppointmentsRendered { get { return isAppointmentsRendered; } }
		#endregion
		protected internal override void CreateControlHierarchyCore(Control parent) {
			if (Owner.ShouldRenderAppointmentEarly()) 
				RenderAppointmentsAndAssingId(parent);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
			if (!IsAppointmentsRendered) 
				RenderAppointmentsAndAssingId(parent);
		}
		protected internal virtual void RenderAppointmentsAndAssingId(Control parent) {
			RenderAppointments(parent);
			AssignAppointmentsIds(parent);
			this.isAppointmentsRendered = true;
		}
		protected internal override void PrepareControlHierarchyCore() {
		}
		public void EnsureAppointments() { 
			if (!IsAppointmentsRendered && ContentControl != null) {
				RenderAppointmentsAndAssingId(ContentControl);
				EnsureChildControlsRecursive(this);
			}
		}
		public virtual void RenderAppointments(Control parent) {
			AppointmentBaseCollection permanentAppointments = GetPermanentAppointments();
			NotificationCollection<Appointment> nonpermanentAppointments = GetNonpermanentAppointments();
			RenderAppointmentsCore(parent, permanentAppointments, nonpermanentAppointments, false);
		}
		protected internal virtual AppointmentBaseCollection GetAppointmentns() {
			AppointmentBaseCollection collection = new AppointmentBaseCollection();
			collection.AddRange(GetPermanentAppointments());
			collection.AddRange(GetNonpermanentAppointments());
			return collection;
		}
		protected internal virtual AppointmentBaseCollection GetPermanentAppointments() {
			AppointmentBaseCollection collection = new AppointmentBaseCollection();
			collection.AddRange(View.FilteredAppointments);
			return collection;
		}
		protected NotificationCollection<Appointment> GetNonpermanentAppointments() {
			NotificationCollection<Appointment> result = new NotificationCollection<Appointment>();
			if (Owner.EditableAppointment.IsNewlyCreated && Owner.ActiveFormType == SchedulerFormType.AppointmentInplace)
				result.Add(Owner.EditableAppointment.NewInstance);
			return result;
		}
		protected internal virtual void RenderAppointmentsCore(Control parent, AppointmentBaseCollection appointments, NotificationCollection<Appointment> nonpermanentAppointments, bool alternate) {
			ISchedulerWebViewInfoBase viewInfo = Owner.ViewInfo;
			IContinuousCellsInfosCalculator cellsInfosCalculator = View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(View, alternate);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = cellsInfosCalculator.Calculate(viewInfo, View.VisibleResources);
			AppointmentContentLayoutCalculator contentCalculator = View.FactoryHelper.CreateAppointmentContentLayoutCalculator(viewInfo, alternate);
			AppointmentBaseLayoutCalculator appointmentLayoutCalculator = View.FactoryHelper.CreateAppointmentLayoutCalculator(viewInfo, contentCalculator, alternate);
			ASPxScheduler control = View.Control;
			control.SubscribeAppointmentContentLayoutCalculatorEvents(contentCalculator);
			try {
				LayoutAppointments(parent, appointments, cellsInfos, appointmentLayoutCalculator);
				LayoutNonpermanentAppointments(parent, nonpermanentAppointments, cellsInfos, appointmentLayoutCalculator);
			} finally {
				control.UnsubscribeAppointmentContentLayoutCalculatorEvents(contentCalculator);
			}
		}
		protected internal virtual void LayoutAppointments(Control parent, AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfo, AppointmentBaseLayoutCalculator layoutCalculator) {
			WebAppointmentsLayoutResult layoutResult = new WebAppointmentsLayoutResult();
			layoutCalculator.CalculateLayout(layoutResult, appointments, cellsInfo);
			int count = layoutResult.AppointmentControls.Count;
			for (int i = 0; i < count; i++)
				parent.Controls.Add(layoutResult.AppointmentControls[i]);
		}
		protected internal virtual void LayoutNonpermanentAppointments(Control parent, NotificationCollection<Appointment> temporalAppointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfo, AppointmentBaseLayoutCalculator layoutCalculator) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(temporalAppointments);
			WebAppointmentsLayoutResult layoutResult = new WebAppointmentsLayoutResult();
			layoutCalculator.CalculateLayout(layoutResult, appointments, cellsInfo);
			int count = layoutResult.AppointmentControls.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl appointmentControl = layoutResult.AppointmentControls[i];
				appointmentControl.IsPermanentAppointment = false;
				parent.Controls.Add(appointmentControl);
			}
		}
		public virtual void AssignAppointmentsIds(Control parent) {
			ControlCollection appointmentControls = parent.Controls;
			int count = appointmentControls.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl aptControl = (AppointmentControl)appointmentControls[i];
				aptControl.AssignId(i);
			}
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			GenerateCreateAppointmentsScript(sb, localVarName);
			GenerateAppointmentsScript(sb, localVarName, GetAppointmentns());
			sb.AppendFormat("{0}.InitializeAppointmentDivCache();\n", localVarName);
		}
		protected internal virtual void GenerateCreateAppointmentsScript(StringBuilder sb, string localVarName) {
			ControlCollection appointmentControls = ContentControl.Controls;
			int count = appointmentControls.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl appointmentControl = appointmentControls[i] as AppointmentControl;
				if (appointmentControl == null)
					continue;
				GenerateCreateAppointmentScript(sb, localVarName, appointmentControl);
			}
		}
		protected internal virtual void GenerateCreateAppointmentScript(StringBuilder sb, string localVarName, AppointmentControl appointmentControl) {
			string appointmentID = Owner.GetAppointmentClientId(appointmentControl.Appointment);
			appointmentControl.GenerateCreateAppointmentScript(sb, localVarName, appointmentID);
			sb.AppendFormat("{0}.AddContextMenuEvent(\"{1}\",\"{2}\");\n", localVarName, appointmentControl.MainDiv.ClientID, ASPxSchedulerScripts.GetAppointmentSelectionContextMenuFunction(Owner.ClientID));
		}
		protected internal virtual void GenerateAppointmentsScript(StringBuilder sb, string localVarName, AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++)
				GenerateAppointmentScript(sb, localVarName, appointments[i]);
		}
		protected internal virtual void GenerateAppointmentScript(StringBuilder sb, string localVarName, Appointment appointment) {
			ASPxScheduler control = Owner;
			string aptScriptArgs = AppointmentScriptRenderHelper.RenderScript(control, appointment);
			Dictionary<String, Object> properties = RaiseInitializeClientAppointment(appointment);
			if (properties.Count != 0) {
				string jsonPropertiesString = HtmlConvertor.ToJSON(properties);
				aptScriptArgs += String.Format(",{0}", jsonPropertiesString);
			}
			sb.AppendFormat(CultureInfo.InvariantCulture, "{0}.AddAppointment({1});\n", localVarName, aptScriptArgs);
		}
		Dictionary<string, object> RaiseInitializeClientAppointment(Appointment appointment) {
			Dictionary<string, object> properties = new Dictionary<string, object>();
			Owner.RaiseInitClientAppointment(appointment, properties);
			return properties;
		}
		protected internal virtual string GetAppointmentStartTime(Appointment appointment) {
			DateTime startTime = Owner.InnerControl.TimeZoneHelper.ToClientTime(((IInternalAppointment)appointment).GetInterval(), appointment.TimeZoneId, true).Start;
			return HtmlConvertor.ToScript(startTime, typeof(DateTime));
		}
		protected override bool IsCollapsedToZeroSize() {
			return true;
		}
		protected override bool IsHiddenInitially() {
			return true;
		}
	}
	public static class AppointmentScriptRenderHelper {
		static int GetLabelIndex(ASPxScheduler scheduler, object labelId) {
			AppointmentLabelCollection labels = scheduler.Storage.Appointments.Labels;
			AppointmentLabel label = labels.GetById(labelId);
			return labels.IndexOf(label);
		}
		static int GetStatusIndex(ASPxScheduler scheduler, object statusId) {
			AppointmentStatusCollection statuses = scheduler.Storage.Appointments.Statuses;
			AppointmentStatus status = statuses.GetById(statusId);
			return statuses.IndexOf(status);
		}
		public static string RenderScript(ASPxScheduler scheduler, Appointment appointment) {
			AppointmentOperationFlagsHelper calc = new AppointmentOperationFlagsHelper();
			AppointmentOperationFlags flags = calc.CalculateAppointmentFlags(scheduler.InnerControl, appointment); 
			string flagsStr = AppointmentOperationFlagsHelper.GenerateScriptAppointmentFlagsParameters(flags);
			string startTime = GetAppointmentStartTime(scheduler, appointment);
			string[] resources = SchedulerIdHelper.GenerateResourceIds(appointment.ResourceIds);
			string labelIndex = SchedulerIdHelper.GenerateObjectId(GetLabelIndex(scheduler, appointment.LabelKey));
			string statusIndex = SchedulerIdHelper.GenerateObjectId(GetStatusIndex(scheduler, appointment.StatusKey));
			string result = String.Format(CultureInfo.InvariantCulture, "\"{0}\", {1}, {2}, {3}, \"{4}\", \"{5}\", {6}, {7}",
							scheduler.GetAppointmentClientId(appointment), startTime,
							appointment.Duration.TotalMilliseconds, HtmlConvertor.ToJSON(resources),
							flagsStr, appointment.Type, labelIndex, statusIndex);
			if (scheduler.Storage.Appointments.IsNewAppointment(appointment))
				result += ", 1";
			else
				result += ", 0";
			return result;
		}
		static string GetAppointmentStartTime(ASPxScheduler scheduler, Appointment appointment) {
			DateTime startTime = scheduler.InnerControl.TimeZoneHelper.ToClientTime(((IInternalAppointment)appointment).GetInterval(), appointment.TimeZoneId, true).Start;
			return HtmlConvertor.ToScript(startTime, typeof(DateTime));
		}
	}
	public class DayViewAppointmentsBlockBuilder : AppointmentsBlock {
		public DayViewAppointmentsBlockBuilder(ASPxScheduler control)
			: base(control) {
		}
		public override void RenderAppointments(Control parent) {
			DayView view = (DayView)View;
			if (!view.ActualShowAllAppointmentsAtTimeCells)
				RenderAllDayAppointments(parent);
			RenderTimeCellAppointments(parent);
		}
		protected internal virtual void RenderAllDayAppointments(Control parent) {
			AppointmentBaseCollection appointments = GetLongAppointments();
			NotificationCollection<Appointment> nonpermanentAppointment = GetLongNonpermanentAppointments();
			RenderAppointmentsCore(parent, appointments, nonpermanentAppointment, true);
		}
		protected internal virtual void RenderTimeCellAppointments(Control parent) {
			AppointmentBaseCollection appointments = GetTimeCellAppointments();
			NotificationCollection<Appointment> nonpermanentAppointment = GetTimeCellNonpermanentAppointments();
			RenderAppointmentsCore(parent, appointments, nonpermanentAppointment, false);
		}
		protected internal virtual AppointmentBaseCollection GetShortAppointments() {
			return GetAppointments(new DayViewTimeCellAppointmentsFilter());
		}
		protected internal virtual AppointmentBaseCollection GetLongAppointments() {
			return GetAppointments(new DayViewAllDayAppointmentsFilter());
		}
		protected internal virtual NotificationCollection<Appointment> GetLongNonpermanentAppointments() {
			return GetNonpermanentAppointments(new DayViewAllDayAppointmentsFilter());
		}
		protected internal virtual NotificationCollection<Appointment> GetShortNonpermanentAppointments() {
			return GetNonpermanentAppointments(new DayViewTimeCellAppointmentsFilter());
		}
		protected internal virtual AppointmentBaseCollection GetTimeCellAppointments() {
			DayView view = (DayView)View;
			if (view.ActualShowAllAppointmentsAtTimeCells)
				return GetPermanentAppointments();
			else
				return GetShortAppointments();
		}
		protected internal virtual NotificationCollection<Appointment> GetTimeCellNonpermanentAppointments() {
			DayView view = (DayView)View;
			if (view.ActualShowAllAppointmentsAtTimeCells)
				return GetNonpermanentAppointments();
			else
				return GetShortNonpermanentAppointments();
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(ProcessorBase<Appointment> processor) {
			AppointmentBaseCollection appointments = GetPermanentAppointments();
			processor.Process(appointments);
			return (AppointmentBaseCollection)processor.DestinationCollection;
		}
		protected internal virtual NotificationCollection<Appointment> GetNonpermanentAppointments(ProcessorBase<Appointment> processor) {
			NotificationCollection<Appointment> appointments = GetNonpermanentAppointments();
			processor.Process(appointments);
			return processor.DestinationCollection;
		}
	}
	public class NonpermanentAppointmentIdHelper {
		int idCount = 0;
		public NonpermanentAppointmentIdHelper() {
		}
		public void Reset() {
			idCount = 0;
		}
		public string GetNextId() {
			string result = idCount.ToString();
			idCount++;
			return String.Format("npa{0}", result);
		}
	}
}
