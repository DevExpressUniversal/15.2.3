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
using DevExpress.XtraScheduler.Internal;
#if !SL
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler.Commands {
	public class ChangeAppointmentReminderUICommand : EditSingleAppointmentPropertyUICommand<TimeSpan> {
		public ChangeAppointmentReminderUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public ChangeAppointmentReminderUICommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.ChangeAppointmentReminderUI; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_ChangeAppointmentReminderUI; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_ChangeAppointmentReminderUI; } }
		protected override void SetAppointmentProperty(Appointment apt, TimeSpan property) {
			apt.HasReminder = property != TimeSpan.MinValue;
			if (!apt.HasReminder)
				return;
			(apt.Reminder).SetTimeBeforeStart(property);
		}
		protected override TimeSpan GetAppointmentProperty(Appointment apt) {
			if (!apt.HasReminder)
				return TimeSpan.MinValue;;
			return apt.Reminder.TimeBeforeStart;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (Control == null || Control.Storage == null)
				return;
			if (!Control.Storage.EnableReminders) {
				state.Visible = false;
				state.Enabled = false;
				return;
			}
			base.UpdateUIStateCore(state);
		}
		protected override object GetEmptyValue() {
			return TimeSpan.MinValue;
		}
	}
}
