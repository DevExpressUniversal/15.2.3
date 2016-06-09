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
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Native {
	#region VisibleIntervalsChangeType
	public enum VisibleIntervalsChangeType {
		None,
		Shift,
		CreateWithSelectionAtBegin,
		CreateWithSelectionAtEnd
	}
	#endregion
	public abstract class SelectionCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		SchedulerViewSelection selection;
		TimeInterval newSelectionInterval;
		TimeSpan visibleIntervalsOffset;
		int resourceOffset;
		int firstVisibleResourceOffset;
		int currentFilteredResourceIndex;
		int currentVisibleResourceIndex;
		int filteredResourceCount;
		int visibleResourceCount;
		TimeScale scale;
		#endregion
		protected SelectionCommand(ISchedulerCommandTarget target)
			: base(target) {
			this.scale = CreateScale();
			this.selection = CalcCurrentSelection(InnerControl.Selection);
		}
		#region Properties
		protected internal SchedulerViewSelection Selection { get { return selection; } }
		protected internal InnerSchedulerViewBase View { get { return InnerControl.ActiveView; } }
		protected internal TimeInterval NewSelectionInterval { get { return newSelectionInterval; } set { newSelectionInterval = value; } }
		protected internal int ResourceOffset { get { return resourceOffset; } set { resourceOffset = value; } }
		protected internal int FirstVisibleResourceOffset { get { return firstVisibleResourceOffset; } set { firstVisibleResourceOffset = value; } }
		protected internal TimeSpan VisibleIntervalsOffset { get { return visibleIntervalsOffset; } set { visibleIntervalsOffset = value; } }
		protected internal int CurrentFilteredResourceIndex { get { return currentFilteredResourceIndex; } internal set { currentFilteredResourceIndex = value; } }
		protected internal int FilteredResourceCount { get { return filteredResourceCount; } internal set { filteredResourceCount = value; } }
		protected internal int CurrentVisibleResourceIndex { get { return currentVisibleResourceIndex; } }
		protected internal int VisibleResourceCount { get { return visibleResourceCount; } }
		protected internal virtual bool ExtendSelection { get { return false; } }
		protected internal virtual bool KeepSelectionDuration { get { return false; } }
		protected internal TimeScale Scale { get { return scale; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string MenuCaption { get { return GetType().Name; } }
		public override string Description { get { return "Description:" + GetType().Name; } }
		protected internal virtual VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.Shift; } }
		#endregion
		protected internal virtual SchedulerViewSelection CalcCurrentSelection(SchedulerViewSelection controlSelection) {
			SchedulerViewSelection result = controlSelection.Clone();
			if (result.Forward) {
				DateTime intervalEnd = result.Interval.End;
				result.Interval = new TimeInterval(Scale.GetPrevDate(intervalEnd), intervalEnd);
			} else {
				DateTime intervalStart = result.Interval.Start;
				result.Interval = new TimeInterval(intervalStart, Scale.GetNextDate(intervalStart));
			}
			result.FirstSelectedInterval = result.Interval.Clone();
			return result;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected internal override void ExecuteCore() {
			CalculateCurrentResourceParameters();
			VisibleIntervalsChangeType changeType = ExecuteSelectionCommandCore();
			ApplyChanges(changeType);
		}
		protected internal virtual VisibleIntervalsChangeType ExecuteSelectionCommandCore() {
			this.newSelectionInterval = CalcNewSelectionInterval(Selection.Interval);
			this.visibleIntervalsOffset = CalcVisibleIntervalsOffset();
			VisibleIntervalsChangeType result = VisibleIntervalsChangeType.Shift;
			if (MoveSelectionIntoHoleVisibleIntervalsChangeType != VisibleIntervalsChangeType.None) {
				if (visibleIntervalsOffset == TimeSpan.Zero) {
					if (!IsViewVisibleIntervalsContainInterval(newSelectionInterval)) {
						result = MoveSelectionIntoHoleVisibleIntervalsChangeType;
						if (MoveSelectionIntoHoleVisibleIntervalsChangeType == VisibleIntervalsChangeType.Shift) {
							do {
								newSelectionInterval = CalcNewSelectionInterval(newSelectionInterval);
							}
							while (!IsViewVisibleIntervalsContainInterval(newSelectionInterval));
						}
					}
				}
			}
			this.resourceOffset = CalcResourceOffset();
			if (resourceOffset != 0) {
				this.firstVisibleResourceOffset = CalcFirstVisibleResourceOffset();
				this.resourceOffset = CorrectResourceOffset();
			}
			return result;
		}
		protected internal virtual bool IsViewVisibleIntervalsContainInterval(TimeInterval interval) {
			return View.InnerVisibleIntervals.Contains(interval);
		}
		protected internal virtual void CalculateCurrentResourceParameters() {
			this.filteredResourceCount = View.FilteredResources.Count;
			this.visibleResourceCount = View.VisibleResources.Count;
			Resource resource;
			resource = View.FilteredResources.GetResourceById(Selection.Resource.Id);
			this.currentFilteredResourceIndex = View.FilteredResources.IndexOf(resource);
			resource = View.VisibleResources.GetResourceById(Selection.Resource.Id);
			this.currentVisibleResourceIndex = View.VisibleResources.IndexOf(resource);
		}
		protected internal virtual int CorrectResourceOffset() {
			if (currentFilteredResourceIndex + resourceOffset >= filteredResourceCount)
				return CorrectResourceOffsetScrollForwardOutsideRange();
			else if (currentFilteredResourceIndex + resourceOffset < 0)
				return CorrectResourceOffsetScrollBackwardOutsideRange();
			else {
				CorrectNewSelectionInterval();
				CorrectVisibleIntervalsOffset();
				return resourceOffset;
			}
		}
		protected internal virtual int CorrectResourceOffsetScrollForwardOutsideRange() {
			return 0;
		}
		protected internal virtual int CorrectResourceOffsetScrollBackwardOutsideRange() {
			return 0;
		}
		protected internal virtual int CalcFirstVisibleResourceOffset() {
			int newResourceIndex = currentFilteredResourceIndex + resourceOffset;
			if (newResourceIndex >= filteredResourceCount)
				return CalcFirstVisibleResourceOffsetScrollForwardOutsideRange();
			else if (newResourceIndex < 0)
				return CalcFirstVisibleResourceOffsetScrollBackwardOutsideRange();
			else
				return CalcInnerFirstVisibleResourceOffset();
		}
		protected internal virtual int CalcFirstVisibleResourceOffsetScrollForwardOutsideRange() {
			return 0;
		}
		protected internal virtual int CalcFirstVisibleResourceOffsetScrollBackwardOutsideRange() {
			return 0;
		}
		protected internal virtual int CalcInnerFirstVisibleResourceOffset() {
			int newResourceIndex = currentVisibleResourceIndex + resourceOffset;
			if (newResourceIndex >= visibleResourceCount)
				return resourceOffset;
			else if (newResourceIndex < 0)
				return resourceOffset;
			else
				return 0;
		}
		protected internal virtual void ApplyChanges(VisibleIntervalsChangeType changeType) {
			TimeIntervalCollection newInnerVisibleIntervals = CloneViewInnerVisibleIntervals();
			ChangeActions actions = CreateNewInnerVisibleIntervals(newInnerVisibleIntervals, changeType);
			if (actions != ChangeActions.None) {
				if (newInnerVisibleIntervals.Start < View.LimitInterval.Start)
					return;
				if (newInnerVisibleIntervals.End > View.LimitInterval.End)
					return;
			}
			actions |= ApplyChangesCore(newInnerVisibleIntervals);
			ApplyChangesToControl(actions);
		}
		protected internal virtual TimeIntervalCollection CloneViewInnerVisibleIntervals() {
			return View.InnerVisibleIntervals.Clone();
		}
		protected internal virtual ChangeActions ApplyChangesCore(TimeIntervalCollection newInnerVisibleIntervals) {
			ApplyVisibleIntervalsOffset(newInnerVisibleIntervals);
			ApplyResourceOffset();
			ChangeActions actions = ApplyFirstVisibleResourceOffset();
			ApplyNewSelectionInterval();
			return actions;
		}
		protected internal virtual ChangeActions CreateNewInnerVisibleIntervals(TimeIntervalCollection newInnerVisibleIntervals, VisibleIntervalsChangeType changeType) {
			switch (changeType) {
				case VisibleIntervalsChangeType.CreateWithSelectionAtBegin:
					return CreateNewVisibleIntervalsWithNewSelectionAtBegin(newInnerVisibleIntervals);
				case VisibleIntervalsChangeType.CreateWithSelectionAtEnd:
					return CreateNewVisibleIntervalsWithNewSelectionAtEnd(newInnerVisibleIntervals);
				case VisibleIntervalsChangeType.Shift:
					return ShiftVisibleIntervals(newInnerVisibleIntervals);
				case VisibleIntervalsChangeType.None:
				default:
					return ChangeActions.None;
			}
		}
		protected internal virtual void ApplyVisibleIntervalsOffset(TimeIntervalCollection newInnerVisibleIntervals) {
			View.InnerVisibleIntervals.Clear();
			View.InnerVisibleIntervals.AddRange(newInnerVisibleIntervals);
		}
		protected internal virtual ChangeActions CreateNewVisibleIntervalsWithNewSelectionAtBegin(TimeIntervalCollection intervals) {
			int count = intervals.Count;
			intervals.Clear();
			intervals.Add(new TimeInterval(NewSelectionInterval.Start, TimeSpan.Zero));
			for (int i = 1; i < count; i++)
				intervals.Add(new TimeInterval(intervals.End, TimeSpan.FromTicks(1)));
			return ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RaiseVisibleIntervalChanged;
		}
		protected internal virtual ChangeActions CreateNewVisibleIntervalsWithNewSelectionAtEnd(TimeIntervalCollection intervals) {
			int count = intervals.Count;
			intervals.Clear();
			intervals.Add(new TimeInterval(NewSelectionInterval.Start, TimeSpan.Zero));
			for (int i = 1; i < count; i++)
				intervals.Add(new TimeInterval(intervals.Start.AddTicks(-1), TimeSpan.FromTicks(1)));
			return ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RaiseVisibleIntervalChanged;
		}
		protected internal virtual ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			if (visibleIntervalsOffset != TimeSpan.Zero) {
				intervals.Shift(visibleIntervalsOffset);
				return ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RaiseVisibleIntervalChanged | ChangeActions.RecalcPreliminaryLayout;
			} else
				return ChangeActions.None;
		}
		protected internal virtual void ApplyResourceOffset() {
			if (resourceOffset != 0)
				InnerControl.Selection.Resource = View.FilteredResources[currentFilteredResourceIndex + resourceOffset];
		}
		protected internal virtual ChangeActions ApplyFirstVisibleResourceOffset() {
			if (firstVisibleResourceOffset != 0) {
				View.SetFirstVisibleResourceIndexCore(View.ActualFirstVisibleResourceIndex + firstVisibleResourceOffset);
				return ChangeActions.RecalcViewLayout | ChangeActions.UpdateResourceScrollBarValue;
			} else
				return ChangeActions.None;
		}
		protected internal virtual void ApplyNewSelectionInterval() {
			SchedulerViewSelection controlSelection = InnerControl.Selection;
			if (ExtendSelection) {
				TimeInterval extendedSelectionInterval = CalcExtendedSelectionInterval();
				controlSelection.FirstSelectedInterval = extendedSelectionInterval.Intersect(controlSelection.FirstSelectedInterval);
				controlSelection.Interval = extendedSelectionInterval;
			} else {
				if (KeepSelectionDuration) {
					TimeSpan firstSelectedIntervalOffset = controlSelection.FirstSelectedInterval.Start - controlSelection.Interval.Start;
					if (controlSelection.Forward)
						controlSelection.Interval = new TimeInterval(newSelectionInterval.End - controlSelection.Interval.Duration, newSelectionInterval.End);
					else
						controlSelection.Interval = new TimeInterval(newSelectionInterval.Start, newSelectionInterval.Start + controlSelection.Interval.Duration);
					controlSelection.FirstSelectedInterval = new TimeInterval(controlSelection.Interval.Start + firstSelectedIntervalOffset, controlSelection.FirstSelectedInterval.Duration);
				} else {
					controlSelection.Interval = newSelectionInterval;
					controlSelection.FirstSelectedInterval = newSelectionInterval.Clone();
				}
			}
		}
		protected internal virtual bool CalcNewForward(TimeInterval newSelectionInterval) {
			TimeInterval controlSelectionInterval = InnerControl.Selection.Interval;
			if (newSelectionInterval.Start >= controlSelectionInterval.Start) {
				if (newSelectionInterval.Start < controlSelectionInterval.End)
					return InnerControl.Selection.Forward;
				return true;
			}
			return false;
		}
		protected internal virtual TimeInterval CalcExtendedSelectionInterval() {
			TimeInterval controlSelectionInterval = InnerControl.Selection.Interval;
			bool oldForward = InnerControl.Selection.Forward;
			if (CalcNewForward(newSelectionInterval)) {
				if (oldForward)
					return new TimeInterval(controlSelectionInterval.Start, newSelectionInterval.End);
				else {
					return new TimeInterval(Scale.GetPrevDate(controlSelectionInterval.End), newSelectionInterval.End);
				}
			} else {
				if (oldForward) {
					return new TimeInterval(newSelectionInterval.Start, Scale.GetNextDate(controlSelectionInterval.Start));
				} else
					return new TimeInterval(newSelectionInterval.Start, controlSelectionInterval.End);
			}
		}
		protected internal virtual void ApplyChangesToControl(ChangeActions actions) {
			InnerControl.BeginUpdate();
			try {
				InnerControl.AppointmentSelectionController.ClearSelection();
				InnerControl.AppointmentDependencySelectionController.ClearSelection();
				if (actions != ChangeActions.None)
					InnerControl.ApplyChangesCore(SchedulerControlChangeType.SelectionChanged, actions | ChangeActions.RaiseSelectionChanged);
				else {
					View.UpdateSelection(InnerControl.Selection);
					List<SchedulerControlChangeType> changeTypes = new List<SchedulerControlChangeType>();
					changeTypes.Add(SchedulerControlChangeType.SelectionChanged);
					InnerControl.RaiseAfterApplyChanges(changeTypes, ChangeActions.RaiseSelectionChanged);
					InnerControl.RaiseSelectionChanged();
					InnerControl.RaiseUpdateUI();
				}
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal abstract TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval);
		protected internal abstract TimeSpan CalcVisibleIntervalsOffset();
		protected internal abstract TimeScale CreateScale();
		protected internal virtual int CalcResourceOffset() {
			return 0;
		}
		protected internal virtual void CorrectNewSelectionInterval() {
		}
		protected internal virtual void CorrectVisibleIntervalsOffset() {
			this.visibleIntervalsOffset = TimeSpan.Zero;
		}
	}
	#region WeekViewSelectionCommand (abstract class)
	public abstract class WeekViewSelectionCommand : SelectionCommand {
		protected WeekViewSelectionCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		protected internal new InnerWeekView View { get { return (InnerWeekView)base.View; } }
		#endregion
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected internal virtual bool IsContinuousInnerVisibleIntervals() {
			return true;
		}
		protected internal virtual bool IsContinuousInnerVisibleIntervalsCore() {
			return View.InnerVisibleIntervals.IsContinuous();
		}
	}
	#endregion
	#region WeekViewMoveNextDayCommand
	public class WeekViewMoveNextDayCommand : WeekViewSelectionCommand {
		public WeekViewMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveNextDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + DateTimeHelper.DaySpan, DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.End > View.InnerVisibleIntervals.End)
				return DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewMovePrevDayCommand
	public class WeekViewMovePrevDayCommand : WeekViewSelectionCommand {
		public WeekViewMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMovePrevDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start - DateTimeHelper.DaySpan, DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.Start < View.InnerVisibleIntervals.Start)
				return -DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewMoveFirstDayCommand
	public class WeekViewMoveFirstDayCommand : WeekViewSelectionCommand {
		public WeekViewMoveFirstDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveFirstDay; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			return new TimeInterval(collection.RoundToStart(selectionInterval), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewMoveLastDayCommand
	public class WeekViewMoveLastDayCommand : WeekViewSelectionCommand {
		public WeekViewMoveLastDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveLastDay; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			return new TimeInterval(collection.RoundToEnd(selectionInterval) - DateTimeHelper.DaySpan, DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewMovePageDownCommand
	public class WeekViewMovePageDownCommand : WeekViewSelectionCommand {
		public WeekViewMovePageDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMovePageDown; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + TimeSpan.FromDays(7 * View.WeekCountCore), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (IsContinuousInnerVisibleIntervals())
				return TimeSpan.FromDays(7 * View.WeekCountCore);
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewMovePageUpCommand
	public class WeekViewMovePageUpCommand : WeekViewSelectionCommand {
		public WeekViewMovePageUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMovePageUp; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start - TimeSpan.FromDays(7 * View.WeekCountCore), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (IsContinuousInnerVisibleIntervals())
				return -TimeSpan.FromDays(7 * View.WeekCountCore);
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewExtendNextDayCommand
	public class WeekViewExtendNextDayCommand : WeekViewMoveNextDayCommand {
		public WeekViewExtendNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendNextDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendPrevDayCommand
	public class WeekViewExtendPrevDayCommand : WeekViewMovePrevDayCommand {
		public WeekViewExtendPrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendPrevDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendFirstDayCommand
	public class WeekViewExtendFirstDayCommand : WeekViewMoveFirstDayCommand {
		public WeekViewExtendFirstDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendFirstDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendLastDayCommand
	public class WeekViewExtendLastDayCommand : WeekViewMoveLastDayCommand {
		public WeekViewExtendLastDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendLastDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendPageDownCommand
	public class WeekViewExtendPageDownCommand : WeekViewMovePageDownCommand {
		public WeekViewExtendPageDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendPageDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendPageUpCommand
	public class WeekViewExtendPageUpCommand : WeekViewMovePageUpCommand {
		public WeekViewExtendPageUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendPageUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand (base class)
	public abstract class WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand : WeekViewSelectionCommand {
		protected WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			DateTime newVisibleStart = collection.RoundToStart(new TimeInterval(NewSelectionInterval.Start, TimeSpan.Zero));
			if (collection.Contains(new TimeInterval(newVisibleStart, TimeSpan.Zero)))
				return TimeSpan.Zero;
			else if (collection.Interval.Contains(new TimeInterval(newVisibleStart, TimeSpan.FromTicks(1)))) 
				return TimeSpan.Zero;
			else
				return newVisibleStart - collection.Start;
		}
	}
	#endregion
	#region WeekViewMoveStartOfMonthCommand
	public class WeekViewMoveStartOfMonthCommand : WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand {
		public WeekViewMoveStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveStartOfMonth; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtBegin; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(DateTimeHelper.GetNextBeginOfMonth(selectionInterval.Start), DateTimeHelper.DaySpan);
		}
	}
	#endregion
	#region WeekViewMoveEndOfMonthCommand
	public class WeekViewMoveEndOfMonthCommand : WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand {
		public WeekViewMoveEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveEndOfMonth; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtEnd; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(DateTimeHelper.GetNextEndOfMonth(selectionInterval.Start), DateTimeHelper.DaySpan);
		}
	}
	#endregion
	#region WeekViewMoveRightCommand
	public class WeekViewMoveRightCommand : WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand {
		public WeekViewMoveRightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveRight; } }
		protected internal virtual TimeSpan CalcSelectionOffset(DayOfWeek dayOfWeek) {
			return TimeSpan.FromDays(dayOfWeek > DayOfWeek.Wednesday ? 4 : 3);
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeSpan offset = CalcSelectionOffset(selectionInterval.Start.DayOfWeek);
			return new TimeInterval(selectionInterval.Start + offset, DateTimeHelper.DaySpan);
		}
	}
	#endregion
	#region WeekViewMoveLeftCommand
	public class WeekViewMoveLeftCommand : WeekViewMoveVisibleIntervalsOffsetByNewSelectionCommand {
		public WeekViewMoveLeftCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewMoveLeft; } }
		protected internal virtual TimeSpan CalcSelectionOffset(DayOfWeek dayOfWeek) {
			return TimeSpan.FromDays(dayOfWeek <= DayOfWeek.Wednesday ? -4 : -3);
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeSpan offset = CalcSelectionOffset(selectionInterval.Start.DayOfWeek);
			return new TimeInterval(selectionInterval.Start + offset, DateTimeHelper.DaySpan);
		}
	}
	#endregion
	#region WeekViewGroupByResourceMoveRightCommand
	public class WeekViewGroupByResourceMoveRightCommand : WeekViewMoveRightCommand {
		public WeekViewGroupByResourceMoveRightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByResourceMoveRight; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return 1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			NewSelectionInterval.Start -= DateTimeHelper.WeekSpan;
		}
	}
	#endregion
	#region WeekViewGroupByResourceMoveLeftCommand
	public class WeekViewGroupByResourceMoveLeftCommand : WeekViewMoveLeftCommand {
		public WeekViewGroupByResourceMoveLeftCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByResourceMoveLeft; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return -1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			NewSelectionInterval.Start += DateTimeHelper.WeekSpan;
		}
	}
	#endregion
	#region WeekViewGroupByDateMoveDownCommand
	public class WeekViewGroupByDateMoveDownCommand : WeekViewSelectionCommand {
		public WeekViewGroupByDateMoveDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByDateMoveDown; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return 1;
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveForward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.End > View.InnerVisibleIntervals.End)
				return DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
		protected internal override void CorrectNewSelectionInterval() {
			NewSelectionInterval.Start -= View.InnerVisibleIntervals.Duration;
		}
	}
	#endregion
	#region WeekViewGroupByDateMoveUpCommand
	public class WeekViewGroupByDateMoveUpCommand : WeekViewSelectionCommand {
		public WeekViewGroupByDateMoveUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByDateMoveUp; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return -1;
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveBackward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.Start < View.InnerVisibleIntervals.Start)
				return -DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
		protected internal override void CorrectNewSelectionInterval() {
			NewSelectionInterval.Start += View.InnerVisibleIntervals.Duration;
		}
	}
	#endregion
	#region WeekViewGroupByDateExtendDownCommand
	public class WeekViewGroupByDateExtendDownCommand : WeekViewSelectionCommand {
		public WeekViewGroupByDateExtendDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByDateExtendDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveForward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.End > View.InnerVisibleIntervals.End)
				return DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewGroupByDateExtendUpCommand
	public class WeekViewGroupByDateExtendUpCommand : WeekViewSelectionCommand {
		public WeekViewGroupByDateExtendUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewGroupByDateExtendUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveBackward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.Start < View.InnerVisibleIntervals.Start)
				return -DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region WeekViewExtendStartOfMonthCommand
	public class WeekViewExtendStartOfMonthCommand : WeekViewMoveStartOfMonthCommand {
		public WeekViewExtendStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendStartOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendEndOfMonthCommand
	public class WeekViewExtendEndOfMonthCommand : WeekViewMoveEndOfMonthCommand {
		public WeekViewExtendEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendEndOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendRightCommand
	public class WeekViewExtendRightCommand : WeekViewMoveRightCommand {
		public WeekViewExtendRightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendRight; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WeekViewExtendLeftCommand
	public class WeekViewExtendLeftCommand : WeekViewMoveLeftCommand {
		public WeekViewExtendLeftCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WeekViewExtendLeft; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region ShowWeekendSelectionHelper (helper class)
	public static class ShowWeekendSelectionHelper {
		public static void ApplyShowWeekendMoveForward(TimeInterval newSelectionInterval, InnerMonthView view) {
			if (!view.ShowWeekend) {
				if (newSelectionInterval.Start.DayOfWeek == DayOfWeek.Saturday)
					newSelectionInterval.Start += TimeSpan.FromDays(2);
				else if (newSelectionInterval.Start.DayOfWeek == DayOfWeek.Sunday)
					newSelectionInterval.Start += TimeSpan.FromDays(1);
			}
		}
		public static void ApplyShowWeekendMoveBackward(TimeInterval newSelectionInterval, InnerMonthView view) {
			if (!view.ShowWeekend) {
				if (newSelectionInterval.Start.DayOfWeek == DayOfWeek.Sunday)
					newSelectionInterval.Start -= TimeSpan.FromDays(2);
				else if (newSelectionInterval.Start.DayOfWeek == DayOfWeek.Saturday)
					newSelectionInterval.Start -= TimeSpan.FromDays(1);
			}
		}
	}
	#endregion
	#region HorizontalWeekMoveVerticallyHelper
	public static class HorizontalWeekMoveVerticallyHelper {
		public static TimeSpan CalcSelectionOffsetMoveForward(DateTime selectionStart, InnerWeekView view) {
			if (view.CompressWeekendInternal) {
				DayOfWeek dayOfWeek = selectionStart.DayOfWeek;
				if (dayOfWeek == DayOfWeek.Saturday)
					return DateTimeHelper.DaySpan;
				else if (dayOfWeek == DayOfWeek.Sunday)
					return TimeSpan.FromDays(6);
				else
					return DateTimeHelper.WeekSpan;
			} else
				return DateTimeHelper.WeekSpan;
		}
		public static TimeSpan CalcSelectionOffsetMoveBackward(DateTime selectionStart, InnerWeekView view) {
			if (view.CompressWeekendInternal) {
				DayOfWeek dayOfWeek = selectionStart.DayOfWeek;
				if (dayOfWeek == DayOfWeek.Saturday)
					return -TimeSpan.FromDays(6);
				else if (dayOfWeek == DayOfWeek.Sunday)
					return -DateTimeHelper.DaySpan;
				else
					return -DateTimeHelper.WeekSpan;
			} else
				return -DateTimeHelper.WeekSpan;
		}
	}
	#endregion
	#region MonthViewMoveNextDayCommand
	public class MonthViewMoveNextDayCommand : WeekViewMoveNextDayCommand {
		public MonthViewMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveNextDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveForward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMovePrevDayCommand
	public class MonthViewMovePrevDayCommand : WeekViewMovePrevDayCommand {
		public MonthViewMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMovePrevDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveBackward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMoveFirstDayCommand
	public class MonthViewMoveFirstDayCommand : WeekViewMoveFirstDayCommand {
		public MonthViewMoveFirstDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveFirstDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveForward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMoveLastDayCommand
	public class MonthViewMoveLastDayCommand : WeekViewMoveLastDayCommand {
		public MonthViewMoveLastDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveLastDay; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveBackward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMoveStartOfMonthCommand
	public class MonthViewMoveStartOfMonthCommand : WeekViewMoveStartOfMonthCommand {
		public MonthViewMoveStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveStartOfMonth; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveBackward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMoveEndOfMonthCommand
	public class MonthViewMoveEndOfMonthCommand : WeekViewMoveEndOfMonthCommand {
		public MonthViewMoveEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveEndOfMonth; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveForward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
	}
	#endregion
	#region MonthViewMovePageDownCommand
	public class MonthViewMovePageDownCommand : WeekViewMovePageDownCommand {
		public MonthViewMovePageDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMovePageDown; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtEnd; } }
		protected internal override bool IsContinuousInnerVisibleIntervals() {
			return IsContinuousInnerVisibleIntervalsCore();
		}
	}
	#endregion
	#region MonthViewMovePageUpCommand
	public class MonthViewMovePageUpCommand : WeekViewMovePageUpCommand {
		public MonthViewMovePageUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMovePageUp; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtBegin; } }
		protected internal override bool IsContinuousInnerVisibleIntervals() {
			return IsContinuousInnerVisibleIntervalsCore();
		}
	}
	#endregion
	#region MonthViewMoveDownCommand
	public class MonthViewMoveDownCommand : WeekViewSelectionCommand {
		public MonthViewMoveDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveDown; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveForward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.End > View.InnerVisibleIntervals.End)
				return DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region MonthViewMoveUpCommand
	public class MonthViewMoveUpCommand : WeekViewSelectionCommand {
		public MonthViewMoveUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewMoveUp; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + HorizontalWeekMoveVerticallyHelper.CalcSelectionOffsetMoveBackward(selectionInterval.Start, View), DateTimeHelper.DaySpan);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			if (NewSelectionInterval.Start < View.InnerVisibleIntervals.Start)
				return -DateTimeHelper.WeekSpan;
			else
				return TimeSpan.Zero;
		}
	}
	#endregion
	#region MonthViewGroupByResourceMoveRightCommand
	public class MonthViewGroupByResourceMoveRightCommand : MonthViewMoveNextDayCommand {
		public MonthViewGroupByResourceMoveRightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewGroupByResourceMoveRight; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveForward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
		protected internal override int CalcResourceOffset() {
			TimeInterval selectionInterval = new TimeInterval(Selection.Interval.Start, DateTimeHelper.DaySpan);
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			TimeInterval currentWeekInterval = new TimeInterval(collection.RoundToStart(selectionInterval), collection.RoundToEnd(selectionInterval));
			if (currentWeekInterval.Contains(NewSelectionInterval))
				return 0;
			else
				return 1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			TimeSpan offset = NewSelectionInterval.Start - collection.RoundToStart(Selection.Interval);
			NewSelectionInterval.Start -= offset;
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveForward(NewSelectionInterval, (InnerMonthView)View);
		}
	}
	#endregion
	#region MonthViewGroupByResourceMoveLeftCommand
	public class MonthViewGroupByResourceMoveLeftCommand : MonthViewMovePrevDayCommand {
		public MonthViewGroupByResourceMoveLeftCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewGroupByResourceMoveLeft; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			TimeInterval newSelectionInterval = base.CalcNewSelectionInterval(selectionInterval);
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveBackward(newSelectionInterval, (InnerMonthView)View);
			return newSelectionInterval;
		}
		protected internal override int CalcResourceOffset() {
			TimeInterval selectionInterval = new TimeInterval(Selection.Interval.Start, DateTimeHelper.DaySpan);
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			TimeInterval currentWeekInterval = new TimeInterval(collection.RoundToStart(selectionInterval), collection.RoundToEnd(selectionInterval));
			if (currentWeekInterval.Contains(NewSelectionInterval))
				return 0;
			else
				return -1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			WeekIntervalCollection collection = (WeekIntervalCollection)View.InnerVisibleIntervals;
			TimeSpan offset = NewSelectionInterval.Start - collection.RoundToEnd(Selection.Interval).AddDays(-1);
			NewSelectionInterval.Start -= offset;
			ShowWeekendSelectionHelper.ApplyShowWeekendMoveBackward(NewSelectionInterval, (InnerMonthView)View);
		}
	}
	#endregion
	#region MonthViewGroupByDateMoveDownCommand
	public class MonthViewGroupByDateMoveDownCommand : WeekViewGroupByDateMoveDownCommand {
		public MonthViewGroupByDateMoveDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id {
			get {
				return SchedulerCommandId.MonthViewGroupByDateMoveDown;
			}
		}
	}
	#endregion
	#region MonthViewGroupByDateMoveUpCommand
	public class MonthViewGroupByDateMoveUpCommand : WeekViewGroupByDateMoveUpCommand {
		public MonthViewGroupByDateMoveUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewGroupByDateMoveUp; } }
	}
	#endregion
	#region MonthViewExtendNextDayCommand
	public class MonthViewExtendNextDayCommand : MonthViewMoveNextDayCommand {
		public MonthViewExtendNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendNextDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendPrevDayCommand
	public class MonthViewExtendPrevDayCommand : MonthViewMovePrevDayCommand {
		public MonthViewExtendPrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendPrevDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendFirstDayCommand
	public class MonthViewExtendFirstDayCommand : MonthViewMoveFirstDayCommand {
		public MonthViewExtendFirstDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendFirstDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendLastDayCommand
	public class MonthViewExtendLastDayCommand : MonthViewMoveLastDayCommand {
		public MonthViewExtendLastDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendLastDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendStartOfMonthCommand
	public class MonthViewExtendStartOfMonthCommand : MonthViewMoveStartOfMonthCommand {
		public MonthViewExtendStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendStartOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendEndOfMonthCommand
	public class MonthViewExtendEndOfMonthCommand : MonthViewMoveEndOfMonthCommand {
		public MonthViewExtendEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendEndOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendPageDownCommand
	public class MonthViewExtendPageDownCommand : MonthViewMovePageDownCommand {
		public MonthViewExtendPageDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendPageDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendPageUpCommand
	public class MonthViewExtendPageUpCommand : MonthViewMovePageUpCommand {
		public MonthViewExtendPageUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendPageUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendDownCommand
	public class MonthViewExtendDownCommand : MonthViewMoveDownCommand {
		public MonthViewExtendDownCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region MonthViewExtendUpCommand
	public class MonthViewExtendUpCommand : MonthViewMoveUpCommand {
		public MonthViewExtendUpCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MonthViewExtendUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewSelectionCommand (abstract class)
	public abstract class DayViewSelectionCommand : SelectionCommand {
		protected DayViewSelectionCommand(ISchedulerCommandTarget target)
			: base(target) {
			View.Changed += OnViewChanged;
		}
		void OnViewChanged(object sender, SchedulerControlStateChangedEventArgs e) {
			DayViewTimeScale dayVIewTimeScale = Scale as DayViewTimeScale;
			if (dayVIewTimeScale == null)
				return;
			if (e.Change == SchedulerControlChangeType.VisibleIntervalsChanged) {
				dayVIewTimeScale.TimeOfDayStart = View.VisibleTime.Start;
			}
		}
		#region Properties
		protected internal new InnerDayView View { get { return (InnerDayView)base.View; } }
		#endregion
		protected internal override TimeScale CreateScale() {
			return new DayViewTimeScale(View.TimeScale, View.VisibleTime.Start);
		}
	}
	#endregion
	#region DayViewMoveSelectionHorizontallyCommand (abstract class)
	public abstract class DayViewMoveSelectionHorizontallyCommand : DayViewSelectionCommand {
		protected DayViewMoveSelectionHorizontallyCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override bool KeepSelectionDuration { get { return true; } }
		protected internal override TimeScale CreateScale() {
			TimeInterval selectionInterval = InnerControl.Selection.Interval;
			if (DateTimeHelper.Floor(selectionInterval.Start, DateTimeHelper.DaySpan) == selectionInterval.Start &&
				DateTimeHelper.Ceil(selectionInterval.End, DateTimeHelper.DaySpan) == selectionInterval.End)
				return new TimeScaleDay();
			else
				return base.CreateScale();
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			TimeInterval interval = new TimeInterval(CalcActualSelectionStartDate(NewSelectionInterval), DateTimeHelper.DaySpan);
			if (View.InnerVisibleIntervals.Contains(interval))
				return TimeSpan.Zero;
			else if (interval.Start < View.InnerVisibleIntervals.Start)
				return interval.Start - View.InnerVisibleIntervals.Start;
			else if (interval.End > View.InnerVisibleIntervals.End)
				return interval.End - View.InnerVisibleIntervals.End;
			else
				return TimeSpan.Zero;
		}
		protected internal virtual DateTime CalcActualSelectionStartDate(TimeInterval selectionInterval) {
			if (IsAllDayAreaInterval(selectionInterval))
				return selectionInterval.Start.Date;
			else
				return View.CalculateActualDate(selectionInterval.Start);
		}
		protected internal virtual bool IsAllDayAreaInterval(TimeInterval selectionInterval) {
			return selectionInterval.Duration.Ticks > 0 && selectionInterval.Start == selectionInterval.Start.Date && selectionInterval.End == selectionInterval.End.Date;
		}
		protected internal override bool IsViewVisibleIntervalsContainInterval(TimeInterval interval) {
			if (IsAllDayAreaInterval(interval))
				return base.IsViewVisibleIntervalsContainInterval(interval);
			else {
				TimeInterval adjustedInterval = new TimeInterval(interval.Start, interval.Duration);
				adjustedInterval.Start -= View.ActualVisibleTime.Start;
				return base.IsViewVisibleIntervalsContainInterval(adjustedInterval);
			}
		}
		protected internal override ChangeActions ApplyFirstVisibleResourceOffset() {
			ChangeActions actions = base.ApplyFirstVisibleResourceOffset();
			if (actions != ChangeActions.None)
				actions |= ChangeActions.UpdateDateTimeScrollBarValue;
			return actions;
		}
	}
	#endregion
	#region DayViewMoveSelectionLeftRightCommand (abstract class)
	public abstract class DayViewMoveSelectionLeftRightCommand : DayViewMoveSelectionHorizontallyCommand {
		#region Fields
		int currentDayIndex;
		#endregion
		protected DayViewMoveSelectionLeftRightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		protected internal int CurrentDayIndex { get { return currentDayIndex; } set { currentDayIndex = value; } }
		#endregion
		protected internal override VisibleIntervalsChangeType ExecuteSelectionCommandCore() {
			this.currentDayIndex = CalcCurrentDayIndex();
			XtraSchedulerDebug.Assert(currentDayIndex >= 0);
			return base.ExecuteSelectionCommandCore();
		}
		protected internal virtual int CalcCurrentDayIndex() {
			TimeInterval interval = new TimeInterval(CalcActualSelectionStartDate(Selection.Interval), DateTimeHelper.DaySpan);
			return View.InnerVisibleIntervals.IndexOf(interval);
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime date = selectionInterval.Start + CalcNewSelectionDayOffset();
			return new TimeInterval(date, Scale.GetNextDate(date));
		}
		protected internal virtual TimeSpan CalcNewSelectionDayOffset() {
			int dayIndexOffset = CalcDayIndexOffset();
			int newDayIndex = currentDayIndex + dayIndexOffset;
			DayIntervalCollection collection = (DayIntervalCollection)View.InnerVisibleIntervals;
			int count = collection.Count;
			if (newDayIndex < 0)
				return CalcNewSelectionDayOffsetMoveOutsideLeft();
			else if (newDayIndex >= count)
				return CalcNewSelectionDayOffsetMoveOutsideRight();
			else
				return collection[newDayIndex].Start - collection[currentDayIndex].Start;
		}
		protected internal virtual TimeSpan CalcNewSelectionDayOffsetMoveOutsideLeft() {
			return -DateTimeHelper.DaySpan;
		}
		protected internal virtual TimeSpan CalcNewSelectionDayOffsetMoveOutsideRight() {
			return DateTimeHelper.DaySpan;
		}
		protected internal abstract int CalcDayIndexOffset();
	}
	#endregion
	#region DayViewMoveNextDayCommand
	public class DayViewMoveNextDayCommand : DayViewMoveSelectionLeftRightCommand {
		public DayViewMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveNextDay; } }
		protected internal override int CalcDayIndexOffset() {
			return 1;
		}
	}
	#endregion
	#region DayViewMovePrevDayCommand
	public class DayViewMovePrevDayCommand : DayViewMoveSelectionLeftRightCommand {
		public DayViewMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMovePrevDay; } }
		protected internal override int CalcDayIndexOffset() {
			return -1;
		}
	}
	#endregion
	#region DayViewMoveNextWeekCommand
	public class DayViewMoveNextWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMoveNextWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveNextWeek; } }
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.FromDays(7);
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start.AddDays(7), selectionInterval.Duration);
		}
	}
	#endregion
	#region DayViewMovePrevWeekCommand
	public class DayViewMovePrevWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMovePrevWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMovePrevWeek; } }
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.FromDays(-7);
		}
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start.AddDays(-7), selectionInterval.Duration);
		}
	}
	#endregion
	#region DayViewMoveFirstDayOfWeekCommand
	public class DayViewMoveFirstDayOfWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMoveFirstDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveFirstDayOfWeek; } }
		protected internal virtual DayOfWeek FirstDayOfWeek { get { return InnerControl.FirstDayOfWeek; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtBegin; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime firstDayOfWeek = DateTimeHelper.GetStartOfWeekUI(currentSelectionDate, FirstDayOfWeek);
			TimeSpan offset = firstDayOfWeek - currentSelectionDate;
			DateTime date = selectionInterval.Start + offset;
			return new TimeInterval(date, Scale.GetNextDate(date));
		}
	}
	#endregion
	#region DayViewMoveLastDayOfWeekCommand
	public class DayViewMoveLastDayOfWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMoveLastDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveLastDayOfWeek; } }
		protected internal virtual DayOfWeek FirstDayOfWeek { get { return InnerControl.FirstDayOfWeek; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtEnd; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime lastDayOfWeek = DateTimeHelper.GetStartOfWeekUI(currentSelectionDate, FirstDayOfWeek);
			lastDayOfWeek += TimeSpan.FromDays(6);
			TimeSpan offset = lastDayOfWeek - currentSelectionDate;
			DateTime date = selectionInterval.Start + offset;
			return new TimeInterval(date, Scale.GetNextDate(date));
		}
	}
	#endregion
	#region DayViewMoveStartOfMonthCommand
	public class DayViewMoveStartOfMonthCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMoveStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveStartOfMonth; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtBegin; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime firstDayOfWeek = DateTimeHelper.GetNextBeginOfMonth(currentSelectionDate);
			TimeSpan offset = firstDayOfWeek - currentSelectionDate;
			DateTime date = selectionInterval.Start + offset;
			return new TimeInterval(date, Scale.GetNextDate(date));
		}
	}
	#endregion
	#region DayViewMoveEndOfMonthCommand
	public class DayViewMoveEndOfMonthCommand : DayViewMoveSelectionHorizontallyCommand {
		public DayViewMoveEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveEndOfMonth; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.CreateWithSelectionAtEnd; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime firstDayOfWeek = DateTimeHelper.GetNextEndOfMonth(currentSelectionDate);
			TimeSpan offset = firstDayOfWeek - currentSelectionDate;
			DateTime date = selectionInterval.Start + offset;
			return new TimeInterval(date, Scale.GetNextDate(date));
		}
	}
	#endregion
	#region DayViewExtendNextDayCommand
	public class DayViewExtendNextDayCommand : DayViewMoveNextDayCommand {
		public DayViewExtendNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendNextDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendPrevDayCommand
	public class DayViewExtendPrevDayCommand : DayViewMovePrevDayCommand {
		public DayViewExtendPrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendPrevDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendFirstDayOfWeekCommand
	public class DayViewExtendFirstDayOfWeekCommand : DayViewMoveFirstDayOfWeekCommand {
		public DayViewExtendFirstDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendFirstDayOfWeek; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendLastDayOfWeekCommand
	public class DayViewExtendLastDayOfWeekCommand : DayViewMoveLastDayOfWeekCommand {
		public DayViewExtendLastDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendLastDayOfWeek; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendStartOfMonthCommand
	public class DayViewExtendStartOfMonthCommand : DayViewMoveStartOfMonthCommand {
		public DayViewExtendStartOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendStartOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendEndOfMonthCommand
	public class DayViewExtendEndOfMonthCommand : DayViewMoveEndOfMonthCommand {
		public DayViewExtendEndOfMonthCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendEndOfMonth; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewGroupByResourceMoveNextDayCommand
	public class DayViewGroupByResourceMoveNextDayCommand : DayViewMoveNextDayCommand {
		public DayViewGroupByResourceMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewGroupByResourceMoveNextDay; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return 1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			TimeSpan offset = View.InnerVisibleIntervals[0].Start.Date - CalcActualSelectionStartDate(NewSelectionInterval);
			NewSelectionInterval.Start += offset;
		}
	}
	#endregion
	#region DayViewGroupByResourceMovePrevDayCommand
	public class DayViewGroupByResourceMovePrevDayCommand : DayViewMovePrevDayCommand {
		public DayViewGroupByResourceMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewGroupByResourceMovePrevDay; } }
		protected internal override int CalcResourceOffset() {
			if (View.InnerVisibleIntervals.Contains(NewSelectionInterval))
				return 0;
			else
				return -1;
		}
		protected internal override void CorrectNewSelectionInterval() {
			int count = View.InnerVisibleIntervals.Count;
			TimeSpan offset = View.InnerVisibleIntervals[count - 1].Start.Date - CalcActualSelectionStartDate(NewSelectionInterval);
			NewSelectionInterval.Start += offset;
		}
	}
	#endregion
	#region DayViewGroupByDateMoveNextDayCommand
	public class DayViewGroupByDateMoveNextDayCommand : DayViewMoveNextDayCommand {
		public DayViewGroupByDateMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewGroupByDateMoveNextDay; } }
		protected internal override int CalcResourceOffset() {
			return 1;
		}
		protected internal override int CalcDayIndexOffset() {
			if (CurrentFilteredResourceIndex == FilteredResourceCount - 1)
				return base.CalcDayIndexOffset();
			else
				return 0;
		}
		protected internal override int CorrectResourceOffsetScrollForwardOutsideRange() {
			return -CurrentFilteredResourceIndex;
		}
		protected internal override int CalcFirstVisibleResourceOffsetScrollForwardOutsideRange() {
			return -View.ActualFirstVisibleResourceIndex;
		}
	}
	#endregion
	#region DayViewGroupByDateMovePrevDayCommand
	public class DayViewGroupByDateMovePrevDayCommand : DayViewMovePrevDayCommand {
		public DayViewGroupByDateMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewGroupByDateMovePrevDay; } }
		protected internal override int CalcResourceOffset() {
			return -1;
		}
		protected internal override int CalcDayIndexOffset() {
			if (CurrentFilteredResourceIndex == 0)
				return base.CalcDayIndexOffset();
			else
				return 0;
		}
		protected internal override int CorrectResourceOffsetScrollBackwardOutsideRange() {
			return (FilteredResourceCount - 1) - CurrentFilteredResourceIndex;
		}
		protected internal override int CalcFirstVisibleResourceOffsetScrollBackwardOutsideRange() {
			return (FilteredResourceCount - View.ActualResourcesPerPage) - View.ActualFirstVisibleResourceIndex;
		}
	}
	#endregion
	#region Helper class
	public static class WorkWeekViewShiftHelper {
		internal static ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals, TimeSpan visibleIntervalsOffset) {
			if (visibleIntervalsOffset != TimeSpan.Zero) {
				TimeIntervalCollection simpleIntervals = new TimeIntervalCollection();
				simpleIntervals.AddRange(intervals);
				simpleIntervals.Shift(visibleIntervalsOffset);
				intervals.SetContent(simpleIntervals);
				return ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue | ChangeActions.RaiseVisibleIntervalChanged | ChangeActions.RecalcPreliminaryLayout;
			} else
				return ChangeActions.None;
		}
	}
	#endregion
	#region WorkWeekViewMoveNextDayCommand
	public class WorkWeekViewMoveNextDayCommand : DayViewMoveNextDayCommand {
		public WorkWeekViewMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveNextDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideRight() {
			return DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewMovePrevDayCommand
	public class WorkWeekViewMovePrevDayCommand : DayViewMovePrevDayCommand {
		public WorkWeekViewMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMovePrevDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideLeft() {
			return -DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewMoveNextWeekCommand
	public class WorkWeekViewMoveNextWeekCommand : DayViewMoveNextWeekCommand {
		public WorkWeekViewMoveNextWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveNextWeek; } }
	}
	#endregion
	#region WorkWeekViewMovePrevWeekCommand
	public class WorkWeekViewMovePrevWeekCommand : DayViewMovePrevWeekCommand {
		public WorkWeekViewMovePrevWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMovePrevWeek; } }
	}
	#endregion
	#region WorkWeekViewMoveFirstDayOfWeekCommand
	public class WorkWeekViewMoveFirstDayOfWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public WorkWeekViewMoveFirstDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveFirstDayOfWeek; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime firstDayOfWeek = View.InnerVisibleIntervals[0].Start;
			TimeSpan offset = firstDayOfWeek - currentSelectionDate;
			return new TimeInterval(selectionInterval.Start + offset, selectionInterval.Duration);
		}
	}
	#endregion
	#region WorkWeekViewMoveLastDayOfWeekCommand
	public class WorkWeekViewMoveLastDayOfWeekCommand : DayViewMoveSelectionHorizontallyCommand {
		public WorkWeekViewMoveLastDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveLastDayOfWeek; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime currentSelectionDate = CalcActualSelectionStartDate(selectionInterval);
			DateTime lastDayOfWeek = View.InnerVisibleIntervals[View.InnerVisibleIntervals.Count - 1].Start;
			TimeSpan offset = lastDayOfWeek - currentSelectionDate;
			return new TimeInterval(selectionInterval.Start + offset, selectionInterval.Duration);
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceMoveNextDayCommand
	public class WorkWeekViewGroupByResourceMoveNextDayCommand : DayViewGroupByResourceMoveNextDayCommand {
		public WorkWeekViewGroupByResourceMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewGroupByResourceMoveNextDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideRight() {
			return DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewGroupByResourceMovePrevDayCommand
	public class WorkWeekViewGroupByResourceMovePrevDayCommand : DayViewGroupByResourceMovePrevDayCommand {
		public WorkWeekViewGroupByResourceMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewGroupByResourceMovePrevDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideLeft() {
			return -DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateMoveNextDayCommand
	public class WorkWeekViewGroupByDateMoveNextDayCommand : DayViewGroupByDateMoveNextDayCommand {
		public WorkWeekViewGroupByDateMoveNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewGroupByDateMoveNextDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideRight() {
			return DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewGroupByDateMovePrevDayCommand
	public class WorkWeekViewGroupByDateMovePrevDayCommand : DayViewGroupByDateMovePrevDayCommand {
		public WorkWeekViewGroupByDateMovePrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewGroupByDateMovePrevDay; } }
		protected internal override TimeSpan CalcNewSelectionDayOffsetMoveOutsideLeft() {
			return -DateTimeHelper.WeekSpan;
		}
		protected internal override ChangeActions ShiftVisibleIntervals(TimeIntervalCollection intervals) {
			return WorkWeekViewShiftHelper.ShiftVisibleIntervals(intervals, VisibleIntervalsOffset);
		}
	}
	#endregion
	#region WorkWeekViewExtendNextDayCommand
	public class WorkWeekViewExtendNextDayCommand : WorkWeekViewMoveNextDayCommand {
		public WorkWeekViewExtendNextDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendNextDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendPrevDayCommand
	public class WorkWeekViewExtendPrevDayCommand : WorkWeekViewMovePrevDayCommand {
		public WorkWeekViewExtendPrevDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendPrevDay; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendFirstDayOfWeekCommand
	public class WorkWeekViewExtendFirstDayOfWeekCommand : WorkWeekViewMoveFirstDayOfWeekCommand {
		public WorkWeekViewExtendFirstDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendFirstDayOfWeek; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendLastDayOfWeekCommand
	public class WorkWeekViewExtendLastDayOfWeekCommand : WorkWeekViewMoveLastDayOfWeekCommand {
		public WorkWeekViewExtendLastDayOfWeekCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendLastDayOfWeek; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region TimelineViewSelectionCommand
	public abstract class TimelineViewSelectionCommand : SelectionCommand {
		protected TimelineViewSelectionCommand(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		protected internal new InnerTimelineView View { get { return (InnerTimelineView)base.View; } }
		#endregion
		protected internal override TimeScale CreateScale() {
			return View.GetBaseTimeScale();
		}
		protected DateTime GetNextDate(DateTime date) {
			return Scale.GetNextDate(date);  
		}
		protected DateTime GetPrevDate(DateTime date) {
			return Scale.GetPrevDate(date);
		}
		protected DateTime Floor(DateTime date) {
			return Scale.Floor(date);
		}
		protected internal override SchedulerViewSelection CalcCurrentSelection(SchedulerViewSelection controlSelection) {
			SchedulerViewSelection result = controlSelection.Clone();
			if (result.Forward)
				result.Interval = new TimeInterval(GetPrevDate(result.Interval.End), result.Interval.End);
			else
				result.Interval = new TimeInterval(result.Interval.Start, GetNextDate(result.Interval.Start));
			result.FirstSelectedInterval = result.Interval.Clone();
			return result;
		}
	}
	#endregion
	#region TimelineViewMoveSelectionHorizontallyCommand
	public abstract class TimelineViewMoveSelectionHorizontallyCommand : TimelineViewSelectionCommand {
		protected TimelineViewMoveSelectionHorizontallyCommand(InnerSchedulerControl control)
			: base(control) {
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			int offset = CalculateSelectionOffsetInCells();
			if (offset == 0)
				return TimeSpan.Zero;
			DateTime newStart = View.InnerVisibleIntervals.Start;
			int count = Math.Abs(offset);
			for (int i = 0; i < count; i++) {
				if (offset < 0)
					newStart = GetPrevDate(newStart);
				else
					newStart = GetNextDate(newStart);
			}
			return newStart - View.InnerVisibleIntervals.Start;
		}
		protected internal virtual int CalculateSelectionOffsetInCells() {
			if (NewSelectionInterval.Start < View.InnerVisibleIntervals.Start)
				return -CalculateSelectionOffsetInCellsBackward();
			else if (NewSelectionInterval.Start >= View.InnerVisibleIntervals.End)
				return CalculateSelectionOffsetInCellsForward();
			return 0;
		}
		protected internal virtual int CalculateSelectionOffsetInCellsForward() {
			int count = 0;
			DateTime date = View.InnerVisibleIntervals.End;
			DateTime maxValidDate = Floor(DateTime.MaxValue);
			while (date < NewSelectionInterval.End) {
				if (date == maxValidDate)
					break;
				count++;
				date = GetNextDate(date);
			}
			return count;
		}
		protected internal virtual int CalculateSelectionOffsetInCellsBackward() {
			int count = 0;
			DateTime date = View.InnerVisibleIntervals.Start;
			while (NewSelectionInterval.Start < date) {
				count++;
				date = GetPrevDate(date);
			}
			return count;
		}
	}
	#endregion
	#region TimelineViewMoveNextDayCommand
	public class TimelineViewMoveNextDateCommand : TimelineViewMoveSelectionHorizontallyCommand {
		public TimelineViewMoveNextDateCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMoveNextDate; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime end = selectionInterval.End;
			DateTime maxValidDate = Floor(DateTime.MaxValue);
			if (end == maxValidDate)
				return selectionInterval.Clone();
			return new TimeInterval(end, GetNextDate(end));
		}
	}
	#endregion
	#region TimelineViewMovePrevDayCommand
	public class TimelineViewMovePrevDateCommand : TimelineViewMoveSelectionHorizontallyCommand {
		public TimelineViewMovePrevDateCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMovePrevDate; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime start = selectionInterval.Start;
			DateTime minValidDate = Floor(DateTime.MinValue); 
			if (start == minValidDate)
				return selectionInterval.Clone();
			return new TimeInterval(GetPrevDate(start), start);
		}
	}
	#endregion
	#region TimelineViewExtendNextDateCommand
	public class TimelineViewExtendNextDateCommand : TimelineViewMoveNextDateCommand {
		public TimelineViewExtendNextDateCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewExtendNextDate; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region TimelineViewExtendPrevDateCommand
	public class TimelineViewExtendPrevDateCommand : TimelineViewMovePrevDateCommand {
		public TimelineViewExtendPrevDateCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewExtendPrevDate; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region TimelineViewGroupByDateMoveSelectionVerticallyCommand
	public abstract class TimelineViewGroupByDateMoveSelectionVerticallyCommand : TimelineViewSelectionCommand {
		protected TimelineViewGroupByDateMoveSelectionVerticallyCommand(InnerSchedulerControl control)
			: base(control) {
		}
		protected internal override bool KeepSelectionDuration { get { return true; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return selectionInterval.Clone();
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
	}
	#endregion
	#region TimelineViewGroupByDateMoveUpCommand
	public class TimelineViewGroupByDateMoveUpCommand : TimelineViewGroupByDateMoveSelectionVerticallyCommand {
		public TimelineViewGroupByDateMoveUpCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewGroupByDateMoveUp; } }
		protected internal override int CalcResourceOffset() {
			return -1;
		}
	}
	#endregion
	#region TimelineViewGroupByDateMoveDownCommand
	public class TimelineViewGroupByDateMoveDownCommand : TimelineViewGroupByDateMoveSelectionVerticallyCommand {
		public TimelineViewGroupByDateMoveDownCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewGroupByDateMoveDown; } }
		protected internal override int CalcResourceOffset() {
			return 1;
		}
	}
	#endregion
	#region TimelineViewMoveToStartOfVisibleTimeCommand
	public class TimelineViewMoveToStartOfVisibleTimeCommand : TimelineViewSelectionCommand {
		public TimelineViewMoveToStartOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMoveToStartOfVisibleTime; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime start = View.InnerVisibleIntervals.Start;
			return new TimeInterval(start, GetNextDate(start));
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
	}
	#endregion
	#region TimelineViewMoveToEndOfVisibleTimeCommand
	public class TimelineViewMoveToEndOfVisibleTimeCommand : TimelineViewSelectionCommand {
		public TimelineViewMoveToEndOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMoveToEndOfVisibleTime; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime end = View.InnerVisibleIntervals.End;
			return new TimeInterval(GetPrevDate(end), end);
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
	}
	#endregion
	#region TimelineViewMoveToMajorStartCommand
	public class TimelineViewMoveToMajorStartCommand : TimelineViewMoveSelectionHorizontallyCommand {
		public TimelineViewMoveToMajorStartCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMoveToMajorStart; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			DateTime date = selectionInterval.Start;
			int count = View.ActualScales.Count;
			TimeScale majorScale = count > 1 ? View.ActualScales[count - 2] : Scale;
			DateTime majorStart = majorScale.Floor(date);
			DateTime start = Floor(majorStart);
			if (start == date) {
				majorStart = majorScale.GetPrevDate(majorStart);
				start = Floor(majorStart);
			}
			return new TimeInterval(start, GetNextDate(start));
		}
	}
	#endregion
	#region TimelineViewMoveToMajorEndCommand
	public class TimelineViewMoveToMajorEndCommand : TimelineViewMoveSelectionHorizontallyCommand {
		public TimelineViewMoveToMajorEndCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.TimelineViewMoveToMajorEnd; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			int count = View.ActualScales.Count;
			TimeScale majorScale = count > 1 ? View.ActualScales[count - 2] : Scale;
			DateTime date = selectionInterval.End;
			DateTime majorStart = majorScale.Floor(date);
			DateTime majorEnd = majorScale.GetNextDate(majorStart);
			if (majorStart == majorEnd)  
				return selectionInterval.Clone();
			DateTime end = Floor(majorEnd);
			if (end < majorEnd)
				return new TimeInterval(end, GetNextDate(end));
			else
				return new TimeInterval(GetPrevDate(end), end);
		}
	}
	#endregion
	public class SetSelectionCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		TimeInterval interval;
		Resource resource;
		#endregion
		public SetSelectionCommand(ISchedulerCommandTarget target, TimeInterval interval, Resource resource)
			: base(target) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Abbr_Day; } } 
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Abbr_Day; } } 
		protected internal TimeInterval Interval { get { return interval; } }
		protected internal Resource Resource { get { return resource; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected internal override void ExecuteCore() {
			InnerControl.BeginUpdate();
			try {
				PerformSetSelection(interval, resource);
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal virtual void PerformSetSelection(TimeInterval interval, Resource resource) {
			InnerSchedulerViewBase view = InnerControl.ActiveView;
			if (ShouldChangeDate(interval)) {
				IDateTimeNavigationService service = (IDateTimeNavigationService)InnerControl.GetService(typeof(IDateTimeNavigationService));
				if (service != null)
					service.GoToDate(interval.Start ); 
			}
			InnerControl.BeginUpdate();
			try {
				if (!interval.LongerThanADay)
					EnsureIntervalVisible(interval);
				SchedulerViewSelection selection = InnerControl.Selection;
				TimeInterval roundedSelectionInterval = view.RoundSelectionInterval(interval);
				selection.Interval = roundedSelectionInterval;
				selection.FirstSelectedInterval = roundedSelectionInterval.Clone();
				selection.Resource = resource;
				view.RaiseChanged(SchedulerControlChangeType.SelectionChanged);
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal virtual bool ShouldChangeDate(TimeInterval interval) {
			InnerSchedulerViewBase view = InnerControl.ActiveView;
			InnerDayView dayView = view as InnerDayView;
			TimeIntervalCollection source = view.InnerVisibleIntervals;
			if (dayView != null && dayView.VisibleTime.End > TimeSpan.FromDays(1))
				interval.Start -= dayView.VisibleTime.Start;
			return !source.Contains(new TimeInterval(interval.Start , TimeSpan.Zero));
		}
		protected internal virtual void EnsureIntervalVisible(TimeInterval interval) {
		}
	}
}
