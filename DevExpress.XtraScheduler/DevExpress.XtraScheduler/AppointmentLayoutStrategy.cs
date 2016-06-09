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
using System.Linq;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Drawing {
	#region AppointmentsBaseLayoutStrategy
	public abstract class AppointmentsBaseLayoutStrategy {
		readonly SchedulerViewBase view;
		protected AppointmentsBaseLayoutStrategy(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public SchedulerViewInfoBase ViewInfo { get { return view.ViewInfo; } }
		public SchedulerViewBase View { get { return view; } }
		public ResourceVisuallyContinuousCellsInfosCollection CalculatePreliminaryCellsInfos() {
			return CalculateCellsInfos(false);
		}
		public ResourceVisuallyContinuousCellsInfosCollection CalculateFinalCellsInfos() {
			return CalculateCellsInfos(true);
		}
		protected internal ResourceVisuallyContinuousCellsInfosCollection CalculateCellsInfos(bool isFinal) {
			VisuallyContinuousCellsInfosCalculator cellsInfosCalculator = CreateCellsInfosCalculator();
			SchedulerViewCellContainerCollection cellContainters = isFinal ? GetCellContainers() : GetPreliminaryCellContainers();
			return cellsInfosCalculator.Calculate(ViewInfo, cellContainters, ViewInfo.VisibleResources);
		}
		protected internal virtual SchedulerViewCellContainerCollection GetCellContainers() {
			return ViewInfo.CellContainers;
		}
		protected internal virtual AppointmentBaseCollection GetAppointments() {
			return View.Control.AppointmentChangeHelper.GetActualVisibleAppointments(View.FilteredAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(ProcessorBase<Appointment> processor) {
			AppointmentBaseCollection appointments = GetAppointments();
			processor.Process(appointments);
			return (AppointmentBaseCollection)processor.DestinationCollection;
		}
		protected internal abstract VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator();
		SchedulerViewCellContainerCollection GetPreliminaryCellContainers() {
			return ViewInfo.PreliminaryLayoutResult.CellContainers;
		}
		public virtual List<AppointmentIntermediateViewInfoCollection> CalculateAppointmentsPreliminaryLayout(GraphicsCache cache) {
			return new List<AppointmentIntermediateViewInfoCollection>();
		}
		public virtual CellsLayerInfos CalculateCellsLayerInfosPreliminaryLayout(GraphicsCache cache) {
			return new CellsLayerInfos();
		}
	}
	#endregion
	#region AppointmentsLayoutStrategyFixedHeightCells
	public abstract class AppointmentsLayoutStrategyFixedHeightCells : AppointmentsBaseLayoutStrategy {
		protected AppointmentsLayoutStrategyFixedHeightCells(SchedulerViewBase view)
			: base(view) {
		}
		public override List<AppointmentIntermediateViewInfoCollection> CalculateAppointmentsPreliminaryLayout(GraphicsCache cache) {
			List<AppointmentIntermediateViewInfoCollection> result = new List<AppointmentIntermediateViewInfoCollection>();
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculatePreliminaryCellsInfos();
			AppointmentBaseLayoutCalculator layoutCalculator = CreateAppointmentLayoutCalculator(cache);
			View.Control.SubscribeAppointmentContentLayoutCalculatorEvents(layoutCalculator.ContentCalculator);
			try {
				result = layoutCalculator.CalculatePreliminaryLayout(GetLayoutAppointments(), cellsInfos);
			} catch {
			}
			View.Control.UnsubscribeAppointmentContentLayoutCalculatorEvents(layoutCalculator.ContentCalculator);
			return result;
		}
		public virtual void CalculateLayout(GraphicsCache cache) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			result.MoreButtons = ViewInfo.MoreButtons;
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculateFinalCellsInfos();
			AppointmentBaseLayoutCalculator layoutCalculator = CreateAppointmentLayoutCalculator(cache);
			try {
				LayoutAppointments(result, cellsInfos, layoutCalculator);
			} catch {
			}
		}
		public void RecalcVisible(GraphicsCache cache) {
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculateFinalCellsInfos();
			AppointmentBaseLayoutCalculator layoutCalculator = CreateAppointmentLayoutCalculator(cache);
			try {
				layoutCalculator.RecalcAppointmentsVisible(cellsInfos);
			} catch {
			}
		}
		protected internal virtual void LayoutAppointments(AppointmentsLayoutResult result, ResourceVisuallyContinuousCellsInfosCollection cellsInfos, AppointmentBaseLayoutCalculator layoutCalculator) {
			AppointmentBaseCollection appointments = GetLayoutAppointments();
			layoutCalculator.CalculateLayout(result, appointments, cellsInfos);
		}
		protected internal virtual AppointmentBaseCollection GetLayoutAppointments() {
			return GetAppointments();
		}
		protected internal abstract AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache);
	}
	#endregion
	#region DayViewTimeCellAppointmentsLayoutStrategy
	public class DayViewTimeCellAppointmentsLayoutStrategy : AppointmentsLayoutStrategyFixedHeightCells {
		public DayViewTimeCellAppointmentsLayoutStrategy(DayView view)
			: base(view) {
		}
		public new DayView View { get { return (DayView)base.View; } }
		public new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, false);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(AppointmentPainter painter) {
			return new DayViewTimeCellAppointmentContentLayoutCalculator(View.ViewInfo, painter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentPainter painter = ((DayViewPainter)ViewInfo.Painter).TimeCellsAppointmentPainter;
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator(painter);
			return new DayViewTimeCellAppointmentLayoutCalculator(View.ViewInfo, contentCalculator, cache, painter);
		}
		protected internal override AppointmentBaseCollection GetLayoutAppointments() {
			if (View.ActualShowAllAppointmentsAtTimeCells)
				return GetAppointments();
			else
				return GetShortAppointments();
		}
		protected internal virtual AppointmentBaseCollection GetShortAppointments() {
			return GetAppointments(new DayViewTimeCellAppointmentsFilter());
		}
	}
	#endregion
	#region AppointmentsLayoutStrategyAutoHeightCells (abstract class)
	public abstract class AppointmentsLayoutStrategyAutoHeightCells : AppointmentsBaseLayoutStrategy {
		protected AppointmentsLayoutStrategyAutoHeightCells(SchedulerViewBase view)
			: base(view) {
		}
		protected internal abstract AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper(GraphicsCache cache);
		public override CellsLayerInfos CalculateCellsLayerInfosPreliminaryLayout(GraphicsCache cache) {
			CellsLayerInfos result = new CellsLayerInfos();
			AppointmentLayoutAutoHeightCellsHelper layoutHelper = CreateAppointmentLayoutHelper(cache);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculatePreliminaryCellsInfos();
			View.Control.SubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
			try {
				AppointmentBaseCollection appointments = GetLayoutAppointments();
				result = layoutHelper.CalculatePreliminaryLayout(appointments, cellsInfos);
			} catch {
			}
			View.Control.UnsubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
			return result;
		}
		public virtual AppointmentsLayoutResult CalculateFinalLayout(GraphicsCache cache, CellsLayerInfos cellsLayerInfos) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			AppointmentLayoutAutoHeightCellsHelper layoutHelper = CreateAppointmentLayoutHelper(cache);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculateFinalCellsInfos();
			View.Control.SubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
			try {
				SchedulerViewCellContainerCollection containers = GetCellContainers();
				result = layoutHelper.CalculateFinalLayout(cellsLayerInfos, containers, cellsInfos);
			} catch {
			}
			View.Control.UnsubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
			return result;
		}
		public MoreButtonCollection CalculateAllDayScrollMoreButtons(GraphicsCache cache) {
			MoreButtonCollection result = new MoreButtonCollection();
			AllDayAppointmentLayoutHelper layoutHelper = (AllDayAppointmentLayoutHelper)CreateAppointmentLayoutHelper(cache);
			try {
				result = layoutHelper.CalculateScrollMoreButtons(GetCellContainers());
			} catch {
			}
			return result;
		}
		public void CalculateAppointmentsHeight(GraphicsCache cache, CellsLayerInfos cellsLayerInfos) {
			AppointmentLayoutAutoHeightCellsHelper layoutHelper = CreateAppointmentLayoutHelper(cache);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = CalculateFinalCellsInfos();
			View.Control.SubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
			try {
				layoutHelper.CalculateHeight(cellsInfos);
			} catch {
			}
			View.Control.UnsubscribeAppointmentContentLayoutCalculatorEvents(layoutHelper.ContentCalculator);
		}
		protected internal abstract AppointmentBaseCollection GetLayoutAppointments();
	}
	#endregion
	#region DayViewAllDayAppointmentsLayoutStrategy
	public class DayViewAllDayAppointmentsLayoutStrategy : AppointmentsLayoutStrategyAutoHeightCells {
		public new DayView View { get { return (DayView)base.View; } }
		public new DayViewInfo ViewInfo { get { return View.ViewInfo; } }
		public DayViewAllDayAppointmentsLayoutStrategy(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override SchedulerViewCellContainerCollection GetCellContainers() {
			return ViewInfo.PreliminaryLayoutResult.CellContainers;
		}
		public override CellsLayerInfos CalculateCellsLayerInfosPreliminaryLayout(GraphicsCache cache) {
			if (View.ActualShowAllAppointmentsAtTimeCells)
				return new CellsLayerInfos();
			return base.CalculateCellsLayerInfosPreliminaryLayout(cache);
		}
		public override AppointmentsLayoutResult CalculateFinalLayout(GraphicsCache cache, CellsLayerInfos cellLayers) {
			if (View.ActualShowAllAppointmentsAtTimeCells)
				return new AppointmentsLayoutResult();
			return base.CalculateFinalLayout(cache, cellLayers);
		}
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, true);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new DayViewAllDayAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper(GraphicsCache cache) {
			HorizontalAppointmentLayoutCalculator aptCalculator = CreateAppointmentLayoutCalculator(cache);
			return new DayViewAllDayAppointmentLayoutHelper(aptCalculator);
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return new DayViewAllDayAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
			else
				return new DayViewAllDayAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
		protected internal override AppointmentBaseCollection GetLayoutAppointments() {
			return GetAppointments(new DayViewAllDayAppointmentsFilter());
		}
	}
	#endregion
	#region DayViewDispatchAppointmentsLayoutStrategy
	public class DayViewDispatchAppointmentsLayoutStrategy : AppointmentsBaseLayoutStrategy {
		DayViewTimeCellAppointmentsLayoutStrategy timeCellLayoutStrategy;
		DayViewAllDayAppointmentsLayoutStrategy allDayLayoutStrategy;
		public DayViewDispatchAppointmentsLayoutStrategy(DayView view)
			: base(view) {
			timeCellLayoutStrategy = CreateTimeCellLayoutStrategy(view);
			allDayLayoutStrategy = CreateAllDayLayoutStrategy(view);
		}
		public DayViewTimeCellAppointmentsLayoutStrategy TimeCellLayoutStrategy { get { return timeCellLayoutStrategy; } }
		public DayViewAllDayAppointmentsLayoutStrategy AllDayLayoutStrategy { get { return allDayLayoutStrategy; } }
		public new DayView View { get { return (DayView)base.View; } }
		public new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		protected internal virtual DayViewTimeCellAppointmentsLayoutStrategy CreateTimeCellLayoutStrategy(SchedulerViewBase view) {
			return new DayViewTimeCellAppointmentsLayoutStrategy(View);
		}
		protected internal virtual DayViewAllDayAppointmentsLayoutStrategy CreateAllDayLayoutStrategy(SchedulerViewBase view) {
			return new DayViewAllDayAppointmentsLayoutStrategy(view);
		}
		public virtual CellsLayerInfos CalculateAllDayPreliminaryLayout(GraphicsCache cache) {
			return AllDayLayoutStrategy.CalculateCellsLayerInfosPreliminaryLayout(cache);
		}
		public override List<AppointmentIntermediateViewInfoCollection> CalculateAppointmentsPreliminaryLayout(GraphicsCache cache) {
			return TimeCellLayoutStrategy.CalculateAppointmentsPreliminaryLayout(cache);
		}
		public virtual void CalculateFinalLayout(GraphicsCache cache) {
			if (ViewInfo.CellContainers.TotalAppointmentViewInfoCount == 0) {
				AppointmentsLayoutResult allDayResult = AllDayLayoutStrategy.CalculateFinalLayout(cache, ViewInfo.PreliminaryLayoutResult.CellsLayerInfos);
				FillViewInfo(allDayResult);
				ViewInfo.AllDayAreaScrollContainer.AppointmentViewInfos.AddRange(allDayResult.AppointmentViewInfos);
				ViewInfo.ApplySelection(ViewInfo.AllDayAreaScrollContainer);
			} else
				ViewInfo.ScrollMoreButtons.AddRange(AllDayLayoutStrategy.CalculateAllDayScrollMoreButtons(cache));
			TimeCellLayoutStrategy.CalculateLayout(cache);
		}
		public void RecalcTimeCellAppointmentsVisible(GraphicsCache cache) {
			TimeCellLayoutStrategy.RecalcVisible(cache);
		}
		public void CalculateAllDayAppointmentsHeight(GraphicsCache cache) {
			if (!View.ShowAllDayArea)
				return;
			AllDayLayoutStrategy.CalculateAppointmentsHeight(cache, ViewInfo.PreliminaryLayoutResult.CellsLayerInfos);
		}
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return null;
		}
		private void FillViewInfo(AppointmentsLayoutResult layoutResult) {
			ViewInfo.ScrollMoreButtons.AddRange(layoutResult.MoreButtons);
			DayViewAppointmentsLayoutResult dayViewLayoutResult = layoutResult as DayViewAppointmentsLayoutResult;
			if (dayViewLayoutResult != null)
				ViewInfo.AllDayAppointmentsStatuses.AddRange(dayViewLayoutResult.AppointmentStatusViewInfos);
		}
	}
	#endregion
	#region MonthViewAppointmentsLayoutStrategy
	public class MonthViewAppointmentsLayoutStrategy : AppointmentsLayoutStrategyFixedHeightCells {
		public MonthViewAppointmentsLayoutStrategy(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, false);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new MonthViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			WeekView weekView = ((WeekView)View);
			bool compressWeekEnd = weekView.ViewInfo.CompressWeekend && weekView.ViewInfo.ShowWeekend;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return new MonthViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter, compressWeekEnd);
			else
				return new MonthViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter, compressWeekEnd);
		}
	}
	#endregion
	#region TimelineViewAppointmentsLayoutAutoHeightStrategy
	public class TimelineViewAppointmentsLayoutAutoHeightStrategy : AppointmentsLayoutStrategyAutoHeightCells {
		public TimelineViewAppointmentsLayoutAutoHeightStrategy(SchedulerViewBase view)
			: base(view) {
		}
		public new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		protected internal override AppointmentLayoutAutoHeightCellsHelper CreateAppointmentLayoutHelper(GraphicsCache cache) {
			HorizontalAppointmentLayoutCalculator appointmentCalculator = CreateAppointmentLayoutCalculator(cache);
			appointmentCalculator.IsCellHeightVariable = true;
			return new TimelineAppointmentLayoutAutoHeightCellsHelper(appointmentCalculator);
		}
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, false);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new TimelineViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return CreateAppointmentAutoHeightLayoutCalculator(cache, contentCalculator, painter);
			else
				return CreateAppointmentFixedHeightLayoutCalculator(cache, contentCalculator, painter);
		}
		protected internal override AppointmentBaseCollection GetLayoutAppointments() {
			return GetAppointments();
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentAutoHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new TimelineViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentFixedHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new TimelineViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region TimelineViewAppointmentsLayoutFixedHeightStrategy
	public class TimelineViewAppointmentsLayoutFixedHeightStrategy : AppointmentsLayoutStrategyFixedHeightCells {
		public TimelineViewAppointmentsLayoutFixedHeightStrategy(SchedulerViewBase view)
			: base(view) {
		}
		public new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, false);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new TimelineViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return CreateAppointmentAutoHeightLayoutCalculator(cache, contentCalculator, painter);
			else
				return CreateAppointmentFixedHeightLayoutCalculator(cache, contentCalculator, painter);
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentAutoHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new TimelineViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
		protected internal virtual HorizontalAppointmentLayoutCalculator CreateAppointmentFixedHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new TimelineViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region GanttViewAppointmentsLayoutAutoHeightStrategy
	public class GanttViewAppointmentsLayoutAutoHeightStrategy : TimelineViewAppointmentsLayoutAutoHeightStrategy {
		public GanttViewAppointmentsLayoutAutoHeightStrategy(GanttView view)
			: base(view) {
		}
		public new GanttViewInfo ViewInfo { get { return (GanttViewInfo)base.ViewInfo; } }
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new GanttViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override HorizontalAppointmentLayoutCalculator CreateAppointmentAutoHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new GanttViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
		protected internal override HorizontalAppointmentLayoutCalculator CreateAppointmentFixedHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new GanttViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region GanttViewAppointmentsLayoutFixedHeightStrategy
	public class GanttViewAppointmentsLayoutFixedHeightStrategy : TimelineViewAppointmentsLayoutFixedHeightStrategy {
		public GanttViewAppointmentsLayoutFixedHeightStrategy(SchedulerViewBase view)
			: base(view) {
		}
		public new GanttViewInfo ViewInfo { get { return (GanttViewInfo)base.ViewInfo; } }
		protected internal override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new GanttViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override HorizontalAppointmentLayoutCalculator CreateAppointmentAutoHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new GanttViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
		protected internal override HorizontalAppointmentLayoutCalculator CreateAppointmentFixedHeightLayoutCalculator(GraphicsCache cache, AppointmentContentLayoutCalculator contentCalculator, AppointmentPainter painter) {
			return new GanttViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
	#region WeekViewAppointmentsLayoutStrategy
	public class WeekViewAppointmentsLayoutStrategy : AppointmentsLayoutStrategyFixedHeightCells {
		public WeekViewAppointmentsLayoutStrategy(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override VisuallyContinuousCellsInfosCalculator CreateCellsInfosCalculator() {
			return View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(ViewInfo, false);
		}
		protected internal virtual AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator() {
			return new WeekViewAppointmentContentLayoutCalculator(ViewInfo, ViewInfo.Painter.AppointmentPainter);
		}
		protected internal override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(GraphicsCache cache) {
			AppointmentContentLayoutCalculator contentCalculator = CreateAppointmentContentLayoutCalculator();
			AppointmentPainter painter = ViewInfo.Painter.AppointmentPainter;
			if (View.AppointmentDisplayOptions.AppointmentAutoHeight)
				return new WeekViewAppointmentAutoHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
			else
				return new WeekViewAppointmentFixedHeightLayoutCalculator(ViewInfo, contentCalculator, cache, painter);
		}
	}
	#endregion
}
