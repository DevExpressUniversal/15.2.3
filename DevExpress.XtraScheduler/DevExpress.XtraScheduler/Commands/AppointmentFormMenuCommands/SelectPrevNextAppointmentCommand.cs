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

using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Commands {
	public abstract class SelectPrevNextAppointmentCommand : SchedulerMenuItemWinSimpleCommand {
		protected SelectPrevNextAppointmentCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_Free; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override void ForceExecute(ICommandUIState state) {
			SchedulerControl.SuspendSelectionPaint();
			try {
				base.ForceExecute(state);
			} finally {
				SchedulerControl.ResumeSelectionPaint();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool enabled = SchedulerControl.ActiveView.ViewInfo.CellContainers.TotalAppointmentViewInfoCount > 0;
			state.Enabled = enabled;
			state.Visible = enabled;
		}
		protected internal override void ExecuteCore() {
			AppointmentViewInfoCollection visibleAppointmentViewInfos = PrepareVisibleAppointmentViewInfos();
			AppointmentViewInfo newSelectedAppointmentViewInfo = CalculateNewSelectedAppointmentViewInfo(visibleAppointmentViewInfos);
			if (newSelectedAppointmentViewInfo != null)
				SelectAppointmentViewInfo(newSelectedAppointmentViewInfo);
			else {
				if (visibleAppointmentViewInfos.Count > 0) {
					int indexOfSelectedAppointment = CalcCurrentSelectedAppointmentViewInfoIndex(visibleAppointmentViewInfos);
					indexOfSelectedAppointment = (visibleAppointmentViewInfos.Count - 1) - indexOfSelectedAppointment;
					SchedulerViewBase view = SchedulerControl.ActiveView;
					AppointmentViewInfo viewInfo = visibleAppointmentViewInfos[indexOfSelectedAppointment];
					TimeInterval appointmentInterval;
					if ((view is DayView) && viewInfo.Interval.Duration >= DateTimeHelper.DaySpan)
						appointmentInterval = view.RoundSelectionInterval(new TimeInterval(viewInfo.Interval.Start, TimeSpan.FromTicks(1)));
					else
						appointmentInterval = viewInfo.Interval;
					view.SetSelectionCore(appointmentInterval, viewInfo.Resource);
					view.GotoTimeInterval(appointmentInterval);
				}
				SchedulerControl.SelectedAppointments.Clear();
			}
		}
		protected internal virtual AppointmentViewInfo CalculateNewSelectedAppointmentViewInfo(AppointmentViewInfoCollection visibleAppointmentViewInfos) {
			if (visibleAppointmentViewInfos.Count <= 0)
				return null;
			AppointmentViewInfoCollection selectedAppointmentViewInfos = PrepareSelectedAppointmentViewInfos();
			if (selectedAppointmentViewInfos.Count > 0) {
				int indexOfSelectedAppointment = CalcCurrentSelectedAppointmentViewInfoIndex(selectedAppointmentViewInfos);
				int selectedAppointmentViewInfoIndex = visibleAppointmentViewInfos.IndexOf(selectedAppointmentViewInfos[indexOfSelectedAppointment]);
				if (selectedAppointmentViewInfoIndex < 0)
					return CalculateNewDefaultSelectedAppointmentViewInfo(visibleAppointmentViewInfos);
				else {
					int newSelectedAppointmentIndex = CalcNewSelectedAppointmentViewInfoIndex(visibleAppointmentViewInfos, selectedAppointmentViewInfoIndex);
					if (newSelectedAppointmentIndex < 0)
						return null;
					else
						return visibleAppointmentViewInfos[newSelectedAppointmentIndex];
				}
			} else
				return FindNearestAppointmentViewInfo(visibleAppointmentViewInfos, SchedulerControl.SelectedInterval);
		}
		protected internal virtual AppointmentViewInfoCollection PrepareVisibleAppointmentViewInfos() {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			result.AddRange(SchedulerControl.ActiveView.ViewInfo.CopyAllAppointmentViewInfos());
			result.Sort(new AppointmentViewInfoTabOrderComparer());
			return result;
		}
		protected internal virtual void SelectAppointmentViewInfo(AppointmentViewInfo viewInfo) {
			SchedulerViewBase view = SchedulerControl.ActiveView;
			view.SetSelectionCore(viewInfo.Interval, viewInfo.Resource);
			Appointment appointment = viewInfo.Appointment;
			view.ChangeAppointmentSelection(appointment);
		}
		protected internal virtual AppointmentViewInfoCollection PrepareSelectedAppointmentViewInfos() {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			result.AddRange(SchedulerControl.ActiveView.ViewInfo.CopyAllAppointmentViewInfos().Where(vi => vi.Selected));
			result.Sort(new AppointmentViewInfoTabOrderComparer());
			return result;
		}
		protected internal abstract int CalcCurrentSelectedAppointmentViewInfoIndex(AppointmentViewInfoCollection selectedAppointmentViewInfos);
		protected internal abstract AppointmentViewInfo CalculateNewDefaultSelectedAppointmentViewInfo(AppointmentViewInfoCollection visibleAppointmentViewInfos);
		protected internal abstract int CalcNewSelectedAppointmentViewInfoIndex(AppointmentViewInfoCollection visibleAppointmentViewInfos, int currentSelectedAppointmentViewInfoIndex);
		protected internal abstract AppointmentViewInfo FindNearestAppointmentViewInfo(AppointmentViewInfoCollection appointmentViewInfos, TimeInterval interval);
	}
}
