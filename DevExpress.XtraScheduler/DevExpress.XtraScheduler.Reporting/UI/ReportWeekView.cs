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
using System.ComponentModel;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System.Drawing;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting {
	#region ReportWeekView
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "weekview.bmp"),
	Description("A View component for a weekly style report.")
	]
	public class ReportWeekView : ReportViewBase {
		public ReportWeekView() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportWeekViewVisibleWeekCount"),
#endif
DefaultValue(DefaultVisibleIntervalCount), Category(SRCategoryNames.Layout)]
		public int VisibleWeekCount { get { return base.VisibleIntervalCount; } set { base.VisibleIntervalCount = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportWeekViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportWeekViewAppearance Appearance { get { return (ReportWeekViewAppearance)base.Appearance; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportWeekViewVisibleWeekDayColumnCount"),
#endif
DefaultValue(DefaultVisibleIntervalColumnCount), Category(SRCategoryNames.Layout)]
		public int VisibleWeekDayColumnCount { get { return base.VisibleIntervalColumnCount; } set { base.VisibleIntervalColumnCount = value; } }
		internal virtual bool ActualExactlyOneMonth { get { return false; } }
		internal virtual WeekControlDataType DataType { get { return WeekControlDataType.Weeks; } }
		#endregion
		protected internal override TimeIntervalFormatType GetDefaultTimeIntervalFormatType() {
			return TimeIntervalFormatType.Weekly;
		}
		protected override BaseViewAppearance CreateAppearance() {
			return new ReportWeekViewAppearance();
		}	   
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			WeekIntervalCollection adjustedIntervals = new WeekIntervalCollection();
			adjustedIntervals.FirstDayOfWeek = GetFirstDayOfWeek();
			adjustedIntervals.CompressWeekend = false;
			return adjustedIntervals;
		}
		protected internal override TimeIntervalCollection CreateFakeTimeIntervalsCore(DateTime date) {
			WeekIntervalCollection weekIntervals = (WeekIntervalCollection)CreateTimeIntervalCollection();
			DateTime start = weekIntervals.RoundToStart(new TimeInterval(date, TimeSpan.Zero));
			TimeInterval interval = new TimeInterval(start, start.AddDays(7 * VisibleWeekCount));
			weekIntervals.Add(interval);
			return weekIntervals;
		}
		protected internal override ViewPainterBase CreateViewPainter() {
			return new WeekViewPainterFlat();
		}		
	}
	#endregion
	#region WeekCellsControlBase (abstract)
	public abstract class WeekCellsControlBase : TimeCellsControlBase, ISupportWeekCells {
		internal const bool DefaultShowMoreItems = true;
		#region Fields
		bool showMoreItems;		
		#endregion
		protected WeekCellsControlBase()
			: base() {
		}
		#region Properties        
		[Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public new ReportWeekView View { get { return (ReportWeekView)base.View; } set { base.View = value; } }
		[DefaultValue(DefaultShowMoreItems), Category(SRCategoryNames.Layout)]
		public bool ShowMoreItems { get { return showMoreItems; } set { if (showMoreItems == value) return; showMoreItems = value; } }		
		protected override int DefaultWidth { get { return 550; } }
		protected override int DefaultHeight { get { return 150; } }
		protected internal override Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportWeekView) }; } }
		protected internal new WeekControlPainterBase Painter { get { return (WeekControlPainterBase)base.Painter; } }
		internal new WeekControlBasePrintController PrintController { get { return (WeekControlBasePrintController)base.PrintController; } }
		protected override bool CanHaveMasterDateIterator { get { return false; } }
		protected internal override bool ActualShowMoreItems { get { return showMoreItems; } }
		protected internal abstract bool ActualCompressWeekend { get;}
		protected internal DayOfWeek ActualFirstDayOfWeek {
			get {
				DayOfWeek firstDayOfWeek = View.GetFirstDayOfWeek();
				if (ActualCompressWeekend && (firstDayOfWeek == DayOfWeek.Sunday))
					return DayOfWeek.Monday;
				return firstDayOfWeek;
			}
		}
		protected internal WeekCollection PrintWeeks { get { return PrintController.PrintWeeks; } }		
		protected internal new ReportWeekViewAppearance Appearance { get { return (ReportWeekViewAppearance)base.Appearance; } }
		internal WeekControlDataType DataType { get { return View.DataType; } }		
		#endregion
		#region CustomDrawDayHeader
		static readonly object CustomDrawDayHeaderEvent = new object();
		[Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawDayHeader {
			add { Events.AddHandler(CustomDrawDayHeaderEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayHeaderEvent, value); }
		}
		protected internal override void RaiseCustomDrawDayHeaderCore(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawEvent(CustomDrawDayHeaderEvent, e);
		}
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			this.showMoreItems = DefaultShowMoreItems;
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.AllowMultiColumn = true;
			PrintController.GroupLength = CalculatePrintControllerGroupLength();
			PrintController.VisibleWeekDays = CalculatePrintControllerVisibleWeekDays();
			PrintController.FirstDayOfWeek = ActualFirstDayOfWeek;			
			PrintController.ExactlyOneMonth = View.ActualExactlyOneMonth;		  
		}
		protected internal virtual int CalculatePrintControllerGroupLength() {
			return DataType == WeekControlDataType.Months ? 1 : View.VisibleIntervalCount;
		}
		protected internal abstract WeekDays CalculatePrintControllerVisibleWeekDays();
		protected override BaseHeaderAppearance CreateAppearance() {
			return new ReportWeekViewAppearance();
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			WeekViewAppearance appearance = (WeekViewAppearance)View.PrintAppearance;
			Appearance.CellHeaderCaption.Assign(appearance.CellHeaderCaption);
			Appearance.CellHeaderCaptionLine.Assign(appearance.CellHeaderCaptionLine);
			Appearance.TodayCellHeaderCaption.Assign(appearance.TodayCellHeaderCaption);
			Appearance.TodayCellHeaderCaptionLine.Assign(appearance.TodayCellHeaderCaptionLine);
		}		
		#region ISupportWeekCells implementation
		WeekViewAppearance ISupportWeekCells.PaintAppearance { get { return (WeekViewAppearance)Appearance; } }
		DayOfWeek ISupportWeekCells.FirstDayOfWeek { get { return View.GetFirstDayOfWeek(); } }
		HeaderCaptionFormatProviderBase ISupportWeekCells.GetCaptionFormatProvider() {
			return View.GetHeaderCaptionFormatProvider();
		}
		bool ISupportWeekCells.CompressWeekend { get { return ActualCompressWeekend; } }
		#endregion
		bool ISupportWeekCells.ShouldHideCellContent(SchedulerViewCellBase cell) {
			if (!View.ActualExactlyOneMonth)
				return false;			
			TimeInterval visibleInterval = PrintController.DataCache.AllColumnPrintIntervals.Interval;
			return !visibleInterval.Contains(cell.Interval);
		}		
		protected override void ApplyColorConverterToCell(SchedulerViewCellBase cell) {			
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			cell.Appearance.BorderColor = Color.Black;
			converter.ConvertAppearance(cell.Appearance);
			ColorConverterHelper helper = new ColorConverterHelper(converter);
			SingleWeekCellBase weekCell = (SingleWeekCellBase)cell;
			helper.ApplyColorConverterToHeader(weekCell.Header);
		}
	}
	#endregion
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "weekcells.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.HorizontalWeek", "HorizontalWeek"),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.HorizontalWeekDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	Designer("DevExpress.XtraScheduler.Reporting.Design.HorizontalWeekDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A time cell control for the multi-week (monthly) report.")
	]
	public class HorizontalWeek : WeekCellsControlBase {
		internal const bool DefaultCompressWeekend = true;
		internal const ControlContentLayoutType DefaultVerticalLayoutType = ControlContentLayoutType.Fit;
		internal const WeekDays DefaultVisibleWeekDays = WeekDays.EveryDay;
		bool compressWeekend;
		WeekDays visibleWeekDays;
		#region Public Properties 
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportMonthViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (ReportMonthViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalResourceHeaders HorizontalHeaders {
			get {
				return base.HorizontalHeaders as HorizontalResourceHeaders;
			}
			set {
				base.HorizontalHeaders = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekCompressWeekend"),
#endif
DefaultValue(DefaultCompressWeekend), Category(SRCategoryNames.Layout)]
		public bool CompressWeekend {
			get { return compressWeekend; }
			set {
				if(compressWeekend == value)
					return;
				compressWeekend = value;
				UpdatePrintController();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekVisibleWeekDays"),
#endif
		Category(SRCategoryNames.Layout), DefaultValue(DefaultVisibleWeekDays),
	   Editor("DevExpress.XtraScheduler.Reporting.Design.WeekDaysEditor," + AssemblyInfo.SRAssemblySchedulerReportingExtensions, typeof(System.Drawing.Design.UITypeEditor)),
	   TypeConverter("DevExpress.XtraScheduler.Reporting.Design.WeekDaysConverter," + AssemblyInfo.SRAssemblySchedulerReportingExtensions)
		]
		public WeekDays VisibleWeekDays {
			get { return visibleWeekDays; }
			set {
				if(visibleWeekDays == value)
					return;
				visibleWeekDays = value == 0 ? WeekDays.EveryDay : value;				 
				UpdatePrintController();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekVerticalLayoutType"),
#endif
Category(SRCategoryNames.Layout), DefaultValue(DefaultVerticalLayoutType)]
		public ControlContentLayoutType VerticalLayoutType { get { return LayoutOptionsVertical.LayoutType; } set { LayoutOptionsVertical.LayoutType = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekCanShrink"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool CanShrink { get { return base.CanShrink; } set { base.CanShrink = value; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("HorizontalWeekCanGrow"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool CanGrow { get { return base.CanGrow; } set { base.CanGrow = value; } }
#endregion
		#region Internal Properties
		protected internal override bool ActualCompressWeekend {
			get {
				bool isSaturdayPresent = (VisibleWeekDays & WeekDays.Saturday) != 0;
				bool isSundayPresent = (VisibleWeekDays & WeekDays.Sunday) != 0;
				return isSaturdayPresent && isSundayPresent && CompressWeekend;
			}
		}
		internal new HorizontalWeekPrintController PrintController { get { return (HorizontalWeekPrintController)base.PrintController; } }
		#endregion
		protected internal override WeekDays CalculatePrintControllerVisibleWeekDays() {
			return ActualCompressWeekend ? VisibleWeekDays & ~WeekDays.Sunday : VisibleWeekDays;
		}		
		protected internal override TimeInterval ValidatePrintTimeInterval(TimeInterval interval) {
			if (ActualCompressWeekend && interval.End.DayOfWeek == DayOfWeek.Sunday)
				interval = new TimeInterval(interval.Start, interval.End.AddDays(1));
			return interval;
		}	
		protected internal override int GetActualVisibleIntervalColumnCount() {
			int baseColumnCount = base.GetActualVisibleIntervalColumnCount();
			int visibleDaysCount = GetVisibleDaysCount();
			XtraSchedulerDebug.Assert(PrintController != null);
			if (ActualCompressWeekend)
				visibleDaysCount -= 1;
			return Math.Min(baseColumnCount, visibleDaysCount);
		}
		protected internal int GetVisibleDaysCount() {
			return DateTimeHelper.ToDayOfWeeks(VisibleWeekDays).Length;
		}		
		protected internal override void Initialize() {
			base.Initialize();
			this.compressWeekend = DefaultCompressWeekend;
			this.visibleWeekDays = DefaultVisibleWeekDays;
		}					   
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new HorizontalWeekPainter();
		}		
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new HorizontalWeekPrintController(View, View);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptions() {
			return new ReportMonthViewAppointmentDisplayOptions();
		}
		protected internal override CellsLayoutStrategyBase CreateLayoutStrategy() {
			return new HorizontalWeekLayoutStrategy(this);
		}
		protected internal override void PrepareControlLayout() {
			base.PrepareControlLayout();
			AppointmentsLayoutResult.Clear();
			CellContainers.Clear();
		}
	}
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "fullweekcells.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.FullWeek", "FullWeek"),
	Description("A time cell control for the weekly style report.")
	]
	public class FullWeek : WeekCellsControlBase {
		#region Properties
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FullWeekAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportWeekViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (ReportWeekViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FullWeekHorizontalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new HorizontalResourceHeaders HorizontalHeaders {
			get {
				return base.HorizontalHeaders as HorizontalResourceHeaders;
			}
			set {
				base.HorizontalHeaders = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("FullWeekVerticalHeaders"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new VerticalResourceHeaders VerticalHeaders {
			get {
				return base.VerticalHeaders as VerticalResourceHeaders;
			}
			set {
				base.VerticalHeaders = value;
			}
		}
		protected internal override bool ActualCompressWeekend { get { return true; } }
		#endregion
		protected internal override WeekDays CalculatePrintControllerVisibleWeekDays() {
			return WeekDays.EveryDay;
		}
		protected internal override int GetActualVisibleIntervalColumnCount() {
			int baseColumnCount = base.GetActualVisibleIntervalColumnCount();		   
			return Math.Min(baseColumnCount, 2);
		}
		protected internal virtual FullWeekLayoutCalculator CreateCellsCalculator() {
			return new FullWeekLayoutCalculator(this);
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new FullWeekPainter();
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new FullWeekPrintController(View, View);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptions() {
			return new ReportWeekViewAppointmentDisplayOptions();
		}
		protected internal override CellsLayoutStrategyBase CreateLayoutStrategy() {
			return new FullWeekLayoutStrategy(this);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
	}
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	public enum WeekControlDataType { Weeks, Months };
	public abstract class WeekControlPainterBase : TimeCellsControlPainter {
		SchedulerHeaderPainter cellHeaderPainter;
		protected WeekControlPainterBase()
			: base() {
		}
		public new SingleWeekPainter CellPainter { get { return (SingleWeekPainter)base.CellPainter; } }
		public SchedulerHeaderPainter CellHeaderPainter { get { return cellHeaderPainter; } set { cellHeaderPainter = value; } }
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			return new AppointmentPainter();
		}
		protected internal override void PrintCells(GraphicsCache cache, SchedulerViewCellContainerCollection cellContainers, ISupportCustomDraw customDrawProvider) {
			int count = cellContainers.Count;
			for (int i = 0; i < count; i++) {
				SingleWeekViewInfo weekInfo = (SingleWeekViewInfo)cellContainers[i];
				CellPainter.Draw(cache, weekInfo, customDrawProvider);
			}
		}
	}
	public class HorizontalWeekPainter : WeekControlPainterBase {
		protected internal override ViewInfoPainterBase CreateCellPainter() {
			CellHeaderPainter = new SchedulerHeaderFlatPrintPainter();
			return new SingleWeekPainter(new HorizontalSingleWeekHeaderPainter(CellHeaderPainter));
		}
	}
	public class FullWeekPainter : WeekControlPainterBase {
		protected internal override ViewInfoPainterBase CreateCellPainter() {
			CellHeaderPainter = new SchedulerHeaderFlatPrintPainter();
			return new SingleWeekPainter(CellHeaderPainter);
		}
	}
	public class Week {
		DateTime[] weekDays = new DateTime[0];
		public Week() {
		}
		public Week(DateTime[] weekDates) {
			this.weekDays = weekDates;
		}
		public DateTime[] WeekDays { get { return weekDays; } set { weekDays = value; } }
		public TimeInterval Interval {
			get {
				int count = WeekDays.Length;
				if (count == 0)
					return TimeInterval.Empty;
				else
					return new TimeInterval(WeekDays[0], WeekDays[count - 1].AddDays(1));
			}
		}
	}
	public class WeekCollection : List<Week> {
		protected internal virtual TimeIntervalCollection GetWeekIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			for (int i = 0; i < Count; i++)
				result.Add(this[i].Interval);
			return result;
		}
	}
}
