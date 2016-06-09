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

using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.Xpf.Scheduler.Commands {
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
		protected override ImageSource GetImage() {
			UserInterfaceObjectWpf uiObject = Label as UserInterfaceObjectWpf;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWpfHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override ImageSource GetLargeImage() {
			UserInterfaceObjectWpf uiObject = Label as UserInterfaceObjectWpf;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWpfHelper.CreateBitmap(uiObject, 32, 32);
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
		protected override ImageSource GetImage() {
			UserInterfaceObjectWpf uiObject = Status as UserInterfaceObjectWpf;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWpfHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override ImageSource GetLargeImage() {
			UserInterfaceObjectWpf uiObject = Status as UserInterfaceObjectWpf;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWpfHelper.CreateBitmap(uiObject, 32, 32);
		}
	}
	public static class UserInterfaceObjectWpfHelper {
		public static ImageSource CreateBitmap(UserInterfaceObjectWpf uiObject, int width, int height) {
			GeometryDrawing geometryDrawing = new GeometryDrawing(uiObject.Brush, new Pen(Brushes.Black, 1.0), new RectangleGeometry(new Rect(0.0, 0.0, width, height)));
			return new DrawingImage(geometryDrawing);
		}
	}
}
