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

using System.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Commands {
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
			UserInterfaceObjectWin uiObject = Status as UserInterfaceObjectWin;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWinHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override Image GetLargeImage() {
			UserInterfaceObjectWin uiObject = Status as UserInterfaceObjectWin;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWinHelper.CreateBitmap(uiObject, 32, 32);
		}
	}
}
