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

using DevExpress.XtraScheduler.Commands;
using System;
namespace DevExpress.XtraScheduler.Native {
	#region DayViewMoveSelectionVerticallyCommand (abstract class)
	public abstract class DayViewMoveSelectionVerticallyCommand : DayViewSelectionCommand {
		#region Fields
		TimeInterval visibleTimeInterval;
		TimeInterval availableTimeInterval;
		TimeInterval workTimeInterval;
		TimeSpan scrollStartTimeOffset;
		#endregion
		protected DayViewMoveSelectionVerticallyCommand(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		protected internal TimeInterval VisibleTimeInterval { get { return visibleTimeInterval; } set { visibleTimeInterval = value; } }
		protected internal TimeInterval WorkTimeInterval { get { return workTimeInterval; } set { workTimeInterval = value; } }
		protected internal TimeInterval AvailableTimeInterval { get { return availableTimeInterval; } set { availableTimeInterval = value; } }
		protected internal TimeSpan ScrollStartTimeOffset { get { return scrollStartTimeOffset; } set { scrollStartTimeOffset = value; } }
		protected internal override VisibleIntervalsChangeType MoveSelectionIntoHoleVisibleIntervalsChangeType { get { return VisibleIntervalsChangeType.None; } }
		#endregion
		protected internal override VisibleIntervalsChangeType ExecuteSelectionCommandCore() {
			CalcDayViewIntervals();
			VisibleIntervalsChangeType result = base.ExecuteSelectionCommandCore();
			ValidateNewSelectionInterval();
			this.scrollStartTimeOffset = CalcScrollStartTimeOffset();
			return result;
		}
		protected internal virtual void CalcDayViewIntervals() {
			TimeSpan currentSelectionDateOffset = CalcCurrentSelectionDateOffset();
			CalcVisibleTimeInterval(currentSelectionDateOffset);
			CalcAvailableTimeInterval(currentSelectionDateOffset);
			CalcWorkTimeInterval(currentSelectionDateOffset);
		}
		protected internal virtual TimeSpan CalcCurrentSelectionDateOffset() {
			return TimeSpan.FromTicks(View.CalculateActualDate(Selection.Interval.Start).Ticks);
		}
		protected internal virtual void CalcVisibleTimeInterval(TimeSpan currentSelectionDateOffset) {
			this.visibleTimeInterval = View.GetVisibleRowsTimeInterval();
			visibleTimeInterval.Start += currentSelectionDateOffset;
		}
		protected internal virtual void CalcAvailableTimeInterval(TimeSpan currentSelectionDateOffset) {
			this.availableTimeInterval = View.GetAvailableRowsTimeInterval();
			availableTimeInterval.Start += currentSelectionDateOffset;
		}
		protected internal virtual void CalcWorkTimeInterval(TimeSpan currentSelectionDateOffset) {
			TimeOfDayIntervalCollection resourceWorkTimeIntervals = View.CalcResourceWorkTimeInterval(availableTimeInterval, Selection.Resource);
			if (resourceWorkTimeIntervals.Count <= 0 || TimeOfDayInterval.Empty.Equals(resourceWorkTimeIntervals[0])) {
				resourceWorkTimeIntervals.Clear();
				resourceWorkTimeIntervals.Add(View.WorkTime.Clone());
			}
			DateTime workIntervalStart = new DateTime(Math.Max(DateTimeHelper.Floor(resourceWorkTimeIntervals.Start, View.TimeScale).Ticks, DateTimeHelper.Floor(View.ActualVisibleTime.Start, View.TimeScale).Ticks));
			DateTime workIntervalEnd = new DateTime(Math.Min(DateTimeHelper.Ceil(resourceWorkTimeIntervals.End, View.TimeScale).Ticks, DateTimeHelper.Ceil(View.ActualVisibleTime.End, View.TimeScale).Ticks));
			if (workIntervalEnd < workIntervalStart)
				this.workTimeInterval = TimeInterval.Empty;
			else
				this.workTimeInterval = new TimeInterval(workIntervalStart, workIntervalEnd);
			workTimeInterval.Start += currentSelectionDateOffset;
		}
		protected internal override ChangeActions ApplyChangesCore(TimeIntervalCollection newInnerVisibleIntervals) {
			ChangeActions actions = base.ApplyChangesCore(newInnerVisibleIntervals);
			actions |= ApplyScrollStartTimeOffset();
			return actions;
		}
		protected internal virtual ChangeActions ApplyScrollStartTimeOffset() {
			if (scrollStartTimeOffset != TimeSpan.Zero) {
				TimeSpan newScrollStartTime = View.GetTopRowTime() + scrollStartTimeOffset;
				TimeSpan minScrollStartTime = DateTimeHelper.Floor(View.ActualVisibleTime.Start, View.TimeScale);
				TimeSpan maxScrollStartTime = minScrollStartTime + availableTimeInterval.Duration - visibleTimeInterval.Duration;
				if (newScrollStartTime < minScrollStartTime)
					newScrollStartTime = minScrollStartTime;
				else if (newScrollStartTime > maxScrollStartTime)
					newScrollStartTime = maxScrollStartTime;
				if (View.GetTopRowTime() != newScrollStartTime) {
					View.SetScrollStartTimeCore(newScrollStartTime);
					return ChangeActions.RecalcViewLayout | ChangeActions.UpdateDateTimeScrollBarValue;
				} else
					return ChangeActions.None;
			} else {
				return ChangeActions.None;
			}
		}
		protected internal virtual void ValidateNewSelectionInterval() {
			if (NewSelectionInterval.Start < availableTimeInterval.Start)
				NewSelectionInterval.Start = availableTimeInterval.Start;
			if (NewSelectionInterval.End > availableTimeInterval.End) {
				NewSelectionInterval.Start = availableTimeInterval.End - View.TimeScale;
				NewSelectionInterval.Duration = View.TimeScale;
			}
		}
		protected internal override TimeSpan CalcVisibleIntervalsOffset() {
			return TimeSpan.Zero;
		}
		protected internal virtual TimeSpan CalcScrollStartTimeOffset() {
			if (VisibleTimeInterval.Contains(NewSelectionInterval))
				return TimeSpan.Zero;
			else {
				if (NewSelectionInterval.Start < VisibleTimeInterval.Start)
					return NewSelectionInterval.Start - VisibleTimeInterval.Start;
				else
					return NewSelectionInterval.End - VisibleTimeInterval.End;
			}
		}
	}
	#endregion
	#region DayViewMovePrevCellCommand
	public class DayViewMovePrevCellCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMovePrevCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override Commands.SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMovePrevCell; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start - View.TimeScale, View.TimeScale);
		}
	}
	#endregion
	#region DayViewMoveNextCellCommand
	public class DayViewMoveNextCellCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMoveNextCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override Commands.SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveNextCell; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + View.TimeScale, View.TimeScale);
		}
	}
	#endregion
	#region DayViewMovePageUpCommand
	public class DayViewMovePageUpCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMovePageUpCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMovePageUp; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start - VisibleTimeInterval.Duration, View.TimeScale);
		}
		protected internal override TimeSpan CalcScrollStartTimeOffset() {
			if (VisibleTimeInterval.Contains(NewSelectionInterval))
				return TimeSpan.Zero;
			else {
				TimeInterval newVisibleTimeInterval = VisibleTimeInterval.Clone();
				newVisibleTimeInterval.Start -= VisibleTimeInterval.Duration;
				if (newVisibleTimeInterval.Contains(NewSelectionInterval))
					return -VisibleTimeInterval.Duration;
				else
					return base.CalcScrollStartTimeOffset();
			}
		}
	}
	#endregion
	#region DayViewMovePageDownCommand
	public class DayViewMovePageDownCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMovePageDownCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMovePageDown; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(selectionInterval.Start + VisibleTimeInterval.Duration, View.TimeScale);
		}
		protected internal override TimeSpan CalcScrollStartTimeOffset() {
			if (VisibleTimeInterval.Contains(NewSelectionInterval))
				return TimeSpan.Zero;
			else {
				TimeInterval newVisibleTimeInterval = VisibleTimeInterval.Clone();
				newVisibleTimeInterval.Start += VisibleTimeInterval.Duration;
				if (newVisibleTimeInterval.Contains(NewSelectionInterval))
					return VisibleTimeInterval.Duration;
				else
					return base.CalcScrollStartTimeOffset();
			}
		}
	}
	#endregion
	#region DayViewMoveToStartOfWorkTimeCommand
	public class DayViewMoveToStartOfWorkTimeCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMoveToStartOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveToStartOfWorkTime; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(WorkTimeInterval.Start, View.TimeScale);
		}
	}
	#endregion
	#region DayViewMoveToEndOfWorkTimeCommand
	public class DayViewMoveToEndOfWorkTimeCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMoveToEndOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveToEndOfWorkTime; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(WorkTimeInterval.End - View.TimeScale, View.TimeScale);
		}
	}
	#endregion
	#region DayViewMoveToStartOfVisibleTimeCommand
	public class DayViewMoveToStartOfVisibleTimeCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMoveToStartOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveToStartOfVisibleTime; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(AvailableTimeInterval.Start, View.TimeScale);
		}
	}
	#endregion
	#region DayViewMoveToEndOfVisibleTimeCommand
	public class DayViewMoveToEndOfVisibleTimeCommand : DayViewMoveSelectionVerticallyCommand {
		public DayViewMoveToEndOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewMoveToEndOfVisibleTime; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			return new TimeInterval(AvailableTimeInterval.End - View.TimeScale, View.TimeScale);
		}
	}
	#endregion
	public class DayViewMoveToTimeCommand : DayViewMoveSelectionVerticallyCommand {
		TimeInterval targetInterval;
		public DayViewMoveToTimeCommand(InnerSchedulerControl control, TimeInterval interval)
			: base(control) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			TimeSpan endOffset;
			if (interval.Duration == TimeSpan.Zero)
				endOffset = TimeSpan.FromTicks(1);
			else
				endOffset = TimeSpan.Zero;
			this.targetInterval = new TimeInterval(Scale.Floor(interval.Start), Scale.Ceil(interval.End + endOffset));
		}
		internal TimeInterval TargetInterval { get { return targetInterval; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override TimeInterval CalcNewSelectionInterval(TimeInterval selectionInterval) {
			this.targetInterval.Start += selectionInterval.Start.Date - this.targetInterval.Start.Date;
			TimeInterval result = targetInterval.Clone();
			if (result.Duration > VisibleTimeInterval.Duration)
				result.Duration = VisibleTimeInterval.Duration;
			if (result.End <= VisibleTimeInterval.End) {
				result.Duration = View.TimeScale;
			} else
				result = new TimeInterval(result.End - View.TimeScale, View.TimeScale);
			return result;
		}
		protected internal override TimeInterval CalcExtendedSelectionInterval() {
			return targetInterval;
		}
		protected internal override void ApplyChangesToControl(ChangeActions actions) {
			base.ApplyChangesToControl(actions | ChangeActions.RaiseSelectionChanged);
		}
	}
	#region DayViewExtendPrevCellCommand
	public class DayViewExtendPrevCellCommand : DayViewMovePrevCellCommand {
		public DayViewExtendPrevCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendPrevCell; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendNextCellCommand
	public class DayViewExtendNextCellCommand : DayViewMoveNextCellCommand {
		public DayViewExtendNextCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendNextCell; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendPageUpCommand
	public class DayViewExtendPageUpCommand : DayViewMovePageUpCommand {
		public DayViewExtendPageUpCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendPageUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendPageDownCommand
	public class DayViewExtendPageDownCommand : DayViewMovePageDownCommand {
		public DayViewExtendPageDownCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendPageDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendToStartOfWorkTimeCommand
	public class DayViewExtendToStartOfWorkTimeCommand : DayViewMoveToStartOfWorkTimeCommand {
		public DayViewExtendToStartOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendToStartOfWorkTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendToEndOfWorkTimeCommand
	public class DayViewExtendToEndOfWorkTimeCommand : DayViewMoveToEndOfWorkTimeCommand {
		public DayViewExtendToEndOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendToEndOfWorkTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendToStartOfVisibleTimeCommand
	public class DayViewExtendToStartOfVisibleTimeCommand : DayViewMoveToStartOfVisibleTimeCommand {
		public DayViewExtendToStartOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendToStartOfVisibleTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region DayViewExtendToEndOfVisibleTimeCommand
	public class DayViewExtendToEndOfVisibleTimeCommand : DayViewMoveToEndOfVisibleTimeCommand {
		public DayViewExtendToEndOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DayViewExtendToEndOfVisibleTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
}
