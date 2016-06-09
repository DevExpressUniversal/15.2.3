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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Operations;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Commands {
	public class SplitAppointmentOperationCommand : OperationCommand {
		public SplitAppointmentOperationCommand(SchedulerControl control)
			: base(control) {
		}
		public SplitAppointmentOperationCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_SplitAppointment; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Caption_SplitAppointment; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SplitAppointment; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.SplitAppointment; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool isSelectedOnlyOneAppointment = SchedulerControl.SelectedAppointments.Count == 1;
			bool canSplit = isSelectedOnlyOneAppointment && CanSplitAppointment(SchedulerControl.SelectedAppointments[0]);
			state.Checked = false;
			state.Enabled = canSplit;
			state.Visible = true;
		}
		protected internal override IOperation CreateOperation() {
			TimeScaleCollection timeScales = CreateTimeScaleCollection();
			XtraSchedulerDebug.Assert(SchedulerControl.SelectedAppointments.Count == 1);
			return new SplitAppointmentOperation(SchedulerControl, timeScales, SchedulerControl.SelectedAppointments[0]);
		}
		bool CanSplitAppointment(Appointment appointment) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointment(appointment);
		}
		TimeScaleCollection CreateTimeScaleCollection() {
			TimeScaleCollection timeScaleCollection = new TimeScaleCollection();
			TimeScaleFixedInterval timeScaleOneMinute = new TimeScaleFixedInterval(new TimeSpan(00, 1, 00));
			timeScaleCollection.Add(timeScaleOneMinute);
			TimeScaleFixedInterval timeScaleFiveMinutes = new TimeScaleFixedInterval(new TimeSpan(00, 5, 00));
			timeScaleCollection.Add(timeScaleFiveMinutes);
			TimeScaleFixedInterval timeScaleSixMinutes = new TimeScaleFixedInterval(new TimeSpan(00, 6, 00));
			timeScaleCollection.Add(timeScaleSixMinutes);
			TimeScaleFixedInterval timeScaleTenMinutes = new TimeScaleFixedInterval(new TimeSpan(00, 10, 00));
			timeScaleCollection.Add(timeScaleTenMinutes);
			TimeScaleFixedInterval timeScaleFifteenMinutes = new TimeScaleFixedInterval(new TimeSpan(00, 15, 00));
			timeScaleCollection.Add(timeScaleFifteenMinutes);
			TimeScaleFixedInterval timeScaleThirtyMinutes = new TimeScaleFixedInterval(new TimeSpan(00, 30, 00));
			timeScaleCollection.Add(timeScaleThirtyMinutes);
			TimeScaleHour timeScaleHour = new TimeScaleHour();
			timeScaleCollection.Add(timeScaleHour);
			TimeScaleDay timeScaleDay = new TimeScaleDay();
			timeScaleCollection.Add(timeScaleDay);
			TimeScaleWeek timeScaleWeek = new TimeScaleWeek();
			timeScaleCollection.Add(timeScaleWeek);
			TimeScaleMonth timeScaleMonth = new TimeScaleMonth();
			timeScaleCollection.Add(timeScaleMonth);
			TimeScaleQuarter timeScaleQuarter = new TimeScaleQuarter();
			timeScaleCollection.Add(timeScaleQuarter);
			TimeScaleYear timeScaleYear = new TimeScaleYear();
			timeScaleCollection.Add(timeScaleYear);
			return timeScaleCollection;
		}
	}
}
