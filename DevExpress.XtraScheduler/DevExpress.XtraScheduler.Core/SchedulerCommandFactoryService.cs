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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Commands.Internal;
namespace DevExpress.XtraScheduler.Services {
	#region ISchedulerCommandFactoryService
	public interface ISchedulerCommandFactoryService {
		SchedulerCommand CreateCommand(SchedulerCommandId id);
	}
	#endregion
	#region SchedulerCommandFactoryServiceWrapper
	public class SchedulerCommandFactoryServiceWrapper : ISchedulerCommandFactoryService {
		readonly ISchedulerCommandFactoryService service;
		public SchedulerCommandFactoryServiceWrapper(ISchedulerCommandFactoryService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		public ISchedulerCommandFactoryService Service { get { return service; } }
		#region ISchedulerCommandFactoryService Members
		public virtual SchedulerCommand CreateCommand(SchedulerCommandId id) {
			return Service.CreateCommand(id);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Services.Implementation {
	#region SchedulerCommandConstructorTable
	public class SchedulerCommandConstructorTable : Dictionary<SchedulerCommandId, ConstructorInfo> {
	}
	#endregion
	#region SchedulerCommandFactoryService
	public class SchedulerCommandFactoryService : ISchedulerCommandFactoryService {
		#region Fields
		readonly ISchedulerCommandTarget target;
		readonly SchedulerCommandConstructorTable commandConstructorTable;
		readonly List<Type[]> constructorParametersList = new List<Type[]>();
		#endregion
		public SchedulerCommandFactoryService(ISchedulerCommandTarget target) {
			Guard.ArgumentNotNull(target, "target");
			this.target = target;
			RegisterConstructorParameters();
			this.commandConstructorTable = CreateCommandConstructorTable();
		}
		public ISchedulerCommandTarget Target { get { return target; } }
		protected virtual void RegisterConstructorParameters() {
			RegisterConstructorParameters(new Type[] { typeof(ISchedulerCommandTarget) });
			RegisterConstructorParameters(new Type[] { typeof(InnerSchedulerControl) });
		}
		protected void RegisterConstructorParameters(Type[] constructorParameters) {
			this.constructorParametersList.Add(constructorParameters);
		}
		protected internal SchedulerCommandConstructorTable CreateCommandConstructorTable() {
			SchedulerCommandConstructorTable result = new SchedulerCommandConstructorTable();
			PopulateConstructorTable(result);
			return result;
		}
		protected void AddCommandConstructor(SchedulerCommandConstructorTable table, SchedulerCommandId commandId, Type commandType) {
			ConstructorInfo ci = null;
			int count = this.constructorParametersList.Count;
			for (int i = 0; i < count; i++) {
				ci = commandType.GetConstructor(this.constructorParametersList[i]);
				if (ci != null)
					break;
			}
			if (ci == null)
				Exceptions.ThrowArgumentException("commandType", commandType);
			table.Add(commandId, ci);
		}
		protected void RemoveCommandConstructor(SchedulerCommandConstructorTable table, SchedulerCommandId commandId) {
			if (!table.ContainsKey(commandId))
				return;
			table.Remove(commandId);
		}
		protected internal virtual void PopulateConstructorTable(SchedulerCommandConstructorTable table) {
			AddCommandConstructor(table, SchedulerCommandId.IncrementResourcePerPageCount, typeof(IncrementResourcePerPageCountCommand));
			AddCommandConstructor(table, SchedulerCommandId.DecrementResourcePerPageCount, typeof(DecrementResourcePerPageCountCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateNextResource, typeof(NavigateNextResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateResourcePageForward, typeof(NavigateResourcePageForwardCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateLastResource, typeof(NavigateLastResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateFirstResource, typeof(NavigateFirstResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateResourcePageBackward, typeof(NavigateResourcePageBackwardCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigatePrevResource, typeof(NavigatePrevResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToDayView, typeof(SwitchToDayViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToWorkWeekView, typeof(SwitchToWorkWeekViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToWeekView, typeof(SwitchToWeekViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToMonthView, typeof(SwitchToMonthViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToTimelineView, typeof(SwitchToTimelineViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToGanttView, typeof(SwitchToGanttViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToFullWeekView, typeof(SwitchToFullWeekViewCommand));
			AddCommandConstructor(table, SchedulerCommandId.GotoDay, typeof(GotoDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.GotoToday, typeof(ServiceGotoTodayCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateViewBackward, typeof(ServiceNavigateViewBackwardCommand));
			AddCommandConstructor(table, SchedulerCommandId.NavigateViewForward, typeof(ServiceNavigateViewForwardCommand));
			AddCommandConstructor(table, SchedulerCommandId.ViewZoomIn, typeof(ViewZoomInCommand));
			AddCommandConstructor(table, SchedulerCommandId.ViewZoomOut, typeof(ViewZoomOutCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToGroupByNone, typeof(SwitchToGroupByNoneCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToGroupByDate, typeof(SwitchToGroupByDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToGroupByResource, typeof(SwitchTOGroupByResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.NewAppointment, typeof(NewAppointmentCommand));
			AddCommandConstructor(table, SchedulerCommandId.NewRecurringAppointment, typeof(NewRecurringAppointmentCommand));
			AddCommandConstructor(table, SchedulerCommandId.DeleteAppointmentsUI, typeof(DeleteAppointmentUICommand));
			AddCommandConstructor(table, SchedulerCommandId.DeleteSeriesUI, typeof(DeleteSeriesUICommand));
			AddCommandConstructor(table, SchedulerCommandId.DeleteOccurrenceUI, typeof(DeleteOccurrenceUICommand));
			AddCommandConstructor(table, SchedulerCommandId.PrintPreview, typeof(PrintPreviewCommand));
			AddCommandConstructor(table, SchedulerCommandId.Print, typeof(PrintCommand));
			AddCommandConstructor(table, SchedulerCommandId.PrintPageSetup, typeof(PrintPageSetupCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchShowWorkTimeOnly, typeof(SwitchShowWorkTimeOnlyCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchCompressWeekend, typeof(SwitchCompressWeekendCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchCellsAutoHeight, typeof(SwitchCellsAutoHeightCommand));
			AddCommandConstructor(table, SchedulerCommandId.ToggleRecurrence, typeof(ToggleRecurrenceCommand));
			AddCommandConstructor(table, SchedulerCommandId.ChangeAppointmentStatusUI, typeof(ChangeAppointmentStatusUICommand));
			AddCommandConstructor(table, SchedulerCommandId.ChangeAppointmentLabelUI, typeof(ChangeAppointmentLabelUICommand));
			AddCommandConstructor(table, SchedulerCommandId.ChangeAppointmentReminderUI, typeof(ChangeAppointmentReminderUICommand));
			AddCommandConstructor(table, SchedulerCommandId.ChangeTimelineScaleWidth, typeof(ChangeTimelineScaleWidthUICommand));
			AddCommandConstructor(table, SchedulerCommandId.OpenSchedule, typeof(OpenScheduleCommand));
			AddCommandConstructor(table, SchedulerCommandId.SaveSchedule, typeof(SaveScheduleCommand));
			AddCommandConstructor(table, SchedulerCommandId.ChangeSnapToCellsUI, typeof(ChangeSnapToCellsUICommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentUI, typeof(EditNormalAppointmentUICommand));
			AddCommandConstructor(table, SchedulerCommandId.EditOccurrenceUI, typeof(EditOccurrenceUICommand));
			AddCommandConstructor(table, SchedulerCommandId.EditSeriesUI, typeof(EditSeriesUICommand));
			AddCommandConstructor(table, SchedulerCommandId.ExpandResource, typeof(ExpandResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.CollapseResource, typeof(CollapseResourceCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchTimeScalesUICommand, typeof(SwitchTimeScalesUICommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchTimeScalesCaptionUICommand, typeof(SwitchTimeScalesCaptionUICommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentSeriesGroup, typeof(EditAppointmentSeriesGroupCommand));
			AddCommandConstructor(table, SchedulerCommandId.DeleteAppointmentSeriesGroupCommand, typeof(DeleteAppointmentSeriesGroupCommand));
			AddCommandConstructor(table, SchedulerCommandId.ToolsAppointmentCommandGroup, typeof(ToolsAppointmentCommandGroupCommand));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToMoreDetailedView, typeof(SwitchToMoreDetailedView));
			AddCommandConstructor(table, SchedulerCommandId.SwitchToLessDetailedView, typeof(SwitchToLessDetailedView));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMovePrevDay, typeof(DayViewMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveNextDay, typeof(DayViewMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewGroupByResourceMovePrevDay, typeof(DayViewGroupByResourceMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewGroupByResourceMoveNextDay, typeof(DayViewGroupByResourceMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewGroupByDateMovePrevDay, typeof(DayViewGroupByDateMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewGroupByDateMoveNextDay, typeof(DayViewGroupByDateMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentQuery, typeof(EditAppointmentQueryCommand));
			AddCommandConstructor(table, SchedulerCommandId.GotoDate, typeof(GotoDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.DeleteAppointmentsQueryOrDependencies, typeof(DeleteAppointmentsQueryOrDependenciesCommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentViaInplaceEditor, typeof(EditAppointmentViaInplaceEditorCommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentOrNewAppointmentViaInplaceEditor, typeof(EditAppointmentOrNewAppointmentViaInplaceEditorCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveFirstDayOfWeek, typeof(DayViewMoveFirstDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveLastDayOfWeek, typeof(DayViewMoveLastDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveStartOfMonth, typeof(DayViewMoveStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveEndOfMonth, typeof(DayViewMoveEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMovePrevWeek, typeof(DayViewMovePrevWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveNextWeek, typeof(DayViewMoveNextWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendFirstDayOfWeek, typeof(DayViewExtendFirstDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendLastDayOfWeek, typeof(DayViewExtendLastDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendStartOfMonth, typeof(DayViewExtendStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendEndOfMonth, typeof(DayViewExtendEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendPrevDay, typeof(DayViewExtendPrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendNextDay, typeof(DayViewExtendNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMovePrevCell, typeof(DayViewMovePrevCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveNextCell, typeof(DayViewMoveNextCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveToStartOfWorkTime, typeof(DayViewMoveToStartOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveToEndOfWorkTime, typeof(DayViewMoveToEndOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveToStartOfVisibleTime, typeof(DayViewMoveToStartOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMoveToEndOfVisibleTime, typeof(DayViewMoveToEndOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMovePageUp, typeof(DayViewMovePageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewMovePageDown, typeof(DayViewMovePageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendPrevCell, typeof(DayViewExtendPrevCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendNextCell, typeof(DayViewExtendNextCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendToStartOfWorkTime, typeof(DayViewExtendToStartOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendToEndOfWorkTime, typeof(DayViewExtendToEndOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendToStartOfVisibleTime, typeof(DayViewExtendToStartOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendToEndOfVisibleTime, typeof(DayViewExtendToEndOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendPageUp, typeof(DayViewExtendPageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.DayViewExtendPageDown, typeof(DayViewExtendPageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveFirstDayOfWeek, typeof(WorkWeekViewMoveFirstDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveLastDayOfWeek, typeof(WorkWeekViewMoveLastDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMovePrevWeek, typeof(WorkWeekViewMovePrevWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveNextWeek, typeof(WorkWeekViewMoveNextWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendFirstDayOfWeek, typeof(WorkWeekViewExtendFirstDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendLastDayOfWeek, typeof(WorkWeekViewExtendLastDayOfWeekCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendPrevDay, typeof(WorkWeekViewExtendPrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendNextDay, typeof(WorkWeekViewExtendNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMovePrevCell, typeof(WorkWeekViewMovePrevCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveNextCell, typeof(WorkWeekViewMoveNextCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveToStartOfWorkTime, typeof(WorkWeekViewMoveToStartOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveToEndOfWorkTime, typeof(WorkWeekViewMoveToEndOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveToStartOfVisibleTime, typeof(WorkWeekViewMoveToStartOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveToEndOfVisibleTime, typeof(WorkWeekViewMoveToEndOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMovePageUp, typeof(WorkWeekViewMovePageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMovePageDown, typeof(WorkWeekViewMovePageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendPrevCell, typeof(WorkWeekViewExtendPrevCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendNextCell, typeof(WorkWeekViewExtendNextCellCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendToStartOfWorkTime, typeof(WorkWeekViewExtendToStartOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendToEndOfWorkTime, typeof(WorkWeekViewExtendToEndOfWorkTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendToStartOfVisibleTime, typeof(WorkWeekViewExtendToStartOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendToEndOfVisibleTime, typeof(WorkWeekViewExtendToEndOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendPageUp, typeof(WorkWeekViewExtendPageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewExtendPageDown, typeof(WorkWeekViewExtendPageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMovePrevDay, typeof(WorkWeekViewMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewMoveNextDay, typeof(WorkWeekViewMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewGroupByResourceMovePrevDay, typeof(WorkWeekViewGroupByResourceMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewGroupByResourceMoveNextDay, typeof(WorkWeekViewGroupByResourceMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewGroupByDateMovePrevDay, typeof(WorkWeekViewGroupByDateMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WorkWeekViewGroupByDateMoveNextDay, typeof(WorkWeekViewGroupByDateMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveLeft, typeof(WeekViewMoveLeftCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveRight, typeof(WeekViewMoveRightCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMovePrevDay, typeof(WeekViewMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveNextDay, typeof(WeekViewMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveFirstDay, typeof(WeekViewMoveFirstDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveLastDay, typeof(WeekViewMoveLastDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMovePageUp, typeof(WeekViewMovePageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMovePageDown, typeof(WeekViewMovePageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveStartOfMonth, typeof(WeekViewMoveStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewMoveEndOfMonth, typeof(WeekViewMoveEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendLeft, typeof(WeekViewExtendLeftCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendRight, typeof(WeekViewExtendRightCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendPrevDay, typeof(WeekViewExtendPrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendNextDay, typeof(WeekViewExtendNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendFirstDay, typeof(WeekViewExtendFirstDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendLastDay, typeof(WeekViewExtendLastDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendPageUp, typeof(WeekViewExtendPageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendPageDown, typeof(WeekViewExtendPageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendStartOfMonth, typeof(WeekViewExtendStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewExtendEndOfMonth, typeof(WeekViewExtendEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByResourceMoveLeft, typeof(WeekViewGroupByResourceMoveLeftCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByResourceMoveRight, typeof(WeekViewGroupByResourceMoveRightCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByDateMoveUp, typeof(WeekViewGroupByDateMoveUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByDateMoveDown, typeof(WeekViewGroupByDateMoveDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByDateExtendUp, typeof(WeekViewGroupByDateExtendUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.WeekViewGroupByDateExtendDown, typeof(WeekViewGroupByDateExtendDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMovePrevDay, typeof(MonthViewMovePrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveNextDay, typeof(MonthViewMoveNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveUp, typeof(MonthViewMoveUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveDown, typeof(MonthViewMoveDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveFirstDay, typeof(MonthViewMoveFirstDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveLastDay, typeof(MonthViewMoveLastDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMovePageUp, typeof(MonthViewMovePageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMovePageDown, typeof(MonthViewMovePageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveStartOfMonth, typeof(MonthViewMoveStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewMoveEndOfMonth, typeof(MonthViewMoveEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendPrevDay, typeof(MonthViewExtendPrevDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendNextDay, typeof(MonthViewExtendNextDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendUp, typeof(MonthViewExtendUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendDown, typeof(MonthViewExtendDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendFirstDay, typeof(MonthViewExtendFirstDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendLastDay, typeof(MonthViewExtendLastDayCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendPageUp, typeof(MonthViewExtendPageUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendPageDown, typeof(MonthViewExtendPageDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendStartOfMonth, typeof(MonthViewExtendStartOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewExtendEndOfMonth, typeof(MonthViewExtendEndOfMonthCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewGroupByResourceMoveLeft, typeof(MonthViewGroupByResourceMoveLeftCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewGroupByResourceMoveRight, typeof(MonthViewGroupByResourceMoveRightCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewGroupByDateMoveUp, typeof(MonthViewGroupByDateMoveUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.MonthViewGroupByDateMoveDown, typeof(MonthViewGroupByDateMoveDownCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMovePrevDate, typeof(TimelineViewMovePrevDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMoveNextDate, typeof(TimelineViewMoveNextDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime, typeof(TimelineViewMoveToStartOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime, typeof(TimelineViewMoveToEndOfVisibleTimeCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMoveToMajorStart, typeof(TimelineViewMoveToMajorStartCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewMoveToMajorEnd, typeof(TimelineViewMoveToMajorEndCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewExtendPrevDate, typeof(TimelineViewExtendPrevDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewExtendNextDate, typeof(TimelineViewExtendNextDateCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewGroupByDateMoveUp, typeof(TimelineViewGroupByDateMoveUpCommand));
			AddCommandConstructor(table, SchedulerCommandId.TimelineViewGroupByDateMoveDown, typeof(TimelineViewGroupByDateMoveDownCommand));
		}
		#region ICommandFactoryService Members
		public virtual SchedulerCommand CreateCommand(SchedulerCommandId commandId) {
			ConstructorInfo ci;
			if (commandConstructorTable.TryGetValue(commandId, out ci))
				return (SchedulerCommand)ci.Invoke(new object[] { Target });
			else {
				Exceptions.ThrowArgumentException("commandId", commandId);
				return null;
			}
		}
		#endregion
	}
	#endregion
}
