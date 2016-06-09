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
using System.Linq;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region AppointmentFormControllerBase
	public abstract class AppointmentFormControllerBase : INotifyPropertyChanged {
		#region Fields
		Appointment sourceApt;
		Appointment sourcePattern;
		Appointment editedAptCopy;
		Appointment editedPattern;
		InnerSchedulerControl innerControl;
		IAppointmentStorageBase innerAppointments;
		bool timeZoneVisible;
		#endregion
		protected AppointmentFormControllerBase(InnerSchedulerControl innerControl, Appointment apt) {
			if (innerControl == null)
				Exceptions.ThrowArgumentException("innerControl", innerControl);
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			ISchedulerStorageBase storage = innerControl.Storage;
			if (storage == null)
				Exceptions.ThrowArgumentException("innerControl.Storage", storage);
			this.innerAppointments = storage.Appointments;
			if (this.innerAppointments == null)
				Exceptions.ThrowArgumentException("innerControl.Storage.Appointments", innerAppointments);
			this.innerControl = innerControl;
			this.sourceApt = apt;
			this.timeZoneVisible = apt.TimeZoneId != innerControl.TimeZoneHelper.ClientTimeZone.Id && !String.IsNullOrEmpty(apt.TimeZoneId);
			CreateAppointmentCopies();
			SubscribeEditedAptCopyEvents();			
		}
		#region Properties
		protected internal InnerSchedulerControl InnerControl { get { return innerControl; } }
		protected internal IAppointmentStorageBase InnerAppointments { get { return innerAppointments; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseIsNewAppointment")]
#endif
		public virtual bool IsNewAppointment { get { return InnerAppointments.IsNewAppointment(sourceApt); } }
		protected internal Appointment SourceAppointment { get { return sourceApt; } }
		internal Appointment SourcePattern { get { return sourcePattern; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseEditedAppointmentCopy")]
#endif
		public Appointment EditedAppointmentCopy { get { return editedAptCopy; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseEditedPattern")]
#endif
		public Appointment EditedPattern { get { return editedPattern; } }
		public AppointmentType AppointmentType {
			get { return EditedAppointmentCopy.Type; }
		}
		#region ReadOnly
		bool readOnly = false;
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				if (ReadOnly == value)
					return;
				readOnly = value;
				NotifyPropertyChanged("ReadOnly");
				NotifyPropertyChanged("Caption");
				NotifyPropertyChanged("CanDeleteAppointment");
			}
		}
		#endregion
		public string Caption { get { return UpdateFormCaption(); } }
		public string TimeZoneId {
			get { return ActualTimeZoneId; }
			set {
				if (TimeZoneId == value)
					return;
				DateTime savedStart = Start;
				editedAptCopy.TimeZoneId = value;
				Start = savedStart;
				NotifyPropertyChanged("TimeZoneId");
			}
		}		
		public bool TimeZoneVisible {
			get { return TimeZonesEnabled && timeZoneVisible; }
			set {
				if (timeZoneVisible == value)
					return;
				timeZoneVisible = value;
				if (TimeZonesEnabled)
					NotifyPropertyChanged("TimeZoneVisible");
			}
		}
		public bool TimeZonesEnabled {
			get {
				return InnerControl.Storage != null && InnerControl.Storage.EnableTimeZones;			
			}
		}
		public bool TimeZoneEnabled {
			get {
				return !AllDay && AppointmentType == XtraScheduler.AppointmentType.Normal && SourceAppointment.Type == XtraScheduler.AppointmentType.Normal;
			}
		}
		public virtual bool IsTimeEnabled {
			get { return IsDateTimeEditable && !AllDay; }
		}
		public virtual bool IsDateTimeEditable {
			get { return AppointmentType != AppointmentType.Pattern; }
		}
		public bool IsRecurrentAppointment {
			get { return AppointmentType == XtraScheduler.AppointmentType.Pattern; }
			set { }
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseSubject")]
#endif
		public string Subject {
			get { return editedAptCopy.Subject; }
			set {
				editedAptCopy.Subject = value;
				NotifyPropertyChanged("Subject");
				NotifyPropertyChanged("Caption");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseLocation")]
#endif
		public string Location {
			get { return editedAptCopy.Location; }
			set {
				editedAptCopy.Location = value;
				NotifyPropertyChanged("Location");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseDescription")]
#endif
		public string Description {
			get { return editedAptCopy.Description; }
			set {
				editedAptCopy.Description = value;
				NotifyPropertyChanged("Description");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseAllDay")]
#endif
		public bool AllDay {
			get { return editedAptCopy.AllDay; }
			set {
				if (editedAptCopy.AllDay == value)
					return;
				if (value)
					TimeZoneId = innerControl.TimeZoneHelper.ClientTimeZone.Id;
				if (editedAptCopy.AllDay == false) {
					editedAptCopy.Start = InnerControl.TimeZoneHelper.ToClientTime(editedAptCopy.Start);
				} 
				editedAptCopy.AllDay = value;
				if (!value) {
					editedAptCopy.Start = InnerControl.TimeZoneHelper.FromClientTime(editedAptCopy.Start);
				}
				if (editedPattern != null) {
					editedPattern.AllDay = value;
					editedPattern.RecurrenceInfo.AllDay = value;
					editedPattern.RecurrenceInfo.Start = editedPattern.Start;
				}
				UpdateAppointmentStatusCore(GetInnerStatus());
				NotifyPropertyChanged("AllDay");
				NotifyPropertyChanged("Status");
				NotifyPropertyChanged("Caption");
				NotifyPropertyChanged("TimeZoneEnabled");
				NotifyPropertyChanged("IsTimeEnabled");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseStatusId")]
#endif
		public object StatusKey {
			get { return editedAptCopy.StatusKey; }
			set {
				if (Object.Equals(editedAptCopy.StatusKey, value))
					return;
				editedAptCopy.StatusKey = value;
				OnStatusIdChanged();
			}
		}
		[Obsolete("Use the StatusKey property instead.")]
		public int StatusId {
			get {
				int statusValue = 0;
				try {
					statusValue = Convert.ToInt32(StatusKey);
				} catch { }
				return statusValue;
			}
			set { StatusKey = value; }
		}
		protected virtual void OnStatusIdChanged() {
			NotifyPropertyChanged("StatusId");
			NotifyPropertyChanged("StatusKey");
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseLabelId")]
#endif
		public object LabelKey {
			get { return editedAptCopy.LabelKey; }
			set {
				if (Object.Equals(editedAptCopy.LabelKey, value))
					return;
				editedAptCopy.LabelKey = value;
				OnLabelIdChanged();
			}
		}
		[Obsolete("Use the LabelKey property instead.")]
		public int LabelId {
			get {
				int labelValue = 0;
				try {
					labelValue = Convert.ToInt32(LabelKey);
				} catch { }
				return labelValue;
			}
			set { LabelKey = value; }
		}
		protected virtual void OnLabelIdChanged() {
			NotifyPropertyChanged("LabelId");
			NotifyPropertyChanged("LabelKey");
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseHasReminder")]
#endif
		public bool HasReminder {
			get { return editedAptCopy.HasReminder; }
			set {
				editedAptCopy.HasReminder = value;
				NotifyPropertyChanged("HasReminder");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseResourceId")]
#endif
		public object ResourceId { get { return editedAptCopy.ResourceId; } set { editedAptCopy.ResourceId = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseResourceIds")]
#endif
		public AppointmentResourceIdCollection ResourceIds { 
			get { 
				return editedAptCopy.ResourceIds; 
			}
			set {
				try {
					editedAptCopy.BeginUpdate();
					editedAptCopy.ResourceIds.Clear();
					if (value == null)
						return;
					editedAptCopy.ResourceIds.AddRange(value);
				} finally {
					editedAptCopy.EndUpdate(); ;
				}
				NotifyPropertyChanged("ResourceIds");
			}
		}
		protected string ActualTimeZoneId {
			get {
				if (String.IsNullOrEmpty(EditedAppointmentCopy.TimeZoneId))
					return innerControl.TimeZoneHelper.ClientTimeZone.Id;
				return EditedAppointmentCopy.TimeZoneId;
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseStart")]
#endif
		public DateTime Start {
			get {
				if (AllDay)
					return editedAptCopy.Start;
				return InnerControl.TimeZoneHelper.FromOperationTime(editedAptCopy.Start, ActualTimeZoneId);
			}
			set {
				DateTime newStart = (AllDay) ? value : InnerControl.TimeZoneHelper.ToOperationTime(value, ActualTimeZoneId);
				if (newStart == editedAptCopy.Start)
					return;
				editedAptCopy.Start = newStart;
				NotifyPropertyChanged("Start");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseEnd")]
#endif
		public DateTime End {
			get {
				if (AllDay)
					return editedAptCopy.End;
				return InnerControl.TimeZoneHelper.FromOperationTime(editedAptCopy.End, ActualTimeZoneId);
			}
			set {
				DateTime newEnd = (AllDay) ? value : InnerControl.TimeZoneHelper.ToOperationTime(value, ActualTimeZoneId);
				if (newEnd == editedAptCopy.End)
					return;
				editedAptCopy.End = newEnd;
				NotifyPropertyChanged("End");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseDisplayStart")]
#endif
		public DateTime DisplayStart {
			get { return Start; }
			set {
				Start = value;
				NotifyPropertyChanged("Start");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseDisplayEnd")]
#endif
		public DateTime DisplayEnd {
			get {
				if (AllDay)
					return End.AddDays(-1);
				else
					return End;
			}
			set {
				if (AllDay)
					End = value.AddDays(1);
				else
					End = value;
				NotifyPropertyChanged("DisplayEnd");
				NotifyPropertyChanged("EndDate");
				NotifyPropertyChanged("End");  
			}
		}		
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseReminderTimeBeforeStart")]
#endif
		public TimeSpan ReminderTimeBeforeStart {
			get {
				if (editedAptCopy.HasReminder)
					return editedAptCopy.Reminder.TimeBeforeStart;
				else
					return TimeSpan.MinValue;
			}
			set {
				if (value == TimeSpan.MinValue) {
					if (HasReminder)
						HasReminder = false;
				} else {
					if (!HasReminder)
						HasReminder = true;
					if (this.editedAptCopy.Reminder.TimeBeforeStart == value)
						return;
					this.editedAptCopy.Reminder.TimeBeforeStart = value;
					NotifyPropertyChanged("ReminderTimeBeforeStart");
				}
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseCanEditResource")]
#endif
		public virtual bool CanEditResource {
			get {
				if (!sourceApt.IsBase)
					return false;
				if (IsNewAppointment)
					return true;
				AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
				return helper.CanDragAppointmentBetweenResources(sourceApt);
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseResourceSharing")]
#endif
		public bool ResourceSharing { get { return innerControl.ResourceSharing; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseCanDeleteAppointment")]
#endif
		public bool CanDeleteAppointment {
			get {
				if (IsNewAppointment)
					return false;
				AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
				return !ReadOnly && helper.CanDeleteAppointment(sourceApt);
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBaseShouldShowRecurrenceButton")]
#endif
		public virtual bool ShouldShowRecurrenceButton {
			get {
				if (!innerControl.SupportsRecurrence)
					return false;
				if (SourceAppointment.Type == AppointmentType.Occurrence || SourceAppointment.Type == AppointmentType.ChangedOccurrence)
					return false;
				SchedulerOptionsCustomization customizationOptions = innerControl.OptionsCustomization;
				if (IsNewAppointment) {
					return (customizationOptions.AllowAppointmentCreate != UsedAppointmentType.NonRecurring &&
						customizationOptions.AllowAppointmentCreate != UsedAppointmentType.None);
				} else {
					return (customizationOptions.AllowAppointmentEdit != UsedAppointmentType.NonRecurring &&
						customizationOptions.AllowAppointmentEdit != UsedAppointmentType.None);
				}
			}
		}
		#endregion
		protected internal virtual string UpdateFormCaption() {
			return SchedulerUtils.FormatAppointmentFormCaption(AllDay, Subject, ReadOnly);
		}
		protected internal virtual void CreateAppointmentCopies() {
			this.editedAptCopy = this.sourceApt.Copy();
			if (this.editedAptCopy.Type == AppointmentType.Pattern) {
				this.sourcePattern = this.sourceApt;
				this.editedPattern = this.editedAptCopy;
			} else if (this.sourceApt.RecurrencePattern != null) {
				this.editedPattern = this.sourceApt.RecurrencePattern.Copy();
				this.sourcePattern = this.sourceApt.RecurrencePattern;
			} else {
				this.editedPattern = null;
				this.sourcePattern = null;
			}
		}
		public void SetStatus(IAppointmentStatus status) {
			editedAptCopy.StatusKey = (int)status.Id;
			NotifyPropertyChanged("StatusId");
			NotifyPropertyChanged("Status");
		}
		protected internal IAppointmentStatus GetInnerStatus() {
			return InnerAppointments.Statuses.GetById(editedAptCopy.StatusKey);
		}
		public void SetLabel(IAppointmentLabel label) {
			editedAptCopy.LabelKey = (int)label.Id;
			NotifyPropertyChanged("LabelId");
			NotifyPropertyChanged("Label");
		}
		protected internal IAppointmentLabel GetInnerLabel() {
			return InnerAppointments.Labels.GetById(editedAptCopy.LabelKey);
		}
		public static bool ValidateInterval(DateTime startDate, TimeSpan startTime, DateTime endDate, TimeSpan endTime) {
			return ValidateInterval(startDate.Date + startTime, endDate.Date + endTime);
		}
		public static bool ValidateInterval(DateTime start, DateTime end) {
			return end >= start;
		}
		protected internal virtual IAppointmentStatus UpdateAppointmentStatusCore(IAppointmentStatus currentStatus) {
			AppointmentStatusType type = currentStatus.Type;
			bool typeToChange = type == AppointmentStatusType.Busy || type == AppointmentStatusType.Free;
			if (!typeToChange)
				return currentStatus;
			IAppointmentStatus newStatus = currentStatus;
			if ( AllDay && type == AppointmentStatusType.Busy )
				newStatus = InnerAppointments.Statuses.SingleOrDefault(s => s.Type == AppointmentStatusType.Free);
			if ( !AllDay && type == AppointmentStatusType.Free )
				newStatus = InnerAppointments.Statuses.SingleOrDefault(s => s.Type == AppointmentStatusType.Busy);
			SetStatus(newStatus);
			return newStatus;
		}
		public virtual bool IsConflictResolved() {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
			return helper.IsConflictResolved(editedAptCopy, sourceApt, false);
		}
		public virtual int CalculateConflictCount() {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
			return helper.CalculateConflictCount(editedAptCopy, sourceApt, false);
		}
		protected internal virtual Appointment CreateNewEditedAppointmentPattern() {
			Appointment patternCopy = editedAptCopy.Copy();
			((IInternalAppointment)patternCopy).SetTypeCore(AppointmentType.Pattern);
			patternCopy.RecurrenceInfo.OccurrenceCount = 10;
			patternCopy.RecurrenceInfo.FirstDayOfWeek = InnerControl.FirstDayOfWeek;
			return patternCopy;
		}
		public virtual Appointment PrepareToRecurrenceEdit() {
			Appointment patternCopy;
			if (editedPattern == null)
				patternCopy = CreateNewEditedAppointmentPattern();
			else {
				patternCopy = (Appointment)Activator.CreateInstance(editedPattern.GetType());
				patternCopy.Assign(editedPattern);
			}
			patternCopy.HasReminder = false;
			return patternCopy;
		}
		public virtual void RemoveRecurrence() {
			XtraSchedulerDebug.Assert(editedAptCopy.IsBase);
			((IInternalAppointment)editedAptCopy).SetTypeCore(AppointmentType.Normal);
			editedPattern = null;
			NotifyPropertyChanged("AppointmentType");
			NotifyPropertyChanged("TimeZoneEnabled");
			NotifyPropertyChanged("IsDateTimeEditable");
			NotifyPropertyChanged("IsTimeEnabled");
			NotifyPropertyChanged("IsRecurrentAppointment");
		}
		public virtual void ApplyRecurrence(Appointment patternCopy) {
			if (editedPattern == null) {
				((IInternalAppointment)editedAptCopy).SetTypeCore(AppointmentType.Pattern);
				editedPattern = editedAptCopy;
			}
			editedPattern.RecurrenceInfo.Assign(patternCopy.RecurrenceInfo);
			editedPattern.AllDay = patternCopy.AllDay;
			editedPattern.Start = patternCopy.Start;
			editedPattern.RecurrenceInfo.AllDay = editedPattern.AllDay;
			editedPattern.RecurrenceInfo.Start = editedPattern.Start;
			editedPattern.Duration = patternCopy.Duration;
			editedPattern.RecurrenceInfo.FirstDayOfWeek = InnerControl.FirstDayOfWeek;
			editedAptCopy = editedPattern;
			if (sourcePattern != null) 
				sourceApt = sourcePattern;
			NotifyPropertyChanged("AppointmentType");
			NotifyPropertyChanged("TimeZoneEnabled");
			NotifyPropertyChanged("IsDateTimeEditable");
			NotifyPropertyChanged("IsTimeEnabled");
			NotifyPropertyChanged("IsRecurrentAppointment");
		}
		public virtual bool AreExceptionsPresent() {
			switch (sourceApt.Type) {
				case AppointmentType.ChangedOccurrence:
					return true;
				case AppointmentType.Occurrence:
					return sourceApt.RecurrencePattern.HasExceptions || IsAppointmentChanged();
				case AppointmentType.Pattern:
					return sourceApt.HasExceptions;
				default:
					return false;
			}
		}
		public virtual bool IsAppointmentChanged() {
			try {
				if (SourceAppointment.Type == AppointmentType.Normal && editedAptCopy.Type == AppointmentType.Pattern ||
					SourceAppointment.Type == AppointmentType.Pattern && editedAptCopy.Type == AppointmentType.Normal)
					return true;
				if (SourceAppointment.Type == AppointmentType.Pattern) {
					IRecurrenceInfo sourceRecurrenceInfo = SourceAppointment.RecurrenceInfo;
					IRecurrenceInfo editedRecurrenceInfo = EditedPattern.RecurrenceInfo;
					if (sourceRecurrenceInfo.Start != editedRecurrenceInfo.Start ||
						sourceRecurrenceInfo.AllDay != editedRecurrenceInfo.AllDay ||
						sourceRecurrenceInfo.DayNumber != editedRecurrenceInfo.DayNumber ||
						sourceRecurrenceInfo.Duration != editedRecurrenceInfo.Duration ||
						sourceRecurrenceInfo.End != editedRecurrenceInfo.End ||
						sourceRecurrenceInfo.Month != editedRecurrenceInfo.Month ||
						sourceRecurrenceInfo.OccurrenceCount != editedRecurrenceInfo.OccurrenceCount ||
						sourceRecurrenceInfo.Periodicity != editedRecurrenceInfo.Periodicity ||
						sourceRecurrenceInfo.Range != editedRecurrenceInfo.Range ||
						sourceRecurrenceInfo.Type != editedRecurrenceInfo.Type ||
						sourceRecurrenceInfo.WeekDays != editedRecurrenceInfo.WeekDays ||
						sourceRecurrenceInfo.WeekOfMonth != editedRecurrenceInfo.WeekOfMonth)
						return true;
				}
				bool wasChanged = SourceAppointment.Start != EditedAppointmentCopy.Start ||
					SourceAppointment.Duration != EditedAppointmentCopy.Duration ||
					SourceAppointment.AllDay != EditedAppointmentCopy.AllDay ||
					SourceAppointment.Subject != EditedAppointmentCopy.Subject ||
					SourceAppointment.Location != EditedAppointmentCopy.Location ||
					!Object.Equals(SourceAppointment.StatusKey, EditedAppointmentCopy.StatusKey) ||
					!Object.Equals(SourceAppointment.LabelKey, EditedAppointmentCopy.LabelKey) ||
					SourceAppointment.Description != EditedAppointmentCopy.Description ||
					SourceAppointment.TimeZoneId != EditedAppointmentCopy.TimeZoneId ||
#if !SL && !WPF
 SourceAppointment.HasReminder != EditedAppointmentCopy.HasReminder ||
					SourceAppointment.PercentComplete != EditedAppointmentCopy.PercentComplete;
#else
					sourceApt.HasReminder != EditedAppointmentCopy.HasReminder;
#endif
				if (wasChanged)
					return true;
				if (!ResourceBase.InternalAreResourceIdsCollectionsSame(sourceApt.ResourceIds, editedAptCopy.ResourceIds))
					return true;
				int reminderCount = sourceApt.Reminders.Count;
				if (editedAptCopy.Reminders.Count != reminderCount)
					return true;
				if (!TimeZoneVisible && !String.IsNullOrEmpty(TimeZoneId) && TimeZoneId != innerControl.TimeZoneHelper.ClientTimeZone.Id)
					return true;
				for (int i = 0; i < reminderCount; i++) {
					if (SourceAppointment.Reminders[i].TimeBeforeStart != editedAptCopy.Reminders[i].TimeBeforeStart)
						return true;
				}
				return false;
			} catch {
			}
			return true;
		}
		public virtual void ApplyChanges() {
			bool isNewAppointment = IsNewAppointment;
			AppointmentType initialSourceAppointmentType = sourceApt.Type;
			PrepareActualSourceAppointment();
			sourceApt.BeginUpdate();
			InternalStartAppointmentsTransaction();
			try {
				ApplyChangesInternal(initialSourceAppointmentType);
			} finally {
				sourceApt.EndUpdate();
				AppointmentsTransactionType type = isNewAppointment ? AppointmentsTransactionType.Insert : AppointmentsTransactionType.Update;
				InternalCommitAppointmentsTransaction(new Appointment[] { sourceApt }, type);
			}
			if (isNewAppointment)
				InnerAppointments.Add(sourceApt);
		}
		void InternalCommitAppointmentsTransaction(Appointment[] appointment, AppointmentsTransactionType type) {
			ISupportsAppointmentTransaction transactionInstance = InnerAppointments as ISupportsAppointmentTransaction;
			if (transactionInstance == null)
				return;
			transactionInstance.InternalCommitAppointmentsTransaction(new Appointment[] { sourceApt }, type);
		}		
		void InternalStartAppointmentsTransaction() {
			ISupportsAppointmentTransaction transactionInstance = InnerAppointments as ISupportsAppointmentTransaction;
			if (transactionInstance == null)
				return;
			transactionInstance.InternalStartAppointmentsTransaction();
		}
		protected void ApplyChangesInternal(AppointmentType initialSourceAppointmentType) {
			SetActualTimeZone();
			ApplySourceAppointmentType(initialSourceAppointmentType);
			ApplyChangesCore();
			if (((IInternalAppointment)sourceApt).OnContentChanging(String.Empty, sourceApt, sourceApt))
				((IInternalAppointment)sourceApt).OnContentChanged();
			CreateAppointmentCopies();
		}
		private void SetActualTimeZone() {
			if (!TimeZoneVisible && !String.IsNullOrEmpty(TimeZoneId))
				TimeZoneId = string.Empty;
		}
		protected void PrepareActualSourceAppointment() {
			sourceApt = GetActualSourceAppointment();
		}
		protected internal virtual Appointment GetActualSourceAppointment() {
			if (!sourceApt.IsBase && editedPattern == null)
				return sourceApt.RecurrencePattern;
			else
				return sourceApt;
		}
		protected internal virtual void ApplySourceAppointmentType(AppointmentType initialSourceAppointmentType) {
			if (SchedulerUtils.IsBaseType(initialSourceAppointmentType)) {
				((IInternalAppointment)sourceApt).SetTypeCore(editedAptCopy.Type);
			} else {
				if (editedPattern == null)
					((IInternalAppointment)sourceApt).SetTypeCore(AppointmentType.Normal);
			}
		}
		protected internal virtual void ApplyChangesCore() {
			AppointmentFormAppointmentCopyHelper helper = CreateAppointmentFormAppointmentCopyHelper();
			helper.AssignRecurrenceProperties(editedPattern, sourceApt);
			helper.AssignSimpleProperties(editedAptCopy, sourceApt);
			helper.AssignCollectionProperties(editedAptCopy, sourceApt);
			ApplyCustomFieldsValues();
		}
		protected virtual AppointmentFormAppointmentCopyHelper CreateAppointmentFormAppointmentCopyHelper() {
			return new AppointmentFormAppointmentCopyHelper(this);
		}
		protected virtual void ApplyCustomFieldsValues() {
		}
		public virtual void DeleteAppointment() {
			if (!IsNewAppointment) {
				DeleteAppointmentsSimpleCommand command = new DeleteAppointmentsSimpleCommand(InnerControl, sourceApt);
				command.ExecuteCore();
			}
		}
		protected internal virtual bool IsCustomFieldsEqual(string customFieldName, Appointment apt1, Appointment apt2) {
			return Object.Equals(apt1.CustomFields.GetFieldByName(customFieldName).Value, apt2.CustomFields.GetFieldByName(customFieldName).Value);
		}
		protected internal virtual void AssignCustomField(string customFieldName, Appointment dest, Appointment source) {
			dest.CustomFields.GetFieldByName(customFieldName).Value = source.CustomFields.GetFieldByName(customFieldName).Value;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler propertyChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentFormControllerBasePropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged { add { propertyChanged += value; } remove { propertyChanged -= value; } }
		protected virtual void NotifyPropertyChanged(String info) {
			if (propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
		public void AssignRecurrenceInfoRangeProperties(RecurrenceInfo rinfo, RecurrenceRange range, DateTime start, DateTime end, int occurrenceCount, Appointment pattern) {
			((IInternalRecurrenceInfo)rinfo).UpdateRange(start, end, range, occurrenceCount, pattern);
		}
		protected virtual void SubscribeEditedAptCopyEvents() {
			if (this.editedAptCopy == null)
				return;
			this.editedAptCopy.ResourceIds.CollectionChanged += OnResourceIdsCollectionChanged;
		}
		void OnResourceIdsCollectionChanged(object sender, Utils.CollectionChangedEventArgs<object> e) {
			NotifyPropertyChanged("ResourceIds");
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public class AppointmentFormAppointmentCopyHelper : AppointmentCopyHelper {
		AppointmentFormControllerBase controller;
		public AppointmentFormAppointmentCopyHelper(AppointmentFormControllerBase controller) {
			if (controller == null)
				Exceptions.ThrowArgumentNullException("controller");
			this.controller = controller;
		}
		public AppointmentFormControllerBase Controller { get { return controller; } }
		protected internal override void AssignRecurrenceProperties(Appointment src, Appointment target) {
			if (target.Type == AppointmentType.Pattern && src != null) {
				TimeInterval srcInterval = ((IInternalAppointment)src).GetInterval();
				TimeInterval targetInterval = ((IInternalAppointment)target).GetInterval();
				if (!(targetInterval.Equals(srcInterval) && target.RecurrenceInfo.Equals(src.RecurrenceInfo)))
					DeleteExceptions(target);
				ExceptionPropertiesMerger margerer = new ExceptionPropertiesMerger(target, src, Controller);
				AppointmentBaseCollection exceptions = target.GetExceptions();
				foreach (Appointment exception in exceptions) {
					margerer.Apply(exception);
				}
				target.RecurrenceInfo.Assign(src.RecurrenceInfo);
			}
		}
		protected virtual void DeleteExceptions(Appointment appointment) {
			appointment.DeleteExceptions();
		}
	}
}
