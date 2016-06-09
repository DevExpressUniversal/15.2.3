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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.Web.ASPxScheduler.Commands {
	#region WebGotoDateCommand
	public class WebGotoDateCommand : SchedulerMenuItemSimpleCommand {
		ASPxScheduler control;
		public WebGotoDateCommand(ASPxScheduler control)
			: base(control.InnerControl) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.GotoDate; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_GotoDate; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string ImageName { get { return MenuImages.GotoDateName; } }
		protected internal ASPxScheduler SchedulerControl { get { return control; } }
		protected internal override void ExecuteCore() {
			SchedulerControl.ShowForm(SchedulerFormType.GotoDate);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
	#region WebNewAppointmentCommand
	public class WebNewAppointmentCommand : NewAppointmentCommand {
		public WebNewAppointmentCommand(ASPxScheduler control)
			: base(control.InnerControl) {
		}
		public override string ImageName { get { return MenuImages.NewAppointmentName; } }
	}
	#endregion
	#region WebNewRecurringAppointmentCommand
	public class WebNewRecurringAppointmentCommand : NewRecurringAppointmentCommand {
		public WebNewRecurringAppointmentCommand(ASPxScheduler control)
			: base(control.InnerControl) {
		}
		public override string ImageName { get { return MenuImages.NewRecurringAppointmentName; } }
	}
	#endregion
	#region WebDeleteAppointmentsCommand
	public class WebDeleteAppointmentsCommand : DeleteAppointmentsQueryCommand {
		#region Fields
		ASPxScheduler control;
		#endregion
		public WebDeleteAppointmentsCommand(ASPxScheduler control)
			: base(control.InnerControl, control.SelectedAppointments) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		protected internal ASPxScheduler SchedulerControl { get { return control; } }
		public override string ImageName { get { return MenuImages.DeleteAppointmentName; } }
		#endregion
		protected internal override bool CanDeleteAppointment(Appointment apt) {
			bool canDelete = base.CanDeleteAppointment(apt);
			return canDelete && (SchedulerControl.ActiveFormType == SchedulerFormType.None);
		}
		protected internal override RecurrentAppointmentAction QueryDeleteAppointmentCore(Appointment apt) {
			if (SchedulerControl.OptionsForms.RecurrentAppointmentDeleteFormVisibility == SchedulerFormVisibility.None)
				return RecurrentAppointmentAction.Occurrence;
			SchedulerControl.SetEditableAppointment(apt);
			SchedulerControl.ShowForm(SchedulerFormType.RecurrentAppointmentDelete);
			return RecurrentAppointmentAction.Cancel;
		}
	}
	#endregion
	#region WebEditAppointmentCommand
	public class WebEditAppointmentCommand : EditAppointmentQueryCommand {
		#region Fields
		readonly ASPxScheduler control;
		#endregion
		public WebEditAppointmentCommand(ASPxScheduler control)
			: base(control.InnerControl) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected internal ASPxScheduler SchedulerControl { get { return control; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected internal override RecurrentAppointmentAction QueryEditAppointmentCore(Appointment apt, bool readOnly) {
			SchedulerControl.ShowForm(SchedulerFormType.RecurrentAppointmentEdit);
			SchedulerControl.SetEditableAppointment(apt);
			return RecurrentAppointmentAction.Cancel;
		}
		protected internal bool ProtectedCanExecute(Appointment appointment) {
			return CanExecute(appointment);
		}
	}
	#endregion
	#region WebEditRecurrentAppointmentCommand
	public class WebEditRecurrentAppointmentCommand : WebEditAppointmentCommand {
		#region Fields
		RecurrentAppointmentAction action;
		#endregion
		public WebEditRecurrentAppointmentCommand(ASPxScheduler control, RecurrentAppointmentAction action)
			: base(control) {
			this.action = action;
		}
		#region Properties
		public RecurrentAppointmentAction Action { get { return action; } set { action = value; } }
		#endregion
		protected internal override RecurrentAppointmentAction QueryEditAppointmentCore(Appointment apt, bool readOnly) {
			return action;
		}
	}
	#endregion
	#region WebNewAppointmentViaInplaceEditorCommand
	public class WebNewAppointmentViaInplaceEditorCommand : NewAppointmentViaInplaceEditorCommand {
		readonly TimeInterval appointmentCreationInterval;
		readonly XtraScheduler.Resource resource;
		public WebNewAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget target, TimeInterval appointmentCreationInterval, XtraScheduler.Resource resource)
			: base(target) {
			Guard.ArgumentNotNull(appointmentCreationInterval, "appointmentCreationInterval");
			this.appointmentCreationInterval = appointmentCreationInterval;
			this.resource = resource;
		}
		public WebNewAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget target, TimeInterval appointmentCreationInterval)
			: this(target, appointmentCreationInterval, null) {
		}
		internal TimeInterval AppointmentCreationInterval { get { return appointmentCreationInterval; } }
		public Resource Resource { get { return resource; } }
		protected internal override TimeInterval GetActualSelectedInterval() {
			return AppointmentCreationInterval;
		}
		protected internal override object GetActualSelectedResourceId() {
			if (Resource == null)
				return base.GetActualSelectedResourceId();
			return Resource.Id;
		}
	}
	#endregion
	public abstract class ChangeAppointmentPropertyImageCommand : ChangeAppointmentPropertyImageCommandCore<Image> {
		protected ChangeAppointmentPropertyImageCommand(ISchedulerCommandTarget target, object newValue)
			: base(target, newValue) {
		}
	}
	public class ChangeAppointmentLabelCommand : ChangeAppointmentPropertyImageCommand {
		AppointmentLabel label;
		public ChangeAppointmentLabelCommand(ISchedulerCommandTarget target, AppointmentLabel label, object labelId)
			: base(target, labelId) {
			if (label == null)
				Exceptions.ThrowArgumentException("label", label);
			this.label = label;
			InitImages();
		}
		protected internal AppointmentLabel Label {
			get { return this.label; }
		}
		public override SchedulerMenuItemId MenuId {
			get { return SchedulerMenuItemId.LabelSubMenu; }
		}
		public override string MenuCaption {
			get { return label.MenuCaption; }
		}
		public override SchedulerStringId MenuCaptionStringId {
			get { return SchedulerStringId.MenuCmd_LabelAs; }
		}
		public override SchedulerStringId DescriptionStringId {
			get { return SchedulerStringId.DescCmd_LabelAs; }
		}
		protected internal override object GetPropertyValue(Appointment apt) {
			return apt.LabelKey;
		}
		protected internal override void SetPropertyValue(Appointment apt, object newValue) {
			IAppointmentLabel label = InnerControl.Storage.Appointments.Labels.GetByIndex((int)newValue);
			apt.LabelKey = label.Id;
		}
		protected override Image GetImage() {
			UserInterfaceObjectAsp uiObject = Label as UserInterfaceObjectAsp;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectAspHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override Image GetLargeImage() {
			UserInterfaceObjectAsp uiObject = Label as UserInterfaceObjectAsp;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectAspHelper.CreateBitmap(uiObject, 32, 32);
		}
	}
	public class ChangeAppointmentStatusCommand : ChangeAppointmentPropertyImageCommand {
		AppointmentStatus status;
		public ChangeAppointmentStatusCommand(ISchedulerCommandTarget target, AppointmentStatus status, object statusId)
				: base(target, statusId) {
			if (status == null)
				Exceptions.ThrowArgumentException("status", status);
			this.status = status;
			InitImages();
		}
		public override SchedulerMenuItemId MenuId {
			get { return SchedulerMenuItemId.StatusSubMenu; }
		}
		public override string MenuCaption {
			get { return status.MenuCaption; }
		}
		public override SchedulerStringId MenuCaptionStringId {
			get { return SchedulerStringId.MenuCmd_ShowTimeAs; }
		}
		public override SchedulerStringId DescriptionStringId {
			get { return SchedulerStringId.DescCmd_ShowTimeAs; }
		}
		protected internal AppointmentStatus Status {
			get { return this.status; }
		}
		protected internal override object GetPropertyValue(Appointment apt) {
			return apt.StatusKey;
		}
		protected internal override void SetPropertyValue(Appointment apt, object newValue) {
			IAppointmentStatus status = InnerControl.Storage.Appointments.Statuses.GetByIndex((int)newValue);
			apt.StatusKey = status.Id;
		}
		protected override Image GetImage() {
			UserInterfaceObjectAsp uiObject = Status as UserInterfaceObjectAsp;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectAspHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override Image GetLargeImage() {
			UserInterfaceObjectAsp uiObject = Status as UserInterfaceObjectAsp;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectAspHelper.CreateBitmap(uiObject, 32, 32);
		}
	}
	public static class UserInterfaceObjectAspHelper {
		public static Bitmap CreateBitmap(UserInterfaceObjectAsp uiObject, int width, int height) {
			Rectangle r = new Rectangle(0, 0, width, height);
			Bitmap bmp = new Bitmap(r.Width, r.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (Brush brush = new SolidBrush(uiObject.Color))
					gr.FillRectangle(brush, r);
				gr.FillRectangle(Brushes.Black, RectUtils.GetTopSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetLeftSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetRightSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetBottomSideRect(r, 1));
			}
			return bmp;
		}
	}
}
