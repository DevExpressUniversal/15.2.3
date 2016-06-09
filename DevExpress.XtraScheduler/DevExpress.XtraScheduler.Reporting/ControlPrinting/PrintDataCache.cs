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
using System.Collections;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region PrintDataCache
	public abstract class PrintDataCache {
		#region Fields
		ControlPrintState printState = ControlPrintState.None;
		int groupStartIndex = -1;
		int groupLength = 1;
		int columnIndex = -1;
		ColumnArrangementMode columnArrangement;
		int visibleIntervalColumnCount = 1;
		bool allowMultiColumn;
		bool pageBreakBeforeNextColumn;
		#endregion
		public int VisibleIntervalColumnCount { get { return visibleIntervalColumnCount; } set { visibleIntervalColumnCount = value; } }
		public ColumnArrangementMode ColumnArrangement { get { return columnArrangement; } set { columnArrangement = value; } }
		protected internal bool AllowMultiColumn { get { return allowMultiColumn; } set { allowMultiColumn = value; } }
		protected internal bool PageBreakBeforeNextColumn { get { return pageBreakBeforeNextColumn; } set { pageBreakBeforeNextColumn = value; } }
		protected internal int ColumnCount { get { return AllowMultiColumn ? VisibleIntervalColumnCount : 1; } }
		protected internal int ColumnIndex { get { return columnIndex; } }
		protected internal ControlPrintState PrintState { get { return printState; } }
		protected internal int GroupStartIndex { get { return groupStartIndex; } }
		protected internal int GroupLength { get { return groupLength; } set { groupLength = Math.Max(1, value); } }
		protected internal abstract int SourceObjectCount { get; }
		protected internal abstract void FillData();
		protected internal virtual void SetPrintState(ControlPrintState printState) {
			SetPrintStateCore(printState);
		}
		void SetPrintStateCore(ControlPrintState printState) {
			this.printState = printState;
		}
		protected bool CheckPrintState(ControlPrintState printState) {
			return PrintState == printState;
		}
		public virtual void Reset() {
			this.groupStartIndex = 0;
			this.columnIndex = 0;
		}
		protected internal virtual bool CanReset() {
			return CheckPrintState(ControlPrintState.Reset);
		}
		protected internal virtual bool CanKeepCurrent() {
			return CheckPrintState(ControlPrintState.KeepCurrent);
		}
		protected internal virtual bool IsPrintingComplete() {
			return CheckPrintState(ControlPrintState.EndOfData);
		}
		protected internal virtual bool IsEndOfData() {
			return !CanMoveNextGroup();
		}
		protected internal virtual bool CanMoveNext() {
			return CheckPrintState(ControlPrintState.Print) && CanMoveNextGroup();
		}
		protected internal virtual void MoveNextData() {
			UpdateGroupStartIndex();
			MoveNextColumnIndex();
		}
		protected internal virtual bool CanMoveNextGroup() {
			int newStartIndex = groupStartIndex + CalculateNextGroupLength();
			return newStartIndex < SourceObjectCount;
		}
		protected internal void MoveNextColumnIndex() {
			this.columnIndex++;
			if (this.columnIndex == ColumnCount) {
				this.columnIndex = 0;
			}
		}
		protected internal virtual void UpdateGroupStartIndex() {
			this.groupStartIndex += CalculateCurrentGroupLength(); 
		}
		protected virtual int CalculateCurrentGroupLength() {
			return GroupLength;
		}
		protected virtual int CalculateNextGroupLength() {
			return GroupLength;
		}
		protected virtual int CalculateValidGroupEndIndex() {
			int groupLength = CalculateCurrentGroupLength();
			return CalculateValidGroupEndIndexCore(GroupStartIndex, groupLength);
		}
		protected virtual int CalculateValidGroupEndIndexCore(int groupStartIndex, int groupLength) {
			return Math.Min(groupStartIndex + groupLength, SourceObjectCount);
		}
		protected void FillGroup(IList group, IList source) {
			int endIndex = CalculateValidGroupEndIndex();
			FillGroupCore(group, source, GroupStartIndex, endIndex);
		}
		protected void FillGroupCore(IList group, IList source, int startIndex, int endIndex) {
			XtraSchedulerDebug.Assert(source.Count == SourceObjectCount);
			group.Clear();
			for (int i = startIndex; i < endIndex; i++)
				group.Add(source[i]);
		}
		protected virtual bool CanMoveNextColumn() {
			return !IsLastColumn(ColumnIndex);
		}
		protected bool IsLastColumn(int columnIndex) {
			return columnIndex >= ColumnCount - 1;
		}
		protected bool IsFirstColumn(int columnIndex) {
			return columnIndex == 0;
		}
		internal bool NeedPageBreakBeforeNexColumn() {
			return ColumnCount > 1 && !IsFirstPrinting(); 
		}
		private bool IsFirstPrinting() { 
			return ColumnIndex < 0;
		}
	}
	#endregion
	#region EmptyPrintDataCache
	public class EmptyPrintDataCache : PrintDataCache {
		#region static
		static readonly EmptyPrintDataCache instance;
		static EmptyPrintDataCache() {
			instance = new EmptyPrintDataCache();
		}
		public static EmptyPrintDataCache Instance { get { return instance; } }
		#endregion
		protected internal EmptyPrintDataCache() {
			SetPrintState(ControlPrintState.EndOfData);
		}
		protected internal override int SourceObjectCount { get { return 0; } }
		protected internal override bool IsEndOfData() {
			return true;
		}
		public override void Reset() {
		}
		protected internal override void FillData() {
		}
		protected internal override void MoveNextData() {
		}
		protected internal override void SetPrintState(ControlPrintState printState) {
			base.SetPrintState(ControlPrintState.EndOfData);
		}
	}
	#endregion
	public class DependentDataCache : PrintDataCache {
		public DependentDataCache() { 
		}
		protected internal override int SourceObjectCount {
			get { return 0; }
		}
		protected internal override void FillData() {
		}
		protected internal override bool CanMoveNextGroup() {
			return true;
		}
	}
	#region ResourceDataCache
	public class ResourceDataCache : PrintDataCache {
		ISchedulerResourceProvider resourceProvider;
		ResourceBaseCollection sourceResources;
		ResourceBaseCollection printResources;
		public ResourceDataCache(ISchedulerResourceProvider provider) {
			if (provider == null)
				Exceptions.ThrowArgumentNullException("provider");
			this.resourceProvider = provider;
			this.sourceResources = new ResourceBaseCollection();
			printResources = new ResourceBaseCollection();
		}
		protected internal ISchedulerResourceProvider ResourceProvider { get { return resourceProvider; } }
		protected internal ResourceBaseCollection SourceResources { get { return sourceResources; } }
		protected internal override int SourceObjectCount { get { return SourceResources.Count; } }
		protected internal ResourceBaseCollection PrintResources { get { return printResources; } }
		public override void Reset() {
			base.Reset();
			SourceResources.Clear();
			SourceResources.AddRange(GetSourceResources());
			PrintResources.Clear();
		}
		protected virtual ResourceBaseCollection GetSourceResources() {
			return ResourceProvider.GetResources();
		}
		protected internal override void FillData() {
			FillGroup(PrintResources, SourceResources);
		}
	}
	#endregion
	#region TimeIntervalDataCache
	public abstract class TimeIntervalDataCache : PrintDataCache {
		readonly ISchedulerTimeIntervalProvider timeIntervalProvider;
		readonly TimeIntervalCollection sourceTimeIntervals;
		readonly TimeIntervalCollection printTimeIntervals;
		readonly TimeIntervalCollection allColumnPrintIntervals;
		readonly TimeIntervalCollection nextPrintIntervals;
		TimeInterval nextPrintTimeInterval;
		protected TimeIntervalDataCache(ISchedulerTimeIntervalProvider provider) {
			if (provider == null)
				Exceptions.ThrowArgumentNullException("provider");
			this.timeIntervalProvider = provider;
			this.sourceTimeIntervals = new TimeIntervalCollection();
			this.nextPrintIntervals = new TimeIntervalCollection();
			this.printTimeIntervals = new TimeIntervalCollection();
			this.allColumnPrintIntervals = new TimeIntervalCollection();
		}
		protected internal ISchedulerTimeIntervalProvider TimeIntervalProvider { get { return timeIntervalProvider; } }
		protected internal TimeIntervalCollection SourceTimeIntervals { get { return sourceTimeIntervals; } }
		protected internal override int SourceObjectCount { get { return SourceTimeIntervals.Count; } }
		public TimeIntervalCollection PrintTimeIntervals { get { return printTimeIntervals; } }
		public TimeIntervalCollection AllColumnPrintIntervals { get { return allColumnPrintIntervals; } }
		protected internal TimeIntervalCollection NextPrintTimeIntervals { get { return nextPrintIntervals; } }
		protected internal TimeInterval NextPrintTimeInterval { get { return nextPrintTimeInterval; } set { nextPrintTimeInterval = value; } }
		public override void Reset() {
			base.Reset();
			SourceTimeIntervals.Clear();
			TimeIntervalCollection source = GetSourceTimeIntervals();
			SourceTimeIntervals.AddRange(source);
			PrepareNextPrintTimeIntervals(source);
			PrintTimeIntervals.Clear();
		}
		protected virtual void PrepareNextPrintTimeIntervals(TimeIntervalCollection source) {
			NextPrintTimeInterval = TimeInterval.Empty;
			NextPrintTimeIntervals.Clear();
			NextPrintTimeIntervals.AddRange(source);
			if (NextPrintTimeIntervals.Count > 0) 
				NextPrintTimeIntervals.RemoveAt(0);
		}
		protected internal virtual TimeIntervalCollection GetSourceTimeIntervals() {
			TimeIntervalCollection intervals = FilterSourceIntervals(TimeIntervalProvider.GetTimeIntervals());
			SplitVisibleIntervals(intervals);
			return intervals;
		}		
		protected internal virtual TimeIntervalCollection FilterSourceIntervals(TimeIntervalCollection intervals) {
			return intervals;
		}
		protected internal virtual void SplitVisibleIntervals(TimeIntervalCollection intervals) {
		}
		protected internal virtual TimeInterval GetPrintTimeInterval(PrintContentMode displayMode) {			
			return displayMode == PrintContentMode.AllColumns ?  AllColumnPrintIntervals.Interval : PrintTimeIntervals.Interval;
		}
		protected internal virtual TimeInterval GetSmartSyncTimeInterval() { 
		   return PrintTimeIntervals.Interval;
		}
		internal TimeInterval GetNextTimeInterval() {
			return NextPrintTimeInterval;	
		}
		protected virtual void FillNextPrintTimeInterval() {
			int groupEndIndex = CalculateValidGroupEndIndex();
			int nextIndex = Math.Max(0, groupEndIndex - 1);
			if (nextIndex >= 0 && nextIndex < NextPrintTimeIntervals.Count)
				NextPrintTimeInterval = NextPrintTimeIntervals[nextIndex];
			else
				NextPrintTimeInterval = new TimeInterval(PrintTimeIntervals.Interval.End, TimeSpan.Zero);
		}
	}
	#endregion
	public class ColumnTimeIntervalDataCache : TimeIntervalDataCache {
		public ColumnTimeIntervalDataCache(ISchedulerTimeIntervalProvider provider): base(provider) {
		}
		protected override int CalculateCurrentGroupLength() {
			return CalculateColumnLength(CalculateCurrentGroupActualLength(), ColumnCount, ColumnIndex);
		}
		protected override int CalculateNextGroupLength() {
			int colIndex = Math.Min(ColumnIndex + 1, ColumnCount - 1);
			return CalculateColumnLength(CalculateCurrentGroupActualLength(), ColumnCount, colIndex);
		}
		protected virtual int CalculateColumnLength(int valueCount, int columnCount, int columnIndex) {
			int[] valueCountByColumns = SchedulerPrintingUtils.SplitVisibleColumns(valueCount, columnCount, ColumnArrangement);
			XtraSchedulerDebug.Assert(columnIndex >= 0 && columnIndex < valueCountByColumns.Length);
			return Math.Max(1, valueCountByColumns[columnIndex]);
		}
		protected internal override void FillData() {
			FillGroup(PrintTimeIntervals, SourceTimeIntervals);
			if (ColumnIndex == 0)
				CalculateAllColumnIntervals(); 
			FillNextPrintTimeInterval();
		}
		protected internal virtual void CalculateAllColumnIntervals() {
			AllColumnPrintIntervals.Clear();
			int groupEndIndex = CalculateValidGroupEndIndex();			 
			for (int i = GroupStartIndex; i < groupEndIndex; i++) 
				AllColumnPrintIntervals.Add(SourceTimeIntervals[i]);			
		}
		protected internal virtual int CalculateCurrentGroupActualLength() {
			return GroupLength;
		}
	}
	public class DaysIntervalDataCache : ColumnTimeIntervalDataCache {
		WeekDays visibleWeekDays = WeekDays.EveryDay;
		public DaysIntervalDataCache(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		public WeekDays VisibleWeekDays { get { return visibleWeekDays; } set { visibleWeekDays = value; } }
		protected internal override TimeIntervalCollection FilterSourceIntervals(TimeIntervalCollection intervals) {
			TimeIntervalCollection result = new TimeIntervalCollection();
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				if (IsIntervalVisible(interval))
					result.Add(interval);
			}
			return result;
		}
		protected internal virtual bool IsIntervalVisible(TimeInterval interval) {		 
			return (VisibleWeekDays & DateTimeHelper.ToWeekDays(interval.Start.DayOfWeek)) != 0;
		}
	}
	public class TimelineIntervalDataCache : ColumnTimeIntervalDataCache {
		VisibleIntervalsSplitting intervalsSplitting;
		DateTimeCollection splitPoints = new DateTimeCollection();
		DayOfWeek firstDayOfWeek;
		public TimelineIntervalDataCache(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		public VisibleIntervalsSplitting IntervalsSplitting { get { return intervalsSplitting; } set { intervalsSplitting = value; } }
		public DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		internal DateTimeCollection SplitPoints { get { return splitPoints; } }
		protected internal override int CalculateCurrentGroupActualLength() {
			return CalculateCurrentGroupActualEnd() - GroupStartIndex;
		}
		protected override int CalculateNextGroupLength() {
			if (ColumnCount > 1)
				return base.CalculateNextGroupLength();
			return CalculateCurrentGroupActualLength();
		}
		protected internal virtual int CalculateCurrentGroupActualEnd() {
			return CalculateGroupActualEnd(GroupStartIndex);
		}
		protected internal virtual int CalculateNextGroupActualLength() {
			int currentGroupEnd = CalculateCurrentGroupActualEnd();
			return Math.Max(1, CalculateGroupActualEnd(currentGroupEnd) - currentGroupEnd);
		}
		protected internal virtual int CalculateGroupActualEnd(int groupStart) {
			int groupEnd = CalculateValidGroupEndIndexCore(groupStart, GroupLength);
			for (int i = groupStart; i < groupEnd; i++)
				if (SplitPoints.Contains(SourceTimeIntervals[i].End))
					return i + 1;
			return groupEnd;
		}
		protected internal override void SplitVisibleIntervals(TimeIntervalCollection intervals) {
			SplitPoints.Clear();
			if (IntervalsSplitting == VisibleIntervalsSplitting.None)
				return;
			TimeIntervalCollection splitPattern = CreateSplitPatternIntervals();
			splitPattern.AddRange(intervals);
			SplitVisibleIntervalsCore(intervals, splitPattern);
		}
		protected internal virtual void SplitVisibleIntervalsCore(TimeIntervalCollection intervals, TimeIntervalCollection splitPattern) {
			XtraSchedulerDebug.Assert(splitPattern.Interval.Contains(intervals.Interval));
			if (!splitPattern.Interval.Contains(intervals.Interval))
				return;
			int currentIndex = 0; int alignemtnIndex = 0;
			while (currentIndex < intervals.Count) {
				TimeInterval currentInterval = intervals[currentIndex];
				DateTime currentSplitEnd = splitPattern[alignemtnIndex].End;
				if (currentInterval.End >= currentSplitEnd) {
					SplitTimeIntervals(intervals, currentSplitEnd, currentIndex);
					alignemtnIndex++;
				}
				currentIndex++;
			}
		}
		protected internal virtual void SplitTimeIntervals(TimeIntervalCollection intervals, DateTime dateTime, int index) {
			TimeIntervalCollection splittedIntervals = SplitTimeInterval(intervals[index], dateTime);
			intervals.RemoveAt(index);
			InsertIntervals(intervals, splittedIntervals, index);
		}
		protected internal virtual TimeIntervalCollection SplitTimeInterval(TimeInterval interval, DateTime dateTime) {
			XtraSchedulerDebug.Assert(interval.Contains(dateTime));
			TimeIntervalCollection result = new TimeIntervalCollection();
			if (interval.Start == dateTime || interval.End == dateTime)
				result.Add(interval);
			else {
				result.Add(new TimeInterval(interval.Start, dateTime));
				result.Add(new TimeInterval(dateTime, interval.End));				
			}
			SplitPoints.Add(dateTime);
			return result;
		}
		protected internal virtual void InsertIntervals(TimeIntervalCollection source, TimeIntervalCollection intervals, int index) {
			source.BeginUpdate();
			int count = intervals.Count;
			for (int i = count - 1; i >= 0; i--)
				source.Insert(index, intervals[i]);
			source.EndUpdate();
		}
		protected internal virtual TimeIntervalCollection CreateSplitPatternIntervals() {
			TimeScale scale = GetAlignmentScale();
			scale.SetFirstDayOfWeek(FirstDayOfWeek);
			return new TimeScaleIntervalCollection(scale);
		}
		protected internal virtual TimeScale GetAlignmentScale() {
			switch (IntervalsSplitting) {
				case VisibleIntervalsSplitting.Day: return new TimeScaleDay();
				case VisibleIntervalsSplitting.Hour: return new TimeScaleHour();
				case VisibleIntervalsSplitting.Month: return new TimeScaleMonth();
				case VisibleIntervalsSplitting.Quarter: return new TimeScaleQuarter();
				case VisibleIntervalsSplitting.Week: return new TimeScaleWeek();
				case VisibleIntervalsSplitting.Year: return new TimeScaleYear();
				default: return new TimeScaleDay();
			}
		}
	}
	public abstract class WeekControlBaseDataCache : TimeIntervalDataCache {
		readonly WeekCollection printWeeks;
		DayOfWeek firstDayOfWeek;
		bool exactlyOneMonth;
		WeekDays visibleWeekDays;
		protected WeekControlBaseDataCache(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
			this.printWeeks = new WeekCollection();
		}
		public WeekCollection PrintWeeks { get { return printWeeks; } }
		public virtual bool ExactlyOneMonth { get { return exactlyOneMonth; } set { exactlyOneMonth = value; } }
		public virtual DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		public virtual WeekDays VisibleWeekDays { get { return visibleWeekDays; } set { visibleWeekDays = value; } }
		protected internal override bool CanMoveNextGroup() {
			if (CanMoveNextColumn())
				return true;
			return base.CanMoveNextGroup();
		}
		protected internal override bool IsEndOfData() {
			return !CanMoveNextColumn() && base.IsEndOfData();
		}
		protected internal override void MoveNextData() {
			int prevColumnIndex = ColumnIndex;
			MoveNextColumnIndex();
			if (IsRowPrintComplete(ColumnIndex, prevColumnIndex))
				UpdateGroupStartIndex();
		}
		protected bool IsRowPrintComplete(int currColumnIndex, int prevColumnIndex) {
			return IsFirstColumn(currColumnIndex) && IsLastColumn(prevColumnIndex + 1);
		}
		protected internal override void FillData() {
			PrepareFillData();
			int groupEndIndex = CalculateValidGroupEndIndex();
			for (int i = GroupStartIndex; i < groupEndIndex; i++) {
				TimeInterval interval = SourceTimeIntervals[i];
				AllColumnPrintIntervals.Add(interval);
				AddPrintWeeks(interval);
			}
			FillNextPrintTimeInterval();
		}
		protected internal virtual void PrepareFillData() {
			PrintWeeks.Clear();
			PrintTimeIntervals.Clear();
			AllColumnPrintIntervals.Clear();
		}
		protected internal virtual WeekIntervalCollection CreateActualWeekIntervalCollection() {
			WeekIntervalCollection weekIntervals = new WeekIntervalCollection();
			weekIntervals.CompressWeekend = false;
			weekIntervals.FirstDayOfWeek = FirstDayOfWeek;
			return weekIntervals;
		}
		protected internal abstract void AddPrintWeeks(TimeInterval interval);
		protected internal virtual void AddPrintWeek(TimeInterval sourceWeekInterval) {
			DateTime[] sourceDates = DateTimeHelper.GetDates(sourceWeekInterval);
			DateTime[] printWeekDates = GetWeekDates(sourceDates);
			if (printWeekDates.Length > 0) {
				Week week = CreateWeek(printWeekDates);
				AddPrintWeekCore(week);
			}
		}
		protected virtual Week CreateWeek(DateTime[] printWeekDates) {
			int[] valueCountByColumns = SchedulerPrintingUtils.SplitVisibleColumns(printWeekDates.Length, ColumnCount, ColumnArrangement);
			int length = Math.Max(1, valueCountByColumns[ColumnIndex]);
			int startIndex = SchedulerPrintingUtils.CalculateTotalValues(valueCountByColumns, 0, ColumnIndex);
			List<DateTime> columnWeekDates = new List<DateTime>();
			for (int i = startIndex; i < startIndex + length; i++)
				columnWeekDates.Add(printWeekDates[i]);
			return new Week(columnWeekDates.ToArray());
		}
		protected internal virtual void AddPrintWeekCore(Week week) {
			PrintWeeks.Add(week);
			AddPrintInterval(week);
		}
		private void AddPrintInterval(Week week) {
			TimeInterval printInterval = GetPrintInterval(week.Interval);
			if (printInterval.Duration != TimeSpan.Zero)
				PrintTimeIntervals.Add(printInterval);
		}
		private TimeInterval GetPrintInterval(TimeInterval weekInterval) {
			TimeInterval printInterval = weekInterval.Clone();
			return printInterval.Intersect(AllColumnPrintIntervals.Interval);
		}
		public virtual DateTime[] GetWeekDates(DateTime[] weekDates) {
			List<DateTime> result = new List<DateTime>();
			int count = weekDates.Length;
			for (int i = 0; i < count; i++)
				if (IsWeekDayVisible(weekDates[i].DayOfWeek))
					result.Add(weekDates[i]);
			return result.ToArray();
		}
		protected internal virtual bool IsWeekDayVisible(DayOfWeek dayOfWeek) {
			return (VisibleWeekDays & DateTimeHelper.ToWeekDays(dayOfWeek)) != 0;
		}
	}
	public class WeekControlWeeksDataCache : WeekControlBaseDataCache {
		public WeekControlWeeksDataCache(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		protected internal override TimeIntervalCollection GetSourceTimeIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			TimeIntervalCollection baseIntervals = base.GetSourceTimeIntervals();
			int count = baseIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeIntervalCollection intervals = ValidateFirstDayOfWeek(baseIntervals[i]);
				AddSourceIntervals(result, intervals);
			}
			return result;
		}
		protected internal virtual TimeIntervalCollection ValidateFirstDayOfWeek(TimeInterval timeInterval) {
			WeekIntervalCollection weekIntervals = CreateActualWeekIntervalCollection();
			weekIntervals.Add(timeInterval);
			return ValidateIntervalsBounds(weekIntervals);
		}
		protected internal virtual TimeIntervalCollection ValidateIntervalsBounds(TimeIntervalCollection intervals) {
			TimeIntervalCollection result = new TimeIntervalCollection();
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				TimeInterval adapterInterval = TimeIntervalProvider.GetAdapterInterval();
				TimeSpan intersection = TimeInterval.Intersect(interval, adapterInterval).Duration;
				if (intersection != TimeSpan.Zero)
					result.Add(interval);
			}
			return result;
		}
		protected internal virtual void AddSourceIntervals(TimeIntervalCollection target, TimeIntervalCollection intervals) {
			for (int j = 0; j < intervals.Count; j++) {
				TimeInterval interval = intervals[j];
				if (!target.Contains(interval))
					target.Add(interval);
			}
		}
		protected internal override void AddPrintWeeks(TimeInterval interval) {
			AddPrintWeek(interval);
		}
	}
	public class WeekControlMonthsDataCache : WeekControlBaseDataCache {
		TimeIntervalCollection monthIntervals;
		public WeekControlMonthsDataCache(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
			monthIntervals = new TimeIntervalCollection();
		}
		protected internal override TimeInterval GetSmartSyncTimeInterval() {
			if (GroupStartIndex >= monthIntervals.Count)
				return TimeInterval.Empty;
			return monthIntervals[GroupStartIndex]; 
		}
		protected internal override TimeIntervalCollection GetSourceTimeIntervals() {
			TimeIntervalCollection monthIntervals = base.GetSourceTimeIntervals();
			this.monthIntervals.Clear();
			this.monthIntervals.AddRange(monthIntervals); 
			if (ExactlyOneMonth) 
				return monthIntervals;
			return AdjustToWeeks(monthIntervals);
		}
		protected internal virtual TimeIntervalCollection AdjustToWeeks(TimeIntervalCollection monthIntervals) {
			TimeIntervalCollection result = new TimeIntervalCollection();
			for (int i = 0; i < monthIntervals.Count; i++)
				result.Add(AdjustToWeeksCore(monthIntervals[i]));
			return result;
		}
		protected internal virtual TimeInterval AdjustToWeeksCore(TimeInterval month) {
			WeekIntervalCollection intervals = CreateActualWeekIntervalCollection();
			intervals.Add(month);
			return intervals.Interval;
		}
		protected internal override void AddPrintWeeks(TimeInterval interval) {
			TimeIntervalCollection weekIntervals = SplitToWeekIntervals(interval);
			int count = weekIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval weekInterval = weekIntervals[i];
				AddPrintWeek(weekInterval);
			}
		}
		protected internal virtual TimeIntervalCollection SplitToWeekIntervals(TimeInterval interval) {
			WeekIntervalCollection weekIntervals = CreateActualWeekIntervalCollection();
			weekIntervals.Add(interval);
			return weekIntervals;
		}
	}	
}
