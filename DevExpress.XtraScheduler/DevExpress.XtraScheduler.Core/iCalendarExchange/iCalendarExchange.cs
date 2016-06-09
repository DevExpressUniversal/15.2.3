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
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Linq;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Xml;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.Utils;
using DevExpress.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.iCalendar.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.iCalendar {
	#region iCalendarImporter
	public class iCalendarImporter : AppointmentImporter {
		#region Fields
		string customPropertyIdentifier = iCalendarSR.DXPropertyIdentifier;
		iCalendarContainer calendars;
		iCalendarParser parser;
		int innerSourceObjectCount = -1;
		PatternExceptionCreator exceptionCreator;
		PatternExceptionBinder binder;
		TimeZoneManager timeZoneManager;
		#endregion
		#region ctor
		public iCalendarImporter(ISchedulerStorageBase storage)
			: base(storage) {
			this.calendars = new iCalendarContainer();
			this.parser = CreateCalendarParser();
			this.parser.ParseError += new iCalendarParseErrorHandler(OnParseError);
			this.exceptionCreator = CreatePatternExceptionCreator();
			this.binder = new PatternExceptionBinder();
			this.timeZoneManager = new TimeZoneManager(((IInternalSchedulerStorageBase)storage).TimeZoneEngine);
			this.timeZoneManager.SetCustomizeTimeZoneMethod(OnCustomizeConvertedTimeZone);
		}
		protected virtual void OnParseError(object sender, iCalendarParseErrorEventArgs e) {
			iCalendarParseExceptionEventArgs args = new iCalendarParseExceptionEventArgs(e.OriginalException, e.Line.SourceLineText, e.Line.SourceLineNum);
			RaiseOnException(args);
			Parser.IsTermination = IsTermination;
			if (!args.Handled) 
				throw args.OriginalException;
		}
		#endregion
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarImporterCustomPropertyIdentifier")]
#endif
		public string CustomPropertyIdentifier { get { return customPropertyIdentifier; } set { customPropertyIdentifier = value; } }
		protected internal IAppointmentStorageBase Appointments { get { return Storage.Appointments; } }
		[DefaultValue(null)]
		public Encoding Encoding { get; set; }
		protected iCalendarParser Parser { get { return parser; } }
		protected internal iCalendarContainer Calendars { get { return calendars; } }
		protected PatternExceptionCreator ExceptionCreator { get { return exceptionCreator; } }
		protected internal PatternExceptionBinder Binder { get { return binder; } }
		protected internal TimeZoneManager TimeZoneManager { get { return timeZoneManager; } }
		#endregion
		public event CustomizeConvertedTimeZoneHandler CustomizeConvertedTimeZone;
		#region CreateParser
		protected virtual iCalendarParser CreateCalendarParser() {
			return new iCalendarParser(Calendars);
		}
		#endregion
		#region CreatePatternExceptionCreator
		protected virtual PatternExceptionCreator CreatePatternExceptionCreator() {
			return new PatternExceptionCreator(Storage);
		}
		#endregion
		#region ImportCore
		protected internal override void ImportCore(Stream stream) {
			Calendars.Clear();
			if (stream == null || stream == Stream.Null)
				return;
			using (MemoryStream streamCopy = CreateStreamCopy(stream)) {
				Encoding encoding = ObtainEncoding(streamCopy);
				using (StreamReader sr = new StreamReader(streamCopy, encoding)) {
					Parser.Parse(sr);
				}
			}
			RaiseCalendarStructureCreated();
			if (IsTermination)
				return;
			ProcessCalendars(Calendars);
		}
		Encoding ObtainEncoding(MemoryStream streamCopy) {
			if (Encoding != null)
				return Encoding;
			InternalEncodingDetector detector = new InternalEncodingDetector();
			Encoding actualEncoding = detector.Detect(streamCopy);
			if (actualEncoding == null)
				actualEncoding = Encoding.Default;
			return actualEncoding;
		}
		#region Events
		iCalendarStructureCreatedEventHandler onCalendarStructureCreated;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarImporterCalendarStructureCreated")]
#endif
		public event iCalendarStructureCreatedEventHandler CalendarStructureCreated { add { onCalendarStructureCreated += value; } remove { onCalendarStructureCreated -= value; } }
		protected internal virtual void RaiseCalendarStructureCreated() {
			if (onCalendarStructureCreated != null)
				onCalendarStructureCreated(this, new iCalendarStructureCreatedEventArgs(Calendars));
		}
		protected internal virtual iCalendarAppointmentImportingEventArgs CreateImportingEventArgs(Appointment apt, VEvent ev) {
			return new iCalendarAppointmentImportingEventArgs(apt, ev);
		}
		protected internal virtual iCalendarAppointmentImportedEventArgs CreateImportedEventArgs(Appointment apt, VEvent ev) {
			return new iCalendarAppointmentImportedEventArgs(apt, ev);
		}
		#endregion
		#endregion
		#region CalculateSourceObjectCount
		protected internal override int CalculateSourceObjectCount() {
			if (this.innerSourceObjectCount == -1)
				this.innerSourceObjectCount = CalculateCalendarsObjectCount(Calendars);
			return this.innerSourceObjectCount;
		}
		#endregion
		#region ProcessCalendars
		public void ProcessCalendars(iCalendarContainer calendars) {
			int count = calendars.Count;
			if (count == 0)
				RaiseExchangeException(new iCalendarInvalidFileFormatException());
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				ProcessCalendar(calendars[i]);
			}
		}
		protected virtual void RaiseExchangeException(iCalendarException innerException) {
			ExchangeExceptionEventArgs args = new ExchangeExceptionEventArgs(innerException);
			RaiseOnException(args);
			if (!args.Handled)
				throw innerException;
		}
		protected internal virtual int CalculateCalendarsObjectCount(iCalendarContainer calendars) {
			int result = 0;
			int count = calendars.Count;
			for (int i = 0; i < count; i++) {
				result += calendars[i].CalculateSourceObjectCount();
			}
			return result;
		}
		#endregion
		#region ProcessCalendar
		protected virtual void ProcessCalendar(iCalendarComponent calendar) {
			if (calendar.Version.Value != DevExpress.XtraScheduler.VCalendar.VCalendarConsts.VCalendar20Version)
				return;
			Binder.Clear();
			TimeZoneManager.Clear();
			TimeZoneManager.ClientTimeZone = calendar.ClientTimeZone;
			TimeZoneManager.CollectTimeZones(calendar.TimeZones);
			VEventCollection events = calendar.Events;
			if (events.Count <= 0)
				return;
			VEventCollection notImportedEvents = ImportEvents(events);
			ImportChangedOccurrences();
			if (notImportedEvents.Count > 0) {
				RaiseExchangeException(new iCalendarEventImportException(notImportedEvents));
			}
		}
		#endregion
		#region BeforeImport
		protected internal virtual iCalendarAppointmentImportingEventArgs BeforeImport(Appointment apt, VEvent ev) {
			if (apt.Type == AppointmentType.Pattern)
				Binder.RegisterEventPattern(ev);
			iCalendarAppointmentImportingEventArgs args = CreateImportingEventArgs(apt, ev);
			RaiseOnAppointmentImporting(args);
			return args;
		}
		#endregion
		#region CanImportEvent
		protected internal virtual bool CanImportEvent(iCalendarAppointmentImportingEventArgs args) {
			return !IsTermination && !args.Cancel && args.VEvent != null;
		}
		#endregion
		protected internal virtual bool CanImportEvent(Appointment apt, VEvent evt) {
			iCalendarAppointmentImportingEventArgs args = BeforeImport(apt, evt);
			return CanImportEvent(args);
		}
		#region AfterImport
		protected internal virtual void AfterImport(Appointment apt, VEvent ev) {
			iCalendarAppointmentImportedEventArgs args = CreateImportedEventArgs(apt, ev);
			RaiseOnAppointmentImported(args);
		}
		#endregion
		#region ImportChangedOccurrences
		protected internal virtual void ImportChangedOccurrences() {
			Binder.Bind(this);
		}
		#endregion
		#region PrepareAppointment
		protected internal virtual Appointment PrepareAppointment(VEvent ev, AppointmentType type) {
			Appointment apt = CreateAppointmentInstance(type);
			InitAppointmentProperties(apt, ev);
			return apt;
		}
		#endregion
		#region ImportEvents
		protected virtual VEventCollection ImportEvents(VEventCollection events) {
			int count = events.Count;
			VEventCollection notImportedEvents = new VEventCollection();
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				VEvent ev = events[i];
				ISupportCalendarTimeZone impl = ev as ISupportCalendarTimeZone;
				if (impl != null)
					impl.ApplyTimeZone(TimeZoneManager);
				try {
					DoImportEvent(ev);
				} catch (System.Exception e) {
					notImportedEvents.Add(ev);
					RaiseOnException(new iCalendarExchangeExceptionEventArgs(e, ev, i));
				}
			}
			return notImportedEvents;
		}
		#endregion
		#region DoImportEvent
		protected virtual void DoImportEvent(VEvent ev) {
			AppointmentType type = ev.IsPattern ? AppointmentType.Pattern : AppointmentType.Normal;
			Appointment apt = PrepareAppointment(ev, type);
			if (ev.IsChangedOccurrence)
				Binder.RegisterChangedOccurrence(ev);
			else
				ImportEvent(apt, ev);
		}
		#endregion
		#region CreateAppointmentInstance
		protected virtual Appointment CreateAppointmentInstance(AppointmentType type) {
			return SchedulerUtils.CreateAppointmentInstance(Storage, type);
		}
		#endregion
		#region DeleteAppointment
		protected internal virtual void DeleteAppointment(Appointment apt) {
			if (apt.Type == AppointmentType.ChangedOccurrence)
				PerformDeleteAppointment(apt);
			PerformDeleteAppointment(apt);
		}
		#endregion
		#region PerformDeleteAppointment
		protected internal virtual void PerformDeleteAppointment(Appointment apt) {
			apt.Delete();
		}
		#endregion
		#region InitAppointmentProperties
		protected internal virtual void InitAppointmentProperties(Appointment apt, VEvent ev) {
			apt.Subject = ev.Summary.Value;
			apt.Location = ev.Location.Value;
			apt.Description = ev.Description.Value;
			apt.Start = ev.Start.Value;
			if (ev.End.Value != DateTime.MinValue)
				apt.End = ev.End.Value;
			apt.AllDay = ev.IsAllDay;
			if (apt.Type == AppointmentType.Pattern)
				InitRecurrenceInfo(apt.RecurrenceInfo, ev);
			apt.StatusKey = GetAppointmentStatusId(ev.CustomProperties);
			apt.LabelKey = GetAppointmentLabelId(ev.CustomProperties);
			InitAppointmentReminders(apt, ev.Alarms);
			InitAppointmentCustomFields(apt, ev.CustomProperties);
			InitAppointmentResourceIds(apt, ev.CustomProperties);
		}
		#endregion
		#region GetAppointmentLabelId
		int GetAppointmentLabelId(iCalendarPropertyCollection properties) {
			iCalendarEventLabelsExchangeHelper helper = new iCalendarEventLabelsExchangeHelper(CustomPropertyIdentifier);
			return helper.GetLabelId(properties);
		}
		#endregion
		#region InitAppointmentReminders
		protected internal virtual void InitAppointmentReminders(Appointment apt, VAlarmCollection vAlarmCollection) {
			int count = vAlarmCollection.Count;
			for (int i = 0; i < count; i++) {
				VAlarm alarm = vAlarmCollection[i];
				Reminder reminder = apt.CreateNewReminder();
				apt.Reminders.Add(reminder);
				InitReminder(reminder, alarm.Trigger);
			}
		}
		#endregion
		#region InitReminder
		protected internal virtual void InitReminder(Reminder reminder, TriggerProperty trigger) {
			if (trigger.ValueDataType == ValueDataTypes.DateTime) {
				reminder.AlertTime = new DateTime(Math.Max(trigger.Value, 0));
			} else {
				long ticks = -trigger.Value;
				long aptDurationTicks = reminder.Appointment.Duration.Ticks;
				if (trigger.AlarmTriggerRelationship == AlarmTriggerRelationship.Start)
					reminder.TimeBeforeStart = new TimeSpan(Math.Max(ticks, 0));
				else
					reminder.TimeBeforeStart = new TimeSpan(Math.Max(ticks - aptDurationTicks, 0));
			}
		}
		#endregion
		#region InitAppointmentCustomFields
		protected void InitAppointmentCustomFields(Appointment apt, iCalendarPropertyCollection customProperties) {
			iCalendarCustomFieldExchangeHelper helper = new iCalendarCustomFieldExchangeHelper(CustomPropertyIdentifier);
			apt.BeginUpdate();
			try {
				helper.InitCustomFields(apt.CustomFields, Appointments.CustomFieldMappings, customProperties);
			} finally {
				apt.CancelUpdate();
			}
		}
		#endregion
		#region GetAppointmentStatusId
		protected internal virtual object GetAppointmentStatusId(iCalendarPropertyCollection properties) {
			iCalendarOutlookStatusExchangeHelper outlookStatusHelper = new iCalendarOutlookStatusExchangeHelper();
			AppointmentStatusType statusType = outlookStatusHelper.GetAppointmentStatusType(properties);
			iCalendarAppointmentStatusExchangeHelper appointmentStatusHelper = new iCalendarAppointmentStatusExchangeHelper(CustomPropertyIdentifier);
			if (appointmentStatusHelper.IsPropertyExists(properties))
				statusType = appointmentStatusHelper.GetAppointmentStatusType(properties);
			IAppointmentStatus statusWithType = Appointments.Statuses.GetByType(statusType);
			if ( statusWithType == null )
				return -1;
			return statusWithType.Id;
		}
		#endregion
		#region InitRecurrenceInfo
		protected internal virtual void InitRecurrenceInfo(IRecurrenceInfo info, VEvent ev) {
			VRecurrenceRule rule = ev.RecurrenceRules[0].Value;
			VRecurrenceConverter conv = VRecurrenceConverter.CreateInstance(rule.Frequency);
			conv.AssignRecurrenceInfo(info, ev.Start.Value, rule);
		}
		#endregion
		#region InitAppointmentResourceIds
		protected virtual void InitAppointmentResourceIds(Appointment apt, iCalendarPropertyCollection customProperties) {
			iCalendarResourceIdsExchangeHelper helper = new iCalendarResourceIdsExchangeHelper(CustomPropertyIdentifier);
			AppointmentResourceIdCollection resourceIds = new AppointmentResourceIdCollection();
			helper.InitAppointmentResourceIds(resourceIds, customProperties);
			IResourceStorageBase stroageResources = Storage.Resources;
			int count = stroageResources.Count;
			for (int i = 0; i < count; i++) { 
				Resource resource = stroageResources[i];
				if (ResourceBase.InternalMatchIdToResourceIdCollection(resourceIds, resource.Id))
					apt.ResourceIds.Add(resource.Id);
			}
		}
		#endregion
		#region ImportEvent
		protected internal virtual void ImportEvent(Appointment apt, VEvent ev) {
			if (CanImportEvent(apt, ev)) {
				CommitImport(apt, ev);
				if (apt.Type == AppointmentType.Pattern)
					ImportDeletedOccurrences(apt, ev);
			} else {
				if (apt.IsException)
					DeleteAppointment(apt);
			}
		}
		#endregion
		#region ImportExceptionEvent
		protected internal virtual void ImportExceptionEvent(Appointment apt, VEvent ev) {
			CommitImport(apt, ev);
		}
		#endregion
		#region ImportDeletedOccurrences
		protected virtual void ImportDeletedOccurrences(Appointment pattern, VEvent vPattern) {
			AppointmentBaseCollection exceptions = ExceptionCreator.CreateExceptions(pattern, vPattern);
			int count = exceptions.Count;
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				ImportDeletedOccurrence(exceptions[i], vPattern);
			}
		}
		#endregion
		#region ImportDeletedOccurrence
		protected virtual void ImportDeletedOccurrence(Appointment exception, VEvent pattern) {
			iCalendarAppointmentImportingEventArgs args = BeforeImport(exception, pattern);
			if (CanImportEvent(args)) {
				AfterImport(exception, pattern);
			} else {
				PerformDeleteAppointment(exception);
			}
		}
		#endregion
		#region CommitImport
		protected internal virtual void CommitImport(Appointment apt, VEvent ev) {
			if (apt.IsBase)
				Appointments.Add(apt);
			if (apt.Type == AppointmentType.Pattern)
				Binder.RegisterAppointmentPattern(ev, apt);
			AfterImport(apt, ev);
		}
		#endregion
		TimeZoneInfo OnCustomizeConvertedTimeZone(TimeZoneInfo timeZoneInfo) {
			if (CustomizeConvertedTimeZone == null)
				return null;
			TimeZoneCustomizationEventArgs args = new TimeZoneCustomizationEventArgs(timeZoneInfo);
			CustomizeConvertedTimeZone(typeof(TimeZoneConverter), args);
			return args.TimeZone;
		}
	}
	#endregion
	#region iCalendarExporter
	public class iCalendarExporter : AppointmentExporter {
		#region Fields
		string customPropertyIdentifier = iCalendarSR.DXPropertyIdentifier;
		string productIdentifier = iCalendarSR.DXProductIdentifier;
		iCalendarContainer calendars;
		AppointmentBaseCollection appointments;
		IUniqueIdentifierProvider uniqueIdentifierProvider;
		DateTime timeStamp;
		TimeZoneManager timeZoneManager;
		#endregion
		public iCalendarExporter(ISchedulerStorageBase storage)
			: base(storage) {
				this.timeZoneManager = new TimeZoneManager(((IInternalSchedulerStorageBase)storage).TimeZoneEngine);
			this.appointments = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			if (storage != null && storage.Appointments != null)
				Appointments.AddRange(storage.Appointments.Items);
			Initialize();
		}
		public iCalendarExporter(ISchedulerStorageBase storage, AppointmentBaseCollection appointments)
			: base(storage) {
			if (appointments == null)
				Exceptions.ThrowArgumentNullException("appointments");
			this.timeZoneManager = new TimeZoneManager(((IInternalSchedulerStorageBase)storage).TimeZoneEngine);
			this.appointments = appointments;
			Initialize();
		}
		#region Properties
		protected internal iCalendarContainer Calendars { get { return calendars; } }
		protected internal iCalendarComponent Calendar { get { return Calendars.Count > 0 ? Calendars[0] : null; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarExporterAppointments")]
#endif
		public AppointmentBaseCollection Appointments {
			get { return appointments; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("Appointments", value);
				appointments = value;
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarExporterCustomPropertyIdentifier")]
#endif
		public string CustomPropertyIdentifier { get { return customPropertyIdentifier; } set { customPropertyIdentifier = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarExporterProductIdentifier")]
#endif
		public string ProductIdentifier { get { return productIdentifier; } set { productIdentifier = value; } }
		protected IUniqueIdentifierProvider UniqueIdentifierProvider { get { return uniqueIdentifierProvider; } }
		protected IAppointmentStorageBase AppointmentStorage { get { return Storage.Appointments; } }
		protected internal DateTime TimeStamp { get { return timeStamp; } set { timeStamp = value; } }
		#endregion
		#region Events
		iCalendarStructureCreatedEventHandler onCalendarStructureCreated;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarExporterCalendarStructureCreated")]
#endif
		public event iCalendarStructureCreatedEventHandler CalendarStructureCreated { add { onCalendarStructureCreated += value; } remove { onCalendarStructureCreated -= value; } }
		protected internal virtual void RaiseCalendarStructureCreated() {
			if (onCalendarStructureCreated != null)
				onCalendarStructureCreated(this, new iCalendarStructureCreatedEventArgs(Calendars));
		}
		protected internal virtual iCalendarAppointmentExportingEventArgs CreateExportingEventArgs(Appointment apt, VEvent ev) {
			return new iCalendarAppointmentExportingEventArgs(apt, ev);
		}
		protected internal virtual iCalendarAppointmentExportedEventArgs CreateExportedEventArgs(Appointment apt, VEvent ev) {
			return new iCalendarAppointmentExportedEventArgs(apt, ev);
		}
		#endregion
		#region Initialize
		void Initialize() {
			this.calendars = new iCalendarContainer();
			this.uniqueIdentifierProvider = CreateUniqueIdentifierProvider();
			this.timeStamp = DateTime.Now;
		}
		#endregion
		#region CreateUniqueIdentifierProvider
		protected virtual IUniqueIdentifierProvider CreateUniqueIdentifierProvider() {
			return new GuidUniqueIdentifierProvider();
		}
		#endregion
		#region ExportCore
		protected internal override void ExportCore(Stream stream) {
			if (Appointments == null)
				return;
			StreamWriter writer = new StreamWriter(stream);
			ExportAppointments(writer);
			writer.Flush();
		}
		#endregion
		#region ExportAppointments
		protected internal virtual void ExportAppointments(StreamWriter writer) {
			AddNewCalendar();
			CreateCalendarStructure();
			if (IsTermination)
				return;
			WriteCalendar(writer, Calendar);
		}
		#endregion
		#region WriteCalendar
		protected internal virtual void WriteCalendar(StreamWriter writer, iCalendarComponent calendar) {
			iCalendarWriter calendarWriter = CreateCalendarWriter(writer);
			calendar.WriteToStream(calendarWriter);
		}
		#endregion
		#region CreateCalendarWriter
		protected virtual iCalendarWriter CreateCalendarWriter(TextWriter writer) {
			return new iCalendarWriter(writer);
		}
		#endregion
		#region AddNewCalendar
		protected internal void AddNewCalendar() {
			iCalendarComponent calendar = new iCalendarComponent(Appointments.Count);
			calendar.ProductIdentifier.Value = ProductIdentifier;
			calendar.Method.Value = "PUBLISH";
			Calendars.AddObject(calendar);
		}
		#endregion
		#region CreateCalenarStructure
		protected internal virtual void CreateCalendarStructure() {
			int count = Appointments.Count;
			if (count > 0)
				ExportTimeZone(Calendar);
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				DoExportAppointment(Appointments[i]);
			}
			RaiseCalendarStructureCreated();
		}
		#endregion
		#region ExportTimeZone
		void ExportTimeZone(iCalendarComponent calendar) {
			VTimeZone timeZone = TimeZoneConverter.ConvertToVTimeZone(((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.OperationTimeZone);
			calendar.TimeZones.Add(timeZone);
		}
		#endregion
		#region DoExportAppointment
		protected internal virtual void DoExportAppointment(Appointment apt) {
			VEvent ev = PrepareEvent(apt);
			ExportAppointmentCore(apt, ev);
		}
		#endregion
		#region ExportAppointmentCore
		protected internal virtual void ExportAppointmentCore(Appointment apt, VEvent ev) {
			iCalendarAppointmentExportingEventArgs args = BeforeExport(apt, ev);
			if (CanExportAppointment(args)) {
				CommitExport(apt, ev);
				if (apt.Type == AppointmentType.Pattern)
					ExportPatternExceptions(apt, ev);
			} else {
				if (apt.Type == AppointmentType.DeletedOccurrence) {
					int index = ev.ExceptionDateTimes.FindPropertyIndexByDate(apt.Start);
					if (index >= 0 && index < ev.ExceptionDateTimes.Count)
						ev.ExceptionDateTimes.RemoveAt(index);
				}
			}
		}
		#endregion
		#region CanExportAppointment
		protected internal virtual bool CanExportAppointment(iCalendarAppointmentExportingEventArgs args) {
			return !IsTermination && !args.Cancel && args.Appointment != null;
		}
		#endregion
		#region BeforeExport
		protected internal virtual iCalendarAppointmentExportingEventArgs BeforeExport(Appointment apt, VEvent ev) {
			iCalendarAppointmentExportingEventArgs args = CreateExportingEventArgs(apt, ev);
			RaiseOnAppointmentExporting(args);
			return args;
		}
		#endregion
		#region AfterExport
		protected internal virtual void AfterExport(Appointment apt, VEvent ev) {
			iCalendarAppointmentExportedEventArgs args = CreateExportedEventArgs(apt, ev);
			RaiseOnAppointmentExported(args);
		}
		#endregion
		#region CommitExport
		protected internal virtual void CommitExport(Appointment apt, VEvent ev) {
			VEventCollection events = Calendar.Events;
			if (NeedCommitEvent(apt))
				events.Add(ev);
			AfterExport(apt, ev);
		}
		#endregion
		#region IsEventCommited
		bool NeedCommitEvent(Appointment apt) {
			return apt.Type != AppointmentType.DeletedOccurrence;
		}
		#endregion
		#region ExportPatternExceptions
		protected internal virtual void ExportPatternExceptions(Appointment pattern, VEvent vPattern) {
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			for (int i = 0; i < exceptions.Count; i++) {
				Appointment apt = exceptions[i];
				if (apt.Type == AppointmentType.DeletedOccurrence)
					ExportPatternDeletedOccurrence(pattern, apt, vPattern);
				else {
					ExportPatternChangedOccurrence(pattern, apt, vPattern);
				}
			}
		}
		#endregion
		#region ExportPatternDeletedOccurrence
		protected virtual void ExportPatternDeletedOccurrence(Appointment pattern, Appointment exception, VEvent vPattern) {
			XtraSchedulerDebug.Assert(exception.Type == AppointmentType.DeletedOccurrence);
			DateTime exdate = CalcOccurrenceStartTime(pattern, exception.RecurrenceIndex);
			vPattern.ExceptionDateTimes.Add(exdate);
			ExportAppointmentCore(exception, vPattern);
		}
		#endregion
		#region ExportPatternChangedOccurrence
		protected virtual void ExportPatternChangedOccurrence(Appointment pattern, Appointment exception, VEvent vPattern) {
			VEvent vException = PrepareEvent(exception);
			vException.UniqueIdentifier.Value = vPattern.UniqueIdentifier.Value;
			DateTime occDateTime = CalcOccurrenceStartTime(pattern, exception.RecurrenceIndex);
			vException.RecurrenceId.Value = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(occDateTime, DateTimeKind.Unspecified), ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.OperationTimeZone);
			ExportAppointmentCore(exception, vException);
		}
		#endregion
		#region CalcOccurrenceStartTime
		DateTime CalcOccurrenceStartTime(Appointment pattern, int recurrenceIndex) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			DateTime occDateTime = calc.CalcOccurrenceStartTime(recurrenceIndex);
			return occDateTime;
		}
		#endregion
		#region DoExportEvent
		protected virtual VEvent PrepareEvent(Appointment apt) {
			VEvent ev = CreateEventInstance();
			InitEventProperties(ev, apt);
			return ev;
		}
		#endregion
		#region CreateVEventInstance
		protected virtual VEvent CreateEventInstance() {
			VEvent result = new VEvent();
			result.SetTimeZoneManager(this.timeZoneManager);
			return result;
		}
		#endregion
		#region InitVEventProperties
		protected internal virtual void InitEventProperties(VEvent ev, Appointment apt) {
			ev.Start.Value = apt.Start;
			ev.End.Value = apt.End;
			if (apt.AllDay) {
				ev.Start.ValueDataType = ValueDataTypes.Date;
				ev.End.ValueDataType = ValueDataTypes.Date;
			}
			ev.Description.Value = apt.Description;
			ev.Summary.Value = apt.Subject;
			ev.Location.Value = apt.Location;
			ev.UniqueIdentifier.Value = UniqueIdentifierProvider.NextUid();
			ev.Stamp.Value = TimeStamp;
			if (apt.Type == AppointmentType.Pattern)
				InitEventPatternProperties(ev, apt);
			InitEventStatus(ev.CustomProperties, apt.StatusKey);
			InitEventLabels(ev.CustomProperties, apt.LabelKey);
			InitEventResourceIds(ev.CustomProperties, apt);
			InitEventAlarms(ev.Alarms, apt);
			InitEventCustomFields(ev.CustomProperties, apt.CustomFields);
		}
		#endregion
		#region InitEventAlarms
		private void InitEventAlarms(VAlarmCollection vAlarmCollection, Appointment apt) {
			int count = apt.Reminders.Count;
			for (int i = 0; i < count; i++) {
				Reminder reminder = apt.Reminders[i];
				VAlarm alarm = CreateAlarm(reminder);
				vAlarmCollection.Add(alarm);
			}
		}
		private VAlarm CreateAlarm(Reminder reminder) {
			VAlarm alarm = new VAlarm();
			alarm.Trigger.Value = -reminder.TimeBeforeStart.Ticks;
			return alarm;
		}
		#endregion
		#region InitEventStatus
		protected virtual void InitEventStatus(iCalendarPropertyCollection customProperties, object statusId) {
			iCalendarOutlookStatusExchangeHelper outlookStatusHelper = new iCalendarOutlookStatusExchangeHelper();
			iCalendarAppointmentStatusExchangeHelper appointmentStatusHelper = new iCalendarAppointmentStatusExchangeHelper(CustomPropertyIdentifier);
			outlookStatusHelper.AddCustomProperties(customProperties, statusId, AppointmentStorage.Statuses);
			appointmentStatusHelper.AddCustomProperties(customProperties, statusId, AppointmentStorage.Statuses);
		}
		#endregion
		#region InitEventResourceIds
		protected virtual void InitEventResourceIds(iCalendarPropertyCollection customProperties, Appointment apt) {
			iCalendarResourceIdsExchangeHelper helper = new iCalendarResourceIdsExchangeHelper(CustomPropertyIdentifier);
			helper.AddCustomProperties(customProperties, apt.ResourceIds);
		}
		#endregion
		#region AddAppointmentCustomFields
		protected virtual void InitEventCustomFields(iCalendarPropertyCollection customProperties, CustomFieldCollection fields) {
			iCalendarCustomFieldExchangeHelper helper = new iCalendarCustomFieldExchangeHelper(CustomPropertyIdentifier);
			helper.AddCustomProperties(customProperties, fields);
		}
		#endregion
		#region InitRecurrenceInfo
		protected virtual void InitEventPatternProperties(VEvent ev, Appointment pattern) {
			IRecurrenceInfo recurrenceInfo = pattern.RecurrenceInfo;
			ev.Start.Value = recurrenceInfo.Start;
			ev.End.Value = recurrenceInfo.Start + pattern.Duration;
			ev.Start.TimeZoneIdentifier = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.OperationTimeZone.DisplayName;
			ev.End.TimeZoneIdentifier = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine.OperationTimeZone.DisplayName;
			RecurrenceType actualType = (recurrenceInfo.Type == RecurrenceType.Daily && recurrenceInfo.WeekDays != WeekDays.EveryDay) ? RecurrenceType.Weekly : recurrenceInfo.Type;
			VRecurrenceConverter converter = VRecurrenceConverter.CreateInstance(actualType);
			VRecurrenceRule rule = converter.FromRecurrenceInfo(recurrenceInfo, pattern);
			ev.RecurrenceRules.Add(rule);
		}
		#endregion
		#region InitEventLabels
		void InitEventLabels(iCalendarPropertyCollection customProperties, object labelId) {
			iCalendarEventLabelsExchangeHelper helper = new iCalendarEventLabelsExchangeHelper(CustomPropertyIdentifier);
			helper.AddCustomProperties(customProperties, labelId, AppointmentStorage.Labels);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.iCalendar.Native {
	#region PatternExceptionCreator
	public class PatternExceptionCreator {
		ISchedulerStorageBase storage;
		TimeSpan recurrenceRangeDuration = TimeSpan.FromDays(365);
		public PatternExceptionCreator(ISchedulerStorageBase storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		public TimeSpan RecurrenceRangeDuration { get { return recurrenceRangeDuration; } set { this.recurrenceRangeDuration = value; } }
		public AppointmentBaseCollection CreateExceptions(Appointment pattern, VEvent vPattern) {
			AppointmentBaseCollection exceptions = new AppointmentBaseCollection();
			exceptions.AddRange(CreateExceptionsByDates(pattern, vPattern.ExceptionDateTimes));
			exceptions.AddRange(CreateExceptionsByRules(pattern, vPattern.ExceptionRules));
			return exceptions;
		}
		protected Appointment CreatePatternExceptionCore(Appointment pattern, int index) {
			return pattern.CreateException(AppointmentType.DeletedOccurrence, index);
		}
		protected virtual AppointmentBaseCollection CreateExceptionsByDates(Appointment pattern, ExceptionDateTimesPropertyCollection dateTimeProps) {
			AppointmentBaseCollection exceptions = new AppointmentBaseCollection();
			if (dateTimeProps.Count <= 0)
				return exceptions;
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			for (int i = 0; i < dateTimeProps.Count; i++) {
				DateTimeCollection dateTimes = dateTimeProps[i].Values;
				for (int j = 0; j < dateTimes.Count; j++) {
					int index = calc.FindOccurrenceIndex(dateTimes[j], pattern);
					if (index < 0)  
						continue;
					Appointment exception = CreatePatternExceptionCore(pattern, index);
					exceptions.Add(exception);
				}
			}
			return exceptions;
		}
		protected virtual AppointmentBaseCollection CreateExceptionsByRules(Appointment pattern, RecurrenceRulePropertyCollection rules) {
			AppointmentBaseCollection exceptions = new AppointmentBaseCollection();
			if (rules.Count <= 0)
				return exceptions;
			for (int i = 0; i < rules.Count; i++) {
				Appointment rulePattern = CreateExceptionRulePattern(pattern, rules[i].Value);
				if (rulePattern == null)
					continue;
				AppointmentBaseCollection occurrences = GenerateOccurrences(rulePattern);
				CreateExceptionByOccurrences(pattern, occurrences, exceptions);
			}
			return exceptions;
		}
		protected virtual void CreateExceptionByOccurrences(Appointment pattern, AppointmentBaseCollection occurrences, AppointmentBaseCollection destExceptions) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			for (int i = 0; i < occurrences.Count; i++) {
				int index = calc.FindOccurrenceIndex(occurrences[i].Start, pattern);
				if (index >= 0 && !ExceptionExists(pattern, index)) {
					Appointment exception = CreatePatternExceptionCore(pattern, index);
					destExceptions.Add(exception);
				}
			}
		}
		protected bool ExceptionExists(Appointment pattern, int index) {
			return pattern.FindException(index) != null;
		}
		protected virtual Appointment CreateExceptionRulePattern(Appointment srcPattern, VRecurrenceRule rule) {
			Appointment result = SchedulerUtils.CreateAppointmentInstance(storage, AppointmentType.Pattern);
			result.Start = srcPattern.Start;
			result.End = srcPattern.End;
			VRecurrenceConverter conv = VRecurrenceConverter.CreateInstance(rule.Frequency);
			bool success = conv.AssignRecurrenceInfo(result.RecurrenceInfo, srcPattern.Start, rule);
			return success ? result : null;
		}
		protected virtual AppointmentBaseCollection GenerateOccurrences(Appointment pattern) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			TimeInterval interval = CalculateRecurrenceInterval(pattern);
			return calc.CalcOccurrences(interval, pattern);
		}
		protected virtual TimeInterval CalculateRecurrenceInterval(Appointment pattern) {
			if (pattern.RecurrenceInfo.Range == RecurrenceRange.NoEndDate)
				return new TimeInterval(pattern.Start, RecurrenceRangeDuration);
			return AppointmentCollection.CalcPatternInterval(pattern);
		}
	}
	#endregion
	#region PatternExceptionBinder
	public class PatternExceptionBinder {
		#region Fields
		Dictionary<string, Appointment> importedAppointmentPatterns;
		Dictionary<string, VEvent> importedEventPatterns;
		List<VEvent> unlinkedExceptions;
		#endregion
		#region ctor
		public PatternExceptionBinder() {
			this.importedAppointmentPatterns = new Dictionary<string, Appointment>();
			this.unlinkedExceptions = new List<VEvent>();
			this.importedEventPatterns = new Dictionary<string, VEvent>();
		}
		#endregion
		#region Properties
		protected internal Dictionary<string, Appointment> ImportedAppointmentPatterns { get { return importedAppointmentPatterns; } }
		protected internal List<VEvent> UnlinkedExceptions { get { return unlinkedExceptions; } }
		public Dictionary<string, VEvent> ImportedEventPatterns { get { return importedEventPatterns; } }
		#endregion
		#region Clear
		public void Clear() {
			ImportedAppointmentPatterns.Clear();
			ImportedEventPatterns.Clear();
			UnlinkedExceptions.Clear();
		}
		#endregion
		#region RegisterChangedOccurrence
		public void RegisterChangedOccurrence(VEvent ev) {
			UnlinkedExceptions.Add(ev);
		}
		#endregion
		#region BindCore
		protected internal virtual bool BindCore(Appointment exception, VEvent vException, iCalendarImporter importer) {
			string uid = vException.UniqueIdentifier.Value;
			IInternalAppointment exceptionInternal = ((IInternalAppointment)exception);
			Appointment aptPattern = FindAppointmentPattern(uid);
			VEvent vPattern = FindEventPattern(uid);
			if (aptPattern == null && vPattern == null) {
				exceptionInternal.SetTypeCore(AppointmentType.Normal);
				return true;
			} else if (aptPattern == null && vPattern != null)
				return false;
			string tzid = vPattern.Start.TimeZoneIdentifier;
			TimeZoneManager manager = importer.TimeZoneManager;
			Appointment patternCopy = CreateSpecificTimeZonePattern(manager, aptPattern, tzid);
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(patternCopy.RecurrenceInfo);
			DateTime occDateTime = manager.FromLocalTime(vException.RecurrenceId.Value, tzid);
			int index = calc.FindOccurrenceIndex(occDateTime, patternCopy);
			if (index < 0)
				return false;
			exceptionInternal.SetRecurrenceIndex(index);
			if (!importer.CanImportEvent(exception, vException))
				return false;
			((IInternalAppointment)aptPattern).RecurrenceChain.RegisterException(exception);
			return true;
		}
		#endregion
		#region CreateOriginalTimeZonePattern
		protected Appointment CreateSpecificTimeZonePattern(TimeZoneManager manager, Appointment aptPattern, string tzid) {
			Appointment patternCopy = aptPattern.Copy();
			patternCopy.Start = manager.FromLocalTime(patternCopy.Start, tzid);
			patternCopy.RecurrenceInfo.Start = manager.FromLocalTime(patternCopy.RecurrenceInfo.Start, tzid);
			return patternCopy;
		}
		#endregion
		#region FindAppointmentPattern
		protected virtual Appointment FindAppointmentPattern(string uid) {
			return ImportedAppointmentPatterns.ContainsKey(uid) ? ImportedAppointmentPatterns[uid] : null;
		}
		#endregion
		#region FindEventPattern
		protected virtual VEvent FindEventPattern(string uid) {
			return ImportedEventPatterns.ContainsKey(uid) ? ImportedEventPatterns[uid] : null;
		}
		#endregion
		#region RegisterPattern
		public void RegisterAppointmentPattern(VEvent ev, Appointment pattern) {
			XtraSchedulerDebug.Assert(pattern.Type == AppointmentType.Pattern);
			string uid = ev.UniqueIdentifier.Value;
			if (uid != null) {
				ImportedAppointmentPatterns.Add(uid, pattern);
			}
		}
		#endregion
		#region RegisterPattern
		public void RegisterEventPattern(VEvent ev) {
			XtraSchedulerDebug.Assert(ev.IsPattern);
			string uid = ev.UniqueIdentifier.Value;
			if (uid != null)
				importedEventPatterns.Add(uid, ev);
		}
		#endregion
		#region Bind
		public void Bind(iCalendarImporter importer) {
			if (importer == null)
				return;
			int count = UnlinkedExceptions.Count;
			for (int i = 0; i < count; i++) {
				VEvent ev = UnlinkedExceptions[i];
				Appointment apt = importer.PrepareAppointment(ev, AppointmentType.ChangedOccurrence);
				if (BindCore(apt, ev, importer)) {
					if (apt.Type == AppointmentType.Normal)
						importer.ImportEvent(apt, ev);
					else
						importer.ImportExceptionEvent(apt, ev);
				}
			}
		}
		#endregion
	}
	#endregion
	#region iCalendarExchangeHelperBase (abstract class)
	public abstract class iCalendarExchangeHelperBase {
		string propertyNameIdent;
		string fullName;
		protected iCalendarExchangeHelperBase(string propertyNameIdent) {
			if (String.IsNullOrEmpty(propertyNameIdent))
				Exceptions.ThrowArgumentException("propertyNameIdent", propertyNameIdent);
			this.propertyNameIdent = propertyNameIdent;
		}
		#region Properties
		public string PropertyNameIdent { get { return propertyNameIdent; } }
		public abstract string PropertyName { get; }
		public string FullName {
			get {
				if (String.IsNullOrEmpty(fullName)) {
					string propertyName = String.Format("{0}-{1}", PropertyNameIdent, PropertyName);
					this.fullName = iCalendarUtils.CreateXString(propertyName);
				}
				return fullName;
			}
		}
		#endregion
		#region IsPropertyExists
		public bool IsPropertyExists(iCalendarPropertyCollection properties) {
			return properties.ContainKeys(FullName);
		}
		#endregion
	}
	#endregion
	#region iCalendarCustomFieldExchangeHelper
	public class iCalendarCustomFieldExchangeHelper : iCalendarExchangeHelperBase {
		public iCalendarCustomFieldExchangeHelper(string propertyNameIdent)
			: base(propertyNameIdent) {
		}
		public override string PropertyName { get { return iCalendarSR.CustomFieldProperty; } }
		#region AddCustomProperties
		public void AddCustomProperties(iCalendarPropertyCollection customProperties, CustomFieldCollection fields) {
			int count = fields.Count;
			for (int i = 0; i < count; i++) {
				CustomField customField = fields.GetFieldByIndex(i);
				string propertyName = GetFullPropertyName(customField.Name);
				string propertyValue = ObjectConverter.ObjectToString(customField.Value);
				CustomProperty value = new CustomProperty(propertyName, null, iCalendarConvert.EscapeString(propertyValue));
				customProperties.Add(value);
			}
		}
		#endregion
		#region GetFullPropertyName
		protected string GetFullPropertyName(string customFieldName) {
			return String.Format("{0}-{1}", FullName, customFieldName.ToUpper());
		}
		#endregion
		#region InitCustomFields
		public void InitCustomFields(CustomFieldCollection customFields, MappingCollection mappings, iCalendarPropertyCollection customProperties) {
			int count = customFields.Count;
			for (int i = 0; i < count; i++) {
				CustomField field = customFields.GetFieldByIndex(i);
				string name = field.Name;
				string stringValue = GetFieldStringValue(name, customProperties);
				if (String.IsNullOrEmpty(stringValue))
					continue;
				MappingBase mapping = mappings[name];
				field.Value = ObjectConverter.StringToObject(stringValue, mapping.Type);
			}
		}
		#endregion
		#region GetFieldStringValue
		protected string GetFieldStringValue(string name, iCalendarPropertyCollection customProperties) {
			string customPropertyName = GetFullPropertyName(name);
			if (!customProperties.ContainKeys(customPropertyName))
				return null;
			CustomProperty property = (CustomProperty)customProperties[customPropertyName];
			return property.Value;
		}
		#endregion
	}
	#endregion
	#region iCalendarResourceIdsExchangeHelper
	public class iCalendarResourceIdsExchangeHelper : iCalendarExchangeHelperBase {
		public iCalendarResourceIdsExchangeHelper(string propertyNameIdent)
			: base(propertyNameIdent) {
		}
		public override string PropertyName { get { return iCalendarSR.ResourceIdProperty; } }
		#region AddCustomProperties
		public void AddCustomProperties(iCalendarPropertyCollection customProperties, AppointmentResourceIdCollection resourceIds) {
			if (resourceIds.Count == 0)
				return;
			if (resourceIds[0] == EmptyResourceId.Id)
				return;
			CustomProperty resourceIdProperty = CreateResourceIdProperty(resourceIds);
			customProperties.Add(resourceIdProperty);
		}
		#endregion
		#region CreateResourceIdProperty
		protected internal CustomProperty CreateResourceIdProperty(AppointmentResourceIdCollection ids) {
			AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(ids);
			string value = iCalendarConvert.EscapeString(helper.ToXml());
			return new CustomProperty(FullName, null, value);
		}
		#endregion
		#region InitAppointmentResourceIds
		public void InitAppointmentResourceIds(AppointmentResourceIdCollection resourceIds, iCalendarPropertyCollection customProperties) {
			if (!customProperties.ContainKeys(FullName))
				return;
			CustomProperty resourceIdsProperty = (CustomProperty)customProperties[FullName];
			AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds);
			helper.FromXml(resourceIdsProperty.Value);
		}
		#endregion
	}
	#endregion
	#region iCalendarStatusExchangeHelperBase (abstract class)
	public abstract class iCalendarStatusExchangeHelperBase : iCalendarExchangeHelperBase {
		protected iCalendarStatusExchangeHelperBase(string propertyNameIdent)
			: base(propertyNameIdent) {
		}
		#region AddCustomProperties
		public void AddCustomProperties(iCalendarPropertyCollection customProperties, object statusId, IAppointmentStatusStorage statuses) {
			CustomProperty status = CreateStatusProperty(statusId, statuses);
			if (status != null)
				customProperties.Add(status);
		}
		#endregion
		#region GetAppointmentStatusType
		public virtual AppointmentStatusType GetAppointmentStatusType(iCalendarPropertyCollection properties) {
			if (!properties.ContainKeys(FullName))
				return AppointmentStatusType.Busy;
			CustomProperty statusProperty = (CustomProperty)properties[FullName];
			return iCalendarConvert.ToAppointmentStatus(statusProperty.Value);
		}
		#endregion
		#region CreateStatusProperty
		protected internal CustomProperty CreateStatusProperty(object statusId, IAppointmentStatusStorage statuses) {
			AppointmentStatusType statusType = statuses.GetById(statusId).Type;
			string value = iCalendarConvert.FromAppointmentStatus(statusType);
			return new CustomProperty(FullName, null, value);
		}
		#endregion
	}
	#endregion
	#region CalendarOutlookStatusExchangeHelper
	public class iCalendarOutlookStatusExchangeHelper : iCalendarStatusExchangeHelperBase {
		public iCalendarOutlookStatusExchangeHelper()
			: base(iCalendarSR.MicrosoftCDO) {
		}
		public override string PropertyName { get { return iCalendarSR.OutlookStatusProperty; } }
	}
	#endregion
	#region iCalendarAppointmentStatusExchangeHelper
	public class iCalendarAppointmentStatusExchangeHelper : iCalendarStatusExchangeHelperBase {
		public iCalendarAppointmentStatusExchangeHelper(string propertyNameIdent)
			: base(propertyNameIdent) {
		}
		public override string PropertyName {
			get { return iCalendarSR.AppointmentStatusProperty; }
		}
	}
	#endregion
	#region iCalendarEventLabelsExchangeHelper
	public class iCalendarEventLabelsExchangeHelper : iCalendarExchangeHelperBase {
		public iCalendarEventLabelsExchangeHelper(string propertyNameIdent)
			: base(propertyNameIdent) {
		}
		public override string PropertyName { get { return iCalendarSR.AppointmentLabelProperty; } }
		#region AddCustomProperties
		public void AddCustomProperties(iCalendarPropertyCollection customProperties, object labelId, IAppointmentLabelStorage labels) {
			if (labelId == null)
				return;
			IAppointmentLabel label0 = labels.GetByIndex(0);
			if (Object.Equals(labelId, label0.Id))
				return;
			CustomProperty label = new CustomProperty(FullName, null, labelId.ToString());
			if (label != null)
				customProperties.Add(label);
		}
		#endregion
		#region GetLabelId
		public int GetLabelId(iCalendarPropertyCollection properties) {
			if (!IsPropertyExists(properties))
				return 0;
			string stringValue = properties[FullName].StringValue;
			return Int32.Parse(stringValue);
		}
		#endregion
	}
	#endregion
}
