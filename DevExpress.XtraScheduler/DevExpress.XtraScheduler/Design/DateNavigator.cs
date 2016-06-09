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
using Microsoft.Win32;
using System.Security;
using DevExpress.Utils;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using System.Collections.Generic;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraEditors.Calendar;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
namespace DevExpress.XtraScheduler {
	public abstract class DateNavigatorBase : DateControl {
		DateNavigatorControllerBase controller;
		protected DateNavigatorBase() {
		}
		protected override void Init() {
			this.controller = CreateController();
			base.Init();
		}
		protected internal DateNavigatorControllerBase Controller { get { return controller; } }
		protected internal abstract DateNavigatorControllerBase CreateController();
	}
	#region DateNavigator
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(DateNavigator), DevExpress.Utils.ControlConstants.BitmapPath + "datenavigator.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteControlDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A calendar control for navigating dates.")
	]
	[Docking(DockingBehavior.Ask)]
	public class DateNavigator : DateNavigatorBase, ISupportInitialize, IDateNavigatorControllerOwner, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		bool printNavigator;
		bool rightMouseDrop;
		DateNavigatorDayNumberCellInfo lastHotTrackCell;
		BatchUpdateHelper batchUpdateHelper;
		#endregion
		[SecuritySafeCritical]
		public DateNavigator() {
			SystemEvents.TimeChanged += new EventHandler(OnTimeChanged);
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			AllowDrop = true;
			SubscribeControllerEvents(Controller);
		}
		#region Properties
		#region DateTime
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorDateTime"),
#endif
		DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All)]
		public override DateTime DateTime {
			get {
				return base.DateTime;
			}
			set {
				base.DateTime = value;
				Controller.SyncSelection();
			}
		}
		#endregion
		protected internal new DateNavigatorControllerWin Controller { get { return (DateNavigatorControllerWin)base.Controller; } }
		protected DateNavigatorDayNumberCellInfo LastHotTrackCell { get { return lastHotTrackCell; } set { lastHotTrackCell = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorSchedulerControl"),
#endif
		Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return Controller.SchedulerControl; }
			set { Controller.SchedulerControl = value; }
		}
		[Browsable(false)]
		public override DayOfWeek FirstDayOfWeek {
			get {
				if (Controller == null)
					return base.FirstDayOfWeek;
				return Controller.FirstDayOfWeek;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorSelectionMode"),
#endif
		DefaultValue(CalendarSelectionMode.Multiple)]
		public new CalendarSelectionMode SelectionMode { get { return base.SelectionMode; } set { base.SelectionMode = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorShowWeekNumbers"),
#endif
		DefaultValue(true)]
		public new bool ShowWeekNumbers { get { return base.ShowWeekNumbers; } set { base.ShowWeekNumbers = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorHighlightHolidays"),
#endif
		DefaultValue(true)]
		public new bool HighlightHolidays { get { return base.HighlightHolidays; } set { base.HighlightHolidays = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorBoldAppointmentDates"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Scheduler)]
		public bool BoldAppointmentDates {
			get { return Controller.BoldAppointmentDates; }
			set {
				Controller.BoldAppointmentDates = value;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorSelectionBehavior"),
#endif
		DefaultValue(CalendarSelectionBehavior.OutlookStyle)]
		public new CalendarSelectionBehavior SelectionBehavior {
			get { return base.SelectionBehavior; }
			set { base.SelectionBehavior = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorInactiveDaysVisibility"),
#endif
		DefaultValue(CalendarInactiveDaysVisibility.FirstLast)]
		public new CalendarInactiveDaysVisibility InactiveDaysVisibility {
			get { return base.InactiveDaysVisibility; }
			set { base.InactiveDaysVisibility = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorAutoSize"),
#endif
		DefaultValue(false)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorCalendarView"),
#endif
		DefaultValue(CalendarView.Classic)]
		public override CalendarView CalendarView {
			get { return base.CalendarView; }
			set { base.CalendarView = value; }
		}
		protected override bool RecalcStartDate { get { return false; } }
		protected internal bool PrintNavigator {
			get { return printNavigator; }
			set {
				if (printNavigator == value)
					return;
				printNavigator = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorAllowDrop"),
#endif
		DefaultValue(true)]
		public override bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorShowTodayButton"),
#endif
		DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
		public override bool ShowTodayButton {
			get { return base.ShowTodayButton; }
			set { base.ShowTodayButton = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DateNavigatorShowClearButton"),
#endif
		DefaultValue(false)]
		public override bool ShowClearButton {
			get { return base.ShowClearButton; }
			set { base.ShowClearButton = value; }
		}
		[
		DefaultValue(true)]
		public new bool ShowMonthHeaders {
			get { return base.ShowMonthHeaders; }
			set { base.ShowMonthHeaders = value; }
		}
		[ Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DateTime MinValue {
			get {
				if (SchedulerControl == null || SchedulerControl.LimitInterval == null)
					return base.MinValue;
				return SchedulerControl.LimitInterval.Start;
			}
			set { base.MinValue = value; }
		}
		[ Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DateTime MaxValue {
			get {
				if (SchedulerControl == null || SchedulerControl.LimitInterval == null)
					return base.MaxValue;
				return SchedulerControl.LimitInterval.End;
			}
			set { base.MaxValue = value; }
		}
		protected override bool AllowSyncSelectionWithSelectedRanges {
			get {
				return true;
			}
		}
		protected override CalendarView ActualCalendarView {
			get {
				return CalendarView.Classic;
			}
		}
		internal CalendarCollection Calendars { get { return ViewInfo.Calendars; } }
		internal CalendarViewInfoBase InternalViewInfo { get { return ViewInfo; } }
		#endregion
		#region IDisposable implementation
		[SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					Controller.Changed -= new EventHandler(OnControllerChanged);
					SystemEvents.TimeChanged -= new EventHandler(OnTimeChanged);
					UnsubscribeControlelrEvents(Controller);
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		public override CalendarHitInfo GetHitInfo(MouseEventArgs e) {
			CalendarHitInfo result = base.GetHitInfo(e);
			DateTime validateDate = ValidateStart(result.HitDate);
			return result;
		}
		public void BeginInit() {
			Controller.BeginUpdate();
		}
		public void EndInit() {
			Controller.EndUpdate();
		}
		public override void Refresh() {
			LayoutChanged();
			base.Refresh();
		}
		protected override void Init() {
			ShowWeekNumbers = true;
			base.Init();
			Controller.Changed += new EventHandler(OnControllerChanged);
			AutoSize = false;
			SelectionMode = CalendarSelectionMode.Multiple;
			ShowMonthHeaders = true;
			ShowClearButton = false;
			SelectionBehavior = CalendarSelectionBehavior.OutlookStyle;
			SyncSelectionWithEditValue = true;
			UpdateSelectionWhenNavigating = false;
			InactiveDaysVisibility = CalendarInactiveDaysVisibility.FirstLast;
			CalendarView = CalendarView.Classic;
			SpecialDateProvider = CreateDateNavigatorSpecialDateProvider();
			CellStyleProvider = CreateDateNavigatorCellAppearanceProvider();
		}
		protected virtual void SubscribeControllerEvents(DateNavigatorControllerWin controller) {
			controller.DateNavigatorQueryActiveViewType += OnControllerDateNavigatorQueryActiveViewType;
		}
		protected virtual void UnsubscribeControlelrEvents(DateNavigatorControllerWin controller) {
			controller.DateNavigatorQueryActiveViewType -= OnControllerDateNavigatorQueryActiveViewType;
		}
		protected ICalendarSpecialDateProvider CreateDateNavigatorSpecialDateProvider() {
			CalendarAppearance.DayCellSpecial.FontStyleDelta = FontStyle.Bold;
			return new DateNavigatorSpecialDateProvider(this);
		}
		protected ICalendarCellStyleProvider CreateDateNavigatorCellAppearanceProvider() {
			return new DateNavigatorCellAppearanceProvider(this);
		}
		protected internal override DateNavigatorControllerBase CreateController() {
			return new DateNavigatorControllerWin(this);
		}
		protected override CalendarControlHandlerBase CreateHandler() {
			return new DateNavigatorHandler(this);
		}
		protected override CalendarSelectionManager CreateSelectionManager() {
			return new DateNavigatorSelectionManager(this);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new DateNavigatorViewInfo(this);
		}
		protected override DateTime ChangeSelectedDay(DateTime value) {
			DateTime validateDate = ValidateStart(value);
			DateTime newDate = base.ChangeSelectedDay(validateDate);
			return newDate;
		}
		protected override DateTime CalculateFirstMonthDate(DateTime dateTime) {
			if (SchedulerControl != null)
				dateTime = DateTimeHelper.Max(DateTimeHelper.Min(dateTime, SchedulerControl.LimitInterval.End), SchedulerControl.LimitInterval.Start);
			return base.CalculateFirstMonthDate(dateTime);
		}
		protected override void OnDateTimeCommit(object value) {
			if (value == null)
				value = DateTime.MinValue;
			value = ValidateStart((DateTime)value);
			base.OnDateTimeCommit(value);
		}
		protected override void LayoutChanged() {
			int prevCalendarsCount = GetCalendarsCount();
			base.LayoutChanged();
			int currCalendarsCount = GetCalendarsCount();
			if (prevCalendarsCount != currCalendarsCount)
				Controller.UpdateAppointmentDatesMap();
		}
		protected internal virtual DateTime AdjustSelectionStart(DateTime start, DateTime end) {
			if (SchedulerControl == null)
				return start;
			TimeSpan weekSpan = TimeSpan.FromDays(6);
			DayOfWeek actualFirstDayOfWeek = Controller.FirstDayOfWeek;
			InnerSchedulerControl innerControl = Controller.InnerControl;
			if (innerControl != null && innerControl.MonthView.Enabled)
				actualFirstDayOfWeek = innerControl.MonthView.ActualFirstDayOfWeek;
			if (end >= start) {
				if (end - start < weekSpan)
					return start;
				else {
					return DateTimeHelper.GetStartOfWeekUI(start, actualFirstDayOfWeek);
				}
			} else {
				if (start - end < weekSpan)
					return start;
				else
					return DateTimeHelper.GetStartOfWeekUI(start, actualFirstDayOfWeek) + weekSpan;
			}
		}
		protected internal virtual DateTime AdjustSelectionEnd(DateTime start, DateTime end) {
			if (SchedulerControl == null)
				return end;
			DayOfWeek actualFirstDayOfWeek = Controller.FirstDayOfWeek;
			InnerSchedulerControl innerControl = Controller.InnerControl;
			if (innerControl != null && innerControl.MonthView.Enabled)
				actualFirstDayOfWeek = innerControl.MonthView.ActualFirstDayOfWeek;
			TimeSpan weekSpan = TimeSpan.FromDays(7);
			if (end >= start) {
				if (end - start < weekSpan)
					return end;
				DateTime adjustedEnd = DateTimeHelper.GetStartOfWeekUI(end, actualFirstDayOfWeek);
				if (end != adjustedEnd)
					return adjustedEnd + weekSpan;
				return end;
			} else {
				if (start - end < weekSpan)
					return end;
				return DateTimeHelper.GetStartOfWeekUI(end, actualFirstDayOfWeek);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (SchedulerControl == null || SchedulerControl.IsDisposed) {
				base.OnKeyDown(e);
				return;
			}
			KeyboardHandler handler = SchedulerControl.InnerControl.KeyboardHandler;
			handler.Context = SchedulerControl.ActiveView.InnerView;
			handler.HandleKey(e.KeyData);
		}
		protected override void OnDragEnter(DragEventArgs drgevent) {
			base.OnDragEnter(drgevent);
			if (DesignMode)
				return;
			IDataObject dragData = drgevent.Data;
			if (SchedulerControl == null || !SchedulerDragData.GetPresent(dragData)) {
				drgevent.Effect = DragDropEffects.None;
				return;
			}
			SchedulerDragData data = (SchedulerDragData)dragData.GetData(typeof(SchedulerDragData));
			SchedulerControl.MouseHandler.OnDragEnterToExternalControl(data);
			OnDragOverCore(drgevent, data);
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			base.OnDragOver(drgevent);
			if (DesignMode)
				return;
			IDataObject dragData = drgevent.Data;
			rightMouseDrop = (drgevent.KeyState & 2) != 0;
			if (SchedulerControl == null || !SchedulerDragData.GetPresent(dragData)) {
				drgevent.Effect = DragDropEffects.None;
				return;
			}
			SchedulerDragData data = (SchedulerDragData)dragData.GetData(typeof(SchedulerDragData));
			OnDragOverCore(drgevent, data);
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
			if (DesignMode)
				return;
			HideLastHotTrackCell();
			IDataObject dragData = drgevent.Data;
			if (SchedulerControl == null || !SchedulerDragData.GetPresent(dragData)) {
				drgevent.Effect = DragDropEffects.None;
				return;
			}
			SchedulerDragData data = (SchedulerDragData)dragData.GetData(typeof(SchedulerDragData));
			DateNavigatorDayNumberCellInfo cell = GetHitCell(drgevent.X, drgevent.Y);
			drgevent.Effect = DragDropEffects.None;
			if (cell != null) {
				DateTime dropDate = cell.Date;
				AppointmentBaseCollection newAppointments = CreateNewAppointment(dropDate, data);
				bool copy = (drgevent.KeyState & (int)KeyState.CtrlKey) != 0;
				bool result = SchedulerControl.MouseHandler.OnDragDropExternalControl(data, newAppointments, new Point(drgevent.X, drgevent.Y), copy, rightMouseDrop);
				if (result)
					drgevent.Effect = copy ? DragDropEffects.Copy : DragDropEffects.Move;
			} else {
				if (SchedulerControl != null)
					SchedulerControl.MouseHandler.OnDragDropExternalControl(data, null, Point.Empty, false, false);
			}
		}
		protected override void OnDragLeave(EventArgs e) {
			base.OnDragLeave(e);
			if (DesignMode)
				return;
			HideLastHotTrackCell();
			if (SchedulerControl != null)
				SchedulerControl.MouseHandler.OnDragLeaveExternalControl();
		}
		void OnDragOverCore(DragEventArgs drgevent, SchedulerDragData data) {
			DateNavigatorDayNumberCellInfo cell = GetHitCell(drgevent.X, drgevent.Y);
			drgevent.Effect = DragDropEffects.None;
			if (cell != null) {
				RefreshHotTrackCell(drgevent);
				DateTime dropDate = cell.Date;
				AppointmentBaseCollection newAppointments = CreateNewAppointment(dropDate, data);
				bool copy = (drgevent.KeyState & (int)KeyState.CtrlKey) != 0;
				bool result = SchedulerControl.MouseHandler.OnDragOverExternalControl(data, newAppointments, copy);
				if (result)
					drgevent.Effect = copy ? DragDropEffects.Copy : DragDropEffects.Move;
			}
		}
		void OnControllerDateNavigatorQueryActiveViewType(object sender, DateNavigatorQueryActiveViewTypeEventArgs e) {
			if (SchedulerControl == null)
				return;
			e.NewViewType = SchedulerControl.RaiseDateNavigatorQueryActiveViewType(e.OldViewType, e.NewViewType, e.SelectedDays);
		}
		void OnTimeChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		void OnControllerChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		int GetCalendarsCount() {
			return ViewInfo.Calendars != null ? ViewInfo.Calendars.Count : 0;
		}
		DateTime ValidateStart(DateTime start) {
			if (SchedulerControl == null)
				return start;
			if (start >= SchedulerControl.LimitInterval.End)
				start = SchedulerControl.LimitInterval.End.AddDays(-1);
			if (start < SchedulerControl.LimitInterval.Start)
				start = SchedulerControl.LimitInterval.Start;
			return start;
		}
		DateTime ValidateEnd(DateTime end) {
			if (SchedulerControl == null)
				return end;
			if (end >= SchedulerControl.LimitInterval.End)
				end = SchedulerControl.LimitInterval.End.AddDays(-1);
			if (end < SchedulerControl.LimitInterval.Start)
				end = SchedulerControl.LimitInterval.Start;
			return end;
		}
		DateNavigatorDayNumberCellInfo GetHitCell(int x, int y) {
			Point pt = new Point(x, y);
			pt = PointToClient(pt);
			CalendarHitInfo hitInfo = GetHitInfo(new MouseEventArgs(MouseButtons.None, 1, pt.X, pt.Y, 0));
			return hitInfo.HitTest == CalendarHitInfoType.MonthNumber ?
				(DateNavigatorDayNumberCellInfo)hitInfo.HitObject : null;
		}
		void RefreshHotTrackCell(DragEventArgs drgevent) {
			Region invalidRegion = new Region();
			if (LastHotTrackCell != null) {
				invalidRegion.Union(LastHotTrackCell.Bounds);
				LastHotTrackCell.HotTrack = false;
			}
			DateNavigatorDayNumberCellInfo cell = GetHitCell(drgevent.X, drgevent.Y);
			if (cell != null) {
				cell.HotTrack = true;
				invalidRegion.Union(cell.Bounds);
			} else
				drgevent.Effect = DragDropEffects.None;
			LastHotTrackCell = cell;
			Invalidate(invalidRegion);
			invalidRegion.Dispose();
			Update();
		}
		void HideLastHotTrackCell() {
			if (LastHotTrackCell != null) {
				LastHotTrackCell.HotTrack = false;
				Invalidate(LastHotTrackCell.Bounds);
				LastHotTrackCell = null;
			}
		}
		AppointmentBaseCollection CreateNewAppointment(DateTime dropDate, SchedulerDragData data) {
			AppointmentBaseCollection newAppointments = new AppointmentBaseCollection();
			int count = data.Appointments.Count;
			TimeSpan delta = dropDate - data.PrimaryAppointment.Start.Date;
			for (int i = 0; i < count; i++) {
				Appointment appointment = ((IInternalAppointment)data.Appointments[i]).CopyCore();
				appointment.Start += delta;
				newAppointments.Add(appointment);
			}
			return newAppointments;
		}
		#region IDateNavigatorControllerOwner Members
		DateTime IDateNavigatorControllerOwner.EndDate {
			get { return GetEndDate(); }
		}
		DateTime IDateNavigatorControllerOwner.GetFirstDayOfMonth(DateTime date) {
			return base.GetFirstDayOfMonth(date);
		}
		DayIntervalCollection IDateNavigatorControllerOwner.GetSelection() {
			DayIntervalCollection result = new DayIntervalCollection();
			int count = SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				result.Add(new TimeInterval(this.SelectedRanges[i].StartDate, this.SelectedRanges[i].EndDate));
			return result;
		}
		void IDateNavigatorControllerOwner.SetSelection(DayIntervalCollection days) {
			this.SelectedRanges.BeginUpdate();
			try {
				this.SelectedRanges.Clear();
				int count = days.Count;
				for (int i = 0; i < count; i++)
					this.SelectedRanges.Add(new DateRange(days[i].Start, days[i].End));
			} finally {
				this.SelectedRanges.EndUpdate();
			}
		}
		DateTime IDateNavigatorControllerOwner.StartDate {
			get {
				return StartDate;
			}
			set {
				StartDate = value;
			}
		}
		#endregion
		#region IBatchUpdateable implementation
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return Controller.IsUpdateLocked; } }
		public void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
			Controller.BeginUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
			Controller.CancelUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
			Controller.EndUpdate();
		}
		#endregion
		#region IBatchUpdateHandler
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
		}
		#endregion
		protected override bool IsInputKey(Keys key) {
			if (key == Keys.Enter)
				return false;
			return base.IsInputKey(key);
		}
	}
	#endregion
	#region DateNavigatorSpecialDateProvider
	public class DateNavigatorSpecialDateProvider : ICalendarSpecialDateProvider {
		DateNavigator dateNavigator;
		public DateNavigatorSpecialDateProvider(DateNavigator calendar) {
			this.dateNavigator = calendar;
		}
		public bool IsSpecialDate(DateTime date, DateEditCalendarViewType view) {
			if (this.dateNavigator.SchedulerControl != null && this.dateNavigator.BoldAppointmentDates)
				return this.dateNavigator.Controller.AppointmentDatesMap.ContainsKey(date);
			return false;
		}
	}
	#endregion
	#region DateNavigatorCellAppearanceProvider
	public class DateNavigatorCellAppearanceProvider : ICalendarCellStyleProvider {
		DateNavigator dateNavigator;
		public DateNavigatorCellAppearanceProvider(DateNavigator calendar) {
			this.dateNavigator = calendar;
		}
		public void UpdateAppearance(CalendarCellStyle cell) {
			if (cell.Holiday)
				cell.Appearance.ForeColor = cell.PaintStyle.HolidayCellAppearance.ForeColor;
		}
	}
	#endregion
	#region DateNavigatorHandler
	public class DateNavigatorHandler : CalendarControlHandler {
		DateNavigator dateNavigator;
		public DateNavigatorHandler(CalendarControlBase calendar)
			: base(calendar) {
			dateNavigator = Calendar as DateNavigator;
		}
		protected override void OnTodayButtonClick(object sender, EventArgs e) {
			if (dateNavigator.SchedulerControl == null)
				base.OnTodayButtonClick(sender, e);
			else
				dateNavigator.SchedulerControl.GoToToday();
		}
		protected override void ChangeSelectedDate(int yearOffset, int monthOffset, int daysOffset, bool updateLayout) {
			if (dateNavigator.SchedulerControl == null)
				base.ChangeSelectedDate(yearOffset, monthOffset, daysOffset, updateLayout);
			else
				dateNavigator.Controller.ShiftVisibleIntervals(yearOffset, monthOffset, daysOffset);
		}
		protected override bool CanProcessCellClick(CalendarCellViewInfo cell) {
			return cell != null;
		}
	}
	#endregion
	#region DateNavigatorSelectionManager
	public class DateNavigatorSelectionManager : CalendarSelectionManager {
		DateNavigatorController controller;
		DateNavigator dateNavigator;
		public DateNavigatorSelectionManager(CalendarControlBase calendar)
			: base(calendar) {
			this.dateNavigator = Calendar as DateNavigator;
			this.controller = dateNavigator.Controller;
		}
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			CalendarHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if (hitInfo.HitDate != null && hitInfo.HitDate.Equals(this.dateNavigator.EditValue))
				this.controller.SyncSelection();
			if (!(hitInfo.IsInCell || hitInfo.IsInFooter || hitInfo.IsInHeader))
				this.controller.SyncSelection();
		}
		protected override DateTime AdjustSelectionStart(DateTime start, DateTime end) {
			return this.dateNavigator.AdjustSelectionStart(start, end);
		}
		protected override DateTime AdjustSelectionEnd(DateTime start, DateTime end) {
			return this.dateNavigator.AdjustSelectionEnd(start, end);
		}
	}
	#endregion
	#region DateNavigatorViewInfo
	public class DateNavigatorViewInfo : CalendarViewInfo {
		public DateNavigatorViewInfo(CalendarControlBase owner) : base(owner) { }
		public new DateNavigator Calendar { get { return (DateNavigator)base.Calendar; } }
		protected override CalendarObjectViewInfo CreateCalendar(int index) {
			DateNavigatorInfoArgs viewInfo = null;
			if (Calendar.PrintNavigator)
				viewInfo = new DateNavigatorPrintInfoArgs(Calendar);
			else
				viewInfo = new DateNavigatorInfoArgs(Calendar);
			viewInfo.ShowHeader = ShowCalendarHeader(index);
			viewInfo.View = Calendar.View;
			return viewInfo;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	#region DateNavigatorDayNumberCellInfo
	public class DateNavigatorDayNumberCellInfo : CalendarCellViewInfo {
		bool hotTrack;
		public DateNavigatorDayNumberCellInfo(DateTime date, CalendarObjectViewInfo viewInfo)
			: base(date, viewInfo) {
		}
		[Obsolete("Use the Appearance.FontStyleDelta property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool Bold {
			get { return Appearance.FontStyleDelta == FontStyle.Bold; }
			set { Appearance.FontStyleDelta = value ? FontStyle.Bold : FontStyle.Regular; }
		}
		public bool HotTrack { get { return hotTrack; } set { hotTrack = value; } }
	}
	#endregion
	#region DateNavigatorBaseInfoArgs (abstract class)
	public abstract class DateNavigatorBaseInfoArgs : CalendarObjectViewInfo {
		protected DateNavigatorBaseInfoArgs(CalendarControl calendar)
			: base(calendar) {
		}
		public new DateNavigatorBase Calendar { get { return (DateNavigatorBase)base.Calendar; } }
		protected DateNavigatorDayNumberCellInfo CreateBoldedCell(DateTime date) {
			DateNavigatorDayNumberCellInfo cell = new DateNavigatorDayNumberCellInfo(date, this);
			if (Calendar.Controller.AppointmentDatesMap.ContainsKey(cell.Date))
				cell.Appearance.FontStyleDelta = FontStyle.Bold;
			return cell;
		}
		protected DateTime CalculateFirstVisibleDate(DateTime editDate) {
			DateTime firstMonthDate = new DateTime(editDate.Year, editDate.Month, 1);
			TimeSpan delta = TimeSpan.FromDays(-GetFirstDayOffset(firstMonthDate));
			if (firstMonthDate.Ticks + delta.Ticks < 0)
				return DateTime.MinValue;
			else {
				try {
					return firstMonthDate + delta;
				} catch (ArgumentOutOfRangeException) {
					return MinValue;
				}
			}
		}
		protected override CalendarCellViewInfo CreateDayCell(DateTime date) {
			return new DateNavigatorDayNumberCellInfo(date, this);
		}
		new protected internal virtual DateEditCalendarViewType View {
			get {
				return base.View;
			}
			set {
				base.View = value;
			}
		}
	}
	#endregion
	#region DateNavigatorBasePrintInfoArgs
	public class DateNavigatorBasePrintInfoArgs : DateNavigatorBaseInfoArgs {
		public DateNavigatorBasePrintInfoArgs(DateNavigatorBase control)
			: base(control) {
		}
		protected override bool IsDateActive(CalendarCellViewInfo cell) {
			return true;
		}
		protected override bool IsDateSelected(CalendarCellViewInfo cell) {
			return false;
		}
		protected override bool IsHolidayDate(CalendarCellViewInfo cell) {
			return false;
		}
		protected override bool IsToday(CalendarCellViewInfo cell) {
			return false;
		}
		protected override CalendarCellViewInfo CreateDayCell(DateTime date) {
			return CreateBoldedCell(date);
		}
		public override DateTime GetFirstVisibleDate(DateTime editDate) {
			return CalculateFirstVisibleDate(editDate);
		}
		protected override bool CanAddDate(DateTime date) {
			if (!base.CanAddDate(date))
				return false;
			return (date.Month == CurrentDate.Month);
		}
	}
	#endregion
	#region DateNavigatorInfoArgs
	public class DateNavigatorInfoArgs : DateNavigatorBaseInfoArgs {
		public DateNavigatorInfoArgs(DateNavigator control)
			: base(control) {
		}
		public new DateNavigator Calendar { get { return (DateNavigator)base.Calendar; } }
		protected override bool IsDateActive(CalendarCellViewInfo cell) {
			return cell.Date.Month == CurrentDate.Month;
		}
		protected override bool IsHolidayDate(CalendarCellViewInfo cell) {
			SchedulerControl control = Calendar.SchedulerControl;
			if (control != null && control.WorkDays != null) {
				return !control.WorkDays.IsWorkDay(cell.Date);
			} else
				return base.IsHolidayDate(cell);
		}
		new public virtual bool ShowHeader {
			get {
				return base.ShowHeader;
			}
			protected internal set {
				base.ShowHeader = value;
			}
		}
	}
	#endregion
	#region DateNavigatorBoldedInfoArgs
	public class DateNavigatorBoldedInfoArgs : DateNavigatorInfoArgs {
		public DateNavigatorBoldedInfoArgs(DateNavigator control)
			: base(control) {
		}
		protected override CalendarCellViewInfo CreateDayCell(DateTime date) {
			return CreateBoldedCell(date);
		}
	}
	#endregion
	#region DateNavigatorPrintInfoArgs
	public class DateNavigatorPrintInfoArgs : DateNavigatorBoldedInfoArgs {
		public DateNavigatorPrintInfoArgs(DateNavigator control)
			: base(control) {
		}
		protected override bool IsDateActive(CalendarCellViewInfo cell) {
			return true;
		}
		protected override bool IsDateSelected(CalendarCellViewInfo cell) {
			return false;
		}
		protected override bool IsHolidayDate(CalendarCellViewInfo cell) {
			return false;
		}
		protected override bool IsToday(CalendarCellViewInfo cell) {
			return false;
		}
		public override DateTime GetFirstVisibleDate(DateTime editDate) {
			return CalculateFirstVisibleDate(editDate);
		}
		protected override bool CanAddDate(DateTime date) {
			if (!base.CanAddDate(date))
				return false;
			return (date.Month == CurrentDate.Month);
		}
	}
	#endregion
	public class DateNavigatorPainterBase : CalendarPainter {
	}
	#region DateNavigatorPainter
	public class DateNavigatorPainter : DateNavigatorPainterBase {
	}
	#endregion
	#region DateNavigatorPrinterBase
	public class DateNavigatorPrinterBase : CalendarObjectPainterBase {
		public DateNavigatorPrinterBase(DateNavigatorBase control)
			: base() {
		}
		protected override void DrawHeader(CalendarControlObjectInfoArgs info) {
			CalendarObjectViewInfo vi = (CalendarObjectViewInfo)info.ViewInfo;
			vi.CalendarInfo.PaintStyle.HeaderAppearance.DrawString(info.Cache, vi.CurrentDate.ToString("MMMM yyyy", vi.DateFormat), vi.Header.ContentBounds);
		}
	}
	#endregion
	#region DateNavigatorPrinter
	public class DateNavigatorPrinter : DateNavigatorPrinterBase {
		public DateNavigatorPrinter(DateNavigator control)
			: base(control) {
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region DateNavigatorControllerWin
	public class DateNavigatorControllerWin : DateNavigatorController {
		SchedulerControl schedulerControl;
		public DateNavigatorControllerWin(IDateNavigatorControllerOwner owner)
			: base(owner) {
		}
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (value != null)
					base.InnerControl = value.InnerControl;
				else
					base.InnerControl = null;
				this.schedulerControl = value;
			}
		}
		protected internal override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (schedulerControl == null)
				return;
			schedulerControl.BeforeDispose += new EventHandler(OnBeforeSchedulerControlDispose);
			if (schedulerControl.OptionsView != null)
				schedulerControl.OptionsView.Changed += new BaseOptionChangedEventHandler(OnOptionsViewChanged);
			schedulerControl.LimitIntervalChanged += OnSchedulerControlLimitIntervalChanged;
			schedulerControl.Refreshed += OnSchedulerControlRefreshed;
		}
		protected internal override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			if (schedulerControl == null)
				return;
			schedulerControl.BeforeDispose -= new EventHandler(OnBeforeSchedulerControlDispose);
			if (schedulerControl.OptionsView != null)
				schedulerControl.OptionsView.Changed -= new BaseOptionChangedEventHandler(OnOptionsViewChanged);
			schedulerControl.LimitIntervalChanged -= OnSchedulerControlLimitIntervalChanged;
			schedulerControl.Refreshed -= OnSchedulerControlRefreshed;
		}
		protected override bool ValidateSelectionVisiblity() {
			bool updateAppointmentDateMap = false;
			if (InnerControl.ActiveViewType == SchedulerViewType.Month) {
				if ((Owner.EndDate - Owner.StartDate).Days < DateTime.DaysInMonth(Owner.StartDate.Year, Owner.StartDate.Month)) { 
					if ((Owner.EndDate - Owner.SelectionStart).Days < 7) { 
						Owner.StartDate = Owner.GetFirstDayOfMonth(Owner.EndDate.AddDays(1)); 
						updateAppointmentDateMap = true;
					}
				}
			}
			return updateAppointmentDateMap;
		}
		protected override bool ValidateNavigatorInterval(TimeInterval visibleInterval) {
			bool updateAppointmentDateMap = false;
			DateTime start = visibleInterval.Start.Date;
			DateTime end = visibleInterval.End.Date - DateTimeHelper.DaySpan;
			if (start < Owner.StartDate) {
				DateTime newStartDate = Owner.GetFirstDayOfMonth(start);
				Owner.StartDate = newStartDate;
				updateAppointmentDateMap = true;
			} else if (start > Owner.StartDate && end > Owner.EndDate) {
				TimeSpan shift = start - Owner.StartDate;
				DateTime newStartDate = Owner.GetFirstDayOfMonth(start);
				Owner.StartDate = newStartDate;
				updateAppointmentDateMap = true;
			} else if (end > Owner.EndDate) {
				TimeSpan shift = end - Owner.EndDate;
				DateTime newStart = DateTimeHelper.Max(InnerControl.LimitInterval.Start, Owner.StartDate.Add(shift));
				DateTime newStartDate = Owner.GetFirstDayOfMonth(newStart);
				if (newStartDate == Owner.StartDate)
					newStartDate = newStartDate.AddMonths(1);
				Owner.StartDate = newStartDate;
				updateAppointmentDateMap = true;
			}
			return updateAppointmentDateMap;
		}
		void OnSchedulerControlRefreshed(object sender, EventArgs e) {
			UpdateAppointmentDatesMap();
		}
		void OnSchedulerControlLimitIntervalChanged(object sender, EventArgs e) {
			Owner.StartDate = Owner.GetFirstDayOfMonth(schedulerControl.Start);
			RaiseChanged();
		}
		void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged();
		}
		void OnBeforeSchedulerControlDispose(object sender, EventArgs e) {
			this.SchedulerControl = null;
		}
	}
	#endregion
}
