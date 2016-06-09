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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	public interface ISupportTimeline {
		WorkDaysCollection WorkDays { get; }
		BaseViewAppearance TimelinePaintAppearance { get; }
	}
	public class TimelineViewInfo : SchedulerViewInfoBase, ISupportTimeline {
		const int defaultSelectionBarHeight = 15;
		SchedulerHeaderCollection corners = new SchedulerHeaderCollection();
		SchedulerHeaderLevelCollection scaleLevels = new SchedulerHeaderLevelCollection();
		TimeScaleCollection scales;
		SelectionBar selectionBar;
		TimeIndicatorViewInfo timeIndicator;
		BaseViewAppearance timelinePaintAppearance;
		Timer timer;
		public TimelineViewInfo(TimelineView view)
			: base(view) {
			this.timelinePaintAppearance = CreateTimelinePaintAppearance();
			SchedulerColorSchema schema = View.GetResourceColorSchema(ResourceBase.Empty, 0);
			this.selectionBar = new SelectionBar(schema);
			this.scales = View.ActualScales;
			this.timeIndicator = new TimeIndicatorViewInfo();
			this.timer = new Timer();
			this.timer.Interval = 30000; 
			SubscribeTimerEvents();
			this.timer.Enabled = true;
		}
		#region Properties
		public new TimelineView View { get { return (TimelineView)base.View; } }
		public SchedulerHeaderLevelCollection ScaleLevels { get { return scaleLevels; } }
		public SchedulerHeaderLevel BaseScaleLevel { get { return ScaleLevels.Count > 0 ? ScaleLevels[ScaleLevels.Count - 1] : new SchedulerHeaderLevel(); } }
		public TimeScaleCollection Scales { get { return scales; } }
		public TimeScale BaseScale { get { return Scales[Scales.Count - 1]; } }
		public SchedulerHeaderCollection Corners { get { return corners; } }
		public SelectionBar SelectionBar { get { return selectionBar; } }
		public SchedulerViewCellContainerCollection Timelines { get { return CellContainers; } }
		public BaseViewAppearance TimelinePaintAppearance { get { return timelinePaintAppearance; } }
		public TimeIndicatorViewInfo TimeIndicator { get { return timeIndicator; } }
		internal new TimelineViewPreliminaryLayoutResult PreliminaryLayoutResult { get { return (TimelineViewPreliminaryLayoutResult)base.PreliminaryLayoutResult; } }
		internal Timer Timer { get { return timer; } }
		protected override bool UseAsyncMode { get { return false; } }
		#endregion
		#region ITimeLineSupport Members
		WorkDaysCollection ISupportTimeline.WorkDays {
			get { return View.Control.WorkDays; }
		}
		#endregion
		protected override ChangeActions PrepateChangeActionsCore(GraphicsCache cache, Rectangle bounds) {
			if (Painter == null)
				return base.PrepateChangeActionsCore(cache, bounds);
			ChangeActions result = ChangeActions.None;
			int visibleIntervalsCount = CalculateVisibleIntervalsCount(cache, bounds);
			if (visibleIntervalsCount != PreliminaryLayoutResult.VisibleIntervalsCount)
				result = ChangeActions.RecalcPreliminaryLayout;
			return result |= base.PrepateChangeActionsCore(cache, bounds);
		}
		public int CalculateHeadersHeight() {
			XtraSchedulerDebug.Assert(ScaleLevels.Count > 0);
			int top = ScaleLevels[0].Headers[0].Bounds.Top;
			int count = ScaleLevels.Count - 1;
			int bottom = ScaleLevels[count].Headers[0].Bounds.Bottom;
			return bottom - top;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					scales = null;
					if (timer != null) {
						timer.Enabled = false;
						UnsubscribeTimerEvents();
						timer.Dispose();
						timer = null;
					}
					if (corners != null) {
						DisposeCorners();
						corners = null;
					}
					if (selectionBar != null) {
						selectionBar = null;
					}
					if (timelinePaintAppearance != null) {
						timelinePaintAppearance.Dispose();
						timelinePaintAppearance = null;
					}
					if (TimeIndicator != null) {
						DisposeTimeIndicator();
						timeIndicator = null;
					}
					timeIndicator = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void DisposeCorners() {
			for (int i = 0; i < this.corners.Count; i++)
				this.corners[i].Dispose();
			this.corners.Clear();
			this.corners = null;
		}
		protected internal virtual void DisposeScaleHeaders() {
			for (int i = 0; i < ScaleLevels.Count; i++) {
				for (int j = 0; j < ScaleLevels[i].Headers.Count; j++)
					ScaleLevels[i].Headers[j].Dispose();
				ScaleLevels[i].Headers.Clear();
			}
		}
		protected internal virtual void DisposeTimeIndicator() {
			TimeIndicator.Dispose();
		}
		#endregion
		protected int GetVerticalOvertlap() {
			return Painter.HorizontalHeaderPainter.VerticalOverlap;
		}
		protected bool IsAutoHeightCells() {
			if (View.FactoryHelper.CalcActualGroupType(View) == SchedulerGroupType.None)
				return false;
			return View.CellsAutoHeightOptions.Enabled;
		}
		protected internal override void CalcPreliminaryLayoutCore(GraphicsCache cache) {
			PreliminaryLayoutResult.VisibleIntervalsCount = CalculateVisibleIntervalsCount(cache, Bounds);
			base.CalcPreliminaryLayoutCore(cache);
		}
		protected override void CalculateCellsPreliminaryLayout(GraphicsCache cache) {
			base.CalculateCellsPreliminaryLayout(cache);
			if (ShouldShowContainerScrollBar())
				CreateContainerScrollBars(PreliminaryLayoutResult.CellContainers);
		}
		protected override void CalculateAppointmentsPreliminaryLayout(GraphicsCache cache) {
			View.UpdateFilteredAppointments();
			base.CalculateAppointmentsPreliminaryLayout(cache);
		}
		protected internal override void CalcFinalLayoutCore(GraphicsCache cache) {
			base.CalcFinalLayoutCore(cache);
			CalculateTimeIndicators(cache);
			if (ShouldShowContainerScrollBar())
				UpdateContainerScrollBars(Timelines);
			if (!View.ShowResourceHeaders)
				HideResourceHeaders();
		}
		protected internal override void ExecuteCellsLayoutCalculator(GraphicsCache cache, Rectangle bounds) {
			base.ExecuteCellsLayoutCalculator(cache, CalculateContainersAvailableBounds());
		}
		protected internal override void ExecuteHeadersLayoutCalculator(GraphicsCache cache, Rectangle bounds) {
			base.ExecuteHeadersLayoutCalculator(cache, bounds);
			ExecuteSelectionBarLayoutCalculator(cache);
		}
		protected internal override void ExecuteAppointmentLayoutCalculatorCore(GraphicsCache cache) {
			if (IsAutoHeightCells())
				CalcFinalLayoutAutoHeightCells(cache);
			else
				CalculateLayoutFixedHeightCells(cache);
		}
		protected internal virtual void ExecuteHeadersAutoHeightLayoutCalculator(GraphicsCache cache, Rectangle bounds, int[] headersHeight) {
			TimelineViewGroupByDateHeadersLayoutCalculator headersLayoutCalculator = (TimelineViewGroupByDateHeadersLayoutCalculator)View.FactoryHelper.CreateHeadersLayoutCalculator(this, cache, Painter.HorizontalHeaderPainter);
			headersLayoutCalculator.CalcLayout(bounds, headersHeight);
		}
		protected internal virtual void ExecuteSelectionBarLayoutCalculator(GraphicsCache cache) {
			TimelineViewFactoryHelper helper = (TimelineViewFactoryHelper)View.FactoryHelper;
			TimelineViewPainter painter = (TimelineViewPainter)Painter;
			SelectionBarLayoutCalculator calc = helper.CreateSelectionBarLayoutCalculator(this, cache, painter.SelectionBarPainter, BaseScaleLevel.Headers);
			calc.CalcLayout(Bounds);
		}
		protected internal int CalculateCellsTotalHeight() {
			int result = 0;
			int count = CellContainers.Count;
			for (int i = 0; i < count; i++)
				result += CellContainers[i].Bounds.Height;
			result += TimelineViewGroupByDateHeadersLayoutCalculator.LayoutCorrection;
			result += GetVerticalOvertlap() * (count - 1);
			return result;
		}
		protected internal virtual Rectangle CalculateContainersAvailableBounds() {
			Rectangle rect = Bounds;
			int contentScrollBarWidth = CalculateContentScrollBarWidth(PreliminaryLayoutResult.CellContainers);
			rect.Width = Math.Max(rect.Width - contentScrollBarWidth, 0);
			return rect;
		}
		protected internal virtual int CalculateContentScrollBarWidth(SchedulerViewCellContainerCollection containers) {
			return containers.Count > 0 ? containers[0].ScrollController.ScrollBarWidth : 0;
		}
		protected internal override SchedulerHitInfo CalculateHitInfoCellContainers(Point pt) {
			SchedulerHitInfo hitInfo = SelectionBar.CalculateHitInfo(pt, SchedulerHitInfo.None);
			if (hitInfo.HitTest != SchedulerHitTest.None) {
				return hitInfo;
			}
			return base.CalculateHitInfoCellContainers(pt);
		}
		protected internal override SchedulerHitInfo CalculateLayoutHitInfo(Point pt) {
			SchedulerHitInfo baseHitInfo = base.CalculateLayoutHitInfo(pt);
			if (baseHitInfo.HitTest != SchedulerHitTest.None)
				return baseHitInfo;
			return CalculateScaleLevelsHitInfo(pt);
		}
		protected internal override void CalculatePaintAppearance(ViewPainterBase painter) {
			base.CalculatePaintAppearance(painter);
			timelinePaintAppearance.Assign(PaintAppearance);
			timelinePaintAppearance.HeaderCaption.TextOptions.WordWrap = WordWrap.NoWrap;
			timelinePaintAppearance.AlternateHeaderCaption.TextOptions.WordWrap = WordWrap.NoWrap;
		}
		protected internal virtual SchedulerHitInfo CalculateScaleLevelsHitInfo(Point pt) {
			for (int i = 0; i < ScaleLevels.Count; i++) {
				SchedulerHitInfo hitInfo = CalculateHeaderCollectionHitInfo(pt, ScaleLevels[i].Headers);
				if (hitInfo.HitTest != SchedulerHitTest.None)
					return hitInfo;
			}
			return SchedulerHitInfo.None;
		}
		protected internal virtual int CalculateSelectionBarHeight() {
			SelectionBarOptions options = View.SelectionBar;
			if (options.Visible) {
				return options.Height == 0 ? defaultSelectionBarHeight : options.Height;
			}
			return 0;
		}
		protected internal virtual void CalculateTimeIndicators(GraphicsCache cache) {
			VerticalTimeIndicatorLayoutCalculator timeIndicatorLayoutCalculator = CreateTimeIndicatorLayoutCalculator(cache);
			timeIndicatorLayoutCalculator.RecalcTimeIndicatorLayout(this.TimeIndicator);
		}
		protected internal virtual TimelineViewAppointmentsLayoutFixedHeightStrategy CreateAppointmentLayoutFixedHeightStrategy() {
			return View.FactoryHelper.CreateAppointmentsLayoutFixedHeightStrategy(View);
		}
		protected internal virtual TimelineViewAppointmentsLayoutAutoHeightStrategy CreateAppointmentsLayoutAutoHeightStrategy() {
			return View.FactoryHelper.CreateAppointmentsLayoutAutoHeightStrategy(View);
		}
		protected internal virtual VerticalTimeIndicatorLayoutCalculator CreateTimeIndicatorLayoutCalculator(GraphicsCache cache) {
			TimeIndicatorVisibility visibility = View.TimeIndicatorDisplayOptions.Visibility;
			return new VerticalTimeIndicatorLayoutCalculator(this, visibility);
		}
		protected internal virtual BaseViewAppearance CreateTimelinePaintAppearance() {
			return View.CreateAppearance();
		}
		protected internal override Rectangle GetInplaceEditorBounds(AppointmentViewInfo vi) {
			MakeAppointmentVisibleInScrollContainers(vi.Appointment);
			Rectangle rect = vi.InnerBounds;
			rect.Offset(0, -vi.ScrollContainer.CalculateScrollOffset());
			return Rectangle.Intersect(Bounds, rect);
		}
		protected internal virtual void HideResourceHeaders() {
			Corners.Clear();
			ResourceHeaders.Clear();
		}
		protected internal virtual bool IsNeedRecalcHeadersLayout(SchedulerHeaderCollection baseHeaders, TimeIntervalCollection visibleIntervals) {
			if (baseHeaders.Count <= 0)
				return false;
			return baseHeaders[0].Interval.Start != visibleIntervals.Start;
		}
		protected internal virtual void OnTimer(object sender, EventArgs e) {
			if (Painter == null || View.Control.InnerControl == null)
				return;
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				CalculateTimeIndicators(cache);
			}
			View.Control.Invalidate();
			View.Control.Update();
		}
		protected internal override void ReleaseScrollContainers() {
			if (Timelines == null)
				return;
			int count = Timelines.Count;
			for (int i = 0; i < count; i++) {
				UnsubsribeScrollContainerEvents(Timelines[i]);
			}
		}
		protected internal virtual void SubscribeTimerEvents() {
			timer.Tick += new EventHandler(OnTimer);
		}
		protected internal virtual void UnsubscribeTimerEvents() {
			timer.Tick -= new EventHandler(OnTimer);
		}
		protected internal override void UpdateSelection(SchedulerViewSelection selection) {
			base.UpdateSelection(selection);
			SelectionBar.UpdateSelection(selection);
		}
		protected internal override ViewInfoBasePreliminaryLayoutResult CreatePreliminaryLayoutResult() {
			return new TimelineViewPreliminaryLayoutResult();
		}
		internal int CalculateVisibleIntervalsCount(GraphicsCache cache, Rectangle bounds) {
			int resourceHeadersWidht = 0;
			if (View.FactoryHelper.CalcActualGroupType(View) != SchedulerGroupType.None) {
				TimelineViewGroupHeadersLayoutCalculator headersLayoutCalculator = (TimelineViewGroupHeadersLayoutCalculator)View.FactoryHelper.CreateHeadersLayoutCalculator(this, cache, Painter.HorizontalHeaderPainter);
				resourceHeadersWidht = headersLayoutCalculator.CalculateResourceHeadersWidht(bounds);
			}
			int itemWidth = View.GetBaseTimeScale().Width;
			int intervalCount = (bounds.Width - resourceHeadersWidht + itemWidth - 1) / itemWidth;
			return Math.Max(1, intervalCount);
		}
		void CalculateLayoutFixedHeightCells(GraphicsCache cache) {
			AppointmentsLayoutStrategyFixedHeightCells layoutStrategy = CreateAppointmentLayoutFixedHeightStrategy();
			layoutStrategy.CalculateLayout(cache);
		}
		void CalcFinalLayoutAutoHeightCells(GraphicsCache cache) {
			TimelineViewAppointmentsLayoutAutoHeightStrategy layoutStrategy = CreateAppointmentsLayoutAutoHeightStrategy();
			CellsLayerInfos cellLayers = CalculateAutoHeightCellLayers(cache, layoutStrategy);
			CalculateAutoHeightHeadersAndCells(cache, cellLayers);
			CalculateAutoHeightFinalAppointments(cache, layoutStrategy, cellLayers);
		}
		void CalculateAutoHeightHeadersAndCells(GraphicsCache cache, CellsLayerInfos cellLayers) {
			ReleaseScrollContainers();
			CellContainers.Clear();
			ExecuteHeadersAutoHeightLayoutCalculator(cache, Bounds, GetHeadersHeight(cellLayers));
			ExecuteCellsLayoutCalculator(cache, CalculateContainersAvailableBounds());
		}
		void CalculateAutoHeightFinalAppointments(GraphicsCache cache, TimelineViewAppointmentsLayoutAutoHeightStrategy layoutStrategy, CellsLayerInfos cellLayers) {
			AppointmentsLayoutResult result = layoutStrategy.CalculateFinalLayout(cache, cellLayers);
			this.MoreButtons.AddRange(result.MoreButtons);
		}
		CellsLayerInfos CalculateAutoHeightCellLayers(GraphicsCache cache, TimelineViewAppointmentsLayoutAutoHeightStrategy layoutStrategy) {
			CellsLayerInfos cellLayers = layoutStrategy.CalculateCellsLayerInfosPreliminaryLayout(cache);
			cellLayers.ForEach(cli => cli.CalculateHeight());
			CellLayersHeightCalculator layerHeightCalculator = new FitCellLayersHeightCalculator(View.CollapsedResourceHeight);
			layerHeightCalculator.CalculateLayersHeight(cellLayers, true, true, CalculateCellsTotalHeight());
			XtraSchedulerDebug.Assert(cellLayers.Count == ResourceHeaders.Count);
			return cellLayers;
		}
		int[] GetHeadersHeight(CellsLayerInfos cellLayers) {
			int count = cellLayers.Count;
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
				result[i] = cellLayers[i].ActualHeight;
			return result;
		}
	}
}
