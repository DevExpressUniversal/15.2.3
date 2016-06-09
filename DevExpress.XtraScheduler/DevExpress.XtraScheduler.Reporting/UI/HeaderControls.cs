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
using DevExpress.XtraScheduler.Reporting.Native;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraReports;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting {
	public abstract class HeadersControlBase : ReportRelatedControlBase {
		protected HeadersControlBase() : base() {			
		}
		protected new HeadersControlPrintInfo PrintInfo { get { return (HeadersControlPrintInfo)base.PrintInfo; } }
		protected internal new SchedulerHeaderPainter Painter { get { return (SchedulerHeaderPainter)base.Painter; } }
		protected internal SchedulerHeaderCollection Headers { get { return PrintInfo.Headers; } }
		protected override BaseHeaderAppearance CreateAppearance() {
			return new BaseHeaderAppearance();
		}
		protected override void ApplyPrintColorSchema() {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			ColorConverterHelper helper = new ColorConverterHelper(converter);
			helper.ApplyColorConverterToHeaders(Headers);
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new SchedulerHeaderFlatPrintPainter();
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return new HeadersControlPrintInfo(this);
		}
	}
	public abstract class HorizontalHeadersControlBase : HeadersControlBase {
		protected override int DefaultWidth { get { return 550; } }
		protected override int DefaultHeight { get { return 26; } }
	}
	public abstract class VerticalHeadersControlBase : HeadersControlBase {
		protected override int DefaultWidth { get { return 25; } }
		protected override int DefaultHeight { get { return 220; } }
	}	
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "resourcehorizontalheaders.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.HorizontalResourceHeaders", "HorizontalResourceHeaders"),
	Description("A control used to print horizontal captions containing resource names.")
	]
	public class HorizontalResourceHeaders : HorizontalHeadersControlBase, ISchedulerResourceIterator {
		ReportResourceHeaderOptions options;
		public HorizontalResourceHeaders()
			: base() {
			this.options = new ReportResourceHeaderOptions();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					this.options = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalResourceHeadersHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalDateHeaders HorizontalHeaders {
			get {
				return base.HorizontalHeaders as HorizontalDateHeaders;
			}
			set { base.HorizontalHeaders = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalResourceHeadersOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportResourceHeaderOptions Options { get { return options; } }
		protected internal new ResourcePrintControllerBase PrintController { get { return (ResourcePrintControllerBase)base.PrintController; } }
		protected internal ResourceBaseCollection PrintResources { get { return PrintController.PrintResources; } }
		#endregion
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new HorizontalResourceHeadersPrintController(View);
		}		
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = CalculateVisibleResourceCount();
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			BaseViewAppearance appearance = View.PrintAppearance;
			Appearance.HeaderCaption.Assign(appearance.ResourceHeaderCaption);
			Appearance.HeaderCaptionLine.Assign(appearance.ResourceHeaderCaptionLine);
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			HorizontalResourceHeadersLayoutCalculator calculator = new HorizontalResourceHeadersLayoutCalculator(this);
			calculator.CalculateLayout(info);
		}
		#region ISchedulerResourceIterator Members
		ResourceDataCache ISchedulerResourceIterator.GetResourceDataCache() {
			EnsurePrintController();
			return PrintController.DataCache;
		}
		#endregion
		#region CustomDrawResourceHeader
		static readonly object CustomDrawResourceHeaderEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalResourceHeadersCustomDrawResourceHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawResourceHeader
		{
			add { Events.AddHandler(CustomDrawResourceHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawResourceHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawResourceHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawResourceHeaderEvent, e);
		}
		#endregion
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "resourceverticalheaders.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.VerticalResourceHeaders", "VerticalResourceHeaders"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.VerticalResourceHeadersDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.VerticalResourceHeadersDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A control used to print vertical captions containing resource names.")
	]
	public class VerticalResourceHeaders : VerticalHeadersControlBase, ISchedulerResourceIterator {
		ReportResourceHeaderOptions options;
		public VerticalResourceHeaders()
			: base() {
			this.options = new ReportResourceHeaderOptions();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					this.options = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Properties
		protected internal new ResourcePrintControllerBase PrintController { get { return (ResourcePrintControllerBase)base.PrintController; } }
		protected internal ResourceBaseCollection PrintResources { get { return PrintController.PrintResources; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("VerticalResourceHeadersTimeCells"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new TimeCellsControlBase TimeCells {
			get {
				return base.TimeCells as TimeCellsControlBase;
			}
			set { base.TimeCells = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("VerticalResourceHeadersCorners"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ControlCornersOptions Corners { get { return CornersOptionsInternal; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("VerticalResourceHeadersOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportResourceHeaderOptions Options { get { return options; } }
		#endregion
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new ReportSchedulerHeaderVerticalPainter();
		}	   
		protected internal override TimeCellsControlBase GetTimeCells() {
			return LayoutOptionsVertical != null ? LayoutOptionsVertical.MasterControl as TimeCellsControlBase : null;
		}
		protected internal override void SetTimeCells(TimeCellsControlBase value) {
			if (LayoutOptionsVertical != null)
				LayoutOptionsVertical.MasterControl = value;
		}
		protected internal override ControlContentAnchorType CalculateVerticalAnchorType() {
			if (TimeCells != null)
				return ControlContentAnchorType.Snap;
			else
				return ControlContentAnchorType.Fit;
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new VerticalResourceHeadersPrintController(View);
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = CalculateVisibleResourceCount();
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			BaseViewAppearance appearance = View.PrintAppearance;
			Appearance.HeaderCaption.Assign(appearance.ResourceHeaderCaption);
			Appearance.HeaderCaptionLine.Assign(appearance.ResourceHeaderCaptionLine);
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			VerticalResourceHeadersLayoutCalculator calculator = new VerticalResourceHeadersLayoutCalculator(this);
			calculator.CalculateLayout(info);
		}
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
			float topShift = XRConvert.Convert(this.Corners.Top, Dpi, GraphicsDpi.Pixel);	
			for (int i = 0; i < count; i++)
				points[i] += topShift;
		}
		#region ISchedulerResourceIterator Members
		ResourceDataCache ISchedulerResourceIterator.GetResourceDataCache() {
			EnsurePrintController();
			return PrintController.DataCache;
		}
		#endregion
		#region CustomDrawResourceHeader
		static readonly object CustomDrawResourceHeaderEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("VerticalResourceHeadersCustomDrawResourceHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawResourceHeader
		{
			add { Events.AddHandler(CustomDrawResourceHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawResourceHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawResourceHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawResourceHeaderEvent, e);
		}
		#endregion
	}
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "dateheaders.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.HorizontalDateHeaders", "HorizontalDateHeaders"),
	Description("A control used to print horizontal captions containing dates.")
	]
	public class HorizontalDateHeaders : HorizontalHeadersControlBase, ISchedulerDateIterator {
		public HorizontalDateHeaders()
			: base() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalDateHeadersHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalResourceHeaders HorizontalHeaders {
			get {
				return base.HorizontalHeaders as HorizontalResourceHeaders;
			}
			set { base.HorizontalHeaders = value; }
		}
		protected internal new HorizontalDateHeadersPrintController PrintController { get { return (HorizontalDateHeadersPrintController)base.PrintController; } }
		protected internal TimeIntervalCollection PrintIntervals { get { return PrintController.PrintTimeIntervals; } }
		protected internal WeekDays VisibleWeekDays { get { return PrintController.VisibleWeekDays; } set { PrintController.VisibleWeekDays = value; } }
		#endregion
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new HorizontalDateHeadersPrintController(View);
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			HeaderCaptionFormatProviderBase provider = View.GetHeaderCaptionFormatProvider();
			DateHeadersLayoutCalculator calculator = new DateHeadersLayoutCalculator(this, provider);
			calculator.CalculateLayout(info);
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = VisibleIntervalCount;
			PrintController.AllowMultiColumn = true;
		}
		#region ISchedulerDateIterator Members
		TimeIntervalDataCache ISchedulerDateIterator.GetTimeIntervalDataCache() {
			EnsurePrintController();
			return PrintController.DataCache;
		}
		int ISchedulerDateIterator.VisibleIntervalColumnCount { get { return GetActualVisibleIntervalColumnCount(); } }
		ColumnArrangementMode ISchedulerDateIterator.ColumnArrangement { get { return View != null ? View.ColumnArrangement : ReportViewBase.DefaultColumnArrangement; } }
		#endregion
		#region CustomDrawDayHeader
		static readonly object CustomDrawDayHeaderEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalDateHeadersCustomDrawDayHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayHeader
		{
			add { Events.AddHandler(CustomDrawDayHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayHeaderEvent, e);
		}
		#endregion
	}
	#region WeekDayHeaders
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "dayofweekheaders.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.DayOfWeekHeaders", "DayOfWeekHeaders"),
	Description("A control to print headers indicating the days of week.")
	]
	public class DayOfWeekHeaders : HorizontalHeadersControlBase {
		public DayOfWeekHeaders()
			: base() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayOfWeekHeadersTimeCells"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalWeek TimeCells {
			get {
				return base.TimeCells as HorizontalWeek;
			}
			set { base.TimeCells = value; }
		}
		protected internal new DayOfWeekHeadersPrintController PrintController { get { return (DayOfWeekHeadersPrintController)base.PrintController; } }
		[Category(SRCategoryNames.Scheduler), DefaultValue(null), 
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayOfWeekHeadersView")
#else
	Description("")
#endif
]
		public new ReportWeekView View { get { return (ReportWeekView)base.View; } set { base.View = value; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportWeekView) }; } }
		protected internal bool ActualCompressWeekend {
			get {
				HorizontalWeek weekControl = LayoutOptionsHorizontal.MasterControl as HorizontalWeek;
				if (weekControl != null)
					return weekControl.ActualCompressWeekend;
				else
					return HorizontalWeek.DefaultCompressWeekend;
			}
		}
		#endregion
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DayOfWeekHeadersPrintController(View);
		}		
		protected internal override ControlContentAnchorType CalculateHorizontalAnchorType() {			
				return ControlContentAnchorType.Snap;
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			DayOfWeekHeadersLayoutCalculator calculator = new DayOfWeekHeadersLayoutCalculator(this);
			calculator.CalculateLayout(info);
		}		
		#region CustomDrawDayOfWeekHeader
		static readonly object CustomDrawDayOfWeekHeaderEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("DayOfWeekHeadersCustomDrawDayOfWeekHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayOfWeekHeader
		{
			add { Events.AddHandler(CustomDrawDayOfWeekHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayOfWeekHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayOfWeekHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayOfWeekHeaderEvent, e);
		}
		#endregion
	}
	#endregion
	#region TimelineScaleHeaders
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "timelineheaders.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.TimelineScaleHeaders", "TimelineScaleHeaders"),
	Description("A control for printing the time scale captions.")
	]
	public class TimelineScaleHeaders : HorizontalHeadersControlBase, ISchedulerDateIterator {
		#region Fields
		TimeScaleCollection scales = new TimeScaleCollection();		
		#endregion
		public TimelineScaleHeaders()
			: base() {
		}
		public TimelineScaleHeaders(ReportTimelineView timelineView) {
		}
		#region Properties
		protected override int DefaultHeight { get { return base.DefaultHeight * 2; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineScaleHeadersHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalResourceHeaders HorizontalHeaders {
			get {
				return base.HorizontalHeaders as HorizontalResourceHeaders;
			}
			set { base.HorizontalHeaders = value; }
		}
		protected internal new TimelineScaleHeadersPrintController PrintController { get { return (TimelineScaleHeadersPrintController)base.PrintController; } }
		protected internal TimeScaleCollection Scales { get { return scales; } }
		protected internal SchedulerHeaderLevelCollection ScaleLevels { get { return ((TimeScaleHeadersPrintInfo)PrintInfo).ScaleLevels; } }
		protected internal TimeIntervalCollection PrintIntervals { get { return PrintController.PrintTimeIntervals; } }
		[Category(SRCategoryNames.Scheduler), DefaultValue(null), 
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineScaleHeadersView")
#else
	Description("")
#endif
]
		public new ReportTimelineView View { get { return (ReportTimelineView)base.View; } set { base.View = value; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportTimelineView) }; } }
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			UpdateActualScales();
		}
		protected override BaseHeaderAppearance CreateAppearance() {
			return new ReportTimelineViewAppearance();
		}		
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return new TimeScaleHeadersPrintInfo(this);
		}
		protected override void SetView(ReportViewBase value) {
			base.SetView(value);
			UpdateActualScales();
		}
		protected override void OnViewAfterApplyChanges(object sender, AfterApplyReportControlChangesEventArgs e) {
			if ((e.Actions & ReportControlChangeActions.UpdateTimeScales) != 0)
				UpdateActualScales();
			base.OnViewAfterApplyChanges(sender, e);
		}
		private void UpdateActualScales() {
			if (View == null)
				return;
			Scales.BeginUpdate();
			try {
				Scales.Clear();
				Scales.AddRange(TimeScaleCollectionHelper.SelectVisibleScales(View.Scales));
			}
			finally {
				Scales.EndUpdate();
			}
		}
		public TimeScale GetBaseTimeScale() {
			int count = Scales.Count;
			XtraSchedulerDebug.Assert(count > 0);
			return Scales[count - 1];
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {			
			return new TimelineScaleHeadersPrintController(View);
		}
		protected override void ApplyPrintColorSchema() {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			ColorConverterHelper helper = new ColorConverterHelper(converter);
			int count = ScaleLevels.Count;
			for (int i = 0; i < count; i++) {
				helper.ApplyColorConverterToHeaders(ScaleLevels[i].Headers);
			}
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			TimelineScaleHeadersLayoutCalculator calculator = new TimelineScaleHeadersLayoutCalculator(this);
			calculator.CalculateLayout(info);
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.GroupLength = VisibleIntervalCount;
			PrintController.AllowMultiColumn = true;
			PrintController.IntervalsSplitting = View.VisibleIntervalsSplitting;
			PrintController.FirstDayOfWeek = View.GetFirstDayOfWeek();
		}
		#region ISchedulerDateIterator Members
		TimeIntervalDataCache ISchedulerDateIterator.GetTimeIntervalDataCache() {
			EnsurePrintController();
			return PrintController.DataCache;
		}
		int ISchedulerDateIterator.VisibleIntervalColumnCount { get { return GetActualVisibleIntervalColumnCount(); } }
		ColumnArrangementMode ISchedulerDateIterator.ColumnArrangement { get { return View != null ? View.ColumnArrangement : ReportViewBase.DefaultColumnArrangement; } }
		#endregion
		#region CustomDrawDayHeader
		static readonly object CustomDrawDayHeaderEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimelineScaleHeadersCustomDrawDayHeader"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayHeader
		{
			add { Events.AddHandler(CustomDrawDayHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayHeaderEvent, e);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	public class HeadersControlPrintInfo : ControlPrintInfo {
		SchedulerHeaderCollection headers;
		public HeadersControlPrintInfo(HeadersControlBase control)
			: base(control) {
			this.headers = new SchedulerHeaderCollection();
		}
		protected internal new HeadersControlBase Control { get { return (HeadersControlBase)base.Control; } }
		public SchedulerHeaderCollection Headers { get { return headers; } set { headers = value; } }
		public override void Print(GraphicsCache cache) {
			Control.Painter.DrawHeaders(cache, Headers, Control);
		}
		protected internal override ControlPrintInfo CloneCore() {
			HeadersControlPrintInfo printInfo = new HeadersControlPrintInfo(Control);
			printInfo.Headers.AddRange(Headers);
			return printInfo;
		}
	}
	public class TimeScaleHeadersPrintInfo : HeadersControlPrintInfo {
		SchedulerHeaderLevelCollection scaleLevels;
		public TimeScaleHeadersPrintInfo(TimelineScaleHeaders control)
			: base(control) {
			this.scaleLevels = new SchedulerHeaderLevelCollection();
		}
		protected internal new TimelineScaleHeaders Control { get { return (TimelineScaleHeaders)base.Control; } }
		public SchedulerHeaderLevelCollection ScaleLevels { get { return scaleLevels; } }
		public override void Print(GraphicsCache cache) {
			int count = ScaleLevels.Count;
			for (int i = 0; i < count; i++)
				Control.Painter.DrawHeaders(cache, ScaleLevels[i].Headers, Control);
		}
		protected internal override ControlPrintInfo CloneCore() {
			TimeScaleHeadersPrintInfo printInfo = new TimeScaleHeadersPrintInfo(Control);
			printInfo.ScaleLevels.AddRange(ScaleLevels);
			return printInfo;
		}
	}
	public class ReportSchedulerHeaderVerticalPainter : SchedulerHeaderVerticalFlatPrintPainter {
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
	}
}
