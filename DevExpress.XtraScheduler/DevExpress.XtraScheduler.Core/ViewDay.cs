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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Drawing;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region IInnerDayViewOwner
	public interface IInnerDayViewOwner {
		TimeInterval GetVisibleRowsTimeInterval();
		TimeInterval GetAvailableRowsTimeInterval();
		TimeSpan GetTopRowTime();
		void SetScrollStartTimeCore(TimeSpan value);
	}
	#endregion
	public interface IDayViewProperties : ISchedulerViewPropertiesBase {
	}
	#region InnerDayView
	public class InnerDayView : InnerSchedulerViewBase {
		internal static readonly TimeOfDayInterval defaultVisibleTime = TimeOfDayInterval.Day;
		internal static readonly WorkTimeInterval defaultWorkTime = WorkTimeInterval.WorkTime;
		internal static readonly TimeSpan defaultTimeScale = TimeSpan.FromMinutes(30); 
		#region Fields        
		internal const bool DefaultShowWorkTimeOnly = false;
		internal const bool DefaultShowDayHeaders = true;
		internal const bool DefaultShowMoreButtonsOnEachColumn = false;
		internal const bool DefaultShowAllDayArea = true;
		internal const bool DefaultVisibleTimeSnapMode = false;
		internal const int DefaultDayCount = 1;
		internal const bool DefaultShowAllAppointmentsAtTimeCells = false;
		internal const TimeMarkerVisibility DefaultTimeMarkerVisibility = TimeMarkerVisibility.TodayView;
		bool showWorkTimeOnly;
		bool showAllDayArea;
		bool showAllAppointmentsAtTimeCells;
		bool visibleTimeSnapMode;
		TimeOfDayInterval workTime;
		SchedulerBlocker lockApplyChanges = new SchedulerBlocker();
		TimeSpan timeScale;
		TimeSpan assignedTimeScale;
		TimeOfDayInterval visibleTime;
		TimeSlotCollection timeSlots;
		TimeRulerCollection timeRulers;
		NotificationCollectionChangedListener<TimeSlot> timeSlotCollectionChangedListener;
		TimeRulerCollectionChangedListener timeRulerCollectionChangedListener;
		int dayCount;
		bool showDayHeaders;
		bool showMoreButtonsOnEachColumn;
		TimeMarkerVisibility timeMarkerVisibility = DefaultTimeMarkerVisibility;
		#endregion
		public InnerDayView(IInnerSchedulerViewOwner owner, IDayViewProperties properties)
			: base(owner, properties) {
		}
		#region Properties
		#region ShowWorkTimeOnly
		[DefaultValue(DefaultShowWorkTimeOnly), XtraSerializableProperty()]
		public bool ShowWorkTimeOnly {
			get { return showWorkTimeOnly; }
			set {
				if (showWorkTimeOnly == value)
					return;
				showWorkTimeOnly = value;
				RaiseChanged(SchedulerControlChangeType.ShowWorkTimeOnlyChanged);
			}
		}
		#endregion
		#region ShowDayHeaders
		[DefaultValue(DefaultShowDayHeaders), XtraSerializableProperty()]
		public bool ShowDayHeaders {
			get { return showDayHeaders; }
			set {
				if (showDayHeaders == value)
					return;
				showDayHeaders = value;
				RaiseChanged(SchedulerControlChangeType.ShowDayViewDayHeadersChanged);
			}
		}
		#endregion
		#region ShowMoreButtonsOnEachColumn
		[DefaultValue(DefaultShowMoreButtonsOnEachColumn), XtraSerializableProperty()]
		public bool ShowMoreButtonsOnEachColumn {
			get { return showMoreButtonsOnEachColumn; }
			set {
				if (showMoreButtonsOnEachColumn == value)
					return;
				showMoreButtonsOnEachColumn = value;
				RaiseChanged(SchedulerControlChangeType.ShowMoreButtonsOnEachColumnChanged);
			}
		}
		#endregion
		#region ShowAllDayArea
		[DefaultValue(DefaultShowAllDayArea), XtraSerializableProperty()]
		public bool ShowAllDayArea {
			get { return showAllDayArea; }
			set {
				if (showAllDayArea == value)
					return;
				showAllDayArea = value;
				RaiseChanged(SchedulerControlChangeType.ShowAllDayAreaChanged);
			}
		}
		#endregion
		#region VisibleTime
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimeOfDayInterval VisibleTime {
			get { return visibleTime; }
			set {
				if (visibleTime.IsEqual(value))
					return;
				UnsubscribeVisibleTimeEvents();
				visibleTime = value.Clone();
				SubscribeVisibleTimeEvents();
				RaiseChanged(SchedulerControlChangeType.VisibleTimeChanged);
			}
		}
		internal bool ShouldSerializeVisibleTime() {
			return !VisibleTime.IsEqual(defaultVisibleTime);
		}
		internal void ResetVisibleTime() {
			VisibleTime = defaultVisibleTime.Clone();
		}
		internal bool XtraShouldSerializeVisibleTime() {
			return ShouldSerializeVisibleTime();
		}
		#endregion
		#region VisibleTimeSnapMode
		[DefaultValue(DefaultVisibleTimeSnapMode), XtraSerializableProperty()]
		public bool VisibleTimeSnapMode {
			get { return visibleTimeSnapMode; }
			set {
				if (visibleTimeSnapMode == value)
					return;
				visibleTimeSnapMode = value;
				RaiseChanged(SchedulerControlChangeType.VisibleTimeSnapModeChanged);
			}
		}
		#endregion
		#region WorkTime
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval WorkTime {
			get { return workTime; }
			set {
				if (workTime.IsEqual(value))
					return;
				UnsubscribeWorkTimeEvents();
				workTime = value.Clone();
				SubscribeWorkTimeEvents();
				RaiseChanged(SchedulerControlChangeType.WorkTimeChanged);
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(defaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = defaultWorkTime;
		}
		internal bool XtraShouldSerializeWorkTime() {
			return ShouldSerializeWorkTime();
		}
		#endregion
		#region TimeScale
		[XtraSerializableProperty()]
		public TimeSpan TimeScale {
			get {
				return timeScale;
			}
			set {
				if (assignedTimeScale == value)
					return;
				assignedTimeScale = value;
				RefreshTimeScaleValue(assignedTimeScale);
			}
		}
		internal bool ShouldSerializeTimeScale() {
			return TimeScale != defaultTimeScale;
		}
		internal void ResetTimeScale() {
			TimeScale = defaultTimeScale;
		}
		internal bool XtraShouldSerializeTimeScale() {
			return ShouldSerializeTimeScale();
		}
		#endregion
		#region TimeSlots
		public TimeSlotCollection TimeSlots { get { return timeSlots; } }
		protected internal virtual object XtraCreateTimeSlotsItem(XtraItemEventArgs e) {
			XtraPropertyInfo propertyInfo = e.Item.ChildProperties["DisplayName"];
			if (propertyInfo == null || propertyInfo.Value == null)
				return new TimeSlot();
			else
				return new TimeSlot(DateTimeHelper.FiveMinutesSpan, propertyInfo.Value.ToString());
		}
		protected internal virtual void XtraSetIndexTimeSlotsItem(XtraSetItemIndexEventArgs e) {
			TimeSlot slot = e.Item.Value as TimeSlot;
			if (slot == null)
				return;
			TimeSlots.Add(slot);
		}
		#endregion
		#region TimeRulers
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TimeRulerCollection TimeRulers { get { return timeRulers; } }
		protected internal virtual object XtraCreateTimeRulersItem(XtraItemEventArgs e) {
			return new TimeRuler();
		}
		protected internal virtual void XtraSetIndexTimeRulersItem(XtraSetItemIndexEventArgs e) {
			TimeRuler ruler = e.Item.Value as TimeRuler;
			if (ruler == null)
				return;
			TimeRulers.Add(ruler);
		}
		#endregion
		#region ShowAllAppointmentsAtTimeCells
		[
DefaultValue(DefaultShowAllAppointmentsAtTimeCells), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowAllAppointmentsAtTimeCells {
			get { return showAllAppointmentsAtTimeCells; }
			set {
				if (showAllAppointmentsAtTimeCells == value)
					return;
				showAllAppointmentsAtTimeCells = value;
				RaiseChanged(SchedulerControlChangeType.ShowAllAppointmentsAtTimeCellsChanged);
			}
		}
		#endregion
		#region TimeMarkerVisibility
		[DefaultValue(DefaultTimeMarkerVisibility), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public virtual TimeMarkerVisibility TimeMarkerVisibility {
			get { return timeMarkerVisibility; }
			set {
				TimeMarkerVisibility oldValue = timeMarkerVisibility;
				if (oldValue == value)
					return;
				timeMarkerVisibility = value;
				RaiseChanged(SchedulerControlChangeType.TimeMarkerVisibilityChanged);
			}
		}
		#endregion
		internal NotificationCollectionChangedListener<TimeSlot> TimeSlotCollectionChangedListener { get { return timeSlotCollectionChangedListener; } }
		internal TimeRulerCollectionChangedListener TimeRulerCollectionChangedListener { get { return timeRulerCollectionChangedListener; } }
		protected internal bool ActualShowAllAppointmentsAtTimeCells { get { return !ShowAllDayArea || ShowAllAppointmentsAtTimeCells; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Day; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override Keys Shortcut { get { return Keys.Control | Keys.Alt | Keys.D1; } }
		protected internal TimeOfDayInterval ActualVisibleTime {
			get { return ShowWorkTimeOnly ? TimeOfDayInterval.Intersect(WorkTime, visibleTime) : visibleTime; }				
		}
		#region DayCount
		[DefaultValue(DefaultDayCount), XtraSerializableProperty()]
		public virtual int DayCount {
			get { return dayCount; }
			set {
				if (value < 1)
					Exceptions.ThrowArgumentException("DayCount", value);
				if (dayCount == value)
					return;
				SetDayCountCore(value);
				RaiseChanged(SchedulerControlChangeType.DayCountChanged, this.lockApplyChanges.IsLocked);
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToDayView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewDisplayName_Day); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultShortDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewShortDisplayName_Day); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchToDayView); } }
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new DayViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (DayViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string Description { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_DayViewDescription); } }
		protected internal virtual bool ShowExtendedCells { get { return false; } }
		protected internal virtual int MinimumExtendedCellsInColumn { get { return 3; } }
		#endregion
		public override void Initialize(SchedulerViewSelection selection) {
			base.Initialize(selection);
			showWorkTimeOnly = DefaultShowWorkTimeOnly;
			timeSlots = CreateTimeSlotCollection();
			timeRulers = CreateTimeRulerCollection();
			timeRulers.Add(new TimeRuler());
			workTime = WorkTimeInterval.WorkTime;
			assignedTimeScale = defaultTimeScale;
			timeScale = defaultTimeScale;
			visibleTime = TimeOfDayInterval.Day.Clone();
			timeSlotCollectionChangedListener = CreateTimeSlotCollectionListener();
			timeRulerCollectionChangedListener = CreateTimeRulerCollectionListener();
			SubscribeTimeSlotsEvents();
			SubscribeTimeRulersEvents();
			SubscribeVisibleTimeEvents();
			SubscribeWorkTimeEvents();
			showAllDayArea = DefaultShowAllDayArea;
			ShowAllAppointmentsAtTimeCells = DefaultShowAllAppointmentsAtTimeCells;
			dayCount = DefaultDayCount;
			showDayHeaders = DefaultShowDayHeaders;
			showMoreButtonsOnEachColumn = DefaultShowMoreButtonsOnEachColumn;
			TimeMarkerVisibility = DefaultTimeMarkerVisibility;
		}
		public override void Reset() {
			base.Reset();
			DayCount = DefaultDayCount;
			ShowWorkTimeOnly = DefaultShowWorkTimeOnly;
			TimeScale = defaultTimeScale;
			VisibleTime = TimeOfDayInterval.Day;
			WorkTime = WorkTimeInterval.WorkTime;
			ShowAllDayArea = DefaultShowAllDayArea;
			ShowDayHeaders = DefaultShowDayHeaders;
			ShowMoreButtonsOnEachColumn = DefaultShowMoreButtonsOnEachColumn;
			ShowAllAppointmentsAtTimeCells = DefaultShowAllAppointmentsAtTimeCells;
			TimeMarkerVisibility = DefaultTimeMarkerVisibility;
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (timeSlotCollectionChangedListener != null) {
						UnsubscribeTimeSlotsEvents();
						timeSlotCollectionChangedListener.Dispose();
						timeSlotCollectionChangedListener = null;
					}
					if (timeRulerCollectionChangedListener != null) {
						UnsubscribeTimeRulersEvents();
						timeRulerCollectionChangedListener.Dispose();
						timeRulerCollectionChangedListener = null;
					}
					if (workTime != null) {
						UnsubscribeWorkTimeEvents();
						workTime = null;
					}
					if (visibleTime != null) {
						UnsubscribeVisibleTimeEvents();
						visibleTime = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void SetDayCountCore(int val) {
			this.dayCount = val;
		}
		protected internal virtual void SubscribeTimeSlotsEvents() {
			timeSlotCollectionChangedListener.Changed += new EventHandler(OnTimeSlotsChanged);
		}
		protected internal virtual void UnsubscribeTimeSlotsEvents() {
			timeSlotCollectionChangedListener.Changed -= new EventHandler(OnTimeSlotsChanged);
		}
		protected internal virtual void SubscribeTimeRulersEvents() {
			timeRulerCollectionChangedListener.Changed += new EventHandler(OnTimeRulersChanged);
		}
		protected internal virtual void UnsubscribeTimeRulersEvents() {
			timeRulerCollectionChangedListener.Changed -= new EventHandler(OnTimeRulersChanged);
		}
		protected internal virtual void SubscribeVisibleTimeEvents() {
			visibleTime.Changed += new EventHandler(OnVisibleTimeChanged);
		}
		protected internal virtual void UnsubscribeVisibleTimeEvents() {
			visibleTime.Changed -= new EventHandler(OnVisibleTimeChanged);
		}
		protected internal virtual void SubscribeWorkTimeEvents() {
			workTime.Changed += new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void UnsubscribeWorkTimeEvents() {
			workTime.Changed -= new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void OnTimeSlotsChanged(object sender, EventArgs e) {
			TimeSlotComparer timeSlotComparer = new TimeSlotComparer();
			TimeSlots.Sort(timeSlotComparer);
			RefreshTimeScaleValue(assignedTimeScale);
		}
		protected internal virtual void OnTimeRulersChanged(object sender, EventArgs e) {
			RaiseChanged(SchedulerControlChangeType.TimeRulersChanged);
		}
		protected internal virtual void OnVisibleTimeChanged(object sender, EventArgs e) {
			RaiseChanged(SchedulerControlChangeType.VisibleTimeChanged);
		}
		protected internal virtual void OnWorkTimeChanged(object sender, EventArgs e) {
			RaiseChanged(SchedulerControlChangeType.WorkTimeChanged);
		}
		protected internal virtual void RefreshTimeScaleValue(TimeSpan assignedTimeScale) {
			TimeSpan newTimeScale;
			if (timeSlots.Count > 0)
				newTimeScale = TimeSlots.FindNearestTimeSlotValue(assignedTimeScale);
			else
				newTimeScale = defaultTimeScale;
			if (newTimeScale == timeScale)
				return;
			TimeSpan oldValue = this.timeScale;
			this.timeScale = newTimeScale;
			RaisePropertyChanged("TimeScale");
			RaiseChanged(SchedulerControlChangeType.TimeScaleChanged);
			RaiseUIChanged("TimeScale", oldValue, newTimeScale);
		}
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			return new DayIntervalCollection();
		}
		protected internal virtual TimeSlotCollection CreateTimeSlotCollection() {
			return new TimeSlotCollection();
		}
		protected internal virtual TimeRulerCollection CreateTimeRulerCollection() {
			return new TimeRulerCollection();
		}
		protected internal override void CreateVisibleIntervalsCore(DateTime date) {
			InnerVisibleIntervals.Add(new TimeInterval(date.Date, TimeSpan.FromDays(DayCount)));
		}
		protected internal override TimeInterval RoundLimitInterval(TimeInterval interval) {
			DateTime start = DateTimeHelper.Floor(interval.Start, DateTimeHelper.DaySpan);
			DateTime end = DateTimeHelper.Ceil(interval.End, DateTimeHelper.DaySpan);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval RoundSelectionInterval(TimeInterval interval) {
			TimeSpan baseTimeOfDayDate = VisibleTime.Start;
			DateTime baseDate = interval.Start.Date.Add(baseTimeOfDayDate);
			DateTime start = DateTimeHelper.Floor(interval.Start, TimeScale, baseDate);
			DateTime end = DateTimeHelper.Ceil(interval.End, TimeScale, baseTimeOfDayDate);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval CreateDefaultSelectionInterval(DateTime date) {
			DateTime start = date.Date;
			start += TimeSpan.FromTicks(Math.Min((VisibleTime.End - TimeScale).Ticks, Math.Max(WorkTime.Start.Ticks, VisibleTime.Start.Ticks)));
			start = DateTimeHelper.Floor(start, TimeScale);
			return new TimeInterval(start, TimeScale);
		}
		protected internal override ChangeActions SynchronizeSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged) {
			DateTime baseDate = selection.Interval.Start.Date + VisibleTime.Start;
			DateTime start = DateTimeHelper.Floor(selection.Interval.Start, TimeScale, baseDate);
			DateTime end = DateTimeHelper.Ceil(selection.Interval.End, TimeScale, VisibleTime.Start);
			TimeInterval newSelectionInterval;
			if (!activeViewChanged || (end - start < DateTimeHelper.DaySpan))
				newSelectionInterval = new TimeInterval(start, end);
			else
				newSelectionInterval = CreateDefaultSelectionInterval(start);
			ChangeActions result = newSelectionInterval.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
			selection.Interval = newSelectionInterval;
			result |= ValidateSelectionInterval(selection);
			return result;
		}
		protected internal virtual TimeInterval CreateValidSelectionInterval(SchedulerViewSelection selection) {
			DateTime start = InnerVisibleIntervals.Start.Date + ActualVisibleTime.Start;
			DateTime end = InnerVisibleIntervals[InnerVisibleIntervals.Count - 1].Start.Date + ActualVisibleTime.End;
			TimeInterval interval = new TimeInterval(start, end);
			return TimeInterval.Intersect(interval, selection.Interval);
		}
		protected internal override ChangeActions ValidateSelectionInterval(SchedulerViewSelection selection) {
			TimeInterval validSelection = CreateValidSelectionInterval(selection);
			if (validSelection.Duration == TimeSpan.Zero) {
				if (selection.Interval.Duration < ActualVisibleTime.Duration) {
					selection.Interval.Start = InnerVisibleIntervals.Start.Date + selection.Interval.Start.TimeOfDay;
					validSelection = CreateValidSelectionInterval(selection);
				}
			}
			if (validSelection.Duration == TimeSpan.Zero)
				return InitializeSelection(selection);
			else {
				TimeInterval startDayInterval = CalcActualDayInterval(validSelection.Start);
				TimeInterval endDayInterval = CalcActualDayInterval(validSelection.End);
				if (!startDayInterval.Contains(validSelection.Start))
					validSelection = new TimeInterval(startDayInterval.End - TimeScale, validSelection.End);
				if (!endDayInterval.Contains(validSelection.End))
					validSelection = new TimeInterval(validSelection.Start, endDayInterval.End);
				ChangeActions result = validSelection.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
				selection.Interval = validSelection;
				selection.FirstSelectedInterval = new TimeInterval(validSelection.Start, TimeScale);
				return result;
			}
		}
		protected internal virtual TimeInterval CalcActualDayInterval(DateTime dateTime) {
			DateTime start;
			if (dateTime.TimeOfDay < ActualVisibleTime.Start)
				start = dateTime.Date.AddDays(-1);
			else
				start = dateTime.Date;
			start += ActualVisibleTime.Start;
			DateTime end = start + ActualVisibleTime.Duration;
			return new TimeInterval(start, end);
		}
		protected internal virtual DateTime CalculateActualDate(DateTime dateTime) {
			DateTime date = dateTime.Date;
			if (dateTime.TimeOfDay < ActualVisibleTime.Start)
				date = date.AddDays(-1);
			return date;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			return days;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DateTime visibleStart, DateTime visibleEnd) {
			DayIntervalCollection result = new DayIntervalCollection();
			result.Add(new TimeInterval(visibleStart, visibleEnd));
			return result;
		}
		protected internal override ChangeActions SetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			base.SetVisibleIntervalsCore(intervals);
			using (SchedulerBlocker locker = this.lockApplyChanges.Lock()) {
				DayCount = intervals.Count;
			}			
			return ChangeActions.None;
		}
		protected internal override TimeInterval ApplyVisibleTime(TimeInterval interval) {
			XtraSchedulerDebug.Assert(interval.Start.TimeOfDay.Ticks == 0);
			XtraSchedulerDebug.Assert(interval.End.TimeOfDay.Ticks == 0);
			TimeSpan delta = ActualVisibleTime.End - TimeSpan.FromDays(1);
			return new TimeInterval(interval.Start.Date.Add(ActualVisibleTime.Start), interval.End.Add(delta));
		}
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return RaiseQueryWorkTime(interval, resource);
		}
		protected internal virtual string GetClientTimeZoneId() {
			return Owner.ClientTimeZoneId;
		}
		protected internal virtual void UpdateTimeRulersClientTimeZone(TimeZoneInfo clientTimeZone) {
			int count = TimeRulers.Count;
			for (int i = 0; i < count; i++)
				TimeRulers[i].UpdateClientTimeZone(clientTimeZone.Id);
		}
		protected internal virtual TimeRulerCollection GetVisibleTimeRulers() {
			TimeRulerCollection result = new TimeRulerCollection();
			int count = TimeRulers.Count;
			for (int i = 0; i < count; i++) {
				if (TimeRulers[i].Visible)
					result.Add(TimeRulers[i]);
			}
			return result;
		}
		public override void ZoomIn() {
			int currentSlotIndex = TimeSlots.FindNearestTimeSlotIndex(TimeScale);
			if (!CanZoomInCore(currentSlotIndex))
				return;
			TimeScale = TimeSlots[currentSlotIndex + 1].Value;
		}
		protected internal override bool CanZoomIn() {
			return CanZoomInCore(TimeSlots.FindNearestTimeSlotIndex(TimeScale));
		}
		protected internal virtual bool CanZoomInCore(int currentSlotIndex) {
			if (currentSlotIndex < 0)
				return false;
			return (currentSlotIndex + 1 < TimeSlots.Count);
		}
		public override void ZoomOut() {
			int currentSlotIndex = TimeSlots.FindNearestTimeSlotIndex(TimeScale);
			if (!CanZoomOutCore(currentSlotIndex))
				return;
			TimeScale = TimeSlots[currentSlotIndex - 1].Value;
		}
		protected internal override bool CanZoomOut() {
			return CanZoomOutCore(TimeSlots.FindNearestTimeSlotIndex(TimeScale));
		}
		protected internal virtual bool CanZoomOutCore(int currentSlotIndex) {
			if (currentSlotIndex < 0)
				return false;
			return (currentSlotIndex - 1) >= 0;
		}
		protected internal virtual TimeInterval GetVisibleRowsTimeInterval() {
			IInnerDayViewOwner owner = Owner as IInnerDayViewOwner;
			if (owner != null)
				return owner.GetVisibleRowsTimeInterval();
			else
				return TimeInterval.Empty;
		}
		protected internal virtual TimeInterval GetAvailableRowsTimeInterval() {
			IInnerDayViewOwner owner = Owner as IInnerDayViewOwner;
			if (owner != null)
				return owner.GetAvailableRowsTimeInterval();
			else
				return TimeInterval.Empty;
		}
		protected internal virtual TimeSpan GetTopRowTime() {
			IInnerDayViewOwner owner = Owner as IInnerDayViewOwner;
			if (owner != null)
				return owner.GetTopRowTime();
			else
				return TimeSpan.Zero;
		}
		protected internal virtual void SetScrollStartTimeCore(TimeSpan value) {
			IInnerDayViewOwner owner = Owner as IInnerDayViewOwner;
			if (owner != null)
				owner.SetScrollStartTimeCore(value);
		}
		protected internal TimeRulerCollectionChangedListener CreateTimeRulerCollectionListener() {
			return new TimeRulerCollectionChangedListener(this, timeRulers);
		}
		private NotificationCollectionChangedListener<TimeSlot> CreateTimeSlotCollectionListener() {
			return new NotificationCollectionChangedListener<TimeSlot>(timeSlots);
		}
	}
	#endregion
	#region WorkTimeInfoCalculatorBase
	public abstract class WorkTimeInfoCalculatorBase {
		public virtual WorkTimeInfo CalcWorkTimeInfo(TimeInterval interval, Resource resource) {
			TimeOfDayIntervalCollection workTimes = CalcResourceWorkTimeInterval(interval, resource);
			bool isWorkDay = IsWorkDay(interval.Start, workTimes);
			if(workTimes.Count <= 0)
				workTimes.Add(WorkTime.Clone());
			return new WorkTimeInfo(workTimes, isWorkDay);
		}
		protected internal virtual bool IsWorkDay(DateTime start, TimeOfDayIntervalCollection workTimes) {
			if(workTimes.Count <= 0)
				return WorkDays.IsWorkDay(start);
			else
				return !TimeOfDayInterval.Empty.IsEqual(new TimeOfDayInterval(workTimes.Start, workTimes.End));
		}
		public virtual bool CalcIsWorkTime(ITimeCell cell) {
			TimeOfDayIntervalCollection workTimes = CalcResourceWorkTimeInterval(cell.Interval, cell.Resource);
			return IsWorkTime(cell.Interval, workTimes);
		}
		protected internal virtual bool IsWorkTime(TimeInterval interval, TimeOfDayIntervalCollection workTimes) {
			if(interval.Duration > TimeSpan.FromDays(1))
				return false;
			if(workTimes.Count <= 0) {
				if(!WorkDays.IsWorkDay(interval.Start))
					return false;
				else {
					TimeOfDayIntervalCollection defaultWorkTimes = new TimeOfDayIntervalCollection();
					defaultWorkTimes.Add(WorkTime);
					return IsWorkTimeCore(interval, defaultWorkTimes);
				}
			}
			else
				return IsWorkTimeCore(interval, workTimes);
		}
		protected internal virtual bool IsWorkTimeCore(TimeInterval interval, TimeOfDayIntervalCollection workTimes) {
			TimeSpan startTime = interval.Start.TimeOfDay;
			TimeOfDayInterval timeInterval1 = new TimeOfDayInterval(startTime, startTime + interval.Duration);			
			startTime = startTime.Add(TimeSpan.FromDays(1));
			TimeOfDayInterval timeInterval2 = new TimeOfDayInterval(startTime, startTime + interval.Duration);
			return workTimes.IntersectsWithExcludingBounds(timeInterval1) || workTimes.IntersectsWithExcludingBounds(timeInterval2);
		}
		public abstract WorkDaysCollection WorkDays { get; }
		protected internal abstract TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource);
		protected internal abstract TimeOfDayInterval WorkTime { get; }
	}
	#endregion
	#region DayViewWorkTimeInfoCalculatorBase
	public abstract class DayViewWorkTimeInfoCalculatorBase : WorkTimeInfoCalculatorBase {
	}
	#endregion
	#region DayViewWorkTimeInfoCalculator
	public class DayViewWorkTimeInfoCalculator : DayViewWorkTimeInfoCalculatorBase {
		InnerDayView innerView;
		public DayViewWorkTimeInfoCalculator(InnerDayView innerView) {
			this.innerView = innerView;
		}
		public InnerDayView InnerView { get { return innerView; } }
		public override WorkDaysCollection WorkDays { get { return InnerView.Owner.WorkDays; } }
		protected internal override TimeOfDayInterval WorkTime {
			get { return InnerView.WorkTime; }
		}
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return InnerView.CalcResourceWorkTimeInterval(interval, resource);
		}
	}
	#endregion
	#region DayViewTimeCellHelper
	public static class DayViewTimeCellHelper {
		public static bool IsWorkTime(IWorkTimeInfo workTimeInfo, TimeInterval cellInterval, TimeInterval containerInterval) {
			DateTime start = containerInterval.Start.Date;
			TimeOfDayInterval timeInterval = new TimeOfDayInterval(cellInterval.Start - start, cellInterval.End - start);
			return workTimeInfo.IsWorkDay && workTimeInfo.WorkTimes.IntersectsWithExcludingBounds(timeInterval);
		}
	}
	#endregion
	#region TimeRulerCollectionChangedListener
	public class TimeRulerCollectionChangedListener : NotificationCollectionChangedListener<TimeRuler> {
		InnerDayView view;
		public TimeRulerCollectionChangedListener(InnerDayView view, TimeRulerCollection collection)
			: base(collection) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			this.view = view;
		}
		public InnerDayView View { get { return view; } }
		protected override void SubscribeObjectEvents(TimeRuler obj) {
			base.SubscribeObjectEvents(obj);
			obj.QueryClientTimeZoneId += new QueryClientTimeZoneIdEventHandler(OnQueryClientTimeZoneId);
		}
		protected override void UnsubscribeObjectEvents(TimeRuler obj) {
			base.UnsubscribeObjectEvents(obj);
			obj.QueryClientTimeZoneId -= new QueryClientTimeZoneIdEventHandler(OnQueryClientTimeZoneId);
		}
		protected internal virtual void OnQueryClientTimeZoneId(object sender, QueryClientTimeZoneIdEventArgs e) {
			e.TimeZoneId = View.GetClientTimeZoneId();
		}
	}
	#endregion
}
