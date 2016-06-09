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
using System.Collections.Generic;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Commands {
	#region EditAppointmentCommand
	public abstract class EditAppointmentCommand : EditExistingAppointmentCommandBase {
		protected EditAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override void Execute(Appointment apt) {
			if (CanEditAppointment(apt))
				ShowEditFormCore(apt, false);
			else if (CanViewAppointmentAsReadOnly(apt))
				ShowEditFormCore(apt, true);
		}
		protected internal override bool CanEditAppointment(Appointment apt) {
			return base.CanEditAppointment(apt) && InnerControl.OptionsCustomization.AllowDisplayAppointmentForm != AllowDisplayAppointmentForm.Never;
		}
		protected internal abstract void ShowEditFormCore(Appointment apt, bool readOnly);
	}
	#endregion
	#region EditSingleAppointmentCommand
	public abstract class EditSingleAppointmentCommand : EditAppointmentCommand {
		protected EditSingleAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.OpenAppointment; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_OpenAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_OpenAppointment; } }
		#endregion
		protected internal override void ShowEditFormCore(Appointment apt, bool readOnly) {
			if (apt.IsBase) { 
				ShowEditAppointmentForm(apt, readOnly);
				return;
			}
			Appointment pattern = apt.RecurrencePattern;
			if (pattern != null)
				EditRecurrentAppointment(apt, readOnly);
		}
		protected internal virtual void ShowEditAppointmentForm(Appointment apt, bool readOnly) {
			SupportAppointmentEdit.ShowAppointmentForm(apt, false, readOnly, CommandSourceType);
		}
		protected internal virtual void ShowEditRecurrencePatternForm(Appointment apt, bool readOnly) {
			SupportAppointmentEdit.ShowAppointmentForm(apt.RecurrencePattern, false, readOnly, CommandSourceType);
		}
		protected internal abstract void EditRecurrentAppointment(Appointment apt, bool readOnly);
	}
	#endregion
	#region EditAppointmentQueryCommand
	public class EditAppointmentQueryCommand : EditSingleAppointmentCommand {
		public EditAppointmentQueryCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditAppointmentQuery; } }
		protected internal override void EditRecurrentAppointment(Appointment apt, bool readOnly) {
			switch (QueryEditAppointment(apt, readOnly)) {
				case RecurrentAppointmentAction.Series:
					ShowEditRecurrencePatternForm(apt, readOnly);
					break;
				case RecurrentAppointmentAction.Occurrence:
					ShowEditAppointmentForm(apt, readOnly);
					break;
				case RecurrentAppointmentAction.Cancel:
				case RecurrentAppointmentAction.Ask:
				default:
					break; 
			}
		}
		protected internal virtual RecurrentAppointmentAction QueryEditAppointment(Appointment apt, bool readOnly) {
			RecurrentAppointmentAction editAction = InnerControl.OptionsBehavior.RecurrentAppointmentEditAction;
			if (editAction == RecurrentAppointmentAction.Ask)
				return QueryEditAppointmentCore(apt, readOnly);
			else
				return editAction;
		}
		protected internal virtual RecurrentAppointmentAction QueryEditAppointmentCore(Appointment apt, bool readOnly) {
			return InnerControl.Owner.ShowEditRecurrentAppointmentForm(apt, readOnly, CommandSourceType);
		}
	}
	#endregion
	#region EditRecurrencePatternCommandBase
	public abstract class EditRecurrencePatternCommandBase : EditAppointmentCommand {
		protected EditRecurrencePatternCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected override bool CheckInitalConditions() {
			return InnerControl.SupportsRecurrence;
		}
		protected internal override bool CanEditAppointment(Appointment apt) {
			bool canEdit = base.CanEditAppointment(apt);
			if (!canEdit)
				return false;
			return EnsureAppointmentType(apt.Type);
		}
		protected virtual bool EnsureAppointmentType(AppointmentType appointmentType) {
			return appointmentType != AppointmentType.Normal;
		}
	}
	#endregion
	#region EditRecurrencePatternCommand
	public class EditRecurrencePatternCommand : EditRecurrencePatternCommandBase {
		public EditRecurrencePatternCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.EditSeries; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_EditSeries; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected internal override void ShowEditFormCore(Appointment apt, bool readOnly) {
			SupportAppointmentEdit.ShowAppointmentForm(apt.RecurrencePattern, false, readOnly, CommandSourceType);
		}
		protected internal override bool CanViewAppointmentAsReadOnly(Appointment apt) {
			bool canViewAsReadonly = base.CanViewAppointmentAsReadOnly(apt);
			if (!canViewAsReadonly)
				return false;
			return apt.Type != AppointmentType.Normal;
		}
	}
	#endregion
}
