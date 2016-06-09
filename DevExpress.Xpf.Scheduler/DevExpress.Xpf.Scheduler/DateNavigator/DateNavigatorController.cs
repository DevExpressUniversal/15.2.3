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
using DevExpress.XtraScheduler.Native;
using System.Windows;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Native {
	[Flags]
	public enum DateNavigatorChangeType { None = 0x00, SyncSelection = 0x01, UpdateSpecialDates = 0x02, SuppressScroll = 0x04, SuppressValidation = 0x08 };
	#region SchedulerDateNavigatorController
	public class SchedulerDateNavigatorController : DateNavigatorController {
		readonly ISchedulerDateNavigatorControllerOwner navigatorOwner;
		SchedulerControl schedulerControl;
		TimeInterval lastActualVisibleInterval = TimeInterval.Empty;
		DateNavigatorChangeType navigatorChangeTypes = DateNavigatorChangeType.None;
		Locker syncNavigatorLock;
		public SchedulerDateNavigatorController(IDateNavigatorControllerOwner owner, ISchedulerDateNavigatorControllerOwner navigatorOwner)
			: base(owner) {
			Guard.ArgumentNotNull(navigatorOwner, "navigatorOwner");
			this.navigatorOwner = navigatorOwner;
			this.syncNavigatorLock = new Locker();
		}
		#region Properties
		internal ISchedulerDateNavigatorControllerOwner NavigatorOwner { get { return navigatorOwner; } }
		protected TimeInterval LastActualVisibleInterval { get { return lastActualVisibleInterval; } }
		protected internal bool RoundSelectionToEntireWeek { get { return NavigatorOwner.RoundSelectionToEntireWeek; } }
		protected internal DateNavigatorChangeType NavigatorChangeTypes { get { return navigatorChangeTypes; } }
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (value != null)
					SetInnerControl(value.InnerControl);
				else
					base.InnerControl = null;
				this.schedulerControl = value;
			}
		}
		protected bool IsReady {
			get {
				if (InnerControl == null)
					return false;
				return NavigatorOwner.IsReady;
			}
		}
		protected internal override bool CanSyncNavigatorWithScheduler { get { return IsReady; } }
		protected virtual internal bool CanSyncSchedulerSelection {
			get { return InnerControl != null && !this.syncNavigatorLock.IsLocked && NavigatorOwner.HasSelection; }
		}
		#endregion
		public void Lock() {
			this.syncNavigatorLock.Lock();
		}
		public void Unlock() {
			this.syncNavigatorLock.Unlock();
		}
		protected internal bool IsNavigatorChanges(DateNavigatorChangeType changes) {
			return (NavigatorChangeTypes & changes) == changes;
		}
		protected internal void ClearNavigatorChanges() {
			this.navigatorChangeTypes = DateNavigatorChangeType.None;
		}
		protected internal void BeginNavigatorChanges(DateNavigatorChangeType change) {
			this.navigatorChangeTypes |= change;
		}
		protected internal void EndNavigatorChanges(DateNavigatorChangeType change) {
			this.navigatorChangeTypes &= ~change;
		}
		protected virtual void SetInnerControl(InnerSchedulerControl innerControl) {
			this.syncNavigatorLock.Lock();  
			try {
				base.InnerControl = innerControl;
			} finally {
				this.syncNavigatorLock.Unlock();
			}
		}
		public bool IsNavigatorVisibleIntervalChanged() {
			if (!CanObtainVisibleAppointments())
				return false;
			TimeInterval actualInterval = GetVisibleInterval();
			return !(LastActualVisibleInterval.Start == actualInterval.Start && LastActualVisibleInterval.Duration == actualInterval.Duration);
		}
		protected override bool CanObtainVisibleAppointments() {
			return base.CanObtainVisibleAppointments() && IsReady;
		}
		protected internal override bool UpdateAppointmentDatesMapCore() {
			EndNavigatorChanges(DateNavigatorChangeType.UpdateSpecialDates);
			if (base.UpdateAppointmentDatesMapCore())
				BeginNavigatorChanges(DateNavigatorChangeType.UpdateSpecialDates);
			UpdateNavigatorVisibleInterval();
			return IsNavigatorChanges(DateNavigatorChangeType.UpdateSpecialDates);
		}
		protected internal void SyncSchedulerSelection() {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerDateNavigatorController.SyncSchedulerSelection: CanSyncSelection={0}", CanSyncSchedulerSelection);
			if (!CanSyncSchedulerSelection)
				return;
			BeginNavigatorChanges(DateNavigatorChangeType.SyncSelection);
			try {
				SyncSelection();
			} finally {
				EndNavigatorChanges(DateNavigatorChangeType.SyncSelection);
			}
		}
		protected internal void ShiftSchedulerIntervals(TimeSpan offset) {
			if (!CanSyncNavigatorWithScheduler || offset == TimeSpan.Zero)
				return;
			BeginNavigatorChanges(DateNavigatorChangeType.SyncSelection | DateNavigatorChangeType.SuppressScroll);
			try {
				ShiftVisibleIntervals(offset);  
			} finally {
				EndNavigatorChanges(DateNavigatorChangeType.SyncSelection | DateNavigatorChangeType.SuppressScroll);
			}
		}
		protected virtual void UpdateNavigatorVisibleInterval() {
			if (!CanObtainVisibleAppointments())
				return;
			this.lastActualVisibleInterval = GetVisibleInterval();
		}
		protected virtual TimeInterval GetNavigatorVisibleInterval() {
			return new TimeInterval(Owner.StartDate, Owner.EndDate);
		}
		protected internal override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (schedulerControl == null)
				return;
			if (InnerControl.OptionsView != null)
				InnerControl.OptionsView.Changed += new BaseOptionChangedEventHandler(OnOptionsViewChanged);
		}
		protected internal override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			if (schedulerControl == null)
				return;
			if (InnerControl.OptionsView != null)
				InnerControl.OptionsView.Changed -= new BaseOptionChangedEventHandler(OnOptionsViewChanged);
		}
		void OnOptionsViewChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		protected internal override void SyncNavigatorWithScheduler() {
			this.syncNavigatorLock.Lock();
			try {
				base.SyncNavigatorWithScheduler();
				UpdateFocusedDate();
			} finally {
				this.syncNavigatorLock.Unlock();
			}
		}
		void UpdateFocusedDate() {
			if (InnerControl == null)
				return;
			NavigatorOwner.Navigation.Move(InnerControl.Start);
		}
		public void UnlockLockFocusedDateUpdate() {
		}
		public void LockFocusedDateUpdate() {
		}
		protected override bool ValidateNavigatorInterval(TimeInterval schedulerVisibleInterval) {
			SchedulerDateNavigatorStyleSettings settings = Owner as SchedulerDateNavigatorStyleSettings;
			if (settings == null || schedulerVisibleInterval.Duration < TimeSpan.FromDays(1))
				return base.ValidateNavigatorInterval(schedulerVisibleInterval);
			TimeInterval visibleInterval = settings.CalculateVisibleInterval();
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerDateNavigatorController.ValidateNavigatorInterval: schedulerInterval: {0}, calendarInterval={1}", schedulerVisibleInterval, visibleInterval);
			bool updateAppointmentDateMap = false;
			schedulerVisibleInterval = new TimeInterval(schedulerVisibleInterval.Start, schedulerVisibleInterval.End.AddDays(-1));
			if (!visibleInterval.Contains(schedulerVisibleInterval)) {
				Owner.StartDate = Owner.GetFirstDayOfMonth(schedulerVisibleInterval.Start);
				updateAppointmentDateMap = true;
			}
			return updateAppointmentDateMap;
		}
		protected override bool ValidateSelectionVisiblity() {
			if (Owner.GetSelection().Count <= 0)
				return false;
			SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "->SchedulerNavigationCallbackService.VilidateSelectionInActiveIntervals: start={0}, end={1}", Owner.StartDate, Owner.EndDate);
			SchedulerDateNavigatorStyleSettings settings = Owner as SchedulerDateNavigatorStyleSettings;
			IDateNavigatorControllerOwner navigator = Owner;
			TimeInterval selectedInterval = new TimeInterval(navigator.SelectionStart, navigator.SelectionEnd);
			TimeInterval activeInterval = new TimeInterval(navigator.StartDate, navigator.EndDate);
			if (activeInterval.Contains(selectedInterval))
				return false;
			TimeInterval visibleInterval = settings.CalculateVisibleInterval();
			bool isSelectionInsideVisibleInterval = visibleInterval.Contains(selectedInterval);
			bool isSelectionIntersectedWidthActiveInterval = activeInterval.IntersectsWith(selectedInterval);
			if (isSelectionIntersectedWidthActiveInterval && isSelectionInsideVisibleInterval) {
				SchedulerLogger.Trace(XpfLoggerTraceLevel.DateNavigator, "    ->intersection detected (selection with active interval)!");
				return false;
			}
			Owner.StartDate = selectedInterval.Start;
			return true;
		}
		public virtual void NavigatorVisibleDateRangeChanged(bool isScrolling) {
			if (isScrolling && !this.syncNavigatorLock)
				EnsureSelectionVisible(isScrolling);
			UpdateSpecialDates();
		}
		public virtual void UpdateSpecialDates() {
			if (IsNavigatorVisibleIntervalChanged())
				UpdateAppointmentDatesMap();
		}
		void EnsureSelectionVisible(bool isScrolling) {
			if (IsSelectionInvisible())
				ShiftSelectionToVisibleArea();
		}
		bool IsSelectionInvisible() {
			TimeInterval selectedInterval = new TimeInterval(Owner.SelectionStart, Owner.SelectionEnd);
			TimeInterval activeInterval = new TimeInterval(Owner.StartDate, Owner.EndDate);
			if (activeInterval.Contains(selectedInterval))
				return false;
			TimeInterval visibleInterval = CalculateNavigatorVisibleInterval();
			bool isSelectionInsideVisibleInterval = visibleInterval.Contains(selectedInterval);
			bool isSelectionIntersectedWidthActiveInterval = activeInterval.IntersectsWith(selectedInterval);
			if (isSelectionIntersectedWidthActiveInterval && isSelectionInsideVisibleInterval) {
				return false;
			}
			return true;
		}
		void ShiftSelectionToVisibleArea() {
			int deltaYear = 0;
			int deltaMonth = 0;
			bool isSelectionInPast = Owner.StartDate > Owner.SelectionEnd;
			bool isSelectionInFuture = Owner.EndDate < Owner.SelectionStart;
			if (isSelectionInFuture && !isSelectionInPast) {
				deltaYear = -(Owner.SelectionStart.Year - Owner.StartDate.Year);
				deltaMonth = -(Owner.SelectionStart.Month - Owner.StartDate.Month);
			} else if (!isSelectionInFuture && isSelectionInPast) {
				deltaYear = Owner.StartDate.Year - Owner.SelectionEnd.Year;
				deltaMonth = Owner.StartDate.Month - Owner.SelectionEnd.Month;
			}
			ShiftVisibleIntervals(deltaYear, deltaMonth, 0);
		}
		TimeInterval CalculateNavigatorVisibleInterval() {
			SchedulerDateNavigatorStyleSettings settings = Owner as SchedulerDateNavigatorStyleSettings;
			return settings.CalculateVisibleInterval();
		}
	}
	#endregion
}
