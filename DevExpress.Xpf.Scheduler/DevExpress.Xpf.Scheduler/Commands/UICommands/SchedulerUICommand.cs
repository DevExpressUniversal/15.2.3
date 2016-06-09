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

using System.Windows.Input;
using DevExpress.XtraScheduler.Commands;
using System;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Scheduler {
	public abstract class SchedulerUICommandBase : ICommand {
		SchedulerCommandId id;
		bool canExecuteAlwaysTrue = true;
		protected SchedulerUICommandBase(SchedulerCommandId id, bool canExecuteAlwaysTrue) {
			this.id = id;
			this.canExecuteAlwaysTrue = canExecuteAlwaysTrue;
		}
		protected SchedulerUICommandBase(SchedulerCommandId id) : this(id, true) { 
		}
		protected SchedulerUICommandBase()
			: this(SchedulerCommandId.None, true) {
		}
		public virtual SchedulerCommandId CommandId { get { return id; } }
		public object CommandContext { get; set; }
		#region ICommand Members
		public virtual bool CanExecute(object parameter) {
			SchedulerControl control = ObtainSchedulerControl(parameter);
			if (control == null)
				return false;
			return CanExecuteCore(control);
		}
		EventHandler onCanExecuteChanged;
		public event EventHandler CanExecuteChanged {
			add {
				onCanExecuteChanged += value;
#if !SL
				CommandManager.RequerySuggested += value;
#endif
			}
			remove {
				onCanExecuteChanged -= value;
#if !SL
				CommandManager.RequerySuggested -= value;
#endif
			}
		}
		internal void RaiseCanExecuteChanged() {
			if (onCanExecuteChanged != null)
				onCanExecuteChanged(this, EventArgs.Empty);
		}
		public virtual void Execute(object parameter) {
			SchedulerControl control = ObtainSchedulerControl(parameter);
			if (control == null)
				Exceptions.ThrowArgumentException("parameter", parameter);
			ExecuteCore(control);
		}
		#endregion
		protected abstract void ExecuteCore(SchedulerControl control);
		protected virtual bool CanExecuteCore(SchedulerControl control) {
			if (this.canExecuteAlwaysTrue)
				return true;
			SchedulerCommand command = CreateCommand(control);
			if (command == null)
				return false;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			return state.Enabled;
		}
		public virtual SchedulerCommand CreateCommand(SchedulerControl control) {
			return control.CreateCommand(CommandId);
		}
		protected internal virtual SchedulerControl ObtainSchedulerControl(object parameter) {
			SchedulerControl control = parameter as SchedulerControl;
			if (control != null)
				return control;
			SchedulerControlAccessor accessor = parameter as SchedulerControlAccessor;
			if (accessor != null)
				return accessor.SchedulerControl;
			return null;
		}
	}
	public class SchedulerUICommand : SchedulerUICommandBase, ICommandWithInvoker {
		static readonly SchedulerUICommand gotoToday = new SchedulerUICommand(SchedulerCommandId.GotoToday);
		static readonly SchedulerUICommand switchToDayView = new SchedulerUICommand(SchedulerCommandId.SwitchToDayView);
		static readonly SchedulerUICommand switchToMonthView = new SchedulerUICommand(SchedulerCommandId.SwitchToMonthView);
		static readonly SchedulerUICommand switchToFullWeekView = new SchedulerUICommand(SchedulerCommandId.SwitchToFullWeekView);
		static readonly SchedulerUICommand switchToTimelineView = new SchedulerUICommand(SchedulerCommandId.SwitchToTimelineView);
		static readonly SchedulerUICommand switchToWeekView = new SchedulerUICommand(SchedulerCommandId.SwitchToWeekView);
		static readonly SchedulerUICommand switchToWorkWeekView = new SchedulerUICommand(SchedulerCommandId.SwitchToWorkWeekView);
		static readonly SchedulerUICommand navigateViewBackward = new SchedulerUICommand(SchedulerCommandId.NavigateViewBackward);
		static readonly SchedulerUICommand navigateViewForward = new SchedulerUICommand(SchedulerCommandId.NavigateViewForward);
		static readonly SchedulerUICommand viewZoomIn = new SchedulerUICommand(SchedulerCommandId.ViewZoomIn);
		static readonly SchedulerUICommand viewZoomOut = new SchedulerUICommand(SchedulerCommandId.ViewZoomOut);
		static readonly SchedulerUICommand switchToGroupByNone = new SchedulerUICommand(SchedulerCommandId.SwitchToGroupByNone);
		static readonly SchedulerUICommand switchToGroupByDate = new SchedulerUICommand(SchedulerCommandId.SwitchToGroupByDate);
		static readonly SchedulerUICommand switchToGroupByResource = new SchedulerUICommand(SchedulerCommandId.SwitchToGroupByResource);
		static readonly SchedulerUICommand decrementResourcePerPageCount = new SchedulerUICommand(SchedulerCommandId.DecrementResourcePerPageCount);
		static readonly SchedulerUICommand incrementResourcePerPageCount = new SchedulerUICommand(SchedulerCommandId.IncrementResourcePerPageCount);
		static readonly SchedulerUICommand navigateFirstResource = new SchedulerUICommand(SchedulerCommandId.NavigateFirstResource, false);
		static readonly SchedulerUICommand navigateLastResource = new SchedulerUICommand(SchedulerCommandId.NavigateLastResource, false);
		static readonly SchedulerUICommand navigateNextResource = new SchedulerUICommand(SchedulerCommandId.NavigateNextResource, false);
		static readonly SchedulerUICommand navigatePrevResource = new SchedulerUICommand(SchedulerCommandId.NavigatePrevResource, false);
		static readonly SchedulerUICommand navigateResourcePageBackward = new SchedulerUICommand(SchedulerCommandId.NavigateResourcePageBackward, false);
		static readonly SchedulerUICommand navigateResourcePageForward = new SchedulerUICommand(SchedulerCommandId.NavigateResourcePageForward, false);
		static readonly SchedulerUICommand newAppointment = new SchedulerUICommand(SchedulerCommandId.NewAppointment);
		static readonly SchedulerUICommand newRecurringAppointment = new SchedulerUICommand(SchedulerCommandId.NewRecurringAppointment);
		static readonly SchedulerUICommand switchTimeScalesUICommand = new SchedulerUICommand(SchedulerCommandId.SwitchTimeScalesUICommand);
		static readonly SchedulerUICommand switchTimeScalesCaptionUICommand = new SchedulerUICommand(SchedulerCommandId.SwitchTimeScalesCaptionUICommand);
		static readonly SchedulerUICommand setTimeIntervalCount = new SchedulerUICommand(SchedulerCommandId.SetTimeIntervalCount);
		static readonly SchedulerUICommand switchCompressWeekend = new SchedulerUICommand(SchedulerCommandId.SwitchCompressWeekend);
		static readonly SchedulerUICommand switchShowWorkTimeOnly = new SchedulerUICommand(SchedulerCommandId.SwitchShowWorkTimeOnly);
		static readonly SchedulerUICommand switchCellsAutoHeight = new SchedulerUICommand(SchedulerCommandId.SwitchCellsAutoHeight);
		static readonly SchedulerUICommand changeSnapToCellsUI = new SchedulerUICommand(SchedulerCommandId.ChangeSnapToCellsUI);
		static readonly SchedulerUICommand openSchedule = new SchedulerUICommand(SchedulerCommandId.OpenSchedule);
		static readonly SchedulerUICommand saveSchedule = new SchedulerUICommand(SchedulerCommandId.SaveSchedule);
		static readonly SchedulerUICommand editAppointment = new SchedulerUICommand(SchedulerCommandId.EditAppointmentUI);
		static readonly SchedulerUICommand editAppointmentSeriesGroup = new SchedulerUICommand(SchedulerCommandId.EditAppointmentSeriesGroup);
		static readonly SchedulerUICommand editOccurrence = new SchedulerUICommand(SchedulerCommandId.EditOccurrenceUI);
		static readonly SchedulerUICommand editPattern = new SchedulerUICommand(SchedulerCommandId.EditSeriesUI);
		static readonly SchedulerUICommand deleteAppointment = new SchedulerUICommand(SchedulerCommandId.DeleteAppointmentsUI);
		static readonly SchedulerUICommand deleteAppointmentSeriesGroup = new SchedulerUICommand(SchedulerCommandId.DeleteAppointmentSeriesGroupCommand);
		static readonly SchedulerUICommand deleteOccurrence = new SchedulerUICommand(SchedulerCommandId.DeleteSeriesUI);
		static readonly SchedulerUICommand deletePattern = new SchedulerUICommand(SchedulerCommandId.DeleteOccurrenceUI);
		static readonly SchedulerUICommand changeAppointmentStatus = new SchedulerUICommand(SchedulerCommandId.ChangeAppointmentStatusUI);
		static readonly SchedulerUICommand changeAppointmentLabel = new SchedulerUICommand(SchedulerCommandId.ChangeAppointmentLabelUI);
		static readonly SchedulerUICommand toggleRecurrence = new SchedulerUICommand(SchedulerCommandId.ToggleRecurrence);
		static readonly SchedulerUICommand toolsAppointmentCommandGroup = new SchedulerUICommand(SchedulerCommandId.ToolsAppointmentCommandGroup);
		public static SchedulerUICommand GotoToday { get { return gotoToday; } }
		public static SchedulerUICommand SwitchToDayView { get { return switchToDayView; } }
		public static SchedulerUICommand SwitchToMonthView { get { return switchToMonthView; } }
		public static SchedulerUICommand SwitchToFullWeekView { get { return switchToFullWeekView; } }
		public static SchedulerUICommand SwitchToTimelineView { get { return switchToTimelineView; } }
		public static SchedulerUICommand SwitchToWeekView { get { return switchToWeekView; } }
		public static SchedulerUICommand SwitchToWorkWeekView { get { return switchToWorkWeekView; } }
		public static SchedulerUICommand NavigateViewBackward { get { return navigateViewBackward; } }
		public static SchedulerUICommand NavigateViewForward { get { return navigateViewForward; } }
		public static SchedulerUICommand ViewZoomIn { get { return viewZoomIn; } }
		public static SchedulerUICommand ViewZoomOut { get { return viewZoomOut; } }
		public static SchedulerUICommand SwitchToGroupByNone { get { return switchToGroupByNone; } }
		public static SchedulerUICommand SwitchToGroupByDate { get { return switchToGroupByDate; } }
		public static SchedulerUICommand SwitchToGroupByResource { get { return switchToGroupByResource; } }
		public static SchedulerUICommand DecrementResourcePerPageCount { get { return decrementResourcePerPageCount; } }
		public static SchedulerUICommand IncrementResourcePerPageCount { get { return incrementResourcePerPageCount; } }
		public static SchedulerUICommand NavigateFirstResource { get { return navigateFirstResource; } }
		public static SchedulerUICommand NavigateLastResource { get { return navigateLastResource; } }
		public static SchedulerUICommand NavigateNextResource { get { return navigateNextResource; } }
		public static SchedulerUICommand NavigatePrevResource { get { return navigatePrevResource; } }
		public static SchedulerUICommand NavigateResourcePageBackward { get { return navigateResourcePageBackward; } }
		public static SchedulerUICommand NavigateResourcePageForward { get { return navigateResourcePageForward; } }
		public static SchedulerUICommand NewAppointment { get { return newAppointment; } }
		public static SchedulerUICommand NewRecurringAppointment { get { return newRecurringAppointment; } }
		public static SchedulerUICommand SwitchTimeScalesUICommand { get { return switchTimeScalesUICommand; } }
		public static SchedulerUICommand SwitchTimeScalesCaptionUICommand { get { return switchTimeScalesCaptionUICommand; } }
		public static SchedulerUICommand SetTimeIntervalCount { get { return setTimeIntervalCount; } }
		public static SchedulerUICommand SwitchCompressWeekend { get { return switchCompressWeekend; } }
		public static SchedulerUICommand SwitchShowWorkTimeOnly { get { return switchShowWorkTimeOnly; } }
		public static SchedulerUICommand SwitchCellsAutoHeight { get { return switchCellsAutoHeight; } }
		public static SchedulerUICommand ChangeSnapToCellsUI { get { return changeSnapToCellsUI; } }
		public static SchedulerUICommand OpenSchedule { get { return openSchedule; } }
		public static SchedulerUICommand SaveSchedule { get { return saveSchedule; } }
		public static SchedulerUICommand EditAppointment { get { return editAppointment; } }
		public static SchedulerUICommand EditAppointmentSeriesGroup { get { return editAppointmentSeriesGroup; } }
		public static SchedulerUICommand EditOccurrence { get { return editOccurrence; } }
		public static SchedulerUICommand EditPattern { get { return editPattern; } }
		public static SchedulerUICommand DeleteAppointment { get { return deleteAppointment; } }
		public static SchedulerUICommand DeleteAppointmentSeriesGroup { get { return deleteAppointmentSeriesGroup; } }
		public static SchedulerUICommand DeleteOccurrence { get { return deleteOccurrence; } }
		public static SchedulerUICommand DeletePattern { get { return deletePattern; } }
		public static SchedulerUICommand ChangeAppointmentStatus { get { return changeAppointmentStatus; } }
		public static SchedulerUICommand ChangeAppointmentLabel { get { return changeAppointmentLabel; } }
		public static SchedulerUICommand ToggleRecurrence { get { return toggleRecurrence; } }
		public static SchedulerUICommand ToolsAppointmentCommandGroup { get { return toolsAppointmentCommandGroup; } }
		public static SchedulerUICommand CreateInstance(SchedulerCommandId id) {
			return new SchedulerUICommand(id, true);
		}
		public static SchedulerUICommand CreateInstance(SchedulerCommandId id, bool canExecuteAlwaysTrue) {
			return new SchedulerUICommand(id, canExecuteAlwaysTrue);
		}
		public SchedulerUICommand() {
		}
		protected SchedulerUICommand(SchedulerCommandId id)
			: base(id, true) {
		}
		protected SchedulerUICommand(SchedulerCommandId id, bool canExecuteAlwaysTrue) : base(id, canExecuteAlwaysTrue) {
		}
		protected override void ExecuteCore(SchedulerControl control) {
			control.CommandManager.ExecuteParametrizedCommand(this, null);
		}
		#region ICommandWithInvoker Members
		public virtual void Execute(object invoker, object parameter) {
			SchedulerControl control = ObtainSchedulerControl(parameter);
			if (control == null)
				return;
			control.CommandManager.ExecuteParametrizedCommand(this, null);
			control.CommandManager.UpdateBarItemState(invoker as BarItem);
		}
		#endregion
	}
}
