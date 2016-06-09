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
using DevExpress.XtraScheduler.Exchange;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.VCalendar {
	public class VCalendarImporter : AppointmentImporter {
		VObjectDecoder decoder;
		readonly Stack<VObjectDecoder> decoderStack;
		TextReader reader;
		readonly List<VCalendarObject> calendars;
		int sourceObjectCount = -1;
		public VCalendarImporter(ISchedulerStorageBase storage)
			: base(storage) {
			calendars = new List<VCalendarObject>();
			decoderStack = new Stack<VObjectDecoder>();
		}
		protected VObjectDecoder Decoder { get { return decoder; } }
		protected Stack<VObjectDecoder> DecoderStack { get { return decoderStack; } }
		protected internal List<VCalendarObject> Calendars { get { return calendars; } }
		protected internal int InnerSourceObjectCount { get { return sourceObjectCount; } set { sourceObjectCount = value; } }
		protected internal override void ImportCore(Stream stream) {
			if (stream == null || stream == Stream.Null)
				return;
			VCalendarObject[] calendars;
			using (MemoryStream memoryStream = CreateStreamCopy(stream)) {
				using (StreamReader sr = new StreamReader(memoryStream)) {
					calendars = Decode(sr);
				}
			}
			if (calendars == null)
				return;
			this.sourceObjectCount = calendars.Length;
			for (int i = 0; i < sourceObjectCount; i++) {
				ImportEvents(calendars[i].EventList);
			}
		}
		protected internal override int CalculateSourceObjectCount() {
			return InnerSourceObjectCount;
		}
		protected internal virtual VCalendarObject[] Decode(TextReader reader) {
			if (reader == null)
				return null;
			this.reader = reader;
			string line = null;
			while (true) {
				line = reader.ReadLine();
				if (line == null)
					break;
				if (!IsNewCalendar(line))
					continue;
				UpdateDecoder(line);
				System.Diagnostics.Debug.Assert(Decoder is VCalendarDecoder);
				Decoder.Decode(reader);
			}
			return Calendars.ToArray();
		}
		protected bool IsNewCalendar(string token) {
			return token.StartsWith(VCalendarTag.BeginCalendar);
		}
		void UpdateDecoder(string token) {
			if (Decoder != null) {
				DecoderStack.Push(Decoder);
			}
			string objectTag = VCalendarUtils.RemoveBeginTag(token);
			this.decoder = CreateDecoder(objectTag);
		}
		VObjectDecoder CreateDecoder(string objectTag) {
			VObjectDecoder decoder = VObjectDecoder.GetDecoder(objectTag);
			if (decoder != null) {
				decoder.DecodeComplete += new EventHandler(OnDecodeComplete);
				decoder.BeginSubItem += new DecodeEventHandler(OnDecodeSubItem);
			}
			return decoder;
		}
		void RestoreDecoder() {
			Decoder.DecodeComplete -= new EventHandler(OnDecodeComplete);
			Decoder.BeginSubItem -= new DecodeEventHandler(OnDecodeSubItem);
			if (DecoderStack.Count > 0) {
				this.decoder = DecoderStack.Pop();
				Decoder.Decode(reader);
			}
		}
		void OnDecodeComplete(object sender, EventArgs e) {
			VCalendarDecoder cDec = sender as VCalendarDecoder;
			if (cDec != null) {
				Calendars.Add(cDec.Calendar);
			}
			VEventDecoder evDec = sender as VEventDecoder;
			if (evDec != null) {
				VCalendarDecoder lastDec = DecoderStack.Peek() as VCalendarDecoder;
				if (lastDec != null)
					lastDec.Calendar.EventList.Add(evDec.Event);
			}
			RestoreDecoder();
		}
		void OnDecodeSubItem(object sender, DecodeEventArgs e) {
			UpdateDecoder(e.Token);
			Decoder.Decode(reader);
		}
		protected void ImportEvents(IList events) {
			int count = events.Count;
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				DoImportEvent(events[i] as VEvent);
			}
		}
		protected internal virtual void DoImportEvent(VEvent ev) {
			if (ev == null)
				return;
			Appointment apt = CreateNewAppointment(ev);
			InitAppointmentProperties(apt, ev);
			ImportEvent(ev, apt);
		}
		protected internal virtual void ImportEvent(VEvent ev, Appointment apt) {
			VCalendarAppointmentImportingEventArgs args = BeforeImport(ev, apt);
			if (CanImportEvent(args)) {
				ImportEventCore(ev, apt);
			}
			else {
				apt.Dispose();
				apt = null;
			}
		}
		protected internal virtual void InitAppointmentProperties(Appointment apt, VEvent ev) {
			ev.ToAppointment(apt);
		}
		protected internal virtual void CommitImport(Appointment apt, VEvent ev) {
			Storage.Appointments.Add(apt);
			AfterImport(apt, ev);
		}
		protected internal virtual VCalendarAppointmentImportingEventArgs CreateImportingEventArgs(Appointment apt, VEvent ev) {
			return new VCalendarAppointmentImportingEventArgs(apt, ev);
		}
		protected internal virtual VCalendarAppointmentImportedEventArgs CreateImportedEventArgs(Appointment apt, VEvent ev) {
			return new VCalendarAppointmentImportedEventArgs(apt, ev);
		}
		protected internal virtual bool CanImportEvent(VCalendarAppointmentImportingEventArgs args) {
			return !IsTermination && !args.Cancel && args.VEvent != null;
		}
		protected internal virtual VCalendarAppointmentImportingEventArgs BeforeImport(VEvent ev, Appointment apt) {
			VCalendarAppointmentImportingEventArgs args = CreateImportingEventArgs(apt, ev);
			RaiseOnAppointmentImporting(args);
			return args;
		}
		protected internal virtual void AfterImport(Appointment apt, VEvent ev) {
			VCalendarAppointmentImportedEventArgs args = CreateImportedEventArgs(apt, ev);
			RaiseOnAppointmentImported(args);
		}
		protected Appointment CreateNewAppointment(VEvent ev) {
			return Storage.CreateAppointment(ev.GetAppointmentType());
		}
		protected void ImportEventCore(VEvent ev, Appointment apt) {
			CommitImport(apt, ev);
			if (apt.Type == AppointmentType.Pattern) {
				ImportExceptions(apt, ev);
			}
		}
		protected void ImportExceptions(Appointment pattern, VEvent vPattern) {
			DateTimeCollection exceptionDateTimes = vPattern.ExceptionDateTimes;
			int count = exceptionDateTimes.Count;
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				ImportExceptionByDateTime(pattern, vPattern, exceptionDateTimes[i]);
			}
			VRecurrenceRuleCollection exceptionRules = vPattern.ExceptionRules;
			count = exceptionRules.Count;
			for (int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				ImportExceptionByRule(pattern, vPattern, exceptionRules[i]);
			}
		}
		protected internal virtual void ImportExceptionByDateTime(Appointment pattern, VEvent vPattern, DateTime exceptionDateTime) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			int index = calc.FindOccurrenceIndex(exceptionDateTime, pattern);
			ImportException(pattern, vPattern, index);
		}
		protected internal virtual void ImportExceptionByRule(Appointment pattern, VEvent vPattern, VRecurrenceRule exRule) {
			Appointment calcPattern = PrepareCalculationPattern(pattern, exRule);
			AppointmentBaseCollection occurrences = GenerateOccurrences(calcPattern);
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(calcPattern.RecurrenceInfo);
			for (int i = 0; i < occurrences.Count; i++) {
				int index = calc.FindOccurrenceIndex(occurrences[i].Start, pattern);
				ImportException(pattern, vPattern, index);
			}
		}
		private Appointment PrepareCalculationPattern(Appointment srcPattern, VRecurrenceRule exceptionRule) {
			Appointment result = SchedulerUtils.CreateAppointmentInstance(Storage, AppointmentType.Pattern);
			result.Start = srcPattern.Start;
			result.End = srcPattern.End;
			VRecurrenceConvert conv = new VRecurrenceConvert();
			conv.AssignRecurrenceInfo(exceptionRule, result.RecurrenceInfo);
			return result;
		}
		AppointmentBaseCollection GenerateOccurrences(Appointment pattern) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			TimeInterval interval = CalcRecurrenceInterval(pattern);
			return calc.CalcOccurrences(interval, pattern);
		}
		TimeInterval CalcRecurrenceInterval(Appointment pattern) {
			return pattern.RecurrenceInfo.Range != RecurrenceRange.NoEndDate ? AppointmentCollection.CalcPatternInterval(pattern) :
				new TimeInterval(pattern.Start, TimeSpan.FromDays(365));
		}
		protected virtual void ImportException(Appointment pattern, VEvent vPattern, int recurrenceIndex) {
			if (recurrenceIndex < 0 || ExceptionExists(pattern, recurrenceIndex))
				return;
			Appointment ex = CreateException(pattern, recurrenceIndex);
			ImportDeletedOccurrence(vPattern, ex);
		}
		bool ExceptionExists(Appointment pattern, int index) {
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			for (int i = 0; i < exceptions.Count; i++) {
				if (exceptions[i].RecurrenceIndex == index)
					return true;
			}
			return false;
		}
		protected internal virtual void ImportDeletedOccurrence(VEvent ev, Appointment apt) {
			VCalendarAppointmentImportingEventArgs args = BeforeImport(ev, apt);
			if (CanImportEvent(args)) {
				AfterImport(apt, ev);
			}
			else {
				PerformDeleteAppointment(apt);
			}
		}
		protected Appointment CreateException(Appointment pattern, int recurrenceIndex) {
			return pattern.CreateException(AppointmentType.DeletedOccurrence, recurrenceIndex);
		}
		protected internal virtual void PerformDeleteAppointment(Appointment apt) {
			apt.Delete();
		}
	}
	public class VCalendarExporter : AppointmentExporter {
		ICollection appointments;
		string productId = VCalendarConsts.DefaultCalendarProductID;
		public VCalendarExporter(ISchedulerStorageBase storage)
			: base(storage) {
			AppointmentBaseCollection apts = new AppointmentBaseCollection();
			if (storage != null && storage.Appointments != null)
				apts.AddRange(storage.Appointments.Items);
			this.appointments = apts;
		}
		[Obsolete("You should use the 'new VCalendarExporter(SchedulerStorage storage, ICollection appointments)' instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		public VCalendarExporter(ICollection appointments)
			: base(null) {
		}
		public VCalendarExporter(ISchedulerStorageBase storage, ICollection appointments)
			: base(storage) {
			this.appointments = appointments;
		}
		public ICollection Appointments { get { return appointments; } set { appointments = value; } }
		public string ProductId { get { return productId; } set { productId = value; } }
		protected internal override void ExportCore(Stream stream) {
			if (Appointments == null || Appointments.Count == 0)
				return;
			using (MemoryStream memoryStream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(memoryStream)) {
					ExportAppointments(writer);
				}
				memoryStream.Close();
				byte[] bytes = memoryStream.ToArray();
				stream.Write(bytes, 0, bytes.Length);
			}
		}
		protected virtual void ExportAppointments(StreamWriter writer) {
			VCalendarEncoder encoder = new VCalendarEncoder(CreateVCalendar());
			encoder.Encode(writer);
		}
		protected VCalendarObject CreateVCalendar() {
			VCalendarObject calendar = new VCalendarObject(ProductId);
			foreach (Appointment apt in Appointments) {
				if (IsTermination)
					break;
				DoExportAppointment(calendar, apt);
			}
			return calendar;
		}
		protected internal virtual void DoExportAppointment(VCalendarObject calendar, Appointment apt) {
			VEvent ev = CreateNewEvent();
			InitEventProperties(apt, ev);
			VCalendarAppointmentExportingEventArgs args = BeforeExport(apt, ev);
			if (CanExportAppointment(args)) {
				ExportAppointmentCore(calendar, ev, apt);
			}
		}
		protected internal virtual VEvent CreateNewEvent() {
			return new VEvent();
		}
		protected internal virtual VCalendarAppointmentExportingEventArgs CreateExportingEventArgs(Appointment apt, VEvent ev) {
			return new VCalendarAppointmentExportingEventArgs(apt, ev);
		}
		protected internal virtual VCalendarAppointmentExportedEventArgs CreateExportedEventArgs(Appointment apt, VEvent ev) {
			return new VCalendarAppointmentExportedEventArgs(apt, ev);
		}
		protected internal virtual bool CanExportAppointment(VCalendarAppointmentExportingEventArgs args) {
			return !IsTermination && !args.Cancel && args.Appointment != null;
		}
		protected internal virtual VCalendarAppointmentExportingEventArgs BeforeExport(Appointment apt, VEvent ev) {
			VCalendarAppointmentExportingEventArgs args = CreateExportingEventArgs(apt, ev);
			RaiseOnAppointmentExporting(args);
			return args;
		}
		protected internal virtual void AfterExport(Appointment apt, VEvent ev) {
			VCalendarAppointmentExportedEventArgs args = CreateExportedEventArgs(apt, ev);
			RaiseOnAppointmentExported(args);
		}
		protected void ExportAppointmentCore(VCalendarObject calendar, VEvent ev, Appointment apt) {
			CommitExport(calendar, apt, ev);
			if (apt.Type == AppointmentType.Pattern) {
				ExportExceptions(calendar, apt, ev);
			}
		}
		protected internal virtual void CommitExport(VCalendarObject calendar, Appointment apt, VEvent ev) {
			calendar.EventList.Add(ev);
			AfterExport(apt, ev);
		}
		protected internal virtual void InitEventProperties(Appointment apt, VEvent ev) {
			ev.FromAppointment(apt);
		}
		protected void ExportExceptions(VCalendarObject calendar, Appointment pattern, VEvent ev) {
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			for (int i = 0; i < exceptions.Count; i++) {
				if (IsTermination)
					break;
				Appointment apt = exceptions[i];
				ExportException(calendar, pattern, apt, ev);
			}
		}
		protected void ExportException(VCalendarObject calendar, Appointment pattern, Appointment exception, VEvent vPattern) {
			DateTime dateTime = CalculateEventExceptionDateTime(pattern, exception.RecurrenceIndex);
			if (exception.Type == AppointmentType.ChangedOccurrence)
				ExportChangedOccurrence(calendar, exception, vPattern, dateTime);
			else
				ExportDeletedOccurrence(exception, vPattern, dateTime);
		}
		private DateTime CalculateEventExceptionDateTime(Appointment pattern, int index) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			return calc.CalcOccurrenceStartTime(index);
		}
		protected internal virtual void ExportChangedOccurrence(VCalendarObject calendar, Appointment apt, VEvent pattern, DateTime exceptionDateTime) {
			VEvent ev = CreateNewEvent();
			InitEventProperties(apt, ev);
			VCalendarAppointmentExportingEventArgs args = BeforeExport(apt, ev);
			if (CanExportAppointment(args)) {
				AddEventExceptionDateTime(pattern, exceptionDateTime);
				ExportAppointmentCore(calendar, ev, apt);
			}
		}
		protected internal virtual void ExportDeletedOccurrence(Appointment apt, VEvent pattern, DateTime exceptionDateTime) {
			VCalendarAppointmentExportingEventArgs args = BeforeExport(apt, pattern);
			if (CanExportAppointment(args)) {
				AddEventExceptionDateTime(pattern, exceptionDateTime);
				AfterExport(apt, pattern);
			}
		}
		protected void AddEventExceptionDateTime(VEvent pattern, DateTime exceptionDateTime) {
			pattern.ExceptionDateTimes.Add(exceptionDateTime);
		}
	}
	#region VCalendarAppointmentExportingEventArgs
	public class VCalendarAppointmentExportingEventArgs : AppointmentExportingEventArgs {
		VEvent ev;
		public VCalendarAppointmentExportingEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region VCalendarAppointmentExportedEventArgs
	public class VCalendarAppointmentExportedEventArgs : AppointmentExportedEventArgs {
		VEvent ev;
		public VCalendarAppointmentExportedEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region VCalendarAppointmentImportingEventArgs
	public class VCalendarAppointmentImportingEventArgs : AppointmentImportingEventArgs {
		VEvent ev;
		public VCalendarAppointmentImportingEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
	#region VCalendarAppointmentImportedEventArgs
	public class VCalendarAppointmentImportedEventArgs : AppointmentImportedEventArgs {
		VEvent ev;
		public VCalendarAppointmentImportedEventArgs(Appointment apt, VEvent ev)
			: base(apt) {
			if (ev == null)
				DevExpress.XtraScheduler.Native.Exceptions.ThrowArgumentException("ev", ev);
			this.ev = ev;
		}
		public VEvent VEvent { get { return ev; } }
	}
	#endregion
}
