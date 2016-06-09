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
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.Native {
	public abstract class SchedulerInnerControlClientDataProvider : SchedulerStorageBaseClientDataProvider {
		InnerSchedulerControl innerControl;
		protected SchedulerInnerControlClientDataProvider(InnerSchedulerControl innerControl)
			: base(innerControl.Storage) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
			SubscribeControlEvents();
		}
		#region Properties
		public InnerSchedulerControl InnerControl { get { return innerControl; } }
		protected internal DayOfWeek FirstDayOfWeek { get { return InnerControl != null ? InnerControl.FirstDayOfWeek : DateTimeHelper.FirstDayOfWeek; } }
		protected bool InActiveViewChanging { get; set; }
		protected bool InSchedulerControlSync { get; set; }
		protected bool PrevAutoAdjustMode { get; set; }
		protected abstract bool AllowChangeActiveView { get; }
		protected abstract bool AutoAdjustMode { get; }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.innerControl != null) {
					UnsubscribeControlEvents();
					this.innerControl = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		protected override Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(TimeIntervalCollection intervals) {
			ResourceBaseCollection resources = InnerControl.GetFilteredResources();
			return InnerControl.GetFilteredAppointmentsByIntervals(intervals, resources, InnerControl.TimeZoneHelper, this);
		}
		protected override DateTime GetSelectedRangeStart() {
			return GetSchedulerVisibleInterval().Start;
		}
		protected override DateTime GetSelectedRangeEnd() {
			return GetSchedulerVisibleInterval().End;
		}
		protected virtual TimeInterval GetSchedulerVisibleInterval() {
			return InnerControl.ActiveView.InnerVisibleIntervals.Interval;
		}
		protected override void OnOptionsChangedCore(string name, object oldValue, object newValue) {
			bool currentAutoAdjust = IsAutoAdjustModeActive(SyncSupport.Options);
			bool init = name == "AutoAdjustMode" || PrevAutoAdjustMode != currentAutoAdjust;
			PrevAutoAdjustMode = currentAutoAdjust;
			if (init)
				SyncSupport.ReInitialize();
		}
		protected override void OnSelectedRangeChangedCore(DateTime rangeMinimum, DateTime rangeMaximum) {
			SyncSchedulerControl(rangeMinimum, rangeMaximum);
			SyncSupport.RefreshRangeControl(false);
		}
		protected override IComparer<Appointment> CreateComparer() {
			if (innerControl.ActiveViewType == SchedulerViewType.Timeline || innerControl.ActiveViewType == SchedulerViewType.Gantt)
				return new TimeLineAppointmentComparer();
			else
				return base.CreateComparer();
		}
		protected internal virtual void SyncSchedulerControl(DateTime visibleStart, DateTime visibleEnd) {
			if (InnerControl == null)
				return;
			InSchedulerControlSync = true;
			try {
				if (visibleStart.TimeOfDay != TimeSpan.Zero || visibleEnd.TimeOfDay != TimeSpan.Zero)
					if (InnerControl.TimelineView.Enabled) {
						SyncSchedulerToTimeline(visibleStart, visibleEnd);
					}
					else
						SyncSchedulerStart(visibleStart);
				else
					SyncSchedulerVisibleDays(visibleStart, visibleEnd);
			} finally {
				InSchedulerControlSync = false;
			}
		}
		private void SyncSchedulerToTimeline(DateTime visibleStart, DateTime visibleEnd) {
			InnerControl.BeginUpdate();
			try {
				InnerControl.ActiveViewType = SchedulerViewType.Timeline;
				InnerControl.TimelineView.SetVisibleInterval(visibleStart, visibleEnd, InnerControl.Selection);
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected virtual void SyncSchedulerVisibleDays(DateTime visibleStart, DateTime visibleEnd) {
			DayIntervalCollection days = new DayIntervalCollection();
			TimeInterval newInterval = new TimeInterval(visibleStart, visibleEnd);
			days.Add(newInterval);
			SchedulerViewType viewType = SchedulerViewAutomaticAdjustHelper.SelectAdjustedView(InnerControl, days);
			if (viewType == InnerControl.ActiveViewType) {
				TimeIntervalCollection intervals = InnerControl.ActiveView.GetVisibleIntervals();
				if (intervals.Count == 1 && intervals[0].Equals(newInterval))
					return; 
			}
			InnerControl.BeginUpdate();
			try {
				if (AllowChangeActiveView && !InActiveViewChanging) {
					InnerControl.ActiveViewType = viewType;
				}
				if (InnerControl.ActiveViewType == SchedulerViewType.Timeline)
					InnerControl.ActiveView.SetVisibleInterval(visibleStart, visibleEnd, InnerControl.Selection);
				else
					InnerControl.ActiveView.SetVisibleDays(days, InnerControl.Selection);
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected virtual void SyncSchedulerStart(DateTime visibleStart) {
			InnerControl.BeginUpdate();
			try {
				InnerControl.Start = visibleStart;
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal virtual void OnVisibleIntervalChanged(object sender, EventArgs e) {
			if (CanAdjustRangeControl()) {
				if (CanAutoAdjustRangeControlRange(SyncSupport.Options))
					AutoAdjustRangeControlRange();
				SyncSupport.SyncRangeControlRange();
			}
		}
		protected internal virtual void OnActiveViewChanging(object sender, InnerActiveViewChangingEventArgs e) {
			InActiveViewChanging = true;
		}
		protected virtual bool CanAdjustRangeControl() {
			return SyncSupport != null && SyncSupport.CanAdjustRangeControl && !InSchedulerControlSync;
		}
		protected internal virtual void OnActiveViewChanged(object sender, EventArgs e) {
			TryAdjustRangeControlScales();
			InActiveViewChanging = false;
		}
		protected virtual void TryAdjustRangeControlScales() {
			if (!CanAdjustRangeControl())
				return;
			if (CanAutoAdjustRangeControlScales(SyncSupport.Options))
				AutoAdjustRangeControlScales();
		}
		protected internal virtual void SubscribeControlEvents() {
			InnerControl.VisibleIntervalChanged += OnVisibleIntervalChanged;
			InnerControl.ActiveViewChanging += OnActiveViewChanging;
			InnerControl.ActiveViewChanged += OnActiveViewChanged;
			InnerControl.BeforeDispose += OnBeforeControlDispose;
			InnerControl.LimitIntervalChanged += OnLimitIntervalChanged;
			InnerControl.StorageChanged += OnStorageChanged;
			InnerControl.ViewUIChanged += OnViewUIChanged;
			InnerControl.OptionsView.Changed += OnOptionsViewChanged;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			InnerControl.VisibleIntervalChanged -= OnVisibleIntervalChanged;
			InnerControl.BeforeDispose -= OnBeforeControlDispose;
			InnerControl.ActiveViewChanging -= OnActiveViewChanging;
			InnerControl.ActiveViewChanged -= OnActiveViewChanged;
			InnerControl.LimitIntervalChanged -= OnLimitIntervalChanged;
			InnerControl.StorageChanged -= OnStorageChanged;
			InnerControl.ViewUIChanged -= OnViewUIChanged;
			InnerControl.OptionsView.Changed -= OnOptionsViewChanged;
		}
		protected void OnLimitIntervalChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void OnOptionsViewChanged(object sender, Utils.Controls.BaseOptionChangedEventArgs e) {
			TryAdjustRangeControlScales();
		}
		protected internal virtual void OnBeforeControlDispose(object sender, EventArgs e) {
			UnsubscribeControlEvents();
			SetStorageInternal(null);
			this.innerControl = null;
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			SetStorageInternal(InnerControl.Storage);
		}
		protected virtual void OnViewUIChanged(object sender, SchedulerViewUIChangedEventArgs e) {
			if (e.PropertyName == "Scales")
				TryAdjustRangeControlScales();
		}
		protected virtual bool CanAutoAdjustRangeControl(IScaleBasedRangeControlClientOptions options) {
			return IsAutoAdjustModeActive(options);
		}
		protected virtual bool IsAutoAdjustModeActive(IScaleBasedRangeControlClientOptions options) {
			return AutoAdjustMode;
		}
		protected virtual bool CanAutoAdjustRangeControlRange(IScaleBasedRangeControlClientOptions options) {
			if (InActiveViewChanging)
				return false;
			if (!CanAutoAdjustRangeControl(options))
				return false;
			TimeInterval newInterval = InnerControl.ActiveView.GetVisibleIntervals().Interval;
			return !SyncSupport.TotalRange.Contains(newInterval);
		}
		protected virtual void AutoAdjustRangeControlRange() {
			ISchedulerControlRangeHelper helper = CreateRangeControlHelper();
			TimeInterval adjustedInterval = helper.CalculateAdjustedRange(InnerControl);
			TimeScaleCollection scales = helper.CalculateAdjustedTimeScales(InnerControl);
			RangeControlAdjustEventArgs args = CreateRangeControlAdjustEventArgs(adjustedInterval, scales);
			SyncSupport.AdjustRangeControlRange(args);
		}
		protected virtual ISchedulerControlRangeHelper CreateRangeControlHelper() {
			return new SchedulerControlRangeHelper();
		}
		protected virtual bool CanAutoAdjustRangeControlScales(IScaleBasedRangeControlClientOptions options) {
			return CanAutoAdjustRangeControl(options);
		}
		protected virtual void AutoAdjustRangeControlScales() {
			ISchedulerControlRangeHelper helper = CreateRangeControlHelper();
			TimeInterval adjustedInterval = helper.CalculateAdjustedRange(InnerControl);
			TimeScaleCollection scales = helper.CalculateAdjustedTimeScales(InnerControl);
			RangeControlAdjustEventArgs args = CreateRangeControlAdjustEventArgs(adjustedInterval, scales);
			SyncSupport.AdjustRangeControlScales(args);
		}
		protected virtual RangeControlAdjustEventArgs CreateRangeControlAdjustEventArgs(TimeInterval interval, TimeScaleCollection scales) {
			RangeControlAdjustEventArgs args = new RangeControlAdjustEventArgs();
			args.RangeMinimum = interval.Start;
			args.RangeMaximum = interval.End;
			args.Scales.AddRange(scales);
			return args;
		}
	}
}
