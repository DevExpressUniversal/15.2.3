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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Reporting {
	public abstract class AppointmentsLayoutStrategyBase {
		readonly TimeCellsControlBase control;
		protected AppointmentsLayoutStrategyBase(TimeCellsControlBase control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		protected internal TimeCellsControlBase Control { get { return control; } }		
	}
	public abstract class SimpleAppointmentsLayoutStrategy : AppointmentsLayoutStrategyBase {		
		readonly AppointmentBaseLayoutCalculator appointmentCalculator;
		readonly VisuallyContinuousCellsCalculatorBase continuosCellsCalculator;
		protected SimpleAppointmentsLayoutStrategy(TimeCellsControlBase control) : base(control) {			
			this.appointmentCalculator = CreateAppointmentCalculator();
			this.continuosCellsCalculator = CreateContinuosCellsCalculator();
			SubscribeContentCalculatorEvents();
		}
		~SimpleAppointmentsLayoutStrategy() {
			UnsubscribeContentCalculatorEvents();
		}
		protected internal bool AutoHeight { get { return Control.AppointmentDisplayOptions.AppointmentAutoHeight; } }
		protected internal AppointmentPainter AppointmentPainter { get { return Control.Painter.AppointmentPainter; } }
		protected internal AppointmentBaseLayoutCalculator AppointmentCalculator { get { return appointmentCalculator; } }
		protected internal VisuallyContinuousCellsCalculatorBase ContinuosCellsCalculator { get { return continuosCellsCalculator; } }
		internal virtual void SubscribeContentCalculatorEvents() {
			if (Control != null)
				Control.SubscribeAppointmentContentCalculatorEvents(AppointmentCalculator.ContentCalculator);
		}
		internal virtual void UnsubscribeContentCalculatorEvents() {
			if (Control != null && AppointmentCalculator != null)
				Control.UnsubscribeAppointmentContentCalculatorEvents(AppointmentCalculator.ContentCalculator);
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			return GetAppointmentsCore(cellsInfos);
		}
		protected internal virtual AppointmentBaseCollection GetFilteredAppointments(DevExpress.XtraScheduler.Native.ProcessorBase<Appointment> processor, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			AppointmentBaseCollection appointments = GetAppointmentsCore(cellsInfos);
			processor.Process(appointments);
			return (AppointmentBaseCollection)processor.DestinationCollection;
		}
		protected internal virtual AppointmentBaseCollection GetAppointmentsCore(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			ResourceBaseCollection resources = GetCellsResources(cellsInfos);
			TimeInterval interval = GetCellsInterval(cellsInfos);
			return Control.GetAppointments(interval, resources);
		}
		protected internal virtual TimeInterval GetCellsInterval(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			TimeIntervalCollectionEx intervals = new TimeIntervalCollectionEx();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++)
				intervals.Add(GetCellsIntervalCore(cellsInfos[i].CellsInfoCollection));
			return intervals.Interval;
		}
		protected internal virtual TimeInterval GetCellsIntervalCore(VisuallyContinuousCellsInfoCollection cellsInfos) {
			TimeIntervalCollectionEx intervals = new TimeIntervalCollectionEx();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++)
				intervals.Add(cellsInfos[i].VisibleInterval);
			return intervals.Interval;
		}
		protected internal virtual ResourceBaseCollection GetCellsResources(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++)
				result.Add(cellsInfos[i].Resource);
			return result;
		}
		protected internal abstract VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator();
		protected internal abstract AppointmentBaseLayoutCalculator CreateAppointmentCalculator();
	}
	public abstract class AutoHeightCellsAppointmentsLayoutStrategy : SimpleAppointmentsLayoutStrategy {
		protected internal new HorizontalAppointmentLayoutCalculator AppointmentCalculator { get { return (HorizontalAppointmentLayoutCalculator)base.AppointmentCalculator; } }
		AppointmentLayoutAutoHeightCellsHelper layoutHelper;
		internal AppointmentLayoutAutoHeightCellsHelper LayoutHelper { get { return layoutHelper; } }
		protected AutoHeightCellsAppointmentsLayoutStrategy(TimeCellsControlBase control)
			: base(control) {
			layoutHelper = CreateAppointmentLayoutHelper();
		}
		protected internal abstract AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper();
		public virtual void CalculatePreliminaryLayout(CellsLayoutPreliminaryInfos preliminaryResult) {
			int count = preliminaryResult.Count;
			for (int i = 0; i < count; i++)
				CalculatePreliminaryLayoutCore(preliminaryResult[i]);
		}
		internal virtual void CalculatePreliminaryLayoutCore(CellsLayoutPreliminaryInfo cellsLayoutInfo) {
			int count = cellsLayoutInfo.CellLayers.Count;
			for (int i = 0; i < count; i++)
				CalculatePreliminaryLayoutSingleLayer((ReportCellsLayerInfo)cellsLayoutInfo.CellLayers[i]);
		}
		internal virtual void CalculatePreliminaryLayoutSingleLayer(ReportCellsLayerInfo layer) {			
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = ContinuosCellsCalculator.Calculate(layer.CellContainers);
			AppointmentBaseCollection appointments = GetAppointments(cellsInfos);			
			AppointmentIntermediateViewInfoCollection intermediateViewInfos = LayoutHelper.CalculatePreliminaryLayout(appointments, cellsInfos).GetAppointmentViewInfos();
			layer.AppointmentViewInfos.AddRange(intermediateViewInfos);
			layer.CalculateHeight();
		}
		public virtual AppointmentsLayoutResult CalculateFinalLayout(CellsLayoutPreliminaryInfos preliminaryResult) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			int count = preliminaryResult.Count;
			for (int i = 0; i < count; i++)
				result.Merge(CalculateFinalLayoutCore(preliminaryResult[i]));
			return result;
		}
		internal virtual AppointmentsLayoutResult CalculateFinalLayoutCore(CellsLayoutPreliminaryInfo cellsLayoutInfo) {
			return LayoutHelper.CalculateFinalLayout(cellsLayoutInfo.CellLayers, new SchedulerViewCellContainerCollection(), new ResourceVisuallyContinuousCellsInfosCollection());
		}
	}
	public abstract class FixedHeightCellsAppointmentsLayoutStrategy : SimpleAppointmentsLayoutStrategy {
		protected FixedHeightCellsAppointmentsLayoutStrategy(TimeCellsControlBase control)
			: base(control) {
		}
		public virtual AppointmentsLayoutResult CalculateLayout(SchedulerViewCellContainerCollection cellContainers) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = ContinuosCellsCalculator.Calculate(cellContainers);
			AppointmentBaseCollection appointments = GetAppointments(cellsInfos);
			AppointmentCalculator.CalculateLayout(result, appointments, cellsInfos);
			return result;
		}		
	}
	public class DayViewDispatchAppointmentsLayoutStrategy : AppointmentsLayoutStrategyBase {
		DayViewAllDayAppointmentsLayoutStrategy allDayStrategy;
		DayViewTimeCellAppointmentsLayoutStrategy timeCellStratery;
		public DayViewDispatchAppointmentsLayoutStrategy(DayViewTimeCells control)
			: base(control) {
			this.allDayStrategy = new DayViewAllDayAppointmentsLayoutStrategy(control);
			this.timeCellStratery = new DayViewTimeCellAppointmentsLayoutStrategy(control);
		}
		protected internal new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		internal bool ShowAllDayArea { get { return Control.ActualShowAllDayArea; } }
		protected internal DayViewAllDayAppointmentsLayoutStrategy AllDayStrategy { get { return allDayStrategy; } }
		protected internal DayViewTimeCellAppointmentsLayoutStrategy TimeCellStratery { get { return timeCellStratery; } }
		public virtual AppointmentIntermediateViewInfoCollection CalculatePreliminaryLayout(SchedulerViewCellContainerCollection cellContainers) {
			if (ShowAllDayArea)
				return AllDayStrategy.CalculatePreliminaryLayout(cellContainers);
			else
				return new AppointmentIntermediateViewInfoCollection();
		}
		public virtual AppointmentsLayoutResult CalculateFinalLayout(AppointmentIntermediateViewInfoCollection aptViewInfos, SchedulerViewCellContainerCollection cellContainers) {
			DayViewAppointmentsLayoutResult result = new DayViewAppointmentsLayoutResult();
			AppointmentsLayoutResult timeCellsResult = TimeCellStratery.CalculateLayout(cellContainers);
			result.Merge(timeCellsResult);
			if (ShowAllDayArea) {
				AppointmentsLayoutResult allDayResult = AllDayStrategy.CalculateFinalLayout(aptViewInfos, cellContainers);
				result.Merge(allDayResult);
			}
			return result;
		}
	}
	public class DayViewAllDayAppointmentsLayoutStrategy : SimpleAppointmentsLayoutStrategy {
		AllDayAppointmentLayoutHelper layoutHelper;
		public DayViewAllDayAppointmentsLayoutStrategy(DayViewTimeCells control)
			: base(control) {
			this.layoutHelper = new AllDayAppointmentLayoutHelper(AppointmentCalculator);
		}
		protected internal new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		protected internal new HorizontalAppointmentLayoutCalculator AppointmentCalculator { get { return (HorizontalAppointmentLayoutCalculator)base.AppointmentCalculator; } }
		public virtual AppointmentIntermediateViewInfoCollection CalculatePreliminaryLayout(SchedulerViewCellContainerCollection cellContainers) {
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = ContinuosCellsCalculator.Calculate(cellContainers);
			AppointmentBaseCollection appointments = GetAppointments(cellsInfos);
			return layoutHelper.CalculatePreliminaryLayout(appointments, cellsInfos).GetAppointmentViewInfos();
		}
		public virtual AppointmentsLayoutResult CalculateFinalLayout(AppointmentIntermediateViewInfoCollection aptViewInfos, SchedulerViewCellContainerCollection cellContainers) {
			ReportCellsLayerInfo stubLayer = new ReportCellsLayerInfo(0, 0);
			stubLayer.AppointmentViewInfos.AddRange(aptViewInfos);
			CellsLayerInfos layerInfos = new CellsLayerInfos();
			layerInfos.Add(stubLayer);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = ContinuosCellsCalculator.Calculate(cellContainers);
			return layoutHelper.CalculateFinalLayout(layerInfos, cellContainers, cellsInfos);
		}		
		protected internal override AppointmentBaseCollection GetAppointments(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			if (Control.ActualShowAllAppointmentsAtTimeCells)
				return new AppointmentBaseCollection();
			else
				return GetAllDayAreaAppointments(cellsInfos);
		}
		protected internal virtual AppointmentBaseCollection GetAllDayAreaAppointments(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			return GetFilteredAppointments(new DayViewAllDayAppointmentsFilter(), cellsInfos);
		}
		protected internal override VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator() {
			return new DayViewAllDayVisuallyContinuousCellsCalculator();
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentCalculator() {
			DayViewAllDayAppointmentContentLayoutCalculator contentCalculator = new DayViewAllDayAppointmentContentLayoutCalculator(Control, AppointmentPainter);
			if (AutoHeight)
				return new DayViewAllDayAppointmentAutoHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
			else
				return new DayViewAllDayAppointmentFixedHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
		}
	}
	public class DayViewTimeCellAppointmentsLayoutStrategy : FixedHeightCellsAppointmentsLayoutStrategy {
		public DayViewTimeCellAppointmentsLayoutStrategy(DayViewTimeCells control)
			: base(control) {
		}
		protected internal new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		protected internal override AppointmentBaseCollection GetAppointments(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			if (Control.ActualShowAllAppointmentsAtTimeCells)
				return base.GetAppointments(cellsInfos);
			else
				return GetTimeCellsAppointments(cellsInfos);
		}
		protected internal virtual AppointmentBaseCollection GetTimeCellsAppointments(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			return GetFilteredAppointments(new DayViewTimeCellAppointmentsFilter(), cellsInfos);
		}
		protected internal override VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator() {
			return new DayViewTimeCellVisuallyContinuousCellsCalculator();
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentCalculator() {
			DayViewTimeCellAppointmentContentLayoutCalculator contentCalculator = new DayViewTimeCellAppointmentContentLayoutCalculator(Control, AppointmentPainter);
			return new ReportDayTimeCellsAppointmentLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
		}		
	}
	public class TimelineAppointmentsLayoutStrategy : AutoHeightCellsAppointmentsLayoutStrategy {
		public TimelineAppointmentsLayoutStrategy(TimelineCells control)
			: base(control) {
		}
		protected internal new TimelineCells Control { get { return (TimelineCells)base.Control; } }
		protected internal override AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper() {
			return new HorizontalAppointmentLayoutAutoHeightCellsHelper(AppointmentCalculator);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentCalculator() {
			TimelineAppointmentContentLayoutCalculator contentCalculator = new TimelineAppointmentContentLayoutCalculator(Control,  AppointmentPainter);
			if (AutoHeight)
				return new TimelineAppointmentAutoHeightLayoutCalculator(Control, contentCalculator,  Control.Cache, AppointmentPainter);
			else
				return new TimelineAppointmentFixedHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
		}
		protected internal override VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator() {
			return new TimelineVisuallyContinuousCellsCalculator();
		}
	}
	public class FullWeekAppointmentsLayoutStrategy : FixedHeightCellsAppointmentsLayoutStrategy {
		public FullWeekAppointmentsLayoutStrategy(FullWeek control)
			: base(control) {
		}
		protected internal new FullWeek Control { get { return (FullWeek)base.Control; } }
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentCalculator() {
			WeekViewAppointmentContentLayoutCalculator contentCalculator = new WeekViewAppointmentContentLayoutCalculator(Control, AppointmentPainter);
			if (AutoHeight)
				return new WeekViewAppointmentAutoHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
			else
				return new WeekViewAppointmentFixedHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter);
		}
		protected internal override VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator() {
			return new FullWeekVisuallyContinuousCellsCalculator(Control);
		}
	}
	public class HorizontalWeekAppointmentsLayoutStrategy : AutoHeightCellsAppointmentsLayoutStrategy {
		public HorizontalWeekAppointmentsLayoutStrategy(HorizontalWeek control)
			: base(control) {
		}
		protected internal new HorizontalWeek Control { get { return (HorizontalWeek)base.Control; } }
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentCalculator() {
			MonthViewAppointmentContentLayoutCalculator contentCalculator = new MonthViewAppointmentContentLayoutCalculator(Control, AppointmentPainter);
			if (AutoHeight)
				return new MonthViewAppointmentAutoHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter, Control.ActualCompressWeekend);
			else
				return new MonthViewAppointmentFixedHeightLayoutCalculator(Control, contentCalculator, Control.Cache, AppointmentPainter, Control.ActualCompressWeekend);
		}
		protected internal override VisuallyContinuousCellsCalculatorBase CreateContinuosCellsCalculator() {
			return new HorizontalWeekVisuallyContinuousCellsCalculator(Control);
		}
		protected internal override AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper() {
			return new HorizontalAppointmentLayoutAutoHeightCellsHelper(AppointmentCalculator);
		}
	}
	public class ReportDayTimeCellsAppointmentLayoutCalculator : VerticalAppointmentLayoutCalculatorBase {
		public ReportDayTimeCellsAppointmentLayoutCalculator(DayViewTimeCells timeCellsControl, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(timeCellsControl, contentCalculator, cache, painter) {
		}
		DayViewTimeCells TimeCellsControl { get { return (DayViewTimeCells)ViewInfo; } }
		protected internal override void AddDownMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
		}
		protected internal override void AddUpMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
		}
		protected internal override bool ShowShadow {
			get { return false; }
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleFixedInterval(TimeCellsControl.TimeScale);
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new VerticalAppointmentIntermediateLayoutCalculator((TimeScaleFixedInterval)Scale, AppointmentsSnapToCells, ViewInfo.TimeZoneHelper);
		}
	}
	#region ReportTimelineViewAppointmentFixedHeightLayoutCalculator
	public class TimelineAppointmentFixedHeightLayoutCalculator : TimelineViewAppointmentFixedHeightLayoutCalculatorBase {		
		public TimelineAppointmentFixedHeightLayoutCalculator(TimelineCells cellsControl, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(cellsControl, contentCalculator, cache, painter) {			
		}
		ReportTimelineView View { get { return ((TimelineCells)ViewInfo).View; } }
		protected internal override TimeScale CreateScale() {
			return View.GetBaseTimeScale();
		}
	}
	#endregion
	#region TimelineAppointmentAutoHeightLayoutCalculator
	public class TimelineAppointmentAutoHeightLayoutCalculator : TimelineViewAppointmentAutoHeightLayoutCalculatorBase {
		public TimelineAppointmentAutoHeightLayoutCalculator(TimelineCells cellsControl, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(cellsControl, contentCalculator, cache, painter) {			
		}
		ReportTimelineView View { get { return ((TimelineCells)ViewInfo).View; } }
		protected internal override TimeScale CreateScale() {
			return View.GetBaseTimeScale();
		}				
	}
	#endregion
	public class TimelineAppointmentContentLayoutCalculator : TimelineViewAppointmentContentLayoutCalculatorBase {
		public TimelineAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {			
		}
	}
	public class PreliminaryAppointmentsLayoutResult : AppointmentsLayoutResult {
		AppointmentIntermediateViewInfoCollection intermediateViewInfos;
		public PreliminaryAppointmentsLayoutResult() : base() {
			this.intermediateViewInfos = new AppointmentIntermediateViewInfoCollection();
		}
		public AppointmentIntermediateViewInfoCollection IntermediateViewInfos { get { return intermediateViewInfos; } }
	}	  
}
