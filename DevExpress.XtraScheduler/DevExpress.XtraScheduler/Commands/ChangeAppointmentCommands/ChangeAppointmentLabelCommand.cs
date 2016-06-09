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
			UserInterfaceObjectWin uiObject = Label as UserInterfaceObjectWin;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWinHelper.CreateBitmap(uiObject, 16, 16);
		}
		protected override Image GetLargeImage() {
			UserInterfaceObjectWin uiObject = Label as UserInterfaceObjectWin;
			if (uiObject == null)
				return null;
			return UserInterfaceObjectWinHelper.CreateBitmap(uiObject, 32, 32);
		}
	}
}
