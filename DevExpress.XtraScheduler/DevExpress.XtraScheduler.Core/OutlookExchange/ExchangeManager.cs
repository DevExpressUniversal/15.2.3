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
using System.Linq;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Outlook;
using DevExpress.XtraScheduler.Outlook.Interop;
using OutlookInterop = DevExpress.XtraScheduler.Outlook.Interop;
using System.Collections.Generic;
using DevExpress.XtraScheduler.OutlookExchange;
using DevExpress.XtraScheduler.OutlookExchange.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Outlook.Native {
	#region OutlookExchangeManager (abstract class)
	[CLSCompliant(false)]
	public abstract class OutlookExchangeManager {
		public const int DefaultReminderMinutesBeforeStart = 5;
		#region Fields
		_Application outlookApp;
		_Items calendarItems;
		List<_AppointmentItem> appointmentsToExchange;
		AppointmentExchanger exchanger;
		IOutlookCalendarProvider calendarProvider;
		OutlookVersions outlookVersion;
		List<Appointment> notSuitableForExportAppointments;
		#endregion
		protected OutlookExchangeManager(AppointmentExchanger exchanger) {
			this.exchanger = exchanger;
			this.notSuitableForExportAppointments = new List<Appointment>();
		}
		#region Properties
		protected internal _Application OutlookApp { get { return outlookApp; } }
		protected internal _Items CalendarItems { get { return calendarItems; } }
		protected internal List<_AppointmentItem> AppointmentsToExchange { get { return appointmentsToExchange; } }
		protected internal OutlookVersions OutlookVersion { get { return this.outlookVersion; } }
		protected internal List<Appointment> NotSuitableForExportAppointments { get { return this.notSuitableForExportAppointments; } }
		public IOutlookCalendarProvider CalendarProvider {
			get {
				if (calendarProvider == null) {
					calendarProvider = CreateOutlookCalendarProvider();
				}
				return calendarProvider;
			}
			set {
				if (value == CalendarProvider)
					return;
				this.calendarProvider = value;
			}
		}
		protected internal virtual IOutlookCalendarProvider CreateOutlookCalendarProvider() {
			return new OutlookCalendarProvider();
		}
		protected internal bool ReadyForExchange { get { return OutlookApp != null && CalendarItems != null; } }
		public AppointmentExchanger Exchanger { get { return exchanger; } }
		public ISchedulerStorageBase Storage { get { return Exchanger.Storage; } }
		public IAppointmentStorageBase Appointments { get { return Storage.Appointments; } }
		#endregion
		public void Exchange() {
			InitializeExchange();
			try {
				if (ReadyForExchange)
					PerformExchange();
			} finally {
				FinalizeExchange();
			}
		}
		protected internal virtual void InitializeExchange() {
			string calendarFolderName = string.Empty;
			if (Exchanger is ISupportCalendarFolders)
				calendarFolderName = ((ISupportCalendarFolders)Exchanger).CalendarFolderName;
			this.outlookApp = CalendarProvider.GetOutlookApplication();
			GetOutlookVersion();
			this.calendarItems = CalendarProvider.GetCalendarItems(OutlookApp, calendarFolderName);
			this.appointmentsToExchange = CalendarProvider.PrepareItemsForExchange(CalendarItems);
		}
		void GetOutlookVersion() {
			int version = int.MinValue;
			if (OutlookApp != null)
				Int32.TryParse(OutlookApp.Version.Substring(0, OutlookApp.Version.IndexOf('.')), out version);
			if (version >= 15 || version == int.MinValue)
				this.outlookVersion = OutlookVersions.OlderThen2013;
			else
				this.outlookVersion = OutlookVersions.Under2013;
		}
		protected internal virtual void FinalizeExchange() {
			try {
				AppointmentsToExchange.Clear();
				this.notSuitableForExportAppointments.Clear();
				this.appointmentsToExchange = null;
				ReleaseCalendarItems();
			} catch {
			}
			ReleaseOutlookObject(outlookApp);
			this.outlookApp = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		protected virtual void ReleaseCalendarItems() {
			IEnumerator en = CalendarItems.GetEnumerator();
			while (en.MoveNext())
				ReleaseOutlookObject(en.Current);
			ReleaseOutlookObject(CalendarItems);
			this.calendarItems = null;
		}
		protected internal void ReleaseOutlookObjects(IList<_AppointmentItem> objectList) {
			int count = objectList.Count;
			for (int i = 0; i < count; i++) {
				ReleaseOutlookObject(objectList[i]);
			}
		}
		protected internal virtual void ReleaseOutlookObject(object obj) {
			OutlookUtils.ReleaseOutlookObject(obj);
		}
		protected internal virtual Appointment CreateAppointment(bool pattern) {
			AppointmentType type = pattern ? AppointmentType.Pattern : AppointmentType.Normal;
			return SchedulerUtils.CreateAppointmentInstance(Storage, type);
		}
		protected internal virtual Appointment GetOccurrence(Appointment pattern, int recurrenceIndex) {
			return pattern.GetOccurrence(recurrenceIndex);
		}
		protected internal virtual Appointment CreateException(Appointment pattern, bool deletedOccurrence, int recurrenceIndex) {
			AppointmentType type = deletedOccurrence ? AppointmentType.DeletedOccurrence : AppointmentType.ChangedOccurrence;
			return pattern.CreateException(type, recurrenceIndex);
		}
		protected internal virtual void PerformDeleteAppointment(Appointment apt) {
			apt.Delete();
		}
		protected internal virtual void DeleteAppointment(Appointment apt) {
			if (apt.Type == AppointmentType.ChangedOccurrence)
				PerformDeleteAppointment(apt);
			PerformDeleteAppointment(apt);
		}
		protected internal virtual void InitAppointmentProperties(Appointment apt, _AppointmentItem olApt, IAppointmentStatusStorage statuses) {
			apt.AllDay = false;
			string localTimeZoneId = TimeZoneInfo.Local.Id;
			apt.Start = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.ToOperationTime(olApt.Start, localTimeZoneId);
			apt.End = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.ToOperationTime(olApt.End, localTimeZoneId);
			apt.AllDay = olApt.AllDayEvent;
			apt.Subject = olApt.Subject;
			apt.Location = olApt.Location;
			try {
				apt.Description = olApt.Body;
			} catch {
			}
			apt.HasReminder = olApt.ReminderSet;
			if (apt.HasReminder)
				apt.Reminder.TimeBeforeStart = TimeSpan.FromMinutes(olApt.ReminderMinutesBeforeStart);
			AppointmentStatusType statusType = OutlookUtils.ConvertToAppointmentStatus(olApt.BusyStatus);
			apt.StatusKey = statuses.GetByType(statusType).Id;
			if (apt.Type == AppointmentType.Pattern) {
				InitAppointmentPatternProperties(apt, olApt);
			}
			#region label search
			#endregion
		}
		protected internal virtual void InitAppointmentPatternProperties(Appointment pattern, _AppointmentItem olApt) {
			RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
			try {
				IRecurrenceInfo rinfo = pattern.RecurrenceInfo;
				_TimeZone startTimeZone = ObtainStartTimeZone(olApt);
				InitRecurrenceInfo(rinfo, olPattern, pattern.AllDay, startTimeZone);
				pattern.Start = ToLocalTime(rinfo.Start, startTimeZone);
				pattern.Duration = rinfo.Duration;
			} finally {
				ReleaseOutlookObject(olPattern);
				olPattern = null;
			}
		}
		_TimeZone ObtainStartTimeZone(_AppointmentItem olApt) {
			_TimeZone result = null;
			try {
				result = olApt.StartTimeZone;
			} catch {
			}
			return result;
		}
		DateTime ToLocalTime(DateTime date, _TimeZone timeZone) {
			if (timeZone == null)
				return date;
			return ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.ToOperationTime(date, timeZone.ID);
		}
		protected internal virtual void InitRecurrenceInfo(IRecurrenceInfo rinfo, RecurrencePattern olPattern, bool allDayPattern, _TimeZone startTimeZone) {
			rinfo.Start = olPattern.PatternStartDate;
			if (!allDayPattern) {
				DateTime localStart = ToLocalTime(olPattern.StartTime, startTimeZone);
				rinfo.Start += localStart.TimeOfDay;
			}
			rinfo.Duration = TimeSpan.FromMinutes(olPattern.Duration);
			if (olPattern.NoEndDate) {
				rinfo.Range = RecurrenceRange.NoEndDate;
			} else {
				rinfo.Range = RecurrenceRange.OccurrenceCount;
				rinfo.OccurrenceCount = olPattern.Occurrences;
			}
			switch (olPattern.RecurrenceType) {
				case OlRecurrenceType.olRecursDaily:
					rinfo.Type = RecurrenceType.Daily;
					rinfo.Periodicity = olPattern.Interval;
					rinfo.WeekDays = WeekDays.EveryDay;
					break;
				case OlRecurrenceType.olRecursWeekly: {
						int periodicity = olPattern.Interval;
						if (periodicity == 0) {
							rinfo.Type = RecurrenceType.Daily;
							rinfo.WeekDays = WeekDays.WorkDays;
							rinfo.Periodicity = 1;
						} else {
							rinfo.Type = RecurrenceType.Weekly;
							rinfo.Periodicity = olPattern.Interval;
							rinfo.WeekDays = (WeekDays)olPattern.DayOfWeekMask;
						}
						break;
					}
				case OlRecurrenceType.olRecursMonthly:
					rinfo.Type = RecurrenceType.Monthly;
					rinfo.DayNumber = olPattern.DayOfMonth;
					rinfo.Periodicity = olPattern.Interval;
					rinfo.WeekOfMonth = WeekOfMonth.None;
					break;
				case OlRecurrenceType.olRecursMonthNth:
					rinfo.Type = RecurrenceType.Monthly;
					rinfo.WeekOfMonth = (WeekOfMonth)olPattern.Instance;
					rinfo.WeekDays = (WeekDays)olPattern.DayOfWeekMask;
					rinfo.Periodicity = olPattern.Interval;
					break;
				case OlRecurrenceType.olRecursYearly:
					rinfo.Type = RecurrenceType.Yearly;
					rinfo.Month = olPattern.MonthOfYear;
					rinfo.DayNumber = olPattern.DayOfMonth;
					rinfo.Periodicity = 1;
					rinfo.WeekOfMonth = WeekOfMonth.None;
					break;
				case OlRecurrenceType.olRecursYearNth:
					rinfo.Type = RecurrenceType.Yearly;
					rinfo.Month = olPattern.MonthOfYear;
					rinfo.WeekOfMonth = (WeekOfMonth)olPattern.Instance;
					rinfo.WeekDays = (WeekDays)olPattern.DayOfWeekMask;
					rinfo.Periodicity = 1;
					break;
			}
		}
		protected internal virtual _AppointmentItem CreateOutlookAppointment() {
			return CalendarItems.Add(OlItemType.olAppointmentItem) as _AppointmentItem;
		}
		protected internal virtual void DeleteOutlookAppointment(_AppointmentItem olApt) {
			olApt.Delete();
		}
		protected internal virtual void CommitOutlookAppointment(_AppointmentItem olApt) {
			olApt.Save();
		}
		protected internal virtual RecurrencePattern GetOutlookRecurrencePattern(_AppointmentItem olApt) {
			return olApt.GetRecurrencePattern();
		}
		protected internal virtual OutlookInterop.Exceptions GetOutlookPatternExceptions(RecurrencePattern olPattern) {
			return olPattern.Exceptions;
		}
		protected internal virtual _AppointmentItem GetOutlookOccurrence(RecurrencePattern olPattern, DateTime startDate) {
			return olPattern.GetOccurrence(startDate);
		}
		protected virtual void ClearOutlookRecurrencePattern(_AppointmentItem olApt) {
			olApt.ClearRecurrencePattern();
		}
		protected internal virtual void InitOutlookAppointmentProperties(_AppointmentItem olApt, Appointment apt, IAppointmentStatusStorage statuses) {
			string localTimeZoneId = TimeZoneInfo.Local.Id;
			olApt.Start = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.FromOperationTime(apt.Start, localTimeZoneId);
			olApt.End = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.FromOperationTime(apt.End, localTimeZoneId);
			olApt.Subject = apt.Subject;
			olApt.AllDayEvent = apt.AllDay;
			olApt.Location = apt.Location;
			olApt.Body = apt.Description;
			olApt.ReminderSet = apt.HasReminder;
			olApt.ReminderMinutesBeforeStart = (apt.HasReminder) ? DateTimeHelper.Divide(apt.Reminder.TimeBeforeStart, TimeSpan.FromMinutes(1)) : DefaultReminderMinutesBeforeStart;
			AppointmentStatusType statusType = statuses.GetById(apt.StatusKey).Type;
			olApt.BusyStatus = OutlookUtils.ConvertToOlBusyStatus(statusType);
			if (apt.Type == AppointmentType.Pattern) {
				InitRecurrencePatternProperties(olApt, apt);
			}
		}
		protected internal virtual void InitRecurrencePatternProperties(_AppointmentItem olApt, Appointment apt) {
			RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
			try {
				InitRecurrencePattern(olPattern, apt);
			} finally {
				ReleaseOutlookObject(olPattern);
				olPattern = null;
			}
		}
		protected internal virtual void InitRecurrencePattern(RecurrencePattern olPattern, Appointment sourcePattern) {
			IRecurrenceInfo rinfo = sourcePattern.RecurrenceInfo;
			string localTimeZoneId = TimeZoneInfo.Local.Id;
			TimeZoneEngine timeZoneEngine = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine;
			DateTime rStart = timeZoneEngine.FromOperationTime(rinfo.Start, localTimeZoneId);
			olPattern.PatternStartDate = rStart.Date;
			DateTime patternStart = timeZoneEngine.FromOperationTime(sourcePattern.Start, localTimeZoneId);
			DateTime patternaEnd = timeZoneEngine.FromOperationTime(sourcePattern.End, localTimeZoneId);
			olPattern.StartTime = new DateTime(patternStart.TimeOfDay.Ticks);
			olPattern.EndTime = new DateTime(patternaEnd.TimeOfDay.Ticks);
			switch (rinfo.Type) {
				case RecurrenceType.Daily:
					if (rinfo.WeekDays == WeekDays.EveryDay) {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursDaily;
						olPattern.Interval = GetValidPatternInterval(rinfo.Periodicity);
					} else {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursWeekly;
						olPattern.DayOfWeekMask = (OlDaysOfWeek)rinfo.WeekDays;
						olPattern.Interval = GetValidPatternInterval(1);
					}
					break;
				case RecurrenceType.Weekly:
					olPattern.RecurrenceType = OlRecurrenceType.olRecursWeekly;
					olPattern.DayOfWeekMask = (OlDaysOfWeek)rinfo.WeekDays;
					olPattern.Interval = GetValidPatternInterval(rinfo.Periodicity);
					break;
				case RecurrenceType.Monthly:
					if (rinfo.WeekOfMonth == WeekOfMonth.None) {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursMonthly;
						olPattern.DayOfMonth = rinfo.DayNumber;
						olPattern.Interval = GetValidPatternInterval(rinfo.Periodicity);
					} else {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursMonthNth;
						olPattern.DayOfWeekMask = (OlDaysOfWeek)rinfo.WeekDays;
						olPattern.Interval = GetValidPatternInterval(rinfo.Periodicity);
						olPattern.Instance = (int)rinfo.WeekOfMonth;
					}
					break;
				case RecurrenceType.Yearly:
					olPattern.Interval = GetValidPatternInterval(1);
					if (rinfo.WeekOfMonth == WeekOfMonth.None) {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursYearly;
						olPattern.DayOfMonth = rinfo.DayNumber;
						olPattern.MonthOfYear = rinfo.Month;
					} else {
						olPattern.RecurrenceType = OlRecurrenceType.olRecursYearNth;
						olPattern.MonthOfYear = rinfo.Month;
						olPattern.DayOfWeekMask = (OlDaysOfWeek)rinfo.WeekDays;
						olPattern.Instance = (int)rinfo.WeekOfMonth;
					}
					break;
			}
			if (rinfo.Range == RecurrenceRange.NoEndDate)
				olPattern.NoEndDate = true;
			else if (rinfo.Range == RecurrenceRange.EndByDate)
				olPattern.PatternEndDate = rinfo.End;
			else
				olPattern.Occurrences = rinfo.OccurrenceCount;
		}
		protected internal int GetValidPatternInterval(int interval) {
			return Math.Max(1, interval);
		}
		protected internal virtual int CalculateSourceObjectCount() {
			InitializeExchange();
			try {
				if (ReadyForExchange)
					return CalculateSourceObjectCountCore();
				else
					return 0;
			} finally {
				FinalizeExchange();
			}
		}
		protected internal virtual int CalculateSourceObjectCountCore() {
			int count = AppointmentsToExchange.Count;
			int result = count;
			for (int i = 0; i < count; i++) {
				_AppointmentItem olApt = AppointmentsToExchange[i];
				if (olApt != null)
					try {
						if (olApt.IsRecurring)
							result += CalcPatternExceptionsCount(olApt);
					} finally {
						ReleaseOutlookObject(olApt);
						olApt = null;
					}
			}
			return result;
		}
		protected internal virtual int CalcPatternExceptionsCount(_AppointmentItem olApt) {
			RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
			try {
				OutlookInterop.Exceptions exceptions = GetOutlookPatternExceptions(olPattern);
				int count = exceptions.Count;
				return count;
			} finally {
				ReleaseOutlookObject(olPattern);
				olPattern = null;
			}
		}
		protected internal abstract void PerformExchange();
	}
	#endregion
	[CLSCompliant(false)]
	public interface IOutlookItemExchanger {
		OutlookExchangeInfo CreateExchangeInfo(Appointment apt, _AppointmentItem olApt, SynchronizeOperation defaultOperation);
		void AfterExchangeItem(Appointment apt, _AppointmentItem olApt);
		void HandleException(System.Exception e, Appointment apt, _AppointmentItem olApt);
	}
	#region ExportManager
	[CLSCompliant(false)]
	public class ExportManager : OutlookExchangeManager {
		public ExportManager(AppointmentExchanger exporter)
			: base(exporter) {
			XtraSchedulerDebug.Assert(exporter as IOutlookItemExchanger != null);
		}
		public IOutlookItemExchanger ItemExporter { get { return (IOutlookItemExchanger)base.Exchanger; } }
		protected internal override void PerformExchange() {
			ExportAppointments(Appointments.Items);
		}
		protected internal virtual OutlookExchangeInfo PrepareExportInfo(Appointment apt, _AppointmentItem olApt) {
			return ItemExporter.CreateExchangeInfo(apt, olApt, SynchronizeOperation.Create);
		}
		protected internal virtual bool CanExportAppointment(OutlookExchangeInfo info, Appointment apt) {
			return !info.Termination && !info.Cancel && apt != null;
		}
		protected internal virtual void CommitExchange(Appointment apt, _AppointmentItem olApt) {
			CommitOutlookAppointment(olApt);
			AfterExchange(apt, olApt);
		}
		protected internal virtual void AfterExchange(Appointment apt, _AppointmentItem olApt) {
			ItemExporter.AfterExchangeItem(apt, olApt);
		}
		protected internal virtual void ExportAppointments(AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				DoExportAppointment(appointments[i]);
			}
		}
		protected internal virtual void DoExportAppointment(Appointment apt) {
			_AppointmentItem olApt = CreateOutlookAppointment();
			if (olApt == null)
				return;
			try {
				try {
					InitOutlookAppointmentProperties(olApt, apt, Appointments.Statuses);
					ExportAppointment(apt, olApt);
				} catch (System.Exception e) {
					ItemExporter.HandleException(e, apt, olApt);
				}
			} finally {
				ReleaseOutlookObject(olApt);
				olApt = null;
			}
		}
		protected internal virtual void ExportAppointment(Appointment apt, _AppointmentItem olApt) {
			OutlookExchangeInfo info = PrepareExportInfo(apt, olApt);
			if (CanExportAppointment(info, apt)) {
				ExportAppointmentCore(apt, olApt);
			}
		}
		protected internal virtual void ExportAppointmentCore(Appointment apt, _AppointmentItem olApt) {
			CommitExchange(apt, olApt);
			if (apt.HasExceptions)
				ExportPatternExceptions(apt, olApt);
		}
		protected internal virtual void ExportPatternExceptions(Appointment pattern, _AppointmentItem olApt) {
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			int count = exceptions.Count;
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				if (exceptions[i].Type == AppointmentType.DeletedOccurrence)
					ExportPatternException(exceptions[i], olApt);
			}
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				if (exceptions[i].Type == AppointmentType.ChangedOccurrence)
					ExportPatternException(exceptions[i], olApt);
			}
		}
		protected internal virtual void ExportPatternException(Appointment exception, _AppointmentItem olApt) {
			Appointment pattern = exception.RecurrencePattern;
			RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
			try {
				DateTime start = OutlookUtils.CalculateOccurrenceStart(pattern.RecurrenceInfo, exception.RecurrenceIndex);
				_AppointmentItem olOccurrence = GetOutlookOccurrence(olPattern, start);
				try {
					try {
						ExportPatternExceptionCore(exception, olOccurrence);
					} catch (System.Exception e) {
						ItemExporter.HandleException(e, exception, olApt);  
					}
				} finally {
					ReleaseOutlookObject(olOccurrence);
					olOccurrence = null;
				}
			} finally {
				ReleaseOutlookObject(olPattern);
				olPattern = null;
			}
		}
		protected internal virtual void ExportPatternExceptionCore(Appointment exception, _AppointmentItem olException) {
			if (exception.Type == AppointmentType.DeletedOccurrence)
				ExportDeletedOccurrence(exception, olException);
			else {
				InitOutlookAppointmentProperties(olException, exception, Appointments.Statuses);
				ExportAppointment(exception, olException);
			}
		}
		protected internal virtual void ExportDeletedOccurrence(Appointment exception, _AppointmentItem olException) {
			OutlookExchangeInfo info = PrepareExportInfo(exception, olException);
			if (CanExportAppointment(info, exception)) {
				ExportAppointmentCore(exception, olException);
				DeleteOutlookAppointment(olException);
			}
		}
	}
	#endregion
	[CLSCompliant(false)]
	public class ImportManager : OutlookExchangeManager {
		public ImportManager(AppointmentExchanger importer)
			: base(importer) {
		}
		public OutlookImport Importer { get { return (OutlookImport)base.Exchanger; } }
		protected internal override void PerformExchange() {
			Appointments.BeginUpdate();
			try {
				ImportOutlookAppointments();
			} finally {
				Appointments.EndUpdate();
			}
		}
		protected internal virtual OutlookExchangeInfo PrepareImportInfo(Appointment apt, _AppointmentItem olApt, bool isNewObject) {
			return Importer.CreateImportInfo(apt, olApt);
		}
		protected internal virtual bool CanImportOutlookAppointment(OutlookExchangeInfo info, _AppointmentItem olApt) {
			return !info.Termination && olApt != null && !info.Cancel;
		}
		protected internal virtual void CommitExchange(Appointment apt, _AppointmentItem olApt) {
			Appointments.Add(apt);
			AfterExchange(apt, olApt);
		}
		protected internal virtual void AfterExchange(Appointment apt, _AppointmentItem olApt) {
			Importer.AfterImport(apt, olApt);
		}
		protected internal virtual void ImportOutlookAppointments() {
			int count = AppointmentsToExchange.Count;
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				_AppointmentItem olApt = AppointmentsToExchange[i];
				if (olApt != null)
					try {
						DoImportOutlookAppointment(olApt);
					} finally {
						ReleaseOutlookObject(olApt);
						olApt = null;
					}
			}
		}
		protected internal virtual void DoImportOutlookAppointment(_AppointmentItem olApt) {
			Appointment apt = null;
			try {
				apt = CreateAppointment(olApt.IsRecurring);
				InitAppointmentProperties(apt, olApt, Appointments.Statuses);
				ImportOutlookAppointment(olApt, apt);
			} catch (System.Exception e) {
				Importer.HandleException(e, apt, olApt);
			}
		}
		protected internal virtual void ImportOutlookAppointment(_AppointmentItem olApt, Appointment apt) {
			OutlookExchangeInfo info = PrepareImportInfo(apt, olApt, true);
			if (CanImportOutlookAppointment(info, olApt)) {
				ImportOutlookAppointmentCore(olApt, apt);
			}
		}
		protected internal virtual void ImportOutlookAppointmentCore(_AppointmentItem olApt, Appointment apt) {
			CommitExchange(apt, olApt);
			if (olApt.IsRecurring) {
				RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
				try {
					if (base.OutlookVersion == OutlookVersions.Under2013)
						ImportOutlookPatternExceptions(olApt, olPattern, apt);
					else
						ImportOutlook2013PatternExceptions(olApt, olPattern, apt);
				} finally {
					ReleaseOutlookObject(olPattern);
					olPattern = null;
				}
			}
		}
		protected internal virtual void ImportOutlook2013PatternExceptions(_AppointmentItem olPatternApt, RecurrencePattern olPattern, Appointment pattern) {
			OutlookInterop.Exceptions exceptions = GetOutlookPatternExceptions(olPattern);
			int count = exceptions.Count;
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			List<DateTime> changedStartDates = new List<DateTime>();
			List<DateTime> deletedDates = new List<DateTime>();
			for (int i = 1; i <= count; i++) {
				if (exceptions[i].Deleted)
					deletedDates.Add(exceptions[i].OriginalDate.Date);
				else
					changedStartDates.Add(exceptions[i].OriginalDate.Date);
			}
			for (int i = 1; i <= count; i++) {
				if (Exchanger.IsTermination)
					break;
				OutlookInterop.Exception olException = exceptions[i];
				if (exceptions[i].Deleted && changedStartDates.Contains(exceptions[i].OriginalDate.Date))
					continue;
				try {
					DateTime originalStart = olException.OriginalDate.Date;
					int originIndex = calc.FindFirstOccurrenceIndex(new TimeInterval(originalStart, TimeSpan.FromDays(1)), pattern);
					int destinationIndex = -1;
					bool notSameDayChangedException = IsNotSameDayChangedException(olException);
					if (notSameDayChangedException)
						destinationIndex = calc.FindFirstOccurrenceIndex(new TimeInterval(exceptions[i].AppointmentItem.Start.Date, TimeSpan.FromDays(1)), pattern);
					try {
						ImportOutlookPatternException(olPatternApt, olException, pattern, originIndex);
						if (destinationIndex != -1) {
							for (int t = Math.Min(originIndex, destinationIndex); t <= Math.Max(originIndex, destinationIndex); t++) {
								DateTime occurenceDate = calc.CalcOccurrenceByIndex(t, pattern).Start.Date;
								if (!changedStartDates.Contains(occurenceDate) && !deletedDates.Contains(occurenceDate))
									ImportOutlookDeletedOccurrence(olPatternApt, null, pattern, t);
							}
						}
					} catch (System.Exception e) {
						Importer.HandleException(e, pattern, olPatternApt);   
					}
				} finally {
					ReleaseOutlookObject(olException);
					olException = null;
				}
			}
		}
		protected internal virtual void ImportOutlookPatternExceptions(_AppointmentItem olPatternApt, RecurrencePattern olPattern, Appointment pattern) {
			OutlookInterop.Exceptions exceptions = GetOutlookPatternExceptions(olPattern);
			int count = exceptions.Count;
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			for (int i = 1; i <= count; i++) {
				if (Exchanger.IsTermination)
					break;
				OutlookInterop.Exception olException = exceptions[i];
				try {
					DateTime originalStart = olException.OriginalDate.Date;
					int originIndex = calc.FindFirstOccurrenceIndex(new TimeInterval(originalStart, TimeSpan.FromDays(1)), pattern);
					int destinationIndex = originIndex;
					bool notSameDayChangedException = IsNotSameDayChangedException(olException);
					if (notSameDayChangedException) {
						TimeInterval interval = CalculateShiftedExceptionInterval(originalStart, olException.AppointmentItem.Start.Date);
						destinationIndex = calc.FindFirstOccurrenceIndex(interval, pattern);
					}
					try {
						ImportOutlookPatternException(olPatternApt, olException, pattern, destinationIndex);
						if (notSameDayChangedException && originIndex != destinationIndex) {
							ImportOutlookDeletedOccurrence(olPatternApt, null, pattern, originIndex);
						}
					} catch (System.Exception e) {
						Importer.HandleException(e, pattern, olPatternApt);   
					}
				} finally {
					ReleaseOutlookObject(olException);
					olException = null;
				}
			}
		}
		protected bool IsNotSameDayChangedException(OutlookInterop.Exception olException) {
			return !olException.Deleted && olException.OriginalDate.Date != olException.AppointmentItem.Start.Date;
		}
		protected TimeInterval CalculateShiftedExceptionInterval(DateTime originalStart, DateTime start) {
			return start < originalStart ? new TimeInterval(start, originalStart.AddDays(1)) :
				new TimeInterval(originalStart, start.AddDays(1));
		}
		protected Appointment FindConflictedException(Appointment pattern, int recurrenceIndex) {
			if (!pattern.HasExceptions)
				return null;
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			for (int i = 0; i < exceptions.Count; i++) {
				if (exceptions[i].RecurrenceIndex == recurrenceIndex) {
					return exceptions[i];
				}
			}
			return null;
		}
		protected internal virtual void ImportOutlookPatternException(_AppointmentItem olPatternApt, OutlookInterop.Exception olException, Appointment pattern, int recurrenceIndex) {
			if (olException.Deleted)
				ImportOutlookDeletedOccurrence(olPatternApt, olException, pattern, recurrenceIndex);
			else
				ImportOutlookChangedOccurrence(olPatternApt, olException, pattern, recurrenceIndex);
		}
		protected internal virtual void ImportOutlookDeletedOccurrence(_AppointmentItem olPatternApt, OutlookInterop.Exception olException, Appointment pattern, int recurrenceIndex) {
			Appointment exception = CreateException(pattern, true, recurrenceIndex);
			ImportOutlookPatternExceptionCore(olPatternApt, exception, true);
		}
		protected internal virtual void ImportOutlookChangedOccurrence(_AppointmentItem olPatternApt, OutlookInterop.Exception olException, Appointment pattern, int recurrenceIndex) {
			Appointment exception = CreateException(pattern, false, recurrenceIndex);
			_AppointmentItem olApt = olException.AppointmentItem;
			try {
				InitAppointmentProperties(exception, olApt, Appointments.Statuses);
				ImportOutlookPatternExceptionCore(olApt, exception, true);
			} finally {
				ReleaseOutlookObject(olApt);
				olApt = null;
			}
		}
		protected internal virtual void ImportOutlookPatternExceptionCore(_AppointmentItem olException, Appointment exception, bool isNewObject) {
			OutlookExchangeInfo info = PrepareImportInfo(exception, olException, isNewObject);
			if (CanImportOutlookAppointment(info, olException)) {
				AfterExchange(exception, olException);
			} else {
				if (isNewObject) DeleteAppointment(exception);
			}
		}
	}
	[CLSCompliant(false)]
	public class ExportSynchronizeManager : ExportManager {
		AppointmentBinder binder;
		public ExportSynchronizeManager(AppointmentExchanger synchronizer)
			: base(synchronizer) {
			XtraSchedulerDebug.Assert(synchronizer as IOutlookItemExchanger != null);
			this.binder = CreateAppointmentBinder();
		}
		IOutlookItemExchanger ItemExchanger { get { return Exchanger as IOutlookItemExchanger; } }
		protected internal AppointmentBinder Binder { get { return binder; } }
		protected virtual AppointmentBinder CreateAppointmentBinder() {
			return new AppointmentBinder(Exchanger as IGetAppointmentForeignId);
		}
		protected internal override void PerformExchange() {
			PrepareSynchronization();
			CalcNotSuitableAppointments();
			SynchronizeOutlookAppointments();
			CreateNewOutlookAppointments();
			if (NotSuitableForExportAppointments.Count > 0)
				throw new System.Exception(String.Format("Export completed with {0} entries skipped due to recurrence rules not supported by MS Outlook.", NotSuitableForExportAppointments.Count));
		}
		protected internal virtual void CalcNotSuitableAppointments() {
			foreach (Appointment appointment in Appointments.Items) {
				if (appointment.IsRecurring) {
					if (appointment.RecurrenceInfo.Type == RecurrenceType.Hourly || appointment.RecurrenceInfo.Type == RecurrenceType.Minutely) {
						NotSuitableForExportAppointments.Add(appointment);
						continue;
					}
					if (IsCrossOccurrenceAppointment(appointment))
						NotSuitableForExportAppointments.Add(appointment);
				}
			}
		}
		bool IsCrossOccurrenceAppointment(Appointment appointment) {
			AppointmentBaseCollection exceptions = appointment.GetExceptions();
			if (exceptions.Count != 0) {
				OccurrenceCalculator calculator = OccurrenceCalculator.CreateInstance(appointment.RecurrenceInfo);
				foreach (Appointment changedOccurence in exceptions.Where(e => e.Type == AppointmentType.ChangedOccurrence)) {
					int destinationIndex = GetDestinationIndex(calculator.CalcOccurrenceByIndex(changedOccurence.RecurrenceIndex, appointment).Start.Date, changedOccurence.Start.Date, calculator, appointment);
					if (destinationIndex != -1 && changedOccurence.RecurrenceIndex != destinationIndex) {
						bool isForwardShifted = destinationIndex > changedOccurence.RecurrenceIndex;
						for (int j = Math.Min(changedOccurence.RecurrenceIndex, destinationIndex); j <= Math.Max(changedOccurence.RecurrenceIndex, destinationIndex); j++) {
							if (j == changedOccurence.RecurrenceIndex)
								continue;
							Appointment exception = exceptions.Find(e => e.RecurrenceIndex == j);
							if (exception == null || exception.Type == AppointmentType.Occurrence)
								return true;
							if (exception.Type == AppointmentType.ChangedOccurrence) {
								int exceptionDestinationIndex = GetDestinationIndex(calculator.CalcOccurrenceByIndex(exception.RecurrenceIndex, appointment).Start.Date, exception.Start.Date, calculator, appointment);
								if (isForwardShifted != exceptionDestinationIndex > destinationIndex)
									return true;
							}
						}
					}
				}
			}
			return false;
		}
		int GetDestinationIndex(DateTime originalDate, DateTime destinationDate, OccurrenceCalculator calculator, Appointment pattern) {
			if (originalDate > destinationDate)
				return calculator.FindFirstOccurrenceIndex(new TimeInterval(destinationDate, originalDate), pattern);
			else
				return calculator.FindLastOccurrenceIndex(new TimeInterval(originalDate, destinationDate.AddDays(1)), pattern);
		}
		protected internal virtual OutlookExchangeInfo PrepareSynchronizationInfo(Appointment apt, _AppointmentItem olApt) {
			return ItemExchanger.CreateExchangeInfo(apt, olApt, CalculateSynchronizeOperation(apt, olApt));
		}
		protected internal override OutlookExchangeInfo PrepareExportInfo(Appointment apt, _AppointmentItem olApt) {
			SynchronizeOperation defaultOperation = SynchronizeOperation.Create;
			return ItemExchanger.CreateExchangeInfo(apt, olApt, defaultOperation);
		}
		protected internal virtual SynchronizeOperation CalculateSynchronizeOperation(Appointment apt, _AppointmentItem olApt) {
			if (apt != null && olApt != null)
				return SynchronizeOperation.Replace;
			return (apt != null) ? SynchronizeOperation.Create : SynchronizeOperation.Delete;
		}
		protected internal override bool CanExportAppointment(OutlookExchangeInfo info, Appointment apt) {
			if (!base.CanExportAppointment(info, apt))
				return false;
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			if (IsSynchronization(syncInfo))
				return syncInfo.Operation == SynchronizeOperation.Replace || syncInfo.Operation == SynchronizeOperation.Create;
			else
				return syncInfo.Operation == SynchronizeOperation.Create;
		}
		protected bool IsSynchronization(OutlookSynchronizeInfo info) {
			return info.DefaultOperation == SynchronizeOperation.Replace;
		}
		protected internal override void AfterExchange(Appointment apt, _AppointmentItem olApt) {
			ItemExchanger.AfterExchangeItem(apt, olApt);
		}
		protected internal virtual bool CanDeleteOutlookAppointment(OutlookExchangeInfo info, _AppointmentItem olApt) {
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			return !info.Termination && !info.Cancel && olApt != null && syncInfo.Operation == SynchronizeOperation.Delete;
		}
		protected internal virtual bool CanReplaceOutlookAppointment(OutlookExchangeInfo info, Appointment apt, _AppointmentItem olApt) {
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			return !info.Termination && !info.Cancel && olApt != null && apt != null && syncInfo.Operation == SynchronizeOperation.Replace;
		}
		protected internal virtual void PrepareSynchronization() {
			binder.CreateOutlookEntryIdTable(Appointments.Items, true);
		}
		protected internal virtual void SynchronizeOutlookAppointments() {
			int count = AppointmentsToExchange.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (Exchanger.IsTermination)
					break;
				_AppointmentItem olApt = AppointmentsToExchange[i];
				if (olApt != null)
					try {
						SynchronizeOutlookAppointment(olApt);
					} finally {
						ReleaseOutlookObject(olApt);
						olApt = null;
					}
			}
		}
		protected internal virtual void SynchronizeOutlookAppointment(_AppointmentItem olApt) {
			if (olApt == null) return;
			string entryId = olApt.EntryID;
			try {
				Appointment apt = Binder.FindLinkedAppointment(entryId);
				if (NotSuitableForExportAppointments.Contains(apt))
					return;
				OutlookAppointmentModificationTracker modificationTracker = new OutlookAppointmentModificationTracker(olApt);
				OutlookExchangeInfo info = PrepareSynchronizationInfo(apt, olApt);
				if (info.Termination || info.Cancel)
					return;
				modificationTracker.FixChanged();
				if (CanDeleteOutlookAppointment(info, olApt)) {
					DeleteOutlookAppointment(olApt);
					AfterExchange(apt, null);
				}
				if (CanReplaceOutlookAppointment(info, apt, olApt)) {
					if (olApt.IsRecurring)
						RemoveOutlookPatternExceptions(olApt);
					InitOutlookAppointmentProperties(olApt, apt, Appointments.Statuses);
					modificationTracker.RestoreToFixed();
					ExportAppointmentCore(apt, olApt);
				}
			} finally {
				Binder.RemoveLink(entryId);
			}
		}
		protected internal virtual void CreateNewOutlookAppointments() {
			ExportAppointments(Binder.GetUnboundAppointments());
		}
		protected internal virtual void RemoveOutlookPatternExceptions(_AppointmentItem olApt) {
			ClearOutlookRecurrencePattern(olApt); 
			RecurrencePattern olPattern = GetOutlookRecurrencePattern(olApt);
			try {
				OutlookInterop.Exceptions olExceptions = GetOutlookPatternExceptions(olPattern);
				try {
					RemoveOutlookExceptions(olExceptions);
				} finally {
					ReleaseOutlookObject(olExceptions);
					olExceptions = null;
				}
			} finally {
				ReleaseOutlookObject(olPattern);
				olPattern = null;
			}
		}
		protected internal virtual void RemoveOutlookExceptions(OutlookInterop.Exceptions olExceptions) {
			int count = olExceptions.Count;
			for (int i = 1; i <= count; i++) {
				OutlookInterop.Exception exception = olExceptions[i];
				try {
					RemoveOutlookException(exception);
				} finally {
					ReleaseOutlookObject(exception);
					exception = null;
				}
			}
		}
		protected internal virtual void RemoveOutlookException(OutlookInterop.Exception exception) {
			if (exception.Deleted)
				return;
			_AppointmentItem olApt = exception.AppointmentItem;
			try {
				DeleteOutlookAppointment(olApt);
			} finally {
				ReleaseOutlookObject(olApt);
				olApt = null;
			}
		}
	}
	[CLSCompliant(false)]
	public class ImportSynchronizeManager : ImportManager {
		AppointmentBinder binder;
		public ImportSynchronizeManager(AppointmentExchanger synchronizer)
			: base(synchronizer) {
			this.binder = CreateAppointmentBinder();
		}
		protected OutlookImportSynchronizer Synchronizer { get { return (OutlookImportSynchronizer)base.Exchanger; } }
		protected internal AppointmentBinder Binder { get { return binder; } }
		protected virtual AppointmentBinder CreateAppointmentBinder() {
			return new AppointmentBinder(Synchronizer);
		}
		protected internal override void PerformExchange() {
			Appointments.BeginUpdate();
			try {
				PrepareSynchronization();
				SynchronizeAppointments();
				DeleteNonSynchronizedAppointments();
			} finally {
				Appointments.EndUpdate();
			}
		}
		protected internal virtual void PrepareSynchronization() {
			binder.CreateOutlookEntryIdTable(Appointments.Items, false);
		}
		protected internal override void AfterExchange(Appointment apt, _AppointmentItem olApt) {
			Synchronizer.AfterSynchronize(apt, olApt);
		}
		protected internal virtual OutlookExchangeInfo PrepareSynchronizationInfo(Appointment apt, _AppointmentItem olApt, bool isNewObject) {
			SynchronizeOperation defaultOperation = isNewObject ? SynchronizeOperation.Create : CalculateSynchronizeOperation(apt, olApt);
			return Synchronizer.CreateSynchronizeInfo(apt, olApt, defaultOperation);
		}
		protected internal override OutlookExchangeInfo PrepareImportInfo(Appointment apt, _AppointmentItem olApt, bool isNewObject) {
			SynchronizeOperation defaultOperation = isNewObject ? SynchronizeOperation.Create : SynchronizeOperation.Replace;
			return Synchronizer.CreateSynchronizeInfo(apt, olApt, defaultOperation);
		}
		protected internal virtual SynchronizeOperation CalculateSynchronizeOperation(Appointment apt, _AppointmentItem olApt) {
			if (apt != null && olApt != null)
				return SynchronizeOperation.Replace;
			return (olApt != null) ? SynchronizeOperation.Create : SynchronizeOperation.Delete;
		}
		protected internal virtual void SynchronizeAppointments() {
			int count = AppointmentsToExchange.Count;
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				_AppointmentItem olApt = AppointmentsToExchange[i];
				if (olApt != null)
					try {
						SynchronizeAppointment(olApt);
					} finally {
						ReleaseOutlookObject(olApt);
						olApt = null;
					}
			}
		}
		protected internal virtual void SynchronizeAppointment(_AppointmentItem olApt) {
			if (olApt == null) return;
			string entryId = olApt.EntryID;
			try {
				Appointment apt = Binder.FindLinkedAppointment(entryId);
				bool isNewObject = apt == null;
				if (isNewObject) {
					apt = CreateAppointment(olApt.IsRecurring);
					InitAppointmentProperties(apt, olApt, Appointments.Statuses);
				}
				OutlookExchangeInfo info = PrepareSynchronizationInfo(apt, olApt, isNewObject);
				if (info.Termination || info.Cancel) {
					if (isNewObject) DeleteAppointment(apt);
					return;
				}
				if (isNewObject) {
					bool success = ImportNewAppointment(info, apt, olApt);
					if (!success) DeleteAppointment(apt);
				} else
					ReplaceAppointment(info, apt, olApt);
			} finally {
				Binder.RemoveLink(entryId);
			}
		}
		protected internal virtual bool ImportNewAppointment(OutlookExchangeInfo info, Appointment apt, _AppointmentItem olApt) {
			if (CanCreateAppointment(info, apt)) {
				ImportOutlookAppointmentCore(olApt, apt);
				return true;
			}
			return false;
		}
		protected internal virtual void ReplaceAppointment(OutlookExchangeInfo info, Appointment apt, _AppointmentItem olApt) {
			if (CanDeleteAppointment(info, apt)) {
				DeleteAppointment(apt);
				AfterExchange(apt, olApt);
			}
			if (CanReplaceAppointment(info, apt, olApt)) {
				OutlookUtils.SetAppointmentType(apt, olApt.IsRecurring ? AppointmentType.Pattern : AppointmentType.Normal);
				InitAppointmentProperties(apt, olApt, Appointments.Statuses);
				ImportOutlookAppointmentCore(olApt, apt);
			}
		}
		protected internal virtual bool CanCreateAppointment(OutlookExchangeInfo info, Appointment apt) {
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			return !info.Termination && !info.Cancel && apt != null && syncInfo.Operation == SynchronizeOperation.Create;
		}
		protected internal virtual bool CanReplaceAppointment(OutlookExchangeInfo info, Appointment apt, _AppointmentItem olApt) {
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			return !info.Termination && !info.Cancel && olApt != null && apt != null && syncInfo.Operation == SynchronizeOperation.Replace;
		}
		protected internal virtual bool CanDeleteAppointment(OutlookExchangeInfo info, Appointment apt) {
			OutlookSynchronizeInfo syncInfo = (OutlookSynchronizeInfo)info;
			bool result = !info.Cancel && apt != null && syncInfo.Operation == SynchronizeOperation.Delete;
			if (apt.Type == AppointmentType.Pattern) {
				result &= !apt.HasExceptions;
			}
			return result;
		}
		protected internal virtual void DeleteNonSynchronizedAppointments() {
			AppointmentBaseCollection apts = SelectAppointmentsToDelete();
			int count = apts.Count;
			for (int i = 0; i < count; i++) {
				if (Exchanger.IsTermination)
					break;
				Appointment apt = apts[i];
				OutlookExchangeInfo info = PrepareSynchronizationInfo(apt, null, false);
				if (Exchanger.IsTermination)
					break;
				DoDeleteAppointment(info, apt, null);
			}
		}
		protected virtual AppointmentBaseCollection SelectAppointmentsToDelete() {
			AppointmentBaseCollection result = Binder.GetUnboundAppointments();
			result.Sort(new AppointmentsDeleteRuleComparer());
			return result;
		}
		protected internal virtual void DoDeleteAppointment(OutlookExchangeInfo info, Appointment apt, _AppointmentItem olApt) {
			if (CanDeleteAppointment(info, apt)) {
				DeleteAppointment(apt);
				AfterExchange(apt, olApt);
			}
		}
		protected internal virtual void SynchronizeDeletedOccurrence(_AppointmentItem olApt, OutlookInterop.Exception olException, Appointment pattern, Appointment apt, int recurrenceIndex) {
			OutlookExchangeInfo info = PrepareImportInfo(apt, olApt, false);
			if (CanDeleteAppointment(info, apt)) {
				Appointment argApt = SynchronizeByDeleteExceptionCore(pattern, apt, recurrenceIndex);
				AfterExchange(argApt, olApt);
			}
			if (CanReplaceAppointment(info, apt, olApt)) {
				if (apt.IsException && apt.Type != AppointmentType.DeletedOccurrence) {
					PerformDeleteAppointment(apt); 
				}
				AfterExchange(apt, olApt);
			}
		}
		protected internal virtual void SynchronizeChangedOccurrence(_AppointmentItem olApt, OutlookInterop.Exception olException, Appointment pattern, Appointment apt, int recurrenceIndex) {
			OutlookExchangeInfo info = PrepareImportInfo(apt, olApt, false);
			if (CanDeleteAppointment(info, apt)) {
				Appointment argApt = SynchronizeByDeleteExceptionCore(pattern, apt, recurrenceIndex);
				AfterExchange(argApt, olApt);
			}
			if (CanReplaceAppointment(info, apt, olApt)) {
				Appointment destApt = GetDestinationChangedOccurrence(apt, pattern, recurrenceIndex);
				InitAppointmentProperties(destApt, olApt, Appointments.Statuses);
				AfterExchange(destApt, olApt);
			}
		}
		private Appointment SynchronizeByDeleteExceptionCore(Appointment pattern, Appointment apt, int recurrenceIndex) {
			bool deletedOccurrence = apt.Type == AppointmentType.DeletedOccurrence;
			PerformDeleteAppointment(apt);
			Appointment argApt = apt;
			if (deletedOccurrence) {
				argApt = GetRestoredOccurrence(pattern, recurrenceIndex);
			}
			return argApt;
		}
		protected Appointment GetRestoredOccurrence(Appointment pattern, int recurrenceIndex) {
			return pattern != null ? pattern.GetOccurrence(recurrenceIndex) : null;
		}
		protected Appointment GetDestinationChangedOccurrence(Appointment originApt, Appointment pattern, int recurrenceIndex) {
			if (originApt.Type == AppointmentType.DeletedOccurrence) {
				originApt.RestoreOccurrence();
				return GetOccurrence(pattern, recurrenceIndex);
			}
			if (originApt.IsBase)
				return CreateException(pattern, false, recurrenceIndex);
			return originApt;
		}
		protected internal override void ImportOutlookChangedOccurrence(_AppointmentItem olPatternApt, DevExpress.XtraScheduler.Outlook.Interop.Exception olException, Appointment pattern, int recurrenceIndex) {
			string entryId = OutlookUtils.GetRecurrenceEntryId(olPatternApt.EntryID, recurrenceIndex);
			_AppointmentItem olApt = olException.AppointmentItem;
			try {
				Appointment apt = Binder.FindLinkedAppointment(entryId);
				if (apt == null) {
					apt = FindConflictedException(pattern, recurrenceIndex);
					if (apt != null) {
						Binder.RemoveAppointmentLinks(apt);
					}
				}
				if (apt != null) {
					SynchronizeChangedOccurrence(olApt, olException, pattern, apt, recurrenceIndex);
				} else {
					Appointment exception = CreateException(pattern, false, recurrenceIndex);
					ImportNewException(exception, olApt);
				}
			} finally {
				Binder.RemoveLink(entryId);
				ReleaseOutlookObject(olApt);
				olApt = null;
			}
		}
		protected internal override void ImportOutlookDeletedOccurrence(_AppointmentItem olPatternApt, DevExpress.XtraScheduler.Outlook.Interop.Exception olException, Appointment pattern, int recurrenceIndex) {
			string entryId = OutlookUtils.GetRecurrenceEntryId(olPatternApt.EntryID, recurrenceIndex);
			try {
				Appointment apt = Binder.FindLinkedAppointment(entryId);
				if (apt == null) {
					apt = FindConflictedException(pattern, recurrenceIndex);
					if (apt != null) {
						Binder.RemoveAppointmentLinks(apt);
					}
				}
				if (apt != null) {
					SynchronizeDeletedOccurrence(olPatternApt, olException, pattern, apt, recurrenceIndex);
				} else {
					Appointment exception = CreateException(pattern, true, recurrenceIndex);
					ImportNewException(exception, olPatternApt);
				}
			} finally {
				Binder.RemoveLink(entryId);
			}
		}
		protected internal virtual void ImportNewException(Appointment exception, _AppointmentItem olApt) {
			InitAppointmentProperties(exception, olApt, Appointments.Statuses);
			OutlookExchangeInfo info = PrepareImportInfo(exception, olApt, true);
			if (CanCreateAppointment(info, exception))
				AfterExchange(exception, olApt);
			else
				DeleteAppointment(exception);
		}
	}
	public enum OutlookVersions { Under2013, OlderThen2013 }
}
