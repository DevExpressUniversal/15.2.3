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
using System.Collections.Specialized;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF || SL
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class WorkTimeCellBase : TimeCellWithContentBase {
		bool isWorkTime;
		protected WorkTimeCellBase(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
		}
		public bool IsWorkTime {
			get { return isWorkTime; }
			internal set {
				if (IsWorkTime == value)
					return;
				isWorkTime = value;
			}
		}
	}
	public class TimeCell : WorkTimeCellBase, ISelectableIntervalViewInfo {
		#region Fields
		bool endOfHour;
		bool showAllDayArea;
		#endregion
		public TimeCell(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
		}
		#region Properties
		public bool EndOfHour {
			get { return endOfHour; }
			internal set {
				if (EndOfHour == value)
					return;
				endOfHour = value;
			}
		}
		public TimeCell Cell { get { return this; } }
		public bool ShowAllDayArea { get { return showAllDayArea; } set { showAllDayArea = value; } }
		#endregion
		#region ISelectableIntervalViewInfo Members
		public bool Selected {
			get {
				return false;
			}
		}
		public virtual SchedulerHitTest HitTestType {
			get { return SchedulerHitTest.Cell; }
		}
		#endregion
	}
	public class TimeCellContainer : TimeCellContainerBase, IWorkTimeInfo {
		bool isWorkDay;
		TimeOfDayIntervalCollection workTimes;
		public TimeCellContainer(DayView view, TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(view, interval, resource, brushes) {
			if (!DateTimeHelper.IsExactlyOneDay(interval))
				Exceptions.ThrowArgumentException("interval", interval);
		}
		public new DayView View { get { return (DayView)base.View; } }
		public bool IsWorkDay {
			get { return isWorkDay; }
			private set {
				if (IsWorkDay == value)
					return;
				isWorkDay = value;
			}
		}
		public TimeOfDayIntervalCollection WorkTimes {
			get { return workTimes; }
			private set {
				if (object.Equals(WorkTimes, value))
					return;
				workTimes = value;
			}
		}
		protected override WorkTimeInfoCalculatorBase CreateWorkTimeCalculator() {
			return new DayViewWorkTimeInfoCalculator(View.InnerView);
		}
		protected internal override void PopulateCellCollection(SchedulerViewBase view) {
			PrepareWorkTimeProperties();
			TimeOfDayIntervalCollection timeOfDayIntervalCollection = TimeRulerHelper.SplitInterval(View.InnerView.ActualVisibleTime, View.InnerView.TimeScale);
			int count = timeOfDayIntervalCollection.Count;
			for (int i = 0; i < count; i++) {
				TimeOfDayInterval timeOfDayInterval = timeOfDayIntervalCollection[i];
				TimeInterval cellInterval = new TimeInterval(Interval.Start + timeOfDayInterval.Start, timeOfDayInterval.Duration);
				TimeCell cell = new TimeCell(cellInterval, Resource, Brushes);
				InitializeCell(cell, Interval);
				Cells.Add(cell);
			}
		}
		void PrepareWorkTimeProperties() {
			if (WorkTimeCalculator != null) {
				WorkTimeInfo workTimeInfo = WorkTimeCalculator.CalcWorkTimeInfo(base.Interval, (XtraScheduler.Resource)base.Resource);
				this.isWorkDay = workTimeInfo.IsWorkDay;
				this.workTimes = workTimeInfo.WorkTimes;
			}
		}
		protected internal virtual void InitializeCell(TimeCell cell, TimeInterval interval) {
			TimeInterval cellInterval = cell.Interval;
			cell.IsWorkTime = DayViewTimeCellHelper.IsWorkTime(this, cellInterval, interval);
			cell.EndOfHour = DateTimeHelper.IsBeginOfHour(cellInterval.End);
		}
	}
	public class AllDayAreaCellsContainter : SimpleTimeCellContainerBase<AllDayAreaCell> {
		public AllDayAreaCellsContainter(SchedulerViewBase view, AllDayAreaCellCollection cells)
			: base(view) {
			Guard.ArgumentNotNull(cells, "cells");
			SetCells(cells);
		}
		public override AssignableSchedulerViewCellBaseCollection<AllDayAreaCell> CreateCellContainerCells() {
			return null;
		}
		public override AssignableSchedulerViewCellBaseCollection<AllDayAreaCell> Cells {
			get {
				return base.Cells;
			}
			protected set {
				base.Cells = value;
			}
		}
		public override Resource Resource {
			get {
				if (CellCount <= 0)
					return ResourceBase.Empty;
				else
					return Cells[0].Resource;
			}
			internal set {
			}
		}
		public override TimeInterval Interval {
			get {
				if (CellCount <= 0)
					return TimeInterval.Empty;
				TimeInterval interval = new TimeInterval();
				interval.Start = Cells[0].Interval.Start;
				interval.End = Cells[Cells.Count - 1].Interval.End;
				return interval;
			}
			internal set {
			}
		}
		public override CellContainerType ContainerType { get { return CellContainerType.Horizontal; } }
		protected override bool FitAppointmentControl(AppointmentControl control) {
			if (!(control is HorizontalAppointmentControl))
				return false;
			return this.Interval.Contains(control.ViewInfo.Interval) && ResourceBase.InternalMatchIds((object)control.ViewInfo.Resource.Id, (object)this.Resource.Id);
		}
		protected internal override void CalculateSelectionLayoutCore(SchedulerViewSelection schedulerViewSelection) {
			TimeInterval interval = schedulerViewSelection.Interval;
			if (DateTimeHelper.IsIntervalWholeDays(interval)) {
				base.CalculateSelectionLayoutCore(schedulerViewSelection);
				return;
			}
			int firstIndex = FindFirstSelectedCellIndex(interval.Start);
			int lastIndex = FindLastSelectedCellIndex(interval.End);
			if (firstIndex >= 0)
				firstIndex++;
			if (lastIndex >= 0)
				lastIndex--;
			if (lastIndex < firstIndex) {
				SelectedCells = null;
				return;
			}
			SetSelectedCells(schedulerViewSelection, firstIndex, lastIndex);
		}
	}
	public class AllDayAreaCell : WorkTimeCellBase, ISelectableIntervalViewInfo {
		public AllDayAreaCell(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
		}
		bool ISelectableIntervalViewInfo.Selected { get { return false; } }
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType { get { return SchedulerHitTest.AllDayArea; } }
	}
	public class DayViewColumn : TimeCellContainer, IViewInfo {
		#region Fields
		AllDayAreaCell allDayAreaCell;
		bool showAllDayArea;
		bool showDayHeaders;
		StringCollection dateFormats;
		string caption;
		#endregion
		public DayViewColumn(DayView view, TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(view, interval, resource, brushes) {
			this.allDayAreaCell = new AllDayAreaCell(Interval, Resource, brushes);
			this.showAllDayArea = view.InnerView.ShowAllDayArea;
			this.showDayHeaders = view.InnerView.ShowDayHeaders;
			ActualShowAllAppointmentsAtTimeCells = view.InnerView.ActualShowAllAppointmentsAtTimeCells;
			this.dateFormats = DateTimeFormatHelper.GenerateFormatsWithoutYear();
		}
		#region Properties
		public bool ShowAllDayArea { get { return showAllDayArea; } }
		public bool ShowDayHeaders { get { return showDayHeaders; } }
		public bool ActualShowAllAppointmentsAtTimeCells { get; private set; }
		protected internal AllDayAreaCell AllDayAreaCell { get { return allDayAreaCell; } }
		public StringCollection DateFormats { get { return dateFormats; } }
		public string Caption {
			get { return caption; }
			set {
				if (caption == value)
					return;
				SetCaptionInternal(value);
			}
		}
		public override CellContainerType ContainerType { get { return CellContainerType.Vertical; } }
		#endregion
		internal void SetCaptionInternal(string value) {
			this.caption = value;
		}
		public override void CalculateSelectionLayout(SchedulerViewSelection schedulerViewSelection) {
			if (DateTimeHelper.IsIntervalWholeDays(schedulerViewSelection.Interval)) {
				SelectedCells = null;
				return;
			}
			base.CalculateSelectionLayout(schedulerViewSelection);
		}
		protected override bool FitAppointmentControlDuration(AppointmentControl control) {
			return !ShowAllDayArea || ActualShowAllAppointmentsAtTimeCells || base.FitAppointmentControlDuration(control);
		}
		protected override bool FitAppointmentControlInterval(TimeInterval appointmentInterval) {
			TimeInterval actualColumnInterval = View.VisibleTime.ToTimeInterval(Interval.Start.Date);
			return actualColumnInterval.Contains(appointmentInterval);
		}
	}
	public abstract class DayViewInfoBase : ViewInfoBase<DayViewColumn> {
		AssignableCollection<TimeRulerViewInfo> timeRulers;
		System.Windows.Visibility moreButtonsVisibility;
		bool showTimeRulerHeader;
		bool showDayHeaders;
		bool showTimeRulers;
		protected DayViewInfoBase(DayView view)
			: base(view) {
		}
		public override void Create() {
			base.Create();
			TimeRulers = CreateTimeRulers();
			this.showTimeRulerHeader = CalculateShowTimeRulerHeader();
			ShowTimeRulers = CalculateShowTimeRuler(TimeRulers);
			ShowDayHeaders = View.InnerView.ShowDayHeaders;
			CalculateMoreButtonsVisibility();
		}
		public new DayView View { get { return (DayView)base.View; } }
		public AssignableCollection<TimeRulerViewInfo> TimeRulers { get { return timeRulers; } set { timeRulers = value; } }
		public System.Windows.Visibility MoreButtonsVisibility { get { return moreButtonsVisibility; } }
		public bool ShowTimeRulerHeader { get { return showTimeRulerHeader; } }
		public bool ShowDayHeaders { get { return showDayHeaders; } set { showDayHeaders = value; } }
		public bool ShowTimeRulers { get { return showTimeRulers; } protected set { showTimeRulers = value; } }
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get {
				if (View == null || View.TimeIndicatorDisplayOptions == null)
					return TimeIndicatorVisibility.Always;
				return View.TimeIndicatorDisplayOptions.Visibility;
			}
		}
		protected internal virtual int TimeRulerHeaderSpan {
			get {
				int result = 0;
				if (View.InnerView.ShowAllDayArea)
					result++;
				if (View.InnerView.ShowDayHeaders)
					result++;
				return result;
			}
		}
		protected internal void CalculateMoreButtonsVisibility() {
			if (View.InnerView.ShowMoreButtons && !View.InnerView.ShowMoreButtonsOnEachColumn)
				moreButtonsVisibility = System.Windows.Visibility.Visible;
			else
				moreButtonsVisibility = System.Windows.Visibility.Collapsed;
		}
		protected virtual bool CalculateShowTimeRulerHeader() {
			return true;
		}
		protected virtual bool CalculateShowTimeRuler(AssignableCollection<TimeRulerViewInfo> timeRulers) {
			int count = timeRulers.Count;
			bool isTimeRulerVisible = count > 0;
			for (int i = 0; i < count; i++)
				isTimeRulerVisible &= timeRulers[i].TimeRuler.Visible;
			return isTimeRulerVisible;
		}
		protected internal AssignableCollection<TimeRulerViewInfo> CreateTimeRulers() {
			InnerDayView innerView = View.InnerView;
			ITimeRulerFormatStringService formatStringProvider = (ITimeRulerFormatStringService)View.Control.GetService(typeof(ITimeRulerFormatStringService));
			TimeZoneHelper timeZoneEngine = View.Control.InnerControl.TimeZoneHelper;
			TimeZoneInfo clientTimeZone = timeZoneEngine.ClientTimeZone;
			int timeRulerHeaderSpan = this.TimeRulerHeaderSpan;
			TimeOfDayInterval interval = innerView.ActualVisibleTime;
			TimeSpan scale = innerView.TimeScale;
			AssignableCollection<TimeRulerViewInfo> result = new AssignableCollection<TimeRulerViewInfo>();
			TimeRulerCollection rulers = innerView.GetVisibleTimeRulers();
			int count = rulers.Count;
			for (int i = 0; i < count; i++) {
				TimeRuler timeRuler = rulers[i];
				TimeFormatInfo timeFormatInfo = new TimeFormatInfo();
				timeFormatInfo.Initialize(timeRuler, formatStringProvider);
				TimeRulerViewInfo ruler = new TimeRulerViewInfo(View, innerView.VisibleStart, timeRuler, interval, clientTimeZone, scale, i == 0, timeRulerHeaderSpan, timeFormatInfo);
				ruler.TimeZoneHelper = Control.InnerControl.TimeZoneHelper;
				result.Add(ruler);
			}
			return result;
		}
		protected internal override AssignableCollection<DayViewColumn> CreateResourceCellContainers(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			AssignableCollection<DayViewColumn> result = base.CreateResourceCellContainers(resource, visibleIntervals, brushes);
			return result;
		}
		protected internal override DayViewColumn CreateCellContainer(TimeInterval interval, Resource resource, ResourceBrushes brushes) {
			DayViewColumn column = new DayViewColumn(View, interval, resource, brushes);
			column.Initialize();
			return column;
		}
		protected internal virtual DateHeader CreateColumnHeader(DayViewColumn column) {
			if (!View.InnerView.ShowDayHeaders)
				return null;
			DateHeader header = new DateHeader(this.View, column.Interval, column.Resource);
			return header;
		}
		protected bool CanAddHorizontalCellContainer() {
			return View.InnerView.ShowAllDayArea;
		}
		#region ISchedulerViewInfoBase Members
		protected internal override CellContainerCollection GetCellContainers(AssignableCollection<DayViewColumn> resourceCellContainers) {
			CellContainerCollection result = base.GetCellContainers(resourceCellContainers);
			if (CanAddHorizontalCellContainer())
				AddAllDayCellsContainers(result, resourceCellContainers);
			return result;
		}
		protected internal override CellContainerCollection GetContainersForUpdateSelection() {
			CellContainerCollection result = new CellContainerCollection();
			int count = ResourcesContainers.Count;
			for (int i = 0; i < count; i++) {
				result.AddRange(ResourcesContainers[i].CellContainers);
			}
			return result;
		}
		protected internal virtual void AddAllDayCellsContainers(CellContainerCollection target, AssignableCollection<DayViewColumn> resourceCellContainers) {
			AssignableCollection<AllDayAreaCellsContainter> allDayCellContainers = GetAllDayCellsContainers(resourceCellContainers);
			int count = allDayCellContainers.Count;
			for (int i = 0; i < count; i++)
				target.Add(allDayCellContainers[i]);
		}
		Dictionary<AssignableCollection<DayViewColumn>, AllDayAreaCellsContainter> allDayAreaCellsContainters = new Dictionary<AssignableCollection<DayViewColumn>, AllDayAreaCellsContainter>();
		protected internal virtual AssignableCollection<AllDayAreaCellsContainter> GetAllDayCellsContainers(AssignableCollection<DayViewColumn> resourceCellContainers) {
			AllDayAreaCellsContainter container;
			if (!allDayAreaCellsContainters.TryGetValue(resourceCellContainers, out container)) {
				AllDayAreaCellCollection allDayAreaCells = new AllDayAreaCellCollection();
				int count = resourceCellContainers.Count;
				for (int j = 0; j < count; j++) {
					AllDayAreaCell allDayCell = ((DayViewColumn)resourceCellContainers[j]).AllDayAreaCell;
					allDayAreaCells.Add(allDayCell);
				}
				container = new AllDayAreaCellsContainter(this.View, allDayAreaCells);
				allDayAreaCellsContainters.Add(resourceCellContainers, container);
			}
			AssignableCollection<AllDayAreaCellsContainter> result = new AssignableCollection<AllDayAreaCellsContainter>();
			result.Add(container);
			return result;
		}
		#endregion
		protected internal override SingleResourceViewInfo CreateSingleResourceViewInfo() {
			return new DayBasedSingleResourceViewInfo(this.View);
		}
		protected internal override SingleResourceViewInfo CreateSingleResourceView(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			DayBasedSingleResourceViewInfo viewInfo = (DayBasedSingleResourceViewInfo)base.CreateSingleResourceView(resource, visibleIntervals, brushes);
			viewInfo.TimeIndicatorVisibility = TimeIndicatorVisibility;
			viewInfo.HorizontalCellContainer = GetHorizontalCellContainer(viewInfo.CellContainers);
			viewInfo.VerticalCellContainers = GetVerticalCellContainers(viewInfo.CellContainers);
			viewInfo.FirstCellContainer = GetFirstCellContainer(viewInfo.CellContainers);
			return viewInfo;
		}
		protected virtual CellContainerCollection GetVerticalCellContainers(CellContainerCollection source) {
			CellContainerCollection result = new CellContainerCollection();
			int count = source.Count;
			for (int i = 0; i < count; i++) {
				DayViewColumn dayViewColumn = source[i] as DayViewColumn;
				if (dayViewColumn != null)
					result.Add(dayViewColumn);
			}
			return result;
		}
		protected virtual ICellContainer GetHorizontalCellContainer(CellContainerCollection source) {
			if (!CanAddHorizontalCellContainer())
				return null;
			CellContainerCollection result = new CellContainerCollection();
			int count = source.Count;
			for (int i = 0; i < count; i++) {
				AllDayAreaCellsContainter allDayAreaCellsContainter = source[i] as AllDayAreaCellsContainter;
				if (allDayAreaCellsContainter != null)
					result.Add(allDayAreaCellsContainter);
			}
			XtraSchedulerDebug.Assert(result.Count == 1);
			return result[0];
		}
		protected virtual DayViewColumn GetFirstCellContainer(CellContainerCollection source) {
			DayViewColumn dayViewColumn = source[0] as DayViewColumn;
			if (dayViewColumn != null)
				return dayViewColumn;
			return null;
		}
		protected internal override IViewInfo CreateAdditionalViewElements() {
			return null;
		}
		protected internal void CreateDaysView() {
			int count = VisibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeIntervalCollection intervals = new TimeIntervalCollection();
				SingleDayViewInfo dayView = new SingleDayViewInfo(this.View);
				dayView.TimeIndicatorVisibility = TimeIndicatorVisibility;
				intervals.Add(VisibleIntervals[i]);
				dayView.SingleResourceViewInfoCollection = CreateResourcesView(intervals);
				DaysContainers.Add(dayView);
			}
		}
	}
	public class DayViewGroupByNone : DayViewInfoBase {
		public DayViewGroupByNone(DayView view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override ResourceHeaderBase CreateResourceHeader(Resource resource, ResourceBrushes brushes) {
			return null;
		}
		public override int GetResourceColorIndex(Resource resource) {
			return 0;
		}
		protected override bool CalculateShowTimeRulerHeader() {
			return View.InnerView.ShowDayHeaders || View.InnerView.ShowAllDayArea;
		}
	}
	public class DayViewGroupByResource : DayViewInfoBase {
		public DayViewGroupByResource(DayView view)
			: base(view) {
		}
	}
	public class DayViewGroupByDate : DayViewInfoBase {
		public DayViewGroupByDate(DayView view)
			: base(view) {
		}
		public override void Create() {
			ResourcesContainers.Clear();
			DaysContainers.Clear();
			CreateDaysView();
			CreateNavigationButtons();
			TimeRulers = CreateTimeRulers();
			ShowTimeRulers = CalculateShowTimeRuler(TimeRulers);
			CalculateMoreButtonsVisibility();
			ShowDayHeaders = View.InnerView.ShowDayHeaders;
		}
	}
}
