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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraScheduler.Commands {
	#region EditAppointmentViaInplaceEditorCommand
	public class EditAppointmentViaInplaceEditorCommand : EditSingleAppointmentCommand {
		public EditAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditAppointmentViaInplaceEditor; } }
		protected internal override bool CanEditAppointment(Appointment apt) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointmentViaInplaceEditor(apt);
		}
		protected internal override bool CanViewAppointmentAsReadOnly(Appointment apt) {
			return false;
		}
		public override void Execute(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			if (CanEditAppointment(apt)) {
				ISchedulerInplaceEditController controller = InnerControl.InplaceEditController;
				controller.SetEditedAppointment(apt);
				controller.Activate();
			}
		}
		protected internal override void EditRecurrentAppointment(Appointment apt, bool readOnly) {
		}
	}
	#endregion
}
