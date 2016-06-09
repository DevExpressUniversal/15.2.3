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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Bars;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Commands;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Scheduler;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Internal;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Xpf.Scheduler.Commands {
	#region SelectPrevNextAppointmentCommand
	public abstract class SelectPrevNextAppointmentCommand : SchedulerMenuItemWinSimpleCommand {
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_Free; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected SelectPrevNextAppointmentCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			AppointmentControlCollection viewInfos = SchedulerControl.ActiveView.ViewInfo.AppointmentControls;
			bool enabled = viewInfos.Count > 0;
			state.Enabled = enabled;
			state.Visible = enabled;
		}
		public override void ForceExecute(ICommandUIState state) {
			try {
				base.ForceExecute(state);
			} finally {
			}
		}
		protected internal override void ExecuteCore() {
#if !SL
			if (CanLeaveFocusOnTab()) {
				LeaveFocus();
				return;
			}
#endif
			AppointmentControlCollection visibleAppointmentControls = PrepareVisibleAppointmentControls();
			AppointmentControl newSelectedAppointmentControl = CalculateNewSelectedAppointmentControl(visibleAppointmentControls);
			if (newSelectedAppointmentControl != null)
				SelectAppointmentControl(newSelectedAppointmentControl);
			else {
				if (visibleAppointmentControls.Count > 0) {
					newSelectedAppointmentControl = CalculateNewDefaultSelectedAppointmentControl(visibleAppointmentControls);
					if (newSelectedAppointmentControl != null)
						SelectAppointmentControl(newSelectedAppointmentControl);
				} else
					SchedulerControl.SelectedAppointments.Clear();
			}
		}
#if !SL
		protected bool CanLeaveFocusOnTab() {
			return SchedulerControl.OptionsBehavior != null && SchedulerControl.OptionsBehavior.AllowLeaveFocusOnTab;
		}
		protected virtual void LeaveFocus() {
			MoveFocusNextPrevCommandBase leaveFocusCommand = CreateLeaveFocusCommand();
			if (leaveFocusCommand == null)
				return;
			leaveFocusCommand.Execute();
		}
#endif
		protected internal virtual AppointmentControl CalculateNewSelectedAppointmentControl(AppointmentControlCollection visibleAppointmentControls) {
			if (visibleAppointmentControls.Count <= 0)
				return null;
			AppointmentControlCollection selectedAppointmentControls = PrepareSelectedAppointmentControls();
			if (selectedAppointmentControls.Count > 0) {
				int indexOfSelectedAppointment = CalcCurrentSelectedAppointmentControlIndex(selectedAppointmentControls);
				int selectedAppointmentControlIndex = visibleAppointmentControls.IndexOf(selectedAppointmentControls[indexOfSelectedAppointment]);
				if (selectedAppointmentControlIndex < 0)
					return CalculateNewDefaultSelectedAppointmentControl(visibleAppointmentControls);
				else {
					int newSelectedAppointmentIndex = CalcNewSelectedAppointmentControlIndex(visibleAppointmentControls, selectedAppointmentControlIndex);
					if (newSelectedAppointmentIndex < 0)
						return null;
					else
						return visibleAppointmentControls[newSelectedAppointmentIndex];
				}
			} else
				return FindNearestAppointmentControl(visibleAppointmentControls, SchedulerControl.SelectedInterval);
		}
		protected internal virtual AppointmentControlCollection PrepareVisibleAppointmentControls() {
			AppointmentControlCollection result = new AppointmentControlCollection();
			AppointmentControlCollection appointmentControls = SchedulerControl.ActiveView.ViewInfo.AppointmentControls;
			result.AddRange(appointmentControls);
			result.Sort(new VisualAppointmentControlTabOrderComparer());
			return result;
		}
		protected internal virtual void SelectAppointmentControl(AppointmentControl appointmentControl) {
			SchedulerViewBase view = SchedulerControl.ActiveView;
			view.SetSelectionCore(appointmentControl.ViewInfo.Interval, appointmentControl.ViewInfo.Resource);
			Appointment appointment = appointmentControl.Appointment;
			view.ChangeAppointmentSelection(appointment);
			ValidateAppointmentVisibility(appointmentControl);
		}
		protected internal virtual AppointmentControlCollection PrepareSelectedAppointmentControls() {
			AppointmentControlCollection result = new AppointmentControlCollection();
			AppointmentControlCollection appointmentControls = SchedulerControl.ActiveView.ViewInfo.AppointmentControls;
			int count = appointmentControls.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl appointmentControl = appointmentControls[i];
				if (appointmentControl.ViewInfo.Selected)
					result.Add(appointmentControl);
			}
			result.Sort(new VisualAppointmentControlTabOrderComparer());
			return result;
		}
		protected internal abstract int CalcCurrentSelectedAppointmentControlIndex(AppointmentControlCollection selectedAppointmentControls);
		protected internal abstract AppointmentControl CalculateNewDefaultSelectedAppointmentControl(AppointmentControlCollection visibleAppointmentControls);
		protected internal abstract int CalcNewSelectedAppointmentControlIndex(AppointmentControlCollection visibleAppointmentControls, int currentSelectedAppointmentViewInfoIndex);
		protected internal abstract AppointmentControl FindNearestAppointmentControl(AppointmentControlCollection appointmentControls, TimeInterval interval);
