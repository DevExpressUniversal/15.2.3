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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Reporting.Native;
using System.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting {
	#region ReportTimelineView    
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "timelineview.bmp"),
	Description("A View component for a timeline style report.")
	]
	public class ReportTimelineView : ReportViewBase {
		#region Fields
		internal const int DefaultTimelineIntervalCount = 10;
		internal const VisibleIntervalsSplitting DefaultVisibleIntervalsSplitting = VisibleIntervalsSplitting.None;
		NotificationCollectionChangedListener<TimeScale> timeScaleCollectionChangedListener;
		TimeScaleCollection scales;
		TimeScaleCollection actualScales;
		VisibleIntervalsSplitting visibleIntervalsSplitting;
		#endregion
		public ReportTimelineView() {
		}
		#region Properties
		protected internal TimeScaleCollection ActualScales { get { return actualScales; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportTimelineViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportTimelineViewAppearance Appearance { get { return (ReportTimelineViewAppearance)base.Appearance; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportTimelineViewScales"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public TimeScaleCollection Scales { get { return scales; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportTimelineViewVisibleIntervalsSplitting"),
#endif
	 Category(SRCategoryNames.Layout), DefaultValue(DefaultVisibleIntervalsSplitting)]
		public VisibleIntervalsSplitting VisibleIntervalsSplitting { get { return visibleIntervalsSplitting; } set { visibleIntervalsSplitting = value; } }
		internal bool ShouldSerializeScales() {
			return !Scales.HasDefaultContent();
		}
		internal void ResetScales() {
			Scales.LoadDefaults();
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportTimelineViewVisibleIntervalCount"),
#endif
DefaultValue(DefaultTimelineIntervalCount), Category(SRCategoryNames.Layout)]
		public new int VisibleIntervalCount { get { return base.VisibleIntervalCount; } set { base.VisibleIntervalCount = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportTimelineViewVisibleIntervalColumnCount"),
#endif
DefaultValue(DefaultVisibleIntervalColumnCount), Category(SRCategoryNames.Layout)]
		public new int VisibleIntervalColumnCount { get { return base.VisibleIntervalColumnCount; } set { base.VisibleIntervalColumnCount = value; } }
		#endregion
		protected internal override void Initialize() {
			this.scales = CreateTimeScaleCollection();
			this.actualScales = CreateTimeScaleCollection();
			this.timeScaleCollectionChangedListener = new NotificationCollectionChangedListener<TimeScale>(Scales);
			this.scales.LoadDefaults();
			UpdateActualScales();
			base.Initialize();
		}
		public override void BeginInit() {
			base.BeginInit();
			UnsubscribeScalesEvents();
			Scales.Clear();
		}
		public override void EndInit() {
			base.EndInit();
			if (Scales.Count <= 0)
				Scales.LoadDefaults();
			SubscribeScalesEvents();
			OnScaleCollectionChanged();
		}
		protected internal override TimeIntervalFormatType GetDefaultTimeIntervalFormatType() {
			return TimeIntervalFormatType.Timeline;
		}
		protected override int CalculateDefaultVisibleIntervalCount() {
			return DefaultTimelineIntervalCount;
		}
		protected internal virtual TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			UpdateScalesFirstDayOfWeek(GetFirstDayOfWeek());
			return new TimeScaleIntervalCollection(GetBaseTimeScale());
		}	  
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			SubscribeScalesEvents();
		}
		protected internal override void UnsubscribeEvents() {
			UnsubscribeScalesEvents();
			base.UnsubscribeEvents();
		}
		protected internal virtual void SubscribeScalesEvents() {
			if (timeScaleCollectionChangedListener != null)
				timeScaleCollectionChangedListener.Changed += new EventHandler(OnScalesChanged);
		}
		protected internal virtual void UnsubscribeScalesEvents() {
			if (timeScaleCollectionChangedListener != null)
				timeScaleCollectionChangedListener.Changed -= new EventHandler(OnScalesChanged);
		}
		protected internal virtual void OnScalesChanged(object sender, EventArgs e) {
			OnScaleCollectionChanged();
			RaiseChanged(ReportControlChangeType.TimelineScalesChanged);
		}
		private void OnScaleCollectionChanged() {
			UpdateActualScales();
			CreateVisibleIntervals();
		}
		protected internal virtual void UpdateActualScales() {
			ActualScales.BeginUpdate();
			try {
				ActualScales.Clear();
				ActualScales.AddRange(TimeScaleCollectionHelper.SelectVisibleScales(Scales));
			}
			finally {
				ActualScales.EndUpdate();
			}
		}
		public TimeScale GetBaseTimeScale() {
			int count = ActualScales.Count;
			XtraSchedulerDebug.Assert(count > 0);
			return ActualScales[count - 1];
		}
		protected override BaseViewAppearance CreateAppearance() {
			return new ReportTimelineViewAppearance();
		}
		protected internal override TimeIntervalCollection CreateFakeTimeIntervalsCore(DateTime date) {
			TimeScaleIntervalCollection intervals = (TimeScaleIntervalCollection)CreateTimeIntervalCollection();
			TimeScale scale = intervals.Scale;
			DateTime start = scale.Floor(date);
			DateTime lastDate = start;
			for (int i = 0; i < VisibleIntervalCount; i++) {
				TimeInterval interval = CalculateScaleInterval(scale, lastDate);
				intervals.Add(interval);
				lastDate = interval.End;
			}
			return intervals;
		}
		protected TimeInterval CalculateScaleInterval(TimeScale scale, DateTime start) {
			return new TimeInterval(start, scale.GetNextDate(start));
		}
		protected internal virtual void UpdateScalesFirstDayOfWeek(DayOfWeek firstDayOfWeek) {
			int count = Scales.Count;
			for (int i = 0; i < count; i++)
				Scales[i].SetFirstDayOfWeek(firstDayOfWeek);
		}
		protected internal override ViewPainterBase CreateViewPainter() {
			return new TimelineViewPainterFlat();
		}
		protected internal TimeScaleCollection GetActualScales() {
			return TimeScaleCollectionHelper.SelectVisibleScales(Scales);
		}
	}
	#endregion
	#region TimelineCells
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "timelinecells.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.TimelineCells", "TimelineCells"),
	Description("A time cell control for the timeline style report.")
	]
	public class TimelineCells : TimeCellsControlBase, ISupportTimeline {
		internal const bool DefaultShowMoreItems = true;
		internal const ControlContentLayoutType DefaultVerticalLayoutType = ControlContentLayoutType.Fit;		
		#region Fields
		bool showMoreItems;
		#endregion
		public TimelineCells() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public override HorizontalHeadersControlBase HorizontalHeaders {
			get {
				return base.HorizontalHeaders;
			}
			set {
				base.HorizontalHeaders = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsShowMoreItems"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultShowMoreItems)]
		public bool ShowMoreItems { get { return showMoreItems; } set { if (showMoreItems == value) return; showMoreItems = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsView"),
#endif
	   Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public new ReportTimelineView View { get { return (ReportTimelineView)base.View; } set { base.View = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsCanShrink"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool CanShrink { get { return base.CanShrink; } set { base.CanShrink = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsCanGrow"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool CanGrow { get { return base.CanGrow; } set { base.CanGrow = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineCellsVerticalLayoutType"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultVerticalLayoutType)]
		public ControlContentLayoutType VerticalLayoutType { get { return LayoutOptionsVertical.LayoutType; } set { LayoutOptionsVertical.LayoutType = value; } }
		protected override int DefaultWidth { get { return 550; } }
		protected override int DefaultHeight { get { return 150; } }
		protected internal new TimelineControlPainter Painter { get { return (TimelineControlPainter)base.Painter; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportTimelineView) }; } }
		protected internal new TimelineCellsPrintController PrintController { get { return (TimelineCellsPrintController)base.PrintController; } }
		protected internal override bool ActualShowMoreItems { get { return showMoreItems; } }
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			this.showMoreItems = DefaultShowMoreItems;
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new TimelineCellsPrintController(View, View);
		}	
		protected internal override ControlContentAnchorType CalculateHorizontalAnchorType() {
			ISchedulerDateIterator dateIterator = GetHorizontalMasterDateIterator();
			if (dateIterator != null)
				return ControlContentAnchorType.Snap;
			else
				return ControlContentAnchorType.Fit;
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = VisibleIntervalCount;
			PrintController.AllowMultiColumn = true;
			PrintController.IntervalsSplitting = View.VisibleIntervalsSplitting;
			PrintController.FirstDayOfWeek = View.GetFirstDayOfWeek();
		}
		protected override BaseHeaderAppearance CreateAppearance() {
			return new ReportTimelineViewAppearance();
		}
		#region ISupportTimeline Members
		BaseViewAppearance ISupportTimeline.TimelinePaintAppearance {
			get { return (BaseViewAppearance)Appearance; }
		}
		WorkDaysCollection ISupportTimeline.WorkDays {
			get { return View.GetWorkDays(); }
		}
		#endregion
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptions() {
			return new ReportAppointmentDisplayOptions();
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new TimelineControlPainter();
		}
		protected internal override CellsLayoutStrategyBase CreateLayoutStrategy() {
			return new TimelineCellsLayoutStrategy(this);
		}
	}
	#endregion
	public enum VisibleIntervalsSplitting {
		None, Hour, Day, Week, Month, Quarter, Year
	}
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	public class TimelineControlPainter : TimeCellsControlPainter {
		public TimelineControlPainter()
			: base() {
		}
		public new TimelineCellsPainter CellPainter { get { return (TimelineCellsPainter)base.CellPainter; } }
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			return new AppointmentPainter();
		}
		protected internal override ViewInfoPainterBase CreateCellPainter() {
			return new TimelineCellsPainter();
		}
		protected internal override void PrintCells(GraphicsCache cache, SchedulerViewCellContainerCollection cellContainers, ISupportCustomDraw customDrawProvider) {
			CellPainter.DrawTimelines(cache, cellContainers, customDrawProvider);
		}
	}
	public class TimelineCellsPainter : TimelinePainter {
		protected internal virtual void DrawTimelines(GraphicsCache cache, SchedulerViewCellContainerCollection timeLines, ISupportCustomDraw customDrawProvider) {
			int count = timeLines.Count;
			for (int i = 0; i < count; i++) {
				ReportTimelineCellContainer viewInfo = (ReportTimelineCellContainer)timeLines[i];
				Draw(cache, viewInfo, customDrawProvider);
			}
		}
	}
	public class CellsLayoutPreliminaryInfos : DXCollection<CellsLayoutPreliminaryInfo> {
	}
	public class CellsLayoutPreliminaryInfo {
		AnchorInfo containersAnchor;
		CellsLayerInfos layers;
		public CellsLayoutPreliminaryInfo(AnchorInfo containersAnchor, CellsLayerInfos layers) {
			XtraSchedulerDebug.Assert(containersAnchor.InnerAnchors.Count == layers.Count);
			this.containersAnchor = containersAnchor;
			this.layers = layers;
		}
		public CellsLayerInfos CellLayers { get { return layers; } }
		public AnchorInfo ContainersAnchor { get { return containersAnchor; } }
	}
}
