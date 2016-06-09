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
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	public interface IAppointmentBase : INotifyPropertyChanged {
		DateTime Start { get; set; }
		DateTime End { get; set; }
		TimeSpan Duration { get; set; }
		bool AllDay { get; set; }
		string Subject { get; set; }
		string Description { get; set; }
		string Location { get; set; }
		object LabelId { get; set; }
		object StatusId { get; set; }
		string TimeZoneId { get; set; }
		event CancellablePropertyChangingEventHandler PropertyChanging;
	}
	internal interface IInternalAppointmentBase {
		TimeInterval Interval { get; }
	}
	#region DefaultIAppointmentBase (abstract class)
	public abstract class DefaultIAppointmentBase : IAppointmentBase {
		public abstract DateTime Start { get; set; }
		public abstract DateTime End { get; set; }
		public abstract TimeSpan Duration { get; set; }
		public abstract bool AllDay { get; set; }
		public abstract string Subject { get; set; }
		public abstract string Description { get; set; }
		public abstract string Location { get; set; }
		public abstract object LabelId { get; set; }
		public abstract object StatusId { get; set; }
		public abstract string TimeZoneId { get; set; }
		#region Events
		#region PropertyChanging
		CancellablePropertyChangingEventHandler onPropertyChanging;
		public event CancellablePropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanging != null) {
				CancellablePropertyChangingEventArgs args = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
				onPropertyChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		protected internal virtual bool OnChanging<T>(string propertyName, T oldValue, T newValue) {
			return RaisePropertyChanging(propertyName, oldValue, newValue);
		}
		protected internal virtual void OnChanged(string propertyName) {
			RaisePropertyChanged(propertyName);
		}
	}
	#endregion
	public class DefaultIAppointment : DefaultIAppointmentBase, IInternalAppointmentBase {
		readonly TimeInterval interval;
		string subject = String.Empty;
		string description = String.Empty;
		string location = String.Empty;
		object labelId;
		object statusId;
		public DefaultIAppointment() {
			this.interval = new TimeInterval();
		}
		public TimeInterval Interval { get { return interval; } }
		#region Start
		public override DateTime Start {
			get { return Interval.Start; }
			set {
				if (Interval.Start == value)
					return;
				DateTime oldValue = Interval.Start;
				Interval.Start = value;
				if (OnChanging("Start", oldValue, value))
					OnChanged("Start");
				else
					Interval.Start = oldValue;
			}
		}
		#endregion
		#region End
		public override DateTime End {
			get { return Interval.End; }
			set {
				if (Interval.End == value)
					return;
				DateTime oldValue = Interval.End;
				Interval.End = value;
				if (OnChanging("End", oldValue, value))
					OnChanged("End");
				else
					Interval.End = oldValue;
			}
		}
		#endregion
		#region Duration
		public override TimeSpan Duration {
			get { return Interval.Duration; }
			set {
				if (Interval.Duration == value)
					return;
				TimeSpan oldValue = Interval.Duration;
				Interval.Duration = value;
				if (OnChanging("Duration", oldValue, value))
					OnChanged("Duration");
				else
					Interval.Duration = oldValue;
			}
		}
		#endregion
		#region AllDay
		public override bool AllDay {
			get { return Interval.AllDay; }
			set {
				if (Interval.AllDay == value)
					return;
				bool oldValue = Interval.AllDay;
				Interval.AllDay = value;
				if (OnChanging("AllDay", oldValue, value))
					OnChanged("AllDay");
				else
					Interval.AllDay = oldValue;
			}
		}
		#endregion
		#region Subject
		public override string Subject {
			get { return subject; }
			set {
				if (value == null)
					value = String.Empty;
				if (subject == value)
					return;
				string oldValue = subject;
				subject = value;
				if (OnChanging("Subject", oldValue, value))
					OnChanged("Subject");
				else
					subject = oldValue;
			}
		}
		#endregion
		#region Description
		public override string Description {
			get { return description; }
			set {
				if (value == null)
					value = String.Empty;
				if (description == value)
					return;
				string oldValue = description;
				description = value;
				if (OnChanging("Description", oldValue, value))
					OnChanged("Description");
				else
					description = oldValue;
			}
		}
		#endregion
		#region Location
		public override string Location {
			get { return location; }
			set {
				if (value == null)
					value = String.Empty;
				if (location == value)
					return;
				string oldValue = location;
				location = value;
				if (OnChanging("Location", oldValue, value))
					OnChanged("Location");
				else
					location = oldValue;
			}
		}
		#endregion
		#region LabelId
		public override object LabelId {
			get { return labelId; }
			set {
				if (Object.Equals(labelId,value))
					return;
				object oldValue = labelId;
				labelId = value;
				if (OnChanging("LabelId", oldValue, value))
					OnChanged("LabelId");
				else
					labelId = oldValue;
			}
		}
		#endregion
		#region StatusId
		public override object StatusId {
			get { return statusId; }
			set {
				if (Object.Equals(statusId, value))
					return;
				object oldValue = statusId;
				statusId = value;
				if (OnChanging("StatusId", oldValue, value))
					OnChanged("StatusId");
				else
					statusId = oldValue;
			}
		}
		#endregion
		#region TimeZoneId
		string timeZoneId = String.Empty;
		public override string TimeZoneId {
			get { return timeZoneId; }
			set {
				if (timeZoneId == value)
					return;
				string oldValue = timeZoneId;
				timeZoneId = value;
				if (OnChanging("TimeZoneId", oldValue, value))
					OnChanged("TimeZoneId");
				else
					timeZoneId = oldValue;
			}
		}
		#endregion
	}
	#region OccurrenceIAppointmentBase (abstract class)
	public class OccurrenceIAppointmentBase : DefaultIAppointmentBase {
		#region Fields
		DateTime start;
		Appointment occurrence;
		IAppointmentBase patternBaseAppointment;
		#endregion
		public OccurrenceIAppointmentBase(Appointment occurrence) {
			Guard.ArgumentNotNull(occurrence, "occurrence");
			this.occurrence = occurrence;
			this.patternBaseAppointment = occurrence.RecurrencePattern as IAppointmentBase;
		}
		#region Properties
		protected internal IAppointmentBase FallbackAppointment { get { return patternBaseAppointment; } }
		#region Start
		public override DateTime Start {
			get { return start; }
			set {
				if (Start == value)
					return;
				DateTime oldValue = Start;
				this.start = value;
				if (OnChanging("Start", oldValue, value))
					OnChanged("Start");
				else
					start = oldValue;
			}
		}
		#endregion
		#region End
		public override DateTime End {
			get { return FallbackAppointment.End; }
			set {
				if (value == End)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.End = value;
			}
		}
		#endregion
		#region Duration
		public override TimeSpan Duration {
			get { return FallbackAppointment.Duration; }
			set {
				if (value == Duration)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.Duration = value;
			}
		}
		#endregion
		#region AllDay
		public override bool AllDay {
			get { return FallbackAppointment.AllDay; }
			set {
				if (value == AllDay)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.AllDay = value;
			}
		}
		#endregion
		#region Subject
		public override string Subject {
			get { return FallbackAppointment.Subject; }
			set {
				if (value == Subject)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.Subject = value;
			}
		}
		#endregion
		#region Description
		public override string Description {
			get { return FallbackAppointment.Description; }
			set {
				if (value == Description)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.Description = value;
			}
		}
		#endregion
		#region Location
		public override string Location {
			get { return FallbackAppointment.Location; }
			set {
				if (value == Location)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.Location = value;
			}
		}
		#endregion
		#region LabelId
		public override object LabelId {
			get { return FallbackAppointment.LabelId; }
			set {
				if (value == LabelId)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.LabelKey = value;
			}
		}
		#endregion
		#region StatusId
		public override object StatusId {
			get { return FallbackAppointment.StatusId; }
			set {
				if (value == StatusId)
					return;
				CreateOwnOccurrenceIAppointment();
				occurrence.StatusKey = value;
			}
		}
		#endregion
		#region TimeZoneId
		public override string TimeZoneId {
			get { return FallbackAppointment.TimeZoneId; }
			set { FallbackAppointment.TimeZoneId = value; }
		}
		#endregion
		#endregion
		protected internal virtual void CreateOwnOccurrenceIAppointment() {
		}
	}
	#endregion
	#region IAppointmentReminders
	public interface IAppointmentReminders : INotifyPropertyChanged, IDisposable {
		ReminderCollection Reminders { get; }
		ReminderCollection CreateRemindersCopy(ReminderCollection source);
		void ReplaceReminders(ReminderCollection source);
		void UpdateReminders();
		int BeginDismissReminder(Reminder reminder);
		void CancelDismissReminder(Reminder reminder, int index);
		void EndDismissReminder(Reminder reminder);
		Reminder CreateNewReminder();
		event CancellablePropertyChangingEventHandler PropertyChanging;
	}
	#endregion
	#region DefaultAppointmentRemindersBase (abstract class)
	public abstract class DefaultAppointmentRemindersBase : IAppointmentReminders {
		#region Fields
		readonly Appointment appointment;
		readonly ReminderCollection reminders;
		ReminderCollectionChangedListener listener;
		#endregion
		protected DefaultAppointmentRemindersBase(Appointment appointment, bool appendSingleReminderByDefault) {
			Guard.ArgumentNotNull(appointment, "appointment");
			this.appointment = appointment;
			this.reminders = CreateReminderCollection();
			if (appendSingleReminderByDefault)
				AppendSingleReminder();
			this.listener = CreateReminderCollectionListener();
			SubscribeReminderCollectionEvents();
		}
		protected internal virtual void AppendSingleReminder() {
			Reminders.Add(CreateNewReminder());
		}
		#region Properties
		public ReminderCollection Reminders { get { return reminders; } }
		public Appointment Appointment { get { return appointment; } }
		protected internal ReminderCollectionChangedListener Listener { get { return listener; } }
		#endregion
		#region Events
		#region PropertyChanging
		CancellablePropertyChangingEventHandler onPropertyChanging;
		public event CancellablePropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanging != null) {
				CancellablePropertyChangingEventArgs args = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
				onPropertyChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (listener != null) {
					UnsubscribeReminderCollectionEvents();
					listener.Dispose();
					listener = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DefaultAppointmentRemindersBase() {
			Dispose(false);
		}
		#endregion
		protected internal virtual ReminderCollection CreateReminderCollection() {
			return new ReminderCollection();
		}
		protected internal virtual ReminderCollectionChangedListener CreateReminderCollectionListener() {
			return new ReminderCollectionChangedListener(Reminders);
		}
		protected internal virtual void SubscribeReminderCollectionEvents() {
			listener.ReminderContentChanging += OnReminderContentChanging;
			listener.Changing += OnRemindersChanging;
			listener.Changed += OnRemindersChanged;
		}
		protected internal virtual void UnsubscribeReminderCollectionEvents() {
			listener.ReminderContentChanging -= OnReminderContentChanging;
			listener.Changing -= OnRemindersChanging;
			listener.Changed -= OnRemindersChanged;
		}
		protected internal virtual void OnReminderContentChanging(object sender, ReminderContentChangingEventArgs e) {
			e.Cancel = !RaisePropertyChanging(String.Format("Reminder.{0}", e.PropertyName), e.OldValue, e.NewValue);
		}
		protected internal virtual void OnRemindersChanging(object sender, CancelEventArgs e) {
			e.Cancel = !RaisePropertyChanging("Reminders", Reminders, Reminders);
		}
		protected internal virtual void OnRemindersChanged(object sender, EventArgs e) {
			RaisePropertyChanged("Reminders");
		}
		public virtual ReminderCollection CreateRemindersCopy(ReminderCollection source) {
			ReminderCollection result = new ReminderCollection(DXCollectionUniquenessProviderType.None);
			int count = source.Count;
			for (int i = 0; i < count; i++)
				result.Add(CopyReminder(source[i]));
			return result;
		}
		public virtual void ReplaceReminders(ReminderCollection source) {
			if (source.Count <= 0 && Reminders.Count <= 0)
				return;
			UnsubscribeReminderCollectionEvents();
			try {
				Reminders.BeginUpdate();
				try {
					Reminders.Clear();
					Reminders.AddRange(source);
				} finally {
					Reminders.CancelUpdate();
				}
			} finally {
				SubscribeReminderCollectionEvents();
			}
		}
		public virtual void UpdateReminders() {
			if (Reminders.Count <= 0)
				return;
			UnsubscribeReminderCollectionEvents();
			try {
				Reminders.ForEach(UpdateReminder);
			} finally {
				SubscribeReminderCollectionEvents();
			}
		}
		public virtual Reminder CreateNewReminder() {
			return new Reminder(Appointment);
		}
		protected internal virtual Reminder CopyReminder(Reminder source) {
			Reminder reminder = CreateNewReminder();
			reminder.TimeBeforeStart = source.TimeBeforeStart;
			UpdateReminder(reminder);
			return reminder;
		}
		protected internal virtual void UpdateReminder(Reminder reminder) {
			(reminder).OnAppointmentChanged();
		}
		public virtual int BeginDismissReminder(Reminder reminder) {
			int index;
			UnsubscribeReminderCollectionEvents();
			try {
				index = Reminders.IndexOf(reminder);
				if (index >= 0)
					Reminders.RemoveAt(index);
			} finally {
				SubscribeReminderCollectionEvents();
			}
			(reminder).ContentChanging += OnReminderContentChanging;
			return index;
		}
		public virtual void CancelDismissReminder(Reminder reminder, int index) {
			(reminder).ContentChanging -= OnReminderContentChanging;
			if (index < 0)
				return;
			UnsubscribeReminderCollectionEvents();
			try {
				Reminders.Insert(index, reminder);
			} finally {
				SubscribeReminderCollectionEvents();
			}
		}
		public virtual void EndDismissReminder(Reminder reminder) {
			(reminder).ContentChanging -= OnReminderContentChanging;
		}
	}
	#endregion
	#region DefaultAppointmentReminders
	public class DefaultAppointmentReminders : DefaultAppointmentRemindersBase {
		public DefaultAppointmentReminders(Appointment appointment, bool appendSingleReminderByDefault)
			: base(appointment, appendSingleReminderByDefault) {
		}
	}
	#endregion
	#region DefaultPatternAppointmentReminders
	public class DefaultPatternAppointmentReminders : DefaultAppointmentRemindersBase {
		public DefaultPatternAppointmentReminders(Appointment appointment, bool appendSingleReminderByDefault)
			: base(appointment, appendSingleReminderByDefault) {
			XtraSchedulerDebug.Assert(appointment.Type == AppointmentType.Pattern);
		}
		public override Reminder CreateNewReminder() {
			return new RecurringReminder(Appointment);
		}
		protected internal override Reminder CopyReminder(Reminder source) {
			RecurringReminder sourceRecurringReminder = source as RecurringReminder;
			if (sourceRecurringReminder == null)
				return base.CopyReminder(source);
			return sourceRecurringReminder.CreateOwnerCopy(Appointment);
		}
	}
	#endregion
	#region DefaultOccurrenceAppointmentReminders
	public class DefaultOccurrenceAppointmentReminders : DefaultAppointmentRemindersBase {
		public DefaultOccurrenceAppointmentReminders(Appointment appointment)
			: base(appointment, false) {
			Guard.ArgumentNotNull(appointment.RecurrencePattern, "appointment.RecurrencePattern");
			XtraSchedulerDebug.Assert(appointment.Type == AppointmentType.Occurrence);
		}
		protected internal override ReminderCollection CreateReminderCollection() {
			return new ReminderReadOnlyCollection(Appointment.RecurrencePattern);
		}
		protected internal override ReminderCollectionChangedListener CreateReminderCollectionListener() {
			return new OccurrenceReminderCollectionChangedListener(Reminders);
		}
		protected internal override void AppendSingleReminder() {
		}
		public override void ReplaceReminders(ReminderCollection source) {
		}
	}
	#endregion
	#region IRecurrenceChain
	public interface IRecurrenceChain : INotifyPropertyChanged, IDisposable {
		Appointment RecurrencePattern { get; }
		IRecurrenceInfo RecurrenceInfo { get; set; }
		AppointmentBaseCollection RecurrenceExceptions { get; }
		Appointment CreateException(AppointmentType type, int recurrenceIndex);
		void RegisterException(Appointment exception);
		bool CanDeleteExceptions();
		void DeleteExceptions();
		Appointment FindException(int recurrenceIndex);
		Appointment CreateOccurrence(int recurrenceIndex); 
		Appointment CreateOccurrence(DateTime start); 
		Appointment CreateOccurrence(DateTime start, int recurrenceIndex);
		Appointment GetOccurrence(int recurrenceIndex);
		bool CanDeleteOccurrence(Appointment occurrence);
		void DeleteOccurrence(Appointment occurrence);
		event CancellablePropertyChangingEventHandler PropertyChanging;
	}
	#endregion
	#region DefaultRecurrenceChain
	public class DefaultRecurrenceChain : IRecurrenceChain {
		#region Fields
		Appointment recurrencePattern;
		AppointmentBaseCollection exceptions;
		IRecurrenceInfo recurrenceInfo;
		#endregion
		public DefaultRecurrenceChain(Appointment recurrencePattern) {
			Guard.ArgumentNotNull(recurrencePattern, "recurrencePattern");
			this.recurrencePattern = recurrencePattern;
			this.exceptions = new AppointmentBaseCollection();
			this.recurrenceInfo = CreateRecurrenceInfo();
			SubscribeRecurrenceInfoEvents();
		}
		#region Properties
		public Appointment RecurrencePattern { get { return recurrencePattern; } }
		TimeZoneEngine TimeZoneEngine {
			get {
				return ((IInternalAppointment)recurrencePattern).TimeZoneEngine;
			}
		}
		#region RecurrenceInfo
		public IRecurrenceInfo RecurrenceInfo {
			get { return recurrenceInfo; }
			set {
				Guard.ArgumentNotNull(value, "RecurrenceInfo");
				if (Object.ReferenceEquals(value, RecurrenceInfo))
					return;
				UnsubscribeRecurrenceInfoEvents();
				IRecurrenceInfo oldRecurrenceInfo = RecurrenceInfo;
				this.recurrenceInfo = value;
				if (RaisePropertyChanging("RecurrenceInfo", String.Empty, String.Empty))
					RaisePropertyChanged("RecurrenceInfo");
				else
					this.recurrenceInfo = oldRecurrenceInfo;
				SubscribeRecurrenceInfoEvents();
			}
		}
		#endregion
		public AppointmentBaseCollection RecurrenceExceptions { get { return exceptions; } }
		#endregion
		#region Events
		#region PropertyChanging
		CancellablePropertyChangingEventHandler onPropertyChanging;
		public event CancellablePropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanging != null) {
				CancellablePropertyChangingEventArgs args = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
				onPropertyChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (exceptions != null) {
					DisposeExceptions();
					exceptions = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DefaultRecurrenceChain() {
			Dispose(false);
		}
		#endregion
		protected internal virtual IRecurrenceInfo CreateRecurrenceInfo() {
			IRecurrenceInfo recurrenceInfo = new RecurrenceInfo();
			((IInternalRecurrenceInfo)recurrenceInfo).SetTimeZoneId(recurrencePattern.TimeZoneId);
			recurrenceInfo.Type = RecurrenceType.Daily;
			recurrenceInfo.Range = RecurrenceRange.NoEndDate;
			recurrenceInfo.Periodicity = 1;
			recurrenceInfo.Start = RecurrencePattern.Start;
			recurrenceInfo.Duration = RecurrencePattern.Duration;
			return recurrenceInfo;
		}
		protected internal virtual void SubscribeRecurrenceInfoEvents() {
			((IInternalRecurrenceInfo)RecurrenceInfo).Changing += OnRecurrenceInfoChanging;
			((IInternalRecurrenceInfo)RecurrenceInfo).Changed += OnRecurrenceInfoChanged;
		}
		protected internal virtual void UnsubscribeRecurrenceInfoEvents() {
			((IInternalRecurrenceInfo)RecurrenceInfo).Changing -= OnRecurrenceInfoChanging;
			((IInternalRecurrenceInfo)RecurrenceInfo).Changed -= OnRecurrenceInfoChanged;
		}
		protected virtual void OnRecurrenceInfoChanging(object sender, CancelEventArgs e) {
			e.Cancel = !RaisePropertyChanging("RecurrenceInfo", String.Empty, String.Empty);
		}
		protected virtual void OnRecurrenceInfoChanged(object sender, EventArgs e) {
			RaisePropertyChanged("RecurrenceInfo");
		}
		protected internal virtual void DisposeExceptions() {
			int count = RecurrenceExceptions.Count;
			for (int i = 0; i < count; i++)
				RecurrenceExceptions[i].Dispose();
			exceptions = null;
		}
		public virtual Appointment CreateException(AppointmentType type, int recurrenceIndex) {
			if (!SchedulerUtils.IsExceptionType(type))
				Exceptions.ThrowArgumentException("type", type);
			if (FindException(recurrenceIndex) != null)
				Exceptions.ThrowArgumentException("recurrenceIndex", recurrenceIndex);
			Appointment occurrence = CreateOccurrence(recurrenceIndex);
			((IInternalAppointment)occurrence).SetTypeCore(type);
			if ( ((IInternalPersistentObject)RecurrencePattern).RaiseStateChanging(occurrence, PersistentObjectState.ChildCreated, String.Empty, null, null) ) {
				RegisterException(occurrence);
				return occurrence;
			} else {
				occurrence.Dispose();
				return null;
			}
		}
		protected internal virtual bool CanRegisterException(AppointmentType exceptionType) {
			return SchedulerUtils.IsExceptionType(exceptionType);
		}
		Dictionary<IPersistentObject, PersistentObjectState> deferredStateChanged = new Dictionary<IPersistentObject, PersistentObjectState>();
		bool beginUpdate = false;
		public void BeginDeferredUpdate() {
			this.beginUpdate = true;
			this.deferredStateChanged.Clear();
		}
		public void EndDeferredUpdate() {
			if (!this.beginUpdate)
				return;
			this.beginUpdate = false;
			foreach ( var stateChangedPair in deferredStateChanged ) {
				((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(stateChangedPair.Key, stateChangedPair.Value);
			}
			this.deferredStateChanged.Clear();
		}
		public void RegisterDeferredUpdate(IPersistentObject obj, PersistentObjectState state) {
			if (this.deferredStateChanged.ContainsKey(obj))
				return;
			this.deferredStateChanged.Add(obj, state);
		}
		public virtual void RegisterException(Appointment exception) {
			Guard.ArgumentNotNull(exception, "exception");
			if (!CanRegisterException(exception.Type))
				Exceptions.ThrowArgumentException("exception", exception);
			if (RecurrenceExceptions.Contains(exception))
				Exceptions.ThrowArgumentException("exception", exception);
			RecurrenceExceptions.Add(exception);
			((IInternalAppointment)exception).SetPattern(RecurrencePattern);
			if (this.beginUpdate)
				RegisterDeferredUpdate(exception, PersistentObjectState.ChildCreated);
			else
				((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(exception, PersistentObjectState.ChildCreated);
		}
		public virtual bool CanDeleteExceptions() {
			int count = RecurrenceExceptions.Count;
			for (int i = 0; i < count; i++)
				if ( !((IInternalPersistentObject)RecurrencePattern).RaiseChildDeleting(RecurrenceExceptions[i]) )
					return false;
			return true;
		}
		public virtual void DeleteExceptions() {
			int count = RecurrenceExceptions.Count;
			for (int i = count - 1; i >= 0; i--) {
				Appointment exception = RecurrenceExceptions[i];
				RecurrenceExceptions.RemoveAt(i);
				((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(exception, PersistentObjectState.ChildDeleted);
			}
			XtraSchedulerDebug.Assert(RecurrenceExceptions.Count == 0);
		}
		public virtual Appointment FindException(int recurrenceIndex) {
			int count = RecurrenceExceptions.Count;
			for ( int i = 0; i < count; i++ )
				if ( RecurrenceExceptions[i].RecurrenceIndex == recurrenceIndex )
					return RecurrenceExceptions[i];
			return null;
		}
		public virtual Appointment CreateOccurrence(DateTime start) {
			Appointment occurrence = (Appointment)Activator.CreateInstance(RecurrencePattern.GetType());
			occurrence.Assign(RecurrencePattern);
			IInternalAppointment occurrenceInternal = (IInternalAppointment) occurrence;
			occurrenceInternal.SetTypeCore(AppointmentType.Occurrence);
			occurrence.Start = start;
			occurrenceInternal.SetPattern(RecurrencePattern);
			return occurrence;
		}
		public virtual Appointment CreateOccurrence(DateTime start, int recurrenceIndex) {
			Appointment occurrence = CreateOccurrence(start);
			((IInternalAppointment)occurrence).SetRecurrenceIndex(recurrenceIndex);
			return occurrence;
		}
		public virtual Appointment CreateOccurrence(int recurrenceIndex) {
			Guard.ArgumentNonNegative(recurrenceIndex, "recurrenceIndex");
			OccurrenceCalculator calculator = OccurrenceCalculator.CreateInstance(RecurrenceInfo, TimeZoneEngine);
			DateTime start = calculator.CalcOccurrenceStartTime(recurrenceIndex);
			return CreateOccurrence(start, recurrenceIndex);
		}
		public virtual Appointment GetOccurrence(int recurrenceIndex) {
			Guard.ArgumentNonNegative(recurrenceIndex, "recurrenceIndex");
			Appointment apt = FindException(recurrenceIndex);
			if (apt != null)
				return (apt.Type == AppointmentType.DeletedOccurrence) ? null : apt;
			return CreateOccurrence(recurrenceIndex);
		}
		public virtual bool CanDeleteOccurrence(Appointment occurrence) {
			return ((IInternalPersistentObject)RecurrencePattern).RaiseChildDeleting(occurrence);
		}
		public virtual void DeleteOccurrence(Appointment occurrence) {
			IInternalAppointment occurrenceInternal = (IInternalAppointment)occurrence;
			switch (occurrence.Type) {
				case AppointmentType.DeletedOccurrence:
					RecurrenceExceptions.Remove(occurrence);
					((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(occurrence, PersistentObjectState.ChildDeleted);
					break;
				case AppointmentType.ChangedOccurrence:
					occurrenceInternal.SetTypeCore(AppointmentType.DeletedOccurrence);
					((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(occurrence, PersistentObjectState.Changed);
					break;
				case AppointmentType.Occurrence:
					occurrenceInternal.SetTypeCore(AppointmentType.DeletedOccurrence);
					RegisterException(occurrence);
					break;
				default:
					Exceptions.ThrowArgumentException("occurrence", occurrence);
					break;
			}
		}
	}
	#endregion
	#region IAppointmentResources
	public interface IAppointmentResources : INotifyPropertyChanged, IDisposable {
		AppointmentResourceIdCollection ResourceIds { get; }
		event CancellablePropertyChangingEventHandler PropertyChanging;
	}
	#endregion
	#region DefaultAppointmentResources
	public class DefaultAppointmentResources : IAppointmentResources {
		#region Fields
		AppointmentResourceIdCollection resourceIds;
		NotificationCollectionChangedListener<object> listener;
		#endregion
		public DefaultAppointmentResources() {
		}
		#region Properties
		public AppointmentResourceIdCollection ResourceIds { get { return resourceIds; } }
		#endregion
		#region Events
		#region PropertyChanging
		CancellablePropertyChangingEventHandler onPropertyChanging;
		public event CancellablePropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanging != null) {
				CancellablePropertyChangingEventArgs args = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
				onPropertyChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (listener != null) {
					UnsubscribeResourceIdsEvents();
					listener.Dispose();
					listener = null;
				}
				resourceIds = null;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal virtual void Initialize() {
			this.resourceIds = CreateResourceIdCollection();
			this.listener = new NotificationCollectionChangedListener<object>(ResourceIds);
			SubscribeResourceIdsEvents();
		}
		protected internal virtual AppointmentResourceIdCollection CreateResourceIdCollection() {
			return new AppointmentResourceIdCollection();
		}
		protected internal virtual void SubscribeResourceIdsEvents() {
			listener.Changing += new CancelEventHandler(OnResourceIdsChanging);
			listener.Changed += new EventHandler(OnResourceIdsChanged);
		}
		protected internal virtual void UnsubscribeResourceIdsEvents() {
			listener.Changing -= new CancelEventHandler(OnResourceIdsChanging);
			listener.Changed -= new EventHandler(OnResourceIdsChanged);
		}
		protected internal virtual void OnResourceIdsChanging(object sender, CancelEventArgs e) {
			e.Cancel = !RaisePropertyChanging("Resources", ResourceIds, ResourceIds);
		}
		protected internal virtual void OnResourceIdsChanged(object sender, EventArgs e) {
			RaisePropertyChanged("ResourceIds");
		}
	}
	#endregion
	#region DefaultOccurrenceResources
	public class DefaultOccurrenceResources : DefaultAppointmentResources {
		readonly AppointmentResourceIdCollection patternResourceIds;
		public DefaultOccurrenceResources(AppointmentResourceIdCollection patternResourceIds) {
			Guard.ArgumentNotNull(patternResourceIds, "patternResourceIds");
			this.patternResourceIds = patternResourceIds;
		}
		protected internal override AppointmentResourceIdCollection CreateResourceIdCollection() {
			return new AppointmentResourceIdReadOnlyCollection(patternResourceIds);
		}
	}
	#endregion
	#region IAppointmentProcess
	public interface IAppointmentProcess : INotifyPropertyChanged {
		int PercentComplete { get; set; }
		event CancellablePropertyChangingEventHandler PropertyChanging;
	}
	#endregion
	#region AppointmentProcessBase (abstract class)
	public abstract class AppointmentProcessBase : IAppointmentProcess {
		#region Properties
		public abstract int PercentComplete { get; set; }
		#endregion
		#region Events
		#region PropertyChanging
		CancellablePropertyChangingEventHandler onPropertyChanging;
		public event CancellablePropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanging != null) {
				CancellablePropertyChangingEventArgs args = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
				onPropertyChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		protected internal virtual bool OnChanging<T>(string propertyName, T oldValue, T newValue) {
			return RaisePropertyChanging(propertyName, oldValue, newValue);
		}
		protected internal virtual void OnChanged(string propertyName) {
			RaisePropertyChanged(propertyName);
		}
	}
	#endregion
	#region EmptyAppointmentProcess
	public class EmptyAppointmentProcess : AppointmentProcessBase {
		#region Properties
		public override int PercentComplete { get { return 0; } set { } }
		#endregion
	}
	#endregion
	#region DefaultAppointmentProcess
	public class DefaultAppointmentProcess : AppointmentProcessBase {
		#region Fields
		int percentComplete;
		#endregion
		public DefaultAppointmentProcess() {
		}
		#region Properties
		public override int PercentComplete {
			get { return percentComplete; }
			set {
				if (percentComplete == value)
					return;
				int oldValue = percentComplete;
				percentComplete = value;
				if (OnChanging("PercentComplete", oldValue, value))
					OnChanged("PercentComplete");
				else
					percentComplete = oldValue;
			}
		}
		#endregion
	}
	#endregion
}
