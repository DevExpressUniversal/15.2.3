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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewInfo : SchedulerViewInfoBase, ISupportTimeRuler {
		#region Fields
		int selectionBorderWidth = 5;
		DayViewRowCollection rows;
		DayViewRowCollection visibleRows;
		DayViewRowCollection extendedRows;
		SchedulerHeaderCollection dayHeaders;
		TimeRulerViewInfoCollection timeRulers;
		TimeIndicatorViewInfo timeIndicator;
		TimeSpan topRowTime;
		Rectangle gapBetweenTimeRulerAndCellsAreaBounds;
		Timer timer;
		AppointmentStatusViewInfoCollection allDayAppointmentsStatuses;
		MoreButtonCollection scrollMoreButtons;
		#endregion
		public DayViewInfo(DayView view)
			: base(view) {
			this.rows = new DayViewRowCollection();
			this.visibleRows = new DayViewRowCollection();
			this.extendedRows = new DayViewRowCollection();
			this.dayHeaders = new SchedulerHeaderCollection();
			this.timeRulers = new TimeRulerViewInfoCollection();
			this.timeIndicator = new TimeIndicatorViewInfo();
			this.scrollMoreButtons = new MoreButtonCollection();
			this.topRowTime = TimeSpan.Zero;
			this.allDayAppointmentsStatuses = new AppointmentStatusViewInfoCollection();
			this.timer = new Timer();
			this.timer.Interval = 30000; 
			SubscribeTimerEvents();
			this.timer.Enabled = true;
		}
		#region Properties
		public DayViewRowCollection Rows { get { return rows; } }
		public DayViewRowCollection VisibleRows { get { return visibleRows; } }
		internal DayViewRowCollection ExtendedRows { get { return extendedRows; } }
		internal new DayViewPreliminaryLayoutResult PreliminaryLayoutResult { get { return (DayViewPreliminaryLayoutResult)base.PreliminaryLayoutResult; } }
		internal DayViewCellsPreliminaryLayoutResult CellsPreliminaryLayoutResult { get { return PreliminaryLayoutResult.CellsPreliminaryLayoutResult; } }
		internal Rectangle GapBetweenTimeRulerAndCellsAreaBounds { get { return gapBetweenTimeRulerAndCellsAreaBounds; } set { gapBetweenTimeRulerAndCellsAreaBounds = value; } }
		internal Timer Timer { get { return timer; } }
		internal MoreButtonCollection ScrollMoreButtons { get { return scrollMoreButtons; } }
		internal AllDayAreaScrollContainer AllDayAreaScrollContainer { get { return PreliminaryLayoutResult.AllDayAreaScrollContainer; } }
		public SchedulerHeaderCollection DayHeaders { get { return dayHeaders; } }
		public TimeRulerViewInfoCollection TimeRulers { get { return timeRulers; } }
		public TimeSpan TopRowTime { get { return topRowTime; } set { topRowTime = value; } }
		public TimeIndicatorViewInfo TimeIndicator { get { return timeIndicator; } }
		public bool DateTimeScrollBarVisible { get { return CellsPreliminaryLayoutResult.DateTimeScrollBarVisible; } }
		public new DayView View { get { return (DayView)base.View; } }
		public new DayViewAppearance PaintAppearance { get { return (DayViewAppearance)base.PaintAppearance; } }
		public SchedulerViewCellContainerCollection Columns { get { return CellContainers; } }
		public AppointmentStatusViewInfoCollection AllDayAppointmentsStatuses { get { return allDayAppointmentsStatuses; } }
		public int SelectionBorderWidth { get { return selectionBorderWidth; } set { selectionBorderWidth = value; } }
		protected internal new DayViewPainter Painter {
			get { return (DayViewPainter)base.Painter; }
			set {
				base.Painter = value;
			}
		}
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (timer != null) {
						timer.Enabled = false;
						UnsubscribeTimerEvents();
						timer.Dispose();
						timer = null;
					}
					if (timeRulers != null) {
						DisposeTimeRulers();
						timeRulers = null;
					}
					timeRulers = null;
					if (TimeIndicator != null) {
						DisposeTimeIndicator();
						timeIndicator = null;
					}
					timeIndicator = null;
					if (scrollMoreButtons != null) {
						scrollMoreButtons.Clear();
						scrollMoreButtons = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		#region ITimeRulerSupport Members
		TimeZoneHelper ISupportTimeRuler.TimeZoneHelper { get { return View.Control.InnerControl.TimeZoneHelper; } }
		TimeSpan ISupportTimeRuler.TimeScale { get { return View.TimeScale; } }
		ITimeRulerFormatStringService ISupportTimeRuler.GetFormatStringProvider() {
			return (ITimeRulerFormatStringService)View.Control.GetService(typeof(ITimeRulerFormatStringService));
		}
		#endregion
		public override SchedulerViewCellContainerCollection GetCellContainers() {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			result.AddRange(CellContainers);
			result.Add(AllDayAreaScrollContainer);
			return result;
		}
		protected internal virtual void DisposeTimeRulers() {
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++)
				timeRulers[i].Dispose();
		}
		protected internal virtual void DisposeTimeIndicator() {
			TimeIndicator.Dispose();
		}
		protected internal virtual void SubscribeTimerEvents() {
			timer.Tick += new EventHandler(OnTimer);
		}
		protected internal virtual void UnsubscribeTimerEvents() {
			timer.Tick -= new EventHandler(OnTimer);
		}
		protected internal override void PrepareAppointmentLayout() {
			MoreButtons.Clear();
			ScrollMoreButtons.Clear();
		}
		protected internal override void ExecuteAppointmentLayoutCalculatorCore(GraphicsCache cache) {
			if (Bounds != PreliminaryLayoutResult.ViewInfoBounds) {
				CellContainers.ForEach(c => c.AppointmentViewInfos.Clear());
				PreliminaryLayoutResult.ViewInfoBounds = Bounds;
			}
			DayViewDispatchAppointmentsLayoutStrategy layoutStrategy = (DayViewDispatchAppointmentsLayoutStrategy)View.FactoryHelper.CreateAppointmentsLayoutStrategy(View);
			if (CellContainers.TotalAppointmentViewInfoCount != 0)
				layoutStrategy.RecalcTimeCellAppointmentsVisible(cache);
			layoutStrategy.CalculateFinalLayout(cache);
		}
		protected internal virtual void OnTimer(object sender, EventArgs e) {
			if (Painter == null || View.Control.InnerControl == null)
				return;
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				TimeRulerLayoutCalculator timeRulerLayoutCalculator = CreateTimeRulerLayoutCalculator(cache);
				timeRulerLayoutCalculator.RecalcRulersCurrentTimeLayout();
				CalculateTimeIndicators(cache);
			}
			View.Control.Invalidate();
			View.Control.Update();
		}
		protected internal override void UpdateScrollMoreButtonsVisibility() {
			AllDayScrollMoreButtonsCalculator calculator = new AllDayScrollMoreButtonsCalculator(this);
			AppointmentViewInfoCollection scrollAppointments = GetScrollContainerAppointmentViewInfos(AllDayAreaScrollContainer);
			calculator.CalculateVisibility(ScrollMoreButtons, scrollAppointments, AllDayAreaScrollContainer);
		}
		protected internal override bool ShouldShowContainerScrollBar() {
			return View.ShowAllDayArea && base.ShouldShowContainerScrollBar();
		}
		protected internal override Rectangle CalculateScrollContainerBounds(SchedulerViewCellContainer container) {
			Rectangle allDayAreaRect = container.Bounds;
			return Rectangle.FromLTRB(allDayAreaRect.Left, Bounds.Top, allDayAreaRect.Right + 1, allDayAreaRect.Bottom - 1);
		}
		protected internal override SchedulerViewCellContainerCollection GetScrollContainers() {
			SchedulerViewCellContainerCollection containers = new SchedulerViewCellContainerCollection();
			if (AllDayAreaScrollContainer != null)
				containers.Add(AllDayAreaScrollContainer);
			return containers;
		}
		protected override void CalcScrollBarVisibilityCore(GraphicsCache cache) {
			base.CalcScrollBarVisibilityCore(cache);
			Rectangle availableBounds = CalculateTimeRulersPreliminaryLayout(cache, Bounds);
			availableBounds = CreateGapBetweenTimeRulerAndCellsArea(availableBounds);
			ExecuteHeadersLayoutCalculator(cache, Bounds);
			CalculateAllDayAppointmentsHeight(cache);
			DayViewGroupCellsLayoutCalculator cellsLayoutCalculator = (DayViewGroupCellsLayoutCalculator)this.View.FactoryHelper.CreateCellsLayoutCalculator(this, cache, (DayViewColumnPainter)this.Painter.SelectCellsLayoutPainter());
			cellsLayoutCalculator.CalculateCellsBounds(availableBounds, PreliminaryLayoutResult);
			cellsLayoutCalculator.CalculateExtendedCellsBounds(availableBounds, PreliminaryLayoutResult);
			CalculateScrollBarVisibility();
		}
		protected internal override void CalcFinalLayoutCore(GraphicsCache cache) {
			bool showContainerScrollBar = ShouldShowContainerScrollBar();
			if (showContainerScrollBar)
				CreateContainerScrollBars(GetScrollContainers());
			TimeRulers.Clear();
			Rectangle availableBounds = CalculateTimeRulersPreliminaryLayout(cache, Bounds);
			base.CalcFinalLayoutCore(cache);
			CalcTimeRulersFinalLayout(cache);
			CalculateTimeIndicators(cache);
			Rectangle bounds = this.PreliminaryLayoutResult.CellsPreliminaryLayoutResult.AllDayAreaBounds;
			bounds.Width = availableBounds.Width;
			bounds.Inflate(0, -1);
			AllDayAreaScrollContainer.Bounds = bounds;
			if (showContainerScrollBar) {
				UpdateContainerScrollBars(GetScrollContainers());
				UpdateScrollMoreButtonsVisibility();
			}
		}
		protected internal override void ExecuteHeadersLayoutCalculator(GraphicsCache cache, Rectangle bounds) {
			Rectangle boundsWithTimeRulers = CalcBoundsWithoutTimeRulers(cache, Bounds);
			boundsWithTimeRulers = CreateGapBetweenTimeRulerAndCellsArea(boundsWithTimeRulers);
			base.ExecuteHeadersLayoutCalculator(cache, boundsWithTimeRulers);
		}
		protected internal override int CalculateVerticalDateTimeScrollBarTop() {
			if (ShouldShowContainerScrollBar())
				return PreliminaryLayoutResult.CellsPreliminaryLayoutResult.AllDayAreaBounds.Bottom;
			return base.CalculateVerticalDateTimeScrollBarTop();
		}
		protected internal virtual Rectangle CreateGapBetweenTimeRulerAndCellsArea(Rectangle availableBounds) {
			int gapWidth = Painter.ViewAndScrollbarVerticalSeparatorWidth;
			this.gapBetweenTimeRulerAndCellsAreaBounds = new Rectangle(availableBounds.X, availableBounds.Top, gapWidth, availableBounds.Height);
			availableBounds.X += gapWidth;
			availableBounds.Width -= gapWidth;
			return availableBounds;
		}
		protected internal virtual void CalcTimeRulersFinalLayout(GraphicsCache cache) {
			TimeRulerLayoutCalculator timeRulerLayoutCalculator = CreateTimeRulerLayoutCalculator(cache);
			timeRulerLayoutCalculator.CalcLayout();
		}
		protected internal virtual Rectangle CalculateTimeRulersPreliminaryLayout(GraphicsCache cache, Rectangle bounds) {
			TimeRulerLayoutCalculator timeRulerLayoutCalculator = CreateTimeRulerLayoutCalculator(cache);
			return timeRulerLayoutCalculator.CalcPreliminaryLayout(bounds);
		}
		protected Rectangle CalcBoundsWithoutTimeRulers(GraphicsCache cache, Rectangle bounds) {
			TimeRulerLayoutCalculator timeRulerLayoutCalculator = CreateTimeRulerLayoutCalculator(cache);
			return timeRulerLayoutCalculator.CalcBoundsWithoutTimeRulers(bounds);
		}
		protected internal virtual TimeRulerLayoutCalculator CreateTimeRulerLayoutCalculator(GraphicsCache cache) {
			DayViewPainter dayViewPainter = (DayViewPainter)Painter;
			TimeMarkerVisibility visibility = View.TimeMarkerVisibility;
			return new TimeRulerLayoutCalculator(cache, this, dayViewPainter.TimeRulerPainter, visibility);
		}
		protected internal virtual void CalculateTimeIndicators(GraphicsCache cache) {
			HorizontalTimeIndicatorLayoutCalculator timeIndicatorLayoutCalculator = CreateTimeIndicatorLayoutCalculator(cache);
			timeIndicatorLayoutCalculator.RecalcTimeIndicatorLayout(this.TimeIndicator);
		}
		protected internal virtual HorizontalTimeIndicatorLayoutCalculator CreateTimeIndicatorLayoutCalculator(GraphicsCache cache) {
			TimeIndicatorVisibility visibility = View.TimeIndicatorDisplayOptions.Visibility;
			return new HorizontalTimeIndicatorLayoutCalculator(this, visibility);
		}
		protected internal override SchedulerHitInfo CalculateMoreButtonHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			SchedulerHitInfo hitInfo = base.CalculateMoreButtonHitInfo(pt, nextHitInfo);
			if (hitInfo == nextHitInfo)
				hitInfo = CalculateMoreButtonHitInfoCore(pt, nextHitInfo, ScrollMoreButtons);
			return hitInfo;
		}
		protected internal override SchedulerHitInfo CalculateAppointmentsHitInfo(Point pt, SchedulerHitInfo layoutHitInfo) {
			SchedulerHitInfo appointmentHitInfo = base.CalculateAppointmentsHitInfo(pt, layoutHitInfo);
			if (IsVerticalAppointmentOverAllDayArea(appointmentHitInfo, layoutHitInfo))
				return layoutHitInfo;
			return appointmentHitInfo;
		}
		protected bool IsVerticalAppointmentOverAllDayArea(SchedulerHitInfo appointmentHitInfo, SchedulerHitInfo layoutHitInfo) {
			if (!Object.ReferenceEquals(appointmentHitInfo, layoutHitInfo) && layoutHitInfo.Contains(SchedulerHitTest.AllDayArea))
				if (appointmentHitInfo.ViewInfo is VerticalAppointmentViewInfo)
					return true;
			return false;
		}
		protected internal override SchedulerViewCellContainer ObtainScrollContainer(SchedulerHitInfo layoutHitInfo) {
			if (layoutHitInfo.Contains(SchedulerHitTest.AllDayArea))
				return AllDayAreaScrollContainer;
			return base.ObtainScrollContainer(layoutHitInfo);
		}
		protected internal override SchedulerHitInfo CalculateLayoutHitInfo(Point pt) {
			SchedulerHitInfo baseHitInfo = base.CalculateLayoutHitInfo(pt);
			if (baseHitInfo.HitTest != SchedulerHitTest.None)
				return baseHitInfo;
			baseHitInfo = CalculateTimeRulersHitInfo(pt);
			if (baseHitInfo.HitTest != SchedulerHitTest.None)
				return baseHitInfo;
			return CalculateHeaderCollectionHitInfo(pt, dayHeaders);
		}
		protected internal virtual SchedulerHitInfo CalculateTimeRulersHitInfo(Point pt) {
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = timeRulers[i].CalculateHitInfo(pt, SchedulerHitInfo.None);
				if (hitInfo.HitTest != SchedulerHitTest.None)
					return hitInfo;
			}
			return SchedulerHitInfo.None;
		}
		protected override void CalculateCellsPreliminaryLayout(GraphicsCache cache) {
			base.CalculateCellsPreliminaryLayout(cache);
			DayViewColumnPainter columnPainter = (DayViewColumnPainter)Painter.SelectCellsLayoutPainter();
			CellsPreliminaryLayoutResult.StatusLineWidth = View.StatusLineWidth <= 0 ? columnPainter.StatusLineWidth : View.StatusLineWidth;
			CalculatePreliminaryAllDayAppointments(cache);
			DayViewGroupCellsLayoutCalculator cellsLayoutCalculator = (DayViewGroupCellsLayoutCalculator)View.FactoryHelper.CreateCellsLayoutCalculator(this, cache, columnPainter);
			cellsLayoutCalculator.CreateRowsInfo();
		}
		void CalculatePreliminaryAllDayAppointments(GraphicsCache cache) {
			DayViewDispatchAppointmentsLayoutStrategy aptLayoutStrategy = (DayViewDispatchAppointmentsLayoutStrategy)View.FactoryHelper.CreateAppointmentsLayoutStrategy(View);
			CellsLayerInfos layerInfos = aptLayoutStrategy.CalculateAllDayPreliminaryLayout(cache);
			PreliminaryLayoutResult.CellsLayerInfos.Clear();
			PreliminaryLayoutResult.CellsLayerInfos.AddRange(layerInfos);
		}
		protected internal virtual void CalculateScrollBarVisibility() {
			int maximumRowsCount = CellsPreliminaryLayoutResult.RowsBoundsWithExtendedCells.Height / CellsPreliminaryLayoutResult.MinRowHeight;
			CellsPreliminaryLayoutResult.DateTimeScrollBarVisible = (CellsPreliminaryLayoutResult.TimeIntervalsCount + PreliminaryLayoutResult.ExtendedRowCount) > maximumRowsCount;
		}
		protected internal virtual DayViewColumn CreateDayViewColumn(Resource resource, TimeInterval timeInterval, SchedulerColorSchema colorSchema) {
			return new DayViewColumn(resource, timeInterval, colorSchema);
		}
		protected internal override Rectangle GetInplaceEditorBounds(AppointmentViewInfo vi) {
			int scrollOffset = vi.ScrollContainer.CalculateScrollOffset();
			Rectangle rect = vi.InnerBounds;
			rect.Offset(0, -scrollOffset);
			if (vi is VerticalAppointmentViewInfo) {
				return Rectangle.Intersect(CellsPreliminaryLayoutResult.RowsBounds, rect);
			} else {
				return Rectangle.Intersect(PreliminaryLayoutResult.CellsPreliminaryLayoutResult.AllDayAreaBounds, rect);
			}
		}
		protected internal override void CalcPreliminaryLayoutCore(GraphicsCache cache) {
			PreliminaryLayoutResult.CellContainers.ForEach(c => c.AppointmentViewInfos.Clear());
			base.CalcPreliminaryLayoutCore(cache);
			PreliminaryLayoutResult.ViewInfoBounds = Bounds;
		}
		protected override void ClearCellContainerAppointments() {
		}
		void CalculateAllDayAppointmentsHeight(GraphicsCache cache) {
			DayViewDispatchAppointmentsLayoutStrategy layoutStrategy = (DayViewDispatchAppointmentsLayoutStrategy)View.FactoryHelper.CreateAppointmentsLayoutStrategy(View);
			layoutStrategy.CalculateAllDayAppointmentsHeight(cache);
		}
		protected internal override ViewInfoBasePreliminaryLayoutResult CreatePreliminaryLayoutResult() {
			return new DayViewPreliminaryLayoutResult(this);
		}
	}
}
