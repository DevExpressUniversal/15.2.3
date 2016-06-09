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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using DevExpress.Utils.Serializing;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
#else
#endif
namespace DevExpress.XtraScheduler {
	public class AlertInfoBase {
		public static bool operator ==(AlertInfoBase a, AlertInfoBase b) {
			if (Object.ReferenceEquals(a, b))
				return true;
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return a.Equals(b);
		}
		public static bool operator !=(AlertInfoBase a, AlertInfoBase b) {
			return !(a == b);
		}
		DateTime dateTime;
		public DateTime DateTime { get { return dateTime; } }
		internal AlertInfoBase() {
		}
		internal AlertInfoBase(DateTime dateTime) {
			this.dateTime = dateTime;
		}
		public virtual AlertInfoBase CloneWithNewTime(DateTime value) {
			return new AlertInfoBase(value);
		}
		public override bool Equals(object obj) {
			AlertInfoBase info = obj as AlertInfoBase;
			if (Object.ReferenceEquals(info, null))
				return false;
			return info.DateTime == DateTime;
		}
		public override int GetHashCode() {
			return DateTime.GetHashCode();
		}
	}
	#region ReminderBase (abstract class)
	public abstract class ReminderBase {
		TimeSpan timeBeforeStart; 
		bool dismissed;
		bool suspended;
		AlertInfoBase alertInfo;
		protected ReminderBase() {
			alertInfo = CreateAlertInfo();
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderBaseTimeBeforeStart")]
#endif
		public virtual TimeSpan TimeBeforeStart {
			get { return timeBeforeStart; }
			set {
				if (timeBeforeStart == value)
					return;
				AlertInfoBase oldAlertInfo = AlertInfo;
				TimeSpan oldTimeBeforeStart = timeBeforeStart;
				SetTimeBeforeStart(value);
				UpdateAlertInfo();
				if (OnContentChanging("TimeBeforeStart", oldTimeBeforeStart, value)) {
					OnContentChanged();
					if (oldAlertInfo != AlertInfo)
						OnRemindTimeChanged();
				} else {
					alertInfo = oldAlertInfo;
					timeBeforeStart = oldTimeBeforeStart;
				}
			}
		}
		public bool Dismissed { get { return dismissed; } }
		internal bool Suspended { get { return suspended; } set { suspended = value; } }
		protected internal AlertInfoBase AlertInfo { get { return alertInfo; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderBaseAlertTime")]
#endif
		public DateTime AlertTime {
			get { return AlertInfo.DateTime; }
			set {
				SetAlertInfo(AlertInfo.CloneWithNewTime(value), true);
			}
		}
		protected virtual DateTime Now {
			get {
#if DEBUGTEST
				return DevExpress.XtraScheduler.Tests.TestEnvironment.GetNowTime();
#else
				return DateTime.Now;
#endif
			}
		}
		#endregion
		#region Events
		#region RemindTimeChanged
		EventHandler onRemindTimeChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderBaseRemindTimeChanged")]
#endif
		public event EventHandler RemindTimeChanged { add { onRemindTimeChanged += value; } remove { onRemindTimeChanged -= value; } }
		protected virtual void RaiseRemindTimeChanged() {
			if (onRemindTimeChanged != null)
				onRemindTimeChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentChanging
		ReminderContentChangingEventHandler onContentChanging;
		internal event ReminderContentChangingEventHandler ContentChanging { add { onContentChanging += value; } remove { onContentChanging -= value; } }
		protected internal virtual bool RaiseContentChanging(string propertyName, object oldValue, object newValue) {
			if (onContentChanging != null) {
				ReminderContentChangingEventArgs e = new ReminderContentChangingEventArgs();
				e.PropertyName = propertyName;
				e.OldValue = oldValue;
				e.NewValue = newValue;
				onContentChanging(this, e);
				return !e.Cancel;
			} else
				return true;
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		internal event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if (onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected virtual AlertInfoBase CreateAlertInfo() {
			return new AlertInfoBase(new DateTime(0));
		}
		protected virtual void Initialize() {
			this.TimeBeforeStart = DateTimeHelper.FifteenMinutesSpan;
		}
		protected virtual bool OnContentChanging(string propertyName, object oldValue, object newValue) {
			return RaiseContentChanging(propertyName, oldValue, newValue);
		}
		protected virtual void OnContentChanged() {
			RaiseContentChanged();
		}
		protected virtual void OnRemindTimeChanged() {
			RaiseRemindTimeChanged();
		}
		protected internal abstract AlertInfoBase CalcFirstReminderInfo(DateTime after);
		public abstract bool CanAlert(DateTime after, int checkInterval);
		protected internal virtual bool HasMultipleReminderTimes { get { return false; } }
		protected internal virtual AlertInfoBase CalcNextReminderInfo(DateTime time) {
			Exceptions.ThrowInternalException();
			return new AlertInfoBase(DateTime.MaxValue);
		}
		internal AlertInfoBase CalcNextAlertInfo(AlertInfoBase afterInfo) {
			XtraSchedulerDebug.Assert(HasMultipleReminderTimes == true);
			DateTime newTime = afterInfo.DateTime + TimeBeforeStart;
			AlertInfoBase reminderInfo = CalcNextReminderInfo(newTime);
			return CalcReminderAlertInfo(reminderInfo);
		}
		internal AlertInfoBase CalcFirstAlertInfo(DateTime after) {
			AlertInfoBase firstReminderInfo = CalcFirstReminderInfo(after);
			AlertInfoBase alertInfo = CalcReminderAlertInfo(firstReminderInfo);
			return alertInfo;
		}
		internal AlertInfoBase CalcReminderAlertInfo(AlertInfoBase reminderInfo) {
			return reminderInfo.CloneWithNewTime(DateTimeHelper.SubTimeSpanWithoutOverfull(reminderInfo.DateTime, TimeBeforeStart));
		}
		protected internal virtual void UpdateAlertInfo() {
			DateTime now = Now;
			alertInfo = CalcFirstAlertInfo(now);
		}
		protected internal virtual void UpdateAlertInfoAndRaiseRemindTimeChangedEvent() {
			AlertInfoBase oldAlertInfo = alertInfo;
			UpdateAlertInfo();
			if (AlertInfo != oldAlertInfo)
				OnRemindTimeChanged();
		}
		protected internal void SetTimeBeforeStart(TimeSpan value) {
			if (timeBeforeStart == value)
				return;
			if (value.Ticks < 0)
				Exceptions.ThrowArgumentException("TimeBeforeStart", value);
			timeBeforeStart = value;
		}
		protected virtual void SetAlertInfo(AlertInfoBase info, bool raiseContentChangeEvents) {
			if (AlertInfo == info)
				return;
			AlertInfoBase oldAlertInfo = AlertInfo;
			alertInfo = info;
			if (!raiseContentChangeEvents) {
				OnRemindTimeChanged();
				return;
			}
			if (OnContentChanging("AlertTime", oldAlertInfo.DateTime, AlertTime)) {
				OnContentChanged();
				OnRemindTimeChanged();
			} else {
				alertInfo = oldAlertInfo;
			}
		}
		public bool Snooze(TimeSpan remindAfter) {
			return Snooze(Now, remindAfter);
		}
		public bool Snooze(DateTime now, TimeSpan remindAfter) {
			if (remindAfter.Ticks < 0)
				Exceptions.ThrowArgumentException("remindAfter", remindAfter);
			AlertInfoBase newAlertInfo = AlertInfo.CloneWithNewTime(now + remindAfter);
			if (OnContentChanging("Snooze", AlertTime, newAlertInfo.DateTime)) {
				suspended = false;
				alertInfo = newAlertInfo;
				OnContentChanged();
				OnRemindTimeChanged();
				return true;
			}
			return false;
		}
		public bool AlertTimeExpired(DateTime currentTime) {
			return AlertTime <= currentTime && !this.suspended;
		}
		public bool Dismiss() {
			if (dismissed)
				return true;
			if (HasMultipleReminderTimes) {
				AlertInfoBase nextReminderInfo = CalcNextReminderInfo(AlertTime + TimeBeforeStart);
				bool patternDismissed = (nextReminderInfo.DateTime == DateTime.MaxValue);
				if (!patternDismissed) {
					suspended = false;
					alertInfo = CalcReminderAlertInfo(nextReminderInfo);
					OnContentChanged();
					RaiseRemindTimeChanged();
					return false;
				} else {
					AlertInfoBase nextAlertInfo = CalcReminderAlertInfo(nextReminderInfo);
					return OnDismissing(nextAlertInfo);
				}
			} else {
				return OnDismissing(AlertInfo);
			}
		}
		bool OnDismissing(AlertInfoBase newAlertInfo) {
			AlertInfoBase oldAlertInfo = alertInfo;
			alertInfo = newAlertInfo;
			dismissed = true;
			int index = OnDismissing();
			if (OnContentChanging("Dismiss", false, true)) {
				suspended = false;
				OnDismissed();
				OnContentChanged();
				RaiseRemindTimeChanged();
				return true;
			}
			alertInfo = oldAlertInfo;
			OnCancelDismiss(index);
			dismissed = false;
			suspended = false;
			return false;
		}
		protected virtual int OnDismissing() {
			return -1;
		}
		protected virtual void OnDismissed() {
		}
		protected virtual void OnCancelDismiss(int index) {
		}
		protected internal virtual void SwitchToTheLatestExpiredAlertTime(DateTime currentTime) {
			XtraSchedulerDebug.Assert(AlertTimeExpired(currentTime));
			XtraSchedulerDebug.Assert(HasMultipleReminderTimes == false);
			return;
		}
		protected internal virtual void EnsureAlertTimeValid() {
		}
#if DEBUG
		public virtual void CheckIntegrity() {
		}
#endif
	}
	#endregion
	#region Reminder
	public class Reminder : ReminderBase {
		Appointment appointment;
		internal Reminder(Appointment appointment)
			: base() {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
			CheckAppointmentType(appointment);
			this.appointment = appointment;
			Initialize();
		}
		internal Reminder(Reminder reminder)
			: base() {
			if (reminder == null)
				Exceptions.ThrowArgumentException("reminder", reminder);
			Appointment appointment = reminder.Appointment;
			System.Diagnostics.Debug.Assert(appointment != null);
			CheckAppointmentType(appointment);
			this.appointment = reminder.Appointment;
			SetTimeBeforeStart(reminder.TimeBeforeStart);
			SetAlertInfo(AlertInfo.CloneWithNewTime((reminder).AlertInfo.DateTime), false);
		}
		#region Properties
		internal DateTime AppointmentStartTime { get { return DateTimeHelper.AddTimeSpanWithoutOverfull(AlertTime, TimeBeforeStart); } }
		internal virtual DateTime AppointmentEndTime { get { return DateTimeHelper.AddTimeSpanWithoutOverfull(AppointmentStartTime, appointment.Duration); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderSubject")]
#endif
		public string Subject { get { return appointment.Subject; } }
		public Appointment Appointment { get { return appointment; } }
		#endregion
		protected virtual void CheckAppointmentType(Appointment appointment) {
			if (appointment.Type == AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("appointment.Type", appointment.Type);
		}
		protected internal override AlertInfoBase CalcFirstReminderInfo(DateTime after) {
			return new AlertInfoBase(appointment.Start);
		}
		public virtual void OnAppointmentChanged() {
			AlertInfoBase oldAlertInfo = AlertInfo;
			UpdateAlertInfo();
			if (oldAlertInfo != AlertInfo)
				OnRemindTimeChanged();
		}
		public override bool CanAlert(DateTime after, int checkInterval) {
			if (after < AlertTime)
				Exceptions.ThrowArgumentException("after", after);
			if (Appointment.Type == AppointmentType.Normal)
				return true;
			if (Appointment.Type == AppointmentType.DeletedOccurrence)
				return false;
			if (Appointment.Type == AppointmentType.ChangedOccurrence)
				return true;
			DateTime appointmentEndTime = AppointmentStartTime + TimeSpan.FromMilliseconds(checkInterval);
			DateTime actualAppointmentEndTime = DateTimeHelper.Max(appointmentEndTime, AppointmentEndTime);
			return after < actualAppointmentEndTime;
		}
		protected override int OnDismissing() {
			return ((IInternalAppointment)appointment).BeginDismissReminder(this);
		}
		protected override void OnDismissed() {
			((IInternalAppointment)appointment).EndDismissReminder(this);
		}
		protected override void OnCancelDismiss(int index) {
			((IInternalAppointment)appointment).CancelDismissReminder(this, index);
		}
		protected internal override void EnsureAlertTimeValid() {
			if (Appointment.Type == AppointmentType.ChangedOccurrence) {
				Appointment pattern = Appointment.RecurrencePattern;
				pattern.Reminders.ForEach(reminder => (reminder).EnsureAlertTimeValid());
			}
		}
#if DEBUG
		public override void CheckIntegrity() {
			base.CheckIntegrity();
			XtraSchedulerDebug.Assert(Appointment.Reminders.Contains(this));
		}
#endif
		internal void UpdateAlertInfo(AlertInfoBase alertInfo) {
			SetAlertInfo(alertInfo, false);
		}
	}
	#endregion
	#region RecurringReminder
	public class RecurringReminder : Reminder {
		protected internal class RecurringAlertInfo : AlertInfoBase {
			int index;
			public RecurringAlertInfo(DateTime dateTime, int index)
				: base(dateTime) {
				this.index = index;
			}
			public int Index { get { return index; } }
			public override AlertInfoBase CloneWithNewTime(DateTime value) {
				return new RecurringAlertInfo(value, Index);
			}
			public override bool Equals(object obj) {
				RecurringAlertInfo info = obj as RecurringAlertInfo;
				if (Object.ReferenceEquals(info, null))
					return false;
				return info.DateTime == DateTime && info.Index == Index;
			}
			public override int GetHashCode() {
				return DateTime.GetHashCode();
			}
		}
		protected internal RecurringReminder(Appointment appointment)
			: base(appointment) {
		}
#if DEBUGTEST
		RecurringReminder(RecurringReminder reminder)
			: base(reminder) {
		}
#endif
		protected internal new RecurringAlertInfo AlertInfo { get { return (RecurringAlertInfo)base.AlertInfo; } }
		public override bool CanAlert(DateTime after, int checkInterval) {
			if (this.AlertOccurrenceIndex < 0)
				return false;
			return base.CanAlert(after, checkInterval);
		}
		protected override AlertInfoBase CreateAlertInfo() {
			return new RecurringAlertInfo(new DateTime(0), 0);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurringReminderAlertOccurrenceIndex")]
#endif
		public int AlertOccurrenceIndex { get { return AlertInfo.Index; } }
		internal RecurringReminder CreateOwnerCopy(Appointment ownerAppointment) {
			RecurringReminder copy = new RecurringReminder(ownerAppointment);
			copy.SetTimeBeforeStart(TimeBeforeStart);
			copy.SetAlertInfo(new RecurringAlertInfo(AlertTime, AlertOccurrenceIndex), false);
			return copy;
		}
		protected override void CheckAppointmentType(Appointment appointment) {
			if (appointment.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("appointment.Type", appointment.Type);
		}
		protected internal override bool HasMultipleReminderTimes { get { return true; } }
		protected internal override AlertInfoBase CalcFirstReminderInfo(DateTime after) {
			return CalcNextReminderInfo(after);
		}
		protected internal override AlertInfoBase CalcNextReminderInfo(DateTime after) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(Appointment.RecurrenceInfo);
			int index;
			DateTime dateTime = calc.FindNextOccurrenceTimeAfter(after, Appointment, out index);
			return new RecurringAlertInfo(dateTime, index);
		}
		protected override int OnDismissing() {
			return -1;
		}
		protected override void OnDismissed() {
		}
		protected internal override void SwitchToTheLatestExpiredAlertTime(DateTime currentTime) {
			XtraSchedulerDebug.Assert(AlertTimeExpired(currentTime));
			AlertInfoBase newAlertInfo = AlertInfo;
			for (; ; ) {
				AlertInfoBase info = CalcNextAlertInfo(newAlertInfo);
				if (info.DateTime > currentTime || info == newAlertInfo)
					break;
				newAlertInfo = info;
			}
			SetAlertInfo(newAlertInfo, false);
		}
		internal void UpdateAlertOccurrenceIndexInternal() {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(Appointment.RecurrenceInfo);
			int index = calc.FindNearestOccurrenceIndex(Appointment, AlertTime + TimeBeforeStart);
			SetAlertInfo(new RecurringAlertInfo(AlertInfo.DateTime, index), false);
		}
		protected internal override void EnsureAlertTimeValid() {
			Appointment exception = Appointment.FindException(AlertInfo.Index);
			if (exception != null)
				this.UpdateAlertInfo();
		}
		internal void MoveToNextAlertInfo() {
			SetAlertInfo(CalcNextAlertInfo(AlertInfo), false);
		}
		internal void UpdateAlertInfo(int indx) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(Appointment.RecurrenceInfo);
			DateTime chainStart = calc.CalcOccurrenceStartTime(indx);
			int occurrenceIndx = calc.FindNearestOccurrenceIndex(Appointment, chainStart);
			DateTime occurrenceStart = calc.CalcOccurrenceStartTime(occurrenceIndx);
			AlertInfoBase alertInfo = new RecurringAlertInfo(occurrenceStart, occurrenceIndx);
			alertInfo = CalcReminderAlertInfo(alertInfo);
			SetAlertInfo(alertInfo, false);
		}
	}
	#endregion
	public class ReminderCollection : NotificationCollection<Reminder> {
		public ReminderCollection()
			: base(DXCollectionUniquenessProviderType.MaximizePerformance) {
		}
		protected internal ReminderCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		internal IList<Reminder> ObtainInnerList() {
			return base.InnerList;
		}
	}
	#region ReminderBaseComparer
	public class ReminderBaseComparer : IComparer<ReminderBase>, IComparer {
		int IComparer.Compare(object x, object y) {
			ReminderBase rX = (ReminderBase)x;
			ReminderBase rY = (ReminderBase)y;
			return CompareCore(rX, rY);
		}
		public int Compare(ReminderBase rX, ReminderBase rY) {
			return CompareCore(rX, rY);
		}
		protected internal int CompareCore(ReminderBase rX, ReminderBase rY) {
			return rX.AlertTime.Ticks.CompareTo(rY.AlertTime.Ticks);
		}
	}
	#endregion
	#region ReminderComparer
	public class ReminderComparer : ReminderBaseComparer, IComparer<Reminder> {
		#region IComparer<Reminder> Members
		public int Compare(Reminder x, Reminder y) {
			return CompareCore(x, y);
		}
		#endregion
	}
	#endregion
	#region ReminderBaseAlertNotification
	public class ReminderBaseAlertNotification {
		ReminderBase reminder;
		bool handled;
		bool ignore;
		public ReminderBaseAlertNotification(ReminderBase reminder) {
			this.reminder = reminder;
		}
		public ReminderBase Reminder { get { return reminder; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderBaseAlertNotificationHandled")]
#endif
		public bool Handled { get { return handled; } set { handled = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ReminderBaseAlertNotificationIgnore")]
#endif
		public bool Ignore { get { return ignore; } set { ignore = value; } }
	}
	#endregion
	#region ReminderAlertNotification
	public class ReminderAlertNotification : ReminderBaseAlertNotification {
		Appointment actualAppointment;
		public ReminderAlertNotification(Reminder reminder, Appointment actualAppointment)
			: base(reminder) {
			this.actualAppointment = actualAppointment;
		}
		public new Reminder Reminder { get { return (Reminder)base.Reminder; } }
		public Appointment ActualAppointment { get { return actualAppointment; } }
	}
	#endregion
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
	public class ReminderBaseAlertNotificationCollection : DXCollectionBase {
		public ReminderBaseAlertNotification this[int index] {
			get { return (ReminderBaseAlertNotification)List[index]; }
		}
		public int Add(ReminderBaseAlertNotification obj) {
			if (obj == null)
				return -1;
			return AddIfNotAlreadyAdded(obj);
		}
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
	public class ReminderAlertNotificationCollection : ReminderBaseAlertNotificationCollection {
		public new ReminderAlertNotification this[int index] {
			get { return (ReminderAlertNotification)List[index]; }
		}
		public int Add(ReminderAlertNotification obj) {
			return base.Add(obj);
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISupportReminders {
		void TriggerAlerts(DateTime currentTime);
	}
	#region ReminderSR
	public static class ReminderSR {
		public const string XmlCollectionName = "Reminders";
		public const string XmlElementName = "Reminder";
		public const string AlertTime = "AlertTime";
		public const string TimeBeforeStart = "TimeBeforeStart";
	}
	#endregion
	#region ReminderBaseCollection
	public class ReminderBaseCollection : NotificationCollection<ReminderBase> {
		public ReminderBaseCollection()
			: base(DXCollectionUniquenessProviderType.MaximizePerformance) {
		}
		protected internal virtual bool FastContains(ReminderBase value) {
			return UniquenessProvider.LookupObjectIndex(value) >= 0;
		}
	}
	#endregion
	#region ReminderReadOnlyCollection
	public class ReminderReadOnlyCollection : ReminderCollection {
		static readonly Reminder[] emptyReminderArray = new Reminder[] { };
		readonly Appointment pattern;
		public ReminderReadOnlyCollection(Appointment pattern) {
			Guard.ArgumentNotNull(pattern, "pattern");
			this.pattern = pattern;
		}
		protected override IList<Reminder> InnerList { get { return this.pattern != null ? this.pattern.Reminders.ObtainInnerList() : null; } }
		internal ReminderCollection SourceCollection { get { return this.pattern.Reminders; } }
		public override Reminder[] ToArray() {
			if (pattern != null)
				return pattern.Reminders.ToArray();
			else
				return emptyReminderArray;
		}
		protected override bool OnClear() {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnInsert(int index, Reminder value) {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnRemove(int index, Reminder value) {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnSet(int index, Reminder oldValue, Reminder newValue) {
			ThrowReadOnlyException();
			return false;
		}
		protected override void RaiseCollectionChanging(CollectionChangingEventArgs<Reminder> e) {
			e.Cancel = true;
		}
		protected override void RaiseCollectionChanged(CollectionChangedEventArgs<Reminder> e) {
		}
		protected internal virtual void ThrowReadOnlyException() {
			throw new Exception("You cannot modify Reminders collection for occurrence or exception appointment. You should modify recurrence pattern instead.");
		}
	}
	#endregion
	public class LeakSafeTimerTickEventRouter : GenericEventListenerWrapper<ReminderEngineBase, Timer> {
		public LeakSafeTimerTickEventRouter(ReminderEngineBase engine, Timer timer)
			: base(engine, timer) {
			Guard.ArgumentNotNull(timer, "timer");
		}
		protected override void SubscribeEvents() {
			SubscribeTimerTickEvent();
		}
		protected override void UnsubscribeEvents() {
			UnsubscribeTimerTickEvent();
		}
		protected virtual void SubscribeTimerTickEvent() {
			if (EventSource != null)
				EventSource.Tick += OnTimerTick;
		}
		protected virtual void UnsubscribeTimerTickEvent() {
			if (EventSource != null)
				EventSource.Tick -= OnTimerTick;
		}
		void OnTimerTick(object sender, EventArgs e) {
			if (IsAlive())
				ListenerInstance.OnTimerTick(sender, e);
			else
				CleanUp();
		}
	}
	public abstract class ReminderEngineBase : IBatchUpdateable, IBatchUpdateHandler, IDisposable {
		#region Fields
		internal const int DefaultCheckInterval = 15000;
		readonly ReminderBaseCollection reminders;
		Timer timer = new Timer();
		LeakSafeTimerTickEventRouter timerEventRouter;
		bool isDisposed;
		bool deferredReminderSort;
		readonly BatchUpdateHelper batchUpdateHelper;
		readonly ReminderBaseCollection deferredRemindersRemove;
		readonly ReminderBaseCollection deferredRemindersAdd;
		bool enabled;
		int checkInterval;
		DateTime lastReminderCheckTime;
		#endregion
		protected ReminderEngineBase() {
			LastReminderCheckTime = DateTime.MinValue;
			this.reminders = new ReminderBaseCollection();
			this.deferredRemindersRemove = new ReminderBaseCollection();
			this.deferredRemindersAdd = new ReminderBaseCollection();
			timerEventRouter = new LeakSafeTimerTickEventRouter(this, timer);
			timer.Enabled = false;
			this.checkInterval = DefaultCheckInterval;
#if !SL
			timer.Interval = DefaultCheckInterval;
#else
			timer.Interval = TimeSpan.FromMilliseconds(DefaultCheckInterval);
#endif
			batchUpdateHelper = new BatchUpdateHelper(this);
			deferredReminderSort = false;
		}
		#region Properties
		protected internal DateTime LastReminderCheckTime { get { return lastReminderCheckTime; } set { lastReminderCheckTime = value; } }
		protected virtual DateTime Now {
			get {
#if DEBUGTEST
				return DevExpress.XtraScheduler.Tests.TestEnvironment.GetNowTime();
#else
				return DateTime.Now;
#endif
			}
		}
		protected internal ReminderBaseCollection Reminders { get { return reminders; } }
		protected internal Timer Timer { get { return timer; } }
		public virtual bool Enabled {
			get { return enabled; }
			set {
				enabled = value;
				timer.Enabled = CheckInterval > 0 ? value : false;
			}
		}
		public int CheckInterval {
			get { return checkInterval; }
			set {
				if (value == 0)
					timer.Enabled = false;
				else {
#if !SL
					timer.Interval = value;
#else
					timer.Interval = TimeSpan.FromMilliseconds(value);
#endif
					timer.Enabled = enabled;
				}
				checkInterval = value;
			}
		}
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredReminderSort = false;
			deferredRemindersRemove.Clear();
			deferredRemindersAdd.Clear();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			ApplyDeferredRemindersRemove();
			deferredRemindersAdd.ForEach(AddReminderCore);
			if (deferredReminderSort)
				SortRemindersCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region Events
		protected abstract void OnReminderAlert(object sender, ReminderBaseEventArgs e);
		protected internal virtual void RaiseReminderAlert(ReminderBaseEventArgs e) {
			int count = e.AlertNotifications.Count;
			if (count <= 0)
				return;
			OnReminderAlert(this, e);
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (timerEventRouter != null) {
					timerEventRouter.CleanUp();
					timerEventRouter = null;
				}
				if (timer != null) {
					timer.Enabled = false;
					timer.Dispose();
					timer = null;
				}
				UnsubscribeAllRemindersEvents();
				reminders.Clear();
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ReminderEngineBase() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void UnsubscribeAllRemindersEvents() {
			reminders.ForEach(UnsubscribeReminderEvents);
		}
		protected internal virtual void ApplyDeferredRemindersRemove() {
			int count = deferredRemindersRemove.Count;
			for (int i = 0; i < count; i++)
				RemoveReminder(deferredRemindersRemove[i]);
		}
		protected internal virtual void SortReminders() {
			if (!IsUpdateLocked)
				SortRemindersCore();
			else
				deferredReminderSort = true;
		}
		protected virtual void SortRemindersCore() {
			reminders.Sort(new ReminderBaseComparer());
			deferredReminderSort = false;
		}
		#region Add / Remove reminder
		public virtual void AddReminder(ReminderBase reminder) {
			if (reminder == null)
				Exceptions.ThrowArgumentException("reminder", reminder);
			if (!OnFilterReminder(reminder))
				return;
			if (!IsUpdateLocked) {
				AddReminderCore(reminder);
				SortReminders();
			} else {
				int index = deferredRemindersRemove.IndexOf(reminder);
				if (index >= 0) {
					deferredRemindersRemove.RemoveAt(index);
				} else {
					if (!reminders.FastContains(reminder)) {
						deferredRemindersAdd.Add(reminder);
						deferredReminderSort = true;
					}
				}
			}
		}
		protected abstract bool OnFilterReminder(ReminderBase reminder);
		internal void AddReminderCore(ReminderBase reminder) {
			XtraSchedulerDebug.Assert(reminders.FastContains(reminder) == false);
			XtraSchedulerDebug.Assert(IsUpdateLocked == false);
			if (!OnFilterReminder(reminder))
				return;
#if DEBUG            
			reminder.CheckIntegrity();
#endif
			reminder.EnsureAlertTimeValid();
			SubscribeReminderEvents(reminder);
			reminders.Add(reminder);
		}
		public virtual void RemoveReminder(ReminderBase reminder) {
			if (!IsUpdateLocked) {
				UnsubscribeReminderEvents(reminder);
				reminders.Remove(reminder);
			} else {
				int index = deferredRemindersAdd.IndexOf(reminder);
				if (index < 0) {
					if (reminders.FastContains(reminder))
						deferredRemindersRemove.Add(reminder);
				} else
					deferredRemindersAdd.RemoveAt(index);
			}
		}
		internal void SubscribeReminderEvents(ReminderBase reminder) {
			reminder.RemindTimeChanged += new EventHandler(OnReminderChanged);
		}
		internal void UnsubscribeReminderEvents(ReminderBase reminder) {
			reminder.RemindTimeChanged -= new EventHandler(OnReminderChanged);
		}
		#endregion
		protected virtual void OnReminderChanged(object sender, EventArgs e) {
			ReminderBase reminder = sender as ReminderBase;
			if (reminder != null) {
				reminder.Suspended = false;
				if (reminder.Dismissed)
					RemoveReminder(reminder);
			}
			SortReminders();
		}
		protected internal virtual void OnTimerTick(object sender, EventArgs e) {
			TriggerAlerts();
		}
		public void TriggerAlerts() {
			TriggerAlerts(Now);
		}
		internal void TriggerAlerts(DateTime currentTime) {
			RaiseRemindersAlerts(currentTime);
			LastReminderCheckTime = currentTime;
		}
		protected virtual ReminderBaseEventArgs CreateRemindersAlertsEventArgs() {
			ReminderBaseAlertNotificationCollection alertNotifications = new ReminderBaseAlertNotificationCollection();
			return new ReminderBaseEventArgs(alertNotifications);
		}
		protected internal virtual void RaiseRemindersAlerts(DateTime currentTime) {
			ReminderBaseEventArgs args = CreateRemindersAlertsEventArgs();
			ProcessReminderAlerts(currentTime, args.AlertNotifications);
			RaiseRemindersAlertsCore(args);
		}
		protected internal virtual void RaiseRemindersAlertsCore(ReminderBaseEventArgs args) {
			RaiseReminderAlert(args);
			DismissNotHandledNotifications(args.AlertNotifications);
		}
		protected internal void ProcessReminderAlerts(DateTime currentTime, ReminderBaseAlertNotificationCollection alertNotifications) {
			BeginUpdate();
			try {
				int count = reminders.Count;
				for (int i = 0; i < count; i++) {
					ReminderBase reminder = reminders[i];
					ProcessReminderAlert(alertNotifications, reminder, currentTime);
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void ProcessReminderAlert(ReminderBaseAlertNotificationCollection alertNotifications, ReminderBase reminder, DateTime currentTime) {
			if (!reminder.AlertTimeExpired(currentTime))
				return;
			BeginUpdate();
			try {
				reminder.SwitchToTheLatestExpiredAlertTime(currentTime);
				reminder.Suspended = true;
				if (reminder.CanAlert(currentTime, CheckInterval))
					alertNotifications.Add(CreateAlertNotification(reminder));
				else
					reminder.Dismiss();
			} finally {
				EndUpdate();
			}
		}
		protected virtual ReminderBaseAlertNotification CreateAlertNotification(ReminderBase reminder) {
			return new ReminderBaseAlertNotification(reminder);
		}
		protected internal virtual void DismissNotHandledNotifications(ReminderBaseAlertNotificationCollection alertNotifications) {
			BeginUpdate();
			try {
				int count = alertNotifications.Count;
				for (int i = 0; i < count; i++) {
					ReminderBaseAlertNotification alertNotification = alertNotifications[i];
					if (!alertNotification.Handled)
						alertNotification.Reminder.Dismiss();
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ReminderEngine : ReminderEngineBase {
		public ReminderEngine() {
		}
		ReminderEventHandler onReminderAlert;
		public event ReminderEventHandler ReminderAlert { add { onReminderAlert += value; } remove { onReminderAlert -= value; } }
		protected override void OnReminderAlert(object sender, ReminderBaseEventArgs e) {
			if (onReminderAlert != null)
				onReminderAlert(this, (ReminderEventArgs)e);
		}
		EventHandler<ReminderCancelEventArgs> onFilterReminder;
		public event EventHandler<ReminderCancelEventArgs> FilterReminder { add { onFilterReminder += value; } remove { onFilterReminder -= value; } }
		protected bool RaiseFilterReminder(ReminderCancelEventArgs ea) {
			if (onFilterReminder == null)
				return true;
			onFilterReminder(this, ea);
			return !ea.Cancel;
		}
		protected override ReminderBaseEventArgs CreateRemindersAlertsEventArgs() {
			ReminderAlertNotificationCollection alertNotifications = new ReminderAlertNotificationCollection();
			return new ReminderEventArgs(alertNotifications);
		}
		protected override bool OnFilterReminder(ReminderBase baseReminder) {
			Reminder reminder = baseReminder as Reminder;
			if (reminder == null)
				return true;
			ReminderCancelEventArgs ea = new ReminderCancelEventArgs(reminder);
			return RaiseFilterReminder(ea);
		}
		protected internal virtual void OnAppointmentCollectionLoaded(AppointmentBaseCollection appointments) {
			UnsubscribeAllRemindersEvents();
			Reminders.Clear();
			AddAppointmentsReminders(appointments);
		}
		protected internal virtual void AddAppointmentsReminders(AppointmentBaseCollection appointments) {
			ForEachInGroupOperation(appointments, AddAppointmentReminders);
		}
		protected internal virtual void AddAppointmentReminders(Appointment apt) {
			if (apt.Type == AppointmentType.DeletedOccurrence || apt.Type == AppointmentType.DeletedOccurrence)
				return;
			if (apt.Reminders != null)
				apt.Reminders.ForEach(AddReminder);
			if (apt.HasExceptions)
				AddAppointmentsReminders(((IInternalAppointment)apt).PatternExceptions);
		}
		protected internal virtual void OnAppointmentCollectionCleared() {
			UnsubscribeAllRemindersEvents();
			Reminders.Clear();
		}
		protected override ReminderBaseAlertNotification CreateAlertNotification(ReminderBase baseReminder) {
			Reminder reminder = (Reminder)baseReminder;
			if (reminder.Appointment.Type != AppointmentType.Pattern)
				return new ReminderAlertNotification(reminder, reminder.Appointment);
			else {
				RecurringReminder recurringReminder = (RecurringReminder)reminder;
				Appointment apt = reminder.Appointment.GetOccurrence(recurringReminder.AlertInfo.Index);
				return new ReminderAlertNotification(reminder, apt);
			}
		}
		protected internal virtual void ForEachInGroupOperation(AppointmentBaseCollection appointments, Action<Appointment> handler) {
			BeginUpdate();
			try {
				appointments.ForEach(handler);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnAppointmentsInserted(AppointmentBaseCollection appointments) {
			ForEachInGroupOperation(appointments, OnAppointmentInserted);
		}
		protected internal virtual void OnAppointmentsChanged(AppointmentBaseCollection appointments) {
			ForEachInGroupOperation(appointments, OnAppointmentChanged);
		}
		protected internal virtual void OnAppointmentsDeleted(AppointmentBaseCollection appointments) {
			ForEachInGroupOperation(appointments, OnAppointmentDeleted);
		}
		protected internal virtual void OnAppointmentsDeferredDeleted(AppointmentBaseCollection appointments) {
			ForEachInGroupOperation(appointments, OnAppointmentDeferredDeleted);
		}
		protected internal virtual void OnAppointmentInserted(Appointment apt) {
			if (apt.Type == AppointmentType.Occurrence || apt.Reminders == null)
				return;
			if (apt.Type == AppointmentType.ChangedOccurrence)
				OnInsertChangedOccurrence(apt);
			else if (apt.Type == AppointmentType.DeletedOccurrence)
				OnInsertDeletedOccurrence(apt);
			else
				apt.Reminders.ForEach(AddReminder);
		}
		protected internal virtual void OnInsertDeletedOccurrence(Appointment deletedOccurrence) {
			ReminderCollection reminders = deletedOccurrence.RecurrencePattern.Reminders;
			if (reminders == null)
				return;
			int count = reminders.Count;
			for (int i = 0; i < count; i++)
				(reminders[i]).OnAppointmentChanged();
		}
		protected internal void OnInsertChangedOccurrence(Appointment changedOccurrence) {
			Appointment pattern = changedOccurrence.RecurrencePattern;
			ReminderCollection recurrenceReminders = pattern.Reminders;
			if (recurrenceReminders == null || recurrenceReminders.Count == 0)
				return;
			int count = Reminders.Count;
			for (int i = 0; i < count; i++) {
				RecurringReminder recurringReminder = Reminders[i] as RecurringReminder;
				if (recurringReminder == null)
					continue;
				if (recurringReminder.Appointment == pattern && recurringReminder.AlertOccurrenceIndex == changedOccurrence.RecurrenceIndex)
					recurringReminder.MoveToNextAlertInfo();
			}
			if (changedOccurrence.Start + pattern.Duration < Now) {
				changedOccurrence.Reminders.Clear();
				return;
			}
			changedOccurrence.Reminders.ForEach(AddReminder);
			SortReminders();
		}
		protected internal virtual void OnAppointmentChanged(Appointment apt) {
			RemoveReminders(apt);
			if (apt.Type == AppointmentType.DeletedOccurrence || apt.Type == AppointmentType.Occurrence)
				return;
			if (apt.Type == AppointmentType.ChangedOccurrence && apt.Start + apt.Duration < Now) {
				apt.Reminders.Clear();
				return;
			}
			if (apt.HasReminder)
				apt.Reminders.ForEach(AddReminder);
		}
		protected internal virtual void OnAppointmentDeleted(Appointment apt) {
			if (apt.Type == AppointmentType.DeletedOccurrence || apt.Type == AppointmentType.Occurrence) {
				DeleteRecurrenceReminder(apt);
				return;
			}
			RemoveReminders(apt);
		}
		protected internal virtual void OnAppointmentDeferredDeleted(Appointment apt) {
			if (apt.Type == AppointmentType.Occurrence || apt.Type == AppointmentType.DeletedOccurrence)
				DeleteRecurrenceReminder(apt);
			if (apt.Type != AppointmentType.Occurrence)
				RemoveReminders(apt);
		}
		void DeleteRecurrenceReminder(Appointment apt) {
			Appointment pattern = apt.RecurrencePattern;
			if (pattern.Type != AppointmentType.Pattern)
				return;
			int deletedOccurrenceIndx = apt.RecurrenceIndex;
			ReminderCollection reminders = pattern.Reminders;
			int count = reminders.Count;
			for (int i = 0; i < count; i++) {
				RecurringReminder patternReminder = reminders[i] as RecurringReminder;
				UpdateDeletedReminder(apt, patternReminder, deletedOccurrenceIndx);
			}
			reminders.ForEach(AddReminder);
		}
		void UpdateDeletedReminder(Appointment apt, RecurringReminder reminder, int deletedOccurrenceIndx) {
			int currentIndx = reminder.AlertInfo.Index;
			reminder.UpdateAlertInfo();
			int newIndx = reminder.AlertInfo.Index;
			if (newIndx == currentIndx)
				return;
			int newActualIndx = CalculateNextRecurrentReminderIndex(newIndx, apt.RecurrencePattern, reminder);
			reminder.UpdateAlertInfo(newActualIndx);
		}
		int CalculateNextRecurrentReminderIndex(int newIndx, Appointment pattern, RecurringReminder reminder) {
			Appointment occurrence = pattern.GetOccurrence(newIndx);
			if (occurrence == null)
				return newIndx;
			if (LastReminderCheckTime > occurrence.Start - reminder.TimeBeforeStart)
				newIndx++;
			return newIndx;
		}
		protected internal virtual void RemoveReminders(Appointment apt) {
			int count = Reminders.Count;
			for (int i = count - 1; i >= 0; i--) {
				Reminder reminder = Reminders[i] as Reminder;
				if (reminder != null && reminder.Appointment == apt)
					RemoveReminder(reminder);
			}
		}
		protected internal virtual void RaiseRemindersAlerts(ReminderCollection reminders) {
			ReminderBaseEventArgs args = CreateRemindersAlertsEventArgs();
			FillAlertNotifications(args.AlertNotifications, reminders);
			RaiseRemindersAlertsCore(args);
		}
		internal void FillAlertNotifications(ReminderBaseAlertNotificationCollection alertNotifications, ReminderCollection reminders) {
			int count = reminders.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = reminders[i];
				ReminderBaseAlertNotification alertNotification = CreateAlertNotification(reminder);
				alertNotifications.Add(alertNotification);
			}
		}
		public void Suspend() {
			Timer.Enabled = false;
		}
		public void Resume() {
			Timer.Enabled = Enabled;
		}
	}
	#region ReminderCollectionChangedListener
	public class ReminderCollectionChangedListener : NotificationCollectionChangedListenerBase<Reminder> {
		public ReminderCollectionChangedListener(ReminderCollection collection)
			: base(collection) {
		}
		#region Events
		ReminderContentChangingEventHandler onReminderContentChanging;
		internal event ReminderContentChangingEventHandler ReminderContentChanging { add { onReminderContentChanging += value; } remove { onReminderContentChanging -= value; } }
		protected internal virtual void RaiseReminderContentChanging(object sender, ReminderContentChangingEventArgs args) {
			if (onReminderContentChanging != null) {
				onReminderContentChanging(sender, args);
			}
		}
		#endregion
		#region SubscribeObjectEvents
		protected override void SubscribeObjectEvents(Reminder reminder) {
			reminder.ContentChanging += new ReminderContentChangingEventHandler(OnReminderContentChanging);
			reminder.ContentChanged += new EventHandler(OnReminderContentChanged);
		}
		#endregion
		#region UnsubscribeObjectEvents
		protected override void UnsubscribeObjectEvents(Reminder reminder) {
			reminder.ContentChanging -= new ReminderContentChangingEventHandler(OnReminderContentChanging);
			reminder.ContentChanged -= new EventHandler(OnReminderContentChanged);
		}
		#endregion
		protected internal virtual void OnReminderContentChanging(object sender, ReminderContentChangingEventArgs e) {
			RaiseReminderContentChanging(sender, e);
		}
		protected internal virtual void OnReminderContentChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
	#endregion
	public class OccurrenceReminderCollectionChangedListener : ReminderCollectionChangedListener {
		public OccurrenceReminderCollectionChangedListener(ReminderCollection collection)
			: base(collection) {
		}
		protected internal override void OnReminderContentChanging(object sender, ReminderContentChangingEventArgs e) {
		}
		protected internal override void OnReminderContentChanged(object sender, EventArgs e) {
		}
		protected override void SubscribeCollectionEvents() {
		}
		protected override void UnsubscribeCollectionEvents() {
		}
		protected override void SubscribeObjectEvents(Reminder reminder) {
		}
		protected override void UnsubscribeObjectEvents(Reminder reminder) {
		}
	}
	public static class ReminderHelper {
		public static string ObtainAppointmentTimeZoneId(Reminder reminder) {
			Appointment apt = reminder.Appointment;
			if (apt == null)
				return null;
			return apt.TimeZoneId;
		}
	}
}
namespace DevExpress.XtraScheduler.Xml {
	public class ReminderContextElement : XmlContextItem {
		public ReminderContextElement(Reminder reminder)
			: this(reminder, new TimeZoneEngine()) {
		}
		internal ReminderContextElement(Reminder reminder, TimeZoneEngine timeZoneEngine)
			: base(ReminderSR.XmlElementName, reminder, null) {
			TimeZoneEngine = timeZoneEngine;
		}
		protected internal Reminder Reminder { get { return (Reminder)Value; } }
		TimeZoneEngine TimeZoneEngine { get; set; }
		public override string ValueToString() {
			return new ReminderXmlPersistenceHelper(Reminder, TimeZoneEngine).ToXml();
		}
	}
	public class ReminderXmlPersistenceHelper : XmlPersistenceHelper {
		Reminder reminder;
		public ReminderXmlPersistenceHelper(Reminder reminder)
			: this(reminder, new TimeZoneEngine()) {
		}
		internal ReminderXmlPersistenceHelper(Reminder reminder, TimeZoneEngine timeZoneEngine) {
			this.reminder = reminder;
			TimeZoneEngine = timeZoneEngine;
		}
		protected internal Reminder Reminder { get { return reminder; } }
		TimeZoneEngine TimeZoneEngine { get; set; }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(ReminderSR.XmlElementName);
			DateTime alertTime = Reminder.AlertTime;
			if (TimeZoneEngine != null) {
				string appointmentTimeZoneId = ReminderHelper.ObtainAppointmentTimeZoneId(this.reminder);
				if (String.IsNullOrEmpty(appointmentTimeZoneId))
					alertTime = TimeZoneEngine.FromOperationTimeToDefaultAppointmentTimeZone(alertTime);
				else
					alertTime = TimeZoneEngine.FromOperationTime(alertTime, appointmentTimeZoneId);
			}
			context.Attributes.Add(new DateTimeContextAttribute(ReminderSR.AlertTime, alertTime, DateTime.MinValue));
			context.Attributes.Add(new TimeSpanContextAttribute(ReminderSR.TimeBeforeStart, Reminder.TimeBeforeStart, DateTimeHelper.FifteenMinutesSpan));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			ReminderXmlLoader xmlLoader = new ReminderXmlLoader(root, reminder, TimeZoneEngine);
			return xmlLoader;
		}
		public static Reminder ObjectFromXml(Reminder reminder, string xml) {
			return ObjectFromXml(reminder, GetRootElement(xml), new TimeZoneEngine());
		}
		public static Reminder ObjectFromXml(Reminder reminder, XmlNode root) {
			return ObjectFromXml(reminder, root, new TimeZoneEngine());
		}
		internal static Reminder ObjectFromXml(Reminder reminder, string xml, TimeZoneEngine timeZoneEngine) {
			return ObjectFromXml(reminder, GetRootElement(xml), timeZoneEngine);
		}
		internal static Reminder ObjectFromXml(Reminder reminder, XmlNode root, TimeZoneEngine timeZoneEngine) {
			ReminderXmlPersistenceHelper helper = new ReminderXmlPersistenceHelper(reminder, timeZoneEngine);
			return (Reminder)helper.FromXmlNode(root);
		}		
	}
	public class ReminderXmlLoader : ObjectXmlLoader {
		Reminder reminder;
		public ReminderXmlLoader(XmlNode root, Reminder reminder)
			: this(root, reminder, new TimeZoneEngine()) {
		}
		internal ReminderXmlLoader(XmlNode root, Reminder reminder, TimeZoneEngine tze)
			: base(root) {
			Guard.ArgumentNotNull(reminder, "reminder");
			this.reminder = reminder;
			TimeZoneEngine = tze;
		}
		TimeZoneEngine TimeZoneEngine { get; set; }
		public override object ObjectFromXml() {
			DateTime alertTime = ReadAttributeAsDateTime(ReminderSR.AlertTime, DateTime.MinValue);
			if (TimeZoneEngine != null) {
				string appointmentTimeZoneId = ReminderHelper.ObtainAppointmentTimeZoneId(this.reminder);
				if (String.IsNullOrEmpty(appointmentTimeZoneId))
					alertTime = TimeZoneEngine.ToOperationTimeFromDefaultAppointmentTimeZone(alertTime);
				else
					alertTime = TimeZoneEngine.ToOperationTime(alertTime, appointmentTimeZoneId);
			}
			this.reminder.AlertTime = alertTime;
			(this.reminder).SetTimeBeforeStart(ReadAttributeAsTimeSpan(ReminderSR.TimeBeforeStart, DateTimeHelper.FifteenMinutesSpan));
			RecurringReminder recurringReminder = reminder as RecurringReminder;
			if (recurringReminder != null) {
				recurringReminder.UpdateAlertOccurrenceIndexInternal();
				if (recurringReminder.AlertOccurrenceIndex < 0)
					return null;
			}
			return reminder;
		}		
	}
	public class ReminderCollectionContextElement : XmlContextItem {
		public ReminderCollectionContextElement(Appointment apt)
			: base(ReminderSR.XmlCollectionName, apt, null) {
		}
		protected Appointment Appointment { get { return (Appointment)Value; } }
		protected ReminderCollection Reminders { get { return Appointment.Reminders; } }
		public override string ValueToString() {
			return ReminderCollectionXmlPersistenceHelper.CreateSaveInstance(Appointment).ToXml();
		}
	}
	public class ReminderCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		static readonly ReminderCollection emptyReminderCollection = new ReminderCollection();
		readonly Appointment apt;
		protected ReminderCollectionXmlPersistenceHelper(Appointment apt, ReminderCollection reminders)
			: this(apt, reminders, InternalAppointmentPropertyAccessor.GetTimeZoneEngine(apt)) {
		}
		protected ReminderCollectionXmlPersistenceHelper(Appointment apt, ReminderCollection reminders, TimeZoneEngine timeZoneEngine)
			: base(reminders) {
			this.apt = apt;
			TimeZoneEngine = timeZoneEngine;
		}
		protected override string XmlCollectionName { get { return ReminderSR.XmlCollectionName; } }
		protected internal ReminderCollection Reminders { get { return apt.Reminders; } }
		TimeZoneEngine TimeZoneEngine { get; set; }
		static ReminderCollectionXmlPersistenceHelper CreateLoadInstance(Appointment apt, TimeZoneEngine timeZoneEngine) {
			return new ReminderCollectionXmlPersistenceHelper(apt, null, timeZoneEngine);
		}
		public static ReminderCollectionXmlPersistenceHelper CreateSaveInstance(Appointment apt) {
			TimeZoneEngine tze = InternalAppointmentPropertyAccessor.GetTimeZoneEngine(apt);
			return CreateSaveInstance(apt, tze ?? new TimeZoneEngine());
		}
		internal static ReminderCollectionXmlPersistenceHelper CreateSaveInstance(Appointment apt, TimeZoneEngine timeZoneEngine) {
			return new ReminderCollectionXmlPersistenceHelper(apt, GetAppointmentRemindersForSaving(apt), timeZoneEngine);
		}
		static ReminderCollection GetAppointmentRemindersForSaving(Appointment apt) {
			return apt.HasReminder ? apt.Reminders : emptyReminderCollection;
		}
		public static ReminderCollection ObjectFromXml(Appointment apt, string xml) {
			return ObjectFromXml(apt, GetRootElement(xml), new TimeZoneEngine());
		}
		public static ReminderCollection ObjectFromXml(Appointment apt, XmlNode root) {
			return ObjectFromXml(apt, root, new TimeZoneEngine());
		}
		internal static ReminderCollection ObjectFromXml(Appointment apt, string xml, TimeZoneEngine timeZoneEngine) {
			return ObjectFromXml(apt, GetRootElement(xml), timeZoneEngine);
		}
		internal static ReminderCollection ObjectFromXml(Appointment apt, XmlNode root, TimeZoneEngine timeZoneEngine) {
			ReminderCollectionXmlPersistenceHelper helper = ReminderCollectionXmlPersistenceHelper.CreateLoadInstance(apt, timeZoneEngine);
			return (ReminderCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return new ReminderCollectionXmlLoader(root, apt, TimeZoneEngine);
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new ReminderContextElement((Reminder)obj, TimeZoneEngine);
		}
	}
	public class ReminderCollectionXmlLoader : ObjectCollectionXmlLoader {
		readonly Appointment apt;
		public ReminderCollectionXmlLoader(XmlNode root, Appointment apt)
			: this(root, apt, new TimeZoneEngine()) {
		}
		internal ReminderCollectionXmlLoader(XmlNode root, Appointment apt, TimeZoneEngine timeZoneEngine)
			: base(root) {
			Guard.ArgumentNotNull(apt, "apt");
			this.apt = apt;
			TimeZoneEngine = timeZoneEngine;
		}
		protected override ICollection Collection { get { return apt.Reminders; } }
		protected override string XmlCollectionName { get { return ReminderSR.XmlCollectionName; } }
		TimeZoneEngine TimeZoneEngine { get; set; }
		protected override object LoadObject(XmlNode root) {
			Reminder r = apt.CreateNewReminder();
			return ReminderXmlPersistenceHelper.ObjectFromXml(r, root, TimeZoneEngine);
		}
		protected override void AddObjectToCollection(object obj) {
			apt.Reminders.Add((Reminder)obj);
		}
		protected override void ClearCollectionObjects() {
			apt.HasReminder = false;
		}
	}
	#region AppointmentReminderInfo
	public class AppointmentReminderInfo {
		ReminderInfoCollection reminderInfos = new ReminderInfoCollection();
		public ReminderInfoCollection ReminderInfos { get { return reminderInfos; } }
		public void FromXml(string val) {
			if (String.IsNullOrEmpty(val))
				return;
			ReminderInfoCollectionXmlPersistenceHelper.ObjectFromXml(ReminderInfos, val);
		}
		public string ToXml() {
			if (reminderInfos.Count == 0)
				return string.Empty;
			ReminderInfoCollectionXmlPersistenceHelper helper = new ReminderInfoCollectionXmlPersistenceHelper(ReminderInfos);
			return helper.ToXml();
		}
	}
	public class ReminderInfo {
		DateTime alertTime = DateTime.MinValue;
		TimeSpan timeBeforeStart = TimeSpan.Zero;
		public DateTime AlertTime { get { return alertTime; } set { alertTime = value; } }
		public TimeSpan TimeBeforeStart { get { return timeBeforeStart; } set { timeBeforeStart = value; } }
		internal string TimeZoneId { get; set; }
	}
	public class ReminderInfoCollection : NotificationCollection<ReminderInfo> {
	}
	public class ReminderInfoContextElement : XmlContextItem {
		public ReminderInfoContextElement(ReminderInfo reminderInfo)
			: base(ReminderSR.XmlElementName, reminderInfo, null) {
		}
		protected internal ReminderInfo ReminderInfo { get { return (ReminderInfo)Value; } }
		public override string ValueToString() {
			return new ReminderInfoXmlPersistenceHelper(ReminderInfo).ToXml();
		}
	}
	public class ReminderInfoXmlPersistenceHelper : XmlPersistenceHelper {
		ReminderInfo reminderInfo;
		public ReminderInfoXmlPersistenceHelper(ReminderInfo reminderInfo) {
			this.reminderInfo = reminderInfo;
		}
		protected internal ReminderInfo ReminderInfo { get { return reminderInfo; } }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(ReminderSR.XmlElementName);
			DateTime alertTime = ReminderInfo.AlertTime;
			context.Attributes.Add(new DateTimeContextAttribute(ReminderSR.AlertTime, alertTime, DateTime.MinValue));
			context.Attributes.Add(new TimeSpanContextAttribute(ReminderSR.TimeBeforeStart, ReminderInfo.TimeBeforeStart, DateTimeHelper.FifteenMinutesSpan));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new ReminderInfoXmlLoader(root, ReminderInfo);
		}
		public static ReminderInfo ObjectFromXml(ReminderInfo reminderInfo, string xml) {
			return ObjectFromXml(reminderInfo, GetRootElement(xml));
		}
		public static ReminderInfo ObjectFromXml(ReminderInfo reminderInfo, XmlNode root) {
			ReminderInfoXmlPersistenceHelper helper = new ReminderInfoXmlPersistenceHelper(reminderInfo);
			return (ReminderInfo)helper.FromXmlNode(root);
		}
	}
	public class ReminderInfoXmlLoader : ObjectXmlLoader {
		ReminderInfo reminderInfo;
		public ReminderInfoXmlLoader(XmlNode root, ReminderInfo reminderInfo)
			: base(root) {
			Guard.ArgumentNotNull(reminderInfo, "reminderInfo");
			this.reminderInfo = reminderInfo;
		}
		protected internal ReminderInfo ReminderInfo { get { return reminderInfo; } }
		public override object ObjectFromXml() {
			DateTime alertTime = ReadAttributeAsDateTime(ReminderSR.AlertTime, DateTime.MinValue);
			ReminderInfo.AlertTime = alertTime;
			TimeSpan timeBeforeStart = ReadAttributeAsTimeSpan(ReminderSR.TimeBeforeStart, DateTimeHelper.FifteenMinutesSpan);
			ReminderInfo.TimeBeforeStart = timeBeforeStart;
			return ReminderInfo;
		}
	}
	public class ReminderInfoCollectionContextElement : XmlContextItem {
		public ReminderInfoCollectionContextElement(ReminderInfoCollection reminderInfos)
			: base(ReminderSR.XmlCollectionName, reminderInfos, null) {
		}
		protected ReminderInfoCollection ReminderInfos { get { return (ReminderInfoCollection)Value; } }
		public override string ValueToString() {
			return new ReminderInfoCollectionXmlPersistenceHelper(ReminderInfos).ToXml();
		}
	}
	public class ReminderInfoCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		public ReminderInfoCollectionXmlPersistenceHelper(ReminderInfoCollection reminderInfos)
			: base(reminderInfos) {
		}
		protected override string XmlCollectionName { get { return ReminderSR.XmlCollectionName; } }
		protected internal ReminderInfoCollection ReminderInfos { get { return (ReminderInfoCollection)base.Collection; } }
		public static ReminderInfoCollection ObjectFromXml(ReminderInfoCollection reminderInfos, string xml) {
			return ObjectFromXml(reminderInfos, GetRootElement(xml));
		}
		public static ReminderInfoCollection ObjectFromXml(ReminderInfoCollection reminderInfos, XmlNode root) {
			ReminderInfoCollectionXmlPersistenceHelper helper = new ReminderInfoCollectionXmlPersistenceHelper(reminderInfos);
			return (ReminderInfoCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return new ReminderInfoCollectionXmlLoader(root, ReminderInfos);
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new ReminderInfoContextElement((ReminderInfo)obj);
		}
	}
	public class ReminderInfoCollectionXmlLoader : ObjectCollectionXmlLoader {
		ReminderInfoCollection reminderInfos;
		public ReminderInfoCollectionXmlLoader(XmlNode root, ReminderInfoCollection reminderInfos)
			: base(root) {
			Guard.ArgumentNotNull(reminderInfos, "reminderInfos");
			this.reminderInfos = reminderInfos;
		}
		protected override ICollection Collection { get { return reminderInfos; } }
		protected override string XmlCollectionName { get { return ReminderSR.XmlCollectionName; } }
		protected override object LoadObject(XmlNode root) {
			ReminderInfo reminderInfo = new ReminderInfo();
			return ReminderInfoXmlPersistenceHelper.ObjectFromXml(reminderInfo, root);
		}
		protected override void AddObjectToCollection(object obj) {
			reminderInfos.Add((ReminderInfo)obj);
		}
		protected override void ClearCollectionObjects() {
			reminderInfos.Clear();
		}
	}
	#endregion
}
