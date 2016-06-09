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
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public abstract class ViewControlBaseLayoutCalculator {
		ReportViewControlBase control;
		protected ViewControlBaseLayoutCalculator(ReportViewControlBase control) {
			this.control = control;
		}
		protected internal ViewInfoPainterBase Painter { get { return Control.Painter; } }
		protected internal ReportViewControlBase Control { get { return control; } }
		protected internal GraphicsCache Cache { get { return Control.Cache; } }
		protected internal ReportViewBase View { get { return Control.View; } }
	}
	public class ReportDayViewWorkTimeInfoCalculator : DayViewWorkTimeInfoCalculatorBase {
		ReportDayView dayView;
		public ReportDayViewWorkTimeInfoCalculator(ReportDayView dayView) {
			Guard.ArgumentNotNull(dayView, "dayView");
			this.dayView = dayView;
		}
		public ReportDayView DayView { get { return dayView; } set { dayView = value; } }
		protected internal override TimeOfDayInterval WorkTime {
			get {
				TimeOfDayIntervalCollection workTimes = DayView.GetWorkTime(TimeInterval.Empty, ResourceBase.Empty);
				return new TimeOfDayInterval(workTimes.Start, workTimes.End);
			}
		}
		public override WorkDaysCollection WorkDays { get { return DayView.GetWorkDays(); } }
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return DayView.GetWorkTime(interval, resource);
		}
		protected internal override bool IsWorkTime(TimeInterval interval, TimeOfDayIntervalCollection workTimes) {
			if (interval.Duration > TimeSpan.FromDays(1))
				return false;
			if (!WorkDays.IsWorkDay(interval.Start))
				return false;
			return IsWorkTimeCore(interval, workTimes);
		}
	}
	public class ReportTimelineWorkTimeCalculator : TimelineWorkTimeCalculatorBase {
		ReportTimelineView view;
		public ReportTimelineWorkTimeCalculator(ReportTimelineView view, WorkDaysCollection workDays)
			: base(workDays) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public ReportTimelineView View { get { return view; } set { view = value; } }
		protected internal override TimeOfDayInterval WorkTime {
			get {
				TimeOfDayIntervalCollection workTimes = View.GetWorkTime(TimeInterval.Empty, ResourceBase.Empty);
				return new TimeOfDayInterval(workTimes.Start, workTimes.End);
			}
		}
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return View.GetWorkTime(interval, resource);
		}
		protected internal override bool IsWorkTime(TimeInterval interval, TimeOfDayIntervalCollection workTimes) {
			if (interval.Duration > TimeSpan.FromDays(1))
				return false;
			if (!WorkDays.IsWorkDay(interval.Start))
				return false;
			return IsWorkTimeCore(interval, workTimes);
		}
	}
	public class DayViewTimeRulerLayoutCalculator : TimeRulerLayoutCalculatorBase {
		public DayViewTimeRulerLayoutCalculator(GraphicsCache cache, DayViewTimeRuler control, TimeRulerPainter painter)
			: base(cache, control, painter) {
		}
		protected internal DayViewTimeRuler Control { get { return (DayViewTimeRuler)TimeRulerSupport; } }
		public void CalculateLayout(ControlLayoutInfo info) {
			Control.TimeRulerViewInfos.Clear();
			XtraSchedulerDebug.Assert(info.HorizontalAnchors.Count == 0);
			AnchorCollection vAnchors = info.VerticalAnchors;
			int count = vAnchors.Count;
			for (int i = 0; i < count; i++)
				CalculateLayoutVerticalAnchor(vAnchors[i], info.ControlPrintBounds);
		}
		protected internal virtual void CalculateLayoutVerticalAnchor(AnchorBase verticalAnchor, Rectangle controlPrintBounds) {
			ITimeRulerFormatStringService formatStringProvider = TimeRulerSupport.GetFormatStringProvider();
			Rectangle rulerBounds = CalculateRulerBounds(verticalAnchor, controlPrintBounds);
			TimeRulerViewInfo viewInfo = CalcRulerPreliminaryLayout(Control.TimeRuler, rulerBounds, formatStringProvider);
			CalcRulerLayout(viewInfo, verticalAnchor);
			Control.TimeRulerViewInfos.Add(viewInfo);
		}
		protected internal virtual Rectangle CalculateRulerBounds(AnchorBase verticalAnchor, Rectangle controlPrintBounds) {
			return Rectangle.FromLTRB(controlPrintBounds.Left, verticalAnchor.Bounds.Top, controlPrintBounds.Right, verticalAnchor.Bounds.Bottom);
		}
		protected internal virtual void CalcRulerLayout(TimeRulerViewInfo timeRuler, AnchorBase verticalAnchor) {
			DateTime[] actualTimes = CreateActualTimes(timeRuler, verticalAnchor);
			Rectangle[] rowsBounds = GetRowBounds(verticalAnchor, actualTimes.Length);
			CalcRulerLayoutCore(timeRuler, rowsBounds, actualTimes, true);
		}
		protected internal override int CalculateTimeRulerWidth(TimeRuler ruler, Rectangle availableBounds, TimeRulerViewInfo viewInfo) {
			return availableBounds.Width;
		}
		protected internal virtual DateTime[] CreateActualTimes(TimeRulerViewInfo ruler, AnchorBase verticalAnchor) {
			TimeIntervalCollection rowIntervals = GetRowIntervals(verticalAnchor);
			TimeSpan currentUtcOffset = TimeRulerSupport.TimeZoneHelper.ClientTimeZone.GetUtcOffset(rowIntervals.Interval.Start);
			TimeSpan targetUtcOffset = CalcTargetUtcOffset(ruler.Ruler, rowIntervals.Interval.Start);
			return CreateActualTimesCore(rowIntervals, currentUtcOffset, targetUtcOffset);
		}
		private static Rectangle[] GetRowBounds(AnchorBase verticalAnchor, int rowsCount) {
			return verticalAnchor.InnerAnchors.GetAnchorBounds(rowsCount);
		}
		protected internal virtual TimeIntervalCollection GetRowIntervals(AnchorBase verticalAnchor) {
			return verticalAnchor.InnerAnchors.GetAnchorIntervals();
		}
		protected internal virtual DateTime[] CreateActualTimesCore(TimeIntervalCollection rowIntervals, TimeSpan currentUtcOffset, TimeSpan targetUtcOffset) {
			int count = rowIntervals.Count;
			List<DateTime> rows = new List<DateTime>();
			for (int i = 0; i < count; i++) {
				TimeInterval interval = rowIntervals[i];
				if (!interval.Equals(TimeInterval.Empty))		
					rows.Add(CalcActualDate(interval.Start, currentUtcOffset, targetUtcOffset));
			}
			return rows.ToArray();
		}
	}
	#region ReportDayCellsCalculatorHelper
	public class ReportDayCellsCalculatorHelper {
		public TimeIntervalCollection CalculateRowIntervals(TimeOfDayInterval visibleTime, TimeSpan timeScale, bool visibleTimeSnapMode) {
			TimeIntervalCollection result = new ReportsTimeIntervalCollection();
			TimeOfDayInterval alignedVisibleTime = DayViewCellsCalculatorHelper.CreateAlignedVisibleTime(visibleTime, timeScale, visibleTimeSnapMode);
			int timeIntervalsCount = DayViewCellsCalculatorHelper.CalculateTimeIntervalsCount(alignedVisibleTime, timeScale);
			TimeSpan topRowTime = alignedVisibleTime.Start;
			for (int i = 0; i < timeIntervalsCount; i++) {
				TimeSpan endRowTime = DayViewCellsCalculatorHelper.GetValidEndRowTime(topRowTime + timeScale, visibleTime);
				result.Add(CreatePrintObject(topRowTime, endRowTime));
				topRowTime += timeScale;
			}
			return result;
		}
		internal TimeInterval CreatePrintObject(TimeSpan topRowTime, TimeSpan endRowTime) {
			DateTime start = new DateTime(topRowTime.Ticks);
			DateTime end = new DateTime(endRowTime.Ticks);
			return new TimeInterval(start, end);
		}
	}
	#endregion
	public abstract class CellsLayoutCalculatorBase : ViewControlBaseLayoutCalculator {
		protected CellsLayoutCalculatorBase(TimeCellsControlBase control)
			: base(control) {
		}
		protected internal new TimeCellsControlBase Control { get { return (TimeCellsControlBase)base.Control; } }
		protected internal virtual SchedulerViewCellContainerCollection CreateContainersCore(AnchorCollection containerAnchors, AnchorBase cellsAnchor) {
			SchedulerViewCellContainerCollection containers = new SchedulerViewCellContainerCollection();
			int count = containerAnchors.Count;
			for (int i = 0; i < count; i++) {
				AnchorBase containerAnchor = containerAnchors[i];
				Resource resource = GetCellContainerResource(cellsAnchor, containerAnchor);
				SchedulerViewCellContainer container = CreateCellContainer(resource, containerAnchor.Interval);
				containers.Add(container);
			}
			return containers;
		}
		protected internal virtual Resource GetCellContainerResource(AnchorBase cellsAnchor, AnchorBase containerAnchor) {
			if (containerAnchor.Resource == ResourceBase.Empty)
				return cellsAnchor.Resource;
			else
				return containerAnchor.Resource;
		}
		protected internal virtual SchedulerViewCellContainer CreateCellContainer(Resource resource, TimeInterval interval) {
			SchedulerColorSchema colorSchema = GetColorSchema(resource);
			SchedulerViewCellContainer container = CreateCellContainerInstance(resource, interval, colorSchema);
			return container;
		}
		protected internal abstract SchedulerViewCellContainer CreateCellContainerInstance(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema);
		protected internal virtual SchedulerColorSchema GetColorSchema(Resource resource) {
			return Control.SchedulerReport.ResourceColorSchemasCache.GetSchema(resource);
		}
		protected internal virtual Rectangle CalculateHorizontalContainerBounds(ISchedulerObjectAnchor containerAnchor, AnchorCollection cellsAnchors) {
			int cellsCount = cellsAnchors.Count;
			XtraSchedulerDebug.Assert(cellsCount != 0);
			int left = cellsAnchors[0].Bounds.Left;
			int right = cellsAnchors[cellsCount - 1].Bounds.Right;
			return Rectangle.FromLTRB(left, containerAnchor.Bounds.Top, right, containerAnchor.Bounds.Bottom);
		}
		protected internal virtual Rectangle CalculateVerticalContainerBounds(ISchedulerObjectAnchor containerAnchor, AnchorCollection cellsAnchors) {
			int cellCount = cellsAnchors.Count;
			XtraSchedulerDebug.Assert(cellCount > 0);
			int top = cellsAnchors[0].Bounds.Top;
			int bottom = cellsAnchors[cellCount - 1].Bounds.Bottom;
			return Rectangle.FromLTRB(containerAnchor.Bounds.Left, top, containerAnchor.Bounds.Right, bottom);
		}
		protected internal virtual DateTime[] GetWeekCellDates(DateTime weekStart, AnchorCollection cellAnchors) {
			int count = cellAnchors.Count;
			DateTime[] result = new DateTime[count];
			DateTime firstAnchor = cellAnchors[0].Interval.Start;
			for (int i = 0; i < count; i++) {
				long offset = cellAnchors[i].Interval.Start.Ticks - firstAnchor.Ticks;
				result[i] = weekStart.AddTicks(offset);
			}
			return result;
		}
	}
	public abstract class AutoHeightCellsLayoutCalculator : CellsLayoutCalculatorBase {
		readonly Size moreButtonSize;
		protected AutoHeightCellsLayoutCalculator(TimeCellsControlBase control)
			: base(control) {
			this.moreButtonSize = control.CalculateMoreButtonSize();
		}
		public Size MoreButtonSize { get { return moreButtonSize; } }
		public virtual CellsLayoutPreliminaryInfos CalculatePreliminaryLayout(ControlLayoutInfo info) {
			CellsLayoutPreliminaryInfos preliminaryResult = new CellsLayoutPreliminaryInfos();
			int count = info.VerticalAnchors.Count;
			for (int i = 0; i < count; i++) {
				CellsLayoutPreliminaryInfo containerInfo = CalculatePreliminaryLayoutVerticalAnchor(info.VerticalAnchors.GetAnchorInfo(i), info.HorizontalAnchors);
				preliminaryResult.Add(containerInfo);
			}
			return preliminaryResult;
		}
		internal virtual CellsLayoutPreliminaryInfo CalculatePreliminaryLayoutVerticalAnchor(AnchorInfo containerAnchor, AnchorCollection cellsAnchors) {
			CellsLayoutPreliminaryInfo info = CreateCellContainersVerticalAnchor(containerAnchor, cellsAnchors);
			CalculatePreliminaryContainers(info, cellsAnchors);
			return info;
		}
		internal virtual CellsLayoutPreliminaryInfo CreateCellContainersVerticalAnchor(AnchorInfo containerAnchor, AnchorCollection cellsAnchors) {
			CellsLayerInfos cellLayers = new CellsLayerInfos();
			int count = containerAnchor.InnerAnchors.Count;
			for (int i = 0; i < count; i++) {
				ReportCellsLayerInfo layerInfo = CreateSingleLayerCellContainters(containerAnchor.InnerAnchors[i], cellsAnchors);
				cellLayers.Add(layerInfo);
			}
			return new CellsLayoutPreliminaryInfo(containerAnchor, cellLayers);
		}
		protected internal virtual ReportCellsLayerInfo CreateSingleLayerCellContainters(AnchorBase containerAnchor, AnchorCollection cellsAnhors) {
			SchedulerViewCellContainerCollection containers = CreateCellContainersCore(containerAnchor, cellsAnhors);
			ReportCellsLayerInfo layerInfo = CreateLayerInfo();
			layerInfo.CellContainers.AddRange(containers);
			return layerInfo;
		}
		internal virtual SchedulerViewCellContainerCollection CreateCellContainersCore(AnchorBase containerAnchor, AnchorCollection cellsAnchors) {
			SchedulerViewCellContainerCollection containers = new SchedulerViewCellContainerCollection();
			int count = cellsAnchors.Count;
			for (int i = 0; i < count; i++) {
				AnchorBase cellsAnchor = cellsAnchors[i];
				Resource resource = GetCellContainerResource(cellsAnchor, containerAnchor);
				TimeInterval interval = GetCellContainerInterval(cellsAnchor, containerAnchor);
				SchedulerViewCellContainer container = CreateCellContainer(resource, interval);
				containers.Add(container);
			}
			return containers;
		}
		protected internal virtual void CalculatePreliminaryContainers(CellsLayoutPreliminaryInfo layoutInfo, AnchorCollection cellsAnchors) {
			int count = layoutInfo.CellLayers.Count;
			XtraSchedulerDebug.Assert(count == layoutInfo.ContainersAnchor.InnerAnchors.Count);
			for (int i = 0; i < count; i++) {
				bool firstLayer = i == 0;
				SchedulerViewCellContainerCollection containers = ((ReportCellsLayerInfo)layoutInfo.CellLayers[i]).CellContainers;
				InitializeCellContainersBorders(containers, layoutInfo.ContainersAnchor, firstLayer);
				CalculatePreliminaryContainersSingleLayer(containers, cellsAnchors);
			}
		}
		protected internal virtual void InitializeCellContainersBorders(SchedulerViewCellContainerCollection containers, AnchorInfo containersAnchor, bool isFirstLevel) {
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = containers[i];
				bool isTopLevelContainer = isFirstLevel && containersAnchor.IsFirstAnchor;
				container.HasLeftBorder = i > 0;
				container.HasRightBorder = false;
				container.HasBottomBorder = false;
				container.HasTopBorder = !isTopLevelContainer;
			}
		}
		protected internal virtual void CalculatePreliminaryContainersSingleLayer(SchedulerViewCellContainerCollection containers, AnchorCollection cellsAnchors) {
			int count = containers.Count;
			XtraSchedulerDebug.Assert(count == cellsAnchors.Count);
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = containers[i];
				AnchorBase cellsAnchor = cellsAnchors[i];
				container.Bounds = CalculatePreliminaryContainerBounds(cellsAnchor);
				CalculatePreliminaryContainer(container, cellsAnchor.InnerAnchors);
			}
		}
		protected internal virtual Rectangle CalculatePreliminaryContainerBounds(AnchorBase cellsAnchor) {
			int left = cellsAnchor.Bounds.Left;
			int right = cellsAnchor.Bounds.Right;
			return Rectangle.FromLTRB(left, 0, right, Int32.MaxValue);
		}
		public virtual SchedulerViewCellContainerCollection CalculateFinalContainers(CellsLayoutPreliminaryInfos preliminaryResult, AnchorCollection cellsAnhors) {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = preliminaryResult.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainerCollection containers = CalculateFinalContainersCore(preliminaryResult[i], cellsAnhors);
				result.AddRange(containers);
			}
			return result;
		}
		internal virtual SchedulerViewCellContainerCollection CalculateFinalContainersCore(CellsLayoutPreliminaryInfo layoutInfo, AnchorCollection cellsAnhors) {
			AnchorCollection innerAnchors = layoutInfo.ContainersAnchor.InnerAnchors;
			XtraSchedulerDebug.Assert(layoutInfo.CellLayers.Count == innerAnchors.Count);
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = layoutInfo.CellLayers.Count;
			for (int i = 0; i < count; i++) {
				ReportCellsLayerInfo layer = (ReportCellsLayerInfo)layoutInfo.CellLayers[i];
				CalculateFinalContainersSingleLayer(layer, innerAnchors[i]);
				result.AddRange(layer.CellContainers);
			}
			return result;
		}
		internal virtual void CalculateFinalContainersSingleLayer(ReportCellsLayerInfo layerInfo, AnchorBase containerAnchor) {
			int count = layerInfo.CellContainers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = layerInfo.CellContainers[i];
				container.Bounds = CalcualteFinalContainerBounds(containerAnchor, container.Bounds);
				CalculateFinalContainer(layerInfo, container);
			}
		}
		internal virtual Rectangle CalcualteFinalContainerBounds(AnchorBase containerAnchor, Rectangle preliminaryBounds) {
			return Rectangle.FromLTRB(preliminaryBounds.Left, containerAnchor.Bounds.Top, preliminaryBounds.Right, containerAnchor.Bounds.Bottom);
		}
		internal abstract void CalculatePreliminaryContainer(SchedulerViewCellContainer container, AnchorCollection cellsAnchors);
		internal abstract void CalculateFinalContainer(ReportCellsLayerInfo layerInfo, SchedulerViewCellContainer container);
		internal abstract ReportCellsLayerInfo CreateLayerInfo();
		protected abstract TimeInterval GetCellContainerInterval(AnchorBase cellsAnchor, AnchorBase containerAnchor);
	}
	public class TimelineCellsLayoutCalculator : AutoHeightCellsLayoutCalculator {
		public TimelineCellsLayoutCalculator(TimelineCells control)
			: base(control) {
		}
		protected internal new TimelineCells Control { get { return (TimelineCells)base.Control; } }
		protected internal override SchedulerViewCellContainer CreateCellContainerInstance(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema) {
			return new ReportTimelineCellContainer(this.Control, resource, interval, colorSchema);
		}
		internal override void CalculatePreliminaryContainer(SchedulerViewCellContainer container, AnchorCollection cellsAnchors) {
			ReportTimelineCellContainer timeLine = (ReportTimelineCellContainer)container;
			timeLine.CreateCellsByAnchors(cellsAnchors);
			timeLine.CalcLayout(cellsAnchors, Control.Painter.CellPainter);
		}
		internal override void CalculateFinalContainer(ReportCellsLayerInfo layerInfo, SchedulerViewCellContainer container) {
			ReportTimelineCellContainer timeLine = (ReportTimelineCellContainer)container;
			timeLine.RecalcVerticalLayout(Control.Painter.CellPainter);
		}
		internal override ReportCellsLayerInfo CreateLayerInfo() {
			AppointmentPainter painter = Control.Painter.AppointmentPainter;
			return new ReportCellsLayerInfo(MoreButtonSize.Height, painter.BottomPadding);
		}
		protected override TimeInterval GetCellContainerInterval(AnchorBase cellsAnchor, AnchorBase containerAnchor) {
			return cellsAnchor.InnerAnchors.GetAnchorIntervals().Interval;
		}
	}
	public class DayViewTimeCellsLayoutCalculator : CellsLayoutCalculatorBase {
		public DayViewTimeCellsLayoutCalculator(DayViewTimeCells control)
			: base(control) {
		}
		protected internal new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		protected internal new DayViewTimeCellsPainter Painter { get { return (DayViewTimeCellsPainter)base.Painter; } }
		protected internal new ReportDayView View { get { return (ReportDayView)base.View; } }
		public virtual SchedulerViewCellContainerCollection CalculatePreliminaryLayout(AnchorInfo cellsAnchorInfo, AnchorCollection containerAnchors) {
			SchedulerViewCellContainerCollection containers = CreateContainersCore(containerAnchors, cellsAnchorInfo.Anchor);
			CalculatePreliminaryCellContainers(containers, containerAnchors, cellsAnchorInfo);
			return containers;
		}
		public virtual void CalculateFinalLayout(SchedulerViewCellContainerCollection preliminaryContainers, AnchorBase cellsAnchor) {
			int count = preliminaryContainers.Count;
			for (int i = 0; i < count; i++)
				CalculateFinalColumn((DayViewColumn)preliminaryContainers[i], cellsAnchor);
		}
		protected internal virtual void CalculatePreliminaryCellContainers(SchedulerViewCellContainerCollection containers, AnchorCollection containersAnchors, AnchorInfo cellsAnchorInfo) {
			int count = containers.Count;
			XtraSchedulerDebug.Assert(count == containersAnchors.Count);
			for (int i = 0; i < count; i++)
				CalculatePreliminaryColumn(containers[i], containersAnchors[i], cellsAnchorInfo.InnerAnchors);
			if (Control.ActualShowExtraCells) {
				AppointmentBaseCollection appointments = GetAppointments(containers);
				TimeOfDayInterval visibleTime = GetColumnsVisibleTime(containers);
				DayViewCellsCalculatorHelper.CreatePreliminaryExtendedCells(containers, appointments, Control.ExtraCells.MinCount, visibleTime);
			}
			InitializeContainersBorders(containers, cellsAnchorInfo);
		}
		TimeOfDayInterval GetColumnsVisibleTime(SchedulerViewCellContainerCollection containers) {
			if (containers.Count == 0)
				return TimeOfDayInterval.Empty;
			DayViewColumn column = (DayViewColumn)containers[0];
			return GetColumnVisibleTime(column.VisibleInterval);
		}
		protected internal virtual TimeOfDayInterval GetColumnVisibleTime(TimeInterval columnInterval) {
			TimeSpan start = columnInterval.Start.TimeOfDay;
			TimeSpan end = columnInterval.End.TimeOfDay;
			if (end <= start)
				end = end.Add(TimeSpan.FromDays(1));
			return new TimeOfDayInterval(start, end);
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(SchedulerViewCellContainerCollection containers) {
			TimeInterval interval = GetCellsInterval(containers);
			ResourceBaseCollection resources = GetCellsResources(containers);
			return Control.GetAppointments(interval, resources);
		}
		protected internal virtual ResourceBaseCollection GetCellsResources(SchedulerViewCellContainerCollection containers) {
			ResourceBaseCollection resources = new ResourceBaseCollection();
			foreach (SchedulerViewCellContainer container in containers)
				resources.Add(container.Resource);
			return resources;
		}
		protected internal virtual TimeInterval GetCellsInterval(SchedulerViewCellContainerCollection containers) {
			TimeIntervalCollectionEx intervals = new TimeIntervalCollectionEx();
			foreach (SchedulerViewCellContainer container in containers)
				intervals.Add(container.Interval);
			return intervals.Interval;
		}
		protected internal override SchedulerViewCellContainer CreateCellContainerInstance(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema) {
			return new ReportDayViewColumn(resource, interval, colorSchema);
		}
		protected internal virtual void CalculatePreliminaryColumn(SchedulerViewCellContainer container, AnchorBase containerAnchor, AnchorCollection cellsAnchors) {
			DayViewColumn column = (DayViewColumn)container;
			CalculateWorkTime(column);
			CalculatePreliminaryColumnBounds(column, containerAnchor.Bounds);
			CreatePreliminaryAllDayAreaCell(column);
			CreatePreliminaryCells(column, cellsAnchors);
		}
		protected internal virtual void CalculatePreliminaryColumnBounds(DayViewColumn column, Rectangle anchorBounds) {
			int statusLineWidth = GetStatusLineWidth();
			Rectangle columnBounds = Rectangle.FromLTRB(anchorBounds.Left, 0, anchorBounds.Right, Int32.MaxValue);
			Rectangle cellsBounds = RectUtils.CutFromLeft(columnBounds, statusLineWidth);
			column.Bounds = columnBounds;
			column.CellsBounds = cellsBounds;
		}
		protected internal virtual void CreatePreliminaryAllDayAreaCell(DayViewColumn column) {
			if (!Control.ActualShowAllDayArea)
				return;
			AllDayAreaCell allDayAreaCell = new AllDayAreaCell();
			column.InitializeCell(allDayAreaCell, column.Interval);
			allDayAreaCell.Bounds = column.Bounds;
			column.AllDayAreaCell = allDayAreaCell;
		}
		protected internal virtual void CreatePreliminaryCells(DayViewColumn column, AnchorCollection cellsAnchors) {
			int count = cellsAnchors.Count;
			for (int i = 0; i < count; i++) {
				AnchorBase anchor = cellsAnchors[i];
				TimeInterval interval = CalculateCellInterval(column, anchor);
				SchedulerViewCellBase cell = column.CreateCell(interval);
				column.Cells.Add(cell);
			}
		}
		public virtual AnchorBase CalculateActualCellsAnchor(AnchorBase cellsAnchor, SchedulerViewCellContainerCollection cellContainers, AppointmentIntermediateViewInfoCollection allDayAppointments, int top) {
			int allDayAreaHeight = CalculateAllDayAreaHeight(allDayAppointments, cellsAnchor);
			int extendedCellsCount = GetExtendedCellsCount(cellContainers);
			if (Control.ShouldFitIntoBounds())
				return CalculateFitCellsAnchor(cellsAnchor, allDayAreaHeight, extendedCellsCount);
			return CalculatTileCellsAnchor(cellsAnchor, allDayAreaHeight, extendedCellsCount, top);
		}
		protected internal virtual int GetExtendedCellsCount(SchedulerViewCellContainerCollection cellContainers) {
			if (cellContainers.Count == 0 || !Control.ActualShowExtraCells)
				return 0;
			return ((DayViewColumn)cellContainers[0]).ExtendedCells.Count;
		}
		protected internal virtual AnchorBase CalculatTileCellsAnchor(AnchorBase cellsAnchor, int allDayAreaHeight, int extendedCellsCount, int actualTop) {
			AnchorBase result = cellsAnchor.Clone();
			Rectangle totalBounds = cellsAnchor.Bounds;
			int extendedCellsHeight = extendedCellsCount * cellsAnchor.InnerAnchors[0].Bounds.Height;
			totalBounds.Height = totalBounds.Height + allDayAreaHeight + extendedCellsHeight;
			totalBounds.Y = actualTop;
			result.Bounds = totalBounds;
			CalculateInnerCellAnchors(result, allDayAreaHeight, extendedCellsCount);
			return result;
		}
		protected internal virtual AnchorBase CalculateFitCellsAnchor(AnchorBase anchor, int allDayAreaHeight, int extendedCellsCount) {
			AnchorBase result = anchor.Clone();
			CalculateInnerCellAnchors(result, allDayAreaHeight, extendedCellsCount);
			return result;
		}
		protected internal virtual void CalculateInnerCellAnchors(AnchorBase anchor, int allDayAreaHeight, int extendedCellsCount) {
			AnchorCollection prevCellsAnchors = anchor.InnerAnchors;
			int cellsCount = prevCellsAnchors.Count;
			Rectangle cellsBounds = RectUtils.CutFromTop(anchor.Bounds, allDayAreaHeight);
			Rectangle[] totalAnchorsBounds = RectUtils.SplitVertically(cellsBounds, cellsCount + extendedCellsCount);
			AnchorCollection actualCellsAnchors = CalculateCellsAnchors(prevCellsAnchors, totalAnchorsBounds);
			AnchorCollection actualExtendedCellsAnchors = CalculateExtendedCellsAnchors(cellsCount, totalAnchorsBounds);
			anchor.InnerAnchors.Clear();
			anchor.InnerAnchors.AddRange(actualCellsAnchors);
			anchor.InnerAnchors.AddRange(actualExtendedCellsAnchors);
		}
		protected internal virtual AnchorCollection CalculateCellsAnchors(AnchorCollection prevCellsAnchors, Rectangle[] anchorBounds) {
			int cellsCount = prevCellsAnchors.Count;
			AnchorCollection result = new AnchorCollection();
			for (int i = 0; i < cellsCount; i++) {
				AnchorBase anchor = prevCellsAnchors[i].Clone();
				anchor.Bounds = anchorBounds[i];
				result.Add(anchor);
			}
			return result;
		}
		protected internal virtual AnchorCollection CalculateExtendedCellsAnchors(int cellsCount, Rectangle[] anchorBounds) {
			AnchorCollection result = new AnchorCollection();
			int totalAnchorsCount = anchorBounds.Length;
			for (int i = cellsCount; i < totalAnchorsCount; i++) {
				AnchorBase anchor = new VerticalAnchor();
				anchor.Bounds = anchorBounds[i];
				result.Add(anchor);
			}
			return result;
		}
		protected internal virtual int CalculateAllDayAreaHeight(AppointmentIntermediateViewInfoCollection allDayAppointments, AnchorBase cellsAnchor) {
			if (!Control.ActualShowAllDayArea)
				return Control.Painter.HiddenAllDayAreaHeight;
			else {
				int minRowHeight = CalculateMinRowHeight(cellsAnchor);
				return DayViewCellsCalculatorHelper.CalculateAllDayAreaHeight(Painter.AppointmentPainter, allDayAppointments, minRowHeight);
			}
		}
		protected internal virtual int CalculateMinRowHeight(AnchorBase cellsAnchor) {
			AppointmentPainter aptPainter = Control.Painter.AppointmentPainter;
			int aptHeight = DayViewCellsCalculatorHelper.CalculateMinAppointmentHeight(Control.Appearance.Appointment, aptPainter.DefaultAppointmentImages, aptPainter, Control.Cache);
			if (cellsAnchor.InnerAnchors.Count == 0)
				return aptHeight;
			return Math.Min(cellsAnchor.InnerAnchors[0].Bounds.Height, aptHeight);
		}
		protected internal virtual void CalculateFinalColumn(DayViewColumn column, AnchorBase cellsAnchor) {
			CalculateFinalBounds(column, cellsAnchor);
			column.CalculateCellBorders(Control.Painter.CellPainter);
			column.CalculateFinalAppearance(View.PrintAppearance, column.ColorSchema);
		}
		protected internal virtual void CalculateWorkTime(DayViewColumn column) {
			ReportDayViewWorkTimeInfoCalculator workTimeInfoCalculator = new ReportDayViewWorkTimeInfoCalculator(View);
			WorkTimeInfo workTimeInfo = workTimeInfoCalculator.CalcWorkTimeInfo(column.Interval, column.Resource);
			column.IsWorkDay = workTimeInfo.IsWorkDay;
			column.WorkTimes.Clear();
			column.WorkTimes.AddRange(workTimeInfo.WorkTimes);
		}
		protected internal virtual AnchorCollection GetCellsAnchors(DayViewColumn column, AnchorBase cellAnchor) {
			AnchorCollection innerAnchors = cellAnchor.InnerAnchors;
			int cellsCount = column.Cells.Count;
			AnchorCollection result = new AnchorCollection();
			for (int i = 0; i < cellsCount; i++)
				result.Add(innerAnchors[i]);
			return result;
		}
		protected internal virtual AnchorCollection GetExtendedCellsAnchors(DayViewColumn column, AnchorBase anchor) {
			AnchorCollection innerAnchors = anchor.InnerAnchors;
			int cellsCount = column.Cells.Count;
			int anchorsCount = innerAnchors.Count;
			XtraSchedulerDebug.Assert(cellsCount + column.ExtendedCells.Count == anchorsCount);
			AnchorCollection result = new AnchorCollection();
			for (int i = cellsCount; i < anchorsCount; i++)
				result.Add(innerAnchors[i]);
			return result;
		}
		protected internal virtual void CalculateFinalBounds(DayViewColumn column, AnchorBase cellAnchor) {
			CalculateFinalColumnBounds(column, cellAnchor);
			CalculateCellsBounds(column, cellAnchor);
			CalculateExtendedCellsBounds(column, cellAnchor);
			CalculateAllDayAreaCellBounds(column);
			CalculateStatusLineBounds(column);
		}
		protected internal virtual void CalculateCellsBounds(DayViewColumn column, AnchorBase cellAnchor) {
			AnchorCollection cellsAnhors = GetCellsAnchors(column, cellAnchor);
			CalculateCellsBoundsCore(column.Cells, column.CellsBounds, cellsAnhors);
		}
		protected internal virtual void CalculateExtendedCellsBounds(DayViewColumn column, AnchorBase cellAnchor) {
			AnchorCollection extendedCellsAnhors = GetExtendedCellsAnchors(column, cellAnchor);
			CalculateCellsBoundsCore(column.ExtendedCells, column.Bounds, extendedCellsAnhors);
		}
		protected internal virtual void CalculateCellsBoundsCore(SchedulerViewCellBaseCollection cells, Rectangle cellsBounds, AnchorCollection cellsAnchors) {
			XtraSchedulerDebug.Assert(cells.Count == cellsAnchors.Count);
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				Rectangle anchorBounds = cellsAnchors[i].Bounds;
				cells[i].Bounds = new Rectangle(cellsBounds.Left, anchorBounds.Top, cellsBounds.Width, anchorBounds.Height);
			}
		}
		protected internal virtual void CalculateFinalColumnBounds(DayViewColumn column, AnchorBase cellAnchor) {
			AnchorCollection cellsAnchors = GetCellsAnchors(column, cellAnchor);
			XtraSchedulerDebug.Assert(cellsAnchors.Count > 0);
			int columnTop = cellAnchor.Bounds.Top;
			int columnBottom = cellAnchor.Bounds.Bottom;
			int cellsTop = cellsAnchors[0].Bounds.Top;
			int cellsBottom = cellsAnchors[cellsAnchors.Count - 1].Bounds.Bottom;
			Rectangle preliminaryColumnBounds = column.Bounds;
			Rectangle preliminaryCellsBounds = column.CellsBounds;
			column.Bounds = Rectangle.FromLTRB(preliminaryColumnBounds.Left, columnTop, preliminaryColumnBounds.Right, columnBottom);
			column.CellsBounds = Rectangle.FromLTRB(preliminaryCellsBounds.Left, cellsTop, preliminaryCellsBounds.Right, cellsBottom);
		}
		protected internal virtual void CalculateAllDayAreaCellBounds(DayViewColumn column) {
			if (Control.ActualShowAllDayArea) {
				int allDayAreaHeight = column.CellsBounds.Top - column.Bounds.Top;
				column.AllDayAreaCell.Bounds = RectUtils.GetTopSideRect(column.Bounds, allDayAreaHeight);
			}
		}
		protected internal virtual void CalculateStatusLineBounds(DayViewColumn column) {
			int statusLineWidth = GetStatusLineWidth();
			Rectangle cellBounds = column.CellsBounds;
			column.StatusLineBounds = new Rectangle(column.Bounds.Left, cellBounds.Top, statusLineWidth, cellBounds.Height);
		}
		protected internal virtual int GetStatusLineWidth() {
			return Control.StatusLineWidth == 0 ? Painter.CellPainter.StatusLineWidth : Control.StatusLineWidth;
		}
		protected internal virtual TimeInterval CalculateCellInterval(DayViewColumn column, AnchorBase anchor) {
			DateTime start = column.Interval.Start.Date.AddTicks(anchor.Interval.Start.Ticks);
			return new TimeInterval(start, anchor.Interval.Duration);
		}
		protected internal virtual void InitializeContainersBorders(SchedulerViewCellContainerCollection containers, AnchorInfo cellsAnchorInfo) {
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer column = containers[i];
				column.HasLeftBorder = i > 0;
				column.HasTopBorder = !cellsAnchorInfo.IsFirstAnchor;
				column.HasBottomBorder = false;
				column.HasRightBorder = false;
			}
		}
	}
	public class HorizontalWeekLayoutCalculator : AutoHeightCellsLayoutCalculator {
		protected internal new WeekControlPainterBase Painter { get { return (WeekControlPainterBase)base.Painter; } }
		public HorizontalWeekLayoutCalculator(HorizontalWeek control)
			: base(control) {
		}
		protected internal new HorizontalWeek Control { get { return (HorizontalWeek)base.Control; } }
		protected internal override SchedulerViewCellContainer CreateCellContainerInstance(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema) {
			return new ReportHorizontalSingleWeekViewInfo(Control, resource, interval, colorSchema);
		}
		protected internal override void InitializeCellContainersBorders(SchedulerViewCellContainerCollection containers, AnchorInfo containersAnchor, bool isFirstLevel) {
			base.InitializeCellContainersBorders(containers, containersAnchor, isFirstLevel);
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SingleWeekViewInfo container = (SingleWeekViewInfo)containers[i];
				container.FirstVisible = isFirstLevel;
			}
		}
		internal override void CalculatePreliminaryContainer(SchedulerViewCellContainer container, AnchorCollection cellsAnchors) {
			ReportHorizontalSingleWeekViewInfo weekInfo = (ReportHorizontalSingleWeekViewInfo)container;
			Rectangle[] cellsBounds = cellsAnchors.GetAnchorBounds();
			DateTime[] cellDates = GetWeekCellDates(weekInfo.Interval.Start, cellsAnchors);
			weekInfo.CalculatePreliminaryLayout(cellDates, cellsBounds, Cache, Painter.CellHeaderPainter);
		}
		internal override void CalculateFinalContainer(ReportCellsLayerInfo layerInfo, SchedulerViewCellContainer container) {
			ReportHorizontalSingleWeekViewInfo weekInfo = (ReportHorizontalSingleWeekViewInfo)container;
			HorizontalWeekCellsLayerInfo weekLayerInfo = (HorizontalWeekCellsLayerInfo)layerInfo;
			weekInfo.CalculateFinalLayout(weekLayerInfo.HeightInfo, Cache, Painter.CellHeaderPainter);
		}
		internal override ReportCellsLayerInfo CreateLayerInfo() {
			AppointmentPainter painter = Control.Painter.AppointmentPainter;
			return new HorizontalWeekCellsLayerInfo(MoreButtonSize.Height, painter.BottomPadding, Control.ActualCompressWeekend);
		}
		protected internal override DateTime[] GetWeekCellDates(DateTime weekStart, AnchorCollection cellAnchors) {
			DateTime[] result = base.GetWeekCellDates(weekStart, cellAnchors);
			if (Control.ActualCompressWeekend)
				result = AddSundayCell(result);
			return result;
		}
		protected internal virtual DateTime[] AddSundayCell(DateTime[] weekDates) {
			int cellIndex = DateTimeHelper.FindDayOfWeekIndex(weekDates, DayOfWeek.Saturday);
			if (cellIndex < 0)
				return weekDates;
			DateTime sunday = weekDates[cellIndex].AddDays(1);
			List<DateTime> dates = new List<DateTime>();
			dates.AddRange(weekDates);
			dates.Insert(cellIndex + 1, sunday);
			return dates.ToArray();
		}
		protected override TimeInterval GetCellContainerInterval(AnchorBase cellsAnchor, AnchorBase containerAnchor) {
			return containerAnchor.Interval;
		}
	}
	public class FullWeekLayoutCalculator : CellsLayoutCalculatorBase {
		public FullWeekLayoutCalculator(FullWeek control)
			: base(control) {
		}
		protected internal new WeekControlPainterBase Painter { get { return (WeekControlPainterBase)base.Painter; } }
		protected internal new FullWeek Control { get { return (FullWeek)base.Control; } }
		protected internal override SchedulerViewCellContainer CreateCellContainerInstance(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema) {
			return new ReportVerticalSingleWeekViewInfo(Control, resource, interval, colorSchema);
		}
		protected internal virtual Rectangle CalculateContainerBounds(ISchedulerObjectAnchor containerAnchor, AnchorCollection cellsAnchors) {
			return CalculateVerticalContainerBounds(containerAnchor, cellsAnchors);
		}
		protected internal virtual AnchorInfo GetContainersAnchor(AnchorInfo horizontalAnchor, AnchorInfo verticalAnchor) {
			return horizontalAnchor;
		}
		protected internal virtual AnchorInfo GetCellsAnchor(AnchorInfo horizontalAnchor, AnchorInfo verticalAnchor) {
			return verticalAnchor;
		}
		protected internal virtual void InitializeContainersBorders(SchedulerViewCellContainerCollection containers, AnchorInfo containersAnchorInfo, AnchorInfo cellsAnchorInfo) {
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SingleWeekViewInfo container = (SingleWeekViewInfo)containers[i];
				bool isFirstContainer = i == 0;
				container.HasLeftBorder = !containersAnchorInfo.IsFirstAnchor || !isFirstContainer;
				container.HasTopBorder = !cellsAnchorInfo.IsFirstAnchor;
				container.HasBottomBorder = false;
				container.HasRightBorder = false;
			}
		}
		public virtual SchedulerViewCellContainerCollection CalculateLayout(ControlLayoutInfo info) {
			int count = info.VerticalAnchors.Count;
			SchedulerViewCellContainerCollection cellContainers = new SchedulerViewCellContainerCollection();
			for (int i = 0; i < count; i++)
				cellContainers.AddRange(CreateContainersVerticalAnchor(info.HorizontalAnchors, info.VerticalAnchors.GetAnchorInfo(i)));
			return cellContainers;
		}
		protected internal virtual SchedulerViewCellContainerCollection CreateContainersVerticalAnchor(AnchorCollection horizontalAnchors, AnchorInfo verticalAnchorInfo) {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = horizontalAnchors.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CreateContainersHorizontalAnchor(horizontalAnchors.GetAnchorInfo(i), verticalAnchorInfo));
			return result;
		}
		protected internal virtual SchedulerViewCellContainerCollection CreateContainersHorizontalAnchor(AnchorInfo horizontalAnchor, AnchorInfo verticalAnchor) {
			AnchorInfo containersAnchor = GetContainersAnchor(horizontalAnchor, verticalAnchor);
			AnchorInfo cellsAnchor = GetCellsAnchor(horizontalAnchor, verticalAnchor);
			if (IsAnchorsEmpty(containersAnchor, cellsAnchor))
				return new SchedulerViewCellContainerCollection();
			SchedulerViewCellContainerCollection containers = CreateContainersCore(containersAnchor.InnerAnchors, cellsAnchor.Anchor);
			CalculateCellContainers(containers, containersAnchor, cellsAnchor);
			return containers;
		}
		protected internal virtual bool IsAnchorsEmpty(AnchorInfo containersAnchor, AnchorInfo cellsAnchor) {
			bool isEmptyContainerAnchor = containersAnchor.InnerAnchors.Count == 0;
			bool isEmptyCellAnchors = cellsAnchor.InnerAnchors.Count == 0;
			return isEmptyContainerAnchor || isEmptyCellAnchors;
		}
		protected internal virtual void CalculateCellContainers(SchedulerViewCellContainerCollection containers, AnchorInfo containersAnchorInfo, AnchorInfo cellsAnchorInfo) {
			int count = containers.Count;
			XtraSchedulerDebug.Assert(count == containersAnchorInfo.InnerAnchors.Count);
			InitializeContainersBorders(containers, containersAnchorInfo, cellsAnchorInfo);
			for (int i = 0; i < count; i++)
				CalculateCellContainerCore(containers[i], containersAnchorInfo.InnerAnchors[i], cellsAnchorInfo.InnerAnchors);
		}
		protected internal virtual void CalculateCellContainerCore(SchedulerViewCellContainer container, AnchorBase containerAnchor, AnchorCollection cellAnchors) {
			SingleWeekViewInfo weekInfo = (SingleWeekViewInfo)container;
			Rectangle containerBounds = CalculateContainerBounds(containerAnchor, cellAnchors);
			weekInfo.Bounds = containerBounds;
			Rectangle[] cellsBounds = cellAnchors.GetAnchorBounds();
			DateTime[] cellDates = GetWeekCellDates(weekInfo.Interval.Start, cellAnchors);
			weekInfo.CalcPreliminaryLayout(cellDates, Cache, Painter.CellHeaderPainter, false, false);
			weekInfo.CalcLayout(cellDates, cellsBounds, Cache, Painter.CellHeaderPainter);
		}
	}
	public class ReportHorizontalSingleWeekViewInfo : HorizontalSingleWeekViewInfoBase {
		public ReportHorizontalSingleWeekViewInfo(WeekCellsControlBase viewInfo, Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, interval.Start, colorSchema) {
			this.Interval = interval.Clone();
		}
		protected internal HorizontalWeek CellsControl { get { return (HorizontalWeek)ViewInfo; } }
		protected internal override bool CalculateCellHeaderAlternate(DateTime date) {
			return false;
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new MonthSingleWeekCell(ViewInfo.PaintAppearance.SingleWeekCellHeaderAppearance);
		}
		public void CalculatePreliminaryLayout(DateTime[] weekDates, Rectangle[] anchorBounds, GraphicsCache cache, SchedulerHeaderPainter painter) {
			Rectangle[] cellRects = CalcPreliminaryCellRectangles(weekDates, anchorBounds, painter.HorizontalOverlap);
			CalcPreliminaryLayout(weekDates, cache, painter, false, false);
			CalcLayoutCore(weekDates, cellRects, cache, painter);
		}
		protected internal virtual Rectangle[] CalcPreliminaryCellRectangles(DateTime[] dates, Rectangle[] anchorBounds, int horizontalOverlap) {
			Rectangle[] result = CalcCellRectangles(dates, anchorBounds, horizontalOverlap);
			int cellIndex = DateTimeHelper.FindDayOfWeekIndex(dates, DayOfWeek.Sunday);
			if (cellIndex > 0) {
				Rectangle sunday = result[cellIndex];
				result[cellIndex] = new Rectangle(sunday.Left, 0, sunday.Width, sunday.Height);
			}
			return result;
		}
		protected internal override int GetFirstDayIndex() {
			return (int)Interval.Start.DayOfWeek;
		}
		protected internal override bool ShouldCompressWeekend() {
			return CellsControl.ActualCompressWeekend;
		}
		protected internal override bool IsSeparatedCell(DayOfWeek cellDay) {
			return CellsControl.ActualCompressWeekend && cellDay == DayOfWeek.Saturday;
		}
		protected internal override void InitializeCellsBorders(bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek) {
			base.InitializeCellsBorders(groupSeparatorBeforeWeek, groupSeparatorAfterWeek);
			InitializeFirstCellBorders();
			InitializeLastCellBorders();
		}
		protected internal virtual void InitializeFirstCellBorders() {
			Cells[0].HasLeftBorder = HasLeftBorder;
		}
		protected internal virtual void InitializeLastCellBorders() {
			SchedulerViewCellBaseCollection cells = GetLastCells();
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				cells[i].HasRightBorder = HasRightBorder;
		}
		protected internal virtual SchedulerViewCellBaseCollection GetLastCells() {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			int count = Cells.Count;
			SingleWeekCellBase lastCell = (SingleWeekCellBase)Cells[count - 1];
			result.Add(lastCell);
			bool isSunday = lastCell.Date.DayOfWeek == DayOfWeek.Sunday;
			if (isSunday && (CellsControl.ActualCompressWeekend))
				result.Add(Cells[count - 2]);
			return result;
		}
		public virtual void CalculateFinalLayout(HorizontalWeekHeightInfo heightInfo, GraphicsCache cache, SchedulerHeaderPainter painter) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				CalcFinalCellBounds(heightInfo, Cells[i]);
			CalculateCellsHeaders(cache, painter);
			CalcCellsLayout();
		}
		internal virtual void CalcFinalCellBounds(HorizontalWeekHeightInfo heightInfo, SchedulerViewCellBase cell) {
			if (ShouldCompressWeekend())
				CalcFinalCompressedCellsBounds(heightInfo, cell);
			else
				CalcFinalCellBoundsCore(cell);
		}
		internal virtual void CalcFinalCompressedCellsBounds(HorizontalWeekHeightInfo heightInfo, SchedulerViewCellBase cell) {
			DayOfWeek day = cell.Interval.Start.DayOfWeek;
			switch (day) {
				case DayOfWeek.Saturday: CalcFinalSaturdayBounds(heightInfo, cell); break;
				case DayOfWeek.Sunday: CalcFinalSundayBounds(heightInfo, cell); break;
				default: CalcFinalCellBoundsCore(cell); break;
			}
		}
		internal virtual void CalcFinalSundayBounds(HorizontalWeekHeightInfo heightInfo, SchedulerViewCellBase cell) {
			int height = Bounds.Height - CalculateSaturdayHeight(heightInfo, Bounds.Height);
			Rectangle prevBounds = cell.Bounds;
			cell.Bounds = new Rectangle(prevBounds.Left, Bounds.Bottom - height, prevBounds.Width, height);
		}
		internal virtual void CalcFinalSaturdayBounds(HorizontalWeekHeightInfo heightInfo, SchedulerViewCellBase cell) {
			int height = CalculateSaturdayHeight(heightInfo, Bounds.Height);
			Rectangle prevBounds = cell.Bounds;
			cell.Bounds = new Rectangle(prevBounds.Left, Bounds.Top, prevBounds.Width, height);
		}
		internal virtual int CalculateSaturdayHeight(HorizontalWeekHeightInfo heightInfo, int containerHeight) {
			return IsAutoHeight() ? CalculateAutoHeightSaturdayHeight(heightInfo, containerHeight) : containerHeight / 2;
		}
		internal virtual bool IsAutoHeight() {
			return CellsControl.ActualCanGrow || CellsControl.ActualCanGrow;
		}
		internal virtual int CalculateAutoHeightSaturdayHeight(HorizontalWeekHeightInfo heightInfo, int containerHeight) {
			int average = containerHeight / 2;
			int available = Math.Max(0, containerHeight - heightInfo.SundayMinHeight);
			int headerHeight = CalculateCellHeaderHeight();
			if (available <= headerHeight)
				return average;
			int result = Math.Min(available, heightInfo.SaturdayHeight);
			int freeSpace = containerHeight - result;
			if ((result > average) || (freeSpace < heightInfo.SundayHeight))
				return result;
			return Math.Min(average, containerHeight - heightInfo.SundayHeight);
		}
		internal virtual void CalcFinalCellBoundsCore(SchedulerViewCellBase cell) {
			Rectangle prevBounds = cell.Bounds;
			cell.Bounds = Rectangle.FromLTRB(prevBounds.Left, Bounds.Top, prevBounds.Right, Bounds.Bottom);
		}
		internal virtual int CalculateCellHeaderHeight() {
			return Cells.Count == 0 ? 0 : ((HorizontalSingleWeekCell)Cells[0]).Header.Bounds.Height;
		}
	}
	public class ReportVerticalSingleWeekViewInfo : VerticalSingleWeekViewInfoBase {
		public ReportVerticalSingleWeekViewInfo(ISupportWeekCells viewInfo, Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(viewInfo, resource, interval.Start, colorSchema) {
			this.Interval = interval;
		}
		protected internal override bool CalculateCellHeaderAlternate(DateTime date) {
			return false;
		}
		protected internal override void InitializeCellHeaderBorders(SingleWeekCellBase cell, bool groupSeparatorBeforeWeek, bool groupSeparatorAfterWeek, int columnIndex) {
			bool isLastColumn = columnIndex > 0;
			cell.Header.HasLeftBorder = HasLeftBorder || isLastColumn;
			cell.Header.HasTopBorder = !IsFirstRow(cell) || HasTopBorder;
			cell.Header.HasBottomBorder = true;
			cell.Header.HasRightBorder = false;
			cell.Header.HasSeparator = false;
		}
		protected internal override int GetFirstDayIndex() {
			return (int)Interval.Start.DayOfWeek;
		}
	}
	#region ReportTimelineCellContainer
	public class ReportTimelineCellContainer : TimelineBase {
		public ReportTimelineCellContainer(TimelineCells cellsControl, Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(cellsControl, resource, interval, colorSchema) {
		}
		public new TimelineCells TimelineSupport { get { return (TimelineCells)base.TimelineSupport; } }
		public virtual void RecalcVerticalLayout(BorderObjectPainter cellsPainter) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				RecalcVerticalCellBounds(Cells[i]);
			CalculateCellsBorders(cellsPainter);
		}
		internal virtual void RecalcVerticalCellBounds(SchedulerViewCellBase cell) {
			Rectangle prevBounds = cell.Bounds;
			cell.Bounds = Rectangle.FromLTRB(prevBounds.Left, Bounds.Top, prevBounds.Right, Bounds.Bottom);
		}
		protected internal override TimelineWorkTimeCalculatorBase CreateWorkTimeCalculator() {
			return new ReportTimelineWorkTimeCalculator(TimelineSupport.View, WorkDays);
		}
		protected internal override void InitializeCellsBorders() {
			base.InitializeCellsBorders();
			InitializeFirstCellBounds();
			InitializeLastCellBounds();
		}
		protected internal virtual void InitializeFirstCellBounds() {
			if (Cells.Count > 0)
				Cells[0].HasLeftBorder = HasLeftBorder;
		}
		protected internal virtual void InitializeLastCellBounds() {
			int count = Cells.Count;
			if (count > 0)
				Cells[count - 1].HasRightBorder = HasRightBorder;
		}
	}
	#endregion
	public class ReportDayViewColumn : DayViewColumn {
		public ReportDayViewColumn(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(resource, interval, colorSchema) {
		}
		protected internal override void InitializeAllDayCellBorders() {
			base.InitializeAllDayCellBorders();
			AllDayAreaCell.HasRightBorder = HasRightBorder;
			AllDayAreaCell.HasTopBorder = HasTopBorder;
			AllDayAreaCell.HasBottomBorder = true;
		}
		protected internal override void InitializeExtendedCellBorders(SchedulerViewCellBase cell) {
			base.InitializeExtendedCellBorders(cell);
			cell.HasLeftBorder = HasLeftBorder;
		}
		protected internal override void InitializeCellBorders() {
			base.InitializeCellBorders();
			SchedulerViewCellBase cell = GetLastCell();
			cell.HasBottomBorder = HasBottomBorder;
		}
		protected internal SchedulerViewCellBase GetLastCell() {
			int cellsCount = Cells.Count;
			XtraSchedulerDebug.Assert(cellsCount > 0);
			int extendedCellsCount = ExtendedCells.Count;
			if (extendedCellsCount > 0)
				return ExtendedCells[extendedCellsCount - 1];
			return Cells[cellsCount - 1];
		}
	}
	public class HorizontalWeekCellsLayerInfo : ReportCellsLayerInfo {
		bool compressWeekEnd;
		HorizontalWeekHeightInfo heightInfo;
		public HorizontalWeekCellsLayerInfo(int moreButtonsHeight, int bottomPadding, bool compressWeekEnd)
			: base(moreButtonsHeight, bottomPadding) {
			this.compressWeekEnd = compressWeekEnd;
		}
		internal bool CompressWeekEnd { get { return compressWeekEnd; } }
		public HorizontalWeekHeightInfo HeightInfo { get { return heightInfo; } }
		public override void CalculateHeight() {
			this.heightInfo = CalculateHeightInfo();
			this.ContentHeight = CalculateContentHeight();
			this.MinHeight = CalculateMinHeight();
		}
		internal virtual int CalculateContentHeight() {
			return CalculateLayerHeightCore(HeightInfo.WorkWeekHeight, HeightInfo.SundayHeight, HeightInfo.SaturdayHeight);
		}
		internal virtual int CalculateMinHeight() {
			int workWeekHeight = CalculateMinHeight(WeekDays.WorkDays);
			int sundayHeight = CalculateMinHeight(WeekDays.Sunday);
			int saturdayHeight = CalculateMinHeight(WeekDays.Saturday);
			return CalculateLayerHeightCore(workWeekHeight, sundayHeight, saturdayHeight);
		}
		internal virtual int CalculateLayerHeightCore(int workWeekHeight, int sundayHeight, int saturdayHeight) {
			int weekEndHeight;
			if (CompressWeekEnd)
				weekEndHeight = sundayHeight + saturdayHeight;
			else
				weekEndHeight = Math.Max(sundayHeight, saturdayHeight);
			return Math.Max(workWeekHeight, weekEndHeight);
		}
		internal virtual int CalculateMinHeight(WeekDays mask) {
			int headerHeight = CalcualteCellsHeaderHeight();
			if (FilterAppointmentViewInfos(mask).Count > 0)
				return headerHeight + MinHeight;
			else
				return headerHeight;
		}
		internal virtual HorizontalWeekHeightInfo CalculateHeightInfo() {
			int weekHeight = CalculateContentHeight(WeekDays.WorkDays);
			int saturdayHeight = CalculateContentHeight(WeekDays.Saturday);
			int sundayHeight = CalculateContentHeight(WeekDays.Sunday);
			int sundayMinHeight = CalculateMinHeight(WeekDays.Sunday);
			return new HorizontalWeekHeightInfo(weekHeight, saturdayHeight, sundayHeight, sundayMinHeight);
		}
		internal virtual int CalculateContentHeight(WeekDays mask) {
			AppointmentIntermediateViewInfoCollection viewInfos = FilterAppointmentViewInfos(mask);
			return CalculateContentHeight(viewInfos);
		}
		internal AppointmentIntermediateViewInfoCollection FilterAppointmentViewInfos(WeekDays mask) {
			AppointmentIntermediateViewInfoCollection result = new AppointmentIntermediateViewInfoCollection();
			int count = AppointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentIntermediateViewInfo intermediateViewInfo = AppointmentViewInfos[i];
				if (FilterAppointment(intermediateViewInfo.ViewInfo.Interval, mask))
					result.Add(intermediateViewInfo);
			}
			return result;
		}
		internal virtual bool FilterAppointment(TimeInterval aptInterval, WeekDays mask) {
			DateTime start = aptInterval.Start;
			while (start.Ticks < aptInterval.End.Ticks) {
				WeekDays aptStartDay = DateTimeHelper.ToWeekDays(start.DayOfWeek);
				if ((aptStartDay & mask) != 0)
					return true;
				start = start.AddDays(1);
			}
			return false;
		}
	}
	public delegate bool AppointmentViewInfoFilter(TimeInterval interval);
	public class HorizontalWeekHeightInfo {
		int workWeekHeight;
		int saturdayHeight;
		int sundayHeight;
		int sundayMinHeight;
		public HorizontalWeekHeightInfo() {
		}
		public HorizontalWeekHeightInfo(int workWeekHeight, int saturdayHeight, int sundayHeight, int sundayMinHeight) {
			this.workWeekHeight = workWeekHeight;
			this.saturdayHeight = saturdayHeight;
			this.sundayHeight = sundayHeight;
			this.sundayMinHeight = sundayMinHeight;
		}
		public int WorkWeekHeight { get { return workWeekHeight; } set { workWeekHeight = value; } }
		public int SaturdayHeight { get { return saturdayHeight; } set { saturdayHeight = value; } }
		public int SundayHeight { get { return sundayHeight; } set { sundayHeight = value; } }
		public int SundayMinHeight { get { return sundayMinHeight; } set { sundayMinHeight = value; } }
	}
}
