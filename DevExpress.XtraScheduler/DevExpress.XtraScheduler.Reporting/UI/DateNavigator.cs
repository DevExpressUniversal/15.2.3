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
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraScheduler.Reporting {
	[DXToolboxItem(true),
   ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
   ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "calendar.bmp"),
   DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraScheduler.Reporting.CalendarControl", "CalendarControl"),
	Description("A control used to print a monthly calendar for a certain time interval.")
	]
	public class CalendarControl : DataDependentControlBase {
		TimeInterval printTimeInterval;
		public CalendarControl() {
		}
		public CalendarControl(ReportViewBase view)
			: base(view) {
		}
		#region Properties
		protected override int DefaultHeight { get { return 150; } }
		protected override int DefaultWidth { get { return 150; } }
		protected new CalendarPrintInfo PrintInfo { get { return (CalendarPrintInfo)base.PrintInfo; } }
		[Browsable(false)]
		public TimeInterval PrintTimeInterval { get { return printTimeInterval; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("CalendarControlTimeCells"),
#endif
Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public new TimeCellsControlBase TimeCells {
			get { return base.TimeCells as TimeCellsControlBase; }
			set { base.TimeCells = value; }
		}
		protected override DevExpress.XtraPrinting.BorderSide DefaultBorders { get { return DevExpress.XtraPrinting.BorderSide.None; } }
		#endregion
		#region Events
		#region CustomDrawDayNumberCell
		static readonly object CustomDrawDayNumberCellEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("CalendarControlCustomDrawDayNumberCell"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawDayNumberCellEventHandler CustomDrawDayNumberCell {
			add { Events.AddHandler(CustomDrawDayNumberCellEvent, value); }
			remove { Events.RemoveHandler(CustomDrawDayNumberCellEvent, value); }
		}
		protected internal void RaiseCustomDrawDayNumberCell(CustomDrawDayNumberCellEventArgs e) {
			CustomDrawDayNumberCellEventHandler handler = (CustomDrawDayNumberCellEventHandler)this.Events[CustomDrawDayNumberCellEvent];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#endregion
		protected override void BeforeCalculatePrintInfo() {
			this.printTimeInterval = GetPrintTimeInterval();
		}
		protected override bool ShouldRecalculatePrintInfo() {
			if (!TimeInterval.Equals(PrintTimeInterval, GetPrintTimeInterval()))
				return true;
			return base.ShouldRecalculatePrintInfo();
		}
		protected internal override ViewInfoPainterBase CreatePainter() {
			return new ViewInfoPainterBase();
		}
		protected TimeInterval GetPrintTimeInterval() {
			ISupportPrintableTimeInterval support = TimeCells as ISupportPrintableTimeInterval;
			return support != null ? support.GetPrintTimeInterval(PrintContentMode.CurrentColumn) : TimeInterval.Empty;
		}
		private SchedulerPrintAdapter GetSchedulerAdapter() {
			if (View != null && View.SchedulerAdapter != null)
				return View.SchedulerAdapter;
			return SchedulerReport.ActualSchedulerAdapter;
		}
		protected override void ApplyPrintColorSchema() {
		}
		protected internal override ControlPrintControllerBase CreatePrintController() {
			return new DependentPrintController();
		}
		protected override ControlPrintInfo CalculatePrintInfo(ControlLayoutInfo info) {
			return CalculatePrintInfoCore(info);
		}
		protected virtual CalendarPrintInfo CalculatePrintInfoCore(ControlLayoutInfo info) {
			CalendarPrintInfo printInfo = new CalendarPrintInfo(this, GetSchedulerAdapter(), TimeCells, info.ControlPrintBounds);
			if (!IsDesignMode)
				printInfo.BoldAppointmentDates();
			return printInfo;
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			return null;
		}
	}
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region CalendarPrintInfo
	public class CalendarPrintInfo : ControlPrintInfo {
		#region Fields
		SchedulerPrintAdapter schedulerAdapter;
		ISupportPrintableTimeInterval supportTimeInterval;
		Rectangle bounds;
		ReportDateNavigator dateNavigator;
		#endregion
		public CalendarPrintInfo(CalendarControl control, SchedulerPrintAdapter schedulerAdapter, ISupportPrintableTimeInterval supportTimeInterval, Rectangle bounds)
			: base(control) {
			Guard.ArgumentNotNull(schedulerAdapter, "schedulerAdapter");
			this.schedulerAdapter = schedulerAdapter;
			this.supportTimeInterval = supportTimeInterval;
			this.bounds = bounds;
		}
		#region Properties
		public CalendarCollection Calendars { get { return DateNavigator.Calendars; } }
		protected internal ReportDateNavigator DateNavigator {
			get {
				if (dateNavigator == null)
					dateNavigator = CreateDateNavigator();
				return dateNavigator;
			}
		}
		protected internal new CalendarControl Control { get { return (CalendarControl)base.Control; } }
		#endregion
		protected internal override ControlPrintInfo CloneCore() {
			return new CalendarPrintInfo(Control, schedulerAdapter, supportTimeInterval, bounds);
		}
		protected internal ReportDateNavigator CreateDateNavigator() {
			ReportDateNavigator dateNavigator = new ReportDateNavigator(schedulerAdapter, supportTimeInterval);
			dateNavigator.CalendarView = XtraEditors.Repository.CalendarView.Classic;
			dateNavigator.ClientSize = bounds.Size;
			dateNavigator.ShowHeader = false;
			dateNavigator.ShowFooter = false;
			dateNavigator.CellPadding = new System.Windows.Forms.Padding(1);
			dateNavigator.AutoSize = false;
			dateNavigator.ClientSize = bounds.Size;
			dateNavigator.HighlightTodayCell = DefaultBoolean.False;
			dateNavigator.HighlightHolidays = false;
			dateNavigator.InactiveDaysVisibility = CalendarInactiveDaysVisibility.Hidden;
			dateNavigator.ShowMonthHeaders = true;
			dateNavigator.WeekDayAbbreviationLength = 1;
			dateNavigator.CreateControl();
			return dateNavigator;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (dateNavigator != null) {
						dateNavigator.Dispose();
						dateNavigator = null;
					}
					schedulerAdapter = null;
					supportTimeInterval = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal CalendarControlPainter CreateCalendarControlPainter() {
			return new CalendarControlPainter(DateNavigator);
		}
		protected internal void BoldAppointmentDates() {
			DateNavigator.BoldAppointmentDates();
		}
		public override void Print(GraphicsCache cache) {
			CalendarControlPainter painter = CreateCalendarControlPainter();
			int count = Calendars.Count;
			 for (int i = 0; i < count; i++) {
				 CalendarObjectViewInfo calendar = DateNavigator.Calendars[i];
				 CalendarControlInfoArgs calendarInfo = new CalendarControlInfoArgs(dateNavigator.InternalViewInfo, cache, calendar.CalendarInfo.ClientRect);
				 painter.DrawObject(new CalendarControlObjectInfoArgs(calendarInfo, calendar, cache));
			 }
		}
	}
	#endregion
	#region ReportDateNavigator
	public class ReportDateNavigator : DateNavigatorBase, IReportDateNavigatorControllerOwner {
		public ReportDateNavigator(SchedulerPrintAdapter schedulerAdapter, ISupportPrintableTimeInterval supportTimeInterval) {
			if (schedulerAdapter == null)
				Exceptions.ThrowArgumentNullException("schedulerAdapter");
			ShowWeekNumbers = false;
			ShowTodayButton = false;
			BackColor = Color.Transparent;
			Controller.SchedulerAdapter = schedulerAdapter;
			Controller.SupportTimeInterval = supportTimeInterval;
			if (supportTimeInterval != null)
				DateTime = supportTimeInterval.GetPrintTimeInterval(PrintContentMode.CurrentColumn).Start;
			SpecialDateProvider = CreateReportDateNavigatorSpecialDateProvider();
			CalendarView = XtraEditors.Repository.CalendarView.Classic;
			CalendarAppearance.DayCellSpecialSelected.BackColor = Color.Transparent;
			CalendarAppearance.DayCellSelected.BackColor = Color.Transparent;
		}
		protected internal CalendarCollection Calendars { get { return base.ViewInfo.Calendars; } }
		protected internal CalendarViewInfoBase InternalViewInfo { get { return base.ViewInfo; } }
		protected internal new CalendarControlController Controller { get { return (CalendarControlController)base.Controller; } }
		protected internal override DateNavigatorControllerBase CreateController() {
			return new CalendarControlController(this);
		}
		protected ICalendarSpecialDateProvider CreateReportDateNavigatorSpecialDateProvider() {
			return new ReportDateNavigatorSpecialDateProvider(this);
		}
		protected internal void BoldAppointmentDates() {
			Controller.UpdateAppointmentDatesMap();
			LayoutChanged();
		}
		#region IReportDateNavigatorControllerOwner Members
		DateTime IReportDateNavigatorControllerOwner.StartDate { get { return this.GetStartDate(); } }
		DateTime IReportDateNavigatorControllerOwner.EndDate { get { return this.GetEndDate(); } }
		#endregion
	}
	#endregion
	public interface IReportDateNavigatorControllerOwner {
		DateTime StartDate { get; }
		DateTime EndDate { get; }
	}
	public class CalendarControlPainter : DateNavigatorPrinterBase {
		CalendarControl calendarControl;
		public CalendarControlPainter(DateNavigatorBase control) : base(control) { }
		public CalendarControlPainter(CalendarControl calendarControl, DateNavigatorBase innerControl)
			: base(innerControl) {
			this.calendarControl = calendarControl;
		}
		public CalendarControl CalendarControl { get { return calendarControl; } }
	}
	#region ReportDateNavigatorSpecialDateProvider
	public class ReportDateNavigatorSpecialDateProvider : ICalendarSpecialDateProvider {
	   ReportDateNavigator dateNavigator;
		public ReportDateNavigatorSpecialDateProvider(ReportDateNavigator calendar) {
			this.dateNavigator = calendar;
		}
		public bool IsSpecialDate(DateTime date, DateEditCalendarViewType view) {
			if (this.dateNavigator.Controller != null)
				return this.dateNavigator.Controller.AppointmentDatesMap.ContainsKey(date);
			return false;
		}
	}
	#endregion
	#region CalendarControlController
	public class CalendarControlController : DateNavigatorControllerBase {
		SchedulerPrintAdapter schedulerAdapter;
		ISupportPrintableTimeInterval supportTimeInterval;
		IReportDateNavigatorControllerOwner owner;
		public CalendarControlController(IReportDateNavigatorControllerOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public SchedulerPrintAdapter SchedulerAdapter { get { return schedulerAdapter; } set { schedulerAdapter = value; } }
		protected internal ISupportPrintableTimeInterval SupportTimeInterval { get { return supportTimeInterval; } set { supportTimeInterval = value; } }
		protected override TimeZoneHelper TimeZoneHelper { get { return SchedulerAdapter.TimeZoneHelper; } }
		internal IReportDateNavigatorControllerOwner Owner { get { return owner; } set { owner = value; } }
		protected override bool CanUpdateAppointmentDatesMap() {
			return true;
		}
		protected override bool CanFillAppointmentDatesMap() {
			return true;
		}
		protected override bool CanObtainVisibleAppointments() {
			return schedulerAdapter != null && supportTimeInterval != null;
		}
		protected AppointmentBaseCollection GetVisibleAppointments(TimeInterval interval, ResourceBaseCollection resources) {
			return SchedulerAdapter.GetAppointments(interval, resources);
		}
		protected override TimeInterval GetVisibleInterval() {
			return new TimeInterval(Owner.StartDate, Owner.EndDate);
		}
		protected override ResourceBaseCollection GetVisibleResources() {
			return SchedulerAdapter.GetResources();
		}
		protected override void PopulateAppointmentDatesMap(TimeInterval filterInterval, ResourceBaseCollection visibleResources) {
			AppointmentBaseCollection appointments = GetVisibleAppointments(filterInterval, visibleResources);
			TimeIntervalCollectionEx intervals = CreateTotalAppointmentsIntervalsAtInterval(appointments, filterInterval);
			DayIntervalCollection days = new DayIntervalCollection();
			days.AddRange(intervals);
			AddDaysToAppointmentDatesMap(days);
		}
	}
	#endregion
}
