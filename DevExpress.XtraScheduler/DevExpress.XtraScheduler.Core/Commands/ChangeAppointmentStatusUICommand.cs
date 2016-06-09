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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
#if !SL
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler.Commands {
	public abstract class EditSingleAppointmentPropertyUICommand<T> : EditExistingAppointmentCommandBase {
		ICommandUIState currentState;
		protected EditSingleAppointmentPropertyUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected EditSingleAppointmentPropertyUICommand(InnerSchedulerControl control)
			: base(control) {
		}
		protected override void NotifyBeginCommandExecution(ICommandUIState state) {
			this.currentState = state;
			base.NotifyBeginCommandExecution(state);
		}
		protected override void NotifyEndCommandExecution(ICommandUIState state) {
			base.NotifyEndCommandExecution(state);
			this.currentState = null;
		}
		protected ICommandUIState GetCurrrentState() {
			return this.currentState;
		}
		public override void Execute(Appointment apt) {
			DefaultValueBasedCommandUIState<T> valueBasedState = GetCurrrentState() as DefaultValueBasedCommandUIState<T>;
			if (valueBasedState == null)
				return;
			SetAppointmentProperty(apt, valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			Appointment editAppointment = ObtainEditedAppointment();
			if (editAppointment == null || InnerControl.Storage == null)
				state.EditValue = GetEmptyValue();
			else
				state.EditValue = GetAppointmentProperty(editAppointment);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<T>();
		}
		protected virtual object GetEmptyValue() {
			return default(T);
		}
		protected abstract void SetAppointmentProperty(Appointment apt, T value);
		protected abstract T GetAppointmentProperty(Appointment apt);
	}
	public class ChangeAppointmentStatusUICommand : EditSingleAppointmentPropertyUICommand<IAppointmentStatus> {
		public ChangeAppointmentStatusUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public ChangeAppointmentStatusUICommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.ChangeAppointmentStatusUI; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_ShowTimeAs; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_ShowTimeAs; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.ChangeStatus; } }
		protected override void SetAppointmentProperty(Appointment apt, IAppointmentStatus value) {
			apt.StatusKey = (int)value.Id;
		}
		protected override IAppointmentStatus GetAppointmentProperty(Appointment apt) {
			return InnerControl.Storage.Appointments.Statuses.GetById(apt.StatusKey);
		}
	}
}