#if !SL
		protected abstract MoveFocusNextPrevCommandBase CreateLeaveFocusCommand();
#endif
		protected internal void ValidateAppointmentVisibility(AppointmentControl appointmentControl) {
			RequestVisualAppointmentInfoEventArgs e = new RequestVisualAppointmentInfoEventArgs(AppointmentDragState.Begin);
			e.SourceAppointments = new AppointmentBaseCollection();
			e.SourceAppointments.Add(appointmentControl.Appointment);
			SchedulerControl.RaiseRequestVisualAppointmentInfo(e);
			XpfMouseUtils.TranslateBoundsToControl(SchedulerControl, e.AppointmentInfos);
			if (e.AppointmentInfos.Count == 0)
				return;
			VisualAppointmentInfo mostleftVisualAppointment = e.AppointmentInfos[0];
			VisualAppointmentControl visualAppointmentControl = mostleftVisualAppointment.Container;
			DynamicAppointmentsPanelBase panel = mostleftVisualAppointment.Panel as DynamicAppointmentsPanelBase;
			ScrollViewer scrollViewer = LayoutHelper.FindParentObject<ScrollViewer>(visualAppointmentControl) as ScrollViewer;
			if (panel == null || scrollViewer == null)
				return;
			DayViewAppointmentsScrollCalculator calculator = new DayViewAppointmentsScrollCalculator(scrollViewer);
			Rect appointmentRect = panel.GetAppointmentRect(visualAppointmentControl);
			double bottom = appointmentRect.Bottom;
			double top = appointmentRect.Top;
			double verticalOffset = calculator.CalculateVerticalScrollOffsetMakeAppointmentVisible(top, bottom);
			scrollViewer.ScrollToVerticalOffset(verticalOffset);
		}
		protected internal abstract int CalcNewBoundaryAppointment(int visibleAppointmentControlsCount, int indexOfSelectedAppointment);
	}
	#endregion
	#region SelectNextAppointmentCommand
	public class SelectNextAppointmentCommand : SelectPrevNextAppointmentCommand {
		public SelectNextAppointmentCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SelectNextAppointment; } }
		protected internal override int CalcCurrentSelectedAppointmentControlIndex(AppointmentControlCollection selectedAppointmentControls) {
			return selectedAppointmentControls.Count - 1;
		}
		protected internal override AppointmentControl CalculateNewDefaultSelectedAppointmentControl(AppointmentControlCollection visibleAppointmentControls) {
			return visibleAppointmentControls[0];
		}
		protected internal override int CalcNewSelectedAppointmentControlIndex(AppointmentControlCollection visibleAppointmentControls, int currentSelectedAppointmentControlIndex) {
			int result = currentSelectedAppointmentControlIndex;
			Appointment apt = visibleAppointmentControls[currentSelectedAppointmentControlIndex].Appointment;
			for (; ; ) {
				result++;
				int count = visibleAppointmentControls.Count;
				if (result >= count)
					return -1;
				else {
					if (visibleAppointmentControls[result].Appointment != apt)
						return result;
				}
			}
		}
		protected internal override AppointmentControl FindNearestAppointmentControl(AppointmentControlCollection appointmentControls, TimeInterval interval) {
			int count = appointmentControls.Count;
			if (count <= 0)
				return null;
			for (int i = 0; i < count; i++) {
				if (appointmentControls[i].ViewInfo.Interval.Start >= interval.Start)
					return appointmentControls[i];
			}
			return appointmentControls[0];
		}
		protected internal override int CalcNewBoundaryAppointment(int visibleAppointmentsControlsCount, int indexOfSelectedAppointment) {
			return (visibleAppointmentsControlsCount - 1) - indexOfSelectedAppointment;
		}
