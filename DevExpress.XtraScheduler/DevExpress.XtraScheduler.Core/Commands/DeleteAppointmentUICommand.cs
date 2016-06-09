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
	#region DeleteAppointmentUICommand
	public class DeleteAppointmentUICommand : DeleteAppointmentsQueryCommand {
		public DeleteAppointmentUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public DeleteAppointmentUICommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		public override bool CanExecute() {
			bool result = base.CanExecute();
			if (!result)
				return result;
			if (Appointments.Count == 1 && Appointments[0].IsRecurring) {
				return false;
			}
			return result;
		}
	}
	#endregion
	#region DeleteAppointmentSeriesGroupCommand
	public class DeleteAppointmentSeriesGroupCommand : DeleteRecurrentAppointmentCommand {
		public DeleteAppointmentSeriesGroupCommand(ISchedulerCommandTarget commandTarget)
			: base(commandTarget) {
		}
		public override string ImageName { get { return SchedulerCommandImagesNames.Delete; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_DeleteAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.MenuCmd_DeleteAppointment; } }
		public override void Execute() {
		}
		protected internal override void ExecuteCore() {
		}
		public override void ForceExecute(ICommandUIState state) {
		}
		protected internal override void DeleteRecurringAppointmentsSingleQuery(AppointmentBaseCollection apts) {
			base.DeleteRecurringAppointmentsSingleQuery(apts);
		}
		protected internal override bool CanDeleteAppointments(AppointmentBaseCollection appointments) {
			return base.CanDeleteAppointments(appointments);
		}
		protected internal override bool CanDeleteAppointment(Appointment apt) {
			bool result = base.CanDeleteAppointment(apt);
			if (!result)
				return result;
			return apt.Type != AppointmentType.Normal;
		}
		protected override bool IsVisible(bool canDelete, bool inplaceOpened) {
			return canDelete;
		}
	}
	#endregion
	#region DeleteRecurrentAppointmentCommand
	public abstract class DeleteRecurrentAppointmentCommand : DeleteAppointmentsQueryCommand {
		protected DeleteRecurrentAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected DeleteRecurrentAppointmentCommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		public override string ImageName { get { return String.Empty; } }
		protected internal override bool QueryDeleteEachRecurringAppointment { get { return false; } }
		protected internal override void DeleteNonRecurringAppointments(AppointmentBaseCollection nonRecurringAppointments) {
		}
		internal override void DeleteRecurringAppointment(AppointmentBaseCollection apts, Appointment apt) {
			if (apts.Count != 1)
				return;
			base.DeleteRecurringAppointment(apts, apt);
		}
		protected internal override bool CanDeleteAppointments(AppointmentBaseCollection appointments) {
			bool canDelete = base.CanDeleteAppointments(appointments);
			if (!canDelete)
				return false;
			return appointments.Count == 1 && appointments[0].IsRecurring;
		}
	}
	#endregion
	#region DeleteOccurrenceUICommand
	public class DeleteOccurrenceUICommand : DeleteRecurrentAppointmentCommand {
		public DeleteOccurrenceUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public DeleteOccurrenceUICommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_DeleteOccurrence; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_DeleteOccurrence; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.DeleteOccurrenceUI; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DeleteOccurrenceUI; } }
		protected internal override RecurrentAppointmentAction RecurrentAppointmentDeleteAction { get { return RecurrentAppointmentAction.Occurrence; } }		
	}
	#endregion
	#region DeleteSeriesUICommand
	public class DeleteSeriesUICommand : DeleteRecurrentAppointmentCommand {
		public DeleteSeriesUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public DeleteSeriesUICommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.DeleteSeriesUI; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DeleteSeriesUI; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_DeleteSeries; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_DeleteSeries; } }
		protected internal override RecurrentAppointmentAction RecurrentAppointmentDeleteAction { get { return RecurrentAppointmentAction.Series; } }		
	}
	#endregion
}
