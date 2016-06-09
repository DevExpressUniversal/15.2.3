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
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.XtraScheduler.Printing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Reporting {
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "dayview.bmp"),
	Description("A View component for a daily style report.")
	]
	public class ReportDayView : ReportViewBase {
		public ReportDayView() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportDayViewVisibleDayCount"),
#endif
DefaultValue(DefaultVisibleIntervalCount), Category(SRCategoryNames.Layout)]
		public int VisibleDayCount { get { return base.VisibleIntervalCount; } set { base.VisibleIntervalCount = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportDayViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportDayViewAppearance Appearance { get { return (ReportDayViewAppearance)base.Appearance; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportDayViewVisibleDayColumnCount"),
#endif
DefaultValue(DefaultVisibleIntervalColumnCount), Category(SRCategoryNames.Layout)]
		public int VisibleDayColumnCount { get { return base.VisibleIntervalColumnCount; } set { base.VisibleIntervalColumnCount = value; } }
		#endregion
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			return new DayIntervalCollection();
		}
		protected internal override TimeIntervalFormatType GetDefaultTimeIntervalFormatType() {
			return TimeIntervalFormatType.Daily;
		}
		protected override BaseViewAppearance CreateAppearance() {
			return new ReportDayViewAppearance();
		}
		protected internal override ViewPainterBase CreateViewPainter() {
			return new DayViewPainterFlat();
		}
		protected internal override TimeIntervalCollection CreateFakeTimeIntervalsCore(DateTime date) {
			TimeIntervalCollection result = CreateTimeIntervalCollection();
			result.Add(new TimeInterval(date.Date, TimeSpan.FromDays(VisibleDayCount)));
			return result;
		}
	}
	public abstract class DayViewControlBase : ReportRelatedControlBase {
		protected DayViewControlBase()
			: base() {
		}
		protected DayViewControlBase(ReportDayView dayView)
			: base(dayView) {
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewControlBaseView"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public new ReportDayView View { get { return (ReportDayView)base.View; } set { base.View = value; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportDayView) }; } }
	}
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "daycells.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.DayViewTimeCells", "DayViewTimeCells"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.DayViewTimeCellsDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.DayViewTimeCellsDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A time cell control used for printing the Day View.")
	]
	public class DayViewTimeCells : TimeCellsControlBase {
		#region Constants
		internal const bool DefaultShowWorkTimeOnly = false;
		internal const bool DefaultShowAllDayArea = true;
		internal const bool DefaultShowAllAppointmentsAtTimeCells = false;
		internal const bool DefaultVisibleTimeSnapMode = false;
		internal const WeekDays DefaultVisibleWeekDays = WeekDays.EveryDay;
		internal const ControlContentLayoutType DefaultVerticalLayoutType = ControlContentLayoutType.Fit;
		internal const int DefaultStatusLineWidth = 0;
		internal static readonly TimeSpan DefaultTimeScale = TimeSpan.FromMinutes(30);
		static readonly TimeOfDayInterval DefaultVisibleTime = TimeOfDayInterval.Day;
		#endregion
		#region Fields
		bool showWorkTimeOnly;
		bool showAllDayArea;
		bool showAllAppointmentsAtTimeCells;
		bool visibleTimeSnapMode;
		int statusLineWidth;
		TimeSpan timeScale;
		TimeOfDayInterval visibleTime;
		ExtraCellsOptions extraCells;
		WeekDays visibleWeekDays;
		#endregion
		public DayViewTimeCells()
			: base() {
		}
		public DayViewTimeCells(ReportDayView view)
			: base(view) {
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					UnsubscribeViewEvents();
					if (extraCells != null)
						extraCells = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#region Public Properties
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportDayViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (ReportDayViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#endregion
		#region HorizontalHeaders
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsHorizontalHeaders"),
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
		#endregion
		#region TimeScale
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsTimeScale"),
#endif
Category(SRCategoryNames.Layout)]
		public TimeSpan TimeScale {
			get { return timeScale; }
			set {
				TimeSpan newVal = ValidateTimeScale(value);
				if (timeScale == newVal)
					return;
				timeScale = newVal;
				UpdatePrintController();
			}
		}
		internal TimeSpan ValidateTimeScale(TimeSpan value) {
			if (value <= TimeSpan.Zero)
				Exceptions.ThrowArgumentException("TimeScale", value);
			return DateTimeHelper.Min(value, TimeSpan.FromDays(1));
		}
		#endregion
		#region VisibleTime
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsVisibleTime"),
#endif
Category(SRCategoryNames.Layout)]
		public TimeOfDayInterval VisibleTime {
			get { return visibleTime; }
			set {
				if (visibleTime == value)
					return;
				visibleTime = value.Clone();
			}
		}
		#endregion
		#region VisibleTimeSnapMode
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsVisibleTimeSnapMode"),
#endif
Category(SRCategoryNames.Layout)]
		public bool VisibleTimeSnapMode {
			get { return visibleTimeSnapMode; }
			set {
				if (visibleTimeSnapMode == value)
					return;
				visibleTimeSnapMode = value;
			}
		}
		#endregion
		#region ExtraCells
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsExtraCells"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtraCellsOptions ExtraCells { get { return extraCells; } }
		#endregion
		#region ShowWorkTimeOnly
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsShowWorkTimeOnly"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultShowWorkTimeOnly)]
		public bool ShowWorkTimeOnly { get { return showWorkTimeOnly; } set { if (showWorkTimeOnly == value) return; showWorkTimeOnly = value; } }
		#endregion
		#region StatusLineWidth
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsStatusLineWidth"),
#endif
DefaultValue(DefaultStatusLineWidth), Category(SRCategoryNames.Layout)]
		public int StatusLineWidth {
			get { return statusLineWidth; }
			set {
				value = Math.Max(0, value);
				if (value == statusLineWidth)
					return;
				this.statusLineWidth = value;
			}
		}
		#endregion
		#region ShowAllDayArea
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsShowAllDayArea"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultShowAllDayArea)]
		public bool ShowAllDayArea { get { return showAllDayArea; } set { showAllDayArea = value; } }
		#endregion
		#region VisibleWeekDays
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsVisibleWeekDays"),
#endif
	 Category(SRCategoryNames.Layout), DefaultValue(DefaultVisibleWeekDays),
	Editor("DevExpress.XtraScheduler.Reporting.Design.WeekDaysEditor," + AssemblyInfo.SRAssemblySchedulerReportingExtensions, typeof(System.Drawing.Design.UITypeEditor)),
	TypeConverter("DevExpress.XtraScheduler.Reporting.Design.WeekDaysConverter," + AssemblyInfo.SRAssemblySchedulerReportingExtensions)
	 ]
		public WeekDays VisibleWeekDays {
			get { return visibleWeekDays; }
			set {
				if (visibleWeekDays == value)
					return;
				visibleWeekDays = value == 0 ? WeekDays.EveryDay : value;
				UpdatePrintController();
			}
		}
		#endregion
		#region ShowAllAppointmentsAtTimeCells
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsShowAllAppointmentsAtTimeCells"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultShowAllAppointmentsAtTimeCells)]
		public bool ShowAllAppointmentsAtTimeCells { get { return showAllAppointmentsAtTimeCells; } set { showAllAppointmentsAtTimeCells = value; } }
		#endregion
		#region PrintColorSchemas
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsPrintColorSchemas"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DayViewTimeCellsPrintColorSchemaOptions PrintColorSchemas { get { return (DayViewTimeCellsPrintColorSchemaOptions)base.PrintColorSchemas; } }
		#endregion
		#region View
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsView"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public new ReportDayView View { get { return (ReportDayView)base.View; } set { base.View = value; } }
		#endregion
		#region VerticalLayoutType
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsVerticalLayoutType"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultVerticalLayoutType)]
		public ControlContentLayoutType VerticalLayoutType { get { return LayoutOptionsVertical.LayoutType; } set { LayoutOptionsVertical.LayoutType = value; } }
		#endregion
		internal bool ShouldSerializeTimeScale() {
			return TimeScale != DefaultTimeScale;
		}
		internal void ResetTimeScale() {
			TimeScale = DefaultTimeScale;
		}
		internal bool ShouldSerializeVisibleTime() {
			return !VisibleTime.IsEqual(DefaultVisibleTime);
		}
		internal void ResetVisibleTime() {
			VisibleTime = DefaultVisibleTime;
		}
		#endregion
		#region Internal Properties
		internal bool ActualShowAllDayArea { get { return ShowAllDayArea && !IsTiledAtDesignMode; } }
		internal bool ActualShowExtraCells { get { return ExtraCells.Visible && !IsTiledAtDesignMode; } }
		protected internal override bool ActualShowMoreItems { get { return false; } }
		protected internal bool ActualShowAllAppointmentsAtTimeCells { get { return !ActualShowAllDayArea || ShowAllAppointmentsAtTimeCells; } }
		protected override int DefaultWidth { get { return 550; } }
		protected override int DefaultHeight { get { return 350; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportDayView) }; } }
		protected internal override bool DrawMoreButtonsOverAppointmentsInternal { get { return true; } }
		internal new DayCellsPrintController PrintController { get { return (DayCellsPrintController)base.PrintController; } }
		internal new DayViewTimeCellsPainter Painter { get { return (DayViewTimeCellsPainter)base.Painter; } }
		protected new DayViewTimeCellsPrintInfo PrintInfo { get { return (DayViewTimeCellsPrintInfo)base.PrintInfo; } }
		protected internal new ReportDayViewAppearance Appearance { get { return (ReportDayViewAppearance)base.Appearance; } }
		protected internal new DayViewAppointmentsLayoutResult AppointmentsLayoutResult { get { return PrintInfo.AppointmentsLayoutResult; } }
		#endregion
		#region Events
		#region CustomDrawDayViewAllDayArea
		static readonly object CustomDrawDayViewAllDayAreaEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeCellsCustomDrawDayViewAllDayArea"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayViewAllDayArea {
			add { Events.AddHandler(CustomDrawDayViewAllDayAreaEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayViewAllDayAreaEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayViewAllDayAreaCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayViewAllDayAreaEvent, e);
		}
		#endregion
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			this.showWorkTimeOnly = DefaultShowWorkTimeOnly;
			this.showAllDayArea = DefaultShowAllDayArea;
			this.showAllAppointmentsAtTimeCells = DefaultShowAllAppointmentsAtTimeCells;
			this.timeScale = DefaultTimeScale;
			this.extraCells = new ExtraCellsOptions();
			this.visibleTime = DefaultVisibleTime.Clone();
			this.visibleWeekDays = DefaultVisibleWeekDays;
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return new DayViewTimeCellsPrintInfo(this);
		}
		internal static TimeOfDayInterval GetDefaultVisibleTime() {
			return DefaultVisibleTime.Clone();
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = VisibleIntervalCount;
			PrintController.AllowMultiColumn = true;
			PrintController.TimeScale = TimeScale;
			PrintController.VisibleWeekDays = VisibleWeekDays;
		}
		protected override BaseHeaderAppearance CreateAppearance() {
			return new ReportDayViewAppearance();
		}
		protected internal override PrintColorSchemaOptions CreatePrintColorSchemas() {
			return new DayViewTimeCellsPrintColorSchemaOptions();
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			DayViewAppearance appearance = (DayViewAppearance)View.PrintAppearance;
			Appearance.AllDayArea.Assign(appearance.AllDayArea);
			Appearance.AllDayAreaSeparator.Assign(appearance.AllDayAreaSeparator);
			Appearance.SelectedAllDayArea.Assign(appearance.SelectedAllDayArea);
		}
		protected override void ApplyPrintColorSchema() {
			base.ApplyPrintColorSchema();
			AppointmentsLayoutResult.AppointmentStatusViewInfos.ForEach(ApplyColorConverterToAllDayAppointmentsStatuses);
		}
		protected override void ApplyColorConverterToContainer(SchedulerViewCellContainer cellContainer) {
			base.ApplyColorConverterToContainer(cellContainer);
			DayViewColumn column = (DayViewColumn)cellContainer;
			if (column.AllDayAreaCell != null) {
				PrintColorConverter converter = GetColorConverter(PrintColorSchemas.AllDayArea);
				converter.ConvertAllDayAreaAppearance(column.AllDayAreaCell.Appearance);
			}
			column.ExtendedCells.ForEach(ApplyColorConverterToCell);
		}
		protected void ApplyColorConverterToAllDayAppointmentsStatuses(AppointmentStatusViewInfo status) {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.AllDayAppointmentStatus);
			status.SetBrush(converter.ConvertBrush(status.GetBrush()));
			status.BorderColor = converter.ConvertColor(status.BorderColor);
		}
		protected internal override ControlContentAnchorType CalculateHorizontalAnchorType() {
			ISchedulerDateIterator dateIterator = GetHorizontalMasterDateIterator();
			return dateIterator != null ? ControlContentAnchorType.Snap : ControlContentAnchorType.Fit;
		}
		protected internal virtual TimeOfDayInterval GetColumnVisibleTime(TimeInterval columnInterval, Resource resource) {
			if (ShowWorkTimeOnly) {
				TimeOfDayIntervalCollection workTimes = View.GetWorkTime(columnInterval, resource);
				return new TimeOfDayInterval(workTimes.Start, workTimes.End);
			} else
				return VisibleTime;
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new DayViewTimeCellsPainter();
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DayCellsPrintController(View, View);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptions() {
			return new ReportDayViewAppointmentDisplayOptions();
		}
		protected internal override CellsLayoutStrategyBase CreateLayoutStrategy() {
			return new DayViewCellsLayoutStrategy(this);
		}
		protected internal override void SynchronizeMasterControlsProperties() {
			HorizontalDateHeaders horizontalHeaders = GetHorizontalMasterDateIterator() as HorizontalDateHeaders;
			if (horizontalHeaders != null)
				horizontalHeaders.VisibleWeekDays = this.VisibleWeekDays;
		}
	}
	#region DayViewTimeRuler
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "ruler.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.DayViewTimeRuler", "DayViewTimeRuler"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.DayViewTimeRulerDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.DayViewTimeRulerDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A time ruler for the daily style report.")
	]
	public class DayViewTimeRuler : DayViewControlBase, ISupportTimeRuler {
		TimeRuler timeRuler;
		public DayViewTimeRuler(ReportDayView dayView)
			: base(dayView) {
		}
		public DayViewTimeRuler()
			: base() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeRulerTimeCells"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new DayViewTimeCells TimeCells {
			get {
				return base.TimeCells as DayViewTimeCells;
			}
			set { base.TimeCells = value; }
		}
		[Category(SRCategoryNames.Layout), DefaultValue(null),
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeRulerTimeRuler"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TimeRuler TimeRuler { get { return timeRuler; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeRulerCorners"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ControlCornersOptions Corners { get { return CornersOptionsInternal; } }
		protected internal new TimeRulerPrintController PrintController { get { return (TimeRulerPrintController)base.PrintController; } }
		protected internal new ReportDayViewTimeRulerPainter Painter { get { return (ReportDayViewTimeRulerPainter)base.Painter; } }
		protected override int DefaultWidth { get { return 50; } }
		protected override int DefaultHeight { get { return 350; } }
		protected internal new ReportDayViewAppearance Appearance { get { return (ReportDayViewAppearance)base.Appearance; } }
		protected new DayViewTimeRulerPrintInfo PrintInfo { get { return (DayViewTimeRulerPrintInfo)base.PrintInfo; } }
		protected internal TimeRulerViewInfoCollection TimeRulerViewInfos { get { return PrintInfo.TimeRulerViewInfos; } }
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			this.timeRuler = CreateTimeRuler();
		}
		protected override BaseHeaderAppearance CreateAppearance() {
			return new ReportDayViewAppearance();
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return new DayViewTimeRulerPrintInfo(this);
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			DayViewAppearance appearance = (DayViewAppearance)View.PrintAppearance;
			Appearance.TimeRuler.Assign(appearance.TimeRuler);
			Appearance.TimeRulerHourLine.Assign(appearance.TimeRulerHourLine);
			Appearance.TimeRulerLine.Assign(appearance.TimeRulerLine);
			Appearance.TimeRulerNowArea.Assign(appearance.TimeRulerNowArea);
			Appearance.TimeRulerNowLine.Assign(appearance.TimeRulerNowLine);
		}
		protected override void ApplyPrintColorSchema() {
			int count = TimeRulerViewInfos.Count;
			for (int i = 0; i < count; i++)
				ApplyColorConverterToTimeRuler(TimeRulerViewInfos[i]);
		}
		void ApplyColorConverterToTimeRuler(TimeRulerViewInfo viewInfo) {
			PrintColorConverter colorConverter = GetColorConverter(PrintColorSchemas.Content);
			colorConverter.ConvertTimeRulerAppearance(viewInfo.BackgroundAppearance);
			colorConverter.ConvertTimeRulerHourLineAppearance(viewInfo.HourLineAppearance);
			colorConverter.ConvertTimeRulerAppearance(viewInfo.LargeHourAppearance);
			colorConverter.ConvertTimeRulerHourLineAppearance(viewInfo.LineAppearance);
			int count = viewInfo.Items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoAppearanceItem item = (ViewInfoAppearanceItem)viewInfo.Items[i];
				ConvertTimeRulerItemAppearance(item, colorConverter);
			}
		}
		protected internal virtual void ConvertTimeRulerItemAppearance(ViewInfoAppearanceItem item, PrintColorConverter colorConverter) {
			ViewInfoAppearanceItem lineItem = item as ViewInfoHorizontalLineItem;
			if (lineItem != null)
				colorConverter.ConvertTimeRulerLineAppearance(item.Appearance);
			else
				colorConverter.ConvertTimeRulerLineAppearance(item.Appearance);
		}
		protected internal override ControlContentAnchorType CalculateVerticalAnchorType() {
			return ControlContentAnchorType.Snap;
		}
		protected internal override TimeCellsControlBase GetTimeCells() {
			return LayoutOptionsVertical != null ? LayoutOptionsVertical.MasterControl as TimeCellsControlBase : null;
		}
		protected internal override void SetTimeCells(TimeCellsControlBase value) {
			if (LayoutOptionsVertical != null)
				LayoutOptionsVertical.MasterControl = value;
		}
		protected internal virtual TimeRuler CreateTimeRuler() {
			return new TimeRuler();
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new ReportDayViewTimeRulerPainter();
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new TimeRulerPrintController(View);
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			DayViewTimeRulerLayoutCalculator calculator = new DayViewTimeRulerLayoutCalculator(Cache, this, Painter);
			calculator.CalculateLayout(info);
		}
		#region ITimeRulerSupport Members
		ITimeRulerFormatStringService ISupportTimeRuler.GetFormatStringProvider() {
			return (ITimeRulerFormatStringService)View.GetService(typeof(ITimeRulerFormatStringService));
		}
		DayViewAppearance ISupportTimeRuler.PaintAppearance {
			get { { return (DayViewAppearance)Appearance; } }
		}
		TimeSpan ISupportTimeRuler.TimeScale {
			get {
				DayViewTimeCells cells = (DayViewTimeCells)LayoutOptionsVertical.MasterControl;
				if (cells != null)
					return cells.TimeScale;
				else
					return DayViewTimeCells.DefaultTimeScale;
			}
		}
		TimeZoneHelper ISupportTimeRuler.TimeZoneHelper {
			get { return SchedulerReport.ActualSchedulerAdapter.TimeZoneHelper; }
		}
		#endregion
		protected internal override SplitPoints GetVerticalSplitPointsCore() {
			if (TimeCells != null) {
				SplitPoints points = TimeCells.GetVerticalSplitPointsCore();
				AdjustSplitPoints(points);
				return points;
			}
			return base.GetVerticalSplitPointsCore();
		}
		protected internal virtual void AdjustSplitPoints(SplitPoints points) {
			int count = points.Count;
			for (int i = 0; i < count; i++)
				points[i] += this.Corners.Top;
		}
		#region CustomDrawDayViewTimeRuler
		static readonly object CustomDrawDayViewTimeRulerEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayViewTimeRulerCustomDrawDayViewTimeRuler"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayViewTimeRuler {
			add { Events.AddHandler(CustomDrawDayViewTimeRulerEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayViewTimeRulerEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayViewTimeRulerCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayViewTimeRulerEvent, e);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	public class DayViewTimeCellsPainter : TimeCellsControlPainter {
		int hiddenAllDayAreaHeight = 0;
		public DayViewTimeCellsPainter()
			: base() {
			this.CellPainter.Initialize();
		}
		public new DayViewColumnPainter CellPainter { get { return (DayViewColumnPainter)base.CellPainter; } }
		public int HiddenAllDayAreaHeight { get { return hiddenAllDayAreaHeight; } }
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			ReportDayViewTimeCellsAppointmentPainter timeCellsAppointmentPainter = new ReportDayViewTimeCellsAppointmentPainter();
			AppointmentPainter allDayAppointmentPainter = new ReportAppointmentPainter();
			return new ReportDayViewDispatchAppointmentPainter(timeCellsAppointmentPainter, allDayAppointmentPainter);
		}
		protected internal override ViewInfoPainterBase CreateCellPainter() {
			return new ReportDayViewColumnPainter();
		}
		protected internal override void PrintCells(GraphicsCache cache, SchedulerViewCellContainerCollection cellContainers, ISupportCustomDraw customDrawProvider) {
			CellPainter.DrawColumns(cache, cellContainers, customDrawProvider);
		}
		protected internal override void PrintAppointments(GraphicsCache cache, AppointmentsLayoutResult aptLayoutResult, ISupportCustomDraw customDrawProvider, IStatusBrushAdapter brushAdapter) {
			FixIt(((DayViewDispatchAppointmentPainter)AppointmentPainter).AllDayAppointmentPainter, brushAdapter);
			FixIt(((DayViewDispatchAppointmentPainter)AppointmentPainter).TimeCellsAppointmentPainter, brushAdapter);
			DayViewAppointmentsLayoutResult dayViewAptLayoutResult = aptLayoutResult as DayViewAppointmentsLayoutResult;
			if (dayViewAptLayoutResult != null)
				PrintAllDayAppointmentStatuses(cache, dayViewAptLayoutResult.AppointmentStatusViewInfos, brushAdapter);
			((ReportAppointmentStatusPainter)AppointmentPainter.StatusPainter).BrushAdapter = brushAdapter;
			base.PrintAppointments(cache, aptLayoutResult, customDrawProvider, brushAdapter);
		}
		void FixIt(Drawing.AppointmentPainter appointmentPainter, IStatusBrushAdapter brushAdapter) {
			((ReportAppointmentStatusPainter)appointmentPainter.StatusPainter).BrushAdapter = brushAdapter;
		}
		protected internal virtual void PrintAllDayAppointmentStatuses(GraphicsCache cache, AppointmentStatusViewInfoCollection statusInfos, IStatusBrushAdapter brushAdapter) {
			int count = statusInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentStatusViewInfo statusInfo = statusInfos[i];
				((ReportAppointmentStatusPainter)AppointmentPainter.StatusPainter).BrushAdapter = brushAdapter;
				AppointmentPainter.StatusPainter.DrawRectangleStatus(cache, statusInfo);
			}
		}
	}
	public class ReportAppointmentPainter : AppointmentPainter {
		public ReportAppointmentPainter() {
		}
		protected override AppointmentStatusPainter CreateStatusPainter() {
			return new ReportAppointmentStatusPainter();
		}
	}
	public class ReportDayViewDispatchAppointmentPainter : DayViewDispatchAppointmentPainter {
		public ReportDayViewDispatchAppointmentPainter(AppointmentPainter timeCellsAppointmentPainter, AppointmentPainter allDayAppointmentPainter)
			: base(timeCellsAppointmentPainter, allDayAppointmentPainter) {
		}
		protected override AppointmentStatusPainter CreateStatusPainter() {
			return new ReportAppointmentStatusPainter();
		}
	}
	public class ReportDayViewTimeCellsAppointmentPainter : DayViewTimeCellsAppointmentPainter {
		public ReportDayViewTimeCellsAppointmentPainter() {
		}
		protected override AppointmentStatusPainter CreateStatusPainter() {
			return new ReportAppointmentStatusPainter();
		}
	}
	public class ReportAppointmentStatusPainter : AppointmentStatusPainter {
		public ReportAppointmentStatusPainter() {
		}
		public IStatusBrushAdapter BrushAdapter { get; set; }
		protected internal override void DrawRectangleStatus(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo) {
			base.DrawRectangleStatus(cache, statusViewInfo);
		}
		protected override void FillStatusBrush(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo) {
			Image image = null;
			if (BrushAdapter != null)
				image = BrushAdapter.CreateImage(statusViewInfo.Bounds, statusViewInfo.Status);
			if (image == null) {
				base.FillStatusBrush(cache, statusViewInfo);
				return;
			}
			cache.Paint.DrawImage(cache.Graphics, image, statusViewInfo.Bounds.Location);
		}
	}
	public class ReportDayViewColumnPainter : DayViewColumnFlatPainter {
		public override int AllDayAreaSeparatorVerticalMargin { get { return 0; } }
		public override void DrawStatusLine(GraphicsCache cache, DayViewColumn column) {
			Brush statusLineBrush = GetStatusLineBrush(cache, column);
			Brush statusLineBorderBrush = GetStatusLineBorderBrush(cache, column);
			DrawStatusLine(cache, column.StatusLineBounds, statusLineBrush, statusLineBorderBrush, column.HasLeftBorder);
		}
	}
	public class ReportDayViewTimeRulerPainter : TimeRulerFlatPainter {
	}
	#region DayViewTimeCellsPrintInfo
	public class DayViewTimeCellsPrintInfo : TimeCellsControlPrintInfo {
		public new DayViewAppointmentsLayoutResult AppointmentsLayoutResult { get { return (DayViewAppointmentsLayoutResult)base.AppointmentsLayoutResult; } }
		public DayViewTimeCellsPrintInfo(TimeCellsControlBase control)
			: base(control) {
		}
		protected internal override AppointmentsLayoutResult CreateAppointmentsLayoutResult() {
			return new DayViewAppointmentsLayoutResult();
		}
		protected internal override ControlPrintInfo CloneCore() {
			DayViewTimeCellsPrintInfo printInfo = new DayViewTimeCellsPrintInfo(Control);
			printInfo.AppointmentsLayoutResult.Merge(AppointmentsLayoutResult);
			printInfo.CellContainers.AddRange(CellContainers);
			return printInfo;
		}
	}
	#endregion
	#region DayViewTimeRulerPrintInfo
	public class DayViewTimeRulerPrintInfo : ControlPrintInfo {
		TimeRulerViewInfoCollection timeRulerViewInfos;
		protected internal new DayViewTimeRuler Control { get { return (DayViewTimeRuler)base.Control; } }
		public TimeRulerViewInfoCollection TimeRulerViewInfos { get { return timeRulerViewInfos; } }
		public DayViewTimeRulerPrintInfo(DayViewTimeRuler control)
			: base(control) {
			timeRulerViewInfos = new TimeRulerViewInfoCollection();
		}
		public override void Print(GraphicsCache cache) {
			Control.Painter.DrawTimeRulers(cache, TimeRulerViewInfos, Control);
		}
		protected internal override ControlPrintInfo CloneCore() {
			DayViewTimeRulerPrintInfo info = new DayViewTimeRulerPrintInfo(Control);
			info.TimeRulerViewInfos.AddRange(TimeRulerViewInfos);
			return info;
		}
	}
	#endregion
}
