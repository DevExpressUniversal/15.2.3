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
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraScheduler.Commands.Internal {
	#region EditNormalAppointmentUICommand
	public class EditNormalAppointmentUICommand : EditAppointmentQueryCommand {
		public EditNormalAppointmentUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override string ImageName { get { return SchedulerCommandImagesNames.Open; } }
		protected override bool CanExecute(Appointment apt) {
			if (!base.CanExecute(apt))
				return false;
			return apt.Type == AppointmentType.Normal;
		}
	}
	#endregion
	#region EditAppointmentSeriesGroupCommand
	public class EditAppointmentSeriesGroupCommand : EditOccurrenceUICommand {
		public EditAppointmentSeriesGroupCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override string ImageName { get { return SchedulerCommandImagesNames.Open; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditAppointmentSeriesGroup; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_OpenAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_OpenAppointment; } }
		protected override bool CanExecute(Appointment apt) {
			if (!base.CanExecute(apt))
				return false;
			return apt.Type != AppointmentType.Normal;
		}
		public override void Execute() {
		}
		public override void Execute(Appointment apt) {
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			Appointment editedAppointment = ObtainEditedAppointment();
			state.Visible = editedAppointment != null && CanExecute(editedAppointment);
		}
	}
	#endregion
	#region EditOccurrenceUICommand
	public class EditOccurrenceUICommand : EditAppointmentQueryCommand {
		public EditOccurrenceUICommand(ISchedulerCommandTarget target)
			: base(target) {
			HideDisabled = true;
		}
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_OpenOccurrence; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_OpenOccurrence; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.EditOccurrenceUI; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditOccurrenceUI; } }
		public override string ImageName { get { return String.Empty; } }
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
		}
		protected internal override bool CanEditAppointment(Appointment apt) {
			bool canEditAppointment = base.CanEditAppointment(apt);
			return canEditAppointment && apt.Type != AppointmentType.Normal;
		}
		protected internal override bool CanViewAppointmentAsReadOnly(Appointment apt) {
			return base.CanViewAppointmentAsReadOnly(apt) && apt.Type != AppointmentType.Normal;
		}
	}
	#endregion
	#region EditSeriesUICommand
	public class EditSeriesUICommand : EditRecurrencePatternCommand {
		public EditSeriesUICommand(ISchedulerCommandTarget target)
			: base(target) {
			HideDisabled = true;
		}
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_OpenSeries; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_OpenSeries; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.EditSeriesUI; } }
		public override string ImageName { get { return String.Empty; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditSeriesUI; } }
	}
	#endregion
}
