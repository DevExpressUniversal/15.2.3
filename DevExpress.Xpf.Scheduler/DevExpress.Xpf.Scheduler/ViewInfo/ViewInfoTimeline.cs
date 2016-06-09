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
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF || SL
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Native {
	#region TimelineViewFactoryHelper
	public class TimelineViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new TimelineViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new TimelineViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new TimelineViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo, bool alternate) {
			return new TimelineViewAppointmentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region TimelineViewGroupByNoneStrategy
	public class TimelineViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new TimelineViewGroupByNone((TimelineView)view);
		}
	}
	#endregion
	#region TimelineViewGroupByDateStrategy
	public class TimelineViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new TimelineViewGroupByDate((TimelineView)view);
		}
	}
	#endregion
	#region TimelineViewGroupByResourceStrategy
	public class TimelineViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new TimelineViewGroupByResource((TimelineView)view);
		}
	}
	#endregion
	#region TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<TimelineViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(TimelineViewGroupByNone timelineView, int resourceIndex) {
			TimeCellCollection cells = timelineView.ResourcesCellContainers[0][0].Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<TimelineViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(TimelineViewGroupByDate timelineView, int resourceIndex) {
			TimelineCellContainer element = timelineView.ResourcesCellContainers[resourceIndex][0];
			TimeCellCollection cells = element.Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region TimelineViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<TimelineViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(TimelineViewGroupByResource timelineView, int resourceIndex) {
			TimelineCellContainer element = timelineView.ResourcesCellContainers[resourceIndex][0];
			TimeCellCollection cells = element.Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region TimelineViewInfoBase
	public abstract class TimelineViewInfoBase : ViewInfoBase<TimelineCellContainer> {
		const int defaultSelectionBarHeight = 15;
		#region Fields
		TimelineHeader header;
		TimelineSelectionBar selectionBar;
		int selectionBarHeight;
		#endregion
		protected TimelineViewInfoBase(TimelineView view)
			: base(view) {
		}
		#region Properties
		public new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal int IntervalCount { get { return View.IntervalCount; } }
		public TimelineHeader Header {
			get { return header; }
			private set {
				header = value;
			}
		}
		public TimelineSelectionBar SelectionBar { get { return selectionBar; } }
		public int SelectionBarHeight {
			get { return selectionBarHeight; }
			set {
				if (selectionBarHeight == value)
					return;
				this.selectionBarHeight = value;
			}
		}
		#endregion
		protected internal override CellContainerCollection GetContainersForUpdateSelection() {
			CellContainerCollection cellContainerCollection = base.GetContainersForUpdateSelection();
			if (ShouldShowSelectionBar())
				cellContainerCollection.Add(SelectionBar);
			return cellContainerCollection;
		}
		#region CreateAdditionalViewElements
		protected internal override IViewInfo CreateAdditionalViewElements() {
			this.selectionBarHeight = CalculateSelectionBarHeight();
			if (ShouldShowSelectionBar()) {
				SchedulerColorSchema schema = GetResourceColorSchema(ResourceBase.Empty, 0);
				ResourceBrushes brushes = new ResourceBrushes(schema);
				this.selectionBar = new TimelineSelectionBar(View, Header.Intervals, brushes);
				SelectionBar.Initialize();
			}
			return null;
		}
		protected internal virtual int CalculateSelectionBarHeight() {
			if (ShouldShowSelectionBar()) {
				SelectionBarOptions options = ((InnerTimelineView)View.InnerView).SelectionBar;
				return options.Height == 0 ? defaultSelectionBarHeight : options.Height;
			}
			return 0;
		}
		protected internal bool ShouldShowSelectionBar() {
			SelectionBarOptions options = ((InnerTimelineView)View.InnerView).SelectionBar;
			return options.Visible;
		}
		#endregion
		#region Create
		public override void Create() {
			InnerTimelineView innerView = (InnerTimelineView)View.InnerView;
			this.header = new TimelineHeader(this.View, innerView.ActualScales, innerView.VisibleStart, View.IntervalCount);
			base.Create();
		}
		#endregion
		#region CreateResourceCellContainers
		protected internal override AssignableCollection<TimelineCellContainer> CreateResourceCellContainers(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			AssignableCollection<TimelineCellContainer> result = new AssignableCollection<TimelineCellContainer>();
			result.Add(CreateCellContainer(View.InnerVisibleIntervals[0], resource, brushes));
			return result;
		}
		#endregion
		#region CreateCellContainer
		protected internal override TimelineCellContainer CreateCellContainer(TimeInterval interval, Resource resource, ResourceBrushes brushes) {
			TimelineCellContainer cells = new TimelineCellContainer(View, Header.Intervals, resource, View.Control.InnerControl.WorkDays, brushes);
			cells.Initialize();
			return cells;
		}
		#endregion
		protected internal override SingleResourceViewInfo CreateSingleResourceViewInfo() {
			return new SingleResourceViewInfo(this.View);
		}
	}
	#endregion
	#region TimelineViewGroupByNone
	public class TimelineViewGroupByNone : TimelineViewInfoBase {
		public TimelineViewGroupByNone(TimelineView view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override ResourceHeaderBase CreateResourceHeader(Resource resource, ResourceBrushes brushes) {
			return null;
		}
	}
	#endregion
	#region TimelineViewGroupByDate
	public class TimelineViewGroupByDate : TimelineViewInfoBase {
		public TimelineViewGroupByDate(TimelineView view)
			: base(view) {
		}
	}
	#endregion
	#region TimelineViewGroupByResource
	public class TimelineViewGroupByResource : TimelineViewGroupByDate {
		public TimelineViewGroupByResource(TimelineView view)
			: base(view) {
		}
	}
	#endregion
	public abstract class TimelineCellContainerBase : TimeCellContainerBase {
		#region Fields
		readonly TimeIntervalCollection intervals;
		#endregion
		protected TimelineCellContainerBase(TimelineView view, TimeIntervalCollection intervals, Resource resource, ResourceBrushes brushes)
			: base(view, intervals.Interval, resource, brushes) {
			Guard.ArgumentNotNull(intervals, "intervals");
			Guard.ArgumentPositive(intervals.Count, "intervals.Count");
			this.intervals = intervals;
		}
		#region Properties
		public TimeIntervalCollection Intervals { get { return intervals; } }
		#endregion
		public override AssignableSchedulerViewCellBaseCollection<TimeCell> CreateCellContainerCells() {
			return new TimeCellCollection();
		}
		protected internal override void PopulateCellCollection(SchedulerViewBase view) {
			int count = Intervals.Count;
			for (int i = 0; i < count; i++) {
				SingleTimelineCell cell = CreateTimeCell(intervals[i], Resource);
				InitializeCell(cell);
				Cells.Add(cell);
			}
		}
		protected virtual SingleTimelineCell CreateTimeCell(TimeInterval interval, Resource resource) {
			SingleTimelineCell cell = new SingleTimelineCell(interval, resource, Brushes);
			return cell;
		}
		protected override bool FitAppointmentControl(AppointmentControl control) {
			TimeInterval interval = new TimeInterval(Intervals.Start, Intervals.End);
			return interval.Contains(control.ViewInfo.Interval) && ResourceBase.InternalMatchIds((object)control.ViewInfo.Resource.Id, (object)this.Resource.Id);
		}
		protected internal abstract void InitializeCell(SingleTimelineCell cell);
	}
	public class TimelineCellContainer : TimelineCellContainerBase {
		WorkDaysCollection workDays;
		public TimelineCellContainer(TimelineView view, TimeIntervalCollection intervals, Resource resource, WorkDaysCollection workDays, ResourceBrushes brushes)
			: base(view, intervals, resource, brushes) {
			Guard.ArgumentNotNull(workDays, "workDays");
			Guard.ArgumentNotNull(view, "view");
			this.workDays = workDays;
		}
		public new TimelineWorkTimeCalculator WorkTimeCalculator { get { return (TimelineWorkTimeCalculator)base.WorkTimeCalculator; } }
		public new TimelineView View { get { return (TimelineView)base.View; } }
		InnerTimelineView InnerView { get { return (InnerTimelineView)View.InnerView; } }
		public WorkDaysCollection WorkDays {
			get { return workDays; }
			private set {
				if (object.Equals(WorkDays, value))
					return;
				workDays = value;
			}
		}
		protected internal override void InitializeCell(SingleTimelineCell cell) {
			cell.IsWorkTime = WorkTimeCalculator.CalcIsWorkTime(cell);
		}
		protected override WorkTimeInfoCalculatorBase CreateWorkTimeCalculator() {
			return new TimelineWorkTimeCalculator(InnerView, WorkDays);
		}
	}
	#region TimelineSelectionBar
	public class TimelineSelectionBar : TimelineCellContainerBase {
		public TimelineSelectionBar(TimelineView view, TimeIntervalCollection intervals, ResourceBrushes brushes)
			: base(view, intervals, ResourceBase.Empty, brushes) {
		}
		protected override SingleTimelineCell CreateTimeCell(TimeInterval interval, Resource resource) {
			return new TimelineSelectionBarCell(interval, Brushes);
		}
		protected internal override void InitializeCell(SingleTimelineCell cell) {
		}
		protected override WorkTimeInfoCalculatorBase CreateWorkTimeCalculator() {
			return null;
		}
	}
	#endregion
	#region SingleTimelineCell
	public class SingleTimelineCell : TimeCell {
		public SingleTimelineCell(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
		}
		public override SchedulerHitTest HitTestType {
			get {
				return base.HitTestType;
			}
		}
	}
	#endregion
	#region TimelineCellCollection
	public class TimelineCellCollection : TimeCellCollection, ITimeIntervalCollection {
		#region ITimeIntervalCollection Members
		ITimeCell ITimeIntervalCollection.this[int index] { get { return this[index]; } }
		#endregion
	}
	#endregion
	#region TimelineHeader
	public class TimelineHeader {
		#region Fields
		WebTimeScaleHeaderLevelCollection headerLevels;
		TimeScaleHeaderLevel baseLevel;
		TimeIntervalCollection intervals;
		EmptyTableCellCollection upperHeaders;
		readonly TimelineView view;
		#endregion
		public TimelineHeader(TimelineView view, TimeScaleCollection scales, DateTime start, int intervalCount) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			if (scales == null)
				Exceptions.ThrowArgumentNullException("scales");
			if (scales.Count == 0)
				Exceptions.ThrowArgumentException("scales.Count", scales.Count);
			TimeScaleLevelsCalculator calculator = CreateCalculator();
			TimeScaleLevelCollection levels = calculator.Calculate(scales, start, intervalCount);
			this.baseLevel = CreateBaseLevel(levels);
			this.intervals = baseLevel.Level.Intervals;
			this.upperHeaders = new EmptyTableCellCollection();
			WebTimeScaleHeaderLevelCollection upperLevels = CreateUpperLevels(levels, BaseLevel);
			this.headerLevels = CreateHeaderLevels(BaseLevel, upperLevels);
		}
		#region Properties
		public TimelineView View { get { return view; } }
		public WebTimeScaleHeaderLevelCollection HeaderLevels {
			get { return headerLevels; }
			private set {
				headerLevels = value;
			}
		}
		public TimeScaleHeaderLevel BaseLevel {
			get { return baseLevel; }
			private set {
				baseLevel = value;
			}
		}
		public TimeIntervalCollection Intervals {
			get { return intervals; }
			private set {
				intervals = value;
			}
		}
		#endregion
		protected internal virtual TimeScaleLevelsCalculator CreateCalculator() {
			return new TimeScaleLevelsCalculator();
		}
		protected internal virtual WebTimeScaleHeaderLevelCollection CreateHeaderLevels(TimeScaleHeaderLevel baseLevel, WebTimeScaleHeaderLevelCollection upperLevels) {
			WebTimeScaleHeaderLevelCollection result = new WebTimeScaleHeaderLevelCollection();
			result.AddRange(upperLevels);
			result.Add(baseLevel);
			return result;
		}
		protected internal virtual TimeScaleHeaderLevel CreateBaseLevel(TimeScaleLevelCollection levels) {
			int count = levels.Count;
			XtraSchedulerDebug.Assert(count != 0);
			TimeScaleHeaderLevel result = new TimeScaleHeaderLevel(this.View, levels[count - 1]);
			result.IsBaseLevel = true;
			TimeScaleHeaderCollection headers = result.Headers;
			int headerCount = headers.Count;
			for (int i = 0; i < headerCount; i++) {
				TimeScaleHeader header = headers[i];
				header.StartOffset = new SingleTimelineHeaderCellOffset(i, 0.0);
				header.EndOffset = new SingleTimelineHeaderCellOffset(i, 1.0);
			}
			return result;
		}
		protected internal virtual WebTimeScaleHeaderLevelCollection CreateUpperLevels(TimeScaleLevelCollection levels, TimeScaleHeaderLevel baseHeaderLevel) {
			WebTimeScaleHeaderLevelCollection result = new WebTimeScaleHeaderLevelCollection();
			int count = levels.Count;
			for (int i = 0; i < count - 1; i++) {
				TimeScaleHeaderLevel upperHeader = new TimeScaleHeaderLevel(this.View, levels[i]);
				upperHeader.CalculateRelativeOffset(baseHeaderLevel);
				result.Add(upperHeader);
			}
			return result;
		}
	}
	#endregion
	#region EmptyTableCell
	public class EmptyTableCell {
		public EmptyTableCell() {
		}
	}
	#endregion
	#region EmptyTableCellCollection
	public class EmptyTableCellCollection : List<EmptyTableCell> {
	}
	#endregion
	#region WebTimeScaleHeaderLevelCollection
	public class WebTimeScaleHeaderLevelCollection : AssignableCollection<TimeScaleHeaderLevel> {
	}
	#endregion
	#region WebTimeScaleHeaderCollection
	public class TimeScaleHeaderCollection : AssignableCollection<TimeScaleHeader> {
	}
	#endregion
	#region TimeScaleHeaderLevel
	public class TimeScaleHeaderLevel {
		TimeScaleLevel level;
		TimeScaleHeaderCollection headers;
		bool isBaseLevel;
		readonly TimelineView view;
		public TimeScaleHeaderLevel(TimelineView view, TimeScaleLevel level) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(level, "level");
			this.view = view;
			this.level = level;
			this.headers = CreateHeaders(Level);
		}
		public TimelineView View { get { return view; } }
		public TimeScaleLevel Level {
			get { return level; }
			private set { level = value; }
		}
		public TimeScaleHeaderCollection Headers {
			get { return headers; }
			set { headers = value; }
		}
		public bool IsBaseLevel {
			get { return isBaseLevel; }
			set { isBaseLevel = value; }
		}
		public void CalculateRelativeOffset(TimeScaleHeaderLevel baseLevel) {
			TimeScaleHeaderCollection baseHeadersCollection = baseLevel.Headers;
			int startIndex = 0;
			int headerCount = Headers.Count;
			List<SingleTimelineHeaderCellOffset> cellOffsets = new List<SingleTimelineHeaderCellOffset>();
			for (int headerIndex = 0; headerIndex < headerCount; headerIndex++) {
				TimeScaleHeader header = Headers[headerIndex];
				TimeScaleHeader baseHeader = FindHeaderByDate(baseHeadersCollection, header.Interval.Start, ref startIndex);
				cellOffsets.Add(CreateHeaderCellOffset(header, baseHeader, startIndex));
			}
			cellOffsets.Add(new SingleTimelineHeaderCellOffset(baseHeadersCollection.Count - 1, 1.0));
			for (int headerIndex = 0; headerIndex < headerCount; headerIndex++) {
				TimeScaleHeader header = Headers[headerIndex];
				header.StartOffset = cellOffsets[headerIndex];
				header.EndOffset = cellOffsets[headerIndex + 1];
			}
		}
		SingleTimelineHeaderCellOffset CreateHeaderCellOffset(TimeScaleHeader header, TimeScaleHeader baseHeader, int baseHeaderIndex) {
			TimeInterval headerInterval = header.Interval;
			TimeInterval baseHeaderInterval = baseHeader.Interval;
			double relativeOffset = ((double)headerInterval.Start.Ticks - baseHeaderInterval.Start.Ticks) / (baseHeaderInterval.Duration.Ticks);
			if (relativeOffset < 0)
				relativeOffset = 0;
			SingleTimelineHeaderCellOffset offset = new SingleTimelineHeaderCellOffset(baseHeaderIndex, relativeOffset);
			return offset;
		}
		protected TimeScaleHeader FindHeaderByDate(TimeScaleHeaderCollection headers, DateTime date, ref int startIndex) {
			for (int i = startIndex; i < headers.Count; i++) {
				TimeInterval interval = headers[i].Interval;
				if (interval.Contains(date) && (interval.End != date)) {
					startIndex = i;
					return headers[i];
				}
			}
			startIndex = 0;
			return headers[0];
		}
		protected internal virtual TimeScaleHeaderCollection CreateHeaders(TimeScaleLevel level) {
			TimeScaleHeaderCollection headerCollection = new TimeScaleHeaderCollection();
			TimeIntervalCollection intervals = level.Intervals;
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				TimeScaleHeader header = new TimeScaleHeader(this.View, interval, level.Scale);
				headerCollection.Add(header);
			}
			return headerCollection;
		}
	}
	#endregion
	#region TimeScaleHeader
	public class TimeScaleHeader {
		#region Fields
		TimeInterval interval;
		TimeScale scale;
		SingleTimelineHeaderCellOffset startOffset;
		SingleTimelineHeaderCellOffset endOffset;
		bool isAlternate;
		readonly TimelineView view;
		#endregion
		public TimeScaleHeader(TimelineView view, TimeInterval interval, TimeScale scale) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(scale, "scale");
			this.view = view;
			this.interval = interval;
			this.scale = scale;
			this.isAlternate = IsAlternateHeader();
		}
		#region Properties
		public TimelineView View { get { return view; } }
		public TimeInterval Interval {
			get { return interval; }
			private set {
				interval = value;
			}
		}
		public TimeScale Scale {
			get { return scale; }
			private set {
				scale = value;
			}
		}
		public bool IsAlternate { get { return isAlternate; } }
		public SingleTimelineHeaderCellOffset StartOffset {
			get { return startOffset; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("Offset");
				startOffset = value;
			}
		}
		public SingleTimelineHeaderCellOffset EndOffset {
			get { return endOffset; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("Offset");
				endOffset = value;
			}
		}
		public string Caption {
			get {
				return CalculateHeaderCaption();
			}
		}
		#endregion
		protected internal virtual bool IsAlternateHeader() {
			DateTime scaleStart = Scale.Floor(Interval.Start);
			TimeInterval interval = new TimeInterval(scaleStart, scale.GetNextDate(scaleStart));
			DateTime clientNow = View.Control.InnerControl.TimeZoneHelper.ToClientTime(DateTime.Now);
			return interval.IntersectsWithExcludingBounds(new TimeInterval(clientNow, TimeSpan.Zero));
		}
		protected internal virtual string CalculateHeaderCaption() {
			return Scale.FormatCaption(interval.Start, interval.End);
		}
	}
	#endregion
	#region SingleTimelineHeaderCellOffset
	public class SingleTimelineHeaderCellOffset {
		readonly int linkCellIndex;
		readonly double relativeOffset;
		public SingleTimelineHeaderCellOffset(int linkCellIndex, double relativeOffset) {
			this.linkCellIndex = linkCellIndex;
			this.relativeOffset = relativeOffset;
		}
		public int LinkCellIndex { get { return linkCellIndex; } }
		public double RelativeOffset { get { return relativeOffset; } }
	}
	#endregion
	#region TimelineSelectionBarCell
	public class TimelineSelectionBarCell : SingleTimelineCell {
		public TimelineSelectionBarCell(TimeInterval interval, ResourceBrushes brushes)
			: base(interval, ResourceBase.Empty, brushes) {
		}
		public override SchedulerHitTest HitTestType {
			get {
				return SchedulerHitTest.SelectionBarCell;
			}
		}
	}
	#endregion
}