#if !SL
		protected override MoveFocusNextPrevCommandBase CreateLeaveFocusCommand() {
			return new MoveFocusNextCommand(SchedulerControl);
		}
#endif
	}
	#endregion
	#region SelectPrevAppointmentCommand
	public class SelectPrevAppointmentCommand : SelectPrevNextAppointmentCommand {
		public SelectPrevAppointmentCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SelectPrevAppointment; } }
		protected internal override int CalcCurrentSelectedAppointmentControlIndex(AppointmentControlCollection selectedAppointmentControls) {
			return 0;
		}
		protected internal override AppointmentControl CalculateNewDefaultSelectedAppointmentControl(AppointmentControlCollection visibleAppointmentControls) {
			return visibleAppointmentControls[visibleAppointmentControls.Count - 1];
		}
		protected internal override int CalcNewSelectedAppointmentControlIndex(AppointmentControlCollection visibleAppointmentControls, int currentSelectedAppointmentControlIndex) {
			int result = currentSelectedAppointmentControlIndex;
			Appointment apt = visibleAppointmentControls[currentSelectedAppointmentControlIndex].Appointment;
			for (; ; ) {
				result--;
				if (result < 0)
					return -1;
				else {
					if (visibleAppointmentControls[result].Appointment != apt)
						return result;
				}
			}
		}
		protected internal override AppointmentControl FindNearestAppointmentControl(AppointmentControlCollection appointmentControls, TimeInterval interval) {
			int count = appointmentControls.Count;
			if (count <= 0)
				return null;
			for (int i = count - 1; i >= 0; i--) {
				if (appointmentControls[i].ViewInfo.Interval.End <= interval.End)
					return appointmentControls[i];
			}
			return appointmentControls[count - 1];
		}
		protected internal override int CalcNewBoundaryAppointment(int visibleAppointmentsControlsCount, int indexOfSelectedAppointment) {
			if (indexOfSelectedAppointment == 0)
				return visibleAppointmentsControlsCount - 1;
			return 0;
		}
#if !SL
		protected override MoveFocusNextPrevCommandBase CreateLeaveFocusCommand() {
			return new MoveFocusPrevCommand(SchedulerControl);
		}
#endif
	}
	#endregion
	#region AppointmentViewInfoTabOrderComparer
	public class VisualAppointmentControlTabOrderComparer : IComparer<AppointmentControl> {
		public int Compare(AppointmentControl x, AppointmentControl y) {
			Appointment aptX = x.Appointment;
			Appointment aptY = y.Appointment;
			int result = Comparer.Default.Compare(aptX.Start.Date, aptY.Start.Date);
			if (result != 0)
				return result;
			result = SeveralDaysAppointmentCore(aptY) - SeveralDaysAppointmentCore(aptX);
			if (result != 0)
				return result;
			result = Comparer.Default.Compare(aptX.Start.TimeOfDay, aptY.Start.TimeOfDay);
			if (result != 0)
				return result;
			result = -Comparer.Default.Compare(aptX.End, aptY.End);
			if (result != 0)
				return result;
			result = Comparer.Default.Compare(aptX.RowHandle, aptY.RowHandle);
			if (result != 0)
				return result;
			aptX = aptX.RecurrencePattern;
			aptY = aptY.RecurrencePattern;
			if (aptX == null)
				return (aptY == null) ? 0 : 1;
			else {
				if (aptY == null)
					return -1;
				else
					return Comparer.Default.Compare(aptX.RowHandle, aptY.RowHandle);
			}
		}
		protected internal virtual int SeveralDaysAppointmentCore(Appointment apt) {
			return apt.LongerThanADay ? 1 : 0;
		}
	}
	#endregion
}
